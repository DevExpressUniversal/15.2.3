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
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Helpers;
using DevExpress.XtraBars;
using DevExpress.LookAndFeel;
using DevExpress.Tutorials;
using DevExpress.Utils.About;
using DevExpress.XtraEditors;
using DevExpress.Tutorials.Properties;
using DevExpress.Tutorials.Controls;
using DevExpress.XtraEditors.ColorWheel;
using DevExpress.Skins;
using DevExpress.DXperience.Demos.CodeDemo;
using DevExpress.DemoData.Model;
using System.Linq;
using DevExpress.XtraBars.Navigation;
namespace DevExpress.DXperience.Demos {
	public class RibbonMainForm : RibbonForm, ITutorialForm, IWhatsThisProvider {
		protected DevExpress.LookAndFeel.DefaultLookAndFeel defaultLookAndFeel1;
		private DevExpress.XtraEditors.LabelControl lineLabel;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		protected DevExpress.Utils.Frames.ApplicationCaption8_1 pnlCaption;
		private DevExpress.XtraEditors.GroupControl gcNavigations;
		private DevExpress.XtraEditors.GroupControl gcContainer;
		private DevExpress.XtraEditors.PanelControl horzSplitter;
		protected AccordionControl accordionControl1;
		private DescriptionLabel gcDescription;
		private System.ComponentModel.IContainer components = null;
		protected RibbonControl ribbonControl1;
		private RibbonPage rpMain;
		private RibbonPageGroup rpgNavigation;
		private RibbonPageGroup rpgAppearance;
		private RibbonPageGroup rpgReserv1;
		private RibbonPageGroup rpgActiveDemo;
		private RibbonPageGroup rpgReserv2;
		private RibbonPageGroup rpgExport;
		private RibbonPageGroup rpgView;
		private DevExpress.XtraBars.BarSubItem bsiModules;
		private DevExpress.XtraBars.RibbonGalleryBarItem rgbiSkins;
		private DevExpress.XtraBars.PopupMenu pmAppearance;
		private DevExpress.XtraBars.BarCheckItem bciAllowFormSkin;
		private DevExpress.XtraBars.BarButtonItem bbiUp;
		private DevExpress.XtraBars.BarButtonItem bbiDown;
		private DevExpress.XtraBars.BarButtonItem bbiActiveDemo;
		private DevExpress.XtraBars.BarButtonItem bbiAbout;
		private DevExpress.XtraBars.BarCheckItem bciFullWindow;
		protected DevExpress.XtraBars.BarCheckItem bciFilter;
		private DevExpress.XtraBars.BarButtonItem bbiPrintPreview;
		private DevExpress.XtraBars.BarButtonItem bbiPrint;
		private DevExpress.XtraBars.BarButtonItem bbiExport;
		private DevExpress.XtraBars.PopupMenu pmExport;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoPDF;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoEPUB;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoXML;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoHTML;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoMHT;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoDOC;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoDOCX;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoXLS;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoXLSX;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoRTF;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoODT;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoImage;
		private DevExpress.XtraBars.BarButtonItem bbiExporttoText;
		AccordionControlElement prevLink = null;
		string startModule = string.Empty;
		bool fullWindow = false;
		string prevModuleName = "";
		RibbonMenuManager ribbonMenuManager = null;
		private RibbonPageGroup rpgAbout;
		private BarCheckItem bciShowRibbonPreview;
		private PopupMenu pmPrintOptions;
		private BarSubItem bsiExporttoImageEx;
		private BarButtonItem bbiCode;
		private RibbonPageGroup rpgCode;
		private RibbonPageCategory demoCategory;
		private BarButtonItem bbiGettingStarted;
		private BarButtonItem bbiGetFreeSupport;
		private BarButtonItem bbiBuyNow;
		protected internal RibbonStatusBar ribbonStatusBar;
		private BarButtonItem barButtonItem1;
		private BarButtonItem bbiShowInVisualStudio;
		private BarButtonItem bbiOpenSolution;
		private PopupMenu pmSolution;
		private BarButtonItem bbiCSSolution;
		private BarButtonItem bbiVBSolution;
		frmProgress progress = null;
		protected virtual bool AllowNavBarFilter { get { return true; } }
		protected virtual bool AllowSkinProgress { get { return true; } }
		protected virtual void SetFormParam() { }
		protected virtual void FillNavBar() {
			ModulesInfo.FillAccordionControl(accordionControl1);
		}
		protected virtual void ShowModule(string name, DevExpress.XtraEditors.GroupControl group, DevExpress.LookAndFeel.DefaultLookAndFeel lookAndFeel, DevExpress.Utils.Frames.ApplicationCaption caption) { }
		protected virtual void ShowModule(string name, DevExpress.XtraEditors.GroupControl group, DevExpress.LookAndFeel.DefaultLookAndFeel lookAndFeel, DevExpress.Utils.Frames.ApplicationCaption caption, Control notePanel) {
			ShowModule(name, group, lookAndFeel, caption);
		}
		protected DevExpress.XtraBars.BarManager Manager {
			get { return ribbonControl1.Manager; }
		}
		TutorialControlBase CurrentModule {
			get {
				return ModulesInfo.Instance.CurrentModuleBase.TModule as TutorialControlBase;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public RibbonControl RibbonControl { get { return ribbonControl1; } }
		protected virtual SourceFileType FileType { get { return SourceFileType.CS; } }
		protected virtual string DemoName { get { return "Features Demo (C# code)"; } }
		public RibbonMainForm() : this(new string[] { }) { }
		public RibbonMainForm(string[] arguments) {
			InitializeComponent();
			InitFileTypeInfo();
			DevExpress.Skins.SkinManager.EnableFormSkins();
			DevExpress.LookAndFeel.LookAndFeelHelper.ForceDefaultLookAndFeelChanged();
			SetFormParam();
				bciFilter.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
			UserLookAndFeel.Default.StyleChanged += new EventHandler(Default_StyleChanged);
			((DevExpress.LookAndFeel.Design.UserLookAndFeelDefault)UserLookAndFeel.Default).StyleChangeProgress += new LookAndFeelProgressEventHandler(Default_StyleChangeProgress);
			RegisterEnumTitles();
			SetFilterSize();
			whatsThisController = new WhatsThisController(this);
			InitWhatsThisModule();
			gcDescription.Appearance.Font = new Font(AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size + 1);
			gcDescription.AppearanceHovered.Font = new Font(AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size + 1);
			gcDescription.AppearanceDisabled.Font = new Font(AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size + 1);
			gcDescription.AppearancePressed.Font = new Font(AppearanceObject.DefaultFont.FontFamily, AppearanceObject.DefaultFont.Size + 1);
			InitOpenSolution(bbiCSSolution, false);
#if DEBUG 
			InitRTLItem();
#endif
			MainFormHelper.SetCommandLineArgs(Environment.GetCommandLineArgs(), out startModule, out fullWindow);  
		}
		void InitRTLItem() {
			BarCheckItem bciRightToLeft = new BarCheckItem();
			ribbonControl1.Items.Add(bciRightToLeft);
			bciRightToLeft.Caption = "RTL";
			bciRightToLeft.CheckBoxVisibility = DevExpress.XtraBars.CheckBoxVisibility.BeforeText;
			bciRightToLeft.CheckedChanged += (s, e) => {
				BarCheckItem ci = s as BarCheckItem;
				this.RightToLeftLayout = ci.Checked;
				WindowsFormsSettings.RightToLeft = ci.Checked ? DefaultBoolean.True : DefaultBoolean.False;
			};
			rpgView.ItemLinks.Add(bciRightToLeft);
		}
		void InitFileTypeInfo() {
			this.TutorialInfo.SourceFileType = FileType;
			this.TutorialInfo.SourceFileComment = FileType == SourceFileType.CS ? "//" : "'";
			MainFormHelper.SetBarButtonImage(bbiCode, FileType == SourceFileType.CS ? "C#" : "VB");
		}
		void SetFilterSize() {
		}
		protected virtual void RegisterEnumTitles() { }
		frmProgress Progress {
			get {
				if(progress == null) progress = new frmProgress(this);
				return progress;
			}
		}
		void Default_StyleChangeProgress(object sender, LookAndFeelProgressEventArgs e) {
			if(!AllowSkinProgress) return;
			if(e.State == 0) {
				Progress.ShowProgress(e.Progress);
				SuspendLayout();
			}
			if(e.State == 1) {
				Progress.Progress(e.Progress);
			}
			if(e.State == 2) {
				Progress.HideProgress();
				ResumeLayout();
			}
		}
		void Default_StyleChanged(object sender, EventArgs e) {
			if(accordionControl1.SelectedElement != null) {
				accordionControl1.SelectedElement.Visible = true;
				if(!accordionControl1.SelectedElement.OwnerElement.Expanded) accordionControl1.SelectedElement.OwnerElement.Expanded = true;
			}
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
				DevExpress.LookAndFeel.UserLookAndFeel.Default.StyleChanged -= new EventHandler(Default_StyleChanged);
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RibbonMainForm));
			this.gcNavigations = new DevExpress.XtraEditors.GroupControl();
			this.accordionControl1 = new DevExpress.XtraBars.Navigation.AccordionControl();
			this.lineLabel = new DevExpress.XtraEditors.LabelControl();
			this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.gcContainer = new DevExpress.XtraEditors.GroupControl();
			this.horzSplitter = new DevExpress.XtraEditors.PanelControl();
			this.gcDescription = new DevExpress.DXperience.Demos.DescriptionLabel();
			this.pnlCaption = new DevExpress.Utils.Frames.ApplicationCaption8_1();
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.bsiModules = new DevExpress.XtraBars.BarSubItem();
			this.rgbiSkins = new DevExpress.XtraBars.RibbonGalleryBarItem();
			this.bciAllowFormSkin = new DevExpress.XtraBars.BarCheckItem();
			this.bbiUp = new DevExpress.XtraBars.BarButtonItem();
			this.bbiDown = new DevExpress.XtraBars.BarButtonItem();
			this.bbiActiveDemo = new DevExpress.XtraBars.BarButtonItem();
			this.bbiAbout = new DevExpress.XtraBars.BarButtonItem();
			this.bciFullWindow = new DevExpress.XtraBars.BarCheckItem();
			this.bciFilter = new DevExpress.XtraBars.BarCheckItem();
			this.bbiPrintPreview = new DevExpress.XtraBars.BarButtonItem();
			this.bbiPrint = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExport = new DevExpress.XtraBars.BarButtonItem();
			this.pmExport = new DevExpress.XtraBars.PopupMenu(this.components);
			this.bbiExporttoPDF = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoEPUB = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoXML = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoHTML = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoMHT = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoDOC = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoDOCX = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoXLS = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoXLSX = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoRTF = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoODT = new DevExpress.XtraBars.BarButtonItem();
			this.bbiExporttoImage = new DevExpress.XtraBars.BarButtonItem();
			this.bsiExporttoImageEx = new DevExpress.XtraBars.BarSubItem();
			this.bbiExporttoText = new DevExpress.XtraBars.BarButtonItem();
			this.bciShowRibbonPreview = new DevExpress.XtraBars.BarCheckItem();
			this.bbiCode = new DevExpress.XtraBars.BarButtonItem();
			this.bbiGettingStarted = new DevExpress.XtraBars.BarButtonItem();
			this.bbiGetFreeSupport = new DevExpress.XtraBars.BarButtonItem();
			this.bbiBuyNow = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.bbiShowInVisualStudio = new DevExpress.XtraBars.BarButtonItem();
			this.bbiOpenSolution = new DevExpress.XtraBars.BarButtonItem();
			this.pmSolution = new DevExpress.XtraBars.PopupMenu(this.components);
			this.bbiCSSolution = new DevExpress.XtraBars.BarButtonItem();
			this.bbiVBSolution = new DevExpress.XtraBars.BarButtonItem();
			this.demoCategory = new DevExpress.XtraBars.Ribbon.RibbonPageCategory();
			this.rpMain = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.rpgNavigation = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.rpgAppearance = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.rpgReserv1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.rpgReserv2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.rpgExport = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.rpgView = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.rpgCode = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.rpgActiveDemo = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.rpgAbout = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonStatusBar = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
			this.pmAppearance = new DevExpress.XtraBars.PopupMenu(this.components);
			this.pmPrintOptions = new DevExpress.XtraBars.PopupMenu(this.components);
			((System.ComponentModel.ISupportInitialize)(this.gcNavigations)).BeginInit();
			this.gcNavigations.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcContainer)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pmExport)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pmSolution)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pmAppearance)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pmPrintOptions)).BeginInit();
			this.SuspendLayout();
			this.gcNavigations.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gcNavigations.CaptionLocation = DevExpress.Utils.Locations.Left;
			this.gcNavigations.Controls.Add(this.accordionControl1);
			this.gcNavigations.Controls.Add(this.lineLabel);
			resources.ApplyResources(this.gcNavigations, "gcNavigations");
			this.gcNavigations.Name = "gcNavigations";
			this.gcNavigations.ShowCaption = false;
			resources.ApplyResources(this.accordionControl1, "accordionControl1");
			this.accordionControl1.Name = "accordionControl1";
			this.accordionControl1.SelectedElement = null;
			this.accordionControl1.ShowGroupExpandButtons = false;
			this.accordionControl1.AllowItemSelection = true;
			this.accordionControl1.ExpandElementMode = ExpandElementMode.Multiple;
			this.accordionControl1.SelectedElementChanged += new SelectedElementChangedEventHandler(this.accordionControl1_SelectedLinkChanged);
			this.accordionControl1.ElementClick += new ElementClickEventHandler(this.accordionControl1_LinkClicked);
			this.accordionControl1.CustomElementText += new CustomElementTextEventHandler(this.accordionControl1_CustomElementText);
			this.accordionControl1.CustomDrawElement += new CustomDrawElementEventHandler(this.accordionControl1_CustomDrawElement);
			this.accordionControl1.ScrollBarMode = ScrollBarMode.Touch;
			this.accordionControl1.ShowFilterControl = ShowFilterControl.Always;
			resources.ApplyResources(this.lineLabel, "lineLabel");
			this.lineLabel.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.lineLabel.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Vertical;
			this.lineLabel.LineVisible = true;
			this.lineLabel.Name = "lineLabel";
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.gcContainer);
			this.panelControl1.Controls.Add(this.horzSplitter);
			this.panelControl1.Controls.Add(this.gcDescription);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			this.gcContainer.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.gcContainer, "gcContainer");
			this.gcContainer.Name = "gcContainer";
			this.gcContainer.ShowCaption = false;
			this.horzSplitter.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			this.horzSplitter.Name = "horzSplitter";
			this.gcDescription.Appearance.Image = ((System.Drawing.Image)(resources.GetObject("gcDescription.Appearance.Image")));
			this.gcDescription.Appearance.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.gcDescription.AppearanceDisabled.Image = ((System.Drawing.Image)(resources.GetObject("gcDescription.AppearanceDisabled.Image")));
			this.gcDescription.AppearanceDisabled.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.gcDescription.AppearanceHovered.Image = ((System.Drawing.Image)(resources.GetObject("gcDescription.AppearanceHovered.Image")));
			this.gcDescription.AppearanceHovered.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			this.gcDescription.AppearancePressed.Image = ((System.Drawing.Image)(resources.GetObject("gcDescription.AppearancePressed.Image")));
			this.gcDescription.AppearancePressed.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
			resources.ApplyResources(this.gcDescription, "gcDescription");
			this.gcDescription.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
			this.gcDescription.Name = "gcDescription";
			this.gcDescription.UseMnemonic = false;
			resources.ApplyResources(this.pnlCaption, "pnlCaption");
			this.pnlCaption.Name = "pnlCaption";
			this.pnlCaption.TabStop = false;
			this.ribbonControl1.AllowMinimizeRibbon = false;
			resources.ApplyResources(this.ribbonControl1, "ribbonControl1");
			this.ribbonControl1.AutoSizeItems = true;
			this.ribbonControl1.ExpandCollapseItem.Id = 0;
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.ribbonControl1.ExpandCollapseItem,
			this.ribbonControl1.AutoHiddenPagesMenuItem,
			this.bsiModules,
			this.rgbiSkins,
			this.bciAllowFormSkin,
			this.bbiUp,
			this.bbiDown,
			this.bbiActiveDemo,
			this.bbiAbout,
			this.bciFullWindow,
			this.bciFilter,
			this.bbiPrintPreview,
			this.bbiPrint,
			this.bbiExport,
			this.bbiExporttoPDF,
			this.bbiExporttoEPUB,
			this.bbiExporttoXML,
			this.bbiExporttoHTML,
			this.bbiExporttoMHT,
			this.bbiExporttoDOC,
			this.bbiExporttoDOCX,
			this.bbiExporttoXLS,
			this.bbiExporttoXLSX,
			this.bbiExporttoRTF,
			this.bbiExporttoODT,
			this.bbiExporttoImage,
			this.bbiExporttoText,
			this.bciShowRibbonPreview,
			this.bsiExporttoImageEx,
			this.bbiCode,
			this.bbiGettingStarted,
			this.bbiGetFreeSupport,
			this.bbiBuyNow,
			this.barButtonItem1,
			this.bbiShowInVisualStudio,
			this.bbiOpenSolution,
			this.bbiCSSolution,
			this.bbiVBSolution});
			this.ribbonControl1.MaxItemId = 43;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.PageCategories.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageCategory[] {
			this.demoCategory});
			this.ribbonControl1.PageCategoryAlignment = DevExpress.XtraBars.Ribbon.RibbonPageCategoryAlignment.Left;
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.rpMain});
			this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2010;
			this.ribbonControl1.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.ShowOnMultiplePages;
			this.ribbonControl1.StatusBar = this.ribbonStatusBar;
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			this.ribbonControl1.Merge += new DevExpress.XtraBars.Ribbon.RibbonMergeEventHandler(this.ribbonControl1_Merge);
			this.ribbonControl1.UnMerge += new DevExpress.XtraBars.Ribbon.RibbonMergeEventHandler(this.ribbonControl1_UnMerge);
			this.ribbonControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.ribbonControl1_Paint);
			resources.ApplyResources(this.bsiModules, "bsiModules");
			this.bsiModules.Id = 1;
			this.bsiModules.Name = "bsiModules";
			this.bsiModules.ShowNavigationHeader = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.rgbiSkins, "rgbiSkins");
			this.rgbiSkins.Id = 2;
			this.rgbiSkins.Name = "rgbiSkins";
			resources.ApplyResources(this.bciAllowFormSkin, "bciAllowFormSkin");
			this.bciAllowFormSkin.Id = 4;
			this.bciAllowFormSkin.Name = "bciAllowFormSkin";
			this.bciAllowFormSkin.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bciAllowFormSkin_ItemClick);
			resources.ApplyResources(this.bbiUp, "bbiUp");
			this.bbiUp.Id = 5;
			this.bbiUp.Name = "bbiUp";
			this.bbiUp.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.bbiUp.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiUp_ItemClick);
			resources.ApplyResources(this.bbiDown, "bbiDown");
			this.bbiDown.Id = 6;
			this.bbiDown.Name = "bbiDown";
			this.bbiDown.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.SmallWithText;
			this.bbiDown.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiDown_ItemClick);
			resources.ApplyResources(this.bbiActiveDemo, "bbiActiveDemo");
			this.bbiActiveDemo.Id = 7;
			this.bbiActiveDemo.Name = "bbiActiveDemo";
			this.bbiActiveDemo.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiActiveDemo_ItemClick);
			resources.ApplyResources(this.bbiAbout, "bbiAbout");
			this.bbiAbout.Id = 8;
			this.bbiAbout.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F1);
			this.bbiAbout.Name = "bbiAbout";
			this.bbiAbout.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiAbout_ItemClick);
			resources.ApplyResources(this.bciFullWindow, "bciFullWindow");
			this.bciFullWindow.Id = 11;
			this.bciFullWindow.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F11);
			this.bciFullWindow.Name = "bciFullWindow";
			this.bciFullWindow.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.bciFullWindow_CheckedChanged);
			resources.ApplyResources(this.bciFilter, "bciFilter");
			this.bciFilter.Id = 12;
			this.bciFilter.ItemShortcut = new DevExpress.XtraBars.BarShortcut(System.Windows.Forms.Keys.F3);
			this.bciFilter.Name = "bciFilter";
			this.bciFilter.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.bciFilter_CheckedChanged);
			resources.ApplyResources(this.bbiPrintPreview, "bbiPrintPreview");
			this.bbiPrintPreview.Id = 13;
			this.bbiPrintPreview.Name = "bbiPrintPreview";
			resources.ApplyResources(this.bbiPrint, "bbiPrint");
			this.bbiPrint.Id = 25;
			this.bbiPrint.Name = "bbiPrint";
			this.bbiExport.ActAsDropDown = true;
			this.bbiExport.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			resources.ApplyResources(this.bbiExport, "bbiExport");
			this.bbiExport.DropDownControl = this.pmExport;
			this.bbiExport.Id = 14;
			this.bbiExport.Name = "bbiExport";
			this.pmExport.ItemLinks.Add(this.bbiExporttoPDF);
			this.pmExport.ItemLinks.Add(this.bbiExporttoEPUB);
			this.pmExport.ItemLinks.Add(this.bbiExporttoXML);
			this.pmExport.ItemLinks.Add(this.bbiExporttoHTML);
			this.pmExport.ItemLinks.Add(this.bbiExporttoMHT);
			this.pmExport.ItemLinks.Add(this.bbiExporttoDOC);
			this.pmExport.ItemLinks.Add(this.bbiExporttoDOCX);
			this.pmExport.ItemLinks.Add(this.bbiExporttoXLS);
			this.pmExport.ItemLinks.Add(this.bbiExporttoXLSX);
			this.pmExport.ItemLinks.Add(this.bbiExporttoRTF);
			this.pmExport.ItemLinks.Add(this.bbiExporttoODT);
			this.pmExport.ItemLinks.Add(this.bbiExporttoImage);
			this.pmExport.ItemLinks.Add(this.bsiExporttoImageEx);
			this.pmExport.ItemLinks.Add(this.bbiExporttoText);
			this.pmExport.MenuDrawMode = DevExpress.XtraBars.MenuDrawMode.SmallImagesText;
			this.pmExport.Name = "pmExport";
			this.pmExport.Ribbon = this.ribbonControl1;
			this.pmExport.ShowNavigationHeader = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.bbiExporttoPDF, "bbiExporttoPDF");
			this.bbiExporttoPDF.Id = 15;
			this.bbiExporttoPDF.Name = "bbiExporttoPDF";
			resources.ApplyResources(this.bbiExporttoEPUB, "bbiExporttoEPUB");
			this.bbiExporttoEPUB.Id = 26;
			this.bbiExporttoEPUB.Name = "bbiExporttoEPUB";
			resources.ApplyResources(this.bbiExporttoXML, "bbiExporttoXML");
			this.bbiExporttoXML.Id = 16;
			this.bbiExporttoXML.Name = "bbiExporttoXML";
			resources.ApplyResources(this.bbiExporttoHTML, "bbiExporttoHTML");
			this.bbiExporttoHTML.Id = 17;
			this.bbiExporttoHTML.Name = "bbiExporttoHTML";
			resources.ApplyResources(this.bbiExporttoMHT, "bbiExporttoMHT");
			this.bbiExporttoMHT.Id = 18;
			this.bbiExporttoMHT.Name = "bbiExporttoMHT";
			resources.ApplyResources(this.bbiExporttoDOC, "bbiExporttoDOC");
			this.bbiExporttoDOC.Id = 27;
			this.bbiExporttoDOC.Name = "bbiExporttoDOC";
			resources.ApplyResources(this.bbiExporttoDOCX, "bbiExporttoDOCX");
			this.bbiExporttoDOCX.Id = 28;
			this.bbiExporttoDOCX.Name = "bbiExporttoDOCX";
			resources.ApplyResources(this.bbiExporttoXLS, "bbiExporttoXLS");
			this.bbiExporttoXLS.Id = 19;
			this.bbiExporttoXLS.Name = "bbiExporttoXLS";
			resources.ApplyResources(this.bbiExporttoXLSX, "bbiExporttoXLSX");
			this.bbiExporttoXLSX.Id = 20;
			this.bbiExporttoXLSX.Name = "bbiExporttoXLSX";
			resources.ApplyResources(this.bbiExporttoRTF, "bbiExporttoRTF");
			this.bbiExporttoRTF.Id = 21;
			this.bbiExporttoRTF.Name = "bbiExporttoRTF";
			resources.ApplyResources(this.bbiExporttoODT, "bbiExporttoODT");
			this.bbiExporttoODT.Id = 29;
			this.bbiExporttoODT.Name = "bbiExporttoODT";
			resources.ApplyResources(this.bbiExporttoImage, "bbiExporttoImage");
			this.bbiExporttoImage.Id = 22;
			this.bbiExporttoImage.Name = "bbiExporttoImage";
			resources.ApplyResources(this.bsiExporttoImageEx, "bsiExporttoImageEx");
			this.bsiExporttoImageEx.Id = 30;
			this.bsiExporttoImageEx.Name = "bsiExporttoImageEx";
			this.bsiExporttoImageEx.ShowNavigationHeader = DevExpress.Utils.DefaultBoolean.True;
			resources.ApplyResources(this.bbiExporttoText, "bbiExporttoText");
			this.bbiExporttoText.Id = 23;
			this.bbiExporttoText.Name = "bbiExporttoText";
			resources.ApplyResources(this.bciShowRibbonPreview, "bciShowRibbonPreview");
			this.bciShowRibbonPreview.Id = 24;
			this.bciShowRibbonPreview.Name = "bciShowRibbonPreview";
			this.bciShowRibbonPreview.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bciShowRibbonPreview_ItemClick);
			this.bbiCode.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
			resources.ApplyResources(this.bbiCode, "bbiCode");
			this.bbiCode.Id = 32;
			this.bbiCode.Name = "bbiCode";
			this.bbiCode.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiCode_DownChanged);
			resources.ApplyResources(this.bbiGettingStarted, "bbiGettingStarted");
			this.bbiGettingStarted.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
			this.bbiGettingStarted.Id = 33;
			this.bbiGettingStarted.Name = "bbiGettingStarted";
			this.bbiGettingStarted.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiGettingStarted_ItemClick);
			resources.ApplyResources(this.bbiGetFreeSupport, "bbiGetFreeSupport");
			this.bbiGetFreeSupport.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
			this.bbiGetFreeSupport.Id = 34;
			this.bbiGetFreeSupport.Name = "bbiGetFreeSupport";
			this.bbiGetFreeSupport.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiGetFreeSupport_ItemClick);
			resources.ApplyResources(this.bbiBuyNow, "bbiBuyNow");
			this.bbiBuyNow.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
			this.bbiBuyNow.Id = 35;
			this.bbiBuyNow.Name = "bbiBuyNow";
			this.bbiBuyNow.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiBuyNow_ItemClick);
			resources.ApplyResources(this.barButtonItem1, "barButtonItem1");
			this.barButtonItem1.Glyph = global::DevExpress.Tutorials.Properties.Resources.ColorMixer_16x16;
			this.barButtonItem1.Id = 38;
			this.barButtonItem1.LargeGlyph = global::DevExpress.Tutorials.Properties.Resources.ColorMixer_32x32;
			this.barButtonItem1.Name = "barButtonItem1";
			this.barButtonItem1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barButtonItem1_ItemClick);
			resources.ApplyResources(this.bbiShowInVisualStudio, "bbiShowInVisualStudio");
			this.bbiShowInVisualStudio.Id = 39;
			this.bbiShowInVisualStudio.Name = "bbiShowInVisualStudio";
			this.bbiShowInVisualStudio.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.bbiShowInVisualStudio.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
			this.bbiOpenSolution.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			resources.ApplyResources(this.bbiOpenSolution, "bbiOpenSolution");
			this.bbiOpenSolution.DropDownControl = this.pmSolution;
			this.bbiOpenSolution.Id = 40;
			this.bbiOpenSolution.Name = "bbiOpenSolution";
			this.bbiOpenSolution.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiOpenSolution_ItemClick);
			this.pmSolution.ItemLinks.Add(this.bbiCSSolution);
			this.pmSolution.ItemLinks.Add(this.bbiVBSolution);
			this.pmSolution.Name = "pmSolution";
			this.pmSolution.Ribbon = this.ribbonControl1;
			resources.ApplyResources(this.bbiCSSolution, "bbiCSSolution");
			this.bbiCSSolution.Glyph = ((System.Drawing.Image)(resources.GetObject("bbiCSSolution.Glyph")));
			this.bbiCSSolution.Id = 41;
			this.bbiCSSolution.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("bbiCSSolution.LargeGlyph")));
			this.bbiCSSolution.Name = "bbiCSSolution";
			this.bbiCSSolution.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiCSSolution_ItemClick);
			resources.ApplyResources(this.bbiVBSolution, "bbiVBSolution");
			this.bbiVBSolution.Glyph = ((System.Drawing.Image)(resources.GetObject("bbiVBSolution.Glyph")));
			this.bbiVBSolution.Id = 42;
			this.bbiVBSolution.LargeGlyph = ((System.Drawing.Image)(resources.GetObject("bbiVBSolution.LargeGlyph")));
			this.bbiVBSolution.Name = "bbiVBSolution";
			this.bbiVBSolution.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bbiVBSolution_ItemClick);
			resources.ApplyResources(this.demoCategory, "demoCategory");
			this.demoCategory.Name = "demoCategory";
			this.rpMain.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.rpgNavigation,
			this.rpgAppearance,
			this.rpgReserv1,
			this.rpgReserv2,
			this.rpgExport,
			this.rpgView,
			this.rpgCode,
			this.rpgActiveDemo,
			this.rpgAbout});
			this.rpMain.Name = "rpMain";
			resources.ApplyResources(this.rpMain, "rpMain");
			this.rpgNavigation.ItemLinks.Add(this.bsiModules);
			this.rpgNavigation.ItemLinks.Add(this.bbiUp, true);
			this.rpgNavigation.ItemLinks.Add(this.bbiDown);
			this.rpgNavigation.Name = "rpgNavigation";
			this.rpgNavigation.ShowCaptionButton = false;
			resources.ApplyResources(this.rpgNavigation, "rpgNavigation");
			this.rpgAppearance.AllowTextClipping = false;
			this.rpgAppearance.ItemLinks.Add(this.rgbiSkins);
			this.rpgAppearance.ItemLinks.Add(this.barButtonItem1);
			this.rpgAppearance.Name = "rpgAppearance";
			resources.ApplyResources(this.rpgAppearance, "rpgAppearance");
			this.rpgAppearance.CaptionButtonClick += new DevExpress.XtraBars.Ribbon.RibbonPageGroupEventHandler(this.rpgAppearance_CaptionButtonClick);
			this.rpgReserv1.AllowTextClipping = false;
			this.rpgReserv1.Name = "rpgReserv1";
			this.rpgReserv1.ShowCaptionButton = false;
			this.rpgReserv1.Visible = false;
			this.rpgReserv2.AllowTextClipping = false;
			this.rpgReserv2.Name = "rpgReserv2";
			this.rpgReserv2.ShowCaptionButton = false;
			this.rpgReserv2.Visible = false;
			this.rpgExport.AllowTextClipping = false;
			this.rpgExport.ItemLinks.Add(this.bbiPrintPreview);
			this.rpgExport.ItemLinks.Add(this.bbiPrint);
			this.rpgExport.ItemLinks.Add(this.bbiExport);
			this.rpgExport.Name = "rpgExport";
			this.rpgExport.ShowCaptionButton = false;
			resources.ApplyResources(this.rpgExport, "rpgExport");
			this.rpgExport.Visible = false;
			this.rpgExport.CaptionButtonClick += new DevExpress.XtraBars.Ribbon.RibbonPageGroupEventHandler(this.rpgExport_CaptionButtonClick);
			this.rpgView.ItemLinks.Add(this.bciFullWindow);
			this.rpgView.ItemLinks.Add(this.bciFilter);
			this.rpgView.Name = "rpgView";
			this.rpgView.ShowCaptionButton = false;
			resources.ApplyResources(this.rpgView, "rpgView");
			this.rpgCode.ItemLinks.Add(this.bbiShowInVisualStudio);
			this.rpgCode.ItemLinks.Add(this.bbiCode);
			this.rpgCode.ItemLinks.Add(this.bbiOpenSolution);
			this.rpgCode.Name = "rpgCode";
			this.rpgCode.ShowCaptionButton = false;
			resources.ApplyResources(this.rpgCode, "rpgCode");
			this.rpgActiveDemo.AllowTextClipping = false;
			this.rpgActiveDemo.ItemLinks.Add(this.bbiActiveDemo);
			this.rpgActiveDemo.Name = "rpgActiveDemo";
			this.rpgActiveDemo.ShowCaptionButton = false;
			resources.ApplyResources(this.rpgActiveDemo, "rpgActiveDemo");
			this.rpgActiveDemo.Visible = false;
			this.rpgAbout.AllowTextClipping = false;
			this.rpgAbout.ItemLinks.Add(this.bbiGettingStarted);
			this.rpgAbout.ItemLinks.Add(this.bbiGetFreeSupport);
			this.rpgAbout.ItemLinks.Add(this.bbiBuyNow);
			this.rpgAbout.ItemLinks.Add(this.bbiAbout);
			this.rpgAbout.Name = "rpgAbout";
			this.rpgAbout.ShowCaptionButton = false;
			resources.ApplyResources(this.rpgAbout, "rpgAbout");
			resources.ApplyResources(this.ribbonStatusBar, "ribbonStatusBar");
			this.ribbonStatusBar.Name = "ribbonStatusBar";
			this.ribbonStatusBar.Ribbon = this.ribbonControl1;
			this.pmAppearance.ItemLinks.Add(this.bciAllowFormSkin);
			this.pmAppearance.MenuDrawMode = DevExpress.XtraBars.MenuDrawMode.SmallImagesText;
			this.pmAppearance.Name = "pmAppearance";
			this.pmAppearance.Ribbon = this.ribbonControl1;
			this.pmAppearance.ShowNavigationHeader = DevExpress.Utils.DefaultBoolean.True;
			this.pmAppearance.Popup += new System.EventHandler(this.pmAppearance_Popup);
			this.pmPrintOptions.ItemLinks.Add(this.bciShowRibbonPreview);
			this.pmPrintOptions.MenuDrawMode = DevExpress.XtraBars.MenuDrawMode.SmallImagesText;
			this.pmPrintOptions.Name = "pmPrintOptions";
			this.pmPrintOptions.Ribbon = this.ribbonControl1;
			this.pmPrintOptions.ShowNavigationHeader = DevExpress.Utils.DefaultBoolean.True;
			this.pmPrintOptions.BeforePopup += new System.ComponentModel.CancelEventHandler(this.pmPrintOptions_BeforePopup);
			this.AllowFormGlass = DevExpress.Utils.DefaultBoolean.False;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.pnlCaption);
			this.Controls.Add(this.gcNavigations);
			this.Controls.Add(this.ribbonStatusBar);
			this.Controls.Add(this.ribbonControl1);
			this.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
			this.KeyPreview = true;
			this.Name = "RibbonMainForm";
			this.Ribbon = this.ribbonControl1;
			this.StatusBar = this.ribbonStatusBar;
			this.MinimumSize = new Size(950, 750);
			this.Load += new System.EventHandler(this.OnLoad);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
			this.Move += new System.EventHandler(this.RibbonMainForm_Move);
			this.Resize += new System.EventHandler(this.RibbonMainForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.gcNavigations)).EndInit();
			this.gcNavigations.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.accordionControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcContainer)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pmExport)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pmSolution)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pmAppearance)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pmPrintOptions)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected virtual void ShowAbout() { }
		public RibbonMenuManager RibbonMenuManager { get { return ribbonMenuManager; } }
		protected virtual RibbonMenuManager CreateRibbonMenuManager() {
			return new RibbonMenuManager(this);
		}
		protected virtual void CreateRibbonMenu() {
			AccordionNavigationMenuHelper.CreateNavigationMenu(bsiModules, accordionControl1, Manager);
			SkinHelper.InitSkinPopupMenu(pmAppearance);
			SkinHelper.InitSkinGallery(rgbiSkins, true);
			rgbiSkins.GalleryInitDropDownGallery += new InplaceGalleryEventHandler(rgbiSkins_GalleryInitDropDownGallery);
			pmAppearance.ItemLinks[1].BeginGroup = true;
		}
		void rgbiSkins_GalleryInitDropDownGallery(object sender, InplaceGalleryEventArgs e) {
			e.PopupGallery.GalleryDropDown.ItemLinks.Add(bciAllowFormSkin);
			bciAllowFormSkin.Checked = Skins.SkinManager.AllowFormSkins;
		}
		protected virtual void InitBarItemImages() {
			MainFormHelper.SetBarButtonImage(bsiModules, "Navigation");
			MainFormHelper.SetBarButtonImage(bbiUp, "Previous");
			MainFormHelper.SetBarButtonImage(bbiDown, "Next");
			MainFormHelper.SetBarButtonImage(bbiActiveDemo, "ActiveDemo");
			MainFormHelper.SetBarButtonImage(bbiAbout, "About");
			MainFormHelper.SetBarButtonImage(bbiBuyNow, "BuyNow");
			MainFormHelper.SetBarButtonImage(bbiGettingStarted, "GetStarted");
			MainFormHelper.SetBarButtonImage(bbiGetFreeSupport, "GetSupport");
			MainFormHelper.SetBarButtonImage(bciFullWindow, "FullWindowView");
			MainFormHelper.SetBarButtonImage(bciFilter, "Filter");
			MainFormHelper.SetBarButtonImage(bbiPrintPreview, "Preview");
			MainFormHelper.SetBarButtonImage(bbiPrint, "Print");
			MainFormHelper.SetBarButtonImage(bbiExport, "Export");
			MainFormHelper.SetBarButtonImage(bbiExporttoPDF, "ExportToPDF");
			MainFormHelper.SetBarButtonImage(bbiExporttoEPUB, "ExportToEPUB");
			MainFormHelper.SetBarButtonImage(bbiExporttoXML, "ExportToXML");
			MainFormHelper.SetBarButtonImage(bbiExporttoHTML, "ExportToHTML");
			MainFormHelper.SetBarButtonImage(bbiExporttoMHT, "ExportToMHT");
			MainFormHelper.SetBarButtonImage(bbiExporttoDOC, "ExportToDOC");
			MainFormHelper.SetBarButtonImage(bbiExporttoDOCX, "ExportToDOCX");
			MainFormHelper.SetBarButtonImage(bbiExporttoXLS, "ExportToExcel");
			MainFormHelper.SetBarButtonImage(bbiExporttoXLSX, "ExportToExcel");
			MainFormHelper.SetBarButtonImage(bbiExporttoRTF, "ExportToRTF");
			MainFormHelper.SetBarButtonImage(bbiExporttoODT, "ExportToODT");
			MainFormHelper.SetBarButtonImage(bbiExporttoImage, "ExportToIMG");
			MainFormHelper.SetBarButtonImage(bsiExporttoImageEx, "ExportToIMG");
			MainFormHelper.SetBarButtonImage(bbiExporttoText, "ExportToTXT");
			PrintPreviewButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.PrintPreview(); });
			PrintButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.Print(); });
			ExportToPDFButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToPDF(); });
			ExportToEPUBButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToEPUB(); });
			ExportToXMLButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToXML(); });
			ExportToHTMLButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToHTML(); });
			ExportToMHTButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToMHT(); });
			ExportToDOCButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToDOC(); });
			ExportToDOCXButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToDOCX(); });
			ExportToXLSButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToXLS(); });
			ExportToXLSXButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToXLSX(); });
			ExportToRTFButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToRTF(); });
			ExportToODTButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToODT(); });
			ExportToImageButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToImage(); });
			ExportToTextButton.ItemClick += new ItemClickEventHandler(delegate { CurrentModule.ExportToText(); });
		}
		protected virtual int CustomWidth { get { return 1270; } }
		protected virtual int CustomHeight { get { return 855; } }
		protected virtual void OnLoad(object sender, System.EventArgs e) {
			MainFormHelper.SetFormClientSize(Screen.GetWorkingArea(this.Location), this, CustomWidth, CustomHeight);
			MainFormHelper.RegisterRibbonDefaultBonusSkin();
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if(entryAssembly == null) return;
			object[] attributes = entryAssembly.GetCustomAttributes(typeof(ProductIdAttribute), false);
			if(attributes.Length == 0)
				throw new Exception("Every demo application must have the ProductId assembly attribute");
			ProductIdAttribute productIdAttribute = (ProductIdAttribute)attributes[0];
			MainFormRegisterDemoHelper.RegisterDemos(productIdAttribute.ProductId);
			FillNavBar();
			RemoveNavBarItems();
			InitBarItemImages();
			CreateRibbonMenu();
			ribbonMenuManager = CreateRibbonMenuManager();
			ModuleInfo info = null;
			if(!startModule.Equals(string.Empty)) {
				info = ModulesInfo.GetItemByType(startModule);
				MainFormHelper.SelectAccordionControlItem(accordionControl1, startModule);
			}
			if(info == null) {
				info = ModulesInfo.GetItem(IsAllowAboutModule ? 0 : DefaultModuleIndex);
				if(info != null) {
					ShowModule(info.Name);
					MainFormHelper.SelectAccordionControlItem(accordionControl1, info.FullTypeName);
				}
			}
			if(fullWindow) {
				bciFullWindow.Checked = true;
				gcDescription.Visible = horzSplitter.Visible = ModulesInfo.Instance.CurrentModuleBase.Description != "";
			}
		}
		protected virtual string NotTranslatedModuleTypes { get { return string.Empty; } }
		protected virtual void RemoveNavBarItems() {
			foreach(AccordionControlElement group in accordionControl1.Elements) {
				for(int i = group.Elements.Count - 1; i >= 0; i--) {
					DevExpress.DXperience.Demos.ModuleInfo info = group.Elements[i].Tag as DevExpress.DXperience.Demos.ModuleInfo;
					if(info == null) return;
					if(NotTranslatedModuleTypes.Contains(string.Format(";{0};", info.TypeName)))
						group.Elements.RemoveAt(i);
				}
				group.Visible = group.Elements.Count != 0;
			}
		}
		public void ResetNavbarSelectedLink() {
			this.accordionControl1.SelectedElement = null;
		}
		protected virtual bool ShowPanelDescription {
			get { return (ModulesInfo.Instance.CurrentModuleBase != null) && ModulesInfo.Instance.CurrentModuleBase.Description != ""; }
		}
		protected virtual bool AllowDescriptionText {
			get { return (ModulesInfo.Instance.CurrentModuleBase != null) && ModulesInfo.Instance.CurrentModuleBase.Description != "*"; }
		}
		public void ShowModule(string name) {
			CloseWhatsThis();
			if(ModulesInfo.Instance.CurrentModuleBase != null) prevModuleName = ModulesInfo.Instance.CurrentModuleBase.Name;
			gcContainer.Parent.SuspendLayout();
			gcContainer.SuspendLayout();
			ShowModule(name, gcContainer, defaultLookAndFeel1, pnlCaption, gcDescription);
			gcDescription.Visible = horzSplitter.Visible = ShowPanelDescription;
			if(AllowDescriptionText)
				gcDescription.Text = ModulesInfo.Instance.CurrentModuleBase.Description;
			if(CurrentModule == null) return;
			ShowCaption();
			gcContainer.ResumeLayout();
			gcContainer.Parent.ResumeLayout();
			CurrentModule.Invalidate();
			InitCurrentRibbon();
			Text = IsCurrentAbout ? DemoName : string.Format("{0} - {1}", DemoName, CurrentModule.TutorialName);
			MainFormHelper.SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
			whatsThisController.UpdateWhatsThisInfo(CurrentModule.TutorialInfo.WhatsThisXMLFile, CurrentModule.TutorialInfo.WhatsThisCodeFile, this.GetType());
			rpgCode.Visible = whatsThisController.IsWhatsThisInfoValid() || !string.IsNullOrEmpty(ProductName);
			bbiCode.Visibility = whatsThisController.IsWhatsThisInfoValid() ? BarItemVisibility.Always : BarItemVisibility.Never;
			bbiOpenSolution.Visibility = !string.IsNullOrEmpty(ProductName) && !(CurrentModule is CodeTutorialControlBase) ? BarItemVisibility.Always : BarItemVisibility.Never;
			UpdateAboutButtonInfo();
		}
		void UpdateAboutButtonInfo() {
			bbiAbout.Caption = DevExpress.Tutorials.Properties.Resources.AboutCaption;
		}
		protected virtual void InitCurrentRibbon() {
			rpgActiveDemo.Visible = CurrentModule.HasActiveDemo;
			rpgAppearance.Visible = CurrentModule.AllowAppearanceGroup;
			rpgExport.ShowCaptionButton = CurrentModule.AllowPrintOptions;
			UpdateNavigationButton();
		}
		void UpdateNavigationButton() {
			bbiUp.Enabled = bbiDown.Enabled = !IsCurrentAbout && accordionControl1.SelectedElement != null && AccordionNavigationMenuHelper.GetNodeCount(accordionControl1) > 1;
		}
		private void accordionControl1_SelectedLinkChanged(object sender, SelectedElementChangedEventArgs e) {
			if(e.Element == null) return;
			ShowModule(e.Element.Text);
			prevLink = e.Element;
		}
		private void accordionControl1_LinkClicked(object sender, ElementClickEventArgs e) {
			if(e.Element == accordionControl1.SelectedElement && IsCurrentAbout && !string.IsNullOrEmpty(prevModuleName))
				ShowModuleByName(prevModuleName);
		}
		void ShowModuleByName(string name) {
			ShowModule(name);
			if(prevLink != null) accordionControl1.SelectElement(prevLink);
		}
		bool IsCurrentAbout { 
			get {
				if(ModulesInfo.Instance.CurrentModuleBase == null || !IsAllowAboutModule) return false;
				return ModulesInfo.Instance.CurrentModuleBase.Name == ModulesInfo.GetItem(0).Name; 
			} 
		}
		protected virtual bool IsAllowAboutModule { get { return true; } }
		protected virtual int DefaultModuleIndex { get { return 1; } }
		private void frmMain_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyData == Keys.Escape && IsCurrentAbout && !string.IsNullOrEmpty(prevModuleName))
				ShowModuleByName(prevModuleName);
		}		
		public void ClearNavBarFilter() {
		}
		#region ITutorialForm Members
		bool ITutorialForm.IsFullMode {
			get { return !this.gcNavigations.Visible; }
		}
		bool ITutorialForm.AllowDemoFilter {
			get { return AllowNavBarFilter; }
		}
		bool ITutorialForm.IsDemoFilterVisible {
			get { return false; }
		}
		void ShowCaption() {
			if(CurrentModule != null) {
				pnlCaption.Visible = false;
			}
		}
		public void HideServiceElements() {
			gcDescription.Visible = horzSplitter.Visible = false;
			this.gcNavigations.Hide();
			this.pnlCaption.Hide();
			bciFilter.Enabled = false;
		}
		public void ShowServiceElements() {
			gcDescription.Visible = horzSplitter.Visible = ModulesInfo.Instance.CurrentModuleBase.Description != "";
			this.gcNavigations.Show();
			ShowCaption();
			bciFilter.Enabled = true;
		}
		public void ShowDemoFilter() { }
		#endregion
		#region Menu
		private void rpgAppearance_CaptionButtonClick(object sender, RibbonPageGroupEventArgs e) {
			pmAppearance.ShowPopup(MousePosition);
		}
		private void pmAppearance_Popup(object sender, EventArgs e) {
			bciAllowFormSkin.Checked = Skins.SkinManager.AllowFormSkins;
			bciAllowFormSkin.Enabled = DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveSkinName != "Office 2013";
		}
		private void bciAllowFormSkin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			this.AllowFormGlass = Skins.SkinManager.AllowFormSkins ? DefaultBoolean.True : DefaultBoolean.False;
			if(Skins.SkinManager.AllowFormSkins)
				Skins.SkinManager.DisableFormSkins();
			else
				Skins.SkinManager.EnableFormSkins();
		}
		private void bbiUp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			AccordionNavigationMenuHelper.ShowPrev(accordionControl1);
		}
		private void bbiDown_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			AccordionNavigationMenuHelper.ShowNext(accordionControl1);
		}
		private void bbiActiveDemo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			TutorialControlBase current = ModulesInfo.Instance.CurrentModuleBase.TModule as TutorialControlBase;
			if(current == null || !current.HasActiveDemo) return;
			current.RunActiveDemo();
		}
		private void bbiAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			ShowAbout();
		}
		internal void StartDemo() {
			AccordionNavigationMenuHelper.StartDemo(accordionControl1);
		}
		private void bciFullWindow_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			if(bciFullWindow.Checked) HideServiceElements();
			else ShowServiceElements();
		}
		private void bciFilter_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
		}
		#endregion
		#region Print and Export
		public RibbonPage MainPage { get { return rpMain; } }
		public RibbonPageGroup PrintExportGroup { get { return rpgExport; } }
		public RibbonPageGroup ReservGroup1 { get { return rpgReserv1; } }
		public RibbonPageGroup ReservGroup2 { get { return rpgReserv2; } }
		internal BarButtonItem PrintPreviewButton { get { return bbiPrintPreview; } }
		internal BarButtonItem PrintButton { get { return bbiPrint; } }
		internal BarButtonItem ExportButton { get { return bbiExport; } }
		internal BarButtonItem ExportToPDFButton { get { return bbiExporttoPDF; } }
		internal BarButtonItem ExportToEPUBButton { get { return bbiExporttoEPUB; } }
		internal BarButtonItem ExportToXMLButton { get { return bbiExporttoXML; } }
		internal BarButtonItem ExportToHTMLButton { get { return bbiExporttoHTML; } }
		internal BarButtonItem ExportToMHTButton { get { return bbiExporttoMHT; } }
		internal BarButtonItem ExportToDOCButton { get { return bbiExporttoDOC; } }
		internal BarButtonItem ExportToDOCXButton { get { return bbiExporttoDOCX; } }
		internal BarButtonItem ExportToXLSButton { get { return bbiExporttoXLS; } }
		internal BarButtonItem ExportToXLSXButton { get { return bbiExporttoXLSX; } }
		internal BarButtonItem ExportToRTFButton { get { return bbiExporttoRTF; } }
		internal BarButtonItem ExportToODTButton { get { return bbiExporttoODT; } }
		internal BarButtonItem ExportToImageButton { get { return bbiExporttoImage; } }
		protected internal BarSubItem ExportToImageExButton { get { return bsiExporttoImageEx; } }
		internal BarButtonItem ExportToTextButton { get { return bbiExporttoText; } }
		internal BarButtonItem ShowInVisualStudio { get { return bbiShowInVisualStudio; } }
		private void pmPrintOptions_BeforePopup(object sender, CancelEventArgs e) {
			bciShowRibbonPreview.Checked = RibbonMenuManager.PrintOptions.ShowRibbonPreviewForm;
		}
		private void bciShowRibbonPreview_ItemClick(object sender, ItemClickEventArgs e) {
			RibbonMenuManager.PrintOptions.ShowRibbonPreviewForm = bciShowRibbonPreview.Checked;
		}
		private void rpgExport_CaptionButtonClick(object sender, RibbonPageGroupEventArgs e) {
			pmPrintOptions.ShowPopup(MousePosition);
		}
		#endregion
		void ribbonControl1_Paint(object sender, PaintEventArgs e) {
			if (IsCurrentAbout)
				return;
			DevExpress.XtraBars.Ribbon.ViewInfo.RibbonViewInfo ribbonViewInfo = ribbonControl1.ViewInfo;
			if (ribbonViewInfo == null)
				return;
			DevExpress.XtraBars.Ribbon.ViewInfo.RibbonPanelViewInfo panelViewInfo = ribbonViewInfo.Panel;
			if (panelViewInfo == null)
				return;
			Rectangle bounds = panelViewInfo.Bounds;
			int minX = bounds.X;
			DevExpress.XtraBars.Ribbon.ViewInfo.RibbonPageGroupViewInfoCollection groups = panelViewInfo.Groups;
			if (groups == null)
				return;
			if (groups.Count > 0)
				minX = IsRibbonRTL ? groups[groups.Count - 1].Bounds.X : groups[groups.Count - 1].Bounds.Right;
			Image image = DevExpress.Utils.Frames.ApplicationCaption8_1.GetImageLogoEx(LookAndFeel);
			if (bounds.Height < image.Height)
				return;
			int sideIndent = 15;
			int offset = (bounds.Height - image.Height) / 2;
			bounds.X = IsRibbonRTL ? sideIndent : bounds.Width - image.Width - sideIndent;
			bounds.Width = image.Width;
			bounds.Y += offset;
			bounds.Height = image.Height;
			if(IsRibbonRTL ? bounds.Right > minX : bounds.X < minX)
				return;
			e.Graphics.DrawImage(image, bounds.Location);
		}
		protected bool IsRibbonRTL { get { return Ribbon.RightToLeft == RightToLeft.Yes; } }
		#region WhatThis Info
		bool WhatsThisEnabled { get { return !bbiCode.Down; } }
		WhatsThisController whatsThisController;
		ModuleWhatsThis whatsThisModule;
		FormTutorialInfo tutorialInfo = new FormTutorialInfo();
		private ImageShaderBase currentShader = new ImageShaderDisable();
		private void bbiCode_DownChanged(object sender, ItemClickEventArgs e) {
			if(WhatsThisEnabled)
				DisableWhatsThis();
			else {
				EnableWhatsThis();
			}
		}
		protected virtual void NotifyModuleWhatsThisStateChange(bool whatsThisStarted) {
			if(CurrentModule == null) return;
			if(whatsThisStarted)
				CurrentModule.StartWhatsThis();
			else
				CurrentModule.EndWhatsThis();
			accordionControl1.Enabled = !whatsThisStarted;
			foreach(RibbonPage page in ribbonControl1.Pages)
				foreach(RibbonPageGroup group in page.Groups) {
					if(group != rpgCode)
						group.Enabled = !whatsThisStarted;
				}
			bbiOpenSolution.Enabled = !whatsThisStarted;
		}
		protected void EnableWhatsThis() {
			if(WhatsThisEnabled) return;
			NotifyModuleWhatsThisStateChange(true);
			whatsThisController.UpdateRegisteredVisibleControls();
			ShowWhatsThisModule();
		}
		void CloseWhatsThis() {
			bbiCode.Down = false;
		}
		protected void DisableWhatsThis() {
			if(!WhatsThisEnabled) return;
			NotifyModuleWhatsThisStateChange(false);
			HideWhatsThisModule();
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
		private void InitWhatsThisModule() {
			whatsThisModule = new ModuleWhatsThis(whatsThisController);
			whatsThisModule.Parent = gcContainer;
			whatsThisModule.Dock = DockStyle.Fill;
			whatsThisModule.Visible = false;
		}
		private void RibbonMainForm_Resize(object sender, EventArgs e) {
			CloseWhatsThis();
		}
		private void RibbonMainForm_Move(object sender, EventArgs e) {
			CloseWhatsThis();
		}
		#endregion
		#region IWhatsThisProvider Members
		public ImageShaderBase CurrentShader { get { return currentShader; }}
		public FormTutorialInfo TutorialInfo { get { return tutorialInfo; } }
		UserControl IWhatsThisProvider.CurrentModule { get { return CurrentModule; } }
		[DefaultValue(true)]
		public bool HintVisible {
			get {
				return true;
			}
			set {
			}
		}
		#endregion
		protected virtual bool AllowMergeStatusBar { get { return false; } }
		public void MergeRibbon(RibbonControl childRibbon) {
			SuspendLayout();
			RibbonControl.MergeRibbon(childRibbon);
			if(demoCategory.Pages.Count == 0) demoCategory.Pages.Add(rpMain);
			if (RibbonControl.MergedPages.Count > 0) RibbonControl.SelectedPage = childRibbon.SelectedPage;
			ResumeLayout();
		}
		public void UnMergeRibbon() {
			RibbonControl.UnMergeRibbon();
			RibbonControl.Pages.Add(rpMain);
		}
		void ribbonControl1_UnMerge(object sender, RibbonMergeEventArgs e) {
			if(!AllowMergeStatusBar) return;
			RibbonControl ribbon = (RibbonControl)sender;
			if(e.MergedChild.StatusBar != null) {
				if(ribbon.StatusBar != null) {
					ribbon.StatusBar.UnMergeStatusBar();
				}
			}
		}
		void ribbonControl1_Merge(object sender, RibbonMergeEventArgs e) {
			if(!AllowMergeStatusBar) return;
			RibbonControl ribbon = (RibbonControl)sender;
			if(e.MergedChild.StatusBar != null) {
				if(ribbon.StatusBar != null) {
					ribbon.StatusBar.ItemLinks.Clear();
					ribbon.StatusBar.MergeStatusBar(e.MergedChild.StatusBar);
				}
			}
		}
		private void bbiGettingStarted_ItemClick(object sender, ItemClickEventArgs e) {
			OverviewButton.ProcessStart(RibbonMainForm.GetStartedLink);
		}
		private void bbiGetFreeSupport_ItemClick(object sender, ItemClickEventArgs e) {
			OverviewButton.ProcessStart(AssemblyInfo.DXLinkGetSupport);
		}
		private void bbiBuyNow_ItemClick(object sender, ItemClickEventArgs e) {
			OverviewButton.ProcessStart(AssemblyInfo.DXLinkBuyNow);
		}
		static string getStartedLink = AssemblyInfo.DXLinkGetStarted;
		protected internal static string GetStartedLink {
			get { return getStartedLink; }
			set {
				if(value == null) return;
				getStartedLink = value;
			}
		}
		private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e) {
			ColorWheelForm form = new ColorWheelForm();
			form.StartPosition = FormStartPosition.CenterParent;
			form.SkinMaskColor = UserLookAndFeel.Default.SkinMaskColor;
			form.SkinMaskColor2 = UserLookAndFeel.Default.SkinMaskColor2;
			form.ShowDialog(this);
		}
		private void accordionControl1_CustomElementText(object sender, XtraBars.Navigation.CustomElementTextEventArgs e) {
			if(e.Text.Contains(" (new)")) {
				e.Text = e.Text.Replace(" (new)", "");
				e.ObjectInfo.Element.TagInternal = "new";
			}
			if(e.Text.Contains(" (updated)")) {
				e.Text = e.Text.Replace(" (updated)", "");
				e.ObjectInfo.Element.TagInternal = "updated";
			}
		}
		private void accordionControl1_CustomDrawElement(object sender, XtraBars.Navigation.CustomDrawElementEventArgs e) {
			if(e.ObjectInfo.HeaderBounds == Rectangle.Empty) return;
			e.DrawHeaderBackground();
			if(e.ObjectInfo.ControlInfo.AccordionControl.ShowGroupExpandButtons)
				e.DrawExpandCollapseButton();
			e.DrawImage();
			e.DrawContextButtons();
			DrawHighlight(e);
			e.DrawText();
			e.DrawElementSelection();
			e.Handled = true;
		}
		protected void DrawHighlight(CustomDrawElementEventArgs e) {
			if(e.ObjectInfo is AccordionGroupViewInfo) DrawGroupHighlight(e);
			if(e.ObjectInfo is AccordionItemViewInfo) DrawItemHighlight(e);
		}
		protected void DrawItemHighlight(CustomDrawElementEventArgs e) {
			if(e.ObjectInfo.Element.OwnerElement != null && e.ObjectInfo.Element.OwnerElement.Text.Contains("New")) return;
			if(e.ObjectInfo.Element == accordionControl1.SelectedElement || e.ObjectInfo.ControlInfo.PressedInfo.ItemInfo == e.ObjectInfo) return;
			if(e.ObjectInfo.Element.TagInternal != null && e.ObjectInfo.Element.TagInternal.ToString() == "new") {
				Rectangle textBounds = e.ObjectInfo.TextBounds;
				Rectangle highlightRect = new Rectangle(textBounds.X - 3, textBounds.Y - 3, textBounds.Width + 6, textBounds.Height + 6);
				e.Cache.FillRectangle(NewItemColor, highlightRect);
			}
			if(e.ObjectInfo.Element.TagInternal != null && e.ObjectInfo.Element.TagInternal.ToString() == "updated") {
				Rectangle textBounds = e.ObjectInfo.TextBounds;
				Rectangle highlightRect = new Rectangle(textBounds.X - 3, textBounds.Y - 3, textBounds.Width + 6, textBounds.Height + 6);
				e.Cache.FillRectangle(UpdatedItemColor, highlightRect);
			}
		}
		protected void DrawGroupHighlight(CustomDrawElementEventArgs e) {
			if(e.ObjectInfo.Text.Contains("New")) {
				Rectangle highlightRect = new Rectangle(e.ObjectInfo.HeaderBounds.X + 2, e.ObjectInfo.HeaderBounds.Y + 2, e.ObjectInfo.HeaderBounds.Width -4 , e.ObjectInfo.HeaderBounds.Height - 4);
				e.Cache.FillRectangle(NewGroupColor, highlightRect);
				return;
			}
			if(e.ObjectInfo.Text.Contains("Highlighted")) {
				Rectangle highlightRect = new Rectangle(e.ObjectInfo.HeaderBounds.X + 2, e.ObjectInfo.HeaderBounds.Y + 2, e.ObjectInfo.HeaderBounds.Width - 4, e.ObjectInfo.HeaderBounds.Height - 4);
				e.Cache.FillRectangle(HighlightedGroupColor, highlightRect);
			}
		}
		Color NewGroupColor { get { return Color.FromArgb(64, 78, 201, 85); } }
		Color HighlightedGroupColor { get { return Color.FromArgb(64, 255, 97, 97); } }
		Color NewItemColor { get { return Color.FromArgb(127, 255, 193, 24); } }
		Color UpdatedItemColor { get { return Color.FromArgb(127, 127, 221, 134); } }
		static string HtmlQuestionColor {
			get {
				Color color = CommonColors.GetQuestionColor(DevExpress.LookAndFeel.UserLookAndFeel.Default);
				return GetRGBColor(color);
			}
		}
		static string HtmlInformationColor {
			get {
				Color color = CommonColors.GetInformationColor(DevExpress.LookAndFeel.UserLookAndFeel.Default);
				return GetRGBColor(color);
			}
		}
		static string HtmlCriticalColor {
			get {
				Color color = CommonColors.GetCriticalColor(DevExpress.LookAndFeel.UserLookAndFeel.Default);
				return GetRGBColor(color);
			}
		}
		static string GetRGBColor(Color color) {
			return string.Format("{0},{1},{2}", color.R, color.G, color.B);
		}
		protected virtual Product GetProduct() { return Repository.WinPlatform.Products.First(p => p.Name == ProductName); }
		protected virtual void OpenSolution() {
			if(string.IsNullOrEmpty(ProductName)) return;
			Product product = GetProduct();
			Demo demo = product.Demos.FirstOrDefault(x => x.Modules.Count > 0);
			SimpleModule module = (SimpleModule)demo.Modules.First(p => p.Type == CurrentModule.FullTypeName);
			if(product.Name == "XtraReportsForWin" && module.Demo.Name == "MainDemo") {
				string csPath = @"\Reporting\CS\DevExpress.DemoReports\DevExpress.DemoReports.sln";
				string vbPath = @"\Reporting\VB\DevExpress.DemoReports\DevExpress.DemoReports.sln";
				module = module.WithDemo(((SimpleDemo)module.Demo).WithSolutionPaths(csPath, vbPath));
			}
			if(ExampleLanguage.Csharp.Equals(bbiOpenSolution.Tag))
				DemoRunner.OpenCSSolution(module, CallingSite.WinDemoChooser);
			else if(ExampleLanguage.VB.Equals(bbiOpenSolution.Tag))
				DemoRunner.OpenVBSolution(module, CallingSite.WinDemoChooser);
		}
		void InitOpenSolution(BarButtonItem item, bool forceOpen) {
			bbiOpenSolution.Glyph = item.Glyph;
			bbiOpenSolution.LargeGlyph = item.LargeGlyph;
			bbiOpenSolution.Tag = item == bbiCSSolution ? ExampleLanguage.Csharp : ExampleLanguage.VB;
			if(forceOpen) OpenSolution();
		}
		private void bbiOpenSolution_ItemClick(object sender, ItemClickEventArgs e) {
			OpenSolution();
		}
		private void bbiCSSolution_ItemClick(object sender, ItemClickEventArgs e) {
			InitOpenSolution(bbiCSSolution, true);
		}
		private void bbiVBSolution_ItemClick(object sender, ItemClickEventArgs e) {
			InitOpenSolution(bbiVBSolution, true);
		}
		protected new virtual string ProductName { get { return string.Empty; } }
	}
	public class DescriptionLabel : LabelControl {
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(AutoSizeMode == LabelAutoSizeMode.Vertical && (Dock == DockStyle.Right || Dock == DockStyle.Bottom)) {
				Size sz = GetPreferredSize(new Size(width, height));
				if(Dock == DockStyle.Right)
					x = (x + width) - sz.Width;
				if(Dock == DockStyle.Bottom)
					y = (y + height) - sz.Height;
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}
	}
	public class SearchControlLogger : ISearchControlClient {
		ISearchControlClient source;
		object owner;
		public SearchControlLogger(object owner, ISearchControlClient source) {
			this.source = source;
			this.owner = owner;
		}
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase searchInfo) {
			Log(searchInfo);
			source.ApplyFindFilter(searchInfo);
		}
		void Log(SearchInfoBase searchInfo) {
			if(string.IsNullOrWhiteSpace(searchInfo.SearchText)) return;
			Log(searchInfo.SearchText);
		}
		Timer timer;
		string timerText;
		void Log(string text) {
			if(timer != null) {
				timer.Stop();
				timer.Dispose();
				timer = null;
				if(text.ToLower().StartsWith(timerText.ToLower())) {
					StartTimer(text);
					return;
				}
				LogCore(timerText);
			}
			StartTimer(text);
		}
		void LogCore(string text) {
		}
		void StartTimer(string text) {
			this.timer = new Timer() { Interval = 2000 };
			this.timerText = text;
			this.timer.Tick += (s, e) => {
				if(timer != null) {
					timer.Dispose();
					this.timer = null;
					LogCore(text);
				}
			};
			this.timer.Start();
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() {
			return source.CreateSearchProvider();
		}
		bool ISearchControlClient.IsAttachedToSearchControl { get { return source.IsAttachedToSearchControl;  } }
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) {
			source.SetSearchControl(searchControl);
		}
	}
	public class TouchNavBarControl : NavBarControl {
		protected override XtraNavBar.ViewInfo.NavBarViewInfo CreateViewInfo() {
			var res = base.CreateViewInfo();
			res.ScrollBar.TouchMode = true;
			return res;
		}
	}
}
