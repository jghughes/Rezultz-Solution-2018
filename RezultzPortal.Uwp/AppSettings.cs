namespace RezultzPortal.Uwp
{
    public static class AppSettings
    {
        /* Note: Mvc is primarily intended for communication from back-end to
         * web browsers and phones, but it is also popular for Web API apps like we are doing here.
         * By contrast, Wcf is designed for machine-to-machine communication and is much
         * more powerful and secure. Mvc is easier to debug because it is text-based and
         * I can use tools like Swagger and Fiddler to debug. In Wcf, I communicate over
         * the wire in binary because that is the most performant approach. Wcf requires
         * a purpose-built client to debug unfortunately, which takes a lot of work.
         * In the grand scheme of things I am agnostic between Mvc and Wcf.
         * If you like, use Mvc for debugging and Wcf for production.
         */
        public static bool MustUseMvcNotWcfForRemoteServices { get; } = true;

        public static bool IsFirstTimeThroughOnLandingPage { get; set; } = true;

        public static int DesiredHeightOfShortListOfHubItemsDefault { get; set; } = 30; // keep this short for phones. don't want to allow latency to creep in

        /* Note: In the UWP app, MustSelectAllRacesOnFirstTimeThroughForAnEvent MUST remain FALSE
         * for the lifetime of the app, and the user must have no access to mess with it. In the
         * mobile app, it can be TRUE or FALSE and the user can be free to change it back and forth
         * during each session via user settings. In the user settings service, the implicit
         * default is FALSE, which is what we keep for the UWP app. In the mobile app, however, we override
         * the implicit default and change it to TRUE on the landing page upon commencement
         * of each launch of the app - merely because it is more aesthetically pleasing this way.
         */
        public static bool MustSelectAllRacesOnFirstTimeThroughForAnEvent { get; set; } = false;

        public const bool IsInDebugMode =
#if DEBUG
            true;
#else
            false;
#endif
    }
}
