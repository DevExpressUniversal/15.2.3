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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data.Access;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web;
using DevExpress.Web.Design;
namespace DevExpress.Web {
	public enum RequiredMarkMode { Auto, None, All, RequiredOnly, OptionalOnly }
	public enum FieldRequiredMarkMode { Auto, Required, Optional, Hidden }
	public class FormLayoutProperties : PropertiesBase, ISkinOwner {
		protected internal const string DefaultRequiredMark = "*";
		protected internal const string DefaultOptionalMark = "(optional)";
		ClientSideEvents clientSideEvents;
		FormLayoutStyles styles;
		StylesBase parentStyles;
		FormLayoutStyles renderStyles;
		ISkinOwner parentSkinOwner;
		LayoutGroup rootLayoutGroup = null;
		Dictionary<LayoutItem, List<Control>> designTimeNestedControlsStorage = null;
		FormLayoutAdaptivitySettings settingsAdaptivity = null;
		public FormLayoutProperties(IPropertiesOwner owner)
			: base(owner) {
			this.clientSideEvents = new ClientSideEvents();
			this.styles = new FormLayoutStyles(this);
			IsStandalone = true;
		}
		public FormLayoutProperties()
			: this(null) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesSettingsAdaptivity"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false),
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public FormLayoutAdaptivitySettings SettingsAdaptivity {
			get {
				if(this.settingsAdaptivity == null)
					this.settingsAdaptivity = new FormLayoutAdaptivitySettings(this);
				return this.settingsAdaptivity;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesAlignItemCaptions"),
#endif
 Category("Behavior"),
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public bool AlignItemCaptions {
			get { return Root.AlignItemCaptions; }
			set { Root.AlignItemCaptions = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesAlignItemCaptionsInAllGroups"),
#endif
 Category("Behavior"),
		DefaultValue(false), NotifyParentProperty(true), AutoFormatEnable]
		public bool AlignItemCaptionsInAllGroups {
			get { return GetBoolProperty("AlignItemCaptionsInAllGroups", false); }
			set { SetBoolProperty("AlignItemCaptionsInAllGroups", false, value); }
		}
		protected internal string ClientInstanceName {
			get { return GetStringProperty("ClientInstanceName", ""); }
			set { SetStringProperty("ClientInstanceName", "", value); }
		}
		protected internal ClientSideEvents ClientSideEvents {
			get { return this.clientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesEncodeHtml"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public virtual bool EncodeHtml {
			get { return GetBoolProperty("EncodeHtml", true); }
			set { SetBoolProperty("EncodeHtml", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesLeftAndRightCaptionsWidth"),
#endif
 Category("Behavior"),
		DefaultValue(0), NotifyParentProperty(true), AutoFormatEnable]
		public int LeftAndRightCaptionsWidth {
			get { return GetIntProperty("LeftAndRightCaptionsWidth", 0); }
			set { SetIntProperty("LeftAndRightCaptionsWidth", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesRequiredMark"),
#endif
 NotifyParentProperty(true),
		Category("Appearance"), DefaultValue(DefaultRequiredMark), Localizable(true), AutoFormatEnable]
		public string RequiredMark {
			get { return GetStringProperty("RequiredMark", DefaultRequiredMark); }
			set { SetStringProperty("RequiredMark", DefaultRequiredMark, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesOptionalMark"),
#endif
 NotifyParentProperty(true),
		Category("Appearance"), DefaultValue(DefaultOptionalMark), Localizable(true), AutoFormatEnable]
		public string OptionalMark {
			get { return GetStringProperty("OptionalMark", DefaultOptionalMark); }
			set { SetStringProperty("OptionalMark", DefaultOptionalMark, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesRequiredMarkDisplayMode"),
#endif
 Category("Appearance"),
		DefaultValue(RequiredMarkMode.RequiredOnly), NotifyParentProperty(true), AutoFormatEnable]
		public RequiredMarkMode RequiredMarkDisplayMode {
			get { return (RequiredMarkMode)GetEnumProperty("RequiredMarkDisplayMode", RequiredMarkMode.RequiredOnly); }
			set {
				SetEnumProperty("RequiredMarkDisplayMode", RequiredMarkMode.RequiredOnly, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesNestedControlWidth"),
#endif
 Category("Layout"),
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public Unit NestedControlWidth {
			get { return (Unit)GetObjectProperty("NestedControlWidth", Unit.Empty); }
			set { SetObjectProperty("NestedControlWidth", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesItems"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false),
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public LayoutItemCollection Items {
			get { return Root.Items; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesColCount"),
#endif
		DefaultValue(1), NotifyParentProperty(true), AutoFormatEnable, Category("Layout")]
		public int ColCount {
			get { return Root.ColCount; }
			set { Root.ColCount = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesSettingsItemCaptions"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutItemCaptionSettings SettingsItemCaptions {
			get { return Root.SettingsItemCaptions; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesSettingsItemHelpTexts"),
#endif
		RefreshProperties(RefreshProperties.All), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Layout"),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public LayoutItemHelpTextSettings SettingsItemHelpTexts {
			get { return Root.SettingsItemHelpTexts; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesSettingsItems"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutGroupItemSettings SettingsItems {
			get { return Root.SettingsItems; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesShowItemCaptionColon"),
#endif
 Category("Appearance"),
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public bool ShowItemCaptionColon {
			get { return GetBoolProperty("ShowItemCaptionColon", true); }
			set { SetBoolProperty("ShowItemCaptionColon", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesUseDefaultPaddings"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable, Category("Layout")]
		public bool UseDefaultPaddings {
			get { return Root.UseDefaultPaddings; }
			set { Root.UseDefaultPaddings = value; }
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			FormLayoutProperties flProperties = source as FormLayoutProperties;
			if(flProperties != null) {
				AlignItemCaptionsInAllGroups = flProperties.AlignItemCaptionsInAllGroups;
				ClientInstanceName = flProperties.ClientInstanceName;
				EncodeHtml = flProperties.EncodeHtml;
				LeftAndRightCaptionsWidth = flProperties.LeftAndRightCaptionsWidth;
				RequiredMark = flProperties.RequiredMark;
				OptionalMark = flProperties.OptionalMark;
				RequiredMarkDisplayMode = flProperties.RequiredMarkDisplayMode;
				ShowItemCaptionColon = flProperties.ShowItemCaptionColon;
				NestedControlWidth = flProperties.NestedControlWidth;
				ClientSideEvents.Assign(flProperties.ClientSideEvents);
				Root.Assign(flProperties.Root);
				SettingsAdaptivity.Assign(flProperties.SettingsAdaptivity);
				Styles.Assign(flProperties.Styles);
			}
		}
		public void ForEach(Action<LayoutItemBase> method) {
			Root.ForEach(method);
		}
		protected internal ASPxFormLayout FormLayout {
			get { return Owner as ASPxFormLayout; }
		}
		protected internal LayoutGroup Root {
			get {
				if(this.rootLayoutGroup == null)
					this.rootLayoutGroup = CreateRootGroup();
				return this.rootLayoutGroup;
			}
		}
		protected virtual LayoutGroup CreateRootGroup() {
			return new LayoutGroup(this);
		}
		protected internal bool IsFlowRender {
			get { return SettingsAdaptivity.AdaptivityMode != FormLayoutAdaptivityMode.Off; }
		}
		protected internal Dictionary<LayoutItem, List<Control>> DesignTimeNestedControlsStorage {
			get { return designTimeNestedControlsStorage; }
			set { designTimeNestedControlsStorage = value; }
		}
		protected internal bool DesignTimeEditingMode {
			get { return designTimeNestedControlsStorage != null; }
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ClientSideEvents, Root, SettingsAdaptivity, Styles });
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesStyles"),
#endif
 NotifyParentProperty(true),
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FormLayoutStyles Styles {
			get { return styles; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public StylesBase ParentStyles {
			get { return parentStyles; }
			set { parentStyles = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ISkinOwner ParentSkinOwner {
			get { return parentSkinOwner; }
			set { parentSkinOwner = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FormLayoutPropertiesPaddings"),
#endif
 NotifyParentProperty(true),
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return RenderStyles.Style.Paddings; }
		}
		protected internal FormLayoutStyles RenderStyles {
			get {
				if(ParentStyles == null) return Styles;
				if(this.renderStyles == null || !UseCachedObjects()) {
					this.renderStyles = new FormLayoutStyles(this);
					this.renderStyles.CopyFrom(ParentStyles);
					this.renderStyles.CopyFrom(Styles);
				}
				return this.renderStyles;
			}
		}
		protected bool UseCachedObjects() {
			return IsRendering() && !IsDesignMode();
		}
		protected bool IsRendering() {
			if(Owner is IWebControlObject)
				return (Owner as IWebControlObject).IsRendering();
			return false;
		}
		protected virtual bool IsDesignMode() {
			if(Owner is IWebControlObject)
				return (Owner as IWebControlObject).IsDesignMode();
			return false;
		}
		protected internal bool IsStandalone { get; set; }
		#region ISkinOwner Members
		string ISkinOwner.GetControlName() {
			return "Web";
		}
		string ISkinOwner.GetCssFilePath() {
			if(!string.IsNullOrEmpty(Styles.CssFilePath))
				return Styles.CssFilePath;
			else if(ParentStyles != null && !string.IsNullOrEmpty(ParentStyles.CssFilePath))
				return ParentStyles.CssFilePath;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetCssFilePath() : string.Empty;
			}
		}		
		string ISkinOwner.GetImageFolder() {
			return "";
		}
		string ISkinOwner.GetSpriteImageUrl() {
			return "";
		}
		string ISkinOwner.GetSpriteCssFilePath() {
			return "";
		}
		string ISkinOwner.GetCssPostFix() {
			if(!string.IsNullOrEmpty(Styles.CssPostfix))
				return Styles.CssPostfix;
			else if(ParentStyles != null && !string.IsNullOrEmpty(ParentStyles.CssPostfix))
				return ParentStyles.CssPostfix;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetCssPostFix() : string.Empty;
			}
		}
		string ISkinOwner.GetTheme() {
			if(!string.IsNullOrEmpty(Styles.Theme))
				return Styles.Theme;
			else if(ParentStyles != null && !string.IsNullOrEmpty(ParentStyles.Theme))
				return ParentStyles.Theme;
			else {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				return (parentSkinOwner != null) ? parentSkinOwner.GetTheme() : string.Empty;
			}
		}
		void ISkinOwner.MergeControlStyle(AppearanceStyleBase style) {
		}
		bool ISkinOwner.IsDefaultAppearanceEnabled() {
			return true;
		}
		bool ISkinOwner.IsAccessibilityCompliant() {
			bool ret = Styles.AccessibilityCompliant;
			if(ParentStyles != null)
				ret = ret || ParentStyles.AccessibilityCompliant;
			ISkinOwner parentSkinOwner = GetParentSkinOwner();
			if(parentSkinOwner != null)
				ret = ret || parentSkinOwner.IsAccessibilityCompliant();
			return ret;
		}
		bool ISkinOwner.IsNative() {
			return false;
		}
		bool ISkinOwner.IsNativeSupported() {
			return false;
		}
		bool ISkinOwner.IsRightToLeft() {
			DefaultBoolean value = Styles.RightToLeft;
			if(value == DefaultBoolean.Default && ParentStyles != null)
				value = ParentStyles.RightToLeft;
			if(value == DefaultBoolean.Default) {
				ISkinOwner parentSkinOwner = GetParentSkinOwner();
				if(parentSkinOwner != null)
					return parentSkinOwner.IsRightToLeft();
			}
			if(value == DefaultBoolean.True)
				return true;
			if(value == DefaultBoolean.False)
				return false;
			return ASPxWebControl.GlobalRightToLeft == DefaultBoolean.True;
		}
		string[] ISkinOwner.GetChildControlNames() {
			return new string[] { };
		}
		#endregion
		ISkinOwner GetParentSkinOwner() {
			if(ParentSkinOwner != null)
				return ParentSkinOwner;
			ISkinOwner skinOwner = RenderUtils.FindParentSkinOwner(Owner as Control);
			if(UseCachedObjects())
				ParentSkinOwner = skinOwner;
			return skinOwner;
		}
		void IPropertiesOwner.Changed(PropertiesBase properties) {
			Changed();
		}
		protected internal IFormLayoutOwner DataOwner { get; set; }
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	Designer("DevExpress.Web.Design.ASPxFormLayoutDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxFormLayout.bmp"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation)]
	public class ASPxFormLayout : ASPxDataWebControl, IControlDesigner {
		protected internal const string ScriptResourceName = WebScriptsResourcePath + "FormLayout.js";
		private static readonly object EventLayoutItemDataBinding = new object();
		private static readonly object EventLayoutItemDataBound = new object();
		int requiredFieldCount = 0;
		int optionalFieldCount = 0;
		int vacantItemEditorIndex = 1;
		FormLayoutProperties properties = null;
		public ASPxFormLayout()
			: base() {
		}
		protected ASPxFormLayout(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		protected internal LayoutGroup Root {
			get { return Properties.Root; }
		}
		protected internal Dictionary<LayoutItem, List<Control>> DesignTimeNestedControlsStorage {
			get { return Properties.DesignTimeNestedControlsStorage; }
			set { Properties.DesignTimeNestedControlsStorage = value; }
		}
		protected internal bool DesignTimeEditingMode {
			get { return Properties.DesignTimeEditingMode; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutSettingsAdaptivity"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false),
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public FormLayoutAdaptivitySettings SettingsAdaptivity {
			get { return Properties.SettingsAdaptivity; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutAlignItemCaptions"),
#endif
 Category("Behavior"),
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public bool AlignItemCaptions {
			get { return Properties.AlignItemCaptions; }
			set { Properties.AlignItemCaptions = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutAlignItemCaptionsInAllGroups"),
#endif
 Category("Behavior"),
		DefaultValue(false), NotifyParentProperty(true), AutoFormatEnable]
		public bool AlignItemCaptionsInAllGroups {
			get { return Properties.AlignItemCaptionsInAllGroups; }
			set { Properties.AlignItemCaptionsInAllGroups = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutLeftAndRightCaptionsWidth"),
#endif
 Category("Behavior"),
		DefaultValue(0), NotifyParentProperty(true), AutoFormatEnable]
		public int LeftAndRightCaptionsWidth {
			get { return Properties.LeftAndRightCaptionsWidth; }
			set { Properties.LeftAndRightCaptionsWidth = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutEncodeHtml"),
#endif
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public override bool EncodeHtml {
			get { return Properties.EncodeHtml; }
			set { Properties.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutItems"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), MergableProperty(false), 
		NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public LayoutItemCollection Items {
			get { return Properties.Items; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutColCount"),
#endif
		DefaultValue(1), NotifyParentProperty(true), AutoFormatEnable, Category("Layout")]
		public int ColCount {
			get { return Properties.ColCount; }
			set { Properties.ColCount = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutSettingsItemCaptions"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutItemCaptionSettings SettingsItemCaptions {
			get { return Properties.SettingsItemCaptions; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutSettingsItemHelpTexts"),
#endif
		RefreshProperties(RefreshProperties.All), NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),  Category("Layout"),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public LayoutItemHelpTextSettings SettingsItemHelpTexts {
			get { return Properties.SettingsItemHelpTexts; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutSettingsItems"),
#endif
		Category("Layout"), NotifyParentProperty(true), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LayoutGroupItemSettings SettingsItems {
			get { return Properties.SettingsItems; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutShowItemCaptionColon"),
#endif
 Category("Appearance"),
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public bool ShowItemCaptionColon {
			get { return Properties.ShowItemCaptionColon; }
			set { Properties.ShowItemCaptionColon = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutRequiredMarkDisplayMode"),
#endif
 Category("Appearance"),
		DefaultValue(RequiredMarkMode.RequiredOnly), NotifyParentProperty(true), AutoFormatEnable]
		public RequiredMarkMode RequiredMarkDisplayMode {
			get { return Properties.RequiredMarkDisplayMode; }
			set { Properties.RequiredMarkDisplayMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutRequiredMark"),
#endif
		Category("Appearance"), DefaultValue(FormLayoutProperties.DefaultRequiredMark), Localizable(true), AutoFormatEnable]
		public string RequiredMark {
			get { return Properties.RequiredMark; }
			set { Properties.RequiredMark = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutOptionalMark"),
#endif
		Category("Appearance"), DefaultValue(FormLayoutProperties.DefaultOptionalMark), Localizable(true), AutoFormatEnable]
		public string OptionalMark {
			get { return Properties.OptionalMark; }
			set { Properties.OptionalMark = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutNestedControlWidth"),
#endif
 Category("Layout"),
		DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public Unit NestedControlWidth {
			get { return Properties.NestedControlWidth; }
			set { Properties.NestedControlWidth = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutUseDefaultPaddings"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable, Category("Layout")]
		public bool UseDefaultPaddings {
			get { return Properties.UseDefaultPaddings; }
			set { Properties.UseDefaultPaddings = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutDataItemPosition"),
#endif
 DefaultValue(0),
		NotifyParentProperty(true), AutoFormatEnable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int DataItemPosition {
			get { return GetIntProperty("DataItemPosition", 0); }
			set {
				CommonUtils.CheckNegativeValue((double)value, "DataItemPosition");
				SetIntProperty("DataItemPosition", 0, value);
				if(CanFindDataSource())
					PerformSelect();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName {
			get { return ClientInstanceNameInternal; }
			set { ClientInstanceNameInternal = value; }
		}
		protected override string ClientInstanceNameInternal {
			get { return Properties.ClientInstanceName; }
			set { Properties.ClientInstanceName = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable]
		public ClientSideEvents ClientSideEvents {
			get { return (ClientSideEvents)ClientSideEventsInternal; }
		}
		protected internal override ClientSideEventsBase ClientSideEventsInternal {
			get { return Properties.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutPaddings"),
#endif
		Category("Layout"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return Properties.Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FormLayoutStyles Styles {
			get { return (FormLayoutStyles)StylesInternal; }
		}
		[Browsable(false), UrlProperty]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DisabledStyle DisabledStyle { 
			get { return base.DisabledStyle; } 
		}
		public void ForEach(Action<LayoutItemBase> method) {
			Properties.ForEach(method);
		}
		public LayoutItemBase FindItemOrGroupByName(string name) {
			return Root.FindItemOrGroupByName(name);
		}
		public LayoutItem FindItemByFieldName(string fieldName) {
			return Root.FindItemByFieldName(fieldName);
		}
		public Control FindNestedControlByFieldName(string fieldName) {
			return Root.FindNestedControlByFieldName(fieldName);
		}
		public object GetNestedControlValueByFieldName(string fieldName) {
			return Root.GetNestedControlValueByFieldName(fieldName);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LayoutItemBase FindItemOrGroupByPath(string path) {
			return Root.FindItemOrGroupByPath(path);
		}
		[Obsolete("This method is now obsolete. Use the FindItemOrGroupByName method instead."), EditorBrowsable(EditorBrowsableState.Never)]
		public LayoutItemBase FindItemByPath(string path) {
			return FindItemOrGroupByPath(path);
		}
		public override bool IsClientSideAPIEnabled() {
			return HasClientInvisibleItems(Root) || base.IsClientSideAPIEnabled();
		}
		protected internal new bool EnableClientSideAPIInternal { get { return base.EnableClientSideAPIInternal; } set { base.EnableClientSideAPIInternal = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutLayoutItemDataBinding"),
#endif
		Category("Data")]
		public event LayoutItemDataBindingEventHandler LayoutItemDataBinding
		{
			add { Events.AddHandler(EventLayoutItemDataBinding, value); }
			remove { Events.RemoveHandler(EventLayoutItemDataBinding, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutLayoutItemDataBound"),
#endif
		Category("Data")]
		public event LayoutItemDataBoundEventHandler LayoutItemDataBound
		{
			add { Events.AddHandler(EventLayoutItemDataBound, value); }
			remove { Events.RemoveHandler(EventLayoutItemDataBound, value); }
		}
		protected internal Type DataType { get; set; }
		protected internal FormLayoutStyles RenderStyles {
			get { return Properties.RenderStyles; }
		}
		protected override StylesBase StylesInternal {
			get { return Properties.Styles; }
		}
		protected override StylesBase RenderStylesInternal {
			get { return Properties.RenderStyles; }
		}
		protected override Style CreateControlStyle() {
			return new FormLayoutStyle();
		}
		protected override StylesBase CreateStyles() {
			return new FormLayoutStyles(this);
		}
		protected internal FormLayoutStyle GetFormLayoutStyle() {
			FormLayoutStyle style = new FormLayoutStyle();
			style.CopyFrom(RenderStyles.GetDefaultFormLayoutStyle());
			string savedCssClass = style.CssClass;
			style.CopyFrom(RenderStyles.Style);
			style.CopyFrom(ControlStyle);
			style.CssClass = savedCssClass;
			return style;
		}
		protected internal LayoutGroupBoxStyle GetLayoutGroupBoxStyle() {
			LayoutGroupBoxStyle style = new LayoutGroupBoxStyle();
			style.CopyFrom(RenderStyles.GetDefaultLayoutGroupBoxStyle());
			style.CopyFrom(RenderStyles.LayoutGroupBox);
			return style;
		}
		protected internal LayoutItemStyle GetLayoutItemStyle() {
			LayoutItemStyle style = new LayoutItemStyle();
			style.CopyFrom(RenderStyles.GetDefaultLayoutItemStyle());
			style.CopyFrom(RenderStyles.LayoutItem);
			return style;
		}
		protected internal LayoutGroupStyle GetLayoutGroupStyle() {
			LayoutGroupStyle style = new LayoutGroupStyle();
			style.CopyFrom(RenderStyles.GetDefaultLayoutGroupStyle());
			style.CopyFrom(RenderStyles.LayoutGroup);
			style.CssClass = RenderUtils.CombineCssClasses(style.CssClass, FormLayoutStyles.LayoutGroupSystemClassName);
			return style;
		}
		protected internal EmptyLayoutItemStyle GetEmptyLayoutItemStyle() {
			EmptyLayoutItemStyle style = new EmptyLayoutItemStyle();
			style.CopyFrom(RenderStyles.GetDefaultEmptyLayoutItemStyle());
			style.CopyFrom(RenderStyles.EmptyLayoutItem);
			return style;
		}
		protected internal AppearanceStyle GetLayoutItemRequiredMarkStyle() {
			AppearanceStyle result = new AppearanceStyle();
			result.CopyFrom(RenderStyles.GetDefaultLayoutItemRequiredMarkStyle());
			return result;
		}
		protected internal AppearanceStyle GetLayoutItemOptionalMarkStyle() {
			AppearanceStyle result = new AppearanceStyle();
			result.CopyFrom(RenderStyles.GetDefaultLayoutItemOptionalMarkStyle());
			return result;
		}
		protected override DataHelperBase CreateDataHelper(string name) {
			return new FormLayoutDataHelper(this, name);
		}
		protected internal FormLayoutProperties Properties {
			get {
				if(this.properties == null)
					this.properties = new FormLayoutProperties(this);
				return this.properties;
			}
		}
		protected override void CreateControlHierarchy() {
			CalculateRequiredAndOptionalFieldCounts();
			base.CreateControlHierarchy();
			Controls.Add(CreateFormLayoutControl());
			FormLayoutRenderHelper.EnsureChildControlsRecursive(this);
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			EnsureLayoutItemNestedControls();
		}
		protected override bool NeedCreateHierarchyOnInit() {
			return true;
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			object dataItem = GetDataItem(data);
			if (dataItem != null) {
				if (Items.IsEmpty)
					CreatePlainLayoutByData(dataItem);
				ForEach(delegate(LayoutItemBase item) {
					if (item is LayoutItem)
						BindLayoutItem((LayoutItem)item, dataItem);
				});
			}
		}
		protected void CreatePlainLayoutByData(object dataItem) {
			foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dataItem))
				if(NestedControlHelper.IsAllowedDataType(propertyDescriptor.PropertyType))
					AddNewLayoutItem(propertyDescriptor);
		}
		protected void EnsureLayoutItemNestedControls() {
			ForEach(delegate(LayoutItemBase item) {
				if (item is LayoutItem)
					((LayoutItem)item).EnsureNestedControl();
			});
		}
		protected internal void AddNewLayoutItem(PropertyDescriptor propertyDescriptor) {
			LayoutItem layoutItem = new LayoutItem();
			Items.Add(layoutItem);
			layoutItem.FieldName = propertyDescriptor.Name;
			layoutItem.DataType = propertyDescriptor.PropertyType;
			layoutItem.EnsureNestedControl();
		}
		private int GetDataItemPosition(int dataItemsCount) {
			return DataItemPosition < dataItemsCount ? DataItemPosition : dataItemsCount - 1;
		}
		private object GetDataItem(IEnumerable data) {
			if (data != null) {
				int dataItemsCount = data is IList ? ((IList)data).Count : data.Cast<object>().Count();
				if (dataItemsCount > 0) {
					int dataItemPosition = GetDataItemPosition(dataItemsCount);
					return data is IList ? ((IList)data)[dataItemPosition] 
						: data.Cast<object>().ElementAt(dataItemPosition);
				}
			}
			return null;
		}
		private PropertyDescriptor GetPropertyDescriptorByLayoutItemFieldName(LayoutItem layoutItem, object dataItem) {
			if(layoutItem.FieldName.Contains('.')) {
				List<string> fieldNames = new List<string>(layoutItem.FieldName.Split('.'));
				PropertyDescriptor rootPropertyDescriptor = TypeDescriptor.GetProperties(dataItem)[fieldNames[0]];
				return rootPropertyDescriptor != null ? new ComplexPropertyDescriptorReflection(dataItem, layoutItem.FieldName) : null;
			}
			return TypeDescriptor.GetProperties(dataItem)[layoutItem.FieldName];
		}
		protected virtual bool LayoutItemHasValidFieldName(LayoutItem layoutItem, object dataItem) {
			return GetPropertyDescriptorByLayoutItemFieldName(layoutItem, dataItem) != null;
		}
		protected virtual void EnsureLayoutItemNestedControl(LayoutItem layoutItem, object dataItem) {
			if (layoutItem.DataType == null) {
				PropertyDescriptor propertyDescriptor = GetPropertyDescriptorByLayoutItemFieldName(layoutItem, dataItem);
				if (propertyDescriptor != null)
					layoutItem.DataType = propertyDescriptor.PropertyType;
			}
			layoutItem.EnsureNestedControl();
		}
		protected void BindLayoutItem(LayoutItem layoutItem, object dataItem) {
			EnsureLayoutItemNestedControl(layoutItem, dataItem);
			OnLayoutItemDataBinding(new LayoutItemDataBindingEventArgs(layoutItem));
			Control nestedControl = layoutItem.GetNestedControl();
			object controlValue = null;
			if (LayoutItemHasValidFieldName(layoutItem, dataItem)) {
				controlValue = GetControlValue(layoutItem, dataItem);
				if (controlValue != null && !NestedControlHelper.IsAllowedDataType(controlValue.GetType()))
					controlValue = controlValue.ToString();
				BindNestedControl(nestedControl, controlValue, layoutItem.DataType);
			}
			OnLayoutItemDataBound(new LayoutItemDataBoundEventArgs(layoutItem, controlValue));
		}
		protected virtual object GetControlValue(LayoutItem layoutItem, object dataItem) {
			return DataUtils.GetFieldValue(dataItem, layoutItem.FieldName, false, DesignMode);
		}
		private void BindNestedControl(Control nestedControl, object controlValue, Type valueType) {
			if (nestedControl is IValueTypeHolder)
				((IValueTypeHolder)nestedControl).ValueType = valueType;
			if (nestedControl is ASPxEditBase)
				((ASPxEditBase)nestedControl).Value = controlValue;
		}
		protected virtual void OnLayoutItemDataBinding(LayoutItemDataBindingEventArgs e) {
			LayoutItemDataBindingEventHandler handler = Events[EventLayoutItemDataBinding] as LayoutItemDataBindingEventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected virtual void OnLayoutItemDataBound(LayoutItemDataBoundEventArgs e) {
			LayoutItemDataBoundEventHandler handler = Events[EventLayoutItemDataBound] as LayoutItemDataBoundEventHandler;
			if (handler != null)
				handler(this, e);
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Properties });
		}
		protected virtual FormLayoutControl CreateFormLayoutControl() {
			return new FormLayoutControl(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientFormLayout";
		}
		protected override bool IsScriptEnabled() {
			return true;
		}
		protected internal bool IsFlowRender {
			get { return Properties.IsFlowRender; }
		}
		protected internal Unit GetNestedControlDefaultWidth() {
			if(!NestedControlWidth.IsEmpty)
				return NestedControlWidth;
			if(IsFlowRender)
				return Unit.Percentage(100);
			return Unit.Empty;
		}
		protected internal bool NestedControlHasPercentageWidth(WebControl nestedControl) {
			Unit actualNestedControlWidth = !nestedControl.Width.IsEmpty ? nestedControl.Width : GetNestedControlDefaultWidth();
			return actualNestedControlWidth.Type == UnitType.Percentage;
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(IsFlowRender) {
				stb.AppendFormat("{0}.adaptivityMode = {1};\n", localVarName, HtmlConvertor.ToScript(SettingsAdaptivity.AdaptivityMode));
				stb.AppendFormat("{0}.switchToSingleColumnAtWindowInnerWidth = {1};\n", localVarName, SettingsAdaptivity.SwitchToSingleColumnAtWindowInnerWidth.ToString());
			}
			if (AlignItemCaptionsInAllGroups && LeftAndRightCaptionsWidth == 0)
				stb.AppendFormat("{0}.alignItemCaptionsInAllGroups = true;\n", localVarName);
			if(LeftAndRightCaptionsWidth != 0)
				stb.AppendFormat("{0}.leftAndRightCaptionsWidth = {1};\n", localVarName, LeftAndRightCaptionsWidth.ToString());
			if (IsClientSideAPIEnabled())
				stb.Append(GetClientItemsScript(localVarName));
			if(!ShowItemCaptionColon)
				stb.AppendFormat("{0}.showItemCaptionColon = false;\n", localVarName);
		}
		protected string GetClientItemsScript(string localVarName) {
			string script = HtmlConvertor.ToJSON(GetClientItemsScriptObject(Root), true);
			return localVarName + ".CreateItems(" + script + ");\n";
		}
		protected object GetClientItemsScriptObject(LayoutGroupBase group) {
			object[] itemsArray = new object[group.Items.Count];
			for (int i = 0; i < group.Items.Count; i++)
				itemsArray[i] = CreateItemProperties(group.Items[i]);
			return itemsArray;
		}
		protected virtual object[] CreateItemProperties(LayoutItemBase item) {
			object[] itemProperties = new object[6];
			itemProperties[0] = item.Name;
			itemProperties[1] = item.Path;
			if(!item.Visible)
				itemProperties[2] = false;
			if(!item.ClientVisible)
				itemProperties[3] = false;
			if(item is LayoutGroupBase) {
				if(item is TabbedLayoutGroup)
					itemProperties[4] = true;
				itemProperties[5] = GetClientItemsScriptObject((LayoutGroupBase)item);
			}
			return itemProperties;
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxFormLayout), ScriptResourceName);
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected bool HasClientInvisibleItems(LayoutItemBase item) {
			if(!item.ClientVisible)
				return true;
			LayoutGroupBase group = item as LayoutGroupBase;
			if(group != null) {
				for(int i = 0; i < group.Items.GetVisibleItemCount(); i++) {
					LayoutItemBase childItem = group.Items.GetVisibleItem(i) as LayoutItemBase;
					if(HasClientInvisibleItems(childItem))
						return true;
				}
			}
			return false;
		}
		protected void ResetRequiredAndOptionalFieldCounts(){
			requiredFieldCount = 0;
			optionalFieldCount = 0;
		}
		protected internal void CalculateRequiredAndOptionalFieldCounts() {
			ResetRequiredAndOptionalFieldCounts();
			Root.ForEach(delegate(LayoutItemBase item) {
				LayoutItem layoutItem = item as LayoutItem;
				if(layoutItem != null) {
					if (layoutItem.IsRequired())
						requiredFieldCount ++;
					else
						optionalFieldCount ++;
				}
			});
		}
		protected internal int GetRequiredFieldCount() {
			CalculateRequiredAndOptionalFieldCounts();
			return requiredFieldCount;
		}
		protected internal bool ShowItemRequiredMark(LayoutItem LayoutItem) {
			bool marksAllowed = 
				RequiredMarkDisplayMode != RequiredMarkMode.None && 
				RequiredMarkDisplayMode != RequiredMarkMode.OptionalOnly  && 
				LayoutItem.RequiredMarkDisplayMode != FieldRequiredMarkMode.Hidden;
			bool shouldShowRequiredMarks = this.requiredFieldCount <= this.optionalFieldCount || 
				RequiredMarkDisplayMode == RequiredMarkMode.RequiredOnly || 
				RequiredMarkDisplayMode == RequiredMarkMode.All;
			return marksAllowed && shouldShowRequiredMarks && LayoutItem.IsRequired();
		}
		protected internal bool ShowItemOptionalMark(LayoutItem LayoutItem) {
			bool marksAllowed = 
				RequiredMarkDisplayMode != RequiredMarkMode.None && 
				RequiredMarkDisplayMode != RequiredMarkMode.RequiredOnly && 
				LayoutItem.RequiredMarkDisplayMode != FieldRequiredMarkMode.Hidden;
			bool shouldShowOptionalMarks = this.requiredFieldCount > this.optionalFieldCount || 
				RequiredMarkDisplayMode == RequiredMarkMode.OptionalOnly || 
				RequiredMarkDisplayMode == RequiredMarkMode.All;
			return marksAllowed && shouldShowOptionalMarks && !LayoutItem.IsRequired();
		}
		protected internal string GetItemCaptionWithMark(LayoutItem layoutItem) {
			string result = layoutItem.GetItemCaption();
			if (ShowItemRequiredMark(layoutItem))
				result += RequiredMark;
			if (ShowItemOptionalMark(layoutItem))
				result += OptionalMark;
			return result;
		}
		private Control FindControlInChildLayoutItemsById(string id, LayoutGroupBase group) {
			foreach (LayoutItemBase childItem in group.Items) {
				if (childItem is LayoutItem) {
					foreach (Control control in (childItem as LayoutItem).GetActualControlsCollection())
						if (control.ID == id)
							return control;
				}
				if (childItem is LayoutGroupBase) {
					Control result = FindControlInChildLayoutItemsById(id, childItem as LayoutGroupBase);
					if (result != null)
						return result;
				}
			}
			return null;
		}
		protected internal string GetVacantItemNestedControlID() {
			for (int i = this.vacantItemEditorIndex; ; i++) {
				string id = string.Format("{0}_E{1}", ID, i);
				if (FindControlInChildLayoutItemsById(id, Root) == null) {
					this.vacantItemEditorIndex = i + 1;
					return id;
				}
			}
		}
		private bool CanFindDataSource() {
			return !IsBoundUsingDataSourceID() || Page != null;
		}
		protected internal bool ContainsGroupsWithoutDefaultPaddings() {
			if(!UseDefaultPaddings)
				return true;
			return Root.FindItemOrGroupByCondition(delegate(LayoutItemBase item) {
				return item is LayoutGroup && !((LayoutGroup)item).UseDefaultPaddings;
			}) != null;
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.FormLayoutCommonDesigner"; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxFormLayoutParentSkinOwner"),
#endif
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ISkinOwner ParentSkinOwner {
			get { return Properties.ParentSkinOwner; }
			set { Properties.ParentSkinOwner = value; }
		}
		protected override string GetSkinControlName() {
			return ((ISkinOwner)Properties).GetControlName();
		}
		protected override string GetCssFilePath() {
			return ((ISkinOwner)Properties).GetCssFilePath();
		}
		protected override string GetImageFolder() {
			return ((ISkinOwner)Properties).GetImageFolder();
		}
		protected override string GetSpriteImageUrl() {
			return ((ISkinOwner)Properties).GetSpriteImageUrl();
		}
		protected override string GetSpriteCssFilePath() {
			return ((ISkinOwner)Properties).GetSpriteCssFilePath();
		}
		protected override string GetCssPostFix() {
			return ((ISkinOwner)Properties).GetCssPostFix();
		}
		protected override void MergeControlStyle(AppearanceStyleBase style) {
			base.MergeControlStyle(style);
			((ISkinOwner)Properties).MergeControlStyle(style);
		}
		protected override bool IsDefaultAppearanceEnabled() {
			return ((ISkinOwner)Properties).IsDefaultAppearanceEnabled();
		}
		protected override bool IsNative() {
			return ((ISkinOwner)Properties).IsNative();
		}
		protected override bool IsNativeSupported() {
			return ((ISkinOwner)Properties).IsNativeSupported();
		}
		protected override bool IsRightToLeft() {
			return ((ISkinOwner)Properties).IsRightToLeft();
		}
		protected override string[] GetChildControlNames() {
			return ((ISkinOwner)Properties).GetChildControlNames();
		}
	}
}
namespace DevExpress.Web.Internal {
	public class FormLayoutDataHelper : DataHelper {
		protected override void ValidateDataSource(object dataSource) {
		}
		public FormLayoutDataHelper(ASPxDataWebControl control, string name) : base(control, name) { }
		public override bool CanBindToSingleObject {
			get {
				return true;
			}
		}
	}
	public static class NestedControlHelper {
		static Dictionary<Type, Type> dataTypeToControlTypeMap = new Dictionary<Type, Type>();
		static NestedControlHelper() {
			dataTypeToControlTypeMap.Add(typeof(String), typeof(ASPxTextBox));
			dataTypeToControlTypeMap.Add(typeof(Char), typeof(ASPxTextBox));
			dataTypeToControlTypeMap.Add(typeof(Byte), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(SByte), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(Int16), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(UInt16), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(Int32), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(UInt32), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(Int64), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(UInt64), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(Single), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(Double), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(Decimal), typeof(ASPxSpinEdit));
			dataTypeToControlTypeMap.Add(typeof(Boolean), typeof(ASPxCheckBox));
			dataTypeToControlTypeMap.Add(typeof(DateTime), typeof(ASPxDateEdit));
			dataTypeToControlTypeMap.Add(typeof(Enum), typeof(ASPxComboBox));
			dataTypeToControlTypeMap.Add(typeof(byte[]), typeof(ASPxBinaryImage));
			dataTypeToControlTypeMap.Add(typeof(Color), typeof(ASPxColorEdit));
		}
		public static bool IsAllowedDataType(Type dataType) {
			if (FindControlType(dataType) != null)
				return true;
			if (IsValidValueType(dataType))
				return true;
			Type underlyingType = Nullable.GetUnderlyingType(dataType);
			return underlyingType != null && IsValidValueType(underlyingType);
		}
		public static Type GetControlTypeByDataType(Type dataType) {
			Type controlType = FindControlType(dataType);
			return controlType != null ? controlType : GetDefaultControlType();
		}
		public static Control CreateControlByDataType(Type dataType) {
			Type controlType = GetControlTypeByDataType(dataType);
			Control control = Activator.CreateInstance(controlType) as Control;
			PrepareControl(control, dataType);
			return control;
		}
		public static void PrepareControl(Control control, Type dataType) {
			if (control is ASPxBinaryImage)
				PrepareBinaryImage((ASPxBinaryImage)control);
			if (control is ASPxComboBox && dataType != null && dataType.IsEnum)
				PrepareComboBox((ASPxComboBox)control, dataType);
		}
		private static void PrepareBinaryImage(ASPxBinaryImage binaryImage) {
			binaryImage.StoreContentBytesInViewState = true;
		}
		private static void PrepareComboBox(ASPxComboBox comboBox, Type dataType) {
			foreach (var enumValue in Enum.GetValues(dataType))
				comboBox.Items.Add(enumValue.ToString(), enumValue);
		}
		private static bool IsValidValueType(Type dataType) {
			return dataType.IsValueType && dataType.IsSerializable && dataType.GetProperties().Count() == 0;
		}
		private static Type FindControlType(Type dataType) {
			if(dataType == null)
				return null;
			foreach (Type possibleDataType in dataTypeToControlTypeMap.Keys)
				if (possibleDataType.IsAssignableFrom(dataType) || possibleDataType.IsAssignableFrom(Nullable.GetUnderlyingType(dataType)))
					return dataTypeToControlTypeMap[possibleDataType];
			return null;
		}
		private static Type GetDefaultControlType() {
			return typeof(ASPxTextBox);
		}
	}
}
