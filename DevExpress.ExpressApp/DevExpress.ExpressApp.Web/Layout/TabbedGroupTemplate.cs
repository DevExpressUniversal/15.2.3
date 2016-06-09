#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Web.SystemModule;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.TestScripts;
using DevExpress.Web;
namespace DevExpress.ExpressApp.Web.Layout {
	public class TabbedGroupTemplateWithDelayedTabsInstantiation : TabbedGroupTemplate, IDisposable {
		private List<IModelLayoutGroup> instantiatedBeforeList = new List<IModelLayoutGroup>();
		private Dictionary<ASPxPageControl, TabbedGroupTemplateContainer> tabControlToTemplateContainerMap = new Dictionary<ASPxPageControl, TabbedGroupTemplateContainer>();
		private void pageControl_ActiveTabChanged(object source, TabControlEventArgs e) {
			ASPxPageControl pageControl = (ASPxPageControl)source;
			if(pageControl.ActiveTabIndex >= 0) { 
				InstantiateTab(pageControl, pageControl.ActiveTabIndex);
			}
		}
		private void InstantiateTab(ASPxPageControl pageControl, int tabIndex) {
			TabPage tabPage = pageControl.TabPages[tabIndex];
			if(tabPage.Controls.Count == 0) {
				TabbedGroupTemplateContainer templateContainer = tabControlToTemplateContainerMap[pageControl];
				InstantiateTabPageContent(templateContainer, pageControl, templateContainer.Model[tabPage.Name]);
			}
		}
		protected override void InstantiateTabPageContentCore(TabbedGroupTemplateContainer templateContainer, TabPage tabPage, IModelLayoutGroup itemInfo) {
			RegisterInstantiation(itemInfo);
			base.InstantiateTabPageContentCore(templateContainer, tabPage, itemInfo);
		}
		private void RegisterInstantiation(IModelLayoutGroup itemInfo) {
			if(!IsInstantiatedBefore(itemInfo)) {
				instantiatedBeforeList.Add(itemInfo);
			}
		}
		private bool IsInstantiatedBefore(IModelLayoutGroup itemInfo) {
			return instantiatedBeforeList.Contains(itemInfo);
		}
		protected override ASPxPageControl CreatePageControl(TabbedGroupTemplateContainer templateContainer) {
			ASPxPageControl pageControl = base.CreatePageControl(templateContainer);
			tabControlToTemplateContainerMap.Add(pageControl, templateContainer);
			pageControl.ActiveTabChanged += new TabControlEventHandler(pageControl_ActiveTabChanged);
			return pageControl;
		}
		protected override bool CanInstantiate(TabPage tabPage, IModelLayoutGroup itemInfo) {
			bool result = false;
			if(IsInstantiatedBefore(itemInfo)) {
				result = true;
			}
			else {
				ASPxPageControl pageControl = (ASPxPageControl)tabPage.TabControl;
				TabbedGroupTemplateContainer templateContainer = tabControlToTemplateContainerMap[pageControl];
#pragma warning disable 0618
				if(IsTabIndexValid(pageControl, templateContainer.ActiveTabIndex)) {
					if(tabPage.Index == templateContainer.ActiveTabIndex) {
						result = true;
					}
				}
#pragma warning restore 0618
				else if(IsTabNameValid(pageControl, templateContainer.ActiveTabName)) {
					if(tabPage.Name == templateContainer.ActiveTabName) {
						result = true;
					}
				}
				else {
					int indexInCookie = GetCurrentTabIndexFromCookies(pageControl);
					if(IsTabIndexValid(pageControl, indexInCookie)) {
						if(tabPage.Index == indexInCookie) {
							result = true;
						}
					}
					else if(pageControl.ActiveTabIndex == -1) {
						if(IsFirstVisiblePage(tabPage, pageControl)) {
							result = true;
						}
					}
					else if(tabPage.Index == pageControl.ActiveTabIndex) {
						result = true;
					}
				}
			}
			return result;
		}
		private bool IsTabIndexValid(ASPxPageControl pageControl, int tabIndex) {
			if(tabIndex < 0) {
				return false;
			}
			foreach(TabPage tabPage in pageControl.TabPages) {
				if(tabPage.Index == tabIndex) {
					return true;
				}
			}
			return false;
		}
		private bool IsTabNameValid(ASPxPageControl pageControl, string tabName) {
			if(string.IsNullOrEmpty(tabName)) {
				return false;
			}
			foreach(TabPage page in pageControl.TabPages) {
				if(page.Name == tabName) {
					return true;
				}
			}
			return false;
		}
		protected virtual int GetCurrentTabIndexFromCookies(ASPxPageControl pageControl) {
			int result = -1;
			if(pageControl.SaveStateToCookies) {
				string cookieValue = DevExpress.Web.Internal.RenderUtils.GetCookie(HttpContext.Current.Request, pageControl.SaveStateToCookiesID); 
				if(!int.TryParse(cookieValue, out result)) {
					result = -1;
				}
			}
			return result;
		}
		private bool IsFirstVisiblePage(TabPage tabPage, ASPxPageControl pageControl) {
			foreach(TabPage tab in pageControl.TabPages) {
				if(tab.Visible && tab.ClientVisible) {
					return tab == tabPage;
				}
			}
			return false;
		}
		public TabbedGroupTemplateWithDelayedTabsInstantiation() : this(new SimpleControlInstantiationStrategy()) { }
		public TabbedGroupTemplateWithDelayedTabsInstantiation(ControlInstantiationStrategyBase controlInstantiationStrategy) : base(controlInstantiationStrategy) { }
		public override void BreakLinksToControl() {
			if(tabControlToTemplateContainerMap != null) {
				foreach(ASPxPageControl key in tabControlToTemplateContainerMap.Keys) {
					key.ActiveTabChanged -= new TabControlEventHandler(pageControl_ActiveTabChanged);
				}
				tabControlToTemplateContainerMap.Clear();
			}
			base.BreakLinksToControl();
		}
		#region IDisposable Members
		public void Dispose() {
			instantiatedBeforeList.Clear();
		}
		#endregion
#if DebugTest
		public bool DebugTest_IsTabIndexValid(ASPxPageControl pageControl, int index) {
			return IsTabIndexValid(pageControl, index);
		}
		public bool DebugTest_IsTabNameValid(ASPxPageControl pageControl, string tabName) {
			return IsTabNameValid(pageControl, tabName);
		}
		public int DebugTest_GetCurrentTabIndexFromCookies(ASPxPageControl pageControl) {
			return GetCurrentTabIndexFromCookies(pageControl);
		}
		public bool DebugTest_IsFirstVisiblePage(TabPage tabPage, ASPxPageControl pageControl) {
			return IsFirstVisiblePage(tabPage, pageControl);
		}
		public bool DebugTest_IsInstantiatedBefore(IModelLayoutGroup itemInfo) {
			return IsInstantiatedBefore(itemInfo);
		}
		public IList<IModelLayoutGroup> DebugTest_InstantiatedBeforeList {
			get { return instantiatedBeforeList; }
		}
		public IDictionary<ASPxPageControl, TabbedGroupTemplateContainer> DebugTest_TabControlToTemplateContainerMap {
			get { return tabControlToTemplateContainerMap; }
		}
#endif
	}
	public class TabbedGroupTemplate : LayoutBaseTemplate, ITestableContainer, ILinkedToControl {
		class ActiveTabIndexOnCallbackSynchronizer {
			private TabbedGroupTemplateContainer templateContainer;
			private ASPxPageControl pageControl;
			public void Attach(TabbedGroupTemplateContainer templateContainer, ASPxPageControl pageControl) {
				Guard.ArgumentNotNull(templateContainer, "templateContainer");
				Guard.ArgumentNotNull(pageControl, "pageControl");
				this.templateContainer = templateContainer;
				this.pageControl = pageControl;
				this.pageControl.Load += pageControl_Load;
			}
			private void pageControl_Load(object sender, EventArgs e) {
				pageControl.Load -= pageControl_Load;
				SynchronizeActiveTabIndex(templateContainer, pageControl);
			}
			private static void SynchronizeActiveTabIndex(TabbedGroupTemplateContainer templateContainer, ASPxPageControl pageControl) {
#pragma warning disable 0618
				if(templateContainer.ActiveTabIndex != -1) {
					pageControl.ActiveTabIndex = templateContainer.ActiveTabIndex;
				}
#pragma warning restore 0618
				else {
					if(!string.IsNullOrEmpty(templateContainer.ActiveTabName)) {
						TabPage tabPage = pageControl.TabPages.FindByName(templateContainer.ActiveTabName);
						if(tabPage != null) {
							pageControl.ActiveTabPage = tabPage;
						}
					}
				}
			}
		}
		private bool enableCallbacks;
		private List<IDisposable> objectsToDispose = new List<IDisposable>();
		private List<ITestable> testableControls = new List<ITestable>();
		private void InitEnableCallbackOption(ControlInstantiationStrategyBase controlInstantiationStrategy) {
			EnableCallbacks = controlInstantiationStrategy is DelayedControlInstantiationStrategy;
		}
		protected override void InstantiateInCore(LayoutItemTemplateContainerBase container) {
			TabbedGroupTemplateContainer tabbedGroupTemplateContainer = (TabbedGroupTemplateContainer)container;
			if(tabbedGroupTemplateContainer.Model.Direction == FlowDirection.Vertical) {
				InstantiateVerticalTabbedGroup(tabbedGroupTemplateContainer);
			}
			else {
				InstantiateHorizontalTabbedGroup(tabbedGroupTemplateContainer);
			}
		}
		protected virtual void InstantiateVerticalTabbedGroup(TabbedGroupTemplateContainer templateContainer) {
			WebControl webControl = templateContainer.Parent as WebControl;
			if(webControl != null) {
				if(!webControl.CssClass.Contains(GroupLayoutCSSInfo.GetGroupContentCssClassName(true))) {
					webControl.CssClass += " " + GroupLayoutCSSInfo.GetGroupContentCssClassName(true);
				}
			}
			foreach(IModelLayoutGroup itemInfo in templateContainer.Model) {
				templateContainer.Controls.Add(templateContainer.Items[itemInfo.Id]);
			}
		}
		protected virtual void InstantiateHorizontalTabbedGroup(TabbedGroupTemplateContainer templateContainer) {
			Panel pageControlHolder = CreatePageControlHolder(templateContainer.Model);
			ASPxPageControl pageControl = CreatePageControl(templateContainer);
			CreateTabPages(templateContainer, pageControl);
			InstantiateTabPagesContent(templateContainer, pageControl);
			new ActiveTabIndexOnCallbackSynchronizer().Attach(templateContainer, pageControl);
			pageControlHolder.Controls.Add(pageControl);
			templateContainer.Controls.Add(pageControlHolder);
			OnTestableControlsCreated();
		}
		private Panel CreatePageControlHolder(IModelTabbedGroup tabbedGroupModel) {
			Panel pageControlHolder = new Panel();
			pageControlHolder.ID = WebIdHelper.GetCorrectedLayoutItemId(tabbedGroupModel, "", "_CardTable");
			if(WebApplicationStyleManager.IsNewStyle) {
				LayoutCSSInfo layoutCSSInfo = LayoutCSSCalculator.GetLayoutCSSInfo(tabbedGroupModel);
				if(layoutCSSInfo != null) {
					if(layoutCSSInfo.CardItem) {
						pageControlHolder.CssClass += layoutCSSInfo.CardCssClassNameCore;
					}
					else {
						pageControlHolder.CssClass += layoutCSSInfo.EditorContainerCssClassName;
					}
				}
			}
			else {
				pageControlHolder.CssClass = LayoutCSSInfo.LayoutTabbedGroupContainerCssClassName;
			}
			return pageControlHolder;
		}
		protected virtual ASPxPageControl CreatePageControl(TabbedGroupTemplateContainer templateContainer) {
			IModelTabbedGroup info = templateContainer.Model;
			ASPxPageControlTestable testablePageControl = new ASPxPageControlTestable();
			testablePageControl.ModelTabbedGroup = info;
			if(TestScriptsManager.EasyTestEnabled) {
				testableControls.Add(testablePageControl);
			}
			ASPxPageControl pageControl = testablePageControl.PageControl;
			pageControl.ID = WebIdHelper.GetCorrectedLayoutItemId(info, "", "_pg");
			pageControl.EnableCallBacks = EnableCallbacks;
			pageControl.SaveStateToCookies = true;
			pageControl.SaveStateToCookiesID = GetCookiesIdFromCurrentTabbedGroup(info);
			pageControl.Width = Unit.Percentage(100);
			pageControl.ContentStyle.Paddings.Padding = Unit.Pixel(0);
			pageControl.ContentStyle.CssClass = "TabControlContent";
			if(info is IModelViewLayoutElementWeb) {
				pageControl.ContentStyle.CssClass += " " + ((IModelViewLayoutElementWeb)info).CustomCSSClassName;
			}
			if(WebApplicationStyleManager.IsNewStyle) {
				pageControl.EnableTabScrolling = true;
			}
			OnPageControlCreated(info, pageControl);
			return pageControl;
		}
		protected virtual String GetCookiesIdFromCurrentTabbedGroup(IModelTabbedGroup info) {
			IModelNode currentNode = info;
			String cookiesId = "";
			while(currentNode != null) {
				if(currentNode is IModelViewLayoutElement) {
					cookiesId = ((IModelViewLayoutElement)currentNode).Id + "_" + cookiesId;
				}
				if(currentNode is IModelDetailView) {
					cookiesId = ((IModelDetailView)currentNode).Id + "_" + cookiesId;
					break;
				}
				currentNode = currentNode.Parent;
			}
			return WebIdHelper.GetCorrectedId(cookiesId).Substring(WebIdHelper.DetailViewItemIdPrefix.Length, cookiesId.Length - 1);
		}
		private void OnPageControlCreated(IModelTabbedGroup info, ASPxPageControl pageControl) {
			if(PageControlCreated != null) {
				PageControlCreated(this, new PageControlCreatedEventArgs(info, pageControl));
			}
		}
		protected virtual void CreateTabPages(TabbedGroupTemplateContainer templateContainer, ASPxPageControl pageControl) {
			IModelTabbedGroup info = templateContainer.Model;
			foreach(IModelLayoutGroup itemInfo in info) {
				TabPage tabPage = pageControl.TabPages.Add(itemInfo.Caption);
				tabPage.Name = itemInfo.Id;
				ImageInfo imageInfo = ImageLoader.Instance.GetImageInfo(itemInfo.ImageName);
				if(!imageInfo.IsEmpty) {
					ASPxImageHelper.SetImageProperties(tabPage.TabImage, imageInfo);
					tabPage.TabImage.AlternateText = itemInfo.Caption;
				}
				((ISupportToolTip)this).SetToolTip(tabPage, itemInfo);
				if(TestScriptsManager.EasyTestEnabled) {
					testableControls.Add(new TabTestControl(tabPage));
				}
				LayoutGroupTemplateContainer tabPageTemplateContainer = (LayoutGroupTemplateContainer)templateContainer.Items[itemInfo.Id];
				objectsToDispose.Add(new TabPageVisibilitySynchronizer(tabPageTemplateContainer, tabPage));
			}
		}
		private void InstantiateTabPagesContent(TabbedGroupTemplateContainer templateContainer, ASPxPageControl pageControl) {
			IModelTabbedGroup info = templateContainer.Model;
			foreach(IModelLayoutGroup itemInfo in info) {
				InstantiateTabPageContent(templateContainer, pageControl, itemInfo);
			}
		}
		protected virtual void InstantiateTabPageContent(TabbedGroupTemplateContainer templateContainer, ASPxPageControl pageControl, IModelLayoutGroup itemInfo) {
			TabPage tabPage = pageControl.TabPages.FindByName(itemInfo.Id);
			if(CanInstantiate(tabPage, itemInfo)) {
				InstantiateTabPageContentCore(templateContainer, tabPage, itemInfo);
			}
		}
		protected virtual bool CanInstantiate(TabPage tabPage, IModelLayoutGroup itemInfo) {
			return tabPage != null;
		}
		protected virtual void InstantiateTabPageContentCore(TabbedGroupTemplateContainer templateContainer, TabPage tabPage, IModelLayoutGroup itemInfo) {
			Panel panel = new Panel();
			panel.CssClass = "LayoutTabContainer";
			LayoutGroupTemplateContainer tabPageTemplateContainer = (LayoutGroupTemplateContainer)templateContainer.Items[itemInfo.Id];
			if(IsNestedFrame(tabPageTemplateContainer)) {
				panel.CssClass += " LayoutTabContainerWithNestedFrame";
			}
			panel.Controls.Add(tabPageTemplateContainer);
			tabPage.Controls.Add(panel);
		}
		protected bool IsNestedFrame(LayoutGroupTemplateContainer templateContainer) {
			if(templateContainer.Model.Count == 1) {
				IModelLayoutViewItem layoutItemModel = templateContainer.Model[0] as IModelLayoutViewItem;
				if(layoutItemModel != null) {
					IModelPropertyEditor propertyEditorModel = layoutItemModel.ViewItem as IModelPropertyEditor;
					if(propertyEditorModel != null) {
						Type propertyEditorType = propertyEditorModel.PropertyEditorType;
						if(propertyEditorType != null) {
							return propertyEditorType.IsAssignableFrom(typeof(ListPropertyEditor));
						}
					}
				}
			}
			return false;
		}
		protected void OnTestableControlsCreated() {
			if(TestableControlsCreated != null) {
				TestableControlsCreated(this, EventArgs.Empty);
			}
		}
		public TabbedGroupTemplate() : this(new SimpleControlInstantiationStrategy()) { }
		public TabbedGroupTemplate(ControlInstantiationStrategyBase controlInstantiationStrategy)
			: base(controlInstantiationStrategy) {
			InitEnableCallbackOption(controlInstantiationStrategy);
		}
		public bool EnableCallbacks {
			get { return enableCallbacks; }
			set { enableCallbacks = value; }
		}
		public event EventHandler<PageControlCreatedEventArgs> PageControlCreated;
		#region ITestableContainer Members
		public ITestable[] GetTestableControls() {
			return testableControls.ToArray();
		}
		public event EventHandler TestableControlsCreated;
		#endregion
		#region ILinkedToControl Members
		public virtual void BreakLinksToControl() {
			foreach(IDisposable disposable in objectsToDispose) {
				disposable.Dispose();
			}
			objectsToDispose.Clear();
			testableControls.Clear();
		}
		#endregion
#if DebugTest
		public string DebugTest_GetCookiesIdFromCurrentTabbedGroup(IModelTabbedGroup info) {
			return GetCookiesIdFromCurrentTabbedGroup(info);
		}
#endif
	}
	public class PageControlCreatedEventArgs : EventArgs {
		public PageControlCreatedEventArgs(IModelTabbedGroup model, ASPxPageControl pageControl) {
			this.PageControl = pageControl;
			this.Model = model;
		}
		public ASPxPageControl PageControl { get; private set; }
		public IModelTabbedGroup Model { get; private set; }
	}
	public class ASPxPageControlTestable : ITestableEx, IShowImages, IDisposable {
		private bool showImages = true;
		private ASPxPageControl pageControl;
		private IModelTabbedGroup modelTabbedGroup;
		private void Control_Unload(object sender, EventArgs e) {
			OnControlInitialized((Control)sender);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		public ASPxPageControlTestable() { }
		public ASPxPageControlTestable(IModelTabbedGroup modelTabbedGroup) {
			this.modelTabbedGroup = modelTabbedGroup;
		}
		public bool ShowImages {
			get { return showImages; }
			set { showImages = value; }
		}
		public ASPxPageControl PageControl {
			get {
				if(pageControl == null) {
					pageControl = RenderHelper.CreateASPxPageControl();
					pageControl.Unload += new EventHandler(Control_Unload);
				}
				return pageControl;
			}
			set { pageControl = value; }
		}
		public IModelTabbedGroup ModelTabbedGroup {
			get { return modelTabbedGroup; }
			set { modelTabbedGroup = value; }
		}
		#region ITestable Members
		string ITestable.ClientId {
			get { return pageControl.ClientID; }
		}
		string ITestable.TestCaption {
			get { return modelTabbedGroup.Id; }
		}
		IJScriptTestControl ITestable.TestControl {
			get { return null; }
		}
		public virtual TestControlType TestControlType {
			get { return TestControlType.Action; }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		#endregion
		#region ITestableEx Members
		public Type RegisterControlType {
			get { return PageControl.GetType(); }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			if(pageControl != null) {
				pageControl.Unload -= new EventHandler(Control_Unload);
			}
		}
		#endregion
	}
	internal class TabTestControl : ITestable {
		private TabPage tab;
		private void Control_Unload(object sender, EventArgs e) {
			OnControlInitialized((Control)sender);
		}
		public TabTestControl(TabPage tab) {
			this.tab = tab;
			this.tab.TabControl.Unload += new EventHandler(Control_Unload);
		}
		protected void OnControlInitialized(Control control) {
			if(ControlInitialized != null) {
				ControlInitialized(this, new ControlInitializedEventArgs(control));
			}
		}
		#region ITestable Members
		public string ClientId {
			get { return tab.TabControl.ClientID; }
		}
		public string TestCaption {
			get { return tab.Text; }
		}
		public IJScriptTestControl TestControl {
			get { return new ASPxTabTestControlScriptsDeclaration(); }
		}
		public virtual TestControlType TestControlType {
			get { return TestControlType.Action; }
		}
		public event EventHandler<ControlInitializedEventArgs> ControlInitialized;
		#endregion
	}
}
