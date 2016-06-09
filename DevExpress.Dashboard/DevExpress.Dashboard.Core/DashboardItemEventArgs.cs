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
using DevExpress.DataAccess;
using System;
namespace DevExpress.DashboardCommon.Native {
	public abstract class DashboardItemEventArgsBase<TInnerArgs> : EventArgs where TInnerArgs : EventArgs {
		public DashboardItem DashboardItem { get; private set; }
		public TInnerArgs InnerArgs { get; private set; }
		protected DashboardItemEventArgsBase(DashboardItem dashboardItem, TInnerArgs innerArgs) {
			DashboardItem = dashboardItem;
			InnerArgs = innerArgs;
		}
	}
	public class DashboardItemEventArgs : DashboardItemEventArgsBase<EventArgs> {
		public DashboardItemEventArgs(DashboardItem dashboardItem)
			: base(dashboardItem, EventArgs.Empty) {
		}
	}
	public class DashboardItemChangedEventArgs : DashboardItemEventArgsBase<ChangedEventArgs> {
		public DashboardItemChangedEventArgs(DashboardItem dashboardItem, ChangedEventArgs innerArgs)
			: base(dashboardItem, innerArgs) {
		}
	}
	public class ComponentNameChangedEventArgs : EventArgs {
		public string OldComponentName { get; private set; }
		public string NewComponentName { get; private set; }
		public ComponentNameChangedEventArgs(string oldComponentName, string newComponentName) {
			OldComponentName = oldComponentName;
			NewComponentName = newComponentName;
		}
	}
	public class EnableAutomaticUpdatesEventArgs : EventArgs {
		public bool EnableAutomaticUpdates{ get; private set; }
		public EnableAutomaticUpdatesEventArgs(bool enableAutomaticUpdates) {
			EnableAutomaticUpdates = enableAutomaticUpdates;
		}
	}
	public class DataSourceChangedEventArgs : EventArgs {
		public IDashboardDataSource DataSource { get; private set; }
		public DataSourceChangedEventArgs(IDashboardDataSource dataSource) {
			DataSource = dataSource;
		}
	}
	public class DataSourceCalcFieldCollectionChangedEventArgs : DataSourceChangedEventArgs {
		public NotifyingCollectionChangedEventArgs<CalculatedField> InnerArgs { get; private set; }
		public DataSourceCalcFieldCollectionChangedEventArgs(IDashboardDataSource dataSource, NotifyingCollectionChangedEventArgs<CalculatedField> innerArgs)
			: base(dataSource) { 
			InnerArgs = innerArgs;
		}
	}
	public class DataSourceCalculatedFieldChangedEventArgs : DataSourceChangedEventArgs { 
		public CalculatedFieldChangedEventArgs InnerArgs { get; private set; }
		public DataSourceCalculatedFieldChangedEventArgs(IDashboardDataSource dataSource, CalculatedFieldChangedEventArgs innerArgs)
			: base(dataSource) { 
			InnerArgs = innerArgs;
		}
	}
	public class RequestMasterFilterParametersEventArgs : EventArgs {
		public IMasterFilterParameters Parameters { get; set; }
		public string DashboardItemName { get; private set; }
		public RequestMasterFilterParametersEventArgs(string dashboardItemName) {
			DashboardItemName = dashboardItemName;
		}
	}
	public class RequestDrillDownParametersEventArgs : EventArgs {
		public IDrillDownParameters Parameters { get; set; }
		public string DashboardItemName { get; private set; }
		public RequestDrillDownParametersEventArgs(string dashboardItemName) {
			DashboardItemName = dashboardItemName;
		}
	}
	public class RequestDrillDownControllerEventArgs : EventArgs {
		public IDrillDownController Controller { get; set; }
		public string DashboardItemName { get; private set; }
		public RequestDrillDownControllerEventArgs(string dashboardItemName) {
			DashboardItemName = dashboardItemName;
		}
	}
	public class RangeFilterRangeChangedEventArgs : EventArgs {
		public string ComponentName { get; private set; }
		public object MinValue { get; private set; }
		public object MaxValue { get; private set; }
		public RangeFilterRangeChangedEventArgs(string componentName, object minValue, object maxValue) {
			ComponentName = componentName;
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}
