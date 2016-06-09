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
using System.Linq;
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.IO;
using DevExpress.Utils.Serializing;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Preview;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Docking;
using DevExpress.Utils;
using System.Resources;
namespace DevExpress.XtraReports.UserDesigner {
	[EditorBrowsable(EditorBrowsableState.Never)]
	public class XRDesignFormExBase : XtraForm, IServiceProvider {
		#region static
		internal static XRDesignDockManager CreateDesignDocManager(Form form, XRDesignPanel designPanel, bool addToolboxPanel) {
			XRDesignDockManager designDockManager;
			ErrorListDockPanel errorListDockPanel1;
			DesignControlContainer errorListDockPanel1_Container;
			GroupAndSortDockPanel groupAndSortDockPanel1;
			DesignControlContainer groupAndSortDockPanel1_Container;
			ToolBoxDockPanel toolBoxDockPanel1;
			DesignControlContainer toolBoxDockPanel1_Container;
			DesignDockPanel panelContainer1;
			DesignDockPanel panelContainer2;
			ReportExplorerDockPanel reportExplorerDockPanel1;
			DesignControlContainer reportExplorerDockPanel1_Container;
			FieldListDockPanel fieldListDockPanel1;
			DesignControlContainer fieldListDockPanel1_Container;
			PropertyGridDockPanel propertyGridDockPanel1;
			DesignControlContainer propertyGridDockPanel1_Container;
			DesignDockPanel panelContainer3;
			System.Resources.ResourceManager resources = new ResourceManager(typeof(XRDesignFormExBase));
			designDockManager = new XRDesignDockManager();
			panelContainer1 = new DesignDockPanel();
			panelContainer2 = new DesignDockPanel();
			reportExplorerDockPanel1 = new ReportExplorerDockPanel();
			reportExplorerDockPanel1_Container = new DesignControlContainer();
			fieldListDockPanel1 = new FieldListDockPanel();
			fieldListDockPanel1_Container = new DesignControlContainer();
			propertyGridDockPanel1 = new PropertyGridDockPanel();
			propertyGridDockPanel1_Container = new DesignControlContainer();
			if(addToolboxPanel) {
				toolBoxDockPanel1 = new ToolBoxDockPanel();
				toolBoxDockPanel1_Container = new DesignControlContainer();
			} else {
				toolBoxDockPanel1 = null;
				toolBoxDockPanel1_Container = null;
			}
			panelContainer3 = new DesignDockPanel();
			groupAndSortDockPanel1 = new GroupAndSortDockPanel();
			groupAndSortDockPanel1_Container = new DesignControlContainer();
			errorListDockPanel1 = new ErrorListDockPanel();
			errorListDockPanel1_Container = new DesignControlContainer();
			((ISupportInitialize)(designDockManager)).BeginInit();
			panelContainer1.SuspendLayout();
			panelContainer2.SuspendLayout();
			reportExplorerDockPanel1.SuspendLayout();
			fieldListDockPanel1.SuspendLayout();
			propertyGridDockPanel1.SuspendLayout();
			if(addToolboxPanel) 
				toolBoxDockPanel1.SuspendLayout();
			panelContainer3.SuspendLayout();
			groupAndSortDockPanel1.SuspendLayout();
			errorListDockPanel1.SuspendLayout();
			form.SuspendLayout();
			designDockManager.Form = form;
			designDockManager.ImageStream = ((ImageCollectionStreamer)(resources.GetObject("fDesignDockManager.ImageStream")));
			designDockManager.RootPanels.AddRange(
				addToolboxPanel ?
					new DockPanel[] { panelContainer1, toolBoxDockPanel1, panelContainer3}
					: new DockPanel[] { panelContainer1, panelContainer3 }
					); 
			designDockManager.TopZIndexControls.AddRange(new string[] {
			"DevExpress.XtraBars.BarDockControl",
			"DevExpress.XtraBars.StandaloneBarDockControl",
			"System.Windows.Forms.StatusBar",
			"DevExpress.XtraBars.Ribbon.RibbonStatusBar",
			"DevExpress.XtraBars.Ribbon.RibbonControl"});
			if(designPanel != null)
				designDockManager.XRDesignPanel = designPanel;
			panelContainer1.Controls.Add(panelContainer2);
			panelContainer1.Controls.Add(propertyGridDockPanel1);
			panelContainer1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Right;
			panelContainer1.ID = new System.Guid("03a0942b-441d-4dfe-be03-7afaae532483");
			panelContainer1.Location = new System.Drawing.Point(458, 0);
			panelContainer1.Name = "panelContainer1";
			panelContainer1.OriginalSize = new System.Drawing.Size(250, 200);
			panelContainer1.Size = new System.Drawing.Size(250, 351);
			panelContainer1.Text = "panelContainer1";
			panelContainer2.ActiveChild = reportExplorerDockPanel1;
			panelContainer2.Controls.Add(reportExplorerDockPanel1);
			panelContainer2.Controls.Add(fieldListDockPanel1);
			panelContainer2.Dock = DockingStyle.Fill;
			panelContainer2.ID = new System.Guid("6a885cd1-b5ce-4a64-8c9e-78872b044258");
			panelContainer2.ImageIndex = 3;
			panelContainer2.Location = new Point(0, 0);
			panelContainer2.Name = "panelContainer2";
			panelContainer2.OriginalSize = new Size(200, 200);
			panelContainer2.Size = new Size(250, 175);
			panelContainer2.Tabbed = true;
			panelContainer2.Text = "panelContainer2";
			reportExplorerDockPanel1.Controls.Add(reportExplorerDockPanel1_Container);
			reportExplorerDockPanel1.Dock = DockingStyle.Fill;
			reportExplorerDockPanel1.ID = new Guid("fb3ec6cc-3b9b-4b9c-91cf-cff78c1edbf1");
			reportExplorerDockPanel1.ImageIndex = 3;
			reportExplorerDockPanel1.Location = new Point(3, 25);
			reportExplorerDockPanel1.Name = "reportExplorerDockPanel1";
			reportExplorerDockPanel1.OriginalSize = new Size(200, 200);
			reportExplorerDockPanel1.Size = new Size(244, 124);
			reportExplorerDockPanel1.Text = "Report Explorer";
			if(designPanel != null)
				reportExplorerDockPanel1.XRDesignPanel = designPanel;
			reportExplorerDockPanel1_Container.Location = new System.Drawing.Point(0, 0);
			reportExplorerDockPanel1_Container.Name = "reportExplorerDockPanel1_Container";
			reportExplorerDockPanel1_Container.Size = new System.Drawing.Size(244, 124);
			reportExplorerDockPanel1_Container.TabIndex = 0;
			fieldListDockPanel1.Controls.Add(fieldListDockPanel1_Container);
			fieldListDockPanel1.Dock = DockingStyle.Fill;
			fieldListDockPanel1.ID = new Guid("faf69838-a93f-4114-83e8-d0d09cc5ce95");
			fieldListDockPanel1.ImageIndex = 0;
			fieldListDockPanel1.Location = new Point(3, 25);
			fieldListDockPanel1.Name = "fieldListDockPanel1";
			fieldListDockPanel1.OriginalSize = new Size(200, 200);
			fieldListDockPanel1.Size = new Size(244, 124);
			fieldListDockPanel1.Text = "Field List";
			if(designPanel != null)
				fieldListDockPanel1.XRDesignPanel = designPanel;
			fieldListDockPanel1_Container.Location = new Point(0, 0);
			fieldListDockPanel1_Container.Name = "fieldListDockPanel1_Container";
			fieldListDockPanel1_Container.Size = new Size(244, 124);
			fieldListDockPanel1_Container.TabIndex = 0;
			propertyGridDockPanel1.Controls.Add(propertyGridDockPanel1_Container);
			propertyGridDockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
			propertyGridDockPanel1.ID = new System.Guid("b38d12c3-cd06-4dec-b93d-63a0088e495a");
			propertyGridDockPanel1.ImageIndex = 2;
			propertyGridDockPanel1.Location = new System.Drawing.Point(0, 175);
			propertyGridDockPanel1.Name = "propertyGridDockPanel1";
			propertyGridDockPanel1.OriginalSize = new System.Drawing.Size(200, 200);
			propertyGridDockPanel1.Size = new System.Drawing.Size(250, 176);
			propertyGridDockPanel1.Text = "Property Grid";
			if(designPanel != null) 
				propertyGridDockPanel1.XRDesignPanel = designPanel;
			propertyGridDockPanel1_Container.Location = new System.Drawing.Point(3, 25);
			propertyGridDockPanel1_Container.Name = "propertyGridDockPanel1_Container";
			propertyGridDockPanel1_Container.Size = new System.Drawing.Size(244, 148);
			propertyGridDockPanel1_Container.TabIndex = 0;
			if(addToolboxPanel) {
				toolBoxDockPanel1.Controls.Add(toolBoxDockPanel1_Container);
				toolBoxDockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
				toolBoxDockPanel1.ID = new System.Guid("161a5a1a-d9b9-4f06-9ac4-d0c3e507c54f");
				toolBoxDockPanel1.ImageIndex = 4;
				toolBoxDockPanel1.Location = new System.Drawing.Point(0, 0);
				toolBoxDockPanel1.Name = "toolBoxDockPanel1";
				toolBoxDockPanel1.OriginalSize = new System.Drawing.Size(165, 200);
				toolBoxDockPanel1.Size = new System.Drawing.Size(165, 351);
				toolBoxDockPanel1.Text = "Tool Box";
				toolBoxDockPanel1.Visibility = DockVisibility.Hidden;
				if(designPanel != null)
					toolBoxDockPanel1.XRDesignPanel = designPanel;
				toolBoxDockPanel1_Container.Location = new System.Drawing.Point(3, 25);
				toolBoxDockPanel1_Container.Name = "toolBoxDockPanel1_Container";
				toolBoxDockPanel1_Container.Size = new System.Drawing.Size(159, 323);
				toolBoxDockPanel1_Container.TabIndex = 0;
			}
			panelContainer3.ActiveChild = groupAndSortDockPanel1;
			panelContainer3.Controls.Add(groupAndSortDockPanel1);
			panelContainer3.Controls.Add(errorListDockPanel1);
			panelContainer3.Dock = DevExpress.XtraBars.Docking.DockingStyle.Bottom;
			panelContainer3.FloatVertical = true;
			panelContainer3.ID = new System.Guid("9959c7d2-bc71-475b-bb0c-13f533ab656a");
			panelContainer3.ImageIndex = 1;
			panelContainer3.Location = new System.Drawing.Point(165, 191);
			panelContainer3.Name = "panelContainer3";
			panelContainer3.OriginalSize = new System.Drawing.Size(200, 160);
			panelContainer3.Size = new System.Drawing.Size(293, 160);
			panelContainer3.Tabbed = true;
			panelContainer3.Text = "panelContainer3";
			groupAndSortDockPanel1.Controls.Add(groupAndSortDockPanel1_Container);
			groupAndSortDockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
			groupAndSortDockPanel1.FloatVertical = true;
			groupAndSortDockPanel1.ID = new System.Guid("4bab159e-c495-4d67-87dc-f4e895da443e");
			groupAndSortDockPanel1.ImageIndex = 1;
			groupAndSortDockPanel1.Location = new System.Drawing.Point(3, 25);
			groupAndSortDockPanel1.Name = "groupAndSortDockPanel1";
			groupAndSortDockPanel1.OriginalSize = new System.Drawing.Size(200, 200);
			groupAndSortDockPanel1.Size = new System.Drawing.Size(287, 109);
			groupAndSortDockPanel1.Text = "Group and Sort";
			if(designPanel != null) 
				groupAndSortDockPanel1.XRDesignPanel = designPanel;
			groupAndSortDockPanel1_Container.Location = new System.Drawing.Point(0, 0);
			groupAndSortDockPanel1_Container.Name = "groupAndSortDockPanel1_Container";
			groupAndSortDockPanel1_Container.Size = new System.Drawing.Size(287, 109);
			groupAndSortDockPanel1_Container.TabIndex = 0;
			errorListDockPanel1.Controls.Add(errorListDockPanel1_Container);
			errorListDockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Fill;
			errorListDockPanel1.FloatVertical = true;
			errorListDockPanel1.ID = new System.Guid("5a9a01fd-6e95-4e81-a8c4-ac63153d7488");
			errorListDockPanel1.ImageIndex = 5;
			errorListDockPanel1.Location = new System.Drawing.Point(3, 25);
			errorListDockPanel1.Name = "errorListDockPanel1";
			errorListDockPanel1.OriginalSize = new System.Drawing.Size(200, 160);
			errorListDockPanel1.Size = new System.Drawing.Size(287, 109);
			errorListDockPanel1.Text = "Scripts Errors";
			if(designPanel != null) 
				errorListDockPanel1.XRDesignPanel = designPanel;
			errorListDockPanel1_Container.Location = new System.Drawing.Point(0, 0);
			errorListDockPanel1_Container.Name = "errorListDockPanel1_Container";
			errorListDockPanel1_Container.Size = new System.Drawing.Size(287, 109);
			errorListDockPanel1_Container.TabIndex = 0;
			form.Controls.Add(panelContainer3);
			if(addToolboxPanel)
				form.Controls.Add(toolBoxDockPanel1);
			form.Controls.Add(panelContainer1);
			((ISupportInitialize)(designDockManager)).EndInit();
			panelContainer1.ResumeLayout(false);
			panelContainer2.ResumeLayout(false);
			reportExplorerDockPanel1.ResumeLayout(false);
			fieldListDockPanel1.ResumeLayout(false);
			propertyGridDockPanel1.ResumeLayout(false);
			if(addToolboxPanel)
				toolBoxDockPanel1.ResumeLayout(false);
			panelContainer3.ResumeLayout(false);
			groupAndSortDockPanel1.ResumeLayout(false);
			errorListDockPanel1.ResumeLayout(false);
			form.ResumeLayout(false);
			designDockManager.ForceInitialize();
			designDockManager.ForceLocalize();
			return designDockManager;
		}
		protected static void SetWindowVisibility(XRDesignDockManager designDockManager, DesignDockPanelType designDockPanels, bool visible) {
			EUDVisibility eudVisibility = visible ? EUDVisibility.Visible : EUDVisibility.Hidden;
			foreach(TypedDesignDockPanel panel in designDockManager.GetDesignDockPanels().ToArray<TypedDesignDockPanel>()) {
				if((designDockPanels & panel.PanelType) > 0)
					panel.EUDVisibility = eudVisibility;
			}
		}
		static void DisposeDesignDockManager(XRDesignDockManager designDockManager) {
			if(designDockManager != null) {
				designDockManager.Controller = null;
				designDockManager.Dispose();
			}
		}
#if DEBUGTEST
		public static XRDesignDockManager Test_CreateDesignDocManager(Form form, XRDesignPanel designPanel, bool addToolboxPanel) {
			return CreateDesignDocManager(form, designPanel, addToolboxPanel);
		}
		public static void Test_SetWindowVisibility(XRDesignDockManager designDockManager, DesignDockPanelType designDockPanels, bool visible) {
			SetWindowVisibility(designDockManager, designDockPanels, visible);
		}
		public static void Test_DisposeDesignDockManager(XRDesignDockManager designDockManager) {
			DisposeDesignDockManager(designDockManager);
		}
#endif
		#endregion
		protected const string layoutKey = "\\Software\\Developer Express\\XtraReports\\";
		protected XRDesignPanel xrDesignPanel;
		FormLayout formLayout;
		BarAndDockingController controller;
		XRDesignDockManager designDockManager;
		#region events
		private static readonly object ReportStateChangedEvent = new object();
		[
#if !SL
	DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignFormExBaseReportStateChanged")
#else
	Description("")
#endif
		]
		public event ReportStateEventHandler ReportStateChanged {
			add { Events.AddHandler(ReportStateChangedEvent, value); }
			remove { Events.RemoveHandler(ReportStateChangedEvent, value); }
		}
		protected virtual void OnReportStateChanged(ReportStateEventArgs e) {
			ReportStateEventHandler handler = (ReportStateEventHandler)this.Events[ReportStateChangedEvent];
			if(handler != null) handler(this, e);
		}
		#endregion events
		#region properties
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignFormExBaseFileName")]
#endif
		public string FileName {
			get { return xrDesignPanel.FileName; }
			set { xrDesignPanel.FileName = value; }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignFormExBaseDesignPanel")]
#endif
		public XRDesignPanel DesignPanel {
			get { return xrDesignPanel; }
			set { xrDesignPanel = value; }
		}
#if !SL
	[DevExpressXtraReportsExtensionsLocalizedDescription("XRDesignFormExBaseDesignDockManager")]
#endif
		public XRDesignDockManager DesignDockManager { 
			get { return designDockManager; } 
			set { designDockManager = value; }
		}
		protected BarAndDockingController BarAndDockingController { 
			get { return controller; } 
		}
		#endregion properties
		public XRDesignFormExBase() {
			formLayout = new FormLayout(this);
			controller = new BarAndDockingController();
			InitializeComponent();
			xrDesignPanel.CommandExecute += new CommandIDEventHandler(OnCommandExecute);
		}
		protected override void Dispose( bool disposing )	{
			if( disposing )	{
				if(xrDesignPanel != null) {
					xrDesignPanel.CommandExecute -= new CommandIDEventHandler(OnCommandExecute);
					xrDesignPanel.Dispose();
					xrDesignPanel = null;
				}
				if(formLayout != null) {
					formLayout.Dispose();
					formLayout = null;
				}
				if(controller != null) {
					controller.Dispose();
					controller = null;
				}
				DisposeDesignDockManager(designDockManager);
				designDockManager = null;
			}
			base.Dispose( disposing );
		}
		protected virtual void CreateDesignDocManager() {
		}
		public virtual void SetWindowVisibility(DesignDockPanelType designDockPanels, bool visible) {
		}
		protected virtual void InitDesignDockManager() {
			DesignDockManager.Controller = controller;
			controller.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		#region OpenReport methods
		public void OpenReport(XtraReport report, UserLookAndFeel lookAndFeel) {
			SetLookAndFeel(lookAndFeel);
			xrDesignPanel.OpenReport(report);
		}
		public void OpenReport(XtraReport report) {
			OpenReport(report, null);
		}
		public void OpenReport(string fileName, UserLookAndFeel lookAndFeel) {
			SetLookAndFeel(lookAndFeel);
			xrDesignPanel.OpenReport(fileName);
		}
		public void OpenReport(string fileName) {
			OpenReport(fileName, null);
		}
		#endregion //OpenReport methods
		public void SaveReport(string fileName) {
			xrDesignPanel.SaveReport(fileName);
		}
		public void SaveReportAs() {
			xrDesignPanel.SaveReportAs();
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(XRDesignFormExBase));
			this.xrDesignPanel = new DevExpress.XtraReports.UserDesigner.XRDesignPanel();
			this.SuspendLayout();
			this.xrDesignPanel.AccessibleDescription = ((string)(resources.GetObject("xrDesignPanel.AccessibleDescription")));
			this.xrDesignPanel.AccessibleName = ((string)(resources.GetObject("xrDesignPanel.AccessibleName")));
			this.xrDesignPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("xrDesignPanel.Anchor")));
			this.xrDesignPanel.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("xrDesignPanel.Dock")));
			this.xrDesignPanel.Enabled = ((bool)(resources.GetObject("xrDesignPanel.Enabled")));
			this.xrDesignPanel.Font = ((System.Drawing.Font)(resources.GetObject("xrDesignPanel.Font")));
			this.xrDesignPanel.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("xrDesignPanel.ImeMode")));
			this.xrDesignPanel.Location = ((System.Drawing.Point)(resources.GetObject("xrDesignPanel.Location")));
			this.xrDesignPanel.Name = "xrDesignPanel";
			this.xrDesignPanel.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("xrDesignPanel.RightToLeft")));
			this.xrDesignPanel.Size = ((System.Drawing.Size)(resources.GetObject("xrDesignPanel.Size")));
			this.xrDesignPanel.TabIndex = ((int)(resources.GetObject("xrDesignPanel.TabIndex")));
			this.xrDesignPanel.Text = resources.GetString("xrDesignPanel.Text");
			this.xrDesignPanel.Visible = ((bool)(resources.GetObject("xrDesignPanel.Visible")));
			this.xrDesignPanel.ReportStateChanged += new DevExpress.XtraReports.UserDesigner.ReportStateEventHandler(this.xrDesignPanel_ReportStateChanged);
			this.AccessibleDescription = ((string)(resources.GetObject("$this.AccessibleDescription")));
			this.AccessibleName = ((string)(resources.GetObject("$this.AccessibleName")));
			this.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("$this.Anchor")));
			this.AutoScaleMode = AutoScaleMode.None;
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.xrDesignPanel});
			this.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("$this.Dock")));
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "XRDesignFormExBase";
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
			RestoreLayout();
		}
		protected override void OnClosed(EventArgs e) {
			SaveLayout();
			base.OnClosed(e);
		}
		protected virtual void SaveLayout() {
			XtraSerializer serializer = new RegistryXtraSerializer();
			formLayout.SaveFormLayout();
			serializer.SerializeObject(formLayout, layoutKey, formLayout.GetType().Name);
		}
		protected virtual void RestoreLayout() {
			XtraSerializer serializer = new RegistryXtraSerializer();
			serializer.DeserializeObject(formLayout, layoutKey, formLayout.GetType().Name);
			formLayout.RestoreFormLayout();
		}
		protected virtual void SetLookAndFeel(UserLookAndFeel lookAndFeel) {
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			controller.LookAndFeel.ParentLookAndFeel = lookAndFeel;
		}
		object IServiceProvider.GetService(Type serviceType) {
			return xrDesignPanel.GetService(serviceType);
		}
		private void OnCommandExecute(object sender, CommandIDEventArgs e) {
			bool handled = false;
			ExecuteCommand(e.CommandID, ref handled);
			if(handled) throw( new Exception() );
		}
		protected virtual void ExecuteCommand(CommandID cmdID, ref bool handled) {
		}
		protected void SetReportState(ReportState reportState) {
			xrDesignPanel.ReportState = reportState;
		}
		private void xrDesignPanel_ReportStateChanged(object sender, DevExpress.XtraReports.UserDesigner.ReportStateEventArgs e) {
			OnReportStateChanged(e);
			UpdateText(e.ReportState);
		}
		private void UpdateText(ReportState reportState) {
			if(xrDesignPanel.FileNameExists && File.Exists(xrDesignPanel.FileName)) {
				string text = String.Format("{0} - {1}", ReportLocalizer.GetString(ReportStringId.UD_FormCaption), xrDesignPanel.FileName);
				Text = XRDesignPanel.ApplyReportState(text, reportState);
			} else
				Text = ReportLocalizer.GetString(ReportStringId.UD_FormCaption);
		}
	}
}
