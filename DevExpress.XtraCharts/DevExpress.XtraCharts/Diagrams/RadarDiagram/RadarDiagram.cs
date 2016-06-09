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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Native {
	public interface IRadarDiagram {
		RadarDiagramMapping RadarDiagramMapping { get; }
	}
}
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum RadarDiagramDrawingStyle {
		Circle,
		Polygon
	}
	[
	TypeConverter(typeof(EnumTypeConverter)),
	ResourceFinder(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile),
	]
	public enum RadarDiagramRotationDirection {
		Counterclockwise,
		Clockwise
	}
	[
	TypeConverter(typeof(TypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class RadarDiagram : Diagram, IXYDiagram, IPane, IBackground, IRadarDiagram {
		#region Nested class: TypeConverter
		public class TypeConverter : ExpandableObjectConverter {
			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
			}
			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
				return destinationType == typeof(InstanceDescriptor) ?
					new InstanceDescriptor(typeof(RadarDiagram).GetConstructor(new Type[] { }), null, false) :
					base.ConvertTo(context, culture, value, destinationType);
			}
		}
		#endregion
		const bool DefaultBorderVisible = true;
		const int DefaultMargins = 5;
		const RadarDiagramDrawingStyle DefaultDrawingStyle = RadarDiagramDrawingStyle.Circle;
		const double DefaultStartAngle = 0.0;
		const RadarDiagramRotationDirection DefaultRotationDirection = RadarDiagramRotationDirection.Counterclockwise;
		static readonly Color defaultColor = Color.Empty;
		readonly RadarAxisX axisX;
		readonly RadarAxisY axisY;
		readonly PolygonFillStyle fillStyle;
		readonly BackgroundImage backImage;
		readonly Shadow shadow;
		readonly RectangleIndents margins;
		Color backColor = defaultColor;
		bool borderVisible = DefaultBorderVisible;
		Color borderColor = defaultColor;
		RadarDiagramDrawingStyle drawingStyle = DefaultDrawingStyle;
		double startAngleInDegrees = DefaultStartAngle;
		RadarDiagramRotationDirection rotationDirection = DefaultRotationDirection;
		RadarDiagramMapping lastMapping;
		RadarDiagramMapping LastMapping {
			get {
				Chart chart = Chart;
				if (chart.CacheToMemory) {
					RadarCoordinatesConversionCache cache = chart.GetCoordinatesConversionCache() as RadarCoordinatesConversionCache;
					if (cache != null)
						return cache.DiagramMapping;
				}
				return lastMapping;
			}
		}
		RadarDiagramAppearance Appearance { get { return CommonUtils.GetActualAppearance(this).RadarDiagramAppearance; } }
		RadarDiagramMapping IRadarDiagram.RadarDiagramMapping { get { return lastMapping; } }
		internal Color ActualBackColor { get { return backColor == Color.Empty ? Appearance.BackColor : backColor; } }
		internal PolygonFillStyle ActualFillStyle {
			get { return fillStyle.FillMode == FillMode.Empty ? (PolygonFillStyle)Appearance.FillStyle : fillStyle; }
		}
		internal double ActualStartAngle {
			get { return (rotationDirection == RadarDiagramRotationDirection.Clockwise ? startAngleInDegrees : -startAngleInDegrees) * Math.PI / 180.0; }
		}
		protected internal virtual bool ClipArgument { get { return true; } }
		protected internal override bool SupportTooltips { get { return true; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramAxisX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.AxisX"),
		Category("Elements"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RadarAxisX AxisX { get { return axisX; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramAxisY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.AxisY"),
		Category("Elements"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RadarAxisY AxisY { get { return axisY; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.FillStyle"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public PolygonFillStyle FillStyle { get { return fillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramBackImage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.BackImage"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public BackgroundImage BackImage { get { return backImage; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramShadow"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.Shadow"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public Shadow Shadow { get { return shadow; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramMargins"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.Margins"),
		Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(typeof(ExpandableObjectConverter)),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleIndents Margins { get { return margins; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.BackColor"),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public Color BackColor {
			get { return backColor; }
			set {
				if (value != backColor) {
					SendNotification(new ElementWillChangeNotification(this));
					backColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramBorderVisible"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.BorderVisible"),
		TypeConverter(typeof(BooleanTypeConverter)),
		Category("Appearance"),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty
		]
		public bool BorderVisible {
			get { return borderVisible; }
			set {
				if (value != borderVisible) {
					SendNotification(new ElementWillChangeNotification(this));
					borderVisible = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramBorderColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.BorderColor"),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public Color BorderColor {
			get { return borderColor; }
			set {
				if (value != borderColor) {
					SendNotification(new ElementWillChangeNotification(this));
					borderColor = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramDrawingStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.DrawingStyle"),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public RadarDiagramDrawingStyle DrawingStyle {
			get { return drawingStyle; }
			set {
				if (value != drawingStyle) {
					SendNotification(new ElementWillChangeNotification(this));
					drawingStyle = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramStartAngleInDegrees"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.StartAngleInDegrees"),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public double StartAngleInDegrees {
			get { return startAngleInDegrees; }
			set {
				if (value != startAngleInDegrees) {
					if (!Loading && (value < -360.0 || value > 360))
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectStartAngle));
					SendNotification(new ElementWillChangeNotification(this));
					startAngleInDegrees = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("RadarDiagramRotationDirection"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.RadarDiagram.RotationDirection"),
		Category("Appearance"),
		XtraSerializableProperty
		]
		public RadarDiagramRotationDirection RotationDirection {
			get { return rotationDirection; }
			set {
				if (value != rotationDirection) {
					SendNotification(new ElementWillChangeNotification(this));
					rotationDirection = value;
					RaiseControlChanged();
				}
			}
		}
		public RadarDiagram() {
			axisX = CreateAxisX();
			axisY = new RadarAxisY(this);
			fillStyle = new PolygonFillStyle(this);
			backImage = new BackgroundImage(this);
			shadow = new Shadow(this);
			margins = new RectangleIndents(this, DefaultMargins);
		}
		#region IPane implementation
		int IPane.PaneIndex { get { return -1; } }
		GRealRect2D? IPane.MappingBounds { get { return null; } }
		#endregion
		#region IXYDiagram implementation
		IList<IPane> IXYDiagram.Panes { get { return new IPane[] { this }; } }
		IAxisData IXYDiagram.AxisX { get { return axisX; } }
		IAxisData IXYDiagram.AxisY { get { return axisY; } }
		IEnumerable<IAxisData> IXYDiagram.SecondaryAxesX { get { return null; } }
		IEnumerable<IAxisData> IXYDiagram.SecondaryAxesY { get { return null; } }
		bool IXYDiagram.ScrollingEnabled { get { return false; } }
		bool IXYDiagram.Rotated { get { return false; } }
		ICrosshairOptions IXYDiagram.CrosshairOptions { get { return null; } }
		IList<IPane> IXYDiagram.GetCrosshairSyncPanes(IPane focusedPane, bool isHorizontalSync) {
			return null;
		}
		InternalCoordinates IXYDiagram.MapPointToInternal(IPane pane, GRealPoint2D point) {
			return null;
		}
		GRealPoint2D IXYDiagram.MapInternalToPoint(IPane pane, IAxisData axisX, IAxisData axisY, double argument, double value) {
			return new GRealPoint2D();
		}
		List<IPaneAxesContainer> IXYDiagram.GetPaneAxesContainers(IList<RefinedSeries> activeSeries) {
			return new List<IPaneAxesContainer>();
		}
		void IXYDiagram.UpdateCrosshairData(IList<RefinedSeries> seriesCollection) {
		}
		void IXYDiagram.UpdateAutoMeasureUnits() {
			UpdateAutoMeasureUnits();
		}
		int IXYDiagram.GetAxisXLength(IAxisData axis) {
			return GetAxisXLength();
		}
		#endregion
		#region IBackground implementation
		bool IBackground.BackImageSupported { get { return true; } }
		FillStyleBase IBackground.FillStyle { get { return fillStyle; } }
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeAxisX() {
			return axisX.ShouldSerialize();
		}
		bool ShouldSerializeAxisY() {
			return axisY.ShouldSerialize();
		}
		bool ShouldSerializeFillStyle() {
			return fillStyle.ShouldSerialize();
		}
		bool ShouldSerializeBackImage() {
			return backImage.ShouldSerialize();
		}
		bool ShouldSerializeShadow() {
			return shadow.ShouldSerialize();
		}
		bool ShouldSerializeMargins() {
			return margins.ShouldSerialize();
		}
		bool ShouldSerializeBackColor() {
			return backColor != defaultColor;
		}
		void ResetBackColor() {
			BackColor = defaultColor;
		}
		bool ShouldSerializeBorderVisible() {
			return this.borderVisible != DefaultBorderVisible;
		}
		void ResetBorderVisible() {
			BorderVisible = DefaultBorderVisible;
		}
		bool ShouldSerializeBorderColor() {
			return borderColor != defaultColor;
		}
		void ResetBorderColor() {
			BorderColor = defaultColor;
		}
		bool ShouldSerializeDrawingStyle() {
			return drawingStyle != DefaultDrawingStyle;
		}
		void ResetDrawingStyle() {
			DrawingStyle = DefaultDrawingStyle;
		}
		bool ShouldSerializeStartAngleInDegrees() {
			return startAngleInDegrees != DefaultStartAngle;
		}
		void ResetStartAngleInDegrees() {
			StartAngleInDegrees = DefaultStartAngle;
		}
		bool ShouldSerializeRotationDirection() {
			return rotationDirection != DefaultRotationDirection;
		}
		void ResetRotationDirection() {
			RotationDirection = DefaultRotationDirection;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeAxisX() || ShouldSerializeAxisY() || ShouldSerializeFillStyle() ||
				ShouldSerializeBackImage() || ShouldSerializeShadow() || ShouldSerializeMargins() || ShouldSerializeBackColor() ||
				ShouldSerializeBorderVisible() || ShouldSerializeBorderColor() || ShouldSerializeDrawingStyle() ||
				ShouldSerializeStartAngleInDegrees() || ShouldSerializeRotationDirection();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AxisX":
					return ShouldSerializeAxisX();
				case "AxisY":
					return ShouldSerializeAxisY();
				case "FillStyle":
					return ShouldSerializeFillStyle();
				case "BackImage":
					return ShouldSerializeBackImage();
				case "Shadow":
					return ShouldSerializeShadow();
				case "Margins":
					return ShouldSerializeMargins();
				case "BackColor":
					return ShouldSerializeBackColor();
				case "BorderVisible":
					return ShouldSerializeBorderVisible();
				case "BorderColor":
					return ShouldSerializeBorderColor();
				case "DrawingStyle":
					return ShouldSerializeDrawingStyle();
				case "StartAngleInDegrees":
					return ShouldSerializeStartAngleInDegrees();
				case "RotationDirection":
					return ShouldSerializeRotationDirection();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		ControlCoordinates CalcDiagramToPoint(object argument, object value) {
			double argumentInternal = axisX.ScaleTypeMap.NativeToInternal(argument);
			double valueInternal = axisY.ScaleTypeMap.NativeToInternal(value);
			if (double.IsNaN(argumentInternal) || double.IsNaN(valueInternal))
				return new ControlCoordinates(null, ControlCoordinatesVisibility.Undefined, Point.Empty);
			if (LastMapping == null)
				Chart.PerformViewDataCalculation(Rectangle.Empty, false);
			RadarDiagramMapping mapping = LastMapping;
			if (mapping == null)
				return new ControlCoordinates();
			DiagramPoint diagramPoint = mapping.GetScreenPoint(argumentInternal, valueInternal);
			if (diagramPoint.IsZero)
				return new ControlCoordinates();
			return new ControlCoordinates(null,
				(mapping.MinValue <= valueInternal && valueInternal <= mapping.MaxValue) ? ControlCoordinatesVisibility.Visible : ControlCoordinatesVisibility.Hidden, (Point)diagramPoint);
		}
		int GetAxisXLength() {
			if (Chart == null)
				return 0;
			Rectangle bounds = Chart.LastBounds;
			return (int)Math.PI * Math.Min(bounds.Width, bounds.Height);
		}
		protected virtual void UpdateAutoMeasureUnits() {
			if (axisX.UpdateAutomaticMeasureUnit(GetAxisXLength()))
				RaiseControlChanged(new DataAggregationUpdate(AxisX));
		}
		protected virtual RadarAxisX CreateAxisX() {
			return new RadarAxisX(this);
		}
		protected override ChartElement CreateObjectForClone() {
			return new RadarDiagram();
		}
		protected internal override bool Contains(object obj) {
			return obj == axisX || obj == axisY;
		}
		protected internal override INativeGraphics CreateNativeGraphics(Graphics gr, IntPtr hDC, Rectangle bounds, Rectangle windowsBounds) {
			return new GdiPlusGraphics(gr);
		}
		protected internal override DiagramViewData CalculateViewData(TextMeasurer textMeasurer, Rectangle diagramBounds, IList<RefinedSeriesData> seriesDataList, bool performRangeCorrection) {
			lastMapping = null;
			if (!diagramBounds.AreWidthAndHeightPositive())
				return null;
			Rectangle bounds = margins.DecreaseRectangle(diagramBounds);
			if (!bounds.AreWidthAndHeightPositive())
				return null;
			if (shadow.Visible) {
				bounds = new Rectangle(bounds.Location, shadow.DecreaseSize(bounds.Size));
				if (!bounds.AreWidthAndHeightPositive())
					return null;
			}
			int borderThickness = 0;
			if (BorderVisible) {
				borderThickness = 1;
				bounds.Inflate(-1, -1);
				if (!bounds.AreWidthAndHeightPositive())
					return null;
			}
			HitTestState hitState = ((IHitTest)this).State;
			RectangleCorrection mappingBoundsCorrection = null;
			for (; ; ) {
				RectangleF actualBounds;
				if (mappingBoundsCorrection == null)
					actualBounds = bounds;
				else {
					Rectangle rect = mappingBoundsCorrection.Correct(bounds);
					if (!rect.AreWidthAndHeightPositive())
						return null;
					actualBounds = rect;
				}
				int dimensionDiff = bounds.Width - bounds.Height;
				if (dimensionDiff >= 0) {
					actualBounds.Width -= dimensionDiff;
					actualBounds.Offset(dimensionDiff / 2, 0.0f);
				} else {
					actualBounds.Height += dimensionDiff;
					actualBounds.Offset(0.0f, -dimensionDiff / 2);
				}
				IMinMaxValues visualAxisXRange = (IMinMaxValues)axisX.VisualRangeData;
				GridAndTextDataEx axisXGridAndTextData = new GridAndTextDataEx(axisX.GetSeries(), axisX, false, visualAxisXRange, visualAxisXRange, Math.Min(actualBounds.Width, actualBounds.Height) * Math.PI, (axisX.Label == null) ? false : axisX.Label.Staggered);
				RadarDiagramMapping mapping = new RadarDiagramMapping(this, (ZPlaneRectangle)actualBounds, axisXGridAndTextData.GridData.Items.VisibleValues);
				if (!mapping.IsValid)
					return null;
				IMinMaxValues visualAxisYRange = (IMinMaxValues)axisY.VisualRangeData;
				GridAndTextDataEx axisYGridAndTextData = new GridAndTextDataEx(axisY.GetSeries(), axisY, false, visualAxisYRange, visualAxisYRange, actualBounds.Height, (axisY.Label == null) ? false : axisY.Label.Staggered);
				List<SeriesLabelLayoutList> labelLayoutLists = new List<SeriesLabelLayoutList>();
				List<AnnotationLayout> annotationsAnchorPointsLayout = new List<AnnotationLayout>();
				foreach (RefinedSeriesData seriesData in seriesDataList) {
					labelLayoutLists.Add(new RadarDiagramSeriesLabelLayoutList(seriesData, mapping, textMeasurer));
					annotationsAnchorPointsLayout.AddRange(new RadarDiagramAnchorPointLayoutList(seriesData, mapping));
				}
				List<AnnotationViewData> annotationsViewData = AnnotationHelper.CreateInnerAnnotationsViewData(annotationsAnchorPointsLayout, textMeasurer);
				RectangleCorrection correction = new RectangleCorrection(diagramBounds);
				RadarAxisXViewData axisXViewData = new RadarAxisXViewData(textMeasurer, axisX, mapping, axisXGridAndTextData, borderThickness);
				axisXViewData.UpdateCorrection(correction);
				double angle = RotatedTextPainterBase.CancelAngle((float)startAngleInDegrees);
				RadarAxisYViewData axisYViewData = new RadarAxisYViewData(textMeasurer, axisY, mapping,
					axisYGridAndTextData, angle == 0.0 || angle == 90.0 || angle == 180.0 || angle == 270.0);
				axisYViewData.UpdateCorrection(correction);
				foreach (SeriesLabelLayoutList labelLayoutList in labelLayoutLists)
					labelLayoutList.CalculateDiagramBoundsCorrection(correction);
				AnnotationHelper.CalculateAnnotationsCorrection(annotationsViewData, correction);
				if (!correction.ShouldCorrect) {
					if (!hitState.Normal)
						borderThickness++;
					Color borderColor = BorderColor;
					if (borderThickness > 0)
						borderColor = GraphicUtils.GetColor(borderColor.IsEmpty ? Appearance.BorderColor : borderColor, hitState);
					List<SeriesLayout> seriesLayoutList = new List<SeriesLayout>();
					foreach (RefinedSeriesData seriesData in seriesDataList)
						seriesLayoutList.Add(new RadarDiagramSeriesLayout(seriesData, mapping));
					lastMapping = mapping;
					return new RadarDiagramViewData(this, mapping, bounds, actualBounds, axisXViewData, axisYViewData,
						borderThickness, borderColor, seriesLayoutList, labelLayoutLists, annotationsAnchorPointsLayout, annotationsViewData, diagramBounds);
				}
				mappingBoundsCorrection = RectangleCorrection.Combine(mappingBoundsCorrection, correction);
			}
		}
		protected internal override void OnEndLoading() {
			base.OnEndLoading();
			axisX.OnEndLoading();
			axisY.OnEndLoading();
		}
		protected internal override CoordinatesConversionCache CreateCoordinatesConversionCache() {
			return new RadarCoordinatesConversionCache(lastMapping);
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			RadarDiagram diagram = obj as RadarDiagram;
			if (diagram != null) {
				axisX.Assign(diagram.axisX);
				axisY.Assign(diagram.axisY);
				fillStyle.Assign(diagram.fillStyle);
				backImage.Assign(diagram.backImage);
				shadow.Assign(diagram.shadow);
				margins.Assign(diagram.margins);
				backColor = diagram.backColor;
				borderVisible = diagram.borderVisible;
				borderColor = diagram.borderColor;
				drawingStyle = diagram.drawingStyle;
				startAngleInDegrees = diagram.startAngleInDegrees;
				rotationDirection = diagram.rotationDirection;
			}
		}
		public DiagramCoordinates PointToDiagram(Point p) {
			DiagramCoordinates coordinates = new DiagramCoordinates();
			if (LastMapping == null)
				Chart.PerformViewDataCalculation(Rectangle.Empty, false);
			RadarDiagramMapping mapping = LastMapping;
			if (mapping == null || mapping.MappingBounds.IsEmpty)
				return coordinates;
			coordinates.SetAxes(axisX, axisY);
			double argumentInternal, valueInternal;
			if (mapping.CalcArgumentAndValue(p, out argumentInternal, out valueInternal))
				coordinates.SetArgumentAndValue(argumentInternal, valueInternal);
			return coordinates;
		}
		public ControlCoordinates DiagramToPoint(string argument, double value) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType));
		}
		public ControlCoordinates DiagramToPoint(string argument, DateTime value) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType));
		}
		public ControlCoordinates DiagramToPoint(double argument, double value) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType));
		}
		public ControlCoordinates DiagramToPoint(double argument, DateTime value) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType));
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, double value) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType));
		}
		public ControlCoordinates DiagramToPoint(DateTime argument, DateTime value) {
			return CalcDiagramToPoint(DiagramToPointUtils.CheckValue(argument, axisX.ScaleType), DiagramToPointUtils.CheckValue(value, axisY.ScaleType));
		}
	}
}
