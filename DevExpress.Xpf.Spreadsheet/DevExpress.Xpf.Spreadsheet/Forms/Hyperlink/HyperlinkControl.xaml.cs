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
using DevExpress.Office.Localization;
using DevExpress.Utils;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using Microsoft.Win32;
namespace DevExpress.Xpf.Spreadsheet.Forms {
	public class Bookmark {
		public Bookmark(string name) {
			Name = name;
			DefinedNames = new List<string>();
			CellReferences = new List<string>();
		}
		public string Name { get; private set; }
		public List<string> DefinedNames { get; private set; }
		public List<string> CellReferences { get; private set; }
		public void AddDefinedName(string name) {
			DefinedNames.Add(name);
		}
		public void AddCellReference(string name) {
			CellReferences.Add(name);
		}
	}
	public partial class HyperlinkControl : System.Windows.Controls.UserControl {
		#region Fields
		readonly HyperlinkFormController controller;
		readonly Dictionary<string, TreeViewItem> worksheetNames;
		readonly List<TreeViewItem> definedNames;
		readonly CellRange defaultRange;
		static HyperlinkType lastLinkTo = HyperlinkType.ExistingFileOrWebPage;
		readonly bool isNewHyperlink;
		bool isSynchonizeDisplayText;
		string nodeCellReferencesCaption;
		string nodeDefinedNameCaption;
		string lastFileOrWebPage;
		string lastCell;
		string lastSheet;
		string lastEmail;
		string lastSubject;
		HyperlinkType linkTo;
		NodeCategory nodeCategory;
		bool emailEdited;
		static int lastWidth = 622;
		static int lastHeight = 342;
		#endregion
		public static readonly DependencyProperty SelectedLinkProperty;
		static HyperlinkControl() {
			SelectedLinkProperty = DependencyProperty.Register("SelectedLink", typeof(HyperlinkType), typeof(HyperlinkControl),
				new FrameworkPropertyMetadata(HyperlinkType.ExistingFileOrWebPage, (d, e) => ((HyperlinkControl)d).OnSelectedLinkChanged()));
		}
		Action postponedAction;
		private void OnSelectedLinkChanged() {
			if (IsDataInitialized)
				OnSelectedLinkChangedCore();
			else
				postponedAction = new Action(OnSelectedLinkChangedCore);
		}
		private void OnSelectedLinkChangedCore() {
			LinkTo = SelectedLink;
			LastLinkTo = LinkTo;
			UpdatePanelsVisibility(LinkTo);
			UpdateOKButtonEnabling(LinkTo);
			FillCurrentPanel(LinkTo);
			Controller.IsExternal = LinkTo != HyperlinkType.PlaceInThisDocument;
			SynchonizeDisplayText();
		}
		public HyperlinkControl(HyperlinkFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controller = CreateController(controllerParameters);
			this.worksheetNames = new Dictionary<string, TreeViewItem>();
			this.definedNames = new List<TreeViewItem>();
			this.nodeCellReferencesCaption = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HyperlinkForm_NodeCellReferences);
			this.nodeDefinedNameCaption = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HyperlinkForm_NodeDefinedName);
			this.isNewHyperlink = String.IsNullOrEmpty(Controller.TargetUri);
			this.defaultRange = CreateDefaultRange();
			InitializeComponent();
			this.Loaded += HyperlinkControl_Loaded;
		}
		void HyperlinkControl_Loaded(object sender, RoutedEventArgs e) {
			if (IsDataInitialized)
				return;
			Dialog = LayoutHelper.FindParentObject<DXDialog>(this);
			InitializeData();
			InitializeForm();
			IsDataInitialized = true;
			SubscribeControlsEvents();
			SynchonizeDisplayText();
			if (postponedAction != null) {
				postponedAction.Invoke();
				postponedAction = null;
			}
		}
		public HyperlinkType SelectedLink {
			get { return (HyperlinkType)GetValue(SelectedLinkProperty); }
			set { SetValue(SelectedLinkProperty, value); }
		}
		#region Properties
		protected HyperlinkFormController Controller { get { return controller; } }
		protected DocumentModel Workbook { get { return controller.Hyperlink.Workbook; } }
		protected Dictionary<string, TreeViewItem> WorksheetNames { get { return worksheetNames; } }
		protected List<TreeViewItem> DefinedNames { get { return definedNames; } }
		protected bool IsNewHyperlink { get { return isNewHyperlink; } }
		protected HyperlinkType LinkTo { get { return linkTo; } set { linkTo = value; } }
		protected static HyperlinkType LastLinkTo { get { return lastLinkTo; } set { lastLinkTo = value; } }
		DXDialog Dialog { get; set; }
		bool IsDataInitialized { get; set; }
		#region DisplayTextEnabled
		protected bool DisplayTextEnabled {
			get {
				if (Controller.Hyperlink.IsDrawingObject)
					return false;
				ICellBase firstCell;
				if (Workbook.ActiveSheet.Selection.IsSingleCell) {
					CellPosition activeCellPosition = Workbook.ActiveSheet.Selection.ActiveCell;
					firstCell = Workbook.ActiveSheet.TryGetCell(activeCellPosition.Column, activeCellPosition.Row);
				}
				else
					firstCell = TryGetFirstCell();
				if (firstCell == null || firstCell.Value.IsText || firstCell.Value.IsEmpty)
					return true;
				return false;
			}
		}
		#endregion
		#endregion
		#region InitializeData
		void InitializeData() {
			isSynchonizeDisplayText = GetInitialIsSynchonizeDisplayText();
			nodeCategory = GetInitialNodeCategory();
			linkTo = GetInitialLinkTo();
			emailEdited = false;
			lastFileOrWebPage = String.Empty;
			lastCell = String.Empty;
			lastSheet = String.Empty;
			lastEmail = String.Empty;
			lastSubject = String.Empty;
			if (Controller.IsExternal) {
				if (Controller.TargetUri.Contains("mailto:"))
					SetEmailAndSubjectFromTargetUri();
				else
					lastFileOrWebPage = Controller.TargetUri;
			}
			SetCellAndSheetFromTargetUri();
		}
		NodeCategory GetInitialNodeCategory() {
			NodeCategory result = NodeCategory.WorksheetName;
			ParsedExpression expression = Controller.Hyperlink.Expression;
			if (expression != null && expression.Count == 1) {
				IParsedThing thing = expression[0];
				if (thing is ParsedThingName || thing is ParsedThingNameX)
					result = NodeCategory.DefinedName;
			}
			return result;
		}
		void SetCellAndSheetFromTargetUri() {
			CellRange range = defaultRange;
			if (!Controller.IsExternal && nodeCategory == NodeCategory.DefinedName) {
				lastCell = Controller.Hyperlink.TargetUri;
				return;
			}
			if (!Controller.IsExternal && nodeCategory == NodeCategory.WorksheetName) {
				CellRange localRange = Controller.Hyperlink.GetTargetRange() as CellRange;
				if (localRange != null)
					range = localRange;
			}
			lastCell = range.ToString(Controller.Hyperlink.Workbook.DataContext);
			lastSheet = range.Worksheet.Name;
		}
		void SetEmailAndSubjectFromTargetUri() {
			string targetUri = controller.TargetUri;
			if (targetUri.Contains("mailto:"))
				lastEmail = targetUri.Contains("?") ? targetUri.Remove(targetUri.IndexOf("?")) : targetUri;
			if (targetUri.Contains("="))
				lastSubject = targetUri.Remove(0, targetUri.IndexOf("=") + 1);
		}
		HyperlinkType GetInitialLinkTo() {
			if (!IsNewHyperlink)
				return GetLinkToCore(Controller.Hyperlink);
			return lastLinkTo;
		}
		HyperlinkType GetLinkToCore(IHyperlinkViewInfo hyperlink) {
			if (!hyperlink.IsExternal)
				return HyperlinkType.PlaceInThisDocument;
			if (hyperlink.TargetUri.Contains("mailto:"))
				return HyperlinkType.EmailAddress;
			return HyperlinkType.ExistingFileOrWebPage;
		}
		#endregion
		#region CreateDefaultRange
		CellRange CreateDefaultRange() {
			CellPosition A1Position;
			if (Controller.Hyperlink.Workbook.DataContext.UseR1C1ReferenceStyle)
				A1Position = new CellPosition(0, 0, PositionType.Absolute, PositionType.Absolute);
			else
				A1Position = new CellPosition(0, 0);
			return new CellRange(Controller.Hyperlink.Workbook.ActiveSheet, A1Position, A1Position);
		}
		#endregion
		#region GetDefaultRange
		string GetDefaultRange() {
			return defaultRange.ToString(Controller.Hyperlink.Workbook.DataContext);
		}
		#endregion
		#region TryGetFirstCell
		Cell TryGetFirstCell() {
			CellPosition topLeft = Workbook.ActiveSheet.Selection.ActiveRange.TopLeft;
			return (Cell)Workbook.ActiveSheet.TryGetCell(topLeft.Column, topLeft.Row);
		}
		#endregion
		#region GetIsSynchonizeDisplayText
		bool GetInitialIsSynchonizeDisplayText() {
			if (Controller.Hyperlink.IsDrawingObject)
				return false;
			Cell firstCell = TryGetFirstCell();
			return (firstCell == null || firstCell.Value.IsEmpty) || (firstCell.Value.IsText && (Controller.DisplayText == Controller.TargetUri) && (Controller.DisplayText == firstCell.Text));
		}
		#endregion
		#region CreateController
		protected internal virtual HyperlinkFormController CreateController(HyperlinkFormControllerParameters controllerParameters) {
			return new HyperlinkFormController(controllerParameters);
		}
		#endregion
		#region SubscribeControlsEvents
		protected internal virtual void SubscribeControlsEvents() {
			teDisplayText.EditValueChanged += TeDisplayText_EditValueChanged;
			emailAddress.EmailTextEdit.EditValueChanging += TeEmail_EditValueChanging;
			emailAddress.SubjectTextEdit.EditValueChanging += TeSubject_EditValueChanging;
			teTooltip.EditValueChanged += TeTooltip_EditValueChanged;
			externalAddress.TextEdit.EditValueChanged += BtnEditAddress_EditValueChanged;
			externalAddress.TextEdit.EditValueChanging += BtnEditAddress_EditValueChanging;
			externalAddress.TextEdit.DefaultButtonClick += BtnEditAddress_ButtonClick;
			cellReferenceAddress.SelectedItemChanged += TlCellReference_FocusedNodeChanged;
			cellReferenceAddress.TextEdit.EditValueChanged += TeCellReference_EditValueChanged;
			Dialog.Closing += Dialog_Closing;
			this.SizeChanged += HyperlinkForm_SizeChanged;
		}
		void Dialog_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			if (Dialog.DialogResult == true) {
				if (!ApplyChanges()) {
					if (DisplayTextEnabled) {
						teDisplayText.EditValueChanged -= TeDisplayText_EditValueChanged;
						teDisplayText.EditValue = String.Empty;
						teDisplayText.EditValueChanged += TeDisplayText_EditValueChanged;
					}
				}
				else
					btnOk_ClickCore();
			}
		}
		#endregion
		#region UnsubscribeControlsEvents
		protected internal virtual void UnsubscribeControlsEvents() {
			teDisplayText.EditValueChanged -= TeDisplayText_EditValueChanged;
			emailAddress.EmailTextEdit.EditValueChanging -= TeEmail_EditValueChanging;
			emailAddress.SubjectTextEdit.EditValueChanging -= TeSubject_EditValueChanging;
			externalAddress.TextEdit.EditValueChanged -= BtnEditAddress_EditValueChanged;
			externalAddress.TextEdit.EditValueChanging -= BtnEditAddress_EditValueChanging;
			externalAddress.TextEdit.DefaultButtonClick -= BtnEditAddress_ButtonClick;
			cellReferenceAddress.SelectedItemChanged -= TlCellReference_FocusedNodeChanged;
			cellReferenceAddress.TextEdit.EditValueChanged -= TeCellReference_EditValueChanged;
			Dialog.Closing -= Dialog_Closing;
			this.SizeChanged -= HyperlinkForm_SizeChanged;
		}
		#endregion
		#region InitializeForm
		void InitializeForm() {
			PopulateBookmark();
			UpdatePanelsVisibility(LinkTo);
			FillCurrentPanel(LinkTo);
			nbgLinkTo.SelectedItemIndex = (int)LinkTo;
			LastLinkTo = LinkTo;
			UpdateOKButtonEnabling(LinkTo);
			InitializeDisplayText();
			teTooltip.Text = Controller.TooltipText;
			this.Width = lastWidth;
			this.Height = lastHeight;
			this.Dialog.Title = (isNewHyperlink ? OfficeLocalizer.GetString(OfficeStringId.Caption_InsertHyperlinkForm) : OfficeLocalizer.GetString(OfficeStringId.Caption_EditHyperlinkForm));
		}
		#endregion
		#region UpdateDisplayText
		void InitializeDisplayText() {
			if (!DisplayTextEnabled) {
				teDisplayText.EditValue = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.HyperlinkForm_DisabledDisplayText);
				teDisplayText.IsEnabled = false;
			}
			else {
				Cell firstCell = TryGetFirstCell();
				teDisplayText.EditValue = (firstCell != null && !firstCell.Value.IsEmpty ? firstCell.Text : Controller.TargetUri);
				Controller.DisplayText = teDisplayText.EditValue.ToString();
			}
		}
		#endregion
		#region PopulateBookmark
		void PopulateBookmark() {
			PopulateColumns();
			PopulateRows();
			cellReferenceAddress.TreeViewExpandAll();
			if (linkTo == Forms.HyperlinkType.PlaceInThisDocument)
				FocusPlaceInThisDocumentNode();
		}
		#endregion
		#region PopulateRows
		void PopulateRows() {
			try {
				TreeViewItem nodeCellReferences = cellReferenceAddress.AddRootNode(nodeCellReferencesCaption);
				TreeViewItem nodeDefinedName = cellReferenceAddress.AddRootNode(nodeDefinedNameCaption);
				PopulateWorksheetNames(nodeCellReferences);
				PopulateDefinedNameFromWorksheets(nodeDefinedName);
				PopulateDefinedNameFromWorkbook(nodeDefinedName);
			}
			finally {
			}
		}
		#endregion
		#region PopulateColumns
		void PopulateColumns() {
		}
		#endregion
		#region PopulateDefinedNameFromWorkbook
		void PopulateDefinedNameFromWorkbook(TreeViewItem rootNode) {
			foreach (DefinedNameBase name in Workbook.DefinedNames) {
				TreeViewItem node = cellReferenceAddress.TreeViewAppendNode(name.Name, rootNode);
				DefinedNames.Add(node);
			}
		}
		#endregion
		#region PopulateDefinedNameFromWorksheets
		void PopulateDefinedNameFromWorksheets(TreeViewItem rootNode) {
			foreach (Worksheet sheet in Workbook.Sheets)
				foreach (DefinedNameBase name in sheet.DefinedNames) {
					string nodeName = sheet.Name + "!" + name.Name;
					TreeViewItem node = cellReferenceAddress.TreeViewAppendNode(nodeName, rootNode);
					DefinedNames.Add(node);
				}
		}
		#endregion
		#region PopulateWorksheetNames
		void PopulateWorksheetNames(TreeViewItem rootNode) {
			foreach (Worksheet sheet in Workbook.Sheets) {
				TreeViewItem node = cellReferenceAddress.TreeViewAppendNode(sheet.Name, rootNode);
				WorksheetNames.Add(sheet.Name, node);
			}
		}
		#endregion
		#region UpdateTeCellReferenceEnabling
		void UpdateTeCellReferenceEnabling(NodeCategory nodeCategory) {
		}
		#endregion
		#region FocusPlaceInThisDocumentNode
		void FocusPlaceInThisDocumentNode() {
			TreeViewItem focusedNode = null;
			switch (nodeCategory) {
				case NodeCategory.DefinedName:
					focusedNode = GetExistingDefinedNameNode();
					break;
				case NodeCategory.WorksheetName:
					focusedNode = WorksheetNames[lastSheet];
					break;
				default:
					throw new ArgumentException("Invalid NodeCategory:" + nodeCategory.ToString());
			}
			if (focusedNode != null)
				cellReferenceAddress.TreeListFocusedNode = focusedNode;
		}
		TreeViewItem GetExistingDefinedNameNode() {
			foreach (TreeViewItem node in DefinedNames)
				if (node.Header.ToString() == lastCell)
					return node;
			return null;
		}
		#endregion
		#region NbLinkTo_LinkClicked
		#endregion
		#region GetDisplayText
		string GetDisplayText(HyperlinkType linkTo) {
			switch (linkTo) {
				case HyperlinkType.ExistingFileOrWebPage:
					return externalAddress.Text;
				case HyperlinkType.PlaceInThisDocument:
					return GetDisplayTextFromPlaceInThisDocument();
				default:
					return GetDisplayTextFromEmail();
			}
		}
		string GetDisplayTextFromEmail() {
			String result = emailAddress.EmailText;
			if (!String.IsNullOrEmpty(emailAddress.SubjectText))
				result += "?subject=" + emailAddress.SubjectText;
			return result;
		}
		string GetDisplayTextFromPlaceInThisDocument() {
			switch (nodeCategory) {
				case NodeCategory.Root:
					return cellReferenceAddress.Text;
				case NodeCategory.WorksheetName:
					return GetDisplayTextFromPlaceInThisDocumentForSheet(lastSheet, cellReferenceAddress.Text);
				default:
					return cellReferenceAddress.TreeListFocusedNode.Header.ToString();
			}
		}
		string GetDisplayTextFromPlaceInThisDocumentForSheet(string sheetName, string cellReference) {
			if (string.IsNullOrEmpty(cellReference))
				return sheetName;
			WorkbookDataContext context = Controller.Hyperlink.Workbook.DataContext;
			ParsedExpression expression = HyperlinkExpressionParser.ValidateCellRefExpression(context, lastSheet, cellReference, false);
			if (expression != null)
				return expression.BuildExpressionString(context);
			else {
				SheetDefinition sheetDefinition = new SheetDefinition();
				sheetDefinition.SheetNameStart = sheetName;
				return sheetDefinition.ToString(context) + cellReference;
			}
		}
		#endregion
		#region FillCurrentPanel
		void FillCurrentPanel(HyperlinkType linkTo) {
			switch (linkTo) {
				case HyperlinkType.ExistingFileOrWebPage:
					FillExistingFileOrWebPagePanel();
					break;
				case HyperlinkType.PlaceInThisDocument:
					FillPlaceInThisDocumentPanel();
					break;
				default:
					FillEmailPanel();
					break;
			}
		}
		void FillExistingFileOrWebPagePanel() {
			externalAddress.Text = lastFileOrWebPage;
			UpdateCursorPosition(externalAddress.TextEdit, externalAddress.TextEditSelectionStart);
		}
		void FillPlaceInThisDocumentPanel() {
			cellReferenceAddress.Text = lastCell;
			UpdateTeCellReferenceEnabling(nodeCategory);
			FocusPlaceInThisDocumentNode();
			UpdateCursorPosition(cellReferenceAddress.TextEdit, cellReferenceAddress.TextEditSelectionStart);
		}
		void FillEmailPanel() {
			emailAddress.SubjectText = lastSubject;
			emailAddress.EmailText = lastEmail;
			UpdateCursorPosition(emailAddress.EmailTextEdit, emailAddress.EmailSelectionStart);
		}
		#endregion
		#region CreateTargetUri
		string CreateTargetUri() {
			switch (linkTo) {
				case HyperlinkType.ExistingFileOrWebPage:
					return CreateExistingFileOrWebPageTargetUri();
				case HyperlinkType.PlaceInThisDocument:
					return CreatePlaceInThisDocumentTargetUri();
				case HyperlinkType.EmailAddress:
					return CreateEmailTargetUri();
				default:
					throw new ArgumentException("Unknown LinkTo:" + LinkTo.ToString());
			}
		}
		string CreatePlaceInThisDocumentTargetUri() {
			switch (nodeCategory) {
				case NodeCategory.Root:
					return null;
				case NodeCategory.WorksheetName:
					return CreatePlaceInThisDocumentTargetUriForSheet(lastSheet, lastCell);
				default:
					return cellReferenceAddress.TreeListFocusedNode.Header.ToString();
			}
		}
		string CreatePlaceInThisDocumentTargetUriForSheet(string sheetName, string cellReference) {
			WorkbookDataContext context = Controller.Hyperlink.Workbook.DataContext;
			if (string.IsNullOrEmpty(cellReference)) {
				SheetDefinition sheetDefinition = new SheetDefinition();
				sheetDefinition.SheetNameStart = sheetName;
				return sheetDefinition.ToString(context) + GetDefaultRange();
			}
			ParsedExpression expression = HyperlinkExpressionParser.ValidateCellRefExpression(context, lastSheet, cellReference, true);
			if (expression == null)
				return null;
			return expression.BuildExpressionString(context);
		}
		string CreateEmailTargetUri() {
			String result = lastEmail;
			if (!String.IsNullOrEmpty(lastSubject))
				result += "?subject=" + lastSubject;
			return result;
		}
		string CreateExistingFileOrWebPageTargetUri() {
			return lastFileOrWebPage.Trim();
		}
		#endregion
		#region UpdatePanelsVisibility
		void UpdatePanelsVisibility(HyperlinkType linkTo) {
		}
		#endregion
		#region OnTxtTextEditValueChanged
		void TeDisplayText_EditValueChanged(object sender, EventArgs e) {
			isSynchonizeDisplayText = false;
			if (teDisplayText.EditValue != null && !String.IsNullOrEmpty(teDisplayText.EditValue.ToString()) || (IsNewHyperlink && Controller.IsExternal))
				Controller.DisplayText = teDisplayText.EditValue.ToString();
		}
		#endregion
		#region TeEmail_EditValueChanging
		void TeEmail_EditValueChanging(object sender, EditValueChangingEventArgs e) {
			TeEmailChangeText();
			SynchonizeDisplayText();
			UpdateOKButtonEnabling(HyperlinkType.EmailAddress);
			UpdateCursorPosition(emailAddress.EmailTextEdit, emailEdited && emailAddress.EmailSelectionStart != 0 ? emailAddress.EmailSelectionStart : emailAddress.EmailText.Length);
			emailEdited = true;
		}
		#endregion
		#region UpdateCursorPosition
		void UpdateCursorPosition(TextEdit textEdit, int selectionStart) {
			UpdateFocusAndSelectionLength(textEdit);
			textEdit.SelectionStart = selectionStart;
		}
		#endregion
		#region TeSubject_EditValueChanging
		void TeSubject_EditValueChanging(object sender, EditValueChangingEventArgs e) {
			lastSubject = emailAddress.SubjectText;
			if (!String.IsNullOrEmpty(emailAddress.EmailText)) {
				SynchonizeDisplayText();
				UpdateOKButtonEnabling(HyperlinkType.EmailAddress);
			}
		}
		#endregion
		#region TeEmailChangeText
		void TeEmailChangeText() {
			try {
				emailAddress.EmailTextEdit.EditValueChanging -= TeEmail_EditValueChanging;
				bool isAddedText = emailAddress.EmailText.Length > lastEmail.Length;
				bool isDeletedFromFirstMailTo = lastEmail.StartsWith("mailto:") && lastEmail != "mailto:";
				if (!emailAddress.EmailText.StartsWith("mailto:") && (isAddedText || (isDeletedFromFirstMailTo && emailAddress.EmailText != String.Empty)))
					emailAddress.EmailText = "mailto:" + emailAddress.EmailText;
				lastEmail = emailAddress.EmailText;
			}
			finally {
				emailAddress.EmailTextEdit.EditValueChanging += TeEmail_EditValueChanging;
			}
		}
		#endregion
		#region OnTxtTooltipEditValueChanged
		void TeTooltip_EditValueChanged(object sender, EventArgs e) {
			Controller.TooltipText = teTooltip.Text;
		}
		#endregion
		#region BtnEditAddress_EditValueChanging
		void BtnEditAddress_EditValueChanging(object sender, EditValueChangingEventArgs e) {
			string uri = ((string)e.NewValue).ToLower();
			if (HyperlinkExpressionParser.IsWebPage(uri))
				Dialog.OkButton.IsEnabled = HyperlinkExpressionParser.IsValidateWebPageUri(uri);
			else
				Dialog.OkButton.IsEnabled = HyperlinkExpressionParser.IsValidateFileUri(uri);
		}
		#endregion
		#region OnBtnEditAddress_EditValueChanged
		void BtnEditAddress_EditValueChanged(object sender, Editors.EditValueChangedEventArgs e) {
			string address = externalAddress.Text;
			SynchonizeDisplayText();
			lastFileOrWebPage = address;
		}
		#endregion
		#region UpdateOKButtonEnabling
		void UpdateOKButtonEnabling(HyperlinkType linkTo) {
			if (linkTo == HyperlinkType.PlaceInThisDocument)
				Dialog.OkButton.IsEnabled = (nodeCategory != NodeCategory.Root);
			else if (linkTo == HyperlinkType.ExistingFileOrWebPage)
				Dialog.OkButton.IsEnabled = !String.IsNullOrEmpty(externalAddress.Text);
			else
				Dialog.OkButton.IsEnabled = !String.IsNullOrEmpty(emailAddress.EmailText);
		}
		#endregion
		#region btnCancel_Click
		void btnCancel_Click(object sender, EventArgs e) {
		}
		#endregion
		#region btnOk_Click
		void btnOk_Click(object sender, EventArgs e) {
			if (!ApplyChanges()) {
				if (DisplayTextEnabled) {
					teDisplayText.EditValueChanged -= TeDisplayText_EditValueChanged;
					teDisplayText.EditValue = String.Empty;
					teDisplayText.EditValueChanged += TeDisplayText_EditValueChanged;
				}
			}
			else
				btnOk_ClickCore();
		}
		#endregion
		#region ApplyChanges
		bool ApplyChanges() {
			string targetUri = CreateTargetUri();
			if (string.IsNullOrEmpty(targetUri)) {
				Controller.Control.ShowWarningMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidReference));
				return false;
			}
			controller.TargetUri = targetUri;
			if (isSynchonizeDisplayText)
				controller.DisplayText = targetUri;
			return true;
		}
		#endregion
		#region btnOk_ClickCore
		void btnOk_ClickCore() {
			controller.ApplyChanges();
		}
		#endregion
		#region OnBtnEditAddress_ButtonClick
		void BtnEditAddress_ButtonClick(object sender, RoutedEventArgs e) {
			OpenFileDialog fileDialog = new OpenFileDialog();
			string description = XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.FileFilterDescription_AllFiles);
			fileDialog.Filter = String.Format("{0}|*.*", description);
			if (fileDialog.ShowDialog().Value == true)
				externalAddress.Text = fileDialog.FileName;
		}
		#endregion
		#region PrepareTargetUri
		string PrepareTargetUri() {
			if (LinkTo == HyperlinkType.PlaceInThisDocument)
				return PrepareTargetUriPlaceInThisDocument(cellReferenceAddress.Text);
			throw new NotImplementedException();
		}
		string PrepareTargetUriPlaceInThisDocument(string cellReference) {
			if (!String.IsNullOrEmpty(cellReference)) {
				WorkbookDataContext context = Controller.Hyperlink.Workbook.DataContext;
				ParsedExpression expression = HyperlinkExpressionParser.ValidateCellRefExpression(context, lastSheet, cellReference, true);
				if (expression != null)
					return expression.BuildExpressionString(context);
			}
			return cellReference;
		}
		#endregion
		#region SynchonizeDisplayText
		void SynchonizeDisplayText() {
			if (isSynchonizeDisplayText && DisplayTextEnabled) {
				string displayText = GetDisplayText(LinkTo);
				teDisplayText.EditValueChanged -= TeDisplayText_EditValueChanged;
				Controller.DisplayText = displayText;
				teDisplayText.EditValue = displayText;
				teDisplayText.EditValueChanged += TeDisplayText_EditValueChanged;
			}
		}
		#endregion
		#region IsNeedAddCell
		bool IsNeedAddCell() {
			return LinkTo == HyperlinkType.PlaceInThisDocument && cellReferenceAddress.TextEditEnabled && String.IsNullOrEmpty(cellReferenceAddress.Text);
		}
		#endregion
		#region TlCellReference_FocusedNodeChanged
		void TlCellReference_FocusedNodeChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
			nodeCategory = GetNodeCategory();
			UpdatePlaceInThisDocumentTargetUriCore();
			UpdateOKButtonEnabling(HyperlinkType.PlaceInThisDocument);
			SynchonizeDisplayText();
		}
		#region GetNodeCategory
		NodeCategory GetNodeCategory() {
			TreeViewItem focusedNode = cellReferenceAddress.TreeListFocusedNode;
			int level = cellReferenceAddress.GetItemLevel(cellReferenceAddress.TreeListFocusedNode);
			if (level == 0)
				return NodeCategory.Root;
			if (DefinedNames.Contains(focusedNode))
				return NodeCategory.DefinedName;
			return NodeCategory.WorksheetName;
		}
		#endregion
		void UpdatePlaceInThisDocumentTargetUriCore() {
			UpdateTeCellReferenceEnabling(nodeCategory);
			lastCell = (!String.IsNullOrEmpty(cellReferenceAddress.Text) ? cellReferenceAddress.Text : GetDefaultRange());
			lastSheet = (nodeCategory == NodeCategory.WorksheetName ? cellReferenceAddress.TreeListFocusedNode.Header.ToString() : lastSheet);
			Controller.IsExternal = false;
		}
		#endregion
		#region TeCellReference_EditValueChanged
		void TeCellReference_EditValueChanged(object sender, EventArgs e) {
			if (String.IsNullOrEmpty(cellReferenceAddress.Text))
				lastCell = GetDefaultRange();
			else
				lastCell = cellReferenceAddress.Text;
			SynchonizeDisplayText();
		}
		#endregion
		#region HyperlinkForm_Shown
		void HyperlinkForm_Shown(object sender, EventArgs e) {
			switch (LinkTo) {
				case HyperlinkType.ExistingFileOrWebPage:
					UpdateFocusAndSelectionLength(externalAddress.TextEdit);
					break;
				case HyperlinkType.PlaceInThisDocument:
					UpdateTeCellReferenceCursorPosition();
					break;
				case HyperlinkType.EmailAddress:
					UpdateTeEmailCursorPosition();
					break;
				default:
					throw new ArgumentException("Unknown LinkTo:" + LinkTo.ToString());
			}
		}
		#endregion
		#region UpdateTeEmailCursorPosition
		void UpdateTeEmailCursorPosition() {
			if (String.IsNullOrEmpty(emailAddress.SubjectText))
				UpdateFocusAndSelectionLength(emailAddress.EmailTextEdit);
			else {
				UpdateFocusAndSelectionLength(emailAddress.SubjectTextEdit);
				emailAddress.SubjectSelectionStart = emailAddress.SubjectText.Length;
			}
		}
		#endregion
		#region UpdateTeCellReferenceCursorPosition
		void UpdateTeCellReferenceCursorPosition() {
			if (cellReferenceAddress.TextEditEnabled)
				UpdateFocusAndSelectionLength(cellReferenceAddress.TextEdit);
			else
				cellReferenceAddress.TreeViewFocus();
		}
		#endregion
		#region UpdateFocusAndSelectionLength
		void UpdateFocusAndSelectionLength(TextEdit textEdit) {
			textEdit.Focus();
			textEdit.SelectionLength = 0;
		}
		#endregion
		#region HyperlinkForm_SizeChanged
		void HyperlinkForm_SizeChanged(object sender, EventArgs e) {
			lastWidth = (int)Width;
			lastHeight = (int)Height;
		}
		#endregion
		enum NodeCategory {
			Root,
			WorksheetName,
			DefinedName
		}
	}
	public enum HyperlinkType {
		ExistingFileOrWebPage = 0,
		PlaceInThisDocument = 1,
		EmailAddress = 2
	}
}
