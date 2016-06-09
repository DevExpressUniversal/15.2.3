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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts.Designer.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class PaletteGalleryControl : UserControl {
		DesignerChartModel chartModel;
		Palette palette;
		PaletteRepository paletteRepository;
		AppearanceRepository appearanceRepository;
		Chart Chart { get { return chartModel == null ? null : chartModel.ChartElement as Chart; } }
		public event EventHandler PaletteChanged;
		public Palette Palette {
			get {
				return this.palette;
			}
			set {
				if (value != null &&
					((value == null && palette != null) || (value != null && palette == null) || !palette.Equals(value))) {
					palette = value;
					OnPaletteChanged();
				}
			}
		}
		public PaletteGalleryControl() {
			InitializeComponent();
		}
		public void Initialize(DesignerChartModel chartModel) {
			this.chartModel = chartModel;
			try {
				paletteEditControl.LookAndFeel.UseDefaultLookAndFeel = false;
				appearanceRepository = Chart.AppearanceRepository;
				paletteRepository = Chart.PaletteRepository;
				paletteEditControl.Chart = Chart;
				paletteEdit.SetPaletteRepository(paletteRepository);
				paletteEdit.Text = Chart.Palette.DisplayName;
			}
			finally { }
		}
		void OnPaletteChanged() {
			if (Palette != null) {
				paletteEdit.EditValue = Palette.DisplayName;
				if (PaletteChanged != null)
					PaletteChanged(this, new EventArgs());
			}
		}
		void paletteEdit_EditValueChanged(object sender, EventArgs e) {
			Palette = paletteRepository[paletteEdit.Text];
		}
		void paletteEdit_QueryPopUp(object sender, CancelEventArgs e) {
			paletteEditControl.SelectedPalette = Chart.Palette;
		}
		void paletteEdit_Closed(object sender, XtraEditors.Controls.ClosedEventArgs e) {
		}
		void paletteEditControl_OnNeedClose(object sender, EventArgs e) {
			paletteEdit.ClosePopup();
		}
		void paletteEditControl_OnPaletteChanged(object sender, EventArgs e) {
			paletteEdit.EditValue = paletteEditControl.SelectedPalette.DisplayName;
		}
	}
	public class DesignerPaletteEditControl : PaletteEditControl {
		internal override void OnSelectedPaletteChanged() {
			RaisePaletteChanged();
		}
	}
}
