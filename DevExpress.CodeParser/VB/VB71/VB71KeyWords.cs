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
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser.VB
#else
namespace DevExpress.CodeParser.VB
#endif
{
	public sealed class VB71KeyWords
	{
		private static StringCollection _KeyWords = null;
		private VB71KeyWords(){}
		static void CreateKeywords()
		{
			lock (typeof(VB71KeyWords))
			{
				_KeyWords = new StringCollection();
				_KeyWords.Add("addhandler");														
				_KeyWords.Add("addressof");															
				_KeyWords.Add("alias");																	
				_KeyWords.Add("and");																		
				_KeyWords.Add("andalso");																
				_KeyWords.Add("ansi");																	
				_KeyWords.Add("as");																		
				_KeyWords.Add("assembly");															
				_KeyWords.Add("auto");																	
				_KeyWords.Add("boolean");																
				_KeyWords.Add("byref");																	
				_KeyWords.Add("byte");																	
				_KeyWords.Add("byval");																	
				_KeyWords.Add("call");																	
				_KeyWords.Add("case");																	
				_KeyWords.Add("catch");																	
				_KeyWords.Add("cbool");																	
				_KeyWords.Add("cbyte");																	
				_KeyWords.Add("cchar");																	
				_KeyWords.Add("cdate");																	
				_KeyWords.Add("cdbl");																	
				_KeyWords.Add("cdec");																	
				_KeyWords.Add("char");																	
				_KeyWords.Add("cint");																	
				_KeyWords.Add("class");																	
				_KeyWords.Add("clng");																	
				_KeyWords.Add("cobj");																	
				_KeyWords.Add("const");																	
				_KeyWords.Add("cshort");																
				_KeyWords.Add("csng");																	
				_KeyWords.Add("cstr");																	
				_KeyWords.Add("ctype");																	
				_KeyWords.Add("date");																	
				_KeyWords.Add("decimal");																
				_KeyWords.Add("declare");																
				_KeyWords.Add("default");																
				_KeyWords.Add("delegate");															
				_KeyWords.Add("dim");																		
				_KeyWords.Add("directcast");														
				_KeyWords.Add("do");																		
				_KeyWords.Add("double");																
				_KeyWords.Add("each");																	
				_KeyWords.Add("else");																	
				_KeyWords.Add("elseif");																
				_KeyWords.Add("end");																		
				_KeyWords.Add("endif");																	
				_KeyWords.Add("enum");																	
				_KeyWords.Add("erase");																	
				_KeyWords.Add("error");																	
				_KeyWords.Add("event");																	
				_KeyWords.Add("exit");																	
				_KeyWords.Add("finally");																
				_KeyWords.Add("for");																		
				_KeyWords.Add("friend");																
				_KeyWords.Add("function");															
				_KeyWords.Add("get");																		
				_KeyWords.Add("gettype");																
				_KeyWords.Add("gosub");																	
				_KeyWords.Add("goto");																	
				_KeyWords.Add("handles");																
				_KeyWords.Add("if");																		
				_KeyWords.Add("implements");														
				_KeyWords.Add("imports");																
				_KeyWords.Add("in");																		
				_KeyWords.Add("inherits");															
				_KeyWords.Add("Integer");																
				_KeyWords.Add("interface");															
				_KeyWords.Add("is");																		
				_KeyWords.Add("let");																		
				_KeyWords.Add("lib");																		
				_KeyWords.Add("like");																	
				_KeyWords.Add("long");																	
				_KeyWords.Add("loop");																	
				_KeyWords.Add("Me");																		
				_KeyWords.Add("mod");																		
				_KeyWords.Add("module");																
				_KeyWords.Add("mustinherit");														
				_KeyWords.Add("mustoverride");													
				_KeyWords.Add("MyBase");																
				_KeyWords.Add("MyClass");																
				_KeyWords.Add("namespace");															
				_KeyWords.Add("new");																		
				_KeyWords.Add("next");																	
				_KeyWords.Add("not");																		
				_KeyWords.Add("nothing");																
				_KeyWords.Add("notinheritable");												
				_KeyWords.Add("notoverridable");												
				_KeyWords.Add("object");																
				_KeyWords.Add("on");																		
				_KeyWords.Add("option");																
				_KeyWords.Add("optional");															
				_KeyWords.Add("or");																		
				_KeyWords.Add("orelse");																
				_KeyWords.Add("overloads");															
				_KeyWords.Add("overridable");														
				_KeyWords.Add("overrides");															
				_KeyWords.Add("paramarray");														
				_KeyWords.Add("preserve");															
				_KeyWords.Add("private");																
				_KeyWords.Add("property");															
				_KeyWords.Add("protected");															
				_KeyWords.Add("public");																
				_KeyWords.Add("raiseevent");														
				_KeyWords.Add("readonly");															
				_KeyWords.Add("redim");																	
				_KeyWords.Add("rem");																		
				_KeyWords.Add("removehandler");													
				_KeyWords.Add("resume");																
				_KeyWords.Add("return");																
				_KeyWords.Add("select");																
				_KeyWords.Add("set");																		
				_KeyWords.Add("shadows");																
				_KeyWords.Add("shared");																
				_KeyWords.Add("short");																	
				_KeyWords.Add("single");																
				_KeyWords.Add("static");																
				_KeyWords.Add("step");																	
				_KeyWords.Add("stop");																	
				_KeyWords.Add("string");																
				_KeyWords.Add("structure");															
				_KeyWords.Add("sub");																		
				_KeyWords.Add("synclock");															
				_KeyWords.Add("then");																	
				_KeyWords.Add("throw");																	
				_KeyWords.Add("to");																		
				_KeyWords.Add("try");																		
				_KeyWords.Add("typeof");																
				_KeyWords.Add("unicode");																
				_KeyWords.Add("until");																	
				_KeyWords.Add("variant");																
				_KeyWords.Add("wend");																	
				_KeyWords.Add("when");																	
				_KeyWords.Add("while");																	
				_KeyWords.Add("with");																	
				_KeyWords.Add("withevents");														
				_KeyWords.Add("writeonly");															
				_KeyWords.Add("xor");																		
				_KeyWords.Add("false");																	
				_KeyWords.Add("true");																	
				_KeyWords.Add("isnot");																	
			}																													
		}																														
		public static StringCollection Collection										
		{																														
			get																												
			{																													
				if (_KeyWords == null)																	
					CreateKeywords();																			
				return _KeyWords;																				
			}																													
		}
	}
}
