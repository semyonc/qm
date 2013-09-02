//        Copyright (c) 2011, Semyon A. Chertkov (semyonc@gmail.com)
//        All rights reserved.
//
//        This program is free software: you can redistribute it and/or modify
//        it under the terms of the GNU General Public License as published by
//        the Free Software Foundation, either version 3 of the License, or
//        any later version.

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.Globalization;

namespace DataEngine.XL
{
    public class Loader: IDisposable
    {
        SpreadsheetDocument document;
        string packageName;

        public Loader(string fileName)
        {
            MultiFile = false;
            packageName = fileName;
            document = SpreadsheetDocument.Open(fileName, false);
        }

        ~Loader()
        {
            Dispose();
        }

        public bool HasHeader { get; private set; }

        public bool HasFormula { get; private set; }

        public bool MultiFile { get; set; }

        public WorksheetData Load(string sheetName)
        {
            IEnumerable<Sheet> sheets = document.WorkbookPart.Workbook.Descendants<Sheet>();
            if (!String.IsNullOrEmpty(sheetName))
                sheets = sheets.Where(s => s.Name == sheetName);
            if (sheets.Count() == 0)
            {
                if (MultiFile)
                    return null;
                throw new ESQLException(Properties.Resources.XlSheetNotExists, sheetName, Path.GetFileName(packageName));
            }
            List<String> sharedStrings = new List<string>();
            if (document.WorkbookPart.SharedStringTablePart != null)
            {
                IEnumerable<String> strings = document.WorkbookPart.SharedStringTablePart
                    .SharedStringTable.Descendants<Text>().Select(t => t.InnerText);
                sharedStrings.AddRange(strings);
            }
            WorkbookStylesPart stylePart = (WorkbookStylesPart)document.WorkbookPart.Parts
                .Where(p => p.OpenXmlPart is WorkbookStylesPart).Select(p => p.OpenXmlPart).FirstOrDefault();
            NumberingFormats numberingFormats = stylePart.Stylesheet.Descendants<NumberingFormats>().FirstOrDefault();
            CellFormats cellFormats = stylePart.Stylesheet.Descendants<CellFormats>().FirstOrDefault();
            HashSet<uint> dateTimeStyleIndex = GetDateTimeStyle(GetDateTimeFmtId(numberingFormats), cellFormats);
            WorksheetPart worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheets.First().Id);
            WorksheetData worksheetData = new WorksheetData();
            HasHeader = worksheetPart.Worksheet.GetAttributes().Where(a => (a.LocalName == "hdr" &&
                a.NamespaceUri == "http://www.wmhelp.com/ext" && a.Value == "yes")).Count() == 1;
            foreach (Row row in worksheetPart.Worksheet.Descendants<Row>())
            {
                int rowIndex = (int)(uint)row.RowIndex;
                WorksheetData.SparseRow sparseRow = worksheetData.GetRow(rowIndex, true);
                foreach (Cell cell in row.Descendants<Cell>())
                {
                    CellRef cellRef = CellRef.ParseRef(cell.CellReference);
                    if (rowIndex != cellRef.R1)
                        throw new ESQLException(Properties.Resources.XlBadFormat, cell.CellReference);
                    if (cell.CellFormula != null)
                        HasFormula = true;
                    if (cell.CellValue != null)
                    {
                        if (!(cell.DataType != null && cell.DataType == CellValues.SharedString) && 
                              cell.StyleIndex != null && dateTimeStyleIndex.Contains(cell.StyleIndex))
                            sparseRow[cellRef.C1] = GetCellDateValue(cell);
                        else
                        {
                            if (cell.DataType != null)
                            {
                                if (cell.DataType == CellValues.Date)
                                    sparseRow[cellRef.C1] = GetCellDateValue(cell);
                                else if (cell.DataType == CellValues.SharedString)
                                    sparseRow[cellRef.C1] = GetCellSharedStringValue(cell, sharedStrings);
                                else if (cell.DataType == CellValues.Boolean)
                                    sparseRow[cellRef.C1] = GetCellBooleanValue(cell);
                                else if (cell.DataType == CellValues.Error)
                                    sparseRow[cellRef.C1] = new CellError();
                                else
                                    sparseRow[cellRef.C1] = GetCellGeneralValue(cell);
                            }
                            else
                                sparseRow[cellRef.C1] = GetCellGeneralValue(cell);
                        }
                    }
                    else if (cell.InlineString != null)
                        sparseRow[cellRef.C1] = cell.InlineString.Text;
                }
            }
            return worksheetData;
        }

