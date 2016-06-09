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
using System.Text;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Filtering;
using System.Web.UI;
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Data;
namespace DevExpress.Web.FilterControl {
	public enum FilterChangedAction { Value, FieldName, Operation, Aggregate, AggregateProperty, GroupType, Remove, AddCondition, AddGroup, Clear, AddValue, 
		ApplyTextTabExpression, ChangeOperandType, ChangeOperandPropertyValue };
	public abstract class WebFilterOperationsBase {
		WebFilterTreeModel model;
		public WebFilterOperationsBase(string text, WebFilterTreeModel model) {
			FilterTreeCloner.FromString(text, new FilterControlNodesFactory(model));
			this.model = model;
		}
		public WebFilterOperationsBase(GroupNode rootNode) {
		}
		public override string ToString() {
			return FilterTreeCloner.ToString(RootNode);
		}
		public WebFilterTreeModel Model { get { return model; } }
		protected abstract string GetDefaultPropertyName();
		protected abstract Type GetTypeByPropertyName(string propertyName);
		protected abstract FilterColumnClauseClass GetClauseClassByPropertyName(string propertyName);
		public GroupNode RootNode { get { return model.RootNode; } }
		public void Perform(params string[] parameters) {
			if(parameters.Length == 0) return;
			FilterChangedAction action = (FilterChangedAction)Enum.Parse(typeof(FilterChangedAction), parameters[0]);
			int index = -1;
			if(parameters.Length > 1) {
				if(!int.TryParse(parameters[1], out index)) {
					index = -1;
				}
			}
			string[] newParameters = new string[Math.Max(0, parameters.Length - 2)];
			if(newParameters.Length > 0) {
				Array.Copy(parameters, 2, newParameters, 0, newParameters.Length); 
			}
			Perform(action, index, newParameters);
		}
		public void Perform(FilterChangedAction action, int index, params string[] parameters) {
			Node node = index > -1 ? RootNode.GetFullChildrenList()[index] : null;
			Perform(action, node, parameters);
		}
		protected void Perform(FilterChangedAction action, Node node, params string[] parameters) {
			switch(action) {
				case FilterChangedAction.Value:
					RequireClauseNode(action, node);
					RequireParameters(action, 2, parameters);
					ChangeValue((ClauseNode)node, parameters);
					break;
				case FilterChangedAction.FieldName:
					RequireClauseNode(action, node);
					RequireParameters(action, 1, parameters);
					ChangeFieldName((ClauseNode)node, parameters[0]);
					break;
				case FilterChangedAction.Operation:
					RequireClauseNode(action, node);
					RequireParameters(action, 1, parameters);
					ChangeOperation((ClauseNode)node, (ClauseType)Enum.Parse(typeof(ClauseType), parameters[0]));
					break;
				case FilterChangedAction.Aggregate:
					RequireClauseNode(action, node);
					RequireParameters(action, 1, parameters);
					ChangeAggregate((AggregateNode)node, (Aggregate)Enum.Parse(typeof(Aggregate), parameters[0]));
					break;
				case FilterChangedAction.AggregateProperty:
					RequireClauseNode(action, node);
					RequireParameters(action, 1, parameters);
					ChangeAggregateProperty((ClauseNode)node, parameters[0]);
					break;
				case FilterChangedAction.GroupType:
					RequireGroupNode(action, node);
					RequireParameters(action, 1, parameters);
					ChangeGroupType((GroupNode)node, (GroupType)Enum.Parse(typeof(GroupType), parameters[0]));					
					break;
				case FilterChangedAction.Remove:
					RequireNode(action, node);
					model.FocusInfo = new FilterControlFocusInfo(node, 0);
					RemoveNode(node);
					break;
				case FilterChangedAction.AddGroup:
					RequireGroupNode(action, node);
					AddGroup((GroupNode)node);
					break;
				case FilterChangedAction.AddCondition:
					RequireGroupNode(action, node);
					AddCondition((GroupNode)node);					
					break;
				case FilterChangedAction.AddValue:
					RequireClauseNode(action, node);
					AddValue((ClauseNode)node);
					break;
				case FilterChangedAction.ChangeOperandType:
					RequireClauseNode(action, node);
					ChangeOperandType((ClauseNode)node, Convert.ToInt32(parameters[0]));
					break;
				case FilterChangedAction.ChangeOperandPropertyValue:
					RequireClauseNode(action, node);
					ChangeOperandPropertyValue((ClauseNode)node, Convert.ToInt32(parameters[0]), parameters[1]);
					break;
				case FilterChangedAction.Clear: 
					Clear(); 
					break;
				case FilterChangedAction.ApplyTextTabExpression:
					Model.ApplyTextTabExpression(parameters[0]);
					break;
			}
		}
		void RequireNode(FilterChangedAction action, Node node) {
			if(node == null) new ArgumentException(string.Format("The '{0}' action requires a node", action));
		}
		void RequireClauseNode(FilterChangedAction action, Node node) {
			ClauseNode clauseNode = node as ClauseNode;
			if(clauseNode == null) new ArgumentException(string.Format("The '{0}' action requires a clause node", action)); 
		}
		void RequireGroupNode(FilterChangedAction action, Node node) {
			GroupNode groupNode = node as GroupNode;
			if(groupNode == null) new ArgumentException(string.Format("The '{0}' action requires a group node", action));
		}
		void RequireParameters(FilterChangedAction action, int parametersCount, params string[] parameters) {
			if(parametersCount < parameters.Length) new ArgumentException(string.Format("The '{0}' action requires at least {1} parameters", action, parametersCount)); 
		}
		protected virtual void Clear() {
			RootNode.SubNodes.Clear();
		}
		protected virtual void ChangeFieldName(ClauseNode node, string fieldName) {
			node.ChangeElement(ElementType.Property, fieldName);
			CorrectOperation(node);
		}
		protected virtual void ChangeValue(ClauseNode node, string[] parameters) {
			int index;
			if(!int.TryParse(parameters[0], out index)) return;
			if (index < 0 || index >= node.AdditionalOperands.Count) return;
			ChangeValue(node, index, parameters[1]);
		}
		void ChangeValue(ClauseNode node, int index, string value) {
			FunctionOperatorType opType;
			if(TryGetDateTimeFunctionOperator(node, value, out opType))
				node.AdditionalOperands[index] = new FunctionOperator(opType);
			else
				node.ChangeValue(index, ConvertValue(node, value));
		}
		bool TryGetDateTimeFunctionOperator(ClauseNode node, string value, out FunctionOperatorType opType) {
			opType = FunctionOperatorType.None;
			var property = node.Property as IFilterColumn;
			return property != null && property.PropertyType == typeof(DateTime) && Enum.TryParse<FunctionOperatorType>(value, out opType);
		}
		protected virtual void AddValue(ClauseNode node) {
			node.AdditionalOperands.Add(new OperandValue());
		}
		protected virtual void ChangeOperation(ClauseNode node, ClauseType operation) {
			node.ChangeElement(ElementType.Operation, operation);
		}
		protected virtual void ChangeAggregate(AggregateNode node, Aggregate aggregateOperation) {
			node.ChangeElement(ElementType.AggregateOperation, aggregateOperation);
			node.AggregateCondition = Model.CreateGroupNode();
			AddGroup(Model.CreateGroupNode());
		}
		protected virtual void ChangeAggregateProperty(ClauseNode node, string aggregateProperty) {
			node.ChangeElement(ElementType.AggregateProperty, aggregateProperty);
		}
		protected virtual void ChangeGroupType(GroupNode node, GroupType nodeType) {
			node.ChangeElement(ElementType.Group, nodeType);
		}
		protected virtual void RemoveNode(Node node) {
			Debug.Assert(model.FocusInfo.Node == node, "focus info should be set by the caller");
			node.DeleteElement();
		}
		GroupType GetReverseGroupType(GroupType groupType) {
			switch(groupType) {
				case GroupType.And: return GroupType.Or;
				case GroupType.NotAnd: return GroupType.NotOr;
				case GroupType.NotOr: return GroupType.NotAnd;
				default: return GroupType.And;
			}
		}
		protected virtual void AddGroup(GroupNode node) {
			GroupNode newGroup = model.CreateGroupNode();
			node.AddNode(newGroup);
			AddCondition(newGroup);
		}
		protected virtual void AddCondition(GroupNode node) {
			node.AddElement();
		}
		void ChangeOperandType(ClauseNode clauseNode, int valueIndex) {
			var op = clauseNode.AdditionalOperands[valueIndex];
			if(op is OperandValue)
				ChangeOperandPropertyValue(clauseNode, valueIndex, clauseNode.Property.Name);
			else
				clauseNode.ChangeValue(valueIndex, null);
		}
		void ChangeOperandPropertyValue(ClauseNode clauseNode, int valueIndex, string propertyName) {
			clauseNode.AdditionalOperands[valueIndex] = new OperandProperty(propertyName);
		}
		protected virtual void CorrectOperation(ClauseNode clauseNode) {
		}
		object ConvertValue(ClauseNode node, string text) {
			object value;
			string propertyName = "";
			if(node.Property != null) {
				propertyName = node.Property.GetFullName();
			}
			else if(!object.ReferenceEquals(node.FirstOperand, null)) {
				propertyName = node.FirstOperand.PropertyName;
			}
			if(TryConverValueCustom(propertyName, text, out value))
				return value;
			return ConverValueDefault(node, text);
		}
		protected virtual bool TryConverValueCustom(string propertyName, string text, out object value) {
			value = null;
			return false;
		}
		object ConverValueDefault(ClauseNode node, string text) {
			if(object.ReferenceEquals(node.FirstOperand, null))
				return text;
			if(string.IsNullOrEmpty(node.FirstOperand.PropertyName))
				return text;
			string propertyName = node.Property != null ? node.Property.GetFullNameWithLists() : node.FirstOperand.PropertyName;
			Type propertyType = GetTypeByPropertyName(propertyName);
			var aggregateNode = node as AggregateNode;
			if(aggregateNode != null) 
				propertyType = aggregateNode.AggregateProperty != null ? aggregateNode.AggregateProperty.Type : typeof(int);
			return ConverValueDefaultCore(propertyType, text);
		}
		object ConverValueDefaultCore(Type type, string text) {
			try {
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				if(converter == null || !converter.CanConvertFrom(typeof(string)))
					return Convert.ChangeType(text, type, System.Globalization.CultureInfo.InvariantCulture);
				return converter.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, text);
			} catch {
				return null;
			}
		}
	}
}
