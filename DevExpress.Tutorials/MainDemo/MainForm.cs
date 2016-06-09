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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DevExpress.Utils;
using DevExpress.XtraNavBar;
using System.Collections.Generic;
using DevExpress.DemoData;
using System.Reflection;
namespace DevExpress.DXperience.Demos {
	public class frmMain : DevExpress.XtraEditors.XtraForm, ITutorialForm {
		protected DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.Utils.Frames.NotePanel8_1 pnlHint;
		protected DevExpress.Utils.Frames.ApplicationCaption8_1 pnlCaption;
		private DevExpress.XtraEditors.GroupControl gcNavigations;
		private DevExpress.XtraEditors.GroupControl gcContainer;
		private DevExpress.XtraEditors.PanelControl horzSplitter;
		protected DevExpress.XtraNavBar.NavBarControl navBarControl1;
		private DevExpress.XtraEditors.SimpleButton sbRun;
		private DevExpress.XtraEditors.PanelControl pcActions;
		private DevExpress.XtraBars.BarManager barManager1;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private DevExpress.XtraBars.Bar mainMenu;
		private DevExpress.XtraBars.Bar statusBar;
		private DevExpress.XtraEditors.GroupControl gcDescription;
		private DevExpress.XtraEditors.SimpleButton sbDescription;
		private DevExpress.XtraBars.BarStaticItem bsUser;
		private DevExpress.XtraBars.BarStaticItem bsData;
		private Panel pnlFilter;
		private DevExpress.XtraEditors.LabelControl lbFilter;
		private DevExpress.XtraEditors.TextEdit tbFilter;
		private System.ComponentModel.IContainer components;
		NavBarFilter filter;
		string startModule = string.Empty;
		bool fullWindow = false;
		protected DevExpress.DXperience.Demos.LookAndFeelMenu appearanceMenu = null;
		string prevModuleName = "";
		DevExpress.XtraNavBar.NavBarItemLink prevLink = null;
		int maxHintRows = 25;
		protected virtual bool AllowNavBarFilter { get { return true; } }
		protected virtual bool ShowStatusBar { get { return false; } }
		protected virtual void SetFormParam() { }
		protected virtual void CreateMenu() { }
		protected virtual void FillNavBar() {
			ModulesInfo.FillNavBar(navBarControl1);
		}
		protected virtual void ShowModule(string name, DevExpress.XtraEditors.GroupControl group, DevExpress.LookAndFeel.DefaultLookAndFeel lookAndFeel, DevExpress.Utils.Frames.ApplicationCaption caption) { }
		protected virtual void ShowModule(string name, DevExpress.XtraEditors.GroupControl group, DevExpress.LookAndFeel.DefaultLookAndFeel lookAndFeel, DevExpress.Utils.Frames.ApplicationCaption caption, DevExpress.Utils.Frames.NotePanel notePanel) {
			ShowModule(name, group, lookAndFeel, caption);
		}
		protected DevExpress.XtraBars.BarManager Manager {
			get { return barManager1; }
		}
		protected DevExpress.XtraBars.Bar StatusBar { get { return statusBar; } }
		public void SetUserInfo(string caption) { bsUser.Caption = caption; }
		public void SetDataInfo(string caption) { bsData.Caption = caption; }
		public frmMain() : this(new string[] { }) { }
		public frmMain(string[] arguments) {
			InitializeComponent();
			DevExpress.Skins.SkinManager.EnableFormSkins();
			DevExpress.LookAndFeel.LookAndFeelHelper.ForceDefaultLookAndFeelChanged();
			navBarControl1.MenuManager = barManager1;
			if(ShowStatusBar) {
				statusBar.Visible = true;
			}
			SetFormParam();
			this.filter = new NavBarFilter(navBarControl1);
			pnlFilter.Visible = AllowNavBarFilter;
			MainFormHelper.SetCommandLineArgs(Environment.GetCommandLineArgs(), out startModule, out fullWindow);   
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(filter != null) filter.Dispose();
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.gcNavigations = new DevExpress.XtraEditors.GroupControl();
			this.navBarControl1 = new DevExpress.XtraNavBar.NavBarControl();
			this.pnlFilter = new System.Windows.Forms.Panel();
			this.lbFilter = new DevExpress.XtraEditors.LabelControl();
			this.tbFilter = new DevExpress.XtraEditors.TextEdit();
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.mainMenu = new DevExpress.XtraBars.Bar();
			this.statusBar = new DevExpress.XtraBars.Bar();
			this.bsUser = new DevExpress.XtraBars.BarStaticItem();
			this.bsData = new DevExpress.XtraBars.BarStaticItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.gcContainer = new DevExpress.XtraEditors.GroupControl();
			this.horzSplitter = new DevExpress.XtraEditors.PanelControl();
			this.pcActions = new DevExpress.XtraEditors.PanelControl();
			this.sbRun = new DevExpress.XtraEditors.SimpleButton();
			this.gcDescription = new DevExpress.XtraEditors.GroupControl();
			this.sbDescription = new DevExpress.XtraEditors.SimpleButton();
			this.pnlHint = new DevExpress.Utils.Frames.NotePanel8_1();
			this.pnlCaption = new DevExpress.Utils.Frames.ApplicationCaption8_1();
			((System.ComponentModel.ISupportInitialize)(this.gcNavigations)).BeginInit();
			this.gcNavigations.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).BeginInit();
			this.pnlFilter.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbFilter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcContainer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcActions)).BeginInit();
			this.pcActions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcDescription)).BeginInit();
			this.gcDescription.SuspendLayout();
			this.SuspendLayout();
			this.gcNavigations.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gcNavigations.CaptionLocation = DevExpress.Utils.Locations.Left;
			this.gcNavigations.Controls.Add(this.navBarControl1);
			this.gcNavigations.Controls.Add(this.pnlFilter);
			this.gcNavigations.Dock = System.Windows.Forms.DockStyle.Left;
			this.gcNavigations.Location = new System.Drawing.Point(0, 24);
			this.gcNavigations.Name = "gcNavigations";
			this.gcNavigations.Padding = new System.Windows.Forms.Padding(1);
			this.gcNavigations.ShowCaption = false;
			this.gcNavigations.Size = new System.Drawing.Size(200, 598);
			this.gcNavigations.TabIndex = 0;
			this.gcNavigations.Text = "Navigations:";
			this.navBarControl1.ActiveGroup = null;
			this.navBarControl1.LinkSelectionMode = LinkSelectionModeType.OneInControl;
			this.navBarControl1.Appearance.Item.Options.UseTextOptions = true;
			this.navBarControl1.Appearance.Item.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.navBarControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.navBarControl1.ContentButtonHint = null;
			this.navBarControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.navBarControl1.Location = new System.Drawing.Point(1, 32);
			this.navBarControl1.Name = "navBarControl1";
			this.navBarControl1.OptionsNavPane.ExpandedWidth = 186;
			this.navBarControl1.Size = new System.Drawing.Size(198, 565);
			this.navBarControl1.StoreDefaultPaintStyleName = true;
			this.navBarControl1.TabIndex = 0;
			this.navBarControl1.Text = "navBarControl1";
			this.navBarControl1.SelectedLinkChanged += new DevExpress.XtraNavBar.ViewInfo.NavBarSelectedLinkChangedEventHandler(this.navBarControl1_SelectedLinkChanged);
			this.pnlFilter.Controls.Add(this.lbFilter);
			this.pnlFilter.Controls.Add(this.tbFilter);
			this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlFilter.Location = new System.Drawing.Point(1, 1);
			this.pnlFilter.Name = "pnlFilter";
			this.pnlFilter.Size = new System.Drawing.Size(198, 31);
			this.pnlFilter.TabIndex = 3;
			this.lbFilter.Location = new System.Drawing.Point(3, 6);
			this.lbFilter.Name = "lbFilter";
			this.lbFilter.Size = new System.Drawing.Size(28, 13);
			this.lbFilter.TabIndex = 0;
			this.lbFilter.Text = "Filter:";
			this.tbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbFilter.Location = new System.Drawing.Point(37, 3);
			this.tbFilter.MenuManager = this.barManager1;
			this.tbFilter.Name = "tbFilter";
			this.tbFilter.Size = new System.Drawing.Size(158, 20);
			this.tbFilter.TabIndex = 1;
			this.tbFilter.EditValueChanged += new System.EventHandler(this.tbFilter_EditValueChanged);
			this.barManager1.AllowCustomization = false;
			this.barManager1.AllowQuickCustomization = false;
			this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.mainMenu,
			this.statusBar});
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.bsUser,
			this.bsData});
			this.barManager1.MainMenu = this.mainMenu;
			this.barManager1.MaxItemId = 2;
			this.barManager1.StatusBar = this.statusBar;
			this.mainMenu.BarName = "Main Menu";
			this.mainMenu.DockCol = 0;
			this.mainMenu.DockRow = 0;
			this.mainMenu.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.mainMenu.OptionsBar.AllowQuickCustomization = false;
			this.mainMenu.OptionsBar.DisableCustomization = true;
			this.mainMenu.OptionsBar.MultiLine = true;
			this.mainMenu.OptionsBar.UseWholeRow = true;
			this.mainMenu.Text = "Main Menu";
			this.statusBar.BarName = "Status Bar";
			this.statusBar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Bottom;
			this.statusBar.DockCol = 0;
			this.statusBar.DockRow = 0;
			this.statusBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Bottom;
			this.statusBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.bsUser),
			new DevExpress.XtraBars.LinkPersistInfo(this.bsData)});
			this.statusBar.OptionsBar.AllowQuickCustomization = false;
			this.statusBar.OptionsBar.DrawDragBorder = false;
			this.statusBar.OptionsBar.UseWholeRow = true;
			this.statusBar.Text = "Status Bar";
			this.statusBar.Visible = false;
			this.bsUser.Id = 0;
			this.bsUser.Name = "bsUser";
			this.bsUser.TextAlignment = System.Drawing.StringAlignment.Near;
			this.bsData.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Spring;
			this.bsData.Id = 1;
			this.bsData.LeftIndent = 4;
			this.bsData.Name = "bsData";
			this.bsData.TextAlignment = System.Drawing.StringAlignment.Near;
			this.bsData.Width = 32;
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.gcContainer);
			this.panelControl1.Controls.Add(this.horzSplitter);
			this.panelControl1.Controls.Add(this.pcActions);
			this.panelControl1.Controls.Add(this.gcDescription);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl1.Location = new System.Drawing.Point(200, 88);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Padding = new System.Windows.Forms.Padding(8);
			this.panelControl1.Size = new System.Drawing.Size(750, 534);
			this.panelControl1.TabIndex = 1;
			this.gcContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gcContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gcContainer.Location = new System.Drawing.Point(8, 45);
			this.gcContainer.Name = "gcContainer";
			this.gcContainer.ShowCaption = false;
			this.gcContainer.Size = new System.Drawing.Size(734, 436);
			this.gcContainer.TabIndex = 1;
			this.gcContainer.Text = "Tutorial:";
			this.horzSplitter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.horzSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.horzSplitter.Location = new System.Drawing.Point(8, 481);
			this.horzSplitter.Name = "horzSplitter";
			this.horzSplitter.Size = new System.Drawing.Size(734, 8);
			this.horzSplitter.TabIndex = 7;
			this.pcActions.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pcActions.Controls.Add(this.sbRun);
			this.pcActions.Dock = System.Windows.Forms.DockStyle.Top;
			this.pcActions.Location = new System.Drawing.Point(8, 8);
			this.pcActions.Name = "pcActions";
			this.pcActions.Size = new System.Drawing.Size(734, 37);
			this.pcActions.TabIndex = 9;
			this.pcActions.Visible = false;
			this.sbRun.Location = new System.Drawing.Point(0, 0);
			this.sbRun.Name = "sbRun";
			this.sbRun.Size = new System.Drawing.Size(128, 27);
			this.sbRun.TabIndex = 1;
			this.sbRun.Text = "Run Active Demo";
			this.sbRun.Click += new System.EventHandler(this.sbRun_Click);
			this.gcDescription.Controls.Add(this.sbDescription);
			this.gcDescription.Controls.Add(this.pnlHint);
			this.gcDescription.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.gcDescription.Location = new System.Drawing.Point(8, 489);
			this.gcDescription.Name = "gcDescription";
			this.gcDescription.ShowCaption = false;
			this.gcDescription.Size = new System.Drawing.Size(734, 37);
			this.gcDescription.TabIndex = 3;
			this.sbDescription.AllowFocus = false;
			this.sbDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.sbDescription.Appearance.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.sbDescription.Appearance.Options.UseFont = true;
			this.sbDescription.Location = new System.Drawing.Point(712, -17);
			this.sbDescription.Name = "sbDescription";
			this.sbDescription.Size = new System.Drawing.Size(20, 17);
			this.sbDescription.TabIndex = 1;
			this.sbDescription.Text = "<<";
			this.sbDescription.Click += new System.EventHandler(this.sbDescription_Click);
			this.pnlHint.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlHint.Location = new System.Drawing.Point(2, 2);
			this.pnlHint.MaxRows = 25;
			this.pnlHint.Name = "pnlHint";
			this.pnlHint.ParentAutoHeight = true;
			this.pnlHint.Size = new System.Drawing.Size(730, 33);
			this.pnlHint.TabIndex = 0;
			this.pnlHint.TabStop = false;
			this.pnlCaption.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlCaption.Location = new System.Drawing.Point(200, 24);
			this.pnlCaption.Name = "pnlCaption";
			this.pnlCaption.Size = new System.Drawing.Size(750, 64);
			this.pnlCaption.TabIndex = 4;
			this.pnlCaption.TabStop = false;
			this.pnlCaption.Text = "Demo";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(950, 650);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.pnlCaption);
			this.Controls.Add(this.gcNavigations);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.KeyPreview = true;
			this.MinimumSize = new System.Drawing.Size(800, 600);
			this.Name = "frmMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Features Demo (C# code)";
			this.Load += new System.EventHandler(this.OnLoad);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
			((System.ComponentModel.ISupportInitialize)(this.gcNavigations)).EndInit();
			this.gcNavigations.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.navBarControl1)).EndInit();
			this.pnlFilter.ResumeLayout(false);
			this.pnlFilter.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.tbFilter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcContainer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcActions)).EndInit();
			this.pcActions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcDescription)).EndInit();
			this.gcDescription.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected virtual void OnLoad(object sender, System.EventArgs e) {
			MainFormHelper.RegisterDefaultBonusSkin();
			CreateMenu();
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if(entryAssembly == null) return;
			object[] attributes = entryAssembly.GetCustomAttributes(typeof(ProductIdAttribute), false);
			if(attributes.Length == 0)
				throw new Exception("Every demo application must have the ProductId assembly attribute");
			ProductIdAttribute productIdAttribute = (ProductIdAttribute)attributes[0];
			MainFormRegisterDemoHelper.RegisterDemos(productIdAttribute.ProductId);
			FillNavBar();
			ModuleInfo info = null;
			if(!startModule.Equals(string.Empty)) {
				info = ModulesInfo.GetItemByType(startModule);
				MainFormHelper.SelectNavBarItem(navBarControl1, startModule);
			}
			if(info == null) {
				info = ModulesInfo.GetItem(0);
				if(info != null) ShowModule(info.Name);
			}
			if(fullWindow) {
				HideServiceElements();
				gcDescription.Visible = horzSplitter.Visible = ModulesInfo.Instance.CurrentModuleBase.Description != "";
			}
		}
		private void sbRun_Click(object sender, System.EventArgs e) {
			TutorialControlBase current = ModulesInfo.Instance.CurrentModuleBase.TModule as TutorialControlBase;
			if(current == null || !current.HasActiveDemo) return;
			current.RunActiveDemo();
		}
		public void ResetNavbarSelectedLink() {
			this.navBarControl1.SelectedLink = null;
		}
		public void ShowModule(string name) {
			if(ModulesInfo.Instance.CurrentModuleBase != null) prevModuleName = ModulesInfo.Instance.CurrentModuleBase.Name;
			gcContainer.Parent.SuspendLayout();
			gcContainer.SuspendLayout();
			ShowModule(name, gcContainer, defaultLookAndFeel1, pnlCaption, pnlHint);
			gcDescription.Visible = horzSplitter.Visible = ModulesInfo.Instance.CurrentModuleBase.Description != "";
			if(ModulesInfo.Instance.CurrentModuleBase.Description != "*")
				pnlHint.Text = ModulesInfo.Instance.CurrentModuleBase.Description;
			TutorialControlBase currentModule = ModulesInfo.Instance.CurrentModuleBase.TModule as TutorialControlBase;
			if(currentModule == null) return;
			pcActions.Visible = currentModule.HasActiveDemo;
			sbRun.Enabled = currentModule.HasActiveDemo;
			gcContainer.ResumeLayout();
			gcContainer.Parent.ResumeLayout();
			currentModule.Invalidate();
			MainFormHelper.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
		}
		private void navBarControl1_SelectedLinkChanged(object sender, DevExpress.XtraNavBar.ViewInfo.NavBarSelectedLinkChangedEventArgs e) {
			if(e.Link == null) return;
			ShowModule(e.Link.Caption);
			prevLink = e.Link;
		}
		private void frmMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(prevModuleName == "") return;
			bool isAbout = ModulesInfo.Instance.CurrentModuleBase.Name == ModulesInfo.GetItem(0).Name;
			if(e.KeyData == Keys.Escape && isAbout) {
				if(prevLink != null) navBarControl1.SelectedLink = prevLink;
			}
		}
		private void sbDescription_Click(object sender, EventArgs e) {
			if(pnlHint.MaxRows == 1) {
				pnlHint.MaxRows = maxHintRows;
				sbDescription.Text = "<<";
			} else {
				pnlHint.MaxRows = 1;
				sbDescription.Text = ">>";
			}
		}
		void tbFilter_EditValueChanged(object sender, EventArgs e) {
			filter.FilterNavBar(tbFilter.Text);
		}
		#region ITutorialForm Members
		bool ITutorialForm.IsFullMode {
			get { return !this.pnlCaption.Visible; }
		}
		bool ITutorialForm.AllowDemoFilter {
			get { return AllowNavBarFilter; }
		}
		bool ITutorialForm.IsDemoFilterVisible {
			get { return pnlFilter.Visible; }
		}
		public void HideServiceElements() {
			gcDescription.Visible = horzSplitter.Visible = false;
			this.gcNavigations.Hide();
			this.pnlCaption.Hide();
			navBarControl1.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Collapsed;
			statusBar.Visible = false;
		}
		public void ShowServiceElements() {
			gcDescription.Visible = horzSplitter.Visible = ModulesInfo.Instance.CurrentModuleBase.Description != "";
			this.gcNavigations.Show();
			this.pnlCaption.Show();
			navBarControl1.OptionsNavPane.NavPaneState = DevExpress.XtraNavBar.NavPaneState.Expanded;
			if(ShowStatusBar) statusBar.Visible = true;
		}
		public void ShowDemoFilter() {
			pnlFilter.Visible = !pnlFilter.Visible;
		}
		#endregion
	}
}
