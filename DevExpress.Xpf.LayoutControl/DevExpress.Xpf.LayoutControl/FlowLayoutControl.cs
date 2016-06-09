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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.UI;
#if !SILVERLIGHT
using System.Windows.Threading;
#endif
namespace DevExpress.Xpf.LayoutControl {
	public interface IMaximizableElement {
		void AfterNormalization();
		void BeforeMaximization();
	}
	public interface IMaximizingContainer {
		FrameworkElement MaximizedElement { get; set; }
	}
	public enum MaximizedElementPosition { Left, Top, Right, Bottom };
	public interface IFlowLayoutModel : ILayoutModelBase {
		FrameworkElement MaximizedElement { get; }
		MaximizedElementPosition MaximizedElementPosition { get; set; }
		Orientation Orientation { get; }
	}
	public interface IFlowLayoutControl : ILayoutControlBase, IFlowLayoutModel {
		void BringSeparatorsToFront();
		bool IsLayerSeparator(UIElement element);
		void OnAllowLayerSizingChanged();
		void OnItemPositionChanged(int oldPosition, int newPosition);
		void SendSeparatorsToBack();
		bool AllowAddFlowBreaksDuringItemMoving { get; }
		bool AnimateItemMoving { get; }
		TimeSpan ItemMovingAnimationDuration { get; }
		double LayerMinWidth { get; }
		double LayerMaxWidth { get; }
		double LayerWidth { get; set; }
		Brush LayerSizingCoverBrush { get; }
		Style MaximizedElementPositionIndicatorStyle { get; }
	}
#if !SILVERLIGHT
#endif
	[DefaultBindingProperty("ItemsSource")]
	[StyleTypedProperty(Property = "LayerSeparatorStyle", StyleTargetType = typeof(LayerSeparator))]
	[StyleTypedProperty(Property = "MaximizedElementPositionIndicatorStyle", StyleTargetType = typeof(MaximizedElementPositionIndicator))]
	[DXToolboxBrowsable]
	public class FlowLayoutControl : LayoutControlBase, IFlowLayoutControl, IMaximizingContainer {
		public const int DefaultItemMovingAnimationDuration = 200;
		public static double DefaultLayerMinWidth = 20;
		public const double DefaultLayerSpace = 7;
		public static TimeSpan ItemDropAnimationDuration = TimeSpan.FromMilliseconds(500);
		public static TimeSpan ItemMaximizationAnimationDuration = TimeSpan.FromMilliseconds(500);
		#region Dependency Properties
		private static bool _IgnoreMaximizedElementChange;
		private static readonly DependencyProperty ItemsAttachedBehaviorProperty =
			DependencyProperty.Register("ItemsAttachedBehavior", typeof(object), typeof(FlowLayoutControl), null);
		public static readonly DependencyProperty AllowAddFlowBreaksDuringItemMovingProperty =
			DependencyProperty.Register("AllowAddFlowBreaksDuringItemMoving", typeof(bool), typeof(FlowLayoutControl),
#if SILVERLIGHT
				new PropertyMetadata((o, e) => o.EnsureDefaultValue(e.Property, ((FlowLayoutControl)o).GetDefaultAllowAddFlowBreaksDuringItemMoving(), true))
#else
				null
#endif
				);
		public static readonly DependencyProperty AllowItemMovingProperty =
			DependencyProperty.Register("AllowItemMoving", typeof(bool), typeof(FlowLayoutControl),
#if SILVERLIGHT
				new PropertyMetadata((o, e) => o.EnsureDefaultValue(e.Property, ((FlowLayoutControl)o).GetDefaultAllowItemMoving(), true))
#else
				null
#endif
				);
		public static readonly DependencyProperty AnimateItemMaximizationProperty =
			DependencyProperty.Register("AnimateItemMaximization", typeof(bool), typeof(FlowLayoutControl), new PropertyMetadata(true));
		public static readonly DependencyProperty AnimateItemMovingProperty =
			DependencyProperty.Register("AnimateItemMoving", typeof(bool), typeof(FlowLayoutControl),
#if SILVERLIGHT
				new PropertyMetadata((o, e) => o.EnsureDefaultValue(e.Property, ((FlowLayoutControl)o).GetDefaultAnimateItemMoving(), true))
#else
				null
#endif
				);
		public static readonly DependencyProperty BreakFlowToFitProperty =
			DependencyProperty.Register("BreakFlowToFit", typeof(bool), typeof(FlowLayoutControl),
				new PropertyMetadata(true, (o, e) => ((FlowLayoutControl)o).OnBreakFlowToFitChanged()));
		public static readonly DependencyProperty IsFlowBreakProperty =
			DependencyProperty.RegisterAttached("IsFlowBreak", typeof(bool), typeof(FlowLayoutControl), new PropertyMetadata(OnAttachedPropertyChanged));
		public static readonly DependencyProperty ItemContainerStyleProperty =
			DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(FlowLayoutControl),
				new PropertyMetadata((o, e) => ((FlowLayoutControl)o).OnItemContainerStyleChanged(e)));
		public static readonly DependencyProperty ItemMovingAnimationDurationProperty =
			DependencyProperty.Register("ItemMovingAnimationDuration", typeof(TimeSpan), typeof(FlowLayoutControl),
				new PropertyMetadata(TimeSpan.FromMilliseconds(DefaultItemMovingAnimationDuration)));
		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(FlowLayoutControl),
				new PropertyMetadata((o, e) => ((FlowLayoutControl)o).OnItemsSourceChanged(e)));
		public static readonly DependencyProperty ItemTemplateProperty =
			DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(FlowLayoutControl),
				new PropertyMetadata((o, e) => ((FlowLayoutControl)o).OnItemTemplateChanged(e)));
		public static readonly DependencyProperty ItemTemplateSelectorProperty =
			DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(FlowLayoutControl),
				new PropertyMetadata((o, e) => ((FlowLayoutControl)o).OnItemTemplateSelectorChanged(e)));
		public static readonly DependencyProperty LayerSeparatorStyleProperty =
			DependencyProperty.Register("LayerSeparatorStyle", typeof(Style), typeof(FlowLayoutControl),
				new PropertyMetadata((o, e) => ((FlowLayoutControl)o).LayerSeparators.ItemStyle = (Style)e.NewValue));
		public static readonly DependencyProperty LayerSizingCoverBrushProperty =
			DependencyProperty.Register("LayerSizingCoverBrush", typeof(Brush), typeof(FlowLayoutControl),
				new PropertyMetadata(new SolidColorBrush(Color.FromArgb(127, 255, 255, 255))));
		public static readonly DependencyProperty LayerSpaceProperty =
			DependencyProperty.Register("LayerSpace", typeof(double), typeof(FlowLayoutControl),
				new PropertyMetadata(DefaultLayerSpace,
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						var control = (FlowLayoutControl)o;
#if SILVERLIGHT
						if (!o.EnsureDefaultValue(e.Property, control.GetDefaultLayerSpace(), true))
#endif
							control.OnLayerSpaceChanged((double)e.OldValue, (double)e.NewValue);
					}));
		public static readonly DependencyProperty MaximizedElementProperty =
			DependencyProperty.Register("MaximizedElement", typeof(FrameworkElement), typeof(FlowLayoutControl),
				new PropertyMetadata(
					delegate(DependencyObject o, DependencyPropertyChangedEventArgs e) {
						if (_IgnoreMaximizedElementChange)
							return;
						var control = (FlowLayoutControl)o;
						if (e.NewValue != null && ((FrameworkElement)e.NewValue).Parent != control) {
							_IgnoreMaximizedElementChange = true;
							o.SetValue(e.Property, e.OldValue);
							_IgnoreMaximizedElementChange = false;
							throw new ArgumentOutOfRangeException("MaximizedElement");
						}
						control.OnMaximizedElementChanged((FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
					}));
		public static readonly DependencyProperty MaximizedElementPositionProperty =
			DependencyProperty.Register("MaximizedElementPosition", typeof(MaximizedElementPosition), typeof(FlowLayoutControl),
				new PropertyMetadata((o, e) => ((FlowLayoutControl)o).OnMaximizedElementPositionChanged((MaximizedElementPosition)e.OldValue)));
		public static readonly DependencyProperty MaximizedElementPositionIndicatorStyleProperty =
			DependencyProperty.Register("MaximizedElementPositionIndicatorStyle", typeof(Style), typeof(FlowLayoutControl), null);
		public static readonly DependencyProperty MovingItemPlaceHolderBrushProperty =
			DependencyProperty.Register("MovingItemPlaceHolderBrush", typeof(Brush), typeof(FlowLayoutControl),
				new PropertyMetadata(DefaultMovingItemPlaceHolderBrush));
		public static readonly DependencyProperty OrientationProperty =
			DependencyProperty.Register("Orientation", typeof(Orientation), typeof(FlowLayoutControl),
				new PropertyMetadata(Orientation.Vertical, (o, e) => ((FlowLayoutControl)o).OnOrientationChanged()));
		public static readonly DependencyProperty ShowLayerSeparatorsProperty =
			DependencyProperty.Register("ShowLayerSeparators", typeof(bool), typeof(FlowLayoutControl),
				new PropertyMetadata((o, e) => ((FlowLayoutControl)o).OnShowLayerSeparatorsChanged()));
		public static readonly DependencyProperty StretchContentProperty =
			DependencyProperty.Register("StretchContent", typeof(bool), typeof(FlowLayoutControl),
				new PropertyMetadata((o, e) => ((FlowLayoutControl)o).OnStretchContentChanged()));
		public static bool GetIsFlowBreak(UIElement element) {
			return (bool)element.GetValue(IsFlowBreakProperty);
		}
		public static void SetIsFlowBreak(UIElement element, bool value) {
			element.SetValue(IsFlowBreakProperty, value);
		}
		#endregion Dependency Properties
		static FlowLayoutControl() {
			DevExpress.Mvvm.UI.ViewInjection.StrategyManager.Default.RegisterStrategy
				<FlowLayoutControl, DevExpress.Mvvm.UI.ViewInjection.ItemsControlStrategy<FlowLayoutControl, DevExpress.Mvvm.UI.ViewInjection.FlowLayoutControlWrapper>>();
		}
		public FlowLayoutControl() {
			LayerSeparators = new LayerSeparators(this);
			LayerSeparators.AreVisible = ShowLayerSeparators;
#if SILVERLIGHT
			this.EnsureDefaultValue(AllowAddFlowBreaksDuringItemMovingProperty, GetDefaultAllowAddFlowBreaksDuringItemMoving(), false);
			this.EnsureDefaultValue(AllowItemMovingProperty, GetDefaultAllowItemMoving(), false);
			this.EnsureDefaultValue(AnimateItemMovingProperty, GetDefaultAnimateItemMoving(), false);
			this.EnsureDefaultValue(LayerSpaceProperty, GetDefaultLayerSpace(), false);
#endif
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlAllowAddFlowBreaksDuringItemMoving")]
#endif
		public bool AllowAddFlowBreaksDuringItemMoving {
			get { return (bool)GetValue(AllowAddFlowBreaksDuringItemMovingProperty); }
			set { SetValue(AllowAddFlowBreaksDuringItemMovingProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlAllowItemMoving")]
#endif
		public bool AllowItemMoving {
			get { return (bool)GetValue(AllowItemMovingProperty); }
			set { SetValue(AllowItemMovingProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlAllowLayerSizing")]
#endif
		public bool AllowLayerSizing {
			get { return Controller.AllowLayerSizing; }
			set { Controller.AllowLayerSizing = value; }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlAllowMaximizedElementMoving")]
#endif
		public bool AllowMaximizedElementMoving {
			get { return Controller.AllowMaximizedElementMoving; }
			set { Controller.AllowMaximizedElementMoving = value; }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlAnimateItemMaximization")]
#endif
		public bool AnimateItemMaximization {
			get { return (bool)GetValue(AnimateItemMaximizationProperty); }
			set { SetValue(AnimateItemMaximizationProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlAnimateItemMoving")]
#endif
		public bool AnimateItemMoving {
			get { return (bool)GetValue(AnimateItemMovingProperty); }
			set { SetValue(AnimateItemMovingProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlBreakFlowToFit")]
#endif
		public bool BreakFlowToFit {
			get { return (bool)GetValue(BreakFlowToFitProperty); }
			set { SetValue(BreakFlowToFitProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlController")]
#endif
		public new FlowLayoutController Controller { get { return (FlowLayoutController)base.Controller; } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlIsItemMaximizationAnimationActive")]
#endif
		public bool IsItemMaximizationAnimationActive { get { return ItemMaximizationAnimation != null && ItemMaximizationAnimation.IsActive; } }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlItemContainerStyle")]
#endif
		public Style ItemContainerStyle {
			get { return (Style)GetValue(ItemContainerStyleProperty); }
			set { SetValue(ItemContainerStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlItemMovingAnimationDuration")]
#endif
		public TimeSpan ItemMovingAnimationDuration {
			get { return (TimeSpan)GetValue(ItemMovingAnimationDurationProperty); }
			set { SetValue(ItemMovingAnimationDurationProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlItemsSource")]
#endif
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlItemTemplate")]
#endif
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlItemTemplateSelector")]
#endif
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlLayerMinWidth")]
#endif
		public double LayerMinWidth {
			get { return Math.Max(DefaultLayerMinWidth, LayoutProvider.GetLayerMinWidth(ChildrenMinSize)); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlLayerMaxWidth")]
#endif
		public double LayerMaxWidth {
			get { return LayoutProvider.GetLayerMaxWidth(ChildrenMaxSize); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlLayerSeparatorStyle")]
#endif
		public Style LayerSeparatorStyle {
			get { return (Style)GetValue(LayerSeparatorStyleProperty); }
			set { SetValue(LayerSeparatorStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlLayerSizingCoverBrush")]
#endif
		public Brush LayerSizingCoverBrush {
			get { return (Brush)GetValue(LayerSizingCoverBrushProperty); }
			set { SetValue(LayerSizingCoverBrushProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlLayerSpace")]
#endif
		public double LayerSpace {
			get { return (double)GetValue(LayerSpaceProperty); }
			set { SetValue(LayerSpaceProperty, Math.Max(0, value)); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlLayerWidth")]
#endif
		public double LayerWidth {
			get {
				Size itemSize;
				return LayoutProvider.GetLayerWidth(GetLogicalChildren(false), out itemSize);
			}
			set {
				value = Math.Max(LayerMinWidth, Math.Min(value, LayerMaxWidth));
				var items = GetLogicalChildren(false);
				Size prevSize, size;
				if(LayoutProvider.GetLayerWidth(items, out prevSize) == value)
					return;
				LayoutProvider.SetLayerWidth(items, value, out size);
				if(MaximizedElement != null)
					MaximizedElementOriginalSize = new Size(
						double.IsInfinity(size.Width) ? MaximizedElementOriginalSize.Width : size.Width,
						double.IsInfinity(size.Height) ? MaximizedElementOriginalSize.Height : size.Height);
				if(ItemsSizeChanged != null)
					ItemsSizeChanged(this, new ValueChangedEventArgs<Size>(prevSize, size));
			}
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlMaximizedElement")]
#endif
		public FrameworkElement MaximizedElement {
			get { return (FrameworkElement)GetValue(MaximizedElementProperty); }
			set { SetValue(MaximizedElementProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlMaximizedElementOriginalSize")]
#endif
		public Size MaximizedElementOriginalSize { get; set; }
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlMaximizedElementPosition")]
#endif
		public MaximizedElementPosition MaximizedElementPosition {
			get { return (MaximizedElementPosition)GetValue(MaximizedElementPositionProperty); }
			set { SetValue(MaximizedElementPositionProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlMaximizedElementPositionIndicatorStyle")]
#endif
		public Style MaximizedElementPositionIndicatorStyle {
			get { return (Style)GetValue(MaximizedElementPositionIndicatorStyleProperty); }
			set { SetValue(MaximizedElementPositionIndicatorStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlMovingItemPlaceHolderBrush")]
#endif
		public Brush MovingItemPlaceHolderBrush {
			get { return (Brush)GetValue(MovingItemPlaceHolderBrushProperty); }
			set { SetValue(MovingItemPlaceHolderBrushProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlOrientation")]
#endif
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlShowLayerSeparators")]
#endif
		public bool ShowLayerSeparators {
			get { return (bool)GetValue(ShowLayerSeparatorsProperty); }
			set { SetValue(ShowLayerSeparatorsProperty, value); }
		}
#if !SL
	[DevExpressXpfLayoutControlLocalizedDescription("FlowLayoutControlStretchContent")]
#endif
		public bool StretchContent {
			get { return (bool)GetValue(StretchContentProperty); }
			set { SetValue(StretchContentProperty, value); }
		}
		public event ValueChangedEventHandler<int> ItemPositionChanged;
		public event ValueChangedEventHandler<Size> ItemsSizeChanged;
		public event ValueChangedEventHandler<FrameworkElement> MaximizedElementChanged;
		public event ValueChangedEventHandler<MaximizedElementPosition> MaximizedElementPositionChanged;
		#region Children
		protected virtual FrameworkElement CreateItem() {
			return new ContentPresenter();
		}
		protected override Rect GetChildBounds(FrameworkElement child) {
			if(child == MaximizedElement)
				return Rect.Empty;
			else
				return base.GetChildBounds(child);
		}
		protected override Rect GetChildrenBounds() {
			var result = base.GetChildrenBounds();
			if(MaximizedElement != null)
				LayoutProvider.UpdateChildrenBounds(ref result, base.GetChildBounds(MaximizedElement));
			return result;
		}
		protected override IEnumerable<UIElement> GetInternalElements() {
			foreach (UIElement element in BaseGetInternalElements())
				yield return element;
			foreach (UIElement element in LayerSeparators.GetInternalElements())
				yield return element;
		}
		protected virtual FrameworkElement GetNeighborChild(FrameworkElement child) {
			FrameworkElements children = GetLogicalChildren(true);
			int index = children.IndexOf(child);
			if (index < children.Count - 1)
				index++;
			else
				if (index > 0)
					index--;
				else
					return null;
			return children[index];
		}
		protected virtual void InitItem(FrameworkElement item) {
			item.SetBinding(ContentPresenter.ContentProperty, new Binding("DataContext") { Source = item });
		}
		protected override bool IsTempChild(UIElement child) {
			return base.IsTempChild(child) || LayerSeparators.IsItem(child);
		}
		protected override void OnChildRemoved(FrameworkElement child) {
			base.OnChildRemoved(child);
			if(child == MaximizedElement)
				MaximizedElement = null;
		}
		protected override bool NeedsChildChangeNotifications { get { return true; } }
		private IEnumerable<UIElement> BaseGetInternalElements() {
			return base.GetInternalElements();
		}
		#endregion Children
		#region Layout
		protected override Size OnArrange(Rect bounds) {
			LayerSeparators.MarkItemsAsUnused();
			var result = base.OnArrange(bounds);
			LayerSeparators.DeleteUnusedItems();
			return result;
		}
		protected override LayoutProviderBase CreateLayoutProvider() {
			return new FlowLayoutProvider(this);
		}
		protected override LayoutParametersBase CreateLayoutProviderParameters() {
			return new FlowLayoutParameters(ItemSpace, LayerSpace, BreakFlowToFit, StretchContent, ShowLayerSeparators, LayerSeparators);
		}
		protected new FlowLayoutProvider LayoutProvider { get { return (FlowLayoutProvider)base.LayoutProvider; } }
		protected Point OffsetBeforeElementMaximization { get; set; }
		#endregion Layout
		#region XML Storage
		protected override void ReadChildrenFromXML(XmlReader xml, out FrameworkElement lastChild) {
			base.ReadChildrenFromXML(xml, out lastChild);
			MoveLogicalChildrenToStart();
		}
		protected override void ReadCustomizablePropertiesFromXML(XmlReader xml) {
			base.ReadCustomizablePropertiesFromXML(xml);
			this.ReadPropertyFromXML(xml, MaximizedElementProperty, "MaximizedElement", typeof(FrameworkElement));
			MaximizedElementOriginalSize = SizeHelper.Parse(xml["MaximizedElementOriginalSize"]);
			this.ReadPropertyFromXML(xml, MaximizedElementPositionProperty, "MaximizedElementPosition", typeof(MaximizedElementPosition));
		}
		protected override void ReadCustomizablePropertiesFromXML(FrameworkElement element, XmlReader xml) {
			base.ReadCustomizablePropertiesFromXML(element, xml);
			element.ReadPropertyFromXML(xml, IsFlowBreakProperty, "IsFlowBreak", typeof(bool));
			element.ReadPropertyFromXML(xml, WidthProperty, "Width", typeof(double));
			element.ReadPropertyFromXML(xml, HeightProperty, "Height", typeof(double));
			if (element is GroupBox)
				((GroupBox)element).ReadCustomizablePropertiesFromXML(xml);
		}
		protected override void WriteCustomizablePropertiesToXML(XmlWriter xml) {
			base.WriteCustomizablePropertiesToXML(xml);
			this.WritePropertyToXML(xml, MaximizedElementProperty, "MaximizedElement");
			xml.WriteAttributeString("MaximizedElementOriginalSize", MaximizedElementOriginalSize.ToString());
			this.WritePropertyToXML(xml, MaximizedElementPositionProperty, "MaximizedElementPosition");
		}
		protected override void WriteCustomizablePropertiesToXML(FrameworkElement element, XmlWriter xml) {
			base.WriteCustomizablePropertiesToXML(element, xml);
			element.WritePropertyToXML(xml, IsFlowBreakProperty, "IsFlowBreak");
			element.WritePropertyToXML(xml, WidthProperty, "Width");
			element.WritePropertyToXML(xml, HeightProperty, "Height");
			if (element is GroupBox)
				((GroupBox)element).WriteCustomizablePropertiesToXML(xml);
		}
		protected void MoveLogicalChildrenToStart() {
			for (int i = Children.Count - 1; i >= 0; i--) {
				UIElement child = Children[i];
				if (!IsLogicalChild(child)) {
					Children.RemoveAt(i);
					Children.Add(child);
				}
			}
		}
		#endregion XML Storage
		protected override PanelControllerBase CreateController() {
			return new FlowLayoutController(this);
		}
#if SILVERLIGHT
		protected virtual bool GetDefaultAllowAddFlowBreaksDuringItemMoving() {
			return false;
		}
		protected virtual bool GetDefaultAllowItemMoving() {
			return false;
		}
		protected virtual bool GetDefaultAnimateItemMoving() {
			return false;
		}
		protected virtual double GetDefaultLayerSpace() {
			return DefaultLayerSpace;
		}
#endif
		protected override void OnAttachedPropertyChanged(FrameworkElement child, DependencyProperty property, object oldValue, object newValue) {
			base.OnAttachedPropertyChanged(child, property, oldValue, newValue);
			if(property == IsFlowBreakProperty)
				OnIsFlowBreakChanged(child);
		}
		protected virtual void OnBreakFlowToFitChanged() {
			SetOffset(new Point(0, 0));
			Changed();
		}
		protected virtual void OnIsFlowBreakChanged(FrameworkElement child) {
			Changed();
		}
		protected virtual void OnItemContainerStyleChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<FlowLayoutControl, FrameworkElement>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemsAttachedBehaviorProperty);
		}
		protected virtual void OnItemPositionChanged(int oldPosition, int newPosition) {
			if (newPosition != oldPosition)
				ItemsAttachedBehaviorCore<FlowLayoutControl, FrameworkElement>.OnTargetItemPositionChanged(this, ItemsAttachedBehaviorProperty,
					oldPosition, newPosition);
			if (ItemPositionChanged != null)
				ItemPositionChanged(this, new ValueChangedEventArgs<int>(oldPosition, newPosition));
		}
		protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<FlowLayoutControl, FrameworkElement>.OnItemsSourcePropertyChanged(
				this, e, ItemsAttachedBehaviorProperty, ItemTemplateProperty, ItemTemplateSelectorProperty, ItemContainerStyleProperty,
				(control) => control.Children, (control) => control.CreateItem(), null, (item) => InitItem(item), useDefaultTemplateSelector:false);
		}
		protected virtual void OnItemTemplateChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<FlowLayoutControl, FrameworkElement>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemsAttachedBehaviorProperty);
		}
		protected virtual void OnItemTemplateSelectorChanged(DependencyPropertyChangedEventArgs e) {
			ItemsAttachedBehaviorCore<FlowLayoutControl, FrameworkElement>.OnItemsGeneratorTemplatePropertyChanged(this, e, ItemsAttachedBehaviorProperty);
		}
		protected virtual void OnLayerSpaceChanged(double oldValue, double newValue) {
			Changed();
		}
		protected virtual void OnMaximizedElementChanged(FrameworkElement oldValue, FrameworkElement newValue) {
			if (!this.IsInDesignTool() && this.IsInVisualTree() && AnimateItemMaximization) {
				if (ItemMaximizationAnimation != null)
					ItemMaximizationAnimation.Stop();
				ItemMaximizationAnimation = new ElementBoundsAnimation(GetLogicalChildren(true));
				ItemMaximizationAnimation.StoreOldElementBounds();
			}
			if (oldValue == null)
				OffsetBeforeElementMaximization = Offset;
			if (oldValue != null) {
				if (double.IsPositiveInfinity(MaximizedElementOriginalSize.Width))
					oldValue.ClearValue(WidthProperty);
				else
					oldValue.Width = MaximizedElementOriginalSize.Width;
				if (double.IsPositiveInfinity(MaximizedElementOriginalSize.Height))
					oldValue.ClearValue(HeightProperty);
				else
					oldValue.Height = MaximizedElementOriginalSize.Height;
				if (oldValue is IMaximizableElement)
					((IMaximizableElement)oldValue).AfterNormalization();
			}
			if (newValue != null) {
				if (newValue is IMaximizableElement)
					((IMaximizableElement)newValue).BeforeMaximization();
				var maximizedElementOriginalSize = new Size(newValue.Width, newValue.Height);
				if (!newValue.IsPropertyAssigned(WidthProperty))
					maximizedElementOriginalSize.Width = double.PositiveInfinity;
				if (!newValue.IsPropertyAssigned(HeightProperty))
					maximizedElementOriginalSize.Height = double.PositiveInfinity;
				MaximizedElementOriginalSize = maximizedElementOriginalSize;
				newValue.Width = double.NaN;
				newValue.Height = double.NaN;
			}
			Changed();
			if (oldValue == null)
				BringChildIntoView(GetNeighborChild(newValue));
			if (newValue == null) {
				SetOffset(OffsetBeforeElementMaximization);
				BringChildIntoView(oldValue);
			}
			if (ItemMaximizationAnimation != null) {
				UpdateLayout();
				ItemMaximizationAnimation.StoreNewElementBounds();
				ItemMaximizationAnimation.Begin(ItemMaximizationAnimationDuration, new CubicEase(), () => ItemMaximizationAnimation = null);
			}
			if (MaximizedElementChanged != null)
				MaximizedElementChanged(this, new ValueChangedEventArgs<FrameworkElement>(oldValue, newValue));
		}
		protected virtual void OnMaximizedElementPositionChanged(MaximizedElementPosition oldValue) {
			Changed();
			if (MaximizedElementPositionChanged != null)
				MaximizedElementPositionChanged(this, new ValueChangedEventArgs<MaximizedElementPosition>(oldValue, MaximizedElementPosition));
		}
		protected virtual void OnOrientationChanged() {
			SetOffset(new Point(0, 0));
			Changed();
		}
		protected virtual void OnShowLayerSeparatorsChanged() {
			LayerSeparators.AreVisible = ShowLayerSeparators;
			Changed();
		}
		protected virtual void OnStretchContentChanged() {
			Changed();
		}
		protected ElementBoundsAnimation ItemMaximizationAnimation { get; private set; }
		protected LayerSeparators LayerSeparators { get; private set; }
		#region IFlowLayoutControl
		void IFlowLayoutControl.BringSeparatorsToFront() {
			LayerSeparators.BringToFront();
		}
		bool IFlowLayoutControl.IsLayerSeparator(UIElement element) {
			return LayerSeparators.IsItem(element);
		}
		void IFlowLayoutControl.OnAllowLayerSizingChanged() {
			LayerSeparators.AreInteractive = Controller.AllowLayerSizing;
		}
		void IFlowLayoutControl.OnItemPositionChanged(int oldPosition, int newPosition) {
			OnItemPositionChanged(oldPosition, newPosition);
		}
		void IFlowLayoutControl.SendSeparatorsToBack() {
			LayerSeparators.SendToBack();
		}
		FrameworkElement IFlowLayoutModel.MaximizedElement {
			get { return MaximizedElement != null && MaximizedElement.GetVisible() ? MaximizedElement : null; }
		}
		#endregion IFlowLayoutControl
		#region UIAutomation
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.LayoutControl.UIAutomation.FlowLayoutControlAutomationPeer(this);
		}
		#endregion
	}
	[TemplatePart(Name = HorizontalRootElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = VerticalRootElementName, Type = typeof(FrameworkElement))]
	public class LayerSeparator : ControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty ThicknessProperty =
			DependencyProperty.Register("Thickness", typeof(double), typeof(LayerSeparator), null);
		#endregion Dependency Properties
		private bool _IsInteractive;
		private Orientation _Kind = Orientation.Vertical;
		public LayerSeparator() {
			DefaultStyleKey = typeof(LayerSeparator);
			OnIsInteractiveChanged();
		}
		public Orientation Kind {
			get { return _Kind; }
			set {
				if(_Kind != value) {
					_Kind = value;
					UpdateTemplate();
				}
			}
		}
		public bool IsInteractive {
			get { return _IsInteractive; }
			set {
				if(_IsInteractive != value) {
					_IsInteractive = value;
					OnIsInteractiveChanged();
				}
			}
		}
		public double Thickness {
			get { return (double)GetValue(ThicknessProperty); }
			set { SetValue(ThicknessProperty, value); }
		}
		#region Template
		const string HorizontalRootElementName = "HorizontalRootElement";
		const string VerticalRootElementName = "VerticalRootElement";
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			HorizontalRootElement = GetTemplateChild(HorizontalRootElementName) as FrameworkElement;
			VerticalRootElement = GetTemplateChild(VerticalRootElementName) as FrameworkElement;
			UpdateTemplate();
		}
		protected virtual void OnIsInteractiveChanged() {
			IsHitTestVisible = IsInteractive;
		}
		protected virtual void UpdateTemplate() {
			if(HorizontalRootElement != null)
				HorizontalRootElement.SetVisible(Kind == Orientation.Horizontal);
			if(VerticalRootElement != null)
				VerticalRootElement.SetVisible(Kind == Orientation.Vertical);
			if (RootElement != null)
				Cursor = RootElement.Cursor;
#if !SILVERLIGHT
			InvalidateMeasure();
#endif
		}
		protected FrameworkElement HorizontalRootElement { get; set; }
		protected FrameworkElement VerticalRootElement { get; set; }
		protected FrameworkElement RootElement { get { return Kind == Orientation.Horizontal ? HorizontalRootElement : VerticalRootElement; } }
		#endregion Template
	}
	public class LayerSeparators : ElementPool<LayerSeparator> {
		public const int SeparatorZIndex = PanelBase.NormalMediumLowZIndex;
		public const int TopMostSeparatorZIndex = PanelBase.NormalMediumHighZIndex;
		private bool _AreInteractive;
		private bool _AreVisible;
		public LayerSeparators(Panel container) : base(container) { }
		public LayerSeparator Add(Orientation kind, Rect bounds) {
			var result = Add();
			result.Kind = kind;
			result.Measure(SizeHelper.Infinite);
			result.Arrange(bounds);
			return result;
		}
		public void BringToFront() {
			foreach (LayerSeparator item in Items)
				item.SetZIndex(TopMostSeparatorZIndex);
		}
		public IEnumerable<UIElement> GetInternalElements() {
			if (StandardItem != null)
				yield return StandardItem;
		}
		public void SendToBack() {
			foreach (LayerSeparator item in Items)
				item.SetZIndex(SeparatorZIndex);
		}
		public bool AreInteractive {
			get { return _AreInteractive; }
			set {
				if(_AreInteractive != value) {
					_AreInteractive = value;
					foreach(var item in Items)
						item.IsInteractive = AreInteractive;
				}
			}
		}
		public bool AreVisible {
			get { return _AreVisible; }
			set {
				if (AreVisible == value)
					return;
				_AreVisible = value;
				CheckStandardItem();
			}
		}
		public double SeparatorThickness { get { return StandardItem.Thickness; } }
		protected override LayerSeparator CreateItem() {
			var result = base.CreateItem();
			result.IsInteractive = AreInteractive;
			result.SetZIndex(SeparatorZIndex);
			return result;
		}
		protected override void OnItemStyleChanged() {
			base.OnItemStyleChanged();
			if (StandardItem != null)
				StandardItem.SetValueIfNotDefault(LayerSeparator.StyleProperty, ItemStyle);
		}
		protected LayerSeparator StandardItem { get; private set; }
		private void CheckStandardItem() {
			if (AreVisible)
				CreateStandardItem();
			else
				DestroyStandardItem();
		}
		private void CreateStandardItem() {
			if (StandardItem != null)
				return;
			StandardItem = CreateItem();
			Container.Children.Add(StandardItem);
		}
		private void DestroyStandardItem() {
			if (StandardItem == null)
				return;
			Container.Children.Remove(StandardItem);
			StandardItem = null;
		}
	}
	public class FlowLayoutParameters : LayoutParametersBase {
		public FlowLayoutParameters(double itemSpace, double layerSpace, bool breakFlowToFit, bool stretchContent,
			bool showLayerSeparators, LayerSeparators layerSeparators) : base(itemSpace) {
			LayerSpace = layerSpace;
			BreakFlowToFit = breakFlowToFit;
			StretchContent = stretchContent;
			ShowLayerSeparators = showLayerSeparators;
			LayerSeparators = layerSeparators;
		}
		public bool BreakFlowToFit { get; private set; }
		public LayerSeparators LayerSeparators { get; private set; }
		public double LayerSpace { get; private set; }
		public bool ShowLayerSeparators { get; private set; }
		public bool StretchContent { get; private set; }
	}
	public struct FlowLayoutItemPosition {
		public FlowLayoutItemPosition(double layerOffset, double itemOffset) {
			LayerOffset = layerOffset;
			ItemOffset = itemOffset;
		}
		public double LayerOffset;
		public double ItemOffset;
	}
	public struct FlowLayoutItemSize {
		public FlowLayoutItemSize(double width, double length) {
			Width = width;
			Length = length;
		}
		public double Width;
		public double Length;
	}
	public class FlowLayoutLayerInfo {
		public FlowLayoutLayerInfo(int firstItemIndex, bool isHardFlowBreak, FlowLayoutItemPosition position) {
			FirstItemIndex = firstItemIndex;
			IsHardFlowBreak = isHardFlowBreak;
			Position = position;
			SlotFirstItemIndexes = new List<int>();
		}
		public int FirstItemIndex { get; private set; }
		public bool IsHardFlowBreak { get; private set; }
		public FlowLayoutItemPosition Position { get; private set; }
		public FlowLayoutItemSize Size;
		public int SlotCount { get { return SlotFirstItemIndexes.Count; } }
		public List<int> SlotFirstItemIndexes { get; private set; }
	}
	public enum FlowBreakKind { None, Existing, New }
	public class FlowLayoutProvider : LayoutProviderBase {
		public FlowLayoutProvider(IFlowLayoutModel model) : base(model) {
			LayerInfos = new List<FlowLayoutLayerInfo>();
		}
		public override void CopyLayoutInfo(FrameworkElement from, FrameworkElement to) {
			base.CopyLayoutInfo(from, to);
			FlowLayoutControl.SetIsFlowBreak(to, FlowLayoutControl.GetIsFlowBreak(from));
		}
		public virtual void UpdateChildrenBounds(ref Rect bounds, Rect maximizedElementBounds) {
			if(bounds.IsEmpty)
				bounds = maximizedElementBounds;
			else
				if(Model.MaximizedElementPosition == MaximizedElementPosition.Left ||
					Model.MaximizedElementPosition == MaximizedElementPosition.Right) {
					RectHelper.SetLeft(ref bounds, Math.Min(bounds.Left, maximizedElementBounds.Left));
					RectHelper.SetRight(ref bounds, Math.Max(bounds.Right, maximizedElementBounds.Right));
				}
				else {
					RectHelper.SetTop(ref bounds, Math.Min(bounds.Top, maximizedElementBounds.Top));
					RectHelper.SetBottom(ref bounds, Math.Max(bounds.Bottom, maximizedElementBounds.Bottom));
				}
		}
		public override void UpdateScrollableAreaBounds(ref Rect bounds) {
			base.UpdateScrollableAreaBounds(ref bounds);
			if (Model.MaximizedElement == null)
				return;
			FlowLayoutItemPosition position, maximizedElementPosition;
			FlowLayoutItemSize size, maximizedElementSize;
			GetItemPositionAndSize(bounds, out position, out size);
			GetItemPositionAndSize(Model.MaximizedElement.GetBounds(), out maximizedElementPosition, out maximizedElementSize);
			if (Model.MaximizedElementPosition == MaximizedElementPosition.Left ||
				Model.MaximizedElementPosition == MaximizedElementPosition.Top) {
				double change = maximizedElementPosition.LayerOffset + maximizedElementSize.Width - position.LayerOffset;
				position.LayerOffset += change;
				size.Width -= change;
			}
			else
				size.Width -= position.LayerOffset + size.Width - maximizedElementPosition.LayerOffset;
			size.Width = Math.Max(0, size.Width);
			bounds = GetItemBounds(position, size);
		}
		public virtual double CalculateLayerWidthChange(LayerSeparator separator, Point positionChange) {
			var layerCount = 1 + Parameters.LayerSeparators.IndexOf(separator);
			var change = Orientation == Orientation.Horizontal ? positionChange.Y : positionChange.X;
			return LayerWidthChangeDirection * Math.Round(change / layerCount);
		}
		public virtual int GetItemIndex(UIElementCollection items, int visibleIndex) {
			if (visibleIndex == LayoutItems.Count)
				if (LayoutItems[LayoutItems.Count - 1] == Model.MaximizedElement)
					return items.IndexOf(LayoutItems[LayoutItems.Count - 2]) + 1;
				else
					return items.IndexOf(LayoutItems[LayoutItems.Count - 1]) + 1;
			else
				if (LayoutItems[visibleIndex] == Model.MaximizedElement)
					if (visibleIndex == 0)
						return items.IndexOf(LayoutItems[visibleIndex + 1]);
					else
						return items.IndexOf(LayoutItems[visibleIndex - 1]) + 1;
				else
					return items.IndexOf(LayoutItems[visibleIndex]);
		}
		public virtual int GetItemPlaceIndex(FrameworkElement item, Rect bounds, Point p, out FlowBreakKind flowBreakKind) {
			flowBreakKind = FlowBreakKind.None;
			if (!bounds.Contains(p) || LayoutItems.Count == 0)
				return -1;
			for (int layerIndex = 0; layerIndex < LayerInfos.Count; layerIndex++) {
				FlowLayoutLayerInfo layerInfo = LayerInfos[layerIndex];
				if (!GetElementHitTestBounds(GetLayerBounds(layerInfo), bounds, true, true).Contains(p))
					continue;
				for (int slotIndex = 0; slotIndex < layerInfo.SlotCount; slotIndex++) {
					Rect slotBounds = GetSlotBounds(layerInfo, slotIndex);
					if (!GetElementHitTestBounds(slotBounds, bounds, slotIndex == 0, slotIndex == layerInfo.SlotCount - 1).Contains(p))
						continue;
					bool isBeforeItemPlace;
					int result = GetSlotItemPlaceIndex(layerInfo, slotIndex, item, bounds, p, out isBeforeItemPlace);
					if (isBeforeItemPlace) {
						if (result == 0 || FlowLayoutControl.GetIsFlowBreak(LayoutItems[result]))
							flowBreakKind = FlowBreakKind.Existing;
					}
					else
						result++;
					return result;
				}
			}
			foreach (FlowLayoutLayerInfo layerInfo in LayerInfos)
				if (layerInfo.IsHardFlowBreak && GetLayerSpaceBounds(layerInfo, bounds).Contains(p)) {
					if (CanAddHardFlowBreaks)
						flowBreakKind = FlowBreakKind.New;
					return layerInfo.FirstItemIndex;
				}
			if (GetRemainderBounds(bounds, GetLayerBounds(GetLayerInfo(LayoutItems.Count - 1))).Contains(p)) {
				if (CanAddHardFlowBreaks)
					flowBreakKind = FlowBreakKind.New;
				return LayoutItems.Count;
			}
			return -1;
		}
		public void OffsetLayerSeparators(double layerWidthChange) {
			if(layerWidthChange == 0)
				return;
			layerWidthChange *= LayerWidthChangeDirection;
			FrameworkElement separator;
			FlowLayoutItemPosition position;
			FlowLayoutItemSize size;
			for(int i = 0; i < Parameters.LayerSeparators.Count; i++) {
				separator = Parameters.LayerSeparators[i];
				GetItemPositionAndSize(separator.GetBounds(), out position, out size);
				position.LayerOffset += (1 + i) * layerWidthChange;
				separator.Arrange(GetItemBounds(position, size));
			}
		}
		public double GetLayerMinWidth(Size itemsMinSize) {
			return GetItemSize(itemsMinSize).Width;
		}
		public double GetLayerMaxWidth(Size itemsMaxSize) {
			return GetItemSize(itemsMaxSize).Width;
		}
		public double GetLayerWidth(FrameworkElements items, out Size itemSize) {
			var result = double.PositiveInfinity;
			itemSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
			foreach(var item in items)
				if(item != Model.MaximizedElement) {
					var itemWidth = GetItemSize(item.GetSize()).Width;
					if(!double.IsPositiveInfinity(result) && itemWidth != result)
						return double.PositiveInfinity;
					result = itemWidth;
				}
			if(double.IsPositiveInfinity(result))
				result = 0;
			itemSize = GetItemSize(new FlowLayoutItemSize(result, double.PositiveInfinity));
			return result;
		}
		public void SetLayerWidth(FrameworkElements items, double value, out Size itemSize) {
			itemSize = GetItemSize(new FlowLayoutItemSize(value, Double.PositiveInfinity));
			Size itemPrevSize;
			if(GetLayerWidth(items, out itemPrevSize) == value)
				return;
			foreach(var item in items)
				if(item != Model.MaximizedElement) {
					if(!double.IsPositiveInfinity(itemSize.Width))
						item.Width = itemSize.Width;
					if(!double.IsPositiveInfinity(itemSize.Height))
						item.Height = itemSize.Height;
				}
		}
		public virtual double GetLayerDistance(bool isHardFlowBreak) {
			if (ShowLayerSeparators)
				return GetLayerSpace(isHardFlowBreak) + LayerSeparatorThickness + GetLayerSpace(isHardFlowBreak);
			else
				return GetLayerSpace(isHardFlowBreak);
		}
		public virtual double GetLayerSpace(bool isHardFlowBreak) {
			return Parameters.LayerSpace;
		}
		public virtual bool CanAddHardFlowBreaks {
			get { return Model.MaximizedElement == null; }
		}
		public double LayerSeparatorThickness { get; private set; }
		public FrameworkElements LayoutItems { get; protected set; }
		public virtual Orientation Orientation {
			get {
				if (Model.MaximizedElement == null)
					return Model.Orientation;
				else
					if (Model.MaximizedElementPosition == MaximizedElementPosition.Left ||
						Model.MaximizedElementPosition == MaximizedElementPosition.Right)
						return Orientation.Vertical;
					else
						return Orientation.Horizontal;
			}
		}
		public virtual bool ShowLayerSeparators {
			get { return !StretchContent && Parameters.ShowLayerSeparators; }
		}
		public virtual bool StretchContent {
			get { return Model.MaximizedElement == null && Parameters.StretchContent; }
		}
		protected delegate int CalculateMaximizedElementIndex(FrameworkElements items);
		protected delegate Size MeasureItem(FrameworkElement item);
		protected delegate void ArrangeItem(FrameworkElement item, ref FlowLayoutItemPosition itemPosition, ref FlowLayoutItemSize itemSize);   
		protected override Size OnMeasure(FrameworkElements items, Size maxSize) {
			CalculateLayout(items, RectHelper.New(maxSize),
				elements => Model.MaximizedElement == null ? -1 : elements.Count - 1,
				delegate(FrameworkElement item) {
					Size availableSize;
					if (item == Model.MaximizedElement)
						availableSize = CalculateMaximizedElementSize(items, RectHelper.New(maxSize));
					else {
						var itemSize = new FlowLayoutItemSize(double.PositiveInfinity, double.PositiveInfinity);
						if (StretchContent)
							itemSize.Width = GetLayerWidth(RectHelper.New(maxSize));
						availableSize = GetItemSize(itemSize);
					}
					item.Measure(availableSize);
					return item.GetDesiredSize();
				},
				null);
			var contentMaxSize = new FlowLayoutItemSize(0, 0);
			if (items.Count != 0) {
				for (int i = 0; i < LayerInfos.Count; i++) {
					FlowLayoutLayerInfo layerInfo = LayerInfos[i];
					if (i > 0)
						contentMaxSize.Width += GetLayerDistance(layerInfo.IsHardFlowBreak);
					contentMaxSize.Width += layerInfo.Size.Width;
					contentMaxSize.Length = Math.Max(contentMaxSize.Length, layerInfo.Size.Length);
				}
				if (ShowLayerSeparators && LayerInfos.Count == 1)
					contentMaxSize.Width += GetLayerDistance(false);
			}
			return GetItemSize(contentMaxSize);
		}
		protected override Size OnArrange(FrameworkElements items, Rect bounds, Rect viewPortBounds) {
			CalculateLayout(items, bounds,
				CalculateMaximizedElementIndexForArrange,
				delegate(FrameworkElement item) {
					if(item == Model.MaximizedElement)
						return CalculateMaximizedElementSize(items, viewPortBounds);
					else
						return item.GetDesiredSize();
				},
				delegate(FrameworkElement item, ref FlowLayoutItemPosition itemPosition, ref FlowLayoutItemSize itemSize) {
					if(item == Model.MaximizedElement)
						itemPosition.ItemOffset = GetLayerContentStart(viewPortBounds);
					else {
						itemSize = GetItemSize(item.DesiredSize);   
						if(StretchContent)
							itemSize.Width = GetLayerWidth(viewPortBounds);
					}
					item.Arrange(GetItemBounds(itemPosition, itemSize));
				});
			if(ShowLayerSeparators && items.Count != 0)
				AddLayerSeparators(bounds, viewPortBounds);
			return bounds.Size();
		}
		protected override void OnParametersChanged() {
			base.OnParametersChanged();
			if (ShowLayerSeparators)
				LayerSeparatorThickness = Parameters.LayerSeparators.SeparatorThickness;
		}
		protected virtual void CalculateLayout(FrameworkElements items, Rect bounds,
			CalculateMaximizedElementIndex calculateMaximizedElementIndex, MeasureItem measureItem, ArrangeItem arrangeItem) {
			var maximizedElementIndex = calculateMaximizedElementIndex(items);
			if (maximizedElementIndex != -1)
				PrepareMaximizedElement(items, maximizedElementIndex);
			double contentStartOffset = GetLayerContentStart(bounds);
			double slotOffset = contentStartOffset;
			var itemPosition = new FlowLayoutItemPosition(GetLayerStart(bounds), slotOffset);
			LayoutItems = items;
			LayerInfos.Clear();
			FlowLayoutLayerInfo layerInfo = null;
			for (int i = 0; i < items.Count; i++) {
				FrameworkElement item = items[i];
				FlowLayoutItemSize itemSize = GetItemSize(measureItem(item));
				if (i == 0) {
					layerInfo = new FlowLayoutLayerInfo(0, true, itemPosition);
					LayerInfos.Add(layerInfo);
				}
				else {
					bool isHardFlowBreak = false;
					if (maximizedElementIndex == -1 && IsFlowBreak(item, bounds, slotOffset, GetSlotLength(items, i, itemSize), out isHardFlowBreak) ||
						maximizedElementIndex != -1 && (i == maximizedElementIndex + 1 || i == maximizedElementIndex)) {
						slotOffset = contentStartOffset;
						itemPosition.LayerOffset += layerInfo.Size.Width + GetLayerDistance(isHardFlowBreak);
						itemPosition.ItemOffset = slotOffset;
						layerInfo = new FlowLayoutLayerInfo(i, isHardFlowBreak, itemPosition);
						LayerInfos.Add(layerInfo);
					}
					else
						itemPosition.ItemOffset = slotOffset;
				}
				layerInfo.SlotFirstItemIndexes.Add(i);
				FlowLayoutItemPosition slotPosition = itemPosition;
				FlowLayoutItemSize slotSize = itemSize;
				if (IsSlotWithMultipleItems(items, i))
					CalculateLayoutForSlotWithMultipleItems(items, ref i, slotPosition, ref slotSize, measureItem, arrangeItem);
				else
					if (arrangeItem != null)
						arrangeItem(item, ref slotPosition, ref slotSize);
				slotOffset = slotPosition.ItemOffset;
				layerInfo.Size.Length = slotOffset + slotSize.Length - contentStartOffset;
				layerInfo.Size.Width = Math.Max(layerInfo.Size.Width, slotSize.Width);
				slotOffset += GetSlotMaxLength(item, itemSize) + Parameters.ItemSpace;
			}
		}
		protected virtual void CalculateLayoutForSlotWithMultipleItems(FrameworkElements items, ref int itemIndex,
			FlowLayoutItemPosition slotPosition, ref FlowLayoutItemSize slotSize, MeasureItem measureItem, ArrangeItem arrangeItem) {
		}
		protected virtual FlowLayoutItemSize CalculateLayerSeparatorSize(Rect viewPortBounds) {
			var result = new FlowLayoutItemSize(LayerSeparatorThickness, GetLayerContentLength(viewPortBounds));
			foreach (FlowLayoutLayerInfo layerInfo in LayerInfos)
				result.Length = Math.Max(result.Length, layerInfo.Size.Length);
			return result;
		}
		protected virtual double GetSlotLength(FrameworkElements items, int itemIndex, FlowLayoutItemSize itemSize) {
			return itemSize.Length;
		}
		protected virtual double GetSlotMaxLength(FrameworkElement item, FlowLayoutItemSize itemSize) {
			return itemSize.Length;
		}
		protected virtual double GetSlotMaxWidth(FrameworkElement item, FlowLayoutItemSize itemSize) {
			return itemSize.Width;
		}
		protected double GetSlotMaxWidth(FrameworkElements items) {
			double result = 0;
			for (int i = 0; i < items.Count; i++) {
				FrameworkElement item = items[i];
				if (item == Model.MaximizedElement)
					continue;
				FlowLayoutItemSize itemSize = GetItemSize(item.GetDesiredSize());
				result = Math.Max(result, GetSlotMaxWidth(item, itemSize));
			}
			return result;
		}
		protected virtual bool NeedsFullSlot(FrameworkElement item) {
			return true;
		}
		protected virtual int CalculateMaximizedElementIndexForArrange(FrameworkElements items) {
			if(Model.MaximizedElement == null)
				return -1;
			if(Model.MaximizedElementPosition == MaximizedElementPosition.Left ||
				Model.MaximizedElementPosition == MaximizedElementPosition.Top)
				return 0;
			else
				return items.Count - 1;
		}
		protected virtual Size CalculateMaximizedElementSize(FrameworkElements items, Rect viewPortBounds) {
			double maximizedElementWidth = Math.Max(0, GetLayerWidth(viewPortBounds) - (GetLayerDistance(false) + GetSlotMaxWidth(items)));
			return GetItemSize(new FlowLayoutItemSize(maximizedElementWidth, GetLayerContentLength(viewPortBounds)));
		}
		protected virtual void PrepareMaximizedElement(FrameworkElements items, int maximizedElementIndex) {
			items.Remove(Model.MaximizedElement);
			items.Insert(maximizedElementIndex, Model.MaximizedElement);
		}
		protected FlowLayoutLayerInfo GetLayerInfo(int itemIndex) {
			for (int i = 0; i < LayerInfos.Count; i++)
				if (itemIndex < LayerInfos[i].FirstItemIndex)
					return i == 0 ? null : LayerInfos[i - 1];
			return LayerInfos.Count != 0 ? LayerInfos[LayerInfos.Count - 1] : null;
		}
		protected int GetSlotLastItemIndex(FlowLayoutLayerInfo layerInfo, int slotIndex) {
			if (slotIndex == layerInfo.SlotCount - 1) {
				int layerInfoIndex = LayerInfos.IndexOf(layerInfo);
				if (layerInfoIndex == LayerInfos.Count - 1)
					return LayoutItems.Count - 1;
				else
					return LayerInfos[layerInfoIndex + 1].FirstItemIndex - 1;
			}
			else
				return layerInfo.SlotFirstItemIndexes[slotIndex + 1] - 1;
		}
		protected virtual bool IsFlowBreak(FrameworkElement item, Rect bounds, double slotOffset, double slotLength, out bool isHardFlowBreak) {
			isHardFlowBreak = FlowLayoutControl.GetIsFlowBreak(item);
			return !StretchContent && (isHardFlowBreak || Parameters.BreakFlowToFit && slotOffset + slotLength > GetLayerContentEnd(bounds));
		}
		protected bool IsSlotWithMultipleItems(FrameworkElements items, int slotFirstItemIndex) {
			if (!NeedsFullSlot(items[slotFirstItemIndex]) && slotFirstItemIndex < items.Count - 1) {
				FrameworkElement subItem = items[slotFirstItemIndex + 1];
				if (!NeedsFullSlot(subItem) && !FlowLayoutControl.GetIsFlowBreak(subItem))
					return true;
			}
			return false;
		}
		protected void AddLayerSeparator(Rect areaBounds, double layerEnd, FlowLayoutItemSize separatorSize) {
			Parameters.LayerSeparators.Add(Orientation, GetLayerSeparatorBounds(areaBounds, layerEnd, separatorSize));
		}
		protected virtual void AddLayerSeparators(Rect bounds, Rect viewPortBounds) {
			FlowLayoutItemSize layerSeparatorSize = CalculateLayerSeparatorSize(viewPortBounds);
			double offset = GetLayerStart(bounds);
			for (int i = 0; i < Math.Max(1, LayerInfos.Count - 1); i++) {
				FlowLayoutLayerInfo layerInfo = LayerInfos[i];
				if (i > 0)
					offset += GetLayerDistance(layerInfo.IsHardFlowBreak);
				offset += layerInfo.Size.Width;
				AddLayerSeparator(bounds, offset, layerSeparatorSize);
			}
		}
		protected virtual Rect GetLayerSeparatorBounds(Rect areaBounds, double layerEnd, FlowLayoutItemSize separatorSize) {
			return GetItemBounds(new FlowLayoutItemPosition(layerEnd + Parameters.LayerSpace, GetLayerContentStart(areaBounds)), separatorSize);
		}
		protected Rect GetItemBounds(FlowLayoutItemPosition itemPosition, FlowLayoutItemSize itemSize) {
			if(Orientation == Orientation.Horizontal)
				return new Rect(itemPosition.ItemOffset, itemPosition.LayerOffset, itemSize.Length, itemSize.Width);
			else
				return new Rect(itemPosition.LayerOffset, itemPosition.ItemOffset, itemSize.Width, itemSize.Length);
		}
		protected void GetItemPositionAndSize(Rect itemBounds, out FlowLayoutItemPosition itemPosition, out FlowLayoutItemSize itemSize) {
			if(Orientation == Orientation.Horizontal) {
				itemPosition = new FlowLayoutItemPosition(itemBounds.Y, itemBounds.X);
				itemSize = new FlowLayoutItemSize(itemBounds.Height, itemBounds.Width);
			}
			else {
				itemPosition = new FlowLayoutItemPosition(itemBounds.X, itemBounds.Y);
				itemSize = new FlowLayoutItemSize(itemBounds.Width, itemBounds.Height);
			}
		}
		protected FlowLayoutItemSize GetItemSize(Size itemSize) {
			FlowLayoutItemPosition itemPosition;
			FlowLayoutItemSize result;
			GetItemPositionAndSize(RectHelper.New(itemSize), out itemPosition, out result);
			return result;
		}
		protected Size GetItemSize(FlowLayoutItemSize itemSize) {
			return GetItemBounds(new FlowLayoutItemPosition(0, 0), itemSize).Size();
		}
		protected double GetLayerStart(Rect bounds) {
			return Orientation == Orientation.Horizontal ? bounds.Top : bounds.Left;
		}
		protected double GetLayerWidth(Rect bounds) {
			return Orientation == Orientation.Horizontal ? bounds.Height : bounds.Width;
		}
		protected double GetLayerEnd(Rect bounds) {
			return Orientation == Orientation.Horizontal ? bounds.Bottom : bounds.Right;
		}
		protected double GetLayerCenter(Rect bounds) {
			return (GetLayerStart(bounds) + GetLayerEnd(bounds)) / 2;
		}
		protected double GetLayerOffset(Point p) {
			return Orientation == Orientation.Horizontal ? p.Y : p.X;
		}
		protected double GetLayerContentStart(Rect bounds) {
			return Orientation == Orientation.Horizontal ? bounds.Left : bounds.Top;
		}
		protected double GetLayerContentLength(Rect bounds) {
			return Orientation == Orientation.Horizontal ? bounds.Width : bounds.Height;
		}
		protected double GetLayerContentEnd(Rect bounds) {
			return GetLayerContentStart(bounds) + GetLayerContentLength(bounds);
		}
		protected double GetLayerContentCenter(Rect bounds) {
			return (GetLayerContentStart(bounds) + GetLayerContentEnd(bounds)) / 2;
		}
		protected double GetLayerContentOffset(Point p) {
			return Orientation == Orientation.Horizontal ? p.X : p.Y;
		}
		protected virtual Rect GetElementHitTestBounds(Rect elementBounds, Rect bounds, bool isFront, bool isBack) {
			FlowLayoutItemPosition elementPosition;
			FlowLayoutItemSize elementSize;
			GetItemPositionAndSize(elementBounds, out elementPosition, out elementSize);
			elementSize.Width += Parameters.ItemSpace;
			if (isFront) {
				double spaceLength = Math.Max(0, elementPosition.ItemOffset - GetLayerContentStart(bounds));
				elementPosition.ItemOffset -= spaceLength;
				elementSize.Length += spaceLength;
			}
			if (isBack)
				elementSize.Length = Math.Max(0, GetLayerContentEnd(bounds) - elementPosition.ItemOffset);
			else
				elementSize.Length += Parameters.ItemSpace;
			return GetItemBounds(elementPosition, elementSize);
		}
		protected Rect GetLayerBounds(FlowLayoutLayerInfo layerInfo) {
			return GetItemBounds(layerInfo.Position, layerInfo.Size);
		}
		protected Rect GetLayerSpaceBounds(FlowLayoutLayerInfo layerInfo, Rect bounds) {
			var layerSpacePosition = new FlowLayoutItemPosition(0, GetLayerContentStart(bounds));
			var layerSpaceSize = new FlowLayoutItemSize(0, GetLayerContentLength(bounds));
			if (layerInfo.FirstItemIndex == 0)
				layerSpaceSize.Width = Math.Max(0, layerInfo.Position.LayerOffset - GetLayerStart(bounds));
			else
				layerSpaceSize.Width = GetLayerDistance(layerInfo.IsHardFlowBreak);
			layerSpacePosition.LayerOffset = layerInfo.Position.LayerOffset - layerSpaceSize.Width;
			return GetItemBounds(layerSpacePosition, layerSpaceSize);
		}
		protected Rect GetRemainderBounds(Rect bounds, Rect lastLayerBounds) {
			FlowLayoutItemPosition position;
			FlowLayoutItemSize size;
			GetItemPositionAndSize(bounds, out position, out size);
			double layersWidth = GetLayerEnd(lastLayerBounds) - position.LayerOffset;
			position.LayerOffset += layersWidth;
			size.Width = Math.Max(0, size.Width - layersWidth);
			return GetItemBounds(position, size);
		}
		protected Rect GetSlotBounds(FlowLayoutLayerInfo layerInfo, int slotIndex) {
			FlowLayoutItemPosition slotPosition;
			FlowLayoutItemSize slotSize;
			GetItemPositionAndSize(GetLayerBounds(layerInfo), out slotPosition, out slotSize);
			int slotFirstItemIndex = layerInfo.SlotFirstItemIndexes[slotIndex];
			FlowLayoutItemPosition itemPosition;
			FlowLayoutItemSize itemSize;
			GetItemPositionAndSize(LayoutItems[slotFirstItemIndex].GetBounds(), out itemPosition, out itemSize);
			slotSize.Length = slotPosition.ItemOffset + slotSize.Length - itemPosition.ItemOffset;
			slotPosition.ItemOffset = itemPosition.ItemOffset;
			if (slotIndex < layerInfo.SlotCount - 1) {
				slotFirstItemIndex = layerInfo.SlotFirstItemIndexes[slotIndex + 1];
				GetItemPositionAndSize(LayoutItems[slotFirstItemIndex].GetBounds(), out itemPosition, out itemSize);
				slotSize.Length = itemPosition.ItemOffset - Parameters.ItemSpace - slotPosition.ItemOffset;
			}
			return GetItemBounds(slotPosition, slotSize);
		}
		protected virtual int GetSlotItemPlaceIndex(FlowLayoutLayerInfo layerInfo, int slotIndex, FrameworkElement item,
			Rect bounds, Point p, out bool isBeforeItemPlace) {
			int slotFirstItemIndex = layerInfo.SlotFirstItemIndexes[slotIndex];
			if (NeedsFullSlot(item) || NeedsFullSlot(LayoutItems[slotFirstItemIndex])) {
				isBeforeItemPlace = GetLayerContentOffset(p) < GetLayerContentCenter(GetSlotBounds(layerInfo, slotIndex));
				if (isBeforeItemPlace)
					return slotFirstItemIndex;
				else
					return GetSlotLastItemIndex(layerInfo, slotIndex);
			}
			isBeforeItemPlace = false;
			return -1;
		}
		protected new IFlowLayoutModel Model { get { return (IFlowLayoutModel)base.Model; } }
		protected List<FlowLayoutLayerInfo> LayerInfos { get; private set; }
		protected double LayerWidthChangeDirection {
			get {
				if(Model.MaximizedElement != null &&
					(Model.MaximizedElementPosition == MaximizedElementPosition.Left ||
					 Model.MaximizedElementPosition == MaximizedElementPosition.Top))
					return -1;
				else
					return 1;
			}
		}
		protected new FlowLayoutParameters Parameters { get { return base.Parameters as FlowLayoutParameters; } }
	}
	public class FlowLayoutController : LayoutControllerBase {
		private bool _AllowLayerSizing;
		public FlowLayoutController(IFlowLayoutControl control) : base(control) { }
		public new IFlowLayoutControl ILayoutControl { get { return base.ILayoutControl as IFlowLayoutControl; } }
		public new FlowLayoutProvider LayoutProvider { get { return (FlowLayoutProvider)base.LayoutProvider; } }	
		#region Drag&Drop
		public bool AllowLayerSizing {
			get { return _AllowLayerSizing; }
			set {
				if (_AllowLayerSizing != value) {
					_AllowLayerSizing = value;
					ILayoutControl.OnAllowLayerSizingChanged();
				}
			}
		}
		public bool AllowMaximizedElementMoving { get; set; }
		protected override bool CanDragAndDropItem(FrameworkElement item) {
			return base.CanDragAndDropItem(item) && item != ILayoutControl.MaximizedElement;
		}
		protected virtual bool CanResizeLayers() {
			return !double.IsInfinity(ILayoutControl.LayerWidth);
		}
		protected override DragAndDropController CreateItemDragAndDropControler(Point startDragPoint, FrameworkElement dragControl) {
			return new FlowLayoutItemDragAndDropController(this, startDragPoint, dragControl);
		}
		protected override bool WantsDragAndDrop(Point p, out DragAndDropController controller) {
			if (AllowMaximizedElementMoving && ILayoutControl.MaximizedElement != null) {
				var child = GetMoveableItem(p);
				if (child == ILayoutControl.MaximizedElement) {
					controller = new FlowLayoutMaximizedElementDragAndDropController(this, p);
					return true;
				}
			}
			if (AllowLayerSizing && CanResizeLayers()) {
				var child = IPanel.ChildAt(p, true, false, false);
				if (ILayoutControl.IsLayerSeparator(child)) {
					controller = new FlowLayoutLayerSizingController(this, p, (LayerSeparator)child);
					return true;
				}
			}
			return base.WantsDragAndDrop(p, out controller);
		}
		#endregion Drag&Drop
	}
	public class FlowLayoutItemDragAndDropController : LayoutItemDragAndDropControllerBase {
		public FlowLayoutItemDragAndDropController(Controller controller, Point startDragPoint, FrameworkElement dragControl)
			: base(controller, startDragPoint, dragControl) {
			if (ElementPositionsAnimation != null) {
				ElementPositionsAnimation.Stop();
				ElementPositionsAnimation = null;
			}
		}
		public override void DragAndDrop(Point p) {
			base.DragAndDrop(p);
			if (ElementPositionsAnimation != null && ElementPositionsAnimation.IsActive)
				return;
			FlowBreakKind itemPlaceFlowBreakKind;
			int itemPlaceVisibleIndex = Controller.LayoutProvider.GetItemPlaceIndex(DragControl, Controller.ClientBounds,
				GetItemPlacePoint(p), out itemPlaceFlowBreakKind);
			if (itemPlaceVisibleIndex == -1)
				return;
			int itemPlaceIndex = Controller.LayoutProvider.GetItemIndex(DragControlParent.Children, itemPlaceVisibleIndex);
			if (itemPlaceFlowBreakKind == FlowBreakKind.New && !Controller.ILayoutControl.AllowAddFlowBreaksDuringItemMoving)
				itemPlaceFlowBreakKind = itemPlaceVisibleIndex == 0 ? FlowBreakKind.Existing : FlowBreakKind.None;
			bool itemPlaceIsFlowBreak = itemPlaceFlowBreakKind != FlowBreakKind.None;
			FrameworkElements visibleChildren = Controller.LayoutProvider.LayoutItems;
			int placeHolderVisibleIndex = visibleChildren.IndexOf(DragControlPlaceHolder);
			int placeHolderIndex = DragControlParent.Children.IndexOf(DragControlPlaceHolder);
			bool placeHolderIsFlowBreak = placeHolderVisibleIndex == 0 || FlowLayoutControl.GetIsFlowBreak(DragControlPlaceHolder);
			bool placeHolderIsOneItemGroup = placeHolderIsFlowBreak &&
				(placeHolderVisibleIndex == visibleChildren.Count - 1 || FlowLayoutControl.GetIsFlowBreak(visibleChildren[placeHolderVisibleIndex + 1]));
			if (itemPlaceIndex > placeHolderIndex)
				itemPlaceIndex--;
			if (placeHolderIndex != itemPlaceIndex ||
				placeHolderVisibleIndex + 1 == itemPlaceVisibleIndex && itemPlaceIsFlowBreak &&
					(!placeHolderIsFlowBreak || itemPlaceFlowBreakKind == FlowBreakKind.Existing) ||
				itemPlaceVisibleIndex == placeHolderVisibleIndex &&
					(!itemPlaceIsFlowBreak && placeHolderIsFlowBreak ||
					 itemPlaceFlowBreakKind == FlowBreakKind.New && !placeHolderIsOneItemGroup)) {
				if (Controller.ILayoutControl.AnimateItemMoving) {
					ElementPositionsAnimation = new ElementBoundsAnimation(visibleChildren);
					ElementPositionsAnimation.StoreOldElementBounds();
				}
				if (placeHolderIsFlowBreak)
					if (placeHolderIsOneItemGroup)
						OnGroupFirstItemChanged(DragControlPlaceHolder, null);
					else
						OnGroupFirstItemChanged(DragControlPlaceHolder, visibleChildren[placeHolderVisibleIndex + 1]);
				if (itemPlaceFlowBreakKind == FlowBreakKind.Existing)
					OnGroupFirstItemChanged(visibleChildren[itemPlaceVisibleIndex], DragControlPlaceHolder);
				DragControlParent.Children.RemoveAt(placeHolderIndex);
				DragControlParent.Children.Insert(itemPlaceIndex, DragControlPlaceHolder);
				if (placeHolderIsFlowBreak && (placeHolderVisibleIndex > 0 || itemPlaceVisibleIndex == 0) && placeHolderVisibleIndex < visibleChildren.Count - 1)
					SetIsFlowBreakAndStoreOriginalValue(visibleChildren[placeHolderVisibleIndex + 1], true);
				if (itemPlaceIsFlowBreak && itemPlaceVisibleIndex < visibleChildren.Count)
					SetIsFlowBreakAndStoreOriginalValue(visibleChildren[itemPlaceVisibleIndex], itemPlaceFlowBreakKind == FlowBreakKind.New);
				FlowLayoutControl.SetIsFlowBreak(DragControlPlaceHolder, itemPlaceIsFlowBreak && itemPlaceVisibleIndex > 0);
				if (Controller.ILayoutControl.AnimateItemMoving) {
					Controller.Control.UpdateLayout();
					ElementPositionsAnimation.StoreNewElementBounds();
					ElementPositionsAnimation.Begin(Controller.ILayoutControl.ItemMovingAnimationDuration);
				}
#if SILVERLIGHT
				else
					Controller.Control.UpdateLayout();
#endif
			}
		}
		public override void EndDragAndDrop(bool accept) {
			if (ElementPositionsAnimation != null) {
				ElementPositionsAnimation.Stop();
				ElementPositionsAnimation = null;
			}
			if (Controller.ILayoutControl.AnimateItemMoving) {
				ElementPositionsAnimation = new ElementBoundsAnimation(new FrameworkElements { DragControl });
				ElementPositionsAnimation.StoreOldElementBounds(Controller.Control);
			}
			int dropIndex = DragControlParent.Children.IndexOf(DragControlPlaceHolder);
			bool dropIsFlowBreak = FlowLayoutControl.GetIsFlowBreak(DragControlPlaceHolder);
			base.EndDragAndDrop(accept);
			if (accept) {
				if (DragControlIndex != dropIndex || FlowLayoutControl.GetIsFlowBreak(DragControl) != dropIsFlowBreak) {
					int oldPosition = Controller.ILayoutControl.GetLogicalChildren(false).IndexOf(DragControl);
					DragControlParent.Children.Remove(DragControl);
					DragControlParent.Children.Insert(dropIndex, DragControl);
					FlowLayoutControl.SetIsFlowBreak(DragControl, dropIsFlowBreak);
					int newPosition = Controller.ILayoutControl.GetLogicalChildren(false).IndexOf(DragControl);
					Controller.ILayoutControl.OnItemPositionChanged(oldPosition, newPosition);
				}
				SendIsFlowBreakChangeNotifications();
			}
			else
				RestoreIsFlowBreakOriginalValues();
			if (Controller.ILayoutControl.AnimateItemMoving) {
#if !SILVERLIGHT
				object storedDragControlOpacity = DragControl.StorePropertyValue(UIElement.OpacityProperty);
				DragControl.Opacity = 0;
#endif
				Controller.Control.UpdateLayout();
				ElementPositionsAnimation.StoreNewElementBounds();
				object storedDragControlZIndex = DragControl.StorePropertyValue(Canvas.ZIndexProperty);
				DragControl.SetZIndex(PanelBase.HighZIndex);
				object storedDragControlIsHitTestVisible = DragControl.StorePropertyValue(UIElement.IsHitTestVisibleProperty);
				DragControl.IsHitTestVisible = false;
				ElementPositionsAnimation.Begin(FlowLayoutControl.ItemDropAnimationDuration, new ExponentialEase { Exponent = 5 },
					delegate {
						DragControl.RestorePropertyValue(Canvas.ZIndexProperty, storedDragControlZIndex);
						DragControl.RestorePropertyValue(UIElement.IsHitTestVisibleProperty, storedDragControlIsHitTestVisible);
						ElementPositionsAnimation = null;
					});
#if !SILVERLIGHT
				Dispatcher.CurrentDispatcher.BeginInvoke(new Action(delegate {
					DragControl.RestorePropertyValue(UIElement.OpacityProperty, storedDragControlOpacity);
				}), DispatcherPriority.Render);
#endif
			}
		}
		protected static ElementBoundsAnimation ElementPositionsAnimation { get; set; }
		protected virtual Point GetItemPlacePoint(Point p) {
			return p;
		}
		protected virtual void OnGroupFirstItemChanged(FrameworkElement oldValue, FrameworkElement newValue) { }
		protected void RestoreIsFlowBreakOriginalValues() {
			if (IsFlowBreakOriginalValues == null)
				return;
			foreach (KeyValuePair<UIElement, bool> originalValue in IsFlowBreakOriginalValues)
				FlowLayoutControl.SetIsFlowBreak(originalValue.Key, originalValue.Value);
		}
		protected void SendIsFlowBreakChangeNotifications() {
			if (IsFlowBreakOriginalValues == null)
				return;
			FrameworkElements children = Controller.ILayoutControl.GetLogicalChildren(false);
			foreach (KeyValuePair<UIElement, bool> originalValue in IsFlowBreakOriginalValues)
				if (FlowLayoutControl.GetIsFlowBreak(originalValue.Key) != originalValue.Value) {
					int position = children.IndexOf((FrameworkElement)originalValue.Key);
					Controller.ILayoutControl.OnItemPositionChanged(position, position);
				}
		}
		protected void SetIsFlowBreakAndStoreOriginalValue(UIElement element, bool value) {
			if (FlowLayoutControl.GetIsFlowBreak(element) == value)
				return;
			if (IsFlowBreakOriginalValues == null)
				IsFlowBreakOriginalValues = new Dictionary<UIElement, bool>();
			if (!IsFlowBreakOriginalValues.ContainsKey(element))
				IsFlowBreakOriginalValues.Add(element, FlowLayoutControl.GetIsFlowBreak(element));
			FlowLayoutControl.SetIsFlowBreak(element, value);
		}
		protected new FlowLayoutController Controller { get { return (FlowLayoutController)base.Controller; } }
		private Dictionary<UIElement, bool> IsFlowBreakOriginalValues { get; set; }
	}
	public class FlowLayoutMaximizedElementDragAndDropController : DragAndDropController {
		public FlowLayoutMaximizedElementDragAndDropController(Controller controller, Point startDragPoint)
			: base(controller, startDragPoint) {
		}
		public override void StartDragAndDrop(Point p) {
			OriginalMaximizedElementPosition = MaximizedElementPosition;
			base.StartDragAndDrop(p);
		}
		public override void DragAndDrop(Point p) {
			base.DragAndDrop(p);
			MaximizedElementPosition = GetMaximizedElementPosition(p) ?? MaximizedElementPosition;
		}
		public override void EndDragAndDrop(bool accept) {
			base.EndDragAndDrop(accept);
			if(!accept)
				MaximizedElementPosition = OriginalMaximizedElementPosition;
		}
		protected MaximizedElementPosition? GetMaximizedElementPosition(Point p) {
			var offset = PointHelper.Subtract(p, StartDragPoint);
			var noChangeOffset = DragImage.NoChangeAreaSize / 2;
			if(Math.Abs(offset.X) <= noChangeOffset && Math.Abs(offset.Y) <= noChangeOffset)
				return null;
			if(Math.Abs(offset.X) > Math.Abs(offset.Y))
				return offset.X < 0 ? MaximizedElementPosition.Left : MaximizedElementPosition.Right;
			else
				return offset.Y < 0 ? MaximizedElementPosition.Top : MaximizedElementPosition.Bottom;
		}
		protected new FlowLayoutController Controller { get { return (FlowLayoutController)base.Controller; } }
		protected MaximizedElementPosition MaximizedElementPosition {
			get { return Controller.ILayoutControl.MaximizedElementPosition; }
			set {
				if(MaximizedElementPosition != value) {
					Controller.ILayoutControl.MaximizedElementPosition = value;
					if(DragImage != null)
						DragImage.SelectedPosition = value;
				}
			}
		}
		protected MaximizedElementPosition OriginalMaximizedElementPosition { get; private set; }
		#region Drag Image
		protected override FrameworkElement CreateDragImage() {
			return new MaximizedElementPositionIndicator {
				SelectedPosition = MaximizedElementPosition,
				Style = Controller.ILayoutControl.MaximizedElementPositionIndicatorStyle
			};
		}
		protected override Point GetDragImagePosition(Point p) {
			var result = StartDragPoint;
			if(DragImage.ActualWidth == 0)
				DragImage.UpdateLayout();
			result.X -= Math.Round(DragImage.ActualWidth / 2);
			result.Y -= Math.Round(DragImage.ActualHeight / 2);
			return result;
		}
		protected new MaximizedElementPositionIndicator DragImage { get { return (MaximizedElementPositionIndicator)base.DragImage; } }
		#endregion Drag Image
	}
	[TemplatePart(Name = NoChangeElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = LeftElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = TopElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = RightElementName, Type = typeof(FrameworkElement))]
	[TemplatePart(Name = BottomElementName, Type = typeof(FrameworkElement))]
	public class MaximizedElementPositionIndicator : ControlBase {
		public const double DefaultNoChangeAreaSize = 30;
		private MaximizedElementPosition _SelectedPosition;
		public MaximizedElementPositionIndicator() {
			DefaultStyleKey = typeof(MaximizedElementPositionIndicator);
		}
		public double NoChangeAreaSize {
			get { return NoChangeElement != null ? NoChangeElement.ActualWidth : DefaultNoChangeAreaSize; }
		}
		public MaximizedElementPosition SelectedPosition {
			get { return _SelectedPosition; }
			set {
				if(SelectedPosition != value) {
					_SelectedPosition = value;
					UpdateTemplate();
				}
			}
		}
		#region Template
		const string NoChangeElementName = "NoChangeElement";
		const string LeftElementName = "LeftElement";
		const string TopElementName = "TopElement";
		const string RightElementName = "RightElement";
		const string BottomElementName = "BottomElement";
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			NoChangeElement = GetTemplateChild(NoChangeElementName) as FrameworkElement;
			LeftElement = GetTemplateChild(LeftElementName) as FrameworkElement;
			TopElement = GetTemplateChild(TopElementName) as FrameworkElement;
			RightElement = GetTemplateChild(RightElementName) as FrameworkElement;
			BottomElement = GetTemplateChild(BottomElementName) as FrameworkElement;
			UpdateTemplate();
		}
		protected virtual void UpdateTemplate() {
			if(LeftElement != null)
				LeftElement.SetVisible(SelectedPosition == MaximizedElementPosition.Left);
			if(TopElement != null)
				TopElement.SetVisible(SelectedPosition == MaximizedElementPosition.Top);
			if(RightElement != null)
				RightElement.SetVisible(SelectedPosition == MaximizedElementPosition.Right);
			if(BottomElement != null)
				BottomElement.SetVisible(SelectedPosition == MaximizedElementPosition.Bottom);
		}
		protected FrameworkElement NoChangeElement { get; set; }
		protected FrameworkElement LeftElement { get; set; }
		protected FrameworkElement TopElement { get; set; }
		protected FrameworkElement RightElement { get; set; }
		protected FrameworkElement BottomElement { get; set; }
		#endregion Template
	}
	public class FlowLayoutLayerSizingController : DragAndDropController {
		private double _LayerWidthChange;
		public FlowLayoutLayerSizingController(Controller controller, Point startDragPoint, LayerSeparator dragSeparator)
			: base(controller, startDragPoint) {
			DragSeparator = dragSeparator;
		}
		public override void StartDragAndDrop(Point p) {
			Controller.ILayoutControl.BringSeparatorsToFront();
			CreateChildCover();
			base.StartDragAndDrop(p);
		}
		public override void DragAndDrop(Point p) {
			base.DragAndDrop(p);
			LayerWidthChange = Controller.LayoutProvider.CalculateLayerWidthChange(DragSeparator,
				new Point(p.X - StartDragPoint.X, p.Y - StartDragPoint.Y));
		}
		public override void EndDragAndDrop(bool accept) {
			base.EndDragAndDrop(accept);
			DestroyChildCover();
			Controller.ILayoutControl.SendSeparatorsToBack();
			if (accept)
				Controller.ILayoutControl.LayerWidth += LayerWidthChange;
			else
				Controller.Control.InvalidateArrange();
		}
		public override IEnumerable<UIElement> GetInternalElements() {
			foreach (UIElement element in BaseGetInternalElements())
				yield return element;
			if (ChildCover != null)
				yield return ChildCover;
		}
		public override void OnMeasure(Size availableSize) {
			base.OnMeasure(availableSize);
			ChildCover.Measure(availableSize);
		}
		public override void OnArrange(Size finalSize) {
			base.OnArrange(finalSize);
			ChildCover.Arrange(RectHelper.New(ChildCover.DesiredSize));
		}
		public override Cursor DragCursor { get { return DragSeparator.Cursor; } }
		public override bool IsImmediateDragAndDrop { get { return true; } }
		protected void CreateChildCover() {
			ChildCover = new Rectangle();
			ChildCover.Fill = Controller.ILayoutControl.LayerSizingCoverBrush;
			ChildCover.SetSize(Controller.Control.GetSize());
			ChildCover.SetZIndex(LayerSeparators.TopMostSeparatorZIndex - 1);
			Controller.IPanel.Children.Add(ChildCover);
		}
		protected void DestroyChildCover() {
			Controller.IPanel.Children.Remove(ChildCover);
			ChildCover = null;
		}
		protected Rectangle ChildCover { get; set; }
		protected new FlowLayoutController Controller { get { return (FlowLayoutController)base.Controller; } }
		protected LayerSeparator DragSeparator { get; private set; }
		protected double LayerWidthChange {
			get { return _LayerWidthChange; }
			set {
				value = Math.Max(LayerWidthMinChange, Math.Min(value, LayerWidthMaxChange));
				if(_LayerWidthChange != value) {
					Controller.LayoutProvider.OffsetLayerSeparators(value - LayerWidthChange);
					_LayerWidthChange = value;
				}
			}
		}
		protected double LayerWidthMinChange {
			get { return Controller.ILayoutControl.LayerMinWidth - Controller.ILayoutControl.LayerWidth; }
		}
		protected double LayerWidthMaxChange {
			get { return Controller.ILayoutControl.LayerMaxWidth - Controller.ILayoutControl.LayerWidth; }
		}
		private IEnumerable<UIElement> BaseGetInternalElements() {
			return base.GetInternalElements();
		}
	}
	public abstract class MaximizableHeaderedContentControlBase : HeaderedContentControlBase {
		#region Dependency Properties
		public static readonly DependencyProperty CurrentContentTemplateProperty =
			DependencyProperty.Register("CurrentContentTemplate", typeof(DataTemplate), typeof(MaximizableHeaderedContentControlBase), null);
		public static readonly DependencyProperty MaximizedContentTemplateProperty =
			DependencyProperty.Register("MaximizedContentTemplate", typeof(DataTemplate), typeof(MaximizableHeaderedContentControlBase),
				new PropertyMetadata((o, e) => ((MaximizableHeaderedContentControlBase)o).OnMaximizedContentTemplateChanged()));
		#endregion Dependency Properties
		private bool _IsMaximizedCore;
		public MaximizableHeaderedContentControlBase() {
			UpdateCurrentContentTemplate();
		}
		public DataTemplate CurrentContentTemplate {
			get { return (DataTemplate)GetValue(CurrentContentTemplateProperty); }
			private set { SetValue(CurrentContentTemplateProperty, value); }
		}
		public DataTemplate MaximizedContentTemplate {
			get { return (DataTemplate)GetValue(MaximizedContentTemplateProperty); }
			set { SetValue(MaximizedContentTemplateProperty, value); }
		}
		protected virtual DependencyProperty GetCurrentContentTemplateProperty() {
			return IsMaximizedCore && MaximizedContentTemplate != null ? MaximizedContentTemplateProperty : ContentTemplateProperty;
		}
		protected virtual void OnIsMaximizedCoreChanged() {
			UpdateCurrentContentTemplate();
		}
		protected virtual void OnMaximizedContentTemplateChanged() {
			UpdateCurrentContentTemplate();
		}
		protected void UpdateCurrentContentTemplate() {
			SetBinding(CurrentContentTemplateProperty,
				new Binding { Source = this, Path = new PropertyPath(GetCurrentContentTemplateProperty()) });
		}
		protected bool IsMaximizedCore {
			get { return _IsMaximizedCore; }
			set {
				if (IsMaximizedCore == value)
					return;
				_IsMaximizedCore = value;
				OnIsMaximizedCoreChanged();
			}
		}
	}
}
