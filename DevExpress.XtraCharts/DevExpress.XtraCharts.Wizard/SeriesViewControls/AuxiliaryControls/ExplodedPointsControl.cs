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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls {
	internal partial class ExplodedPointsControl : ChartUserControl {
		PieSeriesViewBase view;
		SeriesBase series;
		public ExplodedPointsControl() {
			InitializeComponent();
		}
		public void Initialize(PieSeriesViewBase view, SeriesBase series) {
			this.view = view;
			this.series = series;
			txtDistance.EditValue = view.ExplodedDistancePercentage;
			UpdateControls();
		}
		void UpdateControls() {
			this.cbExplodeMode.SelectedIndex = (int)view.ExplodeMode;
			beExplodedPoints.Visible = series is Series;
			chartPanelControl3.Visible = series is Series;
			beFilters.Visible = view.ExplodeMode == PieExplodeMode.UseFilters;
			pnlFilters.Visible = view.ExplodeMode == PieExplodeMode.UseFilters;
		}
		void cbExplodeMode_SelectedIndexChanged(object sender, EventArgs e) {
			view.ExplodeMode = (PieExplodeMode)cbExplodeMode.SelectedIndex;
			UpdateControls();
		}
		void beExplodedPoints_ButtonClick(object sender, ButtonPressedEventArgs e) {
			using (var form = new ExplodedPointsListForm((Series)series, view.ExplodedPoints)) {
				form.ShowDialog();
			}
			UpdateControls();
		}
		void beFilters_ButtonClick(object sender, ButtonPressedEventArgs e) {
			using (SeriesPointFilterCollectionForm form = new SeriesPointFilterCollectionForm(view.ExplodedPointsFilters)) {
				form.ShowDialog();
			}	
		}
		void txtDistance_EditValueChanged(object sender, EventArgs e) {
			view.ExplodedDistancePercentage = Convert.ToDouble(txtDistance.EditValue);
		}
	}
}
