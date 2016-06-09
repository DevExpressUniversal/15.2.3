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

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Design;
using System;
namespace DevExpress.DashboardCommon {
	public abstract class ChartDashboardItemBase : SeriesDashboardItem, IArgumentsDashboardItem {
		const string xmlArguments = "Arguments";
		const string xmlArgument = "Argument";
		readonly DimensionCollection arguments = new DimensionCollection();
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartDashboardItemBaseArguments"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor))
		]
		public DimensionCollection Arguments { get { return arguments; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartDashboardItemBaseInteractivityOptions"),
#endif
		Category(CategoryNames.Interactivity),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ChartInteractivityOptions InteractivityOptions { get { return InteractivityOptionsBase as ChartInteractivityOptions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartDashboardItemBaseColorScheme"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.DashboardItemColorSchemeEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ColorScheme ColorScheme { get { return ColorSchemeInternal; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChartDashboardItemBaseColoringOptions"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(TypeNames.DisplayNameObjectConverter)
		]
		public DashboardItemColoringOptions ColoringOptions { get { return ColoringOptionsBase; } }
		internal bool IsDrillDownEnabledOnArguments { get { return IsDrillDownEnabled && InteractivityOptions.TargetDimensions == TargetDimensions.Arguments; } }
		protected internal override bool IsDrillDownEnabledOnSeries { get { return IsDrillDownEnabled && InteractivityOptions.TargetDimensions == TargetDimensions.Series; } }
		protected override IEnumerable<DataDashboardItemDescription> ArgumentsDescriptions {
			get {
				yield return new DataDashboardItemDescription(
					DashboardLocalizer.GetString(DashboardStringId.DescriptionArguments),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemArgument),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionArguments),
					ItemKind.Dimension, arguments
				);
			}
		}
		protected internal override bool IsDrillDownEnabled {
			get { return InteractivityOptions.ActualIsDrillDownEnabled; }
			set { InteractivityOptions.IsDrillDownEnabled = value; }
		}
		protected internal override DashboardItemMasterFilterMode MasterFilterMode {
			get { return InteractivityOptions.MasterFilterMode; }
			set { InteractivityOptions.MasterFilterMode = value; }
		}
		protected internal override IList<Dimension> SelectionDimensionList { 
			get {
				IList<Dimension> result = new List<Dimension>();
				TargetDimensions targetDimensions = InteractivityOptions.TargetDimensions;
				if(targetDimensions.HasFlag(TargetDimensions.Arguments))
					result.AddRange(arguments);
				if(targetDimensions.HasFlag(TargetDimensions.Series))
					result.AddRange(SeriesDimensions);
				return result;
			} 
		}
		protected internal override bool HasDataItems { get { return base.HasDataItems || arguments.Count > 0; } }
		protected abstract bool IsSupportArgumentNumericGroupIntervals { get; }
		protected abstract bool HasValues { get; }
		protected internal override bool CanColorByMeasures { get { return true; } }
		protected internal override bool CanColorByDimensions { get { return true; } }
		protected internal override Dimension[] ColoringDimensions { get { return ColoringArguments.Union(ColoringSeries).ToArray(); } }
		Dimension[] ColoringArguments { get { return GetColoringDimensions(IsDrillDownEnabledOnArguments, Arguments); } }
		Dimension[] ColoringSeries { get { return GetColoringDimensions(IsDrillDownEnabledOnSeries, SeriesDimensions); } }
		protected ChartDashboardItemBase() {
			arguments.CollectionChanged += OnArgumentsCollectionChanged;
		}
		protected override FilterableDashboardItemInteractivityOptions CreateInteractivityOptions() {
			return new ChartInteractivityOptions(false);
		}
		protected virtual void OnArgumentsCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<Dimension> e) {
			OnDataItemsChanged(e.AddedItems, e.RemovedItems);
		}
		IList<Dimension> IArgumentsDashboardItem.Arguments {
			get {
				if(arguments.Count > 0)
					if(IsDrillDownEnabledOnArguments)
						return new Dimension[] { CurrentDrillDownDimension };
					else
						return arguments;
				else
					return new Dimension[0];
			}
		}
		protected internal override string[] GetSelectionDataMemberDisplayNames() {
			if(!HasValues)
				return new string[0];
			List<string> columnNames = new List<string>();
			if(InteractivityOptions.TargetDimensions.HasFlag(TargetDimensions.Arguments))
				columnNames.AddRange(GetArgumentDataMembers());
			if(InteractivityOptions.TargetDimensions.HasFlag(TargetDimensions.Series))
				columnNames.AddRange(GetSeriesDataMembers());
			return columnNames.ToArray();
		}
		string GetChartArgumentPropertyName(IDashboardDataSource dataSource, string dataMember, int index) {
			IDataSourceSchema dataSchemaProvider = dataSource.GetDataSourceSchema(dataMember);
			return dataSchemaProvider.GetUniqueNamePropertyName(string.Format("{0}_{1}", "ArgumentValue", index));
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			arguments.SaveToXml(element, xmlArguments, xmlArgument);
			ColorScheme.SaveToXml(element);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			arguments.LoadFromXml(element, xmlArguments, xmlArgument);
			ColorScheme.LoadFromXml(element);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			arguments.OnEndLoading(DataItemRepository, this);
		}
		protected internal string[] GetArgumentDataMembers() {
			if(arguments.Count == 0)
				return new string[0];
			if (IsDrillDownEnabledOnArguments) {
				Dimension ddDimension = CurrentDrillDownDimension;
				if (ddDimension != null)
					return new string[] { ddDimension.DisplayName };
				else
					return new string[0];
			}
			int count = arguments.Count;
			string[] dataMembers = new string[count];
			for(int i = 0; i < count; i++)
				dataMembers[i] = arguments[i].DisplayName;
			return dataMembers;
		}
		protected internal string[] GetArgumentDimensionsUniqueNames() {
			if(arguments.Count == 0)
				return new string[0];
			if (IsDrillDownEnabledOnArguments) {
				Dimension ddDimension = CurrentDrillDownDimension;
				if(ddDimension != null)
					return new string[] { ddDimension.ActualId };
				else
					return new string[0];
			}
			int count = arguments.Count;
			string[] dataMembers = new string[count];
			for(int i = 0; i < count; i++)
				dataMembers[i] = arguments[i].ActualId;
			return dataMembers;
		}
		protected override Dictionary<string, int> GetColorDimensionsByAxis(string axisName, IList<DimensionDefinition> actualColorDimensionDefinitions) {
			DimensionCollection dimensions = axisName == DashboardDataAxisNames.ChartSeriesAxis ? SeriesDimensions : Arguments;
			return FilterColorDimensions(dimensions, actualColorDimensionDefinitions);
		}
		internal override string[] GetColorPath() {
			Dictionary<string, int> arguments = GetColorDimensionsByAxis(DashboardDataAxisNames.ChartArgumentAxis);
			Dictionary<string, int> series = GetColorDimensionsByAxis(DashboardDataAxisNames.ChartSeriesAxis);
			List<int> indexes = new List<int>();
			return GetColorPath(arguments, indexes).Union(GetColorPath(series, indexes)).ToArray();
		}
		protected override Dictionary<string, Dictionary<string, int>> GetColorDimensionsByAxes(List<DimensionDefinition> dimensionDefinitions) {
			Dictionary<string, Dictionary<string, int>> dimensionsByAxes = new Dictionary<string, Dictionary<string, int>>(); 
			dimensionsByAxes.Add(DashboardDataAxisNames.ChartArgumentAxis, GetColorDimensionsByAxis(DashboardDataAxisNames.ChartArgumentAxis, dimensionDefinitions));
			dimensionsByAxes.Add(DashboardDataAxisNames.ChartSeriesAxis, GetColorDimensionsByAxis(DashboardDataAxisNames.ChartSeriesAxis, dimensionDefinitions));
			return dimensionsByAxes;
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			List<Dimension> dimensions = new List<Dimension>();
			int lastDimensionIndex = Arguments.Count - 1;
			if(IsDrillDownEnabledOnArguments)
				lastDimensionIndex = CurrentDrillDownLevel.Value;
			for(int i = 0; i <= lastDimensionIndex; i++)
				dimensions.Add(Arguments[i]);
			builder.SetColumnHierarchyDimensions(DashboardDataAxisNames.ChartArgumentAxis, dimensions);
			builder.RowHierarchy = DashboardDataAxisNames.ChartSeriesAxis;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			ChartInteractivityOptions chartInteractivityOptions = description.InteractivityOptions as ChartInteractivityOptions;
			if (chartInteractivityOptions != null)
				chartInteractivityOptions.CopyTo(InteractivityOptions);
			else if (description.InteractivityOptions != null)
				description.InteractivityOptions.CopyTo(InteractivityOptions);
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(arguments.Contains(dimension)) {
				DimensionGroupIntervalInfo groupIntervalInfo = DimensionGroupIntervalInfo.Default;
				groupIntervalInfo.IsSupportNumericGroupIntervals = IsSupportArgumentNumericGroupIntervals;
				return groupIntervalInfo;
			}
			return base.GetDimensionGroupIntervalInfo(dimension);
		}
		protected override string[] GetCurrentAxisName() {
			IList<string> axes = new List<string>();
			if(InteractivityOptions.TargetDimensions.HasFlag(TargetDimensions.Arguments) && arguments.Count > 0)
				axes.Add(DashboardDataAxisNames.ChartArgumentAxis);
			if(InteractivityOptions.TargetDimensions.HasFlag(TargetDimensions.Series) && SeriesDimensions.Count > 0)
				axes.Add(DashboardDataAxisNames.ChartSeriesAxis);
			return axes.ToArray();
		}
		protected internal override ColoringSchemeDefinition GetColoringScheme() {
			ColoringSchemeDefinition schemes = new ColoringSchemeDefinition();
			List<DimensionDefinition> argumentDefinitionList = Helper.GetUniqueDimensionDefinitions(Arguments.Where(dim => dim.ColorByHue)).ToList();
			List<DimensionDefinition> seriesDefinitionList = Helper.GetUniqueDimensionDefinitions(SeriesDimensions.Where(dim => dim.ColorByHue)).ToList();
			Measure[][] measures = GetUniqueColoringMeasures();
			if(IsDrillDownEnabled) {
				if(!ColoringOptions.UseGlobalColors)
					AddScheme(schemes, Helper.GetDimensionDefinitions(ColoringDimensions).ToList(), measures);
				else if(IsDrillDownEnabledOnArguments)
					AddDrillDownSchemes(schemes, argumentDefinitionList, seriesDefinitionList, measures, Arguments, true);
				else if(IsDrillDownEnabledOnSeries)
					AddDrillDownSchemes(schemes, seriesDefinitionList, argumentDefinitionList, measures, SeriesDimensions, false);
			}
			else
				AddScheme(schemes, argumentDefinitionList.Union(seriesDefinitionList).Distinct().ToList(), measures);
			return schemes;
		}
		protected internal override bool LastSingleColoredDefinition(DimensionDefinition definition) {			
			return LastSingleColoredDefinition(definition, Arguments, IsDrillDownEnabledOnArguments) || LastSingleColoredDefinition(definition, SeriesDimensions, IsDrillDownEnabledOnSeries);
		}
		protected override SliceDataQuery GetDataQueryInternal(IActualParametersProvider provider) {
			List<Dimension> arguments = Arguments.ToList();
			List<Dimension> series = SeriesDimensions.ToList();
			List<Dimension> invisibleDimensions = new List<Dimension>();
			if(IsDrillDownEnabled)
				if(InteractivityOptions.TargetDimensions == TargetDimensions.Arguments) {
					arguments = Arguments.Take(Arguments.IndexOf(CurrentDrillDownDimension) + 1).ToList();
					invisibleDimensions = Arguments.Except(arguments).ToList();
				} else if(InteractivityOptions.TargetDimensions == TargetDimensions.Series) {
					series = SeriesDimensions.Take(SeriesDimensions.IndexOf(CurrentDrillDownDimension) + 1).ToList();
					invisibleDimensions = SeriesDimensions.Except(series).ToList();
				} else
					throw new Exception("");
			List<Dimension> filterDimensions = QueryFilterDimensions.Concat(invisibleDimensions).ToList();
			SliceDataQueryBuilder queryBuilder;
			ItemModelBuilder itemBuilder = new ItemModelBuilder(DataSourceModel.DataSourceInfo, GetDataItemUniqueName, provider);
			itemBuilder.MeasureConvertToDecimalForNonNumerical = true;
			if(IsBackCompatibilityDataSlicesRequired) {
				queryBuilder = SliceDataQueryBuilder.CreateWithPivotModel(itemBuilder, arguments, series,
					QueryMeasures, filterDimensions, GetQueryFilterCriteria(provider));
			} else {
				var dimensions = arguments.Concat(series).NotNull().ToList();
				queryBuilder = SliceDataQueryBuilder.CreateEmpty(itemBuilder, filterDimensions, GetQueryFilterCriteria(provider));
				if(QueryMeasures.Count() > 0) {
					queryBuilder.AddSlice(dimensions, QueryMeasures);
					queryBuilder.SetAxes(arguments, series);
				}
			}
			return queryBuilder.FinalQuery();
		}
	}
}
