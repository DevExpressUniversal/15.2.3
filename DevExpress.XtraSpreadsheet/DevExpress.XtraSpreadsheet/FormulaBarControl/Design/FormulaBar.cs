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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Skins;
using DevExpress.Services.Implementation;
using DevExpress.Utils;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Services;
using DevExpress.XtraSpreadsheet.Services.Implementation;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Keyboard;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Services;
using DevExpress.Export.Xl;
using SpreadsheetDocumentModel = DevExpress.XtraSpreadsheet.Model.DocumentModel;
namespace DevExpress.XtraSpreadsheet {
	#region FormulaBarCellInplaceEditor
	[DXToolboxItem(false)]
	public partial class FormulaBarCellInplaceEditor : RichEditControl, ICellInplaceEditor, IFormulaBarExpandControl {
		#region Fields
		const int DefaultMaxHeightIn96DPI = 100;
		bool registered;
		ISpreadsheetControl spreadsheetControl;
		DevExpress.XtraSpreadsheet.SpreadsheetControl winSpreadsheetControl;
		FormulaBarExpandButton expandButton;
		int maxHeight;
		int previousSelectionStart = 0;
		int previousSelectionLength = 0;
		#endregion
		public FormulaBarCellInplaceEditor() {
			this.expandButton = new FormulaBarExpandButton(this);
			this.AddService(typeof(IFormulaBarExpandControl), this);
			InitializeComponent();
			this.registered = false;
			this.maxHeight = (int)(SpreadsheetDocumentModel.Dpi / 96.0f * DefaultMaxHeightIn96DPI);
			this.Controls.Add(expandButton);
			expandButton.Parent = this;
			SetEditorOptions();
			SubscribeEvents();
		}
		#region Properties
		public int MinHeight { get { return MinimumSize.Height; } }
		public int MaxHeight { get { return maxHeight; } }
		public ISpreadsheetControl SpreadsheetControl { get { return spreadsheetControl; } set { spreadsheetControl = value; } }
		#region WinSpreadsheetControl
		DevExpress.XtraSpreadsheet.SpreadsheetControl WinSpreadsheetControl {
			get {
				if (winSpreadsheetControl == null)
					winSpreadsheetControl = SpreadsheetControl as DevExpress.XtraSpreadsheet.SpreadsheetControl;
				return winSpreadsheetControl;
			}
		}
		#endregion
		#region ICellInplaceEditor's Properties
		bool ICellInplaceEditor.IsVisible { get; set; }
		int ICellInplaceEditor.SelectionLength {
			get {
				return Math.Min(this.DocumentModel.Selection.Length, Text.Length - ((ICellInplaceEditor)this).SelectionStart);
			}
		}
		int ICellInplaceEditor.SelectionStart {
			get {
				IConvertToInt<DevExpress.XtraRichEdit.Model.DocumentLogPosition> value = this.DocumentModel.Selection.NormalizedStart;
				return value.ToInt();
			}
		}
		bool ICellInplaceEditor.WrapText { get; set; }
		bool ICellInplaceEditor.CurrentEditable { get; set; }
		public bool Registered { get { return registered; } set { registered = value; } }
		#region IsExpand
		public bool IsExpand {
			get { return expandButton.IsExpand; }
			set {
				if (expandButton.IsExpand == value)
					return;
				expandButton.IsExpand = value;
				RaiseCollapseExpandEvent();
			}
		}
		#endregion
		#endregion
		int BorderExtent { get { return BorderSize.Width / 2; } }
		#endregion
		#region Events
		#region ActivateEditor
		EventHandler onActivateEditor;
		public event EventHandler ActivateEditor { add { onActivateEditor += value; } remove { onActivateEditor -= value; } }
		protected internal virtual void RaiseActivateEditor() {
			if (onActivateEditor != null)
				onActivateEditor(this, EventArgs.Empty);
		}
		#endregion
		#region DeactivateEditor
		EventHandler onDeactivateEditor;
		public event EventHandler DeactivateEditor { add { onDeactivateEditor += value; } remove { onDeactivateEditor -= value; } }
		protected internal virtual void RaiseDeactivateEditor() {
			if (onDeactivateEditor != null)
				onDeactivateEditor(this, EventArgs.Empty);
		}
		#endregion
		#region EditorTextChanged
		TextChangedEventHandler onEditorTextChanged;
		public event TextChangedEventHandler EditorTextChanged { add { onEditorTextChanged += value; } remove { onEditorTextChanged -= value; } }
		protected internal virtual void RaiseEditorTextChanged() {
			if (onEditorTextChanged != null) {
				TextChangedEventArgs args = new TextChangedEventArgs(Text);
				onEditorTextChanged(this, args);
			}
		}
		#endregion
		#region EditorSelectionChanged
		EventHandler onEditorSelectionChanged;
		public event EventHandler EditorSelectionChanged { add { onEditorSelectionChanged += value; } remove { onEditorSelectionChanged -= value; } }
		void RaiseEditorSelectionChanged() {
			if (onEditorSelectionChanged != null) {
				EventArgs args = new EventArgs();
				onEditorSelectionChanged(this, args);
			}
		}
		#endregion
		#region Rollback
		EventHandler onRollback;
		public event EventHandler Rollback { add { onRollback += value; } remove { onRollback -= value; } }
		protected internal virtual void RaiseRollback() {
			if (onRollback != null)
				onRollback(this, EventArgs.Empty);
		}
		#endregion
		#region Collapse
		EventHandler onCollapse;
		public event EventHandler Collapse { add { onCollapse += value; } remove { onCollapse -= value; } }
		protected internal virtual void RaiseCollapse() {
			if (onCollapse != null)
				onCollapse(this, EventArgs.Empty);
		}
		#endregion
		#region Expand
		EventHandler onExpand;
		public event EventHandler Expand { add { onExpand += value; } remove { onExpand -= value; } }
		protected internal virtual void RaiseExpand() {
			if (onExpand != null)
				onExpand(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		void ProcessDeferredSelectionChanged() {
			if (ShouldRaiseSelectionChanged())
				RaiseEditorSelectionChanged();
		}
		bool ShouldRaiseSelectionChanged() {
			int selectionLength = ((ICellInplaceEditor)this).SelectionLength;
			int selectionStart = ((ICellInplaceEditor)this).SelectionStart;
			if (previousSelectionLength == 0 && selectionLength == 0)
				return false;
			if (previousSelectionLength != selectionLength || previousSelectionStart != selectionStart) {
				previousSelectionLength = selectionLength;
				previousSelectionStart = selectionStart;
				return true;
			}
			return false;
		}
		public Keys GetShortcut(RichEditCommandId command) {
			Keys result = Keys.None;
			InnerControl.GetShortcut(command, out result);
			return result;
		}
		void SubscribeEvents() {
			expandButton.Click += OnButtonClick;
			Resize += OnResize;
		}
		void UnsubscribeEvents() {
			expandButton.Click -= OnButtonClick;
			Resize -= OnResize;
		}
		void OnButtonClick(object sender, EventArgs e) {
			RaiseCollapseExpandEvent();
		}
		void RaiseCollapseExpandEvent() {
			if (IsExpand)
				RaiseExpand();
			else
				RaiseCollapse();
		}
		void OnResize(object sender, EventArgs e) {
			if (Height == MinHeight)
				expandButton.IsExpand = false;
			else if (Height > MinHeight) {
				expandButton.IsExpand = true;
				maxHeight = Height;
			}
		}
		void SetEditorOptions() {
			BeginUpdate();
			try {
				ActiveViewType = RichEditViewType.Simple;
				Views.SimpleView.Padding = new Padding(6, 0, 0, 0);
				Views.SimpleView.AdjustColorsToSkins = true;
				this.Font = new Font("Calibri", 11);
				SetOperationRestrictions();
				SetDocumentRestrictions();
				SetPasteSelectionCommand();
				SetupKeyboardShortcuts();
			}
			finally {
				EndUpdate();
			}
		}
		void SetPasteSelectionCommand() {
			FormulaBarCommandFactoryService commandFactory = new FormulaBarCommandFactoryService(this, GetService<IRichEditCommandFactoryService>());
			RemoveService(typeof(IRichEditCommandFactoryService));
			AddService(typeof(IRichEditCommandFactoryService), commandFactory);
		}
		void SetOperationRestrictions() {
			RichEditBehaviorOptions options = this.Options.Behavior;
			options.FontSource = RichEditBaseValueSource.Auto;
			options.Drag = XtraRichEdit.DocumentCapability.Hidden;
			options.Drop = XtraRichEdit.DocumentCapability.Hidden;
			options.Save = XtraRichEdit.DocumentCapability.Hidden;
			options.SaveAs = XtraRichEdit.DocumentCapability.Hidden;
			options.Printing = XtraRichEdit.DocumentCapability.Hidden;
			options.CreateNew = XtraRichEdit.DocumentCapability.Hidden;
			options.Open = XtraRichEdit.DocumentCapability.Hidden;
			options.Zooming = XtraRichEdit.DocumentCapability.Hidden;
		}
		void SetDocumentRestrictions() {
			DocumentCapabilitiesOptions options = Options.DocumentCapabilities;
			options.CharacterFormatting = XtraRichEdit.DocumentCapability.Hidden;
			options.CharacterStyle = XtraRichEdit.DocumentCapability.Hidden;
			options.ParagraphFormatting = XtraRichEdit.DocumentCapability.Hidden;
			options.ParagraphStyle = XtraRichEdit.DocumentCapability.Hidden;
			options.Hyperlinks = XtraRichEdit.DocumentCapability.Hidden;
			options.Bookmarks = XtraRichEdit.DocumentCapability.Hidden;
			options.Numbering.Bulleted = XtraRichEdit.DocumentCapability.Hidden;
			options.Numbering.Simple = XtraRichEdit.DocumentCapability.Hidden;
			options.Numbering.MultiLevel = XtraRichEdit.DocumentCapability.Hidden;
			options.InlinePictures = XtraRichEdit.DocumentCapability.Hidden;
			options.TabSymbol = XtraRichEdit.DocumentCapability.Hidden;
			options.Sections = XtraRichEdit.DocumentCapability.Hidden;
			options.HeadersFooters = XtraRichEdit.DocumentCapability.Hidden;
			options.Tables = XtraRichEdit.DocumentCapability.Hidden;
		}
		void SetupKeyboardShortcuts() {
			this.InnerControl.RemoveShortcutKey(Keys.Enter, Keys.Alt);
			this.InnerControl.AssignShortcutKeyToCommand(Keys.Enter, Keys.Alt, RichEditCommandId.InsertParagraph);
		}
		protected override void RaiseDeferredEvents(XtraRichEdit.Model.DocumentModelChangeActions changeActions) {
			if ((changeActions & XtraRichEdit.Model.DocumentModelChangeActions.RaiseContentChanged) != 0)
				RaiseEditorTextChanged();
			if ((changeActions & XtraRichEdit.Model.DocumentModelChangeActions.RaiseSelectionChanged) != 0)
				ProcessDeferredSelectionChanged();
			base.RaiseDeferredEvents(changeActions);
		}
		void SetLocationAndSizeToResizeButton(int buttonWidth) {
			int buttonIndent = GetButtonIndent();
			int x = this.Width - buttonWidth - buttonIndent - BorderExtent;
			expandButton.Location = new System.Drawing.Point(x, BorderExtent);
			expandButton.Size = new System.Drawing.Size(buttonWidth + 2 * buttonIndent, MinHeight - 2 * BorderExtent);
			expandButton.Region = new Region(Rectangle.FromLTRB(0, 0, expandButton.Size.Width - Math.Min(buttonIndent, BorderExtent), expandButton.Size.Height));
		}
		int GetButtonIndent() {
			int buttonIndent = 0;
			Skin skin = SkinManager.Default.GetSkin(SkinProductId.Editors, this.LookAndFeel);
			if (skin != null)
				buttonIndent = skin.Properties.GetInteger("ButtonIndent");
			return buttonIndent;
		}
		protected override Rectangle GetVerticalScrollbarBounds(int verticalScrollbarWidth, int offset, int horizontalScrollbarHeight) {
			SetLocationAndSizeToResizeButton(verticalScrollbarWidth);
			return base.GetVerticalScrollbarBounds(verticalScrollbarWidth, MinHeight - 2 * BorderExtent, horizontalScrollbarHeight);
		}
		#region ICellInplaceEditor's Methods
		void ICellInplaceEditor.SetBounds(InplaceEditorBoundsInfo boundsInfo) {
		}
		void ICellInplaceEditor.Close() {
			WinSpreadsheetControl.Focus();
		}
		void ICellInplaceEditor.SetFocus() {
			Focus();
		}
		void ICellInplaceEditor.SetFont(Office.Drawing.FontInfo fontInfo, float zoomFactor) {
		}
		void ICellInplaceEditor.SetHorizontalAlignment(XlHorizontalAlignment alignment) {
		}
		void ICellInplaceEditor.SetSelection(int start, int length) {
			SubDocument doc = Document.Selection.BeginUpdateDocument();
			Document.Selection = doc.CreateRange(start, length);
			Document.Selection.EndUpdateDocument(doc);
		}
		void ICellInplaceEditor.SetVerticalAlignment(XlVerticalAlignment alignment) {
		}
		void ICellInplaceEditor.Activate() {
			RaiseActivateEditor();
		}
		void ICellInplaceEditor.Deactivate() {
			RaiseDeactivateEditor();
		}
		void ICellInplaceEditor.Rollback() {
			RaiseRollback();
		}
		#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (expandButton != null) {
					UnsubscribeEvents();
					expandButton.Dispose();
					expandButton = null;
					this.RemoveService(typeof(IFormulaBarExpandControl));
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override void OnUpdateUI() {
			base.OnUpdateUI();
			if (WinSpreadsheetControl != null)
				WinSpreadsheetControl.OnUpdateUI();
		}
	}
	#endregion
	public class FormulaBarPasteSelectionCommand : PasteSelectionCommand {
		public FormulaBarPasteSelectionCommand(IRichEditControl control)
			: base(control) {
		}
		protected override RichEditCommand CreateInsertObjectCommand() {
			return new FormulaBarPasteSelectionCoreCommand(base.Control, new ClipboardPasteSource());
		}
	}
	public class FormulaBarPasteSelectionCoreCommand : PasteSelectionCoreCommand {
		public FormulaBarPasteSelectionCoreCommand(IRichEditControl control, PasteSource pasteSource)
			: base(control, pasteSource) {
		}
		public override void ForceExecute(DevExpress.Utils.Commands.ICommandUIState state) {
			Control.Document.InsertText(Control.Document.CaretPosition, Clipboard.GetText());
		}
	}
	public class FormulaBarCommandFactoryService : IRichEditCommandFactoryService {
		readonly IRichEditCommandFactoryService service;
		readonly RichEditControl control;
		public FormulaBarCommandFactoryService(RichEditControl control, IRichEditCommandFactoryService service) {
			DevExpress.Utils.Guard.ArgumentNotNull(control, "control");
			DevExpress.Utils.Guard.ArgumentNotNull(service, "service");
			this.control = control;
			this.service = service;
		}
		public RichEditCommand CreateCommand(RichEditCommandId id) {
			if (id == RichEditCommandId.PasteSelection)
				return new FormulaBarPasteSelectionCommand(control);
			return service.CreateCommand(id);
		}
	}
}
