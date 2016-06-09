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
  public class FullIfDirective : FullDirective
  {
	IfDirective _IfDirective;
	List<ElifDirective> _ElifDirectives;
	ElseDirective _ElseDirective;
	EndIfDirective _EndifDirective;
	public FullIfDirective()
	  : base()
	{
	  _ElifDirectives = new List<ElifDirective>();
	}
	public override List<PreprocessorDirective> SimpleDirectives
	{
	  get
	  {
		List<PreprocessorDirective> lst = new List<PreprocessorDirective> { IfDirective };
		foreach (ElifDirective elifDirective in ElifDirectives)
		  lst.Add(elifDirective);
		if (ElseDirective != null)
		  lst.Add(ElseDirective);
		lst.Add(EndifDirective);
		return lst;
	  }
	}
	public override PreprocessorDirective FirstSimple
	{
	  get
	  {
		return IfDirective;
	  }
	}
	public override PreprocessorDirective LastSimple
	{
	  get
	  {
		return EndifDirective;
	  }
	}
	public IfDirective IfDirective
	{
	  get { return _IfDirective; }
	  set { _IfDirective = value; }
	}
	public List<ElifDirective> ElifDirectives
	{
	  get { return _ElifDirectives; }
	}
	public ElseDirective ElseDirective
	{
	  get { return _ElseDirective; }
	  set { _ElseDirective = value; }
	}
	public EndIfDirective EndifDirective
	{
	  get { return _EndifDirective; }
	  set { _EndifDirective = value; }
	}
	public PreprocessorDirective SimpleIsSatisfied
	{
	  get
	  {
		if(IfDirective != null && IfDirective.ExpressionValue)
		  return IfDirective;
		foreach(ElifDirective elif in ElifDirectives)
		  if(elif.ExpressionValue)
			return elif;
		if(ElseDirective != null)
		  return ElseDirective;
		return null;
	  }
	}
	public PreprocessorDirective SimpleAfterSimpleIsSatisfied
	{
	  get
	  {
		List<PreprocessorDirective> simples = SimpleDirectives;
		return simples[simples.IndexOf(SimpleIsSatisfied) + 1];
	  }
	}
	public override FullDirectiveType FullType
	{
	  get { return FullDirectiveType.FullIf; }
	}
  }
}
