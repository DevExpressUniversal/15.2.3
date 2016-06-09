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

using DevExpress.XtraPivotGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class ColorTableServerKey : IComparable {
		public ColorTableServerKey(object[] dimensionValues, MeasureDefinition[] measures) {
			DimensionValues = dimensionValues;
			Measures = measures;
		}
		public object[] DimensionValues { get; set; }
		public MeasureDefinition[] Measures { get; set; }
		public int CompareTo(object obj) {
			if(obj == null)
				return 1;
			ColorTableServerKey value = obj as ColorTableServerKey;
			if(value == null)
				return 0;
			if(DimensionValues != null && value.DimensionValues == null)
				return 1;
			if(DimensionValues == null && value.DimensionValues != null)
				return -1;
			if(DimensionValues != null && value.DimensionValues != null) {
				int result = ((IComparer<object[]>)new DimensionValueArrayComparer()).Compare(DimensionValues, value.DimensionValues);
				if(result != 0)
					return result;
			}
			if(Measures != null && value.Measures == null)
				return 1;
			if(Measures == null && value.Measures != null)
				return -1;
			if(Measures != null && value.Measures != null) {
				for(int i = 0; i < Math.Min(Measures.Length, value.Measures.Length); i++) {
					int result = Comparer.Default.Compare(DataItemDefinitionDisplayTextProvider.GetMeasureDefinitionString(Measures[i]),
						DataItemDefinitionDisplayTextProvider.GetMeasureDefinitionString(value.Measures[i]));
					if(result != 0)
						return result;
				}
				if(Measures.Length > value.Measures.Length)
					return 1;
				if(Measures.Length < value.Measures.Length)
					return -1;
			}
			return 0;
		}
	}
	public static class ColorTableKeyDisplayTextProvider {
		public static string GetDisplayText(ColorTableServerKey key, IList<FormatterBase> formatters, ColorRepositoryKey repositoryKey, IDataSourceInfoProvider dataProvider) {
			System.Text.StringBuilder str = new System.Text.StringBuilder();
			IDashboardDataSource dataSource = null;
			if (dataProvider != null) {
				DataSourceInfo dataInfo = dataProvider.GetDataSourceInfo(repositoryKey.DataSourceName, repositoryKey.DataMember);
				if (dataInfo != null)
					dataSource = dataInfo.DataSource;
			}			
			string dataMember = repositoryKey.DataMember;
			IDataSourceSchema dataSourceSchemaProvider = dataSource != null ? dataSource.GetDataSourceSchema(dataMember) : null;
			object[] dimensionValues = key.DimensionValues;
			if (dimensionValues != null && dimensionValues.Length > 0) {
				for (int i = 0; i < dimensionValues.Length; i++) {
					if (i > 0)
						str.Append(" | ");
					object value = dimensionValues[i];
					string valueDisplayText = string.Empty;
					if (!DashboardSpecialValuesInternal.TryGetDisplayText(value, out valueDisplayText)) {
						valueDisplayText = value.ToString();
					}
					IOLAPMember member = dataSource != null ? dataSource.GetOlapDimensionMember(repositoryKey.DimensionDefinitions[i].DataMember, value as string, dataMember) : null;
					if (member != null) {
						valueDisplayText = member.Caption;
					}
					else {
						FormatterBase formatter = formatters[i];
						if (formatter != null)
							valueDisplayText = formatter.Format(value);
					}
					str.Append(valueDisplayText);
				}
			}
			MeasureDefinition[] measures = key.Measures;
			if (measures != null) {
				for (int i = 0; i < measures.Length; i++) {
					if (dimensionValues != null && dimensionValues.Length > 0 || i > 0)
						str.Append(" | ");
					str.Append(DataItemDefinitionDisplayTextProvider.GetMeasureDefinitionDisplayText(measures[i], dataSourceSchemaProvider));
				}
			}
			return str.ToString();
		}
	}
	public class ColorTableServerKeyComparer : IEqualityComparer<ColorTableServerKey> {
		public bool Equals(ColorTableServerKey x, ColorTableServerKey y) {
			object[] xDimensionValues = x.DimensionValues;
			object[] yDimensionValues = y.DimensionValues;
			if(!new OrderedArrayComparer().Equals(xDimensionValues, yDimensionValues))
				return false;
			MeasureDefinition[] xMeasures = x.Measures;
			MeasureDefinition[] yMeasures = y.Measures;
			if(!new UnorderedArrayComparer().Equals(xMeasures, yMeasures))
				return false;
			return true;
		}
		public int GetHashCode(ColorTableServerKey obj) {
			return new OrderedArrayComparer().GetHashCode(obj.DimensionValues)
				^ new UnorderedArrayComparer().GetHashCode(obj.Measures);
		}
	}
	public abstract class ArrayComparer : IEqualityComparer<object[]> {
		public abstract bool Equals(object[] x, object[] y);
		public int GetHashCode(object[] obj) {
			int result = 0;
			if(obj != null && obj.Length > 0) {
				for(int i = 0; i < obj.Length; i++) {
					if(obj[i] != null)
						result = result ^ obj[i].GetHashCode();
				}
			}
			return result;
		}
	}
	public class OrderedArrayComparer : ArrayComparer {
		public override bool Equals(object[] x, object[] y) {
			bool xEmpty = x == null || x.Length == 0;
			bool yEmpty = y == null || y.Length == 0;
			if(xEmpty && yEmpty)
				return true;
			if(xEmpty || yEmpty)
				return false;
			if(x.Length != y.Length)
				return false;
			for(int i = 0; i < x.Length; i++) {
				if(x[i] == null && y[i] == null)
					continue;
				if(x[i] == null || y[i] == null)
					return false;
				if(!x[i].Equals(y[i]))
					return false;
			}
			return true;
		}
	}
	public class UnorderedArrayComparer : ArrayComparer {
		public override bool Equals(object[] x, object[] y) {
			bool xEmpty = x == null || x.Length == 0;
			bool yEmpty = y == null || y.Length == 0;
			if(xEmpty && yEmpty)
				return true;
			if(xEmpty || yEmpty)
				return false;
			if(x.Length != y.Length)
				return false;
			for(int i = 0; i < x.Length; i++)
				if(!x.Contains(y[i]))
					return false;
			return true;
		}
	}
	public class DimensionValueArrayComparer : IComparer<object[]> {
		int IComparer<object[]>.Compare(object[] x, object[] y) {
			return CompareInternal(x, y, 0);
		}
		int CompareInternal(object[] x, object[] y, int i) {
			if(x != null && y == null)
				return 1;
			if(x == null && y == null)
				return 0;
			if(x == null && y != null)
				return -1;
			if(x.Length > i && y.Length <= i)
				return 1;
			if(x.Length <= i && y.Length <= i)
				return 0;
			if(x.Length <= i && y.Length > i)
				return -1;
			if(x[i].GetType() != y[i].GetType())
				return Comparer.Default.Compare(x[i].GetType().Name, y[i].GetType().Name);
			if(!DashboardSpecialValues.IsOthersValue(x[i]) && DashboardSpecialValues.IsOthersValue(y[i]))
				return -1;
			if(DashboardSpecialValues.IsOthersValue(x[i]) && !DashboardSpecialValues.IsOthersValue(y[i]))
				return 1;
			if(!DashboardSpecialValues.IsNullValue(x[i]) && DashboardSpecialValues.IsNullValue(y[i]))
				return 1;
			if(DashboardSpecialValues.IsNullValue(x[i]) && !DashboardSpecialValues.IsNullValue(y[i]))
				return -1;
			if(!DashboardSpecialValues.IsOlapNullValue(x[i]) && DashboardSpecialValues.IsOlapNullValue(y[i]))
				return 1;
			if(DashboardSpecialValues.IsOlapNullValue(x[i]) && !DashboardSpecialValues.IsOlapNullValue(y[i]))
				return -1;
			int result = 0;
			try {
				result = Comparer.Default.Compare(x[i], y[i]);
			}
			catch { }
			if(result == 0)
				return CompareInternal(x, y, i + 1);
			return result;
		}
	}
	public class ServerColorTable {
		Dictionary<ColorTableServerKey, ColorDefinitionBase> rows = new Dictionary<ColorTableServerKey, ColorDefinitionBase>(new ColorTableServerKeyComparer());
		internal Dictionary<ColorTableServerKey, ColorDefinitionBase> Rows { get { return rows; } }
		public ServerColorTable Clone() {
			ServerColorTable clone = new ServerColorTable();
			clone.rows = rows.ToDictionary(row => row.Key, row => row.Value, new ColorTableServerKeyComparer());
			return clone;
		}
		public void Assign(ServerColorTable colorTable) {
			rows = new Dictionary<ColorTableServerKey, ColorDefinitionBase>(colorTable.Rows, new ColorTableServerKeyComparer());
		}
		internal bool ContainsColor(object[] values, MeasureDefinition[] measure) {
			return rows.ContainsKey(new ColorTableServerKey(values, measure));
		}
		public ColorDefinitionBase GetColor(object[] values, MeasureDefinition[] measures) {
			ColorDefinitionBase color;
			if(rows.TryGetValue(CreateKey(values, measures), out color))
				return color;
			return null;
		}
		public void SetColor(object[] values, MeasureDefinition[] measures, ColorDefinitionBase colorDefinition) {
			ColorTableServerKey key = CreateKey(values, measures);
			rows[key] = colorDefinition;
		}
		ColorTableServerKey CreateKey(object[] values, MeasureDefinition[] measures) {
			return new ColorTableServerKey(values, measures);
		}
	}
}
