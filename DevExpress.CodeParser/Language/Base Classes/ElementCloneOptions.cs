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
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class ElementCloneOptions
	{
		bool _CloneUpToMembers;
		bool _KeepAccessSpecifierTemplate;
		bool _PerformParseBeforeClone;
		bool _CloneNodes;
	bool _CloneRegions;
	SourceFile _SourceFile;
	int _CloneChildrenCount;
	Dictionary<LanguageElement, LanguageElement> _ClonedElements;
		ITypeReferenceExpression _TypeReferencePrototype;
		public ElementCloneOptions()
		{
			ResetToDefaults();
		}
	internal void AddClonedElement(LanguageElement sourceElement, LanguageElement clonedElement)
	{
	  if (sourceElement == null || clonedElement == null)
		return;
	  if (_ClonedElements == null)
		return;
	  _ClonedElements[sourceElement] = clonedElement;
	}
	internal LanguageElement GetClonedElement(LanguageElement sourceElement)
	{
	  LanguageElement result = null;
	  if (_ClonedElements != null && _ClonedElements.TryGetValue(sourceElement, out result))
		return result;
	  return null;
	}
	internal bool BeginChildrenCloning()
	{
	  _CloneChildrenCount++;
	  return _CloneChildrenCount == 1;
	}
	internal bool EndChildrenCloning()
	{
	  _CloneChildrenCount--;
	  return (_CloneChildrenCount == 0);
	}
		public virtual bool NeedToCloneNodes(BaseElement element)
		{
			if (!_CloneNodes)
				return false;
			LanguageElement lElement = element as LanguageElement;
			if (lElement != null && CloneUpToMembers)
			{
				return lElement.ElementType != LanguageElementType.PropertyAccessorGet &&
					lElement.ElementType != LanguageElementType.PropertyAccessorSet && 
					lElement.ElementType != LanguageElementType.EventAdd &&
					lElement.ElementType != LanguageElementType.EventRemove && 
					lElement.ElementType != LanguageElementType.Method;
			}
			return true;
		}
		public void ResetToDefaults()
		{
			_CloneUpToMembers = false;
			_PerformParseBeforeClone = false;
			_CloneNodes = true;
	  _CloneRegions = false;
	  _ClonedElements = new Dictionary<LanguageElement, LanguageElement>();
		}
	internal bool CloningChildren
	{
	  get
	  {
		return _CloneChildrenCount > 0;
	  }
	}
	public static ElementCloneOptions Default
	{
	  get
	  {
		return new ElementCloneOptions();
	  }
	}
		public bool CloneNodes
		{
			get
			{
				return _CloneNodes;
			}
			set
			{
				_CloneNodes = value;
			}
		}
	public bool CloneRegions
		{
			get
			{
		return _CloneRegions;
			}
			set
			{
		_CloneRegions = value;
			}
		}	
		public bool CloneUpToMembers
		{
			get
			{
				return _CloneUpToMembers;
			}
			set
			{
				_CloneUpToMembers = value;
			}
		}
	public bool KeepAccessSpecifierTemplate
	{
	  get
	  {
		return _KeepAccessSpecifierTemplate;
	  }
	  set
	  {
		_KeepAccessSpecifierTemplate = value;
	  }
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("Use KeepAccessSpecifierTemplate")]
		public bool KeepAsseccSpecifierTemplate
		{
			get
			{
				return KeepAccessSpecifierTemplate;
			}
			set
			{
				KeepAccessSpecifierTemplate = value;
			}
		}
		public bool PerformParseBeforeClone
		{
			get
			{
				return _PerformParseBeforeClone;
			}
			set
			{
				_PerformParseBeforeClone = value;
			}
		}
		public ITypeReferenceExpression TypeReferencePrototype
		{
			get
			{
				return _TypeReferencePrototype;
			}
			set
			{
				_TypeReferencePrototype = value;
			}
		}
	public SourceFile SourceFile
	{
	  get
	  {
		return _SourceFile;
	  }
	  set
	  {
		_SourceFile = value;
	  }
	}
	}
}
