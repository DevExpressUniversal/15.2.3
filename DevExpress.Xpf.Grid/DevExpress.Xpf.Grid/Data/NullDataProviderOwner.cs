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
	public class NullDataProviderOwner : IDataProviderOwner {
		List<IColumnInfo> IDataProviderOwner.GetColumns() { return new List<IColumnInfo>(); }
		IEnumerable<IColumnInfo> IDataProviderOwner.GetServiceUnboundColumns() { yield break; }
		IEnumerable<DevExpress.Xpf.Grid.SummaryItemBase> IDataProviderOwner.GetServiceSummaries() { yield break; }
		bool IDataProviderOwner.IsDesignTime { get { return false; } }
		void IDataProviderOwner.OnCurrentIndexChanged() { }
		void IDataProviderOwner.OnCurrentIndexChanging(int newControllerRow) { }
		void IDataProviderOwner.OnCurrentRowChanged() { }
		void IDataProviderOwner.OnItemChanged(ListChangedEventArgs e) { }
		bool IDataProviderOwner.HasCustomRowFilter() { return false; }
		bool IDataProviderOwner.RequireSortCell(DataColumnInfo column) { return false; }
		bool IDataProviderOwner.RequireDisplayText(DataColumnInfo column) { return false; }
		string IDataProviderOwner.GetDisplayText(int listSourceIndex, DataColumnInfo column, object value, string columnName) { return string.Empty; }
		void IDataProviderOwner.OnListSourceChanged() { }
		void IDataProviderOwner.RaiseCurrentRowUpdated(ControllerRowEventArgs e) { }
		void IDataProviderOwner.RaiseCurrentRowCanceled(ControllerRowEventArgs e) { }
		void IDataProviderOwner.RaiseValidatingCurrentRow(ValidateControllerRowEventArgs e) { }
		void IDataProviderOwner.OnPostRowException(ControllerRowExceptionEventArgs e) { }
		void IDataProviderOwner.OnStartNewItemRow() { }
		void IDataProviderOwner.OnEndNewItemRow() { }
		void IDataProviderOwner.SynchronizeGroupSortInfo(IList<IColumnInfo> sortList, int groupCount) { }
		void IDataProviderOwner.OnSelectionChanged(SelectionChangedEventArgs e) { }
		bool IDataProviderOwner.CanSortColumn(string fieldName) { return false; }
		void IDataProviderOwner.PopulateColumns() { }
		void IDataProviderOwner.RePopulateDataControllerColumns() { }
		void IDataProviderOwner.UpdateIsAsyncOperationInProgress(bool value) { }
		ColumnGroupInterval IDataProviderOwner.GetGroupInterval(string fieldName) { return ColumnGroupInterval.Default; }
		Dispatcher IDataProviderOwner.Dispatcher { get { return null; } }
		GridSummaryItemCollection IDataProviderOwner.TotalSummary { get { return new GridSummaryItemCollection(null, SummaryItemCollectionType.Total); } }
		GridSummaryItemCollection IDataProviderOwner.GroupSummary { get { return new GridSummaryItemCollection(null, SummaryItemCollectionType.Group); } }
		bool? IDataProviderOwner.AllowLiveDataShaping { get { return false; } }
		bool IDataProviderOwner.OptimizeSummaryCalculation { get { return false; } }
		void IDataProviderOwner.ValidateMasterDetailConsistency() { }
		string[] IDataProviderOwner.GetFindToColumnNames() {
			return null;
		}
		NewItemRowPosition IDataProviderOwner.NewItemRowPosition { get { return NewItemRowPosition.None; } }
		bool IDataProviderOwner.ShowGroupSummaryFooter { get { return false; } }
		Type IDataProviderOwner.ItemType { get { return null; } }
		bool IDataProviderOwner.IsSynchronizedWithCurrentItem { get { return false; } }
	}
}
