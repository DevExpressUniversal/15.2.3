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
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	public abstract class TextBoxPropertiesBase : TextEditProperties {
		public TextBoxPropertiesBase()
			: base() {
		}
		public TextBoxPropertiesBase(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextBoxPropertiesBasePassword"),
#endif
		Category("Behavior"), DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool Password {
			get { return GetBoolProperty("Password", false); }
			set { SetBoolProperty("Password", false, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextBoxPropertiesBaseMaxLength"),
#endif
		Category("Behavior"), DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public virtual int MaxLength {
			get { return GetIntProperty("MaxLength", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "MaxLength");
				SetIntProperty("MaxLength", 0, value);
			}
		}
		protected internal new TextBoxClientSideEventsBase ClientSideEvents {
			get { return (TextBoxClientSideEventsBase)base.ClientSideEvents; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				TextBoxPropertiesBase src = source as TextBoxPropertiesBase;
				if(src != null) {
					Password = src.Password;
					MaxLength = src.MaxLength;
				}
			} finally {
				EndUpdate();
			}
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new TextBoxClientSideEventsBase(this);
		}
	}
	public abstract class ASPxPureTextBoxBase : ASPxTextEdit {
		public ASPxPureTextBoxBase() 
			: base() {
		}
		protected ASPxPureTextBoxBase(ASPxWebControl owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPureTextBoxBaseHorizontalAlign"),
#endif
		Category("Layout"), DefaultValue(HorizontalAlign.NotSet), AutoFormatEnable]
		public HorizontalAlign HorizontalAlign {
			get { return ((AppearanceStyle)ControlStyle).HorizontalAlign; }
			set { ((AppearanceStyle)ControlStyle).HorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxPureTextBoxBasePaddings"),
#endif
		Category("Layout"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public Paddings Paddings {
			get { return ((AppearanceStyle)ControlStyle).Paddings; }
		}
		protected internal new TextBoxClientSideEventsBase ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		protected internal new TextBoxPropertiesBase Properties {
			get { return (TextBoxPropertiesBase)base.Properties; }
		}
		protected override void PrepareEditAreaStyle(EditAreaStyle style) {
			base.PrepareEditAreaStyle(style);
			if(HorizontalAlign != HorizontalAlign.NotSet)
				style.HorizontalAlign = HorizontalAlign;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Paddings });
		}
	}
	public abstract class ASPxTextBoxBase : ASPxPureTextBoxBase {
		public ASPxTextBoxBase() 
			: base() {
		}
		protected ASPxTextBoxBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextBoxBaseAutoCompleteType"),
#endif
		Category("Behavior"), DefaultValue(AutoCompleteType.None), AutoFormatDisable]
		public virtual AutoCompleteType AutoCompleteType {
			get { return (AutoCompleteType)GetObjectProperty("AutoCompleteType", AutoCompleteType.None); }
			set { SetObjectProperty("AutoCompleteType", AutoCompleteType.None, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextBoxBaseEncodeHtml"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable,
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public override bool EncodeHtml {
			get { return base.EncodeHtml; }
			set { base.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextBoxBaseMaxLength"),
#endif
		Category("Behavior"), DefaultValue(0), AutoFormatDisable]
		public virtual int MaxLength {
			get { return Properties.MaxLength; }
			set { Properties.MaxLength = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextBoxBasePassword"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public virtual bool Password {
			get { return Properties.Password; }
			set { Properties.Password = value; }
		}
		protected override bool IsPasswordMode() {
			return Password;
		}
		private HtmlAttribute GetAutoCompleteAttribute() {
			HtmlAttribute autocompete = new HtmlAttribute();
			if(AutoCompleteType == AutoCompleteType.Disabled)
				return new HtmlAttribute("autocomplete", "off");
			if(AutoCompleteType == AutoCompleteType.None || Context.Request.Browser["supportsVCard"] != "true")
				return HtmlAttribute.Empty;
			string vCard = "";
			switch(AutoCompleteType) {
				case AutoCompleteType.Search:
					vCard = "search";
					break;
				case AutoCompleteType.HomeCountryRegion:
					vCard = "HomeCountry";
					break;
				case AutoCompleteType.BusinessCountryRegion:
					vCard = "BusinessCountry";
					break;
				default:
					vCard = Enum.Format(typeof(AutoCompleteType), AutoCompleteType, "G");
					if(vCard.StartsWith("Business", StringComparison.Ordinal))
						vCard = vCard.Insert(8, ".");
					else if(vCard.StartsWith("Home", StringComparison.Ordinal))
						vCard = vCard.Insert(4, ".");
					vCard = "VCard." + vCard;
					break;
			}
			return new HtmlAttribute("vcard_name", vCard);
		}
		protected internal override void RegisterExpandoAttributes(ExpandoAttributes expandoAttributes) {
			base.RegisterExpandoAttributes(expandoAttributes);
			if(IsAutoCompleteAttributeAllowed())
				RegisterAutoCompleteAttribute(expandoAttributes);
		}
		protected void RegisterAutoCompleteAttribute(ExpandoAttributes expandoAttributes) {
			HtmlAttribute autoComplete = GetAutoCompleteAttribute();
			expandoAttributes.AddAttribute(autoComplete.Name, autoComplete.Value, GetFocusableControlID());
		}
		protected virtual bool IsAutoCompleteAttributeAllowed() {
			return true;
		}
		protected override bool IsPostBackValueSecure(object value) {
			bool result = base.IsPostBackValueSecure(value);
			string stringValue = value as string;
			if (!string.IsNullOrEmpty(stringValue) && MaxLength != 0)
				result = result && stringValue.Length <= MaxLength;
			return result;
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTextBoxBase";
		}
		protected internal virtual bool IsUserInputAllowed() {
			return true;
		}
	}
	public class TextBoxProperties : TextBoxPropertiesBase {
		public TextBoxProperties()
			: base() {
		}
		public TextBoxProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextBoxPropertiesNative"),
#endif
		Category("Appearance"), DefaultValue(false), NotifyParentProperty(true), AutoFormatEnable]
		public new bool Native {
			get { return base.Native; }
			set {
				base.Native = value;
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextBoxPropertiesSize"),
#endif
		DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int Size {
			get { return GetIntProperty("Size", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "Size");
				SetIntProperty("Size", 0, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextBoxPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		AutoFormatDisable, MergableProperty(false), NotifyParentProperty(true)]
		public new TextBoxClientSideEvents ClientSideEvents {
			get { return (TextBoxClientSideEvents)base.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextBoxPropertiesMaskSettings"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable, NotifyParentProperty(true)]
		public MaskSettings MaskSettings {
			get { return MaskSettingsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextBoxPropertiesNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string NullText {
			get { return NullTextInternal; }
			set { NullTextInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TextBoxPropertiesMaskHintStyle"),
#endif
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public MaskHintStyle MaskHintStyle {
			get { return Styles.MaskHint; }
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxTextBox();
		}
		protected override string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			if(Password && !CommonUtils.IsNullValue(args.EditValue))
				args.EditValue = GetMaskedPasswordText(args.EditValue.ToString());
			return base.GetDisplayTextCore(args, encode);
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new TextBoxClientSideEvents(this);
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				TextBoxProperties src = source as TextBoxProperties;
				if(src != null) {
					Size = src.Size;
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal string GetMaskedPasswordText(string text) {
			return string.Empty.PadRight(text.Length, '*');
		}
		protected override bool IsNativeSupported() {
			return true;
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	ToolboxData("<{0}:ASPxTextBox runat=\"server\" Width=\"170px\"></{0}:ASPxTextBox>"),
	Designer("DevExpress.Web.Design.ASPxTextBoxDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxTextBox.bmp")]
	public class ASPxTextBox : ASPxTextBoxBase {
		public ASPxTextBox()
			: base() {
		}
		protected ASPxTextBox(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextBoxNative"),
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
	DevExpressWebLocalizedDescription("ASPxTextBoxSize"),
#endif
		DefaultValue(0), AutoFormatDisable]
		public int Size {
			get { return Properties.Size; }
			set {
				if(value > 0)
					Width = Unit.Empty;
				PropertyChanged("Width");
				Properties.Size = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextBoxClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable, MergableProperty(false)]
		public new TextBoxClientSideEvents ClientSideEvents {
			get { return (TextBoxClientSideEvents)base.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextBoxMaskSettings"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatDisable]
		public MaskSettings MaskSettings {
			get { return Properties.MaskSettings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextBoxNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string NullText {
			get { return Properties.NullText; }
			set { Properties.NullText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextBoxWidth"),
#endif
 Category("Layout")]
		public override Unit Width {
			get { return base.Width; }
			set {
				if(!value.IsEmpty)
					Size = 0;
				PropertyChanged("Size");
				base.Width = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTextBoxMaskHintStyle"),
#endif
		Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public MaskHintStyle MaskHintStyle {
			get { return Properties.MaskHintStyle; }
		}
		protected internal TextBoxControl TextBoxControl { get; private set; }
		protected internal new TextBoxProperties Properties {
			get { return (TextBoxProperties)base.Properties; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new TextBoxProperties(this);
		}
		protected internal override void ValidateProperties() {
			base.ValidateProperties();
			if(!string.IsNullOrEmpty(MaskSettings.Mask) && MaxLength > 0) {
				int maskCapacity = new Mask(MaskSettings).GetCapacity();
				if(MaxLength < maskCapacity)
					throw new ArgumentException("value", string.Format(StringResources.ASPxTextBox_MaxLengthAgainstMaskErrorText, maskCapacity));
			}
		}
		protected override AppearanceStyle GetDefaultEditStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CssClass = EditorStyles.TextBoxSystemClassName;
			style.CopyFrom(RenderStyles.GetDefaultTextBoxStyle());
			return style;
		}
		protected override AppearanceStyle GetEditStyleFromStylesStorage() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.TextBox);
			return style;
		}
		internal sealed override bool IsInputStretched {
			get { return Size == 0; }
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(IsNativeRender())
				Controls.Add(new TextBoxNativeControl(this));
			else {
				TextBoxControl = CreateTextBoxControl();
				Controls.Add(TextBoxControl);
			}  
		}
		protected virtual TextBoxControl CreateTextBoxControl() {
			return new TextBoxControl(this);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(!String.IsNullOrEmpty(AccessibilityInputTitle))
				TextBoxControl.SetAccessibilityInputTitle(AccessibilityInputTitle);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTextBox";
		}
	}
}
