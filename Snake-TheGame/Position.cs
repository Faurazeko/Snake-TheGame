namespace Snake_TheGame
{
    public class Position : IEquatable<Position>
    {
        public int X;
        public int Y;

        public bool Equals(Position? other)
        {

            if (other is null)
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            if (this.GetType() != other.GetType())
                return false;

            return (X == other.X) && (Y == other.Y);
        }

        public static bool operator ==(Position left, Position right)
        {
            if (left is null)
            {
                if (left is null)
                    return true;

                return false;
            }
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right) => !(left == right);
    }
}
