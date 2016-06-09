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

using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Views;
namespace DevExpress.DataAccess.UI.Wizard {
	public class WizardPageFactory<TModel, TClient> : WizardPageFactoryBase<TModel, TClient>
		where TModel : IDataSourceModel
		where TClient : IDataSourceWizardClientUI {
		public WizardPageFactory(TClient client) : base(client) { }
		protected override void RegisterDependencies(TClient client) {
			Container.RegisterType<IChooseDataSourceTypePageView, ChooseDataSourceTypePageView>();
			Container.RegisterType<ChooseDataSourceTypePage<TModel>>();
			Container.RegisterType<IChooseDataSourceNamePageView, ChooseDataSourceNamePageView>();
			Container.RegisterType<ChooseDataSourceNamePage<TModel>>();
			Container.RegisterInstance(client.DataSourceTypes);
			Container.RegisterInstance(client.DataSourceNameCreationService);
			SqlWizardPageFactory<TModel, TClient>.RegisterDependencies(Container, client);
			EFWizardPageFactory<TModel, TClient>.RegisterDependencies(Container, client);
			ObjectWizardPageFactory<TModel, TClient>.RegisterDependecies(Container, client);
			ExcelWizardPageFactory<TModel, TClient>.RegisterDependencies(Container, client);
		}
	}
}
