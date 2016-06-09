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
using System.ComponentModel;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Utils;
using System.Windows;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Mvvm.Native;
using System.Collections;
namespace DevExpress.Xpf.Ribbon {
	public class RibbonPagesControl : ItemsControl {
	#region static        
		public static readonly DependencyProperty SelectedPageProperty;		
		static RibbonPagesControl() {
			SelectedPageProperty = DependencyPropertyManager.Register("SelectedPage", typeof(RibbonPage), typeof(RibbonPagesControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnSelectedPagePropertyChanged)));
		}
		SpacingMode spacingMode;
		protected internal SpacingMode SpacingMode {
			get { return spacingMode; }
			set {
				if (value == spacingMode)
					return;
				SpacingMode oldValue = spacingMode;
				spacingMode = value;
				OnSpacingModeChanged(oldValue);
			}
		}
		protected virtual void OnSpacingModeChanged(SpacingMode oldValue) {
			for (int i = 0; i < this.Items.Count; i++) {
				var linkInfo = ItemContainerGenerator.ContainerFromIndex(i) as BarItemLinkInfo;
			}
		}
		protected static void OnSelectedPagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((RibbonPagesControl)d).OnSelectedPageChanged((RibbonPage)e.OldValue);
		}
		#endregion
		#region dep props
		public RibbonPage SelectedPage {
			get { return (RibbonPage)GetValue(SelectedPageProperty); }
			set { SetValue(SelectedPageProperty, value); }
		}
		#endregion
		#region props
		private RibbonPageCategoryBase pageCategoryCore = null;
		public RibbonSelectedPageControl SelectedPageControl {
			get { return ItemsControl.ItemsControlFromItemContainer(this) as RibbonSelectedPageControl; }
		}
		public RibbonPageCategoryBase PageCategory {
			get { return pageCategoryCore; }
			set {
				if(pageCategoryCore == value)
					return;
				RibbonPageCategoryBase oldValue = pageCategoryCore;
				pageCategoryCore = value;
				OnPageCategoryChanged(oldValue);
			}
		}
		AsyncRibbonPagesSourceCollection AsyncItemsSource { get { return ItemsSource as AsyncRibbonPagesSourceCollection; } }
		#endregion
		public RibbonPagesControl() {
			DefaultStyleKey = typeof(RibbonPagesControl);
		}
		public RibbonControl Ribbon { get { return SelectedPageControl == null ? null : SelectedPageControl.Ribbon; } }
		protected virtual void OnSelectedPageChanged(RibbonPage oldValue) {
			ForceAddSelectedPage();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is RibbonPageGroupsControl;
		}
		protected override System.Windows.DependencyObject GetContainerForItemOverride() {			
			return new RibbonPageGroupsControl();
		}		
		protected override void PrepareContainerForItemOverride(System.Windows.DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			RibbonPageGroupsControl pageGroupsControl = (RibbonPageGroupsControl)element;
			pageGroupsControl.Page = (RibbonPage)item;
			pageGroupsControl.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			base.ClearContainerForItemOverride(element, item);
			RibbonPageGroupsControl pageGroupsControl = (RibbonPageGroupsControl)element;
			pageGroupsControl.Page = null;
		}
		protected virtual void OnSelectedPageControlChanged(RibbonSelectedPageControl oldValue) {
			if(SelectedPageControl == null) ClearValue(SelectedPageProperty);
			SetBinding(SelectedPageProperty, new System.Windows.Data.Binding("SelectedPage") { Source = SelectedPageControl });
		}
		protected virtual void OnPageCategoryChanged(RibbonPageCategoryBase oldValue) {
			if(oldValue != null) {
				ItemsSource = null;
			}
			if (PageCategory == null) {
				ItemsSource = null;
				return;
			}
			if (Ribbon != null && Ribbon.LoadPagesInBackground)
				ItemsSource = new AsyncRibbonPagesSourceCollection(PageCategory);
			else
				ItemsSource = PageCategory.ActualPagesCore;
			ForceAddSelectedPage();
		}
		class AsyncRibbonPagesSourceCollection : AsyncObservableCollectionConverter<RibbonPage, RibbonPage> {
			RibbonPageCategoryBase category;
			RibbonControl Ribbon { get { return category.With(x => x.Ribbon).With(x => x.ActualMergedParent ?? x); } }
			public AsyncRibbonPagesSourceCollection(RibbonPageCategoryBase category) {
				this.category = category;
				Dispatcher = category.Dispatcher;
				SleepTime = Int32.MaxValue;
				Selector = new Func<RibbonPage, RibbonPage>(SelectorMethod);
				Source = category.ActualPagesCore;
			}
			protected override void OnReset() {
				base.OnReset();
				ForceAddSelectedPage();
			}
			protected override void OnAdd(int p, IList list) {
				base.OnAdd(p, list);
				ForceAddSelectedPage();
			}
			protected override void OnRemove(int p, IList list) {
				base.OnRemove(p, list);
				ForceAddSelectedPage();
			}
			public void ForceAddSelectedPage() {
				Ribbon.With(x => x.SelectedPage).Do(ForceAdd);
			}
			RibbonPage SelectorMethod(RibbonPage page) { return page; }
		}
		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnItemsChanged(e);
		}
		void ForceAddSelectedPage() {
			AsyncItemsSource.Do(x => x.ForceAddSelectedPage());
		}		
		protected internal virtual void RecreateEditors() {
			for(int i = 0; i < Items.Count; i++) {
				RibbonPageGroupsControl groupsControl = (RibbonPageGroupsControl)ItemContainerGenerator.ContainerFromIndex(i);
				if(groupsControl != null)
					groupsControl.RecreateEditors();
			}
		}
	}		
}
