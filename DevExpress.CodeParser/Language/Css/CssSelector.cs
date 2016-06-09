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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class CssSelector : BaseCssElement, ICssSelector
  {
	CssSelector _ChildSelector;
	CssSelectorType _SelectorType;
	public CssSelector()
	{
	  _SelectorType = CssSelectorType.Name;
	}
	#region Clone
	public override BaseElement Clone()
	{
	  return Clone(ElementCloneOptions.Default);
	}
	#endregion
	#region Clone(ElementCloneOptions options)
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  CssSelector clone = new CssSelector();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	#endregion
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  CssSelector cssSelector = source as CssSelector;
	  if (cssSelector == null)
		return;
	  _SelectorType = cssSelector._SelectorType;
	  if (cssSelector.ChildSelector != null)
	  {
		ChildSelector = ParserUtils.GetCloneFromNodes(this, cssSelector, cssSelector.ChildSelector) as CssSelector;
		if (ChildSelector == null)
		  ChildSelector = cssSelector.ChildSelector.Clone() as CssSelector;
	  }
	  else
		ChildSelector = null;
	  if (ChildSelector != null)
		ChildSelector.SetParent(this);
	}
	#endregion
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.CssSelector;
	  }
	}
	public virtual CssSelectorType SelectorType
	{
	  get
	  {
		return _SelectorType;
	  }
	  set
	  {
		_SelectorType = value;
	  }
	}
	public CssSelector Ancestor 
	{
	  get
	  {
		return Parent as CssSelector;
	  }
	}
	public CssSelector ChildSelector {
		get {
			return _ChildSelector;
		}
		set {
			_ChildSelector = value;
		}
	}
	ICssSelector ICssSelector.Ancestor
	{
	  get
	  {
		return Ancestor;
	  }
	}
	ICssSelector ICssSelector.ChildSelector
	{
	  get
	  {
		return ChildSelector;
	  }
	}
  }
}
