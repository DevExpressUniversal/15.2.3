#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using System.Collections.Generic;
using DevExpress.DashboardWin.Native;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.DataAccess.Wizard.Presenters;
namespace DevExpress.DashboardCommon.DataSourceWizard {
	public class DashboardChooseDataSourceNamePage<TModel> : ChooseDataSourceNamePage<TModel> where TModel : IDataSourceModel {
		readonly bool singleDataSourceType;
		DataSourceType dataSourceType;
		protected IEnumerable<SqlDataConnection> DataConnections { get; set; }
		protected DataSourceTypes DataSourceTypes { get; set; }
		public DashboardChooseDataSourceNamePage(IChooseDataSourceNamePageView view, IDataSourceNameCreationService dataSourceNameCreator, IEnumerable<SqlDataConnection> dataConnections, DataSourceTypes dataSourceTypes)
			: base(view, dataSourceNameCreator) {
			DataConnections = dataConnections;
			DataSourceTypes = dataSourceTypes;
			singleDataSourceType = DataSourceTypes.Count == 1;
		}
		public override void Commit() {
			base.Commit();
			if(singleDataSourceType) {
				Model.DataSourceType = DataSourceType;
			}
		}
		public override Type GetNextPageType() {
			if(singleDataSourceType) {
				return DashboardDataSourceTypeHelper.GetNextPageType<TModel>(DataSourceType, DataConnections.Any());
			}
			return typeof(DashboardChooseDataSourceTypePage<TModel>);
		}
		DataSourceType DataSourceType {
			get { return dataSourceType ?? (dataSourceType = DataSourceTypes.First()); }
		}
	}
}
