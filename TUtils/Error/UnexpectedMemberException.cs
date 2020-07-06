namespace TUtils.Error
{
    /// <summary>
    /// To be thrown when an unexpected item is presented or not known about.
    /// </summary>
    public class UnexpectedMemberException : System.Exception
    {
        public UnexpectedMemberException(string message) : base(message)
        {
        }

        public UnexpectedMemberException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}