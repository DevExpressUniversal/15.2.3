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
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class TokenBlock
	{
		int _Type;
		SourceRange _Range;
		public TokenBlock(SourceRange range, int type)
		{
			_Range = range;
			_Type = type;
		}
		public SourceRange Range
		{
			get
			{
				return _Range;
			}
		}
		public int Type
		{
			get
			{
				return _Type;
			}			
		}
		public bool IsUnknown
		{
			get
			{
				return _Type == -1;
			}
		}
	}
	public class ScannerExtension
	{
		Queue _TokenBlocks;
		public ScannerExtension()
		{
			_TokenBlocks = new Queue();
		}
		Token GetTokenFromBlock(TokenBlock block)
		{
			if (block == null)
				return null;
			return new Token(block.Range, block.Type, String.Empty);
		}
		public void EnqueueBlock(SourceRange range, int type)
		{
			EnqueueBlock(new TokenBlock(range, type));
		}
		public void EnqueueBlock(TokenBlock block)
		{
			_TokenBlocks.Enqueue(block);
		}
		public Token Scan(int startLine, int startOffset)
		{
			return Scan(new SourcePoint(startLine, startOffset));
		}
		public Token Scan(SourcePoint point)
		{
			if (_TokenBlocks.Count == 0)
				return null;
			TokenBlock block = _TokenBlocks.Peek() as TokenBlock;
			while (block != null && block.IsUnknown && point >= block.Range.End)
			{
				_TokenBlocks.Dequeue();
				block = _TokenBlocks.Peek() as TokenBlock;
			}
			if (block.IsUnknown)
				return null;
			if (point < block.Range.Start)
				return null;
			_TokenBlocks.Dequeue();
			return GetTokenFromBlock(block);
		}
		public TokenCollection GetTailTokens()
		{
			TokenCollection result = new TokenCollection();
			while (_TokenBlocks.Count > 0)
			{
				TokenBlock block = (TokenBlock)_TokenBlocks.Dequeue();
				if (block.IsUnknown)
					continue;
				Token token = GetTokenFromBlock(block);
				result.Add(token);
			}
			return result;
		}
	}
}
