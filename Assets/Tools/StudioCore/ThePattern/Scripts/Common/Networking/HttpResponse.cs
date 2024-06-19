using System.Net;
using System.Net.Http;

public class HttpResponse
{
    public HttpResponseMessage Origin { get; private set; }

    public HttpStatusCode StatusCode { get; private set; }

    public string Reason { get; private set; }

    public string Result { get; private set; }

    public bool IsSuccess { get; private set; }

    public HttpResponse(HttpResponseMessage httpResponse)
    {
        this.Origin = httpResponse;
        this.StatusCode = httpResponse.StatusCode;
        this.Reason = httpResponse.ReasonPhrase;
        this.Result = httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        this.IsSuccess = httpResponse.IsSuccessStatusCode;
    }
}
