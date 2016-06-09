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
using System.Collections;
#if SL
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using Diagnostics;
  #if DXCORE
  using DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting;
  #else
  using DevExpress.CodeParser.CodeStyle.Formatting;
  #endif
  public class EmptyStructuralParserServices : IStructuralParserServices
  {
	public EmptyStructuralParserServices()
	{
	}
	public ILogger GetLogger()
	{
	  return null;
	}
	public string GetXMLTagName()
	{
	  return "XML Doc Tag";
	}
	public ArrayList FindAllReferencesForBaseVariable(BaseVariable variable)
	{
	  throw new NotImplementedException();
	}
	public bool IsKeyword(string name)
	{
	  throw new NotImplementedException();
	}
	public bool IsKeyword(IReferenceExpression reference)
	{
	  throw new NotImplementedException();
	}
		public bool IsKeywordElement(IElement element)
		{
			throw new NotImplementedException();
		}
	public Expression BuildQueryTranslation(QueryExpression queryExpression)
	{
	  throw new NotImplementedException();
	}
	public string GetEscapedString(string projectLanguage, string name, bool isVerbatim)
	{
	  throw new NotImplementedException();
	}
	public Expression Invert(Expression expression)
	{
	  throw new NotImplementedException();
	}
	public LanguageElementCollection GetUnusedDeclarations(IEnumerable allVariables)
	{
	  throw new NotImplementedException();
	}
	public IElement FindElementByFullName(IElementCollection iElementCollection, string fullName, bool caseSensitive)
	{
	  throw new NotImplementedException();
	}
	public ITypeElement[] GetAllDescendants(ITypeElement typeElement)
	{
	  throw new NotImplementedException();
	}
	public ITypeElement[] GetAllDescendants(ITypeElement typeElement, IElement scope)
	{
	  throw new NotImplementedException();
	}
	public ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver, ITypeElement typeElement)
	{
	  throw new NotImplementedException();
	}
	public ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver, ITypeElement typeElement, IElement scope)
	{
	  throw new NotImplementedException();
	}
	public ITypeElement[] GetDescendants(ITypeElement typeElement)
	{
	  throw new NotImplementedException();
	}
	public ITypeElement[] GetDescendants(ITypeElement typeElement, IElement scope)
	{
	  throw new NotImplementedException();
	}
	public ITypeElement[] GetDescendants(ISourceTreeResolver resolver, ITypeElement typeElement)
	{
	  throw new NotImplementedException();
	}
	public ITypeElement[] GetDescendants(ISourceTreeResolver resolver, ITypeElement typeElement, IElement scope)
	{
	  throw new NotImplementedException();
	}
	public string GetSignaturePart(IElement element)
	{
	  throw new NotImplementedException();
	}
	public string ReplaceAccessOperators(string fullName)
	{
	  throw new NotImplementedException();
	}
	public TypeReferenceExpression GetMethodType(AnonymousMethodExpression anonymousMethod)
	{
	  throw new NotImplementedException();
	}
	public string GetOverrideCode(IMemberElement member, bool callBase, string codeBefore, string codeAfter)
	{
	  throw new NotImplementedException();
	}
	public string GenerateElement(LanguageElement element)
	{
	  throw new NotImplementedException();
	}
	public string GenerateElement(LanguageElement element, int precedingWhiteSpaceCount)
	{
	  throw new NotImplementedException();
	}
	public string GenerateElement(LanguageElement element, string languageID, int precedingWhiteSpaceCount)
	{
	  throw new NotImplementedException();
	}
	public bool IsBuiltInType(string typeName)
	{
	  throw new NotImplementedException();
	}
	public string GetSimpleTypeName(string typeName)
	{
	  throw new NotImplementedException();
	}
	public string GetComment(string text, string languageID)
	{
	  throw new NotImplementedException();
	}
	public bool IdentifiersMatch(string first, string second)
	{
	  throw new NotImplementedException();
	}
	public string ExtractFirstIdentifier(ref string remainingIdentifiers)
	{
	  throw new NotImplementedException();
	}
	public bool IsCollapsible(LanguageElement element)
	{
	  return false;
	}
	public SourceRange GetCollapsibleRange(LanguageElement element)
	{
	  if (element == null)
		return SourceRange.Empty;
	  return element.Range;
	}
	public bool DescendsFrom(ITypeElement type, string fullTypeName)
	{
	  throw new NotImplementedException();
	}
	public bool DescendsFrom(ISourceTreeResolver resolver, ITypeElement type, string fullTypeName)
	{
	  throw new NotImplementedException();
	}
	public Expression Simplify(Expression expression)
	{
	  throw new NotImplementedException();
	}
	public Expression Simplify(Expression expression, bool considerMethodCalls)
	{
	  throw new NotImplementedException();
	}
	public IElement FindElementInSnapshotStructure(IElement declaration)
	{
	  throw new NotImplementedException();
	}
	public bool InMacroCall(LanguageElement element)
	{
	  return false;
	}
	public IElement GetDeclaration(IElement element)
	{
	  throw new NotImplementedException();
	}
	public IElement GetDeclaration(IElement element, bool restore)
	{
	  throw new NotImplementedException();
	}
	public IElementCollection FindAllReferences(IElement element)
	{
	  throw new NotImplementedException();
	}
	public IElementCollection FindAllReferences(IElement scope, IElement element)
	{
	  throw new NotImplementedException();
	}
	public IAssemblyModel GetAssemblyModel(LanguageElement element)
	{
	  throw new NotImplementedException();
	}
	public bool IsEventHandler(IMethodElement element)
	{
	  throw new NotImplementedException();
	}
	public bool IsMainProcedure(Method method)
	{
	  throw new NotImplementedException();
	}
	public bool IsSerializationConstructor(IMethodElement method)
	{
	  throw new NotImplementedException();
	}
	public bool IsInitializeComponent(IMethodElement method)
	{
	  throw new NotImplementedException();
	}
	public bool IsInteriorPtrPointer(TypeReferenceExpression type)
	{
	  throw new NotImplementedException();
	}
	public IElementFilter GetMemberSignatureFilter(Method method)
	{
	  throw new NotImplementedException();
	}
	public ITypeElement[] GetAllBaseTypes(ITypeElement element)
	{
	  throw new NotImplementedException();
	}
	public bool IsWebMethod(Method method)
	{
	  throw new NotImplementedException();
	}
	public bool CheckExtensionMethod(IMethodElement method)
	{
	  throw new NotImplementedException();
	}
	public bool CheckExtensionMethod(ISourceTreeResolver resolver, IMethodElement method)
	{
	  throw new NotImplementedException();
	}
	public bool HasDllImportAttribute(IMethodElement method)
	{
	  throw new NotImplementedException();
	}
	public bool DeclarationsMatch(ISourceTreeResolver resolver, IElement first, IElement second)
	{
	  throw new NotImplementedException();
	}
	public bool IsIdenticalTo(IElement first, IElement second)
	{
	  throw new NotImplementedException();
	}
	public ITypeReferenceExpression GetInnerType(TypeReferenceExpression typeReferenceExpression)
	{
	  throw new NotImplementedException();
	}
	public bool UsesTypeParameters(TypeReferenceExpression typeReferenceExpression, IGenericElement generic)
	{
	  throw new NotImplementedException();
	}
	public bool IsTypeParameter(TypeReferenceExpression typeReferenceExpression, IGenericElement generic)
	{
	  throw new NotImplementedException();
	}
	public IMemberElement FindMember(ITypeElement type, string name, IElementFilter filter, bool searchInAncestors)
	{
	  throw new NotImplementedException();
	}
	public IMemberElementCollection FindMembers(ITypeElement type, string name, IElementFilter filter, bool searchInAncestors)
	{
	  throw new NotImplementedException();
	}
	public IMemberElementCollection FindMembers(ISourceTreeResolver resolver, ITypeElement type, string name, IElementFilter filter, bool searchInAncestors)
	{
	  throw new NotImplementedException();
	}
	public SourceFileBuildAction GetBuildAction(SourceFile sourceFile)
	{
	  throw new NotImplementedException();
	}
	public ParserBase GetParserFromLanguageID(string language)
	{
	  return ParserFactory.CreateParser(language);
	}
	public IBraceSettings LoadBraceSettings()
	{
	  return new DefaultBraceSettings();
	}
	public ITabSettings GetTabSettings(ParserLanguageID language)
	{
	  return new DefaultTabSettings();
	}
	public ISourceTreeResolver SourceTreeResolver 
	{
	  get
	  {
		throw new NotImplementedException();
	  }
	}
	public IFormattingService FormattingService
	{
	  get
	  {
		return StructuralParserServicesHolder.FormattingService;
	  }
	}
	public ISolutionElement ActiveSolution
	{
	  get
	  {
		return null;
	  }
	}
  }
}
