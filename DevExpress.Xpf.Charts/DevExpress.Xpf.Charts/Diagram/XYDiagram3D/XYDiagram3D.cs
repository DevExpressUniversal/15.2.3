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
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Utils.Serializing;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public sealed class XYDiagram3D : Diagram3D, IXYDiagram, IPane {
		const int defaultPlaneDepthFixed = 15;
		const double defaultHeightToWidthRatio = 0.75;
		const double defaultSeriesDistance = 1.3;
		const double defaultSeriesPadding = 0.35;		
		const double defaultAxisLength = 700;
		public static readonly DependencyProperty DomainBrushProperty;
		public static readonly DependencyProperty HeightToWidthRatioProperty;
		public static readonly DependencyProperty AxisXProperty;
		public static readonly DependencyProperty AxisYProperty;
		public static readonly DependencyProperty SeriesDistanceProperty;
		public static readonly DependencyProperty SeriesPaddingProperty;
		public static readonly DependencyProperty PlaneDepthFixedProperty;
		public static readonly DependencyProperty MaterialProperty;
		public static readonly DependencyProperty BarDistanceProperty;
		public static readonly DependencyProperty BarDistanceFixedProperty;
		public static readonly DependencyProperty EqualBarWidthProperty;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DDomainBrush"),
#endif
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush DomainBrush {
			get { return (Brush)GetValue(DomainBrushProperty); }
			set { SetValue(DomainBrushProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DHeightToWidthRatio"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double HeightToWidthRatio {
			get { return (double)GetValue(HeightToWidthRatioProperty); }
			set { SetValue(HeightToWidthRatioProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DAxisX"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public AxisX3D AxisX {
			get { return (AxisX3D)GetValue(AxisXProperty); }
			set { SetValue(AxisXProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DAxisY"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public AxisY3D AxisY {
			get { return (AxisY3D)GetValue(AxisYProperty); }
			set { SetValue(AxisYProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DSeriesDistance"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double SeriesDistance {
			get { return (double)GetValue(SeriesDistanceProperty); }
			set { SetValue(SeriesDistanceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DSeriesPadding"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public double SeriesPadding {
			get { return (double)GetValue(SeriesPaddingProperty); }
			set { SetValue(SeriesPaddingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DPlaneDepthFixed"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public int PlaneDepthFixed {
			get { return (int)GetValue(PlaneDepthFixedProperty); }
			set { SetValue(PlaneDepthFixedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DMaterial"),
#endif
		Category(Categories.Presentation)
		]
		public Material Material {
			get { return (Material)GetValue(MaterialProperty); }
			set { SetValue(MaterialProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DBarDistance"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public double BarDistance {
			get { return (double)GetValue(BarDistanceProperty); }
			set { SetValue(BarDistanceProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DBarDistanceFixed"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int BarDistanceFixed {
			get { return (int)GetValue(BarDistanceFixedProperty); }
			set { SetValue(BarDistanceFixedProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("XYDiagram3DEqualBarWidth"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool EqualBarWidth {
			get { return (bool)GetValue(EqualBarWidthProperty); }
			set { SetValue(EqualBarWidthProperty, value); }
		}
		static XYDiagram3D() {
			Type ownerType = typeof(XYDiagram3D);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			DomainBrushProperty = DependencyProperty.Register("DomainBrush", typeof(Brush), ownerType,
				new FrameworkPropertyMetadata(null, ChartElementHelper.UpdateWithClearDiagramCache));
			HeightToWidthRatioProperty = DependencyProperty.Register("HeightToWidthRatio", typeof(double), ownerType,
				new FrameworkPropertyMetadata(defaultHeightToWidthRatio, FrameworkPropertyMetadataOptions.AffectsRender,
					ChartElementHelper.UpdateWithClearDiagramCache), new ValidateValueCallback(HeightToWidthRatioValidation));
			AxisXProperty = DependencyProperty.Register("AxisX", typeof(AxisX3D), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, AxisXPropertyChanged));
			AxisYProperty = DependencyProperty.Register("AxisY", typeof(AxisY3D), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, AxisYPropertyChanged));
			SeriesDistanceProperty = DependencyProperty.Register("SeriesDistance", typeof(double), ownerType,
				new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, 
					ChartElementHelper.UpdateWithClearDiagramCache), new ValidateValueCallback(SeriesDistanceValidation));
			SeriesPaddingProperty = DependencyProperty.Register("SeriesPadding", typeof(double), ownerType,
				new FrameworkPropertyMetadata(Double.NaN, FrameworkPropertyMetadataOptions.AffectsRender, 
					ChartElementHelper.UpdateWithClearDiagramCache), new ValidateValueCallback(SeriesPaddingValidation));
			PlaneDepthFixedProperty = DependencyProperty.Register("PlaneDepthFixed", typeof(int), ownerType,
				new FrameworkPropertyMetadata(defaultPlaneDepthFixed, FrameworkPropertyMetadataOptions.AffectsRender, 
					ChartElementHelper.UpdateWithClearDiagramCache), new ValidateValueCallback(PlaneDepthFixedValidation));
			MaterialProperty = DependencyProperty.Register("Material", typeof(Material), ownerType,
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, 
					ChartElementHelper.UpdateWithClearDiagramCache));
			BarDistanceProperty = DependencyProperty.Register("BarDistance", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0.0, ChartElementHelper.UpdateWithClearDiagramCache), 
				new ValidateValueCallback(BarDistanceValidation));
			BarDistanceFixedProperty = DependencyProperty.Register("BarDistanceFixed", typeof(int), ownerType,
				new FrameworkPropertyMetadata(1, ChartElementHelper.UpdateWithClearDiagramCache), 
				new ValidateValueCallback(BarDistanceFixedValidation));
			EqualBarWidthProperty = DependencyProperty.Register("EqualBarWidth", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, ChartElementHelper.UpdateWithClearDiagramCache));
		}
		static void AxisXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYDiagram3D diagram = d as XYDiagram3D;
			if (diagram != null) {
				AxisX3D axis = e.NewValue as AxisX3D;
				IAxisData oldAxis = diagram.actualAxisX;
				if (axis == null) {
					diagram.actualAxisX = new AxisX3D();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, diagram.actualAxisX);
				}
				else 
					diagram.actualAxisX = axis;
				ChartElementHelper.ChangeOwner(d, e);
				ChartElementHelper.Update(d, new PropertyUpdateInfo<IAxisData>(diagram, "AxisX", oldAxis, axis), ChartElementChange.ClearDiagramCache);				
			}
		}
		static void AxisYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			XYDiagram3D diagram = d as XYDiagram3D;
			if (diagram != null) {
				IAxisData oldAxis = diagram.actualAxisY;
				AxisY3D axis = e.NewValue as AxisY3D;
				if (axis == null) {
					diagram.actualAxisY = new AxisY3D();
					e = new DependencyPropertyChangedEventArgs(e.Property, e.OldValue, diagram.actualAxisY);
				}
				else 
					diagram.actualAxisY = axis;
				ChartElementHelper.ChangeOwner(d, e);
				ChartElementHelper.Update(d, new PropertyUpdateInfo<IAxisData>(diagram, "AxisY", oldAxis, axis), ChartElementChange.ClearDiagramCache);
			}
		}
		static bool SeriesDistanceValidation(object distance) {
			return Double.IsNaN((double)distance) || (double)distance >= 0;
		}
		static bool SeriesPaddingValidation(object padding) {
			return Double.IsNaN((double)padding) || (double)padding >= 0;
		}
		static bool HeightToWidthRatioValidation(object heightToWidthRatio) {
			return Double.IsNaN((double)heightToWidthRatio) || ((double)heightToWidthRatio >= 0.1 && (double)heightToWidthRatio <= 10);
		}
		static bool PlaneDepthFixedValidation(object depth) {
			return (int)depth > 0;
		}
		static bool BarDistanceValidation(object barDistance) {
			return (double)barDistance >= 0;
		}
		static bool BarDistanceFixedValidation(object barDistanceFixed) {
			return (int)barDistanceFixed >= 0;
		}
		AxisX3D actualAxisX;
		AxisY3D actualAxisY;
		XYDiagram3DCache cache;
		internal double ActualSeriesPadding { get { return Double.IsNaN(SeriesPadding) ? CalculateSeriesPadding() : SeriesPadding; } }
		internal double ActualSeriesDistance { get { return Double.IsNaN(SeriesDistance) ? CalculateSeriesDistance() : SeriesDistance; } }
		internal XYDiagram3DCache Cache { get { return cache; } }
		protected internal override CompatibleViewType CompatibleViewType { get { return CompatibleViewType.XYView; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("XYDiagram3DActualAxisX")]
#endif
		public AxisX3D ActualAxisX { get { return actualAxisX; } }
#if !SL
	[DevExpressXpfChartsLocalizedDescription("XYDiagram3DActualAxisY")]
#endif
		public AxisY3D ActualAxisY { get { return actualAxisY; } }
		#region IPane implementation
		int IPane.PaneIndex { get { return -1; } }
		GRealRect2D? IPane.MappingBounds { get { return null; } }
		#endregion
		#region IXYDiagram implementation
		void IXYDiagram.UpdateCrosshairData(IList<RefinedSeries> seriesCollection) {
		}
		GRealRect2D IDiagram.ChartBounds {
			get { throw new NotImplementedException(); }
		}
		IList<IPane> IXYDiagram.Panes { get { return new IPane[] { this }; } }
		IAxisData IXYDiagram.AxisX { get { return actualAxisX; } }
		IAxisData IXYDiagram.AxisY { get { return actualAxisY; } }
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
			return new GRealPoint2D(0, 0);
		}
		List<IPaneAxesContainer> IXYDiagram.GetPaneAxesContainers(IList<RefinedSeries> activeSeries) {
			PaneAxesContainer paneAxesContainer = new PaneAxesContainer(this);
			paneAxesContainer.RegisterAxisX(0, actualAxisX);
			paneAxesContainer.RegisterAxisY(0, actualAxisY);
			paneAxesContainer.InitializePrimaryAndSecondaryAxes();
			List<IPaneAxesContainer> containers = new List<IPaneAxesContainer>();
			containers.Add(paneAxesContainer);
			return containers;
		}
		void IXYDiagram.UpdateAutoMeasureUnits() {
		}
		int IXYDiagram.GetAxisXLength(IAxisData axis) {
			return 0;
		}
		#endregion
		public XYDiagram3D() : base() {
			actualAxisX = new AxisX3D();
			actualAxisX.ChangeOwner(null, this);
			actualAxisY = new AxisY3D();
			actualAxisY.ChangeOwner(null, this);
			cache = new XYDiagram3DCache();
			EndInit();
		}
		double CalculateSeriesPadding() {
			if (Series.Count > 0)
				return Math.Max(Series[0].SeriesDepth, Series[Series.Count - 1].SeriesDepth) / 2 + 0.05;
			return defaultSeriesPadding;
		} 
		double CalculateSeriesDistance() {
			double maxDepth = 0;
			foreach (Series series in Series)
				if (series.SeriesDepth > maxDepth)
					maxDepth = series.SeriesDepth;
			return maxDepth == 0 ? defaultSeriesDistance : maxDepth + 0.3;
		}
		bool UpdateAxis(AxisBase axis, Size size) {
			double axisLength = axis.IsValuesAxis ? size.Height : size.Width;
			if (axisLength == 0)
				axisLength = defaultAxisLength;
			return axis.UpdateAutomaticMeasureUnit(axisLength);
		}
		protected internal override void ClearCache() {
			base.ClearCache();
			cache.Clear();
		}
		protected internal override Diagram3DDomain CreateDomain(VisualContainer visualContainer) {
			return new XYDiagram3DDomain(this, visualContainer.Bounds);
		}
		IList<AxisBase> UpdateMeasureUnits(Size size) {
			IList<AxisBase> affectedAxes = new List<AxisBase>();
			if (UpdateAxis(ActualAxisX, size))
				affectedAxes.Add(ActualAxisX);
			if (UpdateAxis(ActualAxisY, size))
				affectedAxes.Add(ActualAxisY);
			return affectedAxes;
		}
		protected internal override void CheckMeasureUnits() {
			IList<AxisBase> affectedAxes = UpdateMeasureUnits(ActualViewport.Size);
			ViewController.SendDataAgreggationUpdates(affectedAxes);
		}
		protected override Size MeasureOverride(Size constraint) {
			Size result = base.MeasureOverride(constraint);
			IList<AxisBase> affectedAxes = UpdateMeasureUnits(constraint);
			if (affectedAxes.Count > 0) {
				ViewController.SendDataAgreggationUpdates(affectedAxes);
				ChartElementHelper.Update((IChartElement)this, ChartElementChange.Diagram3DOnly | ChartElementChange.ClearDiagramCache);
			}
			return result;
		}
		internal List<SeriesData> CreateSeriesDataList() {
			List<SeriesData> seriesDataList = new List<SeriesData>();
			foreach (RefinedSeries refinedSeries in ViewController.ActiveRefinedSeries) {
				Series series = (Series)refinedSeries.Series;
				if (series.Points.Count > 0 && series.GetActualVisible())
					seriesDataList.Add(series.CreateSeriesData());
			}
			return seriesDataList;
		}
		internal void ChangeItemBrush(object item, Color brushColor) {
			if (Cache != null)
				Cache.ChangeModelBrush(item, new SolidColorBrush(brushColor));
			InvalidateDiagram();
		}		
	}
}
