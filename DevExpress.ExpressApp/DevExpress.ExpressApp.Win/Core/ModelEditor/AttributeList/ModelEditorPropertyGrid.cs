#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.ExpressApp.Utils;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	[ToolboxItem(false)]
	public class ModelEditorPropertyGrid : XtraUserControl {
		private DevExpress.Utils.Frames.NotePanelEx pnlHint;
		private DevExpress.XtraBars.BarManager barManager1;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private DevExpress.XtraBars.Bar bMain;
		private AttributesPropertyGrid propertyGridControl1;
		private DevExpress.XtraBars.BarCheckItem bciCategories;
		private DevExpress.XtraBars.BarCheckItem bciAlphabetical;
		private DevExpress.XtraBars.BarButtonItem resetValue;
		private DevExpress.XtraBars.BarButtonItem navigateToRefNode;
		private DevExpress.XtraBars.BarButtonItem navigateToCalculatedProperty;
		private System.ComponentModel.IContainer components;
		public ModelEditorPropertyGrid() {
			InitializeComponent();
			bciCategories.Checked = true;
			SetImages();
			SetLargeImages();
		}
		private void SetLargeImages() {
			ImageCollection images = new ImageCollection();
			images.ImageSize = new Size(32, 32);
			DevExpress.ExpressApp.Utils.ImageInfo imageCategorized = ImageLoader.Instance.GetImageInfo("ModelEditor_Categorized_32x32");
			if(imageCategorized != null) {
				images.AddImage(imageCategorized.Image);
			}
			DevExpress.ExpressApp.Utils.ImageInfo imageAlphabetic = ImageLoader.Instance.GetImageInfo("ModelEditor_Alphabetic_32x32");
			if(imageAlphabetic != null) {
				images.AddImage(imageAlphabetic.Image);
			}
			DevExpress.ExpressApp.Utils.ImageInfo imageReset = ImageLoader.Instance.GetImageInfo("Action_Cancel_32x32");
			if(imageReset != null) {
				images.AddImage(imageReset.Image);
			}
			DevExpress.ExpressApp.Utils.ImageInfo imageNavigateToRefNode = ImageLoader.Instance.GetImageInfo("Action_Open_Object_32x32");
			if(imageNavigateToRefNode != null) {
				images.AddImage(imageNavigateToRefNode.Image);
			}
			DevExpress.ExpressApp.Utils.ImageInfo imageNavigateToRefValue = ImageLoader.Instance.GetImageInfo("ModelEditor_Hyperlink_32x32");
			if(imageNavigateToRefValue != null) {
				images.AddImage(imageNavigateToRefValue.Image);
			}
			barManager1.LargeImages = images;
		}
		private void SetImages() {
			ImageCollection images = new ImageCollection();
			DevExpress.ExpressApp.Utils.ImageInfo imageCategorized = ImageLoader.Instance.GetImageInfo("ModelEditor_Categorized");
			if(imageCategorized != null) {
				images.AddImage(imageCategorized.Image);
			}
			DevExpress.ExpressApp.Utils.ImageInfo imageAlphabetic = ImageLoader.Instance.GetImageInfo("ModelEditor_Alphabetic");
			if(imageAlphabetic != null) {
				images.AddImage(imageAlphabetic.Image);
			}
			DevExpress.ExpressApp.Utils.ImageInfo imageReset = ImageLoader.Instance.GetImageInfo("Action_Cancel");
			if(imageReset != null) {
				images.AddImage(imageReset.Image);
			}
			DevExpress.ExpressApp.Utils.ImageInfo imageNavigateToRefNode = ImageLoader.Instance.GetImageInfo("Action_Open_Object");
			if(imageNavigateToRefNode != null) {
				images.AddImage(imageNavigateToRefNode.Image);
			}
			DevExpress.ExpressApp.Utils.ImageInfo imageNavigateToRefValue = ImageLoader.Instance.GetImageInfo("ModelEditor_Hyperlink");
			if(imageNavigateToRefValue != null) {
				images.AddImage(imageNavigateToRefValue.Image);
			}
			barManager1.Images = images;
		}
		[DefaultValue(true)]
		public bool ShowCategories {
			get { return bciCategories.Checked; }
			set {
				if(value)
					bciCategories.Checked = true;
				else bciAlphabetical.Checked = true;
			}
		}
		[DefaultValue(true)]
		public bool ShowButtons {
			get { return bMain.Visible; }
			set {
				bMain.Visible = value;
			}
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
			this.pnlHint = new DevExpress.Utils.Frames.NotePanelEx();
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.bMain = new DevExpress.XtraBars.Bar();
			this.bciCategories = new DevExpress.XtraBars.BarCheckItem();
			this.bciAlphabetical = new DevExpress.XtraBars.BarCheckItem();
			this.resetValue = new DevExpress.XtraBars.BarButtonItem();
			this.navigateToRefNode = new DevExpress.XtraBars.BarButtonItem();
			this.navigateToCalculatedProperty = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.propertyGridControl1 = new AttributesPropertyGrid();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGridControl1)).BeginInit();
			this.SuspendLayout();
			this.pnlHint.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlHint.Location = new System.Drawing.Point(3, 30);
			this.pnlHint.MaxRows = 10;
			this.pnlHint.Name = "pnlHint";
			this.pnlHint.Size = new System.Drawing.Size(266, 23);
			this.pnlHint.TabIndex = 0;
			this.pnlHint.TabStop = false;
			this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
																			 this.bMain});
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
				this.bciCategories, this.bciAlphabetical, this.resetValue, this.navigateToRefNode, this.navigateToCalculatedProperty 
			});
			this.barManager1.MaxItemId = 5;
			this.bMain.BarName = "Main";
			this.bMain.DockCol = 0;
			this.bMain.DockRow = 0;
			this.bMain.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			DevExpress.XtraBars.LinkPersistInfo linkPersistInfo1 = new DevExpress.XtraBars.LinkPersistInfo(this.resetValue);
			linkPersistInfo1.BeginGroup = true;
			this.bMain.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
				new DevExpress.XtraBars.LinkPersistInfo(this.bciCategories), new DevExpress.XtraBars.LinkPersistInfo(this.bciAlphabetical), 
				linkPersistInfo1, new DevExpress.XtraBars.LinkPersistInfo(this.navigateToRefNode),
				new DevExpress.XtraBars.LinkPersistInfo(this.navigateToCalculatedProperty)
			});
			this.bMain.OptionsBar.AllowDelete = true;
			this.bMain.OptionsBar.AllowQuickCustomization = false;
			this.bMain.OptionsBar.DrawDragBorder = false;
			this.bMain.OptionsBar.UseWholeRow = true;
			this.bMain.Text = "Main";
			this.bciCategories.GroupIndex = 1;
			this.bciCategories.Caption = "Categorized";
			this.bciCategories.Hint = "Categorized";
			this.bciCategories.Id = 0;
			this.bciCategories.ImageIndex = 0;
			this.bciCategories.LargeImageIndex = 0;
			this.bciCategories.Name = "bciCategories";
			this.bciCategories.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.bci_CheckedChanged);
			this.bciAlphabetical.GroupIndex = 1;
			this.bciAlphabetical.Caption = "Alphabetic";
			this.bciAlphabetical.Hint = "Alphabetic";
			this.bciAlphabetical.Id = 1;
			this.bciAlphabetical.ImageIndex = 1;
			this.bciAlphabetical.LargeImageIndex = 1;
			this.bciAlphabetical.Name = "bciAlphabetical";
			this.bciAlphabetical.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.bci_CheckedChanged);
			this.resetValue.GroupIndex = 2;
			this.resetValue.Caption = "Undo Changes";
			this.resetValue.Hint = "Undo Changes";
			this.resetValue.Id = 2;
			this.resetValue.Name = "resetValue";
			this.resetValue.ImageIndex = 2;
			this.resetValue.LargeImageIndex = 2;
			this.resetValue.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(resetValue_ItemClick);
			this.resetValue.Enabled = false;
			this.resetValue.SuperTip = new SuperToolTip();
			this.resetValue.ItemShortcut = new DevExpress.XtraBars.BarShortcut(Shortcut.CtrlZ);
			this.resetValue.SuperTip.Items.Add(resetValue.Hint + " (Ctrl+Z)");
			this.navigateToRefNode.GroupIndex = 2;
			this.navigateToRefNode.Caption = "Open Related Object";
			this.navigateToRefNode.Hint = "Open the object associated with the focused editor";
			this.navigateToRefNode.Id = 4;
			this.navigateToRefNode.ImageIndex = 3;
			this.navigateToRefNode.LargeImageIndex = 3;
			this.navigateToRefNode.Name = "navigateToRefNode";
			this.navigateToRefNode.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(navigateToRefNode_ItemClick);
			this.navigateToRefNode.Enabled = false;
			this.navigateToRefNode.SuperTip = new SuperToolTip();
			this.navigateToRefNode.SuperTip.Items.Add("Open Related Object (Alt+O)");
			this.navigateToRefNode.SuperTip.Items.Add(navigateToRefNode.Hint);
			this.navigateToCalculatedProperty.GroupIndex = 2;
			this.navigateToCalculatedProperty.Caption = "Open Source Property";
			this.navigateToCalculatedProperty.Hint = "Open Source Property";
			this.navigateToCalculatedProperty.Id = 5;
			this.navigateToCalculatedProperty.ImageIndex = 4;
			this.navigateToCalculatedProperty.LargeImageIndex = 4;
			this.navigateToCalculatedProperty.Name = "navigateToCalculatedProperty";
			this.navigateToCalculatedProperty.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(navigateToCalculatedProperty_ItemClick);
			this.navigateToCalculatedProperty.Enabled = false;
			this.navigateToCalculatedProperty.SuperTip = new SuperToolTip();
			this.navigateToCalculatedProperty.SuperTip.Items.Add(navigateToCalculatedProperty.Hint + " (Alt+N)");
			this.propertyGridControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridControl1.Location = new System.Drawing.Point(0, 26);
			this.propertyGridControl1.Name = "propertyGridControl1";
			this.propertyGridControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.propertyGridControl1.Size = new System.Drawing.Size(272, 316);
			this.propertyGridControl1.TabIndex = 7;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.propertyGridControl1,
																		  this.barDockControlLeft,
																		  this.barDockControlRight,
																		  this.barDockControlBottom,
																		  this.barDockControlTop});
			this.Name = "XtraPropertyGrid";
			this.Size = new System.Drawing.Size(272, 400);
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGridControl1)).EndInit();
			this.ResumeLayout(false);
			propertyGridControl1.FocusedRowChanged += new DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventHandler(PropertyGrid_FocusedRowChanged);
			propertyGridControl1.RowChanged += new DevExpress.XtraVerticalGrid.Events.RowChangedEventHandler(PropertyGrid_RowChanged);
			propertyGridControl1.CustomFocusedPropertyChanged += new EventHandler(propertyGridControl1_CustomFocusedPropertyChanged);
		}
		private void UpdateActionState(string focusedPropertyName) {
			if(!string.IsNullOrEmpty(focusedPropertyName)) {
				ModelPropertyEditorCellProperty cellProperty = propertyGridControl1.OnCalculateCellProperty(focusedPropertyName);
				this.resetValue.Enabled = (cellProperty & ModelPropertyEditorCellProperty.ModifiedProperty) == ModelPropertyEditorCellProperty.ModifiedProperty;
				this.navigateToRefNode.Enabled = (cellProperty & ModelPropertyEditorCellProperty.RefProperty) == ModelPropertyEditorCellProperty.RefProperty;
				this.navigateToCalculatedProperty.Enabled = (cellProperty & ModelPropertyEditorCellProperty.CalculateProperty) == ModelPropertyEditorCellProperty.CalculateProperty;
			}
			else {
				this.resetValue.Enabled = false;
				this.navigateToRefNode.Enabled = false;
				this.navigateToCalculatedProperty.Enabled = false;
			}
		}
		void propertyGridControl1_CustomFocusedPropertyChanged(object sender, EventArgs e) {
			UpdateActionState(propertyGridControl1.FocusedPropertyName);
		}
		void PropertyGrid_RowChanged(object sender, DevExpress.XtraVerticalGrid.Events.RowChangedEventArgs e) {
			UpdateActionState(propertyGridControl1.FocusedPropertyName);
		}
		void PropertyGrid_FocusedRowChanged(object sender, DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventArgs e) {
			if(e.Row != null) {
				UpdateActionState(e.Row.Properties.FieldName);
			}
		}
		void resetValue_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			propertyGridControl1.ResetValue();
		}
		void navigateToCalculatedProperty_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			propertyGridControl1.OnNavigateToCalculatedProperty();
		}
		void navigateToRefNode_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			propertyGridControl1.OnNavigateToNode();
		}
		#endregion
		private void bci_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			propertyGridControl1.OptionsView.ShowRootCategories = bciCategories.Checked;
		}
		public IModelAttributesPropertyEditorControl PropertyGrid {
			get { return propertyGridControl1; }
		}
	}
}
