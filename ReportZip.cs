using ReportComparison;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace ReportComparisonTool
{
    class ReportZip
    {
 
        private string zip_file_path;
        private Dictionary<int, double> parms;
        private string nickname;
        private string date;

   

        public ReportZip()
        {
       
            
           
        }

        public void Set_Report_Path(string path)
        {
            zip_file_path = path;
            if(nickname == null)
			{
                nickname = path;
			}
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

        public string Get_Path()
		{
            return zip_file_path;
		}


        public void Populate_Config(string path)
		{

		}
        
        public void Populate_Parm(string path)
		{
            
            using(FileStream file = File.OpenRead(path))
			{
                using (ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in zip.Entries)
                    {
                        if (entry.FullName.Equals("cncm.prm.xml"))
                        {
                            using (StreamReader sr = new StreamReader(entry.Open()))
                            {
                                XmlDocument XmlDoc1 = new XmlDocument();
                                XmlDoc1.Load(sr);
                                XmlElement root = XmlDoc1.DocumentElement;
                                XmlNodeList elemList1 = root.GetElementsByTagName("value");


                                Dictionary<int, double> ps = new Dictionary<int, double>();
                                for (int i = 0; i < elemList1.Count; i++)
                                {
                                    
                                    ps.Add(i, Double.Parse(elemList1[i].InnerText));
                                    
                                }
                                Set_Dict(ps);
                                foreach (var parameter in parms)
                                {
                                    Trace.WriteLine($"{parameter.Key}: {parameter.Value}");
                                }

                            }
                        }

                    } 
				}
			}
		}

        public void CreateComparisonFile(List<ReportZip> files)
		{
            string output_text;
            string header = "";
            int report_index = 1;
            header += "Parameter|  ";
            foreach(ReportZip entity in files)
			{
                header += "Report " + report_index;
			}
           
		}




    }
    
}
