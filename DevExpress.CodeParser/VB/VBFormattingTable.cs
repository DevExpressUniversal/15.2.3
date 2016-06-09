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
  class VBFormattingTable : FormattingTable
  {
	static FormattingTable _Instance;
	protected override void FillFormattingTokenTypeCorrespondanceTable()
	{
	  AddToOnlyGetValue(FormattingTokenType.WhiteSpace, " ");
	  AddToOnlyGetValue(FormattingTokenType.NewLine, "\n");
	  AddToOnlyGetValue(FormattingTokenType.WhitespaceUnderline, " _");
	  AddToOnlyGetValue(FormattingTokenType.Object, "Object");
	  AddToken(FormattingTokenType.None, string.Empty);
	  AddToken(FormattingTokenType.Constructor, "Contructor");
	  AddToken(FormattingTokenType.ColonEquals, ":=");
	  AddToken(FormattingTokenType.Field, "Field");
	  AddToken(FormattingTokenType.IIf, "IIf");
	  AddToken(FormattingTokenType.Strict, "Strict");
	  AddToken(FormattingTokenType.Compare, "Compare");
	  AddToken(FormattingTokenType.Binary, "Binary");
	  AddToken(FormattingTokenType.Text, "Text");
	  AddToken(FormattingTokenType.Infer, "Infer");
	  AddToken(FormattingTokenType.Method, "Method");
	  AddToken(FormattingTokenType.ReturnValue, "ReturnValue");
	  AddToken(FormattingTokenType.Parameter, "Parameter");
	  AddToken(FormattingTokenType.Assembly, "Assembly");
	  AddToken(FormattingTokenType.Abstract, "Abstract");
	  AddToken(FormattingTokenType.As, "As");
	  AddToken(FormattingTokenType.Until, "Until");
	  AddToken(FormattingTokenType.Base, "Base");
	  AddToken(FormattingTokenType.Break, "Break");
	  AddToken(FormattingTokenType.Case, "Case");
	  AddToken(FormattingTokenType.Catch, "Catch");
	  AddToken(FormattingTokenType.Checked, "Checked");
	  AddToken(FormattingTokenType.Class, "Class");
	  AddToken(FormattingTokenType.Const, "Const");
	  AddToken(FormattingTokenType.Continue, "Continue");
	  AddToken(FormattingTokenType.Decimal, "Decimal");
	  AddToken(FormattingTokenType.Default, "Default");
	  AddToken(FormattingTokenType.Delegate, "Delegate");
	  AddToken(FormattingTokenType.Do, "Do");
	  AddToken(FormattingTokenType.Double, "Double");
	  AddToken(FormattingTokenType.Else, "Else");
	  AddToken(FormattingTokenType.Await, "Await");
	  AddToken(FormattingTokenType.Async, "Async");
	  AddToken(FormattingTokenType.Enum, "Enum");
	  AddToken(FormattingTokenType.Event, "Event");
	  AddToken(FormattingTokenType.Explicit, "Explicit");
	  AddToken(FormattingTokenType.Extern, "Extern");
	  AddToken(FormattingTokenType.False, "False");
	  AddToken(FormattingTokenType.IsTrue, "IsTrue");
	  AddToken(FormattingTokenType.IsFalse, "IsFalse");
	  AddToken(FormattingTokenType.Finally, "Finally");
	  AddToken(FormattingTokenType.Fixed, "Fixed");
	  AddToken(FormattingTokenType.For, "For");
	  AddToken(FormattingTokenType.Foreach, "Foreach");
	  AddToken(FormattingTokenType.GoTo, "GoTo");
	  AddToken(FormattingTokenType.Finalize, "Finalize");
	  AddToken(FormattingTokenType.If, "If");
	  AddToken(FormattingTokenType.Implicit, "Implicit");
	  AddToken(FormattingTokenType.In, "In");
	  AddToken(FormattingTokenType.Interface, "Interface");
	  AddToken(FormattingTokenType.Internal, "Internal");
	  AddToken(FormattingTokenType.Is, "Is");
	  AddToken(FormattingTokenType.Lock, "Lock");
	  AddToken(FormattingTokenType.Namespace, "Namespace");
	  AddToken(FormattingTokenType.New, "New");
	  AddToken(FormattingTokenType.Null, "Null");
	  AddToken(FormattingTokenType.Operator, "Operator");
	  AddToken(FormattingTokenType.Out, "Out");
	  AddToken(FormattingTokenType.Override, "Override");
	  AddToken(FormattingTokenType.Params, "Params");
	  AddToken(FormattingTokenType.Private, "Private");
	  AddToken(FormattingTokenType.Protected, "Protected");
	  AddToken(FormattingTokenType.Public, "Public");
	  AddToken(FormattingTokenType.ReadOnly, "ReadOnly");
	  AddToken(FormattingTokenType.Distinct, "Distinct");
	  AddToken(FormattingTokenType.Ref, "Ref");
	  AddToken(FormattingTokenType.Return, "Return");
	  AddToken(FormattingTokenType.Sealed, "sealed");
	  AddToken(FormattingTokenType.Sizeof, "SizeOf");
	  AddToken(FormattingTokenType.Stackalloc, "Stackalloc");
	  AddToken(FormattingTokenType.Static, "Static");
	  AddToken(FormattingTokenType.DotDotDot, "...");
	  AddToken(FormattingTokenType.Struct, "Struct");
	  AddToken(FormattingTokenType.Switch, "Switch");
	  AddToken(FormattingTokenType.This, "This");
	  AddToken(FormattingTokenType.Throw, "Throw");
	  AddToken(FormattingTokenType.True, "True");
	  AddToken(FormattingTokenType.Try, "Try");
	  AddToken(FormattingTokenType.TypeOf, "TypeOf");
	  AddToken(FormattingTokenType.Unchecked, "unchecked");
	  AddToken(FormattingTokenType.Unsafe, "Unsafe");
	  AddToken(FormattingTokenType.Ushort, "Ushort");
	  AddToken(FormattingTokenType.Using, "Using");
	  AddToken(FormattingTokenType.Virtual, "Virtual");
	  AddToken(FormattingTokenType.Void, "Void");
	  AddToken(FormattingTokenType.Volatile, "Volatile");
	  AddToken(FormattingTokenType.While, "While");
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
	  AddToken(FormattingTokenType.Partial, "Partial");
	  AddToken(FormattingTokenType.Yield, "Yield");
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
	  AddToken(FormattingTokenType.Overloads, "Overloads");
	  AddToken(FormattingTokenType.By, "By");
	  AddToken(FormattingTokenType.From, "From");
	  AddToken(FormattingTokenType.Order, "Order");
	  AddToken(FormattingTokenType.Select, "Select");
	  AddToken(FormattingTokenType.On, "On");
	  AddToken(FormattingTokenType.Off, "Off");
	  AddToken(FormattingTokenType.Where, "Where");
	  AddToken(FormattingTokenType.Join, "Join");
	  AddToken(FormattingTokenType.Into, "Into");
	  AddToken(FormattingTokenType.Group, "Group");
	  AddToken(FormattingTokenType.Skip, "Skip");
	  AddToken(FormattingTokenType.Take, "Take");
	  AddToken(FormattingTokenType.Ascending, "Ascending");
	  AddToken(FormattingTokenType.Descending, "Descending");
	  AddToken(FormattingTokenType.Aggregate, "Aggregate");
	  AddToken(FormattingTokenType.Nothing, "Nothing");
	  AddToken(FormattingTokenType.AddHandler, "AddHandler");
	  AddToken(FormattingTokenType.Add, "Add");
	  AddToken(FormattingTokenType.Remove, "Remove");
	  AddToken(FormattingTokenType.AddressOf, "AddressOf");
	  AddToken(FormattingTokenType.Alias, "Alias");
	  AddToken(FormattingTokenType.And, "And");
	  AddToken(FormattingTokenType.AndAlso, "AndAlso");
	  AddToken(FormattingTokenType.Boolean, "Boolean");
	  AddToken(FormattingTokenType.ByRef, "ByRef");
	  AddToken(FormattingTokenType.ByVal, "ByVal");
	  AddToken(FormattingTokenType.Char, "Char");
	  AddToken(FormattingTokenType.Call, "Call");
	  AddToken(FormattingTokenType.CBool, "CBool");
	  AddToken(FormattingTokenType.CByte, "CByte");
	  AddToken(FormattingTokenType.CChar, "CChar");
	  AddToken(FormattingTokenType.CDate, "CDate");
	  AddToken(FormattingTokenType.CDbl, "CDbl");
	  AddToken(FormattingTokenType.CInt, "CInt");
	  AddToken(FormattingTokenType.CLng, "CLng");
	  AddToken(FormattingTokenType.CObj, "CObj");
	  AddToken(FormattingTokenType.CSByte, "CSByte");
	  AddToken(FormattingTokenType.CShort, "CShort");
	  AddToken(FormattingTokenType.CSng, "CSng");
	  AddToken(FormattingTokenType.CStr, "CStr");
	  AddToken(FormattingTokenType.CType, "CType");
	  AddToken(FormattingTokenType.CUInt, "CUInt");
	  AddToken(FormattingTokenType.CULng, "CULng");
	  AddToken(FormattingTokenType.CUShort, "CUShort");
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
	  AddToken(FormattingTokenType.Function, "Function");
	  AddToken(FormattingTokenType.Get, "Get");
	  AddToken(FormattingTokenType.GetType, "GetType");
	  AddToken(FormattingTokenType.GetXmlNamespace, "GetXmlNamespace");
	  AddToken(FormattingTokenType.Global, "Global");
	  AddToken(FormattingTokenType.GoSub, "GoSub");
	  AddToken(FormattingTokenType.Handles, "Handles");
	  AddToken(FormattingTokenType.Implements, "Implements");
	  AddToken(FormattingTokenType.Imports, "Imports");
	  AddToken(FormattingTokenType.Inherits, "Inherits");
	  AddToken(FormattingTokenType.Integer, "Integer");
	  AddToken(FormattingTokenType.Byte, "Byte");
	  AddToken(FormattingTokenType.IsNot, "IsNot");
	  AddToken(FormattingTokenType.Let, "let");
	  AddToken(FormattingTokenType.Lib, "Lib");
	  AddToken(FormattingTokenType.Like, "Like");
	  AddToken(FormattingTokenType.Loop, "Loop");
	  AddToken(FormattingTokenType.Me, "Me");
	  AddToken(FormattingTokenType.Mod, "Mod");
	  AddToken(FormattingTokenType.Module, "Module");
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
	  AddToken(FormattingTokenType.ParamArray, "ParamArray");
	  AddToken(FormattingTokenType.Property, "Property");
	  AddToken(FormattingTokenType.RaiseEvent, "RaiseEvent");
	  AddToken(FormattingTokenType.ReDim, "ReDim");
	  AddToken(FormattingTokenType.REM, "REM");
	  AddToken(FormattingTokenType.RemoveHandler, "RemoveHandler");
	  AddToken(FormattingTokenType.Resume, "Resume");
	  AddToken(FormattingTokenType.Key, "Key");
	  AddToken(FormattingTokenType.Set, "Set");
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
	  AddToken(FormattingTokenType.With, "With");
	  AddToken(FormattingTokenType.WithEvents, "WithEvents");
	  AddToken(FormattingTokenType.WriteOnly, "WriteOnly");
	  AddToken(FormattingTokenType.Xor, "Xor");
	  AddToken(FormattingTokenType.SByteToken, "SByteToken");
	  AddToken(FormattingTokenType.ULongToken, "ULongToken");
	  AddToken(FormattingTokenType.UShortToken, "UShortToken");
	  AddToken(FormattingTokenType.Var, "var");
	  AddToken(FormattingTokenType.Equals, "Equals");
	  AddToken(FormattingTokenType.OrderBy, "Order By");
	  AddToken(FormattingTokenType.IfDirective, "#If");
	  AddToken(FormattingTokenType.ElifDirective, "#ElIf");
	  AddToken(FormattingTokenType.ElseDirective, "#Else");
	  AddToken(FormattingTokenType.EndIfDirective, "#End If");
	  AddToken(FormattingTokenType.DefineDirective, "#Define");
	  AddToken(FormattingTokenType.UndefDirective, "#Undef");
	  AddToken(FormattingTokenType.LineDirective, "#Line");
	  AddToken(FormattingTokenType.ErrorDirective, "#Error");
	  AddToken(FormattingTokenType.WarningDirective, "#Warning");
	  AddToken(FormattingTokenType.RegionDirective, "#Region");
	  AddToken(FormattingTokenType.EndRegionDirective, "#End Region");
	  AddToken(FormattingTokenType.PragmaDirective, "#Pragma");
	  AddToken(FormattingTokenType.GreatThanGreatThanEqual, ">>=");
	  AddToken(FormattingTokenType.GreatThanGreatThan, ">>");
	  AddToken(FormattingTokenType.GreatThanGreatThanGreatThanEqual, ">>>=");
	  AddToken(FormattingTokenType.BackSlashEqual, "\\=");
	  AddToken(FormattingTokenType.InstanceOf, "InstanceOf");
	  AddToken(FormattingTokenType.Delete, "Delete");
	  AddToken(FormattingTokenType.LineContinuation, "_");
	  AddToken(FormattingTokenType.SingleQuote, "'");
	  AddToken(FormattingTokenType.NumberSign, "#");
	  AddToken(FormattingTokenType.DollarSign, "$");
	}
	public static FormattingTable Instance
	{
	  get
	  {
		if (_Instance == null)
		  _Instance = new VBFormattingTable();
		return _Instance;
	  }
	}
  }
}
