#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon.DataSourceWizard;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DataSourceWizard;
using DevExpress.DashboardWin.Native;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.LookAndFeel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DataAccess.Wizard.Presenters;
namespace DevExpress.DashboardWin.Native {
	public class DashboardDataSourceWizardRunner<TModel> : DataSourceWizardRunnerBase<TModel, IDashboardDataSourceWizardClientUI> where TModel : IDataSourceModel, new() {
		bool ShowDataSourceNamePage { get; set; }
		public DashboardDataSourceWizardRunner(UserLookAndFeel lookAndFeel, IWin32Window owner, bool showDataSourceNamePage) : base(lookAndFeel, owner) {
			ShowDataSourceNamePage = showDataSourceNamePage;
		}
		protected override Type StartPage { get { return ShowDataSourceNamePage ? typeof(ChooseDataSourceNamePage<TModel>) : typeof(DashboardChooseDataSourceTypePage<TModel>); } }
		protected override string WizardTitle {
			get { return DataAccessUILocalizer.GetString(DataAccessUIStringId.WizardTitleDatasource); }
		}
		protected override WizardPageFactoryBase<TModel, IDashboardDataSourceWizardClientUI> CreatePageFactory(IDashboardDataSourceWizardClientUI client) {
			return new DashboardWizardPageFactory<TModel, IDashboardDataSourceWizardClientUI>(client);
		}
	}
	public class DashboardOlapParametersWizardRunner<TModel> : DashboardDataSourceWizardRunner<TModel> where TModel : IDataSourceModel, new() {
		public DashboardOlapParametersWizardRunner(UserLookAndFeel lookAndFeel, IWin32Window owner) : base(lookAndFeel, owner, false) { }
		protected override Type StartPage { get { return typeof(ConfigureOlapParametersPage<TModel>); } }
	}
}
