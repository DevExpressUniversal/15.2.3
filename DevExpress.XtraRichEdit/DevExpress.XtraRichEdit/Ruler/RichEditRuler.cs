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

#if !SL
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Services;
using DevExpress.Services.Internal;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Internal;
using System.Security.Permissions;
namespace DevExpress.XtraRichEdit.Ruler {
	#region RichEditRulerControlBase (abstract class)
	[DXToolboxItem(false)]
	public abstract partial class RulerControlBase : Control, ISupportLookAndFeel, IServiceContainer, IRulerControl {
		#region Fields
		readonly RichEditControl richEditControl;
		readonly IOrientation orientation;
		UserLookAndFeel lookAndFeel;
		const int minWidth = 5;
		const int minHeight = 5;
		Font tickMarkFont;
		RulerPainterBase painter;
		float dpiX;
		float dpiY;
		ServiceManager serviceManager;
		MouseHandler mouseHandler;
		int pageIndex = -1;
		int columnIndex = -1;
		ParagraphIndex paragraphIndex = new ParagraphIndex(-1);
		#endregion
		protected RulerControlBase(RichEditControl richEditControl) {
			Guard.ArgumentNotNull(richEditControl, "richEditControl");
			this.TabStop = false;
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserMouse, true);
			this.richEditControl = richEditControl;
			this.dpiX = richEditControl.DpiX;
			this.dpiY = richEditControl.DpiY;
			this.lookAndFeel = new DevExpress.LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			this.lookAndFeel.ParentLookAndFeel = richEditControl.LookAndFeel;
			this.tickMarkFont = new Font("Arial", 7f, GraphicsUnit.Point);
			this.Bounds = new Rectangle(0, 0, minWidth, minWidth);
			this.orientation = CreateOrientation();
			this.painter = CreatePainter();
			this.serviceManager = CreateServiceManager();
			InitializeMouseHandler();
			AddServices();
		}
		#region Properties
		protected internal RichEditControl RichEditControl { get { return richEditControl; } }
		IRichEditControl IRulerControl.RichEditControl { get { return this.RichEditControl; } }
		public float DpiX { get { return dpiX; } }
		public float DpiY { get { return dpiY; } }
		public float ZoomFactor { get { return RichEditControl.ActiveView.ZoomFactor; } }
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		public UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		public Font TickMarkFont { get { return tickMarkFont; } }
		public RulerPainterBase Painter { get { return painter; } }
		public DocumentModel DocumentModel { get { return RichEditControl.DocumentModel; } }
		public MouseHandler MouseHandler { get { return mouseHandler; } }
		public IOrientation Orientation { get { return orientation; } }
		public abstract RulerViewInfoBase ViewInfoBase { get; }
		protected internal abstract bool IsVisible { get; }
		bool IRulerControl.IsVisible { get { return this.IsVisible; } }
		#endregion
		public void Reset() {
			InvalidateRulerViewInfo();
		}
		public void Repaint() {
			Invalidate();
			Update();
		}
		protected internal abstract IOrientation CreateOrientation();
		protected internal abstract RulerPainterBase CreatePainter();
		protected internal abstract void InvalidateRulerViewInfo();
		protected internal abstract MouseHandler CreateMouseHandler();
		protected internal abstract Point GetPhysicalPoint(Point point);
		protected internal abstract bool CanUpdate();
		protected internal abstract int GetRulerSizeInPixels();
		[System.Security.SecuritySafeCritical]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		int IRulerControl.GetRulerSizeInPixels() {
			return this.GetRulerSizeInPixels();
		}
		protected internal virtual void AddServices() {
			AddService(typeof(IMouseHandlerService), new RulerMouseHandlerService(this));
		}
		protected internal virtual ParagraphIndex CalculateParagraphIndex() {
			Selection selection = DocumentModel.Selection;
			DocumentLogPosition logPosition = selection.VirtualEnd;
			ParagraphIndex paragraphIndex = DocumentModel.ActivePieceTable.FindParagraphIndex(logPosition);
			if (logPosition > selection.Start) {
				if (logPosition == DocumentModel.ActivePieceTable.Paragraphs[paragraphIndex].LogPosition)
					paragraphIndex = Algorithms.Max(ParagraphIndex.Zero, paragraphIndex - 1);
			}
			return paragraphIndex;
		}
		protected internal Section GetSection(PieceTable pieceTable) {
			ParagraphIndex paragraphIndex = CalculateParagraphIndex();
			if (pieceTable.IsMain) {
				SectionIndex sectionIndex = DocumentModel.MainPieceTable.LookupSectionIndexByParagraphIndex(paragraphIndex);
				return DocumentModel.Sections[sectionIndex];
			}
			else if (pieceTable.IsComment) {
				SectionIndex sectionIndex = DocumentModel.FindSectionIndex(DocumentModel.Selection.Start);
				return DocumentModel.Sections[sectionIndex];
			}
			else if (pieceTable.IsTextBox) {
				TextBoxContentType textBoxContent = (TextBoxContentType)pieceTable.ContentType;
				if (textBoxContent.AnchorRun.PieceTable.IsMain) {
					SectionIndex sectionIndex = DocumentModel.MainPieceTable.LookupSectionIndexByParagraphIndex(textBoxContent.AnchorRun.Paragraph.Index);
					return DocumentModel.Sections[sectionIndex];
				}
				else {
					CaretPosition caretPosition = RichEditControl.ActiveView.CaretPosition;
					return caretPosition.LayoutPosition.PageArea.Section;
				}
			}
			Section section = DocumentModel.GetActiveSection();
			Guard.ArgumentNotNull(section, "section");
			return section;
		}
		protected internal int LayoutUnitsToPixelsH(int value) {
			return DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(value, RichEditControl.DpiX);
		}
		protected internal int LayoutUnitsToPixelsV(int value) {
			return DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(value, RichEditControl.DpiY);
		}
		public Rectangle LayoutUnitsToPixels(Rectangle value) {
			return DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(value, RichEditControl.DpiX, RichEditControl.DpiY);
		}
		protected internal int PixelsToLayoutUnitsH(int value) {
			return DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(value, RichEditControl.DpiX);
		}
		protected internal int PixelsToLayoutUnitsV(int value) {
			return DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(value, RichEditControl.DpiY);
		}
		public Rectangle PixelsToLayoutUnits(Rectangle value) {
			return DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(value, RichEditControl.DpiX, RichEditControl.DpiY);
		}
		protected internal Size PixelsToLayoutUnits(Size value) {
			return DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(value, RichEditControl.DpiX, RichEditControl.DpiY);
		}
		protected override void OnPaintBackground(PaintEventArgs pevent) {
		}
		protected internal virtual void OnLookAndFeelChanged() {
			RecreatePainter();
		}
		protected internal virtual void RecreatePainter() {
			if (painter != null)
				this.painter.Dispose();
			this.painter = CreatePainter();
		}
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (lookAndFeel != null) {
						lookAndFeel.Dispose();
						lookAndFeel = null;
					}
					if (mouseHandler != null) {
						mouseHandler.Dispose();
						mouseHandler = null;
					}
					if (tickMarkFont != null) {
						tickMarkFont.Dispose();
						tickMarkFont = null;
					}
					if (serviceManager != null) {
						serviceManager.Dispose();
						serviceManager = null;
					}
					if (painter != null) {
						painter.Dispose();
						painter = null;
					}
				}
			} finally {
				base.Dispose(disposing);
			}
		}
		#region InitializeMouseHandlers
		protected internal virtual void InitializeMouseHandler() {
			this.mouseHandler = CreateMouseHandler();
			this.mouseHandler.Initialize();
		}
		#endregion
		#region IServiceContainer Members
		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, callback, promote);
		}
		public void AddService(Type serviceType, ServiceCreatorCallback callback) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, callback);
		}
		public void AddService(Type serviceType, object serviceInstance, bool promote) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, serviceInstance, promote);
		}
		public void AddService(Type serviceType, object serviceInstance) {
			if (serviceManager != null)
				serviceManager.AddService(serviceType, serviceInstance);
		}
		public void RemoveService(Type serviceType, bool promote) {
			if (serviceManager != null)
				serviceManager.RemoveService(serviceType, promote);
		}
		public void RemoveService(Type serviceType) {
			if (serviceManager != null)
				serviceManager.RemoveService(serviceType);
		}
		#endregion
		#region IServiceProvider Members
		public
