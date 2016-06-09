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
namespace DevExpress.XtraBars.Ribbon.Design
{
	public class GalleryToolbar : DevExpress.XtraEditors.XtraUserControl
	{
		private DevExpress.XtraBars.Ribbon.RibbonControl ribbonControl1;
		private DevExpress.XtraBars.Ribbon.RibbonPage ribbonPage1;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup1;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup2;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup4;
		private DevExpress.XtraBars.BarButtonItem btAddItem;
		private DevExpress.XtraBars.BarButtonItem btMoveUp;
		private DevExpress.XtraBars.BarButtonItem btMoveDown;
		private DevExpress.XtraBars.BarButtonItem btRemove;
		private DevExpress.XtraBars.BarButtonItem btAddInplaceGallery;
		private DevExpress.XtraBars.BarButtonItem btAddPopupGallery;
		private DevExpress.Utils.ImageCollection imageCollection1;
		private DevExpress.XtraBars.BarButtonItem btAddGroup;
		private DevExpress.XtraBars.BarButtonItem btGenerateValues;
		private DevExpress.XtraBars.Ribbon.RibbonPageGroup ribbonPageGroup3;
		private System.ComponentModel.IContainer components;
		public DevExpress.XtraBars.BarAndDockingController barAndDockingController1;
		public GalleryToolbar()
		{
			InitializeComponent();
			barAndDockingController1.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(GalleryToolbar));
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
			this.btAddGroup = new DevExpress.XtraBars.BarButtonItem();
			this.btAddItem = new DevExpress.XtraBars.BarButtonItem();
			this.btMoveUp = new DevExpress.XtraBars.BarButtonItem();
			this.btMoveDown = new DevExpress.XtraBars.BarButtonItem();
			this.btRemove = new DevExpress.XtraBars.BarButtonItem();
			this.btAddInplaceGallery = new DevExpress.XtraBars.BarButtonItem();
			this.btAddPopupGallery = new DevExpress.XtraBars.BarButtonItem();
			this.btGenerateValues = new BarButtonItem();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup2 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup3 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonPageGroup4 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
			this.SuspendLayout();
			this.ribbonControl1.Controller = this.barAndDockingController1;
			this.ribbonControl1.ApplicationIcon = null;
			this.ribbonControl1.Images = this.imageCollection1;
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
																					 this.btAddGroup,
																					 this.btAddItem,
																					 this.btMoveUp,
																					 this.btMoveDown,
																					 this.btRemove,
																					 this.btAddInplaceGallery,
																					 this.btAddPopupGallery,
																					 this.btGenerateValues});
			this.ribbonControl1.MaxItemId = 8;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
																							   this.ribbonPage1});
			this.ribbonControl1.SelectedPage = this.ribbonPage1;
			this.ribbonControl1.ShowPageHeadersMode = ShowPageHeadersMode.Hide;
			this.ribbonControl1.Size = new System.Drawing.Size(744, 94);
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			this.imageCollection1.ImageSize = new System.Drawing.Size(32, 32);
			this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
			this.barAndDockingController1.PropertiesBar.AllowLinkLighting = false;
			this.btAddGroup.Caption = "Add Group";
			this.btAddGroup.Id = 0;
			this.btAddGroup.ImageIndex = 2;
			this.btAddGroup.Name = "btAddGroup";
			this.btAddGroup.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.btAddItem.Caption = "Add Item";
			this.btAddItem.Id = 1;
			this.btAddItem.ImageIndex = 3;
			this.btAddItem.Name = "btAddItem";
			this.btAddItem.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
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
			this.btAddInplaceGallery.Caption = "Add In-place Gallery";
			this.btAddInplaceGallery.Id = 5;
			this.btAddInplaceGallery.ImageIndex = 0;
			this.btAddInplaceGallery.Name = "btAddInplaceGallery";
			this.btAddInplaceGallery.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.btAddPopupGallery.Caption = "Add Dropdown Gallery";
			this.btAddPopupGallery.Id = 6;
			this.btAddPopupGallery.ImageIndex = 1;
			this.btAddPopupGallery.Name = "btAddPopupGallery";
			this.btAddPopupGallery.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.btGenerateValues.Caption = "Generate Values";
			this.btGenerateValues.Id = 7;
			this.btGenerateValues.ImageIndex = 3;
			this.btGenerateValues.Name = "btGenerateValues";
			this.btGenerateValues.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonItemStyles.Large;
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
																								  this.ribbonPageGroup1,
																								  this.ribbonPageGroup3,
																								  this.ribbonPageGroup2,
																								  this.ribbonPageGroup4});
			this.ribbonPage1.Name = "ribbonPage1";
			this.ribbonPage1.Text = "ribbonPage1";
			this.ribbonPageGroup1.ItemLinks.Add(this.btAddInplaceGallery);
			this.ribbonPageGroup1.ItemLinks.Add(this.btAddPopupGallery);
			this.ribbonPageGroup1.Name = "ribbonPageGroup1";
			this.ribbonPageGroup1.Text = "Galleries";
			this.ribbonPageGroup1.AllowTextClipping = false;
			this.ribbonPageGroup2.ItemLinks.Add(this.btMoveUp);
			this.ribbonPageGroup2.ItemLinks.Add(this.btMoveDown);
			this.ribbonPageGroup2.ItemLinks.Add(this.btRemove);
			this.ribbonPageGroup2.Name = "ribbonPageGroup2";
			this.ribbonPageGroup2.Text = "Move";
			this.ribbonPageGroup2.AllowTextClipping = false;
			this.ribbonPageGroup3.ItemLinks.Add(this.btAddGroup);
			this.ribbonPageGroup3.ItemLinks.Add(this.btAddItem);
			this.ribbonPageGroup3.Name = "ribbonPageGroup3";
			this.ribbonPageGroup3.Text = "Groups and Items";
			this.ribbonPageGroup3.AllowTextClipping = false;
			this.ribbonPageGroup4.ItemLinks.Add(this.btGenerateValues);
			this.ribbonPageGroup4.Name = "ribbonPageGroup4";
			this.ribbonPageGroup4.Text = "Values";
			this.ribbonPageGroup4.AllowTextClipping = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.ribbonControl1});
			this.Name = "GalleryToolbar";
			this.Size = new System.Drawing.Size(744, 94);
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public DevExpress.XtraBars.Ribbon.RibbonControl ToolbarRibbon { get { return ribbonControl1; } }
		public DevExpress.XtraBars.BarButtonItem AddInplaceGallery { get { return btAddInplaceGallery; } }
		public DevExpress.XtraBars.BarButtonItem AddPopupGallery { get { return btAddPopupGallery; } }
		public DevExpress.XtraBars.BarButtonItem AddGroup { get { return btAddGroup; } }
		public DevExpress.XtraBars.BarButtonItem AddItem { get { return btAddItem; } }
		public DevExpress.XtraBars.BarButtonItem MoveUp { get { return btMoveUp; } }
		public DevExpress.XtraBars.BarButtonItem MoveDown { get { return btMoveDown; } }
		public DevExpress.XtraBars.BarButtonItem Remove { get { return btRemove; } }
		public DevExpress.XtraBars.BarButtonItem GenerateValues { get { return btGenerateValues; } }
		public bool ShowGalleryButtons { get { return ribbonPageGroup1.Visible; } set { ribbonPageGroup1.Visible = value; } }
	}
}
