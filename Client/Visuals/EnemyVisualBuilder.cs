using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using JAKE.client;
using JAKE.Client;

namespace JAKE.classlibrary.Patterns
{
    public class EnemyVisualBuilder : IBuilderVisual<EnemyVisual>
    {
        private EnemyVisual? enemyVisual;

        public IBuilderVisual<EnemyVisual> New()
        {
            enemyVisual = new EnemyVisual();
            return this;
        }

        public IBuilderVisual<EnemyVisual> SetColor(string color)
        {
            Color enemyColor = (Color)ColorConverter.ConvertFromString(color);
            SolidColorBrush solidColorBrush = new(enemyColor);
            enemyVisual.FillColor = solidColorBrush;
            enemyVisual.UpdateEnemy(solidColorBrush);
            return this;
        }

        public IBuilderVisual<EnemyVisual> SetName(string color)
        {
            return this;
        }
        public IBuilderVisual<EnemyVisual> SetSize(int size)
        {
            enemyVisual.EllipseSize = size;
            return this;
        }
        public IBuilderVisual<EnemyVisual> SetPosition(double x, double y)
        {
            Canvas.SetLeft(enemyVisual, x);
            Canvas.SetTop(enemyVisual, y);
            return this;
        }

        public EnemyVisual Build()
        {
            return enemyVisual;
        }
    }
}
