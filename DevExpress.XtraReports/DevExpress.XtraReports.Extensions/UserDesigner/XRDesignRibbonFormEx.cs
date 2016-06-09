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

using System.ComponentModel;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraReports.UserDesigner {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class XRDesignRibbonFormEx : XRDesignFormExBase {
		RibbonControl ribbonControl;
		RibbonStatusBar ribbonStatusBar;
		XRDesignRibbonController designRibbonController;
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignRibbonFormExDesignRibbonController")]
#endif
		public XRDesignRibbonController DesignRibbonController { get { return designRibbonController; } }
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignRibbonFormExRibbonControl")]
#endif
		public RibbonControl RibbonControl { get { return ribbonControl; } }
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignRibbonFormExRibbonStatusBar")]
#endif
		public RibbonStatusBar RibbonStatusBar { get { return ribbonStatusBar; } }
		public XRDesignRibbonFormEx() {
			CreateDesignDocManager();
			InitDesignManagers();
			ShowToolBoxPanel();
		}
		public override void SetWindowVisibility(DesignDockPanelType designDockPanels, bool visible) {
			SetWindowVisibility(DesignDockManager, designDockPanels, visible);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(designRibbonController != null) {
					designRibbonController.XRDesignPanel = null;
					designRibbonController.Dispose();
					designRibbonController = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override void CreateDesignDocManager() {
			DesignDockManager = XRDesignFormExBase.CreateDesignDocManager(this, DesignPanel, true);
		}
		void InitDesignManagers() {
			SuspendLayout();
			InitDesignDockManager();
			ribbonControl = new RibbonControl();
			ribbonStatusBar = new RibbonStatusBar();
			ribbonStatusBar.Ribbon = ribbonControl;
			Controls.Add(ribbonControl);
			Controls.Add(ribbonStatusBar);
			designRibbonController = new XRDesignRibbonController();
			designRibbonController.XRDesignDockManager = DesignDockManager;
			designRibbonController.Initialize(ribbonControl, ribbonStatusBar);
			designRibbonController.XRDesignPanel = DesignPanel;
			ribbonControl.Controller = BarAndDockingController;
			ResumeLayout(false);
		}
		protected override void InitDesignDockManager() {
			base.InitDesignDockManager();
			DesignDockManager.TopZIndexControls.AddRange(new string[] {
				"DevExpress.XtraBars.Ribbon.RibbonControl",
				"DevExpress.XtraBars.Ribbon.RibbonStatusBar"});
		}
		void ShowToolBoxPanel() {
			ToolBoxDockPanel toolBoxPanel = DesignDockManager[DesignDockPanelType.ToolBox] as ToolBoxDockPanel;
			if(toolBoxPanel != null) {
				SuspendLayout();
				toolBoxPanel.Visibility = DockVisibility.Visible;
				ResumeLayout(false);
				toolBoxPanel.Invalidate();
			}
		}
	}
}
