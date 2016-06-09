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

using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Utils;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.PropertyGrid.Internal;
using System.Windows.Input;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.XtraEditors.DXErrorProvider;
namespace DevExpress.Xpf.PropertyGrid {
	public class CellEditor : InplaceEditorBase {
		public static readonly DependencyProperty IsEditorFocusedProperty;
		public static readonly DependencyProperty RowDataProperty;
		public static readonly DependencyProperty OwnerViewProperty;
		public static readonly DependencyProperty RowControlProperty;
		public static readonly DependencyProperty DataValidationErrorProperty;
		public BaseValidationError DataValidationError {
			get { return (BaseValidationError)GetValue(DataValidationErrorProperty); }
			set { SetValue(DataValidationErrorProperty, value); }
		}
		public DataTemplate CustomCellTemplate {
			get { return (DataTemplate)GetValue(CustomCellTemplateProperty); }
			set { SetValue(CustomCellTemplateProperty, value); }
		}
		public DataTemplateSelector CustomCellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(CustomCellTemplateSelectorProperty); }
			set { SetValue(CustomCellTemplateSelectorProperty, value); }
		}
		public bool IsCellTemplateOverriden {
			get { return (bool)GetValue(IsCellTemplateOverridenProperty); }
			protected internal set { SetValue(IsCellTemplateOverridenPropertyKey, value); }
		}
		protected virtual object UpdateIsCellTemplateOverriden(object e) {
			return CustomCellTemplate != null || CustomCellTemplateSelector != null;
		}
		protected virtual void OnCustomCellTemplateSelectorChanged(DataTemplateSelector oldValue, DataTemplateSelector newValue) {
			CoerceValue(IsCellTemplateOverridenProperty);
		}
		protected virtual void OnCustomCellTemplateChanged(DataTemplate oldValue, DataTemplate newValue) {
			CoerceValue(IsCellTemplateOverridenProperty);
		}
		public static readonly DependencyProperty CustomCellTemplateProperty = DependencyPropertyManager.Register("CustomCellTemplate", typeof(DataTemplate), typeof(CellEditor), new FrameworkPropertyMetadata(null, (d, e) => ((CellEditor)d).OnCustomCellTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue)));
		public static readonly DependencyProperty CustomCellTemplateSelectorProperty = DependencyPropertyManager.Register("CustomCellTemplateSelector", typeof(DataTemplateSelector), typeof(CellEditor), new FrameworkPropertyMetadata(null, (d, e) => ((CellEditor)d).OnCustomCellTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue)));
		protected static readonly DependencyPropertyKey IsCellTemplateOverridenPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsCellTemplateOverriden", typeof(bool), typeof(CellEditor), new FrameworkPropertyMetadata(false, (d, e) => ((CellEditor)d).OnIsCellTemplateOverridenChanged((bool)e.OldValue, (bool)e.NewValue), (d, e) => ((CellEditor)d).UpdateIsCellTemplateOverriden(e)));
		public static readonly DependencyProperty IsCellTemplateOverridenProperty = IsCellTemplateOverridenPropertyKey.DependencyProperty;
		protected virtual void OnIsCellTemplateOverridenChanged(bool oldValue, bool newValue) {
			EditorSourceType = newValue ? BaseEditSourceType.CellTemplate : BaseEditSourceType.EditSettings;
			UpdateData();
			UpdateContent();
		}
		static CellEditor() {
			Type ownerType = typeof(CellEditor);
			FocusableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));
			IsEditorFocusedProperty = DependencyPropertyManager.Register("IsEditorFocused", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((CellEditor)d).OnIsEditorFocusedChanged((bool)e.NewValue)));
			RowDataProperty = DependencyPropertyManager.Register("RowData", typeof(RowDataBase), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((CellEditor)d).OnRowDataChanged((RowDataBase)e.OldValue)));
			OwnerViewProperty = DependencyPropertyManager.Register("OwnerView", typeof(PropertyGridView), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((CellEditor)d).OnOwnerViewChanged((PropertyGridView)e.OldValue)));
			RowControlProperty = DependencyPropertyManager.Register("RowControl", typeof(RowControlBase), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((CellEditor)d).OnRowControlChanged((RowControlBase)e.OldValue)));
			DataValidationErrorProperty = DependencyPropertyManager.Register("DataValidationError", typeof(BaseValidationError), typeof(CellEditor), new FrameworkPropertyMetadata(null, (d, e) => ((CellEditor)d).OnDataValidationErrorChanged((BaseValidationError)e.OldValue)));
		}
		public RowDataBase RowData {
			get { return (RowDataBase)GetValue(RowDataProperty); }
			set { SetValue(RowDataProperty, value); }
		}
		public bool IsEditorFocused {
			get { return (bool)GetValue(IsEditorFocusedProperty); }
			set { SetValue(IsEditorFocusedProperty, value); }
		}
		public PropertyGridView OwnerView {
			get { return (PropertyGridView)GetValue(OwnerViewProperty); }
			set { SetValue(OwnerViewProperty, value); }
		}
		public PropertyGridControl PropertyGrid {
			get { return OwnerView == null ? null : OwnerView.PropertyGrid; }
		}
		public RowControlBase RowControl {
			get { return (RowControlBase)GetValue(RowControlProperty); }
			set { SetValue(RowControlProperty, value); }
		}
		protected override IInplaceEditorColumn EditorColumn {
			get { return RowData.With(x => x.Column); }
		}
		protected override bool IsCellFocused {
			get { return IsEditorFocused; }
		}
		protected override bool IsReadOnly {
			get { return RowData.IsReadOnly; }
		}
		protected override bool OverrideCellTemplate {
			get { return false; }
		}
		protected override InplaceEditorOwnerBase Owner {
			get { return OwnerView == null ? null : OwnerView.CellEditorOwner; }
		}
		protected virtual void OnIsEditorFocusedChanged(bool newValue) {
			OnIsFocusedCellChanged();
		}
		protected override void OnEditorActivated(object sender, RoutedEventArgs e) {
			base.OnEditorActivated(sender, e);
			var pGrid = PropertyGridHelper.GetPropertyGrid(this);
			if (pGrid != null && RowData != null) {
				pGrid.RaiseShownEditorEvent(RowData.Handle, editCore);
			}
		}
		protected override sealed bool RaiseShowingEditor() {
			var pGrid = PropertyGridHelper.GetPropertyGrid(this);
			if (pGrid == null)
				return false;
			return pGrid.CanShowEditor(this);			
		}
		protected virtual void OnRowControlChanged(RowControlBase oldValue) {
		}
		protected override void OnEditValueChanged(object sender, EditValueChangedEventArgs e) {
			base.OnEditValueChanged(sender, e);
			var definition = (PropertyDefinition)RowData.Definition;
			if (isUpdating || RowData == null || RowData.Definition as PropertyDefinition == null)
				return;
			BaseEditSettings settings = EditorColumn.With(x => x.EditSettings);
			if (definition.PostOnEditValueChanged || PropertyGridEditSettingsHelper.GetPostOnEditValueChanged(settings)) {
				if (OwnerView != null)
					OwnerView.ImmediateActionsManager.EnqueueAction(PostEditorAction);
				else
					PostEditorAction();
				return;
			}
			if (PropertyGridEditSettingsHelper.GetPostOnPopupClosed(settings) && PopupBaseEditHelper.GetIsValueChangedViaPopup(settings))
				PostEditorAction();
		}
		void PostEditorAction() {
			if (IsEditorVisible)
				PostEditor();
		}
		protected virtual void OnRowDataChanged(RowDataBase oldValue) {
			if (RowData == null) {
				SetEdit(null); 
				ClearValue(CustomCellTemplateSelectorProperty);
				ClearValue(CustomCellTemplateProperty);
				ClearValue(DataValidationErrorProperty);
			}
			else {
				SetBinding(CustomCellTemplateProperty, new Binding("Definition.CellTemplate") { Source = RowData });
				SetBinding(CustomCellTemplateSelectorProperty, new Binding("Definition.CellTemplateSelector") { Source = RowData });
				SetBinding(DataValidationErrorProperty, new Binding("ValidationError") { Source = RowData });
			}
			OnOwnerChanged(oldValue.With(x => x.Column));
			if (editCore != null) {
				UpdateEditValue(editCore);
			}
		}
		protected override bool IsProperEditorSettings() {
			return EditorSettingsProvider.Default.IsCompatible(editCore, EditorColumn.EditSettings);
		}
		protected override DataTemplate SelectTemplate() {
			return (EditorColumn == null || EditorColumn.EditorTemplateSelector == null) ? null : EditorColumn.EditorTemplateSelector.SelectTemplate(RowData, this);
		}
		protected virtual void OnOwnerViewChanged(PropertyGridView oldValue) {
			OnOwnerChanged(RowData.With(x => x.Column));
		}
		public override object GetEditableValueForExternalEditor() {
			return RowData.With(x => x.Value);
		}
		public override void SetEditableValueFromExternalEditor(object value) {
			EditableValue = value;
			CommitEditor(true);
			OwnerView.EnqueueShowEditor(RowData.FullPath);
		}
		protected override object GetEditableValue() {
			if (RowData != null) {
				if (RowData.ShouldConvertValue && RowData.Value != null)
					return RowData.RowDataGenerator.DataView.ConvertToString(RowData.Handle, RowData.Value);
				return RowData.Value;
			}
			return null;
		}
		protected override EditableDataObject GetEditorDataContext() {
			return RowData;
		}
		protected override bool IsInactiveEditorButtonVisible() {
			if (RowData == null)
				return false;
			return editCore != null && PropertyGridEditSettingsHelper.GetIsStandardEditSettings(RowData.EditSettings) && RowData.If(x => x.Handle != null).With(x => x.RowDataGenerator).With(x => x.DataView).If(x => x.IsNewInstanceInitializer(RowData.Handle)).ReturnSuccess();
		}
		protected virtual bool PostEditorCore(object editableValue) {
			if (HasAccessToCellValue && RowData != null && RowData.Handle != null) {
				var oldValue = RowData.Value;
				var newValue = editableValue;
				if (PropertyGrid != null && !PropertyGrid.RaiseCellValueChanging(RowData, oldValue, newValue))
					return true;
				var validationError = OwnerView.GetValidationError(RowData.Handle);
				if (validationError != null) {
					RowData.ValidationError = validationError;
					return false;
				}
				var view = OwnerView.PropertyGrid.DataView;
				var controller = OwnerView.PropertyGrid.DataController;
				RowData.ValidationError = null;
				Exception exception = PropertyGrid == null ? null : PropertyGrid.RaiseValidateCell(RowData, oldValue, newValue);
				exception = exception ?? controller.SetValue(RowData.Handle, newValue);
				string message = exception == null ? null : exception.Message;
				if (PropertyGrid != null) {
					message = PropertyGrid.RaiseInvalidCellException(RowData, exception);
				}
				if (exception != null) {
					CellValidationError error = new CellValidationError(message, exception, ErrorType.Default);
					RowData.ValidationError = error;
					return false;
				}
				if (PropertyGrid != null) {
					Edit.IsValueChanged = false;
					view.Update();
					PropertyGrid.RaiseCellValueChanged(RowData, oldValue, controller.GetValue(view.GetHandleByFieldName(RowData.FullPath)));
				}
			}
			return true;
		}
		protected override bool PostEditorCore() {
			if (Edit.IsValueChanged)
				return PostEditorCore(EditableValue);
			return true;
		}
		protected override void InitializeBaseEdit(IBaseEdit newEdit, BaseEditSourceType newBaseEditSourceType) {
			base.InitializeBaseEdit(newEdit, newBaseEditSourceType);
			newEdit.InvalidValueBehavior = InvalidValueBehavior.WaitForValidValue;
		}
		protected override bool ShowEditorCore(bool selectAll) {
			return ShowEditorInternal(selectAll);
		}
		protected override void OnShowEditor() {
			base.OnShowEditor();
			if (!Equals(OwnerView.CellEditorOwner.CurrentCellEditor, this))
				OwnerView.CellEditorOwner.CurrentCellEditor.With(PropertyGridHelper.GetEditorPresenter).Do(x => x.IsSelected = false);
			PropertyGridHelper.GetEditorPresenter(this).Do(x => x.IsSelected = true);
			OwnerView.PropertyGrid.NavigationManager.OpenEditorOnSelection = false;
		}
		protected virtual void OnDataValidationErrorChanged(BaseValidationError oldValue) {
			UpdateEditValidationError();
		}
		private void UpdateEditValidationError() {
			Edit.SetValidationError(DataValidationError);
		}
		protected override void SetEdit(IBaseEdit value) {
			base.SetEdit(value);
			UpdateEditValidationError();
		}
		protected override void RestoreValidationError() {
			var error = GetValidationErrorFromDataController();
			RowData.ValidationError = error;
			OwnerView.SetValidationError(RowData.Handle, error);
		}
		BaseValidationError GetValidationErrorFromDataController() {
			var validationError = PropertyGrid.DataView.GetValidationError(RowData.Handle);
			return validationError != null && validationError.Any() ? new BaseValidationError(validationError.First()) : null;
		}
		protected override void OnHiddenEditor(bool closeEditor) {
			base.OnHiddenEditor(closeEditor);
			UpdateEditValue(editCore);
			var pGrid = PropertyGridHelper.GetPropertyGrid(this);
			pGrid.UpdateData();
			if (pGrid != null && RowData != null) {
				pGrid.RaiseHiddenEditorEvent(RowData.Handle, editCore);
			}
		}
		bool isUpdating = false;
		protected override void UpdateEditValueCore(IBaseEdit editor) {
			isUpdating = true;
			if (editor != null) {
				editor.IsValueChanged = false;
				editor.EditValue = GetEditableValue();
			}
			isUpdating = false;
		}
		public override bool CanShowEditor() {			
			return RaiseShowingEditor();
		}
		protected override bool ProcessKeyForLookUp(KeyEventArgs e) {
			return base.ProcessKeyForLookUp(e) || ShouldActivateEditor(e);
		}
		bool ShouldActivateEditor(KeyEventArgs e) {
			return !IsEditorVisible && Edit.IsActivatingKey(e) || Edit.NeedsKey(e) || (e.Key == Key.Enter || !IsEditorVisible && e.Key == Key.F2 || IsEditorVisible && e.Key == Key.Escape);
		}
		protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) {
			HitTestResult result = base.HitTestCore(hitTestParameters);
			return result ?? new PointHitTestResult(this, hitTestParameters.HitPoint);
		}
		protected override void OnInnerContentChangedCore() {
			if (!IsEditorVisible) {
				UpdateEditValue(editCore);
			}
		}
		public override void ValidateEditorCore() {
			base.ValidateEditorCore();
			if (editCore != null && Edit != null && !Edit.DoValidate()) {
				var error = BaseEditHelper.GetValidationError((DependencyObject)editCore);
				OwnerView.SetValidationError(RowData.With(x => x.Handle), error);
			}
			else {
				OwnerView.SetValidationError(RowData.With(x => x.Handle), null);
			}
		}
		public void SetValidationErrorInternal(BaseValidationError validationError) {
			Edit.SetValidationError(validationError);
		}
		protected override IBaseEdit CreateEditor(BaseEditSettings settings) {
			return settings.CreateEditor(EditorColumn, PropertyGrid != null && PropertyGrid.ActualUseOptimizedEditor ? EditorOptimizationMode.Simple : EditorOptimizationMode.Disabled);
		}
	}
	public class CellValidationError : BaseValidationError {
		public CellValidationError(object errorContent, Exception exception, ErrorType errorType)
			: base(errorContent, exception, errorType) {
		}
	}
	public class SelectableControl : Control {
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			set { SetValue(IsSelectedProperty, value); }
		}
		public static readonly DependencyProperty IsSelectedProperty =
			DependencyPropertyManager.Register("IsSelected", typeof(bool), typeof(SelectableControl), new FrameworkPropertyMetadata(false));
		static SelectableControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectableControl), new FrameworkPropertyMetadata(typeof(SelectableControl)));
		}
	}
}
