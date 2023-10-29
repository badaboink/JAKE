using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using JAKE.classlibrary.Patterns;
using JAKE.client;

namespace UI_tests
{
    public class BuilderTests
    {
        [Fact]
        public async Task Build_PlayerVisualBuilder()
        {
            var msApplication = JakeClientHelper.LaunchJakeClient();
            var automation = new UIA3Automation();
            var mainWindow = msApplication.GetMainWindow(automation);
            msApplication.Close();
        }
    }
}
