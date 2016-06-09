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
using System.CodeDom;
using System.Collections.Generic;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public static class PrimitiveTypeUtils
	{
		public const string SystemVoid = "System.Void";
		public const string SystemString = "System.String";
		public const string SystemObject = "System.Object";
		public const string SystemDateTime = "System.DateTime";
		public const string SystemDecimal = "System.Decimal";
		public const string SystemDouble = "System.Double";
		public const string SystemSingle = "System.Single";
		public const string SystemUInt64 = "System.UInt64";
		public const string SystemUInt32 = "System.UInt32";
		public const string SystemUInt16 = "System.UInt16";
		public const string SystemInt64 = "System.Int64";
		public const string SystemInt32 = "System.Int32";
		public const string SystemInt16 = "System.Int16";
		public const string SystemDBNull = "System.DBNull";
		public const string SystemChar = "System.Char";
		public const string SystemSByte = "System.SByte";
		public const string SystemByte = "System.Byte";
		public const string SystemBoolean = "System.Boolean";
		static private Dictionary<PrimitiveType, List<PrimitiveType>> _PossibleTypesTable;
		static private List<PrimitiveType> _AllPossibleTypes;
		public static string GetFullTypeName(PrimitiveType type)
		{
			switch (type)
			{
				case PrimitiveType.Boolean:
					return String.Intern(SystemBoolean);
				case PrimitiveType.Byte:
					return String.Intern(SystemByte);
				case PrimitiveType.SByte:
					return String.Intern(SystemSByte);
				case PrimitiveType.Char:
					return String.Intern(SystemChar);
				case PrimitiveType.DBNull:
					return String.Intern(SystemDBNull);
				case PrimitiveType.Int16:
					return String.Intern(SystemInt16);
				case PrimitiveType.Int32:
					return String.Intern(SystemInt32);
				case PrimitiveType.Int64:
					return String.Intern(SystemInt64);
				case PrimitiveType.UInt16:
					return String.Intern(SystemUInt16);
				case PrimitiveType.UInt32:
					return String.Intern(SystemUInt32);
				case PrimitiveType.UInt64:
					return String.Intern(SystemUInt64);
				case PrimitiveType.Single:
					return String.Intern(SystemSingle);
				case PrimitiveType.Double:
					return String.Intern(SystemDouble);
				case PrimitiveType.Decimal:
					return String.Intern(SystemDecimal);
				case PrimitiveType.DateTime:
					return String.Intern(SystemDateTime);
				case PrimitiveType.Object:
					return String.Intern(SystemObject);
				case PrimitiveType.String:
					return String.Intern(SystemString);
				case PrimitiveType.Void:
					return String.Intern(SystemVoid);
			}
			return String.Intern(SystemObject);
		}
		public static bool IsPrimitiveType(string fullName)
		{
			PrimitiveType lType = ToPrimitiveType(fullName);
			return lType != PrimitiveType.Undefined;
		}
		public static PrimitiveType ToPrimitiveType(string fullName)
		{
	  string typeName = StructuralParserServicesHolder.ReplaceAccessOperators(fullName);
			switch (typeName)
			{
				case SystemBoolean:
					return PrimitiveType.Boolean;
				case SystemByte:
					return PrimitiveType.Byte;
				case SystemSByte:
					return PrimitiveType.SByte;
				case SystemChar:
					return PrimitiveType.Char;
				case SystemDBNull:
					return PrimitiveType.DBNull;
				case SystemInt16:
					return PrimitiveType.Int16;
				case SystemInt32:
					return PrimitiveType.Int32;
				case SystemInt64:
					return PrimitiveType.Int64;
				case SystemUInt16:
					return PrimitiveType.UInt16;
				case SystemUInt32:
					return PrimitiveType.UInt32;
				case SystemUInt64:
					return PrimitiveType.UInt64;
				case SystemSingle:
					return PrimitiveType.Single;
				case SystemDouble:
					return PrimitiveType.Double;
				case SystemDecimal:
					return PrimitiveType.Decimal;
				case SystemDateTime:
					return PrimitiveType.DateTime;
				case SystemObject:
					return PrimitiveType.Object;
				case SystemString:
					return PrimitiveType.String;
				case SystemVoid:
					return PrimitiveType.Void;
			}
			return PrimitiveType.Undefined;
		}
		public static bool IsSByte(decimal value)
		{
			return value >= sbyte.MinValue && value <= sbyte.MaxValue;
		}
		public static bool IsByte(decimal value)
		{
			return value >= byte.MinValue && value <= byte.MaxValue;
		}
		public static bool IsShort(decimal value)
		{
			return value >= short.MinValue && value <= short.MaxValue;
		}
		public static bool IsUShort(decimal value)
		{
			return value >= ushort.MinValue && value <= ushort.MaxValue;
		}
		public static bool IsInt(decimal value)
		{
			return value >= int.MinValue && value <= int.MaxValue;
		}
		public static bool IsUInt(decimal value)
		{
			return value >= uint.MinValue && value <= uint.MaxValue;
		}
		public static bool IsLong(decimal value)
		{
			return value >= long.MinValue && value <= long.MaxValue;
		}
		public static bool IsULong(decimal value)
		{
			return value >= ulong.MinValue && value <= ulong.MaxValue;
		}
		static private List<PrimitiveType> GetAllPossibleTypes()
		{
			if (_AllPossibleTypes == null)
			{
				_AllPossibleTypes = new List<PrimitiveType>();
				_AllPossibleTypes.Add(PrimitiveType.Int32);
				_AllPossibleTypes.Add(PrimitiveType.UInt32);
				_AllPossibleTypes.Add(PrimitiveType.Int64);
				_AllPossibleTypes.Add(PrimitiveType.UInt64);
				_AllPossibleTypes.Add(PrimitiveType.Single);
				_AllPossibleTypes.Add(PrimitiveType.Double);
				_AllPossibleTypes.Add(PrimitiveType.Decimal);
			}
			return _AllPossibleTypes;
		}
		static private List<PrimitiveType> GetPossibleTypesForInt32()
		{
			List<PrimitiveType> types = new List<PrimitiveType>(GetAllPossibleTypes());
			types.Remove(PrimitiveType.UInt32);
			return types;
		}
		static private List<PrimitiveType> GetPossibleTypesForUInt32()
		{
			List<PrimitiveType> types = new List<PrimitiveType>(GetAllPossibleTypes());
			types.Remove(PrimitiveType.Int32);
			return types;
		}
		static private List<PrimitiveType> GetPossibleTypesForInt64()
		{
			List<PrimitiveType> types = new List<PrimitiveType>(GetAllPossibleTypes());
			types.Remove(PrimitiveType.Int32);
			types.Remove(PrimitiveType.UInt32);
			types.Remove(PrimitiveType.UInt64);
			return types;
		}
		static private List<PrimitiveType> GetPossibleTypesForUInt64()
		{
			List<PrimitiveType> types = new List<PrimitiveType>(GetAllPossibleTypes());
			types.Remove(PrimitiveType.Int32);
			types.Remove(PrimitiveType.UInt32);
			types.Remove(PrimitiveType.Int64);
			return types;
		}
		static private List<PrimitiveType> GetPossibleTypesForSingle()
		{
			List<PrimitiveType> types = new List<PrimitiveType>();
			types.Add(PrimitiveType.Single);
			types.Add(PrimitiveType.Double);
			return types;
		}
		static private List<PrimitiveType> GetPossibleTypesForDecimal()
		{
			List<PrimitiveType> types = new List<PrimitiveType>();
			types.Add(PrimitiveType.Decimal);
			return types;
		}
		static private List<PrimitiveType> GetPossibleTypesForDouble()
		{
			List<PrimitiveType> types = new List<PrimitiveType>();
			types.Add(PrimitiveType.Double);
			return types;
		}
		static private Dictionary<PrimitiveType, List<PrimitiveType>> GetPossibleTypesTable()
		{
			if (_PossibleTypesTable == null)
			{
				_PossibleTypesTable = new Dictionary<PrimitiveType, List<PrimitiveType>>();
				_PossibleTypesTable.Add(PrimitiveType.Byte, GetAllPossibleTypes());
				_PossibleTypesTable.Add(PrimitiveType.SByte, GetAllPossibleTypes());
				_PossibleTypesTable.Add(PrimitiveType.Int16, GetAllPossibleTypes());
				_PossibleTypesTable.Add(PrimitiveType.UInt16, GetAllPossibleTypes());
				_PossibleTypesTable.Add(PrimitiveType.Int32, GetPossibleTypesForInt32());
				_PossibleTypesTable.Add(PrimitiveType.UInt32, GetPossibleTypesForUInt32());
				_PossibleTypesTable.Add(PrimitiveType.Int64, GetPossibleTypesForInt64());
				_PossibleTypesTable.Add(PrimitiveType.UInt64, GetPossibleTypesForUInt64());
				_PossibleTypesTable.Add(PrimitiveType.Single, GetPossibleTypesForSingle());
				_PossibleTypesTable.Add(PrimitiveType.Double, GetPossibleTypesForDouble());
				_PossibleTypesTable.Add(PrimitiveType.Decimal, GetPossibleTypesForDecimal());
			}
			return _PossibleTypesTable;
		}
		static public List<PrimitiveType> GetBinaryOperatorPossibleTypes(PrimitiveType type)
		{
			Dictionary<PrimitiveType, List<PrimitiveType>> table = GetPossibleTypesTable();
			if (table.ContainsKey(type))
				return table[type];
			else
				return new List<PrimitiveType>();
		}
	public static List<PrimitiveType> GetBinaryOperatorPossibleTypes(object value)
	{
	  return GetBinaryOperatorPossibleTypes(value, PrimitiveType.Int32);
	}
		public static List<PrimitiveType> GetBinaryOperatorPossibleTypes(object value, PrimitiveType primitiveType)
		{
			List<PrimitiveType> types = new List<PrimitiveType>();
			if (value is decimal && primitiveType != PrimitiveType.Single && primitiveType != PrimitiveType.Double)
			{
				types.Add(PrimitiveType.Decimal);
				decimal decimalValue = (decimal)value;
				if (IsInt(decimalValue))
					types.Add(PrimitiveType.Int32);
				if (IsUInt(decimalValue))
					types.Add(PrimitiveType.UInt32);
				if (IsLong(decimalValue))
					types.Add(PrimitiveType.Int64);
				if (IsULong(decimalValue))
					types.Add(PrimitiveType.UInt64);
			}
			if (value is float && primitiveType != PrimitiveType.Double)
				types.Add(PrimitiveType.Single);
			if (value is double)
				types.Add(PrimitiveType.Double);
			return types;
		}
		public static List<PrimitiveType> GetPrimitiveTypes(object value)
		{
			if (value is decimal)
				return GetPrimitiveTypes((decimal)value);
			List<PrimitiveType> types = new List<PrimitiveType>();
			if (value is float)
				types.Add(PrimitiveType.Single);
			else if (value is double)
				types.Add(PrimitiveType.Double);
			else if (value is string)
				types.Add(PrimitiveType.String);
			else if (value is bool)
				types.Add(PrimitiveType.Boolean);
			else if (value is char)
				types.Add(PrimitiveType.Char);
			else if (value is DateTime)
				types.Add(PrimitiveType.DateTime);
			else if (value is DBNull)
				types.Add(PrimitiveType.DBNull);
			else if (value == null)
				types.Add(PrimitiveType.Void);
			return types;
		}
		public static PrimitiveType GetPrimitiveType(object value)
		{
			if (value is decimal)
				return GetPrimitiveType((decimal)value);
			if (value is string)
				return PrimitiveType.String;
			if (value is sbyte)
				return PrimitiveType.SByte;
			if (value is byte)
				return PrimitiveType.Byte;
			if (value is ushort)
				return PrimitiveType.UInt16;
			if (value is short)
				return PrimitiveType.Int16;
			if (value is uint)
				return PrimitiveType.UInt32;
			if (value is int)
				return PrimitiveType.Int32;
			if (value is ulong)
				return PrimitiveType.UInt64;
			if (value is long)
				return PrimitiveType.Int64;
			if (value is float)
				return PrimitiveType.Single;
			if (value is double)
				return PrimitiveType.Double;
			if (value is bool)
				return PrimitiveType.Boolean;
			if (value is char)
				return PrimitiveType.Char;
			if (value is DateTime)
				return PrimitiveType.DateTime;
			if (value is DBNull)
				return PrimitiveType.DBNull;
			if (value == null)
				return PrimitiveType.Void;
			return PrimitiveType.Undefined;
		}
		public static List<PrimitiveType> GetPrimitiveTypes(decimal value)
		{
			List<PrimitiveType> types = new List<PrimitiveType>();
			if (IsByte(value))
				types.Add(PrimitiveType.Byte);
			if (IsSByte(value))
				types.Add(PrimitiveType.SByte);
			if (types.Count > 0)
				return types;
			if (IsShort(value))
				types.Add(PrimitiveType.Int16);
			if (IsUShort(value))
				types.Add(PrimitiveType.UInt16);
			if (types.Count > 0)
				return types;
			if (IsInt(value))
				types.Add(PrimitiveType.Int32);
			if (IsUInt(value))
				types.Add(PrimitiveType.UInt32);
			if (types.Count > 0)
				return types;
			if (IsLong(value))
				types.Add(PrimitiveType.Int64);
			if (IsULong(value))
				types.Add(PrimitiveType.UInt64);
			if (types.Count > 0)
				return types;
			types.Add(PrimitiveType.Decimal);
			return types;
		}
		public static PrimitiveType GetPrimitiveType(decimal value)
		{
			if (IsByte(value))
				return PrimitiveType.Byte;
			else if (IsSByte(value))
				return PrimitiveType.SByte;
			else if (IsShort(value))
				return PrimitiveType.Int16;
			else if (IsUShort(value))
				return PrimitiveType.UInt16;
			else if (IsInt(value))
				return PrimitiveType.Int32;
			else if (IsUInt(value))
				return PrimitiveType.UInt32;
			else if (IsLong(value))
				return PrimitiveType.Int64;
			else if (IsULong(value))
				return PrimitiveType.UInt64;
			return PrimitiveType.Int32;
		}
		public static object GetDefaultValue(string fullTypeName)
		{
			return GetDefaultValue(ToPrimitiveType(fullTypeName));
		}
	public static void GetMinAndMaxValues(PrimitiveType type, out decimal minValue, out decimal maxValue)
	{
	  minValue = 0;
	  maxValue = 0;
	  switch (type)
	  {
		case PrimitiveType.SByte:
		  minValue = SByte.MinValue;
		  maxValue = SByte.MaxValue;
		  break;
		case PrimitiveType.Byte:
		  minValue = Byte.MinValue;
		  maxValue = Byte.MaxValue;
		  break;
		case PrimitiveType.Int16:
		  minValue = Int16.MinValue;
		  maxValue = Int16.MaxValue;
		  break;
		case PrimitiveType.Int32:
		  minValue = Int32.MinValue;
		  maxValue = Int32.MaxValue;
		  break;
		case PrimitiveType.Int64:
		  minValue = Int64.MinValue;
		  maxValue = Int64.MaxValue;
		  break;
		case PrimitiveType.UInt16:
		  minValue = UInt16.MinValue;
		  maxValue = UInt16.MaxValue;
		  break;
		case PrimitiveType.UInt32:
		  minValue = UInt32.MinValue;
		  maxValue = UInt32.MaxValue;
		  break;;
		case PrimitiveType.UInt64:
		  minValue = UInt64.MinValue;
		  maxValue = UInt64.MaxValue;
		  break;
		case PrimitiveType.Single:
		  minValue = Decimal.MinValue;
		  maxValue = Decimal.MaxValue;
		  break;
		case PrimitiveType.Double:
		  minValue = Decimal.MinValue;
		  maxValue = Decimal.MaxValue;
		  break;
		case PrimitiveType.Decimal:
		  minValue = Decimal.MinValue;
		  maxValue = Decimal.MaxValue;
		  break;
	  }
	}
		public static object GetDefaultValue(PrimitiveType type)
		{
			switch (type)
			{
				case PrimitiveType.SByte:
					return (SByte)0;
				case PrimitiveType.Byte:
					return (Byte)0;
				case PrimitiveType.Int16:
					return (Int16)0;
				case PrimitiveType.Int32:
					return (Int32)0;
				case PrimitiveType.Int64:
					return (Int64)0;
				case PrimitiveType.UInt16:
					return (UInt16)0;
				case PrimitiveType.UInt32:
					return (UInt32)0;
				case PrimitiveType.UInt64:
					return (UInt64)0;
				case PrimitiveType.Single:
					return 0F;
				case PrimitiveType.Double:
					return 0D;
				case PrimitiveType.Decimal:
					return 0M;
				case PrimitiveType.String:
					return String.Empty;
				case PrimitiveType.DateTime:
					return DateTime.MinValue;
				case PrimitiveType.Boolean:
					return false;
				case PrimitiveType.Char:
					return Char.MinValue;
				case PrimitiveType.Object:
		case PrimitiveType.Void:
					return null;
				case PrimitiveType.DBNull:
					return DBNull.Value;
			}
			return String.Empty;
		}
		public static PrimitiveType ToPrimitiveType(IElement type)
		{
			if (type == null)
				return PrimitiveType.Object;
			return ToPrimitiveType(type.FullName);
		}
		public static PrimitiveType PromoteType(IElement type)
		{
	  return PromoteType(type, UnaryOperatorType.None);
	}
	public static PrimitiveType PromoteType(IElement type, UnaryOperatorType unaryOperator)
		{
			return PromoteType(ToPrimitiveType(type), unaryOperator);
		}
		public static PrimitiveType PromoteType(PrimitiveType type)
		{
	  return PromoteType(type, UnaryOperatorType.None);
	}
	public static PrimitiveType PromoteType(PrimitiveType type, UnaryOperatorType unaryOperator)
		{
			if ((unaryOperator == UnaryOperatorType.UnaryNegation || 
		   unaryOperator == UnaryOperatorType.UnaryPlus ||
		   unaryOperator == UnaryOperatorType.OnesComplement)
		   &&
				(type == PrimitiveType.SByte || 
				type == PrimitiveType.Byte || 
				type == PrimitiveType.Int16 || 
				type == PrimitiveType.UInt16 || 
				   type == PrimitiveType.Char))
				return PrimitiveType.Int32;
			if (unaryOperator == UnaryOperatorType.UnaryNegation && type == PrimitiveType.UInt32)
				return PrimitiveType.Int64;
			return type;
		}
		public static PrimitiveType PromoteTypes(IElement firstType, IElement secondType)
		{
			return PromoteTypes(ToPrimitiveType(firstType), ToPrimitiveType(secondType));
		}
		public static PrimitiveType PromoteTypes(PrimitiveType first, PrimitiveType second)
		{
			if (first == PrimitiveType.String)
				return PrimitiveType.String;
			if (second == PrimitiveType.String)
				return PrimitiveType.String;
			if (first == PrimitiveType.Char)
				return PrimitiveType.Char;
			if (second == PrimitiveType.Char)
				return PrimitiveType.Char;
			if (first == PrimitiveType.Decimal)
				return PrimitiveType.Decimal;
			if (second == PrimitiveType.Decimal)
				return PrimitiveType.Decimal;
			if (first == PrimitiveType.Double)
				return PrimitiveType.Double;
			if (second == PrimitiveType.Double)
				return PrimitiveType.Double;
			if (first == PrimitiveType.Single)
				return PrimitiveType.Single;
			if (second == PrimitiveType.Single)
				return PrimitiveType.Single;
			if (first == PrimitiveType.UInt64)
				return PrimitiveType.UInt64;
			if (second == PrimitiveType.UInt64)
				return PrimitiveType.UInt64;
			if (first == PrimitiveType.Int64)
				return PrimitiveType.Int64;
			if (second == PrimitiveType.Int64)
				return PrimitiveType.Int64;
			if (first == PrimitiveType.UInt32 && 
				((second == PrimitiveType.SByte) ||
				(second == PrimitiveType.Int16) ||
				(second == PrimitiveType.Int32)))
				return PrimitiveType.UInt32;
			if (second == PrimitiveType.UInt32 && 
				((first == PrimitiveType.SByte) ||
				(first == PrimitiveType.Int16) ||
				(first == PrimitiveType.Int32)))
				return PrimitiveType.UInt32;
			if (first == PrimitiveType.UInt32)
				return PrimitiveType.UInt32;
			if (second == PrimitiveType.UInt32)
				return PrimitiveType.UInt32;
			if (first == PrimitiveType.Int32)
				return PrimitiveType.Int32;
			if (second == PrimitiveType.Int32)
				return PrimitiveType.Int32;
			if (first == PrimitiveType.Boolean && second == first)
				return PrimitiveType.Boolean;
			return PrimitiveType.Int32;
		}
		public static bool IsIntegerType(object value)
		{
			return value is sbyte ||
				value is byte ||
				value is ushort ||
				value is short ||
				value is uint ||
				value is int ||
				value is ulong ||
				value is long;
		}
		public static bool IsIntegerType(PrimitiveType type)
		{
			switch (type)
			{
				case PrimitiveType.SByte:
				case PrimitiveType.Byte:
				case PrimitiveType.UInt16:
				case PrimitiveType.Int16:
				case PrimitiveType.UInt32:
				case PrimitiveType.Int32:
				case PrimitiveType.UInt64:
				case PrimitiveType.Int64:
					return true;
			}
			return false;			
		}
	}
}
