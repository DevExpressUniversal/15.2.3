﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
	abstract class BasePLinqServerModeSourceGenerator : DataSourceGeneratorBase {
		protected sealed override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource(typeof(DevExpress.Data.PLinq.PLinqServerModeSource));
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			IPLinqServerModeSettingsModel serverModeSettingsModel = context.SettingsModel as IPLinqServerModeSettingsModel;
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.DefaultSorting, serverModeSettingsModel.DefaultSorting);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.ElementType, GetRuntimeElementType(context));
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.ClearDataMember();
			context.SetDataSource(dataSourceItem);
		}
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			IModelItemExpression sourceExpression = GenerateSourceBindingExpression(context);
			context.GenerateParameterAssignment(dataSourceItem, "Source", sourceExpression);
			return true;
		}
		protected abstract IModelItemExpression GenerateSourceBindingExpression(IDataSourceGeneratorContext context);
		protected abstract System.Type GetRuntimeElementType(IDataSourceGeneratorContext context);
	}
	class EntityFrameworkPLinqServerModeSourceGenerator : BasePLinqServerModeSourceGenerator {
		protected override IModelItemExpression GenerateSourceBindingExpression(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(
				new object[] { GetElementName(context) }, "{0}.{1}");
		}
		protected override System.Type GetRuntimeElementType(IDataSourceGeneratorContext context) {
			return GetTableRowType(context);
		}
	}
	class LinqToSqlPLinqServerModeSourceGenerator : BasePLinqServerModeSourceGenerator {
		protected override IModelItemExpression GenerateSourceBindingExpression(IDataSourceGeneratorContext context) {
			return context.GenerateDataSourceBindingExpression(
				new object[] { GetElementName(context) }, "{0}.{1}");
		}
		protected override System.Type GetRuntimeElementType(IDataSourceGeneratorContext context) {
			return GetTableRowType(context);
		}
	}
	class IEnumetablePLinqServerModeSourceGenerator : BasePLinqServerModeSourceGenerator {
		protected override IModelItemExpression GenerateSourceBindingExpression(IDataSourceGeneratorContext context) {
			return context.GenerateBindingExpression(typeof(System.Collections.Generic.IEnumerable<>),
				new object[] { GetElementType(context) });
		}
		protected override System.Type GetRuntimeElementType(IDataSourceGeneratorContext context) {
			return GetElementType(context);
		}
	}
}
