namespace TestHarnessConsole
{
    public static class AppSettings
    {

        public static bool MustUseMvcNotWcfForRemoteServices { get; set; } = false; // 

        public const bool DebugMode =
#if DEBUG 
            true;
#else
            false;
#endif

        public const bool UITestMode =
#if IS_UI_TEST
            true;
#else
            false;
#endif

        public static string RootApiUrl { get; set; } = "__ENTER_YOUR_HTTPS_ROOT_API_URL_HERE__";

        public static string RootProductsWebApiUrl
        { get; set; } = "__ENTER_YOUR_HTTPS_ROOT_API_URL_FOR_COMPUTER_VISION_HERE__";
    }
}
