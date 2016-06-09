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
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandDataValidations
	public class XlsCommandDataValidations : XlsCommandContentBase {
		XlsContentDVal content = new XlsContentDVal();
		#region Properties
		public bool InputWindowClosed { get { return content.InputWindowClosed; } set { content.InputWindowClosed = value; } }
		public int XLeft { get { return content.XLeft; } set { content.XLeft = value; } }
		public int YTop { get { return content.YTop; } set { content.YTop = value; } }
		public int ObjId { get { return content.ObjId; } set { content.ObjId = value; } }
		public int RecordCount { get { return content.RecordCount; } set {content.RecordCount = value; } }
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandDataValidation
	public class XlsCommandDataValidation : XlsCommandBase {
		#region Fields
		static readonly char[] nullChar = new char[] { '\0' };
		const string nullString = "\0";
		XLUnicodeString promptTitle = new XLUnicodeString() { Value = nullString };
		XLUnicodeString errorTitle = new XLUnicodeString() { Value = nullString };
		XLUnicodeString prompt = new XLUnicodeString() { Value = nullString };
		XLUnicodeString error = new XLUnicodeString() { Value = nullString };
		ParsedExpression formula1 = new ParsedExpression();
		ParsedExpression formula2 = new ParsedExpression();
		byte[] formula1Bytes = new byte[0];
		byte[] formula2Bytes = new byte[0];
		readonly List<CellRangeInfo> ranges = new List<CellRangeInfo>();
		#endregion
		#region Properties
		public DataValidationType ValidationType { get; set; }
		public DataValidationErrorStyle ErrorStyle { get; set; }
		public bool StringLookup { get; set; }
		public bool AllowBlank { get; set; }
		public bool SuppressCombo { get; set; }
		public DataValidationImeMode ImeMode { get; set; }
		public bool ShowInputMessage { get; set; }
		public bool ShowErrorMessage { get; set; }
		public DataValidationOperator ValidationOperator { get; set; }
		public string PromptTitle {
			get { return promptTitle.Value.TrimEnd(nullChar); }
			set {
				if (string.IsNullOrEmpty(value))
					promptTitle.Value = nullString;
				else {
					ValueChecker.CheckLength(value, XlsDefs.MaxDataValidationTitleLength, "PromptTitle");
					promptTitle.Value = value;
				}
			}
		}
		public string ErrorTitle {
			get { return errorTitle.Value.TrimEnd(nullChar); }
			set {
				if (string.IsNullOrEmpty(value))
					errorTitle.Value = nullString;
				else {
					ValueChecker.CheckLength(value, XlsDefs.MaxDataValidationTitleLength, "ErrorTitle");
					errorTitle.Value = value;
				}
			}
		}
		public string Prompt {
			get { return prompt.Value.TrimEnd(nullChar); }
			set {
				if (string.IsNullOrEmpty(value))
					prompt.Value = nullString;
				else {
					ValueChecker.CheckLength(value, XlsDefs.MaxDataValidationPromptLength, "Prompt");
					prompt.Value = value;
				}
			}
		}
		public string Error {
			get { return error.Value.TrimEnd(nullChar); }
			set {
				if (string.IsNullOrEmpty(value))
					error.Value = nullString;
				else {
					ValueChecker.CheckLength(value, XlsDefs.MaxDataValidationErrorLength, "Error");
					error.Value = value;
				}
			}
		}
		public ParsedExpression Formula1 { get { return formula1; } }
		public ParsedExpression Formula2 { get { return formula2; } }
		public List<CellRangeInfo> Ranges { get { return ranges; } }
		#endregion
		public void SetFormula1(ParsedExpression expression, IRPNContext context) {
			if (expression == null)
				expression = new ParsedExpression();
			this.formula1 = expression;
			if (expression.Count > 0) {
				byte[] buf = context.ExpressionToBinary(expression);
				int size = BitConverter.ToUInt16(buf, 0);
				this.formula1Bytes = new byte[size];
				Array.Copy(buf, 2, this.formula1Bytes, 0, size);
			}
			else
				this.formula1Bytes = new byte[0];
		}
		public void SetFormula2(ParsedExpression expression, IRPNContext context) {
			if (expression == null)
				expression = new ParsedExpression();
			this.formula2 = expression;
			if (expression.Count > 0) {
				byte[] buf = context.ExpressionToBinary(expression);
				int size = BitConverter.ToUInt16(buf, 0);
				this.formula2Bytes = new byte[size];
				Array.Copy(buf, 2, this.formula2Bytes, 0, size);
			}
			else
				this.formula2Bytes = new byte[0];
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			uint bitwiseField = reader.ReadUInt32();
			ValidationType = (DataValidationType)(bitwiseField & 0x000f);
			ErrorStyle = (DataValidationErrorStyle)((bitwiseField & 0x0070) >> 4);
			StringLookup = (bitwiseField & 0x0080) != 0;
			AllowBlank = (bitwiseField & 0x0100) != 0;
			SuppressCombo = (bitwiseField & 0x0200) != 0;
			ImeMode = (DataValidationImeMode)((bitwiseField & 0x03fc00) >> 10);
			ShowInputMessage = (bitwiseField & 0x040000) != 0;
			ShowErrorMessage = (bitwiseField & 0x080000) != 0;
			ValidationOperator = (DataValidationOperator)((bitwiseField & 0x0f00000) >> 20);
			promptTitle = XLUnicodeString.FromStream(reader);
			errorTitle = XLUnicodeString.FromStream(reader);
			prompt = XLUnicodeString.FromStream(reader);
			error = XLUnicodeString.FromStream(reader);
			int formulaBytesCount = reader.ReadUInt16();
			reader.ReadUInt16(); 
			if (formulaBytesCount > 0) {
				formula1Bytes = reader.ReadBytes(formulaBytesCount);
				try {
					formula1 = contentBuilder.RPNContext.BinaryToExpression(formula1Bytes, formulaBytesCount);
				}
				catch {
					formula1 = new ParsedExpression();
					formula1Bytes = new byte[0];
				}
			}
			formulaBytesCount = reader.ReadUInt16();
			reader.ReadUInt16(); 
			if (formulaBytesCount > 0) {
				formula2Bytes = reader.ReadBytes(formulaBytesCount);
				try {
					formula2 = contentBuilder.RPNContext.BinaryToExpression(formula2Bytes, formulaBytesCount);
				}
				catch {
					formula2 = new ParsedExpression();
					formula2Bytes = new byte[0];
				}
			}
			int count = reader.ReadUInt16();
			for (int i = 0; i < count; i++) {
				try {
					Ref8U ref8U = Ref8U.FromStream(reader);
					Ranges.Add(ref8U.CellRangeInfo);
				}
				catch (ArgumentException) { }
			}
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			Worksheet sheet = contentBuilder.CurrentSheet;
			if (!IsValidContent())
				return;
			DataValidation item = new DataValidation(GetCellRange(sheet), sheet);
			XlsRPNContext context = contentBuilder.RPNContext;
			context.WorkbookContext.PushCurrentWorksheet(sheet);
			context.WorkbookContext.PushRelativeToCurrentCell(true);
			context.WorkbookContext.PushCurrentCell(item.CellRange.TopLeft);
			item.BeginUpdate();
			try {
				item.Type = ValidationType;
				item.ErrorStyle = ErrorStyle;
				item.AllowBlank = AllowBlank;
				item.SuppressDropDown = SuppressCombo;
				item.ShowErrorMessage = ShowErrorMessage;
				item.ShowInputMessage = ShowInputMessage;
				item.ImeMode = ImeMode;
				item.ValidationOperator = ValidationOperator;
				item.PromptTitle = PromptTitle;
				item.Prompt = Prompt;
				item.ErrorTitle = ErrorTitle;
				item.Error = Error;
				ParsedExpression expression = XlsParsedThingConverter.ToModelExpression(Formula1, context);
				ConvertListSourceFormula(expression, sheet.DataContext);
				item.SetFormula1(expression);
				expression = XlsParsedThingConverter.ToModelExpression(Formula2, context);
				item.SetFormula2(expression);
			}
			finally {
				item.EndUpdate();
				context.WorkbookContext.PopCurrentCell();
				context.WorkbookContext.PopRelativeToCurrentCell();
				context.WorkbookContext.PopCurrentWorksheet();
			}
			sheet.DataValidations.Add(item);
		}
		protected override void WriteCore(BinaryWriter writer) {
			uint bitwiseField = (uint)ValidationType;
			bitwiseField |= ((uint)ErrorStyle) << 4;
			if (StringLookup)
				bitwiseField |= 0x0080;
			if (AllowBlank)
				bitwiseField |= 0x0100;
			if (SuppressCombo)
				bitwiseField |= 0x0200;
			bitwiseField |= ((uint)ImeMode) << 10;
			if (ShowInputMessage)
				bitwiseField |= 0x040000;
			if (ShowErrorMessage)
				bitwiseField |= 0x080000;
			bitwiseField |= ((uint)ValidationOperator) << 20;
			writer.Write(bitwiseField);
			promptTitle.Write(writer);
			errorTitle.Write(writer);
			prompt.Write(writer);
			error.Write(writer);
			int formulaBytesCount = formula1Bytes.Length;
			writer.Write(formulaBytesCount);
			writer.Write(formula1Bytes);
			formulaBytesCount = formula2Bytes.Length;
			writer.Write(formulaBytesCount);
			writer.Write(formula2Bytes);
			int count = Ranges.Count;
			writer.Write((ushort)count);
			Ref8U ref8U = new Ref8U();
			for (int i = 0; i < count; i++) {
				ref8U.CellRangeInfo = Ranges[i];
				ref8U.Write(writer);
			}
		}
		protected override short GetSize() {
			return (short)(14 + promptTitle.Length + errorTitle.Length + prompt.Length + error.Length + formula1Bytes.Length + formula2Bytes.Length + Ranges.Count * Ref8U.Size);
		}
		public override IXlsCommand GetInstance() {
			formula1Bytes = new byte[0];
			formula2Bytes = new byte[0];
			formula1 = new ParsedExpression();
			formula2 = new ParsedExpression();
			Ranges.Clear();
			return this;
		}
		CellRangeBase GetCellRange(Worksheet sheet) {
			CellRangeBase result = null;
			int count = Ranges.Count;
			if (count > 0) {
				CellRangeInfo range = Ranges[0];
				result = XlsCellRangeFactory.CreateCellRange(sheet, range.First, range.Last);
				for (int i = 1; i < count; i++) {
					range = Ranges[i];
					CellRangeBase part = XlsCellRangeFactory.CreateCellRange(sheet, range.First, range.Last);
					result = result.ConcatinateWith(part).CellRangeValue;
				}
			}
			return result;
		}
		bool IsValidContent() {
			if (ValidationType != DataValidationType.None && formula1.Count == 0)
				return false;
			if (ValidationOperator == DataValidationOperator.Between || ValidationOperator == DataValidationOperator.NotBetween) {
				if (ValidationType != DataValidationType.None && 
					ValidationType != DataValidationType.List && 
					ValidationType != DataValidationType.Custom && formula2.Count == 0)
					return false;
			}
			return Ranges.Count > 0;
		}
		void ConvertListSourceFormula(ParsedExpression expression, WorkbookDataContext context) {
			if (ValidationType != DataValidationType.List || expression.Count == 0)
				return;
			ParsedThingStringValue ptg = expression[0] as ParsedThingStringValue;
			if (ptg != null && !string.IsNullOrEmpty(ptg.Value)) {
				string value = ptg.Value.Replace('\0', context.GetListSeparator());
				expression.RemoveAt(0);
				expression.Insert(0, new ParsedThingStringValue(value) { DataType = ptg.DataType });
			}
		}
	}
	#endregion
}
