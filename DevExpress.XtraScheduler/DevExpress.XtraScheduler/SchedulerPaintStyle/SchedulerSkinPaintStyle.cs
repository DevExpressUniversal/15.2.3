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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public class SchedulerSkinPaintStyle : SchedulerPaintStyle {
		#region Fields
		ISkinProvider provider;
		#endregion
		public SchedulerSkinPaintStyle(ISkinProvider provider) {
			if (provider == null)
				Exceptions.ThrowArgumentException("provider", provider);
			this.provider = provider;
		}
		#region Properties
		public override string Name { get { return "Skin"; } }
		protected internal ISkinProvider Provider { get { return provider; } }
		#endregion
		#region IDisposable implementation
		protected override void Dispose(bool disposing) {
			if (disposing)
				provider = null;
			base.Dispose(disposing);
		}
		#endregion
		public override BorderPainter CreateBorderPainter(BorderStyles borderStyle) {
			if (borderStyle == BorderStyles.Default)
				return new SchedulerControlBorderSkinPainter(UserLookAndFeel);
			else
				return base.CreateBorderPainter(borderStyle);
		}
		public override ViewPainterBase CreateDayViewPainter() {
			ViewPainterBase painter = new DayViewPainterSkin(UserLookAndFeel);
			painter.Initialize();
			return painter;
		}
		public override ViewPainterBase CreateWorkWeekViewPainter() {
			ViewPainterBase painter = new WorkWeekViewPainterSkin(UserLookAndFeel);
			painter.Initialize();
			return painter;
		}
		public override ViewPainterBase CreateWeekViewPainter() {
			ViewPainterBase painter = new WeekViewPainterSkin(UserLookAndFeel);
			painter.Initialize();
			return painter;
		}
		public override ViewPainterBase CreateMonthViewPainter() {
			ViewPainterBase painter = new MonthViewPainterSkin(UserLookAndFeel);
			painter.Initialize();
			return painter;
		}
		public override ViewPainterBase CreateTimelineViewPainter() {
			ViewPainterBase painter = new TimelineViewPainterSkin(UserLookAndFeel);
			painter.Initialize();
			return painter;
		}
		public override ViewPainterBase CreateGanttViewPainter() {
			ViewPainterBase painter = new GanttViewPainterSkin(UserLookAndFeel);
			painter.Initialize();
			return painter;
		}
		protected internal override UserLookAndFeel CreateUserLookAndFeel() {
			UserLookAndFeel lf = new DevExpress.LookAndFeel.Helpers.SkinEmbeddedLookAndFeel(provider);
			return lf;
		}
	}
}
