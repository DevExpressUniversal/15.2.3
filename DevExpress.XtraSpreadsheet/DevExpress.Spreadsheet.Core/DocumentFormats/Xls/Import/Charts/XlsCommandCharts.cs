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
using DevExpress.XtraSpreadsheet.Export.Xls;
#else
using System.Windows.Media;
using DevExpress.XtraSpreadsheet.Export.Xls;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandChartPropertiesBegin
	public class XlsCommandChartPropertiesBegin : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.PushChartBuilder();
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartPropertiesEnd
	public class XlsCommandChartPropertiesEnd : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			try {
				contentBuilder.CurrentChartBuilder.Execute(contentBuilder);
			}
			finally {
				contentBuilder.PopChartBuilder();
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartObjectBase
	public abstract class XlsCommandChartObjectBase : XlsCommandBase {
		#region Properties
		public XlsChartObjectKind ObjectKind { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeaderOld.FromStream(reader);
			ObjectKind = (XlsChartObjectKind)reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeaderOld header = new FutureRecordHeaderOld();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			header.Write(writer);
			writer.Write((ushort)ObjectKind);
		}
		protected override short GetSize() {
			return 12;
		}
	}
	#endregion
	#region XlsCommandChartStartObject
	public class XlsCommandChartStartObject : XlsCommandChartObjectBase {
		#region Fields
		int objectInstance;
		#endregion
		#region Properties
		public int ObjectInstance {
			get { return objectInstance; }
			set {
				Guard.ArgumentNonNegative(value, "ObjectInstance");
				objectInstance = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			reader.ReadUInt16(); 
			objectInstance = reader.ReadUInt16();
			reader.ReadUInt16(); 
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			writer.Write((ushort)0); 
			writer.Write((ushort)objectInstance);
			writer.Write((ushort)0); 
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartEndObject
	public class XlsCommandChartEndObject : XlsCommandChartObjectBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			long startPos = reader.Position;
			base.ReadCore(reader, contentBuilder);
			int bytesToRead = (int)(Size - (reader.Position - startPos));
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartPrintSize
	public class XlsCommandChartPrintSize : XlsCommandBase {
		#region Properties
		public XlsChartPrintSize PrintSize { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			PrintSize = (XlsChartPrintSize)reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)PrintSize);
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartUnits
	public class XlsCommandChartUnits : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)0);
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChart
	public class XlsCommandChart : XlsCommandBase {
		#region Fields
		const short fixedSize = 16;
		FixedPoint x = new FixedPoint();
		FixedPoint y = new FixedPoint();
		FixedPoint dx = new FixedPoint();
		FixedPoint dy = new FixedPoint();
		#endregion
		#region Properties
		public double Left {
			get { return x.Value; }
			set { x.Value = value; }
		}
		public double Top {
			get { return y.Value; }
			set { y.Value = value; }
		}
		public double Width {
			get { return dx.Value; }
			set { dx.Value = value; }
		}
		public double Height {
			get { return dy.Value; }
			set { dy.Value = value; }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			byte[] data = reader.ReadBytes(fixedSize);
			using (MemoryStream dataStream = new MemoryStream(data, false)) {
				using (BinaryReader dataReader = new BinaryReader(dataStream)) {
					x = FixedPoint.FromStream(dataReader);
					y = FixedPoint.FromStream(dataReader);
					dx = FixedPoint.FromStream(dataReader);
					dy = FixedPoint.FromStream(dataReader);
				}
			}
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			XlsChartRootBuilder builder = new XlsChartRootBuilder();
			builder.Left = Left;
			builder.Top = Top;
			builder.Width = Width;
			builder.Height = Height;
			contentBuilder.PutChartBuilder(builder);
		}
		protected override void WriteCore(BinaryWriter writer) {
			x.Write(writer);
			y.Write(writer);
			dx.Write(writer);
			dy.Write(writer);
		}
		protected override short GetSize() {
			return fixedSize;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartPlotGrowth
	public class XlsCommandChartPlotGrowth : XlsCommandBase {
		#region Fields
		const short fixedSize = 8;
		FixedPoint dx;
		FixedPoint dy;
		#endregion
		public XlsCommandChartPlotGrowth() 
			: base() {
			dx = new FixedPoint();
			dy = new FixedPoint();
			dx.Value = 1.0;
			dy.Value = 1.0;
		}
		#region Properties
		public double Horizontal {
			get { return dx.Value; }
			set { dx.Value = value; }
		}
		public double Vertical {
			get { return dy.Value; }
			set { dy.Value = value; }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			byte[] data = reader.ReadBytes(fixedSize);
			using (MemoryStream dataStream = new MemoryStream(data, false)) {
				using (BinaryReader dataReader = new BinaryReader(dataStream)) {
					dx = FixedPoint.FromStream(dataReader);
					dy = FixedPoint.FromStream(dataReader);
				}
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			dx.Write(writer);
			dy.Write(writer);
		}
		protected override short GetSize() {
			return fixedSize;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartSpaceProperties
	public class XlsCommandChartSpaceProperties : XlsCommandBase {
		#region Fields
		DisplayBlanksAs dispBlankAs = DisplayBlanksAs.Gap;
		#endregion
		#region Properties
		public bool ManualSeriesAllocation { get; set; }
		public bool PlotVisibleOnly { get; set; }
		public bool NotSizeWith { get; set; }
		public bool ManualPlotArea { get; set; }
		public bool AlwaysAutoPlotArea { get; set; }
		public DisplayBlanksAs DispBlankAs { get { return dispBlankAs; } set { dispBlankAs = value; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			ManualSeriesAllocation = Convert.ToBoolean(bitwiseField & 0x0001);
			PlotVisibleOnly = Convert.ToBoolean(bitwiseField & 0x0002);
			NotSizeWith = Convert.ToBoolean(bitwiseField & 0x0004);
			ManualPlotArea = Convert.ToBoolean(bitwiseField & 0x0008);
			AlwaysAutoPlotArea = Convert.ToBoolean(bitwiseField & 0x0010);
			dispBlankAs = Translate(reader.ReadByte());
			int bytesToRead = Size - 3;
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			Chart chart = contentBuilder.CurrentChart;
			if (chart == null)
				return;
			chart.BeginUpdate();
			try {
				chart.DispBlanksAs = DispBlankAs;
				chart.PlotVisibleOnly = PlotVisibleOnly;
			}
			finally {
				chart.EndUpdate();
			}
			XlsChartRootBuilder builder = contentBuilder.CurrentChartBuilder as XlsChartRootBuilder;
			if (builder != null) {
				builder.ManualPlotArea = ManualPlotArea;
				builder.AlwaysAutoPlotArea = AlwaysAutoPlotArea;
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(ManualSeriesAllocation)
				bitwiseField |= 0x0001;
			if(PlotVisibleOnly)
				bitwiseField |= 0x0002;
			if(NotSizeWith)
				bitwiseField |= 0x0004;
			if (ManualPlotArea)
				bitwiseField |= 0x0008;
			if (AlwaysAutoPlotArea)
				bitwiseField |= 0x0010;
			writer.Write(bitwiseField);
			writer.Write((ushort)Translate(dispBlankAs));
		}
		protected override short GetSize() {
			return 4;
		}
		DisplayBlanksAs Translate(int value) {
			if (value == 0x00)
				return DisplayBlanksAs.Gap;
			if (value == 0x01)
				return DisplayBlanksAs.Zero;
			return DisplayBlanksAs.Span;
		}
		int Translate(DisplayBlanksAs value) {
			if (value == DisplayBlanksAs.Gap)
				return 0x00;
			if (value == DisplayBlanksAs.Zero)
				return 0x01;
			return 0x02;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartLegend
	public class XlsCommandChartLegend : XlsCommandBase {
		#region Fields
		int left;
		int top;
		int width;
		int height;
		#endregion
		#region Properties
		public int Left {
			get { return left; }
			set {
				Guard.ArgumentNonNegative(value, "Left");
				left = value;
			}
		}
		public int Top {
			get { return top; }
			set {
				Guard.ArgumentNonNegative(value, "Top");
				top = value;
			}
		}
		public int Width {
			get { return width; }
			set {
				Guard.ArgumentNonNegative(value, "Width");
				width = value;
			}
		}
		public int Height {
			get { return height; }
			set {
				Guard.ArgumentNonNegative(value, "Height");
				height = value;
			}
		}
		public bool AutoPosition { get; set; }
		public bool AutoXPos { get; set; }
		public bool AutoYPos { get; set; }
		public bool IsVertical { get; set; }
		public bool WasDataTable { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Left = reader.ReadInt32();
			Top = reader.ReadInt32();
			Width = reader.ReadInt32();
			Height = reader.ReadInt32();
			reader.ReadUInt16(); 
			ushort bitwiseField = reader.ReadUInt16();
			AutoPosition = (bitwiseField & 0x0001) != 0;
			AutoXPos = (bitwiseField & 0x0004) != 0;
			AutoYPos = (bitwiseField & 0x0008) != 0;
			IsVertical = (bitwiseField & 0x0010) != 0;
			WasDataTable = (bitwiseField & 0x0020) != 0;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			XlsChartLegendBuilder builder = new XlsChartLegendBuilder();
			builder.Left = Left;
			builder.Top = Top;
			builder.Width = Width;
			builder.Height = Height;
			builder.AutoPosition = AutoPosition;
			builder.AutoXPos = AutoXPos;
			builder.AutoYPos = AutoYPos;
			builder.IsVertical = IsVertical;
			builder.WasDataTable = WasDataTable;
			contentBuilder.PutChartBuilder(builder);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(Left);
			writer.Write(Top);
			writer.Write(Width);
			writer.Write(Height);
			writer.Write((ushort)0x0100); 
			ushort bitwiseField = 0x0002;
			if (AutoPosition)
				bitwiseField |= 0x0001;
			if (AutoXPos)
				bitwiseField |= 0x0004;
			if (AutoYPos)
				bitwiseField |= 0x0008;
			if (IsVertical)
				bitwiseField |= 0x0010;
			if (WasDataTable)
				bitwiseField |= 0x0020;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 20;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartPos
	public class XlsCommandChartPos : XlsCommandBase {
		#region Fields
		int left;
		int top;
		int width;
		int height;
		#endregion
		#region Properties
		public XlsChartPosMode TopLeftMode { get; set; }
		public XlsChartPosMode BottomRightMode { get; set; }
		public int Left {
			get { return left; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Left");
				left = value;
			}
		}
		public int Top {
			get { return top; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Top");
				top = value;
			}
		}
		public int Width {
			get { return width; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Width");
				width = value;
			}
		}
		public int Height {
			get { return height; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Height");
				height = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			TopLeftMode = (XlsChartPosMode)reader.ReadUInt16();
			BottomRightMode = (XlsChartPosMode)reader.ReadUInt16();
			Left = reader.ReadInt16();
			reader.ReadInt16(); 
			Top = reader.ReadInt16();
			reader.ReadInt16(); 
			Width = reader.ReadInt16();
			reader.ReadInt16(); 
			Height = reader.ReadInt16();
			reader.ReadInt16(); 
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartPosition position = contentBuilder.CurrentChartBuilder as IXlsChartPosition;
			if (position == null)
				return;
			position.Apply = true;
			position.TopLeftMode = TopLeftMode;
			position.BottomRightMode = BottomRightMode;
			position.Left = Left;
			position.Top = Top;
			position.Width = Width;
			position.Height = Height;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)TopLeftMode);
			writer.Write((short)BottomRightMode);
			writer.Write((short)Left);
			writer.Write((short)0); 
			writer.Write((short)Top);
			writer.Write((short)0); 
			writer.Write((short)Width);
			writer.Write((short)0); 
			writer.Write((short)Height);
			writer.Write((short)0); 
		}
		protected override short GetSize() {
			return 20;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartLayout12
	public class XlsCommandChartLayout12 : XlsCommandBase {
		#region Properties
		public int CheckSum { get; set; }
		public LegendPosition LegendPos { get; set; }
		public LayoutMode XMode { get; set; }
		public LayoutMode YMode { get; set; }
		public LayoutMode WidthMode { get; set; }
		public LayoutMode HeightMode { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			CheckSum = reader.ReadInt32();
			ushort bitwiseField = reader.ReadUInt16();
			LegendPos = CodeToLegendPos((bitwiseField & 0x001e) >> 1);
			XMode = (LayoutMode)reader.ReadUInt16();
			YMode = (LayoutMode)reader.ReadUInt16();
			WidthMode = (LayoutMode)reader.ReadUInt16();
			HeightMode = (LayoutMode)reader.ReadUInt16();
			X = reader.ReadDouble();
			Y = reader.ReadDouble();
			Width = reader.ReadDouble();
			Height = reader.ReadDouble();
			reader.ReadUInt16(); 
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartLayout12 layout = contentBuilder.CurrentChartBuilder as IXlsChartLayout12;
			if (layout == null)
				return;
			layout.Apply = CheckSum == CalcCheckSum(contentBuilder, true);
			layout.LegendPos = LegendPos;
			layout.XMode = XMode;
			layout.YMode = YMode;
			layout.WidthMode = WidthMode;
			layout.HeightMode = HeightMode;
			layout.X = X;
			layout.Y = Y;
			layout.Width = Width;
			layout.Height = Height;
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			header.Write(writer);
			writer.Write(CheckSum);
			writer.Write((ushort)(LegendPosToCode(LegendPos) << 1));
			writer.Write((ushort)XMode);
			writer.Write((ushort)YMode);
			writer.Write((ushort)WidthMode);
			writer.Write((ushort)HeightMode);
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Width);
			writer.Write(Height);
			writer.Write((ushort)0); 
		}
		protected override short GetSize() {
			return 60;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		#region Utils
		LegendPosition CodeToLegendPos(int code) {
			switch (code) {
				case 0x01: return LegendPosition.TopRight;
				case 0x02: return LegendPosition.Top;
				case 0x03: return LegendPosition.Right;
				case 0x04: return LegendPosition.Left;
			}
			return LegendPosition.Bottom;
		}
		int LegendPosToCode(LegendPosition pos) {
			switch (pos) {
				case LegendPosition.TopRight: return 0x01;
				case LegendPosition.Top: return 0x02;
				case LegendPosition.Right: return 0x03;
				case LegendPosition.Left: return 0x04;
			}
			return 0x00;
		}
		int CalcCheckSum(XlsContentBuilder contentBuilder, bool embedded) {
			int result = 0;
			if (contentBuilder.CurrentChartBuilder is XlsChartLegendBuilder)
				result = CalcLegendCheckSum(contentBuilder, embedded);
			if (contentBuilder.CurrentChartBuilder is XlsChartTextBuilder) {
				XlsChartTextBuilder textBuilder = contentBuilder.CurrentChartBuilder as XlsChartTextBuilder;
				bool auto = XMode == LayoutMode.Auto && YMode == LayoutMode.Auto && WidthMode == LayoutMode.Auto && HeightMode == LayoutMode.Auto;
				int autoNumeric = auto ? 0 : 1;
				result = textBuilder.Position.Left ^ textBuilder.Position.Top ^ autoNumeric;
			}
			return result;
		}
		int CalcLegendCheckSum(XlsContentBuilder contentBuilder, bool embedded) {
			int result = 0;
			XlsChartRootBuilder rootBuilder = contentBuilder.ChartRootBuilder as XlsChartRootBuilder;
			XlsChartLegendBuilder legendBuilder = contentBuilder.CurrentChartBuilder as XlsChartLegendBuilder;
			if (rootBuilder != null && legendBuilder != null) {
				if (legendBuilder.Position.Apply) {
					result ^= legendBuilder.Position.Left;
					result ^= legendBuilder.Position.Top;
				}
				int chartWidth = (int)((rootBuilder.Width - 8) * DocumentModel.DpiX / 72);
				int chartHeight = (int)((rootBuilder.Height - 8) * DocumentModel.DpiY / 72);
				if (!embedded && rootBuilder.Frame != null && rootBuilder.Frame.FrameType == XlsChartFrameType.FrameWithShadow) {
					chartWidth -= 2;
					chartHeight -= 2;
				}
				int legendWidth = chartWidth * legendBuilder.Width / 4000;
				int legendHeight = chartHeight * legendBuilder.Height / 4000;
				result ^= legendWidth;
				result ^= legendHeight;
				result ^= legendBuilder.AutoXPos ? 1 : 0;
				result ^= legendBuilder.AutoYPos ? 1 : 0;
				if (legendBuilder.Frame != null)
					result ^= legendBuilder.Frame.AutoSize ? 1 : 0;
			}
			return result;
		}
		#endregion
	}
	#endregion
	#region XlsCommandChartPlotArea
	public class XlsCommandChartPlotArea : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if(Size > 0)
				reader.ReadBytes(Size);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			XlsChartAxisGroupBuilder builder = contentBuilder.CurrentChartBuilder as XlsChartAxisGroupBuilder;
			if (builder != null)
				builder.ApplyPlotAreaFormat = true;
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartLayout12A
	public class XlsCommandChartLayout12A : XlsCommandBase {
		#region Fields
		int left;
		int top;
		int right;
		int bottom;
		#endregion
		#region Properties
		public int CheckSum { get; set; }
		public LayoutTarget Target { get; set; }
		public int Left {
			get { return left; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Left");
				left = value;
			}
		}
		public int Top {
			get { return top; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Top");
				top = value;
			}
		}
		public int Right {
			get { return right; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Right");
				right = value;
			}
		}
		public int Bottom {
			get { return bottom; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, short.MaxValue, "Bottom");
				bottom = value;
			}
		}
		public LayoutMode XMode { get; set; }
		public LayoutMode YMode { get; set; }
		public LayoutMode WidthMode { get; set; }
		public LayoutMode HeightMode { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			CheckSum = reader.ReadInt32();
			ushort bitwiseField = reader.ReadUInt16();
			Target = (bitwiseField & 0x0001) != 0 ? LayoutTarget.Inner : LayoutTarget.Outer;
			Left = reader.ReadInt16();
			Top = reader.ReadInt16();
			Right = reader.ReadInt16();
			Bottom = reader.ReadInt16();
			XMode = (LayoutMode)reader.ReadUInt16();
			YMode = (LayoutMode)reader.ReadUInt16();
			WidthMode = (LayoutMode)reader.ReadUInt16();
			HeightMode = (LayoutMode)reader.ReadUInt16();
			X = reader.ReadDouble();
			Y = reader.ReadDouble();
			Width = reader.ReadDouble();
			Height = reader.ReadDouble();
			reader.ReadUInt16(); 
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartRootBuilder builder = contentBuilder.CurrentChartBuilder as XlsChartRootBuilder;
			if (builder == null || contentBuilder.CurrentChart == null)
				return;
			int calculatedCheckSum = (builder.ManualPlotArea ^ builder.AlwaysAutoPlotArea) ? 0 : 1;
			XlsChartLayout12A layout = builder.PlotAreaLayout;
			layout.Apply = calculatedCheckSum == CheckSum;
			layout.Target = Target;
			layout.XMode = XMode;
			layout.YMode = YMode;
			layout.WidthMode = WidthMode;
			layout.HeightMode = HeightMode;
			layout.X = X;
			layout.Y = Y;
			layout.Width = Width;
			layout.Height = Height;
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			header.Write(writer);
			writer.Write(CheckSum);
			writer.Write((ushort)(Target == LayoutTarget.Inner ? 1 : 0));
			writer.Write((short)Left);
			writer.Write((short)Top);
			writer.Write((short)Right);
			writer.Write((short)Bottom);
			writer.Write((ushort)XMode);
			writer.Write((ushort)YMode);
			writer.Write((ushort)WidthMode);
			writer.Write((ushort)HeightMode);
			writer.Write(X);
			writer.Write(Y);
			writer.Write(Width);
			writer.Write(Height);
			writer.Write((ushort)0); 
		}
		protected override short GetSize() {
			return 68;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsChartBlockObjectKind
	public enum XlsChartBlockObjectKind {
		AxisGroup = 0x0000,
		AttachedLabel = 0x0002,
		Axis = 0x0004,
		ChartGroup = 0x0005,
		DataTable = 0x0006,
		Frame = 0x0007,
		Legend = 0x0009,
		LegendException = 0x000a,
		Series = 0x000c,
		Chart = 0x000d,
		DataFormat = 0x000e,
		DropBar = 0x000f
	}
	#endregion
	#region XlsCommandChartBlockBase
	public abstract class XlsCommandChartBlockBase : XlsCommandBase {
		#region Properties
		public XlsChartBlockObjectKind ObjectKind { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeaderOld.FromStream(reader);
			ObjectKind = (XlsChartBlockObjectKind)reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeaderOld header = new FutureRecordHeaderOld();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			header.Write(writer);
			writer.Write((ushort)ObjectKind);
		}
		protected override short GetSize() {
			return 12;
		}
	}
	#endregion
	#region XlsCommandChartStartBlock
	public class XlsCommandChartStartBlock : XlsCommandChartBlockBase {
		#region Fields
		int objectContext;
		int objectInstance1;
		int objectInstance2;
		#endregion
		#region Properties
		public int ObjectContext {
			get { return objectContext; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "ObjectContext");
				objectContext = value;
			}
		}
		public int ObjectInstance1 {
			get { return objectInstance1; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "ObjectInstance1");
				objectInstance1 = value;
			}
		}
		public int ObjectInstance2 {
			get { return objectInstance2; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "ObjectInstance2");
				objectInstance2 = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			objectContext = reader.ReadUInt16();
			objectInstance1 = reader.ReadUInt16();
			objectInstance2 = reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			writer.Write((ushort)objectContext);
			writer.Write((ushort)objectInstance1);
			writer.Write((ushort)objectInstance2);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartEndBlock
	public class XlsCommandChartEndBlock : XlsCommandChartBlockBase {
		#region Fields
		const int fixedPartSize = 6;
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			int bytesToRead = Size - fixedPartSize;
			if(bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
			writer.Write((ushort)0); 
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsFrtFontListType
	public enum XlsFrtFontListType {
		YMult = 9,
		DataLabExt = 10
	}
	#endregion
	#region XlsCommandChartFrtFontList
	public class XlsCommandChartFrtFontList : XlsCommandBase {
		#region Fields
		readonly List<XlsChartFrtFontInfo> frtFonts = new List<XlsChartFrtFontInfo>();
		#endregion
		#region Properties
		public XlsFrtFontListType FontListType { get; set; }
		public List<XlsChartFrtFontInfo> FrtFonts { get { return frtFonts; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeaderOld.FromStream(reader);
			FontListType = (XlsFrtFontListType)reader.ReadByte();
			reader.ReadByte(); 
			int count = reader.ReadUInt16();
			for (int i = 0; i < count; i++)
				this.frtFonts.Add(XlsChartFrtFontInfo.FromStream(reader));
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeaderOld header = new FutureRecordHeaderOld();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			header.Write(writer);
			writer.Write((ushort)FontListType);
			int count = this.frtFonts.Count;
			writer.Write((ushort)count);
			for (int i = 0; i < count; i++)
				this.frtFonts[i].Write(writer);
		}
		protected override short GetSize() {
			return (short)(this.frtFonts.Count * XlsChartFrtFontInfo.RecordSize + 8);
		}
		public override IXlsCommand GetInstance() {
			this.frtFonts.Clear();
			return this;
		}
	}
	#endregion
	#region XlsChartFrtFontInfo
	public class XlsChartFrtFontInfo {
		public const int RecordSize = 4;
		public XlsChartFrtFontInfo() {
		}
		#region Properties
		public bool Scaled { get; set; }
		public int FontIndex { get; set; }
		#endregion
		public static XlsChartFrtFontInfo FromStream(XlsReader reader) {
			XlsChartFrtFontInfo result = new XlsChartFrtFontInfo();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			int bitwiseField = reader.ReadUInt16();
			Scaled = (bitwiseField & 0x0001) != 0;
			FontIndex = reader.ReadUInt16();
		}
		public void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if (chunkWriter != null) chunkWriter.BeginBlock();
			ushort bitwiseField = 0;
			if (Scaled)
				bitwiseField |= 0x0001;
			writer.Write(bitwiseField);
			writer.Write((ushort)FontIndex);
			if (chunkWriter != null) chunkWriter.EndBlock();
		}
	}
	#endregion
	#region XlsApplicationVersion
	public enum XlsApplicationVersion {
		Excel2000 = 9,
		Excel2002OfficeExcel2003 = 10,
		OfficeExcel2007 = 11,
		Excel2010 = 12
	}
	#endregion
	#region XlsCommandChartFrtInfo
	public class XlsCommandChartFrtInfo : XlsCommandBase {
		#region Fields
		int idCount;
		XlsApplicationVersion writer;
		#endregion
		#region Properties
		public XlsApplicationVersion Originator { get; set; }
		public XlsApplicationVersion Writer {
			get { return writer; }
			set {
				if (writer == value)
					return;
				writer = value;
				idCount = GetCount(writer);
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeaderOld.FromStream(reader);
			Originator = (XlsApplicationVersion)reader.ReadByte();
			Writer = (XlsApplicationVersion)reader.ReadByte();
			reader.ReadBytes(Size - 6);
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeaderOld header = new FutureRecordHeaderOld();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			header.Write(writer);
			writer.Write((byte)Originator);
			writer.Write((byte)Writer);
			WriteFrtIds(writer);
		}
		protected override short GetSize() {
			return (short)(this.idCount * 4 + 8);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		int GetCount(XlsApplicationVersion version) {
			if (version == XlsApplicationVersion.Excel2000)
				return 1;
			else if (version == XlsApplicationVersion.Excel2002OfficeExcel2003)
				return 3;
			return 4;
		}
		void WriteFrtIds(BinaryWriter writer) {
			writer.Write((ushort)idCount);
			if (idCount == 1)
				WriteFrtIdsForExcel2000(writer);
			else if (idCount == 3)
				WriteFrtIdsForExcel2003(writer);
			else
				WriteFrtIdsForExcel2007(writer);
		}
		void WriteFrtIdsForExcel2000(BinaryWriter writer) {
			writer.Write((ushort)0x0850);
			writer.Write((ushort)0x085a);
		}
		void WriteFrtIdsForExcel2003(BinaryWriter writer) {
			WriteFrtIdsForExcel2000(writer);
			writer.Write((ushort)0x0861);
			writer.Write((ushort)0x0861);
			writer.Write((ushort)0x086a);
			writer.Write((ushort)0x086b);
		}
		void WriteFrtIdsForExcel2007(BinaryWriter writer) {
			WriteFrtIdsForExcel2003(writer);
			writer.Write((ushort)0x089d);
			writer.Write((ushort)0x08a6);
		}
	}
	#endregion
}
