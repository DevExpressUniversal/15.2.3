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
	[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public abstract class DataSourceGeneratorBase : IDataSourceGenerator {
		public void Generate(IDataSourceGeneratorContext context) {
			string dataSourceGeneration = DataAccessLocalizer.GetString(DataAccessLocalizerStringId.GenerateDataSourceTransactionName);
			IModelItem dataSourceItem;
			using(var changeRoot = context.ModelItem.BeginEdit(dataSourceGeneration)) {
				dataSourceItem = CreateDataSourceItem(context);
				if(dataSourceItem != null)
					InitializeDataSourceItem(dataSourceItem, context);
				SetCustomBindingProperties(context);
				SetDataSource(dataSourceItem, context);
				changeRoot.Complete();
			}
			context.SaveActiveDocument();
			bool isCodeBehindGenerated = false;
			string codeBehindGeneration = DataAccessLocalizer.GetString(DataAccessLocalizerStringId.GenerateCodeBehindTransactionName);
			using(var changeRoot = context.ModelItem.BeginEdit(codeBehindGeneration))
				isCodeBehindGenerated = GenerateCodeBehind(dataSourceItem, context);
			if(CanShowCode(isCodeBehindGenerated, context.SettingsModel.ShowCodeBehind))
				context.ShowCode();
		}
		protected bool CanShowCode(bool codeBehindGenerated, bool showCode) {
			return DevExpress.Design.UI.Platform.IsDesignMode &&
				(showCode && codeBehindGenerated);
		}
		protected virtual void SetCustomBindingProperties(IDataSourceGeneratorContext context) {
			context.ClearCustomBindingProperties();
			foreach(var cp in context.SettingsModel.CustomBindingProperties)
				context.SetCustomBindingProperty(cp.Key, cp.Value);
		}
		protected virtual IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource();
		}
		protected virtual bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			return false;
		}
		protected abstract void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context);
		protected virtual void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.SetDataSource(dataSourceItem);
		}
		protected System.Type GetSourceType(IDataSourceGeneratorContext context) {
			return context.SettingsModel.SourceType;
		}
		protected System.Type GetElementType(IDataSourceGeneratorContext context) {
			return ((IDataTypeInfo)context.SettingsModel.SelectedElement).ElementType;
		}
		protected System.Type GetTableType(IDataSourceGeneratorContext context) {
			return ((IDataTableInfo)context.SettingsModel.SelectedElement).TableType;
		}
		protected System.Type GetTableRowType(IDataSourceGeneratorContext context) {
			return ((IDataTableInfo)context.SettingsModel.SelectedElement).RowType;
		}
		protected System.Type GetTableAdapterType(IDataSourceGeneratorContext context) {
			System.Type tableType = GetTableType(context);
			string tableTypeName = Metadata.TypeConstraint.GetFullName(tableType);
			var adapterConstraint = new Metadata.MethodConstraint(
				"System.ComponentModel.Component", "System.Int32", new string[] { tableTypeName });
			System.Type tableAdapterType = null;
			var sourceType = GetSourceType(context);
			if(sourceType != null) {
				var adapterTypesFromSourceAssembly = DataTypeInfoHelper.GetDataTypes(sourceType.Assembly.GetTypes(), adapterConstraint.Match);
				tableAdapterType = System.Linq.Enumerable.FirstOrDefault(adapterTypesFromSourceAssembly);
			}
			if(tableAdapterType == null) {
				var adapterTypes = DataTypeInfoHelper.GetLocalDataTypes(adapterConstraint.Match);
				tableAdapterType = System.Linq.Enumerable.FirstOrDefault(adapterTypes);
			}
			return tableAdapterType;
		}
		protected IModelItemExpression GenerateAdapterFillExpression(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			System.Type adapterType = GetTableAdapterType(context);
			return GenerateAdapterFillExpression(dataSourceItem, context, adapterType);
		}
		protected IModelItemExpression GenerateAdapterFillExpression(IModelItem dataSourceItem, IDataSourceGeneratorContext context, System.Type adapterType) {
			IModelItemExpression result = null;
			if(adapterType != null) {
				IModelItem dataAdapterItem = context.CreateDataSource(adapterType);
				result = context.GenerateExpression(
					new object[] { dataAdapterItem, dataSourceItem, GetElementName(context) }, "{0}.Fill({1}.{2})");
			}
			return result;
		}
		protected IModelItemExpression GenerateSqlDataSourceFillExpression(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { dataSourceItem, },
				DataSourceGeneratorFormatter.GetSqlDataSourceFillFormat());
		}
		protected IModelItemExpression GenerateExcelDataSourceFillExpression(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { dataSourceItem, },
				DataSourceGeneratorFormatter.GetExcelDataSourceFillFormat());
		}
		protected object GetComponent(IDataSourceGeneratorContext context) {
			return context.SettingsModel.Component;
		}
		protected IModelItem GetComponentModelItem(IDataSourceGeneratorContext context) {
			var modelService = context.ModelItem.EditingContext.ServiceProvider.GetService<IModelService>();
			return modelService.CreateModelItem(GetComponent(context));
		}
		protected IModelItemExpression GenerateReadXmlExpression(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			string xmlPath = ((IXmlDataSetSettingsModel)context.SettingsModel).XmlPath;
			return context.GenerateExpression(new object[] { xmlPath },
				DataSourceGeneratorFormatter.GetReadXmlFormat());
		}
		protected bool IsEntityFrameworkDbContextBinding(IDataSourceGeneratorContext context) {
			var dbContextConstraint = new Metadata.PropertyConstraint("System.Data.Entity.DbContext", "System.Data.Entity.DbSet`1"); 
			return dbContextConstraint.Match(GetSourceType(context));
		}
		protected IModelItemExpression GenerateDBContextLoadExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { GetSourceType(context), GetElementName(context) },
				DataSourceGeneratorFormatter.GetDBContextLoadFormat());
		}
		protected IModelItemExpression GenerateDbContextBindingExpression(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(new object[] { GetElementName(context) },
				DataSourceGeneratorFormatter.GetDBContextBindingFormat());
		}
		protected string GetElementName(IDataSourceGeneratorContext context) {
			return context.SettingsModel.SelectedElement.Name;
		}
		protected string GetQueryableEventFormat(string constructorParameters, string sourceParameter, string sourceName, string dismissQueryableEventName = "DismissQueryable") {
			return DataSourceGeneratorFormatter.GetQueryableEventFormat(constructorParameters, sourceParameter, sourceName, dismissQueryableEventName);
		}
		protected string DismissQueryableEventFormat() {
			return DataSourceGeneratorFormatter.DismissQueryableEventFormat();
		}
		protected string GetSourceEventFormat(string constructorParameters, string sourceParameter, string sourceName) {
			return DataSourceGeneratorFormatter.GetSourceEventFormat(constructorParameters, sourceParameter, sourceName);
		}
		protected string DismissSourceEventFormat() {
			return DataSourceGeneratorFormatter.DismissSourceEventFormat();
		}
		protected string GetEnumerableEventFormat() {
			return DataSourceGeneratorFormatter.GetEnumerableEventFormat();
		}
		protected string DismissEnumerableEventFormat() {
			return DataSourceGeneratorFormatter.DismissEnumerableEventFormat();
		}
		protected string ResolveSessionEventFormat() {
			return DataSourceGeneratorFormatter.ResolveSessionEventFormat();
		}
		protected string DismissSessionEventFormat() {
			return DataSourceGeneratorFormatter.DismissSessionEventFormat();
		}
		protected string GetServiceUri(IDataSourceGeneratorContext context) {
			return ((IServiceSettingsModel)context.SettingsModel).ServiceRoot;
		}
		protected static void SetProperty(IModelItem dataSourceItem, DataSourcePropertyCodeName property, object value) {
			string propertyName = property.ToString();
			SetProperty(dataSourceItem, propertyName, value);
		}
		protected static void SetProperty(IModelItem dataSourceItem, string propertyName, object value) {
			var property = dataSourceItem.Properties[propertyName];
			if(property != null) property.SetValue(value);
		}
		protected static void ClearProperty(IModelItem dataSourceItem, string propertyName) {
			var property = dataSourceItem.Properties[propertyName];
			if(property != null) property.ClearValue();
		}
	}
}
