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
using System.Windows.Input;
using DevExpress.Data;
using DevExpress.Data.ExpressionEditor;
using DevExpress.Utils;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Helpers;
namespace DevExpress.Xpf.Editors.ExpressionEditor {
	public class ExpressionEditorControl : Control, IExpressionEditor, IDialogContent {
		internal DevExpress.Xpf.Editors.TextEdit expressionTextEdit;
		internal System.Windows.Controls.Button buttonPlus;
		internal System.Windows.Controls.Button buttonMinus;
		internal System.Windows.Controls.Button buttonMultiply;
		internal System.Windows.Controls.Button buttonDivide;
		internal System.Windows.Controls.Button buttonModulo;
		internal System.Windows.Controls.Button wrapSelectionButton;
		internal System.Windows.Controls.Button buttonEqual;
		internal System.Windows.Controls.Button buttonNotEqual;
		internal System.Windows.Controls.Button buttonLess;
		internal System.Windows.Controls.Button buttonLessOrEqual;
		internal System.Windows.Controls.Button buttonLargerOrEqual;
		internal System.Windows.Controls.Button buttonLarger;
		internal System.Windows.Controls.Button buttonAnd;
		internal System.Windows.Controls.Button buttonOr;
		internal System.Windows.Controls.Button buttonNot;
		internal System.Windows.Controls.ListBox listOfInputTypes;
		internal DevExpress.Xpf.Editors.ComboBoxEdit functionsTypes;
		internal DevExpress.Xpf.Core.DXListBox listOfInputParameters;
		internal DevExpress.Xpf.Editors.TextEdit descriptionEdit;
		static void SetButtonImage(Button button, string imageName) {
			if (imageName == "Plus")
				return;
			button.Content = new Image() { Source = new BitmapImage(UriHelper.GetUri(XmlNamespaceConstants.UtilsNamespace, "/Editors/Images/ExpressionEditor/" + imageName + ".png")) };
		}
		protected ExpressionEditorLogic fEditorLogic;
		protected IMemoEdit ExpressionMemoEdit;
		ISelector ListOfInputTypes;
		ISelector ListOfInputParameters;
		ISelector FunctionsTypes;
		public string Expression { get { return fEditorLogic.GetExpression(); } }
		protected IDataColumnInfo ColumnInfo { get; set; }
#if DEBUGTEST
		public ListBox ListOfInputParametersControl { get { return listOfInputParameters; } }
		public TextEdit ExpressionMemoEditControl { get { return expressionTextEdit; } }
		internal bool TemplateApplied = false;
#endif
		public ExpressionEditorControl() {
			DefaultStyleKey = typeof(ExpressionEditorControl);
			Loaded += ExpressionEditorControl_Loaded;
		}
		public ExpressionEditorControl(IDataColumnInfo columnInfo) : this() {
			ColumnInfo = columnInfo;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			if (listOfInputParameters != null) {
				listOfInputParameters.ItemLeftButtonDoubleClick -= listOfInputParameters_MouseDoubleClick;
				listOfInputParameters.SelectionChanged -= listOfInputParameters_SelectionChanged;
			}
			if (wrapSelectionButton != null)
				wrapSelectionButton.Click -= wrapSelectionButton_Click;
			if (listOfInputTypes != null)
				listOfInputTypes.SelectionChanged -= listOfInputTypes_SelectionChanged;
			if (functionsTypes != null)
				functionsTypes.SelectedIndexChanged -= functionsTypes_SelectedIndexChanged;
			UnsubscribeStandardOperationButtons();
			this.expressionTextEdit = ((DevExpress.Xpf.Editors.TextEdit)(GetTemplateChild("expressionTextEdit")));
			this.buttonPlus = ((System.Windows.Controls.Button)(GetTemplateChild("buttonPlus")));
			this.buttonMinus = ((System.Windows.Controls.Button)(GetTemplateChild("buttonMinus")));
			this.buttonMultiply = ((System.Windows.Controls.Button)(GetTemplateChild("buttonMultiply")));
			this.buttonDivide = ((System.Windows.Controls.Button)(GetTemplateChild("buttonDivide")));
			this.buttonModulo = ((System.Windows.Controls.Button)(GetTemplateChild("buttonModulo")));
			this.wrapSelectionButton = ((System.Windows.Controls.Button)(GetTemplateChild("wrapSelectionButton")));
			this.buttonEqual = ((System.Windows.Controls.Button)(GetTemplateChild("buttonEqual")));
			this.buttonNotEqual = ((System.Windows.Controls.Button)(GetTemplateChild("buttonNotEqual")));
			this.buttonLess = ((System.Windows.Controls.Button)(GetTemplateChild("buttonLess")));
			this.buttonLessOrEqual = ((System.Windows.Controls.Button)(GetTemplateChild("buttonLessOrEqual")));
			this.buttonLargerOrEqual = ((System.Windows.Controls.Button)(GetTemplateChild("buttonLargerOrEqual")));
			this.buttonLarger = ((System.Windows.Controls.Button)(GetTemplateChild("buttonLarger")));
			this.buttonAnd = ((System.Windows.Controls.Button)(GetTemplateChild("buttonAnd")));
			this.buttonOr = ((System.Windows.Controls.Button)(GetTemplateChild("buttonOr")));
			this.buttonNot = ((System.Windows.Controls.Button)(GetTemplateChild("buttonNot")));
			this.listOfInputTypes = ((System.Windows.Controls.ListBox)(GetTemplateChild("listOfInputTypes")));
			this.functionsTypes = ((DevExpress.Xpf.Editors.ComboBoxEdit)(GetTemplateChild("functionsTypes")));
			this.listOfInputParameters = ((DevExpress.Xpf.Core.DXListBox)(GetTemplateChild("listOfInputParameters")));
			this.descriptionEdit = ((DevExpress.Xpf.Editors.TextEdit)(GetTemplateChild("descriptionEdit")));
			if (listOfInputParameters != null) {
				listOfInputParameters.ItemLeftButtonDoubleClick += listOfInputParameters_MouseDoubleClick;
				listOfInputParameters.SelectionChanged += listOfInputParameters_SelectionChanged;
			}
			if (wrapSelectionButton != null)
				wrapSelectionButton.Click += wrapSelectionButton_Click;
			if (listOfInputTypes != null)
				listOfInputTypes.SelectionChanged += listOfInputTypes_SelectionChanged;
			if (functionsTypes != null)
				functionsTypes.SelectedIndexChanged += functionsTypes_SelectedIndexChanged;
			expressionTextEdit.SelectAllOnGotFocus = false;
			InitializeStandardOperationButtons();
			ExpressionMemoEdit = new MemoEditWrapper(expressionTextEdit);
			ListOfInputTypes = new ListBoxControlWrappper(listOfInputTypes);
			ListOfInputParameters = new ListBoxControlWrappper(listOfInputParameters);
			FunctionsTypes = new ComboBoxEditWrappper(functionsTypes);
			fEditorLogic = GetExpressionEditorLogic();
			fEditorLogic.Initialize();
			fEditorLogic.OnLoad();
#if DEBUGTEST
			TemplateApplied = true;
#endif
		}
		protected virtual ExpressionEditorLogic GetExpressionEditorLogic() {
			return new ExpressionEditorLogicEx(this, ColumnInfo);
		}
		void UnsubscribeStandardOperationButtons() {
			UnsubscribeClickEvent(buttonPlus);
			UnsubscribeClickEvent(buttonMinus);
			UnsubscribeClickEvent(buttonMultiply);
			UnsubscribeClickEvent(buttonDivide);
			UnsubscribeClickEvent(buttonModulo);
			UnsubscribeClickEvent(buttonEqual);
			UnsubscribeClickEvent(buttonNotEqual);
			UnsubscribeClickEvent(buttonLess);
			UnsubscribeClickEvent(buttonLessOrEqual);
			UnsubscribeClickEvent(buttonLargerOrEqual);
			UnsubscribeClickEvent(buttonLarger);
			UnsubscribeClickEvent(buttonAnd);
			UnsubscribeClickEvent(buttonOr);
			UnsubscribeClickEvent(buttonNot);
		}
		void InitializeStandardOperationButtons() {
			InitializeStandardOperationButton(buttonPlus, StandardOperations.Plus);
			InitializeStandardOperationButton(buttonMinus, StandardOperations.Minus);
			InitializeStandardOperationButton(buttonMultiply, StandardOperations.Multiply);
			InitializeStandardOperationButton(buttonDivide, StandardOperations.Divide);
			InitializeStandardOperationButton(buttonModulo, StandardOperations.Modulo);
			InitializeStandardOperationButton(buttonEqual, StandardOperations.Equal);
			InitializeStandardOperationButton(buttonNotEqual, StandardOperations.NotEqual);
			InitializeStandardOperationButton(buttonLess, StandardOperations.Less);
			InitializeStandardOperationButton(buttonLessOrEqual, StandardOperations.LessOrEqual);
			InitializeStandardOperationButton(buttonLargerOrEqual, StandardOperations.LargerOrEqual);
			InitializeStandardOperationButton(buttonLarger, StandardOperations.Larger);
			InitializeStandardOperationButton(buttonAnd, StandardOperations.And);
			InitializeStandardOperationButton(buttonOr, StandardOperations.Or);
			InitializeStandardOperationButton(buttonNot, StandardOperations.Not);
		}
		void UnsubscribeClickEvent(Button button) {
			if (button == null)
				return;
			button.Click -= insertOperationButton_Click;
		}
		void InitializeStandardOperationButton(Button button, string operation) {
			if (button == null)
				return;
			button.Click += insertOperationButton_Click;
			button.Tag = operation;
		}
		void ExpressionEditorControl_Loaded(object sender, RoutedEventArgs e) {
			if (fEditorLogic != null)
				fEditorLogic.OnLoad();
		}
		private void listOfInputTypes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			fEditorLogic.OnInputTypeChanged();
		}
		private void functionsTypes_SelectedIndexChanged(object sender, RoutedEventArgs e) {
			fEditorLogic.OnFunctionTypeChanged();
		}
		private void listOfInputParameters_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
			fEditorLogic.OnInputParametersChanged();
		}
		private void listOfInputParameters_MouseDoubleClick(object sender, EventArgs e) {
			fEditorLogic.OnInsertInputParameter();
		}
		private void insertOperationButton_Click(object sender, RoutedEventArgs e) {
			fEditorLogic.OnInsertOperation((string)((Button)sender).Tag);
		}
		private void wrapSelectionButton_Click(object sender, RoutedEventArgs e) {
			fEditorLogic.OnWrapExpression();
		}
		string GetResourceStringCore(string stringId) {
			string editorStringId = "ExpressionEditor_" + stringId.Replace(".", "_").Replace(" ", "_");
			string result = EditorLocalizer.GetString(editorStringId);
#if DEBUGTEST
			System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(result));
