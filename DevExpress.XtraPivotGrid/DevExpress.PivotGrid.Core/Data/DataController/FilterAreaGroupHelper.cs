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

using System.Collections.Generic;
using DevExpress.Data.Helpers;
using DevExpress.Data.Storage;
namespace DevExpress.Data.PivotGrid {
	public class PivotFilterAreaGroupHelper : DataControllerGroupHelperBase {
		DataColumnSortInfoCollection sortInfo;
		PivotSummaryItemCollection summaries;
		PivotColumnInfoCollection columns;
		public PivotFilterAreaGroupHelper(PivotDataController controller, PivotColumnInfo[] columns)
			: base(controller) {
			this.columns = CreateColumns(columns);
		}
		public PivotColumnInfoCollection Columns { get { return columns; } }
		protected internal override DataColumnSortInfoCollection SortInfo {
			get {
				if(this.sortInfo == null)
					this.sortInfo = new DataColumnSortInfoCollection(Controller, null);
				return sortInfo; ;
			}
		}
		protected override PivotColumnInfo[] CreateSummaryColumns() {
			return new PivotColumnInfo[0];
		}
		protected internal override PivotSummaryItemCollection Summaries {
			get {
				if(summaries == null)
					summaries = new PivotSummaryItemCollection(Controller, null);
				return summaries;
			}
		}
		protected override void ClearVisialIndexesAndGroupInfo() {
			base.ClearVisialIndexesAndGroupInfo();
			PrepareSortInfo();
		}
		protected virtual void PrepareSortInfo() {
			SortInfo.Clear();
			AddColumnsToSortInfo(Columns);
			GroupInfo.LastExpandableLevel = Columns.Count - 1;
		}
		protected override void DoSortRows() {
			if(!IsSorted) return;
			VisibleListSourceRows.ClearAndForceNonIdentity();
			EnsureColumnStorages();
			DoSortRowsCore();
		}
		protected void EnsureColumnStorages() {
			if(!Controller.IsReady) return;
			foreach(DataColumnSortInfo info in SortInfo) {
				if(info.ColumnInfo.GetStorageComparer().IsStorageEmpty)
					Controller.EnsureStorageIsCreated(info.ColumnInfo.Index);
			}
		}
		PivotColumnInfoCollection CreateColumns(PivotColumnInfo[] columns) {
			PivotColumnInfoCollection newColumns = CreateColumnsCore();
			newColumns.ClearAndAddRangeSilent(columns);
			return newColumns;
		}
		PivotColumnInfoCollection CreateColumnsCore() {
			return new PivotColumnInfoCollection(Controller, null);
		}
	}
	public class PivotFilterAreaGroupHelperCache {
		PivotDataController controller;
		List<PivotFilterAreaGroupHelper> helpers;
		public PivotFilterAreaGroupHelperCache(PivotDataController controller) {
			this.controller = controller;
		}
		protected PivotDataController Controller { get { return controller; } }
		public int Count { get { return helpers != null ? helpers.Count : 0; } }
		public PivotFilterAreaGroupHelper GetHelper(PivotColumnInfo[] columns) {
			if(this.helpers == null)
				this.helpers = new List<PivotFilterAreaGroupHelper>();
			PivotFilterAreaGroupHelper res = FindHelper(this.helpers, columns);
			if(res != null)
				return res;
			res = new PivotFilterAreaGroupHelper(Controller, columns);
			res.DoRefresh();
			this.helpers.Add(res);
			return res;
		}
		PivotFilterAreaGroupHelper FindHelper(List<PivotFilterAreaGroupHelper> list, PivotColumnInfo[] columns) {
			for(int i = 0; i < list.Count; i++) {
				if(IsColumnsEqual(list[i].Columns, columns))
					return list[i];
			}
			return null;
		}
		static bool IsColumnsEqual(PivotColumnInfoCollection collection, PivotColumnInfo[] columns) {
			if(collection.Count != columns.Length) return false;
			for(int i = 0; i < collection.Count; i++) {
				if(!object.ReferenceEquals(collection[i], columns[i]))
					return false;
			}
			return true;
		}
		public void Clear() {
			if(this.helpers != null)
				this.helpers.Clear();
		}
	}
}
