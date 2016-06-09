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
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.CSharp
#else
namespace DevExpress.CodeParser.CSharp
#endif
{
	public sealed class CSharp10KeyWords
	{
		private static StringCollection _KeyWords = new StringCollection();
		#region CSharp10KeyWords
		private CSharp10KeyWords(){}
		#endregion	
		#region CSharp10KeyWords
		static CSharp10KeyWords()
		{
			lock (typeof(CSharp10KeyWords))
			{
				_KeyWords.Add("get");
				_KeyWords.Add("set");
				_KeyWords.Add("this");
				_KeyWords.Add("base");
				_KeyWords.Add("as");
				_KeyWords.Add("is");
				_KeyWords.Add("new");
				_KeyWords.Add("sizeof");
				_KeyWords.Add("typeof");
				_KeyWords.Add("true");
				_KeyWords.Add("false");
				_KeyWords.Add("stackalloc");
				_KeyWords.Add("else");
				_KeyWords.Add("if");
				_KeyWords.Add("switch");
				_KeyWords.Add("case");
				_KeyWords.Add("do");
				_KeyWords.Add("for");
				_KeyWords.Add("foreach");
				_KeyWords.Add("in");
				_KeyWords.Add("while");
				_KeyWords.Add("break");
				_KeyWords.Add("continue");
				_KeyWords.Add("default");
				_KeyWords.Add("goto");
				_KeyWords.Add("return");
				_KeyWords.Add("try");
				_KeyWords.Add("throw");
				_KeyWords.Add("catch");
				_KeyWords.Add("finally");
				_KeyWords.Add("checked");
				_KeyWords.Add("unchecked");
				_KeyWords.Add("fixed");
				_KeyWords.Add("unsafe");
				_KeyWords.Add("bool");
				_KeyWords.Add("byte");
				_KeyWords.Add("char");
				_KeyWords.Add("decimal");
				_KeyWords.Add("double");
				_KeyWords.Add("enum");
				_KeyWords.Add("float");
				_KeyWords.Add("int");
				_KeyWords.Add("long");
				_KeyWords.Add("sbyte");
				_KeyWords.Add("short");
				_KeyWords.Add("struct");
				_KeyWords.Add("uint");
				_KeyWords.Add("ushort");
				_KeyWords.Add("ulong");
				_KeyWords.Add("class");
				_KeyWords.Add("interface");
				_KeyWords.Add("delegate");
				_KeyWords.Add("object");
				_KeyWords.Add("string");
				_KeyWords.Add("void");
				_KeyWords.Add("explicit");
				_KeyWords.Add("implicit");
				_KeyWords.Add("operator");
				_KeyWords.Add("params");
				_KeyWords.Add("ref");
				_KeyWords.Add("out");
				_KeyWords.Add("abstract");
				_KeyWords.Add("const");
				_KeyWords.Add("event");
				_KeyWords.Add("extern");
				_KeyWords.Add("override");
				_KeyWords.Add("readonly");
				_KeyWords.Add("sealed");
				_KeyWords.Add("static");
				_KeyWords.Add("virtual");
				_KeyWords.Add("public");
				_KeyWords.Add("protected");
				_KeyWords.Add("private");
				_KeyWords.Add("internal");
				_KeyWords.Add("namespace");
				_KeyWords.Add("using");
				_KeyWords.Add("lock");
				_KeyWords.Add("null");
				_KeyWords.Add("add");
				_KeyWords.Add("remove");
				_KeyWords.Add("volatile");
			}
		}
		#endregion
		#region KeyWords
		public static StringCollection Collection
		{
			get
			{
				return _KeyWords;
			}
		}
		#endregion
		#region Contains
		public static bool Contains(string word)
		{
			return _KeyWords.Contains(word);
		}
		#endregion
	}
}
