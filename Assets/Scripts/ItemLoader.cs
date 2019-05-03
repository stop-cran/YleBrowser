using Assets.Scripts;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ItemLoader : MonoBehaviour
{
    public Transform itemPrefab;
    public string baseUrl;
    public int loadCount;
    public string appId;
    public string appKey;

    private Transform content;
    private bool loadInProgress;
    private string currentText;
    private ItemView expandedView;

    public void Start()
    {
        GetComponentInParent<ScrollRect>().onValueChanged.AddListener(LoadMore);
        content = transform.Find("Viewport").Find("Content");
    }

    public void LoadMore(Vector2 vector)
    {
        if (!loadInProgress && !string.IsNullOrWhiteSpace(currentText) && vector.y <= 0.1)
        {
            Debug.Log("Searching more items...");

            StartCoroutine(
                SearchItems(results =>
                    OnReceivedModels(results)));
        }
    }

    public void Search(string text)
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        content.DetachChildren();
        expandedView = null;

        if (string.IsNullOrWhiteSpace(text))
            currentText = null;
        else
        {
            var newText = Uri.EscapeDataString(text.Trim());

            if (newText != currentText)
            {
                currentText = newText;

                StartCoroutine(
                    SearchItems(results =>
                        OnReceivedModels(results)));
            }
        }
    }

    private void OnReceivedModels(ItemModel[] items)
    {
        foreach (var model in items)
        {
            var instance = Instantiate(itemPrefab.gameObject);
            instance.transform.SetParent(content, false);
            InitializeItemView(instance, model);
        }
    }

    private void InitializeItemView(GameObject instance, ItemModel model)
    {
        var view = new ItemView(instance,
            model,
            thisView =>
            {
                expandedView?.Collapse();

                expandedView = thisView;
            });
    }

    private IEnumerator SearchItems(Action<ItemModel[]> callback)
    {
        if (!loadInProgress)
            try
            {
                loadInProgress = true;

                Debug.Log($"Searching \"{currentText}\", offset = {content.childCount}...");

                using (var request = UnityWebRequest.Get($"{baseUrl}/v1/programs/items.json?app_id={appId}&app_key={appKey}&limit={loadCount}&offset={content.childCount}&language=fi&q={currentText}"))
                {
                    yield return request.SendWebRequest();

                    if (request.isNetworkError)
                        Debug.LogError(request.error);
                    else
                    {
                        // RK TODO: wait for request.downloadHandler.isDone
                        var root = JsonUtility.FromJson<YleModel>(request.downloadHandler.text);

                        Debug.Log($"Success, found {root.data[0].subject.Last().title.fi} items.");

                        callback((from data in root.data
                                  let publicationEvent = data.publicationEvent.Count == 1 ? data.publicationEvent[0] : null
                                  select new ItemModel
                                  {
                                      title = data.title?.fi,
                                      subject = string.Join(", ",
                                          data.subject
                                              .Select(s => s.title?.fi?.Trim())
                                              .Where(s => !string.IsNullOrEmpty(s))),
                                      series = data.partOfSeries?.title?.fi,
                                      audio = string.Join(", ", data.audio.SelectMany(a => a.language)),
                                      @from = FormatDataTime(publicationEvent?.startTime),
                                      to = FormatDataTime(publicationEvent?.endTime)
                                  }).ToArray());
                    }
                }
            }
            finally
            {
                loadInProgress = false;
            }
    }

    private static string FormatDataTime(string dt) =>
        dt != null && DateTime.TryParse(dt, out var parsed)
            ? parsed.ToString()
            : null;
}
