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
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.ExpressApp.DC;
namespace DevExpress.ExpressApp.Utils {
	public class ExpressionTypeResolver : CriteriaTypeResolverBase, IClientCriteriaVisitor<CriteriaTypeResolverResult> {
		private ITypeInfo objectTypeInfo;
		public ExpressionTypeResolver(ITypeInfo objectTypeInfo) {
			this.objectTypeInfo = objectTypeInfo;
		}
		public Type GetExpressionType(CriteriaOperator expression) {
			return base.Process(expression).Type;
		}
		CriteriaTypeResolverResult IClientCriteriaVisitor<CriteriaTypeResolverResult>.Visit(JoinOperand theOperand) {
			throw new NotImplementedException();
		}
		CriteriaTypeResolverResult IClientCriteriaVisitor<CriteriaTypeResolverResult>.Visit(OperandProperty theOperand) {
			IMemberInfo memberInfo = objectTypeInfo.FindMember(theOperand.PropertyName);
			if(memberInfo != null) {
				return new CriteriaTypeResolverResult(memberInfo.MemberType);
			}
			else {
				return new CriteriaTypeResolverResult(typeof(Object));
			}
		}
		CriteriaTypeResolverResult IClientCriteriaVisitor<CriteriaTypeResolverResult>.Visit(AggregateOperand theOperand) {
			if(theOperand.AggregateType == Aggregate.Count) {
				return new CriteriaTypeResolverResult(typeof(Int32));
			}
			else if(theOperand.AggregateType == Aggregate.Exists) {
				return new CriteriaTypeResolverResult(typeof(Boolean));
			}
			else {
				return new CriteriaTypeResolverResult(typeof(Object));
			}
		}
	}
}
