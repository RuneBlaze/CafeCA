using UnityEngine;

namespace Cafeo
{
    public abstract class AbstractItem
    {
        public int b;
        public string description;
        private int exp;
        public Sprite icon;
        public int k;
        public string name;

        public int Level
        {
            get
            {
                var x = 0;
                while (exp < Mathf.Pow(b, x) + k) x++;

                return x + 1;
            }
        }
    }
}