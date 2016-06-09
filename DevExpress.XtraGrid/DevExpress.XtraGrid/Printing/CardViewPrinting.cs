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
using DevExpress.Utils.Design;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Design;
using DevExpress.Utils.Frames;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraTab;
using DevExpress.XtraGrid.Views.Printing;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class CardViewPrinting : DevExpress.XtraEditors.Designer.Utils.XtraFrame, IPrintDesigner {
		private System.Windows.Forms.Panel pnlGridPreview;
		private System.Windows.Forms.Panel pnlOptions;
		private System.Windows.Forms.Label label1;
		private DevExpress.XtraEditors.CheckEdit chbDefaultStyles;
		private System.Windows.Forms.Label label4;
		private DevExpress.XtraEditors.CheckEdit chbCardCaption;
		private DevExpress.XtraEditors.CheckEdit chbAutoHorzWidth;
		private System.Windows.Forms.Label label2;
		private DevExpress.XtraEditors.CheckEdit chbSelectedCardOnly;
		private DevExpress.XtraEditors.CheckEdit chbEmptyFields;
		private System.Windows.Forms.Label label3;
		private DevExpress.XtraEditors.SpinEdit spinEdit1;
		private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
		private DevExpress.XtraEditors.GroupControl groupControl1;
		private System.Windows.Forms.Splitter splitter1;
		#region Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CardViewPrinting));
			this.pnlGridPreview = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.pnlOptions = new System.Windows.Forms.Panel();
			this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
			this.chbEmptyFields = new DevExpress.XtraEditors.CheckEdit();
			this.chbCardCaption = new DevExpress.XtraEditors.CheckEdit();
			this.chbAutoHorzWidth = new DevExpress.XtraEditors.CheckEdit();
			this.label3 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.chbSelectedCardOnly = new DevExpress.XtraEditors.CheckEdit();
			this.label2 = new System.Windows.Forms.Label();
			this.spinEdit1 = new DevExpress.XtraEditors.SpinEdit();
			this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
			this.chbDefaultStyles = new DevExpress.XtraEditors.CheckEdit();
			this.label4 = new System.Windows.Forms.Label();
			this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.pnlOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
			this.xtraTabControl1.SuspendLayout();
			this.xtraTabPage1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chbEmptyFields.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbCardCaption.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAutoHorzWidth.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chbSelectedCardOnly.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).BeginInit();
			this.xtraTabPage2.SuspendLayout();
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
			this.pnlGridPreview.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.pnlGridPreview, "pnlGridPreview");
			this.pnlGridPreview.Name = "pnlGridPreview";
			resources.ApplyResources(this.splitter1, "splitter1");
			this.splitter1.Name = "splitter1";
			this.splitter1.TabStop = false;
			this.pnlOptions.Controls.Add(this.xtraTabControl1);
			resources.ApplyResources(this.pnlOptions, "pnlOptions");
			this.pnlOptions.Name = "pnlOptions";
			resources.ApplyResources(this.xtraTabControl1, "xtraTabControl1");
			this.xtraTabControl1.Name = "xtraTabControl1";
			this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
			this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.xtraTabPage1,
			this.xtraTabPage2});
			this.xtraTabPage1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.xtraTabPage1.Controls.Add(this.chbEmptyFields);
			this.xtraTabPage1.Controls.Add(this.chbCardCaption);
			this.xtraTabPage1.Controls.Add(this.chbAutoHorzWidth);
			this.xtraTabPage1.Controls.Add(this.label3);
			this.xtraTabPage1.Controls.Add(this.label1);
			this.xtraTabPage1.Controls.Add(this.chbSelectedCardOnly);
			this.xtraTabPage1.Controls.Add(this.label2);
			this.xtraTabPage1.Controls.Add(this.spinEdit1);
			this.xtraTabPage1.Name = "xtraTabPage1";
			resources.ApplyResources(this.xtraTabPage1, "xtraTabPage1");
			this.xtraTabPage1.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage1_Paint);
			resources.ApplyResources(this.chbEmptyFields, "chbEmptyFields");
			this.chbEmptyFields.Name = "chbEmptyFields";
			this.chbEmptyFields.Properties.Caption = resources.GetString("chbEmptyFields.Properties.Caption");
			this.chbEmptyFields.Tag = "1";
			this.chbEmptyFields.CheckStateChanged += new System.EventHandler(this.chbEmptyFields_CheckStateChanged);
			resources.ApplyResources(this.chbCardCaption, "chbCardCaption");
			this.chbCardCaption.Name = "chbCardCaption";
			this.chbCardCaption.Properties.Caption = resources.GetString("chbCardCaption.Properties.Caption");
			this.chbCardCaption.Tag = "0";
			this.chbCardCaption.CheckStateChanged += new System.EventHandler(this.chbCardCaption_CheckStateChanged);
			resources.ApplyResources(this.chbAutoHorzWidth, "chbAutoHorzWidth");
			this.chbAutoHorzWidth.Name = "chbAutoHorzWidth";
			this.chbAutoHorzWidth.Properties.Appearance.Options.UseTextOptions = true;
			this.chbAutoHorzWidth.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chbAutoHorzWidth.Properties.AutoHeight = ((bool)(resources.GetObject("chbAutoHorzWidth.Properties.AutoHeight")));
			this.chbAutoHorzWidth.Properties.Caption = resources.GetString("chbAutoHorzWidth.Properties.Caption");
			this.chbAutoHorzWidth.Tag = "3";
			this.chbAutoHorzWidth.CheckStateChanged += new System.EventHandler(this.chbAutoHorzWidth_CheckStateChanged);
			resources.ApplyResources(this.label3, "label3");
			this.label3.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label3.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label3.Name = "label3";
			resources.ApplyResources(this.label1, "label1");
			this.label1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label1.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label1.Name = "label1";
			resources.ApplyResources(this.chbSelectedCardOnly, "chbSelectedCardOnly");
			this.chbSelectedCardOnly.Name = "chbSelectedCardOnly";
			this.chbSelectedCardOnly.Properties.Appearance.Options.UseTextOptions = true;
			this.chbSelectedCardOnly.Properties.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.chbSelectedCardOnly.Properties.AutoHeight = ((bool)(resources.GetObject("chbSelectedCardOnly.Properties.AutoHeight")));
			this.chbSelectedCardOnly.Properties.Caption = resources.GetString("chbSelectedCardOnly.Properties.Caption");
			this.chbSelectedCardOnly.Tag = "2";
			this.chbSelectedCardOnly.CheckStateChanged += new System.EventHandler(this.chbSelectedCardOnly_CheckStateChanged);
			resources.ApplyResources(this.label2, "label2");
			this.label2.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label2.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label2.Name = "label2";
			resources.ApplyResources(this.spinEdit1, "spinEdit1");
			this.spinEdit1.Name = "spinEdit1";
			this.spinEdit1.Properties.Appearance.Options.UseTextOptions = true;
			this.spinEdit1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
			this.spinEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.spinEdit1.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
			this.spinEdit1.Properties.IsFloatValue = false;
			this.spinEdit1.Properties.Mask.EditMask = resources.GetString("spinEdit1.Properties.Mask.EditMask");
			this.spinEdit1.Properties.MaxValue = new decimal(new int[] {
			10,
			0,
			0,
			0});
			this.spinEdit1.Properties.MinValue = new decimal(new int[] {
			1,
			0,
			0,
			-2147483648});
			this.spinEdit1.EditValueChanged += new System.EventHandler(this.spinEdit1_EditValueChanged);
			this.spinEdit1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.spinEdit1_EditValueChanging);
			this.xtraTabPage2.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.xtraTabPage2.Controls.Add(this.chbDefaultStyles);
			this.xtraTabPage2.Controls.Add(this.label4);
			this.xtraTabPage2.Name = "xtraTabPage2";
			resources.ApplyResources(this.xtraTabPage2, "xtraTabPage2");
			this.xtraTabPage2.Paint += new System.Windows.Forms.PaintEventHandler(this.tabPage3_Paint);
			resources.ApplyResources(this.chbDefaultStyles, "chbDefaultStyles");
			this.chbDefaultStyles.Name = "chbDefaultStyles";
			this.chbDefaultStyles.Properties.Caption = resources.GetString("chbDefaultStyles.Properties.Caption");
			this.chbDefaultStyles.Tag = "4";
			this.chbDefaultStyles.CheckStateChanged += new System.EventHandler(this.chbDefaultStyles_CheckStateChanged);
			resources.ApplyResources(this.label4, "label4");
			this.label4.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label4.ForeColor = System.Drawing.SystemColors.HighlightText;
			this.label4.Name = "label4";
			this.groupControl1.Controls.Add(this.pnlGridPreview);
			resources.ApplyResources(this.groupControl1, "groupControl1");
			this.groupControl1.Name = "groupControl1";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "CardViewPrinting";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.pnlOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
			this.xtraTabControl1.ResumeLayout(false);
			this.xtraTabPage1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chbEmptyFields.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbCardCaption.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbAutoHorzWidth.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chbSelectedCardOnly.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.spinEdit1.Properties)).EndInit();
			this.xtraTabPage2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chbDefaultStyles.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.groupControl1)).EndInit();
			this.groupControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		private DevExpress.XtraGrid.GridControl GridPreview;
		public bool AutoApply = true;
		ImageCollection imCol;
		public CardViewPrinting() : base(9) {
			InitializeComponent();
			pnlGridPreview.DockPadding.All = 4;
			GridPreview = new PreviewCardGrid(pnlGridPreview);
			imCol = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraGrid.Printing.CardPrintOptions.png", typeof(CardViewDesigner).Assembly, new Size(16, 16));
		}
		protected override string DescriptionText { get { return GridLocalizer.Active.GetLocalizedString(GridStringId.PrintDesignerDescription); } }
		public override void InitComponent() {
			dv1 = new DataView(((DataTable)GridPreview.DataSource));
			dv2 = new DataView(((DataTable)GridPreview.DataSource));
			dv2.RowFilter = "[Product Name] = 'Tofu'";
			CurrentView.AppearancePrint.Assign(EditingView.AppearancePrint);
			CurrentView.OptionsPrint.Assign(EditingView.OptionsPrint);
			InitViewStyles(chbDefaultStyles.Checked);
			InitPrintOptions();
			InitPrintStates();
			dv1[1]["Discontinued"] = DBNull.Value;
			lbCaption.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.PrintDesignerCardView);;
			CurrentView.PaintStyleName = "Flat";
		}
		public virtual CardView CurrentView {
			get { return GridPreview.MainView as CardView; }
		}
		public virtual CardView EditingView {
			get { return EditingObject as CardView; }
		}
		BaseAppearanceCollection GetPrintAppearance(CardView view) {
			MethodInfo method = view.GetType().GetMethod("CreatePrintInfoCore", BindingFlags.NonPublic | BindingFlags.Instance);
			BaseViewPrintInfo pi = method.Invoke(view, new object[] { new PrintInfoArgs(view)}) as BaseViewPrintInfo;
			return pi.AppearancePrint ;
		}
		private DataView dv1, dv2;
		private void InitViewStyles(bool IsPrintStyles) {
			CurrentView.BeginUpdate();
			try {
				CurrentView.Appearance.Reset();
				if(IsPrintStyles) {
					CardViewPrintAppearances collection = GetPrintAppearance(CurrentView) as CardViewPrintAppearances;
					if(collection != null) 
						CurrentView.Appearance.Assign(collection);
				}
				else 
					CurrentView.Appearance.Assign(EditingView.PaintAppearance);
				CurrentView.Appearance.FocusedCardCaption.Assign(CurrentView.PaintAppearance.CardCaption);
				CurrentView.Appearance.HideSelectionCardCaption.Assign(CurrentView.PaintAppearance.CardCaption);
				CurrentView.Appearance.EmptySpace.BackColor = CurrentView.Appearance.EmptySpace.BackColor2 = Color.White;
			} finally {
				CurrentView.EndUpdate();
			}
		}
		private string[] printFlags = new string[] {
			"PrintCardCaption", "PrintEmptyFields", 
			"PrintSelectedCardsOnly", "AutoHorzWidth", "UsePrintStyles" 
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
			spinEdit1.EditValue = EditingView.PrintMaximumCardColumns;
			lockApply++;
		}
		private void InitPrintStates() {
			chbAutoHorzWidth_CheckStateChanged(chbAutoHorzWidth, EventArgs.Empty);
			chbCardCaption_CheckStateChanged(chbCardCaption, EventArgs.Empty);
			chbDefaultStyles_CheckStateChanged(chbDefaultStyles, EventArgs.Empty);
			chbEmptyFields_CheckStateChanged(chbEmptyFields, EventArgs.Empty);
			chbSelectedCardOnly_CheckStateChanged(chbSelectedCardOnly, EventArgs.Empty);
		}
		#endregion
		#region Editing
		private CheckEdit CheckEditByIndex(int index) {
			foreach(XtraTabPage tp in xtraTabControl1.TabPages) 
				foreach(object o in tp.Controls) 
					if(o is CheckEdit && ((CheckEdit)o).Tag.ToString() == index.ToString())
						return o as CheckEdit;
			return null;
		}
		private void spinEdit1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e) {
			if(e.NewValue != null && e.OldValue != null) {
				string oldV = e.OldValue.ToString(), newV = e.NewValue.ToString();
				if(newV == "0") {
					if(oldV == "-1") e.NewValue = "1";
					else e.NewValue = "-1";
				}
			}
		}
		private void spinEdit1_EditValueChanged(object sender, EventArgs e) {
			CurrentView.MaximumCardColumns = Convert.ToInt32(spinEdit1.Value);
			ApplyOptions();
		}
		private void chbCardCaption_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			CurrentView.OptionsView.ShowCardCaption = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbEmptyFields_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			CurrentView.OptionsView.ShowEmptyFields = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbSelectedCardOnly_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			GridPreview.DataSource = (chb.Checked ? dv2 : dv1);
			InvalidateImage(sender);
		}
		private void chbAutoHorzWidth_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			CurrentView.OptionsBehavior.AutoHorzWidth = chb.Checked;
			InvalidateImage(sender);
		}
		private void chbDefaultStyles_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			InvalidateImage(sender);
			InitViewStyles(chb.Checked);
		}
		#endregion
		#region Images paint
		int x = 10;
		private void InvalidateImage(object checkBox ) {
			CheckEdit chb = checkBox as CheckEdit;
			((XtraTabPage)chb.Parent).Invalidate(new Rectangle(0, chb.Top, 50, chb.Height));
			ApplyOptions();
		} 
		private void tabPage1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			for(int i = 0; i < 4; i++) 
				PrintImage(i, e.Graphics);
		}
		private void tabPage3_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			PrintImage(4, e.Graphics);
		}
		private void PrintImage(int i, Graphics g) {
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
		#endregion
		#region IPrintDesigner
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
			EditingView.PrintMaximumCardColumns = CurrentView.MaximumCardColumns;	
			EditingView.FireChanged();
		}
		private void ApplyOptions() {
			ApplyOptions(AutoApply);
		}
		public void HideCaption() {
			lbCaption.Visible = horzSplitter.Visible = false;
		}
		#endregion
		#region UserControlSize
		public Size UserControlSize {
			get { return DevExpress.Utils.ScaleUtils.GetScaleSize(new Size(560, 345)); }
		}
		#endregion
	}
}
