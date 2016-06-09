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
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class ChartAppearanceControl : InternalWizardControlBase {
		public static string Title { get { return "Appearance"; } }
		Chart chart;
		Chart originalChartObj;
		ChartDesignControl designControl;
		PaletteRepository paletteRepository;
		AppearanceRepository appearanceRepository;
		ChartAppearance previousAppearance;
		Palette previousPalette;
		int previousPaletteIndex;
		bool lockChanges = false;
		public ChartAppearanceControl() {
			InitializeComponent();
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			lockChanges = true;
			try {
				chart = form.Chart;
				paletteEditControl.Chart = chart;
				paletteEditControl.SetLookAndFeel(form.LookAndFeel);
				paletteEditControl.LookAndFeel.UseDefaultLookAndFeel = false;
				stylesContainer.LookAndFeel.ParentLookAndFeel = form.LookAndFeel;
				designControl = form.DesignControl;
				originalChartObj = form.OriginalChart;
				appearanceRepository = chart.AppearanceRepository;
				paletteRepository = chart.PaletteRepository;
				panelControl1.Controls.Add(designControl);
				designControl.Dock = DockStyle.Fill;
				designControl.Visible = true;
				paletteEditControl.Chart = chart;
				paletteEdit.SetPaletteRepository(paletteRepository);
				paletteEdit.Text = chart.Palette.DisplayName;
				SetStyleEditValue();
			}
			finally {
				lockChanges = false;
			}
		}
		public override void Release() {
			base.Release();
			DisposeAppearanceImages();
			panelControl1.Controls.Remove(designControl);
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
			Diagram diagram = chart.Diagram;
			if (diagram != null) {
				IList<ISeries> series = chart.ViewController.GetISeriesForLegendList();
				if (series.Count > 0)
					viewType = SeriesViewFactory.GetViewType(((Series)series[0]).View);
			}
			stylesContainer.SuspendLayout();
			try {
				stylesContainer.SetPaletteRepository(paletteRepository);
				stylesContainer.SetAppearancesCount(appearanceRepository.Names.Length);
				foreach (ChartAppearance appearance in appearanceRepository)
					if (!chart.Palette.Predefined || appearance == chart.Appearance || String.IsNullOrEmpty(appearance.PaletteName) || appearance.PaletteName == chart.Palette.Name)
						stylesContainer.RegisterAppearance(appearance, viewType, chart.Palette);
			}
			finally {
				stylesContainer.ResumeLayout();
			}
		}
		void SetStyleEditValue() {
			string colorIndexTip = chart.PaletteBaseColorNumber == 0 ?
				ChartLocalizer.GetString(ChartStringId.StyleAllColors) :
				String.Format(ChartLocalizer.GetString(ChartStringId.StyleColorNumber), chart.PaletteBaseColorNumber);
			styleEdit.EditValue = String.Format("{0} ({1})", chart.AppearanceName, colorIndexTip);
		}
		void paletteEdit_EditValueChanged(object sender, EventArgs e) {
			if (lockChanges)
				return;
			Palette palette = paletteRepository[paletteEdit.Text];
			if (palette != null && palette != chart.Palette)
				chart.Palette = palette;
			foreach (ChartAppearance appearance in chart.AppearanceRepository)
				if (appearance.PaletteName == palette.Name) {
					chart.Appearance = appearance;
					SetStyleEditValue();
					return;
				}
			if (previousAppearance != null) {
				chart.Appearance = previousAppearance;
				SetStyleEditValue();
			}
		}
		void paletteEdit_QueryPopUp(object sender, CancelEventArgs e) {
			previousAppearance = chart.Appearance;
			previousPalette = chart.Palette;
			paletteEditControl.SelectedPalette = chart.Palette;
		}
		void paletteEdit_Closed(object sender, ClosedEventArgs e) {
			if (e.CloseMode == PopupCloseMode.Cancel) {
				paletteEdit.EditValue = previousPalette.DisplayName;
				chart.Appearance = previousAppearance;
				SetStyleEditValue();
			}
		}
		void paletteEditControl_OnNeedClose(object sender, System.EventArgs e) {
			paletteEdit.ClosePopup();
		}
		void paletteEditControl_OnPaletteChanged(object sender, System.EventArgs e) {
			paletteEdit.EditValue = paletteEditControl.SelectedPalette.DisplayName;
		}
		void styleEdit_QueryPopUp(object sender, CancelEventArgs e) {
			FillAppearances();
			previousAppearance = chart.Appearance;
			previousPaletteIndex = chart.PaletteBaseColorNumber;
			stylesContainer.SelectStyle(previousAppearance, previousPaletteIndex);
		}
		void styleEdit_CloseUp(object sender, CloseUpEventArgs e) {
			if (e.CloseMode == PopupCloseMode.Cancel) {
				chart.Appearance = previousAppearance;
				chart.PaletteBaseColorNumber = previousPaletteIndex;
			}
		}
		void stylesContainer_OnEditValueChanged(object sender, EventArgs e) {
			Palette palette = chart.Palette;
			chart.Appearance = stylesContainer.CurrentAppearance;
			chart.Palette = palette;
			chart.PaletteBaseColorNumber = stylesContainer.CurrentPaletteIndex;
			SetStyleEditValue();
		}
		void stylesContainer_OnNeedClose(object sender, EventArgs e) {
			styleEdit.ClosePopup();
		}
	}
}
