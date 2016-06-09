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
using DevExpress.XtraExport.Helpers;
namespace DevExpress.Xpf.Grid.Printing {
	public abstract class RowBaseWrapper : IRowBase {
		int rowHandle;
		int rowLevel;
		int dataSourceRowIndexCore;
		public int LogicalPosition {
			get { return rowHandle; }
		}
		public abstract bool IsGroupRow { get; }
		public RowBaseWrapper(int rowHandle, int rowLevel, int dataSourceRowIndex) {
			this.rowHandle = rowHandle;
			this.rowLevel = rowLevel;
			dataSourceRowIndexCore = dataSourceRowIndex;
		}
		public FormatSettings FormatSettings { get { return null; } }
		public int DataSourceRowIndex { get { return dataSourceRowIndexCore; } }
		public int GetRowLevel() {
			return rowLevel;
		}
		public string FormatString { get { return null; } }
		public bool IsDataAreaRow { get { return true; } }
	}
	public class DataRowWrapper : RowBaseWrapper {
		public override bool IsGroupRow {
			get { return false; }
		}
#if DEBUGTEST
		public static int dataRowWrappersCount = 0;
#endif
		public DataRowWrapper(int rowHandle, int rowLevel, int dataSourceRowIndex)
			: base(rowHandle, rowLevel, dataSourceRowIndex) {
#if DEBUGTEST
				dataRowWrappersCount++;
#endif
		}
	}
	public class GroupRowWrapper : RowBaseWrapper, IGroupRow<RowBaseWrapper>, IClipboardGroupRow<RowBaseWrapper> {
		string groupValue;
		bool isExpanded;
		IEnumerable<RowBaseWrapper> childRows;
		TableView view;
		public bool IsCollapsed {
			get { return !isExpanded; }
		}
		public override bool IsGroupRow {
			get { return true; }
		}
#if DEBUGTEST
		public static int groupRowWrappersCount = 0;
#endif
		public GroupRowWrapper(int rowHandle, int rowLevel, int dataSourceRowIndex, string groupValue, bool isExpanded, IEnumerable<RowBaseWrapper> childRows, TableView view)
			: base(rowHandle, rowLevel, dataSourceRowIndex) {
#if DEBUGTEST
				groupRowWrappersCount++;
#endif
				this.groupValue = groupValue;
				this.isExpanded = isExpanded;
				this.childRows = childRows;
				this.view = view;
		}
		public string GetGroupRowHeader() {
			return groupValue;
		}
		public IEnumerable<RowBaseWrapper> GetAllRows() {
			return childRows;
		}
		#region IClipboardGroupRow<RowBaseWrapper> Members
		public IEnumerable<RowBaseWrapper> GetSelectedRows() {
			return new List<RowBaseWrapper>();		   
		}
		public bool IsTreeListGroupRow() {
			return false;
		}
		#endregion
	}
}
