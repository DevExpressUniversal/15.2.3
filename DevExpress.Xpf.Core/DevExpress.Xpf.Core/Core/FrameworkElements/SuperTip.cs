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
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Utils;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using FrameworkContentElement = System.Windows.FrameworkElement;
#endif
namespace DevExpress.Xpf.Core {
	public class SuperTipItemControlBase : Control { 
		#region static
		public static readonly DependencyProperty ItemProperty;
		public static readonly DependencyProperty ActualLayoutStyleProperty;
		public static readonly DependencyProperty LayoutStyleProperty;
		public static readonly DependencyProperty UserLayoutStyleProperty;
		public static readonly DependencyProperty ContentStyleProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty UserContentTemplateProperty;
		public static readonly DependencyProperty ActualContentTemplateProperty;
		static SuperTipItemControlBase() {
			ItemProperty = DependencyPropertyManager.Register("Item", typeof(SuperTipItemBase), typeof(SuperTipItemControlBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnItemPropertyChanged)));
			LayoutStyleProperty = DependencyPropertyManager.Register("LayoutStyle", typeof(Style), typeof(SuperTipItemControlBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnLayoutStylePropertyChanged)));
			UserLayoutStyleProperty = DependencyPropertyManager.Register("UserLayoutStyle", typeof(Style), typeof(SuperTipItemControlBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnUserLayoutStylePropertyChanged)));
			ActualLayoutStyleProperty = DependencyPropertyManager.Register("ActualLayoutStyle", typeof(Style), typeof(SuperTipItemControlBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			ContentStyleProperty = DependencyPropertyManager.Register("ContentStyle", typeof(Style), typeof(SuperTipItemControlBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), typeof(SuperTipItemControlBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnContentTemplatePropertyChanged)));
			UserContentTemplateProperty = DependencyPropertyManager.Register("UserContentTemplate", typeof(DataTemplate), typeof(SuperTipItemControlBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnUserContentTemplatePropertyChanged)));
			ActualContentTemplateProperty = DependencyPropertyManager.Register("ActualContentTemplate", typeof(DataTemplate), typeof(SuperTipItemControlBase), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));
		}
		protected static void OnItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SuperTipItemControlBase)d).OnItemChanged(e);
		}
		protected static void OnLayoutStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SuperTipItemControlBase)d).OnLayoutStyleChanged(e);
		}
		protected static void OnUserLayoutStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SuperTipItemControlBase)d).OnUserLayoutStyleChanged(e);
		}
		protected static void OnContentTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SuperTipItemControlBase)d).OnContentTemplateChanged(e);
		}
		protected static void OnUserContentTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SuperTipItemControlBase)d).OnUserContentTemplateChanged(e);
		}
		#endregion
		public SuperTipItemControlBase() {
			DefaultStyleKey = GetType();
		}
		public SuperTipItemBase Item {
			get { return (SuperTipItemBase)GetValue(ItemProperty); }
			set { SetValue(ItemProperty, value); }
		}
		public Style LayoutStyle {
			get { return (Style)GetValue(LayoutStyleProperty); }
			set { SetValue(LayoutStyleProperty, value); }
		}
		public Style UserLayoutStyle {
			get { return (Style)GetValue(UserLayoutStyleProperty); }
			set { SetValue(UserLayoutStyleProperty, value); }
		}
		public Style ActualLayoutStyle {
			get { return (Style)GetValue(ActualLayoutStyleProperty); }
			set { SetValue(ActualLayoutStyleProperty, value); }
		}
		public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public DataTemplate UserContentTemplate {
			get { return (DataTemplate)GetValue(UserContentTemplateProperty); }
			set { SetValue(UserContentTemplateProperty, value); }
		}
		public DataTemplate ActualContentTemplate {
			get { return (DataTemplate)GetValue(ActualContentTemplateProperty); }
			set { SetValue(ActualContentTemplateProperty, value); }
		}
		public Style ContentStyle {
			get { return (Style)GetValue(ContentStyleProperty); }
			set { SetValue(ContentStyleProperty, value); }
		}
		protected virtual void OnItemChanged(DependencyPropertyChangedEventArgs e) {
			UpdateItemBinding();
		}
		protected virtual void OnLayoutStyleChanged(DependencyPropertyChangedEventArgs e) {
			UpdateActualLayout();
		}
		protected virtual void OnUserLayoutStyleChanged(DependencyPropertyChangedEventArgs e) {
			UpdateActualLayout();
		}
		protected virtual void OnContentTemplateChanged(DependencyPropertyChangedEventArgs e) {
			UpdateActualContentTemplate();
		}
		protected virtual void OnUserContentTemplateChanged(DependencyPropertyChangedEventArgs e) {
			UpdateActualContentTemplate();
		}
		protected virtual void UpdateItemBinding() {
#if SILVERLIGHT
			ClearValue(UserLayoutStyleProperty);
			ClearValue(UserContentTemplateProperty);
#else
			BindingOperations.ClearBinding(this, UserLayoutStyleProperty);
			BindingOperations.ClearBinding(this, UserContentTemplateProperty);
#endif
			if(Item as SuperTipItem != null) {
				Binding b = new Binding("LayoutStyle");
				b.Source = Item;
				b.Mode = BindingMode.OneWay;
				BindingOperations.SetBinding(this, UserLayoutStyleProperty, b);
				b = new Binding("ContentTemplate");
				b.Source = Item;
				b.Mode = BindingMode.OneWay;
				BindingOperations.SetBinding(this, UserContentTemplateProperty, b);
			}
		}
		protected virtual void UpdateActualLayout() {
			ActualLayoutStyle = UserLayoutStyle == null ? LayoutStyle : UserLayoutStyle;
		}
		protected virtual void UpdateActualContentTemplate() {
			ActualContentTemplate = UserContentTemplate == null ? ContentTemplate : UserContentTemplate;
		}
	}
	public class SuperTipItemControlSeparator : SuperTipItemControlBase {
	}
	public class SuperTipItemControl : SuperTipItemControlBase {
	}
	public class SuperTipHeaderItemControl : SuperTipItemControl { }
	[ContentProperty("SuperTip")]
	public class SuperTipControl : ItemsControl {
		#region static
		public static readonly DependencyProperty SuperTipProperty;
		static SuperTipControl() {
			SuperTipProperty = DependencyPropertyManager.Register("SuperTip", typeof(SuperTip), typeof(SuperTipControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnSuperTipPropertyChanged)));
		}
		protected static void OnSuperTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SuperTipControl)d).OnSuperTipChanged(e);
		}
		#endregion
		public SuperTipControl() { }
		public SuperTipControl(SuperTip superTip)
			: this() {
			SuperTip = superTip;
		}
		public SuperTip SuperTip {
			get { return (SuperTip)GetValue(SuperTipProperty); }
			set { SetValue(SuperTipProperty, value); }
		}
		protected virtual void OnSuperTipChanged(DependencyPropertyChangedEventArgs e) {
			ItemsSource = null;
			var oldValue = e.OldValue as SuperTip;
			if(oldValue != null) {
				oldValue.ClearValue(SuperTip.DataContextProperty);
			}
			if(SuperTip != null) {
				BindingOperations.SetBinding(SuperTip, SuperTip.DataContextProperty, new Binding() { Source = this, Path = new PropertyPath(DataContextProperty) });
				ItemsSource = SuperTip.Items;
			}
		}
		SuperTipItemBase CurrentItem { get; set; }
		protected override bool IsItemItsOwnContainerOverride(object item) {
			CurrentItem = item as SuperTipItemBase;
			return item is SuperTipItemControl;
		}
		protected override DependencyObject GetContainerForItemOverride() {
			if(CurrentItem is SuperTipItemSeparator)
				return new SuperTipItemControlSeparator();
			if(CurrentItem is SuperTipHeaderItem)
				return new SuperTipHeaderItemControl();
			if(CurrentItem is SuperTipItem)
				return new SuperTipItemControl();
			return base.GetContainerForItemOverride();
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			SuperTipItemControlBase control = element as SuperTipItemControlBase;
			control.Item = item as SuperTipItemBase;
			base.PrepareContainerForItemOverride(element, item);
		}
	}
	public class SuperTipItemsCollection : ObservableCollection<SuperTipItemBase> {
		public SuperTipItemsCollection(SuperTip superTip) {
			SuperTip = superTip;
		}
		public SuperTip SuperTip { get; set; }
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if(e.OldItems != null) {
				foreach(object obj in e.OldItems) {
					SuperTip.RemoveChild(obj);
				}
			}
			if(e.NewItems != null) {
				foreach(object obj in e.NewItems) {
					SuperTip.AddChild(obj);
				}
			}
		}
	}
	[ContentProperty("Items")]
