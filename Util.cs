using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nuVector2 = System.Numerics.Vector2;
using nuVector4 = System.Numerics.Vector4;


namespace MapNotify
{
    public partial class MapNotify
    {
        public nuVector4 HexToVector4(string value)
        {

            uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var abgr);
            Color color = Color.FromAbgr(abgr);
            return new nuVector4((color.R / (float)255), color.G / (float)255, color.B / (float)255, color.A / (float)255);
        }
        public Vector4 HexToSDXVector4(string value)
        {

            uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var abgr);
            Color color = Color.FromAbgr(abgr);
            return new Vector4((color.R / (float)255), color.G / (float)255, color.B / (float)255, color.A / (float)255);
        }
        public Vector4 NuToSharpVec4(nuVector4 value)
        {
            return new Vector4(value.X, value.Y, value.Z, value.W);
        }
        public nuVector4 SharpToNuVec4(Vector4 value)
        {
            return new nuVector4(value.X, value.Y, value.Z, value.W);
        }
        public Vector2 NuToSharpVec2(nuVector2 value)
        {
            return new Vector2(value.X, value.Y);
        }
        public nuVector2 SharpToNuVec2(Vector2 value)
        {
            return new nuVector2(value.X, value.Y);
        }
    }
}
