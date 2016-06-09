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
using DevExpress.PivotGrid.QueryMode;
using DevExpress.PivotGrid.QueryMode.TuplesTree;
using DevExpress.Compatibility.System;
using System.Linq;
using DevExpress.Data.Filtering;
namespace DevExpress.PivotGrid.ServerMode.TuplesTree {
	class MemberCreator : MemberCreatorBase {
		readonly int length;
		readonly ValuePresenterBase[] presenters;
		object[] values;
		int realCount;
		public override int Length { get { return length; } }
		public int RealCount { get { return realCount; } }
		public MemberCreator(IList<IGroupCriteriaConvertible> groups, List<ServerModeColumn> area, List<ServerModeColumn> allArea, IEnumerable<QueryMember> others, int startIndex) {
			length = groups.Count;
			realCount = 0;
			presenters = new ValuePresenterBase[length];
			for(int i = 0; i < length; i++) {
				IQueryMetadataColumn metadata = allArea[i].Metadata;
				QueryMember topN = others == null ? null : others.FirstOrDefault(o => o.Column == allArea[i].Metadata);
				if(topN == null) {
					OperandValue val = groups[i].GetGroupCriteria() as OperandValue;
					if(ReferenceEquals(null, val)) {
						presenters[i] = new OrdinalPresenter(this, realCount + startIndex, metadata);
						realCount++;
					} else {
						presenters[i] = new ConstantPresenter(val.Value, metadata);
					}
				} else {
					presenters[i] = new OthersPresenter((ServerModeMember)topN);
				}
			}
		}
		public override QueryMember GetMember(int index) {
			return presenters[index].GetMember();
		}
		public void SetValues(object[] values) {
			this.values = values;
		}
		public bool IsTotal(int index) {
			return presenters[index].GetIsTotal();
		}
		public override object GetValue(int index) {
			return presenters[index].GetValue();
		}
		public override void Add(int index, PreLastLevelRecord record, LevelRecord child) {
			presenters[index].Add(record, child);
		}
		abstract class ValuePresenterBase {
			public abstract object GetValue();
			public abstract ServerModeMember GetMember();
			public abstract bool GetIsTotal();
			public abstract void Add(PreLastLevelRecord record, LevelRecord child);
		}
		class OthersPresenter : ValuePresenterBase {
			ServerModeMember others;
			public OthersPresenter(ServerModeMember others) {
				this.others = others;
			}
			public override object GetValue() {
				return OthersValueColumn.OthersValue;
			}
			public override ServerModeMember GetMember() {
				return others;
			}
			public override bool GetIsTotal() {
				return false;
			}
			public override void Add(PreLastLevelRecord record, LevelRecord child) {
				record.AddOthersRecord(child);
			}
		}
		class TotalPresenter : ValuePresenterBase {
			ServerModeMember total;
			public TotalPresenter(ServerModeMember total) {
				this.total = total;
			}
			public override object GetValue() {
				return QueryMember.TotalValue;
			}
			public override ServerModeMember GetMember() {
				return total;
			}
			public override bool GetIsTotal() {
				return true;
			}
			public override void Add(PreLastLevelRecord record, LevelRecord child) {
				record.AddTotalRecord(child);
			}
		}
		class ConstantPresenter : ValuePresenterBase {
			readonly object value;
			readonly IQueryMetadataColumn column;
			public ConstantPresenter(object value, IQueryMetadataColumn column) {
				this.column = column;
				this.value = value;
			}
			public override object GetValue() {
				return value;
			}
			public override ServerModeMember GetMember() {
				return new ServerModeMember(column, GetValue());
			}
			public override bool GetIsTotal() {
				return false;
			}
			public override void Add(PreLastLevelRecord record, LevelRecord child) {
				record.AddRecord(child);
			}
		}
		class OrdinalPresenter : ValuePresenterBase {
			readonly int index;
			readonly IQueryMetadataColumn column;
			readonly MemberCreator creator;
			public OrdinalPresenter(MemberCreator creator, int index, IQueryMetadataColumn column) {
				this.index = index;
				this.column = column;
				this.creator = creator;
			}
			public override object GetValue() {
				object value = creator.values[index];
				return value == DBNull.Value ? null : value;
			}
			public override ServerModeMember GetMember() {
				return new ServerModeMember(column, GetValue());
			}
			public override bool GetIsTotal() {
				return false;
			}
			public override void Add(PreLastLevelRecord record, LevelRecord child) {
				record.AddRecord(child);
			}
		}
	}
}
