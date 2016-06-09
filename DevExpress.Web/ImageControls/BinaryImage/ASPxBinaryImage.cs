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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	 DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	 Designer("DevExpress.Web.Design.ASPxBinaryImageDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	 System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxBinaryImage.bmp")
]
	public class ASPxBinaryImage : ASPxEdit, IImageEdit, IBinaryImageEditor {
		readonly static Dictionary<BinaryImageButtonPosition, string> PositionClass = new Dictionary<BinaryImageButtonPosition, string> {
			{BinaryImageButtonPosition.Center, EditorStyles.ContentCenterSystemClassName},
			{BinaryImageButtonPosition.Left, EditorStyles.ContentLeftSystemClassName},
			{BinaryImageButtonPosition.Right, EditorStyles.ContentRightSystemClassName}
		};
		static readonly object EventCallback = new object();
		protected internal const string
			LoadingImageCssClass = "loadingImage",
			ClickHandlerName = "return ASPx.SEClick('{0}', event)",
			StaticEditScriptResourceName = EditScriptsResourcePath + "StaticEdit.js",
			SysBackground = "backgroundSys",
			PictureEditScriptResourceName = EditScriptsResourcePath + "BinaryImage.js",
			PreviewImageID = "DXPreview";
		BinaryImageCallbackArgumentsReader argumentsReader;
		bool uploading = false;
		protected internal BinaryImageMainControl MainControl { get; private set; }
		protected internal ASPxUploadControl UploadControl { get { return MainControl != null ? MainControl.UploadControl : null; } }
		protected internal string ServerValueKey { get { return GetStringProperty("ServerValueKey", String.Empty); } set { SetStringProperty("ServerValueKey", String.Empty, value); } }
		public ASPxBinaryImage()
			: base() {
		}
		[Browsable(false), DefaultValue(null), AutoFormatDisable, Bindable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object Value {
			get { return UseStorageForSaveValue ? GetDataFromBinaryStorage(ServerValueKey) : base.Value; }
			set {
				if(Value == value)
					return;
				if(UseStorageForSaveValue)
					ServerValueKey = StoreData(value);
				else
					base.Value = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageContentBytes"),
#endif
		DefaultValue(null), Browsable(false), Bindable(true), AutoFormatDisable]
		public virtual byte[] ContentBytes {
			get { return Value as byte[]; }
			set { Value = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageBinaryStorageMode"),
#endif
		Category("Behavior"), DefaultValue(BinaryStorageMode.Default), AutoFormatDisable, NotifyParentProperty(true)]
		public BinaryStorageMode BinaryStorageMode {
			get { return Properties.BinaryStorageMode; }
			set { Properties.BinaryStorageMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageStoreContentBytesInViewState"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool StoreContentBytesInViewState {
			get { return Properties.StoreContentBytesInViewState; }
			set { Properties.StoreContentBytesInViewState = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageImageSizeMode"),
#endif
		DefaultValue(ImageSizeMode.ActualSizeOrFit), AutoFormatDisable, NotifyParentProperty(true)]
		public ImageSizeMode ImageSizeMode {
			get { return Properties.ImageSizeMode; }
			set {
				Properties.ImageSizeMode = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageEnableServerResize"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public bool EnableServerResize {
			get { return Properties.EnableServerResize; }
			set {
				Properties.EnableServerResize = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageAlternateText"),
#endif
		DefaultValue(""), Category("Accessibility"), Bindable(true), Localizable(true), AutoFormatDisable]
		public string AlternateText {
			get { return Properties.AlternateText; }
			set { Properties.AlternateText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageToolTip"),
#endif
		DefaultValue(""), Category("Accessibility"), Bindable(true), Localizable(true), AutoFormatDisable]
		public override string ToolTip {
			get { return Properties.ToolTip; }
			set { Properties.ToolTip = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageDescriptionUrl"),
#endif
		DefaultValue(""), Category("Accessibility"), Localizable(false), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string DescriptionUrl {
			get { return Properties.DescriptionUrl; }
			set { Properties.DescriptionUrl = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageEmptyImage"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties EmptyImage {
			get { return Properties.EmptyImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageIsPng"),
#endif
		DefaultValue(false), Localizable(false), Bindable(true), AutoFormatDisable, 
		Obsolete("This property was only required for old browsers (such as IE6), which are not supported now.")]
		public bool IsPng {
			get { return Properties.IsPng; }
			set { Properties.IsPng = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageShowLoadingImage"),
#endif
		Category("Layout"), DefaultValue(false), AutoFormatEnable]
		public bool ShowLoadingImage {
			get { return Properties.ShowLoadingImage; }
			set { Properties.ShowLoadingImage = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageLoadingImageUrl"),
#endif
		DefaultValue(""), Category("Layout"), Localizable(false), AutoFormatDisable,
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor)), UrlProperty]
		public string LoadingImageUrl {
			get { return Properties.LoadingImageUrl; }
			set { Properties.LoadingImageUrl = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageImageAlign"),
#endif
		Category("Layout"), DefaultValue(ImageAlign.NotSet), AutoFormatEnable]
		public ImageAlign ImageAlign {
			get { return Properties.ImageAlign; }
			set { Properties.ImageAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageCustomCallback"),
#endif
 Category("Action")]
		public event CallbackEventHandlerBase CustomCallback {
			add { Events.AddHandler(EventCallback, value); }
			remove { Events.RemoveHandler(EventCallback, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool AutoPostBack {
			get { return base.AutoPostBack; }
			set { base.AutoPostBack = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableFocusedStyle {
			get { return base.EnableFocusedStyle; }
			set { base.EnableFocusedStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ReadOnly {
			get { return base.ReadOnly; }
			set { base.ReadOnly = value; }
		}
		protected new internal BinaryImageEditProperties Properties {
			get { return (BinaryImageEditProperties)base.Properties; }
		}
		protected internal override void InitInternal() {
			BinaryStorage.ProcessRequest();
			base.InitInternal();
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();			
			MainControl = null;
		}
		protected override EditPropertiesBase CreateProperties() {
			return new BinaryImageEditProperties(this);
		}
		[Category("Client-Side"), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public BinaryImageClientSideEvents ClientSideEvents {
			get { return (BinaryImageClientSideEvents)ClientSideEventsInternal; }
		}
		protected BinaryImageDisplayControl CreateImageControl() {
			return new BinaryImageDisplayControl(this);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new BinaryImageClientSideEvents();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientBinaryImage";
		}
		protected override bool IsValueStoreToViewState() {
			return StoreContentBytesInViewState && !AllowEdit;
		}
		[Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BinaryImageDeleteButtonImageProperties DeleteButtonImage {
			get { return Properties.DeleteButtonImage; }
		}
		[Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BinaryImageOpenDialogButtonImageProperties OpenDialogButtonImage {
			get { return Properties.OpenDialogButtonImage; }
		}
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ButtonStyle ButtonStyle { get { return Properties.ButtonStyle; } }
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BinaryImageButtonPanelStyle ButtonPanelStyle { get { return Properties.ButtonPanelStyle; } }
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle DropZoneStyle { get { return Properties.DropZoneStyle; } }
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyle EmptyValueTextStyle { get { return Properties.EmptyValueTextStyle; } }
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProgressBarStyle ProgressBarStyle { get { return Properties.ProgressBarStyle; } }
		[Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ProgressBarIndicatorStyle ProgressBarIndicatorStyle { get { return Properties.ProgressBarIndicatorStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ReadOnlyStyle ReadOnlyStyle { get { return base.ReadOnlyStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorDecorationStyle FocusedStyle { get { return base.FocusedStyle; } }
		protected internal bool AllowDropOnPreview { get { return AllowEdit && EditingSettings.AllowDropOnPreview; } }
		protected internal bool AllowEdit {
			get { return EditingSettings.Enabled; }
			set { EditingSettings.Enabled = value; }
		}
		protected internal BinaryImageButtonPanelSettings ButtonPanelSettings { get { return EditingSettings.ButtonPanelSettings; } }
		protected internal BinaryImageUploadSettings UploadSettings { get { return EditingSettings.UploadSettings; } }
		protected bool IsCleanerActivated { get; set; }
		protected internal bool ShowButtonPanel { get { return AllowEdit && ButtonPanelSettings.Visibility != ElementVisibilityMode.None; } }
		protected bool UseStorageForSaveValue { get { return AllowEdit && !DesignMode; } }
		[AutoFormatDisable, Category("Settings"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public BinaryImageEditingSettings EditingSettings { get { return Properties.EditingSettings; } }
		[Category("Validation"), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new BinaryImageValidationSettings ValidationSettings {
			get { return (BinaryImageValidationSettings) base.ValidationSettings; }
		}
		[AutoFormatEnable, 
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageWidth")
#else
	Description("")
#endif
]
		public override Unit Width {
			get { return Properties.ImageWidth; }
			set { Properties.ImageWidth = value; }
		}
		[AutoFormatEnable, 
#if !SL
	DevExpressWebLocalizedDescription("ASPxBinaryImageHeight")
#else
	Description("")
#endif
]
		public override Unit Height {
			get { return Properties.ImageHeight; }
			set { Properties.ImageHeight = value; }
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			var ButtonPanelID = BinaryImageMainControl.ButtonPanelID;
			helper.AddStyle(GetButtonHoverStyle(), ButtonPanelID + "_" + BinaryImageButtonPanelControl.DeleteButtonID, IsEnabled());
			helper.AddStyle(GetButtonHoverStyle(), ButtonPanelID + "_" + BinaryImageButtonPanelControl.OpenDialogButtonID, IsEnabled());
		}
		protected override AppearanceStyle GetDefaultEditStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(AllowEdit ? 
				RenderStyles.GetDefaultBinaryImageStyle() : 
				RenderStyles.GetDefaultImageStyle());
			return style;
		}
		protected override AppearanceStyle GetEditStyleFromStylesStorage() {
			AppearanceStyle style = new AppearanceStyle();
			if(AllowEdit)
				style.CopyFrom(RenderStyles.BinaryImage);
			return style;
		}
		protected internal AppearanceStyle GetButtonPanelStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultBinaryImageButtonPanelStyle());
			style.CopyFrom(RenderStyles.BinaryImageButtonPanel);
			switch(ButtonPanelSettings.Position) {
				case BinaryImageButtonPanelPosition.Bottom:
					RenderUtils.AppendCssClass(style, EditorStyles.BottomSystemClassName);
					break;
				case BinaryImageButtonPanelPosition.Top:
					RenderUtils.AppendCssClass(style, EditorStyles.TopSystemClassName);
					break;
			}
			RenderUtils.AppendCssClass(style, PositionClass[ButtonPanelSettings.ButtonPosition]);
			RenderUtils.AppendCssClass(style, EditorStyles.BinaryImageControlPanelSystemClassName);
			return style;
		}
		protected internal AppearanceStyle GetButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultBinaryImageButtonStyle());
			style.CopyFrom(RenderStyles.BinaryImageButton);
			RenderUtils.AppendCssClass(style, EditorStyles.BinaryImageButtonSystemClassName);
			return style;
		}
		protected AppearanceStyleBase GetButtonHoverStyle() {
			AppearanceStyle style = new AppearanceStyle();
			AppearanceStyle buttonStyle = GetButtonStyle();
			style.CopyFrom(buttonStyle.HoverStyle);
			RenderUtils.AppendCssClass(style, EditorStyles.BinaryImageButtonHoverSystemClassName);
			return style;
		}
		protected internal AppearanceStyle GetDropZoneStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultBinaryImageDropZoneStyle());
			style.CopyFrom(RenderStyles.BinaryImageDropZone);
			RenderUtils.AppendCssClass(style, EditorStyles.BinaryImageDropZoneSystemClassName);
			return style;
		}
		protected internal AppearanceStyle GetEmptValueTextStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultBinaryImageEmptyValueTextStyle());
			style.CopyFrom(RenderStyles.BinaryImageEmptyValueText);
			RenderUtils.AppendCssClass(style, EditorStyles.BinaryImageEmptySystemClassName);
			return style;
		}
		protected internal AppearanceStyle GetProgressBarStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultProgressBarStyle());
			style.CopyFrom(RenderStyles.ProgressBar);
			return style;
		}
		protected internal AppearanceStyle GetProgressBarIndicatorStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.GetDefaultProgressBarIndicatorStyle());
			style.CopyFrom(RenderStyles.ProgressBarIndicator);
			return style;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			var resourceNames = new List<string>();
			if(!string.IsNullOrWhiteSpace(GetOnClick()))
				resourceNames.Add(StaticEditScriptResourceName);
			if(AllowEdit || IsClientSideAPIEnabled() || ShowLoadingImage)
				resourceNames.Add(PictureEditScriptResourceName);
			resourceNames.ForEach(x => RegisterIncludeScript(typeof(ASPxBinaryImage), x));
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			MainControl = new BinaryImageMainControl(this);
			Controls.Add(MainControl);
		}
		protected internal virtual ASPxUploadControl CreateUploadControl() {
			return new ASPxUploadControl();
		}
		protected internal object GetLoadingImageInfo() {
			return new {
				cssClasses = GetLoadingImageClasses(),
				backgroundImageUrl = string.IsNullOrEmpty(BackgroundImage.ImageUrl) 
					? "" 
					: UrlResolver.ResolveUrl(BackgroundImage.ImageUrl),
				loadingImage = string.IsNullOrEmpty(LoadingImageUrl) 
					? ""
					: string.Format("url(\"{0}\")", UrlResolver.ResolveUrl(LoadingImageUrl))
			};
		}
		protected internal string[] GetLoadingImageClasses() {
			var result = new List<string>();
			if(ShowLoadingImage && !DesignMode) {
				if(!string.IsNullOrEmpty(BackgroundImage.ImageUrl))
					result.Add((this as IImageEdit).GetSysBackgroundCssClassName());
				result.Add((this as IImageEdit).GetLoadingImageCssClassName());
			}
			return result.ToArray();
		}
		protected internal void OnFileUploadComplete(object sender, FileUploadCompleteEventArgs e) {
			this.uploading = true;
			Hashtable result = new Hashtable();
			var key = StoreData(e.UploadedFile.FileBytes);
			result["info"] = new { Key = key, ImageUrl = BinaryStorage.GetResourceUrlInternal(this, BinaryStorageMode, key) };
			e.CallbackData = HtmlConvertor.ToJSON(result);
			this.uploading = false;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(AllowEdit && IsEnabled()) {
				var oldKey = ServerValueKey;
				var postbackDataKey = postCollection[UniqueID] as string;
				if(postbackDataKey != null && !CommonUtils.AreEqual(oldKey, postbackDataKey, false)) {
					ServerValueKey = postbackDataKey;
					return true;
				}
			}
			return false;
		}
		object GetDataFromBinaryStorage(string key) {
			var data = BinaryStorage.GetResourceData(this, BinaryStorageMode, key);
			return data != null ? data.Content : null;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			Action<string, object> setClientPropertyRaw = (name, value) =>
				stb.AppendLine(string.Format("{0}.{1} = {2};", localVarName, name, value));
			Action<string, object> setClientProperty = (name, value) =>
				setClientPropertyRaw(name, HtmlConvertor.ToScript(value));
			setClientProperty("enableEditing", AllowEdit);
			if(ShowButtonPanel)
				setClientProperty("buttonPanelVisibility", (int)ButtonPanelSettings.Visibility);
			setClientProperty("allowDropOnPreview", AllowDropOnPreview);
			if(ShowLoadingImage) {
				setClientProperty("showLoadingImage", true);
				var jss = new JavaScriptSerializer();
				setClientPropertyRaw("loadingImageData", jss.Serialize(GetLoadingImageInfo()));
			}
		}
		protected internal string StoreData(object data) {
			var value = data as byte[];
			if(!AllowEdit || value == null)
				return string.Empty;
			var key = BinaryStorage.GetResourceKeyForResizedImage(this, value, BinaryStorageMode);
			var binaryStorageData = new BinaryStorageData(value, BinaryStorage.GetImageMimeType(value));
			BinaryStorage.StoreResourceData(this, BinaryStorageMode, key, binaryStorageData);
			return key;
		}
		protected override bool HasFunctionalityScripts() { return IsClientSideAPIEnabled() || AllowEdit || ShowLoadingImage; }
		protected override bool HasHoverScripts() { return IsStateScriptEnabled() && AllowEdit; }
		protected override bool HasLoadingDiv() { return false; }
		protected override bool HasLoadingPanel() { return false; }
		protected override void RaiseCallbackEvent(string eventArgument) {
			EnsurePostDataLoaded();
			object oldValue = Value;
			this.argumentsReader = new BinaryImageCallbackArgumentsReader(eventArgument);
			if(argumentsReader.IsCustomCallback)
				RaiseCustomCallback(argumentsReader.CustomCallbackArg);			
		}
		protected void RaiseCustomCallback(string arg) {
			CallbackEventHandlerBase handler = Events[EventCallback] as CallbackEventHandlerBase;
			if(handler != null)
				handler(this, new CallbackEventArgsBase(arg));
		}		
		public override bool IsLoading() {
			return base.IsLoading() || this.uploading;
		}
		protected internal override bool IsCallBacksEnabled() {
			return true;
		}
		protected override object GetCallbackResult() {
			return new { Key = ServerValueKey, ImageUrl = BinaryStorage.GetResourceUrlInternal(this, BinaryStorageMode, ServerValueKey) };
		}
		protected internal ImagePropertiesBase GetOpenDialogButtonImage() {
			return RenderImages.GetImageProperties(Page, EditorImages.BinaryImageOpenDialogButtonImageName);
		}
		protected internal ImagePropertiesBase GetDeleteButtonImage() {
			return RenderImages.GetImageProperties(Page, EditorImages.BinaryImageDeleteButtonImageName);
		}
		protected internal override string GetAssociatedControlID() { return string.Empty; } 
		protected internal string GetEmptyValueText() {
			var text = EditingSettings.EmptyValueText;
			return HtmlEncode(text);
		}		
		bool IBinaryImageEditor.AllowEdit { get { return AllowEdit; } }
		string IBinaryImageEditor.TempFolder { get { return UploadSettings.TemporaryFolder; } }
		int IBinaryImageEditor.TempFileExpirationTime { get { return UploadSettings.TempFileExpirationTime; } }
		IImageEditProperties IImageEdit.ImageEditProperties { get { return Properties; } }
		protected internal virtual string GetImageUrl() { return ""; }
		string IImageEdit.GetImageUrl() { return GetImageUrl(); }
		string IImageEdit.GetLoadingImageCssClassName() { return StylesInternal.GetCssClassNamePrefix() + "-" + LoadingImageCssClass; }
		string IImageEdit.GetSysBackgroundCssClassName() { return StylesInternal.GetCssClassNamePrefix() + "-" + SysBackground; }
		protected internal virtual string GetOnClick() {
			if(ClientSideEvents.Click != "" && Enabled)
				return string.Format(ClickHandlerName, ClientID);
			return "";
		}
		string IImageEdit.GetOnClick() { return GetOnClick(); }
	}
}
namespace DevExpress.Web.Internal{
	public class BinaryImageCallbackArgumentsReader : CallbackArgumentsReader {
		public const string CustomCallbackPrefix = "BICC";
		public BinaryImageCallbackArgumentsReader(string arguments)
			: base(arguments, new[] { CustomCallbackPrefix }) {
		}
		public bool IsCustomCallback { get { return CustomCallbackArg != null; } }
		public string CustomCallbackArg { get { return this[CustomCallbackPrefix]; } }
	}
}
