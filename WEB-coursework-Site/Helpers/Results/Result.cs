namespace WEB_coursework_Site.Helpers.Results
{
    public class Result<T>
    {
        public bool IsSuccessfull { get; private set; }

        public T Value { get; private set; }

        public Result(bool isSuccessfull, T value)
        {
            IsSuccessfull = isSuccessfull;
            Value = value;
        }
    }

    public class Result
    {
        public bool IsSuccessfull { get; private set; }

        public Result(bool isSuccessfull)
        {
            IsSuccessfull = isSuccessfull;
        }
    }
}