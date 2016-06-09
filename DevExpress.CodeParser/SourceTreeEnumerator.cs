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
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class SourceTreeEnumerator : IEnumerator
	{
		IElementFilter _Filter;
		LanguageElement _StartNodeParent;
		LanguageElement _StartNode;
	LanguageElement _StopNode;
		LanguageElement _CurrentNode;
		bool _IsInStart;
		bool _SkipScope;
	Hashtable _IteratedNodes;
		public SourceTreeEnumerator(LanguageElement start)
			: this(start, null)
		{
		}
		public SourceTreeEnumerator(LanguageElement start, IElementFilter filter)
		{
			if (start == null)
				throw new ArgumentNullException("start");
			_Filter = filter;
			_IsInStart = true;
			_SkipScope = false;
			_StartNode = start;
	  _StopNode = DefineStopNode(start);
			_StartNodeParent = _StartNode.Parent;
			_CurrentNode = null;
		}
	public SourceTreeEnumerator(LanguageElement scope, LanguageElement start, IElementFilter filter)
	{
	  if (scope == null)
		throw new ArgumentNullException("scope");
	  if (start == null)
		throw new ArgumentNullException("start");
	  _Filter = filter;
	  _IsInStart = true;
	  _SkipScope = false;
	  _StartNode = start;
	  _StopNode = DefineStopNode(scope);
	  _StartNodeParent = scope.Parent;
	  _CurrentNode = null;
	}
	LanguageElement GetLastChild(LanguageElement node)
	{
	  if (node == null)
		return null;
	  if (node.NodeCount == 0)
		return node.LastDetail;	  
	  return node.LastChild;
	}
	LanguageElement GetDeepestChild(LanguageElement node)
	{
	  if (node == null)
		return null;
	  LanguageElement result = GetLastChild(node);
	  if (result == null)
		return null;
	  while (true)
	  {
		LanguageElement last = GetLastChild(result);
		if (last == null || last == result)
		  break;
		result = last;
	  }
	  return result;
	}
	LanguageElement DefineStopNode(LanguageElement node)
	{
	  if (node == null)
		return null;
	  LanguageElement dippest = GetDeepestChild(node);
	  if (dippest == null)
		dippest = node;
	  return dippest.NextNode;
	}
		bool IsIterationCompleted(LanguageElement node)
		{
	  return node == null || node == _StopNode || _StartNodeParent == node.Parent;
		}
		bool ApplyFilter(LanguageElement node)
		{
			return _Filter == null || _Filter.Apply(node);
		}
		LanguageElement GetFirstChild(LanguageElement node)
		{
			LanguageElement result = node.FirstDetail;
			if (result == null)
				result = node.FirstChild;
			return result;			
		}
		LanguageElement GetStartNode(LanguageElement node, bool skipStart)
		{
			if (node == null)
				return null;			
			LanguageElement lCurrent = node;
			if (skipStart)
				lCurrent = GetFirstChild(node);
			if (lCurrent == null)
				return null;
			while (true)
			{
				if (ApplyFilter(lCurrent))
					return lCurrent;
				lCurrent = lCurrent.NextNode;
				if (IsIterationCompleted(lCurrent))
					break;				
			}
			return null;
		}
	bool GetNextNode(ref LanguageElement node)
	{
	  bool result = GetNextNodeInternal(ref node);
	  if (IteratedNodes.ContainsKey(node))
	  {		
		_IteratedNodes = null;
		return false;
	  }
	  IteratedNodes.Add(node, node);
	  return result;
	}	
		bool GetNextNodeInternal(ref LanguageElement node)
		{
			if (node == null)
				return false;
			LanguageElement lNode = null;
			lNode = node.FirstDetail;
			if (lNode != null)
			{
				node = lNode;
				return true;
			}
			lNode = node.FirstChild;
			if (lNode != null)
			{
				node = lNode;
				return true;
			}
			lNode = node.NextSibling;
			if (lNode != null)
			{
				node = lNode;
				return !IsIterationCompleted(node);
			}
			LanguageElement lParent = node.Parent;
			if (node.IsDetailNode)
			{
				if (lParent != null) 
				{ 
					lNode = lParent.FirstChild;
					if (lNode != null)
					{
						node = lNode;
						return !IsIterationCompleted(node);
					}
				} 
			} 
			lParent = node;
			do
			{ 
				lParent = lParent.Parent; 
				if (lParent == null) 
					return false;
				if (lParent == _StartNodeParent)
					return false;
				lNode = lParent.NextSibling;
				if (lNode == null && lParent.IsDetailNode && lParent.Parent != null)
					lNode = lParent.Parent.FirstChild;
			} 
			while (lNode == null); 
			node = lNode;
			return !IsIterationCompleted(node);
		}
		bool GetNextFilteredNode(ref LanguageElement node)
		{
			LanguageElement lCurrent = node;			
			while (GetNextNode(ref lCurrent))
			{				
				if (!ApplyFilter(lCurrent))
					continue;				
				node = lCurrent;
				return true;								
			}
			return false;
		}
	Hashtable IteratedNodes
	{
	  get
	  {
		if (_IteratedNodes == null)
		  _IteratedNodes = new Hashtable();
		return _IteratedNodes;
	  }
	}
		#region Reset
		public void Reset()
		{
			_IsInStart = true;
			_CurrentNode = null;
		}
		#endregion
		#region MoveNext
		public bool MoveNext()
		{
			 if (_IsInStart)
			{
				 _CurrentNode = GetStartNode(_StartNode, _SkipScope);
				_IsInStart = false;
				return _CurrentNode != null;
			}
			return GetNextFilteredNode(ref _CurrentNode);
		}
		#endregion
		bool HasFilter
		{
			get
			{
				return _Filter != null;
			}
		}
		IElementFilter Filter
		{
			get
			{
				return _Filter;
			}
		}
		#region Current
		public object Current
		{
			get
			{
				return _CurrentNode;
			}
		}
		#endregion
		public bool SkipScope
		{
			get
			{
				return _SkipScope;
			}
			set
			{
				_SkipScope = value;
			}
		}
	}
}
