using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ItemView
    {
        public Text titleText;
        private readonly RectTransform rectTransform;
        private bool expanded;
        private readonly Animator animator;

        public ItemView(GameObject panel, ItemModel model)
        {
            rectTransform = panel.GetComponent<RectTransform>();

            panel.GetComponent<Button>().onClick.AddListener(OnClick);

            animator = panel.GetComponent<Animator>();

            panel.transform.Find("TitleText").GetComponent<Text>().text = model.title ?? "Ei suomenkielistä nimeä";

            var detailsPanel = panel.transform.Find("DetailsPanel");

            detailsPanel.Find("SeriesText").GetComponent<Text>().text = model.series;
            detailsPanel.Find("SubjectText").GetComponent<Text>().text = model.subject;
            detailsPanel.Find("AudioText").GetComponent<Text>().text = model.audio;
            detailsPanel.Find("FromText").GetComponent<Text>().text = model.from;
            detailsPanel.Find("ToText").GetComponent<Text>().text = model.to;
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
