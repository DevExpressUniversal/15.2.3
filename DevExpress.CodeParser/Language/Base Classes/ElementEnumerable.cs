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
	public class ElementEnumerable : IEnumerable
	{
		LanguageElement _Scope;		
		IElementFilter _Filter;
		bool _UseRecursion;
		bool _SkipScope;
		public ElementEnumerable(LanguageElement scope)
			: this(scope, false)
		{
		}
		public ElementEnumerable(LanguageElement scope, bool useRecursion)
			: this(scope, (IElementFilter)null, useRecursion)
		{		
		}
		public ElementEnumerable(LanguageElement scope, IElementFilter filter)
			: this(scope, filter, false)
		{
		}
		public ElementEnumerable(LanguageElement scope, IElementFilter filter, bool useRecursion)
		{
			if (scope == null)
				throw new ArgumentNullException("scope");
			_Scope = scope;
			_Filter = filter;
			_UseRecursion = useRecursion;
			_SkipScope = true;
		}				
		public ElementEnumerable(LanguageElement scope, LanguageElementType filterType)
			: this(scope, filterType, false)
		{
		}
		public ElementEnumerable(LanguageElement scope, LanguageElementType filterType, bool useRecursion)
			: this(scope, useRecursion)
		{
			_Filter = GetFilter(filterType);
		}
		public ElementEnumerable(LanguageElement scope, LanguageElementType[] filterTypes)
			: this(scope, filterTypes, false)
		{
		}
		public ElementEnumerable(LanguageElement scope, LanguageElementType[] filterTypes, bool useRecursion)
			: this(scope, useRecursion)
		{	
			_Filter = GetFilter(filterTypes);
		}
		public ElementEnumerable(LanguageElement scope, Type filterType)
			: this(scope, filterType, false)
		{}
		public ElementEnumerable(LanguageElement scope, Type filterType, bool useRecursion)
			: this(scope, useRecursion)
		{			
			_Filter = GetFilter(filterType);
		}
		public ElementEnumerable(LanguageElement scope, Type[] filterTypes)
			: this(scope, filterTypes, false)
		{
		}
		public ElementEnumerable(LanguageElement scope, Type[] filterTypes, bool useRecursion)
			: this(scope, useRecursion)
		{
			_Filter = GetFilter(filterTypes);
		}
		IElementFilter GetFilter(LanguageElementType filterType)
		{					
			return new ElementTypeFilter(filterType);			
		}
		IElementFilter GetFilter(LanguageElementType[] filterTypes)
		{			
			if (filterTypes == null || filterTypes.Length == 0)
				return null;
			return new ElementTypeFilter(filterTypes);			
		}
		IElementFilter GetFilter(Type filterType)
		{			
			if (filterType == null)
				return null;
			return new ElementTypeFilter(filterType);			
		}
		IElementFilter GetFilter(Type[] filterTypes)
		{			
			if (filterTypes == null || filterTypes.Length == 0)
				return null;
			return new ElementTypeFilter(filterTypes);			
		}				
		public IEnumerator GetEnumerator()
		{
			if (_UseRecursion)
			{
				SourceTreeEnumerator result = new SourceTreeEnumerator(_Scope, _Filter);
				result.SkipScope = _SkipScope;
				return result;
			}
			return new ChildrenEnumerator(_Scope, _Filter);
		}
		public IElementFilter Filter
		{
			get
			{
				return _Filter;
			}
		}
		public bool UseRecursion
		{
			get
			{
				return _UseRecursion;
			}
		}
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
