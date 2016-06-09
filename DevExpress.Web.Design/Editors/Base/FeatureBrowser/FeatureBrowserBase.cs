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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Designer.Utils;
using DevExpress.XtraEditors.FeatureBrowser;
using DevExpress.XtraTab;
namespace DevExpress.Web.Design {
	public interface IViewInfoForm {
		FeatureBrowserViewInfo ViewInfo { get; set; }
		void UpdateForm();
	}
	public interface INavigateFrames {
		void GotoFeature(string featureName, string frameName, string value);
		void Goto(string frameName, string value);
	}
	[CLSCompliant(false)]
	public abstract class MainEmbeddedFrame : FeatureBrowserMainFrameWeb {
		PanelControl MainPanel { get { return this.pnlMain; } }
		SplitContainerControl panelBrowserMain;
		protected internal SplitContainerControl PanelBrowserMain {
			get {
				if(panelBrowserMain == null)
					panelBrowserMain = (SplitContainerControl)FeatureBrowserHelper.FindControlByType(this, typeof(SplitContainerControl));
				return panelBrowserMain;
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			PanelBrowserMain.Visible = false;
			PanelBrowserMain.Panel2.Parent = MainPanel;
			PanelBrowserMain.Panel2.Dock = DockStyle.Fill;
		}
	}
	[CLSCompliant(false)]
	public abstract class FeatureBrowserMainFrameWeb : FeatureBrowserMainFrame {
		FeaturesTreeView treeViewItems;
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			var viewInfoForm = CurrentForm as IViewInfoForm;
			if(viewInfoForm != null) {
				viewInfoForm.ViewInfo = DesignerItem.Tag as FeatureBrowserViewInfo;
				viewInfoForm.UpdateForm();
				SelectFeatureByDiscoverProperty();
			}
		}
		protected void SelectFeatureByDiscoverProperty() {
			var propertyPath = CommonDesignerServiceRegisterHelper.GetDiscoveredPropertyService(GetControlServiceProvider());
			if(string.IsNullOrEmpty(propertyPath))
				return;
			var featureItem = FeatureBrowserHelper.FindFeatureItemByPropertyPath(Root, propertyPath);
			var node = FindNodeByText(null, featureItem.Name); 
			if(node != null)
				TreeViewItems.SelectNode(node);
		}
		IServiceProvider GetControlServiceProvider() {
			var component = EditingObject as IComponent;
			return component != null ? component.Site : null;
		}
		TreeNode FindNodeByText(TreeNode rootNode, string text) {
			if(rootNode != null && rootNode.Text == text)
				return rootNode;
			var nodes = rootNode != null ? rootNode.Nodes : TreeViewItems.Nodes;
			foreach(TreeNode node in nodes) {
				var result = node.Text == text ? node : FindNodeByText(node, text);
				if(result != null)
					return result;
			}
			return null;
		}
		protected internal FeaturesTreeView TreeViewItems {
			get {
				if(treeViewItems == null)
					treeViewItems = (FeaturesTreeView)FeatureBrowserHelper.FindControlByType(this, typeof(FeaturesTreeView));
				return treeViewItems;
			}
		}
	}
	[CLSCompliant(false)]
	public abstract class FeatureTabbedViewForm : FeatureBrowserFormBase, IViewInfoForm, INavigateFrames {
		protected const int FramePadding = 8;
		protected const int FrameIndent = FramePadding * 2;
		IServiceProvider serviceProvider;
		XtraPanel rightPanelContainer;
		PropertyEditorItems viewsSelector;
		Dictionary<string, IEmbeddedFrame> pageToFrameAssociator;
		Dictionary<string, DevExpress.Utils.Design.PropertyStore> localStores;
		string designTimePropertyStorePath;
		string defaultDesignTimePropertyStorePath = @"Software\Developer Express .NET\ASPxClasses\Designer\FeatureBrowser";
		public FeatureTabbedViewForm()
			: base() {
			InitializeForm();
		}
		public FeatureBrowserViewInfo ViewInfo { get; set; }
		protected virtual bool RemoveEmptyPages { get { return false; } }
		protected virtual bool TopPanelVisibility { get { return false; } }
		protected SplitContainerControl MainPanel { get { return this.pnlMain; } }
		protected XtraPanel TopPanel { get; set; }
		protected XtraPanel BottomPanel { get; set; }
		protected internal XtraTabControl MainTabControl { get; private set; }
		protected string DesignTimePropertyStorePath {
			get {
				if(SourceObject == null)
					return defaultDesignTimePropertyStorePath;
				if(string.IsNullOrEmpty(designTimePropertyStorePath))
					designTimePropertyStorePath = string.Format(@"{0}\{1}\", defaultDesignTimePropertyStorePath, SourceObject.GetType().Name);
				return designTimePropertyStorePath;
			}
		}
		XtraPanel RightPanelContainer {
			get {
				if(rightPanelContainer == null)
					rightPanelContainer = CreatePanel("RightPanelContrainer", DockStyle.Fill, 0, MainPanel.Parent);
				return rightPanelContainer;
			}
		}
		protected virtual bool HasPreview { get { return false; } }
		protected PropertyEditorItems ViewsSelector {
			get {
				if(viewsSelector == null)
					viewsSelector = new PropertyEditorItems(GetTopLevelPropertyEditorType(), GetDependendViewSelectorPropertyName());
				return viewsSelector;
			}
		}
		protected ComboBoxEdit ComboBoxViewSelector { get; set; }
		protected internal Dictionary<string, IEmbeddedFrame> PageToFrameAssociator {
			get {
				if(pageToFrameAssociator == null)
					pageToFrameAssociator = new Dictionary<string, IEmbeddedFrame>();
				return pageToFrameAssociator;
			}
		}
		protected internal Dictionary<string, DevExpress.Utils.Design.PropertyStore> LocalStores {
			get {
				if(localStores == null)
					localStores = new Dictionary<string, DevExpress.Utils.Design.PropertyStore>();
				return localStores;
			}
		}
		protected FeatureBrowserItemPage SelectedFeature {
			get {
				if(MainTabControl == null || MainTabControl.SelectedTabPage == null || MainTabControl.SelectedTabPage.Tag == null)
					return null;
				return (FeatureBrowserItemPage)MainTabControl.SelectedTabPage.Tag;
			}
		}
		IServiceProvider ServiceProvider {
			get {
				if(serviceProvider == null) {
					var component = SourceObject as IComponent;
					if(component != null)
						serviceProvider = component.Site != null ? (IServiceProvider)component.Site : null;
				}
				return serviceProvider;
			}
		}
		protected virtual FeatureBrowserDefaultPageBase CreateDefaultPageDesigner() {
			return new FeatureDefaultPageFrame();
		}
		protected virtual void InitializeForm() {
			Load += (sender, e) => {
				CorrectPages();
				this.BorderStyle = BorderStyle.None;
				if(!HasPreview)
					MainPanel.Visible = false;
				CreateInnerControls();
				OnTopPanelVisibleChanged(TopPanel, null);
			};
		}
		protected void CorrectPages() {
			if(!RemoveEmptyPages || Pages == null)
				return;
			var clearList = new List<int>();
			for(var i = 0; i < Pages.Count; ++i) {
				var page = Pages[i];
				if(string.IsNullOrEmpty(page.Name) && (page.Properties == null || page.Properties.Length == 0))
					clearList.Add(i);
			}
			clearList.ForEach(i => Pages.RemoveAt(i));
		}
		void NavigateToFrameByDiscoveredProperty() {
			var tabPage = FindPageByPropertyPath(CommonDesignerServiceRegisterHelper.GetDiscoveredPropertyService(ServiceProvider));
			if(tabPage != null && tabPage.PageVisible) {
				MainTabControl.SelectedTabPage = tabPage;
				CommonDesignerServiceRegisterHelper.SetDiscoveredPropertyService(ServiceProvider, null);
			}
		}
		XtraTabPage FindPageByPropertyPath(string propertyPath) {
			if(string.IsNullOrEmpty(propertyPath) || MainTabControl == null)
				return null;
			var featureItem = FeatureBrowserHelper.FindFeatureItemByPropertyPath(FeatureBrowserItem, propertyPath);
			var pages = MainTabControl.TabPages;
			foreach(FeatureBrowserItemPage page in Pages)
				if(page.Properties.Contains(propertyPath))
					return pages.FirstOrDefault(t => t.Tag == page);
			return null;
		}
		void IViewInfoForm.UpdateForm() {
			FillPageToFrameAssociator();
			OnMainTabControlSelectedPageChanged(null, null);
		}
		protected override void Dispose(bool disposing) {
			StoreProperties();
			base.Dispose(disposing);
		}
		internal virtual void CreateInnerControls() {
			SuspendLayout();
			CreatePanels();
			CreateTabControl();
			CreateComboViewSelector();
			ResumeLayout();
		}
		protected void CreatePanels() {
			TopPanel = CreatePanel("TopPanel", DockStyle.Top, 40, RightPanelContainer, TopPanelVisibility);
			TopPanel.VisibleChanged += OnTopPanelVisibleChanged;
			BottomPanel = CreatePanel("BottomPanel", DockStyle.Fill, 0, RightPanelContainer);
		}
		protected void CreateComboViewSelector() {
			FillViewInfoSelector();
			if(ViewsSelector == null || ViewsSelector.Count == 0)
				return;
			var labelView = new LabelControl() {
				Name = "LabelView",
				Text = "View Type:"
			};
			TopPanel.Controls.Add(labelView);
			labelView.Left = 0;
			labelView.Top = TopPanel.Height / 2 - labelView.Height / 2;
			labelView.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			ComboBoxViewSelector = new ComboBoxEdit();
			ComboBoxViewSelector.Name = "ComboBoxEdit";
			TopPanel.Controls.Add(ComboBoxViewSelector);
			ComboBoxViewSelector.Left = labelView.Left + labelView.Width + FramePadding / 2;
			ComboBoxViewSelector.Top = labelView.Top - 2;
			ComboBoxViewSelector.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			ComboBoxViewSelector.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			ComboBoxViewSelector.Properties.Items.AddRange(ViewsSelector);
			ComboBoxViewSelector.SelectedIndexChanged += (sender, e) => { SelectedViewChanged(); };
			SelectViewInfo();
		}
		protected virtual void FillPageToFrameAssociator() { }
		protected virtual void FillViewInfoSelector() { }
		protected virtual PropertyEditorType GetTopLevelPropertyEditorType() {
			return PropertyEditorType.TextEdit;
		}
		protected internal virtual string GetDependendViewSelectorPropertyName() {
			return string.Empty;
		}
		protected void AddPageToFrameAssociation(string pageName, IEmbeddedFrame embedFrame) {
			if(embedFrame != null)
				PageToFrameAssociator[pageName] = embedFrame;
		}
		protected void SelectViewInfo() {
			if(!ViewsSelector.HasDependendProperty)
				return;
			var propertyValue = FeatureBrowserHelper.GetPropertyValue(SourceObject, ViewsSelector.DependendPropertyName);
			if(propertyValue == null)
				return;
			var selectorInfo = ViewsSelector.FindSelectorInfoByPropertyValue(propertyValue);
			if(selectorInfo != null)
				ComboBoxViewSelector.SelectedItem = selectorInfo;
		}
		protected virtual void SelectedViewChanged() {
			if(ComboBoxViewSelector == null || !ComboBoxViewSelector.Visible)
				return;
			var selectedItem = ComboBoxViewSelector.SelectedItem;
			if(selectedItem == null)
				return;
			var viewInfo = (PropertyEditorItemInfo)selectedItem;
			var tabPageNames = viewInfo.TabNames;
			foreach(XtraTabPage page in MainTabControl.TabPages)
				page.PageVisible = tabPageNames.Contains(page.Name);
			if(ViewsSelector.HasDependendProperty)
				FeatureBrowserHelper.SetPropertyValue(SourceObject, ViewsSelector.DependendPropertyName, viewInfo.PropertyValueToSet);
		}
		protected void CreateTabControl() {
			MainTabControl = new XtraTabControl();
			MainTabControl.Name = "MainTabControl";
			BottomPanel.Controls.Add(MainTabControl);
			MainTabControl.Dock = DockStyle.Fill;
			MainTabControl.TabIndex = 3;
			RecreatePropertyTabs();
		}
		protected XtraPanel CreatePanel(string name, DockStyle dock, int height, Control controlToAdd, bool defaultVisible = true) {
			var result = new XtraPanel() { Name = name };
			controlToAdd.Controls.Add(result);
			result.Height = height;
			result.Dock = dock;
			result.BringToFront();
			result.Visible = defaultVisible;
			return result;
		}
		protected void OnTopPanelVisibleChanged(object sender, EventArgs e) {
			SuspendLayout();
			if(TopPanel.Visible) {
				BottomPanel.Dock = DockStyle.None;
				BottomPanel.Left = 0;
				BottomPanel.Top = TopPanel.Height;
				BottomPanel.Height = RightPanelContainer.Height - TopPanel.Height;
				BottomPanel.Width = RightPanelContainer.Width;
				BottomPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			} else { 
				BottomPanel.Top = 0;
				BottomPanel.Dock = DockStyle.Fill;
			}
			ResumeLayout();
		}
		protected override void OnFeatureNameChanged() {
			base.OnFeatureNameChanged();
			if(TopPanel != null)
				TopPanel.Visible = TopPanelVisibility;
		}
		protected void RecreatePropertyTabs() {
			if(MainTabControl == null)
				return;
			var selectedPage = MainTabControl.SelectedTabPage;
			object oldSelectedPageTag = null;
			if(selectedPage != null)
				oldSelectedPageTag = MainTabControl.SelectedTabPage.Tag;
			UpdateTabPages();
			NavigateToFrameByDiscoveredProperty();
		}
		void UpdateTabPages() {
			MainTabControl.BeginUpdate();
			MainTabControl.SelectedPageChanged -= OnMainTabControlSelectedPageChanged;
			foreach(XtraTabPage tabPage in MainTabControl.TabPages)
				tabPage.PageVisible = false;
			int visibleCount = 0;
			foreach(FeatureBrowserItemPage page in Pages) {
				var pageName = FeatureBrowserHelper.GetTabPageName(page.Name, FeatureBrowserItem.Id);
				var pageText = FeatureBrowserHelper.GetTabPageText(page.Name, pageName, FeatureBrowserItem.Name);
				var tabPage = MainTabControl.TabPages.FirstOrDefault(t => t.Name == pageName);
				if(tabPage == null) {
					tabPage = new XtraTabPage() {
						Name = pageName,
						Text = pageText
					};
					MainTabControl.TabPages.Add(tabPage);
				} else {
					tabPage.PageVisible = true;
				}
				tabPage.Tag = page;
				if(tabPage.PageVisible)
					++visibleCount;
			}
			var singlePage = visibleCount == 1;
			MainTabControl.ShowTabHeader = !singlePage ? DefaultBoolean.True : DefaultBoolean.False;
			MainTabControl.SelectedPageChanged += OnMainTabControlSelectedPageChanged;
			MainTabControl.EndUpdate();
			OnMainTabControlSelectedPageChanged(null, null);
		}
		protected virtual void OnMainTabControlSelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			var selectedTab = MainTabControl.SelectedTabPage;
			if(selectedTab == null)
				return;
			var page = (FeatureBrowserItemPage)selectedTab.Tag;
			if(page == null)
				return;
			if(!PageToFrameAssociator.ContainsKey(selectedTab.Name))
				AddPageToFrameAssociation(selectedTab.Name, CreateDefaultPageDesigner());
			SetEmbeddedFrame(page, PageToFrameAssociator[selectedTab.Name], selectedTab);
		}
		protected override void OnSourceObjectChanged() {
			StoreProperties();
			RecreatePropertyTabs();
			base.OnSourceObjectChanged();
			SelectedViewChanged();
		}
		protected void StoreProperties() {
			foreach(var key in LocalStores.Keys) {
				var propertyStoreFrame = PageToFrameAssociator[key] as XtraFrame;
				if(propertyStoreFrame != null) {
					var path = string.Format("{0}{1}", DesignTimePropertyStorePath, key);
					var localStore = LocalStores[key];
					propertyStoreFrame.StoreLocalProperties(localStore);
					if(!localStore.IsEmpty)
						localStore.Store();
				}
			}
		}
		protected virtual void SetEmbeddedFrame(FeatureBrowserItemPage page, IEmbeddedFrame embedFrame, XtraTabPage tabPage) {
			tabPage.SuspendLayout();
			var frame = (XtraFrame)embedFrame;
			SetupEmbeddedFrame(embedFrame, page, SourceObject, null, page.Description);
			frame.Left = FramePadding;
			frame.Top = FramePadding;
			frame.Size = new Size(tabPage.Width - FrameIndent, tabPage.Height - FrameIndent);
			frame.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
			tabPage.Controls.Clear();
			tabPage.Controls.Add(frame);
			tabPage.ResumeLayout();
			if(MainTabControl != null && MainTabControl.SelectedTabPage != null)
				RestoreFrameLocalProperties(frame, page);
		}
		protected void RestoreFrameLocalProperties(XtraFrame frame, FeatureBrowserItemPage page) {
			var key = PageToFrameAssociator.Keys.FirstOrDefault(k => PageToFrameAssociator[k] == frame);
			if(key == null)
				return;
			var path = string.Format("{0}{1}", DesignTimePropertyStorePath, key);
			if(!LocalStores.ContainsKey(key))
				LocalStores[key] = new DevExpress.Utils.Design.PropertyStore(path);
			var localStore = LocalStores[key];
			localStore.Restore();
			frame.RestoreLocalProperties(localStore);
		}
		protected bool PageHasProperties(FeatureBrowserItemPage page) {
			return page.Properties != null && page.Properties.Length != 0;
		}
		public void GotoFeature(string featureName, string frameName, string value) {
			var featurePageOwner = (IFeatureBrowserPageOwner)this;
			featurePageOwner.GotoFeatureName(featureName);
			Goto(frameName, value);
		}
		public void Goto(string frameName, string value) {
			((IFeatureBrowserPageOwner)this).Goto(frameName, value);
		}
	}
	public class FeatureBrowserViewInfo {
		public FeatureBrowserViewInfo(object tag) {
			Tag = tag;
		}
		public object Tag { get; private set; }
	}
	[CLSCompliant(false)]
	public class FeatureDefaultPageFrame : WebFeatureBrowserDefaultPageBase {
		SplitContainerControl splitContainer;
		public bool SplitHorizontally { get; set; }
		SplitContainerControl SplitContainer {
			get {
				if(splitContainer == null) {
					foreach(var control in Controls) {
						splitContainer = control as SplitContainerControl;
						if(splitContainer != null)
							break;
					}
				}
				return splitContainer;
			}
		}
		public FeatureDefaultPageFrame()
			: base() {
			this.pnlMain = new PanelControl();
			this.pnlMain.Parent = this;
			this.pnlMain.Dock = DockStyle.Fill;
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			this.BorderStyle = BorderStyle.None;
			InitializeLayout();
		}
		protected void InitializeLayout() {
			SuspendLayout();
			SplitContainer.Parent = this.pnlMain;
			if(string.IsNullOrEmpty(EmbeddedFrameInit.Description)) {
				Controls.Add(pgMain);
				pgMain.BringToFront();
				pgMain.Dock = DockStyle.Fill;
				SplitContainer.Visible = false;
			} else {
				SplitContainer.Visible = true;
				SplitContainer.Panel2.Controls.Add(pgMain);
				if(!SplitHorizontally) {
					SplitContainer.Horizontal = SplitHorizontally;
					SplitContainer.SplitterPosition = 80;
					SplitContainer.IsSplitterFixed = true;
				}
				SplitContainer.Parent.Dock = DockStyle.Fill;
			}
			ResumeLayout();
		}
	}
	[CLSCompliant(false)]
	public class WebFeatureBrowserDefaultPageBase : FeatureBrowserDefaultPageBase {
		FeatureLabelInfo labelInfo;
		protected virtual DescriptionActions DescriptionActions { get { return null; } }
		protected FeatureLabelInfo LabelInfo {
			get {
				if(labelInfo == null) {
					labelInfo = FindLabelInfo(this);
					if(labelInfo != null)
						labelInfo.ItemClick += labelInfo_ItemClick;
				}
				return labelInfo;
			}
		}
		bool CanHandleDescriptionActions { get { return LabelInfo != null && DescriptionActions != null; } }
		void labelInfo_ItemClick(object sender, LabelInfoItemClickEventArgs e) {
			var labelInfo = e.InfoText.Tag as IFeatureLabelInfoText;
			if(labelInfo == null)
				return;
			if(TryHandleLink(labelInfo))
				return;
			if(!CanHandleDescriptionActions)
				return;
			if(DescriptionActions.ContainsKey(labelInfo.PropertyName)) {
				var descriptionAction = DescriptionActions[labelInfo.PropertyName];
				descriptionAction.ActionMethod();
				UpdateLabelInfoState(e.InfoText, descriptionAction);
				GotoPageFrame(descriptionAction.GotoFeatureName, descriptionAction.GotoFrameName, descriptionAction.GotoPageName);
			}
		}
		bool TryHandleLink(IFeatureLabelInfoText labelInfo) {
			var url = new Regex(@"(?<=lnk_).*").Match(labelInfo.PropertyName).Value;
			if(string.IsNullOrEmpty(url))
				return false;
			var component = EditingObject as IComponent;
			if(component == null)
				return false;
			var serviceProvider = component.Site as IServiceProvider;
			if(serviceProvider == null)
				return false;
			var helpService = serviceProvider.GetService(typeof(IHelpService)) as IHelpService;
			if(url.StartsWith("#"))
				url = AssemblyInfo.SRDocumentationLink + url;
			DesignUtils.ShowHelpFromUrl(url, helpService);
			return true;
		}
		void GotoPageFrame(string featureName, string pageName = "", string featureValue = "") {
			var frameNavigator = FrameOwner as INavigateFrames;
			if(frameNavigator != null) {
				if(!string.IsNullOrEmpty(featureName))
					frameNavigator.GotoFeature(featureName, pageName, featureValue);
				else
					frameNavigator.Goto(pageName, featureValue);
			}
		}
		void UpdateLabelInfoState(LabelInfoText infoText, DescriptionAction descriptionAction) {
			try {
				infoText.HasCheckBox = descriptionAction.HasCheckbox;
				if(descriptionAction.GetCheckboxValue != null)
					infoText.CheckBoxValue = descriptionAction.GetCheckboxValue();
			} catch(NullReferenceException ecx) {
				Console.WriteLine(ecx.Message);
			}
			var isActive = descriptionAction.IsActive();
			if(infoText.Active == isActive)
				return;
			infoText.Active = isActive;
			var text = descriptionAction.InverseActionText;
			descriptionAction.InverseActionText = infoText.Text;
			infoText.Text = text;
			if(!isActive)
				infoText.Color = Color.Gray;
			else if(infoText.Color == Color.Gray)
				infoText.Color = Color.LightBlue;
		}
		public override void DoInitFrame() {
			base.DoInitFrame();
			UpdateLabelInfoItemState();
			pgMain.PropertyValueChanged += (s, e) => { CommonDesignerUndoHelper.SavePropertyGridChangedValue(((IComponent)EditingObject).Site, (DXPropertyGridEx)s, e); };
		}
		void UpdateLabelInfoItemState() {
			if(LabelInfo == null)
				return;
			foreach(LabelInfoText infoText in LabelInfo.Texts) {
				var featureInfoText = infoText.Tag as IFeatureLabelInfoText;
				if(featureInfoText == null)
					continue;
				if(IsBoldText(featureInfoText)) {
					featureInfoText.Color = Color.Black;
					infoText.Bold = true;
					infoText.Active = false;
				} else if(IsHyperLink(featureInfoText)) {
					infoText.Bold = true;
				} else if(DescriptionActions != null && DescriptionActions.ContainsKey(featureInfoText.PropertyName)) {
					UpdateLabelInfoState(infoText, DescriptionActions[featureInfoText.PropertyName]);
				} else if(!string.IsNullOrEmpty(featureInfoText.PropertyName)) {
					featureInfoText.Color = Color.Orange;
				}
			}
		}
		bool IsBoldText(IFeatureLabelInfoText featureInfoText) {
			return featureInfoText.PropertyName == "bold";
		}
		bool IsHyperLink(IFeatureLabelInfoText featureInfoText) {
			var property = featureInfoText.PropertyName;
			return property.Length > 4 && property.Substring(0, 4) == "lnk_";
		}
		FeatureLabelInfo FindLabelInfo(Control parent) {
			if(parent is FeatureLabelInfo)
				return (FeatureLabelInfo)parent;
			foreach(Control control in parent.Controls) {
				var label = FindLabelInfo(control);
				if(label != null)
					return label;
			}
			return null;
		}
	}
	[CLSCompliant(false)]
	public class FeatureBrowserDefaultPageDescriptions : WebFeatureBrowserDefaultPageBase {
		SplitContainerControl splitContainer;
		GridViewColumnPropertiesOwner columnsOwner;
		double splitterPositionKoeff = 0;
		public FeatureBrowserDefaultPageDescriptions()
			: base() {
			Load += (s, e) => {
				SuspendLayout();
				if(string.IsNullOrEmpty(EmbeddedFrameInit.Description))
					HideInfoLabel();
				else if(NeedSwapSplitterPanels)
					SwapPropertyGridAndLabelInfo();
				ResumeLayout();
			};
		}
		protected virtual SplitContainerControl SplitContainer {
			get {
				if(splitContainer == null) {
					foreach(var control in Controls) {
						splitContainer = control as SplitContainerControl;
						if(splitContainer != null)
							break;
					}
				}
				return splitContainer;
			}
		}
		protected virtual bool NeedSwapSplitterPanels { get { return EmbeddedFrameInit.Properties.Length != 0 && LabelInfo != null; } }
		DevExpress.Utils.Design.PropertyStore LocalStore { get; set; }
		ASPxGridView GridView { get { return EditingObject as ASPxGridView; } }
		GridViewColumnPropertiesOwner ColumnsOwner {
			get {
				if(columnsOwner == null)
					columnsOwner = new GridViewColumnPropertiesOwner(GridView, GridView.Columns, null);
				return columnsOwner;
			}
		}
		double SplitterPositionKoeff {
			get {
				if(splitterPositionKoeff == 0)
					splitterPositionKoeff = Convert.ToDouble(LocalStore.RestoreProperty("SplitterPositionKoeff", 0.5f));
				return splitterPositionKoeff;
			}
		}
		public override void StoreLocalProperties(DevExpress.Utils.Design.PropertyStore localStore) {
			if(SplitContainer != null) {
				var koeff = SplitContainer.SplitterPosition / (double)SplitContainer.Width;
				localStore.AddProperty("SplitterPositionKoeff", koeff.ToString());
			}
		}
		public override void RestoreLocalProperties(DevExpress.Utils.Design.PropertyStore localStore) { 
			LocalStore = localStore;
			RestorePropertiesInternal();
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			RestorePropertiesInternal();
		}
		void RestorePropertiesInternal() {
			if(SplitContainer == null || LocalStore == null)
				return;
			SplitContainer.SplitterPosition = Convert.ToInt32(Width * SplitterPositionKoeff);
		}
		void HideInfoLabel() {
			this.Controls.Clear();
			pgMain.Parent = this;
		}
		void SwapPropertyGridAndLabelInfo() {
			SplitContainer.Panel1.Controls.Clear();
			SplitContainer.Panel2.Controls.Clear();
			FillRightSplitPanel();
			FillLeftSplitPanel();
		}
		protected virtual void FillLeftSplitPanel() {
			pgMain.Parent = SplitContainer.Panel1;
		}
		protected virtual void FillRightSplitPanel() {
			LabelInfo.Margin = new Padding(10);
			var labelInfoSource = LabelInfo.Parent != null ? LabelInfo.Parent : LabelInfo;
			labelInfoSource.Parent = SplitContainer.Panel2;
		}
		public virtual void SelectProperty(string name) {
			if(!string.IsNullOrEmpty(name) && pgMain != null)
				pgMain.SelectItem(name);
		}
	}
	[CLSCompliant(false)]
	public abstract class DocumentSelectorFormBase : FeatureTabbedViewForm {
		DetailsViewSettingsColumnsEditorFrame columnsEditorEmbeddedFrame;
		ItemsEditorFrame toolbarItemsEditorFrame;
		ItemsEditorFrame permissionSettingsFrame;
		protected abstract FileManagerDetailsColumnCollection DetailViewColumns { get; }
		protected abstract FileManagerToolbarItemCollection ToolbarItems { get; }
		protected abstract ItemsEditorOwner AccessRulesOwner { get; }
		protected abstract ItemsEditorOwner DetailsViewColumnsOwner { get; }
		protected abstract FileManagerToolbarItemsOwner ToolbarItemsOwner { get; }
		protected ItemsEditorFrame PermissionSettingsFrame {
			get {
				if(permissionSettingsFrame == null && AccessRulesOwner != null)
					permissionSettingsFrame = new ItemsEditorFrame(AccessRulesOwner);
				return permissionSettingsFrame;
			}
		}
		protected DetailsViewSettingsColumnsEditorFrame ColumnsEditorEmbeddedFrame {
			get {
				if(columnsEditorEmbeddedFrame == null)
					columnsEditorEmbeddedFrame = new DetailsViewSettingsColumnsEditorFrame((DetailsViewSettingsColumnsOwner)DetailsViewColumnsOwner);
				return columnsEditorEmbeddedFrame;
			}
		}
		protected ItemsEditorFrame ToolbarItemsEditorFrame {
			get {
				if(toolbarItemsEditorFrame == null && ToolbarItemsOwner != null)
					toolbarItemsEditorFrame = new ItemsEditorFrame(ToolbarItemsOwner);
				return toolbarItemsEditorFrame;
			}
		}
		protected override bool TopPanelVisibility { get { return FeatureId == "FileListSettings_Selector"; } }
		protected override PropertyEditorType GetTopLevelPropertyEditorType() {
			return PropertyEditorType.ComboBox;
		}
		protected internal override string GetDependendViewSelectorPropertyName() {
			return "SettingsDocumentSelector.FileListSettings.View";
		}
		protected override void FillPageToFrameAssociator() {
			AddPageToFrameAssociation("Columns_DetailsView", ColumnsEditorEmbeddedFrame);
			AddPageToFrameAssociation("Toolbar_Items", ColumnsEditorEmbeddedFrame);
			AddPageToFrameAssociation("PermissionSettings_Selector", PermissionSettingsFrame);
		}
		protected override void FillViewInfoSelector() {
			ViewsSelector.AddViewSelectorInfoItem("Details", new string[] { "Main_DetailsView", "Columns_DetailsView" }, FileListView.Details);
			ViewsSelector.AddViewSelectorInfoItem("Thumbnails", new string[] { "Main_ThumbnailsView" }, FileListView.Thumbnails);
		}
	}
	public abstract class DescriptionActions : Dictionary<string, DescriptionAction> {
		public DescriptionActions(ASPxWebControl webControl) {
			WebControl = webControl;
			FillDescriptionItems();
		}
		protected internal abstract string FeatureItemName { get; }
		protected ASPxWebControl WebControl { get; set; }
		IServiceProvider ServiceProvider { get { return WebControl != null ? WebControl.Site : null; } }
		public void AddDescriptionAction(string name, Action action, bool hasCheckBox, Func<bool> getCheckBoxValue) {
			AddDescriptionAction(name, action, string.Empty, string.Empty, hasCheckBox, getCheckBoxValue);
		}
		public void AddDescriptionAction(string name, Action action, string gotoFrameName, string gotoPageName, bool hasCheckBox, Func<bool> getCheckBoxValue) {
			AddDescriptionAction(name, string.Empty, action, string.Empty, gotoFrameName, gotoPageName, () => { return true; }, hasCheckBox, getCheckBoxValue);
		}
		public void AddDescriptionAction(string name, Action action, string gotoFeatureName, string gotoFrameName, string gotoPageName, bool hasCheckBox, Func<bool> getCheckBoxValue) {
			AddDescriptionAction(name, string.Empty, action, gotoFeatureName, gotoFrameName, gotoPageName, () => { return true; }, hasCheckBox, getCheckBoxValue);
		}
		public void AddDescriptionAction(string name, string activeText, Action action, string gotoFeatureName, string gotoFrameName, string gotoPageName, Func<bool> isActive, bool hasCheckBox, Func<bool> getCheckBoxValue) {
			if(!ContainsKey(name))
				this[name] = new DescriptionAction();
			this[name].ActionMethod = action;
			this[name].GotoFeatureName = gotoFeatureName;
			this[name].GotoFrameName = gotoFrameName;
			this[name].GotoPageName = gotoPageName;
			this[name].IsActive = isActive;
			this[name].InverseActionText = activeText;
			this[name].HasCheckbox = hasCheckBox;
			this[name].GetCheckboxValue = getCheckBoxValue;
		}
		protected abstract void FillDescriptionItems();
		protected void NavigateToEditForm(string navBarItemCaption) {
			if(ServiceProvider != null) {
				var rootForm = CommonDesignerServiceRegisterHelper.GetRootWrapperEditorFormObject(ServiceProvider);
				if(rootForm != null)
					rootForm.NavigateToNavBarItem(navBarItemCaption);
			}
		}
	}
	public class DescriptionAction {
		public Action ActionMethod { get; set; }
		public string GotoFeatureName { get; set; }
		public string GotoFrameName { get; set; }
		public string GotoPageName { get; set; }
		public Func<bool> IsActive { get; set; }
		public string InverseActionText { get; set; }
		public bool HasCheckbox { get; set; }
		public Func<bool> GetCheckboxValue { get; set; }
	}
	public class EmptyEmbeddedFrame : XtraFrame, IEmbeddedFrame {
		protected PanelControl MainPanel { get { return pnlMain; } }
		protected PanelControl HorizontalSplitter { get { return this.horzSplitter; } }
		public override void DoInitFrame() {
			base.DoInitFrame();
			BorderStyle = BorderStyle.None;
			if(this.lbCaption != null)
				this.lbCaption.Dispose();
		}
		Control IEmbeddedFrame.Control { get { return null; } }
		void IEmbeddedFrame.InitEmbeddedFrame(EmbeddedFrameInit frameInit) {
			DoInitFrame();
		}
		void IEmbeddedFrame.RefreshPropertyGrid() { }
		void IEmbeddedFrame.SelectProperty(string propertyName) { }
		void IEmbeddedFrame.SetPropertyGridSortMode(PropertySort propertySort) { }
		void IEmbeddedFrame.ShowPropertyGridToolBar(bool show) { }
	}
	[CLSCompliant(false)]
	public static class FeatureBrowserHelper {
		public static void SetPropertyValue(object rootComponent, string propertyPath, object value) {
			try {
				var propertyInfo = GetPropertyInfo(ref rootComponent, propertyPath);
				if(propertyInfo != null)
					propertyInfo.SetValue(rootComponent, value, null);
			} catch(Exception e) {
				Console.WriteLine("Error set value for '" + propertyPath + "': " + e.Message);
			}
		}
		public static object GetPropertyValue(object rootComponent, string propertyPath) {
			var propertyInfo = GetPropertyInfo(ref rootComponent, propertyPath);
			if(propertyInfo == null)
				return null;
			return propertyInfo.GetValue(rootComponent, null);
		}
		static PropertyInfo GetPropertyInfo(ref object rootComponent, string path) {
			if(string.IsNullOrEmpty(path)) return null;
			var currentType = rootComponent.GetType();
			PropertyInfo result = null;
			var properties = path.Split('.');
			var length = properties.Length;
			for(var i = 0; i < length; ++i) {
				result = currentType.GetProperty(properties[i]);
				if(result == null)
					return null;
				if(i < length - 1)
					rootComponent = result.GetValue(rootComponent, null);
				currentType = result.PropertyType;
			}
			return result;
		}
		public static string GetTabPageName(string pageName, string featureItemID) { 
			return !string.IsNullOrEmpty(pageName) ? pageName : featureItemID;
		}
		public static string GetTabPageText(string pageName, string tabPageName, string featureItemName) {
			return !string.IsNullOrEmpty(pageName) ? tabPageName.Split('_')[0] : featureItemName;
		}
		public static object[] GetFilteredProperties(object[] sourceObjects, string[] filteredProperties) {
			var result = new ArrayList();
			if(filteredProperties.Length == 0)
				return sourceObjects;
			foreach(var property in sourceObjects) {
				if(filteredProperties.Length > 0)
					result.Add(new FilterObject(property, filteredProperties, FilterObjectFilterPropertiesType.Exclude));
				else
					result.Add(property);
			}
			return result.ToArray();
		}
		public static FeatureBrowserItem FindFeatureItemByPropertyPath(FeatureBrowserItem startItem, string propertyPath) {
			return startItem != null ? FindFeatureItemByPropertyPathRecursive(FindRootItemRecursive(startItem), propertyPath) : null;
		}
		static FeatureBrowserItem FindFeatureItemByPropertyPathRecursive(FeatureBrowserItem parentItem, string propertyPath) {
			if(parentItem == null)
				return null;
			foreach(FeatureBrowserItemPage page in parentItem.Pages) {
				if(page.Properties.Contains(propertyPath))
					return parentItem;
			}
			foreach(FeatureBrowserItem item in parentItem) {
				var result = FindFeatureItemByPropertyPathRecursive(item, propertyPath);
				if(result != null)
					return result;
			}
			return null;
		}
		static FeatureBrowserItem FindRootItemRecursive(FeatureBrowserItem nestedItem) {
			return nestedItem.Parent != null ? FindRootItemRecursive(nestedItem.Parent) : nestedItem;
		}
		public static Control FindControlByType<T>(Control rootControl, T findType) where T : Type {
			Control result = null;
			if(rootControl.GetType() == findType) return rootControl;
			foreach(Control control in rootControl.Controls) {
				result = FindControlByType(control, findType);
				if(result != null)
					return result;
			}
			return null;
		}
	}
	public class FeatureBrowserPropertyDiscover {
		public FeatureBrowserPropertyDiscover(object editingObject) {
			EditingObject = editingObject;
			EditingObjectType = editingObject.GetType();
		}
		object EditingObject { get; set; }
		Type EditingObjectType { get; set; }
		string commonDesignerTypeName;
		string CommonDesignerTypeName {
			get {
				if(commonDesignerTypeName == null)
					commonDesignerTypeName = typeof(CommonDesignerEditor).FullName;
				return commonDesignerTypeName;
			}
		}
		Dictionary<string, List<string>> CommonDesignerTypes { get; set; }
		Dictionary<string, List<string>> ViewedTypes { get; set; }
		string PropertyPath { get; set; }
		public string GetPathToProperty(ITypeDescriptorContext descriptorContext) {
			var propertyType = descriptorContext.PropertyDescriptor.PropertyType;
			FillCommonDesignerTypes(EditingObjectType);
			var fullTypeName = propertyType.FullName;
			if(!CommonDesignerTypes.Keys.Contains(fullTypeName))
				return string.Empty;
			var result = CommonDesignerTypes[fullTypeName].FirstOrDefault(p => HasInstanceValue(EditingObject, p, descriptorContext.Instance));
			if(!string.IsNullOrEmpty(result))
				return result;
			var propertyName = descriptorContext.PropertyDescriptor.Name;
			return CommonDesignerTypes[fullTypeName].FirstOrDefault(v => v == propertyName);
		}
		bool HasInstanceValue(object rootComponent, string path, object instance) {
			if(string.IsNullOrEmpty(path))
				return false;
			var currentType = rootComponent.GetType();
			var currentProperty = rootComponent;
			try {
				foreach(string propertyName in path.Split('.')) {
					var propertyInfo = CheckAndGetMemberInfo(currentType, propertyName) as PropertyInfo;
					if(propertyInfo != null) {
						currentType = propertyInfo.PropertyType;
						currentProperty = propertyInfo.GetValue(currentProperty, null);
						if(instance == currentProperty)
							return true;
					}
				}
			} catch(Exception e) {
				Console.WriteLine(e.Message);
			}
			return false;
		}
		MemberInfo CheckAndGetMemberInfo(Type parentType, string propertyName) {
			MemberInfo memberInfo = parentType.GetProperty(propertyName);
			if(memberInfo == null)
				memberInfo = parentType.GetEvent(propertyName);
			return memberInfo;
		}
		void FillCommonDesignerTypes(Type rootType) {
			ViewedTypes = new Dictionary<string, List<string>>();
			CommonDesignerTypes = new Dictionary<string, List<string>>();
			FillCommonDesignerTypesRecursive(rootType);
		}
		void FillCommonDesignerTypesRecursive(Type rootType) {
			var propertyDescriptors = TypeDescriptor.GetProperties(rootType);
			var tempProp = PropertyPath;
			foreach(PropertyDescriptor descriptor in propertyDescriptors) {
				var propertyType = descriptor.PropertyType;
				var descriptorName = descriptor.Name;
				PropertyPath = !string.IsNullOrEmpty(tempProp) ? string.Format("{0}.{1}", tempProp, descriptorName) : descriptorName;
				var hasDesignerEditor = HasDesignerEditor(descriptor);
				if(!CanAnalizeProperty(hasDesignerEditor, propertyType, propertyDescriptors))
					continue;
				if(hasDesignerEditor) {
					var typeFullName = propertyType.FullName;
					if(!CommonDesignerTypes.ContainsKey(typeFullName))
						CommonDesignerTypes.Add(typeFullName, new List<string>());
					CommonDesignerTypes[typeFullName].Add(PropertyPath);
				}
				FillCommonDesignerTypesRecursive(propertyType);
			}
		}
		bool CanAnalizeProperty(bool hasDesignerEditor, Type propertyType, PropertyDescriptorCollection descriptors) {
			if(propertyType == EditingObjectType)
				return false;
			if(!typeof(IList).IsAssignableFrom(propertyType) && !typeof(PropertiesBase).IsAssignableFrom(propertyType))
				return false;
			var typeName = propertyType.Name;
			if(IsPropertyViewed(typeName))
				return false;
			AddViewedProperty(typeName);
			return true;
		}
		bool IsPropertyViewed(string typeName) {
			return ViewedTypes.Keys.Contains(typeName) && ViewedTypes[typeName].Contains(PropertyPath);
		}
		void AddViewedProperty(string typeName) {
			if(!ViewedTypes.ContainsKey(typeName))
				ViewedTypes[typeName] = new List<string>();
			ViewedTypes[typeName].Add(PropertyPath);
		}
		bool HasDesignerEditor(PropertyDescriptor descriptor) {
			Attribute result = descriptor.Attributes.OfType<EditorAttribute>().FirstOrDefault(e => e.EditorTypeName.Contains(CommonDesignerTypeName));
			if(result == null)
				result = descriptor.Attributes.OfType<DesignerAttribute>().FirstOrDefault(d => d.DesignerTypeName.Contains(CommonDesignerTypeName));
			return result != null;
		}
		public bool FindFeatureItemByPropertyName(string name, Type featureFrameType) {
			if(EditingObject == null || !typeof(FeatureBrowserMainFrameWeb).IsAssignableFrom(featureFrameType))
				return false;
			var instance = (FeatureBrowserMainFrameWeb)Activator.CreateInstance(featureFrameType);
			instance.InitFrame(EditingObject, "caption", null);
			return FeatureBrowserHelper.FindFeatureItemByPropertyPath(instance.Root, name) != null;
		}
	}
}
