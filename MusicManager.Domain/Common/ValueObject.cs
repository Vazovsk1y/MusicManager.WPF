namespace MusicManager.Domain.Common
{
    public abstract class ValueObject<T> where T : ValueObject<T>
    {
        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            if (obj is not T valueObject)
                return false;

            return GetEqualityComponents().SequenceEqual(valueObject.GetEqualityComponents());
        }

        public override int GetHashCode() =>
            GetEqualityComponents().Aggregate(1, (current, obj) =>
            {
                unchecked
                {
                    return current * 44 + (obj?.GetHashCode() ?? 0);
                }
            });

        public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(ValueObject<T> a, ValueObject<T> b) => !(a == b);

        protected abstract IEnumerable<object> GetEqualityComponents();
    }
}
