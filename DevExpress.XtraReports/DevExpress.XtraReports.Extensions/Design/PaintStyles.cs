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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design.Ruler;
using System.Drawing.Printing;
using System.Collections;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
namespace DevExpress.XtraReports.Design {
	public static class ReportPaintStylesNames {
		public const string
								Flat = "Flat",
								Skin = "Skin",
								Office2003 = "Office2003";
	}
	public static class ReportPaintStyles {
		static Hashtable paintStyles = new Hashtable();
		static ReportPaintStyles() {
			RegisterPaintStyles();
		}
		static public ReportPaintStyle GetPaintStyle(UserLookAndFeel lookAndFeel) {
			Guard.ArgumentNotNull(lookAndFeel, "lookAndFeel");
			string paintStyleName;
			switch (lookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.WindowsXP:
				case ActiveLookAndFeelStyle.Office2003:
					paintStyleName = ReportPaintStylesNames.Office2003;
					break;
				case ActiveLookAndFeelStyle.Skin:
					paintStyleName = ReportPaintStylesNames.Skin;
					break;
				case ActiveLookAndFeelStyle.Flat:
				default:
					paintStyleName = ReportPaintStylesNames.Flat;
					break;
			}
			return (ReportPaintStyle)paintStyles[paintStyleName];
		}
		static void RegisterPaintStyles() {
			AddPaintStyle(new ReportPaintStyleFlat());
			AddPaintStyle(new ReportPaintStyleSkin());
			AddPaintStyle(new ReportPaintStyleOffice2003());
		}
		static void AddPaintStyle(ReportPaintStyle paintStyle) {
			paintStyles[paintStyle.Name] = paintStyle;
		}
		public static Margins GetPageBorders(UserLookAndFeel lookAndFeel) {
			return GetPaintStyle(lookAndFeel).GetPageBorders(lookAndFeel);
		}
		public static Size GetPageIndent(UserLookAndFeel lookAndFeel) {
			return GetPaintStyle(lookAndFeel).GetPageIndent(lookAndFeel);
		}
		public static Size GetFullPageIndent(UserLookAndFeel lookAndFeel) {
			Size indent = GetPageIndent(lookAndFeel);
			Margins borders = GetPageBorders(lookAndFeel);
			indent.Width += borders.Left;
			indent.Height += borders.Top;
			return indent;
		}
	}
	public abstract class ReportPaintStyle {
		public abstract string Name { get; }
		public virtual Size GetPageIndent(UserLookAndFeel lookAndFeel) {
			return new Size(22, 22);
		}
		public virtual Margins GetPageBorders(UserLookAndFeel lookAndFeel) {
			return new Margins(1, 1, 1, 1);
		}
		public abstract Color GetReportBackgroundColor(UserLookAndFeel lookAndFeel);
		public virtual Padding GetDesignPanelPadding(UserLookAndFeel lookAndFeel) {
			return new Padding(1);
		}
		public virtual int GetDesignPanelBottomIndent(UserLookAndFeel userLookAndFeel) {
			return 1;
		}
		public abstract RulerSectionPaintHelper CreateRulerSectionPaintHelper(UserLookAndFeel lookAndFeel); 
		public abstract CornerPanelPainter CreateCornerPanelPainter(UserLookAndFeel lookAndFeel); 
		public abstract PopupFormPainter CreatePopupFormPainter(UserLookAndFeel lookAndFeel); 
		public abstract SmartTagPainter CreateSmartTagPainter(UserLookAndFeel lookAndFeel); 
		public abstract ComponentTrayPainter CreateComponentTrayPainter(UserLookAndFeel lookAndFeel);
		public virtual ObjectPainter CreateEditPanelPainter() {
			return new EditPanelPainter();
		}
		public virtual ObjectPainter CreateEditPanelPainter(UserLookAndFeel lookAndFeel) {
			return new EditPanelPainter();
		}
	}
	public class ReportPaintStyleFlat : ReportPaintStyle {
		public override string Name { get { return ReportPaintStylesNames.Flat; } }
		public override Color GetReportBackgroundColor(UserLookAndFeel lookAndFeel) {
			return SystemColors.Control;
		}
		public override RulerSectionPaintHelper CreateRulerSectionPaintHelper(UserLookAndFeel lookAndFeel) {
			return new RulerSectionPaintHelperFlat();
		}
		public override CornerPanelPainter CreateCornerPanelPainter(UserLookAndFeel lookAndFeel) {
			return new CornerPanelPainterFlat();
		}
		public override PopupFormPainter CreatePopupFormPainter(UserLookAndFeel lookAndFeel) {
			return new PopupFormPainterFlat();
		}
		public override SmartTagPainter CreateSmartTagPainter(UserLookAndFeel lookAndFeel) {
			return new SmartTagPainterFlat();
		}
		public override ComponentTrayPainter CreateComponentTrayPainter(UserLookAndFeel lookAndFeel) {
			return new ComponentTrayPainterFlat();
		}
	}
	public class ReportPaintStyleSkin : ReportPaintStyle {
		SkinHelperBase helper = new SkinHelperBase(SkinProductId.Reports);
		public override string Name { get { return ReportPaintStylesNames.Skin; } }
		public override Size GetPageIndent(UserLookAndFeel lookAndFeel) {
			int width = helper.GetInteger(lookAndFeel, "PageHorizontalIndent");
			int height = helper.GetInteger(lookAndFeel, "PageVerticalIndent");
			return new Size(width, height);
		}
		public override Margins GetPageBorders(UserLookAndFeel lookAndFeel) {
			SkinPaddingEdges spe = helper.GetSkinEdges(lookAndFeel, PrintingSkins.SkinBorderPage);
			return new Margins(spe.Left, spe.Right, spe.Top, spe.Bottom);
		}
		public override Color GetReportBackgroundColor(UserLookAndFeel lookAndFeel) {
			return ReportsSkins.GetSkin(lookAndFeel).Properties.GetColor("ReportBackground");
		}
		public override Padding GetDesignPanelPadding(UserLookAndFeel lookAndFeel) {
			SkinProperties poperties = ReportsSkins.GetSkin(lookAndFeel).Properties;
			return new Padding(
				(int)(poperties["DesignPanelPaddingLeft"] ?? poperties["DesingPanelPaddingLeft"] ?? 1),
				(int)(poperties["DesignPanelPaddingTop"] ?? poperties["DesingPanelPaddingTop"] ?? 1),
				(int)(poperties["DesignPanelPaddingBottom"] ?? poperties["DesingPanelPaddingBottom"] ?? 1),
				(int)(poperties["DesignPanelPaddingRight"] ?? poperties["DesingPanelPaddingRignt"] ?? 1)
			);
		}
		public override int GetDesignPanelBottomIndent(UserLookAndFeel lookAndFeel) {
			return ReportsSkins.GetSkin(lookAndFeel).Properties.GetInteger("DesignPanelBottomIndent");
		}
		public override RulerSectionPaintHelper CreateRulerSectionPaintHelper(UserLookAndFeel lookAndFeel) {
			return new RulerSectionPaintHelperSkin(lookAndFeel);
		}
		public override CornerPanelPainter CreateCornerPanelPainter(UserLookAndFeel lookAndFeel) {
			return new CornerPanelPainterSkin(lookAndFeel);
		}
		public override PopupFormPainter CreatePopupFormPainter(UserLookAndFeel lookAndFeel) {
			return new PopupFormPainterSkin(lookAndFeel);
		}
		public override SmartTagPainter CreateSmartTagPainter(UserLookAndFeel lookAndFeel) {
			return new SmartTagPainterSkin(lookAndFeel);
		}
		public override ComponentTrayPainter CreateComponentTrayPainter(UserLookAndFeel lookAndFeel) {
			return new ComponentTrayPainterSkin(lookAndFeel);
		}
		public override ObjectPainter CreateEditPanelPainter(UserLookAndFeel lookAndFeel) {
			return new EditPanelPainterSkin(lookAndFeel);
		}
	}
	public class ReportPaintStyleOffice2003 : ReportPaintStyle {
		public override string Name { get { return ReportPaintStylesNames.Office2003; } }
		public override Color GetReportBackgroundColor(UserLookAndFeel lookAndFeel) {
			return Office2003PaintHelper.Colors.ReportBackground;
		}
		public override RulerSectionPaintHelper CreateRulerSectionPaintHelper(UserLookAndFeel lookAndFeel) {
			return new RulerSectionPaintHelperOffice2003();
		}
		public override CornerPanelPainter CreateCornerPanelPainter(UserLookAndFeel lookAndFeel) {
			return new CornerPanelPainterOffice2003();
		}
		public override PopupFormPainter CreatePopupFormPainter(UserLookAndFeel lookAndFeel) {
			return new PopupFormPainterOffice2003();
		}
		public override SmartTagPainter CreateSmartTagPainter(UserLookAndFeel lookAndFeel) {
			return new SmartTagPainterOffice2003();
		}
		public override ComponentTrayPainter CreateComponentTrayPainter(UserLookAndFeel lookAndFeel) {
			return new ComponentTrayPainterOffice2003();
		}
	}
}
