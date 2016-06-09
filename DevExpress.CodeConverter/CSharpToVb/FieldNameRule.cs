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
using System;
namespace DevExpress.CsToVbConverter {
	[ConvertLanguage("CSharp", "Basic")]
	public class FieldNameRule : ConvertRuleBase<Variable> {
		const string fieldPrefix = "_";
		protected override void Convert(Variable var) {
			TypeDeclaration parent = var.Parent as TypeDeclaration;
			if (parent == null || var.Visibility == MemberVisibility.Public || var.Visibility == MemberVisibility.Protected)
				return;
			var.Name = GetNewFieldName(var.Name);
		}
		public static string GetNewFieldName(string name)
		{
			if (string.IsNullOrEmpty(name))
				return name;
			return string.Concat(fieldPrefix, name);
		}
	}
	[ConvertLanguage("CSharp", "Basic")]
	public class FieldReferences : ConvertRule {
		public override void Convert(ConvertArgs args) {
			Expression exp = null;
			exp = args.ElementForConverting as ElementReferenceExpression;
			if (exp == null)
				exp = args.ElementForConverting as MethodReferenceExpression;
			if (!args.Resolver.IsPrivateFieldReference(exp))
				return;
			exp.Name = FieldNameRule.GetNewFieldName(exp.Name);
		}
	}
}
