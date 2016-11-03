using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FTPClientGUI
{
	class FileSorter : IComparer
	{
		Predicate<ListViewItem> isDirectory;

		public FileSorter(Predicate<ListViewItem> isDirectory)
		{
			this.isDirectory = isDirectory;
		}

		public int Compare(object x, object y)
		{
			var i1 = (ListViewItem)x;
			var i2 = (ListViewItem)y;

			if((isDirectory(i1) && isDirectory(i2)) || (!isDirectory(i1) && !isDirectory(i2)))
			{
				return i1.Text.CompareTo(i2.Text);
			}
			else if (!isDirectory(i1) && isDirectory(i2))
			{
				return 1;
			}
			else
			{
				return -1;
			}

		}
	}
}
