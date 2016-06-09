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
using System.Collections.Generic;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  using Diagnostics;
  public interface IMarkupElement
  { 
  }
	public abstract class LanguageElement : DocumentElement, IElement, IElementModifier, IEnumerable	
	{
		const int INT_FirstColumnPos = 1;
		bool _IsFakeNode;
		bool _HasErrors;
	LanguageElementTokens _Tokens;
	bool _IsRemoved;
		class LanguageElementStartComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null)
					return -1;
				return ((LanguageElement)x).InternalRange.Start.CompareTo(y);
			}
		}
		class LanguageElementEndComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null)
					return -1;
				return ((LanguageElement)x).InternalRange.End.CompareTo(y);
			}
		}
		class LanguageElementRangeComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null)
					return -1;
				LanguageElement lElement = ((LanguageElement)x);
				SourceRange lRange = lElement.InternalRange;
				if (lRange.End.IsEmpty || lRange.Start.IsEmpty)
					return -1;
				int end = lRange.End.CompareTo(y);
				int start = lRange.Start.CompareTo(y);
				if(end == start)
					return end;
				return 0;
			}
		}
		const int INT_LoopPreventCount = 10000;
		#region private fields...
		short _SiblingCounter;
		LanguageElement _Parent;
		int _Index = -1;
	bool _IsDetailNode;		
		#endregion
		#region LanguageElement
		public LanguageElement()
		{
		}
		#endregion
		void SetSelection(int startLine, int startOffset, int endLine, int endOffset, bool ensureVisible)
		{
			IDocument textDocument = Document;
			if (textDocument == null)
				return;
			SetSelection(textDocument.ActiveView, startLine, startOffset, endLine, endOffset, ensureVisible);
		}
		void SetSelection(IDXCoreTextView textView, int startLine, int startOffset, int endLine, int endOffset, bool ensureVisible)
		{
			if (textView == null)
				return;
			textView.Select(startLine, startOffset, endLine, endOffset, ensureVisible);
		}
		void AdjustStartToElement(LanguageElement element, ref int startLine, ref int startOffset)
		{
			if (element.StartLine < startLine)
			{
				startLine = element.StartLine;
				startOffset = element.StartOffset;
			}
			else if (element.StartLine == startLine)
				startOffset = Math.Min(startOffset, element.StartOffset);
		}
		void ParseAllPostponedElementsInNodes(NodeList nodes)
		{
			if (nodes == null)
				return;
			for (int i = 0; i < nodes.Count; i++)
			{
				if (!(nodes[i] is LanguageElement))
					continue;
				LanguageElement lElement = (LanguageElement)nodes[i];
				lElement.ParseAllPostponedElements();
			}
		}
		bool CheckNodeListIndexes(NodeList nodeList)
		{
			if (nodeList == null)
				return true;
			for (int i = 0; i < nodeList.Count; i++)
			{
				if (!(nodeList[i] is LanguageElement))
					continue;
				LanguageElement lNode = (LanguageElement)nodeList[i];
				if (lNode._Index != i) 
					return false;
			}
			return true;
		}
		void CheckIndexes(LanguageElement element)
		{
			if (element == null)
				return;
			if (!CheckNodeListIndexes(element.DetailNodes))
				throw new Exception(String.Format("Detail node indexes are not valid inside {0}.", element.Name));
			if (!CheckNodeListIndexes(element.Nodes))
				throw new Exception(String.Format("Node indexes are not valid inside {0}.", element.Name));
		}
		void ValidateIndices()
		{
			_SiblingCounter ++;
			if (_SiblingCounter > INT_LoopPreventCount)
			{
				CheckIndexes(Parent);
				_SiblingCounter = 0;
			}
		}
		bool FindInList(NodeList list, LanguageElement element)
		{
			if (list == null || list.Count == 0 || element == null)
				return false;
			for (int i = 0; i < list.Count; i++)
			{
				LanguageElement lElement = list[i] as LanguageElement;				
				if (FindInScope(lElement, element))
					return true;				
			}
			return false;
		}
		bool FindInScope(LanguageElement scope, LanguageElement element)
		{			
			if (scope == null || element == null)
				return false;
			if (scope == element)
				return true;
			if (FindInList(scope.Nodes, element))
				return true;
			if (FindInList(scope.DetailNodes, element))
				return true;
			return false;			
		}
		bool IsTrailingComment(LanguageElement target, LanguageElement element)
		{
			if (element == null)
				return false;
			if (!(element is Comment))
				return false;
			Comment lComment = (Comment)element;
			if (lComment.TargetNode != target)
				return false;
			return true;
		}
		void SetNodeLinks(NodeList nodes)
		{
			if (nodes == null)
				return;
			for (int i = 0; i < nodes.Count; i++)
			{
				LanguageElement lElement = nodes[i] as LanguageElement;
				if (lElement == null)
					continue;
				lElement._Parent = this;
				lElement._Index = i;
			}
		}
		LanguageElement GetOuterRangeChild(int line, int column, NodeList nodes)
		{
			if (!HasOuterRangeChildren)
				return null;
			int count = nodes.Count;
			for (int i = 0; i < count; i++)
			{
				LanguageElement node = nodes[i] as LanguageElement;
				LanguageElement child = null;
				if (node.Contains(line, column))
				{
					child = node.GetChildAt(line, column);
					if (child == null)
						child = node;
					return child;
				}
				else if (node.HasOuterRangeChildren)
				{
					child = node.GetChildAt(line, column);
					if (child != null)
						return child;
				}
			}
			return null;
		}
		LanguageElement GetChildAtInternal(int line, int column, NodeList lChildList) 
		{
			int lIndex = lChildList.BinarySearch(0, new SourcePoint(line, column), new LanguageElementRangeComparer());
			if(lIndex < 0)
			{
				LanguageElement child = GetOuterRangeChild(line, column, lChildList);
				if (child != null)
					return child;
				return null;
			}
			LanguageElement lElement = (LanguageElement)lChildList[lIndex];
			SourceRange lRange = lElement.InternalRange;
			if (lRange.Start.IsEmpty || lRange.End.IsEmpty)
				return null;
			LanguageElement lResult = lElement.GetChildAt(line, column);
			if (lResult == null)	
				lResult = lElement;
			return lResult;
		}
		int GetNodeIndexBeforeInternal(NodeList list, int line, int offset)
		{
			if (list == null)
				return -1;
			int lIndex = list.BinarySearch(0, new SourcePoint(line, offset), new LanguageElementEndComparer());
			if(lIndex >= 0)
				lIndex--;
			return lIndex;
		}
		int GetNodeIndexAfterInternal(NodeList list, int line, int offset)
		{
			if (list == null)
				return -1;
			int lIndex = list.BinarySearch(0, new SourcePoint(line, offset), new LanguageElementStartComparer());
			if(lIndex >= 0)
				lIndex++;
			return lIndex;
		}
		static bool IsInsideNodeList(NodeList list, SourcePoint point)
		{
			if (list == null || list.Count == 0 || point == SourcePoint.Empty)
				return false;
			int count = list.Count;
			LanguageElement first = list[0] as LanguageElement;
			LanguageElement last = list[count - 1] as LanguageElement;
			SourceRange range = new SourceRange(first.Range.Start, last.Range.End);
			return range.Contains(point);
		}
	bool CanInjectInside(SourceRange targetRange, SourceRange elementRange)
	{
	  if (targetRange.IsEmpty || elementRange.IsEmpty)
		return false;
	  return targetRange.Contains(elementRange);
	}
	bool CanInjectBetween(SourceRange prev, SourceRange next, SourceRange elementRange)
	{
	  if (next.IsEmpty || elementRange.IsEmpty)
		return false;
	  if (prev.IsEmpty)
		return elementRange.Start <= next.Start;
	  return elementRange.Start > prev.Start && elementRange.Start < next.Start;
	}
	void InjectElement(LanguageElement element)
	{
	  if (element == null)
		return;
	  SourceRange elementRange = element.Range;
	  SourceRange prevRange = SourceRange.Empty;
	  int count = NodeCount;
	  for (int i = 0; i < count; i++)
	  {
		LanguageElement current = Nodes[i] as LanguageElement;
		if (current == null)
		  continue;
		SourceRange currentRange = current.Range;
		if (CanInjectInside(currentRange, elementRange))
		{
		  current.InjectElement(element);
		  return;
		}
		if (CanInjectBetween(prevRange, currentRange, elementRange))
		{
		  InsertNode(i, element);
		  return;
		}
		prevRange = currentRange;
	  }
	  AddNode(element);
	}
	ICollection CollectRegions(LanguageElement regionRootNode)
	{	  
	  Hashtable regions = new Hashtable();
	  foreach (RegionDirective region in new ElementEnumerable(regionRootNode, typeof(RegionDirective), true))
	  {
		SourceRange range = region.Range;
		if (regions.Contains(range))
		  continue;
		regions.Add(range, region);
	  }
	  return regions.Values;
	}
	void InjectRegions(LanguageElement regionRootNode)
	{
	  if (regionRootNode == null)
		return;
	  ICollection allRegions = CollectRegions(regionRootNode);
	  foreach (RegionDirective region in allRegions)
	  {
		SourceRange regionRange = region.Range;
		if (!Range.Contains(regionRange))
		  continue;
		RegionDirective start = region.Clone() as RegionDirective;
		start.SetRange(new SourceRange(regionRange.Start.Line, 1, regionRange.Start.Line, 1));
		EndRegionDirective end = new EndRegionDirective();
		end.SetRange(new SourceRange(regionRange.End.Line, 1, regionRange.End.Line, 1));
		InjectElement(start);
		InjectElement(end);
	  }
	}
	bool NodeIsAlreadyAdded(LanguageElement parent, NodeList nodeList, LanguageElement node)
	{
	  if (node == null)
		return false;
	  if (node.Parent == null)
		return false;
	  if (node.Index < 0)
		return false;
	  return node.OwningList == nodeList &&
		node.Parent == parent;
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void CleanUpPostponedData()
		{
			if (PostponedData == null)
				return;
			PostponedData.CleanUp();
			PostponedData = null;
		}
		#region GetCollapsibleRegion
	protected ICollapsibleRegion GetCollapsibleRegion()
	{
	  return GetCollapsibleRegion(null);
	}
	#endregion
	#region GetCollapsibleRegion
	protected ICollapsibleRegion GetCollapsibleRegion(IDXCoreTextView textView)
	{
	  if (!IsCollapsible)
		return null;
	  if (Document == null)
	  {
		Log.SendWarning("LanguageElement.GetCollapsibleRegion() found a null Document property.");
		return null;
	  }
	  return Document.GetCollapsibleRegion(CollapsibleRange, textView);
	}
	#endregion
		protected bool InsertNodeToNodeListWithoutReindex(NodeList list, int index, LanguageElement node)
		{
			if (list == null)
				return false;
			if (index < 0 || index > list.Count)
		return false;
	  if (node == null)
		return false;
	  LanguageElement parent = null;
	  bool isDetailNode = list == InnerDetailNodes;
			if (list == OwningList)
		isDetailNode = IsDetailNode;
	  node._IsDetailNode = isDetailNode;
			if (list == OwningList)
		parent = Parent;
			else if (list == InnerNodes || list == InnerDetailNodes)
		parent = this;
	  if (NodeIsAlreadyAdded(parent, list, node))
		return false;
	  node.SetParent(parent);
	  list.Insert(index, node);
	  node._Index = index;
	  return true;
		}
		#region IncreaseNodesIndices
		protected void IncreaseNodesIndices(NodeList list, int startFrom)
		{
			list.IncreaseNodesIndices(startFrom);
		}
		#endregion
		#region DecreaseNodesIndices
		protected void DecreaseNodesIndices(NodeList list, int startFrom)
		{
			int lListCount = list.Count;
			for (int i = startFrom; i < lListCount; i++)
			{
				LanguageElement lElement = (LanguageElement)list[i];
				if (lElement != null)
					lElement._Index --;
			}
		}
		#endregion
		#region GetCanBeDocumented
		protected internal virtual bool GetCanBeDocumented()
		{
			return false;
		}
		#endregion
		#region GetPreviousChildIdentifier
		protected virtual LanguageElement GetPreviousChildIdentifier(LanguageElement startingChildNode, LanguageElement aReferenceNode)
		{
			LanguageElement lLanguageElement = null;
	  LanguageElement lTestElement;
			NodeList lArrayList = startingChildNode.OwningList;
			int lChildIndex = startingChildNode._Index;
	  if (lArrayList != null && lChildIndex > 0 && lChildIndex < lArrayList.Count)  
	  {
		int count = lChildIndex - 1;
		for (int i = count; i >= 0; i--)
		{
		  lTestElement = (LanguageElement)(lArrayList[i]);
		  if (lTestElement._Index != i)
		  {
			string msg = String.Format("Parse tree is corrupted at: {0}", lTestElement.Range);
			LanguageElement nextElement = startingChildNode;
			if (i != count)
			  nextElement = (LanguageElement)(lArrayList[i + 1]);
			msg += String.Format("Next node range: {0}", nextElement.Range);
			Log.SendErrorWithStackTrace(msg);
			return null;
		  }
		  if (lTestElement != null && lTestElement != startingChildNode && lTestElement.DeclaresIdentifier)
		  {
			lLanguageElement = lTestElement;
			break;
		  }
		}
	  }
			if (lLanguageElement == null)			
				if (lArrayList != DetailNodes && aReferenceNode.IsParentedBy(this))		
					lLanguageElement = GetLastDetailIdentifier();
			if (lLanguageElement == null)			
				lLanguageElement = GetLastSiblingInScope(aReferenceNode);
			return lLanguageElement;
		}
		#endregion
		#region GetLastSiblingInScope
		protected virtual LanguageElement GetLastSiblingInScope(LanguageElement referenceNode)
		{
			LanguageElement lParent = _Parent;
			LanguageElement lChild = this;
			LanguageElement lLastSiblingInScope = null;
			bool lOkayToCheckDetailNodes = false;		
			while (lParent != null)
			{
				if (lOkayToCheckDetailNodes) 
				{
					LanguageElement lDetailNode = lChild.GetLastDetailIdentifier();
					if (lDetailNode != null)
					{
						lLastSiblingInScope = lDetailNode;
						break;
					}
				}
				if (lParent.AllChildrenAccessibleByGrandchildren())
					lLastSiblingInScope = lParent.LastChild;
				else
					lLastSiblingInScope = lChild;
				if (!lLastSiblingInScope.DeclaresIdentifier)
				{
					if (lLastSiblingInScope._Index == 0)	
					{
						if (lOkayToCheckDetailNodes && referenceNode.IsParentedBy(lLastSiblingInScope))		
							lLastSiblingInScope = lLastSiblingInScope.GetLastDetailIdentifier();
						else 
							lLastSiblingInScope = null;		
					}
					else
						lLastSiblingInScope = lParent.GetPreviousChildIdentifier(lLastSiblingInScope, referenceNode);
				}
				if (lLastSiblingInScope != null)
					break;
				lChild = lParent;
				lParent = lParent._Parent;
				lOkayToCheckDetailNodes = true;
			}
			return lLastSiblingInScope;
		}
		#endregion
		#region AllChildrenAccessibleByGrandchildren
		protected virtual bool AllChildrenAccessibleByGrandchildren()
		{
			return (ElementType == LanguageElementType.Class ||
				ElementType == LanguageElementType.ManagedClass ||
				ElementType == LanguageElementType.ValueClass ||
				ElementType == LanguageElementType.Struct ||
				ElementType == LanguageElementType.ManagedStruct ||
				ElementType == LanguageElementType.ValueStruct ||
		ElementType == LanguageElementType.Union ||
				ElementType == LanguageElementType.Namespace);
		}
		#endregion
		#region GetLastIdentifierInList
		protected virtual LanguageElement GetLastIdentifierInList(NodeList list)
		{
			if (list != null)
				for (int i = list.Count - 1; i >= 0; i--)
				{
					LanguageElement lLastIdentifier = (LanguageElement)list[i];
					if (lLastIdentifier != null && lLastIdentifier.DeclaresIdentifier)
						return lLastIdentifier;
				}
			return null;
		}
		#endregion
		#region GetLastDetailIdentifier
		protected virtual LanguageElement GetLastDetailIdentifier()
		{
			return GetLastIdentifierInList(DetailNodes);
		}
		#endregion
		#region GetLastIdentifier
		protected virtual LanguageElement GetLastIdentifier()
		{
			return GetLastIdentifierInList(Nodes);
		}
		#endregion
		#region AddNodeToNodeList
		protected virtual void AddNodeToNodeList(NodeList aList, LanguageElement aLanguageElement)
		{
	  if (aList == null || aLanguageElement == null)
				return;
	  if (NodeIsAlreadyAdded(this, aList, aLanguageElement))
		return;
			aLanguageElement._Index = aList.Count;
			aList.Add(aLanguageElement);
			aLanguageElement._IsDetailNode = aList == InnerDetailNodes;
			aLanguageElement.SetParent(this);
		}
		#endregion
		#region AddNodesToNodeList
		protected virtual void AddNodesToNodeList(NodeList aList, LanguageElementCollectionBase nodes)
		{
			if (aList == null)
				return;
			foreach(object node in nodes)
				if(node is LanguageElement) 
					AddNodeToNodeList(aList, (LanguageElement)node);
		}
		#endregion
		#region RemoveNodeFromNodeList
		protected virtual void RemoveNodeFromNodeList(NodeList list, LanguageElement element)
		{
			if (list == null || element == null)
				return;
			int lIndex = element._Index;
			if (lIndex < 0 || lIndex >= list.Count)
				return;
			element._Index = -1;
			element.SetParent(null);
			if (list.Count > 0)
				list.Remove(element);
			DecreaseNodesIndices(list, lIndex);
		}
		#endregion
		#region InsertNodeToNodeList
		protected virtual void InsertNodeToNodeList(NodeList list, int index, LanguageElement node)
		{
			if (list == null || index < 0 || index > list.Count)
				return;			
			if (InsertNodeToNodeListWithoutReindex(list, index, node))
			IncreaseNodesIndices(list, index + 1);
		}
		#endregion
		protected virtual void InsertNodesToNodeList(NodeList list, int index, LanguageElementCollectionBase nodes)
		{
			if (list == null || 
				index < 0 || index > list.Count || 
				nodes == null || nodes.Count == 0)
				return;
			for (int i = 0; i < nodes.Count; i++)			
			{
		LanguageElement node = nodes[i] as LanguageElement;
		if (node == null)
					continue;				
		if (InsertNodeToNodeListWithoutReindex(list, index, node))
		{
				IncreaseNodesIndices(list, index + 1);
				index++;
			}
		}
		}
		#region FindDeclarationInList
		protected virtual LanguageElement FindDeclarationInList(NodeList list, string firstIdentifier, string remainingIdentifiers, LanguageElement viewer)
		{
			LanguageElement lDeclaration = null;
			if (list != null)
			{
				for (int i = list.Count - 1; i >= 0; i--)
				{
					LanguageElement lChildNode = (LanguageElement)list[i];
					lDeclaration = lChildNode.FindDeclaration(firstIdentifier, remainingIdentifiers, viewer);
					if (lDeclaration != null)
						break;
				}
			}
			return lDeclaration;
		}
		#endregion
		#region FindChildDeclaration
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual LanguageElement FindChildDeclaration(string firstIdentifier, string remainingIdentifiers, LanguageElement viewer)
		{
			return FindDeclarationInList(Nodes, firstIdentifier, remainingIdentifiers, viewer);
		}
		#endregion
		#region FindDetailDeclaration
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual LanguageElement FindDetailDeclaration(string firstIdentifier, string remainingIdentifiers, LanguageElement viewer)
		{
			return FindDeclarationInList(DetailNodes, firstIdentifier, remainingIdentifiers, viewer);
		}
		#endregion
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected int GetNodeIndexAfter(NodeList list, int line, int offset)
	{
	  int lIndex = GetNodeIndexAfterInternal(list, line, offset);
	  if (lIndex < 0)
	  {
		lIndex = ~lIndex;
	  }
	  return lIndex;
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected int GetNodeIndexBefore(NodeList list, int line, int offset)
	{
	  int lIndex = GetNodeIndexBeforeInternal(list, line, offset);
	  if (lIndex < 0)
	  {
		lIndex = ~lIndex;
		lIndex--;
	  }
	  return lIndex;
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected LanguageElement GetNodeAfter(NodeList list, int line, int offset)
		{
	  int lIndex = GetNodeIndexAfter(list, line, offset);
			if (lIndex < 0 || lIndex >= list.Count)
				return null;
			return (LanguageElement)list[lIndex];
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected LanguageElement GetNodeBefore(NodeList list, int line, int offset)
		{
	  int lIndex = GetNodeIndexBefore(list, line, offset);
			if (lIndex < 0 || lIndex >= list.Count)
				return null;
			return (LanguageElement)list[lIndex];
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual string GetSignaturePart()
		{
	  return StructuralParserServicesHolder.GetSignaturePart(this);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual void ReplaceOwnedReference(LanguageElement oldElement, LanguageElement newElement)
		{
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual int NumberOfTrueStatements(ParserBase parser, ParentToSingleStatement Stmt)
		{
			int val = 0;
			for(int i = 0; i < parser.Context.NodeCount; i++)
				if (!((LanguageElement)parser.Context.Nodes[i]).CompletesPrevious)
					val++;
			return val;
		}
	protected override void CloneRegions(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null || !options.CloneRegions || options.CloningChildren)
		return;
	  LanguageElement leSource = source as LanguageElement;
	  if (leSource == null || leSource.FileNode == null)
		return;
	  InjectRegions(leSource.FileNode.RegionRootNode);
	}
		#region CloneDataFrom
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{
			if (source == null)
				return;
			if (!(source is LanguageElement))
				return;
	  LanguageElement sourceElement = (LanguageElement)source;
			if (options.PerformParseBeforeClone)
				if (sourceElement != null && sourceElement.HasUnparsedCode)
					sourceElement.ParseOnDemandIfNeeded();
	  base.CloneDataFrom(source, options);
	  _Parent = null;
			_Index = -1;
			_IsDetailNode = sourceElement._IsDetailNode;
			MacroCall = sourceElement.MacroCall;
			_HasErrors = sourceElement._HasErrors;
			SetNodeLinks(DetailNodes);
			SetNodeLinks(Nodes);
	  _IsRemoved = sourceElement._IsRemoved;
	  _IsFakeNode = sourceElement.IsFakeNode;
	  if (sourceElement._Tokens == null)
		_Tokens = null;
	  else
		_Tokens = sourceElement._Tokens.Clone(options);
		}
		#endregion
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected virtual void FillFormattingCollection(FormattingParsingElementCollection data)
	{
	  if(_Tokens == null)
		_Tokens = data.GetTokens(Range);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public FormattingElements GetFormattingElements(FormattingTokenType type)
	{
	  return GetFormattingElements(type, 0);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public FormattingElements GetFormattingElements(FormattingTokenType type, int index)
	{
	  if (_Tokens == null)
		return null;
	  return _Tokens.Get(type, index);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public void FillFormattingCollectionWithChildren(FormattingParsingElementCollection data)
	{
	  foreach (LanguageElement element in DetailNodes)
		element.FillFormattingCollectionWithChildren(data);
	  foreach (LanguageElement element in Nodes)
		element.FillFormattingCollectionWithChildren(data);
	  FillFormattingCollection(data);
	}
		#region AddHeader(string header)
		public void AddHeader(string header)
		{
			AddHeaderFooter(header, String.Empty, false );
		}
		#endregion
		#region AddHeader(string[] header)
		public void AddHeader(string[] header)
		{
			AddHeaderFooter(header, new string[0], false );
		}
		#endregion
		#region AddHeaderFooter(string header, string footer)
		public void AddHeaderFooter(string header, string footer)
		{
			bool lSelectText = false;
			AddHeaderFooter(header, footer, lSelectText);
		}
		#endregion
		#region AddHeaderFooter(string header, string footer, bool selectText)
		public void AddHeaderFooter(string header, string footer, bool selectText)
		{
			IDocument lDocument = Document;
			if (lDocument == null)
				return;
			string[] lHeader;
			if (header == null || header == "")
				lHeader = new string[0];
			else 
			{
				lHeader = new string[1];
				lHeader[0] = header;
			}
			string[] lFooter;
			if (footer == null || footer == "")
				lFooter = new string[0];
			else 
			{
				lFooter = new string[1];
				lFooter[0] = footer;
			}
			lDocument.AddHeaderFooter(this, lHeader, lFooter, selectText);
		}
		#endregion
		#region AddHeaderFooter(string[] header, string[] footer)
		public void AddHeaderFooter(string[] header, string[] footer)
		{
			bool lSelectText = false;
			AddHeaderFooter(header, footer, lSelectText);
		}
		#endregion
		#region AddHeaderFooter(string[] header, string[] footer, bool selectText)
		public void AddHeaderFooter(string[] header, string[] footer, bool selectText)
		{
			IDocument lDocument = Document;
			if (lDocument == null)
				return;
			lDocument.AddHeaderFooter(this, header, footer, selectText);
		}
		#endregion
		#region AddFooter(string footer)
		public void AddFooter(string footer)
		{
			AddHeaderFooter(String.Empty, footer, false );
		}
		#endregion
		#region AddFooter(string[] footer)
		public void AddFooter(string[] footer)
		{
			AddHeaderFooter(new string[0], footer, false );
		}
		#endregion
		#region ClosestParent
		public LanguageElement ClosestParent(LanguageElement element1, LanguageElement element2)
		{
			if (element1 == null)
				if (element2 == null)
					return null;
				else if (element2.Parents(this))
					return element2;
				else
					return null;
			else 
			{
				if (element2 == null)
					if (element1.Parents(this))
						return element1;
					else
						return null;
				else		
				{
					if (element1.Parents(this))
						if (element2.Parents(this))		
							if (element2.Parents(element1))		
								return element1;
							else		
								return element2;
						else		
							return element1;
					else 
						if (element2.Parents(this))		
						return element2;
					else
						return null;
				}
			}
		}
		#endregion
		#region CleanUpOwnedReferences
		public override void CleanUpOwnedReferences()
		{
			_Parent = null;
			CleanUpPostponedData();
			base.CleanUpOwnedReferences();
		}
		#endregion
		#region ContainsSelection
		public bool ContainsSelection()
		{
			IDocument lTextDocument = Document;
			if (lTextDocument == null)
				return false;
			int lStartLine;
			int lStartOffset;
			int lEndLine;
			int lEndOffset;
			lTextDocument.GetSelectionBounds(out lStartLine, out lStartOffset, out lEndLine, out lEndOffset);
			return Contains(lStartLine, lStartOffset) && Contains(lEndLine, lEndOffset);
		}
		#endregion
		#region Old Drawing Code
		#endregion
		#region GetChildCyclomaticComplexity
		public virtual int GetChildCyclomaticComplexity()
		{
			int result = 0;
			if (Nodes != null)
				for (int i = 0; i < Nodes.Count; i++)
				{
					LanguageElement childElement = (LanguageElement)Nodes[i];
					if (childElement != null)
						result += childElement.GetCyclomaticComplexity();
				}
			if (DetailNodes != null)
				for (int i = 0; i < DetailNodes.Count; i++)
				{
					LanguageElement detailElement = (LanguageElement)DetailNodes[i];
					if (detailElement != null)
						result += detailElement.GetCyclomaticComplexity();
				}
			return result;
		}
		#endregion
		#region GetDeclaringType
		public Class GetDeclaringType()
		{
			Class lThisClass = GetClass();
			if (lThisClass == null)
			{
				lThisClass = GetStruct();
				if (lThisClass == null)
					lThisClass = GetInterface();
			}
			return lThisClass;
		}
		#endregion
		#region GetParent(ElementType aType)
		public virtual LanguageElement GetParent(LanguageElementType aType)
		{
			LanguageElement lLanguageElement = _Parent;
			while (lLanguageElement != null)
			{
				if (lLanguageElement.ElementType == aType)
					return lLanguageElement;
				lLanguageElement = lLanguageElement._Parent;
			}
			return null;
		}
		#endregion
		#region GetParent(ElementType type, params ElementType[] types)
		public LanguageElement GetParent(LanguageElementType type, params LanguageElementType[] types)
		{
			LanguageElement lLanguageElement = _Parent;
			while (lLanguageElement != null)
			{
				if (lLanguageElement.ElementType == type)
					return lLanguageElement;
				foreach(LanguageElementType lElementType in types)
					if (lLanguageElement.ElementType == lElementType)
						return lLanguageElement;
				lLanguageElement = lLanguageElement._Parent;
			}
			return null;
		}
		#endregion
		#region GetParentClassInterfaceOrStruct
		public LanguageElement GetParentClassInterfaceOrStruct()
		{
			return GetParent(LanguageElementType.Class,LanguageElementType.ManagedClass, LanguageElementType.ValueClass, LanguageElementType.Interface, LanguageElementType.Struct, LanguageElementType.ManagedStruct, LanguageElementType.ValueStruct);
		}
		#endregion
		#region GetParentClassInterfaceStructOrModule
		public LanguageElement GetParentClassInterfaceStructOrModule()
		{
			return GetParent(LanguageElementType.Class, LanguageElementType.Interface, LanguageElementType.Struct, LanguageElementType.Module, LanguageElementType.ManagedClass, LanguageElementType.ManagedStruct, LanguageElementType.ValueClass, LanguageElementType.ValueStruct);
		}
		#endregion		
		#region GetParentTypeDeclaration
		public LanguageElement GetParentTypeDeclaration()
		{
			return GetParent(LanguageElementType.Class,
				LanguageElementType.Interface,
				LanguageElementType.Struct,
				LanguageElementType.Enum,
				LanguageElementType.Delegate,
				LanguageElementType.Module,
				LanguageElementType.ManagedClass,
				LanguageElementType.ManagedStruct,
				LanguageElementType.InterfaceClass,
				LanguageElementType.ValueClass,
				LanguageElementType.ValueStruct,
		LanguageElementType.Union);
		}
		#endregion		
		#region GetParentingStatementParent()
		public virtual LanguageElement GetParentingStatementParent()
		{
			LanguageElement lParent = this;
			if (lParent is ParentingStatement || lParent is Method || lParent is Property || lParent is Event || lParent is Accessor)
				return lParent;
			do
			{
				lParent = lParent.Parent;
				if (lParent == null || lParent is SourceFile)
					return null;
			} while (!(lParent is ParentingStatement || lParent is Method || lParent is Property || lParent is Event || lParent is Accessor));
			return lParent;
		}
		#endregion
		#region GetMethod
		public Method GetMethod()
		{
			if (ElementType == LanguageElementType.Method)
				return this as Method;
			else 
				return GetParentMethod();
		}
		#endregion
		#region GetElementThatCanBeDocumented
		public LanguageElement GetElementThatCanBeDocumented()
		{
			if (CanBeDocumented)
				return this;
			else 
				return GetParentElementThatCanBeDocumented();
		}
		#endregion
		#region GetProperty
		public Property GetProperty()
		{
			if (ElementType == LanguageElementType.Property)
				return this as Property;
			else 
				return GetParentProperty();
		}
		#endregion
		#region GetPropertyAccessor
		public PropertyAccessor GetPropertyAccessor()
		{
			if (ElementType == LanguageElementType.PropertyAccessorGet)
				return this as Get;
			else if (ElementType == LanguageElementType.PropertyAccessorSet)
				return this as Set;
			else 
				return GetParentPropertyAccessor();
		}
		#endregion
		#region GetEventAccessor
		public EventAccessor GetEventAccessor()
		{
			if (ElementType == LanguageElementType.EventAdd)
				return this as EventAdd;
			else if (ElementType == LanguageElementType.EventRemove)
				return this as EventRemove;
			else if (ElementType == LanguageElementType.EventRaise)
				return this as EventRaise;
			else 
				return GetParentEventAccessor();
		}
		#endregion
		#region GetClass
		public Class GetClass()
		{
			if (ElementType == LanguageElementType.Class ||
					ElementType == LanguageElementType.ManagedClass ||
					ElementType == LanguageElementType.ValueClass)
				return this as Class;
			else 
				return GetParentClass();
		}
		#endregion
		#region GetValueClass
		public ValueClass GetValueClass()
		{
			if (ElementType == LanguageElementType.ValueClass)
				return this as ValueClass;
			else 
				return GetValueClass();
		}
		#endregion
		#region GetManagedClass
		public ManagedClass GetManagedClass()
		{
			if (ElementType == LanguageElementType.ManagedClass)
				return this as ManagedClass;
			else 
				return GetParentManagedClass();
		}
		#endregion
		#region GetValueStruct
		public ValueStruct GetValueStruct()
		{
			if (ElementType == LanguageElementType.ValueStruct)
				return this as ValueStruct;
			else 
				return GetParentValueStruct();
		}
		#endregion
		#region GetManagedStruct
		public ManagedStruct GetManagedStruct()
		{
			if (ElementType == LanguageElementType.ManagedStruct)
				return this as ManagedStruct;
			else 
				return GetParentManagedStruct();
		}
		#endregion
		#region GetInterfaceClass
		public InterfaceClass GetInterfaceClass()
		{
			if (ElementType == LanguageElementType.InterfaceClass)
				return this as InterfaceClass;
			else 
				return GetParentInterfaceClass();
		}
		#endregion
		#region GetInterfaceStruct
		public InterfaceStruct GetInterfaceStruct()
		{
			if (ElementType == LanguageElementType.InterfaceStruct)
				return this as InterfaceStruct;
			else 
				return GetParentInterfaceStruct();
		}
		#endregion
		#region GetInterface
		public Interface GetInterface()
		{
			if (ElementType == LanguageElementType.Interface)
				return this as Interface;
			else 
				return GetParentInterface();
		}
		#endregion
		#region GetStruct
		public Struct GetStruct()
		{
			if (ElementType == LanguageElementType.Struct ||
				ElementType == LanguageElementType.ManagedStruct ||
				ElementType == LanguageElementType.ValueStruct
				)
				return this as Struct;
			else 
				return GetParentStruct();
		}
		#endregion
		#region GetNamespace
		public Namespace GetNamespace()
		{
			if (ElementType == LanguageElementType.Namespace)
				return this as Namespace;
			else 
				return GetParentNamespace();
		}
		#endregion
		#region GetEvent
		public Event GetEvent()
		{
			if (ElementType == LanguageElementType.Event)
				return this as Event;
			else 
				return GetParentEvent();
		}
		#endregion
		#region GetAnonymousExpression
		public AnonymousMethodExpression GetAnonymousExpression()
		{
			if (ElementType == LanguageElementType.AnonymousMethodExpression || ElementType == LanguageElementType.LambdaExpression || ElementType == LanguageElementType.LambdaFunctionExpression)
				return this as AnonymousMethodExpression;
			else
				return GetParentAnonymousExpression();
		}
		#endregion
		#region GetSourceFile
		public SourceFile GetSourceFile()
		{
			if (ElementType == LanguageElementType.SourceFile)
				return this as SourceFile;
			else 
				return GetParent( LanguageElementType.SourceFile ) as SourceFile;
		}
		#endregion
		#region GetSourceFileSupportLists(out RegionDirective regionDirectives, out TextStringCollection strings, out CompilerDirective compilerDirectives)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void GetSourceFileSupportLists(out RegionDirective regionDirectives, out TextStringCollection strings, out CompilerDirective compilerDirectives)
		{
			SourceFile lSourceFile = null;
			GetSourceFileSupportLists(out regionDirectives, out strings, out compilerDirectives, out lSourceFile);
		}
		#endregion
		#region GetSourceFileSupportLists(out RegionDirective regionDirectives, out TextStringCollection strings, out CompilerDirective compilerDirectives, out SourceFile sourceFile)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void GetSourceFileSupportLists(out RegionDirective regionDirectives, out TextStringCollection strings, out CompilerDirective compilerDirectives, out SourceFile sourceFile)
		{
			regionDirectives = null;
			strings = null;
			compilerDirectives = null;
			sourceFile = null;
			if (this is SourceFile)
				sourceFile = (SourceFile)this;
			else
			{
				VisualStudioDocument lDocument = this.GetParentDocument();
				if (lDocument is SourceFile)
					sourceFile = (SourceFile)lDocument;
			}
			if (sourceFile != null)
			{
				regionDirectives = sourceFile.RegionRootNode;
				strings = sourceFile.TextStrings;
				compilerDirectives = sourceFile.CompilerDirectiveRootNode;
			}
		}
		#endregion
		#region GetTypeName
		public virtual string GetTypeName()
		{
			return String.Empty;
		}
		#endregion
		#region GetParentDocument
		public virtual VisualStudioDocument GetParentDocument()
		{
			if (ElementType == LanguageElementType.SourceFile || ElementType == LanguageElementType.Document)
				return this as VisualStudioDocument;
			LanguageElement lLanguageElement = GetParent(LanguageElementType.SourceFile);
			if (lLanguageElement != null)
				return (VisualStudioDocument)lLanguageElement;
			return GetParent( LanguageElementType.Document ) as VisualStudioDocument;
		}
		#endregion
		#region GetLastCodeChild
		public LanguageElement GetLastCodeChild()
		{
			LanguageElement lElement = LastChild;
			if (lElement == null)
				return null;
			if (lElement is CodeElement)
				return lElement;
			return lElement.GetPreviousCodeElementSibling();
		}
		#endregion
		#region GetPreviousIdentifier(LanguageElement referenceNode)
		public LanguageElement GetPreviousIdentifier(LanguageElement referenceNode)
		{
			if (_Parent != null)
				return _Parent.GetPreviousChildIdentifier(this, referenceNode);
			else
				return null;
		}
		#endregion
		#region GetPreviousIdentifier(LanguageElement referenceNode, int refLine, int refColumn)
		public LanguageElement GetPreviousIdentifier(LanguageElement referenceNode, int refLine, int refColumn)
		{
			if (Contains(refLine, refColumn))
			{
				LanguageElement lLanguageElement = GetLastDetailIdentifier();
				if (lLanguageElement != null)
					return lLanguageElement;
				if (AllChildrenAccessibleByGrandchildren())		
				{
					lLanguageElement = GetLastIdentifier();
					if (lLanguageElement != null)
						return lLanguageElement;
				}
			}
			if (DeclaresIdentifier)
				if (_Parent == null)
					return this;
				else 
				{
					if (_Parent.AllChildrenAccessibleByGrandchildren())
						return GetLastSiblingInScope(referenceNode);
					else
						return this;
				}
			return GetPreviousIdentifier(referenceNode);
		}
		#endregion
		#region GetFullBlockRange
		public SourceRange GetFullBlockRange()
		{
			return ElementRangeHelper.GetFullBlockRange(this);
		}
		#endregion
		#region GetFullBlockRange
		public SourceRange GetFullBlockRange(BlockElements blockElements)
		{
			return ElementRangeHelper.GetFullBlockRange(this, blockElements);
		}
		#endregion
		#region GetFullBlockCoordinates
		public void GetFullBlockCoordinates(out int startLine, out int startOffset, out int endLine, out int endOffset)
		{
			SourceRange range = GetFullBlockRange();
			startLine = range.Start.Line;
			startOffset = range.Start.Offset;
			endLine = range.End.Line;
			endOffset = range.End.Offset;
		}
		#endregion
		#region GetFullBlockCoordinates
		public void GetFullBlockCoordinates(out int startLine, out int startOffset, out int endLine, out int endOffset, BlockElements blockElements)
		{
			SourceRange range = GetFullBlockRange(blockElements);
			startLine = range.Start.Line;
			startOffset = range.Start.Offset;
			endLine = range.End.Line;
			endOffset = range.End.Offset;
		}
		#endregion		
		#region GetFullBlockNodes
		public void GetFullBlockNodes(out LanguageElement startNode, out LanguageElement endNode)
		{
			ElementRangeHelper.GetFullBlockNodes(this, out startNode, out endNode);
		}
		#endregion
		#region GetFullBlockNodes
		public void GetFullBlockNodes(BlockElements blockElements, out LanguageElement startNode, out LanguageElement endNode)
		{
			ElementRangeHelper.GetFullBlockNodes(this, blockElements, out startNode, out endNode);
		}
		#endregion
		#region MoveTo
		public void MoveTo(SourcePoint target, string operation)
		{
			Document.Move(GetCutRange(), target, operation);
		}
		#endregion
		#region ReplaceWith
		public void ReplaceWith(string newCode, string operation)
		{
	  ReplaceWith(newCode, operation, false );
		}
		#endregion
	#region ReplaceWith
	public void ReplaceWith(string newCode, string operation, bool format)
	{
	  Document.Replace(GetCutRange(), newCode, operation, format);
	}
	#endregion
		#region FullBlockMoveTo
		public void FullBlockMoveTo(SourcePoint target, string operation)
		{
			Document.Move(GetFullBlockCutRange(), target, operation);
		}
		#endregion
		#region FullBlockMoveTo(SourcePoint target, string operation, BlockElements blockElements)
		public void FullBlockMoveTo(SourcePoint target, string operation, BlockElements blockElements)
		{
			Document.Move(GetFullBlockCutRange(blockElements), target, operation);
		}
		#endregion
	public void FullBlockMoveWithBinding(SourcePoint target, string operation)
	{
	  Document.MoveWithBinding(GetFullBlockCutRange(), target, operation);
	}
		public void FullBlockMoveWithBinding(SourcePoint target, string operation, BlockElements blockElements)
		{
			Document.MoveWithBinding(GetFullBlockCutRange(blockElements), target, operation);
		}
		#region FullBlockReplaceWith
		public void FullBlockReplaceWith(string newCode, string operation)
		{
			Document.Replace(GetFullBlockCutRange(), newCode, operation);
		}
		#endregion
		#region FullBlockReplaceWith(string newCode, string operation, BlockElements blockElements)
		public void FullBlockReplaceWith(string newCode, string operation, BlockElements blockElements)
		{
			Document.Replace(GetFullBlockCutRange(blockElements), newCode, operation);
		}
		#endregion
		#region GetCutRange
		public SourceRange GetCutRange()
		{
			SourceRange lReturnRange = Range;
			IDocument lTextDocument = Document;
			if (lTextDocument != null)
				lReturnRange = lTextDocument.IncludeWhitespace(lReturnRange);
			return lReturnRange;
		}
		#endregion
		#region GetFullBlockCutRange
		public SourceRange GetFullBlockCutRange()
		{
			return GetFullBlockCutRange(BlockElements.AllSupportElements | BlockElements.AllLeadingWhiteSpaces | BlockElements.Region | BlockElements.TrailingWhiteSpace);
		}
		#endregion
		#region GetFullBlockCutRange(BlockElements blockElements)
		public SourceRange GetFullBlockCutRange(BlockElements blockElements)
		{
			return GetFullBlockRange(blockElements);
		}
		#endregion
		#region SelectCode(bool ensureVisible)
		public void SelectCode(bool ensureVisible)
		{
			SetSelection(StartLine, StartOffset, EndLine, EndOffset, ensureVisible);
		}
		#endregion
		#region SelectCode()
		public void SelectCode()
		{
			SelectCode(false );
		}
		#endregion
		#region SelectFullBlock(ITextView textView, bool ensureVisible)
		public void SelectFullBlock(IDXCoreTextView textView, bool ensureVisible)
		{
			if (textView == null)
				return;
			int startLine;
			int startOffset;
			int endLine;
			int endOffset;
			GetFullBlockCoordinates(out startLine, out startOffset, out endLine, out endOffset);
			SetSelection(textView, startLine, startOffset, endLine, endOffset, ensureVisible);
		}
		#endregion
		#region SelectFullBlock
		public void SelectFullBlock(IDXCoreTextView textView)
		{
			SelectFullBlock(textView, false );
		}
		#endregion
		#region SelectFullBlock
		public void SelectFullBlock(bool ensureVisible)
		{
			int startLine;
			int startOffset;
			int endLine;
			int endOffset;
			GetFullBlockCoordinates(out startLine, out startOffset, out endLine, out endOffset);
			SetSelection(startLine, startOffset, endLine, endOffset, ensureVisible);
		}
		#endregion
		#region SelectFullBlock
		public void SelectFullBlock()
		{
			SelectFullBlock(false );
		}
		#endregion
		#region GetParentMethod
		public Method GetParentMethod()
		{
			return ( GetParent( LanguageElementType.Method ) as Method );
		}
		#endregion
		#region GetParentElementThatCanBeDocumented
		public LanguageElement GetParentElementThatCanBeDocumented()
		{
			LanguageElement lLanguageElement = Parent;
			while (lLanguageElement != null && !lLanguageElement.CanBeDocumented)
			{
				lLanguageElement = lLanguageElement.Parent;
			}
			return lLanguageElement;
		}
		#endregion
		#region GetParentProperty
		public Property GetParentProperty()
		{
			return ( GetParent( LanguageElementType.Property ) as Property );
		}
		#endregion
		#region GetParentPropertyAccessor
		public PropertyAccessor GetParentPropertyAccessor()
		{
			Get lGet = GetParent( LanguageElementType.PropertyAccessorGet ) as Get;
			if (lGet != null)
				return lGet;
			Set lSet = GetParent( LanguageElementType.PropertyAccessorSet ) as Set;
			if (lSet != null)
				return lSet;
			return null;
		}
		#endregion
		#region GetParentEventAccessor
		public EventAccessor GetParentEventAccessor()
		{
			EventAdd lEventAdd = GetParent(LanguageElementType.EventAdd) as EventAdd;
			if (lEventAdd != null)
				return lEventAdd;
			EventRemove lEventRemove = GetParent(LanguageElementType.EventRemove) as EventRemove;
			if (lEventRemove != null)
				return lEventRemove;
			EventRaise lEventRaise = GetParent(LanguageElementType.EventRaise) as EventRaise;
			if (lEventRaise != null)
				return lEventRaise;
			return null;
		}
		#endregion
		#region GetParentClass
		public Class GetParentClass()
		{
			return GetParent(LanguageElementType.Class, LanguageElementType.ManagedClass, LanguageElementType.ValueClass) as Class;
		}
		#endregion
		#region GetParentManagedClass
		public ManagedClass GetParentManagedClass()
		{
			return ( GetParent( LanguageElementType.ManagedClass ) as ManagedClass );
		}
		#endregion
		#region GetParentManagedStruct
		public ManagedStruct GetParentManagedStruct()
		{
			return ( GetParent( LanguageElementType.ManagedStruct) as ManagedStruct );
		}
		#endregion
		#region GetParentInterfaceClass
		public InterfaceClass GetParentInterfaceClass()
		{
			return ( GetParent( LanguageElementType.InterfaceClass) as InterfaceClass);
		}
		#endregion
		#region GetParentInterfaceStruct
		public InterfaceStruct GetParentInterfaceStruct()
		{
			return ( GetParent( LanguageElementType.InterfaceStruct) as InterfaceStruct);
		}
		#endregion
		#region GetParentValueClass
		public ValueClass GetParentValueClass()
		{
			return ( GetParent( LanguageElementType.ValueClass) as ValueClass);
		}
		#endregion
		#region GetParentValueStruct
		public ValueStruct GetParentValueStruct()
		{
			return ( GetParent( LanguageElementType.ValueStruct) as ValueStruct);
		}
		#endregion
		#region GetParentInterface
		public Interface GetParentInterface()
		{
			return GetParent(LanguageElementType.Interface, LanguageElementType.InterfaceClass, LanguageElementType.InterfaceStruct) as Interface;
		}
		#endregion
		#region GetParentStruct
		public Struct GetParentStruct()
		{
			return GetParent(LanguageElementType.Struct, LanguageElementType.ManagedStruct, LanguageElementType.ValueStruct) as Struct;
		}
		#endregion
		#region GetParentNamespace
		public Namespace GetParentNamespace()
		{
			return (GetParent( LanguageElementType.Namespace ) as Namespace );
		}
		#endregion
		#region GetParentEvent
		public Event GetParentEvent()
		{
			return ( GetParent( LanguageElementType.Event ) as Event );
		}
		#endregion
		#region GetParentAnonymousExpression
		public AnonymousMethodExpression GetParentAnonymousExpression()
		{
			return (GetParent(LanguageElementType.AnonymousMethodExpression, LanguageElementType.LambdaExpression, LanguageElementType.LambdaFunctionExpression) as AnonymousMethodExpression);
		}
		#endregion
		#region GetCodeNodeFromList
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LanguageElement GetCodeNodeFromList(NodeList list)
		{
			if (list == null)
				return null;
			for (int i = 0; i < list.Count; i++)
			{
				LanguageElement element = (LanguageElement)list[i];
				if (element.ElementType != LanguageElementType.Comment && 
					element.ElementType != LanguageElementType.XmlDocComment && 
					!(element is PreprocessorDirective))
					return element;		
			}
			return null;
		}
		#endregion
		#region GetFirstCodeChild
		public LanguageElement GetFirstCodeChild()
		{
			return GetFirstCodeChild(true);
		}
		#endregion
		#region GetFirstCodeChild
		public LanguageElement GetFirstCodeChild(bool useDetailNodes)
		{
	  LanguageElement lElement = null;
			if (useDetailNodes)
				lElement = GetCodeNodeFromList(DetailNodes);
			if (lElement != null)
				return lElement;
			return GetCodeNodeFromList(Nodes);
		}
		#endregion		
		#region GetNextCodeElementSibling
		public CodeElement GetNextCodeElementSibling()
		{
			LanguageElement lNext = NextSibling;
			while (lNext != null && !(lNext is CodeElement))
				lNext = lNext.NextSibling;
			return lNext as CodeElement;
		}
		#endregion
		#region GetPreviousCodeElementSibling
		public CodeElement GetPreviousCodeElementSibling()
		{
			LanguageElement lPrevious = PreviousSibling;
			while (lPrevious != null && !(lPrevious is CodeElement))
				lPrevious = lPrevious.PreviousSibling;
			return lPrevious as CodeElement;
		}
		#endregion
		#region GetParentLoopOrFinallyTarget
		public LanguageElement GetParentLoopOrFinallyTarget()
		{
			LanguageElement lParentLoop = ParentLoop;
			LanguageElement lTarget = null;
			Try lParentTryBlock = ParentTryBlock;
			if (lParentTryBlock != null && ClosestParent(lParentLoop, lParentTryBlock) == lParentTryBlock)
				lTarget = lParentTryBlock.GetFinallyTarget();
			if (lTarget == null)
			{
				lTarget = lParentLoop;
			}
			return lTarget;
		}
		#endregion
		#region GetParentMethodOrProperty
		public LanguageElement GetParentMethodOrProperty()
		{
			LanguageElement lParent = GetMethod();
			if (lParent == null)
				lParent = GetProperty();
			return lParent;
		}
		#endregion
		#region GetParentMethodOrPropertyOrEvent
		public LanguageElement GetParentMethodOrPropertyOrEvent()
		{
			LanguageElement lParent = GetParentMethodOrProperty();
			if (lParent == null)
				lParent = GetEvent();
			return lParent;
		}
		#endregion
		#region GetParentMethodOrPropertyAccessor
		public LanguageElement GetParentMethodOrPropertyAccessor()
		{
			LanguageElement lParentMethodOrPropertyAccessor = GetMethod();
			if (lParentMethodOrPropertyAccessor == null)
				lParentMethodOrPropertyAccessor = GetPropertyAccessor();
			return lParentMethodOrPropertyAccessor;
		}
		#endregion
		public LanguageElement GetParentMethodOrAccessor()
		{
			LanguageElement lElement = GetParentMethodOrPropertyAccessor();
			if (lElement == null)
				lElement = GetParentMethodOrEventAccessor();
			return lElement;
		}
		public LanguageElement GetParentMethodOrAccessor(bool checkAnonymousMethod)
		{
			LanguageElement parentMethod = GetParentMethodOrAccessor();
			if (!checkAnonymousMethod)
				return parentMethod;
			LanguageElement lambda = GetAnonymousMethodOrLambda();
			if (lambda == null)
				return parentMethod;
			if (parentMethod == null || lambda.IsParentedBy(parentMethod))
				return lambda;
			return parentMethod;
		}
		public LanguageElement GetAnonymousMethodOrLambda()
		{
			if (ElementType == LanguageElementType.AnonymousMethodExpression || 
				ElementType == LanguageElementType.AnonymousConstructorExpression ||
				ElementType == LanguageElementType.LambdaExpression)
				return this;
			return GetParent(LanguageElementType.AnonymousMethodExpression,
				LanguageElementType.AnonymousConstructorExpression,
				LanguageElementType.LambdaExpression);
		}
		#region GetParentMethodOrEventAccessor
		public LanguageElement GetParentMethodOrEventAccessor()
		{
			LanguageElement lParentMethodOrEventAccessor = GetMethod();
			if (lParentMethodOrEventAccessor == null)
				lParentMethodOrEventAccessor = GetEventAccessor();
			return lParentMethodOrEventAccessor;
		}
		#endregion
		public LanguageElement GetParentCodeBlock()
		{
			LanguageElement lParent = GetParentMethodOrPropertyAccessor();
			if (lParent == null)
				lParent = GetParentEventAccessor();
	  if (lParent == null)
				lParent = GetParent(LanguageElementType.Delegate);
	  if (lParent == null)
		lParent = GetParent(LanguageElementType.AnonymousMethodExpression);
			if (lParent == null)
				lParent = GetParent(LanguageElementType.AnonymousConstructorExpression);
	  if (lParent == null)
		lParent = GetParent(LanguageElementType.LambdaExpression);
	  if (lParent == null)
		lParent = GetParent(LanguageElementType.LambdaFunctionExpression);
			if (lParent == null)
				lParent = GetParentProperty();
			return lParent;
		}
		#region GetParentStatementOrVariable
		public LanguageElement GetParentStatementOrVariable()
		{
			if (this is Statement || this is Variable)
				return this;
			LanguageElement lParent = this;
			while (lParent != null && !(lParent is Statement || lParent is Variable))
				lParent = lParent.Parent;
			return lParent;
		}
		#endregion
		#region GetStartString
		public string GetStartString()
		{
			if (Document == null)
				return string.Empty;
			return Document.GetText(StartLine, 1, StartLine, StartOffset);
		}
		#endregion		
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IElement GetFromLiteModel()
		{
	  return StructuralParserServicesHolder.FindElementInSnapshotStructure(this);
		}
		#region InsideSelection
		public bool InsideSelection()
		{
			IDocument lTextDocument = Document;
			if (lTextDocument == null)
				return false;
			int lStartLine;
			int lStartOffset;
			int lEndLine;
			int lEndOffset;
			lTextDocument.GetSelectionBounds(out lStartLine, out lStartOffset, out lEndLine, out lEndOffset);
			return StartsAfter(lStartLine, lStartOffset - 1) && EndsBefore(lEndLine, lEndOffset + 1);
		}
		#endregion
		#region IsSibling
		public bool IsSibling(LanguageElement aLanguageElement)
		{
			return (aLanguageElement != null && OwningList != null && aLanguageElement.OwningList == OwningList);
		}
		#endregion
		#region IsDeeperThan
		public bool IsDeeperThan(LanguageElement aLanguageElement)
		{
			if (aLanguageElement == null)
				return false;
			return LevelsDeep > aLanguageElement.LevelsDeep;
		}
		#endregion
		#region SameEndPoint
		public bool SameEndPoint(LanguageElement aLanguageElement)
		{
			if (aLanguageElement == null)
				return false;
			return EndLine == aLanguageElement.EndLine && EndOffset == aLanguageElement.EndOffset;
		}
		#endregion
		#region SameStartPoint
		public bool SameStartPoint(LanguageElement aLanguageElement)
		{
			if (aLanguageElement == null)
				return false;
			return StartLine == aLanguageElement.StartLine && StartOffset == aLanguageElement.StartOffset;
		}
		#endregion
		#region AddNode
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void AddNode(LanguageElement element)
		{
			if (element == null)
				return;
			PrepareNodeList();
			AddNodeToNodeList(InnerNodes, element);
		}
		#endregion
		#region AddNodes
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void AddNodes(LanguageElementCollectionBase nodes)
		{
			if (nodes == null)
				return;
			PrepareNodeList();
			AddNodesToNodeList(InnerNodes, nodes);
		}
		#endregion
		#region InsertNode
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void InsertNode(int index, LanguageElement element)
		{
			if (element == null)
				return;
			PrepareNodeList();
			InsertNodeToNodeList(InnerNodes, index, element);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void InsertNodes(int index, LanguageElementCollectionBase nodes)
		{
			if (nodes == null || nodes.Count == 0)
				return;
			PrepareNodeList();			
			InsertNodesToNodeList(InnerNodes, index, nodes);
		}
		#region RemoveNode
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void RemoveNode(LanguageElement element)
		{
			if (element != null)
			{
				RemoveNodeFromNodeList(InnerNodes, element);
				ReplaceOwnedReference(element, null);
			}
		}
		#endregion
	[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal void RemoveNodesBetweenIndexes(int startIndex, int endIndex)
		{
	  if (InnerNodes == null || startIndex < 0 || endIndex < 0 || startIndex >= InnerNodes.Count || endIndex >= InnerNodes.Count)
		return;
	  for(int i = startIndex; i <= endIndex; i++)
	  {
		LanguageElement element = InnerNodes[startIndex] as LanguageElement;
		element._Index = -1;
		element.SetParent(null);
		InnerNodes.RemoveAt(startIndex);
		ReplaceOwnedReference(element, null);
	  }
	  int countDeleted = endIndex - startIndex + 1;
	  for(int i = startIndex + 1; i <= InnerNodeCount; i++)
	  {
		LanguageElement element = InnerNodes[i] as LanguageElement;
		if(element != null)
		  element._Index -= countDeleted;
	  }
		}
	public void RemoveFromParent()
	{
	  LanguageElement parent = Parent;
	  if (parent == null)
		return;
	  if (IsDetailNode)
		parent.RemoveDetailNode(this);
	  else
		parent.RemoveNode(this);
	}
	#region RemoveNodesFromIndex
	public virtual void RemoveNodesFromIndex(int index)
	{
	  if (index < 0 || index >= InnerNodes.Count)
		return;
	  for (int i = index; i < InnerNodes.Count; i++)
	  {
		LanguageElement element = InnerNodes[i] as LanguageElement;
		ReplaceOwnedReference(element, null);
		element._Index = -1;
		element.SetParent(null);
	  }
	  InnerNodes.RemoveFrom(index);
	}
	#endregion
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void RemoveNodes(LanguageElementCollectionBase nodes)
		{
			RemoveNodes(nodes as NodeList);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void RemoveNodes(NodeList nodes)
		{
			if (nodes == null || nodes.Count == 0)
				return;
	  for (int i = nodes.Count - 1; i >= 0; i--)
				RemoveNode(nodes[i] as LanguageElement);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void RemoveAllNodes()
		{
			if (Nodes == null)
				return;
			object[] nodes = InnerNodes.ToArray();
			for (int i = 0; i < nodes.Length; i++)
				RemoveNode(nodes[i] as LanguageElement);
		}
		#region ReplaceNode
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void ReplaceNode(LanguageElement oldElement, LanguageElement newElement)
		{			
			if (oldElement == null && newElement == null)
				return;
			if (oldElement == null)
			{
				AddNode(newElement);
				return;
			}
			else if(newElement == null)
			{
				if (InnerNodes.Contains(oldElement))
					RemoveNode(oldElement);
				return;
			}
			if (!InnerNodes.Contains(oldElement))
				return;
			int lIndex = oldElement._Index;
			RemoveNodeFromNodeList(InnerNodes, oldElement);
			InsertNodeToNodeList(InnerNodes, lIndex, newElement);
			ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void ReplaceNodes(LanguageElementCollectionBase oldNodes, LanguageElementCollectionBase newNodes)
		{
			int lOldIdx = -1;
			if (oldNodes != null && oldNodes.Count > 0)
				lOldIdx = Nodes.IndexOf(oldNodes[0]);
			RemoveNodes(oldNodes);
			if (newNodes == null || newNodes.Count == 0)
				return;
			if (lOldIdx < 0)
				AddNodes(newNodes);
			else
				InsertNodes(lOldIdx, newNodes);			
		}
		#region AddDetailNode
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void AddDetailNode(LanguageElement element)
		{
			if (element == null)
				return;
			PrepareDetailNodeList();
			AddNodeToNodeList(InnerDetailNodes, element);
		}
		#endregion
		#region AddDetailNodes
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void AddDetailNodes(LanguageElementCollectionBase nodes)
		{
			if (nodes == null)
				return;
			PrepareDetailNodeList();
			AddNodesToNodeList(InnerDetailNodes, nodes);
		}
		#endregion
		#region InsertDetailNode
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void InsertDetailNode(int index, LanguageElement element)
		{
			if (element == null)
				return;
			PrepareDetailNodeList();
			InsertNodeToNodeList(InnerDetailNodes, index, element);
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void InsertDetailNodes(int index, LanguageElementCollectionBase nodes)
		{
			if (nodes == null || nodes.Count == 0)
				return;
			PrepareDetailNodeList();			
			InsertNodesToNodeList(InnerDetailNodes, index, nodes);
		}
		#region RemoveDetailNode
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void RemoveDetailNode(LanguageElement element)
		{
			if (element != null)
			{
				RemoveNodeFromNodeList(InnerDetailNodes, element);
				ReplaceOwnedReference(element, null);
			}
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void RemoveDetailNodes(LanguageElementCollectionBase nodes)
		{
			if (nodes == null || nodes.Count == 0)
				return;
			for (int i = nodes.Count - 1; i >=0; i--)
				RemoveDetailNode(nodes[i] as LanguageElement);
		}
		#region ReplaceDetailNode
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public virtual void ReplaceDetailNode(LanguageElement oldElement, LanguageElement newElement)
		{
			if (oldElement == null && newElement == null)
				return;
			if (oldElement == null)
			{
				AddDetailNode(newElement);
				return;
			}
			else if(newElement == null)
			{
				if (InnerDetailNodes.Contains(oldElement))
					RemoveDetailNode(oldElement);
				return;
			}			
			if (!InnerDetailNodes.Contains(oldElement))
				return;
			int lIndex = oldElement._Index;
			RemoveNodeFromNodeList(InnerDetailNodes, oldElement);
			InsertNodeToNodeList(InnerDetailNodes, lIndex, newElement);
			ReplaceOwnedReference(oldElement, newElement);
		}
		#endregion
	public virtual void ReplaceDetailNode(LanguageElement oldElement, LanguageElementCollectionBase newElements)
	{
	  if (oldElement == null && newElements == null)
		return;
	  if (oldElement == null)
	  {
		AddDetailNodes(newElements);
		return;
	  }
	  else if (newElements == null || newElements.Count == 0)
	  {
		if (InnerDetailNodes.Contains(oldElement))
		  RemoveDetailNode(oldElement);
		return;
	  }
	  if (!InnerDetailNodes.Contains(oldElement))
		return;
	  int idx = oldElement._Index;
	  RemoveDetailNode(oldElement);
	  InsertDetailNodes(idx, newElements);
	}
	public virtual void ReplaceNode(LanguageElement oldElement, LanguageElementCollectionBase newElements)
	{			
	  if (oldElement == null && newElements == null)
		return;
	  if (oldElement == null)
	  {
		AddNodes(newElements);
		return;
	  }
	  else if (newElements == null || newElements.Count == 0)
	  {
		if (InnerNodes.Contains(oldElement))
		  RemoveNode(oldElement);
		return;
	  }
	  if (!InnerNodes.Contains(oldElement))
		return;
	  int idx = oldElement._Index;
	  RemoveNode(oldElement);
	  InsertNodes(idx, newElements);
	}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void ReplaceDetailNodes(LanguageElementCollectionBase oldNodes, LanguageElementCollectionBase newNodes)
		{
			int lOldIdx = -1;
			if (oldNodes != null && oldNodes.Count > 0)
				lOldIdx = DetailNodes.IndexOf(oldNodes[0]);
			RemoveDetailNodes(oldNodes);
			if (newNodes == null || newNodes.Count == 0)
				return;
			if (lOldIdx < 0)
				AddDetailNodes(newNodes);
			else
				InsertDetailNodes(lOldIdx, newNodes);			
		}
	public static void RemoveChildElement(LanguageElement element)
	{
	  ReplaceChildElement(element, null);
	}
	public static void ReplaceChildElement(LanguageElement oldElement, LanguageElement newElement)
	{
	  if (oldElement == null || oldElement == newElement)
		return;
	  LanguageElement parent = oldElement.Parent;
	  if (parent == null)
		return;
	  if (oldElement.IsDetailNode)
	  {
		if (newElement is ElementList)
		  parent.ReplaceDetailNode(oldElement, LanguageElementCollection.FromNodeList(newElement.Nodes));
		else
		  parent.ReplaceDetailNode(oldElement, newElement);
	  }
	  else
	  {
		if (newElement is ElementList)
		  parent.ReplaceNode(oldElement, LanguageElementCollection.FromNodeList(newElement.Nodes));
		else
		  parent.ReplaceNode(oldElement, newElement);
	  }
	}
	public static void MoveChildElementBefore(LanguageElement child, LanguageElement targetElement)
	{
	  if (child == null || targetElement == null || targetElement.Index < 0)
		return; 
	  LanguageElement targetParent = targetElement.Parent;
	  if (targetParent == null)
		return;
	  RemoveChildElement(child);
	  targetParent.InsertNode(targetElement.Index, child);
	}
		#region Collapse(bool undoable)
	[Obsolete("Use CollapseInView")]
		public void Collapse(bool undoable)
		{
			ICollapsibleRegion lRegion = GetCollapsibleRegion();
			if (lRegion == null)
				return;
			lRegion.Collapse(undoable);
		}
		#endregion
		#region Collapse
	[Obsolete("Use CollapseInView")]
		public void Collapse()
		{
			Collapse(true);
		}
		#endregion
		#region Expand(bool undoable)
	[Obsolete("Use ExpandInView")]
		public void Expand(bool undoable)
		{
			ICollapsibleRegion lRegion = GetCollapsibleRegion();
			if (lRegion == null)
				return;
			lRegion.Expand(undoable);
		}
		#endregion
		#region Expand
	[Obsolete("Use ExpandInView")]
		public void Expand()
		{
			Expand(true);
		}
		#endregion		
		#region InSameProject
		public bool InSameProject(LanguageElement element)
		{
			if (element == null)
				return false;
			return Project == element.Project;
		}
		#endregion
		#region IsRelatedTo
		public virtual bool IsRelatedTo(LanguageElement element)
		{
			return IsParentedBy(element) || Parents(element);
		}
		#endregion
		#region IsVisibleFrom
		public virtual bool IsVisibleFrom(LanguageElement viewer)
		{
			if (viewer == null)		
				return true;
			else if (IsRelatedTo(viewer))
				return true;		
			else
				return false;
		}
		#endregion
		#region IsIdentifier
		public virtual bool IsIdentifier(string identifier, LanguageElement viewer)
		{
			if (!IdentifiersMatch(Name, identifier))
				return false;
			return IsVisibleFrom(viewer);
		}
		#endregion
	#region IsIdentifier
	public virtual bool IdentifiersMatch(string first, string second)
	{
	  return StructuralParserServicesHolder.IdentifiersMatch(first, second);
	}
	#endregion
	public string ExtractFirstIdentifier(ref string remainingIdentifiers)
	{
	  return StructuralParserServicesHolder.ExtractFirstIdentifier(ref remainingIdentifiers);
	}
		#region FindDeclaration(string firstIdentifier, string remainingIdentifiers, LanguageElement viewer)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LanguageElement FindDeclaration(string firstIdentifier, string remainingIdentifiers, LanguageElement viewer)
		{
			if (!IsIdentifier(firstIdentifier, viewer))
				return null;
			if (remainingIdentifiers == String.Empty)
				return this;
			firstIdentifier = ExtractFirstIdentifier(ref remainingIdentifiers);
			LanguageElement languageElement = FindDetailDeclaration(firstIdentifier, remainingIdentifiers, viewer);
			if (languageElement == null)
				languageElement = FindChildDeclaration(firstIdentifier, remainingIdentifiers, viewer);
			return languageElement;
		}
		#endregion
		#region FindDeclaration(string identifier, LanguageElement viewer)
		public LanguageElement FindDeclaration(string identifier, LanguageElement viewer)
		{
			string firstIdentifier = ExtractFirstIdentifier(ref identifier);
			return FindDeclaration(firstIdentifier, identifier, viewer);
		}
		#endregion
		#region FindChildByName
		public virtual LanguageElement FindChildByName(string aName)
		{
			return FindChildByName(aName, false);
		}
		#endregion
		#region FindChildByName
		public virtual LanguageElement FindChildByName(string aName, bool lookInDetail)
		{
			LanguageElement lElement;
			LanguageElement lChildElement;
			if (lookInDetail)
			{
				for (int i = 0; i < DetailNodes.Count; i++)
				{
					lElement = (LanguageElement)DetailNodes[i];
					if (lElement.Name == aName)
						return lElement;
					lChildElement = lElement.FindChildByName(aName, true);
					if (lChildElement != null)
						return lChildElement;
				}
			}
			for (int i = 0; i < Nodes.Count; i++)
			{
				lElement = (LanguageElement)Nodes[i];
				if (lElement.Name == aName)
					return lElement;
				lChildElement = lElement.FindChildByName(aName, lookInDetail);
				if (lChildElement != null)
					return lChildElement;
			}
			return null;
		}
		#endregion
		#region FindChildByElementType
		public virtual LanguageElement FindChildByElementType(LanguageElementType type)
		{
			return FindChildByElementType(type, false);
		}
		#endregion
		#region FindChildByElementType
		public virtual LanguageElement FindChildByElementType(LanguageElementType type, bool lookInDetail)
		{
			LanguageElement lElement;
			LanguageElement lChildElement;
			if (lookInDetail) 
			{
				for (int i = 0; i < DetailNodes.Count; i++)
				{
					lElement = (LanguageElement)DetailNodes[i];
					if (lElement.ElementType == type)
						return lElement;
					lChildElement = lElement.FindChildByElementType(type, lookInDetail);
					if (lChildElement != null)
						return lChildElement;
				}
			}
			for (int i = 0; i < Nodes.Count; i++)
			{
				lElement = (LanguageElement)Nodes[i];
				if (lElement.ElementType == type)
					return lElement;
				lChildElement = lElement.FindChildByElementType(type, lookInDetail);
				if (lChildElement != null)
					return lChildElement;
			}
			return null;
		}
		#endregion
		#region GetChildAfter(SourcePoint sourcePoint)
		public LanguageElement GetChildAfter(SourcePoint sourcePoint) 
		{
			return GetChildAfter(sourcePoint.Line, sourcePoint.Offset);
		}
		#endregion
		#region GetChildAfter(int line, int offset)
		public LanguageElement GetChildAfter(int line, int offset)
		{
			LanguageElement lResult = GetNodeAfter(Nodes, line, offset);
			if (lResult != null)
				return lResult;
			return GetNodeAfter(DetailNodes, line, offset);
		}
		#endregion		
		public LanguageElement GetSiblingBefore(int line, int offset)
		{
			if (OwningList == null || _Index < 0)
				return null;
			int lIndex = GetNodeIndexBeforeInternal(OwningList, line, offset);
			if (lIndex < 0)
			{
				lIndex = ~lIndex;
				lIndex --;
			}
			if (lIndex == _Index)
				lIndex --;
			if (lIndex < 0 || lIndex >= OwningList.Count)
				return null;
			return (LanguageElement)OwningList[lIndex];
		}
		public LanguageElement GetSiblingAfter(int line, int offset)
		{
			if (OwningList == null || _Index < 0)
				return null;
			int lIndex = GetNodeIndexAfterInternal(OwningList, line, offset);
			if (lIndex < 0)
			{
				lIndex = ~lIndex;
			}
			if (lIndex == _Index)
				lIndex ++;
			if (lIndex < 0  || lIndex >= OwningList.Count)
				return null;
			return (LanguageElement)OwningList[lIndex];
		}
		#region GetChildAt(SourcePoint sourcePoint)
		public virtual LanguageElement GetChildAt(SourcePoint sourcePoint)
		{
			return GetChildAt(sourcePoint.Line, sourcePoint.Offset);
		}
		#endregion
		LanguageElement GetNearestChildInMacro(int line, int column)
		{
			SourceRange macroRange = this.Range;
			if (macroRange.IsEmpty)
				return null;
			IEnumerable allChildren = new ElementEnumerable(this, true);
			foreach (LanguageElement current in allChildren)
			{
				if (current == null || current.Range == macroRange || !current.Range.Contains(line, column))
					continue;
				return current;
			}
			return null;
		}
		#region GetChildAt(int line, int column)
		public virtual LanguageElement GetChildAt(int line, int column)
		{
			LanguageElement lResult = null;
	  bool inMacro = StructuralParserServicesHolder.InMacroCall(this);
			if (inMacro)
			{
				LanguageElement scope = GetNearestChildInMacro(line, column);
				if (scope != null)
					lResult = scope.GetChildAt(line, column);
			}
			else
			{
				lResult = GetChildAtInternal(line, column, Nodes);
				if (lResult == null)
				{
					lResult = GetChildAtInternal(line, column, DetailNodes);
				}
			}
			return lResult;
		}
		#endregion
		#region GetChildBefore(SourcePoint sourcePoint)
		public LanguageElement GetChildBefore(SourcePoint sourcePoint) 
		{
			return GetChildBefore(sourcePoint.Line, sourcePoint.Offset);
		}
		#endregion
		#region GetChildBefore(int line, int offset)
		public LanguageElement GetChildBefore(int line, int offset)
		{
			LanguageElement lResult = GetNodeBefore(Nodes, line, offset);
			if (lResult != null)
				return lResult;
			return GetNodeBefore(DetailNodes, line, offset);
		}
		#endregion
		#region GetCyclomaticComplexity
		public virtual int GetCyclomaticComplexity()
		{
			return 0;
		}
		#endregion
		#region GetMaintenanceComplexity
		public virtual int GetMaintenanceComplexity()
		{
			int result = ThisMaintenanceComplexity;
			if (Nodes != null)
				for (int i = 0; i < Nodes.Count; i++)
				{
					LanguageElement lElement = (LanguageElement)Nodes[i];
					if (lElement != null)
						result += lElement.GetMaintenanceComplexity();
				}
			if (DetailNodes != null)
				for (int i = 0; i < DetailNodes.Count; i++)
				{
					LanguageElement lElement = (LanguageElement)DetailNodes[i];
					if (lElement != null)
						result += lElement.GetMaintenanceComplexity();
				}
			return result;
		}
		#endregion
		#region Inside
		public bool Inside( LanguageElementType aElementType)
		{
			return (GetParent(aElementType) != null);
		}		
		public bool Inside(params LanguageElementType[] types)
		{
			if (types == null || types.Length == 0)
				return false;			
			foreach(LanguageElementType elementType in types)
			{				
				if (GetParent(elementType) != null)
					return true;
			}
			return false;
		}
		#endregion
		#region GetImageIndex
		public override int GetImageIndex()
		{
			return ImageIndex.LanguageElement;	
		}
		#endregion
		#region GetNodeAt(SourcePoint sourcePoint)
		public virtual LanguageElement GetNodeAt(SourcePoint sourcePoint)
		{
			return GetNodeAt(sourcePoint.Line, sourcePoint.Offset);
		}
		#endregion
		#region GetNodeAt(int line, int column)
		public virtual LanguageElement GetNodeAt(int line, int column)
		{
			LanguageElement lResult = null;
			if (Parent != null)
			{
				lResult = Parent.GetChildAt(line, column);
				if (lResult == null && Parent.Contains(line, column))
					return Parent;
				if (lResult != null)
					return lResult;
				else	
					return Parent.GetNodeAt(line, column);
			}
			else		
			{
				lResult = GetChildAt(line, column);
				if (lResult == null && Contains(line, column))
					lResult = this;
				return lResult;
			}
		}
		#endregion
		#region GetNearestParentingStatement(SourcePoint point)
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ParentingStatement GetNearestParentingStatement(SourcePoint point)
		{
			if (point == SourcePoint.Empty)
				return null;
			LanguageElement lActiveElement = GetChildAt(point);
			if (lActiveElement == null)
				return null;			
			if (lActiveElement is ParentingStatement)
				return lActiveElement as ParentingStatement;
			return lActiveElement.GetParentParentingStatement();
		}
		#endregion
		#region GetDefaultVisibility
		public virtual MemberVisibility GetDefaultVisibility()
		{
			if (_Parent == null)
				return MemberVisibility.Illegal;
			else
				return _Parent.GetDefaultVisibility();
		}
		#endregion
		#region GetValidVisibilities
		public virtual MemberVisibility[] GetValidVisibilities()
		{
			if (_Parent == null)
				return null;
			else
				return _Parent.GetValidVisibilities();
		}
		#endregion
		#region IsParentedBy
		public bool IsParentedBy(LanguageElement aParentElement)
		{
			LanguageElement lLanguageElement = _Parent;
			while (lLanguageElement != null)
			{
				if (lLanguageElement == aParentElement)
					return true;
				lLanguageElement = lLanguageElement._Parent;
			}
			return false;
		}
		#endregion
		#region MatchesSelection
		public bool MatchesSelection()
		{
			IDocument lTextDocument = Document;
			if (lTextDocument == null)
				return false;
			int lStartLine;
			int lStartOffset;
			int lEndLine;
			int lEndOffset;
			lTextDocument.GetSelectionBounds(out lStartLine, out lStartOffset, out lEndLine, out lEndOffset);
			return (lStartLine == StartLine && lStartOffset == StartOffset && 
				lEndLine == EndLine && lEndOffset == EndOffset);
		}
		#endregion
		#region Parents
		public bool Parents(LanguageElement aChildElement)
		{
			if (aChildElement == null)
				return false;
			return aChildElement.IsParentedBy(this);
		}
		#endregion
		#region RangeIsClean
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool RangeIsClean(SourceRange sourceRange)
		{
			return false;
		}
		#endregion
		#region ToString
		public override string ToString()
		{
			return( InternalName  );
		}
		#endregion
		#region GetFullPath
		public virtual string GetFullPath()
		{
			string lParentPath = ParentPath;
			string lPathSegment = PathSegment;
			string lFullPath;
			if (lPathSegment != String.Empty)
				if (lParentPath != String.Empty)
					lFullPath = lParentPath + "." + lPathSegment;
				else
					lFullPath = lPathSegment;
			else 
				lFullPath = lParentPath;
			return lFullPath;
		}
		#endregion
		#region PointInDetails
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool PointInDetails(SourcePoint point)
		{
			return IsInsideNodeList(DetailNodes, point);
		}
		#endregion		
		#region PointInNodes
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool PointInNodes(SourcePoint point)
		{
			return IsInsideNodeList(Nodes, point);
		}
		#endregion
		#region HasInDetails
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool HasInDetails(LanguageElement element)
		{
			if (element == null || DetailNodeCount == 0)
				return false;
			return FindInList(DetailNodes, element);			
		}
		#endregion
		#region HasInNodes
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool HasInNodes(LanguageElement element)
		{
			if (element == null || NodeCount == 0)
				return false;			
			return FindInList(Nodes, element);			
		}
		#endregion
		#region IsDetailOfParentStatement
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool IsDetailOfParentStatement()
		{
			LanguageElement lParent = GetParentStatementOrVariable();
			if (lParent == null)
				return false;
			return lParent.HasInDetails(this);
		}
		#endregion
		#region GetParentingStatement
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public ParentingStatement GetParentParentingStatement()
		{
			if (this is Method || Parent == null || Parent is Method)
				return null;
			if (Parent is ParentingStatement)
				return (Parent as ParentingStatement);
			return Parent.GetParentParentingStatement();
		}
		#endregion
		#region GetFirstLevelParentInCustomElement
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LanguageElement GetFirstLevelParentInCustomElement(LanguageElementType elementType)
		{			
			LanguageElement lParentElement = GetParent(elementType);
			if (lParentElement == null)
				return null;
			LanguageElement lResult = this;			
			while (lResult != null)
			{
				if (lResult.CompletesPrevious)
					lResult = lResult.PreviousSibling;
				if (lResult.Parent == null || lResult.Parent.ElementType == elementType)
					return lResult;
				lResult = lResult.Parent;
			}
			return null;
		}
		#endregion
		#region GetFirstLevelParentInParentMethod
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LanguageElement GetFirstLevelParentInParentMethod()
		{			
			return GetFirstLevelParentInCustomElement(LanguageElementType.Method);
		}
		#endregion
		#region GetFirstLevelParentInParentProperty
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LanguageElement GetFirstLevelParentInParentProperty()
		{			
			LanguageElement lElementInGetter = GetFirstLevelParentInCustomElement(LanguageElementType.PropertyAccessorGet);
			if (lElementInGetter != null)
				return lElementInGetter;
			LanguageElement lElementInSetter = GetFirstLevelParentInCustomElement(LanguageElementType.PropertyAccessorSet);
			if (lElementInSetter != null)
				return lElementInSetter;
			return null;
		}
		#endregion
		#region GetFirstLevelParentInParentMethodOrProperty
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LanguageElement GetFirstLevelParentInParentMethodOrProperty()
		{
			LanguageElement lElementInMethod = GetFirstLevelParentInParentMethod();
			if (lElementInMethod != null)
				return lElementInMethod;
			LanguageElement lElementInProperty = GetFirstLevelParentInParentProperty();
			if (lElementInProperty != null)
				return lElementInProperty;
			return null;
		}
		#endregion
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			LanguageElement lClone = (LanguageElement)this.MemberwiseClone();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region ParseOnDemandIfNeeded()
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void ParseOnDemandIfNeeded()
		{
		}
		#endregion
		#region ParseAllPostponedElements()
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ParseAllPostponedElements()
		{
		}
		#endregion
		#region SetParent
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void SetParent(LanguageElement aParent)
		{
			_Parent = aParent;
		}
		#endregion
	void IElementModifier.SetName(string name)
	{
	  Name = name;
	}
	void IElementModifier.InsertChild(int index, IElement child)
	{
	  InsertNode(index, child as LanguageElement);
	}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetFakeFlag(bool isFakeNode)
		{
			_IsFakeNode = isFakeNode;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetIndex(int index)
		{
			_Index = index;
		}
		public virtual IElement GetDeclaration()
		{
	  return StructuralParserServicesHolder.GetDeclaration(this);
		}
		public virtual IElement GetDeclaration(bool restore)
		{
	  return StructuralParserServicesHolder.GetDeclaration(this, restore);
		}
		public virtual IElementCollection FindAllReferences()
		{
	  return StructuralParserServicesHolder.FindAllReferences(this);
		}
		public virtual IElementCollection FindAllReferences(IElement scope)
		{
	  return StructuralParserServicesHolder.FindAllReferences(scope, this);
		}
		#region ThisMaintenanceComplexity
		protected virtual int ThisMaintenanceComplexity
		{
			get
			{
				return 0;
			}
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected virtual PostponedParsingData PostponedData 
		{
			get { return null; }
			set { ; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected internal NodeList OwningList
		{
			get 
			{
				if (Parent == null)
					return null;
				return IsDetailNode ? Parent.InnerDetailNodes : Parent.InnerNodes;
			}
		}
		#region PostponedComments
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected CommentCollection PostponedComments
		{
			get
			{
				if (PostponedData == null)
					return null;
				return PostponedData.Comments;
			}
			set
			{
				if (PostponedData == null)
					return;
				PostponedData.Comments = value;
			}
		}
		#endregion
		public virtual string MacroCall
		{
			get
			{
				return null;
			}
			set
			{
			}
		}
		#region ParsingPostponedTokens
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ParsingPostponedTokens
		{
			get
			{
				return PostponedData != null && PostponedData.IsParsing;
			}
			set
			{
				if (PostponedData != null)
					PostponedData.IsParsing = value;
			}
		}
		#endregion
		#region Range
		[Description("The source range of this language element.")]
		[Category("Position")]
		public SourceRange Range
		{
			get
			{
				return InternalRange;
			}
		}
		#endregion
		#region CanContainCode
		[Description("Whether this language element can contain code statements.")]
		[Category("Family")]
		[DefaultValue(false)]
		public virtual bool CanContainCode
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region CanBeDocumented
		[Description("Whether XML documentation comments can be bound to this language element.")]
		[Category("Family")]
		[DefaultValue(false)]
		public bool CanBeDocumented
		{
			get
			{
				return GetCanBeDocumented();
			}
		}
		#endregion
		#region CompletesPrevious
		[Description("True if this language element connects to the previous statement or preprocessor directive (e.g., catch, finally, #if, #elif, #warning, etc.).")]
		[Category("Family")]
		[DefaultValue(false)]
		public virtual bool CompletesPrevious
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region DeclaresIdentifier
		[Description("Gets whether this node declares an identifier or not. If true, the Name property will hold the name of the identifier.")]
		[Category("Family")]
		[DefaultValue(false)]
		public virtual bool DeclaresIdentifier
		{
			get
			{
				return false;
			}
		}
		#endregion
		#region ElementType
		[Description("The type of this language element.")]
		[Category("Description")]
		public abstract LanguageElementType ElementType
		{
			get;
		}
		#endregion
		#region InsideMethod
		[Description("Whether this language element is inside a Method.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideMethod
		{
			get
			{
				return Inside( LanguageElementType.Method );
			}
		}
		#endregion
		[Description("Returns true if this language element is inside an Event.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideEvent
		{
			get
			{
				return GetParentEvent() != null;
			}
		}
		[Description("Returns true if this language element is inside an Event accessor.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideEventAccessor
		{
			get
			{
				return GetParentEventAccessor() != null;
			}
		}
		[Description("Returns true if this language element is inside left side of an assignment.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideAssignment
		{
			get
			{
				Assignment lParentAssignment = GetParent(LanguageElementType.Assignment) as Assignment;
				if (lParentAssignment == null)
					return false;
				return lParentAssignment.LeftSide == this;
			}
		}
		[Description("Returns true if this language element is used inside out argument direction expression.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideOutArgumentDirection
		{
			get
			{
				ArgumentDirectionExpression lDirection = GetParent(LanguageElementType.ArgumentDirectionExpression) as ArgumentDirectionExpression;
				if (lDirection == null)
					return false;
				return lDirection.Direction == ArgumentDirection.Out;
			}			
		}
		[Description("Returns true if this language element is used inside ref argument direction expression.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideRefArgumentDirection
		{
			get
			{
				ArgumentDirectionExpression direction = GetParent(LanguageElementType.ArgumentDirectionExpression) as ArgumentDirectionExpression;
				if (direction == null)
					return false;
				return direction.Direction == ArgumentDirection.Ref;
			}
		}
		[Description("Returns true if this language element is used inside increment expression.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideIncrement
		{
			get
			{
				UnaryIncrement expression = GetParent(LanguageElementType.UnaryIncrement) as UnaryIncrement;
				if (expression == null)
					return false;
				return expression.Expression == this;
			}
		}
		[Description("Returns true if this language element is used inside decrement expression.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideDecrement
		{
			get
			{
				UnaryDecrement expression = GetParent(LanguageElementType.UnaryDecrement) as UnaryDecrement;
				if (expression == null)
					return false;
				return expression.Expression == this;
			}
		}
		#region InsideProperty
		[Description("Whether this language element is inside a Property.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideProperty
		{
			get
			{
				return Inside( LanguageElementType.Property );
			}
		}
		#endregion
		#region InsideClass
		[Description("Whether this language element is inside a Class.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideClass
		{
			get
			{
				return Inside( LanguageElementType.Class, LanguageElementType.ManagedClass, LanguageElementType.ValueClass );
			}
		}
		#endregion
		#region InsideStruct
		[Description("Whether this language element is inside a Struct.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideStruct
		{
			get
			{
				return Inside( LanguageElementType.Struct, LanguageElementType.ManagedStruct, LanguageElementType.ValueStruct );
			}
		}
		#endregion
		#region InsideInterface
		[Description("Whether this language element is inside an Interface declaration.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideInterface
		{
			get
			{
				return Inside( LanguageElementType.Interface );
			}
		}
		#endregion
		#region InsideNamespace
		[Description("Whether this language element is inside a Namespace.")]
		[Category("Location")]
		[DefaultValue(false)]
		public bool InsideNamespace
		{
			get
			{
				return Inside( LanguageElementType.Namespace );
			}
		}
		#endregion
		#region IsDetailNode
		[Description("Whether this language element is a detail node.")]
		[Category("Family")]
		[DefaultValue(false)]
		public bool IsDetailNode
		{
			get
			{
				return _IsDetailNode;
			}
			set
			{
				_IsDetailNode = value;
			}
		}
		#endregion
		#region HasDocument
		[Description("Returns true if this language element was parsed from a source file.")]
		public bool HasDocument
		{
			get
			{
				return Document != null;
			}
		}
		#endregion
		#region LevelsDeep
		[Description("Gets the number of parent nodes between this node and the topmost node, inclusive of the topmost node.")]
		[Category("Location")]
		public int LevelsDeep
		{
			get
			{
				int levelsDeep = 0;
				LanguageElement lLanguageElement = this;
				while (lLanguageElement.Parent != null)
				{
					lLanguageElement = lLanguageElement.Parent;
					levelsDeep++;
				}
				return levelsDeep;
			}
		}
		#endregion
		#region Parent
		[Description("The parent of this language element.")]
		[Category("Family")]
		public LanguageElement Parent
		{
			get
			{
				return _Parent;
			}
		}
		#endregion
		[Description("The parent of this language element.")]
		[Category("Family")]
		public SourceFile FileNode
		{
			get
			{
				return GetParent(LanguageElementType.SourceFile) as SourceFile;
			}
		}
		#region ParentRegion
		[Description("The region that parents this node.")]
		[Category("Family")]
		[DefaultValue(null)]
		public RegionDirective ParentRegion
		{
			get
			{
				SourceFile lSourceFile = GetSourceFile();
				if (lSourceFile == null || lSourceFile.RegionRootNode == null)
					return null;
				return lSourceFile.RegionRootNode.GetChildAt(InternalRange.Start) as RegionDirective;
			}
		}
		#endregion
		#region IsLoop
		[Description("Returns true if this language element is loop statement.")]
		[Category("Family")]
		public bool IsLoop
		{
			get
			{
				if (this is ParentingStatement)
				{
					ParentingStatement lParentingStatement = (ParentingStatement)this;
					if (lParentingStatement.IsBreakable)
						return true;
				}
				return false;
			}
		}
		#endregion
		#region ParentLoop
		[Description("The parenting loop, switch statement, or other code block that can be broken by a \"break;\" statement.")]
		[Category("Family")]
		[DefaultValue(null)]
		public ParentingStatement ParentLoop
		{
			get
			{
				LanguageElement lLanguageElement = _Parent;
				while (lLanguageElement != null)
				{
		  if (lLanguageElement.ElementType == LanguageElementType.AnonymousMethodExpression ||
			  lLanguageElement.ElementType == LanguageElementType.LambdaExpression)
			break;
		  if (lLanguageElement.IsLoop)
						return lLanguageElement as ParentingStatement;
					lLanguageElement = lLanguageElement._Parent;
				}
				return null;		
			}
		}
		#endregion
		#region ParentTryBlock
		[Description("The parenting try block that contains this language element.")]
		[Category("Family")]
		[DefaultValue(null)]
		public Try ParentTryBlock
		{
			get
			{
				LanguageElement lLanguageElement = _Parent;
				while (lLanguageElement != null)
				{
					if (lLanguageElement is Try)
						return (Try)lLanguageElement;
					lLanguageElement = lLanguageElement._Parent;
				}
				return null;		
			}
		}
		#endregion
		[Description("Gets parent with statement for this element.")]
		[Category("Family")]
		[DefaultValue(null)]
		public LanguageElement ParentWith
		{
			get
			{
				return GetParent(LanguageElementType.With);
			}
		}
		#region ParentPath
		[Description("Gets the full path for this element's parent.")]
		[Category("Location")]
		public virtual string ParentPath
		{
			get
			{
				if (_Parent != null)
					return Parent.GetFullPath();
				else
					return String.Empty;
			}
		}
		#endregion
		#region PathSegment
		[Description("Gets path segment for this element.")]
		[Category("Location")]
		public virtual string PathSegment
		{
			get
			{
				return String.Empty;
			}
		}
		#endregion
		#region Solution
		public virtual ISolutionElement Solution
		{
			get
			{
				IProjectElement project = Project;
		if (project == null)
					return null;
		return project.GetParent(LanguageElementType.SolutionElement) as ISolutionElement;
			}
		}
		#endregion		
		#region Project
		public virtual IProjectElement Project
		{
			get
			{
				VisualStudioDocument lVisualStudioDocument = GetParentDocument();
				if (lVisualStudioDocument != null)
					return lVisualStudioDocument.Project;
				else
					return null;
			}
		}
		#endregion
		#region Location
		[Description("The location of this node as a string.")]
		[Category("Location")]
		public string Location
		{
			get
			{
				string lFullPath = GetFullPath();
				if (PathSegment == String.Empty)
				{
					string lName = ToString();
					if (lName != String.Empty)
						lFullPath = lFullPath + " - " + lName;
				}
				return lFullPath;
			}
		}
		#endregion
#if !SL
	public string GetFileLocation()
	{
	  return ElementLocation.GetFileLocation(this);
	}
	public static LanguageElement Find(IProjectElement proj, string fileLocation)
	{
	  return ElementLocation.FindElement(proj, fileLocation);
	}
	public static LanguageElement Find(string fileLocation)
	{
	  return Find(null, fileLocation);
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static LanguageElement FindInFile(SourceFile file, string fileLocation)
	{
	  return ElementLocation.FindElementInFile(file, fileLocation);
	}
#endif
		#region RootNamespaceLocation
		[Description("Gets the location of this node as a string starting with root namespace.")]
		[Category("Location")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string RootNamespaceLocation
		{
			get
			{
				string result = Location;
				IProjectElement project = Project;
		if (project != null && project.HasRootNamespace)
		  result = String.Concat(project.RootNamespace, ".", Location);
		return result;
			}
		}
		#endregion
		#region PreviousNode
		[Description("The previous node in the code following the start of this node.")] 
		[Category("Family")] 
		[DefaultValue(null)] 
		public LanguageElement PreviousNode
		{ 
			get 
			{ 				
				LanguageElement lPrevSibling = PreviousSibling;
				if (lPrevSibling != null)
				{					
					if (lPrevSibling.LastChild != null)
						return lPrevSibling.LastChild;
					if (lPrevSibling.LastDetail != null)
						return lPrevSibling.LastDetail;
					return lPrevSibling;
				}
				if (!IsDetailNode &&
					Parent != null && 
					Parent.LastDetail != null)
					return Parent.LastDetail;
				return this.Parent;
			} 
		} 
		#endregion
		#region NextNode 
		[Description("The next node in the code following the start of this node.")] 
		[Category("Family")] 
		[DefaultValue(null)] 
		public LanguageElement NextNode 
		{ 
			get 
			{ 
				LanguageElement lResult = FirstDetail;  
				if (lResult != null) 
					return lResult; 
				lResult = FirstChild; 
				if (lResult != null) 
					return lResult; 
				lResult = NextSibling; 
				if (lResult != null) 
					return lResult; 
				if (IsDetailNode) 
				{ 
					if (Parent != null) 
					{ 
						lResult = Parent.FirstChild; 
						if (lResult != null) 
							return lResult; 
					} 
				} 
				LanguageElement thisParent = this; 
				do 
				{ 
					thisParent = thisParent.Parent; 
					if (thisParent == null) 
						return null; 
					lResult = thisParent.NextSibling;
					if (lResult == null && thisParent.IsDetailNode && thisParent.Parent != null)
						lResult = thisParent.Parent.FirstChild;
				} 
				while (lResult == null); 
				return lResult; 
			} 
		} 
		#endregion
		#region NextSibling
		[Description("The next sibling to this element, or null if no siblings follow this element.")]
		[Category("Family")]
		[DefaultValue(null)]
		public virtual LanguageElement NextSibling
		{
			get
			{
				if (OwningList == null)
					return null;
				ValidateIndices();
				int lTargetIndex = _Index + 1;
				if (lTargetIndex >= OwningList.Count)
					return null;
				if (lTargetIndex < 0)
					return null;
				return (LanguageElement)OwningList[lTargetIndex];
			}
		}
		#endregion
		#region PreviousSibling
		[Description("The previous sibling to this element, or null if no siblings precede this element.")]
		[Category("Family")]
		[DefaultValue(null)]
		public virtual LanguageElement PreviousSibling
		{
			get
			{
				if (OwningList == null)
					return null;
				ValidateIndices();
				int lTargetIndex = _Index - 1;
				if (lTargetIndex >= OwningList.Count)
					return null;
				if (lTargetIndex < 0)
					return null;
				return (LanguageElement)OwningList[lTargetIndex];
			}
		}
		#endregion
		#region NextCodeSibling
		[Description("The next non-comment sibling to this element, or null if no siblings containing code follow this element.")]
		[Category("Family")]
		[DefaultValue(null)]
		public virtual LanguageElement NextCodeSibling
		{
			get
			{
				if (OwningList == null)
					return null;
				ValidateIndices();
				int lTargetIndex = _Index + 1;
				if (lTargetIndex < 0)
					return null;
				while (lTargetIndex < OwningList.Count)
				{
					LanguageElement lNextElement = (LanguageElement)OwningList[lTargetIndex];
					if (lNextElement.ElementType != LanguageElementType.Comment &&
			lNextElement.ElementType != LanguageElementType.FictiveAspComment)
						return lNextElement;
					lTargetIndex++;
				}
				return null;
			}
		}
		#endregion
		#region NextStandaloneCodeSibling
		public LanguageElement NextStandaloneCodeSibling(LanguageElement element)
		{
			if (element == null)
				return null;
			LanguageElement lNextElement = element;
			do
			{
				lNextElement = lNextElement.NextCodeSibling;
			} while (lNextElement != null && 
				lNextElement.CompletesPrevious && 
				lNextElement.ElementType != LanguageElementType.Finally);
			if (lNextElement != null && lNextElement.ElementType == LanguageElementType.Finally)
			{
				LanguageElement lFirstChild = lNextElement.GetFirstCodeChild();
				if (lFirstChild != null)
					lNextElement = lFirstChild;
			}
			return lNextElement;
		}
		#endregion
		#region NextStandaloneCodeSibling
		public LanguageElement NextStandaloneCodeSibling()
		{
			return NextStandaloneCodeSibling(this);
		}
		#endregion
		#region PreviousCodeSibling
		[Description("The previous non-comment sibling to this element, or null if no siblings containing code precede this element.")]
		[Category("Family")]
		[DefaultValue(null)]
		public virtual LanguageElement PreviousCodeSibling
		{
			get
			{
				if (OwningList == null)
					return null;
				ValidateIndices();
				int lTargetIndex = _Index - 1;
				if (lTargetIndex >= OwningList.Count)
					return null;
				while (lTargetIndex >= 0)
				{
					LanguageElement lPreviousElement = (LanguageElement)OwningList[lTargetIndex];
					if (!(lPreviousElement is Comment))
						return lPreviousElement;
					lTargetIndex--;
				}
				return null;
			}
		}
		#endregion
		#region FirstDetail
		[Description("The first detail node of this element, or null if no detail nodes exist.")] 
		[Category("Family")] 
		[DefaultValue(null)] 
		public virtual LanguageElement FirstDetail
		{ 
			get 
			{ 
				if (DetailNodes == null || DetailNodes.Count == 0) 
					return null; 
				else 
					return (LanguageElement)DetailNodes[0]; 
			} 
		} 
		#endregion 
		#region FirstChild
		[Description("The first child to this element, or null if no children exist.")]
		[Category("Family")]
		[DefaultValue(null)]
		public virtual LanguageElement FirstChild
		{
			get
			{
				if (Nodes == null || Nodes.Count == 0)
					return null;
				else
					return (LanguageElement)Nodes[0];
			}
		}
		#endregion
		#region FirstSibling
		[Description("The first sibling of this language element.")]
		[Category("Family")]
		[DefaultValue(null)]
		public LanguageElement FirstSibling
		{
			get
			{
				if (OwningList == null || OwningList.Count <= 0)
					return null;
				return (LanguageElement)OwningList[0];
			}
		}
		#endregion
		#region Nodes
		[Description("The language elements parented by this element.")]
		[Category("Family")]
		public override NodeList Nodes
		{
			get
			{
				ParseOnDemandIfNeeded();
				return base.Nodes;
			}
		}
		#endregion
		#region DetailNodes
		[Description("The detail language elements associated with this element.")]
		[Category("Family")]
		public override NodeList DetailNodes
		{
			get
			{
				ParseOnDemandIfNeeded();
				return base.DetailNodes;
			}
		}
		#endregion
		#region UsePostponedParsing
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool UsePostponedParsing
		{
			get
			{
				return true;
			}
		}
		#endregion
		#region HasUnparsedCode
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasUnparsedCode
		{
			get
			{
				return PostponedData != null && PostponedData.HasUnparsedCode;
			}
		}
		#endregion
		#region UnparsedCode
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ISourceReader UnparsedCode
		{
			get
			{
				if (PostponedData != null)
					return PostponedData.UnparsedCode;
				return null;
			}
		}
		#endregion
		#region HasPostponedComments
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasPostponedComments
		{
			get
			{
				return PostponedData != null && PostponedData.HasComments;
			}
		}
		#endregion		
		#region Index
		[Description("The index into the OwningList.")]
		[Category("Family")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(-1)]
		public int Index
		{
			get
			{
				return _Index;
			}
		}
		#endregion
		#region LastSibling
		[Description("The last sibling of this language element.")]
		[Category("Family")]
		[DefaultValue(null)]
		public LanguageElement LastSibling
		{
			get
			{
				if (OwningList == null || OwningList.Count <= 0)
					return null;
				return (LanguageElement)OwningList[OwningList.Count - 1];
			}
		}
		#endregion
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsFakeNode
		{
			get
			{
				return _IsFakeNode;
			}
		}
		#region LastChild
		[Description("The last child to this element, or null if no children exist.")]
		[Category("Family")]
		[DefaultValue(null)]
		public virtual LanguageElement LastChild
		{
			get
			{
				if (Nodes == null || Nodes.Count == 0)
					return null;
				else
					return (LanguageElement)Nodes[Nodes.Count - 1];
			}
		}
		#endregion
		#region LastDetail
		[Description("The last detail to this element, or null if no details exist.")]
		[Category("Family")]
		[DefaultValue(null)]
		public virtual LanguageElement LastDetail
		{
			get
			{
				if (DetailNodes == null || DetailNodes.Count == 0)
					return null;
				else
					return (LanguageElement)DetailNodes[DetailNodes.Count - 1];
			}
		}
		#endregion
		#region Document
		[Description("Gets the IDocument associated with the parenting SourceFile language element. May return null if this node was parsed from a file on disk.")]
		[Category("Family")]
		[DefaultValue(null)]
		public virtual IDocument Document
		{
			get
			{
				VisualStudioDocument lVisualStudioDocument = GetParentDocument();
				return lVisualStudioDocument != null ? lVisualStudioDocument.Document : null;
			}
		}
		#endregion
		#region View
		[Description("Returns a view associated with the document that was the source for this language element.")]
		[Category("Family")]
		[DefaultValue(null)]
		public IDXCoreTextView View
		{
			get
			{
				IDocument lDocument = Document;
				if (lDocument == null)
					return null;
				return lDocument.ActiveView;
			}
		}
		#endregion
		#region IsCollapsible
		public bool IsCollapsible
		{
			get
			{
		return StructuralParserServicesHolder.IsCollapsible(this);
			}
		}
		#endregion
		#region CollapsibleRange
		public SourceRange CollapsibleRange
		{
			get
			{
		return StructuralParserServicesHolder.GetCollapsibleRange(this);
			}
		}
		#endregion
		protected virtual bool HasOuterRangeChildren
		{
			get
			{
				return false;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsTypeDeclaration
		{
			get
			{
				return false;
			}
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public LanguageElementTokens Tokens
	{
	  set
	  {
		_Tokens = value;
	  }
	  get
	  {
		return _Tokens;
	  }
	}
		#region Collapsed
		public bool Collapsed
		{
			get
			{
				ICollapsibleRegion lRegion = GetCollapsibleRegion();
				if (lRegion == null)
					return false;
				return lRegion.Collapsed;
			}
			set
			{
				ICollapsibleRegion lRegion = GetCollapsibleRegion();
				if (lRegion == null)
					return;
				lRegion.Collapsed = value;
			}
		}
		#endregion
		#region Expanded
		public bool Expanded
		{
			get
			{
				ICollapsibleRegion lRegion = GetCollapsibleRegion();
				if (lRegion == null)
					return true;
				return lRegion.Expanded;
			}
			set
			{
				ICollapsibleRegion lRegion = GetCollapsibleRegion();
				if (lRegion == null)
					return;
				lRegion.Expanded = value;
			}
		}
		#endregion
	#region IsExpandedInView
	public bool IsExpandedInView(IDXCoreTextView textView)
	{
	  ICollapsibleRegion lRegion = GetCollapsibleRegion(textView);
	  if (lRegion == null)
		return true;
	  return lRegion.Expanded;
	}
	#endregion
	#region ExpandInView
	public void ExpandInView(IDXCoreTextView textView)
	{
	  ExpandInView(textView, true);
	}
	#endregion
	#region ExpandInView(IDXCoreTextView textView, bool undoable)
	public void ExpandInView(IDXCoreTextView textView, bool undoable)
	{ 
	  ICollapsibleRegion lRegion = GetCollapsibleRegion(textView);
			if (lRegion == null)
				return;
			lRegion.Expand(undoable);
	}
	#endregion
	#region IsCollapsedInView
	public bool IsCollapsedInView(IDXCoreTextView textView)
	{
	  ICollapsibleRegion lRegion = GetCollapsibleRegion(textView);
	  if (lRegion == null)
		return false;
	  return lRegion.Collapsed;
	}
	#endregion
	#region Collapse(IDXCoreTextView textView)
	public void CollapseInView(IDXCoreTextView textView)
	{
	  Collapse(textView, true);
	}
	#endregion
	#region Collapse(IDXCoreTextView textView, bool undoable)
	public void Collapse(IDXCoreTextView textView, bool undoable)
	{
	  ICollapsibleRegion lRegion = GetCollapsibleRegion(textView);
	  if (lRegion == null)
		return;
	  lRegion.Collapse(undoable);
	}
	#endregion
	#region InCollapsedRange
	public bool InCollapsedRange(IDXCoreTextView textView)
	{	  
	  if (Document == null)
	  {
		Log.SendWarning("LanguageElement.GetCollapsibleRegion() found a null Document property.");
		return false;
	  }
	  return Document.InCollapsedRange(this, textView);
	}
	#endregion
	[EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasErrors
		{
			get { return _HasErrors; }
			set
			{
				_HasErrors = value;
			}
		}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public bool IsRemoved
	{
	  get
	  {
		return _IsRemoved;
	  }
	  set
	  {
		if (_IsRemoved == value)
		  return;
		_IsRemoved = value;
		for (int i = 0; i < DetailNodeCount; i++)
		{
		  LanguageElement element = DetailNodes[i] as LanguageElement;
		  if (element == null)
			continue;
		  element.IsRemoved = value;
		}
		for (int i = 0; i < NodeCount; i++)
		{
		  LanguageElement element = Nodes[i] as LanguageElement;
		  if (element == null)
			continue;
		  element.IsRemoved = value;
		}
	  }
	}
	#region IElement Members
	void IElement.Accept(IElementVisitor visitor)
	{
	  NodeList.VisitNodeList(DetailNodes, visitor);
	  NodeList.VisitNodeList(Nodes, visitor);
	}
		IElement IElement.GetParent(LanguageElementType type, params LanguageElementType[] args)
		{
			return GetParent(type, args);
		}
		IElement IElement.GetParentStatementOrVariable()
		{
			return GetParentStatementOrVariable();
		}
		IElement IElement.FindChildByName(string name)
		{
			return FindChildByName(name, true);
		}
		bool IElement.IsParentedBy(IElement element)
		{
			if (!(element is LanguageElement))
				return false;
			return IsParentedBy((LanguageElement)element);
		}
		IElement IElement.Clone()
		{
			return Clone() as IElement;
		}
		IElement IElement.Clone(ElementCloneOptions options)
		{
			return Clone(options) as IElement;
		}		
		LanguageElement IElement.ToLanguageElement()
		{
			return this;
		}		
		string IElement.Name
		{
			get
			{
				return InternalName;
			}
		}
		string IElement.FullName
		{
			get
			{
				return RootNamespaceLocation;
			}
		}
		IElement IElement.Parent
		{
			get
			{
				return Parent;
			}
		}						
		ITypeElement IElement.ParentType
		{ 
			get
			{
				return GetParentTypeDeclaration() as ITypeElement;
			}
		}
		IEventElement IElement.ParentEvent 
		{
			get
			{
				return GetParent(LanguageElementType.Event) as IEventElement;					
			}
		}
		IMemberElement IElement.ParentMember 
		{ 
			get
			{
				return GetParent(LanguageElementType.Method, 
					LanguageElementType.Property,
					LanguageElementType.Event) as IMemberElement;
			}
		}
		IMethodElement IElement.ParentMethod 
		{
			get
			{
				return GetParent(LanguageElementType.Method) as IMethodElement;
			}
		}
		IElement IElement.ParentMethodOrAccessor
		{
			get
			{
				return GetParentMethodOrAccessor();
			}
		}
		IElement IElement.ParentMethodOrPropertyOrEvent
		{
			get 
			{
				return GetParentMethodOrPropertyOrEvent();
			}
		}
		IPropertyElement IElement.ParentProperty 
		{
			get
			{
				return GetParent(LanguageElementType.Property) as IPropertyElement;
			}
		}
		INamespaceElement IElement.ParentNamespace 
		{
			get
			{
				return GetParent(LanguageElementType.Namespace) as INamespaceElement;
			}
		}
		IProjectElement IElement.Project
		{
			get
			{
				return Project;
			}
		}
		ISolutionElement IElement.Solution
		{
			get
			{
				return Solution;
			}
		}
		IAssemblyModel IElement.AssemblyModel
		{ 
			get
			{
		return StructuralParserServicesHolder.GetAssemblyModel(this);
			}
		}
		IElementCollection IElement.Children
		{
			get
			{
				IElementCollection lChildren = new IElementCollection();
				lChildren.AddRange(DetailNodes);
				lChildren.AddRange(Nodes);				
				return lChildren;
			}
		}
		IEnumerable<IElement> IElement.AllChildren
		{
			get
			{
				foreach (IElement detailNode in DetailNodes)
					yield return detailNode;
				foreach (IElement node in Nodes)
					yield return node;
			}
		}
		IElementCollection IElement.CodeChildren
		{
			get
			{
				IElementCollection codeChildren = new IElementCollection();
				codeChildren.AddRange(Nodes);
				return codeChildren;
			}
		}
		ISourceFileCollection IElement.Files 
		{ 
			get 
			{
				LiteSourceFileCollection lResult = new LiteSourceFileCollection();
				if (FileNode == null)
					return lResult;
				lResult.Add(FileNode);				
				return lResult;
			}
		}
		ISourceFile IElement.FirstFile
		{
			get
			{
				return FileNode;
			}
		}
		ITextRangeCollection IElement.Ranges
		{ 
			get
			{
				if (Range.IsEmpty)
					return EmptyLiteElements.EmptyTextRangeCollection;
				TextRangeCollection lRanges = new TextRangeCollection();
				lRanges.Add(Range);
				return lRanges;
			}
		}
		TextRange IElement.FirstRange
		{
			get
			{
				return Range;
			}
		}
		ITextRangeCollection IElement.NameRanges
		{ 
			get
			{
				if (NameRange.IsEmpty)
					return ((IElement)this).Ranges;
				TextRangeCollection lNameRanges = new TextRangeCollection();
				lNameRanges.Add(NameRange);
				return lNameRanges;
			}
		}
		TextRange IElement.FirstNameRange
		{
			get
			{
				if (NameRange.IsEmpty)
					return ((IElement)this).FirstRange;
				return NameRange;
			}
		}
		IElement IElement.NextSibling 
		{ 
			get
			{
				return NextSibling;
			}
		}
		IElement IElement.PreviousSibling 
		{ 
			get
			{
				return PreviousSibling;
			}				 
		}
		string IElement.RootNamespaceFullName
		{
			get
			{
				return RootNamespaceLocation;
			}
		}
		bool IElement.InReferencedAssembly 
		{
			get
			{
		return StructuralParserServicesHolder.GetAssemblyModel(this) != null;
			}
		}
		bool IElement.IsMember
		{
			get
			{
				return (this is IMemberElement && !(this is ITypeElement));
			}
		}
		bool IElement.IsNestedType 
		{
			get
			{
				return (this is ITypeElement) && GetParentClassInterfaceStructOrModule() != null;
			}
		}
		int IElement.ImageIndex
		{
			get
			{
				return GetImageIndex();
			}
		}
		#endregion
		#region IElementModifier Members
		void IElementModifier.SetParent(IElement parent)
		{
			LanguageElement lParent = parent as LanguageElement;
			SetParent(lParent);
		}
		void IElementModifier.ReplaceChild(IElement oldNode, IElement newNode)
		{
	  ReplaceChildElement(oldNode as LanguageElement, newNode as LanguageElement);
		}
	void IElementModifier.SetFakeNode(bool isFakeNode)
	{
	  SetFakeFlag(isFakeNode);
	}
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator()
		{			
			return new SourceTreeEnumerator(this);
		}	
		#endregion
	}
}
