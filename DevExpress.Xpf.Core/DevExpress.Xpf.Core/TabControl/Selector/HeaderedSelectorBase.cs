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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Core.Native {
	public abstract class HeaderedSelectorBase<TContainer, TItem> : SelectorBase<TContainer, TItem>
		where TContainer : HeaderedSelectorBase<TContainer, TItem>
		where TItem : HeaderedSelectorItemBase<TContainer, TItem> {
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ItemHeaderTemplateProperty =
			DependencyProperty.Register("ItemHeaderTemplate", typeof(DataTemplate), typeof(HeaderedSelectorBase<TContainer, TItem>),
			new FrameworkPropertyMetadata(null, (d, e) => ((HeaderedSelectorBase<TContainer, TItem>)d).OnItemHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ItemHeaderTemplateSelectorProperty =
			DependencyProperty.Register("ItemHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(HeaderedSelectorBase<TContainer, TItem>),
			new FrameworkPropertyMetadata(null, (d, e) => ((HeaderedSelectorBase<TContainer, TItem>)d).OnItemHeaderTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty ItemHeaderStringFormatProperty =
			DependencyProperty.Register("ItemHeaderStringFormat", typeof(string), typeof(HeaderedSelectorBase<TContainer, TItem>),
			new FrameworkPropertyMetadata(null, (d, e) => ((HeaderedSelectorBase<TContainer, TItem>)d).OnItemHeaderStringFormatChanged((string)e.OldValue, (string)e.NewValue)));
		public DataTemplate ItemHeaderTemplate {
			get { return (DataTemplate)GetValue(ItemHeaderTemplateProperty); }
			set { SetValue(ItemHeaderTemplateProperty, value); }
		}
		public DataTemplateSelector ItemHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ItemHeaderTemplateSelectorProperty); }
			set { SetValue(ItemHeaderTemplateSelectorProperty, value); }
		}
		public string ItemHeaderStringFormat {
			get { return (string)GetValue(ItemHeaderStringFormatProperty); }
			set { SetValue(ItemHeaderStringFormatProperty, value); }
		}
		protected virtual void OnItemHeaderTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			UpdateContainers(oldValue, ItemHeaderTemplateSelector, ItemHeaderStringFormat);
		}
		protected virtual void OnItemHeaderTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) {
			UpdateContainers(ItemHeaderTemplate, oldValue, ItemHeaderStringFormat);
		}
		protected virtual void OnItemHeaderStringFormatChanged(string oldValue, string newValue) {
			UpdateContainers(ItemHeaderTemplate, ItemHeaderTemplateSelector, oldValue);
		}
		void UpdateContainers(DataTemplate oldItemHeaderTemplate, DataTemplateSelector oldItemHeaderTemplateSelector, string oldItemHeaderStringFormat) {
			for(int i = 0; i < Items.Count; i++) {
				var container = GetContainer(i);
				if(container == null) continue;
				if(container.HeaderTemplate == oldItemHeaderTemplate)
					container.HeaderTemplate = ItemHeaderTemplate;
				if(container.HeaderTemplateSelector == oldItemHeaderTemplateSelector)
					container.HeaderTemplateSelector = ItemHeaderTemplateSelector;
				if(container.HeaderStringFormat == oldItemHeaderStringFormat)
					container.HeaderStringFormat = ItemHeaderStringFormat;
			}
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			TItem container = (TItem)element;
			FrameworkElement c = container;
			if(!(item is Visual) && !container.IsPropertySet(HeaderedSelectorItemBase<TContainer, TItem>.HeaderProperty))
				container.Header = item;
			if(ItemHeaderTemplate != null && !container.IsPropertySet(HeaderedSelectorItemBase<TContainer, TItem>.HeaderTemplateProperty))
				container.HeaderTemplate = ItemHeaderTemplate;
			if(ItemHeaderTemplateSelector != null && !container.IsPropertySet(HeaderedSelectorItemBase<TContainer, TItem>.HeaderTemplateSelectorProperty))
				container.HeaderTemplateSelector = ItemHeaderTemplateSelector;
			if(ItemHeaderStringFormat != null && !container.IsPropertySet(HeaderedSelectorItemBase<TContainer, TItem>.HeaderStringFormatProperty))
				container.HeaderStringFormat = ItemHeaderStringFormat;
			base.PrepareContainerForItemOverride(element, item);
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			TItem container = (TItem)element;
			if(container.Header == item)
				container.Header = null;
			base.ClearContainerForItemOverride(element, item);
		}
	}
	public abstract class HeaderedSelectorItemBase<TContainer, TItem> : SelectorItemBase<TContainer, TItem>
		where TContainer : HeaderedSelectorBase<TContainer, TItem>
		where TItem : HeaderedSelectorItemBase<TContainer, TItem> {
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HeaderProperty =
			HeaderedContentControl.HeaderProperty.AddOwner(typeof(HeaderedSelectorItemBase<TContainer, TItem>), new FrameworkPropertyMetadata(null,
				(d, e) => ((HeaderedSelectorItemBase<TContainer, TItem>)d).OnHeaderChanged(e.OldValue, e.NewValue)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HeaderTemplateProperty =
			HeaderedContentControl.HeaderTemplateProperty.AddOwner(typeof(HeaderedSelectorItemBase<TContainer, TItem>), new FrameworkPropertyMetadata(null,
				(d, e) => ((HeaderedSelectorItemBase<TContainer, TItem>)d).OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HeaderTemplateSelectorProperty =
			HeaderedContentControl.HeaderTemplateSelectorProperty.AddOwner(typeof(HeaderedSelectorItemBase<TContainer, TItem>), new FrameworkPropertyMetadata(null,
				(d, e) => ((HeaderedSelectorItemBase<TContainer, TItem>)d).OnHeaderTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
		[IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty HeaderStringFormatProperty =
			HeaderedContentControl.HeaderStringFormatProperty.AddOwner(typeof(HeaderedSelectorItemBase<TContainer, TItem>), new FrameworkPropertyMetadata(null,
				(d, e) => ((HeaderedSelectorItemBase<TContainer, TItem>)d).OnHeaderStringFormatChanged((string)e.OldValue, (string)e.NewValue)));
		public object Header { get { return GetValue(HeaderProperty); } set { SetValue(HeaderProperty, value); } }
		public DataTemplate HeaderTemplate { get { return (DataTemplate)GetValue(HeaderTemplateProperty); } set { SetValue(HeaderTemplateProperty, value); } }
		public DataTemplateSelector HeaderTemplateSelector { get { return (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty); } set { SetValue(HeaderTemplateSelectorProperty, value); } }
		public String HeaderStringFormat { get { return (String)GetValue(HeaderStringFormatProperty); } set { SetValue(HeaderStringFormatProperty, value); } }
		protected virtual void OnHeaderChanged(object oldValue, object newValue) { }
		protected virtual void OnHeaderTemplateChanged(DataTemplate oldValue, DataTemplate newValue) { }
		protected virtual void OnHeaderTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) { }
		protected virtual void OnHeaderStringFormatChanged(string oldValue, string newValue) { }
	}
}