#if !SL
 new
#endif
 virtual object GetService(Type serviceType) {
			if (serviceManager != null)
				return serviceManager.GetService(serviceType);
			else
				return null;
		}
		#endregion
		public T GetService<T>() where T : class {
			return ServiceUtils.GetService<T>(this);
		}
		public T ReplaceService<T>(T newService) where T : class {
			return ServiceUtils.ReplaceService<T>(this, newService);
		}
		protected internal virtual ServiceManager CreateServiceManager() {
			return new ServiceManager();
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			IKeyboardHandlerService svc = GetService<IKeyboardHandlerService>();
			if (svc != null)
				svc.OnKeyDown(e);
			base.OnKeyDown(e);
			richEditControl.Focus();
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if (DesignMode || !RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return;
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseDown(e);
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if (DesignMode || !RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return;
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseUp(e);
			base.OnMouseUp(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if (DesignMode || !RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return;
			IMouseHandlerService svc = GetService<IMouseHandlerService>();
			if (svc != null)
				svc.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected bool UpdatePageIndex() {
			int index = richEditControl.ActiveView.CaretPosition.LayoutPosition.Page.PageIndex;
			if (index == pageIndex)
				return false;
			pageIndex = index;
			return true;
		}
		protected bool UpdateColumnIndex() {
			int index = richEditControl.ActiveView.CaretPosition.LayoutPosition.PageArea.Columns.IndexOf(richEditControl.ActiveView.CaretPosition.LayoutPosition.Column);
			if (index == columnIndex)
				return false;
			columnIndex = index;
			return true;
		}
		protected bool UpdateParagraphIndex() {
			ParagraphIndex newIndex = FindEndParagraphIndex();
			if (newIndex == paragraphIndex)
				return false;
			paragraphIndex = newIndex;
			return true;
		}
		protected internal virtual ParagraphIndex FindEndParagraphIndex() {
			Selection selection = DocumentModel.Selection;
			PieceTable activePieceTable = DocumentModel.ActivePieceTable;
			ParagraphIndex endParagraphIndex = activePieceTable.FindParagraphIndex(selection.End);
			if (selection.Length > 0 && selection.End == activePieceTable.Paragraphs[endParagraphIndex].LogPosition)
				endParagraphIndex--;
			return endParagraphIndex;
		}
		protected internal SortedList<int> GetTableAlignmentedPositions(TableViewInfo tableViewInfo) {
			SortedList<int> result = new SortedList<int>();
			for (int i = 0; i < tableViewInfo.VerticalBorderPositions.AlignmentedPosition.Count; i++)
				result.Add(tableViewInfo.VerticalBorderPositions.AlignmentedPosition[i]);
			return result;
		}
		protected internal TableCellViewInfo GetTableCellViewInfo(bool isMainPieceTable) {
			Selection selection = DocumentModel.Selection;
			CaretPosition caretPosition = RichEditControl.ActiveView.CaretPosition;
			if (selection.Items.Count == 1 && DocumentModel.Selection.Length == 0) {
				if (caretPosition.Update(DocumentLayoutDetailsLevel.TableCell) && caretPosition.LayoutPosition.TableCell != null)
					return caretPosition.LayoutPosition.TableCell;
				return null;
			}
			else {
				DocumentLogPosition logPosition = DocumentModel.Selection.ActiveSelection.Start;
				DocumentLayout documentLayout = caretPosition.LayoutPosition.DocumentLayout;
				DocumentLayoutPosition layoutPosition;
				if(isMainPieceTable)
					layoutPosition = new DocumentLayoutPosition(documentLayout, caretPosition.LayoutPosition.PieceTable, logPosition);
				else {
					layoutPosition = caretPosition.CreateCaretDocumentLayoutPosition();
					layoutPosition.SetLogPosition(logPosition);
				}
				if (layoutPosition.Update(documentLayout.Pages, DocumentLayoutDetailsLevel.TableCell))
					return layoutPosition.TableCell;
				return null;
			}
		}
		protected internal SortedList<int> GetTableVerticalPositions(bool isMainPieceTable) {
			CaretPosition caretPosition = RichEditControl.ActiveView.CaretPosition;
			SortedList<int> result = new SortedList<int>();
			if (isMainPieceTable && caretPosition.Update(DocumentLayoutDetailsLevel.TableCell) && caretPosition.LayoutPosition.TableCell != null) {
				TableViewInfo tableViewInfo = caretPosition.LayoutPosition.TableCell.TableViewInfo;
				for (int i = 0; i < tableViewInfo.Anchors.Count; i++)
					result.Add(tableViewInfo.Anchors[i].VerticalPosition);
			}
			return result;
		}
	}
	#endregion
	#region HorizontalRulerControl
	[DXToolboxItem(false)]
	public partial class HorizontalRulerControl : RulerControlBase, IHorizontalRulerControl {
		#region Fields
		HorizontalRulerViewInfo viewInfo;
		HorizontalRulerPainter painter;
		int tabTypeIndex;
		#endregion
		public HorizontalRulerControl(RichEditControl richEditControl)
			: base(richEditControl) {
			this.painter = CreatePainterCore();
			this.tabTypeIndex = 0;
			this.viewInfo = CreateViewInfo();
			painter.Initialize();
			this.viewInfo.Initialize();
			this.Bounds = new Rectangle(richEditControl.ClientBounds.X, richEditControl.ClientBounds.Y, richEditControl.ClientBounds.Width, GetRulerHeightInPixels());
		}
		#region Properties
		public int TabTypeIndex { get { return tabTypeIndex; } set { tabTypeIndex = value; } }
		public new HorizontalRulerPainter Painter { get { return painter; } }
		public override RulerViewInfoBase ViewInfoBase { get { return viewInfo; } }
		public HorizontalRulerViewInfo ViewInfo { get { return viewInfo; } }
		protected internal override bool IsVisible { get { return RichEditControl.CalculateHorizontalRulerVisibility(); } }
		#endregion
		public void SetViewInfo(HorizontalRulerViewInfo newViewInfo) {
			viewInfo = newViewInfo;
			viewInfo.Initialize();
		}
		protected internal Paragraph GetParagraph() {
			ParagraphIndex paragraphIndex = CalculateParagraphIndex();
			return DocumentModel.Selection.PieceTable.Paragraphs[paragraphIndex];
		}
		protected internal int GetColumnIndex(bool isMainPieceTable) {
			CaretPosition caretPosition = RichEditControl.ActiveView.CaretPosition;
			if (caretPosition.Update(DocumentLayoutDetailsLevel.Column) && isMainPieceTable)
				return caretPosition.LayoutPosition.PageArea.Columns.IndexOf(caretPosition.LayoutPosition.Column);
			return 0;
		}
		protected internal override MouseHandler CreateMouseHandler() {
			return new HorizontalRulerMouseHandler(this);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
		}
		protected internal virtual HorizontalRulerViewInfo CreateViewInfo() {
			bool isMainPieceTable = DocumentModel.Selection.PieceTable.IsMain;
			Section section = GetSection(DocumentModel.Selection.PieceTable);
			SectionProperties sectionProperties = new SectionProperties(section);
			return CreateViewInfoCore(sectionProperties, isMainPieceTable);
		}
		public virtual HorizontalRulerViewInfo CreateViewInfoCore(SectionProperties sectionProperties, bool isMainPieceTable) {
			Paragraph paragraph = GetParagraph();
			int columnIndex = GetColumnIndex(isMainPieceTable);
			TableCellViewInfo tableCellViewInfo = GetTableCellViewInfo(isMainPieceTable);
			SortedList<int> tableAlignmentedPositions = new SortedList<int>();
			if (tableCellViewInfo != null)
				tableAlignmentedPositions = GetTableAlignmentedPositions(tableCellViewInfo.TableViewInfo);
			return new HorizontalRulerViewInfo(this, sectionProperties, paragraph, columnIndex, isMainPieceTable, tableCellViewInfo, tableAlignmentedPositions);
		}
		protected internal override void InvalidateRulerViewInfo() {
			if (!RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return;
			if (IsVisible) {
				this.viewInfo = CreateViewInfo();
				this.viewInfo.Initialize();
			}
			Repaint();
		}
		protected internal override int GetRulerSizeInPixels() {
			return GetRulerHeightInPixels();
		}
		protected internal int GetRulerHeightInPixels() {
			return ViewInfo.DisplayClientBounds.Height + LayoutUnitsToPixelsV(Painter.PaddingTop + Painter.PaddingBottom);
		}
		protected override void OnPaint(PaintEventArgs e) {
			if (IsVisible)
				DrawRuler(e.Graphics);
		}
		protected internal virtual void DrawRuler(Graphics gr) {
			using (GraphicsCache cache = new GraphicsCache(gr)) {
				Painter.DrawBackground(cache);
				Painter.Draw(cache, null);
				Painter.DrawTabTypeToggle(cache);
			}
		}
		protected internal override void RecreatePainter() {
			base.RecreatePainter();
			if (painter != null)
				this.painter.Dispose();
			this.painter = CreateHorizontalRulerPainter();
		}
		protected internal override IOrientation CreateOrientation() {
			return new HorizontalOrientation();
		}
		protected internal override RulerPainterBase CreatePainter() {
			return CreatePainterCore();
		}
		protected internal virtual HorizontalRulerPainter CreateHorizontalRulerPainter() {
			HorizontalRulerPainter painter = CreatePainterCore();
			painter.Initialize();
			return painter;
		}
		protected internal virtual HorizontalRulerPainter CreatePainterCore() {
			switch (LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Flat:
					return new HorizontalRulerFlatPainter(this);
				case ActiveLookAndFeelStyle.UltraFlat:
					return new HorizontalRulerUltraFlatPainter(this);
				case ActiveLookAndFeelStyle.Style3D:
					return new HorizontalRulerStyle3DPainter(this);
				case ActiveLookAndFeelStyle.Office2003:
					return new HorizontalRulerOffice2003Painter(this);
				case ActiveLookAndFeelStyle.WindowsXP:
					return new HorizontalRulerWindowsXPPainter(this);
				case ActiveLookAndFeelStyle.Skin:
				default:
					return new HorizontalRulerSkinPainter(this);
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (painter != null) {
					this.painter.Dispose();
					this.painter = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal override Point GetPhysicalPoint(Point point) {
			int x = PixelsToLayoutUnitsH(point.X) - RichEditControl.ViewBounds.Left;
			int y = PixelsToLayoutUnitsV(point.Y);
			return new Point(x, y);
		}
		public virtual Size CalculateHotZoneSize(HorizontalRulerHotZone hotZone) {
			return Painter.ElementPainter.CalculateHotZoneSize(hotZone);
		}
		protected internal override bool CanUpdate() {
			if (!RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return false;
			return UpdatePageIndex() || UpdateColumnIndex() || UpdateParagraphIndex();
		}
		public Size GetTabTypeToggleActiveAreaSize() {
			return Painter.GetTabTypeToggleActiveAreaSize();
		}
		public Rectangle CalculateTabTypeToggleBackgroundBounds() {
			Rectangle bounds = ClientRectangle;
			bounds.Width = bounds.Height;
			return bounds;
		}
	}
	#endregion
	#region VerticalRulerControl
	[DXToolboxItem(false)]
	public partial class VerticalRulerControl : RulerControlBase, IVerticalRulerControl {
		#region Fields
		VerticalRulerViewInfo viewInfo;
		VerticalRulerPainter painter;
		#endregion
		public VerticalRulerControl(RichEditControl richEditControl)
			: base(richEditControl) {
			this.painter = CreatePainterCore();
			this.viewInfo = CreateViewInfo();
			painter.Initialize();
			this.viewInfo.Initialize();
		}
		#region Properties
		public new VerticalRulerPainter Painter { get { return painter; } }
		public override RulerViewInfoBase ViewInfoBase { get { return viewInfo; } }
		public VerticalRulerViewInfo ViewInfo { get { return viewInfo; } }
		bool IRulerControl.IsVisible { get { return this.IsVisible; } }
		protected internal override bool IsVisible { get { return RichEditControl.CalculateVerticalRulerVisibility(); } }
		#endregion
		protected internal override void InvalidateRulerViewInfo() {
			if (!RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return;
			if (IsVisible) {
				this.viewInfo = CreateViewInfo();
				this.viewInfo.Initialize();
			}
			Repaint();
		}
		protected internal virtual VerticalRulerViewInfo CreateViewInfo() {
			bool isMainPieceTable = DocumentModel.Selection.PieceTable.IsMain;
			TableCellViewInfo tableCellViewInfo = GetTableCellViewInfo(isMainPieceTable);
			SortedList<int> verticalPositions = new SortedList<int>();
			if (tableCellViewInfo != null) {
				TableCellVerticalAnchorCollection anchors = tableCellViewInfo.TableViewInfo.Anchors;
				for (int i = 1; i < anchors.Count; i++)
					verticalPositions.Add(anchors[i].VerticalPosition);
			}
			Section section = GetSection(DocumentModel.Selection.PieceTable);
			return new VerticalRulerViewInfo(this, new SectionProperties(section), isMainPieceTable, verticalPositions, tableCellViewInfo);
		}
		public void SetViewInfo(VerticalRulerViewInfo newViewInfo) {
			viewInfo = newViewInfo;
			viewInfo.Initialize();
		}
		protected internal override MouseHandler CreateMouseHandler() {
			return new VerticalRulerMouseHandler(this);
		}
		protected internal override int GetRulerSizeInPixels() {
			return GetRulerWidthInPixels();
		}
		protected internal int GetRulerWidthInPixels() {
			return LayoutUnitsToPixelsH(ViewInfo.GetRulerSize());
		}
		protected override void OnPaint(PaintEventArgs e) {
			if (IsVisible)
				DrawRuler(e.Graphics);
		}
		protected internal virtual void DrawRuler(Graphics gr) {
			using (GraphicsCache cache = new GraphicsCache(gr)) {
				Painter.DrawBackground(cache);
				Painter.Draw(cache, null);
			}
		}
		protected internal override void RecreatePainter() {
			base.RecreatePainter();
			if (painter != null)
				this.painter.Dispose();
			this.painter = CreateVerticalRulerPainter();
		}
		protected internal override IOrientation CreateOrientation() {
			return new VerticalOrientation();
		}
		protected internal override RulerPainterBase CreatePainter() {
			return CreatePainterCore();
		}
		protected internal virtual VerticalRulerPainter CreateVerticalRulerPainter() {
			VerticalRulerPainter painter = CreatePainterCore();
			painter.Initialize();
			return painter;
		}
		protected internal virtual VerticalRulerPainter CreatePainterCore() {
			switch (LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Flat:
					return new VerticalRulerFlatPainter(this);
				case ActiveLookAndFeelStyle.UltraFlat:
					return new VerticalRulerUltraFlatPainter(this);
				case ActiveLookAndFeelStyle.Style3D:
					return new VerticalRulerStyle3DPainter(this);
				case ActiveLookAndFeelStyle.Office2003:
					return new VerticalRulerOffice2003Painter(this);
				case ActiveLookAndFeelStyle.WindowsXP:
					return new VerticalRulerWindowsXPPainter(this);
				case ActiveLookAndFeelStyle.Skin:
				default:
					return new VerticalRulerSkinPainter(this);
			}
		}
		protected override void Dispose(bool disposing) {
			try {
				if (painter != null) {
					this.painter.Dispose();
					this.painter = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal override Point GetPhysicalPoint(Point point) {
			int x = PixelsToLayoutUnitsH(point.X);
			int y = PixelsToLayoutUnitsV(point.Y) - RichEditControl.ViewBounds.Top;
			return new Point(x, y);
		}
		protected internal override bool CanUpdate() {
			if (!RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return false;
			return UpdatePageIndex();
		}
	}
	#endregion
}
#endif
