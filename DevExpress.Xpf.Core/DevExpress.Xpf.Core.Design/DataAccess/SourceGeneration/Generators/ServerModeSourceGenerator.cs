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
	using DevExpress.Xpf.Core.Design.CoreUtils;
	using DevExpress.Xpf.Core.ServerMode;
	abstract class BaseServerModeSourceGenerator : BaseXamlDataSourceGenerator {
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			IServerModeSettingsModel serverModeSettingsModel = context.SettingsModel as IServerModeSettingsModel;
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.KeyExpression, serverModeSettingsModel.KeyExpression);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.DefaultSorting, serverModeSettingsModel.DefaultSorting);
			SetContextType(dataSourceItem, context);
			SetPath(dataSourceItem, context);
		}
	}
	class EntityFrameworkServerModeSourceGenerator : BaseServerModeSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(DevExpress.Xpf.Core.ServerMode.EntityServerModeDataSource);
		}
	}
	class LinqToSqlServerModeSourceGenerator : BaseServerModeSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(DevExpress.Xpf.Core.ServerMode.LinqServerModeDataSource);
		}
	}
	class WcfServerModeSourceGenerator : BaseServerModeSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(WcfServerModeDataSource);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDataSourceItem(dataSourceItem, context);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.ServiceRoot, GetServiceUri(context));
		}
	}
	class XPServerCollectionSourceGenerator : XPObjectSourceWithSessionGenerator {
		protected override System.Type GetDataSourceType() {
			return RuntimeTypes.XPServerCollectionDataSource.ResolveType();
		}
		protected override void InitializeDataSourceItem(IModelItem xpoSourceItem, IDataSourceGeneratorContext context) {
			base.InitializeDataSourceItem(xpoSourceItem, context);
			var settingsModel = context.SettingsModel as IXPServerCollectionSourceSettingsModel;
			SetProperty(xpoSourceItem, DataSourcePropertyCodeName.DefaultSorting, settingsModel.DefaultSorting);
		}
	}
}
