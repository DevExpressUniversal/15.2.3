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
using System.Reflection;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Frames;
using DevExpress.XtraGrid.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.Printing;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class GridViewPrinting : DevExpress.XtraEditors.Designer.Utils.XtraFrame, IPrintDesigner {
		private System.Windows.Forms.Panel pnlOptions;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel pnlGridPreview;
		private System.Windows.Forms.Label label1;
		protected DevExpress.XtraEditors.CheckEdit chbPreview;
		protected DevExpress.XtraEditors.CheckEdit chbFooter;
		protected DevExpress.XtraEditors.CheckEdit chbGroupFooter;
		protected DevExpress.XtraEditors.CheckEdit chbHorzLines;
		protected DevExpress.XtraEditors.CheckEdit chbVertLines;
		protected DevExpress.XtraEditors.CheckEdit chbFilterInfo;
		protected DevExpress.XtraEditors.CheckEdit chbDetails;
		protected DevExpress.XtraEditors.CheckEdit chbAutoWidth;
		private System.Windows.Forms.Label label2;
		private DevExpress.XtraEditors.CheckEdit chbAllDetails;
		private System.Windows.Forms.Label label3;
		private DevExpress.XtraEditors.CheckEdit chbAllGroups;
		private DevExpress.XtraEditors.CheckEdit chbDefaultStyles;
		private System.Windows.Forms.Label label4;
		private DevExpress.XtraEditors.CheckEdit chbHeader;
		private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
		protected DevExpress.XtraTab.XtraTabPage xtraTabPage1;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		protected DevExpress.XtraEditors.CheckEdit chbEvenRow;
		protected DevExpress.XtraEditors.CheckEdit chbOddRow;
		private CheckEdit chbSelectedRows;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridViewPrinting));
			this.pnlOptions = new System.Windows.Forms.Panel();
			this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
			this.chbSelectedRows = new DevExpress.XtraEditors.CheckEdit();
			this.chbHorzLines = new DevExpress.XtraEditors.CheckEdit();
			this.chbVertLines = new DevExpress.XtraEditors.CheckEdit();
			this.chbDetails = new DevExpress.XtraEditors.CheckEdit();
			this.chbFooter = new DevExpress.XtraEditors.CheckEdit();
			this.chbPreview = new DevExpress.XtraEditors.CheckEdit();
			this.label1 = new System.Windows.Forms.Label();
			this.chbHeader = new DevExpress.XtraEditors.CheckEdit();
			this.chbGroupFooter = new DevExpress.XtraEditors.CheckEdit();
			this.chbFilterInfo = new DevExpress.XtraEditors.CheckEdit();
			this.chbEvenRow = new DevExpress.XtraEditors.CheckEdit();
			this.chbOddRow = new DevExpress.XtraEditors.CheckEdit();
			this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
			this.chbAutoWidth = new DevExpress.XtraEditors.CheckEdit();
			this.label2 = new System.Windows.Forms.Label();
			this.chbAllDetails = new DevExpress.XtraEditors.CheckEdit();
			this.label3 = new System.Windows.Forms.Label();
			this.chbAllGroups = new DevExpress.XtraEditors.CheckEdit();
			this.chbDefaultStyles = new DevExpress.XtraEditors.CheckEdit();
			this.label4 = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.pnlGridPreview = new System.Windows.Forms.Panel();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.pnlOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
			this.xtraTabControl1.SuspendLayout();
			this.xtraTabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chbSelectedRows.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbHorzLines.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbVertLines.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbDetails.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbPreview.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbHeader.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbGroupFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbFilterInfo.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbEvenRow.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbOddRow.Properties)).BeginInit();
			this.xtraTabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chbAutoWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAllDetails.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAllGroups.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbDefaultStyles.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.pnlMain.Controls.Add(this.groupControl1);
			this.pnlMain.Controls.Add(this.splitter1);
			this.pnlMain.Controls.Add(this.pnlOptions);
			resources.ApplyResources(this.pnlMain, "pnlMain");
			resources.ApplyResources(this.horzSplitter, "horzSplitter");
			this.pnlOptions.Controls.Add(this.xtraTabControl1);
			resources.ApplyResources(this.pnlOptions, "pnlOptions");
			this.pnlOptions.Name = "pnlOptions";
			resources.ApplyResources(this.xtraTabControl1, "xtraTabControl1");
			this.xtraTabControl1.Name = "xtraTabControl1";
			this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
			this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.xtraTabPage1,
			this.xtraTabPage2});
			resources.ApplyResources(this.xtraTabPage1, "xtraTabPage1");
			this.xtraTabPage1.Controls.Add(this.chbSelectedRows);
			this.xtraTabPage1.Controls.Add(this.chbHorzLines);
			this.xtraTabPage1.Controls.Add(this.chbVertLines);
			this.xtraTabPage1.Controls.Add(this.chbDetails);
			this.xtraTabPage1.Controls.Add(this.chbFooter);
			this.xtraTabPage1.Controls.Add(this.chbPreview);
			this.xtraTabPage1.Controls.Add(this.label1);
			this.xtraTabPage1.Controls.Add(this.chbHeader);
			this.xtraTabPage1.Controls.Add(this.chbGroupFooter);
			this.xtraTabPage1.Controls.Add(this.chbFilterInfo);
			this.xtraTabPage1.Controls.Add(this.chbEvenRow);
			this.xtraTabPage1.Controls.Add(this.chbOddRow);
			this.xtraTabPage1.Name = "xtraTabPage1";
			this.xtraTabPage1.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage1_Paint);
			resources.ApplyResources(this.chbSelectedRows, "chbSelectedRows");
			this.chbSelectedRows.Name = "chbSelectedRows";
			this.chbSelectedRows.Properties.Caption = resources.GetString("chbSelectedRows.Properties.Caption");
			this.chbSelectedRows.Tag = "15";
			this.chbSelectedRows.CheckedChanged += new System.EventHandler(this.chbSelectedRows_CheckedChanged);
			resources.ApplyResources(this.chbHorzLines, "chbHorzLines");
			this.chbHorzLines.Name = "chbHorzLines";
			this.chbHorzLines.Properties.Caption = resources.GetString("chbHorzLines.Properties.Caption");
			this.chbHorzLines.Tag = "4";
			this.chbHorzLines.CheckStateChanged += new System.EventHandler(this.chbHorzLines_CheckStateChanged);
			resources.ApplyResources(this.chbVertLines, "chbVertLines");
			this.chbVertLines.Name = "chbVertLines";
			this.chbVertLines.Properties.Caption = resources.GetString("chbVertLines.Properties.Caption");
			this.chbVertLines.Tag = "5";
			this.chbVertLines.CheckStateChanged += new System.EventHandler(this.chbVertLines_CheckStateChanged);
			resources.ApplyResources(this.chbDetails, "chbDetails");
			this.chbDetails.Name = "chbDetails";
			this.chbDetails.Properties.Caption = resources.GetString("chbDetails.Properties.Caption");
			this.chbDetails.Tag = "7";
			this.chbDetails.CheckStateChanged += new System.EventHandler(this.chbDetails_CheckStateChanged);
			resources.ApplyResources(this.chbFooter, "chbFooter");
			this.chbFooter.Name = "chbFooter";
			this.chbFooter.Properties.Caption = resources.GetString("chbFooter.Properties.Caption");
			this.chbFooter.Tag = "2";
			this.chbFooter.CheckStateChanged += new System.EventHandler(this.chbFooter_CheckStateChanged);
			resources.ApplyResources(this.chbPreview, "chbPreview");
			this.chbPreview.Name = "chbPreview";
			this.chbPreview.Properties.Caption = resources.GetString("chbPreview.Properties.Caption");
			this.chbPreview.Tag = "1";
			this.chbPreview.CheckStateChanged += new System.EventHandler(this.chbPreview_CheckStateChanged);
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label1.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label1.Name = "label1";
			resources.ApplyResources(this.chbHeader, "chbHeader");
			this.chbHeader.Name = "chbHeader";
			this.chbHeader.Properties.Caption = resources.GetString("chbHeader.Properties.Caption");
			this.chbHeader.Tag = "0";
			this.chbHeader.CheckStateChanged += new System.EventHandler(this.chbHeader_CheckStateChanged);
			resources.ApplyResources(this.chbGroupFooter, "chbGroupFooter");
			this.chbGroupFooter.Name = "chbGroupFooter";
			this.chbGroupFooter.Properties.Caption = resources.GetString("chbGroupFooter.Properties.Caption");
			this.chbGroupFooter.Tag = "3";
			this.chbGroupFooter.CheckStateChanged += new System.EventHandler(this.chbGroupFooter_CheckStateChanged);
			resources.ApplyResources(this.chbFilterInfo, "chbFilterInfo");
			this.chbFilterInfo.Name = "chbFilterInfo";
			this.chbFilterInfo.Properties.Caption = resources.GetString("chbFilterInfo.Properties.Caption");
			this.chbFilterInfo.Tag = "6";
			this.chbFilterInfo.CheckStateChanged += new System.EventHandler(this.chbFilterInfo_CheckStateChanged);
			resources.ApplyResources(this.chbEvenRow, "chbEvenRow");
			this.chbEvenRow.Name = "chbEvenRow";
			this.chbEvenRow.Properties.Appearance.Options.UseTextOptions = true;
			this.chbEvenRow.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chbEvenRow.Properties.AutoHeight = ((bool)(resources.GetObject("chbEvenRow.Properties.AutoHeight")));
			this.chbEvenRow.Properties.Caption = resources.GetString("chbEvenRow.Properties.Caption");
			this.chbEvenRow.Tag = "12";
			this.chbEvenRow.CheckStateChanged += new System.EventHandler(this.chbEvenRow_CheckStateChanged);
			resources.ApplyResources(this.chbOddRow, "chbOddRow");
			this.chbOddRow.Name = "chbOddRow";
			this.chbOddRow.Properties.Appearance.Options.UseTextOptions = true;
			this.chbOddRow.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chbOddRow.Properties.AutoHeight = ((bool)(resources.GetObject("chbOddRow.Properties.AutoHeight")));
			this.chbOddRow.Properties.Caption = resources.GetString("chbOddRow.Properties.Caption");
			this.chbOddRow.Tag = "13";
			this.chbOddRow.CheckStateChanged += new System.EventHandler(this.chbOddRow_CheckStateChanged);
			this.xtraTabPage2.Controls.Add(this.chbAutoWidth);
			this.xtraTabPage2.Controls.Add(this.label2);
			this.xtraTabPage2.Controls.Add(this.chbAllDetails);
			this.xtraTabPage2.Controls.Add(this.label3);
			this.xtraTabPage2.Controls.Add(this.chbAllGroups);
			this.xtraTabPage2.Controls.Add(this.chbDefaultStyles);
			this.xtraTabPage2.Controls.Add(this.label4);
			this.xtraTabPage2.Name = "xtraTabPage2";
			resources.ApplyResources(this.xtraTabPage2, "xtraTabPage2");
			this.xtraTabPage2.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage2_Paint);
			resources.ApplyResources(this.chbAutoWidth, "chbAutoWidth");
			this.chbAutoWidth.Name = "chbAutoWidth";
			this.chbAutoWidth.Properties.Caption = resources.GetString("chbAutoWidth.Properties.Caption");
			this.chbAutoWidth.Tag = "8";
			this.chbAutoWidth.CheckStateChanged += new System.EventHandler(this.chbAutoWidth_CheckStateChanged);
			resources.ApplyResources(this.label2, "label2");
			this.label2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label2.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label2.Name = "label2";
			resources.ApplyResources(this.chbAllDetails, "chbAllDetails");
			this.chbAllDetails.Name = "chbAllDetails";
			this.chbAllDetails.Properties.Caption = resources.GetString("chbAllDetails.Properties.Caption");
			this.chbAllDetails.Tag = "9";
			this.chbAllDetails.CheckStateChanged += new System.EventHandler(this.chbAllDetails_CheckStateChanged);
			resources.ApplyResources(this.label3, "label3");
			this.label3.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label3.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label3.Name = "label3";
			resources.ApplyResources(this.chbAllGroups, "chbAllGroups");
			this.chbAllGroups.Name = "chbAllGroups";
			this.chbAllGroups.Properties.Caption = resources.GetString("chbAllGroups.Properties.Caption");
			this.chbAllGroups.Tag = "10";
			this.chbAllGroups.CheckStateChanged += new System.EventHandler(this.chbAllGroups_CheckStateChanged);
			resources.ApplyResources(this.chbDefaultStyles, "chbDefaultStyles");
			this.chbDefaultStyles.Name = "chbDefaultStyles";
			this.chbDefaultStyles.Properties.Caption = resources.GetString("chbDefaultStyles.Properties.Caption");
			this.chbDefaultStyles.Tag = "11";
			this.chbDefaultStyles.CheckStateChanged += new System.EventHandler(this.chbDefaultStyles_CheckStateChanged);
			resources.ApplyResources(this.label4, "label4");
			this.label4.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label4.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label4.Name = "label4";
			resources.ApplyResources(this.splitter1, "splitter1");
			this.splitter1.Name = "splitter1";
			this.splitter1.TabStop = false;
			this.pnlGridPreview.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.pnlGridPreview, "pnlGridPreview");
			this.pnlGridPreview.Name = "pnlGridPreview";
			this.groupControl1.Controls.Add(this.pnlGridPreview);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "GridViewPrinting";
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.pnlOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
			this.xtraTabControl1.ResumeLayout(false);
			this.xtraTabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chbSelectedRows.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbHorzLines.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbVertLines.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbDetails.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbPreview.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbHeader.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbGroupFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbFilterInfo.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbEvenRow.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbOddRow.Properties)).EndInit();
			this.xtraTabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chbAutoWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAllDetails.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAllGroups.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbDefaultStyles.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		protected virtual string ViewType { get { return "GridView";} }
		protected PreviewGrid GridPreview;
		public bool AutoApply = true;
		ImageCollection imCol;
		public GridViewPrinting() : base(8) {
			InitializeComponent();
			pnlGridPreview.DockPadding.All = 4;
			GridPreview = new PreviewGrid(pnlGridPreview, ViewType);
			imCol = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Printing.GridPrintOptions.png", typeof(GridViewDesigner).Assembly, new Size(16, 16));
		}
		protected override string DescriptionText { get { return GridLocalizer.Active.GetLocalizedString(GridStringId.PrintDesignerDescription); } }
		public virtual GridView CurrentView {
			get { return GridPreview.MainView as GridView; }
		}
		public virtual GridView EditingView {
			get { return EditingObject as GridView; }
		}
		public override void InitComponent() {
			InitPrintOptions();
			InitPrintStates(); 
			CurrentView.AppearancePrint.Assign(EditingView.AppearancePrint);
			CurrentView.OptionsPrint.Assign(EditingView.OptionsPrint);
			InitViewStyles(chbDefaultStyles.Checked);
			lbCaption.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.PrintDesignerGridView);
			CurrentView.PaintStyleName = "MixedXP";
		}
		BaseAppearanceCollection GetPrintAppearance(GridView view) {
			MethodInfo method = view.GetType().GetMethod("CreatePrintInfoCore", BindingFlags.NonPublic | BindingFlags.Instance);
			BaseViewPrintInfo pi = method.Invoke(view, new object[] { new PrintInfoArgs(view)}) as BaseViewPrintInfo;
			return pi.AppearancePrint ;
		}
		protected virtual void InitViewStyles(bool IsPrintStyles) {
			CurrentView.BeginUpdate();
			try {
				CurrentView.Appearance.Reset();
				if(IsPrintStyles) {
					GridViewPrintAppearances collection = GetPrintAppearance(CurrentView) as GridViewPrintAppearances;
					if(collection != null) {
						CurrentView.Appearance.Assign(collection);
						CurrentView.Appearance.HorzLine.Assign(collection.Lines);
						CurrentView.Appearance.VertLine.Assign(collection.Lines);
					}
				} 
				else 
					CurrentView.Appearance.Assign(EditingView.PaintAppearance);
				CurrentView.Appearance.FooterPanel.BorderColor = CurrentView.Appearance.FooterPanel.BackColor;
				CurrentView.Appearance.HeaderPanel.BorderColor = CurrentView.Appearance.HeaderPanel.BackColor;
				CurrentView.Appearance.GroupFooter.BorderColor = CurrentView.Appearance.GroupFooter.BackColor;
				CurrentView.OptionsView.EnableAppearanceEvenRow = IsPrintStyles ? chbEvenRow.Checked : EditingView.OptionsView.EnableAppearanceEvenRow;
				CurrentView.OptionsView.EnableAppearanceOddRow = IsPrintStyles ? chbOddRow.Checked : EditingView.OptionsView.EnableAppearanceOddRow;
				CurrentView.Appearance.Empty.BackColor = CurrentView.Appearance.Empty.BackColor2 = Color.White;
			} finally {
				CurrentView.EndUpdate();
			}
		}
		#endregion
		#region Grid behavior
		private string[] printFlags = new string[] {
													   "PrintHeader", "PrintPreview", "PrintFooter", "PrintGroupFooter",
													   "PrintHorzLines", "PrintVertLines", "PrintFilterInfo", "PrintDetails", 
													   "AutoWidth", "ExpandAllDetails", "ExpandAllGroups", "UsePrintStyles",
													   "EnableAppearanceEvenRow", "EnableAppearanceOddRow", "PrintBandHeader", "PrintSelectedRowsOnly"
												   };
		private int lockApply = 0;
		private void InitPrintOptions() {
			lockApply--;
			for(int i = 0; i < printFlags.Length; i++) {
				CheckEdit chb = CheckEditByIndex(i);
				if(chb != null) {
					chb.Checked = SetOptions.OptionValueByString(printFlags[i], EditingView.OptionsPrint);
					if(chb.Checked)  
						chb.CheckState = CheckState.Checked;
				}
			}
			lockApply++;
		}
		protected virtual void InitPrintStates() {
			CurrentView.OptionsView.BeginUpdate();
			try {
				chbHeader_CheckStateChanged(chbHeader, EventArgs.Empty);
				chbPreview_CheckStateChanged(chbPreview, EventArgs.Empty);
				chbFooter_CheckStateChanged(chbFooter, EventArgs.Empty);
				chbGroupFooter_CheckStateChanged(chbGroupFooter, EventArgs.Empty);
				chbHorzLines_CheckStateChanged(chbHorzLines, EventArgs.Empty);
				chbVertLines_CheckStateChanged(chbVertLines, EventArgs.Empty);
				chbAutoWidth_CheckStateChanged(chbAutoWidth, EventArgs.Empty);
				chbDefaultStyles_CheckStateChanged(chbDefaultStyles, EventArgs.Empty);
				chbEvenRow_CheckStateChanged(chbEvenRow, EventArgs.Empty);
				chbOddRow_CheckStateChanged(chbOddRow, EventArgs.Empty);
				chbSelectedRows_CheckedChanged(chbSelectedRows, EventArgs.Empty);
			}
			finally {
				CurrentView.OptionsView.EndUpdate();
			}
		}
		#endregion
		#region Images paint
		protected int x = 10;
		protected virtual void tabPage1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			for(int i = 0; i < 8; i++) 
				PrintImage(i, e.Graphics);
			PrintImage(12, e.Graphics);
			PrintImage(13, e.Graphics);
			PrintImage(15, e.Graphics);
		}
		private void tabPage2_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			for(int i = 8; i <= 11; i++) 
				PrintImage(i, e.Graphics);
		}
		protected void PrintImage(int i, Graphics g) {
			CheckEdit chb = CheckEditByIndex(i);
			PrintImage(chb, g, imCol.Images[i]);
		}
		void PrintImage(CheckEdit chb, Graphics g, Image img) {
			if(chb != null) {
				int top = chb.Top + (chb.Height - img.Size.Height) / 2;
				if(chb.Checked) 
					g.DrawImage(img, new Rectangle(x, top, img.Width, img.Height));
				else
					ControlPaint.DrawImageDisabled(g, img, x, top, SystemColors.Control);
			}
		}
		protected void InvalidateImage(object checkBox ) {
			CheckEdit chb = checkBox as CheckEdit;
			((XtraTabPage)chb.Parent).Invalidate(new Rectangle(0, chb.Top, 50, chb.Height));
			ApplyOptions();
		}
		#endregion
		#region IPrintDesigner
		private void ApplyOptions() {
			ApplyOptions(AutoApply);
		}
		public void ApplyOptions(bool setOptions) {
			if(lockApply != 0) return;
			CheckEdit chb;
			for(int i = 0; i < printFlags.Length; i++) {
				chb = CheckEditByIndex(i);
				if(chb != null) {
					if(setOptions) SetOptions.SetOptionValueByString(printFlags[i], EditingView.OptionsPrint, chb.Checked);
					SetOptions.SetOptionValueByString(printFlags[i], CurrentView.OptionsPrint, chb.Checked);
				}
			}
			EditingView.FireChanged();
		}
		public void HideCaption() {
			lbCaption.Visible = horzSplitter.Visible = false;
		}
		#endregion
		#region Editing 
		protected CheckEdit CheckEditByIndex(int index) {
			foreach(XtraTabPage tp in xtraTabControl1.TabPages) 
				foreach(object o in tp.Controls) 
					if(o is CheckEdit && ((CheckEdit)o).Tag.ToString() == index.ToString())
						return o as CheckEdit;
			return null;
		}
		private void chbHeader_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			CurrentView.OptionsView.ShowColumnHeaders = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbPreview_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			CurrentView.OptionsView.ShowPreview = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbFooter_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			CurrentView.OptionsView.ShowFooter = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbGroupFooter_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			DevExpress.XtraGrid.Columns.GridColumn col = CurrentView.Columns["Discontinued"];
			if(chb.Checked) col.GroupIndex = 0;
			else {
				col.GroupIndex = -1;
				col.VisibleIndex = 3;
			}
			CurrentView.ExpandAllGroups();	
			CurrentView.FocusedRowHandle = -99;
			InvalidateImage(sender);
		}
		private void chbHorzLines_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			CurrentView.OptionsView.ShowHorizontalLines = chb.Checked ? DefaultBoolean.True : DefaultBoolean.False;
			InvalidateImage(sender);
		}
		private void chbVertLines_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			CurrentView.OptionsView.ShowVerticalLines = chb.Checked ? DefaultBoolean.True : DefaultBoolean.False;
			InvalidateImage(sender);
		}
		private void chbFilterInfo_CheckStateChanged(object sender, System.EventArgs e) {
			InvalidateImage(sender);
		}
		private void chbDetails_CheckStateChanged(object sender, System.EventArgs e) {
			InvalidateImage(sender);
		}
		private void chbAutoWidth_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			CurrentView.OptionsView.ColumnAutoWidth = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbAllDetails_CheckStateChanged(object sender, System.EventArgs e) {
			InvalidateImage(sender);
		}
		private void chbAllGroups_CheckStateChanged(object sender, System.EventArgs e) {
			InvalidateImage(sender);
		}
		private void chbDefaultStyles_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			InvalidateImage(sender);
			InitViewStyles(chb.Checked);
			chbEvenRow.Enabled = chbOddRow.Enabled = chb.Checked;
		}
		private void chbEvenRow_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			InvalidateImage(sender);
			InitViewStyles(chbDefaultStyles.Checked);
		}
		private void chbOddRow_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			InvalidateImage(sender);
			InitViewStyles(chbDefaultStyles.Checked);
		}
		private void chbSelectedRows_CheckedChanged(object sender, EventArgs e) {
			InvalidateImage(sender);
		}
		#endregion
		#region UserControlSize
		public Size UserControlSize { get { return DevExpress.Utils.ScaleUtils.GetScaleSize(new Size(560, 345)); } }
		#endregion
	}
}
