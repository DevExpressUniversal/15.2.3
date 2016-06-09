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
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.UI.Wizard.Views;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.Utils.IoC;
namespace DevExpress.DataAccess.UI.Wizard {
	public class ObjectWizardPageFactory<TModel, TClient> : WizardPageFactoryBase<TModel, TClient>
		where TModel : IObjectDataSourceModel
		where TClient : IObjectDataSourceWizardClientUI {
		public ObjectWizardPageFactory(TClient client) : base(client) { }
		#region Overrides of WizardPageFactoryBase<TModel,TClient>
		protected override void RegisterDependencies(TClient client) {
			RegisterDependecies(Container, client);
		}
		#endregion
		internal static void RegisterDependecies(IntegrityContainer container, TClient client) {
			container.RegisterInstance(client.ParameterService);
			container.RegisterInstance(client.SolutionTypesProvider);
			container.RegisterInstance(client.OperationMode);
			container.RegisterInstance(client.RepositoryItemsProvider ?? DefaultRepositoryItemsProvider.Instance);
			container.RegisterType<IChooseObjectAssemblyPageView, ChooseObjectAssemblyPageView>();
			container.RegisterType<IChooseObjectTypePageView, ChooseObjectTypePageView>();
			container.RegisterType<IChooseObjectMemberPageView, ChooseObjectMemberPageView>();
			container.RegisterType<IObjectMemberParametersPageView, ObjectMemberParametersPageView>();
			container.RegisterType<IChooseObjectBindingModePageView, ChooseObjectBindingModePageView>();
			container.RegisterType<IChooseObjectConstructorPageView, ChooseObjectConstructorPageView>();
			container.RegisterType<IObjectConstructorParametersPageView, ObjectConstructorParametersPageView>();
			container.RegisterType<ChooseObjectAssemblyPage<TModel>>();
			container.RegisterType<ChooseObjectTypePage<TModel>>();
			container.RegisterType<ChooseObjectMemberPage<TModel>>();
			container.RegisterType<ObjectMemberParametersPage<TModel>>();
			container.RegisterType<ChooseObjectBindingModePage<TModel>>();
			container.RegisterType<ChooseObjectConstructorPage<TModel>>();
			container.RegisterType<ObjectConstructorParametersPage<TModel>>();
		}
	}
}
