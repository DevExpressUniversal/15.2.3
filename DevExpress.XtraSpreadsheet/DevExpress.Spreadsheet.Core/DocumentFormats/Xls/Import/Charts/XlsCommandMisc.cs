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
using DevExpress.Data.Export;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.DrawingML;
using DevExpress.Office.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandChartLineFormat
	public class XlsCommandChartLineFormat : XlsCommandBase {
		#region Properties
		public bool Auto { get; set; }
		public bool Visible { get; set; }
		public XlsChartLineStyle LineStyle { get; set; }
		public XlsChartLineThickness Thickness { get; set; }
		public bool AutoColor { get; set; }
		public Color LineColor { get; set; }
		public int LineColorIndex { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			LineColor = XlsLongRGB.FromStream(reader);
			LineStyle = (XlsChartLineStyle)reader.ReadUInt16();
			Thickness = (XlsChartLineThickness)reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			Auto = (bitwiseField & 0x0001) != 0;
			Visible = (bitwiseField & 0x0004) != 0;
			AutoColor = (bitwiseField & 0x0008) != 0;
			LineColorIndex = reader.ReadUInt16();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartLineFormat lineFormat = contentBuilder.CurrentChartBuilder as IXlsChartLineFormat;
			if (lineFormat == null)
				return;
			lineFormat.Apply = true;
			lineFormat.Auto = Auto;
			lineFormat.AutoColor = AutoColor;
			lineFormat.LineColor = LineColor;
			lineFormat.LineColorIndex = LineColorIndex;
			lineFormat.LineStyle = LineStyle;
			lineFormat.Thickness = Thickness;
			lineFormat.AxisVisible = Visible;
		}
		protected override void WriteCore(BinaryWriter writer) {
			XlsLongRGB.Write(writer, LineColor);
			writer.Write((ushort)LineStyle);
			writer.Write((ushort)Thickness);
			ushort bitwiseField = 0;
			if(Auto)
				bitwiseField |= 0x0001;
			if(Visible)
				bitwiseField |= 0x0004;
			if(AutoColor)
				bitwiseField |= 0x0008;
			writer.Write(bitwiseField);
			writer.Write((ushort)LineColorIndex);
		}
		protected override short GetSize() {
			return 12;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected internal void CalcSpCheckSum(MsoCrc32Compute crc32) {
			crc32.Add((byte)LineColorIndex);
			if (LineStyle == XlsChartLineStyle.None)
				crc32.Add((byte)0xff); 
			else
				crc32.Add((byte)LineStyle);
			crc32.Add((byte)((short)Thickness + 1));
			crc32.Add((byte)(Auto ? 1 : 0));
			crc32.Add(LineColor.R);
			crc32.Add(LineColor.G);
			crc32.Add(LineColor.B);
			crc32.Add((byte)0);
		}
	}
	#endregion
	#region XlsCommandChartAreaFormat
	public class XlsCommandChartAreaFormat : XlsCommandBase {
		#region Properties
		public Color ForegroundColor { get; set; }
		public Color BackgroundColor { get; set; }
		public XlsChartFillType FillType { get; set; }
		public bool AutoColor { get; set; }
		public bool InvertIfNegative { get; set; }
		public int ForegroundColorIndex { get; set; }
		public int BackgroundColorIndex { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ForegroundColor = XlsLongRGB.FromStream(reader);
			BackgroundColor = XlsLongRGB.FromStream(reader);
			FillType = (XlsChartFillType)reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			AutoColor = (bitwiseField & 0x0001) != 0;
			InvertIfNegative = (bitwiseField & 0x0002) != 0;
			ForegroundColorIndex = reader.ReadUInt16();
			BackgroundColorIndex = reader.ReadUInt16();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartAreaFormat areaFormat = contentBuilder.CurrentChartBuilder as IXlsChartAreaFormat;
			if (areaFormat == null)
				return;
			areaFormat.Apply = true;
			areaFormat.ForegroundColor = ForegroundColor;
			areaFormat.BackgroundColor = BackgroundColor;
			areaFormat.FillType = FillType;
			areaFormat.AutoColor = AutoColor;
			areaFormat.InvertIfNegative = InvertIfNegative;
		}
		protected override void WriteCore(BinaryWriter writer) {
			XlsLongRGB.Write(writer, ForegroundColor);
			XlsLongRGB.Write(writer, BackgroundColor);
			writer.Write((ushort)FillType);
			ushort bitwiseField = 0;
			if (AutoColor)
				bitwiseField |= 0x0001;
			if (InvertIfNegative)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
			writer.Write((ushort)ForegroundColorIndex);
			writer.Write((ushort)BackgroundColorIndex);
		}
		protected override short GetSize() {
			return 16;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected internal void CalcSpCheckSum(MsoCrc32Compute crc32) {
			if (AutoColor)
				return;
			crc32.Add(ForegroundColor.R);
			crc32.Add(ForegroundColor.G);
			crc32.Add(ForegroundColor.B);
			crc32.Add((byte)0);
			crc32.Add(BackgroundColor.R);
			crc32.Add(BackgroundColor.G);
			crc32.Add(BackgroundColor.B);
			crc32.Add((byte)0);
			crc32.Add((byte)FillType);
		}
	}
	#endregion
	#region XlsCommandChartDataFormat
	public class XlsCommandChartDataFormat : XlsCommandBase {
		#region Fields
		int pointIndex = XlsChartDefs.PointIndexOfSeries;
		int seriesIndex;
		int seriesOrder;
		#endregion
		#region Properties
		public int PointIndex {
			get { return pointIndex; }
			set {
				if (value != XlsChartDefs.PointIndexOfSeries)
					ValueChecker.CheckValue(value, 0, ushort.MaxValue, "PointIndex");
				pointIndex = value;
			}
		}
		public int SeriesIndex {
			get { return seriesIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "SeriesIndex");
				seriesIndex = value;
			}
		}
		public int SeriesOrder {
			get { return seriesOrder; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "SeriesOrder");
				seriesOrder = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			PointIndex = reader.ReadUInt16();
			SeriesIndex = reader.ReadUInt16();
			SeriesOrder = reader.ReadUInt16();
			reader.ReadUInt16(); 
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			XlsChartDataFormat dataFormat = new XlsChartDataFormat();
			dataFormat.Container = contentBuilder.CurrentChartBuilder as IXlsChartDataFormatContainer;
			dataFormat.PointIndex = PointIndex;
			dataFormat.SeriesIndex = SeriesIndex;
			dataFormat.SeriesOrder = SeriesOrder;
			contentBuilder.PutChartBuilder(dataFormat);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)PointIndex);
			writer.Write((ushort)SeriesIndex);
			writer.Write((ushort)SeriesOrder);
			writer.Write((ushort)0); 
		}
		protected override short GetSize() {
			return 8;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChart3DBarShape
	public class XlsCommandChart3DBarShape : XlsCommandBase {
		#region Properties
		public BarShape Shape { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Shape = CodeToShape(reader.ReadUInt16());
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartDataFormat format = contentBuilder.CurrentChartBuilder as XlsChartDataFormat;
			if (format != null)
				format.Shape = Shape;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(ShapeToCode(Shape));
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		BarShape CodeToShape(int code) {
			switch(code) {
				case 0x0001: return BarShape.Cylinder;
				case 0x0100: return BarShape.Pyramid;
				case 0x0101: return BarShape.Cone;
				case 0x0200: return BarShape.PyramidToMax;
				case 0x0201: return BarShape.ConeToMax;
			}
			return BarShape.Box;
		}
		ushort ShapeToCode(BarShape shape) {
			switch (shape) {
				case BarShape.Cylinder: return 0x0001;
				case BarShape.Pyramid: return 0x0100;
				case BarShape.Cone: return 0x0101;
				case BarShape.PyramidToMax: return 0x0200;
				case BarShape.ConeToMax: return 0x0201;
			}
			return 0x0000;
		}
	}
	#endregion
	#region XlsCommandChartPieExplosion
	public class XlsCommandChartPieExplosion : XlsCommandShortPropertyValueBase {
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartDataFormat format = contentBuilder.CurrentChartBuilder as XlsChartDataFormat;
			if (format != null)
				format.Explosion = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartSeriesFormat
	public class XlsCommandChartSeriesFormat : XlsCommandBase {
		#region Properties
		public bool SmoothLine { get; set; }
		public bool Bubbles3D { get; set; }
		public bool MarkerShadow { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			SmoothLine = (bitwiseField & 0x0001) != 0;
			Bubbles3D = (bitwiseField & 0x0002) != 0;
			MarkerShadow = (bitwiseField & 0x0004) != 0;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartDataFormat dataFormat = contentBuilder.CurrentChartBuilder as XlsChartDataFormat;
			if (dataFormat == null)
				return;
			XlsChartSeriesFormat format = dataFormat.SeriesFormat;
			format.Apply = true;
			format.SmoothLine = SmoothLine;
			format.Bubbles3D = Bubbles3D;
			format.MarkerShadow = MarkerShadow;
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (SmoothLine)
				bitwiseField |= 0x0001;
			if (Bubbles3D)
				bitwiseField |= 0x0002;
			if (MarkerShadow)
				bitwiseField |= 0x0004;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartMarkerFormat
	public class XlsCommandChartMarkerFormat : XlsCommandBase {
		#region Properties
		public Color ForegroundColor { get; set; }
		public Color BackgroundColor { get; set; }
		public MarkerStyle Marker { get; set; }
		public bool ShowInterior { get; set; }
		public bool ShowBorder { get; set; }
		public int ForegroundColorIndex { get; set; }
		public int BackgroundColorIndex { get; set; }
		public int MarkerSize { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ForegroundColor = XlsLongRGB.FromStream(reader);
			BackgroundColor = XlsLongRGB.FromStream(reader);
			Marker = CodeToMarkerStyle(reader.ReadUInt16());
			ushort bitwiseField = reader.ReadUInt16();
			if ((bitwiseField & 0x0001) != 0)
				Marker = MarkerStyle.Auto;
			ShowInterior = (bitwiseField & 0x0010) == 0;
			ShowBorder = (bitwiseField & 0x0020) == 0;
			ForegroundColorIndex = reader.ReadUInt16();
			BackgroundColorIndex = reader.ReadUInt16();
			MarkerSize = reader.ReadInt32();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartMarkerFormat format = contentBuilder.CurrentChartBuilder as IXlsChartMarkerFormat;
			if (format == null)
				return;
			format.Apply = true;
			format.ForegroundColor = ForegroundColor;
			format.BackgroundColor = BackgroundColor;
			format.MarkerSymbol = Marker;
			format.ShowInterior = ShowInterior;
			format.ShowBorder = ShowBorder;
			format.MarkerSize = MarkerSize / 20;
		}
		protected override void WriteCore(BinaryWriter writer) {
			XlsLongRGB.Write(writer, ForegroundColor);
			XlsLongRGB.Write(writer, BackgroundColor);
			writer.Write((ushort)MarkerStyleToCode(Marker));
			ushort bitwiseField = 0;
			if (Marker == MarkerStyle.Auto)
				bitwiseField |= 0x0001;
			if (!ShowInterior)
				bitwiseField |= 0x0010;
			if (!ShowBorder)
				bitwiseField |= 0x0020;
			writer.Write(bitwiseField);
			writer.Write((ushort)ForegroundColorIndex);
			writer.Write((ushort)BackgroundColorIndex);
			writer.Write(MarkerSize);
		}
		protected override short GetSize() {
			return 20;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		protected internal void CalcSpCheckSum(MsoCrc32Compute crc32) {
			crc32.Add(ForegroundColor.R);
			crc32.Add(ForegroundColor.G);
			crc32.Add(ForegroundColor.B);
			crc32.Add((byte)0);
			crc32.Add(BackgroundColor.R);
			crc32.Add(BackgroundColor.G);
			crc32.Add(BackgroundColor.B);
			crc32.Add((byte)0);
			crc32.Add((byte)XlsChartFillType.Solid);
		}
		#region Utils
		MarkerStyle CodeToMarkerStyle(int code) {
			switch (code) {
				case 0x0001: return MarkerStyle.Square;
				case 0x0002: return MarkerStyle.Diamond;
				case 0x0003: return MarkerStyle.Triangle;
				case 0x0004: return MarkerStyle.X;
				case 0x0005: return MarkerStyle.Star;
				case 0x0006: return MarkerStyle.Dot;
				case 0x0007: return MarkerStyle.Dash;
				case 0x0008: return MarkerStyle.Circle;
				case 0x0009: return MarkerStyle.Plus;
			}
			return MarkerStyle.None;
		}
		ushort MarkerStyleToCode(MarkerStyle style) {
			switch (style) {
				case MarkerStyle.Square: return 0x0001;
				case MarkerStyle.Diamond: return 0x0002;
				case MarkerStyle.Triangle: return 0x0003;
				case MarkerStyle.X: return 0x0004;
				case MarkerStyle.Star: return 0x0005;
				case MarkerStyle.Dot: return 0x0006;
				case MarkerStyle.Dash: return 0x0007;
				case MarkerStyle.Circle: return 0x0008;
				case MarkerStyle.Plus: return 0x0009;
			}
			return 0x0000;
		}
		#endregion
	}
	#endregion
	#region XlsCommandChartFrame
	public class XlsCommandChartFrame : XlsCommandBase {
		#region Properties
		public XlsChartFrameType FrameType { get; set; }
		public bool AutoSize { get; set; }
		public bool AutoPosition { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FrameType = (XlsChartFrameType)reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			AutoSize = (bitwiseField & 0x0001) != 0;
			AutoPosition = (bitwiseField & 0x0002) != 0;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			XlsChartFrame frame = new XlsChartFrame();
			frame.Container = contentBuilder.CurrentChartBuilder as IXlsChartFrameContainer;
			frame.FrameType = FrameType;
			frame.AutoSize = AutoSize;
			frame.AutoPosition = AutoPosition;
			contentBuilder.PutChartBuilder(frame);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)FrameType);
			ushort bitwiseField = 0;
			if (AutoSize)
				bitwiseField |= 0x0001;
			if (AutoPosition)
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
	#region XlsCommandChartML
	public class XlsCommandChartML : XlsCommandRecordBase {
		#region Static Members
		static short[] typeCodes = new short[] {
			0x089f 
		};
		#endregion
		#region Fields
		XlsChartExtProperties extProperties = new XlsChartExtProperties();
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.ContentType == XlsContentType.Chart) {
				FutureRecordHeader header = FutureRecordHeader.FromStream(reader);
				using (XlsCommandStream propStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size - header.GetSize())) {
					using (BinaryReader propReader = new BinaryReader(propStream)) {
						this.extProperties.Read(propReader, contentBuilder);
					}
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			if (contentBuilder.HasChartBuilder) {
				IXlsChartExtPropertyVisitor visitor = contentBuilder.CurrentChartBuilder as IXlsChartExtPropertyVisitor;
				if (extProperties.Parent == XlsChartExtPropParent.View3D)
					visitor = contentBuilder.ChartRootBuilder as IXlsChartExtPropertyVisitor;
				if (visitor != null)
					this.extProperties.Visit(visitor);
			}
			else
				this.extProperties.Visit(new XlsChartExtPropertyVisitor(contentBuilder.CurrentChart));
		}
		public override IXlsCommand GetInstance() {
			this.extProperties.Items.Clear();
			return this;
		}
	}
	#endregion
	#region XlsCommandContinueFrt
	public class XlsCommandChartMLContinue : XlsCommandContinueFrtBase {
		public XlsCommandChartMLContinue() : base() { }
		public override IXlsCommand GetInstance() {
			return new XlsCommandChartMLContinue();
		}
	}
	#endregion
	#region XlsCommandChartText
	public class XlsCommandChartText : XlsCommandBase {
		#region Fields
		const int manualLabelPos = 0x0a;
		int trot;
		#endregion
		#region Properties
		public DrawingTextAlignmentType HorizontalAlignment { get; set; }
		public DrawingTextAnchoringType VerticalAlignment { get; set; }
		public bool IsTransparent { get; set; }
		public Color TextColor { get; set; }
		public int Left { get; set; }
		public int Top { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public bool AutoColor { get; set; }
		public bool ShowKey { get; set; }
		public bool ShowValue { get; set; }
		public bool AutoText { get; set; }
		public bool IsGenerated { get; set; }
		public bool Deleted { get; set; }
		public bool AutoMode { get; set; }
		public bool ShowLabelAndPercent { get; set; }
		public bool ShowPercent { get; set; }
		public bool ShowBubbleSize { get; set; }
		public bool ShowLabel { get; set; }
		public int TextColorIndex { get; set; }
		public DataLabelPosition DataLabelPos { get; set; }
		public bool IsManualDataLabelPos { get; set; }
		public XlReadingOrder TextReadingOrder { get; set; }
		public DrawingTextVerticalTextType VerticalText { get; set; }
		public int RotationAngle { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			HorizontalAlignment = HorizontalTextToDrawingTextAlignmentType(reader.ReadByte());
			VerticalAlignment = VerticalTextToDrawingTextAnchoringType(reader.ReadByte());
			ushort bkgMode = reader.ReadUInt16();
			IsTransparent = bkgMode != 0x0002;
			TextColor = XlsLongRGB.FromStream(reader);
			Left = reader.ReadInt32();
			Top = reader.ReadInt32();
			Width = reader.ReadInt32();
			Height = reader.ReadInt32();
			ushort bitwiseField = reader.ReadUInt16();
			AutoColor = (bitwiseField & 0x0001) != 0;
			ShowKey = (bitwiseField & 0x0002) != 0;
			ShowValue = (bitwiseField & 0x0004) != 0;
			AutoText = (bitwiseField & 0x0010) != 0;
			IsGenerated = (bitwiseField & 0x0020) != 0;
			Deleted = (bitwiseField & 0x0040) != 0;
			AutoMode = (bitwiseField & 0x0080) != 0;
			ShowLabelAndPercent = (bitwiseField & 0x0800) != 0;
			ShowPercent = (bitwiseField & 0x1000) != 0;
			ShowBubbleSize = (bitwiseField & 0x2000) != 0;
			ShowLabel = (bitwiseField & 0x4000) != 0;
			TextColorIndex = reader.ReadUInt16();
			bitwiseField = reader.ReadUInt16();
			int code = bitwiseField & 0x000f;
			DataLabelPos = CodeToPosition(code);
			IsManualDataLabelPos = code == manualLabelPos;
			TextReadingOrder = (XlReadingOrder)((bitwiseField & 0xc000) >> 14);
			this.trot = reader.ReadUInt16();
			VerticalText = XlsChartTextRotationHelper.GetVerticalTextType(this.trot);
			RotationAngle = XlsChartTextRotationHelper.GetRotationAngle(this.trot);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			XlsChartTextBuilder builder = new XlsChartTextBuilder();
			builder.Container = contentBuilder.CurrentChartBuilder as IXlsChartTextContainer;
			builder.HorizontalAlignment = HorizontalAlignment;
			builder.VerticalAlignment = VerticalAlignment;
			builder.IsTransparent = IsTransparent;
			builder.TextColor = TextColor;
			builder.Left = Left;
			builder.Top = Top;
			builder.Width = Width;
			builder.Height = Height;
			builder.AutoColor = AutoColor;
			builder.ShowKey = ShowKey;
			builder.ShowValue = ShowValue;
			builder.AutoText = AutoText;
			builder.IsGenerated = IsGenerated;
			builder.Deleted = Deleted;
			builder.AutoMode = AutoMode;
			builder.ShowLabelAndPercent = ShowLabelAndPercent;
			builder.ShowPercent = ShowPercent;
			builder.ShowBubbleSize = ShowBubbleSize;
			builder.ShowLabel = ShowLabel;
			builder.DataLabelPos = DataLabelPos;
			builder.IsManualDataLabelPos = IsManualDataLabelPos;
			builder.TextReadingOrder = TextReadingOrder;
			builder.VerticalText = VerticalText;
			builder.RotationAngle = RotationAngle;
			builder.TextRotation = trot;
			contentBuilder.PutChartBuilder(builder);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((byte)DrawingTextAlignmentTypeToHorizontalText(HorizontalAlignment));
			writer.Write((byte)DrawingTextAnchoringTypeToVerticalText(VerticalAlignment));
			writer.Write((ushort)(IsTransparent ? 0x0001 : 0x0002));
			XlsLongRGB.Write(writer, TextColor);
			writer.Write(Left);
			writer.Write(Top);
			writer.Write(Width);
			writer.Write(Height);
			ushort bitwiseField = 0;
			if (AutoColor)
				bitwiseField |= 0x0001;
			if (ShowKey)
				bitwiseField |= 0x0002;
			if (ShowValue)
				bitwiseField |= 0x0004;
			if (AutoText)
				bitwiseField |= 0x0010;
			if (IsGenerated)
				bitwiseField |= 0x0020;
			if (Deleted)
				bitwiseField |= 0x0040;
			if (AutoMode)
				bitwiseField |= 0x0080;
			if (ShowLabelAndPercent)
				bitwiseField |= 0x0800;
			if (ShowPercent)
				bitwiseField |= 0x1000;
			if (ShowBubbleSize)
				bitwiseField |= 0x2000;
			if (ShowLabel)
				bitwiseField |= 0x4000;
			writer.Write(bitwiseField);
			writer.Write((ushort)TextColorIndex);
			if (IsManualDataLabelPos)
				bitwiseField = (ushort)manualLabelPos;
			else
				bitwiseField = (ushort)PositionToCode(DataLabelPos);
			bitwiseField = (ushort)(bitwiseField + ((int)TextReadingOrder << 14));
			writer.Write(bitwiseField);
			writer.Write((ushort)XlsChartTextRotationHelper.GetTrot(VerticalText, RotationAngle));
		}
		protected override short GetSize() {
			return 32;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		#region Utils
		DataLabelPosition CodeToPosition(int code) {
			switch (code) {
				case 0x01: return DataLabelPosition.OutsideEnd;
				case 0x02: return DataLabelPosition.InsideEnd;
				case 0x03: return DataLabelPosition.Center;
				case 0x04: return DataLabelPosition.InsideBase;
				case 0x05: return DataLabelPosition.Top;
				case 0x06: return DataLabelPosition.Bottom;
				case 0x07: return DataLabelPosition.Left;
				case 0x08: return DataLabelPosition.Right;
				case 0x09: return DataLabelPosition.BestFit;
			}
			return DataLabelPosition.Default;
		}
		int PositionToCode(DataLabelPosition position) {
			switch (position) {
				case DataLabelPosition.OutsideEnd: return 0x01;
				case DataLabelPosition.InsideEnd: return 0x02;
				case DataLabelPosition.Center: return 0x03;
				case DataLabelPosition.InsideBase: return 0x04;
				case DataLabelPosition.Top: return 0x05;
				case DataLabelPosition.Bottom: return 0x06;
				case DataLabelPosition.Left: return 0x07;
				case DataLabelPosition.Right: return 0x08;
				case DataLabelPosition.BestFit: return 0x09;
			}
			return 0x00;
		}
		public static DrawingTextAlignmentType HorizontalTextToDrawingTextAlignmentType(int align) {
			switch (align) {
				case 1: return DrawingTextAlignmentType.Left;
				case 2: return DrawingTextAlignmentType.Center;
				case 3: return DrawingTextAlignmentType.Right;
				case 4: return DrawingTextAlignmentType.Justified;
				case 7: return DrawingTextAlignmentType.Distributed;
			}
			return DrawingTextAlignmentType.Center;
		}
		public static int DrawingTextAlignmentTypeToHorizontalText(DrawingTextAlignmentType align) {
			switch (align) {
				case DrawingTextAlignmentType.Left: return 1;
				case DrawingTextAlignmentType.Center: return 2;
				case DrawingTextAlignmentType.Right: return 3;
				case DrawingTextAlignmentType.Justified: return 4;
				case DrawingTextAlignmentType.Distributed: return 7;
			}
			return 2;
		}
		public static DrawingTextAnchoringType VerticalTextToDrawingTextAnchoringType(int align) {
			switch (align) {
				case 1: return DrawingTextAnchoringType.Top;
				case 2: return DrawingTextAnchoringType.Center;
				case 3: return DrawingTextAnchoringType.Bottom;
				case 4: return DrawingTextAnchoringType.Justified;
				case 7: return DrawingTextAnchoringType.Distributed;
			}
			return DrawingTextAnchoringType.Center;
		}
		public static int DrawingTextAnchoringTypeToVerticalText(DrawingTextAnchoringType align) {
			switch (align) {
				case DrawingTextAnchoringType.Top: return 1;
				case DrawingTextAnchoringType.Center: return 2;
				case DrawingTextAnchoringType.Bottom: return 3;
				case DrawingTextAnchoringType.Justified: return 4;
				case DrawingTextAnchoringType.Distributed: return 7;
			}
			return 2;
		}
		#endregion
	}
	#endregion
	#region XlsCommandChartTextObjectLink
	public class XlsCommandChartTextObjectLink : XlsCommandBase {
		#region Properties
		public XlsChartTextObjectLinkType LinkType { get; set; }
		public int SeriesIndex { get; set; }
		public int PointIndex { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			LinkType = (XlsChartTextObjectLinkType)reader.ReadUInt16();
			SeriesIndex = reader.ReadUInt16();
			PointIndex = reader.ReadUInt16();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartTextBuilder builder = contentBuilder.CurrentChartBuilder as XlsChartTextBuilder;
			if (builder == null)
				return;
			builder.ObjectLink.Apply = true;
			builder.ObjectLink.LinkType = LinkType;
			builder.ObjectLink.SeriesIndex = SeriesIndex;
			builder.ObjectLink.PointIndex = PointIndex;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)LinkType);
			writer.Write((ushort)SeriesIndex);
			writer.Write((ushort)PointIndex);
		}
		protected override short GetSize() {
			return 6;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartTextRuns
	public class XlsCommandChartTextRuns : XlsCommandBase {
		#region Fields
		readonly List<XlsFormatRun> formatRuns = new List<XlsFormatRun>();
		#endregion
		#region Properties
		public List<XlsFormatRun> FormatRuns { get { return formatRuns; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int count = reader.ReadUInt16();
			for (int i = 0; i < count; i++)
				this.formatRuns.Add(XlsFormatRun.FromStream(reader));
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			XlsChartTextBuilder builder = contentBuilder.CurrentChartBuilder as XlsChartTextBuilder;
			if (builder != null) {
				builder.FormatRuns.Clear();
				builder.FormatRuns.AddRange(this.formatRuns);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			int count = this.formatRuns.Count;
			writer.Write((ushort)count);
			for (int i = 0; i < count; i++)
				this.formatRuns[i].Write(writer);
		}
		protected override short GetSize() {
			return (short)(this.formatRuns.Count * XlsFormatRun.RecordSize + 2);
		}
		public override IXlsCommand GetInstance() {
			this.formatRuns.Clear();
			return this;
		}
	}
	#endregion
	#region XlsCommandChartFontX
	public class XlsCommandChartFontX : XlsCommandShortPropertyValueBase {
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartFontX fontX = contentBuilder.CurrentChartBuilder as IXlsChartFontX;
			if (fontX != null) {
				int index = Value;
				if (index == XlsDefs.UnusedFontIndex)
					contentBuilder.ThrowInvalidFile("Font index equal to 0x04");
				else if (index > XlsDefs.UnusedFontIndex)
					index--;
				fontX.Index = index;
				fontX.Apply = true;
			}
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartAttachedLabel
	public class XlsCommandChartAttachedLabel : XlsCommandBase {
		#region Properties
		public bool ShowValue { get; set; }
		public bool ShowPercent { get; set; }
		public bool ShowLabelAndPercent { get; set; }
		public bool ShowLabel { get; set; }
		public bool ShowBubbleSizes { get; set; }
		public bool ShowSeriesName { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			ShowValue = (bitwiseField & 0x0001) != 0;
			ShowPercent = (bitwiseField & 0x0002) != 0;
			ShowLabelAndPercent = (bitwiseField & 0x0004) != 0;
			ShowLabel = (bitwiseField & 0x0010) != 0;
			ShowBubbleSizes = (bitwiseField & 0x0020) != 0;
			ShowSeriesName = (bitwiseField & 0x0040) != 0;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartDataFormat dataFormat = contentBuilder.CurrentChartBuilder as XlsChartDataFormat;
			if (contentBuilder.CurrentChart == null || dataFormat == null)
				return;
			XlsChartDataLabelFormat dataLabelFormat = dataFormat.DataLabelFormat;
			dataLabelFormat.Apply = true;
			dataLabelFormat.ShowValue = ShowValue;
			dataLabelFormat.ShowPercent = ShowPercent;
			dataLabelFormat.ShowLabelAndPercent = ShowLabelAndPercent;
			dataLabelFormat.ShowLabel = ShowLabel;
			dataLabelFormat.ShowBubbleSizes = ShowBubbleSizes;
			dataLabelFormat.ShowSeriesName = ShowSeriesName;
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (ShowValue)
				bitwiseField |= 0x0001;
			if (ShowPercent)
				bitwiseField |= 0x0002;
			if (ShowLabelAndPercent)
				bitwiseField |= 0x0004;
			if (ShowLabel)
				bitwiseField |= 0x0010;
			if (ShowBubbleSizes)
				bitwiseField |= 0x0020;
			if (ShowSeriesName)
				bitwiseField |= 0x0040;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartDataLabelExtContents
	public class XlsCommandChartDataLabelExtContents : XlsCommandBase {
		#region Fields
		ushort fixedSize = 16;
		XLUnicodeStringNoCch separator = new XLUnicodeStringNoCch();
		#endregion
		#region Properties
		public bool ShowSeriesName { get; set; }
		public bool ShowCategoryName { get; set; }
		public bool ShowValue { get; set; }
		public bool ShowPercent { get; set; }
		public bool ShowBubbleSizes { get; set; }
		public string Separator {
			get { return separator.Value; }
			set { separator.Value = value; }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			ushort bitwiseField = reader.ReadUInt16();
			ShowSeriesName = (bitwiseField & 0x0001) != 0;
			ShowCategoryName = (bitwiseField & 0x0002) != 0;
			ShowValue = (bitwiseField & 0x0004) != 0;
			ShowPercent = (bitwiseField & 0x0008) != 0;
			ShowBubbleSizes = (bitwiseField & 0x0010) != 0;
			int separatorLength = reader.ReadUInt16();
			separator = separatorLength > 0 ? XLUnicodeStringNoCch.FromStream(reader, separatorLength) : new XLUnicodeStringNoCch();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			ApplyChartContentToViewBuilder(contentBuilder.CurrentChartBuilder as XlsChartViewBuilder);
			ApplyChartContentToTextBuilder(contentBuilder.CurrentChartBuilder as XlsChartTextBuilder);
		}
		void ApplyChartContentToViewBuilder(XlsChartViewBuilder viewBuilder) {
			if (viewBuilder != null)
				ApplyContent(viewBuilder.DataLabelExtContent);
		}
		void ApplyChartContentToTextBuilder(XlsChartTextBuilder textBuilder) {
			if (textBuilder != null)
				ApplyContent(textBuilder.DataLabelExtContent);
		}
		void ApplyContent(XlsChartDataLabelExtContent dataLabelExtContent) {
			dataLabelExtContent.Apply = true;
			dataLabelExtContent.ShowSeriesName = ShowSeriesName;
			dataLabelExtContent.ShowCategoryName = ShowCategoryName;
			dataLabelExtContent.ShowValue = ShowValue;
			dataLabelExtContent.ShowPercent = ShowPercent;
			dataLabelExtContent.ShowBubbleSizes = ShowBubbleSizes;
			dataLabelExtContent.Separator = Separator;
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			header.Write(writer);
			ushort bitwiseField = 0;
			if (ShowSeriesName)
				bitwiseField |= 0x0001;
			if (ShowCategoryName)
				bitwiseField |= 0x0002;
			if (ShowValue)
				bitwiseField |= 0x0004;
			if (ShowPercent)
				bitwiseField |= 0x0008;
			if (ShowBubbleSizes)
				bitwiseField |= 0x0010;
			writer.Write(bitwiseField);
			writer.Write((ushort)separator.Value.Length);
			separator.Write(writer);
		}
		protected override short GetSize() {
			return (short)(fixedSize + (!string.IsNullOrEmpty(Separator) ? separator.Length : 0));
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartDataLabExt
	public class XlsCommandChartDataLabExt : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			header.Write(writer);
		}
		protected override short GetSize() {
			return 12;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartShapeProperties
	public class XlsCommandChartShapeProperties : XlsCommandRecordBase {
		#region Static Members
		static short[] typeCodes = new short[] {
			0x087f 
		};
		#endregion
		#region Fields
		XlsChartShapeFormat properties = new XlsChartShapeFormat();
		#endregion
		#region Properties
		public XlsChartShapeFormat Properties { get { return properties; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.ContentType == XlsContentType.Chart) {
				FutureRecordHeader header = FutureRecordHeader.FromStream(reader);
				using (XlsCommandStream propStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size - header.GetSize())) {
					using (BinaryReader propReader = new BinaryReader(propStream)) {
						this.properties = XlsChartShapeFormat.FromStream(propReader);
					}
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartShapeFormatContainer container = contentBuilder.CurrentChartBuilder as IXlsChartShapeFormatContainer;
			if (container != null)
				container.Add(this.properties);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartDefaultText
	public class XlsCommandChartDefaultText : XlsCommandBase {
		#region Properties
		public int Id { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Id = reader.ReadUInt16();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartDefaultTextContainer container = contentBuilder.CurrentChartBuilder as IXlsChartDefaultTextContainer;
			if (contentBuilder.CurrentChart == null || container == null)
				return;
			container.DefaultTextId = Id;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)Id);
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartGelFrame
	public class XlsCommandChartGelFrame : XlsCommandRecordBase {
		#region Fields
		static short[] typeCodes = new short[] {
			0x1066, 
			0x003c  
		};
		OfficeArtProperties artProperties;
		OfficeArtTertiaryProperties artTertiaryProperties;
		#endregion
		#region Properties
		protected internal OfficeArtProperties ArtProperties { get { return artProperties; } }
		protected internal OfficeArtTertiaryProperties ArtTertiaryProperties { get { return artTertiaryProperties; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.ContentType == XlsContentType.Chart) {
				using (XlsCommandStream artStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size)) {
					using (BinaryReader artReader = new BinaryReader(artStream)) {
						OfficeArtRecordHeader header = OfficeArtRecordHeader.FromStream(artReader);
						if (header.TypeCode != OfficeArtTypeCodes.PropertiesTable)
							contentBuilder.ThrowInvalidFile("GelFrame doesn't contains FOPT1");
						artProperties = OfficeArtProperties.FromStream(artReader, header);
						if (artStream.Position == artStream.Length) {
							short typeCode = artStream.GetNextTypeCode();
							if (!artStream.IsAppropriateTypeCode(typeCode)) {
								artTertiaryProperties = null;
								return;
							}
						}
						header = OfficeArtRecordHeader.FromStream(artReader);
						if (header.TypeCode != OfficeArtTypeCodes.TertiaryPropertiesTable)
							contentBuilder.ThrowInvalidFile("GelFrame doesn't contains FOPT2");
						artTertiaryProperties = OfficeArtTertiaryProperties.FromStream(artReader, header);
					}
				}
			}
			else {
				base.ReadCore(reader, contentBuilder);
			}
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartGraphicFormat graphicFormat = contentBuilder.CurrentChartBuilder as IXlsChartGraphicFormat;
			if (graphicFormat == null)
				return;
			graphicFormat.Apply = true;
			graphicFormat.ArtProperties = artProperties;
			graphicFormat.ArtTertiaryProperties = artTertiaryProperties;
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartTextProperties
	public class XlsCommandChartTextProperties : XlsCommandRecordBase {
		#region Static Members
		static short[] typeCodes = new short[] {
			0x087f 
		};
		#endregion
		#region Fields
		XlsChartTextFormat properties = new XlsChartTextFormat();
		#endregion
		#region Properties
		public XlsChartTextFormat Properties { get { return properties; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.ContentType == XlsContentType.Chart) {
				FutureRecordHeader header = FutureRecordHeader.FromStream(reader);
				using (XlsCommandStream propStream = new XlsCommandStream(contentBuilder, reader, typeCodes, Size - header.GetSize())) {
					using (BinaryReader propReader = new BinaryReader(propStream)) {
						this.properties = XlsChartTextFormat.FromStream(propReader);
					}
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			IXlsChartTextFormatContainer container = contentBuilder.CurrentChartBuilder as IXlsChartTextFormatContainer;
			if (container != null)
				container.Add(this.properties);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartFbi
	public class XlsCommandChartFbi : XlsCommandBase {
		#region Properties
		public int DmiXBasis { get; set; }
		public int DmiYBasis { get; set; }
		public int HeightBasis { get; set; }
		public bool Scaling { get; set; }
		public int FontIndex { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			DmiXBasis = reader.ReadUInt16();
			DmiYBasis = reader.ReadUInt16();
			HeightBasis = reader.ReadUInt16();
			int bitwiseField = reader.ReadUInt16();
			Scaling = (bitwiseField & 0x0001) != 0;
			FontIndex = reader.ReadUInt16();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)DmiXBasis);
			writer.Write((ushort)DmiYBasis);
			writer.Write((ushort)HeightBasis);
			writer.Write((ushort)(Scaling ? 0x0001 : 0x0000));
			writer.Write((ushort)FontIndex);
		}
		protected override short GetSize() {
			return 10;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
