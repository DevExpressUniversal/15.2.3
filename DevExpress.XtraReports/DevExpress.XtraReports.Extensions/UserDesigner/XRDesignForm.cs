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
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel;
using System.ComponentModel.Design;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Localization;
using System.IO;
using System.Resources;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking;
using System.Drawing;
using DevExpress.XtraTabbedMdi;
using System.Windows.Forms;
namespace DevExpress.XtraReports.UserDesigner {
	public class XRDesignForm : XtraForm, IDesignForm {
		FormLayoutSerializer formLayoutSerializer;
		BarAndDockingController controller;
		XRDesignDockManager designDockManager;
		XRDesignBarManager designBarManager;
		XRDesignMdiController xrDesignMdiController;
		XRTabbedMdiManager xtraTabbedMdiManager;
		#region properties
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignFormDesignBarManager")]
#endif
		public XRDesignBarManager DesignBarManager { get { return designBarManager; } }
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignFormDesignDockManager")]
#endif
		public XRDesignDockManager DesignDockManager { 
			get { return designDockManager; } 
			set { designDockManager = value; }
		}
		protected BarAndDockingController BarAndDockingController { 
			get { return controller; } 
		}
		public XRDesignMdiController DesignMdiController {
			get { return xrDesignMdiController; }
		}
		#endregion properties
		public XRDesignForm() {
			IsMdiContainer = true;
			formLayoutSerializer = CreateFormLayoutSerializer();
			controller = new BarAndDockingController();
			InitializeComponent();
			CreateDesignDockManager();
			InitDesignManagers();
			CreateXRDesignMdiController();
		}
		protected virtual FormLayoutSerializer CreateFormLayoutSerializer() {
			return new FormLayoutSerializer(this, "XRDesignForm");
		}
		void CreateXRDesignMdiController() {
			xrDesignMdiController = new XRDesignMdiController();
			xrDesignMdiController.Form = this;
			xrDesignMdiController.DesignPanelListeners.AddRange(new XRDesignPanelListener[] { new XRDesignPanelListener(this.designBarManager), new XRDesignPanelListener(this.designDockManager) });
			foreach(DockPanel panel in this.designDockManager.Panels) {
				if(panel is IDesignPanelListener)
					xrDesignMdiController.DesignPanelListeners.Add((IDesignPanelListener)panel);
			}
			xtraTabbedMdiManager = new XRTabbedMdiManager();
			xtraTabbedMdiManager.MdiParent = this;
			xtraTabbedMdiManager.BarAndDockingController = this.controller;
			xrDesignMdiController.XtraTabbedMdiManager = xtraTabbedMdiManager;
		}
		void InitDesignManagers() {
			SuspendLayout();
			InitDesignDockManager();
			designBarManager = new XRDesignBarManager();
			designBarManager.Controller = BarAndDockingController;
			designBarManager.DockManager = DesignDockManager;
			designBarManager.Form = this;
			designBarManager.Initialize(null);
			if(designBarManager.StatusBar != null && this.SizeGripStyle == System.Windows.Forms.SizeGripStyle.Show)
				designBarManager.StatusBar.OptionsBar.DrawSizeGrip = true;
			ResumeLayout(false);
		}
		protected void CreateDesignDockManager() {
			DesignDockManager = XRDesignFormExBase.CreateDesignDocManager(this, null, false);
		}
		protected override void Dispose(bool disposing) {
			if( disposing )	{
				if(xrDesignMdiController != null) {
					xrDesignMdiController.Dispose();
					xrDesignMdiController = null;
				}
				if(xtraTabbedMdiManager != null) {
					xtraTabbedMdiManager.Dispose();
					xtraTabbedMdiManager = null;
				}
				if(designBarManager != null) {
					designBarManager.Controller = null;
					designBarManager.Dispose();
					designBarManager = null;
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
			base.Dispose( disposing );
		}
		public virtual void SetWindowVisibility(DesignDockPanelType designDockPanels, bool visible) {
			DesignDockManager.SetWindowVisibility(designDockPanels, visible);
		}
		protected void InitDesignDockManager() {
			DesignDockManager.Controller = controller;
			controller.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			DesignDockManager.TopZIndexControls.AddRange(new string[] {
				"DevExpress.XtraBars.BarDockControl",
				"System.Windows.Forms.StatusBar"});
		}
		public XRDesignPanel ActiveDesignPanel {
			get { return xrDesignMdiController.ActiveDesignPanel; }
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
		#region Windows Form Designer generated code
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
			this.Name = "XRDesignForm";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.Visible = ((bool)(resources.GetObject("$this.Visible")));
			this.ResumeLayout(false);
		}
		#endregion
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			formLayoutSerializer.RestoreLayout();
		}
		protected override void OnClosed(EventArgs e) {
			formLayoutSerializer.SaveLayout();
			base.OnClosed(e);
		}
		protected virtual void SetLookAndFeel(UserLookAndFeel lookAndFeel) {
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			controller.LookAndFeel.ParentLookAndFeel = lookAndFeel;
		}
	}
}
