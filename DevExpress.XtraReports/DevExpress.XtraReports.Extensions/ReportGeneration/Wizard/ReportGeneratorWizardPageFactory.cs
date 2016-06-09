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

using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.XtraReports.ReportGeneration.Wizard.Presenters;
using DevExpress.XtraReports.ReportGeneration.Wizard.Views;
namespace DevExpress.XtraReports.ReportGeneration.Wizard {
	public class ReportGeneratorWizardPageFactory<TModel, TClient> : WizardPageFactory<TModel, TClient>
		where TModel : ReportGridDataModel
		where TClient : IDataSourceWizardClientUI {
		public ReportGeneratorWizardPageFactory(TClient client) : base(client){
		}
		protected override void RegisterDependencies(TClient client){
			base.RegisterDependencies(client);
			Container.RegisterType<WizardStartPage<TModel>>();
			Container.RegisterType<IWizardStartPageView, WizardStartPageView>();
			Container.RegisterType<WizardOptionsGeneralPage<TModel>>();
			Container.RegisterType<IWizardViewOptionsGeneral, WizardViewOptionsGeneral>();
			Container.RegisterType<WizardOptionsGroupingPage<TModel>>();
			Container.RegisterType<IWizardViewOptionsGrouping, WizardViewOptionsGrouping>();
			Container.RegisterType<WizardOptionsStylesPage<TModel>>();
			Container.RegisterType<IWizardViewOptionsStyles, WizardViewOptionsStyles>();
			Container.RegisterType<WizardEndPage<TModel>>();
			Container.RegisterType<IWizardEndPageView, WizardEndPageView>();
		}
	}
}
