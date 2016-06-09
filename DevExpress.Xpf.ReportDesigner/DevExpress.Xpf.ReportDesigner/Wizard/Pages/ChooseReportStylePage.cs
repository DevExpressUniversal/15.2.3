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
using DevExpress.Mvvm.POCO;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages.Common;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	public class ChooseReportStylePage : ReportWizardPageBase {
		public static ChooseReportStylePage Create(ReportWizardModel reportWizardModel) {
			return ViewModelSource.Create(() => new ChooseReportStylePage(reportWizardModel));
		}
		public IEnumerable<ReportStyleViewModel> ReportStyles { get; private set; }
		public virtual ReportStyleViewModel SelectedReportStyle { get; set; }
		public ChooseReportStylePage(ReportWizardModel model) : base(model) {
			FillStyles();
			SelectedReportStyle = ReportStyles.First(x => x.StyleId == ReportModel.ReportStyleId);
		}
		void FillStyles() {
			ReportStyles = new[] {
				new ReportStyleViewModel(ReportStyleId.Bold),
				new ReportStyleViewModel(ReportStyleId.Compact),
				new ReportStyleViewModel(ReportStyleId.Casual),
				new ReportStyleViewModel(ReportStyleId.Corporate),
				new ReportStyleViewModel(ReportStyleId.Formal)
			};
		}
		protected void OnSelectedReportStyleChanged() {
			ReportModel.ReportStyleId = SelectedReportStyle.StyleId;
		}
		#region overrides
		public override bool CanGoForward {
			get { return true; }
		}
		public override bool CanFinish {
			get { return true; }
		}
		protected override void NavigateToNextPage(WizardController wizardController) {
			wizardController.NavigateTo(SetReportTitlePage.Create(ReportWizardModel));
		}
		#endregion
	}
}
