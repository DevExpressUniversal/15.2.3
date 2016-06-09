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
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentDeleteForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentDeleteForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentDeleteForm.lbConfirmText")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentDeleteForm.panelControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentDeleteForm.chkDeleteOccurrence")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Forms.RecurrentAppointmentDeleteForm.chkDeleteSeries")]
#endregion
namespace DevExpress.XtraScheduler.Forms {
	[System.Runtime.InteropServices.ComVisible(false)]
	public partial class RecurrentAppointmentDeleteForm : DevExpress.XtraEditors.XtraForm {
		RecurrentAppointmentAction queryResult;
		#region Properties
		public RecurrentAppointmentAction QueryResult { get { return queryResult; } set { queryResult = value; } }
		[Obsolete("You should use the 'QueryResult' instead.", false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool DeleteSeries {
			get {
				return QueryResult == RecurrentAppointmentAction.Series;
			}
			set {
				QueryResult = value ? RecurrentAppointmentAction.Series : RecurrentAppointmentAction.Occurrence;
			}
		}
		#endregion
		public RecurrentAppointmentDeleteForm() {
			InitializeComponent();
			UpdateQueryResult();
		}
		public RecurrentAppointmentDeleteForm(Appointment apt) {
			InitializeComponent();
			UpdateQueryResult();
			lbConfirmText.Text = String.Format(lbConfirmText.Text, apt.Subject);
		}
		private void RecurrentAppointmentDeleteForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			DrawIcon(e.Graphics);
		}
		protected virtual void DrawIcon(Graphics gr) {
			gr.DrawIcon(SystemIcons.Exclamation, panelControl1.Left, panelControl1.Top);
		}
		private void chkDeleteOccurrence_CheckedChanged(object sender, System.EventArgs e) {
			UpdateQueryResult();
		}
		private void UpdateQueryResult() {
			QueryResult = chkDeleteSeries.Checked ? RecurrentAppointmentAction.Series : RecurrentAppointmentAction.Occurrence;
		}
	}
}
