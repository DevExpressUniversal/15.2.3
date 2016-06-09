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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class ImageControl : ChartUserControl {
		ChartImage chartImage;
		Chart originalChart;
		Action0 changedCallback;
		bool HasImage { get { return chartImage.Image != null; } }
		bool Stretch {
			get {
				BackgroundImage backgroundImage = chartImage as BackgroundImage;
				return backgroundImage != null && backgroundImage.Stretch;
			}
			set {
				BackgroundImage backgroundImage = chartImage as BackgroundImage;
				if (backgroundImage != null)
					backgroundImage.Stretch = value;
			}
		}
		public ImageControl() {
			InitializeComponent();
		}
		public void Initialize(ChartImage chartImage, Chart originalChart) {
			Initialize(chartImage, originalChart, null);
		}
		public void Initialize(ChartImage chartImage, Chart originalChart, Action0 changedCallback) {
			this.chartImage = chartImage;
			this.originalChart = originalChart;
			this.changedCallback = changedCallback;
			chStretch.Visible = chartImage is BackgroundImage;
			chStretch.Checked = Stretch;
			UpdateImageControls();
		}
		void UpdateImageControls() {
			beImage.EditValue = HasImage ? chartImage.ToString() : ChartLocalizer.GetString(ChartStringId.WizNoBackImage);
			chStretch.Enabled = HasImage;
			btnClear.Enabled = HasImage;
			if (changedCallback != null)
				changedCallback();
		}
		void beImage_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if (originalChart != null && originalChart.Container != null && originalChart.Container.Site != null) {
				IDesignerHost designerHost = (IDesignerHost)originalChart.Container.Site.GetService(typeof(IDesignerHost));
				if (designerHost != null) {
					IWebChartDesigner designer = designerHost.GetDesigner(originalChart.Container as IComponent) as IWebChartDesigner;
					if (designer != null) {
						string imageUrl = designer.GetImageUrl(chartImage.ImageUrl);
						if (!String.IsNullOrEmpty(imageUrl)) {
							try {
								chartImage.Image = originalChart.Container.RenderProvider.LoadBitmap(imageUrl);
								chartImage.ImageUrl = imageUrl;
								UpdateImageControls();
							}
							catch {
								originalChart.Container.ShowErrorMessage(ChartLocalizer.GetString(ChartStringId.WizInvalidBackgroundImage),
									ChartLocalizer.GetString(ChartStringId.WizErrorMessageTitle));
							}
						}
						return;
					}
				}
			}
			using (OpenFileDialog dlg = new OpenFileDialog()) {
				dlg.Filter = ChartLocalizer.GetString(ChartStringId.WizBackImageFileNameFilter);
				if (dlg.ShowDialog() == DialogResult.OK)
					try {
						chartImage.Image = Image.FromFile(dlg.FileName);
						UpdateImageControls();
					}
					catch {
						originalChart.Container.ShowErrorMessage(
							ChartLocalizer.GetString(ChartStringId.WizInvalidBackgroundImage),
							ChartLocalizer.GetString(ChartStringId.WizErrorMessageTitle));
					}
			}
		}
		void chStretch_CheckedChanged(object sender, EventArgs e) {
			Stretch = chStretch.Checked;
		}
		void btnClear_Click(object sender, EventArgs e) {
			chartImage.Clear();
			UpdateImageControls();
		}
	}
}
