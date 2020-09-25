using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinSplit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
			System.Diagnostics.Debug.WriteLine("---> START");
			txtStatus.Clear();

			string filePath = selectFile();
			System.Diagnostics.Debug.WriteLine("- File = \"" + filePath + "\"");
			if (filePath == "")
			{
				txtStatus.Text = "失敗：ファイルが選択されていません";
				return;
			}
			string fileDir = System.IO.Path.GetDirectoryName(filePath);
			string fileBase = System.IO.Path.GetFileNameWithoutExtension(filePath);
			string fileExt = System.IO.Path.GetExtension(filePath);

			int unitLine;
			if (int.TryParse(txtLine.Text, out unitLine) == false)
			{
				txtStatus.Text = "失敗：行数に指定の [" + txtLine.Text + "] は数値ではありません";
				return;
			}

			txtStatus.Text += $"■以下ファイルを {unitLine:#,0} 行単位で分割します\r\n";
			txtStatus.Text += $"{filePath}\r\n\r\n";

			txtStatus.Text += "≫ 分割開始 ≫≫≫\r\n";
			txtStatus.Refresh();

			int b;
			FileStream infs = new FileStream(filePath, FileMode.Open);
			FileStream outfs = null;

			int countLf = 0;
			int countLine = 0;
			int newFileCount = 0;
			string newPath = "";
			while ((b = infs.ReadByte()) != -1)
			{
				if (outfs == null)
				{
					newPath = $"{fileDir}\\{fileBase}_{++newFileCount:000}{fileExt}";
					outfs = new FileStream(newPath, FileMode.Create);
					txtStatus.Refresh();
				}
				outfs.WriteByte((byte)b);
				if (b == 10)
				{
					countLf++;
					countLine++;
					if ((countLine % unitLine) == 0)
					{
						countLine = closeNewFile(ref outfs, newPath, countLine);
					}
				}
			}
			infs.Close();
			if (outfs != null)
            {
//				outfs.Close();
				countLine = closeNewFile(ref outfs, newPath, countLine);
			}

			txtStatus.Text += "≫ 分割終了 ≫≫≫\r\n\r\n";
			txtStatus.Text += $"全 {countLf:#,0} 行を処理しました";

			System.Diagnostics.Debug.WriteLine("<--- FINISH");
		}

		private String selectFile()
		{
			String filePath = "";
			OpenFileDialog ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				filePath = ofd.FileName;

			}
			return filePath;
		}

		private int closeNewFile(ref FileStream outfs, string newPath, int countLine)
        {
			outfs.Close();
			outfs = null;
			txtStatus.Text += $"{newPath}\t{countLine:#,0}行\r\n";
			countLine = 0;

			return countLine;
		}

	}
}
