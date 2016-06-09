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
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.QueryMode;
using System.Collections;
using DevExpress.Utils;
namespace DevExpress.PivotGrid.ServerMode {
	class OthersMember : ServerModeMember {
		List<QueryMember> members;
		public override bool IsOthers { get { return true; } }
		public OthersMember(IQueryMetadataColumn column, List<QueryMember> members)
			: base(column, OthersValueColumn.OthersValue) {
			this.members = members;
		}
		protected override CriteriaOperator GetCriteriaCore(object value) {
			CriteriaOperator criteria = new UnaryOperator(UnaryOperatorType.Not, CriteriaOperator.Or((IEnumerable<CriteriaOperator>)DXListExtensions.ConvertAll<QueryMember, CriteriaOperator>(members, (member) => GetMemberExpression(member.Value))));
			if(members.Count > 0 && (members[0].Value is string || members[0].Column.HasNullValues == true) && members.Find((m) => object.ReferenceEquals(null, m.Value)) == null)
				return CriteriaOperator.Or(criteria, ((IRawCriteriaConvertible)new ServerModeMember(members[0].Column, null)).GetRawCriteria());
			return criteria;
		}
	}
	class TotalMemberBase : ServerModeMember {
		public override bool IsTotal {
			get { return true; }
		}
		public TotalMemberBase(IQueryMetadataColumn column) : base(column, QueryMember.TotalValue) { }
	}
	class TotalMember : TotalMemberBase {
		List<QueryMember> members;
		public TotalMember(IQueryMetadataColumn column, List<QueryMember> members) : base(column) {
			this.members = members;
		}
		protected override CriteriaOperator GetCriteriaCore(object value) {
			return CriteriaOperator.Or((IEnumerable<CriteriaOperator>)DXListExtensions.ConvertAll<QueryMember, CriteriaOperator>(members, (member) => GetMemberExpression(member.Value)));
		}
	}
	class ServerModeMember : QueryMember, IRawCriteriaConvertible, IComparable {
		IGroupCriteriaConvertible CriteriaColumn { get { return ((IGroupCriteriaConvertible)Column); } }
		public ServerModeMember(IQueryMetadataColumn column, object value)
			: base(column, value) {
		}
		CriteriaOperator IRawCriteriaConvertible.GetRawCriteria() {
			return GetCriteriaCore(Value);
		}
		protected virtual CriteriaOperator GetCriteriaCore(object value) {
			return GetMemberExpression(value);
		}
		protected CriteriaOperator GetMemberExpression(object value) {
			if(ReferenceEquals(null, value))
				return new NullOperator(CriteriaColumn.GetGroupCriteria());
			else
				return new BinaryOperator(
					  CriteriaColumn.GetGroupCriteria(),
					  new OperandValue(value),
					  BinaryOperatorType.Equal
					 );
		}
		public override bool Equals(object obj) {
			ServerModeMember member = obj as ServerModeMember;
			if(member == null)
				return false;
			if(Value == null)
				return member.Value == null && member.Column == Column;
			if(member.Value == null || member.Value.GetType() != Value.GetType())
				return false;
			return Comparer.Default.Compare(member.Value, Value) == 0 && member.Column == Column;
		}
		public override int GetHashCode() {
			return Value == null ? -1 : Value.GetHashCode();
		}
		int IComparable.CompareTo(object obj) {
			throw new NotImplementedException(); 
		}
	}
}
