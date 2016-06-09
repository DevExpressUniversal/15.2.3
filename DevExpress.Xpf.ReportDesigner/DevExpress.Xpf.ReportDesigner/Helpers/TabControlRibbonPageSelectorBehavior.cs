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
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public class TabControlRibbonPageSelectorBehavior : Behavior<DXTabControl> {
		public static readonly DependencyProperty RibbonProperty;
		static TabControlRibbonPageSelectorBehavior() {
			DependencyPropertyRegistrator<TabControlRibbonPageSelectorBehavior>.New()
				.Register(d => d.Ribbon, out RibbonProperty, null, (d, e) => d.OnRibbonChanged(e))
			;
		}
		public RibbonControl Ribbon {
			get { return (RibbonControl)GetValue(RibbonProperty); }
			set { SetValue(RibbonProperty, value); }
		}
		void OnRibbonChanged(DependencyPropertyChangedEventArgs e) {
			var oldValue = (RibbonControl)e.OldValue;
			var newValue = (RibbonControl)e.NewValue;
			if(oldValue != null)
				oldValue.ActualCategories.CollectionChanged -= OnRibbonActualCategoriesCollectionChanged;
			if(newValue != null)
				newValue.ActualCategories.CollectionChanged += OnRibbonActualCategoriesCollectionChanged;
			UpdateDefaultPageCategory();
		}
		void OnRibbonActualCategoriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateDefaultPageCategory();
		}
		void UpdateDefaultPageCategory() {
			DefaultPageCategory = Ribbon.With(ribbon => ribbon.ActualCategories.FirstOrDefault(x => x.IsDefault));
		}
		RibbonPageCategoryBase defaultPageCategory;
		RibbonPageCategoryBase DefaultPageCategory {
			get { return defaultPageCategory; }
			set {
				if(value != defaultPageCategory) {
					RibbonPageCategoryBase oldValue = defaultPageCategory;
					defaultPageCategory = value;
					OnDefaultPageCategoryChanged(oldValue, value);
				}
			}
		}
		void OnDefaultPageCategoryChanged(RibbonPageCategoryBase oldValue, RibbonPageCategoryBase newValue) {
			if(oldValue != null)
				oldValue.Pages.CollectionChanged -= OnPagesCollectionChanged;
			if(newValue != null)
				newValue.Pages.CollectionChanged += OnPagesCollectionChanged;
			UpdateTabControlAsync();
		}
		void OnPagesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			UpdateTabControlAsync();
		}
		void UpdateTabControlAsync() {
			Dispatcher.BeginInvoke((Action)UpdateTabControl);
		}
		RibbonControl subscribedRibbon;
		void ClearTabControl() {
			if(subscribedRibbon != null)
				subscribedRibbon.SelectedPageChanged -= OnRibbonSelectedPageChanged;
			if(AssociatedObject == null) return;
			AssociatedObject.SelectionChanged -= OnAssociatedObjectSelectionChanged;
			AssociatedObject.Items.Clear();
			AssociatedObject.SelectedItem = null;
		}
		void UpdateTabControl() {
			ClearTabControl();
			if(AssociatedObject == null || DefaultPageCategory == null) return;
			foreach(var item in DefaultPageCategory.With(x => x.Pages.Select(RibbonPageInfo.New).ToArray()))
				AssociatedObject.Items.Add(item);
			subscribedRibbon = Ribbon;
			if(subscribedRibbon != null)
				subscribedRibbon.SelectedPageChanged += OnRibbonSelectedPageChanged;
			UpdateSelectedTabItem();
			AssociatedObject.SelectionChanged += OnAssociatedObjectSelectionChanged;
		}
		void OnRibbonSelectedPageChanged(object sender, RibbonPropertyChangedEventArgs e) {
			UpdateSelectedTabItem();
		}
		protected override void OnAttached() {
			base.OnAttached();
			UpdateTabControl();
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			ClearTabControl();
		}
		void UpdateSelectedTabItem() {
			AssociatedObject.Do(tabControl => tabControl.SelectedItem = RibbonPageInfo.New(Ribbon.With(x => x.SelectedPage)));
		}
		void OnAssociatedObjectSelectionChanged(object sender, TabControlSelectionChangedEventArgs e) {
			Ribbon.Do(ribbon => ribbon.SelectedPage = AssociatedObject.With(x => x.SelectedItem as RibbonPageInfo).With(x => x.RibbonPage));
		}
	}
}
