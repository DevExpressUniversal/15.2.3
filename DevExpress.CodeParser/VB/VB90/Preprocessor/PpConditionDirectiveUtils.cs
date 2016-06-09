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

using System.Collections;
#if SL
using DevExpress.Utils;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB.Preprocessor
#else
namespace DevExpress.CodeParser.VB.Preprocessor
#endif
{
	public partial class VBPreprocessor
	{
		bool IsEOF()
		{
			return  _Scanner.IsEof();
		}
		void IfWaitEndIf()
		{
			ResetNextChCounter();
			int count = 0;
			while (true)
			{
				if (IsEOF())
					return;
				if (IsNextChar('#'))
				{
					tToken = la;
					la = _Scanner.Scan();
					if (la.Type == Tokens.EOF)
						return;
					if (la.Type == Tokens.IfDirective)
						count++;
					if (la.Type == Tokens.ElseDirective || la.Type == Tokens.ElseIfDirective)
					{
						if (count == 0)
							return;
					}
					if (la.Type == Tokens.EndifDirective)
					{
						if (count == 0)
							return;
						count--;
					}
				}
				SkipToEOL();
			}
		}
		protected void SkipToEolIfNeeded()
		{
			if (la.Type == Tokens.LineTerminator || la.Type == Tokens.EOF)
				return;
			SkipToEOL();
		}
		protected void SkipToEOL()
		{
			_Scanner.SkipToEOL();
		}
		void ResetNextChCounter()
		{
			_Scanner.ResetNextChCounter();
		}
		bool IsNextChar(char ch)
		{
			return _Scanner.IsNextChar(ch);
		}
		bool GetBool(object expValue)
		{
			if (expValue == null)
				return false;
			PpResult ppResult = Evaluator.GetResult(expValue);
			return GetBool(ppResult);
		}
		bool GetBool(PpResult result)
		{
			if (result == PpResult.True)
				return true;
			return false;
		}
		protected void ProcessIFDirectiveCondition(bool condition)
		{
			IfConditions.Push(condition);
			if (!condition)
			{
				IfWaitEndIf();
			}
		}
		protected void ProcessDirectiveCondition(bool condition)
		{
			if (ConditionWasTrue)
				IfWaitEndIf();
			else
				ProcessDirectiveConditionCore(condition);
		}
		void ProcessDirectiveConditionCore(object expVal)
		{
			bool condition = GetBool(expVal);
			if (condition)
			{
				if (IfConditions.Count > 0)
					IfConditions.Pop();
				IfConditions.Push(true);
				return;
			}
			IfWaitEndIf();
		}
		public bool ConditionWasTrue
		{
			get
			{
				if (_IfConditions.Count > 0)
					return (bool)_IfConditions.Peek();
				else
					return false;
			}
		}
		protected void ProcessEndIf()
		{
			if (IfConditions.Count <= 0)
				return;
			IfConditions.Pop();
		}
		protected Stack IfConditions
		{
			get
			{
				return _IfConditions;
			}
		}
		public void CleanUp()
		{
			_Defines.Clear();
			_IfConditions.Clear();
			_SourceFile = null;
		  _Scanner = null;
		  _VbParser = null;
	  tToken = null;
		  la = null;
		  errors = null;
		  set = null;
		}
	}
}
