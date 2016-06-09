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
using System.Data;
namespace DevExpress.XtraReports.Native {
	public class DataSetSerializer : CodeDomSerializer {
		#region static
		static CodeStatement FindRelationAddingStatement(CodeStatementCollection codeStatments) {
			foreach(CodeStatement codeStatement in codeStatments) {
				CodeExpressionStatement expressionStatement = codeStatement as CodeExpressionStatement;
				if(expressionStatement == null)
					continue;
				CodeMethodInvokeExpression invokeExpression = expressionStatement.Expression as CodeMethodInvokeExpression;
				if(invokeExpression == null)
					continue;
				CodeMethodReferenceExpression method = invokeExpression.Method;
				if(method.MethodName != "AddRange")
					continue;
				CodePropertyReferenceExpression property = method.TargetObject as CodePropertyReferenceExpression;
				if(property.PropertyName != "Relations")
					continue;
				if(invokeExpression.Parameters.Count > 0) {
					System.CodeDom.CodeArrayCreateExpression array = invokeExpression.Parameters[0] as System.CodeDom.CodeArrayCreateExpression;
					if(array != null && array.CreateType.BaseType.Equals("System.Data.DataRelation"))
						return codeStatement;
				}
			}
			return null;
		}
		#endregion
		CodeDomSerializer serializer;
		public DataSetSerializer() {
			Type type = typeof(CodeDomSerializer).Assembly.GetType("System.ComponentModel.Design.Serialization.ComponentCodeDomSerializer");
			serializer = Activator.CreateInstance(type) as CodeDomSerializer;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			CodeStatementCollection codeStatments = serializer.Serialize(manager, value) as CodeStatementCollection;
			CodeStatement relationAddingStatement = FindRelationAddingStatement(codeStatments);
			if(relationAddingStatement != null) {
				codeStatments.Remove(relationAddingStatement);
			}
			return codeStatments;
		}
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			return serializer.Deserialize(manager, codeObject);
		}
	}
	public class DataSetSerializationProvider : IDesignerSerializationProvider {
		object serializer = null;
		public DataSetSerializationProvider() {
			serializer = new DataSetSerializer();
		}
		public object GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType) {
			return objectType != null && objectType.Equals(typeof(DataSet)) ? serializer : null;
		}
	}
}
