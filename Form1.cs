using Newtonsoft.Json;
using Sudoku_Form.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku_Form {
    public partial class Form1 : Form {

        private Animation anim;
        private int[] ActiveCages;
        private CageBox[] Buttons;
        private Cage[,] Cell;
        private Cage ActiveButton;
        private History history;
        private SoundPlayer Sound_Pen;
        private SoundPlayer Sound_EnterCage;
        private SoundPlayer Sound_EnterLabel;
        private int Move { get; set; } = 0;
        private bool IsSound { get; set; } = true;
        private bool IsErrorActive { get; set; } = true;
        private bool IsSimilarActive { get; set; } = true;
        private bool IsCageGridActive { get; set; } = true;

        public Form1() {
            InitializeComponent();
            Sound_Pen = new SoundPlayer();
            Sound_EnterCage = new SoundPlayer(Resources.move);
            Sound_EnterLabel = new SoundPlayer(Resources.enter);
            anim = new Animation();
            Buttons = Controls.OfType<CageBox>().Where(a => a.Size == new Size(50, 50)).OrderBy(a => int.Parse(a.Name.Remove(0, 7))).ToArray();
            Cell = new Cage[9, 9];
            ActiveCages = new int[21];
            ActiveButton = null;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++) {
                    Cell[i, j] = new Cage(Buttons[i * 9 + j]);
                    //Cell[i, j].Number = (i + j + 1) % 9;
                    //Cell[i, j].Number = Cell[i, j].Number == 0 ? 9 : Cell[i, j].Number;
                }
            CheckErrorCage();
            history = new History(Cell);
        }

        private void PlaySound(byte variant) {
            if (!IsSound)
                return;
            if (variant == 0) {
                Sound_EnterLabel.Play();
                return;
            }
            else if (variant == 1)
                //Sound_Pen.SoundLocation = $"G:/work/Sudoku_Form/Sudoku_Form/bin/Debug/Sounds/draw{new Random().Next(1, 5)}.wav";
                Sound_Pen.SoundLocation = $"{Directory.GetCurrentDirectory()}/Sounds/draw{new Random().Next(1, 5)}.wav";
            else if (variant == 2) {
                Sound_EnterCage.Play();
                return;
            }
            Sound_Pen.Stop();
            Sound_Pen.Play();
        }

        //CELL EVENT
        private void Cage_MouseClick(object sender, MouseEventArgs e) {
            InvisibleButton.Focus();
            if (ActiveButton != null) {
                ActiveButton.BackColor = Color.LavenderBlush;
                SetColorNeighbourCell(ActiveButton, Color.LavenderBlush, true);
                if (ActiveButton.Name == (sender as CageBox).Name) {
                    ShowSimilarCage(ActiveButton, true);
                    ActiveButton = null;
                    return;
                }
            }

            ActiveButton = Cell.OfType<Cage>().Where(a => a.NumName == int.Parse((sender as CageBox).Name.Remove(0, 7))).First();
            ActiveCages = Cell.OfType<Cage>().Where(a => a.NumName == ActiveButton.NumName).First().Neighbour.ToArray();
            SetColorNeighbourCell(ActiveButton, Color.Pink, false);
            ShowSimilarCage(ActiveButton);
            (sender as CageBox).BackColor = Color.HotPink;
        }

        private void Cage_MouseEnter(object sender, EventArgs e) {
            if (ActiveButton != null)
                return;

            PlaySound(2);

            CageBox btn = sender as CageBox;
            btn.BackColor = Color.HotPink;
            int BtnNumber = int.Parse(btn.Name.Remove(0, 7));
            Cage cell = Cell.OfType<Cage>().Where(a => a.NumName == BtnNumber).First();
            SetColorNeighbourCell(cell, Color.Pink, false);
        }

        private void Cage_MouseLeave(object sender, EventArgs e) {
            if (ActiveButton != null)
                return;

            CageBox btn = sender as CageBox;
            btn.BackColor = Color.LavenderBlush;
            int BtnNumber = int.Parse(btn.Name.Remove(0, 7));
            Cage cell = Cell.OfType<Cage>().Where(a => a.NumName == BtnNumber).First();
            SetColorNeighbourCell(cell, Color.LavenderBlush, true);
        }
        //CELL EVENT


        //NUM 1-9
        private void Numbers_Click(object sender, EventArgs e) {
            Label lb = sender as Label;
            int num = int.Parse(lb.Text);
            if (ActiveButton != null) {
                if (!IsPenActive) {
                    SetCageNumber(ActiveButton, num, false);
                    CheckErrorCage();
                    AddMove();
                    ShowSimilarCage(ActiveButton);
                }
                else {
                    SetCageNumber(ActiveButton, num, true);
                    CheckErrorCage();
                    AddMove();
                }
            }
        }

        private void Numbers_MouseEnter(object sender, EventArgs e) {
            PlaySound(0);
            Label lb = sender as Label;
            lb.ForeColor = Color.DeepPink;
            anim.Text_UpDown(lb, true);
        }

        private void Numbers_MouseLeave(object sender, EventArgs e) {
            Label lb = sender as Label;
            lb.ForeColor = Color.Black;
            anim.Text_UpDown(lb, false);
        }
        //NUM 1-9


        private void Form1_MouseClick(object sender, MouseEventArgs e) {
            if (ActiveButton != null) {
                ActiveButton.BackColor = Color.LavenderBlush;
                SetColorNeighbourCell(ActiveButton, Color.LavenderBlush, true);
                ShowSimilarCage(ActiveButton, true);
                ActiveButton = null;
            }
        }

        private void SetColorNeighbourCell(Cage cage, Color color, bool IsHideFocus = false) {
            if (IsCageGridActive) {
                foreach (int num in cage.Neighbour)
                    Cell.OfType<Cage>().Where(a => a.NumName == num).First().BackColor = color;
            }
            if (IsHideFocus)
                InvisibleButton.Focus();
        }

        private void ShowSimilarCage(Cage cage, bool IsClearAllSimilar = false) {
            foreach (Cage CellCage in Cell)
                if (!IsClearAllSimilar && cage != null && IsSimilarActive) {
                    if (!cage.Neighbour.Contains(CellCage.NumName)) {
                        if (CellCage.Number != 0 && CellCage.Number == cage.Number && CellCage.NumName != cage.NumName) {
                            CellCage.IsTempColor = true;
                            CellCage.BackColor = Color.FromArgb(213, 192, 255);
                        }
                        else if (CellCage.IsTempColor) {
                            CellCage.IsTempColor = false;
                            CellCage.BackColor = Color.LavenderBlush;
                        }
                    }
                    else if (CellCage.NumName != ActiveButton.NumName && CellCage.Number != 0) {
                        if (CellCage.Number == ActiveButton.Number) {
                            CellCage.IsTempColor = true;
                            CellCage.BackColor = Color.FromArgb(213, 192, 255);
                        }
                        else {
                            CellCage.IsTempColor = false;
                            CellCage.BackColor = (IsCageGridActive) ? Color.Pink : Color.LavenderBlush;
                        }
                    }
                }
                else if (IsClearAllSimilar && cage != null && CellCage != cage && CellCage.BackColor != Color.LavenderBlush) {
                    CellCage.IsTempColor = false;
                    if (cage.Neighbour.Any(a => a == CellCage.NumName) && IsCageGridActive)
                        CellCage.BackColor = Color.Pink;
                    else
                        CellCage.BackColor = Color.LavenderBlush;
                }
        }

        private void CheckErrorCage(bool IsReset = false) {
            foreach (Cage cage in Cell) {

                if (cage.Number != 0 && IsErrorActive) {
                    bool IsFoundCopies = false;
                    foreach (int num in cage.Neighbour) {
                        Cage cage_Neigh = Cell.OfType<Cage>().Where(a => a.NumName == num).First();
                        if (cage_Neigh.Number == cage.Number) {
                            cage.ForeColor = Color.Red;
                            IsFoundCopies = true;
                        }
                    }
                    if (!IsFoundCopies) {
                        if (cage != ActiveButton)
                            if (ActiveButton == null || !ActiveButton.Neighbour.Contains(cage.NumName))
                                cage.BackColor = Color.LavenderBlush;
                            else if (IsCageGridActive)
                                cage.BackColor = Color.Pink;
                        cage.ForeColor = Color.Black;
                    }
                }

                if (cage.TempNumbers.Count > 0 && IsErrorActive) {
                    foreach (CageTempNum TempCage in cage.TempNumbers) {
                        bool IsFoundTempCopies = false;
                        foreach (int num in cage.Neighbour) {
                            Cage cage_Neigh = Cell.OfType<Cage>().Where(a => a.NumName == num).First();
                            if (cage_Neigh.Number == TempCage.Number) {
                                TempCage.Control.ForeColor = Color.Red;
                                IsFoundTempCopies = true;
                            }
                        }
                        if (!IsFoundTempCopies)
                            TempCage.Control.ForeColor = Color.Black;
                    }
                }

                if (IsReset) {
                    cage.ForeColor = Color.Black;
                    foreach (CageTempNum cageTemp in cage.TempNumbers)
                        cageTemp.Control.ForeColor = Color.Black;
                }
            }
        }

        private void Key_KeyPress(object sender, KeyPressEventArgs e) {
            if (ActiveButton != null && char.IsNumber(e.KeyChar)) {
                SetCageNumber(ActiveButton, int.Parse(e.KeyChar.ToString()), IsPenActive);
                CheckErrorCage();
                ShowSimilarCage(ActiveButton);
                AddMove();
            }
            else if (e.KeyChar == 't' /*eng*/ || e.KeyChar == 'е' /*ru*/ ) { // fill temp numbers
                if (ActiveButton != null) {
                    ActiveButton.AddTempNumber(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
                    CheckErrorCage();
                    AddMove();
                }
            }
            else if (e.KeyChar == 'e' /*eng*/ || e.KeyChar == 'у' /*ru*/ ) { // clear errors
                if (ActiveButton != null) {
                    int[] deleteNumbers = ActiveButton.TempNumbers.Where(a => a.Control.ForeColor == Color.Red).ToArray().Select(a => a.Number).ToArray();
                    ActiveButton.RemoveTempNumber(deleteNumbers);
                }
                else {
                    foreach (Cage cage in Cell) {
                        int[] deleteNumbers = cage.TempNumbers.Where(a => a.Control.ForeColor == Color.Red).ToArray().Select(a => a.Number).ToArray();
                        cage.RemoveTempNumber(deleteNumbers);
                    }
                }
            }
        }

        private void SetCageNumber(Cage cage, int num, bool IsTemp) {
            PlaySound(1);
            if (!IsTemp) {
                cage.Number = (num == 0 || cage.Number == num) ?
                    cage.Number = 0 :
                    cage.Number = num;
            }
            else {
                if (cage.Number != 0)
                    cage.ResetNumber();
                cage.AddTempNumber(num);
            }
        }

        private void AddMove() {
            Move++;
            if (history.GetLengthHistory() > Move)
                history.ClearHistoryAfter(Move);
            history.AddMove(Cell, ActiveButton);
            lbMove.Text = $"{Move}";
        }

        // Back / Forward
        private void Back_MouseClick(object sender, MouseEventArgs e) {
            if (Move > 0)
                Move--;
            Cage[,] BackCell = history.GetLastMove(false, Move);
            for (int i = 0; i < Cell.GetLength(0); i++)
                for (int j = 0; j < Cell.GetLength(1); j++) {
                    if (Cell[i, j] != BackCell[i, j]) {
                        Cell[i, j].Number = BackCell[i, j].Number;
                        Cell[i, j].BackColor = BackCell[i, j].BackColor;
                        Cell[i, j].ForeColor = BackCell[i, j].ForeColor;
                        Cell[i, j].NumName = BackCell[i, j].NumName;
                        Cell[i, j].Neighbour = BackCell[i, j].Neighbour;
                        Cell[i, j].AddTempNumber(BackCell[i, j].TempNumbers);
                    }
                }
            ActiveButton = history.GetLastActiveCage(false, Move);
            lbMove.Text = $"{Move}";
        }

        private void Forward_MouseClick(object sender, MouseEventArgs e) {
            if (history.GetLengthHistory() > Move + 1)
                Move++;
            Cage[,] BackCell = history.GetLastMove(false, Move);
            for (int i = 0; i < Cell.GetLength(0); i++)
                for (int j = 0; j < Cell.GetLength(1); j++) {
                    if (Cell[i, j] != BackCell[i, j]) {
                        Cell[i, j].Number = BackCell[i, j].Number;
                        Cell[i, j].BackColor = BackCell[i, j].BackColor;
                        Cell[i, j].ForeColor = BackCell[i, j].ForeColor;
                        Cell[i, j].NumName = BackCell[i, j].NumName;
                        Cell[i, j].Neighbour = BackCell[i, j].Neighbour;
                        Cell[i, j].AddTempNumber(BackCell[i, j].TempNumbers);
                    }
                }
            ActiveButton = history.GetLastActiveCage(false, Move);
            lbMove.Text = $"{Move}";
        }
        // Back / Forward

        private void Text_MouseEnter(object sender, EventArgs e) {
            PlaySound(0);
            Label lb = sender as Label;
            lb.ForeColor = Color.DeepPink;
            anim.Text_UpDown(lb, true);
        }

        private void Text_MouseLeave(object sender, EventArgs e) {
            Label lb = sender as Label;
            if (lb.Text == "Pen") {
                if (!IsPenActive)
                    lb.ForeColor = Color.DimGray;
            }
            else if (lb.Text == "Error") {
                if (!IsErrorActive)
                    lb.ForeColor = Color.DimGray;
            }
            else if (lb.Text == "Similar") {
                if (!IsSimilarActive)
                    lb.ForeColor = Color.DimGray;
            }
            else if (lb.Text == "Cage grid") {
                if (!IsCageGridActive)
                    lb.ForeColor = Color.DimGray;
            }
            else
                lb.ForeColor = Color.DimGray;
            anim.Text_UpDown(lb, false);
        }

        private bool IsPenActive { get; set; } = false;
        private void Pen_Click(object sender, EventArgs e) {
            IsPenActive = !IsPenActive;
            lbPen.ForeColor = IsPenActive ? Color.DeepPink : Color.DimGray;
        }

        private void Clear_Click(object sender, EventArgs e) {
            if (ActiveButton != null) {
                ActiveButton.ResetNumber();
                ActiveButton.ResetTempNumbers();
                CheckErrorCage();
            }
        }

        private void Similar_Click(object sender, EventArgs e) {
            if (sender != null) {
                IsSimilarActive = !IsSimilarActive;
                if (IsSimilarActive)
                    ShowSimilarCage(ActiveButton);
                else
                    ShowSimilarCage(ActiveButton, true);
            }
            lbSimilar.ForeColor = IsSimilarActive ? Color.DeepPink : Color.DimGray;
        }

        private void Error_Click(object sender, EventArgs e) {
            if (sender != null) {
                IsErrorActive = !IsErrorActive;
                if (IsErrorActive)
                    CheckErrorCage();
                else
                    CheckErrorCage(true);
            }
            lbError.ForeColor = IsErrorActive ? Color.DeepPink : Color.DimGray;
        }

        private void CageGrid_Click(object sender, EventArgs e) {
            if (sender != null) {
                IsCageGridActive = !IsCageGridActive;
                if (ActiveButton != null)
                    foreach (int Neigh in ActiveButton.Neighbour)
                        Cell.OfType<Cage>().Where(a => a.NumName == Neigh).First().BackColor = (IsCageGridActive) ? Color.Pink : Color.LavenderBlush;
            }
            lbCageGrid.ForeColor = IsCageGridActive ? Color.DeepPink : Color.DimGray;
        }

        //SETTINGS
        private void SaveSudoku_Click(object sender, EventArgs e) {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Sudoku File(*.sudoku) | *.sudoku";
            if (save.ShowDialog() == DialogResult.OK) {

                HistoryJson historyJson = new HistoryJson();
                foreach (Cage c in Cell) {
                    if (c.Number != 0 || c.TempNumbers.Count > 0 || c.BackColor != Color.LavenderBlush) {
                        historyJson.LastMoveHistory.Add(new CageJson() {
                            Number = c.Number,
                            TempNumbers = c.TempNumbers.Select(a => a.Number).ToList(),
                            IsTempColor = c.IsTempColor,
                            BackColor = c.BackColor,
                            ForeColor = c.ForeColor,
                            NumName = c.NumName,
                        });
                    }
                }

                if (ActiveButton != null)
                    historyJson.LastActiveCage_NumName = ActiveButton.NumName;
                else
                    historyJson.LastActiveCage_NumName = 0;
                historyJson.IsCageGridActive = IsCageGridActive;
                historyJson.IsErrorActive = IsErrorActive;
                historyJson.IsSimilarActive = IsSimilarActive;
                historyJson.Move = Move;
                String obj = JsonConvert.SerializeObject(historyJson);
                File.WriteAllText(save.FileName, obj);
            }
        }

        private void LoadSudoku_Click(object sender, EventArgs e) {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Sudoku File(*.sudoku) | *.sudoku";
            if (open.ShowDialog() == DialogResult.OK) {
                ClearSudoku();
                history.ClearHistoryAfter(1);

                HistoryJson historyJson = JsonConvert.DeserializeObject<HistoryJson>(File.ReadAllText(open.FileName));

                List<CageJson> BackCell = historyJson.LastMoveHistory;
                foreach (CageJson c in BackCell) {
                    if (c.Number != 0 || c.TempNumbers.Count > 0 || c.BackColor != Color.LavenderBlush) {
                        Cage cg = Cell.OfType<Cage>().Where(a => a.NumName == c.NumName).First();
                        cg.Number = c.Number;
                        c.TempNumbers.ForEach(a => cg.AddTempNumber(a));
                        cg.BackColor = c.BackColor;
                        cg.ForeColor = c.ForeColor;
                        cg.NumName = c.NumName;
                        cg.IsTempColor = c.IsTempColor;
                    }
                }

                Move = historyJson.Move;
                lbMove.Text = $"{Move}";

                if (historyJson.LastActiveCage_NumName != 0) {
                    history.LastActiveCage.Add(Cell.OfType<Cage>().Where(a => a.NumName == historyJson.LastActiveCage_NumName).FirstOrDefault());
                    ActiveButton = history.GetLastActiveCage(false);
                    ActiveButton.BackColor = Color.HotPink;
                }

                IsSimilarActive = historyJson.IsSimilarActive;
                Similar_Click(null, null);
                IsCageGridActive = historyJson.IsCageGridActive;
                CageGrid_Click(null, null);
                IsErrorActive = historyJson.IsErrorActive;
                Error_Click(null, null);
            }
        }

        private void ClearSudoku() {
            foreach (Cage cage in Cell) {
                cage.ResetNumber();
                cage.ResetTempNumbers();
                cage.IsTempColor = false;
                cage.BackColor = Color.LavenderBlush;
                cage.ForeColor = Color.Black;
            }
        }

        private void NewGame_Click(object sender, EventArgs e) {
            ClearSudoku();
            Move = 0;
            lbMove.Text = $"{Move}";
        }

        private void Exit_Click(object sender, EventArgs e) {
            Close();
        }

        private void Sound_Click(object sender, EventArgs e) {
            if (lbSound.Text == "On") {
                lbSound.Text = "Off";
                lbSound.ForeColor = Color.DimGray;
                IsSound = false;
            }
            else {
                lbSound.Text = "On";
                lbSound.ForeColor = Color.DeepPink;
                IsSound = true;
            }
        }

        private void NewGame_MouseEnter(object sender, EventArgs e) {
            PlaySound(2);
        }
    }
}
