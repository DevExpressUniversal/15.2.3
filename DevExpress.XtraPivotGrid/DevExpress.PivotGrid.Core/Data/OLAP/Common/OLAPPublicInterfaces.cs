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
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid {
	public interface IOLAPTitledEntity : IOLAPNamedEntity {
		string Caption { get; }
	}
	public interface IOLAPUniqueEntity : IOLAPTitledEntity {
		string UniqueName { get; }
	}
	public interface IQueryMember {
		object Value { get; }
		IQueryMetadataColumn Column { get; }
		bool IsTotal { get; }
		object UniqueLevelValue { get; }
		bool IsOthers { get; }
	}
	public interface IOLAPEntity {
	}
	public interface IOLAPNamedEntity : IOLAPEntity {
		string Name { get; }
	}
	public interface IOLAPLevel : IOLAPUniqueEntity {
		OLAPLevelType LevelType { get; }
		long Cardinality { get; }
		int LevelNumber { get; }
		int KeyCount { get; }
		string DrillDownColumn { get; }
		string DefaultSortProperty { get; }
		Dictionary<string, OLAPDataType> Properties { get; }
		List<CalculatedMemberSource> CalculatedMembers { get; }
	}
	public interface IOLAPMember : IOLAPUniqueEntity, IQueryMember {
		IOLAPLevel Level { get; }
		OLAPMemberProperties Properties { get; }
		OLAPMemberProperties AutoPopulatedProperties { get; }
	}
	#region OLAP MetaGetter
	public interface IOLAPMetaGetter : IDisposable {
		string ConnectionString { get; set; }
		bool Connected { get; set; }
		List<string> GetCatalogs();
		List<string> GetCubes(string catalogName);
	}
	#endregion
}
namespace DevExpress.XtraPivotGrid.Data {
	#region OLAP Connection Settings
	public interface IOLAPConnectionSettings {
		bool IsValid { get; }
		int ConnectionTimeout { get; set; }
		int LocaleIdentifier { get; set; }
		int QueryTimeout { get; set; }
		string CatalogName { get; set; }
		string CubeName { get; set; }
		string CustomData { get; set; }
		string ConnectionString { get; }
		string FullConnectionString { set; }
		string Password { get; set; }
		string Provider { get; set; }
		string Roles { get; set; }
		string ServerName { get; set; }
		string UserId { get; set; }
	}
	#endregion
	public enum OLAPLevelType {
		Regular = 0x0,
		All = 0x1,
		Calculated = 0x2,
		Time = 0x4,
		Reserved1 = 0x8,
		TimeYears = 0x14,
		TimeHalfYear = 0x24,
		TimeQuarters = 0x44,
		TimeMonths = 0x84,
		TimeWeeks = 0x104,
		TimeDays = 0x204,
		TimeHours = 0x304,
		TimeMinutes = 0x404,
		TimeSeconds = 0x804,
		TimeUndefined = 0x1004,
		GeoContinent = 0x2001,
		GeoRegion = 0x2002,
		GeoCountry = 0x2003,
		GeoStateOrProvince = 0x2004,
		GeoCounty = 0x2005,
		GeoCity = 0x2006,
		GeoPostalCode = 0x2007,
		GeoPoint = 0x2008,
		OrgUnit = 0x1011,
		BomResource = 0x1012,
		Quantitative = 0x1013,
		Account = 0x1014,
		Customer = 0x1021,
		CustomerGroup = 0x1022,
		CustomerHousehold = 0x1023,
		Product = 0x1031,
		ProductGroup = 0x1032,
		Scenario = 0x1015,
		Utility = 0x1016,
		Person = 0x1041,
		Company = 0x1042,
		CurrencySource = 0x1051,
		CurrencyDestination = 0x1052,
		Channel = 0x1061,
		Representative = 0x1062,
		Promotion = 0x1071
	};
}
