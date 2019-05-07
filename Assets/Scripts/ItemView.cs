using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ItemView
    {
        private readonly RectTransform rectTransform;
        private bool expanded;
        private readonly Animator animator;
        private readonly Text titleText, seriesText, subjectText, audioText, fromText, toText;

        public ItemView(GameObject panel)
        {
            rectTransform = panel.GetComponent<RectTransform>();

            panel.GetComponent<Button>().onClick.AddListener(OnClick);

            animator = panel.GetComponent<Animator>();

            titleText = panel.transform.Find("TitleText").GetComponent<Text>();

            var detailsPanel = panel.transform.Find("DetailsPanel");

            seriesText = detailsPanel.Find("SeriesText").GetComponent<Text>();
            subjectText = detailsPanel.Find("SubjectText").GetComponent<Text>();
            audioText = detailsPanel.Find("AudioText").GetComponent<Text>();
            fromText = detailsPanel.Find("FromText").GetComponent<Text>();
            toText = detailsPanel.Find("ToText").GetComponent<Text>();
        }

        public void SetTexts(ItemModel model)
        {
            titleText.text = model.title;
            seriesText.text = model.series;
            subjectText.text = model.subject;
            audioText.text = model.audio;
            fromText.text = model.from;
            toText.text = model.to;
        }

        public event Action OnExpand;

        private void OnClick()
        {
            if (expanded)
                animator.Play("Collapse");
            else
            {
                animator.Play("Expand");
                OnExpand?.Invoke();
            }

            expanded = !expanded;
        }

        public void Collapse()
        {
            if (expanded)
            {
                animator.Play("Collapse");
                expanded = false;
            }
        }
    }
}
