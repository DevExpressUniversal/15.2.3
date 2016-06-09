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
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.DataProcessing {
	public interface IDataSessionFactory {
		IDataSession RequestSession(DataSourceModel dataSource);
	}
	public class DataSessionFactory : IDataSessionFactory {
		static DataSessionFactory defaultDataSessionFactory;
		DataSessionFactory() { }
		public IDataSession RequestSession(DataSourceModel dataSource) {
			return new DataSession(dataSource);
		}
		public static IDataSessionFactory Default {
			get {
				if(defaultDataSessionFactory == null)
					defaultDataSessionFactory = new DataSessionFactory();
				return defaultDataSessionFactory;
			}
		}
	}
   class PivotToSliceHelper<T> {
	   public static IEnumerable<Slice<T>> PivotModelToSlices(PivotAxisModel<T> columnsAxis, PivotAxisModel<T> rowsAxis) {
		   IEnumerable<Slice<T>> grandTotalSlice = new List<Slice<T>>() { new Slice<T> {Dimensions = new List<T>() } };
		   IEnumerable<Slice<T>> columnSlices = GetAxisSlices(columnsAxis).ToList();
		   IEnumerable<Slice<T>> rowSlices = GetAxisSlices(rowsAxis).ToList();
		   List<Slice<T>> crossSlices = new List<Slice<T>>();
		   foreach(var column in columnSlices)
			   foreach(var row in rowSlices)
				   crossSlices.Add(column.Concat(row));
		   return crossSlices.Concat(columnSlices).Concat(rowSlices).Concat(grandTotalSlice);
	   }
	   static IEnumerable<Slice<T>> GetAxisSlices(PivotAxisModel<T> axis) {
		   AxisExpandModel expandModel = axis.ExpandModel;
		   for(int i = 0; i < axis.Dimensions.Count(); i++) {
			   List<RowFiltersModel<T>> rowFilters = new List<RowFiltersModel<T>>();
			   if(i > 0 && expandModel != null && expandModel.Values != null) {
					bool isExpandAction = expandModel.ExpandAction == ExpandAction.Expand;
					int start = isExpandAction ? i : 1;
					for(int j = start; j <= i; j++) {
						var values = expandModel.Values.Where(val => val.Length == j).ToList();
						if(isExpandAction || values.Count > 0) {
							rowFilters.Add(new RowFiltersModel<T> {
								Dimensions = axis.Dimensions.Take(j).ToList(),
								FilterType = isExpandAction ? RowFiltersType.Include : RowFiltersType.Exclude,
								Values = values
							});
						}
					}
			   }
			   yield return new Slice<T> {
				   Dimensions = axis.Dimensions.Take(i + 1).ToArray(),
				   RowFiltersModel = rowFilters.Count > 0 ? rowFilters.ToArray() : null
			   };
		   }
		}
	}
   class Slice<T> {
	   public IEnumerable<T> Dimensions { get; set; }
	   public RowFiltersModel<T>[] RowFiltersModel { get; set; }
	   public Slice<T> Concat(Slice<T> slice) {
		   IEnumerable<RowFiltersModel<T>> rowFilterModels = new RowFiltersModel<T>[] { };
		   if(RowFiltersModel != null)
			   rowFilterModels = rowFilterModels.Concat(RowFiltersModel);
		   if(slice.RowFiltersModel != null)
			   rowFilterModels = rowFilterModels.Concat(slice.RowFiltersModel);
		   return new Slice<T> {
			   Dimensions = Dimensions.Concat(slice.Dimensions).ToArray(),
			   RowFiltersModel = rowFilterModels.Count() > 0 ? rowFilterModels.ToArray() : null
		   };
	   }
   }
	class SliceToPivotHelper<T> {
		enum SliceClass { Axis, CrossAxis, None }
		public static IEnumerable<T> GetAxisComponent(IEnumerable<T> axis, IEnumerable<T> slice) {
			return axis.Where(d => slice.Contains(d));
		}
		static SliceClass ClassifySlice(IEnumerable<T> axis, IEnumerable<T> slice) {
			var axisComponent = GetAxisComponent(axis, slice).ToList();
			int componentLenght = axisComponent.Count();
			int sliceLenght = slice.Count();
			if(componentLenght == sliceLenght)
				return SliceClass.Axis;
			else if(0 < componentLenght && componentLenght < sliceLenght)
				return SliceClass.CrossAxis;
			else 
				return SliceClass.None;
		}
		static bool IsGrandTotalSlice(IEnumerable<T> slice) {
			return slice.Count() == 0;
		}
		public static bool IsCalcGrandTotalOnCrossAxis(IEnumerable<T> axis, IEnumerable<IEnumerable<T>> slices) {
			return slices.Any(slice => ClassifySlice(axis, slice) == SliceClass.Axis) || slices.Any(IsGrandTotalSlice);
		}
		public static IEnumerable<T> GetSummaryLevels(IEnumerable<T> columnsAxis, IEnumerable<T> rowsAxis, IEnumerable<IEnumerable<T>> slices) {
			return slices
				.SelectMany(slice => new T[] { GetAxisComponent(columnsAxis, slice).LastOrDefault(), GetAxisComponent(rowsAxis, slice).LastOrDefault() })
				.NotNull()
				.Distinct();
		}
	}
}
