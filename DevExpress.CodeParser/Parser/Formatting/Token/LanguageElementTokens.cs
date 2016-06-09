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
using System.Collections.Generic;
using System.ComponentModel;
#if DXCORE
using DevExpress.CodeRush.StructuralParser.Diagnostics;
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public enum TokenPosition : byte
  {
	Inside = byte.MaxValue,
	BeforeFirstCodeChild = 1,
	AfterEnd = 3
  }
  public class LanguageElementTokens
  {
	Dictionary<FormattingTokenType, FormattingElementCollection> _Coll;
	private LanguageElementTokens()
	{
	}
	public LanguageElementTokens(List<FormattingParsingElement> elements)
	{
	  Convert(elements);
	}
	public LanguageElementTokens(FormattingTokenType type, FormattingElementCollection collection)
	{
	  if (_Coll == null)
		_Coll = new Dictionary<FormattingTokenType, FormattingElementCollection>();
	  _Coll[type] = collection;
	}
	public LanguageElementTokens(FormattingTokenType[] type, FormattingElementCollection[] collection)
	{
	  if (type.Length != collection.Length)
		return;
	  if(_Coll == null)
		_Coll = new Dictionary<FormattingTokenType,FormattingElementCollection>();
	  for (int i = 0; i < type.Length; i++)
		_Coll[type[i]] = collection[i];
	}
	void Convert(List<FormattingParsingElement> elements)
	{
	  if (elements == null)
		return;
	  _Coll = new Dictionary<FormattingTokenType, FormattingElementCollection>();
	  int count = elements.Count;
	  for (int i = 0; i < count; i++)
	  {
		FormattingParsingElement element = elements[i];
		FormattingTokenType type = element.FormattingType;
		FormattingElementCollection current;
		if (!_Coll.TryGetValue(type, out current))
		{
		  current = new FormattingElementCollection();
		  _Coll.Add(type, current);
		}
		if (element.FormattingElements != null)
		  current.Add(element.FormattingElements);
	  }
	}
	static bool DeleteDirective(LanguageElement fromElement, PreprocessorDirective searchElement)
	{
	  foreach (FormattingElementCollection collection in fromElement.Tokens._Coll.Values)
		foreach (FormattingElements elements in collection)
		  if (elements.DeleteDirective(searchElement))
			return true;
	  foreach (LanguageElement detailNode in fromElement.DetailNodes)
		if (DeleteDirective(detailNode, searchElement))
		  return true;
	  return false;
	}
	FormattingElementCollection GetFormattingElementCollection(TokenPosition position)
	{
	  FormattingElementCollection collection = null;
	  if (position == TokenPosition.BeforeFirstCodeChild)
	  {
		if (!_Coll.TryGetValue(FormattingTokenType.CurlyBraceOpen, out collection))
		  _Coll.TryGetValue(FormattingTokenType.SourceFileStart, out collection);
	  }
	  else if (position == TokenPosition.AfterEnd)
		if (!_Coll.TryGetValue(FormattingTokenType.CurlyBraceClose, out collection))
		  if (!_Coll.TryGetValue(FormattingTokenType.Semicolon, out collection))
			if (!_Coll.TryGetValue(FormattingTokenType.BracketClose, out collection))
			  if(!_Coll.TryGetValue(FormattingTokenType.Comma, out collection))
				_Coll.TryGetValue(FormattingTokenType.Ident, out collection);
	  return collection;
	}
	FormattingElements GetFormattingElements(TokenPosition position)
	{
	  FormattingElementCollection coll = GetFormattingElementCollection(position);
	  if (coll == null || coll.Count == 0)
		return null;
	  return coll[0];
	}
	bool LastIsEOL(FormattingElements elements)
	{
	  if (elements.Count == 0)
		return false;
	  FormattingElement fe = elements[elements.Count - 1] as FormattingElement;
	  return fe != null && fe.Type == FormattingElementType.EOL;
	}
	int FindLineByIndex(List<int> indexesEOLs, int index)
	{
	  if (index == -1)
		return -1;
	  int i;
	  for (i = 0; i < indexesEOLs.Count; i++)
		if (indexesEOLs[i] >= index)
		  return i;
	  return i;
	}
	FormattingElements CutTokens(FormattingElements elements, PreprocessorDirective leave, LanguageElement cut, 
	  List<FullDirective> emptyFulls, Dictionary<PreprocessorDirective, LanguageElement> regionsToDelete,
	  out LanguageElementCollection commentsToFirst, out LanguageElementCollection commentsToSecond)
	{
	  if(elements == null)
	  {
		commentsToFirst = commentsToSecond = null;
		return null;
	  }
	  commentsToFirst = new LanguageElementCollection();
	  commentsToSecond = new LanguageElementCollection();
	  List<int> indexesEOLs = new List<int>();
	  List<bool> haveLEInLines = new List<bool>();
	  Dictionary<LanguageElement, int> supportElementsInLines = new Dictionary<LanguageElement, int>();
	  int indexLeave = leave != null ? elements.IndexOf(leave) : -1;
	  if (indexLeave == -1)
		indexLeave = 0;
	  int indexCut = cut != null ? elements.IndexOf(cut as IFormattingElement) : -1;
	  if (indexCut == -1)
		indexCut = elements.Count;
	  bool haveLE = false;
	  int startLineForDeleting = 1;
	  for (int i = 0; i < elements.Count; i++)
	  {
		IFormattingElement ife = elements[i];
		if (ife is SourceFileStartText)
		{
		  haveLEInLines.Add(haveLE = false);
		  indexesEOLs.Add(i);
		  startLineForDeleting = 0;
		}
		FormattingElement fe = ife as FormattingElement;
		if (fe != null)
		{
		  if (fe.Type != FormattingElementType.EOL)
			continue;
		  haveLEInLines.Add(haveLE);
		  haveLE = false;
		  indexesEOLs.Add(i);
		}
		else
		{
		  LanguageElement le = ife as LanguageElement;
		  if (le == null)
			continue;
		  LanguageElementType type = le.ElementType;
		  if (regionsToDelete != null && (type == LanguageElementType.Region || type == LanguageElementType.EndRegionDirective))
		  {
			PreprocessorDirective region = le as PreprocessorDirective;
			if (le != null && regionsToDelete.ContainsKey(region))
			{
			  elements.RemoveAt(i);
			  if (indexLeave > i)
				indexLeave--;
			  if (indexCut > i)
				indexCut--;
			  i--;
			  regionsToDelete.Remove(region);
			}
			else
			  haveLE = true;
		  }
		  else
		  {
			haveLE = true;
			if (type == LanguageElementType.Comment || type == LanguageElementType.FictiveAspComment ||
			type == LanguageElementType.XmlDocComment)
			  supportElementsInLines.Add(le, indexesEOLs.Count);
		  }
		}
	  }
	  int cutingFirstLine = (FindLineByIndex(indexesEOLs, indexCut) + FindLineByIndex(indexesEOLs, indexLeave) + 1) / 2;
	  if (emptyFulls != null)
		foreach (FullDirective full in emptyFulls)
		{
		  int lineFirst = FindLineByIndex(indexesEOLs, elements.IndexOf(full.FirstSimple));
		  if (lineFirst > cutingFirstLine)
			break;
		  int lineLast = FindLineByIndex(indexesEOLs, elements.IndexOf(full.LastSimple));
		  if (lineFirst == -1 || lineLast == -1)
			continue;
		  if (lineLast >= cutingFirstLine)
		  {
			if (cutingFirstLine - lineFirst <= lineLast - cutingFirstLine)
			  cutingFirstLine = lineFirst;
			else
			  cutingFirstLine = lineLast + 1;
			break;
		  }
		}
	  foreach (KeyValuePair<LanguageElement, int> pair in supportElementsInLines)
		if (pair.Value < cutingFirstLine)
		  commentsToFirst.Add(pair.Key);
		else
		  commentsToSecond.Add(pair.Key);
	  int endLine = indexesEOLs.Count;
	  for (int i = startLineForDeleting; i < endLine; i++)
	  {
		if (haveLEInLines[i])
		  continue;
		int numberPreviousEOL = i > 0 ? indexesEOLs[i - 1] : -1;
		int numberThisEOL = indexesEOLs[i];
		int countRemove = numberThisEOL - numberPreviousEOL;
		elements.RemoveRange(numberPreviousEOL + 1, countRemove);
		for (int j = i; j < endLine; j++)
		  indexesEOLs[j] -= countRemove;
	  }
	  int indexFirstToCut = cutingFirstLine > 0 ? indexesEOLs[cutingFirstLine - 1] + 1 : 0;
	  FormattingElements result = new FormattingElements();
	  for (int i = indexFirstToCut; i < elements.Count; i++)
		result.Add(elements[i]);
	  elements.RemoveRange(indexFirstToCut, elements.Count - indexFirstToCut);
	  return result;
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void AddElements(FormattingTokenType type, IEnumerable<IFormattingElement> fe)
	{
	  if (_Coll == null)
		return;
	  FormattingElementCollection coll = null;
	  if (!_Coll.ContainsKey(type))
	  {
		coll = new FormattingElementCollection();
		_Coll.Add(type, coll);
	  }
	  else
		coll = _Coll[type];
	  FormattingElements elements = null;
	  if (coll.Count == 0)
	  {
		elements = new FormattingElements();
		coll.Add(elements);
	  }
	  else
		elements = coll[0];
	  elements.AddRange(fe);
	}
	public bool HasElement(FormattingTokenType type)
	{
	  if (_Coll == null)
		return false;
	  return _Coll.ContainsKey(type);
	}
	public void RemoveAll(Type type)
	{
	  foreach (FormattingElementCollection elements in _Coll.Values)
	  {
		for (int i = 0; i < elements.Count; i++)
		{
		  FormattingElements tokenElements = elements[i];
		  FormattingElements newElements = new FormattingElements();
		  foreach (IFormattingElement element in tokenElements)
			if (element.GetType() != type)
			  newElements.Add(element);
		  if (newElements.Count != tokenElements.Count)
			elements[i] = newElements;
		}
	  }
	}
	public FormattingElements Get(FormattingTokenType type, int index)
	{
	  if (type == FormattingTokenType.None || index < 0)
		return null;
	  FormattingElementCollection fes;
	  if (!_Coll.TryGetValue(type, out fes))
		return null;
	  if (fes == null || fes.Count <= index)
		return null;
	  return fes[index];
	}
	public LanguageElementTokens Clone(ElementCloneOptions options)
	{
	  LanguageElementTokens result = new LanguageElementTokens();
	  result._Coll = new Dictionary<FormattingTokenType, FormattingElementCollection>();
	  foreach (FormattingTokenType item in _Coll.Keys)
	  {
		FormattingElementCollection coll = _Coll[item];
		if (coll == null)
		  continue;
		result._Coll[item] = coll.CloneList(options);
	  }
	  return result;
	}
	public PreprocessorDirective GetLeaveFromSourceFile(SourceFile file, List<FullDirective> fulls)
	{
	  FormattingElements elements = GetFormattingElements(TokenPosition.BeforeFirstCodeChild);
	  PreprocessorDirective result = null;
	  for (int i = elements.Count - 1; i >= 0; i--)
		if ((result = elements[i] as DefineDirective) != null || (result = elements[i] as UndefDirective) != null)
		  break;
	  if(result == null || fulls == null)
		return result;
	  foreach(FullDirective full in fulls)
	  {
		if(full.EndLine < result.StartLine)
		  continue;
		if(full.StartLine < result.StartLine)
		  return full.LastSimple;
		break;
	  }
	  return result;
	}
	public FormattingElements CutTokens(TokenPosition positionFrom, PreprocessorDirective leave, LanguageElement cut, List<FullDirective> emptyFulls,
	  Dictionary<PreprocessorDirective, LanguageElement> regionsToDelete,
	  out LanguageElementCollection commentsToFirst, out LanguageElementCollection commentsToSecond)
	{
	  FormattingElements elements = GetFormattingElements(positionFrom);
	  return CutTokens(elements, leave, cut, emptyFulls, regionsToDelete, out commentsToFirst, out commentsToSecond);
	}
	public void CutTokens(FormattingTokenType type, int index, PreprocessorDirective leave, LanguageElement cut, List<FullDirective> emptyFulls,
	  Dictionary<PreprocessorDirective, LanguageElement> regionsToDelete, 
	  out FormattingElements fesToFirst, out FormattingElements fesToSecond,
	  out LanguageElementCollection elementsToFirst, out LanguageElementCollection elementsToSecond)
	{
	  FormattingElementCollection coll;
	  if(!_Coll.TryGetValue(type, out coll) || coll.Count <= index)
	  {
		fesToFirst = fesToSecond = null;
		elementsToFirst = elementsToSecond = null;
		return;
	  }
	  fesToFirst = coll[index];
	  fesToSecond = CutTokens(fesToFirst, leave, cut, emptyFulls, regionsToDelete, out elementsToFirst, out elementsToSecond);
	  coll[index] = new FormattingElements();
	}
	public static void DeleteRegions(Dictionary<PreprocessorDirective, LanguageElement> regionsToDelete)
	{
	  foreach(KeyValuePair<PreprocessorDirective, LanguageElement> pair in regionsToDelete)
		DeleteDirective(pair.Value, pair.Key);
	}
	public bool AddEOLToEnd()
	{
	  FormattingElements elements = GetFormattingElements(TokenPosition.AfterEnd);
	  if(elements.Count > 0)
		return false;
	  elements.Add(new FormattingElement(FormattingElementType.EOL));
	  return true;
	}
	public void DeleteLastEOL()
	{
	  FormattingElements elements = GetFormattingElements(TokenPosition.AfterEnd);
	  if(LastIsEOL(elements))
		elements.RemoveAt(elements.Count - 1);
	}
	public bool PasteTokens(TokenPosition toPosition, FormattingElements elementsToAdd)
	{
	  if (elementsToAdd == null || elementsToAdd.Count == 0)
		return false;
	  FormattingElements elements = GetFormattingElements(toPosition);
	  elements.AddRange(elementsToAdd);
	  return true;
	}
	public void PasteTokens(FormattingTokenType typeToAdd, int indexToAdd, FormattingElements elementsToAdd)
	{
	  FormattingElementCollection coll;
	  if(!_Coll.TryGetValue(typeToAdd, out coll) || coll.Count <= indexToAdd)
		return;
	  coll[indexToAdd].AddRange(elementsToAdd);
	}
	public void AddCommentOrRegion(TokenPosition toPosition, int indent, LanguageElement toAdd)
	{
	  IFormattingElement ifeToAdd = toAdd as IFormattingElement;
	  if (ifeToAdd == null)
		return;
	  FormattingElements elements = elements = GetFormattingElements(toPosition);
	  bool fromSourceFileStart = _Coll.ContainsKey(FormattingTokenType.SourceFileStart);
	  if (!fromSourceFileStart && !LastIsEOL(elements))
		elements.Add(new FormattingElement(FormattingElementType.EOL));
	  for (int i = 0; i < indent + indent; i++)
		elements.Add(new FormattingElement(FormattingElementType.WS));
	  elements.Add(ifeToAdd);
	  elements.Add(new FormattingElement(FormattingElementType.EOL));
	}
	public void AddEmptyLinesToEnd(int countEmptyLines)
	{
	  FormattingElements elements = GetFormattingElements(TokenPosition.AfterEnd);
	  if (!LastIsEOL(elements))
		countEmptyLines++;
	  for (int i = 0; i < countEmptyLines; i++)
		elements.Add(new FormattingElement(FormattingElementType.EOL));
	}
	public void DeleteEmptyLines(FormattingTokenType type, int index)
	{
	  DeleteEmptyLines(type, index, false);
	}
	public void DeleteEmptyLines(FormattingTokenType type, int index, bool attemptDeleteLastLine)
	{
	  FormattingElementCollection coll;
	  if (!_Coll.TryGetValue(type, out coll) || coll.Count <= index)
		return;
	  FormattingElements elements = coll[index];
	  elements.DeleteEmptyLines(attemptDeleteLastLine);
	}
  }
}
