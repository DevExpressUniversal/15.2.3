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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI;
using System.Web.UI.Design;
using System.Web.UI.WebControls;
using DevExpress.Utils.Design;
using DevExpress.Web.ASPxHtmlEditor.Internal;
using DevExpress.Web.ASPxSpellChecker.Design;
using DevExpress.Web.Design;
using DevExpress.Web.Internal;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.XtraEditors.FeatureBrowser;
namespace DevExpress.Web.ASPxHtmlEditor.Design {
	public class ASPxHtmlEditorDesigner : ASPxWebControlDesigner {
		public static FormsInfo FormsInfo = new HtmlEditorFormsInfo(); 
		ASPxHtmlEditor htmlEditor = null;
		internal ASPxHtmlEditor HtmlEditor {
			get { return this.htmlEditor; }
		}
		public override void Initialize(IComponent component) {
			this.htmlEditor = (ASPxHtmlEditor)component;
			base.Initialize(component);
			RegisterTagPrefix(typeof(ASPxEditBase));
			RegisterTagPrefix(typeof(ASPxSpellChecker.ASPxSpellChecker));
		}
		protected override bool IsControlRequireHttpHandlerRegistration() {
			return true;
		}
		public override void EnsureControlReferences() {
			base.EnsureControlReferences();
			EnsureReferences(
				AssemblyInfo.SRAssemblySpellCheckerCore,
				AssemblyInfo.SRAssemblyHtmlEditorWeb,
				AssemblyInfo.SRAssemblySpellCheckerWeb,
				AssemblyInfo.SRAssemblyOfficeCore
			);
		}
		protected override Control CreateViewControl() {
			Control view = base.CreateViewControl();
			(view as ASPxHtmlEditor).ShowErrorFrame = HtmlEditor.ShowErrorFrame;
			return view;
		}
		protected override FormsInfo[] GetFormsInfoArray() {
			return new FormsInfo[] { FormsInfo, ASPxSpellCheckerDesigner.FormsInfo };
		}
		protected override bool NeedCopyFormsOnInitialize() {
			return false;
		}
		protected override Object GetControlSettingsForms(FormsInfo formsInfo) {
			if(formsInfo == FormsInfo)
				return HtmlEditor.SettingsForms;
			else if(formsInfo == ASPxSpellCheckerDesigner.FormsInfo)
				return HtmlEditor.SettingsForms.SpellCheckerForms;
			return base.GetControlSettingsForms(formsInfo);
		}
		protected override ASPxWebControlDesignerActionList CreateCommonActionList() {
			return new HtmlEditorDesignerActionList(this);
		}
		public override void RunDesigner() {
			ShowDialog(new WrapperEditorForm(new HtmlEditorCommonFormDesigner(HtmlEditor, DesignerHost)));
		}
		public override void ShowAbout() {
			HtmlEditorAboutDialogHelper.ShowAbout(Component.Site);
		}
		public HtmlEditorToolbarMode ToolbarMode {
			get { return HtmlEditor.ToolbarMode; }
			set {
				HtmlEditor.ToolbarMode = value;
				PropertyChanged("ToolbarMode");
			}
		}
		#region Designer regions
		protected override void AddDesignerRegions(DesignerRegionCollection regions) {
			ASPxHtmlEditor viewControl = (ASPxHtmlEditor)ViewControl;
			if(viewControl.HtmlEditorControl.StatusBar == null || viewControl.HtmlEditorControl.StatusBar.TabControl == null)
				return;
			StatusBarTabControl tabControl = viewControl.HtmlEditorControl.StatusBar.TabControl;
			int regionCount = regions.Count;
			for(int i = 0; i < tabControl.Tabs.Count; i++) {
				Tab tab = tabControl.Tabs[i];
				regions.Add(new ViewTabControlTabSelectableRegion(this, "Activate " + this.GetRegionName(tab), tab));
				WebControl tabItemControl = tabControl.FindControl(tabControl.GetTabElementID(tab, false)) as WebControl;
				tabItemControl.Attributes[DesignerRegion.DesignerRegionAttributeName] = (regionCount + i).ToString();
			}
		}
		protected string GetRegionName(TabBase tab) {
			if(tab.Text != "")
				return tab.Text;
			else if(tab.Name != "")
				return tab.Name;
			else
				return ("[Tab (" + tab.Index + ")]");
		}
		protected override void OnClick(DesignerRegionMouseEventArgs e) {
			base.OnClick(e);
			ViewTabControlTabSelectableRegion region = e.Region as ViewTabControlTabSelectableRegion;
			if(region != null) {
				HtmlEditor.ActiveView = ((ASPxHtmlEditor)ViewControl).HtmlEditorControl.StatusBar.GetModeByTab(region.Tab);
				PropertyChanged("ActiveView");
			}
		}
		public class ViewTabControlTabSelectableRegion : DesignerRegion {
			private Tab tab = null;
			public ViewTabControlTabSelectableRegion(ASPxHtmlEditorDesigner designer, string name, Tab tab)
				: base(designer, name, true) {
				this.tab = tab;
			}
			public Tab Tab {
				get { return tab; }
			}
		}
		#endregion
		#region Allow...View
		public bool AllowDesignView {
			get { return HtmlEditor.Settings.AllowDesignView; }
			set {
				HtmlEditor.Settings.AllowDesignView = value;
				PropertyChanged("Settings");
			}
		}
		public bool AllowHtmlView {
			get { return HtmlEditor.Settings.AllowHtmlView; }
			set {
				HtmlEditor.Settings.AllowHtmlView = value;
				PropertyChanged("Settings");
			}
		}
		public bool AllowPreview {
			get { return HtmlEditor.Settings.AllowPreview; }
			set {
				HtmlEditor.Settings.AllowPreview = value;
				PropertyChanged("Settings");
			}
		}
		protected internal bool ShowErrorFrame {
			get { return HtmlEditor.ShowErrorFrame; }
			set {
				HtmlEditor.ShowErrorFrame = value;
				UpdateDesignTimeHtml();
				TypeDescriptor.Refresh(Component);
			}
		}
		#endregion
		protected override void FillPropertyNameToCaptionMap(Dictionary<string, string> propertyNameToCaptionMap) {
			base.FillPropertyNameToCaptionMap(propertyNameToCaptionMap);
			propertyNameToCaptionMap.Add("CssFiles", "CssFiles");
			propertyNameToCaptionMap.Add("Toolbars", "Toolbars");
			propertyNameToCaptionMap.Add("Items", "Items");
			propertyNameToCaptionMap.Add("RibbonTabs", "Ribbon Tabs");
		}
		protected override void OnBeforeControlHierarchyCompleted() {
			LoadDefaultToolbars();
			LoadDefaultRibbonTabs();
		}
		private void LoadDefaultToolbars() {
			ASPxHtmlEditor editor = (ASPxHtmlEditor)ViewControl;
			if(editor.ToolbarMode == HtmlEditorToolbarMode.Menu && editor.Toolbars.Count == 0)
				editor.Toolbars.CreateDefaultToolbars(editor.IsRightToLeft());
		}
		private void LoadDefaultRibbonTabs() {
			ASPxHtmlEditor editor = (ASPxHtmlEditor)ViewControl;
			if(editor.ToolbarMode == HtmlEditorToolbarMode.Ribbon && editor.RibbonTabs.Count == 0)
				editor.RibbonTabs.CreateDefaultRibbonTabs();
		}
	}
	public class HtmlEditorDesignerActionList: ASPxWebControlDesignerActionList {
		private ASPxHtmlEditorDesigner designer = null;
		public HtmlEditorDesignerActionList(ASPxHtmlEditorDesigner designer)
			: base(designer) {
			this.designer = designer;
		}
		public new ASPxHtmlEditorDesigner Designer {
			get { return this.designer; }
		}
		public override DesignerActionItemCollection GetSortedActionItems() {
			var collection = base.GetSortedActionItems();
			var controlTypeName = typeof(ASPxHtmlEditor);
			collection.Add(new DesignerActionPropertyItem("ToolbarMode", "Toolbar Mode", "Toolbar", string.Empty));			
			if(ToolbarMode == HtmlEditorToolbarMode.ExternalRibbon) {
				collection.Add(new DesignerActionPropertyItem("AssociatedRibbonID", "Associated Ribbon ID", "Toolbar", string.Empty));
				if(!string.IsNullOrEmpty(AssociatedRibbonID)) {
					collection.Add(new DesignerActionMethodItem(this, "CreateDefaultRibbonTabs",
						"Create Default Ribbon Tabs",
						"Toolbar",
						string.Empty, true));
				}				
			}			
			collection.Add(new DesignerActionPropertyItem("AllowDesignView",
				StringResources.HtmlEditorActionList_DesignView,
				StringResources.HtmlEditorActionList_ChecksCategory,
				StringResources.HtmlEditorActionList_DesignViewDescription));
			collection.Add(new DesignerActionPropertyItem("AllowHtmlView",
				StringResources.HtmlEditorActionList_HtmlView,
				StringResources.HtmlEditorActionList_ChecksCategory,
				StringResources.HtmlEditorActionList_HtmlViewDescription));
			collection.Add(new DesignerActionPropertyItem("AllowPreview",
				StringResources.HtmlEditorActionList_Preview,
				StringResources.HtmlEditorActionList_ChecksCategory,
				StringResources.HtmlEditorActionList_PreviewDescription));
			collection.Add(new DesignerActionPropertyItem("ShowErrorFrame",
				"Preview Error Frame",
				"Previews"));
			collection.Add(new DesignerActionMethodItem(this, "OpenConfigureMaximumUploadLimitsHelp",
				StringResources.HtmlEditor_HowConfigureMaximumUploadLimitsActionItem,
				"Help",
				StringResources.ActionList_OpenHelpActionItemDescription, false));
			collection.Add(new DesignerActionMethodItem(this, "OpenHowToSetupSpellCheckerDictionaries", "How to setup SpellChecker dictionaries", true));
			return collection;
		}
		#region Allow...View
		public bool AllowDesignView {
			get { return Designer.AllowDesignView; }
			set { Designer.AllowDesignView = value; }
		}
		public bool AllowHtmlView {
			get { return Designer.AllowHtmlView; }
			set { Designer.AllowHtmlView = value; }
		}
		public bool AllowPreview {
			get { return Designer.AllowPreview; }
			set { Designer.AllowPreview = value; }
		}
		public bool ShowErrorFrame {
			get { return Designer.ShowErrorFrame; }
			set { Designer.ShowErrorFrame = value; }
		}
		#endregion
		protected void OpenConfigureMaximumUploadLimitsHelp() {
			ShowHelpFromUrl("#AspNet/CustomDocument9822");
		}
		public void OpenHowToSetupSpellCheckerDictionaries() {
			SpellCheckerHelper.OpenHowToSetupDictionaries();
		}
		protected void CreateDefaultRibbonTabs() {
			if(ToolbarMode == HtmlEditorToolbarMode.ExternalRibbon && !string.IsNullOrEmpty(Designer.HtmlEditor.AssociatedRibbonID))
				RibbonDesignerHelper.AddTabCollectionToRibbonControl(Designer.HtmlEditor.AssociatedRibbonID, new HtmlEditorDefaultRibbon(Designer.HtmlEditor).DefaultRibbonTabs, Designer.HtmlEditor);
		}
		public HtmlEditorToolbarMode ToolbarMode {
			get { return Designer.ToolbarMode; }
			set { Designer.ToolbarMode = value; }
		}
		[TypeConverterAttribute(typeof(RibbonControlIDConverter))]
		public string AssociatedRibbonID {
			get { return Designer.HtmlEditor.AssociatedRibbonID; }
			set {
				IComponent component = Designer.Component;
				TypeDescriptor.GetProperties(component)["AssociatedRibbonID"].SetValue(component, value);
			}
		}
	}
	public class HtmlEditorFormsInfo : FormsInfo {
		public override string ControlName { get {return "ASPxHtmlEditor";}}
		public override bool NeedCopyDesignerFileFromResource { get { return true; } }
		public override string[] FormNames { get {return ASPxHtmlEditor.UniqueFormNames;} }
		public override Type Type { get{return typeof(ASPxHtmlEditor);} }
	}
	public class HtmlEditorCommonFormDesigner : CommonFormDesigner {
		ItemsEditorOwner itemsOwner;
		ASPxHtmlEditor htmlEditor;
		const string DialogsGroupCaption = "Dialogs";
		public HtmlEditorCommonFormDesigner(ASPxHtmlEditor htmlEditor, IServiceProvider provider)
			: base(htmlEditor, provider) {
		}
		public override ItemsEditorOwner ItemsOwner {
			get {
				if(itemsOwner != null)
					return itemsOwner;
				if(HtmlEditor.ToolbarMode == HtmlEditorToolbarMode.Ribbon)
					itemsOwner = new HtmlEditorRibbonItemsOwner(HtmlEditor, Provider, HtmlEditor.RibbonTabs);
				else if(HtmlEditor.ToolbarMode == HtmlEditorToolbarMode.Menu)
					itemsOwner = new HtmlEditorToolbarItemsOwner(HtmlEditor, Provider);
				return itemsOwner;
			}
		}
		protected internal DesignerGroup DialogsGroup { get { return Groups[DialogsGroupCaption]; } }
		ASPxHtmlEditor HtmlEditor {
			get {
				if(htmlEditor == null)
					htmlEditor = (ASPxHtmlEditor)Control;
				return htmlEditor;
			}
		}
		protected override void CreateGroups() {
			base.CreateGroups();
			Groups.Add(DialogsGroupCaption, DialogsGroupCaption, GetDefaultGroupImage(SettingsGroupImageIndex), false);
			FillDialogsGroup();
		}
		protected override void CreateMainGroupItems() {
			CreateGeneralItem();
			CreateToolbarsItem();
			CreateContextMenuItemsItem();
			CreateCSSFilesItem();
			CreateShortcutsItem();
			CreateEditingSettingsItem();
			CreateSpellCheckingItem();
			CreatePlaceholdersItem();
			base.CreateClientSideEventsItem();
		}
		protected void CreateGeneralItem() {
			MainGroup.Add(CreateDesignerItem("General", "General", typeof(HtmlEditorGeneralFrame), HtmlEditor, ItemsItemImageIndex, new PresetsModel(HtmlEditor)));
		}
		protected void CreateSpellCheckingItem() {
			MainGroup.Add(CreateDesignerItem("Spell Checker", "Spell Checker", typeof(HtmlEditorSpellCheckingEmbeddedFrame), HtmlEditor, DictionariesImageIndex, null));
		}
		protected void CreateEditingSettingsItem() {
			MainGroup.Add(CreateDesignerItem("HTML Editing Settings", "HTML Editing Settings", typeof(HtmlEditorEditingSettingsEmbeddedFrame), HtmlEditor, HTMLEditingSettingsImageIndex, null));
		}
		protected void CreateToolbarsItem() {
			MainGroup.Add(CreateDesignerItem("Toolbars", "Toolbars", typeof(ToolbarEditorFrame), Control, ToolbarItemsImageIndex, new HtmlEditorToolbarInitializer(HtmlEditor, Provider)));
		}
		protected void CreateContextMenuItemsItem() {
			var contextMenuItemsOwner = new HtmlEditorContextMenuItemsOwner(HtmlEditor, Provider, HtmlEditor.ContextMenuItems);
			MainGroup.Add(CreateDesignerItem("ContextMenuItems", contextMenuItemsOwner.GetNavBarItemsGroupName(), typeof(HtmlEditorContextMenuEditorFrame), Control, MenuImageIndex, contextMenuItemsOwner));
		}
		protected void CreateCSSFilesItem() {
			MainGroup.Add(CreateDesignerItem("Css Files", "Css Files", typeof(HtmlEditorCssFilesEmbeddedFrame), HtmlEditor, CssFilesImageIndex, null));
		}
		protected void CreateShortcutsItem() {
			var shortcutsOwner = new ShortcutItemsOwner(HtmlEditor);
			MainGroup.Add(CreateDesignerItem("Shortcuts", shortcutsOwner.GetNavBarItemsGroupName(), typeof(ItemsEditorFrame), Control, ShortcutsImageIndex, shortcutsOwner));
		}
		protected void CreatePlaceholdersItem() {
			MainGroup.Add(CreateDesignerItem("Placeholders", "Placeholders", typeof(HtmlEditorPlaceholdersEmbeddedFrame), HtmlEditor, PlaceholdersImageIndex, null));
		}
		List<DesignerItem> dialogItems = new List<DesignerItem>();
		void FillDialogsGroup() {
			dialogItems.Clear();
			var customDialogsOwner = new FlatCollectionItemsOwner<HtmlEditorCustomDialog>(HtmlEditor, Provider, HtmlEditor.CustomDialogs, "Custom Dialogs");
			AddDialogItem("CustomDialogs", customDialogsOwner.GetNavBarItemsGroupName(), typeof(ItemsEditorFrame), CustomDialogsImageIndex, customDialogsOwner);
			AddDialogItem("Insert Flash", "Insert Flash", typeof(FeatureInsertFlashDialogFrame), InsertFlashDialogImageIndex);
			AddDialogItem("Insert HTML5 Video", "Insert HTML5 Video", typeof(FeatureInsertVideoDialogFrame), InsertVideoDialogImageIndex);
			AddDialogItem("Insert HTML5 Audio", "Insert HTML5 Audio", typeof(FeatureInsertAudioDialogFrame), InsertAudioDialogImageIndex);
			AddDialogItem("Insert Image", "Insert Image", typeof(FeatureInsertImageDialogFrame), InsertImageDialogImageIndex);
			AddDialogItem("Insert Link", "Insert Link", typeof(FeatureInsertLinkDialogFrame), InsertLinkDialogImageIndex);
			AddDialogItem("Insert Table", "Insert Table", typeof(FeatureInsertTableDialogFrame), InsertTableDialogImageIndex);
			AddDialogItem("Insert YouTube Video", "Insert YouTube Video", typeof(FeatureInsertYouTubeVideoDialogFrame), InsertYouTubeVideoDialogImageIndex);
			AddDialogItem("Paste From Word", "Paste From Word", typeof(FeatureInsertPasteFromWordDialogFrame), InsertPasteFromWordDialogImageIndex);
			AddDialogItem("Table Cell", "Table Cell", typeof(FeatureTableCellPropertiesDialogFrame), TableCellPropertiesDialogImageIndex);
			AddDialogItem("Table Column", "Table Column", typeof(FeatureTableColumnPropertiesDialogFrame), TableColumnPropertiesDialogImageIndex);
			AddDialogItem("Table Row", "Table Row", typeof(FeatureTableRowPropertiesDialogFrame), TableRowPropertiesDialogImageIndex);
			AddDialogItem("Change Element Properties", "Change Element Properties", typeof(FeatureChangeElementPropertiesDialogFrame), ChangeElementPropertiesDialogImageIndex);
			AddDialogItem("Spell Checking", "Spell Checking", typeof(FeatureCheckSpellingPropertiesDialogFrame), CheckSpellingDialogImageIndex);
			var orderedItems = dialogItems.OrderBy(i => i.Caption);
			foreach(var item in orderedItems)
				DialogsGroup.Add(item);
		}
		void AddDialogItem(string itemName, string itemCaption, Type editorFrameType, int imageIndex, object tag = null) {
			dialogItems.Add(CreateDesignerItem(itemName, itemCaption, editorFrameType, HtmlEditor, imageIndex, tag));
		}
		FlatCollectionItemsOwner<T> CreateOwner<T>(string title, Collection collection) where T : CollectionItem {
			return new FlatCollectionItemsOwner<T>(HtmlEditor, Provider, collection, title);
		}
		void AddDesignerItem(DesignerGroup group, string title, int index, FlatCollectionOwner owner) {
			group.Add(CreateDesignerItem(title, owner.GetNavBarItemsGroupName(), typeof(ItemsEditorFrame), Control, index, owner));
		}
		void AddDesignerItem(DesignerGroup group, string title, int index, Type editorFrameType) {
			group.Add(CreateDesignerItem(title, title, editorFrameType, Control, index, null));
		}
	}
	public class ShortcutItemsOwner : FlatCollectionItemsOwner<HtmlEditorShortcut> {
		ASPxHtmlEditor htmlEditor;
		public ShortcutItemsOwner(ASPxHtmlEditor component) 
			: base(component, component.Site, component.Shortcuts, "Shortcuts") {
		}
		public override string CreateDefaultItemsConfirmMessage { get { return "Do you want to delete all existing items before retrieving the editor's default item collection?"; } }
		public override string CreateDefaultItemsCaption { get { return string.Format("Create default items for '{1}'", HtmlEditor.ID); } }
		ASPxHtmlEditor HtmlEditor {
			get {
				if(htmlEditor == null)
					htmlEditor = (ASPxHtmlEditor)Component;
				return htmlEditor;
			}
		}
		protected override List<DesignEditorMenuRootItemActionType> GetToolbarActionTypes() {
			return new List<DesignEditorMenuRootItemActionType>() {
				DesignEditorMenuRootItemActionType.AddItem,
				DesignEditorMenuRootItemActionType.InsertBefore, 
				DesignEditorMenuRootItemActionType.Remove,
				DesignEditorMenuRootItemActionType.MoveUp, 
				DesignEditorMenuRootItemActionType.MoveDown,
				DesignEditorMenuRootItemActionType.CreateDefaultItems
			};
		}
		protected override DesignEditorDescriptorItem CreateDefaultItemsMenuItem() {
			var item = base.CreateDefaultItemsMenuItem();
			item.Enabled = true;
			return item;
		}
		public override void CreateDefaultItems(bool deleteExistingItems) {
			BeginUpdate();
			if(deleteExistingItems)
				HtmlEditor.Shortcuts.Clear();
			HtmlEditor.Shortcuts.CreateDefaultItems();
			EndUpdate();
		}
	}
}
