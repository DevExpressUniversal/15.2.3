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
using System.Linq;
namespace DevExpress.PivotGrid.Internal.ThinClientDataSource {
	public abstract class ExpandRequestedEventArgsBase : EventArgs {
		readonly bool isColumn;
		readonly bool isDataRequired;
		readonly IList<ThinClientFieldValueItem> expandHierarchy = new List<ThinClientFieldValueItem>();
		readonly IDictionary<int, Dictionary<PairReferenceKey<ThinClientFieldValueItem, object[]>, ThinClientValueItem>> data = new Dictionary<int, Dictionary<PairReferenceKey<ThinClientFieldValueItem, object[]>, ThinClientValueItem>>();
		public IList<ThinClientFieldValueItem> ExpandHierarchy { get { return expandHierarchy; } }
		public IDictionary<int, Dictionary<PairReferenceKey<ThinClientFieldValueItem, object[]>, ThinClientValueItem>> Data { get { return data; } }
		public bool IsColumn { get { return isColumn; } }
		public bool IsDataRequired { get { return isDataRequired; } }
		public ExpandRequestedEventArgsBase(bool isColumn, bool isDataRequired) {
			this.isColumn = isColumn;
			this.isDataRequired = isDataRequired;
		}
		public void AddExpandHierarchyItem(ThinClientFieldValueItem cellItem) {
			if(isDataRequired)
				ExpandHierarchy.Add(cellItem);
		}
		public void AddDataItem(ThinClientFieldValueItem expandItem, object[] values, int dataItemIndex, ThinClientValueItem dataItem) {		   
			if(!isDataRequired)
				return;
			int key = dataItemIndex;
			PairReferenceKey<ThinClientFieldValueItem, object[]> cellKey = new PairReferenceKey<ThinClientFieldValueItem, object[]>(expandItem, values);
			if(!data.ContainsKey(key))
				data[key] = new Dictionary<PairReferenceKey<ThinClientFieldValueItem, object[]>, ThinClientValueItem>();
			data[key][cellKey] = dataItem;
		}
	}
	public class ExpandValueRequestedEventArgs : ExpandRequestedEventArgsBase {
		readonly object[] values;
		public object[] Values { get { return values; } }
		public bool Result { get; set; }
		public ExpandValueRequestedEventArgs(bool isColumn, object[] values, bool isDataRequired)
			: base(isColumn, isDataRequired) {
			this.values = values;
			Result = true;
		}
	}
	public class ExpandLevelRequestedEventArgs : ExpandRequestedEventArgsBase {
		readonly int level;
		public int Level { get { return level; } }
		public ExpandLevelRequestedEventArgs(bool isColumn, int level, bool isDataRequired)
			: base(isColumn, isDataRequired) {
			this.level = level;
		}
	}
	public abstract class CollapseRequestedEventArgsBase : EventArgs {
		readonly bool isColumn;
		public bool IsColumn { get { return isColumn; } }
		public CollapseRequestedEventArgsBase(bool isColumn) {
			this.isColumn = isColumn;
		}
	}
	public class CollapseValueRequestedEventArgs : CollapseRequestedEventArgsBase {
		readonly object[] values;
		public object[] Values { get { return values; } }
		public CollapseValueRequestedEventArgs(bool isColumn, object[] values)
			: base(isColumn) {
			this.values = values;
		}
	}
	public class CollapseLevelRequestedEventArgs : CollapseRequestedEventArgsBase {
		readonly int level;
		public int Level { get { return level; } }
		public CollapseLevelRequestedEventArgs(bool isColumn, int level)
			: base(isColumn) {
			this.level = level;
		}
	}
}