#endif
			return result;
		}
		#region IExpressionEditor Members
		ExpressionEditorLogic IExpressionEditor.EditorLogic { get { return fEditorLogic; } }
		IMemoEdit IExpressionEditor.ExpressionMemoEdit { get { return ExpressionMemoEdit; } }
		ISelector IExpressionEditor.ListOfInputParameters { get { return ListOfInputParameters; } }
		ISelector IExpressionEditor.ListOfInputTypes { get { return ListOfInputTypes; } }
		ISelector IExpressionEditor.FunctionsTypes { get { return FunctionsTypes; } }
		string IExpressionEditor.FilterCriteriaInvalidExpressionExMessage { get { return EditorLocalizer.GetString(EditorStringId.FilterCriteriaInvalidExpressionEx); } }
		string IExpressionEditor.FilterCriteriaInvalidExpressionMessage { get { return EditorLocalizer.GetString(EditorStringId.FilterCriteriaInvalidExpression); } }
		string IExpressionEditor.GetFunctionTypeStringID(string functionType) {
			return "FunctionType_" + functionType;
		}
		string IExpressionEditor.GetResourceString(string stringId) {
			return GetResourceStringCore(stringId);
		}
		void IExpressionEditor.ShowError(string error) {
			MessageBoxHelper.ShowError(error, EditorLocalizer.GetString(EditorStringId.CaptionError), MessageBoxButton.OK);
		}
		void IExpressionEditor.HideFunctionsTypes() {
			functionsTypes.Visibility = Visibility.Collapsed;
		}
		void IExpressionEditor.ShowFunctionsTypes() {
			functionsTypes.Visibility = Visibility.Visible;
		}
		void IExpressionEditor.SetDescription(string description) {
			descriptionEdit.Text = description;
		}
		#endregion
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return fEditorLogic.CanCloseWithOKResult();
		}
		void IDialogContent.OnOk() { }
		void IDialogContent.OnApply() { }
		#endregion
	}
}
namespace DevExpress.Xpf.Editors.ExpressionEditor.Native {
	public static class ExpressionEditorHelper {
#if SL
		public static DXDialog ShowExpressionEditor(Control expressionEditorControl, FrameworkElement rootElement, DialogClosedDelegate closedHandler) {
			Size expressionEditorSize = new Size(600, 450);
			if(ThemeHelper.IsTouchTheme(rootElement))
				expressionEditorSize = new Size(950, 750);
			return FloatingContainer.ShowDialogContent(expressionEditorControl, rootElement, expressionEditorSize, new FloatingContainerParameters() {
				ClosedDelegate = closedHandler,
				Title = EditorLocalizer.GetString(EditorStringId.ExpressionEditor_Title),
				AllowSizing = true,
				CloseOnEscape = true,
			});
		}
#else
		public static FloatingContainer ShowExpressionEditor(Control expressionEditorControl, FrameworkElement rootElement, DialogClosedDelegate closedHandler) {
			Size expressionEditorSize = new Size(600, 450);
			if(ThemeHelper.IsTouchTheme(rootElement))
				expressionEditorSize = new Size(950, 750);
			return FloatingContainer.ShowDialogContent(expressionEditorControl, rootElement, expressionEditorSize, new FloatingContainerParameters() {
				ClosedDelegate = closedHandler,
				Title = EditorLocalizer.GetString(EditorStringId.ExpressionEditor_Title),
				AllowSizing = true,
				CloseOnEscape = true,
				Icon = new BitmapImage(UriHelper.GetUri(XmlNamespaceConstants.UtilsNamespace, "/Editors/Images/ExpressionEditor/expression.png"))
			});
		}
#endif
	}
}
