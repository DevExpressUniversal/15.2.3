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
using System.Linq;
using System.Collections.Generic;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using DevExpress.Xpo.Metadata;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.ExpressApp.Xpo {
	public class XpoExpressionMemberInfo : XPCustomMemberInfo {
		private String expression;
		private ExpressionEvaluator expressionEvaluator;
		private Type propertyType; 
		private void InitExpressionEvaluator() {
			expressionEvaluator = new ExpressionEvaluator(Owner.GetEvaluatorContextDescriptor(), CriteriaOperator.Parse(expression));
			RemoveAttribute(typeof(PersistentAliasAttribute));
			AddAttribute(new PersistentAliasAttribute(expression));
		}
		public XpoExpressionMemberInfo(XPClassInfo owner, String propertyName, Type propertyType, String expression)
			: base(owner, propertyName, propertyType, null, true, false) {
			this.propertyType = propertyType;
			this.expression = expression;
			InitExpressionEvaluator();
		}
		public String Expression {
			get { return expression; }
			set {
				if(expression != value) {
					expression = value;
					InitExpressionEvaluator();
				}
			}
		}
		public override Type MemberType {
			get { return propertyType; }
		}
		public Type PropertyType {
			get { return propertyType; }
			set { propertyType = value; }
		}
		public override Object GetValue(Object obj) {
			return expressionEvaluator.Evaluate(obj);
		}
	}
}
