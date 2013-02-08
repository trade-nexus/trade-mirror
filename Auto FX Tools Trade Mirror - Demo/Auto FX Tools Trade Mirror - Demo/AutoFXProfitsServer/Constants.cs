using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoFXProfitsServer
{
    public static class Constants
    {
        #region Email Constants

        public const string EmailSubjectGeneric = "AutoFX Profits Trade Alert";
        public const string EmailSubjectEntry = "Entry in <Symbol>";
        public const string EmailSubjectPartial = "Partial Profit Taken in <Symbol> Trade";
        public const string EmailSubjectStopAdjustment = "Stop Adjustment in <Symbol> Trade";
        public const string EmailSubjectExit = "Trade Exited in <Symbol>";
        
        #endregion
    }
}
