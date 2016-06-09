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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
namespace DevExpress.Xpf.Map {
	public enum MapPushpinState {
		Normal,
		Busy
	}
	public class PushpinLocationAnimation : MapDependencyObject {
		#region Dependency properties
		public static readonly DependencyProperty DurationProperty = DependencyPropertyManager.Register("Duration",
			typeof(Duration), typeof(PushpinLocationAnimation), new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(500)), NotifyPropertyChanged));
		public static readonly DependencyProperty EasingFunctionProperty = DependencyPropertyManager.Register("EasingFunction",
			typeof(IEasingFunction), typeof(PushpinLocationAnimation), new PropertyMetadata(null, NotifyPropertyChanged));
		#endregion
		[Category(Categories.Behavior)]
		public Duration Duration {
			get { return (Duration)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		[Category(Categories.Behavior)]
		public IEasingFunction EasingFunction {
			get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
			set { SetValue(EasingFunctionProperty, value); }
		}
		public event EventHandler Completed;
		protected override MapDependencyObject CreateObject() {
			return new PushpinLocationAnimation();
		}
		internal void RaiseAnimationCompleted(EventArgs e) {
			if (Completed != null)
				Completed(this, e);
		}
	}
	public class MapPushpin : MapItem, IWeakEventListener, ISupportCoordLocation, IPointCore, IClusterable, IClusterItem {
		const string templateSource = "<local:PushpinControl ItemInfo=\"{Binding}\" Visibility=\"{Binding Path=MapItem.Visible, Converter={local:BoolToVisibilityConverter}}\"/>";
		const string markerTemplateSource = "<local:PushpinMarkerControl/>";
		#region Dependency properties
		public static readonly DependencyProperty TemplateProperty = DependencyPropertyManager.Register("Template",
			typeof(ControlTemplate), typeof(MapPushpin), new PropertyMetadata(GetDefaultTemplate()));
		public static readonly DependencyProperty MarkerTemplateProperty = DependencyPropertyManager.Register("MarkerTemplate",
			typeof(DataTemplate), typeof(MapPushpin), new PropertyMetadata(GetDefaultMarkerTemplate(), NotifyPropertyChanged));
		public static readonly DependencyProperty LocationProperty = DependencyPropertyManager.Register("Location",
			typeof(CoordPoint), typeof(MapPushpin), new PropertyMetadata(new GeoPoint(0, 0), LocationPropertyChanged, CoerceLocation));
		public static readonly DependencyProperty LocationChangedAnimationProperty = DependencyPropertyManager.Register("LocationChangedAnimation",
			typeof(PushpinLocationAnimation), typeof(MapPushpin), new PropertyMetadata(null, LocationChangedAnimationPropertyChanged));
		public static readonly DependencyProperty InformationProperty = DependencyPropertyManager.Register("Information",
			typeof(object), typeof(MapPushpin), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty TraceDepthProperty = DependencyPropertyManager.Register("TraceDepth",
			typeof(int), typeof(MapPushpin), new PropertyMetadata(0, TraceDepthPropertyChanged));
		public static readonly DependencyProperty TraceStrokeProperty = DependencyPropertyManager.Register("TraceStroke",
			typeof(Brush), typeof(MapPushpin), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF)), TraceStrokePropertyChanged));
		public static readonly DependencyProperty TraceStrokeStyleProperty = DependencyPropertyManager.Register("TraceStrokeStyle",
			typeof(StrokeStyle), typeof(MapPushpin), new PropertyMetadata(null, TraceStrokePropertyChanged));
		public static readonly DependencyProperty StateProperty = DependencyPropertyManager.Register("State",
			typeof(MapPushpinState), typeof(MapPushpin), new PropertyMetadata(MapPushpinState.Normal, NotifyPropertyChanged));
		public static readonly DependencyProperty IsHighlightedProperty = DependencyPropertyManager.Register("IsHighlighted",
			typeof(bool), typeof(MapPushpin), new PropertyMetadata(false, NotifyPropertyChanged));
		public static readonly DependencyProperty BrushProperty = DependencyPropertyManager.Register("Brush",
			typeof(Brush), typeof(MapPushpin), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xFF, 0x8A, 0xFB, 0xFF)), NotifyPropertyChanged));
		public static readonly DependencyProperty TextProperty = DependencyPropertyManager.Register("Text",
			typeof(string), typeof(MapPushpin), new PropertyMetadata(string.Empty, NotifyPropertyChanged));
		public static readonly DependencyProperty TextBrushProperty = DependencyPropertyManager.Register("TextBrush",
			typeof(Brush), typeof(MapPushpin), new PropertyMetadata(new SolidColorBrush(Colors.Black), NotifyPropertyChanged));
		#endregion
		static object CoerceLocation(DependencyObject d, object baseValue) {
			if (baseValue == null)
				return new GeoPoint(0, 0);
			return baseValue;
		}
		[Category(Categories.Common)]
		public MapPushpinState State {
			get { return (MapPushpinState)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		[Category(Categories.Data)]
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		[Category(Categories.Presentation)]
		public ControlTemplate Template {
			get { return (ControlTemplate)GetValue(TemplateProperty); }
			set { SetValue(TemplateProperty, value); }
		}
		[Category(Categories.Presentation)]
		public object Information {
			get { return GetValue(InformationProperty); }
			set { SetValue(InformationProperty, value); }
		}
		[Category(Categories.Layout), TypeConverter(typeof(GeoPointConverter))]
		public CoordPoint Location {
			get { return (CoordPoint)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
		[Category(Categories.Behavior)]
		public PushpinLocationAnimation LocationChangedAnimation {
			get { return (PushpinLocationAnimation)GetValue(LocationChangedAnimationProperty); }
			set { SetValue(LocationChangedAnimationProperty, value); }
		}
		[Category(Categories.Appearance)]
		public int TraceDepth {
			get { return (int)GetValue(TraceDepthProperty); }
			set { SetValue(TraceDepthProperty, value); }
		}
		[Category(Categories.Appearance)]
		public StrokeStyle TraceStrokeStyle {
			get { return (StrokeStyle)GetValue(TraceStrokeStyleProperty); }
			set { SetValue(TraceStrokeStyleProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush TraceStroke {
			get { return (Brush)GetValue(TraceStrokeProperty); }
			set { SetValue(TraceStrokeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public DataTemplate MarkerTemplate {
			get { return (DataTemplate)GetValue(MarkerTemplateProperty); }
			set { SetValue(MarkerTemplateProperty, value); }
		}
		[Category(Categories.Appearance)]
		public bool IsHighlighted {
			get { return (bool)GetValue(IsHighlightedProperty); }
			set { SetValue(IsHighlightedProperty, value); }
		}
		[Category(Categories.Appearance)]
		new public Brush Brush {
			get { return (Brush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush TextBrush {
			get { return (Brush)GetValue(TextBrushProperty); }
			set { SetValue(TextBrushProperty, value); }
		}
		IList<IClusterable> clusteredItems = new List<IClusterable>();
		readonly PushpinAnimationRunner animationRunner;
		CoordPoint actualLocation = new GeoPoint();
		CoordPoint motionStartLocation;
		Queue<CoordPoint> traceLocations;
		PushpinAnimationProgress animationProgress;
		internal VectorLayerBase VectorLayerBase { get { return Layer as VectorLayerBase; } }
		internal PushpinAnimationProgress Progress { get { return animationProgress; } }
		internal CoordPoint ActualLocation {
			get { return actualLocation; }
			set { actualLocation = value; }
		}
		internal Queue<CoordPoint> TraceLocations { get { return traceLocations; } }
		internal Panel ContainerPanel { get; set; }
		internal bool EnableHighlighting { 
			get {
				if (VectorLayerBase == null || VectorLayerBase.Map == null || VectorLayerBase.Map.NavigationController == null)
					return false;
				return VectorLayerBase.EnableHighlighting && !VectorLayerBase.Map.NavigationController.IsRegionSelecting;
			} 
		}
		protected override bool IsVisualElement { get { return false; } }
		protected internal override ControlTemplate ItemTemplate {
			get {
				return Template;
			}
		}
		internal event EventHandler TraceChanged;
		internal static ControlTemplate GetDefaultTemplate() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			return XamlHelper.GetControlTemplate(templateSource);
		}
		internal static DataTemplate GetDefaultMarkerTemplate() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			return XamlHelper.GetTemplate(markerTemplateSource);
		}
		static void LocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPushpin pushpin = d as MapPushpin;
			if (pushpin != null)
				pushpin.LocationChanged((CoordPoint)e.NewValue, (CoordPoint)e.OldValue);
		}
		static void LocationChangedAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPushpin pushpin = d as MapPushpin;
			PushpinLocationAnimation newAnimation = e.NewValue as PushpinLocationAnimation;
			PushpinLocationAnimation oldAnimation = e.OldValue as PushpinLocationAnimation;
			if (oldAnimation != null)
				oldAnimation.Completed -= pushpin.ProgressCompleted;
			if (newAnimation != null)
				newAnimation.Completed += pushpin.ProgressCompleted;
		}
		static void TraceDepthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPushpin pushpin = d as MapPushpin;
			if (pushpin != null)
				pushpin.UpdateTrace();
		}
		static void TraceStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPushpin pushpin = d as MapPushpin;
			if (pushpin != null)
				pushpin.UpdateTrace();
		}
		static void TraceStrokeStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPushpin pushpin = d as MapPushpin;
			if (pushpin != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as StrokeStyle, e.NewValue as StrokeStyle, pushpin);
				pushpin.UpdateTrace();
			}
		}
		public MapPushpin() {
			animationProgress = new PushpinAnimationProgress(this);
			traceLocations = new Queue<CoordPoint>();
			animationRunner = new PushpinAnimationRunner(this);
		}
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		#region IClusterItem, IClusterable implementation
		IClusterItem IClusterable.CreateInstance() {
			return new MapPushpin();
		}
		IMapUnit IClusterItemCore.GetUnitLocation() {
			return Layout.LocationInMapUnits;
		}
		object IClusterItemCore.Owner {
			get {
				return ((IOwnedElement)this).Owner;
			}
			set {
				SetOwnerInternal(value);
			}
		}
		void IClusterItem.ApplySize(double size) {
		}
		IList<IClusterable> IClusterItem.ClusteredItems { get { return clusteredItems; } set { clusteredItems = value; } }
		#endregion
		bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if ((sender is StrokeStyle)) {
					UpdateTrace();
					success = true;
				}
			}
			return success;
		}
		void TraceLocation(CoordPoint point) {
			if ((traceLocations.Count >= TraceDepth) && (TraceLocations.Count > 0))
				traceLocations.Dequeue();
			if (TraceDepth > 0)
				traceLocations.Enqueue(point);
			UpdateTrace();
		}
		void RaiseTraceChanged() {
			if (TraceChanged != null)
				TraceChanged(this, new EventArgs());
			Invalidate();
		}
		void StartAnimation(CoordPoint start) {
			this.motionStartLocation = start;
			animationRunner.Run();
		}
		void Invalidate() {
			if (ContainerPanel != null)
				ContainerPanel.InvalidateMeasure();
			UpdateLayout();
		}
		void UpdateTrace() {
			int removeCount = traceLocations.Count - TraceDepth;
			while (removeCount > 0) {
				traceLocations.Dequeue();
				removeCount--;
			}
			RaiseTraceChanged();
		}
		void LocationChanged(CoordPoint newLocation, CoordPoint oldLocation) {
			if (LocationChangedAnimation != null) {
				TraceLocation(oldLocation);
				StartAnimation(oldLocation);
			}
			else {
				ActualLocation = newLocation;
				TraceLocation(oldLocation);
			}
		}
		void ProgressCompleted(object sender, EventArgs e) {
			ActualLocation = Location;
			UpdateTrace();
		}		
		protected override MapDependencyObject CreateObject() {
			return new MapPushpin();
		}
		protected override void CalculateLayoutInMapUnits() {
			Rect bounds = GetBoundsInMapUnits();
			Layout.LocationInMapUnits = new MapUnit(bounds.X, bounds.Y);
			Layout.SizeInMapUnits = new Size(bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
		}
		protected override void CalculateLayout() {
			Point leftTop = Layer.MapUnitToScreenZeroOffset(Layout.LocationInMapUnits);
			Point rightBottom = Layer.MapUnitToScreenZeroOffset(new MapUnit(Layout.LocationInMapUnits.X + Layout.SizeInMapUnits.Width, Layout.LocationInMapUnits.Y + Layout.SizeInMapUnits.Height));
			Layout.LocationInPixels = new Point(leftTop.X, leftTop.Y);
			Layout.SizeInPixels = new Size(Math.Abs(rightBottom.X - leftTop.X), Math.Abs(rightBottom.Y - leftTop.Y));
		}
		protected override string GetTextCore() {
			return Text;
		}
		protected internal override CoordBounds CalculateBounds() {
			return new CoordBounds(Location, Location);
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return new CoordPoint[] { Location };
		}
		protected internal override void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(sender, e);
			if (EnableHighlighting && !IsHighlighted)
				IsHighlighted = true;
		}
		protected internal override void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
			base.OnMouseLeave(sender, e);
			if (IsHighlighted)
				IsHighlighted = false;
		}
		internal void ProgressChanged(double progress) {
			if (Layer != null) {
				CoordPoint location = Location;
				double y = motionStartLocation.GetY() + (location.GetY() - motionStartLocation.GetY()) * progress;
				double x = motionStartLocation.GetX() + (location.GetX() - motionStartLocation.GetX()) * progress;
				ActualLocation = CoordinateSystem.PointFactory.CreatePoint(x, y);
				UpdateTrace();
			}
			else {
				animationRunner.Stop();
			}
		}
		internal Rect GetBoundsInMapUnits() {
			MapUnit min = CoordinateSystem.CoordPointToMapUnit(ActualLocation);
			MapUnit max = min;
			foreach (CoordPoint trace in traceLocations) {
				MapUnit current = CoordinateSystem.CoordPointToMapUnit(trace);
				if (current.X > max.X) max.X = current.X;
				if (current.Y > max.Y) max.Y = current.Y;
				if (current.X < min.X) min.X = current.X;
				if (current.Y < min.Y) min.Y = current.Y;
			}
			return new Rect(new Point(min.X, min.Y), new Size(max.X - min.X, max.Y - min.Y));
		}
	}
	[
	TemplatePart(Name = "PART_Container", Type = typeof(Panel)),
	TemplatePart(Name = "PART_Trace", Type = typeof(PushpinTraceControl)),
	TemplatePart(Name = "PART_Marker", Type = typeof(ContentPresenter)),
	NonCategorized
	]
	public class PushpinControl : Control {
		#region Dependency properties
		public static readonly DependencyProperty ItemInfoProperty = DependencyPropertyManager.Register("ItemInfo",
			typeof(MapItemInfo), typeof(PushpinControl), new PropertyMetadata(null, ItemPropertyChanged));
		public static readonly DependencyProperty StateProperty = DependencyPropertyManager.Register("State",
			typeof(MapPushpinState), typeof(PushpinControl), new PropertyMetadata(MapPushpinState.Normal));
		#endregion
		public MapItemInfo ItemInfo {
			get { return (MapItemInfo)GetValue(ItemInfoProperty); }
			set { SetValue(ItemInfoProperty, value); }
		}
		public MapPushpinState State {
			get { return (MapPushpinState)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		static void ItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PushpinControl control = d as PushpinControl;
			if (control != null)
				control.Item = e.NewValue is MapItemInfo ? ((MapItemInfo)e.NewValue).MapItem as MapPushpin : null;				
		}
		MapPushpin item;
		MapPushpin Item {
			get { return item; }
			set {
				if (item != value) {
					item = value;
					UpdatePanel();
				}
			}
		}
		public PushpinControl() {
			DefaultStyleKey = typeof(PushpinControl);
		}
		void UpdatePanel() {
			if (item != null)
				Item.ContainerPanel = GetTemplateChild("PART_Container") as Panel;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdatePanel();
		}
	}
	[NonCategorized]
	public class PushpinMarkerControl : Control {
		#region Dependency properties
		public static readonly DependencyProperty StateProperty = DependencyPropertyManager.Register("State",
			typeof(MapPushpinState), typeof(PushpinMarkerControl), new PropertyMetadata(MapPushpinState.Normal, UpdateState));
		public static readonly DependencyProperty ItemProperty = DependencyPropertyManager.Register("Item",
			typeof(MapItem), typeof(PushpinMarkerControl), new PropertyMetadata(null, ItemPropertyChanged));
		public static readonly DependencyProperty IsHighlightedProperty = DependencyPropertyManager.Register("IsHighlighted",
			typeof(bool), typeof(PushpinMarkerControl), new PropertyMetadata(false, UpdateState));
		public static readonly DependencyProperty IsSelectedProperty = DependencyPropertyManager.Register("IsSelected",
			typeof(bool), typeof(PushpinMarkerControl), new PropertyMetadata(false, UpdateState));
		#endregion
		public MapPushpinState State {
			get { return (MapPushpinState)GetValue(StateProperty); }
			set { SetValue(StateProperty, value); }
		}
		public MapItem Item {
			get { return (MapItem)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		public bool IsHighlighted {
			get { return (bool)GetValue(IsHighlightedProperty); }
			set { SetValue(IsHighlightedProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		static void ItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PushpinMarkerControl control = d as PushpinMarkerControl;
			if (control != null)
				control.ItemChanged(e.OldValue as MapPushpin, e.NewValue as MapPushpin);
		}
		static void UpdateState(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PushpinMarkerControl control = d as PushpinMarkerControl;
			if (control != null)
				control.UpdateStates(false);
		}
		public PushpinMarkerControl() {
			DefaultStyleKey = typeof(PushpinMarkerControl);
		}
		void ItemChanged(MapPushpin oldItem, MapPushpin newItem) {
			if ((oldItem != null) && (oldItem.Container == this))
				oldItem.Container = null;
			if (newItem != null)
				newItem.Container = this;
		}
		void UpdateStates(bool useTransitions) {
			switch (State) {
				case MapPushpinState.Busy:
					VisualStateManager.GoToState(this, "Busy", useTransitions);
					break;
				case MapPushpinState.Normal:
					VisualStateManager.GoToState(this, "Normal", useTransitions);
					break;
			}
			if (IsHighlighted)
				VisualStateManager.GoToState(this, "Highlighted", useTransitions);
			else
				VisualStateManager.GoToState(this, "NonHighlighted", useTransitions);
			if (IsSelected)
				VisualStateManager.GoToState(this, "Selected", useTransitions);
			else
				VisualStateManager.GoToState(this, "NotSelected", useTransitions);
		}
		public override void OnApplyTemplate() {
			UpdateStates(false);
			base.OnApplyTemplate();
		}
	}
	[NonCategorized]
	public class PushpinTraceControl : Control {
		#region Dependency properties
		public static readonly DependencyProperty ItemProperty = DependencyPropertyManager.Register("Item",
			typeof(MapItem), typeof(PushpinTraceControl), new PropertyMetadata(null));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(PushpinTraceControl), new PropertyMetadata(true, VisiblePropertyChanged));
		#endregion
		static void VisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PushpinTraceControl control = d as PushpinTraceControl;
			if (control != null)
				control.UpdateVisibility();
		}
		public MapItem Item {
			get { return (MapItem)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		public Polyline Polyline { get { return this.polyline; } }
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		Grid container;
		Polyline polyline;
		public PushpinTraceControl() {
			DefaultStyleKey = typeof(PushpinTraceControl);
			this.DataContextChanged += new DependencyPropertyChangedEventHandler(DataContextPropertyChanged);
		}
		void DataContextPropertyChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs args) {
			MapPushpin newPushpin = args.NewValue is MapItemInfo ? ((MapItemInfo)args.NewValue).MapItem as MapPushpin : null;
			MapPushpin oldPushpin = args.OldValue is MapItemInfo ? ((MapItemInfo)args.OldValue).MapItem as MapPushpin : null;
			if (oldPushpin != null)
				oldPushpin.TraceChanged -= new EventHandler(PushpinTraceChanged);
			if (newPushpin != null)
				newPushpin.TraceChanged += new EventHandler(PushpinTraceChanged);
		}
		void ApplyAppearance(MapPushpin pushpin) {
			if (polyline != null) {
				polyline.Stroke = pushpin.TraceStroke;
				StrokeStyle strokeStyle = pushpin.TraceStrokeStyle;
				if (strokeStyle == null)
					strokeStyle = CreateDefaultStrokeStyle();
				polyline.StrokeThickness = strokeStyle.Thickness;
				polyline.StrokeDashArray = CommonUtils.CloneDoubleCollection(strokeStyle.DashArray);
				polyline.StrokeDashCap = strokeStyle.DashCap;
				polyline.StrokeDashOffset = strokeStyle.DashOffset;
				polyline.StrokeEndLineCap = strokeStyle.EndLineCap;
				polyline.StrokeStartLineCap = strokeStyle.StartLineCap;
				polyline.StrokeLineJoin = strokeStyle.LineJoin;
				polyline.StrokeMiterLimit = strokeStyle.MiterLimit;
				if (((polyline.Stroke is RadialGradientBrush) || (polyline.Stroke is LinearGradientBrush)) && (polyline.Points.Count > 1)) {
					Point first = polyline.Points[0];
					Point last = polyline.Points[polyline.Points.Count - 1];
					RadialGradientBrush radial = polyline.Stroke as RadialGradientBrush;
					if (radial != null) {
						Point origin = new Point();
						origin.X = (first.X < last.X) ? 0.0 : 1.0;
						origin.Y = (first.Y < last.Y) ? 0.0 : 1.0;
						radial.GradientOrigin = origin;
						radial.Center = origin;
					}
					LinearGradientBrush linear = polyline.Stroke as LinearGradientBrush;
					if (linear != null) {
						Point start = new Point();
						Point end = new Point();
						start.X = (first.X < last.X) ? 0.0 : 1.0;
						start.Y = (first.Y < last.Y) ? 0.0 : 1.0;
						end.X = (first.X < last.X) ? 1.0 : 0.0;
						end.Y = (first.Y < last.Y) ? 1.0 : 0.0;
						linear.StartPoint = start;
						linear.EndPoint = end;
					}
				}
			}
		}
		void MakeTracePolyline() {
			polyline = new Polyline();
			polyline.Stretch = Stretch.Fill;
			container.Children.Add(polyline);
		}
		void DeleteTracePolyline() {
			if (polyline != null) {
				container.Children.Remove(polyline);
				polyline = null;
			}
		}
		void PushpinTraceChanged(object sender, EventArgs args) {
			MapPushpin pushpin = sender as MapPushpin;
			if (pushpin != null) {
				StrokeStyle strokeStyle = pushpin.TraceStrokeStyle;
				if (pushpin.TraceLocations.Count > 0) {
					if (polyline == null)
						MakeTracePolyline();
					polyline.Points.Clear();
					Rect boundsInMapUnits = pushpin.GetBoundsInMapUnits();
					foreach (CoordPoint location in pushpin.TraceLocations) {
						MapUnit locationInMapUnits = pushpin.CoordinateSystem.CoordPointToMapUnit(location);
						Point relativeLocation = new Point(locationInMapUnits.X - boundsInMapUnits.X, locationInMapUnits.Y - boundsInMapUnits.Y);
						polyline.Points.Add(relativeLocation);
					}
					MapUnit pointerLocation = pushpin.CoordinateSystem.CoordPointToMapUnit(pushpin.ActualLocation);
					polyline.Points.Add(new Point(pointerLocation.X - boundsInMapUnits.X, pointerLocation.Y - boundsInMapUnits.Y));
					ApplyAppearance(pushpin);
				}
				else
					DeleteTracePolyline();
				UpdateVisibility();
			}
		}
		void UpdateVisibility() {
			Visibility visibility = Visible ? Visibility.Visible : Visibility.Collapsed;
			if (Polyline != null)
				Polyline.Visibility = visibility;
			this.Visibility = visibility;
		}
		StrokeStyle CreateDefaultStrokeStyle() {
			return new StrokeStyle();
		}
		public override void OnApplyTemplate() {
			container = GetTemplateChild("PART_Canvas") as Grid;
			base.OnApplyTemplate();
		}
	}
	[NonCategorized]
	public class PushpinPanel : Panel {
		const double defaultWidth = 1.0;
		const double defaultHeight = 1.0;
		protected override Size MeasureOverride(Size availableSize) {
			double constraintWidth = double.IsInfinity(availableSize.Width) ? defaultWidth : availableSize.Width;
			double constraintHeight = double.IsInfinity(availableSize.Height) ? defaultHeight : availableSize.Height;
			Size constraint = new Size(constraintWidth, constraintHeight);
			foreach (UIElement child in Children) {
				child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			}
			return constraint;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			ArrangeMarker(GetChildElement("PART_Marker") as ContentPresenter);
			ArrangeTrace(GetChildElement("PART_Trace") as PushpinTraceControl);
			return arrangeSize;
		}
		protected FrameworkElement GetChildElement(string name) {
			foreach (UIElement child in Children) {
				FrameworkElement element = child as FrameworkElement;
				if ((element != null) && (element.Name == name))
					return element;
			}
			return null;
		}
		protected void ArrangeMarker(ContentPresenter element) {
			if (element != null) {
				MapItemInfo info = element.Content as MapItemInfo;
				MapPushpin pushpin = info != null ? info.MapItem as MapPushpin : null;
				if (pushpin != null) {
					IMapItem mapItem = pushpin as IMapItem;
					Point pointerLocation = pushpin.Layer.CoordPointToScreenPointZeroOffset(pushpin.ActualLocation, true, true);
					Point relativeLocation = new Point(pointerLocation.X - mapItem.Location.X, pointerLocation.Y - mapItem.Location.Y);
					element.Arrange(new Rect(relativeLocation, element.DesiredSize));
				}
			}
		}
		protected void ArrangeTrace(PushpinTraceControl traceControl) {
			if ((traceControl != null) && (traceControl.Item != null)) {
				MapPushpin pushpin = traceControl.Item as MapPushpin;
				if (pushpin != null && pushpin.TraceLocations.Count > 0) {
					Rect bounds = pushpin.GetBoundsInMapUnits();
					Point location = pushpin.Layer.MapUnitToScreenZeroOffset(new MapUnit(bounds.X, bounds.Y));
					Point size = pushpin.Layer.MapUnitToScreenZeroOffset(new MapUnit(bounds.Width + bounds.X, bounds.Height + bounds.Y));
					double lineThickness = 0;
					if ((traceControl.Polyline != null) && (traceControl.Polyline.StrokeThickness > 1))
						lineThickness = traceControl.Polyline.StrokeThickness;
					traceControl.Arrange(new Rect(new Point(-lineThickness / 2, -lineThickness / 2), new Size(size.X - location.X + lineThickness, size.Y - location.Y + lineThickness)));
				}
				else
					traceControl.Arrange(new Rect(0, 0, 0, 0));
			}
		}
	}
	[NonCategorized]
	public class NullToVisibilityConverter : IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (targetType == typeof(Visibility))
				return (value != null) ? Visibility.Visible : Visibility.Collapsed;
			return Visibility.Collapsed;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotSupportedException();
		}
		#endregion
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class PushpinAnimationProgress : DependencyObject {
		#region Dependency properties
		public static readonly DependencyProperty ProgressProperty = DependencyPropertyManager.Register("Progress",
			typeof(double), typeof(PushpinAnimationProgress), new PropertyMetadata(0.0, ProgressPropertyChanged));
		#endregion
		public double Progress {
			get { return (double)GetValue(ProgressProperty); }
			set { SetValue(ProgressProperty, value); }
		}
		MapPushpin Target { get; set; }
		static void ProgressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PushpinAnimationProgress progress = d as PushpinAnimationProgress;
			if (progress != null)
				progress.Target.ProgressChanged((double)e.NewValue);
		}
		public PushpinAnimationProgress(MapPushpin target) {
			Target = target;
		}
		public void Finish() {
			if (Progress != 0.0)
				Progress = 1.0;
		}
	}
	public class PushpinAnimationRunner {
		readonly MapPushpin pushpin;
		Storyboard storyboard;
		bool storyboardStarted = false;
		PushpinLocationAnimation AnimationSettings { get { return pushpin.LocationChangedAnimation; } }
		public PushpinAnimationRunner(MapPushpin pushpin) {
			this.pushpin = pushpin;
			storyboardStarted = false;
			storyboard = new Storyboard();
			storyboard.Completed += new EventHandler(StoryboardCompleted);
		}
		void StoryboardCompleted(object sender, EventArgs e) {
			AnimationSettings.RaiseAnimationCompleted(e);
		}
		internal void Run() {
			Stop();
			pushpin.Progress.Finish();
			DoubleAnimation animation = new DoubleAnimation() {
				From = 0.0,
				To = 1.0,
				Duration = AnimationSettings.Duration,
				EasingFunction = AnimationSettings.EasingFunction
			};
			Storyboard.SetTarget(animation, pushpin.Progress);
			Storyboard.SetTargetProperty(animation, new PropertyPath(PushpinAnimationProgress.ProgressProperty));
			storyboard.Children.Clear();
			storyboard.Children.Add(animation);
			storyboardStarted = true;
			storyboard.Begin();
		}
		internal void Stop() {
			if (storyboardStarted)
				storyboard.Stop();
		}
	}
}
