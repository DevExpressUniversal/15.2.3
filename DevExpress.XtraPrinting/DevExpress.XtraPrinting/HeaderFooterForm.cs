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
using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraBars;
using DevExpress.XtraLayout;
namespace DevExpress.XtraPrinting.Native.WinControls {
	public abstract class HeaderFooterFormBase : XtraForm {
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.FontDialog fontDialogHead;
		private System.Windows.Forms.FontDialog fontDialogFoot;
		private MemoEdit focusedMemoEdit;
		private MemoEdit[] headControls;
		private MemoEdit[] footControls;
		private DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
		private DevExpress.XtraBars.BarDockControl barDockControl1;
		private DevExpress.XtraBars.BarCheckItem checkItem1;
		private DevExpress.XtraBars.BarCheckItem checkItem2;
		private DevExpress.XtraBars.BarCheckItem checkItem3;
		private DevExpress.XtraPrinting.BrickAlignment headAligment;
		private DevExpress.XtraPrinting.BrickAlignment footAligment;
		private DevExpress.XtraBars.BarManager barManager;
		private DevExpress.XtraBars.Bar bar1;
		private DevExpress.XtraBars.BarButtonItem barButtonItem1;
		private DevExpress.XtraBars.BarButtonItem barButtonItem6;
		private DevExpress.XtraBars.BarButtonItem barButtonItem2;
		private DevExpress.XtraBars.BarButtonItem barButtonItem3;
		private DevExpress.XtraBars.BarButtonItem barButtonItem4;
		private DevExpress.XtraBars.BarButtonItem barButtonItem5;
		protected DevExpress.XtraBars.BarSubItem imageMenuItem;
		private DevExpress.Utils.ImageCollection imageCollection1;
		private BarEditItem beiFont;
		private DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit repositoryItemButtonEdit1;
		private BarStaticItem barStaticItem1;
		private DevExpress.XtraEditors.Repository.RepositoryItemFontEdit repositoryItemFontEdit1;
		private IList images;
		private BarDockControl barDockControlBottom;
		private BarDockControl barDockControlLeft;
		private BarDockControl barDockControlRight;
		protected LayoutControl layoutControl1;
		private MemoEdit tbFoot1;
		protected LayoutControlGroup layoutControlGroup1;
		private LayoutControlItem layoutControlItem1;
		private LabelControl horizontalLine;
		private MemoEdit tbHead3;
		private MemoEdit tbHead2;
		private MemoEdit tbHead1;
		protected SimpleButton buttonCancel;
		protected SimpleButton buttonOK;
		private MemoEdit tbFoot3;
		private MemoEdit tbFoot2;
		private LayoutControlItem layoutControlItem2;
		private LayoutControlItem layoutControlItem3;
		private LayoutControlItem layoutControlItem4;
		private LayoutControlItem layoutControlItem5;
		private LayoutControlItem layoutControlItem6;
		private LayoutControlItem layoutControlItem7;
		private LayoutControlItem layoutControlItem8;
		private CheckEdit headerCheckEdit;
		private CheckEdit footerCheckEdit;
		private LayoutControlGroup layoutControlGroupFooter;
		private LayoutControlItem layoutControlItem10;
		protected LayoutControlGroup grpButtons;
		private LayoutControlGroup layoutControlGroupHeader;
		private LayoutControlItem layoutControlItem11;
		private LayoutControlItem layoutControlItem9;
		public IList Images {
			get { return images; }
			set {
				if(value.Count > 0) {
					images = value;
					FillImageListMenu();
					imageMenuItem.Enabled = true;
				} else {
					images = null;
					imageMenuItem.Enabled = false;
				}
			}
		}
		public HeaderFooterFormBase() {
			InitializeComponent();
			if(!IsFontBarItemVisible) {
				this.beiFont.Visibility = BarItemVisibility.Never;
				this.barStaticItem1.Visibility = BarItemVisibility.Never;
			}
			if(!IsImageBarItemVisible) {
				this.imageMenuItem.Visibility = BarItemVisibility.Never;
			}
			barAndDockingController1.LookAndFeel.ParentLookAndFeel = this.LookAndFeel;
			ImageCollectionHelper.FillImageCollectionFromResources(imageCollection1, "DevExpress.XtraPrinting.Images.HeaderFooterForm.png", System.Reflection.Assembly.GetExecutingAssembly());
			MemoEdit[] controls = new MemoEdit[] { tbHead1, tbHead2, tbHead3, tbFoot1, tbFoot2, tbFoot3 };
			headControls = new MemoEdit[] { tbHead1, tbHead2, tbHead3 };
			footControls = new MemoEdit[] { tbFoot1, tbFoot2, tbFoot3 };
			foreach(MemoEdit control in controls) {
				control.LostFocus += new EventHandler(control_LostFocus);
				control.GotFocus += new EventHandler(control_GotFocus);
			}
			barButtonItem1.Tag = PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageNumber);
			barButtonItem6.Tag = PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageTotal);
			barButtonItem2.Tag = PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageNumberOfTotal);
			barButtonItem3.Tag = PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageDate);
			barButtonItem4.Tag = PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageTime);
			barButtonItem5.Tag = PreviewLocalizer.GetString(PreviewStringId.PageInfo_PageUserName);
			barManager.AllowShowToolbarsPopup = false;
			LookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelStyleChanged);
			UpdateColors();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
				if(LookAndFeel != null) {
					LookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelStyleChanged);
				}
			}
			base.Dispose(disposing);
		}
		protected DevExpress.XtraPrinting.BrickAlignment HeadAligment { get { return headAligment; } set { headAligment = value; } }
		protected DevExpress.XtraPrinting.BrickAlignment FootAligment { get { return footAligment; } set { footAligment = value; } }
		protected Font HeadFont { get { return fontDialogHead.Font; } }
		protected Font FootFont { get { return fontDialogFoot.Font; } }
		protected MemoEdit[] HeadControls { get { return headControls; } }
		protected MemoEdit[] FootControls { get { return footControls; } }
		protected virtual bool IsFontBarItemVisible { get { return true; } }
		protected virtual bool IsImageBarItemVisible { get { return true; } }
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HeaderFooterFormBase));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition16 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition17 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition18 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition11 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition12 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition13 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition14 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition9 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition10 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition11 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition12 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition13 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition14 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition15 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition7 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition8 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition9 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition10 = new DevExpress.XtraLayout.RowDefinition();
			this.fontDialogHead = new System.Windows.Forms.FontDialog();
			this.fontDialogFoot = new System.Windows.Forms.FontDialog();
			this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
			this.checkItem1 = new DevExpress.XtraBars.BarCheckItem();
			this.checkItem2 = new DevExpress.XtraBars.BarCheckItem();
			this.checkItem3 = new DevExpress.XtraBars.BarCheckItem();
			this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.bar1 = new DevExpress.XtraBars.Bar();
			this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonItem6 = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
			this.barButtonItem5 = new DevExpress.XtraBars.BarButtonItem();
			this.imageMenuItem = new DevExpress.XtraBars.BarSubItem();
			this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
			this.beiFont = new DevExpress.XtraBars.BarEditItem();
			this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.repositoryItemFontEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.headerCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.footerCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.horizontalLine = new DevExpress.XtraEditors.LabelControl();
			this.tbHead3 = new DevExpress.XtraEditors.MemoEdit();
			this.tbHead2 = new DevExpress.XtraEditors.MemoEdit();
			this.tbHead1 = new DevExpress.XtraEditors.MemoEdit();
			this.buttonCancel = new DevExpress.XtraEditors.SimpleButton();
			this.buttonOK = new DevExpress.XtraEditors.SimpleButton();
			this.tbFoot3 = new DevExpress.XtraEditors.MemoEdit();
			this.tbFoot2 = new DevExpress.XtraEditors.MemoEdit();
			this.tbFoot1 = new DevExpress.XtraEditors.MemoEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroupFooter = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroupHeader = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.headerCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.footerCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbHead3.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbHead2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbHead1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbFoot3.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbFoot2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbFoot1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFooter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupHeader)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			this.SuspendLayout();
			this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
			this.barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
			this.checkItem1.BindableChecked = true;
			this.checkItem1.Checked = true;
			this.checkItem1.GroupIndex = 1;
			resources.ApplyResources(this.checkItem1, "checkItem1");
			this.checkItem1.Id = 0;
			this.checkItem1.ImageIndex = 0;
			this.checkItem1.Name = "checkItem1";
			this.checkItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.checkItem_ItemClick);
			this.checkItem2.GroupIndex = 1;
			resources.ApplyResources(this.checkItem2, "checkItem2");
			this.checkItem2.Id = 2;
			this.checkItem2.ImageIndex = 1;
			this.checkItem2.Name = "checkItem2";
			this.checkItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.checkItem_ItemClick);
			this.checkItem3.GroupIndex = 1;
			resources.ApplyResources(this.checkItem3, "checkItem3");
			this.checkItem3.Id = 3;
			this.checkItem3.ImageIndex = 2;
			this.checkItem3.Name = "checkItem3";
			this.checkItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.checkItem_ItemClick);
			this.barDockControl1.CausesValidation = false;
			resources.ApplyResources(this.barDockControl1, "barDockControl1");
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.bar1});
			this.barManager.Controller = this.barAndDockingController1;
			this.barManager.DockControls.Add(this.barDockControl1);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.Images = this.imageCollection1;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.barButtonItem1,
			this.barButtonItem6,
			this.barButtonItem2,
			this.barButtonItem3,
			this.barButtonItem4,
			this.barButtonItem5,
			this.imageMenuItem,
			this.checkItem1,
			this.checkItem2,
			this.checkItem3,
			this.beiFont,
			this.barStaticItem1});
			this.barManager.MaxItemId = 15;
			this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemButtonEdit1,
			this.repositoryItemFontEdit1});
			this.bar1.BarName = "Custom 1";
			this.bar1.DockCol = 0;
			this.bar1.DockRow = 0;
			this.bar1.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.bar1.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem6),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem3),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem4),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem5),
			new DevExpress.XtraBars.LinkPersistInfo(this.imageMenuItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.checkItem1, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.checkItem2),
			new DevExpress.XtraBars.LinkPersistInfo(this.checkItem3),
			new DevExpress.XtraBars.LinkPersistInfo(this.barStaticItem1, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.beiFont)});
			this.bar1.OptionsBar.AllowQuickCustomization = false;
			this.bar1.OptionsBar.DrawDragBorder = false;
			this.bar1.OptionsBar.UseWholeRow = true;
			resources.ApplyResources(this.bar1, "bar1");
			resources.ApplyResources(this.barButtonItem1, "barButtonItem1");
			this.barButtonItem1.Id = 0;
			this.barButtonItem1.ImageIndex = 3;
			this.barButtonItem1.Name = "barButtonItem1";
			this.barButtonItem1.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.toolBar_ItemClick);
			resources.ApplyResources(this.barButtonItem6, "barButtonItem6");
			this.barButtonItem6.Id = 5;
			this.barButtonItem6.ImageIndex = 9;
			this.barButtonItem6.Name = "barButtonItem6";
			this.barButtonItem6.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.toolBar_ItemClick);
			resources.ApplyResources(this.barButtonItem2, "barButtonItem2");
			this.barButtonItem2.Id = 1;
			this.barButtonItem2.ImageIndex = 4;
			this.barButtonItem2.Name = "barButtonItem2";
			this.barButtonItem2.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.toolBar_ItemClick);
			resources.ApplyResources(this.barButtonItem3, "barButtonItem3");
			this.barButtonItem3.Id = 2;
			this.barButtonItem3.ImageIndex = 5;
			this.barButtonItem3.Name = "barButtonItem3";
			this.barButtonItem3.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.toolBar_ItemClick);
			resources.ApplyResources(this.barButtonItem4, "barButtonItem4");
			this.barButtonItem4.Id = 3;
			this.barButtonItem4.ImageIndex = 6;
			this.barButtonItem4.Name = "barButtonItem4";
			this.barButtonItem4.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.toolBar_ItemClick);
			resources.ApplyResources(this.barButtonItem5, "barButtonItem5");
			this.barButtonItem5.Id = 4;
			this.barButtonItem5.ImageIndex = 7;
			this.barButtonItem5.Name = "barButtonItem5";
			this.barButtonItem5.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.toolBar_ItemClick);
			resources.ApplyResources(this.imageMenuItem, "imageMenuItem");
			this.imageMenuItem.Id = 5;
			this.imageMenuItem.ImageIndex = 8;
			this.imageMenuItem.Name = "imageMenuItem";
			this.imageMenuItem.PaintStyle = DevExpress.XtraBars.BarItemPaintStyle.CaptionGlyph;
			this.barStaticItem1.Border = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.barStaticItem1, "barStaticItem1");
			this.barStaticItem1.Id = 14;
			this.barStaticItem1.Name = "barStaticItem1";
			this.barStaticItem1.TextAlignment = System.Drawing.StringAlignment.Near;
			this.beiFont.AutoFillWidth = true;
			this.beiFont.Edit = this.repositoryItemButtonEdit1;
			this.beiFont.Id = 12;
			this.beiFont.Name = "beiFont";
			resources.ApplyResources(this.beiFont, "beiFont");
			this.beiFont.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.beiFont_ItemClick);
			resources.ApplyResources(this.repositoryItemButtonEdit1, "repositoryItemButtonEdit1");
			this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			resources.ApplyResources(this.repositoryItemFontEdit1, "repositoryItemFontEdit1");
			this.repositoryItemFontEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemFontEdit1.Buttons"))))});
			this.repositoryItemFontEdit1.Name = "repositoryItemFontEdit1";
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.headerCheckEdit);
			this.layoutControl1.Controls.Add(this.footerCheckEdit);
			this.layoutControl1.Controls.Add(this.horizontalLine);
			this.layoutControl1.Controls.Add(this.tbHead3);
			this.layoutControl1.Controls.Add(this.tbHead2);
			this.layoutControl1.Controls.Add(this.tbHead1);
			this.layoutControl1.Controls.Add(this.buttonCancel);
			this.layoutControl1.Controls.Add(this.buttonOK);
			this.layoutControl1.Controls.Add(this.tbFoot3);
			this.layoutControl1.Controls.Add(this.tbFoot2);
			this.layoutControl1.Controls.Add(this.tbFoot1);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(652, 122, 854, 590);
			this.layoutControl1.Root = this.layoutControlGroup1;
			resources.ApplyResources(this.headerCheckEdit, "headerCheckEdit");
			this.headerCheckEdit.Name = "headerCheckEdit";
			this.headerCheckEdit.Properties.Caption = resources.GetString("headerCheckEdit.Properties.Caption");
			this.headerCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.headerCheckEdit.Properties.RadioGroupIndex = 0;
			this.headerCheckEdit.StyleController = this.layoutControl1;
			this.headerCheckEdit.CheckedChanged += new System.EventHandler(this.headerCheckEdit_CheckedChanged);
			resources.ApplyResources(this.footerCheckEdit, "footerCheckEdit");
			this.footerCheckEdit.Name = "footerCheckEdit";
			this.footerCheckEdit.Properties.Caption = resources.GetString("footerCheckEdit.Properties.Caption");
			this.footerCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.footerCheckEdit.Properties.RadioGroupIndex = 0;
			this.footerCheckEdit.StyleController = this.layoutControl1;
			this.footerCheckEdit.TabStop = false;
			resources.ApplyResources(this.horizontalLine, "horizontalLine");
			this.horizontalLine.LineVisible = true;
			this.horizontalLine.Name = "horizontalLine";
			this.horizontalLine.StyleController = this.layoutControl1;
			resources.ApplyResources(this.tbHead3, "tbHead3");
			this.tbHead3.Name = "tbHead3";
			this.tbHead3.Properties.Appearance.Options.UseTextOptions = true;
			this.tbHead3.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.tbHead3.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.tbHead3.StyleController = this.layoutControl1;
			resources.ApplyResources(this.tbHead2, "tbHead2");
			this.tbHead2.Name = "tbHead2";
			this.tbHead2.Properties.Appearance.Options.UseTextOptions = true;
			this.tbHead2.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tbHead2.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.tbHead2.StyleController = this.layoutControl1;
			resources.ApplyResources(this.tbHead1, "tbHead1");
			this.tbHead1.Name = "tbHead1";
			this.tbHead1.Properties.Appearance.Options.UseTextOptions = true;
			this.tbHead1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.tbHead1.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.tbHead1.StyleController = this.layoutControl1;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.buttonCancel, "buttonCancel");
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.StyleController = this.layoutControl1;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.buttonOK, "buttonOK");
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.StyleController = this.layoutControl1;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			resources.ApplyResources(this.tbFoot3, "tbFoot3");
			this.tbFoot3.Name = "tbFoot3";
			this.tbFoot3.Properties.Appearance.Options.UseTextOptions = true;
			this.tbFoot3.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.tbFoot3.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.tbFoot3.StyleController = this.layoutControl1;
			resources.ApplyResources(this.tbFoot2, "tbFoot2");
			this.tbFoot2.Name = "tbFoot2";
			this.tbFoot2.Properties.Appearance.Options.UseTextOptions = true;
			this.tbFoot2.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.tbFoot2.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.tbFoot2.StyleController = this.layoutControl1;
			resources.ApplyResources(this.tbFoot1, "tbFoot1");
			this.tbFoot1.Name = "tbFoot1";
			this.tbFoot1.Properties.Appearance.Options.UseTextOptions = true;
			this.tbFoot1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.tbFoot1.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.tbFoot1.StyleController = this.layoutControl1;
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroupFooter,
			this.grpButtons,
			this.layoutControlGroupHeader});
			this.layoutControlGroup1.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			columnDefinition16.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition16.Width = 15D;
			columnDefinition17.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition17.Width = 100D;
			columnDefinition18.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition18.Width = 15D;
			this.layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition16,
			columnDefinition17,
			columnDefinition18});
			rowDefinition11.Height = 50D;
			rowDefinition11.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition12.Height = 50D;
			rowDefinition12.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition13.Height = 30D;
			rowDefinition13.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition14.Height = 43D;
			rowDefinition14.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup1.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition11,
			rowDefinition12,
			rowDefinition13,
			rowDefinition14});
			this.layoutControlGroup1.Size = new System.Drawing.Size(492, 283);
			this.layoutControlGroupFooter.GroupBordersVisible = false;
			this.layoutControlGroupFooter.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem2,
			this.layoutControlItem10,
			this.layoutControlItem3});
			this.layoutControlGroupFooter.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroupFooter.Location = new System.Drawing.Point(15, 95);
			this.layoutControlGroupFooter.Name = "layoutControlGroupFooter";
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition1.Width = 33.333333333333336D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 5D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition3.Width = 33.333333333333336D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition4.Width = 5D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition5.Width = 33.333333333333336D;
			this.layoutControlGroupFooter.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4,
			columnDefinition5});
			rowDefinition1.Height = 10D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition2.Height = 23D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition3.Height = 100D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition4.Height = 10D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.Absolute;
			this.layoutControlGroupFooter.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1,
			rowDefinition2,
			rowDefinition3,
			rowDefinition4});
			this.layoutControlGroupFooter.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlGroupFooter.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlGroupFooter.Size = new System.Drawing.Size(442, 95);
			this.layoutControlItem1.Control = this.tbFoot1;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 33);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem1.Size = new System.Drawing.Size(144, 52);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem2.Control = this.tbFoot2;
			this.layoutControlItem2.Location = new System.Drawing.Point(149, 33);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem2.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem2.Size = new System.Drawing.Size(144, 52);
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem10.Control = this.footerCheckEdit;
			this.layoutControlItem10.Location = new System.Drawing.Point(0, 10);
			this.layoutControlItem10.Name = "layoutControlItem10";
			this.layoutControlItem10.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem10.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem10.Size = new System.Drawing.Size(442, 23);
			this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem10.TextVisible = false;
			this.layoutControlItem3.Control = this.tbFoot3;
			this.layoutControlItem3.Location = new System.Drawing.Point(298, 33);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem3.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem3.Size = new System.Drawing.Size(144, 52);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem9});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 220);
			this.grpButtons.Name = "grpButtons";
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition6.Width = 100D;
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition7.Width = 80D;
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition8.Width = 2D;
			columnDefinition9.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition9.Width = 80D;
			columnDefinition10.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition10.Width = 5D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition6,
			columnDefinition7,
			columnDefinition8,
			columnDefinition9,
			columnDefinition10});
			rowDefinition5.Height = 17D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition6.Height = 26D;
			rowDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition5,
			rowDefinition6});
			this.grpButtons.OptionsTableLayoutItem.ColumnSpan = 3;
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 3;
			this.grpButtons.Size = new System.Drawing.Size(472, 43);
			this.layoutControlItem4.Control = this.buttonCancel;
			this.layoutControlItem4.Location = new System.Drawing.Point(387, 17);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 3;
			this.layoutControlItem4.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem4.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem5.Control = this.buttonOK;
			this.layoutControlItem5.Location = new System.Drawing.Point(305, 17);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlItem5.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem5.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem9.Control = this.horizontalLine;
			this.layoutControlItem9.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem9.Name = "layoutControlItem9";
			this.layoutControlItem9.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem9.Size = new System.Drawing.Size(472, 17);
			this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.TextVisible = false;
			this.layoutControlGroupHeader.GroupBordersVisible = false;
			this.layoutControlGroupHeader.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem11,
			this.layoutControlItem6,
			this.layoutControlItem7,
			this.layoutControlItem8});
			this.layoutControlGroupHeader.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroupHeader.Location = new System.Drawing.Point(15, 0);
			this.layoutControlGroupHeader.Name = "layoutControlGroupHeader";
			columnDefinition11.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition11.Width = 33.333333333333336D;
			columnDefinition12.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition12.Width = 5D;
			columnDefinition13.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition13.Width = 33.333333333333336D;
			columnDefinition14.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition14.Width = 5D;
			columnDefinition15.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition15.Width = 33.333333333333336D;
			this.layoutControlGroupHeader.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition11,
			columnDefinition12,
			columnDefinition13,
			columnDefinition14,
			columnDefinition15});
			rowDefinition7.Height = 10D;
			rowDefinition7.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition8.Height = 23D;
			rowDefinition8.SizeType = System.Windows.Forms.SizeType.AutoSize;
			rowDefinition9.Height = 100D;
			rowDefinition9.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition10.Height = 10D;
			rowDefinition10.SizeType = System.Windows.Forms.SizeType.Absolute;
			this.layoutControlGroupHeader.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition7,
			rowDefinition8,
			rowDefinition9,
			rowDefinition10});
			this.layoutControlGroupHeader.OptionsTableLayoutItem.ColumnIndex = 1;
			this.layoutControlGroupHeader.Size = new System.Drawing.Size(442, 95);
			this.layoutControlItem11.Control = this.headerCheckEdit;
			this.layoutControlItem11.Location = new System.Drawing.Point(0, 10);
			this.layoutControlItem11.Name = "layoutControlItem11";
			this.layoutControlItem11.OptionsTableLayoutItem.ColumnSpan = 5;
			this.layoutControlItem11.OptionsTableLayoutItem.RowIndex = 1;
			this.layoutControlItem11.Size = new System.Drawing.Size(442, 23);
			this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem11.TextVisible = false;
			this.layoutControlItem6.Control = this.tbHead1;
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 33);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem6.Size = new System.Drawing.Size(144, 52);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem7.Control = this.tbHead2;
			this.layoutControlItem7.Location = new System.Drawing.Point(149, 33);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem7.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem7.Size = new System.Drawing.Size(144, 52);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.layoutControlItem8.Control = this.tbHead3;
			this.layoutControlItem8.Location = new System.Drawing.Point(298, 33);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItem8.OptionsTableLayoutItem.RowIndex = 2;
			this.layoutControlItem8.Size = new System.Drawing.Size(144, 52);
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.AcceptButton = this.buttonOK;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.buttonCancel;
			this.Controls.Add(this.layoutControl1);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "HeaderFooterFormBase";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemButtonEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemFontEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.headerCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.footerCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbHead3.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbHead2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbHead1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbFoot3.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbFoot2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbFoot1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupFooter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupHeader)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem11)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void checkItem_ItemClick(object sender, ItemClickEventArgs e) {
			BrickAlignment aligment = BrickAlignment.Near;
			if(checkItem1.Checked)
				aligment = BrickAlignment.Near;
			else if(checkItem2.Checked)
				aligment = BrickAlignment.Center;
			else if(checkItem3.Checked)
				aligment = BrickAlignment.Far;
			if(IsHeaderSelected())
				headAligment = aligment;
			else
				footAligment = aligment;
		}
		private void headerCheckEdit_CheckedChanged(object sender, EventArgs e) {
			if(IsHeaderSelected())
				SetHeadAdministration();
			else
				SetFootAdministration();
		}
		private void SetHeadAdministration() {
			if(!tbHead2.Focused && !tbHead3.Focused)
				tbHead1.Focus();
			ShowAligment(headAligment);
			ShowFontInfo(fontDialogHead.Font);
		}
		private void SetFootAdministration() {
			if(!tbFoot2.Focused && !tbFoot3.Focused)
				tbFoot1.Focus();
			ShowAligment(footAligment);
			ShowFontInfo(fontDialogFoot.Font);
		}
		private void FillImageListMenu() {
			for(int i = 0; i < images.Count; i++) {
				BarButtonItem item = new BarButtonItem(barManager, i.ToString());
				item.ItemClick += new ItemClickEventHandler(imageMenu_ItemClick);
				item.Glyph = (Image)images[i];
				imageMenuItem.AddItem(item);
			}
		}
		protected void ShowHeaderArea(PageArea area) {
			ShowArea(area, headControls, fontDialogHead);
		}
		protected void ShowFooterArea(PageArea area) {
			ShowArea(area, footControls, fontDialogFoot);
		}
		void ShowArea(PageArea area, MemoEdit[] controls, FontDialog fontDialog) {
			fontDialog.Font = area.Font;
			ShowCollection(area.Content, controls);
			ShowFontInfo(fontDialog.Font);
			ShowAligment(area.LineAlignment);
		}
		protected void ShowAligment(BrickAlignment aligment) {
			checkItem1.Checked = aligment == BrickAlignment.Near;
			checkItem2.Checked = aligment == BrickAlignment.Center;
			checkItem3.Checked = aligment == BrickAlignment.Far;
		}
		private void ShowCollection(IList collection, MemoEdit[] controls) {
			for(int i = 0; i < controls.Length; i++)
				controls[i].Text = GetItem(collection, i);
		}
		private void ShowFontInfo(Font font) {
			FontConverter conv = new FontConverter();
			beiFont.EditValue = conv.ConvertToString(font);
		}
		private string GetItem(IList collection, int index) {
			if(collection == null || collection.Count == 0) return string.Empty;
			try {
				return collection[index] as string;
			} catch {
				return string.Empty;
			}
		}
		private void control_LostFocus(object sender, System.EventArgs e) {
			focusedMemoEdit = null;
		}
		private void control_GotFocus(object sender, System.EventArgs e) {
			focusedMemoEdit = (MemoEdit)sender;
			if(focusedMemoEdit == tbHead1
				|| focusedMemoEdit == tbHead2
				|| focusedMemoEdit == tbHead3)
				SelectHeader();
			else
				SelectFooter();
		}
		private void SetItemEnabled(BarItem item, bool enabled) {
			if(item != imageMenuItem)
				item.Enabled = enabled;
		}
		private void buttonOK_Click(object sender, System.EventArgs e) {
			Apply();
		}
		protected abstract void Apply();
		private void imageMenu_ItemClick(object sender, ItemClickEventArgs e) {
			string s = String.Format("[Image {0}]", e.Item.Caption);
			InsertText(s);
		}
		private void InsertText(string s) {
			if(focusedMemoEdit == null) return;
			int index = Math.Max(0, focusedMemoEdit.SelectionStart);
			string text = focusedMemoEdit.Text.Insert(index, s);
			focusedMemoEdit.Text = text;
			focusedMemoEdit.SelectionStart = index + s.Length;
		}
		private void EditFont(FontDialog fontDialog) {
			if(fontDialog.ShowDialog() != DialogResult.Cancel) {
				ShowFontInfo(fontDialog.Font);
			}
		}
		private void toolBar_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			if(e.Item.Tag is string) {
				InsertText((string)e.Item.Tag);
			}
		}
		private void beiFont_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			if(IsHeaderSelected()) 
				EditFont(fontDialogHead);
			else 
				EditFont(fontDialogFoot);
		}
		private void UpdateColors() {
			barDockControl1.Appearance.Options.UseBackColor = true;
			barDockControl1.Appearance.BackColor = BackColor;
		}
		private void OnLookAndFeelStyleChanged(object sender, EventArgs e) {
			UpdateColors();
		}
		protected void FocusTabHeader() {
			this.ActiveControl = tbHead1;
			tbHead1.SelectionStart = tbHead1.Text.Length;
			tbHead1.Focus();
		}
		bool IsHeaderSelected(){
			return this.headerCheckEdit.Checked;
		}
		void SelectHeader() {
			this.headerCheckEdit.Checked = true;
		}
		void SelectFooter() {
			this.footerCheckEdit.Checked = true;
		}
	}
	public class HeaderFooterForm : HeaderFooterFormBase {
		bool isRTLChanged;
		public HeaderFooterForm() {
			this.Load += new System.EventHandler(this.HeaderFooterFormBase_Load);
		}
		private PageHeaderFooter editValue;
		public object EditValue {
			get { return editValue; }
			set {
				if(value is PageHeaderFooter) {
					editValue = (PageHeaderFooter)value;
					HeadAligment = editValue.Header.LineAlignment;
					FootAligment = editValue.Footer.LineAlignment;
					ShowFooterArea(editValue.Footer);
					ShowHeaderArea(editValue.Header);
					FocusTabHeader();
				}
			}
		}
		protected override void Apply() {
			Apply(editValue.Header, HeadFont, HeadControls, HeadAligment);
			Apply(editValue.Footer, FootFont, FootControls, FootAligment);
		}
		private void Apply(PageArea area, Font font, MemoEdit[] controls, BrickAlignment aligment) {
			area.Font = font;
			area.Content.Clear();
			area.LineAlignment = aligment;
			FillCollection(area.Content, controls);
		}
		bool IsEmpty(MemoEdit[] controls) {
			foreach(MemoEdit control in controls)
				if(control.Text.Length > 0) return false;
			return true;
		}
		void FillCollection(IList collection, MemoEdit[] controls) {
			if(IsEmpty(controls)) return;
			foreach(MemoEdit control in controls)
				collection.Add(control.Text);
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			isRTLChanged = true;
		}
		private void HeaderFooterFormBase_Load(object sender, EventArgs e) {
			if(DevExpress.XtraEditors.WindowsFormsSettings.TouchUIMode == DevExpress.LookAndFeel.TouchUIMode.True) {
				this.ClientSize = new Size((int)(ClientSize.Width * 1.3), (int)(ClientSize.Height * 1.3));
			}
			InitializeLayout();
		}
		void InitializeLayout() {
			InitializeGroupButtonsLayout();
			Size minLayoutControlSize = (layoutControl1 as DevExpress.Utils.Controls.IXtraResizableControl).MinSize;
			if(minLayoutControlSize.Width > ClientSize.Width || minLayoutControlSize.Height > ClientSize.Height)
				ClientSize = new Size(Math.Max(minLayoutControlSize.Width, ClientSize.Width), Math.Max(minLayoutControlSize.Height, ClientSize.Height));
			if(isRTLChanged)
				DevExpress.XtraPrinting.Native.RTLHelper.ConvertGroupControlAlignments(layoutControlGroup1);
		}
		void InitializeGroupButtonsLayout() {
			int buttonOKBestWidth = buttonOK.CalcBestSize().Width;
			int buttonCancelBestWidth = buttonCancel.CalcBestSize().Width;
			if(buttonOKBestWidth <= buttonOK.Width && buttonCancelBestWidth <= buttonCancel.Width)
				return;
			int btnPrintOKActualSize = Math.Max(buttonOKBestWidth, buttonCancelBestWidth);
		   grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[1].Width =
		   grpButtons.OptionsTableLayoutGroup.ColumnDefinitions[3].Width = btnPrintOKActualSize + 2 + 2;
		}
	}
}
