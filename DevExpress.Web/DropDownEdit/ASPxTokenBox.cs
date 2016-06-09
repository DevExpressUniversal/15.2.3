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
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	public enum ShowDropDownOnFocusMode { Always, Never, [Obsolete("This value is not used any more.")]Auto }
	public class TokenBoxProperties : AutoCompleteBoxPropertiesBase, IControlDesigner {
		private TokenCollection tokens;
		private static readonly object eventTokensChanged = new object();
		private static readonly object eventCustomTokensAdded = new object();
		public TokenBoxProperties()
			: base() {
		}
		public TokenBoxProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesAllowCustomTokens"),
#endif
		DefaultValue(true), NotifyParentProperty(true), Localizable(false), AutoFormatDisable]
		public bool AllowCustomTokens {
			get { return (DropDownStyleInternal == DropDownStyle.DropDown); }
			set { DropDownStyleInternal = value ? DropDownStyle.DropDown : DropDownStyle.DropDownList; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ClearButton ClearButton {
			get {
				return base.ClearButton;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesValueSeparator"),
#endif
		DefaultValue(','), NotifyParentProperty(true), Localizable(false), AutoFormatDisable]
		public char ValueSeparator {
			get { return GetCharProperty("ValueSeparator", ','); }
			set { SetCharProperty("ValueSeparator", ',', value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesItemValueType"),
#endif
 NotifyParentProperty(true),
		TypeConverter(typeof(ListEditValueTypeTypeConverter)), AutoFormatDisable, Themeable(false), DefaultValue(typeof(String))]
		public Type ItemValueType {
			get { return ValueTypeInternal; }
			set { ValueTypeInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesTextSeparator"),
#endif
		DefaultValue(','), NotifyParentProperty(true), Localizable(false), AutoFormatDisable]
		public char TextSeparator {
			get { return GetCharProperty("TextSeparator", ','); }
			set { SetCharProperty("TextSeparator", ',', value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesTokens"),
#endif
 NotifyParentProperty(true), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), TypeConverter(typeof(TokenCollectionConverter)),
		Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public TokenCollection Tokens {
			get {
				if(tokens == null)
					tokens = new TokenCollection(this);
				return tokens;
			}
			set {
				SynchronizeTokens(value.ToArray(), AllowCustomTokens, new ArrayList());
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesClientSideEvents"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false),
		NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public new TokenBoxClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as TokenBoxClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesCaseSensitiveTokens"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool CaseSensitiveTokens {
			get { return GetBoolProperty("CaseSensitiveTokens", true); }
			set {
				SetBoolProperty("CaseSensitiveTokens", true, value);
				Tokens.ChangedInternal();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesShowDropDownOnFocus"),
#endif
		DefaultValue(ShowDropDownOnFocusMode.Always), NotifyParentProperty(true), AutoFormatDisable]
		public ShowDropDownOnFocusMode ShowDropDownOnFocus {
			get { return (ShowDropDownOnFocusMode)GetEnumProperty("ShowDropDownOnFocus", ShowDropDownOnFocusMode.Always); }
			set { SetEnumProperty("ShowDropDownOnFocus", ShowDropDownOnFocusMode.Always, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesTokenStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExtendedStyleBase TokenStyle {
			get { return Styles.TokenBoxTokenStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesTokenHoverStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExtendedStyleBase TokenHoverStyle {
			get { return Styles.TokenBoxTokenHoverStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesTokenTextStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExtendedStyleBase TokenTextStyle {
			get { return Styles.TokenBoxTokenTextStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesTokenBoxInputStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExtendedStyleBase TokenBoxInputStyle {
			get { return Styles.TokenBoxInputStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesTokenRemoveButtonStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExtendedStyleBase TokenRemoveButtonStyle {
			get { return Styles.TokenBoxTokenRemoveButtonStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("TokenBoxPropertiesTokenRemoveButtonHoverStyle"),
#endif
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ExtendedStyleBase TokenRemoveButtonHoverStyle {
			get { return Styles.TokenBoxTokenRemoveButtonHoverStyle; }
		}
		protected override internal DropDownStyle DropDownStyleInternal {
			get { return (DropDownStyle)GetEnumProperty("DropDownStyleInternal", DropDownStyle.DropDown); }
			set { SetEnumProperty("DropDownStyleInternal", DropDownStyle.DropDown, value); }
		}
		protected internal ASPxTokenBox TokenBox {
			get { return AutoCompleteBox as ASPxTokenBox; }
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Tokens });
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				TokenBoxProperties src = source as TokenBoxProperties;
				if(src != null) {
					AllowCustomTokens = src.AllowCustomTokens;
					CaseSensitiveTokens = src.CaseSensitiveTokens;
					ShowDropDownOnFocus = src.ShowDropDownOnFocus;
					TextSeparator = src.TextSeparator;
					ValueSeparator = src.ValueSeparator;
					ItemValueType = src.ItemValueType;
					Tokens.Assign(src.Tokens);
					TokenStyle.Assign(src.TokenStyle);
					TokenHoverStyle.Assign(src.TokenHoverStyle);
					TokenTextStyle.Assign(src.TokenTextStyle);
					TokenBoxInputStyle.Assign(src.TokenBoxInputStyle);
					TokenRemoveButtonStyle.Assign(src.TokenRemoveButtonStyle);
					TokenRemoveButtonHoverStyle.Assign(src.TokenRemoveButtonHoverStyle);
				}
			} finally {
				EndUpdate();
			}
		}
		protected internal object EventCustomTokensAdded { get { return eventCustomTokensAdded; } }
		protected internal object EventTokensChanged { get { return eventTokensChanged; } }
		protected internal string[] ParseValue(string value) {
			string[] stringValues = value.Split(new string[] { ValueSeparator.ToString() }, StringSplitOptions.None).Distinct().ToArray();
			string[] tokens = new string[stringValues.Length];
			for(int i = 0; i < stringValues.Length; i++) {
				ListEditItem item = Items.FindByValue(CommonUtils.GetConvertedArgumentValue(stringValues[i], ValueTypeInternal, "value"));
				tokens[i] = item == null ? stringValues[i] : item.Text;
			}
			return tokens;
		}
		protected internal void SynchronizeTokens(string[] tokens, bool raiseCustomTokenAdded, ArrayList customTokens) {
			bool shouldUpdateTokens = true;
			foreach (string token in tokens) {
				if (raiseCustomTokenAdded || DataSecurityMode == DataSecurityMode.Strict) {
					ListEditItem item = TokenBox.FindItemByText(token);
					if (item == null) {
						if (DataSecurityMode == DataSecurityMode.Default)
							customTokens.Add(token);
						else {
							shouldUpdateTokens = false;
							break;
						}
					}
				}
			}
			if (shouldUpdateTokens) {
				Tokens.Clear();
				Tokens.AddRange(tokens);
			}
			for(int i = customTokens.Count - 1; i >= 0; i--) {
				if(!Tokens.Contains(customTokens[i]))
					customTokens.RemoveAt(i);
			}
		}
		protected override bool IsNativeSupported() {
			return false;
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new TokenBoxClientSideEvents(this);
		}
		protected override Control CreateDisplayControlInstance(CreateDisplayControlArgs args) {
			object convertedValue = ListEditPropertiesHelper.GetConvertedValue(args, typeof(string));
			if(!string.IsNullOrEmpty(args.DisplayText) || convertedValue == null)
				return base.CreateDisplayControlInstance(args);
			else
				return ListEditPropertiesHelper.CreateDisplayControlInstance(args, convertedValue, ItemImage, DisplayImageSpacing);
		}
		protected override string GetDisplayTextCore(CreateDisplayControlArgs args, bool encode) {
			if(args.DisplayText != null || CommonUtils.IsNullValue(args.EditValue))
				return base.GetDisplayTextCore(args, encode);
			else {
				CheckInplaceBound(args.DataType, args.Parent, args.DesignMode);
				string text = String.Join(TextSeparator.ToString() + " ", ParseValue(args.EditValue.ToString()));
				if(encode)
					text = HttpUtility.HtmlEncode(text);
				return text;
			}
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxTokenBox();
		}
		protected internal override ASPxEditBase CreateEdit(CreateEditControlArgs args, bool isInternal, Func<ASPxEditBase> createEditInstance) {
			ASPxEditBase control = base.CreateEdit(args, isInternal, createEditInstance);
			ASPxTokenBox tokenBox = control as ASPxTokenBox;
			if(tokenBox != null) {
				tokenBox.Value = args.EditValue == null ? "" : args.EditValue.ToString();
				if(isInternal) {
					tokenBox.DataSource = null;
					tokenBox.DataSourceID = "";
				}
			}
			return control;
		}
		#region HIDDEN_PROPERTIES
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new ButtonImageProperties ButtonEditEllipsisImage {
			get { return base.ButtonEditEllipsisImage; }
		}
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new EditButtonCollection Buttons {
			get { return base.Buttons; }
		}
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new ITemplate ButtonTemplate {
			get { return base.ButtonTemplate; }
			set { base.ButtonTemplate = value; }
		}
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new EditButtonStyle ButtonStyle {
			get { return base.ButtonStyle; }
		}
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new DropDownButton DropDownButton {
			get { return base.DropDownButton; }
		}
		[DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
		#endregion HIDDEN_PROPERTIES
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.TokenBoxItemsOwner"; } }
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DefaultProperty("Text"), DefaultEvent("TokensChanged"),
	Designer("DevExpress.Web.Design.ASPxTokenBoxDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxTokenBox.bmp"),
	ToolboxData("<{0}:ASPxTokenBox runat=\"server\" ItemValueType=\"System.String\"></{0}:ASPxTokenBox>")
	]
	public class ASPxTokenBox : ASPxAutoCompleteBoxBase, IControlDesigner {
		protected internal const string TokenBoxScriptResourceName = EditScriptsResourcePath + "TokenBox.js";
		internal const string TokensHiddenInputID = "TK";
		internal const string TokensValuesHiddenInputID = "TKV";
		private string internalText = "";
		private string internalValue = "";
		private bool needSyncTokensWithItems = false;
		protected internal const string TokenRBClickHandlerName = "ASPx.TRBClick('{0}', event)";
		protected const string MainElementMouseDownHandlerName = "return ASPx.ME_MD('{0}', event)";
		protected internal const string TokenRemoveButtonID = "TRB";
		public ASPxTokenBox()
			: base() {
		}
		protected ASPxTokenBox(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		protected internal ASPxTokenBox(ASPxWebControl ownerControl, TokenBoxProperties properties)
			: base(ownerControl, properties) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxAllowCustomTokens"),
#endif
		Category("Behavior"), DefaultValue(true), Localizable(false), AutoFormatDisable]
		public bool AllowCustomTokens {
			get { return Properties.AllowCustomTokens; }
			set { Properties.AllowCustomTokens = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ClearButton ClearButton {
			get {
				return base.ClearButton;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public new TokenBoxClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxItemValueType"),
#endif
 NotifyParentProperty(true),
		TypeConverter(typeof(ListEditValueTypeTypeConverter)), AutoFormatDisable, Themeable(false), DefaultValue(typeof(String))]
		public Type ItemValueType {
			get { return Properties.ItemValueType; }
			set { Properties.ItemValueType = value; }
		}
		[
		Category("Behavior"), DefaultValue(true), Localizable(false), AutoFormatDisable]
		public bool CaseSensitiveTokens {
			get { return Properties.CaseSensitiveTokens; }
			set { Properties.CaseSensitiveTokens = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxShowDropDownOnFocus"),
#endif
		Category("Behavior"), DefaultValue(ShowDropDownOnFocusMode.Always), Localizable(false), AutoFormatDisable]
		public ShowDropDownOnFocusMode ShowDropDownOnFocus {
			get { return Properties.ShowDropDownOnFocus; }
			set { Properties.ShowDropDownOnFocus = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxTokens"),
#endif
 Category("Misc"), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), TypeConverter(typeof(TokenCollectionConverter)),
		Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public TokenCollection Tokens {
			get { return Properties.Tokens; }
			set { Properties.Tokens = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxValueSeparator"),
#endif
		Category("Misc"), DefaultValue(','), Localizable(false), AutoFormatDisable]
		public char ValueSeparator {
			get { return Properties.ValueSeparator; }
			set { Properties.ValueSeparator = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxTextSeparator"),
#endif
		Category("Misc"), DefaultValue(','), Localizable(false), AutoFormatDisable]
		public char TextSeparator {
			get { return Properties.TextSeparator; }
			set { Properties.TextSeparator = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxText"),
#endif
 Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text {
			get {
				if(String.IsNullOrEmpty(InternalText))
					InternalText = JoinText();
				return InternalText;
			}
			set {
				if(String.IsNullOrEmpty(value))
					ClearTokens();
				else
					ParseText(value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxValue"),
#endif
 Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override object Value {
			get {
				if(String.IsNullOrEmpty(InternalValue))
					InternalValue = JoinValue();
				return InternalValue;
			}
			set {
				string valueAsString = value as string;
				if(string.IsNullOrEmpty(valueAsString))
					ClearTokens();
				else
					ParseValue(valueAsString);
				NeedSyncTokensWithItems = true;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxTokenStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenStyle {
			get { return Properties.TokenStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxTokenHoverStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenHoverStyle {
			get { return Properties.TokenHoverStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxTokenTextStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenTextStyle {
			get { return Properties.TokenTextStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxTokenBoxInputStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenBoxInputStyle {
			get { return Properties.TokenBoxInputStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxTokenRemoveButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenRemoveButtonStyle {
			get { return Properties.TokenRemoveButtonStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxTokenRemoveButtonHoverStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ExtendedStyleBase TokenRemoveButtonHoverStyle {
			get { return Properties.TokenRemoveButtonHoverStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxTokensChanged"),
#endif
		Category("Action")]
		public event EventHandler TokensChanged
		{
			add { Events.AddHandler(Properties.EventTokensChanged, value); }
			remove { Events.RemoveHandler(Properties.EventTokensChanged, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxTokenBoxCustomTokensAdded"),
#endif
		Category("Action")]
		public event TokenBoxCustomTokensAddedEventHandler CustomTokensAdded
		{
			add { Events.AddHandler(Properties.EventCustomTokensAdded, value); }
			remove { Events.RemoveHandler(Properties.EventCustomTokensAdded, value); }
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			if(PerformDataBindingCore(dataHelperName, data)) {
				SyncTokensWithItems();
				ResetControlHierarchy();
			}
		}
		protected void SyncTokensWithItems() {
			if(NeedSyncTokensWithItems && Items.Count > 0) {
				string[] tokens = new string[Tokens.Count];
				for(int i = 0; i < Tokens.Count; i++ ) {
					string tokenText = Tokens[i];
					if(Items.FindByText(tokenText) == null) {
						ListEditItem item = Items.FindByValue(CommonUtils.GetConvertedArgumentValue(tokenText, ItemValueType, "value"));
						tokenText = item == null ? tokenText : item.Text;
					}
					tokens[i] = tokenText;
				}
				Tokens.Clear();
				Tokens.AddRange(tokens);
				NeedSyncTokensWithItems = false;
			}
		}
		protected internal new TokenBoxProperties Properties {
			get { return (TokenBoxProperties)base.Properties; }
		}
		protected internal string InternalValue {
			get { return this.internalValue; }
			set { this.internalValue = value; }
		}
		protected internal string InternalText {
			get { return this.internalText; }
			set { this.internalText = value; }
		}
		protected override EditPropertiesBase CreatePropertiesInternal() {
			return new TokenBoxProperties(this);
		}
		protected internal override string GetInputText() {
			if(!String.IsNullOrEmpty(NullText) && Tokens.Count == 0)
				return NullText;
			return "";
		}
		protected internal string GetTokenRemoveButtonToolTip() {
			return ASPxEditorsLocalizer.GetString(ASPxEditorsStringId.TokenBox_TokenRemoveButtonToolTip);
		}
		protected internal string GetTokensHiddenInputID() {
			return ClientID + "_" + TokensHiddenInputID;
		}
		protected internal string GetTokensValuesHiddenInputID() {
			return ClientID + "_" + TokensValuesHiddenInputID;
		}
		protected internal void ResolveImageUrl(ImageProperties properties) {
			if(Page != null) {
				properties.Url = Page.ResolveClientUrl(properties.Url);
			}
		}
		protected internal ExtendedStyleBase GetTokenBoxTokenStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultTokenBoxTokenStyle());
			style.CopyFrom(RenderStyles.TokenBoxTokenStyle);
			return style;
		}
		protected internal ExtendedStyleBase GetTokenBoxTokenHoveredStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(RenderStyles.TokenBoxTokenHoverStyle);
			return style;
		}
		protected internal ExtendedStyleBase GetTokenBoxTokenRemoveButtonStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultTokenBoxTokenRemoveButtonStyle());
			style.CopyFrom(RenderStyles.TokenBoxTokenRemoveButtonStyle);
			return style;
		}
		protected internal ImageProperties GetTokenBoxTokenRemoveButtonDefaultImage() {
			return RenderImages.GetImageProperties(Page, EditorImages.TokenBoxTokenRemoveButtonImageName);
		}
		protected internal ExtendedStyleBase GetTokenBoxTokenRemoveButtonHoverStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(RenderStyles.TokenBoxTokenRemoveButtonHoverStyle);
			return style;
		}
		protected internal ExtendedStyleBase GetTokenBoxTokenTextStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultTokenBoxTokenTextStyle());
			style.CopyFrom(RenderStyles.TokenBoxTokenTextStyle);
			return style;
		}
		protected internal ExtendedStyleBase GetTokenBoxInputStyle() {
			ExtendedStyleBase style = new ExtendedStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultTokenBoxInputStyle());
			style.CopyFrom(RenderStyles.TokenBoxInputStyle);
			return style;
		}
		protected override bool IsPostBackValueSecure(object value) {
			return false;
		}
		protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
			bool loadPostData = base.LoadPostData(postCollection);
			return LoadTokens(postCollection) || loadPostData;
		}
		protected bool LoadTokens(NameValueCollection postCollection) {
			string stateString = postCollection[GetTokensHiddenInputID()];
			string[] tokens = !String.IsNullOrEmpty(stateString) ? DeserializeTokens(stateString) : new string[0];
			return SynchronizeTokens(tokens, true, AllowCustomTokens);
		}
		protected void ClearTokens() {
			Tokens.Clear();
			TokensChangedInternal();
		}
		protected bool SynchronizeTokens(string[] tokens, bool monitorTokensChanged, bool raiseCustomTokenAdded) {
			ArrayList customTokens = new ArrayList();
			bool tokensChanged = false;
			string[] tokensBeforeSynchronization = Tokens.ToArray();
			Properties.SynchronizeTokens(tokens, raiseCustomTokenAdded, customTokens);
			if(monitorTokensChanged) {
				tokensChanged = !Tokens.SequenceEqual(tokensBeforeSynchronization);
				if(tokensChanged)
					RaiseTokensChanged();
				if(customTokens.Count > 0)
					RaiseCustomTokenAdded(customTokens.Cast<string>());
			}
			return tokensChanged;
		}
		internal override ValidationResult ValidateInternal() {
			if(!ValidationSettings.ValidationPatterns.RequiredField.IsEmpty) {
				if(!ValidationSettings.ValidationPatterns.RequiredField.EvaluateIsValid(Value))
					return new ValidationResult(false, ValidationSettings.ValidationPatterns.RequiredField.ErrorText);
			}
			if(!ValidationSettings.ValidationPatterns.RegularExpression.IsEmpty) {
				foreach(string value in JoinValue().Split(ValueSeparator)) {
					if(!ValidationSettings.ValidationPatterns.RegularExpression.EvaluateIsValid(value))
						return new ValidationResult(false, ValidationSettings.ValidationPatterns.RegularExpression.ErrorText);
				}
			}
			return ValidationResult.Success;
		}
		protected internal void TokensChangedInternal() {
			LayoutChanged();
			InternalValue = null;
			InternalText = null;
		}
		protected internal string JoinValue() {
			string[] values = new string[Tokens.Count];
			for(int i = 0; i < Tokens.Count; i++) {
				ListEditItem item = FindItemByText(Tokens[i]);
				if(item != null) {
					values[i] = item.Value != null ? item.Value.ToString() : "";
				} else {
					values[i] = Tokens[i];
				}
			}
			return String.Join(ValueSeparator.ToString(), values);
		}
		protected internal ListEditItem FindItemByText(string text) {
			foreach(ListEditItem listEditItem in Items) {
				string listEditItemPlainText = listEditItem.Text.Replace("\r\n", " ").Replace("\r", "\n");
				if(listEditItemPlainText == text)
					return listEditItem;
			}
			return null;
		}
		private void RaiseCustomTokenAdded(IEnumerable<string> customTokens) {
			OnCustomTokenAdded(new TokenBoxCustomTokensAddedEventArgs(customTokens));
		}
		protected internal string JoinText() {
			return String.Join(TextSeparator.ToString(), Tokens.ToArray());
		}
		protected void ParseValue(string value) {
			string[] tokens = Properties.ParseValue(value);
			SynchronizeTokens(tokens, false, false);
		}
		protected void ParseText(string text) {
			string[] tokens = text.Split(new string[] { TextSeparator.ToString() }, StringSplitOptions.None).Distinct().ToArray();
			SynchronizeTokens(tokens, false, false);
		}
		[SecuritySafeCritical]
		protected string[] DeserializeTokens(string stateString) {
			if(!String.IsNullOrEmpty(stateString)) {
				JavaScriptSerializer serializer = new JavaScriptSerializer();
				return serializer.Deserialize<string[]>(stateString).Distinct().ToArray();
			} else
				return null;
		}
		[SecuritySafeCritical]
		protected internal string SerializeTokens() {
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			string[] tokens = new string[Tokens.Count];
			for(int ind = 0; ind < Tokens.Count; ind++) {
				string token = Tokens[ind];
				if(!String.IsNullOrEmpty(token)){
					tokens[ind] = token;
				}
			}
			return serializer.Serialize(tokens);
		}
		[SecuritySafeCritical]
		protected internal string SerializeTokensValues() {
			JavaScriptSerializer serializer = new JavaScriptSerializer();
			string[] tokensValues = new string[Tokens.Count];
			for(int ind = 0; ind < Tokens.Count; ind++) {
				string token = Tokens[ind];
				if(!String.IsNullOrEmpty(token)) {
					ListEditItem item = FindItemByText(token);
					tokensValues[ind] = item != null ? (item.Value != null ? item.Value.ToString() : "") : token;
				}
			}
			return serializer.Serialize(tokensValues);
		}
		protected void RaiseTokensChanged() {
			OnTokensChanged(EventArgs.Empty);
			TokensChangedInternal();
		}
		protected void OnTokensChanged(EventArgs e) {
			EventHandler handler = Events[Properties.EventTokensChanged] as EventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected void OnCustomTokenAdded(TokenBoxCustomTokensAddedEventArgs e) {
			TokenBoxCustomTokensAddedEventHandler handler = Events[Properties.EventCustomTokensAdded] as TokenBoxCustomTokensAddedEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected bool NeedSyncTokensWithItems {
			get { return needSyncTokensWithItems; }
			set { needSyncTokensWithItems = value; }
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientTokenBox";
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxComboBox), ASPxComboBox.ComboBoxScriptResourceName);
			RegisterIncludeScript(typeof(ASPxTokenBox), ASPxTokenBox.TokenBoxScriptResourceName);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(ValueSeparator != ',')
				stb.AppendFormat("{0}.valueSeparator='{1}';\n", localVarName, ValueSeparator.ToString());
			if(TextSeparator != ',')
				stb.AppendFormat("{0}.textSeparator='{1}';\n", localVarName, TextSeparator.ToString());
			if(ShowDropDownOnFocus != ShowDropDownOnFocusMode.Always)
				stb.AppendFormat("{0}.showDropDownOnFocus='{1}';\n", localVarName, ShowDropDownOnFocus.ToString());
			if(!CaseSensitiveTokens)
				stb.AppendFormat("{0}.caseSensitiveTokens=false;\n", localVarName);
			stb.Append(localVarName + ".allowMouseWheel = false;\n");
			GetStateStyleScript(stb, localVarName, GetTokenBoxTokenHoveredStyle(), "hoverTokenClasses", "hoverTokenCssArray");
			GetStateStyleScript(stb, localVarName, GetTokenBoxTokenRemoveButtonHoverStyle(), "hoverTokenRemoveButtonClasses", "hoverTokenRemoveButtonCssArray");
		}
		protected void GetStateStyleScript(StringBuilder stb, string localVarName,
			AppearanceStyleBase style, string classField, string cssField) {
			if(!string.IsNullOrEmpty(style.CssClass))
				stb.AppendLine(localVarName + "." + classField + "=[" + HtmlConvertor.ToScript(style.CssClass) + "];");
			string cssText = style.GetStyleAttributes(Page).Value;
			if(!string.IsNullOrEmpty(cssText))
				stb.AppendLine(localVarName + "." + cssField + "=[" + HtmlConvertor.ToScript(cssText) + "];");
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new TokenBoxControl(this);
		}
		protected new TokenBoxControl DropDownControl {
			get { return base.DropDownControl as TokenBoxControl; }
		}
		protected internal override List<EditButton> GetButtons() {
			return new List<EditButton>();
		}
		protected internal string GetTokenRBClick() {
			return String.Format(ASPxTokenBox.TokenRBClickHandlerName, ClientID);
		}
		protected internal string GetMainElementClick() {
			return String.Format(ASPxTokenBox.MainElementMouseDownHandlerName, ClientID);
		}
		#region HIDDEN_PROPERTIES
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new ButtonImageProperties ButtonEditEllipsisImage {
			get { return Properties.ButtonEditEllipsisImage; }
		}
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new EditButtonCollection Buttons {
			get { return Properties.Buttons; }
		}
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new ITemplate ButtonTemplate {
			get { return Properties.ButtonTemplate; }
			set { Properties.ButtonTemplate = value; }
		}
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new EditButtonStyle ButtonStyle {
			get { return Properties.ButtonStyle; }
		}
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new string DisplayFormatString {
			get { return base.DisplayFormatString; }
			set { base.DisplayFormatString = value; }
		}
		[DefaultValue(null), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new DropDownButton DropDownButton {
			get { return Properties.DropDownButton; }
		}
		[DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new bool AllowMouseWheel {
			get { return Properties.AllowMouseWheel; }
			set { Properties.AllowMouseWheel = value; }
		}
		[DefaultValue(""), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable] 
		public new string TextFormatString {
			get { return base.TextFormatString; }
			set { base.TextFormatString = value; }
		}
		#endregion HIDDEN_PROPERTIES
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.TokenBoxItemsOwner"; } }
	}
	public class TokenCollection : CustomCollection<string> {
		private TokenBoxProperties properties;
		public TokenCollection()
			: base() {
			this.properties = null;
		}
		public TokenCollection(string[] items)
			: base() {
			this.properties = null;
			if(Items != null)
				Items.AddRange(items);
		}
		public TokenCollection(TokenBoxProperties properties)
			: base() {
			this.properties = properties;
		}
		protected internal TokenBoxProperties Properties {
			get { return properties; }
		}
		protected override void Changed() {
			if(Properties != null && Properties.TokenBox != null) {
				Properties.TokenBox.TokensChangedInternal();
				if(!Properties.CaseSensitiveTokens && Items.Count > 1)
					SetItemsSilent(Items.Distinct(StringComparer.InvariantCultureIgnoreCase).ToList());
			}
		}
		protected internal void ChangedInternal() {
			this.Changed();
		}
		protected override bool IsNullOrEmpty(string value) {
			return String.IsNullOrEmpty(value);
		}
	}
	public class ExtendedStyleBase : AppearanceStyleBase {
		[
#if !SL
	DevExpressWebLocalizedDescription("ExtendedStyleBaseHeight"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatEnable]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ExtendedStyleBaseWidth"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatEnable]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		public override void AssignToControl(WebControl control, AttributesRange range, bool exceptTextDecoration, bool useBlockAlignment, bool exceptOpacity) {
			base.AssignToControl(control, range, exceptTextDecoration, useBlockAlignment, exceptOpacity);
			if(!Height.IsEmpty && control.ControlStyle.Height != Height)
				control.ControlStyle.Height = Height;
			if(!Width.IsEmpty && control.ControlStyle.Width != Width)
				control.ControlStyle.Width = Width;
		}
	}
	public class TokenCollectionConverter : StringListConverter {
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			return ((sourceType == typeof(string)) || base.CanConvertFrom(context, sourceType));
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return (destinationType == typeof(string) || destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object sourceObj) {
			if(sourceObj is string) {
				char tokensSeparator = ',';
				return TrimTokens(((string)sourceObj).Split(new char[] { tokensSeparator }));
			}
			return base.ConvertFrom(context, culture, sourceObj);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object destinationObj, Type destinationType) {
			if(destinationObj is TokenCollection) {
				TokenCollection tokens = (TokenCollection)destinationObj;
				if(destinationType == typeof(InstanceDescriptor))
					return new InstanceDescriptor(typeof(TokenCollection).GetConstructor(new Type[] { typeof(string[]) }), new object[] { tokens.ToArray() });
				else if(destinationType == typeof(string)) {
					char tokensSeparator = ',';
					return string.Join(String.Format("{0} ", tokensSeparator), tokens);
				}
			}
			return base.ConvertTo(context, culture, destinationObj, destinationType);
		}
		private TokenCollection TrimTokens(string[] strArray) {
			TokenCollection ret = new TokenCollection();
			for(int i = 0; i < strArray.Length; i++)
				ret.Add(strArray[i].Trim());
			return ret;
		}
	}
}
