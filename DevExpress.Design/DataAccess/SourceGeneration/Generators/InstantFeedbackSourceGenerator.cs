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
	abstract class BaseInstantFeedbackSourceGenerator : DataSourceGeneratorBase {
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			IInstantFeedbackSettingsModel instantFeedbackSettingsModel = context.SettingsModel as IInstantFeedbackSettingsModel;
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.KeyExpression, instantFeedbackSettingsModel.KeyExpression);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.DefaultSorting, instantFeedbackSettingsModel.DefaultSorting);
			SetProperty(dataSourceItem, "DesignTimeElementType", GetTableRowType(context));
		}
		protected override void SetDataSource(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			context.ClearDataMember();
			context.SetDataSource(dataSourceItem);
		}
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			IModelItemExpression getQueryableEventExpression = GenerateGetQueryableEventExpression(context);
			context.GenerateEvent(dataSourceItem, GetQueryableEventName, GetQueryableEventArgsType(), getQueryableEventExpression);
			IModelItemExpression dismissQueryableEventExpression = GenerateDismissQueryableEventExpression(context);
			context.GenerateEvent(dataSourceItem, DismissQueryableEventName, GetQueryableEventArgsType(), dismissQueryableEventExpression);
			return true;
		}
		protected abstract System.Type GetQueryableEventArgsType();
		protected abstract IModelItemExpression GenerateGetQueryableEventExpression(IDataSourceGeneratorContext context);
		protected abstract IModelItemExpression GenerateDismissQueryableEventExpression(IDataSourceGeneratorContext context);
		protected virtual string GetQueryableEventName { get { return "GetQueryable"; } }
		protected virtual string DismissQueryableEventName { get { return "DismissQueryable"; } }
	}
	class EntityFrameworkInstantFeedbackSourceGenerator : BaseInstantFeedbackSourceGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource(typeof(DevExpress.Data.Linq.EntityInstantFeedbackSource));
		}
		protected override System.Type GetQueryableEventArgsType() {
			return typeof(DevExpress.Data.Linq.GetQueryableEventArgs);
		}
		protected override IModelItemExpression GenerateGetQueryableEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { GetSourceType(context), GetElementName(context) },
				GetQueryableEventFormat(null, "QueryableSource", "EntityInstantFeedbackSource"));
		}
		protected override IModelItemExpression GenerateDismissQueryableEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { GetSourceType(context) }, DismissQueryableEventFormat());
		}
	}
	class LinqToSqlInstantFeedbackSourceGenerator : BaseInstantFeedbackSourceGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource(typeof(DevExpress.Data.Linq.LinqInstantFeedbackSource));
		}
		protected override System.Type GetQueryableEventArgsType() {
			return typeof(DevExpress.Data.Linq.GetQueryableEventArgs);
		}
		protected override IModelItemExpression GenerateGetQueryableEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { GetSourceType(context), GetElementName(context) },
				GetQueryableEventFormat(null, "QueryableSource", "LinqInstantFeedbackSource"));
		}
		protected override IModelItemExpression GenerateDismissQueryableEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { GetSourceType(context) }, DismissQueryableEventFormat());
		}
	}
	class WcfInstantFeedbackSourceGenerator : BaseInstantFeedbackSourceGenerator {
		protected override IModelItem CreateDataSourceItem(IDataSourceGeneratorContext context) {
			return context.CreateDataSource(typeof(DevExpress.Data.WcfLinq.WcfInstantFeedbackSource));
		}
		protected override System.Type GetQueryableEventArgsType() {
			return typeof(DevExpress.Data.WcfLinq.GetSourceEventArgs);
		}
		protected override string GetQueryableEventName { get { return "GetSource"; } }
		protected override string DismissQueryableEventName { get { return "DismissSource"; } }
		protected override IModelItemExpression GenerateGetQueryableEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { GetSourceType(context), GetElementName(context), GetServiceUri(context) },
				GetSourceEventFormat("new System.Uri(\"{2}\")", "Query", "WcfInstantFeedbackSource"));
		}
		protected override IModelItemExpression GenerateDismissQueryableEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { GetSourceType(context) }, DismissSourceEventFormat());
		}
	}
	class XPOInstantFeedbackSourceGenerator : XPObjectSourceGenerator {
		protected override string GetDataSourceObjectTypeName() {
			return "XPInstantFeedbackSource";
		}
		protected override void InitializeDataSourceItem(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDataSourceItem(xpoSourceItem, context);
			var settingsModel = context.SettingsModel as IXPInstantFeedbackSourceSettingsModel;
			SetProperty(xpoSourceItem, DataSourcePropertyCodeName.DefaultSorting, settingsModel.DefaultSorting);
		}
		protected override bool GenerateCodeBehind(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			var eventArgsType = XPODataSourceHelper.GetXPOType("ResolveSessionEventArgs");
			context.GenerateUsing(eventArgsType.Namespace);
			var resolveSessionEventExpression = GenerateResolveSessionEventExpression(context);
			context.GenerateEvent(dataSourceItem, "ResolveSession", eventArgsType, resolveSessionEventExpression);
			var dismissSessionEventExpression = GenerateDismissSessionEventExpression(context);
			context.GenerateEvent(dataSourceItem, "DismissSession", eventArgsType, dismissSessionEventExpression);
			return true;
		}
		IModelItemExpression GenerateResolveSessionEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { XPODataSourceHelper.GetXPOSessionType() }, ResolveSessionEventFormat());
		}
		IModelItemExpression GenerateDismissSessionEventExpression(IDataSourceGeneratorContext context) {
			return context.GenerateExpression(new object[] { }, DismissSessionEventFormat());
		}
	}
}
