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
	#region XlsCommandChartViewFormat
	public class XlsCommandChartViewFormat : XlsCommandBase {
		#region Fields
		int zOrder;
		#endregion
		#region Properties
		public bool VaryColors { get; set; }
		public int ZOrder {
			get { return zOrder; }
			set {
				ValueChecker.CheckValue(value, 0, 9, "ZOrder");
				zOrder = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadInt32(); 
			reader.ReadInt32(); 
			reader.ReadInt32(); 
			reader.ReadInt32(); 
			ushort bitwiseField = reader.ReadUInt16();
			VaryColors = Convert.ToBoolean(bitwiseField & 0x0001);
			ZOrder = reader.ReadUInt16();
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			Chart chart = contentBuilder.CurrentChart;
			if (chart == null)
				return;
			XlsChartViewBuilder builder = new XlsChartViewBuilder();
			builder.Container = contentBuilder.CurrentChartBuilder as IXlsChartViewContainer;
			builder.VaryColors = VaryColors;
			builder.ZOrder = ZOrder;
			contentBuilder.PutChartBuilder(builder);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			ushort bitwiseField = 0;
			if(VaryColors)
				bitwiseField |= 0x0001;
			writer.Write(bitwiseField);
			writer.Write((ushort)ZOrder);
		}
		protected override short GetSize() {
			return 20;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartViewBar
	public class XlsCommandChartViewBar : XlsCommandBase {
		#region Fields
		int gapWidth;
		int overlap;
		#endregion
		#region Properties
		public int GapWidth {
			get { return gapWidth; }
			set {
				ValueChecker.CheckValue(value, 0, 500, "GapWidth");
				gapWidth = value;
			}
		}
		public int Overlap {
			get { return -overlap; }
			set {
				ValueChecker.CheckValue(value, -100, 100, "Overlap");
				overlap = -value;
			}
		}
		public bool Transpose { get; set; }
		public bool Stacked { get; set; }
		public bool PercentStacked { get; set; }
		public bool HasShadow { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			this.overlap = reader.ReadInt16();
			GapWidth = reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			Transpose = Convert.ToBoolean(bitwiseField & 0x0001);
			Stacked = Convert.ToBoolean(bitwiseField & 0x0002);
			PercentStacked = Convert.ToBoolean(bitwiseField & 0x0004);
			HasShadow = Convert.ToBoolean(bitwiseField & 0x0008);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.ViewType = ChartViewType.Bar;
			chartBuilder.Overlap = Overlap;
			chartBuilder.GapWidth = GapWidth;
			chartBuilder.BarDirection = Transpose ? BarChartDirection.Bar : BarChartDirection.Column;
			if (PercentStacked && Stacked)
				chartBuilder.BarGrouping = BarChartGrouping.PercentStacked;
			else if(Stacked)
				chartBuilder.BarGrouping = BarChartGrouping.Stacked;
			else
				chartBuilder.BarGrouping = BarChartGrouping.Clustered;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)this.overlap);
			writer.Write((ushort)GapWidth);
			ushort bitwiseField = 0;
			if(Transpose)
				bitwiseField |= 0x0001;
			if(Stacked)
				bitwiseField |= 0x0002;
			if(PercentStacked)
				bitwiseField |= 0x0004;
			if(HasShadow)
				bitwiseField |= 0x0008;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 6;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartViewLineBase
	public abstract class XlsCommandChartViewLineBase : XlsCommandBase {
		#region Properties
		public bool Stacked { get; set; }
		public bool PercentStacked { get; set; }
		public bool HasShadow { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			Stacked = Convert.ToBoolean(bitwiseField & 0x0001);
			PercentStacked = Convert.ToBoolean(bitwiseField & 0x0002);
			HasShadow = Convert.ToBoolean(bitwiseField & 0x0004);
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if (Stacked)
				bitwiseField |= 0x0001;
			if (PercentStacked)
				bitwiseField |= 0x0002;
			if (HasShadow)
				bitwiseField |= 0x0004;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 2;
		}
		protected void SetupGrouping(XlsChartViewBuilder chartBuilder) {
			if (PercentStacked && Stacked)
				chartBuilder.Grouping = ChartGrouping.PercentStacked;
			else if (Stacked)
				chartBuilder.Grouping = ChartGrouping.Stacked;
			else
				chartBuilder.Grouping = ChartGrouping.Standard;
		}
	}
	#endregion
	#region XlsCommandChartViewLine
	public class XlsCommandChartViewLine : XlsCommandChartViewLineBase {
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.ViewType = ChartViewType.Line;
			SetupGrouping(chartBuilder);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartViewArea
	public class XlsCommandChartViewArea : XlsCommandChartViewLineBase {
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.ViewType = ChartViewType.Area;
			SetupGrouping(chartBuilder);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartViewPie
	public class XlsCommandChartViewPie : XlsCommandBase {
		#region Fields
		int firstSliceAngle;
		int doughnutHoleSize;
		#endregion
		#region Properties
		public int FirstSliceAngle {
			get { return firstSliceAngle; }
			set {
				ValueChecker.CheckValue(value, 0, 360, "FirstSliceAngle");
				firstSliceAngle = value;
			}
		}
		public int DoughnutHoleSize {
			get { return doughnutHoleSize; }
			set {
				if(value != 0)
					ValueChecker.CheckValue(value, 10, 90, "DoughnutHoleSize");
				doughnutHoleSize = value;
			}
		}
		public bool HasShadow { get; set; }
		public bool ShowLeaderLines { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FirstSliceAngle = reader.ReadUInt16();
			DoughnutHoleSize = reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			HasShadow = Convert.ToBoolean(bitwiseField & 0x0001);
			ShowLeaderLines = Convert.ToBoolean(bitwiseField & 0x0002);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.ViewType = DoughnutHoleSize != 0 ? ChartViewType.Doughnut : ChartViewType.Pie;
			chartBuilder.FirstSliceAngle = FirstSliceAngle;
			chartBuilder.DoughnutHoleSize = DoughnutHoleSize;
			chartBuilder.ShowLeaderLines = ShowLeaderLines;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)FirstSliceAngle);
			writer.Write((ushort)DoughnutHoleSize);
			ushort bitwiseField = 0;
			if(HasShadow)
				bitwiseField |= 0x0001;
			if(ShowLeaderLines)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 6;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartViewScatter
	public class XlsCommandChartViewScatter : XlsCommandBase {
		#region Fields
		int bubbleScale;
		#endregion
		#region Properties
		public int BubbleScale {
			get { return bubbleScale; }
			set {
				ValueChecker.CheckValue(value, 0, 300, "BubbleScale");
				bubbleScale = value;
			}
		}
		public SizeRepresentsType SizeRepresents { get; set; }
		public bool IsBubble { get; set; }
		public bool ShowNegBubbles { get; set; }
		public bool HasShadow { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			bubbleScale = reader.ReadUInt16();
			int bubbleSize = reader.ReadUInt16();
			SizeRepresents = bubbleSize == 1 ? SizeRepresentsType.Area : SizeRepresentsType.Width;
			ushort bitwiseField = reader.ReadUInt16();
			IsBubble = Convert.ToBoolean(bitwiseField & 0x0001);
			ShowNegBubbles = Convert.ToBoolean(bitwiseField & 0x0002);
			HasShadow = Convert.ToBoolean(bitwiseField & 0x0004);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.ViewType = IsBubble ? ChartViewType.Bubble : ChartViewType.Scatter;
			chartBuilder.BubbleScale = BubbleScale;
			chartBuilder.SizeRepresents = SizeRepresents;
			chartBuilder.ShowNegBubbles = ShowNegBubbles;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)bubbleScale);
			writer.Write((ushort)(SizeRepresents == SizeRepresentsType.Area ? 1 : 2));
			ushort bitwiseField = 0;
			if(IsBubble)
				bitwiseField |= 0x0001;
			if(ShowNegBubbles)
				bitwiseField |= 0x0002;
			if(HasShadow)
				bitwiseField |= 0x0004;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 6;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartViewRadar
	public abstract class XlsCommandChartViewRadarBase : XlsCommandBase {
		#region Properties
		public bool ShowCatLabels { get; set; }
		public bool HasShadow { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			ShowCatLabels = Convert.ToBoolean(bitwiseField & 0x0001);
			HasShadow = Convert.ToBoolean(bitwiseField & 0x0002);
			reader.ReadUInt16(); 
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.ViewType = ChartViewType.Radar;
			chartBuilder.RadarStyle = RadarStyle;
			chartBuilder.ShowCatLabels = ShowCatLabels;
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(ShowCatLabels)
				bitwiseField |= 0x0001;
			if(HasShadow)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
			writer.Write((ushort)0); 
		}
		protected override short GetSize() {
			return 4;
		}
		protected abstract RadarChartStyle RadarStyle { get; }
	}
	public class XlsCommandChartViewRadar : XlsCommandChartViewRadarBase {
		protected override RadarChartStyle RadarStyle {
			get { return RadarChartStyle.Standard; }
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	public class XlsCommandChartViewRadarArea : XlsCommandChartViewRadarBase {
		protected override RadarChartStyle RadarStyle {
			get { return RadarChartStyle.Filled; }
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartViewSurface
	public class XlsCommandChartViewSurface : XlsCommandBase {
		#region Properties
		public bool Wireframe { get; set; }
		public bool Shade3D { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort bitwiseField = reader.ReadUInt16();
			Wireframe = !Convert.ToBoolean(bitwiseField & 0x0001);
			Shade3D = Convert.ToBoolean(bitwiseField & 0x0002);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.ViewType = ChartViewType.Surface;
			chartBuilder.Wireframe = Wireframe;
			chartBuilder.Shade3D = Shade3D;
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(!Wireframe)
				bitwiseField |= 0x0001;
			if(Shade3D)
				bitwiseField |= 0x0002;
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
	#region XlsCommandChartViewOfPie
	public class XlsCommandChartViewOfPie : XlsCommandBase {
		#region Fields
		int secondPieSize = 75;
		int gapWidth = 150;
		#endregion
		#region Properties
		public ChartOfPieType OfPieType { get; set; }
		public OfPieSplitType SplitType { get; set; }
		public double SplitPos { get; set; }
		public int SecondPieSize {
			get { return secondPieSize; }
			set {
				ValueChecker.CheckValue(value, 5, 200, "SecondPieSize");
				secondPieSize = value;
			}
		}
		public int GapWidth {
			get { return gapWidth; }
			set {
				ValueChecker.CheckValue(value, 0, 500, "GapWidth");
				gapWidth = value;
			}
		}
		public bool HasShadow { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			byte flag = reader.ReadByte();
			OfPieType = flag == 0x01 ? ChartOfPieType.Pie : ChartOfPieType.Bar;
			flag = reader.ReadByte();
			int split = reader.ReadUInt16();
			int splitPos = reader.ReadInt16();
			int splitPercent = reader.ReadInt16();
			SecondPieSize = reader.ReadInt16();
			GapWidth = reader.ReadUInt16();
			double splitValue = reader.ReadDouble();
			HasShadow = (reader.ReadUInt16() & 0x0001) != 0;
			if (flag != 0) {
				SplitType = OfPieSplitType.Auto;
				SplitPos = 0.0;
			}
			else if (split == 0) {
				SplitType = OfPieSplitType.Position;
				SplitPos = splitPos;
			}
			else if (split == 1) {
				SplitType = OfPieSplitType.Value;
				SplitPos = splitValue;
			}
			else if (split == 2) {
				SplitType = OfPieSplitType.Percent;
				SplitPos = splitPercent;
			}
			else {
				SplitType = OfPieSplitType.Custom;
				SplitPos = 0.0;
			}
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.ViewType = ChartViewType.OfPie;
			chartBuilder.OfPieType = OfPieType;
			chartBuilder.SplitType = SplitType;
			chartBuilder.SplitPos = SplitPos;
			chartBuilder.SecondPieSize = SecondPieSize;
			chartBuilder.GapWidth = GapWidth;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((byte)(OfPieType == ChartOfPieType.Pie ? 1 : 2));
			if (SplitType == OfPieSplitType.Auto) {
				writer.Write((byte)1);
				writer.Write((short)0); 
				writer.Write((short)0); 
				writer.Write((short)0); 
				writer.Write((short)SecondPieSize);
				writer.Write((short)GapWidth);
				writer.Write((double)0.0);
			}
			else if (SplitType == OfPieSplitType.Position) {
				writer.Write((byte)0);
				writer.Write((short)0); 
				writer.Write((short)SplitPos); 
				writer.Write((short)0); 
				writer.Write((short)SecondPieSize);
				writer.Write((short)GapWidth);
				writer.Write((double)0.0);
			}
			else if (SplitType == OfPieSplitType.Value) {
				writer.Write((byte)0);
				writer.Write((short)1); 
				writer.Write((short)0); 
				writer.Write((short)0); 
				writer.Write((short)SecondPieSize);
				writer.Write((short)GapWidth);
				writer.Write(SplitPos);
			}
			else if (SplitType == OfPieSplitType.Percent) {
				writer.Write((byte)0);
				writer.Write((short)2); 
				writer.Write((short)0); 
				writer.Write((short)SplitPos); 
				writer.Write((short)SecondPieSize);
				writer.Write((short)GapWidth);
				writer.Write(0.0);
			}
			else {
				writer.Write((byte)0);
				writer.Write((short)3); 
				writer.Write((short)0); 
				writer.Write((short)0); 
				writer.Write((short)SecondPieSize);
				writer.Write((short)GapWidth);
				writer.Write((double)0.0);
			}
			writer.Write((ushort)(HasShadow ? 1 : 0));
		}
		protected override short GetSize() {
			return 22;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartViewOfPieCustom
	public class XlsCommandChartViewOfPieCustom : XlsCommandBase {
		#region Fields
		int numberOfPoints;
		List<int> secondPiePoints = new List<int>();
		#endregion
		#region Properties
		public int NumberOfPoints {
			get { return numberOfPoints; }
			set {
				ValueChecker.CheckValue(value, 0, 31999, "NumberOfPoints");
				numberOfPoints = value;
			}
		}
		public List<int> SecondPiePoints { get { return secondPiePoints; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int cxi = reader.ReadUInt16();
			numberOfPoints = cxi - 1;
			byte[] bits = reader.ReadBytes(cxi / 8 + 1);
			int padding = bits.Length * 8 - cxi;
			for (int i = 0; i < numberOfPoints; i++) {
				int offset = i + padding;
				byte mask = (byte)(0x80 >> (offset % 8));
				if ((bits[offset / 8] & mask) != 0)
					SecondPiePoints.Add(i);
			}
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.SecondPiePoints.Clear();
			chartBuilder.SecondPiePoints.AddRange(SecondPiePoints);
		}
		protected override void WriteCore(BinaryWriter writer) {
			int cxi = numberOfPoints + 1;
			writer.Write((ushort)cxi);
			byte[] bits = new byte[cxi / 8 + 1];
			int padding = bits.Length * 8 - cxi;
			Array.Clear(bits, 0, bits.Length);
			int count = SecondPiePoints.Count;
			if (count == 0)
				bits[bits.Length - 1] = 0x01;
			else {
				for (int i = 0; i < count; i++) {
					int offset = SecondPiePoints[i] + padding;
					byte mask = (byte)(0x80 >> (offset % 8));
					bits[offset / 8] |= mask;
				}
			}
			writer.Write(bits);
		}
		protected override short GetSize() {
			return (short)((numberOfPoints + 1) / 8 + 3);
		}
		public override IXlsCommand GetInstance() {
			secondPiePoints.Clear();
			return this;
		}
	}
	#endregion
	#region XlsCommandChartView3D
	public class XlsCommandChartView3D : XlsCommandBase {
		#region Fields
		int xRotation = 15;
		int yRotation = 20;
		int perspective = 30;
		int height;
		int depth = 100;
		int gapDepth = 150;
		#endregion
		#region Properties
		public int XRotation {
			get { return xRotation; }
			set {
				ValueChecker.CheckValue(value, -90, 90, "XRotation");
				xRotation = value;
			}
		}
		public int YRotation {
			get { return yRotation; }
			set {
				ValueChecker.CheckValue(value, 0, 360, "YRotation");
				yRotation = value;
			}
		}
		public int Perspective {
			get { return perspective; }
			set {
				ValueChecker.CheckValue(value, 0, 200, "Perspective");
				perspective = value;
			}
		}
		public int Height {
			get { return height; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "Height");
				height = value;
			}
		}
		public int Depth {
			get { return depth; }
			set {
				ValueChecker.CheckValue(value, 1, 2000, "Depth");
				depth = value;
			}
		}
		public int GapDepth {
			get { return gapDepth; }
			set {
				ValueChecker.CheckValue(value, 0, 500, "GapDepth");
				gapDepth = value;
			}
		}
		public bool HasPerspective { get; set; }
		public bool Clustered { get; set; }
		public bool AutoHeight { get; set; }
		public bool NotPieChart { get; set; }
		public bool Walls2D { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			yRotation = reader.ReadInt16();
			xRotation = reader.ReadInt16();
			perspective = reader.ReadInt16();
			height = reader.ReadInt16();
			depth = reader.ReadInt16();
			gapDepth = reader.ReadInt16();
			ushort bitwiseField = reader.ReadUInt16();
			HasPerspective = (bitwiseField & 0x0001) != 0;
			Clustered = (bitwiseField & 0x0002) != 0;
			AutoHeight = (bitwiseField & 0x0004) != 0;
			NotPieChart = (bitwiseField & 0x0010) != 0;
			Walls2D = (bitwiseField & 0x0020) != 0;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.ViewType = TranslateViewType(chartBuilder.ViewType);
			if (chartBuilder.ViewType == ChartViewType.Bar3D && chartBuilder.BarDirection == BarChartDirection.Column &&
				chartBuilder.BarGrouping == BarChartGrouping.Clustered && !Clustered)
				chartBuilder.BarGrouping = BarChartGrouping.Standard;
			if (chartBuilder.ViewType == ChartViewType.Line3D)
				chartBuilder.Grouping = ChartGrouping.Standard;
			chartBuilder.XRotation = XRotation;
			chartBuilder.YRotation = YRotation;
			chartBuilder.Perspective = Perspective;
			chartBuilder.Height = Height;
			chartBuilder.Depth = Depth;
			chartBuilder.GapDepth = GapDepth;
			chartBuilder.HasPerspective = HasPerspective;
			chartBuilder.AutoHeight = AutoHeight;
			chartBuilder.Walls2D = Walls2D;
			chartBuilder.NotPieChart = NotPieChart;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((short)yRotation);
			writer.Write((short)xRotation);
			writer.Write((short)perspective);
			writer.Write((short)height);
			writer.Write((short)depth);
			writer.Write((short)gapDepth);
			ushort bitwiseField = 0;
			if(HasPerspective)
				bitwiseField |= 0x0001;
			if(Clustered)
				bitwiseField |= 0x0002;
			if(AutoHeight)
				bitwiseField |= 0x0004;
			if(NotPieChart)
				bitwiseField |= 0x0010;
			if(Walls2D)
				bitwiseField |= 0x0020;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 14;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		ChartViewType TranslateViewType(ChartViewType viewType) {
			switch(viewType) {
				case ChartViewType.Area: return ChartViewType.Area3D;
				case ChartViewType.Bar: return ChartViewType.Bar3D;
				case ChartViewType.Line: return ChartViewType.Line3D;
				case ChartViewType.Pie: return ChartViewType.Pie3D;
				case ChartViewType.Surface: return ChartViewType.Surface3D;
			}
			return viewType;
		}
	}
	#endregion
	#region XlsCommandCrtLink
	public class XlsCommandCrtLink : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			reader.ReadBytes(Size);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((short)0); 
		}
		protected override short GetSize() {
			return 10;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartCrtLine
	public class XlsCommandChartCrtLine : XlsCommandShortPropertyValueBase {
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartViewBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartViewBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.LineFormatIndex = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
