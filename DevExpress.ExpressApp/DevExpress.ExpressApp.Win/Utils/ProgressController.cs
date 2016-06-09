#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Threading;
using System.Windows.Forms;
using System.ComponentModel;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Localization;
namespace DevExpress.ExpressApp.Win.Utils {
	public class ProgressController : IDisposable {
		private XtraForm form = new XtraForm();
		private ProgressBarControl progressBar = new ProgressBarControl();
		private SimpleButton cancelButton = new SimpleButton();
		private Label label = new Label();
		private bool isCancelPressed;
		private void DoOnButtonClick(object obj, EventArgs e) {
			isCancelPressed = true;
			form.Hide();
		}
		private void DoOnFormClosed(object sender, EventArgs e) {
			isCancelPressed = true;
		}
		private void DoOnFormLostFocus(object sender, EventArgs e) {
			form.Focus();
		}
		public ProgressController() {
			NativeMethods.SetExecutingApplicationIcon(form);
		}
		public ProgressController(int minimum, int maximum) : this() {
			form.Width = 350;
			form.Height = 125;
			form.StartPosition = FormStartPosition.CenterScreen;
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.ShowInTaskbar = false;
			form.Closed += new EventHandler(DoOnFormClosed);
			label.Parent = form;
			label.Location = new Point(10, 10);
			label.Size = new Size(form.ClientSize.Width - 20, 13);
			progressBar.Parent = form;
			progressBar.Location = new Point(10, 30);
			progressBar.Name = "progressBar";
			progressBar.Size = new Size(form.ClientSize.Width - 20, 15);
			progressBar.Properties.Minimum = minimum;
			progressBar.Properties.Maximum = maximum;
			progressBar.Properties.Step = 1;
			cancelButton.Parent = form;
			cancelButton.DialogResult = DialogResult.Cancel;
			cancelButton.Size = new Size(75, 23);
			cancelButton.Location = new Point((form.Width - cancelButton.Width) / 2, 55);
			cancelButton.Text = "&Cancel";
			cancelButton.Click += new EventHandler(DoOnButtonClick);
			cancelButton.LostFocus += new EventHandler(DoOnFormLostFocus);
			form.CancelButton = cancelButton;
			form.Show();
			Application.DoEvents();
		}
		public ProgressController(string caption, int minimum, int maximum) : this(minimum, maximum) {
			form.Text = caption;
		}
		public void SetStatus(int maximum, int position, string labelText) {
			progressBar.Properties.Maximum = maximum;
			SetStatus(position, labelText);
		}
		public void SetStatus(int position, string labelText) {
			progressBar.EditValue = position;
			label.Text = labelText;
			if (position == progressBar.Properties.Maximum) {
				form.Hide();
			}
			if (isCancelPressed) {
				form.Hide();
				throw new ApplicationException(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.ProcessWasAborted));
			}
			Application.DoEvents();
		}
		public void Close() {
			Form.Hide();
		}
		public bool IsCancelPressed {
			get { return isCancelPressed; }
		}
		protected Form Form {
			get { return form; }
		}
		#region IDisposable Members
		public void Dispose() {
			label = null;
			form.Closed -= new EventHandler(DoOnFormClosed);
			cancelButton.Click -= new EventHandler(DoOnButtonClick);
			cancelButton.LostFocus -= new EventHandler(DoOnFormLostFocus);
			form.Dispose();
			form = null;
		}
		#endregion
	}
}
