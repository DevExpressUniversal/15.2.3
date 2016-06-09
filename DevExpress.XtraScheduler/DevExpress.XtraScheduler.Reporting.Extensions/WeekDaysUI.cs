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
using System.Security;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
namespace DevExpress.XtraScheduler.Reporting.Design {
	[DXToolboxItem(false)]
	public partial class WeekDaysUI : UserControl {
		WeekDays editValue;
		object oldValue;
		public WeekDaysUI(WeekDaysEditor editor) {
			InitializeComponent();
		}
		public WeekDays Value { get { return editValue; } }
		public void Start(System.Windows.Forms.Design.IWindowsFormsEditorService edSvc, object val) {
			oldValue = val;
			this.editValue = (WeekDays)val;
			UnsunscribeCheckBoxesEvents();
			chkSun.Checked = (Value & WeekDays.Sunday) != 0;
			chkMo.Checked = (Value & WeekDays.Monday) != 0;
			chkTue.Checked = (Value & WeekDays.Tuesday) != 0;
			chWed.Checked = (Value & WeekDays.Wednesday) != 0;
			chkThu.Checked = (Value & WeekDays.Thursday) != 0;
			chkFri.Checked = (Value & WeekDays.Friday) != 0;
			chkSat.Checked = (Value & WeekDays.Saturday) != 0;
			SunscribeCheckBoxesEvents();
		}
		protected virtual void SunscribeCheckBoxesEvents() {
			foreach (CheckBox chk in Controls)
				chk.CheckedChanged += new System.EventHandler(this.chkSun_CheckedChanged);
		}
		protected virtual void UnsunscribeCheckBoxesEvents() {
			foreach (CheckBox chk in Controls)
				chk.CheckedChanged -= new System.EventHandler(this.chkSun_CheckedChanged);
		}
		public void End() {
			UnsunscribeCheckBoxesEvents();
		}
		[SecuritySafeCritical]
		protected override bool ProcessDialogKey(Keys keyData) {
			if (keyData == Keys.Escape)
				editValue = (WeekDays)oldValue;
			return base.ProcessDialogKey(keyData);
		}
		private void chkSun_CheckedChanged(object sender, EventArgs e) {
			UpdateEditValue();
		}
		protected virtual void UpdateEditValue() {
			int newVal = 0;
			for (int i = 0; i < Controls.Count; i++) {
				CheckBox chk = (CheckBox)Controls[i];
				if (chk.Checked)
					newVal += (int)chk.Tag;
			}
			editValue = (WeekDays)newVal;
		}
	}
}
