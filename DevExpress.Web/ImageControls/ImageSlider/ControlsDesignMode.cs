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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
namespace DevExpress.Web.Internal {
	public class DesignModeConstants {
		public const string LayoutClassName = "dxis-mainLayout";
		public const string GrayBackgroundClassName = "dxis-grayBackground";
		public const string GrayBorderClassName = "dxis-grayBorder";
		public const string ButtonsClassName = "dxis-buttons";
		public const string ButtonClassName = "dxis-button";
		public const string PrevButtonClassName = "dxis-prevButton";
		public const string NextButtonClassName = "dxis-nextButton";
		public const string ButtonsSpacingClassName = "dxis-buttonSpacing";
		public const string ButtonVerticalOrientationClassName = "dxis-buttonVertOrientation";
		public const string ButtonHorizontalOrientationClassName = "dxis-buttonHorOrientation";
		public const string NavigationBarClassName = "dxis-navBar";
		public const string NavigationBarCellClassName = "dxis-navBarCell";
		public const string DotsNavigationBarCellClassName = "dxis-dotsNavBarCell";
		public const string NavigationBarHorizontalPositionClassName = "dxis-navBarHor";
		public const string NavigationBarVerticalPositionClassName = "dxis-navBarVert";
		public const string NavigationBarWrapperClassName = "dxis-navBarWrapper";
		public const string NavigationBarButtonsWrapperClassName = "dxis-navBarButtonsWrapper";
		public const string ImageAreaClassName = "dxis-imageArea";
		public const string NoImageAreaClassName = "dxis-noImageArea";
		public const string SpacingClassName = "dxis-spacing";
		public const string DotsNavigationBarItemClassName = "dxis-dot";
		public const string DotsNavigationBarSelectedItemClassName = "dxis-dotSelected";
		public const string NavigationBarThumbnailItemClassName = "dxis-thumbnail";
		public const string NavigationBarItemSpacingClassName = "dxis-itemSpacing";
	}
	public class DesignModeUtils {
		public static void AppendGrayBorder(WebControl control) {
			RenderUtils.AppendDefaultDXClassName(control, DesignModeConstants.GrayBorderClassName);
		}
		public static void AppendGrayBackground(WebControl control) {
			RenderUtils.AppendDefaultDXClassName(control, DesignModeConstants.GrayBackgroundClassName);
		}
		public static void AppendBackgroundImage(WebControl control, ASPxImageSliderBase imageSlider) {
			control.Style["background-image"] = string.Format("url({0})", imageSlider.GetImagesInternal().GetImageProperties(imageSlider.Page, ImageSliderImages.DesignTimeSpriteImageName).Url);
		}
	}
	[ToolboxItem(false)]
	public class ImageSliderControlDesignMode : ImageSliderControlBase {
		protected InternalTable LayoutTable { get; private set; }
		protected bool CanCreateSpacing {
			get { return ImageSlider.ShowImageAreaInternal && ImageSlider.ShowNavigationBarInternal; }
		}
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Table; }
		}
		public ImageSliderControlDesignMode(ASPxImageSliderBase imageSlider)
			: base(imageSlider) {
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			CreateLayoutTable();
			switch(ImageSlider.SettingsNavigationBarInternal.Position) {
				case NavigationBarPosition.Bottom:
					CreateBottomNavBarWithImageArea();
					break;
				case NavigationBarPosition.Top:
					CreateTopNavBarWithImageArea();
					break;
				case NavigationBarPosition.Left:
					CreateLeftNavBarWithImageArea();
					break;
				case NavigationBarPosition.Right:
					CreateRightNavBarWithImageArea();
					break;
			}
		}
		protected void CreateLayoutTable() {
			WebControl row = RenderUtils.CreateWebControl(HtmlTextWriterTag.Tr);
			Controls.Add(row);
			WebControl cell = RenderUtils.CreateWebControl(HtmlTextWriterTag.Td);
			row.Controls.Add(cell);
			LayoutTable = RenderUtils.CreateTable();
			cell.Controls.Add(LayoutTable);
		}
		protected void CreateTopNavBarWithImageArea() {
			if(ImageSlider.ShowNavigationBarInternal)
				CreateTableRow().Cells.Add(CreateNavigationBarCell());
			if(CanCreateSpacing)
				CreateTableRow().Cells.Add(CreateSpacingCell());
			if(ImageSlider.ShowImageAreaInternal)
				CreateTableRow().Cells.Add(new ImageAreaControlCell(ImageSlider));
		}
		protected void CreateBottomNavBarWithImageArea() {
			if(ImageSlider.ShowImageAreaInternal)
				CreateTableRow().Cells.Add(new ImageAreaControlCell(ImageSlider));
			if(CanCreateSpacing)
				CreateTableRow().Cells.Add(CreateSpacingCell());
			if(ImageSlider.ShowNavigationBarInternal)
				CreateTableRow().Cells.Add(CreateNavigationBarCell());
		}
		protected void CreateRightNavBarWithImageArea() {
			InternalTableRow row = CreateTableRow();
			if(ImageSlider.ShowImageAreaInternal)
				row.Cells.Add(new ImageAreaControlCell(ImageSlider));
			if(CanCreateSpacing)
				row.Cells.Add(CreateSpacingCell());
			if(ImageSlider.ShowNavigationBarInternal)
				row.Cells.Add(CreateNavigationBarCell());
		}
		protected void CreateLeftNavBarWithImageArea() {
			InternalTableRow row = CreateTableRow();
			if(ImageSlider.ShowNavigationBarInternal)
				row.Cells.Add(CreateNavigationBarCell());
			if(CanCreateSpacing)
				row.Cells.Add(CreateSpacingCell());
			if(ImageSlider.ShowImageAreaInternal)
				row.Cells.Add(new ImageAreaControlCell(ImageSlider));
		}
		protected InternalTableRow CreateTableRow() {
			InternalTableRow row = new InternalTableRow();
			LayoutTable.Rows.Add(row);
			return row;
		}
		protected InternalTableCell CreateSpacingCell() {
			InternalTableCell cell = new InternalTableCell();
			RenderUtils.AppendDefaultDXClassName(cell, DesignModeConstants.SpacingClassName);
			return cell;
		}
		protected NavigationBarControlCellBase CreateNavigationBarCell() {
			if(ImageSlider.SettingsNavigationBarInternal.Mode == NavigationBarMode.Thumbnails)
				return new ThumbnailNavigationBarControlCell(ImageSlider);
			return new DotsNavigationBarControlCell(ImageSlider);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.ApplyCellPaddingAndSpacing(this);
			RenderUtils.AppendDefaultDXClassName(LayoutTable, DesignModeConstants.LayoutClassName);
			if(!ImageSlider.ShowImageAreaInternal)
				RenderUtils.AppendDefaultDXClassName(this, DesignModeConstants.NoImageAreaClassName);
			if(ImageSlider.ShowNavigationBarInternal) {
				bool isHorizontal = ImageSlider.SettingsNavigationBarInternal.Position == NavigationBarPosition.Top || ImageSlider.SettingsNavigationBarInternal.Position == NavigationBarPosition.Bottom;
				RenderUtils.AppendDefaultDXClassName(this, isHorizontal ? DesignModeConstants.NavigationBarHorizontalPositionClassName : DesignModeConstants.NavigationBarVerticalPositionClassName);
			}
		}
	}
	[ToolboxItem(false)]
	public class ImageAreaControlCell : InternalTableCell {
		protected ASPxImageSliderBase ImageSlider { get; private set; }
		public ImageAreaControlCell(ASPxImageSliderBase imageSlider) {
			ImageSlider = imageSlider;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(ImageSlider.SettingsImageAreaInternal.NavigationButtonVisibility != ElementVisibilityMode.None)
				Controls.Add(new NavigationButtons(ImageSlider, ImageSlider.SettingsImageAreaInternal.NavigationDirection == NavigationDirection.Horizontal));
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			DesignModeUtils.AppendGrayBorder(this);
			DesignModeUtils.AppendGrayBackground(this);
			RenderUtils.AppendDefaultDXClassName(this, DesignModeConstants.ImageAreaClassName);
		}
	}
	[ToolboxItem(false)]
	public abstract class NavigationBarControlCellBase : InternalTableCell {
		protected ASPxImageSliderBase ImageSlider { get; private set; }
		protected InternalTable LayoutTable { get; private set; }
		protected WebControl RalativeWrapper { get; private set; }
		protected WebControl ButtonsWrapper { get; private set; }
		protected bool IsHorizontalPosition {
			get {
				NavigationBarPosition position = ImageSlider.SettingsNavigationBarInternal.Position;
				return position == NavigationBarPosition.Bottom || position == NavigationBarPosition.Top;
			}
		}
		public NavigationBarControlCellBase(ASPxImageSliderBase imageSlider) {
			ImageSlider = imageSlider;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(CanCreateButtons()) {
				RalativeWrapper = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
				Controls.Add(RalativeWrapper);
			}
			LayoutTable = RenderUtils.CreateTable();
			(RalativeWrapper ?? this).Controls.Add(LayoutTable);
			if(CanCreateButtons()) {
				ButtonsWrapper = new WebControl(HtmlTextWriterTag.Div);
				RalativeWrapper.Controls.Add(ButtonsWrapper);
				ButtonsWrapper.Controls.Add(new NavigationButtons(ImageSlider, IsHorizontalPosition));
			}
			if(IsHorizontalPosition)
				CreateHorizontalItemElements();
			else
				CreateVerticalItemElements();
		}
		protected void CreateHorizontalItemElements() {
			InternalTableRow row = new InternalTableRow();
			LayoutTable.Rows.Add(row);
			for(int i = 0; i < GetHorizontalItemCount(); i++) {
				row.Cells.Add(CreateItemCell(i));
				if(i < GetHorizontalItemCount() - 1)
					row.Cells.Add(CreateSpacingCell());
			}
		}
		protected void CreateVerticalItemElements() {
			for(int i = 0; i < GetVerticalItemCount(); i++) {
				InternalTableRow itemRow = new InternalTableRow();
				LayoutTable.Rows.Add(itemRow);
				itemRow.Cells.Add(CreateItemCell(i));
				if(i < GetVerticalItemCount() - 1) {
					InternalTableRow spacingRow = new InternalTableRow();
					LayoutTable.Rows.Add(spacingRow);
					spacingRow.Cells.Add(CreateSpacingCell());
				}
			}
		}
		protected virtual InternalTableCell CreateItemCell(int index) {
			InternalTableCell cell = new InternalTableCell();
			return cell;
		}
		protected InternalTableCell CreateSpacingCell() {
			InternalTableCell cell = new InternalTableCell();
			RenderUtils.AppendDefaultDXClassName(cell, DesignModeConstants.NavigationBarItemSpacingClassName);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, GetNavigationBarCellClassName());
			RenderUtils.AppendDefaultDXClassName(LayoutTable, DesignModeConstants.NavigationBarClassName);
			if(RalativeWrapper != null)
				RalativeWrapper.CssClass = DesignModeConstants.NavigationBarWrapperClassName;
			if(ButtonsWrapper != null)
				ButtonsWrapper.CssClass = DesignModeConstants.NavigationBarButtonsWrapperClassName;
		}
		protected virtual bool CanCreateButtons() {
			return true;
		}
		protected virtual int GetHorizontalItemCount() {
			return 0;
		}
		protected virtual int GetVerticalItemCount() {
			return 0;
		}
		protected virtual string GetNavigationBarCellClassName() {
			return string.Empty;
		}
	}
	[ToolboxItem(false)]
	public class ThumbnailNavigationBarControlCell : NavigationBarControlCellBase {
		public ThumbnailNavigationBarControlCell(ASPxImageSliderBase imageSlider)
			: base(imageSlider) {
		}
		protected override bool CanCreateButtons() {
			return ImageSlider.SettingsNavigationBarInternal.ThumbnailsModeNavigationButtonVisibility != ElementVisibilityMode.None;
		}
		protected override int GetHorizontalItemCount() {
			return 6;
		}
		protected override int GetVerticalItemCount() {
			return 7;
		}
		protected override InternalTableCell CreateItemCell(int index) {
			InternalTableCell cell = base.CreateItemCell(index);
			DesignModeUtils.AppendGrayBorder(cell);
			DesignModeUtils.AppendGrayBackground(cell);
			RenderUtils.AppendDefaultDXClassName(cell, DesignModeConstants.NavigationBarThumbnailItemClassName);
			return cell;
		}
		protected override string GetNavigationBarCellClassName() {
			return DesignModeConstants.NavigationBarCellClassName;
		}
	}
	[ToolboxItem(false)]
	public class DotsNavigationBarControlCell : NavigationBarControlCellBase {
		public DotsNavigationBarControlCell(ASPxImageSliderBase imageSlider)
			: base(imageSlider) {
		}
		protected override bool CanCreateButtons() {
			return false;
		}
		protected override int GetHorizontalItemCount() {
			return 6;
		}
		protected override int GetVerticalItemCount() {
			return 6;
		}
		protected override string GetNavigationBarCellClassName() {
			return DesignModeConstants.DotsNavigationBarCellClassName;
		}
		protected override InternalTableCell CreateItemCell(int index) {
			InternalTableCell cell = base.CreateItemCell(index);
			DesignModeUtils.AppendBackgroundImage(cell, ImageSlider);
			RenderUtils.AppendDefaultDXClassName(cell, DesignModeConstants.DotsNavigationBarItemClassName);
			if(index == 1)
				RenderUtils.AppendDefaultDXClassName(cell, DesignModeConstants.DotsNavigationBarSelectedItemClassName);
			return cell;
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(IsHorizontalPosition)
				LayoutTable.HorizontalAlign = HorizontalAlign.Center;
		}
	}
	[ToolboxItem(false)]
	public class NavigationButtons : InternalTable {
		protected ASPxImageSliderBase ImageSlider { get; private set; }
		protected bool IsHorizontal { get; private set; }
		public NavigationButtons(ASPxImageSliderBase imageSlider, bool isHorizontalOrientation) {
			ImageSlider = imageSlider;
			IsHorizontal = isHorizontalOrientation;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			if(IsHorizontal) {
				InternalTableRow row = CreateTableRow();
				InternalTableCell leftCell = CreateTableCell(row);
				leftCell.Controls.Add(new NavigationButton(ImageSlider, IsHorizontal, true));
				CreateSpacingCell(row);
				InternalTableCell rightCell = CreateTableCell(row);
				rightCell.Controls.Add(new NavigationButton(ImageSlider, IsHorizontal, false));
			} else {
				InternalTableCell prevButtonCell = CreateTableCell(CreateTableRow());
				prevButtonCell.Controls.Add(new NavigationButton(ImageSlider, IsHorizontal, true));
				CreateSpacingCell(CreateTableRow());
				InternalTableCell nextButtonCell = CreateTableCell(CreateTableRow());
				nextButtonCell.Controls.Add(new NavigationButton(ImageSlider, IsHorizontal, false));
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			RenderUtils.AppendDefaultDXClassName(this, DesignModeConstants.ButtonsClassName);
		}
		protected void CreateSpacingCell(InternalTableRow row) {
			InternalTableCell spacingCell = CreateTableCell(row);
			RenderUtils.AppendDefaultDXClassName(spacingCell, DesignModeConstants.ButtonsSpacingClassName);
		}
		protected InternalTableCell CreateTableCell(InternalTableRow row) {
			InternalTableCell cell = new InternalTableCell();
			row.Cells.Add(cell);
			return cell;
		}
		protected InternalTableRow CreateTableRow() {
			InternalTableRow row = new InternalTableRow();
			Rows.Add(row);
			return row;
		}
	}
	[ToolboxItem(false)]
	public class NavigationButton : ASPxInternalWebControl {
		protected ASPxImageSliderBase ImageSlider { get; private set; }
		protected WebControl Sprite { get; private set; }
		protected bool IsPrev { get; private set; }
		protected bool IsHorizontal { get; private set; }
		protected override bool HasRootTag() { return true; }
		protected override HtmlTextWriterTag TagKey {
			get { return HtmlTextWriterTag.Div; }
		}
		public NavigationButton(ASPxImageSliderBase imageSlider, bool isHorizontal, bool isPrev) {
			ImageSlider = imageSlider;
			IsHorizontal = isHorizontal;
			IsPrev = isPrev;
		}
		protected override void CreateControlHierarchy() {
			base.CreateControlHierarchy();
			Sprite = RenderUtils.CreateWebControl(HtmlTextWriterTag.Div);
			Controls.Add(Sprite);
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			DesignModeUtils.AppendGrayBorder(this);
			DesignModeUtils.AppendBackgroundImage(Sprite, ImageSlider);
			RenderUtils.AppendDefaultDXClassName(this, DesignModeConstants.ButtonClassName);
			RenderUtils.AppendDefaultDXClassName(this, IsPrev ? DesignModeConstants.PrevButtonClassName : DesignModeConstants.NextButtonClassName);
			RenderUtils.AppendDefaultDXClassName(this, IsHorizontal ? DesignModeConstants.ButtonHorizontalOrientationClassName : DesignModeConstants.ButtonVerticalOrientationClassName);
		}
	}
}
