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

using System;
using System.Collections;
namespace DevExpress.DashboardCommon.Native {
	public class DashboardItemState : BinarySerializableObjectRepository<BaseItemStateHolder> {
		const string ActiveElementStateKey = "ActiveElement";
		const string DrillDownStateKey = "DrillDown";
		const string MasterFilterStateKey = "MasterFilter";
		const string RangeFilterStateKey = "RangeFilter";
		const string PivotExpandCollapseStateKey = "PivotExpandCollapseState";
		public BaseItemStateHolder this[string key] {
			get {
				BaseItemStateHolder state;
				return TryGetValue(key, out state) ? state : null;
			}
			set { SetValue(key, value); }
		}
		public int ActiveElementState {
			get {
				object state = GetState(ActiveElementStateKey);
				return (state != null) ? (int)state : 0;
			}
			set { this[ActiveElementStateKey] = new ActiveElementValueStateHolder { State = value }; }
		}
		public IList DrillDownState {
			get { return (IList)GetState(DrillDownStateKey); }
			set { this[DrillDownStateKey] = new DrillDownSateHolder { State = value }; }
		}
		public MasterFilterState MasterFilterState {
			get { return (MasterFilterState)GetState(MasterFilterStateKey); }
			set { this[MasterFilterStateKey] = new MasterFilterStateHolder { State = value }; }
		}
		public RangeFilterDashboardItemState RangeFilterState {
			get { return (RangeFilterDashboardItemState)GetState(RangeFilterStateKey); }
			set { this[RangeFilterStateKey] = new RangeFilterStateHolder { State = value }; }
		}
		public PivotDashboardItemExpandCollapseState PivotExpandCollapseState {
			get { return (PivotDashboardItemExpandCollapseState)GetState(PivotExpandCollapseStateKey); }
			set { this[PivotExpandCollapseStateKey] = new PivotStateHolder { State = value }; }
		}
		protected object GetState(string key) {
			BaseItemStateHolder holder = this[key];
			return holder != null ? holder.State : null;
		}
		protected override BaseItemStateHolder CreateValue(string name) {
			switch(name) {
				case ActiveElementStateKey:
					return new ActiveElementValueStateHolder();
				case DrillDownStateKey:
					return new DrillDownSateHolder();
				case MasterFilterStateKey:
					return new MasterFilterStateHolder();
				case RangeFilterStateKey:
					return new RangeFilterStateHolder();
				case PivotExpandCollapseStateKey:
					return new PivotStateHolder();
				default:
					throw new NotSupportedException();
			}
		}
	}
}
