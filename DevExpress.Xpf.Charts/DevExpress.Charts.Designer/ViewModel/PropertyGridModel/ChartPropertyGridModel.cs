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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public abstract class WpfChartElementPropertyGridModelBase : NestedElementPropertyGridModelBase {
		readonly SetChartPropertyCommand setPropertyCommand;
		protected override ICommand SetObjectPropertyCommand { get { return setPropertyCommand; } }
		public WpfChartElementPropertyGridModelBase(WpfChartModel chartModel, string propertyPath) : base(chartModel, propertyPath) {
			setPropertyCommand = new SetChartPropertyCommand(chartModel);
		}
	}
	public class WpfChartPropertyGridModel : WpfChartElementPropertyGridModelBase {
		WpfChartToolTipOptionsPropertyGridModel toolTipOptions;
		WpfChartToolTipControllerPropertyGridModel toolTipController;
		WpfChartCrosshairOptionsPropertyGridModel crosshairOptions;
		ChartTitlePropertyGridModelCollection titles;
		ChartControl Chart { get { return ChartModel.Chart; } }
		[Category(Categories.Animation)]
		public ChartAnimationMode AnimationMode {
			get { return Chart.AnimationMode; }
			set {
				SetProperty("AnimationMode", value);
			}
		}
		[Category(Categories.Behavior)]
		public bool? ToolTipEnabled {
			get { return Chart.ToolTipEnabled; }
			set {
				SetProperty("ToolTipEnabled", value);
			}
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartToolTipOptionsPropertyGridModel ToolTipOptions {
			get { return toolTipOptions; }
			set {
				SetProperty("ToolTipOptions", new ToolTipOptions());
			}
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartToolTipControllerPropertyGridModel ToolTipController {
			get { return toolTipController; }
			set {
				SetProperty("ToolTipController", new ChartToolTipController());
			}
		}
		[Category(Categories.Behavior)]
		public bool? CrosshairEnabled {
			get { return Chart.CrosshairEnabled; }
			set {
				SetProperty("CrosshairEnabled", value);
			}
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartCrosshairOptionsPropertyGridModel CrosshairOptions {
			get { return crosshairOptions; }
			set {
				SetProperty("CrosshairOptions", new CrosshairOptions());
			}
		}
		[Category(Categories.Elements)]
		public ChartTitlePropertyGridModelCollection Titles { get { return titles; } }
		public WpfChartPropertyGridModel(WpfChartModel chartModel) : base(chartModel, string.Empty) {
			titles = new ChartTitlePropertyGridModelCollection(chartModel);
			if (chartModel != null && chartModel.TitleCollectionModel != null)
				chartModel.TitleCollectionModel.CollectionUpdated += TitlesCollectionUpdated;
			UpdateInternal();
		}
		void TitlesCollectionUpdated(ChartCollectionUpdateEventArgs args) {
			foreach (InsertedItem item in args.AddedItems)
				if (item.Index < titles.Count)
					titles[item.Index].UpdateModelElement(item.Item);
				else
					titles.Insert(item.Index, (WpfChartTitlePropertyGridModel)item.Item.PropertyGridModel);
			List<WpfChartTitlePropertyGridModel> removedTitleModels = new List<WpfChartTitlePropertyGridModel>();
			foreach (WpfChartTitleModel title in args.RemovedItems)
				foreach (WpfChartTitlePropertyGridModel titleModel in titles)
					if (titleModel.TitleModel == title)
						removedTitleModels.Add(titleModel);
			foreach (WpfChartTitlePropertyGridModel removedTitle in removedTitleModels)
				titles.Remove(removedTitle);
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (Chart.ToolTipOptions != null) {
				if (toolTipOptions != null && Chart.ToolTipOptions != toolTipOptions.ToolTipOptions || toolTipOptions == null)
					toolTipOptions = new WpfChartToolTipOptionsPropertyGridModel(ChartModel, Chart.ToolTipOptions, "ToolTipOptions.");
			}
			else
				toolTipOptions = null;
			if (Chart.ToolTipController != null) {
				if (toolTipController != null && Chart.ToolTipController != toolTipController.ToolTipController || toolTipController == null)
					toolTipController = new WpfChartToolTipControllerPropertyGridModel(ChartModel, Chart.ToolTipController, "ToolTipController.");
			}
			else
				toolTipController = null;
			if (Chart.CrosshairOptions != null) {
				if (crosshairOptions != null && Chart.CrosshairOptions != crosshairOptions.CrosshairOptions || crosshairOptions == null)
					crosshairOptions = new WpfChartCrosshairOptionsPropertyGridModel(ChartModel, Chart.CrosshairOptions, "CrosshairOptions.");
			}
			else
				crosshairOptions = null;
		}
	}
}
