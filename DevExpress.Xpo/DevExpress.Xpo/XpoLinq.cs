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
using System.Reflection;
using System.Collections.Generic;
using DevExpress.Xpo;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;
using DevExpress.Xpo.Helpers;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.Xpo.Generators;
using System.ComponentModel;
using System.Text;
using DevExpress.Data.Filtering.Helpers;
using System.Drawing;
using System.Threading;
using System.Collections.ObjectModel;
namespace DevExpress.Xpo.Helpers {
	class EnumerableWrapper<T> : IEnumerable<T> {
		IEnumerable<T> parent;
		public EnumerableWrapper(IEnumerable<T> parent) {
			this.parent = parent;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return parent.GetEnumerator();
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return parent.GetEnumerator();
		}
	}
	public class ExpressionAccessOperator : CriteriaOperator {
		public Expression LinqExpression;
		public bool InsertFirstNull;
		public CriteriaOperator[] SourceItems;
		public ExpressionAccessOperator(Expression linqExpression, params CriteriaOperator[] sourceItems)
			: this(linqExpression, false, sourceItems) {
		}
		public ExpressionAccessOperator(Expression linqExpression, bool insertFirstNull, params CriteriaOperator[] sourceItems) {
			InsertFirstNull = insertFirstNull;
			LinqExpression = linqExpression;
			SourceItems = sourceItems;
		}
		public override void Accept(ICriteriaVisitor visitor) {
			throw new InvalidOperationException();
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			ILinqExtendedCriteriaVisitor<T> miVisitor;
			if((miVisitor = visitor as ILinqExtendedCriteriaVisitor<T>) == null) throw new InvalidOperationException();
			return miVisitor.Visit(this);
		}
		protected override CriteriaOperator CloneCommon() {
			throw new InvalidOperationException();
		}
		Type[] cachedSourceTypes;
		Type GetSourceType(CriteriaTypeResolver resolver, CriteriaOperator prop) {
			if(prop is GroupSet) return null;
			return resolver.Resolve(prop);
		}
		public Type[] GetSourceTypes(Type type, CriteriaTypeResolver resolver) {
			if(cachedSourceTypes == null) {
				Type[] sourceTypes = new Type[SourceItems.Length];
				for(int i = 0; i < sourceTypes.Length; i++) {
					sourceTypes[i] = GetSourceType(resolver, SourceItems[i]);
				}
				cachedSourceTypes = sourceTypes;
			}
			return cachedSourceTypes;
		}
		public override int GetHashCode() {
			int hashCode = 0x75334882;
			if(LinqExpression != null) hashCode ^= LinqExpression.NodeType.GetHashCode() ^ LinqExpression.Type.GetHashCode();
			if(SourceItems == null) {
				hashCode ^= SourceItems.Length.GetHashCode();
				foreach(CriteriaOperator source in SourceItems) {
					if(ReferenceEquals(source, null)) continue;
					hashCode ^= source.GetHashCode();
				}
			}
			return hashCode;
		}
		public override bool Equals(object obj) {
			if(ReferenceEquals(this, obj))
				return true; 
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			ExpressionAccessOperator another = (ExpressionAccessOperator)obj;
			if(LinqExpression != another.LinqExpression && (LinqExpression == null || another.LinqExpression == null)) return false;
			if(LinqExpression.NodeType != another.LinqExpression.NodeType || LinqExpression.Type != another.LinqExpression.Type) return false;
			if(SourceItems == another.SourceItems) return true;
			if(another.SourceItems == null || SourceItems.Length != another.SourceItems.Length) return false;
			for(int i = 0; i < SourceItems.Length; i++) {
				if(!CriterionEquals(SourceItems[i], another.SourceItems[i])) return false;
			}
			return true;
		}
	}
	public class QuerySet : CriteriaOperator {
		public CriteriaOperator Condition;
		public OperandProperty Property;
		public MemberInitOperator Projection;
		public QuerySet() { }
		public QuerySet(string name) {
			if (name != null)
				Property = new OperandProperty(name);
		}
		public QuerySet(OperandProperty property, CriteriaOperator condition) {
			Property = property;
			Condition = condition;
		}
		public QuerySet(MemberInitOperator projection) {
			Projection = projection;
		}
		public override void Accept(ICriteriaVisitor visitor) {
			IClientCriteriaVisitor clientVisitor = visitor as IClientCriteriaVisitor;
			if(IsEmpty) {
				if(clientVisitor != null) {
					clientVisitor.Visit(Parser.ThisCriteria);
					return;
				}
			} else if(ReferenceEquals(Projection, null) && !ReferenceEquals(Property, null)) {
				if(clientVisitor != null) {
					clientVisitor.Visit(Property);
					return;
				}
			}
			throw new InvalidOperationException();
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			var linqVisitor = visitor as ILinqExtendedCriteriaVisitor<T>;
			if(linqVisitor != null)
				return linqVisitor.Visit(this);
			var clientVisitor = visitor as IClientCriteriaVisitor<T>;
			if(IsEmpty) {
				if(clientVisitor != null) {
					return clientVisitor.Visit(Parser.ThisCriteria);
				}
			} else if(ReferenceEquals(Projection, null) && !ReferenceEquals(Property, null)) {
				if(clientVisitor != null) {
					return clientVisitor.Visit(Property);
				}
			}
			throw new InvalidOperationException();
		}
		protected override CriteriaOperator CloneCommon() {
			return new QuerySet() {
				Condition = Condition,
				Property = Property,
				Projection = Projection
			};
		}
		public bool IsEmpty {
			get { return ReferenceEquals(Condition, null) && ReferenceEquals(Property, null) && ReferenceEquals(Projection, null); }
		}
		internal static QuerySet Empty = new QuerySet();
		public override int GetHashCode() {
			int hashCode = 0x71238812;
			if(!ReferenceEquals(Condition, null)) hashCode ^= Condition.GetHashCode();
			if(!ReferenceEquals(Property, null)) hashCode ^= Property.GetHashCode();
			if(!ReferenceEquals(Projection, null)) hashCode ^= Projection.GetHashCode();
			return hashCode;
		}
		public override bool Equals(object obj) {
			if(ReferenceEquals(this, obj))
				return true;
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			QuerySet qs = (QuerySet)obj;
			return CriterionEquals(Condition, qs.Condition) && CriterionEquals(Property, qs.Property) && CriterionEquals(Projection, qs.Projection);
		}
	}
	public class GroupSet : QuerySet {
		public CriteriaOperator Key;
		public GroupSet() { }
		public GroupSet(MemberInitOperator projection, CriteriaOperator key)
			: base(projection) {
			this.Key = key;
		}
		public override void Accept(ICriteriaVisitor visitor) {
			throw new InvalidOperationException();
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			throw new InvalidOperationException();
		}
		protected override CriteriaOperator CloneCommon() {
			throw new InvalidOperationException();
		}
		public override int GetHashCode() {
			int hashCode = base.GetHashCode();
			if(!ReferenceEquals(Key, null)) hashCode ^= Key.GetHashCode();
			return hashCode;
		}
		public override bool Equals(object obj) {
			if(!base.Equals(obj)) return false;
			GroupSet gs = (GroupSet)obj;
			return CriterionEquals(Key, gs.Key);
		}
	}
	class JoinOperandInfo {
		public readonly Aggregate AggregateType;
		public readonly CriteriaOperator Condition;
		public readonly string JoinTypeName;
		public JoinOperandInfo(string joinTypeName, CriteriaOperator condition, Aggregate aggregateType) {
			AggregateType = aggregateType;
			Condition = condition;
			JoinTypeName = joinTypeName;
		}
		public JoinOperandInfo(JoinOperand joinOperand)
			: this(joinOperand.JoinTypeName, joinOperand.Condition, joinOperand.AggregateType) {
		}
		public override bool Equals(object obj) {
			JoinOperandInfo other = obj as JoinOperandInfo;
			if(other == null) return false;
			return JoinTypeName == other.JoinTypeName && CriteriaOperator.CriterionEquals(Condition, other.Condition) && AggregateType == other.AggregateType;
		}
		public override int GetHashCode() {
			int hashCode = AggregateType.GetHashCode();
			if(JoinTypeName != null) hashCode ^= JoinTypeName.GetHashCode();
			if(!ReferenceEquals(Condition, null)) hashCode ^= Condition.GetHashCode();
			return hashCode;
		}
	}
	public class FreeQuerySet : QuerySet {
		struct FreeJoinPair {
			public readonly CriteriaOperator UpOperand;
			public readonly CriteriaOperator Operand;
			public FreeJoinPair(CriteriaOperator upOperand, CriteriaOperator operand) {
				UpOperand = upOperand;
				Operand = operand;
			}
		}
		public Type JoinType;
		JoinOperandInfo MasterJoin;		
		List<FreeJoinPair> MasterJoinEqualsOperands;
		public bool HasMasterJoin {
			get { return MasterJoin != null; }
		}
		public CriteriaOperator CreateJoinOperand(CriteriaOperator condition, CriteriaOperator expression, Aggregate aggregateType) {
			if(HasMasterJoin) {
				JoinOperand nestedJoin = new JoinOperand(JoinType.FullName, GetGroup(condition, MasterJoinEqualsOperands), aggregateType, expression);
				return new JoinOperand(MasterJoin.JoinTypeName, MasterJoin.Condition, MasterJoin.AggregateType, nestedJoin);
			}
			return new JoinOperand(JoinType.FullName, condition, aggregateType, expression);
		}
		public MemberInitOperator CreateSingleUntypedMemberInitOperator() {
			return new MemberInitOperator(null, new XPMemberAssignmentCollection() { new XPMemberAssignment(this) }, false);
		}
		public FreeQuerySet() { }
		public FreeQuerySet(Type joinType, CriteriaOperator upLevelOperand, CriteriaOperator operand):this(joinType, upLevelOperand, operand, null) { }
		static GroupOperator GetGroup(CriteriaOperator condition, List<FreeJoinPair> joinOperands) {
			GroupOperator group = new GroupOperator(GroupOperatorType.And);
			for(int i = 0; i < joinOperands.Count; i++) {
				group.Operands.Add(new BinaryOperator(joinOperands[i].UpOperand, joinOperands[i].Operand, BinaryOperatorType.Equal));
			}
			if(!ReferenceEquals(condition, null)) group.Operands.Add(condition);
			return group;
		}
		public FreeQuerySet(Type joinType, CriteriaOperator upLevelOperand, CriteriaOperator operand, CriteriaOperator condition) {
			JoinType = joinType;
			MemberInitOperator upLevelMemberInit = upLevelOperand as MemberInitOperator;
			MemberInitOperator memberInit = operand as MemberInitOperator;
			bool isUpLevelMemberInit = !ReferenceEquals(upLevelMemberInit, null);
			bool isMemberInit = !ReferenceEquals(memberInit, null);
			if(isUpLevelMemberInit != isMemberInit) throw new NotSupportedException(Res.GetString(Res.LinqToXpo_SpecifiedJoinKeySelectorsNotCompatibleX0X1, upLevelOperand, operand));
			if(isUpLevelMemberInit && isMemberInit) {
				if(upLevelMemberInit.Members.Count != memberInit.Members.Count) throw new NotSupportedException(Res.GetString(Res.LinqToXpo_SpecifiedJoinKeySelectorsNotCompatibleX0X1, upLevelMemberInit, memberInit));
				JoinOperandInfo foundJoinOperand = null;
				List<FreeJoinPair> joinOperands = new List<FreeJoinPair>(memberInit.Members.Count);
				for(int i = 0; i < upLevelMemberInit.Members.Count; i++) {
					Dictionary<JoinOperandInfo, bool> upJoinOperandList;
					CriteriaOperator upLevelProperty = DownLevelReprocessor.Reprocess(upLevelMemberInit.Members[i].Property, out upJoinOperandList);
					if(upJoinOperandList != null) {
						if(upJoinOperandList.Count != 1) throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheJoinWithManyTablesSimultaneouslyInASing));
						JoinOperandInfo joinOperand = upJoinOperandList.Keys.First();
						if(foundJoinOperand == null) {
							foundJoinOperand = joinOperand;
						} else if(!foundJoinOperand.Equals(joinOperand)) throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheJoinWithManyTablesSimultaneouslyInASing));						
					}
					joinOperands.Add(new FreeJoinPair(upLevelProperty, memberInit.Members[i].Property));
				}
				if(!ReferenceEquals(foundJoinOperand, null)) {
					MasterJoin = foundJoinOperand;
					MasterJoinEqualsOperands = joinOperands;
					Condition = condition;
					return;
				}
				Condition = GetGroup(condition, joinOperands);
				return;
			}
			Dictionary<JoinOperandInfo, bool> joinOperandList;
			CriteriaOperator upOperand = DownLevelReprocessor.Reprocess(upLevelOperand, out joinOperandList);
			if(joinOperandList != null) {
				if(joinOperandList.Count != 1) throw new NotSupportedException(Res.GetString(Res.LinqToXpo_DoesnTSupportNestedJoinsWithLevel));
				MasterJoin = joinOperandList.Keys.First();
				MasterJoinEqualsOperands = new List<FreeJoinPair>();
				MasterJoinEqualsOperands.Add(new FreeJoinPair(upOperand, operand));
				Condition = condition;
				return;
			}
			if(!ReferenceEquals(condition, null))
				Condition = GroupOperator.And(new BinaryOperator(upOperand, operand, BinaryOperatorType.Equal), condition);
			else
				Condition = new BinaryOperator(upOperand, operand, BinaryOperatorType.Equal);
		}
		public FreeQuerySet(Type joinType, CriteriaOperator condition) {
			JoinType = joinType;
			Condition = condition;
		}
		public override void Accept(ICriteriaVisitor visitor) {
			IClientCriteriaVisitor clientVisitor = visitor as IClientCriteriaVisitor;
			if(clientVisitor == null) return;
			if(!ReferenceEquals(Projection, null)) {
				if(!Projection.CreateNewObject && Projection.Members.Count == 1) {
					clientVisitor.Visit((JoinOperand)CreateJoinOperand(Condition, Projection.Members[0].Property, Aggregate.Single));
					return;
				}
			}
			clientVisitor.Visit((JoinOperand)CreateJoinOperand(Condition, Parser.ThisCriteria, Aggregate.Single));
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			var clientVisitor = visitor as IClientCriteriaVisitor<T>;
			if(clientVisitor == null) return default(T);
			if(!ReferenceEquals(Projection, null)) {
				if(!Projection.CreateNewObject && Projection.Members.Count == 1)
					return clientVisitor.Visit((JoinOperand)CreateJoinOperand(Condition, Projection.Members[0].Property, Aggregate.Single));
			}
			return clientVisitor.Visit((JoinOperand)CreateJoinOperand(Condition, Parser.ThisCriteria, Aggregate.Single));
		}
		protected override CriteriaOperator CloneCommon() {
			return new FreeQuerySet() {
				Condition = Condition,
				Property = Property,
				Projection = Projection,
				JoinType = JoinType,
				MasterJoin = MasterJoin,
				MasterJoinEqualsOperands = MasterJoinEqualsOperands == null ? null : new List<FreeJoinPair>(MasterJoinEqualsOperands)
			};
		}
		public override int GetHashCode() {
			int hashCode = base.GetHashCode();
			if(!ReferenceEquals(JoinType, null)) hashCode ^= JoinType.GetHashCode();
			if(MasterJoin != null) hashCode ^= MasterJoin.GetHashCode();
			if(MasterJoinEqualsOperands != null) {
				int count = MasterJoinEqualsOperands.Count;
				hashCode ^= count.GetHashCode();
				for(int i = 0; i < count; i++) {
					FreeJoinPair pair = MasterJoinEqualsOperands[i];
					if(!ReferenceEquals(pair.UpOperand, null)) hashCode ^= pair.UpOperand.GetHashCode();
					if(!ReferenceEquals(pair.Operand, null)) hashCode ^= pair.Operand.GetHashCode();
				}
			}
			return hashCode;
		}
		public override bool Equals(object obj) {
			if(!base.Equals(obj)) return false;
			FreeQuerySet other = obj as FreeQuerySet;
			if(ReferenceEquals(other, null) || JoinType != other.JoinType) return false;
			if(!ReferenceEquals(MasterJoin, other.MasterJoin) && (ReferenceEquals(MasterJoin, null) || ReferenceEquals(other.MasterJoin, null))) return false;
			if(MasterJoin != null && !MasterJoin.Equals(other.MasterJoin)) return false;
			if(MasterJoinEqualsOperands == other.MasterJoinEqualsOperands) return true;
			if(MasterJoinEqualsOperands == null || other.MasterJoinEqualsOperands == null || MasterJoinEqualsOperands.Count != other.MasterJoinEqualsOperands.Count) return false;
			int count = MasterJoinEqualsOperands.Count;
			for(int i = 0; i < count; i++) {
				if(!CriteriaOperator.CriterionEquals(MasterJoinEqualsOperands[i].UpOperand, other.MasterJoinEqualsOperands[i].UpOperand)
					|| !CriteriaOperator.CriterionEquals(MasterJoinEqualsOperands[i].Operand, other.MasterJoinEqualsOperands[i].Operand)) return false;
			}
			return true;
		}
	}
	public interface ILinqExtendedCriteriaVisitor<T> {
		T Visit(MemberInitOperator theOperand);
		T Visit(ExpressionAccessOperator theOperand);
		T Visit(QuerySet theOperand);
	}
	public class MemberInitOperator : CriteriaOperator {
		ConstructorInfo constructor;
		internal static Type GetMemberType(MemberInfo mi) {
			switch (mi.MemberType) {
				case MemberTypes.Field: return ((FieldInfo)mi).FieldType;
				case MemberTypes.Property: return ((PropertyInfo)mi).PropertyType;
				case MemberTypes.Method: return ((MethodInfo)mi).ReturnType;
			}
			throw new ArgumentException();
		}
		Type GetDeclaringTypeInternal() {
			if(string.IsNullOrEmpty(DeclaringTypeAssemblyName))
				throw new InvalidOperationException(Res.GetString(Res.LinqToXpo_TheDeclaringTypeAssemblyNamePropertyIsEmpt));
			if(string.IsNullOrEmpty(DeclaringTypeName))
				throw new InvalidOperationException(Res.GetString(Res.LinqToXpo_TheDeclaringTypeNamePropertyIsEmpty));
			return DevExpress.Xpo.Helpers.XPTypeActivator.GetType(DeclaringTypeAssemblyName, DeclaringTypeName);
		}
		public Type GetDeclaringType() {
			if(constructor == null) return GetDeclaringTypeInternal();
			return constructor.DeclaringType;
		}
		Type[] cachedSourceTypes;
		Type GetSourceType(CriteriaTypeResolver resolver, CriteriaOperator prop){
			if(prop is GroupSet) return null;
			return resolver.Resolve(prop);
		}
		public Type[] GetSourceTypes(Type type, CriteriaTypeResolver resolver) {
			if(cachedSourceTypes == null) {
				if(CreateNewObject) {
					if(UseConstructor) {
						ConstructorInfo con = GetConstructor(type);
						ParameterInfo[] parameters = con.GetParameters();
						Type[] sourceTypes = new Type[parameters.Length];
						for(int i = 0; i < parameters.Length; i++)
							sourceTypes[i] = GetSourceType(resolver, Members[i].Property);
						cachedSourceTypes = sourceTypes;
					} else {
						Type[] sourceTypes = new Type[Members.Count];
						for(int i = 0; i < Members.Count; i++) {
							sourceTypes[i] = GetSourceType(resolver, Members[i].Property);
						}
						cachedSourceTypes = sourceTypes;
					}
				} else {
					Type elementType = type.GetElementType();
					Type[] sourceTypes = new Type[Members.Count];
					for(int i = 0; i < sourceTypes.Length; i++) {
						sourceTypes[i] = GetSourceType(resolver, Members[i].Property);
					}
					cachedSourceTypes = sourceTypes;
				}
			}
			return cachedSourceTypes;
		}
		public ConstructorInfo GetConstructor(Type type) {
			if (constructor == null) {
				if(type == null) {
					type = GetDeclaringTypeInternal();
					if(type == null)
						throw new ArgumentException("type");
				}
				Type[] types = new Type[UseConstructor ? Members.Count : 0];
				if (UseConstructor) {
					for (int i = 0; i < types.Length; i++) {
						MemberInfo mi = Members[i].GetMember(type);
						types[i] = GetMemberType(mi);
					}
				}
				constructor = type.GetConstructor(types);
			}
			return constructor;
		}
		[XmlAttribute]
		public string DeclaringTypeAssemblyName;
		[XmlAttribute]
		public string DeclaringTypeName;
		[XmlAttribute]
		public bool UseConstructor;
		[XmlAttribute]
		public bool CreateNewObject;
		public XPMemberAssignmentCollection Members;
		public MemberInitOperator() { }
		public MemberInitOperator(string declaringTypeAssemblyName, string declaringTypeName, XPMemberAssignmentCollection members, bool createNewObject) {
			this.Members = members;
			CreateNewObject = createNewObject;
			DeclaringTypeAssemblyName = declaringTypeAssemblyName;
			DeclaringTypeName = declaringTypeName;
		}
		public MemberInitOperator(Type declaringType, XPMemberAssignmentCollection members, bool createNewObject) {		   
			this.Members = members;
			CreateNewObject = createNewObject;
			if(declaringType != null) {
				DeclaringTypeAssemblyName = declaringType.Assembly.FullName;
				DeclaringTypeName = declaringType.FullName;
			}
		}
		public MemberInitOperator(bool useConstructor, XPMemberAssignmentCollection members, ConstructorInfo constructor) {
			this.Members = members;
			this.constructor = constructor;
			this.UseConstructor = useConstructor;
			CreateNewObject = true;
			DeclaringTypeAssemblyName = constructor.DeclaringType.Assembly.FullName;
			DeclaringTypeName = constructor.DeclaringType.FullName;
		}
		public override void Accept(ICriteriaVisitor visitor) {
			throw new InvalidOperationException();
		}
		public override T Accept<T>(ICriteriaVisitor<T> visitor) {
			ILinqExtendedCriteriaVisitor<T> miVisitor;
			if((miVisitor = visitor as ILinqExtendedCriteriaVisitor<T>) == null) throw new InvalidOperationException();
			return miVisitor.Visit(this);
		}
		protected override CriteriaOperator CloneCommon() {
			throw new InvalidOperationException();
		}
		public override int GetHashCode() {
			int hashCode = 0x71974591;
			hashCode ^= UseConstructor ? 0x70004524 : 0x79823475;
			hashCode ^= CreateNewObject ? 0x78988557 : 0x71113033;
			if(Members == null) {
				hashCode ^= Members.Count.GetHashCode();
				foreach(XPMemberAssignment source in Members) {
					if(ReferenceEquals(source, null)) continue;
					hashCode ^= source.GetHashCode();
				}
			}
			return hashCode;
		}
		public override bool Equals(object obj) {
			if(ReferenceEquals(this, obj)) 
				return true;
			if(obj == null)
				return false;
			if(!object.ReferenceEquals(this.GetType(), obj.GetType()))
				return false;
			MemberInitOperator another = (MemberInitOperator)obj;
			if(UseConstructor != another.UseConstructor || CreateNewObject != another.CreateNewObject) return false;
			if((UseConstructor && GetConstructor(null) != another.GetConstructor(null)) || DeclaringTypeName != another.DeclaringTypeName) return false;
			if(Members == another.Members) return true;
			if(Members == null || another.Members == null || Members.Count != another.Members.Count) return false;
			for(int i = 0; i < Members.Count; i++) {
				if(Members[i].MemberName != another.Members[i].MemberName || !CriterionEquals(Members[i].Property, another.Members[i].Property)) return false;
			}
			return true;
		}
		public override string ToString() {
			if(CreateNewObject) {
				StringBuilder sb = new StringBuilder();
				sb.Append("new ");
				if(!DeclaringTypeName.Contains("<>")) {
					sb.Append(DeclaringTypeName);
					sb.Append(' ');
				}
				string closingString;
				if(UseConstructor) {
					sb.Append("(");
					closingString = ")";
				} else {
					sb.Append("{ ");
					closingString = " }";
				}
				if(Members != null){
					for(int i = 0; i < Members.Count; i++) {
						var member = Members[i];
						if(i > 0)
							sb.Append(",  ");
						sb.Append(member.MemberName);
						sb.Append(" = ");
						sb.Append(ReferenceEquals(member.Property, null) ? "null" : member.Property.ToString());
					}
				}
				sb.Append(closingString);
				return sb.ToString();
			}
			return Members == null || Members.Count == 0 ? "{ Empty }" : (ReferenceEquals(Members[0].Property, null) ? "{ null }" : Members[0].Property.ToString());
		}
	}
	public class XPMemberAssignmentCollection : List<XPMemberAssignment> {
	}
	public class XPMemberAssignment {
		MemberInfo member;
		public MemberInfo GetMember(Type type) {
			if (member == null && type != null)
				member = type.GetMember(memberName)[0];
			if (member == null)
				throw new InvalidOperationException(Res.GetString(Res.ObjectLayer_MemberNotFound, (type == null ? "" : type.Name), memberName));
			return member;
		}
		string memberName;
		[XmlAttribute]
		public string MemberName {
			get { return memberName; }
			set { memberName = value; }
		}
		CriteriaOperator property;
		public CriteriaOperator Property {
			get { return property; }
			set { property = value; }
		}
		public XPMemberAssignment() { }
		public XPMemberAssignment(MemberInfo member, CriteriaOperator property) {
			this.member = member;
			memberName = member.Name;
			this.property = property;
		}
		public XPMemberAssignment(XPMemberAssignment source, CriteriaOperator property) {
			this.member = source.member;
			this.memberName = source.memberName;
			this.Property = property;
		}
		public XPMemberAssignment(CriteriaOperator property) {
			this.property = property;
		}
	}
	public class QueryProviderEx {
		public static IQueryable CreateQuery<T>(IQueryProvider provider, Expression e) {
			return provider.CreateQuery<T>(e);
		}
		public delegate IQueryable CreateQueryHandler(IQueryProvider provider, Expression e);
	}
	public class InOperatorCompiler : InOperator {
		OperandValue e;
		XPDictionary dictionary;
		Func<CriteriaOperatorCollection> expression;
		public InOperatorCompiler() { expression = delegate { return GetBaseOperands(); }; }
		CriteriaOperatorCollection GetBaseOperands() {
			return base.Operands;
		}
		public InOperatorCompiler(XPDictionary dictionary, CriteriaOperator leftOperand, OperandValue expression)
			: base(leftOperand) {
			e = expression;
			this.dictionary = dictionary;
		}
		public override CriteriaOperatorCollection Operands {
			get {
				if (expression == null) {
					expression = delegate {
						CriteriaOperatorCollection ops = new CriteriaOperatorCollection();
						foreach (object value in (IEnumerable)e.Value) {
							ops.Add(new RefCompiler(dictionary, value));
						}
						return ops;
					};
				}
				return expression();
			}
		}
	}
	public class RefCompiler : OperandValue {
		XPDictionary dictionary;
		public RefCompiler() {
		}
		public RefCompiler(XPDictionary dictionary) {
			this.dictionary = dictionary;
		}
		public RefCompiler(XPDictionary dictionary, object value)
			: base(value) {
			this.dictionary = dictionary;
		}
		protected override object GetXmlValue() {
			object value = Value;
			if (dictionary != null) {
				XPClassInfo ci = dictionary.QueryClassInfo(value);
				if (ci != null && ci.KeyProperty != null) {
					return ci.GetId(value);
				}
			}
			return value;
		}
	}
	public class ParameterOperandValue : OperandValue {
		Func<object, object> getter;
		public Func<object, object> Getter { get { return getter; } }
		public object BaseValue { get { return base.Value; } }
		public ParameterOperandValue() { getter = (o) => BaseValue; }
		public ParameterOperandValue(object value) : this(value, v => v) {
		}
		public ParameterOperandValue(object value, Func<object, object> getter)
			: base(value) {
			this.getter = getter;
		}
		public override object Value {
			get {
				return getter(base.Value);
			}
		}
	}
	public class MemeberAccessOperator : RefCompiler {
		static Dictionary<MemberInfo, Func<object, object>> compiledExpressions = new Dictionary<MemberInfo,Func<object,object>>();
		MemberExpression e;
		Func<object, object> expression;
		public MemeberAccessOperator(MemberExpression expression) {
			e = expression;
		}
		public MemeberAccessOperator() { expression = GetBaseValue; }
		object GetBaseValue(object value) {
			return base.Value;
		}
		public override object Value {
			get {
				if(expression == null)
					expression = GetExpression(e.Member, e.Expression.Type);
				return expression(((ConstantExpression)e.Expression).Value);
			}
		}
		public static Func<object, object> GetExpression(MemberInfo member, Type t) {
			lock(compiledExpressions) {
				Func<object, object> expression;
				if(!compiledExpressions.TryGetValue(member, out expression)) {
					var param = Expression.Parameter(typeof(object), "value");
					Expression obj = t.IsValueType ? Expression.Convert(param, member.DeclaringType) : Expression.TypeAs(param, member.DeclaringType);
					Expression<Func<object, object>> l = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.MakeMemberAccess(obj, member), typeof(object)), param);
					expression = l.Compile();
					compiledExpressions.Add(member, expression);
				}
				return expression;
			}
		}
	}
	public class ConstantCompiler : RefCompiler {
		Expression e;
		public Expression Expression {
			get { return e; }
		}
		Func<object> expression;
		public ConstantCompiler() { expression = GetBaseValue; }
		object GetBaseValue() {
			return base.Value;
		}
		public ConstantCompiler(XPDictionary dictionary, Expression expression)
			: base(dictionary) {
			e = expression;
		}
		public override object Value {
			get {
				if (expression == null) {
					Expression<Func<object>> l = Expression.Lambda<Func<object>>(Expression.Convert(e, typeof(object)));
					expression = l.Compile();
				}
				return expression();
			}
		}
	}
	[XmlInclude(typeof(MemeberAccessOperator))]
	[XmlInclude(typeof(ConstantCompiler))]
	[XmlInclude(typeof(ConstantValue))]
	[XmlInclude(typeof(QuerySet))]
	[XmlInclude(typeof(FreeQuerySet))]
	[XmlInclude(typeof(GroupSet))]
	[XmlInclude(typeof(ParameterOperandValue))]
	[XmlInclude(typeof(InOperatorCompiler))]
	public class XPQueryData {
		CriteriaOperator criteria;
		public CriteriaOperator Criteria {
			get { return criteria; }
			set { criteria = value; }
		}
		CriteriaOperator groupKey;
		public CriteriaOperator GroupKey {
			get { return groupKey; }
			set { groupKey = value; }
		}
		CriteriaOperator groupCriteria;
		public CriteriaOperator GroupCriteria {
			get { return groupCriteria; }
			set { groupCriteria = value; }
		}
		MemberInitOperator projection;
		public MemberInitOperator Projection {
			get { return projection; }
			set { projection = value; }
		}
		SortingCollection sorting;
		public SortingCollection Sorting {
			get { return sorting; }
			set { sorting = value; }
		}
		string objectTypeName;
		[XmlAttribute]
		public string ObjectTypeName {
			get { return objectTypeName; }
			set { objectTypeName = value; }
		}
		int? top;
		[XmlIgnore]
		public int? Top {
			get { return top; }
			set { top = value; }
		}
		int? skip;
		[XmlIgnore]
		public int? Skip {
			get { return skip; }
			set { skip = value; }
		}
		[XmlAttribute]
		public string TopValue {
			get { return top.HasValue ? top.Value.ToString(CultureInfo.InvariantCulture) : String.Empty; }
			set { top = string.IsNullOrEmpty(value) ? (int?)null : int.Parse(value); }
		}
		[XmlAttribute]
		public string SkipValue {
			get { return skip.HasValue ? skip.Value.ToString(CultureInfo.InvariantCulture) : String.Empty; }
			set { skip = string.IsNullOrEmpty(value) ? (int?)null : int.Parse(value); }
		}
		bool inTransaction;
		[XmlAttribute]
		public bool InTransaction {
			get { return inTransaction; }
			set { inTransaction = value; }
		}
		HashSet<CriteriaOperator> existingJoins;
		public HashSet<CriteriaOperator> ExistingJoins {
			get { return existingJoins; }
			set { existingJoins = value; }
		}
	}
	class DownLevelReprocessor: IClientCriteriaVisitor<CriteriaOperator> {
		Dictionary<JoinOperandInfo, bool> joinOperandList;
		public Dictionary<JoinOperandInfo, bool> JoinOperandList {
			get { return joinOperandList; }
		}
		public static CriteriaOperator Reprocess(CriteriaOperator criteria, out Dictionary<JoinOperandInfo, bool> joinOperandList) {
			DownLevelReprocessor processor = new DownLevelReprocessor();
			CriteriaOperator result = processor.Process(criteria);
			joinOperandList = processor.JoinOperandList;
			return result;
		}
		public CriteriaOperator Process(CriteriaOperator criteria) {
			if(ReferenceEquals(criteria, null)) 
				return null;
			return criteria.Accept(this);
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(JoinOperand theOperand) {
			if(joinOperandList == null) joinOperandList = new Dictionary<JoinOperandInfo, bool>();
			joinOperandList[new JoinOperandInfo(theOperand)] = true;
			return Process(theOperand.AggregatedExpression);
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(AggregateOperand theOperand) {
			throw new NotImplementedException();
		}
		CriteriaOperator IClientCriteriaVisitor<CriteriaOperator>.Visit(OperandProperty theOperand) {
			return new OperandProperty("^." + theOperand.PropertyName);
		}
		void ProcessOperands(CriteriaOperatorCollection newCollection, CriteriaOperatorCollection oldCollection) {
			int count = oldCollection.Count;
			for(int i = 0; i < count; i++) {
				newCollection.Add(Process(oldCollection[i]));
			}
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(FunctionOperator theOperator) {
			FunctionOperator result = new FunctionOperator(theOperator.OperatorType);
			ProcessOperands(result.Operands, theOperator.Operands);
			return result;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(OperandValue theOperand) {
			return theOperand is ConstantValue ? new ConstantValue(theOperand.Value) : new OperandValue(theOperand.Value);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(GroupOperator theOperator) {
			GroupOperator result = new GroupOperator(theOperator.OperatorType);
			ProcessOperands(result.Operands, theOperator.Operands);
			return result;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(InOperator theOperator) {
			InOperator result = new InOperator(Process(theOperator.LeftOperand));
			ProcessOperands(result.Operands, theOperator.Operands);
			return result;
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(UnaryOperator theOperator) {
			return new UnaryOperator(theOperator.OperatorType, Process(theOperator.Operand));
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BinaryOperator theOperator) {
			return new BinaryOperator(Process(theOperator.LeftOperand), Process(theOperator.RightOperand), theOperator.OperatorType);
		}
		CriteriaOperator ICriteriaVisitor<CriteriaOperator>.Visit(BetweenOperator theOperator) {
			return new BetweenOperator(Process(theOperator.TestExpression), Process(theOperator.BeginExpression), Process(theOperator.EndExpression));
		}
	}
	class ParamExpression {
		string[] names;
		object[] values;
		bool used;
		public ParamExpression(string[] names, object[] values) {
			this.names = names;
			this.values = values;
		}
		public bool Use() {
			bool prev = used;
			used = false;
			return prev;
		}
		public bool UnUse(bool value) {
			bool prev = used;
			if(value)
				used = value;
			return prev;
		}
		public void SetUsed() {
			used = true;
		}
		public bool MemeberAccessOperator(MemberExpression expression, out object value) {
			int index = Array.IndexOf<string>(names, expression.Member.Name);
			if(index < 0) {
				value = null;
				return false;
			} else {
				value = values[index];
				SetUsed();
				return true;
			}
		}
	}
	abstract class CachedQueryBase {
		MethodCallExpression nextExpression;
		protected CachedQueryBase next;
		readonly string[] names;
		MethodInfo[] converters;
		Expression[] arguments;
		class MemberAccessChecker : ExpressionVisitor {
			string[] names;
			bool memberAccessed;
			MemberAccessChecker(string[] names) {
				this.names = names;
			}
			protected override Expression VisitMember(MemberExpression node) {
				if(Array.IndexOf<string>(names, node.Member.Name) >= 0)
					memberAccessed = true;
				return base.VisitMember(node);
			}
			public static bool HasParameters(Expression e, string[] names) {
				MemberAccessChecker v = new MemberAccessChecker(names);
				v.Visit(e);
				return v.memberAccessed;
			}
		}
		static Expression emptyParams = Expression.Constant(new ParamExpression(new string[0], new object[0]));
		protected void SetExpression(Expression expression) {
			nextExpression = (MethodCallExpression)expression;
			converters = new MethodInfo[nextExpression.Arguments.Count];
			for(int i = 1; i < converters.Length; i++)
				converters[i] = mi.MakeGenericMethod(nextExpression.Arguments[i].Type);
			if(!MemberAccessChecker.HasParameters(expression, names)) {
				arguments = new Expression[nextExpression.Arguments.Count];
				for(int i = 1; i < arguments.Length; i++)
					arguments[i] = Expression.Call(converters[i], nextExpression.Arguments[i], emptyParams);
			}
		}
		static MethodInfo mi;
		static T GetFirst<T>(T value, ParamExpression param) {
			return value;
		}
		static CachedQueryBase() {
			mi = typeof(CachedQueryBase).GetMethod("GetFirst", BindingFlags.Static | BindingFlags.NonPublic);
		}
		public object Process(IQueryable source, params object[] values) {
			return Process(source, Expression.Constant(new ParamExpression(names, values)));
		}
		public object Process(IQueryable source) {
			return Process(source, (Expression)null);
		}
		protected object Process(IQueryable source, Expression values) {
			MethodCallExpression e = nextExpression;
			if(e == null)
				return source;
			Expression[] args = new Expression[e.Arguments.Count];
			args[0] = Expression.Constant(source);
			for(int i = 1; i < args.Length; i++)
				args[i] = arguments == null ? Expression.Call(converters[i], e.Arguments[i], values) : arguments[i];
			e = Expression.Call(e.Method, args);
			if(next != null) {
				return next.Process(source.Provider, e, values);
			}
			return source.Provider.Execute(e);
		}
		protected abstract object Process(IQueryProvider iQueryProvider, MethodCallExpression e, Expression values);
		protected CachedQueryBase(MethodInfo method) {
			ParameterInfo[] parameters = method.GetParameters();
			names = new string[parameters.Length - 1];
			for(int i = 0; i < parameters.Length - 1; i++)
				names[i] = parameters[i + 1].Name;
		}
		protected CachedQueryBase(CachedQueryBase baseQuery) {
			this.names = baseQuery.names;
		}
	}
	sealed class CachedQuery<T> : CachedQueryBase, IOrderedQueryable<T>, IQueryProvider {
		CachedQuery(CachedQueryBase baseQuery)
			: base(baseQuery) {
		}
		public CachedQuery(MethodInfo method)
			: base(method) {
		}
		protected override object Process(IQueryProvider provider, MethodCallExpression e, Expression values) {
			return Process(provider.CreateQuery<T>(e), values);
		}
		public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
			SetExpression(expression);
			CachedQuery<TElement> query = new CachedQuery<TElement>(this);
			next = query;
			return query;
		}
		public TResult Execute<TResult>(Expression expression) {
			SetExpression(expression);
			return default(TResult);
		}
		public Expression Expression {
			get { return Expression.Constant(this); }
		}
		public IQueryProvider Provider {
			get { return this; }
		}
		public IEnumerator<T> GetEnumerator() {
			throw new NotImplementedException();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotImplementedException();
		}
		public Type ElementType {
			get { throw new NotImplementedException(); }
		}
		public IQueryable CreateQuery(Expression expression) {
			throw new NotImplementedException();
		}
		public object Execute(Expression expression) {
			throw new NotImplementedException();
		}
	}
}
namespace DevExpress.Xpo {
	internal class XPQueryPostEvaluatorCore : ExpressionEvaluatorCore, IClientCriteriaVisitor<object> {
		public XPQueryPostEvaluatorCore(bool caseSensitive, EvaluateCustomFunctionHandler customFunctionHandler)
			: base(caseSensitive, customFunctionHandler) {
		}
		object IClientCriteriaVisitor<object>.Visit(OperandProperty theOperand) {
			EvaluatorProperty property = PropertyCache[theOperand];
			return GetContext(property.UpDepth).GetPropertyValue(property);
		}
	}
	internal class XPQueryPostEvaluator : ExpressionEvaluator {
		readonly ExpressionEvaluatorCoreBase evalCore;
		protected override ExpressionEvaluatorCoreBase EvaluatorCore {
			get { return evalCore; }
		}
		public XPQueryPostEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions)
			: base(descriptor, criteria, caseSensitive, false, customFunctions) {
			evalCore = new XPQueryPostEvaluatorCore(caseSensitive, new EvaluateCustomFunctionHandler(EvaluateCustomFunction));
		}
	}
	public abstract class XPQueryBase : IPersistentValueExtractor {
		XPQueryData query;
		Session session;
		IDataLayer layer;
		XPDictionary dictionary;
#if !SL
	[DevExpressXpoLocalizedDescription("XPQueryBaseDictionary")]
#endif
		public XPDictionary Dictionary {
			get { return dictionary; }
		}
		Session GetSession() {
			if(session == null)
				throw new InvalidOperationException(Res.GetString(Res.LinqToXpo_SessionIsNull));
			return session;
		}
		IDataLayer GetLayer() {
			if(session == null)
				throw new InvalidOperationException(Res.GetString(Res.LinqToXpo_SessionIsNull));
			return layer;
		}
#if !SL
	[DevExpressXpoLocalizedDescription("XPQueryBaseSession")]
#endif
		public Session Session {
			get {
				return session;
			}
			set {
				if (dictionary != value.Dictionary)
					throw new ArgumentException(Res.GetString(Res.Helpers_SameDictionaryExpected));
				session = value;
			}
		}
		protected IDataLayer DataLayer {
			get { return layer; }
		}
		CriteriaOperator Criteria {
			get { return query.Criteria; }
			set { query.Criteria = value; }
		}
		internal CriteriaOperator GroupKey {
			get { return query.GroupKey; }
			set { query.GroupKey = value; }
		}
		bool IsGroup {
			get { return !IsNull(query.GroupKey); }
		}
		CriteriaOperator GroupCriteria {
			get { return query.GroupCriteria; }
			set { query.GroupCriteria = value; }
		}
		internal MemberInitOperator Projection {
			get { return query.Projection; }
			set { query.Projection = value; }
		}
		SortingCollection Sorting {
			get { return query.Sorting; }
			set { query.Sorting = value; }
		}
		XPClassInfo objectClassInfo;
		protected XPClassInfo ObjectClassInfo {
			get { return objectClassInfo; }
		}
		int? Top {
			get { return query.Top; }
			set { query.Top = value; }
		}
		int? Skip {
			get { return query.Skip; }
			set { query.Skip = value; }
		}
		bool InTransaction {
			get { return query.InTransaction; }
			set { query.InTransaction = value; }
		}
		HashSet<CriteriaOperator> ExistingJoins {
			get { return query.ExistingJoins; }
			set { query.ExistingJoins = value; }
		}
		CustomCriteriaCollection customCriteriaCollection;
		internal CustomCriteriaCollection CustomCriteriaCollection { get { return customCriteriaCollection; } }
		object IPersistentValueExtractor.ExtractPersistentValue(object criterionValue) {
			if(Dictionary.QueryClassInfo(criterionValue) != null)
				throw new DevExpress.Xpo.Exceptions.CannotResolveClassInfoException((criterionValue == null ? "null" : criterionValue.GetType().AssemblyQualifiedName), (criterionValue == null ? "null" : criterionValue.GetType().FullName));				
			return criterionValue;
		}
		bool IPersistentValueExtractor.CaseSensitive { get { return false; } }
		List<object[]> SessionSelectData(XPClassInfo classInfo, CriteriaOperatorCollection properties, CriteriaOperator criteria, CriteriaOperatorCollection groupProperties, CriteriaOperator groupCriteria, bool selectDeleted, int skipSelectedRecords, int topSelectedRecords, SortingCollection sorting) {
			if(InTransaction) {
				return GetSession().SelectDataInTransaction(classInfo, properties, criteria, groupProperties, groupCriteria, selectDeleted, skipSelectedRecords, topSelectedRecords, sorting);
			}
			if(layer != null) {
				List<object[]> data = Session.PrepareSelectDataInternal(classInfo, ref properties, ref criteria, ref groupProperties, ref groupCriteria, ref sorting, true, this);
				if(data != null)
					return data;
				return SimpleObjectLayer.SelectDataInternal(layer, new ObjectsQuery(classInfo, criteria, sorting, skipSelectedRecords, topSelectedRecords, null, true), properties, groupProperties, groupCriteria);
			}
			return GetSession().SelectData(classInfo, properties, criteria, groupProperties, groupCriteria, selectDeleted, skipSelectedRecords, topSelectedRecords, sorting);
		}
		ICollection SessionGetObjects(XPClassInfo classInfo, CriteriaOperator condition, SortingCollection sorting, int skipSelectedRecords, int topSelectedRecords, bool selectDeleted) {
			if(InTransaction) {
				return GetSession().GetObjectsInTransaction(classInfo, condition, sorting, skipSelectedRecords, topSelectedRecords, selectDeleted);
			}
			return GetSession().GetObjects(classInfo, condition, sorting, skipSelectedRecords, topSelectedRecords, selectDeleted, false);
		}
		void SessionGetObjectsAsync(XPClassInfo classInfo, CriteriaOperator condition, SortingCollection sorting, int skipSelectedRecords, int topSelectedRecords, bool selectDeleted, AsyncLoadObjectsCallback callback) {
			if(InTransaction) {
				GetSession().GetObjectsInTransactionAsync(classInfo, condition, sorting, skipSelectedRecords, topSelectedRecords, selectDeleted, callback);
			} else {
				GetSession().GetObjectsAsync(classInfo, condition, sorting, skipSelectedRecords, topSelectedRecords, selectDeleted, false, callback);
			}
		}
		object SessionEvaluate(XPClassInfo classInfo, CriteriaOperator expression, CriteriaOperator condition) {
			if(InTransaction) {
				return GetSession().EvaluateInTransaction(classInfo, expression, condition);
			}
			if(layer != null) {
				List<object[]> result = Session.PrepareEvaluate(classInfo, ref expression, ref condition, null, false);
				if(result == null) {
					CriteriaOperatorCollection properties = new CriteriaOperatorCollection(1);
					properties.Add(expression);
					CriteriaOperatorCollection groupProperties = null;
					CriteriaOperator groupCriteria = null;
					SortingCollection sorting = null;
					result = Session.PrepareSelectDataInternal(classInfo, ref properties, ref condition, ref groupProperties, ref groupCriteria, ref sorting, true, this);
					if(result == null)
						result = SimpleObjectLayer.SelectDataInternal(layer, new ObjectsQuery(classInfo, condition, sorting, 0, 1, null, true), properties, groupProperties, groupCriteria);
				}
				return (result == null) || (result.Count == 0) || (result[0] == null) || (result[0].Length == 0) ? null : result[0][0];
			}
			return GetSession().Evaluate(classInfo, expression, condition);
		}
		object SessionFindObject(XPClassInfo classInfo, CriteriaOperator criteria) {
			if(InTransaction) {
				return GetSession().FindObject(PersistentCriteriaEvaluationBehavior.InTransaction, ObjectClassInfo, criteria);
			}
			return GetSession().FindObject(ObjectClassInfo, criteria);
		}
		protected XPQueryBase(XPQueryBase baseQuery)
			: this(baseQuery.Session, baseQuery.layer, baseQuery.Dictionary) {
			Assign(baseQuery);
			create = baseQuery.create;
		}
		protected XPQueryBase(XPQueryBase baseQuery, bool inTransaction)
			: this(baseQuery) {
			InTransaction = inTransaction;
		}
		protected XPQueryBase(XPQueryBase baseQuery, CustomCriteriaCollection customCriteriaCollection)
			: this(baseQuery) {
			if(this.customCriteriaCollection == null) this.customCriteriaCollection = customCriteriaCollection;
			else this.customCriteriaCollection.Add(customCriteriaCollection);
		}
		protected XPQueryBase(IDataLayer layer, Type type)
			: this(layer.Dictionary, type, false) {
			this.layer = layer;
		}
		protected XPQueryBase(Session session, Type type, bool inTransaction)
			: this(session.Dictionary, type, inTransaction) {
			this.session = session;
		}
		protected XPQueryBase(XPDictionary dictionary, Type type, bool inTransaction) {
			this.dictionary = dictionary;
			query = new XPQueryData();
			query.InTransaction = inTransaction;
			this.Sorting = new SortingCollection();
			objectClassInfo = Dictionary.GetClassInfo(type);
			this.query.ObjectTypeName = objectClassInfo.FullName;
		}
		protected XPQueryBase(Session session, IDataLayer dataLayer, XPDictionary dictionary) {
			this.session = session;
			this.dictionary = dictionary;
			this.layer = dataLayer;
			query = new XPQueryData();
			this.Sorting = new SortingCollection();
		}
		protected XPQueryBase(Session session, IDataLayer dataLayer, XPDictionary dictionary, string data)
			: this(dictionary, data) {
			this.session = session;
			this.layer = dataLayer;
		}
		protected XPQueryBase(XPDictionary dictionary, string data) {
			StringReader r = new StringReader(data);
			XmlSerializer s = new XmlSerializer(typeof(XPQueryData));
			this.dictionary = dictionary;
			this.query = (XPQueryData)s.Deserialize(r);
			this.objectClassInfo = Dictionary.GetClassInfo("", query.ObjectTypeName);
		}
		internal static CriteriaOperator GetFreeQuerySet(XPQueryBase query) {
			if(query.IsGroup || query.Skip > 0 || query.Top > 0)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0WithSkipOrTakeOrGroupingIsNotSupported, "Subquery"));
			if(!IsNull(query.Projection)) {
				if(query.Projection.CreateNewObject || query.Projection.Members.Count > 1)
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ComplexDataSelectionIsNotSupportedPerhapsY));
				return new FreeQuerySet(query.ObjectClassInfo.ClassType, query.Criteria) {
					Projection = query.Projection
				};
			}
			return new FreeQuerySet(query.ObjectClassInfo.ClassType, query.Criteria);
		}
		public string Serialize() {
			StringWriter w = new StringWriter();
			XmlSerializer s = new XmlSerializer(typeof(XPQueryData));
			s.Serialize(w, query);
			return w.ToString();
		}
		protected internal void Call(MethodCallExpression call, XPQueryBase prev) {
			Assign(prev);
			Call(call);
		}
		protected void Assign(XPQueryBase prev) {
			Criteria = prev.Criteria;
			for (int i = 0; i < prev.Sorting.Count; i++)
				Sorting.Add(prev.Sorting[i]);
			Projection = prev.Projection;
			GroupCriteria = prev.GroupCriteria;
			query.ObjectTypeName = prev.query.ObjectTypeName;
			objectClassInfo = prev.objectClassInfo;
			Top = prev.Top;
			Skip = prev.Skip;
			GroupKey = prev.GroupKey;
			InTransaction = prev.InTransaction;
			ExistingJoins = prev.ExistingJoins == null ? null : new HashSet<CriteriaOperator>(prev.ExistingJoins);
			customCriteriaCollection = prev.CustomCriteriaCollection;
		}
		protected static bool IsNull(object val) {
			return val == null;
		}
		CriteriaOperator ParseExpression(Expression expression, params CriteriaOperator[] maps) {
			return Parser.ParseExpression(ObjectClassInfo, customCriteriaCollection, expression, maps);
		}
		CriteriaOperator ParseObjectExpression(Expression expression, params CriteriaOperator[] maps) {
			return Parser.ParseObjectExpression(ObjectClassInfo, customCriteriaCollection, expression, maps);
		}
		void Call(MethodCallExpression call) {
			switch(call.Method.Name) {
				case "Where":
					Where(call);
					break;
				case "OrderBy":
				case "ThenBy":
					Order(call);
					break;
				case "OrderByDescending":
				case "ThenByDescending":
					OrderDescending(call);
					break;
				case "Reverse":
					Reverse();
					break;
				case "Select":
					Select(call);
					break;
				case "Distinct":
					Distinct(call);
					break;
				case "GroupBy":
					GroupBy(call);
					break;
				case "Take":
					Take(call);
					break;
				case "Skip":
					SkipFn(call);
					break;
				case "Join":
					Join(call, false);
					break;
				case "GroupJoin":
					Join(call, true);
					break;
				case "OfType":
					OfType(call);
					break;
				case "SelectMany":
					SelectMany(call);
					break;
				case "Intersect":
					Intersect(call);
					break;
				case "Union":
					Union(call);
					break;
				case "SkipWhile":
				case "TakeWhile":
				case "DefaultIfEmpty":
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheX0MethodIsNotSupported, call.Method.Name));
				case "Cast":
					break;
				default:
					throw new ArgumentException(call.Method.Name);
			}
		}
		protected bool CanIntersect() {
			return !Top.HasValue && !Skip.HasValue && Sorting.Count == 0 && IsNull(Projection) && !IsGroup;
		}
		void Union(MethodCallExpression call) {
			XPQueryBase next = (XPQueryBase)((ConstantExpression)call.Arguments[1]).Value;
			if (IsNull(Criteria) || IsNull(next.Criteria))
				Criteria = null;
			else
				Criteria = GroupOperator.Or(Criteria, next.Criteria);
		}
		void Intersect(MethodCallExpression call) {
			XPQueryBase next = (XPQueryBase)((ConstantExpression)call.Arguments[1]).Value;
			Criteria = GroupOperator.And(Criteria, next.Criteria);
		}
		void SelectMany(MethodCallExpression call) {
			if(IsGroup || Top.HasValue || Skip.HasValue)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0WithSkipOrTakeOrGroupingIsNotSupported, "SelectMany"));
			if(call.Arguments.Count != 2)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0WithSoManyParametersIsNotSupported, "SelectMany"));
			QuerySet set = ParseExpression(call.Arguments[1], ParseExpression(call.Arguments[0])) as QuerySet;
			if(IsNull(set))
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[1], "SelectMany"));
			OperandProperty prop = set.Property as OperandProperty;
			if(IsNull(set.Property))
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[1], "SelectMany"));
			MemberInfoCollection col = MemberInfoCollection.ParsePersistentPath(ObjectClassInfo, prop.PropertyName);
			if(col.Count != 1 || !col[0].IsAssociationList)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[1], "SelectMany"));
			XPClassInfo firstClassInfo = objectClassInfo;
			objectClassInfo = col[0].CollectionElementType;
			XPMemberInfo associatedMember = col[0].GetAssociatedMember();
			string refMember = associatedMember.Name;
			bool useUpCasting = associatedMember.ReferenceType != null && firstClassInfo != associatedMember.ReferenceType;
			if(col[0].IsManyToManyAlias
#if !SL
 || col[0].IsManyToMany
#endif
) {
				Criteria = CriteriaOperator.And(set.Condition, new AggregateOperand(refMember, Aggregate.Exists, Criteria));
				Projection = set.Projection;
				if(Sorting != null && Sorting.Count > 0)
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_SortingIsNotSupportedForSelectManyOperatio));
			} else {
				string refMemberForPatch = useUpCasting ? string.Format("{0}.<{1}>", refMember, firstClassInfo.FullName) : refMember;
				Criteria = CriteriaOperator.And(set.Condition, PatchParentCriteria(Criteria, refMemberForPatch), new NotOperator(new NullOperator(refMember)));
				Projection = set.Projection;
				if(Sorting != null) {
					foreach(SortProperty s in Sorting)
						s.Property = PatchParentCriteria(s.Property, refMemberForPatch);
				}
			}
		}
		CriteriaOperator PatchParentCriteria(CriteriaOperator criteria, string p) {
			return PersistentCriterionExpander.Prefix(p, criteria);
		}
		void OfType(MethodCallExpression call) {
			Type type = call.Method.GetGenericArguments()[0];
			XPClassInfo newObjectClassInfo = Dictionary.GetClassInfo(type);
			if (!newObjectClassInfo.IsAssignableTo(objectClassInfo))
				throw new InvalidOperationException(); 
			objectClassInfo = newObjectClassInfo;
			this.query.ObjectTypeName = objectClassInfo.FullName;
		}
		void Join(MethodCallExpression call, bool groupJoin) {
			CriteriaOperator p = ParseExpression(call.Arguments[2], Projection);
			XPQueryBase related = ((ConstantExpression)call.Arguments[1]).Value as XPQueryBase;
			if((related.Top.HasValue && related.Top.Value > 0) || (related.Skip.HasValue && related.Skip.Value > 0)) {
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_QueryWithAppliedTheSkipOrTheTakeOperations));
			}
			Type projType = related.ObjectClassInfo.ClassType;
			CriteriaOperator relatedP = related.ParseExpression(call.Arguments[3], related.Projection);
			if(IsNull(p)) {
				OperandProperty prop = relatedP as OperandProperty;
				if(IsNull(prop))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[3], "Join")); 
				XPMemberInfo member = Dictionary.GetClassInfo(projType).FindMember(prop.PropertyName);
				if(member == null)
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[3], "Join")); 
				MemberInitOperator current = (MemberInitOperator)ParseExpression(call.Arguments[4], Projection,
					new QuerySet(new OperandProperty(member.GetAssociatedMember().Name), related.Criteria));
				Projection = current;
			} else {
				var freeQuery = new FreeQuerySet(projType, p, relatedP, related.Criteria);
				freeQuery.Projection = related.Projection;
				CriteriaOperator co = ParseExpression(call.Arguments[4], Projection, freeQuery);
				MemberInitOperator current = co as MemberInitOperator;
				if(ReferenceEquals(current, null)) {
					QuerySet qs = co as QuerySet;
					if(ReferenceEquals(qs, null)) {
						XPMemberAssignmentCollection members = new XPMemberAssignmentCollection();
						members.Add(new XPMemberAssignment(co));
						Projection = new MemberInitOperator((Type)null, members, false);
					}else{
						FreeQuerySet fqs = qs as FreeQuerySet;
						if(ReferenceEquals(fqs, null)) {
							if(!IsNull(qs.Property))
								throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[4], "Join")); 
							Projection = qs.Projection;
						} else {
							XPMemberAssignmentCollection members = new XPMemberAssignmentCollection();
							members.Add(new XPMemberAssignment(fqs));
							Projection = new MemberInitOperator((Type)null, members, false);
							if(!groupJoin) CheckJoinExists(members[0].Property);
						}
					}
				}else{
					Projection = current;
					if(!IsNull(Projection) && Projection.Members.Count == 2) {
						if(!groupJoin) CheckJoinExists(Projection.Members[1].Property);
					}
				}
			}
		}
		void CheckJoinExists(CriteriaOperator operand) {
			FreeQuerySet freeSet = operand as FreeQuerySet;
			if(!IsNull(freeSet))
				operand = freeSet.CreateJoinOperand(freeSet.Condition, new OperandProperty("This"), Aggregate.Single);
			JoinOperand joinOperand = operand as JoinOperand;
			if(IsNull(joinOperand)) return;
			if(ExistingJoins == null)
				ExistingJoins = new HashSet<CriteriaOperator>();
			if(ExistingJoins.Contains(joinOperand))
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_DuplicateJoinOperatorFound));
			ExistingJoins.Add(joinOperand);
		}
		void Take(MethodCallExpression call) {
			int top = GetSafeInt(((OperandValue)ParseExpression(call.Arguments[1])).Value);
			if (!Top.HasValue || top < Top)
				Top = top;
		}
		void SkipFn(MethodCallExpression call) {
			int skip = GetSafeInt(((OperandValue)ParseExpression(call.Arguments[1])).Value);
			if(!Skip.HasValue || skip < Skip)
				Skip = skip;
		}
		void GroupBy(MethodCallExpression call) {
			if(Top.HasValue || Skip.HasValue)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OperatorAfterSkipOrTakeIsNotSupported, "GroupBy operator"));
			foreach (Expression arg in call.Arguments) {
				if (arg.Type.GetGenericTypeDefinition() == typeof(IEqualityComparer<>) || arg.Type == typeof(IEqualityComparer))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_GroupingWithACustomComparerIsNotSupported));
			}
			XPMemberAssignmentCollection list = new XPMemberAssignmentCollection();
			Type[] args = call.Method.GetGenericArguments();
			Type type = typeof(IGrouping<,>).MakeGenericType(args[1], args[0]);
			CriteriaOperator op = ParseExpression(call.Arguments[1], Projection);
			if (call.Arguments.Count == 4) {
				CriteriaOperator key = ParseExpression(call.Arguments[2], Projection);
				if(key is QuerySet && ((QuerySet)key).IsEmpty) {
					key = null;
				}
				Projection = (MemberInitOperator)ParseExpression(call.Arguments[3], op, new QuerySet((OperandProperty)key, null));
			}
			if (!(op is OperandValue))
				GroupKey = op;
			Sorting.Clear(); 
		}
		void Distinct(MethodCallExpression call) {
			if (Top.HasValue || Skip.HasValue)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OperatorAfterSkipOrTakeIsNotSupported, "Distinct operator"));
			if (call.Arguments.Count != 1)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0WithSoManyParametersIsNotSupported, "Distinct operator"));
			if (!IsNull(Projection)) {
				GroupKey = Projection;
				Sorting.Clear(); 
			}
		}
		void Select(MethodCallExpression call) {
			QuerySet set = (QuerySet)ParseExpression(call);
			FreeQuerySet freeQuerySet = set as FreeQuerySet;
			if(!ReferenceEquals(freeQuerySet, null) && ReferenceEquals(freeQuerySet.Projection, null)){
				Projection = freeQuerySet.CreateSingleUntypedMemberInitOperator();
			}else{
				Projection = set.Projection;
			}
		}
		void Reverse() {
			if (Sorting.Count > 0) {
				if (Top.HasValue || Skip.HasValue)
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OperatorAfterSkipOrTakeIsNotSupported, "Reverse operator"));
				for (int i = 0; i < Sorting.Count; i++)
					Sorting[i].Direction = Sorting[i].Direction == DevExpress.Xpo.DB.SortingDirection.Ascending ? DevExpress.Xpo.DB.SortingDirection.Descending : DevExpress.Xpo.DB.SortingDirection.Ascending;
			}
		}
		void OrderDescending(MethodCallExpression call) {
			if(call.Arguments.Count > 2)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0WithSoManyParametersIsNotSupported, "OrderBy operator"));
			if (Top.HasValue || Skip.HasValue)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OperatorAfterSkipOrTakeIsNotSupported, "OrderBy operator"));
			Sorting.Add(new SortProperty(ParseObjectExpression(call.Arguments[1], ParseExpression(call.Arguments[0])), DevExpress.Xpo.DB.SortingDirection.Descending));
		}
		void Order(MethodCallExpression call) {
			if (call.Arguments.Count > 2)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0WithSoManyParametersIsNotSupported, "OrderBy operator"));
			if (Top.HasValue || Skip.HasValue)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OperatorAfterSkipOrTakeIsNotSupported, "OrderBy operator"));
			Sorting.Add(new SortProperty(ParseObjectExpression(call.Arguments[1], ParseExpression(call.Arguments[0])), DevExpress.Xpo.DB.SortingDirection.Ascending));
		}
		void Where(MethodCallExpression call) {
			if(Top.HasValue || Skip.HasValue)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OperatorAfterSkipOrTakeIsNotSupported, "Where operator"));
			QuerySet val = (QuerySet)ParseExpression(call);
			if (!IsGroup)
				Criteria = GroupOperator.And(Criteria, val.Condition);
			else
				GroupCriteria = GroupOperator.And(GroupCriteria, val.Condition);
		}
		protected object Execute(Expression expression) {
			if (expression.NodeType == ExpressionType.Call) {
				MethodCallExpression call = (MethodCallExpression)expression;
				if (call.Method.DeclaringType != typeof(Queryable) && call.Method.DeclaringType != typeof(Enumerable))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_IncorrectDeclaringTypeX0InTheMethodCallQue, call.Method.DeclaringType));
				switch (call.Method.Name) {
					case "Count": return Count(call);
					case "LongCount": return LongCount(call);
					case "Min":
					case "Max":
					case "Sum": return AggregateCall(call);
					case "Contains": return Contains(call);
					case "Average": return Average(call);
					case "FirstOrDefault":
						return ExecuteSingle(call, true, SortAction.Sort);
					case "First":
						return ExecuteSingle(call, false, SortAction.Sort);
					case "Single":
						return ExecuteSingle(call, false, SortAction.None);
					case "SingleOrDefault":
						return ExecuteSingle(call, true, SortAction.None);
					case "Last":
						return ExecuteSingle(call, false, SortAction.Reverse);
					case "LastOrDefault":
						return ExecuteSingle(call, true, SortAction.Reverse);
					case "All": return All(call);
					case "Any": return Any(call);
					case "Aggregate":
					case "ElementAt":
					case "ElementAtOrDefault":
					case "SequenceEqual":
						throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheX0MethodIsNotSupported, call.Method.Name));
					default:
						throw new ArgumentException(call.Method.Name);
				}
			}
			throw new NotSupportedException();
		}
		bool Contains(MethodCallExpression call) {
			if (Top.HasValue || Skip.HasValue)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OperatorAfterSkipOrTakeIsNotSupported, "Contains operator"));
			if(IsNull(Projection)) {
				return (bool)SessionEvaluate(ObjectClassInfo,
					new BinaryOperator(new AggregateOperand(null, Aggregate.Count), new ConstantValue(0), BinaryOperatorType.Greater),
					GroupOperator.And(Criteria,
					new BinaryOperator(new OperandProperty(ObjectClassInfo.KeyProperty.Name), ParseExpression(call.Arguments[1]), BinaryOperatorType.Equal))) == true;
			} else {
				Type type = call.Arguments[1].Type;
				MemberInitOperator init;
				if(IsGroup) {
					init = GroupKey as MemberInitOperator;
				} else
					init = Projection as MemberInitOperator;
				CriteriaOperator criteria;
				if(IsNull(init))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_CurrentExpressionWithX0IsNotSupported, "Contains operator")); 
				if(init.GetConstructor(type) == null) {
					if(!type.IsArray)
						criteria = new BinaryOperator(init.Members[0].Property, new ConstantCompiler(Dictionary, call.Arguments[1]), BinaryOperatorType.Equal);
					else {
						CriteriaOperator[] ops = new CriteriaOperator[init.Members.Count];
						for(int i = 0; i < ops.Length; i++)
							ops[i] = new BinaryOperator(init.Members[i].Property, new ConstantCompiler(Dictionary, Expression.ArrayIndex(call.Arguments[1], Expression.Constant(i))), BinaryOperatorType.Equal);
						criteria = GroupOperator.And(ops);
					}
				} else {
					if(init.UseConstructor) {
						CriteriaOperator[] ops = new CriteriaOperator[init.Members.Count];
						for(int i = 0; i < ops.Length; i++) {
							MemberInfo m = init.Members[i].GetMember(type);
							MethodInfo mi = m is MethodInfo ? (MethodInfo)m : ((PropertyInfo)m).GetGetMethod();
							ops[i] = new BinaryOperator(init.Members[i].Property, new ConstantCompiler(Dictionary, Expression.Call(call.Arguments[1], mi)), BinaryOperatorType.Equal);
						}
						criteria = GroupOperator.And(ops);
					} else
						throw new NotSupportedException(Res.GetString(Res.LinqToXpo_CurrentExpressionWithX0IsNotSupported, "Contains operator")); 
				}
				if(IsGroup) {
					CriteriaOperatorCollection props = new CriteriaOperatorCollection();
					props.Add(new ConstantValue(1));
					List<object[]> data = SessionSelectData(ObjectClassInfo, props, Criteria, GetGrouping(), GroupOperator.And(GroupCriteria, criteria), false, 0, 1, null);
					return data.Count > 0;
				} else
					return (bool)SessionEvaluate(ObjectClassInfo,
						new BinaryOperator(new AggregateOperand(null, Aggregate.Count), new ConstantValue(0), BinaryOperatorType.Greater),
						GroupOperator.And(Criteria, criteria)) == true;
			}
		}
		long LongCount(MethodCallExpression call) {
			return Convert.ToInt64(CountCore(call));
		}
		object CountCore(MethodCallExpression call) {
			if (call.Arguments.Count > 2)
				throw new ArgumentException(Res.GetString(Res.LinqToXpo_X0WithSoManyParametersIsNotSupported, "Count operator"));
			if (ZeroTop)
				return 0;
			if(IsGroup) {
				CriteriaOperatorCollection props = new CriteriaOperatorCollection();
				props.Add(new ConstantValue(1));
				AggregateOperand ag = (AggregateOperand)ParseExpression(call);
				CriteriaOperator criteria = Criteria;
				if(!IsNull(ag.Condition)) {
					if(Top.HasValue || Skip.HasValue)
						throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OperatorAfterSkipOrTakeIsNotSupported, "Count operator with condition"));
					criteria = GroupOperator.And(Criteria, ag.Condition);
				}
				return SessionSelectData(ObjectClassInfo, props, criteria, GetGrouping(), GroupCriteria, false,
					Skip.HasValue ? Skip.Value : 0, Top.HasValue ? Top.Value : 0, null).Count;
			} else {
				AggregateOperand ag = (AggregateOperand)ParseExpression(call);
				CriteriaOperator criteria = Criteria;
				if(!IsNull(ag.Condition)) {
					if(Top.HasValue || Skip.HasValue)
						throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OperatorAfterSkipOrTakeIsNotSupported, "Count operator with condition"));
					criteria = GroupOperator.And(Criteria, ag.Condition);
					ag.Condition = null;
				}
				int count = GetSafeInt(SessionEvaluate(ObjectClassInfo, ag, criteria));
				int result = count;
				if(Top.HasValue) {
					result = Top.Value;
				} else if(Skip.HasValue) {
					result = count - Skip.Value;
				}
				return result;
			}
		}
		static int GetSafeInt(object obj) {
			return obj == null ? 0 : (int)obj;
		}
		bool ZeroTop {
			get { return Top.HasValue && Top.Value <= 0; }
		}
		int Count(MethodCallExpression call) {
			return Convert.ToInt32(CountCore(call));
		}
		object AggregateCall(MethodCallExpression call) {
			if (IsGroup)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OverGroupingIsNotSupported, call.Method.Name));
			if (Top.HasValue || Skip.HasValue)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OperatorAfterSkipOrTakeIsNotSupported, call.Method.Name));
			return SessionEvaluate(ObjectClassInfo, ParseExpression(call), Criteria);
		}
		object Average(MethodCallExpression call) {
			object res = AggregateCall(call);
			IConvertible conv = res as IConvertible;
			if (res != null && conv == null && res.GetType() != call.Type)
				throw new InvalidOperationException();
			return conv != null ? conv.ToType(call.Type, CultureInfo.InvariantCulture) : res;
		}
		bool All(MethodCallExpression call) {
			if (IsGroup)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OverGroupingIsNotSupported, "All operator"));
			CriteriaOperator val = GroupOperator.And(Criteria, new NotOperator(ParseExpression(call.Arguments[1], Projection)));
			return GetSafeInt(SessionEvaluate(ObjectClassInfo, new AggregateOperand(String.Empty, Aggregate.Count), val)) == 0;
		}
		bool Any(MethodCallExpression call) {
			if (IsGroup)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0OverGroupingIsNotSupported, "Any operator"));
			CriteriaOperator val;
			if (call.Arguments.Count > 1) {
				val = GroupOperator.And(Criteria, ParseExpression(call.Arguments[1], Projection));
			} else
				val = Criteria;
			return GetSafeInt(SessionEvaluate(ObjectClassInfo, new AggregateOperand(String.Empty, Aggregate.Count), val)) > 0;
		}
		enum SortAction { None, Sort, Reverse };
		object ExecuteSingle(MethodCallExpression call, bool allowDefault, SortAction sort) {
			bool single = sort == SortAction.None;
			CriteriaOperator val;
			if (call.Arguments.Count > 1) {
				val = ParseExpression(call.Arguments[1], Projection);
			} else
				val = null;
			return IsNull(Projection) && !IsGroup ? GetSingleObject(val, allowDefault, sort, single) : GetSingleData(call.Type, val, allowDefault, sort, single);
		}
		CriteriaOperatorCollection GetGrouping() {
			if (IsNull(GroupKey))
				return null;
			CriteriaOperatorCollection groupProperties = new CriteriaOperatorCollection();
			MemberInitOperator init = GroupKey as MemberInitOperator;
			if (!IsNull(init))
				foreach (XPMemberAssignment m in init.Members)
					groupProperties.Add(m.Property);
			else
				groupProperties.Add(GroupKey);
			return groupProperties;
		}
		object GetSingleData(Type type, CriteriaOperator val, bool allowDefault, SortAction sort, bool single) {
			val = GroupOperator.And(Criteria, val);
			if(IsNull(Projection)) {
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_CurrentExpressionWithX0IsNotSupported, "Single operator"));
			}
			if(Sorting == null && Skip.HasValue){
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_SkipOperationIsNotSupportedWithoutSorting));
			}
			CriteriaOperatorCollection props = new CriteriaOperatorCollection();
			CriteriaOperatorCollection correspondingProps = new CriteriaOperatorCollection();
			MemberInitOperator last = Projection;			
			foreach (XPMemberAssignment mi in last.Members) {
				ProcessAssignmentMember(mi.Property, props, correspondingProps);
			}
			SortingCollection sortCol;
			if (sort == SortAction.Reverse) {
				sortCol = new SortingCollection();
				for (int i = 0; i < Sorting.Count; i++)
					sortCol.Add(new SortProperty(Sorting[i].Property, Sorting[i].Direction == DevExpress.Xpo.DB.SortingDirection.Ascending ? DevExpress.Xpo.DB.SortingDirection.Descending : DevExpress.Xpo.DB.SortingDirection.Ascending));
			} else
				sortCol = Sorting;
			if (ZeroTop) {
				if (!allowDefault)
					throw new InvalidOperationException(Res.GetString(Res.LinqToXpo_SequenceContainsNoMatchingElement));
				else
					return null;
			}
			List<object[]> data = SessionSelectData(ObjectClassInfo, props, val, GetGrouping(), GroupCriteria, false,
					Skip.HasValue ? Skip.Value : 0, single && Top != 1 ? 2 : 1, sortCol);
			if (data.Count == 0) {
				if (!allowDefault)
					throw new InvalidOperationException(Res.GetString(Res.LinqToXpo_SequenceContainsNoMatchingElement));
				else
					return null;
			}
			if (single && data.Count > 1)
				throw new InvalidOperationException(Res.GetString(Res.LinqToXpo_SequenceContainsMoreThanOneElement));
			DataPostProcessing(correspondingProps, data, type);
			return CreateItem(type, last)(data[0]);
		}
		struct PostloadGroupCacheItem {
			public GroupSet Group;
			public Type GroupType;
			public PostloadGroupCacheItem(GroupSet group, Type groupType) {
				Group = group;
				GroupType = groupType;
			}
		}
		void DataPostProcessing(CriteriaOperatorCollection props, List<object[]> data, Type type) {
			int sourcePos = 0;
			Dictionary<PostloadGroupCacheItem, int> groupCache = new Dictionary<PostloadGroupCacheItem, int>();
			for(int i = 0; i < props.Count; i++, sourcePos++) {
				GroupSet group = props[i] as GroupSet;
				if (!IsNull(group)) {
					CriteriaOperatorCollection groupProperties = GetGrouping();
					CreateItemDelegate createGroup;
					Type groupType = null;
					if(!(type.IsGenericType && (groupType = type.GetGenericArguments()[1]).IsGenericType)) {
						Type resolveType = groupType ?? type;
						XPClassInfo typeCI = Dictionary.GetClassInfo(resolveType);
						if(typeCI == null) throw new DevExpress.Xpo.Exceptions.CannotResolveClassInfoException(resolveType.AssemblyQualifiedName, resolveType.FullName);
						Type keyType = new Resolver(typeCI).Resolve(GroupKey);
						Type elementType = groupType ?? type;
						groupType = typeof(IGrouping<,>).MakeGenericType(keyType, elementType);						
					}
					int cachedSourcePos;
					PostloadGroupCacheItem gcItem = new PostloadGroupCacheItem(group, groupType);
					if(!groupCache.TryGetValue(gcItem, out cachedSourcePos)) {
						createGroup = CreateGroupItemCore(groupType, null);
						for(int j = 0; j < data.Count; j++) {
							object[] groupKey = new object[groupProperties.Count];
							Array.Copy(data[j], sourcePos, groupKey, 0, groupKey.Length);
							data[j][i] = createGroup(groupKey);
						}
						groupCache.Add(gcItem, sourcePos);
					} else {
						for(int j = 0; j < data.Count; j++) {
							data[j][i] = data[j][cachedSourcePos];
						}
					}
					sourcePos += groupProperties.Count - 1;
					continue;
				}
				XPClassInfo currentClassInfo = ObjectClassInfo;
				CriteriaOperator currentProperty = props[i];
				JoinOperand joinOperand = currentProperty as JoinOperand;
				while(!IsNull(joinOperand) && joinOperand.AggregateType == Aggregate.Single) {
					XPClassInfo joinClassInfo = null;
					if(!MemberInfoCollection.TryResolveTypeAlsoByShortName(joinOperand.JoinTypeName, currentClassInfo, out joinClassInfo)) {
						throw new DevExpress.Xpo.Exceptions.CannotResolveClassInfoException(string.Empty, joinOperand.JoinTypeName);
					}
					currentClassInfo = joinClassInfo;
					currentProperty = joinOperand.AggregatedExpression;
					joinOperand = currentProperty as JoinOperand;
				}
				OperandProperty op = currentProperty as OperandProperty;
				OperandProperty op2 = null;
				if(IsNull(op)) {
					FunctionOperator funcOp = currentProperty as FunctionOperator;
					if(!IsNull(funcOp) && funcOp.OperatorType == FunctionOperatorType.Iif) {
						op = funcOp.Operands[1] as OperandProperty;
						op2 = funcOp.Operands[2] as OperandProperty;
						if(IsNull(op) && !IsNull(op2)) {
							OperandProperty tempOp = op2;
							op2 = op;
							op = tempOp;
						}
					}
				}
				XPClassInfo classInfo = null;
				ExpressionEvaluator postEval = null;
				if(!IsNull(op)) {
					if(CriteriaOperator.Equals(op, Parser.ThisCriteria)) {
						classInfo = currentClassInfo;
					} else {
						MemberInfoCollection col = MemberInfoCollection.ParsePersistentPath(currentClassInfo, op.PropertyName);
						XPMemberInfo m = col[col.Count - 1];
						classInfo = m.ReferenceType;
						if(!IsNull(op2)) {
							MemberInfoCollection col2 = MemberInfoCollection.ParsePersistentPath(currentClassInfo, op2.PropertyName);
							XPMemberInfo m2 = col2[col2.Count - 1];
							classInfo = AnalyzeCriteriaCreator.GetDownClass(classInfo, m2.ReferenceType);
						}
					}
				} else {
					QuerySet qs = currentProperty as QuerySet;
					if(!IsNull(qs) && !(qs is FreeQuerySet)) {
						classInfo = currentClassInfo;
						if(!qs.IsEmpty && !IsNull(qs.Property)) {
							if(InTransaction){
								postEval = new XPQueryPostEvaluator(classInfo.GetEvaluatorContextDescriptorInTransaction(), qs.Property, false, GetSession().Dictionary.CustomFunctionOperators);
							}else{
								postEval = new XPQueryPostEvaluator(classInfo.GetEvaluatorContextDescriptor(), qs.Property, false, GetSession().Dictionary.CustomFunctionOperators);								
							}
							op = qs.Property;
						}
					}
				}
				if(classInfo != null) {
					object[] ids = new object[data.Count];
					for(int j = 0; j < data.Count; j++) {
						ids[j] = data[j][sourcePos];
					}
					ICollection result = GetSession().GetObjectsByKey(classInfo, ids, false);
					if(postEval != null) {
						if(!IsNull(op)) {
							GetSession().PreFetch(classInfo, result, op.PropertyName);
						}
						result = postEval.EvaluateOnObjects(result);
					}
					int jC = 0;
					foreach(object resObject in result) {
						data[jC++][sourcePos] = resObject;						
					}
				}
				if(sourcePos != i) {
					for(int j = 0; j < data.Count; j++) {
						data[j][i] = data[j][sourcePos];
					}
				}
			}
		}
		object GetSingleObject(CriteriaOperator val, bool allowDefault, SortAction sort, bool single) {
			if(Sorting == null && Skip.HasValue) {
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_SkipOperationIsNotSupportedWithoutSorting));
			}
			val = GroupOperator.And(Criteria, val);
			object res;
			if (ZeroTop) {
				if (!allowDefault)
					throw new InvalidOperationException(Res.GetString(Res.LinqToXpo_SequenceContainsNoMatchingElement));
				return null;
			}
			if(Sorting == null || (single && (!Top.HasValue || Top != 1))) {
				if(!single)
					res = SessionFindObject(ObjectClassInfo, val);
				else {
					ICollection col = SessionGetObjects(ObjectClassInfo, val, Sorting, Skip.GetValueOrDefault(0), Top.HasValue && Top == 1 ? 1 : 2, false);
					IEnumerator enumerator = col.GetEnumerator();
					if(enumerator.MoveNext()) {
						if(col.Count > 1)
							throw new InvalidOperationException(Res.GetString(Res.LinqToXpo_SequenceContainsMoreThanOneElement));
						res = enumerator.Current;
					} else
						res = null;
				}
			} else {
				SortingCollection sortCol;
				bool returnNull = false;
				if(sort == SortAction.Reverse) {
					if(Skip.HasValue) {
						returnNull = (Convert.ToInt32(SessionEvaluate(ObjectClassInfo, new AggregateOperand(null, null, Aggregate.Count, null), val)) - Skip.Value) < 1;
					}
					sortCol = new SortingCollection();
					for(int i = 0; i < Sorting.Count; i++)
						sortCol.Add(new SortProperty(Sorting[i].Property, Sorting[i].Direction == DevExpress.Xpo.DB.SortingDirection.Ascending ? DevExpress.Xpo.DB.SortingDirection.Descending : DevExpress.Xpo.DB.SortingDirection.Ascending));
				} else
					sortCol = Sorting;
				if(returnNull)
					res = null;
				else {
					ICollection col = SessionGetObjects(ObjectClassInfo, val, sortCol, sort == SortAction.Reverse ? 0 : Skip.GetValueOrDefault(0), 1, false);
					IEnumerator enumerator = col.GetEnumerator();
					if(enumerator.MoveNext())
						res = enumerator.Current;
					else
						res = null;
				}
			}
			if (!allowDefault && res == null)
				throw new InvalidOperationException(Res.GetString(Res.LinqToXpo_SequenceContainsNoMatchingElement));
			return res;
		}
		delegate object CreateItemDelegate(object[] row);
		CreateItemDelegate create;
		CreateItemDelegate CreateItem(Type type, MemberInitOperator last) {
			if (create == null)
				create = CreateItemCore(type, last);
			return create;
		}
		Expression ConvertToType(Expression exp, Type targetType, Type sourceType) {
			if(sourceType == null)
				return Expression.Convert(exp, targetType);
			Expression result;
			if(sourceType.IsValueType && !(targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))) {
				if(sourceType.IsGenericType && sourceType.GetGenericTypeDefinition() == typeof(Nullable<>))
					sourceType = sourceType.GetGenericArguments()[0];
				result = Expression.Condition(Expression.Equal(exp, Expression.Constant(null)), Expression.New(sourceType), Expression.Convert(exp, sourceType));
			} else if(sourceType.IsValueType && exp.Type == typeof(object)) {
				if(targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
					if(targetType != sourceType) {
						return Expression.Condition(Expression.Equal(exp, Expression.Constant(null)), Expression.Constant(null, targetType), Expression.Convert(Expression.Convert(exp, sourceType), targetType));
					}
					return Expression.Condition(Expression.Equal(exp, Expression.Constant(null)), Expression.Constant(null, sourceType), Expression.Convert(exp, sourceType));
				}
				result = Expression.Condition(Expression.Equal(exp, Expression.Constant(null)), Expression.New(sourceType), Expression.Convert(exp, sourceType));				
			} else
				result = Expression.Convert(exp, sourceType);
			if(targetType != sourceType)
				result = ElementwiseConvertion(result, targetType) ?? Expression.Convert(result, targetType);
			return result;
		}
		static Type GetSourceType(CriteriaTypeResolver resolver, CriteriaOperator prop) {
			if(prop is GroupSet)
				return null;
			return resolver.Resolve(prop);
		}
		class CreateItemCommonKeyComparer : IEqualityComparer<CreateItemCommonKey> {
			public bool Equals(CreateItemCommonKey x, CreateItemCommonKey y) {
				if(x.Type != y.Type) return false;
				if(x.ProjectionSample == y.ProjectionSample) return true;
				if(x.ProjectionSample == null || y.ProjectionSample == null || x.ProjectionSample.Count != y.ProjectionSample.Count) return false;				
				int count = x.ProjectionSample.Count;
				for(int i = 0; i < count; i++){
					if(!object.Equals(x.ProjectionSample[i], y.ProjectionSample[i])) return false;
				}
				return true;
			}
			public int GetHashCode(CreateItemCommonKey obj) {
				int hashCode = obj.Type == null ? 0x11243391 : obj.Type.GetHashCode();
				if(obj.ProjectionSample == null || obj.ProjectionSample.Count == 0) return hashCode ^ 0x43428292;
				hashCode ^= obj.ProjectionSample.Count.GetHashCode();
				foreach(object sample in obj.ProjectionSample) {
					if(sample == null) continue;
					hashCode ^= sample.GetHashCode();
				}
				return hashCode;
			}
		}
		struct CreateItemCommonKey {
			public Type Type;
			public List<object> ProjectionSample;
		}
		readonly static Dictionary<CreateItemCommonKey, CreateItemDelegate> createItemCommonCache = new Dictionary<CreateItemCommonKey, CreateItemDelegate>(new CreateItemCommonKeyComparer());
		readonly static Dictionary<Type, Delegate> elementwiseConvertionCache = new Dictionary<Type, Delegate>();
		class Resolver: CriteriaTypeResolver, IClientCriteriaVisitor<CriteriaTypeResolverResult>, ILinqExtendedCriteriaVisitor<CriteriaTypeResolverResult> {
			XPClassInfo classInfo;
			public Resolver(XPClassInfo info)
				: base(info, CriteriaTypeResolveKeyBehavior.AsIs) {
				classInfo = info;				
			}
			CriteriaTypeResolverResult IClientCriteriaVisitor<CriteriaTypeResolverResult>.Visit(OperandProperty theOperand) {
				MemberInfoCollection path = MemberInfoCollection.ParsePath(classInfo, theOperand.PropertyName);
				XPMemberInfo member = path[path.Count - 1];
				return new DevExpress.Data.Filtering.Helpers.CriteriaTypeResolverResult(member.MemberType);
			}
			CriteriaTypeResolverResult ILinqExtendedCriteriaVisitor<CriteriaTypeResolverResult>.Visit(MemberInitOperator theOperand) {
				if(!theOperand.CreateNewObject && theOperand.Members.Count == 1) {
					CriteriaOperator property = theOperand.Members[0].Property;
					if(IsNull(property)) {
						return new DevExpress.Data.Filtering.Helpers.CriteriaTypeResolverResult(typeof(object));
					}
					return property.Accept(this);
				}
				return new DevExpress.Data.Filtering.Helpers.CriteriaTypeResolverResult(theOperand.GetDeclaringType());
			}
			CriteriaTypeResolverResult ILinqExtendedCriteriaVisitor<CriteriaTypeResolverResult>.Visit(ExpressionAccessOperator theOperand) {
				return new DevExpress.Data.Filtering.Helpers.CriteriaTypeResolverResult(theOperand.LinqExpression.Type);
			}
			CriteriaTypeResolverResult ILinqExtendedCriteriaVisitor<CriteriaTypeResolverResult>.Visit(QuerySet theOperand) {
				if(theOperand.IsEmpty) {
					return Parser.ThisCriteria.Accept(this);
				}
				if(IsNull(theOperand.Property)) {
					if(!IsNull(theOperand.Projection)) {
						return theOperand.Projection.Accept(this);
					}
				} else {
					if(!IsNull(theOperand.Projection)) {
						MemberInfoCollection path = MemberInfoCollection.ParsePath(classInfo, theOperand.Property.PropertyName);
						XPMemberInfo member = path[path.Count - 1];
						if(member.IsAssociationList) {
							return new Resolver(member.CollectionElementType).Process(theOperand.Projection);
						}
					} else {
						return theOperand.Property.Accept(this);
					}
				}
				throw new NotSupportedException();
			}
		}
		Expression CreateSubItemCore(Type subType, MemberInitOperator last, CriteriaTypeResolver resolver, ParameterExpression row, ref int rowIndex) {
			if(subType == null) {
				subType = last.GetDeclaringType();
			}
			if(last.CreateNewObject) {
				if(last.UseConstructor) {
					ConstructorInfo con = last.GetConstructor(subType);
					ParameterInfo[] parameters = con.GetParameters();
					Type[] sourceTypes = last.GetSourceTypes(subType, resolver);
					Expression[] init = new Expression[parameters.Length];
					for(int i = 0; i < parameters.Length; i++) {
						CriteriaOperator property = last.Members[i].Property;
						if(property is MemberInitOperator) {
							init[i] = CreateSubItemCore(sourceTypes[i], (MemberInitOperator)property, resolver, row, ref rowIndex);
							continue;
						}
						if(property is ExpressionAccessOperator) {
							init[i] = CreateSubItemCore(sourceTypes[i], (ExpressionAccessOperator)property, resolver, row, ref rowIndex);
							continue;
						}
						init[i] = ConvertToType(Expression.ArrayIndex(row, Expression.Constant(rowIndex++)), parameters[i].ParameterType, sourceTypes[i]);
					}
					return Expression.New(con, init);
				} else {
					MemberInfo[] members = new MemberInfo[last.Members.Count];
					Type[] sourceTypes = last.GetSourceTypes(subType, resolver);
					for(int i = 0; i < members.Length; i++) {
						members[i] = last.Members[i].GetMember(subType);
					}
					MemberBinding[] init = new MemberBinding[members.Length];
					for(int i = 0; i < members.Length; i++) {
						CriteriaOperator property = last.Members[i].Property;
						if(property is MemberInitOperator) {
							init[i] = Expression.Bind(members[i],
								CreateSubItemCore(sourceTypes[i], (MemberInitOperator)property, resolver, row, ref rowIndex));
							continue;
						}
						if(property is ExpressionAccessOperator) {
							init[i] = Expression.Bind(members[i],
								CreateSubItemCore(sourceTypes[i], (ExpressionAccessOperator)property, resolver, row, ref rowIndex));
							continue;
						}
						init[i] = Expression.Bind(members[i],
							ConvertToType(Expression.ArrayIndex(row, Expression.Constant(rowIndex++)), MemberInitOperator.GetMemberType(members[i]), sourceTypes[i]));
					}
					return Expression.MemberInit(Expression.New(subType), init);
				}
			} else {
				bool valueMode = (subType == typeof(byte[]) || !subType.IsArray) && last.Members.Count == 1;
				Type elementType = valueMode ? subType : subType.GetElementType();
				Type[] sourceTypes = last.GetSourceTypes(subType, resolver);
				Expression[] init = new Expression[last.Members.Count];
				for(int i = 0; i < last.Members.Count; i++) {
					CriteriaOperator property = last.Members[i].Property;
					if(property is MemberInitOperator) {
						init[i] = CreateSubItemCore(sourceTypes[i], (MemberInitOperator)last.Members[i].Property, resolver, row, ref rowIndex);
						continue;
					}
					if(property is ExpressionAccessOperator) {
						init[i] = CreateSubItemCore(sourceTypes[i], (ExpressionAccessOperator)property, resolver, row, ref rowIndex);
						continue;
					}
					init[i] = ConvertToType(Expression.ArrayIndex(row, Expression.Constant(rowIndex++)), elementType, sourceTypes[i]);
				}
				return valueMode ? init[0] : Expression.NewArrayInit(elementType, init);
			}
		}
		static Expression[] GetArgumentExpressions(Expression[] init, MethodInfo method, bool shiftArguments) {
			Expression[] argumentExpressions = new Expression[shiftArguments ? init.Length - 1 : init.Length];
			ParameterInfo[] parameters = method == null ? null : method.GetParameters();
			for(int i = 0; i < argumentExpressions.Length; i++) {
				Expression currentArgument = init[shiftArguments ? i + 1 : i];
				argumentExpressions[i] = parameters != null && parameters[i].ParameterType != currentArgument.Type ? 
					Expression.Convert(currentArgument, parameters[i].ParameterType) : currentArgument;
			}
			return argumentExpressions;
		}
		Expression CreateBinaryItemWithMethod(BinaryExpression bin, Expression[] init) {
			Expression[] args = GetArgumentExpressions(init, bin.Method, false);
			switch(bin.NodeType) {
				case ExpressionType.Add:
					return Expression.Add(args[0], args[1], bin.Method);
				case ExpressionType.AddChecked:
					return Expression.AddChecked(args[0], args[1], bin.Method);
				case ExpressionType.And:
					return Expression.And(args[0], args[1], bin.Method);
				case ExpressionType.AndAlso:
					return Expression.AndAlso(args[0], args[1], bin.Method);
				case ExpressionType.GreaterThan:
					return Expression.GreaterThan(args[0], args[1], bin.IsLiftedToNull, bin.Method);
				case ExpressionType.GreaterThanOrEqual:
					return Expression.GreaterThanOrEqual(args[0], args[1], bin.IsLiftedToNull, bin.Method);
				case ExpressionType.LessThan:
					return Expression.LessThan(args[0], args[1], bin.IsLiftedToNull, bin.Method);
				case ExpressionType.LessThanOrEqual:
					return Expression.LessThanOrEqual(args[0], args[1], bin.IsLiftedToNull, bin.Method);
				case ExpressionType.Modulo:
					return Expression.Modulo(args[0], args[1], bin.Method);
				case ExpressionType.Multiply:
					return Expression.Multiply(args[0], args[1], bin.Method);
				case ExpressionType.MultiplyChecked:
					return Expression.MultiplyChecked(args[0], args[1], bin.Method);
				case ExpressionType.Equal:
					return Expression.Equal(args[0], args[1], bin.IsLiftedToNull, bin.Method);
				case ExpressionType.NotEqual:
					return Expression.NotEqual(args[0], args[1], bin.IsLiftedToNull, bin.Method);
				case ExpressionType.Or:
					return Expression.Or(args[0], args[1], bin.Method);
				case ExpressionType.OrElse:
					return Expression.OrElse(args[0], args[1], bin.Method);
				case ExpressionType.Subtract:
					return Expression.Subtract(args[0], args[1], bin.Method);
				case ExpressionType.SubtractChecked:
					return Expression.SubtractChecked(args[0], args[1], bin.Method);
			}
			throw new NotSupportedException(Res.GetString(Res.LinqToXpo_CurrentExpressionWithX0IsNotSupported, bin));
		}
		Expression CreateSubItemCore(Type subType, ExpressionAccessOperator last, CriteriaTypeResolver resolver, ParameterExpression row, ref int rowIndex) {
			if(last.SourceItems == null || last.SourceItems.Length == 0)
				return last.LinqExpression;
			Expression[] init = new Expression[last.SourceItems.Length];
			Type[] sourceTypes = last.GetSourceTypes(subType, resolver);
			for(int i = 0; i < init.Length; i++) {
				CriteriaOperator source = last.SourceItems[i];
				if(IsNull(source)){
					init[i] = null;
					continue;
				}
				Type sourceType = sourceTypes[i];
				if(source is MemberInitOperator) {
					init[i] = CreateSubItemCore(sourceType, (MemberInitOperator)source, resolver, row, ref rowIndex);
				} else if(source is ExpressionAccessOperator) {
					init[i] = CreateSubItemCore(sourceType, (ExpressionAccessOperator)source, resolver, row, ref rowIndex);
				} else {
					init[i] = Expression.ArrayIndex(row, Expression.Constant(rowIndex++));
				}
			}
			switch(last.LinqExpression.NodeType) {
				case ExpressionType.Invoke:
					return Expression.Invoke(init[0], GetArgumentExpressions(init, null, true));
				case ExpressionType.MemberAccess:
					MemberInfo mi = ((MemberExpression)last.LinqExpression).Member;
					if(init[0].Type != mi.DeclaringType) {
						init[0] = Expression.Convert(init[0], mi.DeclaringType);
					}
					return Expression.MakeMemberAccess(init[0], ((MemberExpression)last.LinqExpression).Member);
				case ExpressionType.Call:
					MethodCallExpression mce = (MethodCallExpression)last.LinqExpression;
					Expression objectExpression = last.InsertFirstNull ? null : init[0];
					if(objectExpression != null && mce.Method.DeclaringType != objectExpression.Type) {
						objectExpression = Expression.Convert(objectExpression, mce.Method.DeclaringType);
					}
					return Expression.Call(objectExpression, mce.Method, GetArgumentExpressions(init, mce.Method, !last.InsertFirstNull));
				case ExpressionType.Constant:
					return last.LinqExpression;
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
				case ExpressionType.And:
				case ExpressionType.AndAlso:
				case ExpressionType.Coalesce:
				case ExpressionType.Divide:
				case ExpressionType.Equal:
				case ExpressionType.GreaterThan:
				case ExpressionType.GreaterThanOrEqual: 
				case ExpressionType.LessThan: 
				case ExpressionType.LessThanOrEqual: 
				case ExpressionType.Modulo:
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
				case ExpressionType.NotEqual:
				case ExpressionType.Or:
				case ExpressionType.OrElse:
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
					return CreateBinaryItemWithMethod((BinaryExpression)last.LinqExpression, init);
				case ExpressionType.ArrayIndex:
					return Expression.ArrayIndex(init[0], init[1].Type == typeof(int) ? init[1] : Expression.Convert(init[1], typeof(int)));
				case ExpressionType.ArrayLength:
					return Expression.ArrayLength(init[0]);
				case ExpressionType.Convert: {
						UnaryExpression original = ((UnaryExpression)last.LinqExpression);
						Expression currentOperand = init[0];
						if(currentOperand.Type == typeof(object)) {
							if(original.Type == typeof(object)) {
								return currentOperand;
							}
							if(original.Operand.Type != typeof(object)) {
								currentOperand = Expression.Convert(currentOperand, original.Operand.Type);
							}
						}
						return ElementwiseConvertion(currentOperand, last.LinqExpression.Type) ?? Expression.Convert(currentOperand, last.LinqExpression.Type, original.Method);
					}
				case ExpressionType.ConvertChecked: {
						UnaryExpression original = ((UnaryExpression)last.LinqExpression);
						Expression currentOperand = init[0];
						if(currentOperand.Type == typeof(object) && original.Operand.Type != typeof(object)) {
							currentOperand = Expression.Convert(currentOperand, original.Operand.Type);
						}
						return ElementwiseConvertion(currentOperand, last.LinqExpression.Type) ?? Expression.ConvertChecked(currentOperand, last.LinqExpression.Type, ((UnaryExpression)last.LinqExpression).Method);
					}
				case ExpressionType.Negate:
					return Expression.Negate(init[0], ((UnaryExpression)last.LinqExpression).Method);
				case ExpressionType.NegateChecked:
					return Expression.NegateChecked(init[0], ((UnaryExpression)last.LinqExpression).Method);
				case ExpressionType.Not:
					return Expression.Not(init[0], ((UnaryExpression)last.LinqExpression).Method);
				case ExpressionType.Quote:
					return Expression.Quote(init[0]);
				case ExpressionType.UnaryPlus:
					return Expression.UnaryPlus(init[0], ((UnaryExpression)last.LinqExpression).Method);
				case ExpressionType.Conditional: {
						ConditionalExpression conditional = (ConditionalExpression)last.LinqExpression;
						if(init[0].Type != conditional.Test.Type) init[0] = Expression.Convert(init[0], conditional.Test.Type);
						if(init[1].Type != conditional.IfTrue.Type) init[1] = Expression.Convert(init[1], conditional.IfTrue.Type);
						if(init[2].Type != conditional.IfFalse.Type) init[2] = Expression.Convert(init[2], conditional.IfFalse.Type);
						return Expression.Condition(init[0], init[1], init[2]);
					}
				case ExpressionType.Lambda: {
						LambdaExpression lampda = (LambdaExpression)last.LinqExpression;
						return Expression.Lambda(init[0], lampda.Parameters.ToArray());
					}
				case ExpressionType.TypeIs:
					return Expression.TypeIs(init[0], ((TypeBinaryExpression)last.LinqExpression).TypeOperand);
				case ExpressionType.TypeAs:
					return Expression.TypeAs(init[0], ((UnaryExpression)last.LinqExpression).Type);
			}
			throw new NotSupportedException(Res.GetString(Res.LinqToXpo_CurrentExpressionWithX0IsNotSupported, last.LinqExpression));
		}
		readonly static Type[] convertedInterfaces = new Type[] {
			typeof(IEnumerable<>),
			typeof(ICollection<>),
			typeof(IList<>)
		};
		static Delegate GetElementwiseConvertionDelegate(Type destTypeArgument, Type sourceTypeArgument, ParameterExpression parameter, out Type delegateType) {
			delegateType = typeof(Func<,>).MakeGenericType(sourceTypeArgument, destTypeArgument);
			Delegate func;
			lock(elementwiseConvertionCache) {
				if(!elementwiseConvertionCache.TryGetValue(delegateType, out func)) {
					func = Expression.Lambda(delegateType, Expression.Convert(parameter, destTypeArgument), parameter).Compile();
					elementwiseConvertionCache.Add(delegateType, func);
				}
				return func;
			}
		}
		static Expression MakeElementwiseConvertionExpression(Expression source, Type targetTypeArgument, Type sourceTypeArgument) {
			var parameter = Expression.Parameter(sourceTypeArgument, "s");
			Type delegateType;
			var func = Expression.Constant(GetElementwiseConvertionDelegate(targetTypeArgument, sourceTypeArgument, parameter, out delegateType), delegateType);
			var select = Expression.Call(typeof(Enumerable), "Select", new Type[] { sourceTypeArgument, targetTypeArgument }, source, func);
			return Expression.Call(typeof(Enumerable), "ToList", new Type[] { targetTypeArgument }, select);
		}
		static Expression ElementwiseConvertion(Expression source, Type targetType) {
			Type sourceType = source.Type;			
			if(targetType.IsInterface && targetType.IsGenericType) {
				Type targetTypeGenericDefinition = targetType.GetGenericTypeDefinition();
				if(convertedInterfaces.Any(i => i == targetTypeGenericDefinition)) {
					Type destTypeArgument = targetType.GetGenericArguments()[0];
					if(sourceType.IsGenericType) {
						if(Parser.IsImplementsInterface(targetType, sourceType) || Parser.IsImplementsInterface(sourceType, targetType))
							return null;
						Type[] sourceTypeArguments = sourceType.GetGenericArguments();
						if(sourceTypeArguments.Length == 1) {
							return MakeElementwiseConvertionExpression(source, destTypeArgument, sourceTypeArguments[0]);
						}
					} else if(sourceType == typeof(IEnumerable) || sourceType.GetInterfaces().Any(i => i == typeof(IEnumerable))) {
						if(Parser.IsImplementsInterface(sourceType, targetType))
							return null;
						Type typedSourceType = sourceType.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
						Expression typedSource;
						Type sourceArgumentType;
						if(typedSourceType == null) {
							sourceArgumentType = typeof(object);
							typedSource = Expression.Call(typeof(Enumerable), "OfType", new Type[] { sourceArgumentType }, source);
						} else {
							sourceArgumentType = typedSourceType.GetGenericArguments()[0];
							typedSource = Expression.Convert(source, typedSourceType);
						}
						return MakeElementwiseConvertionExpression(typedSource, destTypeArgument, sourceArgumentType);
					}
				}
			}
			return null;
		}
		CreateItemDelegate CreateItemCore(Type type, MemberInitOperator last) {
			if(!last.CreateNewObject) {
				if(!type.IsArray) {
					if(!(last.Members[0].Property is ExpressionAccessOperator))
						return (row) => { return row[0]; };
				} else
					if(type == typeof(object[]))
						return (row) => { return row; };
			}
			CriteriaTypeResolver resolver = new Resolver(ObjectClassInfo);
			List<object> projectionSample = new List<object>(40);
			GetProjectionSample(type, resolver, last, projectionSample);
			CreateItemCommonKey key = new CreateItemCommonKey { Type = type, ProjectionSample = projectionSample };
			lock(createItemCommonCache) {
				CreateItemDelegate create;
				if(!createItemCommonCache.TryGetValue(key, out create)) {
					ParameterExpression row = Expression.Parameter(typeof(object[]), "row");
					int rowIndex = 0;
					Expression createExpression = CreateSubItemCore(type, last, resolver, row, ref rowIndex);
					if(createExpression.NodeType == ExpressionType.MemberInit) {
						createExpression = Expression.Convert(createExpression, typeof(object));
					}
					if(createExpression.NodeType == ExpressionType.Quote) {
						createExpression = ((UnaryExpression)createExpression).Operand;
					}
					if(createExpression.NodeType == ExpressionType.Lambda) {
						createExpression = ((LambdaExpression)createExpression).Body;
					}
					create = Expression.Lambda<CreateItemDelegate>(createExpression, row).Compile();		   
					createItemCommonCache.Add(key, create);
				}				
				return create;
			}
		}
		static readonly object projectionSampleMIO = new object();
		static readonly object projectionSampleEAO = new object();
		static readonly object projectionSampleBegin = new object();
		static readonly object projectionFreeQueryThis = new object();
		static readonly object projectionSampleEnd = new object();
		void GetProjectionSample(Type type, CriteriaTypeResolver resolver, CriteriaOperator property, List<object> projectionSample) {
			if(property is GroupSet) {
				CriteriaOperatorCollection grouping = GetGrouping();
				for(int i = 0; i < grouping.Count; i++) {
					projectionSample.Add(GetSourceType(resolver, grouping[i]));
				}
				return;	
			}
			if(property is QuerySet) {
				QuerySet qs = (QuerySet)property;
				if(qs is FreeQuerySet) {
					FreeQuerySet fqs = (FreeQuerySet)qs;
					if(IsNull(fqs.Projection)) {
						projectionSample.Add(projectionFreeQueryThis);
					} else {
						if(fqs.Projection.CreateNewObject || fqs.Projection.Members.Count > 1)
							throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ComplexDataSelectionIsNotSupportedPerhapsY));
						projectionSample.Add(fqs.Projection.Members[0].Property);
					}
					projectionSample.Add(fqs.JoinType);
					return;
				}
				if((!qs.IsEmpty && IsNull(qs.Property) || !IsNull(qs.Condition))) {
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ComplexDataSelectionIsNotSupportedPerhapsY));
				}
				projectionSample.Add(type);
				return;
			}
			if(property is MemberInitOperator) {
				MemberInitOperator nestedMemberInit = (MemberInitOperator)property;
				projectionSample.Add(projectionSampleMIO);
				projectionSample.Add(nestedMemberInit.CreateNewObject);
				projectionSample.Add(nestedMemberInit.UseConstructor);
				projectionSample.Add(nestedMemberInit.DeclaringTypeName);
				if(nestedMemberInit.UseConstructor) {
					projectionSample.Add(nestedMemberInit.GetConstructor(type));
				}
				projectionSample.Add(projectionSampleBegin);
				Type[] sourceTypes = nestedMemberInit.GetSourceTypes(type, resolver);
				for(int i = 0; i < nestedMemberInit.Members.Count; i++){
					projectionSample.Add(nestedMemberInit.Members[i].MemberName);
					GetProjectionSample(sourceTypes[i], resolver, nestedMemberInit.Members[i].Property, projectionSample);
				}
				projectionSample.Add(projectionSampleEnd);
				return;
			}
			if(property is ExpressionAccessOperator) {
				ExpressionAccessOperator expressionOp = (ExpressionAccessOperator)property;
				projectionSample.Add(projectionSampleEAO);
				projectionSample.Add(expressionOp.LinqExpression.NodeType);
				projectionSample.Add(expressionOp.LinqExpression.Type);
				switch(expressionOp.LinqExpression.NodeType){
					case ExpressionType.Call:
						projectionSample.Add(((MethodCallExpression)expressionOp.LinqExpression).Method);
						break;
					case ExpressionType.MemberAccess:
						projectionSample.Add(((MemberExpression)expressionOp.LinqExpression).Member);
						break;
				}
				projectionSample.Add(projectionSampleBegin);
				Type[] sourceTypes = expressionOp.GetSourceTypes(type, resolver);
				for(int i = 0; i < expressionOp.SourceItems.Length; i++) {
					GetProjectionSample(sourceTypes[i], resolver, expressionOp.SourceItems[i], projectionSample);
				}
				projectionSample.Add(projectionSampleEnd);
				return;
			}
			if(ReferenceEquals(property, null)) {
				projectionSample.Add(typeof(int));
				return;
			}
			projectionSample.Add(type);
		}
		void ProcessAssignmentMember(CriteriaOperator property, CriteriaOperatorCollection props, CriteriaOperatorCollection correspondingPropDict) {
			if(property is GroupSet) {
				correspondingPropDict.Add(property);
				props.AddRange(GetGrouping());
				return;
			}
			if(property is QuerySet) {
				QuerySet qs = (QuerySet)property;
				if(qs is FreeQuerySet) {
					FreeQuerySet fqs = (FreeQuerySet)qs;
					CriteriaOperator expression;
					if(IsNull(fqs.Projection) ) {
						expression = new OperandProperty("This");
					}else{
						if(fqs.Projection.CreateNewObject || fqs.Projection.Members.Count != 1)
							throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ComplexDataSelectionIsNotSupportedPerhapsY));
						expression = fqs.Projection.Members[0].Property;
					}
					CriteriaOperator co = fqs.CreateJoinOperand(fqs.Condition, expression, Aggregate.Single);
					correspondingPropDict.Add(co);
					props.Add(co);
					return;
				}
				if((!qs.IsEmpty && IsNull(qs.Property) || !IsNull(qs.Condition))) {
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ComplexDataSelectionIsNotSupportedPerhapsY));
				}
				correspondingPropDict.Add(property);
				props.Add(Parser.ThisCriteria);
				return;
			}
			if(property is MemberInitOperator) {
				MemberInitOperator nestedMemberInit = (MemberInitOperator)property;
				foreach(XPMemberAssignment nestedMi in nestedMemberInit.Members) {
					ProcessAssignmentMember(nestedMi.Property, props, correspondingPropDict);
				}
				return;
			}
			if(property is ExpressionAccessOperator) {
				ExpressionAccessOperator expressionProperty = ((ExpressionAccessOperator)property);
				for(int i = 0; i < expressionProperty.SourceItems.Length; i++) {
					CriteriaOperator source = expressionProperty.SourceItems[i];
					if(IsNull(source))
						continue;
					ProcessAssignmentMember(source, props, correspondingPropDict);
				}
				return;
			}
			if(ReferenceEquals(property, null)) {
				correspondingPropDict.Add(property);
				props.Add(new ConstantValue(0));
				return;
			}
			correspondingPropDict.Add(property);
			props.Add(property);
		}
		ICollection GetData(Type type) {
			if(IsNull(Projection)) {
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_CurrentExpressionWithX0IsNotSupported, "Select"));
			}
			if(Sorting == null && Skip.HasValue) {
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_SkipOperationIsNotSupportedWithoutSorting));
			}
			CriteriaOperatorCollection props = new CriteriaOperatorCollection();
			CriteriaOperatorCollection correspondingPropDict = new CriteriaOperatorCollection();
			MemberInitOperator last = Projection;
			StringBuilder projectionString = new StringBuilder();
			foreach(XPMemberAssignment mi in last.Members) {
				ProcessAssignmentMember(mi.Property, props, correspondingPropDict);
			}
			List<object[]> data = SessionSelectData(ObjectClassInfo, props, Criteria, GetGrouping(), GroupCriteria, false,
					 Skip ?? 0, Top ?? 0, Sorting);
			DataPostProcessing(correspondingPropDict, data, type);
			List<object> list = new List<object>(data.Count);
			CreateItemDelegate create = CreateItem(type, last);
			foreach(object[] rec in data)
				list.Add(create(rec));
			return list;
		}
		ICollection GetObjects() {
			if(Sorting == null && Skip.HasValue) {
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_SkipOperationIsNotSupportedWithoutSorting));
			}
			return SessionGetObjects(ObjectClassInfo, Criteria, Sorting, Skip ?? 0,
				Top.HasValue ? Top.Value : 0, false);
		}
		internal void EnumerateAsync(Type type, AsyncLoadObjectsCallback callback) {
			if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IGrouping<,>))
				throw new NotSupportedException();
			if(!IsNull(Projection) || IsGroup)
				throw new NotSupportedException();
			if(Sorting == null && Skip.HasValue) {
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_SkipOperationIsNotSupportedWithoutSorting));
			}
			SessionGetObjectsAsync(ObjectClassInfo, Criteria, Sorting, Skip.HasValue ? Skip.Value : 0,
				Top.HasValue ? Top.Value : 0, false, callback);
		}
		protected ICollection Enumerate(Type type) {
			if (ZeroTop)
				return new List<object>();
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IGrouping<,>))
				return EnumerateGroups(type);
			return IsNull(Projection) && !IsGroup ? GetObjects() : GetData(type);
		}
		ICollection EnumerateGroups(Type type) {
			if(Sorting == null && Skip.HasValue) {
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_SkipOperationIsNotSupportedWithoutSorting));
			}
			List<object[]> data = SessionSelectData(ObjectClassInfo, GetGrouping(), Criteria, GetGrouping(), GroupCriteria, false,
					Skip.HasValue ? Skip.Value : 0, Top.HasValue ? Top.Value : 0, Sorting);
			MemberInitOperator init = GroupKey as MemberInitOperator;
			CriteriaOperatorCollection props = new CriteriaOperatorCollection();
			CriteriaOperatorCollection correspondingProps = new CriteriaOperatorCollection();
			if(IsNull(init)) {
				XPMemberAssignmentCollection members = new XPMemberAssignmentCollection();
				members.Add(new XPMemberAssignment(GroupKey));
				props.Add(GroupKey);
				correspondingProps.Add(GroupKey);
				init = new MemberInitOperator(type, members, false);
			} else {
				foreach(XPMemberAssignment member in init.Members) {
					ProcessAssignmentMember(member.Property, props, correspondingProps);
				}
			}
			DataPostProcessing(correspondingProps, data, type);
			List<object> list = new List<object>(data.Count);
			CreateItemDelegate create = CreateGroupItem(type, Projection);
			foreach (object[] rec in data)
				list.Add(create(rec));
			return list;
		}
		CreateItemDelegate CreateGroupItem(Type type, MemberInitOperator last) {
			if (create == null)
				create = CreateGroupItemCore(type, last);
			return create;
		}
		CreateItemDelegate CreateGroupItemCore(Type type, MemberInitOperator last) {
			Type genericGroup = typeof(GroupCollection<,>).MakeGenericType(type.GetGenericArguments());
			ParameterExpression row = Expression.Parameter(typeof(object[]), "row");
			MemberInitOperator init = GroupKey as MemberInitOperator;
			if (IsNull(init)) {
				XPMemberAssignmentCollection members = new XPMemberAssignmentCollection();
				members.Add(new XPMemberAssignment(GroupKey));
				init = new MemberInitOperator(type, members, false);
			}
			Type keyType = type.GetGenericArguments()[0];
			CreateItemDelegate keyCreator = CreateItemCore(keyType, init);
			Expression<CreateItemDelegate> l = Expression.Lambda<CreateItemDelegate>(
				Expression.New(genericGroup.GetConstructor(new Type[] { keyType, typeof(XPQueryBase), typeof(CriteriaOperatorCollection), typeof(MemberInitOperator), typeof(object[]) }),
				ConvertToType(Expression.Invoke(Expression.Constant(keyCreator), row), keyType, keyType), Expression.Constant(this), Expression.Constant(GetGrouping()), Expression.Constant(last, typeof(MemberInitOperator)), row), row);
			return l.Compile();
		}
		class GroupCollection<TKey, TElement> : XPQuery<TElement>, IGrouping<TKey, TElement> {
			TKey key;
			public GroupCollection(TKey key, XPQueryBase parent, CriteriaOperatorCollection props, MemberInitOperator projection, object[] row)
				: base(parent.session) {
				this.key = key;
				query.ObjectTypeName = parent.query.ObjectTypeName;
				objectClassInfo = parent.objectClassInfo;
				List<CriteriaOperator> ops = new List<CriteriaOperator>();
				ops.Add(parent.Criteria);
				for (int i = 0; i < props.Count; i++)
					ops.Add(new BinaryOperator(props[i], new OperandValue(row[i]), BinaryOperatorType.Equal));
				Criteria = GroupOperator.And(ops.ToArray());
				query.Projection = projection;
			}
			public TKey Key {
				get { return key; }
			}
		}
		protected abstract object CloneCore();
	}
	sealed class Parser {
		internal static OperandProperty ThisCriteria = new OperandProperty("This");
		static bool IsNull(object val) {
			return val == null;
		}
		XPDictionary Dictionary { get { return classInfo.Dictionary; } }
		readonly Dictionary<ParameterExpression, CriteriaOperator> parameters;
		readonly XPClassInfo[] upDepthList;
		readonly XPClassInfo classInfo;
		readonly CustomCriteriaCollection customCriteriaCollection;
		readonly Dictionary<Expression, CriteriaOperator> cache;
		readonly ParamExpression Resolver;
		Parser(XPClassInfo[] upDepthList, CustomCriteriaCollection customCriteriaCollection, ParamExpression resolver, Dictionary<ParameterExpression, CriteriaOperator> parameters) {
			this.classInfo = upDepthList[0];
			this.upDepthList = upDepthList;
			this.customCriteriaCollection = customCriteriaCollection;
			this.parameters = parameters;
			if(resolver != null) {
				Resolver = resolver;
				cache = classInfo.CreateCache(() => new Dictionary<Expression, CriteriaOperator>());
			}
		}
		Parser(XPClassInfo classInfo, CustomCriteriaCollection customCriteriaCollection, ParamExpression resolver, Dictionary<ParameterExpression, CriteriaOperator> parameters) {
			this.classInfo = classInfo;
			this.upDepthList = new XPClassInfo[] { classInfo };
			this.customCriteriaCollection = customCriteriaCollection;
			this.parameters = parameters;
			if(resolver != null) {
				Resolver = resolver;
				cache = classInfo.CreateCache(() => new Dictionary<Expression, CriteriaOperator>());
			}
		}
		static bool HasExpressionAccess(params CriteriaOperator[] operands) {
			if(operands == null || operands.Length == 0) return false;
			foreach(CriteriaOperator operand in operands) {
				if(operand is ExpressionAccessOperator)
					return true;
			}
			return false;
		}
		static bool HasExpressionAccess(CriteriaOperator operand) {
			return operand is ExpressionAccessOperator;
		}
		static ExpressionAccessOperator GetCauseOfExpressionAccess(CriteriaOperator operand) {
			ExpressionAccessOperator current = operand as ExpressionAccessOperator;
			if(IsNull(current))
				return null;
			bool isFound;
			do {
				isFound = false;
				foreach(var op in current.SourceItems) {
					var expressionAccessOperator = op as ExpressionAccessOperator;
					if(!IsNull(expressionAccessOperator)) {
						current = expressionAccessOperator;
						isFound = true;
						break;
					}
				}
			} while(isFound);
			return current;
		}
		static string GetCauseStringOfExpressionAccess(CriteriaOperator operand) {
			var result = GetCauseOfExpressionAccess(operand);
			return IsNull(result) ? "unknown" : result.LinqExpression.ToString(); 
		}
		static bool HasExpressionAccess(CriteriaOperator operand1, CriteriaOperator operand2) {
			return operand1 is ExpressionAccessOperator || operand2 is ExpressionAccessOperator;
		}
		static bool HasExpressionAccess(CriteriaOperator operand1, CriteriaOperator operand2, CriteriaOperator operand3) {
			return operand1 is ExpressionAccessOperator || operand2 is ExpressionAccessOperator || operand3 is ExpressionAccessOperator;
		}
		static bool IsNotParsableQuerySet(CriteriaOperator co) {
			QuerySet qSet = co as QuerySet;
			if(IsNull(qSet)) return false;
			if(!IsNull(qSet.Property) && IsNull(qSet.Projection)) {
				return true;
			}
			return false;
		}
		static bool HasExpressionOrMemberInitOrQuerySet(params CriteriaOperator[] operands) {
			if(operands == null || operands.Length == 0) return false;
			foreach(CriteriaOperator operand in operands) {
				if(operand is ExpressionAccessOperator || operand is MemberInitOperator || IsNotParsableQuerySet(operand)) 
					return true;
			}
			return false;
		}
		static bool HasExpressionOrMemberInitOrQuerySet(CriteriaOperator operand) {
			return operand is ExpressionAccessOperator || operand is MemberInitOperator || IsNotParsableQuerySet(operand);
		}
		static bool HasExpressionOrMemberInitOrQuerySet(CriteriaOperator operand1, CriteriaOperator operand2) {
			if(operand1 is ExpressionAccessOperator || operand1 is MemberInitOperator || IsNotParsableQuerySet(operand1)) return true;
			return operand2 is ExpressionAccessOperator || operand2 is MemberInitOperator || IsNotParsableQuerySet(operand2);
		}
		static bool HasExpressionOrMemberInitOrQuerySet(CriteriaOperator operand1, CriteriaOperator operand2, CriteriaOperator operand3) {
			if(operand1 is ExpressionAccessOperator || operand1 is MemberInitOperator || IsNotParsableQuerySet(operand1)) return true;
			if(operand2 is ExpressionAccessOperator || operand2 is MemberInitOperator || IsNotParsableQuerySet(operand2)) return true;
			return operand3 is ExpressionAccessOperator || operand3 is MemberInitOperator || IsNotParsableQuerySet(operand3);
		}
		static IList<ParameterExpression> GetLambdaParams(Expression call, int count) {
			UnaryExpression u = call as UnaryExpression;
			if(u != null)
				call = u.Operand;
			LambdaExpression l = call as LambdaExpression;
			if(l != null) {
				IList<ParameterExpression> lParams = l.Parameters;
				if (lParams.Count != count)
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheLambdaExpressionWithSuchParametersIsNot, l));
				return lParams;
			}
			if (count == 0)
				return null;
			throw new NotSupportedException(Res.GetString(Res.LinqToXpo_LambdaExpressionIsExpectedX0, call));
		}
		CriteriaOperator ParseExpression(Type type, Expression expression, params CriteriaOperator[] maps) {
			XPClassInfo oldClassInfo = classInfo;
			if(!IsNull(type)) {
				oldClassInfo = Dictionary.QueryClassInfo(type);
			}
			ParamExpression resolver = GetResolver(ref expression);
			Dictionary<ParameterExpression, CriteriaOperator> parameters = PrepareParametersForJoin();
			AddMaps(expression, maps, parameters);
			Parser p = new Parser(oldClassInfo, customCriteriaCollection, Resolver ?? resolver, parameters);
			return p.ParseExpression(expression);
		}
		private static ParamExpression GetResolver(ref Expression expression) {
			MethodCallExpression call = expression as MethodCallExpression;
			ReadOnlyCollection<Expression> arguments;
			if(call == null || (arguments = call.Arguments).Count != 2)
				return null;
			ConstantExpression constant = arguments[1] as ConstantExpression;
			if(constant == null)
				return null;
			ParamExpression resolver = constant.Value as ParamExpression;
			if(resolver != null)
				expression = arguments[0];
			return resolver;
		}
		CriteriaOperator ParseExpression(OperandProperty col, Expression expression, params CriteriaOperator[] maps) {
			XPClassInfo oldClassInfo = classInfo;
			int upDepth = 0;
			List<XPClassInfo> newUpDepthList = new List<XPClassInfo>(upDepthList);
			if(!IsNull(col)) {
				int currentUpDepth = 0;
				string propertyName = col.PropertyName;
				while(propertyName.StartsWith("^.")) {
					propertyName = propertyName.Remove(0, 2);
					currentUpDepth++;
					if(newUpDepthList.Count == 0) throw new InvalidOperationException();
					newUpDepthList.RemoveAt(0);
				}
				MemberInfoCollection path = MemberInfoCollection.ParsePersistentPath(upDepthList[currentUpDepth], propertyName);
				upDepth = path.Count;
				oldClassInfo = path[path.Count - 1].CollectionElementType;
			}
			ParamExpression resolver = GetResolver(ref expression);
			Dictionary<ParameterExpression, CriteriaOperator> parameters = PrepareParameters(expression, col, upDepth);
			AddMaps(expression, maps, parameters);
			newUpDepthList.Insert(0, oldClassInfo);
			Parser p = new Parser(newUpDepthList.ToArray(), customCriteriaCollection, Resolver ?? resolver, parameters);
			return p.ParseExpression(expression);
		}
		public static CriteriaOperator ParseObjectExpression(XPClassInfo classInfo, CustomCriteriaCollection customCriteriaCollection, Expression expression, params CriteriaOperator[] maps) {
			ParamExpression resolver = GetResolver(ref expression);
			Dictionary<ParameterExpression, CriteriaOperator> parameters = new Dictionary<ParameterExpression, CriteriaOperator>();
			AddMaps(expression, maps, parameters);
			Parser p = new Parser(classInfo, customCriteriaCollection, resolver, parameters);
			return p.ParseObjectExpression(expression);
		}
		public static CriteriaOperator ParseExpression(XPClassInfo classInfo, CustomCriteriaCollection customCriteriaCollection, Expression expression, params CriteriaOperator[] maps) {
			ParamExpression resolver = GetResolver(ref expression);
			Dictionary<ParameterExpression, CriteriaOperator> parameters = new Dictionary<ParameterExpression, CriteriaOperator>();
			AddMaps(expression, maps, parameters);
			Parser p = new Parser(classInfo, customCriteriaCollection, resolver, parameters);
			return p.ParseExpression(expression);
		}
		static void AddMaps(Expression expression, CriteriaOperator[] maps, Dictionary<ParameterExpression, CriteriaOperator> parameters) {
			if(maps == null)
				throw new ArgumentException("maps");
			IList<ParameterExpression> lParams = GetLambdaParams(expression, maps.Length);
			for(int i = 0; i < maps.Length; i++) {
				parameters[lParams[i]] = IsNull(maps[i]) ? QuerySet.Empty : maps[i];
			}
		}
		CriteriaOperator ParseExpression(Expression expression) {
			CriteriaOperator op;
			if(cache != null) {
				lock(cache) {
					if(cache.TryGetValue(expression, out op)) {
						return op;
					}
				}
				bool used = Resolver.Use();
				op = ParseExpressionCore(expression);
				if(!Resolver.UnUse(used) && !(expression is ParameterExpression))
					lock(cache)
						cache[expression] = op;
			} else
				op = ParseExpressionCore(expression);
			return op;
		}
		public CriteriaOperator ParseExpressionCore(Expression expression) {
			switch (expression.NodeType) {
				case ExpressionType.Add:
				case ExpressionType.AddChecked:
					return Binary((BinaryExpression)expression, BinaryOperatorType.Plus);
				case ExpressionType.And:
					return IsLogical((BinaryExpression)expression) ? Group((BinaryExpression)expression, GroupOperatorType.And) : Binary((BinaryExpression)expression, BinaryOperatorType.BitwiseAnd);
				case ExpressionType.AndAlso:
					return Group((BinaryExpression)expression, GroupOperatorType.And);
				case ExpressionType.ArrayIndex:
					return ArrayIndex((BinaryExpression)expression);
				case ExpressionType.ArrayLength:
					return ArrayLength((UnaryExpression)expression);
				case ExpressionType.Call:
					return Call((MethodCallExpression)expression);
				case ExpressionType.Coalesce:
					return Coalesce((BinaryExpression)expression);
				case ExpressionType.Conditional:
					return Conditional((ConditionalExpression)expression);
				case ExpressionType.Constant:
					return Constant((ConstantExpression)expression);
				case ExpressionType.Convert:
				case ExpressionType.ConvertChecked:
					return Convert((UnaryExpression)expression);
				case ExpressionType.Divide:
					return Binary((BinaryExpression)expression, BinaryOperatorType.Divide);
				case ExpressionType.Equal:
					return Equal((BinaryExpression)expression);
				case ExpressionType.GreaterThan:
					return LogicalBinary((BinaryExpression)expression, BinaryOperatorType.Greater);
				case ExpressionType.GreaterThanOrEqual:
					return LogicalBinary((BinaryExpression)expression, BinaryOperatorType.GreaterOrEqual);
				case ExpressionType.Lambda: {
						CriteriaOperator bodyOperand = ParseExpression(((LambdaExpression)expression).Body);
						if(bodyOperand is ExpressionAccessOperator) return new ExpressionAccessOperator(expression, bodyOperand);
						return bodyOperand;
					}
				case ExpressionType.LessThan:
					return LogicalBinary((BinaryExpression)expression, BinaryOperatorType.Less);
				case ExpressionType.LessThanOrEqual:
					return LogicalBinary((BinaryExpression)expression, BinaryOperatorType.LessOrEqual);
				case ExpressionType.MemberAccess:
					return MemberAccess((MemberExpression)expression);
				case ExpressionType.MemberInit:
					return MemberInit((MemberInitExpression)expression);
				case ExpressionType.Modulo:
					return Binary((BinaryExpression)expression, BinaryOperatorType.Modulo);
				case ExpressionType.Multiply:
				case ExpressionType.MultiplyChecked:
					return Binary((BinaryExpression)expression, BinaryOperatorType.Multiply);
				case ExpressionType.Negate:
				case ExpressionType.NegateChecked:
					return Unary((UnaryExpression)expression, UnaryOperatorType.Minus);
				case ExpressionType.New:
					return New((NewExpression)expression);
				case ExpressionType.NewArrayInit:
					return NewArrayInit((NewArrayExpression)expression);
				case ExpressionType.Not:
					return Unary((UnaryExpression)expression, UnaryOperatorType.Not);
				case ExpressionType.NotEqual:
					return NotEqual((BinaryExpression)expression);
				case ExpressionType.Or:
					return IsLogical((BinaryExpression)expression) ? Group((BinaryExpression)expression, GroupOperatorType.Or) : Binary((BinaryExpression)expression, BinaryOperatorType.BitwiseOr);
				case ExpressionType.OrElse:
					return Group((BinaryExpression)expression, GroupOperatorType.Or);
				case ExpressionType.Parameter:
					return Parameter((ParameterExpression)expression);
				case ExpressionType.Subtract:
				case ExpressionType.SubtractChecked:
					return Binary((BinaryExpression)expression, BinaryOperatorType.Minus);
				case ExpressionType.TypeIs:
					return TypeIs((TypeBinaryExpression)expression);
				case ExpressionType.Quote: {
						CriteriaOperator qOperand = ParseExpression(((UnaryExpression)expression).Operand);
						if(qOperand is ExpressionAccessOperator) return new ExpressionAccessOperator(expression, qOperand);
						return qOperand;
					}
				case ExpressionType.UnaryPlus:
					return Unary((UnaryExpression)expression, UnaryOperatorType.Plus);
				case ExpressionType.Invoke:
					return ParseInvoke((InvocationExpression)expression);
			}
			throw new NotSupportedException(Res.GetString(Res.LinqToXpo_CurrentExpressionWithX0IsNotSupported, expression.NodeType));
		}
		public CriteriaOperator ParseInvoke(InvocationExpression invoke) {
			CriteriaOperator[] arguments = new CriteriaOperator[invoke.Arguments.Count + 1];
			arguments[0] = ParseExpression(invoke.Expression);
			for(int i = 0; i < arguments.Length - 1; i++) {
				arguments[i + 1] = ParseExpression(invoke.Arguments[i]);
			}
			return new ExpressionAccessOperator(invoke, arguments);
		}
		static bool IsLogical(BinaryExpression binaryExpression) {
			return binaryExpression.Left.Type == typeof(bool) || binaryExpression.Left.Type == typeof(bool?);
		}
		CriteriaOperator ArrayLength(UnaryExpression expression) {
			CriteriaOperator operand = ParseExpression(expression.Operand);
			if(HasExpressionAccess(operand))
				return new ExpressionAccessOperator(expression, operand);
			if(!(operand is OperandValue))
				return new ExpressionAccessOperator(expression);
			return new ConstantCompiler(Dictionary, expression);
		}
		CriteriaOperator ArrayIndex(BinaryExpression expression) {
			CriteriaOperator left = ParseExpression(expression.Left);
			CriteriaOperator right = ParseExpression(expression.Right);
			if(HasExpressionAccess(left, right))
				return new ExpressionAccessOperator(expression, left, right);
			if(!(left is OperandValue) || !(right is OperandValue))
				return new ExpressionAccessOperator(expression, new ExpressionAccessOperator(expression.Left), right);
			return new ConstantCompiler(Dictionary, expression);
		}
		CriteriaOperator TypeIs(TypeBinaryExpression expression) {
			CriteriaOperator obj = ParseExpression(expression.Expression);
			if(HasExpressionOrMemberInitOrQuerySet(obj)) {
				return new ExpressionAccessOperator(expression, obj);
			}
			return new FunctionOperator("IsInstanceOfType", obj, new OperandValue(expression.TypeOperand.FullName));
		}
		CriteriaOperator Group(BinaryExpression expression, GroupOperatorType type) {
			CriteriaOperator left = ParseExpression(expression.Left);
			CriteriaOperator right = ParseExpression(expression.Right);
			if(HasExpressionOrMemberInitOrQuerySet(left, right))
				return new ExpressionAccessOperator(expression, left, right);
			return new GroupOperator(type, left, right);
		}
		CriteriaOperator Unary(UnaryExpression expression, UnaryOperatorType type) {
			CriteriaOperator op = ParseExpression(expression.Operand);
			if (op is OperandValue)
				return new ConstantCompiler(Dictionary, expression);
			if(HasExpressionOrMemberInitOrQuerySet(op))
				return new ExpressionAccessOperator(expression, op);
			return new UnaryOperator(type, op);
		}
		readonly static object boxedZero = 0;
		public bool BinaryDetectCompare(BinaryExpression expression, CriteriaOperator right, BinaryOperatorType type, bool rightIsExpressionAccess, out CriteriaOperator result) {
			MethodCallExpression leftMethodCall = expression.Left as MethodCallExpression;
			if(expression.Left.Type == typeof(int) && expression.Right.Type == typeof(int) && leftMethodCall != null) {
				if(leftMethodCall.Method.Name == "CompareTo" && leftMethodCall.Arguments.Count == 1 && right is OperandValue && object.Equals(((OperandValue)right).Value, boxedZero)) {
					CriteriaOperator innerLeft = ParseExpression(leftMethodCall.Object);
					CriteriaOperator innerRight = ParseExpression(leftMethodCall.Arguments[0]);
					if(rightIsExpressionAccess || HasExpressionOrMemberInitOrQuerySet(innerLeft, innerRight)) {						 
						result = new ExpressionAccessOperator(expression, ParseExpression(expression.Left), right);
						return true;
					}					
					result = new BinaryOperator(innerLeft, innerRight, type);
					return true;
				}
				if(leftMethodCall.Method.DeclaringType == typeof(string) && leftMethodCall.Method.Name == "Compare"
					&& leftMethodCall.Arguments.Count == 2 && right is OperandValue && object.Equals(((OperandValue)right).Value, boxedZero)) {
					CriteriaOperator innerLeft = ParseExpression(leftMethodCall.Arguments[0]);
					CriteriaOperator innerRight = ParseExpression(leftMethodCall.Arguments[1]);
					if(rightIsExpressionAccess || HasExpressionOrMemberInitOrQuerySet(innerLeft, innerRight)) {						
						result = new ExpressionAccessOperator(expression, ParseExpression(expression.Left), right);
						return true;
					}					
					result = new BinaryOperator(innerLeft, innerRight, type);
					return true;
				}
			}
			result = null;
			return false;
		}
		CriteriaOperator Binary(BinaryExpression expression, BinaryOperatorType type) {
			if(type == BinaryOperatorType.Plus && ReferenceEquals(expression.Left.Type, typeof(string)) && ReferenceEquals(expression.Right.Type, typeof(string)))
				return FnConcat(expression, expression.Left, expression.Right);
			CriteriaOperator right = ParseExpression(expression.Right);
			bool rightIsExpressionAccess = HasExpressionOrMemberInitOrQuerySet(right);
			CriteriaOperator detectCompareResult;
			if(BinaryDetectCompare(expression, right, type, rightIsExpressionAccess, out detectCompareResult))
				return detectCompareResult;
			CriteriaOperator left = ParseExpression(expression.Left);
			bool leftIsOperandValue = left is OperandValue;
			bool rightIsOperandValue = right is OperandValue;
			if (leftIsOperandValue && rightIsOperandValue)
				return new ConstantCompiler(Dictionary, expression);
			if(rightIsExpressionAccess || HasExpressionOrMemberInitOrQuerySet(left))
				return new ExpressionAccessOperator(expression, left, right);
			if(expression.Left.Type == typeof(TimeSpan) && expression.Right.Type == typeof(TimeSpan)) {
				if(leftIsOperandValue) {
					OperandValue ov = (OperandValue)left;
					if(ov.Value != null) {
						left = new OperandValue(((TimeSpan)ov.Value).Ticks);
					}
				}
				if(rightIsOperandValue) {
					OperandValue ov = (OperandValue)right;
					if(ov.Value != null) {
						right = new OperandValue(((TimeSpan)ov.Value).Ticks);
					}
				}
			}
			return new BinaryOperator(left, right, type);
		}
		CriteriaOperator Conditional(ConditionalExpression expression) {
			CriteriaOperator test = ParseExpression(expression.Test);
			CriteriaOperator ifTrue = ParseExpression(expression.IfTrue);
			CriteriaOperator ifFalse = ParseExpression(expression.IfFalse);
			if(HasExpressionOrMemberInitOrQuerySet(test, ifTrue, ifFalse)) 
				return new ExpressionAccessOperator(expression, test, ifTrue, ifFalse);
			FunctionOperator nestedIif = ifFalse as FunctionOperator;
			if(!IsNull(nestedIif) && nestedIif.OperatorType == FunctionOperatorType.Iif && nestedIif.Operands.Count >= 3) {
				var operands = new List<CriteriaOperator>(2 + nestedIif.Operands.Count);
				operands.Add(test);
				operands.Add(ifTrue);
				operands.AddRange(nestedIif.Operands);
				return new FunctionOperator(FunctionOperatorType.Iif, operands);
			}
			return new FunctionOperator(FunctionOperatorType.Iif, test, ifTrue, ifFalse);
		}
		CriteriaOperator Coalesce(BinaryExpression expression) {
			CriteriaOperator left = ParseExpression(expression.Left);
			CriteriaOperator right = ParseExpression(expression.Right);
			if(HasExpressionOrMemberInitOrQuerySet(left, right)) 
				return new ExpressionAccessOperator(expression, left, right);
			return new FunctionOperator(FunctionOperatorType.IsNull, left, right);
		}
		CriteriaOperator Constant(ConstantExpression expression) {
			XPQueryBase query = expression.Value as XPQueryBase;
			if (query != null) {
				IQueryable queryable = query as IQueryable;
				if(queryable != null && queryable.ElementType.IsGenericType && queryable.ElementType.GetGenericTypeDefinition() == typeof(IGrouping<,>))
					return new GroupSet(query.Projection, query.GroupKey);
				else
					return new QuerySet(query.Projection);
			}
			return expression.Value == null || IsPrimitive(expression.Value.GetType()) ? (CriteriaOperator)new ConstantValue(expression.Value) : new ConstantCompiler(Dictionary, expression);
		}
		public static bool IsPrimitive(Type type) {
			return type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type == typeof(DateTime) || type == typeof(Guid) || type == typeof(TimeSpan) || (type.IsArray && IsPrimitive(type.GetElementType()));
		}
		static HashSet<Type> containsDeclearingTypes = null;
		static HashSet<Type> ContainsDeclearingTypes {
			get {
				if(containsDeclearingTypes == null) {
					HashSet<Type> newContainsDeclearingTypes = new HashSet<Type> {
						typeof(List<>),
						typeof(ICollection<>),
						typeof(Collection<>),
						typeof(ReadOnlyCollection<>)
					};
					containsDeclearingTypes = newContainsDeclearingTypes;
				}
				return containsDeclearingTypes;
			}
		}
		CriteriaOperator Call(MethodCallExpression call) {
			if(call.Method.Name == "ToString") {
				CriteriaOperator obj = ParseExpression(call.Object);
				if(HasExpressionOrMemberInitOrQuerySet(obj)) return new ExpressionAccessOperator(call, obj);
				return new FunctionOperator(FunctionOperatorType.ToStr, obj);
			} else
				if(call.Method.Name == "CompareTo" && call.Arguments.Count == 1) {
					CriteriaOperator obj = ParseExpression(call.Object);
					CriteriaOperator arg = ParseExpression(call.Arguments[0]);
					if(HasExpressionOrMemberInitOrQuerySet(obj, arg)) 
						return new ExpressionAccessOperator(call, obj, arg);
					return FnCompareTo(obj, arg);
				} else
					if(call.Method.DeclaringType == typeof(string)) {
						return CallString(call);
					} else
						if(call.Method.DeclaringType == typeof(Math)) {
							return CallMath(call);
						} else
							if(call.Method.DeclaringType == typeof(DateTime)) {
								return CallDateTime(call);
							} else
#if !SL
								if(call.Method.DeclaringType.FullName == "System.Data.Linq.SqlClient.SqlMethods") {
									return CallDateDiff(call);
								} else
#endif
									if(call.Method.DeclaringType == typeof(Queryable) || call.Method.DeclaringType == typeof(Enumerable)) {
										return CallQueryable(call);
									} else
										if(CheckMethodIsContains(call.Method, call.Arguments)) 
											return ListContains(call);
			return CallDefault(call);
		}
		bool CheckMethodIsContains(MethodInfo method, ReadOnlyCollection<Expression> arguments) {
			if(!(method.Name == "Contains" && method.DeclaringType.IsGenericType
				&& arguments.Count == 1 && method.DeclaringType.GetGenericArguments()[0] == method.GetParameters()[0].ParameterType
				&& method.ReturnType == typeof(bool)))
				return false;
			Type declType = method.DeclaringType;
			do {
				if(declType.GetGenericTypeDefinition() == typeof(ICollection<>)
					|| (!declType.IsInterface && declType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>))))
					return true;
				declType = declType.BaseType;
			} while(declType != null && declType.IsGenericType);
			return false;
		}
