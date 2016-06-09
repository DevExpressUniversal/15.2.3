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
using DevExpress.XtraPivotGrid;
using System;
using System.Linq;
namespace DevExpress.PivotGrid.ServerMode {
	class UniqueValues : UniqueValues<ServerModeColumn> {
		public new IServerModeHelpersOwner Owner { get { return (IServerModeHelpersOwner)base.Owner; } }
		public UniqueValues(ServerModeDataSource ds)
			: base(ds) {
		}
		protected override IEnumerable<object> GetUniqueValueMembers(ServerModeColumn column) {
			return Owner.QueryValues(column, null);
		}
		public override List<object> GetSortedUniqueGroupValues(PivotGridGroup group, object[] parentValues) {
			if(parentValues.Length >= group.Count)
				return new List<object>();
			Dictionary<QueryColumn, object> values = new Dictionary<QueryColumn, object>();
			if(parentValues != null)
				for(int i = 0; i < parentValues.Length; i++)
					values.Add(Owner.CubeColumns[group[i]], parentValues[i]);
			return Owner.QueryValues(Owner.CubeColumns[group[parentValues.Length]], values);
		}
		protected override List<object> GetSortedUniqueValues(ServerModeColumn column) {
			List<object> values = Owner.QueryValues(column, null);
			return column.SortOrder == PivotSortOrder.Ascending ? values.OrderBy((a) => a).ToList() : values.OrderByDescending((a) => a).ToList();
		}
		public override bool? IsGroupFilterValueChecked(PivotGridGroup group, object[] parentValues, object value) {
			int parentCount = parentValues != null ? parentValues.Length : 0;
			object[] items = new object[parentCount + 1];
			if(parentCount > 0)
				Array.Copy(parentValues, items, parentCount);
			items[parentCount] = value;
			int index = 0;
			PivotGroupFilterValuesCollection level = group.FilterValues.Values;
			while(index <= parentCount) {
				PivotGroupFilterValue levelValue = level[items[index]];
				if(levelValue == null)
					return group.FilterValues.FilterType == PivotFilterType.Included;
				if(levelValue.IsLastLevel || levelValue.ChildValues == null || levelValue.ChildValues.Count == 0)
					return group.FilterValues.FilterType != PivotFilterType.Included;
				level = levelValue.ChildValues;
				index++;
			}
			return level.IsEmpty ? (bool?)(group.FilterValues.FilterType == PivotFilterType.Included) : null;
		}
	}
}
