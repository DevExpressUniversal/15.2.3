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
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Windows.Forms;
using System.Xml;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Controls;
using DevExpress.ExpressApp.Win.Templates;
using DevExpress.ExpressApp.Win.Templates.ActionContainers;
using DevExpress.ExpressApp.Win.Templates.ActionContainers.Items;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.ExpressApp.Win.EasyTest {
	public class FrameControlFinder {
		class ControlInfo {
			public ControlInfo(object control, string caption, string fullCaption, int uiHashCode, int xafHashCode) {
				Control = control;
				Caption = caption;
				FullCaption = fullCaption;
				UIHashCode = uiHashCode;
				XafHashCode = xafHashCode;
			}
			public object Control { get; private set; }
			public string Caption { get; private set; }
			public string FullCaption { get; private set; }
			public int UIHashCode { get; private set; }
			public int XafHashCode { get; private set; }
			public override string ToString() {
				return string.Format("Caption: {0}, FullCaption: {1}, UIHashCode: {2}, XafHashCode: {3}", Caption, FullCaption, UIHashCode, XafHashCode);
			}
		}
		class DiagnosticInfoXmlBuilder {
			private FrameControlFinder controlFinder;
			public DiagnosticInfoXmlBuilder(FrameControlFinder controlFinder) {
				this.controlFinder = controlFinder;
			}
			public XmlNode CreateDiagnosticInfoNode(Frame frame) {
				XmlDocument doc = new XmlDocument();
				XmlNode result = doc.CreateNode(XmlNodeType.Element, "EasyTest", "");
				IEnumerable<ControlInfo> actionControls = controlFinder.CollectActionControls(frame);
				AppendControlNode(result, "Actions", actionControls);
				IEnumerable<ControlInfo> fieldControls = controlFinder.CollectFieldControls(frame);
				AppendControlNode(result, "Fields", fieldControls);
				AppendStandardControlDiagnosticNode(result, frame.Template as Form);
				return result;
			}
			private void AppendControlNode(XmlNode parent, string nodeName, IEnumerable<ControlInfo> descriptions) {
				XmlNode subNode = parent.OwnerDocument.CreateNode(XmlNodeType.Element, nodeName, "");
				foreach(ControlInfo description in descriptions) {
					XmlNode control = parent.OwnerDocument.CreateNode(XmlNodeType.Element, "Control", "");
					AppendAttribute(control, "Caption", description.Caption);
					AppendAttribute(control, "FullCaption", description.FullCaption);
					subNode.AppendChild(control);
				}
				parent.AppendChild(subNode);
			}
			private void AppendStandardControlDiagnosticNode(XmlNode parent, Form form) {
				XmlNode formNode = parent.OwnerDocument.CreateNode(XmlNodeType.Element, "Form", "");
				if(form != null) {
					AppendControlAttributes(formNode, form);
					foreach(Control childControl in form.Controls) {
						AppendStandardControlDiagnosticNode(formNode, childControl);
					}
				}
				parent.AppendChild(formNode);
			}
			private void AppendStandardControlDiagnosticNode(XmlNode parent, Control control) {
				XmlNode controlNode = parent.OwnerDocument.CreateNode(XmlNodeType.Element, "Control", "");
				AppendControlAttributes(controlNode, control);
				foreach(Control childControl in control.Controls) {
					AppendStandardControlDiagnosticNode(controlNode, childControl);
				}
				parent.AppendChild(controlNode);
			}
			private void AppendControlAttributes(XmlNode controlNode, Control control) {
				AppendAttribute(controlNode, "Type", control.GetType().FullName);
				AppendAttribute(controlNode, "Name", control.Name);
				AppendAttribute(controlNode, "Text", control.Text);
				AppendAttribute(controlNode, "Tag", control.Tag != null ? control.Tag.ToString() : string.Empty);
			}
			private void AppendAttribute(XmlNode targetNode, string name, string value) {
				XmlAttribute attr = targetNode.OwnerDocument.CreateAttribute(name);
				attr.Value = value;
				targetNode.Attributes.Append(attr);
			}
		}
		private IEnumerable<ControlInfo> CollectControls(Frame frame, string controlType) {
			if(controlType == ControlFinder.ControlTypeAction) {
				return CollectActionControls(frame);
			}
			if(controlType == ControlFinder.ControlTypeField) {
				return CollectFieldControls(frame);
			}
			return new ControlInfo[0];
		}
		private void ProcessFrameRecursive(string frameNamePrefix, Frame frame, Action<string, Frame> callback) {
			callback(frameNamePrefix, frame);
			if(frame.View is DetailView || frame.View is DashboardView) {
				CompositeView view = (CompositeView)frame.View;
				foreach(IFrameContainer item in view.GetItems<IFrameContainer>()) {
					Frame innerFrame = item.Frame;
					if(innerFrame != null) {
						ProcessFrameRecursive(frameNamePrefix + ((ViewItem)item).Caption + ".", innerFrame, callback);
					}
				}
			}
			else if(frame.View is ListView) {
				Frame editFrame = ((ListView)frame.View).EditFrame;
				if(editFrame != null) {
					ProcessFrameRecursive(frameNamePrefix, editFrame, callback);
				}
			}
		}
		private void AddControl(ICollection<ControlInfo> controls, object control, string caption, string fullCaption, int uiHashCode, int xafHashCode) {
			controls.Add(new ControlInfo(control, caption, fullCaption, uiHashCode, xafHashCode));
		}
		private void EnumerateBarItemsRecursive(BarManager barManager, Action<RibbonPageGroup> groupCallback, Action<BarItem> barItemCallback) {
			if(barManager is RibbonBarManager) {
				EnumerateBarItemsRecursive(((RibbonBarManager)barManager).Ribbon, groupCallback, barItemCallback);
			}
			else {
				foreach(Bar bar in barManager.Bars) {
					if(bar.Visible) {
						foreach(LinkPersistInfo linkPersistInfo in bar.LinksPersistInfo) {
							EnumerateBarItemsRecursive(linkPersistInfo.Item, barItemCallback);
						}
					}
				}
			}
		}
		private void EnumerateBarItemsRecursive(RibbonControl ribbon, Action<RibbonPageGroup> groupCallback, Action<BarItem> barItemCallback) {
			foreach(RibbonPage page in ribbon.Pages) {
				if(!page.Visible) continue;
				foreach(RibbonPageGroup group in page.Groups) {
					if(!group.Visible) continue;
					groupCallback(group);
					foreach(BarItemLink link in group.ItemLinks) {
						EnumerateBarItemsRecursive(link.Item, barItemCallback);
					}
				}
			}
			foreach(BarItemLink link in ((PopupMenu)ribbon.ApplicationButtonDropDownControl).ItemLinks) {
				EnumerateBarItemsRecursive(link.Item, barItemCallback);
			}
			if(ribbon.StatusBar != null) {
				foreach(BarItemLink link in ribbon.StatusBar.ItemLinks) {
					EnumerateBarItemsRecursive(link.Item, barItemCallback);
				}
			}
		}
		private void EnumerateBarItemsRecursive(BarItem barItem, Action<BarItem> callback) {
			callback(barItem);
			BarLinkContainerItem barLinkContainerItem = barItem as BarLinkContainerItem;
			if(barLinkContainerItem != null) {
				foreach(LinkPersistInfo item in barLinkContainerItem.LinksPersistInfo) {
					EnumerateBarItemsRecursive(item.Item, callback);
				}
			}
		}
		private void AddBarItem(string frameNamePrefix, ICollection<ControlInfo> controls, string containerId, ActionBase action, BarItem barItem, string navigationActionCaption) {
			BarItemLink barItemLink;
			if(TryGetVisibleBarItemLink(barItem, out barItemLink)) {
				string actionCaption = action.Caption;
				string caption = frameNamePrefix + actionCaption;
				if(caption == navigationActionCaption) {
					caption = "Menu." + actionCaption;
				}
				string fullCaption = frameNamePrefix + containerId + "." + actionCaption;
				int hashCode = action.GetHashCode();
				AddControl(controls, barItemLink, caption, fullCaption, hashCode, hashCode);
			}
		}
		private bool TryGetVisibleBarItemLink(BarItem barItem, out BarItemLink barItemLink) {
			foreach(BarItemLink link in barItem.Links) {
				if(link.CanVisible) {
					barItemLink = link;
					return true;
				}
			}
			barItemLink = null;
			return false;
		}
		private ActionCollection CollectActions(Frame frame) {
			ActionCollection actionCollection = new ActionCollection();
			foreach(Controller controller in frame.Controllers) {
				actionCollection.AddRange(controller.Actions);
			}
			return actionCollection;
		}
		private IEnumerable<ControlInfo> CollectActionControls(Frame frame) {
			List<ControlInfo> result = new List<ControlInfo>();
			ProcessFrameRecursive(null, frame, (currentFrameNamePrefix, currentFrame) => {
				string navigationActionCaption = ProcessNavigationActionContainer(currentFrame, result);
				ProcessBarActionContainers(currentFrameNamePrefix, currentFrame, result, navigationActionCaption);
				ProcessBarActions(currentFrameNamePrefix, currentFrame, result, navigationActionCaption);
				ProcessActionContainerViewItems(currentFrameNamePrefix, currentFrame, result);
				ProcessButtonsActionContainers(currentFrameNamePrefix, currentFrame, result);
			});
			return result;
		}
		private string ProcessNavigationActionContainer(Frame frame, ICollection<ControlInfo> controls) {
			if(frame.Template != null) {
				foreach(IActionContainer container in frame.Template.GetContainers()) {
					if(container is NavigationActionContainer && ((NavigationActionContainer)container).Controls.Count > 0) {
						Control control = ((NavigationActionContainer)container).Controls[0];
						ActionBase navAction = ((NavigationActionContainer)container).Actions[0];
						AddControl(controls, control, navAction.Caption, container.ContainerId, control.GetHashCode(), navAction.GetHashCode());
						return navAction.Caption;
					}
				}
			}
			return null;
		}
		private void ProcessBarActionContainers(string frameNamePrefix, Frame frame, ICollection<ControlInfo> controls, string navigationActionCaption) {
			if(frame.Template is Control && ((Control)frame.Template).Visible) {
				BarManager barManager = frame.Template is IBarManagerHolder ? ((IBarManagerHolder)frame.Template).BarManager : null;
				if(barManager != null) {
					ICollection<BarItem> processedBarItems = new HashSet<BarItem>();
					foreach(IActionContainer actionContainer in CollectVisibleActionContainersFromBarManager(barManager)) {
						if(actionContainer is BarLinksHolder) {
							BarActionItemsFactory barActionItemsFactory = BarActionItemsFactoryProvider.GetBarActionItemsFactory(((BarLinksHolder)actionContainer).Manager);
							foreach(ActionBase action in actionContainer.Actions) {
								BarActionBaseItem actionItem = barActionItemsFactory.GetBarItem(action);
								BarItem barItem = actionItem.Control;
								if(actionItem.IsVisible && !processedBarItems.Contains(barItem)) {
									processedBarItems.Add(barItem);
									AddBarItem(frameNamePrefix, controls, actionContainer.ContainerId, action, barItem, navigationActionCaption);
								}
							}
						}
					}
				}
			}
		}
		private IEnumerable<IActionContainer> CollectVisibleActionContainersFromBarManager(BarManager barManager) {
			List<IActionContainer> containers = new List<IActionContainer>();
			EnumerateBarItemsRecursive(barManager,
				group => {
					ActionContainersRibbonPageGroup actionContainersRibbonPageGroup = group as ActionContainersRibbonPageGroup;
					if(actionContainersRibbonPageGroup != null) {
						containers.AddRange(actionContainersRibbonPageGroup.ActionContainers);
					}
				},
				barItem => {
					IActionContainer container = barItem as IActionContainer;
					if(container != null) {
						containers.Add(container);
					}
				}
			);
			return containers;
		}
		private void ProcessBarActions(string frameNamePrefix, Frame frame, ICollection<ControlInfo> controls, string navigationActionCaption) {
			if(frame.Template is Control && ((Control)frame.Template).Visible) {
				BarManager barManager = frame.Template is IBarManagerHolder ? ((IBarManagerHolder)frame.Template).BarManager : null;
				if(barManager != null) {
					IActionControlsSite actionControlsSite = frame.Template as IActionControlsSite;
					if(actionControlsSite != null) {
						Dictionary<BarItem, bool> processedBarItems = new Dictionary<BarItem, bool>();
						EnumerateBarItemsRecursive(barManager, group => { }, barItem => processedBarItems[barItem] = false);
						ActionCollection actions = CollectActions(frame);
						foreach(IActionControlContainer container in actionControlsSite.ActionContainers) {
							foreach(IActionControl actionControl in container.GetActionControls()) {
								ActionBase action = actions.Find(actionControl.ActionId);
								BarItem barItem = actionControl.NativeControl as BarItem;
								bool isProcessed;
								if(action != null && barItem != null && processedBarItems.TryGetValue(barItem, out isProcessed) && !isProcessed) {
									processedBarItems[barItem] = true;
									AddBarItem(frameNamePrefix, controls, container.ActionCategory, action, barItem, navigationActionCaption);
								}
							}
						}
					}
				}
			}
		}
		private void ProcessActionContainerViewItems(string frameNamePrefix, Frame frame, ICollection<ControlInfo> controls) {
			DetailView detailView = frame.View as DetailView;
			if(detailView != null) {
				foreach(ViewItem item in detailView.Items) {
					ActionContainerViewItem actionContainerViewItem = item as ActionContainerViewItem;
					if(actionContainerViewItem != null) {
						ButtonsContainer buttonsContainer = actionContainerViewItem.Control as ButtonsContainer;
						if(buttonsContainer != null) {
							foreach(ButtonsContainersActionItemBase actionItem in buttonsContainer.ActionItems.Values) {
								ActionBase action = actionItem.Action;
								string caption = frameNamePrefix + action.Caption;
								string fullCaption = frameNamePrefix + buttonsContainer.ContainerId + "." + action.Caption;
								int hashCode = action.GetHashCode();
								AddControl(controls, actionItem.Control, caption, fullCaption, hashCode, hashCode);
							}
						}
					}
				}
			}
		}
		private void ProcessButtonsActionContainers(string frameNamePrefix, Frame frame, ICollection<ControlInfo> controls) {
			if(frame.Template != null) {
				foreach(IActionContainer container in frame.Template.GetContainers()) {
					ButtonsContainer buttonsContainer = container as ButtonsContainer;
					if(buttonsContainer != null) {
						foreach(ButtonsContainersActionItemBase actionItem in buttonsContainer.ActionItems.Values) {
							ActionBase action = actionItem.Action;
							if(action.Active && action.Enabled) {
								string caption = action.Caption;
								string fullCaption = frameNamePrefix + container.ContainerId + "." + caption;
								int hashCode = action.GetHashCode();
								AddControl(controls, actionItem.Control, caption, fullCaption, hashCode, hashCode);
							}
						}
					}
				}
			}
		}
		private IEnumerable<ControlInfo> CollectFieldControls(Frame frame) {
			List<ControlInfo> result = new List<ControlInfo>();
			ProcessFrameRecursive(null, frame, (currentFrameNamePrefix, currentFrame) => {
				ProcessPropertyEditors(currentFrameNamePrefix, currentFrame, result);
			});
			return result;
		}
		private void ProcessPropertyEditors(string frameNamePrefix, Frame frame, ICollection<ControlInfo> controls) {
			DetailView detailView = frame.View as DetailView;
			if(detailView != null) {
				foreach(ViewItem item in detailView.Items) {
					PropertyEditor propertyEditor = item as PropertyEditor;
					if(propertyEditor != null) {
						Control control = propertyEditor.Control as Control;
						if(control != null && control.Visible) { 
							string caption = frameNamePrefix + propertyEditor.Caption;
							AddControl(controls, control, caption, caption, control.GetHashCode(), propertyEditor.GetHashCode());
						}
					}
				}
			}
		}
		private object GetControl(string caption, IEnumerable<ControlInfo> controls) {
			List<ControlInfo> compatibleControlDescriptions = new List<ControlInfo>();
			foreach(ControlInfo description in controls) {
				if(description.Caption == caption || description.FullCaption == caption) {
					compatibleControlDescriptions.Add(description);
				}
			}
			if(compatibleControlDescriptions.Count == 0) {
				return null;
			}
			int uiHashCode = compatibleControlDescriptions[0].UIHashCode;
			foreach(ControlInfo description in compatibleControlDescriptions) {
				if(description.UIHashCode != uiHashCode) {
					string[] fullCaptions = compatibleControlDescriptions.Select<ControlInfo, string>((entry) => entry.FullCaption).ToArray();
					throw new DuplicatedControlCaptionException(caption, fullCaptions);
				}
			}
			return compatibleControlDescriptions[0].Control;
		}
		private IDictionary<int, string> GetControlCaptions(IEnumerable<ControlInfo> controls) {
			Dictionary<int, string> result = new Dictionary<int, string>();
			foreach(ControlInfo controlInfo in controls) {
				if(!result.ContainsKey(controlInfo.XafHashCode)) {
					result.Add(controlInfo.XafHashCode, controlInfo.Caption);
				}
			}
			return result;
		}
		public object Find(Frame frame, string controlType, string caption) {
			IEnumerable<ControlInfo> controls = CollectControls(frame, controlType);
			return GetControl(caption, controls);
		}
		public IDictionary<int, string> GetControlCaptions(Frame frame, string controlType) {
			IEnumerable<ControlInfo> controls = CollectControls(frame, controlType);
			return GetControlCaptions(controls);
		}
		public XmlNode CreateDiagnosticInfoNode(Frame frame) {
			return new DiagnosticInfoXmlBuilder(this).CreateDiagnosticInfoNode(frame);
		}
	}
	[Serializable]
	public class DuplicatedControlCaptionException : Exception {
		private string caption;
		private string[] fullCaptions;
		public const string DuplicateMessageString = "Multiple controls with the '{0}' caption are found. To resolve ambiguity, use fully qualified names: '{1}'";
		protected DuplicatedControlCaptionException(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			caption = info.GetString("Caption");
			fullCaptions = info.GetValue("FullCaptions", typeof(string[])) as string[];
		}
		public DuplicatedControlCaptionException(string caption, string[] fullCaptions)
			: base(string.Format(DuplicateMessageString, caption, string.Join(", ", fullCaptions))) {
			this.caption = caption;
			this.fullCaptions = fullCaptions;
		}
		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context) {
			Guard.ArgumentNotNull(info, "info");
			info.AddValue("Caption", caption);
			info.AddValue("FullCaptions", fullCaptions, typeof(string[]));
			base.GetObjectData(info, context);
		}
		public string Caption {
			get { return caption; }
		}
		public string[] FullCaptions {
			get { return fullCaptions; }
		}
	}
}
