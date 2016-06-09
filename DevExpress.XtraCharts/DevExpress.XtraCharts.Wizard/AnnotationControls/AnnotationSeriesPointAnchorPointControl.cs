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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	internal partial class AnnotationSeriesPointAnchorPointControl : ChartUserControl {
		SeriesPointAnchorPoint anchorPoint;
		public AnnotationSeriesPointAnchorPointControl() {
			InitializeComponent();
		}
		public void Initialize(SeriesPointAnchorPoint anchorPoint) {
			this.anchorPoint = anchorPoint;
		}
		void btnPoints_ButtonClick(object sender, ButtonPressedEventArgs e) {
			Chart chart = CommonUtils.FindOwnerChart(anchorPoint);
			if (chart != null) {
				using (SeriesPointListForm form = new SeriesPointListForm(chart.Series)) {
					form.EditValue = anchorPoint.SeriesPoint;
					form.ShowDialog();
					if (form.DialogResult == DialogResult.OK && form.EditValue != null)
						anchorPoint.SeriesPoint = form.EditValue;
				}
			}
		}
	}
}
