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
using System.Runtime;
using System.Runtime.InteropServices;
using System.Reflection;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	internal class MemoryHelper
	{
		const int INT_ObjectHeaderSize = 8;
		const int INT_ReferenceSize = 4;
		const int INT_Byte = 1;
	const int INT_SByte = 1;
		const int INT_Int16 = 2;
	const int INT_UInt16 = 2;
		const int INT_Int32 = 4;
	const int INT_UInt32 = 4;
		const int INT_Int64 = 8;
	const int INT_UInt64 = 8;
		const int INT_Char = 2;
		const int INT_Single = 4;
		const int INT_Double = 8;
		const int INT_Decimal = 16;
		const int INT_Boolean = 1;
	const int INT_DateTime = 8;
		const int INT_SourcePoint = 17;
		const int INT_SourceRange = 34;
		const string STR_Byte = "System.Byte";
		const string STR_SByte = "System.SByte";
		const string STR_Int16 = "System.Int16";
		const string STR_UInt16 = "System.UInt16";
		const string STR_Int32 = "System.Int32";
		const string STR_UInt32 = "System.UInt32";
		const string STR_Int64 = "System.Int64";
		const string STR_UInt64 = "System.UInt64";
		const string STR_Char = "System.Char";
		const string STR_Single = "System.Single";
		const string STR_Double = "System.Double";
		const string STR_Decimal = "System.Decimal";
		const string STR_String = "System.String";
		const string STR_Boolean = "System.Boolean";
	const string STR_DateTime = "System.DateTime";
		const string STR_SourcePoint = "DevExpress.CodeRush.StructuralParser.SourcePoint";
		const string STR_SourceRange = "DevExpress.CodeRush.StructuralParser.SourceRange";
		private MemoryHelper() {}
		private static long GetFieldSize(object instance, FieldInfo field)
		{
			if (field == null)
				return 0;
	  Type lType = field.FieldType;
			switch (lType.FullName)
			{
				case STR_Byte:
					return INT_Byte;
		case STR_SByte:
		  return INT_SByte;
				case STR_Int16:
		  return INT_Int16;
		case STR_UInt16:
					return INT_UInt16;
				case STR_Int32:
		  return INT_Int32;
				case STR_UInt32:
					return INT_UInt32;
				case STR_Int64:
		  return INT_Int64;
				case STR_UInt64:
					return INT_UInt64;
				case STR_Char:
					return INT_Char;
				case STR_Single:
					return INT_Single;
				case STR_Double:
					return INT_Double;
				case STR_Decimal:
					return INT_Decimal;
				case STR_Boolean:
					return INT_Boolean;
		case STR_SourcePoint:
		  return INT_SourcePoint;
		case STR_SourceRange:
		  return INT_SourceRange;
		case STR_String:
		  object lValue = field.GetValue(instance);
		  if (lValue == null)
			return 4;
		  return 4 + (((string)lValue).Length * 2) + INT_ObjectHeaderSize;
		default:
		  if (!lType.IsValueType)
			return INT_ReferenceSize;
		  return 0;
			}
		}
		public static long SizeOf(object instance)
		{
			if (instance == null)
				return 0;
	  Type lType = instance.GetType();
			FieldInfo[] lFields = lType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			long lMemory = 0;
			for (int i = 0; i < lFields.Length; i++)
				lMemory += GetFieldSize(instance, lFields[i]);
	  if (!lType.IsValueType)
		lMemory += INT_ObjectHeaderSize;
			return lMemory;
		}
	}
}
