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

using DevExpress.Xpf.Core.DataSources;
using DevExpress.Xpf.Core.Design.Wizards.DataAccessTechnologies;
using System;
using System.Reflection;
namespace DevExpress.Design.DataAccess {
	class TypedDataSetSimpleBindingGenerator : BaseXamlDataSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(Xpf.Core.DataSources.TypedSimpleSource);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			SetContextType(dataSourceItem, context);
			SetPath(dataSourceItem, context);
			SetProperty(dataSourceItem, AdapterTypeProperty, GetTableAdapterType(context));
		}
	}
	class EntityFrameworkSimpleBindingGenerator : BaseXamlDataSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(Xpf.Core.DataSources.EntitySimpleDataSource);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			SetContextType(dataSourceItem, context);
			SetPath(dataSourceItem, context);
		}
	}
	class LinqToSqlSimpleBindingGenerator : BaseXamlDataSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(Xpf.Core.DataSources.LinqSimpleDataSource);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			SetContextType(dataSourceItem, context);
			SetPath(dataSourceItem, context);
		}
	}
	class WcfSimpleBindingGenerator : BaseXamlDataSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(WcfSimpleDataSource);
		}
		protected override void InitializeDataSourceItem(IModelItem dataSourceItem, IDataSourceGeneratorContext context) {
			SetContextType(dataSourceItem, context);
			SetPath(dataSourceItem, context);
			SetProperty(dataSourceItem, DataSourcePropertyCodeName.ServiceRoot, GetServiceUri(context));
		}
	}
	class IEnumerableSimpleBindingGenerator : BaseXamlDataSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(Xpf.Core.DataSources.IEnumerableDataSource);
		}
		protected override void InitializeDesignData(IModelItem designDataItem, IDataSourceGeneratorContext context) {
			SetProperty(designDataItem, DataObjectTypeProperty, GetElementType(context));
			base.InitializeDesignData(designDataItem, context);
		}
	}
	class EnumItemsSourceSimpleBindingGenerator : BaseXamlDataSourceGenerator {
		protected override System.Type GetDataSourceType() {
			return typeof(Xpf.Editors.EnumItemsSource);
		}
		protected override void InitializeDataSourceItem(IModelItem designDataItem, IDataSourceGeneratorContext context) {
			SetProperty(designDataItem, EnumTypeProperty, GetElementType(context));
			base.InitializeDataSourceItem(designDataItem, context);
		}
	}
}
