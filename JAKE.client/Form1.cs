using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JAKE.client
{
    public partial class Form1 : Form
    {
        private int cubeX = 100;
        private int cubeY = 100;
        private const int cubeSize = 50;
        private const int moveDistance = 10;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Create a Graphics object to draw on the form
            Graphics g = e.Graphics;

            // Draw the cube
            g.FillRectangle(Brushes.Blue, cubeX, cubeY, cubeSize, cubeSize);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Move the cube based on arrow key input
            switch (e.KeyCode)
            {
                case Keys.Left:
                    cubeX -= moveDistance;
                    break;
                case Keys.Right:
                    cubeX += moveDistance;
                    break;
                case Keys.Up:
                    cubeY -= moveDistance;
                    break;
                case Keys.Down:
                    cubeY += moveDistance;
                    break;
            }

            // Redraw the form to update the cube's position
            this.Invalidate();
        }
    }
}
