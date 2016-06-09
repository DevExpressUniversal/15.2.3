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
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils.Frames;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.ButtonsPanelControl;
namespace DevExpress.XtraEditors.Frames {
	[ToolboxItem(false)]
	public class AppearancesDesignerBase : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		protected DevExpress.XtraEditors.SplitterControl scAppearance;
		protected DevExpress.XtraEditors.GroupControl gcAppearances;
		public GroupControl gcPreview;
		protected DevExpress.XtraEditors.ListBoxControl lbcAppearances;
		private DevExpress.XtraEditors.SimpleButton btnSave;
		private DevExpress.XtraEditors.SimpleButton btnLoad;
		private System.ComponentModel.Container components = null;
		protected DevExpress.XtraEditors.Designer.Utils.FilterButtonPanel bpAppearances;
		public PanelControl pnlAppearances;
		public PanelControl pnlPreview;
		MenuItem miShowAppearancePreview = null;
		public AppearancesDesignerBase() {
			InitializeComponent();
			pgMain.BringToFront();
			if(CreateAppearancesPreview) {
				miShowAppearancePreview = new MenuItem(Properties.Resources.AppearancesPreviewCaption, new EventHandler(miShow_Click));
				miShowAppearancePreview.Checked = true;
				pgMain.PropertyGridMenu.MenuItems.Add(miShowAppearancePreview);
			}
		}
		public override void StoreLocalProperties(PropertyStore localStore) {
			localStore.AddProperty("ShowAppearancePreview", miShowAppearancePreview.Checked);
			base.StoreLocalProperties(localStore);
		}
		public override void RestoreLocalProperties(PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			miShowAppearancePreview.Checked = localStore.RestoreBoolProperty("ShowAppearancePreview", miShowAppearancePreview.Checked);
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppearancesDesignerBase));
			this.scAppearance = new DevExpress.XtraEditors.SplitterControl();
			this.gcAppearances = new DevExpress.XtraEditors.GroupControl();
			this.lbcAppearances = new DevExpress.XtraEditors.ListBoxControl();
			this.bpAppearances = new DevExpress.XtraEditors.Designer.Utils.FilterButtonPanel();
			this.gcPreview = new DevExpress.XtraEditors.GroupControl();
			this.btnSave = new DevExpress.XtraEditors.SimpleButton();
			this.btnLoad = new DevExpress.XtraEditors.SimpleButton();
			this.pnlAppearances = new DevExpress.XtraEditors.PanelControl();
			this.pnlPreview = new DevExpress.XtraEditors.PanelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).BeginInit();
			this.gcAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAppearances)).BeginInit();
			this.pnlAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).BeginInit();
			this.pnlPreview.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.splMain, "splMain");
			resources.ApplyResources(this.pgMain, "pgMain");
			this.pnlControl.Controls.Add(this.btnSave);
			this.pnlControl.Controls.Add(this.btnLoad);
			resources.ApplyResources(this.pnlControl, "pnlControl");
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.pnlPreview);
			this.pnlMain.Controls.Add(this.scAppearance);
			this.pnlMain.Controls.Add(this.pnlAppearances);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			resources.ApplyResources(this.scAppearance, "scAppearance");
			this.scAppearance.Name = "scAppearance";
			this.scAppearance.TabStop = false;
			this.gcAppearances.Controls.Add(this.lbcAppearances);
			resources.ApplyResources(this.gcAppearances, "gcAppearances");
			this.gcAppearances.Name = "gcAppearances";
			this.lbcAppearances.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.lbcAppearances, "lbcAppearances");
			this.lbcAppearances.ItemHeight = 16;
			this.lbcAppearances.Name = "lbcAppearances";
			this.lbcAppearances.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbcAppearances.SortOrder = System.Windows.Forms.SortOrder.Ascending;
			this.lbcAppearances.SelectedIndexChanged += new System.EventHandler(this.lbcAppearances_SelectedIndexChanged);
			this.lbcAppearances.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbcAppearances_KeyDown);
			this.bpAppearances.AllowGlyphSkinning = true;
			this.bpAppearances.ButtonInterval = 3;
			this.bpAppearances.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
			new DevExpress.XtraEditors.ButtonsPanelControl.ButtonControl(resources.GetString("bpAppearances.Buttons"), ((System.Drawing.Image)(resources.GetObject("bpAppearances.Buttons1"))), ((int)(resources.GetObject("bpAppearances.Buttons2"))), DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, resources.GetString("bpAppearances.Buttons5"), ((bool)(resources.GetObject("bpAppearances.Buttons6"))), ((int)(resources.GetObject("bpAppearances.Buttons7"))), ((bool)(resources.GetObject("bpAppearances.Buttons8"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("bpAppearances.Buttons9"))), ((bool)(resources.GetObject("bpAppearances.Buttons10"))), ((bool)(resources.GetObject("bpAppearances.Buttons11"))), ((bool)(resources.GetObject("bpAppearances.Buttons12"))), ((object)(resources.GetObject("bpAppearances.Buttons13"))), resources.GetString("bpAppearances.Buttons14"), ((int)(resources.GetObject("bpAppearances.Buttons15"))), ((bool)(resources.GetObject("bpAppearances.Buttons16")))),
			new DevExpress.XtraEditors.ButtonsPanelControl.ButtonControl(resources.GetString("bpAppearances.Buttons17"), ((System.Drawing.Image)(resources.GetObject("bpAppearances.Buttons18"))), ((int)(resources.GetObject("bpAppearances.Buttons19"))), DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, resources.GetString("bpAppearances.Buttons22"), ((bool)(resources.GetObject("bpAppearances.Buttons23"))), ((int)(resources.GetObject("bpAppearances.Buttons24"))), ((bool)(resources.GetObject("bpAppearances.Buttons25"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("bpAppearances.Buttons26"))), ((bool)(resources.GetObject("bpAppearances.Buttons27"))), ((bool)(resources.GetObject("bpAppearances.Buttons28"))), ((bool)(resources.GetObject("bpAppearances.Buttons29"))), ((object)(resources.GetObject("bpAppearances.Buttons30"))), resources.GetString("bpAppearances.Buttons31"), ((int)(resources.GetObject("bpAppearances.Buttons32"))), ((bool)(resources.GetObject("bpAppearances.Buttons33")))),
			new DevExpress.XtraEditors.ButtonsPanelControl.ButtonControl(resources.GetString("bpAppearances.Buttons34"), ((System.Drawing.Image)(resources.GetObject("bpAppearances.Buttons35"))), ((int)(resources.GetObject("bpAppearances.Buttons36"))), DevExpress.XtraEditors.ButtonPanel.ImageLocation.Default, DevExpress.XtraBars.Docking2010.ButtonStyle.PushButton, resources.GetString("bpAppearances.Buttons39"), ((bool)(resources.GetObject("bpAppearances.Buttons40"))), ((int)(resources.GetObject("bpAppearances.Buttons41"))), ((bool)(resources.GetObject("bpAppearances.Buttons42"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("bpAppearances.Buttons43"))), ((bool)(resources.GetObject("bpAppearances.Buttons44"))), ((bool)(resources.GetObject("bpAppearances.Buttons45"))), ((bool)(resources.GetObject("bpAppearances.Buttons46"))), ((object)(resources.GetObject("bpAppearances.Buttons47"))), resources.GetString("bpAppearances.Buttons48"), ((int)(resources.GetObject("bpAppearances.Buttons49"))), ((bool)(resources.GetObject("bpAppearances.Buttons50"))))});
			this.bpAppearances.ContentAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.bpAppearances.Client = this.lbcAppearances;
			resources.ApplyResources(this.bpAppearances, "bpAppearances");
			this.bpAppearances.Name = "bpAppearances";
			this.bpAppearances.ButtonClick += new DevExpress.XtraBars.Docking2010.BaseButtonEventHandler(this.bpAppearances_ButtonClick);
			this.bpAppearances.Paint += new System.Windows.Forms.PaintEventHandler(this.bpAppearances_Paint);
			resources.ApplyResources(this.gcPreview, "gcPreview");
			this.gcPreview.Name = "gcPreview";
			this.btnSave.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnSave.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.Name = "btnSave";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			this.btnLoad.AllowGlyphSkinning = DevExpress.Utils.DefaultBoolean.True;
			this.btnLoad.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
			resources.ApplyResources(this.btnLoad, "btnLoad");
			this.btnLoad.Name = "btnLoad";
			this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
			this.pnlAppearances.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlAppearances.Controls.Add(this.gcAppearances);
			this.pnlAppearances.Controls.Add(this.bpAppearances);
			resources.ApplyResources(this.pnlAppearances, "pnlAppearances");
			this.pnlAppearances.Name = "pnlAppearances";
			this.pnlPreview.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlPreview.Controls.Add(this.gcPreview);
			resources.ApplyResources(this.pnlPreview, "pnlPreview");
			this.pnlPreview.Name = "pnlPreview";
			this.Name = "AppearancesDesignerBase";
			resources.ApplyResources(this, "$this");
			this.Load += new System.EventHandler(this.AppearancesDesigner_Load);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).EndInit();
			this.gcAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAppearances)).EndInit();
			this.pnlAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).EndInit();
			this.pnlPreview.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		protected override void InitImages() {
			base.InitImages();
			bpAppearances.Images = DesignerImages12;
			bpAppearances.Buttons[0].Properties.ImageIndex = 0;
			bpAppearances.Buttons[1].Properties.ImageIndex = 1;
			bpAppearances.Buttons[2].Properties.ImageIndex = 2;
			btnLoad.Image = DesignerImages16.Images[DesignerImages16LoadIndex];
			btnSave.Image = DesignerImages16.Images[DesignerImages16SaveIndex];
		}
		protected override bool AllowGlobalStore { get { return false; } }
		protected virtual void AppearancesDesigner_Load(object sender, System.EventArgs e) {
		}
		AppearancesPreview preview = null;
		public AppearancesPreview Preview { get { return preview; } }
		protected virtual Image AppearanceImage { get { return null; } }
		protected virtual bool CreateAppearancesPreview { get { return true; } }
		protected virtual XtraTabControl CreateTab() {
			return null;
		}
		DevExpress.XtraEditors.SplitterControl spHorz;
		protected virtual void CreateTabControl() {
			XtraTabControl tc = CreateTab();
			if(!CreateAppearancesPreview) return;
			spHorz = new DevExpress.XtraEditors.SplitterControl();
			preview = new AppearancesPreview(AppearanceImage);
			spHorz.Dock = DockStyle.Bottom;
			preview.Dock = DockStyle.Bottom;
			spHorz.Height = 4;
			tc.TabPages[0].Controls.Add(spHorz);
			tc.TabPages[0].Controls.Add(preview);
			ShowPreviewAppearance();
		}
		void miShow_Click(object sender, EventArgs e) {
			miShowAppearancePreview.Checked = !miShowAppearancePreview.Checked;
			ShowPreviewAppearance();
		}
		void ShowPreviewAppearance() {
			preview.Visible = spHorz.Visible = miShowAppearancePreview.Checked;
		}
		protected override void pgMain_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e) {
			if(Preview != null)
				Preview.Invalidate();
		}
		protected AppearanceObject GetAppearanceObjectByName(BaseAppearanceCollection appCollection, string name) {
			if(appCollection == null) return null;
			else return appCollection.GetAppearance(name);
		}
		protected void InitAppearanceList(BaseAppearanceCollection appCollection) {
			lbcAppearances.BeginUpdate();
			lbcAppearances.Items.Clear();
			if(appCollection == null) return;
			PropertyDescriptorCollection collection = TypeDescriptor.GetProperties(appCollection);
			for(int i = 0; i < collection.Count; i++)
				if(typeof(AppearanceObject).IsAssignableFrom(collection[i].PropertyType))
					lbcAppearances.Items.Add(collection[i].Name);
			lbcAppearances.SelectedIndex = 0;
			lbcAppearances.EndUpdate();
		}
		protected virtual void CreatePreviewControl() {
		}
		protected virtual void InitPreviewControl() {
		}
		#endregion
		#region Editing
		protected virtual void AddObject(ArrayList ret, string item) {
		}
		protected override object[] SelectedObjects {
			get {
				if(lbcAppearances.SelectedItem == null) return null;
				ArrayList ret = new ArrayList();
				for(int i = 0; i < lbcAppearances.SelectedIndices.Count; i++) {
					string selectedItem = lbcAppearances.GetItem(lbcAppearances.SelectedIndices[i]).ToString();
					AddObject(ret, selectedItem);		
				}		
				return ret.ToArray();
			}
		}
		protected bool selectUpdate = false;
		protected virtual void lbcAppearances_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(!selectUpdate) {
				pgMain.SelectedObjects = SelectedObjects;
				SetSelectedObject();
			}
		}
		protected virtual void SetSelectedObject() {}
		protected virtual void SelectAll() {
			lbcAppearances.BeginUpdate();
			selectUpdate = true;
			for(int i = 0; i < lbcAppearances.Items.Count; i++)
				lbcAppearances.SetSelected(i, true);
			pgMain.SelectedObjects = SelectedObjects;
			SetSelectedObject();
			selectUpdate = false;
			lbcAppearances.EndUpdate();
		}
		protected virtual void SetDefault() {
			foreach(object obj in SelectedObjects) {
				AppearanceObject app = obj as AppearanceObject;
				if(app != null) app.Reset();
			}
			pgMain.SelectedObjects = SelectedObjects;
			if(preview != null) preview.Invalidate();
		}
		protected virtual void beAppearances_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
		}
		protected virtual void lbcAppearances_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
		}
		#endregion
		#region Load&Save Layout
		private void btnLoad_Click(object sender, System.EventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "XML files(*.xml)|*.xml|All files|*.*";
			if(dlg.ShowDialog() == DialogResult.OK) {
				LoadAppearances(dlg.FileName);
			}
		}
		protected virtual void LoadAppearances(string name) {
		}
		protected virtual void SaveAppearances(string name) {
		}
		private void btnSave_Click(object sender, System.EventArgs e) {
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = "XML files(*.xml)|*.xml|All files|*.*";
			if(dlg.ShowDialog() == DialogResult.OK) 
				SaveAppearances(dlg.FileName);
		}
		#endregion
		protected virtual void bpAppearances_ButtonClick(object sender, XtraBars.Docking2010.BaseButtonEventArgs e) {
		}
		private void bpAppearances_Paint(object sender, PaintEventArgs e) {
			Color borderColor = DevExpress.Skins.CommonSkins.GetSkin(LookAndFeel)[DevExpress.Skins.CommonSkins.SkinTextBorder].Border.Left;
			e.Graphics.DrawLine(new Pen(borderColor), new Point(0, 0), new Point(0, bpAppearances.Bounds.Height - 1));
			e.Graphics.DrawLine(new Pen(borderColor), new Point(bpAppearances.Bounds.Width - 1, 0), new Point(bpAppearances.Bounds.Width - 1, bpAppearances.Bounds.Height - 1));
			e.Graphics.DrawLine(new Pen(borderColor), new Point(0, bpAppearances.Bounds.Height - 1), new Point(bpAppearances.Bounds.Width - 1, bpAppearances.Bounds.Height - 1));
		}
	}
}
