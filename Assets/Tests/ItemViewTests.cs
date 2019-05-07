using Assets.Scripts;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Tests
{
    public class ItemViewTests
    {
        private Text titleText;
        private ItemView view;

        [SetUp]
        public void Setup()
        {
            var itemPanel = new GameObject();
            var detailsPanel = new GameObject("DetailsPanel");

            itemPanel.AddComponent<Button>();
            detailsPanel.transform.SetParent(itemPanel.transform);

            titleText = AddText(itemPanel, "TitleText");
            AddText(detailsPanel, "SeriesText");
            AddText(detailsPanel, "SubjectText");
            AddText(detailsPanel, "AudioText");
            AddText(detailsPanel, "FromText");
            AddText(detailsPanel, "ToText");

            var scrollView = new GameObject();
            var viewport = new GameObject("Viewport");

            view = new ItemView(itemPanel);
        }

        private Text AddText(GameObject obj, string name)
        {
            var childObj = new GameObject(name);
            var text = childObj.AddComponent<Text>();

            childObj.transform.SetParent(obj.transform);

            return text;
        }

        [Test]
        public void ShouldSetTitleText()
        {
            view.SetTexts(new ItemModel
            {
                title = "test123"
            });

            Assert.AreEqual(titleText.text, "test123", "Should set title text");
        }
    }
}
