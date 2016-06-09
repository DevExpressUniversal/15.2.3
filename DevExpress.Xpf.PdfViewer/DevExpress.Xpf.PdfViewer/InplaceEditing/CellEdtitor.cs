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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Validation;
namespace DevExpress.Xpf.PdfViewer {
	public class CellEditorEditingProvider : IInplaceEditingProvider {
		public bool HandleTextNavigation(Key key, ModifierKeys keys) {
			return true;
		}
		public bool HandleScrollNavigation(Key key, ModifierKeys keys) {
			return true;
		}
	}
	public class CellEditor : InplaceEditorBase {
		readonly CellEditorOwner owner;
		readonly CellEditorColumn column;
		public CellData CellData { get; private set; }
		readonly Locker updateValueLocker = new Locker();
		BaseValidationError ValidationError { get; set; }
		readonly IPdfViewerValueEditingCallBack dataController;
		protected override InplaceEditorOwnerBase Owner { get { return owner; } }
		protected override IInplaceEditorColumn EditorColumn { get { return column; } }
		readonly CellEditorEditingProvider editingProvider = new CellEditorEditingProvider();
		System.Windows.Controls.Grid Grid { get; set; }
		Border Border { get; set; }
		protected override bool IsCellFocused {
			get { return true; }
		}
		protected override bool IsReadOnly {
			get { return column.IsReadOnly; }
		}
		protected override bool OverrideCellTemplate {
			get { return true; }
		}
		public CellEditor(CellEditorOwner owner, IPdfViewerValueEditingCallBack dataController, CellEditorColumn column) {
			this.owner = owner;
			this.column = column;
			this.dataController = dataController;
			CellData = new CellData() { Value = column.InitialValue };
			Grid = new System.Windows.Controls.Grid();
			Border = new Border();
			Grid.Children.Add(Border);
			Grid.Children.Add(this);
			owner.VisualHost = Grid;
			OnOwnerChanged(null);
		}
		protected override bool IsInactiveEditorButtonVisible() {
			return false;
		}
		protected override object GetEditableValue() {
			return CellData.Value;
		}
		public override void ValidateEditorCore() {
			base.ValidateEditorCore();
			if (editCore != null && Edit != null && !Edit.DoValidate()) {
				var error = BaseEditHelper.GetValidationError((DependencyObject)editCore);
				ValidationError = error;
			}
		}
		protected override bool PostEditorCore() {
			if (HasAccessToCellValue) {
				if (ValidationError != null)
					return false;
				object convertedValue = ConvertToEditableValue(EditableValue);
				PdfValidationError validationError = dataController.ValidateEditor(convertedValue);
				if (validationError != null) {
					ValidationError = new BaseValidationError(validationError.Message);
					BaseEditHelper.SetValidationError((DependencyObject)editCore, ValidationError);
					return false;
				}
				dataController.PostEditor(convertedValue);
			}
			return true;
		}
		object ConvertToEditableValue(object editableValue) {
			if (column.EditorType == PdfEditorType.ListBox) {
				return new PdfListBoxEditValue(GetTopIndexFromListBox(editCore), (editableValue as IList).Return(x => x.Cast<string>().ToList(), () => new List<string> { (string)editableValue}));
			}
			return editableValue;
		}
		int GetTopIndexFromListBox(IBaseEdit baseEdit) {
			ListBox lb = LayoutHelper.FindElement((FrameworkElement)baseEdit, x => x is ListBox) as ListBox;
			VirtualizingStackPanel sp = lb.With(x => (VirtualizingStackPanel)LayoutHelper.FindElement(x, fe => fe is VirtualizingStackPanel));
			PdfOptionsFormFieldOption item = sp.With(x => x.Children.Cast<FrameworkElement>().Where(IsItemVisible).Select(element => (element.DataContext as PdfOptionsFormFieldOption)).FirstOrDefault());
			if (item == null)
				return -1;
			var list = lb.ItemsSource.Cast<PdfOptionsFormFieldOption>().ToList();
			int index = list.IndexOf(item);
			return index;
		}
		bool IsItemVisible(FrameworkElement element) {
			Panel panel = LayoutHelper.FindLayoutOrVisualParentObject<Panel>(element, false);
			Rect r = LayoutHelper.GetRelativeElementRect(element, panel);
			Rect panelRect = LayoutHelper.GetRelativeElementRect(panel, panel);
			r.Union(panelRect);
			return r.Height.AreClose(panelRect.Height);
		}
		protected override void OnHiddenEditor(bool closeEditor) {
			base.OnHiddenEditor(closeEditor);
			dataController.HideEditor();
		}
		protected override void UpdateEditValueCore(IBaseEdit editor) {
			updateValueLocker.DoLockedAction(() => editor.EditValue = GetEditableValue());
		}
		protected override EditableDataObject GetEditorDataContext() {
			return CellData;
		}
		protected override bool ProcessKeyForLookUp(KeyEventArgs e) {
			return base.ProcessKeyForLookUp(e);
		}
		protected override IBaseEdit CreateEditor(BaseEditSettings settings) {
			var baseEdit = base.CreateEditor(settings);
			InitializeBorder();
			InitializeComboBoxEdit(baseEdit as ComboBoxEdit);
			InitializeListBoxEdit(baseEdit as ListBoxEdit);
			InitializeIBaseEdit(baseEdit);
			InitializeBaseEdit((BaseEdit)baseEdit);
			return baseEdit;
		}
		void InitializeListBoxEdit(ListBoxEdit lb) {
			if (lb == null)
				return;
		}
		void InitializeComboBoxEdit(ComboBoxEdit cb) {
			if (cb== null)
				return;
			cb.PopupOpened += cb_PopupOpened;
		}
		void cb_PopupOpened(object sender, RoutedEventArgs e) {
			var cb = (ComboBoxEdit)sender;
			var pbc = (PopupBorderControl)cb.GetPopup().Child;
			var elb = (EditorListBox)LayoutHelper.FindElementByType(pbc, typeof(PopupListBox));
			elb.Do(x => x.Background = column.Background);
			elb.Do(x => x.Foreground = column.Foreground);
		}
		void InitializeIBaseEdit(IBaseEdit baseEdit) {
			baseEdit.SetInplaceEditingProvider(editingProvider);
		}
		void InitializeBaseEdit(BaseEdit baseEdit) {
			baseEdit.Margin = column.BorderThickness;
			baseEdit.Foreground = column.Foreground;
			if (column.FontSize > 1)
				baseEdit.FontSize = column.FontSize;
			if (column.FontFamily != null)
				baseEdit.FontFamily = column.FontFamily;
		}
		void InitializeBorder() {
			Border.Background = column.Background;
			Border.BorderBrush = column.BorderBrush;
			Border.BorderThickness = column.BorderThickness;
			Border.CornerRadius = column.CornerRadius;
		}
	}
}
