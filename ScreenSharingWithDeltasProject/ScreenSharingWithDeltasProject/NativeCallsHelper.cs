using System;
using System.Collections.Generic;

namespace ScreenSharingWithDeltasProject
{
    public class NativeCallsHelper
    {
        public static Dictionary<IntPtr, string> AppsAndTitles = new Dictionary<IntPtr, string>();
        public static Dictionary<IntPtr, string> GetValidApplicationList()
        {
            return AppsAndTitles;
        }
    }
}