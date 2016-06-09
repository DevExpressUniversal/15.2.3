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
using DevExpress.Xpf.Core;
using System.Windows.Media;
using System.Windows.Input;
using System;
using DevExpress.Xpf.Editors;
using System.Windows.Data;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
using System.Windows.Controls;
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
	public abstract class CellEditorBase : InplaceEditorBase {
		public static readonly DependencyProperty RowDataProperty;
		public static readonly DependencyProperty FieldNameProperty;
		public static readonly DependencyProperty ColumnProperty;
		public static readonly DependencyProperty IsFocusedCellProperty;
#if SL
		static ControlTemplate template = XamlHelper.GetControlTemplate("<ContentPresenter/>");
		public static readonly DependencyProperty FocusedRowForegroundProperty =
			DependencyPropertyManager.Register("FocusedRowForeground", typeof(Brush), typeof(CellEditorBase),
			new FrameworkPropertyMetadata((d, e) => ((CellEditorBase)d).OnFocusedForegroundChanged()));
		public static readonly DependencyProperty FocusedCellForegroundProperty =
			DependencyPropertyManager.Register("FocusedCellForeground", typeof(Brush), typeof(CellEditorBase),
			new FrameworkPropertyMetadata((d, e) => ((CellEditorBase)d).OnFocusedForegroundChanged()));
		public Brush FocusedRowForeground {
			get { return (Brush)GetValue(FocusedRowForegroundProperty); }
			set { SetValue(FocusedRowForegroundProperty, value); }
		}
		public Brush FocusedCellForeground {
			get { return (Brush)GetValue(FocusedCellForegroundProperty); }
			set { SetValue(FocusedCellForegroundProperty, value); }
		}
		protected virtual void OnFocusedForegroundChanged() {
			Foreground = FocusedCellForeground == null ? FocusedRowForeground : FocusedCellForeground;
		}
#endif
		static CellEditorBase() {
			RowDataProperty = DependencyPropertyManager.Register("RowData", typeof(RowData), typeof(CellEditorBase), new PropertyMetadata(null, (d, e) => ((CellEditorBase)d).OnRowDataChanged((RowData)e.OldValue)));
			FieldNameProperty = DependencyPropertyManager.Register("FieldName", typeof(string), typeof(CellEditorBase), new PropertyMetadata(null, (d, e) => ((CellEditorBase)d).OnFieldNameChanged()));
			ColumnProperty = DependencyPropertyManager.Register("Column", typeof(ColumnBase), typeof(CellEditorBase), new PropertyMetadata(null, (d, e) => ((CellEditorBase)d).OnColumnChanged((ColumnBase)e.OldValue, (ColumnBase)e.NewValue)));
			IsFocusedCellProperty = DependencyPropertyManager.Register("IsFocusedCell", typeof(bool), typeof(CellEditorBase), new PropertyMetadata(false, (d, e) => ((CellEditorBase)d).OnIsFocusedCellChanged()));
#if !SL //TODO SL
			ColumnBase.NavigationIndexProperty.AddOwner(typeof(CellEditorBase));
#endif
		}
		static bool GetIsInactiveEditorButtonVisible(EditorButtonShowMode editorShowMode, bool isFocusedCell, bool isFocusedRow) {
			switch(editorShowMode) {
				case EditorButtonShowMode.ShowAlways:
					return true;
				case EditorButtonShowMode.ShowOnlyInEditor:
					return false;
				case EditorButtonShowMode.ShowForFocusedRow:
					return isFocusedRow;
				default:
					return isFocusedCell;
			}
		}
		protected RowData RowDataCore { get; private set; }
		public RowData RowData {
			get { return (RowData)GetValue(RowDataProperty); }
			set { SetValue(RowDataProperty, value); }
		}
		public string FieldName {
			get { return (string)GetValue(FieldNameProperty); }
			set { SetValue(FieldNameProperty, value); }
		}
		internal ColumnBase ColumnCore { get; private set; }
		public ColumnBase Column {
			get { return (ColumnBase)GetValue(ColumnProperty); }
			set { SetValue(ColumnProperty, value); }
		}
		public int NavigationIndex {
			get { return (int)GetValue(ColumnBase.NavigationIndexProperty); }
			set { SetValue(ColumnBase.NavigationIndexProperty, value); }
		}
		public bool IsFocusedCell {
			get { return (bool)GetValue(IsFocusedCellProperty); }
			set { SetValue(IsFocusedCellProperty, value); }
		}
		IGridCellEditorOwner gridCellEditorOwner;
		internal IGridCellEditorOwner GridCellEditorOwner { get { return gridCellEditorOwner ?? NullGridCellEditorOwner.Instance; } set { gridCellEditorOwner = value; } }
		protected override DependencyObject EditorRoot { get { return GridCellEditorOwner.EditorRoot; } }
		protected sealed override InplaceEditorOwnerBase Owner { get { return View != null ? View.InplaceEditorOwner : null; } }
		protected sealed override IInplaceEditorColumn EditorColumn { get { return ColumnCore; } }
		protected virtual bool IsRowFocused { get { return RowHandle == View.FocusedRowHandle; } }
		internal DataViewBase View { get { return RowDataCore != null ? RowDataCore.View : null; } }
		protected DataControlBase DataControl { get { return View.DataControl; } }
		protected internal abstract int RowHandle { get; }
		protected string FieldNameCore { get { return Column.FieldName; } }
		public EditGridCellData CellData { get; internal set; }
		protected sealed override bool IsCellFocused { get { return IsFocusedCell; } }
		protected override void NullEditorInEditorDataContext() {
			if(CellData.Editor == this)
				CellData.Editor = null;
		}
		protected override void SetEditorInEditorDataContext() {
			CellData.Editor = this;
			UpdateConditionalAppearance();
		}
		public virtual void OnColumnChanged(ColumnBase oldValue, ColumnBase newValue) {
			ColumnCore = newValue;
			if(editCore != null && EditorSourceType == BaseEditSourceType.CellTemplate)
				BaseEditHelper.ApplySettings(editCore, EditorColumn.EditSettings, EditorColumn);
			OnOwnerChanged(oldValue);
			if(CellData != null)
				CellData.UpdateEditorHighlightingText(true);
		}
		protected override void UpdateContent(bool updateDisplayTemplate = true) {
			base.UpdateContent(updateDisplayTemplate);
			UpdateEditorDataContext();
		}
		protected override void UpdateEditorDataContext() {
			if(CellData != null) {
				CellData.OnEditorContentUpdated();
			}
		}
		protected override void OnColumnContentChanged(object sender, ColumnContentChangedEventArgs e) {
			if(e.Property == ColumnBase.DisplayTemplateProperty) {
				UpdateDisplayTemplate(true);
				return;
			}
			base.OnColumnContentChanged(sender, e);
		}
		protected CellEditorBase() {
#if SL
			Template = template;
#endif
		}
		protected sealed override object GetEditableValue() {
			return DataControl.GetCellValue(RowHandle, FieldNameCore);
		}
		protected override void OnShowEditor() {
			View.OnShowEditor(this);
		}
		protected sealed override void OnEditValueChanged(object sender, EditValueChangedEventArgs e) {
			OnEditValueChanged();
			UpdateEditorDataContextValue(e.NewValue);
			View.RaiseCellValueChanging(RowHandle, Column, Edit.EditValue, e.OldValue);
		}
		protected virtual void OnEditValueChanged() {
		}
		public override bool CanShowEditor() {
			return base.CanShowEditor() && View.CanShowEditor(RowHandle, Column);
		}
		protected sealed override bool RaiseShowingEditor() {
			return View.RaiseShowingEditor(RowHandle, Column);
		}
		protected override void OnHiddenEditor(bool closeEditor) {
			base.OnHiddenEditor(closeEditor);
			View.OnHideEditor(this, closeEditor);
			View.RaiseHiddenEditor(RowHandle, Column, editCore);
		}
		void OnFieldNameChanged() {
			if(Column == null && FieldName != null && View != null)
				Column = View.ColumnsCore[FieldName];
		}
		protected sealed override EditableDataObject GetEditorDataContext() {
			return CellData;
		}
		protected override DataTemplate SelectTemplate() {
			return Column.ActualCellTemplateSelector.SelectTemplate(CellData, this);
		}
		internal void UpdateEditableValue() {
			UpdateEditValue(editCore);
		}
		protected abstract void UpdateEditorDataContextValue(object newValue);
		protected override void UpdateEditValueCore(IBaseEdit editor) {
			BaseEditHelper.SetEditValue(editor, EditorDataContext.Value);
		}
#if !SL
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
			HitTestResult result = base.HitTestCore(hitTestParameters);
			return result == null ? new PointHitTestResult(this, hitTestParameters.HitPoint) : result;
		}
