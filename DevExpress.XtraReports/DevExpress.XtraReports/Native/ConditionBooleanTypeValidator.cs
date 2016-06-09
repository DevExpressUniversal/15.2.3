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
using DevExpress.Data.Filtering;
using DevExpress.XtraReports.Native.Data;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.Localization;
using System.Windows.Forms;
using DevExpress.XtraReports.Design;
using System.ComponentModel;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.XtraReports.Native {
	public class ConditionBooleanTypeValidator : IClientCriteriaVisitor<bool> {
		public static string GetExceptionString(CriteriaParserException exception) {
			return String.Format(ReportLocalizer.GetString(ReportStringId.Msg_InvalidExpression), exception.Line + 1, exception.Column);
		}
		CriteriaOperator criteriaOperator;
		PropertyDescriptorCollection properties;
		public ConditionBooleanTypeValidator(object source, string dataMember) {
			using(XRDataContext dataContext = new XRDataContext(null, true)) {
				properties = dataContext.GetListItemProperties(source, dataMember);
			}
		}
		public CriteriaOperator CriteriaOperator {
			get { return criteriaOperator; }
			set { criteriaOperator = value; }
		}
		public bool ValidateCondition(string condition) {
			try {
				this.criteriaOperator = CriteriaOperator.Parse(condition, null);
				return ValidateCondition(this.criteriaOperator);
			} catch(CriteriaParserException exception) {
				throw (new CriteriaParserException(ConditionBooleanTypeValidator.GetExceptionString(exception), exception.Line, exception.Column));
			}
		}
		public bool ValidateCondition(CriteriaOperator criteriaOperator) {
			return !object.ReferenceEquals(criteriaOperator, null) && criteriaOperator.Accept(this);
		}
		#region CriteriaVisitor Members
		bool IClientCriteriaVisitor<bool>.Visit(OperandProperty theOperand) {
			PropertyDescriptor property = properties[theOperand.PropertyName];
			return property != null && property.PropertyType == typeof(bool);
		}
		bool IClientCriteriaVisitor<bool>.Visit(AggregateOperand theOperand) {
			return theOperand.AggregateType == Aggregate.Exists;
		}
		bool IClientCriteriaVisitor<bool>.Visit(JoinOperand theOperand) {
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(FunctionOperator theOperator) {
			if(theOperator.OperatorType == FunctionOperatorType.Iif) {
				for(int i = 0; i < theOperator.Operands.Count - 1; i += 2) {
					if(!Process(theOperator.Operands[i]) || !Process(theOperator.Operands[i + 1]))
						return false;
				}
				return Process(theOperator.Operands[theOperator.Operands.Count - 1]);
			} else if(theOperator.OperatorType == FunctionOperatorType.IsNull)
				return theOperator.Operands.Count == 1;
			switch(theOperator.OperatorType) {
				case FunctionOperatorType.IsOutlookIntervalBeyondThisYear:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisMonth:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisWeek:
				case FunctionOperatorType.IsOutlookIntervalEarlierThisYear:
				case FunctionOperatorType.IsOutlookIntervalLastWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisMonth:
				case FunctionOperatorType.IsOutlookIntervalLaterThisWeek:
				case FunctionOperatorType.IsOutlookIntervalLaterThisYear:
				case FunctionOperatorType.IsOutlookIntervalNextWeek:
				case FunctionOperatorType.IsOutlookIntervalPriorThisYear:
				case FunctionOperatorType.IsOutlookIntervalToday:
				case FunctionOperatorType.IsOutlookIntervalTomorrow:
				case FunctionOperatorType.IsOutlookIntervalYesterday:
				case FunctionOperatorType.IsNullOrEmpty:
				case FunctionOperatorType.StartsWith:
				case FunctionOperatorType.EndsWith:
				case FunctionOperatorType.Contains:
				case FunctionOperatorType.IsThisMonth:
				case FunctionOperatorType.IsThisWeek:
				case FunctionOperatorType.IsThisYear:
				return true;
			}
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(OperandValue theOperand) {
			return theOperand.Value != null ? theOperand.Value.GetType() == typeof(bool) : false;
		}
		bool ICriteriaVisitor<bool>.Visit(GroupOperator theOperator) {
			return true;
		}
		bool ICriteriaVisitor<bool>.Visit(InOperator theOperator) {
			return true;
		}
		bool ICriteriaVisitor<bool>.Visit(UnaryOperator theOperator) {
			switch(theOperator.OperatorType) {
				case UnaryOperatorType.IsNull:
				case UnaryOperatorType.Not:
				return true;
			}
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(BinaryOperator theOperator) {
			switch(theOperator.OperatorType) {
				case BinaryOperatorType.Equal:
				case BinaryOperatorType.Greater:
				case BinaryOperatorType.GreaterOrEqual:
				case BinaryOperatorType.Less:
				case BinaryOperatorType.LessOrEqual:
#pragma warning disable 618
				case BinaryOperatorType.Like:
#pragma warning restore 618
				case BinaryOperatorType.NotEqual:
				return true;
			}
			return false;
		}
		bool ICriteriaVisitor<bool>.Visit(BetweenOperator theOperator) {
			return true;
		}
		bool Process(CriteriaOperator operand) {
			if(ReferenceEquals(operand, null))
				return true;
			return operand.Accept(this);
		}
		#endregion
	}
}
