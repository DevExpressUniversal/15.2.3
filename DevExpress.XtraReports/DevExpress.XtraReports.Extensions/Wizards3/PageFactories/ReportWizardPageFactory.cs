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
using DevExpress.Data.XtraReports.Labels;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Data.XtraReports.Wizard.Presenters;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.XtraReports.Wizards3.Presenters;
using DevExpress.XtraReports.Wizards3.Views;
using System.Linq;
using DevExpress.DataAccess.Native.ObjectBinding;
namespace DevExpress.XtraReports.Wizards3 {
	public class XtraReportWizardPageFactory<TModel, TClient> : WizardPageFactory<TModel, TClient> 
		where TModel : XtraReportModel 
		where TClient : IDataSourceWizardClientUI 
	{
		public XtraReportWizardPageFactory(TClient client) : base(client) { }
		protected override void RegisterDependencies(TClient client) {
			base.RegisterDependencies(client);
			Container.RegisterInstance(new LabelProductRepositoryFactory().Create());
			Container.RegisterType<IColumnInfoCache, ColumnInfoCache>();
			Container.RegisterType<ChooseReportTypePageEx<TModel>>();
			Container.RegisterType<LayoutChooseReportTypePage>();
			Container.RegisterType<SelectLabelTypePage<TModel>>();
			Container.RegisterType<CustomizeLabelPage<TModel>>();
			Container.RegisterType<Wizards3.Presenters.SelectColumnsPage<TModel>>();
			Container.RegisterType<AddGroupingLevelPage<TModel>>();
			Container.RegisterType<ChooseSummaryOptionsPage<TModel>>();
			Container.RegisterType<ChooseReportLayoutPage<TModel>>();
			Container.RegisterType<ChooseReportStylePage<TModel>>();
			Container.RegisterType<SetReportTitlePage<TModel>>();
			Container.RegisterType<ConfigureSqlParametersPage<TModel>, ConfigureSqlParametersPageEx<TModel>>();
			Container.RegisterType<ConfigureEFStoredProceduresPage<TModel>, ConfigureEFStoredProceduresPageEx<TModel>>();
			Container.RegisterType<ConfigureEFConnectionStringPage<TModel>, ConfigureEFConnectionStringPageEx<TModel>>();
			Container.RegisterType<ChooseEFConnectionStringPage<TModel>, ChooseEFConnectionStringPageEx<TModel>>();
			Container.RegisterType<ChooseEFDataMemberPage<TModel>, ChooseEFDataMemberPageEx<TModel>>();
			Container.RegisterType<ConfigureQueryPage<TModel>, ConfigureQueryPageEx<TModel>>();
			Container.RegisterType<ChooseObjectTypePage<TModel>, ChooseObjectTypePageEx<TModel>>();
			Container.RegisterType<ChooseObjectMemberPage<TModel>, ChooseObjectMemberPageEx<TModel>>();
			Container.RegisterType<ObjectMemberParametersPage<TModel>, ObjectMemberParametersPageEx<TModel>>();
			Container.RegisterType<ChooseObjectBindingModePage<TModel>, ChooseObjectBindingModePageEx<TModel>>();
			Container.RegisterType<ChooseObjectConstructorPage<TModel>, ChooseObjectConstructorPageEx<TModel>>();
			Container.RegisterType<ObjectConstructorParametersPage<TModel>, ObjectConstructorParametersPageEx<TModel>>();
			Container.RegisterType<ConfigureExcelFileColumnsPage<TModel>, ConfigureExcelFileColumnsPageEx<TModel>>();
			Container.RegisterType<IChooseReportTypePageViewExtended, ChooseReportTypePageView>();
			Container.RegisterType<ISelectLabelTypePageView, SelectLabelTypePageView>();
			Container.RegisterType<ICustomizeLabelPageView, CustomizeLabelPageView>();
			Container.RegisterType<ISelectColumnsPageView, SelectColumnsPageView>();
			Container.RegisterType<IAddGroupingLevelPageView, AddGroupingLevelPageView>();
			Container.RegisterType<IChooseSummaryOptionsPageView, ChooseSummaryOptionsPageView>();
			Container.RegisterType<IChooseReportLayoutPageView, ChooseReportLayoutPageView>();
			Container.RegisterType<IChooseReportStylePageView, ChooseReportStylePageView>();
			Container.RegisterType<ISetReportTitlePageView, SetReportTitlePageView>();
			Container.RegisterType<IChooseObjectConstructorPageView, ChooseObjectConstructorPageViewEx>();
		}
	}
}
namespace DevExpress.XtraReports.Wizards3.Views {
	public class ChooseObjectConstructorPageViewEx : ChooseObjectConstructorPageView {
		protected override string GetCtorDisplayText(ParametersViewInfo info, string defaultValue) {
			if(info.Data == null)
				return String.Empty;
			string parameters = String.Join(", ", info.Data.Select(pi => TypeNamesHelper.ShortName(pi.ParameterType) + " " + pi.Name));
			return "ctor(" + parameters  + ")"; 
		}
	}
}
