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

using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.XtraPivotGrid;
namespace DevExpress.PivotGrid.QueryMode {
	interface ICurrentFilterInfo {
		CriteriaOperator Criteria { get; }
	}
	class CurrentFilterInfos : IEnumerable<ICurrentFilterInfo> {
		Dictionary<QueryColumn, CurrentFilterInfo> filterInfos = new Dictionary<QueryColumn,CurrentFilterInfo>();
		public void Remove(List<QueryColumn> foundedColumns) {
			List<QueryColumn> missings = new List<QueryColumn>();
			foreach(KeyValuePair<QueryColumn, CurrentFilterInfo> pair in filterInfos)
				if(!foundedColumns.Contains(pair.Key))
					missings.Add(pair.Key);
			foreach(QueryColumn missing in missings)
				filterInfos.Remove(missing);
		}
		public bool IsFieldChanged(QueryColumn key, PivotGridFieldBase field) {
			CurrentFilterInfo filterInfo;
			if(filterInfos.TryGetValue(key, out filterInfo)) {
				if(filterInfo.IsFieldChanged(field)) {
					filterInfo.Assign(field, key);
					return true;
				}
			} else {
				filterInfo = new CurrentFilterInfo();
				filterInfo.Assign(field, key);
				filterInfos.Add(key, filterInfo);
			}
			return false;
		}
		public void Clear() {
			filterInfos.Clear();
		}
		IEnumerator<ICurrentFilterInfo> IEnumerable<ICurrentFilterInfo>.GetEnumerator() {
			foreach(KeyValuePair<QueryColumn, CurrentFilterInfo> pair in filterInfos)
				yield return pair.Value;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			foreach(KeyValuePair<QueryColumn, CurrentFilterInfo> pair in filterInfos)
				yield return pair.Value;
		}
	}
	class CurrentFilterInfo : ICurrentFilterInfo{
		PivotGridFieldFilterValues fieldFilterValues;
		PivotGroupFilterValues groupFilterValues;
		IFilterValues currentValues;
		CriteriaOperator ICurrentFilterInfo.Criteria {
			get {
				return currentValues.GetActualCriteria();
			}
		}
		IFilterValues GetCurrentValues() {
			IFilterValues vals = groupFilterValues as IFilterValues;
			if(vals == null || groupFilterValues.IsEmpty)
				vals = fieldFilterValues as IFilterValues;
			return vals;
		}
		bool IsFirstGroupField(PivotGridFieldBase field) {
			return field.Group != null && field == field.Group[0];
		}
		public void Assign(PivotGridFieldBase field, QueryColumn column) {
			this.fieldFilterValues = field.FilterValues.Clone(true);
			if(IsFirstGroupField(field))
				this.groupFilterValues = field.Group.FilterValues.Clone();
			currentValues = GetCurrentValues();
		}
		public bool IsFieldChanged(PivotGridFieldBase field) {
			if(!this.fieldFilterValues.IsEquals(field.FilterValues))
				return true;
			if(IsFirstGroupField(field) && (this.groupFilterValues == null || !this.groupFilterValues.IsEquals(field.Group.FilterValues)))
				return true;
			return currentValues != GetCurrentValues();
		}
	}
}
