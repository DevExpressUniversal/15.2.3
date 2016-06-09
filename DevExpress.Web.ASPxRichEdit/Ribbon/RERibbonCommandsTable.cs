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

using DevExpress.Web.ASPxRichEdit.Internal;
using DevExpress.Web.Internal;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using System.Drawing;
using DevExpress.Web.ASPxRichEdit.Localization;
namespace DevExpress.Web.ASPxRichEdit {
	public class RERToggleFirstRowCommand : RERCheckBoxCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleFirstRow; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFirstRow); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFirstRowDescription); } }
		public RERToggleFirstRowCommand()
			: base() { }
	}
	public class RERToggleLastRowCommand : RERCheckBoxCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleLastRow; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleLastRow); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleLastRowDescription); } }
		public RERToggleLastRowCommand()
			: base() { }
	}
	public class RERToggleBandedRowsCommand : RERCheckBoxCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleBandedRows; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleBandedRows); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleBandedRowsDescription); } }
		public RERToggleBandedRowsCommand()
			: base() { }
	}
	public class RERToggleFirstColumnCommand : RERCheckBoxCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleFirstColumn; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFirstColumn); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleFirstColumnDescription); } }
		public RERToggleFirstColumnCommand()
			: base() { }
	}
	public class RERToggleLastColumnCommand : RERCheckBoxCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleLastColumn; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleLastColumn); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleLastColumnDescription); } }
		public RERToggleLastColumnCommand()
			: base() { }
	}
	public class RERToggleBandedColumnCommand : RERCheckBoxCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleBandedColumn; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleBandedColumn); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleBandedColumnDescription); } }
		public RERToggleBandedColumnCommand()
			: base() { }
	}
	public class RERChangeTableStyleCommand : RERGalleryBarCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ApplyTableStyle; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeStyle); } }
		public RERChangeTableStyleCommand()
			: base() { }
	}
	public class RERChangeCurrentBorderRepositoryItemLineStyleCommand : RERComboBoxCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangeTableBorderStyleRepositoryItem; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemLineStyle); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemLineStyleDescription); } }
		protected override int DefaultWidth { get { return 100; } }
		public RERChangeCurrentBorderRepositoryItemLineStyleCommand()
			: base() { }
		protected override ListEditItemCollection DefaultItems {
			get {
				if ((base.Items == null) || (base.Items.Count == 0))
					return GetItems();
				return null;
			}
		}
		protected ListEditItemCollection GetItems() {
			ListEditItemCollection result = new ListEditItemCollection();
			result.Add("None", (int)BorderLineStyle.None);
			result.Add("Single", (int)BorderLineStyle.Single);
			result.Add("Dotted", (int)BorderLineStyle.Dotted);
			result.Add("DashSmallGap", (int)BorderLineStyle.DashSmallGap);
			result.Add("Dashed", (int)BorderLineStyle.Dashed);
			result.Add("DotDash", (int)BorderLineStyle.DotDash);
			result.Add("DotDotDash", (int)BorderLineStyle.DotDotDash);
			result.Add("Double", (int)BorderLineStyle.Double);
			result.Add("Wave", (int)BorderLineStyle.Wave);
			return result;
		}
	}
	public class RERChangeCurrentBorderRepositoryItemLineThicknessCommand : RERComboBoxCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangeTableBorderWidthRepositoryItem; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemLineThickness); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemLineThicknessDescription); } }
		protected override int DefaultWidth { get { return 100; } }
		public RERChangeCurrentBorderRepositoryItemLineThicknessCommand()
			: base() { }
		protected override ListEditItemCollection DefaultItems {
			get {
				if ((base.Items == null) || (base.Items.Count == 0))
					return GetItems();
				return null;
			}
		}
		protected ListEditItemCollection GetItems() {
			ListEditItemCollection result = new ListEditItemCollection();
			result.Add("0.25 pt", 5);
			result.Add("0.5 pt", 10);
			result.Add("0.75 pt", 15);
			result.Add("1 pt", 20);
			result.Add("1.5 pt", 30);
			result.Add("2 pt", 40);
			return result;
		}
	}
	public class RERChangeCurrentBorderRepositoryItemColorCommand : RERColorCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangeTableBorderColorRepositoryItem; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemColor); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeCurrentBorderRepositoryItemColorDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.PenColor; } }
		protected override string DefaultAutomaticColorItemCaption { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_ColorAutomatic); } }
		protected override Color DefaultAutomaticColor { get { return Color.Black; } }
		protected override string DefaultAutomaticColorItemValue { get { return ColorTranslator.ToOle(Color.Empty).ToString(); } }
		public RERChangeCurrentBorderRepositoryItemColorCommand()
			: base() { }
		public RERChangeCurrentBorderRepositoryItemColorCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERChangeTableBordersCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeTableBorders); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeTableBordersDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BordersOutside; } }
		protected override bool DefaultDropDownMode { get { return false; } }
		public RERChangeTableBordersCommand()
			: base() { }
		public RERChangeTableBordersCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERToggleTableCellsBottomBorderCommand());
			Items.Add(new RERToggleTableCellsTopBorderCommand());
			Items.Add(new RERToggleTableCellsLeftBorderCommand());
			Items.Add(new RERToggleTableCellsRightBorderCommand());
			Items.Add(new RERBorderLineStyleNoneCommand());
			Items.Add(new RERToggleTableCellsAllBordersCommand());
			Items.Add(new RERToggleTableCellsOutsideBorderCommand());
			Items.Add(new RERToggleTableCellsInsideBorderCommand());
			Items.Add(new RERToggleTableCellsInsideHorizontalBorderCommand());
			Items.Add(new RERToggleTableCellsInsideVerticalBorderCommand());
			Items.Add(new RERDropDownToggleShowTableGridLinesCommand());
		}
	}
	public class RERToggleTableCellsBottomBorderCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTableCellsBottomBorder; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomBorder); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomBorderDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BorderBottom; } }
		public RERToggleTableCellsBottomBorderCommand()
			: base() { }
		public RERToggleTableCellsBottomBorderCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsTopBorderCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTableCellsTopBorder; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopBorder); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopBorderDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BorderTop; } }
		public RERToggleTableCellsTopBorderCommand()
			: base() { }
		public RERToggleTableCellsTopBorderCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsLeftBorderCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTableCellsLeftBorder; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsLeftBorder); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsLeftBorderDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BorderLeft; } }
		public RERToggleTableCellsLeftBorderCommand()
			: base() { }
		public RERToggleTableCellsLeftBorderCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsRightBorderCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTableCellsRightBorder; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsRightBorder); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsRightBorderDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BorderRight; } }
		public RERToggleTableCellsRightBorderCommand()
			: base() { }
		public RERToggleTableCellsRightBorderCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERBorderLineStyleNoneCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTableCellNoBorder; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ResetTableCellsBorders); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ResetTableCellsBordersDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BorderNone; } }
		public RERBorderLineStyleNoneCommand()
			: base() {
			BeginGroup = true;
		}
		public RERBorderLineStyleNoneCommand(RibbonItemSize size)
			: base(size) {
			BeginGroup = true;
		}
	}
	public class RERToggleTableCellsAllBordersCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTableCellAllBorders; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsAllBorders); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsAllBordersDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BordersAll; } }
		public RERToggleTableCellsAllBordersCommand()
			: base() { }
		public RERToggleTableCellsAllBordersCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsOutsideBorderCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTableCellOutsideBorders; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsOutsideBorder); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsOutsideBorderDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BordersOutside; } }
		public RERToggleTableCellsOutsideBorderCommand()
			: base() { }
		public RERToggleTableCellsOutsideBorderCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsInsideBorderCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTableCellInsideBorders; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideBorder); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideBorderDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BordersInside; } }
		public RERToggleTableCellsInsideBorderCommand()
			: base() { }
		public RERToggleTableCellsInsideBorderCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsInsideHorizontalBorderCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTableCellInsideHorizontalBorders; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideHorizontalBorder); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideHorizontalBorderDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BorderInsideHorizontal; } }
		public RERToggleTableCellsInsideHorizontalBorderCommand()
			: base() {
			BeginGroup = true;
		}
		public RERToggleTableCellsInsideHorizontalBorderCommand(RibbonItemSize size)
			: base(size) {
			BeginGroup = true;
		}
	}
	public class RERToggleTableCellsInsideVerticalBorderCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleTableCellInsideVerticalBorders; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideVerticalBorder); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsInsideVerticalBorderDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.BorderInsideVertical; } }
		public RERToggleTableCellsInsideVerticalBorderCommand()
			: base() { }
		public RERToggleTableCellsInsideVerticalBorderCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERDropDownToggleShowTableGridLinesCommand : RERDropDownToggleCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleShowTableGridLines; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleShowTableGridLines); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleShowTableGridLinesDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ViewTableGridlines; } }
		public RERDropDownToggleShowTableGridLinesCommand()
			: base() {
			BeginGroup = true;
		}
		public RERDropDownToggleShowTableGridLinesCommand(RibbonItemSize size)
			: base(size) {
			BeginGroup = true;
		}
	}
	public class RERChangeTableCellShadingCommand : RERColorCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ChangeTableCellShading; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeTableCellShading); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ChangeTableCellShadingDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Shading; } }
		protected override string DefaultAutomaticColorItemCaption { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.Caption_NoColor); } }
		protected override string DefaultAutomaticColorItemValue { get { return ColorTranslator.ToOle(Color.Transparent).ToString(); } }
		public RERChangeTableCellShadingCommand()
			: base() { }
		public RERChangeTableCellShadingCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSelectTableElementsCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectTableElements); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectTableElementsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.Select; } }
		protected override bool DefaultDropDownMode { get { return false; } }
		public RERSelectTableElementsCommand()
			: base() { }
		public RERSelectTableElementsCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERSelectTableCellCommand());
			Items.Add(new RERSelectTableColumnCommand());
			Items.Add(new RERSelectTableRowCommand());
			Items.Add(new RERSelectTableCommand());
		}
	}
	public class RERSelectTableCellCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SelectTableCell; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectTableCell); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectTableCellDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.SelectTableCell; } }
		public RERSelectTableCellCommand()
			: base() { }
		public RERSelectTableCellCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSelectTableColumnCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SelectTableColumn; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectTableColumns); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectTableColumnsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.SelectTableColumn; } }
		public RERSelectTableColumnCommand()
			: base() { }
		public RERSelectTableColumnCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSelectTableRowCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SelectTableRow; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectTableRow); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectTableRowDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.SelectTableRow; } }
		public RERSelectTableRowCommand()
			: base() { }
		public RERSelectTableRowCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSelectTableCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.SelectTable; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectTable); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SelectTableDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.SelectTable; } }
		public RERSelectTableCommand()
			: base() { }
		public RERSelectTableCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleShowTableGridLinesCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ToggleShowTableGridLines; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleShowTableGridLines); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleShowTableGridLinesDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.ViewTableGridlines; } }
		public RERToggleShowTableGridLinesCommand()
			: base() { }
		public RERToggleShowTableGridLinesCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERShowTablePropertiesFormCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowTablePropertiesForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowTablePropertiesForm); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ShowTablePropertiesFormDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.TableProperties; } }
		public RERShowTablePropertiesFormCommand()
			: base() { }
		public RERShowTablePropertiesFormCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERDeleteTableElementsCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.None; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableElements); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableElementsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.DeleteTable; } }
		protected override bool DefaultDropDownMode { get { return false; } }
		public RERDeleteTableElementsCommand()
			: base() { }
		public RERDeleteTableElementsCommand(RibbonItemSize size)
			: base(size) { }
		protected override void FillItems() {
			Items.Add(new RERDeleteTableCellsCommand());
			Items.Add(new RERDeleteTableColumnsCommand());
			Items.Add(new RERDeleteTableRowsCommand());
			Items.Add(new RERDeleteTableCommand());
		}
	}
	public class RERDeleteTableCellsCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowDeleteTableCellsForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableCells); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableCellsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.DeleteTableCells; } }
		public RERDeleteTableCellsCommand()
			: base() { }
		public RERDeleteTableCellsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERDeleteTableColumnsCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.DeleteTableColumns; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableColumns); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableColumnsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.DeleteTableColumns; } }
		public RERDeleteTableColumnsCommand()
			: base() { }
		public RERDeleteTableColumnsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERDeleteTableRowsCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.DeleteTableRows; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableRows); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableRowsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.DeleteTableRows; } }
		public RERDeleteTableRowsCommand()
			: base() { }
		public RERDeleteTableRowsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERDeleteTableCommand : RERDropDownCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.DeleteTable; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTable); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_DeleteTableDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.DeleteTable; } }
		public RERDeleteTableCommand()
			: base() { }
		public RERDeleteTableCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertTableRowAboveCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertTableRowAbove; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableRowAbove); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableRowAboveDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertTableRowsAbove; } }
		public RERInsertTableRowAboveCommand()
			: base() { }
		public RERInsertTableRowAboveCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertTableRowBelowCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertTableRowBelow; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableRowBelow); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableRowBelowDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertTableRowsBelow; } }
		public RERInsertTableRowBelowCommand()
			: base() { }
		public RERInsertTableRowBelowCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertTableColumnToTheLeftCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertTableColumnToTheLeft; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableColumnToTheLeft); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableColumnToTheLeftDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertTableColumnsToTheLeft; } }
		public RERInsertTableColumnToTheLeftCommand()
			: base() { }
		public RERInsertTableColumnToTheLeftCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERInsertTableColumnToTheRightCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.InsertTableColumnToTheRight; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableColumnToTheRight); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_InsertTableColumnToTheRightDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.InsertTableColumnsToTheRight; } }
		public RERInsertTableColumnToTheRightCommand()
			: base() { }
		public RERInsertTableColumnToTheRightCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERMergeTableCellsCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.MergeTableCells; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_MergeTableCells); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_MergeTableCellsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.MergeTableCells; } }
		public RERMergeTableCellsCommand()
			: base() { }
		public RERMergeTableCellsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERSplitTableCellsCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowSplitTableCellsForm; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SplitTableCells); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_SplitTableCellsDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.SplitTableCells; } }
		public RERSplitTableCellsCommand()
			: base() { }
		public RERSplitTableCellsCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsTopLeftAlignmentCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.TableCellAlignTopLeft; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopLeftAlignment); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopLeftAlignmentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignTopLeft; } }
		protected override bool ShowText { get { return false; } }
		protected override string GetSubGroupName() { return "TableCellsTop"; }
		public RERToggleTableCellsTopLeftAlignmentCommand()
			: base() { }
		public RERToggleTableCellsTopLeftAlignmentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsTopCenterAlignmentCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.TableCellAlignTopCenter; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopCenterAlignment); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopCenterAlignmentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignTopCenter; } }
		protected override bool ShowText { get { return false; } }
		protected override string GetSubGroupName() { return "TableCellsTop"; }
		public RERToggleTableCellsTopCenterAlignmentCommand()
			: base() { }
		public RERToggleTableCellsTopCenterAlignmentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsTopRightAlignmentCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.TableCellAlignTopRight; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopRightAlignment); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsTopRightAlignmentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignTopRight; } }
		protected override bool ShowText { get { return false; } }
		protected override string GetSubGroupName() { return "TableCellsTop"; }
		public RERToggleTableCellsTopRightAlignmentCommand()
			: base() { }
		public RERToggleTableCellsTopRightAlignmentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsMiddleLeftAlignmentCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.TableCellAlignMiddleLeft; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleLeftAlignment); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleLeftAlignmentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignMiddleLeft; } }
		protected override bool ShowText { get { return false; } }
		protected override string GetSubGroupName() { return "TableCellsMiddle"; }
		public RERToggleTableCellsMiddleLeftAlignmentCommand()
			: base() { }
		public RERToggleTableCellsMiddleLeftAlignmentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsMiddleCenterAlignmentCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.TableCellAlignMiddleCenter; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleCenterAlignment); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleCenterAlignmentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignMiddleCenter; } }
		protected override bool ShowText { get { return false; } }
		protected override string GetSubGroupName() { return "TableCellsMiddle"; }
		public RERToggleTableCellsMiddleCenterAlignmentCommand()
			: base() { }
		public RERToggleTableCellsMiddleCenterAlignmentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsMiddleRightAlignmentCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.TableCellAlignMiddleRight; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleRightAlignment); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsMiddleRightAlignmentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignMiddleRight; } }
		protected override bool ShowText { get { return false; } }
		protected override string GetSubGroupName() { return "TableCellsMiddle"; }
		public RERToggleTableCellsMiddleRightAlignmentCommand()
			: base() { }
		public RERToggleTableCellsMiddleRightAlignmentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsBottomLeftAlignmentCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.TableCellAlignBottomLeft; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomLeftAlignment); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomLeftAlignmentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignBottomLeft; } }
		protected override bool ShowText { get { return false; } }
		protected override string GetSubGroupName() { return "TableCellsBottom"; }
		public RERToggleTableCellsBottomLeftAlignmentCommand()
			: base() { }
		public RERToggleTableCellsBottomLeftAlignmentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsBottomCenterAlignmentCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.TableCellAlignBottomCenter; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomCenterAlignment); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomCenterAlignmentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignBottomCenter; } }
		protected override bool ShowText { get { return false; } }
		protected override string GetSubGroupName() { return "TableCellsBottom"; }
		public RERToggleTableCellsBottomCenterAlignmentCommand()
			: base() { }
		public RERToggleTableCellsBottomCenterAlignmentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERToggleTableCellsBottomRightAlignmentCommand : RERToggleButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.TableCellAlignBottomRight; } }
		protected override string DefaultText { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomRightAlignment); } }
		protected override string DefaultToolTip { get { return XtraRichEditLocalizer.GetString(XtraRichEditStringId.MenuCmd_ToggleTableCellsBottomRightAlignmentDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.AlignBottomRight; } }
		protected override bool ShowText { get { return false; } }
		protected override string GetSubGroupName() { return "TableCellsBottom"; }
		public RERToggleTableCellsBottomRightAlignmentCommand()
			: base() { }
		public RERToggleTableCellsBottomRightAlignmentCommand(RibbonItemSize size)
			: base(size) { }
	}
	public class RERShowTableOptionsFormCommand : RERButtonCommandBase {
		protected override RichEditClientCommand Command { get { return RichEditClientCommand.ShowCellOptionsForm; } }
		protected override string DefaultText { get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MenuCmd_ShowCellOptionsForm); } }
		protected override string DefaultToolTip { get { return ASPxRichEditLocalizer.GetString(ASPxRichEditStringId.MenuCmd_ShowCellOptionsFormDescription); } }
		protected override string ImageName { get { return RichEditRibbonImages.TableCellMargins; } }
		public RERShowTableOptionsFormCommand()
			: base() { }
		public RERShowTableOptionsFormCommand(RibbonItemSize size)
			: base(size) { }
	}
}
