using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku_Form {
    class Animation {

        public async void Text_UpDown(Label lb, bool IsUp) {
            for (int i = 0; i < 5; i++, await Task.Delay(2)) {
                lb.Location = new Point(lb.Location.X, lb.Location.Y + ((IsUp) ? -1 : 1));
                lb.Size = new Size(lb.Size.Width, lb.Size.Height + ((IsUp) ? 1 : -1));
            }
        }

    }
}
