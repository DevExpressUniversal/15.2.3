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
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.Card)
	]
	public class CardDashboardItem : KpiDashboardItem<Card>, ISparklineArgumentHolder {
		const string xmlSparklineArgument = "SparklineArgument";
		readonly DataItemBox<Dimension> sparklineArgumentBox;
		readonly SparklineArgumentHolder sparklineArgumentHolder;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CardDashboardItemCards"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public CardCollection Cards { get { return (CardCollection)Elements; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("CardDashboardItemSparklineArgument"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableDimensionPropertyTypeConverter),
		RefreshProperties(RefreshProperties.Repaint),
		DefaultValue(null)
		]
		public Dimension SparklineArgument {
			get { return sparklineArgumentBox.DataItem; }
			set { sparklineArgumentBox.DataItem = value; }
		}
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameCardItem); } }
		protected override IEnumerable<DataDashboardItemDescription> ValuesDescriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionCards),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), ItemKind.Card, Cards);
			}
		}
		protected override IEnumerable<DataDashboardItemDescription> ItemDescriptions {
			get {
				List<DataDashboardItemDescription> itemDescriptions = new List<DataDashboardItemDescription>(base.ItemDescriptions);
				itemDescriptions.Add(new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionSparkline),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionSparkline), ItemKind.SingleDimension, sparklineArgumentHolder));
				return itemDescriptions;
			}
		}
		protected override bool ShowMeasures { get { return false; } }
		string SparklineAxisName { get { return DashboardDataAxisNames.SparklineAxis; } }
		public CardDashboardItem()
			: base(new CardCollection()) {
			sparklineArgumentHolder = new SparklineArgumentHolder(this);
			sparklineArgumentBox = InitializeDimensionBox(sparklineArgumentHolder, xmlSparklineArgument);
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			List<CardViewModel> cardViewModels = new List<CardViewModel>();
			string seriesAxisName = null;
			string sparklineAxisName = null;
			if (SeriesDimensions.Count == 0) {
				foreach (Card card in Cards)
					if (card.ActualValue != null || card.TargetValue != null)
						cardViewModels.Add(new CardViewModel(card));
			}
			else {
				seriesAxisName = SeriesAxisName;
				Card actualCard = ActiveElement;
				if (actualCard != null && (actualCard.ActualValue != null || actualCard.TargetValue != null))
					cardViewModels.Add(new CardViewModel(actualCard));
			}
			if(SparklineArgument != null) 
				sparklineAxisName = SparklineAxisName;
			return new CardDashboardItemViewModel(this, cardViewModels, seriesAxisName, sparklineAxisName);
		}
		protected internal override bool CanSpecifyMeasureNumericFormat(Measure measure) {
			bool canSpecify = false;
			foreach(Card card in Cards) {
				Measure actual = card.ActualValue;
				Measure target = card.TargetValue;
				if(actual == measure || (actual == null && target == measure)) {
					canSpecify = true;
					break;
				}
				if(actual != null && target == measure) {
					canSpecify = false;
					break;
				}
			}
			return canSpecify && base.CanSpecifyMeasureNumericFormat(measure);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			sparklineArgumentBox.SaveToXml(element);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			sparklineArgumentBox.LoadFromXml(element);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			sparklineArgumentBox.OnEndLoading();
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			if(SparklineArgument != null)
				builder.SetColumnHierarchyDimensions(DashboardDataAxisNames.SparklineAxis, new Dimension[] { SparklineArgument });
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			foreach (Dimension dimension in SeriesDimensions)
				description.AddMainDimension(dimension);
			description.AddSparklineArgument(SparklineArgument);
			foreach (Card card in Cards)
				if (card.TargetValue != null || card.DeltaOptions.Changed)
					description.AddMeasure(card.ActualValue, card.TargetValue, card.DeltaOptions);
				else
					description.AddMeasure(card.ActualValue);
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			SparklineArgument = description.SparklineArgument;
			AssignDimension(description.Latitude, SeriesDimensions);
			SeriesDimensions.AddRange(description.MainDimensions);
			AssignDimension(description.Longitude, SeriesDimensions);
			SeriesDimensions.AddRange(description.AdditionalDimensions);
			foreach (MeasureDescription measureBox in description.MeasureDescriptions)
				if (measureBox.MeasureType == MeasureDescriptionType.Delta) {
					Card card = new Card(measureBox.ActualValue, measureBox.TargetValue);
					card.DeltaOptions.Assign(measureBox.DeltaOptions);
					Cards.Add(card);
				}
				else
					Cards.Add(new Card(measureBox.Value));
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(dimension == SparklineArgument)
				return DimensionGroupIntervalInfo.Default;
			return base.GetDimensionGroupIntervalInfo(dimension);
		}
		protected override IEnumerable<Dimension> QuerySparklineDimension { get { return new Dimension[] { SparklineArgument }.NotNull(); } }
	}   
}
