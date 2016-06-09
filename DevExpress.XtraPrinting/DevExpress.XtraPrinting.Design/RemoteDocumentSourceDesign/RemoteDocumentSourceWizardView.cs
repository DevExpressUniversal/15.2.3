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
using System.Windows.Forms;
using DevExpress.Data.WizardFramework;
using DevExpress.Utils.Design;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign {
	public partial class RemoteDocumentSourceWizardView : XtraDesignForm, IWizardView {
		IPageView activePageView;
		int stepNumber;
		bool isNextStep = true;
		public string Caption {
			get {
				return activePageView != null ? activePageView.HeaderText : string.Empty;
			}
		}
		public string Description {
			get {
				return activePageView != null ? activePageView.DescriptionText : string.Empty;
			}
		}
		public event System.EventHandler Finish;
		public event System.EventHandler Next;
		public event System.EventHandler Previous;
		public event System.EventHandler Cancel;
		public RemoteDocumentSourceWizardView() {
			InitializeComponent();
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
		}
		public void EnableFinish(bool enable) {
			finishButton.Enabled = enable;
		}
		public void EnableNext(bool enable) {
			nextButton.Enabled = enable;
		}
		public void EnablePrevious(bool enable) {
			previousButton.Enabled = enable;
		}
		public void SetPageContent(object content) {
			activePageView = content as IPageView;
			var frame = content as UserControl;
			if(frame != null) {
				if(isNextStep) stepNumber++;
				else stepNumber--;
				frame.Dock = DockStyle.Fill;
				contentPanel.Controls.Clear();
				contentPanel.Controls.Add(frame);
				descriptionLbl.Text = activePageView.DescriptionText;
			} else ShowError(new ArgumentException("content").Message);
		}
		public void ShowError(string error) {
			XtraMessageBox.Show(this.LookAndFeel, this, error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
		private void cancelButton_Click(object sender, System.EventArgs e) {
			Cancel.SafeRaise(sender);
		}
		private void previousButton_Click(object sender, System.EventArgs e) {
			isNextStep = false;
			Previous.SafeRaise(sender);
		}
		private void nextButton_Click(object sender, System.EventArgs e) {
			isNextStep = true;
			Next.SafeRaise(sender);
		}
		private void finishButton_Click(object sender, System.EventArgs e) {
			Finish.SafeRaise(sender);
		}
		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			System.Diagnostics.Process.Start("mailto:support@devexpress.com");
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if(keyData == Keys.Escape) {
				Close();
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
