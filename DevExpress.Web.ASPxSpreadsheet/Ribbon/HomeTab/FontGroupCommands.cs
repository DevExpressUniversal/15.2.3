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
using System.Drawing;
using DevExpress.Web.ASPxSpreadsheet.Internal;
using DevExpress.Web.ASPxSpreadsheet.Internal.Commands;
using DevExpress.Web.ASPxSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.Export.Xl;
namespace DevExpress.Web.ASPxSpreadsheet {
	public class SRFormatFontNameCommand : SRComboBoxCommandBase {
		private const string FontItemTemplate = "<span style=\"font-family: {0};\">{1}</span>";
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatFontName;
			}
		}
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FormatFontNameWebCommand;
			}
		}
		protected override string DefaultNullText {
			get {
				return "(Font Name)";
			}
		}
		protected override ListEditItemCollection DefaultItems {
			get {
				if((base.Items == null) || (base.Items.Count == 0))
					return GetItems();
				return null;
			}
		}
		protected override int DefaultWidth {
			get {
				return 130;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "FontInformation";
			}
		}
		protected ListEditItemCollection GetItems() {
			ListEditItemCollection fontCollection = new ListEditItemCollection();
			foreach(FontFamily family in FontFamily.Families) {
				if(family.IsStyleAvailable(FontStyle.Regular)) {
					fontCollection.Add(family.Name, family.Name);
				}
			}
			return fontCollection;
		}
		protected override Dictionary<int, string> GetHtmlTextItemsDictionary() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			foreach(ListEditItem item in Items)
				result.Add(item.Index, string.Format(FontItemTemplate, item.Value, string.IsNullOrEmpty(item.Text) ? item.Value.ToString() : item.Text));
			return result;
		}
	}
	public class SRFormatFontSizeCommand : SRComboBoxCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatFontSize;
			}
		}
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FormatFontSizeWebCommand;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "FontInformation";
			}
		}
		protected override string DefaultNullText {
			get {
				return "(Font Size)";
			}
		}
		protected override int DefaultWidth {
			get {
				return 60;
			}
		}
		protected override ListEditItemCollection DefaultItems {
			get {
				return SpreadsheetRibbonHelper.GetFontSizes();
			}
		}
		protected override Dictionary<int, string> GetHtmlTextItemsDictionary() {
			Dictionary<int, string> result = new Dictionary<int, string>();
			foreach(ListEditItem item in Items) {
				var itemValue = Convert.ToInt32(item.Value);
				result.Add(item.Index, string.Format("<span style=\"font-size: {0}pt;\">{1}</span>", itemValue, string.IsNullOrEmpty(item.Text) ? item.Value.ToString() : item.Text));
			}
			return result;
		}
	}
	public class SRFormatIncreaseFontSizeCommand : SRButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatIncreaseFontSize;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "FontSize";
			}
		}
		protected override bool DefaultShowText {
			get {
				return false;
			}
		}
	}
	public class SRFormatDecreaseFontSizeCommand : SRButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatDecreaseFontSize;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "FontSize";
			}
		}
		protected override bool DefaultShowText {
			get {
				return false;
			}
		}
	}
	public class SRFormatFontBoldCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatFontBold;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "FontStyle";
			}
		}
	}
	public class SRFormatFontItalicCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatFontItalic;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "FontStyle";
			}
		}
	}
	public class SRFormatFontStrikeoutCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatFontStrikeout;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "FontStyle";
			}
		}
	}
	public class SRFormatFontUnderlineCommand : SRToggleButtonCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatFontUnderline;
			}
		}
		protected override string DefaultGroupName {
			get {
				return "FontStyle";
			}
		}
	}
	public class SRFormatFontColorCommand : SRColorCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatFontColor;
			}
		}
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FormatFontColor;
			}
		}
	}
	public class SRFormatFillColorCommand : SRColorCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatFillColor;
			}
		}
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FormatFillColor;
			}
		}
	}
	public class SRFormatBorderLineColorCommand : SRColorCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatBorderLineColor;
			}
		}
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FormatBorderLineColor;
			}
		}
	}
	public class SRFormatBordersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatBordersCommandGroup;
			}
		}
		protected override bool DefaultShowText {
			get {
				return false;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFormatBottomBorderCommand());
			Items.Add(new SRFormatTopBorderCommand());
			Items.Add(new SRFormatLeftBorderCommand());
			Items.Add(new SRFormatRightBorderCommand());
			Items.Add(new SRFormatNoBordersCommand());
			Items.Add(new SRFormatAllBordersCommand());
			Items.Add(new SRFormatOutsideBordersCommand());
			Items.Add(new SRFormatThickBorderCommand());
			Items.Add(new SRFormatBottomThickBorderCommand());
			Items.Add(new SRFormatTopAndBottomBorderCommand());
			Items.Add(new SRFormatTopAndThickBottomBorderCommand());
			Items.Add(new SRFormatBorderLineStyle());
		}
	}
	public class SRFormatBottomBorderCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatBottomBorder;
			}
		}
	}
	public class SRFormatTopBorderCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatTopBorder;
			}
		}
	}
	public class SRFormatLeftBorderCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatLeftBorder;
			}
		}
	}
	public class SRFormatRightBorderCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatRightBorder;
			}
		}
	}
	public class SRFormatNoBordersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatNoBorders;
			}
		}
	}
	public class SRFormatAllBordersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatAllBorders;
			}
		}
	}
	public class SRFormatOutsideBordersCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatOutsideBorders;
			}
		}
	}
	public class SRFormatThickBorderCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatThickBorder;
			}
		}
	}
	public class SRFormatBottomThickBorderCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatBottomThickBorder;
			}
		}
	}
	public class SRFormatTopAndBottomBorderCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatTopAndBottomBorder;
			}
		}
	}
	public class SRFormatTopAndThickBottomBorderCommand : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatTopAndThickBottomBorder;
			}
		}
	}
	public class SRFormatBorderLineStyle : SRDropDownCommandBase {
		protected override SpreadsheetCommandId CommandID {
			get {
				return SpreadsheetCommandId.FormatBorderLineStyle;
			}
		}
		protected override WebSpreadsheetCommandID WebCommandID {
			get {
				return WebSpreadsheetCommandID.FormatBorderLineStyle;
			}
		}
		protected override bool DefaultDropDownMode {
			get {
				return false;
			}
		}
		protected override void FillItems() {
			Items.Add(new SRFormatBorderLineStyleNoBorder());
			Items.Add(new SRFormatBorderLineStyleThin());
			Items.Add(new SRFormatBorderLineStyleDashed());
			Items.Add(new SRFormatBorderLineStyleDotted());
			Items.Add(new SRFormatBorderLineStyleDouble());
			Items.Add(new SRFormatBorderLineStyleMediumSolid());
			Items.Add(new SRFormatBorderLineStyleMediumDashed());
			Items.Add(new SRFormatBorderLineStyleMediumDotted());
			Items.Add(new SRFormatBorderLineStyleThick());
		}
	}
	internal class SRFormatBorderLineStyleNoBorder : SRBorderLineStyleCommandBase {
		protected override string BorderCaption {
			get {
				return XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.MenuCmd_FormatNoBorders);
			}
		}
	}
	internal class SRFormatBorderLineStyleThin : SRBorderLineStyleCommandBase {
		protected override XlBorderLineStyle BorderStyle {
			get {
				return XlBorderLineStyle.Thin;
			}
		}
		protected override string BorderCaption {
			get {
				return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.BorderLineStyle_Thin);
			}
		}
	}
	internal class SRFormatBorderLineStyleDashed : SRBorderLineStyleCommandBase {
		protected override XlBorderLineStyle BorderStyle {
			get {
				return XlBorderLineStyle.Dashed;
			}
		}
		protected override string BorderCaption {
			get {
				return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.BorderLineStyle_Dashed);
			}
		}
	}
	internal class SRFormatBorderLineStyleDotted : SRBorderLineStyleCommandBase {
		protected override XlBorderLineStyle BorderStyle {
			get {
				return XlBorderLineStyle.Dotted;
			}
		}
		protected override string BorderCaption {
			get {
				return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.BorderLineStyle_Dotted);
			}
		}
	}
	internal class SRFormatBorderLineStyleDouble : SRBorderLineStyleCommandBase {
		protected override XlBorderLineStyle BorderStyle {
			get {
				return XlBorderLineStyle.Double;
			}
		}
		protected override string BorderCaption {
			get {
				return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.BorderLineStyle_Double);
			}
		}
	}
	internal class SRFormatBorderLineStyleMediumSolid : SRBorderLineStyleCommandBase {
		protected override XlBorderLineStyle BorderStyle {
			get {
				return XlBorderLineStyle.Medium;
			}
		}
		protected override string BorderCaption {
			get {
				return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.BorderLineStyle_Medium);
			}
		}
	}
	internal class SRFormatBorderLineStyleMediumDashed : SRBorderLineStyleCommandBase {
		protected override XlBorderLineStyle BorderStyle {
			get {
				return XlBorderLineStyle.MediumDashed;
			}
		}
		protected override string BorderCaption {
			get {
				return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.BorderLineStyle_MediumDashed);
			}
		}
	}
	internal class SRFormatBorderLineStyleMediumDotted : SRBorderLineStyleCommandBase {
		protected override XlBorderLineStyle BorderStyle {
			get {
				return XlBorderLineStyle.MediumDashDotDot;
			}
		}
		protected override string BorderCaption {
			get {
				return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.BorderLineStyle_MediumDotted);
			}
		}
	}
	internal class SRFormatBorderLineStyleThick : SRBorderLineStyleCommandBase {
		protected override XlBorderLineStyle BorderStyle {
			get {
				return XlBorderLineStyle.Thick;
			}
		}
		protected override string BorderCaption {
			get {
				return ASPxSpreadsheetLocalizer.GetString(ASPxSpreadsheetStringId.BorderLineStyle_Thick);
			}
		}
	}
}
