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
	public class InvisibleAppointmentViewInfosAtVisibleIntervalCalculator : InvisibleAppointmentViewInfosCalculatorBase {
		public InvisibleAppointmentViewInfosAtVisibleIntervalCalculator(GanttViewInfo viewInfo)
			: base(viewInfo) {
		}
		protected internal override AppointmentViewInfoCollection CalculateCore(GraphicsCache cache, VisibleResourcesAppointmentSeparator separator) {
			CalculateHorizontalBounds(separator, cache);
			CalculateVerticalBoundsAndVisibility(separator);
			return GetAppointmentViewInfos(separator);
		}
		protected internal virtual void CalculateHorizontalBounds(VisibleResourcesAppointmentSeparator separator, GraphicsCache cache) {
			CellsAppointmentsInfoCollection cellsInfos = separator.MergeAppointmentCellsInfos();
			InvisibleAppointmentsLayoutCalculator layoutCalculator = CreateLayoutCalculator(cache);
			layoutCalculator.CalculateInvisibleAppointments(cellsInfos);
		}
		protected internal virtual void CalculateVerticalBoundsAndVisibility(VisibleResourcesAppointmentSeparator separator) {
			CalculateInvisibleResourceVerticalBounds(separator.TopInvisible, true);
			CalculateInvisibleResourceVerticalBounds(separator.BottomInvisible, false);
			CalculateVisibleResourcesVerticalBounds(separator.Visible);
		}
		protected internal virtual void CalculateInvisibleResourceVerticalBounds(CellsAppointmentsInfo info, bool top) {
			int count = info.AptViewInfos.Count;
			Rectangle cellsBounds = info.CellsInfo.GetTotalBounds();
			for (int i = 0; i < count; i++) {
				AppointmentViewInfo aptViewInfo = info.AptViewInfos[i];
				Rectangle bounds = aptViewInfo.Bounds;
				int y = top ? cellsBounds.Top - 5 : cellsBounds.Bottom;
				aptViewInfo.Bounds = new Rectangle(bounds.Left, y, bounds.Width, 5);
				if (top)
					aptViewInfo.Visibility.InvisibleTopResource = true;
				else
					aptViewInfo.Visibility.InvisibleBottomResource = true;
			}
		}
		protected internal void CalculateVisibleResourcesVerticalBounds(CellsAppointmentsInfoCollection infos) {
			int count = infos.Count;
			for (int i = 0; i < count; i++) {
				CellsAppointmentsInfo cellInfo = infos[i];
				CalculateVisibleResourcesVerticalBounds(cellInfo.AptViewInfos, cellInfo.CellsInfo);
			}
		}
		protected internal virtual InvisibleAppointmentsLayoutCalculator CreateLayoutCalculator(GraphicsCache cache) {
			AppointmentPainter painter = View.ViewInfo.Painter.AppointmentPainter;
			TimelineViewAppointmentContentLayoutCalculator contentCalculator = new TimelineViewAppointmentContentLayoutCalculator(View.ViewInfo, painter);
			InvisibleAppointmentsLayoutCalculator layoutCalculator = new InvisibleAppointmentsLayoutCalculator(View.ViewInfo, contentCalculator, cache, painter);
			return layoutCalculator;
		}
		protected internal void CalculateVisibleResourcesVerticalBounds(AppointmentViewInfoCollection aptViewInfos, VisuallyContinuousCellsInfo cellsInfo) {
			Rectangle cellsBounds = cellsInfo.GetTotalBounds();
			int count = aptViewInfos.Count;
			for (int i = 0; i < count; i++) {
				AppointmentViewInfo aptViewInfo = aptViewInfos[i];
				int height = GetVisibleResourcesAppointmentsHeight();
				aptViewInfo.Bounds = new Rectangle(aptViewInfo.Bounds.Left, cellsBounds.Bottom - height, aptViewInfo.Bounds.Width, height);
				aptViewInfo.Visibility.InvisibleMoreButton = true;
			}
		}
		protected internal virtual int GetVisibleResourcesAppointmentsHeight() {
			if (ViewInfo.MoreButtons.Count == 0)
				return 2;
			return ViewInfo.MoreButtons[0].Bounds.Height;
		}
		protected internal virtual AppointmentViewInfoCollection GetAppointmentViewInfos(VisibleResourcesAppointmentSeparator separator) {
			CellsAppointmentsInfoCollection cellInfos = separator.MergeAppointmentCellsInfos();
			return GetAppointmentViewInfos(cellInfos);
		}
	}
}
