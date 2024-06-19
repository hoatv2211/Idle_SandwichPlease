using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using ThePattern.Extensions;
using ThePattern.Utils;

using UnityEngine;

namespace ThePattern
{
    public class HttpRequest
    {
        public enum Authorization
        {
            Basic,
            Bearer,
        }
        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);

        private readonly HttpClient _client;
        private HttpMethod _method;
        private Action<HttpResponse> _onCompleted;
        private Dictionary<string, string> _headers;
        private HttpRequest.ProgressChangedHandler _onProgressChanged;
        private string _destinationSavePath;
        private int _bufferSize = 8192;
        private long _bufferTimeOnChange = 3;

        public string BaseAddress { get; set; }
        public HttpResponseMessage Response { get; set; }
        public HttpRequest() => this._client = new HttpClient();

        public HttpRequest(string serverUrl)
        {
            this._client.BaseAddress = new Uri(serverUrl);
        }

        public HttpRequest SetAuth(HttpRequest.Authorization type, string auth, bool isBase64 = true)
        {
            string str = isBase64 ? Convert.ToBase64String(Encoding.ASCII.GetBytes(auth)) : auth;
            this._client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", string.Format("{0} {1}", type, str));
            return this;
        }

        public HttpRequest SetMethod(HttpMethod method)
        {
            this._method = method;
            return this;
        }

        public HttpRequest SetMethod(string methodName)
        {
            this._method = new HttpMethod(methodName);
            return this;
        }

        public HttpRequest SetTimeout(TimeSpan timeOut)
        {
            this._client.Timeout = timeOut;
            return this;
        }

        public HttpRequest SetHeaders(Dictionary<string, string> headers)
        {
            this._headers = headers;
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> header in headers)
                    this._client.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
            return this;
        }

        public HttpRequest SetOnComplete(Action<HttpResponse> callback)
        {
            this._onCompleted = callback;
            return this;
        }

        public HttpRequest SetOnProgress(HttpRequest.ProgressChangedHandler onProgress)
        {
            this._onProgressChanged = onProgress;
            return this;
        }

        public HttpRequest SetSavePath(string destinationSavePath)
        {
            this._destinationSavePath = destinationSavePath;
            return this;
        }

        public HttpRequest SetOnChangeBuffer(int bufferSize, long readTimeBuffer = 3)
        {
            this._bufferSize = bufferSize;
            this._bufferTimeOnChange = readTimeBuffer;
            return this;
        }

        public async void SendSync(string api) => await this.SendAsync(api);

        public async Task SendAsync(string api)
        {
            if (this._method == null)
                this._method = this._headers == null ? HttpMethod.Get : HttpMethod.Post;
            if (this._onProgressChanged == null)
            {
                HttpResponseMessage httpResponseMessage = await this._client.SendAsync(new HttpRequestMessage(this._method, api));
                this.Response = httpResponseMessage;
                httpResponseMessage = null;
                if (!string.IsNullOrWhiteSpace(this._destinationSavePath))
                    await this.DownloadFileFromHttpResponseMessage(this.Response);
            }
            else
            {
                HttpResponseMessage httpResponseMessage = await this._client.GetAsync(api, HttpCompletionOption.ResponseHeadersRead);
                this.Response = httpResponseMessage;
                httpResponseMessage = null;
                await this.DownloadFileFromHttpResponseMessage(this.Response);
            }

            this._onCompleted?.Invoke(new HttpResponse(this.Response));
        }

        public HttpRequest Execute(string api)
        {
            this.SendSync(api);
            return this;
        }

        public HttpRequest Execute(string api, Action<HttpResponse> callback) => this.SetOnComplete(callback).Execute(api);

        public HttpRequest ExecuteToModel<T>(string api, Action<HttpResponse, T> callback) => this.SetOnComplete(response =>
        {
            callback?.Invoke(response, response.IsSuccess ? response.Result.ToObject<T>() : default(T));
        }).Execute(api);

        private async Task DownloadFileFromHttpResponseMessage(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            string pathTemp = string.Empty;
            if (string.IsNullOrWhiteSpace(this._destinationSavePath) && !FileUtils.HasWritePermissionOnDir(Path.GetDirectoryName(this._destinationSavePath)))
            {
                pathTemp = this._destinationSavePath = Path.GetTempFileName();
                Debug.Log($"Created cache file: {pathTemp}");
            }
            long? totalBytes = response.Content.Headers.ContentLength;
            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                await this.ProcessContentStream(totalBytes, contentStream);
            if (string.IsNullOrWhiteSpace(pathTemp))
            {
                pathTemp = null;
            }
            else
            {
                File.Delete(pathTemp);
                Debug.Log($"Deleted cache file: {pathTemp}");
                pathTemp = null;
            }
        }

        private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
        {
            long totalBytesRead = 0;
            this._bufferTimeOnChange = 0;
            byte[] buffer = new byte[this._bufferSize];
            bool isMoreToRead = true;
            using (FileStream fileStream = new FileStream(this._destinationSavePath, FileMode.Create, FileAccess.Write, FileShare.None, this._bufferSize, true))
            {
                do
                {
                    int bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        this.TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                    }
                    else
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        totalBytesRead += (long)bytesRead;
                        ++this._bufferTimeOnChange;
                        if (this._bufferTimeOnChange % 3 == 0)
                            this.TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                    }
                }
                while (isMoreToRead);
            }
            buffer = null;
        }

        private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
        {
            if (this._onProgressChanged == null)
                return;
            double? progressPercentage = new double?();
            if (totalDownloadSize.HasValue)
                progressPercentage = new double?(Math.Round((double)totalBytesRead / (double)totalDownloadSize.Value * 100, 2));
            this._onProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
        }
    }
}