#if !SL
		private CriteriaOperator CallDateDiff(MethodCallExpression call) {
			CriteriaOperator arg0 = null;
			CriteriaOperator arg1 = null;
			if(call.Arguments.Count == 2) {
				arg0 = ParseExpression(call.Arguments[0]);
				arg1 = ParseExpression(call.Arguments[1]);
				if(HasExpressionOrMemberInitOrQuerySet(arg0, arg1)) return new ExpressionAccessOperator(call, null, arg0, arg1);
			}
			switch(call.Method.Name) {
				case "DateDiffDay":
					return new FunctionOperator(FunctionOperatorType.DateDiffDay, arg0, arg1);
				case "DateDiffHour":
					return new FunctionOperator(FunctionOperatorType.DateDiffHour, arg0, arg1);
				case "DateDiffMillisecond":
					return new FunctionOperator(FunctionOperatorType.DateDiffMilliSecond, arg0, arg1);
				case "DateDiffMinute":
					return new FunctionOperator(FunctionOperatorType.DateDiffMinute, arg0, arg1);
				case "DateDiffMonth":
					return new FunctionOperator(FunctionOperatorType.DateDiffMonth, arg0, arg1);
				case "DateDiffSecond":
					return new FunctionOperator(FunctionOperatorType.DateDiffSecond, arg0, arg1);
				case "DateDiffYear":
					return new FunctionOperator(FunctionOperatorType.DateDiffYear, arg0, arg1);
				default: return CallDefault(call);
			}
		}
