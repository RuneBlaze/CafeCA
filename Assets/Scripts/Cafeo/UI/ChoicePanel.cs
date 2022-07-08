using System;
using System.Linq;
using Cafeo.Utils;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cafeo.UI
{
    public class ChoicePanel : Singleton<ChoicePanel>, IPointerEnterHandler, IPointerExitHandler
    {
        private string[] choices;
        private Action<int> onChoice;
        private bool mouseOver = false;

        private float liveSeconds;

        [SerializeField] private Transform contents;

        protected override void Setup()
        {
            base.Setup();
            Assert.IsNotNull(contents);
            choices = new string[] {};
        }

        public void Summon(string[] choices, Action<int> onChoice)
        {
            this.choices = choices;
            this.onChoice = onChoice;
            gameObject.SetActive(true);
            Refresh();
            liveSeconds = 2f;
            MoveToMousePos();
        }

        public void Summon((string, Action)[] choices)
        {
            var arr = choices.Select(it => it.Item1).ToArray();
            Summon(arr, i => choices[i].Item2.Invoke());
        }

        private void Update()
        {
            if (mouseOver)
            {
                liveSeconds = 2f;
            }
            liveSeconds -= Time.deltaTime;
            if (liveSeconds <= 0)
            {
                liveSeconds = 0;
                gameObject.SetActive(false);
            }
        }

        private void Refresh()
        {
            var choiceButtonTemplate = WorldUIManager.Instance.choiceButtonTemplate;
            // remove children under contents
            foreach (Transform child in contents)
            {
                Destroy(child.gameObject);
            }

            int i = 0;
            foreach (var choice in choices)
            {
                var go = Instantiate(choiceButtonTemplate, contents);
                var button = go.GetComponent<Button>();
                var text = go.GetComponentInChildren<Text>();
                text.text = choice;
                var index = i;
                button.onClick.AddListener(() => onChoice.Invoke(index));
                i++;
            }
        }

        private void MoveToMousePos()
        {
            // move to mouse pos on this Canvas
            transform.position = Input.mousePosition;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mouseOver = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mouseOver = false;
        }
    }
}