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

using DevExpress.DashboardCommon.Server;
namespace DevExpress.DashboardCommon.Service {
	public interface IDashboardServiceAdminHandlers {
		void OnSingleFilterDefaultValue(object sender, SingleFilterDefaultValueEventArgs e);
		void OnFilterElementDefaultValues(object sender, FilterElementDefaultValuesEventArgs e);
		void OnRangeFilterDefaultValue(object sender, RangeFilterDefaultValueEventArgs e);
		void OnConfigureDataConnection(object sender, ConfigureDataConnectionServerEventArgs e);
		void OnCustomFilterExpression(object sender, CustomFilterExpressionServerEventArgs e);
		void OnCustomParameters(object sender, CustomParametersServerEventArgs e);
		void OnDataLoading(object sender, DataLoadingServerEventArgs e);
		void OnDashboardLoading(object sender, DashboardLoadingServerEventArgs e);
		void OnDashboardLoaded(object sender, DashboardLoadedServerEventArgs e);
		void OnConnectionError(object sender, ConnectionErrorServerEventArgs e);
		void OnAllowLoadUnusedDataSources(object sender, AllowLoadUnusedDataSourcesServerEventArgs e);
		void OnDashboardUnloading(object sender, DashboardUnloadingEventArgs e);
		void OnRequestCustomizationServices(object sender, RequestCustomizationServicesEventArgs e);
		void OnRequestAppConfigPatcherService(object sender, RequestAppConfigPatcherServiceEventArgs e);
		void OnRequestWaitFormActivator(object sender, RequestWaitFormActivatorEventArgs e);
		void OnRequestErrorHandler(object sender, RequestErrorHandlerEventArgs e);
		void OnRequestUnderlyingDataFormat(object sender, RequestUnderlyingDataFormatEventArgs e);
		void OnRequestDataLoader(object sender, RequestDataLoaderEventArgs e);
		void OnValidateCustomSqlQuery(object sender, ValidateDashboardCustomSqlQueryEventArgs e);
	}
}
