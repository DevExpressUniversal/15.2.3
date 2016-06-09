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
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.Office.Services;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Utils.Zip;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	#region XlsChartExporter
	public partial class XlsChartExporter {
		void WriteSeriesDataFormat(SeriesBase series) {
			if (series == null)
				return;
			WriteDataFormat(XlsChartDefs.PointIndexOfSeries, seriesIndex, series.Index);
			WriteBegin();
			Write3DBarShape(series as BarSeries);
			ResetCrc32();
			int maxSeriesIndex = GetMaxSeriesIndex();
			ChartElement element = GetChartElement(series);
			WriteLineFormat(series.ShapeProperties, element, series.Index, maxSeriesIndex);
			WriteAreaFormat(series.ShapeProperties, element, series.Index, maxSeriesIndex);
			int dataCrcValue = Crc32.CrcValue;
			WritePieFormat(series as PieSeries);
			WriteSeriesFormat(series as SeriesWithMarkerAndSmooth);
			WriteSeriesFormat(series as BubbleSeries);
			ISeriesWithMarker seriesWithMarker = series as ISeriesWithMarker;
			ResetCrc32();
			WriteMarkerFormat(seriesWithMarker, series.Index, maxSeriesIndex);
			int markerCrcValue = Crc32.CrcValue;
			WriteAttachedLabel(series as SeriesWithDataLabelsAndPoints, WriteDataFormatAttachedLabel);
			WriteDataFormatStartBlock(XlsChartDefs.PointIndexOfSeries);
			Crc32.CrcValue = dataCrcValue;
			WriteShapeFormat(series.ShapeProperties, XlsChartDefs.DataContext);
			if (seriesWithMarker != null && seriesWithMarker.Marker.Symbol != MarkerStyle.None) {
				Crc32.CrcValue = markerCrcValue;
				WriteShapeFormat(seriesWithMarker.Marker.ShapeProperties, XlsChartDefs.MarkerContext);
			}
			WriteDataFormatEndBlock();
			WriteEnd();
		}
		ChartElement GetChartElement(ISeries series) {
			if (series.SeriesType == ChartSeriesType.Radar)
				return ChartElement.LinesForDataPoints;
			if (series.SeriesType == ChartSeriesType.Scatter)
				return ChartElement.LinesForDataPoints;
			if (series.SeriesType == ChartSeriesType.Surface)
				return ChartElement.LinesForDataPoints;
			if (series.SeriesType == ChartSeriesType.Line && !series.View.Is3DView)
				return ChartElement.LinesForDataPoints;
			return Chart.Is3DChart ? ChartElement.FillsForDataPoints3D : ChartElement.FillsForDataPoints2D;
		}
		int GetMaxSeriesIndex() {
			int result = 0;
			foreach (IChartView view in Chart.Views) {
				foreach (ISeries series in view.Series)
					result = Math.Max(result, series.Index);
			}
			return result;
		}
		#region DataPoints
		void WriteDataPoints(SeriesWithDataLabelsAndPoints series) {
			if (series == null)
				return;
			ChartElement element = GetChartElement(series);
			if (IsVaryColor(series)) {
				int count = (int)series.Values.ValuesCount;
				int maxPointIndex = count - 1;
				for (int i = 0; i < count; i++) {
					DataPoint dataPoint = series.DataPoints.FindByIndex(i);
					WriteDataPointFormat(series, dataPoint, element, i, maxPointIndex);
				}
			}
			else {
				int maxSeriesIndex = GetMaxSeriesIndex();
				foreach (DataPoint dataPoint in series.DataPoints)
					WriteDataPointFormat(series, dataPoint, element, series.Index, maxSeriesIndex);
			}
		}
		void WriteDataPointFormat(SeriesWithDataLabelsAndPoints series, DataPoint dataPoint, ChartElement element, int index, int maxIndex) {
			ShapeProperties shapeProperties = GetShapeProperties(series, dataPoint);
			ISeriesWithMarker seriesWithMarker = series as ISeriesWithMarker;
			Marker marker = GetMarker(seriesWithMarker, dataPoint);
			WriteDataFormat(dataPoint != null ? dataPoint.Index : index, seriesIndex, series.Index);
			WriteBegin();
			Write3DBarShape(series as BarSeries);
			ResetCrc32();
			WriteLineFormat(shapeProperties, element, index, maxIndex);
			WriteAreaFormat(shapeProperties, element, index, maxIndex);
			int dataCrcValue = Crc32.CrcValue;
			WritePieFormat(GetExplosion(series as PieSeries, dataPoint));
			ResetCrc32();
			WriteMarkerFormat(marker, index, maxIndex);
			int markerCrcValue = Crc32.CrcValue;
			Crc32.CrcValue = dataCrcValue;
			WriteShapeFormat(shapeProperties, XlsChartDefs.DataContext, dataPoint == null);
			if (marker != null && (marker.Symbol != MarkerStyle.None)) {
				Crc32.CrcValue = markerCrcValue;
				WriteShapeFormat(marker.ShapeProperties, XlsChartDefs.MarkerContext);
			}
			if (marker != null && IsVaryColor(series)) {
				XlsChartExtProperties extProperties = new XlsChartExtProperties();
				extProperties.Parent = XlsChartExtPropParent.DataFormat;
				XlsChartExtPropSymbol prop = new XlsChartExtPropSymbol();
				prop.Value = MarkerStyleToCode(marker.Symbol);
				extProperties.Items.Add(prop);
				WriteExtProperties(extProperties);
			}
			WriteEnd();
		}
		ShapeProperties GetShapeProperties(SeriesBase series, DataPoint dataPoint) {
			if (dataPoint != null)
				return dataPoint.ShapeProperties;
			return series.ShapeProperties;
		}
		Marker GetMarker(ISeriesWithMarker series, DataPoint dataPoint) {
			if (series == null)
				return null;
			if (dataPoint != null)
				return dataPoint.Marker;
			return series.Marker;
		}
		int GetExplosion(PieSeries series, DataPoint dataPoint) {
			if (series == null)
				return 0;
			if (dataPoint != null)
				return dataPoint.HasExplosion ? dataPoint.Explosion : 0;
			return series.Explosion;
		}
		bool IsVaryColor(ISeries series) {
			bool varyColors = false;
			ChartViewWithVaryColors view = series.View as ChartViewWithVaryColors;
			if (view != null)
				varyColors = view.VaryColors;
			return varyColors;
		}
		int MarkerStyleToCode(MarkerStyle symbol) {
			switch (symbol) {
				case MarkerStyle.None: return 0x0023;
				case MarkerStyle.Diamond: return 0x0024;
				case MarkerStyle.Square: return 0x0025;
				case MarkerStyle.Triangle: return 0x0026;
				case MarkerStyle.X: return 0x0027;
				case MarkerStyle.Star: return 0x0028;
				case MarkerStyle.Dot: return 0x0029;
				case MarkerStyle.Dash: return 0x002a;
				case MarkerStyle.Circle: return 0x002b;
				case MarkerStyle.Plus: return 0x002c;
			}
			return 0x002d;
		}
		#endregion
		#region DataFormat
		void WriteDataFormat(int pointIndex, int seriesIndex, int seriesOrder) {
			XlsCommandChartDataFormat command = new XlsCommandChartDataFormat();
			command.PointIndex = pointIndex;
			command.SeriesIndex = seriesIndex;
			command.SeriesOrder = seriesOrder;
			command.Write(StreamWriter);
		}
		void Write3DBarShape(BarSeries series) {
			if (series == null)
				return;
			Bar3DChartView view = series.View as Bar3DChartView;
			if (view == null)
				return;
			XlsCommandChart3DBarShape command = new XlsCommandChart3DBarShape();
			command.Shape = series.Shape == BarShape.Auto ? view.Shape : series.Shape;
			command.Write(StreamWriter);
		}
		void WritePieFormat(PieSeries series) {
			WritePieFormat(series != null ? series.Explosion : 0);
		}
		void WritePieFormat(int explosion) {
			XlsCommandChartPieExplosion command = new XlsCommandChartPieExplosion();
			command.Value = (short)explosion;
			command.Write(StreamWriter);
		}
		void WriteSeriesFormat(SeriesWithMarkerAndSmooth series) {
			if (series == null)
				return;
			XlsCommandChartSeriesFormat command = new XlsCommandChartSeriesFormat();
			command.SmoothLine = series.Smooth;
			command.Bubbles3D = false;
			command.Write(StreamWriter);
		}
		void WriteSeriesFormat(BubbleSeries series) {
			if (series == null)
				return;
			XlsCommandChartSeriesFormat command = new XlsCommandChartSeriesFormat();
			command.SmoothLine = false;
			command.Bubbles3D = series.Bubble3D;
			command.Write(StreamWriter);
		}
		void WriteMarkerFormat(ISeriesWithMarker series, int seriesIndex, int maxSeriesIndex) {
			if (series == null)
				return;
			WriteMarkerFormat(series.Marker, seriesIndex, maxSeriesIndex);
		}
		void WriteMarkerFormat(Marker marker, int seriesIndex, int maxSeriesIndex) {
			if (marker == null)
				return;
			XlsCommandChartMarkerFormat command = new XlsCommandChartMarkerFormat();
			command.Marker = marker.Symbol;
			command.MarkerSize = marker.Size * 20;
			command.ShowBorder = marker.ShapeProperties.Outline.Fill.FillType != DrawingFillType.None;
			command.ShowInterior = marker.ShapeProperties.Fill.FillType != DrawingFillType.None;
			XlsChartMarkerFormatExportHelper helper = new XlsChartMarkerFormatExportHelper(this);
			helper.Element = ChartElement.MarkersForDataPoints;
			helper.PointIndex = seriesIndex;
			helper.MaxPointIndex = maxSeriesIndex;
			helper.Prepare(marker.ShapeProperties.Outline.Fill);
			command.ForegroundColor = helper.ColorValue;
			command.ForegroundColorIndex = helper.ColorIndex;
			helper.Prepare(marker.ShapeProperties.Fill);
			command.BackgroundColor = helper.ColorValue;
			command.BackgroundColorIndex = helper.ColorIndex;
			command.CalcSpCheckSum(crc32);
			command.Write(StreamWriter);
		}
		#endregion
		#region Frame
		protected XlsCommandChartFrame WriteFrame(bool autoSize, bool autoPosition) {
			XlsCommandChartFrame command = PrepareFrameCommand(autoSize, autoPosition);
			command.Write(StreamWriter);
			return command;
		}
		protected XlsCommandChartFrame PrepareFrameCommand(bool autoSize, bool autoPosition) {
			XlsCommandChartFrame command = new XlsCommandChartFrame();
			command.FrameType = XlsChartFrameType.Frame;
			command.AutoPosition = autoPosition;
			command.AutoSize = autoSize;
			return command;
		}
		protected void WriteFrame(LayoutOptions layout) {
			XlsCommandChartFrame command = new XlsCommandChartFrame();
			command.FrameType = XlsChartFrameType.Frame;
			command.AutoPosition = layout.Auto;
			command.AutoSize = layout.Auto;
			command.Write(StreamWriter);
		}
		void WriteFrame(ShapeProperties shapeProperties, ChartElement element) {
			WriteFrame(true, true);
			WriteBegin();
			ResetCrc32();
			WriteLineFormat(shapeProperties, element);
			WriteAreaFormat(shapeProperties, element);
			WriteShapeFormat(shapeProperties, XlsChartDefs.DataContext);
			WriteEnd();
		}
		void WriteWrappedFrame(ShapeProperties shapeProperties, ChartElement element) {
			WriteFrtWrapper(PrepareFrameCommand(true, true));
			WriteFrtWrapper(commandBegin);
			ResetCrc32();
			XlsCommandChartLineFormat lineFormat = PrepareLineFormat(shapeProperties, element);
			lineFormat.CalcSpCheckSum(Crc32);
			WriteFrtWrapper(lineFormat);
			XlsCommandChartAreaFormat areaFormat = PrepareAreaFormat(shapeProperties, element);
			areaFormat.CalcSpCheckSum(Crc32);
			WriteFrtWrapper(areaFormat);
			WriteShapeFormat(shapeProperties, XlsChartDefs.DataContext);
			WriteFrtWrapper(commandEnd);
		}
		#endregion
		#region LineFormat
		protected void WriteLineFormat(ShapeProperties shapeProperties, ChartElement element) {
			XlsChartLineFormatExportHelper helper = new XlsChartLineFormatExportHelper(this, shapeProperties);
			helper.Element = element;
			helper.WriteContent();
		}
		protected void WriteLineFormat(ShapeProperties shapeProperties, ChartElement element, int pointIndex, int maxPointIndex) {
			XlsChartLineFormatExportHelper helper = new XlsChartLineFormatExportHelper(this, shapeProperties);
			helper.Element = element;
			helper.PointIndex = pointIndex;
			helper.MaxPointIndex = maxPointIndex;
			helper.WriteContent();
		}
		protected XlsCommandChartLineFormat PrepareLineFormat(ShapeProperties shapeProperties, ChartElement element) {
			XlsChartLineFormatExportHelper helper = new XlsChartLineFormatExportHelper(this, shapeProperties);
			helper.Element = element;
			return helper.PrepareCommand();
		}
		#endregion
		#region AreaFormat
		protected void WriteAreaFormat(ShapeProperties shapeProperties, ChartElement element) {
			XlsChartAreaFormatExportHelper helper = new XlsChartAreaFormatExportHelper(this, shapeProperties);
			helper.Element = element;
			helper.WriteContent();
		}
		protected void WriteAreaFormat(ShapeProperties shapeProperties, ChartElement element, int pointIndex, int maxPointIndex) {
			XlsChartAreaFormatExportHelper helper = new XlsChartAreaFormatExportHelper(this, shapeProperties);
			helper.Element = element;
			helper.PointIndex = pointIndex;
			helper.MaxPointIndex = maxPointIndex;
			helper.WriteContent();
		}
		protected XlsCommandChartAreaFormat PrepareAreaFormat(ShapeProperties shapeProperties, ChartElement element) {
			XlsChartAreaFormatExportHelper helper = new XlsChartAreaFormatExportHelper(this, shapeProperties);
			helper.Element = element;
			return helper.PrepareCommand();
		}
		#endregion
		#region ShapePropStream
		protected void WriteShapeFormat(ShapeProperties shapeProperties, int objContext) {
			WriteShapeFormat(shapeProperties, objContext, false);
		}
		protected void WriteShapeFormat(ShapeProperties shapeProperties, int objContext, bool writeAutomatic) {
			XlsChartShapeFormat format = new XlsChartShapeFormat();
			format.CheckSum = crc32.CrcValue;
			format.ObjContext = objContext;
			format.Content = GenerateShapePropertiesContent(shapeProperties, writeAutomatic);
			XlsCommandChartShapeProperties firstCommand = new XlsCommandChartShapeProperties();
			XlsCommandContinueFrt12 continueCommand = new XlsCommandContinueFrt12();
			using (XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
				FutureRecordHeader header = new FutureRecordHeader();
				header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(firstCommand.GetType());
				header.Write(writer);
				format.Write(writer);
			}
		}
		byte[] GenerateShapePropertiesContent(ShapeProperties shapeProperties, bool writeAutomatic) {
			if (!shapeProperties.IsAutomatic || writeAutomatic) {
				using (MemoryStream outputStream = new MemoryStream()) {
					XlsChartShapePropStreamExporter exporter = new XlsChartShapePropStreamExporter(shapeProperties);
					exporter.WriteAutomatic = writeAutomatic;
					exporter.Export(outputStream);
					return outputStream.ToArray();
				}
			}
			return null;
		}
		#endregion
	}
	#endregion
	#region XlsChartFormatExporterBase
	public abstract class XlsChartFormatExporterBase {
		#region Fields
		XlsChartExporter exporter;
		#endregion
		protected XlsChartFormatExporterBase(XlsChartExporter exporter) {
			this.exporter = exporter;
		}
		#region Properties
		public XlsChartExporter Exporter { get { return exporter; } }
		public Chart Chart { get { return exporter.Chart; } }
		public DocumentModel DocumentModel { get { return exporter.Chart.DocumentModel; } }
		public ChartElement Element { get; set; }
		public int PointIndex { get; set; }
		public int MaxPointIndex { get; set; }
		#endregion
	}
	#endregion
	#region XlsChartLineFormatExportHelper
	public class XlsChartLineFormatExportHelper : XlsChartFormatExporterBase, IDrawingFillVisitor {
		#region Fields
		Outline shapeOutline;
		XlsCommandChartLineFormat command;
		#endregion
		public XlsChartLineFormatExportHelper(XlsChartExporter exporter, ShapeProperties shapeProperties) 
			: base(exporter) {
			this.shapeOutline = shapeProperties.Outline;
		}
		public void WriteContent() {
			PrepareCommand();
			command.CalcSpCheckSum(Exporter.Crc32);
			command.Write(Exporter.StreamWriter);
		}
		public XlsCommandChartLineFormat PrepareCommand() {
			command = new XlsCommandChartLineFormat();
			shapeOutline.Fill.Visit((IDrawingFillVisitor)this);
			return command;
		}
		public void WriteContent(bool delete) {
			PrepareCommand();
			command.Visible = !delete;
			if (delete)
				PrepareNoFill(DocumentModel.StyleSheet.Palette);
			command.CalcSpCheckSum(Exporter.Crc32);
			command.Write(Exporter.StreamWriter);
		}
		#region IDrawingFillVisitor Members
		void IDrawingFillVisitor.Visit(DrawingFill fill) {
			if (fill.FillType == DrawingFillType.Automatic)
				PrepareAutomatic();
			else if (fill.FillType == DrawingFillType.None)
				PrepareNoFill(DocumentModel.StyleSheet.Palette);
		}
		void IDrawingFillVisitor.Visit(DrawingSolidFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			command.LineStyle = GetLineStyle(shapeOutline.Dashing);
			command.Thickness = CalcThickness(shapeOutline.Width);
			command.LineColorIndex = palette.GetPaletteNearestColorIndex(fill.Color.FinalColor);
			command.LineColor = palette[command.LineColorIndex];
		}
		void IDrawingFillVisitor.Visit(DrawingPatternFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			command.LineStyle = GetLineStyle(fill.PatternType);
			command.Thickness = CalcThickness(shapeOutline.Width);
			command.LineColorIndex = palette.GetPaletteNearestColorIndex(fill.ForegroundColor.FinalColor);
			command.LineColor = palette[command.LineColorIndex];
		}
		void IDrawingFillVisitor.Visit(DrawingGradientFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			command.LineStyle = GetLineStyle(shapeOutline.Dashing);
			command.Thickness = CalcThickness(shapeOutline.Width);
			command.LineColorIndex = palette.GetPaletteNearestColorIndex(fill.GradientStops[0].Color.FinalColor);
			command.LineColor = palette[command.LineColorIndex];
		}
		void IDrawingFillVisitor.Visit(DrawingBlipFill fill) {
		}
		#endregion
		#region Automatic and NoFill
		void PrepareAutomatic() {
			Palette palette = DocumentModel.StyleSheet.Palette;
			DrawingFillType fillType = DrawingFillType.Automatic;
			IChartAppearance appearance = Chart as IChartAppearance;
			if (appearance != null)
				fillType = appearance.GetOutlineFillType(Element, PointIndex, MaxPointIndex);
			if (fillType == DrawingFillType.Automatic) {
				command.AutoColor = true;
				command.LineStyle = XlsChartLineStyle.Solid;
				command.Thickness = XlsChartLineThickness.Narrow;
				command.LineColorIndex = Palette.DefaultChartForegroundColorIndex;
				command.LineColor = palette.DefaultChartForegroundColor;
			}
			else if (fillType == DrawingFillType.None) {
				PrepareNoFill(palette);
			}
			else if (fillType == DrawingFillType.Solid || fillType == DrawingFillType.Gradient) {
				command.LineStyle = GetLineStyle(appearance.GetOutlineDashing(Element, PointIndex, MaxPointIndex));
				command.Thickness = CalcThickness(appearance.GetOutlineWidth(Element, PointIndex, MaxPointIndex));
				command.LineColorIndex = palette.GetPaletteNearestColorIndex(appearance.GetOutlineColor(Element, PointIndex, MaxPointIndex));
				command.LineColor = palette[command.LineColorIndex];
			}
			else if (fillType == DrawingFillType.Pattern) {
				command.LineStyle = GetLineStyle(appearance.GetOutlineFillPatternType(Element, PointIndex, MaxPointIndex));
				command.Thickness = CalcThickness(appearance.GetOutlineWidth(Element, PointIndex, MaxPointIndex));
				command.LineColorIndex = palette.GetPaletteNearestColorIndex(appearance.GetOutlineColor(Element, PointIndex, MaxPointIndex));
				command.LineColor = palette[command.LineColorIndex];
			}
		}
		void PrepareNoFill(Palette palette) {
			command.AutoColor = true;
			command.LineStyle = XlsChartLineStyle.None;
			command.Thickness = XlsChartLineThickness.Hairline;
			command.LineColorIndex = Palette.DefaultChartForegroundColorIndex;
			command.LineColor = palette.DefaultChartForegroundColor;
		}
		#endregion
		#region Utils
		XlsChartLineThickness CalcThickness(int width) {
			float widthInPoints = DocumentModel.UnitConverter.ModelUnitsToPointsF(width);
			if (widthInPoints >= 2.5)
				return XlsChartLineThickness.Wide;
			if (widthInPoints >= 1.5)
				return XlsChartLineThickness.Medium;
			if (widthInPoints >= 0.5)
				return XlsChartLineThickness.Narrow;
			return XlsChartLineThickness.Hairline;
		}
		XlsChartLineStyle GetLineStyle(OutlineDashing dashing) {
			switch (dashing) {
				case OutlineDashing.SystemDot:
				case OutlineDashing.Dot:
					return XlsChartLineStyle.Dot;
				case OutlineDashing.SystemDash:
				case OutlineDashing.Dash:
					return XlsChartLineStyle.Dash;
				case OutlineDashing.SystemDashDot:
				case OutlineDashing.LongDashDot:
					return XlsChartLineStyle.DashDot;
				case OutlineDashing.SystemDashDotDot:
				case OutlineDashing.LongDashDotDot:
					return XlsChartLineStyle.DashDotDot;
			}
			return XlsChartLineStyle.Solid;
		}
		XlsChartLineStyle GetLineStyle(DrawingPatternType patternType) {
			switch (patternType) {
				case DrawingPatternType.Percent5:
				case DrawingPatternType.Percent10:
				case DrawingPatternType.Percent20:
				case DrawingPatternType.Percent25:
					return XlsChartLineStyle.LightGray;
				case DrawingPatternType.Percent75:
				case DrawingPatternType.Percent80:
				case DrawingPatternType.Percent90:
					return XlsChartLineStyle.LightGray;
			}
			return XlsChartLineStyle.MediumGray;
		}
		#endregion
	}
	#endregion
	#region XlsChartAreaFormatExportHelper
	public class XlsChartAreaFormatExportHelper : XlsChartFormatExporterBase, IDrawingFillVisitor {
		#region Fields
		IDrawingFill shapeFill;
		XlsCommandChartAreaFormat command;
		XlsChartGraphicFormat gelFrame;
		#endregion
		public XlsChartAreaFormatExportHelper(XlsChartExporter exporter, ShapeProperties shapeProperties) 
			: base(exporter) {
			this.shapeFill = shapeProperties.Fill;
		}
		public void WriteContent() {
			PrepareCommand();
			if (!gelFrame.Apply)
				command.CalcSpCheckSum(Exporter.Crc32);
			command.Write(Exporter.StreamWriter);
			if (gelFrame.Apply) {
				gelFrame.CalcSpCheckSum(Exporter.Crc32);
				XlsCommandChartGelFrame firstCommand = new XlsCommandChartGelFrame();
				XlsCommandContinue continueCommand = new XlsCommandContinue();
				using (XlsChunkWriter frameWriter = new XlsChunkWriter(Exporter.StreamWriter, firstCommand, continueCommand)) {
					gelFrame.ArtProperties.Write(frameWriter);
					gelFrame.ArtTertiaryProperties.Write(frameWriter);
				}
			}
		}
		public XlsCommandChartAreaFormat PrepareCommand() {
			command = new XlsCommandChartAreaFormat();
			gelFrame = new XlsChartGraphicFormat();
			shapeFill.Visit((IDrawingFillVisitor)this);
			return command;
		}
		#region IDrawingFillVisitor Members
		void IDrawingFillVisitor.Visit(DrawingFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			if (fill.FillType == DrawingFillType.Automatic)
				PrepareAutomatic();
			else if (fill.FillType == DrawingFillType.Group) {
				command.FillType = XlsChartFillType.Solid;
				command.ForegroundColorIndex = 9;
				command.ForegroundColor = palette[command.ForegroundColorIndex];
				command.BackgroundColorIndex = Palette.DefaultChartForegroundColorIndex;
				command.BackgroundColor = palette[command.BackgroundColorIndex];
			}
			else if (fill.FillType == DrawingFillType.None) {
				command.FillType = XlsChartFillType.None;
				command.ForegroundColorIndex = 9;
				command.ForegroundColor = palette[command.ForegroundColorIndex];
				command.BackgroundColorIndex = Palette.DefaultChartForegroundColorIndex;
				command.BackgroundColor = palette[command.BackgroundColorIndex];
			}
		}
		void IDrawingFillVisitor.Visit(DrawingSolidFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			command.FillType = XlsChartFillType.Solid;
			Color color = fill.Color.FinalColor;
			command.ForegroundColorIndex = palette.GetPaletteNearestColorIndex(color);
			command.ForegroundColor = palette[command.ForegroundColorIndex];
			command.BackgroundColorIndex = 9;
			command.BackgroundColor = palette[command.BackgroundColorIndex];
			if (!IsEqualColors(command.ForegroundColor, color))
				PrepareGelFrame(color);
		}
		void IDrawingFillVisitor.Visit(DrawingPatternFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			command.FillType = XlsChartFillType.Solid;
			command.ForegroundColorIndex = palette.GetPaletteNearestColorIndex(fill.ForegroundColor.FinalColor);
			command.ForegroundColor = palette[command.ForegroundColorIndex];
			command.BackgroundColorIndex = palette.GetPaletteNearestColorIndex(fill.BackgroundColor.FinalColor);
			command.BackgroundColor = palette[command.BackgroundColorIndex];
			PrepareGelFrame(fill.ForegroundColor.FinalColor);
		}
		void IDrawingFillVisitor.Visit(DrawingGradientFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			command.FillType = XlsChartFillType.Solid;
			command.ForegroundColorIndex = palette.GetPaletteNearestColorIndex(fill.GradientStops[0].Color.FinalColor);
			command.ForegroundColor = palette[command.ForegroundColorIndex];
			command.BackgroundColorIndex = Palette.DefaultChartForegroundColorIndex;
			command.BackgroundColor = palette[command.BackgroundColorIndex];
			PrepareGelFrame(fill.GradientStops[0].Color.FinalColor);
		}
		void IDrawingFillVisitor.Visit(DrawingBlipFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			command.FillType = XlsChartFillType.Solid;
			command.ForegroundColorIndex = 9;
			command.ForegroundColor = palette[command.ForegroundColorIndex];
			command.BackgroundColorIndex = Palette.DefaultChartForegroundColorIndex;
			command.BackgroundColor = palette[command.BackgroundColorIndex];
		}
		#endregion
		#region Automatic
		void PrepareAutomatic() {
			Palette palette = DocumentModel.StyleSheet.Palette;
			DrawingFillType fillType = DrawingFillType.Automatic;
			IChartAppearance appearance = Chart as IChartAppearance;
			if (appearance != null)
				fillType = appearance.GetFillType(Element, PointIndex, MaxPointIndex);
			if (fillType == DrawingFillType.Automatic) {
				command.AutoColor = true;
				command.FillType = XlsChartFillType.Solid;
				command.ForegroundColorIndex = 9;
				command.ForegroundColor = palette[command.ForegroundColorIndex];
				command.BackgroundColorIndex = Palette.DefaultChartForegroundColorIndex;
				command.BackgroundColor = palette[command.BackgroundColorIndex];
			}
			else if (fillType == DrawingFillType.None) {
				if (Element == ChartElement.LinesForDataPoints) {
					command.FillType = XlsChartFillType.Solid;
					command.ForegroundColorIndex = Palette.DefaultChartForegroundColorIndex;
					command.ForegroundColor = palette[command.ForegroundColorIndex];
					command.BackgroundColorIndex = Palette.DefaultChartForegroundColorIndex;
					command.BackgroundColor = palette[command.BackgroundColorIndex];
				}
				else {
					command.FillType = XlsChartFillType.None;
					command.ForegroundColorIndex = Palette.DefaultChartBackgroundColorIndex;
					command.ForegroundColor = palette[command.ForegroundColorIndex];
					command.BackgroundColorIndex = Palette.DefaultChartForegroundColorIndex;
					command.BackgroundColor = palette[command.BackgroundColorIndex];
				}
			}
			else if (fillType == DrawingFillType.Solid) {
				command.FillType = XlsChartFillType.Solid;
				Color color = appearance.GetFillForegroundColor(Element, PointIndex, MaxPointIndex);
				command.ForegroundColorIndex = palette.GetPaletteNearestColorIndex(color);
				command.ForegroundColor = palette[command.ForegroundColorIndex];
				command.BackgroundColorIndex = 9;
				command.BackgroundColor = palette[command.BackgroundColorIndex];
				if (!IsEqualColors(command.ForegroundColor, color))
					PrepareGelFrame(color);
			}
			else if (fillType == DrawingFillType.Pattern || fillType == DrawingFillType.Gradient) {
				command.FillType = XlsChartFillType.Solid; 
				Color color = appearance.GetFillForegroundColor(Element, PointIndex, MaxPointIndex);
				command.ForegroundColorIndex = palette.GetPaletteNearestColorIndex(color);
				command.ForegroundColor = palette[command.ForegroundColorIndex];
				command.BackgroundColorIndex = palette.GetPaletteNearestColorIndex(appearance.GetFillBackgroundColor(Element, PointIndex, MaxPointIndex));
				command.BackgroundColor = palette[command.BackgroundColorIndex];
				PrepareGelFrame(color);
			}
			else {
				command.FillType = XlsChartFillType.Solid;
				command.ForegroundColorIndex = 9;
				command.ForegroundColor = palette[command.ForegroundColorIndex];
				command.BackgroundColorIndex = Palette.DefaultChartForegroundColorIndex;
				command.BackgroundColor = palette[command.BackgroundColorIndex];
			}
		}
		#endregion
		#region GelFrame
		bool IsEqualColors(Color paletteColor, Color originalColor) {
			return originalColor.A == 0xff && 
				paletteColor.R == originalColor.R &&
				paletteColor.G == originalColor.G && 
				paletteColor.B == originalColor.B;
		}
		void PrepareGelFrame(Color foregroundColor) {
			gelFrame.Apply = true;
			PrepareArtProperties(foregroundColor);
			PrepareArtTertiaryProperties();
		}
		void PrepareArtProperties(Color foregroundColor) {
			List<IOfficeDrawingProperty> properties = gelFrame.ArtProperties.Properties;
			properties.Add(new OfficeDrawingFillType() { FillType = OfficeFillType.Solid });
			properties.Add(new DrawingFillColor(foregroundColor));
			properties.Add(new DrawingFillOpacity(Math.Round(foregroundColor.A / 255.0, 2)));
			properties.Add(new DrawingFillBackColor(DXColor.White));
			properties.Add(new DrawingFillBackOpacity(1.0));
			DrawingFillBWColor bwColor = new DrawingFillBWColor();
			bwColor.ColorRecord.SystemColorIndex = 0xf4;
			properties.Add(bwColor);
			properties.Add(new DrawingFillBlip());
			properties.Add(new DrawingFillBlipName());
			properties.Add(new DrawingFillBlipFlags());
			properties.Add(new DrawingFillWidth());
			properties.Add(new DrawingFillHeight());
			properties.Add(new DrawingFillAngle());
			properties.Add(new DrawingFillFocus());
			properties.Add(new DrawingFillToLeft());
			properties.Add(new DrawingFillToTop());
			properties.Add(new DrawingFillToRight());
			properties.Add(new DrawingFillToBottom());
			properties.Add(new DrawingFillRectLeft());
			properties.Add(new DrawingFillRectTop());
			properties.Add(new DrawingFillRectRight());
			properties.Add(new DrawingFillRectBottom());
			properties.Add(new DrawingFillDzType() { DzType = OfficeDzType.Default });
			properties.Add(new DrawingFillShadePreset());
			properties.Add(new DrawingFillShadeColors());
			properties.Add(new DrawingFillOriginX());
			properties.Add(new DrawingFillOriginY());
			properties.Add(new DrawingFillShapeOriginX());
			properties.Add(new DrawingFillShapeOriginY());
			properties.Add(new DrawingFillShadeType());
			DrawingFillStyleBooleanProperties fillBoolProps = new DrawingFillStyleBooleanProperties();
			fillBoolProps.FillShape = true;
			fillBoolProps.HitTestFill = true;
			fillBoolProps.UseNoFillHitTest = true;
			fillBoolProps.UseFillUseRect = true;
			fillBoolProps.UseFillShape = true;
			fillBoolProps.UseHitTestFill = true;
			properties.Add(fillBoolProps);
		}
		void PrepareArtTertiaryProperties() {
			List<IOfficeDrawingProperty> properties = gelFrame.ArtTertiaryProperties.Properties;
			DrawingFillColorExt colorExt = new DrawingFillColorExt();
			colorExt.ColorRecord = new OfficeColorRecord();
			properties.Add(colorExt);
			properties.Add(new DrawingFillReserved415());
			properties.Add(new DrawingFillTintShade() { Value = 0x20000000 });
			properties.Add(new DrawingFillReserved417());
			DrawingFillBackColorExt backColorExt = new DrawingFillBackColorExt();
			backColorExt.ColorRecord = new OfficeColorRecord();
			properties.Add(backColorExt);
			properties.Add(new DrawingFillReserved419());
			properties.Add(new DrawingFillBackTintShade() { Value = 0x20000000 });
			properties.Add(new DrawingFillReserved421());
			properties.Add(new DrawingFillReserved422());
			properties.Add(new DrawingFillReserved423());
			DrawingFillStyleBooleanProperties fillBoolProps = new DrawingFillStyleBooleanProperties();
			fillBoolProps.Value = 0x00600000;
			properties.Add(fillBoolProps);
		}
		#endregion
	}
	#endregion
	#region XlsChartMarkerFormatExportHelper
	public class XlsChartMarkerFormatExportHelper : XlsChartFormatExporterBase, IDrawingFillVisitor {
		public XlsChartMarkerFormatExportHelper(XlsChartExporter exporter)
			: base(exporter) {
		}
		#region Properties
		public Color ColorValue { get; set; }
		public int ColorIndex { get;set; }
		#endregion
		public void Prepare(IDrawingFill fill) {
			fill.Visit(this);
		}
		#region IDrawingFillVisitor Members
		void IDrawingFillVisitor.Visit(DrawingFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			if (fill.FillType == DrawingFillType.Automatic || fill.FillType == DrawingFillType.Group)
				PrepareAutomatic();
			else if (fill.FillType == DrawingFillType.None) {
				ColorIndex = 8;
				ColorValue = palette[ColorIndex];
			}
		}
		void IDrawingFillVisitor.Visit(DrawingSolidFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			ColorIndex = palette.GetPaletteNearestColorIndex(fill.Color.FinalColor);
			ColorValue = palette[ColorIndex];
		}
		void IDrawingFillVisitor.Visit(DrawingPatternFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			ColorIndex = palette.GetPaletteNearestColorIndex(fill.ForegroundColor.FinalColor);
			ColorValue = palette[ColorIndex];
		}
		void IDrawingFillVisitor.Visit(DrawingGradientFill fill) {
			Palette palette = DocumentModel.StyleSheet.Palette;
			ColorIndex = palette.GetPaletteNearestColorIndex(fill.GradientStops[0].Color.FinalColor);
			ColorValue = palette[ColorIndex];
		}
		void IDrawingFillVisitor.Visit(DrawingBlipFill fill) {
			PrepareAutomatic();
		}
		#endregion
		#region Automatic
		void PrepareAutomatic() {
			Palette palette = DocumentModel.StyleSheet.Palette;
			DrawingFillType fillType = DrawingFillType.Automatic;
			IChartAppearance appearance = Chart as IChartAppearance;
			if (appearance != null)
				fillType = appearance.GetFillType(Element, PointIndex, MaxPointIndex);
			if (fillType == DrawingFillType.Automatic || fillType == DrawingFillType.None) {
				ColorIndex = 8;
				ColorValue = palette[ColorIndex];
			}
			else {
				ColorIndex = palette.GetPaletteNearestColorIndex(appearance.GetFillForegroundColor(Element, PointIndex, MaxPointIndex));
				ColorValue = palette[ColorIndex];
			}
		}
		#endregion
	}
	#endregion
}
