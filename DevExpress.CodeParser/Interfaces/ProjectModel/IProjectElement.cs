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
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{	
	public interface IProjectElement
	{
		FilteredSourceFile[] FilterDiskFiles(string name);
		FilteredSourceFile[] FilterDiskFiles(StringCollection names);
	[EditorBrowsable(EditorBrowsableState.Never)]
	FilteredSourceFile[] FilterDiskFile(StringCollection names, SourceFile proxy);
		ISourceFileCollection GetAllFiles();
		void ReleaseAllDiskFiles();
		IElement FindByFullNameInsideAssemblies(string fullName, bool cache, bool isCaseSensitive);
	IElementCollection FindElementsByFullNameInsideAssemblies(string fullName, bool cache, bool isCaseSensitive);
		IElement FindElementInsideDefaultNamespace(string name);
		IElement FindElementInsideDefaultNamespace(string name, bool isCaseSensitive);
		IElement FindElementInsideNamespace(string namespaceFullName, string name);
		IElement FindElementInsideNamespace(string namespaceFullName, string name, bool isCaseSensitive);
		IElement FindElementInsideParentNamespace(string namespaceFullName, string name);
		IElement FindElementInsideParentNamespace(string namespaceFullName, string name, bool isCaseSensitive);
		IElement FindElementInsideParentNamespace(IElement start, string name);
		IElement FindElementByFullName(string fullName);
		IElement FindElementByFullName(string fullName, bool isCaseSensitive);
		IElement FindElementByFullName(string fullName, bool searchInAssemblies, bool cache);
		IElement FindElementByFullName(string fullName, bool searchInAssemblies, bool cache, bool isCaseSensitive);
		[Obsolete("Use FindElementByFullName instead.")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		IElement FindTypeOrNamespaceByFullName(string fullName, bool searchInAssemblies, bool cache);
	IAssemblyReference FindAssemblyByAlias(string alias);
	IElementCollection FindAllAssembliesByAlias(string alias);
		IElement FindElementInsideIncludes(string filePath, string name);
	IElementCollection FindExtensionMethods(string nameSpace, string name, bool isCaseSensitive);
		IElementCollection FindExtensionMethods(ISourceTreeResolver resolver, string nameSpace, string name, bool isCaseSensitive);
		StringCollection GetFileImportedNamespaces(ISourceFile file);
	[EditorBrowsable(EditorBrowsableState.Never)]
	StringCollection GetFileImportedNamespaces(SourceFile file, bool useFileUsingList);
		IElementCollection GetModuleMembers(string name);
	IElementCollection GetModules(string name);
		IElementFilter GetDirectlyVisibleMembersFilter();
		bool IsBuiltInType(string name);
		bool IsBuiltInType(IElement element, string name);
		string GetFullTypeName(string name);
		string GetFullTypeName(IElement element, string name);
		string GetSimpleTypeName(string name);
		string GetSimpleTypeName(IElement element, string name);
		ISourceFile FindDiskFile(string path);
		ISourceFile FindDiskFile(ISourceFile file);
		bool IsCaseSensitive(IElement element);
	IElementCollection FindCssStylesByClassName(ISourceFile file, string styleName, string ancestorName);
	IElementCollection FindCssStylesById(ISourceFile file, string id, string ancestorName);
	IEnumerable<IAttributeElement> GetAssemblyAttributes();
		IProjectSourceModelCache SourceModel { get; }
		bool HasRootNamespace { get; }
		string RootNamespace { get; }
		bool IsClosed { get; }
		bool IsCaseSensitiveLanguage { get; }
		string Language { get; }
		ISolutionElement Solution { get; }
		string Name { get; }
		string FullName { get; }
		bool HasNativeCompileOptions { get; }
		bool HasOldSyntaxCompileOptions { get; }
		bool CanUseAspImportedNamespaces { get; }
	string AssemblyName { get; }
	string[] FriendAssemblyNames { get; }
	StringCollection ImportedNamespaces { get; }
	FrameworkVersion TargetFramework { get; }
	bool NotImportStdLib { get; }
	bool SupportsRootNamespace { get; }
	void InvalidateProjectSymbols(SourceFile sourceFile);
	void ReleaseDiskFile(SourceFile proxy);
	void BuildProjectSymbols(SourceFile sourceFile);
	SourceFile SynchronizeDiskFile(string name, SourceFile value);
	void RemoveFilePathsForMacroCalls(System.Collections.Generic.List<string> macroCalls, string fileName);
	OptionInfer OptionInfer { get; set; }
	OptionStrict OptionStrict { get; set; }
	OptionExplicit OptionExplicit { get; set; }
	IElement GetParent(LanguageElementType languageElementType);
	int AllFilesCount { get; }
	NodeList AllFiles { get; }
	string FilePath { get; }
	bool IsMiscProject { get; }
	bool IsWebSite { get; }
	string[] Defines { get; }
	Dictionary<string, object> ValueDefines { get; }
	void ClearProjectSymbols(SourceFile sourceFile);
  }
}
