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
using System.Drawing.Design;
using DevExpress.Charts.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public interface IXYSeriesView2D {
		Axis2D AxisX { get; }
		Axis2D AxisY { get; }
		XYDiagramPaneBase Pane { get; }
	}
	public abstract class XYDiagram2DSeriesViewBase : SeriesViewBase, IXYSeriesView2D, IXYSeriesView, IXtraSupportDeserializeCollectionItem {
		readonly IndicatorCollection indicators;
		readonly FinancialIndicators financialIndicators;
		readonly RegressionLines regressionLines;
		readonly RangeControlOptions rangeControlOptions;
		string paneName = String.Empty;
		string axisXName = String.Empty;
		string axisYName = String.Empty;
		protected abstract int PixelsPerArgument { get; }
		protected internal abstract XYDiagramPaneBase ActualPane { get; set; }
		protected internal abstract Axis2D ActualAxisX { get; set; }
		protected internal abstract Axis2D ActualAxisY { get; set; }
		protected internal abstract bool HitTestingSupported { get; }
		protected internal override bool HitTestingSupportedForLegendMarker { get { return true; } }
		protected internal override bool DateTimeValuesSupported { get { return true; } }
		protected internal override bool AnnotationLabelModeSupported { get { return true; } }
		protected internal override bool ActualSideMarginsEnabled {
			get {
				Axis2D actualAxisX = ActualAxisX;
				return actualAxisX == null ? base.ActualSideMarginsEnabled : actualAxisX.VisualRange.AutoSideMargins;
			}
		}
		protected override bool Is3DView { get { return false; } }
		protected override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.XYView; } }
		protected internal override bool IsSupportedToolTips { get { return true; } }
		protected internal override bool IsSupportedCrosshair { get { return true; } }
		protected internal override bool NeedFilterVisiblePoints { get { return (Chart != null) && !(Chart.AnnotationRepository.HasSeriesPointAnchoredAnnotations()); } }
		internal bool IsScrollingEnabled {
			get {
				XYDiagramPaneBase actualPane = ActualPane;
				return actualPane != null && (actualPane.ActualEnableAxisXScrolling || actualPane.ActualEnableAxisYScrolling);
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string PaneName {
			get {
				XYDiagramPaneBase actualPane = ActualPane;
				if (actualPane == null) {
					XYDiagram2D diagram = GetXYDiagram();
					return diagram == null ? String.Empty : diagram.DefaultPane.Name;
				}
				return actualPane.Name;
			}
			set { paneName = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string AxisXName {
			get {
				Axis2D actualAxisX = ActualAxisX;
				if (actualAxisX == null) {
					XYDiagram2D diagram = GetXYDiagram();
					return diagram == null ? String.Empty : diagram.ActualAxisX.Name;
				}
				return actualAxisX.Name;
			}
			set { axisXName = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		NonTestableProperty,
		XtraSerializableProperty
		]
		public string AxisYName {
			get {
				Axis2D actualAxisY = ActualAxisY;
				if (actualAxisY == null) {
					XYDiagram2D diagram = GetXYDiagram();
					return diagram == null ? String.Empty : diagram.ActualAxisY.Name;
				}
				return actualAxisY.Name;
			}
			set { axisYName = value; }
		}
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DSeriesViewBaseIndicators"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.XYDiagram2DSeriesViewBase.Indicators"),
		TypeConverter(typeof(CollectionTypeConverter)),
		Editor("DevExpress.XtraCharts.Design.IndicatorsEditor," + AssemblyInfo.SRAssemblyChartsWizard, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true)
		]
		public IndicatorCollection Indicators { get { return indicators; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("XYDiagram2DSeriesViewBaseRangeControlOptions"),
#endif
		DXDisplayNameIgnore,
		Category(Categories.Behavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RangeControlOptions RangeControlOptions { get { return this.rangeControlOptions; } }
		[
		Obsolete("This property is now obsolete. Use the Indicators property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public FinancialIndicators FinancialIndicators { get { return financialIndicators; } }
		[
		Obsolete("This property is now obsolete. Use the Indicators property instead."),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NestedTagProperty,
		XtraSerializableProperty(XtraSerializationVisibility.Content)
		]
		public RegressionLines RegressionLines { get { return regressionLines; } }
		protected XYDiagram2DSeriesViewBase() : base() {
			indicators = new IndicatorCollection(this);
			financialIndicators = new FinancialIndicators(this);
			regressionLines = new RegressionLines(this);
			rangeControlOptions = new RangeControlOptions(this);
		}
		#region IXYSeriesView2D implementation
		Axis2D IXYSeriesView2D.AxisX { get { return ActualAxisX; } }
		Axis2D IXYSeriesView2D.AxisY { get { return ActualAxisY; } }
		XYDiagramPaneBase IXYSeriesView2D.Pane { get { return ActualPane; } }
		#endregion
		#region IXYSeriesView
		bool IXYSeriesView.SideMarginsEnabled { get { return SideMarginsEnabled; } }
		bool IXYSeriesView.CrosshairEnabled { get { return Series.ActualCrosshairEnabled; } }
		int IXYSeriesView.PixelsPerArgument { get { return PixelsPerArgument; } }
		string IXYSeriesView.CrosshairLabelPattern { get { return Series.CrosshairLabelPattern; } }
		IAxisData IXYSeriesView.AxisXData { get { return ActualAxisX; } }
		IAxisData IXYSeriesView.AxisYData { get { return ActualAxisY; } }
		IPane IXYSeriesView.Pane { get { return ActualPane; } }
		ToolTipPointDataToStringConverter IXYSeriesView.CrosshairConverter { get { return CreateToolTipValueToStringConverter(); ; } }
		IEnumerable<double> IXYSeriesView.GetCrosshairValues(RefinedPoint refinedPoint) {
			return GetCrosshairValues(refinedPoint);
		}
		List<ISeparatePaneIndicator> IXYSeriesView.GetSeparatePaneIndicators() {
			var separatePaneIndicators = new List<ISeparatePaneIndicator>();
			foreach (Indicator indicator in Indicators) {
				ISeparatePaneIndicator separatePaneIndicator = indicator as ISeparatePaneIndicator;
				if (separatePaneIndicator != null)
					separatePaneIndicators.Add(separatePaneIndicator);
			}
			return separatePaneIndicators;
		}
		List<IAffectsAxisRange> IXYSeriesView.GetIndicatorsAffectRange() {
			var affectingRangeIndicators = new List<IAffectsAxisRange>();
			foreach (Indicator indicator in Indicators) {
				IAffectsAxisRange affectingRangeIndicator = indicator as IAffectsAxisRange;
				if (affectingRangeIndicator != null && indicator.Visible)
					affectingRangeIndicators.Add(affectingRangeIndicator);
			}
			return affectingRangeIndicators;
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializePaneName() {
			XYDiagramPaneBase actualPane = ActualPane;
			if (actualPane == null)
				return false;
			XYDiagram2D diagram = GetXYDiagram();
			return diagram == null || actualPane != diagram.DefaultPane;
		}
		bool ShouldSerializeAxisXName() {
			Axis2D actualAxisX = ActualAxisX;
			if (actualAxisX == null)
				return false;
			XYDiagram2D diagram = GetXYDiagram();
			return diagram == null || actualAxisX != diagram.ActualAxisX;
		}
		bool ShouldSerializeAxisYName() {
			Axis2D actualAxisY = ActualAxisY;
			if (actualAxisY == null)
				return false;
			XYDiagram2D diagram = GetXYDiagram();
			return diagram == null || actualAxisY != diagram.ActualAxisY;
		}
		bool ShouldSerializeIndicators() {
			return indicators.Count > 0;
		}
		bool ShouldSerializeFinancialIndicators() {
			return false;
		}
		bool ShouldSerializeRegressionLines() {
			return false;
		}
		bool ShouldSerializeRangeControlOptions() {
			return this.rangeControlOptions.ShouldSerialize();
		}
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || ShouldSerializePaneName() || ShouldSerializeAxisXName() ||
				ShouldSerializeAxisYName() || ShouldSerializeIndicators() || ShouldSerializeFinancialIndicators() ||
				ShouldSerializeRangeControlOptions();
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "PaneName":
					return ShouldSerializePaneName();
				case "AxisXName":
					return ShouldSerializeAxisXName();
				case "AxisYName":
					return ShouldSerializeAxisYName();
				case "Indicators":
					return ShouldSerializeIndicators();
				case "FinancialIndicators":
					return ShouldSerializeFinancialIndicators();
				case "RegressionLines":
					return ShouldSerializeRegressionLines();
				case "RangeControlOptions":
					return ShouldSerializeRangeControlOptions();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			return propertyName == "Indicators" ? (XtraSerializingUtils.GetContentPropertyInstance(e, SerializationUtils.ExecutingAssembly, SerializationUtils.PublicNamespace) as Indicator) : null;
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if (propertyName == "Indicators")
				indicators.Add((Indicator)e.Item.Value);
		}
		#endregion
		protected virtual IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint) {
			yield return ((IValuePoint)refinedPoint).Value;
		}
		protected override PatternDataProvider GetDataProvider(PatternConstants patternConstant) {
			return GetXYDataProvider(patternConstant);
		}
		protected override AxisBase GetAxisX() {
			if (ActualAxisX != null)
				return ActualAxisX;
			if (Chart != null) {
				XYDiagram xyDiagram = Chart.Diagram as XYDiagram;
				if (xyDiagram != null) {
					if (!String.IsNullOrEmpty(AxisXName)) {
						AxisBase axis = xyDiagram.FindAxisXByName(AxisXName);
						if (axis != null)
							return axis;
					}
					return xyDiagram.AxisX;
				}
			}
			return null;
		}
		protected override AxisBase GetAxisY() {
			if (ActualAxisY != null)
				return ActualAxisY;
			if (Chart != null) {
				XYDiagram xyDiagram = Chart.Diagram as XYDiagram;
				if (xyDiagram != null) {
					if (!String.IsNullOrEmpty(AxisYName)) {
						AxisBase axis = xyDiagram.FindAxisYByName(AxisYName);
						if (axis != null)
							return axis;
					}
					return xyDiagram.AxisY;
				}
			}
			return null;
		}
		internal string GetPaneName() {
			return paneName;
		}
		internal string GetAxisXName() {
			return axisXName;
		}
		internal string GetAxisYName() {
			return axisYName;
		}
		internal XYDiagram2D GetXYDiagram() {
			SeriesBase series = Owner as SeriesBase;
			if (series == null)
				return null;
			Chart chart = series.Chart;
			if (chart == null)
				return null;
			XYDiagram2D diagram = chart.Diagram as XYDiagram2D;
			if (diagram == null)
				diagram = chart.DiagramCache.Load(DiagramType) as XYDiagram2D;
			return diagram;
		}
		protected void CheckPane(XYDiagramPaneBase pane, bool allowNull) {
			if (pane == null) {
				if (!allowNull)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgNullSeriesViewPane));
			}
			else {
				XYDiagram2D diagram = GetXYDiagram();
				if (diagram != null && pane != diagram.DefaultPane && !diagram.Panes.Contains(pane))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesViewPane));
			}
		}
		protected void CheckAxisX(Axis2D axisX, bool allowNull) {
			if (axisX == null) {
				if (!allowNull)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgNullSeriesViewAxisX));
			}
			else {
				XYDiagram2D diagram = GetXYDiagram();
				if (diagram != null && axisX != diagram.ActualAxisX && !diagram.ActualSecondaryAxesX.ContainsInternal(axisX))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesViewAxisX));
			}
		}
		protected void CheckAxisY(Axis2D axisY, bool allowNull) {
			if (axisY == null) {
				if (!allowNull)
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgNullSeriesViewAxisY));
			}
			else {
				XYDiagram2D diagram = GetXYDiagram();
				if (diagram != null && axisY != diagram.ActualAxisY && !diagram.ActualSecondaryAxesY.ContainsInternal(axisY))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectSeriesViewAxisY));
			}
		}
		protected internal override void OnEndLoading() {
			base.OnEndLoading();
			XYDiagram2D diagram = GetXYDiagram();
			if (diagram != null) {
				if (!String.IsNullOrEmpty(paneName)) {
					ActualPane = diagram.FindPaneByName(paneName) ?? diagram.DefaultPane;
					RaiseControlChanged(new PropertyUpdateInfo(base.Owner, "Pane"));
				}
				if (!String.IsNullOrEmpty(axisXName)) {
					ActualAxisX = diagram.FindAxisXByName(axisXName) ?? diagram.ActualAxisX;
					RaiseControlChanged(new PropertyUpdateInfo(base.Owner, "AxisX"));
				}
				if (!String.IsNullOrEmpty(axisYName)) {
					ActualAxisY = diagram.FindAxisYByName(axisYName) ?? diagram.ActualAxisY;
					RaiseControlChanged(new PropertyUpdateInfo(base.Owner, "AxisY"));
				}
			}
			else {
				ActualPane = null;
				ActualAxisX = null;
				ActualAxisY = null;
			}
			regressionLines.OnEndLoading();
			indicators.OnEndLoding();
		}
		protected internal override bool Contains(object obj) {
			Indicator indicator = obj as Indicator;
			return indicator == null ? base.Contains(obj) : indicators.Contains(indicator);
		}
		protected internal override void CheckOnAddingSeries() {
			base.CheckOnAddingSeries();
			CheckPane(ActualPane, true);
			CheckAxisX(ActualAxisX, true);
			CheckAxisY(ActualAxisY, true);
		}
		protected internal virtual SeriesPointLayout CalculateSeriesPointLayout(XYDiagramMappingBase diagramMapping, RefinedPointData pointData) {
			return null;
		}
		protected internal virtual HighlightedPointLayout CalculateHighlightedPointLayout(XYDiagramMappingBase diagramMapping, RefinedPoint refinedPoint, ISeriesView seriesView, DrawOptions drawOptions) {
			return null;
		}
		protected internal virtual WholeSeriesLayout CalculateWholeSeriesLayout(XYDiagramMappingBase diagramMapping, SeriesLayout seriesLayout) {
			return null;
		}
		protected internal virtual DiagramPoint? CalculateRelativeToolTipPosition(XYDiagramMappingBase mapping, RefinedPointData pointData) {
			return null;
		}
		protected internal override GraphicsCommand CreateGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal override GraphicsCommand CreateShadowGraphicsCommand(Rectangle mappingBounds, SeriesPointLayout pointLayout, DrawOptions drawOptions) {
			return null;
		}
		protected internal abstract void CalculateAnnotationsAnchorPointsLayout(XYDiagramAnchorPointLayoutList anchorPointLayoutList);
		protected internal override bool IsCompatible(SeriesViewBase view) {
			XYDiagram2DSeriesViewBase xyView = view as XYDiagram2DSeriesViewBase;
			return xyView != null && base.IsCompatible(xyView) && object.Equals(ActualAxisX, xyView.ActualAxisX);
		}
		protected internal virtual void RenderHighlightedPoint(IRenderer renderer, HighlightedPointLayout pointLayout) {
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			XYDiagram2DSeriesViewBase view = obj as XYDiagram2DSeriesViewBase;
			if (view != null) {
				paneName = view.PaneName;
				axisXName = view.AxisXName;
				axisYName = view.AxisYName;
				indicators.Assign(view.indicators);
				financialIndicators.Assign(view.financialIndicators);
				rangeControlOptions.Assign(view.rangeControlOptions);
			}
		}
	}
}
