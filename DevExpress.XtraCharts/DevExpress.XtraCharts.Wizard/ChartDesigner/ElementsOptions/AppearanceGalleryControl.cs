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
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Designer.Native {
	public partial class AppearanceGalleryControl : UserControl {
		ChartAppearance appearance;
		DesignerChartModel chartModel;
		PaletteRepository paletteRepository;
		AppearanceRepository appearanceRepository;
		int currentPaletteIndex;
		public event EventHandler AppearanceChanged;
		Chart Chart { get { return chartModel == null ? null : chartModel.ChartElement as Chart; } }
		public ChartAppearanceModel Appearance {
			get {
				return new ChartAppearanceModel() { Appearance = appearance, CurrentPaletteIndex = currentPaletteIndex };
			}
			set {
				if (appearance == null || !appearance.Equals(value)) {
					appearance = value.Appearance;
					currentPaletteIndex = value.CurrentPaletteIndex;
					OnAppearanceChanged();
				}
			}
		}
		ChartAppearance ChartAppearance {
			get {
				return appearance;
			}
			set {
				appearance = value;
				OnAppearanceChanged();
			}
		}
		int CurrentPaletteIndex {
			get {
				return currentPaletteIndex;
			}
			set {
				currentPaletteIndex = value;
			}
		}
		public AppearanceGalleryControl() {
			InitializeComponent();
		}
		void OnAppearanceChanged() {
			if (AppearanceChanged != null)
				AppearanceChanged(this, new EventArgs());
			UpdateText();
		}
		void stylesContainer_OnEditValueChanged(object sender, EventArgs e) {
			CurrentPaletteIndex = stylesContainer.CurrentPaletteIndex;
			ChartAppearance = stylesContainer.CurrentAppearance;
		}
		void stylesContainer_OnNeedClose(object sender, EventArgs e) {
		}
		void DisposeAppearanceImages() {
			foreach (Control control in stylesContainer.Controls) {
				PictureEdit pictureEdit = control as PictureEdit;
				if (pictureEdit != null && pictureEdit.Image != null)
					pictureEdit.Image.Dispose();
				control.Dispose();
			}
			stylesContainer.Controls.Clear();
		}
		void FillAppearances() {
			DisposeAppearanceImages();
			ViewType viewType = ViewType.Bar;
			Diagram diagram = Chart.Diagram;
			if (diagram != null) {
				IList<ISeries> series = Chart.ViewController.GetISeriesForLegendList();
				if (series.Count > 0)
					viewType = SeriesViewFactory.GetViewType(((Series)series[0]).View);
			}
			stylesContainer.SuspendLayout();
			try {
				stylesContainer.SetPaletteRepository(paletteRepository);
				stylesContainer.SetAppearancesCount(appearanceRepository.Names.Length);
				foreach (ChartAppearance appearance in appearanceRepository)
					if (!Chart.Palette.Predefined || appearance == Chart.Appearance || String.IsNullOrEmpty(appearance.PaletteName) || appearance.PaletteName == Chart.Palette.Name)
						stylesContainer.RegisterAppearance(appearance, viewType, Chart.Palette);
			}
			finally {
				stylesContainer.ResumeLayout();
			}
		}
		void popupContainerEdit1_QueryPopUp(object sender, CancelEventArgs e) {
			FillAppearances();
			ChartAppearance previousAppearance = Chart.Appearance;
			int previousPaletteIndex = Chart.PaletteBaseColorNumber;
			stylesContainer.SelectStyle(previousAppearance, previousPaletteIndex);
		}
		void popupContainerEdit1_QueryCloseUp(object sender, CancelEventArgs e) {
			e.Cancel = true;
		}
		bool ShouldSerializeAppearance() {
			return appearance != null;
		}
		public void Initialize(DesignerChartModel chartModel) {
			this.chartModel = chartModel;
			try {
				appearanceRepository = Chart.AppearanceRepository;
				paletteRepository = Chart.PaletteRepository;
			}
			finally {
			}
		}
		public void UpdateText() {
			if (Chart != null) {
				string colorIndexTip = Chart.PaletteBaseColorNumber == 0 ?
					ChartLocalizer.GetString(ChartStringId.StyleAllColors) :
					String.Format(ChartLocalizer.GetString(ChartStringId.StyleColorNumber), Chart.PaletteBaseColorNumber);
				popupAppearanceControl.EditValue = String.Format("{0} ({1})", Chart.AppearanceName, colorIndexTip);
			}
		}
	}
	public struct ChartAppearanceModel {
		public ChartAppearance Appearance;
		public int CurrentPaletteIndex;
	}
}
