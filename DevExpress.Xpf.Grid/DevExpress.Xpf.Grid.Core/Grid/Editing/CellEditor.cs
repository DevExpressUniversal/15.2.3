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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Utils;
using DevExpress.XtraEditors.DXErrorProvider;
using System.Windows.Input;
#if SL
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Utils;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Data.Browsing;
#else
using System.ComponentModel;
#endif
namespace DevExpress.Xpf.Grid {
	public class CellEditor : CellEditorBase {
		public CellEditor() {
			this.SetDefaultStyleKey(typeof(CellEditor));
#if !SL
			FocusVisualStyle = null;
#endif
		}
		protected override bool OptimizeEditorPerformance { get { return true; } }
		protected override bool IsInTree { get { return base.IsInTree && RowDataCore != null; } }
		protected internal override int RowHandle { get { return RowDataCore.RowHandleCore != null ? RowDataCore.RowHandleCore.Value : DataControlBase.InvalidRowHandle; } }
		protected override bool IsReadOnly {
			get {
#if SL
				bool requireColumn = RowHandle != DataControlBase.NewItemRowHandle;
#else
				bool requireColumn = true;
#endif
				return Column.ReadOnly || !Column.IsDisplayMemberBindingEditable || View.DataProviderBase.IsColumnReadonly(Column.FieldName, requireColumn);
			}
		}
		protected override bool OverrideCellTemplate { get { return Column == null || Column.OwnerControl == null; } }
		protected override bool CanRefreshContent() {
			return IsInTree && GridCellEditorOwner.CanRefreshContent;
		}
		protected override bool PostEditorCore() {
			if(View.HasCellEditorError) {
				return false;
			}
			if(HasAccessToCellValue) {
				if(!SkipSetValue()) {
					try {
						DataControl.SetCellValueCore(View.DataProviderBase.CurrentControllerRow, FieldNameCore, EditableValue);
					}
					catch(Exception e) {
						ShowError(View.CreateCellValidationError(e.Message, e, ErrorType.Default, RowData.RowHandle.Value, Column));
						return false;
					}
				}
				if(View != null && View.DataControl != null && !View.AllowLeaveInvalidEditor)
					RowData.UpdateData();
			}
			return true;
		}
		bool SkipSetValue() {
			RowData currentRowData = View.GetRowData(View.DataProviderBase.CurrentControllerRow);
			return currentRowData != null && RowData.Row != currentRowData.Row;
		}
		public override void ValidateEditorCore() {
			ShowError(GetValidationError(), View != null ? !View.AllowLeaveInvalidEditor : true);
		}
		protected override void OnHiddenEditor(bool closeEditor) {
			if(Column.ActualBinding != null)
				UpdateDisplayMemberBindingValue();
			CellData.UpdateValue();
			base.OnHiddenEditor(closeEditor);
		}
		void UpdateDisplayMemberBindingValue() {
			if (RowData.Row == null && RowData.RowHandle.Value == DXGridDataController.NewItemRow)
				RowData.Row = RowData.treeBuilder.GetRowValue(RowData);
			CellData.UpdateGridCellDataValue();
		}
		protected override void CancelRowEdit(KeyEventArgs e) {
			e.Handled = View.IsFocusedRowModified;
			View.CancelRowEdit();
		}
		protected override void OnEditValueChanged() {
			base.OnEditValueChanged();
			View.ViewBehavior.OnFocusedRowCellModified();
		}
		protected override IBaseEdit CreateEditor(BaseEditSettings settings) {
			IBaseEdit baseEdit = base.CreateEditor(settings);
			if(RowHandle == DataControlBase.NewItemRowHandle)
				baseEdit.DisableExcessiveUpdatesInInplaceInactiveMode = false;
			return baseEdit;
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			if((RowHandle == DataControlBase.NewItemRowHandle || View.EnterMoveNextColumn) && e.Key == Key.Enter && !IsEditorVisible) {
				View.MoveNextCell(false, true);
				Owner.EditorWasClosed = false;
			}
		}
		void ShowError(RowValidationError validationError, bool customValidate = true) {
			SetValidationError(null);
			if(validationError != null)
				SetValidationError(validationError);
			else
				RestoreValidationError(customValidate);
		}
		RowValidationError GetValidationError() {
			int rowHandle = RowData.RowHandle.Value;
			RowValidationError error = null;
			if(!Edit.DoValidate())
				error = RowValidationHelper.CreateEditorValidationError(View, rowHandle, null, Column);
			if(error != null)
				return error;
			if(!View.AllowCommitOnValidationAttributeError) {
				error = RowValidationHelper.ValidateAttributes(View, EditableValue, RowData.RowHandle.Value, Column);
				if(error != null)
					return error;
			}
			if(View.AllowLeaveInvalidEditor)
				return null;
			return RowValidationHelper.ValidateEvents(View, this, EditableValue, RowData.RowHandle.Value, Column);
		}
		void SetValidationError(RowValidationError validationError) {
			View.ValidationError = validationError;
			BaseEditHelper.SetValidationError(RowData, validationError);
			BaseEditHelper.SetValidationError(CellData, validationError);
			Edit.SetValidationError(validationError);
			RowData.UpdateIndicatorState();
		}
		protected void RestoreValidationError(bool customValidate) {
			View.ValidationError = null;
			RowData.UpdateDataErrors(customValidate);
		}
		protected override void RestoreValidationError() {
			RestoreValidationError(true);		   
		}
		protected override void UpdateValidationError() {
			Edit.SetValidationError(BaseEdit.GetValidationError(CellData));
		}
		protected override void OnRowDataChanged(RowData oldValue) {
			base.OnRowDataChanged(oldValue);
			if(!IsInTree)
				if(oldValue != null)
					ClearViewCurrentCellEditor(oldValue.View.InplaceEditorOwner);
		}
		protected override bool IsProperEditorSettings() {
			return EditSettingsComparer.IsCompatibleEditSettings(editCore, ColumnCore.ActualEditSettingsCore);
		}
		protected override void UpdateEditorDataContextValue(object newValue) {
			EditorDataContext.Value = newValue;
		}
#if DEBUGTEST
		public bool GetIsReadOnly() { return IsReadOnly; }
#endif 
	}
	internal static class RowValidationHelper {
		public static RowValidationError ValidateEvents(DataViewBase view, object source, object value, int rowHandle, ColumnBase column) {
			GridRowValidationEventArgs eventArgs = view.CreateCellValidationEventArgs(source, value, rowHandle, column);
			ValidationResult validationResult = null;
			Exception exception = null;
			try {
				view.OnValidation(column, eventArgs);
				validationResult = new ValidationResult(eventArgs.IsValid, eventArgs.ErrorContent);
			} catch(Exception e) {
				exception = e;
				validationResult = new ValidationResult(false, e.Message);
			}
			return validationResult.IsValid ? null : view.CreateCellValidationError(validationResult.ErrorContent, exception, eventArgs.ErrorType == ErrorType.None ? ErrorType.Default : eventArgs.ErrorType, rowHandle, column);
		}
		public static RowValidationError ValidateAttributes(DataViewBase view, object value, int rowHandle, ColumnBase column) {
			string error = column.GetValidationAttributesErrorText(value, rowHandle);
			if(!string.IsNullOrEmpty(error))
				return view.CreateCellValidationError(error, null, ErrorType.Default, rowHandle, column);
			return null;
		}
		public static RowValidationError CreateEditorValidationError(DataViewBase view, int rowHandle, Exception exception, ColumnBase column) {
			object errorContent = exception == null ? view.GetLocalizedString(GridControlStringId.InvalidValueErrorMessage) : exception.Message;
			return view.CreateCellValidationError(errorContent, null, ErrorType.Default, rowHandle, column);
		}
	}
	public class FilterRowCellEditor : CellEditorBase {
		EditSettingsChangedEventHandler<FilterRowCellEditor> EditSettingsChangedEventHandler { get; set; }
		public FilterRowCellEditor() : base() {
			DataViewBase.SetRowHandle(this, new RowHandle(RowHandle));
			EditSettingsChangedEventHandler = new EditSettingsChangedEventHandler<FilterRowCellEditor>(this, (owner, o, e) => owner.EditSettingsChanged(o, e));
		}
		protected internal override int RowHandle { get { return DataControlBase.AutoFilterRowHandle; } }
		protected override bool IsReadOnly { get { return !Column.AllowAutoFilter || View.ColumnsCore[Column.FieldName] == null; } }
		protected override bool OverrideCellTemplate { get { return true; } }
		protected override void UpdateEditContext() {
		}
		protected override void UpdateDisplayTemplate(bool updateForce = false) {
			if(!updateForce && Column.AutoFilterRowDisplayTemplate == null)
				return;
			Edit.SetDisplayTemplate(Column.AutoFilterRowDisplayTemplate);
		}
		protected override void UpdateEditTemplate() {
			Edit.SetEditTemplate(Column.AutoFilterRowEditTemplate);
		}
		BaseEditSettings oldEditSettings;
		protected override IBaseEdit CreateEditor(BaseEditSettings settings) {
			if(oldEditSettings != settings) {
				if(oldEditSettings != null)
					EditSettingsChangedEventHandler.Unsubscribe(oldEditSettings);
				EditSettingsChangedEventHandler.Subscribe(settings);
				oldEditSettings = settings;
			}
			IBaseEdit editor = CreateEditorCore(settings);
			BaseEdit be = editor as BaseEdit;
			if(be != null) {
				be.EditValuePostMode = PostMode.Delayed;
				be.SetBinding(BaseEdit.EditValuePostDelayProperty, new System.Windows.Data.Binding("FilterRowDelay") { Source = View });
			}
			return editor;
		}
		IBaseEdit CreateEditorCore(BaseEditSettings settings) {
			if(Column.ColumnFilterMode == ColumnFilterMode.DisplayText && !(settings is CheckEditSettings)) {
				TextEdit textEdit = new TextEdit() { VerticalContentAlignment = settings.VerticalContentAlignment };
				BaseEditHelper.AssignViewInfoProperties(textEdit, settings, Column);
				return textEdit;
			}
			IBaseEdit editor = settings.CreateEditor(false, Column, EditorOptimizationMode.Disabled);
			BaseEditHelper.UpdateHighlightingText(editor, null);
			if(editor is ButtonEdit)
				((ButtonEdit)editor).DefaultButtonClick += (d, e) => BaseEditHelper.RaiseDefaultButtonClick((ButtonEditSettings)settings);
			ApplyEditSettings(editor, settings, false);
			return editor;
		}
		void EditSettingsChanged(object sender, EventArgs e) {
			ApplyEditSettings(editCore, Column.ActualEditSettings, true);
		}
		void ApplyEditSettings(IBaseEdit editor, BaseEditSettings settings, bool applySettings) {
			if(Column.ColumnFilterMode == ColumnFilterMode.DisplayText && !(settings is CheckEditSettings)) return;
			if(applySettings) {
				TextEditSettings textEditSettings = settings as TextEditSettings;
				if(textEditSettings == null || String.IsNullOrEmpty(textEditSettings.HighlightedText))
					settings.ApplyToEdit(editor, false, Column);
			}
			CheckEdit checkEdit = editor as CheckEdit;
			if(checkEdit != null) {
				checkEdit.IsThreeState = true;
				checkEdit.IsChecked = null;
				return;
			}
			ComboBoxEdit comboBoxEdit = editor as ComboBoxEdit;
			if(comboBoxEdit != null) {
				comboBoxEdit.ItemsSource = new FilterComboBoxItemsList(comboBoxEdit.ItemsSource != null ? comboBoxEdit.ItemsSource : comboBoxEdit.Items);
				return;
			}
			SpinEdit spinEdit = editor as SpinEdit;
			if(spinEdit != null) {
				spinEdit.MinValue = null;
				spinEdit.MaxValue = null;
				return;
			}
			DateEdit dateEdit = editor as DateEdit;
			if(dateEdit != null) {
				dateEdit.MinValue = null;
				dateEdit.MaxValue = null;
				return;
			}
		}
		class FilterComboBoxItemsList : List<object>, INotifyCollectionChanged, IWeakEventListener {
			CustomComboBoxItem emptyItem;
			IEnumerable source;
			public FilterComboBoxItemsList(object items) {
				emptyItem = new CustomComboBoxItem() { DisplayValue = string.Empty, EditValue = string.Empty };
#if SL
				emptyItem.DisplayValue = " ";
#endif          
				source = DataBindingHelper.ExtractDataSource(items);
				LoadItems();
				if(source is IBindingList)
					ListChangedEventManager.AddListener(source as IBindingList, this);
			}
			void LoadItems() {
				Clear();
				foreach(object item in source)
					Add(item);
				if(Count != 0)
					Insert(0, emptyItem);
			}
			void OnChanged() {
				LoadItems();
				if(collectionChanged != null)
					collectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
			NotifyCollectionChangedEventHandler collectionChanged;
			event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged {
				add { collectionChanged += value; }
				remove { collectionChanged -= value; }
			}
			bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
				if(managerType == typeof(ListChangedEventManager)) {
					OnChanged();
					return true;
				}
				return false;
			}
		}
		protected override void UpdateEditValueCore(IBaseEdit editor) {
			if(!IsEditorVisible)
				editor.EditValue = Column.AutoFilterValue;
		}
		protected override bool PostEditorCore() {
			Edit.FlushPendingEditActions();
			Column.SetAutoFilterValue(Edit.EditValue);
			return true;
		}
		protected override void OnEditValueChanged() {
			if(Column.ImmediateUpdateAutoFilter)
				Column.SetAutoFilterValue(Edit.EditValue);
		}
		protected override void OnHiddenEditor(bool closeEditor) {
			Edit.EditValue = Column.AutoFilterValue;
			base.OnHiddenEditor(closeEditor);
		}
		protected override void OnColumnContentChanged(object sender, ColumnContentChangedEventArgs e) {
			if(e.Property == ColumnBase.AutoFilterValueProperty && !object.Equals(Edit.EditValue, Column.AutoFilterValue)) {
				Edit.EditValue = Column.AutoFilterValue;
				return;
			}
			if(e.Property == ColumnBase.ColumnFilterModeProperty) {
				UpdateContent();
				return;
			}
			if(e.Property == ColumnBase.AutoFilterRowDisplayTemplateProperty) {
				UpdateDisplayTemplate(true);
				return;
			}
			base.OnColumnContentChanged(sender, e);
		}
		protected override void SetDisplayTextProvider(IBaseEdit newEdit) {
		}
		protected override void UpdateEditorDataContextValue(object newValue) {
			if(Column != null && Column.ImmediateUpdateAutoFilter)
				Column.AutoFilterValue = newValue;
		}
	}
}
