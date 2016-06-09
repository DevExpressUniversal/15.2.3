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
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Web.Internal;
using DevExpress.XtraEditors.Filtering;
namespace DevExpress.Web.FilterControl {
	public class WebFilterTreeModel : FilterTreeNodeModel {
		ASPxFilterControlBase control;
		protected WebFilterTreeModelValidationHelper ValidationHelper { get; private set; }
		public WebFilterTreeModel(ASPxFilterControlBase control) {
			this.control = control;
			FilterCriteria = null;
			AllowAggregateEditing = XtraEditors.FilterControlAllowAggregateEditing.AggregateWithCondition;
			ValidationHelper = new WebFilterTreeModelValidationHelper(this);
		}
		internal ASPxFilterControlBase Control { get { return control; } }
		public override IBoundProperty CreateProperty(object dataSource, string dataMember, string displayName, bool isList, System.ComponentModel.PropertyDescriptor property) {
			var type = ReflectionUtils.StripNullableType(property.PropertyType);
			var column = new FilterControlDummyColumn(this, displayName, property.Name, isList, false, type);
			return column;
		}
		public override string GetLocalizedStringForFilterEmptyParameter() { return string.Empty; }
		public override string GetLocalizedStringForFilterEmptyEnter() { return string.Empty; }
		public override string GetLocalizedStringForFilterClauseBetweenAnd() { return string.Empty; }
		public override string GetLocalizedStringForFilterEmptyValue() { return string.Empty; }
		public override string GetMenuStringByType(DevExpress.Data.Filtering.Helpers.ClauseType type) {
			return "$unknown_clause_string"; 
		}
		public override string GetMenuStringByType(Aggregate type) {
			return "$unknown_aggregate_string"; 
		}
		public override string GetMenuStringByType(GroupType type) {
			return "$unknown_group_string"; 
		}
		public override void OnVisualChange(FilterChangedActionInternal action, Node node) {
		}
		protected override void SetFilterColumnsCollection(IBoundPropertyCollection propertyCollection) {
		}
		public override void SetParent(IBoundProperty property, IBoundProperty parent) { 
		}
		protected override IBoundPropertyCollection CreateIBoundPropertyCollection() {
			return new FilterControlColumnCollection(control);
		}
		protected override FilterColumnClauseClass GetClauseClass(IBoundProperty property) {
			return IBoundPropertyDefaults.GetDefaultClauseClass(property);
		}
		public override ClauseType GetDefaultOperation(IBoundPropertyCollection properties, OperandProperty operandProperty) {
			ClauseType operation = base.GetDefaultOperation(properties, operandProperty);
			IFilterColumn column = (IFilterColumn)FilterProperties[operandProperty.PropertyName];
			return column != null ? CorrectOperation(column, operation) : operation;
		}
		public ClauseType CorrectOperation(IFilterColumn column, ClauseType operation) {
			IFilterControlOwner owner = (IFilterControlOwner)Control;
			if(!IsValidClause(operation, column.ClauseClass))
				operation = GetDefaultOperation(column.ClauseClass);
			if(!owner.IsOperationHiddenByUser(column, operation))
				return operation;
			foreach(ClauseType clause in FilterControlOperationPopupMenu.SupportedClauses) {
				if(IsValidClause(clause, column.ClauseClass) && !owner.IsOperationHiddenByUser(column, clause))
					return clause;
			}
			return operation;
		}
		protected virtual ClauseType GetDefaultOperation(FilterColumnClauseClass clauseClass) {
			if(clauseClass == FilterColumnClauseClass.Blob)
				return ClauseType.IsNull;
			return ClauseType.Equals;
		}
		protected internal List<IBoundProperty> GetChildrenProperties(IBoundProperty property) {
			return IsPropertyPrimitive(property) ? new List<IBoundProperty>() : PickChildrenProperties(property);
		}
		bool IsPropertyPrimitive(IBoundProperty p) {
			return p.Type.IsPrimitive || p.Type == typeof(string) || p.Type == typeof(DateTime) || p.Type == typeof(decimal) || p.Type == typeof(byte[]);
		}
		List<IBoundProperty> PickChildrenProperties(IBoundProperty p) {
			var type = p.IsList ? DevExpress.Data.Helpers.GenericTypeHelper.GetGenericIListTypeArgument(p.Type) : p.Type;
			var children = PickManager.PickProperties(type, string.Empty, null);
			foreach (var child in children)
				((FilterControlDummyColumn)child).SetParent(p);
			return children;
		}
		protected internal string TextTabExpression { get; set; }
		public void ApplyTextTabExpression(string expression) {
			TextTabExpression = expression;
			ValidationHelper.Validate(expression);
			if(ValidationHelper.IsValid())
				FilterString = expression;
		}
		public bool IsTextTabExpressionValid() {
			return (TextTabExpression == null) ? true : ValidationHelper.IsValid();
		}
		public List<CriteriaValidatorError> Errors { get { return ValidationHelper.Errors; } }
		protected override CriteriaOperator CriteriaFromString(string value) {
			CriteriaOperator criteria = CriteriaOperator.Parse(value);
			if(!ReferenceEquals(criteria, null))
				criteria.Accept(new DateDiffPatchCriteriaVisitor(false));
			return criteria;
		}
		public override CriteriaOperator ToCriteria(INode node) {
			CriteriaOperator criteria = base.ToCriteria(node);
			if(!ReferenceEquals(criteria, null))
				criteria.Accept(new DateDiffPatchCriteriaVisitor(true));
			return criteria;
		}
		#region class DateDiffPatchCriteriaVisitor
		class DateDiffPatchCriteriaVisitor : IClientCriteriaVisitor {
			static List<FunctionOperatorType> KnownDiffDateOperatorTypes = new List<FunctionOperatorType> { FunctionOperatorType.AddMonths, FunctionOperatorType.AddYears };
			internal DateDiffPatchCriteriaVisitor(bool decode) {
				Decode = decode;
			}
			bool Decode { get; set; }
			#region IClientCriteriaVisitor Members
			public void Visit(JoinOperand theOperand) { }
			public void Visit(OperandProperty theOperand) { }
			public void Visit(AggregateOperand theOperand) { }
			#endregion
			#region ICriteriaVisitor Members
			public void Visit(BinaryOperator theOperator) {
				if(Decode)
					DecodeRightOperand(theOperator);
				else
					EncodeRightOperand(theOperator);
			}
			void EncodeRightOperand(BinaryOperator theOperator) {
				var funcOperator = theOperator.RightOperand as FunctionOperator;
				if(ReferenceEquals(funcOperator, null) || !KnownDiffDateOperatorTypes.Contains(funcOperator.OperatorType))
					return;
				theOperator.RightOperand = new OperandValue(theOperator.RightOperand.ToString());
			}
			void DecodeRightOperand(BinaryOperator theOperator) {
				var opValue = theOperator.RightOperand as OperandValue;
				if(ReferenceEquals(opValue, null) || opValue.Value == null || !Type.Equals(opValue.Value.GetType(), typeof(string)))
					return;
				var rightOperand = CriteriaOperator.TryParse(opValue.Value.ToString()) as FunctionOperator;
				if(!ReferenceEquals(rightOperand, null) && KnownDiffDateOperatorTypes.Contains(rightOperand.OperatorType))
					theOperator.RightOperand = rightOperand;
			}
			public void Visit(FunctionOperator theOperator) { }
			public void Visit(OperandValue theOperand) { }
			public void Visit(GroupOperator theOperator) {
				foreach(var operand in theOperator.Operands) {
					if(ReferenceEquals(operand, null))
						continue;
					operand.Accept(this);
				}
			}
			public void Visit(InOperator theOperator) { }
			public void Visit(UnaryOperator theOperator) { }
			public void Visit(BetweenOperator theOperator) { }
			#endregion
		}
		#endregion
	}
	public class WebFilterTreeModelValidationHelper {
		const string ExpressionCantBeDisplayByTreeError = "Expression can't be displayed as tree";
		public WebFilterTreeModelValidationHelper(WebFilterTreeModel model) {
			Model = model;
			Errors = new List<CriteriaValidatorError>();
		}
		public List<CriteriaValidatorError> Errors { get; private set; }
		protected WebFilterTreeModel Model { get; set; }
		public bool IsValid() {
			return Errors.Count == 0;
		}
		public void Validate(string filter) {
			var filterProperties = Model.FilterProperties.Cast<IBoundProperty>().ToList();
			var predefinedPropertiesVisitor = new PredefinedOperandPropertiesVisitor(filterProperties);
			predefinedPropertiesVisitor.Validate(Model.FilterCriteria);
			var validator = new ErrorsEvaluatorCriteriaValidatorWeb(filterProperties, predefinedPropertiesVisitor.PredefinedOperandProperties);
			validator.Validate(filter);
			Errors = validator.GetErrors();
			CriteriaOperator criteria;
			var parsedSuccessfully = TryCriteriaParse(filter, out criteria);
			if(!parsedSuccessfully || Errors.Count > 0)
				return;
			if(!CanBeDisplayedByTree(criteria) && !string.IsNullOrEmpty(filter))
				Errors.Add(new CriteriaValidatorError(ExpressionCantBeDisplayByTreeError));
		}
		bool CanBeDisplayedByTree(CriteriaOperator criteria) {
			List<CriteriaOperator> skippedCriteriaOperator = new List<CriteriaOperator>();
			CriteriaToTreeProcessor.GetTree(new FilterControlNodesFactory(Model), criteria, skippedCriteriaOperator);
			return skippedCriteriaOperator.Count == 0;
		}
		bool TryCriteriaParse(string filter, out CriteriaOperator criteria) {
			criteria = null;
			try { criteria = Model.CriteriaParse(filter); }
			catch { return false; }
			return true;
		}
		class ErrorsEvaluatorCriteriaValidatorWeb : ErrorsEvaluatorCriteriaValidator {
			public List<OperandProperty> PredefinedOperandProperties { get; set; }
			public ErrorsEvaluatorCriteriaValidatorWeb(List<IBoundProperty> properties, List<OperandProperty> predefinedOperandProperties) : base(properties) { 
				PredefinedOperandProperties = predefinedOperandProperties;
			}
			public List<CriteriaValidatorError> GetErrors() {
				return this.errors;
			}
			public override void Visit(OperandProperty theOperand) {
				if(!PredefinedOperandProperties.Contains(theOperand))
					base.Visit(theOperand);
			}
		}
		class PredefinedOperandPropertiesVisitor : ErrorsEvaluatorCriteriaValidator {
			public List<OperandProperty> PredefinedOperandProperties { get; set; }
			public PredefinedOperandPropertiesVisitor(List<IBoundProperty> properties) : base(properties) {
				PredefinedOperandProperties = new List<OperandProperty>();
			}
			public override void Visit(OperandProperty theOperand) {
				IBoundProperty property = GetFilterPropertyByPropertyName(theOperand);
				if(property == null)
					PredefinedOperandProperties.Add(theOperand);
			}
		}
	}
	public static class NodeExtensions {
		public static int GetIndex(this Node node) {
			if(node.RootNode != null)
				return node.RootNode.GetFullChildrenList().IndexOf(node);
			return -1;
		}
		public static List<Node> GetFullChildrenList(this Node node) {
			var list = new List<Node>();
			GetFullChildrenList(node, list);
			return list;
		}
		static void GetFullChildrenList(Node node, List<Node> list) {
			list.Add(node);
			foreach(Node subNode in GetSubNodes(node))
				GetFullChildrenList(subNode, list);
		}
		static IList<INode> GetSubNodes(Node node) {
			var result = new List<INode>();
			var groupNode = node as GroupNode;
			if(groupNode != null)
				return groupNode.SubNodes;
			var aggregateNode = node as AggregateNode;
			if(aggregateNode != null)
				result.Add(aggregateNode.AggregateCondition);
			return result;
		}
	}
}
