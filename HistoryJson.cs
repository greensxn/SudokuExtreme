using System.Collections.Generic;
using System.Drawing;

namespace Sudoku_Form {
    class HistoryJson {

        public List<CageJson> LastMoveHistory = new List<CageJson>();
        public int LastActiveCage_NumName;
        public bool IsErrorActive;
        public bool IsSimilarActive;
        public bool IsCageGridActive;
        public int Move = 0;

    }

    class CageJson {

        public int NumName;
        public int Number;
        public bool IsTempColor;
        public Color BackColor;
        public Color ForeColor;
        public List<int> TempNumbers;

    }
}
