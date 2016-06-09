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
using System.Collections.Specialized;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public class ErrorList: List<Error>
  {
  }
  public struct Error
  {
	int _ErrorNum;
	int _Col;
	int _Line;
	public int ErrorNum { get { return _ErrorNum; } }
	public int Col { get { return _Col; } }
	public int Line { get { return _Line; } }
	public Error(int line, int col, int error)
	{
	  _ErrorNum = error;
	  _Col = col;
	  _Line = line;
	}
  }
	public abstract class ParserErrorsBase
	{
		const string errMsgFormat = "-- line {0} col {1}: {2}\r\n";
		StringCollection errors;
	ErrorList _Errors;
	public ErrorList Errors
	{
	  get 
	  {
		if(_Errors == null)
		  _Errors = new ErrorList();
		return _Errors;
	  }
	}
		public ParserErrorsBase()
		{
			errors = new StringCollection();
		}
		protected abstract string GetSyntaxErrorText(int n);
		protected void LogError(string format, params object[] args)
		{
			errors.Add(String.Format(format, args));
		}
		public void SynErr(int line, int col, int n)
		{
	  string s = GetSyntaxErrorText(n);
			LogError(errMsgFormat, line, col, s);
	  Errors.Add(new Error(line, col, n));
		}
		public void SemErr(int line, int col, int n)
		{
			string s = String.Format("error {0}", n);
			LogError(errMsgFormat, line, col, s);
	  Errors.Add(new Error(line, col, n));
		}
		public void Error(int line, int col, string s)
		{
			LogError(errMsgFormat, line, col, s);
	  Errors.Add(new Error(line, col, -1));
		}
		public void Exception(string s)
		{
			LogError(errMsgFormat, 0, 0, s);
		}
		public void Clear()
		{
			if (errors != null)
				errors.Clear();
	  _Errors = null;
		}
		public int Count
		{
			get
			{
				return errors.Count;
			}
		}
	public ErrorList GetClonedErrorList()
	{
	  ErrorList clone = new ErrorList();
	  foreach(Error err in Errors)
		clone.Add(err);
	  return clone;
	}
  }
}
