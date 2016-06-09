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
using System.ComponentModel.Design;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.About;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.XtraNavBar.ViewInfo;
using Microsoft.Win32;
namespace DevExpress.Web.Design {
	public class WrapperEditorForm : BaseDesignerForm {
		ISupportInitialize panelFrameInitializer;
		CommonFormDesigner activeDesigner;
		ASPxWebControl editingControl;
		DevExpress.XtraEditors.PanelControl headerPanel;
		DefaultBoolean isCreateBottomPanel = DefaultBoolean.Default;
		bool showNavBar = true;
		int navBarlinkCaptionLeft = 0;
		string windowCaption = null;
		const string RegistryOfficeStyleOptionPath = "Software\\Developer Express\\Designer\\";
		public WrapperEditorForm(object activeObject)
			: this(activeObject, true) {
		}
		public WrapperEditorForm(object activeObject, string windowCaption)
			: this(activeObject, true) {
			this.windowCaption = windowCaption;
			Text = windowCaption;
		}
		public WrapperEditorForm(object activeObject, bool navBarVisible) {
			showNavBar = navBarVisible;
			if(activeObject is CommonFormDesigner)
				InitializeByActiveDesigner((CommonFormDesigner)activeObject);
			else if(activeObject is ItemsEditorOwner)
				InitializeByItemsOwner((ItemsEditorOwner)activeObject);
		}
		public WrapperEditorForm(CommonFormDesigner activeDesigner) {
			InitializeByActiveDesigner(activeDesigner);
		}
		public WrapperEditorForm(ItemsEditorOwner itemsOwner) {
			InitializeByItemsOwner(itemsOwner);
		}
		void InitializeByActiveDesigner(CommonFormDesigner activeDesigner) { 
			ActiveDesigner = activeDesigner;
			ItemsOwner = activeDesigner.ItemsOwner;
			InitializeForm();
		}
		void InitializeByItemsOwner(ItemsEditorOwner itemsOwner) { 
			ItemsOwner = itemsOwner;
			InitializeForm();
		}
		public override bool CanUseOfficeStyle() {
			return GetRegKeyBoolValue(RegistryOfficeStyleOptionPath, "UseExploreStyleInControlDesigner_Web", true);
		}
		public void NavigateToNavBarItem(string itemCaption) {
			var item = NavBar.Items.FirstOrDefault(i => i.Caption == itemCaption);
			if(item != null)
				NavBar.SelectedLink = item.Links.First();
		}
		protected override bool CanUseDefaultControlDesignersSkin() {
			return GetRegKeyBoolValue(RegistryOfficeStyleOptionPath, "UseDefaultControlDesignersSkin_Web", true);
		}
		bool GetRegKeyBoolValue(string path, string regKey, bool defaultValue) {
			using(var subKey = Registry.CurrentUser.OpenSubKey(path)) {
				if(subKey == null)
					return defaultValue;
				var value = subKey.GetValue(regKey);
				return value != null ? Convert.ToBoolean(value) : defaultValue;
			}
		}
		ISupportInitialize PanelFrameInitializer {
			get {
				if(panelFrameInitializer == null)
					panelFrameInitializer = (ISupportInitialize)PanelFrame;
				return panelFrameInitializer;
			}
		}
		protected internal DevExpress.XtraEditors.PanelControl PanelFrame { get { return this.pnlFrame; } }
		DevExpress.XtraEditors.PanelControl BottomPanel { get; set; }
		protected SimpleButton ButtonOk { get; set; }
		protected SimpleButton ButtonCancel { get; set; }
		ItemsEditorOwner ItemsOwner { get; set; }
		bool NavBarWidthUpdated { get; set; }
		CommonFormDesigner CommonDesigner {
			get {
				if(activeDesigner == null)
					activeDesigner = (CommonFormDesigner)ActiveDesigner;
				return activeDesigner;
			}
		}
		ASPxWebControl EditingControl {
			get {
				if(editingControl == null)
					editingControl = ItemsOwner != null ? (ASPxWebControl)ItemsOwner.Component : CommonDesigner.Control;
				return editingControl;
			}
		}
		DevExpress.XtraEditors.PanelControl HeaderPanel {
			get {
				if(headerPanel == null) {
					headerPanel = DesignTimeFormHelper.CreatePanel(pnlFrame, "HeaderPanel", DockStyle.Top);
					headerPanel.Height = 44;
				}
				return headerPanel;
			}
		}
		bool IsCreateBottomPanel {
			get {
				if(isCreateBottomPanel == DefaultBoolean.Default)
					return true;
				return isCreateBottomPanel == DefaultBoolean.True;
			}
			set { isCreateBottomPanel = value ? DefaultBoolean.True : DefaultBoolean.False; }
		}
		protected override Size DefaultMinimumSize { get { return new Size(770, 600); } }
		void InitializeForm() {
			if(CommonDesigner != null)
				CommonDesignerServiceRegisterHelper.AddRootWrapperEditorFormObject(CommonDesigner.Provider, this);
			var editingObjectType = EditingControl.GetType().Name;
			PanelFrameInitializer.BeginInit();
			SuspendLayout();
			ClientSize = new System.Drawing.Size(800, 600);
			Name = "WrapperEditorForm";
			Text = windowCaption == null ? string.Format("{0} Designer", editingObjectType) : windowCaption;
			PanelFrameInitializer.EndInit();
			CreateBottomPanel();
			InitEditingObject(EditingControl, string.Format("{0} Designer", editingObjectType));
			InitializeNavBar();
			Load += (s, e) => { UpdateMinimumSize(); };
			KeyPreview = true;
			ResumeLayout(false);
		}
		protected internal void UpdateMinimumSize() {
			if(PanelFrame == null || XF == null || XF.MinimumSize.IsEmpty) {
				MinimumSize = DefaultMinimumSize;
				return;
			}
			var panelFrameDeltaFormWidth = Width - (PanelFrame.Location.X + PanelFrame.Width) + PanelFrame.Parent.Padding.Right;
			var panelFrameDeltaFormHeight = Height - (PanelFrame.Location.Y + PanelFrame.Height) + PanelFrame.Parent.Padding.Bottom;
			var minWidth = PanelFrame.Location.X + XF.MinimumSize.Width + PanelFrame.Margin.Horizontal + PanelFrame.Padding.Horizontal + panelFrameDeltaFormWidth;
			var minHeight = PanelFrame.Location.Y + XF.MinimumSize.Height + PanelFrame.Margin.Vertical + PanelFrame.Padding.Vertical + panelFrameDeltaFormHeight;
			if(minWidth < MinimumSize.Width)
				minWidth = MinimumSize.Width;
			if(minHeight < MinimumSize.Height)
				minHeight = MinimumSize.Height;
			MinimumSize = new Size(minWidth, minHeight);
		}
		void CreateBottomPanel() {
			if(!IsCreateBottomPanel)
				return;
			BottomPanel = DesignTimeFormHelper.CreatePanel(this, "BottomPanel", DockStyle.Bottom);
			BottomPanel.Height = 40;
			BottomPanel.TabIndex = 4;
			var buttonSize = new Size(80, 23);
			var top = (BottomPanel.Height - buttonSize.Height) / 2;
			var location = new Point(BottomPanel.Width - (buttonSize.Width + 17) * 2, top);
			ButtonOk = DesignTimeFormHelper.CreateButton(BottomPanel, buttonSize, location, 0, "&OK", DialogResult.OK, OnOK);
			location.X += buttonSize.Width + 17;
			ButtonCancel = DesignTimeFormHelper.CreateButton(BottomPanel, buttonSize, location, 1, "&Cancel", DialogResult.Cancel, OnCancel);
			ButtonCancel = DesignTimeFormHelper.CreateButton(BottomPanel, buttonSize, location, 1, "&Cancel", DialogResult.Cancel, OnCancel);
		}
		protected void OnOK(object sender, EventArgs e) {
			CommonDesigner.SaveData();
		}
		bool cancelClicked;
		protected void OnCancel(object sender, EventArgs e) {
			cancelClicked = true;
			CommonDesigner.UndoData();
			CommonDesigner.ClearResources();
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			if(e.KeyCode == Keys.Escape) {
				var escapeService = CommonDesignerServiceRegisterHelper.GetEscapeBtnUpService(CommonDesigner.Provider);
				if(escapeService == null || !escapeService())
					Close();
			}
			base.OnKeyUp(e);
		}
		protected override void OnFormClosing(FormClosingEventArgs e) {
			base.OnFormClosing(e);
			if(e.CloseReason == CloseReason.UserClosing && !cancelClicked)
				e.Cancel = BeforeCloseForm();
			if(!e.Cancel)
				CommonDesigner.ClearResources();
		}
		bool BeforeCloseForm() {
			CommonDesigner.BeforeClosed();
			if(CommonDesigner.HasChanges) {
				var questionResult = MessageBox.Show("Would you like to save changed data?", "Save data", MessageBoxButtons.YesNoCancel);
				if(questionResult == DialogResult.No)
					CommonDesigner.UndoData();
				else if(questionResult == DialogResult.Yes)
					CommonDesigner.SaveData();
				return questionResult == DialogResult.Cancel;
			}
			return false;
		}
		void InitializeNavBar() {
			NavBar.OptionsNavPane.ShowGroupImageInHeader = true;
			NavBar.OptionsNavPane.ShowOverflowPanel = false;
			NavBar.OptionsNavPane.ShowSplitter = false;
			SetNavBarSelectedLink();			
			NavBar.CustomDrawLink += NavBar_CustomDrawLink;
			NavBar.LinkPressed += (s, e) => { SendSaveFrameChanges(); };
			NavBar.SelectedLinkChanged += (s, e) => { UpdateMinimumSize(); };
			NavBar.Visible = showNavBar && (NavBar.Items.Count > 1);			
		}
		void UpdateNavBarWidth() {
			int width = 0;
			Font groupFont = null;
			foreach(DevExpress.XtraNavBar.NavBarGroup group in NavBar.Groups) {
				if(groupFont == null)
					groupFont = new Font(group.Appearance.Font.FontFamily, group.Appearance.Font.Size, group.Appearance.Font.Style | FontStyle.Bold);
				width = Math.Max(TextRenderer.MeasureText(group.Caption, groupFont).Width, width);
			}
			foreach(DevExpress.XtraNavBar.NavBarItem item in NavBar.Items)
				width = Math.Max(TextRenderer.MeasureText(item.Caption, item.Appearance.Font).Width, width);
			NavBar.BeginUpdate();
			NavBar.Width = width + 40 + navBarlinkCaptionLeft;			
			NavBar.EndUpdate();
			NavBarWidthUpdated = true;
		}		
		void NavBar_CustomDrawLink(object sender, CustomDrawNavBarElementEventArgs e) {
			if(NavBarWidthUpdated)
				return;
			var linkInfo = e.ObjectInfo as NavLinkInfoArgs;			
			navBarlinkCaptionLeft = linkInfo.CaptionRectangle.X;
			UpdateNavBarWidth();			
		}
		void SetNavBarSelectedLink() {
			var activeDesignerItem = CommonDesigner.GetActiveDesignerItem();
			if(activeDesignerItem == null)
				return;
			foreach(DevExpress.XtraNavBar.NavBarItem item in NavBar.Items) {
				if(item.Tag == activeDesignerItem) {
					var selectedLink = item.Links.First();
					selectedLink.Group.Expanded = true;
					NavBar.SelectedLink = selectedLink;
					break;
				}
			}
		}
		protected override void OnClosing(CancelEventArgs e) {
			SendSaveFrameChanges();
			base.OnClosing(e);
		}
		void SendSaveFrameChanges() {
			foreach(var frame in PanelFrame.Controls) {
				var saveChangesframe = frame as ISaveChangesFrame;
				if(saveChangesframe != null)
					saveChangesframe.SaveChanges();
			}
		}
		protected override void CreateDesigner() {
			if(ActiveDesigner == null)
				ActiveDesigner = new CommonFormDesigner(ItemsOwner);
		}
	}
	public class CommonFormDesigner : BaseDesigner {
		protected const string ClientSideEventsCaption = "Client-Side Events";
		protected const string SummaryGroupCaption = "Summary";
		DesignerGroup mainGroup;
		Dictionary<IOwnerEditingProperty, DesignerItem> designerItems;
		Dictionary<Type, DesignerItem> featureBrowserDesignerItems;
		ImageCollection groupImages;
		ImageCollection smallImages;
		int itemsImageIndex = -1;
		IServiceProvider controlServiceProvider;
		public const int MainGroupImageIndex = 0;
		public const int SummaryGroupImageIndex = 1;
		public const int SettingsGroupImageIndex = 2;
		public const int ColumnsItemImageIndex = 0;
		public const int GroupSummaryItemImageIndex = 1;
		public const int TotalSummaryItemImageIndex = 2;
		public const int ClientSideEventsItemImageIndex = 3;
		public const int StartItemImageIndex = 4;
		public const int LayoutItemsImageIndex = 5;
		public const int ItemsItemImageIndex = 6;
		public const int RankItemImageIndex = 7;
		public const int ButtonsItemImageIndex = 8;
		public const int MenuImageIndex = 9;
		public const int TabPagesImageIndex = 10;
		public const int RibbonItemsImageIndex = 11;
		public const int AccessRulesImageIndex = 12;
		public const int CssFilesImageIndex = 13;
		public const int CustomDialogsImageIndex = 14;
		public const int DetailsViewSettingsImageIndex = 15;
		public const int DictionariesImageIndex = 16;
		public const int ForbiddenZonesImageIndex = 17;
		public const int InsertClassCssItemsImageIndex = 18;
		public const int ShortcutsImageIndex = 19;
		public const int ToolbarItemsImageIndex = 20;
		public const int WindowsImageIndex = 21;
		public const int ResizingImageIndex = 22;
		public const int FeatureBrowserItemImageIndex = 23;
		public const int InsertFlashDialogImageIndex = 24;
		public const int InsertVideoDialogImageIndex = 25;
		public const int InsertAudioDialogImageIndex = 26;
		public const int InsertImageDialogImageIndex = 27;
		public const int InsertLinkDialogImageIndex = 28;
		public const int InsertTableDialogImageIndex = 29;
		public const int InsertYouTubeVideoDialogImageIndex = 30;
		public const int InsertPasteFromWordDialogImageIndex = 31;
		public const int TableCellPropertiesDialogImageIndex = 32;
		public const int TableColumnPropertiesDialogImageIndex = 33;
		public const int TableRowPropertiesDialogImageIndex = 34;
		public const int CheckSpellingDialogImageIndex = 35;
		public const int HTMLEditingSettingsImageIndex = 36;
		public const int PlaceholdersImageIndex = 37;
		public const int ChangeElementPropertiesDialogImageIndex = 38;
		public CommonFormDesigner(ItemsEditorOwner itemsOwner)
			: this((ASPxWebControl)itemsOwner.Component, itemsOwner.ServiceProvider) {
			ItemsOwner = itemsOwner;
			ItemsImageIndex = ItemsItemImageIndex;
		}
		public CommonFormDesigner(ASPxWebControl control, IServiceProvider provider)
			: base() {
			Control = control;
			Provider = provider;
			RegisterPropertyValuesSaver();
		}
		public virtual bool HasChanges { get { return CommonDesignerUndoHelper.IsPropertiesModified(ControlServiceProvider) || DesignerItems.Keys.ToList().Exists(i => ((IOwnerEditingProperty)i).ItemsChanged); } }
		public int ItemsImageIndex {
			get {
				if(itemsImageIndex == -1)
					itemsImageIndex = ItemsItemImageIndex;
				return itemsImageIndex;
			}
			set {
				itemsImageIndex = value;
			}
		}
		protected internal DesignerGroup MainGroup {
			get {
				if(mainGroup == null)
					mainGroup = Groups[NavBarMainGroupName];
				return mainGroup;
			}
		}
		protected virtual Type DefaultItemsFrameType { get { return typeof(ItemsEditorFrame); } }
		public string NavBarMainGroupName { get { return "Main"; } }
		public string NavBarItemsGroupName { get { return "Items"; } }
		public string NavBarItemsGroupDescription { get { return string.Format("Manage and create {0}.", NavBarItemsGroupName); } }
		public string NavBarClientSideEventsGroupName { get { return "Client-Side Events"; } }
		public string NavBarMainGroupNameDescription { get { return string.Format("{0} settings ({1}, Client-Side Events).", NavBarMainGroupName, NavBarItemsGroupName); } }
		public virtual ItemsEditorOwner ItemsOwner { get; private set; }
		public ASPxWebControl Control { get; private set; }
		public IServiceProvider Provider { get; set; }
		public Dictionary<IOwnerEditingProperty, DesignerItem> DesignerItems {
			get {
				if(designerItems == null) {
					designerItems = new Dictionary<IOwnerEditingProperty, DesignerItem>();
					CommonDesignerServiceRegisterHelper.AddOwnerEditingItemsObjectService(ControlServiceProvider, designerItems);
				}
				return designerItems;
			}
		}
		public Dictionary<Type, DesignerItem> FeatureBrowserDesignerItems {
			get {
				if(featureBrowserDesignerItems == null)
					featureBrowserDesignerItems = new Dictionary<Type, DesignerItem>();
				return featureBrowserDesignerItems;
			}
		}
		protected virtual bool CanCreateItemsGroup { get { return ItemsOwner != null; } }
		protected override object GroupImageList {
			get {
				if(groupImages == null)
					groupImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.Web.Design.Images.NavBarGroupIcons.png", GetType().Assembly, new Size(16, 16));
				return groupImages;
			}
		}
		protected override object SmallImageList {
			get {
				if(smallImages == null)
					smallImages = ImageHelper.CreateImageCollectionFromResources("DevExpress.Web.Design.Images.NavBarItemsIcons.png", GetType().Assembly, new Size(16, 16));
				return smallImages;
			}
		}
		IServiceProvider ControlServiceProvider {
			get {
				if(controlServiceProvider == null && Control != null)
					controlServiceProvider = Control.Site;
				return controlServiceProvider;
			}
		}
		public void ClearResources() {
			CommonDesignerServiceRegisterHelper.RemoveDiscoveredPropertyService(ControlServiceProvider);
		}
		void RegisterPropertyValuesSaver() {
			CommonDesignerServiceRegisterHelper.AddPropertyValueSaverObjectService(ControlServiceProvider);
		}
		bool ListContainsOrEqual(IList items, object obj) {
			if(items == null)
				return false;
			var list = new ArrayList(items);
			if(list.Contains(obj) || items == obj)
				return true;
			foreach(var item in list) {
				var childItems = item is IDesignTimeCollectionItem ? (item as IDesignTimeCollectionItem).Items : item as IList;
				if(ListContainsOrEqual(childItems, obj))
					return true;
			}
			return false;
		}
		public DesignerItem GetActiveDesignerItem() {
			var typeDescriptor = Provider as ITypeDescriptorContext;
			if(typeDescriptor == null)
				return null;
			var key = FindActiveKey(typeDescriptor);
			if(key != null)
				return DesignerItems[key];
			return DiscoverPropertyInFeatureItems(typeDescriptor);
		}
		DesignerItem DiscoverPropertyInFeatureItems(ITypeDescriptorContext typeDescriptor) {
			var discoverService = new FeatureBrowserPropertyDiscover(Control);
			var propertyPath = discoverService.GetPathToProperty(typeDescriptor);
			DesignerItem result = null;
			foreach(var type in FeatureBrowserDesignerItems.Keys) {
				if(discoverService.FindFeatureItemByPropertyName(propertyPath, type)) {
					result = FeatureBrowserDesignerItems[type];
					break;
				}
			}
			if(result != null)
				CommonDesignerServiceRegisterHelper.SetDiscoveredPropertyService(ControlServiceProvider, propertyPath);
			return result;
		}
		IOwnerEditingProperty FindActiveKey(ITypeDescriptorContext typeDescriptor) {
			var propertyValue = typeDescriptor.PropertyDescriptor.GetValue(typeDescriptor.Instance);
			if(propertyValue == null)
				return null;
			var key = DesignerItems.Keys.FirstOrDefault(i => i.PropertyInstance == propertyValue || i.PropertyInstance == typeDescriptor.Instance);
			if(key == null)
				key = DesignerItems.Keys.Where(i => (i.PropertyInstance is IList)).FirstOrDefault(o => ListContainsOrEqual(o.PropertyInstance as IList, propertyValue));
			return key;
		}
		public void BeforeClosed() {
			IterateByPropertyOwners(o => o.BeforeClosed());
		}
		public void UndoData() {
			CommonDesignerUndoHelper.UndoPropertyGridChangedValues(ControlServiceProvider);
			IterateByPropertyOwners(o => {
				if(o.ItemsChanged)
					o.UndoChanges();
			});
		}
		public void SaveData() {
			IterateByPropertyOwners(o => {
				if(o.ItemsChanged)
					o.SaveChanges();
			});
		}
		void IterateByPropertyOwners(Action<IOwnerEditingProperty> action) {
			foreach(var key in DesignerItems.Keys) {
				var owner = key as IOwnerEditingProperty;
				if(owner != null)
					action(owner);
			}
		}
		protected internal ImageCollection GetSmallImageList() {
			return SmallImageList as ImageCollection;
		}
		protected override void CreateGroups() {
			Groups.Clear();
			Groups.Add(NavBarMainGroupName, NavBarMainGroupNameDescription, GetDefaultGroupImage(MainGroupImageIndex), true);
			CreateMainGroupItems();
		}
		protected virtual void CreateMainGroupItems() { 
			CreateItemsItem();
			CreateClientSideEventsItem();
		}
		protected virtual void CreateItemsItem() {
			if(CanCreateItemsGroup)
				MainGroup.Add(CreateDesignerItem(ItemsOwner, DefaultItemsFrameType, ItemsImageIndex));
		}
		protected virtual void CreateClientSideEventsItem() {
			var properties = Control.GetType().GetProperties();
			foreach(var property in properties) {
				if(property.Name == "ClientSideEvents") {
					var eventsHelper = new ClientSideEventsOwner(Control, Provider);
					MainGroup.Add(CreateDesignerItem(property.Name, ClientSideEventsCaption, typeof(ClientSideEventsFrame), Control, ClientSideEventsItemImageIndex, eventsHelper));
					return;
				}
			}
		}
		protected DesignerItem CreateDesignerItem(ItemsEditorOwner itemsOwner, Type frameType, int imageIndex) {
			return CreateDesignerItem(itemsOwner.CollectionPropertyName, itemsOwner.GetNavBarItemsGroupName(), frameType, itemsOwner.Component, imageIndex, itemsOwner);
		}
		protected DesignerItem CreateDesignerItem(string propertyName, string caption, Type frameType, object component, int imageIndex, object tag) {
			return CreateDesignerItem(propertyName, caption, GetDesignItemEditDescription(component, caption), frameType, component, imageIndex, tag);
		}
		protected DesignerItem CreateDesignerItem(string propertyName, string caption, string description, Type frameType, object component, int imageIndex, object tag) {
			var designerItem = new DesignerItem(caption, description, frameType, GetDefaultLargeImage(imageIndex), GetDefaultSmallImage(imageIndex), tag);
			AddEditingProperty(tag as IOwnerEditingProperty, designerItem);
			AddFeatureBrowserDescription(frameType, designerItem);
			return designerItem;
		}
		private void AddFeatureBrowserDescription(Type featureFrameType, DesignerItem designerItem) {
			if(!typeof(FeatureBrowserMainFrameWeb).IsAssignableFrom(featureFrameType))
				return;
			if(featureFrameType != null)
				FeatureBrowserDesignerItems[featureFrameType] = designerItem;
		}
		void AddEditingProperty(IOwnerEditingProperty editingProperty, DesignerItem designerItem) {
			if(editingProperty != null)
				DesignerItems[editingProperty] = designerItem;
		}
		protected virtual string GetDesignItemEditDescription(object Component, string itemName) {
			return string.Format("Edit {0} {1}.", Component.GetType().Name, itemName);
		}
	}
	public class CommonDesignerEditor : TypeEditorBase {
		public override Form CreateEditorForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue) {
			var instance = context.Instance;
			var clientSideEvents = GetClientSideEventsForEdit(component, instance, propertyValue);
			if(clientSideEvents != null)
				return CreateClientSideEventsEditorForm(component, provider, clientSideEvents);
			return CreateCommonDesignerForm(component, context, provider, propertyValue);
		}
		protected Form CreateCommonDesignerForm(object component, ITypeDescriptorContext context, IServiceProvider provider, object propertyValue) {
			var controlDesigner = component as IControlDesigner;
			var componentType = controlDesigner != null ? Type.GetType(controlDesigner.DesignerType) : typeof(CommonFormDesigner);
			return new WrapperEditorForm(Activator.CreateInstance(componentType, component, provider));
		}
		protected Form CreateClientSideEventsEditorForm(object component, IServiceProvider provider, ClientSideEventsBase clientSideEvents) {
			var form = new WrapperEditorForm(new ClientSideEventsCommonFormDesigner(component, provider, clientSideEvents));
			form.Text = String.Format("Client-Side Events Editor");
			return form;
		}
		protected ClientSideEventsBase GetClientSideEventsForEdit(object component, object instance, object propertyValue) {
			if(instance is ClientSideEventsBase || propertyValue is ClientSideEventsBase) {
				var clientSideEvents = instance is ClientSideEventsBase ? (ClientSideEventsBase)instance : (ClientSideEventsBase)propertyValue;
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
				if(!properties.OfType<PropertyDescriptor>().Any(i => i.GetValue(component) == clientSideEvents))
					return clientSideEvents;
			}
			return null;
		}
	}
	public class CommonDesignerParametersService {
		public CommonDesignerParametersService() {
			Parameters = new Dictionary<string, object>();
		}
		Dictionary<string, object> Parameters { get; set; }
		public void AddParameter(string name, object value) {
			Parameters[name] = value;
		}
		public object GetParameter(string name) {
			return Parameters.ContainsKey(name) ? Parameters[name] : null;
		}
		public void RemoveParameter(string name) {
			if(Parameters.ContainsKey(name))
				Parameters.Remove(name);
		}
	}
	public class CommonDesignerServiceRegisterHelper {
		const string OwnerEditingItemsServiceName = "OwnerEditingItems";
		const string PropertyValuesSaverServiceName = "PropertyValuesSaver";
		const string RootWrapperEditorFormServiceName = "RootWrapperEditorForm";
		const string DiscoveredPropertyServiceName = "DiscoveredProperty";
		const string EscapeBtnUpServiceServiceName = "EscapeBtnUpService";
		const string ControlDesignerPrefix = "ControlDesigner_";
		public delegate bool OnEscapeKeyUp();
		public static void AddOwnerEditingItemsObjectService(IServiceProvider provider, Dictionary<IOwnerEditingProperty, DesignerItem> items) {
			AddParameterObjectToService(provider, OwnerEditingItemsServiceName, items);
		}
		public static void AddPropertyValueSaverObjectService(IServiceProvider provider) {
			AddParameterObjectToService(provider, PropertyValuesSaverServiceName, new Dictionary<object, Dictionary<string, object>>());
		}
		public static void AddRootWrapperEditorFormObject(IServiceProvider provider, WrapperEditorForm form) {
			AddParameterObjectToService(provider, RootWrapperEditorFormServiceName, form);
		}
		public static void AddWebControlDesigner(IServiceProvider provider, string controlID, ASPxWebControlDesigner designer) {
			if(designer != null)
				AddParameterObjectToService(provider, ControlDesignerPrefix + controlID, designer);
		}
		public static ASPxWebControlDesigner GetWebControlDesigner(IServiceProvider provider, string controlID) {
			return provider != null ? GetCommonDesignerParametersService(provider).GetParameter(ControlDesignerPrefix + controlID) as ASPxWebControlDesigner : null;
		}
		public static void RemoveWebControlDesigner(IServiceProvider provider, string controlID) {
			RemoveCommonDesignerParameter(provider, ControlDesignerPrefix + controlID);
		}
		public static WrapperEditorForm GetRootWrapperEditorFormObject(IServiceProvider provider) {
			return provider != null ? GetCommonDesignerParametersService(provider).GetParameter(RootWrapperEditorFormServiceName) as WrapperEditorForm : null;
		}
		public static Dictionary<object, Dictionary<string, object>> GetPropertyValueSaverObject(IServiceProvider provider) {
			if(provider == null)
				return null;
			return GetCommonDesignerParametersService(provider).GetParameter(PropertyValuesSaverServiceName) as Dictionary<object, Dictionary<string, object>>;
		}
		public static string GetDiscoveredPropertyService(IServiceProvider provider) {
			return GetCommonDesignerParameter<string>(provider, DiscoveredPropertyServiceName);
		}
		public static void SetDiscoveredPropertyService(IServiceProvider provider, object value) {
			SetCommonDesignerParameter(provider, DiscoveredPropertyServiceName, value);
		}
		public static void RegisterItemsOwner(IServiceProvider provider, IOwnerEditingProperty editingOwner, DesignerItem designerItem) {
			if(editingOwner == null || provider == null)
				return;
			var parametersService = GetCommonDesignerParametersService(provider);
			if(parametersService == null)
				return;
			var editingOwners = parametersService.GetParameter(OwnerEditingItemsServiceName) as Dictionary<IOwnerEditingProperty, DesignerItem>;
			if(editingOwners != null && !editingOwners.ContainsKey(editingOwner))
				editingOwners[editingOwner] = designerItem;
		}
		public static void SetEscapeBtnUpService(IServiceProvider provider, OnEscapeKeyUp escapeBtnUp) {
			SetCommonDesignerParameter(provider, EscapeBtnUpServiceServiceName, escapeBtnUp);
		}
		public static OnEscapeKeyUp GetEscapeBtnUpService(IServiceProvider provider) {
			return GetCommonDesignerParameter<OnEscapeKeyUp>(provider, EscapeBtnUpServiceServiceName);
		}
		public static void RemoveEscapeBtnUpService(IServiceProvider provider) {
			RemoveCommonDesignerParameter(provider, EscapeBtnUpServiceServiceName);
		}
		public static void RemoveDiscoveredPropertyService(IServiceProvider provider) {
			var result = FindCommonDesignerParametersService(provider);
			if(result != null) {
				var serviceContainer = provider.GetService(typeof(IServiceContainer)) as IServiceContainer;
				if(serviceContainer != null)
					serviceContainer.RemoveService(typeof(CommonDesignerParametersService));
			}
		}
		static void AddParameterObjectToService(IServiceProvider provider, string paramName, object obj) { 
			if(provider != null) {
				var parameterService = GetCommonDesignerParametersService(provider);
				if(parameterService.GetParameter(paramName) == null)
					parameterService.AddParameter(paramName, obj);
			}
		}
		static T GetCommonDesignerParameter<T>(IServiceProvider provider, string parameterName) {
			var service = GetCommonDesignerParametersService(provider);
			if(service == null)
				return default(T);
			var result = service.GetParameter(parameterName);
			return result is T ? (T)result : default(T);
		}
		static void SetCommonDesignerParameter(IServiceProvider provider, string parameterName, object value) {
			var service = GetCommonDesignerParametersService(provider);
			if(service != null)
				service.AddParameter(parameterName, value);
		}
		static void RemoveCommonDesignerParameter(IServiceProvider provider, string parameterName) {
			var service = GetCommonDesignerParametersService(provider);
			if(service != null)
				service.RemoveParameter(parameterName);
		}
		static CommonDesignerParametersService GetCommonDesignerParametersService(IServiceProvider provider) {
			if(provider == null)
				return null;
			var result = FindCommonDesignerParametersService(provider);
			if(result != null)
				return result;
			var serviceContainer = provider.GetService(typeof(IServiceContainer)) as IServiceContainer;
			if(serviceContainer != null) {
				result = new CommonDesignerParametersService();
				serviceContainer.AddService(typeof(CommonDesignerParametersService), result);
			}
			return result;
		}
		static CommonDesignerParametersService FindCommonDesignerParametersService(IServiceProvider provider) {
			return provider != null ? provider.GetService(typeof(CommonDesignerParametersService)) as CommonDesignerParametersService : null;
		}
	}
	public class CommonDesignerUndoHelper {
		public static void SavePropertyGridChangedValue(IServiceProvider serviceProvider, DXPropertyGridEx propertyGrid, PropertyValueChangedEventArgs evt) {
			var propertySaverObject = CommonDesignerServiceRegisterHelper.GetPropertyValueSaverObject(serviceProvider);
			if(propertySaverObject == null) return;
			var rootInstance = ConverterHelper.DiscoverObjectInstance(propertyGrid.SelectedObject);
			var propertyName = evt.ChangedItem.Label;
			var rootHasProperty = rootInstance.GetType().GetProperties().Any(p => p.Name == propertyName);
			var propertyPath = rootHasProperty ? propertyName : GetPropertyGridItemPath(evt.ChangedItem);
			var oldValue = evt.OldValue;
			if(!propertySaverObject.ContainsKey(rootInstance))
				propertySaverObject[rootInstance] = new Dictionary<string, object>();
			if(!propertySaverObject[rootInstance].ContainsKey(propertyPath))
				propertySaverObject[rootInstance][propertyPath] = oldValue;
		}
		public static void UndoPropertyGridChangedValues(IServiceProvider serviceProvider) {
			var changedPropertiesOldValues = CommonDesignerServiceRegisterHelper.GetPropertyValueSaverObject(serviceProvider);
			if(changedPropertiesOldValues == null) return;
			foreach(var instance in changedPropertiesOldValues.Keys)
				foreach(var propertyName in changedPropertiesOldValues[instance].Keys)
					FeatureBrowserHelper.SetPropertyValue(instance, propertyName, changedPropertiesOldValues[instance][propertyName]);
		}
		public static bool IsPropertiesModified(IServiceProvider serviceProvider) {
			var propertySaverObject = CommonDesignerServiceRegisterHelper.GetPropertyValueSaverObject(serviceProvider);
			return propertySaverObject != null && propertySaverObject.Count > 0;
		}
		static string GetPropertyGridItemPath(GridItem item) {
			var result = item.Label;
			if(item.Parent == null)
				return result;
			var parentPath = GetPropertyGridItemPath(item.Parent);
			return string.IsNullOrEmpty(parentPath) ? result : parentPath + "." + result;
		}
	}
}
