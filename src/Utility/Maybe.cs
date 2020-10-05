namespace Utility
{
    public class Maybe<T>
    {

        private readonly T value;

        public Maybe(T value)
        {
            this.value = value;
        }

        public T GetOrElse(T defaultValue)
        {
            return value == null ? defaultValue : value;
        }

    }

    public static class MaybeExtensions
    {

        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return new Maybe<T>(value);
        }

    }
}