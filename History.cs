using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku_Form {
    class History {

        public List<Cage[,]> MoveHistory;
        public List<Cage> LastActiveCage;

        public History(Cage[,] Cell) {
            MoveHistory = new List<Cage[,]>();
            LastActiveCage = new List<Cage>();
            MoveHistory.Add(NewArrayForList(Cell));
        }

        private Cage[,] NewArrayForList(Cage[,] Cell) {
            Cage[,] NewCell = new Cage[Cell.GetLength(0), Cell.GetLength(1)];
            for (int i = 0; i < Cell.GetLength(0); i++)
                for (int j = 0; j < Cell.GetLength(1); j++) {

                    Cage cage = new Cage(new CageBox() { Name = "CageBox0" }) {
                        Name = Cell[i, j].Name,
                        NumName = Cell[i, j].NumName,
                        Number = Cell[i, j].Number,
                        Neighbour = Cell[i, j].Neighbour,
                        ForeColor = Cell[i, j].ForeColor,
                        BackColor = Cell[i, j].BackColor,
                        TempNumbers = Cell[i, j].TempNumbers,
                        IsTempColor = Cell[i, j].IsTempColor
                    };

                    NewCell[i, j] = cage;
                }
            return NewCell;
        }

        public Cage[,] GetLastMove(bool IsDelLast, int Move = -1) {
            if (IsDelLast && MoveHistory.Count > 1)
                MoveHistory.RemoveAt(MoveHistory.Count - 1);

            if (Move != -1 && MoveHistory.Count > Move)
                return MoveHistory[Move];
            else
                return MoveHistory[MoveHistory.Count - 1];
        }

        public void AddMove(Cage[,] Cell, Cage lastActiveCage) {
            MoveHistory.Add(NewArrayForList(Cell));
            LastActiveCage.Add(lastActiveCage);
        }

        public Cage GetLastActiveCage(bool IsDelLast, int Move = -1) {
            if (IsDelLast && LastActiveCage.Count > 1)
                LastActiveCage.RemoveAt(LastActiveCage.Count - 1);

            if (Move > 0 && LastActiveCage.Count > Move)
                return LastActiveCage[Move - 1];
            else if (LastActiveCage.Count > 0)
                return LastActiveCage[LastActiveCage.Count - 1];
            else return null;
        }

        public int GetLengthHistory() {
            return MoveHistory.Count;
        }

        public List<Cage[,]> GetMoveHistory => MoveHistory;

        public void ClearHistoryAfter(int Number) {
            if (Number < MoveHistory.Count) {
                int length = MoveHistory.Count - Number;
                while (length != 0) {
                    MoveHistory.RemoveAt(MoveHistory.Count - 1);
                    LastActiveCage.RemoveAt(LastActiveCage.Count - 1);
                    length--;
                }
            }
        }
    }
}
