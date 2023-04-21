using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml;

namespace ReportParmandConfigTool
{
	class ReportZip
	{

		private string zip_file_path;
		//Parm number and parm value pairs for individual report zip file
		private Dictionary<int, double> parms;
		//List of axes for a configuration file; each list entry is a single axis,
		//Each dictionary is label-value pairs for an axis
		private List<Dictionary<string, string>> configurations;
		private string nickname;
		private string date;

		public void Set_Report_Path(string path)
		{
			zip_file_path = path;
		}

		public void Set_Nickname(string name)
		{
			nickname = name;
		}

		public void Set_Date(string _date)
		{
			date = _date;
		}

		public string Get_Nickname()
		{
			return nickname;
		}

		public string Get_Date()
		{
			return date;
		}

		public void Set_Dict(Dictionary<int, double> parameters)
		{
			parms = parameters;
		}

		public void Set_Dict_Config(List<Dictionary<string, string>> configs)
		{
			configurations = configs;
		}

		public List<Dictionary<string, string>> Get_Dict_Config()
		{
			return configurations;
		}

		public string Get_Path()
		{
			return zip_file_path;
		}


		/* Author: Noah Snare
		 * Method Name: Populate_Parm
		 * Called in: Add_Item (MainWindow.xaml.cs)
		 * Purpose: When new report is added in main screen, 
		 * .zip file is parsed to find a parameter file. 
		 * The parm values in the file populate the "parms" variable
		 * of the current ReportZip object. This object 
		 * is then added to the MainWindow.xaml.cs variable 
		 * "zips", containing all current zips being handled.
		 */


		public void Populate_Parm(string path)
		{
			//Read in file
			using (FileStream file = File.OpenRead(path))
			{
				//Begin parsing zip
				using (ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Read))
				{
					//Go through each file in the .zip until a parameter file is found
					foreach (ZipArchiveEntry entry in zip.Entries)
					{
						//If parm file found
						if (entry.FullName.Equals("cncm.prm.xml") || entry.FullName.Equals("cnct.prm.xml"))
						{
							//Open the parm file into steam
							using (StreamReader sr = new StreamReader(entry.Open()))
							{
								//Parse as XML file
								XmlDocument XmlDocParms = new XmlDocument();
								XmlDocParms.Load(sr);
								XmlElement root = XmlDocParms.DocumentElement;
								//Get list of parameters
								XmlNodeList elemList1 = root.GetElementsByTagName("value");
								//Create dictionary corresponding to the parameters
								Dictionary<int, double> ps = new Dictionary<int, double>();
								//Add all values to this dictionary
								for (int i = 0; i < elemList1.Count; i++)
								{
									ps.Add(i, Double.Parse(elemList1[i].InnerText));
								}
								//Set parms value of currect ReportZip object to ps
								Set_Dict(ps);
							}
						}

					}
				}
			}
		}



		/* Author: Noah Snare
		 * Method Name: Populate_Config
		 * Called in: Add_Item (MainWindow.xaml.cs)
		 * Purpose: Same function as Populate_Parm, for the Config file
		 */



