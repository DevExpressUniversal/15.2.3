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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Services;
using DevExpress.Services.Internal;
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Ruler;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.Xpf.RichEdit.Ruler;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using KeyboardHandlerClass = DevExpress.Utils.KeyboardHandler.KeyboardHandler;
#if SL
using PlatformIndependentColor = System.Windows.Media.Color;
using PlatformIndependentKeyEventArgs = DevExpress.Data.KeyEventArgs;
using PlatformIndependentMouseEventArgs = DevExpress.Data.MouseEventArgs;
using PlatformIndependentDragEventArgs = DevExpress.Utils.DragEventArgs;
using PlatformIndependentMouseButtons = DevExpress.Data.MouseButtons;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
using PlatformIndependentFontStyle = DevExpress.Xpf.Drawing.FontStyle;
#else
using PlatformIndependentColor = System.Drawing.Color;
using PlatformIndependentMouseEventArgs = System.Windows.Forms.MouseEventArgs;
using PlatformIndependentKeyEventArgs = System.Windows.Forms.KeyEventArgs;
using PlatformIndependentDragEventArgs = System.Windows.Forms.DragEventArgs;
using PlatformIndependentMouseButtons = System.Windows.Forms.MouseButtons;
using PlatformIndependentFontStyle = System.Drawing.FontStyle;
#endif
namespace DevExpress.Xpf.RichEdit.Controls.Internal {
	public abstract partial class RulerControlBase : Control {
		#region Fields
		const string SurfaceName = "Surface";
		Panel surface;
		XpfDrawingSurface drawingSurface;
		const int minWidth = 5;
		const int minHeight = 5;
		float dpiX;
		float dpiY;
		ServiceManager serviceManager;
		MouseHandler mouseHandler;
		FontInfo tickMarkFont;
		PlatformIndependentColor numberTickMarkColor;
		RichEditControl richEditControl;
		IOrientation orientation;
		RulerPainterBase painter;
		int pageIndex = -1;
		int columnIndex = -1;
		ParagraphIndex paragraphIndex = new ParagraphIndex(- 1);
		RulerNumberTickMarkControl numberTickMark;
		#endregion
		public void InitializeBase(RichEditControl richEditControl) {
			Guard.ArgumentNotNull(richEditControl, "richEditControl");
			this.richEditControl = richEditControl;
			this.orientation = CreateOrientation();
			UpdateTickMarkFont();
			AddHandler(System.Windows.FrameworkElement.KeyDownEvent, new System.Windows.Input.KeyEventHandler(OnKeyDownEventHandler), true);
			this.dpiX = richEditControl.DpiX;
			this.dpiY = richEditControl.DpiY;
			this.painter = new RulerPainterBase(this);
			this.serviceManager = CreateServiceManager();
			InitializeMouseHandler();
			AddServices();
		}
		#region Properties
		protected internal RichEditControl RichEditControl { get { return richEditControl; } }
		public FontInfo TickMarkFont { get { return tickMarkFont; } }
		public PlatformIndependentColor NumberTickMarkColor { get { return numberTickMarkColor; } }
		public RulerPainterBase Painter { get { return painter; } }
		public float DpiX { get { return dpiX; } }
		public float DpiY { get { return dpiY; } }
		public float ZoomFactor { get { return RichEditControl.ActiveView.ZoomFactor; } }
		public DocumentModel DocumentModel { get { return RichEditControl.DocumentModel; } }
		public MouseHandler MouseHandler { get { return mouseHandler; } }
		public Panel Surface { get { return surface; } }
		public abstract RulerViewInfoBase ViewInfoBase { get; }
		protected XpfDrawingSurface DrawingSurface { get { return drawingSurface; } }
		public IOrientation Orientation { get { return orientation; } }
		protected internal abstract bool InnerIsVisible { get; }
		#endregion
		public void OnKeyDownEventHandler(object sender, System.Windows.Input.KeyEventArgs e) {
			IKeyboardHandlerService svc = (IKeyboardHandlerService)GetService(typeof(IKeyboardHandlerService));
			if (svc != null)
				svc.OnKeyDown(e.ToPlatformIndependent());
			base.OnKeyDown(e);
			richEditControl.SetFocus();
		}
		protected internal abstract IOrientation CreateOrientation();
		public abstract void OnTransform();
		public abstract void RepaintCore();
		protected internal abstract bool CanUpdate();
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CreateSurface();
			RulerNumberTickMarkControl numberTickMark = GetTemplateChild("PART_NumberTickMark") as RulerNumberTickMarkControl;
			if (numberTickMark != null) {
				this.numberTickMark = numberTickMark;
				this.numberTickMark.ApplyTemplate();
			}
		}
		protected void CreateSurface() {
			this.surface = GetSurface();
			this.drawingSurface = new XpfDrawingSurface(surface.Children);
		}
		protected virtual Panel GetSurface() {
			return GetTemplateChild(SurfaceName) as Panel;
		}
		public void Repaint() {
			if (surface == null || !InnerIsVisible)
				return;
			OnTransform();
			UpdateTickMarkFont();
			if (!ViewInfoBase.IsReady)
				this.InvalidateRulerViewInfoCore();
			RepaintCore();
		}
		public void Reset() {
			UpdateTickMarkFont();
			InvalidateRulerViewInfo();
		}
		protected internal virtual void UpdateTickMarkFont() {
			if (tickMarkFont != null && !tickMarkFont.IsDisposed)
				return;
			BackgroundFormatter formatter = richEditControl.InnerControl.Formatter;
			formatter.SuspendWorkerThread();
			try {
				FontCache fontCache = richEditControl.DocumentModel.FontCache;
				int fontIndex = fontCache.CalcFontIndex("Arial", 14, false, false, CharacterFormattingScript.Normal, false, false);
				this.tickMarkFont = fontCache[fontIndex];
			}
			finally {
				formatter.ResumeWorkerThread();
			}
			ApplyTemplate();
			this.numberTickMarkColor = DXColor.FromArgb(110, 110, 110);
			if (numberTickMark != null) {
				SolidColorBrush brush = this.numberTickMark.Foreground as SolidColorBrush;
				if (brush != null) {
					System.Windows.Media.Color color = brush.Color;
					this.numberTickMarkColor = DXColor.FromArgb(color.A, color.R, color.G, color.B);
				}
			}
		}
		protected internal virtual void InvalidateRulerViewInfo() {
			InvalidateRulerViewInfoCore();
			Repaint();
		}
		protected internal abstract void InvalidateRulerViewInfoCore();
		protected internal abstract MouseHandler CreateMouseHandler();
		protected internal abstract int GetRulerHeightInPixels();
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
			if (pieceTable.IsMain) {
				ParagraphIndex paragraphIndex = CalculateParagraphIndex();
				SectionIndex sectionIndex = DocumentModel.MainPieceTable.LookupSectionIndexByParagraphIndex(paragraphIndex);
				return DocumentModel.Sections[sectionIndex];
			}
			else if (pieceTable.IsTextBox) {
				TextBoxContentType textBoxPieceTable = (TextBoxContentType)pieceTable.ContentType;
				FloatingObjectAnchorRun anchorRun = textBoxPieceTable.AnchorRun;
				if (anchorRun.PieceTable.IsMain) {
					SectionIndex sectionIndex = DocumentModel.MainPieceTable.LookupSectionIndexByParagraphIndex(anchorRun.Paragraph.Index);
					return DocumentModel.Sections[sectionIndex];
				}
				else {
					CaretPosition caretPosition = RichEditControl.ActiveView.CaretPosition;
					return caretPosition.LayoutPosition.PageArea.Section;
				}
			}
			else {
				Section section = DocumentModel.GetActiveSection();
				Guard.ArgumentNotNull(section, "section");
				return section;
			}
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
		virtual object GetService(Type serviceType) {
			if (serviceManager != null)
				return serviceManager.GetService(serviceType);
			else
				return null;
		}
		#endregion
		public T GetService<T>() {
			if (serviceManager != null)
				return (T)serviceManager.GetService(typeof(T));
			else
				return default(T);
		}
		protected internal virtual ServiceManager CreateServiceManager() {
			return new ServiceManager();
		}
		protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
			if (!RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return;
			IMouseHandlerService svc = (IMouseHandlerService)GetService(typeof(IMouseHandlerService));
			if (svc != null)
				svc.OnMouseUp(ConvertMouseEventArgs(e, PlatformIndependentMouseButtons.Left, 1));
			base.OnMouseLeftButtonUp(e);
		}
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			if (!RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return;
			IMouseHandlerService svc = (IMouseHandlerService)GetService(typeof(IMouseHandlerService));
			if (svc != null)
				svc.OnMouseDown(ConvertMouseEventArgs(e, PlatformIndependentMouseButtons.Left, 1));
			base.OnMouseLeftButtonDown(e);
		}
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e) {
			if (!RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return;
			IMouseHandlerService svc = (IMouseHandlerService)GetService(typeof(IMouseHandlerService));
			if (svc != null)
				svc.OnMouseMove(ConvertMouseEventArgs(e, PlatformIndependentMouseButtons.None, 1));
			base.OnMouseMove(e);
		}
		PlatformIndependentMouseEventArgs ConvertMouseEventArgs(System.Windows.Input.MouseEventArgs e, PlatformIndependentMouseButtons buttons, int clicks) {
			System.Windows.Point p = e.GetPosition(surface);
#if SL
			return new PlatformIndependentMouseEventArgs(buttons, clicks, (int)p.X, (int)p.Y, 0, KeyboardHandlerClass.IsShiftPressed, KeyboardHandlerClass.IsControlPressed);
#else
			return new PlatformIndependentMouseEventArgs(buttons, clicks, (int)p.X, (int)p.Y, 0);
#endif
		}
		protected bool UpdatePageIndex() {
			int index = richEditControl.ActiveView.CaretPosition.LayoutPosition.Page.PageIndex;
			if (index == pageIndex)
				return false;
			pageIndex = index;
			return true;
		}
		protected bool UpdateColumnIndex() {
			int index = GetColumnIndex(true);
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
		protected internal int GetColumnIndex(bool isMainPieceTable) {
			CaretPosition caretPosition = RichEditControl.ActiveView.CaretPosition;
			if (caretPosition.Update(DocumentLayoutDetailsLevel.Column) && isMainPieceTable)
				return caretPosition.LayoutPosition.PageArea.Columns.IndexOf(caretPosition.LayoutPosition.Column);
			return 0;
		}
		protected internal SortedList<int> GetTableAlignmentedPositions(TableViewInfo tableViewInfo) {
			SortedList<int> result = new SortedList<int>();
			for (int i = 0; i < tableViewInfo.VerticalBorderPositions.AlignmentedPosition.Count; i++)
				result.Add(tableViewInfo.VerticalBorderPositions.AlignmentedPosition[i]);
			return result;
		}
		protected internal TableCellViewInfo GetTableCellViewInfo(bool isMainPieceTable) {
			CaretPosition caretPosition = RichEditControl.ActiveView.CaretPosition;
			if (caretPosition.Update(DocumentLayoutDetailsLevel.TableCell) && isMainPieceTable && caretPosition.LayoutPosition.TableCell != null)
				return caretPosition.LayoutPosition.TableCell;
			return null;
		}
		protected internal SortedList<int> GetTableVerticalPositions(bool isMainPieceTable) {
			CaretPosition caretPosition = RichEditControl.ActiveView.CaretPosition;
			SortedList<int> result = new SortedList<int>();
			if (caretPosition.Update(DocumentLayoutDetailsLevel.TableCell) && isMainPieceTable && caretPosition.LayoutPosition.TableCell != null) {
				TableViewInfo tableViewInfo = caretPosition.LayoutPosition.TableCell.TableViewInfo;
				for (int i = 0; i < tableViewInfo.Anchors.Count; i++)
					result.Add(tableViewInfo.Anchors[i].VerticalPosition);
			}
			return result;
		}
	}
	#region HorizontalRulerControl
	[ToolboxItem(false)]
	public partial class HorizontalRulerControl : RulerControlBase, IHorizontalRulerControl, INotifyPropertyChanged {
		#region Fields
		HorizontalRulerViewInfo viewInfo;
		HorizontalRulerPainter painter;
		int tabTypeIndex;
		LeftIndentHotZoneControl indent;
		RightIndentHotZoneControl rightIndent;
		FirstLineIndentHotZoneControl firstLineIndent;
		LeftIndentBottomHotZoneControl indentBottom;
		HorizontalHotZoneControl tab;
		TabTypeToggleHotZoneControl tabTypeToggle;
		#endregion
		public HorizontalRulerControl() {
			this.DefaultStyleKey = typeof(HorizontalRulerControl);
		}
		public void Initialize(RichEditControl richEditControl) {
			base.InitializeBase(richEditControl);
			this.tabTypeIndex = 0;
			painter = new HorizontalRulerPainter(this);
		}
		#region Properties
		public new HorizontalRulerPainter Painter { get { return painter; } }
		public int TabTypeIndex { get { return tabTypeIndex; } set { tabTypeIndex = value; } }
		public override RulerViewInfoBase ViewInfoBase { get { return viewInfo; } }
		public HorizontalRulerViewInfo ViewInfo { get { return viewInfo; } }
		bool IRulerControl.IsVisible { get { return this.InnerIsVisible; } }
		protected internal override bool InnerIsVisible { get { return RichEditControl.CalculateHorizontalRulerVisibility(); } }
		protected internal LeftIndentHotZoneControl Indent { get { return indent; } }
		protected internal RightIndentHotZoneControl RightIndent { get { return rightIndent; } }
		protected internal FirstLineIndentHotZoneControl FirstLineIndent { get { return firstLineIndent; } }
		protected internal LeftIndentBottomHotZoneControl IndentBottom { get { return indentBottom; } }
		protected internal HorizontalHotZoneControl Tab { get { return tab; } }
		protected internal TabTypeToggleHotZoneControl TabTypeToggle { get { return tabTypeToggle; } }
		IRichEditControl IRulerControl.RichEditControl { get { return this.RichEditControl; } }
		#endregion
		public void SetViewInfo(HorizontalRulerViewInfo newViewInfo) {
			viewInfo = newViewInfo;
			viewInfo.Initialize();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			LeftIndentHotZoneControl indent = GetTemplateChild("PART_Indent") as LeftIndentHotZoneControl;
			if (indent != null)
				this.indent = indent;
			RightIndentHotZoneControl rightIndent = GetTemplateChild("PART_RightIndent") as RightIndentHotZoneControl;
			if (rightIndent != null)
				this.rightIndent = rightIndent;
			FirstLineIndentHotZoneControl firstLineIndent = GetTemplateChild("PART_TopIndent") as FirstLineIndentHotZoneControl;
			if (firstLineIndent != null)
				this.firstLineIndent = firstLineIndent;
			LeftIndentBottomHotZoneControl indentBottom = GetTemplateChild("PART_BottomIndent") as LeftIndentBottomHotZoneControl;
			if (indentBottom != null)
				this.indentBottom = indentBottom;
			HorizontalHotZoneControl tab = GetTemplateChild("PART_Tab") as HorizontalHotZoneControl;
			if (tab != null)
				this.tab = tab;
			TabTypeToggleHotZoneControl tabTypeToggle = GetTemplateChild("PART_TabTypeToggle") as TabTypeToggleHotZoneControl;
			if (tabTypeToggle != null)
				this.tabTypeToggle = tabTypeToggle;
		}
		protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize) {
			base.MeasureOverride(availableSize);
			if (viewInfo == null) {
				this.viewInfo = CreateViewInfo();
				this.viewInfo.Initialize();
			}
			int controlHeight = GetRulerHeightInPixels();
			double width = 0;
			if (availableSize.Width != Double.PositiveInfinity)
				width = availableSize.Width;
			return new System.Windows.Size(width, controlHeight); ;
		}
		protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize) {
			System.Windows.Size size = new System.Windows.Size(finalSize.Width, GetRulerHeightInPixels());
			RectangleGeometry clipRectGeometry = new RectangleGeometry();
			clipRectGeometry.Rect = new System.Windows.Rect(new System.Windows.Point(0, 0), size);
			Clip = clipRectGeometry;
			base.ArrangeOverride(finalSize);
			return size;
		}
		public override void RepaintCore() {
			this.painter = new HorizontalRulerPainter(this, DrawingSurface);
			painter.Clear();
			painter.Draw();
		}
		protected internal Paragraph GetParagraph() {
			ParagraphIndex paragraphIndex = CalculateParagraphIndex();
			return DocumentModel.Selection.PieceTable.Paragraphs[paragraphIndex];
		}
		protected internal override MouseHandler CreateMouseHandler() {
			return new HorizontalRulerMouseHandler(this);
		}
		protected internal virtual HorizontalRulerViewInfo CreateViewInfo() {
			bool isMainPieceTable = Object.ReferenceEquals(DocumentModel.Selection.PieceTable, DocumentModel.MainPieceTable);
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
		int IRulerControl.GetRulerSizeInPixels() {
			return ViewInfo.DisplayClientBounds.Height + LayoutUnitsToPixelsV(Painter.PaddingTop + Painter.PaddingBottom);
		}
		protected internal override int GetRulerHeightInPixels() {
			return ViewInfo.DisplayClientBounds.Height + LayoutUnitsToPixelsV(Painter.PaddingTop + Painter.PaddingBottom);
		}
		protected internal virtual void DrawRuler(Graphics gr) {
		}
		protected internal override void AddServices() {
			AddService(typeof(IMouseHandlerService), new RulerMouseHandlerService(this));
		}
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected virtual void RaisePropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		protected internal override void InvalidateRulerViewInfoCore() {
			if (InnerIsVisible) {
				this.viewInfo = CreateViewInfo();
				this.viewInfo.Initialize();
			}
		}
		protected internal override IOrientation CreateOrientation() {
			return new HorizontalOrientation();
		}
		public override void OnTransform() {
			if (Surface != null) {
				System.Windows.Media.TranslateTransform transform = new System.Windows.Media.TranslateTransform();
				transform.X = -GetPhysicalLeftInvisibleWidthInPixel();
				if (RichEditControl.VerticalRulerVisibility == System.Windows.Visibility.Visible)
					transform.X += GetRulerHeightInPixels();
				Surface.RenderTransform = transform;
			}
		}
		protected internal int GetPhysicalLeftInvisibleWidthInPixel() {
			int physicalLeftInvisibleWidth = RichEditControl.ActiveView.HorizontalScrollController.GetPhysicalLeftInvisibleWidth();
			return LayoutUnitsToPixelsH(physicalLeftInvisibleWidth);
		}
		public Size CalculateHotZoneSize(HorizontalRulerHotZone hotZone) { 
			Type type = hotZone.GetType();
			if (type == typeof(LeftIndentHotZone)) {
				if (Indent != null)
					return CalculateHotZoneSizeCore(Indent);
			}
			if (type == typeof(RightIndentHotZone)) {
				if (RightIndent != null)
					return CalculateHotZoneSizeCore(RightIndent);
			}
			if (type == typeof(FirstLineIndentHotZone)) {
				if (FirstLineIndent != null)
					return CalculateHotZoneSizeCore(FirstLineIndent);
			}
			if (type == typeof(LeftBottomHotZone)) {
				if (IndentBottom != null)
					return CalculateHotZoneSizeCore(IndentBottom);
			}
			if (typeof(TabHotZone).IsAssignableFrom(type)) {
				if (Tab != null)
					return CalculateHotZoneSizeCore(Tab);
			}
			if (typeof(TabTypeToggleHotZone).IsAssignableFrom(type)) {
				if (TabTypeToggle != null)
					return CalculateHotZoneSizeCore(TabTypeToggle);
			}
			return PixelsToLayoutUnits(new System.Drawing.Size(9, 7));
		}
		protected internal Size CalculateHotZoneSizeCore(Control hotZoneControl) {
			return PixelsToLayoutUnits(new System.Drawing.Size((int)(hotZoneControl.DesiredSize.Width - hotZoneControl.Margin.Left - hotZoneControl.Margin.Right), (int)(hotZoneControl.DesiredSize.Height - hotZoneControl.Margin.Top - hotZoneControl.Margin.Bottom)));
		}
		internal Rectangle CalculateHotZoneMarginInPixels(RulerHotZone hotZone) {
			Type type = hotZone.GetType();
			if (type == typeof(LeftIndentHotZone) || type == typeof(RightIndentHotZone)) {
				if (Indent != null)
					return CalculateHotZoneMarginCore(Indent);
			}
			if (type == typeof(RightIndentHotZone)) {
				if (RightIndent != null)
					return CalculateHotZoneMarginCore(RightIndent);
			}
			if (type == typeof(FirstLineIndentHotZone)) {
				if (FirstLineIndent != null)
					return CalculateHotZoneMarginCore(FirstLineIndent);
			}
			if (type == typeof(LeftBottomHotZone)) {
				if (IndentBottom != null)
					return CalculateHotZoneMarginCore(IndentBottom);
			}
			if (typeof(TabHotZone).IsAssignableFrom(type)) {
				if (Tab != null)
					return CalculateHotZoneMarginCore(Tab);
			}
			if (typeof(TabTypeToggleHotZone).IsAssignableFrom(type)) {
				if (TabTypeToggle != null)
					return CalculateHotZoneMarginCore(TabTypeToggle);
			}
			return new Rectangle();
		}
		protected internal Rectangle CalculateHotZoneMarginCore(Control hotZoneControl) {
			return new Rectangle((int)hotZoneControl.Margin.Left, (int)hotZoneControl.Margin.Top, (int)(hotZoneControl.Margin.Right - hotZoneControl.Margin.Left), (int)(hotZoneControl.Margin.Bottom - hotZoneControl.Margin.Top)); 
		}
		protected internal Rectangle CalculateToggleHotZoneBounds() {
			Size size = PixelsToLayoutUnits(new System.Drawing.Size(9, 7));
			Rectangle bounds = new Rectangle(0 + ((int)Surface.ActualHeight - size.Width) / 2, 0 + ((int)Surface.ActualHeight - size.Height) / 2, size.Width, size.Height);
			return PixelsToLayoutUnits(bounds);
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
			int width = GetRulerHeightInPixels();
			int padding = 0;
			if (RichEditControl.VerticalRulerVisibility == System.Windows.Visibility.Visible)
				padding = GetRulerHeightInPixels();
			return new Rectangle(-padding, -LayoutUnitsToPixelsV(Painter.PaddingTop), width, width);
		}
	}
	#endregion
	#region VerticalRulerControl
	[ToolboxItem(false)]
	public partial class VerticalRulerControl : RulerControlBase, IVerticalRulerControl, INotifyPropertyChanged {
		#region Fields
		VerticalRulerViewInfo viewInfo;
		VerticalRulerPainter painter;
		#endregion
		public VerticalRulerControl() {
			this.DefaultStyleKey = typeof(VerticalRulerControl);
		}
		public void Initialize(RichEditControl richEditControl) {
			base.InitializeBase(richEditControl);
			painter = new VerticalRulerPainter(this);
			this.viewInfo = CreateViewInfo();
			this.viewInfo.Initialize();
		}
		#region Properties
		public new VerticalRulerPainter Painter { get { return painter; } }
		public override RulerViewInfoBase ViewInfoBase { get { return viewInfo; } }
		public VerticalRulerViewInfo ViewInfo { get { return viewInfo; } }
		bool IRulerControl.IsVisible { get { return this.InnerIsVisible; } }
		IRichEditControl IRulerControl.RichEditControl { get { return this.RichEditControl; } }
		protected internal override bool InnerIsVisible { get { return RichEditControl.CalculateVerticalRulerVisibility(); } }
		#endregion
		protected internal virtual VerticalRulerViewInfo CreateViewInfo() {
			bool isMainPieceTable = Object.ReferenceEquals(DocumentModel.Selection.PieceTable, DocumentModel.MainPieceTable);
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
		protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize) {
			base.MeasureOverride(availableSize);
			return new System.Windows.Size(GetRulerHeightInPixels(), 0);
		}
		protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize) {
			System.Windows.Size size = new System.Windows.Size(GetRulerHeightInPixels(), finalSize.Height);
			RectangleGeometry clipRectGeometry = new RectangleGeometry();
			clipRectGeometry.Rect = new System.Windows.Rect(new System.Windows.Point(0, 0), size);
			Clip = clipRectGeometry;
			base.ArrangeOverride(finalSize);
			return size;
		}
		public void SetViewInfo(VerticalRulerViewInfo newViewInfo) {
			viewInfo = newViewInfo;
			viewInfo.Initialize();
		}
		protected internal override int GetRulerHeightInPixels() {
			return (int)ViewInfo.DisplayClientBounds.Width + LayoutUnitsToPixelsV(Painter.PaddingTop + Painter.PaddingBottom);
		}
		protected internal override MouseHandler CreateMouseHandler() {
			return new VerticalRulerMouseHandler(this);
		}
		#region IRulerControl Members
		public int GetRulerSizeInPixels() {
			throw new NotImplementedException();
		}
		#endregion
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged {
			add { onPropertyChanged += value; }
			remove { onPropertyChanged -= value; }
		}
		protected virtual void RaisePropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		protected internal override void InvalidateRulerViewInfoCore() {
			if (InnerIsVisible) {
				this.viewInfo = CreateViewInfo();
				this.viewInfo.Initialize();
			}
		}
		public override void RepaintCore() {
			this.painter = new VerticalRulerPainter(this, DrawingSurface);
			painter.Clear();
			painter.Draw();
		}
		protected internal override IOrientation CreateOrientation() {
			return new VerticalOrientation();
		}
		public override void OnTransform() {
			if (Surface != null) {
				System.Windows.Media.TranslateTransform transform = new System.Windows.Media.TranslateTransform();
				if (RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.PageArea)) {
					transform.Y = LayoutUnitsToPixelsV(RichEditControl.ActiveView.CaretPosition.PageViewInfo.ClientBounds.Y);
					Surface.RenderTransform = transform;
				}
			}
		}
		protected internal override bool CanUpdate() {
			if (!RichEditControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.Column))
				return false;
			return UpdatePageIndex();
		}
	}
	#endregion
}
