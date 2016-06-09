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
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Editors;
using System.Web.UI;
using DevExpress.ExpressApp.Web.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Layout;
using DevExpress.ExpressApp.EasyTest;
namespace DevExpress.ExpressApp.Web.TestScripts {
	public class TestScriptsController : WindowController, IControlsEnumeration {
		private class TestItemRegisterData {
			Frame webFrame;
			string prefix;
			bool isRegister;
			public TestItemRegisterData(Frame webFrame, string prefix, bool isRegister) {
				this.webFrame = webFrame;
				this.prefix = prefix;
				this.isRegister = isRegister;
			}
			public Frame WebFrame {
				get {
					return webFrame;
				}
			}
			public string Prefix {
				get {
					return prefix;
				}
			}
			public bool IsRegister {
				get {
					return isRegister;
				}
			}
		}
		private bool isRegisterControlsComplited = false;
		private TestScriptsManager testScriptsManager;
		private Dictionary<object, TestItemRegisterData> deferredItems = new Dictionary<object, TestItemRegisterData>();
		private Dictionary<ITestable, int> deferredControls = new Dictionary<ITestable, int>();
		private Dictionary<ITestable, int> allControls = new Dictionary<ITestable, int>();
		private void UnsubscrubeFromDynamicContainersTemplate() {
			IFrameTemplate template = Frame.Template;
			if (template != null && template is IDynamicContainersTemplate) {
				((IDynamicContainersTemplate)template).ActionContainersChanged -= new EventHandler<ActionContainersChangedEventArgs>(dynamicContainersTemplate_ActionContainersChanged);
				deferredItems.Remove(template);
			}
		}
		private void SubscrubeFromDynamicContainersTemplate() {
			SubscrubeFromDynamicContainersTemplate(Frame, Frame.Template, null, true);
		}
		private void SubscrubeFromDynamicContainersTemplate(Frame frame, IFrameTemplate template, string prefix, bool isRegister) {
			if(template != null && template is IDynamicContainersTemplate) {
				((IDynamicContainersTemplate)template).ActionContainersChanged += new EventHandler<ActionContainersChangedEventArgs>(dynamicContainersTemplate_ActionContainersChanged);
				deferredItems[template] = new TestItemRegisterData(frame, prefix, isRegister);
			}
		}
		private void dynamicContainersTemplate_ActionContainersChanged(object sender, ActionContainersChangedEventArgs e) {
			if(isRegisterControlsComplited && e.ChangedType == ActionContainersChangedType.Added) {
				TestItemRegisterData registerData = null;
				if(deferredItems.TryGetValue(sender, out registerData)) {
					RegisterActionContainers(registerData.WebFrame, e.ActionContainers, registerData.Prefix, registerData.IsRegister);
				}
			}
		}
		private void TestScriptsController_TemplateChanged(object sender, EventArgs e) {
			SubscrubeFromDynamicContainersTemplate();
		}
		private void TestScriptsController_TemplateChanging(object sender, EventArgs e) {
			UnsubscrubeFromDynamicContainersTemplate();
		}
		private string CombineWithPrefix(string prefix, string str) {
			return string.IsNullOrEmpty(prefix) ? str : string.Join(".", new string[] { prefix, str });
		}
		private void RegisterFrameScripts(Frame webFrame, string prefix, bool isRegister) {
			if(webFrame != null && webFrame.View != null) {
				testScriptsManager.AddComment(string.Format("ViewID: {0}. CurrentObject: {1}", webFrame.View.Id, webFrame.View.CurrentObject != null ? System.Web.HttpUtility.HtmlEncode(webFrame.View.CurrentObject.ToString()) : "<NULL>"));
			}
			IFrameTemplate template = webFrame.Template;
			if(template != null) {
				RegisterActionContainers(webFrame, template.GetContainers(), prefix, isRegister);
			}
			DetailView detailView = webFrame.View as DetailView;
			if(detailView != null) {
				RegisterDetailViewEditorsScripts(webFrame, detailView, prefix, isRegister);
			}
			ListView listView = webFrame.View as ListView;
			if(listView != null) {
				RegisterListViewEditorsScripts(webFrame, listView, prefix, isRegister);
			}
			DashboardView dashboardView = webFrame.View as DashboardView;
			if(dashboardView != null) {
				foreach(ViewItem testControl in dashboardView.GetItems<ViewItem>()) {
					RegisterViewItemEditorsScripts(webFrame, testControl, prefix, isRegister);
				}
			}
		}
		private void RegisterViewItemEditorsScripts(Frame webFrame, ViewItem viewItem, string prefix, bool isRegister) {
			if(viewItem.Control == null) {
				deferredItems[viewItem] = new TestItemRegisterData(webFrame, prefix, isRegister);
				viewItem.ControlCreated += new EventHandler<EventArgs>(testControl_ControlCreated);
			}
			else if(viewItem is ListPropertyEditor) {
				RegisterFrameScripts(((ListPropertyEditor)viewItem).Frame, CombineWithPrefix(prefix, viewItem.Caption), isRegister);
				if(((ListPropertyEditor)viewItem).Frame.Template != null) {
					IFrameTemplate viewItemFrameTemplate = ((ListPropertyEditor)viewItem).Frame.Template;
					SubscrubeFromDynamicContainersTemplate(webFrame, viewItemFrameTemplate, CombineWithPrefix(prefix, viewItem.Caption), isRegister);
				}
			}
			else if(viewItem is DetailPropertyEditor) {
				if(((DetailPropertyEditor)viewItem).DetailView != null) {
					RegisterFrameScripts(((DetailPropertyEditor)viewItem).Frame, CombineWithPrefix(prefix, viewItem.Caption), isRegister);
					if(((DetailPropertyEditor)viewItem).Frame.Template != null) {
						IFrameTemplate viewItemFrameTemplate = ((DetailPropertyEditor)viewItem).Frame.Template;
						SubscrubeFromDynamicContainersTemplate(webFrame, viewItemFrameTemplate, CombineWithPrefix(prefix, viewItem.Caption), isRegister);
					}
				}
			}
			else if(viewItem is ActionContainerViewItem) {
				ActionContainerHolder holder = viewItem.Control as ActionContainerHolder;
				RegisterActionContainers(webFrame, holder.ActionContainers, prefix, isRegister);
			}
			else {
				if(viewItem is DashboardViewItem) {
					RegisterFrameScripts(((DashboardViewItem)viewItem).Frame, CombineWithPrefix(prefix, viewItem.Caption), isRegister);
				}
				else {
					if(viewItem is ITestable) {
						RegisterControlCore((ITestable)viewItem, ((ITestable)viewItem).TestControlType, prefix, isRegister);
					}
				}
			}
		}
		private void testControl_ControlCreated(object sender, EventArgs e) {
			TestItemRegisterData testRegisterData = null;
			if(deferredItems.TryGetValue(sender, out testRegisterData)) {
				if(sender is ViewItem) {
					ViewItem viewItem = (ViewItem)sender;
					if(testRegisterData != null) {
						RegisterViewItemEditorsScripts(testRegisterData.WebFrame, viewItem, testRegisterData.Prefix, testRegisterData.IsRegister);
						viewItem.ControlCreated -= testControl_ControlCreated;
						deferredItems.Remove(viewItem);
					}
				}
			}
		}
		private void RegisterDetailViewEditorsScripts(Frame webFrame, DetailView detailView, string prefix, bool isRegister) {
			foreach(ITestable testControl in detailView.GetItems<ITestable>()) {
				if(testControl.TestControl != null || testControl is ITestableEx) {
					if(testControl is ViewItem && ((ViewItem)testControl).Control == null) {
						deferredItems[(ViewItem)testControl] = new TestItemRegisterData(webFrame, prefix, isRegister);
						((ViewItem)testControl).ControlCreated += new EventHandler<EventArgs>(testControl_ControlCreated);
						continue;
					}
					RegisterControlCore(testControl, testControl.TestControlType, prefix, isRegister);
				}
			} 
			foreach(ITestableContainer testContainer in detailView.GetItems<ITestableContainer>()) {
				foreach(ITestable testControl in testContainer.GetTestableControls()) {
					RegisterControlCore(testControl, testControl.TestControlType, prefix, isRegister);
				}
				deferredItems[testContainer] = new TestItemRegisterData(null, prefix, isRegister);
				testContainer.TestableControlsCreated += new EventHandler(testableContainer_TestableControlsCreated);
			}
			foreach(ViewItem testControl in detailView.GetItems<ViewItem>()) {
				RegisterViewItemEditorsScripts(webFrame, testControl, prefix, isRegister);
			}
			RegisterLayoutTestControls(detailView, isRegister);
		}
		private void RegisterLayoutTestControls(DetailView detailView, bool isRegister) {
			WebLayoutManager layoutManager = detailView.LayoutManager as WebLayoutManager;
			if(layoutManager != null) {
				foreach(ITestable testable in layoutManager.GetTestableControls()) {
					RegisterControlCore(testable, testable.TestControlType, null, isRegister);
				}
			}
		}
		private void RegisterListViewEditorsScripts(Frame webFrame, ListView listView, string prefix, bool isRegister) {
			if(listView.Editor is ITestable) {
				ITestable testControl = (ITestable)listView.Editor;
				RegisterControlCore(testControl, testControl.TestControlType, prefix, isRegister);
			}
			if(listView.Editor is ITestableContainer) {
				ITestableContainer testableContainer = (ITestableContainer)listView.Editor;
				foreach(ITestable testControl in testableContainer.GetTestableControls()) {
					RegisterControlCore(testControl, testControl.TestControlType, prefix, isRegister);
				}
				deferredItems[testableContainer] = new TestItemRegisterData(null, prefix, isRegister);
				testableContainer.TestableControlsCreated += new EventHandler(testableContainer_TestableControlsCreated);
			}
			if(listView.Editor is IDetailFramesContainer) {
				IEnumerable<DetailFrameInfo> detailFramesInfo = (listView.Editor as IDetailFramesContainer).GetDetailFramesInfo();
				foreach(DetailFrameInfo detailFrameInfo in detailFramesInfo) {
					Frame frame = detailFrameInfo.DetailFrame;
					RegisterDetailViewEditorsScripts(frame, frame.View as DetailView, "DetailRow" + detailFrameInfo.FrameIndex.ToString() + prefix, isRegister);
				}
			}
		}
		private void testableContainer_TestableControlsCreated(object sender, EventArgs e) {
			ITestableContainer testableContainer = (ITestableContainer)sender;
			TestItemRegisterData testItemRegisterData = null;
			if(deferredItems.TryGetValue(testableContainer, out testItemRegisterData)) {
				foreach(ITestable testControl in testableContainer.GetTestableControls()) {
					RegisterControlCore(testControl, testControl.TestControlType, testItemRegisterData.Prefix, testItemRegisterData.IsRegister);
				}
				testableContainer.TestableControlsCreated -= testableContainer_TestableControlsCreated;
				deferredItems.Remove(testableContainer);
			}
		}
		private void RegisterTestContainerControlsInternal(Frame webFrame, string actionPrefix, ITestableContainer testableContainer, bool isRegister) {
			foreach(ITestable testableControl in testableContainer.GetTestableControls()) {
				ActionBaseItem actionItem = testableControl as ActionBaseItem;
				if(actionItem == null ||
					(actionItem.Action.Active
					)
					) {
					RegisterControlCore(testableControl, testableControl.TestControlType, actionPrefix, ((IActionContainer)testableContainer).ContainerId, isRegister);
					if(testableControl is ITestableContainer) {
						RegisterTestContainerControls(webFrame, actionPrefix, testableControl as ITestableContainer, isRegister);
					}
				}
			}
		}
		private void actionTestableContainer_TestableControlsCreated(object sender, EventArgs e) {
			ITestableContainer testableContainer = (ITestableContainer)sender;
			TestItemRegisterData testItemRegisterData = null;
			if(deferredItems.TryGetValue(testableContainer, out testItemRegisterData)) {
				RegisterTestContainerControlsInternal(testItemRegisterData.WebFrame, testItemRegisterData.Prefix, testableContainer, testItemRegisterData.IsRegister);
				testableContainer.TestableControlsCreated -= actionTestableContainer_TestableControlsCreated;
				deferredItems.Remove(testableContainer);
			}
		}
		private void RegisterTestContainerControls(Frame webFrame, string actionPrefix, ITestableContainer testableContainer, bool isRegister) {
			if(testableContainer != null) {
				RegisterTestContainerControlsInternal(webFrame, actionPrefix, testableContainer, isRegister);
				deferredItems[testableContainer] = new TestItemRegisterData(webFrame, actionPrefix, isRegister);
				testableContainer.TestableControlsCreated += new EventHandler(actionTestableContainer_TestableControlsCreated);
				ITestable testableControl_ = testableContainer as ITestable;
				if(testableControl_ != null) {
					RegisterControlCore(testableControl_, testableControl_.TestControlType, null, isRegister);
				}
			}
		}
		private void RegisterActionContainers(Frame webFrame, IEnumerable<IActionContainer> containers, string actionPrefix, bool isRegister) {
			foreach(IActionContainer container in containers) {
				ITestableContainer testableControlContainer = container as ITestableContainer;
				if(testableControlContainer != null) {
					RegisterTestContainerControls(webFrame, actionPrefix, testableControlContainer, isRegister);
				} else {
					ITestable testableControl = container as ITestable;
					if(testableControl != null) {
						RegisterControlCore(testableControl, testableControl.TestControlType, actionPrefix, container.ContainerId, isRegister);
					}
				}
			}
		}
		private void RegisterCustomControlScripts(WebWindow webWindow, bool isRegister) {
			ITestable testControl = webWindow.ScrollControl as ITestable;
			if(testControl != null) {
				RegisterControlCore(testControl, testControl.TestControlType, "", isRegister);
			}
			Page page = webWindow.Template as Page;
			if(page != null) {
				BrowserNavigation browserNavigation = new BrowserNavigation();
				browserNavigation.ID = "BrowserNavigation";
				if(isRegister) {
					testScriptsManager.RegisterControl(browserNavigation);
				}
				if(page.FindControl(browserNavigation.ID) == null) {
					page.Controls.Add(browserNavigation);
				}
			}
		}
		private void RegisterControlCore(ITestable testableControl, TestControlType testControlType, string caption, string fullCaption) {
			if(!deferredControls.ContainsKey(testableControl)) {
				int index = testScriptsManager.RegisterControl(testableControl, testControlType, caption, fullCaption);
				deferredControls[testableControl] = index;
				allControls[testableControl] = index;
				testableControl.ControlInitialized += new EventHandler<ControlInitializedEventArgs>(testableControl_ControlInitialized);
			}
		}
		private void testableControl_ControlInitialized(object sender, ControlInitializedEventArgs e) {
			ITestable testableControl = (ITestable)sender;
			if(allControls.ContainsKey(testableControl)) { 
				testScriptsManager.UpdateClientControl(allControls[testableControl], testableControl);
				if(e.Control != null) {
					if(!e.Control.Visible) {
						RemoveTestControl(testableControl);
					}
				}
				deferredControls.Remove(testableControl);
			}
		}
		private void RegisterControlCore(ITestable testableControl, TestControlType testControlType, string prefix, bool isRegister) {
			AddTestControlToFound(testableControl, prefix);
			if(isRegister) {
				RegisterControlCore(testableControl, testControlType, testableControl.TestCaption, CombineWithPrefix(prefix, testableControl.TestCaption));
			}
		}
		private void RegisterControlCore(ITestable testableControl, TestControlType testControlType, string prefix, string containerId, bool isRegister) {
			AddTestControlToFound(testableControl, prefix);
			if(isRegister) {
				string caption = CombineWithPrefix(prefix, testableControl.TestCaption);
				string fullCaption = CombineWithPrefix(prefix, CombineWithPrefix(containerId, testableControl.TestCaption));
				RegisterControlCore(testableControl, testControlType, caption, fullCaption);
			}
		}
		private void AddTestControlToFound(ITestable testableControl, string prefix) {
			object control = testableControl;
			if(testableControl is ActionBaseItem) {
				control = ((ActionBaseItem)testableControl).Action;
			}
			foundControls[control.GetHashCode()] = CombineWithPrefix(prefix, testableControl.TestCaption);
		}
		private void RemoveTestControl(ITestable testableControl) {
			testScriptsManager.RemoveTestControl(allControls[testableControl]);
			object control = testableControl;
			if(testableControl is ActionBaseItem) {
				control = ((ActionBaseItem)testableControl).Action;
			}
			if(control != null) {
				foundControls.Remove(control.GetHashCode());
			}
		}
		private void RegisterControls() {
			WebWindow webWindow = (WebWindow)Frame;
			Page page = webWindow.Template as Page;
			RegisterControls(page);
			if(!page.IsCallback) {
			testScriptsManager.AllControlRegistered("");
		}
		}
		private void webWindow_PagePreRender(object sender, EventArgs e) {
			RegisterControls();
		}
		private void RegisterControls(Page page) {
			if(page != null) {
				page.Unload += new EventHandler(page_Unload); 
			}
			SubscrubeFromDynamicContainersTemplate();
			RegisterControls((WebWindow)Frame, true);
			isRegisterControlsComplited = true;
		}
		private void RegisterControls(WebWindow webWindow, bool isRegister) {
			Clear();
			testScriptsManager.Clear();
			foundControls.Clear();
			RegisterFrameScripts(webWindow, null, isRegister);
			RegisterCustomControlScripts(webWindow, isRegister);
		}
		private void UnsubscrubeFromDeferredItem(object obj) {
			if(obj is ViewItem) {
				((ViewItem)obj).ControlCreated -= testControl_ControlCreated;
			}
			else if(obj is IDynamicContainersTemplate) {
				((IDynamicContainersTemplate)obj).ActionContainersChanged -= dynamicContainersTemplate_ActionContainersChanged;
			}
			else if(obj is ITestableContainer) {
				((ITestableContainer)obj).TestableControlsCreated -= testableContainer_TestableControlsCreated;
				((ITestableContainer)obj).TestableControlsCreated -= actionTestableContainer_TestableControlsCreated;
			}
		}
#if DebugTest
		public
#else
		private 
#endif 
		void Clear() {
			foreach(object deferredItem in deferredItems.Keys) {
				UnsubscrubeFromDeferredItem(deferredItem);
			}
			deferredItems.Clear();
			deferredControls.Clear();
			foreach(ITestable testable in allControls.Keys) {
				testable.ControlInitialized -= testableControl_ControlInitialized;
			}
			allControls.Clear();
		}
		private void page_Unload(object sender, EventArgs e) {
			Clear();
			isRegisterControlsComplited = false;
		}
		protected override void OnActivated() {
			base.OnActivated();
			WebWindow window = this.Frame as WebWindow;
			if(window != null && TestScriptsManager.EasyTestEnabled) {
				testScriptsManager = new TestScriptsManager(Frame as WebWindow);
				window.ControlsCreating += TestScriptsController_ControlsCreating;
				window.PagePreRender += webWindow_PagePreRender;
				window.TemplateChanging += TestScriptsController_TemplateChanging;
				window.TemplateChanged += TestScriptsController_TemplateChanged;
			}
		}
		void TestScriptsController_ControlsCreating(object sender, EventArgs e) {
			Page page = (Page)sender;
			if(page.IsCallback) {
				page.LoadComplete += TestScriptsController_LoadComplete;
			}
		}
		void TestScriptsController_LoadComplete(object sender, EventArgs e) {
			RegisterControls(sender as Page);
			((Page)sender).LoadComplete -= TestScriptsController_LoadComplete;
		}
		protected override void OnDeactivated() {
			base.OnDeactivated();
			WebWindow window = this.Frame as WebWindow;
			if(window != null && TestScriptsManager.EasyTestEnabled) {
				window.ControlsCreating -= TestScriptsController_ControlsCreating;
				window.PagePreRender -= webWindow_PagePreRender;
				window.TemplateChanging -= TestScriptsController_TemplateChanging;
				window.TemplateChanged -= TestScriptsController_TemplateChanged;
				UnsubscrubeFromDynamicContainersTemplate();
			}
			testScriptsManager = null;
		}
		protected override void Dispose(bool disposing) {
			Clear();
			base.Dispose(disposing);
			if(disposing == true) {
				WebWindow window = this.Frame as WebWindow;
				if(this.Frame is WebWindow && TestScriptsManager.EasyTestEnabled) {
					window.PagePreRender -= webWindow_PagePreRender;
				}
				testScriptsManager = null;
			}
		}
		Dictionary<int, string> foundControls = new Dictionary<int, string>();
		public IDictionary<int, string> CollectAllControls() {
			return foundControls;
		}
#if DebugTest
		public TestScriptsManager TestScriptsManager {
			get {
				return testScriptsManager;
			}
		}
#endif
	}
}
