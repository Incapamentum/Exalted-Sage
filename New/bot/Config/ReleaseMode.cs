namespace Bot.Config
{
    /// <summary>
    ///     Catch-all solution on the type of release application has been
    ///     built for.
    /// </summary>
    public static class ReleaseMode
    {
        public static string Mode
        {
            get
            {
                #if DEBUG
                    return "DevSettings";
                #else
                    return "ProdSettings";
                #endif
            }
        }
    }
}
