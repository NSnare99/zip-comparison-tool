using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Windows;



namespace ReportParmandConfigTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private Dictionary<int, ReportParmandConfigTool.ReportZip> zips = new Dictionary<int, ReportParmandConfigTool.ReportZip>();


		public MainWindow()
		{
			InitializeComponent();
		}







		private void AddItem(object sender, RoutedEventArgs e)
		{

			int AddIndex = 0;

			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Title = "Open Zip";
			openFileDialog.InitialDirectory = "c:\\";
			openFileDialog.Filter = "zip files | *.zip";
			openFileDialog.RestoreDirectory = true;
			ReportZip zip = new ReportZip();

			if (openFileDialog.ShowDialog() == true)
			{

				if (sender.Equals(AddFile1))
				{
					Report1Upload.Text = openFileDialog.FileName;
					AddIndex = 0;
				}

				if (sender.Equals(AddFile2))
				{
					Report2Upload.Text = openFileDialog.FileName;
					AddIndex = 1;
				}

				if (sender.Equals(AddFile3))
				{
					Report3Upload.Text = openFileDialog.FileName;
					AddIndex = 2;
				}

				zip.Set_Report_Path(openFileDialog.FileName);
				zip.Populate_Parm(zip.Get_Path());
				zip.Populate_Config(zip.Get_Path());
				zips.Add(AddIndex, zip);
			}



		}



		private void RemoveItem(object sender, RoutedEventArgs e)
		{



			if (sender == RemoveFile1)
			{
				if (Report1Upload.Text != "")

				{
					zips.Remove(0);
					Report1Upload.Text = "";
					Report1Nickname.Text = "";
				}

				if (Report2Upload.Text != "")
				{
					Report1Upload.Text = Report2Upload.Text;
					Report1Nickname.Text = Report2Nickname.Text;
					Report2Upload.Text = "";
					Report2Nickname.Text = "";
					ReportZip tempZip = zips[1];
					zips.Remove(1);
					zips.Add(0, tempZip);
				}

				if (Report3Upload.Text != "")
				{
					Report2Upload.Text = Report3Upload.Text;
					Report2Nickname.Text = Report3Nickname.Text;
					Report3Upload.Text = "";
					Report3Nickname.Text = "";
					ReportZip tempZip = zips[2];
					zips.Remove(2);
					zips.Add(1, tempZip);
				}
				else
				{
					Report2Nickname.Text = "";
				}

			}

			if (sender == RemoveFile2)
			{
				if (Report2Upload.Text != "")

				{
					zips.Remove(1);
					Report2Upload.Text = "";
					Report2Nickname.Text = "";

					if (Report3Upload.Text != "")
					{
						Report2Upload.Text = Report3Upload.Text;
						Report2Nickname.Text = Report3Nickname.Text;
						Report3Upload.Text = "";
						Report3Nickname.Text = "";
						ReportZip tempZip = zips[2];
						zips.Remove(2);
						zips.Add(1, tempZip);
					}




				}


			}

			if (sender == RemoveFile3)
			{
				if (Report3Upload.Text != "")

				{
					zips.Remove(2);
					Report3Upload.Text = "";
					Report3Nickname.Text = "";

				}
			}
		}

		private void CompareReports(object sender, RoutedEventArgs e)
		{
			//Go through each report boxes when button is pressed and add nicknames accordingly
			int PlacementOfZipsNickname = 0;

			if (Report1Upload.Text != "")
			{
				if (Report1Nickname.Text == "")
				{
					Report1Nickname.Text = "Report #1";
					zips.ElementAt(PlacementOfZipsNickname).Value.Set_Nickname(Report1Nickname.Text);
					PlacementOfZipsNickname++;
				}
				else
				{
					zips.ElementAt(PlacementOfZipsNickname).Value.Set_Nickname(Report1Nickname.Text);
					PlacementOfZipsNickname++;
				}
			}

			if (Report2Upload.Text != "")
			{
				if (Report2Nickname.Text == "")
				{
					Report2Nickname.Text = "Report #2";
					zips.ElementAt(PlacementOfZipsNickname).Value.Set_Nickname(Report2Nickname.Text);
					PlacementOfZipsNickname++;
				}
				else
				{
					zips.ElementAt(PlacementOfZipsNickname).Value.Set_Nickname(Report2Nickname.Text);
					PlacementOfZipsNickname++;
				}
			}

			if (Report3Upload.Text != "")
			{
				if (Report3Nickname.Text == "")
				{
					Report3Nickname.Text = "Report #3";
					zips.ElementAt(PlacementOfZipsNickname).Value.Set_Nickname(Report3Nickname.Text);
					PlacementOfZipsNickname++;
				}
				else
				{
					zips.ElementAt(PlacementOfZipsNickname).Value.Set_Nickname(Report3Nickname.Text);
					PlacementOfZipsNickname++;
				}
			}
			ReportParmandConfigTool.ReportZip zip = new ReportParmandConfigTool.ReportZip();
			if (sender.Equals(CompareReportsButtonConfig))
			{
				zip.CreateConfigComparisonFile(zips);
			}
			else
			{
				zip.CreateParmComparisonFile(zips);
			}


		}

	}
}
