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
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.PivotGrid.CriteriaVisitors;
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class QueryMemberEvaluatorBase : EvaluatorContextDescriptor {
		internal static string ErrorValue = "XtraPivotGridError";
		protected QueryMemberEvaluatorBase() { }
		public override System.Collections.IEnumerable GetCollectionContexts(object source, string collectionName) {
			throw new NotImplementedException();
		}
		public override EvaluatorContext GetNestedContext(object source, string propertyPath) {
			throw new NotImplementedException();
		}
		public static void GetPathAndSuffix(string propertyPath, out string path, out string suffix) {
			path = propertyPath;
			suffix = null;
			if(propertyPath.StartsWith("[") && !propertyPath.EndsWith("]")) {
				path = path.Substring(0, path.LastIndexOf('.'));
				suffix = propertyPath.Substring(propertyPath.LastIndexOf('.') + 1);
			}
			if(!propertyPath.StartsWith("[") && path.LastIndexOf('.') > 0) {
				path = path.Substring(0, path.LastIndexOf('.'));
				suffix = propertyPath.Substring(propertyPath.LastIndexOf('.') + 1);
			}
		}
	}
	public class QueryCriteriaOperatorVisitor : CriteriaPatcherBase {
		Dictionary<string, string> columns;
		public QueryCriteriaOperatorVisitor(Dictionary<string, string> columns) {
			this.columns = columns;
		}
		public override CriteriaOperator Visit(OperandProperty theOperand) {
			string path, suffix;
			QueryMemberEvaluatorBase.GetPathAndSuffix(theOperand.PropertyName, out path, out suffix);
			return columns.ContainsKey(path) ? new OperandProperty(string.IsNullOrEmpty(suffix) ? columns[path] : columns[path] + "." + suffix) : null;
		}
	}
}
