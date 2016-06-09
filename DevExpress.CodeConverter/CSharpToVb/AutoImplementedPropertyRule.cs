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

using DevExpress.CodeParser;
using DevExpress.CodeConverter;
namespace DevExpress.CsToVbConverter {
  [ConvertLanguage("CSharp", "Basic")]
	public class AutoImplementedPropertyRule : ConvertRuleBase<Property> {
	protected override void Convert(Property property) {
	  if (property == null || !property.IsAutoImplemented)
		return;
	  Get getter = property.Getter;
	  Set setter = property.Setter;
	  if (getter.Visibility == setter.Visibility)
		return;
	  TypeDeclaration type = property.Parent as TypeDeclaration;
	  if (type == null)
		return;
	  if (setter.Visibility == MemberVisibility.Private) {
		  string fieldName = FieldNameRule.GetNewFieldName(property.Name);
		Variable field = new Variable(fieldName);
		field.MemberTypeReference = property.MemberTypeReference.Clone() as TypeReferenceExpression;
		field.Visibility = setter.Visibility;
		  type.InsertNode(0, field);
		property.Nodes.Remove(setter);
		property.IsAutoImplemented = false;
		property.Getter.AddNode(new Return(new ElementReferenceExpression(fieldName)));
	  }
	}
  }
}
