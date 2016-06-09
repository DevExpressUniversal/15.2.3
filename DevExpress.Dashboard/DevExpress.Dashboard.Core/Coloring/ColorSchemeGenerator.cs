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

using System.Collections.Generic;
using System.Linq;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native {
	public class ColorSchemeGenerator {
		public static ColorRepository GenerateColoringCache(IColorSchemeProvider provider, ColorRepository repository, IActualParametersProvider parameters) {
			ColorSchemeGenerator colorGenerator = new ColorSchemeGenerator(provider, repository, new DashboardPalette().ColorsCount);
			colorGenerator.GenerateColoring(parameters);
			return colorGenerator.Cache;
		}
		readonly IColorSchemeProvider provider;
		readonly ColorRepository repository;
		readonly ColoringSchemeDefinition schemeDefinition;
		readonly int paletteColorCount;
		SortedDictionary<int, int> occupiedColors = new SortedDictionary<int, int>();
		public ColorRepository Cache { get; private set; }
		public ColorSchemeGenerator(IColorSchemeProvider provider, ColorRepository repository, int paletteColorsCount, ColorRepository cache = null) {
			Guard.ArgumentNotNull(provider, "provider");
			Guard.ArgumentNotNull(repository, "repository");
			this.provider = provider;
			this.repository = repository;
			this.schemeDefinition = provider.GetColoringScheme();
			this.paletteColorCount = paletteColorsCount;
			this.Cache = cache;
		}
		public void GenerateColoring(IActualParametersProvider parameters) {
			ColorRepository cache = new ColorRepository();
			occupiedColors.Clear();
			foreach(KeyValuePair<ColorRepositoryKey, MeasureDefinition[][]> coloringDefinition in schemeDefinition) {
				ColorRepositoryKey key = coloringDefinition.Key;
				ColorTablePair table = BuildTable(key, coloringDefinition.Value, parameters);
				cache.Records.Add(table.Key, table.Table);
			}
			Cache = cache;
		}
		List<int> OccupyUserColors(ServerColorTable table) {
			List<int> internalOccupiedColors = new List<int>();
			foreach(KeyValuePair<ColorTableServerKey, ColorDefinitionBase> tableRow in table.Rows) {
				PaletteColor paletteColor = tableRow.Value as PaletteColor;
				if(paletteColor != null) {
					int index = paletteColor.ColorIndex;
					internalOccupiedColors.Add(index);
					OccupyColor(index);
				}
			}
			return internalOccupiedColors;
		}
		ServerColorTable MergeTables(ServerColorTable primaryTable, ServerColorTable secondaryTable) {
			foreach(KeyValuePair<ColorTableServerKey, ColorDefinitionBase> row in secondaryTable.Rows) {
				if(!primaryTable.Rows.ContainsKey(row.Key)) {
					primaryTable.Rows.Add(row.Key, row.Value);
				}
			}
			return primaryTable;
		}
		ColorTablePair BuildTable(ColorRepositoryKey repositoryKey, MeasureDefinition[][] measures, IActualParametersProvider parameters) {
			ColorTablePair result = repository.GetTable(repositoryKey, measures != null && measures.Length > 0, false);
			result = result.Key != null ? new ColorTablePair(result.Key, result.Table.Clone()) : new ColorTablePair(repositoryKey, new ServerColorTable());
			if(repositoryKey.IsDataSourceSpecified) {
				ColorRepositoryKey anyDataSourceKey = new ColorRepositoryKey(repositoryKey.DimensionDefinitions);
				ColorTablePair anyDataSourceTable = repository.GetTable(anyDataSourceKey, measures != null && measures.Length > 0, false);
				if(anyDataSourceTable.Key != null)
					result.Table = MergeTables(result.Table, anyDataSourceTable.Table.Clone());
			}
			ColorTablePair cachedTable = Cache != null ? Cache.GetTable(repositoryKey, measures != null && measures.Length > 0, false) : new ColorTablePair();
			IList<object[]> values = cachedTable.Key != null ? GetUniqueValuesFromTable(cachedTable.Table) : provider.GetColoringValues(repositoryKey, parameters);
			values = ColoringHelper.ApplyDefaultSorting(values);
			if(cachedTable.Key != null)
				result.Key = cachedTable.Key;
			result.Table = Initialize(result.Table, values, measures, OccupyUserColors(result.Table));
			return result;
		}
		IList<object[]> GetUniqueValuesFromTable(ServerColorTable cachedTable) {
			return cachedTable.Rows.Keys.Select(key => key.DimensionValues).Where(value => value != null).Distinct(new OrderedArrayComparer()).ToList();
		}
		ServerColorTable Initialize(ServerColorTable existingTable, IList<object[]> values, MeasureDefinition[][] measures, List<int> internalOccupiedColors) {
			ServerColorTable resultTable = new ServerColorTable();
			if(measures != null && measures.Length > 0) {
				foreach(MeasureDefinition[] measure in measures) {
					if(values != null && values.Count > 0) {
						foreach(object[] value in values)
							AddCore(resultTable, existingTable, value, measure, internalOccupiedColors);
					}
					else
						AddCore(resultTable, existingTable, null, measure, internalOccupiedColors);
				}
			}
			else if(values.Count > 0) {
				foreach(object[] value in values)
					AddCore(resultTable, existingTable, value, null, internalOccupiedColors);
			}
			else
				resultTable.SetColor(null, null, new AutoAssignedColor(0));
			return resultTable;
		}
		void AddCore(ServerColorTable resultTable, ServerColorTable existingTable, object[] value, MeasureDefinition[] measures, List<int> internalOccupiedColors) {
			if(resultTable.ContainsColor(value, measures))
				return;
			if(existingTable.ContainsColor(value, measures)) {
				resultTable.SetColor(value, measures, existingTable.GetColor(value, measures));
				return;
			}
			int index = CalculatePaletteIndex(internalOccupiedColors);
			resultTable.SetColor(value, measures, new AutoAssignedColor(index % paletteColorCount));
			OccupyColor(index % paletteColorCount);
			if(index != -1)
				internalOccupiedColors.Add(index);
		}
		int CalculatePaletteIndex(List<int> internalOccupiedColors) {
			if(occupiedColors.Count < paletteColorCount) {
				for(int i = 0; i < paletteColorCount; i++) {
					if(!occupiedColors.ContainsKey(i))
						return i;
				}
			}
			else {
				int minCount = occupiedColors.Values.Min();
				var paletteUnUsedIndexes = occupiedColors.Where(pair => !internalOccupiedColors.Select(ind => ind % paletteColorCount).Contains(pair.Key)).ToArray();
				if(paletteUnUsedIndexes.Count() > 0) {
					int subMinCount = paletteUnUsedIndexes.Min(pair => pair.Value);
					return paletteUnUsedIndexes.First(pair => pair.Value == subMinCount).Key + subMinCount * paletteColorCount;
				}
				else
					return occupiedColors.First(pair => pair.Value == minCount).Key + minCount * paletteColorCount;
			}
			return -1;
		}
		void OccupyColor(int index) {
			if(!occupiedColors.ContainsKey(index))
				occupiedColors.Add(index, 0);
			occupiedColors[index] = occupiedColors[index] + 1;
		}
	}
}
