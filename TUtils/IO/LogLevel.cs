namespace TUtils.IO
{
    /// <summary>
    /// Controls the level of a logged statement.
    /// </summary>
    public enum LogLevel
    {
        Error, //For exceptions and their data
        Warning, //For information that might be a warning that something isn't quite right, but won't crash or cause permenent harm
        Information, //Useful or interesting information
        Debug, //Debug level information. Anytime you feel like being verbose about things.
        None //We don't want to have a prefix for some reason.
    }
}