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

using System.Windows.Controls;
using DevExpress.Xpf.Utils;
using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.Native;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
#if !SL
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using DependencyPropertyChangedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventHandler;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
#endif
namespace DevExpress.Xpf.Grid {
	public class DetailHeaderControl : DetailHeaderControlBase {
#if DEBUGTEST
		internal static double? DebugTestFixedHeight = null;
#endif
		public DetailHeaderControl() {
#if DEBUGTEST
			if(DebugTestFixedHeight.HasValue)
				Height = DebugTestFixedHeight.Value;
#endif
			this.SetDefaultStyleKey(typeof(DetailHeaderControl));
		}
	}
	public class DetailContentControl : DetailHeaderControlBase {
#if DEBUGTEST
		internal static double? DebugTestFixedHeight = null;
#endif
		public DetailContentControl() {
#if DEBUGTEST
			if(DebugTestFixedHeight.HasValue)
				Height = DebugTestFixedHeight.Value;
#endif
			this.SetDefaultStyleKey(typeof(DetailContentControl));
		}
	}
	public class DetailTabHeadersControl : DetailTabHeadersControlBase {
		public DetailTabHeadersControl() {
			this.SetDefaultStyleKey(typeof(DetailTabHeadersControl));
		}
	}
	public class DetailTabHeaderContentControl : ContentControl {
		public DetailTabHeaderContentControl() {
			this.SetDefaultStyleKey(typeof(DetailTabHeaderContentControl));
		}
		protected override Size MeasureOverride(Size constraint) {
			return MeasurePixelSnapperHelper.MeasureOverride(base.MeasureOverride(constraint), SnapperType.Ceil);
		}
	}
	public class DetailColumnHeadersControl : DetailRowControlBase {
#if DEBUGTEST
		internal static double? DebugTestFixedHeight = null;
#endif
		public DetailColumnHeadersControl() {
#if DEBUGTEST
			if(DebugTestFixedHeight.HasValue)
				Height = DebugTestFixedHeight.Value;
#endif
			this.SetDefaultStyleKey(typeof(DetailColumnHeadersControl));
		}
	}
	public class DetailSummaryControlBase : DetailRowControlBase {
		public static readonly DependencyProperty SummaryDetailLevelProperty;
		static DetailSummaryControlBase() {
			SummaryDetailLevelProperty = DependencyPropertyManager.RegisterAttached("SummaryDetailLevel", typeof(int), typeof(DetailSummaryControlBase), new PropertyMetadata(-1));
		}
		public static int GetSummaryDetailLevel(DependencyObject element) {
			if(element == null) throw new ArgumentNullException("element");
			return (int)element.GetValue(SummaryDetailLevelProperty);
		}
		public static void SetSummaryDetailLevel(DependencyObject element, int value) {
			if(element == null) throw new ArgumentNullException("element");
			element.SetValue(SummaryDetailLevelProperty, value);
		}
	}
	public class DetailTotalSummaryControl : DetailSummaryControlBase {
#if DEBUGTEST
		internal static double? DebugTestFixedHeight = null;
#endif
		public DetailTotalSummaryControl() {
#if DEBUGTEST
			if(DebugTestFixedHeight.HasValue)
				Height = DebugTestFixedHeight.Value;
#endif
			this.SetDefaultStyleKey(typeof(DetailTotalSummaryControl));
		}
	}
	public class DetailFixedTotalSummaryControl : DetailSummaryControlBase {
#if DEBUGTEST
		internal static double? DebugTestFixedHeight = null;
#endif
		public DetailFixedTotalSummaryControl() {
#if DEBUGTEST
			if(DebugTestFixedHeight.HasValue)
				Height = DebugTestFixedHeight.Value;
#endif
			this.SetDefaultStyleKey(typeof(DetailFixedTotalSummaryControl));
		}
	}
	public class GroupPanelContainer : DetailControlPartContainer {
		public GroupPanelContainer() {
			this.SetDefaultStyleKey(typeof(GroupPanelContainer));
		}	   
	}
	public class FilterPanelContainer : DetailControlPartContainer {
		public FilterPanelContainer() {
			this.SetDefaultStyleKey(typeof(FilterPanelContainer));
		}
	}
	public class GroupPanelCaptionControl : ContentControl {
		public GroupPanelCaptionControl() {
			this.SetDefaultStyleKey(typeof(GroupPanelCaptionControl));
		}
	}
	public class DetailRowsIndentControl : ItemsControl {
		public static readonly DependencyProperty IsSummaryDetailLevelProperty;
		static DetailRowsIndentControl() {
			IsSummaryDetailLevelProperty = DependencyPropertyManager.RegisterAttached("IsSummaryDetailLevel", typeof(bool), typeof(DetailRowsIndentControl));
		}
		public static bool GetIsSummaryDetailLevel(DependencyObject element) {
			if(element == null) throw new ArgumentNullException("element");
			return (bool)element.GetValue(IsSummaryDetailLevelProperty);
		}
		public static void SetIsSummaryDetailLevel(DependencyObject element, bool value) {
			if(element == null) throw new ArgumentNullException("element");
			element.SetValue(IsSummaryDetailLevelProperty, value);
		}
		public DetailRowsIndentControl() {
			this.SetDefaultStyleKey(typeof(DetailRowsIndentControl));
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			element.SetValue(DataObjectBase.DataObjectProperty, DataContext);
			FrameworkElement elem = element as FrameworkElement;
			if(elem == null || !(elem.DataContext is DetailIndent)) return;
			bool isSummaryDetailLevel = ((DetailIndent)elem.DataContext).Level == DetailSummaryControlBase.GetSummaryDetailLevel(this);
			SetIsSummaryDetailLevel(element, isSummaryDetailLevel);
		}
	}
	public class DetailRowsIndentRightControl : ItemsControl {
		public static readonly DependencyProperty ItemsSourceToReverseProperty;
		static DetailRowsIndentRightControl() { 
			ItemsSourceToReverseProperty = DependencyPropertyManager.Register("ItemsSourceToReverse", typeof(IEnumerable), typeof(DetailRowsIndentRightControl), new FrameworkPropertyMetadata(null, (d,e) => ((DetailRowsIndentRightControl)d).OnItemsSourceToReverseChanged()));
		}
		public IEnumerable ItemsSourceToReverse {
			get { return (IEnumerable)GetValue(ItemsSourceToReverseProperty); }
			set { SetValue(ItemsSourceToReverseProperty, value); }
		}
		void OnItemsSourceToReverseChanged() {
			IList<DetailIndent> list = ItemsSourceToReverse as IList<DetailIndent>;
			if(list == null || list.Count < 2) {
				ItemsSource = ItemsSourceToReverse;
				return;
			}
			List<DetailIndent> revList = new List<DetailIndent>();
			for(int i = list.Count - 1; i >= 0; i--)
				revList.Add(list[i]);
			ItemsSource = revList;
		}
		public DetailRowsIndentRightControl() {
			this.SetDefaultStyleKey(typeof(DetailRowsIndentRightControl));
		}
	}
	public class DetailRowsContentControl : ContentControl {
		public static readonly DependencyProperty ViewProperty;
		public static readonly DependencyProperty DetailDescriptorProperty;
		public static readonly DependencyProperty NeedBottomLineProperty;
		static DetailRowsContentControl() {
			ViewProperty = DependencyPropertyManager.Register("View", typeof(DataViewBase), typeof(DetailRowsContentControl), new FrameworkPropertyMetadata(null));
			DetailDescriptorProperty = DependencyPropertyManager.Register("DetailDescriptor", typeof(DetailDescriptorBase), typeof(DetailRowsContentControl), new FrameworkPropertyMetadata(null));
			NeedBottomLineProperty = DependencyPropertyManager.Register("NeedBottomLine", typeof(bool), typeof(DetailRowsContentControl), new FrameworkPropertyMetadata(false));
		}
		public DataViewBase View {
			get { return (DataViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public DetailDescriptorBase DetailDescriptor {
			get { return (DetailDescriptorBase)GetValue(DetailDescriptorProperty); }
			set { SetValue(DetailDescriptorProperty, value); }
		}
		public bool NeedBottomLine {
			get { return (bool)GetValue(NeedBottomLineProperty); }
			set { SetValue(NeedBottomLineProperty, value); }
		}
		public DetailRowsContentControl() {
			this.SetDefaultStyleKey(typeof(DetailRowsContentControl));
		}
	}
	[DXToolboxBrowsable(false)]
	public class DetailTabControl : DXTabControl {
		public static readonly DependencyProperty RowDataProperty = DependencyPropertyManager.Register("RowData", typeof(TabsDetailInfo.TabHeadersRowData), typeof(DetailTabControl), new FrameworkPropertyMetadata(null, (d, e) => ((DetailTabControl)d).OnRowDataChanged((TabsDetailInfo.TabHeadersRowData)e.OldValue)));
		public static readonly DependencyProperty ActualSelectedIndexProperty =
			DependencyProperty.Register("ActualSelectedIndex", typeof(int), typeof(DetailTabControl), new PropertyMetadata(-1, (d,e) => ((DetailTabControl)d).OnActualSelectedIndexChanged()));
		public TabsDetailInfo.TabHeadersRowData RowData {
			get { return (TabsDetailInfo.TabHeadersRowData)GetValue(RowDataProperty); }
			set { SetValue(RowDataProperty, value); }
		}
		public int ActualSelectedIndex {
			get { return (int)GetValue(ActualSelectedIndexProperty); }
			set { SetValue(ActualSelectedIndexProperty, value); }
		}
		public DetailTabControl() {
			SelectionChanging += OnSelectionChanging;
			SelectionChanged += OnSelectionChanged;
		}
		void OnSelectionChanging(object sender, TabControlSelectionChangingEventArgs e) {
			if(RowData == null) return;
			RowData.SelectedIndexLocker.DoIfNotLocked(() => {
				e.Cancel = !RowData.CurrentTabsDetailInfo.MasterDataControl.DataView.CommitEditing();
			});
		}
		void OnSelectionChanged(object sender, TabControlSelectionChangedEventArgs e) {
			if(RowData == null) return;
			ActualSelectedIndex = e.NewSelectedIndex;
		}
		void OnActualSelectedIndexChanged() {
			SelectItem(ActualSelectedIndex);
		}
		void OnRowDataChanged(TabsDetailInfo.TabHeadersRowData oldRowData) {
			if(oldRowData != null)
				BindingOperations.ClearBinding(oldRowData, TabsDetailInfo.TabHeadersRowData.SelectedTabIndexProperty);
			if(RowData == null) return;
			RowData.SelectedIndexLocker.DoLockedAction(() => {
				SelectItem(RowData.SelectedTabIndex);
			});
			BindingOperations.SetBinding(RowData, TabsDetailInfo.TabHeadersRowData.SelectedTabIndexProperty, new Binding("ActualSelectedIndex") { Mode = BindingMode.TwoWay, Source = this });
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			FrameworkElement grid = LayoutHelper.FindElementByName(this, "PART_ContentContainer");
			grid.Height = 1;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			base.PrepareContainerForItemOverride(element, item);
			var tab = (DXTabItem)element;
			tab.Focusable = false;
			tab.IsTabStop = false;
		}
	}
	public class DetailDescriptorContentBorder : ContentControl {
		public static readonly DependencyProperty ShowBottomLineProperty =
			DependencyPropertyManager.Register("ShowBottomLine", typeof(bool), typeof(DetailDescriptorContentBorder), new UIPropertyMetadata(false));
		public bool ShowBottomLine {
			get { return (bool)GetValue(ShowBottomLineProperty); }
			set { SetValue(ShowBottomLineProperty, value); }
		}
		public DetailDescriptorContentBorder() {
			this.SetDefaultStyleKey(typeof(DetailDescriptorContentBorder));
		}
	}
	public class BackgroundBorderControl : ContentControl {
		public static readonly DependencyProperty RowLevelProperty =
			DependencyPropertyManager.Register("RowLevel", typeof(int), typeof(BackgroundBorderControl), new UIPropertyMetadata(0, (d, e) => ((BackgroundBorderControl)d).UpdateMargin()));
		public static readonly DependencyProperty ItemLevelProperty =
			DependencyPropertyManager.Register("ItemLevel", typeof(int), typeof(BackgroundBorderControl), new UIPropertyMetadata(0, (d, e) => ((BackgroundBorderControl)d).UpdateMargin()));
		public static readonly DependencyProperty LineLevelProperty =
			DependencyPropertyManager.Register("LineLevel", typeof(int), typeof(BackgroundBorderControl), new UIPropertyMetadata(0, (d, e) => ((BackgroundBorderControl)d).UpdateMargin()));
		public static readonly DependencyProperty IsMasterRowExpandedProperty =
			DependencyPropertyManager.Register("IsMasterRowExpanded", typeof(bool), typeof(BackgroundBorderControl), new UIPropertyMetadata(false, (d, e) => ((BackgroundBorderControl)d).UpdateMargin()));
		public int RowLevel {
			get { return (int)GetValue(RowLevelProperty); }
			set { SetValue(RowLevelProperty, value); }
		}
		public int ItemLevel {
			get { return (int)GetValue(ItemLevelProperty); }
			set { SetValue(ItemLevelProperty, value); }
		}
		public int LineLevel {
			get { return (int)GetValue(LineLevelProperty); }
			set { SetValue(LineLevelProperty, value); }
		}
		public bool IsMasterRowExpanded {
			get { return (bool)GetValue(IsMasterRowExpandedProperty); }
			set { SetValue(IsMasterRowExpandedProperty, value); }
		}
		public BackgroundBorderControl() {
			this.SetDefaultStyleKey(typeof(BackgroundBorderControl));
		}
		void UpdateMargin() {
			if((RowLevel - LineLevel < ItemLevel) && !IsMasterRowExpanded) {
				Margin = new Thickness(0, 0, 0, 1);
			} else {
				Margin = new Thickness(0);
			}
		}
	}
	public class DetailNewItemRowControl : DetailRowControlBase {
		public DetailNewItemRowControl() {
			this.SetDefaultStyleKey(typeof(DetailNewItemRowControl));
		}
	}
}
