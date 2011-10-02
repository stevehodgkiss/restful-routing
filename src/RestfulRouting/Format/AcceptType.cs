using System;

namespace RestfulRouting.Format
{
    public class AcceptType : IComparable
    {
        public string Type { get; set; }
        public float Quality { get; set; }
        public int Order { get; set; }

        public AcceptType(string type, float quality, int order)
        {
            Type = type;
            Quality = quality;
            if (type == "*/*")
                Quality = 0;
            Order = order;
        }

        public int CompareTo(object obj)
        {
            var other = (AcceptType)obj;
            var result = other.Quality.CompareTo(Quality);
            if (result == 0)
                result = Order.CompareTo(other.Order);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return false;
            if (object.ReferenceEquals(this, obj)) return true;
            var other = (AcceptType) obj;
            return Type.Equals(other.Type);
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", Type, Quality, Order);
        }
    }
}