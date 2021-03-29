using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku_Form {
    public partial class CageBox : UserControl {

        private List<CageTempNum> TempNumbers { get; }
        private int number;

        public CageBox() {
            InitializeComponent();
            foreach (Panel pn in Controls.OfType<Panel>())
                SetEvent(pn);
            TempNumbers = new List<CageTempNum>();
        }

        public Color BorderColor {
            get {
                return panel1.BackColor;
            }
            set {
                panel1.BackColor = value;
                panel2.BackColor = value;
                panel3.BackColor = value;
                panel4.BackColor = value;
            }
        }

        public void AddTempNumber(int Number) {
            _AddTempNumber(Number);
        }

        public void AddTempNumber(List<CageTempNum> Numbers) {
            foreach (CageTempNum cageTemp in Numbers)
                _AddTempNumber(cageTemp.Number);
        }

        public void AddTempNumber(int[] Numbers) {
            foreach (int cageTemp in Numbers)
                _AddTempNumber(cageTemp);
        }

        private void _AddTempNumber(int Number) {
            if (TempNumbers.Any(a => a.Number == Number)) {
                RemoveTempNumber(Number);
                return;
            }

            if (Number == 0) {
                ResetTempNumbers();
                return;
            }

            ResetNumber();

            Label lb = new Label();
            Controls.Add(lb);
            lb.Font = new Font("BlowBrush", 8, FontStyle.Regular);
            lb.Text = Number.ToString();
            lb.AutoSize = true;
            SetEvent(lb);
            switch (Number) {
                case 1:
                    lb.Location = new Point(3, 2);
                    break;
                case 2:
                    lb.Location = new Point(18, 2);
                    break;
                case 3:
                    lb.Location = new Point(33, 2);
                    break;
                case 4:
                    lb.Location = new Point(3, 17);
                    break;
                case 5:
                    lb.Location = new Point(18, 17);
                    break;
                case 6:
                    lb.Location = new Point(33, 17);
                    break;
                case 7:
                    lb.Location = new Point(3, 32);
                    break;
                case 8:
                    lb.Location = new Point(18, 32);
                    break;
                case 9:
                    lb.Location = new Point(33, 32);
                    break;

                default:
                    break;
            }
            TempNumbers.Add(new CageTempNum(lb, Number));
        }

        public void RemoveTempNumber(int Number) {
            if (TempNumbers.Any(a => a.Number == Number)) {
                Controls.Remove(TempNumbers.Where(a => a.Number == Number).First().Control);
                TempNumbers.Remove(TempNumbers.Where(a => a.Number == Number).First());
            }
        }

        public void RemoveTempNumber(int[] Numbers) {
            foreach (int num in Numbers)
                if (TempNumbers.Any(a => a.Number == num)) {
                    Controls.Remove(TempNumbers.Where(a => a.Number == num).First().Control);
                    TempNumbers.Remove(TempNumbers.Where(a => a.Number == num).First());
                }
        }

        public int Number {
            get {
                return number;
            }
            set {
                number = value;
                ResetNumber();
                if (number == 0)
                    return;
                ResetTempNumbers();
                Label lb = new Label();
                lb.Name = "lab";
                SetEvent(lb);
                Controls.Add(lb);
                lb.Text = number.ToString();
                lb.TextAlign = ContentAlignment.MiddleCenter;
                lb.Dock = DockStyle.Fill;
                lb.Font = new Font("BlowBrush", 18, FontStyle.Regular);
            }
        }
        public void ResetNumber() {
            if (Controls.OfType<Label>().Any(a => a.Name == "lab"))
                Controls.Remove(Controls.OfType<Label>().Where(a => a.Name == "lab").First());
        }


        public void ResetTempNumbers() {
            TempNumbers.Clear();
            Label[] labels = Controls.OfType<Label>().ToArray();
            foreach (Label lb in labels)
                Controls.Remove(lb);
        }

        public List<CageTempNum> GetTempNumbers() {
            return TempNumbers;
        }

        // Events
        private void SetEvent(Control control) {
            control.MouseEnter += Lb_MouseEnter;
            control.MouseLeave += Lb_MouseLeave;
            control.MouseClick += Lb_MouseClick;
        }

        private void Lb_MouseClick(object sender, MouseEventArgs e) {
            OnMouseClick(e);
        }

        private void Lb_MouseLeave(object sender, EventArgs e) {
            OnMouseLeave(e);
        }

        private void Lb_MouseEnter(object sender, EventArgs e) {
            OnMouseEnter(e);
        }
    }

    public class CageTempNum {

        public int Number;
        public Label Control;

        public CageTempNum(Label Control, int Number) {
            this.Control = Control;
            this.Number = Number;
        }
    }
}
