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
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	internal partial class AnnotationImageContentControl : ChartUserControl {
		struct ChartImageSizeModeItem {
			readonly ChartImageSizeMode sizeMode;
			readonly string text;
			public ChartImageSizeMode SizeMode { get { return sizeMode; } }
			public ChartImageSizeModeItem(ChartImageSizeMode sizeMode) {
				this.sizeMode = sizeMode;
				switch (sizeMode) {
					case ChartImageSizeMode.AutoSize:
						text = ChartLocalizer.GetString(ChartStringId.WizChartImageSizeModeAutoSize);
						break;
					case ChartImageSizeMode.Stretch:
						text = ChartLocalizer.GetString(ChartStringId.WizChartImageSizeModeStretch);
						break;
					case ChartImageSizeMode.Tile:
						text = ChartLocalizer.GetString(ChartStringId.WizChartImageSizeModeTile);
						break;
					case ChartImageSizeMode.Zoom:
						text = ChartLocalizer.GetString(ChartStringId.WizChartImageSizeModeZoom);
						break;
					default:
						ChartDebug.Fail("Unknown chart image size mode.");
						goto case ChartImageSizeMode.Stretch;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is ChartImageSizeModeItem) && sizeMode == ((ChartImageSizeModeItem)obj).sizeMode;
			}
			public override int GetHashCode() {
				return sizeMode.GetHashCode();
			}
		}
		ImageAnnotation annotation;
		Action0 changedCallback;
		bool inUpdate;
		public AnnotationImageContentControl() {
			InitializeComponent();
		}
		public void Initialize(ImageAnnotation annotation, Action0 changedCallback, Chart originalChart) {
			this.annotation = annotation;
			this.changedCallback = changedCallback;
			imageControl.Initialize(annotation.Image, originalChart, changedCallback);
			cbSizeMode.Properties.Items.Clear();
			foreach (ChartImageSizeMode sizeMode in Enum.GetValues(typeof(ChartImageSizeMode)))
				cbSizeMode.Properties.Items.Add(new ChartImageSizeModeItem(sizeMode));
			UpdateSizeModeCombo();
		}
		public void UpdateSizeModeCombo() {
			inUpdate = true;
			try {
				cbSizeMode.SelectedItem = new ChartImageSizeModeItem(annotation.SizeMode);
			}
			finally {
				inUpdate = false;
			}
		}
		void cbSizeMode_SelectedIndexChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				annotation.SizeMode = ((ChartImageSizeModeItem)cbSizeMode.SelectedItem).SizeMode;
				if (changedCallback != null)
					changedCallback();
			}
		}
	}
}
