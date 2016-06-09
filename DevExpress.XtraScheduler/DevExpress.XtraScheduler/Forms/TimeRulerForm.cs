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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors.Native;
using DevExpress.Utils.Menu;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.TimeRulerForm.grpTimeZone")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.TimeRulerForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.TimeRulerForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.TimeRulerForm.edtTimeZone")]
#endregion
namespace DevExpress.XtraScheduler.Forms {
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class TimeRulerForm : DevExpress.XtraEditors.XtraForm {
		TimeRuler timeRuler;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public TimeRuler TimeRuler {
			get { return timeRuler; }
			set {
				timeRuler = value;
				if (edtTimeZone != null)
					edtTimeZone.TimeRuler = timeRuler;
			}
		}
		public TimeRulerForm() {
			InitializeComponent();
		}
		public virtual void SetMenuManager(Utils.Menu.IDXMenuManager menuManager) {
			MenuManagerUtils.SetMenuManager(Controls, menuManager);
		}
		void TimeRulerForm_Load(object sender, System.EventArgs e) {
			OnFormLoad();
		}
		protected virtual void OnFormLoad() {
			edtTimeZone.TimeRuler = timeRuler;
		}
		void btnOk_Click(object sender, System.EventArgs e) {
			OnOkButton();
		}
		protected virtual void OnOkButton() {
			if (TimeRuler == null)
				return;
			timeRuler.TimeZoneId = edtTimeZone.TimeZoneId;
			timeRuler.Caption = edtTimeZone.Caption;
			timeRuler.AdjustForDaylightSavingTime = edtTimeZone.AdjustForDaylightSavingTime;
		}
	}
}
