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
namespace DevExpress.XtraScheduler.Design.Wizards
{
	public class SchedulerWizardForm : DevExpress.Utils.WizardForm
	{
		const int ButtonOffsetFromBottomLine = 35;
		const int DefaultButtonDistance = 10;
		System.ComponentModel.IContainer components = null;
		public SchedulerWizardForm()
		{
			InitializeComponent();
			WizardButtons = WizardButton.Back | WizardButton.Next | WizardButton.Finish;
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
		}
		public override WizardButton WizardButtons {
			get { return base.WizardButtons; }
			set {
				base.WizardButtons = value;
				btnNext.Visible = true;
				AcceptButton = (IButtonControl)(btnNext.Visible && btnNext.Enabled ? btnNext : btnFinish);
			}
		}
		protected override Control CreateWizardButton() {
			return new SimpleButton();
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		protected override void OnPaint(PaintEventArgs e) {
			Point location = new System.Drawing.Point(ClientSize.Width, ClientSize.Height - ButtonOffsetFromBottomLine);
			btnFinish.Location = CalcButtonLocation(btnFinish, location);
			btnNext.Location = CalcButtonLocation(btnNext, btnFinish.Location);
			btnBack.Location = CalcButtonLocation(btnBack, btnNext.Location);
			btnCancel.Location = CalcButtonLocation(btnCancel, btnBack.Location);
			base.OnPaint(e);
		}
		#region Designer generated code
		void InitializeComponent()
		{
			this.SuspendLayout();
			this.separator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.separator.Location = new System.Drawing.Point(0, 312);
			this.separator.Margin = new System.Windows.Forms.Padding(0);
			this.separator.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.separator.Size = new System.Drawing.Size(498, 2);
			this.ClientSize = new System.Drawing.Size(500, 360);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.MinimumSize = new System.Drawing.Size(515, 399);
			this.Name = "SchedulerWizardForm";
			this.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
			this.ResumeLayout(false);
		}
		#endregion
		Point CalcButtonLocation(Control button, Point location) {
			return new System.Drawing.Point(location.X - button.Width - DefaultButtonDistance, location.Y);
		}
	}
}
