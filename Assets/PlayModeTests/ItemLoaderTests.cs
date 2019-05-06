using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace Tests
{
    public class ItemLoaderTests
    {
        private Mock<IYleApiClient> yleClientMock;
        private ItemLoader loader;
        private GameObject content;

        [SetUp]
        public void Setup()
        {
            yleClientMock = new Mock<IYleApiClient>();

            yleClientMock.Setup(c => c.Success).Returns(true);
            yleClientMock.Setup(c => c.Search("muumi", 0, 1))
                .Returns(new object[] { null }.GetEnumerator());
            yleClientMock.Setup(c => c.Result).Returns(new[]
            {
                new ItemModel
                {
                    title = "a title",
                    subject = "a subject",
                    series=  "a series",
                    audio = "en"
                }
            });

            var detailsPanel = new GameObject("DetailsPanel");
            var itemPrefab = new GameObject();

            itemPrefab.AddComponent<Button>();
            detailsPanel.transform.SetParent(itemPrefab.transform);

            AddText(itemPrefab, "TitleText");
            AddText(detailsPanel, "SeriesText");
            AddText(detailsPanel, "SubjectText");
            AddText(detailsPanel, "AudioText");
            AddText(detailsPanel, "FromText");
            AddText(detailsPanel, "ToText");

            var scrollView = new GameObject();
            var viewport = new GameObject("Viewport");

            content = new GameObject("Content");
            viewport.transform.SetParent(scrollView.transform);
            content.transform.SetParent(viewport.transform);

            scrollView.AddComponent<ScrollRect>();
            loader = scrollView.AddComponent<ItemLoader>();

            loader.itemPrefab = itemPrefab.transform;
            loader.baseUrl = "https://external.api.yle.fi";
            loader.loadCount = 1;
            loader.appId = "c6d65741";
            loader.appKey = "6a8b448b276a88171bad6f22a059b6cd";
            loader.Start();
        }

        private void AddText(GameObject obj, string name)
        {
            var childObj = new GameObject(name);

            childObj.AddComponent<Text>();
            childObj.transform.SetParent(obj.transform);
        }

        [UnityTest]
        public IEnumerator ShouldCreateChildElements()
        {
            yield return loader.CoSearch("muumi", yleClientMock.Object);

            Assert.Greater(content.transform.childCount, 0,
                "The contant pane should have children.");

            Assert.Greater(content
                .transform
                .GetChild(0)
                .Find("TitleText")
                .GetComponent<Text>()
                .text
                .Length,
                0,
                "Title text should not be empty");
        }

        [UnityTest]
        public IEnumerator ShouldFillTitleText()
        {
            yield return loader.CoSearch("muumi", yleClientMock.Object);

            Assert.Greater(content
                .transform
                .GetChild(0)
                .Find("TitleText")
                .GetComponent<Text>()
                .text
                .Length,
                0,
                "Title text should not be empty");
        }

        [UnityTest]
        [TestCase("SeriesText", ExpectedResult = "")]
        [TestCase("SubjectText", ExpectedResult = "")]
        [TestCase("AudioText", ExpectedResult = "")]
        public IEnumerator ShouldFillDetailsText(string fileName)
        {
            yield return loader.CoSearch("muumi", yleClientMock.Object);

            Assert.Greater(content
                .transform
                .GetChild(0)
                .Find("DetailsPanel")
                .Find(fileName)
                .GetComponent<Text>()
                .text
                .Length,
                0,
                "Title text should not be empty");
        }
    }
}
