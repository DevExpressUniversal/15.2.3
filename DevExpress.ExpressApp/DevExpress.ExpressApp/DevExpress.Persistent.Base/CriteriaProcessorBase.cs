#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Model;
namespace DevExpress.Persistent.Base {
	public class CriteriaProcessorBase : IClientCriteriaVisitor {
		protected Int32 AggregateLevel { get; private set; }
		protected virtual void Process(OperandProperty operand) { }
		protected virtual void Process(OperandValue operand) { }
		protected virtual void Process(AggregateOperand operand) { }
		protected virtual void Process(JoinOperand operand) { }
		protected virtual void Process(FunctionOperator theOperator) { }
		protected virtual void Process(GroupOperator theOperator) { }
		protected virtual void Process(InOperator theOperator) { }
		protected virtual void Process(UnaryOperator theOperator) { }
		protected virtual void Process(BinaryOperator theOperator) { }
		protected virtual void Process(BetweenOperator theOperator) { }
		public void Process(CriteriaOperator criteria) {
			if(!Object.ReferenceEquals(criteria, null)) {
				criteria.Accept(this);
			}
		}
		void IClientCriteriaVisitor.Visit(OperandProperty operand) {
			Process(operand);
		}
		void ICriteriaVisitor.Visit(OperandValue operand) {
			Process(operand);
		}
		void IClientCriteriaVisitor.Visit(AggregateOperand operand) {
			Process(operand);
			AggregateLevel++;
			if(!CriteriaOperator.ReferenceEquals(operand.AggregatedExpression, null)) {
				operand.AggregatedExpression.Accept(this);
			}
			if(!CriteriaOperator.ReferenceEquals(operand.CollectionProperty, null)) {
				operand.CollectionProperty.Accept(this);
			}
			AggregateLevel--;
		}
		void IClientCriteriaVisitor.Visit(JoinOperand operand) {
			Process(operand);
			if(!CriteriaOperator.ReferenceEquals(operand.AggregatedExpression, null)) {
				operand.AggregatedExpression.Accept(this);
			}
		}
		void ICriteriaVisitor.Visit(FunctionOperator theOperator) {
			Process(theOperator);
			foreach(CriteriaOperator operand in theOperator.Operands) {
				operand.Accept(this);
			}
		}
		void ICriteriaVisitor.Visit(GroupOperator theOperator) {
			Process(theOperator);
			foreach(CriteriaOperator operand in theOperator.Operands) {
				operand.Accept(this);
			}
		}
		void ICriteriaVisitor.Visit(InOperator theOperator) {
			Process(theOperator);
			if(!CriteriaOperator.ReferenceEquals(theOperator.LeftOperand, null)) {
				theOperator.LeftOperand.Accept(this);
			}
			foreach(CriteriaOperator operand in theOperator.Operands) {
				operand.Accept(this);
			}
		}
		void ICriteriaVisitor.Visit(UnaryOperator theOperator) {
			Process(theOperator);
			if(!CriteriaOperator.ReferenceEquals(theOperator.Operand, null)) {
				theOperator.Operand.Accept(this);
			}
		}
		void ICriteriaVisitor.Visit(BinaryOperator theOperator) {
			Process(theOperator);
			if(!CriteriaOperator.ReferenceEquals(theOperator.LeftOperand, null)) {
				theOperator.LeftOperand.Accept(this);
			}
			if(!CriteriaOperator.ReferenceEquals(theOperator.RightOperand, null)) {
				theOperator.RightOperand.Accept(this);
			}
		}
		void ICriteriaVisitor.Visit(BetweenOperator theOperator) {
			Process(theOperator);
			if(!CriteriaOperator.ReferenceEquals(theOperator.BeginExpression, null)) {
				theOperator.BeginExpression.Accept(this);
			}
			if(!CriteriaOperator.ReferenceEquals(theOperator.EndExpression, null)) {
				theOperator.EndExpression.Accept(this);
			}
			if(!CriteriaOperator.ReferenceEquals(theOperator.TestExpression, null)) {
				theOperator.TestExpression.Accept(this);
			}
		}
	}
	public class EnumPropertyValueCriteriaProcessor : CriteriaProcessorBase {
		private ITypeInfo objectTypeInfo;
		public EnumPropertyValueCriteriaProcessor(ITypeInfo objectTypeInfo)
			: base() {
			this.objectTypeInfo = objectTypeInfo;
		}
		protected ITypeInfo ObjectTypeInfo { get { return objectTypeInfo; } }
		protected Type GetPropertyType(String fieldName) {
			IMemberInfo memberInfo = null;
			if(ObjectTypeInfo != null) {
				memberInfo = ObjectTypeInfo.FindMember(fieldName);
			}
			return memberInfo != null ? memberInfo.MemberType : null;
		}
		protected virtual object GetEnumValue(Type enumType, Object value) {
			object enumValue = value;
			if(enumType != null && enumType.IsEnum && !(value is Enum)) {
				try {
					enumValue = Enum.Parse(enumType, value.ToString());
				}
				catch { }
			}
			return enumValue;
		}
		protected virtual string GetLocalizedPropertyName(String propertyName) {
			return propertyName;
		}
		protected override void Process(BinaryOperator theOperator) {
			OperandValue operandValue = null;
			OperandProperty operandProperty = null;
			if((theOperator.LeftOperand is OperandProperty) && (theOperator.RightOperand is OperandValue)) {
				operandProperty = (OperandProperty)theOperator.LeftOperand;
				operandValue = (OperandValue)theOperator.RightOperand;
			}
			else if((theOperator.LeftOperand is OperandValue) && (theOperator.RightOperand is OperandProperty)) {
				operandProperty = (OperandProperty)theOperator.RightOperand;
				operandValue = (OperandValue)theOperator.LeftOperand;
			}
			if(!ReferenceEquals(operandProperty, null) && !ReferenceEquals(operandValue, null)) {
				Type type = GetPropertyType(operandProperty.PropertyName);
				operandProperty.PropertyName = GetLocalizedPropertyName(operandProperty.PropertyName);
				operandValue.Value = GetEnumValue(type, operandValue.Value);
			}
		}
		protected override void Process(InOperator theOperator) {
			if(theOperator.LeftOperand is OperandProperty) {
				OperandProperty operandProperty = (OperandProperty)theOperator.LeftOperand;
				Type type = GetPropertyType(operandProperty.PropertyName);
				operandProperty.PropertyName = GetLocalizedPropertyName(operandProperty.PropertyName);
				foreach(CriteriaOperator operand in theOperator.Operands) {
					((OperandValue)operand).Value = GetEnumValue(type, ((OperandValue)operand).Value);
				}
			}
		}
	}
	public class EnumLocalizationCriteriaProcessor : EnumPropertyValueCriteriaProcessor {
		private IModelColumns columns;
		protected IModelColumns Columns { get { return columns; } }
		public EnumLocalizationCriteriaProcessor(ITypeInfo objectTypeInfo, IModelColumns columns)
			: base(objectTypeInfo) {
			this.columns = columns;
		}
		protected override object GetEnumValue(Type enumType, object value) {
			object localizedValue = value;
			if(enumType != null && enumType.IsEnum) {
				localizedValue = (new EnumDescriptor(enumType)).GetCaption(value);
			}
			return localizedValue;
		}
		protected override string GetLocalizedPropertyName(string propertyName) {
			string columnName = propertyName;
			foreach(IModelColumn columnInfo in Columns) {
				if(propertyName.Equals(columnInfo.PropertyName)) {
					columnName = columnInfo.Caption;
					break;
				}
			}
			return columnName;
		}
	}
}
