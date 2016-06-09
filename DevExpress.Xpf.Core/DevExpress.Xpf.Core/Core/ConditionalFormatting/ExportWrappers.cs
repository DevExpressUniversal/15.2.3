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

using DevExpress.XtraExport.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using DevExpress.Export.Xl;
using System.Windows;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Core.ConditionalFormatting.Printing {
	public class DateOccurringConditionRuleDetector {
		class DateOccurringOperator : DevExpress.Xpf.Core.ConditionalFormattingManager.ContainOperatorBase<DateOccurringConditionRule> {
			public DateOccurringOperator(string name, DateOccurringConditionRule rule, Func<OperandProperty, OperandValue[], CriteriaOperator> factory)
				: base(name, rule, factory, (op) => new object[0]) { }
		}
		IList<DateOccurringOperator> Operators;
		public DateOccurringConditionRuleDetector() {
			InitOperators();
		}
		void InitOperators() {
			Operators = new List<DateOccurringOperator>();
			foreach(var factory in DevExpress.Xpf.Core.ConditionalFormatting.DateOccurringConditionalFormattingDialogViewModel.GetFactories())
				Operators.Add(new DateOccurringOperator(factory.ToString(), factory.Rule, (op, _) => factory.Factory(op)));
		}
		public DateOccurringConditionRule DetectRule(string expression) {
			var criteriaOperator = CriteriaOperator.TryParse(expression);
			if(ReferenceEquals(criteriaOperator, null))
				return DateOccurringConditionRule.None;
			DateOccurringOperator dateOccurringOperator = Operators.FirstOrDefault(x => x.Match(criteriaOperator));
			if(dateOccurringOperator == null)
				return DateOccurringConditionRule.None;
			return dateOccurringOperator.Rule;
		}
	}
	public enum DateOccurringConditionRule {
		None,
		Yesterday,
		Today,
		Tomorrow,
		InTheLast7Days,
		LastWeek,
		ThisWeek,
		NextWeek,
		LastMonth,
		ThisMonth,
		NextMonth
	}
}
