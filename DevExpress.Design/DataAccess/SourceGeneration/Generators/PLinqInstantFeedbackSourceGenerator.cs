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
	abstract class BasePLinqInstantFeedbackSourceGenerator : DataSourceGeneratorBase {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource(typeof(DevExpress.Data.PLinq.PLinqInstantFeedbackSource));
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			IPLinqInstantFeedbackSettingsModel instantFeedbackSettingsModel = context.SettingsModel as IPLinqInstantFeedbackSettingsModel;
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.DefaultSorting, instantFeedbackSettingsModel.DefaultSorting);
			SetProperty(dataSourceItem, "DesignTimeElementType", GetDesignTimeElementType(context));
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.ClearDataMember();
			context.SetDataSource(dataSourceItem);
		}
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			IModelItemExpression getEnumerableEventExpression = GenerateGetEnumerableEventExpression(context);
			context.GenerateEvent(dataSourceItem, "GetEnumerable", GetEnumerableEventArgsType(), getEnumerableEventExpression);
			IModelItemExpression dismissEnumerableEventExpression = GenerateDismissEnumerableEventExpression(context);
			context.GenerateEvent(dataSourceItem, "DismissEnumerable", GetEnumerableEventArgsType(), dismissEnumerableEventExpression);
			return true;
		}
		protected virtual System.Type GetEnumerableEventArgsType() {
			return typeof(DevExpress.Data.PLinq.GetEnumerableEventArgs);
		}
		protected virtual IModelItemExpression GenerateGetEnumerableEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { GetSourceType(context), GetElementName(context) },
				GetQueryableEventFormat(null, "Source", "PLinqInstantFeedbackSource", "DismissEnumerable"));
		}
		protected virtual IModelItemExpression GenerateDismissEnumerableEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { GetSourceType(context) }, DismissQueryableEventFormat());
		}
		protected abstract System.Type GetDesignTimeElementType(IDataSourceGeneratorContext context);
	}
	class EntityFrameworkPLinqInstantFeedbackSourceGenerator : BasePLinqInstantFeedbackSourceGenerator {
		protected override System.Type GetDesignTimeElementType(IDataSourceGeneratorContext context) {
			return GetTableRowType(context);
		}
	}
	class LinqToSqlPLinqInstantFeedbackSourceGenerator : BasePLinqInstantFeedbackSourceGenerator {
		protected override System.Type GetDesignTimeElementType(IDataSourceGeneratorContext context) {
			return GetTableRowType(context);
		}
	}
	class IEnumetablePLinqInstantFeedbackSourceGenerator : BasePLinqInstantFeedbackSourceGenerator {
		protected override System.Type GetDesignTimeElementType(IDataSourceGeneratorContext context) {
			return GetElementType(context);
		}
		protected override IModelItemExpression GenerateGetEnumerableEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { GetElementType(context) }, GetEnumerableEventFormat());
		}
		protected override IModelItemExpression GenerateDismissEnumerableEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { }, DismissEnumerableEventFormat());
		}
	}
}
