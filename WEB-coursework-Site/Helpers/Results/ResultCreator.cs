namespace WEB_coursework_Site.Helpers.Results
{
    static class ResultCreator<T>
    {
        static public Result<T> CreateFailedResult(T value)
        {
            return new Result<T>(false, value);
        }
        static public Result<T> CreateSuccessfulResult(T value)
        {
            return new Result<T>(true, value);
        }
    }

    static class ResultCreator
    {
        static public Result CreateFailedResult()
        {
            return new Result(false);
        }
        static public Result CreateSuccessfulResult()
        {
            return new Result(true);
        }
    }
}