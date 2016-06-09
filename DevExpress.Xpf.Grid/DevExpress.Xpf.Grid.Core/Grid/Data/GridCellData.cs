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
using System.Windows.Data;
using DevExpress.Data.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Utils;
using DevExpress.XtraEditors.DXErrorProvider;
namespace DevExpress.Xpf.Grid {
	public class GridCellData : GridColumnData {
		public static readonly DependencyProperty DisplayMemberBindingValueProperty;
#if DEBUGTEST
		internal static int GlobalInstanceCount;
#endif
		static GridCellData() {
			DisplayMemberBindingValueProperty = DependencyPropertyManager.Register("DisplayMemberBindingValue", typeof(object), typeof(GridCellData), new UIPropertyMetadata(null, new PropertyChangedCallback((d, e) => ((GridCellData)d).OnDisplayMemberBindingValueChanged())));
		}
		bool isSelected;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridCellDataIsSelected")]
#endif
		public bool IsSelected {
			get { return isSelected; }
			private set {
				if(isSelected != value) {
					isSelected = value;
					UpdateSelectionState();
					RaisePropertyChanged("IsSelected");
				}
			}
		}
		bool isFocusedCell;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridCellDataIsFocusedCell")]
#endif
		public bool IsFocusedCell {
			get { return isFocusedCell; }
			private set {
				if(isFocusedCell != value) {
					isFocusedCell = value;
					OnIsFocusedCellChanged();
					RaisePropertyChanged("IsFocusedCell");
				}
			}
		}
		SelectionState selectionState = SelectionState.None;
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridCellDataSelectionState")]
#endif
		public SelectionState SelectionState {
			get { return selectionState; }
			private set {
				if(selectionState != value) {
					selectionState = value;
					OnSelectionStateChanged();
					RaisePropertyChanged("SelectionState");
				}
			}
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridCellDataRowData")]
#endif
		public RowData RowData { get { return (RowData)RowDataBase; } }
		internal DataControlBase DataControl { get { return View.DataControl; } }
		bool isValueBinded;
		public GridCellData(RowData rowData)
			: base(rowData) {
#if DEBUGTEST
			GlobalInstanceCount++;
#endif
			UpdateLanguage();
		}
		internal virtual bool IsEditing { get { return false; } }
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridCellDataDisplayMemberBindingValue")]
#endif
		public object DisplayMemberBindingValue {
			get { return (object)GetValue(DisplayMemberBindingValueProperty); }
			set { SetValue(DisplayMemberBindingValueProperty, value); }
		}
		protected override void OnEditorChanged() {
			base.OnEditorChanged();
			if(SelectionState != Grid.SelectionState.None)
				UpdateEditorSelectionState();
			if(IsFocusedCell)
				UpdateEditorIsFocusedCell();
		}
		void OnSelectionStateChanged() {
			SyncCellContentPresenterProperties();
			UpdateEditorSelectionState();
		}
		void UpdateEditorSelectionState() {
			if(editor != null)
				editor.GridCellEditorOwner.SetSelectionState(SelectionState);
		}
		void UpdateEditorIsFocusedCell() {
			if(editor != null)
				editor.GridCellEditorOwner.SetIsFocusedCell(IsFocusedCell);
		}
		void OnDisplayMemberBindingValueChanged() {
			if(!IsEditing) UpdateGridCellDataValue();
		}
		internal void UpdateEditorDisplayText() {
			if(this.editor == null) return;
			this.editor.UpdateDisplayText();
		}
		internal void UpdatePrintingMergeValue() {
			if(this.editor == null) return;
			editor.UpdatePrintingMergeValue();
		}
		internal void UpdateGridCellDataValue() {
			Value = DisplayMemberBindingValue;
		}
		protected override void OnValueChanged(object oldValue) {
			base.OnValueChanged(oldValue);
			OnEditorDataChanged();
		}
		Locker editorDataChangeLocker = new Locker();
		void OnEditorDataChanged() {
			if(editor != null)
				editorDataChangeLocker.DoIfNotLocked(() => editor.OnDataChanged());
		}
		internal BindingBase displayMemberBinding;
#if DEBUGTEST
		internal int UpdateValueCount;
		internal int SimplePropertyChangedCount;
		internal static int UpdateLanguageCount;
#endif
		protected internal override void UpdateValue() {
			if(!RowData.GetIsReady())
				return;
#if DEBUGTEST
			UpdateValueCount++;
#endif
			editorDataChangeLocker.Lock();
			ResubscribeSimpleBinding();
			BindingBase binding = ColumnCore == null ? null : ColumnCore.ActualBinding;
			if(isValueBinded) {
				if(binding != displayMemberBinding)
					ClearValue(DisplayMemberBindingValueProperty);
			}
			if(!IsEditing && ColumnCore != null && Data != null && DataControl != null && ColumnCore.OwnerControl != null) {
				isValueBinded = binding != null;
				if(isValueBinded) {
					if(binding != displayMemberBinding) {
						displayMemberBinding = binding;
						if(!View.IsDesignTime)
							BindingOperations.SetBinding(this, DisplayMemberBindingValueProperty, binding);
						if(Value != DisplayMemberBindingValue)
							UpdateGridCellDataValue();
					}
				}
				else {
					displayMemberBinding = null;
					object newValue = GetValue();
					if(ColumnCore.ActualBinding != null) {
						UpdateValue();
						return;
					}
					if(DevExpress.Data.AsyncServerModeDataController.IsNoValue(newValue)) {
						return;
					}
					if(!object.Equals(Value, newValue)) 
						Value = newValue;
					if(ColumnCore.IsSimpleBindingEnabled)
						DisplayMemberBindingValue = Value;
				}
			}
			UpdateEditorHighlightingText(false);
			editorDataChangeLocker.Unlock();
			OnEditorDataChanged();
		}
		protected internal override void ClearBindingValue() {
			ClearValue(DisplayMemberBindingValueProperty);
			displayMemberBinding = null;
		}
		protected virtual object GetValue() {
			return RowData.treeBuilder.GetCellValue(RowData, Column.FieldName);
		}
		internal void SetDataErrorText(int rowHandle, ErrorInfo info) {
			if(info == null) {
				SetDataErrorTextCore(rowHandle, null);
				return;
			}
			SetDataErrorText(rowHandle, info.ErrorText, info.ErrorType, info is CustomErrorInfo);
		}
		internal void SetDataErrorText(int rowHandle, string errorText, ErrorType errorType, bool customErrorType = false) {
			BaseValidationError newError = string.IsNullOrEmpty(errorText) && !customErrorType ? null : View.CreateCellValidationError(errorText, errorType, rowHandle, Column);
			SetDataErrorTextCore(rowHandle, newError);
		}
		void SetDataErrorTextCore(int rowHandle, BaseValidationError newError) {
			BaseValidationError oldError = BaseEdit.GetValidationError(this);
			if(!object.Equals(newError, oldError) && !HasCellEditorError) {
				BaseEditHelper.SetValidationError(this, newError);
				RaiseContentChanged();
			}
		}
		bool HasCellEditorError { get { return View.HasCellEditorError && View.ValidationError == BaseEdit.GetValidationError(this); } }
		internal void SyncCellContentPresenterProperties() {
			if(editor != null)
				editor.SyncProperties();
		}
		internal void OnRowChanging(object oldRow, object newRow) {
			if(!RowData.GetIsReady(newRow))
				ClearBindingValue();
			UnsubscribeSimpleBinding();
		}
		internal void UpdateIsReady() {
			if(editor == null) return;
			editor.UpdateIsReady();
		}
		internal void OnViewChanged() {
			if(editor == null) return;
			editor.OnViewChanged();
		}
		internal virtual void UpdateEditorButtonVisibility() {
		}
		internal void UpdateEditorHighlightingText(bool columnChanged) {
			if(View == null || editor == null || Column == null)
				return;
			editor.UpdateHighlightingText(View.GetTextHighlightingProperties(Column), columnChanged);
		}
		protected internal void UpdateLanguage() {
			if(View == null || View.DataControl == null)
				return;
#if DEBUGTEST
			UpdateLanguageCount++;
#endif
			SetValue(FrameworkElement.LanguageProperty, View.DataControl.GetValue(FrameworkElement.LanguageProperty));
			if(ColumnCore != null && ColumnCore.IsSimpleBindingEnabled)
				UpdateValue();
		}
		internal void UpdateIsSelected(int rowHandle) {
			UpdateIsSelected(rowHandle, View.ViewBehavior.GetIsCellSelected(rowHandle, Column));
		}
#if DEBUGTEST
		public static int UpdateIsSelectedCountDebugTest { get; private set; }
#endif
		internal void UpdateIsSelected(int rowHandle, bool forceIsSelected) {
#if DEBUGTEST
			UpdateIsSelectedCountDebugTest++;
#endif
			IsSelected = forceIsSelected;
		}
		void UpdateIsFocusedCellCore(int rowHandle) {
			IsFocusedCell = View.GetIsCellFocused(rowHandle, Column) && RowData.GetIsFocusable();
		}
		internal void UpdateIsFocusedCell(int rowHandle) {
			UpdateIsFocusedCellCore(rowHandle);
		}
		internal void OnIsFocusedCellChanged() {
			UpdateSelectionState();
			UpdateEditorIsFocusedCell();
		}
		internal void UpdateSelectionState() {
			SelectionState = View.GetCellSelectionState(RowData.RowHandle.Value, IsFocusedCell, IsSelected);
			UpdateCellState();
		}
		internal void UpdateFullState(int rowHandle) {
			UpdateIsSelected(rowHandle);
			UpdateIsFocusedCellCore(rowHandle);
			UpdateSelectionState();
		}
		void UpdateCellState() {
			if(editor == null) return;
			editor.UpdateCellState();
		}
		protected override bool CanRaiseContentChangedWhenDataChanged() {
			return RowData.RowHandle.Value != DataControlBase.AutoFilterRowHandle;
		}
		internal double GetActualCellWidth() {
			return double.IsInfinity(Column.ActualDataWidth) ? 0 : Math.Max(0, Column.ActualDataWidth + RowData.GetRowIndent(Column));
		}
		internal void SyncLeftMargin(FrameworkElement cell) {
			double left = RowData.GetRowLeftMargin(this);
			if(cell.Margin.Left == left)
				return;
			cell.Margin = new Thickness(left, 0.0, 0.0, 0.0);
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property == Validation.ErrorsProperty) {
				UpdateCellError(RowData.RowHandle, Column);
			}
		}
		protected internal void UpdateCellError(RowHandle handle, ColumnBase column, bool customValidate = true) {
			if(CanShowCellError) {
				SetCellError(handle, column, customValidate);
				return;
			}
			ClearCellError(handle);
		}
		bool CanShowCellError {
			get { return (View.ItemsSourceErrorInfoShowMode & ItemsSourceErrorInfoShowMode.Cell) != ItemsSourceErrorInfoShowMode.None; }
		}
		void SetCellError(RowHandle handle, ColumnBase column, bool customValidate) {
			ErrorInfo info = null;
			if(View.AllowLeaveInvalidEditor && customValidate) {
				info = CustomSetValidationError(handle, column);
				if(info != null) {
					SetDataErrorText(handle.Value, info);
					return;
				}
			}
			info = View.DataProviderBase.GetErrorInfo(handle, column.FieldName);
			var errors = Validation.GetErrors(this);
			if(errors.Count > 0) {
				info.ErrorText = errors[0].ErrorContent as string;
			}
			SetDataErrorText(handle.Value, info);
		}
		void ClearCellError(RowHandle handle) {
			SetDataErrorText(handle.Value, null);
		}
		ErrorInfo CustomSetValidationError(RowHandle handle, ColumnBase column) {			
			ValidationResult validationResult;
			GridRowValidationEventArgs eventArgs = View.CreateCellValidationEventArgs(this, IsValueDirty ? GetValue() : Value, handle.Value, column);		 
			View.OnValidation(column, eventArgs);
			validationResult = new ValidationResult(eventArgs.IsValid, eventArgs.ErrorContent);
			if(validationResult.IsValid) {
				return null;		 
			} else
				return new CustomErrorInfo(validationResult.ErrorContent != null ? validationResult.ErrorContent.ToString() : null, eventArgs.ErrorType == ErrorType.None ? ErrorType.Default : eventArgs.ErrorType);
		}
		class CustomErrorInfo : ErrorInfo {
			public CustomErrorInfo() : base() { }
			public CustomErrorInfo(string errorText, ErrorType errorType) : base(errorText, errorType) { }
		}
		WeakEventHandler<GridCellData, EventArgs, EventHandler> simpleBindingHandler;
		WeakEventHandler<GridCellData, EventArgs, EventHandler> SimpleBindingHandler {
			get {
				if(simpleBindingHandler == null)
					simpleBindingHandler = new WeakEventHandler<GridCellData, EventArgs, EventHandler>(this,
						(cell, _, __) => cell.OnSimplePropertyChanged(),
						(_, owner) => UnsubscribeSimpleBinding(),
						(wh) => wh.OnEvent);
				return simpleBindingHandler;
			}
		}
		PropertyDescriptor descriptorToListen;
		void ResubscribeSimpleBinding() {
			if(ColumnCore == null || !ColumnCore.IsSimpleBindingEnabled) {
				UnsubscribeSimpleBinding();
				return;
			}
			PropertyDescriptor desctiptor = Column.SimpleBindingProcessor.DescriptorToListen;
			object row = RowData != null ? RowData.Row : null;
			if(descriptorToListen != desctiptor && desctiptor != null && row != null) {
				UnsubscribeSimpleBinding();
				desctiptor.AddValueChanged(row, SimpleBindingHandler.Handler);
				descriptorToListen = desctiptor;
			}
		}
		void UnsubscribeSimpleBinding() {
			if(descriptorToListen != null) {
				object row = RowData != null ? RowData.Row : null;
				if(row != null) {
					descriptorToListen.RemoveValueChanged(row, SimpleBindingHandler.Handler);
					descriptorToListen = null;
				}
			}
		}
		void OnSimplePropertyChanged() {
#if DEBUGTEST
			SimplePropertyChangedCount++;
#endif
			DisplayMemberBindingValue = GetValue();
		}
	}
}
