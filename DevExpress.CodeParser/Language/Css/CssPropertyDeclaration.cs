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
  public class CssPropertyDeclaration : BaseCssElement, ICssPropertyDeclaration
  {
	bool _IsEqual;
	bool _IsImportant;
	CssTermCollection _Values;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  CssPropertyDeclaration cssPropertyDeclaration = source as CssPropertyDeclaration;
	  if (cssPropertyDeclaration == null)
		return;
	  IsEqual = cssPropertyDeclaration.IsEqual;
	  IsImportant = cssPropertyDeclaration.IsImportant;
	  if (cssPropertyDeclaration._Values == null)
		return;
	  foreach (LanguageElement element in cssPropertyDeclaration._Values)
		AddValue(element.Clone(options) as CssTerm);
	}
	#endregion
	public void AddValue(CssTerm cssTerm)
	{
	  if (cssTerm == null)
		return;
	  Values.Add(cssTerm);
	  cssTerm.SetParent(this);
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
	  CssPropertyDeclaration clone = new CssPropertyDeclaration();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	#endregion
	public bool IsEqual {
		get {
			return _IsEqual;
		}
		set {
			_IsEqual = value;
		}
	}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.CssPropertyDeclaration;
	  }
	}
	public CssTermCollection Values
	{
	  get
	  {
		if (_Values == null)
		  _Values = new CssTermCollection();
		return _Values;
	  }
	}
	public int ValuesCount
	{
	  get
	  {
		if (_Values == null)
		  return 0;
		return _Values.Count;
	  }
	}
	public bool IsImportant {
		get {
			return _IsImportant;
		}
		set {
			_IsImportant = value;
		}
	}
	#region ICssPropertyDeclaration Members
	ICssTermCollection ICssPropertyDeclaration.Values
	{
	  get
	  {
		return Values;
	  }
	}
	#endregion
  }
}
