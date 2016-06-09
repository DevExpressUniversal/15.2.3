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
	abstract class BaseDirectBindingGenerator : DataSourceGeneratorBase {
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.ClearDataSource();
			context.ClearDataMember();
			if(!string.IsNullOrEmpty(context.DesignTimeElementTypeProperty)) {
				System.Type designTimeElementType = context.SettingsModel.SelectedElementIsDataTable ?
					GetTableRowType(context) : GetElementType(context);
				context.SetCustomBindingProperty(context.DesignTimeElementTypeProperty, designTimeElementType);
			}
		}
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.GenerateDataSourceAssignment((IModelItemExpression)dataSourceItem);
			return true;
		}
	}
	class TypedDataSetDirectBindingGenerator : BaseDirectBindingGenerator {
		IModelItemExpression fillExpression;
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			fillExpression = GenerateAdapterFillExpression(dataSourceItem, context);
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.SetDataSource(dataSourceItem);
			context.SetDataMember();
		}
		protected sealed override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			if(fillExpression != null) 
				context.GenerateCode(fillExpression);
			return true;
		}
	}
	class SQLDataSourceDirectBindingGenerator : BaseDirectBindingGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return GetComponentModelItem(context);
		}
		IModelItemExpression fillExpression;
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			fillExpression = GenerateSqlDataSourceFillExpression(dataSourceItem, context);
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.SetDataSource(dataSourceItem);
			context.SetDataMember();
		}
		protected sealed override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			if(fillExpression != null)
				context.GenerateCode(fillExpression);
			return true;
		}
	}
	class ExcelDataSourceDirectBindingGenerator : BaseDirectBindingGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return GetComponentModelItem(context);
		}
		IModelItemExpression fillExpression;
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			fillExpression = GenerateExcelDataSourceFillExpression(dataSourceItem, context);
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.SetDataSource(dataSourceItem);
			context.ClearDataMember();
		}
		protected sealed override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			if(fillExpression != null)
				context.GenerateCode(fillExpression);
			return true;
		}
	}
	class EntityFrameworkDirectBindingGenerator : BaseDirectBindingGenerator {
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			if(IsEntityFrameworkDbContextBinding(context)) {
				context.GenerateUsing("System.Data.Entity");
				context.GenerateCode(GenerateDBContextLoadExpression(context));
			}
			return base.GenerateCodeBehind(dataSourceItem, context);
		}
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			if(IsEntityFrameworkDbContextBinding(context)) 
				return GenerateDbContextBindingExpression(context);
			return context.GenerateDataSourceBindingExpression(
				new object[] { GetElementName(context) }, "{0}.{1}");
		}
	}
	class LinqToSqlDirectBindingGenerator : BaseDirectBindingGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(
				new object[] { GetElementName(context) }, "{0}.{1}");
		}
	}
	class WcfDirectBindingGenerator : BaseDirectBindingGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(
				new object[] { GetServiceUri(context), GetElementName(context) }, "{0}(new System.Uri(\"{1}\")).{2}.ToList()");
		}
	}
	class IEnumerableDirectBindingGenerator : BaseDirectBindingGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.GenerateBindingExpression(typeof(System.Collections.Generic.List<>),
				new object[] { GetElementType(context) });
		}
	}
	class EnumDirectBindingGenerator : BaseDirectBindingGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.GenerateBindingExpression(typeof(System.Collections.Generic.List<>),
				new object[] { GetElementType(context) });
		}
	}
}
