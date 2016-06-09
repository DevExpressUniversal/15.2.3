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

using DevExpress.Data.WizardFramework;
using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraReports.ReportGeneration.Wizard.Views;
namespace DevExpress.XtraReports.ReportGeneration.Wizard {
	[ToolboxItem(false)]
	public partial class ReportGeneratorWizardView : XtraForm, IWizardView {
		bool WizardCompleted { get; set; }
		WizardPageView currentView;
		public ReportGeneratorWizardView(){
			InitializeComponent();
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			WizardCompleted = false;
			Next += (sender, args) => this.DialogResult = DialogResult.None;
			Finish += (sender, args) => WizardCompleted = true;
			this.FormClosing += WizardView_FormClosing;
		}
		void WizardView_FormClosing(object sender, FormClosingEventArgs e){
			if(this.DialogResult == DialogResult.None)
				e.Cancel = true;
		}
		public void EnableNext(bool enable){
			if(currentView != null)
				currentView.EnableNext(enable);
		}
		public void EnablePrevious(bool enable){
			if (currentView != null)
				currentView.EnablePrevious(enable);
		}
		public void EnableFinish(bool enable){
			if(currentView != null){
				currentView.EnableFinish(enable);
				AcceptButton = currentView.ActiveButton;
			}
		}
		public void SetPageContent(object pageView){
			var view = pageView as Control;
			if(view != null){
				view.Dock = DockStyle.Fill;
				this.Controls.Clear();
				this.Controls.Add(view);
				view.Focus();
			}
			var sprtLookAndFeel = pageView as ISupportLookAndFeel;
			if(sprtLookAndFeel != null)
				sprtLookAndFeel.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			var wizardView = pageView as WizardPageView;
			if(wizardView != null){
				if(currentView != null){
					currentView.Previous -= Previous;
					currentView.Next -= Next;
					currentView.Finish -= Finish;
				}
				currentView = wizardView;
				wizardView.Previous += Previous;
				wizardView.Next += Next;
				wizardView.Finish += Finish;
			}
		}
		public void ShowError(string error){
		}
		void cancel_Click(object sender, EventArgs e){
			if(Cancel != null)
				Cancel(sender, EventArgs.Empty);
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData){
			if(keyData == Keys.Escape)
				cancel_Click(this, EventArgs.Empty);
			return base.ProcessCmdKey(ref msg, keyData);
		}
		public event EventHandler Cancel;
		public event EventHandler Next;
		public event EventHandler Previous;
		public event EventHandler Finish;
	}
}
