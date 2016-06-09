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
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using System.Windows.Media;
using System.ComponentModel;
using DevExpress.Xpf.Core.Native;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System;
using DevExpress.Xpf.Grid.Native;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Utils;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class BestFitGridCellContentPresenter : GridCellContentPresenter {
		internal override bool CanRefreshContent() {
			return true;
		}
		protected override void SyncWidth(GridCellData cellData) {
		}
	}
	public class GridCellContentPresenter : CellContentPresenter, ISupportLoadingAnimation {
		static GridCellContentPresenter() {			
		}
		GridCellData data;
		internal bool ShouldSyncProperties { get { return data == null; } }
		public GridCellContentPresenter() {
			this.SetDefaultStyleKey(typeof(GridCellContentPresenter));
		}
		internal GridCellContentPresenter(GridCellData data)
			: this() {
			this.data = data;
			DataContext = data;
		}
		LoadingAnimationHelper loadingAnimationHelper;
		internal LoadingAnimationHelper LoadingAnimationHelper {
			get {
				if(loadingAnimationHelper == null)
					loadingAnimationHelper = new LoadingAnimationHelper(this);
				return loadingAnimationHelper;
			}
		}
		protected internal override void OnIsReadyChanged() {
			LoadingAnimationHelper.ApplyAnimation();
		}
		protected override void OnRowDataChanged() {
			base.OnRowDataChanged();
			if(RowData != null) IsReady = RowData.IsReady;
		}
		public FrameworkElement Element { get { return Editor; } }
		public DataViewBase DataView { get { return View; } }
		public bool IsGroupRow { get { return false; } }
#if DEBUGTEST
		internal static int MeasureCount { get; private set; }
		internal static int ArrangeCount { get; private set; }
		protected override Size MeasureOverride(Size constraint) {
			MeasureCount++;
			return base.MeasureOverride(constraint);
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			ArrangeCount++;
			return base.ArrangeOverride(arrangeBounds);
		}
#endif
		internal override void SyncProperties(GridCellData cellData) {
			if(cellData.Column == null)
				return;
			DataContext = cellData;
			if(ShowVerticalLines != ((ITableView)cellData.View).ShowVerticalLines)
				ShowVerticalLines = ((ITableView)cellData.View).ShowVerticalLines;
			if(ShowHorizontalLines != ((ITableView)cellData.View).ShowHorizontalLines)
				ShowHorizontalLines = ((ITableView)cellData.View).ShowHorizontalLines;
			if(HasRightSibling != cellData.Column.HasRightSibling)
				HasRightSibling = cellData.Column.HasRightSibling;
			if(HasLeftSibling != cellData.Column.HasLeftSibling)
				HasLeftSibling = cellData.Column.HasLeftSibling;
			if(HasTopElement != cellData.Column.HasTopElement)
				HasTopElement = cellData.Column.HasTopElement;
			Style = cellData.Column.ActualCellStyle;
#if SL
			ApplyTemplate();
#endif
			SyncWidth(cellData);
			SyncLeftMargin(cellData);
			RowData = cellData.RowData;
			GridColumn.SetNavigationIndex(this, GridColumn.GetVisibleIndex(cellData.Column));
			Column = cellData.Column;
			cellData.OnEditorContentUpdated();
			ColumnPosition = cellData.Column.ColumnPosition;
			if(IsSelected != cellData.IsSelected)
				IsSelected = cellData.IsSelected;
			if(SelectionState != cellData.SelectionState)
				SelectionState = cellData.SelectionState;
			if(IsFocusedCell != cellData.IsFocusedCell)
				IsFocusedCell = cellData.IsFocusedCell;
			UpdateRowSelectionState();
		}
		void SyncLeftMargin(GridCellData cellData) {
			cellData.SyncLeftMargin(this);
		}
		protected virtual void SyncWidth(GridCellData cellData) {
			Width = cellData.GetActualCellWidth();
		}
		public override void OnApplyTemplate() {
			if(data != null) {
				SyncProperties(data);
				data = null;
			}
			base.OnApplyTemplate();
		}
	}
	public class FilterCellContentPresenter : CellContentPresenter {
		public FilterCellContentPresenter() {
			this.SetDefaultStyleKey(typeof(FilterCellContentPresenter));
		}
	}
	public class NewItemRowCellContentPresenter : CellContentPresenter {
		public NewItemRowCellContentPresenter() {
			this.SetDefaultStyleKey(typeof(NewItemRowCellContentPresenter));
		}
		internal override bool CanRefreshContent() {
			return true;
		}
	}
	public class CardCellContentPresenter : CellContentPresenter {
		public static readonly DependencyProperty CellStyleProperty;
		static CardCellContentPresenter() {
			CellStyleProperty = DependencyProperty.Register("CellStyle", typeof(Style), typeof(CardCellContentPresenter), new PropertyMetadata(null, (d, e) => ((CardCellContentPresenter)d).UpdateStyle()));
		}
		public Style CellStyle {
			get { return (Style)GetValue(CellStyleProperty); }
			set { SetValue(CellStyleProperty, value); }
		}
		public CardCellContentPresenter() {
			this.SetDefaultStyleKey(typeof(CardCellContentPresenter));
		}
		protected override void OnColumnChanged() {
			base.OnColumnChanged();
			UpdateStyle();
		}
		void UpdateStyle() {
			EditGridCellData cellData = DataContext as EditGridCellData;
			if(cellData == null || Column == null)
				return;
			if(cellData.View == Column.View)
				Style = CellStyle;
		}
		protected override Size MeasureOverride(Size constraint) {
			UpdateStyle();
			return base.MeasureOverride(constraint);
		}
	}
}
