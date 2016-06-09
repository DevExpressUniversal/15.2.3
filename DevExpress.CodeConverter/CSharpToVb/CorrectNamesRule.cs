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
using DevExpress.CodeParser.VB;
using DevExpress.CodeConverter;
namespace DevExpress.CsToVbConverter {
	public static class LanguageElementExtesion {
		internal static bool IsOneOfElementType(this LanguageElement element, params LanguageElementType[] elementTypes) {
			if(elementTypes == null)
				return false;
			foreach(LanguageElementType type in elementTypes)
				if(type == element.ElementType)
					return true;
			return false;
		}
	}
	[ConvertLanguage("CSharp", "Basic")]
	public class CorrectNamesRule : ConvertRule {
		public override void Convert(ConvertArgs args) {
			LanguageElement element = args.ElementForConverting;
			if(element == null || element.Name == null) return;
			if(element.Name == "object") {
				element.Name = "Object";
				return;
			}
			if(!(element.IsOneOfElementType(LanguageElementType.ElementReferenceExpression) || element is Member || element is MethodReferenceExpression)) return;
			element.Name = CorrectName(element.Name);
		}
		public static string CorrectName(string name) {
			if(string.IsNullOrEmpty(name) || !VB90Tokens.Instance.IsKeyword(name))
				return name;
			return string.Format("{0}{1}{2}", "[", name, "]");
		}
	}
}
