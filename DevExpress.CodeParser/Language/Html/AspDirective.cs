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
  public class RazorInheritsDirective : AspDirective,IMarkupElement
  {
	private string _Model;
	private SourceRange _ModelRange;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is RazorInheritsDirective))
		return;
	  RazorInheritsDirective lSource = (RazorInheritsDirective)source;
	  _Model = lSource._Model;
	  _ModelRange = lSource._ModelRange;
	}
	#endregion
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.RazorInheritsDirective;
	  }
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  RazorInheritsDirective lClone = new RazorInheritsDirective();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	public string Model
	{
	  get
	  {
		return _Model;
	  }
	  set
	  {
		_Model = value;
	  }
	}
	public SourceRange ModelRange
	{
	  get
	  {
		return _ModelRange;
	  }
	  set
	  {
		_ModelRange = value;
	  }
	}
  }
  public class RazorModelDirective : AspDirective
  {
	private string _Model;
	private SourceRange _ModelRange;
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is RazorModelDirective))
		return;
	  RazorModelDirective lSource = (RazorModelDirective)source;
	  _Model = lSource._Model;
	  _ModelRange = lSource._ModelRange;
	}
	#endregion
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.RazorModelDirective;
	  }
	}
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  RazorModelDirective lClone = new RazorModelDirective();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	public string Model
	{
	  get
	  {
		return _Model;
	  }
	  set
	  {
		_Model = value;
	  }
	}
	public SourceRange ModelRange
	{
	  get
	  {
		return _ModelRange;
	  }
	  set
	  {
		_ModelRange = value;
	  }
	}
  }
  public class AspDirective : HtmlElement, IAspDirective
  {
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is AspDirective))
		return;
	  AspDirective lSource = (AspDirective)source;
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  AspDirective lClone = new AspDirective();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	#region ElementType
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.AspDirective;
	  }
	}
	#endregion   
  }
}
