using SharpDX;
using nuVector4 = System.Numerics.Vector4;


namespace MapNotify
{
    public partial class MapNotify
    {
        public nuVector4 HexToVector4(string value)
        {
            uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var abgr);
            var color = Color.FromAbgr(abgr);
            return new nuVector4((color.R / (float)255), color.G / (float)255, color.B / (float)255, color.A / (float)255);
        }
        public Vector4 HexToSDXVector4(string value)
        {

            uint.TryParse(value, System.Globalization.NumberStyles.HexNumber, null, out var abgr);
            var color = Color.FromAbgr(abgr);
            return new Vector4((color.R / (float)255), color.G / (float)255, color.B / (float)255, color.A / (float)255);
        }
        public Vector4 NuToSharp(nuVector4 value)
        {
            return new Vector4(value.X, value.Y, value.Z, value.W);
        }
        public nuVector4 SharpToNu(Vector4 value)
        {
            return new nuVector4(value.X, value.Y, value.Z, value.W);
        }
        public uint ColorToUint(nuVector4 value)
        {
            var rVal = (int)(value.X * 255f);
            rVal |= (int)(value.Y * 255f) << 8;
            rVal |= (int)(value.Z * 255f) << 16;
            rVal |= (int)(value.W * 255f) << 24;
            return (uint)rVal;
        }
    }
}
