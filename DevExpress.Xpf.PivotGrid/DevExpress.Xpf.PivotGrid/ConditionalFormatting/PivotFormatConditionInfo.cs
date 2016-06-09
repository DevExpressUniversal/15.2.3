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
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class PivotFormatConditionInfo : FormatConditionInfo {
		public override bool CalcCondition(FormatValueProvider provider) {
			if(ValueRule == ConditionRule.Expression) { 
				ExpressionEvaluator evaluator = new ExpressionEvaluator(new PivotFormatConditionInfoEvaluatorContextDescriptor(), CriteriaOperator.Parse(ActualExpression));
				bool? value = evaluator.Evaluate(provider) as bool?;
				return value.HasValue && value.Value;
			}
			return base.CalcCondition(provider);
		}
	}
	class PivotFormatConditionInfoEvaluatorContextDescriptor : EvaluatorContextDescriptor  {
		public override System.Collections.IEnumerable GetCollectionContexts(object source, string collectionName) {
			throw new NotImplementedException();
		}
		public override EvaluatorContext GetNestedContext(object source, string propertyPath) {
			throw new NotImplementedException();
		}
		public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) {
			return ((FormatValueProvider)source).GetCellValue(propertyPath.PropertyPath);
		}
	}
}
