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
using System.Web.Security;
using System.Web.UI;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Templates.ActionContainers;
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.ExpressApp.Web.Templates;
namespace DevExpress.ExpressApp.Web {
	public class DefaultFrameTemplateFactory : IFrameTemplateFactory {
		private IFrameTemplate CreateFrameTemplateByVirtualPath(string path) {
			return (IFrameTemplate)System.Web.Compilation.BuildManager.CreateInstanceFromVirtualPath(path, typeof(Page));
		}
		#region IFrameTemplateFactory Members
		public IFrameTemplate CreateTemplate(TemplateContext context) {
			IFrameTemplate result = null;
			if(context == TemplateContext.LogonWindow) {
				result = CreateFrameTemplateByVirtualPath(FormsAuthentication.LoginUrl);
			}
			else {
				if(context == TemplateContext.ApplicationWindow || context == TemplateContext.PopupWindow) {
					result = CreateFrameTemplateByVirtualPath(VirtualPathUtility.GetDirectory(FormsAuthentication.DefaultUrl) + WebApplication.DefaultPage);
				}
				else {
					if(context == TemplateContext.NestedFrame) {
						result = (IFrameTemplate)TemplateContentFactory.Instance.CreateTemplateContent(WebWindow.CurrentRequestPage, WebApplication.Instance.Settings, TemplateType.NestedFrameControl);
					}
				}
			}
			return result;
		}
		#endregion
	}
	public abstract class NestedFrameControlBase : TemplateContent, IFrameTemplate, IDynamicContainersTemplate, IViewHolder, IViewDependentControlsHolder {
		private View view;
		private ContextActionsMenu contextMenu;
		private ActionContainerCollection actionContainers = new ActionContainerCollection();
		private bool isInitialized = false;
		protected override void OnInit(EventArgs e) {
			base.OnInit(e);
			SetViewToViewDependentControls();
			isInitialized = true;
		}
		private void SetViewToViewDependentControls() {
			if(view != null) {
				contextMenu.CreateControls(view);
			}
			foreach(IViewDependentControl control in viewDependenControls) {
				control.SetView(view);
			}
			OnViewChanged(view);
		}
		protected abstract ContextActionsMenu CreateContextMenu();
		public override void Dispose() {
			if(contextMenu != null) {
				contextMenu.Dispose();
				contextMenu = null;
			}
			base.Dispose();
		}
		public NestedFrameControlBase() {
			contextMenu = CreateContextMenu();
			actionContainers.AddRange(contextMenu.Containers);
		}
		#region IDynamicContainersTemplate Members
		private void OnActionContainersChanged(ActionContainersChangedEventArgs args) {
			if(ActionContainersChanged != null) {
				ActionContainersChanged(this, args);
			}
		}
		public void RegisterActionContainers(IEnumerable<IActionContainer> actionContainers) {
			IList<IActionContainer> addedContainers = this.actionContainers.TryAdd(actionContainers);
			if(addedContainers.Count > 0) {
				OnActionContainersChanged(new ActionContainersChangedEventArgs(addedContainers, ActionContainersChangedType.Added));
			}
		}
		public void UnregisterActionContainers(IEnumerable<IActionContainer> actionContainers) {
			IList<IActionContainer> removedContainers = new List<IActionContainer>();
			foreach(IActionContainer actionContainer in actionContainers) {
				if(this.actionContainers.Contains(actionContainer)) {
					this.actionContainers.Remove(actionContainer);
					removedContainers.Add(actionContainer);
				}
			}
			if(removedContainers.Count > 0) {
				OnActionContainersChanged(new ActionContainersChangedEventArgs(removedContainers, ActionContainersChangedType.Removed));
			}
		}
		public event EventHandler<ActionContainersChangedEventArgs> ActionContainersChanged;
		#endregion
		#region IFrameTemplate Members
		public ICollection<IActionContainer> GetContainers() {
			return actionContainers.ToArray();
		}
		#endregion
		#region IFrameTemplate Members
		public void SetView(DevExpress.ExpressApp.View view) {
			this.view = view;
			if(isInitialized) {
				SetViewToViewDependentControls();
			}
		}
		#endregion
		#region IViewHolder Members
		public DevExpress.ExpressApp.View View {
			get { return view; }
		}
		#endregion
		#region ISupportViewChanged Members
		protected virtual void OnViewChanged(DevExpress.ExpressApp.View view) {
			if(ViewChanged != null) {
				ViewChanged(this, new TemplateViewChangedEventArgs(view));
			}
		}
		public event EventHandler<TemplateViewChangedEventArgs> ViewChanged;
		#endregion
		List<IViewDependentControl> viewDependenControls = new List<IViewDependentControl>();
		public void Register(IViewDependentControl control) {
			viewDependenControls.Add(control);
		}
	}
}
