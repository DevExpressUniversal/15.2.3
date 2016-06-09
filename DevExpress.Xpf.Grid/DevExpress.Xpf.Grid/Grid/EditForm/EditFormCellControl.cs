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
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.EditForm;
using System.Linq;
namespace DevExpress.Xpf.Grid {
	public class EditFormEditor : ContentPresenter {
		public static readonly DependencyProperty EditFormCellDataProperty;
		public static readonly DependencyProperty FieldNameProperty;
		static EditFormEditor() {
			DependencyPropertyRegistrator<EditFormEditor>.New()
				.Register(d => d.EditFormCellData, out EditFormCellDataProperty, null, OnEditFormCellDataChanged)
				.Register(d => d.FieldName, out FieldNameProperty, null, OnFieldNameChanged);
			Type ownerType = typeof(EditFormEditor);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
		static void OnEditFormCellDataChanged(EditFormEditor sender, DependencyPropertyChangedEventArgs args) {
			sender.OnEditFormCellDataChanged(args.OldValue as EditFormCellData);
		}
		static void OnFieldNameChanged(EditFormEditor sender, DependencyPropertyChangedEventArgs args) {
			sender.InvalidateFieldData();
		}
		public EditFormCellData EditFormCellData {
			get { return (EditFormCellData)GetValue(EditFormCellDataProperty); }
			set { SetValue(EditFormCellDataProperty, value); }
		}
		public string FieldName {
			get { return (string)GetValue(FieldNameProperty); }
			set { SetValue(FieldNameProperty, value); }
		}
		IBaseEdit editorCore;
		internal IBaseEdit Editor {
			get { return editorCore; }
			private set {
				if(editorCore != value) {
					OnEditorChanged(editorCore, value);
					editorCore = value;
				}
			}
		}
		void OnEditFormCellDataChanged(EditFormCellData oldValue) {
			if(oldValue != null)
				oldValue.PropertyChanged -= OnDataPropertyChanged;
			if(EditFormCellData != null)
				EditFormCellData.PropertyChanged += OnDataPropertyChanged;
			OnDataChanged();
		}
		void OnDataPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == EditFormCellData.ValidationErrorPropertyName)
				ValidateEditor();
		}
		public EditFormEditor() {
			Loaded += OnLoaded;
			DataContextChanged += OnDataContextChanged;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			ProcessKey(e.Key);
		}
		protected void ProcessKey(Key key) {
			if(key == Key.Enter && Editor != null && Editor.IsEditorActive)
				MoveNext();
		}
		internal virtual void MoveNext() {
			MoveFocus(false);
		}
		internal virtual void MovePrev() {
			MoveFocus(true);
		}
		void MoveFocus(bool isReverse) {
			MoveFocusHelper.MoveFocus(this, isReverse);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			if(Editor != null && EditFormCellData != null && EditFormCellData.VisibleIndex == 0)
				Editor.Focus();
		}
		void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
			InvalidateFieldData();
		}
		void InvalidateFieldData() {
			if(string.IsNullOrEmpty(FieldName))
				return;
			var rowData = DataContext as EditFormRowData;
			if(rowData == null)
				return;
			EditFormCellData cellData = rowData.GetEditorData(FieldName);
			if(cellData != null)
				EditFormCellData = cellData;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if(ContentTemplate != null) {
				IBaseEdit templatedEditor = ContentTemplate.FindName("PART_Editor", this) as IBaseEdit;
				if(templatedEditor != null)
					Editor = templatedEditor;
			}
		}
		void OnDataChanged() {
			if(EditFormCellData == null)
				Reset();
			else if(EditFormCellData.EditorTemplate != null) {
				Content = EditFormCellData;
				ContentTemplate = EditFormCellData.EditorTemplate;
			} else {
				Reset();
				Editor = EditFormCellData.EditSettings.With(s => s.CreateEditor());
				Content = Editor;
			}
		}
		void Reset() {
			if(Content != null)
				Content = null;
			if(ContentTemplate != null)
				ContentTemplate = null;
		}
		void OnEditorChanged(IBaseEdit oldValue, IBaseEdit newValue) {
			if(oldValue != null)
				oldValue.EditValueChanged -= OnEditValueChanged;
			if(newValue != null) {
				newValue.EditValueChanged += OnEditValueChanged;
				newValue.EditValue = EditFormCellData.Value;
				if(EditFormCellData.ReadOnly)
					newValue.IsReadOnly = true;
				ValidateEditorCore(newValue);
			}
		}
		void ValidateEditor() {
			ValidateEditorCore(Editor);
		}
		void ValidateEditorCore(IBaseEdit editor) {
			if(editor != null && EditFormCellData != null)
				editor.ValidationError = EditFormCellData.ValidationError;
		}
		void OnEditValueChanged(object sender, EditValueChangedEventArgs e) {
			IBaseEdit editor = (IBaseEdit)sender;
			if(EditFormCellData != null) {
				bool isEditorValid = editor.DoValidate();
				EditFormCellData.HasInnerError = !isEditorValid;
				if(editor.EditValue != EditFormCellData.Value)
					EditFormCellData.Value = editor.EditValue;
			}
		}
	}
	public class EditFormCellTemplateSelector : DataTemplateSelector {
		public DataTemplate CaptionTemplate { get; set; }
		public DataTemplate EditorTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			var layoutItem = item as IEditFormLayoutItem;
			if(layoutItem == null)
				return null;
			EditFormLayoutItemType itemType = layoutItem.ItemType;
			if(layoutItem.ItemType == EditFormLayoutItemType.Caption)
				return CaptionTemplate;
			if(layoutItem.ItemType == EditFormLayoutItemType.Editor)
				return EditorTemplate;
			return null;
		}
	}
}
