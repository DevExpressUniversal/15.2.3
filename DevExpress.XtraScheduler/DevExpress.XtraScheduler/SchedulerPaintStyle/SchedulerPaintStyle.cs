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
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Drawing {
	public abstract class SchedulerPaintStyle : IDisposable {
		#region Fields
		UserLookAndFeel userLookAndFeel;
		bool isDisposed;
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (userLookAndFeel != null) {
					userLookAndFeel.Dispose();
					userLookAndFeel = null;
				}
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~SchedulerPaintStyle() {
			Dispose(false);
		}
		#endregion
		#region Properties
		public UserLookAndFeel UserLookAndFeel {
			get {
				if (userLookAndFeel == null)
					userLookAndFeel = CreateUserLookAndFeel();
				return userLookAndFeel;
			}
		}
		public abstract string Name { get; }
		public virtual bool IsAvailable { get { return true; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal UserLookAndFeel InnerUserLookAndFeel { get { return userLookAndFeel; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		internal bool IsDisposed { get { return isDisposed; } }
		#endregion
		public virtual BorderPainter CreateBorderPainter(BorderStyles borderStyle) {
			return CreateDefaultBorderPainter(UserLookAndFeel, borderStyle);
		}
		public static BorderPainter CreateDefaultBorderPainter(UserLookAndFeel lookAndFeel, BorderStyles borderStyle) {
			return BorderHelper.GetPainter(borderStyle, lookAndFeel);
		}
		protected internal abstract UserLookAndFeel CreateUserLookAndFeel();
		public abstract ViewPainterBase CreateDayViewPainter();
		public abstract ViewPainterBase CreateWorkWeekViewPainter();
		public abstract ViewPainterBase CreateWeekViewPainter();
		public abstract ViewPainterBase CreateMonthViewPainter();
		public abstract ViewPainterBase CreateTimelineViewPainter();
		public abstract ViewPainterBase CreateGanttViewPainter();
	}
}
