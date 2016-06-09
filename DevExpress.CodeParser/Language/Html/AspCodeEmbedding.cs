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
  public class RazorFunctions : AspCodeEmbedding,IMarkupElement
  {
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  RazorFunctions lClone = new RazorFunctions();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	public override bool IsFunctionsEmbedding
	{
	  get
	  {
		return true;
	  }
	}
	public override MemberVisibility[] GetValidVisibilities()
	{
	  return new MemberVisibility[] 
		{
		  MemberVisibility.Public,
		  MemberVisibility.Internal,
		  MemberVisibility.ProtectedInternal,
		  MemberVisibility.Protected,
		  MemberVisibility.Private
		};
	}
  }
  public class RazorSection : AspCodeEmbedding
  {
	DotNetLanguageType _DotNetLanguageType;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is RazorSection))
		return;
	  RazorSection lSource = (RazorSection)source;
	  _DotNetLanguageType = lSource._DotNetLanguageType;
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  RazorSection lClone = new RazorSection();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.RazorSectionElement;
	  }
	}
	#endregion
	public DotNetLanguageType DotNetLanguageType
	{
	  get
	  {
		return _DotNetLanguageType;
	  }
	  set
	  {
		_DotNetLanguageType = value;
	  }
	}
  }
  public class RazorHelper : AspCodeEmbedding
  {
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  RazorHelper lClone = new RazorHelper();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	public override MemberVisibility[] GetValidVisibilities()
	{
	  return EmptyVisibilityArray.Value;
	}
  }
  public class AspCodeEmbedding : HtmlElement, IAspCodeEmbedding
  {
	string _Code;
		string _CodeToParse;
		Token _CodeEmbeddingToken;
	SourceRange _CodeRange = SourceRange.Empty;
	bool _IsRazorEmbedding;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AspCodeEmbedding))
		return;
	  AspCodeEmbedding lSource = (AspCodeEmbedding)source;
	  _Code = lSource._Code;
	  _CodeRange = lSource.CodeRange;
			_CodeEmbeddingToken = lSource._CodeEmbeddingToken;
	  _IsRazorEmbedding = lSource._IsRazorEmbedding;
	}
	#endregion
	protected override void UpdateRanges()
	{
	  base.UpdateRanges();
	  _CodeRange = CodeRange;
	}
	public AspCodeEmbedding()
	{
	}
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AspCodeEmbedding lClone = new AspCodeEmbedding();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
		internal Token CodeEmbeddingToken
		{
			get { return _CodeEmbeddingToken; }
			set { _CodeEmbeddingToken = value; }
		}
		internal string CodeToParse
		{
			get
			{
				if (String.IsNullOrEmpty(_CodeToParse))
					return _Code;
				else
					return _CodeToParse;
			}
			set
			{
				_CodeToParse = value;
			}
		}
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.AspCodeEmbedding;
	  }
	}
	#endregion
	#region GetImageIndex
	public override int GetImageIndex()
	{
	  return ImageIndex.AspCodeNugget;
	}
	#endregion
	#region Code
	public string Code
	{
	  get
	  {
		return _Code;
	  }
	  set
	  {
		_Code = value;
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
	public bool IsRazorEmbedding
	{
	  get { return _IsRazorEmbedding; }
	  set { _IsRazorEmbedding = value; }
	}
	public virtual bool IsFunctionsEmbedding
	{
	  get { return false; }
	}
  }
}
