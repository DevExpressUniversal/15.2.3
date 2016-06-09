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
	sealed class DataProcessingModesInfo : IDataProcessingModesInfo {
		DataAccessTechnologyCodeName codeName;
		IEnumerable<DataProcessingModeCodeName> allowedModes;
		public DataProcessingModesInfo(DataAccessTechnologyCodeName codeName, IEnumerable<DataProcessingModeCodeName> allowedModes) {
			this.codeName = codeName;
			this.allowedModes = allowedModes;
		}
		IDataProcessingModesSet modesCore;
		public IDataProcessingModesSet Modes {
			get {
				if(modesCore == null)
					modesCore = new DataProcessingModeSet(codeName, allowedModes);
				return modesCore;
			}
		}
		internal readonly static IEnumerable<IDataProcessingMode> EmptyModes = new IDataProcessingMode[0];
		class DataProcessingModeSet : IDataProcessingModesSet {
			IEnumerable<IDataProcessingMode> modesCore;
			public DataProcessingModeSet(DataAccessTechnologyCodeName codeName, IEnumerable<DataProcessingModeCodeName> allowed) {
				var predefinedModesSet = CodeNamesResolver.GetModesSet(codeName);
				var allowedModes = System.Linq.Enumerable.Select<DataProcessingModeCodeName, IDataProcessingMode>(
					allowed, (cName) => DataProcessingModes.FromCodeName(cName));
				modesCore = System.Linq.Enumerable.Intersect(predefinedModesSet, allowedModes);
			}
			public IEnumerator<IDataProcessingMode> GetEnumerator() {
				return modesCore.GetEnumerator();
			}
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}
		static class CodeNamesResolver {
			static IDictionary<DataAccessTechnologyCodeName, IDataProcessingModesSet> modeSets;
			static CodeNamesResolver() {
				modeSets = new Dictionary<DataAccessTechnologyCodeName, IDataProcessingModesSet>();
				modeSets.Add(DataAccessTechnologyCodeName.XmlDataSet, new XmlDataSetDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.TypedDataSet, new TypedDataSetDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.SQLDataSource, new SQLDataSourceDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.ExcelDataSource, new ExcelDataSourceDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.LinqToSql, new LinqToSqlDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.EntityFramework, new EntityFrameworkDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.Wcf, new WcfDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.Ria, new RiaDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.IEnumerable, new IEnumerableDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.OLAP, new OLAPDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.Enum, new EnumDataProcessingModesSet());
				modeSets.Add(DataAccessTechnologyCodeName.XPO, new XPODataProcessingModesSet());
			}
			internal static IDataProcessingModesSet GetModesSet(DataAccessTechnologyCodeName codeName) {
				return modeSets[codeName];
			}
		}
	}
}
