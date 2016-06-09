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
using System.IO;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Xpo.DB;
using DevExpress.Compatibility.System;
#if !SL
using DevExpress.Data.IO;
#endif
namespace DevExpress.Data.Filtering.Helpers {
	[Serializable]
	public enum GroupType { And, Or, NotAnd, NotOr }
	[Serializable]
	public enum ClauseType {
		Equals, DoesNotEqual,
		Greater, GreaterOrEqual, Less, LessOrEqual, Between, NotBetween,
		Contains, DoesNotContain, BeginsWith, EndsWith, Like, NotLike,
		IsNull, IsNotNull,
		AnyOf, NoneOf,
		IsNullOrEmpty, IsNotNullOrEmpty,
		IsBeyondThisYear,
		IsLaterThisYear,
		IsLaterThisMonth,
		IsNextWeek,
		IsLaterThisWeek,
		IsTomorrow,
		IsToday,
		IsYesterday,
		IsEarlierThisWeek,
		IsLastWeek,
		IsEarlierThisMonth,
		IsEarlierThisYear,
		IsPriorThisYear,
	}
	public enum FilterColumnClauseClass { Generic, DateTime, String, Lookup, Blob }
	public interface INode {
		IGroupNode ParentNode { get;}
		void SetParentNode(IGroupNode parentNode);
		object Accept(INodeVisitor visitor);
	}
	public interface INodeVisitor {
		object Visit(IGroupNode group);
		object Visit(IClauseNode clause);
		object Visit(IAggregateNode aggregate);
	}
	public interface IGroupNode : INode {
		GroupType NodeType { get;}
		IList<INode> SubNodes { get;}
	}
	public interface IClauseNode : INode {
		OperandProperty FirstOperand { get;}
		ClauseType Operation { get;}
		IList<CriteriaOperator> AdditionalOperands { get;}
	}
	public interface IAggregateNode : IClauseNode {
		Aggregate Aggregate { get; }
		OperandProperty AggregateOperand { get; }
		INode AggregateCondition { get; set; }
	}
	public static class FilterTreeCloner {
		public static IGroupNode Clone(IGroupNode source, INodesFactory factory) {
			return factory.Create(source.NodeType, Clone(source.SubNodes, factory));
		}
		static ICollection<INode> Clone(ICollection<INode> source, INodesFactory factory) {
			IList<INode> rv = new List<INode>(source.Count);
			foreach(INode node in source) {
				rv.Add(Clone(node, factory));
			}
			return rv;
		}
		static INode Clone(INode node, INodesFactory factory) {
			if(node is IGroupNode) {
				return Clone((IGroupNode)node, factory);
			} else if(node is IClauseNode) {
				return Clone((IClauseNode)node, factory);
			}
			throw new InvalidOperationException();
		}
		static ICollection<CriteriaOperator> Clone(ICollection<CriteriaOperator> source) {
			IList<CriteriaOperator> rv = new List<CriteriaOperator>(source.Count);
			foreach(CriteriaOperator op in source)
				rv.Add(CriteriaOperator.Clone(op));
			return rv;
		}
		static IClauseNode Clone(IClauseNode source, INodesFactory factory) {
			return factory.Create(source.Operation, CriteriaOperator.Clone(source.FirstOperand), Clone(source.AdditionalOperands));
		}
#if !SL
		public static void Serialize(IGroupNode source, Stream stream) {
			TypedBinaryWriter writer = new TypedBinaryWriter(stream);
			SerializeNode(source, writer);
		}
		public static IGroupNode Deserialize(Stream stream, INodesFactory factory) {
			TypedBinaryReader reader = new TypedBinaryReader(stream);
			return DeserializeNode(reader, factory) as IGroupNode;
		}
		static void SerializeNode(INode node, TypedBinaryWriter writer) {
			byte nodeType = node is IGroupNode ? (byte)1 : (node is IAggregateNode ? (byte)2 : (byte)0);
			writer.WriteObject(nodeType);
			switch(nodeType) {
				case 1: SerializeGroupNode(node as IGroupNode, writer); break;
				case 0: SerializeClauseNode(node as IClauseNode, writer); break;
				default: SerializeAggregateNode(node as IAggregateNode, writer); break;
			}
		}
		static INode DeserializeNode(TypedBinaryReader reader, INodesFactory factory) {
			byte nodeType = reader.ReadObject<byte>();
			switch(nodeType) {
				case 1: return DeserializeGroupNode(reader, factory);
				case 0: return DeserializeClauseNode(reader, factory);
				default: return DeserializeAggregateNode(reader, factory);
			}
		}
		static void SerializeGroupNode(IGroupNode source, TypedBinaryWriter writer) {
			writer.WriteObject((int)source.NodeType);
			IList<INode> subNodes = source.SubNodes;
			writer.WriteObject(subNodes.Count);
			for (int i = 0; i < subNodes.Count; i++) {
				SerializeNode(subNodes[i], writer);
			}
		}
		static IGroupNode DeserializeGroupNode(TypedBinaryReader reader, INodesFactory factory) {
			GroupType groupType = (GroupType)reader.ReadObject<int>();
			int count = reader.ReadObject<int>();
			Collection<INode> nodes = new Collection<INode>();
			for (int i = 0; i < count; i++) {
				nodes.Add(DeserializeNode(reader, factory));
			}
			return factory.Create(groupType, nodes);
		}
		static void SerializeClauseNode(IClauseNode source, TypedBinaryWriter writer) {
			writer.WriteObject((int)source.Operation);
			writer.WriteObject(CriteriaOperator.ToString(source.FirstOperand));
			writer.WriteObject(source.AdditionalOperands.Count);
			for (int i = 0; i < source.AdditionalOperands.Count; i++) {
				writer.WriteObject(CriteriaOperator.ToString(source.AdditionalOperands[i]));
			}
		}
		static void SerializeAggregateNode(IAggregateNode source, TypedBinaryWriter writer) {
			SerializeClauseNode(source, writer);
			writer.WriteObject((int)source.Aggregate);
			writer.WriteObject(CriteriaOperator.ToString(source.AggregateOperand));
			bool hasCondition = !ReferenceEquals(source.AggregateCondition, null);
			writer.WriteObject(hasCondition);
			if(hasCondition) {
				SerializeNode(source.AggregateCondition, writer);
			}
		}
		static void DeserializeClauseNode(TypedBinaryReader reader, out ClauseType clauseType,
			out CriteriaOperator firstOperand, Collection<CriteriaOperator> operands, INodesFactory factory) {
			clauseType = (ClauseType)reader.ReadObject<int>();
			firstOperand = CriteriaParse(reader.ReadObject<string>(), factory);
			int count = reader.ReadObject<int>();
			for(int i = 0; i < count; i++) {
				operands.Add(CriteriaParse(reader.ReadObject<string>(), factory));
			}
		}
		static IClauseNode DeserializeClauseNode(TypedBinaryReader reader, INodesFactory factory) {
			ClauseType clauseType;
			CriteriaOperator firstOperand;
			Collection<CriteriaOperator> operands = new Collection<CriteriaOperator>();
			DeserializeClauseNode(reader, out clauseType, out firstOperand, operands, factory);
			return factory.Create(clauseType, firstOperand as OperandProperty, operands);
		}
		static IAggregateNode DeserializeAggregateNode(TypedBinaryReader reader, INodesFactory factory) {
			ClauseType clauseType;
			CriteriaOperator firstOperand;
			Collection<CriteriaOperator> operands = new Collection<CriteriaOperator>();
			DeserializeClauseNode(reader, out clauseType, out firstOperand, operands, factory);
			Aggregate aggregate = (Aggregate)reader.ReadObject<int>();
			CriteriaOperator aggregateOperand = CriteriaParse(reader.ReadObject<string>(), factory);
			bool hasCondition = reader.ReadObject<bool>();
			INode condtion = null;
			if(hasCondition) {
				condtion = DeserializeNode(reader, factory);
			}
			INodesFactoryEx factoryEx = factory as INodesFactoryEx;
			if(factoryEx == null) return null;
			return factoryEx.Create(firstOperand as OperandProperty, aggregate, aggregateOperand as OperandProperty, clauseType, operands, condtion);
		}
		static CriteriaOperator CriteriaParse(string criteria, INodesFactory factory) {
			var fcFactory = factory as DevExpress.XtraEditors.Filtering.FilterControlNodesFactory;
			if(fcFactory != null)
				return fcFactory.Model.CriteriaParse(criteria);
			return CriteriaOperator.Parse(criteria);
		}
		public static string ToString(IGroupNode source) {
			using (MemoryStream stream = new MemoryStream()) {
				Serialize(source, stream);
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		public static IGroupNode FromString(string str, INodesFactory factory) {
			if(string.IsNullOrEmpty(str))
				return factory.Create(GroupType.And, new INode[0]);
			using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(str))) {
				return Deserialize(stream, factory);
			}
		}
#endif
	}
	[Serializable]
	[XmlInclude(typeof(SerializableTreeGroupNode))]
	[XmlInclude(typeof(SerializableTreeClauseNode))]
	public abstract class SerializableTreeNode : INode {
		[XmlIgnore]
		public IGroupNode ParentNode {
			set { }
			get { throw new NotImplementedException(); }
		}
		public void SetParentNode(IGroupNode parentNode) { }
		object INode.Accept(INodeVisitor visitor) {
			return Accept(visitor);
		}
		protected abstract object Accept(INodeVisitor visitor);
	}
	[Serializable]
	public class SerializableTreeGroupNode : SerializableTreeNode, IGroupNode {
		GroupType _NodeType;
		[XmlAttribute]
		public GroupType NodeType {
			get { return _NodeType; }
			set { _NodeType = value; }
		}
		List<SerializableTreeNode> _SubNodes = new List<SerializableTreeNode>();
		[XmlArrayItem(typeof(SerializableTreeNode))]
		public List<SerializableTreeNode> SubNodes {
			get { return _SubNodes; }
		}
		IList<INode> IGroupNode.SubNodes {
			get {
				List<INode> rv = new List<INode>();
				foreach(SerializableTreeNode n in SubNodes)
					rv.Add(n);
				return rv;
			}
		}
		protected override object Accept(INodeVisitor visitor) {
			return visitor.Visit(this);
		}
	}
	[Serializable]
	public class SerializableTreeClauseNode : SerializableTreeNode, IClauseNode {
		OperandProperty _FirstOperand;
		public OperandProperty FirstOperand {
			get { return _FirstOperand; }
			set { _FirstOperand = value; }
		}
		ClauseType _Operation;
		[XmlAttribute]
		public ClauseType Operation {
			get { return _Operation; }
			set { _Operation = value; }
		}
		List<CriteriaOperator> _AdditionalOperands = new List<CriteriaOperator>();
		[XmlArrayItem(typeof(CriteriaOperator))]
		public List<CriteriaOperator> AdditionalOperands {
			get { return _AdditionalOperands; }
		}
		IList<CriteriaOperator> IClauseNode.AdditionalOperands {
			get { return AdditionalOperands; }
		}
		protected override object Accept(INodeVisitor visitor) {
			return visitor.Visit(this);
		}
	}
	[Serializable]
	public class SerializableTreeAggregateNode : SerializableTreeClauseNode, IAggregateNode {
		OperandProperty _AggregateOperand;
		INode _AggregateCondition;
		public OperandProperty AggregateOperand {
			get { return _AggregateOperand; }
			set { _AggregateOperand = value; }
		}
		Aggregate _Aggregate;
		[XmlAttribute]
		public Aggregate Aggregate {
			get { return _Aggregate; }
			set { _Aggregate = value; }
		}
		[XmlAttribute]
		public INode AggregateCondition {
			get { return _AggregateCondition; }
			set { _AggregateCondition = value; }
		}
		protected override object Accept(INodeVisitor visitor) {
			return visitor.Visit(this);
		}
	}
	public class SerializableTreeFactory : INodesFactory {
		public IGroupNode Create(GroupType type, ICollection<INode> subNodes) {
			SerializableTreeGroupNode rv = new SerializableTreeGroupNode();
			rv.NodeType = type;
			foreach(SerializableTreeNode node in subNodes) {
				rv.SubNodes.Add(node);
				node.SetParentNode(rv);
			}
			return rv;
		}
		public IClauseNode Create(ClauseType type, OperandProperty firstOperand, ICollection<CriteriaOperator> operands) {
			SerializableTreeClauseNode rv = new SerializableTreeClauseNode();
			rv.Operation = type;
			rv.FirstOperand = firstOperand;
			foreach(CriteriaOperator op in operands) {
				rv.AdditionalOperands.Add(op);
			}
			return rv;
		}
	}
	public class SerializableTreeFactoryEx : SerializableTreeFactory, INodesFactoryEx {
		IAggregateNode INodesFactoryEx.Create(OperandProperty firstOperand, Aggregate aggregate, OperandProperty aggregateOperand, ClauseType operation, ICollection<CriteriaOperator> operands, INode conditionNode) {
			SerializableTreeAggregateNode rv = new SerializableTreeAggregateNode();
			rv.Operation = operation;
			rv.FirstOperand = firstOperand;
			rv.AggregateOperand = aggregateOperand;
			rv.Aggregate = aggregate;
			if(operands != null) {
				foreach(CriteriaOperator op in operands) {
					rv.AdditionalOperands.Add(op);
				}
			}
			rv.AggregateCondition = conditionNode;
			return rv;
		}
	}
	public interface INodesFactory {
		IGroupNode Create(GroupType type, ICollection<INode> subNodes);
		IClauseNode Create(ClauseType type, OperandProperty firstOperand, ICollection<CriteriaOperator> operands);
	}
	public interface INodesFactoryEx : INodesFactory {
		IAggregateNode Create(OperandProperty firstOperand, Aggregate aggregate, OperandProperty aggregateOperand, ClauseType operation, ICollection<CriteriaOperator> operands, INode conditionNode);
	}
	public class CriteriaToTreeProcessor : IClientCriteriaVisitor<INode> {
		protected readonly INodesFactory Factory;
		protected readonly INodesFactoryEx FactoryEx;
		public readonly IList<CriteriaOperator> Skipped;
		protected CriteriaToTreeProcessor(INodesFactory nodesFactory, IList<CriteriaOperator> skippedHolder) {
			this.Factory = nodesFactory;
			this.FactoryEx = Factory as INodesFactoryEx;
			Skipped = skippedHolder;
		}
		protected IClauseNode Skip(CriteriaOperator skip) {
			if(Skipped != null && !ReferenceEquals(skip, null))
				Skipped.Add(skip);
			return null;
		}
		protected IClauseNode CreateClauseNode(CriteriaOperator origionalOperator, ClauseType type, CriteriaOperator firstOperand, ICollection<CriteriaOperator> operands) {
			OperandProperty property = firstOperand as OperandProperty;
			if(!ReferenceEquals(property, null))
				return Factory.Create(type, property, operands);
			AggregateOperand aggregate = FactoryEx != null ? firstOperand as AggregateOperand : null;
			if(!ReferenceEquals(aggregate, null)) {
				INode condition = CriteriaToTreeProcessor.GetTree(Factory, aggregate.Condition, Skipped);
				return FactoryEx.Create(aggregate.CollectionProperty, aggregate.AggregateType, aggregate.AggregatedExpression as OperandProperty, type, operands, condition);
			}
			return Skip(origionalOperator);
		}
		INode IClientCriteriaVisitor<INode>.Visit(OperandProperty theOperand) {
			return Skip(theOperand);
		}
		INode IClientCriteriaVisitor<INode>.Visit(AggregateOperand theOperand) {
			if(FactoryEx == null) return Skip(theOperand);
			INode condition = CriteriaToTreeProcessor.GetTree(Factory, theOperand.Condition, Skipped);
			return FactoryEx.Create(theOperand.CollectionProperty, theOperand.AggregateType, theOperand.AggregatedExpression as OperandProperty, ClauseType.IsNull, null, condition);
		}
		INode IClientCriteriaVisitor<INode>.Visit(JoinOperand theOperand) {
			return Skip(theOperand);
		}
		INode CreateNodeForUnaryClause(FunctionOperator theOperator, ClauseType clauseType) {
			if(theOperator.Operands.Count != 1)
				return Skip(theOperator);
			return CreateClauseNode(theOperator, clauseType, theOperator.Operands[0], new CriteriaOperator[0]);
		}
		INode ICriteriaVisitor<INode>.Visit(FunctionOperator theOperator) {
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsBeyondThisYear);
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsLaterThisYear);
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsLaterThisMonth);
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsNextWeek);
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsLaterThisWeek);
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsTomorrow);
				case FunctionOperatorType.IsOutlookIntervalToday:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsToday);
				case FunctionOperatorType.IsOutlookIntervalYesterday:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsYesterday);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsEarlierThisWeek);
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsLastWeek);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsEarlierThisMonth);
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsEarlierThisYear);
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsPriorThisYear);
				case FunctionOperatorType.IsNullOrEmpty:
					return CreateNodeForUnaryClause(theOperator, ClauseType.IsNullOrEmpty);
				case FunctionOperatorType.StartsWith:
				case FunctionOperatorType.EndsWith:
				case FunctionOperatorType.Contains:
					return DoStartsEndsContains(theOperator);
				case FunctionOperatorType.Custom:
					if(LikeCustomFunction.IsBinaryCompatibleLikeFunction(theOperator) && IsGoodForAdditionalOperands(theOperator.Operands[2]))
						return CreateClauseNode(theOperator, ClauseType.Like, theOperator.Operands[1], new CriteriaOperator[] { theOperator.Operands[2] });
					break;
			}
			return Skip(theOperator);
		}
		INode DoStartsEndsContains(FunctionOperator opa) {
			if(opa.Operands.Count != 2)
				return Skip(opa);
			CriteriaOperator first = opa.Operands[0];
			CriteriaOperator additional = opa.Operands[1];
			if(!(IsGoodForAdditionalOperands(additional)))
				return Skip(opa);
			switch(opa.OperatorType){
				case FunctionOperatorType.StartsWith:
					return CreateClauseNode(opa, ClauseType.BeginsWith, first, new CriteriaOperator[] { additional });
				case FunctionOperatorType.EndsWith:
					return CreateClauseNode(opa, ClauseType.EndsWith, first, new CriteriaOperator[] { additional });
				case FunctionOperatorType.Contains:
					return CreateClauseNode(opa, ClauseType.Contains, first, new CriteriaOperator[] { additional });
				default:
					throw new InvalidOperationException("Unexpected " + CriteriaOperator.ToString(opa));
			}
		}
		INode ICriteriaVisitor<INode>.Visit(OperandValue theOperand) {
			return Skip(theOperand);
		}
		INode ICriteriaVisitor<INode>.Visit(GroupOperator theOperator) {
			GroupType resolvedType = (theOperator.OperatorType == GroupOperatorType.And) ? GroupType.And : GroupType.Or;
			List<INode> subNodes = new List<INode>();
			foreach(CriteriaOperator subOperand in theOperator.Operands) {
				INode nestedNode = Process(subOperand);
				if(nestedNode != null) {
					subNodes.Add(nestedNode);
				}
			}
			return Factory.Create(resolvedType, subNodes);
		}
		INode ICriteriaVisitor<INode>.Visit(InOperator theOperator) {
			List<CriteriaOperator> operands = new List<CriteriaOperator>();
			foreach(CriteriaOperator ao in theOperator.Operands) {
				if(IsGoodForAdditionalOperands(ao)) {
					operands.Add(ao);
				} else {
					return Skip(theOperator);
				}
			}
			return CreateClauseNode(theOperator, ClauseType.AnyOf, theOperator.LeftOperand, operands);
		}
		INode ICriteriaVisitor<INode>.Visit(UnaryOperator theOperator) {
			if(theOperator.OperatorType == UnaryOperatorType.IsNull) {
				return CreateClauseNode(theOperator, ClauseType.IsNull, theOperator.Operand, new CriteriaOperator[0]);
			} else if(theOperator.OperatorType == UnaryOperatorType.Not) {
				INode subNode = Process(theOperator.Operand);
				if(subNode is IGroupNode) {
					IGroupNode gr = (IGroupNode)subNode;
					GroupType invertedType;
					switch(gr.NodeType) {
						case GroupType.And:
							invertedType = GroupType.NotAnd;
							break;
						case GroupType.Or:
							invertedType = GroupType.NotOr;
							break;
						case GroupType.NotAnd:
							invertedType = GroupType.And;
							break;
						case GroupType.NotOr:
							invertedType = GroupType.Or;
							break;
						default:
							throw new NotImplementedException(gr.NodeType.ToString());
					}
					return Factory.Create(invertedType, gr.SubNodes);
				} else if(subNode is IClauseNode) {
					IClauseNode oldClause = (IClauseNode)subNode;
					ClauseType invertedType = GetInvertedOperation(oldClause);
					if(invertedType != oldClause.Operation) {
						if(oldClause is IAggregateNode) {
							IAggregateNode oldAggregateNode = subNode as IAggregateNode;
							return FactoryEx.Create(oldAggregateNode.FirstOperand, oldAggregateNode.Aggregate, oldAggregateNode.AggregateOperand, invertedType, oldAggregateNode.AdditionalOperands, oldAggregateNode.AggregateCondition);
						} else {
							return Factory.Create(invertedType, oldClause.FirstOperand, oldClause.AdditionalOperands);
						}
					} else {
						return Factory.Create(GroupType.NotAnd, new INode[] { oldClause });
					}
				} else {
					return Skip(theOperator);
				}
			} else {
				return Skip(theOperator);
			}
		}
		ClauseType GetInvertedOperation(IClauseNode subNode) {
			IAggregateNode aggregateNode = subNode as IAggregateNode;
			if(aggregateNode != null && aggregateNode.Aggregate == Aggregate.Exists) return subNode.Operation;
			switch(subNode.Operation) {
				case ClauseType.AnyOf: return ClauseType.NoneOf;
				case ClauseType.Between: return ClauseType.NotBetween;
				case ClauseType.Contains: return ClauseType.DoesNotContain;
				case ClauseType.Equals: return ClauseType.DoesNotEqual;
				case ClauseType.Greater: return ClauseType.LessOrEqual;
				case ClauseType.GreaterOrEqual: return ClauseType.Less;
				case ClauseType.Like: return ClauseType.NotLike;
				case ClauseType.IsNotNull: return ClauseType.IsNull;
				case ClauseType.IsNull: return ClauseType.IsNotNull;
				case ClauseType.IsNullOrEmpty: return ClauseType.IsNotNullOrEmpty;
				case ClauseType.IsNotNullOrEmpty: return ClauseType.IsNullOrEmpty;
				case ClauseType.Less: return ClauseType.GreaterOrEqual;
				case ClauseType.LessOrEqual: return ClauseType.Greater;
				case ClauseType.NoneOf: return ClauseType.AnyOf;
				case ClauseType.NotBetween: return ClauseType.Between;
				case ClauseType.DoesNotContain: return ClauseType.Contains;
				case ClauseType.DoesNotEqual: return ClauseType.Equals;
				case ClauseType.NotLike: return ClauseType.Like;
			}
			return subNode.Operation;
		}
		bool IsGoodForAdditionalOperands(CriteriaOperator opa) {
			if(opa is OperandValue)
				return true;
			if(opa is OperandProperty)
				return true;
			FunctionOperator fn = opa as FunctionOperator;
			if(!ReferenceEquals(fn, null)) {
				switch(fn.OperatorType) {
					case FunctionOperatorType.LocalDateTimeThisYear:
					case FunctionOperatorType.LocalDateTimeThisMonth:
					case FunctionOperatorType.LocalDateTimeLastWeek:
					case FunctionOperatorType.LocalDateTimeThisWeek:
					case FunctionOperatorType.LocalDateTimeYesterday:
					case FunctionOperatorType.LocalDateTimeToday:
					case FunctionOperatorType.LocalDateTimeNow:
					case FunctionOperatorType.LocalDateTimeTomorrow:
					case FunctionOperatorType.LocalDateTimeDayAfterTomorrow:
					case FunctionOperatorType.LocalDateTimeNextWeek:
					case FunctionOperatorType.LocalDateTimeTwoWeeksAway:
					case FunctionOperatorType.LocalDateTimeNextMonth:
					case FunctionOperatorType.LocalDateTimeNextYear:
						return true;
					case FunctionOperatorType.Custom:
						return fn.Operands.Count == 1;
				}
			}
			return false;
		}
		bool IsConstantPercent(CriteriaOperator op) {
			OperandValue ov = op as OperandValue;
			if(ReferenceEquals(null, ov))
				return false;
			if(ov is OperandParameter)
				return false;
			string strValue = ov.Value as string;
			return strValue == "%";
		}
		INode ICriteriaVisitor<INode>.Visit(BinaryOperator theOperator) {
			ClauseType type;
			switch(theOperator.OperatorType) {
				case BinaryOperatorType.Equal:
					type = ClauseType.Equals;
					break;
				case BinaryOperatorType.Greater:
					type = ClauseType.Greater;
					break;
				case BinaryOperatorType.GreaterOrEqual:
					type = ClauseType.GreaterOrEqual;
					break;
				case BinaryOperatorType.Less:
					type = ClauseType.Less;
					break;
				case BinaryOperatorType.LessOrEqual:
					type = ClauseType.LessOrEqual;
					break;
#pragma warning disable 618
				case BinaryOperatorType.Like:
#pragma warning restore 618
					type = ClauseType.Like;
					break;
				case BinaryOperatorType.NotEqual:
					type = ClauseType.DoesNotEqual;
					break;
				default:
					return Skip(theOperator);
			}
			if(!IsGoodForAdditionalOperands(theOperator.RightOperand))
				return Skip(theOperator);
			return CreateClauseNode(theOperator, type, theOperator.LeftOperand, new CriteriaOperator[] { theOperator.RightOperand });
		}
		INode ICriteriaVisitor<INode>.Visit(BetweenOperator theOperator) {
			if(!IsGoodForAdditionalOperands(theOperator.BeginExpression))
				return Skip(theOperator);
			if(!IsGoodForAdditionalOperands(theOperator.EndExpression))
				return Skip(theOperator);
			return CreateClauseNode(theOperator, ClauseType.Between, theOperator.TestExpression, new CriteriaOperator[] { theOperator.BeginExpression, theOperator.EndExpression });
		}
		INode Process(CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return null;
			return op.Accept(this);
		}
		public static INode GetTree(INodesFactory nodesFactory, CriteriaOperator op, IList<CriteriaOperator> skippedCriteria) {
			INode result = new CriteriaToTreeProcessor(nodesFactory, skippedCriteria).Process(op);
			return result;
		}
		public static bool IsConvertibleOperator(CriteriaOperator opa) {
			IList<CriteriaOperator> skippedList = new CriteriaOperatorCollection();
			INodesFactory dummyFactory = new IsConvertibleFactory();
			GetTree(dummyFactory, opa, skippedList);
			return skippedList.Count == 0;
		}
		class IsConvertibleFactory : INodesFactory {
			public IGroupNode Create(GroupType type, ICollection<INode> subNodes) {
				return new IsConvertibleGroupNode(type, new List<INode>(subNodes));
			}
			public IClauseNode Create(ClauseType type, OperandProperty firstOperand, ICollection<CriteriaOperator> operands) {
				return new IsConvertibleClauseNode(type, firstOperand, new List<CriteriaOperator>(operands));
			}
		}
		class IsConvertibleGroupNode : IGroupNode {
			readonly GroupType nodeType;
			readonly IList<INode> subNodes;
			public IsConvertibleGroupNode(GroupType nodeType, IList<INode> subNodes) {
				this.nodeType = nodeType;
				this.subNodes = subNodes;
				foreach(INode subNode in subNodes)
					subNode.SetParentNode(this);
			}
			public GroupType NodeType {
				get { return nodeType; }
			}
			public IList<INode> SubNodes {
				get { return subNodes; }
			}
			IGroupNode ParentNodeCore;
			public IGroupNode ParentNode { get { return ParentNodeCore; } }
			public void SetParentNode(IGroupNode node) {
				ParentNodeCore = node;
			}
			object INode.Accept(INodeVisitor visitor) {
				return visitor.Visit(this);
			}
		}
		class IsConvertibleClauseNode : IClauseNode {
			readonly OperandProperty firstOperand;
			readonly ClauseType operation;
			readonly IList<CriteriaOperator> additinalOperands;
			public OperandProperty FirstOperand {
				get { return firstOperand; }
			}
			public ClauseType Operation {
				get { return operation; }
			}
			public IList<CriteriaOperator> AdditionalOperands {
				get { return additinalOperands; }
			}
			public IsConvertibleClauseNode(ClauseType type, OperandProperty firstOperand, IList<CriteriaOperator> operands) {
				operation = type;
				this.firstOperand = firstOperand;
				additinalOperands = operands;
			}
			IGroupNode ParentNodeCore;
			public IGroupNode ParentNode { get { return ParentNodeCore; } }
			public void SetParentNode(IGroupNode node) {
				ParentNodeCore = node;
			}
			object INode.Accept(INodeVisitor visitor) {
				return visitor.Visit(this);
		}
	}
	}
	public class NodeToCriteriaProcessor : INodeVisitor {
		public virtual object Visit(IGroupNode ign) {
			GroupOperatorType combineStatus = (ign.NodeType == GroupType.And || ign.NodeType == GroupType.NotAnd) ? GroupOperatorType.And : GroupOperatorType.Or;
			CriteriaOperator result = null;
			foreach(INode subNode in ign.SubNodes) {
				result = GroupOperator.Combine(combineStatus, result, Process(subNode));
			}
			if((ign.NodeType == GroupType.NotAnd || ign.NodeType == GroupType.NotOr) && !ReferenceEquals(result, null)) {
				result = new UnaryOperator(UnaryOperatorType.Not, result);
			}
			return result;
		}
		public virtual object Visit(IClauseNode icn) {
			return Visit(icn.Operation, icn.FirstOperand, icn.AdditionalOperands);
		}
		public virtual object Visit(IAggregateNode ign) {
			OperandProperty collectionProperty = ign.FirstOperand as OperandProperty;
			OperandProperty aggregateProperty = ign.AggregateOperand as OperandProperty;
			CriteriaOperator condition = CriteriaOperator.Parse(string.Empty);
			if(ign.AggregateCondition != null) {
				condition = FilterControlHelpers.ToCriteria(ign.AggregateCondition);
			}
			AggregateOperand aggregateOperand = new AggregateOperand(collectionProperty, aggregateProperty, ign.Aggregate, condition);
			if(ign.Aggregate == Aggregate.Exists) return aggregateOperand;
			return Visit(ign.Operation, aggregateOperand, ign.AdditionalOperands);
		}
		public CriteriaOperator Process(INode node) {
			return (CriteriaOperator)node.Accept(this);
		}
		public virtual object Visit(ClauseType clauseType, CriteriaOperator firstOperand, IList<CriteriaOperator> additionalOperands) {
			switch(clauseType) {
				case ClauseType.Equals:
					return firstOperand == additionalOperands[0];
				case ClauseType.DoesNotEqual:
					return firstOperand != additionalOperands[0];
				case ClauseType.Less:
					return firstOperand < additionalOperands[0];
				case ClauseType.Greater:
					return firstOperand > additionalOperands[0];
				case ClauseType.LessOrEqual:
					return firstOperand <= additionalOperands[0];
				case ClauseType.GreaterOrEqual:
					return firstOperand >= additionalOperands[0];
				case ClauseType.AnyOf:
					return new InOperator(firstOperand, additionalOperands);
				case ClauseType.NoneOf:
					return new UnaryOperator(UnaryOperatorType.Not, new InOperator(firstOperand, additionalOperands));
				case ClauseType.BeginsWith:
					return new FunctionOperator(FunctionOperatorType.StartsWith, firstOperand, additionalOperands[0]);
				case ClauseType.EndsWith:
					return new FunctionOperator(FunctionOperatorType.EndsWith, firstOperand, additionalOperands[0]);
				case ClauseType.Between:
					return new BetweenOperator(firstOperand, additionalOperands[0], additionalOperands[1]);
				case ClauseType.NotBetween:
					return new UnaryOperator(UnaryOperatorType.Not, new BetweenOperator(firstOperand, additionalOperands[0], additionalOperands[1]));
				case ClauseType.Contains:
					return new FunctionOperator(FunctionOperatorType.Contains, firstOperand, additionalOperands[0]);
				case ClauseType.DoesNotContain:
					return new UnaryOperator(UnaryOperatorType.Not, new FunctionOperator(FunctionOperatorType.Contains, firstOperand, additionalOperands[0]));
				case ClauseType.Like:
					return LikeCustomFunction.Create(firstOperand, additionalOperands[0]);
				case ClauseType.NotLike:
					return !LikeCustomFunction.Create(firstOperand, additionalOperands[0]);;
				case ClauseType.IsNull:
					return firstOperand.IsNull();
				case ClauseType.IsNotNull:
					return firstOperand.IsNotNull();
				case ClauseType.IsNullOrEmpty:
					return new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, firstOperand);
				case ClauseType.IsNotNullOrEmpty:
					return new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, firstOperand).Not();
				case ClauseType.IsBeyondThisYear:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalBeyondThisYear, firstOperand);
				case ClauseType.IsLaterThisYear:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLaterThisYear, firstOperand);
				case ClauseType.IsLaterThisMonth:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLaterThisMonth, firstOperand);
				case ClauseType.IsNextWeek:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalNextWeek, firstOperand);
				case ClauseType.IsLaterThisWeek:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLaterThisWeek, firstOperand);
				case ClauseType.IsTomorrow:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalTomorrow, firstOperand);
				case ClauseType.IsToday:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalToday, firstOperand);
				case ClauseType.IsYesterday:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalYesterday, firstOperand);
				case ClauseType.IsEarlierThisWeek:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalEarlierThisWeek, firstOperand);
				case ClauseType.IsLastWeek:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLastWeek, firstOperand);
				case ClauseType.IsEarlierThisMonth:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalEarlierThisMonth, firstOperand);
				case ClauseType.IsEarlierThisYear:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalEarlierThisYear, firstOperand);
				case ClauseType.IsPriorThisYear:
					return new FunctionOperator(FunctionOperatorType.IsOutlookIntervalPriorThisYear, firstOperand);
				default:
					throw new NotImplementedException();
			}
		}
	}
	public static class FilterControlHelpers {
		public static CriteriaOperator ToCriteria(INode node) {
			return new NodeToCriteriaProcessor().Process(node);
		}
		public static int GetLastElementIndex(INode node) {
			if(node is IGroupNode) {
				return 0;
			}
			else if(node is IClauseNode) {
				return ((IClauseNode)node).AdditionalOperands.Count + 1;
			}
			else {
				throw new InvalidOperationException("Only GroupNode and ClauseNode may be used in this context.");
			}
		}
		public static INode GetNextNodeAfter(IGroupNode currentNode, INode node) {
			int indexOfNode = currentNode.SubNodes.IndexOf(node);
			System.Diagnostics.Debug.Assert(indexOfNode >= 0);
			int nextIndex = indexOfNode + 1;
			if(nextIndex < currentNode.SubNodes.Count) {
				return (INode)currentNode.SubNodes[nextIndex];
			}
			else {
				if(currentNode.ParentNode == null)
					return currentNode;
				else
					return GetNextNodeAfter((IGroupNode)currentNode.ParentNode, currentNode);
			}
		}
		public static INode GetNextNode(INode currentNode) {
			IGroupNode currentGroup = currentNode as IGroupNode;
			if(currentGroup != null && currentGroup.SubNodes.Count > 0)
				return currentGroup.SubNodes[0];
			if(currentNode.ParentNode == null)
				return currentNode;
			return FilterControlHelpers.GetNextNodeAfter(currentNode.ParentNode, currentNode);
		}
		public static INode GetLastNode(INode currentNode) {
			if(currentNode is IGroupNode) {
				IGroupNode currentGroupNode = (IGroupNode)currentNode;
				if(currentGroupNode.SubNodes.Count <= 0)
					return currentGroupNode;
				return GetLastNode(currentGroupNode.SubNodes[currentGroupNode.SubNodes.Count - 1]);
			}
			if(currentNode is IClauseNode) 
				return (IClauseNode)currentNode;
			throw new InvalidOperationException("Only IGroupNode and IClauseNode accepted");
		}
		public static void ForceAdditionalParamsCount(IList<CriteriaOperator> additionalOperands, int p) {
			List<CriteriaOperator> copyOperands = new List<CriteriaOperator>(additionalOperands);
			additionalOperands.Clear();
			while (additionalOperands.Count < p) {
				CriteriaOperator value = additionalOperands.Count < copyOperands.Count ? copyOperands[additionalOperands.Count] : new OperandValue();
				additionalOperands.Add(value);
			}
		}
		public static void ValidateAdditionalOperands(ClauseType operation, IList<CriteriaOperator> additionalOperands) {
			switch(operation) {
				case ClauseType.Equals:
				case ClauseType.DoesNotEqual:
				case ClauseType.Less:
				case ClauseType.Greater:
				case ClauseType.LessOrEqual:
				case ClauseType.GreaterOrEqual:
				case ClauseType.BeginsWith:
				case ClauseType.EndsWith:
				case ClauseType.Contains:
				case ClauseType.DoesNotContain:
				case ClauseType.Like:
				case ClauseType.NotLike:
					ForceAdditionalParamsCount(additionalOperands, 1);
					break;
				case ClauseType.IsNull:
				case ClauseType.IsNotNull:
				case ClauseType.IsNullOrEmpty:
				case ClauseType.IsNotNullOrEmpty:
					ForceAdditionalParamsCount(additionalOperands, 0);
					break;
				case ClauseType.Between:
				case ClauseType.NotBetween:
					ForceAdditionalParamsCount(additionalOperands, 2);
					break;
				case ClauseType.AnyOf:
				case ClauseType.NoneOf:
					break;
				case ClauseType.IsBeyondThisYear:
				case ClauseType.IsLaterThisYear:
				case ClauseType.IsLaterThisMonth:
				case ClauseType.IsNextWeek:
				case ClauseType.IsLaterThisWeek:
				case ClauseType.IsTomorrow:
				case ClauseType.IsToday:
				case ClauseType.IsYesterday:
				case ClauseType.IsEarlierThisWeek:
				case ClauseType.IsLastWeek:
				case ClauseType.IsEarlierThisMonth:
				case ClauseType.IsEarlierThisYear:
				case ClauseType.IsPriorThisYear:
					ForceAdditionalParamsCount(additionalOperands, 0);
					break;
				default:
					throw new NotImplementedException();
			}
		}
		public static bool IsValidClause(ClauseType clause, FilterColumnClauseClass clauseClass) {
			return IsValidClause(clause, clauseClass, false);
		}
		public static bool IsValidClause(ClauseType clause, FilterColumnClauseClass clauseClass, bool showIsNullOperatorsForStrings) {
			switch(clause) {
				case ClauseType.IsNotNull:
				case ClauseType.IsNull:
					return showIsNullOperatorsForStrings || clauseClass != FilterColumnClauseClass.String;
				case ClauseType.AnyOf:
				case ClauseType.NoneOf:
				case ClauseType.Equals:
				case ClauseType.DoesNotEqual:
					return clauseClass != FilterColumnClauseClass.Blob;
				case ClauseType.Contains:
				case ClauseType.DoesNotContain:
				case ClauseType.BeginsWith:
				case ClauseType.EndsWith:
				case ClauseType.Like:
				case ClauseType.NotLike:
				case ClauseType.IsNullOrEmpty:
				case ClauseType.IsNotNullOrEmpty:
					return clauseClass == FilterColumnClauseClass.String;
				case ClauseType.IsBeyondThisYear:
				case ClauseType.IsLaterThisYear:
				case ClauseType.IsLaterThisMonth:
				case ClauseType.IsNextWeek:
				case ClauseType.IsLaterThisWeek:
				case ClauseType.IsTomorrow:
				case ClauseType.IsToday:
				case ClauseType.IsYesterday:
				case ClauseType.IsEarlierThisWeek:
				case ClauseType.IsLastWeek:
				case ClauseType.IsEarlierThisMonth:
				case ClauseType.IsEarlierThisYear:
				case ClauseType.IsPriorThisYear:
					return clauseClass == FilterColumnClauseClass.DateTime;
				default:
					return clauseClass != FilterColumnClauseClass.Lookup && clauseClass != FilterColumnClauseClass.Blob;
			}
		}
	}
	public interface IDisplayCriteriaGeneratorNamesSource {
		string GetDisplayPropertyName(OperandProperty property);
		string GetValueScreenText(OperandProperty property, object value);
	}
	public class DisplayCriteriaGenerator: IClientCriteriaVisitor<CriteriaOperator> {
		public readonly IDisplayCriteriaGeneratorNamesSource NamesSource;
		protected DisplayCriteriaGenerator(IDisplayCriteriaGeneratorNamesSource namesSource) {
			this.NamesSource = namesSource;
		}
		protected virtual CriteriaOperator ProcessPossibleValue(CriteriaOperator possibleProperty, CriteriaOperator possibleValue) {
			OperandProperty prop = possibleProperty as OperandProperty;
			if(!ReferenceEquals(prop, null)) {
				OperandValue val = possibleValue as OperandValue;
				if(!ReferenceEquals(val, null)) {
					return new OperandValue(ProcessValue(prop, val.Value));
				}
			}
			return Process(possibleValue);
		}
		protected virtual OperandProperty Convert(OperandProperty theOperand) {
			if(ReferenceEquals(theOperand, null))
				return null;
			return new OperandProperty(NamesSource.GetDisplayPropertyName(theOperand));
		}
		protected virtual object ProcessValue(OperandProperty originalProperty, object originalValue) {
			return NamesSource.GetValueScreenText(originalProperty, originalValue);
		}
		public virtual CriteriaOperator Visit(OperandProperty theOperand) {
			return Convert(theOperand);
		}
		public virtual CriteriaOperator Visit(AggregateOperand theOperand) {
			return new AggregateOperand(Convert(theOperand.CollectionProperty), CriteriaOperator.Clone(theOperand.AggregatedExpression), theOperand.AggregateType, CriteriaOperator.Clone(theOperand.Condition));
		}
		public virtual CriteriaOperator Visit(JoinOperand theOperand) {
			return new JoinOperand(theOperand.JoinTypeName, CriteriaOperator.Clone(theOperand.Condition), theOperand.AggregateType, CriteriaOperator.Clone(theOperand.AggregatedExpression));
		}
		public virtual CriteriaOperator Visit(FunctionOperator theOperator) {
			switch(theOperator.OperatorType){
				case FunctionOperatorType.StartsWith:
				case FunctionOperatorType.EndsWith:
				case FunctionOperatorType.Contains:
					if(theOperator.Operands.Count == 2)
						return new FunctionOperator(theOperator.OperatorType, Process(theOperator.Operands[0]), ProcessPossibleValue(theOperator.Operands[0], theOperator.Operands[1]));
					break;
				case FunctionOperatorType.Custom:
					if(LikeCustomFunction.IsBinaryCompatibleLikeFunction(theOperator))
						return Process(LikeCustomFunction.Convert(theOperator));
					break;
			}
			FunctionOperator result = new FunctionOperator(theOperator.OperatorType);
			IEnumerable<CriteriaOperator> toProcess;
			if((theOperator.OperatorType == FunctionOperatorType.Custom || theOperator.OperatorType == FunctionOperatorType.CustomNonDeterministic) && theOperator.Operands.Count > 0 && theOperator.Operands[0] is OperandValue && ((OperandValue)theOperator.Operands[0]).Value is string) {
				result.Operands.Add(CriteriaOperator.Clone(theOperator.Operands[0]));
				toProcess = theOperator.Operands.Skip(1);
			} else {
				toProcess = theOperator.Operands;
			}
			foreach(CriteriaOperator op in toProcess)
				result.Operands.Add(Process(op));
			return result;
		}
		public virtual CriteriaOperator Visit(OperandValue theOperand) {
			return new OperandValue(CriteriaOperator.ToString(theOperand));
		}
		public virtual CriteriaOperator Visit(GroupOperator theOperator) {
			CriteriaOperator result = null;
			foreach(CriteriaOperator op in theOperator.Operands) {
				result = GroupOperator.Combine(theOperator.OperatorType, result, Process(op));
			}
			return result;
		}
		public virtual CriteriaOperator Visit(InOperator theOperator) {
			InOperator result = new InOperator(Process(theOperator.LeftOperand));
			foreach(CriteriaOperator op in theOperator.Operands)
				result.Operands.Add(ProcessPossibleValue(theOperator.LeftOperand, op));
			return result;
		}
		public virtual CriteriaOperator Visit(UnaryOperator theOperator) {
			return new UnaryOperator(theOperator.OperatorType, Process(theOperator.Operand));
		}
		public virtual CriteriaOperator Visit(BinaryOperator theOperator) {
			return new BinaryOperator(Process(theOperator.LeftOperand), ProcessPossibleValue(theOperator.LeftOperand, theOperator.RightOperand), theOperator.OperatorType);
		}
		public virtual CriteriaOperator Visit(BetweenOperator theOperator) {
			return new BetweenOperator(Process(theOperator.TestExpression), ProcessPossibleValue(theOperator.TestExpression, theOperator.BeginExpression), ProcessPossibleValue(theOperator.TestExpression, theOperator.EndExpression));
		}
		protected virtual CriteriaOperator Process(CriteriaOperator inputValue) {
			if(ReferenceEquals(inputValue, null))
				return null;
			return inputValue.Accept(this);
		}
		public static CriteriaOperator Process(IDisplayCriteriaGeneratorNamesSource namesSource, CriteriaOperator op) {
			return new DisplayCriteriaGenerator(namesSource).Process(op);
		}
	}
	public interface IDisplayCriteriaGeneratorNamesSourcePathed {
		string GetDisplayPropertyName(OperandProperty property, string fullPath);
		string GetValueScreenText(OperandProperty property, object value);
	}
	public static class DisplayCriteriaGeneratorPathed {
		class Impl: DisplayCriteriaGenerator {
			class NamesSourceWrapper: IDisplayCriteriaGeneratorNamesSource {
				readonly IDisplayCriteriaGeneratorNamesSourcePathed Pathed;
				public string GetDisplayPropertyName(OperandProperty property) {
					throw new NotSupportedException();
				}
				public string GetValueScreenText(OperandProperty property, object value) {
					return Pathed.GetValueScreenText(property, value);
				}
				public NamesSourceWrapper(IDisplayCriteriaGeneratorNamesSourcePathed pathed) {
					this.Pathed = pathed;
				}
			}
			public Impl(IDisplayCriteriaGeneratorNamesSourcePathed namesSource)
				: base(new NamesSourceWrapper(namesSource)) {
					this.PathedNamesSource = namesSource;
			}
			public readonly IDisplayCriteriaGeneratorNamesSourcePathed PathedNamesSource;
			readonly Stack<string> stackTrace = new Stack<string>();
			string PropertyFullPath {
				get {
					string res = string.Empty;
					foreach(var line in stackTrace) {
						res = line + "." + res;
					}
					return res.Trim('.');
				}
			}
			OperandProperty AwkwardConvert(OperandProperty theOperand) {
				if(ReferenceEquals(theOperand, null))
					return null;
				return new OperandProperty(PathedNamesSource.GetDisplayPropertyName(theOperand, PropertyFullPath));
			}
			public override CriteriaOperator Visit(OperandProperty theOperand) {
				stackTrace.Push(theOperand.PropertyName);
				var res = AwkwardConvert(theOperand);
				stackTrace.Pop();
				return res;
			}
			public override CriteriaOperator Visit(AggregateOperand theOperand) {
				stackTrace.Push(theOperand.CollectionProperty.PropertyName);
				CriteriaOperator condition = Process(theOperand.Condition);
				AggregateOperand aggregate = new AggregateOperand(AwkwardConvert(theOperand.CollectionProperty),
					CriteriaOperator.Clone(theOperand.AggregatedExpression), theOperand.AggregateType, condition);
				stackTrace.Pop();
				return aggregate;
			}
			public override CriteriaOperator Visit(FunctionOperator theOperator) {
				if(theOperator.OperatorType == FunctionOperatorType.Custom && !LikeCustomFunction.IsBinaryCompatibleLikeFunction(theOperator)) {
					FunctionOperator result = new FunctionOperator(theOperator.OperatorType);
					FunctionOperator clone = theOperator.Clone();
					FunctionOperator func = new FunctionOperator(PathedNamesSource.GetValueScreenText(null, clone));
					result.Operands.Add(func.Operands[0]);
					for(int i = 1; i < clone.Operands.Count; i++)
						result.Operands.Add(Process(clone.Operands[i]));
					return result;
				}
				return base.Visit(theOperator);
			}
			public CriteriaOperator Do(CriteriaOperator arg) {
				return Process(arg);
			}
		}
		public static CriteriaOperator Process(IDisplayCriteriaGeneratorNamesSourcePathed namesSource, CriteriaOperator op) {
			return new Impl(namesSource).Do(op);
		}
	}
	public interface ILocalaizableCriteriaToStringProcessorOpNamesSource {
		string GetString(GroupOperatorType opType);
		string GetString(UnaryOperatorType opType);
		string GetString(BinaryOperatorType opType);
		string GetString(FunctionOperatorType opType);
		string GetString(Aggregate opType);
		string GetIsNullString();
		string GetIsNotNullString();
		string GetBetweenString();
		string GetInString();
		string GetNotLikeString();
	}
	public class LocalaizableCriteriaToStringProcessorCore : CriteriaToStringBase {
		public readonly ILocalaizableCriteriaToStringProcessorOpNamesSource OpNamesSource;
		protected LocalaizableCriteriaToStringProcessorCore(ILocalaizableCriteriaToStringProcessorOpNamesSource opNamesSource) {
			this.OpNamesSource = opNamesSource;
		}
		public override string GetOperatorString(GroupOperatorType opType) {
			return OpNamesSource.GetString(opType);
		}
		protected override string GetIsNullText() {
			return OpNamesSource.GetIsNullString() ?? base.GetIsNullText();
		}
		public override string GetOperatorString(UnaryOperatorType opType) {
			return OpNamesSource.GetString(opType);
		}
		public override string GetOperatorString(BinaryOperatorType opType) {
			return OpNamesSource.GetString(opType);
		}
		public override CriteriaToStringVisitResult Visit(OperandValue operand) {
			return CriteriaToStringParameterlessProcessor.ValueToCriteriaToStringVisitResult(operand);
		}
		protected override string GetBetweenText() {
			return OpNamesSource.GetBetweenString() ?? base.GetBetweenText();
		}
		protected override string GetFunctionText(FunctionOperatorType operandType) {
			return OpNamesSource.GetString(operandType) ?? base.GetFunctionText(operandType);
		}
		protected override string GetInText() {
			return OpNamesSource.GetInString() ?? base.GetInText();
		}
		protected override string GetIsNotNullText() {
			return OpNamesSource.GetIsNotNullString() ?? base.GetIsNotNullText();
		}
		protected override string GetNotLikeText() {
			return OpNamesSource.GetNotLikeString() ?? base.GetNotLikeText();
		}
		protected override string GetOperatorString(Aggregate operandType) {
			return OpNamesSource.GetString(operandType) ?? base.GetOperatorString(operandType);
		}
		public static string Process(ILocalaizableCriteriaToStringProcessorOpNamesSource opNamesSource, CriteriaOperator op) {
			if(ReferenceEquals(op, null))
				return string.Empty;
			return new LocalaizableCriteriaToStringProcessorCore(opNamesSource).Process(op).Result;
		}
	}
}
