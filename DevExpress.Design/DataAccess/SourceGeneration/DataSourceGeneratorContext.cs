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
	using DevExpress.Design.CodeGenerator;
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public abstract class BaseDataSourceGeneratorContext : IDataSourceGeneratorContext {
		protected IDataAccessMetadata metadata;
		public BaseDataSourceGeneratorContext(IModelItem modelItem, IDataSourceSettingsModel settingsModel, IDataAccessMetadata metadata) {
			this.metadata = metadata;
			this.ModelItem = modelItem;
			this.SettingsModel = settingsModel;
		}
		#region IDataSourceGeneratorContext Members
		public IModelItem ModelItem {
			get;
			private set;
		}
		public string DataSourceProperty {
			get { return metadata.DataSourceProperty; }
		}
		public string DataMemberProperty {
			get { return metadata.DataMemberProperty; }
		}
		public string DesignTimeElementTypeProperty {
			get {
				IDashboardDataAccessMetadata dashboardMetadata = metadata as IDashboardDataAccessMetadata;
				return (dashboardMetadata != null) ? dashboardMetadata.DesignTimeElementTypeProperty : null;
			}
		}
		public string OLAPConnectionStringProperty {
			get {
				IOLAPDataAccessMetadata olapMetadata = metadata as IOLAPDataAccessMetadata;
				return (olapMetadata != null) ? olapMetadata.OLAPConnectionStringProperty : null;
			}
		}
		public string OLAPDataProviderProperty {
			get {
				IOLAPDataAccessMetadata olapMetadata = metadata as IOLAPDataAccessMetadata;
				return (olapMetadata != null) ? olapMetadata.OLAPDataProviderProperty : null;
			}
		}
		public IDataSourceSettingsModel SettingsModel {
			get;
			private set;
		}
		#endregion
		public IModelItem CreateDataSource() {
			return CreateDataSource(SettingsModel.SourceType);
		}
		public abstract IModelItem CreateDataSource(System.Type dataSourceType);
		public abstract void SetDataMember();
		public abstract void ClearDataMember();
		public abstract void SetDataSource(IModelItem dataSourceItem);
		public abstract void ClearDataSource();
		public abstract void SetCustomBindingProperty(string propertyName, object value);
		public abstract void ClearCustomBindingProperties();
		public IModelItemExpression GenerateDataSourceBindingExpression(object[] parameters, string format) {
			return GenerateBindingExpression(SettingsModel.SourceType, parameters, format);
		}
		public abstract IModelItemExpression GenerateBindingExpression(System.Type dataSourceType, object[] parameters, string format);
		public abstract IModelItemExpression GenerateExpression(object[] parameters, string format);
		public void GenerateDataSourceAssignment(IModelItemExpression expression) {
			GenerateDataSourceAssignment(ModelItem, expression);
		}
		public void GenerateDataSourceAssignment(IModelItem modelItem, IModelItemExpression expression) {
			GenerateParameterAssignment(modelItem, DataSourceProperty, expression);
		}
		public abstract void GenerateParameterAssignment(IModelItem modelItem, string parameterName, IModelItemExpression expression);
		public abstract void GenerateEvent(IModelItem modelItem, string eventName, System.Type eventArgsType, IModelItemExpression expression);
		public abstract void GenerateCode(IModelItemExpression expression);
		public abstract void GenerateUsing(string namespaceString);
		public virtual void SaveActiveDocument() {
			if(DevExpress.Design.UI.Platform.IsDesignMode) {
				IModelServiceProvider serviceProvider = ModelItem.EditingContext.ServiceProvider;
				var dte = serviceProvider.GetService<EnvDTE.DTE>();
				if(dte != null && dte.Solution.IsOpen)
					dte.ActiveDocument.Save();
			}
		}
		public abstract void ShowCode();
		#region Code Tools
		protected static class CodeGenerator {
			public static int AddUsing(IModelServiceProvider serviceProvider, string namespaceString, string strCode) {
				return CodeGeneratorHelper.AddUsing(serviceProvider, namespaceString, strCode);
			}
			public static int AppendConstructorBody(IModelServiceProvider serviceProvider, string strCode) {
				return CodeGeneratorHelper.AppendConstructorBody(serviceProvider, strCode);
			}
			public static int AddEventHandler(IModelServiceProvider serviceProvider, string target, string eventName, string eventHandlerName, System.Type eventArgsType, string strCode, PlatformCodeName platform) {
				DTEEventInfo info = new DTEEventInfo(eventHandlerName, eventName)
				{
					Code = strCode,
					Comment = DataSourceGeneratorFormatter.GetAutoGeneratedEventComment(platform),
					EventSubscriptionComment = DataSourceGeneratorFormatter.GetAutoGeneratedLineComment(platform)
				};
				info.AddParameter("sender", typeof(object));
				info.AddParameter("e", eventArgsType);
				return CodeGeneratorHelper.AddEventHandler(serviceProvider, target, info);
			}
		}
		#endregion Code Tools
	}
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public abstract class BaseDataSourceGeneratorContextFactory : IDataSourceGeneratorContextFactory {
		IDataSourceGeneratorContext IDataSourceGeneratorContextFactory.GetContext(IModelItem modelItem, IDataSourceSettingsModel settingsModel, IDataAccessMetadata metadata) {
			return GetContextCore(modelItem, settingsModel, metadata);
		}
		protected abstract IDataSourceGeneratorContext GetContextCore(IModelItem modelItem, IDataSourceSettingsModel settingsModel, IDataAccessMetadata metadata);
	}
}
