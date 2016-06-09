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
using System.Text;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using System.Text.RegularExpressions;
	using DevExpress.Utils;
	using System.Collections;
	public abstract class TextEditProperties : EditProperties {
		private MaskSettings maskSettings;
		private TextEditHelpTextSettings helpTextSettings;
		public TextEditProperties()
			: this(null) {
		}
		public TextEditProperties(IPropertiesOwner owner)
			: base(owner) {
			this.maskSettings = CreateMaskSettings();
			this.helpTextSettings = CreateHelpTextSettings();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditPropertiesWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable, NotifyParentProperty(true)]
		public Unit Width {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set { SetUnitProperty("Width", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditPropertiesHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable, NotifyParentProperty(true)]
		public Unit Height {
			get { return GetUnitProperty("Height", Unit.Empty); }
			set { SetUnitProperty("Height", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditPropertiesDisplayFormatInEditMode"),
#endif
		DefaultValue(false), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual bool DisplayFormatInEditMode {
			get { return GetBoolProperty("DisplayFormatInEditMode", false); }
			set { SetBoolProperty("DisplayFormatInEditMode", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditPropertiesNullTextStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle NullTextStyle {
			get { return Styles.NullText; }
		}
		protected internal new TextEditClientSideEvents ClientSideEvents {
			get { return (TextEditClientSideEvents)base.ClientSideEvents; }
		}
		protected internal MaskSettings MaskSettingsInternal {
			get { return maskSettings; }
		}
		protected internal string NullTextInternal {
			get { return GetStringProperty("NullText", ""); }
			set { 
				SetStringProperty("NullText", "", value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditPropertiesHelpText"),
#endif
		Localizable(true), DefaultValue(""), AutoFormatEnable, NotifyParentProperty(true)]
		public string HelpText {
			get { return GetStringProperty("HelpText", string.Empty); }
			set { SetStringProperty("HelpText", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditPropertiesHelpTextSettings"),
#endif
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TextEditHelpTextSettings HelpTextSettings {
			get { return this.helpTextSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditPropertiesHelpTextStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HelpTextStyle HelpTextStyle {
			get { return Styles.HelpText; }
		}
		protected virtual TextEditHelpTextSettings CreateHelpTextSettings() {
			return new TextEditHelpTextSettings(this);
		}
		protected virtual MaskSettings CreateMaskSettings() {
			return new MaskSettings(this);
		}
		protected override void AssignEditorProperties(ASPxEditBase edit) {
			base.AssignEditorProperties(edit);
			ASPxTextEdit textEdit = edit as ASPxTextEdit;
			if(textEdit == null) return;
			if(DisplayFormatInEditMode)
				textEdit.DisplayFormatString = DisplayFormatString;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				TextEditProperties src = source as TextEditProperties;
				if(src != null) {
					Width = src.Width;
					Height = src.Height;
					NullTextInternal = src.NullTextInternal;
					HelpText = src.HelpText;
					MaskSettingsInternal.Assign(src.MaskSettingsInternal);
					HelpTextSettings.Assign(src.HelpTextSettings);
					DisplayFormatInEditMode = src.DisplayFormatInEditMode;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new TextEditClientSideEvents(this);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), 
				new IStateManager[] { MaskSettingsInternal, HelpTextSettings });
		}
	}
	public class TextEditHelpTextSettings : PropertiesBase {
		Margins popupMargins = new Margins();
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { PopupMargins });
		}
		internal HelpTextPosition GetPosition() {
			if (Position == HelpTextPosition.Auto)
				return DisplayMode == HelpTextDisplayMode.Inline ? HelpTextPosition.Bottom : HelpTextPosition.Right;
			else
				return Position;
		}
		internal HelpTextHorizontalAlign GetHorizontalAlign() {
			if (HorizontalAlign == HelpTextHorizontalAlign.Auto) {
				bool rtl = (Owner as ISkinOwner).IsRightToLeft();
				if (GetPosition() == HelpTextPosition.Left)
					return rtl ? HelpTextHorizontalAlign.Left : HelpTextHorizontalAlign.Right;
				else
					return rtl ? HelpTextHorizontalAlign.Right : HelpTextHorizontalAlign.Left;
			}
			else
				return HorizontalAlign;
		}
		internal HelpTextVerticalAlign GetVerticalAlign() {
			if (VerticalAlign == HelpTextVerticalAlign.Auto)
				if (DisplayMode == HelpTextDisplayMode.Inline)
					return GetPosition() == HelpTextPosition.Top ? HelpTextVerticalAlign.Bottom : HelpTextVerticalAlign.Top;
				else
					return HelpTextVerticalAlign.Middle;
			else
				return VerticalAlign;
		}
		public TextEditHelpTextSettings(IPropertiesOwner owner) : base(owner) { }
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				TextEditHelpTextSettings src = source as TextEditHelpTextSettings;
				if (src != null) {
					DisplayMode = src.DisplayMode;
					EnablePopupAnimation = src.EnablePopupAnimation;
					HorizontalAlign = src.HorizontalAlign;
					PopupMargins = src.PopupMargins;
					Position = src.Position;
					VerticalAlign = src.VerticalAlign;
				}
			}
			finally {
				EndUpdate();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditHelpTextSettingsPopupMargins"),
#endif
		AutoFormatEnable, NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margins PopupMargins {
			get { return this.popupMargins; }
			set { this.popupMargins = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditHelpTextSettingsPosition"),
#endif
		DefaultValue(HelpTextPosition.Auto), NotifyParentProperty(true), AutoFormatEnable]
		public HelpTextPosition Position {
			get { return (HelpTextPosition)GetEnumProperty("Position", HelpTextPosition.Auto); }
			set { SetEnumProperty("Position", HelpTextPosition.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditHelpTextSettingsHorizontalAlign"),
#endif
		DefaultValue(HelpTextHorizontalAlign.Auto), NotifyParentProperty(true), AutoFormatEnable]
		public HelpTextHorizontalAlign HorizontalAlign {
			get { return (HelpTextHorizontalAlign)GetEnumProperty("HorizontalAlign", HelpTextHorizontalAlign.Auto); }
			set { SetEnumProperty("HorizontalAlign", HelpTextHorizontalAlign.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditHelpTextSettingsVerticalAlign"),
#endif
		DefaultValue(HelpTextVerticalAlign.Auto), NotifyParentProperty(true), AutoFormatEnable]
		public HelpTextVerticalAlign VerticalAlign {
			get { return (HelpTextVerticalAlign)GetEnumProperty("VerticalAlign", HelpTextVerticalAlign.Auto); }
			set { SetEnumProperty("VerticalAlign", HelpTextVerticalAlign.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditHelpTextSettingsEnablePopupAnimation"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public bool EnablePopupAnimation {
			get { return GetBoolProperty("EnablePopupAnimation", true); }
			set { SetBoolProperty("EnablePopupAnimation", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextEditHelpTextSettingsDisplayMode"),
#endif
		DefaultValue(HelpTextDisplayMode.Inline), NotifyParentProperty(true), AutoFormatEnable]
		public HelpTextDisplayMode DisplayMode {
			get { return (HelpTextDisplayMode)GetEnumProperty("DisplayMode", HelpTextDisplayMode.Inline); }
			set { SetEnumProperty("DisplayMode", HelpTextDisplayMode.Inline, value); }
		}
	}
	[DefaultProperty("Text"), DefaultEvent("TextChanged"), ControlValueProperty("Text"), 
	ValidationProperty("Value"), DataBindingHandler(typeof(TextDataBindingHandler))
	]
	public abstract class ASPxTextEdit : ASPxEdit, ITextControl, IEditableTextControl {
		protected internal const string TextChangedHandlerName = "ASPx.ETextChanged('{0}')";
		protected internal const string InputControlSuffix = "I";
		protected internal const string RawValueKey = "rawValue";
		protected internal const string TextEditScriptResourceName = EditScriptsResourcePath + "TextEdit.js";
		protected internal const string MaskScriptResourceName = EditScriptsResourcePath + "Mask.js";
		WebControl maskHintControl = null;
		private static readonly object EventTextChanged = new object();
		public ASPxTextEdit()
			: base() {
		}
		protected ASPxTextEdit(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextEditText"),
#endif
		Localizable(true), DefaultValue(""),
		Bindable(true, BindingDirection.TwoWay), AutoFormatDisable]
		public virtual string Text {
			get { return CommonUtils.ValueToString(this.Value); }
			set { Value = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextEditAutoResizeWithContainer"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public bool AutoResizeWithContainer {
			get { return GetBoolProperty("AutoResizeWithContainer", false); }
			set { SetBoolProperty("AutoResizeWithContainer", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextEditNullTextStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle NullTextStyle {
			get { return Properties.NullTextStyle; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxTextEditWidth")]
#endif
		public override Unit Width {
			get { return Properties.Width; }
			set { Properties.Width = value; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxTextEditHeight")]
#endif
		public override Unit Height {
			get { return Properties.Height; }
			set { Properties.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextEditRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		protected MaskSettings MaskSettingsInternal {
			get { return Properties.MaskSettingsInternal; }
		}
		protected string NullTextInternal {
			get { return Properties.NullTextInternal; }
			set { Properties.NullTextInternal = value; }
		}
		protected override bool HasValidationPatterns {
			get { return base.HasValidationPatterns || IsMaskValidationPatternRequired(); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextEditDisplayFormatString"),
#endif
		DefaultValue(""), Localizable(true), AutoFormatDisable]
		public virtual string DisplayFormatString {
			get { return GetStringProperty("DisplayFormatString", ""); }
			set { SetStringProperty("DisplayFormatString", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextEditTextChanged"),
#endif
		Category("Action")]
		public event EventHandler TextChanged
		{
			add { Events.AddHandler(EventTextChanged, value); }
			remove { Events.RemoveHandler(EventTextChanged, value); }
		}
		internal virtual bool IsInputStretched {
			get { return true; }
		}
		protected internal TextEditClientSideEvents ClientSideEvents {
			get { return ClientSideEventsInternal as TextEditClientSideEvents; }
		}
		protected internal new TextEditProperties Properties {
			get { return (TextEditProperties)base.Properties; }
		}
		protected override Style CreateControlStyle() {
			return new EditAreaStyle();
		}
		protected virtual bool IsPasswordMode() {
			return false;
		}
		protected internal virtual bool IsMaskCapabilitiesEnabled() {
			return !string.IsNullOrEmpty(MaskSettingsInternal.Mask) && !IsPasswordMode();
		}
		protected internal virtual bool IsDisplayFormatCapabilityEnabled() {
			return !string.IsNullOrEmpty(DisplayFormatString) && !IsPasswordMode();
		}
		protected virtual bool IsMaskValidationPatternRequired() {
			return IsMaskCapabilitiesEnabled();
		}
		protected virtual bool IsNullTextCapabilityEnabled() {
			return !string.IsNullOrEmpty(NullTextInternal);
		}
		protected virtual bool HasTextDecorator() {
			return IsMaskCapabilitiesEnabled() || IsNullTextCapabilityEnabled() || IsDisplayFormatCapabilityEnabled();
		}
		protected override bool IsScriptEnabled() {
			return base.IsScriptEnabled() || HasTextDecorator();
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTextEdit";
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextEditHelpText"),
#endif
		Localizable(true), DefaultValue(""), AutoFormatEnable]
		public string HelpText {
			get { return Properties.HelpText; }
			set { Properties.HelpText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextEditHelpTextSettings"),
#endif
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TextEditHelpTextSettings HelpTextSettings
		{
			get { return Properties.HelpTextSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextEditHelpTextStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public HelpTextStyle HelpTextStyle
		{
			get { return Properties.HelpTextStyle; }
		}
		internal void GetHelpTextStyle(StringBuilder stb, string localVarName) {
			HelpTextStyle style = GetHelpTextStyle();
			string cssText = style.GetStyleAttributes(Page).Value;
			stb.AppendFormat("{0}.helpTextStyle=[{1}, {2}];\n", localVarName, HtmlConvertor.ToScript(style.CssClass),
				HtmlConvertor.ToScript(cssText));
		}
		protected internal HelpTextStyle GetHelpTextStyle() {
			HelpTextStyle style = new HelpTextStyle();
			style.CopyFrom(RenderStyles.GetDefaultHelpTextStyle());
			MergeControlStyle(style, false);
			style.CopyFrom(RenderStyles.HelpText);
			return style;
		}
		protected override bool IsAnimationScriptNeeded() {
			return !string.IsNullOrEmpty(HelpText) || IsOutOfRangeWarningMode();
		}
		internal void GenerateHelpTextStartupScript(StringBuilder builder, string localVarName) {
			if (!string.IsNullOrEmpty(HelpText)) {
				HelpTextPosition helpTextPosition = HelpTextSettings.GetPosition();
				HelpTextHorizontalAlign helpTextHorizontalAlign = HelpTextSettings.GetHorizontalAlign();
				HelpTextVerticalAlign helpTextTipVerticalAlign = HelpTextSettings.GetVerticalAlign();
				Margins helpTextPopupMargins = HelpTextSettings.PopupMargins;
				string encodedHelpText = HtmlEncode(HelpText);
				builder.AppendFormat("{0}.helpText = {1};\n", localVarName, HtmlConvertor.ToScript(encodedHelpText));
				builder.AppendFormat("{0}.helpTextDisplayMode = {1};\n", localVarName, HtmlConvertor.ToScript(HelpTextSettings.DisplayMode));
				if(!HelpTextSettings.EnablePopupAnimation)
					builder.AppendFormat("{0}.enableHelpTextPopupAnimation = false;\n", localVarName);
				if (helpTextPosition != HelpTextPosition.Right)
					builder.AppendFormat("{0}.helpTextPosition = {1};\n", localVarName, HtmlConvertor.ToScript(helpTextPosition));
				if (helpTextHorizontalAlign != HelpTextHorizontalAlign.Left)
					builder.AppendFormat("{0}.helpTextHAlign = {1};\n", localVarName, HtmlConvertor.ToScript(helpTextHorizontalAlign));
				if (helpTextTipVerticalAlign != HelpTextVerticalAlign.Top)
					builder.AppendFormat("{0}.helpTextVAlign = {1};\n", localVarName, HtmlConvertor.ToScript(helpTextTipVerticalAlign));
				if (helpTextPopupMargins != null && !helpTextPopupMargins.IsEmpty)
					builder.AppendFormat("{0}.helpTextMargins = [{1},{2},{3},{4}];\n", localVarName, helpTextPopupMargins.GetMarginTop().Value,
						helpTextPopupMargins.GetMarginRight().Value, helpTextPopupMargins.GetMarginBottom().Value, helpTextPopupMargins.GetMarginLeft().Value);
				GetHelpTextStyle(builder, localVarName);
				if (ExternalTable == null) {
					AppearanceStyle externalTableStyle = GetRootStyle();
					string cssText = externalTableStyle.GetStyleAttributes(Page).Value;
					builder.AppendFormat("{0}.externalTableStyle=[{1}, {2}];\n", localVarName, HtmlConvertor.ToScript(externalTableStyle.CssClass),
						HtmlConvertor.ToScript(cssText));
				}
			}
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(AutoResizeWithContainer)
				stb.AppendFormat("{0}.autoResizeWithContainer = true;\n", localVarName);
			if(IsMaskCapabilitiesEnabled())
				GenerateMaskStartupScript(stb, localVarName);
			if(IsNullTextCapabilityEnabled())
				stb.AppendFormat("{0}.nullText = {1};\n", localVarName, HtmlConvertor.ToScript(NullTextInternal));
			if(IsDisplayFormatCapabilityEnabled())
				stb.AppendFormat("{0}.displayFormat={1};\n", localVarName, HtmlConvertor.ToScript(CommonUtils.GetFormatString(DisplayFormatString)));
			GenerateHelpTextStartupScript(stb, localVarName);
			if (IsCustomValidationEnabled && ValidationSettings.Display != Display.None && ValidationSettings.ErrorTextPosition != ErrorTextPosition.Right)
				stb.AppendFormat("{0}.errorCellPosition = {1};\n", localVarName, HtmlConvertor.ToScript(ValidationSettings.ErrorTextPosition));
			ProvideOutOfRangeWarningFields(stb, localVarName);
		}		
		void ProvideOutOfRangeWarningFields(StringBuilder stb, string localVarName) {
			if (IsOutOfRangeWarningSupported()) {
				if (IsOutOfRangeWarningMode()) {
					AppearanceStyleBase outOfRangeWarningStyle = RenderStyles.GetOutOfRangeWarningStyle(HasRightPopupHelpText());
					stb.AppendFormat("{0}.outOfRangeWarningClassName={1};\n", localVarName, HtmlConvertor.ToScript(outOfRangeWarningStyle.CssClass));
					stb.AppendFormat("{0}.outOfRangeWarningMessages=[{1}, {2}, {3}];\n", localVarName,
						HtmlConvertor.ToScript(GetInvalidValueRangeWarningMessage()),
						HtmlConvertor.ToScript(GetInvalidMinValueWarningMessage()),
						HtmlConvertor.ToScript(GetInvalidMaxValueWarningMessage()));
				}
				else
					stb.Append(localVarName + ".showOutOfRangeWarning = false;\n");
			}
		}
		protected bool HasRightPopupHelpText() {
			return !string.IsNullOrEmpty(HelpText) && HelpTextSettings.DisplayMode == HelpTextDisplayMode.Popup
				&& HelpTextSettings.GetPosition() == HelpTextPosition.Right;
		}
		protected virtual bool IsOutOfRangeWarningSupported() {
			return false;
		}
		protected virtual bool IsOutOfRangeWarningMode() {
			return false;
		}
		protected virtual string GetInvalidValueRangeWarningMessage() {
			return string.Empty;
		}
		protected virtual string GetInvalidMinValueWarningMessage() {
			return string.Empty;
		}
		protected virtual string GetInvalidMaxValueWarningMessage() {
			return string.Empty;
		}
		void GenerateMaskStartupScript(StringBuilder builder, string localVarName) {
			builder.AppendFormat("{0}.maskInfo = ASPx.MaskInfo.Create({1},{2});\n", localVarName, 
				HtmlConvertor.ToScript(MaskSettingsInternal.Mask), HtmlConvertor.ToScript(MaskSettingsInternal.IsDateTimeOnly));
			if(!MaskSettingsInternal.AllowMouseWheel)
				builder.AppendFormat("{0}.maskInfo.allowMouseWheel = false;\n", localVarName);
			if(MaskSettingsInternal.PromptChar != MaskSettings.DefaultPromptChar)
				builder.AppendFormat("{0}.maskInfo.promptChar = {1};\n", localVarName, HtmlConvertor.ToScript(MaskSettingsInternal.PromptChar));
			if(MaskSettingsInternal.IncludeLiterals != MaskIncludeLiteralsMode.All)
				builder.AppendFormat("{0}.maskInfo.includeLiterals = {1};\n", localVarName, (int)MaskSettingsInternal.IncludeLiterals);
			if(!String.IsNullOrEmpty(MaskSettingsInternal.ErrorText))
				builder.AppendFormat("{0}.maskInfo.errorText = {1};\n", localVarName, HtmlConvertor.ToScript(MaskSettingsInternal.ErrorText));
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = new Hashtable();
			if(RequireRenderRawInput())
				result.Add(RawValueKey, GetRawValue());
			return result;
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			helper.AddStyle(GetDisabledCssStyle(), "", IsEnabled());
			helper.AddStyle(GetDisabledCssStyleForInputElement(), InputControlSuffix, IsEnabled());
		}
		protected override bool HasClientInitialization() {
			return base.HasClientInitialization() || HeightCorrectionRequired || NeedFocusCorrectionWhenDisabled();
		}
		protected override bool HasFocusEvents() {
			return base.HasFocusEvents() || RequireRenderRawInput() || RequireFocusEventsForInputMaxLengthCorrection();
		}
		protected override bool HasGotFocusEvent() {
			return HasFocusEvents() || NeedFocusCorrectionWhenDisabled();
		}
		protected internal bool UseReadOnlyForDisabled() {
			return (Browser.IsIE && Browser.Version < 10) && !IsNative(); 
		}
		protected internal bool NeedFocusCorrectionWhenDisabled() {
			return (!IsEnabled() || IsClientSideAPIEnabled()) && UseReadOnlyForDisabled(); 
		}
		protected internal bool RequireFocusEventsForInputMaxLengthCorrection() {
			return Browser.IsIE && Browser.Version >= 10 && !IsNative();
		}
		protected override object GetPostBackValue(string controlName, System.Collections.Specialized.NameValueCollection postCollection) {
			if(!RequireRenderRawInput())
				return base.GetPostBackValue(controlName, postCollection);
			return GetClientObjectStateValue<string>(RawValueKey);
		}
		protected override bool IsPostBackValueSecure(object value) {
			return base.IsPostBackValueSecure(value) && (!IsMaskValidationPatternRequired() || MaskValidator.IsValueValid(value as string, MaskSettingsInternal));
		}
		protected override void CreateControlHierarchy() {
			AddMaskHintControl();
			base.CreateControlHierarchy();
		}
		void AddMaskHintControl() {
			if(!RequireRenderMaskHintControl())
				return;
			this.maskHintControl = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			this.maskHintControl.ID = "MaskHint";
			ControlsBase.Add(this.maskHintControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(this.maskHintControl != null) {
				GetMaskHintPopupStyle().AssignToControl(this.maskHintControl, true);
				this.maskHintControl.Style[HtmlTextWriterStyle.Display] = "none";
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { HelpTextSettings });
		}
		protected override string GetFocusableControlID() {
			return IsNativeRender() ? ClientID : string.Format("{0}_{1}", ClientID, InputControlSuffix);
		}
		protected virtual bool RequireRenderRawInput() {
			return HasTextDecorator();
		}
		protected virtual string GetRawValue() {
			return !IsPasswordMode() ? Text : string.Empty;
		}
		protected bool RequireRenderMaskHintControl() {
			return IsMaskCapabilitiesEnabled() && MaskSettingsInternal.ShowHints && !DesignMode;
		}
		protected internal virtual string GetInputText() {
			if(IsPasswordMode())
				return string.Empty;
			else if(DesignMode && IsMaskCapabilitiesEnabled() && !string.IsNullOrEmpty(Text))
				return "(Mask)";
			else if(string.IsNullOrEmpty(Text) && !String.IsNullOrEmpty(NullTextInternal))
				return NullTextInternal;
			else if(IsDisplayFormatCapabilityEnabled())
				return GetFormattedInputText();				
			else
				return Text ?? string.Empty;
		}
		static class DefaultInputTextFormatter {
			readonly static Regex SpecRegex = new Regex(@"(?:[^{]|^)\{\d+:([a-z])", RegexOptions.IgnoreCase);
			enum FormattingMode { Default, Integral, Decimal };
			public static string Format(string format, string text) {
				format = CommonUtils.GetFormatString(format);
				FormattingMode mode = GetFormattingMode(format);
				string fixedText = text;
				if(mode == FormattingMode.Integral) {
					long number;
					fixedText = DataUtils.FixFloatingPoint(fixedText, System.Globalization.CultureInfo.InvariantCulture);
					int pointPos = fixedText.IndexOf('.');
					if(pointPos > -1)
						fixedText = fixedText.Substring(0, pointPos);
					if(Int64.TryParse(fixedText, out number))
						return String.Format(format, number);
				}
				if(mode == FormattingMode.Decimal) {
					decimal number;
					fixedText = DataUtils.FixFloatingPoint(fixedText, System.Globalization.CultureInfo.InvariantCulture);
					if(Decimal.TryParse(fixedText, System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out number))
						return String.Format(format, number);
				}
				return String.Format(format, text);
			}
			static FormattingMode GetFormattingMode(string format) {
				MatchCollection matches = SpecRegex.Matches(format);
				if(matches.Count < 1) {
					if(format.Contains(":"))
						return FormattingMode.Decimal;
					return FormattingMode.Default;
				}
				var integralFormatCharters = new string[] { "D", "X" };
				foreach(Match match in matches) {
					var groupValue = match.Groups[1].Value;
					if(!string.IsNullOrEmpty(groupValue) && integralFormatCharters.Any(f => string.Compare(groupValue, f, true) == 0))
						return FormattingMode.Integral;
				}
				return FormattingMode.Decimal;
			}
		}
		protected virtual string GetFormattedInputText() {
			return DefaultInputTextFormatter.Format(DisplayFormatString, Text);
		}
		protected internal override void RegisterExpandoAttributes(ExpandoAttributes expandoAttributes) {
			base.RegisterExpandoAttributes(expandoAttributes);
			if(RequireRenderRawInput())
				expandoAttributes.AddAttribute("autocomplete", "off", GetFocusableControlID());
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxTextEdit), TextEditScriptResourceName);
			if(IsMaskCapabilitiesEnabled())
				RegisterIncludeScript(typeof(ASPxTextEdit), MaskScriptResourceName);
			if(IsFormatterScriptRequired())
				RegisterFormatterScript();
		}
		protected internal virtual bool IsFormatterScriptRequired() {
			return IsDisplayFormatCapabilityEnabled() || IsOutOfRangeWarningMode();
		}
		protected internal virtual bool IsCultureInfoScriptRequired() {
			return IsMaskCapabilitiesEnabled() || IsDisplayFormatCapabilityEnabled() || IsOutOfRangeWarningMode();
		}
		protected override void RegisterScriptBlocks() {
			base.RegisterScriptBlocks();
			if(IsCultureInfoScriptRequired())
				RegisterCultureInfoScript();
		}
		protected virtual bool HasOnTextChanged() {
			return IsClientSideAPIEnabled() || IsCustomValidationEnabled
				|| IsNullTextCapabilityEnabled() || IsDisplayFormatCapabilityEnabled() || this.requireValueChangedHandler;
		}
		protected internal virtual string GetOnTextChanged() {
			if(HasOnTextChanged())
				return string.Format(ValueChangedHandlerName, ClientID);
			else if(AutoPostBack)
				return GetPostBackEventReference(false, false);
			return "";
		}
		protected internal virtual string GetOnMouseOver() {
			return "";
		}
		protected override AppearanceStyle GetDefaultEditStyle() {
			return RenderStyles.GetDefaultEditAreaStyle();
		}
		protected internal EditAreaStyle GetEditAreaStyleInternal(AppearanceStyleBase baseDefaultStyle) {
			EditAreaStyle style = new EditAreaStyle();
			style.CopyFrom(baseDefaultStyle);
			PrepareEditAreaStyle(style);
			MergeDisableStyle(style);
			return style;
		}
		protected virtual void PrepareEditAreaStyle(EditAreaStyle style) {
			style.Border.Reset();
			MergeControlStyleForInput(style);
			if(style.HorizontalAlign == HorizontalAlign.NotSet)
				style.HorizontalAlign = GetControlStyle().HorizontalAlign;
		}
		protected internal EditAreaStyle GetEditAreaStyle() {
			return GetEditAreaStyleInternal(RenderStyles.GetDefaultEditAreaStyle());
		}
		protected internal AppearanceStyleBase GetMaskHintPopupStyle() {
			AppearanceStyleBase result = RenderStyles.GetDefaultMaskHintStyle();
			MergeControlStyle(result, false);
			result.CopyFrom(RenderStyles.MaskHint);
			return result;
		}
		protected AppearanceStyleBase GetNullTextStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultNullTextStyle());
			style.CopyFrom(RenderStyles.NullText);
			style.CopyFrom(NullTextStyle);
			return style;
		}
		protected virtual bool NeedNullTextStyle() {
			return IsEnabled() && !String.IsNullOrEmpty(NullTextInternal);
		}
		protected override Dictionary<string, AppearanceStyleBase> GetDecorationStyles() {
			Dictionary<string, AppearanceStyleBase> map = base.GetDecorationStyles();
			if(NeedNullTextStyle()) {
				AppearanceStyleBase style = GetNullTextStyle();
				if(!style.IsEmpty)
					map.Add("N", style);
			}
			return map;
		}
		protected override void RaiseValueChanged() {
			base.RaiseValueChanged();
			OnTextChanged(EventArgs.Empty);
		}
		protected internal virtual void OnTextChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[EventTextChanged];
			if (handler != null)
				handler(this, e);
		}
		protected internal override string GetAssociatedControlID() {
			return GetFocusableControlID();
		}
	}
}
