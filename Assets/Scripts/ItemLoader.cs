using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;
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
        GetComponentInParent<ScrollRect>()
            .onValueChanged
            .AddListener(vector =>
                StartCoroutine(
                    LoadMore(vector,
                        new YleApiClient(baseUrl, appId, appKey))));
        content = transform.Find("Viewport").Find("Content");
    }

    public IEnumerator LoadMore(Vector2 vector, IYleApiClient searcher)
    {
        if (!loadInProgress && !string.IsNullOrWhiteSpace(currentText) && vector.y <= 0.1)
        {
            Debug.Log("Searching more items...");

            yield return StartCoroutine(SearchItems(searcher));
        }
    }

    public void Search(string text) =>
        StartCoroutine(CoSearch(text,
            new YleApiClient(baseUrl, appId, appKey)));

    public IEnumerator CoSearch(string text, IYleApiClient searcher)
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

                yield return StartCoroutine(SearchItems(searcher));
            }
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

    private IEnumerator SearchItems(IYleApiClient searcher)
    {
        if (!loadInProgress)
            try
            {
                loadInProgress = true;

                Debug.Log($"Searching \"{currentText}\", offset = {content.childCount}...");

                yield return searcher.Search(currentText, content.childCount, loadCount);

                if (!searcher.Success)
                    Debug.LogError(searcher.Error);
                else
                {
                    Debug.Log($"Success, found {searcher.Result.Count} items.");

                    foreach (var item in searcher.Result)
                    {
                        var instance = Instantiate(itemPrefab.gameObject);
                        instance.transform.SetParent(content, false);
                        InitializeItemView(instance, item);
                    }
                }
            }
            finally
            {
                loadInProgress = false;
            }
    }
}
