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
  public class Resume : FlowBreak
  {
	string _Label;
	bool _HasNextClause;
	#region Resume
	public Resume()
	{
	}
	#endregion
	#region CloneDataFrom
	protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
	{
	  if (source == null)
		return;
	  base.CloneDataFrom(source, options);
	  if (!(source is Resume))
		return;
	  Resume lSource = (Resume)source;
	  _Label = lSource._Label;
	  _HasNextClause = lSource._HasNextClause;
	}
	#endregion
	#region GetImageIndex
	public override int GetImageIndex()
	{
	  return ImageIndex.Throw;
	}
	#endregion
	#region ToString
	public override string ToString()
	{
	  string lResult = "Resume";
	  if (_HasNextClause)
		lResult += " Next";
	  else
		lResult += " " + _Label;
	  return lResult;
	}
	#endregion
	#region Clone
	public override BaseElement Clone(ElementCloneOptions options)
	{
	  Resume lClone = new Resume();
	  lClone.CloneDataFrom(this, options);
	  return lClone;
	}
	#endregion
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.Resume;
	  }
	}
	#region Label
	public string Label
	{
	  get
	  {
		return _Label;
	  }
	  set
	  {
		_Label = value;
	  }
	}
	#endregion
	#region HasNextClause
	public bool HasNextClause
	{
	  get
	  {
		return _HasNextClause;
	  }
	  set
	  {
		_HasNextClause = value;
	  }
	}
	#endregion
  }
}
