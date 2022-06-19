namespace Bot.Config
{
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
