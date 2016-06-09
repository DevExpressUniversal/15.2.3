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
	abstract class BaseBindingSourceGenerator : DataSourceGeneratorBase {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource(typeof(System.Windows.Forms.BindingSource));
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.ClearDataMember();
			context.SetDataSource(dataSourceItem);
		}
	}
	class TypedDataSetBindingListViewSourceGenerator : BaseBindingSourceGenerator {
		IModelItemExpression fillExpression;
		protected override void InitializeDataSourceItem(IModelItem bindingSourceItem, IDataSourceGeneratorContext context) {
			IModelItem bindingSourceDataSourceItem = context.CreateDataSource();
			object dataSourceValue = bindingSourceDataSourceItem.Value;
			SetProperty(bindingSourceItem, "DataSource", dataSourceValue);
			try {
				string dataTableName = GetDataTableName(dataSourceValue, context);
				SetProperty(bindingSourceItem, "DataMember", dataTableName);
			}
			catch(System.ArgumentException) {
				ClearProperty(bindingSourceItem, "DataMember");
			}
			IBindingListViewSourceSettingsModel bindingListViewSettingsModel = (IBindingListViewSourceSettingsModel)context.SettingsModel;
			SetProperty(bindingSourceItem, "Sort", ComponentModel.SortStringExtension.ToSortString(bindingListViewSettingsModel.SortDescriptions));
			SetProperty(bindingSourceItem, "Filter", bindingListViewSettingsModel.Filter);
			fillExpression = GenerateAdapterFillExpression(bindingSourceDataSourceItem, context);
		}
		protected string GetDataTableName(object dataSourceValue, IDataSourceGeneratorContext context) {
			return Win.DataSourceGeneratorContextFactory.CheckDataTableName(dataSourceValue, GetElementName(context));
		}
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			if(fillExpression != null)
				context.GenerateCode(fillExpression);
			return true;
		}
	}
	class SQLDataSourceBindingListViewSourceGenerator : BaseBindingSourceGenerator {
		IModelItemExpression fillExpression;
		protected override void InitializeDataSourceItem(IModelItem bindingSourceItem, IDataSourceGeneratorContext context) {
			IModelItem bindingSourceDataSourceItem = GetComponentModelItem(context);
			SetProperty(bindingSourceItem, "DataSource", bindingSourceDataSourceItem.Value);
			try {
				SetProperty(bindingSourceItem, "DataMember", GetElementName(context));
			}
			catch(System.ArgumentException) {
				ClearProperty(bindingSourceItem, "DataMember");
			}
			fillExpression = GenerateSqlDataSourceFillExpression(bindingSourceDataSourceItem, context);
		}
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			if(fillExpression != null)
				context.GenerateCode(fillExpression);
			return true;
		}
	}
	class ExcelDataSourceBindingListViewSourceGenerator : BaseBindingSourceGenerator {
		IModelItemExpression fillExpression;
		protected override void InitializeDataSourceItem(IModelItem bindingSourceItem, IDataSourceGeneratorContext context) {
			IModelItem bindingSourceDataSourceItem = GetComponentModelItem(context);
			SetProperty(bindingSourceItem, "DataSource", bindingSourceDataSourceItem.Value);
			fillExpression = GenerateExcelDataSourceFillExpression(bindingSourceDataSourceItem, context);
		}
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			if(fillExpression != null)
				context.GenerateCode(fillExpression);
			return true;
		}
	}
	abstract class BindingSourceExpressionGenerator : BaseBindingSourceGenerator {
		protected override bool GenerateCodeBehind(IModelItem bindingSourceItem, IDataSourceGeneratorContext context) {
			IModelItemExpression dataSourceBindingExpression = GenerateDataSourceBindingExpression(context);
			context.GenerateParameterAssignment(bindingSourceItem, "DataSource", dataSourceBindingExpression);
			return true;
		}
		protected abstract IModelItemExpression GenerateDataSourceBindingExpression(IDataSourceGeneratorContext context);
		protected override void InitializeDataSourceItem(IModelItem bindingSourceItem, IDataSourceGeneratorContext context) {
			SetProperty(bindingSourceItem, "DataSource", GetTableRowType(context));
		}
	}
	class EntityFrameworkBindingListViewSourceGenerator : BindingSourceExpressionGenerator {
		protected override bool GenerateCodeBehind(IModelItem bindingSourceItem, IDataSourceGeneratorContext context) {
			if(IsEntityFrameworkDbContextBinding(context)) {
				context.GenerateUsing("System.Data.Entity");
				context.GenerateCode(GenerateDBContextLoadExpression(context));
				IModelItemExpression dbContextBindingExpression = GenerateDbContextBindingExpression(context);
				context.GenerateParameterAssignment(bindingSourceItem, "DataSource", dbContextBindingExpression);
				return true;
			}
			return base.GenerateCodeBehind(bindingSourceItem, context);
		}
		protected override IModelItemExpression GenerateDataSourceBindingExpression(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(new object[] { GetElementName(context) }, "{0}.{1}");
		}
	}
	class LinqToSqlBindingListViewSourceGenerator : BindingSourceExpressionGenerator {
		protected override IModelItemExpression GenerateDataSourceBindingExpression(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(new object[] { GetElementName(context) }, "{0}.{1}");
		}
	}
	class WcfBindingListViewSourceGenerator : BindingSourceExpressionGenerator {
		protected override IModelItemExpression GenerateDataSourceBindingExpression(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(
				new object[] { GetServiceUri(context), GetElementName(context) }, "{0}(new System.Uri(\"{1}\")).{2}");
		}
	}
}