#if !SILVERLIGHT
	public class SuperTip : FrameworkContentElement {
#else
	public class SuperTip : LogicalChildContainer {
#endif
		public SuperTip() { 
		}
		SuperTipItemsCollection items;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("SuperTipItems")]
#endif
public SuperTipItemsCollection Items {
			get {
				if(items == null)
					items = CreateItems();
				return items;
			}   
		}
		protected virtual SuperTipItemsCollection CreateItems() {
			return new SuperTipItemsCollection(this);
		}
		protected internal void AddChild(object child) {
			AddLogicalChild(child);
		}
		protected internal void RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
#if !SILVERLIGHT
		protected override IEnumerator LogicalChildren {
			get {
				List<object> children = new List<object>();
				IEnumerator logicalChildrenEnumerator = base.LogicalChildren;
				if(logicalChildrenEnumerator != null) {
					logicalChildrenEnumerator.Reset();
					while(logicalChildrenEnumerator.MoveNext()) {
						children.Add(logicalChildrenEnumerator.Current);
					}
				}
				foreach(SuperTipItemBase item in Items) {
					children.Add(item);
				}
				return children.GetEnumerator();
			}
		}
#endif
	}
	public class SuperTipItemBase : FrameworkContentElement {
	}
	public class SuperTipItemSeparator : SuperTipItemBase { 
	}
	public class SuperTipItem : SuperTipItemBase {
		#region static
		public static readonly DependencyProperty GlyphProperty;
		public static readonly DependencyProperty ContentProperty;
		public static readonly DependencyProperty ContentTemplateProperty;
		public static readonly DependencyProperty LayoutStyleProperty;
		static SuperTipItem() { 
			GlyphProperty = DependencyPropertyManager.Register("Glyph", typeof(ImageSource), typeof(SuperTipItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnGlyphPropertyChanged)));
			ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(SuperTipItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnContentPropertyChanged)));
			ContentTemplateProperty = DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), typeof(SuperTipItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnContentTemplatePropertyChanged)));
			LayoutStyleProperty = DependencyPropertyManager.Register("LayoutStyle", typeof(Style), typeof(SuperTipItem), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, new PropertyChangedCallback(OnLayoutStylePropertyChanged)));
		}
		protected static void OnGlyphPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SuperTipItem)d).OnGlyphChanged(e);
		}
		protected static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SuperTipItem)d).OnContentChanged(e);
		}
		protected static void OnContentTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SuperTipItem)d).OnContentTemplateChanged(e);
		}
		protected static void OnLayoutStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((SuperTipItem)d).OnLayoutStyleChanged(e);
		}
		#endregion
