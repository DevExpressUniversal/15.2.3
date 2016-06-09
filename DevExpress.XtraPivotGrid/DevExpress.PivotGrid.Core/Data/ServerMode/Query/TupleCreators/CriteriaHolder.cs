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
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.PivotGrid.ServerMode {
	class EmptyCriteriaHolder : BaseCriteriaHolder {
		IRawCriteriaConvertible conv;
		public EmptyCriteriaHolder()
			: base(new List<QueryMember>()) {
		}
		public EmptyCriteriaHolder(IRawCriteriaConvertible conv)
			: base(new List<QueryMember>()) {
			this.conv = conv;
		}
		protected override CriteriaOperator GetRawCriteria() {
			return conv == null ? null : conv.GetRawCriteria();
		}
	}
	abstract class BaseCriteriaHolder : IRawCriteriaConvertible {
		public static BaseCriteriaHolder Empty {
			get { return new EmptyCriteriaHolder(); }
		}
		readonly List<QueryMember> others = new List<QueryMember>();
		public CriteriaOperator Criteria { get { return GetRawCriteria(); } }
		public List<QueryMember> Others { get { return others; } }
		protected BaseCriteriaHolder(IEnumerable<BaseCriteriaHolder> convs) {
			if(convs != null)
				foreach(BaseCriteriaHolder holder in convs)
					Add(holder);
		}
		protected BaseCriteriaHolder(IEnumerable<BaseCriteriaHolder> convs, QueryMember basic)
			: this(basic) {
			if(convs != null)
				foreach(BaseCriteriaHolder holder in convs)
					Add(holder);
		}
		void Add(BaseCriteriaHolder holder) {
			others.AddRange(holder.others);
		}
		protected BaseCriteriaHolder(QueryMember basic) {
			others.Add(basic);
		}
		protected BaseCriteriaHolder(List<QueryMember> basic) {
			others.AddRange(basic);
		}
		protected BaseCriteriaHolder(BaseCriteriaHolder basic) {
			others.AddRange(basic.others);
		}
		protected BaseCriteriaHolder(BaseCriteriaHolder basic, QueryMember other)
			: this(basic) {
			others.Add(other);
		}
		CriteriaOperator IRawCriteriaConvertible.GetRawCriteria() {
			return GetRawCriteria();
		}
		protected abstract CriteriaOperator GetRawCriteria();
	}
	class AndCriteriaHolder : BaseCriteriaHolder {
		BaseCriteriaHolder holder;
		IRawCriteriaConvertible criteriaConv;
		public AndCriteriaHolder(BaseCriteriaHolder holder, IRawCriteriaConvertible iRawCriteriaConvertible)
			: base(holder) {
			this.holder = holder;
			this.criteriaConv = iRawCriteriaConvertible;
		}
		protected override CriteriaOperator GetRawCriteria() {
			CriteriaOperator criteria = criteriaConv == null ? null : criteriaConv.GetRawCriteria();
			return CriteriaOperator.And(criteria, holder.Criteria);
		}
	}
	class OrMembers : BaseCriteriaHolder {
		readonly IEnumerable<QueryMember> convs;
		public OrMembers(IEnumerable<QueryMember> convs)
			: base((IEnumerable<BaseCriteriaHolder>)null) {
			this.convs = convs;
		}
		protected override CriteriaOperator GetRawCriteria() {
			CriteriaOperator op = null;
			if(convs != null) {
				foreach(IRawCriteriaConvertible conv1 in convs)
					op = CriteriaOperator.Or(op, conv1.GetRawCriteria());
			}
			return op;
		}
	}
	class OrCrossJoinCriteria : BaseCriteriaHolder {
		readonly IEnumerable<IRawCriteriaConvertible> convs;
		readonly IRawCriteriaConvertible conv;
		public OrCrossJoinCriteria(IEnumerable<BaseCriteriaHolder> convs, OthersMember member)
			: base(convs, member) {
			this.convs = convs;
			this.conv = member;
		}
		public OrCrossJoinCriteria(IEnumerable<BaseCriteriaHolder> convs, IRawCriteriaConvertible conv)
			: base(convs) {
			this.convs = convs;
			this.conv = conv;
		}
		protected override CriteriaOperator GetRawCriteria() {
			CriteriaOperator op = null;
			if(convs != null) {
				foreach(IRawCriteriaConvertible conv1 in convs)
					op = CriteriaOperator.Or(op, CriteriaOperator.And(conv1.GetRawCriteria(), conv.GetRawCriteria()));
			}
			return op ?? conv.GetRawCriteria();
		}
	}
	class OrCrossCriteriaHolder : BaseCriteriaHolder {
		readonly IEnumerable<IRawCriteriaConvertible> convs;
		public OrCrossCriteriaHolder(IEnumerable<IRawCriteriaConvertible> convs) : base((IEnumerable<BaseCriteriaHolder>)null) {
			this.convs = convs;
		}
		protected override CriteriaOperator GetRawCriteria() {
			CriteriaOperator op = null;
			if(convs != null) {
				foreach(IRawCriteriaConvertible conv1 in convs)
					op = CriteriaOperator.Or(op, conv1.GetRawCriteria());
			}
			return op;
		}
	}
}
