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
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Charts.Native;
namespace DevExpress.Charts.Native {
	public enum DateTimeMeasureUnitNative {
		Millisecond,
		Second,
		Minute,
		Hour,
		Day,
		Week,
		Month,
		Quarter,
		Year
	}
}
namespace DevExpress.Data.ChartDataSources {
	public enum DataChangedType { ItemsChanged, Reset };
	public class DataChangedEventArgs : EventArgs {
		readonly DataChangedType dataChangedType;
		public DataChangedType DataChangedType { get { return dataChangedType; } }
		public DataChangedEventArgs(DataChangedType dataChangedType) {
			this.dataChangedType = dataChangedType;
		}
	}
	public delegate void DataChangedEventHandler(object sender, DataChangedEventArgs e);
	public interface IChartDataSource {
		string SeriesDataMember { get; }
		string ArgumentDataMember { get; }
		string ValueDataMember { get; }
		DateTimeMeasureUnitNative? DateTimeArgumentMeasureUnit { get; }
		event DataChangedEventHandler DataChanged;
	}
	public interface IPivotGrid : IChartDataSource {
		IList<string> ArgumentColumnNames { get; }
		IList<string> ValueColumnNames { get; }
		bool RetrieveDataByColumns { get; set; }
		bool RetrieveEmptyCells { get; set; }
		DefaultBoolean RetrieveFieldValuesAsText { get; set; }
		bool SelectionSupported { get; }
		bool SelectionOnly { get; set; }
		bool SinglePageSupported { get; }
		bool SinglePageOnly { get; set; }
		bool RetrieveColumnTotals { get; set; }
		bool RetrieveColumnGrandTotals { get; set; }
		bool RetrieveColumnCustomTotals { get; set; }
		bool RetrieveRowTotals { get; set; }
		bool RetrieveRowGrandTotals { get; set; }
		bool RetrieveRowCustomTotals { get; set; }
		bool RetrieveDateTimeValuesAsMiddleValues { get; set; }
		int MaxAllowedSeriesCount { get; set; }
		int MaxAllowedPointCountInSeries { get; set; }
		int UpdateDelay { get; set; }
		int AvailableSeriesCount { get; }
		IDictionary<object, int> AvailablePointCountInSeries { get; }
		IDictionary<DateTime, DateTimeMeasureUnitNative> DateTimeMeasureUnitByArgument { get; }
		void LockListChanged();
		void UnlockListChanged();
	}
}
