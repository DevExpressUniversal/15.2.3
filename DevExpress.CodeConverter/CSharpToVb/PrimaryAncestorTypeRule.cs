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
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.CsToVbConverter {
	[ConvertLanguage("CSharp", "Basic")]
	public class PrimaryAncestorTypeRule :ConvertRule {
		public override void Convert(ConvertArgs args) {
			TypeReferenceExpression typeRef = args.ElementForConverting as TypeReferenceExpression;
			if (typeRef == null || typeRef.Parent == null)
				return;
			TypeDeclaration type = typeRef.Parent as TypeDeclaration;
			if (type == null || !type.IsOneOfElementType(LanguageElementType.Class, LanguageElementType.Struct))
				return;			
			if (type.PrimaryAncestorType != typeRef || !args.Resolver.IsInterface(typeRef))
				return;
			type.AddSecondaryAncestorType(typeRef);
			type.PrimaryAncestorType = null;
		}
	}
	[ConvertLanguage("CSharp", "Basic")]
	public class AliasReferenceRule : ConvertRule {
		const string aliasSuffix = "Class";
		public static string GetAliasName(string name) {
			if (string.IsNullOrEmpty(name))
				return name;
			return string.Concat(name, aliasSuffix);
		}
		public override void Convert(ConvertArgs args) {
			TypeReferenceExpression typeRef = args.ElementForConverting as TypeReferenceExpression;
			if (typeRef == null || typeRef.Parent == null)
				return;
			TypeDeclaration type = typeRef.Parent as TypeDeclaration;
			if (!args.Resolver.HasAlias(typeRef))
				return;
			typeRef.Name = GetAliasName(typeRef.Name);
		}
	}
	[ConvertLanguage("CSharp", "Basic")]
	public class AliasRule : ConvertRuleBase<NamespaceReference> {
		protected override void Convert(NamespaceReference namespaceRef)
		{
			if(!namespaceRef.IsAlias)
				return;
			Namespace ns = namespaceRef.Parent as Namespace;
			if(ns == null || ns.Parent == null)
				return;
			LanguageElement parent = ns.Parent;
			int index = parent.Nodes.IndexOf(ns);
			ns.RemoveNode(namespaceRef);
			parent.InsertNode(index, namespaceRef);
			Expression aliasExp = namespaceRef.AliasExpression;
			if(aliasExp != null)
				aliasExp.Name = AliasReferenceRule.GetAliasName(aliasExp.Name);
			namespaceRef.AliasName = AliasReferenceRule.GetAliasName(namespaceRef.AliasName);
		}
	}
}
