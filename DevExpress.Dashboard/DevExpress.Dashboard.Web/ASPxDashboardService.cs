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
using System.Collections;
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Server;
using DevExpress.DashboardCommon.Service;
using DevExpress.Web.Internal;
namespace DevExpress.DashboardWeb.Native {
	public class ASPxDashboardService : DashboardService {
		readonly static DashboardServer server = new DashboardServer();
		public ASPxDashboardService()
			: base(server, false) {
		}
		public event EventHandler<SingleFilterDefaultValueEventArgs> SingleFilterDefaultValue;
		public event EventHandler<FilterElementDefaultValuesEventArgs> FilterElementDefaultValues;
		public event EventHandler<RangeFilterDefaultValueEventArgs> RangeFilterDefaultValue;
		public event EventHandler<DashboardLoadingServerEventArgs> DashboardLoadingEvent;
		public event EventHandler<DataLoadingServerEventArgs> DataLoadingEvent;
		public event EventHandler<ConfigureDataConnectionServerEventArgs> ConfigureDataConnectionEvent;
		public event EventHandler<CustomFilterExpressionServerEventArgs> CustomFilterExpressionEvent;
		public event EventHandler<CustomParametersServerEventArgs> CustomParametersEvent;
		public event EventHandler<DashboardLoadedServerEventArgs> DashboardLoadedEvent;
		public event EventHandler<ValidateDashboardCustomSqlQueryEventArgs> ValidateCustomSqlQuery;
		protected override void OnSingleFilterDefaultValue(SingleFilterDefaultValueEventArgs e) {
			if(SingleFilterDefaultValue != null)
				SingleFilterDefaultValue(this, e);
		}
		protected override void OnFilterElementDefaultValues(FilterElementDefaultValuesEventArgs e) {
			if (FilterElementDefaultValues != null)
				FilterElementDefaultValues(this, e);
		}
		protected override void OnRangeFilterDefaultValue(RangeFilterDefaultValueEventArgs e) {
			if (RangeFilterDefaultValue != null)
				RangeFilterDefaultValue(this, e);
		}
		protected override void DashboardLoading(DashboardLoadingServerEventArgs e) {
			if(DashboardLoadingEvent != null)
				DashboardLoadingEvent(this, e);
		}
		protected override void DataLoading(DataLoadingServerEventArgs e) {
			if(DataLoadingEvent != null)
				DataLoadingEvent(this, e);
		}
		protected override void ConfigureDataConnection(ConfigureDataConnectionServerEventArgs e) {
			if(ConfigureDataConnectionEvent != null)
				ConfigureDataConnectionEvent(this, e);
		}
		protected override void CustomFilterExpression(CustomFilterExpressionServerEventArgs e) {
			if(CustomFilterExpressionEvent != null)
				CustomFilterExpressionEvent(this, e);
		}
		protected override void CustomParameters(CustomParametersServerEventArgs e) {
			if(CustomParametersEvent != null)
				CustomParametersEvent(this, e);
		}
		protected override void DashboardLoaded(DashboardLoadedServerEventArgs e) {
			if(DashboardLoadedEvent != null)
				DashboardLoadedEvent(this, e);
		}
		protected override void ConnectionError(ConnectionErrorServerEventArgs e) {
			throw e.Exception;
		}
		protected override void AllowLoadUnusedDataSources(AllowLoadUnusedDataSourcesServerEventArgs e) {
			e.Allow = false;
		}
		protected override void OnValidateCustomSqlQuery(ValidateDashboardCustomSqlQueryEventArgs e) {
			if (ValidateCustomSqlQuery != null)
				ValidateCustomSqlQuery(this, e);
		}
	}
}
