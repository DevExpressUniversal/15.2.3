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
namespace DevExpress.CodeRush.StructuralParser.CodeStyle.Formatting
#else
namespace DevExpress.CodeParser.CodeStyle.Formatting
#endif
{
  abstract class WrapBase : WrappingAlignmentBaseRule
  {
	string _AlignRuleName;
	string _WrapRuleName;
	public WrapBase(string alignRuleName, string wrapRuleName)
	{
	  _AlignRuleName = alignRuleName;
	  _WrapRuleName = wrapRuleName;
	}
	protected override void ValueChanged(object oldValue)
	{
	  if (Enabled || ParentCollection == null)
		return;
	  if (!string.IsNullOrEmpty(_WrapRuleName))
	  {
		BaseFormattingRule wrapRule = ParentCollection[_WrapRuleName];
		if (wrapRule != null && wrapRule.Enabled)
		  return;
	  }
	  if (!string.IsNullOrEmpty(_AlignRuleName))
	  {
		BaseFormattingRule alignRule = ParentCollection[_AlignRuleName];
		if (alignRule != null)
		  alignRule.Value = false;
	  }
	}
  }
  abstract class AlignBase : WrappingAlignmentBaseRule
  {
	string _WrapRuleName;
	string _WrapFirstRuleName;
	public AlignBase(string wrapRuleName, string wrapFirstRuleName)
	{
	  _WrapFirstRuleName = wrapFirstRuleName;
	  _WrapRuleName = wrapRuleName;
	}
	protected override void ValueChanged(object oldValue)
	{
	  if (!Enabled || ParentCollection == null)
		return;
	  BaseFormattingRule wrapFirstRule = null;
	  if (!string.IsNullOrEmpty(_WrapFirstRuleName))
	  {
		wrapFirstRule = ParentCollection[_WrapFirstRuleName];
		if (wrapFirstRule != null && wrapFirstRule.Enabled)
		  return;
	  }
	  if (!string.IsNullOrEmpty(_WrapRuleName))
	  {
		BaseFormattingRule wrapRule = ParentCollection[_WrapRuleName];
		if (wrapRule != null)
		{
		  if (wrapRule.Enabled)
			return;
		  wrapRule.Value = true;
		  return;
		}
	  }
	  if (wrapFirstRule != null)
		wrapFirstRule.Value = true;
	}
  }
}
