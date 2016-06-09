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
using System.Reflection;
using System.Drawing.Drawing2D;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using System.Runtime.InteropServices;
using DevExpress.XtraBars;
using DevExpress.Tutorials.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.DXperience.Demos;
namespace DevExpress.Tutorials {
	public class FrmMain : FrmMainBase, ITutorialForm, IWhatsThisProvider {
		protected DevExpress.XtraEditors.SimpleButton btnWhatsThis;
		private DevExpress.DXperience.Demos.LookAndFeelMenu mainMenu;
		private System.ComponentModel.IContainer components = null;
		private WhatsThisController whatsThisController;
		private bool whatsThisEnabled;
		private ModuleWhatsThis whatsThisModule;
		private bool disableWhatsThisOnMove;
		protected DevExpress.XtraBars.BarManager barManager1;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private DevExpress.XtraBars.Bar bar1;
		protected DevExpress.XtraBars.BarSubItem bsiOptions;
		private DevExpress.XtraBars.BarSubItem bsiShader;
		private DevExpress.XtraBars.BarCheckItem bciDisable;
		private DevExpress.XtraBars.BarCheckItem bciDots;
		private DevExpress.XtraBars.BarSubItem bsiHatch;
		private DevExpress.XtraBars.BarListItem bliHatch;
		private DevExpress.XtraBars.BarSubItem bsiHotTrack;
		private DevExpress.XtraBars.BarCheckItem bciHint;
		private DevExpress.XtraBars.BarCheckItem bciFrame;
		protected DevExpress.XtraEditors.SimpleButton btnAbout;
		private Panel pnlFilter;
		private DevExpress.XtraEditors.LabelControl lbFilter;
		private DevExpress.XtraEditors.TextEdit tbFilter;
		private Point prevFormLocation;
		public DevExpress.DXperience.Demos.LookAndFeelMenu MainMenu {
			get { return mainMenu; }
		}
		public FrmMain() {
			InitializeComponent();
			applicationStartupPath = Application.StartupPath;
			pnlCaption.Controls.Add(btnWhatsThis);
			pnlCaption.Controls.Add(btnAbout);
			btnWhatsThis.Top = btnAbout.Top = (pnlCaption.Height - btnWhatsThis.Height) / 2;
			whatsThisController = new WhatsThisController(this);
			InitWhatsThisModule();
			whatsThisEnabled = false;
			disableWhatsThisOnMove = true;
			prevFormLocation = Point.Empty;
			currentShader = new ImageShaderDisable();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
			this.btnWhatsThis = new DevExpress.XtraEditors.SimpleButton();
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.bar1 = new DevExpress.XtraBars.Bar();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.bsiOptions = new DevExpress.XtraBars.BarSubItem();
			this.bsiShader = new DevExpress.XtraBars.BarSubItem();
			this.bciDisable = new DevExpress.XtraBars.BarCheckItem();
			this.bciDots = new DevExpress.XtraBars.BarCheckItem();
			this.bsiHatch = new DevExpress.XtraBars.BarSubItem();
			this.bliHatch = new DevExpress.XtraBars.BarListItem();
			this.bsiHotTrack = new DevExpress.XtraBars.BarSubItem();
			this.bciHint = new DevExpress.XtraBars.BarCheckItem();
			this.bciFrame = new DevExpress.XtraBars.BarCheckItem();
			this.btnAbout = new DevExpress.XtraEditors.SimpleButton();
			this.lbFilter = new DevExpress.XtraEditors.LabelControl();
			this.tbFilter = new DevExpress.XtraEditors.TextEdit();
			this.pnlFilter = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.gcNavigations)).BeginInit();
			this.gcNavigations.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcContainer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcDescription)).BeginInit();
			this.gcDescription.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pcMain)).BeginInit();
			this.pcMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbFilter.Properties)).BeginInit();
			this.pnlFilter.SuspendLayout();
			this.SuspendLayout();
			this.pnlHint.Size = new System.Drawing.Size(506, 33);
			this.pnlCaption.Location = new System.Drawing.Point(166, 24);
			this.pnlCaption.Size = new System.Drawing.Size(526, 51);
			this.pnlCaption.SizeChanged += new System.EventHandler(this.pnlCaption_SizeChanged);
			this.gcNavigations.Controls.Add(this.pnlFilter);
			this.gcNavigations.Location = new System.Drawing.Point(0, 24);
			this.gcNavigations.Size = new System.Drawing.Size(166, 451);
			this.gcContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gcContainer.Size = new System.Drawing.Size(510, 339);
			this.horzSplitter.Location = new System.Drawing.Point(8, 347);
			this.horzSplitter.Size = new System.Drawing.Size(510, 8);
			this.gcDescription.Location = new System.Drawing.Point(8, 355);
			this.gcDescription.Size = new System.Drawing.Size(510, 37);
			this.pcMain.Location = new System.Drawing.Point(166, 75);
			this.pcMain.Size = new System.Drawing.Size(526, 400);
			this.btnWhatsThis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnWhatsThis.Location = new System.Drawing.Point(184, 32);
			this.btnWhatsThis.Name = "btnWhatsThis";
			this.btnWhatsThis.Size = new System.Drawing.Size(96, 32);
			this.btnWhatsThis.TabIndex = 0;
			this.btnWhatsThis.Text = "What\'s This?";
			this.btnWhatsThis.Visible = false;
			this.btnWhatsThis.Click += new System.EventHandler(this.btnWhatsThis_Click);
			this.barManager1.AllowCustomization = false;
			this.barManager1.AllowQuickCustomization = false;
			this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.bar1});
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.bsiOptions,
			this.bsiShader,
			this.bsiHotTrack,
			this.bciDisable,
			this.bciDots,
			this.bsiHatch,
			this.bliHatch,
			this.bciHint,
			this.bciFrame});
			this.barManager1.MainMenu = this.bar1;
			this.barManager1.MaxItemId = 9;
			this.bar1.BarName = "Main Menu";
			this.bar1.DockCol = 0;
			this.bar1.DockRow = 0;
			this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.bar1.OptionsBar.AllowQuickCustomization = false;
			this.bar1.OptionsBar.DisableCustomization = true;
			this.bar1.OptionsBar.MultiLine = true;
			this.bar1.OptionsBar.UseWholeRow = true;
			this.bar1.Text = "Main Menu";
			this.bsiOptions.Caption = "What\'s This Options";
			this.bsiOptions.Id = 0;
			this.bsiOptions.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.bsiShader),
			new DevExpress.XtraBars.LinkPersistInfo(this.bsiHotTrack)});
			this.bsiOptions.Name = "bsiOptions";
			this.bsiShader.Caption = "Choose shader";
			this.bsiShader.Id = 1;
			this.bsiShader.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.bciDisable),
			new DevExpress.XtraBars.LinkPersistInfo(this.bciDots),
			new DevExpress.XtraBars.LinkPersistInfo(this.bsiHatch)});
			this.bsiShader.Name = "bsiShader";
			this.bciDisable.Caption = "Disable";
			this.bciDisable.Checked = true;
			this.bciDisable.GroupIndex = 1;
			this.bciDisable.Id = 3;
			this.bciDisable.Name = "bciDisable";
			this.bciDisable.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bciDisable_ItemClick);
			this.bciDots.Caption = "Dots";
			this.bciDots.GroupIndex = 1;
			this.bciDots.Id = 4;
			this.bciDots.Name = "bciDots";
			this.bciDots.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bciDots_ItemClick);
			this.bsiHatch.Caption = "Hatch";
			this.bsiHatch.Id = 5;
			this.bsiHatch.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.bliHatch)});
			this.bsiHatch.Name = "bsiHatch";
			this.bliHatch.Caption = "barListItem1";
			this.bliHatch.Id = 6;
			this.bliHatch.Name = "bliHatch";
			this.bliHatch.ShowChecks = true;
			this.bliHatch.ListItemClick += new DevExpress.XtraBars.ListItemClickEventHandler(this.bliHatch_ListItemClick);
			this.bliHatch.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bliHatch_ItemClick);
			this.bsiHotTrack.Caption = "Choose hot-tracker";
			this.bsiHotTrack.Id = 2;
			this.bsiHotTrack.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.bciHint),
			new DevExpress.XtraBars.LinkPersistInfo(this.bciFrame)});
			this.bsiHotTrack.Name = "bsiHotTrack";
			this.bciHint.Caption = "Hint";
			this.bciHint.Checked = true;
			this.bciHint.GroupIndex = 2;
			this.bciHint.Id = 7;
			this.bciHint.Name = "bciHint";
			this.bciHint.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bciHint_ItemClick);
			this.bciFrame.Caption = "Frame";
			this.bciFrame.GroupIndex = 2;
			this.bciFrame.Id = 8;
			this.bciFrame.Name = "bciFrame";
			this.bciFrame.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bciFrame_ItemClick);
			this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAbout.Image = ((System.Drawing.Image)(resources.GetObject("btnAbout.Image")));
			this.btnAbout.Location = new System.Drawing.Point(304, 32);
			this.btnAbout.Name = "btnAbout";
			this.btnAbout.Size = new System.Drawing.Size(80, 32);
			this.btnAbout.TabIndex = 9;
			this.btnAbout.Text = "&About";
			this.btnAbout.Visible = false;
			this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
			this.lbFilter.Location = new System.Drawing.Point(3, 7);
			this.lbFilter.Name = "lbFilter";
			this.lbFilter.Size = new System.Drawing.Size(28, 13);
			this.lbFilter.TabIndex = 0;
			this.lbFilter.Text = "Filter:";
			this.tbFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbFilter.Location = new System.Drawing.Point(34, 3);
			this.tbFilter.MenuManager = this.barManager1;
			this.tbFilter.Name = "tbFilter";
			this.tbFilter.Size = new System.Drawing.Size(104, 20);
			this.tbFilter.TabIndex = 1;
			this.pnlFilter.Controls.Add(this.lbFilter);
			this.pnlFilter.Controls.Add(this.tbFilter);
			this.pnlFilter.Dock = System.Windows.Forms.DockStyle.Top;
			this.pnlFilter.Location = new System.Drawing.Point(22, 2);
			this.pnlFilter.Name = "pnlFilter";
			this.pnlFilter.Size = new System.Drawing.Size(142, 27);
			this.pnlFilter.TabIndex = 2;
			this.pnlFilter.Visible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(692, 475);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.MinimumSize = new System.Drawing.Size(700, 500);
			this.Name = "FrmMain";
			this.Text = "XtraEditors Tutorials";
			this.TutorialInfo.AboutFile = null;
			this.TutorialInfo.ImagePatternFill = null;
			this.TutorialInfo.ImageWhatsThisButton = ((System.Drawing.Image)(resources.GetObject("FrmMain.TutorialInfo.ImageWhatsThisButton")));
			this.TutorialInfo.ImageWhatsThisButtonStop = ((System.Drawing.Image)(resources.GetObject("FrmMain.TutorialInfo.ImageWhatsThisButtonStop")));
			this.TutorialInfo.SourceFileComment = null;
			this.TutorialInfo.SourceFileType = DevExpress.Tutorials.SourceFileType.CS;
			this.Load += new System.EventHandler(this.MainTutorialForm_Load);
			this.Move += new System.EventHandler(this.MainTutorialForm_Move);
			this.Resize += new System.EventHandler(this.MainTutorialForm_Resize);
			this.Controls.SetChildIndex(this.barDockControlTop, 0);
			this.Controls.SetChildIndex(this.barDockControlBottom, 0);
			this.Controls.SetChildIndex(this.barDockControlRight, 0);
			this.Controls.SetChildIndex(this.barDockControlLeft, 0);
			this.Controls.SetChildIndex(this.gcNavigations, 0);
			this.Controls.SetChildIndex(this.pnlCaption, 0);
			this.Controls.SetChildIndex(this.pcMain, 0);
			((System.ComponentModel.ISupportInitialize)(this.gcNavigations)).EndInit();
			this.gcNavigations.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcContainer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcDescription)).EndInit();
			this.gcDescription.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pcMain)).EndInit();
			this.pcMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbFilter.Properties)).EndInit();
			this.pnlFilter.ResumeLayout(false);
			this.pnlFilter.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private ImageShaderBase currentShader;
		public ModuleBase CurrentModule { 
			get {
				if(DesignMode) return null;
				return ModuleInfoCollection.CurrentModule; 
			} 
		}
		UserControl IWhatsThisProvider.CurrentModule{
			get {
				return CurrentModule;
			}
		}
		protected override void HideHint() {
			if (WhatsThisController != null) {
				WhatsThisController.HideHint();
			}
		}
		[Browsable(false)]
		public WhatsThisController WhatsThisController { get { return whatsThisController; } }
		[Browsable(false)]
		public bool WhatsThisEnabled { get { return whatsThisEnabled; } }
		private void InitWhatsThisModule() {
			whatsThisModule = new ModuleWhatsThis(whatsThisController);
			whatsThisModule.Parent = gcContainer;
			whatsThisModule.Dock = DockStyle.Fill;
			whatsThisModule.Visible = false;
		}
		protected virtual void NotifyModuleWhatsThisStateChange(bool whatsThisStarted) {
			if(CurrentModule == null) return;
			if(whatsThisStarted)
				CurrentModule.StartWhatsThis();
			else
				CurrentModule.EndWhatsThis();
			MainMenu.EnabledLookFeelMenu(!whatsThisStarted);
			bsiOptions.Enabled = !whatsThisStarted;
		}
		protected void EnableWhatsThis() {
			if(whatsThisEnabled) return;
			NotifyModuleWhatsThisStateChange(true);
			whatsThisEnabled = true;
			disableWhatsThisOnMove = true;
			if(CurrentModuleFitsInScreen()) {
				disableWhatsThisOnMove = false;
				prevFormLocation = this.Location;
			}
			whatsThisController.UpdateRegisteredVisibleControls();
			ShowWhatsThisModule();
			UpdateWhatsThisButton();
		}
		protected void DisableWhatsThis() {
			if(!whatsThisEnabled) return;
			NotifyModuleWhatsThisStateChange(false);
			whatsThisEnabled = false;
			HideWhatsThisModule();
			UpdateWhatsThisButton();
		}
		private void ShowWhatsThisModule() {
			whatsThisController.UpdateWhatsThisBitmaps();
			CurrentModule.Hide();
			whatsThisModule.Show();
		}
		private void HideWhatsThisModule() {
			whatsThisModule.Visible = false;
			CurrentModule.Show();
		}
		private void UpdateWhatsThisButton() {
			if(DesignMode) return;
			Image buttonImage = TutorialInfo.ImageWhatsThisButton;
			string buttonText = "What's this?";
			if(whatsThisEnabled) {
				buttonImage = TutorialInfo.ImageWhatsThisButtonStop;
				buttonText = "Stop";
			}
			btnWhatsThis.Text = buttonText;
			btnWhatsThis.Image = buttonImage;
			try {
				GraphicsInfo ginfo = new GraphicsInfo();
				ginfo.AddGraphics(null);
				try {
					btnWhatsThis.Width = btnWhatsThis.CalcBestFit(ginfo.Graphics).Width;
					btnWhatsThis.Left = btnWhatsThis.Parent.Width - btnWhatsThis.Width - 16;
				}
				finally {
					ginfo.ReleaseGraphics();
				}
			} catch {}
			UpdateAboutButton();
		}
		void UpdateAboutButton() {
			if(btnAbout.Parent == null) return;
			if(btnWhatsThis.Visible) {
				btnAbout.Left = btnWhatsThis.Left - btnAbout.Width - 8;
			} else {
				btnAbout.Left = btnAbout.Parent.Width - btnAbout.Width - 16;
			}
		}
		private bool CurrentModuleFitsInScreen() {
			Rectangle screenBounds = SystemInformation.WorkingArea;
			return screenBounds.Contains(CurrentModule.RectangleToScreen(CurrentModule.Bounds));
		}
		private void btnWhatsThis_Click(object sender, System.EventArgs e) {
			if(whatsThisEnabled)
				DisableWhatsThis();
			else {
				EnableWhatsThis();
			}
		}
		protected virtual string GetStartModuleName() {
			return string.Empty;
		}
		private void MainTutorialForm_Load(object sender, System.EventArgs e) {
			if(this.DesignMode) return;
			UpdateWhatsThisButton();
			InitMenu();
			InitModuleInformation();
		}
		protected virtual void InitModuleInformation() { }
		void SetItems() {
			FillHatchStyles();
			mainMenu.SetTutorialsMenu();
		}
		protected virtual void InitMenu() {
			mainMenu = CreateMenu(barManager1, defaultLookAndFeel);
			barManager1.BeginUpdate();
			try {
				SetItems();
			} finally {
				barManager1.EndUpdate();
			}
		}
		protected virtual DevExpress.DXperience.Demos.LookAndFeelMenu CreateMenu(BarManager manager, DefaultLookAndFeel lookAndFeel) {
			return new DevExpress.DXperience.Demos.LookAndFeelMenu(manager, lookAndFeel, string.Empty);
		}
		public virtual Type GetModuleType(string typeName) {
			return Type.GetType(typeName);
		}
		private void MainTutorialForm_Move(object sender, System.EventArgs e) {
			if(DesignMode) return;
			if(whatsThisEnabled && disableWhatsThisOnMove)
				DisableWhatsThis();
			if(whatsThisEnabled && !disableWhatsThisOnMove) {
				whatsThisController.Collection.UpdateControlRects(this.Location.X - prevFormLocation.X, this.Location.Y - prevFormLocation.Y);
				prevFormLocation = this.Location;
			}
		}
		private void MainTutorialForm_Resize(object sender, System.EventArgs e) {
			if(DesignMode) return;
			DisableWhatsThis();
		}
		protected void SelectModule(ModuleBase module, bool makeVisible) {
			if(module == null) return;
			DisableWhatsThis();
			UpdatePanelsWithModuleInfo(module);
			if(makeVisible)
				ModuleInfoCollection.ShowModule(module, gcContainer);
			else
				ModuleInfoCollection.SetCurrentModule(module);
			WhatsThisController.UpdateWhatsThisInfo(module.TutorialInfo.WhatsThisXMLFile, module.TutorialInfo.WhatsThisCodeFile);
			btnWhatsThis.Visible = WhatsThisController.IsWhatsThisInfoValid();
			btnAbout.Visible = GetAboutFileName(module.TutorialInfo.AboutFile) != "";
			pnlCaption.ShowLogo(!WhatsThisController.IsWhatsThisInfoValid() && !btnAbout.Visible);
			UpdateAboutButton();
		}
		string applicationStartupPath = "";
		string GetAboutFileName(string fileName) {
			return DevExpress.Utils.FilesHelper.FindingFileName(applicationStartupPath, fileName, false);
		}
		private void UpdatePanelsWithModuleInfo(ModuleBase module) {
			gcDescription.Parent.SuspendLayout();
			pnlCaption.Text = module.TutorialInfo.TutorialName;
			if(module.TutorialInfo.Description == string.Empty || module.TutorialInfo.Description == null) {
				gcDescription.Visible = false;
				horzSplitter.Visible = false;
			}
			else {
				pnlHint.Text = module.TutorialInfo.Description;
				gcDescription.Visible = true;
				horzSplitter.Visible = true;  
			}
			gcDescription.Parent.ResumeLayout();
		}
		private void bciDisable_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			ClearShaderItems(1);
			currentShader = new ImageShaderDisable();
		}
		private void bciDots_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			ClearShaderItems(1);
			currentShader = new ImageShaderPatternDots();
		}
		private void bciHint_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			whatsThisModule.SetHotTrackPainter(e.Item.Caption);
		}
		private void bciFrame_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			whatsThisModule.SetHotTrackPainter(e.Item.Caption);
		}
		public ImageShaderBase CurrentShader { get { return currentShader; } }
		protected void FillHatchStyles() {
			bliHatch.Strings.AddRange(Enum.GetNames(typeof(HatchStyle)));
		}
		private void ClearShaderItems(int groupIndex) {
			foreach(BarItemLink item in bsiShader.ItemLinks) {
				BarCheckItem ci = item.Item as BarCheckItem;	
				if(ci != null) {
					ci.GroupIndex = groupIndex;
					if(groupIndex == 0) ci.Checked = false;
					else bliHatch.ItemIndex = -1;
				}
			}
		}
		private void pnlCaption_SizeChanged(object sender, System.EventArgs e) {
			UpdateWhatsThisButton();
		}
		private void bliHatch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
		}
		private void bliHatch_ListItemClick(object sender, DevExpress.XtraBars.ListItemClickEventArgs e) {
			ClearShaderItems(0);
			HatchStyle hs = (HatchStyle)System.Enum.Parse(typeof(HatchStyle), ((BarListItem)e.Item).Strings[e.Index]);
			currentShader = new ImageShaderHatch(hs);	
		}
		private void btnAbout_Click(object sender, System.EventArgs e) {
			using(frmAbout dlg = new frmAbout(GetAboutFileName(CurrentModule.TutorialInfo.AboutFile), "About " + CurrentModule.TutorialInfo.TutorialName, this.FindForm().Icon))
				dlg.ShowDialog();
		}
		#region ITutorialForm Members
		bool ITutorialForm.IsFullMode {
			get { return !this.gcNavigations.Visible; }
		}
		bool ITutorialForm.AllowDemoFilter {
			get { return false; }
		}
		bool ITutorialForm.IsDemoFilterVisible {
			get { return false; }
		}
		public virtual void HideServiceElements() {
			DisableWhatsThis();
			gcDescription.Visible = horzSplitter.Visible = false;
			this.gcNavigations.Hide();
			this.pnlCaption.Hide();
		}
		public virtual void ShowServiceElements() {
			DisableWhatsThis();
			gcDescription.Visible = horzSplitter.Visible = CurrentModule.TutorialInfo.Description != string.Empty && CurrentModule.TutorialInfo.Description != null;
			this.gcNavigations.Show();
			this.pnlCaption.Show();
		}
		public void ShowDemoFilter() { }
		void ITutorialForm.ShowModule(string name) { }
		void ITutorialForm.ResetNavbarSelectedLink() { }
		#endregion
	}
}
