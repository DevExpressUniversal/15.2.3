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
  public class CssStyleRule : CssPageStyle, ICssStyleRule, ICapableBlock
  {
	CssSelectorCollection _Selectors;
	TextRangeWrapper _BlockStart;
	TextRangeWrapper _BlockEnd;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  CssStyleRule cssStyleRule = source as CssStyleRule;
	  if (cssStyleRule == null)
		return;
	  _Selectors = cssStyleRule.Selectors.DeepClone(options) as CssSelectorCollection;
	  if (_Selectors == null)
		return;
	  foreach (LanguageElement element in _Selectors)
		element.SetParent(this);
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
	  CssStyleRule clone = new CssStyleRule();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	#endregion
	public void AddSelector(CssSelector selector)
	{
	  if (selector == null)
		return;
	  Selectors.Add(selector);
	  selector.SetParent(this);
	}
	public override string ToString()
	{
	  return "CssStyleRule";
	}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.CssStyleRule;
	  }
	}
	public CssSelectorCollection Selectors
	{
	  get
	  {
		if (_Selectors == null)
		  _Selectors = new CssSelectorCollection();
		return _Selectors;
	  }
	}
	public int SelectorCount
	{
	  get
	  {
		if (_Selectors == null)
		  return 0;
		return _Selectors.Count;
	  }
	}
	#region ICssStyleRule Members
	ICssSelectorCollection ICssStyleRule.Selectors
	{
	  get
	  {
		return Selectors;
	  }
	}
	#endregion
	#region ICapableBlock Members
	public DelimiterBlockType BlockType
	{
	  get
	  {
		return DelimiterBlockType.Brace;
	  }
	}
	public bool HasDelimitedBlock
	{
	  get
	  {
		return true;
	  }
	}
	#endregion
	#region IHasBlock Members
	public SourceRange BlockStart
	{
	  get
	  {
		return GetTransformedRange(_BlockStart);
	  }
	}
	public SourceRange BlockEnd
	{
	  get
	  {
		return GetTransformedRange(_BlockEnd);
	  }
	}
	public void SetBlockStart(SourceRange blockStart)
	{
	  ClearHistory();
	  _BlockStart = blockStart;
	}
	public void SetBlockEnd(SourceRange blockEnd)
	{
	  ClearHistory();
	  _BlockEnd = blockEnd;
	}
	#endregion
  }
}
