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

		private List<string> ParmListFile1 = new List<string>();
		private List<string> ParmListFile2 = new List<string>();
		private List<string> ComparatorList = new List<string>();
		private string firstFilePathZip = string.Empty;
		private string secondFilePathZip = string.Empty;
		private string parmSettingsXmlPath = string.Empty;
		private string configSettingsXmlPath = string.Empty;
		private string parmTempFile = string.Empty;
		private string configTempFile = string.Empty;
		private string filePathParm1 = string.Empty;
		private string filePathConfig1 = string.Empty;
		private string filePathParm2 = string.Empty;
		private string filePathConfig2 = string.Empty;
		private string filePathZip = string.Empty;


		public MainWindow()
		{
			InitializeComponent();
			SetDefaults();
			
			
			
		}

		private void SetDefaults()
		{
			File1Name.IsReadOnly = true;
			File2Name.IsReadOnly = true;
			

		}

		private void GetCNCVersionFromReport(string ZipPath)
		{

			using (ZipArchive archive = ZipFile.OpenRead(ZipPath))
			{
				foreach (ZipArchiveEntry entry in archive.Entries)
				{
					if(entry.FullName.Contains("report") && entry.FullName.Contains(".txt"))
					{

					}


				}
			}
		}


				private void ReadInParmValues(string parmXmlPath, int FileUploadNumber)
		{
			var parmXmlFile = File.ReadAllLines(parmXmlPath);
			var parmList = new List<string>(parmXmlFile);
			foreach(string entry in parmList)
			{
				Trace.WriteLine(entry);
			}
			if(FileUploadNumber == 1)
			{
				ParmListFile1 = parmList;
			}
			else
			{
				ParmListFile2 = parmList;
			}
			
		}

		private void ScanZipForSettingsFile(int FileUploadNumber)
		{
			
			
			
			var tempFilePath = "c:\\Users\\CNC User\\log.txt";
			

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Open Zip";
			openFileDialog.InitialDirectory = "c:\\";
			openFileDialog.Filter = "zip files | *.zip";
			openFileDialog.RestoreDirectory = true;



			if (openFileDialog.ShowDialog() == true)
			{
				int index = 0;
				if(FileUploadNumber == 1)
				{
					
					firstFilePathZip = openFileDialog.FileName;
					index = firstFilePathZip.LastIndexOf("report");
					configTempFile = firstFilePathZip.Substring(0, index);

				}
				else
				{
					
					secondFilePathZip = openFileDialog.FileName;
					index = secondFilePathZip.LastIndexOf("report");
					configTempFile = secondFilePathZip.Substring(0, index);
				}

				filePathZip = openFileDialog.FileName;

				Trace.WriteLine(filePathZip);

				Trace.WriteLine(configTempFile);
				Trace.WriteLine(parmTempFile);


				using (ZipArchive archive = ZipFile.OpenRead(filePathZip))
				{
					foreach (ZipArchiveEntry entry in archive.Entries)
					{
						//First sweep through report.zip, skipping other folders and non .xml files
						//TODO: sweep through a second time to double check
						if (entry.FullName.Contains("/") || !entry.FullName.EndsWith(".xml"))
						{
							continue;
						}

						switch (entry.FullName)
						{
							case "cncm.prm.xml":
								parmSettingsXmlPath = entry.FullName;
								ComparatorList.Remove(entry.FullName);
								break;
							case "cncmcfg.xml":
								configSettingsXmlPath = entry.FullName;
								ComparatorList.Remove(entry.FullName);
								break;
							default:
								break;
						}



						if (entry.FullName.Equals("cncm.prm.xml"))
						{
							if(FileUploadNumber == 1)
							{
								filePathParm1 = "C:\\cncm\\" + entry.FullName + "_report1.xml";
								Trace.WriteLine(filePathParm1);
								if (!File.Exists(filePathParm1))
								{
									entry.ExtractToFile(filePathParm1);

								}
								else
								{
									File.Delete(filePathParm1);
									entry.ExtractToFile(filePathParm1);
								}
							}

							else
							{
								filePathParm2 = "C:\\cncm\\" + entry.FullName + "_report2.xml";
								Trace.WriteLine(filePathParm2);
								if (!File.Exists(filePathParm2))
								{
									entry.ExtractToFile(filePathParm2);

								}
								else
								{
									File.Delete(filePathParm2);
									entry.ExtractToFile(filePathParm2);
								}

							}
							
							string displayName = filePathZip;
							while (displayName.Contains("\\"))
							{
								displayName = displayName.Substring(displayName.IndexOf("\\") + 1);
							}


							ReadInParmValues(tempFilePath, FileUploadNumber);
						
					


						}

						if (entry.FullName.Equals("cncmcfg.xml"))
						{
							if( FileUploadNumber == 1)
							{
								filePathConfig1 = "C:\\cncm\\" + entry.FullName + "_report1.xml";
							
								if (!File.Exists(filePathConfig1))
								{
									entry.ExtractToFile(filePathConfig1);

								}
								else
								{
									File.Delete(filePathConfig1);
									entry.ExtractToFile(filePathConfig1);
								}
							}
							else
							{
								filePathConfig2 = "C:\\cncm\\" + entry.FullName + "_report2.xml";

								if (!File.Exists(filePathConfig2))
								{
									entry.ExtractToFile(filePathConfig2);

								}
								else
								{
									File.Delete(filePathConfig2);
									entry.ExtractToFile(filePathConfig2);
								}
							}
							
							string displayName = filePathZip;
							while (displayName.Contains("\\"))
							{
								displayName = displayName.Substring(displayName.IndexOf("\\") + 1);
							}


							ReadInParmValues(tempFilePath, FileUploadNumber);
							if (FileUploadNumber == 1)
							{
								File1Name.Text = displayName;
								File1Name.FontSize = 12;
							}
							else
							{
								File2Name.Text = displayName;
								File2Name.FontSize = 12;
							}



						}


					}
				}
				if (filePathParm1.Equals(null))
				{
					MessageBox.Show("No valid parameter file found in report.zip");
				}
				else
				{
					
				}
			}

		}

		private void ExtractToTempFile()
		{
			

			//Looks at settings files and extracts to temp locations
			if (!parmSettingsXmlPath.Equals(string.Empty))
			{
				ComparatorList.Add(parmSettingsXmlPath);
			}
			if (!configSettingsXmlPath.Equals(string.Empty))
			{
				ComparatorList.Add(configSettingsXmlPath);
			}

			foreach(string entity in ComparatorList)
			{

				
				/*if (!File.Exists(configTempFile))
				{
					entity.ExtractToFile(configTempFile);

				}
				else
				{
					File.Delete(tempFilePath);
					entry.ExtractToFile(tempFilePath);
				}
				string displayName = filePathZip;
				while (displayName.Contains("\\"))
				{
					displayName = displayName.Substring(displayName.IndexOf("\\") + 1);
				}*/


				/*ReadInParmValues(tempFilePath, FileUploadNumber);
				if (FileUploadNumber == 1)
				{
					ZipFile1Name.Text = displayName;
				}
				else
				{
					ZipFile2Name.Text = displayName;
				}
*/


			}


		}

		private void Button_Click_File1(object sender, RoutedEventArgs e)

		{

			ScanZipForSettingsFile(1);
		


		}

		private void Button_Click_File2(object sender, RoutedEventArgs e)

		{

			ScanZipForSettingsFile(2);


		}

		private void AddOrRemoveComparator_Parm(object sender, RoutedEventArgs e)
		{
			if (ParmSelect.IsChecked == false)
			{
				ComparatorList.Remove("cncm.prm.xml");
			}
			else
			{
				ComparatorList.Add("cncm.prm.xml");
			}

		}
		private void AddOrRemoveComparator_Config(object sender, RoutedEventArgs e)
		{
			if (ConfigSelect.IsChecked == false)
			{
				ComparatorList.Remove("cncmcfg.xml");
			}
			else
			{
				ComparatorList.Add("cncmcfg.xml");
			}


		}

		private void File1_TextChanged(object sender, RoutedEventArgs e)
		{

			
		




		}

		private void File2_TextChanged(object sender, RoutedEventArgs e)
		{

		}

		private void CompareParm(object sender, RoutedEventArgs e)
		{
			List<String> Differences = new List<String>();

			XmlDocument XmlDoc1 = new XmlDocument();
			XmlDoc1.Load(filePathParm1);

			XmlDocument XmlDoc2 = new XmlDocument();
			XmlDoc2.Load(filePathParm2);


			XmlElement root = XmlDoc1.DocumentElement;
			XmlNodeList elemList = root.GetElementsByTagName("value");

			XmlElement root2 = XmlDoc2.DocumentElement;
			XmlNodeList elemList2 = root2.GetElementsByTagName("value");

			var path = "c:\\cncm\\logger.txt";
			

			int longestLengthOfParm = 0;

			for (int i = 0; i < elemList.Count; i++)
			{
				if (elemList[i].InnerText.Length > longestLengthOfParm)
				{

					longestLengthOfParm = elemList[i].InnerText.Length;

				}
			}

			string header = WriteParametersHeaderToFile(path, longestLengthOfParm);

			File.AppendAllText(path, header);


			for (int i = 0; i < elemList.Count; i++)
			{
				if (!elemList[i].InnerText.Equals(elemList2[i].InnerText))
				{
					File.AppendAllText(path, WriteParameterDifferenceToFile(i.ToString(), elemList[i].InnerText, elemList2[i].InnerText, path, longestLengthOfParm));
					

				}
			}

		

		}

		static string WriteParameterDifferenceToFile(string parm_name, string val1, string val2, string path, int longest)
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

		static string WriteParametersHeaderToFile(string path, int longest)
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

			if(firstFilePathZip == string.Empty || secondFilePathZip == string.Empty)
			{
				MessageBox.Show("Load valid report zip files before continuing");
				return;
			}


			SaveFileDialog ComparisonFileDialog = new SaveFileDialog();

			ComparisonFileDialog.Filter = "xml files (*.xml) |*.xml";
			ComparisonFileDialog.FilterIndex = 2;
			ComparisonFileDialog.RestoreDirectory = true;
			ComparisonFileDialog.Title = "Save Comparison File";

			if(ComparisonFileDialog.ShowDialog() == true)
			{
				string copyText = "";
				foreach (string entry in ParmListFile1)
				{
					copyText += entry;
				}
				
				File.WriteAllText(ComparisonFileDialog.FileName, copyText);
				
				XmlName = ComparisonFileDialog.FileName;
				XmlDocument XmlDoc = new XmlDocument();
				XmlDoc.Load(XmlName);
				XmlNodeList parms = XmlDoc.GetElementsByTagName("value");
				Trace.WriteLine(parms[0].InnerText);











			}
		}
	}
}
