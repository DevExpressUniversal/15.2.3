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

using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections;
namespace DevExpress.Xpf.Controls.Native {
	public abstract class FooteredSelectorBase<TContainer, TItem> : SelectorBase<TContainer, TItem>
		where TContainer : FooteredSelectorBase<TContainer, TItem>
		where TItem : FooteredSelectorItemBase<TContainer, TItem> {
		static readonly DependencyPropertyKey SelectedItemFooterPropertyKey =
		  DependencyProperty.RegisterReadOnly("SelectedItemFooter", typeof(object), typeof(FooteredSelectorBase<TContainer, TItem>), null);
		static readonly DependencyPropertyKey SelectedItemFooterTemplatePropertyKey =
			DependencyProperty.RegisterReadOnly("SelectedItemFooterTemplate", typeof(DataTemplate), typeof(FooteredSelectorBase<TContainer, TItem>), null);
		static readonly DependencyPropertyKey SelectedItemFooterTemplateSelectorPropertyKey =
			DependencyProperty.RegisterReadOnly("SelectedItemFooterTemplateSelector", typeof(DataTemplateSelector), typeof(FooteredSelectorBase<TContainer, TItem>), null);
		static readonly DependencyPropertyKey SelectedItemFooterStringFormatPropertyKey =
			DependencyProperty.RegisterReadOnly("SelectedItemFooterStringFormat", typeof(String), typeof(FooteredSelectorBase<TContainer, TItem>), null);
		public static readonly DependencyProperty SelectedItemFooterProperty = SelectedItemFooterPropertyKey.DependencyProperty;
		public static readonly DependencyProperty SelectedItemFooterTemplateProperty = SelectedItemFooterTemplatePropertyKey.DependencyProperty;
		public static readonly DependencyProperty SelectedItemFooterTemplateSelectorProperty = SelectedItemFooterTemplateSelectorPropertyKey.DependencyProperty;
		public static readonly DependencyProperty SelectedItemFooterStringFormatProperty = SelectedItemFooterStringFormatPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ItemFooterTemplateProperty = DependencyProperty.Register("ItemFooterTemplate", typeof(DataTemplate), typeof(FooteredSelectorBase<TContainer, TItem>),
			new FrameworkPropertyMetadata(null, (d, e) => ((FooteredSelectorBase<TContainer, TItem>)d).OnItemFooterTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
		public static readonly DependencyProperty ItemFooterTemplateSelectorProperty = DependencyProperty.Register("ItemFooterTemplateSelector", typeof(DataTemplateSelector), typeof(FooteredSelectorBase<TContainer, TItem>),
			new FrameworkPropertyMetadata(null, (d, e) => ((FooteredSelectorBase<TContainer, TItem>)d).OnItemFooterTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
		public static readonly DependencyProperty ItemFooterStringFormatProperty = DependencyProperty.Register("ItemFooterStringFormat", typeof(string), typeof(FooteredSelectorBase<TContainer, TItem>),
			new FrameworkPropertyMetadata(null, (d, e) => ((FooteredSelectorBase<TContainer, TItem>)d).OnItemFooterStringFormatChanged((string)e.OldValue, (string)e.NewValue)));
		public DataTemplate ItemFooterTemplate { get { return (DataTemplate)GetValue(ItemFooterTemplateProperty); } set { SetValue(ItemFooterTemplateProperty, value); } }
		public DataTemplateSelector ItemFooterTemplateSelector { get { return (DataTemplateSelector)GetValue(ItemFooterTemplateSelectorProperty); } set { SetValue(ItemFooterTemplateSelectorProperty, value); } }
		public string ItemFooterStringFormat { get { return (string)GetValue(ItemFooterStringFormatProperty); } set { SetValue(ItemFooterStringFormatProperty, value); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object SelectedItemFooter { get { return GetValue(SelectedItemFooterProperty); } protected set { SetValue(SelectedItemFooterPropertyKey, value); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTemplate SelectedItemFooterTemplate { get { return (DataTemplate)GetValue(SelectedItemFooterTemplateProperty); } protected set { SetValue(SelectedItemFooterTemplatePropertyKey, value); } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DataTemplateSelector SelectedItemFooterTemplateSelector { get { return (DataTemplateSelector)GetValue(SelectedItemFooterTemplateSelectorProperty); } protected set { SetValue(SelectedItemFooterTemplateSelectorPropertyKey, value); } }
		public String SelectedItemFooterStringFormat { get { return (String)GetValue(SelectedItemFooterStringFormatProperty); } protected set { SetValue(SelectedItemFooterStringFormatPropertyKey, value); } }
		protected virtual void OnItemFooterTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			UpdateContainers(oldValue, ItemFooterTemplateSelector, ItemFooterStringFormat);
		}
		protected virtual void OnItemFooterTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) {
			UpdateContainers(ItemFooterTemplate, oldValue, ItemFooterStringFormat);
		}
		protected virtual void OnItemFooterStringFormatChanged(string oldValue, string newValue) {
			UpdateContainers(ItemFooterTemplate, ItemFooterTemplateSelector, oldValue);
		}
		void UpdateContainers(DataTemplate oldItemFooterTemplate, DataTemplateSelector oldItemFooterTemplateSelector, string oldItemFooterStringFormat) {
			for(int i = 0; i < Items.Count; i++) {
				var container = GetContainer(i);
				if(container == null) continue;
				if(container.FooterTemplate == oldItemFooterTemplate)
					container.FooterTemplate = ItemFooterTemplate;
				if(container.FooterTemplateSelector == oldItemFooterTemplateSelector)
					container.FooterTemplateSelector = ItemFooterTemplateSelector;
				if(container.FooterStringFormat == oldItemFooterStringFormat)
					container.FooterStringFormat = ItemFooterStringFormat;
			}
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			TItem container = (TItem)element;
			FrameworkElement c = container;
			if(!(item is Visual) && !container.IsPropertySet(FooteredSelectorItemBase<TContainer, TItem>.FooterProperty))
				container.Footer = item;
			if(ItemFooterTemplate != null && !container.IsPropertySet(FooteredSelectorItemBase<TContainer, TItem>.FooterTemplateProperty))
				container.FooterTemplate = ItemFooterTemplate;
			if(ItemFooterTemplateSelector != null && !container.IsPropertySet(FooteredSelectorItemBase<TContainer, TItem>.FooterTemplateSelectorProperty))
				container.FooterTemplateSelector = ItemFooterTemplateSelector;
			if(ItemFooterStringFormat != null && !container.IsPropertySet(FooteredSelectorItemBase<TContainer, TItem>.FooterStringFormatProperty))
				container.FooterStringFormat = ItemFooterStringFormat;
			base.PrepareContainerForItemOverride(element, item);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			TItem container = (TItem)element;
			if(container.Footer == item)
				container.Footer = null;
			base.ClearContainerForItemOverride(element, item);
		}
		protected override void UpdateSelectionProperties() {
			base.UpdateSelectionProperties();
			if(!IsIndexInRange(SelectedIndex)) {
				SelectedItemFooter = null;
				SelectedItemFooterTemplate = null;
				SelectedItemFooterTemplateSelector = null;
				SelectedItemFooterStringFormat = null;
				return;
			}
			TItem container = SelectedContainer;
			if(container == null) return;
			SelectedItemFooter = container.Footer;
			if(container.FooterTemplate != null || container.FooterTemplateSelector != null || container.FooterStringFormat != null) {
				SelectedItemFooterTemplate = container.FooterTemplate;
				SelectedItemFooterTemplateSelector = container.FooterTemplateSelector;
				SelectedItemFooterStringFormat = container.FooterStringFormat;
			} else {
				SelectedItemFooterTemplate = ItemFooterTemplate;
				SelectedItemFooterTemplateSelector = ItemFooterTemplateSelector;
				SelectedItemFooterStringFormat = ItemFooterStringFormat;
			}
		}
	}
	public abstract class FooteredSelectorItemBase<TContainer, TItem> : SelectorItemBase<TContainer, TItem>
		where TContainer : FooteredSelectorBase<TContainer, TItem>
		where TItem : FooteredSelectorItemBase<TContainer, TItem> {
		public static readonly DependencyProperty FooterProperty = DependencyProperty.Register("Footer", typeof(object), typeof(FooteredSelectorItemBase<TContainer, TItem>),
			new FrameworkPropertyMetadata(null, (d, e) => ((FooteredSelectorItemBase<TContainer, TItem>)d).OnFooterChanged(e.OldValue, e.NewValue)));
		public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.Register("FooterTemplate", typeof(DataTemplate), typeof(FooteredSelectorItemBase<TContainer, TItem>),
			new FrameworkPropertyMetadata(null, (d, e) => ((FooteredSelectorItemBase<TContainer, TItem>)d).OnFooterTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
		public static readonly DependencyProperty FooterTemplateSelectorProperty = DependencyProperty.Register("FooterTemplateSelector", typeof(DataTemplateSelector), typeof(FooteredSelectorItemBase<TContainer, TItem>),
			new FrameworkPropertyMetadata(null, (d, e) => ((FooteredSelectorItemBase<TContainer, TItem>)d).OnFooterTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
		public static readonly DependencyProperty FooterStringFormatProperty = DependencyProperty.Register("FooterStringFormat", typeof(string), typeof(FooteredSelectorItemBase<TContainer, TItem>),
			new FrameworkPropertyMetadata(null, (d, e) => ((FooteredSelectorItemBase<TContainer, TItem>)d).OnFooterStringFormatChanged((string)e.OldValue, (string)e.NewValue)));
		public object Footer { get { return GetValue(FooterProperty); } set { SetValue(FooterProperty, value); } }
		public DataTemplate FooterTemplate { get { return (DataTemplate)GetValue(FooterTemplateProperty); } set { SetValue(FooterTemplateProperty, value); } }
		public DataTemplateSelector FooterTemplateSelector { get { return (DataTemplateSelector)GetValue(FooterTemplateSelectorProperty); } set { SetValue(FooterTemplateSelectorProperty, value); } }
		public String FooterStringFormat { get { return (String)GetValue(FooterStringFormatProperty); } set { SetValue(FooterStringFormatProperty, value); } }
		protected override IEnumerator LogicalChildren {
			get {
				if(!(Footer is Visual)) return base.LogicalChildren;
				else return new MergedEnumerator(base.LogicalChildren, new SingleObjectEnumerator((DependencyObject)Footer));
			}
		}
		protected virtual void OnFooterChanged(object oldValue, object newValue) {
			(oldValue as Visual).Do(RemoveLogicalChild);
			(newValue as Visual).Do(AddLogicalChild);
		}
		protected virtual void OnFooterTemplateChanged(DataTemplate oldValue, DataTemplate newValue) { }
		protected virtual void OnFooterTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) { }
		protected virtual void OnFooterStringFormatChanged(string oldValue, string newValue) { }
	}
}
