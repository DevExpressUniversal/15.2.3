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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Editors.Native{
	public partial class TestInplaceContainer : UserControl {
		const int RowCount = 5;
		const int ColCount = 5;
		TestInplaceEditorOwner owner;
		ImmediateActionsManager immediateActionsManager;
		public TestInplaceContainer() {
			InitializeComponent();
			immediateActionsManager = new ImmediateActionsManager(this);
			owner = new TestInplaceEditorOwner(this);
			for(int i = 0; i < RowCount; i++) {
				grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
			}
			for(int i = 0; i < ColCount; i++) {
				grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			}
			for(int i = 0; i < RowCount; i++) {
				for(int j = 0; j < ColCount; j++) {
					TestInplaceEditor editor = new TestInplaceEditor(owner, new TestInplaceEditorColumn((ControlTemplate)FindResource("emptyValueTemplate"), new EditableDataObject() { Value = (i + j).ToString() }));
					TestBorder border = new TestBorder() { BorderThickness = new Thickness(1), Child = editor };
					Grid.SetRow(border, i);
					Grid.SetColumn(border, j);
					grid.Children.Add(border);
				}
			}
			LayoutUpdated += delegate { immediateActionsManager.ExecuteActions(); };
		}
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonDown(e);
			owner.ProcessMouseLeftButtonDown(e);
		}
		protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnPreviewMouseLeftButtonUp(e);
			owner.ProcessMouseLeftButtonUp(e);
		}
		protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e) {
			base.OnIsKeyboardFocusWithinChanged(e);
			owner.ProcessIsKeyboardFocusWithinChanged();
		}
		protected override void OnPreviewLostKeyboardFocus(KeyboardFocusChangedEventArgs e) {
			base.OnPreviewLostKeyboardFocus(e);
			owner.ProcessPreviewLostKeyboardFocus(e);
		}
		internal void EnqueueImmediateAction(IAction action) {
			immediateActionsManager.EnqueueAction(action);
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			if(e.Key == Key.Delete && ModifierKeysHelper.IsCtrlPressed(e.KeyboardDevice.Modifiers)) {
				foreach(TestBorder item in grid.Children) {
					((TestInplaceEditor)item.Child).Data.Value = null;
				}
				e.Handled = true;
			}
			base.OnPreviewKeyDown(e);
		}
		internal void ProcessKeyDown(KeyEventArgs e) {
			if(e.Key == Key.Tab) {
				owner.MoveFocus(e);
				return;
			}
			TestBorder editor = LayoutHelper.FindLayoutOrVisualParentObject<TestBorder>((DependencyObject)e.OriginalSource);
			int row = Grid.GetRow(editor);
			int col = Grid.GetColumn(editor);
			switch(e.Key) { 
				case Key.Up:
					row--;
					e.Handled = true;
					break;
				case Key.Down:
					row++;
					e.Handled = true;
					break;
				case Key.Left:
					col--;
					e.Handled = true;
					break;
				case Key.Right:
					col++;
					e.Handled = true;
					break;
			}
			TestBorder newEditor = Enumerable.FirstOrDefault(grid.Children.Cast<TestBorder>(), (element) => Grid.GetRow(element) == row && Grid.GetColumn(element) == col);
			if(newEditor != null)
				SetFocusedEditor(newEditor);
		}
		internal void PerformNavigationOnLeftButtonDown(DependencyObject originalSource) {
			TestBorder editor = LayoutHelper.FindLayoutOrVisualParentObject<TestBorder>(originalSource);
			SetFocusedEditor(editor);
		}
		void SetFocusedEditor(TestBorder editor) {
			foreach(TestBorder item in grid.Children) {
				((TestInplaceEditor)item.Child).IsEditorFocused = item == editor;
				item.BorderBrush = item == editor ? Brushes.Red : null;
			}
		}
	}
	public class TestBorder : Border { }
	public class TestInplaceEditorColumn : IInplaceEditorColumn {
		readonly TextEditSettings settings;
		readonly ControlTemplate emptyValueDisplayTemplate;
		public EditableDataObject Data { get; private set; }
		public TestInplaceEditorColumn(ControlTemplate emptyValueDisplayTemplate, EditableDataObject data) {
			settings = new TextEditSettings();
			this.emptyValueDisplayTemplate = emptyValueDisplayTemplate;
			this.Data = data;
		}
		#region IInplaceEditorColumn Members
		Settings.BaseEditSettings IInplaceEditorColumn.EditSettings { get { return settings; } }
		DataTemplateSelector IInplaceEditorColumn.EditorTemplateSelector { get { return null; } }
		ControlTemplate IInplaceEditorColumn.EditTemplate { get { return null; } }
		ControlTemplate IInplaceEditorColumn.DisplayTemplate {
			get { return Data.Value == null || (Data.Value as string) == string.Empty ? emptyValueDisplayTemplate : null; } 
		}
		event ColumnContentChangedEventHandler IInplaceEditorColumn.ContentChanged { add { } remove {  } }
		#endregion
		#region IDefaultEditorViewInfo Members
		HorizontalAlignment Settings.IDefaultEditorViewInfo.DefaultHorizontalAlignment { get { return HorizontalAlignment.Left; } }
		bool IDefaultEditorViewInfo.HasTextDecorations { get { return false; } }
		#endregion
	}
	public class TestInplaceEditorOwner : InplaceEditorOwnerBase {
		TestInplaceContainer Container { get { return (TestInplaceContainer)owner; } }
		public TestInplaceEditorOwner(TestInplaceContainer container) : base(container) {
		}
		protected override FrameworkElement FocusOwner { get { return owner; } }
		protected override Core.EditorShowMode EditorShowMode { get { return EditorShowMode.MouseDown; } }
		protected override bool EditorSetInactiveAfterClick { get { return false; } }
		protected override bool CanCommitEditing { get { return true; } }
		protected override Type OwnerBaseType { get { return typeof(TestInplaceContainer); } }
		protected internal override string GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value) { return originalDisplayText; }
		protected internal override bool? GetDisplayText(InplaceEditorBase inplaceEditor, string originalDisplayText, object value, out string displayText) {
			displayText = originalDisplayText;
			return true;
		}
		protected override bool CommitEditing() { return true; }
		public override void ProcessKeyDown(KeyEventArgs e) {
			Container.ProcessKeyDown(e);
		}
		protected override bool PerformNavigationOnLeftButtonDown(MouseButtonEventArgs e) {
			Container.PerformNavigationOnLeftButtonDown(e.OriginalSource as DependencyObject);
			return true;
		}
		protected internal override void EnqueueImmediateAction(IAction action) {
			Container.EnqueueImmediateAction(action);
		}
	}
	public class TestInplaceEditor : InplaceEditorBase {
		protected override EditorOptimizationMode GetEditorOptimizationMode() {
			return EditorOptimizationMode.Simple;
		}
		public bool IsEditorFocused {
			get { return (bool)GetValue(IsEditorFocusedProperty); }
			set { SetValue(IsEditorFocusedProperty, value); }
		}
		public static readonly DependencyProperty IsEditorFocusedProperty =
			DependencyProperty.Register("IsEditorFocused", typeof(bool), typeof(TestInplaceEditor), new UIPropertyMetadata(false, (d, e) =>((TestInplaceEditor)d).OnIsFocusedCellChanged()));
		readonly TestInplaceEditorOwner owner;
		readonly TestInplaceEditorColumn column;
		internal EditableDataObject Data { get { return column.Data; } }
		public TestInplaceEditor(TestInplaceEditorOwner owner, TestInplaceEditorColumn column) {
			this.owner = owner;
			this.column = column;
			OnOwnerChanged(null);
		}
		protected override bool IsCellFocused { get { return IsEditorFocused; } }
		protected override InplaceEditorOwnerBase Owner { get { return owner; } }
		protected override IInplaceEditorColumn EditorColumn { get { return column; } }
		protected override bool IsReadOnly { get { return false; } }
		protected override bool OverrideCellTemplate { get { return true; } }
		protected override bool IsInactiveEditorButtonVisible() { return false; }
		protected override object GetEditableValue() { return Data.Value; }
		protected override EditableDataObject GetEditorDataContext() { return Data; }
		protected override bool PostEditorCore() {
			Data.Value = EditableValue;
			return true;
		}
		protected override void UpdateEditValueCore(IBaseEdit editor) {
			editor.EditValue = GetEditableValue();
		}
		protected override void OnInnerContentChangedCore() {
			base.OnInnerContentChangedCore();
			UpdateDisplayTemplate();
		}
		protected override void OnHiddenEditor(bool closeEditor) {
			base.OnHiddenEditor(closeEditor);
			EditableValue = GetEditableValue();
		}
	}
}
