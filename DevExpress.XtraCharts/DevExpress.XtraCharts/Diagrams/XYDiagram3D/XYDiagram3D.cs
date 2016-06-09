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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Collections.Generic;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts {
	[
	TypeConverter(typeof(XYDiagram3DTypeConverter)),
	DesignerSerializer("DevExpress.XtraCharts.Design.ChartItemSerializer," + AssemblyInfo.SRAssemblyChartsExtensions,
					   "System.ComponentModel.Design.Serialization.CodeDomSerializer,System.Design")
	]
	public class XYDiagram3D : Diagram3D, IXYDiagram, IPane, IBackground {
		const int DefaultSeriesIndentFixed = 10;
		const int DefaultSeriesDistanceFixed = 0;
		const int DefaultPlaneDepthFixed = 15;
		const double DefaultSeriesDistance = 0.5;
		readonly AxisX3D axisX;
		readonly AxisY3D axisY;
		readonly RectangleFillStyle3D fillStyle;
		readonly BackgroundImage backImage;
		Color backColor = Color.Empty;
		int seriesIndentFixed = DefaultSeriesIndentFixed;
		int seriesDistanceFixed = DefaultSeriesDistanceFixed;
		int planeDepthFixed = DefaultPlaneDepthFixed;
		double seriesDistance = DefaultSeriesDistance;
		internal XYDiagram3DAppearance Appearance { get { return CommonUtils.GetActualAppearance(this).XYDiagram3DAppearance; } }
		internal RectangleFillStyle3D ActualFillStyle { 
			get { return fillStyle.FillMode == FillMode3D.Empty ? (RectangleFillStyle3D)Appearance.FillStyle : fillStyle; } 
		}
		internal Color ActualBackColor { get { return backColor == Color.Empty ? Appearance.BackColor : backColor; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram3DAxisX"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram3D.AxisX"),
		Category("Elements"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public AxisX3D AxisX { get { return axisX; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram3DAxisY"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram3D.AxisY"),
		Category("Elements"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public AxisY3D AxisY { get { return axisY; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram3DFillStyle"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram3D.FillStyle"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RectangleFillStyle3D FillStyle { get { return fillStyle; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram3DBackImage"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram3D.BackImage"),
		Category("Appearance"),
		TypeConverter(typeof(ExpandableObjectConverter)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public BackgroundImage BackImage { get { return backImage; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram3DBackColor"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram3D.BackColor"),
		Category("Appearance"),
		RefreshProperties(RefreshProperties.All),
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
	DevExpressXtraChartsLocalizedDescription("XYDiagram3DSeriesDistance"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram3D.SeriesDistance"),
		XtraSerializableProperty
		]
		public double SeriesDistance {
			get { return seriesDistance; }
			set {
				if (value != seriesDistance) {
					if (value < 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesDistance));
					SendNotification(new ElementWillChangeNotification(this));
					seriesDistance = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram3DSeriesDistanceFixed"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram3D.SeriesDistanceFixed"),
		XtraSerializableProperty
		]
		public int SeriesDistanceFixed {
			get { return seriesDistanceFixed; }
			set {
				if (value != seriesDistanceFixed) {
					if (value < 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesDistanceFixed));
					SendNotification(new ElementWillChangeNotification(this));
					seriesDistanceFixed = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram3DSeriesIndentFixed"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram3D.SeriesIndentFixed"),
		XtraSerializableProperty
		]
		public int SeriesIndentFixed { 
			get { return seriesIndentFixed; }
			set {
				if (value != seriesIndentFixed) {
					if (value < 0)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesIndentFixed));
					SendNotification(new ElementWillChangeNotification(this));
					seriesIndentFixed = value;
					RaiseControlChanged();
				}
			}
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram3DPlaneDepthFixed"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram3D.PlaneDepthFixed"),
		XtraSerializableProperty
		]
		public int PlaneDepthFixed { 
			get { return planeDepthFixed; } 
			set {
				if (value != planeDepthFixed) {
					if (value < 1)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPlaneDepthFixed));
					SendNotification(new ElementWillChangeNotification(this));
					planeDepthFixed = value;
					RaiseControlChanged();
				}
			}
		}
		public XYDiagram3D() {
			axisX = new AxisX3D(this);
			axisY = new AxisY3D(this);
			fillStyle = new RectangleFillStyle3D(this);
			backImage = new BackgroundImage(this);
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
			List<IPaneAxesContainer> containers = new List<IPaneAxesContainer>();
			PaneAxesContainer paneAxesContainer = new PaneAxesContainer(this);
			paneAxesContainer.RegisterAxisX(0, axisX);
			paneAxesContainer.RegisterAxisY(0, axisY);
			paneAxesContainer.InitializePrimaryAndSecondaryAxes();
			containers.Add(paneAxesContainer);
			return containers;
		}
		void IXYDiagram.UpdateCrosshairData(IList<RefinedSeries> seriesCollection) {
		}
		void IXYDiagram.UpdateAutoMeasureUnits() {
			if (AxisX.UpdateAutomaticMeasureUnit(GetAxisXLength()))
				RaiseControlChanged(new DataAggregationUpdate(AxisX));
		}
		int IXYDiagram.GetAxisXLength(IAxisData axis) {
			return GetAxisXLength();
		}
		#endregion
		#region IBackgroundimplementation
		bool IBackground.BackImageSupported { get { return true; } }
		FillStyleBase IBackground.FillStyle { get { return FillStyle; } }
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
		bool ShouldSerializeBackColor() {
			return !backColor.IsEmpty;
		}
		void ResetBackColor() {
			BackColor = Color.Empty;
		}
		bool ShouldSerializeSeriesDistance() {
			return seriesDistance != DefaultSeriesDistance;
		}
		void ResetSeriesDistance() {
			SeriesDistance = DefaultSeriesDistance;
		}
		bool ShouldSerializeSeriesDistanceFixed() {
			return seriesDistanceFixed != DefaultSeriesDistanceFixed;
		}
		void ResetSeriesDistanceFixed() {
			SeriesDistanceFixed = DefaultSeriesDistanceFixed;
		}
		bool ShouldSerializeSeriesIndentFixed() {
			return seriesIndentFixed != DefaultSeriesIndentFixed;
		}
		void ResetSeriesIndentFixed() {
			SeriesIndentFixed = DefaultSeriesIndentFixed;
		}
		bool ShouldSerializePlaneDepthFixed() {
			return planeDepthFixed != DefaultPlaneDepthFixed;
		}
		void ResetPlaneDepthFixed() {
			PlaneDepthFixed = DefaultPlaneDepthFixed;
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializeAxisX() || ShouldSerializeAxisY() || ShouldSerializeFillStyle() || 
				ShouldSerializeBackImage() || ShouldSerializeBackColor() || ShouldSerializeSeriesDistance() || ShouldSerializeSeriesDistanceFixed() ||
				ShouldSerializeSeriesIndentFixed() || ShouldSerializePlaneDepthFixed();
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
				case "BackColor":
					return ShouldSerializeBackColor();
				case "SeriesDistance":
					return ShouldSerializeSeriesDistance();
				case "SeriesDistanceFixed":
					return ShouldSerializeSeriesDistanceFixed();
				case "SeriesIndentFixed":
					return ShouldSerializeSeriesIndentFixed();
				case "PlaneDepthFixed":
					return ShouldSerializePlaneDepthFixed();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		int GetAxisXLength() {
			return Chart == null ? 300 : Chart.LastBounds.Width;
		}
		protected override ChartElement CreateObjectForClone() {
			return new XYDiagram3D();
		}
		protected internal override bool Contains(object obj) {
			return false;
		}
		protected internal override DiagramViewData CalculateViewData(TextMeasurer textMeasurer, Rectangle diagramBounds, IList<RefinedSeriesData> seriesDataList, bool performRangeCorrection) {
			UpdateLastDiagramBounds(diagramBounds);
			if (!diagramBounds.AreWidthAndHeightPositive())
				return null;
			XYDiagram3DCoordsCalculator coordsCalculator = XYDiagram3DCoordsCalculator.Create(this, diagramBounds, seriesDataList);
			List<SeriesLayout> seriesLayoutList = new List<SeriesLayout>();
			List<SeriesLabelLayoutList> labelLayoutLists = new List<SeriesLabelLayoutList>();
			List<AnnotationLayout> annotationsAnchorPointsLayout = new List<AnnotationLayout>();
			foreach (RefinedSeriesData seriesData in seriesDataList) {
				seriesLayoutList.Add(new XYDiagram3DSeriesLayout(seriesData, coordsCalculator));
				labelLayoutLists.Add(new XYDiagram3DSeriesLabelLayoutList(seriesData, coordsCalculator, axisY.VisualRangeData, textMeasurer));
				annotationsAnchorPointsLayout.AddRange(new XYDiagram3DAnchorPointLayoutList(seriesData, coordsCalculator, axisY.VisualRangeData));
			}
			List<AnnotationViewData> annotationsViewData = AnnotationHelper.CreateInnerAnnotationsViewData(annotationsAnchorPointsLayout,
				textMeasurer);
			return new XYDiagram3DViewData(coordsCalculator, Chart.Graphics3DCache, textMeasurer,
				seriesLayoutList, labelLayoutLists, annotationsAnchorPointsLayout, annotationsViewData);
		}
		protected internal override void OnEndLoading() {
			base.OnEndLoading();
			axisX.OnEndLoading();
			axisY.OnEndLoading();
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			XYDiagram3D diagram = obj as XYDiagram3D;
			if (diagram != null) {
				axisX.Assign(diagram.axisX);
				axisY.Assign(diagram.axisY);
				fillStyle.Assign(diagram.fillStyle);
				backImage.Assign(diagram.backImage);
				backColor = diagram.backColor;
				seriesIndentFixed = diagram.seriesIndentFixed;
				seriesDistanceFixed = diagram.seriesDistanceFixed;
				seriesDistance = diagram.seriesDistance;
				planeDepthFixed = diagram.planeDepthFixed;
			}
		}
	}
}
