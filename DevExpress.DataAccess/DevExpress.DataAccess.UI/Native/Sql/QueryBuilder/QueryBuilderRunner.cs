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

using System;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	public class QueryBuilderRunner : QueryBuilderRunnerBase {
		readonly IWin32Window owner;
		readonly UserLookAndFeel lookAndFeel;
		readonly bool noCustomSql;
		readonly bool light;
		readonly IDisplayNameProvider displayNameProvider;
		readonly IServiceProvider propertyGridServices;
		public QueryBuilderRunner(IDBSchemaProvider schemaProvider, SqlDataConnection connection, IWin32Window owner, UserLookAndFeel lookAndFeel, bool noCustomSql, bool light, IDisplayNameProvider displayNameProvider, IParameterService parameterService, IServiceProvider propertyGridServices, ICustomQueryValidator customQueryValidator)
			: base(schemaProvider, connection, parameterService, customQueryValidator) {
			this.owner = owner;
			this.lookAndFeel = lookAndFeel;
			this.noCustomSql = noCustomSql;
			this.light = light;
			this.displayNameProvider = displayNameProvider;
			this.propertyGridServices = propertyGridServices;
		}
		#region Overrides of QueryBuilderRunnerBase
		protected override IQueryBuilderView CreateView(QueryBuilderViewModel vm) {
			if(light)
				return new QueryBuilderLightView(vm, owner, lookAndFeel, displayNameProvider, parameterService, propertyGridServices);
			if(noCustomSql)
				return new QueryBuilderNoSqlView(vm, owner, lookAndFeel, parameterService, propertyGridServices);
			return new QueryBuilderView(vm, owner, lookAndFeel, parameterService, propertyGridServices);
		}
		#endregion
	}
}
