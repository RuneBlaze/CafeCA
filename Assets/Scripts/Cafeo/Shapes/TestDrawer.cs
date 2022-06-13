using System;
using Drawing;
using UnityEngine;

namespace Cafeo.Shapes
{
    public class TestDrawer : MonoBehaviour
    {
        public void LateUpdate()
        {
            var draw = Draw.ingame;
            using (draw.InScreenSpace(Camera.main))
            {
                
            }
        }
    }
}