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
using DevExpress.Utils;
using DevExpress.XtraScheduler.Drawing;
using System.Collections;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class CellsLayerInfoBase {
		int bottomPadding;
		int actualHeight;
		int contentHeight;
		int minHeight;
		AppointmentIntermediateViewInfoCollection aptViewInfos;
		protected CellsLayerInfoBase(int minHeight, int bottomPadding) {
			this.minHeight = minHeight;
			this.bottomPadding = bottomPadding;
			this.aptViewInfos = new AppointmentIntermediateViewInfoCollection();
		}
		public int ContentHeight { get { return Math.Max(minHeight, contentHeight); } set { contentHeight = value; } }
		public int MinHeight { get { return minHeight; } set { minHeight = value; } }
		public int ActualHeight { get { return actualHeight; } set { actualHeight = value; } }
		internal int BottomPadding { get { return bottomPadding; } }
		internal virtual bool IsExpanded { get { return true; } }
		internal AppointmentIntermediateViewInfoCollection AppointmentViewInfos { get { return aptViewInfos; } }
		protected internal abstract SchedulerViewCellBase GetFirstCell();
		internal virtual int CalculateContentHeight(AppointmentIntermediateViewInfoCollection viewInfos) {
			int cellsTop = CalcualteCellsTop();
			int result = CalcualteCellsHeaderHeight();
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++)
				result = Math.Max(result, viewInfos[i].Bounds.Bottom - cellsTop);
			return result + BottomPadding;
		}
		internal virtual int CalcualteCellsHeaderHeight() {
			SchedulerViewCellBase cell = GetFirstCell();
			return cell == null ? 0 : cell.ContentBounds.Top - cell.Bounds.Top;
		}
		internal virtual int CalcualteCellsTop() {
			SchedulerViewCellBase cell = GetFirstCell();
			return cell == null ? 0 : cell.Bounds.Top;
		}
		public virtual void CalculateHeight() {
			this.contentHeight = CalculateContentHeight(AppointmentViewInfos);
		}
	}
	public class CellsLayerInfo : CellsLayerInfoBase {
		VisuallyContinuousCellsInfo cells;
		public CellsLayerInfo(int minHeight, int bottomPadding, VisuallyContinuousCellsInfo cells)
			: base(minHeight, bottomPadding) {
			Guard.ArgumentNotNull(cells, "cells");
			this.cells = cells;
			AppointmentViewInfos.SetResourceAndInterval(cells.Resource, cells.Interval);
		}
		public VisuallyContinuousCellsInfo CellsInfo { get { return cells; } }
		protected internal override SchedulerViewCellBase GetFirstCell() {
			if (CellsInfo.VisibleCells.Count == 0)
				return null;
			return CellsInfo.VisibleCells[0];
		}
	}
	public class CellsLayerInfos : DXCollection<CellsLayerInfoBase> {
		public AppointmentIntermediateViewInfoCollection GetAppointmentViewInfos() {
			AppointmentIntermediateViewInfoCollection result = new AppointmentIntermediateViewInfoCollection();
			int count = this.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(this[i].AppointmentViewInfos);
			return result;
		}
	}
}
namespace DevExpress.XtraScheduler.Native {	
	public class CollapsedCellsLayerInfo : CellsLayerInfo {		
		public CollapsedCellsLayerInfo(int collapsedResourceHeight, int bottomPadding, VisuallyContinuousCellsInfo cells)
			: base(collapsedResourceHeight, bottomPadding, cells) {			
		}
		internal override bool IsExpanded { get { return false; } }
	}
	#region LayerHeightInfo
	public class LayerHeightInfo : IComparable<LayerHeightInfo> {
		readonly CellsLayerInfoBase layer;
		bool cutToMinHeight;
		public LayerHeightInfo(CellsLayerInfoBase layer, bool cutToMinHeight) {
			Guard.ArgumentNotNull(layer, "layer");
			this.layer = layer;
			this.cutToMinHeight = cutToMinHeight;
		}
		public bool CutToMinHeight { get { return cutToMinHeight; } }
		public int ActualHeight { get { return layer.ActualHeight; } set { layer.ActualHeight = value; } }
		public int AvailableSpace {
			get {
				int contentHeight = cutToMinHeight ? layer.MinHeight : layer.ContentHeight;
				return Math.Max(0, ActualHeight - contentHeight);
			}
		}
		#region IComparable<ContaintHeightInfo> Members
		public int CompareTo(LayerHeightInfo info) {
			return Comparer.Default.Compare(AvailableSpace, info.AvailableSpace);
		}
		#endregion
	}
	#endregion
	#region CellLayersHeightCalculator
	public abstract class CellLayersHeightCalculator {
		public abstract void CalculateLayersHeight(CellsLayerInfos layers, bool canShrink, bool canGrow, int totalHeight);
	}
	#endregion
	#region FitCellLayersHeightCalculator
	public class FitCellLayersHeightCalculator : CellLayersHeightCalculator {
		CellsLayerInfos layers = null;
		int collapsedResourcesHeight = 0;
		public CellsLayerInfos Layers { get { return layers; } }
		public FitCellLayersHeightCalculator()
			: this(0) {
		}
		public FitCellLayersHeightCalculator(int collapsedResourcesHeight) {
			this.collapsedResourcesHeight = collapsedResourcesHeight;
		}
		int CollapsedResourcesHeight { get { return collapsedResourcesHeight; } }
		public override void CalculateLayersHeight(CellsLayerInfos layers, bool canShrink, bool canGrow, int totalHeight) {
			 if (layers.Count == 0)
				 return;
			 this.layers = layers;
			 CalculateInitialHeight(layers, totalHeight);
			 if (canShrink || canGrow)
				 CalculateCore(layers);
		}
		protected internal virtual void CalculateInitialHeight(CellsLayerInfos layers, int totalHeight) {
			ResourceHeadersHeightCalculator calculator = new ResourceHeadersHeightCalculator();
			int[] heights = calculator.Calculate(layers, totalHeight, 0, CollapsedResourcesHeight);
			XtraSchedulerDebug.Assert(heights.Length == layers.Count);
			int count = layers.Count;			
			for (int i = 0; i < count; i++) {
				CellsLayerInfoBase container = layers[i];
				container.ActualHeight = heights[i];
			}			
		}
		protected internal virtual void CalculateCore(CellsLayerInfos layers) {
			CellsLayerInfos expandedLayres = FilterLayers(layers, true);
			CellsLayerInfos collapsedLayres = FilterLayers(layers, false);
			ReallocateFreeSpace(expandedLayres, layers, false);
			ReallocateFreeSpace(expandedLayres, collapsedLayres, true);
			ReallocateFreeSpace(collapsedLayres, layers, false);
		}
		protected internal CellsLayerInfos FilterLayers(CellsLayerInfos layers, bool isExpanded) {
			CellsLayerInfos result = new CellsLayerInfos();
			int count = layers.Count;
			for (int i = 0; i < count; i++) {
				CellsLayerInfoBase layer = layers[i];
				if (layer.IsExpanded == isExpanded)
					result.Add(layer);
			}
			return result;
		}
		internal virtual void ReallocateFreeSpace(CellsLayerInfos targetLayers, CellsLayerInfos sourceLayers, bool cutToMinHeight) {
			List<LayerHeightInfo> layersWithFreeSpace = GetFreeSpaceLayers(sourceLayers, cutToMinHeight);
			for (int i = 0; i < targetLayers.Count; i++) {
				CellsLayerInfoBase layer = targetLayers[i];
				if (IsLayerFitted(layer))
					continue;
				AllocateFreeSpace(layer, layersWithFreeSpace);
				if (layersWithFreeSpace.Count == 0)
					return;
			}
		}
		protected internal virtual void AllocateFreeSpace(CellsLayerInfoBase layer, List<LayerHeightInfo> freeSpaceLayers) {
			int necessarySpace = GetNessesarySpace(layer);
			while (freeSpaceLayers.Count > 0 && necessarySpace > 0) {
				int value = GetMinimumAllocationSpace(freeSpaceLayers, necessarySpace);
				int temp = IncreaseLayerHeight(freeSpaceLayers, necessarySpace, value);
				layer.ActualHeight += temp;
				necessarySpace -= temp;
				CleanUpFreeSpaceCollection(freeSpaceLayers);
			}
		}
		internal virtual int IncreaseLayerHeight(List<LayerHeightInfo> freeSpaceLayers, int necessarySpace, int value) {
			int result = 0;
			for (int index = 0; index < freeSpaceLayers.Count && necessarySpace > 0; index++) {
				freeSpaceLayers[index].ActualHeight -= value;
				necessarySpace -= value;
				result += value;
			}
			return result;
		}
		internal virtual void CleanUpFreeSpaceCollection(List<LayerHeightInfo> collection) {
			while (collection.Count > 0 && collection[0].AvailableSpace == 0)
				collection.RemoveAt(0);
			collection.Sort();
		}
		internal virtual int GetMinimumAllocationSpace(List<LayerHeightInfo> freeSpaceLayers, int necessarySpace) {
			int spaceValuePerContainer = Math.Max(1, necessarySpace / freeSpaceLayers.Count);
			int value = Math.Min(spaceValuePerContainer, freeSpaceLayers[0].AvailableSpace);
			return value;
		}
		List<LayerHeightInfo> GetFreeSpaceLayers(CellsLayerInfos layers, bool cutToMinHeight) {
			List<LayerHeightInfo> result = new List<LayerHeightInfo>();
			for (int i = 0; i < layers.Count; i++) {
				CellsLayerInfoBase layer = layers[i];
				LayerHeightInfo info = new LayerHeightInfo(layer, cutToMinHeight);
				if (info.AvailableSpace > 0)
					result.Add(info);
			}
			result.Sort();
			return result;
		}
		protected internal virtual int GetNessesarySpace(CellsLayerInfoBase container) {
			return Math.Max(0, container.ContentHeight - container.ActualHeight);
		}		   
		bool IsLayerFitted(CellsLayerInfoBase container) {
			return container.ActualHeight >= container.ContentHeight;
		}
	}
	#endregion
	#region TileCellLayersHeightCalculator
	public class TileCellLayersHeightCalculator : CellLayersHeightCalculator {
		public override void CalculateLayersHeight(CellsLayerInfos containers, bool canShrink, bool canGrow, int height) {
			for (int i = 0; i < containers.Count; i++) {
				CellsLayerInfoBase container = containers[i];
				container.ActualHeight = CalculateCore(container.ContentHeight, canShrink, canGrow, height);
			}
		}
		protected internal virtual int CalculateCore(int totalContaintHeight, bool canShrink, bool canGrow, int height) {
			int result = totalContaintHeight;
			if (!canShrink)
				result = Math.Max(result, height);
			if (!canGrow)
				result = Math.Min(result, height);
			return result;
		}
	}
	#endregion   
	public class ResourceHeaderWrapper {
		bool isExpanded;
		public ResourceHeaderWrapper(SchedulerHeader header) {
			this.isExpanded = ((IInternalResource)header.Resource).IsExpanded;
		}
		public ResourceHeaderWrapper(CellsLayerInfoBase layerInfo) {
			this.isExpanded = layerInfo.IsExpanded;
		}
		public bool IsExpanded { get { return isExpanded; } }
	}
	public class ResourceHeadersHeightCalculator {
	   public int[] Calculate(SchedulerHeaderCollection headers, int totalHeight, int groupSeparatorWidth, int collapsedResourceHeight) {
		   DXCollection<ResourceHeaderWrapper> wrappers = CreateWrappers(headers);
		   return CalculateCore(wrappers, totalHeight, groupSeparatorWidth, collapsedResourceHeight);
	   }
	   public int[] Calculate(CellsLayerInfos cellLayers, int totalHeight, int groupSeparatorWidth, int collapsedResourceHeight) {
		   DXCollection<ResourceHeaderWrapper> wrappers = CreateWrappers(cellLayers);
		   return CalculateCore(wrappers, totalHeight, groupSeparatorWidth, collapsedResourceHeight);
	   }
	   internal DXCollection<ResourceHeaderWrapper> CreateWrappers(SchedulerHeaderCollection headers) {
		   DXCollection<ResourceHeaderWrapper> result = new DXCollection<ResourceHeaderWrapper>();
		   int count = headers.Count;
		   for (int i = 0; i < count; i++) 
			   result.Add(new ResourceHeaderWrapper(headers[i]));
		   return result;
	   }
	   internal DXCollection<ResourceHeaderWrapper> CreateWrappers(CellsLayerInfos layers) {
		   DXCollection<ResourceHeaderWrapper> result = new DXCollection<ResourceHeaderWrapper>();
		   int count = layers.Count;
		   for (int i = 0; i < count; i++) 
			   result.Add(new ResourceHeaderWrapper(layers[i]));
		   return result;
	   }
	   protected internal virtual int[] CalculateCore(DXCollection<ResourceHeaderWrapper> wrappers, int totalHeight, int groupSeparatorWidth, int collapsedResourceHeight) {
		   if (wrappers.Count == 0)
			   return new int[] { totalHeight };
		   int collapsedResCount = GetCollapsedResourcesCount(wrappers);
		   int expandedResCount = wrappers.Count - collapsedResCount;
		   int collapsedResActualHeight = GetCollapsedResourceHeight(totalHeight, collapsedResCount, groupSeparatorWidth, collapsedResourceHeight);
		   int expandedResWidth = GetExpandedResourceHeight(totalHeight, expandedResCount, collapsedResActualHeight * collapsedResCount);
		   return CalculateHeights(wrappers, totalHeight, collapsedResActualHeight, expandedResWidth);
	   }
	   protected internal virtual int GetCollapsedResourcesCount(DXCollection<ResourceHeaderWrapper> wrappers) {
		   int result = 0;
		   int count = wrappers.Count;
		   for (int i = 0; i < count; i++)
			   if (!wrappers[i].IsExpanded)
				   result++;
		   return result;
	   }
	   protected internal virtual int GetExpandedResourceHeight(int totalHeight, int resCount, int collapsedResTotalHeight) {
		   if (resCount == 0)
			   return 0;
		   return (totalHeight - collapsedResTotalHeight) / resCount;
	   }
	   protected internal virtual int GetCollapsedResourceHeight(int totalHeight, int resCount, int groupSeparatorWidth, int collapsedResourceHeight) {
		   if (resCount == 0)
			   return 0;
		   int collapsedResTotalHeight = Math.Min(totalHeight, (collapsedResourceHeight + groupSeparatorWidth) * resCount);
		   return collapsedResTotalHeight / resCount;
	   }
	   protected internal virtual int[] CalculateHeights(DXCollection<ResourceHeaderWrapper> wrappers, int totalHeight, int collapsedResHeight, int expandedResHeight) {
		   int count = wrappers.Count;
		   int[] result = new int[count];
		   int remainder = totalHeight;
		   for (int i = 0; i < count; i++) {
			   int width = wrappers[i].IsExpanded ? expandedResHeight : collapsedResHeight;
			   result[i] = width;
			   remainder -= width;
		   }
		   AllocateRemainder(result, remainder);
		   return result;
	   }
	   protected internal virtual void AllocateRemainder(int[] heights, int remainder) {
		   if (remainder == 0)
			   return;
		   int count = heights.Length;
		   int additionalHeight = remainder / count;
		   for (int i = 0; i < count; i++)
			   heights[i] += additionalHeight;
		   remainder = remainder % count;
		   for (int i = 0; i < count; i++)
			   if (remainder > 0) {
				   heights[i]++;
				   remainder--;
			   }
	   }
   }
}