#endif
		private CriteriaOperator CallString(MethodCallExpression call) {
			switch(call.Method.Name) {
				case "Concat":
					return FnConcatList(call, call.Arguments);
				case "IsNullOrEmpty":
					return FunctionUniversal(call, FunctionOperatorType.IsNullOrEmpty, false, 1, 1);
				case "ToLower":
					return FunctionUniversal(call, FunctionOperatorType.Lower, true, 0, 0);
				case "ToUpper":
					return FunctionUniversal(call, FunctionOperatorType.Upper, true, 0, 0);
				case "Trim":
					return FunctionUniversal(call, FunctionOperatorType.Trim, true, 0, 0);
				case "StartsWith":
					return FunctionUniversal(call, FunctionOperatorType.StartsWith, true, 1, 1);
				case "Equals":
					if(call.Arguments.Count == 1) {
						CriteriaOperator obj = ParseExpression(call.Object);
						CriteriaOperator arg0 = ParseExpression(call.Arguments[0]);
						if(HasExpressionOrMemberInitOrQuerySet(obj, arg0)) return new ExpressionAccessOperator(call, obj, arg0);
						return new BinaryOperator(obj, arg0, BinaryOperatorType.Equal);
					} else
						return CallDefault(call);
				case "EndsWith":
					return FunctionUniversal(call, FunctionOperatorType.EndsWith, true, 1, 1);
				case "Substring":
					if(call.Arguments.Count == 2) {
						CriteriaOperator obj = ParseExpression(call.Object);
						CriteriaOperator arg0 = ParseExpression(call.Arguments[0]);
						CriteriaOperator arg1 = ParseExpression(call.Arguments[1]);
						if(HasExpressionOrMemberInitOrQuerySet(obj, arg0, arg1)) return new ExpressionAccessOperator(call, obj, arg0, arg1);
						return new FunctionOperator(FunctionOperatorType.Substring, obj, arg0, arg1);
					} else {
						CriteriaOperator obj = ParseExpression(call.Object);
						CriteriaOperator arg0 = ParseExpression(call.Arguments[0]);
						if(HasExpressionOrMemberInitOrQuerySet(obj, arg0)) return new ExpressionAccessOperator(call, obj, arg0);
						return new FunctionOperator(FunctionOperatorType.Substring, obj, arg0);
					}
				case "PadLeft":
					return FunctionUniversal(call, FunctionOperatorType.PadLeft, true, 1, 2);
				case "PadRight":
					return FunctionUniversal(call, FunctionOperatorType.PadRight, true, 1, 2);
				case "IndexOf":
					return FnIndexOf(call);
				case "Insert":
					return FunctionUniversal(call, FunctionOperatorType.Insert, true, 2, 2);
				case "Remove":
					return FunctionUniversal(call, FunctionOperatorType.Remove, true, 1, 2);
				case "Replace":
					return FunctionUniversal(call, FunctionOperatorType.Replace, true, 2, 2);
				case "Contains":
					return FunctionUniversal(call, FunctionOperatorType.Contains, true, 1, 1);
				default: return CallDefault(call);
			}
		}
		private CriteriaOperator CallMath(MethodCallExpression call) {
			switch (call.Method.Name) {
				case "Abs":
					return FunctionUniversal(call, FunctionOperatorType.Abs, false, 1, 1);
				case "Atan":
					return FunctionUniversal(call, FunctionOperatorType.Atn, false, 1, 1);
				case "Atan2":
					return FunctionUniversal(call, FunctionOperatorType.Atn2, false, 2, 2);
				case "BigMul":
					return FunctionUniversal(call, FunctionOperatorType.BigMul, false, 2, 2);
				case "Ceiling":
					return FunctionUniversal(call, FunctionOperatorType.Ceiling, false, 1, 1);
				case "Cos":
					return FunctionUniversal(call, FunctionOperatorType.Cos, false, 1, 1);
				case "Cosh":
					return FunctionUniversal(call, FunctionOperatorType.Cosh, false, 1, 1);
				case "Exp":
					return FunctionUniversal(call, FunctionOperatorType.Exp, false, 1, 1);
				case "Floor":
					return FunctionUniversal(call, FunctionOperatorType.Floor, false, 1, 1);
				case "Log10":
					return FunctionUniversal(call, FunctionOperatorType.Log10, false, 1, 1);
				case "Log":
					return FunctionUniversal(call, FunctionOperatorType.Log, false, 1, 2);
				case "Pow":
					return FunctionUniversal(call, FunctionOperatorType.Power, false, 2, 2);
				case "Round":
					return FunctionUniversal(call, FunctionOperatorType.Round, false, 1, 2);
				case "Sign":
					return FunctionUniversal(call, FunctionOperatorType.Sign, false, 1, 1);
				case "Sin":
					return FunctionUniversal(call, FunctionOperatorType.Sin, false, 1, 1);
				case "Sinh":
					return FunctionUniversal(call, FunctionOperatorType.Sinh, false, 1, 1);
				case "Tan":
					return FunctionUniversal(call, FunctionOperatorType.Tan, false, 1, 1);
				case "Tanh":
					return FunctionUniversal(call, FunctionOperatorType.Tanh, false, 1, 1);
				case "Sqrt":
					return FunctionUniversal(call, FunctionOperatorType.Sqr, false, 1, 1);
				case "Acos":
					return FunctionUniversal(call, FunctionOperatorType.Acos, false, 1, 1);
				case "Asin":
					return FunctionUniversal(call, FunctionOperatorType.Asin, false, 1, 1);
				default: return CallDefault(call);
			}
		}
		private CriteriaOperator CallDateTime(MethodCallExpression call) {
			switch (call.Method.Name) {
				case "Add":
					return FunctionUniversal(call, FunctionOperatorType.AddTimeSpan, true, 1, 1);
				case "AddMilliseconds":
					return FunctionUniversal(call, FunctionOperatorType.AddMilliSeconds, true, 1, 1);
				case "AddSeconds":
					return FunctionUniversal(call, FunctionOperatorType.AddSeconds, true, 1, 1);
				case "AddMinutes":
					return FunctionUniversal(call, FunctionOperatorType.AddMinutes, true, 1, 1);
				case "AddHours":
					return FunctionUniversal(call, FunctionOperatorType.AddHours, true, 1, 1);
				case "AddDays":
					return FunctionUniversal(call, FunctionOperatorType.AddDays, true, 1, 1);
				case "AddMonths":
					return FunctionUniversal(call, FunctionOperatorType.AddMonths, true, 1, 1);
				case "AddYears":
					return FunctionUniversal(call, FunctionOperatorType.AddYears, true, 1, 1);
				case "AddTicks":
					return FunctionUniversal(call, FunctionOperatorType.AddTicks, true, 1, 1);
				default: return CallDefault(call);
			}
		}
		private CriteriaOperator CallQueryable(MethodCallExpression call) {
			switch (call.Method.Name) {
				case "All":
					return All(call);
				case "Any":
					return Any(call);
				case "Average": return Average(call);
				case "Contains":
					return Contains(call);
				case "Count":
				case "LongCount":
					return Count(call);
				case "Max": return Max(call);
				case "Min": return Min(call);
				case "Select": return Select(call);
				case "Sum": return Sum(call);
				case "Where": return Where(call);
				default: return CallDefault(call);
			}
		}
		public CriteriaOperator FnCompareTo(CriteriaOperator obj, CriteriaOperator argument) {
			return new FunctionOperator(FunctionOperatorType.Iif, new BinaryOperator(obj, argument, BinaryOperatorType.Equal), new ConstantValue(0), new BinaryOperator(obj, argument, BinaryOperatorType.Greater), new ConstantValue(1), new ConstantValue(-1));
		}
		CriteriaOperator FnConcat(BinaryExpression binary, params Expression[] arguments) {
			bool createExpressionAccess = false;
			CriteriaOperator[] resultOperands = new CriteriaOperator[arguments.Length];
			for(int i = 0; i < arguments.Length; i++) {
				CriteriaOperator operand = ParseExpression(arguments[i]);
				if(HasExpressionOrMemberInitOrQuerySet(operand))
					createExpressionAccess = true;
				resultOperands[i] = operand;
			}
			if(createExpressionAccess)
				return new ExpressionAccessOperator(binary, resultOperands);
			return new FunctionOperator(FunctionOperatorType.Concat, resultOperands);
		}
		CriteriaOperator FnConcatList(MethodCallExpression call, IList<Expression> arguments) {
			bool createExpressionAccess = false;
			CriteriaOperator[] resultOperands = new CriteriaOperator[arguments.Count];
			for(int i = 0; i < arguments.Count; i++) {
				CriteriaOperator operand = ParseExpression(arguments[i]);
				if(HasExpressionOrMemberInitOrQuerySet(operand))
					createExpressionAccess = true;
				resultOperands[i] = operand;
			}
			if(createExpressionAccess)
				return new ExpressionAccessOperator(call, true, resultOperands);
			return new FunctionOperator(FunctionOperatorType.Concat, resultOperands);
		}
		CriteriaOperator FnIndexOf(MethodCallExpression call) {
			CriteriaOperator obj = ParseExpression(call.Object);
			CriteriaOperator arg0 = ParseExpression(call.Arguments[0]);
			if(call.Arguments.Count == 3) {
				CriteriaOperator arg1 = ParseExpression(call.Arguments[1]);
				CriteriaOperator arg2 = ParseExpression(call.Arguments[2]);
				if(HasExpressionOrMemberInitOrQuerySet(obj, arg0, arg1, arg2)) return new ExpressionAccessOperator(call, obj, arg0, arg1, arg2);
				return new FunctionOperator(FunctionOperatorType.CharIndex, arg0, obj, arg1, arg2);
			} else if(call.Arguments.Count == 2) {
				CriteriaOperator arg1 = ParseExpression(call.Arguments[1]);
				if(HasExpressionOrMemberInitOrQuerySet(obj, arg0, arg1)) return new ExpressionAccessOperator(call, obj, arg0, arg1);
				return new FunctionOperator(FunctionOperatorType.CharIndex, arg0, obj, arg1);
			}
			if(HasExpressionOrMemberInitOrQuerySet(obj, arg0)) return new ExpressionAccessOperator(call, obj, arg0);
			return new FunctionOperator(FunctionOperatorType.CharIndex, arg0, obj);
		}
		CriteriaOperator FunctionUniversal(MethodCallExpression call, FunctionOperatorType fType, bool obj, int minCount, int maxCount){
			bool createExpressionAccess = false;
			bool insertFirstNull = true;
			CriteriaOperator objOperand = null;
			if(call.Object != null) {
				objOperand = ParseExpression(call.Object);
				insertFirstNull = false;
				if(HasExpressionOrMemberInitOrQuerySet(objOperand))
					createExpressionAccess = true;
			}
			List<CriteriaOperator> operands = new List<CriteriaOperator>(call.Arguments.Count);
			for(int i = 0; i < call.Arguments.Count; i++) {
				CriteriaOperator operand = ParseExpression(call.Arguments[i]);
				if(HasExpressionOrMemberInitOrQuerySet(operand))
					createExpressionAccess = true;
				operands.Add(operand);
			}
			if(createExpressionAccess) 
				return new ExpressionAccessOperator(call, insertFirstNull, operands.ToArray());
			if(operands.Count < minCount)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0WithSoManyParametersIsNotSupported, call.Method.Name));
			if(operands.Count > maxCount) {
				operands.RemoveRange(maxCount, operands.Count - maxCount);
			}
			if(obj)
				operands.Insert(0, objOperand);
			return new FunctionOperator(fType, operands);
		}
		CriteriaOperator Select(MethodCallExpression call) {
			CriteriaOperator coParent = ParseExpression(call.Arguments[0]);
			QuerySet parent = coParent as QuerySet;
			if(IsNull(parent)) {
				if(HasExpressionOrMemberInitOrQuerySet(coParent) && call.Arguments.Count == 2) {					
					return new ExpressionAccessOperator(call, call.Object == null ? true : false, coParent, new ExpressionAccessOperator(call.Arguments[1]));
				}
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[0], "Select"));
			}
			if(call.Arguments[0].NodeType != ExpressionType.Constant && call.Arguments.Count == 2 && HasExpressionOrMemberInitOrQuerySet(coParent)) {
				return new ExpressionAccessOperator(call, call.Object == null ? true : false, coParent, new ExpressionAccessOperator(call.Arguments[1]));
			}
			QuerySet newSub = new QuerySet(parent.Property, parent.Condition);
			CriteriaOperator op = ParseExpression(parent.Property, call.Arguments[1], parent);
			MemberInitOperator init = op as MemberInitOperator;
			QuerySet set = op as QuerySet;
			FreeQuerySet freeSet = op as FreeQuerySet;
			if(!IsNull(freeSet))
				return freeSet;
			if(!IsNull(set)) {
				init = set.Projection;
			}
			if(IsNull(init) && !IsNull(op)) {
				CriteriaOperator opForProjection = null;
				if(IsNull(set)) {
					opForProjection = op;
				} else if(!IsNull(set.Property)) {
					opForProjection = set;
				}
				if(!IsNull(opForProjection)) {
					XPMemberAssignmentCollection members = new XPMemberAssignmentCollection();
					members.Add(new XPMemberAssignment(opForProjection));
					if(call.Object == null) init = new MemberInitOperator((Type)null, members, false);
					else
						init = new MemberInitOperator(call.Object.Type, members, false);
				}
			}
			newSub.Projection = init;
			return newSub;
		}
		CriteriaOperator Where(MethodCallExpression call) {
			CriteriaOperator co = ParseExpression(call.Arguments[0]);
			QuerySet parent = co as QuerySet;
			if(IsNull(parent)) {
				OperandProperty op = co as OperandProperty;
				if(IsNull(op))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[0], "the Where clause"));
				parent = new QuerySet(op, null);
			}
			FreeQuerySet freeParent = parent as FreeQuerySet;
			if(IsNull(freeParent)) {
				CriteriaOperator criteria = ParseExpression(parent.Property, call.Arguments[1], parent is GroupSet ? new GroupSet(parent.Projection, ((GroupSet)parent).Key) : new QuerySet(parent.Projection));
				if(HasExpressionAccess(criteria))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, GetCauseStringOfExpressionAccess(criteria), "the Where clause"));
				if(IsNull(parent.Condition)) {
					return new QuerySet(parent.Property, criteria);					
				}
				return new QuerySet(parent.Property, GroupOperator.And(parent.Condition, criteria));
			} else {
				CriteriaOperator additionalCriteria = ParseExpression(freeParent.JoinType, call.Arguments[1], new QuerySet(freeParent.Projection));
				if(HasExpressionAccess(additionalCriteria))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, GetCauseStringOfExpressionAccess(additionalCriteria), "the Where clause"));
				if(IsNull(additionalCriteria)) {
					return new FreeQuerySet(freeParent.JoinType, freeParent.Condition);
				}
				return new FreeQuerySet(freeParent.JoinType, GroupOperator.And(freeParent.Condition, additionalCriteria));
			}
		}
		CriteriaOperator CallDefault(MethodCallExpression call) {
			CriteriaOperator op = CallCustomCriteria(call);
			if (!IsNull(op)) return op;
			op = CallCustomFunction(call);
			if (!IsNull(op)) return op;
			bool createExpressionAccessOperator = false;
			List<CriteriaOperator> arguments = new List<CriteriaOperator>();
			for (int i = 0; i < call.Arguments.Count; i++) {
				op = ParseExpression(call.Arguments[i]);
				arguments.Add(op);
				if(!(op is OperandValue)) {
					createExpressionAccessOperator = true;
				}
			}
			op = call.Object != null ? ParseExpression(call.Object) : null;
			arguments.Insert(0, op);
			if(!createExpressionAccessOperator) {
				if(op is OperandValue || IsNull(op))
					return new ConstantCompiler(Dictionary, call);
			}
			return new ExpressionAccessOperator(call, arguments.ToArray());
		}
		CriteriaOperator CallCustomFunction(MethodCallExpression call) {
			FunctionOperatorType customFunctionType;
			ICustomFunctionOperator customFunction = Dictionary.CustomFunctionOperators.GetCustomFunction(call.Method.Name);
			if(!IsValidCustomFunctionQueryable(call, customFunction, out customFunctionType)) {
				if(CriteriaOperator.CustomFunctionCount > 0) {
					customFunction = CriteriaOperator.GetCustomFunction(call.Method.Name);
					if(!IsValidCustomFunctionQueryable(call, customFunction, out customFunctionType)) {
						return null;
					}
				} else {
					return null;
				}
			}
			List<CriteriaOperator> operands = new List<CriteriaOperator>();
			operands.Add(new OperandValue(customFunction.Name));
			bool insertFirstNull = true;
			if(call.Object != null) {
				operands.Add(ParseExpression(call.Object));
				insertFirstNull = false;
			}
			for (int i = 0; i < call.Arguments.Count; i++) {
				operands.Add(ParseExpression(call.Arguments[i]));
			}
			CriteriaOperator[] operandsArray = operands.ToArray();
			if(HasExpressionOrMemberInitOrQuerySet(operandsArray))
				return new ExpressionAccessOperator(call, insertFirstNull, operandsArray);
			return new FunctionOperator(customFunctionType, operandsArray);
		}
		static bool IsValidCustomFunctionQueryable(MethodCallExpression call, ICustomFunctionOperator customFunction, out FunctionOperatorType customFunctionType) {
			customFunctionType = FunctionOperatorType.Custom;
			if(customFunction == null) return false;
			if((customFunction is ICustomFunctionOperatorQueryable) && ReferenceEquals(((ICustomFunctionOperatorQueryable)customFunction).GetMethodInfo(), call.Method)) {
				if(customFunction is ICustomNonDeterministicFunctionOperatorQueryable)
					customFunctionType = FunctionOperatorType.CustomNonDeterministic;
				return true;
			}
			return false;
		}
		CriteriaOperator CallCustomCriteria(MethodCallExpression call) {
			ICustomCriteriaOperatorQueryable customCriteria = null;
			if(customCriteriaCollection != null) {
				customCriteria = customCriteriaCollection.GetItem(call.Method);
			}
			if(customCriteria == null) {
				if(CustomCriteriaManager.RegisteredCriterionCount > 0) {
					customCriteria = CustomCriteriaManager.GetCriteria(call.Method);
					if(customCriteria == null) {
						return null;
					}
				} else {
					return null;
				}
			}
			List<CriteriaOperator> operands = new List<CriteriaOperator>();
			bool insertFirstNull = true;
			if(call.Object != null) {
				operands.Add(ParseExpression(call.Object));
				insertFirstNull = false;
			}
			for(int i = 0; i < call.Arguments.Count; i++) {
				operands.Add(ParseExpression(call.Arguments[i]));
			}
			CriteriaOperator[] operandsArray = operands.ToArray();
			if(HasExpressionOrMemberInitOrQuerySet(operandsArray))
				return new ExpressionAccessOperator(call, insertFirstNull, operandsArray);
			return customCriteria.GetCriteria(operandsArray);
		}
		CriteriaOperator LogicalBinary(BinaryExpression expression, BinaryOperatorType type) {
			if (IsVBStringCompare(expression))
				return CompareVB((MethodCallExpression)expression.Left, type);
			return Binary(expression, type);
		}
		int forceExpressionOperatorForConvertion = 0;
		public static bool IsImplementsInterface(Type classType, Type interfaceType) {
			return interfaceType.IsInterface && classType.GetInterfaces().Any(i => i == interfaceType);
		}
		CriteriaOperator Convert(UnaryExpression expression) {
			CriteriaOperator res = ParseExpression(expression.Operand);
			if(HasExpressionAccess(res))
				return new ExpressionAccessOperator(expression, res);
			if (expression.Operand.Type == typeof(char) && expression.Type == typeof(int)) {
				return new FunctionOperator(FunctionOperatorType.Ascii, res);
			}
			Type t = Nullable.GetUnderlyingType(expression.Type) ?? expression.Type;
			Type tObject = Nullable.GetUnderlyingType(expression.Operand.Type) ?? expression.Operand.Type;
			if(t == tObject) {
				return res;
			}
			if(IsImplementsInterface(t, tObject) || IsImplementsInterface(tObject, t)) {
				return res;
			}
			if(forceExpressionOperatorForConvertion > 0) {
				return new ExpressionAccessOperator(expression, res);
			}
			if((tObject.IsEnum && t == typeof(int)) || (res is ConstantValue && ((ConstantValue)res).Value == null)) {
				return res;
			}
			switch (Type.GetTypeCode(t)) {				
				case TypeCode.Int32:
					return new FunctionOperator(FunctionOperatorType.ToInt, res);
				case TypeCode.Int64:
					return new FunctionOperator(FunctionOperatorType.ToLong, res);
				case TypeCode.Single:
					return new FunctionOperator(FunctionOperatorType.ToFloat, res);
				case TypeCode.Double:
					return new FunctionOperator(FunctionOperatorType.ToDouble, res);
				case TypeCode.Decimal:
					return new FunctionOperator(FunctionOperatorType.ToDecimal, res);			   
			}
			OperandProperty prop = res as OperandProperty;
			QuerySet set = res as QuerySet;
			if(!IsNull(prop)) {
				if(Dictionary.QueryClassInfo(expression.Type) != null && !expression.Type.IsAssignableFrom(expression.Operand.Type)) {
					prop.PropertyName = prop.PropertyName + ".<" + expression.Type.Name + ">";
				}
			} else if(!IsNull(set)) {
				if(set.IsEmpty) {
					if(Dictionary.QueryClassInfo(expression.Type) != null && !expression.Type.IsAssignableFrom(expression.Operand.Type)) {
						res = new OperandProperty(string.Concat("<", expression.Type.Name, ">"));
					}
				} else if(IsNull(set.Projection)) {
					if(Dictionary.QueryClassInfo(expression.Type) != null && !expression.Type.IsAssignableFrom(expression.Operand.Type)) {
						var newSet = (QuerySet)((ICloneable)res).Clone();
						newSet.Projection = new MemberInitOperator(null, new XPMemberAssignmentCollection(){
							new XPMemberAssignment(new OperandProperty(string.Concat("<", expression.Type.Name, ">")))
						}, false);
						return newSet;
					}
				}
			}
			return res;
		}
		CriteriaOperator NewArrayInit(NewArrayExpression expression) {
			XPMemberAssignmentCollection members = new XPMemberAssignmentCollection();
			for (int i = 0; i < expression.Expressions.Count; i++) {
				Expression e = expression.Expressions[i];
				members.Add(new XPMemberAssignment(ConvertStructAccessIfNeeded(e, ParseExpression(e))));
			}
			return new MemberInitOperator(expression.Type, members, false);
		}
		CriteriaOperator New(NewExpression expression) {
			if(typeof(XPQueryBase).IsAssignableFrom(expression.Type)) {
				Type xpQueryType = expression.Type;
				if(xpQueryType.IsGenericType) {
					return new FreeQuerySet(xpQueryType.GetGenericArguments()[0], null);
				}
				Expression<Func<XPQueryBase>> ex = LambdaExpression.Lambda<Func<XPQueryBase>>(expression);
				Func<XPQueryBase> getXPQuery = ex.Compile();
				XPQueryBase related = getXPQuery();
				return new FreeQuerySet(((IQueryable)related).ElementType, null);
			}
			XPMemberAssignmentCollection members = new XPMemberAssignmentCollection();
			bool isConstant = true;
			for (int i = 0; i < expression.Arguments.Count; i++) {
				Expression e = expression.Arguments[i];
				CriteriaOperator exp = ParseExpression(e);
				if (!(exp is OperandValue))
					isConstant = false;
				exp = ConvertStructAccessIfNeeded(e, exp);
				members.Add(expression.Members == null ? new XPMemberAssignment(exp) : new XPMemberAssignment(expression.Members[i], exp));
			}
			if (isConstant)
				return new ConstantCompiler(Dictionary, expression);
			return new MemberInitOperator(true, members, expression.Constructor);
		}
		bool IsStructType(Type type) {
			return type.IsValueType && !type.IsEnum && !type.IsPrimitive && !type.IsGenericType && type != typeof(decimal) && type != typeof(DateTime) && type != typeof(Guid) && type != typeof(TimeSpan);
		}
		CriteriaOperator ConvertStructAccessIfNeeded(Expression expression, CriteriaOperator criteria) {
			if((expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked) && expression.Type == typeof(object) && !HasExpressionAccess(criteria)) {
				UnaryExpression ue = ((UnaryExpression)expression);
				CriteriaOperator result = ConvertStructAccessIfNeeded(ue.Operand, criteria);
				if(ReferenceEquals(result, criteria) && (ue.Operand.NodeType == ExpressionType.Convert || ue.Operand.NodeType == ExpressionType.ConvertChecked)) {
					Interlocked.Increment(ref forceExpressionOperatorForConvertion);
					try {
						return ParseExpression(ue);
					} finally {
						Interlocked.Decrement(ref forceExpressionOperatorForConvertion);
					}
				}
				return new ExpressionAccessOperator(expression, result);
			} else
				if(!HasExpressionAccess(criteria) && IsStructType(expression.Type)) {
					Type structType = expression.Type;
					XPMemberAssignmentCollection structMembers = new XPMemberAssignmentCollection();
					foreach(MemberInfo mi in structType.GetFields(BindingFlags.Instance | BindingFlags.Public)) {
						structMembers.Add(new XPMemberAssignment(mi, MemberAccess(Expression.MakeMemberAccess(expression, mi), criteria, classInfo)));
					}
					foreach(MemberInfo mi in structType.GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
						if(((PropertyInfo)mi).CanWrite) {
							structMembers.Add(new XPMemberAssignment(mi, MemberAccess(Expression.MakeMemberAccess(expression, mi), criteria, classInfo)));
						}
					}
					criteria = new MemberInitOperator(structType, structMembers, true);
				}
			return criteria;
		}
		CriteriaOperator MemberInit(MemberInitExpression expression) {
			XPMemberAssignmentCollection members = new XPMemberAssignmentCollection();
			foreach (MemberBinding b in expression.Bindings) {
				if (b.BindingType == MemberBindingType.Assignment) {
					MemberAssignment ma = (MemberAssignment)b;
					CriteriaOperator memberOperator = ParseExpression(ma.Expression);
					members.Add(new XPMemberAssignment(ma.Member, ConvertStructAccessIfNeeded(ma.Expression, memberOperator)));
				}
			}
			if(expression.NewExpression.Constructor == null)
				return new MemberInitOperator(expression.NewExpression.Type, members, true);
			return new MemberInitOperator(false, members, expression.NewExpression.Constructor);
		}
		CriteriaOperator ParseObjectExpression(Expression expression) {
			CriteriaOperator result = ParseExpression(expression);
			if(HasExpressionAccess(result)) 
				return result;
			QuerySet set = result as QuerySet;
			if (IsNull(set))
				return result;
			if(!IsNull(set.Property)) {
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupported, expression));
			}
			CriteriaOperator propertyFromProjection = null;
			if(!IsNull(set.Projection)) {
				MemberInitOperator mio = set.Projection;
				if(!IsNull(mio) && !mio.CreateNewObject && mio.Members.Count == 1) {
					propertyFromProjection = mio.Members[0].Property;
				}
			}
			FreeQuerySet freeSet = set as FreeQuerySet;
			if(!IsNull(freeSet)) {
				if(IsNull(set.Projection)) {
					return freeSet.CreateJoinOperand(freeSet.Condition, new OperandProperty("This"), Aggregate.Single);
				} else {
					if(!IsNull(propertyFromProjection))
						return freeSet.CreateJoinOperand(freeSet.Condition, propertyFromProjection, Aggregate.Single);
				}
			}
			return IsNull(propertyFromProjection) ? ThisCriteria : propertyFromProjection;
		}
		CriteriaOperator NotEqual(BinaryExpression expression) {
			if (IsVBStringCompare(expression))
				return CompareVB((MethodCallExpression)expression.Left, BinaryOperatorType.NotEqual);
			CriteriaOperator right = ParseObjectExpression(expression.Right);
			bool rightIsExpressionAccess = HasExpressionOrMemberInitOrQuerySet(right);
			CriteriaOperator detectCompareResult;
			if(BinaryDetectCompare(expression, right, BinaryOperatorType.NotEqual, rightIsExpressionAccess, out detectCompareResult))
				return detectCompareResult;
			CriteriaOperator left = ParseObjectExpression(expression.Left);
			if(rightIsExpressionAccess || HasExpressionOrMemberInitOrQuerySet(left))
				return new ExpressionAccessOperator(expression, left, right);
			ConstantValue c = right as ConstantValue;
			if (!IsNull(c) && c.Value == null)
				return new NotOperator(new NullOperator(left));
			return new BinaryOperator(left, right, BinaryOperatorType.NotEqual);
		}
		CriteriaOperator CompareVB(MethodCallExpression expression, BinaryOperatorType type) {
			CriteriaOperator left = ParseObjectExpression(expression.Arguments[0]);
			CriteriaOperator right = ParseObjectExpression(expression.Arguments[1]);
			if(HasExpressionOrMemberInitOrQuerySet(left, right)) return new ExpressionAccessOperator(expression, left, right);
			return new BinaryOperator(left, right, type);
		}
		CriteriaOperator Parameter(ParameterExpression expression) {
			CriteriaOperator val;
			if(!parameters.TryGetValue(expression, out val))
				return new ExpressionAccessOperator(expression);
			if(Resolver != null) {
				QuerySet q = val as QuerySet;
				if(!q.IsEmpty)
					Resolver.SetUsed();
			}
			return val;
		}
		CriteriaOperator Equal(BinaryExpression expression) {
			if (IsVBStringCompare(expression))
				return CompareVB((MethodCallExpression)expression.Left, BinaryOperatorType.Equal);
			CriteriaOperator right = ParseObjectExpression(expression.Right);
			bool rightIsExpressionAccess = HasExpressionOrMemberInitOrQuerySet(right);
			CriteriaOperator result;
			if(BinaryDetectCompare(expression, right, BinaryOperatorType.Equal, rightIsExpressionAccess, out result))
				return result;
			CriteriaOperator left = ParseObjectExpression(expression.Left);
			if(rightIsExpressionAccess || HasExpressionOrMemberInitOrQuerySet(left)) 
				return new ExpressionAccessOperator(expression, left, right);
			ConstantValue c = right as ConstantValue;
			if (!IsNull(c) && c.Value == null)
				return new NullOperator(left);
			return new BinaryOperator(left, right, BinaryOperatorType.Equal);
		}
		CriteriaOperator MemberAccess(MemberExpression expression) {
			CriteriaOperator parent = null;
			if(typeof(XPQueryBase).IsAssignableFrom(expression.Type)) {
				Expression<Func<XPQueryBase>> ex = LambdaExpression.Lambda<Func<XPQueryBase>>(expression);
				Func<XPQueryBase> getXPQuery = ex.Compile();
				XPQueryBase related = getXPQuery();
				return XPQueryBase.GetFreeQuerySet(related);
			}
			if(expression.Expression != null)
				parent = ParseExpression(expression.Expression);
			return MemberAccess(expression, parent, classInfo);
		}
		CriteriaOperator MemberAccess(MemberExpression expression, CriteriaOperator parent, XPClassInfo currentClassInfo) {
			if(HasExpressionAccess(parent)) return new ExpressionAccessOperator(expression, parent);
			JoinOperand joinOperandParent = parent as JoinOperand;
			if(!IsNull(joinOperandParent) && joinOperandParent.AggregateType == Aggregate.Single) {
				XPClassInfo joinClassInfo = null;
				if(!MemberInfoCollection.TryResolveTypeAlsoByShortName(joinOperandParent.JoinTypeName, classInfo, out joinClassInfo)) {
					throw new DevExpress.Xpo.Exceptions.CannotResolveClassInfoException(string.Empty, joinOperandParent.JoinTypeName);
				}
				CriteriaOperator joinExpression = MemberAccess(expression, joinOperandParent.AggregatedExpression, joinClassInfo);
				return new JoinOperand(joinOperandParent.JoinTypeName, joinOperandParent.Condition, joinOperandParent.AggregateType, joinExpression);
			}
			OperandValue constant = parent as OperandValue;
			if (!IsNull(constant)) {
				if(expression.Expression is ConstantExpression) {
					object value;
					if(Resolver != null && Resolver.MemeberAccessOperator(expression, out value))
						return new ParameterOperandValue(value);
					return new MemeberAccessOperator(expression);
				}
				if(constant is ParameterOperandValue) {
					ParameterOperandValue parameter = (ParameterOperandValue)constant;
					Func<object, object> getter = MemeberAccessOperator.GetExpression(expression.Member, expression.Expression.Type);
					Func<object, object> prev = parameter.Getter;
					return new ParameterOperandValue(parameter.BaseValue, v => getter(prev(v)));
				}
				return new ConstantCompiler(Dictionary, expression);
			}
			MemberInfo member = expression.Member;
			string name = member.Name;
			bool canCreateConstantCompiler = false;
			PropertyInfo pi = member as PropertyInfo;
			FieldInfo fi = member as FieldInfo;
			if ((pi != null && pi.GetGetMethod(true).IsStatic) || (fi != null && fi.IsStatic)) {
				canCreateConstantCompiler = true;
			}
			QuerySet set = parent as QuerySet;
			GroupSet group = parent as GroupSet;
			if (member.DeclaringType == typeof(DateTime)) {
				CriteriaOperator dateTimeResult;
				if(AccessDateTime(parent, name, out dateTimeResult)) {
					return dateTimeResult;
				}
				if(!canCreateConstantCompiler) throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupported, expression));
			}
			if(canCreateConstantCompiler) {
				return new ConstantCompiler(Dictionary, expression);
			}
			if (member.DeclaringType == typeof(string)) {
				return AccessString(parent, name);
			}
			if ((member.DeclaringType.IsGenericType && member.DeclaringType.GetGenericTypeDefinition() == typeof(ICollection<>))
#if !SL
				|| member.DeclaringType == typeof(XPBaseCollection)
#endif
			) {
				if (IsNull(set))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupported, expression));
				if(name == "Count") {
					FreeQuerySet freeSet = set as FreeQuerySet;
					if(IsNull(freeSet))
						return new AggregateOperand(set.Property, null, Aggregate.Count, set.Condition);
					else
						return freeSet.CreateJoinOperand(freeSet.Condition, null, Aggregate.Count);
				}
			}
			if (member.DeclaringType.IsGenericType && member.DeclaringType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
				return AccessNullable(parent, name);
			}
			FreeQuerySet fSet = parent as FreeQuerySet;
			if(!IsNull(fSet)) {
				if(IsNull(fSet.Projection))
					return fSet.CreateJoinOperand(fSet.Condition, new OperandProperty(name), Aggregate.Single);
				if(!fSet.Projection.CreateNewObject && fSet.Projection.Members.Count == 1)
					return fSet.CreateJoinOperand(fSet.Condition, MemberAccess(expression, fSet.Projection.Members[0].Property, Dictionary.GetClassInfo(fSet.JoinType)), Aggregate.Single);
			}
			MemberInitOperator init = IsNull(set) ? parent as MemberInitOperator : set.Projection;
			if (!IsNull(group) && name == "Key")
				return group.Key;
			if (!IsNull(init)) {
				foreach (XPMemberAssignment m in init.Members) {
					if (m.MemberName == null)
						continue;
					MemberInfo mi = m.GetMember(member.DeclaringType);
					if (mi == member || (pi != null && mi == pi.GetGetMethod()))
						return m.Property;
					if(mi.DeclaringType == member.DeclaringType && mi.ReflectedType != member.ReflectedType && mi.Name == member.Name
						&& (mi.ReflectedType.IsAssignableFrom(member.ReflectedType) || member.ReflectedType.IsAssignableFrom(mi.ReflectedType))) {
						return m.Property;
					}
				}
				if(!init.CreateNewObject && init.Members.Count == 1) {
					return MemberAccess(expression, init.Members[0].Property, currentClassInfo);
				}
				return new ExpressionAccessOperator(expression, init);
			}
			OperandProperty parentProp = IsNull(set) ? parent as OperandProperty : set.Property;
			if (!IsNull(parentProp)) {
				if (parentProp.PropertyName[parentProp.PropertyName.Length - 1] == '>')
					name = parentProp.PropertyName + name;
				else
					name = string.Concat(parentProp.PropertyName, ".", name);
			} else {
				if(!IsNull(parent) && IsNull(set)) {
					FunctionOperator func = parent as FunctionOperator;					
					if(!ReferenceEquals(func, null) && func.OperatorType == FunctionOperatorType.Iif && func.Operands.Count == 3) {
						if(func.Operands[1] is OperandValue && ((OperandValue)func.Operands[1]).Value == null) {
							CriteriaOperator nestedResult = MemberAccess(expression, func.Operands[2], currentClassInfo);
							return new FunctionOperator(FunctionOperatorType.Iif, func.Operands[0], func.Operands[1], nestedResult);
						}
						if(func.Operands[2] is OperandValue && ((OperandValue)func.Operands[2]).Value == null) {
							CriteriaOperator nestedResult = MemberAccess(expression, func.Operands[1], currentClassInfo);
							return new FunctionOperator(FunctionOperatorType.Iif, func.Operands[0], nestedResult, func.Operands[2]);
						}
					}
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupported, expression));
				}
			}
			if(name.StartsWith("This.")) name = name.Substring(5);
			else name = name.Replace(".This.", ".");
			if (name[0] != '^') {
				MemberInfoCollection col = MemberInfoCollection.ParsePath(currentClassInfo, name);
				if (col[col.Count - 1].IsAssociationList)
					return new QuerySet(name);
			}
			return new OperandProperty(name);
		}
		static bool AccessDateTime(CriteriaOperator parent, string name, out CriteriaOperator result) {
			switch (name) {
				case "Date":
					result = new FunctionOperator(FunctionOperatorType.GetDate, parent);
					return true;
				case "Millisecond":
					result = new FunctionOperator(FunctionOperatorType.GetMilliSecond, parent);
					return true;
				case "Second":
					result = new FunctionOperator(FunctionOperatorType.GetSecond, parent);
					return true;
				case "Minute":
					result = new FunctionOperator(FunctionOperatorType.GetMinute, parent);
					return true;
				case "Hour":
					result = new FunctionOperator(FunctionOperatorType.GetHour, parent);
					return true;
				case "Day":
					result = new FunctionOperator(FunctionOperatorType.GetDay, parent);
					return true;
				case "Month":
					result = new FunctionOperator(FunctionOperatorType.GetMonth, parent);
					return true;
				case "Year":
					result = new FunctionOperator(FunctionOperatorType.GetYear, parent);
					return true;
				case "DayOfWeek":
					result = new FunctionOperator(FunctionOperatorType.GetDayOfWeek, parent);
					return true;
				case "DayOfYear":
					result = new FunctionOperator(FunctionOperatorType.GetDayOfYear, parent);
					return true;
				case "TimeOfDay":
					result = new FunctionOperator(FunctionOperatorType.GetTimeOfDay, parent);
					return true;
				case "Now":
					result = new FunctionOperator(FunctionOperatorType.Now);
					return true;
				case "UtcNow":
					result = new FunctionOperator(FunctionOperatorType.UtcNow);
					return true;
				case "Today":
					result = new FunctionOperator(FunctionOperatorType.Today);
					return true;
				default:
					result = null;
					return false;
			}
		}
		private static CriteriaOperator AccessNullable(CriteriaOperator parent, string name) {
			switch(name) {
				case "Value":
					return parent;
				case "HasValue":
					return new NotOperator(new NullOperator(parent));
				default:
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_MethodX0ForX1IsNotSupported, name, "nullable type"));
			}
		}
		private static CriteriaOperator AccessString(CriteriaOperator parent, string name) {
			switch (name) {
				case "Length":
					return new FunctionOperator(FunctionOperatorType.Len, parent);
				default:
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_MethodX0ForX1IsNotSupported, name, "string"));
			}
		}
		CriteriaOperator AggregateCall(MethodCallExpression call, Aggregate type) {
			CriteriaOperator co = ParseExpression(call.Arguments[0]);;
			QuerySet col = co as QuerySet;
			if(IsNull(col)) {
				OperandProperty prop = co as OperandProperty;
				if(IsNull(prop))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[0], call.Method.Name));
				col = new QuerySet(prop, null);
			}
			FreeQuerySet freeSet = col as FreeQuerySet;
			if(IsNull(freeSet)) {
				CriteriaOperator expression;
				if(call.Arguments.Count == 2)
					expression = ParseExpression(col.Property, call.Arguments[1], new QuerySet(col.Projection));
				else {
					if(IsNull(col.Projection) || col.Projection.CreateNewObject)
						throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[0], call.Method.Name));
					expression = col.Projection.Members[0].Property;
				}
				if(HasExpressionAccess(expression))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, GetCauseStringOfExpressionAccess(expression), call.Method.Name));
				return new AggregateOperand(col.Property, expression, type, col.Condition);
			} else {
				CriteriaOperator expression;
				if(call.Arguments.Count == 2)
					expression = ParseExpression(freeSet.JoinType, call.Arguments[1], QuerySet.Empty);
				else throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0WithSoManyParametersIsNotSupported, call.Method.Name));
				if(HasExpressionAccess(expression))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, GetCauseStringOfExpressionAccess(expression), call.Method.Name));
				return freeSet.CreateJoinOperand(freeSet.Condition, expression, type);
			}
		}
		CriteriaOperator Sum(MethodCallExpression call) {
			return AggregateCall(call, Aggregate.Sum);
		}
		CriteriaOperator Average(MethodCallExpression call) {
			return AggregateCall(call, Aggregate.Avg);
		}
		CriteriaOperator Min(MethodCallExpression call) {
			return AggregateCall(call, Aggregate.Min);
		}
		CriteriaOperator Max(MethodCallExpression call) {
			return AggregateCall(call, Aggregate.Max);
		}
		CriteriaOperator All(MethodCallExpression call) {
			CriteriaOperator co = ParseExpression(call.Arguments[0]);
			QuerySet col = co as QuerySet;
			if(IsNull(col)) {
				OperandProperty op = co as OperandProperty;
				if(IsNull(op))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[0], "the All method call")); ;
				col = new QuerySet(op, null);
			}
			FreeQuerySet freeCol = col as FreeQuerySet;
			if(call.Arguments.Count == 2) {
				if(!IsNull(freeCol)) {
					return new NotOperator(freeCol.CreateJoinOperand(GroupOperator.And(new NotOperator(ParseExpression(freeCol.JoinType, call.Arguments[1], new QuerySet(freeCol.Projection))), freeCol.Condition),
						null, Aggregate.Exists));					
				}
				if(!IsNull(col))
					return new NotOperator(new ContainsOperator(col.Property, new NotOperator(GroupOperator.And(ParseExpression(col.Property, call.Arguments[1], new QuerySet(col.Projection))))));
			}
			throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[0], "the All method call"));
		}
		CriteriaOperator Any(MethodCallExpression call) {
			CriteriaOperator co = ParseExpression(call.Arguments[0]);
			QuerySet col = co as QuerySet;
			if(IsNull(col)) {
				OperandProperty op = co as OperandProperty;
				if(IsNull(op))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[0], "the Any method call"));
				col = new QuerySet(op, null);
			}
			FreeQuerySet freeCol = col as FreeQuerySet;
			switch(call.Arguments.Count) {
				case 2:
					if(!IsNull(freeCol)) {
						return freeCol.CreateJoinOperand(GroupOperator.And(ParseExpression(freeCol.JoinType, call.Arguments[1], new QuerySet(freeCol.Projection)), freeCol.Condition), null, Aggregate.Exists);
					}
					if(!IsNull(col))
						if(IsNull(col.Condition))
							return new ContainsOperator(col.Property, GroupOperator.And(ParseExpression(col.Property, call.Arguments[1], new QuerySet(col.Projection))));
						else
							return new ContainsOperator(col.Property, GroupOperator.And(col.Condition, GroupOperator.And(ParseExpression(col.Property, call.Arguments[1], new QuerySet(col.Projection)))));
					break;
				case 1:
					if(!IsNull(freeCol)) {
						return freeCol.CreateJoinOperand(freeCol.Condition, null, Aggregate.Exists);
					}
					if(!IsNull(col))
						return new ContainsOperator(col.Property, col.Condition);
					break;
				default:
					break;
			}
			throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[0], "the Any method call"));
		}
		CriteriaOperator Contains(MethodCallExpression call) {
			if (call.Arguments.Count != 2)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_X0WithSoManyParametersIsNotSupported, "Contains operator"));
			if (call.Arguments[0].NodeType == ExpressionType.NewArrayInit) {
				List<CriteriaOperator> ops = new List<CriteriaOperator>();
				foreach (Expression e in ((NewArrayExpression)call.Arguments[0]).Expressions)
					ops.Add(ParseExpression(e));
				return new InOperator(ParseObjectExpression(call.Arguments[1]), ops);
			}
			CriteriaOperator inList = ParseExpression(call.Arguments[0]);
			FreeQuerySet freePath = inList as FreeQuerySet;
			if(!IsNull(freePath)) {
				return freePath.CreateJoinOperand(GroupOperator.And(freePath.Condition, new BinaryOperator(ThisCriteria, ParseExpression(freePath.JoinType, call.Arguments[1]), BinaryOperatorType.Equal)), null, Aggregate.Exists);
			}
			QuerySet col = inList as QuerySet;
			if (!IsNull(col))
				return new ContainsOperator(col.Property, GroupOperator.And(col.Condition, new BinaryOperator(ThisCriteria, ParseExpression(call.Arguments[1]), BinaryOperatorType.Equal)));
			else {
				OperandValue constantList = inList as OperandValue;
				if (IsNull(constantList))
					throw new NotSupportedException(); 
				return new InOperatorCompiler(Dictionary, ParseObjectExpression(call.Arguments[1]), constantList);
			}
		}
		CriteriaOperator ListContains(MethodCallExpression call) {
			CriteriaOperator inList = ParseExpression(call.Object);
			QuerySet col = inList as QuerySet;
			if (!IsNull(col))
				return new ContainsOperator(col.Property, GroupOperator.And(col.Condition, new BinaryOperator(ThisCriteria, ParseExpression(call.Arguments[0]), BinaryOperatorType.Equal)));
			else {
				OperandValue constantList = inList as OperandValue;
				if (IsNull(constantList))
					throw new NotSupportedException(); 
				return new InOperatorCompiler(Dictionary, ParseObjectExpression(call.Arguments[0]), constantList);
			}
		}
		CriteriaOperator Count(MethodCallExpression call) {
			CriteriaOperator co = ParseExpression(call.Arguments[0]);
			QuerySet path = co as QuerySet;
			if(IsNull(path)) {
				OperandProperty op = co as OperandProperty;
				if(IsNull(op))
					throw new NotSupportedException(Res.GetString(Res.LinqToXpo_ExpressionX0IsNotSupportedInX1, call.Arguments[0], "the Count method call"));
				path = new QuerySet(op, null);
			}
			FreeQuerySet freePath = path as FreeQuerySet;
			if(IsNull(freePath)) {
				CriteriaOperator criteria = call.Arguments.Count > 1 ? ParseExpression(path.Property, call.Arguments[1], new QuerySet(path.Projection)) : null;
				return new AggregateOperand(path.Property, null, Aggregate.Count, GroupOperator.And(path.Condition, criteria));
			} else {
				return freePath.CreateJoinOperand(freePath.Condition, null, Aggregate.Count);
			}
		}
		Dictionary<ParameterExpression, CriteriaOperator> PrepareParametersForJoin() {
			return PatchParameters("^");
		}
		Dictionary<ParameterExpression, CriteriaOperator> PrepareParameters(Expression call, OperandProperty path, int upDepth) {
			string pathStr = String.Empty;
			if (!IsNull(path)) {
				for(int i = 0; i < upDepth; i++) {
					pathStr += string.IsNullOrEmpty(pathStr) ? "^" : ".^";
				}
				return PatchParameters(pathStr);
			} else
				return parameters;
		}
		Dictionary<ParameterExpression, CriteriaOperator> PatchParameters(string pathStr) {
			Dictionary<ParameterExpression, CriteriaOperator> newParameters = new Dictionary<ParameterExpression, CriteriaOperator>();
			foreach (KeyValuePair<ParameterExpression, CriteriaOperator> p in parameters) {
				CriteriaOperator parameter = PatchParameter(pathStr, p.Value);
				newParameters.Add(p.Key, parameter);
			}
			return newParameters;
		}
		static CriteriaOperator PatchParameter(string pathStr, CriteriaOperator p) {
			MemberInitOperator init = p as MemberInitOperator;
			CriteriaOperator parameter;
			if (IsNull(init)) {
				QuerySet set = p as QuerySet;
				if (IsNull(set)) {
					JoinOperand joinOperand = p as JoinOperand;
					if (IsNull(joinOperand)) {
						parameter = new OperandProperty(pathStr + (IsNull(p) ? String.Empty : "." + ((OperandProperty)p).PropertyName));
					} else {
						parameter = new JoinOperand(joinOperand.JoinTypeName, joinOperand.Condition, joinOperand.AggregateType, joinOperand.AggregatedExpression);
					}
				} else {
					FreeQuerySet freeSet = set as FreeQuerySet;
					if (!IsNull(freeSet)) {
						parameter = new FreeQuerySet(freeSet.JoinType, freeSet.Condition);
					} else {
						if (!IsNull(set.Condition))
							throw new NotSupportedException(); 
						QuerySet newSet = new QuerySet();
						if (!IsNull(set.Projection))
							newSet.Projection = (MemberInitOperator)PatchParameter(pathStr, set.Projection);
						if (!IsNull(set.Property))
							newSet.Property = new OperandProperty(string.Concat(pathStr, ".", set.Property.PropertyName));
						else
							newSet.Property = new OperandProperty(pathStr);
						parameter = newSet;
					}
				}
			} else {
				XPMemberAssignmentCollection list = new XPMemberAssignmentCollection();
				foreach (XPMemberAssignment ma in init.Members)
					list.Add(new XPMemberAssignment(ma, PatchParameter(pathStr, ma.Property)));
				parameter = new MemberInitOperator(init.DeclaringTypeAssemblyName, init.DeclaringTypeName, list, false);
			}
			return parameter;
		}
		bool IsVBStringCompare(BinaryExpression expression) {
			return expression.Left.NodeType == ExpressionType.Call && expression.Right.NodeType == ExpressionType.Constant && 0.Equals(((ConstantExpression)expression.Right).Value) && ((MethodCallExpression)expression.Left).Method.DeclaringType.FullName == "Microsoft.VisualBasic.CompilerServices.Operators" && ((MethodCallExpression)expression.Left).Method.Name == "CompareString";
		}
	}
	public class XPQuery<T> : XPQueryBase, IOrderedQueryable<T>, IQueryProvider {
		public XPQuery(IDataLayer dataLayer) : base(dataLayer, typeof(T)) { }
		public XPQuery(Session session) : base(session, typeof(T), false) { }
		public XPQuery(XPDictionary dictionary) : base(dictionary, typeof(T), false) { }
		public XPQuery(Session session, bool inTransaction) : base(session, typeof(T), inTransaction) { }
		public XPQuery(XPDictionary dictionary, bool inTransaction) : base(dictionary, typeof(T), inTransaction) { }
		XPQuery(XPQuery<T> baseQuery) : base(baseQuery) { }
		XPQuery(XPQuery<T> baseQuery, bool inTransaction) : base(baseQuery, inTransaction) { }
		XPQuery(XPQuery<T> baseQuery, CustomCriteriaCollection customCriteriaCollection) : base(baseQuery, customCriteriaCollection) { }
		XPQuery(Session session, IDataLayer dataLayer, XPDictionary dictionary) : base(session, dataLayer, dictionary) { }
		XPQuery(Session session, IDataLayer dataLayer, XPDictionary dictionary, string data) : base(session, dataLayer, dictionary, data) { }
		XPQuery(XPDictionary dictionary, string data) : base(dictionary, data) { }
		public static XPQuery<T> Deserialize(Session session, string data) {
			return new XPQuery<T>(session, null, session.Dictionary, data);
		}
		public static XPQuery<T> Deserialize(IDataLayer dataLayer, string data) {
			return new XPQuery<T>(null, dataLayer, dataLayer.Dictionary, data);
		}
		public static XPQuery<T> Deserialize(XPDictionary dictionary, string data) {
			return new XPQuery<T>(dictionary, data);
		}
		static CriteriaOperator[] ThisParams = new CriteriaOperator[] { Parser.ThisCriteria };
		public CriteriaOperator TransformExpression(Expression<Func<T, bool>> expression) {
			return TransformExpression(expression, null);
		}
		public CriteriaOperator TransformExpression(Expression<Func<T, bool>> expression, CustomCriteriaCollection customCriteriaCollection) {
			return TransformExpression(Dictionary, expression, customCriteriaCollection);
		}
		public static CriteriaOperator TransformExpression(Session session, Expression<Func<T, bool>> expression) {
			return TransformExpression(session, expression, null);
		}
		public static CriteriaOperator TransformExpression(XPDictionary dictionary, Expression<Func<T, bool>> expression, CustomCriteriaCollection customCriteriaCollection) {
			return Parser.ParseExpression(dictionary.GetClassInfo(typeof(T)), customCriteriaCollection, expression, ThisParams);
		}
		public static CriteriaOperator TransformExpression(Session session, Expression<Func<T, bool>> expression, CustomCriteriaCollection customCriteriaCollection) {
			return TransformExpression(session.Dictionary, expression, customCriteriaCollection);
		}
		IQueryable IQueryProvider.CreateQuery(Expression expression) {
			if(expression.NodeType != ExpressionType.Call)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheCallExpressionIsExpectedX0, expression));
			MethodCallExpression call = ((MethodCallExpression)expression);
			Type returnType = call.Method.ReturnType;
			if (returnType == null)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheCallExpressionReturnTypeX0, expression));
			Type[] genArguments = returnType.GetGenericArguments();
			if (genArguments.Length == 0)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheCallExpressionGenericReturnTypeX0, expression));
			return GetCreateQueryMethod(genArguments[0])(this, expression);
		}
		readonly static Dictionary<Type, QueryProviderEx.CreateQueryHandler> createQueryMethods = new Dictionary<Type, QueryProviderEx.CreateQueryHandler>();
		static QueryProviderEx.CreateQueryHandler GetCreateQueryMethod(Type type) {
			lock(createQueryMethods) {
				QueryProviderEx.CreateQueryHandler createQueryMethod;
				if(!createQueryMethods.TryGetValue(type, out createQueryMethod)) {
					MethodInfo mi = typeof(QueryProviderEx).GetMethod("CreateQuery").MakeGenericMethod(type);
					createQueryMethod = (QueryProviderEx.CreateQueryHandler)Delegate.CreateDelegate(typeof(QueryProviderEx.CreateQueryHandler), mi);
					createQueryMethods.Add(type, createQueryMethod);
				}
				return createQueryMethod;
			}
		}
		IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression) {
			XPQuery<S> col = new XPQuery<S>(Session, DataLayer, Dictionary);
			if(expression.NodeType != ExpressionType.Call)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheCallExpressionIsExpectedX0, expression));
			MethodCallExpression call = (MethodCallExpression)expression;
			if (call.Method.DeclaringType != typeof(Queryable) && call.Method.DeclaringType != typeof(Enumerable))
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_IncorrectDeclaringTypeX0InTheMethodCallQue, call.Method.DeclaringType));
			var arguments = call.Arguments;
			if(arguments[0].NodeType == ExpressionType.Call) {
				IQueryable q = (IQueryable)((IQueryProvider)this).CreateQuery(arguments[0]);
				List<Expression> newArguments = new List<Expression>(arguments);
				newArguments[0] = Expression.Constant(q);
				return q.Provider.CreateQuery<S>(Expression.Call(call.Method, newArguments.ToArray()));
			}
			ConstantExpression prevConst = arguments[0] as ConstantExpression;
			if(prevConst == null)
				throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheCallExpressionXPQueryX0, arguments[0]));
			XPQueryBase prevBase = prevConst.Value as XPQueryBase;
			XPQuery<T> prev = prevConst.Value as XPQuery<T>;			
			switch (call.Method.Name) {
				case "Union":
				case "Intersect": {
						if(prev == null)
							throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheCallExpressionXPQueryX0, arguments[0]));
						Type type = call.Method.GetGenericArguments()[0];
						XPQuery<T> next = ((ConstantExpression)arguments[1]).Value as XPQuery<T>;
						XPClassInfo newObjectClassInfo = Dictionary.QueryClassInfo(type);
						if (next == null || type != typeof(T) || newObjectClassInfo == null ||
							!prev.CanIntersect() || !next.CanIntersect())
							return prev.CallGeneric<S>(call);
						break;
					}
				case "Except":
				case "Concat":
					if(prev == null)
						throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheCallExpressionXPQueryX0, arguments[0]));
					return prev.CallGeneric<S>(call);
				case "Cast": {
						if(prev == null)
							throw new NotSupportedException(Res.GetString(Res.LinqToXpo_TheCallExpressionXPQueryX0, arguments[0]));
						Type type = call.Method.GetGenericArguments()[0];
						XPClassInfo newObjectClassInfo = Dictionary.QueryClassInfo(type);
						if (newObjectClassInfo == null || !prev.ObjectClassInfo.IsAssignableTo(newObjectClassInfo))
							return prev.CallGeneric<S>(call);
						break;
					}
				default:
					break;
			}
			col.Call(call, prevBase);
			return col;
		}
		IQueryable<S> CallGeneric<S>(MethodCallExpression e) {
			IQueryable q = Queryable.AsQueryable(new EnumerableWrapper<T>(this));
			List<Expression> arguments = new List<Expression>(e.Arguments);
			arguments[0] = Expression.Constant(q);
			return q.Provider.CreateQuery<S>(Expression.Call(e.Method, arguments.ToArray()));
		}
		object IQueryProvider.Execute(Expression expression) {
			return Execute(expression);
		}
		S IQueryProvider.Execute<S>(Expression expression) {
			object res = Execute(expression);
			return res == null ? default(S) : (S)res;
		}
		Type IQueryable.ElementType { get { return typeof(T); } }
		Expression IQueryable.Expression { get { return Expression.Constant(this); } }
		IQueryProvider IQueryable.Provider { get { return this; } }
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		IEnumerator<T> GetEnumerator() {
			ICollection data = Enumerate(typeof(T));
			List<T> list = new List<T>(data.Count);
			foreach (T item in data)
				list.Add(item);
			return list.GetEnumerator();
		}
		protected override object CloneCore() {
			return Clone();
		}
		public XPQuery<T> Clone() {
			return new XPQuery<T>(this);
		}
		public XPQuery<T> InTransaction() {
			return new XPQuery<T>(this, true);
		}
		public XPQuery<T> WithCustomCriteria(CustomCriteriaCollection customCriteriaCollection){
			return new XPQuery<T>(this, customCriteriaCollection);
		}
		public XPQuery<T> WithCustomCriteria(ICustomCriteriaOperatorQueryable customCriteria) {
			if(customCriteria == null) throw new ArgumentNullException();
			CustomCriteriaCollection addColection = new CustomCriteriaCollection();
			addColection.Add(customCriteria);
			return WithCustomCriteria(addColection);
		}
	}
	public static class XPQueryExtensions {
		public delegate void AsyncEnumerateCallback<T>(IEnumerable<T> result, Exception ex);
		public delegate void AsyncEnumerateCallback(IEnumerable result, Exception ex);
		public static void EnumerateAsync<T>(this IQueryable<T> query, AsyncEnumerateCallback<T> callback) {
			XPQuery<T> q = query as XPQuery<T>;
			if (q == null)
				throw new ArgumentException("query");
			q.EnumerateAsync(typeof(T), delegate(ICollection[] result, Exception ex) {
				if (ex != null)
					callback(null, ex);
				else {
					ICollection data = result[0];
					List<T> list = new List<T>(data.Count);
					foreach (T item in data)
						list.Add(item);
					callback(list, ex);
				}
			});
		}
		public static void EnumerateAsync<T>(this IQueryable<T> query, AsyncEnumerateCallback callback) {
			query.EnumerateAsync(delegate(IEnumerable<T> result, Exception ex) { callback(result, ex); });
		}
		public static XPQuery<T> Query<T>(this Session session) {
			if(session == null) throw new NullReferenceException();
			return new XPQuery<T>(session);
		}
		public static XPQuery<T> Query<T>(this IDataLayer layer) {
			if(layer == null) throw new NullReferenceException();
			return new XPQuery<T>(layer);
		}
		public static XPQuery<T> QueryInTransaction<T>(this Session session) {
			if(session == null) throw new NullReferenceException();
			return new XPQuery<T>(session, true);
		}
		class Cache<TSource> {
			public static Dictionary<Delegate, CachedQuery<TSource>> cache = new Dictionary<Delegate, CachedQuery<TSource>>();
			public static bool GetCached(Delegate f, out CachedQuery<TSource> q) {
				if(!cache.TryGetValue(f, out q)) {
					if(f.Target != null && (Attribute.GetCustomAttribute(f.Target.GetType(), typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)) == null || !Array.TrueForAll(f.Target.GetType().GetFields(), ff => ff.IsStatic)))
						throw new Exception(); 
					q = new CachedQuery<TSource>(f.Method);
					cache.Add(f, q);
					return false;
				}
				return true;
			}
		}
		public static TResult CachedExpression<TSource, TResult>(this IQueryable<TSource> source, Func<IQueryable<TSource>, TResult> f) {
			CachedQuery<TSource> q;
			lock(Cache<TSource>.cache) {
				if(!Cache<TSource>.GetCached(f, out q))
					f(q);
			}
			return (TResult)q.Process(source);
		}
		public static TResult CachedExpression<TSource, TArg1, TResult>(this IQueryable<TSource> source, Func<IQueryable<TSource>, TArg1, TResult> f, TArg1 a1) {
			CachedQuery<TSource> q;
			lock(Cache<TSource>.cache) {
				if(!Cache<TSource>.GetCached(f, out q))
					f(q, default(TArg1));
			}
			return (TResult)q.Process(source, a1);
		}
		public static TResult CachedExpression<TSource, TArg1, TArg2, TResult>(this IQueryable<TSource> source, Func<IQueryable<TSource>, TArg1, TArg2, TResult> f, TArg1 a1, TArg2 a2) {
			CachedQuery<TSource> q;
			lock(Cache<TSource>.cache) {
				if(!Cache<TSource>.GetCached(f, out q))
					f(q, default(TArg1), default(TArg2));
			}
			return (TResult)q.Process(source, a1, a2);
		}
		public static TResult CachedExpression<TSource, TArg1, TArg2, TArg3, TResult>(this IQueryable<TSource> source, Func<IQueryable<TSource>, TArg1, TArg2, TArg3, TResult> f, TArg1 a1, TArg2 a2, TArg3 a3) {
			CachedQuery<TSource> q;
			lock(Cache<TSource>.cache) {
				if(!Cache<TSource>.GetCached(f, out q))
					f(q, default(TArg1), default(TArg2), default(TArg3));
			}
			return (TResult)q.Process(source, a1, a2, a3);
		}
	}
	public interface ICustomFunctionOperatorQueryable {
		MethodInfo GetMethodInfo();
	}
	public interface ICustomNonDeterministicFunctionOperatorQueryable : ICustomFunctionOperatorQueryable {
	}
	public interface ICustomCriteriaOperatorQueryable {
		CriteriaOperator GetCriteria(params CriteriaOperator[] operands);
		MethodInfo GetMethodInfo();
	}
	public class CustomCriteriaCollection : CustomDictionaryCollection<MethodInfo, ICustomCriteriaOperatorQueryable> {
		public CustomCriteriaCollection() : base() { }
		protected override MethodInfo GetKey(ICustomCriteriaOperatorQueryable item) {
			return item.GetMethodInfo();
		}
	}
	public static class CustomCriteriaManager {
		static CustomCriteriaCollection registeredCustomCriteria = new CustomCriteriaCollection();
		public static void RegisterCriterion(ICustomCriteriaOperatorQueryable customCriterion) {
			lock(registeredCustomCriteria) {
				registeredCustomCriteria.Add(customCriterion);
			}
		}
		public static void RegisterCriteria(IEnumerable<ICustomCriteriaOperatorQueryable> customCriteria) {
			lock(registeredCustomCriteria) {
				registeredCustomCriteria.Add(customCriteria);
			}
		}
		public static bool UnregisterCriterion(ICustomCriteriaOperatorQueryable customCriterion) {
			lock(registeredCustomCriteria) {
				return registeredCustomCriteria.Remove(customCriterion);
			}
		}
		public static bool UnregisterCriterion(MethodInfo methodInfo) {
			lock(registeredCustomCriteria) {
				ICustomCriteriaOperatorQueryable customCriterion = registeredCustomCriteria.GetItem(methodInfo);
				if(customCriterion == null) return false;
				return registeredCustomCriteria.Remove(customCriterion);
			}
		}
#if !SL
	[DevExpressXpoLocalizedDescription("CustomCriteriaManagerRegisteredCriterionCount")]
#endif
public static int RegisteredCriterionCount {
			get { return registeredCustomCriteria.Count; }
		}
		public static ICustomCriteriaOperatorQueryable GetCriteria(MethodInfo methodInfo) {
			return registeredCustomCriteria.GetItem(methodInfo);
		}
		public static CustomCriteriaCollection GetRegisteredCriteria() {
			CustomCriteriaCollection result = new CustomCriteriaCollection();
			lock(registeredCustomCriteria) {
				result.Add(registeredCustomCriteria);
			}
			return result;
		}
	}
}
