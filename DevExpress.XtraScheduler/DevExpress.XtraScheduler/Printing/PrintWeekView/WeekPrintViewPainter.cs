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
using DevExpress.XtraScheduler.Printing.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	public class WeekPrintViewPainter : WeekViewPainterFlat {
		PrintColorConverter colorConverter;
		ColorConverterHelper colorConverterHelper;
		public WeekPrintViewPainter(PrintColorConverter colorConverter) {
			this.colorConverter = colorConverter;
			this.colorConverterHelper = new ColorConverterHelper(colorConverter);
		}
		protected internal override SchedulerHeaderPainter CreateHorizontalHeaderPainter() {
			return new SchedulerHeaderFlatPrintPainter();
		}
		protected internal override SchedulerHeaderPainter CreateVerticalHeaderPainter() {
			return new SchedulerHeaderVerticalFlatPrintPainter();
		}
		public override void Draw(GraphicsInfoArgs e, SchedulerViewInfoBase viewInfo) {
			ApplyColorConverter((WeekViewInfo)viewInfo);
			base.Draw(e, viewInfo);
			e.Cache.DrawRectangle(e.Cache.GetPen(Color.Black), viewInfo.Bounds);
		}
		protected override MoreButtonPainter CreateMoreButtonPainter() {
			return new MoreItemsPrintPainter();
		}
		protected internal override NavigationButtonPainter CreateNavigationButtonPainter() {
			return null; 
		}
		void ApplyColorConverter(WeekViewInfo viewInfo) {
			colorConverterHelper.ApplyColorConverterToAppointments(viewInfo.CellContainers.SelectMany(c => c.AppointmentViewInfos));
			colorConverterHelper.ApplyColorConverterToHeaders(viewInfo.GroupSeparators);
			colorConverterHelper.ApplyColorConverterToHeaders(viewInfo.WeekDaysHeaders);
			colorConverterHelper.ApplyColorConverterToHeaders(viewInfo.ResourceHeaders);
			viewInfo.Weeks.ForEach(ApplyColorConverterToWeek);
		}
		void ApplyColorConverterToWeek(SchedulerViewCellContainer week) {
			SingleWeekViewInfo viewInfo = (SingleWeekViewInfo)week;
			viewInfo.Appearance.BorderColor = Color.Black;
			colorConverter.ConvertAppearance(viewInfo.Appearance);
			viewInfo.Cells.ForEach(ApplyColorConverterToCell);
		}
		void ApplyColorConverterToCell(SchedulerViewCellBase cell) {
			SingleWeekCellBase weekCell = (SingleWeekCellBase)cell;
			cell.Appearance.BorderColor = Color.Black;
			colorConverter.ConvertCellAppearance(cell.Appearance);
			colorConverterHelper.ApplyColorConverterToHeader(weekCell.Header);
		}
	}
}
