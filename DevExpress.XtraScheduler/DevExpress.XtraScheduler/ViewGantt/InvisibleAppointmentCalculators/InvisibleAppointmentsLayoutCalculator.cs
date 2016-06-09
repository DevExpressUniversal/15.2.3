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

using DevExpress.Utils.Drawing;
using DevExpress.XtraScheduler.Internal.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class InvisibleAppointmentsLayoutCalculator : TimelineViewAppointmentFixedHeightLayoutCalculator {
		public InvisibleAppointmentsLayoutCalculator(GanttViewInfo viewInfo, AppointmentContentLayoutCalculator contentCalculator, GraphicsCache cache, AppointmentPainter painter)
			: base(viewInfo, contentCalculator, cache, painter) {
		}
		public void CalculateInvisibleAppointments(CellsAppointmentsInfoCollection infos) {
			int count = infos.Count;
			for (int i = 0; i < count; i++)
				CalculateInvisibleAppointments(infos[i]);
		}
		protected internal virtual void CalculateInvisibleAppointments(CellsAppointmentsInfo info) {
			AppointmentBaseCollection appointments = GetActualInvisibleAppointments(info);
			AppointmentIntermediateViewInfoCollection intermediateResult = (AppointmentIntermediateViewInfoCollection)CreateIntermediateLayoutCalculator().CreateIntermediateViewInfoCollection(info.CellsInfo.Resource, info.CellsInfo.Interval);
			CalculateIntermediateViewInfos(intermediateResult, appointments, info.CellsInfo);
			int count = intermediateResult.Count;
			for (int i = 0; i < count; i++) {
				AppointmentViewInfo aptViewInfo = SnapToCells(intermediateResult[i], info.CellsInfo);
				info.AptViewInfos.Add(aptViewInfo);
			}
		}
		protected override IAppointmentIntermediateViewInfoCoreCollection FilterAppointmentIntermediateViewInfoResult(IAppointmentIntermediateViewInfoCoreCollection intermediateResult, IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateLayoutCalculatorCore intermediateCalculator) {
			return intermediateResult;
		}
		protected internal virtual AppointmentBaseCollection GetActualInvisibleAppointments(CellsAppointmentsInfo info) {
			AppointmentBaseCollection result = new AppointmentBaseCollection();
			AppointmentViewInfoCollection visibleViewInfos = info.CellsInfo.ScrollContainer.AppointmentViewInfos;
			int count = info.Appointments.Count;
			for (int i = 0; i < count; i++) {
				Appointment apt = info.Appointments[i];
				if (!visibleViewInfos.Contains(apt))
					result.Add(apt);
			}
			return result;
		}
		protected internal AppointmentViewInfo SnapToCells(AppointmentIntermediateViewInfo intermediateResult, VisuallyContinuousCellsInfo cellsInfo) {
			CalculateIntermediateViewInfoBounds(intermediateResult, cellsInfo);
			AppointmentViewInfo viewInfo = intermediateResult.ViewInfo;
			Resource resource = View.FilteredResources.GetResourceById(intermediateResult.Appointment.ResourceId);
			viewInfo.Resource = resource;
			return viewInfo;
		}
		protected internal override void AddMoreButtons(IVisuallyContinuousCellsInfo cellsInfo, AppointmentIntermediateViewInfo intermediateResult) {
		}
	}
}
