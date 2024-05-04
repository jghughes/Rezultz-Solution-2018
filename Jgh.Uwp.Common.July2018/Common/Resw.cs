using System;
using Windows.ApplicationModel.Resources.Core;

namespace Jgh.Uwp.Common.July2018.Common
{
    public static class Resw
    {
        public static string GetString(string subTree, string key)
        {
            // Two coding patterns will be used:
            //   1. Get a ResourceContext on the UI thread using GetForCurrentView and pass 
            //      to the non-UI thread
            //   2. Get a ResourceContext on the non-UI thread using GetForViewIndependentUse
            //
            // Two analogous patterns could be used for ResourceLoader instead of ResourceContext.

            string answer;

            // pattern 1: get a ResourceContext for the UI thread
            try
            {
                if (string.IsNullOrWhiteSpace(subTree))
                    return "empty text resources file path";

                if (string.IsNullOrWhiteSpace(key))
                    return "empty text resource key";


                var defaultContextForUiThread = ResourceContext.GetForCurrentView();

                var stringResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree(subTree);

                answer = stringResourceMap.GetValue(key, defaultContextForUiThread).ValueAsString;

            }
            catch (Exception)
            {
                answer = $"missing text: '{subTree}' '{key}]";
            }
            return answer;
        }

    }
}