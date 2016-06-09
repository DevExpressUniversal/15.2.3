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
using System.Collections;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class ElementsGenerationRules
  {
	bool _WrapFirst;
	bool _WrapParams;
	bool _AlignIfWrap;
	bool _Indenting = true;
	string _StringDelimiter = ","; 
	FormattingTokenType _Delimiter = FormattingTokenType.Comma; 
	#region WrapFirst
	public bool WrapFirst
	{
	  get { return _WrapFirst; }
	  set { _WrapFirst = value; }
	}
	#endregion
	#region WrapParams
	public bool WrapParams
	{
	  get { return _WrapParams; }
	  set { _WrapParams = value; }
	}
	#endregion
	#region AlignIfWrap
	public bool AlignIfWrap
	{
	  get { return _AlignIfWrap; }
	  set { _AlignIfWrap = value; }
	}
	#endregion
	#region Indenting
	public bool Indenting
	{
	  get { return _Indenting; }
	  set { _Indenting = value; }
	}
	#endregion
	#region Delimiter
	public FormattingTokenType Delimiter
	{
	  get { return _Delimiter; }
	  set { _Delimiter = value; }
	}
	public string StringDelimiter
	{
	  get
	  {
		return _StringDelimiter;
	  }
	  set
	  {
	  	_StringDelimiter = value;
	  }
	}
	public bool HasStringDelimiter
	{
	  get
	  {
		return !string.IsNullOrEmpty(_StringDelimiter);
	  }
	}
	#endregion
  }
}
