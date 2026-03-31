using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace WeifenLuo.WinFormsUI.ThemeVS2013
{
    [ToolboxItem(false)]
    internal class VS2013SplitterControl : DockPane.SplitterControlBase
    {
        private readonly SolidBrush _horizontalBrush;
        private int SplitterSize { get; }

        public VS2013SplitterControl(DockPane pane)
            : base(pane)
        {
            _horizontalBrush = pane.DockPanel.Theme.PaintingService.GetBrush(pane.DockPanel.Theme.ColorPalette.MainWindowActive.Background);
            SplitterSize = pane.DockPanel.Theme.Measures.SplitterSize;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            try
            {
                Rectangle rect = ClientRectangle;
                if (rect.Width <= 0 || rect.Height <= 0)
                    return;

                switch (Alignment)
                {
                    case DockAlignment.Right:
                    case DockAlignment.Left:
                        e.Graphics.FillRectangle(_horizontalBrush, rect);
                        break;
                    case DockAlignment.Bottom:
                    case DockAlignment.Top:
                        e.Graphics.FillRectangle(_horizontalBrush, rect);
                        break;
                }
            }
            catch (System.Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
    }
}
