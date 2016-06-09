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
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.QueryMode.TuplesTree;
namespace DevExpress.PivotGrid.OLAP {
	class OLAPMeasuredMemberCreator : OLAPMemberCreator {
		Dictionary<string, IQueryMetadataColumn> cache;
		public OLAPMeasuredMemberCreator(List<OLAPCubeColumn> columns, List<OLAPCubeColumn> thisArea, bool thisExpand, List<OLAPCubeColumn> measures)
			: base(columns, thisArea, thisExpand) {
			cache = new Dictionary<string, IQueryMetadataColumn>();
			foreach(OLAPCubeColumn column in measures)
				cache.Add(column.UniqueName, column.Metadata);
		}
		public override QueryMember GetMember(int index) {
			if(hierachyExpand) {
				if(index == count - 2)
					return expandedParent;
				if(index >= count - 1)
					index--;
			}
			return base.GetMember(index);
		}
		public override object GetValue(int i) {
			if(hierachyExpand) {
				if(i == count - 2)
					return expandedParent.UniqueLevelValue;
				if(i >= count - 1)
					i--;
			}
			return base.GetValue(i);
		}
		public override int Length {
			get {
				return base.Length - 1;
			}
		}
		protected override int GetLastDiff() {
			return 2;
		}
	}
	class OLAPMemberCreator : MemberCreatorBase {
		readonly List<OLAPCubeColumn> columns;
		readonly Dictionary<string, OLAPCubeColumn> columns2;
		protected IOLAPTuple current;
		protected int count;
		protected bool hierachyExpand;
		protected QueryMember expandedParent;
		OLAPCubeColumn parentCol;
		IQueryMetadataColumn parent;
		IQueryMetadataColumn child;
		public OLAPMemberCreator(List<OLAPCubeColumn> columns, List<OLAPCubeColumn> thisArea, bool thisExpand) {
			if(thisExpand && thisArea.Count != 0) {
				if(columns[columns.IndexOf(thisArea[0]) - 1].IsParent(thisArea[0])) {
						hierachyExpand = true;
						parentCol = columns[columns.IndexOf(thisArea[0]) - 1];
						parent = parentCol.Metadata;
						child = thisArea[0].Metadata;
				}
			}
			this.columns = columns;
			this.columns2 = new Dictionary<string, OLAPCubeColumn>();
			foreach(OLAPCubeColumn column in columns) {
				if(column.ParentColumn != null || column.Metadata.ChildColumn != null) {
					List<IQueryMetadataColumn> cols = column.Metadata.GetColumnHierarchy();
					foreach(OLAPMetadataColumn meta2 in cols)
						columns2[meta2.UniqueName] = column.Owner.CubeColumns[meta2.UniqueName];
				}
				columns2[column.UniqueName] = column;
			}
		}
		public bool SetCurrent(IOLAPTuple current) {
			this.current = current;
			count = current.Count;
			if(count == 0)
				return false;
			if(hierachyExpand && count != 0) {
				OLAPMember w = current[count - GetLastDiff()];
				if(w.Column.UniqueName == parent.UniqueName) {
					expandedParent = parentCol[w.UniqueName];
					return false;
				}
			}
			return true;
		}
		protected virtual int GetLastDiff() {
			return 1;
		}
		public override QueryMember GetMember(int index) {
			if(hierachyExpand) {
				if(index == count - 1)
					return expandedParent;
				if(index == count)
					index--;
			}
			return current[index];
		}
		public override object GetValue(int i) {
			if(hierachyExpand) {
				if(i == count - 1)
					return expandedParent.UniqueLevelValue;
				if(i == count)
					i--;
			}
			return current[i].UniqueName;
		}
		public override int Length {
			get { return hierachyExpand ? count + 1 : count; }
		}
		public override void Add(int index, PreLastLevelRecord record, LevelRecord child) {
			record.AddRecord(child);
		}
	}
}
