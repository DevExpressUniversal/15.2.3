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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.DataSourceWizard;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.DataSourceWizard;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.DashboardWin.Native {
	public class DashboardWizardPageFactory<TModel, TClient> : WizardPageFactory<TModel, TClient> 
		where TModel : IDataSourceModel
		where TClient : IDashboardDataSourceWizardClientUI {		
		public DashboardWizardPageFactory(TClient client) : base(client) { }
		protected override void RegisterDependencies(TClient client) {
			base.RegisterDependencies(client);
			Container.RegisterInstance<ISupportedDataSourceTypesService>(client.DataSourceTypesService);
			Container.RegisterInstance<IDTEService>(client.DTEService);
			Container.RegisterType<IChooseDataSourceTypePageView, DashboardChooseDataSourceTypePageView>();
			Container.RegisterType<DashboardChooseDataSourceTypePage<TModel>>();
			Container.RegisterType<DashboardChooseDataSourceNamePage<TModel>>();
			Container.RegisterType<IChooseConnectionPageView, ChooseConnectionPageView>();
			Container.RegisterType<ChooseOlapConnectionPage<TModel>>();
			Container.RegisterType<IConfigureOlapParametersPageView, ConfigureOlapParametersPageView>();
			Container.RegisterType<ConfigureOlapParametersPage<TModel>>();
			Container.RegisterType<ISaveConnectionPageView, SaveConnectionPageView>();
			Container.RegisterType<SaveOlapConnectionPage<TModel>>();
			Container.RegisterType<IConfigureXmlSchemaPageView, ConfigureXmlSchemaPageView>();
			Container.RegisterType<ConfigureXmlSchemaPage<TModel>>();
		}
	}	
}
