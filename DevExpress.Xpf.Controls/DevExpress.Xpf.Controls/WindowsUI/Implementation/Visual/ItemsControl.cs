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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace DevExpress.Xpf.WindowsUI.Base {
	interface IItemContainer {
		void PrepareContainer(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector);
		void ClearContainer();
	}
	interface IItemsPanel {
		ItemsControl ItemsControl { get; set; }
	}
	[TemplatePart(Name = "PART_ItemsPresenter", Type = typeof(ItemsPresenter))]
	public class veItemsControl : ItemsControl, IDisposable {
		#region static
#if SILVERLIGHT
		public static readonly DependencyProperty HasItemsProperty;
		public static readonly DependencyProperty ItemContainerStyleProperty;
#endif
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty HasItemsInternalProperty;
		static veItemsControl() {
			var dProp = new DependencyPropertyRegistrator<veItemsControl>();
#if SILVERLIGHT
			dProp.Register("HasItems", ref HasItemsProperty, (bool)false);
			dProp.Register("ItemContainerStyle", ref ItemContainerStyleProperty, (Style)null);
#endif
			dProp.Register("HasItemsInternal", ref HasItemsInternalProperty, (bool)false,
				(dObj, e) => ((veItemsControl)dObj).OnHasItemsChanged((bool)e.NewValue));
		}
		#endregion static
		public veItemsControl() {
			IsTabStop = false;
			SubscribeEvents();
			SetBinding(HasItemsInternalProperty, new Binding() { Path = new PropertyPath("HasItems"), Source = this });
		}
		public bool IsDisposing { get; private set; }
		void IDisposable.Dispose() {
			if(!IsDisposing) {
				IsDisposing = true;
				UnSubscribeEvents();
				OnDispose();
				ClearValue(ItemsSourceProperty);
				if(PartItemsPanel != null)
					ReleaseItemsPanelCore(PartItemsPanel);
				PartItemsPanel = null;
				PartItemsPresenter = null;
			}
			GC.SuppressFinalize(this);
		}
		#region Properties
#if SILVERLIGHT
		public bool HasItems {
			get { return (bool)GetValue(HasItemsProperty); }
		}
		public Style ItemContainerStyle {
			get { return (Style)GetValue(ItemContainerStyleProperty); }
			set { SetValue(ItemContainerStyleProperty, value); }
		}
#endif
		#endregion Properties
		protected virtual void SubscribeEvents() {
			Loaded += veItemsControl_Loaded;
			Unloaded += veItemsControl_Unloaded;
			SizeChanged += veItemsControl_SizeChanged;
			IsEnabledChanged += veItemsControl_IsEnabledChanged;
		}
		protected virtual void UnSubscribeEvents() {
			IsEnabledChanged -= veItemsControl_IsEnabledChanged;
			SizeChanged -= veItemsControl_SizeChanged;
			Loaded -= veItemsControl_Loaded;
			Unloaded -= veItemsControl_Unloaded;
		}
#if SILVERLIGHT
		public bool IsLoaded { get; private set; }
#endif
		void veItemsControl_Loaded(object sender, RoutedEventArgs e) {
#if SILVERLIGHT
			IsLoaded = true;
#endif
			OnLoaded();
		}
		void veItemsControl_Unloaded(object sender, RoutedEventArgs e) {
			OnUnloaded();
#if SILVERLIGHT
			IsLoaded = false;
#endif
		}
		void veItemsControl_SizeChanged(object sender, SizeChangedEventArgs e) {
			OnSizeChanged(e.NewSize);
		}
		void veItemsControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e) {
			OnIsEnabledChanged();
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
#if SILVERLIGHT
			SetValue(HasItemsProperty, Items.Count > 0);
#endif
			base.OnItemsChanged(e);
		}
		protected sealed override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			PrepareContainer(element, item);
#if SILVERLIGHT
			ApplyItemContainerStyle(element);
#endif
		}
#if SILVERLIGHT
		void ApplyItemContainerStyle(DependencyObject item) {
			if(item == null || ItemContainerStyle == null || item.GetType() != ItemContainerStyle.TargetType) return;
			item.SetValue(Control.StyleProperty, ItemContainerStyle);
		}
#endif
		protected virtual void PrepareContainer(DependencyObject element, object item) {
			if(element is IItemContainer) {
#if !SILVERLIGHT
				((IItemContainer)element).PrepareContainer(item, ItemTemplate, ItemTemplateSelector);
#else
				((IItemContainer)element).PrepareContainer(item, ItemTemplate, null);
#endif
			}
		}
		protected sealed override void ClearContainerForItemOverride(DependencyObject element, object item) {
			ClearContainer(element, item);
			base.ClearContainerForItemOverride(element, item);
		}
		protected virtual void ClearContainer(DependencyObject element, object item) {
			if(element is IItemContainer && element != item)
				((IItemContainer)element).ClearContainer();
		}
		protected internal ItemsPresenter PartItemsPresenter;
		protected internal Panel PartItemsPanel;
		public sealed override void OnApplyTemplate() {
			ClearTemplateChildren();
			BeforeApplyTemplate();
			base.OnApplyTemplate();
			GetTemplateChildren();
			OnApplyTemplateComplete();
		}
		protected virtual void ClearTemplateChildren() {
			if(PartItemsPanel != null && !LayoutTreeHelper.IsTemplateChild(PartItemsPanel, this)) {
				ReleaseItemsPanelCore(PartItemsPanel);
				PartItemsPanel = null;
			}
			if(PartItemsPresenter != null && !LayoutTreeHelper.IsTemplateChild(PartItemsPresenter, this))
				PartItemsPresenter.SizeChanged -= PartItemsPresenter_SizeChanged;
		}
		protected virtual void GetTemplateChildren() {
			PartItemsPresenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter ??
				LayoutTreeHelper.GetTemplateChild<ItemsPresenter, veItemsControl>(this);
			if(PartItemsPresenter != null)
				PartItemsPresenter.SizeChanged += PartItemsPresenter_SizeChanged; 
		}
		protected virtual void BeforeApplyTemplate() { }
		protected virtual void OnApplyTemplateComplete() { }
		void PartItemsPresenter_SizeChanged(object sender, SizeChangedEventArgs e) {
			if(PartItemsPresenter != null)
				PartItemsPresenter.SizeChanged -= PartItemsPresenter_SizeChanged;
			EnsureItemsPanel(PartItemsPresenter);
		}
		protected internal void EnsureItemsPanel(ItemsPresenter itemsPresenter, Panel panel = null) {
			if(PartItemsPanel == null) {
				PartItemsPanel = panel ?? LayoutTreeHelper.GetTemplateChild<Panel, ItemsPresenter>(itemsPresenter);
				if(PartItemsPanel != null)
					EnsureItemsPanelCore(PartItemsPanel);
			}
			else {
				if(panel != PartItemsPanel)
					PartItemsPanel.InvalidateMeasure();
			}
		}
		protected virtual void ReleaseItemsPanelCore(Panel itemsPanel) { }
		protected virtual void EnsureItemsPanelCore(Panel itemsPanel) { }
		protected virtual void OnDispose() { }
		protected virtual void OnLoaded() { }
		protected virtual void OnUnloaded() { }
		protected virtual void OnIsEnabledChanged() { }
		protected virtual void OnSizeChanged(Size size) { }
		protected virtual void OnHasItemsChanged(bool hasItems) { }
	}
}
namespace DevExpress.Xpf.WindowsUI.Internal {
	public class ItemsPresenter : ContentPresenter {
	}
}
