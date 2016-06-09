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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
namespace DevExpress.XtraScheduler.Reporting.Native {
	public abstract class CellsLayoutStrategyBase {
		readonly TimeCellsControlBase control;
		readonly AppointmentsLayoutStrategyBase appointmentsLayoutStrategy;
		readonly CellsLayoutCalculatorBase cellsCalculator;
		protected CellsLayoutStrategyBase(TimeCellsControlBase control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.appointmentsLayoutStrategy = CreateAppointmentsLayoutStrategy();
			this.cellsCalculator = CreateCellsCalculator();
		}
		internal TimeCellsControlBase Control { get { return control; } }
		internal AppointmentsLayoutStrategyBase AppointmentsLayoutStrategy { get { return appointmentsLayoutStrategy; } }
		internal CellsLayoutCalculatorBase CellsCalculator { get { return cellsCalculator; } }
		internal abstract AppointmentsLayoutStrategyBase CreateAppointmentsLayoutStrategy();
		internal abstract CellsLayoutCalculatorBase CreateCellsCalculator();
		public abstract void CalculateLayout(ControlLayoutInfo info);		
	}
	public abstract class AutoHeightCellsLayoutStrategy : CellsLayoutStrategyBase {
		readonly CellLayersHeightCalculator layerHeightCalculator;
		protected AutoHeightCellsLayoutStrategy(TimeCellsControlBase control)
			: base(control) {
			this.layerHeightCalculator = CreateLayersHeightCalculator();
		}
		internal new AutoHeightCellsAppointmentsLayoutStrategy AppointmentsLayoutStrategy { get { return (AutoHeightCellsAppointmentsLayoutStrategy)base.AppointmentsLayoutStrategy; } }
		internal new AutoHeightCellsLayoutCalculator CellsCalculator { get { return (AutoHeightCellsLayoutCalculator)base.CellsCalculator; } }
		internal CellLayersHeightCalculator LayerHeightCalculator { get { return layerHeightCalculator; } }
		public override void CalculateLayout(ControlLayoutInfo info) {
			Control.ActualVerticalAnchors.Clear();
			CellsLayoutPreliminaryInfos preliminaryResult = CalculatePreliminaryLayout(info);			
			CalculateFinalLayout(preliminaryResult, info);
		}
		internal virtual SchedulerViewCellContainerCollection GetPreliminaryCellContainers(CellsLayoutPreliminaryInfos preliminaryResult) {
			SchedulerViewCellContainerCollection result = new SchedulerViewCellContainerCollection();
			int count = preliminaryResult.Count;
			for (int i = 0; i < count; i++) 
				result.AddRange(GetPreliminaryCellContainers(preliminaryResult[i].CellLayers));
			return result;
		}
		internal virtual SchedulerViewCellContainerCollection GetPreliminaryCellContainers(CellsLayerInfos cellLayers) {
			SchedulerViewCellContainerCollection result = new SchedulerViewCellContainerCollection();
			int count = cellLayers.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellContainerCollection containers = ((ReportCellsLayerInfo)cellLayers[i]).CellContainers;
				result.AddRange(containers);
			}
			return result;
		}
		internal virtual CellsLayoutPreliminaryInfos CalculatePreliminaryLayout(ControlLayoutInfo info) {			
			CellsLayoutPreliminaryInfos preliminaryResult =  CellsCalculator.CalculatePreliminaryLayout(info);
			Control.CellContainers.AddRange(GetPreliminaryCellContainers(preliminaryResult));
			AppointmentsLayoutStrategy.CalculatePreliminaryLayout(preliminaryResult);
			return preliminaryResult;
		}
		internal virtual void CalculateFinalLayout(CellsLayoutPreliminaryInfos preliminaryResult, ControlLayoutInfo info) {
			AnchorCollection actualContainersAnchor = CalculateActualContainersAnchor(preliminaryResult, info);
			SchedulerViewCellContainerCollection containers = CellsCalculator.CalculateFinalContainers(preliminaryResult, info.HorizontalAnchors);
			AppointmentsLayoutResult aptLayoutResult = AppointmentsLayoutStrategy.CalculateFinalLayout(preliminaryResult);
			Control.ActualVerticalAnchors.AddRange(actualContainersAnchor);
			Control.CellContainers.Clear();
			Control.CellContainers.AddRange(containers);
			Control.AppointmentsLayoutResult.Merge(aptLayoutResult);
		}
		protected internal virtual CellLayersHeightCalculator CreateLayersHeightCalculator() {
			if (Control.ShouldFitIntoBounds())				
				return new FitCellLayersHeightCalculator();
			else
				return new TileCellLayersHeightCalculator();
		}
		protected internal virtual AnchorCollection CalculateActualContainersAnchor(CellsLayoutPreliminaryInfos preliminaryResult, ControlLayoutInfo info) {
			CellsLayerInfos layers = GetContainerLayers(preliminaryResult);
			LayerHeightCalculator.CalculateLayersHeight(layers, Control.ActualCanShrink, Control.ActualCanGrow, info.ControlPrintBounds.Height);
			return CalculateFinalContainersAnchors(preliminaryResult, info.ControlPrintBounds);
		}
		internal CellsLayerInfos GetContainerLayers(CellsLayoutPreliminaryInfos infos) {
			CellsLayerInfos layers = new CellsLayerInfos();
			int count = infos.Count;
			for (int i = 0; i < count; i++)
				layers.AddRange(infos[i].CellLayers);
			return layers;
		}
		protected internal virtual AnchorCollection CalculateFinalContainersAnchors(CellsLayoutPreliminaryInfos preliminaryResult, Rectangle controlPrintBounds) {
			AnchorCollection result = new AnchorCollection();
			int count = preliminaryResult.Count;
			int offset = controlPrintBounds.Top;
			for (int i = 0; i < count; i++) {
				CellsLayoutPreliminaryInfo layoutInfo = preliminaryResult[i];
				CalculateFinalContainerAnchor(layoutInfo, offset);
				offset = layoutInfo.ContainersAnchor.Bounds.Bottom;
				result.Add(layoutInfo.ContainersAnchor.Anchor);
			}
			return result;
		}
		internal virtual void CalculateFinalContainerAnchor(CellsLayoutPreliminaryInfo preliminaryInfo, int offset) {
			CalculateInnerContainerAnchorsBounds(preliminaryInfo, offset);
			CalculateContainerAhcnorBounds(preliminaryInfo.ContainersAnchor.Anchor);
		}
		internal virtual void CalculateContainerAhcnorBounds(AnchorBase containerAnchor) {
			int count = containerAnchor.InnerAnchors.Count;
			if (count == 0)
				return;
			Rectangle firstAnchor = containerAnchor.InnerAnchors[0].Bounds;
			Rectangle lastAnchor = containerAnchor.InnerAnchors[count - 1].Bounds;
			containerAnchor.Bounds = Rectangle.FromLTRB(firstAnchor.Left, firstAnchor.Top, firstAnchor.Right, lastAnchor.Bottom);
		}
		internal virtual void CalculateInnerContainerAnchorsBounds(CellsLayoutPreliminaryInfo preliminaryInfo, int offset) {
			AnchorCollection innerAnchors = preliminaryInfo.ContainersAnchor.InnerAnchors;
			int count = preliminaryInfo.CellLayers.Count;
			XtraSchedulerDebug.Assert(count == innerAnchors.Count);
			for (int i = 0; i < count; i++) {
				CellsLayerInfoBase layer = preliminaryInfo.CellLayers[i];
				AnchorBase layerAnchor = innerAnchors[i];
				UpdateAnchorBounds(layerAnchor, offset, offset + layer.ActualHeight);
				offset += layer.ActualHeight;
			}
		}
		internal virtual void UpdateAnchorBounds(AnchorBase anchor, int top, int bottom) {
			Rectangle bounds = anchor.Bounds;
			bounds.Y = top;
			bounds.Height = bottom - top;
			anchor.Bounds = bounds;
		}		
	}
	public class TimelineCellsLayoutStrategy : AutoHeightCellsLayoutStrategy {
		public TimelineCellsLayoutStrategy(TimelineCells control)
			: base(control) {
		}
		public new TimelineCells Control { get { return (TimelineCells)base.Control; } }
		internal override CellsLayoutCalculatorBase CreateCellsCalculator() {			
			return new TimelineCellsLayoutCalculator(Control);
		}
		internal override AppointmentsLayoutStrategyBase CreateAppointmentsLayoutStrategy() {
			return new TimelineAppointmentsLayoutStrategy(Control);
		}
	}	
	public class HorizontalWeekLayoutStrategy : AutoHeightCellsLayoutStrategy {
		public HorizontalWeekLayoutStrategy(HorizontalWeek control)
			: base(control) {
		}
		internal new HorizontalWeek Control { get { return (HorizontalWeek)base.Control; } }
		internal new HorizontalWeekLayoutCalculator CellsCalculator { get { return (HorizontalWeekLayoutCalculator)base.CellsCalculator; } }
		internal new HorizontalWeekAppointmentsLayoutStrategy AppointmentsLayoutStrategy { get { return (HorizontalWeekAppointmentsLayoutStrategy)base.AppointmentsLayoutStrategy; } }
		internal override CellsLayoutCalculatorBase CreateCellsCalculator() {
			return new HorizontalWeekLayoutCalculator(Control);
		}
		internal override AppointmentsLayoutStrategyBase CreateAppointmentsLayoutStrategy() {
			return new HorizontalWeekAppointmentsLayoutStrategy(Control);
		}
	}
	public class FullWeekLayoutStrategy : CellsLayoutStrategyBase {
		public FullWeekLayoutStrategy(FullWeek control)
			: base(control) {
		}
		internal new FullWeek Control { get { return (FullWeek)base.Control; } }
		internal new FullWeekLayoutCalculator CellsCalculator { get { return (FullWeekLayoutCalculator)base.CellsCalculator; } }
		internal new FullWeekAppointmentsLayoutStrategy AppointmentsLayoutStrategy { get { return (FullWeekAppointmentsLayoutStrategy)base.AppointmentsLayoutStrategy; } }
		public override void CalculateLayout(ControlLayoutInfo info) {			
			SchedulerViewCellContainerCollection containers =  CellsCalculator.CalculateLayout(info);
			AppointmentsLayoutResult layoutResult = AppointmentsLayoutStrategy.CalculateLayout(containers);
			Control.AppointmentsLayoutResult.Merge(layoutResult);
			Control.CellContainers.AddRange(containers);
		}
		internal override CellsLayoutCalculatorBase CreateCellsCalculator() {
			return new FullWeekLayoutCalculator(Control);
		}
		internal override AppointmentsLayoutStrategyBase CreateAppointmentsLayoutStrategy() {
			return new FullWeekAppointmentsLayoutStrategy(Control);
		}
	}
	public class DayViewCellsLayoutStrategy : CellsLayoutStrategyBase {
		public DayViewCellsLayoutStrategy(DayViewTimeCells control)
			: base(control) {
		}
		internal new DayViewTimeCells Control { get { return (DayViewTimeCells)base.Control; } }
		internal new DayViewTimeCellsLayoutCalculator CellsCalculator { get { return (DayViewTimeCellsLayoutCalculator)base.CellsCalculator; } }
		internal new DayViewDispatchAppointmentsLayoutStrategy AppointmentsLayoutStrategy { get { return (DayViewDispatchAppointmentsLayoutStrategy)base.AppointmentsLayoutStrategy; } }
		public override void CalculateLayout(ControlLayoutInfo info) {			
			Control.ActualVerticalAnchors.Clear();
			int count = info.VerticalAnchors.Count;
			int actualTop = 0;
			for (int i = 0; i < count; i++) {
				AnchorCollection mergedContainerAnchors = MergeContainersAnchors(info.HorizontalAnchors);
				Rectangle actualBounds = CalculateLayoutVerticalAnchor(info.VerticalAnchors.GetAnchorInfo(i), mergedContainerAnchors, actualTop);
				actualTop += actualBounds.Height;
			}
		}
		protected internal virtual AnchorCollection MergeContainersAnchors(AnchorCollection containersAnchors) {
			AnchorCollection result = new AnchorCollection();
			int count = containersAnchors.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(containersAnchors[i].InnerAnchors);
			return result;
		}
		protected internal virtual Rectangle CalculateLayoutVerticalAnchor(AnchorInfo cellsAnchorInfo, AnchorCollection containersAnchors, int top) {
			SchedulerViewCellContainerCollection cellContainers = CellsCalculator.CalculatePreliminaryLayout(cellsAnchorInfo, containersAnchors);
			AppointmentIntermediateViewInfoCollection aptViewInfos = AppointmentsLayoutStrategy.CalculatePreliminaryLayout(cellContainers);
			AnchorBase actualCellsAnchor = CalculateFinalCells(cellsAnchorInfo.Anchor, cellContainers, aptViewInfos, top);
			AppointmentsLayoutResult aptLayoutResult = CalculateFinalAppointments(cellContainers, aptViewInfos);
			MergeLayoutResult(cellContainers, aptLayoutResult, actualCellsAnchor);
			return actualCellsAnchor.Bounds;
		}
		protected internal virtual AppointmentsLayoutResult CalculateFinalAppointments(SchedulerViewCellContainerCollection cellContainers, AppointmentIntermediateViewInfoCollection aptViewInfos) {
			return AppointmentsLayoutStrategy.CalculateFinalLayout(aptViewInfos, cellContainers);
		}
		protected internal virtual AnchorBase CalculateFinalCells(AnchorBase cellsAnchor, SchedulerViewCellContainerCollection cellContainers, AppointmentIntermediateViewInfoCollection aptViewInfos, int top) {			
			if (cellsAnchor.InnerAnchors.Count == 0)
				return cellsAnchor;
			AnchorBase actualCellsAnchor = CellsCalculator.CalculateActualCellsAnchor(cellsAnchor, cellContainers, aptViewInfos, top);
			CellsCalculator.CalculateFinalLayout(cellContainers, actualCellsAnchor);
			return actualCellsAnchor;
		}
		protected internal virtual void MergeLayoutResult(SchedulerViewCellContainerCollection cellContainers, AppointmentsLayoutResult aptLayoutResult, AnchorBase actualCellsAnchor) {
			Control.CellContainers.AddRange(cellContainers);
			Control.ActualVerticalAnchors.Add(actualCellsAnchor);
			Control.AppointmentsLayoutResult.Merge(aptLayoutResult);
		}		
		internal override CellsLayoutCalculatorBase CreateCellsCalculator() {
			return new DayViewTimeCellsLayoutCalculator(Control);
		}
		internal override AppointmentsLayoutStrategyBase CreateAppointmentsLayoutStrategy() {
			return new DayViewDispatchAppointmentsLayoutStrategy(Control);
		}
	}
	public class ReportCellsLayerInfo : CellsLayerInfoBase {
		SchedulerViewCellContainerCollection cellContainers;
		public ReportCellsLayerInfo(int moreButtonHeight, int bottomPadding)
			: base(moreButtonHeight, bottomPadding) {
			this.cellContainers = new SchedulerViewCellContainerCollection();
		}
		public SchedulerViewCellContainerCollection CellContainers { get { return cellContainers; } }
		protected internal override SchedulerViewCellBase GetFirstCell() {
			if (CellContainers.Count == 0)
				return null;
			if (CellContainers[0].Cells.Count == 0)
				return null;
			return CellContainers[0].Cells[0];
		}
	}
}
