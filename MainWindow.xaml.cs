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


		public MainWindow()
		{
			InitializeComponent();
			
			
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
			
			var filePathParm = string.Empty;
			var filePathConfig = string.Empty;
			var filePathZip = string.Empty;
			
			var tempFilePath = "c:\\Users\\CNC User\\log.txt";
			

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Open Zip";
			openFileDialog.InitialDirectory = "c:\\";
			openFileDialog.Filter = "zip files | *.zip";
			openFileDialog.RestoreDirectory = true;



			if (openFileDialog.ShowDialog() == true)
			{

				if(FileUploadNumber == 1)
				{
					firstFilePathZip = openFileDialog.FileName;
				}
				else
				{
					secondFilePathZip = openFileDialog.FileName;
				}

				filePathZip = openFileDialog.FileName;

				Trace.WriteLine(filePathZip);


				using (ZipArchive archive = ZipFile.OpenRead(filePathZip))
				{
					foreach (ZipArchiveEntry entry in archive.Entries)
					{
						//First sweep through report.zip, skipping other folders and non .xml files
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
							filePathParm = firstFilePathZip + "\\" + entry.FullName;
							Trace.WriteLine(filePathParm);
							if (!File.Exists(tempFilePath))
							{
								entry.ExtractToFile(tempFilePath);

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
							}


							ReadInParmValues(tempFilePath, FileUploadNumber);
							if(FileUploadNumber == 1)
							{
								ZipFile1Name.Text = displayName;
							}
							else
							{
								ZipFile2Name.Text = displayName;
							}
					


						}


					}
				}
				if (filePathParm.Equals(null))
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
