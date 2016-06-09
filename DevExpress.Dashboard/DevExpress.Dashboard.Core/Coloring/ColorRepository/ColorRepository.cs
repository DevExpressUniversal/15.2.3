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
namespace DevExpress.DashboardCommon.Native {
	public class ColorRepository {
		Dictionary<ColorRepositoryKey, ServerColorTable> records = new Dictionary<ColorRepositoryKey, ServerColorTable>(new ColorRepositoryKeyComparer());
		public Dictionary<ColorRepositoryKey, ServerColorTable> Records { get { return records; } }
		public ColorRepository Clone() {
			ColorRepository clone = new ColorRepository();
			clone.records = records.ToDictionary(row => row.Key, row => row.Value.Clone(), new ColorRepositoryKeyComparer());
			return clone;
		}
		public void Assign(ColorRepository repository) {
			List<ColorRepositoryKey> keysToRemove = new List<ColorRepositoryKey>();
			foreach (KeyValuePair<ColorRepositoryKey, ServerColorTable> localRow in this.records) {
				ServerColorTable repositoryTable;
				if (repository.Records.TryGetValue(localRow.Key, out repositoryTable)) {
					localRow.Value.Assign(repositoryTable);
				}
				else {
					keysToRemove.Add(localRow.Key);
				}
			}
			foreach (ColorRepositoryKey keyToRemove in keysToRemove)
				this.records.Remove(keyToRemove);
			foreach (KeyValuePair<ColorRepositoryKey, ServerColorTable> repositoryRow in repository.Records) {
				if (!this.records.ContainsKey(repositoryRow.Key)) {
					this.records.Add(repositoryRow.Key, repositoryRow.Value);
				}
			}
		}
		public ColorTablePair GetTable(ColorRepositoryKey repositoryKey, bool withMeasures, bool createNew) {
			ColorTablePair colorTable = GetTable(repositoryKey);
			if (colorTable.Key == null && createNew) {
				ServerColorTable table = CreateTable(repositoryKey.DimensionDefinitions.ToArray());
				ColorRepositoryKey key = new ColorRepositoryKey(repositoryKey.DimensionDefinitions) {
					DataSourceName = repositoryKey.DataSourceName,
					DataMember = repositoryKey.DataMember
				};
				records.Add(key, table);
				return new ColorTablePair(key, table);
			}
			return colorTable;
		}
		public void SetColorSchemeEntries(IList<ColorSchemeEntry> colorScheme) {
			records.Clear();
			foreach (ColorSchemeEntry entry in colorScheme) {
				MeasureDefinition[] measureDefinitions = entry.MeasureKey != null ? entry.MeasureKey.MeasureDefinitions.ToArray() : null;
				DimensionDefinition[] definitions = entry.DimensionKeys.Select(key => key.DimensionDefinition).ToArray();
				ServerColorTable table = GetTable(definitions, entry.DataSourceName, entry.DataMember, measureDefinitions != null, true).Table;
				object[] dimensionValue = entry.DimensionKeys.Select(key => key.Value).ToArray();
				ColorDefinition schemeDefinition = entry.ColorDefinition;
				ColorDefinitionBase repositoryDefinition = null;
				if (schemeDefinition == null)
					repositoryDefinition = UserColor.Empty;
				else if (schemeDefinition.PaletteIndex != null)
					repositoryDefinition = new PaletteColor(schemeDefinition.PaletteIndex.Value);
				else if (schemeDefinition.Color != null)
					repositoryDefinition = new UserColor(schemeDefinition.Color.Value);
				table.SetColor(dimensionValue, measureDefinitions, repositoryDefinition);
			}
		}
		public IList<ColorSchemeEntry> GetColorSchemeEntries() {
			List<ColorSchemeEntry> entries = new List<ColorSchemeEntry>();
			foreach (KeyValuePair<ColorRepositoryKey, ServerColorTable> repositoryRecord in records) {
				ServerColorTable table = repositoryRecord.Value;
				foreach (KeyValuePair<ColorTableServerKey, ColorDefinitionBase> tableRecord in table.Rows) {
					object[] dimensionValues = tableRecord.Key.DimensionValues;
					MeasureDefinition[] measureDefinitions = tableRecord.Key.Measures;
					ColorSchemeEntry entry = new ColorSchemeEntry(repositoryRecord.Key.DataSourceName, repositoryRecord.Key.DataMember);
					if (dimensionValues != null)
						for (int i = 0; i < dimensionValues.Length; i++)
							entry.DimensionKeys.Add(new ColorSchemeDimensionKey(repositoryRecord.Key.DimensionDefinitions[i], dimensionValues[i]));
					if (measureDefinitions != null && measureDefinitions.Length > 0)
						entry.MeasureKey = new ColorSchemeMeasureKey(measureDefinitions);
					entry.ColorDefinition = tableRecord.Value.ToColorSchemeDefinition();
					entries.Add(entry);
				}
			}
			return entries;
		}
		public ColorTablePair GetTable(DimensionDefinition[] definitions, string dataSourceName, string dataMember, bool withMeasures, bool createNew) {
			DimensionDefinition[] actualDefinitions = withMeasures ? ColoringHelper.AddMeasureNames(definitions) : definitions;
			ColorRepositoryKey key = new ColorRepositoryKey(actualDefinitions) {
				DataSourceName = dataSourceName,
				DataMember = dataMember
			};
			return GetTable(key, withMeasures, createNew);
		}
		ColorTablePair GetTable(ColorRepositoryKey repositoryKey) {
			ColorRepositoryKey actualRepositoryKey = records.Keys.FirstOrDefault(key => new ColorRepositoryKeyComparer().Equals(key, repositoryKey));
			return actualRepositoryKey != null ? new ColorTablePair(actualRepositoryKey, records[actualRepositoryKey]) : new ColorTablePair();
		}
		ServerColorTable CreateTable(DimensionDefinition[] dimensions) {
			return new ServerColorTable();
		}
	}
}
