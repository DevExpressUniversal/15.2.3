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
using System.Reflection;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.XtraCharts.Design {
	public class ChartItemSerializer : CodeDomSerializer {
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
		void SerializeMembers(IDesignerSerializationManager manager, CodeStatementCollection statements, object value) {
			SerializeProperties(manager, statements, value, null);
			SerializeEvents(manager, statements, value, null);
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			if (manager == null)
				throw new ArgumentNullException("manager");
			if (value == null)
				throw new ArgumentNullException("value");
			bool isComplete = false;
			CodeExpression expression = SerializeCreationExpression(manager, value, out isComplete);
			if (expression == null || isComplete)
				return expression;
			ExpressionContext context = manager.Context[typeof(ExpressionContext)] as ExpressionContext;
			bool isPresetValue = context != null && Object.ReferenceEquals(context.PresetValue, value);
			CodeStatementCollection statements = new CodeStatementCollection();
			if (isPresetValue) {
				SetExpression(manager, value, expression, true);
				SerializeMembers(manager, statements, value);
			}
			else {
				TypeConverter converter = TypeDescriptor.GetConverter(value);
				if (converter == null)
					return null;
				InstanceDescriptor descriptor = (InstanceDescriptor)converter.ConvertTo(value, typeof(InstanceDescriptor));
				if (descriptor == null)
					return null;
				string uniqueName = GetUniqueName(manager, value);
				CodeVariableDeclarationStatement statement = new CodeVariableDeclarationStatement(descriptor.MemberInfo.DeclaringType, uniqueName);
				statement.InitExpression = expression;
				statements.Add(statement);
				SetExpression(manager, value, new CodeVariableReferenceExpression(uniqueName));
				ISupportInitialize supportInitialize = value as ISupportInitialize;
				if (supportInitialize != null)
					statements.Add(SerializeSupportInitialize(uniqueName, true));
				SerializeMembers(manager, statements, value);
				if (supportInitialize != null)
				   statements.Add(SerializeSupportInitialize(uniqueName, false));
			}
			return statements;
		}
	}
	public class ChartCollectionSerializer : CollectionCodeDomSerializer {
		protected override object SerializeCollection(IDesignerSerializationManager manager, CodeExpression targetExpression, Type targetType, ICollection originalCollection, ICollection valuesToSerialize) {
			if (manager == null)
				throw new ArgumentNullException("manager");
			if (targetType == null)
				throw new ArgumentNullException("targetType");
			if (originalCollection == null)
				throw new ArgumentNullException("originalCollection");
			if (valuesToSerialize == null)
				throw new ArgumentNullException("valuesToSerialize");
			if (valuesToSerialize.Count == 0)
				return null;
			ICollection coll = (ICollection)originalCollection;
			ArrayList list = new ArrayList(coll.Count);
			foreach (object obj in coll) {
				CodeExpression expr = SerializeToExpression(manager, obj);
				if (expr != null)
					list.Add(expr);
			}
			if (list.Count == 0)
				return null;
			Type collectionType = TypeDescriptor.GetReflectionType(originalCollection);
			string methodName = collectionType.GetMethod("ClearAndAddRange") == null ? "AddRange" : "ClearAndAddRange";
			MethodInfo methodInfo = collectionType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
			if (methodInfo == null)
				methodInfo = collectionType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
			if (methodInfo == null)
				return null;
			ParameterInfo[] parameters = methodInfo.GetParameters();
			if (parameters.Length != 1 || !parameters[0].ParameterType.IsArray)
				return null;
			CodeArrayCreateExpression createExpression = new CodeArrayCreateExpression();
			createExpression.CreateType = new CodeTypeReference(parameters[0].ParameterType.GetElementType());
			foreach (CodeExpression item in list)
				createExpression.Initializers.Add(item);
			CodeMethodInvokeExpression addRangeExpression = new CodeMethodInvokeExpression();
			addRangeExpression.Method = new CodeMethodReferenceExpression(targetExpression, methodName);
			addRangeExpression.Parameters.Add(createExpression);
			CodeStatementCollection statements = new CodeStatementCollection();
			statements.Add(addRangeExpression);
			return statements;
		}
	}
	public class ChartDictionarySerializer : CodeDomSerializer {
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			IDictionary dictionary = value as IDictionary;
			if (dictionary == null || dictionary.Count == 0) 
				return null;
			ExpressionContext expressionContext = manager.Context.Current as ExpressionContext;
			if (expressionContext == null) 
				return null;
			CodePropertyReferenceExpression expression = expressionContext.Expression as CodePropertyReferenceExpression;
			if (expression == null) {
				Type expressionType = expressionContext.GetType();
				if (expressionType.Name == "CodeValueExpression") {
					expression = expressionType.InvokeMember("Expression", 
						BindingFlags.GetProperty, null, expressionContext, null) as CodePropertyReferenceExpression;
					if (expression == null)
						return null;
				}
			}
			CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(expression, "Add");
			CodeStatementCollection statements = new CodeStatementCollection();
			foreach (DictionaryEntry entry in dictionary) {
				CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression();
				invoke.Method = method;
				invoke.Parameters.Add(SerializeToExpression(manager, entry.Key));
				invoke.Parameters.Add(SerializeToExpression(manager, entry.Value));
				statements.Add(invoke);
			}
			return statements;
		}
		public override object Deserialize(IDesignerSerializationManager manager,object codeObject) {
			return null;
		}
	}
}
