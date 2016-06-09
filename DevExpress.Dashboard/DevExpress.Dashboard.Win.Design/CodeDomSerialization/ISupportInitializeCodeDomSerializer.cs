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

using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.DashboardWin.Design {
	public class ISupportInitializeCodeDomSerializer : CodeDomSerializer {
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			bool isComplete;
			CodeExpression expression = SerializeCreationExpression(manager, value, out isComplete);
			if(expression != null && !isComplete) {
				ExpressionContext context = manager.Context[typeof(ExpressionContext)] as ExpressionContext;
				bool isPresetValue = context != null && object.ReferenceEquals(context.PresetValue, value);
				if(!isPresetValue) {
					CodeStatementCollection statements = new CodeStatementCollection();
					string uniqueName = GetUniqueName(manager, value);
					CodeVariableDeclarationStatement statement = new CodeVariableDeclarationStatement(value.GetType(), uniqueName);
					statement.InitExpression = expression;
					statements.Add(statement);
					SetExpression(manager, value, new CodeVariableReferenceExpression(uniqueName));
					ISupportInitialize supportInitialize = value as ISupportInitialize;
					if(supportInitialize != null)
						statements.Add(SerializeSupportInitialize(uniqueName, true));
					SerializeProperties(manager, statements, value, null);
					SerializeEvents(manager, statements, value, null);
					if(supportInitialize != null)
						statements.Add(SerializeSupportInitialize(uniqueName, false));
					return statements;
				}
			}
			return null;
		}
		CodeExpressionStatement SerializeSupportInitialize(string name, bool beginInit) {
			CodeVariableReferenceExpression variableExpression = new CodeVariableReferenceExpression(name);
			CodeTypeReference typeReference = new CodeTypeReference(typeof(ISupportInitialize));
			CodeCastExpression castExpression = new CodeCastExpression(typeReference, variableExpression);
			CodeMethodReferenceExpression methodExpression = new CodeMethodReferenceExpression(castExpression, beginInit ? "BeginInit" : "EndInit");
			CodeMethodInvokeExpression expression = new CodeMethodInvokeExpression();
			expression.Method = methodExpression;
			CodeExpressionStatement statement = new CodeExpressionStatement(expression);
			statement.UserData["statement-ordering"] = beginInit ? "begin" : "end";
			return statement;
		}
	}
}
