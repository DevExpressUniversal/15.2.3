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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.Services;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandCellBase (abstract class)
	public abstract class XlsCommandCellBase : XlsCommandContentBase {
		#region Properties
		public int RowIndex { get { return Content.RowIndex; } set { Content.RowIndex = value; } }
		public int ColumnIndex { get { return Content.ColumnIndex; } set { Content.ColumnIndex = value; } }
		public int FormatIndex { get { return Content.FormatIndex; } set { Content.FormatIndex = value; } }
		XlsContentCellBase Content { get { return GetContent() as XlsContentCellBase; } }
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			ICell cell = contentBuilder.CurrentSheet.Rows[RowIndex].Cells[ColumnIndex];
			ApplyCellValue(cell);
			if (FormatIndex > contentBuilder.DocumentModel.StyleSheet.DefaultCellFormatIndex)
				cell.SetCellFormatIndex(contentBuilder.StyleSheet.GetCellFormatIndex(FormatIndex));
		}
		protected void ApplyCellValue(ICell cell) {
			DocumentModel workbook = cell.Worksheet.Workbook;
			bool suppressCellValueAssignment = workbook.SuppressCellValueAssignment;
			workbook.SuppressCellValueAssignment = false;
			ApplyCellValueCore(cell);
			workbook.SuppressCellValueAssignment = suppressCellValueAssignment;
		}
		protected abstract void ApplyCellValueCore(ICell cell);
	}
	#endregion
	#region XlsCommandBlank
	public class XlsCommandBlank : XlsCommandCellBase {
		XlsContentBlank content = new XlsContentBlank();
		protected override void ApplyCellValueCore(ICell cell) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandMulBlank
	public class XlsCommandMulBlank : XlsCommandContentBase {
		XlsContentMulBlank content = new XlsContentMulBlank();
		#region Properties
		public int RowIndex { get { return content.RowIndex; } set { content.RowIndex = value; } }
		public int FirstColumnIndex { get { return content.FirstColumnIndex; } set { content.FirstColumnIndex = value; } }
		public int LastColumnIndex { get { return content.LastColumnIndex; } }
		public IList<int> FormatIndices { get { return content.FormatIndices; } }
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			Row row = contentBuilder.CurrentSheet.Rows[RowIndex];
			int count = FormatIndices.Count;
			for (int i = 0; i < count; i++) {
				ICell cell = row.Cells[FirstColumnIndex + i];
				cell.SetCellFormatIndex(contentBuilder.StyleSheet.GetCellFormatIndex(FormatIndices[i]));
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandBoolErr
	public class XlsCommandBoolErr : XlsCommandCellBase {
		XlsContentBoolErr content = new XlsContentBoolErr();
		#region Properties
		public bool BoolValue {
			get { return content.Value != 0; }
			set {
				content.IsError = false;
				content.Value = (byte)(value ? 1 : 0);
			}
		}
		public VariantValue ErrorValue {
			get {
				if (this.IsError)
					return ErrorConverter.ErrorCodeToValue(content.Value);
				return VariantValue.Empty;
			}
			set {
				if (value.Type == VariantValueType.Error) {
					content.IsError = true;
					content.Value = (byte)ErrorConverter.ValueToErrorCode(value);
				}
			}
		}
		public bool IsError { get { return content.IsError; } }
		#endregion
		protected override void ApplyCellValueCore(ICell cell) {
			if (IsError)
				cell.AssignValueCore(ErrorValue);
			else
				cell.AssignValueCore(BoolValue);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			List<XlsChartCachedValue> cache = contentBuilder.GetCurrentDataCache(ColumnIndex);
			if (cache != null) {
				XlsChartCachedValue cachedValue = IsError ? new XlsChartCachedValue(RowIndex, ErrorValue) : new XlsChartCachedValue(RowIndex, BoolValue);
				cache.Add(cachedValue);
			}
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandLabel
	public class XlsCommandLabel : XlsCommandCellBase {
		XlsContentLabel content = new XlsContentLabel();
		public string Value { get { return content.Value; } set { content.Value = value; } }
		protected override void ApplyCellValueCore(ICell cell) {
			VariantValue value = new VariantValue();
			SharedStringIndex index = cell.Worksheet.Workbook.SharedStringTable.RegisterString(Value);
			value.SetSharedString(cell.Worksheet.Workbook.SharedStringTable, index);
			cell.AssignValueCore(value);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			List<XlsChartCachedValue> cache = contentBuilder.GetCurrentDataCache(ColumnIndex);
			if (cache != null) {
				XlsChartCachedValue cachedValue = new XlsChartCachedValue(RowIndex, Value);
				cache.Add(cachedValue);
			}
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandLabelSst
	public class XlsCommandLabelSst : XlsCommandCellBase {
		XlsContentLabelSst content = new XlsContentLabelSst();
		public int StringIndex { get { return content.StringIndex; } set { content.StringIndex = value; } }
		protected override void ApplyCellValueCore(ICell cell) {
			VariantValue value = new VariantValue();
			value.SetSharedString(cell.Worksheet.Workbook.SharedStringTable, new SharedStringIndex(StringIndex));
			cell.AssignValueCore(value);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandNumber
	public class XlsCommandNumber : XlsCommandCellBase {
		XlsContentNumber content = new XlsContentNumber();
		public double Value { get { return content.Value; } set { content.Value = value; } }
		protected override void ApplyCellValueCore(ICell cell) {
			cell.AssignValueCore(Value);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			List<XlsChartCachedValue> cache = contentBuilder.GetCurrentDataCache(ColumnIndex);
			if (cache != null) {
				XlsChartCachedValue cachedValue = new XlsChartCachedValue(RowIndex, Value);
				cache.Add(cachedValue);
			}
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region IXlsFormulaBuilder
	public interface IXlsRangeFormulaData {
		CellRangeInfo Range { get; }
		ParsedExpression ParsedExpression { get; }
	}
	public interface IXlsArrayFormulaData : IXlsRangeFormulaData {
	}
	public interface IXlsSharedFormulaData : IXlsRangeFormulaData {
	}
	public interface IXlsFormulaBuilder {
		void Register(XlsContentBuilder contentBuilder);
		void Unregister();
		void Build();
		void Build(string data);
		void Build(IXlsArrayFormulaData data);
		void Build(IXlsSharedFormulaData data);
	}
	#endregion
	#region XlsCommandFormula
	public class XlsCommandFormula : XlsCommandCellBase, IXlsFormulaBuilder {
		XlsContentFormula content = new XlsContentFormula();
		#region Fields
		string stringValue = string.Empty;
		ParsedExpression parsedExpression = new ParsedExpression();
		#endregion
		#region Properties
		public XlsFormulaValue Value { get { return content.Value; } }
		public bool AlwaysCalc { get { return content.AlwaysCalc; } set { content.AlwaysCalc = value; } }
		public bool HasFillAlignment { get { return content.HasFillAlignment; } set { content.HasFillAlignment = value; } }
		public bool PartOfSharedFormula {
			get {
				if (ParsedExpression.Count > 0) {
					return (ParsedExpression[0] is ParsedThingExp) && content.PartOfSharedFormula;
				}
				return false;
			}
			set { content.PartOfSharedFormula = value; }
		}
		public bool ClearErrors { get { return content.ClearErrors; } set { content.ClearErrors = value; } }
		public bool PartOfArrayFormula {
			get {
				if (ParsedExpression.Count > 0) {
					return (ParsedExpression[0] is ParsedThingExp) && !content.PartOfSharedFormula;
				}
				return false;
			}
		}
		public bool PartOfTable {
			get {
				if (ParsedExpression.Count > 0) {
					return ParsedExpression[0] is ParsedThingDataTable;
				}
				return false;
			}
		}
		public ParsedExpression ParsedExpression { get { return parsedExpression; } }
		#endregion
		public void SetParsedExpression(ParsedExpression parsedExpression, IRPNContext context) {
			this.parsedExpression = parsedExpression;
			content.FormulaBytes = context.ExpressionToBinary(parsedExpression);
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			parsedExpression = contentBuilder.RPNContext.BinaryToExpression(content.FormulaBytes);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			Register(contentBuilder);
			Build();
		}
		#region IXlsFormulaBuilder Members
		XlsContentBuilder contentBuilder;
		public void Register(XlsContentBuilder contentBuilder) {
			if (contentBuilder.FormulaBuilder != null)
				contentBuilder.FormulaBuilder.Unregister();
			contentBuilder.FormulaBuilder = this;
			this.contentBuilder = contentBuilder;
		}
		public void Unregister() {
			this.contentBuilder.FormulaBuilder = null;
			this.contentBuilder = null;
		}
		public void Build() {
			if (GetCompleted()) {
				DocumentModel documentModel = contentBuilder.DocumentModel;
				WorkbookDataContext dataContext = documentModel.DataContext;
				ICell cell = contentBuilder.CurrentSheet.Rows[RowIndex].Cells[ColumnIndex];
				if (PartOfArrayFormula) {
					ArrayFormula arrayFormula = FindArrayFormula();
					CellRange range = arrayFormula.Range;
					ICell topLeftCell = cell.Worksheet[range.TopLeft];
					if (!Object.ReferenceEquals(cell, topLeftCell))
						cell.ApplyFormulaCore(new ArrayFormulaPart(cell, topLeftCell));
				}
				else if (PartOfTable) {
				}
				else if (PartOfSharedFormula) {
					bool suppressCellValueAssignment = documentModel.SuppressCellValueAssignment;
					documentModel.SuppressCellValueAssignment = false;
					dataContext.PushCurrentCell(cell);
					try {
						int sharedFormulaIndex = FindSharedFormula();
						SharedFormula sharedFormula = contentBuilder.CurrentSheet.SharedFormulas[sharedFormulaIndex];
						if (sharedFormula.Range.ContainsCell(cell) && sharedFormula.IsFormulaCompliant) {
							SharedFormulaRef sharedFormulaRef = new SharedFormulaRef(cell, sharedFormulaIndex, sharedFormula);
							cell.FormulaInfo = new FormulaInfo();
							cell.FormulaInfo.BinaryFormula = sharedFormulaRef.GetBinary(dataContext);
						}
						else {
							SharedFormulaRef sharedFormulaRef = new SharedFormulaRef(cell, sharedFormulaIndex, sharedFormula);
							ParsedExpression expression = sharedFormulaRef.GetNormalCellFormula(cell);
							CheckFormulaCompliance(contentBuilder, expression);
							Formula formula = new Formula(cell, expression);
							cell.ApplyFormulaCore(formula);
						}
					}
					finally {
						dataContext.PopCurrentCell();
						documentModel.SuppressCellValueAssignment = suppressCellValueAssignment;
					}
				}
				else { 
					dataContext.PushCurrentCell(cell);
					contentBuilder.RPNContext.PushCurrentSubject(cell.GetDescription());
					try {
						CheckFormulaCompliance(contentBuilder, parsedExpression);
						ParsedExpression expression = XlsParsedThingConverter.ToModelExpression(ParsedExpression, contentBuilder.RPNContext);
						if (expression.IsValidExpression(contentBuilder)) {
							Formula formula = new Formula(cell, expression);
							cell.ApplyFormulaCore(formula);
						}
						else {
							string position = cell.Position.ToString();
							string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidFormulaRemoved), position, cell.Sheet.Name);
							contentBuilder.LogMessage(LogCategory.Warning, message);
						}
					}
					finally {
						dataContext.PopCurrentCell();
						contentBuilder.RPNContext.PopCurrentSubject();
					}
				}
				ApplyCellValue(cell);
				cell.SetCellFormatIndex(contentBuilder.StyleSheet.GetCellFormatIndex(FormatIndex));
				if (AlwaysCalc) {
					FormulaFactory.SetFormulaCalculateAlways(cell, AlwaysCalc);
					if (documentModel.CalculationChain.Enabled)
						documentModel.RecalculateAfterLoad = true;
					else {
						if (cell.HasFormula && cell.Formula.IsVolatile())
							documentModel.RecalculateAfterLoad = true;
					}
				}
				if (!PartOfArrayFormula && !PartOfSharedFormula && !PartOfTable)
					Unregister();
			}
		}
		public void Build(string data) {
			this.stringValue = data;
			Build();
		}
		public void Build(IXlsArrayFormulaData data) {
			ICell cell = contentBuilder.CurrentSheet.Rows[RowIndex].Cells[ColumnIndex];
			CellRange range = new CellRange(cell.Sheet, data.Range.First, data.Range.Last);
			contentBuilder.RPNContext.WorkbookContext.PushArrayFormulaProcessing(true);
			contentBuilder.RPNContext.WorkbookContext.PushCurrentCell(cell);
			contentBuilder.RPNContext.PushCurrentSubject(cell.GetDescription(range));
			try {
				if (!data.ParsedExpression.IsXlsArrayFormulaCompliant()) {
					string position = (new CellPosition(cell.ColumnIndex, cell.RowIndex)).ToString();
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_IncompliantArrayParsedFormula), position, cell.Sheet.Name);
					contentBuilder.LogMessage(LogCategory.Warning, message);
					data.ParsedExpression.Clear();
					data.ParsedExpression.Add(new ParsedThingError(VariantValue.ErrorValueNotAvailable.ErrorValue));
				}
				ParsedExpression expression = XlsParsedThingConverter.ToModelExpression(data.ParsedExpression, contentBuilder.RPNContext);
				if (expression.IsValidExpression(contentBuilder)) {
					ArrayFormula arrayFormula = new ArrayFormula(range, expression);
					cell.Worksheet.ArrayFormulaRanges.Add(range);
					cell.ApplyFormulaCore(arrayFormula);
				}
				else {
					string position = (new CellPosition(cell.ColumnIndex, cell.RowIndex)).ToString();
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidFormulaRemoved), position, cell.Sheet.Name);
					contentBuilder.LogMessage(LogCategory.Warning, message);
				}
			}
			finally {
				contentBuilder.RPNContext.WorkbookContext.PopArrayFormulaProcessing();
				contentBuilder.RPNContext.WorkbookContext.PopCurrentCell();
				contentBuilder.RPNContext.PopCurrentSubject();
			}
			Build();
		}
		public void Build(IXlsSharedFormulaData data) {
			ICell cell = contentBuilder.CurrentSheet.Rows[RowIndex].Cells[ColumnIndex];
			CellRange range = new CellRange(cell.Sheet, data.Range.First, data.Range.Last);
			contentBuilder.RPNContext.WorkbookContext.PushSharedFormulaProcessing(true);
			contentBuilder.RPNContext.WorkbookContext.PushCurrentCell(cell);
			contentBuilder.RPNContext.PushCurrentSubject(cell.GetDescription(range));
			try {
				ParsedExpression expression = XlsParsedThingConverter.ToModelExpression(data.ParsedExpression, contentBuilder.RPNContext);
				if (expression.IsValidExpression(contentBuilder)) {
					SharedFormula sharedFormula = new SharedFormula(expression, range);
					int index = cell.Worksheet.SharedFormulas.AddWithoutHistory(sharedFormula);
					contentBuilder.SharedFormulas.Add(cell.Position, index);
				}
				else {
					string position = cell.Position.ToString();
					string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_InvalidFormulaRemoved), position, cell.Sheet.Name);
					contentBuilder.LogMessage(LogCategory.Warning, message);
				}
			}
			finally {
				contentBuilder.RPNContext.WorkbookContext.PopSharedFormulaProcessing();
				contentBuilder.RPNContext.WorkbookContext.PopCurrentCell();
				contentBuilder.RPNContext.PopCurrentSubject();
			}
			Build();
		}
		#endregion
		bool GetCompleted() {
			if (PartOfArrayFormula) {
				if (FindArrayFormula() == null)
					return false;
			}
			if (PartOfTable) {
				if (FindTable() == null)
					return false;
			}
			if (PartOfSharedFormula) {
				if (FindSharedFormula() < 0)
					return false;
			}
			if (content.Value.IsString)
				return !string.IsNullOrEmpty(this.stringValue);
			return true;
		}
		ArrayFormula FindArrayFormula() {
			ArrayFormulaRangesCollection formulas = contentBuilder.CurrentSheet.ArrayFormulaRanges;
			if (formulas.Count > 0) {
				ParsedThingExp exp = ParsedExpression[0] as ParsedThingExp;
				ICell hostCell = contentBuilder.CurrentSheet.TryGetCell(exp.Position.Column, exp.Position.Row) as ICell;
				ArrayFormula arrayFormula = null;
				if (hostCell != null)
					arrayFormula = hostCell.GetFormula() as ArrayFormula;
				if (exp.Position.Row == RowIndex && exp.Position.Column == ColumnIndex) {
					CellRange formulaRange = formulas[formulas.Count - 1];
					if (exp.Position.EqualsPosition(formulaRange.TopLeft))
						return arrayFormula;
				}
				else
					return arrayFormula;
			}
			return null;
		}
		Table FindTable() {
			return null;
		}
		int FindSharedFormula() {
			int result = -1;
			SharedFormulaCollection formulas = contentBuilder.CurrentSheet.SharedFormulas;
			if (formulas.Count > 0) {
				ParsedThingExp exp = ParsedExpression[0] as ParsedThingExp;
				contentBuilder.SharedFormulas.TryGetValue(exp.Position, out result);
			}
			return result;
		}
		protected override void ApplyCellValueCore(ICell cell) {
			if (Value.IsBoolean)
				cell.AssignValueCore(Value.BooleanValue);
			else if (Value.IsError)
				cell.AssignValueCore(Value.GetErrorValue());
			else if (Value.IsNumeric)
				cell.AssignValueCore(Value.NumericValue);
			else if (Value.IsString)
				cell.AssignValueCore(this.stringValue);
			else if (Value.IsBlankString)
				cell.AssignValueCore(string.Empty);
		}
		void CheckFormulaCompliance(XlsContentBuilder contentBuilder, ParsedExpression parsedExpression) {
			if (!parsedExpression.IsXlsCellFormulaCompliant()) {
				WorkbookDataContext context = contentBuilder.RPNContext.WorkbookContext;
				string position = (new CellPosition(context.CurrentColumnIndex, context.CurrentRowIndex)).ToString();
				string message = string.Format(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_IncompliantCellParsedFormula), position, context.CurrentWorksheet.Name);
				contentBuilder.LogMessage(LogCategory.Warning, message);
				parsedExpression.Clear();
				parsedExpression.Add(new ParsedThingError(VariantValue.ErrorValueNotAvailable.ErrorValue));
			}
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandFormula();
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	public static class XlsFormulaValueExtensions {
		public static VariantValue GetErrorValue(this XlsFormulaValue formulaValue) {
			if (formulaValue.IsError)
				return ErrorConverter.ErrorCodeToValue(formulaValue.ErrorCode);
			return VariantValue.Empty;
		}
		public static void SetErrorValue(this XlsFormulaValue formulaValue, VariantValue value) {
			if (value.Type == VariantValueType.Error)
				formulaValue.ErrorCode = (byte)ErrorConverter.ValueToErrorCode(value);
		}
	}
	#endregion
	#region XlsCommandRangeBase
	public abstract class XlsCommandRangeBase : XlsCommandBase {
		#region Fields
		CellRangeInfo range = new CellRangeInfo();
		#endregion
		#region Properties
		public CellRangeInfo Range {
			get { return range; }
			set {
				if (value == null)
					value = new CellRangeInfo();
				range = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int firstRow = reader.ReadUInt16();
			int lastRow = reader.ReadUInt16();
			int firstColumn = reader.ReadByte();
			int lastColumn = reader.ReadByte();
			this.range = new CellRangeInfo(new CellPosition(firstColumn, firstRow), new CellPosition(lastColumn, lastRow));
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)range.First.Row);
			writer.Write((ushort)range.Last.Row);
			writer.Write((byte)range.First.Column);
			writer.Write((byte)range.Last.Column);
		}
	}
	#endregion
	#region XlsCommandArrayFormula
	public class XlsCommandArrayFormula : XlsCommandRangeBase, IXlsArrayFormulaData {
		#region Fields
		const int fixedPartSize = 12;
		ParsedExpression parsedExpression = new ParsedExpression();
		byte[] formulaBytes;
		#endregion
		#region Properties
		public bool AlwaysCalc { get; set; }
		public ParsedExpression ParsedExpression { get { return parsedExpression; } }
		#endregion
		public void SetParsedExpression(ParsedExpression parsedExpression, IRPNContext context) {
			this.parsedExpression = parsedExpression;
			formulaBytes = context.ExpressionToBinary(parsedExpression);
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			ushort bitwiseField = reader.ReadUInt16();
			AlwaysCalc = (bitwiseField & 0x0001) != 0;
			reader.ReadUInt32(); 
			int formulaBytesCount = Size - fixedPartSize;
			formulaBytes = reader.ReadBytes(formulaBytesCount);
			parsedExpression = contentBuilder.RPNContext.BinaryToExpression(formulaBytes);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.FormulaBuilder != null)
				contentBuilder.FormulaBuilder.Build(this as IXlsArrayFormulaData);
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			ushort bitwiseField = 0;
			if (AlwaysCalc)
				bitwiseField |= 0x0001;
			writer.Write(bitwiseField);
			writer.Write((uint)0); 
			writer.Write(formulaBytes);
		}
		protected override short GetSize() {
			return (short)(fixedPartSize + formulaBytes.Length);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandArrayFormula();
		}
	}
	#endregion
	#region XlsCommandTable
	public class XlsCommandTable : XlsCommandRangeBase {
		#region Properties
		public bool AlwaysCalc { get; set; }
		public bool IsRowInputCell { get; set; }
		public bool IsTwoVarDataTable { get; set; }
		public bool InputRowDeleted { get; set; }
		public bool InputColumnDeleted { get; set; }
		public int RowInputRow { get; set; }
		public int ColumnInputRow { get; set; }
		public int RowInputColumn { get; set; }
		public int ColumnInputColumn { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			ushort bitwiseField = reader.ReadUInt16();
			AlwaysCalc = (bitwiseField & 0x0001) != 0;
			IsRowInputCell = (bitwiseField & 0x0004) != 0;
			IsTwoVarDataTable = (bitwiseField & 0x0008) != 0;
			InputRowDeleted = (bitwiseField & 0x0010) != 0;
			InputColumnDeleted = (bitwiseField & 0x0020) != 0;
			RowInputRow = reader.ReadUInt16();
			ColumnInputRow = reader.ReadInt16();
			RowInputColumn = reader.ReadUInt16();
			ColumnInputColumn = reader.ReadInt16();
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if (contentBuilder.FormulaBuilder != null)
				contentBuilder.FormulaBuilder.Build();
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			ushort bitwiseField = 0;
			if (AlwaysCalc)
				bitwiseField |= 0x0001;
			if (IsRowInputCell)
				bitwiseField |= 0x0004;
			if (IsTwoVarDataTable)
				bitwiseField |= 0x0008;
			if (InputRowDeleted)
				bitwiseField |= 0x0010;
			if (InputColumnDeleted)
				bitwiseField |= 0x0020;
			writer.Write(bitwiseField);
			writer.Write((ushort)RowInputRow);
			writer.Write((short)ColumnInputRow);
			if (IsTwoVarDataTable && InputColumnDeleted)
				writer.Write((ushort)0xffff);
			else if (!IsTwoVarDataTable)
				writer.Write((ushort)0);
			else
				writer.Write((ushort)RowInputColumn);
			if (IsTwoVarDataTable && InputColumnDeleted)
				writer.Write((short)-1);
			else if (!IsTwoVarDataTable)
				writer.Write((short)0);
			else
				writer.Write((short)ColumnInputColumn);
		}
		protected override short GetSize() {
			return 16;
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandTable();
		}
	}
	#endregion
	#region XlsCommandSharedFormula
	public class XlsCommandSharedFormula : XlsCommandRangeBase, IXlsSharedFormulaData {
		#region Fields
		const int fixedPartSize = 8;
		int useCount;
		ParsedExpression parsedExpression = new ParsedExpression();
		byte[] formulaBytes;
		#endregion
		#region Properties
		public int UseCount {
			get { return useCount; }
			set {
				ValueChecker.CheckValue(value, 0, byte.MaxValue, "UseCount");
				useCount = value;
			}
		}
		public ParsedExpression ParsedExpression { get { return parsedExpression; } }
		#endregion
		public void SetParsedExpression(ParsedExpression parsedExpression, IRPNContext context) {
			this.parsedExpression = parsedExpression;
			formulaBytes = context.ExpressionToBinary(parsedExpression);
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			reader.ReadByte(); 
			UseCount = reader.ReadByte();
			int formulaBytesCount = Size - fixedPartSize;
			formulaBytes = reader.ReadBytes(formulaBytesCount);
			parsedExpression = contentBuilder.RPNContext.BinaryToExpression(formulaBytes);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.FormulaBuilder != null)
				contentBuilder.FormulaBuilder.Build(this as IXlsSharedFormulaData);
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			writer.Write((byte)0); 
			writer.Write((byte)UseCount);
			writer.Write(formulaBytes);
		}
		protected override short GetSize() {
			return (short)(fixedPartSize + formulaBytes.Length);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandSharedFormula();
		}
	}
	#endregion
	#region XlsCommandString
	public class XlsCommandString : XlsCommandDataCollectorBase {
		XLUnicodeString value = new XLUnicodeString();
		protected override void ReadData(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.value.ReadData(reader);
			if (this.value.Complete && contentBuilder.FormulaBuilder != null) {
				contentBuilder.FormulaBuilder.Build(this.value.Value);
			}
		}
		protected override bool GetCompleted() {
			return this.value.Complete;
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandString();
		}
	}
	#endregion
}
