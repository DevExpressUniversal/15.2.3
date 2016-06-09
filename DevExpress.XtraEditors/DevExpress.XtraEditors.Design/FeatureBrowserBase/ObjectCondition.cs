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
using System.Collections;
using System.ComponentModel;
using DevExpress.Utils.Design;
using DevExpress.Data.Filtering.Helpers;
namespace DevExpress.XtraEditors.Design {
	public class ObjectCondition {
		object sourceObject;
		string condition;
		public ObjectCondition(object sourceObject, string condition) : this(sourceObject, condition, string.Empty) {}
		public ObjectCondition(object sourceObject, string condition, string sourceProperty) {
			this.sourceObject = new ObjectValueGetter(sourceObject, sourceProperty).GetterObject;
			this.condition = condition.Trim().Replace("!=", "<>");
		}
		public object SourceObject { get { return sourceObject; } }
		public string Condition { get { return condition; } }
		public bool Run() {
			if(IsCollectionCondition)
				return RunCollectionCondition();
			else return RunObjectCondition();
		}
		const string stringAny = "Any.";
		protected bool IsCollectionCondition { get { return Condition.StartsWith(stringAny); } }
		protected bool RunCollectionCondition() {
			string colCondition = Condition.Remove(0, stringAny.Length);
			int indexOf = colCondition.IndexOf('(');
			string colObjectName = indexOf > -1 ? colCondition.Substring(0, indexOf) : string.Empty;
			object colObject = new ObjectValueGetter(SourceObject).GetValue(colObjectName);
			ICollection collection = colObject as ICollection;
			if(collection == null) return false;
			colCondition = colCondition.Remove(0, indexOf);
			foreach(object obj in collection) {
				if(Run(colCondition, obj))
					return true;
			}
			return false;
		}
		protected bool RunObjectCondition() {
			return Run(Condition, SourceObject);
		}
		protected bool Run(string condition, object src) {
			if(condition == string.Empty)
				return true;
			ExpressionEvaluator evaluator = new ExpressionEvaluator(TypeDescriptor.GetProperties(src), condition);
			return evaluator.Fit(src);
		}
	}
}
