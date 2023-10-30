using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Threading;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Input;
using FlaUI.Core.Tools;
using FlaUI.UIA3;
using JAKE.classlibrary;
using JAKE.classlibrary.Patterns;
using JAKE.client;
using System.Windows.Media;
using JAKE.Client;
using JAKE.classlibrary.Enemies;
using System.Windows;
using JAKE.classlibrary.Shots;
using System.Windows.Shapes;

namespace UI_tests
{
    public class BuilderTests
    {
        public async Task Somethingseomthign()
        {
            //var msApplication = JakeClientHelper.LaunchJakeClient();
            //var automation = new UIA3Automation();
            //var mainWindow = msApplication.GetMainWindow(automation);

            //msApplication.Close();
        }
        [Fact]
        public async Task Build_PlayerVisualBuilder()
        {
            bool tested = false;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                IBuilderVisual<PlayerVisual> playerVisualBuilder = new PlayerVisualBuilder();
                PlayerVisual playerVisual = playerVisualBuilder.New()
                                .SetName("a")
                                .SetColor("red")
                                .SetPosition(0, 0)
                                .Build();
                Assert.Equal("a", playerVisual.PlayerName);
                System.Windows.Media.Brush brush = playerVisual.PlayerColor;
                System.Windows.Media.Color color = Colors.Transparent;
                if (brush is SolidColorBrush solidColorBrush)
                {
                    color = solidColorBrush.Color;
                }
                string colorString = "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
                Assert.Equal("#FFFF0000", colorString);
                Assert.Equal(0, Canvas.GetLeft(playerVisual));
                Assert.Equal(0, Canvas.GetTop(playerVisual));
                tested = true;

                manualResetEvent.Set();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();

            manualResetEvent.WaitOne();

            Assert.True(tested);
        }
        [Fact]
        public async Task Build_EnemyVisualBuilder()
        {
            bool tested = false;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                IBuilderVisual<EnemyVisual> enemyVisualBuilder = new EnemyVisualBuilder();
                EnemyVisual enemyVisual = enemyVisualBuilder.New()
                                    .SetColor("red")
                                    .SetSize(20)
                                    .SetPosition(0, 0)
                                    .Build();
                System.Windows.Media.Brush brush = enemyVisual.FillColor;
                System.Windows.Media.Color color = Colors.Transparent;
                if (brush is SolidColorBrush solidColorBrush)
                {
                    color = solidColorBrush.Color;
                }
                string colorString = "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
                Assert.Equal("#FFFF0000", colorString);
                Assert.Equal(20, enemyVisual.EllipseSize);
                Assert.Equal(0, Canvas.GetLeft(enemyVisual));
                Assert.Equal(0, Canvas.GetTop(enemyVisual));
                tested = true;
                manualResetEvent.Set();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();

            manualResetEvent.WaitOne();

            Assert.True(tested);
        }
        [Fact]
        public async Task Build_ShotVisualBuilder()
        {
            bool tested = false;
            ManualResetEvent manualResetEvent = new ManualResetEvent(false);

            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                IBuilderVisual<ShotVisual> shotVisualBuilder = new ShotVisualBuilder();
                ShotVisual shotVisual = shotVisualBuilder.New()
                                .SetColor($",")
                                .SetSize(20)
                                .SetPosition(0, 0)
                                .Build();
                System.Windows.Media.Brush brush = shotVisual.FillColor;
                System.Windows.Media.Color color = Colors.Transparent;
                if (brush is SolidColorBrush solidColorBrush)
                {
                    color = solidColorBrush.Color;
                }
                string colorString = "#" + color.A.ToString("X2") + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
                Assert.Equal("#FF0000FF", colorString);
                Assert.Equal(20, shotVisual.EllipseSize);
                Assert.Equal(20, shotVisual.PolygonSize);
                Assert.Equal(0, Canvas.GetLeft(shotVisual));
                Assert.Equal(0, Canvas.GetTop(shotVisual));
                tested = true;

                manualResetEvent.Set();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();

            manualResetEvent.WaitOne();

            Assert.True(tested);
        }
    }
}
