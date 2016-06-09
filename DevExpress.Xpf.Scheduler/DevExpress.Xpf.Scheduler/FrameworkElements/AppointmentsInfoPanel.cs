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
using System.Windows;
using DevExpress.XtraScheduler;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Core;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using System.Windows.Media;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface IAppoinmentsInfoContainer {
		ISchedulerTimeCellControl SchedulerTimeCellControl { get; set; }
		ISchedulerWeekMoreButtonControl SchedulerWeekMoreButtonControl { get; set; }
		MoreButtonInfoCollection MoreButtonInfos { get; }
		Rect TranslateRectTo(Rect cellBounds, UIElement element);
		Rect TranslateRectFrom(Rect cellBounds, UIElement element);
		WeekViewMoreButtonInfo CreateMoreButtonInfo(int cellIndex, Rect cellBounds, DateTime date);
	}
	public interface ISchedulerWeekMoreButtonControl {
		double GetMoreButtonSize(int cellIndex);
		void ShowMoreButton(int cellIndex);
		void HideAllMoreButton();
		void InvalidateArrange();
	}
	#region AppointmentsInfoPanel
	public class AppointmentsInfoPanel : XPFContentControl, IAppoinmentsInfoContainer {
		MoreButtonInfoCollection moreButtonInfos;
		#region SchedulerTimeCellControl
		public ISchedulerTimeCellControl SchedulerTimeCellControl {
			get { return (ISchedulerTimeCellControl)GetValue(SchedulerTimeCellControlProperty); }
			set { SetValue(SchedulerTimeCellControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerTimeCellControlProperty = DependencyPropertyHelper.RegisterProperty<AppointmentsInfoPanel, ISchedulerTimeCellControl>("SchedulerTimeCellControl", null);
		#endregion
		#region SchedulerWeekMoreButtonControl
		public ISchedulerWeekMoreButtonControl SchedulerWeekMoreButtonControl {
			get { return (ISchedulerWeekMoreButtonControl)GetValue(SchedulerWeekMoreButtonControlProperty); }
			set { SetValue(SchedulerWeekMoreButtonControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerWeekMoreButtonControlProperty = DependencyPropertyHelper.RegisterProperty<AppointmentsInfoPanel, ISchedulerWeekMoreButtonControl>("SchedulerWeekMoreButtonControl", null);
		#endregion
		#region View
		public SchedulerViewBase View {
			get { return (SchedulerViewBase)GetValue(ViewProperty); }
			set { SetValue(ViewProperty, value); }
		}
		public static readonly DependencyProperty ViewProperty = CreateViewProperty();
		static DependencyProperty CreateViewProperty() {
			return DependencyPropertyHelper.RegisterProperty<AppointmentsInfoPanel, SchedulerViewBase>("View", null, OnViewChanged);
		}
		static void OnViewChanged(AppointmentsInfoPanel panel, DependencyPropertyChangedEventArgs<SchedulerViewBase> e) {
			panel.OnViewChanged(e.OldValue, e.NewValue);
		}
		#endregion
		#region SchedulerControl
		public SchedulerControl SchedulerControl {
			get { return (SchedulerControl)GetValue(SchedulerControlProperty); }
			set { SetValue(SchedulerControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerControlProperty = CreateSchedulerControlProperty();
		static DependencyProperty CreateSchedulerControlProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AppointmentsInfoPanel, SchedulerControl>("SchedulerControl", null, (d, e) => d.OnSchedulerControlChanged(e.OldValue, e.NewValue));
		}
		void OnSchedulerControlChanged(SchedulerControl oldValue, SchedulerControl newValue) {
		}
		#endregion
		public MoreButtonInfoCollection MoreButtonInfos { get { return moreButtonInfos; } }
		public AppointmentsInfoPanel() {
			this.moreButtonInfos = new MoreButtonInfoCollection();
		}
		public Rect TranslateRectTo(Rect cellBounds, UIElement element) {
			Point topLeft = this.TranslatePoint(RectHelper.TopLeft(cellBounds), element);
			Point bottomRight = this.TranslatePoint(RectHelper.BottomRight(cellBounds), element);
			return new Rect(topLeft, bottomRight);
		}
		public Rect TranslateRectFrom(Rect cellBounds, UIElement element) {
			Point topLeft = element.TranslatePoint(RectHelper.TopLeft(cellBounds), this);
			Point bottomRight = element.TranslatePoint(RectHelper.BottomRight(cellBounds), this);
			return new Rect(topLeft, bottomRight);
		}
		public WeekViewMoreButtonInfo CreateMoreButtonInfo(int cellIndex, Rect cellBounds, DateTime date) {
			WeekViewMoreButtonInfo info = new WeekViewMoreButtonInfo(cellIndex, cellBounds, date);
			info.View = View;
			return info;
		}
		protected internal virtual void OnViewChanged(SchedulerViewBase oldValue, SchedulerViewBase newValue) {
			if (newValue != null)
				SchedulerControl = newValue.Control;
		}
	}
	#endregion
}
