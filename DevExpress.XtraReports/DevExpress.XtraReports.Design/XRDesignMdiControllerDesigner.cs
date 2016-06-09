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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraReports.UserDesigner;
using System.ComponentModel.Design;
using System.Collections;
using System.ComponentModel;
using DevExpress.Utils.Design;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using DevExpress.Utils.About;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraReports.Design {
	public class XRDesignMdiControllerDesigner : BaseComponentDesignerSimple {
		IComponentChangeService componentChangeService;
		IDesignerHost designerHost;
		IDesignerHost DesignerHost {
			get {
				if(designerHost == null)
					designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
				return designerHost;
			}
		}
		XRDesignMdiController XRDesignMdiController {
			get { return (XRDesignMdiController)Component; }
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList list = new ArrayList(base.AssociatedComponents);
				AddAssociatedComponent(list, typeof(XRDesignBarManager));
				AddAssociatedComponent(list, typeof(XRDesignRibbonController));
				AddAssociatedComponent(list, typeof(XRDesignDockManager));
				return list;
			}
		}
		void AddAssociatedComponent(IList components, Type componentType) {
			IComponent component = DevExpress.XtraPrinting.Native.DesignHelpers.FindSameOrInheritedComponent(DesignerHost.Container, componentType) as IComponent;
			if(component != null)
				components.Add(component);
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
			componentChangeService.ComponentAdded += new ComponentEventHandler(componentChangeService_ComponentAdded);
			componentChangeService.ComponentRemoved += new ComponentEventHandler(componentChangeService_ComponentRemoved);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(componentChangeService != null) {
					componentChangeService.ComponentAdded -= new ComponentEventHandler(componentChangeService_ComponentAdded);
					componentChangeService.ComponentRemoved -= new ComponentEventHandler(componentChangeService_ComponentRemoved);
					componentChangeService = null;
				}
				DestroyComponent(typeof(XRDesignBarManager));
				if(DestroyComponent(typeof(XRDesignRibbonController))) {
					DestroyComponent(typeof(RibbonStatusBar));
					DestroyComponent(typeof(RibbonControl));
					DestroyComponent(typeof(ApplicationMenu));
				}
				DestroyComponent(typeof(XRDesignDockManager));
			}
			base.Dispose(disposing);
		}
		bool DestroyComponent(Type componentType) {
			IComponent component = DevExpress.XtraPrinting.Native.DesignHelpers.FindComponent(DesignerHost.Container, componentType) as IComponent;
			if(component != null) {
				DesignerHost.DestroyComponent(component);
				return true;
			}
			return false;
		}
		void componentChangeService_ComponentRemoved(object sender, ComponentEventArgs e) {
			if(e.Component is IDesignPanelListener)
				XRDesignMdiController.DesignPanelListeners.Remove((IDesignPanelListener)e.Component);
		}
		void componentChangeService_ComponentAdded(object sender, ComponentEventArgs e) {
			TryAddListener(e.Component as IDesignPanelListener);
			if(e.Component is XRTabbedMdiManager)
				XRDesignMdiController.XtraTabbedMdiManager = (XRTabbedMdiManager)e.Component;
		}
		void TryAddListener(IDesignPanelListener listener) {
			if(listener != null && !XRDesignMdiController.DesignPanelListeners.Contains(listener))
				XRDesignMdiController.DesignPanelListeners.Add(listener);
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			System.Windows.Forms.Form form = DesignerHost.RootComponent as System.Windows.Forms.Form;
			XRDesignMdiController.Form = form;
			if(form == null)
				XRDesignMdiController.ContainerControl = DesignerHost.RootComponent as System.Windows.Forms.ContainerControl;
			foreach(IComponent comp in DesignerHost.Container.Components)
				TryAddListener(comp as IDesignPanelListener);
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(About)); 
		}
		void About() {
		}
	}
}
