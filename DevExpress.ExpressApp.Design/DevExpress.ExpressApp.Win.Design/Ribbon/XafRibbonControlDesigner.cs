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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using DevExpress.ExpressApp.Templates.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Bars.ActionControls;
using DevExpress.ExpressApp.Win.Templates.Ribbon;
using DevExpress.ExpressApp.Win.Templates.Ribbon.ActionControls;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Design;
namespace DevExpress.ExpressApp.Win.Design.Ribbon {
	public class XafRibbonControlV2Designer : RibbonControlDesigner {
		private IComponentChangeService componentChangeService;
		private void componentChangeService_ComponentRemoved(object sender, ComponentEventArgs e) {
			object component = e.Component;
			if(component is BarItem) {
				RemoveActionControl(component);
			}
			if(component is BarLinkContainerItem) {
				RemoveActionControlContainer(entry => entry is BarLinkActionControlContainer && ((BarLinkActionControlContainer)entry).BarContainerItem == component);
			}
			if(component is RibbonPageGroup) {
				RemoveActionControlContainer(entry => entry is RibbonGroupActionControlContainer && ((RibbonGroupActionControlContainer)entry).RibbonGroup == component);
			}
		}
		private void RemoveActionControl(object nativeControl) {
			List<IActionControl> toRemove = Ribbon.ActionControls.FindAll(entry => entry.NativeControl == nativeControl);
			foreach(IActionControl control in toRemove) {
				Ribbon.ActionControls.Remove(control);
				if(control is IComponent) {
					Ribbon.Container.Remove((IComponent)control);
				}
			}
		}
		private void RemoveActionControlContainer(Predicate<IActionControlContainer> match) {
			List<IActionControlContainer> toRemove = Ribbon.ActionContainers.FindAll(match);
			foreach(IActionControlContainer container in toRemove) {
				Ribbon.ActionContainers.Remove(container);
				if(container is IComponent) {
					Ribbon.Container.Remove((IComponent)container);
				}
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			componentChangeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(componentChangeService != null) {
				componentChangeService.ComponentRemoved += componentChangeService_ComponentRemoved;
			}
		}
		protected override void ShowDesignerForm(RibbonControl ribbon) {
			if(ribbon == null) return;
			IUIService srv = Component.Site.GetService(typeof(IUIService)) as IUIService;
			using(XafRibbonEditorForm form = new XafRibbonEditorForm()) {
				form.InitEditingObject(ribbon);
				form.ShowDialog(srv == null ? null : srv.GetDialogOwnerWindow());
			}
		}
		protected override void Dispose(bool disposing) {
			if(componentChangeService != null) {
				componentChangeService.ComponentRemoved -= componentChangeService_ComponentRemoved;
				componentChangeService = null;
			}
			base.Dispose(disposing);
		}
		protected override void CreateRibbonDesignerActionLists(DesignerActionListCollection list) {
			list.Add(new XafRibbonDesignerActionList(Component));
		}
		protected new XafRibbonControlV2 Ribbon {
			get { return (XafRibbonControlV2)base.Ribbon; }
		}
	}
}
