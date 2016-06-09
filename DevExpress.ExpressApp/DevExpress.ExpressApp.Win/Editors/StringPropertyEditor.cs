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
using System.ComponentModel;
using System.Collections.Generic;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors.Mask;
using DevExpress.Utils.Controls;
using DevExpress.ExpressApp.Win.Localization;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win.Editors {
	public class StringPropertyEditor : DXPropertyEditor {
		private RepositoryItem repositoryItem;
		protected override void SetupRepositoryItem(RepositoryItem item) {
			base.SetupRepositoryItem(item);
			if(item is RepositoryItemStringEdit) {
				((RepositoryItemStringEdit)item).Init(EditMask, EditMaskType);
			}
			else if(item is RepositoryItemPredefinedValuesStringEdit) {
				((RepositoryItemPredefinedValuesStringEdit)item).Init(EditMask, EditMaskType);
			}
		}
		private bool IsSimpleStringEdit() {
			if(MemberInfo == null) return true;
			return MemberInfo.MemberType != typeof(string) || (Model.RowCount <= 1);
		}
		private bool IsComboBoxStringEdit() {
			return IsSimpleStringEdit() && !string.IsNullOrEmpty(Model.PredefinedValues);
		}
		protected override object CreateControlCore() {
			BaseEdit result;
			if(IsComboBoxStringEdit()) {
				result = new PredefinedValuesStringEdit(Model.MaxLength, PredefinedValuesEditorHelper.CreatePredefinedValuesFromString(Model.PredefinedValues));
			}
			else if(IsSimpleStringEdit()) {
				result = new StringEdit(Model.MaxLength);
			}
			else {
				result = new LargeStringEdit(Model.RowCount, Model.MaxLength);
			}
			return result;
		}			
		protected override RepositoryItem CreateRepositoryItem() {
			if(IsComboBoxStringEdit()) {
				repositoryItem = new RepositoryItemPredefinedValuesStringEdit(Model.MaxLength, PredefinedValuesEditorHelper.CreatePredefinedValuesFromString(Model.PredefinedValues));
			}
			else if(IsSimpleStringEdit()) {
				repositoryItem = new RepositoryItemStringEdit(Model.MaxLength);
			}
			else {
				RepositoryItemMemoExEdit memoEdit = new RepositoryItemMemoExEdit();
				memoEdit.MaxLength = Model.MaxLength;
				memoEdit.ShowDropDown = ShowDropDown.SingleClick;
				memoEdit.AutoHeight = false;
				memoEdit.ShowIcon = false;
				memoEdit.ScrollBars = ScrollBars.Both;
				repositoryItem = memoEdit;
			}
			return repositoryItem;
		}
		public StringPropertyEditor(Type objectType, IModelMemberViewItem model)
			: base(objectType, model) {
		}
		public new TextEdit Control {
			get { return (TextEdit)base.Control; }
		}
		public override bool CanFormatPropertyValue {
			get { return true; }
		}
	}
	public class RepositoryItemStringEdit : RepositoryItemTextEdit {
		internal const string EditorName = "StringEdit";
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(StringEdit),
					typeof(RepositoryItemStringEdit),
					typeof(TextEditViewInfo), new TextEditPainter(), true, EditImageIndexes.TextEdit, 
					typeof(DevExpress.Accessibility.TextEditAccessible)));
			}
		}
		static RepositoryItemStringEdit() {
			RepositoryItemStringEdit.Register();
		}
		public override string EditorTypeName { get { return EditorName; } }
		public RepositoryItemStringEdit(int maxLength) : this() {
			MaxLength = maxLength;
		}
		public RepositoryItemStringEdit() {
			Mask.MaskType = MaskType.None;
			if(Mask.MaskType != MaskType.RegEx) {
				Mask.UseMaskAsDisplayFormat = true;
			}
		}
		public void Init(string editMask, EditMaskType maskType) {
			if(!string.IsNullOrEmpty(editMask)) {
				Mask.EditMask = editMask;
				switch(maskType) {
					case EditMaskType.RegEx:
						Mask.UseMaskAsDisplayFormat = false;
						Mask.MaskType = MaskType.RegEx;
						break;
					default:
						Mask.MaskType = MaskType.Simple;
						break;
				}
			}
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class StringEdit : TextEdit {
		static StringEdit() {
			RepositoryItemStringEdit.Register();
		}
		public StringEdit() { }
		public StringEdit(int maxLength) {
			((RepositoryItemStringEdit)Properties).MaxLength = maxLength;
		}
		public override string EditorTypeName { get { return RepositoryItemStringEdit.EditorName; } }
	}
	public class RepositoryItemPredefinedValuesStringEdit : RepositoryItemComboBox {
		internal const string EditorName = "PredefinedValuesStringEdit";
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(PredefinedValuesStringEdit),
					typeof(RepositoryItemPredefinedValuesStringEdit),
					typeof(ComboBoxViewInfo), new ButtonEditPainter(), true,
					EditImageIndexes.ComboBoxEdit, typeof(DevExpress.Accessibility.PopupEditAccessible)));
			}
		}
		public override string EditorTypeName { get { return EditorName; } }
		static RepositoryItemPredefinedValuesStringEdit() {
			RepositoryItemPredefinedValuesStringEdit.Register();
		}
		public RepositoryItemPredefinedValuesStringEdit() {
			Mask.MaskType = MaskType.None;
			Mask.UseMaskAsDisplayFormat = true;
		}
		public RepositoryItemPredefinedValuesStringEdit(int maxLength, IEnumerable<string> predefinedValues)
			: this() {
			MaxLength = maxLength;
			CreatePredefinedListItems(predefinedValues);
		}
		public void CreatePredefinedListItems(IEnumerable<string> predefinedValues) {
			Items.Clear();
			foreach(string item in predefinedValues) {
				Items.Add(item);
			}
		}
		public void Init(string editMask, EditMaskType maskType) {
			if(!string.IsNullOrEmpty(editMask)) {
				Mask.EditMask = editMask;
				switch(maskType) {
					case EditMaskType.RegEx:
						Mask.MaskType = MaskType.RegEx;
						break;
					default:
						Mask.MaskType = MaskType.Simple;
						break;
				}
			}
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class PredefinedValuesStringEdit : ComboBoxEdit {
		static PredefinedValuesStringEdit() {
			RepositoryItemPredefinedValuesStringEdit.Register();
		}
		public PredefinedValuesStringEdit() { }
		public PredefinedValuesStringEdit(int maxLength, IEnumerable<string> predefinedValues)
			: this() {
			((RepositoryItemPredefinedValuesStringEdit)Properties).MaxLength = maxLength;
			CreatePredefinedListItems(predefinedValues);
		}
		public void CreatePredefinedListItems(IEnumerable<string> predefinedValues) {
			((RepositoryItemPredefinedValuesStringEdit)Properties).CreatePredefinedListItems(predefinedValues);
		}
		public override string EditorTypeName { get { return RepositoryItemPredefinedValuesStringEdit.EditorName; } }
	}
	public class RepositoryItemLargeStringEdit : RepositoryItemMemoEdit {
		internal const string EditorName = "LargeStringEdit";
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(LargeStringEdit),
					typeof(RepositoryItemLargeStringEdit),
					typeof(MemoEditViewInfo), new MemoEditPainter(), true,
					EditImageIndexes.MemoEdit, typeof(DevExpress.Accessibility.TextEditAccessible)));
			}
		}
		public override string EditorTypeName { get { return EditorName; } }
		static RepositoryItemLargeStringEdit() {
			RepositoryItemLargeStringEdit.Register();
		}
		public RepositoryItemLargeStringEdit() { }
		public RepositoryItemLargeStringEdit(int maxLength) : this() {
			MaxLength = maxLength;
		}
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class LargeStringEdit : MemoEdit, IXtraResizableControl {
		private bool enableFind = true;		
		private DXMenuItem findMenuItem;
		private FindForm findForm;
		private int rowCount;
		private Size minimumSize;
		private void findMenuItem_Click(object sender, EventArgs e) {
			ShowFindForm();
		}
		private void DisposeFindForm() {
			if(findForm != null) {
				findForm.DoFind -= new EventHandler(form_DoFind);
				findForm.Dispose();
				findForm = null;
			}
		}
		private void CreateFindForm() {
			findForm = new FindForm();
			findForm.DoFind += new EventHandler(form_DoFind);
			findForm.FormBorderStyle = FormBorderStyle.FixedDialog;
			findForm.StartPosition = FormStartPosition.CenterParent;
			findForm.ShowInTaskbar = true;
			findForm.ShowIcon = false;
			findForm.MaximizeBox = false;
			findForm.MinimizeBox = false;
		}
		private FindForm GetFindForm {
			get {
				if(findForm == null) {
					CreateFindForm();
				}
				return findForm;
			}
		}
		private void ShowFindForm() {
			if(SelectionLength < Text.Length) {
				GetFindForm.TextToFind = SelectedText;
			}
			else {
				DeselectAll();
			}
			bool hideSelectionValue = Properties.HideSelection;
			try {
				Properties.HideSelection = false;
				GetFindForm.ShowDialog();
			}
			finally {
				if(SelectionLength <= 0) {
					Properties.HideSelection = hideSelectionValue;
				}
			}
		}
		private void DoFind(string text) {
			int startIndex = SelectionStart + SelectionLength;
			int foundIndex = Text.IndexOf(text, startIndex >= 0 ? startIndex : 0);
			if(foundIndex >= 0) {
				Select(foundIndex, text.Length);
				ScrollToCaret();
				ShowCaret();
			}
		}
		private void form_DoFind(object sender, EventArgs e) {
			DoFind(GetFindForm.TextToFind);
		}
		private void RaiseXtraResizableChanged() {
			if(changedEventHandler != null) {
				changedEventHandler(this, EventArgs.Empty);
			}
		}		
		private void SetMinimumSize() {
			BaseEditViewInfo viewInfo = Properties.CreateViewInfo();
			Graphics graphics = Graphics.FromHwnd(Handle);
			int rowHeight = viewInfo.CalcMinHeight(graphics);
			minimumSize = new Size(MinimumSize.Width, ((rowCount == 0) ? 1 : rowCount) * rowHeight + 2);
			RaiseXtraResizableChanged();
		}
		private void LargeStringEdit_HandleCreated(object sender, EventArgs e) {
			HandleCreated -= new EventHandler(LargeStringEdit_HandleCreated);
			BeginInvoke(new MethodInvoker(SetMinimumSize));
		}		
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			if(enableFind) {
				if(e.KeyCode == Keys.F && e.Modifiers == Keys.Control) {
					ShowFindForm();
					e.Handled = true;
				}
				else if(e.KeyCode == Keys.F3 && e.Modifiers == Keys.None) {
					DoFind(GetFindForm.TextToFind);
					e.Handled = true;
				}
			}
			base.OnEditorKeyDown(e);
		}		
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					DisposeFindForm();
					if(findMenuItem != null) {
						findMenuItem.Click -= new EventHandler(findMenuItem_Click);
						findMenuItem = null;
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		static LargeStringEdit() {
			RepositoryItemLargeStringEdit.Register();
		}
		public LargeStringEdit(int rowCount, int maxLength) {
			((RepositoryItemLargeStringEdit)Properties).MaxLength = maxLength;
			this.rowCount = rowCount;
			if(enableFind) {
				findMenuItem = new DXMenuItem(LargeStringEditFindFormLocalizer.Active.GetLocalizedString("FindMenuItemText"));
				findMenuItem.BeginGroup = true;
				findMenuItem.Click += new EventHandler(findMenuItem_Click);
				Menu.Items.Add(findMenuItem);
			}
			HandleCreated += new EventHandler(LargeStringEdit_HandleCreated);		
		}		
		public override string EditorTypeName { get { return RepositoryItemLargeStringEdit.EditorName; } }
		public bool EnableFind {
			get { return enableFind; }
			set { enableFind = value; }
		}
		public LargeStringEdit()
			: this(0, 0) {
		}
		#region IXtraResizableControl Members
		EventHandler changedEventHandler;
		event EventHandler IXtraResizableControl.Changed {
			add { changedEventHandler += value; }
			remove { changedEventHandler -= value; }
		}
		bool IXtraResizableControl.IsCaptionVisible {
			get { return false; }
		}
		Size IXtraResizableControl.MaxSize {
			get { return MaximumSize; }
		}
		Size IXtraResizableControl.MinSize {
			get { return minimumSize; }
		}
		#endregion
	}
}
