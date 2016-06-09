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
using System.Collections.Generic;
using System;
using System.Collections;
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  class CSharpFormattingTable : FormattingTable
  {
	static FormattingTable _Instance;
	protected override void FillFormattingTokenTypeCorrespondanceTable()
	{
	  AddToOnlyGetValue(FormattingTokenType.WhiteSpace, " ");
	  AddToOnlyGetValue(FormattingTokenType.NewLine, "\n");
	  AddToOnlyGetValue(FormattingTokenType.Void, "void");
	  AddToken(FormattingTokenType.None, string.Empty);
	  AddToken(FormattingTokenType.Constructor, "contructor");
	  AddToken(FormattingTokenType.Field, "field");
	  AddToken(FormattingTokenType.Method, "method");
	  AddToken(FormattingTokenType.ReturnValue, "returnvalue");
	  AddToken(FormattingTokenType.Parameter, "parameter");
	  AddToken(FormattingTokenType.Param, "param");
	  AddToken(FormattingTokenType.Assembly, "assembly");
	  AddToken(FormattingTokenType.Abstract, "abstract");
	  AddToken(FormattingTokenType.As, "as");
	  AddToken(FormattingTokenType.Async, "async");
	  AddToken(FormattingTokenType.Await, "await");
	  AddToken(FormattingTokenType.Base, "base");
	  AddToken(FormattingTokenType.Break, "break");
	  AddToken(FormattingTokenType.Case, "case");
	  AddToken(FormattingTokenType.Catch, "catch");
	  AddToken(FormattingTokenType.Checked, "checked");
	  AddToken(FormattingTokenType.Class, "class");
	  AddToken(FormattingTokenType.Const, "const");
	  AddToken(FormattingTokenType.Continue, "continue");
	  AddToken(FormattingTokenType.Decimal, "decimal");
	  AddToken(FormattingTokenType.Default, "default");
	  AddToken(FormattingTokenType.Delegate, "delegate");
	  AddToken(FormattingTokenType.Do, "do");
	  AddToken(FormattingTokenType.Double, "double");
	  AddToken(FormattingTokenType.Else, "else");
	  AddToken(FormattingTokenType.Enum, "enum");
	  AddToken(FormattingTokenType.Event, "event");
	  AddToken(FormattingTokenType.Explicit, "explicit");
	  AddToken(FormattingTokenType.Extern, "extern");
	  AddToken(FormattingTokenType.False, "false");
	  AddToken(FormattingTokenType.Finally, "finally");
	  AddToken(FormattingTokenType.Fixed, "fixed");
	  AddToken(FormattingTokenType.For, "for");
	  AddToken(FormattingTokenType.Foreach, "foreach");
	  AddToken(FormattingTokenType.GoTo, "goto");
	  AddToken(FormattingTokenType.If, "if");
	  AddToken(FormattingTokenType.Implicit, "implicit");
	  AddToken(FormattingTokenType.In, "in");
	  AddToken(FormattingTokenType.Interface, "interface");
	  AddToken(FormattingTokenType.Internal, "internal");
	  AddToken(FormattingTokenType.Is, "is");
	  AddToken(FormattingTokenType.Lock, "lock");
	  AddToken(FormattingTokenType.Namespace, "namespace");
	  AddToken(FormattingTokenType.New, "new");
	  AddToken(FormattingTokenType.Null, "null");
	  AddToken(FormattingTokenType.Operator, "operator");
	  AddToken(FormattingTokenType.Out, "out");
	  AddToken(FormattingTokenType.Override, "override");
	  AddToken(FormattingTokenType.Params, "params");
	  AddToken(FormattingTokenType.Private, "private");
	  AddToken(FormattingTokenType.Protected, "protected");
	  AddToken(FormattingTokenType.Public, "public");
	  AddToken(FormattingTokenType.ReadOnly, "readonly");
	  AddToken(FormattingTokenType.Ref, "ref");
	  AddToken(FormattingTokenType.Return, "return");
	  AddToken(FormattingTokenType.Sealed, "sealed");
	  AddToken(FormattingTokenType.Sizeof, "sizeof");
	  AddToken(FormattingTokenType.Stackalloc, "stackalloc");
	  AddToken(FormattingTokenType.Static, "static");
	  AddToken(FormattingTokenType.Struct, "struct");
	  AddToken(FormattingTokenType.Switch, "switch");
	  AddToken(FormattingTokenType.This, "this");
	  AddToken(FormattingTokenType.Throw, "throw");
	  AddToken(FormattingTokenType.True, "true");
	  AddToken(FormattingTokenType.Try, "try");
	  AddToken(FormattingTokenType.TypeOf, "typeof");
	  AddToken(FormattingTokenType.Unchecked, "unchecked");
	  AddToken(FormattingTokenType.Unsafe, "unsafe");
	  AddToken(FormattingTokenType.Ushort, "ushort");
	  AddToken(FormattingTokenType.Using, "using");
	  AddToken(FormattingTokenType.Virtual, "virtual");
	  AddToken(FormattingTokenType.Volatile, "volatile");
	  AddToken(FormattingTokenType.While, "while");
	  AddToken(FormattingTokenType.Ampersand, "&");
	  AddToken(FormattingTokenType.AmpersandEqual, "&=");
	  AddToken(FormattingTokenType.Equal, "=");
	  AddToken(FormattingTokenType.AtColon, "@:");
	  AddToken(FormattingTokenType.At, "@");
	  AddToken(FormattingTokenType.Colon, ":");
	  AddToken(FormattingTokenType.Comma, ",");
	  AddToken(FormattingTokenType.MinusMinus, "--");
	  AddToken(FormattingTokenType.SlashEqual, "/=");
	  AddToken(FormattingTokenType.Dot, ".");
	  AddToken(FormattingTokenType.ColonColon, "::");
	  AddToken(FormattingTokenType.EqualEqual, "==");
	  AddToken(FormattingTokenType.EqualEqualEqual, "===");
	  AddToken(FormattingTokenType.GreaterThen, ">");
	  AddToken(FormattingTokenType.GreaterThenEqual, ">=");
	  AddToken(FormattingTokenType.PlusPlus, "++");
	  AddToken(FormattingTokenType.CurlyBraceOpen, "{");
	  AddToken(FormattingTokenType.BracketOpen, "[");
	  AddToken(FormattingTokenType.ParenOpen, "(");
	  AddToken(FormattingTokenType.LessThanLessThanEqual, "<<=");
	  AddToken(FormattingTokenType.LessThan, "<");
	  AddToken(FormattingTokenType.LessThanLessThan, "<<");
	  AddToken(FormattingTokenType.Minus, "-");
	  AddToken(FormattingTokenType.MinusEqual, "-=");
	  AddToken(FormattingTokenType.PercentEqual, "%=");
	  AddToken(FormattingTokenType.ExclamationEqual, "!=");
	  AddToken(FormattingTokenType.ExclamationEqualEqual, "!==");
	  AddToken(FormattingTokenType.Exclamation, "!");
	  AddToken(FormattingTokenType.VerticalBarEqual, "|=");
	  AddToken(FormattingTokenType.Plus, "+");
	  AddToken(FormattingTokenType.PlusEqual, "+=");
	  AddToken(FormattingTokenType.Question, "?");
	  AddToken(FormattingTokenType.CurlyBraceClose, "}");
	  AddToken(FormattingTokenType.BracketClose, "]");
	  AddToken(FormattingTokenType.ParenClose, ")");
	  AddToken(FormattingTokenType.Semicolon, ";");
	  AddToken(FormattingTokenType.Tilde, "~");
	  AddToken(FormattingTokenType.Asterisk, "*");
	  AddToken(FormattingTokenType.AsteriskEqual, "*=");
	  AddToken(FormattingTokenType.CaretEqual, "^=");
	  AddToken(FormattingTokenType.MinusGreaterThenAsterisk, "->*");
	  AddToken(FormattingTokenType.Partial, "partial");
	  AddToken(FormattingTokenType.Yield, "yield");
	  AddToken(FormattingTokenType.QuestionQuestion, "??");
	  AddToken(FormattingTokenType.VerticalBarVerticalBar, "||");
	  AddToken(FormattingTokenType.AmpersandAmpersand, "&&");
	  AddToken(FormattingTokenType.VerticalBar, "|");
	  AddToken(FormattingTokenType.Caret, "^");
	  AddToken(FormattingTokenType.LessThanEqual, "<=");
	  AddToken(FormattingTokenType.Slash, "/");
	  AddToken(FormattingTokenType.Percent, "%");
	  AddToken(FormattingTokenType.MinusGreaterThen, "->");
	  AddToken(FormattingTokenType.EqualGreaterThen, "=>");
	  AddToken(FormattingTokenType.Overloads, "overloads");
	  AddToken(FormattingTokenType.By, "by");
	  AddToken(FormattingTokenType.From, "from");
	  AddToken(FormattingTokenType.Order, "order");
	  AddToken(FormattingTokenType.Select, "select");
	  AddToken(FormattingTokenType.On, "on");
	  AddToken(FormattingTokenType.Where, "where");
	  AddToken(FormattingTokenType.Join, "join");
	  AddToken(FormattingTokenType.Into, "into");
	  AddToken(FormattingTokenType.Group, "group");
	  AddToken(FormattingTokenType.Skip, "skip");
	  AddToken(FormattingTokenType.Take, "take");
	  AddToken(FormattingTokenType.Ascending, "ascending");
	  AddToken(FormattingTokenType.Descending, "descending");
	  AddToken(FormattingTokenType.Aggregate, "aggregate");
	  AddToken(FormattingTokenType.Nothing, "Nothing");
	  AddToken(FormattingTokenType.AddHandler, "AddHandler");
	  AddToken(FormattingTokenType.Add, "add");
	  AddToken(FormattingTokenType.Remove, "remove");
	  AddToken(FormattingTokenType.Alias, "alias");
	  AddToken(FormattingTokenType.Call, "Call");
	  AddToken(FormattingTokenType.BackSlash, @"\");
	  AddToken(FormattingTokenType.LessThanGreaterThan, "<>");
	  AddToken(FormattingTokenType.Date, "Date");
	  AddToken(FormattingTokenType.Declare, "Declare");
	  AddToken(FormattingTokenType.Dim, "Dim");
	  AddToken(FormattingTokenType.DirectCast, "DirectCast");
	  AddToken(FormattingTokenType.Each, "Each");
	  AddToken(FormattingTokenType.ElseIf, "ElseIf");
	  AddToken(FormattingTokenType.End, "End");
	  AddToken(FormattingTokenType.EndIf, "EndIf");
	  AddToken(FormattingTokenType.Erase, "Erase");
	  AddToken(FormattingTokenType.Error, "Error");
	  AddToken(FormattingTokenType.Exit, "Exit");
	  AddToken(FormattingTokenType.Friend, "Friend");
	  AddToken(FormattingTokenType.Function, "function");
	  AddToken(FormattingTokenType.Get, "get");
	  AddToken(FormattingTokenType.GetType, "GetType");
	  AddToken(FormattingTokenType.GetXmlNamespace, "GetXmlNamespace");
	  AddToken(FormattingTokenType.Global, "global");
	  AddToken(FormattingTokenType.GoSub, "GoSub");
	  AddToken(FormattingTokenType.Handles, "Handles");
	  AddToken(FormattingTokenType.Implements, "Implements");
	  AddToken(FormattingTokenType.Imports, "Imports");
	  AddToken(FormattingTokenType.Inherits, "Inherits");
	  AddToken(FormattingTokenType.Integer, "Integer");
	  AddToken(FormattingTokenType.IsNot, "IsNot");
	  AddToken(FormattingTokenType.Let, "let");
	  AddToken(FormattingTokenType.Lib, "Lib");
	  AddToken(FormattingTokenType.Like, "Like");
	  AddToken(FormattingTokenType.Loop, "Loop");
	  AddToken(FormattingTokenType.Me, "Me");
	  AddToken(FormattingTokenType.Mod, "Mod");
	  AddToken(FormattingTokenType.Module, "module");
	  AddToken(FormattingTokenType.MustInherit, "MustInherit");
	  AddToken(FormattingTokenType.MustOverride, "MustOverride");
	  AddToken(FormattingTokenType.MyBase, "MyBase");
	  AddToken(FormattingTokenType.MyClass, "MyClass");
	  AddToken(FormattingTokenType.Narrowing, "Narrowing");
	  AddToken(FormattingTokenType.Next, "Next");
	  AddToken(FormattingTokenType.Not, "Not");
	  AddToken(FormattingTokenType.NotInheritable, "NotInheritable");
	  AddToken(FormattingTokenType.NotOverridable, "NotOverridable");
	  AddToken(FormattingTokenType.Of, "Of");
	  AddToken(FormattingTokenType.Option, "Option");
	  AddToken(FormattingTokenType.Optional, "Optional");
	  AddToken(FormattingTokenType.Or, "Or");
	  AddToken(FormattingTokenType.OrElse, "OrElse");
	  AddToken(FormattingTokenType.Overridable, "Overridable");
	  AddToken(FormattingTokenType.Overrides, "Overrides");
	  AddToken(FormattingTokenType.Property, "property");
	  AddToken(FormattingTokenType.RaiseEvent, "RaiseEvent");
	  AddToken(FormattingTokenType.ReDim, "ReDim");
	  AddToken(FormattingTokenType.REM, "REM");
	  AddToken(FormattingTokenType.RemoveHandler, "RemoveHandler");
	  AddToken(FormattingTokenType.Resume, "Resume");
	  AddToken(FormattingTokenType.Set, "set");
	  AddToken(FormattingTokenType.Shadows, "Shadows");
	  AddToken(FormattingTokenType.Shared, "Shared");
	  AddToken(FormattingTokenType.Single, "Single");
	  AddToken(FormattingTokenType.Step, "Step");
	  AddToken(FormattingTokenType.Stop, "Stop");
	  AddToken(FormattingTokenType.String, "String");
	  AddToken(FormattingTokenType.Structure, "Structure");
	  AddToken(FormattingTokenType.Sub, "Sub");
	  AddToken(FormattingTokenType.SyncLock, "SyncLock");
	  AddToken(FormattingTokenType.Then, "Then");
	  AddToken(FormattingTokenType.To, "To");
	  AddToken(FormattingTokenType.TryCast, "TryCast");
	  AddToken(FormattingTokenType.UInteger, "UInteger");
	  AddToken(FormattingTokenType.Variant, "Variant");
	  AddToken(FormattingTokenType.Wend, "Wend");
	  AddToken(FormattingTokenType.When, "When");
	  AddToken(FormattingTokenType.Widening, "Widening");
	  AddToken(FormattingTokenType.With, "with");
	  AddToken(FormattingTokenType.WithEvents, "WithEvents");
	  AddToken(FormattingTokenType.WriteOnly, "WriteOnly");
	  AddToken(FormattingTokenType.Xor, "Xor");
	  AddToken(FormattingTokenType.SByteToken, "SByteToken");
	  AddToken(FormattingTokenType.ULongToken, "ULongToken");
	  AddToken(FormattingTokenType.UShortToken, "UShortToken");
	  AddToken(FormattingTokenType.Var, "var");
	  AddToken(FormattingTokenType.Equals, "equals");
	  AddToken(FormattingTokenType.OrderBy, "orderby");
	  AddToken(FormattingTokenType.IfDirective, "#if");
	  AddToken(FormattingTokenType.ElifDirective, "#elif");
	  AddToken(FormattingTokenType.ElseDirective, "#else");
	  AddToken(FormattingTokenType.EndIfDirective, "#endif");
	  AddToken(FormattingTokenType.DefineDirective, "#define");
	  AddToken(FormattingTokenType.UndefDirective, "#undef");
	  AddToken(FormattingTokenType.LineDirective, "#line");
	  AddToken(FormattingTokenType.ErrorDirective, "#error");
	  AddToken(FormattingTokenType.WarningDirective, "#warning");
	  AddToken(FormattingTokenType.RegionDirective, "#region");
	  AddToken(FormattingTokenType.EndRegionDirective, "#endregion");
	  AddToken(FormattingTokenType.PragmaDirective, "#pragma");
	  AddToken(FormattingTokenType.GreatThanGreatThanEqual, ">>=");
	  AddToken(FormattingTokenType.GreatThanGreatThanGreatThanEqual, ">>>=");
	  AddToken(FormattingTokenType.GreatThanGreatThanGreatThan, ">>>");
	  AddToken(FormattingTokenType.GreatThanGreatThan, ">>");
	  AddToken(FormattingTokenType.BackSlashEqual, "\\=");
	  AddToken(FormattingTokenType.InstanceOf, "instanceof");
	  AddToken(FormattingTokenType.Delete, "delete");
	}
	public static FormattingTable Instance
	{
	  get
	  {
		if (_Instance == null)
		  _Instance = new CSharpFormattingTable();
		return _Instance;
	  }
	}
  }
}
