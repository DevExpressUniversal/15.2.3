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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Charts.Native;
using DevExpress.Data.Browsing;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraCharts {
	public interface ISupportTransparency {
		byte Transparency { get; set; }
	}
	public interface ISupportStackedGroup {
		object StackedGroup { get; set; }
	}
}
namespace DevExpress.XtraCharts.Native {
	public enum ChartContainerType {
		WinControl,
		WebControl,
		XRControl
	}
	public enum RangeValidationBase {
		Minimum,
		Maximum,
		Average
	}
	public interface ITransparableView : ISupportTransparency {
		void AssignTransparency(ITransparableView view);
	}
	public interface IHitTest {
		object Object { get; }
		HitTestState State { get; }
	}
	public interface ISupportTextAntialiasing {
		bool Rotated { get; }
		bool DefaultAntialiasing { get; }
		DefaultBoolean EnableAntialiasing { get; }
		Color TextBackColor { get; }
		RectangleFillStyle TextBackFillStyle { get; }
		ChartElement BackElement { get; }
	}
	public interface ITextPropertiesProvider : IHitTest, ISupportTextAntialiasing {
		Font Font { get; }
		RectangleFillStyle FillStyle { get; }
		RectangularBorder Border { get; }
		Shadow Shadow { get; }
		StringAlignment Alignment { get; }
		bool ChangeSelectedBorder { get; }
		bool CorrectBoundsByBorder { get; }
		Color BackColor { get; }
		Color GetTextColor(Color color);
		Color GetBorderColor(Color color);
	}
	public interface IChartDataProvider {
		event BoundDataChangedEventHandler BoundDataChanged;
		bool CanUseBoundPoints { get; }
		object DataAdapter { get; set; }
		DataContext DataContext { get; }
		object DataSource { get; set; }
		bool SeriesDataSourceVisible { get; }
		object ParentDataSource { get; }
		void OnBoundDataChanged(EventArgs e);
		void OnPivotGridSeriesExcluded(PivotGridSeriesExcludedEventArgs e);
		void OnPivotGridSeriesPointsExcluded(PivotGridSeriesPointsExcludedEventArgs e);
		bool ShouldSerializeDataSource(object dataSource);
	}
	public interface IChartRenderProvider {
		Rectangle DisplayBounds { get; }
		bool IsPrintingAvailable { get; }
		object LookAndFeel { get; }
		IPrintable Printable { get; }
		ComponentExporter CreateComponentPrinter(IPrintable iPrintable);
		void Invalidate();
		void InvokeInvalidate();
		Bitmap LoadBitmap(string url);
	}
	public interface IChartEventsProvider {
		bool ShouldCustomDrawSeries { get; }
		bool ShouldCustomDrawAxisLabels { get; }
		bool ShouldCustomDrawSeriesPoints { get; }
		void OnCustomDrawAxisLabel(CustomDrawAxisLabelEventArgs e);
		void OnCustomDrawSeries(CustomDrawSeriesEventArgs e);
		void OnCustomDrawSeriesPoint(CustomDrawSeriesPointEventArgs e);
		void OnCustomPaint(CustomPaintEventArgs e);
		void OnCustomizeAutoBindingSettings(EventArgs e);
		void OnCustomizeSimpleDiagramLayout(CustomizeSimpleDiagramLayoutEventArgs e);
		void OnPivotChartingCustomizeLegend(CustomizeLegendEventArgs e);
		void OnPivotChartingCustomizeResolveOverlappingMode(CustomizeResolveOverlappingModeEventArgs e);
		void OnPivotChartingCustomizeXAxisLabels(CustomizeXAxisLabelsEventArgs e);
		void OnAxisScaleChanged(AxisScaleChangedEventArgs e);
		void OnAxisWholeRangeChanged(AxisRangeChangedEventArgs e);
		void OnAxisVisualRangeChanged(AxisRangeChangedEventArgs e);
	}
	public interface IChartInteractionProvider {
		event HotTrackEventHandler ObjectHotTracked;
		event HotTrackEventHandler ObjectSelected;
		event SelectedItemsChangedEventHandler SelectedItemsChanged;
		bool CanShowTooltips { get; }
		bool Capture { get; set; }
		bool DragCtrlKeyRequired { get; }
		bool EnableChartHitTesting { get; }
		bool HitTestingEnabled { get; }
		ElementSelectionMode SelectionMode { get; }
		SeriesSelectionMode SeriesSelectionMode { get; }
		void OnCustomDrawCrosshair(CustomDrawCrosshairEventArgs e);
		void OnLegendItemChecked(LegendItemCheckedEventArgs e);
		void OnObjectHotTracked(HotTrackEventArgs e);
		void OnObjectSelected(HotTrackEventArgs e);
		void OnSelectedItemsChanged(SelectedItemsChangedEventArgs e);
		void OnPieSeriesPointExploded(PieSeriesPointExplodedEventArgs e);
		void OnQueryCursor(QueryCursorEventArgs e);
		void OnScroll(ChartScrollEventArgs e);
		void OnZoom(ChartZoomEventArgs e);
		void OnScroll3D(ChartScroll3DEventArgs e);		
		void OnZoom3D(ChartZoom3DEventArgs e);
		Point PointToClient(Point p);
		Point PointToCanvas(Point p);
	}
	public interface ISupportBarsInteraction : IServiceProvider, ICommandAwareControl<ChartCommandId> {
		void RaiseUIUpdated();
	}
	public interface IChartContainer : ISupportBarsInteraction {
		event EventHandler EndLoading;
		Chart Chart { get; }
		ChartContainerType ControlType { get; }
		IChartDataProvider DataProvider { get; }
		IChartRenderProvider RenderProvider { get; }
		IChartEventsProvider EventsProvider { get; }
		IChartInteractionProvider InteractionProvider { get; }
		bool DesignMode { get; }
		bool Loading { get; }
		bool ShowDesignerHints { get; }
		bool IsDesignControl { get; }
		bool IsEndUserDesigner { get; }
		bool ShouldEnableFormsSkins { get; }
		bool CanDisposeItems { get; }
		IServiceProvider ServiceProvider { get; }
		ISite Site { get; set; }
		IComponent Parent { get; }
		void Assign(Chart chart);
		void Changing();
		void Changed();
		void LockChangeService();
		void UnlockChangeService();
		void ShowErrorMessage(string message, string title);
		void RaiseRangeControlRangeChanged(object minValue, object maxValue, bool invalidate);
		bool GetActualRightToLeft();
	}
	public interface IPolygon {
		GraphicsPath GetPath();
		RectangleF Rect { get; set; }
		LineStrip Vertices { get; }
		void RenderShadow(IRenderer renderer, Shadow shadow, int shadowSize);
		void Render(IRenderer renderer, FillOptionsBase fillOptions, Color color, Color color2, Color borderColor, int borderThickness);
	}
	public interface IGradientPainter {
		void FillPolygon(Graphics gr, IPolygon polygon, Color color, Color color2);
		PlanePolygon[] FillPlanePolygon(PlanePolygon polygon, PlaneRectangle gradientRect, Color color, Color color2);
		#region Graphics commands
		GraphicsCommand CreateRectangleGraphicsCommand(ZPlaneRectangle rect, ZPlaneRectangle gradientRect, Color color, Color color2);
		GraphicsCommand CreateGraphicsCommand(LineStrip vertices, ZPlaneRectangle boundRectangle, Color color, Color color2);
		GraphicsCommand CreatePieGraphicsCommand(GRealPoint2D center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2);
		#endregion
		#region Direct rendering
		void RenderRectangle(RectangleF rect, RectangleF gradientRect, Color color, Color color2);
		void RenderStrip(LineStrip vertices, RectangleF boundRectangle, Color color, Color color2);
		void RenderBezier(BezierRangeStrip strip, Color color, Color color2);
		void RenderCircle(PointF center, float radius, Color color, Color color2);
		void RenderEllipse(PointF center, float semiAxisX, float semiAxisY, Color color, Color color2);
		void RenderPie(PointF center, float majorSemiAxis, float minorSemiAxis, float startAngle, float sweepAngle, float depth, float holePercent, RectangleF gradientBounds, Color color, Color color2);
		void RenderPath(GraphicsPath path, RectangleF gradientRect, Color color, Color color2);
		#endregion
	}
	public interface IXYDiagramMapping {		
		DiagramPoint GetScreenPoint(double argument, double value);
	}
	public interface ISimpleDiagramDomain {
		Rectangle Bounds { get; }
		Rectangle LabelsBounds { get; }
		Rectangle ElementBounds { get; }
	}
	public interface ISimpleDiagram3DSeriesView {
		double DepthFactor { get; }
		double HeightToWidthRatio { get; }
	}
	public interface IEllipse {
		GRealPoint2D Center { get; }
		GRealPoint2D CalcEllipsePoint(double angle);
	}
	public interface IColorEachSupportView {
		bool ColorEach { get; set; }
	}
	public interface IShadowSupportView {
		Shadow Shadow { get; }
	}
	public interface IPointSeriesView {
		MarkerBase Marker { get; }
	}
	public interface ILineSeriesView {
		LineStyle LineStyle { get; }
		bool MarkerVisible { get; }
	}
	public interface IAreaSeriesView : IGeometryStripCreator, ITransparableView {
		CustomBorder Border { get; }
		PolygonFillStyle FillStyle { get; }
		PolygonFillStyle ActualFillStyle { get; }
		bool Rotated { get; }
		bool Closed { get; }
		Marker MarkerOptions { get; }
		AreaSeriesViewAppearance Appearance { get; }
		bool GetActualAntialiasing(int pointsCount);
	}
	public interface ISimpleDiagram2DSeriesView {
		PolygonFillStyle FillStyle { get; }
		CustomBorder Border { get; }
	}
	public interface IDoughnutSeriesView {
		int HoleRadiusPercent { get; set; }
	}
	public interface IBackground {
		Color BackColor { get; set; }
		FillStyleBase FillStyle { get; }
		BackgroundImage BackImage { get; }
		bool BackImageSupported { get; }
	}
	public interface ITextAppearance {
		DefaultBoolean EnableAntialiasing { get; set; }
		Color TextColor { get; set; }
		Font Font { get; set; }
	}
	public interface IHitRegion : IDisposable, ICloneable {
		bool IsVisible(PointF hitPoint);
		Region GetGDIRegion();
		void Serialize(IHitRegionSerializer serializer);
	}
	public interface IHitRegionSerializer {
		void SerializeRectangle(RectangleF rect);
		void SerializePath(GraphicsPath path);
		void SerializeEmptyRegion();
		void SerializeUnionExpression(IHitRegion leftOperand, IHitRegion rightOperand);
		void SerializeIntersectExpression(IHitRegion leftOperand, IHitRegion rightOperand);
		void SerializeExcludeExpression(IHitRegion leftOperand, IHitRegion rightOperand);
		void SerializeXorExpression(IHitRegion leftOperand, IHitRegion rightOperand);
	}
	public interface ISupportID {
		int ID { get; set; }
	}
	public interface IWebChartDesigner {
		string GetImageUrl(string oldUrl);
	}
	public interface IScrollingZoomingOptions {
		bool UseKeyboardScrolling { get; }
		bool UseKeyboardZooming { get; }
		bool UseKeyboardWithMouseZooming { get; }
		bool UseMouseScrolling { get; }
		bool UseMouseWheelZooming { get; }
		bool UseScrollBarsScrolling { get; }
		bool UseTouchDevicePanning { get; }
		bool UseTouchDeviceZooming { get; }
		bool UseTouchDeviceRotation { get; }
	}
	public struct NormalizedRange {
		double min, max;
		public double Minimum {
			get { return this.min; }
			set { this.min = value; }
		}
		public double Maximum {
			get { return this.max; }
			set { this.max = value; }
		}
		public NormalizedRange(double min, double max) {
			this.min = min;
			this.max = max;
		}
	}
	public interface IRangeControlPaint {
		int CalcX(double value);
		Rectangle ContentBounds { get; }
		Graphics Graphics { get; }
	}
	public interface ISupportRangeControl {
		int TopIndent { get; }
		int BottomIndent { get; }
		bool IsValid { get; }
		string InvalidText { get; }
		void OnRangeControlChanged(object sender);
		bool CheckTypeSupport(Type type);
		void DrawContent(IRangeControlPaint paint);
		NormalizedRange ValidateNormalRange(NormalizedRange range, RangeValidationBase validationBase);
		string RulerToString(int rulerIndex);
		string ValueToString(double normalizedValue);
		object ProjectBackValue(double normalOffset);
		double ProjectValue(object value);
		List<object> GetValuesInRange(object minValue, object maxValue, int scaleLength);
		void RangeChanged(object minValue, object maxValue);
		void Invalidate(bool redraw);
		object NativeValue(double normalOffset);
		object GetOptions();
	}
	public interface ISupportBorderVisibility {
		bool BorderVisible { get; }
	}
	public interface IAxisLabelFormatter : IAxisLabelFormatterCore { }
	public interface IDataLabelFormatter {
		string GetDataLabelText(RefinedPoint point);
	}
	public interface IGradientFillOptions<TGradientMode> {
		IGradientPainter GetPainter(TGradientMode gradientMode, IRenderer renderer);
	}
	public interface IChartElementDesignerModel {
		object SourceElement { get; }
	}
}
