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
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.Layout;
using DevExpress.XtraGrid.Columns;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.WinExplorer;
using DevExpress.XtraGrid.WinExplorer;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class AppearancesDesigner : DevExpress.XtraEditors.Frames.AppearancesDesignerBase {
		private DevExpress.XtraEditors.PanelControl pcPaintStyle;
		private DevExpress.XtraEditors.ComboBoxEdit cbePaintStyle;
		private System.Windows.Forms.Label label2;
		private DevExpress.XtraEditors.CheckEdit ceEven;
		private DevExpress.XtraEditors.CheckEdit ceOdd;
		private DevExpress.XtraEditors.CheckEdit ceFocusedCell;
		private DevExpress.XtraEditors.CheckEdit ceFocusedRow;
		protected DevExpress.XtraEditors.Designer.Utils.FilterButtonPanel bpMode;
		private System.ComponentModel.Container components = null;
		public AppearancesDesigner() {
			InitializeComponent();
			pgMain.BringToFront();
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(previewGrid != null) {
					previewGrid.AppearanceCollectionChanged -= new EventHandler(AppearanceCollectionChanged);
					previewGrid.Dispose();
					previewGrid = null;
				}
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		public override void StoreLocalProperties(PropertyStore localStore) {
			localStore.AddProperty("AppearancePanel", gcAppearances.Width);
			base.StoreLocalProperties(localStore);
		}
		public override void RestoreLocalProperties(PropertyStore localStore) {
			base.RestoreLocalProperties(localStore);
			gcAppearances.Width = localStore.RestoreIntProperty("AppearancePanel", gcAppearances.Width);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AppearancesDesigner));
			this.pcPaintStyle = new DevExpress.XtraEditors.PanelControl();
			this.ceFocusedRow = new DevExpress.XtraEditors.CheckEdit();
			this.ceFocusedCell = new DevExpress.XtraEditors.CheckEdit();
			this.ceOdd = new DevExpress.XtraEditors.CheckEdit();
			this.ceEven = new DevExpress.XtraEditors.CheckEdit();
			this.cbePaintStyle = new DevExpress.XtraEditors.ComboBoxEdit();
			this.label2 = new System.Windows.Forms.Label();
			this.bpMode = new DevExpress.XtraEditors.Designer.Utils.FilterButtonPanel();
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).BeginInit();
			this.gcAppearances.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAppearances)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).BeginInit();
			this.pnlPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pcPaintStyle)).BeginInit();
			this.pcPaintStyle.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ceFocusedRow.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceFocusedCell.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceOdd.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceEven.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbePaintStyle.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.scAppearance, "scAppearance");
			resources.ApplyResources(this.gcAppearances, "gcAppearances");
			resources.ApplyResources(this.gcPreview, "gcPreview");
			resources.ApplyResources(this.lbcAppearances, "lbcAppearances");
			resources.ApplyResources(this.bpAppearances, "bpAppearances");
			resources.ApplyResources(this.pnlAppearances, "pnlAppearances");
			this.pnlPreview.Controls.Add(this.bpMode);
			resources.ApplyResources(this.pnlPreview, "pnlPreview");
			this.pnlPreview.Controls.SetChildIndex(this.bpMode, 0);
			this.pnlPreview.Controls.SetChildIndex(this.gcPreview, 0);
			resources.ApplyResources(this.splMain, "splMain");
			resources.ApplyResources(this.pgMain, "pgMain");
			resources.ApplyResources(this.pnlControl, "pnlControl");
			this.lbCaption.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("lbCaption.Appearance.Font")));
			resources.ApplyResources(this.lbCaption, "lbCaption");
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			this.pcPaintStyle.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.pcPaintStyle.Controls.Add(this.ceFocusedRow);
			this.pcPaintStyle.Controls.Add(this.ceFocusedCell);
			this.pcPaintStyle.Controls.Add(this.ceOdd);
			this.pcPaintStyle.Controls.Add(this.ceEven);
			this.pcPaintStyle.Controls.Add(this.cbePaintStyle);
			this.pcPaintStyle.Controls.Add(this.label2);
			resources.ApplyResources(this.pcPaintStyle, "pcPaintStyle");
			this.pcPaintStyle.Name = "pcPaintStyle";
			this.pcPaintStyle.Resize += new System.EventHandler(this.pcPaintStyle_Resize);
			resources.ApplyResources(this.ceFocusedRow, "ceFocusedRow");
			this.ceFocusedRow.Name = "ceFocusedRow";
			this.ceFocusedRow.Properties.Caption = resources.GetString("ceFocusedRow.Properties.Caption");
			this.ceFocusedRow.CheckedChanged += new System.EventHandler(this.ceFocusedRow_CheckedChanged);
			resources.ApplyResources(this.ceFocusedCell, "ceFocusedCell");
			this.ceFocusedCell.Name = "ceFocusedCell";
			this.ceFocusedCell.Properties.Caption = resources.GetString("ceFocusedCell.Properties.Caption");
			this.ceFocusedCell.CheckedChanged += new System.EventHandler(this.ceFocusedCell_CheckedChanged);
			resources.ApplyResources(this.ceOdd, "ceOdd");
			this.ceOdd.Name = "ceOdd";
			this.ceOdd.Properties.Caption = resources.GetString("ceOdd.Properties.Caption");
			this.ceOdd.CheckedChanged += new System.EventHandler(this.ceOdd_CheckedChanged);
			resources.ApplyResources(this.ceEven, "ceEven");
			this.ceEven.Name = "ceEven";
			this.ceEven.Properties.Caption = resources.GetString("ceEven.Properties.Caption");
			this.ceEven.CheckedChanged += new System.EventHandler(this.ceEven_CheckedChanged);
			resources.ApplyResources(this.cbePaintStyle, "cbePaintStyle");
			this.cbePaintStyle.Name = "cbePaintStyle";
			this.cbePaintStyle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbePaintStyle.Properties.Buttons"))))});
			this.cbePaintStyle.Properties.Items.AddRange(new object[] {
			resources.GetString("cbePaintStyle.Properties.Items")});
			this.cbePaintStyle.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbePaintStyle.SelectedIndexChanged += new System.EventHandler(this.cbePaintStyle_SelectedIndexChanged);
			this.label2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			this.bpMode.AllowGlyphSkinning = true;
			this.bpMode.ButtonInterval = 3;
			this.bpMode.Buttons.AddRange(new DevExpress.XtraEditors.ButtonPanel.IBaseButton[] {
			new DevExpress.XtraEditors.ButtonsPanelControl.ButtonControl(resources.GetString("bpMode.Buttons"), ((System.Drawing.Image)(resources.GetObject("bpMode.Buttons1"))), ((int)(resources.GetObject("bpMode.Buttons2"))), ((DevExpress.XtraEditors.ButtonPanel.ImageLocation)(resources.GetObject("bpMode.Buttons3"))), ((DevExpress.XtraBars.Docking2010.ButtonStyle)(resources.GetObject("bpMode.Buttons4"))), resources.GetString("bpMode.Buttons5"), ((bool)(resources.GetObject("bpMode.Buttons6"))), ((int)(resources.GetObject("bpMode.Buttons7"))), ((bool)(resources.GetObject("bpMode.Buttons8"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("bpMode.Buttons9"))), ((bool)(resources.GetObject("bpMode.Buttons10"))), ((bool)(resources.GetObject("bpMode.Buttons11"))), ((bool)(resources.GetObject("bpMode.Buttons12"))), ((object)(resources.GetObject("bpMode.Buttons13"))), resources.GetString("bpMode.Buttons14"), ((int)(resources.GetObject("bpMode.Buttons15"))), ((bool)(resources.GetObject("bpMode.Buttons16")))),
			new DevExpress.XtraEditors.ButtonsPanelControl.ButtonControl(resources.GetString("bpMode.Buttons17"), ((System.Drawing.Image)(resources.GetObject("bpMode.Buttons18"))), ((int)(resources.GetObject("bpMode.Buttons19"))), ((DevExpress.XtraEditors.ButtonPanel.ImageLocation)(resources.GetObject("bpMode.Buttons20"))), ((DevExpress.XtraBars.Docking2010.ButtonStyle)(resources.GetObject("bpMode.Buttons21"))), resources.GetString("bpMode.Buttons22"), ((bool)(resources.GetObject("bpMode.Buttons23"))), ((int)(resources.GetObject("bpMode.Buttons24"))), ((bool)(resources.GetObject("bpMode.Buttons25"))), ((DevExpress.Utils.SuperToolTip)(resources.GetObject("bpMode.Buttons26"))), ((bool)(resources.GetObject("bpMode.Buttons27"))), ((bool)(resources.GetObject("bpMode.Buttons28"))), ((bool)(resources.GetObject("bpMode.Buttons29"))), ((object)(resources.GetObject("bpMode.Buttons30"))), resources.GetString("bpMode.Buttons31"), ((int)(resources.GetObject("bpMode.Buttons32"))), ((bool)(resources.GetObject("bpMode.Buttons33"))))});
			this.bpMode.ContentAlignment = System.Drawing.ContentAlignment.MiddleLeft;
			this.bpMode.SearchControlVisible = false;
			resources.ApplyResources(this.bpMode, "bpMode");
			this.bpMode.Name = "bpMode";
			this.bpMode.ButtonChecked += new DevExpress.XtraBars.Docking2010.BaseButtonEventHandler(this.bpMode_ButtonChecked);
			this.bpMode.Paint += new System.Windows.Forms.PaintEventHandler(this.bpMode_Paint);
			this.Controls.Add(this.pcPaintStyle);
			this.Name = "AppearancesDesigner";
			resources.ApplyResources(this, "$this");
			this.Controls.SetChildIndex(this.lbCaption, 0);
			this.Controls.SetChildIndex(this.pnlControl, 0);
			this.Controls.SetChildIndex(this.horzSplitter, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			this.Controls.SetChildIndex(this.splMain, 0);
			this.Controls.SetChildIndex(this.pgMain, 0);
			this.Controls.SetChildIndex(this.pcPaintStyle, 0);
			((System.ComponentModel.ISupportInitialize)(this.gcAppearances)).EndInit();
			this.gcAppearances.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbcAppearances)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlAppearances)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlPreview)).EndInit();
			this.pnlPreview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pcPaintStyle)).EndInit();
			this.pcPaintStyle.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ceFocusedRow.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceFocusedCell.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceOdd.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceEven.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbePaintStyle.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		protected override string DescriptionText { get { return DevExpress.XtraGrid.Design.Properties.Resources.AppearanceDesignerDescription; } }
		private DevExpress.XtraGrid.Views.Base.BaseView EditingView { get { return EditingObject as DevExpress.XtraGrid.Views.Base.BaseView; } }
		public override void InitComponent() {
			CreateTabControl();
			CreatePreviewControl();
			InitPreviewControl();
			InitAppearanceList(previewGrid.MainView.Appearance);
			InitPaintStyles();
		}
		protected override Image AppearanceImage { get { return EditingView.GridControl.BackgroundImage; } }
		protected override XtraTabControl CreateTab() {
			return DevExpress.XtraEditors.Design.FramesUtils.CreateTabProperty(this, new Control[] {pgMain, pcPaintStyle}, new string[] {DevExpress.XtraGrid.Design.Properties.Resources.PropertiesCaption, DevExpress.XtraGrid.Design.Properties.Resources.PaintStyleCaption});
		}
		void InitPaintStyles() {
			for(int i = 0; i < previewGrid.MainView.BaseInfo.PaintStyles.Count; i++) 
				cbePaintStyle.Properties.Items.Add(EditingView.BaseInfo.PaintStyles[i].Name);
			cbePaintStyle.EditValue = previewGrid.MainView.PaintStyleName;
		}
		GridControlAppearences previewGrid = null;
		protected override void CreatePreviewControl() {
			previewGrid = new GridControlAppearences();
			previewGrid.TabStop = false;
			AssignGrid(previewGrid);
			previewGrid.Dock = DockStyle.Fill;
			gcPreview.Controls.Add(previewGrid);
			previewGrid.AppearanceCollectionChanged += new EventHandler(AppearanceCollectionChanged);
			previewGrid.BringToFront();
		}
		void InitPreviewControlEx() {
			GridView view = previewGrid.MainView as GridView;
			bpMode.Visible = ceEven.Visible = ceOdd.Visible = ceFocusedCell.Visible = ceFocusedRow.Visible = view != null;
			if(view != null) {
				view.BeginUpdate();
				view.ClearGrouping();
				view.OptionsView.AutoCalcPreviewLineCount = true;
				view.OptionsView.ShowPreview = true;
				view.CalcPreviewText += new DevExpress.XtraGrid.Views.Grid.CalcPreviewTextEventHandler(CalcPreviewText);
				GridColumn col = view.Columns["Unit Price"];
				view.GroupSummary.Add(SummaryItemType.Average, "Unit Price", col, view.GetSummaryFormat(col, SummaryItemType.Average), col.DisplayFormat.Format);
				view.Columns["Discontinued"].Visible = false;
				view.Columns["Category"].OptionsFilter.AllowFilter = false;
				InitGroupMode();
				ceEven.Checked = view.OptionsView.EnableAppearanceEvenRow;
				ceOdd.Checked = view.OptionsView.EnableAppearanceOddRow;
				ceFocusedCell.Checked = view.OptionsSelection.EnableAppearanceFocusedCell;
				ceFocusedRow.Checked = view.OptionsSelection.EnableAppearanceFocusedRow;
				view.RowSeparatorHeight = ((GridView)EditingView).RowSeparatorHeight;
				view.EndUpdate();
			}
			CardView cView = previewGrid.MainView as CardView;
			if(cView != null) {
				cView.OptionsView.ShowQuickCustomizeButton = false;
				cView.OptionsView.ShowHorzScrollBar = false;
				cView.OptionsBehavior.AutoHorzWidth = false;
			}
			LayoutView lView = previewGrid.MainView as LayoutView;
			if(lView != null) {
				lView.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.ShowAlways;
				lView.OptionsView.ViewMode = LayoutViewMode.Column;
			}
			WinExplorerView eView = previewGrid.MainView as WinExplorerView;
			if(eView != null) {
				eView.OptionsView.Style = WinExplorerViewStyle.Tiles;
				bpMode.Visible = true;
				InitGroupMode();
			}
		}
		bool IsGroup { get { return bpMode.Buttons[0].Properties.Checked; } }
		void InitGroupMode() {
			WinExplorerView eView = previewGrid.MainView as WinExplorerView;
			if(eView != null) {
				eView.ColumnSet.GroupColumn = IsGroup? eView.Columns["Group"]: null;
				eView.GroupCount = IsGroup ? 1 : 0;
				return;
			}
			GridView view = previewGrid.MainView as GridView;
			if(view == null) return;
			view.BestFitColumns();
			view.Columns["Category"].Visible = !IsGroup;
			view.Columns["Category"].GroupIndex = IsGroup ? 0 : -1;
			view.OptionsView.ColumnAutoWidth = !IsGroup;
			if(IsGroup) {
				view.Columns["Product Name"].Width += 30;
				view.Columns["Unit Price"].Width += 20;
			}
			view.FocusedRowHandle = 0;
		}
		private void bpMode_ButtonChecked(object sender, XtraBars.Docking2010.BaseButtonEventArgs e) {
			InitGroupMode();
		}
		protected override void InitPreviewControl() {
			DevExpress.XtraGrid.Design.XViewsPrinting xv = new DevExpress.XtraGrid.Design.XViewsPrinting(previewGrid, true);
			InitPreviewControlEx();
		}
		protected override void InitImages() {
			base.InitImages();
			bpMode.ButtonBackgroundImages = ButtonPanelImages;
			bpMode.Images = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Design.Designer.GridMode.png", typeof(AppearancesDesigner).Assembly, new Size(16, 16));
		}
		void AssignGrid(GridControl grid) {
			string viewName = EditingView.BaseInfo.ViewName;
			System.Reflection.MethodInfo method = typeof(GridControl).GetMethod("RegisterAvailableViewsCore", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
			method.Invoke(EditingView.GridControl, new Object[] { grid.AvailableViews });
			BaseView oldView = grid.MainView;
			grid.MainView = EditingView.BaseInfo.CreateView(grid);
			if(grid.MainView == null) {
				grid.MainView = oldView;
				return;
			}
			if(oldView != null) oldView.Dispose();
			DevExpress.XtraGrid.Views.BandedGrid.BandedGridView bv = grid.MainView as DevExpress.XtraGrid.Views.BandedGrid.BandedGridView;
			if(bv != null) {
				if(bv.Bands.Count == 0)
					bv.Bands.Add();
			}
			DevExpress.XtraGrid.Design.GridAssign.AppearanceAssign(EditingView, grid.MainView);
		}
		#endregion
		#region Editing
		protected string[] SelectedObjectNames {
			get {
				if(SelectedObjects == null) return null;
				ArrayList list = new ArrayList();
				for(int i = 0; i < SelectedObjects.Length; i ++) {
					string name = (SelectedObjects[i] as AppearanceObject).Name;
					if(list.IndexOf(name) < 0)
						list.Add(name);
				}
				return (string[])list.ToArray(typeof(string));
			}
		}
		protected override void SetSelectedObject() {
			if(Preview == null) return;
			ArrayList arr = new ArrayList();
			if(SelectedObjects != null) {
				foreach(AppearanceObject obj in this.SelectedObjects) {
					AppearanceObject app = EditingView.PaintAppearance.GetAppearance(obj.Name);
					if(arr.IndexOf(app) < 0)
						arr.Add(EditingView.PaintAppearance.GetAppearance(obj.Name));
				}
				Preview.SetAppearance(arr.ToArray());
			}
			previewGrid.SelectedAppearanceNames = SelectedObjectNames;
		}
		protected override void AddObject(ArrayList ret, string item) {
			object obj = GetAppearanceObjectByName(previewGrid.MainView.Appearance, item);
			ret.Add(obj);
			object obj2 = GetAppearanceObjectByName(EditingView.Appearance, item);
			ret.Add(obj2); 
		}
		void AppearanceCollectionChanged(object sender, EventArgs e) {
			string[] appearances = sender as string[];
				if(appearances == null || appearances.Length < 1) 
					InitAppearanceList(previewGrid.MainView.Appearance);
				else {
					lbcAppearances.Items.Clear();
					foreach(string name in appearances) {
						if(previewGrid.MainView.Appearance.GetAppearance(name) != null)
							lbcAppearances.Items.Add(name);
					}
					lbcAppearances.SelectedValue = appearances[0];
					if(lbcAppearances.SelectedIndex == -1 && lbcAppearances.Items.Count > 0)
						lbcAppearances.SelectedIndex = 0;
				}
		}
		void CalcPreviewText(object sender, DevExpress.XtraGrid.Views.Grid.CalcPreviewTextEventArgs e) {
			if(e.RowHandle == 1)
				e.PreviewText = DevExpress.XtraGrid.Design.Properties.Resources.GridAppearancePreviewText;
		}
		protected override void SetDefault() {
			previewGrid.MainView.BeginUpdate();
			EditingView.BeginUpdate();
			base.SetDefault();
			previewGrid.MainView.EndUpdate();
			EditingView.EndUpdate();
		}
		protected override void bpAppearances_ButtonClick(object sender, XtraBars.Docking2010.BaseButtonEventArgs e) {
			if(e.Button.Properties.Tag == null) return;
			switch(e.Button.Properties.Tag.ToString()) {
				case "show":
					InitAppearanceList(previewGrid.MainView.Appearance);
					if(previewGrid != null) previewGrid.ResetFocusedElement();
					break;
				case "select":
					SelectAll();
					break;
				case "default":
					SetDefault();
					break;
			}
		}
		protected override void lbcAppearances_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if(!e.Control) return;
			if(e.KeyCode == Keys.A) SelectAll();
			if(e.KeyCode == Keys.Z)	InitAppearanceList(previewGrid.MainView.Appearance);
			if(e.KeyCode == Keys.D)	SetDefault();
		}
		void cbePaintStyle_SelectedIndexChanged(object sender, System.EventArgs e) {
			string styleName = cbePaintStyle.EditValue.ToString();
			previewGrid.MainView.PaintStyleName = styleName;
			EditingView.PaintStyleName = styleName;
			SetSelectedObject();
		}
		private void rgMode_SelectedIndexChanged(object sender, System.EventArgs e) {
		}
		private void ceEven_CheckedChanged(object sender, System.EventArgs e) {
			if(EditingView is GridView) 
				((GridView)EditingView).OptionsView.EnableAppearanceEvenRow = 
					((GridView)previewGrid.MainView).OptionsView.EnableAppearanceEvenRow = ceEven.Checked;
		}
		private void ceOdd_CheckedChanged(object sender, System.EventArgs e) {
			if(EditingView is GridView) 
				((GridView)EditingView).OptionsView.EnableAppearanceOddRow = 
					((GridView)previewGrid.MainView).OptionsView.EnableAppearanceOddRow = ceOdd.Checked;
		}
		private void ceFocusedCell_CheckedChanged(object sender, System.EventArgs e) {
			if(EditingView is GridView) 
				((GridView)EditingView).OptionsSelection.EnableAppearanceFocusedCell = 
					((GridView)previewGrid.MainView).OptionsSelection.EnableAppearanceFocusedCell = ceFocusedCell.Checked;
		}
		private void ceFocusedRow_CheckedChanged(object sender, System.EventArgs e) {
			if(EditingView is GridView) 
				((GridView)EditingView).OptionsSelection.EnableAppearanceFocusedRow = 
					((GridView)previewGrid.MainView).OptionsSelection.EnableAppearanceFocusedRow = ceFocusedRow.Checked;
		}
		#endregion
		#region Load&Save Layout
		protected override void LoadAppearances(string name) {
			previewGrid.MainView.Appearance.RestoreLayoutFromXml(name);
			EditingView.Appearance.RestoreLayoutFromXml(name);
		}
		protected override void SaveAppearances(string name) {
			previewGrid.MainView.Appearance.SaveLayoutToXml(name);
		}
		#endregion
		private void pcPaintStyle_Resize(object sender, EventArgs e) {
			cbePaintStyle.Width = pcPaintStyle.Width - cbePaintStyle.Left - 10;
		}
		private void bpMode_Paint(object sender, PaintEventArgs e) {
			Color borderColor = DevExpress.Skins.CommonSkins.GetSkin(LookAndFeel)[DevExpress.Skins.CommonSkins.SkinTextBorder].Border.Left;
			e.Graphics.DrawLine(new Pen(borderColor), new Point(0, 0), new Point(0, bpMode.Bounds.Height - 1));
			e.Graphics.DrawLine(new Pen(borderColor), new Point(bpMode.Bounds.Width - 1, 0), new Point(bpMode.Bounds.Width - 1, bpMode.Bounds.Height - 1));
			e.Graphics.DrawLine(new Pen(borderColor), new Point(0, bpMode.Bounds.Height - 1), new Point(bpMode.Bounds.Width - 1, bpMode.Bounds.Height - 1));
		}
	}
}