        private HashSet<uint> GetDateTimeFmtId(NumberingFormats numFormats)
        {
            HashSet<uint> fmtIds = new HashSet<uint>();
            fmtIds.Add(14); //14 = 'mm-dd-yy';
            fmtIds.Add(15); //15 = 'd-mmm-yy';
            fmtIds.Add(16); //16 = 'd-mmm';
            fmtIds.Add(17); //17 = 'mmm-yy';
            fmtIds.Add(18); //18 = 'h:mm AM/PM';
            fmtIds.Add(19); //19 = 'h:mm:ss AM/PM';
            fmtIds.Add(20); //20 = 'h:mm';
            fmtIds.Add(21); //21 = 'h:mm:ss';
            fmtIds.Add(22); //22 = 'm/d/yy h:mm';
            if (numFormats != null)
                foreach (var f in numFormats.Descendants<NumberingFormat>())
                {
                    string formatCode = f.FormatCode;
                    if (formatCode.Contains("m") || formatCode.Contains("d") ||
                        formatCode.Contains("Days") || formatCode.Contains("Years") ||
                        formatCode.Contains("Months") || formatCode.Contains("yy") ||
                        formatCode.Contains("h") || formatCode.Contains("ss"))
                        fmtIds.Add(f.NumberFormatId);
                }
            return fmtIds;
        }

        private HashSet<uint> GetDateTimeStyle(HashSet<uint> fmtIds, CellFormats cellFormats)
        {
            HashSet<uint> styleIds = new HashSet<uint>();
            uint id = 0;
            if (cellFormats != null)
                foreach (CellFormat xf in cellFormats.Descendants<CellFormat>())
                {
                    if (xf.ApplyNumberFormat != null && xf.ApplyNumberFormat && 
                        fmtIds.Contains(xf.NumberFormatId))
                        styleIds.Add(id);
                    id++;
                }
            return styleIds;
        }

        private object GetCellDateValue(Cell cell)
        {
            try
            {
                double oaDate = Double.Parse(cell.CellValue.Text, CultureInfo.InvariantCulture);
                return DateTime.FromOADate(oaDate);
            }
            catch (Exception ex)
            {
                throw new ESQLException(ex, Properties.Resources.XlBadFormat, cell.CellReference);
            }
        }


        private Object GetCellSharedStringValue(Cell cell, List<String> sharedStrings)
        {
            try
            {
                int index = Int32.Parse(cell.CellValue.Text);
                return sharedStrings[index];
            }
            catch (Exception ex)
            {
                throw new ESQLException(ex, Properties.Resources.XlBadFormat, cell.CellReference);
            }
        }

        private Object GetCellGeneralValue(Cell cell)
        {
            if (cell.CellValue != null)
            {
                double num; 
                if (Double.TryParse(cell.CellValue.Text, NumberStyles.Float, 
                    CultureInfo.InvariantCulture, out num))
                    return num;
                else
                    return cell.CellValue.Text;
            }
            return null;
        }

        private Object GetCellBooleanValue(Cell cell)
        {
            if (cell.CellValue != null)
            {
                if (cell.CellValue.Text == "1")
                    return true;
                else if (cell.CellValue.Text == "0")
                    return false;
            }
            throw new ESQLException(Properties.Resources.XlBadFormat, cell.CellReference);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (document != null)
            {
                document.Close();
                document = null;
            }
        }

        #endregion
    }
}
