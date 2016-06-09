#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public class FormatConditionExpression : FormatConditionStyleBase, IEvaluatorRequired {
		const string XmlExpression = "Expression";
		const string DefaultExpression = "";
		ExpressionEvaluator evaluator;
		string expression = DefaultExpression;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FormatConditionExpressionExpression"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultExpression),
		Localizable(false),
		Editor(TypeNames.CalculatedFieldExpressionEditor, typeof(UITypeEditor))
		]
		public string Expression {
			get { return expression; }
			set {
				if(expression != value) {
					expression = value;
					OnChanged();
				}
			}
		}
		public FormatConditionExpression() : this(DefaultExpression) { }
		public FormatConditionExpression(string expression) {
			this.expression = expression;
		}
		protected override FormatConditionBase CreateInstance() {
			return new FormatConditionExpression();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XmlHelper.Save(element, XmlExpression, Expression, DefaultExpression);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			XmlHelper.Load<string>(element, XmlExpression, x => expression = x);
		}
		protected override void AssignCore(FormatConditionBase obj) {
			base.AssignCore(obj);
			var source = obj as FormatConditionExpression;
			if(source != null) {
				Expression = source.Expression;
			}
		}
		protected override bool IsFitCore(IFormatConditionValueProvider valueProvider) {
			if(evaluator == null) return false;
			try {
				object res = evaluator.Evaluate(valueProvider.GetExpressionObject(this));
				if(res is bool) {
					return (bool)res;
				} else {
					return Convert.ToBoolean(res);
				}
			} catch {
			}
			return false;
		}
		#region IEvaluatorRequired Members
		string IEvaluatorRequired.Expression {
			get { return Expression; }
		}
		void IEvaluatorRequired.Initialize(ExpressionEvaluator evaluator) {
			this.evaluator = evaluator;
		}
		IEnumerable<string> IEvaluatorRequired.GetDataMembers() {
			DataMembersCriteriaVisitor visitor = new DataMembersCriteriaVisitor();
			CriteriaOperator criteriaOperator = CriteriaOperator.TryParse(Expression);
			return visitor.ProcessDataMembers(criteriaOperator);
		}
		#endregion
	}
	public class DataMembersCriteriaVisitor : ClientCriteriaVisitorBase {
		IList<string> dataMembers = new List<string>();
		public IEnumerable<string> ProcessDataMembers(CriteriaOperator criteriaOperator) {
			dataMembers.Clear();
			Process(criteriaOperator);
			return dataMembers;
		}
		protected override CriteriaOperator Visit(OperandProperty theOperand) {
			string dataMember = theOperand.PropertyName;
			if(!dataMembers.Contains(dataMember))
				dataMembers.Add(dataMember);
			return base.Visit(theOperand);
		}
	}
}
