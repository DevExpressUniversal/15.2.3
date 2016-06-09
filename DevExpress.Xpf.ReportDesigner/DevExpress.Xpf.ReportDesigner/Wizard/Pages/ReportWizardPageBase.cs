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

using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.XtraReports.Wizards3;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	public abstract class ReportWizardPageBase : ISupportWizard, ISupportWizardBackCommand, ISupportWizardCancelCommand, ISupportWizardFinishCommand, ISupportWizardNextCommand {
		protected ReportWizardModel ReportWizardModel { get; private set; }
		protected XtraReportModel ReportModel {get { return ReportWizardModel.Return(x => x.ReportModel, () => null); } }
		protected ReportWizardPageBase(ReportWizardModel wizardModel) {
			ReportWizardModel = wizardModel;
		}
		public abstract bool CanFinish { get; }
		public abstract bool CanGoForward { get; }
		void ISupportWizard.NavigateToNextPage(WizardController wizardController) {
			NavigateToNextPage(wizardController);
		}
		protected abstract void NavigateToNextPage(WizardController wizardController);
		void ISupportWizardNextCommand.OnGoForward(CancelEventArgs e) {
			OnGoForward(e);
		}
		protected virtual void OnGoForward(CancelEventArgs e) { }
		void ISupportWizardBackCommand.OnGoBack(CancelEventArgs e) {
			OnGoBack(e);
		}
		protected virtual void OnGoBack(CancelEventArgs e) { }
		void ISupportWizardCancelCommand.OnCancel(CancelEventArgs e) {
			OnCancel(e);
		}
		protected virtual void OnCancel(CancelEventArgs e) { }
		void ISupportWizardFinishCommand.OnFinish(CancelEventArgs e) {
			OnFinish(e);
		}
		protected virtual void OnFinish(CancelEventArgs e) {
			ReportWizardModel.Finish();
		}
	}
}
