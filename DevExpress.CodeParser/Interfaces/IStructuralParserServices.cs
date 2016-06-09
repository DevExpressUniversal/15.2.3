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
  public interface IStructuralParserServices
  {
	ArrayList FindAllReferencesForBaseVariable(BaseVariable variable);
	bool IsKeyword(string name);
	bool IsKeyword(IReferenceExpression reference);
		bool IsKeywordElement(IElement element);
	Expression BuildQueryTranslation(QueryExpression queryExpression);
	string GetEscapedString(string projectLanguage, string name, bool isVerbatim);
	ISourceTreeResolver SourceTreeResolver { get; }
	Expression Invert(Expression expression);
	Expression Simplify(Expression expression);
	Expression Simplify(Expression expression, bool considerMethodCalls);
	LanguageElementCollection GetUnusedDeclarations(IEnumerable allVariables);
	IElement FindElementByFullName(IElementCollection iElementCollection, string fullName, bool caseSensitive);
	ITypeElement[] GetAllDescendants(ITypeElement delegateDefinition);
	ITypeElement[] GetAllDescendants(ITypeElement delegateDefinition, IElement scope);
	ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver, ITypeElement delegateDefinition);
	ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver, ITypeElement delegateDefinition, IElement scope);
	ITypeElement[] GetDescendants(ITypeElement delegateDefinition);
	ITypeElement[] GetDescendants(ITypeElement delegateDefinition, IElement scope);
	ITypeElement[] GetDescendants(ISourceTreeResolver resolver, ITypeElement delegateDefinition);
	ITypeElement[] GetDescendants(ISourceTreeResolver resolver, ITypeElement delegateDefinition, IElement scope);
	string GetSignaturePart(IElement element);
	string ReplaceAccessOperators(string fullName);
	string GetOverrideCode(IMemberElement member, bool callBase, string codeBefore, string codeAfter);
	TypeReferenceExpression GetMethodType(AnonymousMethodExpression anonymousMethod);
	string GenerateElement(LanguageElement element);
	string GenerateElement(LanguageElement element, int precedingWhiteSpaceCount);
	string GenerateElement(LanguageElement element, string languageID, int precedingWhiteSpaceCount);
	bool IsBuiltInType(string typeName);
	string GetSimpleTypeName(string typeName);
	string GetComment(string text, string languageID);
	bool IdentifiersMatch(string first, string second);
	string ExtractFirstIdentifier(ref string remainingIdentifiers);
	bool IsCollapsible(LanguageElement element);
	SourceRange GetCollapsibleRange(LanguageElement element);
	bool DescendsFrom(ITypeElement type, string fullTypeName);
	bool DescendsFrom(ISourceTreeResolver resolver, ITypeElement type, string fullTypeName);
	IElement FindElementInSnapshotStructure(IElement declaration);
	bool InMacroCall(LanguageElement element);
	IElement GetDeclaration(IElement element);
	IElement GetDeclaration(IElement element, bool restore);
	IElementCollection FindAllReferences(IElement element);
	IElementCollection FindAllReferences(IElement scope, IElement element);
	IAssemblyModel GetAssemblyModel(LanguageElement element);
	bool IsEventHandler(IMethodElement element);
	bool IsMainProcedure(Method method);
	bool IsSerializationConstructor(IMethodElement method);
	bool IsInitializeComponent(IMethodElement method);
	bool IsInteriorPtrPointer(TypeReferenceExpression type);
	IElementFilter GetMemberSignatureFilter(Method method);
	ITypeElement[] GetAllBaseTypes(ITypeElement element);
	bool IsWebMethod(Method method);
	bool CheckExtensionMethod(IMethodElement method);
	bool CheckExtensionMethod(ISourceTreeResolver resolver, IMethodElement method);
	bool HasDllImportAttribute(IMethodElement method);
	bool DeclarationsMatch(ISourceTreeResolver resolver, IElement first, IElement second);
	bool IsIdenticalTo(IElement first, IElement second);
	ITypeReferenceExpression GetInnerType(TypeReferenceExpression typeReferenceExpression);
	bool UsesTypeParameters(TypeReferenceExpression typeReferenceExpression, IGenericElement generic);
	bool IsTypeParameter(TypeReferenceExpression typeReferenceExpression, IGenericElement generic);
	IMemberElement FindMember(ITypeElement type, string name, IElementFilter filter, bool searchInAncestors);
	IMemberElementCollection FindMembers(ISourceTreeResolver resolver, ITypeElement type, string name, IElementFilter filter, bool searchInAncestors);
	IMemberElementCollection FindMembers(ITypeElement type, string name, IElementFilter filter, bool searchInAncestors);
	SourceFileBuildAction GetBuildAction(SourceFile sourceFile);
	ParserBase GetParserFromLanguageID(string language);
	IBraceSettings LoadBraceSettings();
	ITabSettings GetTabSettings(ParserLanguageID language);
	IFormattingService FormattingService { get; }
		ILogger GetLogger();
	string GetXMLTagName();
	ISolutionElement ActiveSolution { get; }
	}
  public static class StructuralParserServicesHolder
  {
	static IFormattingService _FormattingService;
	public static void SetFormattingService(IFormattingService fs)
	{
	  _FormattingService = fs;
	}
	public static IStructuralParserServices StructuralParserServices = new EmptyStructuralParserServices();
	public static ArrayList FindAllReferencesForBaseVariable(BaseVariable variable)
	{
	  return StructuralParserServices.FindAllReferencesForBaseVariable(variable);
	}
	public static bool IsKeyword(string name)
	{
	  return StructuralParserServices.IsKeyword(name);
	}
	public static bool IsKeyword(IReferenceExpression reference)
	{
	  return StructuralParserServices.IsKeyword(reference);
	}
		public static bool IsKeywordElement(IElement element)
		{
			return StructuralParserServices.IsKeywordElement(element);
		}
	public static Expression BuildQueryTranslation(QueryExpression queryExpression)
	{
	  return StructuralParserServices.BuildQueryTranslation(queryExpression);
	}
	public static string GetEscapedString(string projectLanguage, string name, bool isVerbatim)
	{
	  return StructuralParserServices.GetEscapedString(projectLanguage, name, isVerbatim);
	}
	public static Expression Invert(Expression expression)
	{
	  return StructuralParserServices.Invert(expression);
	}
	public static Expression Simplify(Expression expression)
	{
	  return StructuralParserServices.Simplify(expression);
	}
	public static Expression Simplify(Expression expression, bool considerMethodCalls)
	{
	  return StructuralParserServices.Simplify(expression, considerMethodCalls);
	}
	public static LanguageElementCollection GetUnusedDeclarations(IEnumerable allVariables)
	{
	  return StructuralParserServices.GetUnusedDeclarations(allVariables);
	}
	public static IElement FindElementByFullName(IElementCollection iElementCollection, string fullName, bool caseSensitive)
	{
	  return StructuralParserServices.FindElementByFullName(iElementCollection, fullName, caseSensitive);
	}
	public static ITypeElement[] GetAllDescendants(ITypeElement delegateDefinition)
	{
	  return StructuralParserServices.GetAllDescendants(delegateDefinition);
	}
	public static ITypeElement[] GetAllDescendants(ITypeElement delegateDefinition, IElement scope)
	{
	  return StructuralParserServices.GetAllDescendants(delegateDefinition, scope);
	}
	public static ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver, ITypeElement delegateDefinition)
	{
	  return StructuralParserServices.GetAllDescendants(resolver, delegateDefinition);
	}
	public static ITypeElement[] GetAllDescendants(ISourceTreeResolver resolver, ITypeElement delegateDefinition, IElement scope)
	{
	  return StructuralParserServices.GetAllDescendants(resolver, delegateDefinition, scope);
	}
	public static ITypeElement[] GetDescendants(ITypeElement delegateDefinition)
	{
	  return StructuralParserServices.GetDescendants(delegateDefinition);
	}
	public static ITypeElement[] GetDescendants(ITypeElement delegateDefinition, IElement scope)
	{
	  return StructuralParserServices.GetDescendants(delegateDefinition, scope);
	}
	public static ITypeElement[] GetDescendants(ISourceTreeResolver resolver, ITypeElement delegateDefinition)
	{
	  return StructuralParserServices.GetDescendants(resolver, delegateDefinition);
	}
	public static ITypeElement[] GetDescendants(ISourceTreeResolver resolver, ITypeElement delegateDefinition, IElement scope)
	{
	  return StructuralParserServices.GetDescendants(resolver, delegateDefinition, scope);
	}
	public static string GetSignaturePart(IElement element)
	{
	  return StructuralParserServices.GetSignaturePart(element);
	}
	public static string ReplaceAccessOperators(string fullName)
	{
	  return StructuralParserServices.ReplaceAccessOperators(fullName);
	}
	public static string GetOverrideCode(IMemberElement member, bool callBase, string codeBefore, string codeAfter)
	{
	  return StructuralParserServices.GetOverrideCode(member, callBase, codeBefore, codeAfter);
	}
	public static TypeReferenceExpression GetMethodType(AnonymousMethodExpression anonymousMethod)
	{
	  return StructuralParserServices.GetMethodType(anonymousMethod);
	}
	public static string GenerateElement(LanguageElement element)
	{
	  return StructuralParserServices.GenerateElement(element);
	}
	public static string GenerateElement(LanguageElement element, int precedingWhiteSpaceCount)
	{
	  return StructuralParserServices.GenerateElement(element, precedingWhiteSpaceCount);
	}
	public static string GenerateElement(LanguageElement element, string languageID, int precedingWhiteSpaceCount)
	{
	  return StructuralParserServices.GenerateElement(element, languageID, precedingWhiteSpaceCount);
	}
	public static bool IsBuiltInType(string typeName)
	{
	  return StructuralParserServices.IsBuiltInType(typeName);
	}
	public static string GetSimpleTypeName(string typeName)
	{
	  return StructuralParserServices.GetSimpleTypeName(typeName);
	}
	public static string GetComment(string text, string languageID)
	{
	  return StructuralParserServices.GetComment(text, languageID);
	}
	public static bool IdentifiersMatch(string first, string second)
	{
	  return StructuralParserServices.IdentifiersMatch(first, second);
	}
	public static string ExtractFirstIdentifier(ref string remainingIdentifiers)
	{
	  return StructuralParserServices.ExtractFirstIdentifier(ref remainingIdentifiers);
	}
	public static bool IsCollapsible(LanguageElement element)
	{
	  return StructuralParserServices.IsCollapsible(element);
	}
	public static SourceRange GetCollapsibleRange(LanguageElement element)
	{
	  return StructuralParserServices.GetCollapsibleRange(element);
	}
	public static bool DescendsFrom(ITypeElement type, string fullTypeName)
	{
	  return StructuralParserServices.DescendsFrom(type, fullTypeName);
	}
	public static bool DescendsFrom(ISourceTreeResolver resolver, ITypeElement type, string fullTypeName)
	{
	  return StructuralParserServices.DescendsFrom(resolver, type, fullTypeName);
	}
	public static IElement FindElementInSnapshotStructure(IElement declaration)
	{
	  return StructuralParserServices.FindElementInSnapshotStructure(declaration);
	}
	public static bool InMacroCall(LanguageElement element)
	{
	  return StructuralParserServices.InMacroCall(element);
	}
	public static IElement GetDeclaration(IElement element)
	{
	  return StructuralParserServices.GetDeclaration(element);
	}
	public static IElement GetDeclaration(IElement element, bool restore)
	{
	  return StructuralParserServices.GetDeclaration(element, restore);
	}
	public static IElementCollection FindAllReferences(IElement element)
	{
	  return StructuralParserServices.FindAllReferences(element);
	}
	public static IElementCollection FindAllReferences(IElement scope, IElement element)
	{
	  return StructuralParserServices.FindAllReferences(scope, element);
	}
	public static IAssemblyModel GetAssemblyModel(LanguageElement element)
	{
	  return StructuralParserServices.GetAssemblyModel(element);
	}
	public static bool IsEventHandler(IMethodElement element)
	{
	  return StructuralParserServices.IsEventHandler(element);
	}
	public static bool IsMainProcedure(Method method)
	{
	  return StructuralParserServices.IsMainProcedure(method);
	}
	public static bool IsSerializationConstructor(Method method)
	{
	  return StructuralParserServices.IsSerializationConstructor(method);
	}
	public static bool IsInitializeComponent(IMethodElement method)
	{
	  return StructuralParserServices.IsInitializeComponent(method);
	}
	public static bool IsInteriorPtrPointer(TypeReferenceExpression type)
	{
	  return StructuralParserServices.IsInteriorPtrPointer(type);
	}
	public static IElementFilter GetMemberSignatureFilter(Method method)
	{
	  return StructuralParserServices.GetMemberSignatureFilter(method);
	}
	public static ITypeElement[] GetAllBaseTypes(ITypeElement element)
	{
	  return StructuralParserServices.GetAllBaseTypes(element);
	}
	public static bool IsWebMethod(Method method)
	{
	  return StructuralParserServices.IsWebMethod(method);
	}
	public static bool CheckExtensionMethod(IMethodElement method)
	{
	  return StructuralParserServices.CheckExtensionMethod(method);
	}
	public static bool CheckExtensionMethod(ISourceTreeResolver resolver, IMethodElement method)
	{
	  return StructuralParserServices.CheckExtensionMethod(resolver, method);
	}
	public static bool HasDllImportAttribute(IMethodElement method)
	{
	  return StructuralParserServices.HasDllImportAttribute(method);
	}
	public static bool DeclarationsMatch(ISourceTreeResolver resolver, IElement first, IElement second)
	{
	  return StructuralParserServices.DeclarationsMatch(resolver, first, second);
	}
	public static bool IsIdenticalTo(IElement first, IElement second)
	{
	  return StructuralParserServices.IsIdenticalTo(first, second);
	}
	public static ITypeReferenceExpression GetInnerType(TypeReferenceExpression typeReferenceExpression)
	{
	  return StructuralParserServices.GetInnerType(typeReferenceExpression);
	}
	public static bool UsesTypeParameters(TypeReferenceExpression typeReferenceExpression, IGenericElement generic)
	{
	  return StructuralParserServices.UsesTypeParameters(typeReferenceExpression, generic);
	}
	public static bool IsTypeParameter(TypeReferenceExpression typeReferenceExpression, IGenericElement generic)
	{
	  return StructuralParserServices.IsTypeParameter(typeReferenceExpression, generic);
	}
	public static IMemberElement FindMember(ITypeElement type, string name, IElementFilter filter, bool searchInAncestors)
	{
	  return StructuralParserServices.FindMember(type, name, filter, searchInAncestors);
	}
	public static IMemberElementCollection FindMembers(ISourceTreeResolver resolver, ITypeElement type, string name, IElementFilter filter, bool searchInAncestors)
	{
	  return StructuralParserServices.FindMembers(resolver, type, name, filter, searchInAncestors);
	}
	public static IMemberElementCollection FindMembers(ITypeElement type, string name, IElementFilter filter, bool searchInAncestors)
	{
	  return StructuralParserServices.FindMembers(type, name, filter, searchInAncestors);
	}
	public static SourceFileBuildAction GetBuildAction(SourceFile sourceFile)
	{
	  return StructuralParserServices.GetBuildAction(sourceFile);
	}
	public static ParserBase GetParserFromLanguageID(string language)
	{
	  return StructuralParserServices.GetParserFromLanguageID(language);
	}
	public static IBraceSettings LoadBraceSettings()
	{
	  return StructuralParserServices.LoadBraceSettings();
	}
	public static ITabSettings GetTabSettings(ParserLanguageID language)
	{
	  return StructuralParserServices.GetTabSettings(language);
	}
	public static ILogger GetLogger()
	{
	  return StructuralParserServices.GetLogger();
	}
	public static string GetXMLTagName()
	{
	  return StructuralParserServices.GetXMLTagName();
	}
	public static ISourceTreeResolver SourceTreeResolver
	{
	  get { return StructuralParserServices.SourceTreeResolver; }
	}
	public static IFormattingService FormattingService
	{
	  get { return _FormattingService; }
	}
	public static ISolutionElement ActiveSolution
	{
	  get { return StructuralParserServices.ActiveSolution; }
	}
  }
}
