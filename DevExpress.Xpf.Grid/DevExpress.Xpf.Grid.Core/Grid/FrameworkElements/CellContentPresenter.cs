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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
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
	public class CellContentPresenter : Control, INotifyNavigationIndexChanged, IGridCellEditorOwner {
		public static readonly DependencyProperty RowDataProperty;
		public static readonly DependencyProperty FieldNameProperty;
		public static readonly DependencyProperty ColumnProperty;
		public static readonly DependencyProperty ColumnPositionProperty;
		public static readonly DependencyProperty HasRightSiblingProperty;
		public static readonly DependencyProperty HasLeftSiblingProperty;
		public static readonly DependencyProperty HasTopElementProperty;
		public static readonly DependencyProperty ShowVerticalLinesProperty;
		public static readonly DependencyProperty ShowHorizontalLinesProperty;
		public static readonly DependencyProperty BorderStateProperty;
		public static readonly DependencyProperty IsSelectedProperty;
		public static readonly DependencyProperty IsFocusedCellProperty;
		public static readonly DependencyProperty SelectionStateProperty;
		public static readonly DependencyProperty IsReadyProperty;
		static CellContentPresenter() {
			RowDataProperty = DependencyPropertyManager.Register("RowData", typeof(RowData), typeof(CellContentPresenter), new PropertyMetadata(null, (d, e) => ((CellContentPresenter)d).OnRowDataChanged()));
			FieldNameProperty = DependencyPropertyManager.Register("FieldName", typeof(string), typeof(CellContentPresenter), new FrameworkPropertyMetadata(null, OnFieldNameChanged));
			ColumnProperty = DependencyPropertyManager.Register("Column", typeof(ColumnBase), typeof(CellContentPresenter), new FrameworkPropertyMetadata(null, OnColumnChanged));
			ColumnPositionProperty = DependencyPropertyManager.Register("ColumnPosition", typeof(ColumnPosition), typeof(CellContentPresenter), new FrameworkPropertyMetadata(ColumnPosition.Middle, (d, e) => ((CellContentPresenter)d).UpdateLineState()));
#if SL
			IsFocusedCellProperty = DependencyPropertyManager.Register("IsFocusedCell", typeof(bool), typeof(CellContentPresenter), new FrameworkPropertyMetadata(false, (d, e) => ((CellContentPresenter)d).OnIsFocusedCellChanged()));
#else
			IsFocusedCellProperty = DataViewBase.IsFocusedCellProperty.AddOwner(typeof(CellContentPresenter), new FrameworkPropertyMetadata((d, e) => ((CellContentPresenter)d).OnIsFocusedCellChanged()));
#endif
#if !SL //WPF back compatibility
			ColumnBase.NavigationIndexProperty.AddOwner(typeof(CellContentPresenter));
#endif
			HasRightSiblingProperty = DependencyPropertyManager.Register("HasRightSibling", typeof(bool), typeof(CellContentPresenter), new FrameworkPropertyMetadata(true, (d, e) => ((CellContentPresenter)d).UpdateLineState()));
			HasLeftSiblingProperty = DependencyPropertyManager.Register("HasLeftSibling", typeof(bool), typeof(CellContentPresenter), new FrameworkPropertyMetadata(true, (d, e) => ((CellContentPresenter)d).UpdateLineState()));
			HasTopElementProperty = DependencyPropertyManager.Register("HasTopElement", typeof(bool), typeof(CellContentPresenter), new FrameworkPropertyMetadata(false, (d, e) => ((CellContentPresenter)d).UpdateLineState()));
			ShowVerticalLinesProperty = DependencyPropertyManager.Register("ShowVerticalLines", typeof(bool), typeof(CellContentPresenter), new FrameworkPropertyMetadata(true, (d, e) => ((CellContentPresenter)d).UpdateLineState()));
			ShowHorizontalLinesProperty = DependencyPropertyManager.Register("ShowHorizontalLines", typeof(bool), typeof(CellContentPresenter), new FrameworkPropertyMetadata(true, (d, e) => ((CellContentPresenter)d).UpdateLineState()));
			BorderStateProperty = DependencyPropertyManager.Register("BorderState", typeof(Thickness), typeof(CellContentPresenter), new FrameworkPropertyMetadata(new Thickness()));
			IsSelectedProperty = DependencyPropertyManager.Register("IsSelected", typeof(bool), typeof(CellContentPresenter), new FrameworkPropertyMetadata(false));
			SelectionStateProperty = DependencyPropertyManager.Register("SelectionState", typeof(SelectionState), typeof(CellContentPresenter), new FrameworkPropertyMetadata(SelectionState.None, OnSelectionStateChanged));
			IsReadyProperty = DependencyPropertyManager.Register("IsReady", typeof(bool), typeof(CellContentPresenter), new FrameworkPropertyMetadata(true, OnIsReadyChanged));
		}
		static void OnFieldNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CellContentPresenter)d).OnFieldNameChanged();
		}
		static void OnColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CellContentPresenter)d).OnColumnChanged();
		}
		static void OnSelectionStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CellContentPresenter)d).UpdateSelectionState();
		}
		static void OnIsReadyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CellContentPresenter)d).OnIsReadyChanged();
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterRowData")]
#endif
		public RowData RowData {
			get { return (RowData)GetValue(RowDataProperty); }
			set { SetValue(RowDataProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterFieldName")]
#endif
		public string FieldName {
			get { return (string)GetValue(FieldNameProperty); }
			set { SetValue(FieldNameProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterColumn")]
#endif
		public ColumnBase Column {
			get { return (ColumnBase)GetValue(ColumnProperty); }
			set { SetValue(ColumnProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterColumnPosition")]
#endif
		public ColumnPosition ColumnPosition {
			get { return (ColumnPosition)GetValue(ColumnPositionProperty); }
			set { SetValue(ColumnPositionProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterHasRightSibling")]
#endif
		public bool HasRightSibling {
			get { return (bool)GetValue(HasRightSiblingProperty); }
			set { SetValue(HasRightSiblingProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterHasLeftSibling")]
#endif
		public bool HasLeftSibling {
			get { return (bool)GetValue(HasLeftSiblingProperty); }
			set { SetValue(HasLeftSiblingProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterHasTopElement")]
#endif
		public bool HasTopElement {
			get { return (bool)GetValue(HasTopElementProperty); }
			set { SetValue(HasTopElementProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterShowVerticalLines")]
#endif
		public bool ShowVerticalLines {
			get { return (bool)GetValue(ShowVerticalLinesProperty); }
			set { SetValue(ShowVerticalLinesProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterShowHorizontalLines")]
#endif
		public bool ShowHorizontalLines {
			get { return (bool)GetValue(ShowHorizontalLinesProperty); }
			set { SetValue(ShowHorizontalLinesProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterBorderState")]
#endif
		public Thickness BorderState {
			get { return (Thickness)GetValue(BorderStateProperty); }
			set { SetValue(BorderStateProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterIsSelected")]
#endif
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterIsFocusedCell")]
#endif
		public bool IsFocusedCell {
			get { return (bool)GetValue(IsFocusedCellProperty); }
			set { SetValue(IsFocusedCellProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterSelectionState")]
#endif
		public SelectionState SelectionState {
			get { return (SelectionState)GetValue(SelectionStateProperty); }
			set { SetValue(SelectionStateProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterIsReady")]
#endif
		public bool IsReady {
			get { return (bool)GetValue(IsReadyProperty); }
			set { SetValue(IsReadyProperty, value); }
		}
		internal CellEditorBase Editor {
			get;
			set;
		}
#if !SL //WPF back compatibility
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("CellContentPresenterNavigationIndex")]
#endif
		public int NavigationIndex {
			get { return (int)GetValue(ColumnBase.NavigationIndexProperty); }
			set { SetValue(ColumnBase.NavigationIndexProperty, value); }
		}
#else
		int NavigationIndex {
			get { return (int)GetValue(ColumnBase.NavigationIndexProperty); }
		}
#endif
		protected DataViewBase View { get { return RowData != null ? RowData.View : null; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			Editor = GetTemplateChild("PART_CellEditor") as CellEditorBase;
			if(Editor != null) {
				Editor.GridCellEditorOwner = this;
				UpdateEditorCellData();
			}
			OnIsFocusedCellChanged();
			OnNavigationIndexChanged();
			OnRowDataChanged();
			SyncColumn();
			SyncFieldName();
			UpdateSelectionState();
			UpdateLineState();
		}
		void UpdateEditorCellData() {
			if(Editor == null || Editor.CellData != null) return;
			Editor.CellData = GetCellData();
		}
		EditGridCellData GetCellData() {
			EditGridCellData cellData = DataContext as EditGridCellData;
			if(cellData != null)
				return cellData;
			return RowData != null ? (EditGridCellData)RowData.GetCellDataByColumn(Column) : null;
		}
		protected internal virtual void UpdateLineState() {
			GoToState(GetShowVerticalLineState());
			UpdateContentBorder();
		}
		void UpdateContentBorder() {
			double left = Column != null && ColumnPosition == ColumnPosition.Left && HasLeftSibling && ShowVerticalLines ? 1d : 0d;
			double top = HasTopElement && ShowHorizontalLines? 1d : 0d;
			double right = HasRightSibling && ShowVerticalLines ? 1d : 0d;
			Thickness borderThickness = new Thickness(left, top, right, 0);
			if(BorderState != borderThickness) {
				BorderState = borderThickness;
			}
		}
		protected internal virtual void UpdateRowSelectionState() {
			UpdateSelectionState();
			UpdateLineState();
		}
		void UpdateSelectionState() {
			GoToState(GetSelectionState());
		}
		protected internal virtual void OnIsReadyChanged(){
		}
		protected virtual string GetShowVerticalLineState() {
			return RowData != null && RowData.SelectionState == SelectionState.Focused ? "VisibleFocused" : "Visible";
		}
		protected virtual string GetSelectionState() {
			if(RowData == null) return "None";
			if(SelectionState == SelectionState.None && RowData.SelectionState == SelectionState.Focused)
				return "RowFocused";
			if(SelectionState == SelectionState.None && RowData.SelectionState == SelectionState.Selected)
				return "RowSelected";
			return Enum.GetName(typeof(SelectionState), SelectionState);
		}
		protected virtual void GoToState(string state){
			VisualStateManager.GoToState(this, state, false);
		}
		void SyncFieldName() {
			SyncCellEditor(FieldNameProperty, CellEditorBase.FieldNameProperty);
		}
		void SyncColumn() {
			SyncCellEditor(ColumnProperty, CellEditorBase.ColumnProperty);
		}
		void SyncCellEditor(DependencyProperty property, DependencyProperty editorProperty) {
			if(Editor != null)
				Editor.SetValue(editorProperty, GetValue(property));
		}
		internal virtual void SyncProperties(GridCellData cellData) { }
		void OnIsFocusedCellChanged() {
			if(Editor != null)
				Editor.IsFocusedCell = IsFocusedCell;
		}
		void OnNavigationIndexChanged() {
			if(Editor != null)
				Editor.NavigationIndex = NavigationIndex;
		}
		void OnFieldNameChanged() {
			if(View != null && Column == null && FieldName != null) {
				Column = View.ColumnsCore[FieldName];
			}
			SyncFieldName();
		}
		protected virtual void OnColumnChanged() {
			SyncColumn();
			UpdateEditorCellData();
		}
		protected virtual void OnRowDataChanged() {
			if(Editor != null)
				Editor.RowData = RowData;
			if(View != null)
				View.ViewBehavior.OnCellContentPresenterRowChanged(this);
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
#if !SL //WPF back compatibility
			if(e.Property == DataContextProperty && DataContext is RowData)
				RowData = DataContext as RowData;
#else
			if(e.Property == DataViewBase.IsFocusedCellProperty) {
				IsFocusedCell = (bool)e.NewValue;
			}
#endif
		}
		#region INotifyNavigationIndexChanged Members
		void INotifyNavigationIndexChanged.OnNavigationIndexChanged() {
			OnNavigationIndexChanged();
		}
		#endregion
		internal virtual bool CanRefreshContent() {
			return LayoutHelper.IsChildElement(RowData.RowElement, this);
		}
		#region IGridCellEditorOwner
		DependencyObject IGridCellEditorOwner.EditorRoot { get { return this; } }
		ColumnBase IGridCellEditorOwner.AssociatedColumn {
			get {
				if(DataContext is EditGridCellData)
					return ((EditGridCellData)DataContext).ColumnCore;
				return Column;
			}
		}
		bool IGridCellEditorOwner.CanRefreshContent { get { return CanRefreshContent(); } }
		void IGridCellEditorOwner.SynProperties(GridCellData cellData) { 
			SyncProperties(cellData); 
		}
		void IGridCellEditorOwner.UpdateIsReady() {
			IsReady = RowData.IsReady;
		}
		void IGridCellEditorOwner.UpdateCellState() {
			UpdateRowSelectionState();
		}
		void IGridCellEditorOwner.OnViewChanged() {
			View.ViewBehavior.OnCellContentPresenterRowChanged(this);
		}
		void IGridCellEditorOwner.SetSelectionState(SelectionState state) { }
		void IGridCellEditorOwner.SetIsFocusedCell(bool isFocusedCell) { }
		void IGridCellEditorOwner.UpdateCellBackgroundAppearance() { }
		void IGridCellEditorOwner.UpdateCellForegroundAppearance() { }
		#endregion
	}
}
