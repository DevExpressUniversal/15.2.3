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
  public class LineDirective : CompilerDirective
  {
	int _LineNumber;
	string _FileName;
	bool _Hidden;
	bool _Default;
	public LineDirective()
	{
	}
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  LineDirective lineDirective = source as LineDirective;
	  if (lineDirective == null)
		return;
	  _LineNumber = lineDirective._LineNumber;
	  _FileName = lineDirective._FileName;
	  _Hidden = lineDirective._Hidden;
	  _Default = lineDirective._Default;
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  LineDirective clone = new LineDirective();
	  clone.CloneDataFrom(this, options);
	  return clone;
	}
	#endregion
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.LineDirective;
	  }
	}
	#region LineNumber
	public int LineNumber
	{
	  get
	  {
		return _LineNumber;
	  }
	  set
	  {
		_LineNumber = value;
	  }
	}
	#endregion
	#region FileName
	public string FileName
	{
	  get
	  {
		return _FileName;
	  }
	  set
	  {
		_FileName = value;
	  }
	}
	#endregion
	#region Hidden
	public bool Hidden
	{
	  get
	  {
		return _Hidden;
	  }
	  set
	  {
		_Hidden = value;
	  }
	}
	#endregion
	#region Default
	public bool Default
	{
	  get
	  {
		return _Default;
	  }
	  set
	  {
		_Default = value;
	  }
	}
	#endregion
  }
}
