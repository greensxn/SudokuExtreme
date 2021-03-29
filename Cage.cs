using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku_Form {
    class Cage {

        public int NumName;
        public bool IsTempColor;
        public List<int> Neighbour;
        private CageBox Control;

        public Cage(CageBox btn) {
            Control = btn;
            NumName = int.Parse(Control.Name.Remove(0, 7));
            Neighbour = new List<int>();

            // line up
            for (int i = 1; i <= ((NumName % 9 != 0) ? NumName / 9 : (NumName / 9) - 1); i++)
                Neighbour.Add(NumName - 9 * i);

            // line right
            for (int i = 9; i <= 81; i += 9)
                if (i >= NumName) {
                    for (int j = NumName + 1; j <= i; j++)
                        Neighbour.Add(j);
                    break;
                }

            // line down
            for (int i = 1; i < ((NumName % 9 != 0) ? 9 - (NumName / 9) : 9 - (NumName / 9) + 1); i++)
                Neighbour.Add(NumName + 9 * i);

            // line left
            for (int i = 9; i <= 81; i += 9)
                if (i >= NumName) {
                    for (int j = 9; j - 1 > i - NumName; j--)
                        Neighbour.Add(NumName - 10 + j);
                    break;
                }

            // 3x3 cell
            getGroup(NumName).ForEach(num => Neighbour.Add(num));


            //           1   2    3      4    5     6       7    8     9
            //          10  11   12     13   14    15       16   17    18
            //          19  20   21     22   23    24       25   26    27

            //          28  29   30     31   32    33       34   35    36
            //          37  38   39     40   41    42       43   44    45
            //          46  47   48     49   50    51       52   53    54

            //          55  56   57     58   59    60       61   62    63
            //          64  65   66     67   68    69       70   71    72
            //          73  74   75     76   77    78       79   80    81
        }

        private List<int> getGroup(int number) {
            List<int> group = new List<int>();

            int LocX = number % 3;
            LocX = (LocX == 0) ? 3 : LocX;
            int LocY = (number / 9 + ((number % 9 == 0) ? 0 : 1)) % 3;
            LocY = (LocY == 0) ? 3 : LocY;

            int neight = number - LocX;

            for (int j = 1; j <= 3; j++) {
                switch (LocY) {

                    case 1:
                        if (number != neight + j)
                            group.Add(neight + j);
                        group.Add(neight + 9 + j);
                        group.Add(neight + 9 * 2 + j);
                        break;

                    case 2:
                        if (number != neight + j)
                            group.Add(neight + j);
                        group.Add(neight - 9 + j);
                        group.Add(neight + 9 + j);
                        break;

                    case 3:
                        if (number != neight + j)
                            group.Add(neight + j);
                        group.Add(neight - 9 + j);
                        group.Add(neight - 9 * 2 + j);
                        break;
                }
            }

            return group;
        }

        public Cage(Cage cage) {
            Control = cage.Control;
            NumName = cage.NumName;
            Neighbour = cage.Neighbour;
            BackColor = cage.BackColor;
            ForeColor = cage.ForeColor;
        }

        public Cage() { }

        public int Number {
            get {
                return Control.Number;
            }
            set {
                Control.Number = value;
                if (value == 0)
                    Control.ResetTempNumbers();
            }
        }

        public Color BackColor {
            get {
                return Control.BackColor;
            }
            set {
                Control.BackColor = value;
            }
        }

        public Color ForeColor {
            get {
                return Control.ForeColor;
            }
            set {
                Control.ForeColor = value;
            }
        }

        public String Name {
            get {
                return Control.Name;
            }
            set {
                Control.Name = value;
            }
        }

        public void ResetTempNumbers() => Control.ResetTempNumbers();

        public void ResetNumber() {
            Number = 0;
            Control.ResetNumber();
        }

        public void AddTempNumber(int Number) {
            Control.AddTempNumber(Number);
        }

        public void AddTempNumber(List<CageTempNum> Numbers) {
            Control.AddTempNumber(Numbers);
        }

        public void AddTempNumber(int[] Numbers) {
            Control.AddTempNumber(Numbers);
        }

        public void RemoveTempNumber(int Number) {
            Control.RemoveTempNumber(Number);
        }

        public void RemoveTempNumber(int[] Numbers) {
            Control.RemoveTempNumber(Numbers);
        }

        //public List<CageTempNum> GetTempNumbers() {
        //    return Control.GetTempNumbers();
        //}

        public List<CageTempNum> TempNumbers {
            get {
                return Control.GetTempNumbers();
            }
            set {
                Control.AddTempNumber(value);
            }
        }
    }
}
