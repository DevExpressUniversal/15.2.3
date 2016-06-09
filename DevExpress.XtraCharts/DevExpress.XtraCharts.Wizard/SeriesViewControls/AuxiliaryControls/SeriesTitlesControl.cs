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
using DevExpress.XtraCharts.Wizard.ChartAxesControls;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesViewControls.AuxiliaryControls {
	internal partial class SeriesTitlesControl : ChartUserControl {
		SimpleDiagramSeriesViewBase view;
		Chart chart;
		public SeriesTitlesControl() {
			InitializeComponent();
			seriesTitleListRedactControl1.SelectedElementChanged += new SelectedElementChangedEventHandler(seriesTitleListRedactControl1_SelectedElementChanged);
		}
		void  seriesTitleListRedactControl1_SelectedElementChanged() {
			SeriesTitle title = (SeriesTitle)this.seriesTitleListRedactControl1.CurrentElement;
			if (title != null) {
				this.titleGeneralControl.Initialize(title, null);
				if (view != null)
					chart.SelectHitElement(title);
			}
			txtText.EditValue = title != null ? title.Text : "";
			UpdateControls();
		}
		public void Initialize(SimpleDiagramSeriesViewBase view, Chart chart) {
			this.view = view;			
			this.chart = chart;
			seriesTitleListRedactControl1.Initialize(view.Titles);
			UpdateControls();
		}
		public void SelectTitle(SeriesTitle title) {
			seriesTitleListRedactControl1.CurrentElement = title;
		}
		void UpdateControls() {
			this.txtText.Enabled = view.Titles.Count > 0;
			this.titleGeneralControl.Enabled = view.Titles.Count > 0;
		}
		private void txtText_EditValueChanged(object sender, EventArgs e) {
			SeriesTitle title = (SeriesTitle)seriesTitleListRedactControl1.CurrentElement;
			if(title == null)
				return;
			title.Text = txtText.EditValue.ToString();
			seriesTitleListRedactControl1.UpdateList();
		}
	}
}
