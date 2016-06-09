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
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DataAccess;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public abstract class SeriesDashboardItem : DataDashboardItem {
		const string xmlSeriesDimensions = "SeriesDimensions";
		const string xmlSeriesDimension = "SeriesDimension";
		readonly DimensionCollection seriesDimensions = new DimensionCollection();
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("SeriesDashboardItemSeriesDimensions"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor))
		]
		public DimensionCollection SeriesDimensions { get { return seriesDimensions; } }
		protected internal override bool HasDataItems { get { return base.HasDataItems || seriesDimensions.Count > 0; } }
		protected override IEnumerable<DataDashboardItemDescription> ItemDescriptions {
			get {
				foreach (DataDashboardItemDescription valueDescription in ValuesDescriptions)
					yield return valueDescription;
				foreach (DataDashboardItemDescription argumentDescription in ArgumentsDescriptions)
					yield return argumentDescription;
				yield return new DataDashboardItemDescription(
					DashboardLocalizer.GetString(DashboardStringId.DescriptionSeries),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemSeries),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionSeries),
					ItemKind.Dimension, seriesDimensions
				);
			}
		}
		protected internal virtual bool IsDrillDownEnabledOnSeries { get { return IsDrillDownEnabled; } }
		protected abstract IEnumerable<DataDashboardItemDescription> ValuesDescriptions { get; }
		protected abstract IEnumerable<DataDashboardItemDescription> ArgumentsDescriptions { get; }
		protected SeriesDashboardItem() {
			seriesDimensions.CollectionChanged += OnSeriesDimensionCollectionChanged;
		}
		protected virtual void OnSeriesDimensionCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<Dimension> e) {
			OnDataItemsChanged(e.AddedItems, e.RemovedItems);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			seriesDimensions.SaveToXml(element, xmlSeriesDimensions, xmlSeriesDimension);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			seriesDimensions.LoadFromXml(element, xmlSeriesDimensions, xmlSeriesDimension);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			seriesDimensions.OnEndLoading(DataItemRepository, this);
		}
		protected internal string[] GetSeriesDataMembers() {
			if(SeriesDimensions.Count == 0)
				return new string[0];
			if (IsDrillDownEnabledOnSeries) {
				Dimension ddDimension = CurrentDrillDownDimension;
				if(ddDimension != null)
					return new string[] { ddDimension.DisplayName };
				else
					return new string[0];
			}
			int count = SeriesDimensions.Count;
			string[] dataMembers = new string[count];
			for(int i = 0; i < count; i++)
				dataMembers[i] = SeriesDimensions[i].DisplayName;
			return dataMembers;
		}
		protected internal string[] GetSeriesDimensionUniqueNames() {
			if(SeriesDimensions.Count == 0)
				return new string[0];
			if (IsDrillDownEnabledOnSeries) {
				Dimension ddDimension = CurrentDrillDownDimension;
				if(ddDimension != null)
					return new string[] { CurrentDrillDownDimension.ActualId };
				else
					return new string[0];
			}
			int count = SeriesDimensions.Count;
			string[] dataMembers = new string[count];
			for(int i = 0; i < count; i++)
				dataMembers[i] = SeriesDimensions[i].ActualId;
			return dataMembers;
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			List<Dimension> dimensions = new List<Dimension>();
			int lastDimensionIndex = SeriesDimensions.Count - 1;
			if(IsDrillDownEnabledOnSeries)
				lastDimensionIndex = CurrentDrillDownLevel.Value;
			for(int i = 0; i <= lastDimensionIndex; i++)
				dimensions.Add(SeriesDimensions[i]);
			builder.SetColumnHierarchyDimensions(DashboardDataAxisNames.ChartSeriesAxis, dimensions);
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(seriesDimensions.Contains(dimension))
				return DimensionGroupIntervalInfo.Default;
			return base.GetDimensionGroupIntervalInfo(dimension);
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			HiddenDimensions.AddRange(description.TooltipDimensions);
			HiddenMeasures.AddRange(description.TooltipMeasures);
		}
	}
}
