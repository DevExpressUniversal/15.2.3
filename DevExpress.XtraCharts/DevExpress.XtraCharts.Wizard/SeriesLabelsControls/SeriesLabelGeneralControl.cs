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
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.SeriesLabelsControls {
	internal partial class SeriesLabelGeneralControl : ChartUserControl {
		SeriesLabelBase label;
		SeriesBase series;
		MethodInvoker updateMethod;
		ChartUserControl optionsControl;
		ILabelBehaviorProvider LabelBehaviorProvider { get { return label; } }
		public SeriesLabelGeneralControl() {
			InitializeComponent();
		}
		public void Initialize(UserLookAndFeel lookAndFeel, SeriesLabelBase label, SeriesBase series, Chart chart, MethodInvoker updateMethod) {
			this.label = label;
			this.series = series;
			this.updateMethod = updateMethod;
			if (series != null)
				CheckEditHelper.SetCheckEditState(chVisible, series.LabelsVisibility);
			else
				CheckEditHelper.SetCheckEditState(chVisible, DevExpress.Utils.DefaultBoolean.False);
			pnlResolveOverlappingSettings.Visible = LabelBehaviorProvider.ResolveOverlappingSupported;
			if (LabelBehaviorProvider.ResolveOverlappingSupported)
				overlappingSettingsControl.Initialize(label, series, chart);
			textSettingsControl.Initialize(lookAndFeel, label);
			labelLocationControl.Initialize(label, series, chart, UpdateControls);
			optionsControl = CreateOptionsControl();
			if (optionsControl != null) {
				optionsControl.Dock = DockStyle.Fill;
				pnlOptions.Controls.Add(optionsControl);
			}
			else
				pnlOptions.Visible = false;
			UpdateControls();
		}
		ChartUserControl CreateOptionsControl() {
			BubbleSeriesLabel bubbleLabel = label as BubbleSeriesLabel;
			if (bubbleLabel != null) {
				BubbleViewControl bubbleControl = new BubbleViewControl();
				bubbleControl.Initialize(bubbleLabel, UpdateControls);
				return bubbleControl;
			}
			return null;
		}
		void UpdateControls() {
			bool labelVisible = series.ActualLabelsVisibility;
			overlappingSettingsControl.Enabled = labelVisible && LabelBehaviorProvider.ResolveOverlappingEnabled;
			textSettingsControl.Enabled = labelVisible;
			labelLocationControl.Enabled = labelVisible;
			pnlOptions.Enabled = labelVisible;
			if(updateMethod != null)
				updateMethod();
		}
		void chVisible_CheckStateChanged(object sender, EventArgs e) {
			series.LabelsVisibility = CheckEditHelper.GetCheckEditState(chVisible);
			UpdateControls();
		}
	}
}
