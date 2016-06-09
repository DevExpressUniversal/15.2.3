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
using System.Collections.Generic;
using System.Text;
using System.Collections;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public static class ParserUtils
  {
	public enum Position
	{
	  BeforeStart,
	  Inside,
	  AfterEnd,
	  Error
	}
	public static object GetCloneFromNodeList(NodeList target, NodeList source, object obj)
	{
	  if (target == null || source == null || obj == null)
		return null;
	  int lIndex = source.IndexOf(obj);
	  if (lIndex >= 0 && lIndex < target.Count)
		return target[lIndex];
	  return null;
	}
	public static object GetCloneFromNodes(BaseElement target, BaseElement source, object obj)
	{
	  if (target == null || source == null || obj == null)
		return null;
	  object lObj = GetCloneFromNodeList(target.DetailNodes, source.DetailNodes, obj);
	  if (lObj != null)
		return lObj;
	  lObj = GetCloneFromNodeList(target.Nodes, source.Nodes, obj);
	  if (lObj != null)
		return lObj;
	  return null;
	}
	public static void GetClonesFromNodes(NodeList targetNodes, NodeList sourceNodes, NodeList targetCollection, NodeList sourceCollection)
	{
	  if (targetNodes == null || sourceNodes == null || targetCollection == null || sourceCollection == null)
		return;
	  for (int i = 0; i < sourceCollection.Count; i++)
	  {
		LanguageElement lSourceElement = sourceCollection[i] as LanguageElement;
		if (lSourceElement == null)
		  continue;
		int lIndex = sourceNodes.IndexOf(lSourceElement);
		if (lIndex < 0 || lIndex >= targetNodes.Count)
		  continue;
		LanguageElement lTargetElement = targetNodes[lIndex] as LanguageElement;
		if (lTargetElement == null)
		  continue;
		targetCollection.Add(lTargetElement);
	  }
	}
	public static LanguageElementCollection GetSelectedNodes(LanguageElement scope, SourceRange range)
	{
	  LanguageElementCollection lResult = new LanguageElementCollection();
	  if (scope == null || range.IsEmpty)
		return lResult;
	  SourceTreeEnumerator lEnumerator = new SourceTreeEnumerator(scope);
	  while (lEnumerator.MoveNext())
	  {
		LanguageElement lNode = lEnumerator.Current as LanguageElement;
		if (lNode == null)
		  continue;
		if (range.Contains(lNode.Range))
		  lResult.Add(lNode);
	  }
	  return lResult;
	}
	static bool IsBadPoint(SourcePoint point)
	{
	  return point.Line <= 0 || point.Offset <= 0;
	}
	static bool ContainsPoint(IList list, int startIndex, int endIndex, SourcePoint point)
	{
	  LanguageElement start = list[startIndex] as LanguageElement;
	  LanguageElement end = list[endIndex] as LanguageElement;
	  if (start == null || end == null)
		return false;
	  SourceRange commonRange = new SourceRange(start.Range.Start, end.Range.End);
	  return RangeIsCorrupted(commonRange) || !commonRange.IsEmpty && commonRange.Contains(point);
	}
	public static bool RangeIsCorrupted(SourceRange range)
	{
	  return IsBadPoint(range.Start) || IsBadPoint(range.End);
	}
	static Position GetPosition(IList list, int startIndex, int endIndex, SourcePoint point)
	{
	  if(list == null || list.Count == 0 || point.IsEmpty || IsBadPoint(point) || startIndex > endIndex)
		return Position.Error;
	  try
	  {
		LanguageElement first = list[startIndex] as LanguageElement;
		if(first == null || first.Range.IsEmpty || RangeIsCorrupted(first.Range))
		  return Position.Error;
		LanguageElement last = list[endIndex] as LanguageElement;
		if(last == null || last.Range.IsEmpty || RangeIsCorrupted(last.Range))
		  return Position.Error;
		if(point < first.Range.Start)
		  return Position.BeforeStart;
		if(point >= last.Range.End)
		  return Position.AfterEnd;
		return Position.Inside;
	  }
	  catch { return Position.Error; }
	}
	public static int GetIndexAt(IList list, SourcePoint point)
	{
	  return GetIndexAt(list, 0, list.Count - 1, point);
	}
	public static int GetIndexAt(IList list, SourcePoint point, out Position position)
	{
	  return GetIndexAt(list, 0, list.Count - 1, point, out position);
	}
	public static int GetIndexAt(IList list, int startIndex, int endIndex, SourcePoint point)
	{
	  Position position;
	  return GetIndexAt(list, startIndex, endIndex, point, out position);
	}
	public static int GetIndexAt(IList list, int startIndex, int endIndex, SourcePoint point, out Position position)
	{
	  if((position = GetPosition(list, startIndex, endIndex, point)) != Position.Inside)
		return -1;
	  for(;;)
	  {
		int middleIndex = (endIndex + startIndex) / 2;
		if (middleIndex == startIndex)
		  return startIndex;
		if(ContainsPoint(list, middleIndex, endIndex, point))
		  startIndex = middleIndex;
		else
		  endIndex = middleIndex;
	  }
	}
	public static bool GetBounds(IList list, SourceRange range, out int left, out int right)
	{
	  return GetBounds(list, 0, list.Count - 1, range, out left, out right);
	}
	public static bool GetBounds(IList list, int startIndex, int endIndex, SourceRange range, out int left, out int right)
	{
	  Position startPosition;
	  left = GetIndexAt(list, startIndex, endIndex, range.Start, out startPosition);
	  if(startPosition == Position.BeforeStart)
		left = startIndex;
	  else if(startPosition == Position.Inside)
		left++;
	  Position endPosition;
	  right = GetIndexAt(list, startIndex, endIndex, range.End, out endPosition);
	  if(endPosition == Position.AfterEnd)
		right = endIndex;
	  return left != -1 && right != -1 && left <= right;
	}
	public static IList<LanguageElement> GetNodesInRange(LanguageElement scope, SourceRange range)
	{
	  return GetNodesInRange(scope, range, false);
	}
	public static IList<LanguageElement> GetNodesInRange(LanguageElement scope, SourceRange range, bool useBinarySerach)
	{
	  List<LanguageElement> result = new List<LanguageElement>();
	  if (scope == null)
		return result;
	  return GetNodesInRange(scope.Nodes, range, useBinarySerach);
	}
	public static IList<LanguageElement> GetNodesInRange(NodeList nodes, SourceRange range)
	{
	  return GetNodesInRange(nodes, range, false);
	}
	public static IList<LanguageElement> GetNodesInRange(NodeList nodes, SourceRange range, bool useBinarySerach)
	{
	  List<LanguageElement> result = new List<LanguageElement>();
	  if (nodes == null || nodes.Count == 0 || range.IsEmpty)
		return result;
	  int count = nodes.Count;
	  int index = useBinarySerach ? GetIndexAt(nodes, 0, count - 1, range.Start) : 0;
	  if (index < 0)
		index = 0;
	  for (int i = index; i < count; i++)
	  {
		LanguageElement current = nodes[i] as LanguageElement;
		if (current == null)
		  continue;
		SourceRange currentRange = current.Range;
		if (range.Contains(currentRange))
		  result.Add(current);
		if (useBinarySerach && currentRange.Start > range.End)
		  break;
	  }
	  return result;
	}
	public static void RemoveNodesInRange(LanguageElement scope, SourceRange range)
	{
	  if (scope == null)
		return;
	  RemoveNodesInRange(scope.Nodes, range);
	}
	public static void RemoveNodesInRange(NodeList nodes, SourceRange range, bool useBinarySerach, bool recursive)
	{
	  RemoveNodesInRange(nodes, range, useBinarySerach);
	  if (!recursive)
		return;
	  foreach (LanguageElement element in nodes)
		RemoveNodesInRange(element.Nodes, range, useBinarySerach, recursive);
	}
	public static void RemoveNodesInRange(NodeList nodes, SourceRange range)
	{
	  RemoveNodesInRange(nodes, range, false);
	}
	public static void RemoveNodesInRange(NodeList nodes, SourceRange range, bool useBinarySerach)
	{
	  if (nodes == null || range.IsEmpty)
		return;
	  IList<LanguageElement> nodesInRange = GetNodesInRange(nodes, range, useBinarySerach);
	  if (nodesInRange == null)
		return;
	  for (int i = 0; i < nodesInRange.Count; i++)
		nodes.Remove(nodesInRange[i]);
	}
	public static int CountChildStatements(Statement statement)
	{
	  if (statement == null)
		return 0;
	  return CountStatements(statement.Nodes);
	}
	public static bool IsStatement(LanguageElement element)
	{
	  return element is Statement ||
		element is Variable ||
		element is InitializedVariable;
	}
	public static int CountStatements(NodeList nodes)
	{
	  if (nodes == null)
		return 0;
	  int lStatementCount = 0;
	  int lCount = nodes.Count;
	  for (int i = 0; i < lCount; i++)
	  {
		LanguageElement lElement = nodes[i] as LanguageElement;
		if (lElement == null)
		  continue;
		if (lElement.CompletesPrevious)
		  continue;
		if (IsStatement(lElement))
		  lStatementCount++;
	  }
	  return lStatementCount;
	}
	public static string GetElementTypeName(LanguageElement element)
	{
	  if (element == null)
		return String.Empty;
	  if (element is BaseVariable)
		return ((BaseVariable)element).MemberType;
	  if (element is Param)
		return ((Param)element).ParamType;
	  if (element is Member)
		return ((Member)element).MemberType;
	  return String.Empty;
	}
	public static TypeReferenceExpression GetElementType(LanguageElement element)
	{
	  if (element == null)
		return null;
	  TypeReferenceExpression lTypeRef = null;
	  if (element is BaseVariable)
		lTypeRef = ((BaseVariable)element).MemberTypeReference;
	  if (element is Member)
		lTypeRef = ((Member)element).MemberTypeReference;
	  if (lTypeRef != null && lTypeRef.Parent == null)
		lTypeRef.SetParent(element); 
	  return lTypeRef;
	}
  }
}
