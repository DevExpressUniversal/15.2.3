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
using System.IO;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using System.Windows.Forms;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.Utils.Menu;
using DevExpress.ExpressApp.Utils;
using System.Drawing;
using DevExpress.ExpressApp.Win.Utils;
using System.ComponentModel;
using DevExpress.Skins;
using DevExpress.ExpressApp.DC;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.ExpressApp.FileAttachments.Win {
	public class CreateCustomFileDataObjectEventArgs : EventArgs {
		private IFileData fileData;
		public CreateCustomFileDataObjectEventArgs(IFileData fileData) {
			this.fileData = fileData;
		}
		public IFileData FileData {
			get { return fileData; }
			set { fileData = value; }
		}
	}
	public class RepositoryItemFileDataEdit : RepositoryItemButtonEdit {
		private string fileTypesFilter = string.Empty;
		private IFileDataManager fileDataManager;
		private IObjectSpace objectSpace;
		private IMemberInfo memberInfo;
		internal static string EditorName {
			get { return typeof(FileDataEdit).FullName; }
		}
		internal static void Register() {
			if(!EditorRegistrationInfo.Default.Editors.Contains(EditorName)) {
				EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo(EditorName, typeof(FileDataEdit),
					typeof(RepositoryItemFileDataEdit), typeof(ButtonEditViewInfo),
					new ButtonEditPainter(), true, EditImageIndexes.ButtonEdit, typeof(DevExpress.Accessibility.ButtonEditAccessible)));
			}
		}
		static RepositoryItemFileDataEdit() {
			RepositoryItemFileDataEdit.Register();
		}
		public IFileData CreateFileDataObject() {
			IFileData fileData = FileAttachmentsWindowsFormsModule.CreateFileData(objectSpace, memberInfo);
			CreateCustomFileDataObjectEventArgs fileDataArgs = new CreateCustomFileDataObjectEventArgs(fileData);
			if(CreateCustomFileDataObject != null) {
				CreateCustomFileDataObject(this, fileDataArgs);
			}
			return fileDataArgs.FileData;
		}
		private bool fileDataReadOnly;
		public override string EditorTypeName { get { return EditorName; } }
		public RepositoryItemFileDataEdit() {
			InitializeAppearance();
		}
		internal void InitializeAppearance() {
			Color color = Color.Empty;
			if(LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin) {
				Skin skin = EditorsSkins.GetSkin(LookAndFeel);
				if(skin != null) {
					color = skin.Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor);
				}
			}
			if(color.IsEmpty) color = Color.Blue;
			Appearance.ForeColor = color;
			Appearance.Font = new Font(Appearance.Font, FontStyle.Underline);
		}
		public bool FileDataReadOnly {
			get {
				return fileDataReadOnly;
			}
			set {
				fileDataReadOnly = value;
			}
		}
		public IObjectSpace ObjectSpace {
			get {
				return objectSpace;
			}
			set {
				objectSpace = value;
			}
		}
		public IMemberInfo MemberInfo {
			get { return memberInfo; }
			set { memberInfo = value; }
		}
		public override void Assign(RepositoryItem item) {
			base.Assign(item);
			if(item is RepositoryItemFileDataEdit) {
				RepositoryItemFileDataEdit source = ((RepositoryItemFileDataEdit)item);
				this.fileDataReadOnly = source.fileDataReadOnly;
				this.CreateCustomFileDataObject = source.CreateCustomFileDataObject;
				this.fileTypesFilter = source.fileTypesFilter;
				this.fileDataManager = source.fileDataManager;
				this.ObjectSpace = source.ObjectSpace;
				this.MemberInfo = source.MemberInfo;
			}
		}
		public override string GetDisplayText(FormatInfo format, object editValue) {
			return DevExpress.Persistent.Base.ReflectionHelper.GetObjectDisplayText(editValue);
		}
		public string FileTypesFilter {
			get {
				return fileTypesFilter;
			}
			set {
				fileTypesFilter = value;
			}
		}
		public IFileDataManager FileDataManager {
			get { return fileDataManager; }
			set { fileDataManager = value; }
		}
		public event EventHandler<CreateCustomFileDataObjectEventArgs> CreateCustomFileDataObject;
	}
	[System.ComponentModel.ToolboxItem(false)]
	public class FileDataEdit : ButtonEdit {
		static FileDataEdit() {
			RepositoryItemFileDataEdit.Register();
		}
		private ControlCursorHelper cursorHelper;
		private void OnClearMenuItemSelected(object sender, EventArgs e) {
			ClearFileData();
		}
		private void OnSaveMenuItemSelected(object sender, EventArgs e) {
			Properties.FileDataManager.SaveFiles(new List<IFileData>(new IFileData[] { FileData }));
			UpdateEditValue();
		}
		private void OnOpenMenuItemSelected(object sender, EventArgs e) {
			Properties.FileDataManager.Open(FileData);
			UpdateEditValue();
		}
		private void UpdateDeleteMenuItem(DXMenuItem item, EventArgs e) {
			item.Visible = false;
		}
		private void UpdateClearMenuItem(DXMenuItem item, EventArgs e) {
			item.Enabled = !Properties.FileDataReadOnly && !FileDataHelper.IsFileDataEmpty(FileData);
		}
		private void UpdateSaveOpenMenuItem(DXMenuItem item, EventArgs e) {
			item.Enabled = !FileDataHelper.IsFileDataEmpty(FileData);
		}
		private void UpdateEditValue() {
			this.UpdateDisplayText();
		}
		private void DropFile(string[] fileNames) {
			FileSelected(fileNames[0]);
		}
		private void buttonPressed(object sender, ButtonPressedEventArgs e) {
			ShowFileOpenDialog();
		}
		private void MaskBox_MouseLeave(object sender, EventArgs e) {
			cursorHelper.Restore();
		}
		private void MaskBox_MouseMove(object sender, MouseEventArgs e) {
			if(MaskBox != null) {
				float textWidth = CreateGraphics().MeasureString(MaskBox.MaskBoxText, ViewInfo.Appearance.Font).Width;
				if(e.X < textWidth && e.Button == MouseButtons.None) {
					cursorHelper.ChangeControlCursor(Cursors.Hand);
				}
				else {
					cursorHelper.Restore();
				}
			}
		}
		protected override void OnClick(EventArgs e) {
			if(MaskBox != null && MaskBox.Cursor == Cursors.Hand) {
				OnOpenMenuItemSelected(this, e);
			}
			base.OnClick(e);
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.KeyCode == Keys.Enter ||
				(e.Modifiers == Keys.Alt && e.KeyCode == Keys.Down)) {
				ShowFileOpenDialog();
			}
		}
		protected override void OnEditorKeyDown(KeyEventArgs e) {
			base.OnEditorKeyDown(e);
			if((e.KeyCode == Keys.Delete) && (e.Modifiers == Keys.Control)) {
				ClearFileData();
			}
		}
		protected override void OnClickButton(EditorButtonObjectInfoArgs buttonInfo) {
			base.OnClickButton(buttonInfo);
			if(buttonInfo.BuiltIn) {
				ShowFileOpenDialog();
			}
		}
		protected override DevExpress.Utils.Menu.DXPopupMenu CreateMenu() {
			DXPopupMenu result = base.CreateMenu();
			string deleteName = Localizer.Active.GetLocalizedString(StringId.TextEditMenuDelete);
			foreach(DXMenuItem menuItem in result.Items) {
				if(menuItem is DXMenuItemTextEdit && menuItem.Caption == deleteName) {
					DXMenuItemTextEdit menuItemTextEdit = (DXMenuItemTextEdit)menuItem;
					menuItemTextEdit.UpdateElement = new MenuItemUpdateElement(menuItemTextEdit, new MenuItemUpdateHandler(UpdateDeleteMenuItem));
				}
			}
			System.Drawing.Image img;
			DXMenuItemTextEdit item;
			img = ImageLoader.Instance.GetImageInfo("MenuBar_Open").Image;
			item = new DXMenuItemTextEdit(StringId.PictureEditOpenFileTitle, new EventHandler(OnOpenMenuItemSelected), img);
			item.Caption = CaptionHelper.GetLocalizedText(FileAttachmentsWindowsFormsModule.LocalizationGroup, "Editor_Open");
			item.UpdateElement = new MenuItemUpdateElement(item, new MenuItemUpdateHandler(UpdateSaveOpenMenuItem));
			item.BeginGroup = true;
			result.Items.Add(item);
			img = ImageLoader.Instance.GetImageInfo("MenuBar_SaveTo").Image;
			item = new DXMenuItemTextEdit(StringId.PictureEditSaveFileTitle, new EventHandler(OnSaveMenuItemSelected), img);
			item.Caption = CaptionHelper.GetLocalizedText(FileAttachmentsWindowsFormsModule.LocalizationGroup, "Editor_Save");
			item.UpdateElement = new MenuItemUpdateElement(item, new MenuItemUpdateHandler(UpdateSaveOpenMenuItem));
			result.Items.Add(item);
			img = ImageLoader.Instance.GetImageInfo("MenuBar_Clear").Image;
			item = new DXMenuItemTextEdit(StringId.DateEditClear, new EventHandler(OnClearMenuItemSelected), img);
			item.Caption = CaptionHelper.GetLocalizedText(FileAttachmentsWindowsFormsModule.LocalizationGroup, "Editor_Clear");
			item.UpdateElement = new MenuItemUpdateElement(item, new MenuItemUpdateHandler(UpdateClearMenuItem));
			result.Items.Add(item);
			return result;
		}
		protected override void OnDragDrop(DragEventArgs e) {
			if(e.Effect == DragDropEffects.Copy) {
				DropFile((string[])e.Data.GetData(DataFormats.FileDrop));
			}
			else {
				base.OnDragDrop(e);
			}
		}
		protected override void UpdateMaskBoxProperties(bool always) {
			base.UpdateMaskBoxProperties(always);
			MaskBox.ReadOnly = true;
		}
		protected override void OnDragOver(DragEventArgs e) {
			e.Effect = DragDropEffects.None;
			if(!Properties.FileDataReadOnly && e.Data.GetDataPresent(DataFormats.FileDrop)) {
				if(((string[])e.Data.GetData(DataFormats.FileDrop)).Length == 1) {
					e.Effect = DragDropEffects.Copy;
				}
			}
		}
		protected virtual IFileData OnCreateCustomFileDataObject() {
			return Properties.CreateFileDataObject();
		}
		protected virtual void ShowFileOpenDialog() {
			if(!Properties.FileDataReadOnly) {
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.CheckFileExists = true;
				dialog.CheckPathExists = true;
				dialog.DereferenceLinks = true;
				dialog.Multiselect = false;
				dialog.Filter = Properties.FileTypesFilter;
				if(dialog.ShowDialog(Form.ActiveForm) == DialogResult.OK && !string.IsNullOrEmpty(dialog.FileName)) {
					FileSelected(dialog.FileName);
				}
			}
		}
		public FileDataEdit() {
			AllowDrop = true;
			MaskBox.MouseMove += new MouseEventHandler(MaskBox_MouseMove);
			MaskBox.MouseLeave += new EventHandler(MaskBox_MouseLeave);
			MaskBox.DoubleClick += new EventHandler(MaskBox_DoubleClick);
			cursorHelper = new ControlCursorHelper(MaskBox);
		}
		void MaskBox_DoubleClick(object sender, EventArgs e) {
			if(MaskBox != null) {
				float textWidth = CreateGraphics().MeasureString(MaskBox.MaskBoxText, ViewInfo.Appearance.Font).Width;
				if(textWidth < ((MouseEventArgs)e).X) {
					ShowFileOpenDialog();
				}
			}
			else {
				ShowFileOpenDialog();
			}
		}
		public void ClearFileData() {
			if(FileData != null) {
				FileData.Clear();
				UpdateEditValue();
				IsModified = true;
			}
		}
		public void FileSelected(string fileName) {
			if(!string.IsNullOrEmpty(fileName)) {
				Focus();
				if(ContainsFocus) { 
					using(FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
						if(FileData == null) {
							EditValue = OnCreateCustomFileDataObject();
							IsModified = true; 
							DoValidate();
						}
						if(FileData != null) {
							FileDataHelper.LoadFromStream(FileData, Path.GetFileName(fileName), stream, fileName);
							UpdateEditValue();
							IsModified = true;
						}
					}
				}
			}
		}
		public override string EditorTypeName { get { return RepositoryItemFileDataEdit.EditorName; } }
		public IFileData FileData {
			get { return EditValue as IFileData; }
		}
		public new RepositoryItemFileDataEdit Properties {
			get { return (RepositoryItemFileDataEdit)base.Properties; }
		}
	}
}
