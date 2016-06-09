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
  public class HtmlStyleDefinition : HtmlElement, IHtmlStyleDefinition, IMarkupElement
  {
	string _StyleText;
	SourceRange _TextRange = SourceRange.Empty;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is HtmlStyleDefinition))
		return;
	  HtmlStyleDefinition lSource = (HtmlStyleDefinition)source;
	  _StyleText = lSource._StyleText;
	  if (lSource._TextRange != SourceRange.Empty)
		_TextRange = lSource.TextRange;
	  else
		_TextRange = SourceRange.Empty;
	}
	#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _TextRange = TextRange;
	}
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  HtmlStyleDefinition lClone = new HtmlStyleDefinition();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.HtmlStyle;
	  }
	}
	#endregion
	#region StyleText
	public string StyleText
	{
	  get
	  {
		return _StyleText;
	  }
	  set
	  {
		_StyleText = value;
	  }
	}
	#endregion
	#region TextRange
	public SourceRange TextRange
	{
	  get
	  {
		return GetTransformedRange(_TextRange);
	  }
	  set
	  {
		ClearHistory();
		_TextRange = value;
	  }
	}
	#endregion
	#region StyleText
	string IHtmlStyleDefinition.StyleText
	{
	  get { return _StyleText; }
	}
	#endregion
	#region TextRange
	TextRange IHtmlStyleDefinition.TextRange
	{
	  get
	  {
		return _TextRange;
	  }
	}
	#endregion
  }
}
