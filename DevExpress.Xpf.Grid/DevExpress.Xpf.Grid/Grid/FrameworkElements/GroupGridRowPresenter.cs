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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Xpf.Grid.Native;
#if !SILVERLIGHT
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class GroupGridRowPresenter : GridDataContentPresenter, IFocusedRowBorderObject {
		public GroupGridRowPresenter() {
			this.SetDefaultStyleKey(typeof(GroupGridRowPresenter));
			RowData.SetRowHandleBinding(this);
		}
		public FrameworkElement RowDataContent { get; set; }
		double IFocusedRowBorderObject.LeftIndent { get { return 0d; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			RowDataContent = this;
			RowData.ReassignCurrentRowData(this, GetTemplateChild("PART_GroupRowContent"));
		}
	}
	public class GroupSummaryContainer : ContentPresenter, INotifyCurrentRowDataChanged, INotifyPositionChanged {
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty GroupSummaryDisplayModeProperty =
			DependencyProperty.Register("GroupSummaryDisplayMode", typeof(GroupSummaryDisplayMode), typeof(GroupSummaryContainer), new PropertyMetadata(GroupSummaryDisplayMode.Default, (d, e) => ((GroupSummaryContainer)d).OnGroupSummaryDisplayModeChanged()));
		public GroupSummaryContainer() {
			ContentTemplate = null;
#if !SL
			ContentTemplateSelector = null;
#endif
		}
		GroupRowData RowData { get { return GroupRowData.GetCurrentRowData(this) as GroupRowData; } }
		void OnGroupSummaryDisplayModeChanged() {
			if(RowData == null) return;
			UpdateContent();
		}
		void UpdateContent() {
			bool alignGroupSummariesByColumns = RowData.View is TableView && ((TableView)RowData.View).GroupSummaryDisplayMode == GroupSummaryDisplayMode.AlignByColumns;
			if(alignGroupSummariesByColumns && !(Content is GroupSummaryAlignByColumnsLayoutControl))
				Content = new GroupSummaryAlignByColumnsLayoutControl();
			if(!alignGroupSummariesByColumns && !(Content is GroupSummaryDefaultLayoutControl))
				Content = new GroupSummaryDefaultLayoutControl();
		}
		#region INotifyCurrentRowDataChanged Members
		void INotifyCurrentRowDataChanged.OnCurrentRowDataChanged() {
			GroupRowData rowData = GroupRowData.GetCurrentRowData(this) as GroupRowData;
			if(rowData == null)
				return;
			if(rowData.View is TableView)
				SetBinding(GroupSummaryDisplayModeProperty, new Binding("View.GroupSummaryDisplayMode") { Source = rowData });
			UpdateContent();
		}
		#endregion
		#region INotifyPositionChanged Members
		void INotifyPositionChanged.OnPositionChanged(Rect newRect) {
			GroupSummaryAlignByColumnsLayoutControl groupSummaryAlignByColumnsLayoutControl = Content as GroupSummaryAlignByColumnsLayoutControl;
			if(groupSummaryAlignByColumnsLayoutControl != null)
				groupSummaryAlignByColumnsLayoutControl.SetContentOffset(-newRect.X);
		}
		#endregion
	}
	public class GroupSummaryDefaultLayoutControl : ItemsControl, ISupportLoadingAnimation {
		public bool IsReady {
			get { return (bool)GetValue(IsReadyProperty); }
			set { SetValue(IsReadyProperty, value); }
		}
		public static readonly DependencyProperty IsReadyProperty =
			DependencyProperty.Register("IsReady", typeof(bool), typeof(GroupSummaryDefaultLayoutControl), new PropertyMetadata(true, (d, e) => ((GroupSummaryDefaultLayoutControl)d).OnIsReadyChanged()));
		public GroupSummaryDefaultLayoutControl() {
			this.SetDefaultStyleKey(typeof(GroupSummaryDefaultLayoutControl));
			SetBinding(ItemsSourceProperty, new Binding("GroupSummaryData"));
			SetBinding(IsReadyProperty, new Binding("IsReady"));
		}
		LoadingAnimationHelper loadingAnimationHelper;
		internal LoadingAnimationHelper LoadingAnimationHelper {
			get {
				if(loadingAnimationHelper == null)
					loadingAnimationHelper = new LoadingAnimationHelper(this);
				return loadingAnimationHelper;
			}
		}
		void OnIsReadyChanged() {
			if(DataContext != null) LoadingAnimationHelper.ApplyAnimation();
		}
		public DataViewBase DataView { get { return ((RowData)DataContext).View; } }
		public FrameworkElement Element { get { return this; } }
		public bool IsGroupRow { get { return true; } }
	}
	public class GroupColumnSummaryControl : Control, ISupportLoadingAnimation {
		public bool IsReady {
			get { return (bool)GetValue(IsReadyProperty); }
			set { SetValue(IsReadyProperty, value); }
		}
		public static readonly DependencyProperty IsReadyProperty =
			DependencyProperty.Register("IsReady", typeof(bool), typeof(GroupColumnSummaryControl), new PropertyMetadata(true, (d, e) => ((GroupColumnSummaryControl)d).OnIsReadyChanged()));
		public Brush NormalBorderBrush {
			get { return (Brush)GetValue(NormalBorderBrushProperty); }
			set { SetValue(NormalBorderBrushProperty, value); }
		}
		public static readonly DependencyProperty NormalBorderBrushProperty =
			DependencyProperty.Register("NormalBorderBrush", typeof(Brush), typeof(GroupColumnSummaryControl), new PropertyMetadata(null, (d, e) => ((GroupColumnSummaryControl)d).UpdateBrushes()));
		public Brush FocusedBorderBrush {
			get { return (Brush)GetValue(FocusedBorderBrushProperty); }
			set { SetValue(FocusedBorderBrushProperty, value); }
		}
		public static readonly DependencyProperty FocusedBorderBrushProperty =
			DependencyProperty.Register("FocusedBorderBrush", typeof(Brush), typeof(GroupColumnSummaryControl), new PropertyMetadata(null, (d, e) => ((GroupColumnSummaryControl)d).UpdateBrushes()));
		public bool IsGroupRowFocused {
			get { return (bool)GetValue(IsGroupRowFocusedProperty); }
			set { SetValue(IsGroupRowFocusedProperty, value); }
		}
		public static readonly DependencyProperty IsGroupRowFocusedProperty =
			DependencyProperty.Register("IsGroupRowFocused", typeof(bool), typeof(GroupColumnSummaryControl), new PropertyMetadata(false, (d, e) => ((GroupColumnSummaryControl)d).UpdateBrushes()));
		public GroupColumnSummaryControl() {
			this.SetDefaultStyleKey(typeof(GroupColumnSummaryControl));
		}
		protected virtual void UpdateBrushes() {
			if(!IsGroupRowFocused || (!DataView.IsKeyboardFocusWithinView && DataView.FadeSelectionOnLostFocus)) {
				BorderBrush = NormalBorderBrush;
				return;
			}
			BorderBrush = FocusedBorderBrush;
		}
		LoadingAnimationHelper loadingAnimationHelper;
		internal LoadingAnimationHelper LoadingAnimationHelper {
			get {
				if(loadingAnimationHelper == null)
					loadingAnimationHelper = new LoadingAnimationHelper(this);
				return loadingAnimationHelper;
			}
		}
		void OnIsReadyChanged() {
			if(DataContext != null) LoadingAnimationHelper.ApplyAnimation();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Element = GetTemplateChild("PART_Content") as FrameworkElement;
			DataView.IsKeyboardFocusWithinViewChanged += new EventHandler((s, e) => UpdateBrushes());
		}
		public FrameworkElement Element { get; private set; }
		public DataViewBase DataView { get { return ((GridColumnData)DataContext).View; } }
		public bool IsGroupRow { get { return true; } }
	}
	public class GroupBandSummaryControl : GroupColumnSummaryControl {
		public static readonly DependencyProperty NormalForegroundProperty;
		public static readonly DependencyProperty FocusedForegroundProperty;
		public static readonly DependencyProperty NormalBackgroundProperty;
		public static readonly DependencyProperty FocusedBackgroundProperty;
		public static readonly DependencyProperty HasTopElementProperty;
		static GroupBandSummaryControl() {
			Type ownerType = typeof(GroupBandSummaryControl);
			NormalForegroundProperty = DependencyProperty.Register("NormalForeground", typeof(Brush), ownerType, new PropertyMetadata(null, (d, e) => ((GroupBandSummaryControl)d).UpdateBrushes()));
			FocusedForegroundProperty = DependencyProperty.Register("FocusedForeground", typeof(Brush), ownerType, new PropertyMetadata(null, (d, e) => ((GroupBandSummaryControl)d).UpdateBrushes()));
			NormalBackgroundProperty = DependencyProperty.Register("NormalBackground", typeof(Brush), ownerType, new PropertyMetadata(null, (d, e) => ((GroupBandSummaryControl)d).UpdateBrushes()));
			FocusedBackgroundProperty = DependencyProperty.Register("FocusedBackground", typeof(Brush), ownerType, new PropertyMetadata(null, (d, e) => ((GroupBandSummaryControl)d).UpdateBrushes()));
			HasTopElementProperty = DependencyProperty.Register("HasTopElement", typeof(bool), ownerType, new PropertyMetadata(false));
		}
		public GroupBandSummaryControl() {
			this.SetDefaultStyleKey(typeof(GroupBandSummaryControl));
		}
		public Brush NormalForeground {
			get { return (Brush)GetValue(NormalForegroundProperty); }
			set { SetValue(NormalForegroundProperty, value); }
		}
		public Brush FocusedForeground {
			get { return (Brush)GetValue(FocusedForegroundProperty); }
			set { SetValue(FocusedForegroundProperty, value); }
		}
		public Brush NormalBackground {
			get { return (Brush)GetValue(NormalBackgroundProperty); }
			set { SetValue(NormalBackgroundProperty, value); }
		}
		public Brush FocusedBackground {
			get { return (Brush)GetValue(FocusedBackgroundProperty); }
			set { SetValue(FocusedBackgroundProperty, value); }
		}
		public bool HasTopElement {
			get { return (bool)GetValue(HasTopElementProperty); }
			set { SetValue(HasTopElementProperty, value); }
		}
		protected override void UpdateBrushes() {
			base.UpdateBrushes();
			if(IsGroupRowFocused) {
				Foreground = FocusedForeground;
				Background = FocusedBackground;
				return;
			}
			Foreground = NormalForeground;
			Background = NormalBackground;
		}
	}
	public class GroupSummaryAlignByColumnsLayoutControl : Control {
		public GroupSummaryAlignByColumnsLayoutControl() {
			this.SetDefaultStyleKey(typeof(GroupSummaryAlignByColumnsLayoutControl));
		}
		BandedViewContentSelector selector;
		double offset;
		internal void SetContentOffset(double newOffset) {
			GroupRowData data = DataContext as GroupRowData;
			ITableView tableView = (TableView)data.View;
			newOffset -= (data != null) ? data.Level * tableView.LeftGroupAreaIndent : 0;
			newOffset += tableView.ActualShowDetailButtons ? tableView.ActualExpandDetailHeaderWidth : 0;
			newOffset -= tableView.ShowIndicator ? 0 : tableView.LeftDataAreaIndent;
			if(this.offset != newOffset) {
				this.offset = newOffset;
				UpdateSelector();
			}
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			selector = GetTemplateChild("PART_BandedViewContentSelector") as BandedViewContentSelector;
		}
		void UpdateSelector() {
			if(selector != null) {
				selector.Margin = new Thickness(offset, 0, 0, 0);
			}
		}
	}
	public class GroupSummaryItemsControl : CachedItemsControl {
		public GroupSummaryItemsControl() {
			this.SetDefaultStyleKey(typeof(GroupSummaryItemsControl));
		}
	}
}
