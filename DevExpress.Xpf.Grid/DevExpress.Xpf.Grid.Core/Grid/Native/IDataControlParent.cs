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
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Grid.Native {
	public interface IDataControlParent {
		IEnumerable<ColumnsRowDataBase> GetColumnsRowDataEnumerator();
		ColumnsRowDataBase GetNewItemRowData();
		ColumnsRowDataBase GetHeadersRowData();
		DataViewBase FindMasterView();
		bool FindMasterRow(out DataViewBase targetView, out int targetVisibleIndex);
		bool FindNextOuterMasterRow(out DataViewBase targetView, out int targetVisibleIndex);
		void InvalidateTree();
		void EnumerateParentDataControls(Action<DataControlBase, int> action);
		void ValidateMasterDetailConsistency(DataControlBase dataControl);
		void CollectViewVisibleIndexChain(List<KeyValuePair<DataViewBase, int>> chain);
		void CollectParentFixedRowsScrollIndexes(List<int> scrollIndexes);
		int CalcTotalLevel();
	}
	public sealed class NullDataControlParent : IDataControlParent {
		ColumnsRowDataBase[] Empty = new ColumnsRowDataBase[0];
		public static readonly IDataControlParent Instance = new NullDataControlParent();
		NullDataControlParent() { }
		IEnumerable<ColumnsRowDataBase> IDataControlParent.GetColumnsRowDataEnumerator() {
			return Empty;
		}
		ColumnsRowDataBase IDataControlParent.GetNewItemRowData() {
			return null;
		}
		ColumnsRowDataBase IDataControlParent.GetHeadersRowData() {
			return null;
		}
		DataViewBase IDataControlParent.FindMasterView() {
			return null;
		}
		bool IDataControlParent.FindMasterRow(out DataViewBase targetView, out int targetVisibleIndex) {
			targetView = null;
			targetVisibleIndex = -1;
			return false;
		}
		bool IDataControlParent.FindNextOuterMasterRow(out DataViewBase targetView, out int targetVisibleIndex) {
			targetView = null;
			targetVisibleIndex = -1;
			return false;
		}
		void IDataControlParent.InvalidateTree() { }
		void IDataControlParent.EnumerateParentDataControls(Action<DataControlBase, int> action) { }
		void IDataControlParent.ValidateMasterDetailConsistency(DataControlBase dataControl) { }
		void IDataControlParent.CollectViewVisibleIndexChain(List<KeyValuePair<DataViewBase, int>> chain) { }
		void IDataControlParent.CollectParentFixedRowsScrollIndexes(List<int> scrollIndexes) { }
		int IDataControlParent.CalcTotalLevel() { return 0; }
	}
}
