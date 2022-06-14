using System;

namespace Cafeo
{
    [Serializable]
    public class BaseAttrs
    {
        public float str;
        public float con;
        public float dex;
        public float per;
        public float lea;
        public float wil;
        public float mag;
        public float cut;
        public float awe;
        public float life;
        public float mana;

        public BaseAttrs()
        {
            // initialize all attributes to be 10
            str = 10;
            con = 10;
            dex = 10;
            per = 10;
            lea = 10;
            wil = 10;
            mag = 10;
            cut = 10;
            awe = 10;
            life = 30;
            mana = 10;
        }

        public void CopyTo(float[] target)
        {
            target[0] = str;
            target[1] = con;
            target[2] = dex;
            target[3] = per;
            target[4] = lea;
            target[5] = wil;
            target[6] = mag;
            target[7] = cut;
            target[8] = awe;
            target[9] = life;
            target[10] = mana;
        }

        public void CopyFrom(float[] source)
        {
            str = source[0];
            con = source[1];
            dex = source[2];
            per = source[3];
            lea = source[4];
            wil = source[5];
            mag = source[6];
            cut = source[7];
            awe = source[8];
            life = source[9];
            mana = source[10];
        }
    }
}