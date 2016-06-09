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
using DevExpress.Data.WizardFramework;
using DevExpress.Utils.IoC;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Presenters;
using DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign.Views;
namespace DevExpress.XtraPrinting.Design.RemoteDocumentSourceDesign {
	public class RemoteDocumentSourceWizardPageFactory : IWizardPageFactory<RemoteDocumentSourceModel> {
		readonly IntegrityContainer container = new IntegrityContainer();
		protected IntegrityContainer Container { get { return container; } }
		public RemoteDocumentSourceWizardPageFactory(ReportTypesProvider reportTypesProvider, IAppConfigHelper configHelper) {
			RegisterDependencies(reportTypesProvider, configHelper);
		}
		void RegisterDependencies(ReportTypesProvider reportTypesProvider, IAppConfigHelper configHelper) {
			container.RegisterType<ChooseDocumentSourceTypePresenter<IChooseDocumentSourceTypeView>>();
			container.RegisterType<SetReportServerCredentialsPresenter<ISetReportServerCredentialsView>>();
			container.RegisterType<SetReportServerEndpointPresenter<ISetReportServerEndpointView>>();
			container.RegisterType<ChooseRemoteReportPresenter<IChooseRemoteReportView>>();
			container.RegisterType<ISetReportServiceReportNamePresenter, SetReportServiceReportNamePresenter<ISetReportServiceReportNameView>>();
			container.RegisterType<IChooseDocumentSourceTypeView, ChooseDocumentSourceTypeView>();
			container.RegisterType<ISetReportServerCredentialsView, SetReportServerCredentialsView>();
			container.RegisterType<IChooseRemoteReportView, ChooseRemoteReportView>();
			container.RegisterType<ISetReportServiceReportNameView, SetReportServiceReportNameView>();
			container.RegisterType<ISetReportServerEndpointView, SetReportServerEndpointView>();
			container.RegisterInstance(reportTypesProvider);
			container.RegisterInstance(configHelper);
		}
		public IRemoteDocumentSourceWizardPage GetPage(Type pageType) {
			return ((IWizardPageFactory<RemoteDocumentSourceModel>)this).GetPage(pageType) as IRemoteDocumentSourceWizardPage;
		}
		IWizardPage<RemoteDocumentSourceModel> IWizardPageFactory<RemoteDocumentSourceModel>.GetPage(Type pageType) {
			return (IRemoteDocumentSourceWizardPage)container.Resolve(pageType);
		}
	}
}
