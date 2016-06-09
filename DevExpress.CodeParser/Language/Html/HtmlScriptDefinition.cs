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
  public class HtmlScriptDefinition : HtmlElement, IHtmlScriptDefinition, IMarkupElement
  {
	string _ScriptText;
	DotNetLanguageType _Language = DotNetLanguageType.Unknown;
	SourceRange _CodeRange = SourceRange.Empty;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is HtmlScriptDefinition))
		return;
	  HtmlScriptDefinition lSource = (HtmlScriptDefinition)source;
	  _ScriptText = lSource._ScriptText;
	  _Language = lSource._Language;
	  if (lSource._CodeRange != SourceRange.Empty)
		_CodeRange = lSource.CodeRange;
	  else
		_CodeRange = SourceRange.Empty;
	}
	#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _CodeRange = CodeRange;
	}
	#region GetImageIndex
	public override int GetImageIndex()
	{
	  return ImageIndex.HtmlScript;
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  HtmlScriptDefinition lClone = new HtmlScriptDefinition();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
		public override MemberVisibility GetDefaultVisibility()
		{
			return MemberVisibility.Private;
		}
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.HtmlScript;
	  }
	}
	#endregion
	#region ScriptText
	public string ScriptText
	{
	  get
	  {
		return _ScriptText;
	  }
	  set
	  {
		_ScriptText = value;
	  }
	}
	#endregion
	#region Language
	public DotNetLanguageType Language
	{
	  get
	  {
		return _Language;
	  }
	  set
	  {
		_Language = value;
	  }
	}
	#endregion
	#region CodeRange
	public SourceRange CodeRange
	{
	  get
	  {
		return GetTransformedRange(_CodeRange);
	  }
	  set
	  {
		ClearHistory();
		_CodeRange = value;
	  }
	}
	#endregion
	#region ScriptText
	string IHtmlScriptDefinition.ScriptText
	{
	  get
	  {
		return _ScriptText;
	  }
	}
	#endregion
	#region Language
	DotNetLanguageType IHtmlScriptDefinition.Language
	{
	  get
	  {
		return _Language;
	  }
	}
	#endregion
	#region CodeRange
	TextRange IHtmlScriptDefinition.CodeRange
	{
	  get
	  {
		return _CodeRange;
	  }
	}
	#endregion
  }
}
