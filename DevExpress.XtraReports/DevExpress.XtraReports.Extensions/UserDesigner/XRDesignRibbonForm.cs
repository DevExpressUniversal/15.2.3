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
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars;
using DevExpress.XtraTabbedMdi;
using System.ComponentModel;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars.Docking;
using DevExpress.Utils;
using System.Drawing;
using System.Resources;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraReports.UserDesigner {
	public class XRDesignRibbonForm : RibbonForm, IDesignForm {
		FormLayoutSerializer formLayoutSerializer;
		BarAndDockingController controller;
		RibbonControl ribbonControl;
		RibbonStatusBar ribbonStatusBar;
		XRDesignRibbonController designRibbonController;
		XRDesignDockManager designDockManager;
		XRDesignMdiController xrDesignMdiController;
		XRTabbedMdiManager xtraTabbedMdiManager;
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignRibbonFormDesignDockManager")]
#endif
		public XRDesignDockManager DesignDockManager {
			get { return designDockManager; }
			set { designDockManager = value; }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignRibbonFormDesignRibbonController")]
#endif
		public XRDesignRibbonController DesignRibbonController { get { return designRibbonController; } }
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignRibbonFormRibbonControl")]
#endif
		public RibbonControl RibbonControl { get { return ribbonControl; } }
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignRibbonFormRibbonStatusBar")]
#endif
		public RibbonStatusBar RibbonStatusBar { get { return ribbonStatusBar; } }
		public XRDesignMdiController DesignMdiController {
			get { return xrDesignMdiController; }
		}
		protected BarAndDockingController BarAndDockingController {
			get { return controller; }
		}
		public XRDesignRibbonForm() {
			IsMdiContainer = true;
			formLayoutSerializer = CreateFormLayoutSerializer();
			controller = new BarAndDockingController();
			InitializeComponent();
			CreateDesignDocManager();
			InitDesignManagers();
			ShowToolBoxPanel();
			CreateXRDesignMdiController();
		}
		protected virtual FormLayoutSerializer CreateFormLayoutSerializer() {
			return new FormLayoutSerializer(this, "XRDesignRibbonForm");
		}
		void CreateXRDesignMdiController() {
			xrDesignMdiController = new XRDesignMdiController();
			xrDesignMdiController.Form = this;
			xrDesignMdiController.DesignPanelListeners.AddRange(new XRDesignPanelListener[] { new XRDesignPanelListener(this.designRibbonController), new XRDesignPanelListener(this.designDockManager) });
			foreach(DockPanel panel in this.designDockManager.Panels) {
				if(panel is IDesignPanelListener)
					xrDesignMdiController.DesignPanelListeners.Add((IDesignPanelListener)panel);
			}
			xtraTabbedMdiManager = new XRTabbedMdiManager();
			xtraTabbedMdiManager.MdiParent = this;
			xtraTabbedMdiManager.BarAndDockingController = this.controller;
			xrDesignMdiController.XtraTabbedMdiManager = xtraTabbedMdiManager;
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
			ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
			ribbonControl.RibbonStyle = RibbonControlStyle.Office2010;
			ribbonControl.ToolbarLocation = RibbonQuickAccessToolbarLocation.Hidden;
			ribbonControl.ShowPageHeadersMode = ShowPageHeadersMode.ShowOnMultiplePages;
			ribbonControl.Controller = BarAndDockingController;
			ResumeLayout(false);
		}
		protected void InitDesignDockManager() {
			DesignDockManager.Controller = controller;
			controller.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			DesignDockManager.TopZIndexControls.AddRange(new string[] {
				"DevExpress.XtraBars.Ribbon.RibbonControl",
				"DevExpress.XtraBars.Ribbon.RibbonStatusBar"});
		}
		private void InitializeComponent() {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(XRDesignForm));
			this.SuspendLayout();
			this.AccessibleDescription = ((string)(resources.GetObject("$this.AccessibleDescription")));
			this.AccessibleName = ((string)(resources.GetObject("$this.AccessibleName")));
			this.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("$this.Anchor")));
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "XRDesignRibbonForm";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Visible = ((bool)(resources.GetObject("$this.Visible")));
			this.ResumeLayout(false);
		}
		public virtual void SetWindowVisibility(DesignDockPanelType designDockPanels, bool visible) {
			DesignDockManager.SetWindowVisibility(designDockPanels, visible);
		}
		#region OpenReport methods
		public void OpenReport(XtraReport report, UserLookAndFeel lookAndFeel) {
			SetLookAndFeel(lookAndFeel);
			xrDesignMdiController.OpenReport(report);
		}
		public void OpenReport(XtraReport report) {
			OpenReport(report, null);
		}
		public void OpenReport(string fileName, UserLookAndFeel lookAndFeel) {
			SetLookAndFeel(lookAndFeel);
			xrDesignMdiController.OpenReport(fileName);
		}
		public void OpenReport(string fileName) {
			OpenReport(fileName, null);
		}
		#endregion //OpenReport methods
		public XRDesignPanel ActiveDesignPanel {
			get { return xrDesignMdiController.ActiveDesignPanel; }
		}
		protected virtual void SetLookAndFeel(UserLookAndFeel lookAndFeel) {
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			controller.LookAndFeel.ParentLookAndFeel = lookAndFeel;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			formLayoutSerializer.RestoreLayout();
		}
		protected override void OnClosed(EventArgs e) {
			formLayoutSerializer.SaveLayout();
			base.OnClosed(e);
		}
		protected void CreateDesignDocManager() {
			DesignDockManager = XRDesignFormExBase.CreateDesignDocManager(this, null, true);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(xrDesignMdiController != null) {
					xrDesignMdiController.Dispose();
					xrDesignMdiController = null;
				}
				if(xtraTabbedMdiManager != null) {
					xtraTabbedMdiManager.Dispose();
					xtraTabbedMdiManager = null;
				}
				if(formLayoutSerializer != null) {
					formLayoutSerializer.Dispose();
					formLayoutSerializer = null;
				}
				if(controller != null) {
					controller.Dispose();
					controller = null;
				}
				if(designDockManager != null) {
					designDockManager.Controller = null;
					designDockManager.Dispose();
					designDockManager = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
