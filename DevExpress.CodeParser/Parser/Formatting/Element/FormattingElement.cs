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
using System.ComponentModel;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class FormattingElement : IFormattingElement
  {
	int _CountConsecutive = 1;
	FormattingElementType _Type;
	public FormattingElement(FormattingElementType type)
	{
	  Type = type;
	}
	public FormattingElement(FormattingElementType type, int countConsecutive)
	  : this(type)
	{
	  _CountConsecutive = countConsecutive;
	}
	public FormattingElementType Type
	{
	  get { return _Type; }
	  set { _Type = value; }
	}
	public int CountConsecutive
	{
	  get { return _CountConsecutive; }
	  internal set { _CountConsecutive = value; }
	}
	public bool IsEol()
	{
	  return Type == FormattingElementType.EOL || Type == FormattingElementType.CR;
	}
	public bool IsWsOrTab()
	{
	  return Type == FormattingElementType.WS || Type == FormattingElementType.Tab;
	}
	public bool IsIndent()
	{
	  return Type == FormattingElementType.IncreaseIndent || Type == FormattingElementType.DecreaseIndent;
	}
	public object Clone()
	{
	  FormattingElement result = new FormattingElement(Type, CountConsecutive);
	  return result;
	}
	public override string ToString()
	{
	  if (CountConsecutive == 1)
		return Type.ToString();
	  return string.Format("{0} (Count consecutive: {1}", Type, CountConsecutive);
	}
  }
}
