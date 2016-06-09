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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.XtraBars.Design {
	public class TileControlToolbar : DevExpress.XtraEditors.XtraUserControl {
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
		private DevExpress.XtraBars.BarButtonItem btAddItem;
		private DevExpress.XtraBars.BarButtonItem btMoveUp;
		private DevExpress.XtraBars.BarButtonItem btMoveDown;
		private DevExpress.XtraBars.BarButtonItem btRemove;
		private DevExpress.XtraBars.BarButtonItem btAddInplaceTileControl;
		private DevExpress.XtraBars.BarButtonItem btAddPopupTileControl;
		private DevExpress.XtraBars.BarButtonItem btAddGroup;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
		private System.ComponentModel.IContainer components;
		private DevExpress.Utils.ImageCollection imageCollection1;
		public DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
		private BarButtonItem btnAddMediumItemPopupCmd;
		private BarButtonItem btnAddWideItemPopupCmd;
		private BarButtonItem btnAddSmallItemPopupCmd;
		private BarButtonItem btnAddLargeItemPopupCmd;
		private DevExpress.XtraBars.PopupMenu popupMenu1;
		public TileControlToolbar() {
			InitializeComponent();
			barAndDockingController1.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TileControlToolbar));
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
			this.btAddGroup = new DevExpress.XtraBars.BarButtonItem();
			this.btAddItem = new DevExpress.XtraBars.BarButtonItem();
			this.popupMenu1 = new DevExpress.XtraBars.PopupMenu(this.components);
			this.btnAddMediumItemPopupCmd = new DevExpress.XtraBars.BarButtonItem();
			this.btnAddWideItemPopupCmd = new DevExpress.XtraBars.BarButtonItem();
			this.btnAddSmallItemPopupCmd = new DevExpress.XtraBars.BarButtonItem();
			this.btnAddLargeItemPopupCmd = new DevExpress.XtraBars.BarButtonItem();
			this.btMoveUp = new DevExpress.XtraBars.BarButtonItem();
			this.btMoveDown = new DevExpress.XtraBars.BarButtonItem();
			this.btRemove = new DevExpress.XtraBars.BarButtonItem();
			this.btAddInplaceTileControl = new DevExpress.XtraBars.BarButtonItem();
			this.btAddPopupTileControl = new DevExpress.XtraBars.BarButtonItem();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).BeginInit();
			this.SuspendLayout();
			this.ribbonControl1.Controller = this.barAndDockingController1;
			this.ribbonControl1.ExpandCollapseItem.Id = 0;
			this.ribbonControl1.Images = this.imageCollection1;
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.ribbonControl1.ExpandCollapseItem,
			this.btAddGroup,
			this.btAddItem,
			this.btMoveUp,
			this.btMoveDown,
			this.btRemove,
			this.btAddInplaceTileControl,
			this.btAddPopupTileControl,
			this.btnAddMediumItemPopupCmd,
			this.btnAddWideItemPopupCmd,
			this.btnAddSmallItemPopupCmd,
			this.btnAddLargeItemPopupCmd});
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.MaxItemId = 9;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.ribbonPage1});
			this.ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
			this.ribbonControl1.Size = new System.Drawing.Size(744, 96);
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
			this.barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.imageCollection1.ImageSize = new System.Drawing.Size(32, 32);
			this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
			this.btAddGroup.Caption = "Add Group";
			this.btAddGroup.Id = 0;
			this.btAddGroup.ImageIndex = 2;
			this.btAddGroup.Name = "btAddGroup";
			this.btAddGroup.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.btAddItem.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			this.btAddItem.Caption = "Add Item";
			this.btAddItem.DropDownControl = this.popupMenu1;
			this.btAddItem.Id = 1;
			this.btAddItem.ImageIndex = 3;
			this.btAddItem.Name = "btAddItem";
			this.btAddItem.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.popupMenu1.ItemLinks.Add(this.btnAddMediumItemPopupCmd);
			this.popupMenu1.ItemLinks.Add(this.btnAddWideItemPopupCmd);
			this.popupMenu1.ItemLinks.Add(this.btnAddSmallItemPopupCmd);
			this.popupMenu1.ItemLinks.Add(this.btnAddLargeItemPopupCmd);
			this.popupMenu1.Name = "popupMenu1";
			this.popupMenu1.Ribbon = this.ribbonControl1;
			this.btnAddMediumItemPopupCmd.Caption = "Medium Item";
			this.btnAddMediumItemPopupCmd.Id = 7;
			this.btnAddMediumItemPopupCmd.Name = "btnAddMediumItemPopupCmd";
			this.btnAddWideItemPopupCmd.Caption = "Wide Item";
			this.btnAddWideItemPopupCmd.Id = 8;
			this.btnAddWideItemPopupCmd.Name = "btnAddWideItemPopupCmd";
			this.btnAddSmallItemPopupCmd.Caption = "Small Item";
			this.btnAddSmallItemPopupCmd.Id = 9;
			this.btnAddSmallItemPopupCmd.Name = "btnAddSmallItemPopupCmd";
			this.btnAddLargeItemPopupCmd.Caption = "Large Item";
			this.btnAddLargeItemPopupCmd.Id = 10;
			this.btnAddLargeItemPopupCmd.Name = "btnAddLargeItemPopupCmd";
			this.btMoveUp.Caption = "Move Up";
			this.btMoveUp.Id = 2;
			this.btMoveUp.ImageIndex = 4;
			this.btMoveUp.Name = "btMoveUp";
			this.btMoveUp.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.btMoveDown.Caption = "Move Down";
			this.btMoveDown.Id = 3;
			this.btMoveDown.ImageIndex = 5;
			this.btMoveDown.Name = "btMoveDown";
			this.btMoveDown.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.btRemove.Caption = "Remove";
			this.btRemove.Id = 4;
			this.btRemove.ImageIndex = 6;
			this.btRemove.Name = "btRemove";
			this.btRemove.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.btAddInplaceTileControl.Caption = "Add In-place TileControl";
			this.btAddInplaceTileControl.Id = 5;
			this.btAddInplaceTileControl.ImageIndex = 0;
			this.btAddInplaceTileControl.Name = "btAddInplaceTileControl";
			this.btAddInplaceTileControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.btAddPopupTileControl.Caption = "Add DropdownTileControl";
			this.btAddPopupTileControl.Id = 6;
			this.btAddPopupTileControl.ImageIndex = 1;
			this.btAddPopupTileControl.Name = "btAddPopupTileControl";
			this.btAddPopupTileControl.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.ribbonPageGroup3,
			this.ribbonPageGroup2});
			this.ribbonPage1.Name = "ribbonPage1";
			this.ribbonPage1.Text = "ribbonPage1";
			this.ribbonPageGroup3.ItemLinks.Add(this.btAddGroup);
			this.ribbonPageGroup3.ItemLinks.Add(this.btAddItem);
			this.ribbonPageGroup3.Name = "ribbonPageGroup3";
			this.ribbonPageGroup3.Text = "Groups and Items";
			this.ribbonPageGroup2.ItemLinks.Add(this.btMoveUp);
			this.ribbonPageGroup2.ItemLinks.Add(this.btMoveDown);
			this.ribbonPageGroup2.ItemLinks.Add(this.btRemove);
			this.ribbonPageGroup2.Name = "ribbonPageGroup2";
			this.ribbonPageGroup2.Text = "Move";
			this.Controls.Add(this.ribbonControl1);
			this.Name = "TileControlToolbar";
			this.Size = new System.Drawing.Size(744, 94);
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public RibbonControl ToolbarRibbon { get { return ribbonControl1; } }
		public BarButtonItem AddInplaceTileControl { get { return btAddInplaceTileControl; } }
		public BarButtonItem AddPopupTileControl { get { return btAddPopupTileControl; } }
		public BarButtonItem AddGroup { get { return btAddGroup; } }
		public BarButtonItem AddItem { get { return btAddItem; } }
		public BarButtonItem MoveUp { get { return btMoveUp; } }
		public BarButtonItem MoveDown { get { return btMoveDown; } }
		public BarButtonItem Remove { get { return btRemove; } }
		public BarButtonItem AddMediumItemPopupCmd { get { return btnAddMediumItemPopupCmd; } }
		public BarButtonItem AddWideItemPopupCmd { get { return btnAddWideItemPopupCmd; } }
		public BarButtonItem AddLargeItemPopupCmd { get { return btnAddLargeItemPopupCmd; } }
		public BarButtonItem AddSmallItemPopupCmd { get { return btnAddSmallItemPopupCmd; } }
	}
}
