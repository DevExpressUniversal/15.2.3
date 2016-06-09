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

using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region CellDestinationBase (abstract class)
	public abstract class CellDestinationBase<T> : ElementDestination<SpreadsheetMLBaseImporter> where T : class, ICellBase {
		#region Fields
		T cell;
		string value;
		bool isAscendingMode = true;
		ICellCollectionGeneric<T> cells;
		string reference = string.Empty;
		int lastColumnIndex = -1;
		static Dictionary<string, CellDataType> cellDataTypeTable = CreateCellDataTypeTable();
		static Dictionary<string, CellDataType> CreateCellDataTypeTable() {
			Dictionary<string, CellDataType> result = new Dictionary<string, CellDataType>();
			result.Add("b", CellDataType.Bool);
			result.Add("e", CellDataType.Error);
			result.Add("inlineStr", CellDataType.InlineString);
			result.Add("n", CellDataType.Number);
			result.Add("s", CellDataType.SharedString);
			result.Add("str", CellDataType.FormulaString);
			return result;
		}
		#endregion
		protected CellDestinationBase(SpreadsheetMLBaseImporter importer, ICellCollectionGeneric<T> cells)
			: base(importer) {
			Guard.ArgumentNotNull(cells, "cells");
			this.cells = cells;
		}
		static CellDestinationBase<T> GetThis(SpreadsheetMLBaseImporter importer) {
			return (CellDestinationBase<T>)importer.PeekDestination();
		}
		#region Properties
		public T Cell { get { return cell; } set { cell = value; } }
		public string Value { get { return value; } set { this.value = value; } }
		public ICellCollectionGeneric<T> Cells { get { return cells; } }
		protected static Dictionary<string, CellDataType> CellDataTypeTable { get { return cellDataTypeTable; } }
		#endregion
		public virtual void Reset(ICellCollectionGeneric<T> cells) {
			this.cell = default(T);
			this.value = null;
			this.reference = string.Empty;
			if(!Object.ReferenceEquals(cells, this.cells)){
				this.isAscendingMode = true;
				this.cells = cells;
				this.lastColumnIndex = -1;
			}
		}
		protected abstract void ProcessElementOpenCore(XmlReader reader);
		public override void ProcessElementOpen(XmlReader reader) {
				ReadCellReference(reader);
				ProcessElementOpenCore(reader);
		}
		protected internal void ReadCellReference(XmlReader reader) {
				reference = Importer.ReadAttribute(reader, "r");
		}
		protected T CreateCell() {
			if (!String.IsNullOrEmpty(reference)) {
				int columnIndex = CellReferenceParser.ParseColumnPartA1Style(reference);
				if (columnIndex == Int32.MinValue)
					Importer.ThrowInvalidFile("Invalid cell reference");
				lastColumnIndex = columnIndex;
				int exisgingCellCount = cells.Count;
				int lastExistingColumnIndex = exisgingCellCount > 0 ? cells.Last.ColumnIndex : -1;
				if (isAscendingMode && lastColumnIndex <= lastExistingColumnIndex)
					isAscendingMode = false;
				if (isAscendingMode) {
					T cell = cells.CreateNewCell(lastColumnIndex);
					cells.InsertInternal(exisgingCellCount, cell);
					return cell;
				}
				else
					return cells[lastColumnIndex];
			}
			else {
				lastColumnIndex++;
				return cells[lastColumnIndex];
			}
		}
		protected void MoveLastColumnIndex() {
			if (!String.IsNullOrEmpty(reference)) {
				CellPosition position = CellReferenceParser.Parse(reference);
				lastColumnIndex = position.Column;
			}
			else
				lastColumnIndex++;
		}
		protected void AssignBooleanValue(WorkbookDataContext context) {
			cell.AssignValueCore(CellValueFormatter.GetValueCore(Value, context, false).Value.ToBoolean(context));
		}
		protected void AssignErrorValue() {
			ICellError error;
			if (CellErrorFactory.TryCreateErrorByInvariantName(Value, out error))
				Cell.AssignValueCore(error.Value);
		}
		protected void AssignNumericValue(WorkbookDataContext context) {
			if (Value != null) {
				double doubleValue;
				if (Double.TryParse(Value, NumberStyles.Float, context.Culture, out doubleValue))
					Cell.AssignValueCore(doubleValue);
				else
					Cell.AssignValueCore(VariantValue.Empty);
			}
		} 
		protected static Destination OnCellValue(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CellDestinationBase<T> thisDestination = GetThis(importer);
			thisDestination.value = string.Empty;
			return CellValueDestination<T>.GetInstance(importer, thisDestination);
		}
	}
	#endregion
	#region CellDestination
	public class CellDestination : CellDestinationBase<ICell> {
		[ThreadStatic]
		static CellDestination instance;
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("f", OnFormula);
			result.Add("is", OnRichTextInline);
			result.Add("v", OnCellValue);
			return result;
		}
		static CellDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (CellDestination)importer.PeekDestination();
		}
		CellDataType cellDataType;
		bool suppressCellValueAssignment;
		int formatIndex;
		public static CellDestination GetInstance(SpreadsheetMLBaseImporter importer, ICellCollection cells) {
			if (instance == null || instance.Importer != importer)
				instance = new CellDestination(importer, cells);
			else {
				instance.Reset(cells);
			}
			return instance;
		}
		public static void ClearInstance() {
			instance = null;
		}
		CellDestination(SpreadsheetMLBaseImporter importer, ICellCollectionGeneric<ICell> cells)
			: base(importer, cells) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void Reset(ICellCollectionGeneric<ICell> cells) {
			base.Reset(cells);
			this.cellDataType = CellDataType.Bool; 
			this.suppressCellValueAssignment = false;
		}
		protected override void ProcessElementOpenCore(XmlReader reader) {
			suppressCellValueAssignment = Importer.DocumentModel.SuppressCellValueAssignment;
			Importer.DocumentModel.SuppressCellValueAssignment = false;
			this.formatIndex = Importer.GetWpSTIntegerValue(reader, "s", Int32.MinValue);
			this.cellDataType = Importer.GetWpEnumValue<CellDataType>(reader, "t", CellDataTypeTable, CellDataType.Number);
		}
		public override void ProcessElementClose(XmlReader reader) {
			ProcessElementCloseCore(reader);
			Importer.DocumentModel.SuppressCellValueAssignment = suppressCellValueAssignment;
		}
		void ProcessElementCloseCore(XmlReader reader) {
			if (Cell == null) {
				if (this.formatIndex <= 0 && string.IsNullOrEmpty(Value)) {
					MoveLastColumnIndex();
					return;
				}
				Cell = CreateCell();
				if (Cell == null)
					return;
			}
			if (this.formatIndex > 0) {
				int index;
				if (Importer.StyleSheet.CellFormatTable.TryGetValue(formatIndex, out index))
					Cell.SetCellFormatIndex(index);
			}
			if (Cell.HasFormula && Value == null) {
				Cell.MarkUpForRecalculation();
				Importer.DocumentModel.RecalculateAfterLoad = true;
				return;
			}
			switch (cellDataType) {
				case CellDataType.Bool:
					AssignBooleanValue(Cell.Sheet.Workbook.DataContext);
					break;
				case CellDataType.Error:
					AssignErrorValue();
					break;
				case CellDataType.InlineString:
					AssignInlineStringValue();
					break;
				case CellDataType.Number:
					AssignNumericValue(Cell.Sheet.Workbook.DataContext);
					break;
				case CellDataType.SharedString:
					AssignSharedStringValue();
					break;
				case CellDataType.FormulaString:
					AssignFormulaStringValue();
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		void AssignSharedStringValue() {
			int index;
			if (!Int32.TryParse(Value, out index))
				Cell.AssignValueCore(String.Empty);
			else {
				VariantValue value = new VariantValue();
				value.SetSharedString(Importer.DocumentModel.SharedStringTable, new SharedStringIndex(index));
				Cell.AssignValueCore(value);
			}
		}
		void AssignInlineStringValue() {
			VariantValue value = Importer.DecodeXmlChars(Value);
			value.SetSharedString(Cell.Worksheet.SharedStringTable, value.InlineTextValue);
			Cell.AssignValueCore(value);
		}
		void AssignFormulaStringValue() {
			VariantValue value = Importer.DecodeXmlChars(Value);
			if (!Cell.HasFormula)
				value.SetSharedString(Cell.Worksheet.SharedStringTable, value.InlineTextValue);
			Cell.AssignValueCore(value);
		}
		static Destination OnFormula(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			CellDestination thisDestination = GetThis(importer);
			thisDestination.Cell = thisDestination.CreateCell();
			return FormulaDestination.GetInstance(importer, thisDestination.Cell);
		}
		static Destination OnRichTextInline(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new InlineStringDestination(importer, GetThis(importer));
		}
		public void SetSharedStringIndex(SharedStringIndex index) {
			cellDataType = CellDataType.SharedString;
			Value = index.ToInt().ToString();
		}
	}
	#endregion
	#region CellValueDestination<T>
	public class CellValueDestination<T> : LeafElementDestination<SpreadsheetMLBaseImporter> where T : class, ICellBase {
		[ThreadStatic]
		static CellValueDestination<T> instance;
		#region Fields
		CellDestinationBase<T> cellDestination;
		#endregion
		internal static CellValueDestination<T> GetInstance(SpreadsheetMLBaseImporter importer, CellDestinationBase<T> cellDestination) {
			if (instance == null || instance.Importer != importer)
				instance = new CellValueDestination<T>(importer, cellDestination);
			else
				instance.cellDestination = cellDestination;
			return instance;
		}
		internal static void ClearInstance() {
			instance = null;
		}
		CellValueDestination(SpreadsheetMLBaseImporter importer, CellDestinationBase<T> cellDestination)
			: base(importer) {
			Guard.ArgumentNotNull(cellDestination, "cellDestination");
			this.cellDestination = cellDestination;
		}
		public override bool ProcessText(XmlReader reader) {
			cellDestination.Value = reader.Value;
			return true;
		}
	}
	#endregion
	#region FormulaDestination
	public class FormulaDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		[ThreadStatic]
		static FormulaDestination instance;
		#region Fields
		ICell cell;
		OpenXmlFormulaType formulaType;
		string reference;
		int index;
		string formula;
		bool calculateAlways;
		#endregion
		#region OpenXmlFormulaTypeTable
		public static Dictionary<OpenXmlFormulaType, string> openXmlFormulaTypeTable = CreateOpenXmlFormulaTypeTable();
		static Dictionary<string, OpenXmlFormulaType> reverseOpenXmlFormulaTypeTable = DictionaryUtils.CreateBackTranslationTable(openXmlFormulaTypeTable);
		static Dictionary<OpenXmlFormulaType, string> CreateOpenXmlFormulaTypeTable() {
			Dictionary<OpenXmlFormulaType, string> result = new Dictionary<OpenXmlFormulaType, string>();
			result.Add(OpenXmlFormulaType.Normal, "normal");
			result.Add(OpenXmlFormulaType.Array, "array");
			result.Add(OpenXmlFormulaType.Shared, "shared");
			result.Add(OpenXmlFormulaType.DataTable, "dataTable");
			return result;
		}
		#endregion
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public static FormulaDestination GetInstance(SpreadsheetMLBaseImporter importer, ICell cell) {
			if (instance == null || instance.Importer != importer)
				instance = new FormulaDestination(importer, cell);
			else
				instance.Reset(cell);
			return instance;
		}
		public static void ClearInstance() {
			instance = null;
		}
		FormulaDestination(SpreadsheetMLBaseImporter importer, ICell cell)
			: base(importer) {
			Guard.ArgumentNotNull(cell, "cell");
			this.cell = cell;
		}
		public void Reset(ICell cell) {
			Guard.ArgumentNotNull(cell, "cell");
			this.cell = cell;
			this.formulaType = OpenXmlFormulaType.Normal;
			this.reference = null;
			this.index = 0;
			this.formula = null;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.formulaType = Importer.GetWpEnumValue<OpenXmlFormulaType>(reader, "t", reverseOpenXmlFormulaTypeTable, OpenXmlFormulaType.Normal);
			this.reference = Importer.ReadAttribute(reader, "ref");
			string siAttribute = Importer.ReadAttribute(reader, "si");
			this.calculateAlways = Importer.GetWpSTOnOffValue(reader, "ca", false);
			if (Importer.GetWpSTOnOffValue(reader, "aca", false))
				System.Diagnostics.Debug.WriteLine("test");
			this.index = siAttribute != null ? int.Parse(siAttribute) : Int32.MaxValue;
		}
		public override bool ProcessText(XmlReader reader) {
			formula = Importer.DecodeXmlChars(reader.Value);
			return true;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (formulaType == OpenXmlFormulaType.Array && !String.IsNullOrEmpty(this.reference)) {
				CellRange arrayRange = CellRange.Create(cell.Sheet, this.reference) as CellRange;
				if (arrayRange != null)
					cell.Worksheet.CreateArrayCore(formula, arrayRange);
			}
			else if (formulaType == OpenXmlFormulaType.Shared) {
				bool isFirstSharedFormulaCell = !String.IsNullOrEmpty(this.reference);
				int sharedFormulaIndex;
				SharedFormula sharedFormula;
				if (isFirstSharedFormulaCell) {
					CellRange range = CellRange.Create(cell.Worksheet, this.reference) as CellRange;
					sharedFormula = new SharedFormula(cell, formula, range);
					sharedFormulaIndex = cell.Worksheet.SharedFormulas.AddWithoutHistory(sharedFormula);
					Importer.SharedFormulaIds.Add(this.index, sharedFormulaIndex);
				}
				else {
					sharedFormulaIndex = Importer.SharedFormulaIds[this.index];
					sharedFormula = cell.Worksheet.SharedFormulas[sharedFormulaIndex];
				}
				SharedFormulaRef sharedFormulaRef = new SharedFormulaRef(cell, sharedFormulaIndex, sharedFormula);
				cell.FormulaInfo = new FormulaInfo();
				cell.FormulaInfo.BinaryFormula = sharedFormulaRef.GetBinary(Importer.DocumentModel.DataContext);
			}
			else if (formulaType == OpenXmlFormulaType.DataTable) {
			}
			else {
				if (!string.IsNullOrEmpty(formula)) {
					Formula cellFormula = new Formula(cell);
					cellFormula.SetBodyTemporarily(formula, cell);
					cell.ApplyFormulaCore(cellFormula);
				}
			}
			if (calculateAlways) {
				FormulaFactory.SetFormulaCalculateAlways(cell, calculateAlways);
				if (Importer.DocumentModel.CalculationChain.Enabled)
					Importer.DocumentModel.RecalculateAfterLoad = true; 
				else {
					if (!string.IsNullOrEmpty(formula)) {
						IncompleteExpressionParserContext parserContext = Importer.DocumentModel.DataContext.ExpressionParser.ParseIncomplete("=" + formula, OperandDataType.Value);
						if (parserContext.Result != null && parserContext.Result.IsVolatile)
						Importer.DocumentModel.RecalculateAfterLoad = true;
					}
				}
			}
			cell.ContentVersion = Importer.DocumentModel.ContentVersion;
			++Importer.NonRegisteredCellsInCellsChainCount;
		}
	}
	#endregion
}
