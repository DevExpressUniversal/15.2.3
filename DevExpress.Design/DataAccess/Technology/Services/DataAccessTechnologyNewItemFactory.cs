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

namespace DevExpress.Design.DataAccess {
	using System.Collections.Generic;
	public class DefaultDataAccessTechnologyNewItemFactory : IDataAccessTechnologyNewItemService {
		Design.UI.IServiceContainer serviceContainer;
		UI.IDataAccessConfiguratorContext context;
		public DefaultDataAccessTechnologyNewItemFactory()
			: this(null, null) {
		}
		public DefaultDataAccessTechnologyNewItemFactory(Design.UI.IServiceContainer serviceContainer, UI.IDataAccessConfiguratorContext context) {
			this.serviceContainer = serviceContainer;
			this.context = context;
		}
		interface INewItemCreator {
			void Create();
			void Create(EnvDTE.DTE dte);
		}
		interface INewItemCreatorEx : INewItemCreator {
			void SetServiceContainerAndContext(Design.UI.IServiceContainer serviceContainer, UI.IDataAccessConfiguratorContext context);
		}
		#region static
		static IDictionary<DataAccessTechnologyCodeName, INewItemCreator> creators;
		static DefaultDataAccessTechnologyNewItemFactory() {
			creators = new Dictionary<DataAccessTechnologyCodeName, INewItemCreator>();
			creators.Add(DataAccessTechnologyCodeName.TypedDataSet, new TypedDataSetItemCreator());
			creators.Add(DataAccessTechnologyCodeName.SQLDataSource, new SQLDataSourceItemCreator());
			creators.Add(DataAccessTechnologyCodeName.ExcelDataSource, new ExcelDataSourceItemCreator());
			creators.Add(DataAccessTechnologyCodeName.EntityFramework, new EntityFrameworkItemCreator());
			creators.Add(DataAccessTechnologyCodeName.LinqToSql, new LinqToSqlItemCreator());
			creators.Add(DataAccessTechnologyCodeName.Wcf, new WcfItemCreator());
			creators.Add(DataAccessTechnologyCodeName.XPO, new XPOItemCreator());
		}
		#endregion static
		void IDataAccessTechnologyNewItemService.Create(DataAccessTechnologyCodeName codeName) {
			INewItemCreator creator;
			if(creators.TryGetValue(codeName, out creator)) {
				INewItemCreatorEx ex = creator as INewItemCreatorEx;
				if(ex != null)
					ex.SetServiceContainerAndContext(serviceContainer, context);
				Design.UI.Platform.Queue(creator.Create);
			}
		}
		#region Real Creators
		class TypedDataSetItemCreator : Utils.Design.ExecuteCommandCreator, INewItemCreator {
			public TypedDataSetItemCreator()
				: base(DevExpress.Design.UI.Platform.IsVS2012OrAbove ? "Project.AddNewDataSource" : "Data.AddNewDataSource") {
			}
		}
		abstract class DataAccessComponentDataSourceItemCreator : INewItemCreatorEx {
			Design.UI.IServiceContainer serviceContainer;
			System.Threading.SynchronizationContext synchronizationContext;
			UI.IDataAccessConfiguratorContext configurationContext;
			public void SetServiceContainerAndContext(Design.UI.IServiceContainer serviceContainer, UI.IDataAccessConfiguratorContext context) {
				this.serviceContainer = serviceContainer;
				this.configurationContext = context;
				if(serviceContainer != null)
					this.synchronizationContext = System.Threading.SynchronizationContext.Current;
			}
			public void Create(EnvDTE.DTE dte) {
				if(dte == null) return;
				CheckDevExpressDataAccessReference(dte);
				synchronizationContext.Post((state) =>
				{
					object[] parameters = (object[])state;
					var svcContainer = parameters[0] as Design.UI.IServiceContainer;
					if(svcContainer != null) {
						System.Action<object> configureCompletion = null;
						object componentDataSource = CreateDataSource(svcContainer, GetDataSourceComponentTypeName(), ref configureCompletion);
						if(componentDataSource != null) {
							var configurationService = serviceContainer.Resolve<IDataAccessConfigurationService>();
							configurationService.Configure(componentDataSource, svcContainer, parameters[1] as UI.IDataAccessConfiguratorContext);
							if(configureCompletion != null)
								configureCompletion(componentDataSource);
						}
					}
				}, new object[] { serviceContainer, configurationContext });
			}
			public void Create() {
				using(new Utils.Design.MessageFilter())
					Create(Utils.Design.DTEHelper.GetCurrentDTE());
			}
			protected abstract string GetDataSourceComponentTypeName();
			void CheckDevExpressDataAccessReference(EnvDTE.DTE dte) {
				EnvDTE.Project project = Utils.Design.DTEHelper.GetCurrentProject(dte);
				if(project == null) return;
				try {
					if(!Utils.Design.ProjectHelper.IsReferenceExists(project, AssemblyInfo.SRAssemblyDataAccess))
						Utils.Design.ProjectHelper.AddReference(project, AssemblyInfo.SRAssemblyDataAccess + AssemblyInfo.FullAssemblyVersionExtension);
				}
				catch { }
			}
			static object CreateDataSource(Design.UI.IServiceContainer serviceContainer, string dataSourceComponentTypeName, ref System.Action<object> completionCallback) {
				object component = null;
				if(serviceContainer != null) {
					try {
						var componentsProvider = serviceContainer.Resolve<IDataAccessTechnologyComponentsProvider>();
						if(componentsProvider != null) {
							var defaultValues = new Dictionary<string, object>() { { DataSourceConfigurationConstants.ForceInitializeNewComponent, true } };
							component = componentsProvider.CreateComponent(dataSourceComponentTypeName, defaultValues);
							object componentDesignerCallback;
							if(defaultValues.TryGetValue(DataSourceConfigurationConstants.NewComponentDesignerCallback, out componentDesignerCallback))
								completionCallback = componentDesignerCallback as System.Action<object>;
						}
					}
					catch { }
				}
				return component;
			}
		}
		class SQLDataSourceItemCreator : DataAccessComponentDataSourceItemCreator {
			protected override string GetDataSourceComponentTypeName() {
				return "DevExpress.DataAccess.Sql.SqlDataSource";
			}
		}
		class ExcelDataSourceItemCreator : DataAccessComponentDataSourceItemCreator {
			protected override string GetDataSourceComponentTypeName() {
				return "DevExpress.DataAccess.Excel.ExcelDataSource";
			}
		}
		public class EntityFrameworkItemCreator : Utils.Design.ProjectItemCreator, INewItemCreator {
			public EntityFrameworkItemCreator()
				: base("Data", "ADO.NET Entity Data Model") {
			}
		}
		class LinqToSqlItemCreator : Utils.Design.ProjectItemCreator, INewItemCreator {
			public LinqToSqlItemCreator()
				: base("Data", "LINQ to SQL Classes") {
			}
		}
		class WcfItemCreator : Utils.Design.ExecuteCommandCreator, INewItemCreator {
			public WcfItemCreator()
				: base("Project.AddServiceReference") {
			}
		}
		public class XPOItemCreator : Utils.Design.ProjectItemCreator, INewItemCreator {
			public XPOItemCreator()
				: base("DevExpress", "DevExpress ORM Data Model Wizard") {
			}
		}
		#endregion Real Creators
	}
}
