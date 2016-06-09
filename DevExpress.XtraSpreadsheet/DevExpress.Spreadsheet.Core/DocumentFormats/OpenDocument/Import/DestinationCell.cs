#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

#if OPENDOCUMENT
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Export.OpenDocument;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenDocument {
	#region CellDestination
	public class CellDestination : ElementDestination {
		#region Static members
		static readonly Dictionary<string, CellValueOdsType> CellValueOdsTypeTable = DictionaryUtils.CreateBackTranslationTable<CellValueOdsType>(OpenDocumentExporter.CellValueOdsTypeTable);
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static readonly Dictionary<CellValueOdsType, TypeHandler> typeHandlerTable = CreateTypeHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable { { "p", OnParagraph } }; 
			return result;
		}
		static CellDestination GetThis(OpenDocumentWorkbookImporter importer) {
			return (CellDestination)importer.PeekDestination();
		}
		static Dictionary<CellValueOdsType, TypeHandler> CreateTypeHandlerTable() {
			Dictionary<CellValueOdsType, TypeHandler> result = new Dictionary<CellValueOdsType, TypeHandler>();
			result.Add(CellValueOdsType.Float, ReadNumeric);
			result.Add(CellValueOdsType.Currency, ReadNumeric);
			result.Add(CellValueOdsType.Percentage, ReadNumeric);
			result.Add(CellValueOdsType.Date, ReadDate);
			result.Add(CellValueOdsType.Time, ReadTime);
			result.Add(CellValueOdsType.Boolean, ReadBoolean);
			result.Add(CellValueOdsType.String, ReadString);
			return result;
		}
		#endregion
		#region Fields
		delegate VariantValue TypeHandler(OpenDocumentWorkbookImporter importer, XmlReader reader);
		RowDestination parentDestination;
		ICell cell;
		CellPosition currentCellPos;
		CellValueOdsType valueType;
		VariantValue cellValue;
		StringBuilder cellTextBuilder;
		string styleName;
		string formula;
		CellRange formulaRange;
		int numberColumnsRepeated;
		#endregion
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public CellDestination(OpenDocumentWorkbookImporter importer, RowDestination destination)
			: base(importer) {
			this.cellTextBuilder = new StringBuilder();
			this.parentDestination = destination;
			ParagraphDestination.RefreshParagraphsCount();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			currentCellPos = new CellPosition(Importer.IndexNextCell, Importer.IndexNextRow);
			ReadCellValue(reader);
			ReadFormula(reader);
			MergeCells(reader);
			numberColumnsRepeated = Importer.GetWpSTIntegerValue(reader, "table:number-columns-repeated", 1);
			styleName = reader.GetAttribute("table:style-name");
			cell = Importer.TryCreateCellAndApplyFormat(valueType, styleName, parentDestination);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (cell != null) {
				string cellText = cellTextBuilder.ToString();
				ICellError errorValue = CellErrorFactory.CreateError(cellText, Importer.DocumentModel.DataContext);
				if (errorValue != null)
					cellValue = errorValue.Value;
				else
					if (valueType == CellValueOdsType.String)
						cellValue = cellText;
				cell.AssignValueCore(cellValue);
				Regex regex = new Regex(@"Err:\d\d\d");
				Match match = regex.Match(cellText);
				if (match.Success && cellText.Length == 7) {
					formula = null;
				}
				Importer.SetFormula(cell, formula, formulaRange);
			}
			Importer.IndexNextCell += numberColumnsRepeated;
		}
		void ReadCellValue(XmlReader reader) {
			valueType = Importer.GetWpEnumValue<CellValueOdsType>(reader, "office:value-type", CellValueOdsTypeTable, CellValueOdsType.None);
			if (valueType == CellValueOdsType.None)
				return;
			TypeHandler typeHandler;
			if (!typeHandlerTable.TryGetValue(valueType, out typeHandler))
				Importer.ThrowInvalidFile();
			cellValue = typeHandler(Importer, reader);
		}
		void ReadFormula(XmlReader reader) {
			formula = reader.GetAttribute("table:formula");
			formulaRange = Importer.GetSpannedRange(reader, currentCellPos, true);
		}
		void MergeCells(XmlReader reader) {
			CellRange range = Importer.GetSpannedRange(reader, currentCellPos, false);
			if (range == null)
				return;
			if (range.CellCount > 1)
				Importer.CurrentSheet.MergedCells.Add(range);
		}
		#region Handlers
		static Destination OnParagraph(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			CellDestination destination = GetThis(importer);
			CellRange hyperlinkRange = new CellRange(importer.CurrentSheet, destination.currentCellPos, destination.currentCellPos);
			return new ParagraphDestination(importer, hyperlinkRange, destination.cellTextBuilder);
		}
		#endregion
		#region ReadCellValueHandlers
		static VariantValue ReadNumeric(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return importer.GetWpDoubleValue(reader, "office:value", 0);
		}
		static VariantValue ReadDate(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			string strDate = reader.GetAttribute("office:date-value");
			VariantValue variantValue = new VariantValue();
			DateTime dateValue;
			if (!DateTime.TryParse(strDate, out dateValue))
				return variantValue;
			variantValue.SetDateTime(dateValue, importer.DocumentModel.DataContext);
			if (variantValue.IsError && variantValue.ErrorValue.Value == VariantValue.ErrorInvalidValueInFunction) {
				importer.DocumentModel.LogMessage(Office.Services.LogCategory.Warning, Localization.XtraSpreadsheetStringId.Msg_OdsTooLowDateValue);
				return strDate.Replace("T", " ");
			}
			return variantValue;
		}
		static VariantValue ReadTime(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			Regex regex = new Regex(@"PT(?<hours>\d+)H(?<minutes>\d+)M(?<seconds>\d+)(?<milliseconds>\.\d+)?S");
			string time = regex.Replace(reader.GetAttribute("office:time-value"), delegate(Match m){
				string ms = m.Groups["milliseconds"].Value;
				string result = string.Concat(m.Groups["hours"].Value, ":", m.Groups["minutes"].Value, ":", m.Groups["seconds"]);
				return string.IsNullOrEmpty(ms) ? result : string.Concat(result, ms);
			});
			return time.Equals("24:00:00", StringComparison.OrdinalIgnoreCase) ? 1 : importer.DocumentModel.DataContext.TryConvertStringToDateTimeValue(time, false).Value;
		}
		static VariantValue ReadBoolean(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			string value = importer.ReadAttribute(reader, "office:boolean-value");
			bool result;
			if (bool.TryParse(value, out result))
				return result;
			double num;
			if (double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out num))
				return num != 0;
			return false;
		}
		static VariantValue ReadString(OpenDocumentWorkbookImporter importer, XmlReader reader) {
			return VariantValue.Empty;
		}
		#endregion
	}
	#endregion
}
#endif
