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
namespace DevExpress.XtraReports.UserDesigner {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class XRDesignFormEx : XRDesignFormExBase {
		XRDesignBarManager designBarManager;
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignFormExDesignBarManager")]
#endif
		public XRDesignBarManager DesignBarManager { get { return designBarManager; } }
		public XRDesignFormEx() {
			CreateDesignDocManager();
			InitDesignManagers();
		}
		protected override void Dispose( bool disposing )	{
			if( disposing )	{
				if(designBarManager != null) {
					designBarManager.Controller = null;
					designBarManager.Dispose();
					designBarManager = null;
				}
			}
			base.Dispose( disposing );
		}
		public override void SetWindowVisibility(DesignDockPanelType designDockPanels, bool visible) {
			SetWindowVisibility(DesignDockManager, designDockPanels, visible);
		}
		protected override void CreateDesignDocManager() {
			DesignDockManager = XRDesignFormExBase.CreateDesignDocManager(this, DesignPanel, false);
		}
		void InitDesignManagers() {
			SuspendLayout();
			InitDesignDockManager();
			designBarManager = new XRDesignBarManager();
			designBarManager.Controller = BarAndDockingController;
			designBarManager.DockManager = DesignDockManager;
			designBarManager.Form = this;
			designBarManager.Initialize(DesignPanel);
			ResumeLayout(false);
		}
		protected override void InitDesignDockManager() {
			base.InitDesignDockManager();
			DesignDockManager.TopZIndexControls.AddRange(new string[] {
				"DevExpress.XtraBars.BarDockControl",
				"System.Windows.Forms.StatusBar"});
		}
	}
}
