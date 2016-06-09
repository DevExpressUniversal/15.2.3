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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class SiblingHelper
  {
	IElement _Start;
	Dictionary<IElement, List<IElement>> _ElementsCache;
	Dictionary<IElement, int> _ElementsIndexCache;
	public SiblingHelper(IElement start)
	{
	  if (start == null)
		throw new ArgumentNullException("start");
	  SetStart(start);
	}
	void SetStart(IElement start)
	{
	  if (_Start == start || start == null)
		return;
	  start = GetParentMember(start);
	  if (start == null)
		return;
	  if (_Start != null && _Start == start)
		return;
	  _Start = start;
	  _ElementsCache = null;
	  _ElementsIndexCache = null;
	}
	void BuildElements()
	{
	  if (_Start == null)
		return;
	  _ElementsCache = new Dictionary<IElement, List<IElement>>();
	  _ElementsIndexCache = new Dictionary<IElement, int>();
	  BuildElements(_Start, _Start.AllChildren, _ElementsCache);
	}
	void BuildElements(IElement parent, IEnumerable<IElement> children, Dictionary<IElement, List<IElement>> elements)
	{
	  if (children == null || elements == null)
		return;
	  List<IElement> coll = new List<IElement>();
	  coll.Add(parent);
	  foreach (IElement child in children)
	  {
		try
		{
					if (_ElementsIndexCache.ContainsKey(child))
						continue;
		  _ElementsIndexCache.Add(child, coll.Count);
		  elements.Add(child, coll);
		  coll.Add(child);
		}
		catch (ArgumentException argEx)
		{
		  System.Diagnostics.Debug.WriteLine(argEx.Message);
		  continue;
		}
		catch (Exception ex)
		{
		  System.Diagnostics.Debug.WriteLine(ex.Message);
		  continue;
		}
		BuildElements(child, child.AllChildren, elements);
	  }
	}
	IElement GetPrevious(IElement element, ref bool isSibling)
	{
	  if (element == null)
		return null;
	  if (!_ElementsCache.ContainsKey(element))
		return null;
	  List<IElement> coll = _ElementsCache[element];
	  int innerIndex = _ElementsIndexCache[element];
	  if (innerIndex <= 0)
		return null;
	  if (innerIndex == 1)
		isSibling = false;
	  return coll[innerIndex - 1];
	}
	public IElement GetPreviousElement(IElement element, out bool isSibling)
	{
	  isSibling = true;
	  if (element == null)
		return null;
	  if (element == null)
		return null;
	  if (_ElementsCache == null)
		BuildElements();
	  if (_ElementsCache == null)
		return null;
	  return GetPrevious(element, ref isSibling);
	}
	public static IElement GetParentMember(IElement start)
	{
	  while (start != null && !(start is IMemberElement) && !(start is IBaseVariable))
		start = start.Parent;
	  if (start == null)
		return null;
	  return start;
	}
  }
}
