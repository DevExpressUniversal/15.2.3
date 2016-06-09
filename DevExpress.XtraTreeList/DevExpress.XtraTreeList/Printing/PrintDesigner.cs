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
using DevExpress.XtraTreeList;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.XtraTreeList.Design;
using DevExpress.XtraTreeList.Printing;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraTab;
namespace DevExpress.XtraTreeList.Frames {
	[ToolboxItem(false)]
	public class TreeListPrinting : DevExpress.XtraEditors.Designer.Utils.XtraFrame, IPrintDesigner {
		private bool autoApply = true;
		private XViews XV;
		private System.Windows.Forms.Panel pnlOptions;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.Panel pnlTreeListPreview;
		private System.Windows.Forms.Label label1;
		private DevExpress.XtraEditors.CheckEdit chbHeader;
		private DevExpress.XtraEditors.CheckEdit chbPreview;
		private DevExpress.XtraEditors.CheckEdit chbFooter;
		private DevExpress.XtraEditors.CheckEdit chbHorzLines;
		private DevExpress.XtraEditors.CheckEdit chbVertLines;
		private DevExpress.XtraEditors.CheckEdit chbAutoWidth;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private DevExpress.XtraEditors.CheckEdit chbDefaultStyles;
		private System.Windows.Forms.Label label4;
		private DevExpress.XtraEditors.CheckEdit chbTree;
		private DevExpress.XtraEditors.CheckEdit chbImages;
		private DevExpress.XtraEditors.CheckEdit chbRowFooter;
		private DevExpress.XtraEditors.CheckEdit chbTreeButtons;
		private DevExpress.XtraEditors.CheckEdit chbTreeIndent;
		private DevExpress.XtraEditors.CheckEdit chbAllNodes;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
		private DevExpress.XtraEditors.CheckEdit chbAutoRowHeight;
		public TreeList EditingTreeList { get { return EditingObject as TreeList; } }
		public bool AutoApply {
			get { return autoApply; }
			set { autoApply = value; }
		}
		public TreeListPrinting() : base(8) {
			InitializeComponent();
			InitTreeListPreview();
			lbCaption.Text = TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.PrintDesignerHeader);
		}
		protected override string DescriptionText { get { return TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.PrintDesignerDescription); } }
		DevExpress.XtraTreeList.TreeList TreeListPreview;
		void InitTreeListPreview() {
			this.TreeListPreview = new DevExpress.XtraTreeList.TreeList();
			((System.ComponentModel.ISupportInitialize)(this.TreeListPreview)).BeginInit();
			TreeListPreview.Dock = DockStyle.Fill;
			TreeListPreview.Enabled = false;
			TreeListPreview.UseDisabledStatePainter = false;
			TreeListPreview.TabIndex = 0;
			pnlTreeListPreview.Controls.Add(TreeListPreview);
			((System.ComponentModel.ISupportInitialize)(this.TreeListPreview)).EndInit();
		}
		object iml = null;
		ImageCollection imlCollection = null;
		public override void InitComponent() {
			imlCollection = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraTreeList.Printing.TreeListPrintOptions.png", typeof(TreeListPrinting).Assembly, new Size(16,16));
			TreeListPreview.BeforeFocusNode += new BeforeFocusNodeEventHandler(BeforeFocusNode);
			XV = new XViews(TreeListPreview);
			iml = TreeListPreview.SelectImageList;
			InitPrintOptions();
			InitPrintStates();
			InitViewStyles(chbDefaultStyles.Checked);
			TreeListPreview.OptionsSelection.EnableAppearanceFocusedRow = false;
			TreeListPreview.OptionsSelection.EnableAppearanceFocusedCell = false;
			TreeListPreview.OptionsView.AutoCalcPreviewLineCount = true; 
			TreeListPreview.OptionsView.ShowIndicator = false;
			DevExpress.XtraTreeList.Columns.TreeListColumn col = TreeListPreview.Columns[1];
			TreeListPreview.LookAndFeel.UseWindowsXPTheme = false;
			TreeListPreview.LookAndFeel.UseDefaultLookAndFeel = false;
			TreeListPreview.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
			col.SummaryFooter = SummaryItemType.Sum;
			col.SummaryFooterStrFormat = "Sum = {0:c}";
			col.RowFooterSummary = SummaryItemType.Sum;
			col.RowFooterSummaryStrFormat = "{0:c}";
			col.AllNodesSummary = true; 
			col = TreeListPreview.Columns[0];
			col.SummaryFooter = SummaryItemType.Count;
			col.RowFooterSummary = SummaryItemType.Count;
			col.AllNodesSummary = true;
			TreeListPreview.GetPreviewText += new GetPreviewTextEventHandler(GetPreviewText);
		}
		private void BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e) {
			e.CanFocus = false;
		}
		private void GetPreviewText(object sender, GetPreviewTextEventArgs e) {
			if(e.Node != null && Convert.ToDouble(e.Node[1]) > 1000000)
				e.PreviewText = "This is a description for the " + e.Node[0].ToString() + " department";	
		}
		BaseAppearanceCollection GetPrintAppearance(TreeList treeList) {
			TreeListPrintInfo printInfo = treeList.InternalGetService(typeof(TreeListPrintInfo)) as TreeListPrintInfo;
			return printInfo.PrintAppearance;
		}
		protected virtual void InitViewStyles(bool IsPrintStyles) {
			TreeListPreview.Appearance.Reset();
			if(IsPrintStyles) {
				TreeListPreview.Appearance.Assign(GetPrintAppearance(EditingTreeList));
				TreeListPreview.Appearance.FooterPanel.BorderColor = TreeListPreview.Appearance.FooterPanel.BackColor;
				TreeListPreview.Appearance.HeaderPanel.BorderColor = TreeListPreview.Appearance.HeaderPanel.BackColor;
				TreeListPreview.Appearance.GroupFooter.BorderColor = TreeListPreview.Appearance.GroupFooter.BackColor;
				TreeListPreview.Appearance.HorzLine.Assign(EditingTreeList.AppearancePrint.Lines);
				TreeListPreview.Appearance.VertLine.Assign(EditingTreeList.AppearancePrint.Lines);
				TreeListPreview.OptionsView.EnableAppearanceEvenRow = true;
				TreeListPreview.OptionsView.EnableAppearanceOddRow = true;
			} 
			else {
				TreeListPreview.Appearance.Assign(EditingTreeList.Appearance);
				TreeListPreview.OptionsView.EnableAppearanceEvenRow = EditingTreeList.OptionsView.EnableAppearanceEvenRow;
				TreeListPreview.OptionsView.EnableAppearanceOddRow = EditingTreeList.OptionsView.EnableAppearanceOddRow;
			}
			TreeListPreview.Appearance.Empty.BackColor = TreeListPreview.Appearance.Empty.BackColor2 = Color.White;
			TreeListPreview.LayoutChanged();
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TreeListPrinting));
			this.pnlOptions = new System.Windows.Forms.Panel();
			this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
			this.chbImages = new DevExpress.XtraEditors.CheckEdit();
			this.chbHorzLines = new DevExpress.XtraEditors.CheckEdit();
			this.chbTree = new DevExpress.XtraEditors.CheckEdit();
			this.chbVertLines = new DevExpress.XtraEditors.CheckEdit();
			this.chbTreeIndent = new DevExpress.XtraEditors.CheckEdit();
			this.chbTreeButtons = new DevExpress.XtraEditors.CheckEdit();
			this.chbFooter = new DevExpress.XtraEditors.CheckEdit();
			this.chbRowFooter = new DevExpress.XtraEditors.CheckEdit();
			this.label1 = new System.Windows.Forms.Label();
			this.chbPreview = new DevExpress.XtraEditors.CheckEdit();
			this.chbHeader = new DevExpress.XtraEditors.CheckEdit();
			this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
			this.chbAutoWidth = new DevExpress.XtraEditors.CheckEdit();
			this.chbAutoRowHeight = new DevExpress.XtraEditors.CheckEdit();
			this.label3 = new System.Windows.Forms.Label();
			this.chbAllNodes = new DevExpress.XtraEditors.CheckEdit();
			this.label2 = new System.Windows.Forms.Label();
			this.chbDefaultStyles = new DevExpress.XtraEditors.CheckEdit();
			this.label4 = new System.Windows.Forms.Label();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.pnlTreeListPreview = new System.Windows.Forms.Panel();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.pnlOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
			this.xtraTabControl1.SuspendLayout();
			this.xtraTabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chbImages.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbHorzLines.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbTree.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbVertLines.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbTreeIndent.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbTreeButtons.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbRowFooter.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbPreview.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbHeader.Properties)).BeginInit();
			this.xtraTabPage2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chbAutoWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAutoRowHeight.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAllNodes.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbDefaultStyles.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).BeginInit();
			this.groupControl1.SuspendLayout();
			this.SuspendLayout();
			resources.ApplyResources(this.lbCaption, "lbCaption");
			this.lbCaption.AutoSizeMode = LabelAutoSizeMode.Default;
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
			this.xtraTabControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage1_Paint);
			this.xtraTabPage1.Controls.Add(this.chbImages);
			this.xtraTabPage1.Controls.Add(this.chbHorzLines);
			this.xtraTabPage1.Controls.Add(this.chbTree);
			this.xtraTabPage1.Controls.Add(this.chbVertLines);
			this.xtraTabPage1.Controls.Add(this.chbTreeIndent);
			this.xtraTabPage1.Controls.Add(this.chbTreeButtons);
			this.xtraTabPage1.Controls.Add(this.chbFooter);
			this.xtraTabPage1.Controls.Add(this.chbRowFooter);
			this.xtraTabPage1.Controls.Add(this.label1);
			this.xtraTabPage1.Controls.Add(this.chbPreview);
			this.xtraTabPage1.Controls.Add(this.chbHeader);
			this.xtraTabPage1.Name = "xtraTabPage1";
			resources.ApplyResources(this.xtraTabPage1, "xtraTabPage1");
			this.xtraTabPage1.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage1_Paint);
			resources.ApplyResources(this.chbImages, "chbImages");
			this.chbImages.Name = "chbImages";
			this.chbImages.Properties.Caption = resources.GetString("chbImages.Properties.Caption");
			this.chbImages.Tag = "6";
			this.chbImages.CheckStateChanged += new System.EventHandler(this.chbImages_CheckStateChanged);
			resources.ApplyResources(this.chbHorzLines, "chbHorzLines");
			this.chbHorzLines.Name = "chbHorzLines";
			this.chbHorzLines.Properties.Caption = resources.GetString("chbHorzLines.Properties.Caption");
			this.chbHorzLines.Tag = "4";
			this.chbHorzLines.CheckStateChanged += new System.EventHandler(this.chbHorzLines_CheckStateChanged);
			resources.ApplyResources(this.chbTree, "chbTree");
			this.chbTree.Name = "chbTree";
			this.chbTree.Properties.Caption = resources.GetString("chbTree.Properties.Caption");
			this.chbTree.Tag = "7";
			this.chbTree.CheckStateChanged += new System.EventHandler(this.chbTree_CheckStateChanged);
			resources.ApplyResources(this.chbVertLines, "chbVertLines");
			this.chbVertLines.Name = "chbVertLines";
			this.chbVertLines.Properties.Caption = resources.GetString("chbVertLines.Properties.Caption");
			this.chbVertLines.Tag = "5";
			this.chbVertLines.CheckStateChanged += new System.EventHandler(this.chbVertLines_CheckStateChanged);
			resources.ApplyResources(this.chbTreeIndent, "chbTreeIndent");
			this.chbTreeIndent.Name = "chbTreeIndent";
			this.chbTreeIndent.Properties.Caption = resources.GetString("chbTreeIndent.Properties.Caption");
			this.chbTreeIndent.Tag = "9";
			this.chbTreeIndent.CheckStateChanged += new System.EventHandler(this.chbTreeIndent_CheckStateChanged);
			resources.ApplyResources(this.chbTreeButtons, "chbTreeButtons");
			this.chbTreeButtons.Name = "chbTreeButtons";
			this.chbTreeButtons.Properties.Caption = resources.GetString("chbTreeButtons.Properties.Caption");
			this.chbTreeButtons.Tag = "8";
			this.chbTreeButtons.CheckStateChanged += new System.EventHandler(this.chbTreeButtons_CheckStateChanged);
			resources.ApplyResources(this.chbFooter, "chbFooter");
			this.chbFooter.Name = "chbFooter";
			this.chbFooter.Properties.Caption = resources.GetString("chbFooter.Properties.Caption");
			this.chbFooter.Tag = "2";
			this.chbFooter.CheckStateChanged += new System.EventHandler(this.chbFooter_CheckStateChanged);
			resources.ApplyResources(this.chbRowFooter, "chbRowFooter");
			this.chbRowFooter.Name = "chbRowFooter";
			this.chbRowFooter.Properties.Caption = resources.GetString("chbRowFooter.Properties.Caption");
			this.chbRowFooter.Tag = "3";
			this.chbRowFooter.CheckStateChanged += new System.EventHandler(this.chbRowFooter_CheckStateChanged);
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label1.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label1.Name = "label1";
			resources.ApplyResources(this.chbPreview, "chbPreview");
			this.chbPreview.Name = "chbPreview";
			this.chbPreview.Properties.Caption = resources.GetString("chbPreview.Properties.Caption");
			this.chbPreview.Tag = "1";
			this.chbPreview.CheckStateChanged += new System.EventHandler(this.chbPreview_CheckStateChanged);
			resources.ApplyResources(this.chbHeader, "chbHeader");
			this.chbHeader.Name = "chbHeader";
			this.chbHeader.Properties.Caption = resources.GetString("chbHeader.Properties.Caption");
			this.chbHeader.Tag = "0";
			this.chbHeader.CheckStateChanged += new System.EventHandler(this.chbHeader_CheckStateChanged);
			this.xtraTabPage2.Controls.Add(this.chbAutoWidth);
			this.xtraTabPage2.Controls.Add(this.chbAutoRowHeight);
			this.xtraTabPage2.Controls.Add(this.label3);
			this.xtraTabPage2.Controls.Add(this.chbAllNodes);
			this.xtraTabPage2.Controls.Add(this.label2);
			this.xtraTabPage2.Controls.Add(this.chbDefaultStyles);
			this.xtraTabPage2.Controls.Add(this.label4);
			this.xtraTabPage2.Name = "xtraTabPage2";
			resources.ApplyResources(this.xtraTabPage2, "xtraTabPage2");
			this.xtraTabPage2.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage2_Paint);
			resources.ApplyResources(this.chbAutoWidth, "chbAutoWidth");
			this.chbAutoWidth.Name = "chbAutoWidth";
			this.chbAutoWidth.Properties.Caption = resources.GetString("chbAutoWidth.Properties.Caption");
			this.chbAutoWidth.Tag = "10";
			this.chbAutoWidth.CheckStateChanged += new System.EventHandler(this.chbAutoWidth_CheckStateChanged);
			resources.ApplyResources(this.chbAutoRowHeight, "chbAutoRowHeight");
			this.chbAutoRowHeight.Name = "chbAutoRowHeight";
			this.chbAutoRowHeight.Properties.Caption = resources.GetString("chbAutoRowHeight.Properties.Caption");
			this.chbAutoRowHeight.Tag = "11";
			this.chbAutoRowHeight.CheckStateChanged += new System.EventHandler(this.chbAutoRowHeight_CheckStateChanged);
			resources.ApplyResources(this.label3, "label3");
			this.label3.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label3.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label3.Name = "label3";
			resources.ApplyResources(this.chbAllNodes, "chbAllNodes");
			this.chbAllNodes.Name = "chbAllNodes";
			this.chbAllNodes.Properties.Caption = resources.GetString("chbAllNodes.Properties.Caption");
			this.chbAllNodes.Tag = "12";
			this.chbAllNodes.CheckStateChanged += new System.EventHandler(this.chbAllNodes_CheckStateChanged);
			resources.ApplyResources(this.label2, "label2");
			this.label2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label2.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label2.Name = "label2";
			resources.ApplyResources(this.chbDefaultStyles, "chbDefaultStyles");
			this.chbDefaultStyles.Name = "chbDefaultStyles";
			this.chbDefaultStyles.Properties.Caption = resources.GetString("chbDefaultStyles.Properties.Caption");
			this.chbDefaultStyles.Tag = "13";
			this.chbDefaultStyles.CheckStateChanged += new System.EventHandler(this.chbDefaultStyles_CheckStateChanged);
			resources.ApplyResources(this.label4, "label4");
			this.label4.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label4.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label4.Name = "label4";
			resources.ApplyResources(this.splitter1, "splitter1");
			this.splitter1.Name = "splitter1";
			this.splitter1.TabStop = false;
			this.pnlTreeListPreview.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.pnlTreeListPreview, "pnlTreeListPreview");
			this.pnlTreeListPreview.Name = "pnlTreeListPreview";
			this.groupControl1.Controls.Add(this.pnlTreeListPreview);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "TreeListPrinting";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.pnlOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
			this.xtraTabControl1.ResumeLayout(false);
			this.xtraTabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chbImages.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbHorzLines.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbTree.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbVertLines.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbTreeIndent.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbTreeButtons.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbRowFooter.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbPreview.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbHeader.Properties)).EndInit();
			this.xtraTabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chbAutoWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAutoRowHeight.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAllNodes.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbDefaultStyles.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		int x = 10;
		private void tabPage1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			for(int i = 0; i < 10; i++) 
				PrintImage(i, e.Graphics);
		}
		private void tabPage2_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			for(int i = 10; i < 14; i++) 
				PrintImage(i, e.Graphics);
		}
		private void PrintImage(int i, Graphics g) {
			if(imlCollection == null) return;
			CheckEdit chb = CheckBoxByIndex(i);
			if(chb != null) {
				int top = chb.Top + (chb.Height - imlCollection.ImageSize.Height) / 2;
				if(chb.Checked)
					g.DrawImage(imlCollection.Images[i], x, top);
				else
					ControlPaint.DrawImageDisabled(g, imlCollection.Images[i], x, top, SystemColors.Control);
			}
		}
		private void ApplyOptions() {
			ApplyOptions(AutoApply);
		}
		public void ApplyOptions(bool setOptions) {
			if(lockApply != 0 || !setOptions) return;
			CheckEdit chb;
			for(int i = 0; i < printFlags.Length; i++) {
				chb = CheckEditByIndex(i);
				if(chb != null)
					SetOptions.SetOptionValueByString(printFlags[i], EditingTreeList.OptionsPrint, chb.Checked);
			}
			EditingTreeList.FireChanged();
		}
		private void InvalidateImage(object checkBox ) {
			CheckEdit chb = checkBox as CheckEdit;
			((XtraTabPage)chb.Parent).Invalidate(new Rectangle(0, chb.Top, 50, chb.Height));
			ApplyOptions();
		} 
		private CheckEdit CheckBoxByIndex(int index) {
			foreach(XtraTabPage tp in xtraTabControl1.TabPages) 
				foreach(object o in tp.Controls) 
					if(o is CheckEdit && ((CheckEdit)o).Tag.ToString() == index.ToString())
						return o as CheckEdit;
			return null;
		}
		private string[] printFlags = new string[] {
			"PrintPageHeader", "PrintPreview", "PrintReportFooter", "PrintRowFooterSummary",
			"PrintHorzLines", "PrintVertLines", "PrintImages", "PrintTree", 
			"PrintTreeButtons", "PrintFilledTreeIndent", 
			"AutoWidth", "AutoRowHeight", "PrintAllNodes",
			"UsePrintStyles"
		};
		private int lockApply = 0;
		private void InitPrintOptions() {
			lockApply--;
			for(int i = 0; i < printFlags.Length; i++) {
				CheckEdit chb = CheckEditByIndex(i);
				if(chb != null) {
					chb.Checked = SetOptions.OptionValueByString(printFlags[i], EditingTreeList.OptionsPrint);
					if(printFlags[i] == "PrintAllNodes")
						chb.Enabled = !EditingTreeList.IsVirtualMode; 
					if(chb.Checked)  
						chb.CheckState = CheckState.Checked;
				}
			}
			lockApply++;
		}
		protected virtual void InitPrintStates() {
			chbHeader_CheckStateChanged(chbHeader, EventArgs.Empty);
			chbPreview_CheckStateChanged(chbPreview, EventArgs.Empty);
			chbFooter_CheckStateChanged(chbFooter, EventArgs.Empty);
			chbRowFooter_CheckStateChanged(chbRowFooter, EventArgs.Empty);
			chbHorzLines_CheckStateChanged(chbHorzLines, EventArgs.Empty);
			chbVertLines_CheckStateChanged(chbVertLines, EventArgs.Empty);
			chbAutoWidth_CheckStateChanged(chbAutoWidth, EventArgs.Empty);
			chbDefaultStyles_CheckStateChanged(chbDefaultStyles, EventArgs.Empty);
			chbTreeButtons_CheckStateChanged(chbTreeButtons, EventArgs.Empty);
			chbTreeIndent_CheckStateChanged(chbTreeIndent, EventArgs.Empty);
			chbImages_CheckStateChanged(chbImages, EventArgs.Empty);
			chbTree_CheckStateChanged(chbTree, EventArgs.Empty);
		}
		protected CheckEdit CheckEditByIndex(int index) {
			foreach(XtraTabPage tp in xtraTabControl1.TabPages) 
				foreach(object o in tp.Controls) 
					if(o is CheckEdit && ((CheckEdit)o).Tag.ToString() == index.ToString())
						return o as CheckEdit;
			return null;
		}
		private void chbHeader_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			TreeListPreview.OptionsView.ShowColumns = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbPreview_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			TreeListPreview.OptionsView.ShowPreview = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbFooter_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			TreeListPreview.OptionsView.ShowSummaryFooter = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbRowFooter_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			TreeListPreview.OptionsView.ShowRowFooterSummary = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbHorzLines_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			TreeListPreview.OptionsView.ShowHorzLines = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbVertLines_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			TreeListPreview.OptionsView.ShowVertLines = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbAutoWidth_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			TreeListPreview.OptionsView.AutoWidth = chb.Checked;
			TreeListPreview.BestFitColumns();
			InvalidateImage(sender);
		}
		private void chbDefaultStyles_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			InitViewStyles(chb.Checked);
			InvalidateImage(sender);
		}
		private void chbImages_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			if(chb.Checked)
				TreeListPreview.SelectImageList = iml;
			else TreeListPreview.SelectImageList = null;
			InvalidateImage(sender);
		}
		private void chbTree_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			if(chb.Checked)
				TreeListPreview.TreeLineStyle = LineStyle.Percent50;
			else TreeListPreview.TreeLineStyle = LineStyle.None;
			InvalidateImage(sender);
		}
		private void chbTreeButtons_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			TreeListPreview.OptionsView.ShowButtons = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbTreeIndent_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			TreeListPreview.OptionsView.ShowIndentAsRowStyle = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbAllNodes_CheckStateChanged(object sender, System.EventArgs e) {
			InvalidateImage(sender);
		}
		private void chbAutoRowHeight_CheckStateChanged(object sender, System.EventArgs e) {
			InvalidateImage(sender);
		}
		public Size UserControlSize {
			get { return DevExpress.Utils.ScaleUtils.GetScaleSize(new Size(560, 345)); }
		}
		#region IPrintDesigner Members
		public void HideCaption() {
			lbCaption.Visible = horzSplitter.Visible = false;
		}
		#endregion
	}
}
