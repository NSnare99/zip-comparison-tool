using Microsoft.Win32;
using System.Diagnostics;
using System.Windows;
using System.IO;
using System;
using System.IO.Compression;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ReportComparison
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private string[] ParmListFile1;
		private string[] ParmListFile2;
		private List<string> ComparatorList;
 
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
				ParmListFile1 = parmList.Select(i => i.ToString()).ToArray();
			}
			else
			{
				ParmListFile2 = parmList.Select(i => i.ToString()).ToArray();
			}
			
		}

		private void ScanZipForSettingsFile(int FileUploadNumber, string SettingsFile)
		{
			var fileContentParm = string.Empty;
			var filePathParm = string.Empty;
			var filePathZip = string.Empty;
			var tempFilePath = "c:\\Users\\CNC User\\log.txt";
			

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Open Zip";
			openFileDialog.InitialDirectory = "c:\\";
			openFileDialog.Filter = "zip files | *.zip";
			openFileDialog.RestoreDirectory = true;



			if (openFileDialog.ShowDialog() == true)
			{


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

						if (entry.FullName.Equals(SettingsFile))
						{
							filePathParm = filePathZip + "\\" + entry.FullName;
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

		private void Button_Click_File1(object sender, RoutedEventArgs e)

		{

			ScanZipForSettingsFile(1, "cncm.prm.xml");
			ScanZipForSettingsFile(1, "cncmcfg.xml");


		}

		private void Button_Click_File2(object sender, RoutedEventArgs e)

		{

			ScanZipForSettingsFile(2, "cncm.prm.xml");


		}

		private void AddOrRemoveComparator_Parm(object sender, RoutedEventArgs e)
		{
			if (ParmSelect.IsChecked == true)
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


		}
	}
}
