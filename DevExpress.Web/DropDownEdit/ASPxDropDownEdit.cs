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
using System.ComponentModel;
using System.Text;
using DevExpress.Web;
using DevExpress.Web.Internal;
using System.Web.UI;
using System.Drawing.Design;
namespace DevExpress.Web {
	using DevExpress.Web.Internal;
	using System.Web.UI.WebControls;
	using DevExpress.Utils;
	using DevExpress.Web.Data;
	using System.Collections;
	using System.Web;
	public abstract class DropDownEditPropertiesBase : ButtonEditPropertiesBase {
		private DropDownButton dropDownButton = null;
		public DropDownEditPropertiesBase()
			: base() {
		}
		public DropDownEditPropertiesBase(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesBaseEnableAnimation"),
#endif
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable,
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the AnimationType property instead")]
		public bool EnableAnimation {
			get { return GetBoolProperty("EnableAnimation", true); }
			set { SetBoolProperty("EnableAnimation", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesBaseAnimationType"),
#endif
		DefaultValue(AnimationType.Auto), NotifyParentProperty(true), AutoFormatDisable]
		public AnimationType AnimationType
		{
			get { return (AnimationType)GetEnumProperty("AnimationType", AnimationType.Auto); }
			set { SetEnumProperty("AnimationType", AnimationType.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesBaseShowShadow"),
#endif
		Category("Appearance"), DefaultValue(true), NotifyParentProperty(true), AutoFormatEnable]
		public virtual bool ShowShadow {
			get { return GetBoolProperty("ShowShadow", true); }
			set { SetBoolProperty("ShowShadow", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesBaseRenderIFrameForPopupElements"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true), AutoFormatEnable, Themeable(false)]
		public DefaultBoolean RenderIFrameForPopupElements {
			get { return (DefaultBoolean)GetEnumProperty("RenderIFrameForPopupElements", DefaultBoolean.Default); }
			set { SetEnumProperty("RenderIFrameForPopupElements", DefaultBoolean.Default, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesBasePopupHorizontalAlign"),
#endif
		Category("Layout"), DefaultValue(PopupHorizontalAlign.LeftSides), NotifyParentProperty(true), AutoFormatDisable]
		public PopupHorizontalAlign PopupHorizontalAlign {
			get { return (PopupHorizontalAlign)GetEnumProperty("PopupHorizontalAlign", PopupHorizontalAlign.LeftSides); }
			set { SetEnumProperty("PopupHorizontalAlign", PopupHorizontalAlign.LeftSides, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesBasePopupVerticalAlign"),
#endif
		Category("Layout"), DefaultValue(PopupVerticalAlign.Below), NotifyParentProperty(true), AutoFormatDisable]
		public PopupVerticalAlign PopupVerticalAlign {
			get { return (PopupVerticalAlign)GetEnumProperty("PopupVerticalAlign", PopupVerticalAlign.Below); }
			set { SetEnumProperty("PopupVerticalAlign", PopupVerticalAlign.Below, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Password {
			get { return false; }
			set { }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesBaseDropDownButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable, NotifyParentProperty(true)]
		public DropDownButton DropDownButton {
			get {
				if(dropDownButton == null)
					dropDownButton = new DropDownButton(this);
				return dropDownButton;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesBaseCustomButtonsPosition"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(CustomButtonsPosition.Near), AutoFormatEnable]
		public CustomButtonsPosition CustomButtonsPosition {
			get { return CustomButtonsPositionInternal; }
			set { CustomButtonsPositionInternal = value; }
		}
		protected DropDownWindowStyle DropDownWindowStyleInternal {
			get { return Styles.DropDownWindow; }
		}
		protected internal new DropDownClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as DropDownClientSideEvents; }
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { DropDownButton });
		}
		public override void Assign(PropertiesBase source) {
			DropDownEditPropertiesBase src = source as DropDownEditPropertiesBase;
			if(src != null) {
				DropDownButton.Assign(src.DropDownButton);
#pragma warning disable 618
				EnableAnimation = src.EnableAnimation;
#pragma warning restore 618
				AnimationType = src.AnimationType;
				ShowShadow = src.ShowShadow;
				RenderIFrameForPopupElements = src.RenderIFrameForPopupElements;
				PopupHorizontalAlign = src.PopupHorizontalAlign;
				PopupVerticalAlign = src.PopupVerticalAlign;
			}
			base.Assign(source);
		}
		protected AppearanceStyleBase GetDefaultDropDownWindowStyleInternal() {
			return Styles.GetDefaultDropDownWindowStyle();
		}
	}
	[ControlValueProperty("Value")]
	public abstract class ASPxDropDownEditBase : ASPxButtonEditBase {
		protected internal const string DropDownEditScriptResourceName = EditScriptsResourcePath + "DropDownEdit.js";
		protected const string PopupControlID = "DDD";
		protected const string MainCellMouseDownHandlerName = "return ASPx.DDMC_MD('{0}', event)";
		protected const string DropDownHandlerName = "return ASPx.DDDropDown('{0}', event)";
		private DropDownControlBase dropDownControl = null;
		public ASPxDropDownEditBase()
			: base() {
			base.AutoCompleteType = System.Web.UI.WebControls.AutoCompleteType.Disabled;
		}
		protected ASPxDropDownEditBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditBaseDropDownButton"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public DropDownButton DropDownButton {
			get { return Properties.DropDownButton; }
		}
		[
		DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable,
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the AnimationType property instead")]
		public bool EnableAnimation {
			get { return Properties.EnableAnimation; }
			set { Properties.EnableAnimation = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditBaseAnimationType"),
#endif
		DefaultValue(AnimationType.Auto), NotifyParentProperty(true), AutoFormatDisable]
		public AnimationType AnimationType
		{
			get { return (AnimationType)GetEnumProperty("AnimationType", AnimationType.Auto); }
			set { SetEnumProperty("AnimationType", AnimationType.Auto, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditBaseShowShadow"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool ShowShadow {
			get { return Properties.ShowShadow; }
			set {
				LayoutChanged();
				Properties.ShowShadow = value;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditBaseRenderIFrameForPopupElements"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), AutoFormatEnable]
		public DefaultBoolean RenderIFrameForPopupElements {
			get { return RenderIFrameForPopupElementsInternal; }
			set { RenderIFrameForPopupElementsInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditBasePopupHorizontalAlign"),
#endif
		Category("Layout"), DefaultValue(PopupHorizontalAlign.LeftSides), NotifyParentProperty(true), AutoFormatDisable]
		public PopupHorizontalAlign PopupHorizontalAlign {
			get { return Properties.PopupHorizontalAlign; }
			set { Properties.PopupHorizontalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditBasePopupVerticalAlign"),
#endif
		Category("Layout"), DefaultValue(PopupVerticalAlign.Below), NotifyParentProperty(true), AutoFormatDisable]
		public PopupVerticalAlign PopupVerticalAlign {
			get { return Properties.PopupVerticalAlign; }
			set { Properties.PopupVerticalAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditBaseCustomButtonsPosition"),
#endif
		Category("Layout"), NotifyParentProperty(true), DefaultValue(CustomButtonsPosition.Near), AutoFormatEnable]
		public CustomButtonsPosition CustomButtonsPosition {
			get { return Properties.CustomButtonsPosition; }
			set { Properties.CustomButtonsPosition = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AutoCompleteType AutoCompleteType {
			get { return base.AutoCompleteType; }
			set { base.AutoCompleteType = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool Password {
			get { return false; }
			set {  }
		}
		protected internal virtual bool NeedPopupControl {
			get { return (!DesignMode || NeedPopupControlInDesingMode) && IsEnabled() && !IsPopupControlShared; }
		}
		protected virtual bool NeedPopupControlInDesingMode {
			get { return false; }
		}
		protected internal virtual bool IsPopupControlShared {
			get { return false; }
		}
		protected internal virtual bool IsNeedItemImageCell {
			get { return false; }
		}
		protected internal new DropDownEditPropertiesBase Properties {
			get { return base.Properties as DropDownEditPropertiesBase; }
		}
		protected new DefaultBoolean RenderIFrameForPopupElementsInternal {
			get { return Properties.RenderIFrameForPopupElements; }
			set { Properties.RenderIFrameForPopupElements = value; }
		}
		protected virtual DropDownControlBase DropDownControl {
			get { return dropDownControl; }
		}
		protected internal override List<EditButton> GetButtonsCore() {
			List<EditButton> defaultButtons = new List<EditButton>();
			defaultButtons.Add(DropDownButton);
			ButtonsMergeHelper helper = new ButtonsMergeHelper(DropDownButton.Position, CustomButtonsPosition, defaultButtons, base.GetButtonsCore());
			List<EditButton> buttons = helper.GetMergedButtons();
			return buttons;
		}
		protected internal int GetDropDownButtonIndex() {
			return DropDownButton.Position == ButtonsPosition.Right ? Buttons.Count : 0;
		}
		protected internal bool ButtonIsDropDown(EditButton button){
			return button is DropDownButton;
		}
		protected internal int GetDropDownButtonIndexForRender() {
			if(!DropDownButton.Visible){
				foreach(EditButton button in Buttons){
					if(button is DropDownButton)
						return button.Index;
				}
			}
			return -1;
		}
		protected internal override string GetButtonOnClick(EditButton button) {
			return button == DropDownButton ? "" : base.GetButtonOnClick(button);
		}
		protected sealed override void ClearControlFields() {
			ClearDropDownControlFields();
			base.ClearControlFields();
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateDropDownControlHierarchy();
		}
		protected sealed override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			PrepareDropDownControlHierarchy();
		}
		protected virtual void ClearDropDownControlFields() {
			this.dropDownControl = null;
		}
		protected virtual void CreateDropDownControlHierarchy() {
			this.dropDownControl = CreateDropDownControl();
			DropDownControl.PopupControlId = PopupControlID;
			DropDownControl.ReadOnly = ReadOnly; 
			DropDownControl.DropDownButton = DropDownButton;
#pragma warning disable 618
			DropDownControl.AnimationType = (!EnableAnimation && AnimationType == AnimationType.Auto) ? AnimationType.None : AnimationType;
#pragma warning restore 618
			DropDownControl.ShowShadow = ShowShadow;
			AddDropDownControl();
		}
		protected virtual bool ClearHeirarchyBefore_AddDropDownControl(){
			return false;
		}
		protected void AddDropDownControl() {
			if(ClearHeirarchyBefore_AddDropDownControl())
				Controls.Clear();
			Controls.Add(DropDownControl);
		}
		protected virtual void PrepareDropDownControlHierarchy() {
			DropDownControl.MainCellMouseDownHandler = GetMainCellMouseDownHandler();
			DropDownControl.DropDownHandler = GetDropDownHandler();
			if(!String.IsNullOrEmpty(AccessibilityInputTitle))
				DropDownControl.SetAccessibilityInputTitle(AccessibilityInputTitle);
		}
		protected virtual DropDownControlBase CreateDropDownControl() {
			return null;
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterIncludeScript(typeof(ASPxDropDownEditBase), DropDownEditScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientDropDownEditBase";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			int dropDownButtonIndex = GetDropDownButtonIndexForRender();
			if(dropDownButtonIndex != -1)
				stb.AppendFormat("{0}.dropDownButtonIndex = {1};\n", localVarName, dropDownButtonIndex);
		}
		protected internal override string GetOnTextChanged() {
			return string.Format(TextChangedHandlerName, ClientID);
		}
		protected string GetMainCellMouseDownHandler() {
			return string.Format(MainCellMouseDownHandlerName, ClientID);
		}
		protected internal string GetDropDownHandler() {
			return string.Format(DropDownHandlerName, ClientID);
		}
		protected internal string GetClientHandler(string functionName) {
			return "function (s, e) { " + string.Format("{0}('{1}', e);", functionName, ClientID) + " }";
		}
		protected internal string GetPopupControlOnShown() {
			return GetClientHandler("ASPx.DDBPCShown");
		}
		protected internal override int GetButtonIndex(EditButton button) {
			if(button == DropDownButton)
				return -1;
			return base.GetButtonIndex(button);
		}
	}
	public class AutoCompleteBoxPropertiesBase : DropDownEditPropertiesBase, IListEditItemsRequester {
		private ComboBoxListBoxProperties autoCompleteBoxListBoxProperties = null;
		private ListEditPropertiesHelper listEditPropertiesHelper = null;
		internal const int DefaultIncrementalFilteringDelay = 100;
		internal const int DefaultDropDownRows = 7;
		private ASPxAutoCompleteBoxBase parentAutoCompleteBox = null;
		public AutoCompleteBoxPropertiesBase()
			: base() {
		}
		public AutoCompleteBoxPropertiesBase(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal ListBoxColumnCollection ColumnsInternal {
			get { return ListBoxProperties.Columns; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string NullText {
			get { return base.NullTextInternal; }
			set { base.NullTextInternal = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowUserInput {
			get { return GetBoolProperty("AllowUserInput", true); }
			set { SetBoolProperty("AllowUserInput", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseCallbackPageSize"),
#endif
		Category("Behavior"), DefaultValue(ComboBoxListBoxProperties.CallbackPageSizeDefault), NotifyParentProperty(true),
		AutoFormatDisable, Themeable(false)]
		public int CallbackPageSize {
			get { return ListBoxProperties.CallbackPageSize; }
			set { ListBoxProperties.CallbackPageSize = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false),
		NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public new AutoCompleteBoxClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as AutoCompleteBoxClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseEnableCallbackMode"),
#endif
		Category("Behavior"), DefaultValue(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public bool EnableCallbackMode {
			get { return ListBoxProperties.EnableCallbackMode; }
			set { ListBoxProperties.EnableCallbackMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseEnableIncrementalFiltering"),
#endif
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the IncrementalFilteringMode property instead."), DefaultValue(false),
		NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public bool EnableIncrementalFiltering {
			get { return IncrementalFilteringMode == IncrementalFilteringMode.StartsWith; }
			set { IncrementalFilteringMode = value ? IncrementalFilteringMode.StartsWith : IncrementalFilteringMode.None; }
		}
		protected bool ShouldSerializeEnableIncrementalFiltering() { return false; }
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseFilterMinLength"),
#endif
		Category("Behavior"), DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int FilterMinLength {
			get { return GetIntProperty("FilterMinLength", 0); }
			set { SetIntProperty("FilterMinLength", 0, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseIncrementalFilteringMode"),
#endif
		Category("Behavior"), DefaultValue(IncrementalFilteringMode.Contains), NotifyParentProperty(true), AutoFormatDisable]
		public virtual IncrementalFilteringMode IncrementalFilteringMode {
			get { return (IncrementalFilteringMode)GetEnumProperty("IncrementalFilteringMode", IncrementalFilteringMode.Contains); }
			set { SetEnumProperty("IncrementalFilteringMode", IncrementalFilteringMode.Contains, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseIncrementalFilteringDelay"),
#endif
		Category("Behavior"), DefaultValue(DefaultIncrementalFilteringDelay), NotifyParentProperty(true), AutoFormatDisable]
		public int IncrementalFilteringDelay {
			get { return GetIntProperty("IncrementalFilteringDelay", DefaultIncrementalFilteringDelay); }
			set { SetIntProperty("IncrementalFilteringDelay", DefaultIncrementalFilteringDelay, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseEnableSynchronization"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true),
		AutoFormatDisable, Themeable(false)]
		public DefaultBoolean EnableSynchronization {
			get { return ListBoxProperties.EnableSynchronization; }
			set { ListBoxProperties.EnableSynchronization = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseDataSecurityMode"),
#endif
		DefaultValue(DataSecurityMode.Default), NotifyParentProperty(true),
		AutoFormatDisable, Themeable(false)]
		public DataSecurityMode DataSecurityMode {
			get { return ListBoxProperties.DataSecurityMode; }
			set { ListBoxProperties.DataSecurityMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseDropDownRows"),
#endif
		Category("Layout"), DefaultValue(DefaultDropDownRows), NotifyParentProperty(true), AutoFormatDisable]
		public int DropDownRows {
			get { return GetIntProperty("DropDownRows", DefaultDropDownRows); }
			set {
				CommonUtils.CheckNegativeOrZeroValue(value, "DropDownRows");
				SetIntProperty("DropDownRows", DefaultDropDownRows, value);
			}
		}
		[Bindable(false), Browsable(false), Category("Data"), Themeable(false), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public object DataSource {
			get { return ListBoxProperties.DataSource; }
			set { ListBoxProperties.DataSource = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseDataSourceID"),
#endif
		DefaultValue(""), Category("Data"), Localizable(false), AutoFormatDisable, Themeable(false),
		NotifyParentProperty(true), IDReferenceProperty(typeof(DataSourceControl)),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceIDConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string DataSourceID {
			get { return ListBoxProperties.DataSourceID; }
			set { ListBoxProperties.DataSourceID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseDataMember"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable, NotifyParentProperty(true), Themeable(false)]
		public string DataMember {
			get { return ListBoxProperties.DataMember; }
			set { ListBoxProperties.DataMember = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseImageUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string ImageUrlField {
			get { return ListBoxProperties.ImageUrlField; }
			set { ListBoxProperties.ImageUrlField = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string TextField {
			get { return ListBoxProperties.TextField; }
			set { ListBoxProperties.TextField = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseValueField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable, Themeable(false),
		TypeConverter("DevExpress.Web.Design.EditPropertiesDataSourceViewSchemaConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string ValueField {
			get { return ListBoxProperties.ValueField; }
			set { ListBoxProperties.ValueField = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseTextFormatString"),
#endif
		NotifyParentProperty(true), Category("Data"), DefaultValue(""), Localizable(true), AutoFormatEnable,
		Editor("DevExpress.Web.Design.TextFormatStringUIEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public string TextFormatString {
			get { return ListBoxProperties.TextFormatString; }
			set { ListBoxProperties.TextFormatString = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		NotifyParentProperty(true), AutoFormatDisable, Themeable(false),
		Editor("DevExpress.Web.Design.ColumnsPropertiesCommonEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public ListEditItemCollection Items {
			get { return ListBoxProperties.Items; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseItemImage"),
#endif
		Category("Images"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ItemImage {
			get { return Images.ListEditItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseCssFilePath"),
#endif
		Category("Styles"), DefaultValue(""), UrlProperty, AutoFormatUrlProperty, NotifyParentProperty(true),
		Editor(typeof(System.Web.UI.Design.UrlEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseCssPostfix"),
#endif
		Category("Styles"), AutoFormatEnable, DefaultValue(""), AutoFormatUrlProperty, NotifyParentProperty(true)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseDisplayImageSpacing"),
#endif
		DefaultValue(typeof(Unit), "4"), NotifyParentProperty(true), AutoFormatEnable]
		public virtual Unit DisplayImageSpacing {
			get { return GetUnitProperty("DisplayImageSpacing", 4); }
			set {
				UnitUtils.CheckNegativeUnit(value, "DisplayImageSpacing");
				SetUnitProperty("DisplayImageSpacing", 4, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseDropDownHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public Unit DropDownHeight {
			get { return GetUnitProperty("DropDownHeight", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "DropDownHeight");
				SetUnitProperty("DropDownHeight", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseDropDownWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public Unit DropDownWidth {
			get { return GetUnitProperty("DropDownWidth", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "DropDownWidth");
				SetUnitProperty("DropDownWidth", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseItemStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ListBoxItemStyle ItemStyle {
			get { return Styles.ListBoxItem; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseListBoxStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceStyleBase ListBoxStyle {
			get { return Styles.ListBox; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("AutoCompleteBoxPropertiesBaseLoadDropDownOnDemand"),
#endif
		Category("Behavior"), DefaultValue(false), NotifyParentProperty(true), AutoFormatEnable, Themeable(false)]
		public bool LoadDropDownOnDemand {
			get { return GetBoolProperty("LoadDropDownOnDemand", false); }
			set { SetBoolProperty("LoadDropDownOnDemand", false, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool EnableSynchronizationOnPerformCallback {
			get { return ListBoxProperties.EnableSynchronizationOnPerformCallback; }
			set { ListBoxProperties.EnableSynchronizationOnPerformCallback = value; }
		}
		protected virtual internal DropDownStyle DropDownStyleInternal {
			get { return (DropDownStyle)GetEnumProperty("DropDownStyleInternal", DropDownStyle.DropDownList); }
			set { SetEnumProperty("DropDownStyleInternal", DropDownStyle.DropDownList, value); }
		}
		protected internal Type ValueTypeInternal {
			get { return ListBoxProperties.ValueType; }
			set { ListBoxProperties.ValueType = value; }
		}
		protected internal ASPxAutoCompleteBoxBase AutoCompleteBox {
			get {
				ASPxAutoCompleteBoxBase autoCompleteBoxBase = Owner as ASPxAutoCompleteBoxBase;
				if(autoCompleteBoxBase != null)
					return autoCompleteBoxBase;
				else
					return parentAutoCompleteBox;
			}
		}
		protected internal string DefaultTextFormatString {
			get { return ListBoxProperties.DefaultTextFormatString; }
		}
		internal ListBoxItemsSerializingHelper SerializingHelper {
			get { return ListBoxProperties.SerializingHelper; }
		}
		protected internal string CustomCallback {
			get { return ListBoxProperties.CustomCallback; }
			set { ListBoxProperties.CustomCallback = value; }
		}
		protected internal object EventCallback {
			get { return ListBoxProperties.EventCallback; }
		}
		protected internal object EventItemInserting {
			get { return ListBoxProperties.EventItemInserting; }
		}
		protected internal object EventItemInserted {
			get { return ListBoxProperties.EventItemInserted; }
		}
		protected internal object EventItemDeleting {
			get { return ListBoxProperties.EventItemDeleting; }
		}
		protected internal object EventItemDeleted {
			get { return ListBoxProperties.EventItemDeleted; }
		}
		protected internal ComboBoxListBoxProperties ListBoxProperties {
			get {
				if(autoCompleteBoxListBoxProperties == null)
					autoCompleteBoxListBoxProperties = CreateListBoxProperties();
				return autoCompleteBoxListBoxProperties;
			}
		}
		protected ListEditPropertiesHelper ListEditPropertiesHelper {
			get {
				if(listEditPropertiesHelper == null)
					listEditPropertiesHelper = new ListEditPropertiesHelper(this, Items);
				return listEditPropertiesHelper;
			}
		}
		protected virtual ComboBoxListBoxProperties CreateListBoxProperties() {
			ComboBoxListBoxProperties properties = new ComboBoxListBoxProperties(this);
			properties.ParentSkinOwner = AutoCompleteBox;
			return properties;
		}
		protected internal void AssignParentStylesToListBoxProperties() {
			ListBoxProperties.ParentImages = RenderImages;
			ListBoxProperties.ParentStyles = RenderStyles;
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				AutoCompleteBoxPropertiesBase src = source as AutoCompleteBoxPropertiesBase;
				if(src != null) {
					EnableCallbackMode = src.EnableCallbackMode;
					LoadDropDownOnDemand = src.LoadDropDownOnDemand;
					EnableSynchronization = src.EnableSynchronization;
					CallbackPageSize = src.CallbackPageSize;
					DisplayImageSpacing = src.DisplayImageSpacing;
					DropDownRows = src.DropDownRows;
					DropDownHeight = src.DropDownHeight;
					DropDownWidth = src.DropDownWidth;
					DropDownStyleInternal = src.DropDownStyleInternal;
					IncrementalFilteringMode = src.IncrementalFilteringMode;
					FilterMinLength = src.FilterMinLength;
					IncrementalFilteringDelay = src.IncrementalFilteringDelay;
					AssignItems(src.Items);
					ListBoxProperties.Assign(src.ListBoxProperties);
					DataSource = src.DataSource;
					DataSourceID = src.DataSourceID;
					DataMember = src.DataMember;
					ImageUrlField = src.ImageUrlField;
					TextField = src.TextField;
					ValueField = src.ValueField;
				}
			} finally {
				EndUpdate();
			}
		}
		bool itemsAssigning = false;
		protected override bool LayoutChangedLocked() {
			return base.LayoutChangedLocked() || this.itemsAssigning;
		}
		protected void AssignItems(ListEditItemCollection srcItems) {
			this.itemsAssigning = true;
			try {
				Items.Assign(srcItems);
				if(AutoCompleteBox != null)
					AutoCompleteBox.OnItemsAssigned();
			} finally {
				this.itemsAssigning = false;
			}
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new AutoCompleteBoxClientSideEvents(this);
		}
		protected override bool IsRequireInplaceBound {
			get { return ListBoxProperties.IsRequireInplaceBoundInternal; }
		}
		protected override void AssignEditorProperties(ASPxEditBase edit) {
			edit.DataSource = DataSource;
			edit.DataSourceID = DataSourceID;
			edit.DataMember = DataMember;
		}
		protected override void AssignInplaceBoundProperties(ASPxEditBase edit) {
			ASPxAutoCompleteBoxBase autoCompleteBoxBase = (ASPxAutoCompleteBoxBase)edit;
			AssignItems(autoCompleteBoxBase.Items);
		}
		protected override ASPxEditBase CreateEditInstance() {
			return null;
		}
		public override HorizontalAlign GetDisplayControlDefaultAlign() {
			return IsRightToLeft() ? HorizontalAlign.Right : HorizontalAlign.Left;
		}
		protected internal bool IsRegularCallback(AutoCompleteBoxCallbackArgumentsReader argumentsReader) {
			return argumentsReader.IsCustomCallback || argumentsReader.IsLoadFilteringItemsCallback || argumentsReader.IsFilterCorrectionCallback || argumentsReader.IsLoadRangeItemsCallback;
		}
		protected virtual internal string OnSpecialCallback(AutoCompleteBoxCallbackArgumentsReader argumentsReader) {
			if(IsRegularCallback(argumentsReader))
				OnRegularCallback(argumentsReader);
			return AutoCompleteBox.GetLoadDropDownOnDemandRender(argumentsReader.LoadDropDownOnDemandRequest);
		}
		protected virtual bool IsSpecialCallback(AutoCompleteBoxCallbackArgumentsReader argumentsReader) { 
			return argumentsReader.IsLoadDropDownOnDemandCallback;
		}
		protected internal string OnRegularCallback(AutoCompleteBoxCallbackArgumentsReader argumentsReader) {
			int beginIndex = argumentsReader.BeginIndex;
			int endIndex = argumentsReader.EndIndex;
			if(argumentsReader.IsCustomCallback) {
				RaiseCustomCallback(argumentsReader.CustomCallbackArg);
				if(EnableCallbackMode && LoadDropDownOnDemand && argumentsReader.IsLoadDropDownOnDemandCallback) {
					beginIndex = ListBoxProperties.LoadOnDemandStrategy.FirstVisibleItemIndex;
					endIndex = beginIndex + (CallbackPageSize - 1);
				}
			}
			ListEditLoadOnDemandStrategyBase.FilteringMode filtrationMode =
				ListEditLoadOnDemandStrategyBase.FilteringMode.None;
			string filter = string.Empty;
			if(argumentsReader.IsLoadFilteringItemsCallback) {
				filtrationMode = ListEditLoadOnDemandStrategyBase.FilteringMode.OnlyFullCoincideWithFilterAllowed;
				filter = argumentsReader.Filter;
			} else if(argumentsReader.IsFilterCorrectionCallback) {
				filtrationMode = ListEditLoadOnDemandStrategyBase.FilteringMode.FilterOutItemsWithMaxCoincide;
				filter = argumentsReader.FilterForCorrection;
			}
			IListEditItemSerializer itemsSerializer = SerializingHelper;
			return LoadOnDemandStrategy.GetSerializedItems(itemsSerializer, filter, filtrationMode, IncrementalFilteringMode, beginIndex, endIndex, argumentsReader.IsCustomCallback);
		}
		protected internal virtual string OnCallback(string eventArgument) {
			AutoCompleteBoxCallbackArgumentsReader argumentsReader = new AutoCompleteBoxCallbackArgumentsReader(eventArgument);
			if(IsSpecialCallback(argumentsReader))
				return OnSpecialCallback(argumentsReader);
			else
				return OnRegularCallback(argumentsReader);
		}
		private void RaiseCustomCallback(string arg) {
			if(AutoCompleteBox != null)
				AutoCompleteBox.OnCustomCallback(new CallbackEventArgsBase(arg));
		}
		protected internal CallbackEventArgsBase GetCustomCallbackSynchronizationEventArg(string customCallbackArgSerialized) {
			return ListBoxProperties.GetCustomCallbackSynchronizationEventArg(customCallbackArgSerialized);
		}
		protected internal ImageProperties GetItemImage(ListEditItem item, bool sampleItem) {
			return ListBoxProperties.GetItemImage(item, sampleItem);
		}
		public override EditorType GetEditorType() {
			return EditorType.Lookup;
		}
		public override object GetExportValue(CreateDisplayControlArgs args) {
			return null;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ListBoxProperties });
		}
		ListEditLoadOnDemandStrategyBase LoadOnDemandStrategy {
			get { return autoCompleteBoxListBoxProperties.LoadOnDemandStrategy; }
		}
		protected internal bool ImageColumnExists {
			get { return ListBoxProperties.ImageColumnExists; }
		}
		protected internal void SetAutoCompleteBoxParent(ASPxAutoCompleteBoxBase parentAutoCompleteBox) {
			this.parentAutoCompleteBox = parentAutoCompleteBox;
			ListBoxProperties.ParentSkinOwner = parentAutoCompleteBox;
			ListBoxProperties.ParentImages = parentAutoCompleteBox.RenderImages;
			ListBoxProperties.ParentStyles = parentAutoCompleteBox.RenderStyles;
		}
		int IListEditItemsRequester.SelectedIndex {
			get { return -1; }
		}
	}
	public abstract class ASPxAutoCompleteBoxBase : ASPxDropDownEditBase, IEditDataHelperOwner, IListEditItemsRequester {
		private string callbackResult = "";
		private int baseDataBindCallLockCount = 0;
		private ListEditHelper listEditHelper = null;
		private AutoCompleteBoxPropertiesBase propertiesFromCtor = null;
		private int loadingPostDataCount = 0;
		private const string ListBoxID = "L";
		public ASPxAutoCompleteBoxBase()
			: base() {
		}
		protected ASPxAutoCompleteBoxBase(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		protected internal ASPxAutoCompleteBoxBase(ASPxWebControl ownerControl, AutoCompleteBoxPropertiesBase properties)
			: base(ownerControl) {
			propertiesFromCtor = properties;
			propertiesFromCtor.SetAutoCompleteBoxParent(this);
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string NullText {
			get { return Properties.NullText; }
			set { Properties.NullText = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowUserInput {
			get { return base.AllowUserInput; }
			set { base.AllowUserInput = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public new AutoCompleteBoxClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseDropDownHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable]
		public Unit DropDownHeight {
			get { return Properties.DropDownHeight; }
			set { Properties.DropDownHeight = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseDropDownWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable]
		public Unit DropDownWidth {
			get { return Properties.DropDownWidth; }
			set { Properties.DropDownWidth = value; }
		}
		protected internal DropDownStyle DropDownStyleInternal {
			get { return Properties.DropDownStyleInternal; }
			set { Properties.DropDownStyleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseCallbackPageSize"),
#endif
		Category("Behavior"), DefaultValue(ListBoxProperties.CallbackPageSizeDefault), AutoFormatDisable, Themeable(false)]
		public int CallbackPageSize {
			get { return Properties.CallbackPageSize; }
			set { Properties.CallbackPageSize = value; }
		}
		protected ListBoxColumnCollection ColumnsInternal {
			get { return Properties.ColumnsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseEnableCallbackMode"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable, Themeable(false)]
		public bool EnableCallbackMode {
			get { return Properties.EnableCallbackMode; }
			set { Properties.EnableCallbackMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseEnableSynchronization"),
#endif
		Category("Behavior"), DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true),
		AutoFormatDisable, Themeable(false)]
		public DefaultBoolean EnableSynchronization {
			get { return Properties.EnableSynchronization; }
			set { Properties.EnableSynchronization = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseDataSecurityMode"),
#endif
		DefaultValue(DataSecurityMode.Default), NotifyParentProperty(true),
		AutoFormatDisable, Themeable(false)]
		public DataSecurityMode DataSecurityMode {
			get { return Properties.DataSecurityMode; }
			set { Properties.DataSecurityMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseEnableIncrementalFiltering"),
#endif
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the IncrementalFilteringMode property instead."),
		Category("Behavior"), DefaultValue(true), AutoFormatDisable, Themeable(false)]
		public bool EnableIncrementalFiltering {
			get { return IncrementalFilteringMode == IncrementalFilteringMode.StartsWith; }
			set { IncrementalFilteringMode = value ? IncrementalFilteringMode.StartsWith : IncrementalFilteringMode.None; }
		}
		protected bool ShouldSerializeEnableIncrementalFiltering() { return false; }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseIncrementalFilteringDelay"),
#endif
		Category("Behavior"), DefaultValue(AutoCompleteBoxPropertiesBase.DefaultIncrementalFilteringDelay), AutoFormatDisable]
		public int IncrementalFilteringDelay {
			get { return Properties.IncrementalFilteringDelay; }
			set { Properties.IncrementalFilteringDelay = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseIncrementalFilteringMode"),
#endif
		Category("Behavior"), DefaultValue(IncrementalFilteringMode.Contains), AutoFormatDisable]
		public virtual IncrementalFilteringMode IncrementalFilteringMode {
			get { return Properties.IncrementalFilteringMode; }
			set { Properties.IncrementalFilteringMode = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseFilterMinLength"),
#endif
		Category("Behavior"), DefaultValue(0), AutoFormatDisable]
		public int FilterMinLength {
			get { return Properties.FilterMinLength; }
			set { Properties.FilterMinLength = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseDropDownRows"),
#endif
		Category("Layout"), DefaultValue(AutoCompleteBoxPropertiesBase.DefaultDropDownRows), AutoFormatDisable]
		public int DropDownRows {
			get { return Properties.DropDownRows; }
			set { Properties.DropDownRows = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseItems"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable, Themeable(false),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(UITypeEditor))]
		public virtual ListEditItemCollection Items {
			get { return Properties.Items; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseItemImage"),
#endif
		Category("Images"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ImageProperties ItemImage {
			get { return Properties.ItemImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseItemStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ListBoxItemStyle ItemStyle {
			get { return Properties.ItemStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseLoadDropDownOnDemand"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public virtual bool LoadDropDownOnDemand {
			get { return Properties.LoadDropDownOnDemand; }
			set { Properties.LoadDropDownOnDemand = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseListBoxStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public AppearanceStyleBase ListBoxStyle {
			get { return Properties.ListBoxStyle; }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseReadOnly")]
#endif
		public override bool ReadOnly {
			get { return base.ReadOnly; }
			set {
				base.ReadOnly = value;
				LayoutChanged();
			}
		}
		int IListEditItemsRequester.SelectedIndex {
			get { return ((IListEditItemsRequester)Properties).SelectedIndex; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseDataSource"),
#endif
		EditorBrowsable(EditorBrowsableState.Always), AutoFormatDisable]
		public override object DataSource {
			get { return base.DataSource; }
			set { base.DataSource = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseDataSourceID"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		AutoFormatDisable, Themeable(false)]
		public override string DataSourceID {
			get { return base.DataSourceID; }
			set { base.DataSourceID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseImageUrlField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable, Themeable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string ImageUrlField {
			get { return Properties.ImageUrlField; }
			set {
				Properties.ImageUrlField = value;
				OnDataFieldChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseTextField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable, Themeable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string TextField {
			get { return Properties.TextField; }
			set { Properties.TextField = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseValueField"),
#endif
		Category("Data"), DefaultValue(""), Localizable(false), AutoFormatDisable, Themeable(false),
		TypeConverter(typeof(System.Web.UI.Design.DataSourceViewSchemaConverter))]
		public virtual string ValueField {
			get { return Properties.ValueField; }
			set { Properties.ValueField = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseTextFormatString"),
#endif
		Category("Data"), DefaultValue(""), Localizable(true), AutoFormatEnable,
		Editor("DevExpress.Web.Design.TextFormatStringUIEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string TextFormatString {
			get { return Properties.TextFormatString; }
			set { Properties.TextFormatString = value; }
		}
		[DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), AutoFormatDisable]
		public virtual bool EnableSynchronizationOnPerformCallback {
			get { return Properties.EnableSynchronizationOnPerformCallback; }
			set { Properties.EnableSynchronizationOnPerformCallback = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseSettingsLoadingPanel"),
#endif
		Category("Settings"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new SettingsLoadingPanel SettingsLoadingPanel {
			get { return base.SettingsLoadingPanel; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseLoadingPanelImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ImageProperties LoadingPanelImage {
			get { return base.LoadingPanelImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseLoadingPanelStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new LoadingPanelStyle LoadingPanelStyle {
			get { return base.LoadingPanelStyle; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(SettingsLoadingPanel.DefaultDelay), AutoFormatDisable]
		public int LoadingPanelDelay {
			get { return SettingsLoadingPanel.Delay; }
			set { SettingsLoadingPanel.Delay = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(ImagePosition.Left), AutoFormatEnable]
		public ImagePosition LoadingPanelImagePosition {
			get { return SettingsLoadingPanel.ImagePosition; }
			set { SettingsLoadingPanel.ImagePosition = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(StringResources.LoadingPanelText), AutoFormatEnable, Localizable(true)]
		public string LoadingPanelText {
			get { return SettingsLoadingPanel.Text; }
			set { SettingsLoadingPanel.Text = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatEnable]
		public bool ShowLoadingPanelImage {
			get { return SettingsLoadingPanel.ShowImage; }
			set { SettingsLoadingPanel.ShowImage = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the SettingsLoadingPanel property instead."),
		DefaultValue(true), AutoFormatDisable]
		public bool ShowLoadingPanel {
			get { return SettingsLoadingPanel.Enabled; }
			set { SettingsLoadingPanel.Enabled = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseCallback"),
#endif
		Category("Action")]
		public event CallbackEventHandlerBase Callback
		{
			add { Events.AddHandler(Properties.EventCallback, value); }
			remove { Events.RemoveHandler(Properties.EventCallback, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseItemDeleting"),
#endif
		Category("Action")]
		public event ASPxDataDeletingEventHandler ItemDeleting
		{
			add { Events.AddHandler(Properties.EventItemDeleting, value); }
			remove { Events.RemoveHandler(Properties.EventItemDeleting, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseItemDeleted"),
#endif
		Category("Action")]
		public event ASPxDataDeletedEventHandler ItemDeleted
		{
			add { Events.AddHandler(Properties.EventItemDeleted, value); }
			remove { Events.RemoveHandler(Properties.EventItemDeleted, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseItemInserting"),
#endif
		Category("Action")]
		public event ASPxDataInsertingEventHandler ItemInserting
		{
			add { Events.AddHandler(Properties.EventItemInserting, value); }
			remove { Events.RemoveHandler(Properties.EventItemInserting, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxAutoCompleteBoxBaseItemInserted"),
#endif
		Category("Action")]
		public event ASPxDataInsertedEventHandler ItemInserted
		{
			add { Events.AddHandler(Properties.EventItemInserted, value); }
			remove { Events.RemoveHandler(Properties.EventItemInserted, value); }
		}
		protected override bool ClientFocusHandlersRequiredForKBSupport {
			get { return true; }
		}
		protected new AutoCompleteControlBase DropDownControl {
			get { return base.DropDownControl as AutoCompleteControlBase; }
		}
		internal virtual bool IsMultiColumn {
			get { return !ColumnsInternal.IsEmpty; }
		}
		protected internal virtual new AutoCompleteBoxPropertiesBase Properties {
			get { return (AutoCompleteBoxPropertiesBase)base.Properties; }
		}
		protected ListEditHelper ListEditHelper {
			get {
				if(listEditHelper == null)
					listEditHelper = new ListEditHelper(this);
				return listEditHelper;
			}
		}
		protected bool IsLoadingPostData { get { return loadingPostDataCount != 0; } }
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(IsMultiColumn && Items.IsEmpty && Page != null && Page.IsPostBack)
				RequireDataBinding();
		}
		protected internal override void ValidateProperties() {
			if(EnableCallbackMode)
				CommonUtils.CheckGreaterOrEqual(CallbackPageSize, DropDownRows, "CallbackPageSize", "DropDownRows");
		}
		protected internal void OnItemsAssigned() {
			ListEditHelper.OnItemsAssigned();
		}
		protected AppearanceStyleBase GetListBoxStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(ListBoxStyle);
			MergeDisableStyle(style);
			return style;
		}
		protected internal ImageProperties GetItemImage(ListEditItem item, bool sampleItem) {
			return Properties.GetItemImage(item, sampleItem);
		}
		protected override bool IsAutoCompleteAttributeAllowed() {
			return !IsNative();
		}
		protected internal override bool IsCallBacksEnabled() {
			return EnableCallbackMode || IsClientSideAPIEnabled();
		}
		public override bool IsClientSideAPIEnabled() {
			return base.IsClientSideAPIEnabled() || EnableCallbackMode || LoadDropDownOnDemand;
		}
		protected internal override bool NeedPopupControl {
			get { return base.NeedPopupControl && (!LoadDropDownOnDemand || IsCallback); }
		}
		protected override void ClearDropDownControlFields() {
			if(!IsNativeRender())
				base.ClearDropDownControlFields();
		}
		protected override void PrepareDropDownControlHierarchy() {
			if(!IsNativeRender()) {
				base.PrepareDropDownControlHierarchy();
				DropDownControl.AutoPostBack = AutoPostBack;
				DropDownControl.DropDownStyle = DropDownStyleInternal;
				DropDownControl.DataSourceID = DataSourceID;
				DropDownControl.DataMember = DataMember;
				DropDownControl.ImageUrlField = ImageUrlField;
				DropDownControl.TextField = TextField;
				DropDownControl.ValueField = ValueField;
			}
		}
		protected internal ASPxListBox CreateListBox() {
			ASPxListBox listBox = CreateListBoxControl();
			ClientIDHelper.EnableClientIDGeneration(listBox);
			listBox.ID = ListBoxID;
			listBox.EnableCallbackMode = EnableCallbackMode;
			listBox.EnableViewState = false;
			listBox.IsComboBoxList = true;
			listBox.IsComboBoxClientSideAPIEnabled = IsClientSideAPIEnabled();
			listBox.EnableSynchronizationOnPerformCallback = EnableSynchronizationOnPerformCallback;
			return listBox;
		}
		protected ASPxListBox CreateListBoxControl() {
			Properties.AssignParentStylesToListBoxProperties();
			return CreateListBoxControlCore();
		}
		protected virtual ASPxListBox CreateListBoxControlCore() {
			return new ASPxListBox(this, Properties.ListBoxProperties);
		}
		protected internal void PrepareListBox(ASPxListBox listBox) {
			listBox.EnableClientSideAPI = EnableClientSideAPI;
			listBox.EncodeHtml = EncodeHtml;
			listBox.InplaceMode = InplaceMode;
			listBox.ReadOnly = ReadOnly;
			listBox.Value = Value;
			GetListBoxStyle().AssignToControl(listBox);
			if(!IsNativeRender()) {
				listBox.ClientSideEvents.SelectedIndexChanged = GetClientHandler("ASPx.CBLBSelectedIndexChanged");
				listBox.ClientSideEvents.ItemClick = GetClientHandler("ASPx.CBLBItemMouseUp");
			}
		}
		protected override EditPropertiesBase CreateProperties() {
			return propertiesFromCtor != null ? propertiesFromCtor : CreatePropertiesInternal();
		}
		protected virtual EditPropertiesBase CreatePropertiesInternal() {
			return null;
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new AutoCompleteControlBase(this);
		}
		protected override bool HasLoadingPanel() {
			return !IsNativeRender() && base.HasLoadingPanel();
		}
		protected override bool HasLoadingDiv() {
			return HasLoadingPanel();
		}
		private string GetListBoxClientObjectStateInputID() {
			if(IsNativeRender())
				return string.Format("{0}${1}{2}", UniqueID, ListBoxID, ClientObjectStateInputIDSuffix);
			else
				return string.Format("{0}${1}${2}{3}", UniqueID, PopupControlID, ListBoxID, ClientObjectStateInputIDSuffix);
		}
		protected override string GetFormattedInputText() { return base.GetFormattedInputText(); }
		protected object AdjustArgTypeForFormatString(string arg) {
			long l;
			decimal d;
			if(long.TryParse(arg, out l))
				return l;
			else if(decimal.TryParse(arg, out d))
				return d;
			return arg;
		}
		protected virtual bool IsTextFormatCapabilityEnabled() {
			return !string.IsNullOrEmpty(TextFormatString) || IsMultiColumn;
		}
		protected internal override bool IsFormatterScriptRequired() {
			return base.IsFormatterScriptRequired() || IsTextFormatCapabilityEnabled() || IncrementalFilteringMode == IncrementalFilteringMode.Contains;
		}
		protected internal override bool IsCultureInfoScriptRequired() {
			return base.IsCultureInfoScriptRequired() || IsTextFormatCapabilityEnabled();
		}
		public override void RegisterEditorIncludeScripts() {
			base.RegisterEditorIncludeScripts();
			RegisterPopupUtilsScripts();
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!IsNativeRender()) {
				if(!DropDownHeight.IsEmpty)
					stb.AppendFormat("{0}.dropDownHeight='{1}';\n", localVarName, DropDownHeight.ToString());
				if(!DropDownWidth.IsEmpty)
					stb.AppendFormat("{0}.dropDownWidth='{1}';\n", localVarName, DropDownWidth.ToString());
				if(DropDownStyleInternal == DropDownStyle.DropDown && DataSecurityMode == DataSecurityMode.Default)
					stb.AppendFormat("{0}.isDropDownListStyle=false;\n", localVarName);
				if(IncrementalFilteringMode != IncrementalFilteringMode.None) {
					if(IncrementalFilteringDelay != AutoCompleteBoxPropertiesBase.DefaultIncrementalFilteringDelay)
						stb.AppendFormat("{0}.filterTimer = {1};\n", localVarName, IncrementalFilteringDelay);
					if(IncrementalFilteringMode == IncrementalFilteringMode.StartsWith)
						stb.AppendFormat("{0}.incrementalFilteringMode='{1}';\n", localVarName, "StartsWith");
				}
				else
					stb.AppendFormat("{0}.incrementalFilteringMode='{1}';\n", localVarName, "None");
				if(EnableCallbackMode)
					stb.AppendFormat("{0}.isCallbackMode = true;\n", localVarName);
				if(LoadDropDownOnDemand)
					stb.AppendFormat("{0}.loadDropDownOnDemand = true;\n", localVarName);
				if(!EncodeHtml && IsEnabled())
					stb.AppendFormat("{0}.initTextCorrectionRequired = true;\n", localVarName);
				if(DropDownRows != AutoCompleteBoxPropertiesBase.DefaultDropDownRows)
					stb.AppendFormat("{0}.dropDownRows = {1};\n", localVarName, DropDownRows);
				if(FilterMinLength != 0)
					stb.AppendFormat("{0}.filterMinLength = {1};\n", localVarName, FilterMinLength);
				stb.AppendFormat("{0}.lastSuccessValue = {1};\n", localVarName,
					HtmlConvertor.ToJSON(Properties.ValueTypeInternal == typeof(String) ? HttpUtility.HtmlAttributeEncode(Value as string) : Value));
				stb.AppendFormat("{0}.islastSuccessValueInit = true;\n", localVarName);
			}
		}
		protected override object GetCallbackResult() {
			return callbackResult;
		}
		protected override void RaiseCallbackEvent(string eventArgument) {
			callbackResult = Properties.OnCallback(eventArgument);
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			if(IsMultiColumn && Items.IsEmpty)
				EnsureItemsDataBound(); 
			else
				EnsureDataBound();
			SynchronizeCustomCallback(postCollection); 
			if(DataSecurityMode == DataSecurityMode.Default)
				SynchronizeItems(postCollection); 
			if(NotValueChangingRequest())
				return false;
			loadingPostDataCount++;
			bool result = base.LoadPostData(postCollection);
			loadingPostDataCount--;
			return result;
		}
		protected virtual bool NotValueChangingRequest() {
			return false;
		}
		protected void SynchronizeCustomCallback(NameValueCollection postCollection) {
			if(Page == null || !Page.IsCallback) {
				Hashtable listBoxClientState = LoadClientObjectState(postCollection, GetListBoxClientObjectStateInputID());
				string customCallbackArgSerialized = GetClientObjectStateValueString(listBoxClientState, ASPxListBox.SynchronizationType.CustomCallback.ToString());
				CallbackEventArgsBase eventArg = Properties.GetCustomCallbackSynchronizationEventArg(customCallbackArgSerialized);
				if(eventArg != null)
					OnCustomCallback(eventArg);
			}
		}
		protected void SynchronizeItems(NameValueCollection postCollection) {
			Hashtable listBoxClientState = LoadClientObjectState(postCollection, GetListBoxClientObjectStateInputID());
			string stateString = GetClientObjectStateValueString(listBoxClientState, ASPxListBox.SynchronizationType.DeletedItems.ToString());
			if(!string.IsNullOrEmpty(stateString))
				Properties.ListBoxProperties.SynchronizeItems(stateString, false);
			stateString = GetClientObjectStateValueString(listBoxClientState, ASPxListBox.SynchronizationType.InsertedItems.ToString());
			if(!string.IsNullOrEmpty(stateString))
				Properties.ListBoxProperties.SynchronizeItems(stateString, true);
		}
		protected internal virtual void OnCustomCallback(CallbackEventArgsBase e) {
			LockBaseDataBindCall(); 
			try {
				CallbackEventHandlerBase handler = Events[Properties.EventCallback] as CallbackEventHandlerBase;
				if(handler != null)
					handler(this, e);
			} finally {
				UnlockBaseDataBindCall();
			}
		}
		protected internal void RaiseItemDeleting(ASPxDataDeletingEventArgs e) {
			ASPxDataDeletingEventHandler handler = (ASPxDataDeletingEventHandler)Events[Properties.EventItemDeleting];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseItemDeleted(ASPxDataDeletedEventArgs e) {
			ASPxDataDeletedEventHandler handler = (ASPxDataDeletedEventHandler)Events[Properties.EventItemDeleted];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseItemInserting(ASPxDataInsertingEventArgs e) {
			ASPxDataInsertingEventHandler handler = (ASPxDataInsertingEventHandler)Events[Properties.EventItemInserting];
			if(handler != null)
				handler(this, e);
		}
		protected internal void RaiseItemInserted(ASPxDataInsertedEventArgs e) {
			ASPxDataInsertedEventHandler handler = (ASPxDataInsertedEventHandler)Events[Properties.EventItemInserted];
			if(handler != null)
				handler(this, e);
		}
		public virtual void DataBindItems() { 
			LockBaseDataBindCall();
			try {
				DataBind();
			} finally {
				UnlockBaseDataBindCall();
			}
		}
		protected internal override void PerformDataBinding(string dataHelperName, IEnumerable data) {
			if(PerformDataBindingCore(dataHelperName, data))
				ResetControlHierarchy();
		}
		protected virtual bool PerformDataBindingCore(string dataHelperName, IEnumerable data) {
			if(ColumnsInternal.IsEmpty)
				return ListEditHelper.PerformDataBinding(data, ValueField, TextField, ImageUrlField);
			else
				return ListEditHelper.PerformDataBindingMulticolumn(data);
		}
		protected override void OnDataBinding(EventArgs e) {
			if(BaseDataBindCallIsLocked())
				return; 
			EnsureChildControls();
			base.OnDataBinding(e);
		}
		protected void EnsureItemsDataBound() {
			LockBaseDataBindCall();
			try {
				RequireDataBinding();
				EnsureDataBound();
			} finally {
				UnlockBaseDataBindCall();
			}
		}
		protected void LockBaseDataBindCall() {
			baseDataBindCallLockCount++;
		}
		protected void UnlockBaseDataBindCall() {
			baseDataBindCallLockCount--;
		}
		protected bool BaseDataBindCallIsLocked() {
			return baseDataBindCallLockCount > 0;
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			ListEditHelper.OnViewStateLoaded();
		}
		protected internal virtual string GetLoadDropDownOnDemandRender(string args) {
			RenderUtils.EnsureChildControlsRecursive(this, false);
			RenderUtils.EnsurePrepareChildControlsRecursive(this, false);
			string renderHtml = RenderUtils.GetRenderResult(DropDownControl.PopupControl);
			return RenderUtils.CallBackResultPrefix + HtmlConvertor.ToJSON(renderHtml);
		}
		#region IEditDataHelperOwner Members
		Type IEditDataHelperOwner.ValueType {
			get { return Properties.ValueTypeInternal; }
		}
		#endregion
	}
	public class DropDownEditProperties : DropDownEditPropertiesBase {
		private ITemplate dropDownWindowTemplate = null;
		public DropDownEditProperties()
			: base() {
		}
		public DropDownEditProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowUserInput {
			get { return base.AllowUserInput; }
			set { base.AllowUserInput = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(true)]
		public string NullText {
			get { return NullTextInternal; }
			set { NullTextInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesDropDownWindowHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public Unit DropDownWindowHeight {
			get { return GetUnitProperty("DropDownWindowHeight", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "DropDownWindowHeight");
				SetUnitProperty("DropDownWindowHeight", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesDropDownWindowWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public Unit DropDownWindowWidth {
			get { return GetUnitProperty("DropDownWindowWidth", Unit.Empty); }
			set {
				UnitUtils.CheckNegativeUnit(value, "DropDownWindowWidth");
				SetUnitProperty("DropDownWindowWidth", Unit.Empty, value);
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesClientSideEvents"),
#endif
		Category("Client-Side"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false),
		NotifyParentProperty(true), AutoFormatDisable, Themeable(false)]
		public new DropDownClientSideEvents ClientSideEvents {
			get { return base.ClientSideEvents as DropDownClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("DropDownEditPropertiesDropDownWindowStyle"),
#endif
		Category("Styles"), NotifyParentProperty(true), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public DropDownWindowStyle DropDownWindowStyle {
			get { return DropDownWindowStyleInternal; }
		}
		[Browsable(false), DefaultValue(null),
		TemplateContainer(typeof(TemplateContainerBase)),
		PersistenceMode(PersistenceMode.InnerProperty),
		NotifyParentProperty(true), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate DropDownWindowTemplate {
			get { return dropDownWindowTemplate; }
			set {
				dropDownWindowTemplate = value;
				Changed();
			}
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxDropDownEdit();
		}
		protected override EditClientSideEventsBase CreateClientSideEvents() {
			return new DropDownClientSideEvents(this);
		}
		public override void Assign(PropertiesBase source) {
			DropDownEditProperties src = source as DropDownEditProperties;
			if(src != null) {
				DropDownWindowHeight = src.DropDownWindowHeight;
				DropDownWindowWidth = src.DropDownWindowWidth;
				DropDownWindowTemplate = src.DropDownWindowTemplate;
			}
			base.Assign(source);
		}
		protected internal AppearanceStyleBase GetDefaultDropDownWindowStyle() {
			return GetDefaultDropDownWindowStyleInternal();
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxDropDownEdit.bmp"),
	Designer("DevExpress.Web.Design.ASPxDropDownEditDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull)
	]
	public class ASPxDropDownEdit : ASPxDropDownEditBase {
		protected const string DropDownTemplateContainerID = "DDTC";
		protected const string DropDownDivContainerID = "DDDC";
		protected const string KeyValueKey = "keyValue";
		public ASPxDropDownEdit()
			: base() {
		}
		protected ASPxDropDownEdit(ASPxWebControl ownerControl)
			: base(ownerControl) {
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AllowUserInput {
			get { return base.AllowUserInput; }
			set { base.AllowUserInput = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowMouseWheel {
			get { return base.AllowMouseWheel; }
			set { base.AllowMouseWheel = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditNullText"),
#endif
		DefaultValue(""), AutoFormatDisable, Localizable(true)]
		public string NullText {
			get { return Properties.NullText; }
			set { Properties.NullText = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditDropDownWindowHeight"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable]
		public Unit DropDownWindowHeight {
			get { return Properties.DropDownWindowHeight; }
			set { Properties.DropDownWindowHeight = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditDropDownWindowWidth"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatDisable]
		public Unit DropDownWindowWidth {
			get { return Properties.DropDownWindowWidth; }
			set { Properties.DropDownWindowWidth = value; }
		}
		[Browsable(false), DefaultValue(null), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object KeyValue {
			get { return GetObjectProperty("KeyValue", null); }
			set { SetObjectProperty("KeyValue", null, value); }
		}
		[Browsable(false), DefaultValue(null),
		TemplateContainer(typeof(TemplateContainerBase)),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate DropDownWindowTemplate {
			get { return Properties.DropDownWindowTemplate; }
			set { Properties.DropDownWindowTemplate = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditClientSideEvents"),
#endif
		Category("Client-Side"), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public new DropDownClientSideEvents ClientSideEvents {
			get { return Properties.ClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxDropDownEditDropDownWindowStyle"),
#endif
		Category("Styles"), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public DropDownWindowStyle DropDownWindowStyle {
			get { return Properties.DropDownWindowStyle; }
		}
		protected AppearanceStyleBase DefaultDropDownWindowStyle {
			get { return Properties.GetDefaultDropDownWindowStyle(); }
		}
		protected internal new DropDownEditProperties Properties {
			get { return base.Properties as DropDownEditProperties; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new DropDownEditProperties(this);
		}
		protected override DropDownControlBase CreateDropDownControl() {
			return new DropDownControl(this);
		}
		public new virtual Control FindControl(string id) {
			if(DropDownWindowTemplate != null) {
				DropDownPopupControl popupControl = (DropDownPopupControl)base.FindControl(PopupControlID);
				if(popupControl != null) {
					TemplateContainerBase container = popupControl.FindControl(GetDropDownWindowTemplateContainerID()) as TemplateContainerBase;
					if(container != null) {
						Control control = container.FindControl(id);
						if(control != null)
							return control;
					}
				}
			}
			return base.FindControl(id);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientDropDownEdit";
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!DropDownWindowHeight.IsEmpty)
				stb.AppendFormat("{0}.dropDownWindowHeight='{1}';\n", localVarName, DropDownWindowHeight.Value);
			if(!DropDownWindowWidth.IsEmpty)
				stb.AppendFormat("{0}.dropDownWindowWidth='{1}';\n", localVarName, DropDownWindowWidth.Value);
		}
		protected override Hashtable GetClientObjectState() {
			Hashtable result = base.GetClientObjectState();
			result.Add(KeyValueKey, KeyValue != null ? KeyValue.ToString() : "");
			return result;
		}
		protected internal AppearanceStyleBase GetDropDownWindowStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(DefaultDropDownWindowStyle);
			style.CopyFrom(RenderStyles.DropDownWindow);
			return style;
		}
		protected internal string GetDropDownWindowDivContainerID() {
			return DropDownDivContainerID;
		}
		protected internal string GetDropDownWindowTemplateContainerID() {
			return DropDownTemplateContainerID;
		}
		protected override bool LoadPostData(NameValueCollection postCollection) {
			bool valueChanged = base.LoadPostData(postCollection);
			string newKeyValueString = GetClientObjectStateValueString(KeyValueKey);
			if(IsEnabled() && !newKeyValueString.Equals(KeyValue)) {
				KeyValue = newKeyValueString;
				valueChanged = true;
			}
			return valueChanged;
		}
		protected override object SaveViewState() {
			SetViewStateStoringFlag();
			return base.SaveViewState();
		}
	}
}
