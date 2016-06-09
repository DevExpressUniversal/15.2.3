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
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.Web.ASPxScheduler.Rendering;
using DevExpress.Web.Internal;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Implementations;
namespace DevExpress.Web.ASPxScheduler {
	#region ASPxSchedulerStyleNames
	static class ASPxSchedulerStyleNames {
		public const string AllDayArea = "AllDayAreaStyle";
		public const string AllDayAreaLeft = "AllDayAreaLeftStyle";
		public const string ResourceNavigator = "ResourceNavigatorStyle";
		public const string ViewNavigator = "ViewNavigatorStyle";
		public const string ViewSelector = "ViewSelectorStyle";
		public const string ViewVisibleInterval = "ViewVisibleIntervalStyle";
		public const string DateHeader = "DateHeaderStyle";
		public const string AlternateDateHeader = "AlternateDateHeaderStyle";
		public const string HorizontalResourceHeader = "HorizontalResourceHeaderStyle";
		public const string VerticalResourceHeader = "VerticalResourceHeaderStyle";
		public const string DayHeader = "DayHeaderStyle";
		public const string RightTopCorner = "RightTopCornerStyle";
		public const string LeftTopCorner = "LeftTopCornerStyle";
		public const string GroupSeparatorHorizontal = "GroupSeparatorHorizontalStyle";
		public const string GroupSeparatorVertical = "GroupSeparatorVerticalStyle";
		public const string Appointment = "AppointmentStyle";
		public const string AppointmentInnerBorders = "AppointmentInnerBordersStyle";
		public const string AppointmentHorizontalSeparator = "AppointmentHorizontalSeparatorStyle";
		public const string MoreButton = "MoreButtonStyle";
		public const string Toolbar = "ToolbarStyle";
		public const string ToolbarContainer = "ToolbarContainerStyle";
		public const string ToolTipSquaredCorners = "ToolTipSquaredCornersStyle";
		public const string ErrorInfo = "ErrorInfoStyle";
		public const string InplaceEditor = "InplaceEditorStyle";
		public const string TimeRulerHoursItem = "TimeRulerHoursItemStyle";
		public const string TimeRulerMinuteItem = "TimeRulerMinuteItemStyle";
		public const string TimeCellBody = "TimeCellBodyStyle";
		public const string DateCellHeader = "DateCellHeaderStyle";
		public const string TodayCellHeader = "TodayCellHeaderStyle";
		public const string DateCellBody = "DateCellBodyStyle";
		public const string SelectionBar = "SelectionBarStyle";
		public const string TimelineCellBody = "TimelineCellBodyStyle";
		public const string TimelineDateHeader = "TimelineDateHeaderStyle";
		public const string AlternateTimelineDateHeader = "AlternateTimelineDateHeaderStyle";
		public const string ControlAreaForm = "ControlAreaFormStyle";
		public const string SlaveRowTimeRulerHeaderHourItem = "TimeRulerHeaderHourItemStyle";
		public const string SlaveRowTimeRulerHeaderMinuteItem = "TimeRulerHeaderMinuteItemStyle";
		public const string SlaveRowScrollHeaderItem = "ScrollHeaderItemStyle";
		public const string ToolTipRoundedCornersTopBottomRow = "ToolTipRoundedCornersTopBottomRowStyle";
		public const string ToolTipRoundedCornersTopSide = "ToolTipRoundedCornersTopSideStyle";
		public const string ToolTipRoundedCornersLeftSide = "ToolTipRoundedCornersLeftSideStyle";
		public const string ToolTipRoundedCornersRightSide = "ToolTipRoundedCornersRightSideStyle";
		public const string ToolTipRoundedCornersBottomSide = "ToolTipRoundedCornersBottomSideStyle";
		public const string ToolTipRoundedCornersContent = "ToolTipRoundedCornersContent";
		public const string TimeMarker = "TimeMarkerStyle";
		public const string TimeMarkerLineHorizontal = "TimeMarkerLineHStyle";
		public const string TimeMarkerLineVertical = "TimeMarkerLineVStyle";
	}
	#endregion
	#region CellBodyStyle
	public class CellBodyStyle : AppearanceStyleBase {
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		AutoFormatEnable]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		public override void AssignToControl(WebControl control, AttributesRange range) {
			base.AssignToControl(control, range);
			control.Height = Height;
		}
	}
	#endregion
	#region SchedulerCellAppearanceStyle
	public class SchedulerCellAppearanceStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override DefaultBoolean Wrap { get { return DefaultBoolean.False; } set { } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
	}
	#endregion
	#region HeaderStyle
	public class HeaderStyle : SchedulerCellAppearanceStyle {
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatEnable]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override DefaultBoolean Wrap { get { return DefaultBoolean.Default; } set { } }
		public override void AssignToControl(WebControl control, AttributesRange range) {
			base.AssignToControl(control, range);
			control.Height = Height;
			Paddings.AssignToControl(control);
		}
	}
	#endregion
	#region VerticalResourceHeaderStyle
	public class VerticalResourceHeaderStyle : SchedulerCellAppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override DefaultBoolean Wrap { get { return DefaultBoolean.Default; } set { } }
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatEnable]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		public override void AssignToControl(WebControl control, AttributesRange range) {
			base.AssignToControl(control, range);
			control.Width = Width;
			Paddings.AssignToControl(control);
		}
	}
	#endregion
	#region AllDayAreaStyle
	public class AllDayAreaStyle : SchedulerCellAppearanceStyle {
	}
	#endregion
	#region LeftTopCornerStyle
	public class LeftTopCornerStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override DefaultBoolean Wrap { get { return base.Wrap; } set { base.Wrap = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		public override void AssignToControl(WebControl control, AttributesRange range) {
			base.AssignToControl(control, range);
			control.Height = Height;
			Paddings.AssignToControl(control);
		}
	}
	#endregion
	#region RightTopCornerStyle
	public class RightTopCornerStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override DefaultBoolean Wrap { get { return base.Wrap; } set { base.Wrap = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
	}
	#endregion
	#region TimeRulerItemStyleBase
	public class TimeRulerItemStyle : AppearanceStyle {
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatEnable]
		public override Unit Width {
			get { return base.Width; }
			set { base.Width = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override DefaultBoolean Wrap { get { return base.Wrap; } set { base.Wrap = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		public override void AssignToControl(WebControl control, AttributesRange range) {
			base.AssignToControl(control, range);
			control.Width = Width;
		}
	}
	#endregion
	#region CellHeaderStyle
	public class CellHeaderStyle : AppearanceStyleBase {
	}
	#endregion
	#region SelectionBarStyle
	public class SelectionBarStyle : SchedulerCellAppearanceStyle {
		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), AutoFormatEnable]
		public override Unit Height {
			get { return base.Height; }
			set { base.Height = value; }
		}
		public override void AssignToControl(WebControl control, AttributesRange range) {
			base.AssignToControl(control, range);
			control.Height = Height;
			Paddings.AssignToControl(control);
		}
	}
	#endregion
	#region GroupSeparatorStyle
	public class GroupSeparatorStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new DefaultBoolean Wrap { get { return base.Wrap; } set { base.Wrap = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
	}
	#endregion
	#region AppointmentInnerBordersStyle
	public class AppointmentInnerBordersStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override BackgroundImage BackgroundImage { get { return base.BackgroundImage; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override BorderWrapper Border { get { return base.Border; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Border BorderBottom { get { return base.BorderBottom; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Border BorderLeft { get { return base.BorderLeft; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Border BorderRight { get { return base.BorderRight; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Border BorderTop { get { return base.BorderTop; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override DefaultBoolean Wrap { get { return base.Wrap; } set { base.Wrap = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new FontInfo Font { get { return base.Font; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new string Cursor { get { return base.Cursor; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		public Border HorizontalBorder { get { return base.BorderTop; } }
		public Border VerticalBorder { get { return base.BorderRight; } }
	}
	#endregion
	#region AppointmentHorizontalSeparator
	public class AppointmentHorizontalSeparatorStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
	}
	#endregion
	#region AppointmentStyle
	public class AppointmentStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
	}
	#endregion
	#region AppointmentTimeTextStyle
	public class AppointmentTimeTextStyle : AppearanceStyleBase {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
	}
	#endregion
	#region SchedulerMenuStyles
	public class SchedulerMenuStyles : MenuStyles {
		public SchedulerMenuStyles(ASPxMenuBase menu)
			: base(menu) {
		}
		#region Hide unused
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new string CssFilePath { get { return base.CssFilePath; } }
		#endregion
	}
	#endregion
	#region SchedulerEditorStyles
	public class SchedulerEditorStyles : EditorStyles {
		public SchedulerEditorStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		#region Hide unused
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new string CssFilePath { get { return base.CssFilePath; } }
		#endregion
	}
	#endregion
	#region SchedulerButtonStyles
	public class SchedulerButtonStyles : ButtonControlStyles {
		public SchedulerButtonStyles(ISkinOwner skinOwner)
			: base(skinOwner) {
		}
		#region Hide unused
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override string CssPostfix { get { return base.CssPostfix; } set { base.CssPostfix = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new string CssFilePath { get { return base.CssFilePath; } }
		#endregion
	}
	#endregion
	#region MoreButtonStyle
	public class MoreButtonStyle : AppearanceStyle {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override AppearanceSelectedStyle HoverStyle { get { return base.HoverStyle; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Unit ImageSpacing { get { return base.ImageSpacing; } set { base.ImageSpacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override Unit Spacing { get { return base.Spacing; } set { base.Spacing = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public new DefaultBoolean Wrap { get { return base.Wrap; } set { base.Wrap = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override VerticalAlign VerticalAlign { get { return base.VerticalAlign; } set { base.VerticalAlign = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), AutoFormatDisable]
		public override HorizontalAlign HorizontalAlign { get { return base.HorizontalAlign; } set { base.HorizontalAlign = value; } }
		public override void AssignToControl(WebControl control, AttributesRange range) {
			base.AssignToControl(control, range);
			Paddings.AssignToControl(control);
		}
	}
	#endregion
	#region ASPxSchedulerStylesBase
	public class ASPxSchedulerStylesBase : StylesBase {
		public const string HorizontalResourceHeaderStyleName = "HorizontalResourceHeader";
		public const string VerticalResourceHeaderStyleName = "VerticalResourceHeader";
		public const string DayHeaderStyleName = "DayHeader";
		public const string DateHeaderStyleName = "DateHeader";
		public const string AlternateDateHeaderStyleName = "AlternateDateHeader";
		public const string GroupSeparatorHorizontalStyleName = "GroupSeparatorHorizontal";
		public const string GroupSeparatorVerticalStyleName = "GroupSeparatorHorizontal";
		public const string LeftTopCornerStyleName = "LeftTopCorner";
		public const string RightTopCornerStyleName = "RightTopCorner";
		public const string AppointmentStyleName = "Appointment";
		public const string AppointmentInnerBordersStyleName = "AppointmentInnerBorders";
		public const string MoreButtonStyleName = "MoreButton";
		public const string TimeMarkerLineStyleName = "TimeMarkerLine";		
		public ASPxSchedulerStylesBase(ASPxScheduler control)
			: base(control) {
		}
		#region Properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public HeaderStyle DateHeader {
			get { return (HeaderStyle)GetStyle(DateHeaderStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public HeaderStyle AlternateDateHeader {
			get { return (HeaderStyle)GetStyle(AlternateDateHeaderStyleName); }
		}
		[NotifyParentProperty(true), TypeConverter(typeof(WebColorConverter)),
		DefaultValue(typeof(Color), ""), AutoFormatEnable()]
		public Color SelectionColor {
			get { return GetColorProperty("SelectionColor", Color.Empty); }
			set { SetColorProperty("SelectionColor", Color.Empty, value); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public HeaderStyle DayHeader {
			get { return (HeaderStyle)GetStyle(DayHeaderStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public HeaderStyle HorizontalResourceHeader {
			get { return (HeaderStyle)GetStyle(HorizontalResourceHeaderStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public VerticalResourceHeaderStyle VerticalResourceHeader {
			get { return (VerticalResourceHeaderStyle)GetStyle(VerticalResourceHeaderStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public AppointmentStyle Appointment {
			get { return (AppointmentStyle)GetStyle(AppointmentStyleName); }
		}
		[Obsolete("This property is obsolete. Use appointment templates for customization", true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable, Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public AppointmentTimeTextStyle AppointmentTimeText {
			get { return new AppointmentTimeTextStyle(); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public AppointmentInnerBordersStyle AppointmentInnerBorders {
			get { return (AppointmentInnerBordersStyle)GetStyle(AppointmentInnerBordersStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public MoreButtonStyle MoreButton {
			get { return (MoreButtonStyle)GetStyle(MoreButtonStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public RightTopCornerStyle RightTopCorner {
			get { return (RightTopCornerStyle)GetStyle(RightTopCornerStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public LeftTopCornerStyle LeftTopCorner {
			get { return (LeftTopCornerStyle)GetStyle(LeftTopCornerStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public GroupSeparatorStyle GroupSeparatorHorizontal {
			get { return (GroupSeparatorStyle)GetStyle(GroupSeparatorHorizontalStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), AutoFormatEnable]
		public GroupSeparatorStyle GroupSeparatorVertical {
			get { return (GroupSeparatorStyle)GetStyle(GroupSeparatorVerticalStyleName); }
		}
		internal AppearanceStyle InnerTimeMarkerLine { get { return (AppearanceStyle)GetStyle(TimeMarkerLineStyleName); } }
		#endregion
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			ASPxSchedulerStylesBase styles = source as ASPxSchedulerStylesBase;
			if (styles == null)
				return;
			SelectionColor = styles.SelectionColor;
			DateHeader.CopyFrom(styles.DateHeader);
			AlternateDateHeader.CopyFrom(styles.AlternateDateHeader);
			DayHeader.CopyFrom(styles.DayHeader);
			HorizontalResourceHeader.CopyFrom(styles.HorizontalResourceHeader);
			VerticalResourceHeader.CopyFrom(styles.VerticalResourceHeader);
			Appointment.CopyFrom(styles.Appointment);
			AppointmentInnerBorders.CopyFrom(styles.AppointmentInnerBorders);
			MoreButton.CopyFrom(styles.MoreButton);
			RightTopCorner.CopyFrom(styles.RightTopCorner);
			LeftTopCorner.CopyFrom(styles.LeftTopCorner);
			GroupSeparatorHorizontal.CopyFrom(styles.GroupSeparatorHorizontal);
			GroupSeparatorVertical.CopyFrom(styles.GroupSeparatorVertical);
		}
		#region GetCssClassNamePrefix
		protected override string GetCssClassNamePrefix() {
			return "dxsc";
		}
		#endregion
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(DateHeaderStyleName, delegate() { return new HeaderStyle(); }));
			list.Add(new StyleInfo(AlternateDateHeaderStyleName, delegate() { return new HeaderStyle(); }));
			list.Add(new StyleInfo(DayHeaderStyleName, delegate() { return new HeaderStyle(); }));
			list.Add(new StyleInfo(HorizontalResourceHeaderStyleName, delegate() { return new HeaderStyle(); }));
			list.Add(new StyleInfo(VerticalResourceHeaderStyleName, delegate() { return new VerticalResourceHeaderStyle(); }));
			list.Add(new StyleInfo(AppointmentStyleName, delegate() { return new AppointmentStyle(); }));
			list.Add(new StyleInfo(AppointmentInnerBordersStyleName, delegate() { return new AppointmentInnerBordersStyle(); }));
			list.Add(new StyleInfo(MoreButtonStyleName, delegate() { return new MoreButtonStyle(); }));
			list.Add(new StyleInfo(RightTopCornerStyleName, delegate() { return new RightTopCornerStyle(); }));
			list.Add(new StyleInfo(LeftTopCornerStyleName, delegate() { return new LeftTopCornerStyle(); }));
			list.Add(new StyleInfo(GroupSeparatorHorizontalStyleName, delegate() { return new GroupSeparatorStyle(); }));
			list.Add(new StyleInfo(GroupSeparatorVerticalStyleName, delegate() { return new GroupSeparatorStyle(); }));
			list.Add(new StyleInfo(TimeMarkerLineStyleName, delegate() { return new AppearanceStyle(); }));
		}
		#region Default styles
		protected internal virtual AppearanceStyle GetDefaultTimeMarkerLineStyle() {
			return new AppearanceStyle();
		}
		protected new internal virtual AppearanceStyle GetDefaultControlStyle() {
			return base.GetDefaultControlStyle();
		}
		protected internal virtual HeaderStyle GetDefaultDateHeaderStyle() {
			HeaderStyle style = new HeaderStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.DateHeader));
			return style;
		}
		protected internal virtual HeaderStyle GetDefaultAlternateDateHeaderStyle() {
			HeaderStyle style = new HeaderStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.AlternateDateHeader));
			return style;
		}
		protected internal virtual HeaderStyle GetDefaultHorizontalResourceHeaderStyle() {
			HeaderStyle style = new HeaderStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.HorizontalResourceHeader));
			return style;
		}
		protected internal virtual HeaderStyle GetDefaultVerticalResourceHeaderStyle() {
			HeaderStyle style = new HeaderStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.VerticalResourceHeader));
			return style;
		}
		protected internal virtual HeaderStyle GetDefaultDayHeaderStyle() {
			HeaderStyle style = new HeaderStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.DayHeader));
			return style;
		}
		protected internal virtual RightTopCornerStyle GetDefaultRightTopCornerStyle() {
			RightTopCornerStyle style = new RightTopCornerStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.RightTopCorner));
			return style;
		}
		protected internal virtual LeftTopCornerStyle GetDefaultLeftTopCornerStyle() {
			LeftTopCornerStyle style = new LeftTopCornerStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.LeftTopCorner));
			return style;
		}
		protected internal virtual AppointmentStyle GetDefaultAppointmentStyle() {
			AppointmentStyle style = new AppointmentStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.Appointment));
			return style;
		}		
		protected internal virtual GroupSeparatorStyle GetDefaultGroupSeparatorVerticalStyle() {
			GroupSeparatorStyle style = new GroupSeparatorStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.GroupSeparatorVertical));
			return style;
		}
		protected internal virtual GroupSeparatorStyle GetDefaultGroupSeparatorHorizontalStyle() {
			GroupSeparatorStyle style = new GroupSeparatorStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.GroupSeparatorHorizontal));
			return style;
		}
		protected internal virtual AppointmentInnerBordersStyle GetDefaultAppointmentInnerBordersStyle() {
			AppointmentInnerBordersStyle style = new AppointmentInnerBordersStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.AppointmentInnerBorders));
			return style;
		}
		protected internal virtual MoreButtonStyle GetDefaultMoreButtonStyle() {
			MoreButtonStyle style = new MoreButtonStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.MoreButton));
			return style;
		}
		#endregion
	}
	#endregion
	#region ToolTipSquareCornerStyle
	public class ToolTipSquareCornerStyle : AppearanceStyle {
		public ToolTipSquareCornerStyle() {
		}
	}
	#endregion
	#region ToolTipRoundedCornersStyles
	public class ToolTipRoundedCornersStyles : StylesBase {
		public const string TopSideStyleName = "TopSide";
		public const string LeftSideStyleName = "LeftSide";
		public const string RightSideStyleName = "RightSide";
		public const string BottomSideStyleName = "BottomSide";
		public const string ContentStyleName = "Content";
		public ToolTipRoundedCornersStyles(ISkinOwner properties)
			: base(properties) {
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ToolTipSideStyle TopSide { get { return GetStyle(TopSideStyleName) as ToolTipSideStyle; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ToolTipSideStyle LeftSide { get { return GetStyle(LeftSideStyleName) as ToolTipSideStyle; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ToolTipSideStyle RightSide { get { return GetStyle(RightSideStyleName) as ToolTipSideStyle; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ToolTipSideStyle BottomSide { get { return GetStyle(BottomSideStyleName) as ToolTipSideStyle; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyle Content { get { return GetStyle(ContentStyleName) as AppearanceStyle; } }
		protected override string GetCssClassNamePrefix() {
			return "dxsc";
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(TopSideStyleName, delegate() { return new ToolTipSideStyle(); }));
			list.Add(new StyleInfo(LeftSideStyleName, delegate() { return new ToolTipSideStyle(); }));
			list.Add(new StyleInfo(RightSideStyleName, delegate() { return new ToolTipSideStyle(); }));
			list.Add(new StyleInfo(BottomSideStyleName, delegate() { return new ToolTipSideStyle(); }));
			list.Add(new StyleInfo(ContentStyleName, delegate() { return new AppearanceStyle(); }));
		}
		protected internal virtual ToolTipSideStyle GetTopSideStyle() {
			ToolTipSideStyle style = new ToolTipSideStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ToolTipRoundedCornersTopSide));
			style.CopyFrom(TopSide);
			return style;
		}
		protected internal virtual ToolTipSideStyle GetLeftSideStyle() {
			ToolTipSideStyle style = new ToolTipSideStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ToolTipRoundedCornersLeftSide));
			style.CopyFrom(LeftSide);
			return style;
		}
		protected internal virtual ToolTipSideStyle GetRightSideStyle() {
			ToolTipSideStyle style = new ToolTipSideStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ToolTipRoundedCornersRightSide));
			style.CopyFrom(RightSide);
			return style;
		}
		protected internal virtual ToolTipSideStyle GetBottomSideStyle() {
			ToolTipSideStyle style = new ToolTipSideStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ToolTipRoundedCornersBottomSide));
			style.CopyFrom(BottomSide);
			return style;
		}
		protected internal virtual AppearanceStyle GetContentStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ToolTipRoundedCornersContent));
			style.CopyFrom(Content);
			return style;
		}
	}
	#endregion
	#region TooltipSideStyle
	public class ToolTipSideStyle : AppearanceStyle {
	}
	#endregion
	#region ASPxSchedulerStyles
	public class ASPxSchedulerStyles : ASPxSchedulerStylesBase {
		public const string NavigationButtonStyleName = "NavigationButton";
		public const string ToolbarStyleName = "Toolbar";
		public const string ErrorInformationStyleName = "ErrorInformation";
		public const string ViewVisibleIntervalStyleName = "ViewVisibleInterval";
		public const string ToolTipSquaredCornersStyleName = "ToolTipSquaredCorners";
		public const string InplaceEditorStyleName = "InplaceEditor";
		public const string FormButtonStyleName = "FormButton";
		#region Fields
		ResourceNavigatorStyles resourceNavigator;
		ViewSelectorStyles viewSelector;
		ViewNavigatorStyles viewNavigator;
		PopupFormStyles popupForm;
		SchedulerMenuStyles menu;
		SchedulerEditorStyles formEditors;
		SchedulerButtonStyles buttons;
		ToolTipRoundedCornersStyles toolTipRoundedCornersStyles;
		#endregion
		public ASPxSchedulerStyles(ASPxScheduler control)
			: base(control) {
			this.resourceNavigator = new ResourceNavigatorStyles(control);
			this.viewSelector = new ViewSelectorStyles(control);
			this.viewNavigator = new ViewNavigatorStyles(control);
			this.popupForm = new PopupFormStyles(control);
			this.menu = new SchedulerMenuStyles(null);
			this.formEditors = new SchedulerEditorStyles(control);
			this.buttons = new SchedulerButtonStyles(control);
			this.toolTipRoundedCornersStyles = new ToolTipRoundedCornersStyles(control);
		}
		#region Properties
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ResourceNavigatorStyles ResourceNavigator { get { return resourceNavigator; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ViewSelectorStyles ViewSelector { get { return viewSelector; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ViewNavigatorStyles ViewNavigator { get { return viewNavigator; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public PopupFormStyles PopupForm { get { return popupForm; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public SchedulerMenuStyles Menu { get { return menu; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public SchedulerEditorStyles FormEditors { get { return formEditors; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public SchedulerButtonStyles Buttons { get { return buttons; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ToolTipRoundedCornersStyles ToolTipRoundedCorners { get { return toolTipRoundedCornersStyles; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ToolTipSquareCornerStyle ToolTipSquaredCorners {
			get { return (ToolTipSquareCornerStyle)GetStyle(ToolTipSquaredCornersStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ButtonControlStyle NavigationButton {
			get { return (ButtonControlStyle)GetStyle(NavigationButtonStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyle Toolbar {
			get { return (AppearanceStyle)GetStyle(ToolbarStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyle ErrorInformation {
			get { return (AppearanceStyle)GetStyle(ErrorInformationStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyle ViewVisibleInterval {
			get { return (AppearanceStyle)GetStyle(ViewVisibleIntervalStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public AppearanceStyle InplaceEditor {
			get { return (AppearanceStyle)GetStyle(InplaceEditorStyleName); }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public LoadingPanelStyle LoadingPanel {
			get { return base.LoadingPanelInternal; }
		}
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), AutoFormatEnable]
		public LoadingDivStyle LoadingDiv {
			get { return base.LoadingDivInternal; }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), AutoFormatEnable]
		public ButtonControlStyle FormButton {
			get { return (ButtonControlStyle)GetStyle(FormButtonStyleName); }
		}
		#endregion
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			ASPxSchedulerStyles styles = source as ASPxSchedulerStyles;
			if (styles == null)
				return;
			ResourceNavigator.CopyFrom(styles.ResourceNavigator);
			ViewSelector.CopyFrom(styles.ViewSelector);
			ViewNavigator.CopyFrom(styles.ViewNavigator);
			PopupForm.CopyFrom(styles.PopupForm);
			Menu.CopyFrom(styles.Menu);
			FormEditors.CopyFrom(styles.FormEditors);
			Buttons.CopyFrom(styles.Buttons);
			ToolTipRoundedCorners.CopyFrom(styles.ToolTipRoundedCorners);
			ToolTipSquaredCorners.CopyFrom(styles.ToolTipSquaredCorners);
			NavigationButton.CopyFrom(styles.NavigationButton);
			Toolbar.CopyFrom(styles.Toolbar);
			ErrorInformation.CopyFrom(styles.ErrorInformation);
			ViewVisibleInterval.CopyFrom(styles.ViewVisibleInterval);
			InplaceEditor.CopyFrom(styles.InplaceEditor);
			LoadingPanel.CopyFrom(styles.LoadingPanel);
			LoadingDiv.CopyFrom(styles.LoadingDiv);
			FormButton.CopyFrom(styles.FormButton);
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(NavigationButtonStyleName, delegate() { return new ButtonControlStyle(); }));
			list.Add(new StyleInfo(ToolbarStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(ErrorInformationStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(ViewVisibleIntervalStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(ToolTipSquaredCornersStyleName, delegate() { return new ToolTipSquareCornerStyle(); }));
			list.Add(new StyleInfo(InplaceEditorStyleName, delegate() { return new AppearanceStyle(); }));
			list.Add(new StyleInfo(FormButtonStyleName, delegate() { return new ButtonControlStyle(); }));
		}
		protected internal virtual ButtonControlStyle GetNavigationButtonStyle() {
			ButtonControlStyle style = new ButtonControlStyle();
			style.CopyFrom(NavigationButton);
			return style;
		}
		protected internal virtual AppearanceStyle GetToolbarStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.Toolbar));
			style.CopyFrom(Toolbar);
			return style;
		}
		protected internal virtual AppearanceStyle GetToolbarContainerStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ToolbarContainer));
			return style;
		}
		protected internal virtual ToolTipSquareCornerStyle GetToolTipSquaredCornersStyle() {
			ToolTipSquareCornerStyle style = new ToolTipSquareCornerStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ToolTipSquaredCorners));
			style.CopyFrom(ToolTipSquaredCorners);
			return style;
		}
		protected internal virtual AppearanceStyle GetErrorInformationStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ErrorInfo));
			style.CopyFrom(ErrorInformation);
			return style;
		}
		protected internal virtual AppearanceStyle GetViewVisibleIntervalStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ViewVisibleInterval));
			style.CopyFrom(ViewVisibleInterval);
			return style;
		}
		protected internal virtual AppearanceStyle GetInplaceEditorStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.InplaceEditor));
			style.CopyFrom(InplaceEditor);
			return style;
		}
		protected internal virtual AppearanceStyle GetControlAreaFormControlStyle() {
			AppearanceStyle style = PopupForm.Content;
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.ControlAreaForm));
			style.CopyFrom(InplaceEditor);
			return style;
		}
		#region ViewState
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { ResourceNavigator, ViewSelector, ViewNavigator, PopupForm, Menu, FormEditors, Buttons, ToolTipRoundedCorners });
		}
		#endregion
	}
	#endregion
	#region DayViewStyles
	public class DayViewStyles : ASPxSchedulerStylesBase {
		public const string TimeCellBodyStyleName = "TimeCellBody";
		public const string AllDayAreaStyleName = "AllDayArea";
		public const string TimeRulerHoursItemStyleName = "TimeRulerHoursItem";
		public const string TimeRulerMinuteItemStyleName = "TimeRulerMinuteItem";
		public const string TopMoreButtonStyleName = "TopMoreButton";
		public const string BottomMoreButtonStyleName = "BottomMoreButton";
		public const string AppointmentHorizontalSeparatorStyleName = "AppointmentHorizontalSeparator";
		public const string TimeMarkerStyleName = "TimeMarker";
		public DayViewStyles(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		#region Properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CellBodyStyle TimeCellBody {
			get { return (CellBodyStyle)GetStyle(TimeCellBodyStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public AllDayAreaStyle AllDayArea {
			get { return (AllDayAreaStyle)GetStyle(AllDayAreaStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TimeRulerItemStyle TimeRulerHoursItem {
			get { return (TimeRulerItemStyle)GetStyle(TimeRulerHoursItemStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public TimeRulerItemStyle TimeRulerMinuteItem {
			get { return (TimeRulerItemStyle)GetStyle(TimeRulerMinuteItemStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public AppointmentHorizontalSeparatorStyle AppointmentHorizontalSeparator {
			get { return (AppointmentHorizontalSeparatorStyle)GetStyle(AppointmentHorizontalSeparatorStyleName); }
		}
		[NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable()]
		public Unit ScrollAreaHeight {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "ScrollAreaHeight", Unit.Empty); }
			set { ViewStateUtils.SetUnitProperty(ViewState, "ScrollAreaHeight", Unit.Empty, value); }
		}
		[NotifyParentProperty(true), DefaultValue(typeof(Unit), ""), AutoFormatEnable()]
		public Unit AllDayAreaHeight {
			get { return ViewStateUtils.GetUnitProperty(ReadOnlyViewState, "AllDayAreaHeight", Unit.Empty); }
			set { ViewStateUtils.SetUnitProperty(ViewState, "AllDayAreaHeight", Unit.Empty, value); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ButtonControlStyle TopMoreButton {
			get { return (ButtonControlStyle)GetStyle(TopMoreButtonStyleName); }
		}
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public ButtonControlStyle BottomMoreButton {
			get { return (ButtonControlStyle)GetStyle(BottomMoreButtonStyleName); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new HeaderStyle DayHeader { get { return base.DayHeader; } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public AppearanceStyle TimeMarker { get { return (AppearanceStyle)GetStyle(TimeMarkerStyleName); } }
		[NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty)]
		public AppearanceStyle TimeMarkerLine { get { return InnerTimeMarkerLine; } }
		#endregion
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			DayViewStyles dayViewStyles = source as DayViewStyles;
			if (dayViewStyles != null) {
				AllDayArea.CopyFrom(dayViewStyles.AllDayArea);
				AppointmentHorizontalSeparator.CopyFrom(dayViewStyles.AppointmentHorizontalSeparator);
				BottomMoreButton.CopyFrom(dayViewStyles.BottomMoreButton);
				DayHeader.CopyFrom(dayViewStyles.DayHeader);
				TimeCellBody.CopyFrom(dayViewStyles.TimeCellBody);
				TimeMarker.CopyFrom(dayViewStyles.TimeMarker);
				TimeMarkerLine.CopyFrom(dayViewStyles.TimeMarkerLine);
				TimeRulerHoursItem.CopyFrom(dayViewStyles.TimeRulerHoursItem);
				TimeRulerMinuteItem.CopyFrom(dayViewStyles.TimeRulerMinuteItem);
				TopMoreButton.CopyFrom(dayViewStyles.TopMoreButton);
				VerticalResourceHeader.CopyFrom(dayViewStyles.VerticalResourceHeader);
				ScrollAreaHeight = dayViewStyles.ScrollAreaHeight;
				AllDayAreaHeight = dayViewStyles.AllDayAreaHeight;
			}
		}
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(TimeCellBodyStyleName, delegate() { return new CellBodyStyle(); }));
			list.Add(new StyleInfo(AllDayAreaStyleName, delegate() { return new AllDayAreaStyle(); }));
			list.Add(new StyleInfo(TimeRulerHoursItemStyleName, delegate() { return new TimeRulerItemStyle(); }));
			list.Add(new StyleInfo(TimeRulerMinuteItemStyleName, delegate() { return new TimeRulerItemStyle(); }));
			list.Add(new StyleInfo(TopMoreButtonStyleName, delegate() { return new ButtonControlStyle(); }));
			list.Add(new StyleInfo(BottomMoreButtonStyleName, delegate() { return new ButtonControlStyle(); }));
			list.Add(new StyleInfo(AppointmentHorizontalSeparatorStyleName, delegate() { return new AppointmentHorizontalSeparatorStyle(); }));
			list.Add(new StyleInfo(TimeMarkerStyleName, delegate() { return new AppearanceStyle(); }));
		}
		protected internal virtual ButtonControlStyle GetTopMoreButtonStyle() {
			ButtonControlStyle style = new ButtonControlStyle();
			style.CopyFrom(TopMoreButton);
			return style;
		}
		protected internal virtual ButtonControlStyle GetBottomMoreButtonStyle() {
			ButtonControlStyle style = new ButtonControlStyle();
			style.CopyFrom(BottomMoreButton);
			return style;
		}
		#region Default styles
		protected internal override AppearanceStyle GetDefaultTimeMarkerLineStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.TimeMarkerLineHorizontal));
			return style;
		}
		protected internal virtual AppearanceStyle GetDefaultTimeMarkerStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.TimeMarker));
			return style;
		}
		protected internal virtual TimeRulerItemStyle GetDefaultTimeRulerHoursItemStyle() {
			TimeRulerItemStyle style = new TimeRulerItemStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.TimeRulerHoursItem));
			return style;
		}
		protected internal virtual TimeRulerItemStyle GetDefaultTimeRulerMinuteItemStyle() {
			TimeRulerItemStyle style = new TimeRulerItemStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.TimeRulerMinuteItem));
			return style;
		}
		protected internal virtual AllDayAreaStyle GetDefaultAllDayAreaStyle() {
			AllDayAreaStyle style = new AllDayAreaStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.AllDayArea));
			return style;
		}
		protected internal virtual AllDayAreaStyle GetDefaultAllDayAreaLeftStyle() {
			AllDayAreaStyle style = new AllDayAreaStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.AllDayAreaLeft));
			return style;
		}
		protected internal virtual CellBodyStyle GetDefaultTimeCellBodyStyle() {
			CellBodyStyle style = new CellBodyStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.TimeCellBody));
			return style;
		}
		protected internal virtual AppointmentHorizontalSeparatorStyle GetDefaultAppointmentHorizontalSeparatorStyle() {
			AppointmentHorizontalSeparatorStyle style = new AppointmentHorizontalSeparatorStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.AppointmentHorizontalSeparator));
			return style;
		}
		#endregion
		#region SlaveRow
		protected internal AppearanceStyleBase GetSlaveRowTimeRulerHeaderHourItemStyle() {
			return CreateStyleByName(ASPxSchedulerStyleNames.SlaveRowTimeRulerHeaderHourItem);
		}
		protected internal AppearanceStyleBase GetSlaveRowTimeRulerHeaderMinuteItemStyle() {
			return CreateStyleByName(ASPxSchedulerStyleNames.SlaveRowTimeRulerHeaderMinuteItem);
		}
		protected internal AppearanceStyleBase GetSlaveRowScrollItemStyle() {
			return CreateStyleByName(ASPxSchedulerStyleNames.SlaveRowScrollHeaderItem);
		}
		#endregion
	}
	#endregion
	#region WeekViewStyles
	public class WeekViewStyles : ASPxSchedulerStylesBase {
		public const string DateCellHeaderStyleName = "DateCellHeader";
		public const string TodayCellHeaderStyleName = "TodayCellHeader";
		public const string DateCellBodyStyleName = "DateCellBody";
		public WeekViewStyles(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		#region Properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CellHeaderStyle DateCellHeader {
			get { return (CellHeaderStyle)GetStyle(DateCellHeaderStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CellHeaderStyle TodayCellHeader {
			get { return (CellHeaderStyle)GetStyle(TodayCellHeaderStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CellBodyStyle DateCellBody {
			get { return (CellBodyStyle)GetStyle(DateCellBodyStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new HeaderStyle DateHeader { get { return base.DateHeader; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new HeaderStyle AlternateDateHeader { get { return base.AlternateDateHeader; } }
		#endregion
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(DateCellHeaderStyleName, delegate() { return new CellHeaderStyle(); }));
			list.Add(new StyleInfo(TodayCellHeaderStyleName, delegate() { return new CellHeaderStyle(); }));
			list.Add(new StyleInfo(DateCellBodyStyleName, delegate() { return new CellBodyStyle(); }));
		}
		#region Default styles
		public virtual HeaderStyle GetDefaultDateCellHeaderStyle() {
			HeaderStyle style = new HeaderStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.DateCellHeader));
			return style;
		}
		public virtual HeaderStyle GetDefaultTodayCellHeaderStyle() {
			HeaderStyle style = new HeaderStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.TodayCellHeader));
			return style;
		}
		public virtual CellBodyStyle GetDefaultDateCellBodyStyle() {
			CellBodyStyle style = new CellBodyStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.DateCellBody));
			return style;
		}
		#endregion
	}
	#endregion
	#region MonthViewStyles
	public class MonthViewStyles : WeekViewStyles {
		public MonthViewStyles(ASPxScheduler scheduler)
			: base(scheduler) {
		}
	}
	#endregion
	#region WorkWeekViewStyles
	public class WorkWeekViewStyles : DayViewStyles {
		public WorkWeekViewStyles(ASPxScheduler scheduler)
			: base(scheduler) {
		}
	}
	#endregion
	#region FullWeekViewStyles
	public class FullWeekViewStyles : WorkWeekViewStyles {
		public FullWeekViewStyles(ASPxScheduler scheduler)
			: base(scheduler) {
		}
	}
	#endregion
	#region TimelineViewStyles
	public class TimelineViewStyles : ASPxSchedulerStylesBase {
		public const string SelectionBarStyleName = "SelectionBar";
		public const string TimelineCellBodyStyleName = "TimelineCellBody";
		public const string TimelineDateHeaderStyleName = "TimelineDateHeader";
		public const string AlternateTimelineDateHeaderStyleName = "AlternateTimelineDateHeader";
		public TimelineViewStyles(ASPxScheduler scheduler)
			: base(scheduler) {
		}
		#region Properties
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public SelectionBarStyle SelectionBar {
			get { return (SelectionBarStyle)GetStyle(SelectionBarStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public CellBodyStyle TimelineCellBody {
			get { return (CellBodyStyle)GetStyle(TimelineCellBodyStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeaderStyle TimelineDateHeader {
			get { return (HeaderStyle)GetStyle(TimelineDateHeaderStyleName); }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true)]
		public HeaderStyle AlternateTimelineDateHeader {
			get { return (HeaderStyle)GetStyle(AlternateTimelineDateHeaderStyleName); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new HeaderStyle DayHeader { get { return base.DayHeader; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new HeaderStyle DateHeader { get { return base.DateHeader; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new HeaderStyle AlternateDateHeader { get { return base.AlternateDateHeader; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new HeaderStyle HorizontalResourceHeader { get { return base.HorizontalResourceHeader; } }
		public AppearanceStyle TimeMarkerLine { get { return base.InnerTimeMarkerLine; } }
		#endregion
		protected override void PopulateStyleInfoList(List<StyleInfo> list) {
			base.PopulateStyleInfoList(list);
			list.Add(new StyleInfo(SelectionBarStyleName, delegate() { return new SelectionBarStyle(); }));
			list.Add(new StyleInfo(TimelineCellBodyStyleName, delegate() { return new CellBodyStyle(); }));
			list.Add(new StyleInfo(TimelineDateHeaderStyleName, delegate() { return new HeaderStyle(); }));
			list.Add(new StyleInfo(AlternateTimelineDateHeaderStyleName, delegate() { return new HeaderStyle(); }));
		}
		#region DefaultStyles
		protected internal override AppearanceStyle GetDefaultTimeMarkerLineStyle() {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.TimeMarkerLineVertical));
			return style;
		}
		protected internal virtual SelectionBarStyle GetDefaultSelectionBarStyle() {
			SelectionBarStyle style = new SelectionBarStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.SelectionBar));
			return style;
		}
		protected internal virtual CellBodyStyle GetDefaultTimelineCellBodyStyle() {
			CellBodyStyle style = new CellBodyStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.TimelineCellBody));
			return style;
		}
		protected internal virtual HeaderStyle GetDefaultTimelineDateHeaderStyle() {
			HeaderStyle style = new HeaderStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.TimelineDateHeader));
			return style;
		}
		protected internal virtual HeaderStyle GetDefaultAlternateTimelineDateHeader() {
			HeaderStyle style = new HeaderStyle();
			style.CopyFrom(CreateStyleByName(ASPxSchedulerStyleNames.AlternateTimelineDateHeader));
			return style;
		}
		#endregion
	}
	#endregion
	#region WebElementType
	public enum WebElementType {
		None,
		DateHeader,
		HorizontalResourceHeader,
		VerticalResourceHeader,
		DayHeader,
		RightTopCorner,
		LeftTopCorner,
		GroupSeparatorHorizontal,
		GroupSeparatorVertical,
		TimeCellBody,
		AllDayArea,
		TimeRulerMinute,
		TimeRulerHours,
		DateCellHeader,
		DateCellBody,
		SelectionBar,
		TimelineCellBody,
		TimelineDateHeader,
		LabelWebControl,
		TimeRulerHeader
	}
	#endregion
}
namespace DevExpress.Web.ASPxScheduler.Internal {
	#region StylesHelper
	public abstract class StylesHelper {
		#region Create
		public static StylesHelper Create(SchedulerViewBase view, ISchedulerWebViewInfoBase viewInfo, ASPxSchedulerStylesBase controlStyles) {
			switch (view.Type) {
				case SchedulerViewType.Day:
					return new DayViewStylesHelper(view, viewInfo, controlStyles);
				case SchedulerViewType.Week:
					return new WeekViewStylesHelper(view, viewInfo, controlStyles);
				case SchedulerViewType.Month:
					return new MonthViewStylesHelper(view, viewInfo, controlStyles);
				case SchedulerViewType.WorkWeek:
					return new WorkWeekViewStylesHelper(view, viewInfo, controlStyles);
				case SchedulerViewType.Timeline:
					return new TimelineViewStylesHelper(view, viewInfo, controlStyles);
				case SchedulerViewType.FullWeek:
					return new FullWeekViewStylesHelper(view, viewInfo, controlStyles);
			}
			return null;
		}
		#endregion
		#region GetControlStyle
		public static AppearanceStyle GetControlStyle(ASPxScheduler control) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(control.Styles.GetDefaultControlStyle());
			style.CopyFrom(control.ControlStyle);
			return style;
		}
		#endregion
		#region Fields
		SchedulerViewBase view;
		ASPxSchedulerStylesBase controlStyles;
		ISchedulerWebViewInfoBase viewInfo;
		SchedulerColorSchemaCollection resourceColorSchemas;
		#endregion
		protected StylesHelper(SchedulerViewBase view, ISchedulerWebViewInfoBase viewInfo, ASPxSchedulerStylesBase controlStyles) {
			if (view == null)
				Exceptions.ThrowArgumentNullException("view");
			if (viewInfo == null)
				Exceptions.ThrowArgumentNullException("viewInfo");
			if (controlStyles == null)
				Exceptions.ThrowArgumentNullException("controlStyles");
			this.view = view;
			this.viewInfo = viewInfo;
			this.controlStyles = controlStyles;
			this.resourceColorSchemas = (SchedulerColorSchemaCollection)View.Control.InnerControl.ResourceColorSchemas;
		}
		#region Properties
		protected internal ASPxSchedulerStylesBase ControlStyles { get { return controlStyles; } }
		protected internal SchedulerViewBase View { get { return view; } }
		protected internal ISchedulerWebViewInfoBase ViewInfo { get { return viewInfo; } }
		protected internal SchedulerColorSchemaCollection ResourceColorSchemas { get { return resourceColorSchemas; } }
		protected internal abstract ASPxSchedulerStylesBase InnerStyles { get; }
		#endregion
		#region GetStyle
		public virtual AppearanceStyleBase GetStyle(WebElementType type, ITimeCell timeCell, bool isAlternate) {
			AppearanceStyleBase result = null;
			switch (type) {
				case WebElementType.DateHeader:
					result = GetDateHeaderStyle(timeCell, isAlternate);
					break;
				case WebElementType.HorizontalResourceHeader:
					result = GetHorizontalResourceHeaderStyle(timeCell);
					break;
				case WebElementType.VerticalResourceHeader:
					result = GetVerticalResourceHeaderStyle(timeCell);
					break;
				case WebElementType.DayHeader:
					result = GetDayHeaderStyle(timeCell);
					break;
				case WebElementType.GroupSeparatorHorizontal:
					result = GetGroupSeparatorHorizontalStyle();
					break;
				case WebElementType.GroupSeparatorVertical:
					result = GetGroupSeparatorVerticalStyle();
					break;
				case WebElementType.LeftTopCorner:
					result = GetLeftTopCornerStyle();
					break;
			}
			if (result == null)
				result = new AppearanceStyle();
			if (View != null && View.Control != null)
				View.Control.RaiseCustomizeElementStyle(result, type, timeCell, isAlternate);
			return result;
		}
		#endregion
		#region GetAppointmentStyle
		public virtual AppointmentStyle GetAppointmentStyle() {
			AppointmentStyle style = CreateAppointmentStyle();
			style.CopyFrom(controlStyles.GetDefaultAppointmentStyle());
			style.CopyFrom(controlStyles.Appointment);
			style.CopyFrom(View.InnerStyles.Appointment);
			return style;
		}
		protected internal virtual AppointmentStyle CreateAppointmentStyle() {
			return new AppointmentStyle();
		}
		#endregion
		public virtual AppointmentHorizontalSeparatorStyle GetAppointmentHorizontalSeparatorStyle() {
			return new AppointmentHorizontalSeparatorStyle();
		}
		#region GetAppointmentInnerBordersStyle
		public virtual AppointmentInnerBordersStyle GetAppointmentInnerBordersStyle() {
			AppointmentInnerBordersStyle style = CreateAppointmentInnerBordersStyle();
			style.CopyFrom(controlStyles.GetDefaultAppointmentInnerBordersStyle());
			style.CopyFrom(controlStyles.AppointmentInnerBorders);
			style.CopyFrom(View.InnerStyles.AppointmentInnerBorders);
			return style;
		}
		protected internal virtual AppointmentInnerBordersStyle CreateAppointmentInnerBordersStyle() {
			return new AppointmentInnerBordersStyle();
		}
		#endregion
		#region GetMoreButtonTextStyle
		public virtual MoreButtonStyle GetMoreButtonStyle() {
			MoreButtonStyle style = CreateMoreButtonStyle();
			style.CopyFrom(controlStyles.GetDefaultMoreButtonStyle());
			style.CopyFrom(controlStyles.MoreButton);
			style.CopyFrom(View.InnerStyles.MoreButton);
			return style;
		}
		protected virtual MoreButtonStyle CreateMoreButtonStyle() {
			return new MoreButtonStyle();
		}
		#endregion
		#region GetDateHeaderStyle
		public virtual HeaderStyle GetDateHeaderStyle(ITimeCell timeCell, bool isAlternateHeaderCaption) {
			HeaderStyle style = CreateHeaderStyle();
			ASPxSchedulerStylesBase innerViewStyles = View.InnerStyles;
			style.CopyFrom(ControlStyles.GetDefaultDateHeaderStyle());
			if (isAlternateHeaderCaption)
				style.CopyFrom(ControlStyles.GetDefaultAlternateDateHeaderStyle());
			style.CopyFrom(ControlStyles.DateHeader);
			if (isAlternateHeaderCaption)
				style.CopyFrom(ControlStyles.AlternateDateHeader);
			style.CopyFrom(innerViewStyles.DateHeader);
			if (isAlternateHeaderCaption)
				style.CopyFrom(innerViewStyles.AlternateDateHeader);
			return style;
		}
		protected internal virtual HeaderStyle CreateHeaderStyle() {
			return new HeaderStyle();
		}
		#endregion
		#region GetHorizontalResourceHeaderStyle
		public virtual HeaderStyle GetHorizontalResourceHeaderStyle(ITimeCell timeCell) {
			HeaderStyle style = CreateHeaderStyle();
			ASPxSchedulerStylesBase innerViewStyles = View.InnerStyles;
			style.CopyFrom(ControlStyles.GetDefaultHorizontalResourceHeaderStyle());
			style.CopyFrom(ControlStyles.HorizontalResourceHeader);
			style.CopyFrom(innerViewStyles.HorizontalResourceHeader);
			return style;
		}
		#endregion
		#region GetVerticalResourceHeaderStyle
		public virtual VerticalResourceHeaderStyle GetVerticalResourceHeaderStyle(ITimeCell timeCell) {
			VerticalResourceHeaderStyle style = CreateVerticalResourceHeaderStyle();
			ASPxSchedulerStylesBase innerViewStyles = View.InnerStyles;
			style.CopyFrom(ControlStyles.GetDefaultVerticalResourceHeaderStyle());
			style.CopyFrom(ControlStyles.VerticalResourceHeader);
			style.CopyFrom(innerViewStyles.VerticalResourceHeader);
			return style;
		}
		protected internal virtual VerticalResourceHeaderStyle CreateVerticalResourceHeaderStyle() {
			return new VerticalResourceHeaderStyle();
		}
		#endregion
		#region GetDayHeaderStyle
		public virtual HeaderStyle GetDayHeaderStyle(ITimeCell timeCell) {
			HeaderStyle style = CreateHeaderStyle();
			ASPxSchedulerStylesBase innerViewStyles = View.InnerStyles;
			style.CopyFrom(ControlStyles.GetDefaultDayHeaderStyle());
			style.CopyFrom(ControlStyles.DayHeader);
			style.CopyFrom(innerViewStyles.DayHeader);
			return style;
		}
		#endregion
		#region GetSelectionColor
		public virtual Color GetSelectionColor() {
			ASPxSchedulerStylesBase innerViewStyles = View.InnerStyles;
			Color viewSelectionColor = innerViewStyles.SelectionColor;
			if (!viewSelectionColor.IsEmpty)
				return viewSelectionColor;
			if (!ControlStyles.SelectionColor.IsEmpty)
				return ControlStyles.SelectionColor;
			return Color.Empty;
		}
		#endregion
		#region GetGroupSeparatorHorizontalStyle
		public virtual GroupSeparatorStyle GetGroupSeparatorHorizontalStyle() {
			GroupSeparatorStyle style = CreateGroupSeparatorStyle();
			ASPxSchedulerStylesBase viewInnerStyles = View.InnerStyles;
			style.CopyFrom(controlStyles.GetDefaultGroupSeparatorHorizontalStyle());
			style.CopyFrom(controlStyles.GroupSeparatorHorizontal);
			style.CopyFrom(viewInnerStyles.GroupSeparatorHorizontal);
			return style;
		}
		protected virtual GroupSeparatorStyle CreateGroupSeparatorStyle() {
			return new GroupSeparatorStyle();
		}
		#endregion
		#region GetGroupSeparatorVerticalStyle
		public virtual GroupSeparatorStyle GetGroupSeparatorVerticalStyle() {
			GroupSeparatorStyle style = CreateGroupSeparatorStyle();
			ASPxSchedulerStylesBase viewInnerStyles = View.InnerStyles;
			style.CopyFrom(controlStyles.GetDefaultGroupSeparatorVerticalStyle());
			style.CopyFrom(controlStyles.GroupSeparatorVertical);
			style.CopyFrom(viewInnerStyles.GroupSeparatorVertical);
			return style;
		}
		#endregion
		#region GetRightTopCornerStyle
		public virtual RightTopCornerStyle GetRightTopCornerStyle() {
			RightTopCornerStyle style = CreateRightTopCornerStyle();
			ASPxSchedulerStylesBase viewInnerStyles = View.InnerStyles;
			style.CopyFrom(ControlStyles.GetDefaultRightTopCornerStyle());
			style.CopyFrom(ControlStyles.RightTopCorner);
			style.CopyFrom(viewInnerStyles.RightTopCorner);
			return style;
		}
		protected internal virtual RightTopCornerStyle CreateRightTopCornerStyle() {
			return new RightTopCornerStyle();
		}
		#endregion
		#region GetLeftTopCornerStyle
		public virtual LeftTopCornerStyle GetLeftTopCornerStyle() {
			LeftTopCornerStyle style = CreateLeftTopCornerStyle();
			ASPxSchedulerStylesBase viewInnerStyles = View.InnerStyles;
			style.CopyFrom(ControlStyles.GetDefaultLeftTopCornerStyle());
			style.CopyFrom(ControlStyles.LeftTopCorner);
			style.CopyFrom(viewInnerStyles.LeftTopCorner);
			return style;
		}
		protected internal virtual LeftTopCornerStyle CreateLeftTopCornerStyle() {
			return new LeftTopCornerStyle();
		}
		#endregion
		#region GetTimeMarkerLineStyle
		public virtual AppearanceStyle GetTimeMarkerLineStyle() {
			AppearanceStyle style = CreateTimeMarkerStyle();
			style.CopyFrom(InnerStyles.GetDefaultTimeMarkerLineStyle());
			style.CopyFrom(InnerStyles.InnerTimeMarkerLine);
			return style;
		}
		protected virtual AppearanceStyle CreateTimeMarkerStyle() {
			return new AppearanceStyle();
		}
		#endregion
		#region CreateAppearanceStyle
		protected internal virtual AppearanceStyle CreateAppearanceStyle() {
			return new AppearanceStyle();
		}
		protected internal virtual AppearanceStyleBase CreateAppearanceStyleBase() {
			return new AppearanceStyleBase();
		}
		#endregion
	}
	#endregion
	#region DayViewStylesHelper
	public class DayViewStylesHelper : StylesHelper {
		public DayViewStylesHelper(SchedulerViewBase view, ISchedulerWebViewInfoBase viewInfo, ASPxSchedulerStylesBase controlStyles)
			: base(view, viewInfo, controlStyles) {
		}
		#region Properties
		internal new DayView View { get { return (DayView)base.View; } }
		internal DayViewStyles Styles { get { return (DayViewStyles)InnerStyles; } }
		protected internal override ASPxSchedulerStylesBase InnerStyles { get { return View.Styles; } }
		#endregion
		#region GetStyle
		public override AppearanceStyleBase GetStyle(WebElementType type, ITimeCell timeCell, bool isAlternate) {
			AppearanceStyleBase resultStyle = null;
			switch (type) {
				case WebElementType.TimeCellBody:
					resultStyle = GetCellBodyStyle(timeCell);
					break;
				case WebElementType.AllDayArea:
					resultStyle = GetAllDayAreaStyle();
					break;
				case WebElementType.TimeRulerHours:
					resultStyle = GetTimeRulerHoursItemStyle();
					break;
				case WebElementType.TimeRulerMinute:
					resultStyle = GetTimeRulerMinuteItemStyle();
					break;
				case WebElementType.TimeRulerHeader:
					resultStyle = GetLeftTopCornerStyle();
					break;
			}
			if (resultStyle != null) {
				if (View != null && View.Control != null)
					View.Control.RaiseCustomizeElementStyle(resultStyle, type, timeCell, isAlternate);
				return resultStyle;
			}
			return base.GetStyle(type, timeCell, isAlternate);
		}
		#endregion
		#region GetAllDayAreaStyle
		public virtual AllDayAreaStyle GetAllDayAreaStyle() {
			AllDayAreaStyle style = CreateAllDayAreaStyle();
			style.CopyFrom(Styles.GetDefaultAllDayAreaStyle());
			style.CopyFrom(Styles.AllDayArea);
			return style;
		}
		protected internal virtual AllDayAreaStyle CreateAllDayAreaStyle() {
			return new AllDayAreaStyle();
		}
		#endregion
		#region GetCellBodyStyle
		public virtual CellBodyStyle GetCellBodyStyle(ITimeCell timeCell) {
			CellBodyStyle style = CreateCellBodyStyle();
			CellBodyStyle defaultStyle = Styles.GetDefaultTimeCellBodyStyle();
			style.CopyFrom(defaultStyle);
			SetCellBodyColor(style, timeCell);
			style.CopyFrom(Styles.TimeCellBody);
			return style;
		}
		protected internal virtual void SetCellBodyColor(CellBodyStyle style, ITimeCell cell) {
			SchedulerColorSchema colorSchema = ResourceColorSchemaHelper.GetResourceColorSchema((XtraScheduler.Resource)cell.Resource, ViewInfo, ResourceColorSchemas);
			WebTimeCell timeCell = cell as WebTimeCell;
			if (timeCell != null) {
				SchedulerCellColorSelector colorSelector = SchedulerCellColorSelector.Create(colorSchema, timeCell.IsWorkTime);
				SetCellBodyColorCore(style, timeCell, colorSelector);
			}
		}
		protected internal virtual void SetCellBodyColorCore(CellBodyStyle style, WebTimeCell timeCell, SchedulerCellColorSelector colorSelector) {
			style.BackColor = colorSelector.Cell;
			if (RenderUtils.Browser.IsOpera || RenderUtils.Browser.IsSafari) {
				style.BorderTop.BorderColor = ResourceColorSchemaHelper.CalculateTimeCellBorderColor(timeCell.EndOfHour, colorSelector);
				if (RenderUtils.Browser.IsSafari) 
					style.BorderBottom.BorderColor = ResourceColorSchemaHelper.CalculateTimeCellBorderColor(!timeCell.EndOfHour, colorSelector);
			} else {
				style.BorderBottom.BorderColor = ResourceColorSchemaHelper.CalculateTimeCellBorderColor(!timeCell.EndOfHour, colorSelector);
				if (RenderUtils.Browser.IsIE)
					style.BorderTop.BorderColor = ResourceColorSchemaHelper.CalculateTimeCellBorderColor(timeCell.EndOfHour, colorSelector);
			}
			style.BorderRight.BorderColor = colorSelector.CellBorderDark;
			style.BorderLeft.BorderColor = colorSelector.CellBorderDark;
		}
		protected internal virtual CellBodyStyle CreateCellBodyStyle() {
			return new CellBodyStyle();
		}
		#endregion
		#region GetTimeRulerHoursItemStyle
		public virtual TimeRulerItemStyle GetTimeRulerHoursItemStyle() {
			TimeRulerItemStyle style = CreateTimeRulerItemStyle();
			style.CopyFrom(Styles.GetDefaultTimeRulerHoursItemStyle());
			style.CopyFrom(Styles.TimeRulerHoursItem);
			return style;
		}
		protected internal virtual TimeRulerItemStyle CreateTimeRulerItemStyle() {
			return new TimeRulerItemStyle();
		}
		#endregion
		#region GetTimeRulerMinuteItemStyle
		public virtual TimeRulerItemStyle GetTimeRulerMinuteItemStyle() {
			TimeRulerItemStyle style = CreateTimeRulerItemStyle();
			style.CopyFrom(Styles.GetDefaultTimeRulerMinuteItemStyle());
			style.CopyFrom(Styles.TimeRulerMinuteItem);
			return style;
		}
		#endregion
		#region GetAppointmentHorizontalSeparatorStyle
		public override AppointmentHorizontalSeparatorStyle GetAppointmentHorizontalSeparatorStyle() {
			AppointmentHorizontalSeparatorStyle style = new AppointmentHorizontalSeparatorStyle();
			style.CopyFrom(Styles.GetDefaultAppointmentHorizontalSeparatorStyle());
			style.CopyFrom(View.Styles.AppointmentHorizontalSeparator);
			return style;
		}
		#endregion
		#region GetTimeMarkerStyle
		public virtual AppearanceStyle GetTimeMarkerStyle() {
			AppearanceStyle style = CreateTimeMarkerStyle();
			style.CopyFrom(Styles.GetDefaultTimeMarkerStyle());
			style.CopyFrom(Styles.TimeMarker);
			return style;
		}
		#endregion
	}
	#endregion
	#region WorkWeekViewStylesHelper
	public class WorkWeekViewStylesHelper : DayViewStylesHelper {
		public WorkWeekViewStylesHelper(SchedulerViewBase view, ISchedulerWebViewInfoBase viewInfo, ASPxSchedulerStylesBase controlStyles)
			: base(view, viewInfo, controlStyles) {
		}
	}
	#endregion
	#region FullWeekViewStylesHelper
	public class FullWeekViewStylesHelper : DayViewStylesHelper {
		public FullWeekViewStylesHelper(SchedulerViewBase view, ISchedulerWebViewInfoBase viewInfo, ASPxSchedulerStylesBase controlStyles)
			: base(view, viewInfo, controlStyles) {
		}
	}
	#endregion    
	#region WeekViewStylesHelper
	public class WeekViewStylesHelper : StylesHelper {
		public WeekViewStylesHelper(SchedulerViewBase view, ISchedulerWebViewInfoBase viewInfo, ASPxSchedulerStylesBase controlStyles)
			: base(view, viewInfo, controlStyles) {
		}
		#region Properties
		internal new WeekView View { get { return (WeekView)base.View; } }
		internal WeekViewStyles Styles { get { return (WeekViewStyles)InnerStyles; } }
		protected internal override ASPxSchedulerStylesBase InnerStyles { get { return View.Styles; } }
		#endregion
		#region GetStyle
		public override AppearanceStyleBase GetStyle(WebElementType type, ITimeCell timeCell, bool isAlternate) {
			AppearanceStyleBase resultStyle = null;
			switch (type) {
				case WebElementType.DateCellBody:
					resultStyle = GetDateCellBodyStyle(timeCell);
					break;
				case WebElementType.DateCellHeader:
					resultStyle = GetDateCellHeaderStyle(isAlternate);
					break;
			}
			if (resultStyle != null) {
				if (View != null && View.Control != null)
					View.Control.RaiseCustomizeElementStyle(resultStyle, type, timeCell, isAlternate);
				return resultStyle;
			}
			return base.GetStyle(type, timeCell, isAlternate);
		}
		#endregion
		#region GetDateCellBodyStyle
		public virtual CellBodyStyle GetDateCellBodyStyle(ITimeCell timeCell) {
			CellBodyStyle style = CreateCellBodyStyle();
			style.CopyFrom(Styles.GetDefaultDateCellBodyStyle());
			XtraScheduler.Resource resource = (timeCell == null) ? ResourceBase.Empty : timeCell.Resource;
			SchedulerColorSchema resourceColorSchema = ResourceColorSchemaHelper.GetResourceColorSchema(resource, ViewInfo, ResourceColorSchemas);
			ApplyColorSchemaToStyle(style, resourceColorSchema, timeCell.Interval.Start.Month % 2 == 0);
			style.CopyFrom(Styles.DateCellBody);
			return style;
		}
		protected internal virtual void ApplyColorSchemaToStyle(CellBodyStyle style, SchedulerColorSchema resourceColorSchema, bool isAlternate) {
			style.BackColor = resourceColorSchema.Cell;
			style.Border.BorderColor = resourceColorSchema.CellBorder;
		}
		protected internal virtual CellBodyStyle CreateCellBodyStyle() {
			return new CellBodyStyle();
		}
		#endregion
		#region GetDateCellHeaderStyle
		public virtual HeaderStyle GetDateCellHeaderStyle(bool isAlternate) {
			HeaderStyle style = CreateHeaderStyle();
			if (isAlternate) {
				style.CopyFrom(Styles.GetDefaultTodayCellHeaderStyle());
				style.CopyFrom(Styles.TodayCellHeader);
			} else {
				style.CopyFrom(Styles.GetDefaultDateCellHeaderStyle());
				style.CopyFrom(Styles.DateCellHeader);
			}
			return style;
		}
		#endregion
	}
	#endregion
	#region MonthViewStylesHelper
	public class MonthViewStylesHelper : WeekViewStylesHelper {
		public MonthViewStylesHelper(SchedulerViewBase view, ISchedulerWebViewInfoBase viewInfo, ASPxSchedulerStylesBase controlStyles)
			: base(view, viewInfo, controlStyles) {
		}
		protected internal override void ApplyColorSchemaToStyle(CellBodyStyle style, SchedulerColorSchema resourceColorSchema, bool isAlternate) {
			if (isAlternate) {
				style.BackColor = resourceColorSchema.Cell;
				style.Border.BorderColor = resourceColorSchema.CellBorder;
			} else {
				style.BackColor = resourceColorSchema.CellLight;
				style.Border.BorderColor = resourceColorSchema.CellLightBorder;
			}
		}
	}
	#endregion
	#region TimelineViewStylesHelper
	public class TimelineViewStylesHelper : StylesHelper {
		public TimelineViewStylesHelper(SchedulerViewBase view, ISchedulerWebViewInfoBase viewInfo, ASPxSchedulerStylesBase controlStyles)
			: base(view, viewInfo, controlStyles) {
		}
		#region Properties
		internal new TimelineView View { get { return (TimelineView)base.View; } }
		internal TimelineViewStyles Styles { get { return (TimelineViewStyles)InnerStyles; } }
		protected internal override ASPxSchedulerStylesBase InnerStyles { get { return View.Styles; } }
		#endregion
		#region GetStyle
		public override AppearanceStyleBase GetStyle(WebElementType type, ITimeCell timeCell, bool isAlternate) {
			AppearanceStyleBase resultStyle = null;
			switch (type) {
				case WebElementType.SelectionBar:
					resultStyle = GetSelectionBarStyle();
					break;
				case WebElementType.TimelineCellBody:
					resultStyle = GetTimelineCellBodyStyle(timeCell);
					break;
				case WebElementType.TimelineDateHeader:
					resultStyle = GetTimelineDateHeaderStyle(isAlternate);
					break;
			}
			if (resultStyle != null) {
				if (View != null && View.Control != null)
					View.Control.RaiseCustomizeElementStyle(resultStyle, type, timeCell, isAlternate);
				return resultStyle;
			}
			return base.GetStyle(type, timeCell, isAlternate);
		}
		#endregion
		#region GetSelectionBarStyle
		public virtual SelectionBarStyle GetSelectionBarStyle() {
			SelectionBarStyle style = CreateSelectionBarStyle();
			style.CopyFrom(Styles.GetDefaultSelectionBarStyle());
			SetSelectionBarCellColor(style, ResourceColorSchemas.GetSchema(0));
			style.CopyFrom(Styles.SelectionBar);
			return style;
		}
		protected internal virtual void SetSelectionBarCellColor(SelectionBarStyle style, SchedulerColorSchema colorSchema) {
			FreeTimeSchedulerCellColorSelector colorSelector = new FreeTimeSchedulerCellColorSelector(colorSchema);
			style.BackColor = colorSelector.Cell;
			style.Border.BorderColor = colorSelector.CellBorderDark;
		}
		protected internal virtual SelectionBarStyle CreateSelectionBarStyle() {
			return new SelectionBarStyle();
		}
		#endregion
		#region GetTimelineCellBodyStyle
		public virtual CellBodyStyle GetTimelineCellBodyStyle(ITimeCell timeCell) {
			CellBodyStyle style = CreateTimelineCellBodyStyle();
			style.CopyFrom(Styles.GetDefaultTimelineCellBodyStyle());
			SchedulerColorSchema colorSchema = ResourceColorSchemaHelper.GetResourceColorSchema((XtraScheduler.Resource)timeCell.Resource, ViewInfo, ResourceColorSchemas);
			WebTimeCell webTimeCell = timeCell as WebTimeCell;
			if (webTimeCell != null)
				SetTimeCellBodyColor(style, webTimeCell.IsWorkTime, colorSchema);
			style.CopyFrom(Styles.TimelineCellBody);
			if (View.CellAutoHeightOptions.Mode != AutoHeightMode.None && View.CellAutoHeightOptions.MinHeight > 0) {
				style.Height = Math.Max(View.CellAutoHeightOptions.MinHeight, ASPxSchedulerOptionsCellAutoHeight.MinDefaultHeight);
			}
			return style;
		}
		protected internal virtual void SetTimeCellBodyColor(CellBodyStyle style, bool isWorkTime, SchedulerColorSchema colorSchema) {
			SchedulerCellColorSelector colorSelector = SchedulerCellColorSelector.Create(colorSchema, isWorkTime);
			style.BackColor = colorSelector.Cell;
			style.Border.BorderColor = colorSelector.CellBorderDark;
		}
		protected internal virtual CellBodyStyle CreateTimelineCellBodyStyle() {
			return new CellBodyStyle();
		}
		#endregion
		#region GetTimelineDateheaderStyle
		public virtual HeaderStyle GetTimelineDateHeaderStyle(bool isAlternate) {
			HeaderStyle style = CreateTimelineDateHeaderStyle();
			style.CopyFrom(Styles.GetDefaultTimelineDateHeaderStyle());
			if (isAlternate)
				style.CopyFrom(Styles.GetDefaultAlternateTimelineDateHeader());
			style.CopyFrom(Styles.TimelineDateHeader);
			if (isAlternate)
				style.CopyFrom(Styles.AlternateTimelineDateHeader);
			return style;
		}
		protected internal virtual HeaderStyle CreateTimelineDateHeaderStyle() {
			return new HeaderStyle();
		}
		#endregion
	}
	#endregion
	#region SchedulerCellColorSelector
	public abstract class SchedulerCellColorSelector {
		public static SchedulerCellColorSelector Create(SchedulerColorSchema colorSchema, bool isWorkTime) {
			if (isWorkTime)
				return new WorkTimeSchedulerCellColorSelector(colorSchema);
			return new FreeTimeSchedulerCellColorSelector(colorSchema);
		}
		SchedulerColorSchema colorSchema;
		protected internal SchedulerCellColorSelector(SchedulerColorSchema colorSchema) {
			if (colorSchema == null)
				Exceptions.ThrowArgumentNullException("colorSchema");
			this.colorSchema = colorSchema;
		}
		public abstract Color Cell { get; }
		public abstract Color CellBorder { get; }
		public abstract Color CellBorderDark { get; }
		public SchedulerColorSchema ColorSchema { get { return colorSchema; } }
	}
	#endregion
	#region WorkTimeSchedulerCellColorSelector
	public class WorkTimeSchedulerCellColorSelector : SchedulerCellColorSelector {
		public WorkTimeSchedulerCellColorSelector(SchedulerColorSchema colorSchema)
			: base(colorSchema) {
		}
		public override Color Cell { get { return ColorSchema.CellLight; } }
		public override Color CellBorder { get { return ColorSchema.CellLightBorder; } }
		public override Color CellBorderDark { get { return ColorSchema.CellLightBorderDark; } }
	}
	#endregion
	#region FreeTimeSchedulerCellColorSelector
	public class FreeTimeSchedulerCellColorSelector : SchedulerCellColorSelector {
		public FreeTimeSchedulerCellColorSelector(SchedulerColorSchema colorSchema)
			: base(colorSchema) {
		}
		public override Color Cell { get { return ColorSchema.Cell; } }
		public override Color CellBorder { get { return ColorSchema.CellBorder; } }
		public override Color CellBorderDark { get { return ColorSchema.CellBorderDark; } }
	}
	#endregion
	#region ResourceColorSchemaHelper
	public static class ResourceColorSchemaHelper {
		public static SchedulerColorSchema GetResourceColorSchema(XtraScheduler.Resource resource, ISchedulerWebViewInfoBase viewInfo, SchedulerColorSchemaCollection resourceColorSchemas) {
			int resourceIndex = viewInfo.GetResourceColorIndex(resource);
			return resourceColorSchemas.GetSchema(resource.GetColor(), resourceIndex);
		}
		public static Color CalculateTimeCellBorderColor(bool endOfHour, SchedulerCellColorSelector colorSelector) {
			return (endOfHour) ? colorSelector.CellBorder : colorSelector.CellBorderDark;
		}
	}
	#endregion
}
