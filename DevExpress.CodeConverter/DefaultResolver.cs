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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.CodeConverter {
	public class DefaultConvertResolver : ConvertResolver {
		Lazy<FileContainer> typeContainer;
		Lazy<FileContainer> convertedTypeContainer;
		public DefaultConvertResolver(bool languageIsCaseSensitivity) {
			LanguageIsCaseSensetivity = languageIsCaseSensitivity;
			typeContainer = new Lazy<FileContainer>(() => new FileContainer(languageIsCaseSensitivity));
			convertedTypeContainer = new Lazy<FileContainer>(() => new FileContainer(languageIsCaseSensitivity));
		}
		public override bool IsInterface(LanguageElement element) {
			if(element == null || string.IsNullOrEmpty(element.Name))
				return false;
			if(char.ToLower(element.Name[0]) != 'i')
				return false;
			return element.Name.Length == 1 || char.IsUpper(element.Name[1]);
		}
		public override bool IsClassOrStruct(LanguageElement element) {
			return !IsInterface(element);
		}
		public override bool IsPrivateFieldReference(LanguageElement element) {
			if(element == null)
				return false;
			ReferenceExpressionBase refExp = element as ReferenceExpressionBase;
			if(refExp != null) {
				ThisReferenceExpression thisRefExp = refExp.Qualifier as ThisReferenceExpression;
				if(thisRefExp == null && GetLocalNames(element).Where(n => n == FileContainer.CorrectName(element.Name, LanguageIsCaseSensetivity)).Any()) return false;
			}
			FileContainer container = typeContainer.Value;
			return container.HasNonPublicNonProtectedField(element as Expression);
		}
		IEnumerable<string> GetLocalNames(LanguageElement element) {
			List<string> names = new List<string>();
			LambdaExpression parentBlock = element.GetParent(LanguageElementType.LambdaExpression) as LambdaExpression;
			if(parentBlock != null && parentBlock.Nodes != null)
				names.AddRange(parentBlock.Nodes.OfType<Variable>().Select(v => v.Name).ToList());
			Method method = element.GetParentMethod();
			if(method == null) return names;
			names.AddRange(method.Nodes.OfType<ImplicitVariable>().Select(x => x.Name).ToArray());
			if(method.Parameters != null)
				names.AddRange(method.Parameters.OfType<Param>().Select(v => v.Name).ToList());
			return names;
		}
		public override bool IsVoidMethod(LanguageElement element) {
			return typeContainer.Value.IsVoidMethod(element as MethodCallExpression);
		}
		public bool LanguageIsCaseSensetivity { get; set; }
		public override void Collect(LanguageElement element) {
			CollectElement(element);
			foreach(LanguageElement currentElement in element.DetailNodes)
				Collect(currentElement);
			foreach(LanguageElement currentElement in element.Nodes)
				Collect(currentElement);
		}
		public void CollectElement(LanguageElement element) {
			TypeDeclaration type = element as TypeDeclaration;
			if(type == null ||
				(type.ElementType != LanguageElementType.Class && type.ElementType != LanguageElementType.Struct))
				return;
			FileContainer container = typeContainer.Value;
			container.Add(element);
			if(type.Nodes == null)
				return;
			foreach(LanguageElement node in type.Nodes)
				container.Add(type, node);
		}
		public override void CollectConverted(LanguageElement newElement) {
			TypeDeclaration type = newElement as TypeDeclaration;
			if(type == null || (type.ElementType != LanguageElementType.Class && type.ElementType != LanguageElementType.Struct)) {
				type = newElement.GetParentClass() as TypeDeclaration;
				if(type == null)
					return;
				convertedTypeContainer.Value.Add(type, newElement);
			} else
				convertedTypeContainer.Value.Add(type);
		}
		public override bool IsReadOnlyProperty(LanguageElement element) {
			return typeContainer.Value.HasReadOnlyProperty(element as Expression);
		}
		public override bool NeedReplaceAutoImplementedPropertyReference(LanguageElement element) {
			return typeContainer.Value.NeedReplaceAutoImplementedPropertyReference(element as Expression);
		}
		public override bool IsProperty(LanguageElement element) {
			return typeContainer.Value.HasProperty(element as Expression);
		}
		public override bool HasMemberInParent(string name, LanguageElement child) {
			return typeContainer.Value.HasMemberInParent(name, child) || convertedTypeContainer.Value.HasMemberInParent(name, child);
		}
		public override bool HasAlias(TypeReferenceExpression expression) {
			return typeContainer.Value.HasAlias(expression);
		}
	}
}
