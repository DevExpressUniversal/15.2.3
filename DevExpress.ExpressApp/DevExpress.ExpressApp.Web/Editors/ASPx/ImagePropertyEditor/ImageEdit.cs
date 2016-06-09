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
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.Templates;
using DevExpress.ExpressApp.Web.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.ExpressApp.Web.Editors.ASPx {
	public class ImageChangedEventArgs : EventArgs {
		private byte[] imageBytes;
		public ImageChangedEventArgs(byte[] imageBytes) {
			this.imageBytes = imageBytes;
		}
		public byte[] ImageBytes { get { return imageBytes; } }
	}
	[DefaultProperty("Text")]
	[ToolboxData("<{0}:ImageEdit runat=server></{0}:ImageEdit>")]
	[ToolboxItem(false)]
	public class ImageEdit : Panel, INamingContainer, IPictureHolder, IXafCallbackHandler {
		public const string EmptyImageUrl = "empty";
		private enum LeadingProperty {
			Width,
			Height,
			Unassigned
		}
		private bool readOnly;
		private ImagePropertyEditorStorageMode storageMode;
		private bool postDataImmediatelly;
		private bool visibleOnRender;
		private int customImageHeight;
		private int customImageWidth;
		private System.Drawing.Image image;
		private Panel innerPlaceholder;
		private Panel uploadControlHolder;
		private DevExpress.Persistent.Base.ImageSizeMode imageSizeMode;
		private Image imageControl;
		private ASPxButton changeButton;
		private ASPxButton clearButton;
		private WebControl imageContainer;
		private WebControl imageContainerWrapper;
		private Table buttonsTable;
		private ASPxUploadControl uploadControl;
		private void OnImageChanged(byte[] imageBytes) {
			if(ImageChanged != null) {
				ImageChanged(this, new ImageChangedEventArgs(imageBytes));
			}
		}
		private ASPxButton CreateButton(string imageName, string caption, string id) {
			ASPxButton button = RenderHelper.CreateASPxButton();
			button.CssClass = "xafLookupButton";
			button.ID = id;
			DevExpress.ExpressApp.Utils.ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(imageName);
			ASPxImageHelper.SetImageProperties(button.Image, imageInfo, imageName);
			if(imageInfo.IsEmpty) {
				button.Text = caption;
			}
			button.ToolTip = caption;
			return button;
		}
		private Panel CreateUploadControlHolder() {
			uploadControl = new ASPxUploadControl();
			uploadControl.ID = "UPC";
			uploadControl.ShowUploadButton = false;
			uploadControl.ShowProgressPanel = true;
			uploadControl.ValidationSettings.AllowedFileExtensions = new string[] { ".jpe", ".jpeg", ".jpg", ".bmp", ".gif", ".png" };
			uploadControl.FileUploadComplete += uploadControl_FileUploadComplete;
			Panel uploadControlHolder = new Panel();
			uploadControlHolder.ID = "UPCH";
			uploadControlHolder.Controls.Add(uploadControl);
			return uploadControlHolder;
		}
		private void uploadControl_FileUploadComplete(object sender, FileUploadCompleteEventArgs e) {
			if(e.UploadedFile != null && !string.IsNullOrEmpty(e.UploadedFile.FileName) && e.UploadedFile.ContentLength > 0) {
				e.CallbackData = ProcessUploadResult(e.UploadedFile.FileBytes);
			}
		}
		private Image CreateImageControl() {
			Image imageControl = new Image();
			imageControl.ID = "IC";
			imageControl.EnableViewState = false;
			return imageControl;
		}
		private void ApplyRestrictions() {
			if(imageSizeMode == DevExpress.Persistent.Base.ImageSizeMode.Zoom) {
				RestrictImageControl(imageControl);
			}
			else {
				if(HasCustomHeight) {
					ImageContainer.Style.Add("max-height", Unit.Pixel(customImageHeight).ToString());
					imageContainerWrapper.Style.Add("max-height", Unit.Pixel(customImageHeight).ToString());
				}
				if(HasCustomWidth) {
					ImageContainer.Style.Add("max-width", Unit.Pixel(customImageWidth).ToString());
					imageContainerWrapper.Style.Add("max-width", Unit.Pixel(customImageWidth).ToString());
				}
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		private WebControl ImageContainer {
			get {
				return readOnly ? imageContainer : innerPlaceholder;
			}
		}
		private Table CreateButtonsTable() {
			Table table = RenderHelper.CreateTable();
			table.ID = "BT";
			table.BorderWidth = 0;
			table.CellPadding = 0;
			table.CellSpacing = 0;
			TableRow tableRow = new TableRow();
			TableCell changeButtonCell = new TableCell();
			changeButton = CreateButton("Editor_Edit", CaptionHelper.GetLocalizedText(ASPxImagePropertyEditor.LocalizationGroupName, "Edit", "Edit"), "Edit");
			changeButtonCell.Controls.Add(changeButton);
			TableCell clearButtonCell = new TableCell();
			clearButton = CreateButton("Editor_Clear", CaptionHelper.GetLocalizedText(ASPxImagePropertyEditor.LocalizationGroupName, "Clear", "Clear"), "Clear");
			clearButtonCell.Controls.Add(clearButton);
			tableRow.Cells.Add(changeButtonCell);
			tableRow.Cells.Add(clearButtonCell);
			table.Rows.Add(tableRow);
			return table;
		}
		private WebControl CreateImageContainer() {
			imageControl = CreateImageControl();
			if(!readOnly) {
				Table imageTable = RenderHelper.CreateTable();
				imageTable.CellPadding = 0;
				imageTable.CellSpacing = 0;
				imageTable.Rows.Add(new TableRow());
				TableCell outerPlaceholder = new TableCell();
				innerPlaceholder = new Panel();
				innerPlaceholder.Style.Add("overflow", "hidden");
				innerPlaceholder.ID = "innerPH";
				innerPlaceholder.Controls.Add(imageControl);
				outerPlaceholder.Controls.Add(innerPlaceholder);
				imageTable.Rows[0].Cells.Add(outerPlaceholder);
				return imageTable;
			}
			else {
				WebControl controlContainer = new WebControl(HtmlTextWriterTag.Div);
				controlContainer.Style.Add("overflow", "hidden");
				controlContainer.Controls.Add(imageControl);
				return controlContainer;
			}
		}
		private void UpdateChildControlsVisibility() {
			bool isEnabled = IsEnabled && !readOnly;
			if(!readOnly) {
				uploadControl.Visible = isEnabled;
				buttonsTable.Visible = isEnabled;
			}
			if(isEnabled) {
				imageControl.Attributes.Add("onload", string.Format("ShowHideImageControlEditMode(\"{0}\", \"{1}\", \"{2}\", true)", innerPlaceholder.ClientID, uploadControlHolder.ClientID, buttonsTable.ClientID));
				imageControl.Attributes.Add("onerror", string.Format("ShowHideImageControlEditMode(\"{0}\", \"{1}\", \"{2}\", false)", innerPlaceholder.ClientID, uploadControlHolder.ClientID, buttonsTable.ClientID));
				innerPlaceholder.Style["Display"] = visibleOnRender ? "" : "none";
				uploadControlHolder.Style["Display"] = "none";
			}
			else {
				imageControl.Attributes.Add("onload", string.Format("SetControlVisibility(\"{0}\", true)", ImageContainer.ClientID));
				imageControl.Attributes.Add("onerror", string.Format("SetControlVisibility(\"{0}\", false)", ImageContainer.ClientID));
			}
		}
		private void RestrictImageControl(Image imageControl) {
			DevExpress.ExpressApp.Utils.Guard.ArgumentNotNull(imageControl, "imageControl");
			if(storageMode == ImagePropertyEditorStorageMode.MediaData || storageMode == ImagePropertyEditorStorageMode.ByteArray) {
				if(HasCustomHeight) {
					ImageContainer.Style.Add("height", Unit.Pixel(customImageHeight).ToString());
					imageContainerWrapper.Style.Add("height", Unit.Pixel(customImageHeight).ToString());
					imageControl.Style.Add("max-height", Unit.Percentage(100).ToString());
				}
				if(HasCustomWidth) {
					ImageContainer.Style.Add("width", Unit.Pixel(customImageWidth).ToString());
					imageContainerWrapper.Style.Add("width", Unit.Pixel(customImageWidth).ToString());
					imageControl.Style.Add("max-width", Unit.Percentage(100).ToString());
				}
			}
			else {
				if(image != null) {
					LeadingProperty leadingProperty = LeadingProperty.Unassigned;
					if(HasCustomHeight && HasCustomWidth) {
						if((float)image.Height / image.Width > (float)CustomImageHeight / CustomImageWidth)
							leadingProperty = LeadingProperty.Height;
						else
							leadingProperty = LeadingProperty.Width;
					}
					else
						if(HasCustomHeight)
							leadingProperty = LeadingProperty.Height;
						else
							if(HasCustomWidth)
								leadingProperty = LeadingProperty.Width;
					switch(leadingProperty) {
						case LeadingProperty.Width:
							imageControl.Style["width"] = Unit.Pixel(customImageWidth).ToString();
							imageControl.Style["height"] = null;
							break;
						case LeadingProperty.Height:
							imageControl.Style["height"] = Unit.Pixel(customImageHeight).ToString();
							imageControl.Style["width"] = null;
							break;
					}
				}
			}
		}
		private bool HasCustomHeight {
			get {
				return customImageHeight > 0;
			}
		}
		private bool HasCustomWidth {
			get {
				return customImageWidth > 0;
			}
		}
		private string GetImageUrl(System.Drawing.Image image) {
			return image != null ? ImageResourceHttpHandler.GetWebResourceUrl(GetImageKey(image)) : EmptyImageUrl;
		}
		private string GetImageKey(System.Drawing.Image image) {
			return "IC_" + WebImageHelper.GetImageHash(image);
		}
		private void AddImage(System.Drawing.Image image) {
			lock(images) {
				string imageKey = GetImageKey(image);
				if(!images.ContainsKey(imageKey)) {
					images.Add(imageKey, image);
				}
			}
		}
		protected string ProcessUploadResult(byte[] imageBytes) {
			string result = string.Empty;
			if(storageMode == ImagePropertyEditorStorageMode.Image) {
				Image = System.Drawing.Image.FromStream(new MemoryStream(imageBytes));
				AddImage(Image);
				imageControl.ImageUrl = GetImageUrl(Image);
				OnImageChanged(null);
			}
			else {
				OnImageChanged(imageBytes);
			}
			ApplyRestrictions();
			string clientId = ImageContainer.ClientID;
			UpdateChildControlsVisibility();
			if(!postDataImmediatelly) {  
				result = string.IsNullOrEmpty(clientId) ? string.Empty : string.Format(@"{0}|{1}", clientId, RenderUtils.GetRenderResult(imageControl));
			}
			return result;
		}
		protected virtual void SubscribeClientSideEvents(XafCallbackManager callbackManager) {
			changeButton.ClientSideEvents.Click = string.Format("function(s, e) {{ SetControlVisibility(\"{0}\", !GetControlVisible(\"{0}\")); }}", uploadControlHolder.ClientID);
			clearButton.ClientSideEvents.Click = string.Format("function (s, e){{{0}}}", callbackManager.GetScript(UniqueID, "'Clear'"));
			uploadControl.ClientSideEvents.TextChanged = ASPxUploadControlHelper.GetTextChangedScript(postDataImmediatelly, callbackManager);
			uploadControl.ClientSideEvents.FileUploadComplete = string.Format(@"function (s, e){{
                if(xaf.ConfirmUnsavedChangedController){{
                    xaf.ConfirmUnsavedChangedController.EditorValueChanged(s, e);
                }}
                if (e.callbackData && e.callbackData != ''){{
                        var controlId = e.callbackData.split('|')[0];
                        if(controlId != null && controlId != '') {{
                            var markup = e.callbackData.substring(controlId.length + 1);
                            var control = document.getElementById(controlId);
                            if (control) {{
                                ASPx.SetInnerHtml(control, markup);
                            }}
                        }}
                    }}
                }}");
		}
		static ImageEdit() {
			ImageResourceHttpHandler.QueryImageInfo += new EventHandler<ImageInfoEventArgs>(ImageResourceHttpHandler_QueryImageInfo);
		}
		static void ImageResourceHttpHandler_QueryImageInfo(object sender, ImageInfoEventArgs e) {
			if(e.Url.StartsWith("IC")) {
				lock(images) {
					if(images.ContainsKey(e.Url)) {
						System.Drawing.Image image = images[e.Url];
						e.ImageInfo = new DevExpress.ExpressApp.Utils.ImageInfo("", image, "" );
						images.Remove(e.Url);
					}
				}
			}
		}
		private static Dictionary<string, System.Drawing.Image> images = new Dictionary<string, System.Drawing.Image>();
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			ICallbackManagerHolder holder = Page as ICallbackManagerHolder;
			if(holder != null) {
				holder.CallbackManager.RegisterHandler(UniqueID, this);
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			PrepareChildControlsOnLoad();
		}
		protected virtual void PrepareChildControlsOnLoad() {
			if(!readOnly && Page != null) {
				Guard.TypeArgumentIs(typeof(ICallbackManagerHolder), Page.GetType(), "Page");
				ICallbackManagerHolder holder = Page as ICallbackManagerHolder;
				SubscribeClientSideEvents(holder.CallbackManager);
			}
			UpdateChildControlsVisibility();
			ApplyRestrictions();
		}
		public ImageEdit(bool readOnly, ImagePropertyEditorStorageMode storageMode) : this(readOnly, storageMode, false) { }
		public ImageEdit(bool readOnly, ImagePropertyEditorStorageMode storageMode, bool postDataImmediatelly) {
			this.readOnly = readOnly;
			this.storageMode = storageMode;
			this.postDataImmediatelly = postDataImmediatelly;
			imageContainer = CreateImageContainer();
			imageContainerWrapper = new WebControl(HtmlTextWriterTag.Div);
			imageContainerWrapper.Controls.Add(imageContainer);
			Controls.Add(imageContainerWrapper);
			if(!readOnly) {
				uploadControlHolder = CreateUploadControlHolder();
				buttonsTable = CreateButtonsTable();
				Controls.Add(buttonsTable);
				Controls.Add(uploadControlHolder);
			}
		}
		public override void Dispose() {
			if(clearButton != null) {
				clearButton.Dispose();
				clearButton = null;
			}
			if(changeButton != null) {
				changeButton.Dispose();
				changeButton = null;
			}
			if(uploadControl != null) {
				uploadControl.FileUploadComplete -= new EventHandler<FileUploadCompleteEventArgs>(uploadControl_FileUploadComplete);
				uploadControl.Dispose();
				uploadControl = null;
			}
			if(imageContainer != null) {
				imageContainer.Dispose();
				imageContainer = null;
			}
			if(imageContainerWrapper != null) {
				imageContainerWrapper.Dispose();
				imageContainerWrapper = null;
			}
			if(buttonsTable != null) {
				buttonsTable.Dispose();
				buttonsTable = null;
			}
			base.Dispose();
		}
		public void SetControlImageUrl(string imageUrl, bool visibleOnRender) {
			if(!String.IsNullOrEmpty(imageUrl)) {
				imageControl.ImageUrl = BinaryDataHttpHandler.GetHandlerUrl(imageUrl);
			}
			else {
				imageControl.ImageUrl = String.Empty;
			}
			this.visibleOnRender = visibleOnRender;
		}
		public DevExpress.Persistent.Base.ImageSizeMode ImageSizeMode {
			get {
				return imageSizeMode;
			}
			set {
				imageSizeMode = value;
			}
		}
		public int CustomImageHeight {
			get {
				return customImageHeight;
			}
			set {
				customImageHeight = value;
			}
		}
		public int CustomImageWidth {
			get {
				return customImageWidth;
			}
			set {
				customImageWidth = value;
			}
		}
		public System.Drawing.Image Image {
			get { return image; }
			set {
				image = value;
				imageControl.ImageUrl = GetImageUrl(Image);
				visibleOnRender = image != null;
				AddImage(Image);
				ApplyRestrictions();
			}
		}
#if DebugTest
		public WebControl ImageContainer_ForTests {
			get {
				return ImageContainer;
			}
		}
		public string DebugTest_GetImageUrl(System.Drawing.Image image) {
			return GetImageUrl(image);
		}
		public void DebugTest_ProcessUploadResults(byte[] imageBytes) {
			ProcessUploadResult(imageBytes);
		}
		public Image ImageControl { get { return imageControl; } }
		public ASPxUploadControl UploadControl { get { return uploadControl; } }
		public Panel UploadControlHolder { get { return uploadControlHolder; } }
		public Panel InnerPlaceholder { get { return innerPlaceholder; } }
		public ASPxButton ChangeButton { get { return changeButton; } }
		public ASPxButton ClearButton { get { return clearButton; } }
		public Table ButtonsTable { get { return buttonsTable; } }
#endif
		public event EventHandler<ImageChangedEventArgs> ImageChanged;
		public void ProcessAction(string parameter) {
			if(parameter == "Clear") {
				if(storageMode == ImagePropertyEditorStorageMode.Image) {
					image = null;
				}
				OnImageChanged(null);
			}
		}
	}
}