		public void Populate_Config(string path)
		{

			using (FileStream file = File.OpenRead(path))
			{
				using (ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Read))
				{
					foreach (ZipArchiveEntry entry in zip.Entries)
					{
						if (entry.FullName.Equals("cncmcfg.xml") || entry.FullName.Equals("cnctcfg.xml"))
						{
							using (StreamReader sr = new StreamReader(entry.Open()))
							{
								//Because the config files are not "Proper" xml files, the parsing is slightly different
								XmlDocument XmlDocConfigs = new XmlDocument();
								XmlDocConfigs.Load(sr);
								//Create empty variable to fill with configuration settings
								List<Dictionary<string, string>> configurations = new List<Dictionary<string, string>>();
								//Xml doc is collapsed into each line of text; each line is parsed and cleaned up for
								//proper formatting
								foreach (XmlElement config in XmlDocConfigs.DocumentElement.SelectNodes("*"))
								{
									//config.OuterXml is entire text of a single line in configuration file
									string outstr = config.OuterXml;
									Dictionary<string, string> elements = new Dictionary<string, string>();
									//See method; MakeXmlEntryArray contains many method calls to parse and clean the line
									//before it can be added to configurations
									elements = MakeXmlEntryArray(outstr);
									//Add finished product to the list of dictionarys containing axis info
									configurations.Add(elements);



								}

								Set_Dict_Config(configurations);




							}
						}

					}
				}
			}
		}

		public int GetLongestOfConfigurationStrings(ReportZip zip)
		{
			int re_int = 0;
			foreach (var config_entry in zip.configurations)
			{
				string AxisLabel = "";
				if (config_entry.TryGetValue("Label", out AxisLabel) == false)
					continue;
				if (AxisLabel == "N")
					continue;

				foreach (var keys in config_entry.Keys)
				{

					if (keys.Length > re_int)
						re_int = keys.Length;
				}

				foreach (var vals in config_entry.Values)
				{
					if (vals.Length > re_int)
						re_int = vals.Length;
				}




			}

			if (zip.Get_Nickname().Length > re_int)
				re_int = zip.Get_Nickname().Length;


			return re_int;
		}


		/* Author: Noah Snare
		 * Method Name: MakeXmlEntryArray
		 * Called in: Populate_Config 
		 * Purpose: Raw line of config xml file is very "messy"; to parse, 
		 * this method breaks it up by different symbols to create a cleaner 
		 * output.
		 */

		public Dictionary<string, string> MakeXmlEntryArray(string xml_line)
		{
			//Empty dictionary to put a line (or axis) of ocnfig file into
			Dictionary<string, string> out_dict = new Dictionary<string, string>();
			//Find each value assignment in axis
			int num_of_values = Occurences(xml_line, '=');
			//Position of each equals sign
			int pos;
			//Unseparated name of value (i.e. "HomeFastJog")
			string raw_name_of_val;
			//Spaced out name ("Home Fast Jog")
			string name_of_val;
			//Value of axis setting 
			string val;
			//Go through each variable assignment
			for (int i = 1; i <= num_of_values; i++)
			{

				pos = GetNthPosition(xml_line, '=', i);
				raw_name_of_val = StringBetweenBacktrack(xml_line, pos, '_');
				name_of_val = SpaceOutString(raw_name_of_val);
				val = ExtractValue(xml_line, pos);
				out_dict.Add(name_of_val, val);


			}

			return out_dict;
		}



		/* Author: Noah Snare
		 * Method Name: StringBetweenBacktrack
		 * Called in: MakeXmlEntryArray 
		 * Purpose: given a position, a string, and a character to find, works backward to extract new string between position and character
		 * For example: StrBtwnChars("Hello_Earth", 9, '_')  starts at 't', backtracks to '_', and returns string between them ("Ear")
		 */


		public string StringBetweenBacktrack(string line, int pos, char find)
		{

			StringBuilder re_str = new StringBuilder();
			if (pos < 1 || pos > line.Length - 1)
			{
				MessageBox.Show("Fatal error; invalid position");
				return "";
			}

			if (line.ElementAt(pos) == find)
			{
				return "";
			}

			while (pos > 1)
			{

				if (line.ElementAt(pos) == find)
				{
					re_str.Remove(0, 1);
					return re_str.ToString();
				}
				pos--;
				re_str.Insert(0, line.ElementAt(pos));


			}

			MessageBox.Show("No matching string found");
			return "";
		}

		/* Author: Noah Snare
	 * Method Name: StpaceOutString
	 * Called in: MakeXmlEntryArray 
	 * Purpose: given a string, find capitalized letters and separate by these (following the format
	 * in the config files)
	 * 
	 */


		public string SpaceOutString(string line)
		{
			StringBuilder re_str = new StringBuilder();
			int pos_start = 1;
			int pos_end = 1;
			foreach (char ch in line)
			{

				if (Char.IsUpper(ch) && line.IndexOf(ch) != 0)
				{
					re_str.Append(" ");
				}

				re_str.Append(ch);
			}

			return re_str.ToString();
		}
		/* Author: Noah Snare
	 * Method Name: GetNthPosition
	 * Called in: MakeXmlEntryArray 
	 * Purpose: given a string, a character to find, and an integer, finds the nth appearance of the 
	 * char in line and returns the position
	 */

		public int GetNthPosition(string line, char find, int nth)
		{

			int pos = 0;
			int count = 0;
			foreach (char ch in line)
			{
				if (ch == find)
				{
					count++;

					if (count == nth)
					{
						return pos;

					}


				}

				pos++;
			}

			return -1;



		}

		/* Author: Noah Snare
	 * Method Name: Occurences
	 * Called in: MakeXmlEntryArray 
	 * Purpose: given a string and a char, find how many times this char appears
	 */


		public int Occurences(string line, char look)
		{
			int occ = 0;

			foreach (char ch in line)
			{
				if (ch == look)
				{
					occ++;
				}
			}

			return occ;
		}


		/* Author: Noah Snare
	 * Method Name: ExtractValue
	 * Called in: MakeXmlEntryArray 
	 * Purpose: Finds value inside of quotation marks from beginning position
	 */


		public string ExtractValue(string line, int begin_pos)
		{
			bool seeking = true;
			StringBuilder re_str = new StringBuilder();
			while (seeking)
			{
				if (line.ElementAt(begin_pos) == '"')
				{
					seeking = false;
				}

				if (begin_pos == line.Length - 1)
				{
					seeking = false;
					MessageBox.Show("Value not found");
				}
				begin_pos++;
			}
			seeking = true;
			while (seeking)
			{
				if (line.ElementAt(begin_pos) == '"')
				{
					seeking = false;
				}
				else
				{
					re_str.Append(line.ElementAt(begin_pos));
				}


				if (begin_pos == line.Length - 1)
				{
					seeking = false;
				}
				begin_pos++;
			}

			return re_str.ToString();
		}

		/* Author: Noah Snare
	 * Method Name: CreateConfigComparisonFile
	 * Called in: CompareReportsConfig
	 * Purpose: Uses all config methods to parse cncmfg/cnctfg files and create text file with differences
	 * apparent
	 */

		public void CreateConfigComparisonFile(Dictionary<int, ReportZip> files)
		{
			//Formats text file based on longest element of files; names, values, etc.
			int LengthToAdjustBy = 0;
			//How many valid axes (Non-N) exist
			int axes_to_compare = 8;
			//Cycles each file to find the longest element
			foreach (var file in files)
			{
				//Length check
				if (GetLongestOfConfigurationStrings(file.Value) > LengthToAdjustBy)
					LengthToAdjustBy = GetLongestOfConfigurationStrings(file.Value);

				int i = 0;
				foreach (var entry in file.Value.configurations)
				{
					//Skips over N-axes
					if (entry.ContainsValue("N"))
						i++;

				}
				if (8 - i < axes_to_compare)
					axes_to_compare = 8 - i;
			}


			//Full list of possible config settings to be compared
			string[] ListOfConfigs =
			{

				"Label", "Slow Jog", "Slow Jog Probe", "Fast Jog", "Home Jog", "Fast Jog Minus",
				"Fast Jog Plus", "Fast Jog Minus Probe", "Fast Jog Plus Probe", "Max Rate", "Turns Per Unit", "Lash Comp", "Counts Per Unit", "Accel Time", "Deadstart Velocity", "Delta V Max", "Counts Per Turn", "Minus Limit", "Plus Limit", "Minus Home", "Plus Home", "Reversed", "Laser Comp", "Manual", "Triangular Rotary A0", "Triangular Rotary B", "Triangular Rotary C"

			};


			//List of existing values for the ordered list of configuration settings

			List<string> values = new List<string>();

			foreach (var file in files)
			{
				for (int i = 0; i < axes_to_compare; i++)
				{
					foreach (string config in ListOfConfigs)
					{
						//If value is present, this value is added to the ordered list of settings
						if (file.Value.configurations.ElementAt(i).Keys.Contains(config))
						{
							values.Add(file.Value.configurations.ElementAt(i)[config]);
						}
						//otherwise, it is null
						else
						{
							values.Add("No Value Assigned");
						}
					}

				}
			}
			//Header contains all report names
			string header = "";
			//Output is the complete file text to be written when file is created
			string output = "";
			//Current line to be added to output at end of current output
			string line = "";
			//Keeps track of whether report is first loaded in or not
			int index = 1;

			//Temp string; stores previous report name to space current one out for
			string temp = "";
			foreach (var file in files)
			{
				//Add formatted whitespace and then report name
				if (index == 1)
				{
					header += Create_White_Space(GetLongestOfConfigurationStrings(file.Value)) + file.Value.Get_Nickname();
					temp = file.Value.Get_Nickname();
				}
				//Otherwise, format by the previous name
				else
				{
					header += Create_White_Space(GetLongestOfConfigurationStrings(file.Value) - temp.Length) + "     " + file.Value.Get_Nickname();
					temp = file.Value.Get_Nickname();
				}

				index++;
			}

			//End of header
			//
			//

			output += header + "\n";
			//Iterate through valid axes
			for (int h = 0; h < axes_to_compare; h++)
			{
				//Go through each possible entry in config list
				for (int i = 0; i < ListOfConfigs.Length; i++)
				{
					//If at first entry ("Label") format header axis
					if (i == 0)
					{

						output += "\n";
						output += "-------------------------------------------------------------------------------------------\n";
						output += values[i + h * 27] + " Axis";
						output += "\n";
						output += "-------------------------------------------------------------------------------------------\n\n";
						i++;
					}


					if (files.Count == 2)
					{
						// Compare value
						// i represents the index within the list of configuration settings (fast jog, etc.)
						// i is shifted by which axis is currently active
						// h * 27 gives the current axis width
						// axes_to_compare * 27 gives the width of entire file


						if (values[i + h * 27] != values[i + h * 27 + axes_to_compare * 27])
						{
							line += ListOfConfigs[i] + ":";
							line += Create_White_Space(GetLongestOfConfigurationStrings(files.ElementAt(0).Value) - (ListOfConfigs[i] + ":").Length);
							line += values[i + h * 27];
							line += Create_White_Space(GetLongestOfConfigurationStrings(files.ElementAt(1).Value) - values[i + h * 27].Length);
							line += "     ";
							line += values[i + h * 27 + axes_to_compare * 27];
							line += "\n\n";
							output += line;
							line = "";

						}
						//If no difference, the line is empty
						else
						{
							line = "";
						}

					}


					if (files.Count == 3)
					{
						if (values[i + h * 27] != values[i + h * 27 + axes_to_compare * 27] || values[i + h * 27] != values[i + h * 27 + 2 * axes_to_compare * 27])
						{

							line += ListOfConfigs[i] + ":";


							line += Create_White_Space(GetLongestOfConfigurationStrings(files.ElementAt(0).Value) - (ListOfConfigs[i] + ":").Length);

							line += values[i + h * 27];

							line += Create_White_Space(GetLongestOfConfigurationStrings(files.ElementAt(1).Value) - values[i + h * 27].Length);

							line += "     ";

							line += values[i + h * 27 + axes_to_compare * 27];

							line += Create_White_Space(GetLongestOfConfigurationStrings(files.ElementAt(2).Value) - values[i + h * 27 + axes_to_compare * 27].Length);

							line += "     ";

							line += values[i + h * 27 + 2 * axes_to_compare * 27];

							line += "\n\n";

							output += line;
							line = "";
						}
					}
				}
			}

			SaveFileDialog ComparisonFileDialog = new SaveFileDialog();

			ComparisonFileDialog.Filter = "txt files (*.txt) |*.txt";
			ComparisonFileDialog.FilterIndex = 2;
			ComparisonFileDialog.RestoreDirectory = true;
			ComparisonFileDialog.Title = "Save Comparison File";

			if (ComparisonFileDialog.ShowDialog() == true)
			{
				File.WriteAllText(ComparisonFileDialog.FileName, output);
			}



		}

		public void CreateParmComparisonFile(Dictionary<int, ReportZip> files)
		{

			List<int> list_of_longest_elements = new List<int>();
			string output_text = "";
			string header = "";
			int report_index = 1;
			header += "Parameter|  ";
			string report_name = "";

			foreach (var entity in files)
			{
				int remaining_length;

				if (entity.Value.nickname.Equals(""))
				{
					report_name += "Report #" + report_index;
					report_name += Create_White_Space(Get_Longest_String(entity.Value) - report_name.Length + 4);
					report_index += 1;
					header += report_name;
					report_name = "";


				}
				else
				{
					header += entity.Value.Get_Nickname() + Create_White_Space(Get_Longest_String(entity.Value) - entity.Value.Get_Nickname().Length + 4);

				}



			}

			header += "\n";

			output_text += header;

			string parm_text = "";
			string parm_line = "";

			List<double> _vals = new List<double>();
			int longest = 0;
			for (int i = 0; i <= 999; i++)
			{
				parm_line += "      ";


				if (i.ToString().Length == 1)

				{

					parm_line += "00";

				}

				if (i.ToString().Length == 2)

				{

					parm_line += "0";

				}

				parm_line += i.ToString() + "|  ";


				foreach (var entity in files)
				{



					longest = Get_Longest_String(entity.Value);
					list_of_longest_elements.Add(longest);

					double ex = entity.Value.parms.ElementAt(i).Value;
					_vals.Add(ex);

				}

				if (IsIdenticalList(_vals))
				{
					parm_line = "";
				}
				else
				{
					parm_line += ParmAddLine(_vals, list_of_longest_elements) + "\n";

				}
				parm_text += parm_line;
				parm_line = "";
				longest = 0;
				_vals.Clear();

			}



			output_text += parm_text;
			SaveFileDialog ComparisonFileDialog = new SaveFileDialog();

			ComparisonFileDialog.Filter = "txt files (*.txt) |*.txt";
			ComparisonFileDialog.FilterIndex = 2;
			ComparisonFileDialog.RestoreDirectory = true;
			ComparisonFileDialog.Title = "Save Comparison File";

			if (ComparisonFileDialog.ShowDialog() == true)
			{
				File.WriteAllText(ComparisonFileDialog.FileName, output_text);
			}

		}



		//For the purpose of creating output files, you need the longest string across all variables
		//(parms, configs, report titles, etc.) so things don't overlap and can be properly formatted

		public int Get_Longest_String(ReportZip curr_zip)
		{
			int longest = 0;

			foreach (var parameter in curr_zip.parms)
			{
				if (parameter.Key.ToString().Length > longest)
					longest = parameter.Key.ToString().Length;
				if (parameter.Value.ToString().Length > longest)
					longest = parameter.Value.ToString().Length;
			}

			int[] AllStrings = { curr_zip.Get_Nickname().Length, longest };
			return AllStrings.Max();
		}
		//Generates as many spaces as you need
		public string Create_White_Space(int spaces)
		{
			string text = "";
			if (spaces == 0)
				return text;
			for (int i = 1; i <= spaces; i++)
			{
				text += " ";
			}
			return text;
		}

		public bool IsIdenticalList(List<double> _complist)
		{
			double to_compare = _complist.FirstOrDefault();
			foreach (double val in _complist)
			{
				if (val == to_compare)
				{
					continue;
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		public string ParmAddLine(List<double> _addlist, List<int> longest)
		{
			string ret = "";
			int index = 0;
			foreach (var val in _addlist)
			{
				ret += val.ToString() + Create_White_Space(longest[index] - val.ToString().Length) + "    ";
				index++;
			}

			return ret;


		}

		public string AddSeparator(List<int> longest)
		{
			string ret = "         |";

			for (int i = 0; i < longest.FirstOrDefault(); i++)
			{
				ret += "-";
			}

			return ret;
		}

		public void AddNickNames(Dictionary<int, ReportZip> files)
		{
			int index = 1;
			Window otherwin = Application.Current.MainWindow;
			foreach (var file in files)
			{
				switch (index)
				{
					case 1:
						file.Value.Set_Nickname((otherwin as MainWindow).Report1Nickname.Text);
						break;
					case 2:
						file.Value.Set_Nickname((otherwin as MainWindow).Report2Nickname.Text);
						break;
					case 3:
						file.Value.Set_Nickname((otherwin as MainWindow).Report3Nickname.Text);
						break;
				}
				index++;
			}
		}



	}






}
