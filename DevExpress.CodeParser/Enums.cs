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
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
  public enum PrimitiveType : byte
  {
	Undefined = 0,
	Boolean = 1,
	Byte = 2,
	SByte = 3,
	Char = 4,
	DBNull = 5,
	Int16 = 6,
	Int32 = 7,
	Int64 = 8,
	UInt16 = 9,
	UInt32 = 10,
	UInt64 = 11,
	Single = 12,
	Double = 13,
	Decimal = 14,
	DateTime = 15,
	Object = 16,
	String = 17,
	Void = 18,
	RegularExpression = 19,
	Path = 20
  }
  public enum SearchScope
  {
	ThisDeclaration,
	AllPartialClasses
  }
  public enum MemberAccesOperatorType
  {
	Default,
	Nested,
	Pointer
  }
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Flags]
  public enum MethodCharacteristics
  {
	Abstract = 0x01,
	Virtual = 0x02,
	Override = 0x04,
	New = 0x08,
	Extern = 0x10,
	ClassOperator = 0x20,
	MainProcedure = 0x40,
	PinPtrPointer = 0x80,
	InteriorPtrPointer = 0x100,
	Generic = 0x200,
	InterfaceImplementer = 0x400,
	EventHandler = 0x800,
	InClass = 0x1000,
	InStruct = 0x2000,
	InModule = 0x4000,
	InInterface = 0x8000,
	IsExtensionMethod = 0x10000,
	IsWebMethod = 0x20000
  }
}