#if !SL
	[DevExpressXpfCoreLocalizedDescription("SuperTipItemGlyph")]
#endif
public ImageSource Glyph {
			get { return (ImageSource)GetValue(GlyphProperty); }
			set { SetValue(GlyphProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("SuperTipItemContent")]
#endif
public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("SuperTipItemContentTemplate")]
#endif
public DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("SuperTipItemLayoutStyle")]
#endif
public Style LayoutStyle {
			get { return (Style)GetValue(LayoutStyleProperty); }
			set { SetValue(LayoutStyleProperty, value); }
		}
		protected virtual  void OnGlyphChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void OnContentChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void OnContentTemplateChanged(DependencyPropertyChangedEventArgs e) {
		}
		protected virtual void OnLayoutStyleChanged(DependencyPropertyChangedEventArgs e) {
		}
	}
	public class SuperTipHeaderItem : SuperTipItem { 
	}
	public class SuperTipPanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			Size res = new Size();
			for(int i = 0; i < Children.Count; i++) {
				Children[i].Measure(availableSize);
				if(Children[i].Visibility == Visibility.Collapsed)
					continue;
				res.Width = Math.Max(res.Width, Children[i].DesiredSize.Width);
				res.Height += Children[i].DesiredSize.Height;
			}
			return res;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Point loc = new Point();
			Size size;
			for(int i = 0; i < Children.Count; i++) {
				if(Children[i].Visibility == Visibility.Collapsed)
					continue;
				size = Children[i].DesiredSize;
				if(Children[i] is SuperTipItemControlSeparator)
					size.Width = finalSize.Width;
				Children[i].Arrange(new Rect(loc, size));
				loc.Y += Children[i].DesiredSize.Height;
			}
			return finalSize;
		}
	}
}
