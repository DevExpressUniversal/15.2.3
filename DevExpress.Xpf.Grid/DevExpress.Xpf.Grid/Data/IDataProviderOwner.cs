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
using System.Windows.Threading;
using DevExpress.Data;
using DevExpress.Xpf.Grid;
using DevExpress.XtraGrid;
using DevExpress.Xpf.GridData;
using System.ComponentModel;
namespace DevExpress.Xpf.Data {
	public interface IDataProviderOwner {
		List<IColumnInfo> GetColumns();
		IEnumerable<IColumnInfo> GetServiceUnboundColumns();
		IEnumerable<DevExpress.Xpf.Grid.SummaryItemBase> GetServiceSummaries();
		bool IsDesignTime { get; }
		void OnCurrentIndexChanged();
		void OnCurrentIndexChanging(int newControllerRow);
		void OnCurrentRowChanged();
		void OnItemChanged(ListChangedEventArgs e);
		bool RequireSortCell(DataColumnInfo column);
		bool RequireDisplayText(DataColumnInfo column);
		string GetDisplayText(int listSourceIndex, DataColumnInfo column, object value, string columnName);
		bool HasCustomRowFilter();
		void OnListSourceChanged();
		void RaiseCurrentRowUpdated(ControllerRowEventArgs e);
		void RaiseCurrentRowCanceled(ControllerRowEventArgs e);
		void RaiseValidatingCurrentRow(ValidateControllerRowEventArgs e);
		void OnPostRowException(ControllerRowExceptionEventArgs e);
		void OnStartNewItemRow();
		void OnEndNewItemRow();
		void SynchronizeGroupSortInfo(IList<IColumnInfo> sortList, int groupCount);
		void OnSelectionChanged(SelectionChangedEventArgs e);
		bool CanSortColumn(string fieldName);
		void PopulateColumns();
		void RePopulateDataControllerColumns();
		void UpdateIsAsyncOperationInProgress(bool value);
		ColumnGroupInterval GetGroupInterval(string fieldName);
		Dispatcher Dispatcher { get; }
		GridSummaryItemCollection TotalSummary { get; }
		GridSummaryItemCollection GroupSummary { get; }
		bool? AllowLiveDataShaping { get; }
		bool OptimizeSummaryCalculation { get; }
		void ValidateMasterDetailConsistency();
		string[] GetFindToColumnNames();
		NewItemRowPosition NewItemRowPosition { get; }
		bool ShowGroupSummaryFooter { get; }
		Type ItemType { get; }
		bool IsSynchronizedWithCurrentItem { get; }
	}
}
