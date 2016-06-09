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

namespace DevExpress.PivotGrid.OLAP {
#if SL || DXPORTABLE
	public enum OLAPDataType {
		Empty = 0,
		SmallInt = 2,
		Integer = 3,
		Single = 4,
		Double = 5,
		Currency = 6,
		Date = 7,
		BSTR = 8,
		IDispatch = 9,
		Error = 10,
		Boolean = 11,
		Variant = 12,
		IUnknown = 13,
		Decimal = 14,
		TinyInt = 16,
		UnsignedTinyInt = 17,
		UnsignedSmallInt = 18,
		UnsignedInt = 19,
		BigInt = 20,
		UnsignedBigInt = 21,		
		Filetime = 64,
		Guid = 72,
		Binary = 128,
		Char = 129,
		WChar = 130,
		Numeric = 131,
		DBDate = 133,
		DBTime = 134,
		DBTimeStamp = 135,
		PropVariant = 138,
		VarNumeric = 139,
		VarChar = 200,
		LongVarChar = 201,
		VarWChar = 202,
		LongVarWChar = 203,
		VarBinary = 204,
		LongVarBinary = 205,
	}
#else
	public enum OLAPDataType {
		Empty = System.Data.OleDb.OleDbType.Empty,
		SmallInt = System.Data.OleDb.OleDbType.SmallInt,
		Integer = System.Data.OleDb.OleDbType.Integer,
		Single = System.Data.OleDb.OleDbType.Single,
		Double = System.Data.OleDb.OleDbType.Double,
		Currency = System.Data.OleDb.OleDbType.Currency,
		Date = System.Data.OleDb.OleDbType.Date,
		BSTR = System.Data.OleDb.OleDbType.BSTR,
		IDispatch = System.Data.OleDb.OleDbType.IDispatch,
		Error = System.Data.OleDb.OleDbType.Error,
		Boolean = System.Data.OleDb.OleDbType.Boolean,
		Variant = System.Data.OleDb.OleDbType.Variant,
		IUnknown = System.Data.OleDb.OleDbType.IUnknown,
		Decimal = System.Data.OleDb.OleDbType.Decimal,
		TinyInt = System.Data.OleDb.OleDbType.TinyInt,
		UnsignedTinyInt = System.Data.OleDb.OleDbType.UnsignedTinyInt,
		UnsignedSmallInt = System.Data.OleDb.OleDbType.UnsignedSmallInt,
		UnsignedInt = System.Data.OleDb.OleDbType.UnsignedInt,
		BigInt = System.Data.OleDb.OleDbType.BigInt,
		UnsignedBigInt = System.Data.OleDb.OleDbType.UnsignedBigInt,
		Filetime = System.Data.OleDb.OleDbType.Filetime,
		Guid = System.Data.OleDb.OleDbType.Guid,
		Binary = System.Data.OleDb.OleDbType.Binary,
		Char = System.Data.OleDb.OleDbType.Char,
		WChar = System.Data.OleDb.OleDbType.WChar,
		Numeric = System.Data.OleDb.OleDbType.Numeric,
		DBDate = System.Data.OleDb.OleDbType.DBDate,
		DBTime = System.Data.OleDb.OleDbType.DBTime,
		DBTimeStamp = System.Data.OleDb.OleDbType.DBTimeStamp,
		PropVariant = System.Data.OleDb.OleDbType.PropVariant,
		VarNumeric = System.Data.OleDb.OleDbType.VarNumeric,
		VarChar = System.Data.OleDb.OleDbType.VarChar,
		LongVarChar = System.Data.OleDb.OleDbType.LongVarChar,
		VarWChar = System.Data.OleDb.OleDbType.VarWChar,
		LongVarWChar = System.Data.OleDb.OleDbType.LongVarWChar,
		VarBinary = System.Data.OleDb.OleDbType.VarBinary,
		LongVarBinary = System.Data.OleDb.OleDbType.LongVarBinary
	}
#endif
	public enum OLAPCubeType {
		Unknown,
		Cube,
		Dimension
	}
	public enum OLAPMemberType {
		Unknown = 0, 
		Regular = 1, 
		All = 2, 
		Measure = 3, 
		Formula = 4
	}
	public enum OLAPDimensionType {
		Unknown = 0,
		Time = 1,
		Measure = 2,
		Other = 3,
		Quantitative = 5,
		Accounts = 6,
		Customers = 7,
		Products = 8,
		Scenario = 9,		
		Utility = 10,
		Currency = 11,
		Rates = 12,
		Channel = 13,
		Promotion = 14,
		Organization = 15,
		BillOfMaterials = 0x10,
		Geography = 0x11
	}
}
