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
namespace DevExpress.XtraTreeList.Blending {
	public class Preview : System.Windows.Forms.Form {
		private System.Windows.Forms.Panel panel1;
		private System.ComponentModel.Container components = null;
		XtraTreeListBlending blending;
		private DevExpress.XtraEditors.SimpleButton btnEditor;
		private DevExpress.XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit1;
		private DevExpress.XtraTreeList.TreeList treePreview;
		DevExpress.XtraTreeList.Design.XViews XV;
		Preview() { InitializeComponent(); } 
		public Preview(XtraTreeListBlending blending) {
			InitializeComponent();
			((Bitmap)btnEditor.Image).MakeTransparent();
			this.blending = blending;
			XV = new DevExpress.XtraTreeList.Design.XViews(treePreview);
			if(blending.TreeListControl != null) {
				treePreview.BeginUpdate();
				treePreview.Appearance.Assign(blending.TreeListControl.Appearance);
				if(blending.TreeListControl.BackgroundImage != null)
					treePreview.BackgroundImage = (Image)blending.TreeListControl.BackgroundImage.Clone();
				treePreview.LookAndFeel.Assign(blending.TreeListControl.LookAndFeel);
				treePreview.EndUpdate();
			}
			InitView();
			blending.RefreshPaintAppearance(treePreview);
		}
		private void InitView() {
			if(blending.TreeListControl != null) {
				treePreview.OptionsView.Assign(blending.TreeListControl.OptionsView); 
			}
			DevExpress.XtraTreeList.Columns.TreeListColumn col = treePreview.Columns[1];
			col.SummaryFooter = SummaryItemType.Sum;
			col.SummaryFooterStrFormat = "Sum = {0:c}";
			col.RowFooterSummary = SummaryItemType.Sum;
			col.RowFooterSummaryStrFormat = "{0:c}";
			col.AllNodesSummary = true; 
			col = treePreview.Columns[0];
			col.SummaryFooter = SummaryItemType.Count;
			col.RowFooterSummary = SummaryItemType.Count;
			col.AllNodesSummary = true;
			treePreview.GetPreviewText += new GetPreviewTextEventHandler(GetPreviewText);
		}
		private void GetPreviewText(object sender, GetPreviewTextEventArgs e) {
			if(e.Node != null && Convert.ToDouble(e.Node[1]) > 1000000)
				e.PreviewText = "This is a description for the " + e.Node[0].ToString() + " department";	
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Preview));
			this.panel1 = new System.Windows.Forms.Panel();
			this.btnEditor = new DevExpress.XtraEditors.SimpleButton();
			this.treePreview = new DevExpress.XtraTreeList.TreeList();
			this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.treePreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).BeginInit();
			this.SuspendLayout();
			this.panel1.Controls.Add(this.btnEditor);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(440, 32);
			this.panel1.TabIndex = 1;
			this.btnEditor.Image = ((System.Drawing.Image)(resources.GetObject("btnEditor.Image")));
			this.btnEditor.Location = new System.Drawing.Point(4, 4);
			this.btnEditor.LookAndFeel.UseDefaultLookAndFeel = false;
			this.btnEditor.Name = "btnEditor";
			this.btnEditor.Size = new System.Drawing.Size(88, 24);
			this.btnEditor.TabIndex = 0;
			this.btnEditor.Text = "Editor...";
			this.btnEditor.Click += new System.EventHandler(this.btnEditor_Click);
			this.treePreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treePreview.Location = new System.Drawing.Point(0, 32);
			this.treePreview.Name = "treePreview";
			this.treePreview.Size = new System.Drawing.Size(440, 245);
			this.treePreview.TabIndex = 2;
			this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
			this.ClientSize = new System.Drawing.Size(440, 277);
			this.Controls.Add(this.treePreview);
			this.Controls.Add(this.panel1);
			this.Name = "Preview";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Preview";
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.treePreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private void btnEditor_Click(object sender, System.EventArgs e) {
			AlphaStyleEditor editor = new AlphaStyleEditor(blending.AlphaStyles, blending.Site, treePreview);
			editor.ShowDialog();
		}
	}
}
