namespace saper1.Data
{
    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }


        public static bool operator ==(Coordinates? left, Coordinates? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.X == right.X && left.Y == right.Y;
        }

        public static bool operator !=(Coordinates? left, Coordinates? right)
        {
            return !(left == right);
        }
    }
}
