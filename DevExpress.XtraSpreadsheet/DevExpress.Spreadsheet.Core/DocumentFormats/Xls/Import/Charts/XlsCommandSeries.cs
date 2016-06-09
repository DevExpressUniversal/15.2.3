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
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsSeriesDataType
	public enum XlsSeriesDataType {
		Numeric = 0x0001,
		Text = 0x0003
	}
	#endregion
	#region XlsCommandChartSeries
	public class XlsCommandChartSeries : XlsCommandBase {
		#region Fields
		int argumentCount;
		int valueCount;
		int bubbleSizeCount;
		#endregion
		#region Properties
		public XlsSeriesDataType ArgumentType { get; set; }
		public int ArgumentCount {
			get { return argumentCount; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "ArgumentCount");
				argumentCount = value;
			}
		}
		public int ValueCount {
			get { return valueCount; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "ValueCount");
				valueCount = value;
			}
		}
		public int BubbleSizeCount {
			get { return bubbleSizeCount; }
			set {
				ValueChecker.CheckValue(value, 0, short.MaxValue, "BubbleSizeCount");
				bubbleSizeCount = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ArgumentType = (XlsSeriesDataType)reader.ReadUInt16();
			reader.ReadUInt16(); 
			argumentCount = reader.ReadUInt16();
			valueCount = reader.ReadUInt16();
			reader.ReadUInt16(); 
			bubbleSizeCount = reader.ReadUInt16();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			Chart chart = contentBuilder.CurrentChart;
			if (chart == null)
				return;
			XlsChartSeriesBuilder builder = new XlsChartSeriesBuilder();
			builder.ArgumentType = ArgumentType;
			builder.ArgumentCount = ArgumentCount;
			builder.ValueCount = ValueCount;
			builder.BubbleSizeCount = BubbleSizeCount;
			contentBuilder.PutChartBuilder(builder);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)ArgumentType);
			writer.Write((ushort)XlsSeriesDataType.Numeric); 
			writer.Write((ushort)argumentCount);
			writer.Write((ushort)valueCount);
			writer.Write((ushort)XlsSeriesDataType.Numeric); 
			writer.Write((ushort)bubbleSizeCount);
		}
		protected override short GetSize() {
			return 12;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsChartDataRefId
	public enum XlsChartDataRefId {
		Name = 0x00,
		Values = 0x01,
		Arguments = 0x02,
		BubbleSize = 0x03
	}
	#endregion
	#region XlsChartDataRefType
	public enum XlsChartDataRefType {
		Auto = 0x00,
		Literal = 0x01,
		Reference = 0x02
	}
	#endregion
	#region XlsChartDataRef
	public class XlsChartDataRef {
		#region Fields
		byte[] formulaBytes = new byte[0];
		ParsedExpression expression = new ParsedExpression();
		string seriesText = string.Empty;
		#endregion
		#region Properties
		public XlsChartDataRefId Id { get; set; }
		public XlsChartDataRefType DataType { get; set; }
		public bool IsSourceLinked { get; set; }
		public int NumberFormatId { get; set; }
		public ParsedExpression Expression { get { return expression; } }
		public string SeriesText { get { return seriesText; } set { seriesText = value; } }
		#endregion
		public static XlsChartDataRef FromStream(XlsReader reader, XlsContentBuilder contentBuilder) {
			XlsChartDataRef result = new XlsChartDataRef();
			result.Read(reader, contentBuilder);
			return result;
		}
		protected void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			Id = (XlsChartDataRefId)reader.ReadByte();
			DataType = (XlsChartDataRefType)reader.ReadByte();
			IsSourceLinked = !Convert.ToBoolean(reader.ReadUInt16());
			NumberFormatId = reader.ReadUInt16();
			int formulaSize = reader.ReadUInt16();
			this.formulaBytes = reader.ReadBytes(formulaSize);
			this.expression = contentBuilder.RPNContext.BinaryToExpression(formulaBytes, formulaSize);
			CheckExpression(contentBuilder);
		}
		public void Write(BinaryWriter writer) {
			writer.Write((byte)Id);
			writer.Write((byte)DataType);
			writer.Write((ushort)(IsSourceLinked ? 0 : 1));
			writer.Write((ushort)NumberFormatId);
			int formulaSize = this.formulaBytes.Length;
			writer.Write((ushort)formulaSize);
			if(formulaSize > 0)
				writer.Write(this.formulaBytes);
		}
		public int GetSize() {
			return this.formulaBytes.Length + 8;
		}
		public void SetExpression(ParsedExpression expression, IRPNContext context) {
			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(context, "context");
			this.expression = expression;
			if (expression.Count > 0) {
				byte[] buf = context.ExpressionToBinary(expression);
				int size = BitConverter.ToUInt16(buf, 0);
				this.formulaBytes = new byte[size];
				Array.Copy(buf, 2, this.formulaBytes, 0, size);
			}
			else
				this.formulaBytes = new byte[0];
		}
		void CheckExpression(XlsContentBuilder contentBuilder) {
			if (this.expression.Count == 0)
				return;
			if (!this.expression.IsXlsChartFormulaCompliant()) {
				SetupErrorExpression(contentBuilder);
				return;
			}
			for (int i = 0; i < this.expression.Count; i++) {
				ParsedThingNameX nameX = this.expression[i] as ParsedThingNameX;
				if (nameX == null)
					continue;
				if (string.IsNullOrEmpty(nameX.DefinedName) || nameX.SheetDefinitionIndex < 0) {
					SetupErrorExpression(contentBuilder);
					return;
				}
			}
		}
		void SetupErrorExpression(XlsContentBuilder contentBuilder) {
			this.expression.Clear();
			SheetDefinition currentSheetDefinition = new SheetDefinition();
			currentSheetDefinition.SheetNameStart = contentBuilder.CurrentSheet.Name;
			ParsedThingErr3d thing = new ParsedThingErr3d(contentBuilder.RPNContext.WorkbookContext.RegisterSheetDefinition(currentSheetDefinition));
			thing.DataType = OperandDataType.Value;
			this.expression.Add(thing);
		}
	}
	#endregion
	#region IXlsChartDataRefContainer
	public interface IXlsChartDataRefContainer {
		void Add(XlsChartDataRef dataRef);
		void SetSeriesText(string value);
	}
	#endregion
	#region XlsCommandChartDataRef
	public class XlsCommandChartDataRef : XlsCommandBase {
		#region Fields
		XlsChartDataRef dataRef = new XlsChartDataRef();
		#endregion
		#region Properties
		public XlsChartDataRef DataRef { get { return dataRef; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.dataRef = XlsChartDataRef.FromStream(reader, contentBuilder);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartDataRefContainer container = contentBuilder.CurrentChartBuilder as IXlsChartDataRefContainer;
			if (container != null)
				container.Add(this.dataRef);
		}
		protected override void WriteCore(BinaryWriter writer) {
			this.dataRef.Write(writer);
		}
		protected override short GetSize() {
			return (short)(this.dataRef.GetSize());
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartSeriesText
	public class XlsCommandChartSeriesText : XlsCommandBase {
		#region Fields
		ShortXLUnicodeString stringValue = new ShortXLUnicodeString();
		#endregion
		#region Properties
		public string Value { get { return stringValue.Value; } set { stringValue.Value = value; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadUInt16(); 
			this.stringValue = ShortXLUnicodeString.FromStream(reader);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartDataRefContainer container = contentBuilder.CurrentChartBuilder as IXlsChartDataRefContainer;
			if (container != null)
				container.SetSeriesText(Value);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)0);
			this.stringValue.Write(writer);
		}
		protected override short GetSize() {
			return (short)(this.stringValue.Length + 2);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartSeriesToView
	public class XlsCommandChartSeriesToView : XlsCommandBase {
		#region Properties
		public int ViewIndex { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ViewIndex = reader.ReadUInt16();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartSeriesBuilder builder = contentBuilder.CurrentChartBuilder as XlsChartSeriesBuilder;
			if (builder == null)
				return;
			builder.ViewIndex = ViewIndex;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)ViewIndex);
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartSeriesParent
	public class XlsCommandChartSeriesParent : XlsCommandBase {
		#region Properties
		public int ParentIndex { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ParentIndex = reader.ReadUInt16();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartSeriesBuilder builder = contentBuilder.CurrentChartBuilder as XlsChartSeriesBuilder;
			if (builder == null)
				return;
			builder.ParentIndex = ParentIndex;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)ParentIndex);
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartSeriesDataIndex
	public class XlsCommandChartSeriesDataIndex : XlsCommandShortPropertyValueBase {
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart != null)
				contentBuilder.CurrentSeriesDataIndex = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartLegendException
	public class XlsCommandChartLegendException : XlsCommandBase {
		#region Fields
		int index;
		#endregion
		#region Properties
		public int Index {
			get { return index; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "Index");
				index = value;
			}
		}
		public bool Deleted { get; set; }
		public bool Formatted { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Index = reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			Deleted = (bitwiseField & 0x0001) != 0;
			Formatted = (bitwiseField & 0x0002) != 0;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			XlsChartLegendEntry entry = new XlsChartLegendEntry();
			entry.Index = Index;
			entry.Deleted = Deleted;
			entry.Formatted = Formatted;
			XlsChartSeriesBuilder builder = contentBuilder.CurrentChartBuilder as XlsChartSeriesBuilder;
			if (builder != null)
				builder.LegendEntries.Add(entry);
			if (Formatted)
				contentBuilder.PutChartBuilder(entry);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)Index);
			ushort bitwiseField = 0;
			if (Deleted)
				bitwiseField |= 0x0001;
			if (Formatted)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 4;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartSeriesAuxTrend
	public class XlsCommandChartSeriesAuxTrend : XlsCommandBase {
		#region Properties
		public ChartTrendlineType TrendlineType { get; set; }
		public int OrderOrPeriod { get; set; }
		public bool DisplayEquation { get; set; }
		public bool DisplayRSquare { get; set; }
		public bool HasIntercept { get; set; }
		public double Intercept { get; set; }
		public double Backward { get; set; }
		public double Forward { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			TrendlineType = CodeToTrendlineType(reader.ReadByte());
			OrderOrPeriod = reader.ReadByte();
			byte[] numIntercept = reader.ReadBytes(8);
			if (numIntercept[7] == 0xff && numIntercept[6] == 0xff) {
				HasIntercept = false;
				Intercept = 0.0;
			}
			else {
				HasIntercept = true;
				Intercept = BitConverter.ToDouble(numIntercept, 0);
			}
			DisplayEquation = reader.ReadBoolean();
			DisplayRSquare = reader.ReadBoolean();
			Forward = reader.ReadDouble();
			Backward = reader.ReadDouble();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartSeriesBuilder builder = contentBuilder.CurrentChartBuilder as XlsChartSeriesBuilder;
			if (builder == null)
				return;
			XlsChartTrendline trendline = builder.Trendline;
			trendline.Apply = true;
			trendline.TrendlineType = TrendlineType;
			trendline.OrderOrPeriod = OrderOrPeriod;
			trendline.HasIntercept = HasIntercept;
			trendline.Intercept = Intercept;
			trendline.DisplayEquation = DisplayEquation;
			trendline.DisplayRSquare = DisplayRSquare;
			trendline.Forward = Forward;
			trendline.Backward = Backward;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(TrendlineTypeToCode(TrendlineType));
			writer.Write((byte)OrderOrPeriod);
			if (HasIntercept) {
				writer.Write(Intercept);
			}
			else {
				writer.Write((int)0);
				writer.Write((ushort)0);
				writer.Write((ushort)0xffff);
			}
			writer.Write((byte)(DisplayEquation ? 1 : 0));
			writer.Write((byte)(DisplayRSquare ? 1 : 0));
			writer.Write(Forward);
			writer.Write(Backward);
		}
		protected override short GetSize() {
			return 28;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		#region Utils
		ChartTrendlineType CodeToTrendlineType(byte code) {
			switch (code) {
				case 0x01: return ChartTrendlineType.Exponential;
				case 0x02: return ChartTrendlineType.Logarithmic;
				case 0x03: return ChartTrendlineType.Power;
				case 0x04: return ChartTrendlineType.MovingAverage;
			}
			return ChartTrendlineType.Polynomial;
		}
		byte TrendlineTypeToCode(ChartTrendlineType type) {
			switch (type) {
				case ChartTrendlineType.Exponential: return 0x01;
				case ChartTrendlineType.Logarithmic: return 0x02;
				case ChartTrendlineType.Power: return 0x03;
				case ChartTrendlineType.MovingAverage: return 0x04;
			}
			return 0x00;
		}
		#endregion
	}
	#endregion
}
