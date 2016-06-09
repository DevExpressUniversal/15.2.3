﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
  public class CssAttributeSelector : CssSelector, ICssAttributeSelector
  {
	bool _IsStringValue;
	string _Value = string.Empty;
	AttributeSelectorEqualityType _EqualityType = AttributeSelectorEqualityType.None;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  CssAttributeSelector cssAttributeSelector = source as CssAttributeSelector;
	  if (cssAttributeSelector == null)
		return;
	  _Value = cssAttributeSelector._Value;
	  _EqualityType = cssAttributeSelector._EqualityType;
	  IsStringValue = cssAttributeSelector.IsStringValue;
	}
	#endregion
	#region Clone
	public override BaseElement Clone()
	{
	  return Clone(ElementCloneOptions.Default);
	}
	#endregion
	#region Clone(ElementCloneOptions options)
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  CssAttributeSelector clone = new CssAttributeSelector();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	#endregion
	public override CssSelectorType SelectorType
	{
	  get
	  {
		return CssSelectorType.Attrib;
	  }
	}
	public string Value
	{
	  get
	  {
		return _Value;
	  }
	  set
	  {
		_Value = value;
	  }
	}
	public AttributeSelectorEqualityType EqualityType
	{
	  get
	  {
		return _EqualityType;
	  }
	  set
	  {
		_EqualityType = value;
	  }
	}
	public bool IsStringValue {
		get {
			return _IsStringValue;
		}
		set {
			_IsStringValue = value;
		}
	}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.CssAttribute;
	  }
	}
  }
}
