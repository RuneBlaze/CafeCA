using System;
using Drawing;
using UnityEngine;

namespace Cafeo.UI
{
    public class PartyHud : MonoBehaviour
    {
        public void Update()
        {
            var draw = Draw.ingame;
            using (draw.InScreenSpace(Camera.main))
            {
                draw.SolidRectangle(new Rect(0, 0, 100, 20), Color.red);
            }
        }
    }
}