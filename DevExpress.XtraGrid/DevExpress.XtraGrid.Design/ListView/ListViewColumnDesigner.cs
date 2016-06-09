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

using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.XtraGrid.Design.Properties;
using DevExpress.XtraGrid.Frames;
namespace DevExpress.XtraGrid.Design.ListView {
	public class ListViewColumnDesigner : ColumnDesigner {
		private XtraEditors.LabelControl lblLinkToLayoutPage;
		public ListViewColumnDesigner() {
			InitializeComponent();
		}
		public override void InitComponent() {
			base.InitComponent();
			lblLinkToLayoutPage.Font = DescriptionPanel.DescriptionDefaultFont;
			lblLinkToLayoutPage.Text = Resources.WinExplorerViewLinkToLayoutPageText;
		}
		void OnLinkToLayoutPageClick(object sender, HyperlinkClickEventArgs e) {
			TryNavigateToLayoutModule();
		}
		protected virtual void TryNavigateToLayoutModule() {
			if(GridInfo != null && GridInfo.ModuleNavigator != null)
				GridInfo.ModuleNavigator.NavigateTo(typeof(ListViewLayout));
		}
		protected EditingGridInfo GridInfo { get { return InfoObject as EditingGridInfo; } }
		protected override string GroupControlColumnsText {
			get { return Resources.WinExplorerViewColumnsCaption; }
		}
		protected override string DescriptionText {
			get { return Resources.WinExplorerViewColumnDesignerDescription; }
		}
		#region Initialize Component
		void InitializeComponent() {
			this.lblLinkToLayoutPage = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.groupControlFields)).BeginInit();
			this.buttonPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			this.pnlControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			this.SuspendLayout();
			this.btnDown.Location = new System.Drawing.Point(136, 11);
			this.btnUp.Location = new System.Drawing.Point(102, 11);
			this.btRetrieve.Location = new System.Drawing.Point(34, 42);
			this.btRemove.Location = new System.Drawing.Point(68, 11);
			this.btAdd.Location = new System.Drawing.Point(0, 11);
			this.btInsert.Location = new System.Drawing.Point(34, 11);
			this.chbFieldList.Location = new System.Drawing.Point(0, 42);
			this.groupControlFields.Size = new System.Drawing.Size(267, 445);
			this.buttonPanel.Location = new System.Drawing.Point(272, 31);
			this.buttonPanel.Size = new System.Drawing.Size(193, 49);
			this.splMain.Location = new System.Drawing.Point(432, 139);
			this.splMain.Size = new System.Drawing.Size(5, 445);
			this.pgMain.Location = new System.Drawing.Point(437, 139);
			this.pgMain.Size = new System.Drawing.Size(391, 445);
			this.pnlControl.Controls.Add(this.lblLinkToLayoutPage);
			this.pnlControl.Location = new System.Drawing.Point(0, 46);
			this.pnlControl.Size = new System.Drawing.Size(828, 93);
			this.pnlControl.Controls.SetChildIndex(this.buttonPanel, 0);
			this.pnlControl.Controls.SetChildIndex(this.btRetrieve, 0);
			this.pnlControl.Controls.SetChildIndex(this.chbFieldList, 0);
			this.pnlControl.Controls.SetChildIndex(this.lblLinkToLayoutPage, 0);
			this.lbCaption.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 34F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
			this.lbCaption.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Bottom;
			this.pnlMain.Location = new System.Drawing.Point(272, 139);
			this.pnlMain.Size = new System.Drawing.Size(160, 445);
			this.lblLinkToLayoutPage.AllowHtmlString = true;
			this.lblLinkToLayoutPage.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
			this.lblLinkToLayoutPage.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.lblLinkToLayoutPage.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.lblLinkToLayoutPage.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblLinkToLayoutPage.Location = new System.Drawing.Point(4, 4);
			this.lblLinkToLayoutPage.Name = "lblLinkToLayoutPage";
			this.lblLinkToLayoutPage.Size = new System.Drawing.Size(820, 33);
			this.lblLinkToLayoutPage.TabIndex = 9;
			this.lblLinkToLayoutPage.Text = "(link)";
			this.lblLinkToLayoutPage.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(this.OnLinkToLayoutPageClick);
			this.Name = "ListViewColumnDesigner";
			((System.ComponentModel.ISupportInitialize)(this.groupControlFields)).EndInit();
			this.buttonPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			this.pnlControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
	}
}
