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
using System.Collections.Generic;
using System.Text;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public abstract class FullDirective
  {
	public enum FullDirectiveType
	{
	  FullRegion,
	  FullIf
	}
	List<FullDirective> _NestedFulls;
	FullDirective _ParentFull;
	SourceRange _Range;
	public FullDirective()
	{
	  _NestedFulls = new List<FullDirective>();
	}
	public FullDirective GoToNextFull()
	{
	  FullDirective old = this;
	  bool fromParent = false;
	  for (; ; )
	  {
		if (!fromParent && old.NestedFulls.Count > 0)
		  return old.NestedFulls[0];
		FullDirective parent = old.ParentFull;
		if (parent == null)
		  return null;
		int index = parent.NestedFulls.IndexOf(old);
		if (index == -1)
		  return null;
		if (index < parent.NestedFulls.Count - 1)
		  return parent.NestedFulls[++index];
		old = parent;
		fromParent = true;
	  }
	}
	public abstract List<PreprocessorDirective> SimpleDirectives { get; }
	public abstract PreprocessorDirective FirstSimple { get; }
	public abstract PreprocessorDirective LastSimple { get; }
	public List<FullDirective> NestedFulls
	{
	  get { return _NestedFulls; }
	}
	public FullDirective ParentFull
	{
	  get { return _ParentFull; }
	  set { _ParentFull = value; }
	}
	public SourceRange Range
	{
	  get { return _Range; }
	  set { _Range = value; }
	}
	public int StartLine
	{
	  get { return _Range.Start.Line; }
	}
	public int EndLine
	{
	  get { return _Range.End.Line; }
	}
	public abstract FullDirectiveType FullType { get; }
  }
}
