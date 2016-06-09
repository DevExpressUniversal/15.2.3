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
using DevExpress.Data;
using DevExpress.XtraGrid;
using DevExpress.Utils;
using DevExpress.Utils.Frames;
using DevExpress.XtraGrid.Design;
using DevExpress.XtraEditors;
using DevExpress.Utils.Design;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Localization;
namespace DevExpress.XtraGrid.Frames {
	[ToolboxItem(false)]
	public class BandedViewPrinting : GridViewPrinting, IPrintDesigner {
		#region Init & Ctor
		const int bandImageIndex = 14;
		protected override string ViewType { get { return "BandedGridView"; } }
		public BandedViewPrinting() : base() {
			InitBandedGridViewOptions((BandedGridView)GridPreview.MainView);
			InitNewProperty();
		}
		private void InitBandedGridViewOptions(BandedGridView view) {
			view.CustomDrawBandHeader += new DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventHandler(this.view_CustomDrawBandHeader);
		}
		private void view_CustomDrawBandHeader(object sender, DevExpress.XtraGrid.Views.BandedGrid.BandHeaderCustomDrawEventArgs e) {
			GridPreview.DrawBrick(e.Bounds, e.Appearance, e.Cache, e.Graphics, e.Info.Caption, 0, 0);
			e.Handled = true;
		}
		public override void InitComponent() {
			base.InitComponent();
			lbCaption.Text = GridLocalizer.Active.GetLocalizedString(GridStringId.PrintDesignerBandedView);
		}
		private CheckEdit chbBand;
		private void InitNewProperty() {
			chbBand = new CheckEdit();
			for(int i = 0; i < 8; i++) {
				ShiftCheckEdit(i);
			}
			ShiftCheckEdit(12);
			ShiftCheckEdit(13);
			ShiftCheckEdit(15);
			chbBand.Tag = bandImageIndex.ToString();
			chbBand.Location = new System.Drawing.Point(36, 28);
			chbBand.Size = new System.Drawing.Size(96, 20);
			chbBand.TabIndex = 0;
			chbBand.Properties.Caption = GridLocalizer.Active.GetLocalizedString(GridStringId.PrintDesignerBandHeader);
			xtraTabPage1.Controls.Add(chbBand);
			chbBand.CheckStateChanged += new System.EventHandler(this.chbBand_CheckStateChanged);
		}
		void ShiftCheckEdit(int index) {
			CheckEdit chb = CheckEditByIndex(index);
			if(chb != null) 
				chb.Top += 20;
		}
		#endregion
		#region Override
		protected override void InitViewStyles(bool IsPrintStyles) {
			base.InitViewStyles(IsPrintStyles);
			if(IsPrintStyles) {
				((BandedGridView)CurrentView).Appearance.BandPanel.BorderColor = ((BandedGridView)CurrentView).Appearance.BandPanel.BackColor;
			} 
		}
		protected override void InitPrintStates() {
			base.InitPrintStates();
			chbBand_CheckStateChanged(chbBand, EventArgs.Empty);
		}
		protected override void tabPage1_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			base.tabPage1_Paint(sender, e);
			PrintImage(bandImageIndex, e.Graphics);
		}
		#endregion
		#region Editing
		private void chbBand_CheckStateChanged(object sender, System.EventArgs e) {
			CheckEdit chb = sender as CheckEdit;
			((BandedGridView)CurrentView).OptionsView.ShowBands = chb.Checked;
			InvalidateImage(sender);
		}
		#endregion
	}
}
