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
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Printing.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class DayPrintViewPainter : DayViewPainterFlat {
		PrintColorConverter colorConverter;
		ColorConverterHelper colorConverterHelper;
		public DayPrintViewPainter(PrintColorConverter colorConverter) {
			this.colorConverter = colorConverter;
			this.colorConverterHelper = new ColorConverterHelper(colorConverter);
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderFlatPrintPainter();
		}
		public override void Draw(GraphicsInfoArgs e, SchedulerViewInfoBase viewInfo) {
			ApplyColorConverter((DayViewInfo)viewInfo);
			base.Draw(e, viewInfo);
			e.Cache.DrawRectangle(e.Cache.GetPen(Color.Black), viewInfo.Bounds);
		}
		protected internal virtual void ConvertTimeRulerItemAppearance(ViewInfoAppearanceItem item, PrintColorConverter colorConverter) {
			ViewInfoAppearanceItem lineItem = item as ViewInfoHorizontalLineItem;
			if (lineItem != null)
				colorConverter.ConvertTimeRulerLineAppearance(item.Appearance);
			else
				colorConverter.ConvertTimeRulerLineAppearance(item.Appearance);
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalFlatPrintPainter();
		}
		protected override DayViewColumnPainter CreateColumnPainter() {
			return new DayViewColumnFlatPrintPainter();
		}
		protected override void DrawTimeIndicator(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
		}
		protected internal override void DrawCellContainers(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			DayViewInfo dayViewInfo = viewInfo as DayViewInfo;
			base.DrawCellContainers(cache, viewInfo);
			Rectangle leftCellsBounds = RectUtils.GetLeftSideRect(dayViewInfo.CellsPreliminaryLayoutResult.ColumnsBounds);
			cache.FillRectangle(Color.Black, leftCellsBounds);
			Rectangle topRowsBounds = RectUtils.GetTopSideRect(dayViewInfo.CellsPreliminaryLayoutResult.RowsBounds);
			cache.FillRectangle(Color.Black, topRowsBounds);
		}
		protected override TimeRulerPainter CreateTimeRulerPainter() {
			return new TimeRulerFlatPrintPainter();
		}
		void ApplyColorConverter(DayViewInfo viewInfo) {
			ApplyColorConverterToAppointmentStatuses(viewInfo.AllDayAppointmentsStatuses);
			colorConverterHelper.ApplyColorConverterToAppointments(viewInfo.CellContainers.SelectMany(c => c.AppointmentViewInfos));
			colorConverterHelper.ApplyColorConverterToHeaders(viewInfo.GroupSeparators);
			colorConverterHelper.ApplyColorConverterToHeaders(viewInfo.ResourceHeaders);
			colorConverterHelper.ApplyColorConverterToHeaders(viewInfo.DayHeaders);
			ApplyColorConverterToTimeRulers(viewInfo.TimeRulers);
			viewInfo.Columns.ForEach(ApplyColorConverterToColumn);
		}
		void ApplyColorConverterToTimeRulers(TimeRulerViewInfoCollection viewInfos) {
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++)
				ApplyColorConverterToTimeRuler(viewInfos[i]);
		}
		void ApplyColorConverterToAppointmentStatuses(AppointmentStatusViewInfoCollection statuses) {
			int count = statuses.Count;
			for (int i = 0; i < count; i++) {
				AppointmentStatusViewInfo status = statuses[i];
				status.SetBrush(colorConverter.ConvertBrush(status.GetBrush()));
				status.BorderColor = colorConverter.ConvertColor(status.BorderColor);
			}
		}
		void ApplyColorConverterToTimeRuler(TimeRulerViewInfo viewInfo) {
			colorConverter.ConvertTimeRulerAppearance(viewInfo.BackgroundAppearance);
			colorConverter.ConvertTimeRulerHourLineAppearance(viewInfo.HourLineAppearance);
			colorConverter.ConvertTimeRulerAppearance(viewInfo.LargeHourAppearance);
			colorConverter.ConvertTimeRulerHourLineAppearance(viewInfo.LineAppearance);
			int count = viewInfo.Items.Count;
			for (int i = 0; i < count; i++) {
				ViewInfoAppearanceItem item = (ViewInfoAppearanceItem)viewInfo.Items[i];
				ConvertTimeRulerItemAppearance(item, colorConverter);
			}
		}
		void ApplyColorConverterToColumn(SchedulerViewCellContainer cellContainer) {
			DayViewColumn column = (DayViewColumn)cellContainer;
			if (column.AllDayAreaCell != null) {
				colorConverter.ConvertAllDayAreaAppearance(column.AllDayAreaAppearance);
				colorConverter.ConvertAllDayAreaSeparatorAppearance(column.AllDayAreaSeparatorAppearance);
				colorConverter.ConvertAllDayAreaAppearance(column.AllDayAreaCell.Appearance);
			}
			colorConverter.ConvertAppearance(column.Appearance);
			column.Cells.ForEach(ApplyColorConverterToCell);
			column.ExtendedCells.ForEach(ApplyColorConverterToCell);
		}
		void ApplyColorConverterToCell(SchedulerViewCellBase cell) {
			colorConverter.ConvertCellAppearance(cell.Appearance);
		}
	}
}
