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
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Services;
using DevExpress.Snap.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.Snap.Services {
	public class SnapProgressIndicationService : ISnapMailMergeProgressIndicationService, IDisposable {
		readonly SnapControl snapControl;
		CancellationTokenSource cts;
		CancellationToken token;
		ProgressIndicationForm frm;
		public SnapProgressIndicationService(SnapControl snapControl) {
			Guard.ArgumentNotNull(snapControl, "snapControl");
			this.snapControl = snapControl;
		}
#if !SL
	[DevExpressSnapLocalizedDescription("SnapProgressIndicationServiceCancellationToken")]
#endif
		public CancellationToken CancellationToken { get { return token; } }
		public void Begin(string displayName, int minProgress, int maxProgress, int currentProgress) {
			Action begin = delegate {
				this.frm = new ProgressIndicationForm();
				SubscribeFormEvents();
				this.frm.Begin(displayName, minProgress, maxProgress, currentProgress);
				this.frm.ShowDialog(this.snapControl);
			};
			this.snapControl.BeginInvoke(begin);
		}
		public void SetProgress(int currentProgress) {
			if (this.frm == null)
				return;
			Action setProgress = delegate { this.frm.SetProgress(currentProgress); };
			this.snapControl.BeginInvoke(setProgress);
		}
		public void End() {
			if (this.frm == null)
				return;
			Action end = delegate {
				frm.End();
				UnsubscribeFormEvents();
				frm.Dispose();
				frm = null;
			};
			snapControl.BeginInvoke(end);
		}
		public void Reset() {
			if (this.cts != null) {
				this.cts.Dispose();
				this.cts = null;
			}
			this.cts = new CancellationTokenSource();
			this.token = this.cts.Token;
		}
		void SubscribeFormEvents() {
			this.frm.FormClosing += OnProgressIndicationFormClosing;
		}
		void UnsubscribeFormEvents() {
			if (this.frm != null)
				this.frm.FormClosing -= OnProgressIndicationFormClosing;
		}
		void OnProgressIndicationFormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {
			if (XtraMessageBox.Show(frm.LookAndFeel, frm, SnapLocalizer.GetString(SnapStringId.Msg_StopMailMerge), SnapLocalizer.GetString(SnapStringId.ProgressIndicationForm_Text), MessageBoxButtons.OKCancel) == DialogResult.OK) {
				this.cts.Cancel();
				this.snapControl.UpdateCommandUI();
			}
			else
				e.Cancel = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SnapProgressIndicationService() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (frm != null) {
					frm.Dispose();
					frm = null;
				}
				if (cts != null) {
					cts.Dispose();
					cts = null;
				}
			}
		}
	}
}
