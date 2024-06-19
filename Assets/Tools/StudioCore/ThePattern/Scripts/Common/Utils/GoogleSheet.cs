using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ThePattern
{
    public class GoogleSheet
    {
        private readonly string _googlesheetFileId;
        private readonly string _googlesheetSheetId;
        private readonly string _googleApiToken;
        private readonly List<Dictionary<string, string>> _csvData;
        private readonly List<string> _titles;

        private string URL => "https://docs.google.com/spreadsheets/d/" + this._googlesheetFileId + "/export?format=csv" + (string.IsNullOrWhiteSpace(this._googlesheetSheetId) ? string.Empty : "&gid=" + this._googlesheetSheetId);
        public int RowCount => this._csvData.Count;
        public int ColumnCount => this._titles.Count;

        public GoogleSheet(string apiToken, string fileId, string sheetId)
        {
            this._googleApiToken = apiToken;
            this._googlesheetFileId = fileId;
            this._googlesheetSheetId = sheetId;
            this._csvData = new List<Dictionary<string, string>>();
            this._titles = new List<string>();
        }

        public GoogleSheet Request(Action<bool, string, IList> onCompleted)
        {
            new HttpRequest().SetAuth(HttpRequest.Authorization.Bearer, this._googleApiToken).Execute(this.URL, response =>
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    onCompleted?.Invoke(false, response.Reason, null);
                }
                else
                {
                    try
                    {
                        string[] strArray1 = response.Result.Split(new string[3]
                        {
                            "\r\n",
                            "\n",
                            "\r"
                        }, StringSplitOptions.None);
                        if (strArray1.Length >= 1)
                        {
                            this._titles.AddRange(strArray1[0].Split(",".ToCharArray()));
                            for (int index1 = 1; index1 < strArray1.Length; ++index1)
                            {
                                string[] strArray2 = Regex.Replace(strArray1[index1], "(,)(?![^[]*\\])", ";").Split(";");
                                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                                for (int index2 = 0; index2 < this._titles.Count; ++index2)
                                    dictionary.Add(this._titles[index2], strArray2[index2]);
                                this._csvData.Add(dictionary);
                            }
                        }
                        onCompleted?.Invoke(true, response.Result, this._csvData);
                    }
                    catch (Exception ex)
                    {
                        onCompleted?.Invoke(false, ex.Message, null);
                    }
                }
            });
            return this;
        }

        public bool ContainsColumn(string columnName) => this._titles.Contains(columnName);

        public T GetValue<T>(string columnName, int rowIndex) => this.ContainsColumn(columnName) ? (T)Convert.ChangeType(this._csvData[rowIndex][columnName], typeof(T)) : default(T);

        public List<string> GetColumnData(string columnName)
        {
            List<string> columnData = new List<string>();
            foreach (Dictionary<string, string> dictionary in this._csvData)
            {
                if (dictionary.ContainsKey(columnName))
                    columnData.Add(dictionary[columnName]);
            }
            return columnData;
        }

        public List<string> GetColumnData(int index)
        {
            List<string> columnData = new List<string>();
            foreach (Dictionary<string, string> dictionary in this._csvData)
                columnData.Add(dictionary.Values.ToList<string>()[index]);
            return columnData;
        }

        public Dictionary<string, string> GetRowData(int index) => this._csvData.Count > index ? this._csvData[index] : new Dictionary<string, string>();
    }
}