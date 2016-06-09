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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.DashboardCommon.Data;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
namespace DevExpress.DashboardCommon.Viewer {
	public interface IDashboardCardControl {
		void BeginUpdate();
		void EndUpdate();
		void SetSize(Size size);
	}
	public class CardContainerControlModel : IContentProvider {
		const int DefaultDeviceDpi = 96;
		readonly IDashboardCardControl cardControl;
		readonly List<CardModel> items = new List<CardModel>();
		Size itemProportions = new Size(1, 1);
		Size itemMargin = Size.Empty;
		Size size;
		int itemMinWidth = 0;
		bool isSparklineMode = false;
		bool hasSeriesDimensions = false;
		bool hasAtLeastOneSparkline = false;
		KpiViewerDataController dataController;
		event EventHandler<ContentProviderEventArgs> changed;
		decimal IContentProvider.BorderProportion { get { return 0; } }
		public ICollection Items { get { return items; } }
		public Size ItemMargin { get { return itemMargin; } }
		public Size ItemProportions { get { return itemProportions; } }
		public Size Size { get { return size; } }
		public int ItemMinWidth { get { return itemMinWidth; } }
		public bool IsSparklineMode { get { return isSparklineMode; } }
		public bool HasSeriesDimensions { get { return hasSeriesDimensions; } }
		public bool HasAtLeastOneSparkline { get { return hasAtLeastOneSparkline; } }
		public bool IsDataReduced { get { return dataController.IsDataReduced; } }
		public event EventHandler<ContentProviderEventArgs> Changed {
			add { changed = (EventHandler<ContentProviderEventArgs>)Delegate.Combine(changed, value); }
			remove { changed = (EventHandler<ContentProviderEventArgs>)Delegate.Remove(changed, value); }
		}
		public CardContainerControlModel(IDashboardCardControl cardControl) {
			this.cardControl = cardControl;
		}
		void IContentProvider.BeginUpdate() {
			cardControl.BeginUpdate();
		}
		void IContentProvider.EndUpdate() {
			cardControl.EndUpdate();
		}
		public void SetSize(Size size) {
			this.size = size;
			cardControl.SetSize(size);
		}
		public void RaiseChanged(ContentProviderChangeReason reason) {
			if(changed != null)
				changed(this, new ContentProviderEventArgs(reason));
		}
		public void UpdateVisualState(CardStyleProperties styleProperties, float dpiX) {
			itemMinWidth = (int)Math.Round(180 * dpiX / DefaultDeviceDpi);
			itemMargin = styleProperties.Margin;
			itemProportions = styleProperties.Proportions;
		}
		public void Update(CardDashboardItemViewModel viewModel, MultiDimensionalData data, bool drilledDown) {
			Update(viewModel, data, drilledDown, false);
		}
		public void Update(CardDashboardItemViewModel viewModel, MultiDimensionalData data, bool drilledDown, bool isDesignMode) {
			items.Clear();
			dataController = new CardViewerDataController(data, viewModel, drilledDown, isDesignMode);
			isSparklineMode = viewModel.IsSparklineMode;
			hasSeriesDimensions = viewModel.HasSeriesDimensions;
			hasAtLeastOneSparkline = viewModel.HasAtLeastOneSparkline;
			foreach(CardViewModel cardViewModel in viewModel.Cards) {
				foreach(KpiViewerElementData kpiData in dataController.GetElementData(cardViewModel)) {
					items.Add(new CardModel(cardViewModel, CreateCardData(kpiData)));
				}
			}
			RaiseChanged(ContentProviderChangeReason.Data);
		}
		CardData CreateCardData(KpiViewerElementData kpiData) {
			return new CardData(kpiData.Title, kpiData.SelectionData, kpiData.DeltaValues, kpiData.SparklineValues, kpiData.SparklineTooltipValues);
		}
	}
}
