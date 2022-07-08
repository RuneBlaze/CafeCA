using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Cafeo.UI
{
    public class SimpleChoiceInvoker : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private string[] choices;
        [SerializeField] private UnityEvent[] callbacks;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            var choicePanel = ChoicePanel.Instance;
            choicePanel.Summon(choices, i => callbacks[i].Invoke());
        }
    }
}