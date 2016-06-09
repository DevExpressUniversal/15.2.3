#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class PivotAxisModel<T> {
		public IEnumerable<T> Dimensions { get; set; }
		public AxisExpandModel ExpandModel { get; set; }
	}
	public enum ExpandAction { Expand, Collapse }
	public class AxisExpandModel : DataModelBase<AxisExpandModel> {
		public IList<object[]> Values { get; private set; }
		public ExpandAction ExpandAction { get; private set; }
		public AxisExpandModel(ExpandAction expandAction) {
			Values = new List<object[]>();
			ExpandAction = expandAction;
		}
		public AxisExpandModel(IList<object[]> values, ExpandAction expandAction)
			: this(expandAction) {
			this.Values = values;
		}
		protected override int ModelHashCode() {
			return HashcodeHelper.GetCompositeHashCode(Values)
				^ ExpandAction.GetHashCode();
		}
		protected override bool ModelEquals(AxisExpandModel other) {
			return ExpandAction == other.ExpandAction
				&& Values.Count == other.Values.Count
				&& Values.SequenceEqual(other.Values, new EnumerableEqualityComparer<object>());
		}
		public static AxisExpandModel FullExpandModel() {
			return new AxisExpandModel(ExpandAction.Collapse);
		}
		public static AxisExpandModel FullCollapseModel() {
			return new AxisExpandModel(ExpandAction.Expand);
		}
		public CollapsedState ToPivotCollapsedState() {
			Dictionary<int, List<object[]>> expandsByLevel = new Dictionary<int, List<object[]>>();
			List<object[]> expands;
			foreach(object[] expand in Values) {
				if(!expandsByLevel.TryGetValue(expand.Length, out expands)) {
					expands = new List<object[]>();
					expandsByLevel[expand.Length] = expands;
				}
				expands.Add(expand);
			}
			List<LevelCollapsedState> state = new List<LevelCollapsedState>();
			foreach(List<object[]> levelExpands in expandsByLevel.Values) {
				LevelCollapsedState levelState = new LevelCollapsedState(ExpandAction == ExpandAction.Collapse, levelExpands.Count);
				levelState.AddRange(levelExpands);
				state.Add(levelState);
			}
			CollapsedState result = new CollapsedState(false, ExpandAction == ExpandAction.Collapse, state);
			return result;
		}
	}
}
