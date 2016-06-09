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
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.PivotGrid.ServerMode {
	class QueryTupleListToCriteriaOperatorConverter : IRawCriteriaConvertible {
		 static IRawCriteriaConvertible empty = new CriteriaOperatorToServerModeCriteria(new OperandValue(true), null, null, false);
		class CriteriaDictionary : Dictionary<IRawCriteriaConvertible, CriteriaDictionary> {
			IRawCriteriaConvertible member;
			public CriteriaDictionary() {
			}
			public CriteriaDictionary(IRawCriteriaConvertible member) {
				this.member = member;
			}
			public void Add(QueryTuple tuple) {
				if(tuple.MemberCount == 0)
					return;
				Add(tuple, 0);				
			}
			public void Add(QueryTuple tuple, int index) {
				IRawCriteriaConvertible member = tuple[index] as IRawCriteriaConvertible;
				if(member == null)
					return;
				CriteriaDictionary dic = null;
				if(!TryGetValue(member, out dic)) {
					dic = new CriteriaDictionary(member);
					Add(member, dic);
				}
				if(tuple.MemberCount > ++index)
					dic.Add(tuple, index);
			}
			public CriteriaOperator GetCriteria() {
				CriteriaOperator criteria = null;
				if(member != null)
					criteria = member.GetRawCriteria();
				if(Count == 0)
					return criteria;
				return CriteriaOperator.And(criteria, CriteriaOperator.Or(this.Select(s => s.Value.GetCriteria())));  
			}
			public void TryMinify(List<GroupInfo> currentFirstLevel) {
				if(currentFirstLevel == null)
					return;
				int count = currentFirstLevel.Count;
				if(count > 0 && currentFirstLevel[0] == GroupInfo.GrandTotalGroup)
					count--;
				foreach(GroupInfo info in currentFirstLevel) {
					IRawCriteriaConvertible member = info.Member as IRawCriteriaConvertible;
					if(member == null)
						continue;
					CriteriaDictionary dic = null;
					if(TryGetValue(member, out dic)) {
						dic.TryMinify(info.GetChildren());
					}
				}
				if(count == Count) {
					CriteriaDictionary dic = new CriteriaDictionary();
					foreach(KeyValuePair<IRawCriteriaConvertible, CriteriaDictionary> pair in this) {
						pair.Value.member = null;
						foreach(KeyValuePair<IRawCriteriaConvertible, CriteriaDictionary> pair2 in pair.Value) {
							CriteriaDictionary inner;
							if(dic.TryGetValue(pair2.Key, out inner))
								inner.Add(pair2.Value);
							else
								dic.Add(pair2.Key, pair2.Value);
						}
					}
					Clear();
					if(dic.Count != 0)
						Add(empty, dic);
				}
			}
			void Add(CriteriaDictionary toAdd) {
				if(Count == 0)
					return;
				if(toAdd.Count == 0) {
					Clear();
					return;
				}
				foreach(KeyValuePair<IRawCriteriaConvertible, CriteriaDictionary> pair in toAdd) {
					pair.Value.member = null;
					CriteriaDictionary inner;
					if(TryGetValue(pair.Key, out inner)) {
						inner.Add(pair.Value);
					} else {
						Add(pair.Key, pair.Value);
					}
				}
			}
			public int CountRecursive() {
				int count = 1;
				foreach(KeyValuePair<IRawCriteriaConvertible, CriteriaDictionary> pair in this)
					count += pair.Value.CountRecursive();
				return count;
			}
		}
		class CriteriaDictionaryRoot {
			protected readonly IEnumerable<QueryTuple> tuples;
			CriteriaOperator criteria;
			public CriteriaDictionaryRoot(IEnumerable<QueryTuple> tuples) {
				this.tuples = tuples;
			}
			public CriteriaOperator GetCriteria() {
				if(ReferenceEquals(null, criteria))
					criteria = CreateCriteria();
				return criteria;
			}
			protected virtual CriteriaOperator CreateCriteria() {
				CriteriaDictionary nodes = new CriteriaDictionary();
				foreach(QueryTuple tuple in tuples)
					nodes.Add(tuple);
				 return CriteriaOperator.Or(nodes.Select(s => s.Value.GetCriteria()));
			}
		}
		class CriteriaFetchedDictionaryRoot : CriteriaDictionaryRoot {
			List<GroupInfo> currentFirstLevel;
			int maxFetchCount;
			public CriteriaFetchedDictionaryRoot(IEnumerable<QueryTuple> tuples, List<GroupInfo> groupInfo, int maxFetchCount) : base(tuples) {
				this.currentFirstLevel = groupInfo;
				this.maxFetchCount = maxFetchCount;
			}
			protected override CriteriaOperator CreateCriteria() {
				if(tuples.Count() > 1000)
					return null;
				CriteriaDictionary nodes = new CriteriaDictionary();
				foreach(QueryTuple tuple in tuples)
					nodes.Add(tuple);
				nodes.TryMinify(currentFirstLevel);
				if(nodes.CountRecursive() > maxFetchCount)
					return null;
				return nodes.GetCriteria();
			}
		}
		readonly IEnumerable<QueryTuple> tuples;
		CriteriaDictionaryRoot root;
		public QueryTupleListToCriteriaOperatorConverter(params QueryTuple[] tuples) {
			if(tuples != null && tuples.Length > 0 && tuples[0] != null)
				this.tuples = tuples;
			else
				this.tuples = null;
		}
		public QueryTupleListToCriteriaOperatorConverter(IEnumerable<QueryTuple> tuples) {
			this.tuples = tuples;
		}
		public QueryTupleListToCriteriaOperatorConverter(IEnumerable<QueryTuple> tuples, List<GroupInfo> groupInfo, int maxFetchCount) {
			this.tuples = tuples;
			root = new CriteriaFetchedDictionaryRoot(tuples, groupInfo, maxFetchCount);
		}
		CriteriaOperator IRawCriteriaConvertible.GetRawCriteria() {
			if(tuples == null)
				return null;
			if(root == null)
				root = new CriteriaDictionaryRoot(tuples);
			return root.GetCriteria();
		}
	}
}
