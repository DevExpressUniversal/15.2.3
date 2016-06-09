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
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.WXPaint;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils.Text;
using System.Security;
using DevExpress.XtraScheduler.Internal.Diagnostics;
using System.Runtime.CompilerServices;
using DevExpress.XtraScheduler.Printing.Native;
using System.Diagnostics;
namespace DevExpress.XtraScheduler.Drawing {
	public class ViewInfoPainterBase {
	}
	public abstract class ViewPainterBase : ObjectPainter {
		bool hideSelection;
		SchedulerHeaderPainter horizontalHeaderPainter;
		SchedulerHeaderPainter verticalHeaderPainter;
		AppointmentPainter appointmentPainter;
		MoreButtonPainter moreButtonPainter;
		NavigationButtonPainter navigationButtonPainter;
		protected ViewPainterBase() {
		}
		public virtual void Initialize() {
			this.horizontalHeaderPainter = CreateHorizontalHeaderPainter();
			this.verticalHeaderPainter = CreateVerticalHeaderPainter();
			this.appointmentPainter = CreateAppointmentPainter();
			this.moreButtonPainter = CreateMoreButtonPainter();
			this.navigationButtonPainter = CreateNavigationButtonPainter();
		}
		public SchedulerHeaderPainter HorizontalHeaderPainter { get { return horizontalHeaderPainter; } }
		public SchedulerHeaderPainter VerticalHeaderPainter { get { return verticalHeaderPainter; } }
		public AppointmentPainter AppointmentPainter { get { return appointmentPainter; } }
		public MoreButtonPainter MoreButtonPainter { get { return moreButtonPainter; } }
		public NavigationButtonPainter NavigationButtonPainter { get { return navigationButtonPainter; } }
		public virtual bool HideSelection {
			get { return hideSelection; }
			set {
				hideSelection = value;
				horizontalHeaderPainter.HideSelection = value;
				verticalHeaderPainter.HideSelection = value;
			}
		}
		protected internal abstract ViewInfoPainterBase SelectCellsLayoutPainter();
		public virtual int ViewAndScrollbarVerticalSeparatorWidth { get { return 0; } }
		public virtual int ViewAndScrollbarHorizontalSeparatorHeight { get { return 0; } }
		protected internal abstract SchedulerHeaderPainter CreateHorizontalHeaderPainter();
		protected internal abstract SchedulerHeaderPainter CreateVerticalHeaderPainter();
		protected internal abstract NavigationButtonPainter CreateNavigationButtonPainter();
		protected internal abstract AppointmentPainter CreateAppointmentPainter();
		protected internal abstract AppearanceDefaultInfo[] GetDefaultAppearances();
		protected virtual MoreButtonPainter CreateMoreButtonPainter() {
			return new MoreButtonPainter();
		}
		public virtual void Draw(GraphicsInfoArgs e, SchedulerViewInfoBase viewInfo) {
			GraphicsCache cache = e.Cache;
			DrawBorders(cache, viewInfo);
			DrawCellContainers(cache, viewInfo);
			DrawHeaders(cache, viewInfo);
			DrawAppointmentsAndTimeIndicator(cache, viewInfo);
			DrawMoreButtons(viewInfo, cache);
			SchedulerControl control = viewInfo.View.Control;
			DrawNavigationButtons(cache, viewInfo.NavigationButtons, control);
		}
		void DrawAppointmentsAndTimeIndicator(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			bool showOverAppointment = ShowTimeIndicatorOverAppointment(viewInfo.View);
			if (showOverAppointment)
				DrawTimeIndicatorOverAppointments(cache, viewInfo);
			else
				DrawTimeIndicatorUnderAppointments(cache, viewInfo);
		}
		bool ShowTimeIndicatorOverAppointment(SchedulerViewBase view) {
			DayView dayView = view as DayView;
			if (dayView != null)
				return dayView.TimeIndicatorDisplayOptions.ShowOverAppointment;
			TimelineView timelineView = view as TimelineView;
			if (timelineView != null)
				return timelineView.TimeIndicatorDisplayOptions.ShowOverAppointment;
			return false;
		}
		void DrawTimeIndicatorUnderAppointments(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			DrawTimeIndicator(cache, viewInfo);
			DrawAppointments(cache, viewInfo);
		}
		void DrawTimeIndicatorOverAppointments(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			DrawAppointments(cache, viewInfo);
			DrawTimeIndicator(cache, viewInfo);
		}
		protected virtual void DrawTimeIndicator(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
		}
		protected virtual internal void DrawAppointments(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			AppointmentPainter.DrawAppointments(cache, viewInfo);
		}
		public virtual void DrawBorders(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
		}
		public virtual void DrawHeaders(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
		}
		protected internal virtual void DrawCellContainers(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
		}
		protected internal virtual void DrawMoreButtons(SchedulerViewInfoBase viewInfo, GraphicsCache cache) {
			DrawMoreButtonsCore(cache, viewInfo.MoreButtons);
		}
		protected virtual void DrawMoreButtonsCore(GraphicsCache cache, MoreButtonCollection moreButtons) {
			int count = moreButtons.Count;
			for (int i = 0; i < count; i++) {
				moreButtonPainter.Draw(cache, moreButtons[i]);
			}
		}
		protected virtual void DrawNavigationButtons(GraphicsCache cache, NavigationButtonCollection navigationButtons, SchedulerControl control) {
			int count = navigationButtons.Count;
			for (int i = 0; i < count; i++) {
				navigationButtonPainter.Draw(cache, navigationButtons[i], control);
			}
		}
		public virtual void DrawViewAndScrollBarSeparator(GraphicsCache cache, Rectangle bounds) {
		}
	}
	public class SchedulerControlBorderSkinPainter : SkinBorderPainter {
		public SchedulerControlBorderSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElementInfo CreateInfo(ObjectInfoArgs e) {
			return new SkinElementInfo(SchedulerSkins.GetSkin(Provider)[SchedulerSkins.SkinBorder]);
		}
	}
	#region AppointmentDefaultAppearancesHelper
	public sealed class AppointmentDefaultAppearancesHelper {
		AppointmentDefaultAppearancesHelper() {
		}
		public static AppearanceDefaultInfo CreateAppointmentAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.Appointment, new AppearanceDefault(SystemColors.WindowText, SystemColors.Window, SystemColors.WindowText, Color.Empty, HorzAlignment.Default, VertAlignment.Default));
		}
	}
	#endregion
	#region NavigationButtonFlatAppearancesHelper
	public sealed class NavigationButtonFlatAppearancesHelper {
		NavigationButtonFlatAppearancesHelper() {
		}
		public static AppearanceDefaultInfo CreateNavigationButtonAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.NavigationButton, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlLightLight, SystemColors.Control, HorzAlignment.Default, VertAlignment.Default));
		}
		public static AppearanceDefaultInfo CreateNavigationButtonDisabledAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.NavigationButtonDisabled, new AppearanceDefault(SystemColors.GrayText, SystemColors.Control, SystemColors.ControlLightLight, SystemColors.Control, HorzAlignment.Default, VertAlignment.Default));
		}
	}
	#endregion
	#region NavigationButtonUltraFlatAppearancesHelper
	public sealed class NavigationButtonUltraFlatAppearancesHelper {
		NavigationButtonUltraFlatAppearancesHelper() {
		}
		public static AppearanceDefaultInfo CreateNavigationButtonAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.NavigationButton, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDarkDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Default));
		}
		public static AppearanceDefaultInfo CreateNavigationButtonDisabledAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.NavigationButtonDisabled, new AppearanceDefault(SystemColors.GrayText, SystemColors.Control, SystemColors.ControlDarkDark, SystemColors.Control, HorzAlignment.Default, VertAlignment.Default));
		}
	}
	#endregion
	#region NavigationButtonOffice2003AppearancesHelper
	public sealed class NavigationButtonOffice2003AppearancesHelper {
		NavigationButtonOffice2003AppearancesHelper() {
		}
		public static AppearanceDefaultInfo CreateNavigationButtonAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.NavigationButton, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1], Office2003Colors.Default[Office2003Color.Border], Office2003Colors.Default[Office2003Color.Button2], LinearGradientMode.Horizontal, HorzAlignment.Default, VertAlignment.Default));
		}
		public static AppearanceDefaultInfo CreateNavigationButtonDisabledAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.NavigationButtonDisabled, new AppearanceDefault(SystemColors.GrayText, ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Button1]), Office2003Colors.Default[Office2003Color.Border], ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Button2]), LinearGradientMode.Horizontal, HorzAlignment.Default, VertAlignment.Default));
		}
	}
	#endregion
	#region NavigationButtonSkinAppearancesHelper
	public sealed class NavigationButtonSkinAppearancesHelper {
		NavigationButtonSkinAppearancesHelper() {
		}
		public static AppearanceDefaultInfo CreateNavigationButtonAppearance(HorzAlignment hAlign, UserLookAndFeel lookAndFeel) {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.NavigationButton, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinNavigationButtonPrev, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center), lookAndFeel));
		}
		public static AppearanceDefaultInfo CreateNavigationButtonDisabledAppearance(HorzAlignment hAlign, UserLookAndFeel lookAndFeel) {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.NavigationButtonDisabled, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinNavigationButtonNext, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center), lookAndFeel));
		}
	}
	#endregion
	#region HeaderAppearanceFlatHelper
	public sealed class HeaderAppearanceFlatHelper {
		HeaderAppearanceFlatHelper() {
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.HeaderCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.HeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.AlternateHeaderCaption, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1Hot], Color.Transparent, Office2003Colors.Default[Office2003Color.Button2Hot], LinearGradientMode.Vertical, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.AlternateHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button2Hot], Color.Transparent, Office2003Colors.Default[Office2003Color.Button2Hot]));
		}
		public static AppearanceDefaultInfo CreateResourceHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.ResourceHeaderCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateResourceHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.ResourceHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control));
		}
		public static AppearanceDefaultInfo CreateSelectionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.Selection, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, Color.Transparent, SystemColors.Highlight, hAlign, VertAlignment.Center));
		}
	}
	#endregion
	#region HeaderAppearanceUltraFlatHelper
	public sealed class HeaderAppearanceUltraFlatHelper {
		HeaderAppearanceUltraFlatHelper() {
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.HeaderCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDarkDark, SystemColors.Control, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.HeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDarkDark, SystemColors.Control));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.AlternateHeaderCaption, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1Hot], SystemColors.ControlDarkDark, Office2003Colors.Default[Office2003Color.Button2Hot], LinearGradientMode.Vertical, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.AlternateHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button2Hot], SystemColors.ControlDarkDark, Office2003Colors.Default[Office2003Color.Button2Hot]));
		}
		public static AppearanceDefaultInfo CreateResourceHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.ResourceHeaderCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDarkDark, SystemColors.Control, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateResourceHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.ResourceHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDarkDark, SystemColors.Control));
		}
		public static AppearanceDefaultInfo CreateSelectionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.Selection, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, SystemColors.ControlDarkDark, SystemColors.Highlight, hAlign, VertAlignment.Center));
		}
	}
	#endregion
	#region HeaderAppearanceOffice2003Helper
	public sealed class HeaderAppearanceOffice2003Helper {
		HeaderAppearanceOffice2003Helper() {
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.HeaderCaption, new AppearanceDefault(SystemColors.ControlText, ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Header]), Color.Transparent, ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Header2]), LinearGradientMode.Vertical, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.HeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]), Color.Transparent, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2])));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.AlternateHeaderCaption, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1Hot], Color.Transparent, Office2003Colors.Default[Office2003Color.Button2Hot], LinearGradientMode.Vertical, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.AlternateHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1Pressed], Color.Transparent, Office2003Colors.Default[Office2003Color.Button1Pressed]));
		}
		public static AppearanceDefaultInfo CreateResourceHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.ResourceHeaderCaption, new AppearanceDefault(SystemColors.ControlText, ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Header]), Color.Transparent, ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Header2]), LinearGradientMode.Vertical, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateResourceHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.ResourceHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]), Color.Transparent, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2])));
		}
		public static AppearanceDefaultInfo CreateSelectionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.Selection, new AppearanceDefault(SystemColors.HighlightText, SystemColors.Highlight, Color.Transparent, SystemColors.Highlight, hAlign, VertAlignment.Center));
		}
	}
	#endregion
	#region HeaderAppearanceSkinHelper
	public sealed class HeaderAppearanceSkinHelper {
		HeaderAppearanceSkinHelper() {
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionAppearance(HorzAlignment hAlign, UserLookAndFeel lookAndFeel) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			Color foreColor = skin[SchedulerSkins.SkinHeader].Color.ForeColor;
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.HeaderCaption, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinHeader, new AppearanceDefault(foreColor, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center), lookAndFeel));
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.HeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionAppearance(HorzAlignment hAlign, UserLookAndFeel lookAndFeel) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			Color foreColor = skin[SchedulerSkins.SkinHeaderAlternate].Color.ForeColor;
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.AlternateHeaderCaption, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinHeaderAlternate, new AppearanceDefault(foreColor, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center), lookAndFeel));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.AlternateHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control));
		}
		public static AppearanceDefaultInfo CreateResourceHeaderCaptionAppearance(HorzAlignment hAlign, UserLookAndFeel lookAndFeel) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			Color foreColor = skin[SchedulerSkins.SkinHeaderResource].Color.ForeColor;
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.ResourceHeaderCaption, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinHeaderResource, new AppearanceDefault(foreColor, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center), lookAndFeel));
		}
		public static AppearanceDefaultInfo CreateResourceHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(SchedulerAppearanceNames.ResourceHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control));
		}
		public static AppearanceDefaultInfo CreateSelectionAppearance(UserLookAndFeel lookAndFeel, HorzAlignment hAlign) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			Color backColor = skin.GetSystemColor(SystemColors.Highlight);
			Color foreColor = skin.GetSystemColor(SystemColors.HighlightText);
			return new AppearanceDefaultInfo(BaseHeaderAppearanceNames.Selection, new AppearanceDefault(foreColor, backColor, Color.Transparent, backColor, hAlign, VertAlignment.Center));
		}
	}
	#endregion
	#region CellHeaderAppearanceFlatHelper
	public sealed class CellHeaderAppearanceFlatHelper {
		CellHeaderAppearanceFlatHelper() {
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.CellHeaderCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.CellHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.TodayCellHeaderCaption, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1Hot], Color.Transparent, Office2003Colors.Default[Office2003Color.Button2Hot], LinearGradientMode.Vertical, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.TodayCellHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button2Hot], Color.Transparent, Office2003Colors.Default[Office2003Color.Button2Hot]));
		}
	}
	#endregion
	#region CellHeaderAppearanceUltraFlatHelper
	public sealed class CellHeaderAppearanceUltraFlatHelper {
		CellHeaderAppearanceUltraFlatHelper() {
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.CellHeaderCaption, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDarkDark, SystemColors.Control, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.CellHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SystemColors.ControlDarkDark, SystemColors.Control));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.TodayCellHeaderCaption, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1Hot], SystemColors.ControlDarkDark, Office2003Colors.Default[Office2003Color.Button2Hot], LinearGradientMode.Vertical, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.TodayCellHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button2Hot], SystemColors.ControlDarkDark, Office2003Colors.Default[Office2003Color.Button2Hot]));
		}
	}
	#endregion
	#region CellHeaderAppearanceOffice2003Helper
	public sealed class CellHeaderAppearanceOffice2003Helper {
		CellHeaderAppearanceOffice2003Helper() {
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.CellHeaderCaption, new AppearanceDefault(SystemColors.ControlText, ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Header]), Color.Transparent, ControlPaint.LightLight(Office2003Colors.Default[Office2003Color.Header2]), LinearGradientMode.Vertical, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.CellHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]), Color.Transparent, ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2])));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionAppearance(HorzAlignment hAlign) {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.TodayCellHeaderCaption, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1Hot], Color.Transparent, Office2003Colors.Default[Office2003Color.Button2Hot], LinearGradientMode.Vertical, hAlign, VertAlignment.Center));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.TodayCellHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, Office2003Colors.Default[Office2003Color.Button1Pressed], Color.Transparent, Office2003Colors.Default[Office2003Color.Button1Pressed]));
		}
	}
	#endregion
	#region CellHeaderAppearanceSkinHelper
	public sealed class CellHeaderAppearanceSkinHelper {
		CellHeaderAppearanceSkinHelper() {
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionAppearance(HorzAlignment hAlign, UserLookAndFeel lookAndFeel) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			Color foreColor = skin[SchedulerSkins.SkinHeader].Color.ForeColor;
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.CellHeaderCaption, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinHeader, new AppearanceDefault(foreColor, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center), lookAndFeel));
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionAppearanceMonth(HorzAlignment hAlign, UserLookAndFeel lookAndFeel) {
			Color defaultForeColor = SystemColors.ControlText;
			AppearanceDefault defaultAppearance = new AppearanceDefault(defaultForeColor, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center);
			AppearanceDefault updatedAppearance = SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinHeader, defaultAppearance, lookAndFeel);
			updatedAppearance.ForeColor = defaultForeColor;
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.CellHeaderCaption, updatedAppearance);
		}
		public static AppearanceDefaultInfo CreateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.CellHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionAppearance(HorzAlignment hAlign, UserLookAndFeel lookAndFeel) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			Color foreColor = skin[SchedulerSkins.SkinHeaderAlternate].Color.ForeColor;
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.TodayCellHeaderCaption, SkinPainterHelper.UpdateAppearance(SchedulerSkins.SkinHeaderAlternate, new AppearanceDefault(foreColor, SystemColors.Control, Color.Transparent, SystemColors.Control, hAlign, VertAlignment.Center), lookAndFeel));
		}
		public static AppearanceDefaultInfo CreateAlternateHeaderCaptionLineAppearance() {
			return new AppearanceDefaultInfo(WeekViewAppearanceNames.TodayCellHeaderCaptionLine, new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, Color.Transparent, SystemColors.Control));
		}
	}
	#endregion
	#region SkinPainterHelper
	public sealed class SkinPainterHelper {
		SkinPainterHelper() {
		}
		public static SkinElementInfo UpdateObjectInfoArgs(UserLookAndFeel lookAndFeel, string skinElementName) {
			return UpdateObjectInfoArgs(lookAndFeel, skinElementName, Rectangle.Empty);
		}
		public static SkinElementInfo UpdateObjectInfoArgs(UserLookAndFeel lookAndFeel, string skinElementName, Rectangle bounds) {
			SkinElement skinEl = GetSkinElement(lookAndFeel, skinElementName);
			return new SkinElementInfo(skinEl, bounds);
		}
		public static SkinElement GetSkinElement(UserLookAndFeel lookAndFeel, string skinElementName) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			return skin[skinElementName];
		}
		public static Color GetSkinForeColor(UserLookAndFeel lookAndFeel, string skinElementName) {
			SkinElement skinEl = GetSkinElement(lookAndFeel, skinElementName);
			if (skinEl != null && skinEl.Color != null)
				return skinEl.Color.GetForeColor();
			return Color.Empty;
		}
		public static SkinPaddingEdges GetSkinPaddings(UserLookAndFeel lookAndFeel, string skinElementName) {
			SkinElement skinEl = GetSkinElement(lookAndFeel, skinElementName);
			return skinEl.ContentMargins;
		}
		public static AppearanceDefault UpdateAppearance(string elementName, AppearanceDefault info, UserLookAndFeel lookAndFeel) {
			SkinElement element = SchedulerSkins.GetSkin(lookAndFeel)[elementName];
			if (element == null)
				return info;
			if (element.Color.GetBackColor() != Color.Empty) {
				info.BackColor = element.Color.GetBackColor();
				info.BackColor2 = element.Color.GetBackColor2();
				info.GradientMode = element.Color.GradientMode;
			}
			if (element.Color.FontBold) {
				info.Font = new Font(info.Font == null ? AppearanceObject.DefaultFont : info.Font, FontStyle.Bold);
			}
			if (element.Color.GetForeColor() != Color.Empty) {
				info.ForeColor = element.Color.GetForeColor();
			}
			return info;
		}
		public static int GetViewAndScrollbarVerticalSeparatorWidth(UserLookAndFeel lookAndFeel) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			if (skin.Properties.GetBoolean(SchedulerSkins.OptHeaderRequireHorzOffset))
				return 1;
			else
				return 0;
		}
		public static int GetViewAndScrollbarHorizontalSeparatorHeight(UserLookAndFeel lookAndFeel) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			if (skin.Properties.GetBoolean(SchedulerSkins.OptHeaderRequireVertOffset))
				return 1;
			else
				return 0;
		}
		public static void DrawViewAndDateTimeSeparator(GraphicsCache cache, UserLookAndFeel lookAndFeel, Rectangle bounds) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			SkinElement skinEl = skin[SchedulerSkins.SkinBorder];
			Color color = skinEl.Color.GetForeColor();
			cache.FillRectangle(cache.GetSolidBrush(color), bounds);
		}
		public static Color GetResourceColorSchemaColor(UserLookAndFeel lookAndFeel, string colorPropertyName) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			return skin.Colors.GetColor(colorPropertyName);
		}
		public static int GetSkinIntegerProperty(UserLookAndFeel lookAndFeel, string name) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			return skin.Properties.GetInteger(name);
		}
	}
	#endregion
	public abstract class BorderObjectPainter : ViewInfoPainterBase {
		public virtual void DrawBorders(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			if (viewInfo.TopBorderBounds.Height > 0)
				DrawTopBorder(cache, viewInfo);
			if (viewInfo.BottomBorderBounds.Height > 0)
				DrawBottomBorder(cache, viewInfo);
			if (viewInfo.LeftBorderBounds.Width > 0)
				DrawLeftBorder(cache, viewInfo);
			if (viewInfo.RightBorderBounds.Width > 0)
				DrawRightBorder(cache, viewInfo);
		}
		protected abstract void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo);
		protected abstract void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo);
		protected abstract void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo);
		protected abstract void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo);
		public virtual int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public virtual int GetNoLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public virtual int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public virtual int GetNoRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public virtual int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public virtual int GetNoTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public virtual int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public virtual int GetNoBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		protected internal virtual Size CalculateTotalBorderSize(BorderObjectViewInfo border) {
			int leftWidth = border.HasLeftBorder ? GetLeftBorderWidth(border) : GetNoLeftBorderWidth(border);
			int rightWidth = border.HasRightBorder ? GetRightBorderWidth(border) : GetNoRightBorderWidth(border);
			int topHeight = border.HasTopBorder ? GetTopBorderWidth(border) : GetNoTopBorderWidth(border);
			int bottomHeight = border.HasBottomBorder ? GetBottomBorderWidth(border) : GetNoBottomBorderWidth(border);
			return new Size(leftWidth + rightWidth, topHeight + bottomHeight);
		}
	}
	public class SchedulerViewCellPainter : BorderObjectPainter {
		bool hideSelection;
		public bool HideSelection { get { return hideSelection; } set { hideSelection = value; } }
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			SchedulerViewCellBase cell = (SchedulerViewCellBase)viewInfo;
			Brush brush = cell.Appearance.GetBorderBrush(cache);
			cache.FillRectangle(brush, cell.LeftBorderBounds);
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			SchedulerViewCellBase cell = (SchedulerViewCellBase)viewInfo;
			Brush brush = cell.Appearance.GetBorderBrush(cache);
			cache.FillRectangle(brush, cell.TopBorderBounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			SchedulerViewCellBase cell = (SchedulerViewCellBase)viewInfo;
			Brush brush = cell.Appearance.GetBorderBrush(cache);
			cache.FillRectangle(brush, cell.RightBorderBounds);
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			SchedulerViewCellBase cell = (SchedulerViewCellBase)viewInfo;
			Brush brush = cell.Appearance.GetBorderBrush(cache);
			cache.FillRectangle(brush, cell.BottomBorderBounds);
		}
	}
	#region SchedulerHeaderPainter
	public abstract class SchedulerHeaderPainter : BorderObjectPainter {
		bool hideSelection;
		public virtual int HeaderSeparatorWidth { get { return 2; } }
		public virtual int HeaderSeparatorPadding { get { return 1; } }
		public virtual int CaptionLineWidth { get { return 1; } }
		public virtual int ImageTextGapSize { get { return 2; } }
		public virtual int HorizontalOverlap { get { return 0; } }
		public virtual int VerticalOverlap { get { return 0; } }
		public virtual int CornerHorizontalOverlap { get { return 0; } }
		public virtual bool ShouldCacheSkinElementInfo { get { return false; } }
		public bool HideSelection { get { return hideSelection; } set { hideSelection = value; } }
		public virtual int GetContentLeftPadding(SchedulerHeader header) { return 2; }
		public virtual int GetContentRightPadding(SchedulerHeader header) { return 2; }
		public virtual int GetContentTopPadding(SchedulerHeader header) { return 2; }
		public virtual int GetContentBottomPadding(SchedulerHeader header) { return 2; }
		public virtual int GetHorizontalGroupSeparatorHeight(GraphicsCache cache) {
			return 10;
		}
		public virtual int GetVerticalGroupSeparatorWidth(GraphicsCache cache) {
			return 10;
		}
		public virtual SkinElementInfo PrepareCachedSkinElementInfo(SchedulerHeader header, Color color) {
			return null;
		}
		public virtual void DrawHeaders(GraphicsCache cache, SchedulerHeaderCollection headers, ISupportCustomDraw control) {
			int count = headers.Count;
			for (int i = 0; i < count; i++)
				Draw(cache, headers[i], control);
		}
		public virtual void Draw(GraphicsCache cache, SchedulerHeader header, ISupportCustomDraw customDrawProvider) {
			DefaultDrawDelegate defaultDraw = delegate() { DrawCore(cache, header); };
			if (header.RaiseCustomDrawEvent(cache, customDrawProvider, defaultDraw))
				return;
			defaultDraw();
		}
		protected virtual void DrawCore(GraphicsCache cache, SchedulerHeader header) {
			DrawBackground(cache, header);
			using (IntersectClipper clipper = new IntersectClipper(cache, header.ContentBounds)) {
				DrawImage(cache, header);
				DrawCaption(cache, header);
			}
			if (header.Bounds.Height > 0)
				DrawUnderline(cache, header);
			if (header.HasSeparator && header.SeparatorBounds.Width > 0)
				DrawSeparator(cache, header);
			DrawBorders(cache, header);
		}
		protected internal virtual void DrawBackground(GraphicsCache cache, SchedulerHeader header) {
			AppearanceObject appearance = header.CalcActualCaptionAppearance(HideSelection);
			appearance.DrawBackground(cache, header.Bounds);
		}
		protected virtual void DrawImage(GraphicsCache cache, SchedulerHeader header) {
			Image image = header.Image;
			if (image == null)
				return;
			InterpolationMode oldInterpolationMode = cache.Graphics.InterpolationMode;
			try {
				cache.Graphics.InterpolationMode = header.ImageInterpolationMode;
				cache.Paint.DrawImage(cache.Graphics, image, header.ImageBounds);
			} finally {
				cache.Graphics.InterpolationMode = oldInterpolationMode;
			}
		}
		protected internal virtual AppearanceObject GetActualCaptionAppearance(SchedulerHeader header) {
			return header.CalcActualCaptionAppearance(HideSelection);
		}
		protected virtual void DrawCaption(GraphicsCache cache, SchedulerHeader header) {
			AppearanceObject captionAppearance = header.CalcActualCaptionAppearance(HideSelection);
			if (header.RotateCaption)
				DrawVerticalCaption(cache, header, captionAppearance);
			else
				DrawHorizontalCaption(cache, header, captionAppearance);
		}
		protected internal virtual void DrawVerticalCaption(GraphicsCache cache, SchedulerHeader header, AppearanceObject captionAppearance) {
			Brush brush = captionAppearance.GetForeBrush(cache);
			cache.DrawVString(header.Caption, captionAppearance.Font, brush, header.TextBounds, captionAppearance.GetStringFormat(TextOptions.DefaultOptionsCenteredWithEllipsis), 270);
		}
		protected internal virtual void DrawHorizontalCaption(GraphicsCache cache, SchedulerHeader header, AppearanceObject captionAppearance) {
			StringPainter.Default.DrawString(cache, captionAppearance, header.Caption, header.TextBounds, TextOptions.DefaultOptionsCenteredWithEllipsis);
		}
		protected internal virtual void DrawUnderline(GraphicsCache cache, SchedulerHeader header) {
			header.UnderlineAppearance.FillRectangle(cache, header.UnderlineBounds);
		}
		protected virtual void DrawSeparator(GraphicsCache cache, SchedulerHeader header) {
		}
		protected internal virtual Color GetHeaderForeColor(string skinElementName, Color defaultColor) {
			return defaultColor;
		}
		protected internal virtual bool GetHeaderFontBold(string skinElementName, bool defaultFontBold) {
			return defaultFontBold;
		}
		protected internal virtual Color GetCellHeaderForeColor(string skinElementName, Color defaultColor) {
			return defaultColor;
		}
	}
	#endregion
	#region SchedulerHeaderFlatPainter
	public class SchedulerHeaderFlatPainter : SchedulerHeaderPainter {
		public override int HeaderSeparatorWidth { get { return 2; } }
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 2;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 2;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 2;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		protected virtual BBrushes CreateBorderBrushes(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			SchedulerHeader header = (SchedulerHeader)viewInfo;
			return new BBrushes(cache, header.Appearance.HeaderCaption.BackColor);
		}
		protected override void DrawSeparator(GraphicsCache cache, SchedulerHeader header) {
			header.Appearance.HeaderCaption.FillRectangle(cache, header.FullSeparatorBounds);
			BBrushes brushes = CreateBorderBrushes(cache, header);
			Rectangle bounds = header.SeparatorBounds;
			bounds.Width = 1;
			cache.FillRectangle(brushes.Dark, bounds);
			bounds.X++;
			cache.FillRectangle(brushes.LightLight, bounds);
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetTopSideRect(viewInfo.TopBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
			bounds.Y++;
			cache.FillRectangle(brushes.LightLight, bounds);
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetBottomSideRect(viewInfo.BottomBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetLeftSideRect(viewInfo.LeftBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
			bounds.X++;
			bounds.Inflate(0, -1);
			cache.FillRectangle(brushes.LightLight, bounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetLeftSideRect(viewInfo.RightBorderBounds, 1);
			bounds.Inflate(0, -1);
			cache.FillRectangle(brushes.LightLight, bounds);
			bounds.X++;
			bounds.Inflate(0, 1);
			cache.FillRectangle(brushes.Dark, bounds);
		}
	}
	#endregion
	#region SchedulerHeaderVerticalFlatPainter
	public class SchedulerHeaderVerticalFlatPainter : SchedulerHeaderFlatPainter {
		public override void DrawBorders(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			if (viewInfo.TopBorderBounds.Width > 0)
				DrawTopBorder(cache, viewInfo);
			if (viewInfo.BottomBorderBounds.Width > 0)
				DrawBottomBorder(cache, viewInfo);
			if (viewInfo.LeftBorderBounds.Height > 0)
				DrawLeftBorder(cache, viewInfo);
			if (viewInfo.RightBorderBounds.Height > 0)
				DrawRightBorder(cache, viewInfo);
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetLeftSideRect(viewInfo.TopBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
			bounds.X++;
			cache.FillRectangle(brushes.LightLight, bounds);
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetRightSideRect(viewInfo.BottomBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetTopSideRect(viewInfo.LeftBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
			bounds.Y++;
			bounds.Inflate(-1, 0);
			cache.FillRectangle(brushes.LightLight, bounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetTopSideRect(viewInfo.RightBorderBounds, 1);
			bounds.Inflate(-1, 0);
			cache.FillRectangle(brushes.LightLight, bounds);
			bounds.Y++;
			bounds.Inflate(1, 0);
			cache.FillRectangle(brushes.Dark, bounds);
		}
	}
	#endregion
	#region SchedulerHeaderUltraFlatPainter
	public class SchedulerHeaderUltraFlatPainter : SchedulerHeaderPainter {
		public override int HeaderSeparatorWidth { get { return 1; } }
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		protected override void DrawSeparator(GraphicsCache cache, SchedulerHeader header) {
			cache.FillRectangle(header.Appearance.HeaderCaption.GetBorderBrush(cache), header.SeparatorBounds);
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			SchedulerHeader header = (SchedulerHeader)viewInfo;
			cache.FillRectangle(header.Appearance.HeaderCaption.GetBorderBrush(cache), header.TopBorderBounds);
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			SchedulerHeader header = (SchedulerHeader)viewInfo;
			cache.FillRectangle(header.Appearance.HeaderCaption.GetBorderBrush(cache), header.BottomBorderBounds);
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			SchedulerHeader header = (SchedulerHeader)viewInfo;
			cache.FillRectangle(header.Appearance.HeaderCaption.GetBorderBrush(cache), header.LeftBorderBounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			SchedulerHeader header = (SchedulerHeader)viewInfo;
			cache.FillRectangle(header.Appearance.HeaderCaption.GetBorderBrush(cache), header.RightBorderBounds);
		}
	}
	#endregion
	#region SchedulerHeaderVerticalUltraFlatPainter
	public class SchedulerHeaderVerticalUltraFlatPainter : SchedulerHeaderUltraFlatPainter {
	}
	#endregion
	#region SchedulerHeaderWindowsXPPainter
	public class SchedulerHeaderWindowsXPPainter : SchedulerHeaderPainter {
		WXPPainterArgs drawArgs = new WXPPainterArgs("header", XPConstants.HP_HEADERITEM, 0);
		WXPPainterArgs DrawArgs { get { return drawArgs; } }
		public override int CaptionLineWidth { get { return 0; } }
		public override int GetContentBottomPadding(SchedulerHeader header) { return 4; }
		protected internal override void DrawBackground(GraphicsCache cache, SchedulerHeader header) {
			DrawArgs.Bounds = header.Bounds;
			DrawArgs.ThemeHandle = IntPtr.Zero;
			if (header.Selected && !HideSelection)
				DrawArgs.StateId = XPConstants.HIS_PRESSED;
			else
				DrawArgs.StateId = header.Alternate ? XPConstants.HIS_HOT : XPConstants.HIS_NORMAL;
			Rectangle saveBounds = DrawArgs.Bounds;
			try {
				WXPPainter.Default.DrawTheme(DrawArgs, cache.Graphics, null);
			} finally {
				DrawArgs.Bounds = saveBounds;
			}
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
	}
	#endregion
	#region SchedulerHeaderVerticalWindowsXPPainter
	public class SchedulerHeaderVerticalWindowsXPPainter : SchedulerHeaderWindowsXPPainter {
	}
	#endregion
	#region SchedulerHeaderOffice2003Painter
	public class SchedulerHeaderOffice2003Painter : SchedulerHeaderPainter {
		public override int HeaderSeparatorWidth { get { return 2; } }
		protected override void DrawSeparator(GraphicsCache cache, SchedulerHeader header) {
			header.Appearance.HeaderCaption.FillRectangle(cache, header.FullSeparatorBounds);
			Brush light = cache.GetSolidBrush(Color.White);
			Brush dark = cache.GetSolidBrush(ControlPaint.Light(Office2003Colors.Default[Office2003Color.Header2]));
			Rectangle bounds = header.SeparatorBounds;
			bounds.Width = 1;
			cache.FillRectangle(dark, bounds);
			bounds.X++;
			cache.FillRectangle(light, bounds);
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
	}
	#endregion
	#region SchedulerHeaderVerticalOffice2003Painter
	public class SchedulerHeaderVerticalOffice2003Painter : SchedulerHeaderOffice2003Painter {
	}
	#endregion
	#region SchedulerHeaderSkinPainter
	public class SchedulerHeaderSkinPainter : SchedulerHeaderPainter {
		UserLookAndFeel lookAndFeel;
		public SchedulerHeaderSkinPainter(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		public override int HeaderSeparatorWidth { get { return 0; } }
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override int HorizontalOverlap {
			get {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				if (skin.Properties.GetBoolean(SchedulerSkins.OptHeaderRequireHorzOffset))
					return 1;
				else
					return 0;
			}
		}
		public override int VerticalOverlap {
			get {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				if (skin.Properties.GetBoolean(SchedulerSkins.OptHeaderRequireVertOffset))
					return 1;
				else
					return 0;
			}
		}
		public override bool ShouldCacheSkinElementInfo {
			get {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				return skin.Properties.GetBoolean(SchedulerSkins.OptPaintResourceHeaderWithResourceColor);
			}
		}
		public override int GetHorizontalGroupSeparatorHeight(GraphicsCache cache) {
			SkinElementInfo el = GetSkinElementInfo(SchedulerSkins.SkinGroupSeparatorHorizontal, Rectangle.Empty);
			return ObjectPainter.CalcObjectMinBounds(cache.Graphics, SkinElementPainter.Default, el).Height;
		}
		public override int GetVerticalGroupSeparatorWidth(GraphicsCache cache) {
			SkinElementInfo el = GetSkinElementInfo(SchedulerSkins.SkinGroupSeparatorVertical, Rectangle.Empty);
			return ObjectPainter.CalcObjectMinBounds(cache.Graphics, SkinElementPainter.Default, el).Width;
		}
		public override int GetContentLeftPadding(SchedulerHeader header) { return GetHeaderMargins(header).Left; }
		public override int GetContentRightPadding(SchedulerHeader header) { return GetHeaderMargins(header).Right; }
		public override int GetContentTopPadding(SchedulerHeader header) { return GetHeaderMargins(header).Top; }
		public override int GetContentBottomPadding(SchedulerHeader header) { return GetHeaderMargins(header).Bottom; }
		public override SkinElementInfo PrepareCachedSkinElementInfo(SchedulerHeader header, Color color) {
			string skinElementName = header.CalcActualSkinElementName(true);
			SkinElementInfo el = GetSkinElementInfo(skinElementName, header.Bounds);
			return SkinElementColorer.PaintElementWithColor(el, color);
		}
		SkinPaddingEdges GetHeaderMargins(SchedulerHeader header) {
			return SkinPainterHelper.GetSkinPaddings(lookAndFeel, header.SkinElementName); 
		}
		protected internal override void DrawBackground(GraphicsCache cache, SchedulerHeader header) {
			SkinElementInfo el;
			if (!header.Selected && header.CachedSkinElementInfo != null)
				el = header.CachedSkinElementInfo;
			else {
				string skinElementName = header.CalcActualSkinElementName(HideSelection);
				el = GetSkinElementInfo(skinElementName, header.Bounds);
			}
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
		}
		protected internal override AppearanceObject GetActualCaptionAppearance(SchedulerHeader header) {
			return header.CalcActualCaptionAppearance(true);
		}
		protected SkinElementInfo GetSkinElementInfo(string skinElementName, Rectangle bounds) {
			return SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, skinElementName, bounds);
		}
		protected internal override void DrawUnderline(GraphicsCache cache, SchedulerHeader header) {
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected internal override Color GetHeaderForeColor(string skinElementName, Color defaultColor) {
			SkinElement skinEl = SkinPainterHelper.GetSkinElement(lookAndFeel, skinElementName);
			return skinEl != null ? skinEl.Color.GetForeColor() : defaultColor;
		}
		protected internal override bool GetHeaderFontBold(string skinElementName, bool defaultFontBold) {
			SkinElement skinEl = SkinPainterHelper.GetSkinElement(lookAndFeel, skinElementName);
			return skinEl != null ? skinEl.Color.FontBold : defaultFontBold;
		}
		protected internal override Color GetCellHeaderForeColor(string skinElementName, Color defaultColor) {
			Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
			return skin.Properties.GetColor(skinElementName);
		}
	}
	#endregion
	#region SchedulerHeaderVerticalSkinPainter
	public class SchedulerHeaderVerticalSkinPainter : SchedulerHeaderSkinPainter {
		public SchedulerHeaderVerticalSkinPainter(UserLookAndFeel lookAndFeel)
			: base(lookAndFeel) {
		}
	}
	#endregion
	public class ExtendedTimeCellPainter : TimeCellPainter {
		protected override void DrawCellCore(GraphicsCache cache, TimeCell cell) {
			base.DrawCellCore(cache, cell);
			ExtendedCell extendedCell = cell as ExtendedCell;
			Rectangle bounds = extendedCell.Bounds;
			bounds.Inflate(-4, 1);
			cache.DrawString(extendedCell.Text, extendedCell.Appearance.Font, cache.GetSolidBrush(Color.Black), bounds, cell.Appearance.GetStringFormat());
		}
	}
	public class TimeCellPainter : SchedulerViewCellPainter {
		public virtual int AllDayAreaTopBorderSize { get { return 1; } }
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public void DrawCells(GraphicsCache cache, SchedulerViewCellBaseCollection cells, ISupportCustomDraw customDrawProvider) {
			int count = cells.Count;
			for (int i = 0; i < count; i++)
				DrawCell(cache, (TimeCell)cells[i], customDrawProvider);
		}
		public virtual void DrawCell(GraphicsCache cache, TimeCell cell, ISupportCustomDraw customDrawProvider) {
			DefaultDrawDelegate defaultDraw = delegate() { DrawCellCore(cache, cell); };
			if (cell.RaiseCustomDrawEvent(cache, customDrawProvider, defaultDraw))
				return;
			defaultDraw();
		}
		protected virtual void DrawCellCore(GraphicsCache cache, TimeCell cell) {
			DrawBackground(cache, cell);
			DrawBorders(cache, cell);
		}
		protected virtual void DrawBackground(GraphicsCache cache, TimeCell cell) {
			bool isSelected = cell.Selected && !HideSelection;
			AppearanceObject appearance = isSelected ? cell.SelectionAppearance : cell.Appearance;
			appearance.FillRectangle(cache, cell.Bounds);
		}
	}
	public class AppointmentStatusPainter : BorderObjectPainter {
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			Brush borderBrush = cache.GetSolidBrush(((AppointmentStatusViewInfo)viewInfo).BorderColor);
			cache.FillRectangle(borderBrush, viewInfo.LeftBorderBounds);
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			Brush borderBrush = cache.GetSolidBrush(((AppointmentStatusViewInfo)viewInfo).BorderColor);
			cache.FillRectangle(borderBrush, viewInfo.TopBorderBounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			Brush borderBrush = cache.GetSolidBrush(((AppointmentStatusViewInfo)viewInfo).BorderColor);
			cache.FillRectangle(borderBrush, viewInfo.RightBorderBounds);
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			Brush borderBrush = cache.GetSolidBrush(((AppointmentStatusViewInfo)viewInfo).BorderColor);
			cache.FillRectangle(borderBrush, viewInfo.BottomBorderBounds);
		}
		protected internal virtual void DrawRectangleStatus(GraphicsCache cache, AppointmentStatusViewInfo statusViewInfo) {
			FillStatusBrush(cache, statusViewInfo);
			statusViewInfo.CalcBorderBounds(this);
			DrawBorders(cache, statusViewInfo);
		}
		protected virtual void FillStatusBrush(GraphicsCache cache, AppointmentStatusViewInfo statusViewInfo) {
			Brush statusBrush = statusViewInfo.GetBrush(cache);
			lock (statusBrush)
				cache.FillRectangle(statusBrush, statusViewInfo.Bounds);
		}
		protected internal virtual void DrawTriangleStatus(GraphicsCache cache, AppointmentStatusViewInfo statusViewInfo, ViewInfoItemAlignment alignment) {
			Rectangle bounds = statusViewInfo.Bounds;
			Point topLeft = new Point(bounds.Left, bounds.Top);
			Point topRight = new Point(bounds.Right - 1, bounds.Top);
			Point bottomLeft = new Point(bounds.Left, bounds.Bottom - 1);
			Point bottomRight = new Point(bounds.Right - 1, bounds.Bottom - 1);
			Point[] triagnle;
			if ((alignment == ViewInfoItemAlignment.Right) || (alignment == ViewInfoItemAlignment.Bottom))
				triagnle = new Point[] { bottomLeft, topRight, topLeft };
			else {
				Point corner = (alignment == ViewInfoItemAlignment.Left) ? topRight : bottomLeft;
				triagnle = new Point[] { topLeft, bottomRight, corner };
			}
			DrawTriangleStatus(cache, statusViewInfo, triagnle);
		}
		protected internal virtual void DrawTriangleStatus(GraphicsCache cache, AppointmentStatusViewInfo statusViewInfo, Point[] triangle) {
			cache.Paint.FillPolygon(cache.Graphics, statusViewInfo.GetBrush(cache), triangle);
			statusViewInfo.CalcBorderBounds(this);
			DrawBorders(cache, statusViewInfo);
			cache.Paint.DrawLine(cache.Graphics, cache.GetPen(statusViewInfo.BorderColor), triangle[0], triangle[1]);
		}
	}
	public class AppointmentPainter : BorderObjectPainter, IViewInfoItemPainter {
		#region Fields
		AppointmentImageProvider imageProvider;
		AppointmentStatusPainter statusPainter;
		int scrollOffset;
		[ThreadStatic]
		static ImageCollection defaultAppointmentImages;
		#endregion
		public AppointmentPainter() {
			if (defaultAppointmentImages == null)
				defaultAppointmentImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraScheduler.Images.appointment_images.png", System.Reflection.Assembly.GetExecutingAssembly(), new Size(15, 15));
			ImageCollection images = new ImageCollection(false);
			foreach (Image image in defaultAppointmentImages.Images) {
				lock (image)
					images.AddImage(image);
			}
			this.imageProvider = new AppointmentImageProvider(images);
			this.statusPainter = CreateStatusPainter();
		}
		#region Properties
		#region Appointments Layout Properties
		public int HorizontalInterspacing { get { return 2; } }
		public int VerticalInterspacing { get { return 2; } }
		public int ContentHorizontalGap { get { return 2; } }
		public virtual int LeftPadding { get { return 2; } }
		public virtual int RightPadding { get { return 2; } }
		public int TopPadding { get { return 2; } }
		public int BottomPadding { get { return 2; } }
		#endregion
		public AppointmentImageProvider ImageProvider { get { return imageProvider; } }
		public virtual ImageCollection DefaultAppointmentImages { get { return (ImageCollection)ImageProvider.DefaultAppointmentImages; } }
		public virtual Color NightClockArrowsColor { get { return Color.White; } }
		public virtual Color DayClockArrowsColor { get { return Color.Black; } }
		public virtual bool ShouldCacheSkinElementInfos { get { return false; } }
		public static int DayClockImageIndex { get { return AppointmentImageProvider.DayClockImageIndex; } }
		public static int NightClockImageIndex { get { return AppointmentImageProvider.NightClockImageIndex; } }
		public static int RecurrenceExceptionImageIndex { get { return AppointmentImageProvider.RecurrenceExceptionImageIndex; } }
		public static int RecurrenceImageIndex { get { return AppointmentImageProvider.RecurrenceImageIndex; } }
		public static int ReminderImageIndex { get { return AppointmentImageProvider.ReminderImageIndex; } }
		public AppointmentStatusPainter StatusPainter { get { return statusPainter; } }
		protected internal virtual int RightShadowSize { get { return 0; } }
		protected internal virtual int BottomShadowSize { get { return 0; } }
		protected internal virtual int VerticalStatusLineWidth { get { return 6; } }
		protected internal virtual int HorizontalStatusLineHeight { get { return 5; } }
		protected internal int ScrollOffset { get { return scrollOffset; } set { scrollOffset = value; } }
		#endregion
		protected virtual AppointmentStatusPainter CreateStatusPainter() {
			return new AppointmentStatusPainter();
		}
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return GetSameDayTopBorderWidth();
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return GetSameDayBottomBorderWidth();
		}
		protected internal virtual int GetSameDayTopBorderWidth() {
			return 1;
		}
		protected internal virtual int GetSameDayBottomBorderWidth() {
			return 1;
		}
		public virtual int GetLeftContentPadding(BorderObjectViewInfo viewInfo) {
			return 2;
		}
		public virtual int GetTopContentPadding(BorderObjectViewInfo viewInfo) {
			return GetSameDayTopContentPadding();
		}
		public virtual int GetRightContentPadding(BorderObjectViewInfo viewInfo) {
			return 2;
		}
		public virtual int GetBottomContentPadding(BorderObjectViewInfo viewInfo) {
			return GetSameDayBottomContentPadding();
		}
		protected internal virtual int GetSameDayTopContentPadding() {
			return 2;
		}
		protected internal virtual int GetSameDayBottomContentPadding() {
			return 2;
		}
		public virtual int CalcLeftStatusOffset(Rectangle leftBorderBounds) {
			return 0;
		}
		public virtual int CalcRightStatusOffset(Rectangle rightBorderBounds) {
			return 0;
		}
		protected internal virtual AppointmentViewInfoSkinElementInfoCache PrepareSkinElementInfoCache(AppointmentViewInfo viewInfo, ColoredSkinElementCache coloredSkinElementCache) {
			return null;
		}
		protected internal virtual void CacheStatusImage(AppointmentViewInfoStatusItem statusItem) {
		}
		public virtual Image GetAppointmentImage(AppointmentImageType type) {
			return ImageProvider.GetAppointmentImage(type);
		}
		public virtual int GetAppointmentImageIndex(AppointmentImageType type) {
			return ImageProvider.GetAppointmentImageIndex(type);
		}
		protected internal virtual Color GetBorderColor(AppearanceObject appointmentAppearance) {
			return appointmentAppearance.BorderColor;
		}
		protected internal virtual Color GetForeColor(AppearanceObject appointmentAppearance) {
			return appointmentAppearance.ForeColor;
		}
		protected internal virtual void DrawAppointments(GraphicsCache cache, SchedulerViewInfoBase viewInfo) {
			DrawAppointments(cache, viewInfo, ProcessAppointmentType.All);
		}
		protected internal virtual void DrawAppointments(GraphicsCache cache, SchedulerViewInfoBase viewInfo, ProcessAppointmentType selection) {
			SchedulerViewCellContainerCollection containers = GetScrollContainers(viewInfo);
			for (int i = 0; i < containers.Count; i++) {
				SchedulerViewCellContainer container = containers[i];
				AppointmentViewInfoCollection appointments = GetAppointments(viewInfo, container, selection);
				if (appointments.Count > 0) {
					int scrollOffset = container.CalculateScrollOffset();
					DrawContainerAppointments(cache, appointments, viewInfo.View.Control, container, scrollOffset);
				}
			}
		}
		protected internal virtual void DrawAppointmentsWithoutScrolling(GraphicsCache cache, AppointmentViewInfoCollection viewInfos, ISupportCustomDraw customDrawProvider) {
			DrawAppointmentsCore(cache, viewInfos, customDrawProvider);
		}
		protected internal virtual AppointmentViewInfoCollection GetAppointments(SchedulerViewInfoBase viewInfo, SchedulerViewCellContainer scrollContainer, ProcessAppointmentType selection) {
			AppointmentViewInfoCollection originalAptViewInfos = viewInfo.GetScrollContainerAppointmentViewInfos(scrollContainer);
			AppointmentViewInfoCollection copiedViewInfos;
			lock (originalAptViewInfos)
				copiedViewInfos = new AppointmentViewInfoCollection(originalAptViewInfos);
			return FilterAppointmentViewInfos(copiedViewInfos, selection);
		}
		protected internal virtual AppointmentViewInfoCollection FilterAppointmentViewInfos(AppointmentViewInfoCollection viewInfos, ProcessAppointmentType selection) {
			if (selection == ProcessAppointmentType.All)
				return viewInfos;
			AppointmentViewInfoCollection result = new AppointmentViewInfoCollection();
			bool selected = selection == ProcessAppointmentType.Selected;
			int count = viewInfos.Count;
			for (int i = 0; i < count; i++)
				if (viewInfos[i].Selected == selected)
					result.Add(viewInfos[i]);
			return result;
		}
		protected internal virtual SchedulerViewCellContainerCollection GetScrollContainers(SchedulerViewInfoBase viewInfo) {
			return viewInfo.GetScrollContainers();
		}
		protected internal virtual void DrawContainerAppointments(GraphicsCache cache, AppointmentViewInfoCollection appointments, ISupportCustomDraw customDrawProvider, SchedulerViewCellContainer container, int scrollOffset) {
			PrepareAppointmentDrawing(cache, scrollOffset);
			try {
				Rectangle clipBounds = CalculateContainerClipBounds(container);
				clipBounds.Offset(0, scrollOffset);
				using (IntersectClipper clipper = new IntersectClipper(cache, clipBounds, IntersectClipperOptions.SkipCurrentClip)) {
					DrawAppointmentsCore(cache, appointments, customDrawProvider);
				}
			} finally {
				RestoreAppointmentDrawing(cache);
			}
		}
		protected internal virtual Rectangle CalculateContainerClipBounds(SchedulerViewCellContainer container) {
			return container.Bounds;
		}
		protected internal virtual void PrepareAppointmentDrawing(GraphicsCache cache, int scrollOffset) {
			if (scrollOffset <= 0)
				return;
			cache.ResetMatrix();
			cache.Graphics.TranslateTransform(0, -scrollOffset);
			ScrollOffset = scrollOffset;
		}
		protected internal virtual void RestoreAppointmentDrawing(GraphicsCache cache) {
			cache.Graphics.ResetTransform();
			cache.ResetMatrix();
			ScrollOffset = 0;
		}
		public virtual void DrawAppointmentsCore(GraphicsCache cache, AppointmentViewInfoCollection aptViewInfos, ISupportCustomDraw customDrawProvider) {
			int count = aptViewInfos.Count;
			for (int i = 0; i < count; i++)
				DrawAppointment(cache, aptViewInfos[i], customDrawProvider);
		}
		protected internal virtual bool ShouldShowShadow(AppointmentViewInfo viewInfo) {
			return !viewInfo.Selected && viewInfo.ShowShadow;
		}
		public virtual void DrawAppointment(GraphicsCache cache, AppointmentViewInfo viewInfo, ISupportCustomDraw customDrawProvider) {
			if (!viewInfo.Visibility.Visible)
				return;
			if (cache.Paint is XBrickPaint) {
				XBrickPaint paint = (XBrickPaint)cache.Paint;
				paint.AppointmentViewInfo = viewInfo;
			}
			if (ShouldShowShadow(viewInfo))
				DrawShadow(cache, viewInfo);
			Rectangle clipBounds = cache.ClipInfo.MaximumBounds;
			using (IntersectClipper clipper = new IntersectClipper(cache, viewInfo.Bounds)) {
				DrawBackground(cache, viewInfo, customDrawProvider, () => DrawBackgroundCore(cache, viewInfo));
				DrawForeground(cache, viewInfo, clipBounds, customDrawProvider);
				if (viewInfo.DisableDrop)
					DrawDisabledAppointment(cache, viewInfo);
			}
			if (cache.Paint is XBrickPaint) {
				XBrickPaint paint = (XBrickPaint)cache.Paint;
				paint.AppointmentViewInfo = null;
			}
#if DEBUGTEST
			viewInfo.Painted = true;
#endif
		}
		public virtual void DrawAppointmentSimple(GraphicsCache cache, AppointmentViewInfo viewInfo, ISupportCustomDraw customDrawProvider) {
			if (!viewInfo.Visibility.Visible)
				return;
			using (IntersectClipper clipper = new IntersectClipper(cache, viewInfo.Bounds)) {
				DrawBackground(cache, viewInfo, customDrawProvider, () => DrawBackgroundSimpleCore(cache, viewInfo));
				if (viewInfo.DisableDrop)
					DrawDisabledAppointment(cache, viewInfo);
			}
#if DEBUGTEST
			viewInfo.Painted = true;
#endif
		}
		void DrawDisabledAppointment(GraphicsCache cache, AppointmentViewInfo viewInfo) {
			Color c = Color.FromArgb(220, 0xF0, 0xF0, 0xF0);
			cache.FillRectangle(cache.GetSolidBrush(c), viewInfo.Bounds);
		}
		protected internal virtual bool RaiseCustomDrawAppointmentBackground(GraphicsCache cache, AppointmentViewInfo viewInfo, ISupportCustomDraw customDrawProvider, DefaultDrawDelegate defaultDrawDelegate) {
			viewInfo.Cache = cache;
			try {
				CustomDrawObjectEventArgs args = new CustomDrawObjectEventArgs(viewInfo, viewInfo.Bounds, defaultDrawDelegate);
				customDrawProvider.RaiseCustomDrawAppointmentBackground(args);
				return args.Handled;
			} finally {
				viewInfo.Cache = null;
			}
		}
		protected virtual void DrawBackground(GraphicsCache cache, AppointmentViewInfo viewInfo, ISupportCustomDraw customDrawProvider, DefaultDrawDelegate defaultDraw) {
			if (RaiseCustomDrawAppointmentBackground(cache, viewInfo, customDrawProvider, defaultDraw))
				return;
			defaultDraw();
		}
		protected virtual void DrawBackgroundSimpleCore(GraphicsCache cache, AppointmentViewInfo viewInfo) {
			viewInfo.Appearance.DrawBackground(cache, viewInfo.Bounds);
			DrawBorders(cache, viewInfo);
		}
		protected virtual void DrawBackgroundCore(GraphicsCache cache, AppointmentViewInfo viewInfo) {
			DrawBackgroundContent(cache, viewInfo);
			DrawBorders(cache, viewInfo);
		}
		protected virtual void DrawBackgroundContent(GraphicsCache cache, AppointmentViewInfo viewInfo) {
			Rectangle bounds = viewInfo.Bounds;
			viewInfo.Appearance.DrawBackground(cache, bounds);
		}
		protected internal virtual bool RaiseCustomDrawAppointment(GraphicsCache cache, AppointmentViewInfo viewInfo, ISupportCustomDraw customDrawProvider, DefaultDrawDelegate defaultDrawDelegate) {
			viewInfo.Cache = cache;
			try {
				CustomDrawObjectEventArgs args = new CustomDrawObjectEventArgs(viewInfo, viewInfo.Bounds, defaultDrawDelegate);
				customDrawProvider.RaiseCustomDrawAppointment(args);
				return args.Handled;
			} finally {
				viewInfo.Cache = null;
			}
		}
		protected virtual void DrawForeground(GraphicsCache cache, AppointmentViewInfo viewInfo, Rectangle viewClipBounds, ISupportCustomDraw customDrawProvider) {
			DefaultDrawDelegate defaultDraw = delegate() { DrawForegroundCore(cache, viewInfo, viewClipBounds, customDrawProvider); };
			if (RaiseCustomDrawAppointment(cache, viewInfo, customDrawProvider, defaultDraw))
				return;
			defaultDraw();
		}
		protected virtual void DrawForegroundCore(GraphicsCache cache, AppointmentViewInfo viewInfo, Rectangle viewClipBounds, ISupportCustomDraw customDrawProvider) {
			DrawAppointmentItems(cache, viewInfo.Items);
			DrawAppointmentItems(cache, viewInfo.StatusItems);
			if (viewInfo.Selected)
				DrawSelection(cache, viewInfo, viewClipBounds, customDrawProvider);
		}
		protected virtual void DrawAppointmentItems(GraphicsCache cache, ViewInfoItemCollection items) {
			ViewInfoItemCollection itemsCopy = new ViewInfoItemCollection();
			itemsCopy.AddRange(items);
			int count = itemsCopy.Count;
			for (int i = 0; i < count; i++)
				itemsCopy[i].Draw(cache, this);
		}
		#region IViewInfoItemPainter implementation
		void IViewInfoItemPainter.DrawTextItem(GraphicsCache cache, ViewInfoTextItem item) {
			this.DrawTextItem(cache, item);
		}
		void IViewInfoItemPainter.DrawVerticalTextItem(GraphicsCache cache, ViewInfoVerticalTextItem item) {
		}
		void IViewInfoItemPainter.DrawImageItem(GraphicsCache cache, ViewInfoImageItem item) {
			this.DrawImageItem(cache, item);
		}
		void IViewInfoItemPainter.DrawHorizontalLineItem(GraphicsCache cache, ViewInfoHorizontalLineItem item) {
			this.DrawHorizontalLineItem(cache, item);
		}
		#endregion
		protected internal virtual void DrawSelection(GraphicsCache cache, AppointmentViewInfo viewInfo, Rectangle viewClipBounds, ISupportCustomDraw customDrawProvider) {
			AppearanceObject appearance = viewInfo.Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			Rectangle bounds = viewInfo.TopBorderBounds;
			bounds.Y++;
			cache.FillRectangle(borderBrush, bounds);
			bounds = viewInfo.BottomBorderBounds;
			bounds.Y--;
			cache.FillRectangle(borderBrush, bounds);
		}
		protected internal virtual void DrawImageItem(GraphicsCache cache, ViewInfoImageItem item) {
			Rectangle itemBounds = item.Bounds;
			lock (item.Image) {
				using (IntersectClipper clipper = new IntersectClipper(cache, itemBounds, new Rectangle(itemBounds.Location, item.Image.Size), IntersectClipperOptions.ApplyCurrentClipOptimizePerformance)) {
					cache.Paint.DrawImage(cache.Graphics, item.Image, itemBounds.Location);
				}
			}
		}
		protected internal virtual void DrawStatusItem(GraphicsCache cache, AppointmentViewInfoStatusItem item) {
			statusPainter.DrawRectangleStatus(cache, item.BackgroundViewInfo);
			if (item.ForegroundViewInfo.Interval.Duration == TimeSpan.Zero)
				statusPainter.DrawTriangleStatus(cache, item.ForegroundViewInfo, item.Alignment);
			else
				statusPainter.DrawRectangleStatus(cache, item.ForegroundViewInfo);
		}
		protected internal virtual void DrawHorizontalLineItem(GraphicsCache cache, ViewInfoHorizontalLineItem item) {
			item.Appearance.FillRectangle(cache, item.Bounds);
		}
		protected internal virtual void DrawTextItem(GraphicsCache cache, ViewInfoTextItem item) {
			AppearanceObject appearance = item.Appearance;
			appearance.DrawString(cache, item.Text, item.Bounds);
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((AppointmentViewInfo)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.LeftBorderBounds);
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((AppointmentViewInfo)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.TopBorderBounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((AppointmentViewInfo)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.RightBorderBounds);
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((AppointmentViewInfo)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.BottomBorderBounds);
		}
		protected internal virtual void DrawShadow(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
		}
	}
	public class AppointmentImageProvider : AppointmentImageProviderCore {
		protected internal new ImageCollection DefaultAppointmentImages { get { return (ImageCollection)base.DefaultAppointmentImages; } }
		public AppointmentImageProvider(ImageCollection defaultImages)
			: base(defaultImages) {
		}
		public virtual Image GetAppointmentImage(AppointmentImageType type) {
			int index = GetAppointmentImageIndex(type);
			return index >= 0 ? DefaultAppointmentImages.Images[index] : null;
		}
		protected internal virtual void DrawShadow(GraphicsCache cache, AppointmentViewInfo aptViewInfo) {
		}
	}
	public class AppointmentStatusImageCalculatorFactory {
		Hashtable calculatorsTable = new Hashtable();
		public AppointmentStatusImageCalculatorFactory(UserLookAndFeel lookAndFeel) {
			calculatorsTable[SchedulerSkins.SkinAppointmentStatusTopMask] = new AppointmentStatusAtTopImageCalculator(lookAndFeel, SchedulerSkins.SkinAppointmentStatusTopMask);
			calculatorsTable[SchedulerSkins.SkinAppointmentStatusLeftMask] = new AppointmentStatusAtLeftImageCalculator(lookAndFeel, SchedulerSkins.SkinAppointmentStatusLeftMask);
		}
		public AppointmentStatusImageCalculator GetStatusImageCalculator(string maskSkinElementName) {
			AppointmentStatusImageCalculator result = (AppointmentStatusImageCalculator)calculatorsTable[maskSkinElementName];
			if (result == null)
				Exceptions.ThrowInternalException();
			return result;
		}
	}
	public abstract class AppointmentStatusImageCalculator {
		#region Fields
		UserLookAndFeel lookAndFeel;
		string maskSkinElementName;
		SkinElementInfo maskSkinElementInfo;
		Bitmap[] maskBitmaps = new Bitmap[4];
		Rectangle maskNearSourceBounds;
		Rectangle maskFarSourceBounds;
		SkinPaddingEdges margins;
		#endregion
		protected AppointmentStatusImageCalculator(UserLookAndFeel lookAndFeel, string maskSkinElementName) {
			this.lookAndFeel = lookAndFeel;
			this.maskSkinElementName = maskSkinElementName;
		}
		#region Properties
		internal SkinElementInfo MaskSkinElementInfo {
			get {
				if (maskSkinElementInfo == null)
					maskSkinElementInfo = CreateMaskSkinElementInfo();
				return maskSkinElementInfo;
			}
		}
		internal SkinElementInfo CreateMaskSkinElementInfo() {
			SkinElementInfo result = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, maskSkinElementName);
			result.Bounds = result.Element.Image.GetImageBounds(0);
			CalculateMaskSourceBounds(result);
			return result;
		}
		#endregion
		internal Bitmap GetMaskBitmap(int index) {
			Bitmap result = maskBitmaps[index];
			if (result == null) {
				result = CreateMaskBitmap(index);
				maskBitmaps[index] = result;
			}
			return result;
		}
		internal Bitmap CreateMaskBitmap(int index) {
			SkinElementInfo info = this.MaskSkinElementInfo;
			Bitmap result = new Bitmap(info.Bounds.Width, info.Bounds.Height);
			using (Graphics gr = Graphics.FromImage(result)) {
				info.ImageIndex = index;
				using (GraphicsCache cache = new GraphicsCache(gr)) {
					ObjectPainter.DrawObject(cache, SkinElementPainter.Default, info);
				}
			}
			return result;
		}
		protected internal virtual void CalculateMaskSourceBounds(SkinElementInfo info) {
			if (info.Element == null)
				return;
			SkinImage skinImage = info.Element.Image;
			if (skinImage == null)
				return;
			this.margins = skinImage.SizingMargins;
			Size size = info.Bounds.Size;
			this.maskNearSourceBounds = CalculateNearBounds(size, margins);
			this.maskFarSourceBounds = CalculateFarBounds(size, margins);
		}
		protected internal abstract Rectangle CalculateNearBounds(Size size, SkinPaddingEdges margins);
		protected internal abstract Rectangle CalculateFarBounds(Size size, SkinPaddingEdges margins);
		protected internal abstract Rectangle CalculateActualNearMaskBounds(Rectangle maskBounds, Rectangle targetBounds);
		protected internal abstract Rectangle CalculateActualFarMaskBounds(Rectangle maskBounds, Rectangle targetBounds);
		protected internal abstract Rectangle CalculateActualNearTargetBounds(Rectangle maskBounds, Rectangle targetBounds);
		protected internal abstract Rectangle CalculateActualFarTargetBounds(Rectangle maskBounds, Rectangle targetBounds);
		protected internal abstract void MaskBorders(BitmapData statusBitmap, Rectangle targetNearBounds, Rectangle targetFarBounds);
		[SecuritySafeCritical]
		public void MaskStatusBitmap(Bitmap statusBitmap, AppointmentViewInfoStatusItem statusItem) {
			Bitmap maskBitmap = GetMaskBitmap(statusItem.MaskImageIndex);
			Size size = statusBitmap.Size;
			Rectangle targetNearBounds = CalculateNearBounds(size, margins);
			Rectangle targetFarBounds = CalculateFarBounds(size, margins);
			Rectangle actualNearMaskBounds = CalculateActualNearMaskBounds(maskNearSourceBounds, targetNearBounds);
			Rectangle actualFarMaskBounds = CalculateActualFarMaskBounds(maskFarSourceBounds, targetFarBounds);
			Rectangle actualNearTargetBounds = CalculateActualNearTargetBounds(actualNearMaskBounds, targetNearBounds);
			Rectangle actualFarTargetBounds = CalculateActualFarTargetBounds(actualFarMaskBounds, targetFarBounds);
			XtraSchedulerDebug.Assert(actualNearMaskBounds.Width == actualNearTargetBounds.Width);
			XtraSchedulerDebug.Assert(actualNearMaskBounds.Height == actualNearTargetBounds.Height);
			XtraSchedulerDebug.Assert(actualFarMaskBounds.Width == actualFarTargetBounds.Width);
			XtraSchedulerDebug.Assert(actualFarMaskBounds.Height == actualFarTargetBounds.Height);
			BitmapData statusBitmapData = statusBitmap.LockBits(new Rectangle(Point.Empty, size), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			try {
				BitmapData maskBitmapData = maskBitmap.LockBits(new Rectangle(0, 0, maskBitmap.Width, maskBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
				try {
					MaskStatus(statusBitmapData, maskBitmapData, targetNearBounds, maskNearSourceBounds);
					MaskStatus(statusBitmapData, maskBitmapData, targetFarBounds, maskFarSourceBounds);
					MaskBorders(statusBitmapData, targetNearBounds, targetFarBounds);
				} finally {
					maskBitmap.UnlockBits(maskBitmapData);
				}
			} finally {
				statusBitmap.UnlockBits(statusBitmapData);
			}
		}
		[SecuritySafeCritical]
		protected internal virtual void MaskTop(BitmapData statusBitmap, Rectangle targetBounds) {
			IntPtr ptr = statusBitmap.Scan0;
			int lastX = (targetBounds.Right - 1) * 4;
			for (int x = targetBounds.Left * 4; x <= lastX; x += 4)
				System.Runtime.InteropServices.Marshal.WriteInt32(ptr, x, 0);
		}
		[SecuritySafeCritical]
		protected internal virtual void MaskBottom(BitmapData statusBitmap, Rectangle targetBounds) {
			IntPtr ptr = statusBitmap.Scan0;
			int offset = statusBitmap.Stride * (targetBounds.Bottom - 1);
			int lastX = offset + (targetBounds.Right - 1) * 4;
			for (int x = offset + targetBounds.Left * 4; x <= lastX; x += 4)
				System.Runtime.InteropServices.Marshal.WriteInt32(ptr, x, 0);
		}
		[SecuritySafeCritical]
		protected internal virtual void MaskLeft(BitmapData statusBitmap, Rectangle targetBounds) {
			IntPtr ptr = statusBitmap.Scan0;
			int stride = statusBitmap.Stride;
			int lastY = (targetBounds.Bottom - 1) * stride;
			for (int y = targetBounds.Top * stride; y <= lastY; y += stride)
				System.Runtime.InteropServices.Marshal.WriteInt32(ptr, y, 0);
		}
		[SecuritySafeCritical]
		protected internal virtual void MaskRight(BitmapData statusBitmap, Rectangle targetBounds) {
			IntPtr ptr = statusBitmap.Scan0;
			int stride = statusBitmap.Stride;
			int offset = (targetBounds.Right - 1) * 4;
			int lastY = offset + (targetBounds.Bottom - 1) * stride;
			for (int y = offset + targetBounds.Top * stride; y <= lastY; y += stride)
				System.Runtime.InteropServices.Marshal.WriteInt32(ptr, y, 0);
		}
		[SecuritySafeCritical]
		void MaskStatus(BitmapData status, BitmapData mask, Rectangle targetBounds, Rectangle maskBounds) {
			int sourceX, sourceY;
			int lastSourceX = Math.Min(maskBounds.Right - 1, targetBounds.Right - 1);
			int lastSourceY = Math.Min(maskBounds.Bottom - 1, targetBounds.Bottom - 1);
			IntPtr maskPtr = mask.Scan0;
			int maskStride = mask.Stride;
			IntPtr statusPtr = status.Scan0;
			int statusStride = status.Stride;
			int targetX, targetY;
			for (sourceY = maskBounds.Y, targetY = targetBounds.Y; sourceY <= lastSourceY; sourceY++, targetY++) {
				for (sourceX = maskBounds.X, targetX = targetBounds.X; sourceX <= lastSourceX; sourceX++, targetX++) {
					if (System.Runtime.InteropServices.Marshal.ReadByte(maskPtr, sourceY * maskStride + sourceX * 4) == 0)
						System.Runtime.InteropServices.Marshal.WriteByte(statusPtr, targetY * statusStride + targetX * 4 + 3, 0);
				}
			}
		}
	}
	public class AppointmentStatusAtTopImageCalculator : AppointmentStatusImageCalculator {
		public AppointmentStatusAtTopImageCalculator(UserLookAndFeel lookAndFeel, string maskSkinElementName)
			: base(lookAndFeel, maskSkinElementName) {
		}
		protected internal override Rectangle CalculateNearBounds(Size size, SkinPaddingEdges margins) {
			return new Rectangle(0, 0, Math.Min(margins.Left, size.Width), size.Height);
		}
		protected internal override Rectangle CalculateFarBounds(Size size, SkinPaddingEdges margins) {
			int width = Math.Min(margins.Right, size.Width);
			return new Rectangle(size.Width - width, 0, width, size.Height);
		}
		protected internal override Rectangle CalculateActualNearMaskBounds(Rectangle maskBounds, Rectangle targetBounds) {
			return new Rectangle(maskBounds.X, maskBounds.Y, Math.Min(maskBounds.Width, targetBounds.Width), Math.Min(maskBounds.Height, targetBounds.Height));
		}
		protected internal override Rectangle CalculateActualFarMaskBounds(Rectangle maskBounds, Rectangle targetBounds) {
			int minWidth = Math.Min(maskBounds.Width, targetBounds.Width);
			return new Rectangle(maskBounds.X + (maskBounds.Width - minWidth), maskBounds.Y, minWidth, Math.Min(maskBounds.Height, targetBounds.Height));
		}
		protected internal override Rectangle CalculateActualNearTargetBounds(Rectangle maskBounds, Rectangle targetBounds) {
			return new Rectangle(targetBounds.X, targetBounds.Y, Math.Min(maskBounds.Width, targetBounds.Width), Math.Min(maskBounds.Height, targetBounds.Height));
		}
		protected internal override Rectangle CalculateActualFarTargetBounds(Rectangle maskBounds, Rectangle targetBounds) {
			int minWidth = Math.Min(maskBounds.Width, targetBounds.Width);
			return new Rectangle(targetBounds.X + (targetBounds.Width - minWidth), targetBounds.Y, minWidth, Math.Min(maskBounds.Height, targetBounds.Height));
		}
		protected internal override void MaskBorders(BitmapData statusBitmap, Rectangle targetNearBounds, Rectangle targetFarBounds) {
			Rectangle horizontalBounds = Rectangle.FromLTRB(targetNearBounds.Left, targetNearBounds.Top, targetFarBounds.Right, targetFarBounds.Bottom);
			Rectangle verticalBounds = Rectangle.FromLTRB(targetNearBounds.Left, targetNearBounds.Top, targetFarBounds.Right, targetFarBounds.Bottom);
			MaskTop(statusBitmap, horizontalBounds);
			MaskLeft(statusBitmap, verticalBounds);
			MaskRight(statusBitmap, verticalBounds);
		}
	}
	public class AppointmentStatusAtLeftImageCalculator : AppointmentStatusImageCalculator {
		public AppointmentStatusAtLeftImageCalculator(UserLookAndFeel lookAndFeel, string maskSkinElementName)
			: base(lookAndFeel, maskSkinElementName) {
		}
		protected internal override Rectangle CalculateNearBounds(Size size, SkinPaddingEdges margins) {
			return new Rectangle(0, 0, size.Width, Math.Min(margins.Top, size.Height));
		}
		protected internal override Rectangle CalculateFarBounds(Size size, SkinPaddingEdges margins) {
			int height = Math.Min(size.Height, margins.Bottom);
			return new Rectangle(0, size.Height - height, size.Width, height);
		}
		protected internal override Rectangle CalculateActualNearMaskBounds(Rectangle maskBounds, Rectangle targetBounds) {
			return new Rectangle(maskBounds.X, maskBounds.Y, Math.Min(maskBounds.Width, targetBounds.Width), Math.Min(maskBounds.Height, targetBounds.Height));
		}
		protected internal override Rectangle CalculateActualFarMaskBounds(Rectangle maskBounds, Rectangle targetBounds) {
			int minHeight = Math.Min(maskBounds.Height, targetBounds.Height);
			return new Rectangle(maskBounds.X, maskBounds.Y + (maskBounds.Height - minHeight), Math.Min(maskBounds.Width, targetBounds.Width), minHeight);
		}
		protected internal override Rectangle CalculateActualNearTargetBounds(Rectangle maskBounds, Rectangle targetBounds) {
			return new Rectangle(targetBounds.X, targetBounds.Y, Math.Min(maskBounds.Width, targetBounds.Width), Math.Min(maskBounds.Height, targetBounds.Height));
		}
		protected internal override Rectangle CalculateActualFarTargetBounds(Rectangle maskBounds, Rectangle targetBounds) {
			int minHeight = Math.Min(maskBounds.Height, targetBounds.Height);
			return new Rectangle(targetBounds.X, targetBounds.Y + (targetBounds.Height - minHeight), Math.Min(maskBounds.Width, targetBounds.Width), minHeight);
		}
		protected internal override void MaskBorders(BitmapData statusBitmap, Rectangle targetNearBounds, Rectangle targetFarBounds) {
			Rectangle horizontalBounds = Rectangle.FromLTRB(targetNearBounds.Left, targetNearBounds.Top, targetFarBounds.Right, targetFarBounds.Bottom);
			Rectangle verticalBounds = Rectangle.FromLTRB(targetNearBounds.Left, targetNearBounds.Bottom, targetFarBounds.Right, targetFarBounds.Top);
			MaskTop(statusBitmap, horizontalBounds);
			MaskLeft(statusBitmap, verticalBounds);
			MaskBottom(statusBitmap, horizontalBounds);
		}
	}
	public class AppointmentSkinPainter : AppointmentPainter {
		AppointmentStatusImageCalculatorFactory statusImageCalculatorFactory;
		UserLookAndFeel lookAndFeel;
		public AppointmentSkinPainter(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
			this.statusImageCalculatorFactory = new AppointmentStatusImageCalculatorFactory(lookAndFeel);
		}
		protected internal UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		public override bool ShouldCacheSkinElementInfos { get { return true; } }
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			return aptViewInfo.SameDay ? CalculateObjectMinSize(SchedulerSkins.SkinAppointmentSameDayLeftBorder).Width : CalculateObjectMinSize(SchedulerSkins.SkinAppointmentLeftBorder).Width;
		}
		public override int GetNoLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			return aptViewInfo.SameDay ? CalculateObjectMinSize(SchedulerSkins.SkinAppointmentSameDayLeftBorder).Width : CalculateObjectMinSize(SchedulerSkins.SkinAppointmentNoLeftBorder).Width;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			return aptViewInfo.SameDay ? CalculateObjectMinSize(SchedulerSkins.SkinAppointmentSameDayRightBorder).Width : CalculateObjectMinSize(SchedulerSkins.SkinAppointmentRightBorder).Width;
		}
		public override int GetNoRightBorderWidth(BorderObjectViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			return aptViewInfo.SameDay ? CalculateObjectMinSize(SchedulerSkins.SkinAppointmentSameDayRightBorder).Width : CalculateObjectMinSize(SchedulerSkins.SkinAppointmentNoRightBorder).Width;
		}
		public override int GetLeftContentPadding(BorderObjectViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			return aptViewInfo.SameDay ? CalculateContentMargins(SchedulerSkins.SkinAppointmentSameDayContent).Left : CalculateContentMargins(SchedulerSkins.SkinAppointmentContent).Left;
		}
		public override int GetRightContentPadding(BorderObjectViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			return aptViewInfo.SameDay ? CalculateContentMargins(SchedulerSkins.SkinAppointmentSameDayContent).Right : CalculateContentMargins(SchedulerSkins.SkinAppointmentContent).Right;
		}
		public override int GetTopContentPadding(BorderObjectViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			return aptViewInfo.SameDay ? GetSameDayTopContentPadding() : CalculateContentMargins(SchedulerSkins.SkinAppointmentContent).Top;
		}
		public override int GetBottomContentPadding(BorderObjectViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			return aptViewInfo.SameDay ? GetSameDayBottomContentPadding() : CalculateContentMargins(SchedulerSkins.SkinAppointmentContent).Bottom;
		}
		protected internal override int GetSameDayTopContentPadding() {
			return CalculateContentMargins(SchedulerSkins.SkinAppointmentSameDayContent).Top;
		}
		protected internal override int GetSameDayBottomContentPadding() {
			return CalculateContentMargins(SchedulerSkins.SkinAppointmentSameDayContent).Bottom;
		}
		protected internal override int GetSameDayTopBorderWidth() {
			return 0;
		}
		protected internal override int GetSameDayBottomBorderWidth() {
			return 0;
		}
		protected internal override int VerticalStatusLineWidth {
			get {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				return skin.Properties.GetInteger(SchedulerSkins.OptAppointmentVerticalStatusLineWidth);
			}
		}
		protected internal override int HorizontalStatusLineHeight {
			get {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				return skin.Properties.GetInteger(SchedulerSkins.OptAppointmentHorizontalStatusLineHeight);
			}
		}
		public override int CalcLeftStatusOffset(Rectangle leftBorderBounds) {
			return -leftBorderBounds.Width;
		}
		public override int CalcRightStatusOffset(Rectangle rightBorderBounds) {
			return rightBorderBounds.Width;
		}
		protected internal override Color GetBorderColor(AppearanceObject appointmentAppearance) {
			Skin skin = SchedulerSkins.GetSkin(LookAndFeel);
			SkinElement borderElement = skin[SchedulerSkins.SkinAppointmentBorder];
			Color sourceColor = borderElement.Border.All;
			SkinElement appointmentElement = skin[SchedulerSkins.SkinAppointment];
			Color appearanceBackColor = appointmentAppearance.BackColor;
			if (appearanceBackColor == SystemColors.Window)
				return sourceColor;
			Color appointmentBaseColor = appointmentElement.Properties.GetColor(SchedulerSkins.PropBaseColor);
			ColorMatrix m = DevExpress.Utils.Paint.XPaint.GetColorMatrix(appointmentBaseColor, appearanceBackColor);
			return SkinElementColorer.ApplyColorTransform(sourceColor, m);
		}
		protected internal override Color GetForeColor(AppearanceObject appointmentAppearance) {
			return SkinPainterHelper.GetSkinForeColor(LookAndFeel, SchedulerSkins.SkinAppointment);
		}
		protected override void DrawBackgroundContent(GraphicsCache cache, AppointmentViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			SkinElementInfo el = aptViewInfo.CachedSkinElementInfos.Content;
			el.ImageIndex = viewInfo.Selected ? 1 : 0;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			SkinElementInfo el = aptViewInfo.CachedSkinElementInfos.LeftBorder;
			el.ImageIndex = aptViewInfo.Selected ? 1 : 0;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppointmentViewInfo aptViewInfo = (AppointmentViewInfo)viewInfo;
			SkinElementInfo el = aptViewInfo.CachedSkinElementInfos.RightBorder;
			el.ImageIndex = aptViewInfo.Selected ? 1 : 0;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
		}
		protected SkinElementInfo GetSkinElementInfo(string skinElementName, Rectangle bounds) {
			return SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, skinElementName, bounds);
		}
		protected Size CalculateObjectMinSize(string skinElementName) {
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, skinElementName);
			Rectangle minBounds = ObjectPainter.CalcObjectMinBounds(null, SkinElementPainter.Default, el);
			return minBounds.Size;
		}
		protected internal SkinPaddingEdges CalculateContentMargins(string skinElementName) {
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, skinElementName);
			return el.Element.ContentMargins;
		}
		protected internal override AppointmentViewInfoSkinElementInfoCache PrepareSkinElementInfoCache(AppointmentViewInfo viewInfo, ColoredSkinElementCache coloredSkinElementCache) {
			AppointmentViewInfoSkinElementInfoCache cache = new AppointmentViewInfoSkinElementInfoCache();
			Color color = viewInfo.Appearance.BackColor;
			cache.RightBorder = coloredSkinElementCache.GetAppointmentSkinElementInfo(viewInfo.EndBorderSkinElementName, color, LookAndFeel, viewInfo.RightBorderBounds);
			cache.LeftBorder = coloredSkinElementCache.GetAppointmentSkinElementInfo(viewInfo.StartBorderSkinElementName, color, LookAndFeel, viewInfo.LeftBorderBounds);
			int progressWidth = GetAppointmentPercentCompleteBoundsWidth(viewInfo);
			Rectangle bounds = Rectangle.FromLTRB(viewInfo.LeftBorderBounds.Right, viewInfo.Bounds.Top, viewInfo.RightBorderBounds.Left, viewInfo.Bounds.Bottom);
			Rectangle contentBounds = new Rectangle(bounds.X + progressWidth, bounds.Y, bounds.Width - progressWidth, bounds.Height);
			cache.Content = coloredSkinElementCache.GetAppointmentSkinElementInfo(viewInfo.ContentSkinElementName, color, LookAndFeel, contentBounds);
			PrepareSkinElementInfosForPercentComplete(viewInfo, cache, progressWidth, bounds);
			return cache;
		}
		protected internal virtual void PrepareSkinElementInfosForPercentComplete(AppointmentViewInfo viewInfo, AppointmentViewInfoSkinElementInfoCache cache, int progressWidth, Rectangle contentBounds) {
		}
		protected internal virtual int GetAppointmentPercentCompleteBoundsWidth(AppointmentViewInfo viewInfo) {
			return 0;
		}
		protected internal virtual SkinElementInfo ModifySkinElement(SkinElementInfo el, Rectangle newBounds, Color newColor, bool selected) {
			SkinElementInfo newElement = new SkinElementInfo(el.Element, newBounds);
			newElement.ImageIndex = selected ? 1 : 0;
			return SkinElementColorer.PaintElementWithColor(newElement, newColor);
		}
		protected internal override void CacheStatusImage(AppointmentViewInfoStatusItem statusItem) {
			Bitmap statusBitmap = CreateUnclippedStatusBitmap(statusItem);
			AppointmentStatusImageCalculator calc = statusImageCalculatorFactory.GetStatusImageCalculator(statusItem.SkinElementName);
			calc.MaskStatusBitmap(statusBitmap, statusItem);
			statusItem.CachedImage = statusBitmap;
		}
		protected internal virtual Bitmap CreateUnclippedStatusBitmap(AppointmentViewInfoStatusItem statusItem) {
			Rectangle bounds = statusItem.Bounds;
			Bitmap result = new Bitmap(Math.Max(1, bounds.Width), Math.Max(1, bounds.Height));
			using (Graphics gr = Graphics.FromImage(result)) {
				gr.TranslateTransform(-bounds.X, -bounds.Y);
				using (GraphicsCache cache = new GraphicsCache(gr)) {
					base.DrawStatusItem(cache, statusItem);
				}
			}
			return result;
		}
		protected internal override void DrawSelection(GraphicsCache cache, AppointmentViewInfo viewInfo, Rectangle viewClipBounds, ISupportCustomDraw customDrawProvider) {
		}
		protected internal override void DrawStatusItem(GraphicsCache cache, AppointmentViewInfoStatusItem item) {
			Image image = item.CachedImage;
			if (image != null)
				cache.Graphics.DrawImage(image, item.Bounds.X, item.Bounds.Y);
		}
	}
	public static class ClockArrowsPainterHelper {
		public static void DrawClockArrows(GraphicsCache cache, AppointmentViewInfoClockImageItem clockitem) {
			DateTime time = clockitem.Time;
			lock (clockitem.Image) {
				Rectangle imageBounds = new Rectangle(clockitem.Bounds.Location, clockitem.Image.Size);
				Rectangle itemBounds = clockitem.Bounds;
				Pen pen = cache.GetPen(clockitem.ArrowsColor);
				int radius = Math.Min(imageBounds.Width, imageBounds.Height) / 2;
				Point clockCenter = new Point(imageBounds.X + radius, imageBounds.Y + radius);
				using (IntersectClipper clipper = new IntersectClipper(cache, itemBounds, imageBounds, IntersectClipperOptions.ApplyCurrentClipOptimizePerformance)) {
					DrawClockLine(cache, pen, clockCenter, radius - 3, GetHourHandPosition(time.Hour, time.Minute));
					DrawClockLine(cache, pen, clockCenter, radius - 1, time.Minute );
				}
			}
		}
		public static void DrawClockLine(GraphicsCache cache, Pen pen, Point center, int radius, int minuteHandPosition) {
			double angle = GetAngleByPosition(minuteHandPosition);
			Point pt = Point.Empty;
			pt.X = center.X + Convert.ToInt32(radius * Math.Cos(angle));
			pt.Y = center.Y - Convert.ToInt32(radius * Math.Sin(angle));
			cache.Paint.DrawLine(cache.Graphics, pen, center, pt);
		}
		internal static double GetAngleByPosition(int minuteHandPosition) {
			return Math.PI * GetAngleByPositionInDegrees(minuteHandPosition) / 180;
		}
		internal static int GetAngleByPositionInDegrees(int minuteHandPosition) {
			return (int)(90 - 6 * minuteHandPosition);
		}
		public static int GetHourHandPosition(int hour, int minute) {
			return (hour % 12) * 5 + minute / 12;
		}
	}
	public class MoreButtonPainter {
		public virtual void Draw(GraphicsCache cache, MoreButton moreButton) {
			if (moreButton.Visible) {
				Image image = moreButton.GoUp ? MoreButton.MoreButtonUp : MoreButton.MoreButtonDown;
				cache.Paint.DrawImage(cache.Graphics, image, moreButton.Bounds);
			}
		}
		protected internal virtual Size CalculateObjectMinSize() {
			return new Size(MoreButton.MoreButtonUp.Width, MoreButton.MoreButtonUp.Height);
		}
	}
	public class MoreButtonSkinPainter : MoreButtonPainter {
		UserLookAndFeel lookAndFeel;
		public MoreButtonSkinPainter(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		public override void Draw(GraphicsCache cache, MoreButton moreButton) {
			if (moreButton.Visible) {
				SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinMoreButton, moreButton.Bounds);
				el.ImageIndex = moreButton.CalculateImageIndex();
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
			}
		}
		protected internal override Size CalculateObjectMinSize() {
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, SchedulerSkins.SkinMoreButton);
			Rectangle minBounds = ObjectPainter.CalcObjectMinBounds(null, SkinElementPainter.Default, el);
			return minBounds.Size;
		}
	}
	public abstract class NavigationButtonPainter : BorderObjectPainter, IViewInfoItemPainter {
		public virtual bool CanCacheSkinElementInfo { get { return false; } }
		public virtual int HorizontalOverlap { get { return 1; } }
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public virtual int GetContentLeftPadding(NavigationButton button) { return 2; }
		public virtual int GetContentRightPadding(NavigationButton button) { return 2; }
		public virtual int GetContentTopPadding(NavigationButton button) { return 2; }
		public virtual int GetContentBottomPadding(NavigationButton button) { return 2; }
		public virtual int GetContentVerticalGap(NavigationButton button) { return 2; }
		protected internal virtual Color GetDisplayTextForeColor(NavigationButton button) {
			return button.Appearance.ForeColor;
		}
		public virtual SkinElementInfo PrepareCachedSkinElementInfo(string skinElementName, Rectangle bounds, Color color) {
			return null;
		}
		public virtual void Draw(GraphicsCache cache, NavigationButton button, SchedulerControl control) {
			DefaultDrawDelegate defaultDraw = delegate() { DrawCore(cache, button); };
			if (RaiseCustomDrawNavigationButton(cache, button, control, defaultDraw))
				return;
			defaultDraw();
		}
		protected virtual void DrawCore(GraphicsCache cache, NavigationButton button) {
			DrawBackground(cache, button);
			DrawBorders(cache, button);
			DrawItems(cache, button.Items);
		}
		protected internal virtual bool RaiseCustomDrawNavigationButton(GraphicsCache cache, NavigationButton button, SchedulerControl control, DefaultDrawDelegate defaultDrawDelegate) {
			button.Cache = cache;
			try {
				CustomDrawObjectEventArgs args = new CustomDrawObjectEventArgs(button, button.Bounds, defaultDrawDelegate);
				control.RaiseCustomDrawNavigationButton(args);
				return args.Handled;
			} finally {
				button.Cache = null;
			}
		}
		protected internal virtual Image GetButtonImage(NavigationButton button) {
			return button.DefaultImage;
		}
		protected internal virtual int GetButtonImageCount(NavigationButton button) {
			return 1;
		}
		protected internal virtual Size CalculateObjectMinSize(GraphicsCache cache, NavigationButton button, NavigationButtonPreliminaryLayoutResult preliminaryLayout) {
			return Size.Empty;
		}
		#region IViewInfoItemPainter implementation
		void IViewInfoItemPainter.DrawTextItem(GraphicsCache cache, ViewInfoTextItem item) {
		}
		void IViewInfoItemPainter.DrawVerticalTextItem(GraphicsCache cache, ViewInfoVerticalTextItem item) {
			this.DrawVerticalTextItem(cache, item);
		}
		void IViewInfoItemPainter.DrawImageItem(GraphicsCache cache, ViewInfoImageItem item) {
			this.DrawImageItem(cache, item);
		}
		void IViewInfoItemPainter.DrawHorizontalLineItem(GraphicsCache cache, ViewInfoHorizontalLineItem item) {
		}
		#endregion
		protected internal virtual void DrawVerticalTextItem(GraphicsCache cache, ViewInfoVerticalTextItem item) {
			AppearanceObject appearance = item.Appearance;
			appearance.DrawVString(cache, item.Text, appearance.Font, appearance.GetForeBrush(cache), item.Bounds, appearance.GetStringFormat(TextOptions.DefaultOptionsCenteredWithEllipsis), 270);
		}
		protected internal virtual void DrawImageItem(GraphicsCache cache, ViewInfoImageItem item) {
			using (IntersectClipper clipper = new IntersectClipper(cache, item.Bounds)) {
				cache.Paint.DrawImage(cache.Graphics, item.Image, item.Bounds.Location);
			}
		}
		protected virtual void DrawBackground(GraphicsCache cache, NavigationButton button) {
			button.Appearance.DrawBackground(cache, button.Bounds);
		}
		protected virtual void DrawItems(GraphicsCache cache, ViewInfoItemCollection items) {
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				items[i].Draw(cache, this);
			}
		}
	}
	public class NavigationButtonFlatPainter : NavigationButtonPainter {
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 2;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 2;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 1;
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetTopSideRect(viewInfo.TopBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
			bounds.Y++;
			cache.FillRectangle(brushes.LightLight, bounds);
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetBottomSideRect(viewInfo.BottomBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetLeftSideRect(viewInfo.LeftBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
			bounds.X++;
			bounds.Inflate(0, -1);
			cache.FillRectangle(brushes.LightLight, bounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			BBrushes brushes = CreateBorderBrushes(cache, viewInfo);
			Rectangle bounds = RectUtils.GetLeftSideRect(viewInfo.RightBorderBounds, 1);
			cache.FillRectangle(brushes.Dark, bounds);
		}
		protected virtual BBrushes CreateBorderBrushes(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			NavigationButton button = (NavigationButton)viewInfo;
			return new BBrushes(cache, button.Appearance.BorderColor);
		}
	}
	public class NavigationButtonUltraFlatPainter : NavigationButtonPainter {
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((NavigationButton)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.LeftBorderBounds);
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((NavigationButton)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.TopBorderBounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((NavigationButton)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.RightBorderBounds);
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((NavigationButton)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.BottomBorderBounds);
		}
	}
	public class NavigationButtonOffice2003Painter : NavigationButtonPainter {
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((NavigationButton)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.LeftBorderBounds);
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((NavigationButton)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.TopBorderBounds);
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((NavigationButton)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.RightBorderBounds);
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
			AppearanceObject appearance = ((NavigationButton)viewInfo).Appearance;
			Brush borderBrush = appearance.GetBorderBrush(cache);
			cache.FillRectangle(borderBrush, viewInfo.BottomBorderBounds);
		}
	}
	public class NavigationButtonWindowsXPPainter : NavigationButtonPainter {
		WXPPainterArgs drawArgs = new WXPPainterArgs("tab", XPConstants.TABP_TABITEM, XPConstants.TIBES_NORMAL);
		protected WXPPainterArgs DrawArgs { get { return drawArgs; } }
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		protected override void DrawBackground(GraphicsCache cache, NavigationButton button) {
			DrawArgs.Bounds = new Rectangle(0, 0, button.Bounds.Height, button.Bounds.Width);
			DrawArgs.ThemeHandle = IntPtr.Zero;
			if (button.Enabled)
				DrawArgs.StateId = button.HotTracked ? XPConstants.TIBES_HOT : XPConstants.TIBES_NORMAL;
			else
				DrawArgs.StateId = XPConstants.TIBES_DISABLED;
			Rectangle saveBounds = DrawArgs.Bounds;
			try {
				using (Bitmap bitmap = CreateBackgroundBitmap(button)) {
					RotateFlipType type = button is NavigationButtonNext ? RotateFlipType.Rotate270FlipNone : RotateFlipType.Rotate90FlipNone;
					bitmap.RotateFlip(type);
					cache.Graphics.DrawImage(bitmap, button.Bounds.Location);
				}
			} finally {
				DrawArgs.Bounds = saveBounds;
			}
		}
		protected internal virtual Bitmap CreateBackgroundBitmap(NavigationButton button) {
			Bitmap result = new Bitmap(button.Bounds.Height, button.Bounds.Width);
			using (Graphics gr = Graphics.FromImage(result)) {
				WXPPainter.Default.DrawTheme(DrawArgs, gr, null);
			}
			return result;
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
	}
	public class NavigationButtonSkinPainter : NavigationButtonPainter {
		UserLookAndFeel lookAndFeel;
		public NavigationButtonSkinPainter(UserLookAndFeel lookAndFeel) {
			this.lookAndFeel = lookAndFeel;
		}
		public override bool CanCacheSkinElementInfo {
			get {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				return skin.Properties.GetBoolean(SchedulerSkins.OptPaintResourceHeaderWithResourceColor);
			}
		}
		public virtual bool ShouldPaintHotTrackedWithResourceColor {
			get {
				Skin skin = SchedulerSkins.GetSkin(lookAndFeel);
				return skin.Properties.GetBoolean(SchedulerSkins.OptPaintHotTrackedWithResourceColor);
			}
		}
		public override int GetContentLeftPadding(NavigationButton button) { return GetButtonPaddings(button).Left; }
		public override int GetContentRightPadding(NavigationButton button) { return GetButtonPaddings(button).Right; }
		public override int GetContentTopPadding(NavigationButton button) { return GetButtonPaddings(button).Top; }
		public override int GetContentBottomPadding(NavigationButton button) { return GetButtonPaddings(button).Bottom; }
		public override int GetLeftBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override int GetRightBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override int GetTopBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override int GetBottomBorderWidth(BorderObjectViewInfo viewInfo) {
			return 0;
		}
		public override SkinElementInfo PrepareCachedSkinElementInfo(string skinElementName, Rectangle bounds, Color color) {
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, skinElementName, bounds);
			return SkinElementColorer.PaintElementWithColor(el, color);
		}
		protected virtual SkinPaddingEdges GetButtonPaddings(NavigationButton button) {
			return SkinPainterHelper.GetSkinPaddings(lookAndFeel, button.SkinElementName);
		}
		public override int GetContentVerticalGap(NavigationButton button) {
			SkinElement skinEl = SkinPainterHelper.GetSkinElement(lookAndFeel, button.SkinElementName);
			return skinEl.Properties.GetInteger(SchedulerSkins.PropContentVerticalGap);
		}
		protected internal override Color GetDisplayTextForeColor(NavigationButton button) {
			SkinElement skinEl = SkinPainterHelper.GetSkinElement(lookAndFeel, button.SkinElementName);
			return button.Enabled ? skinEl.Color.GetForeColor() : skinEl.Properties.GetColor(SchedulerSkins.PropAlternateColor);
		}
		protected override void DrawBackground(GraphicsCache cache, NavigationButton button) {
			SkinElementInfo el;
			if (ShouldUseCachedSkinElement(button.CachedSkinElementInfo, button))
				el = button.CachedSkinElementInfo;
			else
				el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, button.SkinElementName, button.Bounds);
			int index = button.CalculateImageIndex();
			el.ImageIndex = index;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
			DrawArrowElement(cache, button, index);
		}
		protected internal virtual void DrawArrowElement(GraphicsCache cache, NavigationButton button, int index) {
			if (button.ImageItem == null)
				return;
			SkinElementInfo el;
			if (ShouldUseCachedSkinElement(button.CachedSkinArrowElementInfo, button))
				el = button.CachedSkinArrowElementInfo;
			else
				el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, button.SkinArrowElementName, button.ImageItem.Bounds);
			el.ImageIndex = index;
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, el);
		}
		protected virtual bool ShouldUseCachedSkinElement(SkinElementInfo el, NavigationButton button) {
			if (el == null)
				return false;
			return (button.Enabled && button.HotTrackedInternal) ? ShouldPaintHotTrackedWithResourceColor : true;
		}
		protected internal override Image GetButtonImage(NavigationButton button) {
			SkinElement element = SkinPainterHelper.GetSkinElement(lookAndFeel, button.SkinArrowElementName);
			return element != null ? element.Image.Image : base.GetButtonImage(button);
		}
		protected internal override int GetButtonImageCount(NavigationButton button) {
			SkinElement element = SkinPainterHelper.GetSkinElement(lookAndFeel, button.SkinArrowElementName);
			return element != null ? element.Image.ImageCount : base.GetButtonImageCount(button);
		}
		protected internal override Size CalculateObjectMinSize(GraphicsCache cache, NavigationButton button, NavigationButtonPreliminaryLayoutResult preliminaryLayout) {
			SkinElementInfo el = SkinPainterHelper.UpdateObjectInfoArgs(lookAndFeel, button.SkinElementName);
			Rectangle bounds = ObjectPainter.CalcObjectMinBounds(null, SkinElementPainter.Default, el);
			return MathUtils.Max(bounds.Size, preliminaryLayout.ImageSize);
		}
		protected internal override void DrawImageItem(GraphicsCache cache, ViewInfoImageItem item) {
		}
		protected override void DrawLeftBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawTopBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawRightBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
		protected override void DrawBottomBorder(GraphicsCache cache, BorderObjectViewInfo viewInfo) {
		}
	}
	public enum IntersectClipperOptions {
		SkipCurrentClip,
		ApplyCurrentClip,
		ApplyCurrentClipOptimizePerformance
	}
	public class IntersectClipper : IDisposable {
		GraphicsCache cache;
		GraphicsClipState oldClipping;
		Rectangle oldMaxBounds;
		bool clipChanged = false;
		public IntersectClipper(GraphicsCache cache, Rectangle clipBounds)
			: this(cache, clipBounds, clipBounds, IntersectClipperOptions.ApplyCurrentClip) {
		}
		public IntersectClipper(GraphicsCache cache, Rectangle clipBounds, IntersectClipperOptions options)
			: this(cache, clipBounds, clipBounds, options) {
		}
		public IntersectClipper(GraphicsCache cache, Rectangle clipBounds, Rectangle drawingBounds, IntersectClipperOptions options) {
			Guard.ArgumentNotNull(cache, "cache");
			this.cache = cache;
			GraphicsClip clipInfo = cache.ClipInfo;
			this.oldMaxBounds = clipInfo.MaximumBounds;
			if (options == IntersectClipperOptions.ApplyCurrentClipOptimizePerformance)
				CalculateNewClipOptimalPerfomance(cache, clipBounds, drawingBounds);
			else
				CalculateNewClip(clipBounds, options, clipInfo);
		}
		protected internal virtual void CalculateNewClip(Rectangle clipBounds, IntersectClipperOptions options, GraphicsClip clipInfo) {
			XtraSchedulerDebug.Assert(options != IntersectClipperOptions.ApplyCurrentClipOptimizePerformance);
			Rectangle newClip = Rectangle.Empty;
			if (options == IntersectClipperOptions.ApplyCurrentClip)
				newClip = Rectangle.Intersect(oldMaxBounds, clipBounds);
			if (options == IntersectClipperOptions.SkipCurrentClip)
				newClip = clipBounds;
			SetNewClip(clipInfo, newClip);
		}
		protected internal virtual void CalculateNewClipOptimalPerfomance(GraphicsCache cache, Rectangle clipBounds, Rectangle drawingBounds) {
			if (clipBounds.Width < drawingBounds.Width || clipBounds.Height < drawingBounds.Height) {
				Rectangle newClip = Rectangle.Intersect(oldMaxBounds, clipBounds);
				SetNewClip(cache.ClipInfo, newClip);
				return;
			}
			if (!oldMaxBounds.Contains(drawingBounds)) {
				Rectangle newClip = Rectangle.Intersect(oldMaxBounds, drawingBounds);
				SetNewClip(cache.ClipInfo, newClip);
			}
		}
		protected internal virtual void SetNewClip(GraphicsClip clipInfo, Rectangle newClip) {
			clipInfo.MaximumBounds = newClip;
			this.oldClipping = clipInfo.SaveAndSetClip(newClip);
			this.clipChanged = true;
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (clipChanged) {
					GraphicsClip clipInfo = cache.ClipInfo;
					clipInfo.RestoreClipRelease(this.oldClipping);
					clipInfo.MaximumBounds = this.oldMaxBounds;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~IntersectClipper() {
			Dispose(false);
		}
		#endregion
	}
}
