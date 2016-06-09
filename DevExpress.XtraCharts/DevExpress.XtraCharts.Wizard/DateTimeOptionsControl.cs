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
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraCharts.Wizard;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class DateTimeOptionsControl : ChartUserControl {
		DateTimeOptions options;
		Locker updateLocker = new Locker();
		public DateTimeOptionsControl() {
			InitializeComponent();
		}
		public void Initialize(DateTimeOptions options) {
			updateLocker.Lock();
			try {
				this.options = options;
				cbFormat.SelectedIndex = (int)options.Format;
				txtFormatString.EditValue = options.FormatString;
				UpdateControls();
			}
			finally {
				updateLocker.Unlock();
			}
		}
		void UpdateControls() {
			updateLocker.Lock();
			try {
				txtFormatString.Enabled = options.Format == DateTimeFormat.Custom;
			}
			finally {
				updateLocker.Unlock();
			}
		}
		private void cbFormat_SelectedIndexChanged(object sender, EventArgs e) {
			if (!updateLocker.IsLocked) {
				options.Format = (DateTimeFormat)cbFormat.SelectedIndex;
				UpdateControls();
			}
		}
		private void txtFormatString_EditValueChanged(object sender, EventArgs e) {
			if (!updateLocker.IsLocked)
				options.FormatString = txtFormatString.EditValue.ToString();
		}
	}
}
