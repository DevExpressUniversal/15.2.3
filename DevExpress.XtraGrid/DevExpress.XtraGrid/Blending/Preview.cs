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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Data;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Card;
using DevExpress.XtraGrid.Design;
using DevExpress.XtraGrid.Columns;
namespace DevExpress.XtraGrid.Blending {
	public class Preview : System.Windows.Forms.Form {
		private System.Windows.Forms.Panel panel1;
		private System.ComponentModel.Container components = null;
		private DevExpress.XtraEditors.SimpleButton btnEditor;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preview));
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnEditor = new DevExpress.XtraEditors.SimpleButton();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			this.panel1.Controls.Add(this.btnEditor);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(476, 36);
			this.panel1.TabIndex = 1;
			this.btnEditor.Image = ((System.Drawing.Image)(resources.GetObject("btnEditor.Image")));
			this.btnEditor.Location = new System.Drawing.Point(4, 4);
			this.btnEditor.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnEditor.Name = "btnEditor";
			this.btnEditor.Size = new System.Drawing.Size(88, 24);
			this.btnEditor.TabIndex = 0;
			this.btnEditor.Text = "Editor...";
			this.btnEditor.Click += new System.EventHandler(this.btnEditor_Click);
			this.ClientSize = new System.Drawing.Size(476, 369);
			this.Controls.Add(this.panel1);
			this.Name = "Preview";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Preview";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		#region Init & Ctor
		XtraGridBlending blending;
		GridControl gridPreview = null;
		private GridView gridView { get { return gridPreview.MainView as GridView; } }
		private CardView cardView { get { return gridPreview.MainView as CardView; } }
		public Preview() : this(null) {}
		public Preview(XtraGridBlending blending) {
			InitializeComponent();
			gridPreview = new GridControl();
			gridPreview.Dock = DockStyle.Fill;
			this.Controls.Add(gridPreview);
			gridPreview.BringToFront();
			gridPreview.ForceInitialize();
			if(blending == null) return;
			this.blending = blending;
			if(blending.GridControl != null) {
				DevExpress.XtraGrid.Views.Base.BaseView oldView = gridPreview.MainView;
				gridPreview.MainView = gridPreview.CreateView(blending.GridControl.MainView.BaseInfo.ViewName);
				if(oldView != null) oldView.Dispose();
			}
			new DevExpress.XtraGrid.Design.XViewsPrinting(gridPreview, true);
			if(blending.GridControl != null) {
				DevExpress.XtraGrid.Design.GridAssign.SetAppearances(blending.GridControl, gridPreview);
				if(blending.GridControl.BackgroundImage != null)
					gridPreview.BackgroundImage = (Image)blending.GridControl.BackgroundImage.Clone();
				gridPreview.LookAndFeel.Assign(blending.GridControl.LookAndFeel);
				gridPreview.MainView.PaintStyleName = blending.GridControl.MainView.PaintStyleName;
			}
			gridPreview.OldAlphaBlending = blending;
			RefreshAppearances();
			if(gridPreview.MainView is GridView) InitGridView();
			if(gridPreview.MainView is CardView) InitCardView();
		}
		void OnRefresh(object sender, EventArgs e) {
			RefreshAppearances();
		}
		void RefreshAppearances() {
			if(gridPreview.MainView.ViewInfo != null) gridPreview.MainView.ViewInfo.SetPaintAppearanceDirty();
			gridPreview.MainView.LayoutChanged();
		}
		private void InitGridView() {
			if(blending.GridControl != null) {
				GridView gv = blending.GridControl.MainView as GridView;
				gridView.OptionsView.Assign(gv.OptionsView); 
				gridView.BorderStyle = gv.BorderStyle;
				gridView.RowSeparatorHeight = gv.RowSeparatorHeight;
			}
			gridView.CalcPreviewText += new DevExpress.XtraGrid.Views.Grid.CalcPreviewTextEventHandler(CalcPreviewText);
			gridView.GroupSummary.Add(SummaryItemType.Count, "Product Name", gridView.Columns["Product Name"]);
			gridView.GroupSummary.Add(SummaryItemType.Max, "Unit Price", gridView.Columns["Unit Price"]);
			GridColumn col = gridView.Columns["Unit Price"];
			gridView.GroupSummary.Add(SummaryItemType.Max, "Unit Price", col, gridView.GetSummaryFormat(col, SummaryItemType.Max), col.DisplayFormat.Format);
			gridView.Columns["Discontinued"].GroupIndex = 0;
			gridView.ExpandAllGroups();
		}
		private void InitCardView() {
			if(blending.GridControl != null) {
				CardView cv = blending.GridControl.MainView as CardView;
				cardView.OptionsView.Assign(cv.OptionsView); 
				cardView.BorderStyle = cv.BorderStyle;
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
		#endregion
		#region Editing
		private void CalcPreviewText(object sender, DevExpress.XtraGrid.Views.Grid.CalcPreviewTextEventArgs e) {
			if(e.RowHandle >= 0) {
				System.Data.DataRow row = gridView.GetDataRow(e.RowHandle);
				e.PreviewText = string.Format(Properties.Resources.PreviewRowDescription, row["Product Name"]);	
			}
		}
		private void btnEditor_Click(object sender, System.EventArgs e) {
			AlphaStyleEditor editor = new AlphaStyleEditor(blending.AlphaStyles, blending.Site, gridPreview);
			editor.RefreshStyles += new EventHandler(OnRefresh);
			editor.ShowDialog();
			editor.RefreshStyles -= new EventHandler(OnRefresh);
		}
		#endregion
	}
}
