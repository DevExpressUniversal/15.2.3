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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{	
	public class ChildrenEnumerator : IEnumerator
	{
		IElementFilter _Filter;
		bool _IsInStart;
		LanguageElement _StartNode;
		LanguageElement _CurrentNode;
		public ChildrenEnumerator(LanguageElement start)
			: this(start, null)
		{
		}
		public ChildrenEnumerator(LanguageElement start, IElementFilter filter)
		{
			if (start == null)
				throw new ArgumentNullException("start");
			_Filter = filter;
			_IsInStart = true;
			_StartNode = start;			
			_CurrentNode = null;
		}
		bool ApplyFilter(LanguageElement node)
		{
			if (node == null)
				return false;
			return _Filter == null || _Filter.Apply(node);
		}
		LanguageElement GetNextNode(LanguageElement node)
		{
			if (node == null)
				return null;
			LanguageElement lNode = node.NextSibling;			
			if (lNode != null)			
				return lNode;				
			LanguageElement lParent = node.Parent;
			if (lParent != null && node.IsDetailNode)
			{				 
				lNode = lParent.FirstChild;
				if (lNode != null)
					return lNode;							 
			}
			return null;
		}
		LanguageElement GetFirstChild(LanguageElement node)
		{
			LanguageElement result = node.FirstDetail;
			if (result == null)
				result = node.FirstChild;
			return result;			
		}
		LanguageElement GetStartNode(LanguageElement node)
		{
			if (node == null)
				return null;			
			LanguageElement lCurrent = GetFirstChild(node);			
			if (lCurrent == null)
				return null;
			while (lCurrent != null)
			{
				if (ApplyFilter(lCurrent))
					return lCurrent;
				lCurrent = GetNextNode(lCurrent);				
			}
			return null;
		}
		bool GetNextFilteredNode(ref LanguageElement node)
		{
			LanguageElement lCurrent = node;			
			while (true)
			{			
				lCurrent = GetNextNode(lCurrent);
				if (lCurrent == null)
					return false;
				if (!ApplyFilter(lCurrent))
					continue;				
				node = lCurrent;
				return true;								
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
				_CurrentNode = GetStartNode(_StartNode);
				_IsInStart = false;
				return _CurrentNode != null;
			}
			return GetNextFilteredNode(ref _CurrentNode);
		}
		#endregion
		#region Current
		public object Current
		{
			get
			{
				return _CurrentNode;
			}
		}
		#endregion
	}
}
