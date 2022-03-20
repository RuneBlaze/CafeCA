using UnityEngine;

namespace Cafeo
{
    public abstract class AbstractItem
    {
        public Texture icon;
        public string name;
        public string description;
        private int exp;
        public int k;
        public int b;

        public int Level
        {
            get
            {
                int x = 0;
                while (exp < Mathf.Pow(b, x) + k)
                {
                    x++;
                }

                return x + 1;
            }
        }
    }
}