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

using System.ComponentModel;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon {
	public class DashboardUnderlyingDataSet : DashboardDataSet<DashboardUnderlyingDataRow> {
		internal DashboardUnderlyingDataSet(IDashboardDataSet dataSource)
			: base(dataSource) {
			foreach(IDashboardDataRow internalRow in dataSource) {
				Rows[internalRow.Index] = new DashboardUnderlyingDataRow(this, internalRow);
			}
		}
#if !SL
	[DevExpressDashboardCoreLocalizedDescription("DashboardUnderlyingDataSetItem")]
#endif
		public new DashboardUnderlyingDataRow this[int index] { get { return Rows[index]; } }
	}
	public class DashboardUnderlyingDataRow : DashboardDataRow {
		DashboardUnderlyingDataSet dataSet;
		internal DashboardUnderlyingDataRow(DashboardUnderlyingDataSet dataSet, IDashboardDataRow row)
			: base(null, row) {
				this.dataSet = dataSet;
		}
#if !SL
	[DevExpressDashboardCoreLocalizedDescription("DashboardUnderlyingDataRowListSourceRowIndex")]
#endif
		public int ListSourceRowIndex { get { return Row.ListSourceRowIndex; } }
#if !SL
	[DevExpressDashboardCoreLocalizedDescription("DashboardUnderlyingDataRowDataSet")]
#endif
		public new DashboardUnderlyingDataSet DataSet { get { return dataSet; } }
	}
}
