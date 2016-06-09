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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.XtraReports.Wizards3.Localization;
using DevExpress.XtraReports.Wizards3.Presenters;
using DevExpress.XtraReports.Wizards3.Views;
namespace DevExpress.XtraReports.Wizards3 {
	public class LayoutWizardRunner<TModel> : DataSourceWizardRunnerBase<TModel, IDataSourceWizardClientUI> where TModel : XtraReportModel, new() {
		public LayoutWizardRunner(UserLookAndFeel lookAndFeel, IWin32Window owner) : base(lookAndFeel, owner) { }
		internal LayoutWizardRunner(IWizardRunnerContext context) : base(context) { }
		protected override WizardPageFactoryBase<TModel, IDataSourceWizardClientUI> CreatePageFactory(IDataSourceWizardClientUI client) {
			return new XtraReportWizardPageFactory<TModel, IDataSourceWizardClientUI>(client);
		}
		protected override Type StartPage {
			get { return typeof(LayoutChooseReportTypePage); }
		}
		protected override string WizardTitle {
			get { return ReportDesignerLocalizer.GetString(ReportBoxDesignerStringId.Wizard_WindowTitleReport); }
		}
	}
	class LayoutChooseReportTypePage : ChooseReportTypePageEx<XtraReportModel> {
		public override Type GetNextPageType() {
			return View.ReportType == ReportType.Standard ? 
				typeof(SelectColumnsPage<XtraReportModel>) :
				base.GetNextPageType();
		}
		public LayoutChooseReportTypePage(IChooseReportTypePageViewExtended view, IEnumerable<SqlDataConnection> dataConnections, DataSourceTypes dataSourceTypes)
			: base(view, dataConnections, dataSourceTypes) { 
		}
	}
}