#endif
		protected sealed override bool HasEditorError() {
			return View.HasCellEditorError;
		}
		protected override void OnEditorActivated(object sender, RoutedEventArgs e) {
#if SL
			View.SetActiveEditor(editCore as BaseEdit);
#else
			View.SetActiveEditor();
#endif
			View.ViewBehavior.OnEditorActivated();
			View.RaiseShownEditor(RowHandle, Column, editCore);
		}
		protected sealed override bool IsInactiveEditorButtonVisible() {
			if(Column != null && !Column.GetAllowEditing() && AllowDefaultButton())
				return false;
			return GetIsInactiveEditorButtonVisible(View.EditorButtonShowMode, IsFocusedCell, IsRowFocused);
		}
		bool AllowDefaultButton() {
			var settings = Column.EditSettings as ButtonEditSettings;
			return settings != null && ((settings.AllowDefaultButton.HasValue && settings.AllowDefaultButton.Value) || !settings.AllowDefaultButton.HasValue);
		}
		protected virtual void OnRowDataChanged(RowData oldValue) {
			RowDataCore = RowData;
			UpdateData();
			UpdateContent();
		}
		protected virtual bool ShouldSyncCellContentPresenterProperties { get { return true; } }
		internal void SyncProperties() {
			if(ShouldSyncCellContentPresenterProperties)
				GridCellEditorOwner.SynProperties(CellData);
		}
		internal void UpdateIsReady() {
			GridCellEditorOwner.UpdateIsReady();
		}
		internal void UpdateCellState() {
			GridCellEditorOwner.UpdateCellState();
		}
		internal void OnViewChanged() {
			GridCellEditorOwner.OnViewChanged();
		}
		internal void OnDataChanged() {
			UpdateConditionalAppearance();
		}
		protected virtual void UpdateConditionalAppearance() { }
		protected internal virtual void UpdatePrintingMergeValue() { }
		protected override bool? GetAllowDefaultButton() {
			if(Column != null)
				return Column.GetAllowEditing();
			return base.GetAllowDefaultButton();
		}
	}
	public interface IGridCellEditorOwner {
		DependencyObject EditorRoot { get; }
		ColumnBase AssociatedColumn { get; }
		double ActualHeight { get; }
		bool CanRefreshContent { get; }
		void SynProperties(GridCellData cellData);
		void UpdateIsReady();
		void UpdateCellState();
		void OnViewChanged();
		void SetSelectionState(SelectionState state);
		void SetIsFocusedCell(bool isFocusedCell);
		void UpdateCellBackgroundAppearance();
		void UpdateCellForegroundAppearance();
	}
	public class NullGridCellEditorOwner : IGridCellEditorOwner {
		public static IGridCellEditorOwner Instance = new NullGridCellEditorOwner();
		NullGridCellEditorOwner() { }
		DependencyObject IGridCellEditorOwner.EditorRoot { get { return null; } }
		ColumnBase IGridCellEditorOwner.AssociatedColumn { get { return null; } }
		double IGridCellEditorOwner.ActualHeight { get { return 0d; } }
		bool IGridCellEditorOwner.CanRefreshContent { get { return false; } }
		void IGridCellEditorOwner.SynProperties(GridCellData cellData) { }
		void IGridCellEditorOwner.UpdateIsReady() { }
		void IGridCellEditorOwner.UpdateCellState() { }
		void IGridCellEditorOwner.OnViewChanged() { }
		void IGridCellEditorOwner.SetSelectionState(SelectionState state) { }
		void IGridCellEditorOwner.SetIsFocusedCell(bool isFocusedCell) { }
		void IGridCellEditorOwner.UpdateCellBackgroundAppearance() { }
		void IGridCellEditorOwner.UpdateCellForegroundAppearance() { }
	}
}
