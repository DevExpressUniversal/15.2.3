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

using System;
using System.Collections.Generic;
namespace DevExpress.XtraCharts.Designer.Native {
	public class ChartTreeElement {
		readonly DesignerChartElementModelBase chartModel;
		readonly ChartTreeCollectionElement parentCollection;
		public DesignerChartElementModelBase ChartModel { get { return chartModel; } }
		public ChartTreeCollectionElement ParentCollection { get { return parentCollection; } }
		public virtual int NodeIndex { get { return -1; } }
		public bool Updated { get; set; }
		public ChartTreeElement(DesignerChartElementModelBase chartElement) {
			this.chartModel = chartElement;
			this.parentCollection = null;
			Updated = true;
		}
		public ChartTreeElement(DesignerChartElementModelBase chartElement, ChartTreeCollectionElement parentCollection) {
			this.chartModel = chartElement;
			this.parentCollection = parentCollection;
			Updated = true;
		}
		public override string ToString() {
			return chartModel.ChartTreeText; ;
		}
		public virtual bool AreModelsEquals(ChartTreeElement treeElement) {
			return treeElement != null && treeElement.ChartModel.ChartElement == this.ChartModel.ChartElement;
		}
	}
	public class ChartTreeDiagramElement : ChartTreeElement {
		public override int NodeIndex { get { return 1; } }
		public ChartTreeDiagramElement(DesignerChartElementModelBase chartElement)
			: base(chartElement) {
		}
	}
	public class ChartTreeCollectionElement : ChartTreeElement {
		public ChartCollectionBaseModel CollectionModel { get { return ChartModel as ChartCollectionBaseModel; } }
		public virtual bool HasGalleryForElementAdding { get { return false; } }
		public ChartTreeCollectionElement(ChartCollectionBaseModel collectionModel)
			: base(collectionModel) {
		}
		public TreeNodeUpdateType CheckUpdate(ChartCollectionBaseModel collectionModel) {
			if (collectionModel == null || collectionModel != null && collectionModel.Count == 0)
				return TreeNodeUpdateType.Remove;
			if (collectionModel.Count == this.CollectionModel.Count)
				return TreeNodeUpdateType.None;
			else
				return TreeNodeUpdateType.Replace;
		}
		public void AddNewElement(object parameter = null) {
			CollectionModel.AddNewElement(parameter);
		}
		public void DeleteElement(ChartTreeElement treeElement) {
			CollectionModel.DeleteElement(treeElement.ChartModel);
		}
		public override bool AreModelsEquals(ChartTreeElement treeElement) {
			return treeElement is ChartTreeCollectionElement && ((ChartTreeCollectionElement)treeElement).CollectionModel.ChartCollection == this.CollectionModel.ChartCollection;
		}
		public virtual ChartStructurePopupUserControl GetUserControlForPopup() {
			return null;
		}
	}
	public class ChartTreeSeriesCollectionElement : ChartTreeCollectionElement {
		SeriesCollectionModel SeriesCollectionModel { get { return CollectionModel as SeriesCollectionModel; } }
		public override int NodeIndex { get { return 0; } }
		public override bool HasGalleryForElementAdding { get { return true; } }
		public ChartTreeSeriesCollectionElement(ChartCollectionBaseModel collectionModel)
			: base(collectionModel) { }
		public override ChartStructurePopupUserControl GetUserControlForPopup() {
			var compatibleViewTypes = new SortedSet<ViewType>();
			foreach (ViewType viewType in Enum.GetValues(typeof(ViewType)))
				if (SeriesCollectionModel.IsCompartibleViewType(viewType))
					compatibleViewTypes.Add(viewType);
			return new AddSeriesUserControl(compatibleViewTypes);
		}
	}
	public class ChartTreeScaleBreakCollectionElement : ChartTreeCollectionElement {
		public override int NodeIndex { get { return 2; } }
		public ChartTreeScaleBreakCollectionElement(ScaleBreakCollectionModel collectionModel)
			: base(collectionModel) {
		}
	}
	public class ChartTreeConstantLineCollectionElement : ChartTreeCollectionElement {
		public override int NodeIndex { get { return 0; } }
		public ChartTreeConstantLineCollectionElement(ConstantLineCollectionModel collectionModel)
			: base(collectionModel) {
		}
	}
	public class ChartTreeStripCollectionElement : ChartTreeCollectionElement {
		public override int NodeIndex { get { return 1; } }
		public ChartTreeStripCollectionElement(StripCollectionModel collectionModel)
			: base(collectionModel) {
		}
	}
	public class ChartTreePaneCollectionElement : ChartTreeCollectionElement {
		public override int NodeIndex { get { return 1; } }
		public ChartTreePaneCollectionElement(XYDiagramPaneCollectionModel collectionModel)
			: base(collectionModel) {
		}
	}
	public class ChartTreeDefaultPaneElement : ChartTreeElement {
		public override int NodeIndex { get { return 0; } }
		public ChartTreeDefaultPaneElement(XYDiagramDefaultPaneModel chartElement)
			: base(chartElement) {
		}
	}
	public class ChartTreeSeriesLabelElement : ChartTreeElement {
		public override int NodeIndex { get { return 0; } }
		public ChartTreeSeriesLabelElement(SeriesLabelBaseModel chartElement)
			: base(chartElement) {
		}
	}
	public class ChartTreeSeriesPointsElement : ChartTreeElement {
		public override int NodeIndex { get { return 0; } }
		public SeriesPointCollectionModel CollectionModel { get { return ChartModel as SeriesPointCollectionModel; } }
		public ChartTreeSeriesPointsElement(SeriesPointCollectionModel chartElement)
			: base(chartElement) {
		}
		bool CompareModels(ChartTreeSeriesPointsElement treeElement) {
			return treeElement != null && treeElement.CollectionModel.ChartCollection == CollectionModel.ChartCollection;
		}
		public override bool AreModelsEquals(ChartTreeElement treeElement) {
			return CompareModels(treeElement as ChartTreeSeriesPointsElement);
		}
	}
	public class ChartTreeIndicatorCollectionElement : ChartTreeCollectionElement {
		public override bool HasGalleryForElementAdding { get { return true; } }
		public ChartTreeIndicatorCollectionElement(ChartCollectionBaseModel collectionModel)
			: base(collectionModel) { }
		public override ChartStructurePopupUserControl GetUserControlForPopup() {
			return new AddIndicatorUserControl();
		}
	}
	public class ChartTreeAnnotationCollectionElement : ChartTreeCollectionElement {
		public override bool HasGalleryForElementAdding { get { return true; } }
		public ChartTreeAnnotationCollectionElement(ChartCollectionBaseModel collectionModel)
			: base(collectionModel) { }
		public override ChartStructurePopupUserControl GetUserControlForPopup() {
			return new AddAnnotationUserControl();
		}
	}
}
