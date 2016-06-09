#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using mshtml;
namespace DevExpress.ExpressApp.HtmlPropertyEditor.Win {
	[System.ComponentModel.ToolboxItem(false)]
	public partial class HtmlEditor : UserControl, IXtraResizableControl {
		private List<string> selectionDependentCommands;
		private bool isUpdating = false;
		private EventHandler SizeChangedHandler;
		private bool readOnly;
		private bool isFirstAssignment = true;
		private bool showComboboxCaptions;
		private bool enableKeyUp;
		private string headerInnerHtml;
		private IHTMLSelectionObject prevSelection;
		private const string TextSelectionType = "Text";
		private const string ControlSelectionType = "Control";
		private Dictionary<string, string> localizedValues = new Dictionary<string, string>();
		private void InitButtonsImages() {
			this.Undo.Glyph = ImageLoader.Instance.GetImageInfo("Action_Cancel").Image;
			this.Undo.GlyphDisabled = ImageLoader.Instance.GetImageInfo("Action_Cancel", false).Image;
			this.Redo.Glyph = ImageLoader.Instance.GetImageInfo("Action_Redo").Image;
			this.Redo.GlyphDisabled = ImageLoader.Instance.GetImageInfo("Action_Redo", false).Image;
		}
		private void Memo_TextChanged(object sender, EventArgs e) {
			OnModified();
		}
		private bool IsCommandEnabled(string command) {
			if(selectionDependentCommands.Contains(command)) {
				if(TextSelectionRange != null) {
					return TextSelectionRange.queryCommandEnabled(command);
				}
				else if(ControlSelectionRange != null) {
					return ControlSelectionRange.queryCommandEnabled(command);
				}
			}
			return HTMLDocument2.queryCommandEnabled(command);
		}
		internal void UpdateButtonState() {
			if(HTMLDocument2 != null) {
				try {
					BarManager.BeginUpdate();
					foreach(BarItem item in BarManager.Items) {
						if(item.Tag != null) {
							string command = item.Tag.ToString();
							if(item is BarButtonItem) {
								BarButtonItem barButtonItem = (BarButtonItem)item;
								barButtonItem.Enabled = IsCommandEnabled(command);
								barButtonItem.Down = HTMLDocument2.queryCommandState(command);
							}
							if(item is BarEditItem) {
								BarEditItem barEditItem = (BarEditItem)item;
								isUpdating = true;
								try {
									object editValue = HTMLDocument2.queryCommandValue(command);
									if(editValue != null && editValue is string) {
										foreach(KeyValuePair<string, string> localizedValue in localizedValues) {
											if(localizedValue.Value == (string)editValue) {
												editValue = localizedValue.Key;
											}
										}
									}
									barEditItem.EditValue = editValue;
								}
								finally {
									isUpdating = false;
								}
							}
						}
					}
				}
				finally {
					BarManager.EndUpdate();
				}
			}
		}
		private void OnSelectionChanges(object sender, EventArgs e) {
			UpdateButtonState();
		}
		#region Magic
		private void OnCut(object sender, EventArgs e) {
			if(Selection != null) {
				enableKeyUp = true;
			}
		}
		private void OnPaste(object sender, EventArgs e) {
			enableKeyUp = true;
		}
		private void OnKeyPress(object sender, EventArgs e) {
			enableKeyUp = true;
		}
		private void OnKeyUp(object sender, EventArgs e) {
			try {
				if(enableKeyUp) {
					OnModified();
				}
			}
			finally {
				enableKeyUp = false;
			}
		}
		private void browserControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
			enableKeyUp = e.KeyData == Keys.Back || e.KeyData == Keys.Delete;
		}
		#endregion
		private void Execute(string command, bool showUI, object parameters) {
			SaveSelection();
			browserControl.Document.ExecCommand(command, showUI, parameters);
			OnModified();
			UpdateButtonState();
			RestoreSelection();
		}
		private void BarButtonItem_ItemClick(object sender, ItemClickEventArgs e) {
			Execute(e.Item.Tag.ToString(), false, null);
		}
		private void FontSelector_EditValueChanged(object sender, EventArgs e) {
			BarEditItem control = sender as BarEditItem;
			if(control != null && !isUpdating) {
				string editValue = control.EditValue.ToString();
				if(localizedValues.ContainsKey(editValue)) {
					editValue = localizedValues[editValue];
				}
				Execute(control.Tag.ToString(), false, editValue);
			}
		}
		private void InsertImage_ItemClick(object sender, ItemClickEventArgs e) {
			Execute(e.Item.Tag.ToString(), true, null);
		}
		private void SetBarsVisible(bool visible) {
			this.bar2.Visible = visible;
			this.bar1.Visible = visible;
		}
		private void XtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			XtraTabControl xtraTabControl = sender as XtraTabControl;
			if(xtraTabControl != null && xtraTabControl.SelectedTabPageIndex == 1) {
				SetBarsVisible(false);
				if(DocumentInnerHtml != null) {
					memo.Text = DocumentInnerHtml;
				}
			}
			else {
				SetBarsVisible(!ReadOnly);
				DocumentInnerHtml = memo.Text;
			}
			FocusSelectedTabPageEditor(xtraTabControl);
		}
		private void XtraTabControl1_GotFocus(object sender, EventArgs e) {
			FocusSelectedTabPageEditor(sender as XtraTabControl);
		}
		private void ColorPopupControl_ColorChanged(object sender, EventArgs e) {
			ColorPopupControl control = (ColorPopupControl)sender;
			string arg = ColorTranslator.ToHtml(control.ResultColor);
			Execute(control.Tag.ToString(), false, arg);
		}
		private void Forecolor_ItemClick(object sender, ItemClickEventArgs e) {
			((BarButtonItemLink)e.Link).ShowPopup();
		}
		private void FocusSelectedTabPageEditor(XtraTabControl tabControl) {
			if(tabControl != null) {
				tabControl.SelectedTabPage.Controls[0].Focus();
			}
		}
		private void UpdateReadOnly() {
			if(HTMLDocument2 != null) {
				HTMLDocument2.designMode = ReadOnly ? "Off" : "On";
				if(ReadOnly) {
					RecreateBody(headerInnerHtml);
					DetachBodyEvents();
				}
			}
			Memo.Properties.ReadOnly = ReadOnly;
			if(BarManager != null) {
				try {
					BarManager.BeginUpdate();
					SetBarsVisible(!ReadOnly && tabControl.SelectedTabPageIndex != 1);
				}
				finally {
					BarManager.EndUpdate();
				}
			}
		}
		[System.Security.SecuritySafeCritical]
		internal void LockUndo() {
			IOleUndoManager undoManager = null;
			IServiceProvider serviceProvider = browserControl.Document.DomDocument as IServiceProvider;
			Guid undoManagerGuid = typeof(IOleUndoManager).GUID;
			IntPtr undoManagerPtr = IntPtr.Zero;
			int result = serviceProvider.QueryService(ref undoManagerGuid, ref undoManagerGuid, out undoManagerPtr);
			if(result == 0 && undoManagerPtr != IntPtr.Zero) {
				undoManager = (IOleUndoManager)Marshal.GetObjectForIUnknown(undoManagerPtr);
				Marshal.Release(undoManagerPtr);
			}
			undoManager.Enable(0);
			undoManager.Enable(1);
		}
		private void Localize() {
			this.Bold.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.BoldHint);
			this.Italic.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.ItalicHint);
			this.Underline.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.UnderlineHint);
			this.JustifyLeft.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.JustifyLeftHint);
			this.JustifyCenter.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.JustifyCenterHint);
			this.JustifyRight.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.JustifyRightHint);
			this.JustifyFull.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.JustifyFullHint);
			this.Forecolor.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.ForecolorHint);
			this.backColor.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.BackcolorHint);
			this.FontSelector.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.FontHint);
			this.FontSize.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.FontSizeHint);
			this.Styles.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.StylesHint);
			this.Cut.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.CutHint);
			this.Copy.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.CopyHint);
			this.Paste.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.PasteHint);
			this.Undo.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.UndoHint);
			this.Redo.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.RedoHint);
			this.RemoveFormat.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.RemoveFormatingHint);
			this.Subscript.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.SubscriptHint);
			this.Superscript.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.SuperscriptHint);
			this.InsertOrderedList.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.InsertOrderedListHint);
			this.InsertUnorderedList.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.InsertUnorderedListHint);
			this.Indent.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.IndentHint);
			this.Outdent.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.OutdentHint);
			this.CreateLink.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.InsertHyperlinkHint);
			this.Unlink.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.RemoveHyperlinkHint);
			this.StrikeThrough.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.StrikeoutHint);
			this.InsertImage.Hint = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.InsertImageHint);
			this.htmlPage.Text = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.TabHtmlCaption);
			this.designPage.Text = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.TabDesignCaption);
			LocalizeStyles();
		}
		private void LocalizeStyles() {
			this.repositoryItemStyles.Items.Clear();
			string styleNormalCaption = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.StyleNormalCaption);
			this.localizedValues[styleNormalCaption] = "Normal";
			this.repositoryItemStyles.Items.Add(styleNormalCaption);
			for(int headingLevel = 1; headingLevel <= 6; headingLevel++) {
				string styleHeadingCaption = string.Format("{0} {1}", HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.StyleHeadingCaption), headingLevel);
				this.localizedValues[styleHeadingCaption] = "Heading " + headingLevel;
				this.repositoryItemStyles.Items.Add(styleHeadingCaption);
			}
			string styleAddressCaption = HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.StyleAddressCaption);
			this.localizedValues[styleAddressCaption] = "Address";
			this.repositoryItemStyles.Items.Add(styleAddressCaption);
		}
		private void SaveSelection() {
			prevSelection = HTMLDocument2 != null ? Selection : null;
		}
		private IHTMLSelectionObject Selection {
			get {
				return HTMLDocument2.selection;
			}
		}
		private IHTMLControlRange ControlSelectionRange {
			get {
				return Selection.type == ControlSelectionType ? (IHTMLControlRange)Selection.createRange() : null;
			}
		}
		private IHTMLTxtRange TextSelectionRange {
			get {
				return Selection.type == TextSelectionType ? (IHTMLTxtRange)Selection.createRange() : null;
			}
		}
		private void RestoreSelection() {
			if(prevSelection.type == TextSelectionType) {
				RestoreTextSelection();
			}
			else if(prevSelection.type == ControlSelectionType) {
				RestoreControlSelection();
			}
		}
		private void RestoreTextSelection() {
			if(HTMLDocument2 != null && prevSelection != null) {
				TextSelectionRange.setEndPoint("StartToEnd", (IHTMLTxtRange)prevSelection.createRange());
				TextSelectionRange.select();
			}
		}
		private void RestoreControlSelection() {
			if(HTMLDocument2 != null && prevSelection != null) {
				IHTMLControlRange prevSelectionRange = (IHTMLControlRange)prevSelection.createRange();
				for(int i = 0; i < prevSelectionRange.length; i++) {
					IHTMLElement htmlElement = prevSelectionRange.item(i);
					ControlSelectionRange.add((IHTMLControlElement)htmlElement);
				}
				ControlSelectionRange.select();
			}
		}
		private void ChangeComboboxesCaptionsVisibility() {
			this.FontSelector.Caption = showComboboxCaptions ? HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.FontCaption) : "";
			this.FontSize.Caption = showComboboxCaptions ? HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.FontSizeCaption) : "";
			this.Styles.Caption = showComboboxCaptions ? HtmlEditorControlLocalizer.Active.GetLocalizedString(HtmlEditorControlId.StylesCaption) : "";
		}
		private void UpdateCurrentValue(string value) {
			if(DocumentInnerHtml != value) {
				DocumentInnerHtml = value;
				if(isFirstAssignment) {
					LockUndo();
					isFirstAssignment = false;
				}
			}
			memo.TextChanged -= new EventHandler(Memo_TextChanged);
			memo.Text = value;
			memo.TextChanged += new EventHandler(Memo_TextChanged);
		}
		private void AttachBodyEvents() {
			browserControl.Document.Body.AttachEventHandler("onselect", new EventHandler(OnSelectionChanges));
			browserControl.Document.Body.AttachEventHandler("onclick", new EventHandler(OnSelectionChanges));
			browserControl.Document.Body.AttachEventHandler("onkeydown", new EventHandler(OnSelectionChanges));
			browserControl.Document.Body.AttachEventHandler("onkeypress", new EventHandler(OnKeyPress));
			browserControl.Document.Body.AttachEventHandler("onkeyup", new EventHandler(OnKeyUp));
			browserControl.Document.Body.AttachEventHandler("oncut", new EventHandler(OnCut));
			browserControl.Document.Body.AttachEventHandler("onpaste", new EventHandler(OnPaste));
		}
		private void DetachBodyEvents() {
			browserControl.Document.Body.DetachEventHandler("onselect", new EventHandler(OnSelectionChanges));
			browserControl.Document.Body.DetachEventHandler("onclick", new EventHandler(OnSelectionChanges));
			browserControl.Document.Body.DetachEventHandler("onkeydown", new EventHandler(OnSelectionChanges));
			browserControl.Document.Body.DetachEventHandler("onkeypress", new EventHandler(OnKeyPress));
			browserControl.Document.Body.DetachEventHandler("onkeyup", new EventHandler(OnKeyUp));
			browserControl.Document.Body.DetachEventHandler("oncut", new EventHandler(OnCut));
			browserControl.Document.Body.DetachEventHandler("onpaste", new EventHandler(OnPaste));
		}
		private string GetCurrentValue() {
			string result = string.Empty;
			if(tabControl != null && tabControl.SelectedTabPageIndex == 1) {
				result = memo.Text;
			}
			else if(browserControl.Document != null) {
				result = DocumentInnerHtml;
			}
			return result;
		}
		private string DocumentInnerHtml {
			get {
				string result = null;
				if(browserControl != null && browserControl.Document != null && browserControl.Document.Body != null) {
					result = browserControl.Document.Body.InnerHtml;
				}
				return result;
			}
			set {
				if(browserControl != null && browserControl.Document != null) {
					if(browserControl.Document.Body != null) {
						browserControl.Document.Body.InnerHtml = value;
					}
				}
			}
		}
		protected virtual void OnModified() {
			if(Modified != null) {
				Modified(this, EventArgs.Empty);
			}
			OnHtmlChanged();
		}
		private void OnHtmlChanged() {
			if(HtmlChanged != null) {
				HtmlChanged(this, EventArgs.Empty);
			}
		}
		public void RecreateBody(string headerInnerHtml) {
			if(!string.IsNullOrEmpty(headerInnerHtml)) {
				SetHeaderHtml(headerInnerHtml);
			}
			HTMLDocument2.write("<body></body>");
			AttachBodyEvents();
		}
		public string Html {
			get { return GetCurrentValue(); }
			set {
				UpdateCurrentValue(value);
			}
		}
		public bool ReadOnly {
			get { return readOnly; }
			set {
				if(readOnly != value) {
					readOnly = value;
					UpdateReadOnly();
				}
			}
		}
		public HtmlEditor()
			: this(string.Empty) {
		}
		public HtmlEditor(string headerInnerHtml) {
			Initialize(headerInnerHtml);
		}
		private void Initialize(string headerInnerHtml) {
			InitializeComponent();
			Localize();
			InitButtonsImages();
			selectionDependentCommands = new List<string>(new string[] { "Copy", "Cut" });
			browserControl.DocumentText = @"<html></html>";
			if(HTMLDocument2 != null) {
				UpdateReadOnly();
				RecreateBody(headerInnerHtml);
				HTMLDocument2.close();
			}
			memo.TextChanged += new EventHandler(Memo_TextChanged);
			foreach(BarItem item in barManager.Items) {
				if(item is BarButtonItem) {
					BarButtonItem barButtonItem = (BarButtonItem)item;
					if(barButtonItem.ButtonStyle != BarButtonStyle.DropDown) {
						barButtonItem.ButtonStyle = BarButtonStyle.Check;
					}
				}
			}
			UpdateButtonState();
		}
		public void SetHeaderHtml(string headerInnerHtml) {
			this.headerInnerHtml = headerInnerHtml;
			HTMLDocument2.write("<head>" + headerInnerHtml + "</head>");
		}
		public WebBrowser BrowserControl {
			get {
				return this.browserControl;
			}
		}
		public XtraBars.BarManager BarManager {
			get {
				return this.barManager;
			}
		}
		public XtraTabControl TabControl {
			get {
				return tabControl;
			}
		}
		public MemoEdit Memo {
			get {
				return memo;
			}
		}
		public IHTMLDocument2 HTMLDocument2 {
			get {
				return (browserControl != null && browserControl.Document != null) ? browserControl.Document.DomDocument as IHTMLDocument2 : null;
			}
		}
		public bool ShowComboboxCaptions {
			get {
				return this.showComboboxCaptions;
			}
			set {
				if(this.showComboboxCaptions != value) {
					this.showComboboxCaptions = value;
					ChangeComboboxesCaptionsVisibility();
				}
			}
		}
		public event EventHandler Modified;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler HtmlChanged;
		#region IXtraResizableControl Members
		public bool IsCaptionVisible {
			get { return true; }
		}
		public Size MaxSize {
			get { return Size.Empty; }
		}
		public Size MinSize {
			get { return new Size(0, Math.Max(bar1.Size.Height + bar2.Size.Height + 50, TabControl.CalcSizeByPageClient(Memo.MinimumSize).Height)); }
		}
		event EventHandler IXtraResizableControl.Changed {
			add { SizeChangedHandler += value; }
			remove { SizeChangedHandler -= value; }
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				this.Bold.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Italic.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Underline.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.StrikeThrough.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.JustifyLeft.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.JustifyCenter.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.JustifyRight.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.JustifyFull.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Forecolor.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.Forecolor_ItemClick);
				this.forecolorPopupControl.ColorChanged -= new System.EventHandler(this.ColorPopupControl_ColorChanged);
				this.backColor.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.Forecolor_ItemClick);
				this.backcolorPopupControl.ColorChanged -= new System.EventHandler(this.ColorPopupControl_ColorChanged);
				this.FontSelector.EditValueChanged -= new System.EventHandler(this.FontSelector_EditValueChanged);
				this.FontSize.EditValueChanged -= new System.EventHandler(this.FontSelector_EditValueChanged);
				this.Styles.EditValueChanged -= new System.EventHandler(this.FontSelector_EditValueChanged);
				this.Cut.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Copy.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Paste.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Undo.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Redo.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.RemoveFormat.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Subscript.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Superscript.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.InsertOrderedList.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.InsertUnorderedList.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Indent.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Outdent.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.CreateLink.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.Unlink.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.BarButtonItem_ItemClick);
				this.InsertImage.ItemClick -= new DevExpress.XtraBars.ItemClickEventHandler(this.InsertImage_ItemClick);
				this.tabControl.SelectedPageChanged -= new DevExpress.XtraTab.TabPageChangedEventHandler(XtraTabControl1_SelectedPageChanged);
				this.tabControl.GotFocus -= new System.EventHandler(XtraTabControl1_GotFocus);
				this.memo.TextChanged -= new EventHandler(Memo_TextChanged);
				this.browserControl.PreviewKeyDown -= new System.Windows.Forms.PreviewKeyDownEventHandler(browserControl_PreviewKeyDown);
				components.Dispose();
				Modified = null;
				HtmlChanged = null;
			}
			base.Dispose(disposing);
		}
	}
	[Browsable(false)]
	[ToolboxItem(false)]
	public class WebBrowserEx : WebBrowser {
		protected override bool IsInputKey(Keys keyData) {
			return keyData == Keys.Return || base.IsInputKey(keyData);
		}
	}
}
