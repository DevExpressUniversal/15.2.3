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
namespace DevExpress.XtraCharts.Wizard.ChartAxesControls {
	internal partial class ScaleBreaksAutoControl : ChartUserControl {
		AutoScaleBreaks autoScaleBreaks;
		public ScaleBreaksAutoControl() {
			InitializeComponent();
		}
		public void Initialize(AutoScaleBreaks autoScaleBreaks) {
			this.autoScaleBreaks = autoScaleBreaks;
			chEnabled.Checked = autoScaleBreaks.Enabled;
			spnMaxCount.EditValue = autoScaleBreaks.MaxCount;
			UpdateControls();
		}
		void UpdateControls() {
			lblMaxCount.Enabled = autoScaleBreaks.Enabled;
			spnMaxCount.Enabled = autoScaleBreaks.Enabled;
		}
		void chEnabled_CheckedChanged(object sender, EventArgs e) {
			autoScaleBreaks.Enabled = chEnabled.Checked;
			UpdateControls();
		}
		void spnMaxCount_EditValueChanged(object sender, EventArgs e) {
			autoScaleBreaks.MaxCount = Convert.ToInt32(spnMaxCount.EditValue);
		}
	}
}
