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
namespace DevExpress.PivotGrid.OLAP {
	public static class OLAPDataTypeConverter {
		public static Type Convert(OLAPDataType type) {
			switch(type) {
				case OLAPDataType.BigInt: return typeof(Int64);
				case OLAPDataType.Binary: return typeof(string);
				case OLAPDataType.Boolean: return typeof(Boolean);
				case OLAPDataType.BSTR: return typeof(String);
				case OLAPDataType.Char: return typeof(String);
				case OLAPDataType.Currency: return typeof(Decimal);
				case OLAPDataType.Date: return typeof(DateTime);
				case OLAPDataType.DBDate: return typeof(DateTime);
				case OLAPDataType.DBTime: return typeof(TimeSpan);
				case OLAPDataType.DBTimeStamp: return typeof(DateTime);
				case OLAPDataType.Decimal: return typeof(Decimal);
				case OLAPDataType.Double: return typeof(Double);
				case OLAPDataType.Empty: return typeof(object);
				case OLAPDataType.Error: return typeof(Exception);
				case OLAPDataType.Filetime: return typeof(DateTime);
				case OLAPDataType.Guid: return typeof(Guid);
				case OLAPDataType.IDispatch: return typeof(Object);
				case OLAPDataType.Integer: return typeof(Int32);
				case OLAPDataType.IUnknown: return typeof(Object);
				case OLAPDataType.LongVarBinary: return typeof(Byte);
				case OLAPDataType.LongVarChar: return typeof(String);
				case OLAPDataType.LongVarWChar: return typeof(String);
				case OLAPDataType.Numeric: return typeof(Decimal);
				case OLAPDataType.PropVariant: return typeof(Object);
				case OLAPDataType.Single: return typeof(Single);
				case OLAPDataType.SmallInt: return typeof(Int16);
				case OLAPDataType.TinyInt: return typeof(SByte);
				case OLAPDataType.UnsignedBigInt: return typeof(UInt64);
				case OLAPDataType.UnsignedInt: return typeof(UInt32);
				case OLAPDataType.UnsignedSmallInt: return typeof(UInt16);
				case OLAPDataType.UnsignedTinyInt: return typeof(Byte);
				case OLAPDataType.VarBinary: return typeof(Byte);
				case OLAPDataType.VarChar: return typeof(String);
				case OLAPDataType.Variant: return typeof(Object);
				case OLAPDataType.VarNumeric: return typeof(Decimal);
				case OLAPDataType.VarWChar: return typeof(String);
				case OLAPDataType.WChar: return typeof(String);
			}
			return null;
		}
	}
}
