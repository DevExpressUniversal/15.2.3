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
	[Flags]
	public enum ElementCollectorFlags : int
	{
		Recursive = 0x01,
		CheckRootNode = 0x02,
		IterateChildren = 0x04,
	}
	public sealed class ElementCollector
	{
		ElementCollector()
		{
		}
		static ElementCollectorFlags DirectChildren
		{
			get
			{
				return ElementCollectorFlags.IterateChildren;
			}
		}
		static ElementCollectorFlags AllChildren
		{
			get
			{
				return DirectChildren | ElementCollectorFlags.Recursive;
			}
		}
		static ElementCollectorFlags AllChildrenWithScopeCheck
		{
			get
			{
				return AllChildren | ElementCollectorFlags.CheckRootNode;
			}
		}		
		static bool IsDefined(ElementCollectorFlags flags, ElementCollectorFlags value)
		{
			return (flags & value) == value;
		}
		static void Collect(IElementCollection collector, IElement scope, IElementFilter filter)
		{
			if (collector == null || scope == null || filter == null)
				return;
			if (filter.Apply(scope))
				collector.Add(scope);
			IElementCollection lChildren = scope.Children;
			if (lChildren == null)
				return;
			int lCount = lChildren.Count;
			for (int i = 0; i < lCount; i++)
				Collect(collector, lChildren[i], filter);
		}
		public static IElementCollection Collect(IElement scope, IElementFilter filter)
		{
			IElementCollection lResult = new IElementCollection();
			if (scope == null)
				return lResult;
			Collect(lResult, scope, filter);
			return lResult;
		}
		public static IElementCollection CollectDirectChildren(IElement scope, IElementFilter filter)
		{
			IElementCollection lResult = new IElementCollection();
			CollectElements(lResult, scope, filter, DirectChildren);
			return lResult;
		}
		public static IElementCollection CollectDirectChildren(IElementCollection scopeList, IElementFilter filter)
		{
			IElementCollection lResult = new IElementCollection();
			if (scopeList == null)
				return lResult;
			for (int i = 0; i < scopeList.Count; i++)
			{
				IElement lScope = scopeList[i];
				CollectElements(lResult, lScope, filter, DirectChildren);
			}
			return lResult;
		}
		public static IElementCollection CollectAllChildren(IElement scope, IElementFilter filter)
		{
			IElementCollection lResult = new IElementCollection();
			CollectElements(lResult, scope, filter, AllChildren);
			return lResult;
		}
		public static IElementCollection CollectElements(IElementCollection list, IElementFilter filter)
		{
			return Filter(list, filter);
		}		
		public static void CollectElements(IElementCollection list, IElementCollection nodes, IElementFilter filter, ElementCollectorFlags flags)
		{
			if (list == null || nodes == null || filter == null)
				return;
			for (int i = 0; i < nodes.Count; i++)
			{
				IElement lElement = nodes[i];
				if (lElement == null)
					continue;
				if (filter.Apply(lElement))
				{
					list.Add(lElement);
					if (IsDefined(flags, ElementCollectorFlags.Recursive))
						CollectElements(list, lElement, filter, flags & ~ElementCollectorFlags.CheckRootNode);
				}
			}
		}
		public static void CollectElements(IElementCollection list, IElement scope, IElementFilter filter, ElementCollectorFlags flags)
		{
			if (list == null || scope == null || filter == null)
				return;
			if (IsDefined(flags, ElementCollectorFlags.CheckRootNode) && filter.Apply(scope))
				list.Add(scope);
			if (IsDefined(flags, ElementCollectorFlags.IterateChildren))
				CollectElements(list, scope.Children, filter, flags);
		}
		public static IElementCollection Filter(ICollection list, IElementFilter filter)
		{
			if (list == null || list.Count == 0)
				return null;
			IElementCollection result = new IElementCollection();
			foreach (object obj in list)
			{
				IElement element = obj as IElement;
				if (element == null)
					continue;
				if (filter.Apply(element))
					result.Add(element);
			}
			return result;
		}
	}
}
