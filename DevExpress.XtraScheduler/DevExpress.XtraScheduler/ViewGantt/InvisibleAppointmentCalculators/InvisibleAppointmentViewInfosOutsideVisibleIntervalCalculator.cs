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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class InvisibleAppointmentViewInfosOutsideVisibleIntervalCalculator : InvisibleAppointmentViewInfosCalculatorBase {
		public InvisibleAppointmentViewInfosOutsideVisibleIntervalCalculator(GanttViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected internal override AppointmentViewInfoCollection CalculateCore(GraphicsCache cache, VisibleResourcesAppointmentSeparator separator) {
			CellsAppointmentsInfoCollection merged = separator.MergeAppointmentCellsInfos();
			CreateViewInfosAndCalculateBounds(merged, cache);
			CalculateVisibility(separator);
			return GetAppointmentViewInfos(merged);
		}
		protected internal virtual void CalculateVisibility(VisibleResourcesAppointmentSeparator separator) {
			SetVisibility(separator.TopInvisible, AppointmentViewInfoVisibilitySet.InvisibleTopResource | AppointmentViewInfoVisibilitySet.InvisibleInterval);
			SetVisibility(separator.Visible, AppointmentViewInfoVisibilitySet.InvisibleInterval);
			SetVisibility(separator.BottomInvisible, AppointmentViewInfoVisibilitySet.InvisibleBottomResource | AppointmentViewInfoVisibilitySet.InvisibleInterval);
		}
		protected internal virtual void SetVisibility(CellsAppointmentsInfo info, AppointmentViewInfoVisibilitySet visibility) {
			CellsAppointmentsInfoCollection infos = new CellsAppointmentsInfoCollection();
			infos.Add(info);
			SetVisibility(infos, visibility);
		}
		protected internal virtual void SetVisibility(CellsAppointmentsInfoCollection infos, AppointmentViewInfoVisibilitySet visibility) {
			int count = infos.Count;
			for (int i = 0; i < count; i++)
				SetVisibilityCore(infos[i].AptViewInfos, visibility);
		}
		protected internal virtual void SetVisibilityCore(AppointmentViewInfoCollection aptViewInfos, AppointmentViewInfoVisibilitySet visibility) {
			int count = aptViewInfos.Count;
			for (int i = 0; i < count; i++)
				aptViewInfos[i].Visibility.SetValue(visibility, true);
		}
		protected internal virtual void CreateViewInfosAndCalculateBounds(CellsAppointmentsInfoCollection infos, GraphicsCache cache) {
			int count = infos.Count;
			for (int i = 0; i < count; i++)
				CreateViewInfosAndCalculateBounds(infos[i]);
		}
		protected internal virtual void CreateViewInfosAndCalculateBounds(CellsAppointmentsInfo cellInfo) {
			AppointmentBaseCollection appointments = cellInfo.Appointments;
			int count = appointments.Count;
			for (int i = 0; i < count; i++) {
				AppointmentViewInfo viewInfo = new HorizontalAppointmentViewInfo(appointments[i], TimeZoneHelper);
				viewInfo.Bounds = CalculateBounds(cellInfo.CellsInfo, viewInfo.AppointmentInterval.Start);
				viewInfo.Visibility.InvisibleInterval = true;
				cellInfo.AptViewInfos.Add(viewInfo);
			}
		}
		protected internal virtual Rectangle CalculateBounds(VisuallyContinuousCellsInfo cellInfo, DateTime startAptViewInfo) {
			Rectangle cellBounds = cellInfo.GetTotalBounds();
			int delta = 13;
			if (startAptViewInfo < cellInfo.Interval.Start)
				return new Rectangle(cellBounds.X - delta, cellBounds.Bottom - 7, 2, 2);
			return new Rectangle(cellBounds.Right + delta, cellBounds.Bottom - 7, 2, 2);
		}
	}
}
