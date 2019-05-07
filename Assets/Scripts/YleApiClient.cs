using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class YleApiClient : IYleApiClient
    {
        private readonly string baseUrl;
        private readonly string appId;
        private readonly string appKey;

        public YleApiClient(string baseUrl, string appId, string appKey)
        {
            this.baseUrl = baseUrl;
            this.appId = appId;
            this.appKey = appKey;
        }

        public bool Success { get; private set; }
        public string Error { get; private set; }
        public IReadOnlyList<ItemModel> Result { get; private set; }

        public IEnumerator Search(string query, int offset, int limit)
        {
            using (var request = UnityWebRequest.Get($"{baseUrl}/v1/programs/items.json?app_id={appId}&app_key={appKey}&limit={limit}&offset={offset}&language=fi&q={query}"))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Error = request.error;
                    Success = false;
                }
                else
                    try
                    {
                        var root = JsonUtility.FromJson<YleModel>(request.downloadHandler.text);

                        Result = (from data in root.data
                                  let publicationEvent = data.publicationEvent.Count == 1 ? data.publicationEvent[0] : null
                                  select new ItemModel
                                  {
                                      title = data.title?.fi ?? "--No title--",
                                      subject = string.Join(", ",
                                          data.subject
                                              .Select(s => s.title?.fi?.Trim())
                                              .Where(s => !string.IsNullOrEmpty(s))),
                                      series = data.partOfSeries?.title?.fi,
                                      audio = string.Join(", ", data.audio.SelectMany(a => a.language)),
                                      @from = FormatDataTime(publicationEvent?.startTime),
                                      to = FormatDataTime(publicationEvent?.endTime)
                                  }).ToList()
                                  .AsReadOnly();

                        Success = true;
                    }
                    catch (Exception ex) // RK TODO: Catch only related exception types.
                    {
                        Error = ex.Message;
                        Success = false;
                    }
            }
        }

        private static string FormatDataTime(string dt) =>
            dt != null && DateTime.TryParse(dt, out var parsed)
                ? parsed.ToString()
                : null;
    }
}
