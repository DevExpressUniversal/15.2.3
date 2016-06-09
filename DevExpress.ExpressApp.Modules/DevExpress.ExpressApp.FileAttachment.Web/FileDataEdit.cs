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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Web;
namespace DevExpress.ExpressApp.FileAttachments.Web {
	public class CreateCustomFileDataObjectEventArgs : EventArgs {
		private IFileData fileData;
		public IFileData FileData {
			get { return fileData; }
			set { fileData = value; }
		}
		public CreateCustomFileDataObjectEventArgs(IFileData fileData) {
			this.fileData = fileData;
		}
	}
	public class FileDataEdit : Table, INamingContainer, IXafCallbackHandler {
		const string LocalizationGroup = "FileAttachments";
		const string FileAnchorStyleClassName = "XafFileDataAnchor";
		private IFileData fileData;
		private bool postDataImmediatelly;
		private ViewEditMode viewEditMode;
		private bool readOnly;
		private HtmlAnchor fileAnchor;
		private ASPxButton changeButton;
		private ASPxButton clearButton;
		private Table table;
		private ASPxUploadControl uploadControl;
		private bool isPrerendered;
		private bool IsFileDataEmpty() {
			return FileData == null || String.IsNullOrEmpty(FileData.FileName);
		}
		private void AddFileDownloadLink() {
			fileAnchor = new HtmlAnchor();
			fileAnchor.ID = "HA";
			fileAnchor.Title = CaptionHelper.GetLocalizedText(LocalizationGroup, "Editor_DownLoad");
			fileAnchor.ServerClick += new EventHandler(saveButton_Click);
			fileAnchor.Attributes["class"] = FileAnchorStyleClassName;
			table.Rows[0].Cells[0].Controls.Add(fileAnchor);
			table.Rows[0].Cells[0].Attributes.Add("onclick", "event.cancelBubble = true; cancelProgress()");
		}
		private ASPxUploadControl CreateUploadControl() {
			ASPxUploadControl uploadControl = new ASPxUploadControl();
			uploadControl.ID = "UPC";
			uploadControl.ClientInstanceName = ClientID + "_UploadControl";
			uploadControl.ShowProgressPanel = true;
			uploadControl.FileUploadComplete += new EventHandler<FileUploadCompleteEventArgs>(uploadControl_FileUploadComplete);
			if(uploadControl.Page != null) {
				SubscribeToClientEvents(uploadControl.Page);
			}
			else {
				uploadControl.Load += new EventHandler(uploadControl_Load);
			}
			return uploadControl;
		}
		private void uploadControl_Load(object sender, EventArgs e) {
			ASPxUploadControl uploadControl = (ASPxUploadControl)sender;
			uploadControl.Load -= new EventHandler(uploadControl_Load);
			SubscribeToClientEvents(uploadControl.Page);
		}
		private void SubscribeToClientEvents(Page page) {
			ICallbackManagerHolder holder = page as ICallbackManagerHolder;
			if(holder != null && holder.CallbackManager != null) {
				uploadControl.ClientSideEvents.TextChanged = ASPxUploadControlHelper.GetTextChangedScript(postDataImmediatelly, holder.CallbackManager);
			}
		}
		private void uploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e) {
			if(e.UploadedFile != null) {
				string fileName = e.UploadedFile.FileName;
				if(!string.IsNullOrEmpty(fileName)) {
					if(FileData == null) {
						FileData = OnCreateCustomFileDataObject();
					}
					if(FileData != null) {
						FileDataHelper.LoadFromStream(FileData, Path.GetFileName(fileName), e.UploadedFile.FileContent, fileName);
					}
					e.CallbackData = GetFileName(FileData);
				}
			}
		}
		private void saveButton_Click(object sender, EventArgs e) {
			FileDataDownloader.Download(FileData);
		}
		private void CreateTableContent() {
			CellPadding = 0;
			CellSpacing = 0;
			Width = Unit.Percentage(1);
			Rows.Add(new TableRow());
			Rows.Add(new TableRow());
			Rows[0].Cells.Add(new TableCell());
			Rows[1].Cells.Add(new TableCell());
			table = RenderHelper.CreateTable();
			table.Rows.Add(new TableRow());
			table.Rows[0].Cells.Add(new TableCell());
			table.Rows[0].Cells.Add(new TableCell());
			table.Rows[0].Cells.Add(new TableCell());
			Rows[0].Cells[0].Controls.Add(table);
		}
		private ASPxButton CreateButton(string imageName, string caption, string id) {
			ASPxButton button = RenderHelper.CreateASPxButton();
			button.CssClass = "xafLookupButton";
			button.ID = id;
			ASPxImageHelper.SetImageProperties(button.Image, imageName);
			if(button.Image.IsEmpty) {
				button.Text = caption;
			}
			button.ToolTip = caption;
			return button;
		}
		private void CreateControlButtons() {
			changeButton = CreateButton("Editor_Edit", CaptionHelper.GetLocalizedText(LocalizationGroup, "Editor_Edit"), "Edit");
			clearButton = CreateButton("Editor_Clear", CaptionHelper.GetLocalizedText(LocalizationGroup, "Editor_Clear"), "Clear");
			clearButton.AutoPostBack = false;
			uploadControl = CreateUploadControl();
			Rows[1].Cells[0].Controls.Add(uploadControl);
			table.Rows[0].Cells[1].Controls.Add(changeButton);
			table.Rows[0].Cells[2].Controls.Add(clearButton);
			changeButton.AutoPostBack = false;
			changeButton.ClientSideEvents.Click = string.Format(@"function (s, e) {{
											if ({0}){{
                                                {0}.SetVisible(!{0}.GetVisible());
                                            }}
										}}", uploadControl.ClientInstanceName);
		}
		private void CreateEditControls() {
			CreateTableContent();
			AddFileDownloadLink();
			CreateControlButtons();
		}
		internal IFileData FileData {
			get { return fileData; }
			set { fileData = value; }
		}
		protected override void OnPreRender(EventArgs e) {
			UpdateControls();
			Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), Page.GetType(), "Page");
			clearButton.ClientSideEvents.Click = string.Format(@"function (s, e){{{0}}}", ((ICallbackManagerHolder)Page).CallbackManager.GetScript(UniqueID, "'Clear'"));
			uploadControl.ClientSideEvents.FileUploadComplete = string.Format(@"function (s, e){{  
                    if (e.callbackData){{
                        var fileAnchor = document.getElementById('{0}');
                        if (fileAnchor){{ 
                            fileAnchor.style.display = 'block';
                            if(fileAnchor.textContent != null){{
                                fileAnchor.textContent = e.callbackData;
                            }}else{{
                                fileAnchor.innerText = e.callbackData;
                            }}
                            s.SetVisible(false);
                        }}    
                        var changeButton = document.getElementById('{1}');
                        var clearButton = document.getElementById('{2}');
                        if(changeButton && clearButton){{
                            changeButton.style.display = 'block';
                            clearButton.style.display = 'block';
                        }}
                    }}
                }}", fileAnchor.ClientID, changeButton.ClientID, clearButton.ClientID);
			if((viewEditMode != ViewEditMode.Edit || readOnly) && IsFileDataEmpty()) {
				Rows[1].Cells[0].Controls.Add(new LiteralControl(WebPropertyEditor.EmptyValue));
				Rows[1].Cells[0].Wrap = false; 
			}
			isPrerendered = true;
			base.OnPreRender(e);
		}
		protected override void Render(HtmlTextWriter writer) {
			if(!isPrerendered) {
				OnPreRender(EventArgs.Empty);
			}
			base.Render(writer);
		}
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			CreateEditControls();
			ICallbackManagerHolder holder = Page as ICallbackManagerHolder;
			if(holder != null) {
				holder.CallbackManager.RegisterHandler(UniqueID, this);
			}
		}
		protected virtual IFileData OnCreateCustomFileDataObject() {
			CreateCustomFileDataObjectEventArgs fileDataArgs = new CreateCustomFileDataObjectEventArgs(null);
			if(CreateCustomFileDataObject != null) {
				CreateCustomFileDataObject(this, fileDataArgs);
			}
			return fileDataArgs.FileData;
		}
		private void UpdateControls() {
			bool isReadOnlyMode = viewEditMode != ViewEditMode.Edit || readOnly;
			bool isDownloadLinkVisible = !IsFileDataEmpty();
			bool isControlButtonsVisible = !isReadOnlyMode && !IsFileDataEmpty();
			fileAnchor.Attributes["style"] = "display:" + (isDownloadLinkVisible ? "block" : "none");
			UpdateBrowseControl(IsFileDataEmpty(), !isReadOnlyMode);
			changeButton.ClientVisible = isControlButtonsVisible;
			clearButton.ClientVisible = isControlButtonsVisible;
		}
		private string GetFileName(IFileData fileData) {
			return fileData != null ? fileData.FileName : "";
		}
		private void UpdateBrowseControl(bool isClientVisible, bool isVisible) {
			uploadControl.Visible = isVisible;
			uploadControl.ClientVisible = isClientVisible;
			fileAnchor.InnerText = GetFileName(FileData);
		}
		public override void Dispose() {
			if(fileAnchor != null) {
				fileAnchor.ServerClick -= new EventHandler(saveButton_Click);
				fileAnchor.Dispose();
				fileAnchor = null;
			}
			if(uploadControl != null) {
				uploadControl.FileUploadComplete -= new EventHandler<FileUploadCompleteEventArgs>(uploadControl_FileUploadComplete);
				uploadControl.Dispose();
				uploadControl = null;
			}
			if(changeButton != null) {
				changeButton.Dispose();
				changeButton = null;
			}
			if(clearButton != null) {
				clearButton.Dispose();
				clearButton = null;
			}
			base.Dispose();
		}
		public ASPxUploadControl UploadControl {
			get { return uploadControl; }
		}
		public FileDataEdit(ViewEditMode viewEditMode, IFileData fileData, bool readOnly, bool postDataImmediatelly) {
			this.fileData = fileData;
			this.viewEditMode = viewEditMode;
			this.readOnly = readOnly;
			this.postDataImmediatelly = postDataImmediatelly;
		}
		public event EventHandler<CreateCustomFileDataObjectEventArgs> CreateCustomFileDataObject;
		#region IXafCallbackHandler Members
		public void ProcessAction(string parameter) {
			if(parameter == "Clear") {
				FileData.Clear();
			}
		}
		#endregion
	}
}
