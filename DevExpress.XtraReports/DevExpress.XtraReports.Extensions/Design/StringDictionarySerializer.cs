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
using System.ComponentModel.Design.Serialization;
using System.CodeDom;
using System.ComponentModel;
using System.Collections;
namespace DevExpress.XtraReports.Design {
	public class StringDictionarySerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			return null;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			object obj2 = null;
			IDictionary<String, String> dictionary = value as IDictionary<String, String>;
			if(dictionary == null) {
				return obj2;
			}
			object current = manager.Context.Current;
			ExpressionContext context = current as ExpressionContext;
			if(context != null) {
				current = context.Expression;
			}
			CodePropertyReferenceExpression targetObject = current as CodePropertyReferenceExpression;
			if(targetObject == null) {
				return obj2;
			}
			object component = base.DeserializeExpression(manager, null, targetObject.TargetObject);
			if((component == null) || (TypeDescriptor.GetProperties(component)[targetObject.PropertyName] == null)) {
				return obj2;
			}
			CodeStatementCollection statements = new CodeStatementCollection();
			CodeMethodReferenceExpression expression2 = new CodeMethodReferenceExpression(targetObject, "Add");
			foreach(KeyValuePair<String, String> entry in dictionary) {
				CodeExpression expression3 = base.SerializeToExpression(manager, entry.Key);
				CodeExpression expression4 = base.SerializeToExpression(manager, entry.Value);
				if((expression3 != null) && (expression4 != null)) {
					CodeMethodInvokeExpression expression5 = new CodeMethodInvokeExpression();
					expression5.Method = expression2;
					expression5.Parameters.Add(expression3);
					expression5.Parameters.Add(expression4);
					statements.Add(expression5);
				}
			}
			return statements;
		}
	}
}
