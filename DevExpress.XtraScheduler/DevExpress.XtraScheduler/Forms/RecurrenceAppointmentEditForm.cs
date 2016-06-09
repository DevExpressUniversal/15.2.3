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
using System.Reflection;
using DevExpress.Utils.Controls;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentEditForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentEditForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentEditForm.lbConfirmText")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentEditForm.panelControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentEditForm.chkEditOccurrence")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentEditForm.chkEditSeries")]
#endregion
namespace DevExpress.XtraScheduler.Forms {
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class RecurrentAppointmentEditForm : DevExpress.XtraEditors.XtraForm {
		RecurrentAppointmentAction queryResult;
		public RecurrentAppointmentEditForm() {
			InitializeComponent();
			UpdateQueryResult();
		}
		public RecurrentAppointmentEditForm(Appointment apt) {
			InitializeComponent();
			UpdateQueryResult();
			lbConfirmText.Text = String.Format(lbConfirmText.Text, apt.Subject);
		}
		public RecurrentAppointmentAction QueryResult { get { return queryResult; } set { queryResult = value; } }
		private void RecurrentAppointmentEditForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			DrawIcon(e.Graphics);
		}
		protected virtual void DrawIcon(Graphics gr) {
			gr.DrawIcon(SystemIcons.Question, panelControl1.Left, panelControl1.Top);
		}
		private void chkEditOccurrence_CheckedChanged(object sender, System.EventArgs e) {
			UpdateQueryResult();
		}
		private void UpdateQueryResult() {
			QueryResult = chkEditSeries.Checked ? RecurrentAppointmentAction.Series : RecurrentAppointmentAction.Occurrence;
		}
	}
}
