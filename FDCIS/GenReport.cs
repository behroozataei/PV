using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DCIS
{
   
    internal class GenReport
    {
        private WorkbookWrapper workbook;
        private int RowCount = 0;
        public GenReport()
        {
            workbook = new WorkbookWrapper();
            workbook.generateWorksheet("Consumed");
            workbook.put("Consumed", new List<Cell> {
            new Cell(0, 0, "NetworkID"),
            new Cell(0, 1, "Value"),
            });
            RowCount++;
        }

        public void AddConsumed(String NetworkID, float value)
        {
            Cell networkID = new Cell(RowCount, 0, NetworkID);
            Cell consumedValue = new Cell(RowCount, 1, value.ToString());
            workbook.put("Consumed", networkID);
            workbook.put("Consumed", consumedValue);
            RowCount++;
        }
        public void save()
        {
            string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string strWorkPath = $@"{ System.IO.Path.GetDirectoryName(strExeFilePath)}//rep";
            Console.WriteLine(strWorkPath);
            var datetime = DateTime.Now.ToString().Replace("/", "_").Replace(":", "_");

            string fileFullname = Path.Combine(strWorkPath, "Output.xlsx");

            if (File.Exists(fileFullname))
            {
                fileFullname = Path.Combine(strWorkPath, "Output_" + datetime + ".xlsx");
            }
            workbook.save(fileFullname);
        }


    }
}
