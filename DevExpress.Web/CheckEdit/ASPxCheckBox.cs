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
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using DevExpress.Web.Localization;
	[ControlBuilder(typeof(CheckBoxBuilder))]
	public class CheckBoxProperties: EditProperties {
		private bool isAllowGrayedInitialized = false;
		public CheckBoxProperties()
			: this(null) {
		}
		public CheckBoxProperties(IPropertiesOwner owner)
			: base(owner) {
			EnableFocusedStyle = false;
		}
		protected internal bool IsAllowGrayedInitialized {
			get { return isAllowGrayedInitialized; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesValueType"),
#endif
		NotifyParentProperty(true), DefaultValue(typeof(bool)), AutoFormatDisable,
		TypeConverter(typeof(ValueTypeTypeConverter))]
		public Type ValueType {
			get { return (Type)GetObjectProperty("ValueType", typeof(bool)); }
			set {
				if(ValueType != value) {
					SetObjectProperty("ValueType", typeof(bool), value);
					CheckValueCheckedUnchecked();
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesValueChecked"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable,
		TypeConverter(typeof(StringToObjectTypeConverter))]
		public object ValueChecked {
			get { return GetObjectProperty("ValueChecked", true); }
			set {
				value = CommonUtils.GetConvertedArgumentValue(value, ValueType, "ValueChecked");
				if(value == null)
					value = "";
				SetObjectProperty("ValueChecked", true, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesValueUnchecked"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatDisable,
		TypeConverter(typeof(StringToObjectTypeConverter))]
		public object ValueUnchecked {
			get { return GetObjectProperty("ValueUnchecked", false); }
			set {
				value = CommonUtils.GetConvertedArgumentValue(value, ValueType, "ValueUnchecked");
				if(value == null)
					value = "";
				SetObjectProperty("ValueUnchecked", false, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesValueGrayed"),
#endif
		NotifyParentProperty(true), DefaultValue(null), AutoFormatDisable,
		TypeConverter(typeof(StringToObjectTypeConverter))]
		public object ValueGrayed {
			get { return GetObjectProperty("ValueGrayed", null); }
			set {
				value = CommonUtils.GetConvertedArgumentValue(value, ValueType, "ValueGrayed");
				SetObjectProperty("ValueGrayed", null, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesAllowGrayed"),
#endif
		NotifyParentProperty(true), DefaultValue(false), AutoFormatEnable]
		public bool AllowGrayed {
			get { return GetBoolProperty("AllowGrayed", false); }
			set {
				SetBoolProperty("AllowGrayed", false, value);
				isAllowGrayedInitialized = true;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesAllowGrayedByClick"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatEnable]
		public bool AllowGrayedByClick {
			get { return GetBoolProperty("AllowGrayedByClick", true); }
			set { SetBoolProperty("AllowGrayedByClick", true, value); }
		}
		[NotifyParentProperty(true), Browsable(false), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ValueCheckedString {
			get { return ""; }
			set { ValueChecked = value; }
		}
		[NotifyParentProperty(true), Browsable(false), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ValueUncheckedString {
			get { return ""; }
			set { ValueUnchecked = value; }
		}
		[NotifyParentProperty(true), Browsable(false), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string ValueGrayedString {
			get { return string.Empty; }
			set { ValueGrayed = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesDisplayTextChecked"),
#endif
		DefaultValue(StringResources.CheckBox_Checked), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(true)]
		public string DisplayTextChecked {
			get { return GetStringProperty("DisplayTextChecked", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.CheckBox_Checked)); }
			set { SetStringProperty("DisplayTextChecked", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.CheckBox_Checked), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesDisplayTextUnchecked"),
#endif
		DefaultValue(StringResources.CheckBox_Unchecked), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(true)]
		public string DisplayTextUnchecked {
			get { return GetStringProperty("DisplayTextUnchecked", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.CheckBox_Unchecked)); }
			set { SetStringProperty("DisplayTextUnchecked", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.CheckBox_Unchecked), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesDisplayTextUndefined"),
#endif
		DefaultValue(StringResources.CheckBox_Undefined), NotifyParentProperty(true),
		Obsolete("This method is now obsolete. Use the DisplayTextGrayed property instead."),
		AutoFormatDisable, Localizable(true)]
		public string DisplayTextUndefined {
			get { return DisplayTextGrayed; }
			set { DisplayTextGrayed = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesDisplayTextGrayed"),
#endif
		DefaultValue(StringResources.CheckBox_Undefined), NotifyParentProperty(true),
		AutoFormatDisable, Localizable(true)]
		public string DisplayTextGrayed {
			get { return GetStringProperty("DisplayTextGrayed", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.CheckBox_Undefined)); }
			set { SetStringProperty("DisplayTextGrayed", ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.CheckBox_Undefined), value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesUseDisplayImages"),
#endif
		NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool UseDisplayImages {
			get { return GetBoolProperty("UseDisplayImages", true); }
			set { SetBoolProperty("UseDisplayImages", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesDisplayImageChecked"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual InternalCheckBoxImageProperties DisplayImageChecked {
			get { return Images.CheckBoxChecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesDisplayImageUnchecked"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual InternalCheckBoxImageProperties DisplayImageUnchecked {
			get { return Images.CheckBoxUnchecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesDisplayImageUndefined"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		Obsolete("This method is now obsolete. Use the DisplayImageGrayed property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual InternalCheckBoxImageProperties DisplayImageUndefined {
			get { return DisplayImageGrayed; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesDisplayImageGrayed"),
#endif
		NotifyParentProperty(true), DefaultValue(""), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual InternalCheckBoxImageProperties DisplayImageGrayed {
			get { return Images.CheckBoxGrayed; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		AutoFormatDisable, NotifyParentProperty(true)]
		public new CheckEditClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as CheckEditClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesCheckBoxFocusedStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle CheckBoxFocusedStyle { get { return Styles.CheckBoxFocused; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxPropertiesCheckBoxStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle CheckBoxStyle { get { return Styles.CheckBox; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override EditorDecorationStyle FocusedStyle {
			get { return base.FocusedStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableFocusedStyle {
			get { return base.EnableFocusedStyle; }
			set { base.EnableFocusedStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		protected internal new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				CheckBoxProperties src = source as CheckBoxProperties;
				if(src != null) {
					ValueType = src.ValueType;
					ValueChecked = src.ValueChecked;
					ValueUnchecked = src.ValueUnchecked;
					ValueGrayed = src.ValueGrayed;
					AllowGrayed = src.AllowGrayed;
					AllowGrayedByClick = src.AllowGrayedByClick;
					DisplayTextChecked = src.DisplayTextChecked;
					DisplayTextUnchecked = src.DisplayTextUnchecked;
					DisplayTextGrayed = src.DisplayTextGrayed;
					UseDisplayImages = src.UseDisplayImages;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new CheckEditClientSideEvents(this);
		}
		protected override void AssignInplaceProperties(CreateEditorArgs args) {
			base.AssignInplaceProperties(args);
			if(args.DataType != null && args.DataType != typeof(object)) {
				ValueType = args.DataType;
				ValueChecked = ValueChecked;
				ValueUnchecked = ValueUnchecked;
				ValueGrayed = ValueGrayed;
			}
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxCheckBox();
		}
		protected internal virtual AppearanceStyleBase GetCheckBoxStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultICBClass());
			style.CopyFrom(CheckBoxStyle);
			return style;
		}
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			if(!string.IsNullOrEmpty(args.DisplayText))
				return base.CreateDisplayControlInstance(args);
			else {
				CheckEditDisplayControl control = new CheckEditDisplayControl(this, args);
				control.Text = RenderUtils.CheckEmptyRenderText(GetDisplayText(args));
				control.UseImages = UseDisplayImages;
				return control;
			}
		}
		protected override string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			if(args.DisplayText != null)
				return base.GetDisplayTextCore(args, encode);
			else {
				string text = "";
				switch(GetCheckState(args.EditValue, true)) {
					case CheckState.Checked: 
						text = DisplayTextChecked;
						break;
					case CheckState.Unchecked:
						text = DisplayTextUnchecked;
						break;
					default:
						text = DisplayTextGrayed;
						break;
				}
				return text;
			}
		}
		public override HorizontalAlign GetDisplayControlDefaultAlign() {
			return UseDisplayImages ? HorizontalAlign.Center : HorizontalAlign.Left;
		}
		internal CheckState GetCheckState(object value, bool isInternal) {
			if(CommonUtils.AreEqual(value, ValueChecked, ConvertEmptyStringToNull))
				return CheckState.Checked;
			else if(CommonUtils.AreEqual(value, ValueUnchecked, ConvertEmptyStringToNull))
				return CheckState.Unchecked;
			else
				return AllowGrayed ? CheckState.Indeterminate : 
					(isInternal ? CheckState.Indeterminate : CheckState.Unchecked);
		}
		internal object GetValue(CheckState checkState, bool isInternal, object defaultValue) {
			switch(checkState) {
				case CheckState.Checked: return ValueChecked;
				case CheckState.Unchecked: return ValueUnchecked;
				default: return AllowGrayed ? ValueGrayed :
					(isInternal ? null : defaultValue);
			}
		}
		internal virtual InternalCheckBoxImageProperties GetImage(CheckState checkState, Page page) {
			InternalCheckBoxImageProperties result = new InternalCheckBoxImageProperties();
			string imageName = string.Empty;
			switch (checkState) {
				case CheckState.Checked:
					imageName = InternalCheckboxControl.CheckBoxCheckedImageName;
					result.MergeWith(Images.CheckBoxChecked);
					break;
				case CheckState.Unchecked:
					imageName = InternalCheckboxControl.CheckBoxUncheckedImageName;
					result.MergeWith(Images.CheckBoxUnchecked);
					break;
				default:
					imageName = InternalCheckboxControl.CheckBoxGrayedImageName;
					result.MergeWith(Images.CheckBoxGrayed);
					break;
			}
			Images.UpdateSpriteUrl(result, page, InternalCheckboxControl.WebSpriteControlName, typeof(ASPxWebControl), InternalCheckboxControl.DesignModeSpriteImagePath);
			result.MergeWith(Images.GetImageProperties(page, imageName));
			return result;
		}
		protected void CheckValueCheckedUnchecked() {
			ValueChecked = CommonUtils.ConvertToType(ValueChecked, ValueType, true);
			ValueUnchecked = CommonUtils.ConvertToType(ValueUnchecked, ValueType, false);
			ValueGrayed = CommonUtils.ConvertToType(ValueGrayed, ValueType, false);
		}
		public override object GetExportValue(CreateDisplayControlArgs args) {
			return null;
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DefaultProperty("Checked"), DefaultEvent("CheckedChanged"), ControlValueProperty("Value"),
	DataBindingHandler(typeof(System.Web.UI.Design.TextDataBindingHandler)),
	Designer("DevExpress.Web.Design.ASPxCheckBoxDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	ControlBuilder(typeof(CheckBoxBuilder)),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxCheckBox.bmp")
	]
	public class ASPxCheckBox : ASPxEdit, ICheckBoxControl, IInternalCheckBoxOwner, IValueTypeHolder {
		protected internal const string CheckEditScriptResourceName = EditScriptsResourcePath + "CheckEdit.js";
		private const string ClickHandlerName = "ASPx.ChkOnClick('{0}')";
		protected internal const string CheckBoxInputIDSuffix = "I";
		protected internal const string StateInputIDSuffix = "S";
		protected internal const string ICBElementIDSuffix = "S_D";
		private bool usingInsideList = false;
		private CheckState? checkState = null;
		private static readonly object EventCheckedChanged = new object();
		private ImageProperties image = null;
		public ASPxCheckBox()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxEncodeHtml"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Behavior"), DefaultValue(true), AutoFormatDisable,
		NotifyParentProperty(true)]
		public override bool EncodeHtml {
			get { return Properties.EncodeHtml; }
			set { Properties.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxLayout"),
#endif
		Category("Layout"), DefaultValue(RepeatLayout.Table), AutoFormatDisable]
		public RepeatLayout Layout {
			get { return (RepeatLayout)GetEnumProperty("Layout", RepeatLayout.Table); }
			set {
				SetEnumProperty("Layout", RepeatLayout.Table, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxWrap"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatEnable]
		public DefaultBoolean Wrap {
			get { return (ControlStyle as AppearanceStyleBase).Wrap; }
			set { (ControlStyle as AppearanceStyleBase).Wrap = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxTextAlign"),
#endif
		Category("Layout"), DefaultValue(TextAlign.Right), AutoFormatEnable]
		public TextAlign TextAlign {
			get { return (TextAlign)GetEnumProperty("TextAlign", TextAlign.Right); }
			set {
				SetEnumProperty("TextAlign", TextAlign.Right, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxTextSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit TextSpacing {
			get { return GetUnitProperty("TextSpacing", Unit.Empty); }
			set {
				CommonUtils.CheckNegativeValue(value.Value, "TextSpacing");
				SetUnitProperty("TextSpacing", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxCheckState"),
#endif
		DefaultValue(CheckState.Indeterminate), Bindable(true, BindingDirection.TwoWay), AutoFormatDisable]
		public virtual CheckState CheckState {
			get {
				return GetCheckState(false);
			}
			set {
				SetCheckState(value, false);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxChecked"),
#endif
		DefaultValue(false), Bindable(true, BindingDirection.TwoWay), AutoFormatDisable]
		public virtual bool Checked {
			get {
				return CheckState == CheckState.Checked;
			}
			set {
				CheckState = value ? CheckState.Checked : CheckState.Unchecked;
				PropertyChanged("CheckState");
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxText"),
#endif
		DefaultValue(""), Bindable(true), Localizable(true), AutoFormatDisable]
		public string Text {
			get { return GetStringProperty("Text", ""); }
			set {
				SetStringProperty("Text", "", value);
				LayoutChanged();
			}
		}
		protected internal ImageProperties Image {
			get {
				if(image == null)
					image = new ImageProperties(this);
				return image;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public CheckEditClientSideEvents ClientSideEvents {
			get { return ClientSideEventsInternal as CheckEditClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxCheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual InternalCheckBoxImageProperties CheckedImage {
			get { return Properties.Images.CheckBoxChecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxUncheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual InternalCheckBoxImageProperties UncheckedImage {
			get { return Properties.Images.CheckBoxUnchecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxGrayedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual InternalCheckBoxImageProperties GrayedImage {
			get { return Properties.Images.CheckBoxGrayed; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxValueType"),
#endif
		DefaultValue(typeof(bool)), TypeConverter(typeof(ValueTypeTypeConverter)), AutoFormatDisable]
		public virtual Type ValueType {
			get { return Properties.ValueType; }
			set {
				if(Properties.ValueType != value) {
					Properties.ValueType = value;
					PropertyChanged("ValueChecked");
					PropertyChanged("ValueUnchecked");
					PropertyChanged("ValueGrayed");
				}
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxValueChecked"),
#endif
		DefaultValue(true), TypeConverter(typeof(StringToObjectTypeConverter)), AutoFormatDisable]
		public object ValueChecked {
			get { return Properties.ValueChecked; }
			set { Properties.ValueChecked = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxValueUnchecked"),
#endif
		DefaultValue(false), TypeConverter(typeof(StringToObjectTypeConverter)), AutoFormatDisable]
		public object ValueUnchecked {
			get { return Properties.ValueUnchecked; }
			set { Properties.ValueUnchecked = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxValueGrayed"),
#endif
		DefaultValue(null), TypeConverter(typeof(StringToObjectTypeConverter)), AutoFormatDisable]
		public object ValueGrayed {
			get { return Properties.ValueGrayed; }
			set { Properties.ValueGrayed = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxAllowGrayed"),
#endif
		DefaultValue(false), AutoFormatEnable]
		public virtual bool AllowGrayed {
			get { return Properties.AllowGrayed; }
			set { Properties.AllowGrayed = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxAllowGrayedByClick"),
#endif
		DefaultValue(true), AutoFormatEnable]
		public virtual bool AllowGrayedByClick {
			get { return Properties.AllowGrayedByClick; }
			set { Properties.AllowGrayedByClick = value; }
		}	   
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxNative"),
#endif
		Category("Appearance"), DefaultValue(false), AutoFormatEnable]
		public new bool Native {
			get { return base.Native; }
			set {
				base.Native = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxCheckBoxFocusedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle CheckBoxFocusedStyle {
			get { return Properties.CheckBoxFocusedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxCheckBoxStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle CheckBoxStyle {
			get { return Properties.CheckBoxStyle; }
		}
		[DefaultValue(""), Browsable(false), Localizable(false), AutoFormatDisable]
		public string ValueCheckedString {
			get { return Properties.ValueCheckedString; }
			set { Properties.ValueCheckedString = value; }
		}
		[DefaultValue(""), Browsable(false), Localizable(false), AutoFormatDisable]
		public string ValueUncheckedString {
			get { return Properties.ValueUncheckedString; }
			set { Properties.ValueUncheckedString = value; }
		}
		[DefaultValue(""), Browsable(false), Localizable(false), AutoFormatDisable]
		public string ValueGrayedString {
			get { return Properties.ValueGrayedString; }
			set { Properties.ValueGrayedString = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override EditorDecorationStyle FocusedStyle {
			get { return base.FocusedStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnableFocusedStyle {
			get { return base.EnableFocusedStyle; }
			set { base.EnableFocusedStyle = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		protected internal CheckState GetCheckState(bool isInternal) {
			if (IsLoading()) {
				return checkState.HasValue ? checkState.Value : 
					AllowGrayed ? CheckState.Indeterminate : CheckState.Unchecked;
			}
			else
				return Properties.GetCheckState(Value, isInternal);
		}
		protected internal void SetCheckState(CheckState value, bool isInternal) {
			if (IsLoading())
				this.checkState = value;
			else
				Value = Properties.GetValue(value, isInternal, Value);
		}
		protected CheckState InternalCheckState {
			get {
				return GetCheckState(true);
			}
			set {
				SetCheckState(value, true);
			}
		}
		protected virtual AppearanceStyleBase GetICBFocusedStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultICBFocusedClass());
			style.CopyFrom(CheckBoxFocusedStyle);
			return style;
		}
		protected internal new CheckBoxProperties Properties {
			get { return (CheckBoxProperties)base.Properties; }
		}
		protected internal bool UsingInsideList {
			get { return usingInsideList; }
			set { usingInsideList = value; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new CheckBoxProperties(this);
		}
		Dictionary<string, string> GetAccessibilityCheckBoxAttributes() {
			if(!IsAriaSupported() || !IsAccessibilityCompliantRender())
				return null;
			Dictionary<string, string> settings = AccessibilityUtils.CreateCheckBoxAttributes(GetInputType(), InternalCheckState);
			settings.Add("aria-label", Text);
			if(Browser.IsIE || Browser.IsEdge)
				settings.Add("aria-selected", AccessibilityUtils.GetStringCheckedState(InternalCheckState));
			return settings;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckBoxCheckedChanged"),
#endif
		Category("Action")]
		public event EventHandler CheckedChanged
		{
			add { Events.AddHandler(EventCheckedChanged, value); }
			remove { Events.RemoveHandler(EventCheckedChanged, value); }
		}
		protected internal override void InitInternal() {
			base.InitInternal();
			if(InplaceMode == EditorInplaceMode.StandAlone)
				RestoreChecked();
		}
		private void RestoreChecked() {
			if(this.checkState.HasValue)
				InternalCheckState = this.checkState.Value;
		}
		protected override bool IsWebSourcesRegisterRequired() {
			return true;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(IsAriaSupported() && IsAccessibilityCompliantRender())
				RenderUtils.SetStringAttribute(this, "role", "presentation");
		}
		string IInternalCheckBoxOwner.GetCheckBoxInputID() {
			return StateInputIDSuffix;
		}
		bool IInternalCheckBoxOwner.IsInputElementRequired {
			get { return true; }
		}
		CheckState IInternalCheckBoxOwner.CheckState {
			get { return InternalCheckState; }
		}
		InternalCheckBoxImageProperties IInternalCheckBoxOwner.GetCurrentCheckableImage() {
			return Properties.GetImage(CheckState, Page);
		}
		AppearanceStyleBase IInternalCheckBoxOwner.InternalCheckBoxStyle {
			get { return Properties.GetCheckBoxStyle(); }
		}
		Dictionary<string, string> IInternalCheckBoxOwner.AccessibilityCheckBoxAttributes {
			get { return GetAccessibilityCheckBoxAttributes(); }
		}
		protected virtual CheckBoxControlBase CreateCheckEditControl() {
			if(Native) 
				return new CheckBoxNativeControl(this);
			else 
				return CreateCheckBoxControl();
		}
		protected virtual CheckBoxControl CreateCheckBoxControl() {
			return new CheckBoxControl(this);
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Controls.Add(CreateCheckEditControl());
		}
		protected internal Unit GetTextSpacing() {
			return !TextSpacing.IsEmpty ? TextSpacing : ((AppearanceStyle)GetControlStyle()).Spacing;
		}
		protected internal bool HasImage() {
			return !Image.IsEmpty;
		}
		protected internal bool HasText() {
			return Text != "" || DesignMode;
		}
		protected internal bool HasLabel() {
			return HasImage() || HasText();
		}
		protected internal bool HasSpan() {
			return !GetControlStyle().IsEmpty;
		}
		protected internal bool HasTable() {
			return Layout == RepeatLayout.Table && HasLabel();
		}
		protected internal string GetText() {
			return EncodeHtml ? HtmlEncode(Text) : Text;
		}
		protected virtual void OnCheckedChanged(EventArgs e) {
			EventHandler handler = Events[EventCheckedChanged] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientCheckBox";
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxCheckBox), CheckEditScriptResourceName);
		}
		protected internal string GetOnClick() {
			return !ReadOnly ? GetOnClickNormal() : GetOnClickReadonly();
		}
		protected virtual string GetOnClickNormal() {
			return string.Format(ClickHandlerName, ClientID);
		}
		protected virtual string GetOnClickReadonly() {
			return "return false;";
		}
		protected virtual string GetDefaultICBFocusedCssClass() {
			return RenderStyles.GetDefaultInternalCheckBoxStyle().CssClass;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if (!object.Equals(ValueChecked, true))
				stb.Append(string.Format("{0}.valueChecked = {1};\n", localVarName, HtmlConvertor.ToScript(ValueChecked)));
			if (!object.Equals(ValueUnchecked, false))
				stb.Append(string.Format("{0}.valueUnchecked = {1};\n", localVarName, HtmlConvertor.ToScript(ValueUnchecked)));
			if (!Native && Enabled) {
				stb.Append(string.Format("{0}.imageProperties = {1};\n", localVarName, ImagePropertiesSerializer.GetImageProperties(GetImages(), this)));
				stb.Append(string.Format("{0}.icbFocusedStyle = {1};\n", localVarName, InternalCheckboxControl.SerializeFocusedStyle(GetICBFocusedStyle(), this)));
				if (AllowGrayed) {
					if (!object.Equals(ValueGrayed, null))
						stb.Append(string.Format("{0}.valueGrayed = {1};\n", localVarName, HtmlConvertor.ToScript(ValueGrayed)));
					if (!AllowGrayedByClick)
						stb.Append(string.Format("{0}.allowGrayedByClick = false;\n", localVarName));
				}
			}
		}
		#region Images
		protected InternalCheckBoxImageProperties GetCheckedImage() {
			return Properties.GetImage(CheckState.Checked, Page);
		}
		protected InternalCheckBoxImageProperties GetUncheckedImage() {
			return Properties.GetImage(CheckState.Unchecked, Page);
		}
		protected InternalCheckBoxImageProperties GetGrayedImage() {
			return Properties.GetImage(CheckState.Indeterminate, Page);
		}
		protected List<InternalCheckBoxImageProperties> GetImages() {
			List<InternalCheckBoxImageProperties> result = new List<InternalCheckBoxImageProperties>(new InternalCheckBoxImageProperties[] { GetCheckedImage(), GetUncheckedImage() });
			if(AllowGrayed)
				result.Add(GetGrayedImage());
			return result;
		}
		#endregion
		protected internal virtual string GetInputType() {
			return "checkbox";
		}
		protected internal virtual string GetInputName() {
			return "";
		}
		protected internal virtual string GetInputValue() {
			return "";
		}
		protected internal string GetStatus() {
			return InternalCheckboxControl.GetCheckStateKey(InternalCheckState);
		}
		protected override void PrepareControlStyleCore(AppearanceStyleBase style) {
			style.CopyFrom(RenderStyles.GetDefaultCheckEditStyle());
			MergeParentSkinOwnerControlStyle(style);
			style.CopyFrom(RenderStyles.CheckEdit);
			style.CopyFrom(RenderStyles.Style);
			style.CopyFrom(ControlStyle);
			MergeDisableStyle(style);
		}
		protected override void PostPrepareControlStyle(AppearanceStyleBase style) {
			style.Wrap = DefaultBoolean.Default; 
		}
		protected internal override string GetAssociatedControlID() {
			if(HasTable() || HasSpan())
				return string.Format("{0}_{1}", ClientID, GetAssociatedControlIDSuffix());
			else
				return ClientID;
		}
		string GetAssociatedControlIDSuffix() {
			string suffix = "";
			if(Native)
				suffix = CheckBoxInputIDSuffix;
			else
				suffix = IsAccessibilityCompliantRender() ? ICBElementIDSuffix : StateInputIDSuffix;
			return suffix;
		}
		protected internal override bool IsAccessibilityAssociatingSupported() {
			return RenderUtils.IsHtml5Mode(this) && IsAccessibilityCompliantRender() && !IsNativeRender();
		}
		protected override void RaiseValueChanged() {
			base.RaiseValueChanged();
			OnCheckedChanged(EventArgs.Empty);
		}
		protected override object GetPostBackValue(string controlName, System.Collections.Specialized.NameValueCollection postCollection) {
			string state = postCollection[UniqueID];
			switch(state) {
				case InternalCheckboxControl.CheckedStateKey:
					return ValueChecked;
				case InternalCheckboxControl.UncheckedStateKey:
					return ValueUnchecked;
				default:
					return null;
			}
		}
	}
}
namespace DevExpress.Web.Internal {
	public class CheckBoxBuilder : ThemableControlBuilder {
		private void InitializeEditorProperties(Type type, IDictionary attribs) {
			object value;
			value = attribs["ValueChecked"];
			if(value != null) {
				attribs["ValueCheckedString"] = value;
				attribs.Remove("ValueChecked");
			}
			value = attribs["ValueUnchecked"];
			if(value != null) {
				attribs["ValueUncheckedString"] = value;
				attribs.Remove("ValueUnchecked");
			}
			value = attribs["ValueGrayed"];
			if(value != null) {
				attribs["ValueGrayedString"] = value;
				attribs.Remove("ValueGrayed");
			}
		}
		public override void Init(TemplateParser parser, ControlBuilder parentBuilder, Type type, string tagName, string id, IDictionary attribs) {
			InitializeEditorProperties(type, attribs);
			base.Init(parser, parentBuilder, type, tagName, id, attribs);
		}
	}
}
