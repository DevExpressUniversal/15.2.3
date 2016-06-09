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
using DevExpress.XtraEditors;
using System.Threading;
using DevExpress.LookAndFeel;
namespace DevExpress.Xpo.Design {
	public delegate object OperationHandler(FormProgress.SetMaximumValueHandler setMaximum, FormProgress.SetPositionValueHandler setPosition);
	public partial class FormProgress : XtraForm {
		Thread workThread;
		bool isMarquee;
		bool doClose;
		bool operationSucces;
		object result;
		Exception exception;
		public int ProgressBarMax { get { return pbcProgress.Properties.Maximum; } }
		public FormProgress() {
			InitializeComponent();
		}
		delegate void UpdateDelegate();
		public delegate void SetMaximumValueHandler(int maxValue);
		public delegate void SetPositionValueHandler(int position);
		public static bool ShowModal<T>(IWin32Window owner, string caption, bool isMarquee, bool hideCancel, OperationHandler worker, out T result) {
			using(FormProgress fmProgress = new FormProgress()) {
				if(hideCancel) fmProgress.HideCancel();
				return fmProgress.Show(owner, caption, isMarquee, worker, out result);
			}
		}
		public bool Show<T>(IWin32Window owner, string caption, bool isMarquee, OperationHandler worker, out T result) {
			this.lcText.Text = caption;
			this.isMarquee = isMarquee;
			if(isMarquee) {
				mpbMarque.Visible = true;
				pbcProgress.Visible = false;
			} else {
				mpbMarque.Visible = false;
				pbcProgress.Visible = true;
			}
			this.workThread = new Thread(() => {
				try {
					if(worker != null) {
						this.result = worker(SetMaxValue, SetPositionValue);
					} else {
						this.result = null;
					}
					operationSucces = true;
				} catch(ThreadAbortException) {
					this.result = null;
				} catch(Exception ex) {
					exception = ex;
					this.result = null;
				} finally {
					try {
						Invoke(new UpdateDelegate(CorrectClose));
					} catch(Exception) { }
				}
			});
			this.workThread.IsBackground = true;
			this.ShowDialog(owner);
			if(exception != null) throw exception;
			if(this.result == null) result = default(T);
			else result = (T)this.result;
			return operationSucces;
		}
		void FormProgress_Shown(object sender, EventArgs e) {
			workThread.Start();
		}
		void CorrectClose() {
			doClose = true;
			this.Close();
		}
		void btCancel_Click(object sender, EventArgs e) {
			if(workThread.ThreadState != ThreadState.Running) {
				try {
					workThread.Abort();
				} catch(Exception) { }
				CorrectClose();
			}
		}
		public void HideCancel() {
			pbcProgress.Width = btCancel.Left + btCancel.Width - pbcProgress.Left;
			mpbMarque.Width = btCancel.Left + btCancel.Width - mpbMarque.Left;
			btCancel.Visible = false;
		}
		public void SetMaxValue(int max) {
			if(!isMarquee) {
				Invoke(new UpdateDelegate(() => {
					pbcProgress.Properties.Maximum = max;
				}));
			}
		}
		public void SetPositionValue(int value) {
			if(!isMarquee) {
				Invoke(new UpdateDelegate(() => {
					pbcProgress.Position = value;
				}));
			}
		}
		private void FormProgress_FormClosing(object sender, FormClosingEventArgs e) {
			e.Cancel = !doClose;
		}
	}
}
