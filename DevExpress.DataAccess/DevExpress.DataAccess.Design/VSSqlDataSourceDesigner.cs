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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data.Entity;
using DevExpress.Data.Utils;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Design;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.UI.Wizard.Clients;
using DevExpress.DataAccess.UI.Wizard.Services;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Design;
namespace DevExpress.DataAccess.Design {
	public class VSSqlDataSourceDesigner : XRSqlDataSourceDesigner {
		static VSSqlDataSourceDesigner() {
			SkinManager.EnableFormSkins();
		}
		DesignerVerbCollection verbs;
		protected override UserLookAndFeel GetLookAndFeel(IServiceProvider serviceProvider) {
			return VSLookAndFeelHelper.GetLookAndFeel(designerHost);
		}
		public override DesignerVerbCollection Verbs {
			get {
				if(verbs == null) { 
					this.verbs = base.Verbs;
					DXSmartTagsHelper.CreateDefaultVerbs(this, verbs);
				}
				return verbs;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			ProjectHelper.AddReference(component.Site, AssemblyInfo.SRAssemblyXpo + AssemblyInfo.FullAssemblyVersionExtension);
			ProjectHelper.AddReference(component.Site, AssemblyInfo.SRAssemblyDataAccess + AssemblyInfo.FullAssemblyVersionExtension);
		}
		protected override IConnectionProviderService GetConnectionProviderService() {
			return new VSConnectionProviderService(DataSource.Site);
		}
		protected override IConnectionStorageService GetConnectionStorageService() {
			return new VSConnectionStorageService(DataSource.Site);
		}
		protected override IConnectionStringsProvider GetConnectionStringsProvider() {
			return new VSConnectionStringsService(DataSource.Site);
		}
		protected override ICustomQueryValidator GetCustomQueryValidator() {
			return new VSCustomQueryValidator();
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
			var uiService = this.designerHost.GetService<IUIService>();
			IWin32Window owner = uiService != null ? uiService.GetDialogOwnerWindow() : null;
			using(var lookAndFeel = GetLookAndFeel(designerHost)) {
				var runner = new SqlDataSourceWizardRunner<SqlDataSourceModel>(lookAndFeel, owner);
				var parameterService = designerHost.GetService<IParameterService>();
				var dataConnectionParametersService = DataSource.GetService<IDataConnectionParametersService>();
				var connectionStorageService = designerHost.GetService<IConnectionStorageService>();
				var repositoryItemsProvider = designerHost.GetService<IRepositoryItemsProvider>();
				var connectionStringsProvider = designerHost.GetService<IConnectionStringsProvider>();
				var client = new SqlDataSourceWizardClientUI(connectionStorageService, parameterService, new DBSchemaProvider(),
					connectionStringsProvider) {
						DataConnectionParametersProvider = dataConnectionParametersService,
						RepositoryItemsProvider = repositoryItemsProvider, 
						Options = SqlWizardOptions.EnableCustomSql,
						CustomQueryValidator = new VSCustomQueryValidator()
					};
				if(runner.Run(client)) {
					SqlDataSourceModel wizardModel = runner.WizardModel;
					DataComponentCreator.SaveConnectionIfShould(wizardModel, connectionStorageService);
					componentChangeService.OnComponentChanging(DataSource, SqlQueryCollectionPropertyDescriptor);
					componentChangeService.OnComponentChanging(DataSource, ResultSchemaPropertyDescriptor);
					componentChangeService.OnComponentChanging(DataSource, ConnectionNamePropertyDescriptor);
					componentChangeService.OnComponentChanging(DataSource, ConnectionParametersPropertyDescriptor);
					DataSource.AssignConnection(wizardModel.DataConnection);
					SqlQuery query = wizardModel.Query;
					query.Name = DataSource.Queries.GenerateUniqueName(query);
					DataSource.Queries.Add(query);
					DataSource.SetResultSchemaPart(query.Name, wizardModel.DataSchema);
					componentChangeService.OnComponentChanged(DataSource, ConnectionNamePropertyDescriptor, null, null);
					componentChangeService.OnComponentChanged(DataSource, ConnectionParametersPropertyDescriptor, null, null);
					componentChangeService.OnComponentChanged(DataSource, SqlQueryCollectionPropertyDescriptor, null, null);
					componentChangeService.OnComponentChanged(DataSource, ResultSchemaPropertyDescriptor, null, null);
				}
			}
			UpdateVerbsState();
		}
	}
}
