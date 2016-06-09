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

using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class DayViewAllDayAppointmentLayoutHelper : AllDayAppointmentLayoutHelper {
		public DayViewAllDayAppointmentLayoutHelper(HorizontalAppointmentLayoutCalculator layoutCalculator)
			: base(layoutCalculator) {
		}
		public override void CalculateHeight(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			DayViewInfo viewInfo = (DayViewInfo)ViewInfo;
			VisuallyContinuousCellsInfoCollection cellsInfoCollection = GetCellsInfoCollection(cellsInfos);
			List<AppointmentIntermediateViewInfoCollection> intermediateViewInfos = GetIntermediateViewInfos(viewInfo.PreliminaryLayoutResult.CellsLayerInfos);
			foreach (AppointmentIntermediateViewInfoCollection intermediateViewInfoCollection in intermediateViewInfos) {
				IVisuallyContinuousCellsInfo cellInfo = (IVisuallyContinuousCellsInfo)cellsInfoCollection.Find(ci => ci.Resource == intermediateViewInfoCollection.Resource && ci.Interval.Equals(intermediateViewInfoCollection.Interval));
				LayoutCalculator.PreliminaryContentLayout(intermediateViewInfoCollection, cellInfo);
				LayoutCalculator.CalculateIntermediateViewInfosHeight(intermediateViewInfoCollection);
				LayoutCalculator.CalculateAppointmentRelativePositions(intermediateViewInfoCollection, Int32.MaxValue);
				LayoutCalculator.CalculateViewInfosVerticalBounds(intermediateViewInfoCollection, cellInfo);
			}
		}
		public override AppointmentsLayoutResult CalculateFinalLayout(CellsLayerInfos cellLayers, SchedulerViewCellContainerCollection containers, ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			VisuallyContinuousCellsInfoCollection cellsInfoCollection = GetCellsInfoCollection(cellsInfos);
			List<AppointmentIntermediateViewInfoCollection> intermediateViewInfos = GetIntermediateViewInfos(cellLayers);
			foreach (AppointmentIntermediateViewInfoCollection intermediateViewInfoCollection in intermediateViewInfos) {
				IVisuallyContinuousCellsInfo cellInfo = (IVisuallyContinuousCellsInfo)cellsInfoCollection.Find(ci => ci.Resource == intermediateViewInfoCollection.Resource && ci.Interval.Equals(intermediateViewInfoCollection.Interval));
				LayoutCalculator.PreliminaryContentLayout(intermediateViewInfoCollection, cellInfo);
				PreliminarySnapToCells(intermediateViewInfoCollection, cellInfo);
			}
			return base.CalculateFinalLayout(cellLayers, containers, cellsInfos);
		}
		protected internal override AppointmentIntermediateViewInfoCollection CalculatePreliminaryLayoutCore(AppointmentBaseCollection appointments, IVisuallyContinuousCellsInfo cellsInfo) {
			AppointmentIntermediateViewInfoCollection intermediateResult = (AppointmentIntermediateViewInfoCollection)LayoutCalculator.CreateIntermediateLayoutCalculator().CreateIntermediateViewInfoCollection(cellsInfo.Resource, cellsInfo.Interval);
			LayoutCalculator.CalculateIntermediateViewInfos(intermediateResult, appointments, cellsInfo);
			return intermediateResult;
		}
		VisuallyContinuousCellsInfoCollection GetCellsInfoCollection(ResourceVisuallyContinuousCellsInfosCollection cellsInfos) {
			VisuallyContinuousCellsInfoCollection cellsInfoCollection = new VisuallyContinuousCellsInfoCollection();
			cellsInfoCollection.AddRange(cellsInfos.SelectMany(ci => ci.CellsInfoCollection));
			return cellsInfoCollection;
		}
		List<AppointmentIntermediateViewInfoCollection> GetIntermediateViewInfos(CellsLayerInfos cellLayers) {
			List<AppointmentIntermediateViewInfoCollection> intermediateViewInfos = new List<AppointmentIntermediateViewInfoCollection>();
			intermediateViewInfos.AddRange(cellLayers.Select(cli => cli.AppointmentViewInfos));
			return intermediateViewInfos;
		}
	}
}
