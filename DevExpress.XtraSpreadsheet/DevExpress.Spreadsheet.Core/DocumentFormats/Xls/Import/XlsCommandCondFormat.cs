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
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraExport.Xls;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandConditionalFormat
	public class XlsCommandConditionalFormat : XlsCommandContentBase {
		XlsContentCondFmt content;
		public XlsCommandConditionalFormat() 
			: this(new XlsContentCondFmt()) {
		}
		protected XlsCommandConditionalFormat(XlsContentCondFmt content) {
			this.content = content;
		}
		#region Properties
		public int RecordCount { get { return content.RecordCount; } set { content.RecordCount = value; } }
		public bool ToughRecalc { get { return content.ToughRecalc; } set { content.ToughRecalc = value; } }
		public int Id { get { return content.Id; } set { content.Id = value; } }
		public XlsRef8 BoundRange { get { return content.BoundRange; } set { content.BoundRange = value; } }
		public IList<XlsRef8> Ranges { get { return content.Ranges; } }
		#endregion
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			CellRangeBase cellRange = GetCellRange(contentBuilder.CurrentSheet);
			XlsCondFormat format = new XlsCondFormat(Id, ToughRecalc, cellRange);
			contentBuilder.AddConditionalFormat(format);
		}
		protected CellRangeBase GetCellRange(Worksheet sheet) {
			CellRangeBase result = null;
			int count = Ranges.Count;
			if(count > 0) {
				XlsRef8 range = Ranges[0];
				result = XlsCellRangeFactory.CreateCellRange(sheet, 
					new CellPosition(range.FirstColumnIndex, range.FirstRowIndex), 
					new CellPosition(range.LastColumnIndex, range.LastRowIndex));
				for(int i = 1; i < count; i++) {
					range = Ranges[i];
					CellRangeBase part = XlsCellRangeFactory.CreateCellRange(sheet, 
						new CellPosition(range.FirstColumnIndex, range.FirstRowIndex), 
						new CellPosition(range.LastColumnIndex, range.LastRowIndex));
					result = result.ConcatinateWith(part).CellRangeValue;
				}
			}
			return result;
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandConditionalFormat();
		}
		protected override IXlsContent GetContent() {
 			return content;
		}
	}
	#endregion
	#region XlsCommandConditionalFormat12
	public class XlsCommandConditionalFormat12 : XlsCommandConditionalFormat {
		public XlsCommandConditionalFormat12()
			: base(new XlsContentCondFmt12()) {
		}
		protected override void WriteCore(BinaryWriter writer) {
			XlsContentCondFmt12 content = GetContent() as XlsContentCondFmt12;
			content.RecordHeader.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandConditionalFormat12));
			base.WriteCore(writer);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandConditionalFormat12();
		}
	}
	#endregion
	#region XlsCommandCF
	public class XlsCommandCF : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 6;
		DxfN12Info format = new DxfN12Info();
		byte[] firstFormulaBytes = new byte[0];
		byte[] secondFormulaBytes = new byte[0];
		int firstFormulaSize;
		int secondFormulaSize;
		#endregion
		#region Properties
		public ConditionalFormattingRuleType RuleType { get; set; }
		public ConditionalFormattingOperator ComparisonFunction { get; set; }
		public DxfN12Info Format { 
			get { return format; }
			set {
				if(value != null)
					format = value;
				else
					format = new DxfN12Info();
			}
		}
		#endregion
		public XlsCommandCF() {
			RuleType = ConditionalFormattingRuleType.CompareWithFormulaResult;
			ComparisonFunction = ConditionalFormattingOperator.Between;
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			RuleType = XlsCFHelper.ConditionRuleCodeToType(reader.ReadByte());
			ComparisonFunction = XlsCFHelper.ComparisonFunctionCodeToType(reader.ReadByte());
			firstFormulaSize = reader.ReadUInt16();
			secondFormulaSize = reader.ReadUInt16();
			this.format = ReadFormat(reader);
			if(firstFormulaSize > 0)
				firstFormulaBytes = reader.ReadBytes(firstFormulaSize);
			if(secondFormulaSize > 0)
				secondFormulaBytes = reader.ReadBytes(secondFormulaSize);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsCondFormat activeFormat = contentBuilder.ActiveConditionalFormat;
			if(activeFormat != null) {
				XlsCondFormatRule item = new XlsCondFormatRule();
				item.RuleType = RuleType;
				item.ComparisonFunction = ComparisonFunction;
				item.ApplyFormat(Format, contentBuilder.DocumentModel);
				XlsRPNContext context = contentBuilder.RPNContext;
				context.WorkbookContext.PushCurrentWorksheet(contentBuilder.CurrentSheet);
				try {
					item.FirstFormula = XlsParsedThingConverter.ToCFExpression(GetFirstFormula(context), context);
					item.SecondFormula = XlsParsedThingConverter.ToCFExpression(GetSecondFormula(context), context);
				}
				finally {
					context.WorkbookContext.PopCurrentWorksheet();
				}
				activeFormat.Rules.Add(item);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(XlsCFHelper.ConditionRuleTypeToCode(RuleType));
			writer.Write(XlsCFHelper.ComparisonFunctionTypeToCode(ComparisonFunction));
			writer.Write((ushort)firstFormulaSize);
			ushort actualSecondFormulaSize = (ushort)GetActualSecondFormulaSize();
			writer.Write(actualSecondFormulaSize);
			WriteFormat(writer);
			if(firstFormulaSize > 0)
				writer.Write(firstFormulaBytes);
			if(actualSecondFormulaSize > 0)
				writer.Write(secondFormulaBytes);
		}
		protected override short GetSize() {
			return (short)(GetSizeCore() + Format.DifferentialFormatInfo.GetSize());
		}
		#region Formulas
		public ParsedExpression GetFirstFormula(XlsRPNContext context) {
			return XlsCFHelper.GetFormulaExpression(context, firstFormulaBytes, firstFormulaSize);
		}
		public ParsedExpression GetSecondFormula(XlsRPNContext context) {
			return XlsCFHelper.GetFormulaExpression(context, secondFormulaBytes, secondFormulaSize);
		}
		public void SetFirstFormula(ParsedExpression parsedExpression, XlsRPNContext context) {
			this.firstFormulaBytes = XlsCFHelper.GetFormulaBytes(parsedExpression, context, ref firstFormulaSize);
		}
		public void SetSecondFormula(ParsedExpression parsedExpression, XlsRPNContext context) {
			this.secondFormulaBytes = XlsCFHelper.GetFormulaBytes(parsedExpression, context, ref secondFormulaSize);
		}
		#endregion
		#region Internals
		protected virtual DxfN12Info ReadFormat(XlsReader reader) {
			DxfN12Info result = new DxfN12Info();
			using(XlsCommandStream dxfnStream = new XlsCommandStream(reader, Size - fixedPartSize)) {
				using(BinaryReader dxfnReader = new BinaryReader(dxfnStream))
					result.DifferentialFormatInfo.Read(dxfnReader);
			}
			return result;
		}
		protected virtual void WriteFormat(BinaryWriter writer) {
			Format.DifferentialFormatInfo.Write(writer);
		}
		protected short GetSizeCore() {
			return (short)(fixedPartSize + firstFormulaSize + GetActualSecondFormulaSize());
		}
		int GetActualSecondFormulaSize() {
			if(RuleType != ConditionalFormattingRuleType.CompareWithFormulaResult) 
				return 0;
			if(ComparisonFunction != ConditionalFormattingOperator.Between && ComparisonFunction != ConditionalFormattingOperator.NotBetween) 
				return 0;
			return secondFormulaSize;
		}
		#endregion
		public override IXlsCommand GetInstance() {
			return new XlsCommandCF();
		}
	}
	#endregion
	#region XlsCommandCF12
	public class XlsCommandCF12 : XlsCommandCF, IXlsCFRuleTemplateContainer {
		#region Fields
		byte[] activeFormulaBytes = new byte[0];
		int activeFormulaSize;
		int priority;
		int stdDev;
		XlsCFColorScaleParams colorScaleParams = new XlsCFColorScaleParams();
		XlsCFDatabarParams dataBarParams = new XlsCFDatabarParams();
		XlsCFFilterParams filterParams = new XlsCFFilterParams();
		XlsCFIconSetParams iconSetParams = new XlsCFIconSetParams();
		#endregion
		#region Properties
		public bool StopIfTrue { get; set; }
		public int Priority {
			get { return priority; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "Priority");
				priority = value;
			}
		}
		public XlsCFRuleTemplate RuleTemplate { get; set; }
		public bool FilterTop { get; set; }
		public bool FilterPercent { get; set; }
		public int FilterValue { get; set; }
		public ConditionalFormattingTextCondition TextRule { get; set; }
		public int StdDev {
			get { return stdDev; }
			set {
				ValueChecker.CheckValue(value, 0, 3, "StdDev");
				stdDev = value;
			}
		}
		public XlsCFColorScaleParams ColorScaleParams {
			get { return colorScaleParams; }
			set {
				if(value != null)
					colorScaleParams = value;
				else
					colorScaleParams = new XlsCFColorScaleParams();
			}
		}
		public XlsCFDatabarParams DataBarParams {
			get { return dataBarParams; }
			set {
				if(value != null)
					dataBarParams = value;
				else
					dataBarParams = new XlsCFDatabarParams();
			}
		}
		public XlsCFFilterParams FilterParams {
			get { return filterParams; }
			set {
				if(value != null)
					filterParams = value;
				else
					filterParams = new XlsCFFilterParams();
			}
		}
		public XlsCFIconSetParams IconSetParams {
			get { return iconSetParams; }
			set {
				if(value != null)
					iconSetParams = value;
				else
					iconSetParams = new XlsCFIconSetParams();
			}
		}
		#endregion
		public XlsCommandCF12() 
			: base() {
			RuleTemplate = XlsCFRuleTemplate.CellValue;
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			base.ReadCore(reader, contentBuilder);
			activeFormulaSize = reader.ReadUInt16();
			if(activeFormulaSize > 0)
				activeFormulaBytes = reader.ReadBytes(activeFormulaSize);
			byte bitwiseField = reader.ReadByte();
			StopIfTrue = Convert.ToBoolean(bitwiseField & 0x02);
			this.priority = reader.ReadUInt16();
			RuleTemplate = (XlsCFRuleTemplate)reader.ReadUInt16();
			XlsCFHelper.ReadTemplateParams(reader, this);
			if(RuleType == ConditionalFormattingRuleType.ColorScale)
				this.colorScaleParams = XlsCFColorScaleParams.FromStream(reader);
			else if(RuleType == ConditionalFormattingRuleType.DataBar)
				this.dataBarParams = XlsCFDatabarParams.FromStream(reader);
			else if(RuleType == ConditionalFormattingRuleType.TopOrBottomValue) {
				this.filterParams = XlsCFFilterParams.FromStream(reader);
				FilterTop = FilterParams.Top;
				FilterPercent = FilterParams.Percent;
				FilterValue = FilterParams.Value;
			}
			else if(RuleType == ConditionalFormattingRuleType.IconSet)
				this.iconSetParams = XlsCFIconSetParams.FromStream(reader);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			XlsCondFormat activeFormat = contentBuilder.ActiveConditionalFormat;
			if(activeFormat != null) {
				XlsCondFormatRule item = new XlsCondFormatRule();
				item.RuleType = RuleType;
				item.ComparisonFunction = ComparisonFunction;
				item.ApplyFormat(Format, contentBuilder.DocumentModel);
				XlsRPNContext context = contentBuilder.RPNContext;
				context.WorkbookContext.PushCurrentWorksheet(contentBuilder.CurrentSheet);
				try {
					item.FirstFormula = XlsParsedThingConverter.ToCFExpression(GetFirstFormula(context), context);
					item.SecondFormula = XlsParsedThingConverter.ToCFExpression(GetSecondFormula(context), context);
					item.ActiveFormula = XlsParsedThingConverter.ToCFExpression(GetActiveFormula(context), context);
				}
				finally {
					context.WorkbookContext.PopCurrentWorksheet();
				}
				item.ApplyTemplate(RuleTemplate, TextRule);
				item.StopIfTrue = StopIfTrue;
				item.Priority = Priority;
				item.FilterTop = FilterTop;
				item.FilterPercent = FilterPercent;
				item.FilterValue = FilterValue;
				item.StdDev = StdDev;
				item.ColorScaleParams = ColorScaleParams;
				item.DataBarParams = DataBarParams;
				item.FilterParams = FilterParams;
				item.IconSetParams = IconSetParams;
				activeFormat.Rules.Add(item);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader frtHeader = new FutureRecordHeader();
			frtHeader.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandCF12));
			frtHeader.RangeOfCells = false;
			frtHeader.Write(writer);
			base.WriteCore(writer);
			writer.Write((ushort)activeFormulaSize);
			if(activeFormulaSize > 0)
				writer.Write(activeFormulaBytes);
			byte bitwiseField = 0;
			if(StopIfTrue)
				bitwiseField |= 0x02;
			writer.Write(bitwiseField);
			writer.Write((ushort)Priority);
			writer.Write((ushort)RuleTemplate);
			XlsCFHelper.WriteTemplateParams(writer, this);
			if(RuleType == ConditionalFormattingRuleType.ColorScale)
				this.colorScaleParams.Write(writer);
			else if(RuleType == ConditionalFormattingRuleType.DataBar)
				this.dataBarParams.Write(writer);
			else if(RuleType == ConditionalFormattingRuleType.TopOrBottomValue)
				this.filterParams.Write(writer);
			else if(RuleType == ConditionalFormattingRuleType.IconSet)
				this.iconSetParams.Write(writer);
		}
		protected override short GetSize() {
			int result = 12 + GetSizeCore() + Format.GetSize() + 2 + activeFormulaSize + 5 + 17;
			if(RuleType == ConditionalFormattingRuleType.ColorScale)
				result += this.colorScaleParams.GetSize();
			else if(RuleType == ConditionalFormattingRuleType.DataBar)
				result += this.dataBarParams.GetSize();
			else if(RuleType == ConditionalFormattingRuleType.TopOrBottomValue)
				result += this.filterParams.GetSize();
			else if(RuleType == ConditionalFormattingRuleType.IconSet)
				result += this.iconSetParams.GetSize();
			return (short)result;
		}
		public ParsedExpression GetActiveFormula(XlsRPNContext context) {
			return XlsCFHelper.GetFormulaExpression(context, activeFormulaBytes, activeFormulaSize);
		}
		public void SetActiveFormula(ParsedExpression parsedExpression, XlsRPNContext context) {
			this.activeFormulaBytes = XlsCFHelper.GetFormulaBytes(parsedExpression, context, ref activeFormulaSize);
		}
		protected override DxfN12Info ReadFormat(XlsReader reader) {
			return DxfN12Info.FromStream(reader);
		}
		protected override void WriteFormat(BinaryWriter writer) {
			Format.Write(writer);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandCF12();
		}
	}
	#endregion
	#region XlsCommandCFEx
	public class XlsCommandCFEx : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 18;
		CellRangeInfo range = new CellRangeInfo();
		int id;
		XlsCondFmtExtNonCF12 content = new XlsCondFmtExtNonCF12();
		#endregion
		#region Properties
		public CellRangeInfo Range {
			get { return range; }
			set {
				if(value != null)
					range = value;
				else
					range = new CellRangeInfo();
			}
		}
		public bool IsCF12 { get; set; }
		public int Id {
			get { return id; }
			set {
				ValueChecker.CheckValue(value, 0, 32767, "Id");
				id = value;
			}
		}
		public XlsCondFmtExtNonCF12 Content { get { return content; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeaderRef frtHeader = FutureRecordHeaderRef.FromStream(reader);
			this.range = frtHeader.Range;
			IsCF12 = Convert.ToBoolean(reader.ReadUInt32());
			this.id = reader.ReadUInt16();
			if(!IsCF12)
				this.content = XlsCondFmtExtNonCF12.FromStream(reader);
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			contentBuilder.SetActiveConditionalFormat(Id);
			if(!IsCF12) {
				XlsCondFormat activeFormat = contentBuilder.ActiveConditionalFormat;
				if(activeFormat != null && Content.CFIndex < activeFormat.Rules.Count) {
					XlsCondFormatRule item = activeFormat.Rules[Content.CFIndex];
					item.ComparisonFunction = Content.ComparisonFunction;
					item.ApplyTemplate(Content.RuleTemplate, Content.TextRule);
					item.Priority = content.Priority;
					item.IsActive = content.IsActive;
					item.StopIfTrue = Content.StopIfTrue;
					if(Content.HasFormat)
						item.ApplyFormat(Content.Format, contentBuilder.DocumentModel);
					item.FilterTop = Content.FilterTop;
					item.FilterPercent = Content.FilterPercent;
					item.FilterValue = content.FilterValue;
					item.StdDev = Content.StdDev;
				}
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeaderRef frtHeader = new FutureRecordHeaderRef();
			frtHeader.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandCFEx));
			frtHeader.RangeOfCells = true;
			frtHeader.Range = this.range;
			frtHeader.Write(writer);
			writer.Write((int)(IsCF12 ? 1 : 0));
			writer.Write((ushort)id);
			if(!IsCF12)
				this.content.Write(writer);
		}
		protected override short GetSize() {
			int result = fixedPartSize;
			if(!IsCF12)
				result += this.content.GetSize();
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandCFEx();
		}
	}
	#endregion
}
