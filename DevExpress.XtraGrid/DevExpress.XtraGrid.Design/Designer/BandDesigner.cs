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
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Design;
using System.Collections.Generic;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)] 
	public class BandDesigner : DevExpress.XtraEditors.Designer.Utils.XtraPGFrame {
		private DevExpress.XtraGrid.GridControl gridControl1;
		private DevExpress.XtraGrid.Views.BandedGrid.BandedGridView bandedGridView1;
		private DevExpress.XtraGrid.Views.BandedGrid.GridBand gridBand1;
		private DevExpress.XtraEditors.SplitterControl splitter1;
		private DevExpress.XtraEditors.SimpleButton btAdd;
		private DevExpress.XtraEditors.SimpleButton btReset;
		private DevExpress.XtraEditors.SimpleButton chColumns;
		private DevExpress.XtraEditors.SimpleButton btDel;
		private DevExpress.XtraEditors.PanelControl pnlMProperties;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private DevExpress.XtraEditors.TextEdit txtCaption;
		private DevExpress.XtraEditors.ImageComboBoxEdit piImage;
		private DevExpress.XtraEditors.SpinEdit sRowCount;
		private System.Windows.Forms.Label lbImageHint;
		private System.Windows.Forms.ImageList imlImages;
		private DevExpress.XtraEditors.CheckEdit ceAutoWidth;
		protected override bool AllowGlobalStore { get { return false; } }
		public override void StoreLocalProperties(PropertyStore localStore) {
			localStore.AddProperty("BandPanel", pnlControl.Height);
			localStore.AddProperty("AutoWidth", ceAutoWidth.Checked);
			base.StoreLocalProperties(localStore);
		}
		public override void RestoreLocalProperties(PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			pnlControl.Height = localStore.RestoreIntProperty("BandPanel", pnlControl.Height);
			ceAutoWidth.Checked = localStore.RestoreBoolProperty("AutoWidth", ceAutoWidth.Checked);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BandDesigner));
			this.gridControl1 = new DevExpress.XtraGrid.GridControl();
			this.bandedGridView1 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
			this.gridBand1 = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
			this.splitter1 = new DevExpress.XtraEditors.SplitterControl();
			this.btAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btReset = new DevExpress.XtraEditors.SimpleButton();
			this.chColumns = new DevExpress.XtraEditors.SimpleButton();
			this.btDel = new DevExpress.XtraEditors.SimpleButton();
			this.pnlMProperties = new DevExpress.XtraEditors.PanelControl();
			this.lbImageHint = new System.Windows.Forms.Label();
			this.sRowCount = new DevExpress.XtraEditors.SpinEdit();
			this.piImage = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.imlImages = new System.Windows.Forms.ImageList();
			this.txtCaption = new DevExpress.XtraEditors.TextEdit();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.ceAutoWidth = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bandedGridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMProperties)).BeginInit();
			this.pnlMProperties.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sRowCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.piImage.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtCaption.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceAutoWidth.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.splMain, "splMain");
			resources.ApplyResources(this.pgMain, "pgMain");
			this.pnlControl.Controls.Add(this.gridControl1);
			resources.ApplyResources(this.pnlControl, "pnlControl");
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.ceAutoWidth);
			this.pnlMain.Controls.Add(this.btAdd);
			this.pnlMain.Controls.Add(this.btReset);
			this.pnlMain.Controls.Add(this.btDel);
			this.pnlMain.Controls.Add(this.chColumns);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			resources.ApplyResources(this.gridControl1, "gridControl1");
			this.gridControl1.MainView = this.bandedGridView1;
			this.gridControl1.Name = "gridControl1";
			this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.bandedGridView1});
			this.gridControl1.Load += new System.EventHandler(this.gridControl1_Load);
			this.gridControl1.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(this.gridControl1_ProcessGridKey);
			this.bandedGridView1.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[] {
			this.gridBand1});
			this.bandedGridView1.FixedLineWidth = 10;
			this.bandedGridView1.GridControl = this.gridControl1;
			this.bandedGridView1.Name = "bandedGridView1";
			resources.ApplyResources(this.gridBand1, "gridBand1");
			this.gridBand1.VisibleIndex = 0;
			resources.ApplyResources(this.splitter1, "splitter1");
			this.splitter1.Name = "splitter1";
			this.splitter1.TabStop = false;
			resources.ApplyResources(this.btAdd, "btAdd");
			this.btAdd.Name = "btAdd";
			this.btAdd.KeyUp += new System.Windows.Forms.KeyEventHandler(this.btAdd_KeyUp);
			this.btAdd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btAdd_MouseDown);
			this.btAdd.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btAdd_MouseMove);
			this.btAdd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btAdd_MouseUp);
			resources.ApplyResources(this.btReset, "btReset");
			this.btReset.Name = "btReset";
			this.btReset.Click += new System.EventHandler(this.btReset_Click);
			resources.ApplyResources(this.chColumns, "chColumns");
			this.chColumns.Name = "chColumns";
			this.chColumns.Click += new System.EventHandler(this.chColumns_Click);
			resources.ApplyResources(this.btDel, "btDel");
			this.btDel.Name = "btDel";
			this.btDel.Click += new System.EventHandler(this.btDel_Click);
			this.pnlMProperties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pnlMProperties.Controls.Add(this.lbImageHint);
			this.pnlMProperties.Controls.Add(this.sRowCount);
			this.pnlMProperties.Controls.Add(this.piImage);
			this.pnlMProperties.Controls.Add(this.txtCaption);
			this.pnlMProperties.Controls.Add(this.label4);
			this.pnlMProperties.Controls.Add(this.label2);
			this.pnlMProperties.Controls.Add(this.label1);
			resources.ApplyResources(this.pnlMProperties, "pnlMProperties");
			this.pnlMProperties.Name = "pnlMProperties";
			resources.ApplyResources(this.lbImageHint, "lbImageHint");
			this.lbImageHint.Name = "lbImageHint";
			resources.ApplyResources(this.sRowCount, "sRowCount");
			this.sRowCount.Name = "sRowCount";
			this.sRowCount.Properties.Appearance.Options.UseTextOptions = true;
			this.sRowCount.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.sRowCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.sRowCount.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.sRowCount.Properties.IsFloatValue = false;
			this.sRowCount.Properties.Mask.EditMask = resources.GetString("sRowCount.Properties.Mask.EditMask");
			this.sRowCount.Properties.MaxValue = new decimal(new int[] {
			10,
			0,
			0,
			0});
			this.sRowCount.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.sRowCount.EditValueChanged += new System.EventHandler(this.sRowCount_ValueChanged);
			resources.ApplyResources(this.piImage, "piImage");
			this.piImage.Name = "piImage";
			this.piImage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("piImage.Properties.Buttons"))))});
			this.piImage.Properties.SmallImages = this.imlImages;
			this.piImage.EditValueChanged += new System.EventHandler(this.piImage_ValueChanged);
			this.imlImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlImages.ImageStream")));
			this.imlImages.TransparentColor = System.Drawing.Color.Magenta;
			this.imlImages.Images.SetKeyName(0, "");
			resources.ApplyResources(this.txtCaption, "txtCaption");
			this.txtCaption.Name = "txtCaption";
			this.txtCaption.EditValueChanged += new System.EventHandler(this.txtCaption_ValueChanged);
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			resources.ApplyResources(this.ceAutoWidth, "ceAutoWidth");
			this.ceAutoWidth.Name = "ceAutoWidth";
			this.ceAutoWidth.Properties.Caption = resources.GetString("ceAutoWidth.Properties.Caption");
			this.ceAutoWidth.CheckedChanged += new System.EventHandler(this.ceAutoWidth_CheckedChanged);
			this.Controls.Add(this.pnlMProperties);
			this.Controls.Add(this.splitter1);
			this.Name = "BandDesigner";
			resources.ApplyResources(this, "$this");
			this.Controls.SetChildIndex(this.lbCaption, 0);
			this.Controls.SetChildIndex(this.pnlControl, 0);
			this.Controls.SetChildIndex(this.splitter1, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			this.Controls.SetChildIndex(this.pgMain, 0);
			this.Controls.SetChildIndex(this.pnlMProperties, 0);
			this.Controls.SetChildIndex(this.horzSplitter, 0);
			this.Controls.SetChildIndex(this.splMain, 0);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bandedGridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMProperties)).EndInit();
			this.pnlMProperties.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sRowCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.piImage.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtCaption.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceAutoWidth.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		protected override void InitImages() {
			base.InitImages();
			btReset.Image = DesignerImages16.Images[DesignerImages16ResetIndex];
		}
		BandedGridView EditingView { get { return EditingObject as BandedGridView;} }
		protected override string DescriptionText { get { return DevExpress.XtraGrid.Design.Properties.Resources.BandDesignerDescription; } }
		public BandDesigner() : base(11) {
			InitializeComponent();
			pgMain.BringToFront();
			try {
				btAdd.Font = new Font(btAdd.Font, FontStyle.Underline);
				btAdd.BackColor = ColorUtils.OffsetColor(btAdd.BackColor, -10, -10, -10);
			} catch {}
		}
		private bool layoutChanged;
		protected override void Dispose(bool disposing) {
			(View as IGridDesignTime).ForceDesignMode = false;
			View.DestroyCustomization();
			View.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.bandedGridView_MouseDown);
			View.CustomDrawEmptyForeground -= new DevExpress.XtraGrid.Views.Base.CustomDrawEventHandler(this.bandedGridView_CustomDrawEmptyForeground);
			View.CustomDrawBandHeader -= new DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventHandler(this.bandedGridView_CustomDrawBandHeader);
			View.DragObjectDrop -= new DevExpress.XtraGrid.Views.Base.DragObjectDropEventHandler(this.bandedGridView_DragObjectDrop);
			View.ShowCustomizationForm -= new System.EventHandler(this.bandedGridView_ShowCustomizationForm);
			View.HideCustomizationForm -= new System.EventHandler(this.bandedGridView_HideCustomizationForm);
			View.Layout -= new System.EventHandler(this.bandedGridView_Layout);
			if(layoutChanged) SetLayout();
			base.Dispose(disposing);
		}
		protected virtual bool ShowTabControl { get { return EmbeddedFrameInit == null; } }
		public override void InitComponent() {
			InitView();
			if(ShowTabControl) {
				DevExpress.XtraTab.XtraTabControl tc = FramesUtils.CreateTabProperty(this, new Control[] {pgMain, null, pnlMProperties}, new string[] {DevExpress.XtraGrid.Design.Properties.Resources.BandPropertiesCaption, 
					DevExpress.XtraGrid.Design.Properties.Resources.BandOptionsCaption, DevExpress.XtraGrid.Design.Properties.Resources.MainPropertiesCaption});
				tc.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(changeTabPage);
			}
			AddMainProperties();
			SetColumnSelectorCaption(false);
			SetSelectedBand(null);
			SetupTimer();			
		}
		protected virtual void  SetupTimer() {
			if(EmbeddedFrameInit != null) return;
			Timer tmr = new Timer();
			tmr.Interval = 300;
			tmr.Tick += new EventHandler(OnTick);
			tmr.Start();
		}
		void OnTick(object sender, EventArgs e) {
			Timer tmr = sender as Timer;
			if(needShowCustomizationForm && View != null) View.ColumnsCustomization();
			tmr.Stop();
			tmr.Dispose();
		}
		private bool showingColumnsSelector = false;
		private void SetColumnSelectorCaption(bool showing) {
			showingColumnsSelector = showing;
			chColumns.Text = showing ? DevExpress.XtraGrid.Design.Properties.Resources.HideColumnsSelectorCaption : DevExpress.XtraGrid.Design.Properties.Resources.ShowColumnsSelectorCaption;	
		}
		private BandedGridView View { get { return gridControl1.MainView as BandedGridView;}}
		bool needShowCustomizationForm = false;
		private void InitView() {
			DevExpress.XtraGrid.Views.Base.BaseView oldView = gridControl1.MainView;
			gridControl1.MainView = gridControl1.CreateView(EditingView.BaseInfo.ViewName);
			if(oldView != null) oldView.Dispose();
			DevExpress.XtraGrid.Design.GridAssign.GridControlAssign(EditingView.GridControl, EditingView, gridControl1, false, true);
			(View as IGridDesignTime).ForceDesignMode = true;
			View.MouseDown += new System.Windows.Forms.MouseEventHandler(this.bandedGridView_MouseDown);
			View.CustomDrawEmptyForeground += new DevExpress.XtraGrid.Views.Base.CustomDrawEventHandler(this.bandedGridView_CustomDrawEmptyForeground);
			View.CustomDrawBandHeader += new DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventHandler(this.bandedGridView_CustomDrawBandHeader);
			View.DragObjectDrop += new DevExpress.XtraGrid.Views.Base.DragObjectDropEventHandler(this.bandedGridView_DragObjectDrop);
			View.ShowCustomizationForm += new System.EventHandler(this.bandedGridView_ShowCustomizationForm);
			View.HideCustomizationForm += new System.EventHandler(this.bandedGridView_HideCustomizationForm);
			View.Layout += new System.EventHandler(this.bandedGridView_Layout);
			new XtraGridBandsCopier(EditingView, gridControl1.MainView as BandedGridView).Copy();
			foreach(BandedGridColumn col in View.Columns) 
				if(col.OwnerBand == null && col.OptionsColumn.ShowInCustomizationForm) {
					needShowCustomizationForm = true;
					break;
				}
			SetRequiredOptions();
		}
		private bool showGroupPanel, allowChangeBandParent, allowChangeColumnParent, useTabKey, autoWidth, showAutoFilterRow, showBands, showColumns, allowBandResizing, allowBandMoving;
		NewItemRowPosition itemRowPosition;
		ArrayList arrColumns = null;
		private void SetRequiredOptions() {
			arrColumns = new ArrayList();
			showGroupPanel = View.OptionsView.ShowGroupPanel;
			allowChangeBandParent = View.OptionsCustomization.AllowChangeBandParent;
			allowChangeColumnParent = View.OptionsCustomization.AllowChangeColumnParent;
			useTabKey = View.OptionsNavigation.UseTabKey;
			autoWidth = View.OptionsView.ColumnAutoWidth;
			showAutoFilterRow = View.OptionsView.ShowAutoFilterRow;
			itemRowPosition = View.OptionsView.NewItemRowPosition;
			showBands = View.OptionsView.ShowBands;
			showColumns = View.OptionsView.ShowColumnHeaders;
			allowBandResizing = View.OptionsCustomization.AllowBandResizing;
			allowBandMoving = View.OptionsCustomization.AllowBandMoving;
			View.BeginUpdate();
			View.OptionsView.ShowGroupPanel = false;
			View.OptionsCustomization.AllowChangeBandParent = true;
			View.OptionsCustomization.AllowChangeColumnParent = true;
			View.OptionsNavigation.UseTabKey = false;
			View.OptionsView.ColumnAutoWidth = ceAutoWidth.Checked;
			View.OptionsView.ShowAutoFilterRow = false;
			View.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
			View.OptionsView.ShowBands = true;
			View.OptionsView.ShowColumnHeaders = true;
			View.OptionsCustomization.AllowBandResizing = true;
			View.OptionsCustomization.AllowBandMoving = true;
			foreach(GridColumn col in View.Columns) 
				if(!col.OptionsColumn.ShowInCustomizationForm) {
					arrColumns.Add(col);
					col.OptionsColumn.ShowInCustomizationForm = true;
				}
			View.EndUpdate();
		}
		private void GetRequiredOptions() {
			View.OptionsView.ShowGroupPanel = showGroupPanel;
			View.OptionsCustomization.AllowChangeBandParent = allowChangeBandParent;
			View.OptionsCustomization.AllowChangeColumnParent = allowChangeColumnParent;
			View.OptionsNavigation.UseTabKey = useTabKey;
			View.OptionsView.ColumnAutoWidth = autoWidth;
			View.OptionsView.ShowAutoFilterRow = showAutoFilterRow;
			View.OptionsView.NewItemRowPosition = itemRowPosition;
			View.OptionsView.ShowBands = showBands;
			View.OptionsView.ShowColumnHeaders = showColumns;
			View.OptionsCustomization.AllowBandResizing = allowBandResizing;
			View.OptionsCustomization.AllowBandMoving = allowBandMoving;
			if(arrColumns == null || arrColumns.Count == 0) return;
			foreach(GridColumn col in arrColumns) 
				col.OptionsColumn.ShowInCustomizationForm = false;
			arrColumns = null;
		}
		#endregion
		#region Selecting band
		protected void SetSelectedBand(GridBand band) {
			GridBandCollection collection = View.Bands;
			if(band != null && band.ParentBand != null) {
				collection = band.ParentBand.Children;
				if(collection.Count <= 1) {
					ShowBandProperties(band.ParentBand);
					return;
				}
			} 
			int index = -1; 
			if(band == null) {
				if(collection.Count > 0) index = 0;
			} else {
				if(band.Index > 0) index = band.Index - 1;
				if(band.Index == 0 && collection.Count > 1) index = 1;
			}
			if(index > -1) ShowBandProperties(collection[index]);
			else ShowBandProperties(null);
		}
		#endregion
		#region Bands recursion
		private bool BandByCaption(GridBand band, string caption) {
			GridBandCollection bndCollections;
			if(band == null) 
				bndCollections = View.Bands;
			else
				bndCollections = band.Children;
			foreach(GridBand bnd in bndCollections) {
				if(bnd.Caption == caption) return true;
				if(BandByCaption(bnd, caption)) return true;
			}
			return false;
		}
		bool IsBandNameExist(string name, GridBandCollection bands, GridBand band) {
			if(name == null) return false;
			foreach(GridBand aBand in bands) {
				if(aBand.Equals(band)) continue;
				if(aBand.Name.ToLowerInvariant().Equals(name.ToLowerInvariant())) return true;
				if(aBand.HasChildren && IsBandNameExist(name, aBand.Children, band)) return true;
			}
			return false;
		}
		bool IsBandNameExist(string name, GridBandCollection bands) {
			return IsBandNameExist(name, bands, null);
		}
		#endregion
		#region Main properties
		private bool mainPropertiesUpdate = false;
		private void AddMainProperties() {
			mainPropertiesUpdate = true;
			piImage.Visible = View.Images != null;
			lbImageHint.Visible = View.Images == null;
			piImage.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem("None", -1, 0));
			if(View.Images != null) {
				for(int i = 0; i < ImageCollection.GetImageListImageCount(View.Images); i++) {
					imlImages.Images.Add(ImageCollection.GetImageListImage(View.Images, i));
					piImage.Properties.Items.Add(new DevExpress.XtraEditors.Controls.ImageComboBoxItem(i.ToString(), i, i + 1));
				}
			}
			piImage.EditValue = -1;
			mainPropertiesUpdate = false;
		}
		private void InitMainProperties(GridBand band) {
			mainPropertiesUpdate = true;
			if(band != null) {
				txtCaption.Text = band.Caption;
				piImage.EditValue = band.ImageIndex;
				sRowCount.Text = band.RowCount.ToString();
			}
			SetMainPropertiesEnabled(band != null);
			mainPropertiesUpdate = false;
		}
		private void SetMainPropertiesEnabled(bool f) {
			pnlMProperties.Enabled = txtCaption.Properties.Enabled =
				piImage.Properties.Enabled = sRowCount.Properties.Enabled = f;
			if(!f) {
				txtCaption.Text = "";
				sRowCount.EditValue = 1;
				piImage.EditValue = -1;
			}
		}
		private GridBand SelectedBand { get {return selectedBand as GridBand;}}
		private void txtCaption_ValueChanged(object sender, System.EventArgs e) {
			if(mainPropertiesUpdate || selectedBand == null) return;
			SelectedBand.Caption = txtCaption.Text;
		}
		private void sRowCount_ValueChanged(object sender, System.EventArgs e) {
			if(mainPropertiesUpdate || selectedBand == null) return;
			SelectedBand.RowCount = Convert.ToInt32(sRowCount.Value);
		}	
		private void piImage_ValueChanged(object sender, System.EventArgs e) {
			if(mainPropertiesUpdate || selectedBand == null) return;
			SelectedBand.ImageIndex = Convert.ToInt32(piImage.EditValue);
		}
		#endregion
		#region Band options
		int selectedTabPage = 0;
		private void changeTabPage(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e) {
			DevExpress.XtraTab.XtraTabControl tc = sender as DevExpress.XtraTab.XtraTabControl;
			if(tc.SelectedTabPageIndex == 2) InitMainProperties(SelectedBand);
			else {
				selectedTabPage = tc.SelectedTabPageIndex;
				ShowBandProperties(SelectedBand);
				e.Page.Controls.Add(pgMain);
				pgMain.Refresh();
			}
		}
		#endregion
		#region Properties
		private object selectedBand = null;
		protected virtual void ShowBandProperties(GridBand band) {
			selectedBand = band;
			btDel.Enabled = selectedBand != null;
			InitMainProperties(SelectedBand); 
			RefreshPropertyGrid();
		}
		protected override object SelectedObject {
			get {
				if(selectedTabPage == 0) {
					if(selectedBand == null) return null;
					return new CustomNameObject(selectedBand as GridBand);
				}
				else return selectedBand == null ? null : SelectedBand.OptionsBand;
			}
		}
		#endregion
		#region Editing
		private void btDel_Click(object sender, System.EventArgs e) {
			if(selectedBand != null) {
				GridBand band = selectedBand as GridBand;
				SetSelectedBand(band);
				band.Dispose();
			} else ShowBandProperties(null);
		}
		public void SetLayout() {
			gridControl1.BeginUpdate();
			GetRequiredOptions();
			new XtraGridBandsCopier(gridControl1.MainView as BandedGridView, EditingView).Copy();
			SetRequiredOptions();
			gridControl1.EndUpdate();
		}
		private void btReset_Click(object sender, System.EventArgs e) {
			InitView();
			SetSelectedBand(null);
			layoutChanged = false;
		}
		private void chColumns_Click(object sender, System.EventArgs e) {
			if(chColumnsUpdate) return;
			if(!showingColumnsSelector) View.ColumnsCustomization();
			else View.DestroyCustomization();
		}
		private void ceAutoWidth_CheckedChanged(object sender, System.EventArgs e) {
			View.OptionsView.ColumnAutoWidth = ceAutoWidth.Checked;
		}
		#endregion
		#region BandView events
		private void bandedGridView_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			DevExpress.XtraGrid.Views.BandedGrid.ViewInfo.BandedGridHitInfo bghi = ((BandedGridView)sender).CalcHitInfo(new Point(e.X, e.Y));
			if(bghi.Band != null) {
				ShowBandProperties(bghi.Band);
				View.InvalidateBandHeader(null); 
			}
		}
		private void bandedGridView_CustomDrawEmptyForeground(object sender, DevExpress.XtraGrid.Views.Base.CustomDrawEventArgs e) {
			StringFormat sf = new StringFormat();
			sf.Alignment = sf.LineAlignment = StringAlignment.Near;
			e.Graphics.DrawString(DevExpress.XtraGrid.Design.Properties.Resources.ClickBandCaption, e.Appearance.Font, SystemBrushes.ControlText, e.Bounds, sf);
		}
		private void bandedGridView_CustomDrawBandHeader(object sender, DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventArgs e) {
			if(e.Info.Band != null && e.Info.Band.Equals(selectedBand)) {
				e.Handled = true;
				e.Painter.DrawObject(e.Info);
				Rectangle r = e.Bounds;
				e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 255, 255)), r);
				r.Height--; r.Width--;
			}
		}
		private void gridControl1_ProcessGridKey(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Delete) {
				btDel_Click(this, EventArgs.Empty);
			}
		}
		private void bandedGridView_DragObjectDrop(object sender, DevExpress.XtraGrid.Views.Base.DragObjectDropEventArgs e) {
			if(selectedBand != null && ((GridBand)selectedBand).View == null) {
				(selectedBand as GridBand).Dispose();
				ShowBandProperties(null);
			}
			SetButtonCaption(false);
		}
		private bool chColumnsUpdate = false;
		private void bandedGridView_ShowCustomizationForm(object sender, System.EventArgs e) {
			chColumnsUpdate = true;
			BandedGridView view = sender as BandedGridView;
			Rectangle r = chColumns.RectangleToScreen(chColumns.ClientRectangle);
			view.CustomizationFormBounds = new Rectangle(r.X, r.Y + r.Height, 0, 0);
			SetColumnSelectorCaption(true);
			chColumnsUpdate = false;
		}
		private void bandedGridView_HideCustomizationForm(object sender, System.EventArgs e) {
			chColumnsUpdate = true;
			SetColumnSelectorCaption(false);
			chColumnsUpdate = false;
		}
		private void bandedGridView_Layout(object sender, System.EventArgs e) {
			layoutChanged = true;
		}
		#endregion
		#region Add bands
		private void btAdd_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				if(Math.Abs(e.X - clickPoint.X) > 5 || Math.Abs(e.Y - clickPoint.Y) > 5) {
					add = false;
					IGridDesignTime dt = View as IGridDesignTime;
					if(dt == null || !dt.Enabled) return;
					DragDropEffects effect = dt.DoDragDrop(AddBand());
				}
			}
		}
		private bool add = false;
		private Point clickPoint = Point.Empty;
		private void btAdd_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e) {
			if(e.Button == MouseButtons.Left) {
				clickPoint = new Point(e.X, e.Y);
				SetButtonCaption(true);
				add = true;
			}
		}
		private GridBand AddBand() {
			GridBand band = View.Bands.CreateBand();
			band.Caption = band.Name = GetComponentName("gridBand");
			ShowBandProperties(band);
			return band;
		}
		private void btAdd_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			SetButtonCaption(false);
			if(add) {
				View.Bands.Add(AddBand());
			}
			add = false;
		}
		private void SetButtonCaption(bool click) {
			btAdd.Text = click ? DevExpress.XtraGrid.Design.Properties.Resources.DragBandCaption : DevExpress.XtraGrid.Design.Properties.Resources.AddNewBandCaption;
		}
		private void btAdd_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(e.KeyCode == Keys.Space) View.Bands.Add(AddBand());
		}
		#endregion
		#region Grid events	
		private void gridControl1_Load(object sender, System.EventArgs e) {
			layoutChanged = false;
		}
		#endregion
		protected override void OnPropertyValueChanged(PropertyValueChangedEventArgs e) {
			base.OnPropertyValueChanged(e);
			if(e.ChangedItem.Label == "Name") {
				string value = GetCorrectNameValue(e.ChangedItem.Value);
				if(IsNameExist(value) || IsBandNameExist(value, View.Bands, GetSelectedParent())) {
					MessageBox.Show(string.Format(DevExpress.XtraGrid.Design.Properties.Resources.NameWarning, value), 
						DevExpress.XtraGrid.Design.Properties.Resources.PropertyValueWarning, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					SetName(string.Format("{0}", e.OldValue));
				}
				else {
					if(!value.Equals(e.ChangedItem.Value))
						SetName(value);
					layoutChanged = true;
				}
			}
		}
		string GetCorrectNameValue(object val) {
			if(val == null) return GetComponentName("gridBand");
			return val.ToString().Replace(" ", "");
		}
		GridBand GetSelectedParent() {
			CustomNameObject obj = pgMain.SelectedObject as CustomNameObject;
			if(obj == null) return null;
			return obj.Object as GridBand;
		}
		void SetName(string name) {
			GridBand band = GetSelectedParent();
			if(band == null) return;
			band.Name = name;
		}
		bool IsNameExist(string name) {
			if(EditingView.Site == null || name == null) return false;
			foreach(IComponent cmp in EditingView.Site.Container.Components)
				if(name.ToLowerInvariant().Equals(cmp.Site.Name.ToLowerInvariant())) return true;
			return false;
		}
		string GetComponentName(string typeName) {
			for(int i = 1; i < 10000; i++ ) {
				string name = string.Format("{0}{1}", typeName, i);
				if(!IsNameExist(name) && !IsBandNameExist(name, View.Bands)) return name;
			}
			return string.Empty;
		}
	}
	#region Bands Copier
	public class XtraGridBandsCopier {
		BandedGridView source;
		BandedGridView destination;
		public XtraGridBandsCopier(BandedGridView source, BandedGridView destination) {
			this.source = source;
			this.destination = destination;
		}
		public BandedGridView Source { get { return source; } }
		public BandedGridView Destination { get { return destination; } }
		public void Copy() {
			if(Source == null || Destination == null) return;
			Destination.BeginUpdate();
			try {
				DeleteBands();
				AssignBands();
			}
			finally {
				Destination.EndUpdate();
			}
		}
		void DeleteBands() {
			List<GridBand> bands = GetBands(Destination);
			Destination.Bands.Clear();
			for(int n = bands.Count - 1; n >= 0; n--) {
				bands[n].Dispose();
			}
		}
		List<GridBand> GetBands(BandedGridView view) {
			List<GridBand> res = new List<GridBand>();
			return GetBandsCore(view.Bands, res);
			}
		List<GridBand> GetBandsCore(GridBandCollection collection, List<GridBand> res) {
			foreach(GridBand band in collection) {
				res.Add(band);
				if(band.HasChildren) GetBandsCore(band.Children, res);
				}
			return res;
				}
		GridBand FindBand(BandedGridView view, string name) { return FindBandCore(view.Bands, name); }
		GridBand FindBandCore(GridBandCollection collection, string name) {
			GridBand res = collection[name];
			if(res != null) return res;
			for(int n = 0; n < collection.Count; n++) {
				res = collection[n];
				if(!res.HasChildren) continue;
				res = FindBandCore(res.Children, name);
				if(res != null) return res;
			}
			return null;
		}
		void AssignBands() {
			MethodInfo methodInfo = Source.GetType().GetMethod("AssignColumns", BindingFlags.NonPublic | BindingFlags.Instance);
			if(methodInfo != null) 
				methodInfo.Invoke(Destination, new object[] {Source, true});
		}
	}
	#endregion
	#region Fake object
	[ToolboxItem(false)]
	public class CustomNameObject : ICustomTypeDescriptor {
		object _object;
		public CustomNameObject(object band) {
			this._object = band;
		}
		public object Object { get { return _object; } }
		System.ComponentModel.AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return TypeDescriptor.GetAttributes(Object.GetType());
		}
		string ICustomTypeDescriptor.GetClassName() {
			return Object.GetType().Name;
		}
		string ICustomTypeDescriptor.GetComponentName() {
			return Object.GetType().FullName;
		}
		TypeConverter ICustomTypeDescriptor.GetConverter() {
			return TypeDescriptor.GetConverter(Object);
		}
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() {
			return TypeDescriptor.GetDefaultEvent(Object);
		}
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() {
			return TypeDescriptor.GetDefaultProperty(Object);
		}
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) {
			return TypeDescriptor.GetEditor(typeof(Object), editorBaseType);
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() {
			return EventDescriptorCollection.Empty;
		}
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attribute) {
			return EventDescriptorCollection.Empty;
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			PropertyDescriptorCollection sourcePdc = TypeDescriptor.GetProperties(Object);
			ArrayList list = new ArrayList(sourcePdc);
			PropertyDescriptor name = sourcePdc["Name"];
			list.Remove(name);
			list.Add(TypeDescriptor.CreateProperty(typeof(GridBand), name, new BrowsableAttribute(true), new CategoryAttribute("Name")));
			PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(list.ToArray(typeof(PropertyDescriptor)) as PropertyDescriptor[]);
			return pdc;
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return ((ICustomTypeDescriptor)this).GetProperties();
		}
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) {
			return Object;
		}
	}
	#endregion
}
