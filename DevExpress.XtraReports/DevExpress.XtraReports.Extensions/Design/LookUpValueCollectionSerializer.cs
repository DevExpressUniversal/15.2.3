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
using System.Linq;
using System.Text;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.Parameters;
using System.CodeDom;
using System.ComponentModel;
namespace DevExpress.XtraReports.Design {
	public class LookUpValueCollectionSerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			return null;
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			LookUpValueCollection LookUpValueCollection = value as LookUpValueCollection;
			if(LookUpValueCollection == null) {
				return null;
			}
			object current = manager.Context.Current;
			ExpressionContext context = current as ExpressionContext;
			if(context != null) {
				current = context.Expression;
			}
			CodePropertyReferenceExpression targetObject = current as CodePropertyReferenceExpression;
			if(targetObject == null) {
				return null;
			}
			object component = base.DeserializeExpression(manager, null, targetObject.TargetObject);
			if((component == null) || (TypeDescriptor.GetProperties(component)[targetObject.PropertyName] == null)) {
				return null;
			}
			CodeStatementCollection statements = new CodeStatementCollection();
			CodeMethodReferenceExpression addMethodReferenceExpression = new CodeMethodReferenceExpression(targetObject, "Add");
			foreach(LookUpValue lookUpValue in LookUpValueCollection) {
				CodeExpression valueExpression = base.SerializeToExpression(manager, lookUpValue.Value);
				CodeExpression descriptionExpression = base.SerializeToExpression(manager, lookUpValue.Description);
				CodeObjectCreateExpression createLookUpValueExpression = new CodeObjectCreateExpression() { CreateType = new CodeTypeReference(typeof(LookUpValue)) };
				createLookUpValueExpression.Parameters.Add(valueExpression);
				createLookUpValueExpression.Parameters.Add(descriptionExpression);
				CodeMethodInvokeExpression invokeAddMethodExpression = new CodeMethodInvokeExpression() { Method = addMethodReferenceExpression };
				invokeAddMethodExpression.Parameters.Add(createLookUpValueExpression);
				statements.Add(invokeAddMethodExpression);
			}
			return statements;
		}
	}
}
