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

using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;
using System.Linq;
using DevExpress.Xpf.Utils;
using System;
using System.ComponentModel;
using DevExpress.Utils;
using System.Windows.Controls;
using DevExpress.Xpf.Bars.Native;
namespace DevExpress.Xpf.Bars {
	[ContentProperty("Items")]
	public class GalleryItemGroup : FrameworkContentElement {
		#region static
		private static readonly object isCaptionVisibilityChangedEventHandler;
		public static readonly DependencyProperty GalleryProperty;
		public static readonly DependencyProperty IsVisibleProperty;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty IsCaptionVisibleProperty;
		public static readonly DependencyProperty ItemsSourceProperty;
		public static readonly DependencyProperty ItemStyleProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty ItemTemplateSelectorProperty;
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		static readonly DependencyProperty ItemsAttachedBehaviorProperty;
		#endregion
		static GalleryItemGroup() {
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(object), typeof(GalleryItemGroup), new FrameworkPropertyMetadata(null));
			GalleryProperty = DependencyPropertyManager.Register("Gallery", typeof(Gallery), typeof(GalleryItemGroup), new FrameworkPropertyMetadata(null, (d, e) => ((GalleryItemGroup)d).OnGalleryChanged(e.OldValue as Gallery)));
			IsVisibleProperty = DependencyPropertyManager.Register("IsVisible", typeof(bool), typeof(GalleryItemGroup), new FrameworkPropertyMetadata(true));
			IsCaptionVisibleProperty = DependencyPropertyManager.Register("IsCaptionVisible", typeof(DefaultBoolean), typeof(GalleryItemGroup),
				new FrameworkPropertyMetadata(DefaultBoolean.Default, new PropertyChangedCallback(OnIsCaptionVisiblePropertyChanged)));
			ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(GalleryItemGroup), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemsSourcePropertyChanged)));
			ItemStyleProperty = DependencyProperty.Register("ItemStyle", typeof(Style), typeof(GalleryItemGroup), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemStylePropertyChanged)));
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(GalleryItemGroup), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplatePropertyChanged)));
			ItemTemplateSelectorProperty = DependencyProperty.Register("ItemTemplateSelector", typeof(DataTemplateSelector), typeof(GalleryItemGroup), new System.Windows.PropertyMetadata(null, new System.Windows.PropertyChangedCallback(OnItemTemplateSelectorPropertyChanged)));
			ItemsAttachedBehaviorProperty = DependencyProperty.Register("ItemsAttachedBehavior", typeof(ItemsAttachedBehaviorCore<GalleryItemGroup, GalleryItem>), typeof(GalleryItemGroup), new System.Windows.PropertyMetadata(null));
			isCaptionVisibilityChangedEventHandler = new object();
		}
		static void OnIsCaptionVisiblePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) {
			((GalleryItemGroup)o).RaiseEventByHandler(isCaptionVisibilityChangedEventHandler, new EventArgs());
		}
		protected static void OnItemTemplateSelectorPropertyChanged(DependencyObject o, System.Windows.DependencyPropertyChangedEventArgs e) {
			((GalleryItemGroup)o).OnItemTemplateChanged(e);
		}
		protected static void OnItemTemplatePropertyChanged(DependencyObject o, System.Windows.DependencyPropertyChangedEventArgs e) {
			((GalleryItemGroup)o).OnItemTemplateChanged(e);
		}
		protected static void OnItemStylePropertyChanged(DependencyObject o, System.Windows.DependencyPropertyChangedEventArgs e) {
			((GalleryItemGroup)o).OnItemTemplateChanged(e);
		}
		protected static void OnItemsSourcePropertyChanged(DependencyObject o, System.Windows.DependencyPropertyChangedEventArgs e) {
			((GalleryItemGroup)o).OnItemsSourceChanged(e);
		}
		EventHandlerList events;
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		protected void RaiseEventByHandler(object eventHandler, EventArgs args) {
			EventHandler h = Events[eventHandler] as EventHandler;
			if(h != null) h(this, args);
		}
		protected internal event EventHandler CaptionVisibilityChanged {
			add { Events.AddHandler(isCaptionVisibilityChangedEventHandler, value); }
			remove { Events.RemoveHandler(isCaptionVisibilityChangedEventHandler, value); }
		}
		protected internal GalleryItemGroupControl GroupControl {
			get {
				return
					Gallery != null &&
					Gallery.GalleryControl != null &&
					Gallery.GalleryControl.GroupsControl != null &&
					Gallery.GalleryControl.GroupsControl.ItemContainerGenerator != null
					? Gallery.GalleryControl.GroupsControl.ItemContainerGenerator.ContainerFromItem(this) as GalleryItemGroupControl
					: null;
			}
		}
		internal void AddLogicalChildCore(object obj) {
			if(obj != null) {
				AddLogicalChild(obj);
			}
		}
		internal void RemoveLogicalChildCore(object obj) {
			if(obj != null) {
				RemoveLogicalChild(obj);
			}
		}
		WeakList<object> logicalChildrenCore = new WeakList<object>();
		new void AddLogicalChild(object obj) {
			base.AddLogicalChild(obj);
			logicalChildrenCore.Add(obj);
		}
		new void RemoveLogicalChild(object obj) {
			logicalChildrenCore.Remove(obj);
			base.RemoveLogicalChild(obj);
		}
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return logicalChildrenCore.GetEnumerator(); }
		}
		GalleryItemCollection itemsCore;
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGroupIsVisible")]
#endif
		public bool IsVisible {
			get { return (bool)GetValue(IsVisibleProperty); }
			set { SetValue(IsVisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGroupGallery")]
#endif
		public Gallery Gallery {
			get { return (Gallery)GetValue(GalleryProperty); }
			set { SetValue(GalleryProperty, value); }
		}
		[TypeConverter(typeof(ObjectConverter)),
#if !SL
	DevExpressXpfCoreLocalizedDescription("GalleryItemGroupCaption")
#else
	Description("")
#endif
]
		public object Caption {
			get { return (object)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGroupIsCaptionVisible")]
#endif
		public DefaultBoolean IsCaptionVisible {
			get { return (DefaultBoolean)GetValue(IsCaptionVisibleProperty); }
			set { SetValue(IsCaptionVisibleProperty, value); }
		}
		public GalleryItemGroup() {
			DefaultStyleKey = typeof(GalleryItemGroup);
			itemsCore = new GalleryItemCollection(this);			
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGroupItems")]
#endif
		public GalleryItemCollection Items {
			get {
				return itemsCore;
			}
			set {
				if(value == itemsCore) return;
				GalleryItemCollection oldValue = itemsCore;
				itemsCore = value;
			}
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGroupItemsSource")]
#endif
		public IEnumerable ItemsSource {
			get { return (IEnumerable)GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGroupItemTemplate")]
#endif
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGroupItemTemplateSelector")]
#endif
		public DataTemplateSelector ItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfCoreLocalizedDescription("GalleryItemGroupItemStyle")]
#endif
		public Style ItemStyle {
			get { return (Style)GetValue(ItemStyleProperty); }
			set { SetValue(ItemStyleProperty, value); }
		}
		void CloneDataTo(GalleryItemGroup targetObject) {
			targetObject.IsVisible = IsVisible;
			targetObject.Caption = Caption;
		}
		protected internal virtual void UncheckAllItems(GalleryItem ignorableItem) {
			foreach(GalleryItem item in Items) {
				if(item != ignorableItem)
					item.IsChecked = false;
			}
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			this.SafeOnPropertyChanged(e);
		}
		protected virtual void OnGalleryChanged(Gallery oldValue) {
			if(Gallery != null && Items.Count((i) => i.IsChecked) != 0)
				Gallery.UpdateCheckedItems();				
		}
		protected internal virtual void OnItemsSourceChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			var converter = new DevExpress.Xpf.Bars.Native.ObservableCollectionConverter<object, object>();
			converter.Source = e.NewValue as IEnumerable;
			converter.Selector = (item) => item;
			var args = new System.Windows.DependencyPropertyChangedEventArgs(e.Property, e.OldValue, converter);
			ItemsAttachedBehaviorCore<GalleryItemGroup, GalleryItem>.OnItemsSourcePropertyChanged(
				this,
				args,
				ItemsAttachedBehaviorProperty,
				ItemTemplateProperty,
				ItemTemplateSelectorProperty,
				ItemStyleProperty,
				gallery => gallery.Items,
				gallery => new GalleryItem(),
				null,
				null,
				null,
				(item, dataItem) => item.DataItem = dataItem, useDefaultTemplateSelector: true
				);
		}
		protected internal virtual void OnItemTemplateChanged(System.Windows.DependencyPropertyChangedEventArgs e) {
			if(ItemsSource == null) return;
			ItemsAttachedBehaviorCore<GalleryItemGroup, GalleryItem>.OnItemsGeneratorTemplatePropertyChanged(
				this,
				e,
				ItemsAttachedBehaviorProperty);
		}
		public GalleryItemGroup CloneWithEvents() {
			GalleryItemGroup clone = new GalleryItemGroup();
			foreach(GalleryItem item in Items) {
				clone.Items.Add(item.CloneWithEvents());
			}
			CloneDataTo(clone);
			return clone;
		}
		public GalleryItemGroup CloneWithoutEvents() {
			GalleryItemGroup clone = new GalleryItemGroup();
			foreach(GalleryItem item in Items) {
				clone.Items.Add(item.CloneWithoutEvents());
			}
			CloneDataTo(clone);
			return clone;
		}
	}
	public class GalleryItemCollection : GalleryCollection<GalleryItem> {
		GalleryItemGroup ParentGroup { get; set; }
		public GalleryItemCollection(GalleryItemGroup parentGroup) {
			ParentGroup = parentGroup;
		}
		protected override void ClearItem(GalleryItem item) {
			item.Group = null;			
				ParentGroup.RemoveLogicalChildCore(item);
		}
		protected override void PrepareItem(GalleryItem item) {
			item.Group = ParentGroup;   
			if(item.Parent == null)
				ParentGroup.AddLogicalChildCore(item);
		}
	}
}
