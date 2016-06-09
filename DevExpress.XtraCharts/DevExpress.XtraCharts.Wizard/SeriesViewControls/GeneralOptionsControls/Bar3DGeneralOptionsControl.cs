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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class Bar3DGeneralOptionsControl : ChartUserControl {
		struct Bar3DModelItem {
			readonly Bar3DModel model;
			readonly string text;
			public Bar3DModel Model { get { return model; } }
			public Bar3DModelItem(Bar3DModel model) {
				this.model = model;
				switch (model) {
					case Bar3DModel.Box:
						text = ChartLocalizer.GetString(ChartStringId.WizBar3DModelBox);
						break;
					case Bar3DModel.Cylinder:
						text = ChartLocalizer.GetString(ChartStringId.WizBar3DModelCylinder);
						break;
					case Bar3DModel.Cone:
						text = ChartLocalizer.GetString(ChartStringId.WizBar3DModelCone);
						break;
					case Bar3DModel.Pyramid:
						text = ChartLocalizer.GetString(ChartStringId.WizBar3DModelPyramid);
						break;
					default:
						ChartDebug.Fail("Unknown bar model.");
						text = ChartLocalizer.GetString(ChartStringId.WizBar3DModelBox);
						break;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is Bar3DModelItem) && model == ((Bar3DModelItem)obj).model;
			}
			public override int GetHashCode() {
				return model.GetHashCode();
			}
		}
		Bar3DSeriesView view;
		ISideBySideBarSeriesView sideBySideView;
		public Bar3DGeneralOptionsControl() {
			InitializeComponent();
		}
		public void Initialize(Bar3DSeriesView view) {
			this.view = view;
			sideBySideView = view as ISideBySideBarSeriesView;
			pnlDistance.Visible = sideBySideView != null;
			sepDistance.Visible = sideBySideView != null;
			pnlDistanceFixed.Visible = sideBySideView != null;
			sepDistanceFixed.Visible = sideBySideView != null;
			chEqualBarWidth.Visible = sideBySideView != null;
			InitializeControls();
		}
		void InitializeControls() {
			chBarDepthAuto.Checked = view.BarDepthAuto;
			txtDepth.EditValue = view.BarDepth;
			txtWidth.EditValue = view.BarWidth;
			chShowFacet.Checked = view.ShowFacet;
			cbModel.Properties.Items.AddRange(new Bar3DModelItem[] { new Bar3DModelItem(Bar3DModel.Box), 
				new Bar3DModelItem(Bar3DModel.Cylinder), new Bar3DModelItem(Bar3DModel.Cone), new Bar3DModelItem(Bar3DModel.Pyramid) });
			cbModel.SelectedItem = new Bar3DModelItem(view.Model);
			if(sideBySideView != null) {
				chEqualBarWidth.Checked = sideBySideView.EqualBarWidth;
				txtDistance.EditValue = sideBySideView.BarDistance * 100.0;
				txtDistanceFixed.EditValue = sideBySideView.BarDistanceFixed;
			}
		}
		void chBarDepthAuto_CheckedChanged(object sender, EventArgs e) {
			view.BarDepthAuto = chBarDepthAuto.Checked;
			pnlDepth.Enabled = !view.BarDepthAuto;
		}
		void chEqualBarWidth_CheckedChanged(object sender, EventArgs e) {
			if(sideBySideView != null)
				sideBySideView.EqualBarWidth = chEqualBarWidth.Checked;
		}
		void txtDepth_EditValueChanged(object sender, EventArgs e) {
			view.BarDepth = Convert.ToDouble(txtDepth.EditValue);
		}
		void txtWidth_EditValueChanged(object sender, EventArgs e) {
			view.BarWidth = Convert.ToDouble(txtWidth.EditValue);
		}
		void txtDistance_EditValueChanged(object sender, EventArgs e) {
			if(sideBySideView != null)
				sideBySideView.BarDistance = Convert.ToDouble(txtDistance.EditValue) / 100.0;
		}
		void txtDistanceFixed_EditValueChanged(object sender, EventArgs e) {
			if(sideBySideView != null)
				sideBySideView.BarDistanceFixed = Convert.ToInt32(txtDistanceFixed.EditValue);
		}
		void chShowFacet_CheckedChanged(object sender, EventArgs e) {
			view.ShowFacet = chShowFacet.Checked;
		}
		void cbModel_SelectedIndexChanged(object sender, EventArgs e) {
			view.Model = ((Bar3DModelItem)cbModel.SelectedItem).Model;
			chShowFacet.Enabled = view.Model == Bar3DModel.Box || view.Model == Bar3DModel.Cylinder;
		}
	}
}
