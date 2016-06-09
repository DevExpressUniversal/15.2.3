﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
  public enum FormattingTokenType : short
  {
	None,
	SourceFileStart,
	WhiteSpace,
	NewLine,
	Fictive,
	Abstract, 
	As, 
	Async, 
	Await, 
	Base, 
	BackSlashEqual,
	Bool, 
	Break, 
	Byte, 
	Case, 
	Catch, 
	Char, 
	Checked, 
	Class, 
	Const, 
	Continue, 
	Decimal, 
	Default, 
	Delegate, 
	Do, 
	DollarSign, 
	Double, 
	Else, 
	Enum, 
	Equals,
	Event, 
	Explicit, 
	Extern, 
	False, 
	Finally, 
	Fixed, 
	Float, 
	For, 
	Foreach, 
	GreatThanGreatThanEqual,
	GreatThanGreatThanGreatThanEqual,
	If, 
	Implicit, 
	In, 
	Int, 
	Interface, 
	Internal, 
	Is, 
	Lock, 
	Long, 
	Namespace, 
	New, 
	Null, 
	Object, 
	Operator, 
	OrderBy,
	Out, 
	Override, 
	Params, 
	Private, 
	Protected, 
	Public, 
	ReadOnly, 
	Ref, 
	Return, 
	Sbyte, 
	Sealed, 
	Short, 
	Sizeof, 
	Stackalloc, 
	Static, 
	Struct, 
	Switch, 
	This, 
	Throw, 
	True, 
	Try, 
	TypeOf, 
	Uint, 
	Ulong, 
	Unchecked, 
	Unsafe, 
	Ushort, 
	Using, 
	Var,
	Virtual, 
	Void, 
	Volatile, 
	While, 
	Ampersand, 
	AmpersandEqual, 
	Equal, 
	AtColon, 
	At, 
	Colon, 
	Comma, 
	MinusMinus, 
	SlashEqual, 
	Dot, 
	ColonColon, 
	EqualEqual, 
	GreaterThen, 
	GreaterThenEqual, 
	PlusPlus, 
	CurlyBraceOpen, 
	BracketOpen, 
	ParenOpen, 
	LessThanLessThanEqual, 
	LessThan, 
	LessThanLessThan, 
	Minus, 
	MinusEqual, 
	PercentEqual, 
	ExclamationEqual, 
	Exclamation, 
	VerticalBarEqual, 
	Plus, 
	PlusEqual, 
	Question, 
	CurlyBraceClose, 
	BracketClose, 
	ParenClose, 
	Semicolon, 
	Tilde, 
	Asterisk, 
	AsteriskEqual, 
	CaretEqual, 
	MinusGreaterThenAsterisk, 
	Partial, 
	Yield, 
	QuestionQuestion, 
	VerticalBarVerticalBar, 
	AmpersandAmpersand, 
	VerticalBar, 
	Caret, 
	LessThanEqual, 
	Slash, 
	Percent, 
	MinusGreaterThen, 
	EqualGreaterThen, 
	Ident,
	Overloads,
	By,
	From,
	Order,
	Select,
	On,
	Where,
	Join,
	Into,
	Group,
	Skip,
	Take,
	Ascending,
	Descending,
	Aggregate,
	Nothing, 
	AddHandler,
	AddressOf,
	Alias,
	And,
	AndAlso,
	Boolean,
	ByRef,
	ByVal,
	Call,
	CBool,
	CByte,
	CChar,
	CDate,
	CDbl,
	CDec,
	CInt,
	CLng,
	CObj,
	CSByte,
	CShort,
	CSng,
	CStr,
	CType,
	CUInt,
	CULng,
	CUShort,
	BackSlash,
	LessThanGreaterThan,
	Date,
	Declare,
	Dim,
	DirectCast,
	Each,
	ElseIf,
	End,
	EndIf,
	Erase,
	Error,
	Exit,
	Friend,
	Function,
	Get,
	GetType,
	GetXmlNamespace,
	Global,
	GoSub,
	GoTo,
	Handles,
	Implements,
	Imports,
	Inherits,
	Integer,
	IsNot,
	Let,
	Lib,
	Like,
	Loop,
	Me,
	Mod,
	Module,
	MustInherit,
	MustOverride,
	MyBase,
	MyClass,
	Narrowing,
	Next,
	Not,
	NotInheritable,
	NotOverridable,
	Of,
	Option,
	Optional,
	Or,
	OrElse,
	Overridable,
	Overrides,
	ParamArray,
	Property,
	RaiseEvent,
	ReDim,
	REM,
	RemoveHandler,
	Resume,
	Set,
	Shadows,
	Shared,
	Single,
	Step,
	Stop,
	String,
	Structure,
	Sub,
	SyncLock,
	Then,
	To,
	TryCast,
	UInteger,
	Variant,
	Wend,
	When,
	Widening,
	With,
	WithEvents,
	WriteOnly,
	Xor,
	SByteToken,
	ULongToken,
	UShortToken,
	IfDirective,
	ElseDirective,
	ElifDirective,
	EndIfDirective,
	DefineDirective, 
	UndefDirective, 
	LineDirective, 
	ErrorDirective, 
	WarningDirective, 
	RegionDirective,  
	EndRegionDirective, 
	PragmaDirective, 
	Add,
	Remove,
	InstanceOf,
	Delete,
	Assembly,
	Constructor,
	Field,
	Method,
	Parameter,
	Param,
	ReturnValue,
	EqualEqualEqual,
	ExclamationEqualEqual,
	GreatThanGreatThanGreatThan,
	GreatThanGreatThan,
	DotDotDot,
	Key,
	WhitespaceUnderline,
	IIf,
	NumberSign,
	ColonEquals,
	Distinct,
	Infer,
	Strict,
	Compare,
	Binary,
	Text,
	Off,
	Preserve,
	Until,
	Finalize,
	Custom,
	PCDATA,
	Notation,
	System,
	NData,
	Entity,
	Page,
	XmlCommentClose,
	XmlCommentOpen,
	Grad,
	Rad,
	Deg,
	Khz,
	Hz,
	Px,
	Cm,
	Mm,
	Pt,
	Pc,
	Em,
	Ex,
	Ms,
	S,
	Important,
	TildeEqual,
	Doctype,
	ExclamationElement,
	Empty,
	LessThanQuestionXml,
	Version,
	Standalone,
	Encording,
	AtList,
	CDATA,
	BracketOpenBracketOpenGreatThen,
	LineContinuation,
	IsTrue,
	IsFalse,
	SingleQuote
  }
}
