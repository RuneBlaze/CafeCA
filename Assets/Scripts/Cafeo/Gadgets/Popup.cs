using System;
using DG.Tweening;
using UnityEngine;

namespace Cafeo.Gadgets
{
    public class Popup : MonoBehaviour
    {
        [SerializeField] private TextMesh textMesh;
        public Color textColor;

        public void Start()
        {
            textMesh.color = textColor;
            transform.DOMoveY(transform.position.y + 1, 1.2f).SetEase(Ease.OutBack);
            DOTween.To(() => textMesh.color, 
                    x => textMesh.color = x, 
                    new Color(textColor.r, textColor.b, textColor.b, 0), 1.21f)
                .OnComplete(() => Destroy(gameObject));
        }
        
        public void SetText(string text)
        {
            textMesh.text = text;
        }

    }
}