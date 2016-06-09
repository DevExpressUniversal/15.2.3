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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.Captcha;
namespace DevExpress.Web {
	[DXWebToolboxItem(DXToolboxItemKind.Free), 
	DevExpress.Utils.Design.DXClientDocumentationProviderWeb("ASPxCaptcha"),
	Designer("DevExpress.Web.Design.ASPxCaptchaDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxCaptcha.bmp")]
	public class ASPxCaptcha : ASPxWebControl, IRequiresLoadPostDataControl {
		protected internal const string CaptchaScriptResourceName = "DevExpress.Web.Scripts.Editors.Captcha.js";
		protected const int DefaultCodeLength = 5;
		protected const int MinCodeLength = 3;
		protected const int DefaultEditorWidth = 200;
		protected const string DefaultCharacterSet = "abcdefhjklmnpqrstuvxyz23456789";
		const string TextBoxID = "TB";
		const string ImageID = "IMG";
		const string RefreshButtonID = "RB";
		const string ImageDivID = "IMGD";
		const string TextBoxInputPostfix = "_I";
		const string ClientObjectClassName = "ASPxClientCaptcha";
		const string RefreshCommand = "R";
		bool isValid;
		bool requiresEditorValidation;
		IRandomNumberGenerator randomGenerator;
		IImageFactory imageFactory;
		ICodeGenerator codeGenerator;
		WebControl mainDiv;
		WebControl imageDiv;
		Table contentTable;
		TableCell textBoxCell;
		TableCell refreshButtonCell;
		System.Web.UI.WebControls.Image imageControl;
		RefreshButtonControl refreshButtonControl;
		WebControl textBoxLabel;
		LiteralControl textBoxLabelText;
		ASPxTextBox textBoxControl;
		CaptchaValidationSettings validationSettings;
		CaptchaTextBoxProperties textBox;
		RefreshButtonProperties refreshButton;
		CaptchaImageProperties challengeImage;
		static readonly object EventChallengeImageCustomRender = new object();
		public ASPxCaptcha()
			: this(null) {
		}
		public ASPxCaptcha(ASPxWebControl owner)
			: base(owner) {
			this.randomGenerator = CreateRandomGenerator();
			this.imageControl = RenderUtils.CreateImage();
			this.textBoxControl = new ASPxTextBox();
			this.textBoxControl.Validation += new EventHandler<ValidationEventArgs>(OnTextBoxValidation);
			this.validationSettings = new CaptchaValidationSettings(this);
			this.textBox = new CaptchaTextBoxProperties(this);
			this.refreshButton = new RefreshButtonProperties(this);
			this.challengeImage = new CaptchaImageProperties(this);
			EnableCallBacks = true;
			RequiresEditorValidation = true;
			IsValid = false;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaEnableCallBacks"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallBacks {
			get { return EnableCallBacksInternal; }
			set { EnableCallBacksInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaEnableCallbackAnimation"),
#endif
		Category("Behavior"), DefaultValue(DefaultEnableCallbackAnimation), AutoFormatDisable]
		public bool EnableCallbackAnimation {
			get { return EnableCallbackAnimationInternal; }
			set { EnableCallbackAnimationInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaEnableCallbackCompression"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool EnableCallbackCompression {
			get { return EnableCallbackCompressionInternal; }
			set { EnableCallbackCompressionInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaCodeLength"),
#endif
		Category("Behavior"), DefaultValue(DefaultCodeLength), AutoFormatDisable]
		public int CodeLength {
			get { return GetIntProperty("CodeLength", DefaultCodeLength); }
			set {
				CommonUtils.CheckMinimumValue(value, MinCodeLength, "CodeLength");
				SetIntProperty("CodeLength", DefaultCodeLength, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaCharacterSet"),
#endif
		Category("Behavior"), DefaultValue(DefaultCharacterSet), Localizable(false), AutoFormatDisable]
		public string CharacterSet {
			get { return GetStringProperty("CharacterSet", DefaultCharacterSet); }
			set {
				SetStringProperty("CharacterSet", DefaultCharacterSet, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return Images.LoadingPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaSpriteCssFilePath"),
#endif
		Category("Images"), DefaultValue(""), Localizable(false), UrlProperty,
		AutoFormatEnable, AutoFormatUrlProperty, Editor(typeof(System.Web.UI.Design.UrlEditor),
		typeof(System.Drawing.Design.UITypeEditor))]
		public string SpriteCssFilePath {
			get { return SpriteCssFilePathInternal; }
			set { SpriteCssFilePathInternal = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected CaptchaStyles Styles {
			get { return (CaptchaStyles)StylesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaRefreshButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual RefreshButtonStyle RefreshButtonStyle {
			get { return Styles.RefreshButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaDisabledRefreshButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual RefreshButtonStyle DisabledRefreshButtonStyle {
			get { return Styles.DisabledRefreshButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaNullTextStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle NullTextStyle {
			get { return Styles.NullText; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaTextBoxStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual CaptchaTextBoxStyle TextBoxStyle {
			get { return Styles.TextBox; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaValidationSettings"),
#endif
		Category("Validation"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatDisable]
		public CaptchaValidationSettings ValidationSettings {
			get { return this.validationSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaRefreshButton"),
#endif
		Category("Misc"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RefreshButtonProperties RefreshButton {
			get { return this.refreshButton; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaTextBox"),
#endif
		Category("Misc"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CaptchaTextBoxProperties TextBox {
			get { return this.textBox; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaChallengeImage"),
#endif
		Category("Misc"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		AutoFormatEnable, DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CaptchaImageProperties ChallengeImage {
			get { return this.challengeImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SettingsLoadingPanel LoadingPanel {
			get { return SettingsLoadingPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public CallbackClientSideEventsBase ClientSideEvents {
			get { return (CallbackClientSideEventsBase)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCaptchaChallengeImageCustomRender"),
#endif
		Category("Misc")]
		public event ChallengeImageCustomRenderEventHandler ChallengeImageCustomRender
		{
			add { Events.AddHandler(EventChallengeImageCustomRender, value); }
			remove { Events.RemoveHandler(EventChallengeImageCustomRender, value); }
		}
		[Browsable(false), DefaultValue(false), AutoFormatDisable]
		public bool IsValid {
			get { return isValid; }
			set { isValid = value; }
		}
		[Browsable(false)]
		public virtual string Code {
			get {
				var session = HttpUtils.GetSession();
				if (DesignMode || session == null)
					return (string)GetDesignModeState()[CodeSessionKey];
				return (string)session[CodeSessionKey];
			}
			protected set {
				var session = HttpUtils.GetSession();
				if (DesignMode || session == null) {
					GetDesignModeState()[CodeSessionKey] = value;
					return;
				}
				session[CodeSessionKey] = value;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected string EditorPostDataKey {
			get { return GetEditorPostDataKey(UniqueID, IdSeparator); }
		}
		protected static string GetEditorPostDataKey(string id, char idSeparator) {
			return id + idSeparator + TextBoxID;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected bool RequiresEditorValidation {
			get { return requiresEditorValidation; }
			set { requiresEditorValidation = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected ICodeGenerator CodeGenerator {
			get {
				if (this.codeGenerator == null)
					this.codeGenerator = CreateCodeGenerator();
				return this.codeGenerator;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected IImageFactory ImageFactory {
			get {
				if (this.imageFactory == null)
					this.imageFactory = CreateImageFactory();
				return this.imageFactory;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected string CodeSessionKey {
			get { return GetCodeSessionKey(ID, Page); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected static string GetCodeSessionKey(string id, Page page){
			return (page != null ? page.GetType().FullName : string.Empty) + "_" + id + "_CK";
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected internal CaptchaImages Images {
			get { return (CaptchaImages)ImagesInternal; }
		}
		protected virtual IRandomNumberGenerator CreateRandomGenerator() {
			return RandomAdapter.Instance;
		}
		protected virtual ICodeGenerator CreateCodeGenerator() {
			return DesignMode ? (ICodeGenerator)new DesignModeCodeGenerator(this) :
				(ICodeGenerator)new RuntimeCodeGenerator(this, this.randomGenerator);
		}
		protected virtual IImageFactory CreateImageFactory() {
			return new ChallengeImageFactory(this, this.randomGenerator);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ValidationSettings, RefreshButton, TextBox, ChallengeImage });
		}
		protected override SettingsLoadingPanel CreateSettingsLoadingPanel() {
			return new CaptchaSettingsLoadingPanel(this);
		}
		protected override ImagesBase CreateImages() {
			return new CaptchaImages(this);
		}
		protected override void RegisterDefaultRenderCssFile() {
			ResourceManager.RegisterCssResource(Page, typeof(ASPxCaptcha), ASPxEditBase.EditDefaultCssResourceName);
		}
		protected override string GetSkinControlName() {
			return "Editors";
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new CallbackClientSideEventsBase();
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxCaptcha), CaptchaScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return ClientObjectClassName;
		}
		public override void Focus() {
			if (TextBox.Visible) {
				EnsureChildControls();
				this.textBoxControl.Focus();
			} else
				base.Focus();
		}
		protected override bool IsServerSideEventsAssigned() {
			return HasEvents();
		}
		protected void OnCaptchaImageCustomRender(ChallengeImageCustomRenderEventArgs e) {
			ChallengeImageCustomRenderEventHandler handler =
				(ChallengeImageCustomRenderEventHandler)Events[EventChallengeImageCustomRender];
			if (handler != null)
				handler(this, e);
			this.RaiseBubbleEvent(this, e);
		}
		protected internal override bool IsCallBacksEnabled() {
			return EnableCallBacks;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			base.RaiseCallbackEvent(eventArgument);
			if (eventArgument == RefreshCommand)
				RequiresEditorValidation = false;
		}
		protected override object GetCallbackResult() {
			return GetImageUrl();
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			base.RaisePostBackEvent(eventArgument);
			if (eventArgument == RefreshCommand)
				RequiresEditorValidation = false;
		}
		protected void ValidateSubmittedText(NameValueCollection postCollection) {
			if (string.IsNullOrEmpty(Code))
				return;
			string submittedText = postCollection[EditorPostDataKey];
			IsValid = submittedText == Code;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			ValidateSubmittedText(postCollection);
			return base.LoadPostData(postCollection);
		}
		protected void OnTextBoxValidation(object sender, ValidationEventArgs e) {
			if (!ValidationSettings.EnableValidation)
				return;
			e.IsValid = IsValid;
		}
		protected override StylesBase CreateStyles() {
			return new CaptchaStyles(this);
		}
		protected override bool CanAppendDefaultLoadingPanelCssClass() {
			return false;
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetDisabledRefreshButtonStyle(), RefreshButtonID, IsEnabled());
			helper.AddStyle(GetDisabledRefreshButtonTextStyle(), RefreshButtonControl.TextSpanID, IsEnabled());
		}
		protected internal AppearanceStyle GetRefreshButtonStyle() {
			RefreshButtonStyle style = new RefreshButtonStyle();
			style.CopyFrom(Styles.GetDefaultRefreshButtonStyle());
			if (!Enabled)
				style.CopyFrom(Styles.GetDefaultDisabledRefreshButtonStyle());
			style.CopyFrom(RefreshButtonStyle);
			if (!Enabled) {
				MergeDisableStyle(style, false);
				style.CopyFrom(DisabledRefreshButtonStyle);
			}
			return style;
		}
		protected internal AppearanceStyle GetRefreshButtonTextStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultRefreshButtonTextStyle());
			if (!Enabled)
				style.CopyFrom(Styles.GetDefaultDisabledRefreshButtonTextStyle());
			if (!Enabled)
				MergeDisableStyle(style, false);
			return style;
		}
		protected AppearanceStyle GetDisabledRefreshButtonTextStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultDisabledRefreshButtonTextStyle());
			return style;
		}
		protected AppearanceStyle GetDisabledRefreshButtonStyle() {
			RefreshButtonStyle style = new RefreshButtonStyle();
			style.CopyFrom(Styles.GetDefaultDisabledRefreshButtonStyle());
			style.CopyFrom(DisabledRefreshButtonStyle);
			return style;
		}
		protected AppearanceStyle GetRefreshButtonCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultRefreshButtonCellStyle());
			return style;
		}
		protected AppearanceStyle GetTextBoxCellStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultTextBoxCellNoIndentStyle());
			return style;
		}
		protected AppearanceStyle GetTextBoxLabelStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetDefaultTextBoxLabelStyle());
			return style;
		}
		protected AppearanceStyle GetTextBoxStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(TextBoxStyle);
			return style;
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			this.mainDiv = null;
			this.imageDiv = null;
			this.contentTable = null;
			this.textBoxCell = null;
			this.refreshButtonCell = null;
			this.refreshButtonControl = null;
			this.textBoxLabel = null;
			this.textBoxLabelText = null;
		}
		protected override bool HasLoadingPanel() {
			return EnableCallBacks;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.mainDiv = RenderUtils.CreateDiv();
			this.contentTable = RenderUtils.CreateTable();
			if (TextBox.Position == ControlPosition.Top)
				CreateTextBoxRow();
			if (RefreshButton.Position == ControlPosition.Top)
				CreateRefreshButtonRow();
			CreateMainRow();
			if (RefreshButton.Position == ControlPosition.Bottom)
				CreateRefreshButtonRow();
			if (TextBox.Position == ControlPosition.Bottom)
				CreateTextBoxRow();
			this.mainDiv.Controls.Add(contentTable);
			Controls.Add(this.mainDiv);
		}
		protected void CreateMainRow() {
			TableRow mainRow = RenderUtils.CreateTableRow();
			if (TextBox.Position == ControlPosition.Left)
				CreateTextBoxCell(mainRow);
			if (RefreshButton.Position == ControlPosition.Left)
				CreateRefreshButtonCell(mainRow);
			CreateImageCell(mainRow);
			if (RefreshButton.Position == ControlPosition.Right)
				CreateRefreshButtonCell(mainRow);
			if (TextBox.Position == ControlPosition.Right)
				CreateTextBoxCell(mainRow);
			this.contentTable.Rows.Add(mainRow);
		}
		protected void CreateTextBoxRow() {
			if (!TextBox.Visible)
				return;
			TableRow textBoxRow = RenderUtils.CreateTableRow();
			CreateTextBoxCell(textBoxRow);
			if (RefreshButton.Position == ControlPosition.Left ||
				RefreshButton.Position == ControlPosition.Right) {
				textBoxRow.Cells.Add(RenderUtils.CreateTableCell());
			}
			this.contentTable.Rows.Add(textBoxRow);
		}
		protected void CreateRefreshButtonRow() {
			if (!RefreshButton.Visible)
				return;
			TableRow refreshButtonRow = RenderUtils.CreateTableRow();
			if (TextBox.Visible && TextBox.Position == ControlPosition.Left)
				refreshButtonRow.Cells.Add(RenderUtils.CreateTableCell());
			CreateRefreshButtonCell(refreshButtonRow);
			if (TextBox.Position == ControlPosition.Left ||
			   TextBox.Position == ControlPosition.Right) {
				refreshButtonRow.Cells.Add(RenderUtils.CreateTableCell());
			}
			this.contentTable.Rows.Add(refreshButtonRow);
		}
		protected void CreateImageCell(TableRow row) {
			TableCell imageCell = RenderUtils.CreateTableCell();
			this.imageDiv = RenderUtils.CreateDiv();
			this.imageDiv.ID = ImageDivID;
			this.imageControl.ID = ImageID;
			this.imageDiv.Controls.Add(this.imageControl);
			imageCell.Controls.Add(this.imageDiv);
			row.Cells.Add(imageCell);
		}
		protected void CreateTextBoxCell(TableRow row) {
			if (!TextBox.Visible)
				return;
			this.textBoxCell = RenderUtils.CreateTableCell();
			if (TextBox.ShowLabel) {
				this.textBoxLabelText = RenderUtils.CreateLiteralControl();
				this.textBoxLabel = RenderUtils.CreateWebControl(HtmlTextWriterTag.Label);
				this.textBoxLabel.Controls.Add(this.textBoxLabelText);
				this.textBoxCell.Controls.Add(this.textBoxLabel);
			}
			this.textBoxControl.ID = TextBoxID;
			this.textBoxControl.EnableViewState = false;
			this.textBoxCell.Controls.Add(this.textBoxControl);
			row.Cells.Add(this.textBoxCell);
		}
		protected void CreateRefreshButtonCell(TableRow row) {
			if (!RefreshButton.Visible)
				return;
			this.refreshButtonCell = RenderUtils.CreateTableCell();
			this.refreshButtonControl = new RefreshButtonControl(this);
			this.refreshButtonControl.ID = RefreshButtonID;
			this.refreshButtonControl.Enabled = Enabled;
			this.refreshButtonCell.Controls.Add(this.refreshButtonControl);
			row.Cells.Add(this.refreshButtonCell);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AssignAttributes(this, this.mainDiv);
			GetControlStyle().AssignToControl(this.mainDiv); 
			RenderUtils.SetVisibility(this.mainDiv, IsClientVisible(), true);
			if (this.TextBox.Visible)
				this.mainDiv.TabIndex = 0;
			PrepareImageCell();
			PrepareRefreshButtonCell();
			PrepareTextBoxCell();
		}
		protected string GetImageUrl() {
			Code = CodeGenerator.GetCode();
			string code = Code;
			if(RightToLeft == DefaultBoolean.True) {
				char[] arr = code.ToCharArray();
				Array.Reverse(arr);
				code = new string(arr);
			}
			byte[] imageData = null;
			if (IsServerSideEventsAssigned() && Events[EventChallengeImageCustomRender] != null) {
				Bitmap image = new Bitmap(ChallengeImage.Width, ChallengeImage.Height);
				ChallengeImageCustomRenderEventArgs e = new ChallengeImageCustomRenderEventArgs(image, code);
				OnCaptchaImageCustomRender(e);
				imageData = e.GetImageData();
			} else
				imageData = ImageFactory.GetImage(code);
			return BinaryStorage.GetImageUrl(this, imageData, challengeImage.BinaryStorageMode);
		}
		protected void PrepareImageCell() {
			ImageProperties imageProperties = new ImageProperties();
			imageProperties.AlternateText = ChallengeImage.AlternateText;
			imageProperties.ToolTip = ChallengeImage.ToolTip;
			imageProperties.Url = GetImageUrl();
			imageProperties.Width = ChallengeImage.Width;
			imageProperties.Height = ChallengeImage.Height;
			imageProperties.AssignToControl(this.imageControl, DesignMode);
			ChallengeImage.BackgroundImage.AssignToControl(this.imageDiv);
			this.imageDiv.Width = imageProperties.Width;
			RenderUtils.SetLineHeight(this.imageDiv, Unit.Pixel(0));
			this.imageDiv.BorderWidth = Unit.Pixel(ChallengeImage.BorderWidth);
			this.imageDiv.BorderColor = ChallengeImage.BorderColor;
			this.imageDiv.BackColor = ChallengeImage.BackgroundColor;
			RenderUtils.AppendDefaultDXClassName(this.imageDiv, "dxca-imageDiv");
			RenderUtils.SetStyleUnitAttribute(this.imageDiv, "font-size", Unit.Pixel(0));
		}
		protected void PrepareRefreshButtonCell() {
			if (this.refreshButtonCell == null)
				return;
			GetRefreshButtonCellStyle().AssignToControl(this.refreshButtonCell, true);
			this.refreshButtonControl.ButtonStyle = GetRefreshButtonStyle();
		}
		protected void PrepareTextBoxCell() {
			if (!TextBox.Visible)
				return;
			GetTextBoxCellStyle().AssignToControl(this.textBoxCell, true);
			PrepareTextBox();
			if (this.textBoxLabel != null) {
				RenderUtils.SetStringAttribute(this.textBoxLabel, "for",
					this.textBoxControl.ClientID + TextBoxInputPostfix);
				this.textBoxLabelText.Text = HtmlEncode(TextBox.LabelText);
				GetTextBoxLabelStyle().AssignToControl(this.textBoxLabel);
			}
		}
		protected void PrepareTextBox() {
			TextBox.AssignToControl(this.textBoxControl);
			this.textBoxControl.EncodeHtml = EncodeHtml;
			this.textBoxControl.Enabled = Enabled;
			this.textBoxControl.AutoCompleteType = AutoCompleteType.Disabled;
			ValidationSettings.AssignToControl(this.textBoxControl);
			GetTextBoxStyle().AssignToControl(this.textBoxControl);
			this.textBoxControl.Width = TextBoxStyle.Width;
			if (this.textBoxControl.Width.IsEmpty)
				this.textBoxControl.Width = Unit.Pixel(DefaultEditorWidth);
			this.textBoxControl.NullTextStyle.Assign(NullTextStyle);
			this.textBoxControl.ValidationSettings.ValidateOnLeave = false;
			this.textBoxControl.ParentSkinOwner = this;
			this.textBoxControl.TabIndex = TabIndex; 
			if (IsValidateTextBox())
				ASPxEdit.ValidateEditorsInContainer(this.textBoxControl);
			if (!RequiresEditorValidation || IsValid)
				ASPxEdit.ClearEditorsInContainer(this.textBoxControl);
			this.textBoxControl.Value = string.Empty;
		}
		protected virtual bool IsValidateTextBox() {
			return Page != null && Page.IsPostBack;
		}
	}
}
