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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.Web.ASPxScheduler.Design.Wizards {
	public class ASPxSchedulerNotifyWizardForm : DevExpress.Utils.WizardForm {
		private System.ComponentModel.IContainer components = null;
		public ASPxSchedulerNotifyWizardForm() {
			InitializeComponent();
			btnCancel.Location = new System.Drawing.Point(167, 327);
			btnFinish.Location = new System.Drawing.Point(412, 327);
			WizardButtons = WizardButton.Finish;
		}
		public override WizardButton WizardButtons {
			get { return base.WizardButtons; }
			set {
				base.WizardButtons = value;
				AcceptButton = (IButtonControl)(btnNext.Visible && btnNext.Enabled ? btnNext : btnFinish);
			}
		}
		protected override Control CreateWizardButton() {
			return new SimpleButton();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.SuspendLayout();
			this.btnBack.Visible = false;
			this.btnNext.Visible = true;
			this.btnCancel.Visible = false;
			this.btnFinish.Text = "Close";
			this.ClientSize = new System.Drawing.Size(499, 362);
			this.Name = "ASPxSchedulerNotifyWizardForm";
			this.ResumeLayout(false);
		}
		#endregion
	}
}
