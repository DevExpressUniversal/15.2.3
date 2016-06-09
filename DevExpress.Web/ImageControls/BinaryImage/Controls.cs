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
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web.Internal {
	public class BinaryImageDisplayControl: ImageControl<ASPxBinaryImage>, IBinaryImageEditor {
		protected WebControl SpanElement { get; private set; }
		protected IValueProvider ValueProvider { get; private set; }
		protected internal BinaryImageEditProperties BinaryProperties { get; private set; }
		protected virtual bool AssignControlID { get { return !EditMode; } }
		protected bool EditMode { get { return Control != null && Control.AllowEdit; } }
		protected byte[] ContentBytes { get; set; }
		protected internal string Digest {
			get {
				BinaryStorageData bindata = BinaryStorage.GetResourceData(this, BinaryProperties.BinaryStorageMode, GetKeyName("digest"));
				return bindata != null ? Encoding.ASCII.GetString(GetContentFromBinaryData(bindata)) : string.Empty;
			}
			set {
				BinaryStorageData bindata = new BinaryStorageData(Encoding.ASCII.GetBytes(value), "text/plain");
				BinaryStorage.StoreResourceData(this, BinaryProperties.BinaryStorageMode, GetKeyName("digest"), bindata);
			}
		}
		protected override bool NeedRenderSizes { get { return !CanImageResize(); } }
		public BinaryImageDisplayControl(ASPxBinaryImage control)
			: this(control.Properties, null) {
			Control = control;
		}
		public BinaryImageDisplayControl(BinaryImageEditProperties properties, byte[] content, IValueProvider valueProvider = null)
			: base() {
			BinaryProperties = properties;
			ContentBytes = content;
			Width = GetCorrectedUnit(BinaryProperties.ImageWidth);
			Height = GetCorrectedUnit(BinaryProperties.ImageHeight);
			ValueProvider = valueProvider;
		}
		public override bool AssignEmptyImage { get { return !EditMode; } }		
		public byte[] Content { get { return Control != null ? Control.Value as byte[] : ContentBytes; } }
		protected Unit GetCorrectedUnit(Unit value) {
			return value.Value == 0 ? Unit.Empty : value;
		}
		protected virtual bool CanRenderSpanElement() {
			return CanImageResize() && BinaryProperties.ImageSizeMode == ImageSizeMode.ActualSizeOrFit;
		}
		internal protected bool CanImageResize() {
			return BinaryProperties.EnableServerResize && !EditMode && Content != null && Content.Length > 0 && HasImageSize();
		}
		protected bool HasImageSize() {
			return IsImageSizesInPixelFormat() && (!Width.IsEmpty || !Height.IsEmpty);
		}
		protected bool IsImageSizesInPixelFormat() {
			return Width.Type == UnitType.Pixel && Height.Type == UnitType.Pixel;
		}
		protected byte[] GetContentFromBinaryData(BinaryStorageData bindata) {
			return bindata.Content ?? new byte[0];
		}
		protected override string GetImageUrl() {
			return GetResizedImageUrl();
		}
		protected internal string GetResizedImageUrl() {
			bool canImageResize = CanImageResize();
			if(DesignMode)
				return BinaryProperties.Images.GetImageProperties(Page, EditorImages.BinaryImageDesignImageName).Url;
			if(canImageResize)
				ResizeImage();
			return BinaryStorage.GetImageUrl(this, Content, BinaryProperties.BinaryStorageMode, canImageResize);
		}
		protected bool IsChangedOriginImage() {
			string currentDigest = Digest;
			Digest = string.Format("{0}{1}{2}{3}", (int)Width.Value, (int)Height.Value, (int)BinaryProperties.ImageSizeMode, CommonUtils.GetMD5Hash(Content ?? new byte[0]));
			return currentDigest != Digest;
		}
		protected void ResizeImage() {
			if(IsChangedOriginImage())
				ResizeImageCore();
		}
		protected virtual void ResizeImageCore() {
			byte[] resizedImage = CreateThumbnailImage();
			BinaryStorageData data = new BinaryStorageData(resizedImage, BinaryStorage.GetImageMimeType(Content));
			BinaryStorage.StoreResourceData(this, BinaryProperties.BinaryStorageMode, Digest, data);
		}
		protected bool IsEqualSizes(Size originalSizes, Size resizedSizes) {
			return (originalSizes.Width < resizedSizes.Width && originalSizes.Height < resizedSizes.Height && BinaryProperties.ImageSizeMode == ImageSizeMode.ActualSizeOrFit) ||
				(resizedSizes.Width == 0 && originalSizes.Height == resizedSizes.Height) ||
				(resizedSizes.Height == 0 && originalSizes.Width == resizedSizes.Width) ||
				(originalSizes.Width == resizedSizes.Width && originalSizes.Height == resizedSizes.Height);
		}
		protected byte[] CreateThumbnailImage() {
			using(Bitmap originalImage = (Bitmap)System.Drawing.Image.FromStream(new MemoryStream(Content))) {
				byte[] image = null;
				int height = (int)Height.Value;
				int width = (int)Width.Value;
				int resizedImageWidth = originalImage.Width;
				int resizedImageHeight = originalImage.Height;
				if(IsEqualSizes(new Size { Width = originalImage.Width, Height = originalImage.Height }, new Size { Width = width, Height = height })) 
					image = Content;
				else {
					Bitmap resizedImage = ImageUtils.CreateThumbnailImage(originalImage, BinaryProperties.ImageSizeMode, new Size { Width = width, Height = height });
					ImageConverter converter = new ImageConverter();
					image = (byte[])converter.ConvertTo(resizedImage, typeof(byte[]));
					resizedImageWidth = resizedImage.Width;
					resizedImageHeight = resizedImage.Height;
				}
				SaveResizedImageSizes(string.Format("{0}|{1}", resizedImageWidth, resizedImageHeight));
				return image;
			}
		}
		protected string GetKeyName(string key) {
			return string.Format("{0}_{1}", BinaryStorage.GetResourceKeyForResizedImage(this, Content, BinaryProperties.BinaryStorageMode), key);
		}
		protected Size GetResizedImageSizes() {
			BinaryStorageData bindata = BinaryStorage.GetResourceData(this, BinaryProperties.BinaryStorageMode, GetKeyName("resizedSizes"));
			string sizesString = bindata != null ? Encoding.ASCII.GetString(GetContentFromBinaryData(bindata)) : string.Empty;
			string[] sizes = sizesString.Split('|');
			Size restoredSizes = new Size();
			if(!string.IsNullOrEmpty(sizesString)) {
				restoredSizes.Width = Convert.ToInt32(sizes[0]);
				restoredSizes.Height = Convert.ToInt32(sizes[1]);
			}
			return restoredSizes;
		}
		protected void SaveResizedImageSizes(string value) {
			BinaryStorageData bindata = new BinaryStorageData(Encoding.ASCII.GetBytes(value), "text/plain");
			BinaryStorage.StoreResourceData(this, BinaryProperties.BinaryStorageMode, GetKeyName("resizedSizes"), bindata);
		}
		private int GetMarginLeftRight() {
			double imageWidth = Width.Value;
			double spanCenter = imageWidth / 2;
			double imageCenter = GetResizedImageSizes().Width / 2;
			return (int)(spanCenter == 0 ? imageWidth : spanCenter - imageCenter);
		}
		private int GetMarginTopBottom() {
			double imageHeight = Height.Value;
			double spanCenter = imageHeight / 2;
			double imageCenter = GetResizedImageSizes().Height / 2;
			return (int)(spanCenter == 0 ? imageHeight : spanCenter - imageCenter);
		}
		protected override void CreateControlHierarchy() {
			Image = RenderUtils.CreateImage();
			if(CanRenderSpanElement()) {
				SpanElement = RenderUtils.CreateWebControl(HtmlTextWriterTag.Span);
				if(AssignControlID && Control != null)
					SpanElement.ID = Control.ID;
				SpanElement.Controls.Add(Image);
				Controls.Add(SpanElement);
			} else
				Controls.Add(Image);
		}
		protected void SetMarginsToImageIfRequired() {
			int marginLeft = GetMarginLeftRight();
			int marginRight = GetMarginLeftRight();
			int marginTop = GetMarginTopBottom();
			int marginBottom = GetMarginTopBottom();
			int canculatedControlWidth = marginLeft + marginRight + GetResizedImageSizes().Width;
			int canculatedControlHeight = marginBottom + marginTop + GetResizedImageSizes().Height;
			if(Width.Value != 0 && canculatedControlWidth > Width.Value)
				marginRight--;
			if(Height.Value != 0 && canculatedControlHeight > Height.Value)
				marginBottom--;
			if(marginLeft + marginRight != 0) {
				Image.Style[HtmlTextWriterStyle.MarginLeft] = marginLeft + "px";
				Image.Style[HtmlTextWriterStyle.MarginRight] = marginRight + "px";
			}
			if(marginBottom + marginTop != 0) {
				Image.Style[HtmlTextWriterStyle.MarginTop] = marginTop + "px";
				Image.Style[HtmlTextWriterStyle.MarginBottom] = marginBottom + "px";
			}
		}
		protected override void PrepareControlHierarchy() {
			bool hasSpanElement = CanRenderSpanElement();
			if(Control != null) {
				if(hasSpanElement) {
					RenderUtils.AssignAttributes(Control, SpanElement, !AssignControlID, false, true, true);
					GetControlStyle().AssignToControl(SpanElement);
				} else
					GetControlStyle().AssignToControl(Image);
				RenderUtils.AssignAttributes(Control, Image, hasSpanElement || !AssignControlID, false, hasSpanElement, false);
				PrepareControlHierarchyInternal();
			} else
				SetProperties(GetResizedImageUrl(), string.Empty, BinaryProperties, ValueProvider);
			MakeAdjustmentsToElementsIfRequired();
			ImageProperties.AssignToControl(Image, DesignMode); 
			if(EditMode && !DesignMode) {
				Image.ID = ASPxBinaryImage.PreviewImageID;
				Image.Height = Unit.Empty;
				Image.Width = Unit.Empty;
				Image.Style["height"] = "auto";
				Image.Style["width"] = "auto";
			}
		}
		protected void MakeAdjustmentsToElementsIfRequired() {
			if(CanRenderSpanElement()) {
				SetMarginsToImageIfRequired();
				if(SpanElement.ControlStyle.BorderStyle == BorderStyle.NotSet || SpanElement.ControlStyle.BorderWidth.IsEmpty)
					SpanElement.Style[HtmlTextWriterStyle.Display] = "inline-block";
				SpanElement.Style[HtmlTextWriterStyle.FontSize] = "0px";
				SpanElement.Attributes.Remove("alt");
				Image.CssClass = string.Empty;
				SpanElement.CssClass = GetClassName();
			}
		}
		protected string GetClassName() {
			return Control != null ? Control.CssClass : BinaryProperties.Style.CssClass;
		}
		protected virtual AppearanceStyleBase GetControlStyle() {
			if(EditMode)
				return AppearanceStyle.Empty;
			AppearanceStyle style = new AppearanceStyle();
			if(Control != null)
				style.CopyFrom(Control.GetControlStyle());
			return style;
		}
		IBinaryImageEditor BinaryImageEditor { get { return Control; } }
		bool IBinaryImageEditor.AllowEdit { get { return EditMode; } }
		int IBinaryImageEditor.TempFileExpirationTime { get { return BinaryImageEditor.TempFileExpirationTime; } }
		string IBinaryImageEditor.TempFolder { get { return BinaryImageEditor.TempFolder; } }
	}
	public class BinaryImageButtonControl : SimpleButtonControl {
		protected internal new HyperLink Hyperlink {
			get { return base.Hyperlink; }
		}
		public BinaryImageButtonControl()
			: this(String.Empty) {
		}
		public BinaryImageButtonControl(string text)
			: base() {
			this.ButtonText = text;
			this.ButtonUrl = RenderUtils.AccessibilityEmptyUrl;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(Hyperlink != null) {
				RenderUtils.AssignAttributes(this, Hyperlink);
				if(ButtonStyle != null && !ButtonStyle.IsEmpty)
					ButtonStyle.AssignToControl(Hyperlink, AttributesRange.All);
			}
		}
	}
	public class BinaryImageButtonPanelControl : BinaryImageContentControl, INamingContainer {
		const int minFadedOpacity = 60;
		protected internal const string ButtonsShaderID = "DXButtonsShader",
										DeleteButtonID = "DXDeleteButton",
										OpenDialogButtonID = "DXOpenDialogButton";
		public List<BinaryImageButtonControl> Buttons { get; private set; }
		public WebControl ButtonsShader { get; private set; }
		public BinaryImageButtonControl DeleteButton { get; private set; }
		public BinaryImageButtonControl OpenDialogButton { get; private set; }
		public override AppearanceStyleBase RootStyle { get { return Owner.GetButtonPanelStyle(); } }
		public BinaryImageButtonPanelControl(ASPxBinaryImage binaryImage)
			: base(binaryImage) { }
		bool IsDeleteButtonVisible() {
			return Owner.Value != null;
		}
		protected override void ClearContentControls() {
			OpenDialogButton = null;
			DeleteButton = null;
			ButtonsShader = null;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateButtonsShader(this);
		}
		protected void CreateButtonsShader(WebControl container) {
			ButtonsShader = RenderUtils.CreateDiv(EditorStyles.BinaryImageButtonShaderSystemClassName, EditorStyles.FillParentSystemClassName);
			ButtonsShader.ID = ButtonsShaderID;
			container.Controls.AddAt(0, ButtonsShader);
		}
		protected override void CreateContentHierarchy(WebControl container) {
			CreateButtons(container);
		}		
		protected override AppearanceStyleBase GetMainTableStyle() {
			var style = base.GetMainTableStyle();
			RenderUtils.AppendCssClass(style, EditorStyles.FillParentSystemClassName);
			return style;
		}
		protected void CreateButtons(WebControl container) {
			OpenDialogButton = new BinaryImageButtonControl {
				ID = OpenDialogButtonID,
				ButtonImage = Owner.GetOpenDialogButtonImage(),
				ButtonStyle = Owner.GetButtonStyle(),
			};
			DeleteButton = new BinaryImageButtonControl {
				ID = DeleteButtonID,
				ButtonImage = Owner.GetDeleteButtonImage(),
				ButtonStyle = Owner.GetButtonStyle(),
			};
			Buttons = new List<BinaryImageButtonControl> { OpenDialogButton, DeleteButton };
			Buttons.ForEach(container.Controls.Add);
		}
		protected override void PrepareRootControl() {
			base.PrepareRootControl();
			if(Owner.ButtonPanelSettings.Visibility == ElementVisibilityMode.OnMouseOver)
				RenderUtils.SetOpacity(this, 0);
		}
		protected override void PrepareContentHierarchy() {
			PrepareDeleteButton();
			PrepareButtonsShader();
		}
		protected void PrepareDeleteButton() {
			RenderUtils.SetVisibility(DeleteButton, IsDeleteButtonVisible(), true);
		}
		protected void PrepareButtonsShader() {
			if(Owner.ButtonPanelSettings.Visibility == ElementVisibilityMode.Faded)
				RenderUtils.SetOpacity(ButtonsShader, minFadedOpacity);
			if(DesignMode)
				ButtonsShader.BackColor = Color.Black;
		}
	}
	public class BinaryImageDropZone : BinaryImageTextPanelControl {
		public BinaryImageDropZone(ASPxBinaryImage binaryImage) : base(binaryImage) { }
		public override AppearanceStyleBase RootStyle { get { return Owner.GetDropZoneStyle(); } }
		public override string GetText() { return Owner.Properties.EditingSettings.DropZoneText; }
		protected override void PrepareRootControl() {
			base.PrepareRootControl();
			RenderUtils.SetVisibility(this, false, true);
		}
	}
	public class BinaryImageEmptyValuePlaceholder : BinaryImageTextPanelControl {
		protected internal const string EmptyValueImage = "EVI";
		protected System.Web.UI.WebControls.Image EmptyImage { get; private set; }
		protected ImageProperties EmptyImageProperties { get { return Owner.EmptyImage; } }
		protected bool ShowEmptyImage { get { return !EmptyImageProperties.IsEmpty; } }
		public BinaryImageEmptyValuePlaceholder(ASPxBinaryImage binaryImage) : base(binaryImage) { }
		public override AppearanceStyleBase RootStyle { get { return Owner.GetEmptValueTextStyle(); } }
		protected override void ClearContentControls() {
			base.ClearContentControls();
			EmptyImage = null;
		}
		protected override void CreateContentHierarchy(WebControl container) {
			base.CreateContentHierarchy(container);
			EmptyImage = RenderUtils.CreateImage();
			EmptyImage.ID = EmptyValueImage;
			container.Controls.Add(EmptyImage);
		}
		public override string GetText() { return Owner.GetEmptyValueText(); }
		protected override void PrepareContentHierarchy() {
			PrepareEmptyImage();
			if(!ShowEmptyImage)
				PrepareTextContainer();
		}
		protected void PrepareEmptyImage() {
			EmptyImageProperties.AssignToControl(EmptyImage, DesignMode);
			SetImageDesignModeSizes();
			RenderUtils.SetVisibility(EmptyImage, ShowEmptyImage, true);
		}
		protected void SetImageDesignModeSizes(){
			var needToResize = DesignMode && !String.IsNullOrEmpty(EmptyImageProperties.Url) ;
			if(needToResize) {
				if(EmptyImage.Width.IsEmpty) 
					EmptyImage.Width = Owner.Width.IsEmpty ? BinaryImageMainControl.MinWidth : Owner.Width;
				if(EmptyImage.Height.IsEmpty)
					EmptyImage.Height = Owner.Height.IsEmpty ? BinaryImageMainControl.MinHeight : Owner.Height;
			}
		}
	}
	public class BinaryImageMainControl : BinaryImageContentControl {
		protected internal const string
			 ButtonPanelID = "DXButtonPanel",
			 DisabledCoverID = "DXDisabledCover",
			 EmptyValuePlaceholderID = "EVP",
			 DropZoneID = "DXDropZone",
			 PreviewContainerID = "DXImageContainer",
			 ProgressPanelID = "DXProgressPanel",
			 UploadEditorID = "DXUploadEditor",
			 ValueKeyInputID = "DXValueKeyInput";
		public static Unit MinWidth = Unit.Pixel(150);
		public static Unit MinHeight = Unit.Pixel(150);
		public BinaryImageMainControl(ASPxBinaryImage owner)
			: base(owner) {
		}
		public BinaryImageButtonPanelControl ButtonPanel { get; private set; }
		public WebControl ContentContainer { get; private set; }
		public WebControl DisabledCover { get; private set; }
		public BinaryImageDropZone DropZonePlaceholder { get; private set; }
		public WebControl ImageContainer { get; private set; }
		public BinaryImageDisplayControl ImageControl { get; private set; }
		public BinaryImageEmptyValuePlaceholder EmptyValuePlaceholder { get; set; }
		public BinaryImageProgressPanelControl ProgressPanel { get; private set; }
		public ASPxUploadControl UploadControl { get; set; }
		public WebControl ValueKeyHiddenInput { get; private set; }
		public bool AllowDropOnPreview { get { return Owner.AllowDropOnPreview; } }
		public bool EditMode { get { return Owner.AllowEdit; } }
		public bool ShowButtonPanel { get { return Owner.ShowButtonPanel; } }
		public bool ShowEmptyValuePlaceHolder { get { return Owner.Value == null; } }
		protected override void ClearContentControls() {
			ButtonPanel = null;
			ContentContainer = null;
			DisabledCover = null;
			DropZonePlaceholder = null;
			ImageContainer = null;
			ImageControl = null;
			EmptyValuePlaceholder = null;
			ProgressPanel = null;
			UploadControl = null;
			ValueKeyHiddenInput = null;
		}
		protected override void CreateControlHierarchy() {
			if(EditMode)
				base.CreateControlHierarchy();
			else
				CreateImageControl(this);
		}
		protected override void CreateContentHierarchy(WebControl container) {
			ContentContainer = RenderUtils.CreateDiv(EditorStyles.BinaryImageContentContainerSystemClassName);
			container.Controls.Add(ContentContainer);
			CreateImageContainer(ContentContainer);
			CreateValueKeyHiddenInput(ContentContainer);
			CreateUploadControl(ContentContainer);
			CreateEmptyValuePlaceHolder(ContentContainer);
			CreateButtonPanel(ContentContainer);
			CreateProgressPanel(ContentContainer);
			CreateDisabledCover(ContentContainer);
		}
		protected void CreateDisabledCover(WebControl container) {
			DisabledCover = RenderUtils.CreateDiv(EditorStyles.BinaryImageDisabledCoverSystemClassName, EditorStyles.FillParentSystemClassName);
			DisabledCover.ID = DisabledCoverID;
			container.Controls.Add(DisabledCover);
		}
		protected void CreateProgressPanel(WebControl container) {
			ProgressPanel = new BinaryImageProgressPanelControl(Owner);
			ProgressPanel.ID = ProgressPanelID;
			container.Controls.Add(ProgressPanel);
		}
		protected void CreateButtonPanel(WebControl container) {
			if(ShowButtonPanel) {
				ButtonPanel = new BinaryImageButtonPanelControl(Owner) { ID = ButtonPanelID };
				container.Controls.Add(ButtonPanel);
			}
		}
		protected void CreateImageContainer(WebControl container) {
			ImageContainer = RenderUtils.CreateDiv(EditorStyles.BinaryImagePreviewContainerSystemClassName);
			ImageContainer.ID = PreviewContainerID;
			container.Controls.Add(ImageContainer);
			CreateImageControl(ImageContainer);
		}
		protected void CreateValueKeyHiddenInput(WebControl container) {
			ValueKeyHiddenInput = RenderUtils.CreateWebControl(HtmlTextWriterTag.Input);
			ValueKeyHiddenInput.ID = ValueKeyInputID;
			container.Controls.Add(ValueKeyHiddenInput);
		}
		protected void CreateUploadControl(WebControl container) {
			if(!DesignMode) {
				UploadControl = Owner.CreateUploadControl();
				container.Controls.Add(UploadControl);
				ConfigureUploadControl();
				if(AllowDropOnPreview)
					CreateUploadDropZone(container);
			}
		}
		protected void CreateUploadDropZone(WebControl container) {
			DropZonePlaceholder = new BinaryImageDropZone(Owner) { ID = DropZoneID };
			container.Controls.Add(DropZonePlaceholder);
		}
		protected virtual void ConfigureUploadControl() {
			UploadControl.ID = UploadEditorID;
			UploadControl.AdvancedModeSettings.EnableFileList = false;
			UploadControl.AdvancedModeSettings.EnableMultiSelect = false;
			UploadControl.FileUploadComplete += UploadControl_OnFileUploadComplete;
			UploadControl.AutoStartUpload = true;
			UploadControl.ShowUploadButton = false;
			UploadControl.ShowUI = false;
		}
		void UploadControl_OnFileUploadComplete(object sender, FileUploadCompleteEventArgs e) {
			Owner.OnFileUploadComplete(sender, e); 
		}
		private void CreateEmptyValuePlaceHolder(WebControl container) {
			EmptyValuePlaceholder = new BinaryImageEmptyValuePlaceholder(Owner);
			EmptyValuePlaceholder.ID = EmptyValuePlaceholderID;
			container.Controls.Add(EmptyValuePlaceholder);
		}
		protected override string GetMainTableCssClass() {
			return EditorStyles.DisplayInlineTableSystemClassName;
		}
		protected override AppearanceStyleBase GetMainTableStyle() {
			var style = base.GetMainTableStyle();
			style.CopyFrom(Owner.GetControlStyle());
			RenderUtils.AppendCssClass(style, EditorStyles.BinaryImageSystemClassName);
			return style;
		}
		public void CreateImageControl(WebControl container) {
			ImageControl = new BinaryImageDisplayControl(Owner);
			container.Controls.Add(ImageControl);
		}
		protected override bool HasRootTag() {
			return false;
		}
		protected override void PrepareControlHierarchy() {
			if(EditMode)
				base.PrepareControlHierarchy();
		}
		protected override void PrepareContentHierarchy() {
			PrepareContentContainer();
			PrepareValueKeyHiddenInput();
			PrepareUploadControl();
			PrepareEmptyValuePlaceHolder();
			RenderUtils.SetVisibility(ImageContainer, false, true);
			RenderUtils.SetVisibility(ProgressPanel, false, true);
			RenderUtils.SetVisibility(DisabledCover, false, true);
		}
		protected void PrepareContentContainer() {
			if(DesignMode) {
				ContentContainer.Width = MainTable.Width.IsEmpty ? MinWidth : MainTable.Width;
				ContentContainer.Height = MainTable.Height.IsEmpty ? MinHeight : MainTable.Height;
			}
		}
		protected void PrepareEmptyValuePlaceHolder() {
			RenderUtils.SetVisibility(EmptyValuePlaceholder, ShowEmptyValuePlaceHolder, true);
			RenderUtils.SetStringAttribute(EmptyValuePlaceholder, "onclick", Owner.GetOnClick());
		}
		protected void PrepareUploadControl() {
			if(UploadControl != null) {
				UploadControl.ValidationSettings.Assign(Owner.UploadSettings.UploadValidationSettings);
				if(AllowDropOnPreview) {
					UploadControl.AdvancedModeSettings.EnableDragAndDrop = true;
					UploadControl.AdvancedModeSettings.ExternalDropZoneID = Owner.ClientID;
				}
			}
		}
		protected void PrepareValueKeyHiddenInput() {
			RenderUtils.SetStringAttribute(ValueKeyHiddenInput, "type", "hidden");
			RenderUtils.SetStringAttribute(ValueKeyHiddenInput, "name", Owner.UniqueID);
			RenderUtils.SetStringAttribute(ValueKeyHiddenInput, "value", Owner.ServerValueKey);
		}
		protected override void PrepareMainTable() {
			base.PrepareMainTable();
			RenderUtils.AssignAttributes(Owner, MainTable);
		}
	}
	public abstract class BinaryImageTextPanelControl : BinaryImageContentControl {
		public LiteralControl TextContainer { get; private set; }
		public BinaryImageTextPanelControl(ASPxBinaryImage binaryImage)
			: base(binaryImage) {
		}
		protected override void ClearContentControls() {
			TextContainer = null;
		}
		protected override void CreateContentHierarchy(WebControl container) {
			TextContainer = new LiteralControl();
			container.Controls.Add(TextContainer);
		}
		protected override AppearanceStyleBase GetRootStyle() {
			var style = base.GetRootStyle();
			RenderUtils.AppendCssClass(style, EditorStyles.BinaryImageTextPanelSystemClassName);
			return style;
		}
		public abstract string GetText();
		protected override void PrepareContentHierarchy() {
			PrepareTextContainer();
		}
		protected virtual void PrepareTextContainer() {
			TextContainer.Text = GetText();
		}
	}
	public class BinaryImageProgressPanelControl :BinaryImageContentControl {
		static readonly Unit progressBarHeight = new Unit(0.5, UnitType.Em);
		static readonly Unit progressBarWidth = Unit.Percentage(100);
		protected internal const string
			CancelButtonID = "DXCancelButton",
			ProgressBarID = "DXProgressBar";
		public BinaryImageButtonControl CancelButton { get; private set; }
		public ASPxProgressBarBase ProgressBar { get; private set; }
		public WebControl ProgressBarContainer { get; private set; }
		public Label UploadLabel { get; private set; }
		public BinaryImageProgressPanelControl(ASPxBinaryImage binaryImage)
			: base(binaryImage) {
		}
		protected override void ClearContentControls() {
			ProgressBar = null;
			ProgressBarContainer = null;
			CancelButton = null;
			UploadLabel = null;
		}
		protected override void CreateContentHierarchy(WebControl container) {
			ProgressBarContainer = RenderUtils.CreateDiv(EditorStyles.BinaryImageProgressBarContainerSystemClassName);
			container.Controls.Add(ProgressBarContainer);
			CreateUploadLabel(ProgressBarContainer);
			CreateProgressBar(ProgressBarContainer);
			CreateCancelButton(ProgressBarContainer);
		}
		void CreateUploadLabel(WebControl container) {
			UploadLabel = RenderUtils.CreateLabel();
			UploadLabel.Text = ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.BinaryImage_Uploading);
			container.Controls.Add(UploadLabel);
		}
		void CreateProgressBar(WebControl container) {
			ProgressBar = new ASPxProgressBarBase();
			ProgressBar.ID = ProgressBarID;
			container.Controls.Add(ProgressBar);
		}
		void CreateCancelButton(WebControl container) {
			CancelButton = new BinaryImageButtonControl(ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.BinaryImage_Cancel));
			CancelButton.ID = CancelButtonID;
			container.Controls.Add(CancelButton);
		}
		protected override AppearanceStyleBase GetRootStyle() {
			var style = base.GetRootStyle();
			RenderUtils.AppendCssClass(style, EditorStyles.BinaryImageProgressPanelSystemClassName);
			RenderUtils.AppendCssClass(style, EditorStyles.FillParentSystemClassName);
			return style;
		}
		protected override void PrepareContentHierarchy() {
			PrepareProgressBar();
		}
		void PrepareProgressBar() {
			ProgressBar.ControlStyle.Reset();
			ProgressBar.IndicatorStyle.Reset();
			ProgressBar.ShowPosition = false;
			ProgressBar.Height = progressBarHeight;
			ProgressBar.Width = progressBarWidth;
			ProgressBar.ControlStyle.CopyFrom(Owner.GetProgressBarStyle());
			ProgressBar.IndicatorStyle.CopyFrom(Owner.GetProgressBarIndicatorStyle());
		}
	}
	public abstract class BinaryImageContentControl : ASPxInternalWebControl {
		public BinaryImageContentControl(ASPxBinaryImage owner) { Owner = owner; }
		protected override HtmlTextWriterTag TagKey { get { return HtmlTextWriterTag.Div; } }
		public WebControl MainTable { get; private set; }
		public WebControl ContentCell { get; private set; }
		public ASPxBinaryImage Owner { get; private set; }
		public virtual AppearanceStyleBase RootStyle { get { return null; } }
		protected override void ClearControlFields() {
			base.ClearControlFields();
			MainTable = null;
			ContentCell = null;
			ClearContentControls();
		}
		protected abstract void ClearContentControls();
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateMainTable(this);
			CreateContentCell(MainTable);
			CreateContentHierarchy(ContentCell);
		}
		protected virtual void CreateMainTable(WebControl container) {
			MainTable = DesignMode ? RenderUtils.CreateTable() : RenderUtils.CreateDiv();
			container.Controls.Add(MainTable);
		}
		protected virtual void CreateContentCell(WebControl table) {
			ContentCell = DesignMode ? RenderUtils.CreateTableCell() : RenderUtils.CreateDiv();
			if(DesignMode)
				RenderUtils.PutInControlsSequentially(table, RenderUtils.CreateTableRow(), ContentCell);
			else
				table.Controls.Add(ContentCell);			
		}
		protected abstract void CreateContentHierarchy(WebControl container);
		protected virtual string GetContentCellCssClass() {
			return EditorStyles.DisplayCellSystemClassName;
		}
		protected virtual AppearanceStyleBase GetContentCellStyle() {
			var style = new AppearanceStyleBase();
			RenderUtils.AppendCssClass(style, GetContentCellCssClass());
			return style;
		}
		protected virtual string GetMainTableCssClass() {
			return EditorStyles.DisplayTableSystemClassName;
		}
		protected virtual AppearanceStyleBase GetMainTableStyle() {
			var style = new AppearanceStyleBase();
			RenderUtils.AppendCssClass(style, GetMainTableCssClass());
			return style;
		}
		protected virtual AppearanceStyleBase GetRootStyle() {
			var style = new AppearanceStyleBase();
			style.CopyFrom(RootStyle);
			return style;
		}
		protected override bool HasRootTag() {
			return true;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareRootControl();
			PrepareMainTable();
			PrepareContentCell();
			PrepareContentHierarchy();
		}
		protected virtual void PrepareRootControl() {
			GetRootStyle().AssignToControl(this);
		}
		protected virtual void PrepareContentCell() {
			GetContentCellStyle().AssignToControl(ContentCell);
		}
		protected virtual void PrepareMainTable() {
			GetMainTableStyle().AssignToControl(MainTable);
		}
		protected abstract void PrepareContentHierarchy();
	}
}
