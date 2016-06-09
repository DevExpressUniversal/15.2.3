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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Map.Native;
using DevExpress.Map;
namespace DevExpress.Xpf.Map {
	public abstract class MapShapeBase : MapItem, IWeakEventListener, IColorizerElement, ISupportStyleCore, IMapShapeStyleCore {
		protected const double ShapeScale = 1;
		#region Dependency properties
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
			typeof(Brush), typeof(MapShapeBase), new PropertyMetadata(null, AppearancePropertyChanged));
		public static readonly DependencyProperty StrokeProperty = DependencyPropertyManager.Register("Stroke",
			typeof(Brush), typeof(MapShapeBase), new PropertyMetadata(null, AppearancePropertyChanged));
		public static readonly DependencyProperty StrokeStyleProperty = DependencyPropertyManager.Register("StrokeStyle",
			typeof(StrokeStyle), typeof(MapShapeBase), new PropertyMetadata(null, StrokeStylePropertyChanged));
		public static readonly DependencyProperty HighlightFillProperty = DependencyPropertyManager.Register("HighlightFill",
			typeof(Brush), typeof(MapShapeBase), new PropertyMetadata(null));
		public static readonly DependencyProperty HighlightStrokeProperty = DependencyPropertyManager.Register("HighlightStroke",
			typeof(Brush), typeof(MapShapeBase), new PropertyMetadata(null));
		public static readonly DependencyProperty HighlightStrokeStyleProperty = DependencyPropertyManager.Register("HighlightStrokeStyle",
			typeof(StrokeStyle), typeof(MapShapeBase), new PropertyMetadata(null));
		public static readonly DependencyProperty SelectedFillProperty = DependencyPropertyManager.Register("SelectedFill",
			typeof(Brush), typeof(MapShapeBase), new PropertyMetadata(null, AppearancePropertyChanged));
		public static readonly DependencyProperty SelectedStrokeProperty = DependencyPropertyManager.Register("SelectedStroke",
			typeof(Brush), typeof(MapShapeBase), new PropertyMetadata(null, AppearancePropertyChanged));
		public static readonly DependencyProperty SelectedStrokeStyleProperty = DependencyPropertyManager.Register("SelectedStrokeStyle",
			typeof(StrokeStyle), typeof(MapShapeBase), new PropertyMetadata(null, StrokeStylePropertyChanged));
		public static readonly DependencyProperty TitleOptionsProperty = DependencyPropertyManager.Register("TitleOptions",
			typeof(ShapeTitleOptions), typeof(MapShapeBase), new PropertyMetadata(null, TitleOptionsPropertyChanged));
		public static readonly DependencyProperty EffectProperty = DependencyPropertyManager.Register("Effect",
			typeof(Effect), typeof(MapShapeBase), new PropertyMetadata(null, EffectPropertyChanged));
		internal static readonly DependencyPropertyKey TitlePropertyKey = DependencyPropertyManager.RegisterReadOnly("Title",
			typeof(ShapeTitle), typeof(MapShapeBase), new PropertyMetadata());
		public static readonly DependencyProperty TitleProperty = TitlePropertyKey.DependencyProperty;
		#endregion
		static void StrokeStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapShapeBase shape = d as MapShapeBase;
			if (shape != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as StrokeStyle, e.NewValue as StrokeStyle, shape);
				shape.ApplyAppearance();
			}
		}
		static void TitleOptionsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapShapeBase shape = d as MapShapeBase;
			if (shape != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as ShapeTitleOptions, e.NewValue as ShapeTitleOptions, shape);
				shape.ApplyTitleOptions();
			}
		}
		static void EffectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapShapeBase shape = d as MapShapeBase;
			if ((shape != null) && (shape.StyleProvider != null))
				shape.StyleProvider.Effect = e.NewValue as Effect;
		}
		protected static void AppearancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapShapeBase shape = d as MapShapeBase;
			if (shape != null)
				shape.ApplyAppearance();
		}
		ShapeTitleOptions ActualTitleOptions {
			get {
				ShapeTitleOptions result = VectorLayer != null && TitleOptions == null ? VectorLayer.ShapeTitleOptions : TitleOptions;
				if (result == null)
					result = CreateDefaultTitleOptions();
				return result;
			}
		}
		protected abstract IMapItemStyleProvider StyleProvider { get; }
		protected override bool IsVisualElement { get { return false; } }
		protected Brush ActualStroke { get { return VectorLayer != null && Stroke == null ? VectorLayer.ShapeStroke : Stroke; } }
		protected StrokeStyle ActualStrokeStyle {
			get {
				if (Info == null) {
					return GetRegularStrokeStyle();
				}
				else if (!Info.IsHighlighting && !Info.IsSelected) {
					return GetRegularStrokeStyle();
				}
				else if (Info.IsHighlighting && !Info.IsSelected)
					return ActualHighlightStrokeStyle;
				else
					return ActualSelectedStrokeStyle;
			}
		}
		protected Brush ActualHighlightFill { get { return VectorLayer != null && HighlightFill == null ? VectorLayer.HighlightShapeFill : HighlightFill; } }
		protected Brush ActualHighlightStroke { get { return VectorLayer != null && HighlightStroke == null ? VectorLayer.HighlightShapeStroke : HighlightStroke; } }
		protected StrokeStyle ActualHighlightStrokeStyle {
			get {
				StrokeStyle result = VectorLayer != null && HighlightStrokeStyle == null ? VectorLayer.HighlightShapeStrokeStyle : HighlightStrokeStyle;
				if (result == null)
					result = CreateDefaultHighlightStrokeStyle();
				return result;
			}
		}
		protected Brush ActualSelectedFill {
			get {
				return VectorLayer != null && SelectedFill == null ? VectorLayer.SelectedShapeFill : SelectedFill;
			}
		}
		protected Brush ActualSelectedStroke { get { return VectorLayer != null && SelectedStroke == null ? VectorLayer.SelectedShapeStroke : SelectedStroke; } }
		protected StrokeStyle ActualSelectedStrokeStyle {
			get {
				StrokeStyle result = VectorLayer != null && SelectedStrokeStyle == null ? VectorLayer.SelectedShapeStrokeStyle : SelectedStrokeStyle;
				if (result == null)
					result = CreateDefaultSelectedStrokeStyle();
				return result;
			}
		}
		protected VectorLayer VectorLayer { 
			get { return Layer as VectorLayer; }
		}
		internal bool EnableHighlighting {
			get {
				if (VectorLayer == null || VectorLayer.Map == null || VectorLayer.Map.NavigationController == null)
					return false;
				return VectorLayer.EnableHighlighting && !VectorLayer.Map.NavigationController.IsRegionSelecting;
			}
		}
		protected internal virtual Brush ActualFill {
			get { return GetActualBrush(); } }
		[Category(Categories.Appearance)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush Stroke {
			get { return (Brush)GetValue(StrokeProperty); }
			set { SetValue(StrokeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public StrokeStyle StrokeStyle {
			get { return (StrokeStyle)GetValue(StrokeStyleProperty); }
			set { SetValue(StrokeStyleProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush HighlightFill {
			get { return (Brush)GetValue(HighlightFillProperty); }
			set { SetValue(HighlightFillProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush HighlightStroke {
			get { return (Brush)GetValue(HighlightStrokeProperty); }
			set { SetValue(HighlightStrokeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public StrokeStyle HighlightStrokeStyle {
			get { return (StrokeStyle)GetValue(HighlightStrokeStyleProperty); }
			set { SetValue(HighlightStrokeStyleProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush SelectedFill {
			get { return (Brush)GetValue(SelectedFillProperty); }
			set { SetValue(SelectedFillProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Brush SelectedStroke {
			get { return (Brush)GetValue(SelectedStrokeProperty); }
			set { SetValue(SelectedStrokeProperty, value); }
		}
		[Category(Categories.Appearance)]
		public StrokeStyle SelectedStrokeStyle {
			get { return (StrokeStyle)GetValue(SelectedStrokeStyleProperty); }
			set { SetValue(SelectedStrokeStyleProperty, value); }
		}
		[Category(Categories.Appearance)]
		public Effect Effect {
			get { return (Effect)GetValue(EffectProperty); }
			set { SetValue(EffectProperty, value); }
		}
		[Category(Categories.Presentation)]
		public ShapeTitleOptions TitleOptions {
			get { return (ShapeTitleOptions)GetValue(TitleOptionsProperty); }
			set { SetValue(TitleOptionsProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), NonTestableProperty]
		public ShapeTitle Title {
			get { return (ShapeTitle)GetValue(TitleProperty); }
		}
		public MapShapeBase() {
			this.SetValue(TitlePropertyKey, new ShapeTitle(this));
		}
		void SetStrokeWidth(double width) {
			if (StrokeStyle == null)
				StrokeStyle = CreateDefaultStrokeStyle();
			if (HighlightStrokeStyle == null)
				HighlightStrokeStyle = CreateDefaultHighlightStrokeStyle();
			if (SelectedStrokeStyle == null)
				SelectedStrokeStyle = CreateDefaultSelectedStrokeStyle();
			StrokeStyle.Thickness = width;
			HighlightStrokeStyle.Thickness = width;
			SelectedStrokeStyle.Thickness = width;
		}
		void SetFill(System.Drawing.Color color) {
			SolidColorBrush brush = color != null ? new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B)) : null;
			SetValue(FillProperty, brush);
		}
		void SetStroke(System.Drawing.Color color) {
			SolidColorBrush brush = color != null ? new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B)) : null;
			SetValue(StrokeProperty, brush);
		}
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		#region IColorizerElement Members
		Color IColorizerElement.ColorizerColor {
			get {
				return ColorizerColor;
			}
			set {
				ColorizerColor = value;
			}
		}
		#endregion
		#region ISupportStyleCore
		void ISupportStyleCore.SetStrokeWidth(double width) {
			SetStrokeWidth(width);
		}
		void ISupportStyleCore.SetFill(System.Drawing.Color color) {
			SetFill(color);
		}
		void ISupportStyleCore.SetStroke(System.Drawing.Color color) {
			SetStroke(color);
		}
		#endregion
		#region IMapShapeStyleCore implementation
		System.Drawing.Color IMapShapeStyleCore.Fill {
			get {
				SolidColorBrush fill = ActualFill as SolidColorBrush;
				if (fill != null)
					return fill.Color.ToWinFormsColor();
				return System.Drawing.Color.Empty;
			}
			set { 
				if (!value.IsEmpty) 
					SetFill(value); 
			}
		}
		System.Drawing.Color IMapShapeStyleCore.Stroke {
			get {
				SolidColorBrush stroke = ActualStroke as SolidColorBrush;
				if (stroke != null)
					return stroke.Color.ToWinFormsColor();
				return System.Drawing.Color.Empty;
			}
			set {
				if (!value.IsEmpty) 
					SetStroke(value); 
			}
		}
		double IMapShapeStyleCore.StrokeWidth {
			get { return ActualStrokeStyle.Thickness; }
			set { SetStrokeWidth(value); }
		}
		#endregion
		bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if(managerType == typeof(PropertyChangedWeakEventManager)) {
				if((sender is StrokeStyle)) {
					ApplyAppearance();
					success = true;
				} else if((sender is ShapeTitleOptions)) {
					ApplyTitleOptions();
					success = true;
				}
			}
			return success;
		}
		StrokeStyle CreateDefaultStrokeStyle() {
			return new StrokeStyle();
		}
		StrokeStyle CreateDefaultHighlightStrokeStyle() {
			return new StrokeStyle(); 
		}
		StrokeStyle CreateDefaultSelectedStrokeStyle() {
			return new StrokeStyle() { Thickness = 2 };
		}
		ShapeTitleOptions CreateDefaultTitleOptions() {
			return new ShapeTitleOptions();
		}
		StrokeStyle GetRegularStrokeStyle() {
			StrokeStyle style = VectorLayer != null && StrokeStyle == null ? VectorLayer.ShapeStrokeStyle : StrokeStyle;
			if (style == null)
				style = CreateDefaultStrokeStyle();
			return style;
		}
		protected virtual Brush CalculateShapeFill() {
			Brush brush = null;
			if(Info != null && Info.IsSelected)
				brush = ActualSelectedFill;
			return brush != null ? brush : ActualFill;
		}
		protected virtual Brush CalculateShapeStroke() {
			Brush stroke = null;
			if(Info != null && Info.IsSelected)
				stroke = ActualSelectedStroke;
			return stroke != null ? stroke : ActualStroke;
		}
		protected virtual StrokeStyle CalculateShapeStrokeStyle() {
			if(Info != null && Info.IsSelected)
				return ActualSelectedStrokeStyle;
			return ActualStrokeStyle;
		}
		protected override Brush GetFill() {
			return VectorLayer != null && Fill == null ? VectorLayer.ShapeFill : Fill;
		}
		protected override void CalculateLayout() {
			Point p1 = Layer.MapUnitToScreenZeroOffset(Layout.LocationInMapUnits);
			Point p2 = Layer.MapUnitToScreenZeroOffset(new MapUnit(Layout.LocationInMapUnits.X + Layout.SizeInMapUnits.Width, Layout.LocationInMapUnits.Y + Layout.SizeInMapUnits.Height));
			double strokeThickness = ActualStrokeStyle != null ? ActualStrokeStyle.Thickness : 0.0;
			Layout.SizeInPixels = new Size(Math.Max(strokeThickness, Math.Abs(p2.X - p1.X)), Math.Max(strokeThickness, Math.Abs(p2.Y - p1.Y)));
			Layout.LocationInPixels = p1;
		}
		protected override void UpdateVisibility() {
			base.UpdateVisibility();
			if (StyleProvider != null)
				if (Visible)
					StyleProvider.Visibility = Visibility.Visible;
				else
					StyleProvider.Visibility = Visibility.Collapsed;
		}
		protected override string GetTextCore() {
			return Title.Text;
		}
		protected internal virtual void ApplyHighlightAppearance() {
			if (StyleProvider != null) {
				if (ActualHighlightFill != null)
					StyleProvider.Fill = ActualHighlightFill;
				StyleProvider.Stroke = ActualHighlightStroke;
				if (ActualHighlightStrokeStyle != null) {
					StyleProvider.StrokeThickness = ActualHighlightStrokeStyle.Thickness;
					StyleProvider.StrokeDashArray = ActualHighlightStrokeStyle.DashArray;
					StyleProvider.StrokeDashCap = ActualHighlightStrokeStyle.DashCap;
					StyleProvider.StrokeDashOffset = ActualHighlightStrokeStyle.DashOffset;
					StyleProvider.StrokeEndLineCap = ActualHighlightStrokeStyle.EndLineCap;
					StyleProvider.StrokeStartLineCap = ActualHighlightStrokeStyle.StartLineCap;
					StyleProvider.StrokeLineJoin = ActualHighlightStrokeStyle.LineJoin;
					StyleProvider.StrokeMiterLimit = ActualHighlightStrokeStyle.MiterLimit;
				}
			}
		}
		protected internal virtual Rect CalculateTitleLayout(Size titleSize, VectorLayerBase layer) {
			Point leftTopPoint = layer.MapUnitToScreenZeroOffset(Layout.LocationInMapUnits);
			return new Rect(leftTopPoint, Layout.SizeInPixels);
		}
		protected internal virtual void OnItemChanged(object shape) {
			if (shape != null && Layer != null) {
				ApplyAppearance();
				UpdateLayout();
				UpdateVisibility();
				CalculateLayout();
			}
		}
		protected internal virtual Size GetAvailableSizeForTitle() {
			return ((IMapItem)this).Size;
		}
		protected internal override void SetIsSelected(bool value) {
			base.SetIsSelected(value);
			Title.SetIsSelectedInternal(value);
		}
		protected internal override void SetIsHighlighted(bool value) {
			base.SetIsHighlighted(value);
			Title.SetIsHighlightedInternal(value);
		}
		protected internal override void ApplyAppearance() {
			if(StyleProvider != null) {
				StyleProvider.Fill = CalculateShapeFill();
				StyleProvider.Stroke = CalculateShapeStroke();
				StrokeStyle style = CalculateShapeStrokeStyle();
				if(style != null) {
					StyleProvider.StrokeThickness = style.Thickness;
					StyleProvider.StrokeDashArray = CommonUtils.CloneDoubleCollection(style.DashArray);
					StyleProvider.StrokeDashCap = style.DashCap;
					StyleProvider.StrokeDashOffset = style.DashOffset;
					StyleProvider.StrokeEndLineCap = style.EndLineCap;
					StyleProvider.StrokeStartLineCap = style.StartLineCap;
					StyleProvider.StrokeLineJoin = style.LineJoin;
					StyleProvider.StrokeMiterLimit = style.MiterLimit;
				}
			}
		}
		protected override void OnOwnerChanged() {
			base.OnOwnerChanged();
			ApplyTitleOptions();
		}
		protected internal void ApplyTitleOptions() {
			string result = string.Empty;
			ShapeTitleOptions options = ActualTitleOptions;
			if (!string.IsNullOrEmpty(options.Pattern)) {
				string[] template = options.Pattern.Split('{', '}');
				for (int i = 0; i < template.Length; i++) {
					if (i % 2 == 0)
						result += template[i];
					else {
						MapItemAttribute attribute = Attributes[template[i]];
						if (attribute != null && attribute.Value != null)
							result += attribute.Value.ToString();
					}
				}
			}
			Title.ApplyOptions(result, options.Template, options.VisibilityMode);
		}
		protected internal override void OnMouseEnter(object sender, MouseEventArgs e) {
			base.OnMouseEnter(sender, e);
			bool selected = Info != null ? Info.IsSelected : false;
			if(EnableHighlighting & !selected) {
				SetIsHighlighted(true);
				ApplyHighlightAppearance();
			}
		}
		protected internal override void OnMouseLeave(object sender, MouseEventArgs e) {
			base.OnMouseLeave(sender, e);
			if(EnableHighlighting)
				SetIsHighlighted(false);
			ApplyAppearance(); 
		}
		protected internal virtual CoordPoint GetCenterCore() {
			CoordBounds itemBounds = CalculateBounds();
			return CoordinateSystem.PointFactory.CreatePoint(itemBounds.X1 + itemBounds.Width / 2.0, itemBounds.Y1 + itemBounds.Height / 2.0);
		}
		public CoordPoint GetCenter() {
			return GetCenterCore();
		}
	}
}
