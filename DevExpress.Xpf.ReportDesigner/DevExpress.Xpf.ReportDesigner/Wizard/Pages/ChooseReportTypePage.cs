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
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Utils;
using System.Collections;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.POCO;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.Xpf.DataAccess.DataSourceWizard;
using DevExpress.Xpf.Editors;
using DevExpress.DataAccess.Wizard.Model;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	public class ChooseReportTypePage : ReportWizardPageBase {
		public static ChooseReportTypePage Create(ReportWizardModel wizardModel) {
			return ViewModelSource.Create(() => new ChooseReportTypePage(wizardModel));
		}
		IEnumerable<ReportType> reportTypes;
		public IEnumerable<ReportType> ReportTypes {
			get {
				return reportTypes ?? (reportTypes = GetAvailableTypes());
			}
		}
		public virtual ReportType ReportType {
			get { return ReportModel.ReportType; }
			set { ReportModel.ReportType = value; }
		}
		protected ChooseReportTypePage(ReportWizardModel wizardModel) : base(wizardModel) { }
		public override bool CanFinish {
			get {
				return ReportType == ReportType.Empty;
			}
		}
		public override bool CanGoForward {
			get {
				return ReportType != ReportType.Empty;
			}
		}
		protected override void NavigateToNextPage(WizardController wizardController) {
			if (ReportType == ReportType.Label)
				wizardController.NavigateTo(SelectLabelTypePage.Create(ReportWizardModel));
			else
				ReportWizardModel.StartDataSourceWizard(wizardController);
		}
		protected virtual IEnumerable<ReportType> GetAvailableTypes() {
			return new[] { ReportType.Empty, ReportType.Standard, ReportType.Label };
		}
	}
}
