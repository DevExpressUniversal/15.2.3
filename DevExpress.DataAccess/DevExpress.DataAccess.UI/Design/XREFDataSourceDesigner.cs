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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Security.Permissions;
using DevExpress.Data.Entity;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.EntityFramework;
using DevExpress.DataAccess.UI.EntityFramework;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.Entity.ProjectModel;
using DevExpress.LookAndFeel.DesignService;
namespace DevExpress.DataAccess.UI.Design {
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class XREFDataSourceDesigner : EFDataSourceDesignerBase {
		protected EFDataSource DataSource {
			[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")] get { return (EFDataSource) Component; }
		}
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public XREFDataSourceDesigner() : base() {}
		[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
		public override void Initialize(IComponent component) {
			base.Initialize(component);
		}
		protected override ISolutionTypesProvider CreateSolutionTypesProvider() {
			Assembly asm = DataSource.Connection != null && DataSource.Connection.ConnectionParameters != null && DataSource.Connection.ConnectionParameters.Source != null ?
				DataSource.Connection.ConnectionParameters.Source.Assembly : Assembly.GetEntryAssembly();
			return EntityServiceHelper.GetRuntimeSolutionProvider(asm);
		}
		protected override IConnectionStringsProvider CreateConnectionStringsProvider() {
			return new RuntimeConnectionStringsProvider();
		}
		protected override IConnectionStorageService CreateConnectionStorageService() {
			return new ConnectionStorageService();
		}
		protected override void AddServicesToDesignerHost() {
			if(this.designerHost.GetService<ILookAndFeelService>() == null)
				designerHost.AddService(typeof(ILookAndFeelService), lookAndFeelService);
			if(this.designerHost.GetService<IConnectionStorageService>() == null)
				this.designerHost.AddService(typeof(IConnectionStorageService), connectionStorageService);
			if(designerHost.GetService<ISolutionTypesProvider>() == null)
				designerHost.AddService(typeof(ISolutionTypesProvider), solutionTypesProvider);
			if(designerHost.GetService<IConnectionStringsProvider>() == null)
				designerHost.AddService(typeof(IConnectionStringsProvider), connectionStringsProvider);
			IServiceProvider serviceProvider = designerHost.RootComponent as IServiceProvider;
			if(serviceProvider != null) {
				IServiceContainer serviceContainer = serviceProvider.GetService<IServiceContainer>();
				if(serviceContainer.GetService<ISolutionTypesProvider>() == null)
					serviceContainer.AddService(typeof(ISolutionTypesProvider), solutionTypesProvider);
				if(serviceContainer.GetService<IConnectionStringsProvider>() == null)
					serviceContainer.AddService(typeof(IConnectionStringsProvider), connectionStringsProvider);
			}
		}
		protected override void AddServicesToDataSource() {
			IServiceContainer DataSourceServiceContainer = (IServiceContainer) DataSource;
			DataSourceServiceContainer.RemoveService(typeof(ISolutionTypesProvider));
			DataSourceServiceContainer.AddService(typeof(ISolutionTypesProvider), solutionTypesProvider);
			DataSourceServiceContainer.RemoveService(typeof(IConnectionStringsProvider));
			DataSourceServiceContainer.AddService(typeof(IConnectionStringsProvider), connectionStringsProvider);
		}
		protected override void EditVerbHandler(object sender, EventArgs a) {
			PropertyDescriptor pdConnectionParameters = TypeDescriptor.GetProperties(typeof(EFDataSource))["ConnectionParameters"];
			PropertyDescriptor pdStoredProcedures = TypeDescriptor.GetProperties(typeof(EFDataSource))["StoredProcedures"];
			using(DesignerTransaction transaction = designerHost.CreateTransaction("Edit EFDataSource")) {
				componentChangeService.OnComponentChanging(Component, pdConnectionParameters);
				componentChangeService.OnComponentChanging(Component, pdStoredProcedures);
				DefaultWizardRunnerContext context = new DefaultWizardRunnerContext(lookAndFeelService.LookAndFeel, owner);
				if(!DataSource.Configure(context, solutionTypesProvider, connectionStringsProvider, connectionStorageService, parameterService))
					transaction.Cancel();
				componentChangeService.OnComponentChanged(Component, pdConnectionParameters, null, null);
				componentChangeService.OnComponentChanged(Component, pdStoredProcedures, null, null);
				transaction.Commit();
			}
		}
	}
}
