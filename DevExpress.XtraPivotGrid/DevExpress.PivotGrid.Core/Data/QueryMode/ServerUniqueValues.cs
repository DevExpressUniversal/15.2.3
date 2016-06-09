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
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPivotGrid;
using System.Linq;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.PivotGrid.QueryMode {
	class UniqueValuesComparer : IComparer, IComparer<object> {
		public int Compare(object a, object b) {
			int res = -1;
			try {
				res = Comparer.Default.Compare(a, b);
			} catch {
				if(a != null) {
					if(b != null) {
						int result = Comparer<bool>.Default.Compare(a.GetType().IsValueType(), b.GetType().IsValueType());
						if(result != 0)
							return -result;
						result = Comparer<string>.Default.Compare(a.GetType().FullName, b.GetType().FullName);
						if(result != 0)
							return result;
					} else {
						return 1;
					}
				} else {
					if(b != null)
						return -1;
					else
						return 0;
				}
			}
			return res;
		}
	}
	public abstract class UniqueValues<AreasType> where AreasType : QueryColumn {
		readonly IDataSourceHelpersOwner<AreasType> owner;
		protected UniqueValues(IDataSourceHelpersOwner<AreasType> owner) {
			this.owner = owner;
		}
		protected IQueryMetadata Metadata { get { return owner.Metadata; } }
		protected IDataSourceHelpersOwner<AreasType> Owner { get { return owner; } }
		public object[] GetUniqueValues(PivotGridFieldBase field) {
			if(!IsValidField(field))
				return new object[0];
			AreasType column = Owner.CubeColumns[field];
			return GetUniqueValues(column);
		}
		public object[] GetUniqueValues(AreasType column) {
			if(column.UniqueValueMembersCache != null)
				return column.UniqueValueMembersCache;
			IEnumerable<object> uniqueValues = GetUniqueValueMembers(column);
			object[] result = uniqueValues.ToArray<object>();
			Array.Sort(result, new UniqueValuesComparer());
			column.UniqueValueMembersCache = result;
			return result;
		}
		protected abstract IEnumerable<object> GetUniqueValueMembers(AreasType column);
		protected abstract List<object> GetSortedUniqueValues(AreasType column);
		public abstract List<object> GetSortedUniqueGroupValues(PivotGridGroup group, object[] parentValues);
		public abstract bool? IsGroupFilterValueChecked(PivotGridGroup group, object[] parentValues, object value);
		public List<object> GetSortedUniqueValues(PivotGridFieldBase field) {
			if(!IsValidField(field))
				return new List<object>();
			AreasType column = Owner.CubeColumns[field];
			column.Assign(field, true);
			if(column.SortedUniqueValuesCache != null)
				return column.SortedUniqueValuesCache;
			List<object> values = GetSortedUniqueValues(column);
			column.SortedUniqueValuesCache = values;
			return values;
		}
		protected bool IsValidField(PivotGridFieldBase field) {
			AreasType res;
			if(Owner.CubeColumns.TryGetValue(field, out res))
				return !res.IsMeasure;
			return false;
		}
	}
}
