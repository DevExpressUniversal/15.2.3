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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.Utils.Paint;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Helpers;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid.Selection;
namespace DevExpress.XtraPivotGrid.ViewInfo {
	public class PivotGridEditCellDataProvider : PivotGridCellDataProvider {
		public new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		public PivotGridEditCellDataProvider(PivotGridViewInfoData data) : base(data) { }
		public override PivotCellValue GetCellValueEx(PivotGridCellItem cellItem) {
			PivotCellValue value = base.GetCellValueEx(cellItem);
			if(cellItem == null || Data.GetCellEdit(cellItem) == null || Data.IsLocked)
				return value;
			object customValue = Data.CustomEditValue(value != null ? value.Value : null, cellItem);
			return new PivotCellValue(customValue);
		}
	}
	public class PivotCellViewInfo : PivotCellViewInfoBase {
		List<PivotGridFormatRule> rules;
		public PivotCellViewInfo(PivotGridCellDataProviderBase dataProvider, PivotFieldsAreaCellViewInfo columnViewInfoValue, PivotFieldsAreaCellViewInfo rowViewInfoValue, int columnIndex, int rowIndex, Dictionary<PivotFieldItem, List<PivotGridFormatRule>> rules)
			: base(dataProvider, columnViewInfoValue, rowViewInfoValue, columnIndex, rowIndex) {
			if(rules != null && DataField != null)
				rules.TryGetValue(DataField, out this.rules);
		}
		protected PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)Data.ViewInfo; } }
		public BaseEditViewInfo EditViewInfo {
			get {
				RepositoryItem edit = Edit;
				if(edit != null) {
					BaseEditViewInfo info = ViewInfo.GetEditViewInfo(edit);
					info.DefaultBorderStyle = BorderStyles.NoBorder;
					return info;
				} else
					return null;
			}
		}
		public DetailLevel DetailLevel {
			get { return ViewInfo.GetEditDetailLevel(this); }
		}
		public RepositoryItem Edit { get { return Data.GetCellEdit(this); } }
		protected new PivotFieldsAreaCellViewInfo ColumnViewInfoValue { get { return (PivotFieldsAreaCellViewInfo)base.ColumnViewInfoValue; } }
		protected new PivotFieldsAreaCellViewInfo RowViewInfoValue { get { return (PivotFieldsAreaCellViewInfo)base.RowViewInfoValue; } }
		public new PivotFieldItem ColumnField { get { return ColumnViewInfoValue.ColumnField; } }
		public new PivotFieldItem RowField { get { return RowViewInfoValue.RowField; } }
		public new PivotFieldItem DataField { get { return (PivotFieldItem)base.DataField; } }
		public Rectangle PaintBounds {
			get {
				Rectangle res = new Rectangle(ColumnViewInfoValue.PaintBounds.X, RowViewInfoValue.PaintBounds.Y,
					ColumnViewInfoValue.PaintBounds.Width, RowViewInfoValue.PaintBounds.Height);
				if(res.Width == 0)
					res.Width = ColumnViewInfoValue.Bounds.Width;
				if(res.Height == 0)
					res.Height = RowViewInfoValue.Bounds.Height;
				return res;
			}
		}
		public Rectangle GetEditorBounds(Point offset, Size inc, bool checkRowTreeTotal) {
			Rectangle bounds = new Rectangle(ColumnViewInfoValue.PaintBounds.X + offset.X,
											 RowViewInfoValue.PaintBounds.Y + offset.Y,
											 ColumnViewInfoValue.Bounds.Width - inc.Width,
											 RowViewInfoValue.Bounds.Height - inc.Height);
			return bounds;
		}
		public void Draw(ViewInfoPaintArgs e, Rectangle cellBounds, AppearanceObject cellAppearance, bool drawFocusedCellRect) {
			if(Edit != null)
				DrawEditViewInfo(e, cellBounds, cellAppearance);
			else {
				if(ShowKPIGraphic) {
					cellBounds = DrawKPIGraphic(e, cellBounds, cellAppearance);
				} else
					DrawText(e, cellBounds, cellAppearance);
			}
			if(Focused && drawFocusedCellRect && e.IsFocused) {
				e.GraphicsCache.Paint.DrawFocusRectangle(e.Graphics, cellBounds, cellAppearance.GetForeColor(), cellAppearance.GetBackColor());
			}
		}
		protected Rectangle DrawKPIGraphic(ViewInfoPaintArgs e, Rectangle cellBounds, AppearanceObject cellAppearance) {
			cellAppearance.FillRectangle(e.GraphicsCache, cellBounds);
			Bitmap bitmap = Data.GetKPIBitmap(KPIGraphic, KPIValue);
			e.Graphics.DrawImage(bitmap,
					cellBounds.Left + (cellBounds.Width - bitmap.Width) / 2,
					cellBounds.Top + (cellBounds.Height - bitmap.Height) / 2);
			return cellBounds;
		}
		protected void DrawText(ViewInfoPaintArgs e, Rectangle cellBounds, AppearanceObject cellAppearance) {
			List<PivotGridFormatRule> toDraw = PrepareCellApparance(ref cellAppearance);
			cellAppearance.FillRectangle(e.GraphicsCache, cellBounds);
			bool drawValue = true;
			Rectangle drawBounds = new Rectangle(cellBounds.X + 1, cellBounds.Y + 1, cellBounds.Width - 2, cellBounds.Height - 2);
			bool contextImageDrawn = false;
			Rectangle imageBounds = Rectangle.Empty;
			foreach(PivotGridFormatRule rule in toDraw) {
				FormatRuleDrawArgs args = CreateFormatRuleDrawArgs(e, cellBounds, (d1, e1) => e1.DrawString(e.GraphicsCache, Text, GetTextBounds(cellBounds, imageBounds)), cellAppearance, rule);
				IFormatRuleDraw drawRule = (IFormatRuleDraw)rule.Rule;
				IFormatRuleContextImage supportContextImage = drawRule as IFormatRuleContextImage;
				if(supportContextImage == null) {
					drawRule.DrawOverlay(args);
				} else {
					if(!contextImageDrawn) {
						Image image = supportContextImage.GetContextImage(args);
						if(image != null) {
							int x = cellBounds.X + 4;
							int width = Math.Min(cellBounds.Width, image.Width);
							int height = Math.Min(cellBounds.Width, image.Height);
							int y = cellBounds.Y;
							if(cellBounds.Height > image.Height)
								y += (cellBounds.Height - image.Height) / 2;
							imageBounds = new Rectangle(x, y, width, height);
							e.GraphicsCache.Paint.DrawImage(e.GraphicsCache.Graphics, image, imageBounds);
							contextImageDrawn = true;
						}
					}
				}
				if(!drawRule.AllowDrawValue)
					drawValue = false;
			}
			if(!drawValue)
				return;
			TextBounds = GetTextBounds(cellBounds, imageBounds);
			cellAppearance.DrawString(e.GraphicsCache, Text, TextBounds);
		}
		Rectangle GetTextBounds(Rectangle cellBounds, Rectangle imageBounds) {
			int contextImageWidth = imageBounds.IsEmpty ? 0 : imageBounds.Width + 6;
			return new Rectangle(cellBounds.X + ViewInfo.FieldMeasures.LeftCellPadding + contextImageWidth, cellBounds.Y,
							cellBounds.Width - ViewInfo.FieldMeasures.LeftCellPadding - ViewInfo.FieldMeasures.RightCellPadding - contextImageWidth, cellBounds.Height);
		}
		FormatRuleDrawArgs CreateFormatRuleDrawArgs(ViewInfoPaintArgs e, Rectangle cellBounds, DrawAppearanceMethod method, AppearanceObject cellAppearance, PivotGridFormatRule rule) {
			return new FormatRuleDrawArgs(e.GraphicsCache, cellBounds, rule.ValueProvider) {
				OriginalContentAppearance = cellAppearance,
				OriginalContentPainter = method
			};
		}
		private List<PivotGridFormatRule> PrepareCellApparance(ref AppearanceObject cellAppearance) {
			List<PivotGridFormatRule> toDraw = new List<PivotGridFormatRule>();
			if(rules != null)
				foreach(PivotGridFormatRule rule in rules) {				   
					if(!rule.CheckValue(this))
						continue;
					IFormatRuleAppearance appearanceRule = rule.Rule as IFormatRuleAppearance;
					if(appearanceRule != null) {
						AppearanceObject appearance = appearanceRule.QueryAppearance(new FormatRuleAppearanceArgs(rule.ValueProvider, Value));
						if(Selected) {
							AppearanceObject ca = (AppearanceObject)appearance.Clone();
							ca.Combine(cellAppearance);
							cellAppearance = ca;
						} else
							cellAppearance.Combine(appearance);
					}
					IFormatRuleDraw drawRule = rule.Rule as IFormatRuleDraw;
					if(drawRule != null)
						toDraw.Add(rule);
					if(rule.StopIfTrue)
						break;
				}
			return toDraw;
		}
		protected void DrawEditViewInfo(ViewInfoPaintArgs e, Rectangle cellBounds, AppearanceObject cellAppearance) {
			List<PivotGridFormatRule> toDraw = PrepareCellApparance(ref cellAppearance);
			bool hasContextImage = false;
			BaseEditViewInfo viewInfo = EditViewInfo;
			Rectangle bounds = new Rectangle(cellBounds.X + 1, cellBounds.Y + 1, cellBounds.Width - 2, cellBounds.Height - 2);
			AppearanceObject savedStyle = viewInfo.PaintAppearance;
			viewInfo.PaintAppearance = cellAppearance;
			viewInfo.AllowDrawFocusRect = false;
			viewInfo.DefaultBorderStyle = BorderStyles.NoBorder;
			viewInfo.EditValue = Value;
			if(!viewInfo.IsReady)
				viewInfo.CalcViewInfo(e.Graphics);
			UpdateEditViewInfoCellFormat(viewInfo);
			viewInfo.DetailLevel = DetailLevel;
			viewInfo.SetDisplayText(Text);
			viewInfo.ReCalcViewInfo(e.Graphics, MouseButtons.None, Point.Empty, bounds);
			viewInfo.SetDisplayText(Text);
			viewInfo.FillBackground = false;
			viewInfo.Bounds = bounds;
			cellAppearance.FillRectangle(e.GraphicsCache, cellBounds);
			ControlGraphicsInfoArgs args1 = new ControlGraphicsInfoArgs(viewInfo, e.GraphicsCache, cellBounds);
			foreach(PivotGridFormatRule rule in toDraw) {
				IFormatRuleDraw drawRule = (IFormatRuleDraw)rule.Rule;
				IFormatRuleContextImage contextImage = rule.Rule as IFormatRuleContextImage;
				FormatRuleDrawArgs args = CreateFormatRuleDrawArgs(e, cellBounds, (e1, d1) => Data.PivotGrid.EditorHelper.DrawCellEdit(args1, Edit, viewInfo, d1, Point.Empty), cellAppearance, rule);
				if(contextImage == null) {
					drawRule.DrawOverlay(args);
					if(!drawRule.AllowDrawValue)
						return;
				} else {
					IFormatRuleSupportContextImage supportImage = viewInfo as IFormatRuleSupportContextImage;
					if(supportImage != null) {
						supportImage.SetContextImage(contextImage.GetContextImage(args));
						hasContextImage = true;
						if(!drawRule.AllowDrawValue)
							return;
					}
				}
			}
			try {
				viewInfo.Painter.Draw(args1);
			} finally {
				if(hasContextImage) {
					IFormatRuleSupportContextImage supportImage2 = viewInfo as IFormatRuleSupportContextImage;
					if(supportImage2 != null)
						supportImage2.SetContextImage(null);
				}
				viewInfo.PaintAppearance = savedStyle;
			}
		}
		void UpdateEditViewInfoCellFormat(BaseEditViewInfo editViewInfo) {
			FormatInfo cellFormat = GetCellFormatInfo();
			if(cellFormat != null && !cellFormat.IsEmpty && !PivotGridFieldBase.IsDefaultFormat(cellFormat)) {
				editViewInfo.Item.BeginUpdate();
				editViewInfo.Format.Assign(cellFormat);
				editViewInfo.Item.CancelUpdate();
			}
		}
		protected override string GetDisplayText(PivotCellValue cellValue) {
			string text = base.GetDisplayText(cellValue);
			BaseEditViewInfo info = EditViewInfo;
			if(info == null)
				return text;
			UpdateEditViewInfoCellFormat(info);
			info.EditValue = Value;
			return info.DisplayText;
		}
	}
	public abstract class PivotCellViewInfoBase : PivotGridCellItem {
		Rectangle bounds;
		Rectangle textBounds;
		AppearanceObject appearance;
		bool focused;
		bool selected;
		PivotFieldsAreaCellViewInfoBase columnViewInfoValue;
		PivotFieldsAreaCellViewInfoBase rowViewInfoValue;
		int isTextFit;
		public PivotCellViewInfoBase(PivotGridCellDataProviderBase dataProvider, PivotFieldsAreaCellViewInfoBase columnViewInfoValue, PivotFieldsAreaCellViewInfoBase rowViewInfoValue, int columnIndex, int rowIndex)
			: base(dataProvider, columnViewInfoValue.Item, rowViewInfoValue.Item, columnIndex, rowIndex) {
			this.columnViewInfoValue = columnViewInfoValue;
			this.rowViewInfoValue = rowViewInfoValue;
			this.appearance = new AppearanceObject();
			this.bounds = Rectangle.Empty;
			this.textBounds = Rectangle.Empty;
			this.focused = false;
			this.selected = false;
			this.isTextFit = -1;
		}
		public new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		public AppearanceObject Appearance {
			get { return appearance; }
			set {
				if(value == null)
					return;
				this.appearance = value;
			}
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle TextBounds {
			get { return textBounds.IsEmpty ? Bounds : textBounds; }
			set { textBounds = value; }
		}
		public bool Focused { get { return focused; } set { focused = value; } }
		public bool Selected { get { return selected; } set { selected = value; } }
		public FormatType FormatType {
			get {
				FormatInfo formatInfo = GetCellFormatInfo();
				return formatInfo != null ? formatInfo.FormatType : FormatType.None;
			}
		}
		public bool ShowTooltip {
			get { return !IsTextFit || ShowKPIGraphic; }
		}
		public string TooltipText {
			get {
				if(!ShowKPIGraphic)
					return Text;
				return PivotGridData.GetKPITooltipText(DataField.KPIType, KPIValue);
			}
		}
		public bool IsTextFit {
			get {
				if(isTextFit < 0) {
					if(FieldItems.DataFieldCount == 0 && !PivotCellsViewInfoBase.GetShowDragDataCaption(Data))
						return true;
					string CellText = FieldItems.DataFieldCount == 0 ? PivotGridLocalizer.GetHeadersAreaText((int)PivotArea.DataArea) : Text;
					GraphicsInfo ginfo = new GraphicsInfo();
					ginfo.AddGraphics(null);
					Size size = Appearance.CalcTextSize(ginfo.Graphics, CellText, 0).ToSize();
					ginfo.ReleaseGraphics();
					isTextFit = TextBounds.Contains(new Rectangle(TextBounds.Location, size)) ? 1 : 0;
				}
				return isTextFit > 0 ? true : false;
			}
		}
		protected PivotFieldsAreaCellViewInfoBase ColumnViewInfoValue { get { return columnViewInfoValue; } }
		protected PivotFieldsAreaCellViewInfoBase RowViewInfoValue { get { return rowViewInfoValue; } }
	}
	public class PivotCellsViewInfo : PivotCellsViewInfoBase {
		SelectionScroller selectionScroller;
		readonly PivotVisualItems visualItems;
		public PivotCellsViewInfo(PivotGridViewInfo viewInfo)
			: base(viewInfo) {
			this.visualItems = VisualItems;
			SubscribeEvents();
		}
		public override void Dispose() {
			UnsubscribeEvents();
			StopScrollTimer();
			DoubleClickChecker.Reset();
			base.Dispose();
		}
		protected new PivotGridViewInfo ViewInfo { get { return (PivotGridViewInfo)base.ViewInfo; } }
		protected new PivotGridViewInfoData Data { get { return (PivotGridViewInfoData)base.Data; } }
		protected new PivotVisualItems VisualItems { get { return (PivotVisualItems)base.VisualItems; } }
		protected internal SelectionScroller Scroller {
			get {
				if(selectionScroller == null)
					selectionScroller = new SelectionScroller(this);
				return selectionScroller;
			}
		}
		public override void InvalidatedCell(Point cell) {
			if(!VisualItems.IsCellValid(cell))
				return;
			Rectangle bounds = GetCellBounds(cell);
			Data.Invalidate(bounds);
		}
		public override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			if(!Data.OptionsHint.ShowCellHints)
				return null;
			PivotCellViewInfo cellViewInfo = GetCellViewInfoAt(pt);
			if(cellViewInfo != null && cellViewInfo.ShowTooltip) {
				return new ToolTipControlInfo(cellViewInfo, FieldItems.DataFieldCount == 0 ? GetLocalizedString(PivotGridStringId.DataHeadersCustomization) : cellViewInfo.TooltipText);
			}
			return null;
		}
		Point curr = Point.Empty;
		protected override BaseViewInfo MouseDownCore(MouseEventArgs e) {
			curr = e.Location;
			if(e.Button == MouseButtons.Right) {
				ShowPopupMenu(e);
				return null;
			}
			VisualItems.OnCellMouseDown(GetCellAt(e.Location));
			return this;
		}
		protected override Control GetControlOwner() {
			return Data.ControlOwner;
		}
		protected override PivotGridMenuEventArgsBase CreateMenuEventArgs() {
#pragma warning disable 618 // Obsolete
#pragma warning disable 612 // Obsolete
			return Data.EventArgsHelper.CreateMenuEventArgs(menu, MenuType, (PivotGridField)Data.GetField(MenuField), MenuArea, menuLocation);
#pragma warning restore 618 // Obsolete
#pragma warning restore 612 // Obsolete
		}
		protected override void CreatePopupMenuItems(DevExpress.Utils.Menu.DXPopupMenu menu) {
			base.CreatePopupMenuItems(menu);
			if(!Data.OptionsMenu.EnableFormatRulesMenu)
				return;
			PivotCellViewInfo cell = GetCellViewInfoAt(curr);
			if(cell == null || cell.DataField == null)
				return;
			DXSubMenuItem conditionalFormattingItem = CreateSubMenuItem(MenuImages.Images[5]);
			PivotGridFormatRuleMenuItems formatRuleItems = new PivotGridFormatRuleMenuItems(Data, cell, conditionalFormattingItem.Items);
			string clearCaption = Localizer.Active.GetLocalizedString(StringId.FormatRuleMenuItemClearRules);
			if(formatRuleItems.Count > 0) {
				foreach(DXMenuItem item in conditionalFormattingItem.Items)
					if(item.Caption == clearCaption)
						formatRuleItems.ConfigureClearFormatRulesItem(item as DXSubMenuItem);
				menu.Items.Add((DXMenuItem)conditionalFormattingItem);
			}
		}
		protected virtual DXSubMenuItem CreateSubMenuItem(Image image) {
			return CreateSubMenuItem(PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRules), image, true, false);
		}
		protected virtual DXSubMenuItem CreateSubMenuItem(string caption, Image image, bool enabled, bool beginGroup) {
			DXSubMenuItem item = new DXSubMenuItem(caption);
			item.Image = image;
			item.Enabled = enabled;
			item.BeginGroup = beginGroup;
			return item;
		}
		protected override PivotGridMenuType MenuType {
			get {
				return PivotGridMenuType.Cell;
			}
		}
		protected override void OnMenuItemClick(DevExpress.Utils.Menu.DXMenuItem menuItem) {
			base.OnMenuItemClick(menuItem);
		}
		protected override void MouseUpCore(MouseEventArgs e) {
			VisualItems.OnCellMouseUp();
			StopScrollTimer();
			PivotCellViewInfo cellViewInfo = GetCellViewInfoAt(e.Location);
			if(VisualItems.Selection == Rectangle.Empty) {
				if(cellViewInfo != null)
					if(DoubleClickChecker.CheckDoubleClick(cellViewInfo, e)) {
						Data.CellDoubleClick(cellViewInfo);
					} else {
						Data.CellClick(cellViewInfo);
					}
			}
			base.MouseUpCore(e);
		}
		protected override void MouseMoveCore(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left || !VisualItems.IsCellMouseDown)
				return;
			StartScrollTimer(new Point(e.X, e.Y));
			VisualItems.OnCellMouseMove(GetCellAt(e.Location));
		}
		protected override void OnGestureTwoFingerSelectionCore(Point start, Point end) {
			VisualItems.OnCellGestureTwoFingerSelection(GetCellAt(start), GetCellAt(end));
		}
		void StartScrollTimer(Point pt) {
			Scroller.StartScrollTimer(pt);
		}
		void StopScrollTimer() {
			if(this.selectionScroller != null)
				this.selectionScroller.StopScrollTimer();
		}
		protected override bool GetIsCellFocused(PivotCellViewInfo cell) {
			return Data.VisualItems.IsCellFocused(cell);
		}
		protected override bool GetIsCellSelected(PivotCellViewInfo cell) {
			return Data.VisualItems.IsCellSelected(cell);
		}
		internal protected override void CombineWithSelectAppearance(PivotGridCellItem cell, AppearanceObject cellAppearance) {
			if(Data.VisualItems.IsCellFocused(cell) && Data.OptionsSelection.EnableAppearanceFocusedCell) {
				AppearanceHelper.Combine(cellAppearance, new AppearanceObject[] { Data.PaintAppearance.FocusedCell, cellAppearance });
			} else {
				if(Data.VisualItems.IsCellSelected(cell)) {
					bool blendBackColor = cellAppearance.Options.UseBackColor && Data.PaintAppearance.SelectedCell.Options.UseBackColor;
					bool blendForeColor = cellAppearance.Options.UseForeColor && Data.PaintAppearance.SelectedCell.Options.UseForeColor;
					Color backColor = DXColor.Blend(Data.PaintAppearance.SelectedCell.BackColor, cellAppearance.BackColor);
					Color foreColor = DXColor.Blend(Data.PaintAppearance.SelectedCell.ForeColor, cellAppearance.ForeColor);
					AppearanceHelper.Combine(cellAppearance, new AppearanceObject[] { Data.PaintAppearance.SelectedCell, cellAppearance });
					if(blendForeColor)
						cellAppearance.ForeColor = foreColor;
					if(blendBackColor)
						cellAppearance.BackColor = backColor;
				}
			}
		}
		internal protected override bool CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject cellAppearance, PivotCellViewInfo cellViewInfo, MethodInvoker defaultDraw) {
			return Data.CustomDrawCell(paintArgs, ref cellAppearance, cellViewInfo, defaultDraw);
		}
		protected override PivotCellViewInfo CreateCellViewInfoCore(PivotFieldsAreaCellViewInfoBase columnViewInfoValue, PivotFieldsAreaCellViewInfoBase rowViewInfoValue, int col, int row) {
			return new PivotCellViewInfo(VisualItems.CellDataProvider, (PivotFieldsAreaCellViewInfo)columnViewInfoValue, (PivotFieldsAreaCellViewInfo)rowViewInfoValue, col, row, formatRules);
		}
		protected void SubscribeEvents() {
			this.visualItems.FocusedCellChanged += OnFocusedCellChanged;
			this.visualItems.SelectionChanged += OnSelectionChanged;
		}
		protected void UnsubscribeEvents() {
			this.visualItems.FocusedCellChanged -= OnFocusedCellChanged;
			this.visualItems.SelectionChanged -= OnSelectionChanged;
		}
		protected void OnFocusedCellChanged(object sender, FocusedCellChangedEventArgs e) {
			OnFocusedCellChanged(e.OldCell, e.NewCell);
		}
		protected void OnSelectionChanged(object sender, EventArgs e) {
			OnSelectionChanged();
		}
		public class SelectionScroller : SelectionScrollerBase {
			PivotCellsViewInfo viewInfo;
			public SelectionScroller(PivotCellsViewInfo viewInfo)
				: base(viewInfo.Data.VisualItems) {
				this.viewInfo = viewInfo;
			}
			protected override Point GetLeftTopCoordOffset(Point pt) {
				return this.viewInfo.GetSelectionOffset(pt);
			}
			protected override Point LeftTopCoord {
				get { return this.viewInfo.LeftTopCoord; }
				set { this.viewInfo.LeftTopCoord = value; }
			}
			protected override Point GetCellAt(Point pt) {
				return this.viewInfo.GetCellAt(pt);
			}
		}
		public delegate void DoubleClickHandler(PivotCellViewInfo e);
		public static class DoubleClickChecker {
			const long tickLength = 0x2710L;
			static Point lastMouseDowntPoint;
			static long lastMouseDownTime;
			static Point lastMouseDownCell;
			static bool waitSecondClick;
			static DoubleClickChecker() {
				waitSecondClick = false;
			}
			public static bool CheckDoubleClick(PivotCellViewInfo cell, MouseEventArgs e) {
				if(IsLocked)
					return false;
				if((e.Button & MouseButtons.Left) == 0) {
					waitSecondClick = false;
					return false;
				}
				Point cellPoint = new Point(cell.ColumnIndex, cell.RowIndex);
				if(waitSecondClick && CheckTimeCondition(DateTime.Now.Ticks) && CheckSameCellCondition(cellPoint) && CheckLocationCondition(e.Location)) {
					return true; 
				}
				SaveFirstClick(cell, e.Location, DateTime.Now.Ticks);
				waitSecondClick = true;
				return false;
			}
			public static void Reset() {
				lastMouseDownCell = Point.Empty;
				lastMouseDowntPoint = Point.Empty;
				lastMouseDownTime = 0;
				waitSecondClick = false;
			}
			static void SaveFirstClick(PivotCellViewInfo cell, Point point, long time) {
				lastMouseDownCell = new Point(cell.ColumnIndex, cell.RowIndex);
				lastMouseDowntPoint = point;
				lastMouseDownTime = time;
			}
			static bool CheckSameCellCondition(Point point) {
				bool condition = lastMouseDownCell == point;
				lastMouseDownCell = point;
				return condition;
			}
			static bool CheckTimeCondition(long currentMouseDownTime) {
				bool condition = (Math.Abs(lastMouseDownTime - currentMouseDownTime) / tickLength) < SystemInformation.DoubleClickTime;
				lastMouseDownTime = currentMouseDownTime;
				return condition;
			}
			static bool CheckLocationCondition(Point currentMouseEventPoint) {
				bool conditionX = Math.Abs(currentMouseEventPoint.X - lastMouseDowntPoint.X) <= SystemInformation.DoubleClickSize.Width;
				bool conditionY = Math.Abs(currentMouseEventPoint.Y - lastMouseDowntPoint.Y) <= SystemInformation.DoubleClickSize.Height;
				lastMouseDowntPoint = currentMouseEventPoint;
				return conditionX && conditionY;
			}
			static int lockCounter = 0;
			static bool IsLocked {
				get { return lockCounter > 0; }
			}
			public static void Lock() {
				lockCounter++;
			}
			public static void Unlock() {
				if(--lockCounter == 0)
					Reset();
			}
		}
	}
	public abstract class PivotCellsViewInfoBase : PivotViewInfo, ICellsBestFitProvider {
		public delegate void CellCreatedDelegate(PivotCellViewInfo cell);
		public static bool GetShowDragDataCaption(PivotGridData data) {
			if(data == null)
				return false;
			PivotFieldItemCollection FieldItems = data.FieldItems;
			if(FieldItems != null)
				foreach(PivotFieldItemBase field in FieldItems)
					if(field.CanDrag || field.CanDragInCustomizationForm)
						return true;
			if(FieldItems.Count == 0)
				return data.OptionsCustomization.AllowDrag || data.OptionsCustomization.AllowDragInCustomizationForm;
			return false;
		}
		float GetFontHeight(Graphics graphics, Font origionalFont, string text, StringFormat format, Size size) {
			const float MaxFontSize = 16, MinFontSize = 5;
			float fontSize = origionalFont.Size;
			size.Width -= FieldMeasures.LeftCellPadding + FieldMeasures.RightCellPadding;
			size.Height -= FieldMeasures.TopCellPadding + FieldMeasures.BottomCellPadding;
			Size testSize = XPaint.TextSizeRound(XPaint.Graphics.CalcTextSize(graphics, text, origionalFont, format, 0));
			float dx = testSize.Width > size.Width && testSize.Height > size.Height ? -1 : 1;
			while(dx > 0 ? fontSize < MaxFontSize : fontSize > MinFontSize) {
				Font testFont = new Font(origionalFont.FontFamily, fontSize + dx, origionalFont.Style);
				testSize = XPaint.TextSizeRound(XPaint.Graphics.CalcTextSize(graphics, text, testFont, format, 0));
				if(dx > 0) {
					if(testSize.Width > size.Width || testSize.Height > size.Height)
						break;
				} else {
					if(testSize.Width <= size.Width && testSize.Height <= size.Height)
						break;
				}
				fontSize += dx;
			}
			return fontSize;
		}
		ArrayList rowLines;
		ArrayList columnLines;
		List<PivotCellViewInfo> cells;
		int cellsBottom;
		public PivotCellsViewInfoBase(PivotGridViewInfoBase viewInfo)
			: base(viewInfo) {
			this.rowLines = new ArrayList();
			this.columnLines = new ArrayList();
			this.cells = new List<PivotCellViewInfo>();
			this.cellsBottom = 0;
		}
		public int ColumnCount { get { return GetFieldsArea(true).LastLevelItemCount; } }
		public int RowCount { get { return GetFieldsArea(false).LastLevelItemCount; } }
		public int TotalHeight { get { return GetTotalSize(false, int.MaxValue); } }
		public int TotalWidth { get { return GetTotalSize(true, int.MaxValue); } }
		public Point LeftTopCoord {
			get { return ViewInfo.LeftTopCoord; }
			set { ViewInfo.LeftTopCoord = value; }
		}
		public int GetVisibleWidth() {
			int actualWidth = ViewInfo.Bounds.Right - LeftVisibleCoord;
			return Math.Min(Bounds.Width, actualWidth);
		}
		internal Point FirstCellOffSet {
			get {
				return new Point(Bounds.X - ViewInfo.Scroller.BoundsOffsetX, Bounds.Y - ViewInfo.Scroller.BoundsOffsetY);
			}
		}
		internal int LeftVisibleCoord {
			get {
				int coord = Bounds.X;
				if(ViewInfo.IsHorzScrollControl)
					coord -= ViewInfo.Scroller.BoundsOffsetX;
				return coord > ViewInfo.PivotScrollableRectangle.Top ? coord : ViewInfo.PivotScrollableRectangle.Top;
			}
		}
		internal int TopVisibleCoord { get { return Bounds.Y > ViewInfo.PivotScrollableRectangle.Top ? Bounds.Y : ViewInfo.PivotScrollableRectangle.Top; } }
		public virtual void InvalidatedCell(Point cell) {  }
		public override bool AcceptDragDrop { get { return true; } }
		public override PivotGridHitInfo CalcHitInfo(Point hitPoint) {
			PivotCellViewInfo cellViewInfo = GetCellViewInfoAt(hitPoint);
			return cellViewInfo != null ? new PivotGridHitInfo(cellViewInfo, hitPoint, (PivotGridViewInfo)ViewInfo) : new PivotGridHitInfo(hitPoint);
		}
		public override Rectangle GetDragDrawRectangle(PivotFieldItem field, Point pt) {
			return PaintBounds;
		}
		public override int GetNewFieldPosition(PivotFieldItem field, Point pt, out PivotArea area) {
			area = PivotArea.DataArea;
			return FieldItems.DataFieldCount;
		}
		public PivotCellViewInfo GetCellViewInfoAt(Point pt) {
			if(this.cells.Count == 0)
				CalculateCellsViewInfo();
			return this.cells.Find(new Predicate<PivotCellViewInfo>(
				delegate(PivotCellViewInfo cell) {
					return cell.Bounds.Contains(pt);
				}
			));
		}
		public PivotCellViewInfo GetCellViewInfoByCoord(int columnIndex, int rowIndex) {
			if(this.cells.Count == 0)
				CalculateCellsViewInfo();
			return this.cells.Find(new Predicate<PivotCellViewInfo>(
				delegate(PivotCellViewInfo cell) {
					return cell.ColumnIndex == columnIndex && cell.RowIndex == rowIndex;
				}
			));
		}
		public Point GetCellCoordAt(Point pt) {
			PivotCellViewInfo viewInfo = GetCellViewInfoAt(pt);
			return viewInfo == null ? SelectionVisualItems.EmptyCoord : new Point(viewInfo.ColumnIndex, viewInfo.RowIndex);
		}
		public Rectangle GetCellBounds(Point cell) {
			PivotCellViewInfo visibleCell = GetCellViewInfoByCoord(cell.X, cell.Y);
			return visibleCell != null ? visibleCell.Bounds : Rectangle.Empty;
		}
		protected internal int GetCellWidth(PivotFieldsAreaCellViewInfoBase columnViewInfoValue) {
			if(columnViewInfoValue == null)
				return FieldMeasures.DefaultFieldWidth;
			return columnViewInfoValue.Bounds.Width;
		}
		protected internal int GetCellHeight(PivotFieldsAreaCellViewInfoBase rowViewInfoValue) {
			if(rowViewInfoValue == null)
				return FieldMeasures.DefaultFieldValueHeight;
			return rowViewInfoValue.Bounds.Height;
		}
		protected internal PivotFieldsAreaCellViewInfoBase GetRowValue(int index) {
			return GetValue(false, index);
		}
		protected internal PivotFieldsAreaCellViewInfoBase GetColumnValue(int index) {
			return GetValue(true, index);
		}
		protected PivotFieldsAreaCellViewInfoBase GetValue(bool isColumn, int index) {
			return GetFieldsArea(isColumn).GetLastLevelViewInfo(index);
		}
		protected PivotFieldsAreaViewInfoBase GetFieldsArea(bool isColumn) {
			return ViewInfo.GetFieldsArea(isColumn);
		}
		protected int LineWidth { get { return 1; } }
		int[] totalSize = new int[] { -1, -1 };
		public int GetTotalSize(bool isColumn, int maxCount) {
			if(maxCount != int.MaxValue)
				return GetTotalSizeCore(isColumn, maxCount);
			int index = isColumn ? 1 : 0;
			if(totalSize[index] < 0)
				totalSize[index] = GetTotalSizeCore(isColumn, maxCount);
			return totalSize[index];
		}
		int GetTotalSizeCore(bool isColumn, int maxCount) {
			return isColumn ?
				FieldMeasures.GetWidthDifference(true, 0, Math.Min(maxCount, GetFieldsArea(isColumn).LastLevelItemCount)) :
				FieldMeasures.GetHeightDifference(false, 0, Math.Min(maxCount, GetFieldsArea(isColumn).LastLevelItemCount));
		}
		protected void ResetTotalSizeCache() {
			this.totalSize[0] = -1;
			this.totalSize[1] = -1;
		}
		protected AppearanceObject CellAppearance { get { return ViewInfo.PrintAndPaintAppearance.Cell; } }
		protected AppearanceObject TotalAppearance { get { return ViewInfo.PrintAndPaintAppearance.GetTotalCellAppearance(); } }
		protected AppearanceObject GrandTotalAppearance { get { return ViewInfo.PrintAndPaintAppearance.GetGrandTotalCellAppearance(); } }
		protected int GetColumnIndexAt(int x, List<PivotCellViewInfo> cells) {
			foreach(PivotCellViewInfo cell in cells) {
				if(x >= cell.Bounds.X && x <= cell.Bounds.Right)
					return cell.ColumnIndex;
			}
			return -1;
		}
		protected internal int GetRowIndexAt(int y, List<PivotCellViewInfo> cells) {
			foreach(PivotCellViewInfo cell in cells) {
				if(y >= cell.Bounds.Y && y <= cell.Bounds.Bottom)
					return cell.RowIndex;
			}
			return -1;
		}
		public Point GetCellAt(Point pt) {
			if(cells.Count == 0)
				return Point.Empty;
			int x = GetColumnIndexAt(pt.X, cells), y = GetRowIndexAt(pt.Y, cells);
			if(x < 0) {
				if(IsPointNearerThenNearBorder(pt.X, PaintBounds))
					x = this.cells[0].ColumnIndex;
				else
					x = this.cells[cells.Count - 1].ColumnIndex;
			}
			if(y < 0)
				y = pt.Y < Bounds.Y ? this.cells[0].RowIndex : this.cells[cells.Count - 1].RowIndex;
			return new Point(x, y);
		}
		public virtual bool ShowVertLines { get { return Data.OptionsView.ShowVertLines; } }
		public virtual bool ShowHorzLines { get { return Data.OptionsView.ShowHorzLines; } }
		protected virtual bool DrawFocusedCellRect { get { return Data.OptionsView.DrawFocusedCellRect; } }
		protected override void InternalPaint(ViewInfoPaintArgs e) {
			CalculateCellsViewInfo();
			GraphicsInfoState state = e.GraphicsCache.ClipInfo.SaveState();
			if(!ViewInfo.IsHorzScrollControl)
				e.GraphicsCache.ClipInfo.SetClip(PaintBounds);
			if(FieldItems.DataFieldCount == 0)
				DrawEmptyCells(e);
			else
				DrawDataCells(e);
			e.GraphicsCache.ClipInfo.RestoreState(state);
		}
		Point lastXY;
		Point lastLeftTop;
		Size lastViewInfoSize;
		protected Dictionary<PivotFieldItem, List<PivotGridFormatRule>> formatRules;
		public void CalculateCellsViewInfo() {
			if(lastLeftTop != LeftTopCoord || lastXY != ControlBounds.Location || lastViewInfoSize != ViewInfo.ControlBounds.Size) {
				lastLeftTop = LeftTopCoord;
				lastXY = ControlBounds.Location;
				lastViewInfoSize = ViewInfo.ControlBounds.Size;
				this.cells.Clear();
				this.rowLines.Clear();
				this.columnLines.Clear();
				this.cellsBottom = 0;
				ResetTotalSizeCache();
				formatRules = null;
			}
			if(this.cells.Count == 0) {
				formatRules = new Dictionary<PivotFieldItem, List<PivotGridFormatRule>>();
				foreach(PivotGridFormatRule rule in Data.FormatRules) {
					if(!rule.IsValid)
						continue;
					List<PivotGridFormatRule> fRules = null;
					if(!formatRules.TryGetValue(Data.GetFieldItem(rule.Measure), out fRules)) {
						fRules = new List<PivotGridFormatRule>();
						formatRules.Add(Data.GetFieldItem(rule.Measure), fRules);
					}
					fRules.Add(rule);
				}
				CreateCellsViewInfo(FirstCellOffSet, new Point(ViewInfo.Bounds.Right, ViewInfo.Bounds.Bottom), this.cells, this.columnLines, this.rowLines, out cellsBottom, null);
				SetFocusAndSelectionCell(this.cells);
				ResetTotalSizeCache();
				foreach(PivotCellViewInfo cell in this.cells)
					cell.Bounds = RightToLeftRect(cell.Bounds);
				for(int i = 0; i < columnLines.Count; i++)
					columnLines[i] = RightToLeftRect((Rectangle)columnLines[i]);
				for(int i = 0; i < rowLines.Count; i++)
					rowLines[i] = RightToLeftRect((Rectangle)rowLines[i]);
			}
		}
		protected virtual void DrawDataCells(ViewInfoPaintArgs e) {
			DrawCells(e, this.cells);
			DrawLines(e, this.rowLines, -1);
			DrawLines(e, this.columnLines, this.cellsBottom - Bounds.Y);
		}
		protected virtual void DrawEmptyCells(ViewInfoPaintArgs e) {
			Rectangle dataBounds = new Rectangle(LeftVisibleCoord, TopVisibleCoord, GetVisibleWidth(), ViewInfo.Bounds.Bottom - TopVisibleCoord);
			if(dataBounds.Bottom > ViewInfo.RowAreaFields.Bounds.Bottom)
				dataBounds.Height = ViewInfo.RowAreaFields.Bounds.Bottom - dataBounds.Y;
			dataBounds = RightToLeftRect(dataBounds);
			CellAppearance.FillRectangle(e.GraphicsCache, dataBounds);
			if(PivotCellsViewInfoBase.GetShowDragDataCaption(Data)) {
				StringFormat format = new StringFormat();
				format.Alignment = StringAlignment.Center;
				format.LineAlignment = StringAlignment.Center;
				string text = PivotGridLocalizer.GetString(PivotGridStringId.DataHeadersCustomization);
				Font font = new Font(CellAppearance.Font.FontFamily, GetFontHeight(e.Graphics, CellAppearance.Font, text, format, dataBounds.Size), CellAppearance.Font.Style);
				CellAppearance.DrawString(e.GraphicsCache, text, dataBounds, font, format);
				font.Dispose();
			}
			DrawEmptyLines(e);
		}
		protected virtual void DrawEmptyLines(ViewInfoPaintArgs e) {
			int right = Math.Min(ViewInfo.ColumnAreaFields.ControlBounds.Right, ViewInfo.Bounds.Right);
			int bottom = Math.Min(ViewInfo.RowAreaFields.ControlBounds.Bottom, ViewInfo.Bounds.Bottom);
			if(ViewInfo.RowAreaFields.ControlBounds.Bottom < ViewInfo.Bounds.Bottom)
				Data.PaintAppearance.Lines.FillRectangle(e.GraphicsCache, RightToLeftRect(new Rectangle(FirstCellOffSet.X, bottom - LineWidth, right - FirstCellOffSet.X, LineWidth)));
			if(ViewInfo.RowAreaFields.ControlBounds.Right < ViewInfo.Bounds.Right)
				Data.PaintAppearance.Lines.FillRectangle(e.GraphicsCache, RightToLeftRect(new Rectangle(right - LineWidth, Bounds.Y, LineWidth, bottom - Bounds.Y)));
		}
		public PivotCellViewInfoBase CreateCellViewInfo(int columnIndex, int rowIndex) {
			if(columnIndex < 0 || columnIndex >= ColumnCount)
				return null;
			if(rowIndex < 0 || rowIndex >= RowCount)
				return null;
			PivotFieldsAreaCellViewInfoBase columnViewInfoValue = GetColumnValue(columnIndex);
			PivotFieldsAreaCellViewInfoBase rowViewInfoValue = GetRowValue(rowIndex);
			return CreateCellViewInfo(columnViewInfoValue, rowViewInfoValue, columnIndex, rowIndex);
		}
		bool IsPointNearerThenNearBorder(int x, Rectangle rect) {
			return IsRightToLeft ?
				x > rect.Right :
				x < rect.X;
		}
		bool IsPointFartherThenFarBorder(int x, Rectangle rect) {
			return IsRightToLeft ?
				x < rect.X :
				x > rect.Right;
		}
		public Point GetSelectionOffset(Point pt) {
			Point offset = Point.Empty;
			if(ViewInfo.IsHScrollBarVisible) {
				if(IsPointNearerThenNearBorder(pt.X, PaintBounds))
					offset.X = -ViewInfo.Scroller.ScrollSmallChange;
				if(IsPointFartherThenFarBorder(pt.X, PaintBounds))
					offset.X = ViewInfo.Scroller.ScrollSmallChange;
			}
			if(ViewInfo.IsVScrollBarVisible) {
				if(pt.Y < Bounds.Y)
					offset.Y = -ViewInfo.Scroller.ScrollSmallChange;
				if(pt.Y > ViewInfo.Bounds.Bottom)
					offset.Y = ViewInfo.Scroller.ScrollSmallChange;
			}
			return offset;
		}
		public void CreateCellsViewInfo(Point leftTop, Point rightBottom, List<PivotCellViewInfo> cells, ArrayList colLines, ArrayList rowLines, out int cellsBottom, CellCreatedDelegate onCell) {
			int y = leftTop.Y;
			for(int row = 0; row < RowCount; row++) {
				PivotFieldsAreaCellViewInfoBase rowViewInfoValue = GetRowValue(row);
				if(rowViewInfoValue != null)
					y += rowViewInfoValue.Separator;
				int cellHeight = GetCellHeight(rowViewInfoValue);
				if(y >= rightBottom.Y)
					break;
				int x = leftTop.X;
				if(y + cellHeight > TopVisibleCoord) {
					for(int col = 0; col < ColumnCount; col++) {
						PivotFieldsAreaCellViewInfoBase columnViewInfoValue = GetColumnValue(col);
						if(columnViewInfoValue != null)
							x += columnViewInfoValue.Separator;
						int cellWidth = GetCellWidth(columnViewInfoValue);
						if(x >= rightBottom.X)
							break;
						if(x + cellWidth > LeftVisibleCoord) {
							PivotCellViewInfo cell = CreateCellViewInfo(columnViewInfoValue, rowViewInfoValue, col, row);
							cell.Bounds = new Rectangle(x, y, cellWidth, cellHeight);
							if(cells != null)
								cells.Add(cell);
							if(onCell != null)
								onCell(cell);
						}
						x += cellWidth;
						if(colLines != null && ShowVertLines)
							colLines.Add(new Rectangle(x - LineWidth, ControlBounds.Y, LineWidth, 0));
					}
				}
				y += cellHeight;
				if(rowLines != null && ShowHorzLines)
					rowLines.Add(new Rectangle(ControlBounds.X, y - LineWidth, x - ControlBounds.X, LineWidth));
			}
			cellsBottom = y;
		}
		protected abstract PivotCellViewInfo CreateCellViewInfoCore(PivotFieldsAreaCellViewInfoBase columnViewInfoValue, PivotFieldsAreaCellViewInfoBase rowViewInfoValue, int col, int row);
		protected PivotCellViewInfo CreateCellViewInfo(PivotFieldsAreaCellViewInfoBase columnViewInfoValue, PivotFieldsAreaCellViewInfoBase rowViewInfoValue, int col, int row) {
			PivotCellViewInfo cell = CreateCellViewInfoCore(columnViewInfoValue, rowViewInfoValue, col, row);
			AppearanceHelper.Combine(cell.Appearance, new AppearanceObject[] { ViewInfo.PrintAndPaintAppearance.GetActualCellAppearance(cell) });
			return cell;
		}
		protected virtual void OnFocusedCellChanged(Point oldCell, Point newCell) {
			PivotCellViewInfo oldViewInfo = this.cells.Find(delegate(PivotCellViewInfo cell) {
				return cell.ColumnIndex == oldCell.X && cell.RowIndex == oldCell.Y;
			});
			PivotCellViewInfo newViewInfo = this.cells.Find(delegate(PivotCellViewInfo cell) {
				return cell.ColumnIndex == newCell.X && cell.RowIndex == newCell.Y;
			});
			if(oldViewInfo != null)
				oldViewInfo.Focused = false;
			if(newViewInfo != null)
				newViewInfo.Focused = true;
		}
		protected virtual void OnSelectionChanged() {
			SetFocusAndSelectionCell(this.cells);
		}
		protected virtual void SetFocusAndSelectionCell(List<PivotCellViewInfo> cells) {
			for(int i = 0; i < cells.Count; i++) {
				PivotCellViewInfo cell = cells[i];
				cell.Focused = GetIsCellFocused(cell);
				cell.Selected = GetIsCellSelected(cell);
			}
		}
		protected virtual bool GetIsCellFocused(PivotCellViewInfo cell) {
			return false;
		}
		protected virtual bool GetIsCellSelected(PivotCellViewInfo cell) {
			return false;
		}
		void DrawCells(ViewInfoPaintArgs e, List<PivotCellViewInfo> cells) {
			for(int i = 0; i < cells.Count; i++)
				DrawCell(e, cells[i]);
		}
		void DrawCell(ViewInfoPaintArgs e, PivotCellViewInfo cellViewInfo) {
			Rectangle cellBounds = cellViewInfo.Bounds;
			if(!cellBounds.IntersectsWith(e.ClipRectangle))
				return;
			if(ShowVertLines) {
				if(IsRightToLeft)  
					cellBounds.X += LineWidth;
				cellBounds.Width -= LineWidth;
			}
			if(ShowHorzLines)
				cellBounds.Height -= LineWidth;
			AppearanceObject cellAppearance = new AppearanceObject();
			AppearanceHelper.Combine(cellAppearance, new AppearanceObject[] { cellViewInfo.Appearance });
			CombineWithSelectAppearance(cellViewInfo, cellAppearance);
			ViewInfo.PrintAndPaintAppearance.GetCustomCellAppearance(ref cellAppearance, cellViewInfo, null);
			MethodInvoker invoker = () => {
				Data.PaintAppearance.Empty.FillRectangle(e.GraphicsCache, cellBounds);
				cellViewInfo.Draw(e, cellBounds, cellAppearance, DrawFocusedCellRect);
				};
			if(!CustomDrawCell(e, ref cellAppearance, cellViewInfo, invoker))
				invoker();
		}
		internal protected virtual void CombineWithSelectAppearance(PivotGridCellItem cellItem, AppearanceObject cellAppearance) {
		}
		internal protected abstract bool CustomDrawCell(ViewInfoPaintArgs paintArgs, ref AppearanceObject cellAppearance, PivotCellViewInfo cellViewInfo, MethodInvoker defaultDraw);
		void DrawLines(ViewInfoPaintArgs e, ArrayList lines, int height) {
			for(int i = 0; i < lines.Count; i++) {
				Rectangle bounds = (Rectangle)lines[i];
				if(height > 0)
					bounds.Height = height;
				Data.PaintAppearance.Lines.FillRectangle(e.GraphicsCache, bounds);
			}
		}
		#region ICellsBestFitProvider
		int ICellsBestFitProvider.VertLinesWidth { get { return ShowVertLines ? LineWidth : 0; } }
		bool ICellsBestFitProvider.BestFitConsiderCustomAppearance { get { return Data.OptionsBehavior.BestFitConsiderCustomAppearance; } }
		int ICellsBestFitProvider.GetCellStringWidth(PivotFieldItemBase field, string text) {
			return GetStringWidthCore(field, text, CellAppearance, true);
		}
		int ICellsBestFitProvider.GetTotalCellStringWidth(PivotFieldItemBase field, string text) {
			return GetStringWidthCore(field, text, TotalAppearance, true);
		}
		int ICellsBestFitProvider.GetGrandTotalCellStringWidth(PivotFieldItemBase field, string text) {
			return GetStringWidthCore(field, text, GrandTotalAppearance, true);
		}
		int ICellsBestFitProvider.GetItemTextWidth(PivotFieldItemBase field, PivotGridCellItem item, Rectangle? bounds) {
			AppearanceObject customCellApearance = ViewInfo.PrintAndPaintAppearance.GetCellAppearanceWithCustomCellAppearance(item, bounds);
			return GetStringWidthCore(field, item.Text, customCellApearance, false);
		}
		Graphics bestFitGraphics = null;
		Dictionary<PivotFieldItemBase, int> iconsWidth = null;
		void ICellsBestFitProvider.BeginBestFitCalculcations() {
			bestFitGraphics = GraphicsInfo.Default.AddGraphics(null);
			iconsWidth = new Dictionary<PivotFieldItemBase, int>();
		}
		void ICellsBestFitProvider.EndBestFitCalculcations() {
			GraphicsInfo.Default.ReleaseGraphics();
			bestFitGraphics = null;
			iconsWidth = null;
		}
		int GetStringWidthCore(PivotFieldItemBase baseField, string drawText, AppearanceObject appearance, bool isDataCell) {
			PivotFieldItem field = (PivotFieldItem)baseField;
			int iconWidth = isDataCell ? GetIconWidth(field) : 0;
			if(field.FieldEdit == null) {
				if(drawText.Length == 0)
					return 0;
				return XPaint.TextSizeRound(XPaint.Graphics.CalcTextSize(bestFitGraphics, drawText, appearance.Font, appearance.GetStringFormat(), 0).Width) + iconWidth;
			} else {
				return FieldMeasures.GetEditCellWidth(field, drawText, bestFitGraphics, appearance) + iconWidth;
			}
		}
		int GetIconWidth(PivotFieldItem field) {
			if(field.Area != PivotArea.DataArea)
				return 0;
			int iconWidth;
			if(iconsWidth.TryGetValue(field, out iconWidth))
				return iconWidth;
			int maxWidth = 0;
			foreach(PivotGridFormatRule rule in Data.FormatRules) {
				if(rule.Measure == Data.GetField(field)) {
					FormatConditionRuleIconSet set = rule.Rule as FormatConditionRuleIconSet;
					if(set != null) {
						for(int i = 0; i < set.IconSet.Icons.Count; i++) {
							Image icon = set.IconSet.Icons[i].GetIcon();
							if(icon != null)
								maxWidth = Math.Max(maxWidth, icon.Width + 6);
						}
					}
				}
			}
			iconsWidth.Add(field, maxWidth);
			return iconsWidth[field];
		}
		#endregion
	}
}
