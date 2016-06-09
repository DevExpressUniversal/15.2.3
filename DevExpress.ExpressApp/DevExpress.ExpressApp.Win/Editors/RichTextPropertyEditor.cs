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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Editors;
using System.ComponentModel;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win.Editors {
	public class RichTextPropertyEditor : WinPropertyEditor, IInplaceEditSupport {
		private int rowCount = 10;
		private void Editor_RtfTextChanged(object sender, EventArgs e) {
			if(!inReadValue) {
				OnControlValueChanged();
			}
		}
		protected override object CreateControlCore() {
			RichTextBoxWithBinding editor = new RichTextBoxWithBinding();
			if(rowCount != 0) {
				editor.Height = rowCount * editor.PreferredHeight + 2;
			}
			else {
				editor.Height = 15 * editor.PreferredHeight + 2;
			}
			if(MaxLength > 0) {
				editor.MaxLength = MaxLength;
			}
			editor.RtfTextChanged += new EventHandler(Editor_RtfTextChanged);
			editor.ScrollBars = RichTextBoxScrollBars.Both;
			editor.WordWrap = false;
			return editor;
		}
		protected override void UpdateControlEnabled(bool enabled) {
			if (Control != null) {
				Control.ReadOnly = !enabled;
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing && Control != null) {
					Control.RtfTextChanged -= new EventHandler(Editor_RtfTextChanged);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		public RichTextPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
			ControlBindingProperty = "RtfText";
			rowCount = model.RowCount;
		}
		public new RichTextBoxWithBinding Control {
			get { return ((RichTextBoxWithBinding)base.Control); }
		}
		public int RowCount {
			get { return rowCount; }
			set { rowCount = value; }
		}
		#region IInplaceEditSupport Members
		public RepositoryItem CreateRepositoryItem() {
			RepositoryItemRtfEditEx result = new RepositoryItemRtfEditEx();
			result.ReadOnly = !AllowEdit;
			return result;
		}
		#endregion
	}
	public class RepositoryItemRtfEditEx : RepositoryItemBlobBaseEdit {
		static RepositoryItemRtfEditEx() {
			RepositoryItemRtfEditEx.Register();
		}
		internal const string EditorName = "RtfEdit";
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(RtfEditEx),
					typeof(RepositoryItemRtfEditEx), typeof(BlobBaseEditViewInfo), new BlobBaseEditPainter(), false)); 
			}
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			RichTextBox rtfBox = new RichTextBox();
			rtfBox.Rtf = (editValue == DBNull.Value) ? null : (string)editValue;
			string result = (string)rtfBox.Text;
			rtfBox.Dispose();
			return result;
		}
		public RepositoryItemRtfEditEx() { }
		public new RtfEditEx OwnerEdit { get { return (RtfEditEx)base.OwnerEdit; } }
		public override string EditorTypeName { get { return EditorName; } }
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class RtfEditEx : BlobBaseEdit {
		static RtfEditEx() {
			RepositoryItemRtfEditEx.Register();
		}
		private bool canClosePopupForm = true;
		protected override bool CanShowPopup { get { return base.CanShowPopup; } }
		protected override bool IsNeedHideCursorOnPopup { get { return true; } }
		protected override bool AllowPopupTabOut { get { return false; } }
		protected override PopupBaseForm CreatePopupForm() {
			PopupRtfEditExForm form = new PopupRtfEditExForm(this);
			return form;
		}
		protected internal new PopupRtfEditExForm PopupForm { get { return (PopupRtfEditExForm)base.PopupForm; } }
		protected override void DoClosePopup(PopupCloseMode closeMode) {
			if(canClosePopupForm) {
				base.DoClosePopup(closeMode);
			}
		}
		public RtfEditEx() { }
		public override string EditorTypeName { get { return RepositoryItemRtfEditEx.EditorName; } }
		public new RepositoryItemRtfEditEx Properties {
			get { return (RepositoryItemRtfEditEx)base.Properties; }
		}
		public bool CanClosePopupForm {
			get { return canClosePopupForm; }
			set { canClosePopupForm = value; }
		}
	}
	public class PopupRtfEditExForm : BlobBasePopupForm {
		public const int DefaultPopupFormWidth = 300;
		public const int DefaultPopupFormHeight = 200;
		private RichTextBox rtfEdit;
		private void rtfEdit_ModifiedChanged(object sender, EventArgs e) {
			if(!IsControlUpdateLocked) {
				OkButton.Enabled = true;
			}
		}
		private void item_Click(object sender, EventArgs e) {
			FontDialog dialog = new FontDialog();
			try {
				dialog.Font = rtfEdit.SelectionFont;
				OwnerEdit.CanClosePopupForm = false;
				if(dialog.ShowDialog(FindForm()) == DialogResult.OK) {
					rtfEdit.SelectionFont = dialog.Font;
				}
			}
			finally {
				OwnerEdit.CanClosePopupForm = true;
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if(disposing && rtfEdit != null) {
					rtfEdit.ModifiedChanged -= new EventHandler(rtfEdit_ModifiedChanged);
					rtfEdit.LinkClicked -= new LinkClickedEventHandler(rtfEdit_LinkClicked);
					rtfEdit = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected override Control EmbeddedControl {
			get { return rtfEdit; }
		}
		protected override Size CalcFormSizeCore() {
			return new Size(DefaultPopupFormWidth, DefaultPopupFormHeight);
		}
		protected override void OnOkButtonClick() {
			base.OnOkButtonClick();
		}
		private void rtfEdit_LinkClicked(object sender, LinkClickedEventArgs e) {
			System.Diagnostics.Process.Start(e.LinkText);
		}
		public PopupRtfEditExForm(RtfEditEx edit)
			: base(edit) {
			this.rtfEdit = new RichTextBox();
			this.rtfEdit.Visible = false;
			this.rtfEdit.ModifiedChanged += new EventHandler(rtfEdit_ModifiedChanged);
			this.rtfEdit.ContextMenuStrip = new ContextMenuStrip();
			this.rtfEdit.ContextMenuStrip.Items.Add("Font...");
			this.rtfEdit.ContextMenuStrip.Items[0].Click += new EventHandler(item_Click);
			this.rtfEdit.LinkClicked += new LinkClickedEventHandler(rtfEdit_LinkClicked);
			this.Controls.Add(rtfEdit);
		}
		public override bool AllowMouseClick(Control control, Point mousePosition) {
			if(base.AllowMouseClick(control, mousePosition)) {
				return true;
			}
			return rtfEdit.Focused;
		}
		public override object ResultValue {
			get { return rtfEdit.Rtf; }
		}
		public new RtfEditEx OwnerEdit {
			get { return (RtfEditEx)base.OwnerEdit; }
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			if(e.KeyData == (Keys.Control | Keys.Enter)) {
				e.Handled = true;
				OwnerEdit.ClosePopup();
				return;
			}
			base.ProcessKeyDown(e);
		}
		public override void ShowPopupForm() {
			BeginControlUpdate();
			try {
				rtfEdit.Rtf = (OwnerEdit.EditValue == DBNull.Value) ? null : (string)OwnerEdit.EditValue;
				rtfEdit.Modified = false;
				rtfEdit.ReadOnly = OwnerEdit.Properties.ReadOnly;
				rtfEdit.BackColor = SystemColors.Window;
				rtfEdit.ContextMenuStrip.Enabled = !rtfEdit.ReadOnly;
			}
			finally {
				EndControlUpdate();
			}
			base.ShowPopupForm();
			FocusFormControl(rtfEdit);
		}
	}
}
