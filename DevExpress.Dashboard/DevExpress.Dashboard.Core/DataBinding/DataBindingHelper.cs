#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.Data.Summary;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native {
	public static class DataBindingHelper {
		static readonly Type[] TextTypes = new Type[] { 
			typeof(string), 
			typeof(char),
			typeof(char?)
		};
		static readonly Type[] TimeSpanTypes = new Type[] {
			typeof(TimeSpan),
			typeof(TimeSpan?)
		};
		static readonly Type[] IntegerTypes = new Type[] {
			typeof(Int16),
			typeof(Int16?),
			typeof(Int32),
			typeof(Int32?),
			typeof(Int64),
			typeof(Int64?),
			typeof(UInt16),
			typeof(UInt16?),
			typeof(UInt32),
			typeof(UInt32?),
			typeof(UInt64),
			typeof(UInt64?),
			typeof(Byte),
			typeof(Byte?),
			typeof(SByte),
			typeof(SByte?)
		};
		static readonly Type[] FloatTypes = new Type[] {
			typeof(Single),
			typeof(Single?)
		};
		static readonly Type[] DoubleTypes = new Type[] {
			typeof(Double),
			typeof(Double?)
		};
		static readonly Type[] DecimalTypes = new Type[] {
			typeof(Decimal),
			typeof(Decimal?)
		};
		static bool ContainsType(Type[] types, Type type) {
			return Array.IndexOf<Type>(types, type) >= 0;
		}
		static bool IsTextType(Type type) {
			return ContainsType(TextTypes, type);
		}
		static bool IsDateTime(Type type) {
			return SummaryItemTypeHelper.IsDateTime(type);
		}
		static bool IsBool(Type type) {
			return SummaryItemTypeHelper.IsBool(type);
		}
		static bool IsInteger(Type type) {
			return ContainsType(IntegerTypes, type);
		}
		static bool IsFloat(Type type) {
			return ContainsType(FloatTypes, type);
		}
		static bool IsDouble(Type type) {
			return ContainsType(DoubleTypes, type);
		}
		static bool IsDecimal(Type type) {
			return ContainsType(DecimalTypes, type);
		}
		public static bool IsTimeSpanType(Type type) {
			return ContainsType(TimeSpanTypes, type);
		}
		public static bool IsEnumType(Type type) {
			return type.IsSubclassOf(typeof(Enum));
		}
		public static DataFieldType GetDataFieldType(Type type) {
			if(IsTextType(type))
				return DataFieldType.Text;
			if(IsDateTime(type))
				return DataFieldType.DateTime;
			if(IsBool(type))
				return DataFieldType.Bool;
			if(IsInteger(type) || IsTimeSpanType(type))
				return DataFieldType.Integer;
			if (IsFloat(type))
				return DataFieldType.Float;
			if (IsDouble(type))
				return DataFieldType.Double;
			if (IsDecimal(type))
				return DataFieldType.Decimal;
			if(IsEnumType(type))
				return DataFieldType.Enum;
			return DataFieldType.Custom;
		}
		public static bool IsNumericType(Type type) {
			return IsFloat(type) || IsDouble(type) || IsInteger(type) || IsDecimal(type);
		}
		public static bool IsText(DataFieldType fieldType) {
			return fieldType == DataFieldType.Text;
		}
		public static bool IsBoolean(DataFieldType fieldType) {
			return fieldType == DataFieldType.Bool;
		}
		public static bool IsDateTime(DataFieldType fieldType) {
			return fieldType == DataFieldType.DateTime;
		}
		public static bool IsNumeric(DataFieldType fieldType) {
			return fieldType == DataFieldType.Integer ||
				fieldType == DataFieldType.Float ||
				fieldType == DataFieldType.Double ||
				fieldType == DataFieldType.Decimal;
		}
		public static bool IsCustom(DataFieldType fieldType) {
			return fieldType == DataFieldType.Custom;
		}
		public static DataFieldFilterEditorType GetEditorType(Type type) {			
			if(IsDateTime(type))
				return DataFieldFilterEditorType.DateTime;
			if(IsEnumType(type))
				return DataFieldFilterEditorType.ComboBox;
			return DataFieldFilterEditorType.Text;
		}
		public static IList GetEditorValues(Type type) {
			if(IsEnumType(type))
				return Enum.GetValues(type);
			return null;
		}
	}
}
