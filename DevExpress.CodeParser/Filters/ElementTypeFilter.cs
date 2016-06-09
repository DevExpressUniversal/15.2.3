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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ElementTypeFilter : ElementFilterBase
	{
		LanguageElementType[] _Types;
		Type[] _ObjectTypes;
		public ElementTypeFilter(LanguageElementType type)
		{
			_Types = new LanguageElementType[1];
			_Types[0] = type;
		}
		public ElementTypeFilter(LanguageElementType[] types)
		{
			_Types = types;
		}
		public ElementTypeFilter(Type type)
		{
			_ObjectTypes = new Type[1];
			_ObjectTypes[0] = type;
		}
		public ElementTypeFilter(Type[] types)
		{
			_ObjectTypes = types;
		}
		bool FilterByElementType(IElement element)
		{
			if (_Types == null)
				return false;
			LanguageElementType lElementType = element.ElementType;
			int lCount = _Types.Length;
			for (int i = 0; i < lCount; i++)
				if (lElementType == _Types[i])
					return true;
			return false;
		}
		bool FilterByObjectType(IElement element)
		{
			if (_ObjectTypes == null)
				return false;			
			int lCount = _ObjectTypes.Length;
			for (int i = 0; i < lCount; i++)
				if (_ObjectTypes[i].IsInstanceOfType(element))
					return true;
			return false;
		}
		public override bool Apply(IElement element)
		{
			if (element == null)
				return false;
			return FilterByElementType(element) || FilterByObjectType(element);			
		}
	}
}
