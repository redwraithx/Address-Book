using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AddressBook
{
    class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder sort;
        
        public ListViewItemComparer()
        {
            col = 0;
            sort = SortOrder.Ascending;
        }





        public ListViewItemComparer(int column, SortOrder sortOrder)
        {
            col = column;
            sort = sortOrder;
        }




        public int Compare(object x, object y)
        {
            Regex r = new Regex("^[0-9]+$");
            int result;

            string xs = ((ListViewItem)x).SubItems[col].Text;
            string ys = ((ListViewItem)y).SubItems[col].Text;

            if (r.IsMatch(xs) && r.IsMatch(ys))
            {
                result = Int64.Parse(xs).CompareTo(Int64.Parse(ys));
            }
            else
            {
                result = String.Compare(xs, ys);
            }

            bool isAscendingOrder = false;
            if (sort == SortOrder.Ascending)
            {
                isAscendingOrder = true;
            }
            else
            {
                isAscendingOrder = false;
            }
            
            return isAscendingOrder ? result : -result;
        }
    }
}
