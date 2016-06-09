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
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraPrinting.Design;
namespace DevExpress.XtraReports.Design
{
	[ToolboxItem(false)]
	public class XtraReportWizardForm : DevExpress.Utils.WizardForm
	{
		public override WizardButton WizardButtons {
			get { return base.WizardButtons; }
			set {
				base.WizardButtons = value;
				btnNext.Visible = true;
				AcceptButton = (IButtonControl)(btnNext.Visible && btnNext.Enabled ? btnNext : btnFinish);
			}
		}
		XtraReportWizardForm()
			: base() {
			Initialize();
		}
		public XtraReportWizardForm(IServiceProvider serviceProvider) : base() {
			LookAndFeelProviderHelper.SetParentLookAndFeel(this, serviceProvider);
			Initialize();
		}
		void Initialize() {
			InitializeComponent();
			WizardButtons = WizardButton.Back | WizardButton.Next | WizardButton.Finish;
		}
		protected override Control CreateWizardButton() {
			return new SimpleButton();
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XtraReportWizardForm));
			this.SuspendLayout();
			resources.ApplyResources(this.btnBack, "btnBack");
			resources.ApplyResources(this.btnNext, "btnNext");
			resources.ApplyResources(this.btnCancel, "btnCancel");
			resources.ApplyResources(this.btnFinish, "btnFinish");
			resources.ApplyResources(this.separator, "separator");
			resources.ApplyResources(this, "$this");
			this.ControlBox = false;
			this.Name = "XtraReportWizardForm";
			this.ResumeLayout(false);
		}
		protected override void OnLayout(LayoutEventArgs levent) {
			base.OnLayout(levent);
			if(levent.AffectedProperty == "Visible") 
				LayoutHelper.DoButtonLayout(this.btnBack, this.btnNext, this.btnCancel, this.btnFinish);
		}
	}
}
