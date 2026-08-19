using saper1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace saper1.Entities
{
    public class Cell(Coordinates? coordinates)
    {
        public Coordinates? Coordinates { get; private set; } = coordinates;

        public Border Border { get; set; } = new();

        public bool IsMine { get; set; } = false;
        public bool IsOpen { get; set; } = false;
        public bool IsFlagged { get; set; } = false;
        
        public int AdjacentMines { get; set; } = 0;

        public static bool operator ==(Cell? left, Cell? right)
        {
            if (left is null && right is null) return true;
            if (left is null || right is null) return false;
            return left.Coordinates == right.Coordinates;
        }

        public static bool operator !=(Cell? left, Cell? right)
        {
            return !(left == right);
        }
    }
}
