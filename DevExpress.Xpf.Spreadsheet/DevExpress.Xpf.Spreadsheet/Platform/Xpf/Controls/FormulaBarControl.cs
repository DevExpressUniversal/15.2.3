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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Spreadsheet.Internal;
using DevExpress.XtraSpreadsheet;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.Xpf.Spreadsheet {
	[DXToolboxBrowsable(true)]
	[ToolboxTabName(AssemblyInfo.DXTabWpfSpreadsheet)]
	public class SpreadsheetFormulaBarControl : Control, IFormulaBarControl, IFormulaBarControllerOwner {
		public static readonly DependencyProperty IsExpandedProperty;
		public static readonly DependencyProperty SpreadsheetControlProperty;
		protected static readonly DependencyPropertyKey OkCommandPropertyKey;
		protected static readonly DependencyPropertyKey CancelCommandPropertyKey;
		protected static readonly DependencyPropertyKey FunctionCommandPropertyKey;
		public static readonly DependencyProperty OkCommandProperty;
		public static readonly DependencyProperty CancelCommandProperty;
		public static readonly DependencyProperty FunctionCommandProperty;
		public static readonly DependencyProperty ShowNameBoxProperty;
		public static readonly DependencyProperty ShowFormulaBarButtonsProperty;
		static SpreadsheetFormulaBarControl() {
			Type ownerType = typeof(SpreadsheetFormulaBarControl);
			IsExpandedProperty = DependencyProperty.Register("IsExpanded", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((SpreadsheetFormulaBarControl)d).OnIsExpandedChanged()));
			SpreadsheetControlProperty = DependencyProperty.Register("SpreadsheetControl", typeof(ISpreadsheetControl), ownerType,
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetFormulaBarControl)d).OnSpreadsheetChanged(e.OldValue as SpreadsheetControl)));
			OkCommandPropertyKey = DependencyProperty.RegisterReadOnly("OkCommand", typeof(ICommand), ownerType, new FrameworkPropertyMetadata());
			OkCommandProperty = OkCommandPropertyKey.DependencyProperty;
			CancelCommandPropertyKey = DependencyProperty.RegisterReadOnly("CancelCommand", typeof(ICommand), ownerType, new FrameworkPropertyMetadata());
			CancelCommandProperty = CancelCommandPropertyKey.DependencyProperty;
			FunctionCommandPropertyKey = DependencyProperty.RegisterReadOnly("FunctionCommand", typeof(ICommand), ownerType, new FrameworkPropertyMetadata());
			FunctionCommandProperty = FunctionCommandPropertyKey.DependencyProperty;
			ShowNameBoxProperty = DependencyProperty.Register("ShowNameBox", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, (d, e) => ((SpreadsheetFormulaBarControl)d).OnShowNameBoxChanged()));
			ShowFormulaBarButtonsProperty = DependencyProperty.Register("ShowFormulaBarButtons", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true, (d, e) => ((SpreadsheetFormulaBarControl)d).OnShowFormulaBarButtonsChanged()));
		}
		public SpreadsheetFormulaBarControl() {
			DefaultStyleKey = typeof(SpreadsheetFormulaBarControl);
			CreateController();
			this.SizeChanged += SpreadsheetFormulaBarControlSizeChanged;
			this.LostFocus += SpreadsheetFormulaBarControlLostFocus;
			this.Loaded += SpreadsheetFormulaBarControlLoaded;
			this.Unloaded += SpreadsheetFormulaBarControlUnloaded;
			InitializeCommands();
		}
		void SpreadsheetFormulaBarControlUnloaded(object sender, RoutedEventArgs e) {
			UnSubcribeControlsEvents();
		}
		void SpreadsheetFormulaBarControlLoaded(object sender, RoutedEventArgs e) {
			SetSpreadsheetToController();
			SubcribeControlsEvents();
			SetRowHeight(MinResizeHeight);
			UpdateButtonsWidth();
			UpdateNameBoxWidth();
			SetButtonsEnabled(false);
		}
		private void InitializeCommands() {
			OkCommand = new FormulaBarCommand(new Action(RaiseButtonOkClick));
			CancelCommand = new FormulaBarCommand(new Action(RaiseButtonCancelClick));
			FunctionCommand = new FormulaBarCommand(new Action(RaiseButtonFunctionsClick));
		}
		public ICommand OkCommand {
			get { return (ICommand)GetValue(OkCommandProperty); }
			protected set { SetValue(OkCommandPropertyKey, value); }
		}
		public ICommand CancelCommand {
			get { return (ICommand)GetValue(CancelCommandProperty); }
			protected set { SetValue(CancelCommandPropertyKey, value); }
		}
		public ICommand FunctionCommand {
			get { return (ICommand)GetValue(FunctionCommandProperty); }
			protected set { SetValue(FunctionCommandPropertyKey, value); }
		}
		public bool EditMode { get { return editor != null ? editor.Registered : false; } }
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
		public bool ShowNameBox {
			get { return (bool)GetValue(ShowNameBoxProperty); }
			set { SetValue(ShowNameBoxProperty, value); }
		}
		public bool ShowFormulaBarButtons {
			get { return (bool)GetValue(ShowFormulaBarButtonsProperty); }
			set { SetValue(ShowFormulaBarButtonsProperty, value); }
		}
		public ISpreadsheetControl SpreadsheetControl {
			get { return (ISpreadsheetControl)GetValue(SpreadsheetControlProperty); }
			set { SetValue(SpreadsheetControlProperty, value); }
		}
		SpreadsheetControl Control { get { return SpreadsheetControl as SpreadsheetControl; } }
		double ExpandHeight { get; set; }
		double MinResizeHeight { get { return MinHeight; } }
		double NameBoxMinWidth { get; set; }
		void SpreadsheetFormulaBarControlLostFocus(object sender, RoutedEventArgs e) {
		}
		private void CreateController() {
			Controller = new FormulaBarController(this);
			SubcribeControllerEvents();
		}
		private void SubcribeControllerEvents() {
			Controller.SelectionChanged += OnControllerSelectionChanged;
		}
		private void OnControllerSelectionChanged(object sender, EventArgs e) {
			if (editor != null)
				editor.Text = Controller.OwnersText;
		}
		void SpreadsheetFormulaBarControlSizeChanged(object sender, SizeChangedEventArgs e) {
			if (IsExpanded)
				ExpandHeight = e.NewSize.Height - horizontalThumb.ActualHeight;
		}
		public FormulaBarController Controller { get; private set; }
		private void SetSpreadsheetToController() {
			if (Controller != null && SpreadsheetControl != null && Control.DocumentModel.ActiveSheet != null) Controller.SpreadsheetControl = SpreadsheetControl;
		}
		FormulaBarEditor editor;
		SpreadsheetNameBoxControl nameBox;
		Thumb horizontalThumb;
		Thumb verticalThumb;
		Grid formulaBarEditorContainer;
		Grid root;
		ScrollBar scrollBar;
		Button okButton;
		Button cancelButton;
		Button functionButton;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UnSubcribeControlsEvents();
			editor = LayoutHelper.FindElementByType(this, typeof(FormulaBarEditor)) as FormulaBarEditor;
			nameBox = LayoutHelper.FindElementByType(this, typeof(SpreadsheetNameBoxControl)) as SpreadsheetNameBoxControl;
			root = LayoutHelper.FindElementByType(this, typeof(Grid)) as Grid;
			if (SpreadsheetControl != null && nameBox != null)
				nameBox.SpreadsheetControl = SpreadsheetControl;
			if (scrollBar != null) scrollBar.ValueChanged -= OnScrollBarValueChanged;
			scrollBar = LayoutHelper.FindElementByType(this, typeof(ScrollBar)) as ScrollBar;
			scrollBar.ValueChanged += OnScrollBarValueChanged;
			formulaBarEditorContainer = LayoutHelper.FindElementByName(this, "PART_FormulaBarEditorContainer") as Grid;
			horizontalThumb = LayoutHelper.FindElementByName(this, "PART_HorizontalThumb") as Thumb;
			verticalThumb = LayoutHelper.FindElementByName(this, "PART_VerticalThumb") as Thumb;
			okButton = LayoutHelper.FindElementByName(this, "PART_OkButton") as Button;
			cancelButton = LayoutHelper.FindElementByName(this, "PART_CancelButton") as Button;
			functionButton = LayoutHelper.FindElementByName(this, "PART_FunctionButton") as Button;
			SetNameBoxMinWidth();
			SetSpreadsheetToController();
			if (SpreadsheetControl != null) editor.InitializeSpreadsheet(SpreadsheetControl);
		}
		private void SetNameBoxMinWidth() {
			NameBoxMinWidth = GetNameBoxColumn().MinWidth;
		}
		void OnScrollBarValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			if (editor != null) editor.ScrollToVerticalOffset(e.NewValue);
		}
		private void SubcribeControlsEvents() {
			if (verticalThumb != null)
				verticalThumb.DragDelta += OnVerticalDragDelta;
			if (horizontalThumb != null) {
				horizontalThumb.DragDelta += OnHorizontalDragDelta;
				horizontalThumb.DragCompleted += OnHorizontalDragCompleted;
			}
			if (editor != null) {
				editor.Rollback += OnEditorRollback;
				editor.GotFocus += OnEditorGotFocus;
				editor.ActivateEditor += OnActivateEditor;
				editor.DeactivateEditor += OnDeactivateEditor;
			}
			if (okButton != null)
				okButton.Click += OnOkButtonClick;
			if (cancelButton != null)
				cancelButton.Click += OnCancelButtonClick;
			if (functionButton != null)
				functionButton.Click += OnFunctionButtonClick;
		}
		void OnFunctionButtonClick(object sender, RoutedEventArgs e) {
			RaiseButtonFunctionsClick();
		}
		void OnCancelButtonClick(object sender, RoutedEventArgs e) {
			RaiseButtonCancelClick();
		}
		void OnOkButtonClick(object sender, RoutedEventArgs e) {
			RaiseButtonOkClick();
		}
		void OnEditorGotFocus(object sender, RoutedEventArgs e) {
			SetButtonsEnabled(true);
			RaiseFormulaBarEnter();
			CheckInplaceEditorActive();
		}
		void CheckInplaceEditorActive() {
			if (Control == null)
				return;
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			if (innerControl != null && !innerControl.IsInplaceEditorActive) {
				SetButtonsEnabled(false);
				Control.Focus();
			}
		}
		void OnHorizontalDragCompleted(object sender, DragCompletedEventArgs e) {
			isDrag = false;
		}
		void UnSubcribeControlsEvents() {
			if (verticalThumb != null)
				verticalThumb.DragDelta -= OnVerticalDragDelta;
			if (horizontalThumb != null) {
				horizontalThumb.DragDelta -= OnHorizontalDragDelta;
				horizontalThumb.DragCompleted -= OnHorizontalDragCompleted;
			}
			if (editor != null) {
				editor.Rollback -= OnEditorRollback;
				editor.GotFocus -= OnEditorGotFocus;
				editor.ActivateEditor -= OnActivateEditor;
				editor.DeactivateEditor -= OnDeactivateEditor;
			}
			if (okButton != null)
				okButton.Click -= OnOkButtonClick;
			if (cancelButton != null)
				cancelButton.Click -= OnCancelButtonClick;
			if (functionButton != null)
				functionButton.Click -= OnFunctionButtonClick;
		}
		void OnActivateEditor(object sender, EventArgs e) {
			SetButtonsEnabled(true);
		}
		void OnDeactivateEditor(object sender, EventArgs e) {
			SetButtonsEnabled(false);
		}
		void SetButtonsEnabled(bool isEanbled) {
			if (okButton != null)
				okButton.IsEnabled = isEanbled;
			if (cancelButton != null)
				cancelButton.IsEnabled = isEanbled;
			if (functionButton != null)
				functionButton.IsEnabled = false;
		}
		void OnEditorRollback(object sender, EventArgs e) {
			RaiseFormulaBarKeyDown(new System.Windows.Forms.KeyEventArgs(System.Windows.Forms.Keys.Escape));
			RaiseRollback();
		}
		void OnVerticalDragDelta(object sender, DragDeltaEventArgs e) {
			ResizeFormulaBarWidth(e.HorizontalChange);
		}
		private void ResizeFormulaBarWidth(double delta) {
			if (formulaBarEditorContainer != null) {
				double width = formulaBarEditorContainer.ColumnDefinitions[0].Width.Value;
				double value = width + delta;
				formulaBarEditorContainer.ColumnDefinitions[0].Width =
					new GridLength(Math.Min(this.ActualWidth - verticalThumb.ActualWidth, Math.Max(verticalThumb.ActualWidth, value)), GridUnitType.Pixel);
			}
		}
		bool isDrag = false;
		void OnHorizontalDragDelta(object sender, DragDeltaEventArgs e) {
			isDrag = true;
			ResizeFormulaBarHeight(e.VerticalChange);
		}
		private void ResizeFormulaBarHeight(double delta) {
			double height = ActualHeight + delta;
			IsExpanded = height > MinResizeHeight;
			SetRowHeight(Math.Max(height, MinResizeHeight));
		}
		private void OnShowFormulaBarButtonsChanged() {
			UpdateButtonsWidth();
		}
		private void UpdateButtonsWidth() {
			ColumnDefinition column = GetButtonsColumn();
			if (column == null) return;
			GridLength length = ShowFormulaBarButtons ? new GridLength(1, GridUnitType.Auto) : new GridLength(column.MinWidth);
			column.Width = length;
			InvalidateMeasure();
		}
		private ColumnDefinition GetButtonsColumn() {
			if (formulaBarEditorContainer == null) return null;
			return formulaBarEditorContainer.ColumnDefinitions[2];
		}
		private void OnShowNameBoxChanged() {
			UpdateNameBoxWidth();
		}
		private void UpdateNameBoxWidth() {
			ColumnDefinition column = GetNameBoxColumn();
			ColumnDefinition column2 = GetHorizontalResizeThumbColumn();
			if (column == null || column2 == null) return;
			if (ShowNameBox) {
				column.MinWidth = NameBoxMinWidth;
				column.Width = new GridLength(NameBoxMinWidth);
				column2.Width = new GridLength(1, GridUnitType.Auto);
			}
			else {
				column2.Width = new GridLength(0);
				column.MinWidth = 0;
				column.Width = new GridLength(0);
			}
			InvalidateMeasure();
		}
		private ColumnDefinition GetNameBoxColumn() {
			if (formulaBarEditorContainer == null) return null;
			return formulaBarEditorContainer.ColumnDefinitions[0];
		}
		private ColumnDefinition GetHorizontalResizeThumbColumn() {
			if (formulaBarEditorContainer == null) return null;
			return formulaBarEditorContainer.ColumnDefinitions[1];
		}
		private void OnIsExpandedChanged() {
			if (!isDrag) {
				if (IsExpanded) SetRowHeight(Math.Max(MinResizeHeight, GetExpandHeight()));
				else SetRowHeight(MinResizeHeight);
			}
		}
		private void SetRowHeight(double height) {
			if (root != null) {
				root.RowDefinitions[0].Height = new GridLength(height, GridUnitType.Pixel);
			}
		}
		double defaultExpandHeight = 100;
		private double GetExpandHeight() {
			return ExpandHeight > MinResizeHeight + horizontalThumb.ActualHeight ? ExpandHeight : defaultExpandHeight;
		}
		private void OnSpreadsheetChanged(SpreadsheetControl oldValue) {
			if (SpreadsheetControl != null) {
				if (oldValue != null) {
					RemoveFormulaBarService();
					UnSubcribeEvents();
				}
				RegistrateFormulaBarService();
				SubcribeEvents();
				SetSpreadsheetToController();
				if (editor != null) editor.InitializeSpreadsheet(SpreadsheetControl);
			}
		}
		private void SubcribeEvents() {
			((SpreadsheetControl)SpreadsheetControl).InnerControlInitialized += OnInnerControlInitialized;
		}
		void OnInnerControlInitialized(object sender, EventArgs e) {
			SetSpreadsheetToController();
		}
		private void UnSubcribeEvents() {
			((SpreadsheetControl)SpreadsheetControl).InnerControlInitialized -= OnInnerControlInitialized;
		}
		void SpreadsheetControlSelectionChanged(object sender, EventArgs e) {
			if (editor != null) editor.Text = SpreadsheetControl.SelectedCell.Value.ToString();
		}
		private void RemoveFormulaBarService() {
			SpreadsheetControl.RemoveService(typeof(IFormulaBarControl));
		}
		private void RegistrateFormulaBarService() {
			SpreadsheetControl.AddService(typeof(IFormulaBarControl), this);
		}
		#region IFormulaBarControl Members
		XtraSpreadsheet.Internal.ICellInplaceEditor IFormulaBarControl.InplaceEditor {
			get { return editor; }
		}
		bool IFormulaBarControl.Expanded {
			get { return IsExpanded; }
			set { IsExpanded = value; }
		}
		#endregion
		#region IFormulaBarControllerOwner Members
		EventHandler onButtonOkClick;
		event EventHandler IFormulaBarControllerOwner.OkButtonClick { add { onButtonOkClick += value; } remove { onButtonOkClick -= value; } }
		protected internal virtual void RaiseButtonOkClick() {
			if (onButtonOkClick != null)
				onButtonOkClick(this, EventArgs.Empty);
		}
		EventHandler onButtonFunctionsClick;
		event EventHandler IFormulaBarControllerOwner.InsertFunctionButtonClick { add { onButtonFunctionsClick += value; } remove { onButtonFunctionsClick -= value; } }
		protected internal virtual void RaiseButtonFunctionsClick() {
			if (onButtonFunctionsClick != null)
				onButtonFunctionsClick(this, EventArgs.Empty);
		}
		EventHandler onButtonCancelClick;
		event EventHandler IFormulaBarControllerOwner.CancelButtonClick { add { onButtonCancelClick += value; } remove { onButtonCancelClick -= value; } }
		protected internal virtual void RaiseButtonCancelClick() {
			if (onButtonCancelClick != null)
				onButtonCancelClick(this, EventArgs.Empty);
		}
		EventHandler onRollback;
		event EventHandler IFormulaBarControllerOwner.Rollback { add { onRollback += value; } remove { onRollback -= value; } }
		protected internal virtual void RaiseRollback() {
			if (onRollback != null)
				onRollback(this, EventArgs.Empty);
		}
		EventHandler onFormulaBarEnter;
		event EventHandler IFormulaBarControllerOwner.Enter { add { onFormulaBarEnter += value; } remove { onFormulaBarEnter -= value; } }
		protected internal virtual void RaiseFormulaBarEnter() {
			if (onFormulaBarEnter != null) {
				onFormulaBarEnter(this, EventArgs.Empty);
			}
		}
		System.Windows.Forms.KeyEventHandler onFormulaBarKeyDown;
		event System.Windows.Forms.KeyEventHandler IFormulaBarControllerOwner.KeyDown { add { onFormulaBarKeyDown += value; } remove { onFormulaBarKeyDown -= value; } }
		protected internal virtual void RaiseFormulaBarKeyDown(System.Windows.Forms.KeyEventArgs e) {
			if (onFormulaBarKeyDown != null) {
				onFormulaBarKeyDown(this, e);
			}
		}
		#endregion
	}
	public class FormulaBarCommand : ICommand {
		public FormulaBarCommand(Action predicate) {
			Predicate = predicate;
		}
		public Action Predicate { get; protected set; }
		#region ICommand Members
		public bool CanExecute(object parameter) {
			return Predicate != null;
		}
		public event EventHandler CanExecuteChanged { add { } remove { } }
		public void Execute(object parameter) {
			Predicate.DynamicInvoke(new object[] { });
		}
		#endregion
	}
}
