using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BLL.Drawing
{
    public class ColourLookUp
    {
        Int32[] _ColourList;
        float[] ScaleList = new float[12];

        public ColourLookUp(float Scale, float Offset, Int32[] ColourList)
        {
            _ColourList = ColourList;

            float top = Offset + (Scale / 2) + (Scale * 5);
            for (int i = 0; i < 12; i++)
            {
                ScaleList[i] = float.Parse(((float)(top - (Scale * i))).ToString("0.0"));
            }

        }

        public Color ScaledColour(float data)
        {
            for (int i = 0; i < 12; i++)
            {
                if (data > ScaleList[i])
                {
                    return Color.FromArgb(_ColourList[i]);
                }
            }
            if (data < ScaleList[11])
            {
                return Color.FromArgb(_ColourList[12]);
            }


            return Color.FromArgb(_ColourList[13]);
        }
    }
}
