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
using System.Windows.Controls;
using System.Windows;
using DevExpress.Xpf.Core;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Settings;
using System.Windows.Markup;
using System.Windows.Data;
using DevExpress.Data.Helpers;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System.Globalization;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DevExpress.Mvvm.Native;
#if !SL
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
#endif
namespace DevExpress.Xpf.Editors.Filtering {
	public class FilterControlEditor : ContentControl, IFilterControlNavigationItem {
		#region dependency properties fields
		public static readonly DependencyProperty EditModeProperty;
		public static readonly DependencyProperty NodeProperty;
		public static readonly DependencyProperty FieldInValueProperty;
		public static readonly DependencyProperty OperatorProperty;
		public static readonly DependencyProperty IndexProperty;
		#endregion
		static FilterControlEditor() {
			Type ownerType = typeof(FilterControlEditor);
			EditModeProperty = DependencyPropertyManager.Register("EditMode", typeof(EditMode), ownerType, new PropertyMetadata(EditMode.InplaceInactive, (d, e) => ((FilterControlEditor)d).OnEditModeChanged()));
			NodeProperty = DependencyPropertyManager.Register("Node", typeof(ClauseNode), ownerType, new PropertyMetadata(null, (d, e) => ((FilterControlEditor)d).OnNodeChanged(e)));
			FieldInValueProperty = DependencyPropertyManager.Register("FieldInValue", typeof(bool), ownerType, new PropertyMetadata(false));
			OperatorProperty = DependencyPropertyManager.Register("Operator", typeof(CriteriaOperator), ownerType, new PropertyMetadata(null));
			IndexProperty = DependencyPropertyManager.Register("Index", typeof(int), ownerType, new PropertyMetadata(-1, (d, e) => ((FilterControlEditor)d).IndexChanged((int)e.NewValue)));
		}
		#region dependency properties accessors
		public ClauseNode Node {
			get { return (ClauseNode)GetValue(NodeProperty); }
			set { SetValue(NodeProperty, value); }
		}
		public bool FieldInValue {
			get { return (bool)GetValue(FieldInValueProperty); }
			set { SetValue(FieldInValueProperty, value); }
		}
		public CriteriaOperator Operator {
			get { return (CriteriaOperator)GetValue(OperatorProperty); }
			set { SetValue(OperatorProperty, value); }
		}
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public EditMode EditMode {
			get { return (EditMode)GetValue(EditModeProperty); }
			set { SetValue(EditModeProperty, value); }
		}
		#endregion
		public ICommand ChangeOperandTypeCommand { get; private set; }
		public ICommand ChangeColumnCommand { get; private set; }
		ClauseNode lastNode = null;
		void OnNodeChanged(DependencyPropertyChangedEventArgs e) {
			if(e.NewValue != null)
				lastNode = (ClauseNode)e.NewValue;
		}
		bool isInitialized = false;
		void IndexChanged(int newValue) {
			InitializeEditor(newValue);
		}
		void InitializeEditor(int index) {
			if (Node == null || isInitialized || index < 0)
				return;
			isInitialized = true;
			CreateEditor();
			Node.Editors.Insert(index, this);
		}
		public FilterControlEditor() {
			this.SetDefaultStyleKey(typeof(FilterControlEditor));
			Loaded += new RoutedEventHandler(FilterControlEditorLoaded);
			Unloaded += new RoutedEventHandler(FilterControlEditorUnloaded);
			ChangeOperandTypeCommand = DelegateCommandFactory.Create<object>(obj => ChangeOperandType(), false);
			ChangeColumnCommand = DelegateCommandFactory.Create<string>(str => ChangeColumn(str), false);
		}
		void ChangeOperandType() {
			int index = NavigationIndex;
			if(FieldInValue) {
				ResetOperator(Node.Owner.CreateDefaultProperty(Node));
				CreateEditor();
			} else
				ResetOperator(new OperandValue(null));
			Node.Owner.FocusNodeChild(Node, index);
		}
		void ChangeColumn(string parameter) {
			int index = NavigationIndex;
			ResetOperator(new OperandProperty(parameter));
			ApplyColumnTemplate(parameter);
			Node.Owner.FocusNodeChild(Node, index);
		}
		void ApplyColumnTemplate(string fieldName) {
			FilterColumn column = Node.Owner.GetColumnByFieldName(fieldName);
			if(column != null && ColumnButton != null) {
				ColumnButton.ContentTemplate = column.HeaderTemplate;
				ColumnButton.ContentTemplateSelector = column.HeaderTemplateSelector;
			}
		}
		void ResetOperator(CriteriaOperator newOperator) {
			if (Index < 0)
				return;
			Node.ResetAdditionalOperand(newOperator, Index);
		}
		internal EditableDataObject Data { get; set; }
		XPFContentControl ColumnButton { get; set; }
		void FilterControlEditorLoaded(object sender, RoutedEventArgs e) {
			if(Node == null)
				return;
			InitializeEditor(Index);
		}
		void FilterControlEditorUnloaded(object sender, RoutedEventArgs e) {
			isInitialized = false;
			lastNode.Editors.Remove(this);
		}
		public void ResetEditor() {
			if (Node == null || (Node.Owner.GetColumnByFieldName(Node.FirstOperand.PropertyName) == null))
				return;
			object currentValue = Data.Value;
			if (currentValue != null) 
				ResetEditorData(Node.Owner.GetColumnByFieldName(Node.FirstOperand.PropertyName).ColumnType, currentValue);
			AddEditorToContent(Node.Owner.GetColumnByFieldName(Node.FirstOperand.PropertyName).EditSettings);
		}
		protected virtual void ResetEditorData(Type columnType, object currentValue) {
			if (!TypeConvertionValidator.CanConvert(currentValue, columnType))
				ResetData(null);
			else {
				columnType = Nullable.GetUnderlyingType(columnType) ?? columnType;
				currentValue = currentValue is IConvertible ? currentValue : Convert.ToString(currentValue, CultureInfo.CurrentCulture);
				try {
					object convertedValue = Convert.ChangeType(currentValue, columnType, CultureInfo.CurrentCulture);
					ResetData(convertedValue);
				}
				catch {
					ResetData(null);
				}
			}
		}
		void CreateEditor() {
			if(Operator is OperandValue) {
				ResetData(((OperandValue)Operator).Value);
			} else if(Operator is OperandProperty) {
				FieldInValue = true;
				ResetData(null);
				ApplyColumnTemplate(((OperandProperty)Operator).PropertyName);
			} else {
				ResetData(null);
			}
			FilterColumn column = Node.Owner.GetColumnByFieldName(Node.FirstOperand.PropertyName);
			BaseEditSettings editSettings = (column != null) ? column.EditSettings : new TextEditSettings();
			AddEditorToContent(editSettings);
		}
		void AddEditorToContent(BaseEditSettings editSettings) {
			Content = new InplaceFilterEditor(Node.Owner.EditorsOwner, new InplaceFilterEditorColumn(editSettings, Node.Owner.EmptyValueTemplate, Node.Owner.EmptyStringTemplate, Node.Owner.ValueTemplate, Node.Owner.BooleanValueTemplate, Data), Node);
		}
		void ResetData(object value) {
			if(Data != null)
				Data.ContentChanged -= new EventHandler(DataContentChanged);
			Data = new EditableDataObject() { Value = value };
			Data.ContentChanged += new EventHandler(DataContentChanged);
			FilterColumn column = Node.Owner.GetColumnByFieldName(Node.FirstOperand.PropertyName);
			if(!FieldInValue) {
				value = (column != null) ? FilterHelper.CorrectFilterValueType(column.ColumnType, value) : value;
				ResetOperator(new OperandValue(value));
			}
		}
		internal void ColumnButtonMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			Node.Owner.FocusNodeChild(Node, NavigationIndex);
			Node.VisualClauseNode.CreateColumnsMenu(ColumnButton, ChangeColumnCommand);
		}
		void DataContentChanged(object sender, EventArgs e) {
			if(Node != null) {
				FilterColumn column = Node.Owner.GetColumnByFieldName(Node.FirstOperand.PropertyName);
				if(column != null)
					ResetOperator(new OperandValue(FilterHelper.CorrectFilterValueType(column.ColumnType, ((EditableDataObject)sender).Value)));
			}
		}
		ContentPresenter editorPresenter;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			editorPresenter = (ContentPresenter)GetTemplateChild("PART_EditorPresenter");
			if(ColumnButton != null)
				ColumnButton.RemoveMouseUpHandler(ColumnButtonMouseUp);
			ColumnButton = GetTemplateChild("PART_Column") as XPFContentControl;
			if(ColumnButton != null)
				ColumnButton.AddMouseUpHandler(ColumnButtonMouseUp);
			UpdateVisualStateCore(false);
		}
		void OnEditModeChanged() {
			UpdateVisualStateCore(true);
		}
		void UpdateVisualStateCore(bool useTransition) {
			if(EditMode == EditMode.InplaceActive) {
				editorPresenter.ClearValue(ContentPresenter.CursorProperty);
			} else {
				editorPresenter.Cursor = Cursors.Hand;
			}
			VisualStateManager.GoToState(this, EditMode.ToString(), useTransition);
		}
		int NavigationIndex {
			get {
				UIElement navigationChild = (this as IFilterControlNavigationItem).Child;
				return (Node.VisualClauseNode as IFilterControlNavigationNode).Children.IndexOf(navigationChild);
			}
		}
		#region IFilterControlNavigationItem Members
		UIElement IFilterControlNavigationItem.Child {
			get {
				return FieldInValue ? GetTemplateChild("PART_Column") as UIElement : Content as UIElement;
			}
		}
		bool IFilterControlNavigationItem.ShowPopupMenu() {
			ColumnButtonMouseUp(null, null);
			return true;
		}
		#endregion
	}
	internal delegate bool DisplayTextIsEmptyDelegate();
	public class InplaceFilterEditorColumn : IInplaceEditorColumn {
		readonly BaseEditSettings settings;
		readonly ControlTemplate emptyValueDisplayTemplate;
		readonly ControlTemplate valueTemplate;
		readonly ControlTemplate stringEmptyDisplayTemplate;
		readonly ControlTemplate booleanValueTemplate;
		public EditableDataObject Data { get; private set; }
		public InplaceFilterEditorColumn(BaseEditSettings settings, ControlTemplate emptyValueDisplayTemplate, ControlTemplate stringEmptyDisplayTemplate, ControlTemplate valueTemplate, ControlTemplate booleanValueTemplate, EditableDataObject data) {
			this.settings = settings;
			this.emptyValueDisplayTemplate = emptyValueDisplayTemplate;
			this.valueTemplate = valueTemplate;
			this.stringEmptyDisplayTemplate = stringEmptyDisplayTemplate;
			this.booleanValueTemplate = booleanValueTemplate;
			this.Data = data;
		}
		DisplayTextIsEmptyDelegate DisplayTextIsEmpty;
		internal void SetDisplayTextIsEmpty(DisplayTextIsEmptyDelegate DisplayTextIsEmpty) {
			this.DisplayTextIsEmpty = DisplayTextIsEmpty;
		}
		#region IInplaceEditorColumn Members
		Settings.BaseEditSettings IInplaceEditorColumn.EditSettings { get { return settings; } }
		DataTemplateSelector IInplaceEditorColumn.EditorTemplateSelector { get { return null; } }
		ControlTemplate IInplaceEditorColumn.EditTemplate { get { return null; } }
		ControlTemplate IInplaceEditorColumn.DisplayTemplate {
			get {
				if(Data.Value == null)
					return emptyValueDisplayTemplate;
				if(settings is CheckEditSettings)
					return booleanValueTemplate;
				if(((Data.Value as string) == string.Empty) || DisplayTextIsEmpty())
					return stringEmptyDisplayTemplate;
				return valueTemplate;
			}
		}
		event ColumnContentChangedEventHandler IInplaceEditorColumn.ContentChanged { add { } remove { } }
		#endregion
		#region IDefaultEditorViewInfo Members
		HorizontalAlignment Settings.IDefaultEditorViewInfo.DefaultHorizontalAlignment { get { return HorizontalAlignment.Left; } }
		bool IDefaultEditorViewInfo.HasTextDecorations { get { return false; } }
		#endregion
	}
	public class InplaceFilterEditorOwner : InplaceEditorOwnerBase {
		FilterControl Container { get { return (FilterControl)owner; } }
		public InplaceFilterEditorOwner(FilterControl container)
			: base(container) {
		}
		protected override FrameworkElement FocusOwner { get { return owner; } }
		protected override Core.EditorShowMode EditorShowMode { get { return EditorShowMode.MouseDown; } }
		protected override bool EditorSetInactiveAfterClick { get { return false; } }
		protected override bool CanCommitEditing { get { return true; } }
		protected override Type OwnerBaseType { get { return typeof(FilterControl); } }
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
	public partial class InplaceFilterEditor : InplaceEditorBase {
		public bool IsEditorFocused {
			get { return (bool)GetValue(IsEditorFocusedProperty); }
			set { SetValue(IsEditorFocusedProperty, value); }
		}
		internal void SetActiveForTest() {
			SetActiveEditMode();
		}
		void ClearEditorValidation(IBaseEdit editor) {
			if(editor is SpinEdit) {
				(editor as SpinEdit).MinValue = null;
				(editor as SpinEdit).MaxValue = null;
			}
			if(editor is DateEdit) {
				(editor as DateEdit).MinValue = null;
				(editor as DateEdit).MaxValue = null;
			}
		}
		public static readonly DependencyProperty IsEditorFocusedProperty =
			DependencyPropertyManager.Register("IsEditorFocused", typeof(bool), typeof(InplaceFilterEditor), new PropertyMetadata(false, (d, e) => ((InplaceFilterEditor)d).OnIsFocusedCellChanged()));
		readonly InplaceFilterEditorOwner owner;
		readonly InplaceFilterEditorColumn column;
		internal EditableDataObject Data { get { return column.Data; } }
		ClauseNode Node { get; set; }
		public InplaceFilterEditor(InplaceFilterEditorOwner owner, InplaceFilterEditorColumn column, ClauseNode node) {
			this.SetDefaultStyleKey(typeof(InplaceFilterEditor));
			SetContentAlignment();
			this.Node = node;
			this.owner = owner;
			this.column = column;
			column.SetDisplayTextIsEmpty(DisplayTextIsEmpty);
			OnOwnerChanged(null);
		}
		bool DisplayTextIsEmpty() {
			return (((editCore as BaseEdit) != null) && ((BaseEdit)editCore).DisplayText == string.Empty);
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
			UpdateDisplayTemplate();
			return true;
		}
		protected override void UpdateEditValueCore(IBaseEdit editor) {
			editor.EditValue = GetEditableValue();
		}
		protected override void OnHiddenEditor(bool closeEditor) {
			base.OnHiddenEditor(closeEditor);
			EditableValue = GetEditableValue();
		}
		protected override void CheckFocus() {
#if SL
			FilterControlNodeBase node = this.FindElementByTypeInParents<FilterControlNodeBase>(Owner.TopOwner);
			int index = (node as IFilterControlNavigationNode).Children.IndexOf(this);
			node.Node.Owner.FocusNodeChild(node.Node, index);
#else
			Focus();
#endif
		}
		public override void ValidateEditorCore() {
			base.ValidateEditorCore();
			Edit.DoValidate();
		}
	}
	public class CheckedUncheckedBoolToStringConverter : BoolToStringConverter {
		public CheckedUncheckedBoolToStringConverter()
			: base(EditorLocalizer.GetString(EditorStringId.FilterEditorChecked), EditorLocalizer.GetString(EditorStringId.FilterEditorUnchecked)) {
		}
	}
	public class OperatorPropertyNameConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			OperandProperty property = value as OperandProperty;
			return CriteriaOperator.ReferenceEquals(property, null) ? string.Empty : property.PropertyName;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
