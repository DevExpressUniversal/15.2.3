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
	public class PostponedParsingData
	{
		CommentCollection _Comments;
		ISourceReader _UnparsedCode;
		RegionDirective _RegionContext;
		bool _IsParsing;
		int _LineOffset;
		int _ColumnOffset;
		ParserBase _Parser;
		public PostponedParsingData()
		{
		}
		public void CleanUp()
		{
			if (_Comments != null)
			{
				_Comments.Clear();
				_Comments = null;
			}
			_UnparsedCode = null;
			_RegionContext = null;
			_Parser = null;
		}
		public bool IsParsing
		{
			get
			{
				return _IsParsing;
			}
			set
			{
				_IsParsing = value;
			}
		}
		public CommentCollection Comments
		{
			get
			{
				return _Comments;
			}
			set
			{
				_Comments = value;
			}
		}
		public ISourceReader UnparsedCode
		{
			get
			{
				return _UnparsedCode;
			}
			set
			{
				_UnparsedCode = value;
			}
		}
		public RegionDirective RegionContext
		{
			get
			{
				return _RegionContext;
			}
			set
			{
				_RegionContext = value;
			}
		}
		public bool HasUnparsedCode
		{
			get
			{
				return _UnparsedCode != null;
			}
		}
		public bool HasComments
		{
			get
			{
				return _Comments != null && _Comments.Count > 0;
			}
		}
		public int LineOffset
		{
			get 
			{
				return _LineOffset;
			}
			set 
			{
				_LineOffset = value;
			}
		}
		public int ColumnOffset
		{
			get 
			{
				return _ColumnOffset;
			}
			set 
			{
				_ColumnOffset = value;
			}
		}
		public ParserBase Parser
		{
			get
			{
				return _Parser;
			}
			set
			{
				_Parser = value;
			}
		}
	}
}
