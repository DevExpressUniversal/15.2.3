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
using System.Windows.Forms;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Docking.Design;
using System.Drawing;
using System.Reflection;
using DevExpress.XtraPrinting.Design;
using DevExpress.XtraReports.UserDesigner.Native;
using DevExpress.Utils.Design;
using DevExpress.Utils.About;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Design {
	public class DesignDockManagerDesigner : DockManagerDesigner {
		DesignerVerbCollection baseVerbs;
		XRDesignDockManager XRDesignDockManager { get { return Component as XRDesignDockManager; } }
		public override DesignerVerbCollection Verbs {
			get {
				if(baseVerbs == null)
					baseVerbs = base.Verbs;
				if(DesignDockManagerTestHelper.GetDesignDockPanels(XRDesignDockManager).Length != typeof(DesignDockPanelType).GetFields(BindingFlags.Public | BindingFlags.Static).Length) {
					DesignerVerbCollection verbs = new DesignerVerbCollection();
					verbs.AddRange(baseVerbs);
					verbs.Insert(0, new XRDesignerVerb("Reset Panels", new EventHandler(OnResetPanels), ReportCommand.None));
					return verbs;
				}
				return baseVerbs;
			}
		}
		protected override void OnInitialize() {
			base.OnInitialize();
			BarManager barManager = DesignHelpers.FindInheritedComponent(XRDesignDockManager.Container, typeof(BarManager)) as BarManager;
			if(barManager != null)
				barManager.DockManager = XRDesignDockManager;
			XRDesignRibbonController ribbonController = DesignHelpers.FindSameOrInheritedComponent(XRDesignDockManager.Container, typeof(XRDesignRibbonController)) as XRDesignRibbonController;
			if(ribbonController != null)
				ribbonController.XRDesignDockManager = XRDesignDockManager;
			XRDesignDockManager.Initialize(null, AvailableDockPanelTypes);
			BarsReferencesHelper.AddBarsReferences(this);
		}
		void OnResetPanels(object sender, EventArgs e) {
			XRDesignDockManager.Images.Clear();
			XRDesignDockManager.Initialize(XRDesignDockManager.XRDesignPanel, AvailableDockPanelTypes);
		}
		DesignDockPanelType AvailableDockPanelTypes {
			get {
				DesignDockPanelType panelTypes = DesignDockPanelType.All;
				IDesignerHost host = GetService(typeof(IDesignerHost)) as IDesignerHost;
				if(host != null && host.FirstComponent<XRDesignBarManager>() != null)
					panelTypes &= ~DesignDockPanelType.ToolBox;
				return panelTypes;
			}
		}
		protected override DXAboutActionList GetAboutAction() {
			return new DXAboutActionList(Component, new MethodInvoker(About));
		}
		void About() {
		}
	}
}
