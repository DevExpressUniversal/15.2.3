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
  public class CssNumberLiteral : CssTerm, ICssNumberLiteral
  {
	object _Value;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  CssNumberLiteral cssNumberLiteral = source as CssNumberLiteral;
	  if (cssNumberLiteral == null)
		return;
	  Value = cssNumberLiteral.Value;
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
	  CssNumberLiteral clone = new CssNumberLiteral();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	#endregion
	public override string ToString()
	{
	  if (Value == null)
		return base.ToString();
	  else if (Value is string)
		return (Value as string).Trim();
	  else if (Value is decimal)
		return Value.ToString();
	  return base.ToString();
	}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.CssNumberLiteral;
	  }
	}
	public virtual object Value {
		get {
			return _Value;
		}
		set {
			_Value = value;
		}
	}
  }
}
