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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.Office.Layout;
using RichEditLayoutPage = DevExpress.XtraRichEdit.Layout.Page;
using DevExpress.Office.API.Internal;
using DevExpress.Office.Drawing;
#if !SL && !WPF
using RulerFont = System.Drawing.Font;
#else
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Ruler;
using DevExpress.Xpf.RichEdit.Controls.Internal;
using RulerFont = DevExpress.Office.Drawing.FontInfo;
#endif
#if !SL && !WPF
namespace DevExpress.XtraRichEdit.Ruler {
#else
namespace DevExpress.Xpf.RichEdit.Ruler {
#endif
	public interface IRulerControl2 : IRulerControl {
		float ZoomFactor { get; }
		DocumentModel DocumentModel { get; }
		RulerViewInfoBase ViewInfoBase { get; }
		float DpiX { get; }
		float DpiY { get; }
#if !SL && !WPF
		Rectangle Bounds { get; set; }
#endif
		RulerFont TickMarkFont { get; }
		RulerPainterBase Painter { get; }
		IOrientation Orientation { get; }
		void Reset();
		Rectangle LayoutUnitsToPixels(Rectangle value);
		Rectangle PixelsToLayoutUnits(Rectangle value);
	}
	public interface IHorizontalRulerControl : IRulerControl2 {
		HorizontalRulerViewInfo ViewInfo { get; }
		int TabTypeIndex { get; set; }
		HorizontalRulerViewInfo CreateViewInfoCore(SectionProperties sectionProperties, bool isMainPieceTable);
		void SetViewInfo(HorizontalRulerViewInfo newViewInfo);
		Size CalculateHotZoneSize(HorizontalRulerHotZone hotZone);
		Size GetTabTypeToggleActiveAreaSize();
		Rectangle CalculateTabTypeToggleBackgroundBounds();
	}
	public interface IVerticalRulerControl : IRulerControl2 {
		VerticalRulerViewInfo ViewInfo { get; }
		void SetViewInfo(VerticalRulerViewInfo newViewInfo);
	}
	#region RulerViewInfoBase (abstract class)
	public abstract class RulerViewInfoBase : INotifyPropertyChanged {
#if !SL
		static Graphics measureGraphics;
		static RulerViewInfoBase() {
			measureGraphics = Graphics.FromHwnd(IntPtr.Zero);
		}
#endif
		#region Fields
		readonly List<RulerHotZone> hotZones;
		readonly SectionProperties sectionProperties;
		Rectangle bounds;
		readonly List<RulerTickmark> rulerTickmarks;
		readonly Size textSize;
		readonly IRulerControl2 control;
		readonly List<RectangleF> activeAreaCollection;
		readonly List<RectangleF> displayActiveAreaCollection;
		readonly List<RectangleF> spaceAreaCollection;
		readonly List<RectangleF> displaySpaceAreaCollection;
		Rectangle clientBounds;
		RectangleF halfTickBounds;
		RectangleF quarterTickBounds;
		Rectangle displayClientBounds;
		Rectangle displayHalfTickBounds;
		Rectangle displayQuarterTickBounds;
		readonly bool isMainPieceTable;
		bool isReady;
		#endregion
		protected RulerViewInfoBase(IRulerControl2 control, SectionProperties sectionProperties, bool isMainPieceTable) {
			Guard.ArgumentNotNull(control, "control");
			this.isMainPieceTable = isMainPieceTable;
			this.control = control;
			DocumentLayoutUnitConverter unitConverter = control.DocumentModel.LayoutUnitConverter;
#if !SL && !WPF
			this.bounds = unitConverter.PixelsToLayoutUnits(control.Bounds, Control.DpiX, Control.DpiY);
#endif
			this.rulerTickmarks = new List<RulerTickmark>();
			this.sectionProperties = sectionProperties;
			this.textSize = MeasureString("yW");
			this.activeAreaCollection = new List<RectangleF>();
			this.displayActiveAreaCollection = new List<RectangleF>();
			this.spaceAreaCollection = new List<RectangleF>();
			this.displaySpaceAreaCollection = new List<RectangleF>();
			this.clientBounds = CalculateClientBounds(control);
			CalculateBounds(unitConverter);
			this.hotZones = new List<RulerHotZone>();
		}
		protected void CalculateBounds(DocumentLayoutUnitConverter unitConverter) {
			this.displayClientBounds = unitConverter.LayoutUnitsToPixels(clientBounds, Control.DpiX, Control.DpiY);
			this.halfTickBounds = CalculateHalfTickBounds();
			this.displayHalfTickBounds = unitConverter.LayoutUnitsToPixels(Rectangle.Round(halfTickBounds), Control.DpiX, Control.DpiY);
			this.quarterTickBounds = CalculateQuarterTickBounds();
			this.displayQuarterTickBounds = unitConverter.LayoutUnitsToPixels(Rectangle.Round(quarterTickBounds), Control.DpiX, Control.DpiY);
		}
		protected internal virtual void Initialize() {
			CalculateActiveAreaCollection();
			CalculateSpaceAreaCollection();
			CalculateRulerSeparators();
		}
		protected void SetClientBounds(Rectangle bounds) {
			this.clientBounds = bounds;
		}
		#region Properties
		protected internal bool IsMainPieceTable { get { return isMainPieceTable; } }
		public IRulerControl2 Control { get { return control; } }
		protected internal DocumentModelUnitToLayoutUnitConverter ToDocumentLayoutUnitConverter { get { return control.DocumentModel.ToDocumentLayoutUnitConverter; } }
		public SectionProperties SectionProperties { get { return sectionProperties; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public List<RulerTickmark> RulerTickmarks { get { return rulerTickmarks; } }
		public Size TextSize { get { return textSize; } }
		public List<RectangleF> SpaceAreaCollection { get { return spaceAreaCollection; } }
		public List<RectangleF> ActiveAreaCollection { get { return activeAreaCollection; } }
		public Rectangle ClientBounds { get { return clientBounds; } }
		public RectangleF HalfTickBounds { get { return halfTickBounds; } }
		public RectangleF QuarterTickBounds { get { return quarterTickBounds; } }
		public List<RectangleF> DisplaySpaceAreaCollection { get { return displaySpaceAreaCollection; } }
		public List<RectangleF> DisplayActiveAreaCollection { get { return displayActiveAreaCollection; } }
		public Rectangle DisplayClientBounds { get { return displayClientBounds; } }
		public RectangleF DisplayHalfTickBounds { get { return displayHalfTickBounds; } }
		public RectangleF DisplayQuarterTickBounds { get { return displayQuarterTickBounds; } }
		public virtual int CurrentActiveAreaIndex { get { return 0; } }
		public List<RulerHotZone> HotZones { get { return hotZones; } }
		protected internal DocumentModel DocumentModel { get { return Control.RichEditControl.InnerControl.DocumentModel; } }
		protected internal IOrientation Orientation { get { return Control.Orientation; } }
		protected int MaxRulerDimensionInModelUnits { get { return DocumentModel.UnitConverter.DocumentsToModelUnits(300 * 50); } }
		protected internal bool IsReady { get { return isReady; } set { isReady = value; } }
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
		protected internal virtual Size MeasureString(string value) {
#if !SL && !WPF
			lock (measureGraphics) {
				using (GraphicsToLayoutUnitsModifier modifier = new GraphicsToLayoutUnitsModifier(measureGraphics, control.DocumentModel.LayoutUnitConverter)) {
					SizeF size = measureGraphics.MeasureString(value, control.TickMarkFont, int.MaxValue, StringFormat.GenericTypographic);
					return new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
				}
			}
#else
#if SL
			PrecalculatedMetricsFontInfoMeasurer measurer = new PrecalculatedMetricsFontInfoMeasurer(control.DocumentModel.LayoutUnitConverter);
			return measurer.MeasureString(value, control.TickMarkFont);
#else
			WpfFontInfoMeasurer measurer = new WpfFontInfoMeasurer(control.DocumentModel.LayoutUnitConverter);
			return measurer.MeasureString(value, control.TickMarkFont);
#endif
#endif
		}
		protected internal virtual float CalculateTickStep(UnitConverter converter) {
			DocumentUnit unit = control.RichEditControl.InnerControl.UIUnit;
			float value;
			if (unit == DocumentUnit.Millimeter)
				value = 20; 
			else if (unit == DocumentUnit.Point)
				value = 72; 
			else
				value = 1;
			return ToDocumentLayoutUnitConverter.ToLayoutUnits((int)converter.ToUnits(value)) * Control.ZoomFactor;
		}
		#region ResampleDownRulerTickmarks
		protected internal virtual void ResampleDownRulerTickmarks() {
			ResampleDownTicks();
			if (AreRulerTickmarksOverlapped())
				DeleteRulerTickmarks(RulerTickmarkType.QuarterTick);
			if (AreRulerTickmarksOverlapped())
				DeleteRulerTickmarks(RulerTickmarkType.HalfTick);
		}
		protected internal void ResampleDownTicks() {
			bool isDelete = true;
			for (; ; ) {
				if (!ShouldResampleDownTicks())
					return;
				ResampleDownTicksCore(ref isDelete);
			}
		}
		protected internal void ResampleDownTicksCore(ref bool isDelete) {
			for (int i = 0; i < RulerTickmarks.Count; i++) {
				if (RulerTickmarks[i].RulerTickmarkType == RulerTickmarkType.Tick) {
					if (isDelete) {
						RulerTickmarks.Remove(RulerTickmarks[i]);
						isDelete = false;
					}
					else
						isDelete = true;
				}
			}
		}
		protected virtual bool ShouldResampleDownTicks() {
			float totalTickExtent = 0;
			for (int i = 0; i < RulerTickmarks.Count; i++) {
				if (RulerTickmarks[i].RulerTickmarkType == RulerTickmarkType.Tick)
					totalTickExtent += Orientation.GetPrimaryCoordinateExtent(RulerTickmarks[i].Bounds);
			}
			if (totalTickExtent <= 0)
				return false;
			else {
				const float ratio = 1.65f;
				return Orientation.GetPrimaryCoordinateExtent(ClientBounds) / totalTickExtent < ratio;
			}
		}
		protected virtual bool AreRulerTickmarksOverlapped() {
			for (int i = 1; i < RulerTickmarks.Count; i++)
				if (Orientation.GetFarPrimaryCoordinate(RulerTickmarks[i - 1].Bounds) >= Orientation.GetNearPrimaryCoordinate(RulerTickmarks[i].Bounds))
					return true;
			return false;
		}
		protected internal virtual void DeleteRulerTickmarks(RulerTickmarkType type) {
			for (int i = RulerTickmarks.Count - 1; i >= 0; i--) {
				if (RulerTickmarks[i].RulerTickmarkType == type)
					RulerTickmarks.Remove(RulerTickmarks[i]);
			}
		}
		#endregion
		#region Ruler Ticks
		protected internal virtual void CalculateRulerSeparators() {
			UnitConverter uiUnitConverter = Control.DocumentModel.InternalAPI.UnitConverters[Control.RichEditControl.InnerControl.UIUnit];
			DocumentModelUnitToLayoutUnitConverter toLayoutUnitConverter = Control.DocumentModel.ToDocumentLayoutUnitConverter;
			float step = CalculateTickStep(uiUnitConverter);
			float activeAreaCollectionNear = Orientation.GetNearPrimaryCoordinate(ActiveAreaCollection[CurrentActiveAreaIndex]);
			int clientBoundsNear = Orientation.GetNearPrimaryCoordinate(ClientBounds);
			int clientBoundsFar = Orientation.GetFarPrimaryCoordinate(ClientBounds);
			float distanceToZero = Math.Abs(clientBoundsNear - activeAreaCollectionNear);
			int distanceToZeroInSteps = (int)(distanceToZero / step);
			float offset = distanceToZero - distanceToZeroInSteps * step;
			float startPos = clientBoundsNear + offset;
			AddTicks(step, startPos - step);
			for (float u = startPos; u < clientBoundsFar; u += step) {
				float valueInLayoutUnits = (u - activeAreaCollectionNear) / Control.ZoomFactor;
				float valueInModelUnits = toLayoutUnitConverter.ToModelUnits(valueInLayoutUnits);
				int value = Math.Abs(Convert.ToInt32(uiUnitConverter.FromUnits(valueInModelUnits)));
				AddTickNumber(u, value);
				AddTicks(step, u);
			}
			ResampleDownRulerTickmarks();
		}
		protected internal void AddTicks(float step, float u) {
			AddQuarterTicks(step / 2, u);
			AddHalfTick(u + step / 2.0f);
			AddQuarterTicks(step / 2, u + step / 2);
		}
		protected internal virtual void AddQuarterTicks(float step, float u) {
			if (Control.RichEditControl.InnerControl.UIUnit == DocumentUnit.Centimeter) {
				AddQuarterTick(u + step / 2.0f);
				return;
			}
			AddQuarterTick(u + step / 4.0f);
			AddQuarterTick(u + step / 2.0f);
			AddQuarterTick(u + step * 3 / 4.0f);
		}
		protected internal void AddHalfTick(float u) {
			if (u < Orientation.GetFarPrimaryCoordinate(ClientBounds) && u > Orientation.GetNearPrimaryCoordinate(ClientBounds)) {
				RectangleF bounds = Orientation.SetNearPrimaryCoordinate(HalfTickBounds, u);
				RulerTickmarks.Add(new RulerTickmarkHalf(control, bounds));
			}
		}
		protected internal void AddQuarterTick(float u) {
			if (u < Orientation.GetFarPrimaryCoordinate(ClientBounds) && u > Orientation.GetNearPrimaryCoordinate(ClientBounds)) {
				RectangleF bounds = Orientation.SetNearPrimaryCoordinate(QuarterTickBounds, u);
				RulerTickmarks.Add(new RulerTickmarkQuarter(control, bounds));
			}
		}
		protected internal abstract void AddTickNumber(float u, int value);
		#endregion
		protected internal Rectangle CalculatePageClientBounds(PageBoundsCalculator pageCalculator, SectionProperties properties) {
			return pageCalculator.CalculatePageClientBoundsCore(properties.PageWidth, properties.PageHeight, properties.LeftMargin, properties.TopMargin, properties.RightMargin, properties.BottomMargin);
		}
		protected internal abstract void CalculateSpaceAreaCollection();
		protected internal abstract void CalculateActiveAreaCollection();
		protected internal abstract void CalculateTableHotZone();
		protected internal abstract RectangleF CalculateQuarterTickBounds();
		protected internal abstract RectangleF CalculateHalfTickBounds();
		protected internal abstract Rectangle CalculateClientBounds(IRulerControl2 control);
		protected internal abstract int GetRulerLayoutPosition(int modelPosition);
		protected internal abstract int GetRulerModelPosition(int layoutPosition);
	}
	#endregion
	#region HorizontalRulerViewInfo
	public class HorizontalRulerViewInfo : RulerViewInfoBase {
		#region Fields
		const int minDistanceBetweenIndents = 10;
		readonly IHorizontalRulerControl control;
		List<Rectangle> defaultTabsCollection;
		List<Rectangle> displayDefaultTabsCollection;
		Rectangle defaultTabBounds;
		Rectangle displayDefaultTabBounds;
		TabTypeToggleHotZone tabTypeToggle;
		TabTypeToggleBackgroundHotZone tabTypeToggleBackground;
		int defaultTabWidth;
		Paragraph paragraph;
		int currentActiveAreaIndex;
		TableCellViewInfo tableCellViewInfo;
		SortedList<int> tableAlignmentedPositions;
		#endregion
		public HorizontalRulerViewInfo(IHorizontalRulerControl control, SectionProperties sectionProperties, Paragraph currentParagraph, int currentActiveAreaIndex, bool isMainPieceTable, TableCellViewInfo tableCellViewInfo, SortedList<int> tableAlignmentedPositions)
			: base(control, sectionProperties, isMainPieceTable) {
			this.control = control;
			this.currentActiveAreaIndex = currentActiveAreaIndex;
			this.paragraph = currentParagraph;
			this.defaultTabBounds = CalculateDefaultTabBounds();
			DocumentLayoutUnitConverter unitConverter = control.DocumentModel.LayoutUnitConverter;
			this.displayDefaultTabBounds = unitConverter.LayoutUnitsToPixels(defaultTabBounds, Control.DpiX, Control.DpiY);
			this.defaultTabWidth = GetDefaultTabWidth();
			this.defaultTabsCollection = new List<Rectangle>();
			this.displayDefaultTabsCollection = new List<Rectangle>();
			this.tableCellViewInfo = tableCellViewInfo;
			this.tableAlignmentedPositions = tableAlignmentedPositions;
		}
		protected internal override void Initialize() {
			CalculateClientBounds(control);
#if SL || WPF
			Rectangle rect = ClientBounds;
			DocumentLayoutUnitConverter unitConverter = control.DocumentModel.LayoutUnitConverter;
			IRichEditControl richEditControl = control.RichEditControl;
			rect.X = Math.Max(rect.X, unitConverter.PixelsToLayoutUnits(richEditControl.InnerControl.ActiveView.ActualPadding.Left, richEditControl.InnerControl.DpiX));
			SetClientBounds(rect);
			control.RichEditControl.OnViewPaddingChanged();
			CalculateBounds(unitConverter);
#endif
			base.Initialize();
			CalculateTabsBounds();
			CalculateIndentElementsBounds();
			CalculateSectionSeparator();
			CalculateTabTypeToggle();
#if !SL && !WPF
			CalculateBounds();
#endif
			CalculateTableHotZone();
			SortingHotZones();
			RaisePropertyChanged("ViewInfo");
		}
		#region Properties
		public Paragraph Paragraph { get { return paragraph; } }
		public override int CurrentActiveAreaIndex { get { return currentActiveAreaIndex; } }
		public TabTypeToggleBackgroundHotZone TabTypeToggleBackground { get { return tabTypeToggleBackground; } }
		public TabTypeToggleHotZone TabTypeToggleHotZone { get { return tabTypeToggle; } }
		public new IHorizontalRulerControl Control { get { return control; } }
		public Rectangle DefaultTabBounds { get { return defaultTabBounds; } }
		public Rectangle DisplayDefaultTabBounds { get { return displayDefaultTabBounds; } }
		public List<Rectangle> DefaultTabsCollection { get { return defaultTabsCollection; } }
		public List<Rectangle> DisplayDefaultTabsCollection { get { return displayDefaultTabsCollection; } }
		public TableCellViewInfo TableCellViewInfo { get { return tableCellViewInfo; } }
		#endregion
#if !SL && !WPF
		protected internal void CalculateBounds() {
			Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, GetRulerSize());
			DocumentLayoutUnitConverter unitConverter = control.DocumentModel.LayoutUnitConverter;
			int height = DisplayClientBounds.Height + unitConverter.LayoutUnitsToPixels(control.Painter.PaddingTop + control.Painter.PaddingBottom, control.DpiY);
			Control.Bounds = new Rectangle(Control.Bounds.X, Control.Bounds.Y, Control.Bounds.Width, height);
		}
		protected internal int GetRulerSize() {
			return Control.Painter.CalculateTotalRulerSize(TextSize.Height);
		}
#endif
		protected internal override Rectangle CalculateClientBounds(IRulerControl2 control) {
			this.IsReady = false;
			Rectangle pageClientBounds;
			CaretPosition caretPosition = control.RichEditControl.InnerControl.ActiveView.CaretPosition;
			if (!caretPosition.Update(DocumentLayoutDetailsLevel.Page))
				return new Rectangle(0, 0, 100, 100);
			this.IsReady = true;
			pageClientBounds = caretPosition.PageViewInfo.ClientBounds;
			if (control.RichEditControl.InnerControl.ActiveViewType == RichEditViewType.PrintLayout)
				pageClientBounds.Width = (int)Math.Round(ToDocumentLayoutUnitConverter.ToLayoutUnits(Math.Min(MaxRulerDimensionInModelUnits, SectionProperties.PageWidth)) * control.ZoomFactor);
			else if (control.RichEditControl.InnerControl.ActiveViewType == RichEditViewType.Draft)
				pageClientBounds.Width = (int)Math.Round(ToDocumentLayoutUnitConverter.ToLayoutUnits(Math.Min(MaxRulerDimensionInModelUnits, SectionProperties.PageWidth - SectionProperties.LeftMargin)) * control.ZoomFactor);
			int clientHeight = TextSize.Height + 2 * control.Painter.VerticalTextPaddingBottom;
#if !SL && !WPF
			int y = control.Painter.PaddingTop;
#else
			int y = 0;
#endif
			return new Rectangle(pageClientBounds.X, y, pageClientBounds.Width, clientHeight);
		}
		protected internal override void CalculateActiveAreaCollection() {
			Rectangle bounds = GetActiveAreaBounds();
			if (IsMainPieceTable) {
				List<Rectangle> rulerAreaCollection = GetRulerAreaCollection(SectionProperties, bounds);
				for (int i = 0; i < rulerAreaCollection.Count; i++)
					AddActiveArea(rulerAreaCollection[i]);
			}
			else
				AddActiveArea(bounds);
		}
		protected internal void AddActiveArea(Rectangle bounds) {
			DocumentLayoutUnitConverter layoutUnitConverter = control.DocumentModel.LayoutUnitConverter;
			Rectangle rulerArea = ApplyZoomFactor(bounds);
			rulerArea.Offset(ClientBounds.X, 0);
			ActiveAreaCollection.Add(rulerArea);
			DisplayActiveAreaCollection.Add(layoutUnitConverter.LayoutUnitsToPixels(rulerArea, Control.DpiX, Control.DpiY));
		}
		protected internal Rectangle GetActiveAreaBounds() {
			IRichEditControl control = Control.RichEditControl;
			DocumentModel documentModel = Control.DocumentModel;
			PageBoundsCalculator pageCalculator = new PageBoundsCalculator(documentModel.ToDocumentLayoutUnitConverter);
			Rectangle printLayoutViewPageClientBounds = Rectangle.Empty;
			if (control.InnerControl.ActiveViewType == RichEditViewType.Simple)
				printLayoutViewPageClientBounds = control.InnerControl.ActiveView.PageViewInfos.First.ClientBounds;
			else
				printLayoutViewPageClientBounds = CalculatePageClientBounds(pageCalculator, SectionProperties);
			PageBoundsCalculator activeViewPageCalculator = control.InnerControl.Formatter.DocumentFormatter.Controller.PageController.PageBoundsCalculator;
			Rectangle activeViewPageClientBounds = CalculatePageClientBounds(activeViewPageCalculator, SectionProperties);
			int deltaX = activeViewPageClientBounds.Left - printLayoutViewPageClientBounds.Left;
			return new Rectangle(printLayoutViewPageClientBounds.X + deltaX, ClientBounds.Y, printLayoutViewPageClientBounds.Width, ClientBounds.Height);
		}
		protected List<Rectangle> GetRulerAreaCollection(SectionProperties sectionProperties, Rectangle bounds) {
			ColumnsBoundsCalculator calculator = new ColumnsBoundsCalculator(control.DocumentModel.ToDocumentLayoutUnitConverter);
			List<Rectangle> result = new List<Rectangle>();
			if (sectionProperties.EqualWidthColumns)
				calculator.PopulateEqualWidthColumnsBounds(result, bounds, sectionProperties.ColumnCount, calculator.UnitConverter.ToLayoutUnits(sectionProperties.Space));
			else
				calculator.PopulateColumnsBounds(result, bounds, sectionProperties.ColumnInfoCollection);
			return result;
		}
		protected internal override void CalculateSpaceAreaCollection() {
			if (IsMainPieceTable) {
				DocumentLayoutUnitConverter layoutUnitConverter = Control.DocumentModel.LayoutUnitConverter;
				for (int i = 1; i < ActiveAreaCollection.Count; i++) {
					RectangleF bounds = new RectangleF(ActiveAreaCollection[i - 1].Right, ActiveAreaCollection[i].Y, ActiveAreaCollection[i].X - ActiveAreaCollection[i - 1].Right, ActiveAreaCollection[i].Height);
					SpaceAreaCollection.Add(bounds);
					DisplaySpaceAreaCollection.Add(layoutUnitConverter.LayoutUnitsToPixels(Rectangle.Round(bounds), Control.DpiX, Control.DpiY));
				}
			}
		}
		protected internal override void CalculateTableHotZone() {
			if (!DocumentModel.DocumentCapabilities.TablesAllowed)
				return;
			if (TableCellViewInfo == null)
				return;
			AddTableLeftBorderHotZone();
			TableRow row = TableCellViewInfo.Cell.Row;
			TableCellCollection cells = row.Cells;
			TableViewInfo tableViewInfo = TableCellViewInfo.TableViewInfo;
			for (int i = 0; i < cells.Count; i++) {
				TableCell cell = cells[i];
				int startColumnIndex = TableCellVerticalBorderCalculator.GetStartColumnIndex(cell, true);
				int borderLayoutHorizontalPositionIndex = startColumnIndex + cell.LayoutProperties.ColumnSpan;
				Rectangle bounds = new Rectangle(GetTableHotZoneLeft(borderLayoutHorizontalPositionIndex), (int)ActiveAreaCollection[CurrentActiveAreaIndex].Y, 0, (int)ActiveAreaCollection[CurrentActiveAreaIndex].Height);
				TableHotZone tableHotZone = new TableHotZone(Control, bounds, borderLayoutHorizontalPositionIndex, tableViewInfo, cell);
				HotZones.Add(tableHotZone);
			}
		}
		protected internal virtual void AddTableLeftBorderHotZone() {
			TableViewInfo tableViewInfo = TableCellViewInfo.TableViewInfo;
			Rectangle bounds = new Rectangle(GetTableHotZoneLeft(0), (int)ActiveAreaCollection[CurrentActiveAreaIndex].Y, 0, (int)ActiveAreaCollection[CurrentActiveAreaIndex].Height);
			TableLeftBorderHotZone tableLeftBorderHotZone = new TableLeftBorderHotZone(Control, bounds, 0, tableViewInfo, TableCellViewInfo.Cell.Row.Cells[0]);
			HotZones.Add(tableLeftBorderHotZone);
		}
		protected internal virtual int GetTableHotZoneLeft(int tableAlignmentedPositionsIndex) {
			return (int)((tableAlignmentedPositions[tableAlignmentedPositionsIndex] + GetAdditionalParentCellIndent(false)) * Control.ZoomFactor + ActiveAreaCollection[CurrentActiveAreaIndex].X);
		}
		protected internal Rectangle ApplyZoomFactor(Rectangle area) {
			area.Width = (int)Math.Ceiling(area.Width * Control.ZoomFactor);
			area.X = (int)Math.Ceiling(area.X * Control.ZoomFactor);
			return area;
		}
		protected internal override void AddTickNumber(float x, int value) {
			string number = value.ToString();
			SizeF size = MeasureString(number);
			float y = ClientBounds.Top + Control.Painter.VerticalTextPaddingTop;
			RectangleF bounds = new RectangleF(x - size.Width / 2.0f, y, size.Width, size.Height);
			RulerTickmarks.Add(new RulerTickmarkNumber(control, bounds, number));
		}
		protected internal virtual void CalculateTabsBounds() {
			TabsController tabsController = new TabsController();
			tabsController.BeginParagraph(Paragraph);
			CalculateTabsBoundsCore(tabsController);
		}
		protected internal virtual void CalculateTabsBoundsCore(TabsController tabsController) {
			int additionalCellIndent = GetAdditionalCellIndent(true);
			int layoutPosition = ClientBounds.X - (int)((ActiveAreaCollection[CurrentActiveAreaIndex].X / Control.ZoomFactor));
			int modelPosition = Control.DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(layoutPosition);
			TabInfo tabInfo = tabsController.GetNextTab(modelPosition);
			int pos = GetRulerLayoutPosition(tabInfo.Position) + additionalCellIndent;
			while (pos > ClientBounds.X && pos < ClientBounds.Right) {
				if (tabInfo.IsDefault)
					AddDefaultTab(pos);
				else
					AddTabHotZone(tabInfo, pos);
				tabInfo = tabsController.GetNextTab(tabInfo.Position);
				pos = GetRulerLayoutPosition(tabInfo.Position) + additionalCellIndent;
			}
		}
		public int GetAdditionalCellIndent(TableCellViewInfo cellViewInfo, bool applyZoomFactor) {
			int result = 0;
			while (cellViewInfo != null) {
				result += cellViewInfo.TextLeft;
				cellViewInfo = cellViewInfo.TableViewInfo.ParentTableCellViewInfo;
			}
			if (applyZoomFactor)
				return (int)(result * Control.ZoomFactor);
			else
				return result;
		}
		public int GetCellTextRight(TableCellViewInfo cellViewInfo, bool applyZoomFactor) {
			int result = GetAdditionalCellIndent(cellViewInfo, false) + cellViewInfo.TextWidth;
			if (applyZoomFactor)
				return (int)(result * Control.ZoomFactor);
			else
				return result;
		}
		public int GetAdditionalCellIndent(bool applyZoomFactor) {
			return GetAdditionalCellIndent(TableCellViewInfo, applyZoomFactor);
		}
		public float GetRightIndentBasePosition() {
			if (TableCellViewInfo == null)
				return ActiveAreaCollection[CurrentActiveAreaIndex].Right;
			return ActiveAreaCollection[CurrentActiveAreaIndex].X + GetCellTextRight(TableCellViewInfo, true);			
		}
		public int GetAdditionalParentCellIndent(bool applyZoomFactor) {
			if (TableCellViewInfo == null)
				return 0;
			return GetAdditionalCellIndent(TableCellViewInfo.TableViewInfo.ParentTableCellViewInfo, applyZoomFactor);
		}
		protected internal void AddTabHotZone(TabInfo tabInfo, int pos) {
			defaultTabsCollection.Clear();
			displayDefaultTabsCollection.Clear();
			if (DocumentModel.DocumentCapabilities.ParagraphTabsAllowed && Control.RichEditControl.InnerControl.Options.HorizontalRuler.ShowTabs)
				AddTabHotZoneCore(tabInfo, pos);
		}
		protected internal TabHotZone AddTabHotZoneCore(TabInfo tabInfo, int pos) {
			TabHotZone tab = CreateTabHotZone(tabInfo, GetTabBounds(pos));
			HotZones.Add(tab);
			return tab;
		}
		protected internal TabHotZone CreateTabHotZone(TabInfo tabInfo, Rectangle bounds) {
			switch (tabInfo.Alignment) {
				default:
				case TabAlignmentType.Left:
					return new LeftTabHotZone(Control, bounds, tabInfo);
				case TabAlignmentType.Center:
					return new CenterTabHotZone(Control, bounds, tabInfo);
				case TabAlignmentType.Right:
					return new RightTabHotZone(Control, bounds, tabInfo);
				case TabAlignmentType.Decimal:
					return new DecimalTabHotZone(Control, bounds, tabInfo);
			}
		}
		protected internal Rectangle GetTabBounds(int position) {
			return new Rectangle(position, ClientBounds.Top + ClientBounds.Height / 2, 10, ClientBounds.Height / 2);
		}
		protected internal void AddDefaultTab(int pos) {
			RectangleF areaBounds = ActiveAreaCollection[CurrentActiveAreaIndex];
			if (pos > areaBounds.Left && pos < areaBounds.Right) {
				Rectangle bounds = defaultTabBounds;
				bounds.X = pos;
				defaultTabsCollection.Add(bounds);
				displayDefaultTabsCollection.Add(Control.DocumentModel.LayoutUnitConverter.LayoutUnitsToPixels(bounds, Control.DpiX, Control.DpiY));
			}
		}
		protected internal override int GetRulerLayoutPosition(int modelPosition) {
			int layoutPosition = Control.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(modelPosition);
			return (int)(layoutPosition * Control.ZoomFactor + ActiveAreaCollection[CurrentActiveAreaIndex].X);
		}
		protected internal override int GetRulerModelPosition(int layoutPosition) {
			int k = (int)((layoutPosition - ActiveAreaCollection[CurrentActiveAreaIndex].X) / Control.ZoomFactor);
			return Control.DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(k);
		}
		protected internal int GetRulerModelPositionRelativeToTableCellViewInfo(int layoutPosition) {
			int additionalIndent = GetAdditionalCellIndent(false);
			int k = (int)((layoutPosition - ActiveAreaCollection[CurrentActiveAreaIndex].X) / Control.ZoomFactor) - additionalIndent;
			return Control.DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(k);
		}
		protected internal void CalculateIndentElementsBounds() {
			if (!DocumentModel.DocumentCapabilities.ParagraphFormattingAllowed)
				return;
			int additionalIndent = GetAdditionalCellIndent(true);
			int firstIndentPosition = GetRulerLayoutPosition(GetIndentInModelUnits(Paragraph)) + additionalIndent;
			int leftIndentPosition = GetRulerLayoutPosition(Paragraph.LeftIndent) + additionalIndent;
			int rightIndentPosition = GetRightIndentLayoutPosition(Paragraph.RightIndent);
			int tabWidth = Control.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(defaultTabWidth);
			if (rightIndentPosition < leftIndentPosition)
				rightIndentPosition = leftIndentPosition + Control.DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(minDistanceBetweenIndents);
			int rightBorder = rightIndentPosition - tabWidth;
			int leftBorder = Math.Max(leftIndentPosition, firstIndentPosition) + tabWidth;
			if (leftBorder > rightIndentPosition)
				leftBorder = rightIndentPosition;
			if (Control.RichEditControl.InnerControl.Options.HorizontalRuler.ShowLeftIndent)
				AddLeftIndentHotZone(Paragraph, leftIndentPosition, rightBorder, firstIndentPosition);
			if (Control.RichEditControl.InnerControl.Options.HorizontalRuler.ShowRightIndent)
				AddRightIndentHotZone(Paragraph, rightIndentPosition, leftBorder);
		}
		protected internal int GetRightIndentLayoutPosition(int modelPosition) {
			float rightPosition = GetRightIndentBasePosition();
			int layoutPosition = Control.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(modelPosition);
			return (int)(rightPosition - layoutPosition * Control.ZoomFactor);
		}
		protected internal int GetRightIndentModelPosition(int layoutPosition) {
			float rightPosition = GetRightIndentBasePosition();
			int layoutRightIndent = (int)((rightPosition - layoutPosition) / Control.ZoomFactor);
			return Control.DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(layoutRightIndent);
		}
		protected internal int GetIndentInModelUnits(Paragraph paragraph) {
			int indent;
			switch (paragraph.FirstLineIndentType) {
				case ParagraphFirstLineIndent.Indented:
					indent = paragraph.FirstLineIndent;
					break;
				case ParagraphFirstLineIndent.Hanging:
					indent = -paragraph.FirstLineIndent;
					break;
				default:
					indent = 0;
					break;
			}
			return indent + paragraph.LeftIndent;
		}
		protected internal void AddLeftIndentHotZone(Paragraph paragraph, int leftIndent, int rightBorder, int firstIndent) {
			int height = ClientBounds.Height / 2;
			FirstLineIndentHotZone firstLineHotZone = AddFirstLineHotZone(paragraph, rightBorder, firstIndent, height);
			Rectangle bounds = new Rectangle(leftIndent, ClientBounds.Top + height, 0, height);
			LeftBottomHotZone bottomHotZone = new LeftBottomHotZone(Control, bounds, paragraph.LeftIndent, rightBorder, firstLineHotZone);
			bottomHotZone.AddLeftHotZone(bounds);
			HotZones.Add(bottomHotZone);
		}
		private FirstLineIndentHotZone AddFirstLineHotZone(Paragraph paragraph, int rightBorder, int firstIndent, int height) {
			Rectangle bounds = new Rectangle(firstIndent, ClientBounds.Top, 0, height);
			FirstLineIndentHotZone firstLineHotZone = new FirstLineIndentHotZone(Control, bounds, GetIndentInModelUnits(paragraph), paragraph.LeftIndent);
			firstLineHotZone.RightBorder = rightBorder;
			HotZones.Add(firstLineHotZone);
			return firstLineHotZone;
		}
		protected internal void AddRightIndentHotZone(Paragraph paragraph, int rightIndent, int leftBorder) {
			int height = ClientBounds.Height / 2;
			Rectangle bounds = new Rectangle(rightIndent, ClientBounds.Top + height, 0, height);
			RightIndentHotZone hotZone = new RightIndentHotZone(Control, bounds, paragraph.RightIndent);
			hotZone.LeftBorder = leftBorder;
			HotZones.Add(hotZone);
		}
		protected internal void CalculateSectionSeparator() {
			if (!IsMainPieceTable)
				return;
			if (!DocumentModel.DocumentCapabilities.SectionsAllowed)
				return;
			bool isEqualWidthColumns = SectionProperties.EqualWidthColumns;
			for (int i = 0; i < ActiveAreaCollection.Count; i++) {
				if (Control.RichEditControl.InnerControl.ActiveViewType != RichEditViewType.Draft) {
					AddRightColumnResizer(i);
					AddLeftColumnResizer(i);
				}
				else {
					if (i != ActiveAreaCollection.Count - 1)
						AddRightColumnResizer(i);
					if (i != 0)
						AddLeftColumnResizer(i);
				}
				if (!isEqualWidthColumns && i < ActiveAreaCollection.Count - 1)
					AddMiddleColumnResizer(i);
			}
		}
		protected internal void AddMiddleColumnResizer(int index) {
			int x = (int)((ActiveAreaCollection[index].Right + (ActiveAreaCollection[index + 1].Left - ActiveAreaCollection[index].Right) / 2));
			MiddleColumnResizerHotZone hotZone = new MiddleColumnResizerHotZone(Control, new Rectangle(x, ClientBounds.Y, 0, 0), index);
			HotZones.Add(hotZone);
		}
		protected internal void AddLeftColumnResizer(int index) {
			Rectangle bounds = new Rectangle((int)ActiveAreaCollection[index].Left, ClientBounds.Y, 0, 0);
			LeftColumnResizerHotZone hotZone = new LeftColumnResizerHotZone(Control, bounds, index);
			if (index == 0)
				hotZone.Visible = false;
			HotZones.Add(hotZone);
		}
		protected internal void AddRightColumnResizer(int index) {
			Rectangle bounds = new Rectangle((int)ActiveAreaCollection[index].Right, ClientBounds.Y, 0, 0);
			RightColumnResizerHotZone hotZone = new RightColumnResizerHotZone(Control, bounds, index);
			if (index == ActiveAreaCollection.Count - 1)
				hotZone.Visible = false;
			HotZones.Add(hotZone);
		}
		protected internal int GetDefaultTabWidth() {
			return (int)(Control.DocumentModel.DocumentProperties.DefaultTabWidth * Control.ZoomFactor);
		}
		protected internal void CalculateTabTypeToggle() {
			Rectangle bounds = control.CalculateTabTypeToggleBackgroundBounds();
			tabTypeToggleBackground = new TabTypeToggleBackgroundHotZone(control, bounds);
			Size size = control.GetTabTypeToggleActiveAreaSize();
			bounds = new Rectangle(bounds.X + (bounds.Width - size.Width) / 2, bounds.Y + (bounds.Height - size.Height) / 2, size.Width, size.Height);
			tabTypeToggle = new TabTypeToggleHotZone(control, bounds);
		}
		protected internal override RectangleF CalculateHalfTickBounds() {
			SizeF size = TextSize;
			size.Height /= 2.0f;
			const int halfTickWidth = 1;
			float y = (ClientBounds.Top + base.Control.Painter.VerticalTextPaddingTop + size.Height / 2.0f);
			return new RectangleF(0, y, halfTickWidth, size.Height);
		}
		protected internal override RectangleF CalculateQuarterTickBounds() {
			const int quarterTickWidth = 1;
			return new RectangleF(0, HalfTickBounds.Y + HalfTickBounds.Height / 4.0f, quarterTickWidth, HalfTickBounds.Height / 2.0f);
		}
		protected internal virtual Rectangle CalculateDefaultTabBounds() {
			const int tabWidth = 1;
			int paddingBottom = Control.Painter.PaddingBottom;
			float height = paddingBottom / 2.0f;
			float Y = ClientBounds.Bottom + (paddingBottom - height) / 2.0f;
			return new Rectangle(0, (int)Y, tabWidth, (int)height);
		}
		protected internal virtual void SortingHotZones() {
			HotZones.Sort(new RulerHotZonesComparer());
		}
	}
	#endregion
	#region RulerHotZonesComparer
	public class RulerHotZonesComparer : IComparer<RulerHotZone> {
		#region IComparer<RulerHotZone> Members
		public int Compare(RulerHotZone hotZone, RulerHotZone nextHotZone) {
			if (hotZone.Weight > nextHotZone.Weight)
				return -1;
			if (hotZone.Weight < nextHotZone.Weight)
				return 1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
	#region VerticalRulerViewInfo
	public class VerticalRulerViewInfo : RulerViewInfoBase {
		#region Field
		readonly IVerticalRulerControl control;
		SortedList<int> tableVerticalPositions;
		TableCellViewInfo tableCellViewInfo;
		#endregion
		public VerticalRulerViewInfo(IVerticalRulerControl control, SectionProperties sectionProperties, bool isMainPieceTable, SortedList<int> tableVerticalPositions, TableCellViewInfo tableCellViewInfo)
			: base(control, sectionProperties, isMainPieceTable) {
			this.control = control;
			this.tableVerticalPositions = tableVerticalPositions;
			this.tableCellViewInfo = tableCellViewInfo;
		}
		protected internal override void Initialize() {
			CalculateClientBounds(control);
			base.Initialize();
#if !SL && !WPF
			CalculateBounds();
#endif
			CalculateSectionSeparator();
			CalculateTableHotZone();
		}
		#region Properties
		public new IVerticalRulerControl Control { get { return control; } }
		protected internal SortedList<int> TableVerticalPositions { get { return tableVerticalPositions; } }
		protected internal TableCellViewInfo TableCellViewInfo { get { return tableCellViewInfo; } }
		#endregion
		protected internal override Rectangle CalculateClientBounds(IRulerControl2 control) {
			this.IsReady = false;
			Rectangle pageClientBounds;
			CaretPosition caretPosition = control.RichEditControl.InnerControl.ActiveView.CaretPosition;
			if (!caretPosition.Update(DocumentLayoutDetailsLevel.Page))
				return new Rectangle(0, 0, 100, 100);
			this.IsReady = true;
			pageClientBounds = caretPosition.PageViewInfo.ClientBounds;
			pageClientBounds.Y = 0;
			if (control.RichEditControl.InnerControl.ActiveViewType == RichEditViewType.PrintLayout)
				pageClientBounds.Height = (int)Math.Round(ToDocumentLayoutUnitConverter.ToLayoutUnits(Math.Min(MaxRulerDimensionInModelUnits, SectionProperties.PageHeight)) * control.ZoomFactor);
			else
				pageClientBounds.Height = (int)Math.Round(ToDocumentLayoutUnitConverter.ToLayoutUnits(Math.Min(MaxRulerDimensionInModelUnits, SectionProperties.PageHeight - SectionProperties.TopMargin)) * control.ZoomFactor);
			int clientWidth = TextSize.Height + 2 * control.Painter.VerticalTextPaddingBottom;
#if !SL && !WPF
			int x = control.Painter.PaddingTop;
#else
			int x = 0;
#endif
			return new Rectangle(x, pageClientBounds.Y, clientWidth, pageClientBounds.Height);
		}
#if !SL && !WPF
		protected internal void CalculateBounds() {
			Bounds = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, GetRulerSize());
			DocumentLayoutUnitConverter unitConverter = control.DocumentModel.LayoutUnitConverter;
			int width = DisplayClientBounds.Width + unitConverter.LayoutUnitsToPixels(control.Painter.PaddingTop + control.Painter.PaddingBottom, Control.DpiX);
			Control.Bounds = new Rectangle(Control.Bounds.X, Control.Bounds.Y, width, Control.Bounds.Height);
		}
		protected internal int GetRulerSize() {
			return Control.Painter.CalculateTotalRulerSize(TextSize.Height);
		}
#endif
		protected internal override RectangleF CalculateHalfTickBounds() {
			SizeF size = TextSize;
			size.Height /= 2.0f;
			const int halfTickHeight = 1;
			float x = ClientBounds.Left + base.Control.Painter.VerticalTextPaddingBottom + size.Height / 2.0f;
			return new RectangleF(x, 0, size.Height, halfTickHeight);
		}
		protected internal override RectangleF CalculateQuarterTickBounds() {
			const int quarterTickHeight = 1;
			return new RectangleF(HalfTickBounds.X + HalfTickBounds.Width / 4.0f, 0, HalfTickBounds.Width / 2.0f, quarterTickHeight);
		}
		protected internal override void CalculateActiveAreaCollection() {
			Rectangle activeBounds = GetActiveBounds();
			activeBounds = new Rectangle(ClientBounds.X, activeBounds.Y, ClientBounds.Width, activeBounds.Height);
			activeBounds = ApplyZoomFactor(activeBounds);
			activeBounds.Offset(0, ClientBounds.Y);
			ActiveAreaCollection.Add(activeBounds);
			DocumentLayoutUnitConverter layoutUnitConverter = control.DocumentModel.LayoutUnitConverter;
			DisplayActiveAreaCollection.Add(layoutUnitConverter.LayoutUnitsToPixels(Rectangle.Round(activeBounds), Control.DpiX, Control.DpiY));
		}
		protected internal Rectangle GetActiveBounds() {
			if (!IsMainPieceTable && Control.RichEditControl.InnerControl.ActiveView.CaretPosition.Update(DocumentLayoutDetailsLevel.PageArea)) {
				RichEditLayoutPage page = Control.RichEditControl.InnerControl.ActiveView.CaretPosition.LayoutPosition.Page;
				if (page.Header != null && Object.ReferenceEquals(page.Header.PieceTable, Control.DocumentModel.Selection.PieceTable))
					return page.Header.Bounds;
				else if (page.Footer != null && Object.ReferenceEquals(page.Footer.PieceTable, Control.DocumentModel.Selection.PieceTable))
					return page.Footer.Bounds;
			}
			return GetPageBounds();
		}
		protected internal Rectangle GetPageBounds() {
			IRichEditControl control = Control.RichEditControl;
			PageBoundsCalculator pageCalculator = new PageBoundsCalculator(Control.DocumentModel.ToDocumentLayoutUnitConverter);
			Rectangle printLayoutViewPageClientBounds = CalculatePageClientBounds(pageCalculator, SectionProperties);
			PageBoundsCalculator activeViewPageCalculator = control.InnerControl.Formatter.DocumentFormatter.Controller.PageController.PageBoundsCalculator;
			Rectangle activeViewPageClientBounds = CalculatePageClientBounds(activeViewPageCalculator, SectionProperties);
			int deltaY = activeViewPageClientBounds.Top - printLayoutViewPageClientBounds.Top;
			int deltaHeight = activeViewPageClientBounds.Height - printLayoutViewPageClientBounds.Height;
			printLayoutViewPageClientBounds.Y += deltaY;
			printLayoutViewPageClientBounds.Height += deltaHeight;
			return printLayoutViewPageClientBounds;
		}
		protected internal override void CalculateSpaceAreaCollection() {
		}
		protected internal override void CalculateTableHotZone() {
			if (!DocumentModel.DocumentCapabilities.TablesAllowed)
				return;
			for (int i = 0; i < tableVerticalPositions.Count; i++) {
				Rectangle bounds = new Rectangle((int)ActiveAreaCollection[0].X, (int)(tableVerticalPositions[i] * Control.ZoomFactor), (int)ActiveAreaCollection[0].Width, (int)(ActiveAreaCollection[0].Width * Control.ZoomFactor));
				VerticalTableHotZone hotZone = new VerticalTableHotZone(Control, bounds, tableCellViewInfo.TableViewInfo, i);
				HotZones.Add(hotZone);
			}
		}
		protected internal Rectangle ApplyZoomFactor(Rectangle area) {
			area.Height = (int)Math.Ceiling(area.Height * Control.ZoomFactor);
			area.Y = (int)Math.Ceiling(area.Y * Control.ZoomFactor);
			return area;
		}
		protected internal override void AddTickNumber(float y, int value) {
			string number = value.ToString();
			SizeF size = MeasureString(number);
			float x = ClientBounds.Left + base.Control.Painter.VerticalTextPaddingBottom;
			RectangleF bounds = new RectangleF(x, y - size.Height / 2.0f, size.Height, size.Width);
			RulerTickmarks.Add(new RulerTickmarkNumber(control, bounds, number));
		}
		protected internal void CalculateSectionSeparator() {
			if (!IsMainPieceTable)
				return;
			if (!DocumentModel.DocumentCapabilities.SectionsAllowed)
				return;
			for (int i = 0; i < ActiveAreaCollection.Count; i++) {
				AddTopSectionResizer(i);
				AddBottomSectionResizer(i);
			}
		}
		protected internal void AddBottomSectionResizer(int index) {
			Rectangle bounds = new Rectangle(ClientBounds.X, (int)ActiveAreaCollection[index].Bottom, ClientBounds.Width, ClientBounds.Width);
			SectionBottomResizerHotZone hotZone = new SectionBottomResizerHotZone(Control, bounds, index);
			HotZones.Add(hotZone);
		}
		protected internal void AddTopSectionResizer(int index) {
			Rectangle bounds = new Rectangle(ClientBounds.X, (int)ActiveAreaCollection[index].Top, ClientBounds.Width, ClientBounds.Width);
			SectionTopResizerHotZone hotZone = new SectionTopResizerHotZone(Control, bounds, index);
			HotZones.Add(hotZone);
		}
		protected internal override int GetRulerLayoutPosition(int modelPosition) {
			int layoutPosition = Control.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(modelPosition);
			return (int)(layoutPosition * Control.ZoomFactor + ActiveAreaCollection[CurrentActiveAreaIndex].Y);
		}
		protected internal override int GetRulerModelPosition(int layoutPosition) {
			int k = (int)((layoutPosition - ActiveAreaCollection[CurrentActiveAreaIndex].Y) / Control.ZoomFactor);
			return Control.DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(k);
		}
	}
	#endregion
}
