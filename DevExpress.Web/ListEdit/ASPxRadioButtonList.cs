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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Internal.InternalCheckBox;
namespace DevExpress.Web {
	public abstract class CheckListPropertiesBase : ListEditProperties {
		public CheckListPropertiesBase()
			: base() {
		}
		public CheckListPropertiesBase(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckListPropertiesBaseClientSideEvents"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty),
		AutoFormatDisable, MergableProperty(false), NotifyParentProperty(true)]
		public new ListEditClientSideEvents ClientSideEvents
		{
			get { return base.ClientSideEvents as ListEditClientSideEvents; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckListPropertiesBaseItemSpacing"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatEnable, NotifyParentProperty(true)]
		public Unit ItemSpacing
		{
			get { return GetUnitProperty("ItemSpacing", Unit.Empty); }
			set { SetUnitProperty("ItemSpacing", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckListPropertiesBaseTextWrap"),
#endif
		DefaultValue(true), AutoFormatEnable, NotifyParentProperty(true)]
		public bool TextWrap
		{
			get { return GetBoolProperty("TextWrap", true); }
			set { SetBoolProperty("TextWrap", true, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckListPropertiesBaseTextAlign"),
#endif
		DefaultValue(TextAlign.Right), AutoFormatEnable, NotifyParentProperty(true)]
		public TextAlign TextAlign
		{
			get { return (TextAlign)GetEnumProperty("TextAlign", TextAlign.Right); }
			set { SetEnumProperty("TextAlign", TextAlign.Right, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckListPropertiesBaseTextSpacing"),
#endif
		DefaultValue(typeof(Unit), ""), AutoFormatEnable, NotifyParentProperty(true)]
		public Unit TextSpacing
		{
			get { return GetUnitProperty("TextSpacing", Unit.Empty); }
			set { SetUnitProperty("TextSpacing", Unit.Empty, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckListPropertiesBaseRepeatColumns"),
#endif
		DefaultValue(0), NotifyParentProperty(true), AutoFormatDisable]
		public int RepeatColumns
		{
			get { return GetIntProperty("RepeatColumns", 0); }
			set
			{
				CommonUtils.CheckNegativeValue(value, "RepeatColumns");
				SetIntProperty("RepeatColumns", 0, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckListPropertiesBaseRepeatDirection"),
#endif
		DefaultValue(RepeatDirection.Vertical), NotifyParentProperty(true), AutoFormatDisable]
		public RepeatDirection RepeatDirection
		{
			get { return (RepeatDirection)GetEnumProperty("RepeatDirection", RepeatDirection.Vertical); }
			set
			{
				SetEnumProperty("RepeatDirection", RepeatDirection.Vertical, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckListPropertiesBaseRepeatLayout"),
#endif
		DefaultValue(RepeatLayout.Table), NotifyParentProperty(true), AutoFormatDisable]
		public RepeatLayout RepeatLayout
		{
			get { return (RepeatLayout)GetEnumProperty("RepeatLayout", RepeatLayout.Table); }
			set
			{
				SetEnumProperty("RepeatLayout", RepeatLayout.Table, value);
				LayoutChanged();
			}
		}
		protected internal ASPxCheckListBase ButtonListBase {
			get { return Owner as ASPxCheckListBase; }
		}
		public override void Assign(PropertiesBase source) {
			BeginUpdate();
			try {
				base.Assign(source);
				CheckListPropertiesBase src = source as CheckListPropertiesBase;
				if(src != null) {
					ItemSpacing = src.ItemSpacing;
					TextAlign = src.TextAlign;
					TextSpacing = src.TextSpacing;
					TextWrap = src.TextWrap;
					RepeatColumns = src.RepeatColumns;
					RepeatDirection = src.RepeatDirection;
					RepeatLayout = src.RepeatLayout;
				}
			} finally {
				EndUpdate();
			}
		}
		internal abstract InternalCheckBoxImageProperties GetImage(CheckState checkState, Page page);
		protected abstract string GetButtonCheckedImageName();
		protected abstract string GetButtonUncheckedImageName();
		protected abstract InternalCheckBoxImageProperties GetButtonCheckedImageProperties();
		protected abstract InternalCheckBoxImageProperties GetButtonUncheckedImageProperties();
		protected internal override bool? GetItemSelected(ListEditItem item) {
			if(ButtonListBase != null)
				return ButtonListBase.SelectedItem == item;
			return null;
		}
		protected internal override void OnItemSelectionChanged(ListEditItem item, bool selected) {
			if(ButtonListBase != null) {
				ButtonListBase.SelectedItem = item;
			}
		}
		public override EditorType GetEditorType() {
			return EditorType.Lookup;
		}
	}
	public class RadioButtonListProperties : CheckListPropertiesBase {
		public RadioButtonListProperties()
			: base() {
		}
		public RadioButtonListProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RadioButtonListPropertiesCheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public InternalCheckBoxImageProperties CheckedImage {
			get { return Images.RadioButtonChecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RadioButtonListPropertiesUncheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public InternalCheckBoxImageProperties UncheckedImage {
			get { return Images.RadioButtonUnchecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RadioButtonListPropertiesRadioButtonFocusedStyle"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle RadioButtonFocusedStyle {
			get { return Styles.RadioButtonFocused; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("RadioButtonListPropertiesRadioButtonStyle"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle RadioButtonStyle {
			get { return Styles.RadioButton; }
		}
		protected override string GetButtonCheckedImageName(){
			return EditorImages.RadioButtonCheckedImageName;
		}
		protected override string GetButtonUncheckedImageName(){
			return EditorImages.RadioButtonUncheckedImageName;
		}
		protected override InternalCheckBoxImageProperties GetButtonCheckedImageProperties(){
			return Images.RadioButtonChecked;
		}
		protected override InternalCheckBoxImageProperties GetButtonUncheckedImageProperties(){
			return Images.RadioButtonUnchecked;
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxRadioButtonList();
		}
		internal override InternalCheckBoxImageProperties GetImage(CheckState checkState, Page page) {
			InternalCheckBoxImageProperties result = new InternalCheckBoxImageProperties();
			string imageName = string.Empty;
			switch(checkState) {
				case CheckState.Checked:
					imageName = GetButtonCheckedImageName();
					result.MergeWith(GetButtonCheckedImageProperties());
					break;
				default:
					imageName = GetButtonUncheckedImageName();
					result.MergeWith(GetButtonUncheckedImageProperties());
					break;
			}
			result.MergeWith(Images.GetImageProperties(page, imageName));
			return result;
		}
	}
	public class CheckBoxListProperties : CheckListPropertiesBase {
		public CheckBoxListProperties()
			: base() {
		}
		public CheckBoxListProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxListPropertiesCheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public InternalCheckBoxImageProperties CheckedImage {
			get { return Images.CheckBoxChecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxListPropertiesUncheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public InternalCheckBoxImageProperties UncheckedImage {
			get { return Images.CheckBoxUnchecked; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxListPropertiesCheckBoxFocusedStyle"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle CheckBoxFocusedStyle {
			get { return Styles.CheckBoxFocused; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("CheckBoxListPropertiesCheckBoxStyle"),
#endif
		NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle CheckBoxStyle {
			get { return Styles.CheckBox; }
		}
		protected ASPxCheckBoxList CheckBoxList {
			get { return ButtonListBase as ASPxCheckBoxList;}
		}
		protected override string GetButtonCheckedImageName(){
			return EditorImages.CheckBoxCheckedImageName;
		}
		protected override string GetButtonUncheckedImageName(){
			return EditorImages.CheckBoxUncheckedImageName;
		}
		protected override InternalCheckBoxImageProperties GetButtonCheckedImageProperties(){
			return Images.CheckBoxChecked;
		}
		protected override InternalCheckBoxImageProperties GetButtonUncheckedImageProperties(){
			return Images.CheckBoxUnchecked;
		}
		protected override ASPxEditBase CreateEditInstance() {
			return new ASPxCheckBoxList();
		}
		protected internal override bool? GetItemSelected(ListEditItem item) {
			if(CheckBoxList != null)
				return CheckBoxList.GetItemSelected(item);
			return null;
		}
		protected internal override void OnItemSelectionChanged(ListEditItem item, bool selected) {
			if(CheckBoxList != null)
				CheckBoxList.OnItemSelectionChanged(item, selected);
		}
		protected internal override void OnItemDeleting(ListEditItem item) {
			if(CheckBoxList != null)
				CheckBoxList.OnItemDeleting(item);
		}
		protected internal override void OnItemsCleared() {
			if(CheckBoxList != null)
				CheckBoxList.OnItemsCleared();
		}
		internal override InternalCheckBoxImageProperties GetImage(CheckState checkState, Page page) {
			InternalCheckBoxImageProperties result = new InternalCheckBoxImageProperties();
			string imageName = string.Empty;
			switch(checkState) {
				case CheckState.Checked:
					imageName = GetButtonCheckedImageName();
					result.MergeWith(GetButtonCheckedImageProperties());
					break;
				default:
					imageName = GetButtonUncheckedImageName();
					result.MergeWith(GetButtonUncheckedImageProperties());
					break;
			}
			Images.UpdateSpriteUrl(result, page, InternalCheckboxControl.WebSpriteControlName, typeof(ASPxWebControl), InternalCheckboxControl.DesignModeSpriteImagePath);
			result.MergeWith(Images.GetImageProperties(page, imageName));
			return result;
		}
	}
	public abstract class ASPxCheckListBase : ASPxListEdit {
		private ItemsControl<ListEditItem> itemsControl = null;
		public ASPxCheckListBase()
			: base() {
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseItemSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit ItemSpacing {
			get { return Properties.ItemSpacing; }
			set { Properties.ItemSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBasePaddings"),
#endif
		Category("Layout"), AutoFormatEnable,
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return (ControlStyle as AppearanceStyle).Paddings; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseTextWrap"),
#endif
		Category("Appearance"), DefaultValue(true), AutoFormatEnable]
		public bool TextWrap {
			get { return Properties.TextWrap; }
			set { Properties.TextWrap = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseTextAlign"),
#endif
		Category("Appearance"), DefaultValue(TextAlign.Right), AutoFormatEnable]
		public TextAlign TextAlign {
			get { return Properties.TextAlign; }
			set { Properties.TextAlign = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseTextSpacing"),
#endif
		Category("Layout"), DefaultValue(typeof(Unit), ""), AutoFormatEnable]
		public Unit TextSpacing {
			get { return Properties.TextSpacing; }
			set { Properties.TextSpacing = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseRightToLeft"),
#endif
		Category("Layout"), DefaultValue(DefaultBoolean.Default), AutoFormatDisable]
		public DefaultBoolean RightToLeft {
			get { return RightToLeftInternal; }
			set { RightToLeftInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseNative"),
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
	DevExpressWebLocalizedDescription("ASPxCheckListBaseEncodeHtml"),
#endif
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always), AutoFormatDisable,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Category("Behavior"), DefaultValue(true), NotifyParentProperty(true)]
		public override bool EncodeHtml {
			get { return Properties.EncodeHtml; }
			set { Properties.EncodeHtml = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseRepeatColumns"),
#endif
		Category("Layout"), DefaultValue(0), AutoFormatDisable]
		public int RepeatColumns {
			get { return Properties.RepeatColumns; }
			set { Properties.RepeatColumns = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseRepeatDirection"),
#endif
		Category("Layout"), DefaultValue(RepeatDirection.Vertical), AutoFormatDisable]
		public RepeatDirection RepeatDirection {
			get { return Properties.RepeatDirection; }
			set { Properties.RepeatDirection = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseRepeatLayout"),
#endif
		Category("Layout"), DefaultValue(RepeatLayout.Table), AutoFormatDisable]
		public RepeatLayout RepeatLayout {
			get { return Properties.RepeatLayout; }
			set { Properties.RepeatLayout = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseItemImage"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ImageProperties ItemImage {
			get { return Properties.ItemImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxCheckListBaseClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public ListEditClientSideEvents ClientSideEvents {
			get { return base.ClientSideEventsInternal as ListEditClientSideEvents; }
		}
		protected internal new CheckListPropertiesBase Properties {
			get { return base.Properties as CheckListPropertiesBase; }
		}
		protected ItemsControl<ListEditItem> ItemsControl {
			get { return itemsControl; }
		}
		protected internal override void InitInternal() {
			base.InitInternal();
			Properties.ConvertItemTypes();
		}
		protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
			EnsureDataBound(); 
			return base.LoadPostData(postCollection);
		}
		protected override bool HasFunctionalityScripts() {
			return Items.Count > 0;
		}
		protected override bool IsCustomValidationEnabled {
			get {
				return base.IsCustomValidationEnabled && !Items.IsEmpty;
			}
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			base.AddDisabledItems(helper);
			for(int i = 0; i < Items.Count; i++)
				helper.AddStyle(GetDisabledCssStyle(), GetItemID(i), IsEnabled());
		}
		protected override void ClearControlFields() {
			this.itemsControl = null;
			base.ClearControlFields();
		}
		protected virtual ItemsControl<ListEditItem> GetCreateControl() {
			return new ButtonListItemsControlBase(this, GetItemsList());
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			this.itemsControl = GetCreateControl();
			Controls.Add(ItemsControl);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(ItemsControl != null) {
				RenderUtils.AssignAttributes(this, ItemsControl); 
				ItemsControl.ItemsControlStyle = GetControlStyle();
				ItemsControl.ItemsControlPaddings = Paddings;
				ItemsControl.ItemsControlMainCellCssClass = GetCssClassNamePrefix();
				ItemsControl.ItemSpacing = ItemSpacing;
			}
		}
		protected abstract ASPxCheckBox CreateItemControlCore(int index, ListEditItem item);
		protected internal Control CreateItemControl(int index, ListEditItem item) {
			ASPxCheckBox itemControl = CreateItemControlCore(index, item);			
			itemControl.Layout = Properties.RepeatLayout;
			itemControl.TextAlign = Properties.TextAlign;
			itemControl.RightToLeft = RightToLeft;
			itemControl.Native = Native;
			itemControl.ParentSkinOwner = this;
			return itemControl as Control;
		}
		protected internal void PrepareItemControl(Control control, int index, ListEditItem item) {
			ASPxCheckBox itemControl = (ASPxCheckBox)control;
			itemControl.TabIndex = this.TabIndex;
			itemControl.ID = GetItemID(index);
			itemControl.Enabled = IsEnabled();
			itemControl.ReadOnly = ReadOnly;
			itemControl.EnableViewState = false;
			itemControl.AutoPostBack = AutoPostBack;
			itemControl.ValidationSettings.CausesValidation = ValidationSettings.CausesValidation;
			itemControl.ValidationSettings.ValidationGroup = ValidationSettings.ValidationGroup;
			itemControl.Image.Assign(GetItemImage(index));
			itemControl.Text = item.Text;
			itemControl.TextSpacing = Properties.TextSpacing;
			itemControl.Wrap = Properties.TextWrap ? DefaultBoolean.True : DefaultBoolean.False;
			itemControl.ControlStyle.CopyFrom(GetItemControlStyle());
			itemControl.ValueType = ValueType;
			if(GetItemSelected(item).Value)
				itemControl.Checked = true;
			itemControl.ClientSideEvents.AssignWithoutValidation(ClientSideEvents);
			if(Native && IsRightToLeft() && RepeatLayout == System.Web.UI.WebControls.RepeatLayout.Flow) {
				itemControl.Style["display"] = "inline-block";
			}
		}
		protected internal virtual bool? GetItemSelected(ListEditItem item) {
			return SelectedIndex > -1 && item.Index == SelectedIndex;
		}
		protected internal List<ListEditItem> GetItemsList() {
			List<ListEditItem> itemList = new List<ListEditItem>();
			for(int i = 0; i < Properties.Items.Count; i ++)
				itemList.Add(Properties.Items[i]);
			return itemList;
		}
		protected internal string GetItemID(int index) {
			return "RB" + index.ToString();
		}
		protected internal ImageProperties GetItemImage(int index) {
			ImageProperties image = new ImageProperties();
			image.CopyFrom(Properties.ItemImage);
			if(Properties.Items[index].ImageUrl != "")
				image.Url = Properties.Items[index].ImageUrl;
			return image;
		}
		protected abstract List<InternalCheckBoxImageProperties> GetImages();
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			stb.Append(GetClientItemsScript(localVarName));
			if(!Native && Enabled) {
				stb.Append(string.Format("{0}.imageProperties = {1};\n", localVarName, ImagePropertiesSerializer.GetImageProperties(GetImages(), this)));
				stb.Append(string.Format("{0}.icbFocusedStyle = {1};\n", localVarName, InternalCheckboxControl.SerializeFocusedStyle(GetInternalCheckBoxFocusedStyle(), this)));
			}
		}
		protected string GetClientItemsScript(string localVarName) {
			object[] itemsArray = new object[Properties.Items.Count];
			for(int i = 0; i < Properties.Items.Count; i++) {
				object[] itemProperties = new object[3];
				ListEditItem item = Properties.Items[i];
				itemProperties[0] = item.Text;
				itemProperties[1] = GetItemClientValue(item);
				itemProperties[2] = item.ImageUrl;
				itemsArray[i] = itemProperties;
			}
			return localVarName + ".CreateItems(" + HtmlConvertor.ToJSON(itemsArray) + ");\n";
		}
		protected abstract AppearanceStyleBase GetInternalCheckBoxFocusedStyle();
		protected internal abstract AppearanceStyleBase GetInternalCheckBoxStyle();
		protected AppearanceStyleBase GetItemControlStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			MergeDisableStyle(style);
			return style;
		}
		protected override bool IsWebSourcesRegisterRequired() {
			return true;
		}
		protected internal override string GetAssociatedControlID() {
			return ClientID + "_" + GetItemID(SelectedIndex == -1 ? 0 : SelectedIndex) + "_I";
		}
	}
	[DXWebToolboxItem(DXToolboxItemKind.Free),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabCommon),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxRadioButtonList.bmp"),
	ToolboxData("<{0}:ASPxRadioButtonList runat=\"server\" ValueType=\"System.String\"></{0}:ASPxRadioButtonList>")
	]
	public class ASPxRadioButtonList : ASPxCheckListBase {
		public ASPxRadioButtonList()
			: base() {
		}
		protected internal new RadioButtonListProperties Properties {
			get { return base.Properties as RadioButtonListProperties; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRadioButtonListCheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public InternalCheckBoxImageProperties CheckedImage {
			get { return Properties.CheckedImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRadioButtonListUncheckedImage"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Images"),
		AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty)]
		public InternalCheckBoxImageProperties UncheckedImage {
			get { return Properties.UncheckedImage; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRadioButtonListRadioButtonFocusedStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual  EditorDecorationStyle RadioButtonFocusedStyle {
			get { return Properties.RadioButtonFocusedStyle; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRadioButtonListRadioButtonStyle"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual EditorDecorationStyle RadioButtonStyle {
			get { return Properties.RadioButtonStyle; }
		}
		protected override object GetPostBackValue(string key, System.Collections.Specialized.NameValueCollection postCollection) {
			int selectedIndex = -1;
			string clientValue = postCollection[key];
			if(!string.IsNullOrEmpty(clientValue))
				selectedIndex = Int32.Parse(clientValue);
			SelectedIndex = selectedIndex;
			return Value;
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRadioButtonListAccessibilityCompliant"),
#endif
		Category("Accessibility"), DefaultValue(false), AutoFormatDisable]
		public bool AccessibilityCompliant {
			get { return AccessibilityCompliantInternal; }
			set { AccessibilityCompliantInternal = value; }
		}
		protected override EditPropertiesBase CreateProperties() {
			return new RadioButtonListProperties(this);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientRadioButtonList";
		}
		protected override List<InternalCheckBoxImageProperties> GetImages() {
			InternalCheckBoxImageProperties checkedImage = new InternalCheckBoxImageProperties();
			checkedImage.MergeWith(Properties.Images.RadioButtonChecked);
			checkedImage.MergeWith(Properties.Images.GetImageProperties(Page, EditorImages.RadioButtonCheckedImageName));
			InternalCheckBoxImageProperties uncheckedImage = new InternalCheckBoxImageProperties();
			uncheckedImage.MergeWith(Properties.Images.RadioButtonUnchecked);
			uncheckedImage.MergeWith(Properties.Images.GetImageProperties(Page, EditorImages.RadioButtonUncheckedImageName));
			return new List<InternalCheckBoxImageProperties>(new InternalCheckBoxImageProperties[] { checkedImage, uncheckedImage });	  
		}
		protected override AppearanceStyle GetDefaultEditStyle() {
			return RenderStyles.GetDefaultRadioButtonListStyle();
		}
		protected override AppearanceStyle GetEditStyleFromStylesStorage() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(RenderStyles.RadioButtonList);
			return style;
		}
		protected override AppearanceStyleBase GetInternalCheckBoxFocusedStyle(){
			return GetIRBFocusedStyle();
		}
		protected internal override AppearanceStyleBase GetInternalCheckBoxStyle(){
			return GetIRBStyle();
		}
		protected AppearanceStyleBase GetIRBFocusedStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultIRBFocusedClass());
			style.CopyFrom(RadioButtonFocusedStyle);
			return style;
		}
		protected AppearanceStyleBase GetIRBStyle() {
			AppearanceStyleBase style = new AppearanceStyleBase();
			style.CopyFrom(RenderStyles.GetDefaultIRBClass());
			style.CopyFrom(RadioButtonStyle);
			return style;
		}
		protected override ASPxCheckBox CreateItemControlCore(int index, ListEditItem item){
			return new RadioButtonListItemControl(this, item, index);			
		}
	}
}
