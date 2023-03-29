using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Xml;


namespace ReportComparison
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private List<ReportComparisonTool.ReportZip> zips = new List<ReportComparisonTool.ReportZip>();

		public MainWindow()
		{
			InitializeComponent();
		}

	





		private void AddItem(object sender, RoutedEventArgs e)
		{
			

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Open Zip";
			openFileDialog.InitialDirectory = "c:\\";
			openFileDialog.Filter = "zip files | *.zip";
			openFileDialog.RestoreDirectory = true;
			ReportComparisonTool.ReportZip zip = new ReportComparisonTool.ReportZip();
			if (Nickname.Text != "")
			{
				zip.Set_Nickname(Nickname.Text);	
			}
			if(Date.Text != "")
			{
				zip.Set_Date(Date.Text);
			}

			if (openFileDialog.ShowDialog() == true)
			{
				Trace.WriteLine(openFileDialog.FileName);
				
				zip.Set_Report_Path(openFileDialog.FileName);
				zip.Populate_Parm(zip.Get_Path());
				ReportList.Items.Add(zip.Get_Nickname());
			}

			Nickname.Text = "";
			Date.Text = "";

		}

		private void RemoveItem(object sender, RoutedEventArgs e)
		{
			if(ReportList.SelectedItem != null)
			{
				ReportList.Items.Remove(ReportList.SelectedItem);
				
			}

			
		}




		private void Compare(object sender, RoutedEventArgs e)
		{
			

			ReportComparisonTool.ReportZip zip = new ReportComparisonTool.ReportZip();
			foreach(ReportComparisonTool.ReportZip entity in zips)
			{
				Trace.WriteLine(entity.Get_Path());
			}
			zip.CreateComparisonFile(zips);

		}

		private void RunFileWindow(object sender, RoutedEventArgs e)
		{
			ReportComparisonTool.ReportZip zip = new ReportComparisonTool.ReportZip();
			zip.Set_Report_Path("c:\\cncm\\report_88C2558E769B-0303212554_2023-03-28_15-38-36.zip");
			zip.Populate_Parm("c:\\cncm\\report_88C2558E769B-0303212554_2023-03-28_15-38-36.zip");

		
		}

	
		/*private void CompareParm()
		{


			if(elemList1.Count != elemList2.Count)
			{
				MessageBox.Show("Incompatible report .zip folders; files may be corrupted or mismatched machines may be compared");
				return;
			}

			string full_text = "";


			int longestLengthOfParm = 0;

			for (int i = 0; i < elemList1.Count; i++)
			{
				if (elemList1[i].InnerText.Length > longestLengthOfParm)
				{

					longestLengthOfParm = elemList1[i].InnerText.Length;

				}
			}

			string header = WriteParametersHeaderToFile(longestLengthOfParm);

			full_text += header;


			for (int i = 0; i < elemList1.Count; i++)
			{
				if (!elemList1[i].InnerText.Equals(elemList2[i].InnerText))
				{
					full_text += WriteParameterDifferenceToFile(i.ToString(), elemList1[i].InnerText, elemList2[i].InnerText, longestLengthOfParm);
					


				}
			}

			SaveFileDialog ComparisonFileDialog = new SaveFileDialog();

			ComparisonFileDialog.Filter = "txt files (*.txt) |*.txt";
			ComparisonFileDialog.FilterIndex = 2;
			ComparisonFileDialog.RestoreDirectory = true;
			ComparisonFileDialog.Title = "Save Comparison File";

			if (ComparisonFileDialog.ShowDialog() == true)
			{
				File.WriteAllText(ComparisonFileDialog.FileName, full_text);
			}


		}*/

		static string WriteParameterDifferenceToFile(string parm_name, string val1, string val2, int longest)
		{
			string result = "";



			result += "Parameter ";

			result += parm_name;

			int v = (3 - parm_name.Length);

			for (int i = 1; i <= v; i++)
			{
				result += " ";
			}




			result += "|  " + val1;

			v = (longest - val1.Length);

			for(int i = 1; i < v; i++)
			{
				result += " ";
			}
			
			result += "|  " + val2 + "\n";

			return result;
		}

		static string WriteParametersHeaderToFile(int longest)
		{
			string result = "                ";
			result += "Report 1";
			int v = longest - "Report 1".Length;
			for (int i = 1; i < v; i++)
			{
				result += " ";
			}
			result += "|  Report 2\n";

			return result;
			

		}

		private void ComparisonFunction(object sender, RoutedEventArgs e)
		{
			string XmlName = ""; 

			


			SaveFileDialog ComparisonFileDialog = new SaveFileDialog();

			ComparisonFileDialog.Filter = "xml files (*.xml) |*.xml";
			ComparisonFileDialog.FilterIndex = 2;
			ComparisonFileDialog.RestoreDirectory = true;
			ComparisonFileDialog.Title = "Save Comparison File";

			if(ComparisonFileDialog.ShowDialog() == true)
			{
				string copyText = "";
				/*foreach (string entry in ParmListFile1)
				{
					copyText += entry;
				}*/
				
				File.WriteAllText(ComparisonFileDialog.FileName, copyText);
				
				XmlName = ComparisonFileDialog.FileName;
				XmlDocument XmlDoc = new XmlDocument();
				XmlDoc.Load(XmlName);
				XmlNodeList parms = XmlDoc.GetElementsByTagName("value");
				Trace.WriteLine(parms[0].InnerText);











			}
		}

		public void Populate_Parm()
		{
			using (FileStream file = File.OpenRead("c:\\cncm\\report_88C2558E769B-0303212554_2023-03-28_15-38-36.zip"))
			{
				using (ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Read))
				{
					foreach (ZipArchiveEntry entry in zip.Entries)
					{
						if (entry.FullName.Contains(".xml"))
						{
							using (StreamReader sr = new StreamReader(entry.Open()))
							{
								XmlDocument XmlDoc1 = new XmlDocument();
								XmlDoc1.Load(sr);
								XmlElement root = XmlDoc1.DocumentElement;
								XmlNodeList elemList1 = root.GetElementsByTagName("value");


								for (int i = 0; i < elemList1.Count; i++)
								{
									Trace.WriteLine(elemList1[i]);
								}

							}
						}

					}
				}
			}
		}
	}
}
