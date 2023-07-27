using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCIS
{
    public struct Cell
    {
        public Cell(int row, int col, string value)
        {
            this.row = row;
            this.col = col;
            this.value = value;
        }

        public int row { get; set; }
        public int col { get; set; }
        public string value { get; set; }
    }
    public class WorkbookWrapper 
    {
        public Workbook wb;
        public WorksheetCollection wsc;
        

        public WorkbookWrapper()
        {
            var enc = CodePagesEncodingProvider.Instance.GetEncoding(1252);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this.wb = new Workbook();
            this.wsc = wb.Worksheets;
        }

        public void generateWorksheet(string name)
        {
            var worksheetIndex = this.wsc.Add();
            var ws = this.wsc[worksheetIndex];
            ws.Name = name;
        }

        public void removeWorksheet(string name)
        {
            var worksheet = this.wb.Worksheets[name];
            this.wb.Worksheets.RemoveAt(worksheet.Name);
        }

        public void put(string worksheetName, Cell cell)
        {
            wsc[worksheetName].Cells[cell.row, cell.col].PutValue(cell.value);
        }

        public void put(string worksheetName, List<Cell> cells)
        {
            foreach (var cell in cells)
                put(worksheetName, cell);
        }

        public int maxRowIdx(string worksheetName)
        {
            return wsc[worksheetName].Cells.MaxDataRow;
        }

        public int maxColIdx(string worksheetName)
        {
            return wsc[worksheetName].Cells.MaxDataColumn;
        }

        public void save(string filePath)
        {
            this.wb.Save(filePath);
        }
    }
}
