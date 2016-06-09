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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.Office.History;
#if !SL
using System.Windows.Forms;
#endif
#if SL || WPF
using DevExpress.Xpf.RichEdit.Controls.Internal;
using DevExpress.Xpf.RichEdit.Ruler;
#endif
namespace DevExpress.XtraRichEdit.Ruler {
	#region RulerTickmark (abstract class)
	public abstract class RulerTickmark {
		#region Field
		readonly RectangleF bounds;
		readonly RectangleF displayBounds;
		bool visible;
		#endregion
		protected RulerTickmark(IRulerControl2 control, RectangleF bounds) {
			Guard.ArgumentNotNull(control, "control");
			this.bounds = bounds;
			this.displayBounds = control.LayoutUnitsToPixels(Rectangle.Round(bounds));
			this.visible = true;
		}
		#region Properties
		public bool Visible { get { return visible; } set { visible = value; } }
		public RectangleF DisplayBounds { get { return displayBounds; } }
		public RectangleF Bounds { get { return bounds; } }
		public abstract RulerTickmarkType RulerTickmarkType { get; }
		#endregion
		public abstract void Draw(HorizontalRulerPainter painter);
		public abstract void Draw(VerticalRulerPainter painter);
	}
	#endregion
	#region RulerTickmarkNumber
	public class RulerTickmarkNumber : RulerTickmark {
		readonly string number;
		public RulerTickmarkNumber(IRulerControl2 control, RectangleF bounds, string number)
			: base(control, bounds) {
			this.number = number;
		}
		public override RulerTickmarkType RulerTickmarkType { get { return RulerTickmarkType.Tick; } }
		public string Number { get { return number; } }
		public override void Draw(HorizontalRulerPainter painter) {
			painter.DrawTickmarkNumber(this);
		}
		public override void Draw(VerticalRulerPainter painter) {
			painter.DrawTickmarkNumber(this);
		}
	}
	#endregion
	#region RulerTickmarkHalf
	public class RulerTickmarkHalf : RulerTickmark {
		public RulerTickmarkHalf(IRulerControl2 control, RectangleF bounds)
			: base(control, bounds) {
		}
		public override RulerTickmarkType RulerTickmarkType { get { return RulerTickmarkType.HalfTick; } }
		public override void Draw(HorizontalRulerPainter painter) {
			painter.DrawTickmarkHalf(this);
		}
		public override void Draw(VerticalRulerPainter painter) {
			painter.DrawTickmarkHalf(this);
		}
	}
	#endregion
	#region RulerTickmarkQuarter
	public class RulerTickmarkQuarter : RulerTickmark {
		public RulerTickmarkQuarter(IRulerControl2 control, RectangleF bounds)
			: base(control, bounds) {
		}
		public override RulerTickmarkType RulerTickmarkType { get { return RulerTickmarkType.QuarterTick; } }
		public override void Draw(HorizontalRulerPainter painter) {
			painter.DrawTickmarkQuarter(this);
		}
		public override void Draw(VerticalRulerPainter painter) {
			painter.DrawTickmarkQuarter(this);
		}
	}
	#endregion
	#region RulerTickmarkType
	public enum RulerTickmarkType {
		Tick,
		HalfTick,
		QuarterTick
	}
	#endregion
	#region RulerHotZone (abstract class)
	public abstract class RulerHotZone {
		#region Fields
		readonly IRulerControl2 control;
		bool enabled = true;
		bool visible = true;
		bool isNew;
		Rectangle bounds;
		Rectangle displayBounds;
		#endregion
		protected internal RulerHotZone(IRulerControl2 control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		public Rectangle Bounds {
			get { return bounds; }
			set {
				if (value == bounds)
					return;
				bounds = value;
				displayBounds = RulerControl.LayoutUnitsToPixels(bounds);
			}
		}
		public Rectangle DisplayBounds { get { return displayBounds; } }
		public virtual RichEditCursor Cursor { get { return RichEditCursors.Default; } }
		public bool Enabled { get { return enabled; } set { enabled = value; } }
		public bool Visible { get { return visible; } set { visible = value; } }
		public bool IsNew { get { return isNew; } set { isNew = value; } }
		public IRulerControl2 RulerControl { get { return control; } }
		public IRichEditControl RichEditControl { get { return RulerControl.RichEditControl; } }
		protected internal DocumentModel DocumentModel { get { return RulerControl.DocumentModel; } }
		protected internal RulerViewInfoBase RulerViewInfo { get { return RulerControl.ViewInfoBase; } }
		protected internal Rectangle RulerClientBounds { get { return RulerViewInfo.ClientBounds; } }
		protected internal float ZoomFactor { get { return RulerControl.ZoomFactor; } }
		protected internal abstract int Weight { get; }
		#endregion
		public abstract void OnMouseDoubleClick();
		public abstract void OnMove(Point mousePosition);
		public abstract void Commit(Point mousePosition);
		protected internal abstract void SetNewValue(int newValue);
		public abstract RulerHotZone CreateEmptyClone();
		public virtual RulerHotZone Clone() {
			RulerHotZone clone = CreateEmptyClone();
			clone.Bounds = Bounds;
			return clone;
		}
		protected internal virtual void AddFakeHotZone() {
			RulerHotZone hotZone = Clone();
			hotZone.Enabled = false;
			List<RulerHotZone> hotZones = RulerViewInfo.HotZones;
			for (int i = 0; i < hotZones.Count; i++) {
				if (hotZone.GetType() == hotZones[i].GetType() && hotZones[i].Enabled == false)
					hotZones.RemoveAt(i);
			}
			hotZones.Insert(0, hotZone);
		}
		public virtual void Activate(RulerMouseHandler handler, Point mousePosition) {
			DragAndDropMouseHandlerState state = new DragAndDropMouseHandlerState(handler, this, mousePosition);
			BeginDragHotZoneMouseDragHelperState helperState = new BeginDragHotZoneMouseDragHelperState(handler, state, mousePosition, this);
			handler.SwitchStateCore(helperState, mousePosition);
		}
		protected internal abstract int GetVisualFeedbackValue(Point mousePosition, PageViewInfo pageViewInfo);
		public virtual bool CanEdit() {
			return true;
		}
	}
	#endregion
	#region EmptyHorizontalRulerHotZone
	public class EmptyHorizontalRulerHotZone : HorizontalRulerHotZone {
		public EmptyHorizontalRulerHotZone(IHorizontalRulerControl control, Rectangle bounds)
			: base(control) {
			Bounds = CalculateBounds(bounds);
		}
		protected internal override int Weight { get { return 0; } }
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			return new Rectangle(bounds.X, RulerClientBounds.Bottom - size.Height, size.Width, size.Height);
		}
		public override void OnMouseDoubleClick() {
		}
		public override void OnMove(Point mousePosition) {
		}
		public override void Commit(Point mousePosition) {
		}
		protected internal override void SetNewValue(int newValue) {
		}
		public override RulerHotZone CreateEmptyClone() {
			return new EmptyHorizontalRulerHotZone(HorizontalRulerControl, Bounds);
		}
		protected internal override int GetVisualFeedbackValue(Point mousePosition, PageViewInfo pageViewInfo) {
			return 0;
		}
		public override void Activate(RulerMouseHandler handler, Point mousePosition) {
		}
	}
	#endregion
	#region HorizontalRulerHotZone (abstract class)
	public abstract class HorizontalRulerHotZone : RulerHotZone {
		protected HorizontalRulerHotZone(IHorizontalRulerControl control)
			: base(control) {
		}
		#region Properties
		public IHorizontalRulerControl HorizontalRulerControl { get { return (IHorizontalRulerControl)RulerControl; } }
		protected HorizontalRulerViewInfo HorizontalRulerViewInfo { get { return HorizontalRulerControl.ViewInfo; } }
		#endregion
		protected abstract Rectangle CalculateBounds(Rectangle bounds);
		protected internal virtual int GetDocumentLayoutDistanceToCurrentAreaStart(int mouseX) {
			return (int)((mouseX - RulerViewInfo.ActiveAreaCollection[RulerViewInfo.CurrentActiveAreaIndex].X) / ZoomFactor);
		}
		protected internal virtual int GetDocumentLayoutDistanceToCellLeft(int mouseX) {
			HorizontalRulerViewInfo horizontalRulerViewInfo = RulerViewInfo as HorizontalRulerViewInfo;
			if (horizontalRulerViewInfo != null && horizontalRulerViewInfo.TableCellViewInfo != null)
				return (int)((mouseX - horizontalRulerViewInfo.TableCellViewInfo.TextLeft) / ZoomFactor);
			else
				return GetDocumentLayoutDistanceToCurrentAreaStart(mouseX);
		}
		protected internal virtual int GetDocumentLayoutDistanceToCurrentAreaEnd(int mouseX) {
			return (int)((RulerViewInfo.ActiveAreaCollection[RulerViewInfo.CurrentActiveAreaIndex].Right - mouseX) / ZoomFactor);
		}
		protected internal Size CalculateSize() {
			return HorizontalRulerControl.CalculateHotZoneSize(this);
		}
		protected internal override int GetVisualFeedbackValue(Point mousePosition, PageViewInfo pageViewInfo) {
			if (pageViewInfo == null)
				return 0;
			int x = (int)((mousePosition.X - RulerViewInfo.ActiveAreaCollection[0].X) / RulerControl.ZoomFactor);
			return pageViewInfo.Page.ClientBounds.X + x;
		}
	}
	#endregion
	#region TabHotZone (abstract class)
	public abstract class TabHotZone : HorizontalRulerHotZone {
		#region Fields
		readonly TabInfo oldTabInfo;
		TabInfo tabInfo;
		#endregion
		protected TabHotZone(IHorizontalRulerControl control, Rectangle bounds, TabInfo tabInfo)
			: base(control) {
			this.tabInfo = tabInfo;
			this.oldTabInfo = tabInfo;
			this.Bounds = CalculateBounds(bounds);
		}
		#region Properties
		public TabInfo TabInfo { get { return tabInfo; } }
		protected internal override int Weight { get { return 1; } }
		#endregion
		public override void OnMove(Point mousePosition) {
			int newTabPosition = HorizontalRulerViewInfo.GetRulerModelPositionRelativeToTableCellViewInfo(mousePosition.X);
			SetNewValue(newTabPosition);
			this.Visible = CanDropTab(mousePosition);
		}
		protected internal override void SetNewValue(int newValue) {
			tabInfo = new TabInfo(newValue, tabInfo.Alignment, tabInfo.Leader, tabInfo.Deleted);
			Bounds = new Rectangle(RulerViewInfo.GetRulerLayoutPosition(newValue) + HorizontalRulerViewInfo.GetAdditionalCellIndent(true), Bounds.Y, Bounds.Width, Bounds.Height);
			Bounds = CalculateBounds(Bounds);
		}
		bool CanDropTab(Point mousePosition) {
			Rectangle clientBounds = RulerClientBounds;
#if !SL && !WPF
			Rectangle bounds = RulerViewInfo.Bounds;
			clientBounds.Y = bounds.Y;
			clientBounds.Height = bounds.Height;
#endif
			return clientBounds.Contains(mousePosition);
		}
		public override void Commit(Point mousePosition) {
			CommandCollection commands = new CommandCollection();
			commands.Add(new DeleteTabAtParagraphCommand(RichEditControl, oldTabInfo));
			if (CanDropTab(mousePosition))
				commands.Add(new InsertTabToParagraphCommand(RichEditControl, tabInfo));
			CustomTransactedMultiCommand command = new CustomTransactedMultiCommand(RichEditControl, commands, MultiCommandExecutionMode.ExecuteAllAvailable, MultiCommandUpdateUIStateMode.EnableIfAllAvailable);
			command.Execute();
			RulerControl.Reset();
		}
		public override void OnMouseDoubleClick() {
			ShowTabsFormCommand command = new ShowTabsFormCommand(RichEditControl);
			command.Execute();
		}
		protected internal void AddNewTab() {
			InsertTabToParagraphCommand command = new InsertTabToParagraphCommand(RichEditControl, tabInfo);
			command.Execute();
		}
	}
	#endregion
	#region LeftTabHotZone
	public class LeftTabHotZone : TabHotZone {
		public LeftTabHotZone(IHorizontalRulerControl control, Rectangle bounds, TabInfo tabInfo)
			: base(control, bounds, tabInfo) {
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			return new Rectangle(bounds.X, RulerClientBounds.Bottom - size.Height, size.Width, size.Height);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new LeftTabHotZone(this.HorizontalRulerControl, this.Bounds, this.TabInfo);
		}
	}
	#endregion
	#region RightTabHotZone
	public class RightTabHotZone : TabHotZone {
		public RightTabHotZone(IHorizontalRulerControl control, Rectangle bounds, TabInfo tabInfo)
			: base(control, bounds, tabInfo) {
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			return new Rectangle(bounds.X - size.Width, RulerClientBounds.Bottom - size.Height, size.Width, size.Height);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new RightTabHotZone(this.HorizontalRulerControl, this.Bounds, this.TabInfo);
		}
	}
	#endregion
	#region CenterTabHotZone
	public class CenterTabHotZone : TabHotZone {
		public CenterTabHotZone(IHorizontalRulerControl control, Rectangle bounds, TabInfo tabInfo)
			: base(control, bounds, tabInfo) {
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			return new Rectangle(bounds.X - size.Width / 2, RulerClientBounds.Bottom - size.Height, size.Width, size.Height);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new CenterTabHotZone(this.HorizontalRulerControl, this.Bounds, this.TabInfo);
		}
	}
	#endregion
	#region DecimalTabHotZone
	public class DecimalTabHotZone : TabHotZone {
		public DecimalTabHotZone(IHorizontalRulerControl control, Rectangle bounds, TabInfo tabInfo)
			: base(control, bounds, tabInfo) {
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			return new Rectangle(bounds.X - size.Width / 2, RulerClientBounds.Bottom - size.Height, size.Width, size.Height);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new DecimalTabHotZone(this.HorizontalRulerControl, this.Bounds, this.TabInfo);
		}
	}
	#endregion
	#region TableHotZone
	public class TableHotZone : TableLeftBorderHotZone {
		public TableHotZone(IHorizontalRulerControl control, Rectangle bounds, int borderIndex, TableViewInfo tableViewInfo, TableCell tableCell)
			: base(control, bounds, borderIndex, tableViewInfo, tableCell) {
		}
		protected internal override TableElementAccessorBase GetElementAccessor() {
			return new TableCellAccessor(TableCell);
		}
		public override void Commit(Point mousePosition) {
			if (Column == null)
				return;
			ChangeTableVirtualColumnRightCommand command = new ChangeTableVirtualColumnRightCommand(RichEditControl, Column, GetDocumentLayoutDistanceToCurrentAreaStart(NewValue) + TableViewInfo.Column.Bounds.Left - HorizontalRulerViewInfo.GetAdditionalParentCellIndent(false));
			command.Execute();
		}
		public override RulerHotZone CreateEmptyClone() {
			return new TableHotZone(this.HorizontalRulerControl, this.Bounds, this.BorderIndex, this.TableViewInfo, this.TableCell);
		}
	}
	#endregion
	#region TableLeftBorderHotZone
	public class TableLeftBorderHotZone : HorizontalRulerHotZone {
		#region Fields
		TableViewInfo tableViewInfo;
		int leftBorder;
		float rightBorder;
		int newValue;
		VirtualTableColumn column;
		TableCell tableCell;
		int borderIndex;
		#endregion
		public TableLeftBorderHotZone(IHorizontalRulerControl control, Rectangle bounds, int borderIndex, TableViewInfo tableViewInfo, TableCell tableCell)
			: base(control) {
			this.Bounds = CalculateBounds(bounds);
			this.tableCell = tableCell;
			this.tableViewInfo = tableViewInfo;
			this.borderIndex = borderIndex;
			column = GetVirtualTableColumn();
			CalculateBorders();
			this.newValue = bounds.X;
		}
		#region Properties
		protected internal override int Weight { get { return 3; } }
		protected internal TableViewInfo TableViewInfo { get { return tableViewInfo; } }
		public override RichEditCursor Cursor { get { return RichEditCursors.ResizeTableColumn; } }
		protected internal VirtualTableColumn Column { get { return column; } }
		protected internal TableCell TableCell { get { return tableCell; } }
		protected internal int NewValue { get { return newValue; } }
		protected internal int BorderIndex { get { return borderIndex; } }
		#endregion
		void CalculateBorders() {
			int currentColumnIndex = RulerViewInfo.CurrentActiveAreaIndex;
			int minCellWidth = RulerViewInfo.ToDocumentLayoutUnitConverter.ToLayoutUnits(tableCell.GetMinCellWidth());
			int additionalIndent = +HorizontalRulerViewInfo.GetAdditionalParentCellIndent(false);
			leftBorder = (int)(((column.MaxLeftBorder + minCellWidth - TableViewInfo.Column.Bounds.Left + additionalIndent) * ZoomFactor) + RulerViewInfo.ActiveAreaCollection[currentColumnIndex].X);
			rightBorder = ((column.MaxRightBorder - minCellWidth - TableViewInfo.Column.Bounds.Left + additionalIndent) * ZoomFactor) + RulerViewInfo.ActiveAreaCollection[currentColumnIndex].X;
		}
		protected internal virtual VirtualTableColumn GetVirtualTableColumn() {
			if (tableCell == null)
				return null;
			TableElementAccessorBase elementAccessor = GetElementAccessor();
			EnhancedSelectionManager enhancedSelectionManager = new EnhancedSelectionManager(DocumentModel.ActivePieceTable);
			return enhancedSelectionManager.CalculateTableCellsToResizeHorizontallyCore(TableViewInfo, tableCell.Row, elementAccessor, borderIndex, borderIndex);
		}
		protected internal virtual TableElementAccessorBase GetElementAccessor() {
			return new TableRowBeforeAccessor(tableCell.Row);
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			return new Rectangle(bounds.X - size.Width / 2, bounds.Y, size.Width, size.Height);
		}
		public override void OnMove(Point mousePosition) {
			if (mousePosition.X <= leftBorder || mousePosition.X >= rightBorder)
				return;
			newValue = mousePosition.X;
			Rectangle newBounds = new Rectangle(newValue, Bounds.Y, Bounds.Width, Bounds.Height);
			Bounds = CalculateBounds(newBounds);
		}
		public override void Commit(Point mousePosition) {
			TableViewInfo.Table.TableProperties.TableIndent.Type = WidthUnitType.ModelUnits;
			TableViewInfo.Table.TableProperties.TableIndent.Value = RulerViewInfo.ToDocumentLayoutUnitConverter.ToModelUnits(GetDocumentLayoutDistanceToCurrentAreaStart(NewValue) - HorizontalRulerViewInfo.GetAdditionalParentCellIndent(false) - TableViewInfo.TextAreaOffset) + TableCell.GetActualLeftMargin().Value;
		}
		public override RulerHotZone CreateEmptyClone() {
			return new TableLeftBorderHotZone(this.HorizontalRulerControl, this.Bounds, this.borderIndex, this.tableViewInfo, this.TableCell);
		}
		public override void OnMouseDoubleClick() {
		}
		protected internal override void SetNewValue(int newValue) {
		}
		public override bool CanEdit() {
			if (!base.CanEdit())
				return false;
			ChangeTableVirtualColumnRightCommand command = new ChangeTableVirtualColumnRightCommand(RichEditControl, Column, 100);
			return command.CanExecute();
		}
	}
	#endregion
	#region IndentHotZone (abstract class)
	public abstract class IndentHotZone : HorizontalRulerHotZone {
		readonly int indent;
		int rightBorder;
		int leftBorder;
		int newIndent;
		protected IndentHotZone(IHorizontalRulerControl control, Rectangle bounds, int indent)
			: base(control) {
			this.indent = indent;
			this.newIndent = indent;
			this.rightBorder = RulerClientBounds.Right;
			this.leftBorder = RulerClientBounds.Left;
		}
		#region Properties
		public int NewIndent { get { return newIndent; } set { newIndent = value; } }
		public int Indent { get { return indent; } }
		public int LeftBorder { get { return leftBorder; } set { leftBorder = value; } }
		public int RightBorder { get { return rightBorder; } set { rightBorder = value; } }
		protected internal override int Weight { get { return 2; } }
		#endregion
		public override void OnMove(Point mousePosition) {
			if (mousePosition.X >= LeftBorder && mousePosition.X <= RightBorder) {
				int newIndent = HorizontalRulerViewInfo.GetRulerModelPositionRelativeToTableCellViewInfo(mousePosition.X);
				SetNewValue(newIndent);
			}
		}
		protected internal override void SetNewValue(int newIndent) {
			this.newIndent = newIndent;
			Bounds = new Rectangle(RulerViewInfo.GetRulerLayoutPosition(newIndent) + HorizontalRulerViewInfo.GetAdditionalCellIndent(true), Bounds.Y, Bounds.Width, Bounds.Height);
			Bounds = CalculateBounds(Bounds);
		}
		public override void OnMouseDoubleClick() {
			ShowParagraphFormCommand command = new ShowParagraphFormCommand(RichEditControl);
			command.Execute();
		}
	}
	#endregion
	#region LeftIndentHotZone
	public class LeftIndentHotZone : IndentHotZone {
		readonly LeftBottomHotZone leftBottomHotZone;
		readonly int firstLineIndent;
		public LeftIndentHotZone(IHorizontalRulerControl control, Rectangle bounds, int indent, LeftBottomHotZone leftBottomHotZone, int firstLineIndent)
			: base(control, bounds, indent) {
			this.leftBottomHotZone = leftBottomHotZone;
			SetNewValue(indent);
			this.firstLineIndent = firstLineIndent;
		}
		public override void Activate(RulerMouseHandler handler, Point mousePosition) {
			base.Activate(handler, mousePosition);
			if (!IsNew)
				leftBottomHotZone.AddFakeHotZone();
		}
		public override void Commit(Point mousePosition) {
			DocumentModel.BeginUpdate();
			using (HistoryTransaction transaction = new HistoryTransaction(DocumentModel.History)) {
				ChangeParagraphFirstLineIndentCommand firstLineCommand = new ChangeParagraphFirstLineIndentCommand(RichEditControl, firstLineIndent - (NewIndent - Indent));
				firstLineCommand.Execute();
				ChangeParagraphLeftIndentCommand command = new ChangeParagraphLeftIndentCommand(RichEditControl, NewIndent);
				command.Execute();
			}
			DocumentModel.EndUpdate();
		}
		public override RulerHotZone CreateEmptyClone() {
			return new LeftIndentHotZone(this.HorizontalRulerControl, this.Bounds, this.Indent, leftBottomHotZone, firstLineIndent);
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			Size sizeBottom = leftBottomHotZone.CalculateSize();
			int y = RulerClientBounds.Bottom + HorizontalRulerControl.Painter.PaddingBottom - sizeBottom.Height - size.Height;
			return new Rectangle(bounds.X - size.Width / 2, y, size.Width, size.Height);
		}
		public override void OnMove(Point mousePosition) {
			base.OnMove(mousePosition);
			leftBottomHotZone.SetNewValue(NewIndent);
		}
	}
	#endregion
	#region RightIndentHotZone
	public class RightIndentHotZone : IndentHotZone {
		public RightIndentHotZone(IHorizontalRulerControl control, Rectangle bounds, int indent)
			: base(control, bounds, indent) {
			this.Bounds = CalculateBounds(bounds);
		}
		public override void Commit(Point mousePosition) {
			ChangeParagraphRightIndentCommand command = new ChangeParagraphRightIndentCommand(RichEditControl, NewIndent);
			command.Execute();
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			int y = RulerClientBounds.Bottom - size.Height;
			return new Rectangle(bounds.X - size.Width / 2, y, size.Width, size.Height);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new RightIndentHotZone(this.HorizontalRulerControl, this.Bounds, this.Indent);
		}
		public override void OnMove(Point mousePosition) {
			if (mousePosition.X >= LeftBorder && mousePosition.X <= RightBorder) {
				NewIndent = HorizontalRulerControl.ViewInfo.GetRightIndentModelPosition(mousePosition.X);
				Bounds = new Rectangle(HorizontalRulerControl.ViewInfo.GetRightIndentLayoutPosition(NewIndent), Bounds.Y, Bounds.Width, Bounds.Height);
				Bounds = CalculateBounds(Bounds);
			}
		}
		protected internal override void SetNewValue(int newIndent) {
			NewIndent = newIndent;
			Bounds = new Rectangle(HorizontalRulerControl.ViewInfo.GetRightIndentLayoutPosition(newIndent), Bounds.Y, Bounds.Width, Bounds.Height);
			Bounds = CalculateBounds(Bounds);
		}
	}
	#endregion
	#region FirstLineIndentHotZone
	public class FirstLineIndentHotZone : IndentHotZone {
		readonly int leftIndent;
		public FirstLineIndentHotZone(IHorizontalRulerControl control, Rectangle bounds, int indent, int leftIndent)
			: base(control, bounds, indent) {
			SetNewValue(indent);
			this.leftIndent = leftIndent;
		}
		public override void Commit(Point mousePosition) {
			ChangeParagraphFirstLineIndentCommand command = new ChangeParagraphFirstLineIndentCommand(RichEditControl, NewIndent - leftIndent);
			command.Execute();
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			int y = RulerClientBounds.Top - HorizontalRulerControl.Painter.PaddingTop;
			return new Rectangle(bounds.X - size.Width / 2, y, size.Width, size.Height);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new FirstLineIndentHotZone(this.HorizontalRulerControl, this.Bounds, this.Indent, this.leftIndent);
		}
	}
	#endregion
	#region LeftBottomHotZone
	public class LeftBottomHotZone : IndentHotZone {
		readonly FirstLineIndentHotZone firstLineHotZone;
		LeftIndentHotZone leftIndentHotZone;
		public LeftBottomHotZone(IHorizontalRulerControl control, Rectangle bounds, int indent, int rightBorder, FirstLineIndentHotZone firstLineIndentHotZone)
			: base(control, bounds, indent) {
			SetNewValue(indent);
			this.firstLineHotZone = firstLineIndentHotZone;
			this.RightBorder = rightBorder;
		}
		protected internal void AddLeftHotZone(Rectangle bounds) {
			this.leftIndentHotZone = new LeftIndentHotZone(HorizontalRulerControl, bounds, Indent, this, firstLineHotZone.Indent - Indent);
			leftIndentHotZone.RightBorder = RightBorder;
			RulerViewInfo.HotZones.Add(leftIndentHotZone);
		}
		protected internal override void AddFakeHotZone() {
			firstLineHotZone.AddFakeHotZone();
			leftIndentHotZone.AddFakeHotZone();
			base.AddFakeHotZone();
		}
		public override void Commit(Point mousePosition) {
			ChangeParagraphLeftIndentCommand command = new ChangeParagraphLeftIndentCommand(RichEditControl, NewIndent);
			command.Execute();
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			int y = RulerClientBounds.Bottom + HorizontalRulerControl.Painter.PaddingBottom - size.Height;
			return new Rectangle(bounds.X - size.Width / 2, y, size.Width, size.Height);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new LeftBottomHotZone(this.HorizontalRulerControl, this.Bounds, this.Indent, this.RightBorder, firstLineHotZone);
		}
		public override void OnMove(Point mousePosition) {
			Rectangle bounds = firstLineHotZone.Bounds;
			int x = mousePosition.X + bounds.Left - Bounds.Left;
			if (x >= LeftBorder && x <= RightBorder) {
				base.OnMove(mousePosition);
			}
			leftIndentHotZone.SetNewValue(NewIndent);
			firstLineHotZone.SetNewValue(firstLineHotZone.Indent + (NewIndent - Indent));
		}
	}
	#endregion
	#region ColumnResizerHotZone (abstract class)
	public abstract class ColumnResizerHotZone : HorizontalRulerHotZone {
		readonly int columnIndex;
		protected ColumnResizerHotZone(IHorizontalRulerControl control, Rectangle bounds, int columnIndex)
			: base(control) {
			this.columnIndex = columnIndex;
			Bounds = CalculateBounds(bounds);
		}
		#region Properties
		public int ColumnIndex { get { return columnIndex; } }
		public SectionProperties SectionProperties { get { return RulerViewInfo.SectionProperties; } }
		public override RichEditCursor Cursor { get { return RichEditCursors.SizeWE; } }
		protected internal override int Weight { get { return 4; } }
		#endregion
		public override void OnMouseDoubleClick() {
		}
		protected internal override void SetNewValue(int newPosition) {
		}
		protected internal override void AddFakeHotZone() {
		}
		public override void OnMove(Point mousePosition) {
			OnMoveCore(mousePosition);
			HorizontalRulerControl.SetViewInfo(HorizontalRulerControl.CreateViewInfoCore(SectionProperties, RulerViewInfo.IsMainPieceTable));
		}
		protected internal abstract void OnMoveCore(Point mousePosition);
		public override void Commit(Point mousePosition) {
			if (!RichEditControl.InnerControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return;
			RulerControl.Reset();
		}
		protected abstract int CalculateNewValue(Point mousePosition);
	}
	#endregion
	#region MiddleColumnResizerHotZone
	public class MiddleColumnResizerHotZone : ColumnResizerHotZone {
		public MiddleColumnResizerHotZone(IHorizontalRulerControl control, Rectangle bounds, int columnIndex)
			: base(control, bounds, columnIndex) {
		}
		protected internal override void OnMoveCore(Point mousePosition) {
			int offset = CalculateNewValue(mousePosition);
			MoveColumnCommand command = new MoveColumnCommand(RichEditControl, SectionProperties, ColumnIndex, offset);
			command.GetNewSection(SectionProperties);
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			int y = RulerClientBounds.Y + RulerClientBounds.Height / 2 - size.Height / 2;
			return new Rectangle(bounds.X - size.Width / 2, y, size.Width, size.Height);
		}
		public override void Commit(Point mousePosition) {
			int offset = CalculateNewValue(mousePosition);
			MoveColumnCommand command = new MoveColumnCommand(RichEditControl, SectionProperties, ColumnIndex, offset);
			command.Execute();
			base.Commit(mousePosition);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new MiddleColumnResizerHotZone(HorizontalRulerControl, Bounds, ColumnIndex);
		}
		protected override int CalculateNewValue(Point mousePosition) {
			List<RectangleF> areaCollection = RulerViewInfo.ActiveAreaCollection;
			float pos = mousePosition.X - (areaCollection[ColumnIndex].Right + (areaCollection[ColumnIndex + 1].Left - areaCollection[ColumnIndex].Right) / 2);
			return (int)(pos / ZoomFactor);
		}
		protected internal override int GetVisualFeedbackValue(Point mousePosition, PageViewInfo pageViewInfo) {
			if (pageViewInfo != null) {
				float middle = (RulerViewInfo.ActiveAreaCollection[ColumnIndex].Right + RulerViewInfo.ActiveAreaCollection[ColumnIndex + 1].Left) / 2.0f;
				return (int)((middle - RulerViewInfo.ActiveAreaCollection[0].Left) / ZoomFactor) + pageViewInfo.Page.ClientBounds.X;
			}
			else
				return base.GetVisualFeedbackValue(mousePosition, pageViewInfo);
		}
	}
	#endregion
	#region LeftColumnResizerHotZone
	public class LeftColumnResizerHotZone : ColumnResizerHotZone {
		public LeftColumnResizerHotZone(IHorizontalRulerControl control, Rectangle bounds, int columnIndex)
			: base(control, bounds, columnIndex) {
		}
		protected internal override void OnMoveCore(Point mousePosition) {
			ChangeColumnSizeCommand command = CreateCommand(mousePosition);
			command.GetNewSection(SectionProperties);
		}
		public override void Commit(Point mousePosition) {
			ChangeColumnSizeCommand command = CreateCommand(mousePosition);
			command.Execute();
			base.Commit(mousePosition);
		}
		ChangeColumnSizeCommand CreateCommand(Point mousePosition) {
			int offset = CalculateNewValue(mousePosition);
			if (SectionProperties.EqualWidthColumns)
				return new ChangeWidthEqualWidthColumnsByLeftCommand(RichEditControl, SectionProperties, ColumnIndex, offset);
			else
				return new ChangeColumnSizeByLeftCommand(RichEditControl, SectionProperties, ColumnIndex, offset);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new LeftColumnResizerHotZone(HorizontalRulerControl, Bounds, ColumnIndex);
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			int y = RulerClientBounds.Y + RulerClientBounds.Height / 2 - size.Height / 2;
			return new Rectangle(bounds.X - size.Width, y, size.Width, size.Height);
		}
		protected override int CalculateNewValue(Point mousePosition) {
			int offset = (int)((RulerViewInfo.ActiveAreaCollection[ColumnIndex].Left - mousePosition.X) / ZoomFactor);
			return RulerViewInfo.ToDocumentLayoutUnitConverter.ToModelUnits(offset);
		}
		protected internal override int GetVisualFeedbackValue(Point mousePosition, PageViewInfo pageViewInfo) {
			if (ColumnIndex == 0)
				return RulerViewInfo.ToDocumentLayoutUnitConverter.ToLayoutUnits(SectionProperties.LeftMargin);
			else {
				if (pageViewInfo != null)
					return (int)((RulerViewInfo.ActiveAreaCollection[ColumnIndex].Left - RulerViewInfo.ActiveAreaCollection[0].Left) / ZoomFactor) + pageViewInfo.Page.ClientBounds.X;
				else
					return base.GetVisualFeedbackValue(mousePosition, pageViewInfo);
			}
		}
		public override void OnMouseDoubleClick() {
			if (ColumnIndex == 0) {
				ShowPageSetupFormCommand command = new ShowPageSetupFormCommand(RichEditControl);
				command.Execute();
			}
		}
	}
	#endregion
	#region RightColumnResizerHotZone
	public class RightColumnResizerHotZone : ColumnResizerHotZone {
		public RightColumnResizerHotZone(IHorizontalRulerControl control, Rectangle bounds, int columnIndex)
			: base(control, bounds, columnIndex) {
		}
		protected internal override void OnMoveCore(Point mousePosition) {
			int offset = CalculateNewValue(mousePosition);
			ChangeColumnSizeCommand command;
			int index;
			if (RulerViewInfo.IsMainPieceTable)
				index = ColumnIndex;
			else
				index = RulerViewInfo.SectionProperties.ColumnCount - 1;
			if (SectionProperties.EqualWidthColumns)
				command = new ChangeWidthEqualWidthColumnsByRightCommand(RichEditControl, SectionProperties, index, offset);
			else
				command = new ChangeColumnWidthCommand(RichEditControl, SectionProperties, index, offset);
			command.GetNewSection(SectionProperties);
		}
		public override void Commit(Point mousePosition) {
			int offset = CalculateNewValue(mousePosition);
			ChangeColumnSizeCommand command;
			if (SectionProperties.EqualWidthColumns)
				command = new ChangeWidthEqualWidthColumnsByRightCommand(RichEditControl, SectionProperties, ColumnIndex, offset);
			else
				command = new ChangeColumnWidthCommand(RichEditControl, SectionProperties, ColumnIndex, offset);
			command.Execute();
			base.Commit(mousePosition);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new RightColumnResizerHotZone(HorizontalRulerControl, Bounds, ColumnIndex);
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			Size size = CalculateSize();
			int y = RulerClientBounds.Y + RulerClientBounds.Height / 2 - size.Height / 2;
			return new Rectangle(bounds.X, y, size.Width, size.Height);
		}
		protected override int CalculateNewValue(Point mousePosition) {
			return (int)((RulerViewInfo.ActiveAreaCollection[ColumnIndex].Right - mousePosition.X) / ZoomFactor);
		}
		protected internal override int GetVisualFeedbackValue(Point mousePosition, PageViewInfo pageViewInfo) {
			if (pageViewInfo != null)
				return (int)((RulerViewInfo.ActiveAreaCollection[ColumnIndex].Right - RulerViewInfo.ActiveAreaCollection[0].Left) / ZoomFactor) + pageViewInfo.Page.ClientBounds.X;
			else
				return base.GetVisualFeedbackValue(mousePosition, pageViewInfo);
		}
	}
	#endregion
	#region TabTypeToggleBackgroundHotZone
	public class TabTypeToggleBackgroundHotZone : HorizontalRulerHotZone {
		public TabTypeToggleBackgroundHotZone(IHorizontalRulerControl control, Rectangle bounds)
			: base(control) {
			Bounds = CalculateBounds(bounds);
		}
		protected internal override int Weight { get { return 5; } }
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			return bounds;
		}
		public override void OnMove(Point mousePosition) {
		}
		public override void Commit(Point mousePosition) {
		}
		public override void OnMouseDoubleClick() {
		}
		public override RulerHotZone CreateEmptyClone() {
			return new TabTypeToggleBackgroundHotZone(HorizontalRulerControl, Bounds);
		}
		protected internal override void SetNewValue(int newValue) {
		}
	}
	#endregion
	#region TabTypeToggleHotZone
	public class TabTypeToggleHotZone : HorizontalRulerHotZone {
		readonly List<HorizontalRulerHotZone> hotZones;
		Rectangle hotZoneBounds;
		public TabTypeToggleHotZone(IHorizontalRulerControl control, Rectangle bounds)
			: base(control) {
			Bounds = CalculateBounds(bounds);
			this.hotZones = CreateHotZones();
			this.hotZoneBounds = CalculateHotZoneBounds();
		}
		#region Properties
		public HorizontalRulerHotZone HotZone {
			get {
				int index = HorizontalRulerControl.TabTypeIndex;
				if (index >= hotZones.Count)
					return hotZones[0];
				else
					return hotZones[index];
			}
		}
		public Rectangle HotZoneBounds { get { return hotZoneBounds; } }
		protected internal override int Weight { get { return 0; } }
		#endregion
		protected internal List<HorizontalRulerHotZone> CreateHotZones() {
			List<HorizontalRulerHotZone> hotZones = new List<HorizontalRulerHotZone>();
			if (DocumentModel.DocumentCapabilities.ParagraphTabsAllowed && RichEditControl.InnerControl.Options.HorizontalRuler.ShowTabs) {
				hotZones.Add(new LeftTabHotZone(HorizontalRulerControl, Bounds, new TabInfo(-1, TabAlignmentType.Left)));
				hotZones.Add(new RightTabHotZone(HorizontalRulerControl, Bounds, new TabInfo(-1, TabAlignmentType.Right)));
				hotZones.Add(new CenterTabHotZone(HorizontalRulerControl, Bounds, new TabInfo(-1, TabAlignmentType.Center)));
				hotZones.Add(new DecimalTabHotZone(HorizontalRulerControl, Bounds, new TabInfo(-1, TabAlignmentType.Decimal)));
			}
			if (DocumentModel.DocumentCapabilities.ParagraphFormattingAllowed && RichEditControl.InnerControl.Options.HorizontalRuler.ShowLeftIndent)
				hotZones.Add(new FirstLineIndentHotZone(HorizontalRulerControl, Bounds, 0, 0));
			if (hotZones.Count <= 0)
				hotZones.Add(new EmptyHorizontalRulerHotZone(HorizontalRulerControl, Bounds));
			return hotZones;
		}
		protected override Rectangle CalculateBounds(Rectangle bounds) {
			return RulerControl.PixelsToLayoutUnits(bounds);
		}
		protected internal Rectangle CalculateHotZoneBounds() {
			Size size = HotZone.CalculateSize();
			Rectangle bounds = new Rectangle(Bounds.X + (Bounds.Width - size.Width) / 2, Bounds.Y + (Bounds.Height - size.Height) / 2, size.Width, size.Height);
			return RulerControl.LayoutUnitsToPixels(bounds);
		}
		public override void OnMove(Point mousePosition) {
		}
		public override void Commit(Point mousePosition) {
			HorizontalRulerControl.TabTypeIndex++;
			if (HorizontalRulerControl.TabTypeIndex >= hotZones.Count)
				HorizontalRulerControl.TabTypeIndex = 0;
			hotZoneBounds = CalculateHotZoneBounds();
		}
		public override void OnMouseDoubleClick() {
		}
		public override RulerHotZone CreateEmptyClone() {
			return new TabTypeToggleHotZone(HorizontalRulerControl, Bounds);
		}
		protected internal override void SetNewValue(int newValue) {
		}
	}
	#endregion
	#region VerticalRulerHotZone (abstract class)
	public abstract class VerticalRulerHotZone : RulerHotZone {
		protected VerticalRulerHotZone(IVerticalRulerControl control)
			: base(control) {
		}
		#region Properties
		public IVerticalRulerControl VerticalRulerControl { get { return (IVerticalRulerControl)RulerControl; } }
		protected internal override int Weight { get { return 0; } }
		#endregion
		protected abstract int CalculateNewValue(Point mousePosition);
		protected internal override void SetNewValue(int newValue) {
		}
		protected internal override void AddFakeHotZone() {
		}
		protected internal override int GetVisualFeedbackValue(Point mousePosition, PageViewInfo pageViewInfo) {
			if (pageViewInfo == null)
				return 0;
			int y = (int)((mousePosition.Y - RulerViewInfo.ActiveAreaCollection[0].Y) / RulerControl.ZoomFactor);
			return pageViewInfo.Page.ClientBounds.Y + y;
		}
	}
	#endregion
	#region SectionResizerHotZone (abstract class)
	public abstract class SectionResizerHotZone : VerticalRulerHotZone {
		readonly int columnIndex;
		protected SectionResizerHotZone(IVerticalRulerControl control, Rectangle bounds, int columnIndex)
			: base(control) {
			this.columnIndex = columnIndex;
			Bounds = bounds;
		}
		public int ColumnIndex { get { return columnIndex; } }
		public SectionProperties SectionProperties { get { return RulerViewInfo.SectionProperties; } }
		public override RichEditCursor Cursor { get { return RichEditCursors.SizeNS; } }
		public override void OnMouseDoubleClick() {
		}
		public override void OnMove(Point mousePosition) {
			OnMoveCore(mousePosition);
			VerticalRulerControl.SetViewInfo(new VerticalRulerViewInfo(VerticalRulerControl, SectionProperties, RulerViewInfo.IsMainPieceTable, VerticalRulerControl.ViewInfo.TableVerticalPositions, VerticalRulerControl.ViewInfo.TableCellViewInfo));
		}
		protected internal abstract void OnMoveCore(Point mousePosition);
	}
	#endregion
	#region SectionTopResizerHotZone
	public class SectionTopResizerHotZone : SectionResizerHotZone {
		public SectionTopResizerHotZone(IVerticalRulerControl control, Rectangle bounds, int columnIndex)
			: base(control, bounds, columnIndex) {
		}
		protected internal override void OnMoveCore(Point mousePosition) {
			int offset = CalculateNewValue(mousePosition);
			ChangeSectionHeightByTopCommand command = new ChangeSectionHeightByTopCommand(RichEditControl, SectionProperties, 0, offset);
			command.GetNewSection(SectionProperties);
		}
		public override void Commit(Point mousePosition) {
			int offset = CalculateNewValue(mousePosition);
			ChangeSectionHeightByTopCommand command = new ChangeSectionHeightByTopCommand(RichEditControl, SectionProperties, 0, offset);
			command.Execute();
		}
		protected override int CalculateNewValue(Point mousePosition) {
			int offset = (int)((RulerViewInfo.ActiveAreaCollection[0].Top - mousePosition.Y) / ZoomFactor);
			return RulerViewInfo.ToDocumentLayoutUnitConverter.ToModelUnits(offset);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new SectionTopResizerHotZone(VerticalRulerControl, Bounds, ColumnIndex);
		}
		protected internal override int GetVisualFeedbackValue(Point mousePosition, PageViewInfo pageViewInfo) {
			if (ColumnIndex == 0)
				return RulerViewInfo.ToDocumentLayoutUnitConverter.ToLayoutUnits(SectionProperties.TopMargin);
			else
				return base.GetVisualFeedbackValue(mousePosition, pageViewInfo);
		}
		public override void OnMouseDoubleClick() {
			ShowPageSetupFormCommand command = new ShowPageSetupFormCommand(RichEditControl);
			command.Execute();
		}
	}
	#endregion
	#region SectionBottomResizerHotZone
	public class SectionBottomResizerHotZone : SectionResizerHotZone {
		public SectionBottomResizerHotZone(IVerticalRulerControl control, Rectangle bounds, int columnIndex)
			: base(control, bounds, columnIndex) {
		}
		protected internal override void OnMoveCore(Point mousePosition) {
			int offset = CalculateNewValue(mousePosition);
			ChangeSectionHeightByBottomCommand command = new ChangeSectionHeightByBottomCommand(RichEditControl, SectionProperties, 0, offset);
			command.GetNewSection(SectionProperties);
		}
		public override void Commit(Point mousePosition) {
			int offset = CalculateNewValue(mousePosition);
			ChangeSectionHeightByBottomCommand command = new ChangeSectionHeightByBottomCommand(RichEditControl, SectionProperties, 0, offset);
			command.Execute();
		}
		protected override int CalculateNewValue(Point mousePosition) {
			int offset = (int)((RulerViewInfo.ActiveAreaCollection[0].Bottom - mousePosition.Y) / ZoomFactor);
			return RulerViewInfo.ToDocumentLayoutUnitConverter.ToModelUnits(offset);
		}
		public override RulerHotZone CreateEmptyClone() {
			return new SectionBottomResizerHotZone(VerticalRulerControl, Bounds, ColumnIndex);
		}
		public override void OnMouseDoubleClick() {
			ShowPageSetupFormCommand command = new ShowPageSetupFormCommand(RichEditControl);
			command.Execute();
		}
	}
	#endregion
	#region VerticalTableHotZone
	public class VerticalTableHotZone : VerticalRulerHotZone {
		#region Fields
		readonly TableViewInfo tableViewInfo;
		readonly int anchorIndex;
		#endregion
		public VerticalTableHotZone(IVerticalRulerControl control, Rectangle bounds, TableViewInfo tableViewInfo, int anchorIndex)
			: base(control) {
			this.Bounds = CalculateBounds(bounds);
			this.tableViewInfo = tableViewInfo;
			this.anchorIndex = anchorIndex;
		}
		protected internal TableViewInfo TableViewInfo { get { return tableViewInfo; } }
		public override RichEditCursor Cursor { get { return RichEditCursors.ResizeTableRow; } }
		protected virtual Rectangle CalculateBounds(Rectangle bounds) {
			return bounds;
		}
		protected override int CalculateNewValue(Point mousePosition) {
			return (int)((mousePosition.Y - RulerViewInfo.ActiveAreaCollection[0].Top) / ZoomFactor);
		}
		public override void OnMove(Point mousePosition) {
			int newValue = mousePosition.Y;
			Rectangle newBounds = new Rectangle(Bounds.X, newValue, Bounds.Width, Bounds.Height);
			Bounds = CalculateBounds(newBounds);
		}
		public override void Commit(Point mousePosition) {
			int rowIndex = anchorIndex + tableViewInfo.TopRowIndex;
			if (rowIndex < 0)
				return;
			TableViewInfo.Table.Rows[rowIndex].Properties.Height.Type = HeightUnitType.Minimum;
			int value = (int)(mousePosition.Y / ZoomFactor) - TableViewInfo.Anchors[anchorIndex].VerticalPosition;
			int minValue = DocumentModel.LayoutUnitConverter.PointsToFontUnits(1);
			int heightIndex = RulerViewInfo.ToDocumentLayoutUnitConverter.ToModelUnits(Math.Max(value, minValue));
			TableViewInfo.Table.Rows[rowIndex].Properties.Height.Value = heightIndex;
		}
		protected internal override int GetVisualFeedbackValue(Point mousePosition, PageViewInfo pageViewInfo) {
			if (pageViewInfo == null)
				return 0;
			int y = (int)((mousePosition.Y) / RulerControl.ZoomFactor);
			return y;
		}
		public override void OnMouseDoubleClick() {
		}
		public override RulerHotZone CreateEmptyClone() {
			return new VerticalTableHotZone(VerticalRulerControl, Bounds, TableViewInfo, anchorIndex);
		}
		public override bool CanEdit() {
			if (!base.CanEdit())
				return false;
			int rowIndex = anchorIndex + tableViewInfo.TopRowIndex;
			if (rowIndex < 0)
				return false;
			TableRow row = TableViewInfo.Table.Rows[rowIndex];
			ChangeTableRowHeightCommand command = new ChangeTableRowHeightCommand(RichEditControl, row, 0);
			return command.CanExecute();
		}
	}
	#endregion
}
