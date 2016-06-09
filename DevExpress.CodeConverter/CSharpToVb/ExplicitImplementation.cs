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
  public class ExplicitImplementation : ConvertRule {
	const string explicitImplementedMemberSuffix = "_Impl";
	public override void Convert(ConvertArgs args) {
	  Member member = args.ElementForConverting as Member;
	  if (member == null || member.ImplementsExpressions == null || member.ImplementsExpressions.Count == 0)
		return;
	  string name = member.Name;
	  if (string.IsNullOrEmpty(name))
		return;
	  string[] nameParts = name.Split('.');
	  if (nameParts != null && nameParts.Length != 0)
	  {
		  string newName = CorrectNamesRule.CorrectName(nameParts[nameParts.Length - 1]);
		  if (args.Resolver.HasMemberInParent(newName, member))
			  member.Name = newName + explicitImplementedMemberSuffix;
		  else
			member.Name = newName;
	  }
	}
	string GetNameForImplementedMember(string name) {
	  char[] charsForReplacing = { ',', '.', '<', '>', ' ' };
	  foreach (char ch in charsForReplacing)
		name = name.Replace(ch, '_');
	  return name;
	}
	bool HasSameMember(Member member) {
	  TypeDeclaration typeDecl = member.Parent as TypeDeclaration;
	  if (typeDecl == null || typeDecl.NodeCount == 0)
		return false;
	  foreach (LanguageElement element in typeDecl.Nodes)
		if (element != member && element.ElementType == member.ElementType && string.Equals(element.Name, member.Name, System.StringComparison.OrdinalIgnoreCase))
		  return true;
	  return false;
	}
  }
}
