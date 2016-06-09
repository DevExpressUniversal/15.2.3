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
using System.IO;
using System.Text;
using System.ComponentModel;
#if SL
using DXEncoding = DevExpress.Utils.DXEncoding;
#else
using DXEncoding = System.Text.Encoding;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public sealed class LanguageElementRestorer
	{
		LanguageElementRestorer()
		{
		}
		static string GetText(SourceFile file)
		{
			string lResult = String.Empty;
			if (file == null)
				return lResult;
			if (file.Document != null)
				return file.Document.Text;
			string lPath = file.Name;
			if (lPath == null || !File.Exists(lPath))
				return lResult;
			using (StreamReader lReader = new StreamReader(lPath, DXEncoding.Default))
			{
				lResult = lReader.ReadToEnd();
			}
			return lResult;
		}
		static LanguageElement GetStartForExpression(LanguageElement node)
		{
			if (node == null)
				return null;
			LanguageElement result = node.GetParentStatementOrVariable();
			if (result == null)
				result = node.GetParent(LanguageElementType.NamespaceReference);
			if (result == null)
				result = node.GetParentMethodOrAccessor();
			if (result == null)
				result = node.GetParentClassInterfaceStructOrModule();
			if (result == null)
				result = node.Parent;
			return result;
		}
		static LanguageElement GetAppropriateStart(LanguageElement scope, SourceRange range)		
		{
			if (scope == null || range.IsEmpty)
				return null;
			LanguageElement node = scope.GetChildAt(range.Start);
			if (node == null)
				return null;
			if (node is Expression || node is MethodCall)
				return GetStartForExpression(node);
			else
				return node;
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static LanguageElement FindElement(LanguageElement scope, SourceRange range, bool checkNameRangeOnly)
	{
	  return FindElement(scope, range, LanguageElementType.Unknown, checkNameRangeOnly);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static LanguageElement FindElement(LanguageElement scope, SourceRange range, LanguageElementType type)
	{
	  return FindElement(scope, range, type, true);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static LanguageElement FindElement(LanguageElement scope, SourceRange range, LanguageElementType type, bool checkNameRangeOnly)
		{
			if (scope == null)
				return null;
			LanguageElement start = GetAppropriateStart(scope, range);
			if (start == null)
				return null;
			SourceTreeEnumerator treeEnum = new SourceTreeEnumerator(scope, start, null);
			while (treeEnum.MoveNext())
			{
				LanguageElement current = treeEnum.Current as LanguageElement;
				if (current == null)
					continue;
		SourceRange currentRange = current.NameRange;
				if (currentRange.IsEmpty)
					currentRange = current.Range;
		if (currentRange != range)
		{
		  if (checkNameRangeOnly)
			continue;
		  if (current.Range != range)
			continue;
		}
				if (type == LanguageElementType.Unknown || IsValidType(type, current.ElementType))
					return current;
			}
			return null;
		}
	static bool IsValidType(LanguageElementType first, LanguageElementType second)
	{
	  if (IsValidQualifiedTypeReference(first, second))
		return true;
	  if (IsValidVariable(first, second))
		return true;
	  return first == second;
	}
	static bool IsValidVariable(LanguageElementType first, LanguageElementType second)
	{
	  {
		if (first == LanguageElementType.Variable)
		  return second == LanguageElementType.InitializedVariable;
		if (second == LanguageElementType.Variable)
		  return first == LanguageElementType.InitializedVariable;
		if (first == LanguageElementType.ImplicitVariable)
		  return second == LanguageElementType.InitializedVariable;
		if (second == LanguageElementType.ImplicitVariable)
		  return first == LanguageElementType.InitializedVariable;
		return false;
	  }
	}
	static bool IsValidQualifiedTypeReference(LanguageElementType first, LanguageElementType second)
	{
	  if (first == LanguageElementType.QualifiedTypeReferenceExpression)
		return second == LanguageElementType.TypeReferenceExpression ||
		  second == LanguageElementType.QualifiedTypeReferenceExpression;
	  if (second == LanguageElementType.QualifiedTypeReferenceExpression)
		return first == LanguageElementType.TypeReferenceExpression ||
		  first == LanguageElementType.QualifiedTypeReferenceExpression;
	  return false;
	}
	static bool TryConvertAspxFieldTag(LanguageElementCollection result, IFieldElement field, IElement element)
	{
	  if (!field.IsAspxTag)
		return false;
	  IProjectElement project = element.Project as IProjectElement;
	  if (project == null)
		return false;
	  ICanRestoreFromGeneratedSymbols cache = project.SourceModel as ICanRestoreFromGeneratedSymbols;
	  if (cache == null)
		return false;
	  LanguageElement restored = cache.RestoreFromGeneratedSymbols(element) as LanguageElement;
	  if (restored == null)
		return false;
	  result.Add(restored);
	  return true;
	}
	public static LanguageElementCollection ConvertToLanguageElements(IElement element)
	{
	  LanguageElementCollection result = new LanguageElementCollection();
	  if (element == null)
		return result;
	  if (element is LanguageElement)
	  {
		result.Add((LanguageElement)element);
		return result;
	  }
	  if (element is IFieldElement)
	  {
		IFieldElement field = (IFieldElement)element;
		if (field.HasNodeLink && field.NodeLink is LanguageElement)
		{
		  result.Add((LanguageElement)field.NodeLink);
		  return result;
		}
		if (TryConvertAspxFieldTag(result, field, element))
		  return result;
	  }
	  if (element.Files == null || element.Files.Count == 0)
		return null;
	  IProjectElement lProject = element.Project as IProjectElement;
	  if (lProject == null)
		lProject = element.FirstFile.Project;
	  if (lProject == null)
		return null;
	  int lFilesCount = element.Files.Count;
	  int nameRangesCount = element.NameRanges.Count;
	  if (lFilesCount != nameRangesCount)
		return null;
	  LanguageElementType type = element.ElementType;
	  if (type == LanguageElementType.Method && element.Parent is IPropertyElement)
		if (element.Name.StartsWith("get_"))
		  type = LanguageElementType.PropertyAccessorGet;
		else
		  type = LanguageElementType.PropertyAccessorSet;
	  for (int i = 0; i < lFilesCount; i++)
	  {
		SourceFile lFile = lProject.FindDiskFile(element.Files[i].Name) as SourceFile;
		if (lFile == null)
		  continue;
		LanguageElement scope = lFile;
		IAspSourceFile IAsp = (IAspSourceFile)lFile;
		if (IAsp.Code != null)
		  scope = IAsp.Code;
		bool checkNameRangeOnly = NeedsToCheckNameRangeOnly(type);
		LanguageElement lElement = FindElement(scope, element.NameRanges[i], type, checkNameRangeOnly);
		if (lElement != null)
		  result.Add(lElement);
	  }
	  return result;
	}
	static bool NeedsToCheckNameRangeOnly(LanguageElementType type)
	{
	  return type != LanguageElementType.LogicalOperation;
	}
		public static LanguageElement ConvertToLanguageElement(IElement element)
		{
			LanguageElementCollection elements = ConvertToLanguageElements(element);
			if (elements == null || elements.Count == 0)
				return  null;
			return  elements[0];
		}
		public static LanguageElementCollection ConvertToLanguageElements(IElementCollection elements)
		{
			LanguageElementCollection lResult = new LanguageElementCollection();
			if (elements == null || elements.Count == 0)
				return lResult;
			int lCount = elements.Count;
			for (int i = 0; i < lCount; i++)
			{
				LanguageElement lElement = ConvertToLanguageElement(elements[i]);
				if (lElement != null)
					lResult.Add(lElement);
			}
			return lResult;
		}
	}
}
