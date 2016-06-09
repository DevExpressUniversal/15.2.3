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

#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class SourceRangeWrapper
	{
		public SourceRange Range;
		public SourceRangeWrapper(SourceRange range)
		{
			Range = range;
		}
	}
	public class TextRangeWrapper 
	{
		public TextRange Range;
		public TextRangeWrapper(TextRange range) 
		{
			Range = range;
		}
		public static void Set(ref TextRangeWrapper _BlockStart, TextRange range) 
		{
			if(_BlockStart == null)
				_BlockStart = new TextRangeWrapper(range);
			else
				_BlockStart.Range = range;
		}
		public static TextRange Get(TextRangeWrapper _BlockStart) 
		{
			if(_BlockStart == null)
				return new TextRange();
			else
				return _BlockStart.Range;
		}
		public static implicit operator TextRangeWrapper(SourceRange range) 
		{
			return new TextRangeWrapper(range);
		}
		public static implicit operator TextRangeWrapper(TextRange range) 
		{
			return new TextRangeWrapper(range);
		}
		public static implicit operator SourceRange(TextRangeWrapper range) 
		{
			return range == null ? SourceRange.Empty : (SourceRange)range.Range;
		}
		public static implicit operator TextRange(TextRangeWrapper range) 
		{
			return range == null ? TextRange.Empty : range.Range;
		}
	}
}
