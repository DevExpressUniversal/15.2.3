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
using System.Collections;
using System.Collections.Generic;
using DevExpress.ExpressApp.EasyTest;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.ScriptRecorder {
	public class ControlNameHelper {
		private Dictionary<Frame, string> rootFrames = new Dictionary<Frame, string>();
		public static ControlNameHelper Instance {
			get {
				IValueManager<ControlNameHelper> manager = ValueManager.GetValueManager<ControlNameHelper>("ScriptRecorder_ControlNameHelper");
				if(manager.Value == null) {
					manager.Value = new ControlNameHelper();
				}
				return manager.Value;
			}
		}
		private void frame_Disposed(object sender, EventArgs e) {
			rootFrames.Remove((Frame)sender);
		}
		private IList<Frame> GetFramePaths(View targetView) {
			IList<Frame> frames = new List<Frame>();
			foreach(Frame frame in rootFrames.Keys) {
				if(frame.View == targetView) {
					frames.Add(frame);
				} else {
					if(GetFramePaths(frame.View, targetView, frames)) {
						frames.Insert(0, frame);
					}
				}
			}
			return frames;
		}
		private bool GetFramePaths(View currentView, View targetView, IList<Frame> frames) {
			if(currentView is CompositeView) {
				foreach(IFrameContainer frameContainer in ((CompositeView)currentView).GetItems<IFrameContainer>()) {
					if(frameContainer.Frame != null && frameContainer.Frame.View != null) {
						View processedView = frameContainer.Frame.View;
						if(processedView == targetView) {
							frames.Add(frameContainer.Frame);
							return true;
						}
						if(GetFramePaths(processedView, targetView, frames)) {
							frames.Insert(0, frameContainer.Frame);
							return true;
						}
					}
				}
			}
			return false;
		}
		private void CheckActiveTabForNestedView(View targetView, Frame targetFrame, Frame rootFr) {
			if(targetFrame != rootFr) {
				if(targetFrame != null && rootFr.View.Model is IModelDetailView) {
					string tabName = GetActiveTabFullName((IModelDetailView)rootFr.View.Model, "View", targetView.Id);
					if(tabName != null) {
						SetCurrentTab(rootFr, tabName);
					}
				}
			}
		}
		private IModelLayoutViewItem GetLayoutItem(IEnumerable list, string itemId) {
			IModelLayoutViewItem result = null;
			foreach(ModelNode item in list) {
				if(item is IModelLayoutViewItem && item.Id == itemId) {
					result = (IModelLayoutViewItem)item;
					break;
				} else {
					IEnumerable childList = item as IEnumerable;
					if(childList != null) {
						result = GetLayoutItem(childList, itemId);
					}
				}
				if(result != null) {
					break;
				}
			}
			return result;
		}
		private string GetAttributeValue(string attributeName, IModelNode node) {
			string result = null;
			if(attributeName == "View") {
				IModelView viewInfo = node.GetValue<IModelView>(attributeName);
				if(viewInfo != null) {
					result = viewInfo.Id;
				}
			}
			else {
				if(attributeName == "Id") {
					result = node.GetValue<string>(attributeName);
				}
			}
			return result;
		}
		protected string GetFullNameCore(Frame frame, object control) {
			string result = null;
			IControlsEnumeration controlFinder = null;
			foreach(Controller controller in frame.Controllers.Values) {
				if(controller is IControlsEnumeration && controller.Active) {
					controlFinder = (IControlsEnumeration)controller;
					break;
				}
			}
			if(controlFinder != null) {
				IDictionary<int, string> foundControls = controlFinder.CollectAllControls();
				foreach(int contrlId in foundControls.Keys) {
					if(contrlId == control.GetHashCode()) {
						foundControls.TryGetValue(contrlId, out result);
					}
				}
			}
			return result;
		}
		public bool IsParentTabbedGroup(IModelViewLayoutElement item) {
			IModelNode parent = item;
			while(parent != null) {
				if(parent is IModelTabbedGroup) {
					return true;
				}
				parent = parent.Parent;
			}
			return false;
		}
		public string GetActiveTabFullName(IModelDetailView modelView, string attributeName, string attributeValue) {
			IModelViewItem item = null;
			foreach(IModelViewItem detailViewItem in modelView.Items) {
				if(detailViewItem is IModelPropertyEditor) {
					string valueToString = GetAttributeValue(attributeName, detailViewItem);
					if(valueToString != null && valueToString == attributeValue) {
						item = detailViewItem;
						break;
					}
				}
			}
			if(item == null) {
				IModelDetailView viewInfo = modelView.Application.Views[attributeValue] as IModelDetailView;
				if(viewInfo != null) {
					foreach(IModelViewItem item2 in modelView.Items) {
						if(item2 is IModelMemberViewItem) {
							if(((IModelMemberViewItem)item2).ModelMember.Type == viewInfo.ModelClass.TypeInfo.Type) {
								item = item2;
								break;
							}
						}
					}
				}
			}
			string result = null;
			if(item != null) {
				IModelLayoutViewItem layoutItem = GetLayoutItem(modelView.Layout, item.Id);
				if(layoutItem != null && IsParentTabbedGroup(layoutItem)) {
					IModelNode parent = layoutItem.Parent;
					bool isChildNodeLayoutItem = true;
					while(parent != null && parent.Parent != null) {
						if(parent is IModelTabbedGroup || (!(parent.Parent is IModelTabbedGroup) && !isChildNodeLayoutItem)) {
							parent = parent.Parent;
							continue;
						}
						isChildNodeLayoutItem = false;
						if(parent.Parent is IModelViewLayout) {
							break;
						}
						if(parent.Parent is IModelLayoutGroup) {
							parent = parent.Parent;
							continue;
						}
						string caption = parent is IModelLayoutElementWithCaption ? ((IModelLayoutElementWithCaption)parent).Caption : "";
						result = !string.IsNullOrEmpty(result) ? caption + "." + result : caption;
						parent = parent.Parent;
					}
				}
			}
			return result;
		}
		public void CheckActiveTabForPropertyEditor(View targetView, string editorId) {
			Frame editorParentFrame = null;
			foreach(Frame frame in rootFrames.Keys) {
				if(frame.View == targetView) {
					editorParentFrame = frame;
					break;
				}
			}
			if(editorParentFrame != null && editorParentFrame.View is DetailView) {
				string tabName = GetActiveTabFullName((IModelDetailView)editorParentFrame.View.Model, "Id", editorId);
				if (tabName != null) {
					SetCurrentTab(editorParentFrame, tabName);
				}
			}
		}
		public void SetCurrentTab(Frame frame, string curretnTabName) {
			string activeTabName = null;
			rootFrames.TryGetValue(frame, out activeTabName);
			if(activeTabName == null || activeTabName != curretnTabName) {
				rootFrames[frame] = curretnTabName;
				string[] tabs = curretnTabName.Split('.');
				bool isOldTabPath = true;
				for(int counter = 0; counter < tabs.Length; counter++) {
					if(activeTabName != null) {
						string[] existActiveTabs = activeTabName.Split('.');
						if(existActiveTabs.Length > counter && isOldTabPath) {
							if(existActiveTabs[counter] != tabs[counter]) {
								Logger.Instance.WriteMessage("*Action " + tabs[counter]);
								isOldTabPath = false;
							}
						} else {
							Logger.Instance.WriteMessage("*Action " + tabs[counter]);
						}
					} else {
						Logger.Instance.WriteMessage("*Action " + tabs[counter]);
					}
				}
			}
		}
		public void RegisterFrame(Frame frame) {
			if(!(frame is NestedFrame) && !rootFrames.ContainsKey(frame)) {
				frame.Disposed += new EventHandler(frame_Disposed);
				rootFrames.Add(frame, string.Empty);
			}
		}
		public string GetFullName(View targetView, string postfix, object control) {
			string result = null;
			result = GetFullControlName(targetView, control);
			return string.IsNullOrEmpty(result) ? postfix : result; 
		}
		public string GetFullControlName(View targetView, object control) {
			IList<Frame> framePaths = GetFramePaths(targetView);
			if(framePaths.Count > 0) {
				Frame rootFrame = framePaths[0];
				Frame targetFrame = framePaths[framePaths.Count - 1];
				if(framePaths.Count > 1) {
					CheckActiveTabForNestedView(targetView, targetFrame, framePaths[framePaths.Count - 2]);
				}
				if(control == null) {
					return String.Join(".", Enumerator.ToArray<string>(Enumerator.Convert<Frame, string>(framePaths, inParam => { return inParam.View.Caption; })), 1, framePaths.Count - 1);
				} else {
					return GetFullNameCore(rootFrame, control);
				}
			}
			return null;
		}
		public string GetFullViewName(View targetView) {
			return GetFullControlName(targetView, null);
		}
#if DebugTest
		public Dictionary<Frame, string> DebugTest_RootFrames {
			get { return rootFrames; }
		}
#endif
	}
}
