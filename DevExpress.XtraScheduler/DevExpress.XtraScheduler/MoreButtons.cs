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
using System.Collections.Generic;
using System.Text;
using DevExpress.XtraScheduler.Drawing;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.XtraScheduler.Drawing {
	public class MoreButton : SelectableIntervalViewInfo {
		static readonly MoreButton empty = new MoreButton();
		public static new MoreButton Empty { get { return empty; } }
		internal static System.Drawing.Image MoreButtonUp;
		internal static System.Drawing.Image MoreButtonDown;
		static MoreButton() {
			MoreButtonUp = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraScheduler.Images.moreButtonUp.bmp", System.Reflection.Assembly.GetExecutingAssembly());
			XtraSchedulerDebug.Assert(MoreButtonUp != null);
			MoreButtonDown = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.XtraScheduler.Images.moreButtonDown.bmp", System.Reflection.Assembly.GetExecutingAssembly());
			XtraSchedulerDebug.Assert(MoreButtonDown != null);
			XtraSchedulerDebug.Assert(MoreButtonUp.Width == MoreButtonDown.Width);
			XtraSchedulerDebug.Assert(MoreButtonUp.Height == MoreButtonDown.Height);
		}
		DateTime targetViewStart;
		bool goUp;
		bool visible = true;
		public MoreButton() {
			Interval = TimeInterval.Empty;
			targetViewStart = DateTime.MinValue;
		}
		public bool GoUp { get { return goUp; } set { goUp = value; } }
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.MoreButton; } }
		public bool Visible { get { return visible; } set { visible = value; } }
		public DateTime TargetViewStart { get { return targetViewStart; } set { targetViewStart = value; } }
		protected internal override bool AllowHotTrack { get { return true; } }
		public override Rectangle Bounds {
			get {
				return base.Bounds;
			}
			set {
				base.Bounds = value;
			}
		}
		protected internal virtual int CalculateImageIndex() {
			if (HotTrackedInternal)
				return GoUp ? 2 : 3;
			else
				return GoUp ? 0 : 1;
		}
		public bool HotTracked { get { return HotTrackedInternal; } }
		public override SchedulerHitInfo CalculateHitInfo(Point pt, SchedulerHitInfo nextHitInfo) {
			if (Visible)
				return base.CalculateHitInfo(pt, nextHitInfo);
			else return nextHitInfo;
		}
	}
	public class MoreButtonCollection : DXCollection<MoreButton> {
	}
}
namespace DevExpress.XtraScheduler.Native {
	#region  ScrollMoreButton
	public class ScrollMoreButton : MoreButton {
		SchedulerViewCellContainer scrollContainer;
		public ScrollMoreButton(SchedulerViewCellContainer scrollContainer)
			: base() {
			if (scrollContainer == null)
				Exceptions.ThrowArgumentNullException("scrollContainer");
			this.scrollContainer = scrollContainer;
		}
		public SchedulerViewCellContainer ScrollContainer { get { return scrollContainer; } }
		public override SchedulerHitTest HitTestType { get { return SchedulerHitTest.ScrollMoreButton; } }
	}
	#endregion
	#region ScrollMoreButtonCalculator
	public class AllDayScrollMoreButtonsCalculator {
		Size buttonSize;
		SchedulerViewCellContainer scrollContainer;
		public AllDayScrollMoreButtonsCalculator(DayViewInfo viewInfo) {
			if (viewInfo == null)
				Exceptions.ThrowArgumentNullException("viewInfo");
			buttonSize = viewInfo.CalculateMoreButtonMinSize();
			scrollContainer = viewInfo.AllDayAreaScrollContainer;
		}
		public Size ButtonSize { get { return buttonSize; } }
		public SchedulerViewCellContainer ScrollContainer { get { return scrollContainer; } }
		public virtual MoreButtonCollection CalculateMoreButtons(SchedulerViewCellBaseCollection cells) {
			MoreButtonCollection result = new MoreButtonCollection();
			int count = cells.Count;
			for (int i = 0; i < count; i++) {
				SchedulerViewCellBase cell = cells[i];
				if (!IsValidCellBounds(cell.Bounds))
					continue;
				MoreButtonCollection buttons = CalculateMoreButtons(cell);
				result.AddRange(buttons);
			}
			return result;
		}
		bool IsValidCellBounds(Rectangle rectangle) {
			return rectangle.Width > 0 && rectangle.Height > 0 && rectangle.X > 0 && rectangle.Y > 0;
		}
		protected internal virtual MoreButtonCollection CalculateMoreButtons(SchedulerViewCellBase cell) {
			MoreButtonCollection buttons = new MoreButtonCollection();
			buttons.Add(CreateScrollButton(cell.Interval, cell.Resource, cell.ContentBounds, true));
			buttons.Add(CreateScrollButton(cell.Interval, cell.Resource, cell.ContentBounds, false));
			return buttons;
		}
		protected internal virtual ScrollMoreButton CreateScrollButton(TimeInterval interval, Resource resource, Rectangle bounds, bool goUp) {
			ScrollMoreButton button = new ScrollMoreButton(ScrollContainer);
			button.GoUp = goUp;
			button.Interval = interval;
			button.Resource = resource;
			button.Bounds = CalculateButtonBounds(bounds, goUp);
			return button;
		}
		protected internal virtual Rectangle CalculateButtonBounds(Rectangle cellBounds, bool goUp) {
			int left = cellBounds.Right - ButtonSize.Width - 1;
			if (goUp)
				return new Rectangle(left, cellBounds.Top + 2, ButtonSize.Width, ButtonSize.Height);
			else
				return new Rectangle(left, cellBounds.Bottom - ButtonSize.Height - 1, ButtonSize.Width, ButtonSize.Height);
		}
		public virtual void CalculateVisibility(MoreButtonCollection moreButtons, AppointmentViewInfoCollection containerAppointments, SchedulerViewCellContainer scrollContainer) {
			ResetVisibility(moreButtons, scrollContainer);
			int count = containerAppointments.Count;
			for (int i = 0; i < count; i++)
				CalculateVisibleButtons(containerAppointments[i], moreButtons, scrollContainer);
		}
		protected internal virtual void ResetVisibility(MoreButtonCollection moreButtons, SchedulerViewCellContainer scrollContainer) {
			int count = moreButtons.Count;
			for (int i = 0; i < count; i++) {
				ScrollMoreButton scrollButton = moreButtons[i] as ScrollMoreButton;
				if (scrollButton == null)
					continue;
				if (scrollButton.ScrollContainer != scrollContainer)
					continue;
				moreButtons[i].Visible = false;
			}
		}
		protected internal virtual void CalculateVisibleButtons(AppointmentViewInfo aptViewInfo, MoreButtonCollection buttons, SchedulerViewCellContainer scrollContainer) {
			if (IsAppointmentAboveVisibleArea(aptViewInfo, scrollContainer))
				UpdateUpButtonsVisibility(aptViewInfo, buttons, scrollContainer);
			if (IsAppointmentBelowVisibleArea(aptViewInfo, scrollContainer))
				UpdateDownButtonsVisibility(aptViewInfo, buttons, scrollContainer);
		}
		protected internal virtual bool IsAppointmentAboveVisibleArea(AppointmentViewInfo aptViewInfo, SchedulerViewCellContainer scrollContainer) {
			int visibleAreaTop = scrollContainer.Bounds.Top + scrollContainer.CalculateScrollOffset();
			return aptViewInfo.Bounds.Top < visibleAreaTop;
		}
		protected internal virtual bool IsAppointmentBelowVisibleArea(AppointmentViewInfo aptViewInfo, SchedulerViewCellContainer scrollContainer) {
			int visibleAreaBottom = scrollContainer.Bounds.Bottom + scrollContainer.CalculateScrollOffset();
			return aptViewInfo.Bounds.Bottom > visibleAreaBottom;
		}
		protected internal virtual void UpdateDownButtonsVisibility(AppointmentViewInfo aptViewInfo, MoreButtonCollection buttons, SchedulerViewCellContainer scrollContainer) {
			UpdateButtonsVisibilityCore(aptViewInfo, buttons, scrollContainer, false);
		}
		protected internal virtual void UpdateUpButtonsVisibility(AppointmentViewInfo aptViewInfo, MoreButtonCollection buttons, SchedulerViewCellContainer scrollContainer) {
			UpdateButtonsVisibilityCore(aptViewInfo, buttons, scrollContainer, true);
		}
		protected internal virtual void UpdateButtonsVisibilityCore(AppointmentViewInfo aptViewInfo, MoreButtonCollection buttons, SchedulerViewCellContainer scrollContainer, bool goUp) {
			int count = buttons.Count;
			for (int i = 0; i < count; i++) {
				MoreButton button = buttons[i];
				if (IsButtonVisible(aptViewInfo, button, goUp))
					button.Visible = true;
			}
		}
		protected internal virtual bool IsButtonVisible(AppointmentViewInfo aptViewInfo, MoreButton button, bool goUp) {
			if (button.GoUp != goUp)
				return false;
			bool isResourcesEquial = (ResourceBase.InternalMatchIds(aptViewInfo.Resource.Id, button.Resource.Id));
			TimeInterval intersection = TimeInterval.Intersect(button.Interval, aptViewInfo.Interval);
			return (intersection.Duration != TimeSpan.Zero) && isResourcesEquial;
		}
	}
	#endregion
}
