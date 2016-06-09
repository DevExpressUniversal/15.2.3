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

namespace DevExpress.Design.DataAccess {
	using System.Collections.Generic;
	sealed class DefaultDataSourceSettingsFactory : IDataSourceSettingsFactory {
		public IDataSourceSettings GetSettings(IDataAccessTechnologyName technologyName, IDataSourceSettingsBuilderContext context) {
			IDataSourceSettingsBuilder builder;
			if(!builders.TryGetValue(technologyName.GetCodeName(), out builder))
				builder = Empty;
			builder.Build(context);
			return new DataSourceSettings(context.Result);
		}
		#region Builders
		static IDictionary<DataAccessTechnologyCodeName, IDataSourceSettingsBuilder> builders;
		static DefaultDataSourceSettingsFactory() {
			Empty = new EmptyBuilder();
			builders = new Dictionary<DataAccessTechnologyCodeName, IDataSourceSettingsBuilder>();
			builders.Add(DataAccessTechnologyCodeName.XmlDataSet, new XmlDataSetSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.TypedDataSet, new TypedDataSetSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.SQLDataSource, new SQLDataSourceSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.ExcelDataSource, new ExcelDataSourceSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.EntityFramework, new EntityFrameworkSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.LinqToSql, new LinqToSqlSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.Wcf, new WcfSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.Ria, new RiaSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.IEnumerable, new IEnumerableSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.OLAP, new OLAPSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.Enum, new EnumItemsSourceSettingsBuilder());
			builders.Add(DataAccessTechnologyCodeName.XPO, new XPOItemsSourceSettingsBuilder());
		}
		static IDataSourceSettingsBuilder Empty = new EmptyBuilder();
		class EmptyBuilder : IDataSourceSettingsBuilder {
			void IDataSourceSettingsBuilder.Build(IDataSourceSettingsBuilderContext context) { }
		}
		#endregion Builders
	}
}
