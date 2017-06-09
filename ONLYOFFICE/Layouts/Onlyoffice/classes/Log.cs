using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Administration;

namespace Onlyoffice
{
    public class Log : SPDiagnosticsServiceBase
    {
        public static string DiagnosticAreaName = "SharePoint ONLYOFFICE";
        private static Log _Current;
        public static Log Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new Log();
                }

                return _Current;
            }
        }

        private Log()
            : base("ONLYOFFICE Logging Service", SPFarm.Local)
        {
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
        {
            new SPDiagnosticsArea(DiagnosticAreaName, new List<SPDiagnosticsCategory>
            {
                new SPDiagnosticsCategory("ONLYOFFICE", 
                            TraceSeverity.High, EventSeverity.Error)
            })
        };
            return areas;
        }

        public static void LogError(string errorMessage)
        {
            SPDiagnosticsCategory category =
                 Log.Current
                               .Areas[DiagnosticAreaName]
                               .Categories["ONLYOFFICE"];

            Log.Current.WriteTrace(0, category,
                  TraceSeverity.High, errorMessage);
        }
    }
}
