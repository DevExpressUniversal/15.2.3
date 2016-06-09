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

using DevExpress.LookAndFeel;
using System.Windows.Forms;
using System;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class LabelsGeneralControl : ChartUserControl {
		public LabelsGeneralControl() {
			InitializeComponent();
			redactControl.SelectedElementChanged += new SelectedElementChangedEventHandler(redactControl_SelectedElementChanged);
		}
		public void Initialize(UserLookAndFeel lookAndFeel, AxisBase axis, Chart chart) {
			axisLabelControl.Initialize(lookAndFeel, axis, chart, this);
			labelTextPatternControl.Initialize(axis.Label);
			if (axis is Axis2D) {
				redactControl.Initialize(((Axis2D)axis).CustomLabels);
				tabPageCustom.PageVisible = true;
			}
			else
				tabPageCustom.PageVisible = false;
		}
		void redactControl_SelectedElementChanged() {
			customLabelsControl.Initialize((CustomAxisLabel)redactControl.CurrentElement);
		}
		public void UpdateControls(bool enabled) {
			labelTextPatternControl.Enabled = enabled;
			customLabelsControl.Enabled = enabled;
			redactControl.Enabled = enabled;
		}
		protected override void OnSizeChanged(EventArgs e) {
			if (this.Handle != null) {
				BeginInvoke(new MethodInvoker(ChangeSize));
			}
			base.OnResize(e);
		}
		protected void ChangeSize() {
			if (axisLabelControl != null) {
				axisLabelControl.Size = this.ClientSize;
			}
		}
	}
}
