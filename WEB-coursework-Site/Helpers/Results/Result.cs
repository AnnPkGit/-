namespace WEB_coursework_Site.Helpers.Results
{
    public class Result<T>
    {
        public bool IsSuccessful { get; private set; }

        public T? Value { get; private set; }

        public string Message { get; private set; }

        public Result(bool isSuccessfull, T value, string message)
        {
            IsSuccessful = isSuccessfull;
            Value = value;
            Message = message;
        }

        public Result(bool isSuccessfull, string message)
        {
            IsSuccessful = isSuccessfull;
            Message = message;
        }
    }

    public class Result
    {
        public bool IsSuccessful { get; private set; }

        public Result(bool isSuccessfull)
        {
            IsSuccessful = isSuccessfull;
        }
    }
}