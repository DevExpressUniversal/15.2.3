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
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.Web.Localization;
namespace DevExpress.Web {
	[DXWebToolboxItem(true), DefaultProperty("Tabs"), DefaultEvent("ItemClick"),
	Designer("DevExpress.Web.Design.ASPxRibbonDesigner, " + AssemblyInfo.SRAssemblyWebDesignFull),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabNavigation),
	System.Drawing.ToolboxBitmap(typeof(ToolboxBitmapAccess), ToolboxBitmapAccess.BitmapPath + "ASPxRibbon.bmp")
]
	public class ASPxRibbon : ASPxHierarchicalDataWebControl, IParentSkinOwner, IControlDesigner, IRequiresLoadPostDataControl {
		protected internal const string RibbonScriptResourceName = WebScriptsResourcePath + "Ribbon.js";
		RibbonTabCollection tabs;
		RibbonContextTabCategoryCollection contextTabs;
		RibbonEditorStyles stylesEditors;
		RibbonTabControlStyles stylesTabControl;
		RibbonMenuStyles stylesPopupMenu;
		ITemplate fileTabTemplate;
		bool fDataBound = false;
		RibbonItemDataFields buttonItemDataFields;
		RibbonTabDataFields tabDataFields;
		RibbonGroupDataFields groupDataFields;
		RibbonSettingsPopupMenu settingPopupMenu;
		RibbonClientStateHelper clientStateHelper;
		static readonly object EventTabDataBound = new object();
		static readonly object EventItemDataBound = new object();
		static readonly object EventGroupDataBound = new object();
		static readonly object EventCommandExecuted = new object();
		static readonly object EventDialogBoxLauncherClicked = new object();
		internal const string ButtonItemTypeName = "Button",
							  OptionButtonItemTypeName = "OptionButton",
							  ToggleButtonItemTypeName = "ToggleButton",
							  ColorButtonItemTypeName = "ColorButton",
							  TextBoxItemTypeName = "TextBox",
							  SpinEditItemTypeName = "SpinEdit",
							  ComboBoxItemTypeName = "ComboBox",
							  CheckBoxItemTypeName = "CheckBox",
							  DropDownButtonItemTypeName = "DropDownButton",
							  DateEditItemTypeName = "DateEdit",
							  DropDownToggleButtonItemTypeName = "DropDownToggleButton",
							  GalleryDropDownItemTypeName = "GalleryDropDown",
							  GalleryBarItemTypeName = "GalleryBar";
		public ASPxRibbon() {
			stylesEditors = new RibbonEditorStyles(this);
			stylesTabControl = new RibbonTabControlStyles(this);
			stylesPopupMenu = new RibbonMenuStyles(this);
			EditorItems = new Dictionary<RibbonItemBase, ASPxEditBase>();
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonTabs"),
#endif
		PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public RibbonTabCollection Tabs
		{
			get
			{
				if (tabs == null)
					tabs = new RibbonTabCollection(this);
				return tabs;
			}
		}
		[PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		MergableProperty(false), AutoFormatDisable,
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public RibbonContextTabCategoryCollection ContextTabCategories {
			get {
				if(contextTabs == null)
					contextTabs = new RibbonContextTabCategoryCollection(this);
				return contextTabs;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override System.Web.UI.WebControls.Unit Height { get { return base.Height; } set { base.Height = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonStyles"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonStyles Styles
		{
			get { return (RibbonStyles)StylesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonStylesEditors"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonEditorStyles StylesEditors
		{
			get { return stylesEditors; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonStylesTabControl"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonTabControlStyles StylesTabControl
		{
			get { return stylesTabControl; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonStylesPopupMenu"),
#endif
		Category("Styles"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonMenuStyles StylesPopupMenu
		{
			get { return stylesPopupMenu; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonImages"),
#endif
		Category("Images"), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonImages Images
		{
			get { return (RibbonImages)ImagesInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonClientInstanceName"),
#endif
		Category("Client-Side"), DefaultValue(""), AutoFormatDisable, Localizable(false)]
		public string ClientInstanceName
		{
			get { return base.ClientInstanceNameInternal; }
			set { base.ClientInstanceNameInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonClientSideEvents"),
#endif
		Category("Client-Side"), PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		AutoFormatDisable, MergableProperty(false)]
		public RibbonClientSideEvents ClientSideEvents
		{
			get { return (RibbonClientSideEvents)base.ClientSideEventsInternal; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonAllowMinimize"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatEnable]
		public bool AllowMinimize
		{
			get { return GetBoolProperty("AllowMinimize", true); }
			set
			{
				SetBoolProperty("AllowMinimize", true, value);
				LayoutChanged();
			}
		}
		[Category("Behavior"), DefaultValue(false), AutoFormatEnable]
		public bool Minimized {
			get { return GetBoolProperty("Minimized", false); }
			set {
				SetBoolProperty("Minimized", false, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonActiveTabIndex"),
#endif
		Category("Behavior"), DefaultValue(0), AutoFormatDisable]
		public int ActiveTabIndex
		{
			get { return GetIntProperty("ActiveTabIndex", 0); }
			set
			{
				if (value == ActiveTabIndex) return;
				SetIntProperty("ActiveTabIndex", 0, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonShowFileTab"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool ShowFileTab
		{
			get { return GetBoolProperty("ShowFileTab", true); }
			set
			{
				if (value == ShowFileTab) return;
				SetBoolProperty("ShowFileTab", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonShowTabs"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool ShowTabs
		{
			get { return GetBoolProperty("ShowTabs", true); }
			set
			{
				if (value == ShowTabs) return;
				SetBoolProperty("ShowTabs", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonShowGroupLabels"),
#endif
		Category("Behavior"), DefaultValue(true), AutoFormatDisable]
		public bool ShowGroupLabels
		{
			get { return GetBoolProperty("ShowGroupLabels", true); }
			set
			{
				if (value == ShowGroupLabels) return;
				SetBoolProperty("ShowGroupLabels", true, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonSaveStateToCookies"),
#endif
		Category("Behavior"), DefaultValue(false), AutoFormatDisable]
		public new bool SaveStateToCookies
		{
			get { return base.SaveStateToCookies; }
			set { base.SaveStateToCookies = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonSaveStateToCookiesID"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), AutoFormatDisable]
		public new string SaveStateToCookiesID
		{
			get { return base.SaveStateToCookiesID; }
			set { base.SaveStateToCookiesID = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonClientVisible"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientVisible {
			get { return base.ClientVisibleInternal; }
			set { base.ClientVisibleInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonClientEnabled"),
#endif
		Category("Client-Side"), DefaultValue(true), AutoFormatDisable, Localizable(false)]
		public bool ClientEnabled
		{
			get { return base.ClientEnabledInternal; }
			set { base.ClientEnabledInternal = value; }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonTarget"),
#endif
		DefaultValue(""), Localizable(false), AutoFormatDisable]
		public string Target
		{
			get { return GetStringProperty("Target", ""); }
			set { SetStringProperty("Target", "", value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonTabDataFields"),
#endif
		Category("Data"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonTabDataFields TabDataFields
		{
			get
			{
				if (tabDataFields == null)
					tabDataFields = new RibbonTabDataFields(this);
				return tabDataFields;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonGroupDataFields"),
#endif
		Category("Data"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonGroupDataFields GroupDataFields
		{
			get
			{
				if (groupDataFields == null)
					groupDataFields = new RibbonGroupDataFields(this);
				return groupDataFields;
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonItemDataFields"),
#endif
		Category("Data"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonItemDataFields ItemDataFields
		{
			get
			{
				if (buttonItemDataFields == null)
					buttonItemDataFields = new RibbonItemDataFields(this);
				return buttonItemDataFields;
			}
		}
		[Category("Settings"), AutoFormatDisable, PersistenceMode(PersistenceMode.InnerProperty),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RibbonSettingsPopupMenu SettingsPopupMenu {
			get {
				if(settingPopupMenu == null)
					settingPopupMenu = new RibbonSettingsPopupMenu(this);
				return settingPopupMenu;
			}
		}
		[Category("Behavior"), DefaultValue(false), AutoFormatDisable, Localizable(false)]
		public bool OneLineMode {
			get { return GetBoolProperty("OneLineMode", false); }
			set { 
				SetBoolProperty("OneLineMode", false, value);
				LayoutChanged();
			}
		}
		[Browsable(false), DefaultValue(null), AutoFormatEnable, PersistenceMode(PersistenceMode.InnerProperty),
		TemplateContainer(typeof(DevExpress.Web.TabControlTemplateContainer)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ITemplate FileTabTemplate {
			get { return fileTabTemplate; }
			set {
				fileTabTemplate = value;
				TemplatesChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonFileTabText"),
#endif
		DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public string FileTabText
		{
			get { return GetStringProperty("FileTabText", string.Empty); }
			set
			{
				if (value == FileTabText) return;
				SetStringProperty("FileTabText", string.Empty, value);
				LayoutChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonTabDataBound"),
#endif
		Category("Data")]
		public event RibbonTabEventHandler TabDataBound
		{
			add { Events.AddHandler(EventTabDataBound, value); }
			remove { Events.RemoveHandler(EventTabDataBound, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonGroupDataBound"),
#endif
		Category("Data")]
		public event RibbonGroupEventHandler GroupDataBound
		{
			add { Events.AddHandler(EventGroupDataBound, value); }
			remove { Events.RemoveHandler(EventGroupDataBound, value); }
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("ASPxRibbonItemDataBound"),
#endif
		Category("Data")]
		public event RibbonItemEventHandler ItemDataBound
		{
			add { Events.AddHandler(EventItemDataBound, value); }
			remove { Events.RemoveHandler(EventItemDataBound, value); }
		}
#if !SL
	[DevExpressWebLocalizedDescription("ASPxRibbonClientLayout")]
#endif
		public event ASPxClientLayoutHandler ClientLayout {
			add { Events.AddHandler(EventClientLayout, value); }
			remove { Events.RemoveHandler(EventClientLayout, value); }
		}
		public event RibbonCommandExecutedEventHandler CommandExecuted {
			add { Events.AddHandler(EventCommandExecuted, value); }
			remove { Events.RemoveHandler(EventCommandExecuted, value); }
		}
		public event RibbonDialogBoxLauncherClickedEventHandler DialogBoxLauncherClicked {
			add { Events.AddHandler(EventDialogBoxLauncherClicked, value); }
			remove { Events.RemoveHandler(EventDialogBoxLauncherClicked, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), UrlProperty,
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssFilePath {
			get { return base.CssFilePath; }
			set { base.CssFilePath = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string CssPostfix {
			get { return base.CssPostfix; }
			set { base.CssPostfix = value; }
		}
		protected internal RibbonControl RibbonControl { get; private set; }
		protected internal Dictionary<RibbonItemBase, ASPxEditBase> EditorItems { get; private set; }
		protected RibbonClientStateHelper ClientStateHelper {
			get {
				if(clientStateHelper == null)
					clientStateHelper = new RibbonClientStateHelper(this);
				return clientStateHelper;
			}
		}
		protected internal List<RibbonTab> AllTabs {
			get {
				var allTabs = new List<RibbonTab>();
				allTabs.AddRange(Tabs);
				allTabs.AddRange(ContextTabCategories.SelectMany(category => category.Tabs));
				return allTabs;
			}
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			RibbonControl = DesignMode ? new RibbonControlDesignTime(this) : new RibbonControl(this);
			Controls.Add(RibbonControl);
		}
		protected override void ClearControlFields() {
			base.ClearControlFields();
			RibbonControl = null;
		}
		public override void RegisterStyleSheets() {
			base.RegisterStyleSheets();
			if(Images.IconSet == MenuIconSetType.NotSet) {
				foreach(string key in RegisterRibbonIconsHelper.RegisteredRibbonSpriteCssFiles.Values) {
					string spriteCssFile = GetCustomSpriteCssFilePath();
					if(!string.IsNullOrEmpty(spriteCssFile) && spriteCssFile.IndexOf(ImagesBase.SpriteImageName + ".css") > 0)
						ResourceManager.RegisterCssResource(Page, spriteCssFile.Replace(ImagesBase.SpriteImageName + ".css", key + ".css"));
					else if(string.IsNullOrEmpty(GetCssPostFix()))
						ResourceManager.RegisterCssResource(Page, typeof(ASPxRibbon), WebCssResourcePath + key + ".css");
				}
			}
			RegisterRibbonIconsHelper.RegisterResources(Page);
		}
		protected override void GetCreateClientObjectScript(StringBuilder stb, string localVarName, string clientName) {
			base.GetCreateClientObjectScript(stb, localVarName, clientName);
			if(!ShowTabs)
				stb.AppendLine(string.Format("{0}.showTabs = false;", localVarName));
			if(!ShowFileTab)
				stb.AppendLine(string.Format("{0}.showFileTab = false;", localVarName));
			if(AllowMinimize && Minimized)
				stb.AppendLine(string.Format("{0}.minimized = true;", localVarName));
			if(OneLineMode)
				stb.AppendLine(string.Format("{0}.oneLineMode = true;", localVarName));
			stb.AppendLine(string.Format("{0}.loadItems({1});", localVarName, HtmlConvertor.ToJSON(RibbonHelper.GetClientItems(AllTabs))));
			if(ActiveTabIndex != 0)
				stb.AppendLine(string.Format("{0}.activeTabIndex = {1};", localVarName, ActiveTabIndex));
			if(!AllowMinimize)
				stb.AppendLine(string.Format("{0}.allowMinimize = false;", localVarName));
		}
		protected override Hashtable GetClientObjectState() {
			return ClientStateHelper.GetClientState();
		}
		protected override void GetClientObjectAssignedServerEvents(List<string> eventNames) {
			if(!HasEvents())
				return;
			if(Events[EventCommandExecuted] != null)
				eventNames.Add("CommandExecuted");
			if(Events[EventDialogBoxLauncherClicked] != null)
				eventNames.Add("DialogBoxLauncherClicked");
		}
		protected override void RegisterIncludeScripts() {
			base.RegisterIncludeScripts();
			RegisterIncludeScript(typeof(ASPxRibbon), RibbonScriptResourceName);
		}
		protected override string GetClientObjectClassName() {
			return "ASPxClientRibbon";
		}
		protected override bool HasClientInitialization() {
			return true;
		}
		protected override bool HasFunctionalityScripts() {
			return true;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new RibbonClientSideEvents();
		}
		protected override bool HasHoverScripts() {
			return true;
		}
		protected override bool HasPressedScripts() {
			return true;
		}
		protected override bool HasSelectedScripts() {
			return true;
		}
		protected override bool HasDisabledScripts() {
			return true;
		}
		protected override bool HasContent() {
			return DesignMode || AllTabs.Any();
		}
		protected override bool IsScriptEnabled() {
			return true;
		}
		protected override void AddHoverItems(StateScriptRenderHelper helper) {
			foreach(RibbonTab tab in AllTabs) {
				foreach(RibbonGroup group in tab.Groups) {
					if(group.ShowDialogBoxLauncher) {
						helper.AddStyle(Styles.GetGroupDialogBoxLauncherHoverStyle(),
							RibbonHelper.GetGroupDialogBoxLauncherID(group),
							new string[0],
							Images.GetDialogBoxLauncherImageProperties().GetHottrackedScriptObject(Page),
							"I",
							IsEnabled()
						);
					}
					if(OneLineMode)
						helper.AddStyle(Styles.GetOneLineModeGroupExpandButtonHoverStyle(), 
							RibbonHelper.GetOneLineModeGroupExpandButtonID(group), 
							new string[0],
							RibbonHelper.GetExpandGroupButtonImageObjects(group, Page),
							new string[]{ RibbonHelper.OneLineModeGroupExpandButtonImagePostfix, RibbonHelper.OneLineModeGroupExpandButtonPopOutImagePostfix },
							IsEnabled());
					else
						helper.AddStyle(Styles.GetGroupExpandButtonHoverStyle(),
							RibbonHelper.GetGroupExpandButtonID(group),
							new string[0],
							group.GetImage().GetHottrackedScriptObject(Page),
							"I",
							IsEnabled()
					);
					foreach(RibbonItemBase item in group.Items) {
						if(item is RibbonGalleryDropDownItem) {
							var dropDownGallery = (RibbonGalleryDropDownItem)item;
							AddDropDownGalleryHoverItems(helper, dropDownGallery);
						}
						if(item is RibbonGalleryBarItem) {
							var ribbonGalleryBar = (RibbonGalleryBarItem)item;
							AddGalleryBarHoverItems(helper, ribbonGalleryBar);
						}
						if(item is RibbonButtonItem || item.IsButtonMode()) {
							helper.AddStyle(Styles.GetButtonItemHoverStyle(item),
								RibbonHelper.GetItemID(group, item),
								new string[0],
								RibbonHelper.GetHoverItemImageObjects(item, Page),
								RibbonHelper.GetItemImagePrefixes(item),
								IsEnabled()
							);
						}
					}
				}
			}
			if(AllowMinimize) {
				helper.AddStyle(Styles.GetMinimizeButtonHoverStyle(),
					RibbonHelper.GetMinimizeButtonID(this),
					new string[0],
					Images.GetMinimizeButtonImageProperties().GetHottrackedScriptObject(Page),
					RibbonMinimizeButtonTemplate.ImageID,
					IsEnabled()
				);
			}
		}
		private void AddDropDownGalleryHoverItems(StateScriptRenderHelper helper, RibbonItemBase gallery) {
			var galleryItems = RibbonHelper.GetRibbonGalleriesItems(gallery);
			foreach(var galleryItem in galleryItems) {
				helper.AddStyle(Styles.GetItemHoverStyle(gallery),
				string.Format("{0}_{1}", RibbonHelper.GetPopupGalleryID(gallery), RibbonHelper.GetGalleryDropDownItemID(galleryItem)),
				new string[0],
				IsEnabled());
			}
		}
		private void AddGalleryBarHoverItems(StateScriptRenderHelper helper, RibbonGalleryBarItem ribbonGalleryBar) {
			AddDropDownGalleryHoverItems(helper, ribbonGalleryBar);
			IEnumerable<RibbonGalleryItem> galleryItems = ribbonGalleryBar.GetAllItems();
			foreach(var galleryItem in galleryItems) {
				helper.AddStyle(Styles.GetItemHoverStyle(ribbonGalleryBar),
				RibbonHelper.GetGalleryBarItemID(galleryItem),
				new string[0],
				IsEnabled());
			}
			helper.AddStyle(Styles.GetGalleryButtonHoverStyle(ribbonGalleryBar), RibbonHelper.GetGalleryUpButtonID(ribbonGalleryBar), new string[0],
				Images.GetGalleryUpButtonImageProperties().GetHottrackedScriptObject(Page),
				RibbonHelper.GetGalleryBarImagePrefixes()[0], IsEnabled());
			helper.AddStyle(Styles.GetGalleryButtonHoverStyle(ribbonGalleryBar), RibbonHelper.GetGalleryDownButtonID(ribbonGalleryBar), new string[0],
				Images.GetGalleryDownButtonImageProperties().GetHottrackedScriptObject(Page),
				RibbonHelper.GetGalleryBarImagePrefixes()[1], IsEnabled());
			helper.AddStyle(Styles.GetGalleryButtonHoverStyle(ribbonGalleryBar), RibbonHelper.GetGalleryPopOutButtonID(ribbonGalleryBar), new string[0],
				Images.GetGalleryPopOutButtonImageProperties().GetHottrackedScriptObject(Page),
				RibbonHelper.GetGalleryBarImagePrefixes()[2], IsEnabled());
		}
		private void AddGalleryBarPressedItems(StateScriptRenderHelper helper, RibbonGalleryBarItem ribbonGalleryBar) {
			AddDropDownGalleryHoverItems(helper, ribbonGalleryBar);
			IEnumerable<RibbonGalleryItem> galleryItems = ribbonGalleryBar.GetAllItems();
			foreach(var galleryItem in galleryItems) {
				helper.AddStyle(Styles.GetItemPressedStyle(ribbonGalleryBar),
				RibbonHelper.GetGalleryBarItemID(galleryItem),
				new string[0],
				IsEnabled());
			}
			helper.AddStyle(Styles.GetGalleryButtonPressedStyle(ribbonGalleryBar), RibbonHelper.GetGalleryUpButtonID(ribbonGalleryBar), new string[0], IsEnabled());
			helper.AddStyle(Styles.GetGalleryButtonPressedStyle(ribbonGalleryBar), RibbonHelper.GetGalleryDownButtonID(ribbonGalleryBar), new string[0], IsEnabled());
			helper.AddStyle(Styles.GetGalleryButtonPressedStyle(ribbonGalleryBar), RibbonHelper.GetGalleryPopOutButtonID(ribbonGalleryBar), new string[0], IsEnabled());
		}
		protected override void AddDisabledItems(StateScriptRenderHelper helper) {
			foreach(RibbonTab tab in AllTabs) {
				foreach(RibbonGroup group in tab.Groups) {
					if(group.ShowDialogBoxLauncher) {
						helper.AddStyle(Styles.GetGroupDialogBoxLauncherDisabledStyle(),
							RibbonHelper.GetGroupDialogBoxLauncherID(group),
							new string[0],
							Images.GetDialogBoxLauncherImageProperties().GetDisabledScriptObject(Page),
							"I",
							IsEnabled()
						);
					}
					if(OneLineMode)
						helper.AddStyle(Styles.GetOneLineModeGroupExpandButtonDisabledStyle(),
							RibbonHelper.GetOneLineModeGroupExpandButtonID(group),
							new string[0],
							group.OneLineModeSettings.Image.GetDisabledScriptObject(Page),
							RibbonHelper.OneLineModeGroupExpandButtonImagePostfix,
							true);
					else
						helper.AddStyle(Styles.GetGroupExpandButtonDisabledStyle(),
							RibbonHelper.GetGroupExpandButtonID(group),
							new string[0],
							group.GetImage().GetDisabledScriptObject(Page),
							"I",
							true
					);
					foreach(RibbonItemBase item in group.Items) {
						if(item is RibbonButtonItem || item.IsButtonMode()) {
							helper.AddStyle(
								Styles.GetButtonItemDisabledStyle(item),
								RibbonHelper.GetItemID(group, item),
								new string[0],
								RibbonHelper.GetDisabledItemImageObjects(item, Page),
								RibbonHelper.GetItemImagePrefixes(item),
								IsEnabled()
							);
						} else {
							if(item is RibbonGalleryBarItem)
								AddGalleryBarDisabledItems(helper, item);
							helper.AddStyle(
								Styles.GetItemDisabledStyle(),
								RibbonHelper.GetItemID(group, item),
								new string[0],
								IsEnabled()
							);
						}
					}
				}
			}
			if(AllowMinimize) {
				helper.AddStyle(Styles.GetMinimizeButtonDisabledStyle(),
					RibbonHelper.GetMinimizeButtonID(this),
					new string[0],
					Images.GetMinimizeButtonImageProperties().GetDisabledScriptObject(Page),
					RibbonMinimizeButtonTemplate.ImageID,
					IsEnabled()
				);
			}
		}
		private void AddGalleryBarDisabledItems(StateScriptRenderHelper helper, RibbonItemBase gallery) {
			var ribbonGalleryBar = (RibbonGalleryBarItem)gallery;
			helper.AddStyle(Styles.GetItemDisabledStyle(), RibbonHelper.GetGalleryUpButtonID(ribbonGalleryBar), new string[0],
				Images.GetGalleryUpButtonImageProperties().GetDisabledScriptObject(Page), RibbonHelper.GetGalleryBarImagePrefixes()[0], IsEnabled());
			helper.AddStyle(Styles.GetItemDisabledStyle(), RibbonHelper.GetGalleryDownButtonID(ribbonGalleryBar), new string[0],
				Images.GetGalleryDownButtonImageProperties().GetDisabledScriptObject(Page), RibbonHelper.GetGalleryBarImagePrefixes()[1], IsEnabled());
			helper.AddStyle(Styles.GetItemDisabledStyle(), RibbonHelper.GetGalleryPopOutButtonID(ribbonGalleryBar), new string[0],
				Images.GetGalleryPopOutButtonImageProperties().GetDisabledScriptObject(Page), RibbonHelper.GetGalleryBarImagePrefixes()[2], IsEnabled());
			IEnumerable<RibbonGalleryItem> galleryItems = ribbonGalleryBar.GetAllItems();
			foreach(var galleryItem in galleryItems) {
				helper.AddStyle(Styles.GetItemDisabledStyle(),
				RibbonHelper.GetGalleryBarItemID(galleryItem),
				new string[0],
				IsEnabled());
			}
		}
		protected override void AddPressedItems(StateScriptRenderHelper helper) {
			foreach(RibbonTab tab in AllTabs) {
				foreach(RibbonGroup group in tab.Groups) {
					if(group.ShowDialogBoxLauncher) {
						helper.AddStyle(Styles.GetGroupDialogBoxLauncherPressedStyle(),
							RibbonHelper.GetGroupDialogBoxLauncherID(group),
							new string[0],
							Images.GetDialogBoxLauncherImageProperties().GetPressedScriptObject(Page),
							"I",
							IsEnabled()
						);
					}
					if(OneLineMode) {
						helper.AddStyle(Styles.GetOneLineModeGroupExpandButtonPressedStyle(),
							RibbonHelper.GetOneLineModeGroupExpandButtonID(group),
							new string[0],
							RibbonHelper.GetExpandGroupButtonImageObjects(group, Page),
							new string[] { RibbonHelper.OneLineModeGroupExpandButtonImagePostfix, RibbonHelper.OneLineModeGroupExpandButtonPopOutImagePostfix },
							IsEnabled());
					} else {
						helper.AddStyle(Styles.GetGroupExpandButtonPressedStyle(),
						RibbonHelper.GetGroupExpandButtonID(group),
						new string[0],
						group.GetImage().GetPressedScriptObject(Page),
						"I",
						IsEnabled()
					);
					}
					foreach(RibbonItemBase item in group.Items) {
						if(item is RibbonButtonItem || item.IsButtonMode()) {
							helper.AddStyle(
								Styles.GetButtonItemPressedStyle(item), 
								RibbonHelper.GetItemID(group, item),
								new string[0],
								RibbonHelper.GetPressedItemImageObjects(item, Page),
								RibbonHelper.GetItemImagePrefixes(item),
								IsEnabled()
							);
						} else if(item is RibbonGalleryBarItem) {
							var ribbonGalleryBar = (RibbonGalleryBarItem)item;
							AddGalleryBarPressedItems(helper, ribbonGalleryBar);
						}
					}
				}
			}
			if(AllowMinimize) {
				helper.AddStyle(Styles.GetMinimizeButtonPressedStyle(),
					RibbonHelper.GetMinimizeButtonID(this),
					new string[0],
					Images.GetMinimizeButtonImageProperties().GetPressedScriptObject(Page),
					RibbonMinimizeButtonTemplate.ImageID,
					IsEnabled()
				);
			}
		}
		protected override void AddSelectedItems(StateScriptRenderHelper helper) {
			foreach(RibbonTab tab in AllTabs) {
				foreach(RibbonGroup group in tab.Groups) {
					foreach(RibbonItemBase item in group.Items) {
						if(item is RibbonToggleButtonItem || item is RibbonDropDownToggleButtonItem) {
							var btnItem = (RibbonButtonItem)item;
							helper.AddStyle(
								Styles.GetButtonItemCheckedStyle(btnItem), 
								RibbonHelper.GetItemID(group, item),
								new string[0],
								RibbonHelper.GetSelectedItemImageObjects(btnItem, Page),
								RibbonHelper.GetItemImagePrefixes(btnItem),
								IsEnabled()
							);
						}
						if(item is RibbonGalleryDropDownItem) {
							var dropDownGallery = (RibbonGalleryDropDownItem)item;
							AddDropDownGallerySelectedItems(helper, dropDownGallery);
						}
						if(item is RibbonGalleryBarItem) {
							var ribbonGalleryBar = (RibbonGalleryBarItem)item;
							AddGalleryBarSelectedItems(helper, ribbonGalleryBar);
						}
					}
				}
			}
			if(AllowMinimize) {
				helper.AddStyle(Styles.GetMinimizeButtonCheckedStyle(),
					RibbonHelper.GetMinimizeButtonID(this),
					new string[0],
					Images.GetMinimizeButtonImageProperties().GetCheckedScriptObject(Page),
					RibbonMinimizeButtonTemplate.ImageID,
					IsEnabled()
				);
			}
		}
		private void AddDropDownGallerySelectedItems(StateScriptRenderHelper helper, RibbonItemBase gallery) {
			var galleryItems = RibbonHelper.GetRibbonGalleriesItems(gallery);
			foreach(var galleryItem in galleryItems) {
				helper.AddStyle(Styles.GetGalleryItemCheckedStyle(gallery),
				string.Format("{0}_{1}", RibbonHelper.GetPopupGalleryID(gallery), RibbonHelper.GetGalleryDropDownItemID(galleryItem)),
				new string[0],
				IsEnabled());
			}
		}
		private void AddGalleryBarSelectedItems(StateScriptRenderHelper helper, RibbonGalleryBarItem ribbonGalleryBar) {
			AddDropDownGallerySelectedItems(helper, ribbonGalleryBar);
			IEnumerable<RibbonGalleryItem> galleryItems = ribbonGalleryBar.GetAllItems();
			foreach(var galleryItem in galleryItems) {
				helper.AddStyle(Styles.GetGalleryItemCheckedStyle(ribbonGalleryBar),
				RibbonHelper.GetGalleryBarItemID(galleryItem),
				new string[0],
				IsEnabled());
			}
		}
		protected override StylesBase CreateStyles() {
			return new RibbonStyles(this);
		}
		protected override ImagesBase CreateImages() {
			return new RibbonImages(this);
		}
		protected override bool LoadPostData(System.Collections.Specialized.NameValueCollection postCollection) {
			if(ClientObjectState == null) return false;
			ClientStateHelper.SyncClientState(ClientObjectState);
			return false;
		}
		protected internal override void LoadClientState(string state) {
			ClientStateHelper.SyncClientStateString(state);
		}
		protected internal override string SaveClientState() {
			return ClientStateHelper.GetClientStateString();
		}
		protected override bool NeedLoadClientState() {
			return string.IsNullOrEmpty(Request.Form[GetClientObjectStateInputID()]);
		}
		protected internal void SetCheckedState(RibbonOptionButtonItem optionItem) {
			foreach(RibbonTab tab in this.AllTabs) {
				foreach(RibbonGroup group in tab.Groups) {
					foreach(RibbonItemBase item in group.Items) {
						if(item.ItemType == RibbonItemType.OptionButton && ((RibbonOptionButtonItem)item).OptionGroupName == optionItem.OptionGroupName && item != optionItem)
							((RibbonOptionButtonItem)item).Checked = false;
					}
				}
			}
		}
		protected override void RaisePostBackEvent(string eventArgument) {
			EnsureDataBound();
			string[] arguments = eventArgument.Split(new char[] { ':' });
			string[] indexes = arguments[1].Split(RenderUtils.IndexSeparator);
			switch(arguments[0]) {
				case "COMMANDEXECUTED":
					RibbonItemBase item = GetItemByIndexes(indexes);
					var parameter = arguments[2] == "null" ? string.Empty : arguments[2];
					RaiseCommandExecuted(new RibbonCommandExecutedEventArgs(item, parameter));
					break;
				case "DIALOGBOXLAUNCHERCLICKED":
					RaiseDialogBoxLauncherClicked(new DialogBoxLauncherClickedEventArgs(GetGroupByIndexes(indexes)));
					break;
			}
		}
		protected internal RibbonGroup GetGroupByIndexes(string[] indexes) {
			int tabIndex = int.Parse(indexes[0]);
			if(0 <= tabIndex && tabIndex < AllTabs.Count) {
				RibbonTab tab = AllTabs[tabIndex];
				int groupIndex = int.Parse(indexes[1]);
				if(0 <= groupIndex && groupIndex < tab.Groups.Count) {
					return tab.Groups[groupIndex];
				}
			}
			return null;
		}
		protected internal RibbonItemBase GetItemByIndexes(string[] indexes) {
			RibbonGroup group = GetGroupByIndexes(indexes);
			if(group != null) {
				int itemIndex = int.Parse(indexes[2]);
				if(0 <= itemIndex && itemIndex < group.Items.Count) {
					RibbonItemBase item = group.Items[itemIndex];
					if(indexes.Length > 3 && item as RibbonDropDownButtonItem != null) {
						var ddItem = (RibbonDropDownButtonItem)item;
						for(int i = 3; i < indexes.Length; i++) {
							int ddItemIdex = int.Parse(indexes[i]);
							if(0 <= ddItemIdex && ddItemIdex < ddItem.Items.Count)
								ddItem = ddItem.Items[ddItemIdex];
							else
								return null;
						}
						return ddItem;
					} else
						return item;
				}
			}
			return null;
		}
		protected internal RibbonItemBase FindItemByName(string name) {
			var item = Tabs.FindItemByName(name);
			if(item == null)
				foreach(RibbonContextTabCategory category in ContextTabCategories) {
					item = category.Tabs.FindItemByName(name);
					if(item != null)
						return item;
				}
			return item;
		}
		protected virtual RibbonItemBase RaiseItemDataBound(RibbonItemEventArgs e) {
			RibbonItemEventHandler handler = (RibbonItemEventHandler)Events[EventItemDataBound];
			if(handler != null)
				handler(this, e);
			return e.Item;
		}
		protected virtual void RaiseTabDataBound(RibbonTabEventArgs e) {
			RibbonTabEventHandler handler = (RibbonTabEventHandler)Events[EventTabDataBound];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void RaiseGroupDataBound(RibbonGroupEventArgs e) {
			RibbonGroupEventHandler handler = (RibbonGroupEventHandler)Events[EventGroupDataBound];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void RaiseCommandExecuted(RibbonCommandExecutedEventArgs e) {
			RibbonCommandExecutedEventHandler handler = (RibbonCommandExecutedEventHandler)Events[EventCommandExecuted];
			if(handler != null)
				handler(this, e);
		}
		protected virtual void RaiseDialogBoxLauncherClicked(DialogBoxLauncherClickedEventArgs e) {
			RibbonDialogBoxLauncherClickedEventHandler handler = (RibbonDialogBoxLauncherClickedEventHandler)Events[EventDialogBoxLauncherClicked];
			if(handler != null)
				handler(this, e);
		}
		protected internal override void PerformDataBinding(string dataHelperName) {
			if(!DesignMode && fDataBound && string.IsNullOrEmpty(DataSourceID) && (DataSource == null))
				Tabs.Clear();
			else if(!string.IsNullOrEmpty(DataSourceID) || (DataSource != null)) {
				DataBindTabs();
				ResetControlHierarchy();
			}
		}
		protected internal void DataBindTabs() {
			HierarchicalDataSourceView view = GetData("");
			if(view != null) {
				IHierarchicalEnumerable enumerable = view.Select();
				Tabs.Clear();
				if(enumerable != null) {
					foreach(object obj in enumerable) {
						IHierarchyData data = enumerable.GetHierarchyData(obj);
						RibbonTab tab = new RibbonTab();
						PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
						var categoryName = string.Empty;
						DataUtils.GetPropertyValue<string>(obj, TabDataFields.ContextTabCategoryNameField, props, value => { categoryName = value; });
						if(!string.IsNullOrWhiteSpace(categoryName)) {
							var tabCategory = ContextTabCategories.FirstOrDefault(category => category.Name == categoryName) ?? new RibbonContextTabCategory(categoryName);
							ContextTabCategories.Add(tabCategory);
							DataUtils.GetPropertyValue<string>(obj, TabDataFields.ColorField, props, value => { tabCategory.Color = System.Drawing.ColorTranslator.FromHtml(value); });
							tabCategory.Tabs.Add(tab);
						} else
							Tabs.Add(tab);
						DataBindTabProperties(tab, obj);
						if(data.HasChildren)
							DataBindGroups(tab, data.GetChildren());
						tab.DataPath = data.Path;
						tab.DataItem = obj;
						RaiseTabDataBound(new RibbonTabEventArgs(tab));
					}
				}
			}
		}
		protected internal void DataBindGroups(RibbonTab tab, IHierarchicalEnumerable enumerable) {
			foreach(object obj in enumerable) {
				IHierarchyData data = enumerable.GetHierarchyData(obj);
				RibbonGroup group = new RibbonGroup();
				tab.Groups.Add(group);
				DataBindGroupProperties(group, obj);
				if(data.HasChildren)
					DataBindItems(group, data.GetChildren());
				group.DataPath = data.Path;
				group.DataItem = obj;
				RaiseGroupDataBound(new RibbonGroupEventArgs(group));
			}
		}
		protected internal void DataBindItems(RibbonGroup group, IHierarchicalEnumerable enumerable) {
			foreach(object obj in enumerable) {
				IHierarchyData data = enumerable.GetHierarchyData(obj);
				RibbonItemBase item = CreateBindItem(obj);
				if(item == null) continue;
				DataBindItemProperties(item, obj);
				item.DataPath = data.Path;
				item.DataItem = obj;
				item = RaiseItemDataBound(new RibbonItemEventArgs(item));
				if(item == null) continue;
				group.Items.Add(item);
				if(data.HasChildren)
					DataBindItems(item, data.GetChildren());
			}
		}
		protected internal void DataBindItems(RibbonItemBase parent, IHierarchicalEnumerable enumerable) {
			if(parent is RibbonComboBoxItem) {
				foreach(object obj in enumerable) {
					ListEditItem item = new ListEditItem();
					if(item != null && item is ListEditItem) {
						PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
						if(props == null)
							continue;
						DataUtils.GetPropertyValue<string>(obj, "Value", props, value => { item.Value = value; });
						DataUtils.GetPropertyValue<string>(obj, "Text", props, value => { item.Text = value; });
						DataUtils.GetPropertyValue<string>(obj, "ImageUrl", props, value => { item.ImageUrl = value; });
						((RibbonComboBoxItem)parent).Items.Add(item);
					}
				}
			}
			if(parent is RibbonDropDownButtonItem) {
				foreach(object obj in enumerable) {
					IHierarchyData data = enumerable.GetHierarchyData(obj);
					RibbonItemBase item = CreateBindItem(obj);
					if(item != null && item is RibbonDropDownButtonItem) {
						DataBindItemProperties(item, obj);
						item.DataPath = data.Path;
						item.DataItem = obj;
						item = RaiseItemDataBound(new RibbonItemEventArgs(item));
						if(item == null)
							continue;
						((RibbonDropDownButtonItem)parent).Items.Add(item);
						if(data.HasChildren)
							DataBindItems((RibbonDropDownButtonItem)item, data.GetChildren());
					}
				}
			}			
		}
		protected internal bool HasContextTabs() {
			return ContextTabCategories.SelectMany(category => category.Tabs).Any();
		}
		void DataBindTabProperties(RibbonTab tab, object obj) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
			if(props == null)
				return;
			DataUtils.GetPropertyValue<string>(obj, TabDataFields.NameField, props, value => { tab.Name = value; });
			DataUtils.GetPropertyValue<string>(obj, TabDataFields.TextField, props, value => { tab.Text = value; });
		}
		void DataBindGroupProperties(RibbonGroup group, object obj) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
			if(props == null)
				return;
			DataUtils.GetPropertyValue<string>(obj, GroupDataFields.NameField, props, value => { group.Name = value; });
			DataUtils.GetPropertyValue<string>(obj, GroupDataFields.TextField, props, value => { group.Text = value; });
			DataUtils.GetPropertyValue<string>(obj, GroupDataFields.ImageUrlField, props, value => { group.Image.Url = value; });
			DataUtils.GetPropertyValue<bool>(obj, GroupDataFields.ShowDialogBoxLauncherField, props, value => { group.ShowDialogBoxLauncher = value; });
		}
		void DataBindItemProperties(RibbonItemBase item, object obj) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
			if(props == null)
				return;
			DataUtils.GetPropertyValue<string>(obj, ItemDataFields.NameField, props, value => { item.Name = value; });
			DataUtils.GetPropertyValue<string>(obj, ItemDataFields.TextField, props, value => { item.Text = value; });
			DataUtils.GetPropertyValue<string>(obj, ItemDataFields.ToolTipField, props, value => { item.ToolTip = value; });
			DataUtils.GetPropertyValue<bool>(obj, ItemDataFields.BeginGroupField, props, value => { item.BeginGroup = value; });
			var buttonItem = item as RibbonButtonItem;
			if(buttonItem != null) {
				DataUtils.GetPropertyValue<string>(obj, ItemDataFields.SmallImageUrlField, props, value => { buttonItem.SmallImage.Url = value; });
				DataUtils.GetPropertyValue<string>(obj, ItemDataFields.LargeImageUrlField, props, value => { buttonItem.LargeImage.Url = value; });
				DataUtils.GetPropertyValue<string>(obj, ItemDataFields.SizeField, props, value => { buttonItem.Size = (RibbonItemSize)Enum.Parse(typeof(RibbonItemSize), value); });
				if(buttonItem.GetType().Equals(typeof(RibbonButtonItem)) || buttonItem.GetType().Equals(typeof(RibbonDropDownButtonItem)))
					DataUtils.GetPropertyValue<string>(obj, ItemDataFields.NavigateUrlField, props, value => { buttonItem.NavigateUrl = value; });
				if(buttonItem.GetType().Equals(typeof(RibbonOptionButtonItem)))
					DataUtils.GetPropertyValue<string>(obj, ItemDataFields.OptionGroupNameField, props, value => { (buttonItem as RibbonOptionButtonItem).OptionGroupName = value;});
			}
		}
		internal RibbonItemBase CreateBindItem(object obj) {
			PropertyDescriptorCollection props = TypeDescriptor.GetProperties(obj);
			if(props == null)
				return null;
			string itemTypeName = ButtonItemTypeName;
			DataUtils.GetPropertyValue<string>(obj, ItemDataFields.ItemTypeField, props, value => { itemTypeName = value; });
			if(itemTypeName == null)
				return new RibbonButtonItem();
			if(itemTypeName.Equals(ButtonItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonButtonItem();
			if(itemTypeName.Equals(CheckBoxItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonCheckBoxItem();
			if(itemTypeName.Equals(ColorButtonItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonColorButtonItem();
			if(itemTypeName.Equals(ComboBoxItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonComboBoxItem();
			if(itemTypeName.Equals(DropDownButtonItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonDropDownButtonItem();
			if(itemTypeName.Equals(DropDownToggleButtonItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonDropDownToggleButtonItem();
			if(itemTypeName.Equals(OptionButtonItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonOptionButtonItem();
			if(itemTypeName.Equals(SpinEditItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonSpinEditItem();
			if(itemTypeName.Equals(TextBoxItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonTextBoxItem();
			if(itemTypeName.Equals(ToggleButtonItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonToggleButtonItem();
			if(itemTypeName.Equals(DateEditItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonDateEditItem();
			if(itemTypeName.Equals(GalleryDropDownItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonGalleryDropDownItem();
			if(itemTypeName.Equals(GalleryBarItemTypeName, StringComparison.InvariantCultureIgnoreCase))
				return new RibbonGalleryBarItem();
			return new RibbonButtonItem();
		}
		protected internal void OnDataFieldChangedInternal() {
			OnDataFieldChanged();
		}
		protected internal string GetFileTabText() {
			if(!string.IsNullOrEmpty(FileTabText)) return FileTabText;
			return HtmlEncode(ASPxperienceLocalizer.GetString(ASPxperienceStringId.Ribbon_FileTabText));
		}
		protected override void LoadViewState(object savedState) {
			base.LoadViewState(savedState);
			fDataBound = true;
		}
		protected override System.Web.UI.IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { 
					Tabs, ContextTabCategories, StylesTabControl, StylesEditors, StylesPopupMenu,
					ItemDataFields, TabDataFields, GroupDataFields
				});
		}
		public Control FindFileTabTemplateControl(string id) {
			if(!ShowFileTab) return null;
			if(DataSource != null || !string.IsNullOrEmpty(DataSourceID))
				DataBind();
			else
				EnsureChildControls();
			return RibbonControl.TabControl.Tabs[0].FindControl(id);
		}
		string IControlDesigner.DesignerType { get { return "DevExpress.Web.Design.RibbonItemsOwner"; } }
	}
}
