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
using DevExpress.XtraEditors;
using DevExpress.Utils;
namespace DevExpress.DXperience.Demos {
	[ToolboxItem(false)]
	public class XtraPropertyGrid : XtraUserControl {
		private DevExpress.Utils.Frames.NotePanelEx pnlHint;
		private DevExpress.XtraBars.BarManager barManager1;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private DevExpress.XtraBars.Bar bMain;
		private System.Windows.Forms.Label lbCaption;
		private DevExpress.XtraEditors.PanelControl pncDescription;
		private DevExpress.XtraVerticalGrid.PropertyGridControl propertyGridControl1;
		private System.Windows.Forms.Panel pnlTop;
		private System.Windows.Forms.Panel pnlBottom;
		private DevExpress.XtraBars.BarCheckItem bciCategories;
		private DevExpress.XtraBars.BarCheckItem bciAlphabetical;
		private DevExpress.XtraBars.BarButtonItem biDescription;
		private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
		private DevExpress.XtraEditors.Repository.RepositoryItemColorEdit repositoryItemColorEdit1;
		private System.ComponentModel.IContainer components;
		public XtraPropertyGrid() {
			InitializeComponent();
			biDescription.Down = true;
			bciCategories.Checked = true;
			ImageCollection images = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.Tutorials.MainDemo.Properties.png", typeof(XtraPropertyGrid).Assembly, new Size(16, 16));
			barManager1.Images = images;
		}
		[DefaultValue(true)]
		public bool ShowDescription {
			get { return biDescription.Down; }
			set {
				biDescription.Down = value;
			}
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
				bMain.Visible = pnlTop.Visible = value;
			}
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XtraPropertyGrid));
			this.pnlHint = new DevExpress.Utils.Frames.NotePanelEx();
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.bMain = new DevExpress.XtraBars.Bar();
			this.bciCategories = new DevExpress.XtraBars.BarCheckItem();
			this.bciAlphabetical = new DevExpress.XtraBars.BarCheckItem();
			this.biDescription = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.lbCaption = new System.Windows.Forms.Label();
			this.pncDescription = new DevExpress.XtraEditors.PanelControl();
			this.propertyGridControl1 = new DevExpress.XtraVerticalGrid.PropertyGridControl();
			this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.repositoryItemColorEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
			this.pnlTop = new System.Windows.Forms.Panel();
			this.pnlBottom = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pncDescription)).BeginInit();
			this.pncDescription.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.propertyGridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.pnlHint, "pnlHint");
			this.pnlHint.ForeColor = System.Drawing.Color.Black;
			this.pnlHint.MaxRows = 10;
			this.pnlHint.Name = "pnlHint";
			this.pnlHint.TabStop = false;
			this.barManager1.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.bMain});
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.Form = this;
			this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.bciCategories,
			this.bciAlphabetical,
			this.biDescription});
			this.barManager1.MaxItemId = 3;
			this.bMain.BarName = "Main";
			this.bMain.DockCol = 0;
			this.bMain.DockRow = 0;
			this.bMain.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.bMain.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.bciCategories),
			new DevExpress.XtraBars.LinkPersistInfo(this.bciAlphabetical),
			new DevExpress.XtraBars.LinkPersistInfo(this.biDescription, true)});
			this.bMain.OptionsBar.AllowDelete = true;
			this.bMain.OptionsBar.AllowQuickCustomization = false;
			this.bMain.OptionsBar.DrawDragBorder = false;
			this.bMain.OptionsBar.UseWholeRow = true;
			resources.ApplyResources(this.bMain, "bMain");
			this.bciCategories.GroupIndex = 1;
			resources.ApplyResources(this.bciCategories, "bciCategories");
			this.bciCategories.Id = 0;
			this.bciCategories.ImageIndex = 0;
			this.bciCategories.Name = "bciCategories";
			this.bciCategories.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.bci_CheckedChanged);
			this.bciAlphabetical.GroupIndex = 1;
			resources.ApplyResources(this.bciAlphabetical, "bciAlphabetical");
			this.bciAlphabetical.Id = 1;
			this.bciAlphabetical.ImageIndex = 1;
			this.bciAlphabetical.Name = "bciAlphabetical";
			this.bciAlphabetical.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.bci_CheckedChanged);
			this.biDescription.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
			resources.ApplyResources(this.biDescription, "biDescription");
			this.biDescription.Id = 2;
			this.biDescription.ImageIndex = 2;
			this.biDescription.Name = "biDescription";
			this.biDescription.DownChanged += new DevExpress.XtraBars.ItemClickEventHandler(this.biDescription_DownChanged);
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			this.lbCaption.BackColor = System.Drawing.SystemColors.Info;
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.lbCaption.Name = "lbCaption";
			this.pncDescription.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("pncDescription.Appearance.BackColor")));
			this.pncDescription.Appearance.Options.UseBackColor = true;
			this.pncDescription.Controls.Add(this.lbCaption);
			this.pncDescription.Controls.Add(this.pnlHint);
			resources.ApplyResources(this.pncDescription, "pncDescription");
			this.pncDescription.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
			this.pncDescription.LookAndFeel.UseDefaultLookAndFeel = false;
			this.pncDescription.Name = "pncDescription";
			this.propertyGridControl1.DefaultEditors.AddRange(new DevExpress.XtraVerticalGrid.Rows.DefaultEditor[] {
			new DevExpress.XtraVerticalGrid.Rows.DefaultEditor(typeof(bool), this.repositoryItemCheckEdit1),
			new DevExpress.XtraVerticalGrid.Rows.DefaultEditor(typeof(System.Drawing.Color), this.repositoryItemColorEdit1)});
			resources.ApplyResources(this.propertyGridControl1, "propertyGridControl1");
			this.propertyGridControl1.Name = "propertyGridControl1";
			this.propertyGridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemCheckEdit1,
			this.repositoryItemColorEdit1});
			this.propertyGridControl1.FocusedRowChanged += new DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventHandler(this.propertyGridControl1_FocusedRowChanged);
			resources.ApplyResources(this.repositoryItemCheckEdit1, "repositoryItemCheckEdit1");
			this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
			resources.ApplyResources(this.repositoryItemColorEdit1, "repositoryItemColorEdit1");
			this.repositoryItemColorEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("repositoryItemColorEdit1.Buttons"))))});
			this.repositoryItemColorEdit1.Name = "repositoryItemColorEdit1";
			this.repositoryItemColorEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
			resources.ApplyResources(this.pnlTop, "pnlTop");
			this.pnlTop.Name = "pnlTop";
			resources.ApplyResources(this.pnlBottom, "pnlBottom");
			this.pnlBottom.Name = "pnlBottom";
			this.Controls.Add(this.propertyGridControl1);
			this.Controls.Add(this.pnlBottom);
			this.Controls.Add(this.pnlTop);
			this.Controls.Add(this.pncDescription);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "XtraPropertyGrid";
			resources.ApplyResources(this, "$this");
			this.Resize += new System.EventHandler(this.XtraPropertyGrid_Resize);
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pncDescription)).EndInit();
			this.pncDescription.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.propertyGridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemColorEdit1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void propertyGridControl1_FocusedRowChanged(object sender, DevExpress.XtraVerticalGrid.Events.FocusedRowChangedEventArgs e) {
			PropertyDescriptor descriptor = null;
			if(e.Row != null) descriptor = propertyGridControl1.GetPropertyDescriptor(e.Row);
			if(descriptor != null) {
				lbCaption.Text = descriptor.Name;
				pnlHint.Text = descriptor.Description;
			} else if(e.Row != null) {
				lbCaption.Text = e.Row.Properties.Caption;
				pnlHint.Text = string.Empty;
			} else {
				lbCaption.Text = pnlHint.Text = string.Empty;
			}
			SetPanelHeight();
		}
		void SetPanelHeight() {
			pncDescription.Height = lbCaption.Height + pnlHint.Height + 4;
		}
		private void XtraPropertyGrid_Resize(object sender, System.EventArgs e) {
			SetPanelHeight();
		}
		private void bci_CheckedChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			propertyGridControl1.OptionsView.ShowRootCategories = bciCategories.Checked;
		}
		private void biDescription_DownChanged(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			pncDescription.Visible = pnlBottom.Visible = biDescription.Down;
		}
		public DevExpress.XtraVerticalGrid.PropertyGridControl PropertyGrid {
			get { return propertyGridControl1; }
		}
	}
}
