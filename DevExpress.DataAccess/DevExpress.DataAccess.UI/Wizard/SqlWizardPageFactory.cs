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

using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Utils.IoC;
namespace DevExpress.DataAccess.UI.Wizard {
	public class SqlWizardPageFactory<TModel, TClient> : WizardPageFactoryBase<TModel, TClient>
		where TModel : ISqlDataSourceModel
		where TClient : ISqlDataSourceWizardClientUI {
		public SqlWizardPageFactory(TClient client) : base(client) { }
		protected override void RegisterDependencies(TClient client) { 
			RegisterDependencies(Container, client); 
		}
		internal static void RegisterDependencies(IntegrityContainer container, TClient client) {
			container.RegisterInstance(client.DataConnections);
			container.RegisterInstance(client.DBSchemaProvider ?? new DBSchemaProvider());
			container.RegisterInstance(client.RepositoryItemsProvider ?? DefaultRepositoryItemsProvider.Instance);
			container.RegisterInstance(client.PropertyGridServices);
			container.RegisterInstance(client.Options);
			container.RegisterInstance(client.DisplayNameProvider);
			container.RegisterInstance(client.DataProviders);
			container.RegisterInstance(client.CustomQueryValidator);
			container.RegisterType<IConnectionPropertiesPageView, ConnectionPropertiesPageView>();
			container.RegisterType<IChooseConnectionPageView, ChooseConnectionPageView>();
			container.RegisterType<ISaveConnectionPageView, SaveConnectionPageView>();
			container.RegisterType<IConfigureQueryPageView, ConfigureQueryPageView>();
			container.RegisterType<IConfigureParametersPageView, ConfigureParametersPageView>();
			container.RegisterType<ChooseConnectionPage<TModel>>();
			container.RegisterType<ConnectionPropertiesPage<TModel>>();
			container.RegisterType<SaveConnectionPage<TModel>>();
			container.RegisterType<ConfigureQueryPage<TModel>>();
			container.RegisterType<ConfigureSqlParametersPage<TModel>>();
		}
	}
}
