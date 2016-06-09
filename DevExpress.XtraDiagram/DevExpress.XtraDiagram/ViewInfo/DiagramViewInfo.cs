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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.Diagram.Core.Themes;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraDiagram.Adorners;
using DevExpress.XtraDiagram.Animations;
using DevExpress.XtraDiagram.Base;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.InplaceEditing;
using DevExpress.XtraDiagram.Layers;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.Parser;
using DevExpress.XtraDiagram.Themes;
using DevExpress.XtraDiagram.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraDiagram.ViewInfo {
	public class DiagramControlViewInfo : BaseControlViewInfo {
		DiagramItems items;
		bool isHScrollVisible, isVScrollVisible;
		Rectangle pageContentRect;
		Rectangle pageDisplayRect;
		Rectangle gridDrawArea;
		Point gridContentLocation;
		Rectangle pageContentDisplayRect;
		Rectangle hRulerBounds, vRulerBounds;
		Pen connectorOutlinePen;
		Image rotationImage;
		DiagramShapePainter shapePainter;
		DiagramShapeParser shapeParser;
		DiagramConnectorParser connectorParser;
		DiagramShapeObjectInfoArgs shapeDrawArgs;
		DiagramItemParseStrategy itemParseStrategy;
		DiagramContainerPainter containerItemPainter;
		DiagramContainerObjectInfoArgs containerItemDrawArgs;
		DiagramConnectorPainter connectorPainter;
		DiagramConnectorObjectInfoArgs connectorDrawArgs;
		DiagramHRulerPainter hRulerPainter;
		DiagramVRulerPainter vRulerPainter;
		DiagramHRulerBackgroundPainter hRulerBackgroundPainter;
		DiagramVRulerBackgroundPainter vRulerBackgroundPainter;
		DiagramHRulerObjectInfoArgs hRulerDrawArgs;
		DiagramVRulerObjectInfoArgs vRulerDrawArgs;
		DiagramGridPainter gridPainter;
		DiagramGridObjectInfoArgs gridDrawArgs;
		DiagramBackgroundPainter backgroundPainter;
		DiagramBackgroundObjectInfoArgs backgroundDrawArgs;
		DiagramPagePainter pagePainter;
		DiagramPageObjectInfoArgs pageDrawArgs;
		DiagramItemViewControllers itemViewControllers;
		DiagramShapeDragPreviewPainter dragPreviewShapePainter;
		DiagramConnectorDragPreviewPainter dragPreviewConnectorPainter;
		DiagramConnectorPointDragPreviewPainter connectorPointDragPreviewPainter;
		DiagramConnectorSelectionPartPainter connectorSelectionPartPainter;
		DiagramAppearanceObject shapePaintAppearance;
		DiagramAppearanceObject connectorPaintAppearance;
		DiagramAppearanceObject hRulerPaintAppearance;
		DiagramAppearanceObject vRulerPaintAppearance;
		DiagramPaintCache paintCache;
		DiagramDefaultAppearances defaultAppearances;
		public DiagramControlViewInfo(DiagramControl owner) : base(owner) {
			this.isHScrollVisible = this.isVScrollVisible = false;
			this.gridDrawArea = Rectangle.Empty;
			this.gridContentLocation = Point.Empty;
			this.pageContentDisplayRect = Rectangle.Empty;
			this.pageDisplayRect = this.pageContentRect = Rectangle.Empty;
			this.hRulerBounds = this.vRulerBounds = Rectangle.Empty;
			this.rotationImage = null;
			this.items = new DiagramItems();
			this.shapeDrawArgs = CreateShapeDrawArgs();
			this.itemParseStrategy = CreateItemParseStrategy();
			this.shapePainter = CreateShapePainter();
			this.shapeParser = CreateShapeParser();
			this.connectorParser = CreateConnectorParser();
			this.connectorOutlinePen = CreateConnectorOutlinePen();
			this.containerItemPainter = CreateContainerItemPainter();
			this.containerItemDrawArgs = CreateContainerItemDrawArgs();
			this.connectorPainter = CreateConnectorPainter();
			this.connectorDrawArgs = CreateConnectorDrawArgs();
			this.hRulerPainter = CreateHRulerPainter();
			this.vRulerPainter = CreateVHRulerPainter();
			this.hRulerBackgroundPainter = CreateHRulerBackgroundPainter();
			this.vRulerBackgroundPainter = CreateVRulerBackgroundPainter();
			this.hRulerDrawArgs = CreateHRulerDrawArgs();
			this.vRulerDrawArgs = CreateVRulerDrawArgs();
			this.gridPainter = CreateGridPainter();
			this.gridDrawArgs = CreateGridDrawArgs();
			this.backgroundPainter = CreateBackgroundPainter();
			this.backgroundDrawArgs = CreateBackgroundDrawArgs();
			this.pagePainter = CreatePagePainter();
			this.pageDrawArgs = CreatePageDrawArgs();
			this.itemViewControllers = CreateItemViewControllers();
			this.dragPreviewShapePainter = CreateDragPreviewShapePainter();
			this.dragPreviewConnectorPainter = CreateDragPreviewConnectorPainter();
			this.connectorPointDragPreviewPainter = CreateConnectorPointDragPreviewPainter();
			this.connectorSelectionPartPainter = CreateConnectorSelectionPartPainter();
			this.shapePaintAppearance = CreateDiagramPaintAppearance();
			this.connectorPaintAppearance = CreateDiagramPaintAppearance();
			this.hRulerPaintAppearance = CreateDiagramPaintAppearance();
			this.vRulerPaintAppearance = CreateDiagramPaintAppearance();
			this.paintCache = new DiagramPaintCache();
			this.defaultAppearances = CreateDefaultAppearances();
		}
		protected virtual DiagramAppearanceObject CreateDiagramPaintAppearance() {
			return new DiagramAppearanceObject();
		}
		protected virtual DiagramShapePainter CreateShapePainter() {
			return new DefaultDiagramShapePainter();
		}
		protected virtual DiagramShapeDragPreviewPainter CreateDragPreviewShapePainter() {
			return new DiagramShapeDragPreviewPainter();
		}
		protected virtual DiagramConnectorDragPreviewPainter CreateDragPreviewConnectorPainter() {
			return new DiagramConnectorDragPreviewPainter();
		}
		protected virtual DiagramConnectorPointDragPreviewPainter CreateConnectorPointDragPreviewPainter() {
			return new DiagramConnectorPointDragPreviewPainter();
		}
		protected virtual DiagramShapeParser CreateShapeParser() {
			return new DiagramShapeParser(ItemParseStrategy);
		}
		protected virtual DiagramConnectorParser CreateConnectorParser() {
			return new DiagramConnectorParser(ItemParseStrategy);
		}
		protected virtual DiagramItemParseStrategy CreateItemParseStrategy() {
			return new DiagramItemParseStrategy();
		}
		protected virtual DiagramShapeObjectInfoArgs CreateShapeDrawArgs() {
			return new DiagramShapeObjectInfoArgs();
		}
		protected virtual DiagramContainerObjectInfoArgs CreateContainerItemDrawArgs() {
			return new DiagramContainerObjectInfoArgs();
		}
		protected virtual DiagramContainerPainter CreateContainerItemPainter() {
			return new DiagramContainerPainter();
		}
		protected virtual DiagramConnectorPainter CreateConnectorPainter() {
			return new DiagramConnectorPainter();
		}
		protected virtual DiagramConnectorObjectInfoArgs CreateConnectorDrawArgs() {
			return new DiagramConnectorObjectInfoArgs();
		}
		protected virtual DiagramHRulerPainter CreateHRulerPainter() {
			return new DiagramHRulerPainter();
		}
		protected virtual DiagramVRulerPainter CreateVHRulerPainter() {
			return new DiagramVRulerPainter();
		}
		protected virtual DiagramHRulerBackgroundPainter CreateHRulerBackgroundPainter() {
			return new DiagramHRulerBackgroundPainter();
		}
		protected virtual DiagramVRulerBackgroundPainter CreateVRulerBackgroundPainter() {
			return new DiagramVRulerBackgroundPainter();
		}
		protected virtual DiagramHRulerObjectInfoArgs CreateHRulerDrawArgs() {
			return new DiagramHRulerObjectInfoArgs();
		}
		protected virtual DiagramVRulerObjectInfoArgs CreateVRulerDrawArgs() {
			return new DiagramVRulerObjectInfoArgs();
		}
		protected virtual DiagramGridPainter CreateGridPainter() {
			return new DiagramGridPainter();
		}
		protected virtual DiagramGridObjectInfoArgs CreateGridDrawArgs() {
			return new DiagramGridObjectInfoArgs();
		}
		protected virtual DiagramBackgroundObjectInfoArgs CreateBackgroundDrawArgs() {
			return new DiagramBackgroundObjectInfoArgs();
		}
		protected virtual DiagramBackgroundPainter CreateBackgroundPainter() {
			return new DiagramBackgroundPainter();
		}
		protected virtual DiagramPageObjectInfoArgs CreatePageDrawArgs() {
			return new DiagramPageObjectInfoArgs();
		}
		protected virtual DiagramPagePainter CreatePagePainter() {
			return new DiagramPagePainter();
		}
		protected virtual DiagramItemViewControllers CreateItemViewControllers() {
			return new DiagramItemViewControllers(this);
		}
		protected virtual DiagramConnectorSelectionPartPainter CreateConnectorSelectionPartPainter() {
			return new DiagramConnectorSelectionPartPainter();
		}
		protected virtual DiagramDefaultAppearances CreateDefaultAppearances() {
			return new DiagramDefaultAppearances(LookAndFeel);
		}
		protected virtual Pen CreateConnectorOutlinePen() {
			return new Pen(Color.Empty, ConnectorOutlineSize);
		}
		protected internal virtual int ConnectorOutlineSize { get { return 14; } }
		public bool IsHScrollVisible { get { return isHScrollVisible; } }
		public bool IsVScrollVisible { get { return isVScrollVisible; } }
		#region ShapePaintAppearance
		public virtual DiagramAppearanceObject ShapePaintAppearance {
			get { return shapePaintAppearance; }
			set { SetShapePaintAppearance(value); }
		}
		protected void SetShapePaintAppearance(DiagramAppearanceObject appearance) {
			this.shapePaintAppearance = appearance;
			this.shapePaintAppearance.TextOptions.RightToLeft = RightToLeft;
			OnShapePaintAppearanceChanged();
		}
		protected virtual void OnShapePaintAppearanceChanged() {
		}
		AppearanceDefault defaultShapeAppearance = null;
		public virtual AppearanceDefault DefaultShapeAppearance {
			get {
				if(defaultShapeAppearance == null) {
					defaultShapeAppearance = CreateDefaultShapeAppearance();
				}
				return defaultShapeAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultShapeAppearance() {
			return DefaultAppearances.CreateDefaultShapeAppearance(GetDefaultFont());
		}
		protected internal virtual void ResetDefaultShapeAppearance() {
			this.defaultShapeAppearance = null;
		}
		#endregion
		#region ConnectorPaintAppearance
		public virtual DiagramAppearanceObject ConnectorPaintAppearance {
			get { return connectorPaintAppearance; }
			set { SetConnectorPaintAppearance(value); }
		}
		protected virtual void SetConnectorPaintAppearance(DiagramAppearanceObject appearance) {
			this.connectorPaintAppearance = appearance;
			this.connectorPaintAppearance.TextOptions.RightToLeft = RightToLeft;
			OnConnectorPaintAppearanceChanged();
		}
		protected virtual void OnConnectorPaintAppearanceChanged() {
		}
		AppearanceDefault defaultConnectorAppearance = null;
		public virtual AppearanceDefault DefaultConnectorAppearance {
			get {
				if(defaultConnectorAppearance == null) {
					defaultConnectorAppearance = CreateDefaultConnectorAppearance();
				}
				return defaultConnectorAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultConnectorAppearance() {
			return DefaultAppearances.CreateDefaultConnectorAppearance(GetDefaultFont());
		}
		protected internal virtual void ResetDefaultConnectorAppearance() {
			this.defaultConnectorAppearance = null;
		}
		#endregion
		#region HRuler Paint Appearance
		public virtual DiagramAppearanceObject HRulerPaintAppearance {
			get { return hRulerPaintAppearance; }
			set { SetHRulerPaintAppearance(value); }
		}
		protected void SetHRulerPaintAppearance(DiagramAppearanceObject appearance) {
			this.hRulerPaintAppearance = appearance;
			this.hRulerPaintAppearance.TextOptions.RightToLeft = RightToLeft;
			OnHRulerPaintAppearanceChanged();
		}
		protected virtual void OnHRulerPaintAppearanceChanged() {
		}
		AppearanceDefault defaultHRulerAppearance = null;
		public virtual AppearanceDefault DefaultHRulerAppearance {
			get {
				if(defaultHRulerAppearance == null) {
					defaultHRulerAppearance = CreateDefaultHRulerAppearance();
				}
				return defaultHRulerAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultHRulerAppearance() {
			return DefaultAppearances.CreateDefaultHRulerAppearance();
		}
		protected virtual void ResetDefaultHRulerAppearance() {
			this.defaultHRulerAppearance = null;
		}
		#endregion
		#region VRuler Paint Appearance
		public virtual DiagramAppearanceObject VRulerPaintAppearance {
			get { return vRulerPaintAppearance; }
			set { SetVRulerPaintAppearance(value); }
		}
		protected virtual void SetVRulerPaintAppearance(DiagramAppearanceObject appearance) {
			this.vRulerPaintAppearance = appearance;
			this.vRulerPaintAppearance.TextOptions.RightToLeft = RightToLeft;
			OnVRulerPaintAppearanceChanged();
		}
		protected virtual void OnVRulerPaintAppearanceChanged() {
		}
		AppearanceDefault defaultVRulerAppearance = null;
		public virtual AppearanceDefault DefaultVRulerAppearance {
			get {
				if(defaultVRulerAppearance == null) {
					defaultVRulerAppearance = CreateDefaultVRulerAppearance();
				}
				return defaultVRulerAppearance;
			}
		}
		protected virtual AppearanceDefault CreateDefaultVRulerAppearance() {
			return DefaultAppearances.CreateDefaultVRulerAppearance();
		}
		protected virtual void ResetDefaultVRulerAppearance() {
			this.defaultVRulerAppearance = null;
		}
		#endregion
		public DiagramDefaultAppearances DefaultAppearances { get { return defaultAppearances; } }
		public override void UpdatePaintAppearance() {
			base.UpdatePaintAppearance();
			UpdateShapePaintAppearance();
			UpdateConnectorPaintAppearance();
			UpdateHRulerPaintAppearance();
			UpdateVRulerPaintAppearance();
		}
		protected virtual void UpdateShapePaintAppearance() {
			ResetDefaultShapeAppearance();
			ShapePaintAppearance.Combine(new DiagramAppearanceObject[] { OwnerControl.Appearance.Shape, }, DefaultShapeAppearance);
			ShapePaintAppearance.TextOptions.RightToLeft = RightToLeft;
		}
		protected virtual void UpdateConnectorPaintAppearance() {
			ResetDefaultConnectorAppearance();
			ConnectorPaintAppearance.Combine(new DiagramAppearanceObject[] { OwnerControl.Appearance.Connector, }, DefaultConnectorAppearance);
			ConnectorPaintAppearance.TextOptions.RightToLeft = RightToLeft;
		}
		protected virtual void UpdateHRulerPaintAppearance() {
			ResetDefaultHRulerAppearance();
			HRulerPaintAppearance.Combine(new DiagramAppearanceObject[] { OwnerControl.Appearance.HRuler, }, DefaultHRulerAppearance);
			HRulerPaintAppearance.TextOptions.RightToLeft = RightToLeft;
		}
		protected virtual void UpdateVRulerPaintAppearance() {
			ResetDefaultVRulerAppearance();
			VRulerPaintAppearance.Combine(new DiagramAppearanceObject[] { OwnerControl.Appearance.VRuler, }, DefaultVRulerAppearance);
			VRulerPaintAppearance.TextOptions.RightToLeft = RightToLeft;
		}
		protected override void UpdateFromOwner() {
			base.UpdateFromOwner();
			if(OwnerControl == null) return;
			Bounds = OwnerControl.ClientRectangle;
			Focused = OwnerControl.ContainsFocus;
			RightToLeft = WindowsFormsSettings.GetIsRightToLeft(OwnerControl);
			UpdatePaintAppearance();
		}
		public override bool AllowAnimation {
			get { return true; }
		}
		protected internal virtual void OnAppearanceChanged() {
			PaintCache.ClearCache();
		}
		public Rectangle PageDisplayRect { get { return pageDisplayRect; } }
		public Rectangle PageContentDisplayRect { get { return pageContentDisplayRect; } }
		public Rectangle PageContentRect { get { return pageContentRect; } }
		public Rectangle GridDrawArea { get { return gridDrawArea; } }
		public Point GridContentLocation { get { return gridContentLocation; } }
		public Rectangle HRulerBounds { get { return hRulerBounds; } }
		public Rectangle VRulerBounds { get { return vRulerBounds; } }
		public DiagramHRulerPainter HRulerPainter { get { return hRulerPainter; } }
		public DiagramVRulerPainter VRulerPainter { get { return vRulerPainter; } }
		public DiagramHRulerBackgroundPainter HRulerBackgroundPainter { get { return hRulerBackgroundPainter; } }
		public DiagramVRulerBackgroundPainter VRulerBackgroundPainter { get { return vRulerBackgroundPainter; } }
		public DiagramHRulerObjectInfoArgs HRulerDrawArgs { get { return hRulerDrawArgs; } }
		public DiagramVRulerObjectInfoArgs VRulerDrawArgs { get { return vRulerDrawArgs; } }
		public DiagramShapeParser ShapeParser { get { return shapeParser; } }
		public DiagramConnectorParser ConnectorParser { get { return connectorParser; } }
		public DiagramShapePainter ShapePainter { get { return shapePainter; } }
		public DiagramShapeDragPreviewPainter DragPreviewShapePainter { get { return dragPreviewShapePainter; } }
		public DiagramConnectorDragPreviewPainter DragPreviewConnectorPainter { get { return dragPreviewConnectorPainter; } }
		public DiagramConnectorPointDragPreviewPainter ConnectorPointDragPreviewPainter { get { return connectorPointDragPreviewPainter; } }
		public DiagramConnectorSelectionPartPainter ConnectorSelectionPartPainter { get { return connectorSelectionPartPainter; } }
		public DiagramShapeObjectInfoArgs ShapeDrawArgs { get { return shapeDrawArgs; } }
		public DiagramItemParseStrategy ItemParseStrategy { get { return itemParseStrategy; } }
		public DiagramContainerPainter ContainerItemPainter { get { return containerItemPainter; } }
		public DiagramContainerObjectInfoArgs ContainerItemDrawArgs { get { return containerItemDrawArgs; } }
		public DiagramConnectorPainter ConnectorPainter { get { return connectorPainter; } }
		public DiagramConnectorObjectInfoArgs ConnectorDrawArgs { get { return connectorDrawArgs; } }
		public DiagramGridPainter GridPainter { get { return gridPainter; } }
		public DiagramGridObjectInfoArgs GridDrawArgs { get { return gridDrawArgs; } }
		public DiagramBackgroundPainter BackgroundPainter { get { return backgroundPainter; } }
		public DiagramBackgroundObjectInfoArgs BackgroundDrawArgs { get { return backgroundDrawArgs; } }
		public DiagramPagePainter PagePainter { get { return pagePainter; } }
		public DiagramPageObjectInfoArgs PageDrawArgs { get { return pageDrawArgs; } }
		public DiagramPaintCache PaintCache { get { return paintCache; } }
		public DiagramAdornerController AdornerController { get { return OwnerControl.AdornerController; } }
		protected override void CalcClientRect(Rectangle bounds) {
			base.CalcClientRect(bounds);
			CalcRulers();
		}
		protected virtual void CalcRulers() {
			this.hRulerBounds = CalcHRulerBounds();
			this.vRulerBounds = CalcVRulerBounds();
		}
		protected virtual int RulerMetric { get { return 20; } }
		protected virtual Rectangle CalcHRulerBounds() {
			Rectangle rect = ClientRect;
			rect.Height = RulerMetric;
			return rect;
		}
		protected virtual Rectangle CalcVRulerBounds() {
			Rectangle rect = ClientRect;
			rect.Width = RulerMetric;
			return rect;
		}
		public int RulerMargin { get { return 2; } }
		public Rectangle HRulerContentBounds {
			get {
				Rectangle bounds = HRulerBounds;
				bounds.X += RulerMetric;
				bounds.Y += RulerMargin;
				bounds.Height -= RulerMargin;
				return bounds;
			}
		}
		public Rectangle VRulerContentBounds {
			get {
				Rectangle bounds = VRulerBounds;
				bounds.X += RulerMargin;
				bounds.Y += RulerMetric;
				bounds.Width -= RulerMargin;
				return bounds;
			}
		}
		public virtual int RulerTickSize { get { return 6; } }
		public virtual Rectangle CalcHRulerShadowRect(Rectangle itemDisplayBounds) {
			Rectangle rect = Rectangle.Empty;
			rect.X = itemDisplayBounds.X;
			rect.Y = HRulerContentBounds.Y;
			rect.Width = itemDisplayBounds.Width;
			rect.Height = HRulerContentBounds.Height - 1;
			return rect;
		}
		public virtual Rectangle CalcVRulerShadowRect(Rectangle itemDisplayBounds) {
			Rectangle rect = Rectangle.Empty;
			rect.X = VRulerContentBounds.X;
			rect.Y = itemDisplayBounds.Y;
			rect.Width = VRulerContentBounds.Width - 1;
			rect.Height = itemDisplayBounds.Height;
			return rect;
		}
		public bool AllowDrawGrid { get { return OwnerControl.OptionsView.DrawGrid; } }
		public bool AllowDrawRulers { get { return OwnerControl.OptionsView.DrawRulers; } }
		public bool AllowTransparentRulerBackground { get { return OwnerControl.OptionsView.AllowTransparentRulerBackground(); } }
		public virtual Point CalcHBoundsSnapLineStartPos(Rectangle displayBounds) {
			return PointUtils.ApplyOffset(displayBounds.Location, -BoundsSnapLineExceed, 0);
		}
		public virtual Point CalcHBoundsSnapLineEndPos(Rectangle displayBounds) {
			return PointUtils.ApplyOffset(displayBounds.Location, displayBounds.Width + BoundsSnapLineExceed, 0);
		}
		public virtual Point CalcVBoundsSnapLineStartPos(Rectangle displayBounds) {
			return PointUtils.ApplyOffset(displayBounds.Location, 0, -BoundsSnapLineExceed);
		}
		public virtual Point CalcVBoundsSnapLineEndPos(Rectangle displayBounds) {
			return PointUtils.ApplyOffset(displayBounds.Location, 0, displayBounds.Height + BoundsSnapLineExceed);
		}
		protected virtual int BoundsSnapLineExceed { get { return 7; } }
		public virtual Point CalcHSizeSnapLineStartPos(DiagramAdornerBase adorner) {
			return PointUtils.ApplyOffset(adorner.DisplayBounds.Location, 0, SizeSnapLineMargin);
		}
		public virtual Point CalcHSizeSnapLineEndPos(DiagramAdornerBase adorner) {
			Point startPos = adorner.DisplayBounds.Location;
			return PointUtils.ApplyOffset(startPos, adorner.DisplayBounds.Width, SizeSnapLineMargin);
		}
		public virtual Point CalcVSizeSnapLineStartPos(DiagramAdornerBase adorner) {
			return PointUtils.ApplyOffset(adorner.DisplayBounds.Location, SizeSnapLineMargin, 0);
		}
		public virtual Point CalcVSizeSnapLineEndPos(DiagramAdornerBase adorner) {
			Point startPos = adorner.DisplayBounds.Location;
			return PointUtils.ApplyOffset(startPos, SizeSnapLineMargin, adorner.DisplayBounds.Height);
		}
		protected internal virtual int SizeSnapLineMargin { get { return 16; } }
		public virtual PointPair[] CalcHSizeSnapLineDelimiters(PointPair snapLine, DiagramAdornerBase adorner) {
			PointPair[] pair = new PointPair[2];
			pair[0] = snapLine.Start.CreateVLine(SizeSnapLineDelimiterSize);
			pair[1] = snapLine.End.CreateVLine(SizeSnapLineDelimiterSize);
			return pair;
		}
		public virtual PointPair[] CalcVSizeSnapLineDelimiters(PointPair snapLine, DiagramAdornerBase adorner) {
			PointPair[] pair = new PointPair[2];
			pair[0] = snapLine.Start.CreateHLine(SizeSnapLineDelimiterSize);
			pair[1] = snapLine.End.CreateHLine(SizeSnapLineDelimiterSize);
			return pair;
		}
		protected internal virtual int SizeSnapLineDelimiterSize { get { return 17; } }
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			AdjustContent();
			CalcPageRects();
			CalcItems();
			CalcAdornments();
		}
		protected internal DiagramItemInfo GetItemInfo(DiagramItem item) {
			DiagramItemInfo itemInfo = Items[item] as DiagramItemInfo;
			if(itemInfo == null)
				itemInfo = GetChildItemInfo(item);
			return itemInfo;
		}
		protected internal DiagramItemInfo GetChildItemInfo(DiagramItem childItem) {
			foreach(DiagramItemInfo item in VisibleItems) {
				DiagramCompositeItemInfo containerInfo = item as DiagramCompositeItemInfo;
				if(containerInfo != null && containerInfo.Contains(childItem)) {
					return containerInfo.GetChildInfo(childItem);
				}
			}
			return null;
		}
		protected internal Rectangle GetItemDisplayBounds(DiagramItem item) {
			Rectangle logicalRect = GetItemInfo(item).Bounds;
			return LogicalRectToDisplayRect(logicalRect);
		}
		protected internal DiagramItems Items { get { return items; } }
		protected virtual void AdjustContent() {
			bool allowHScroll = CalcHScrollBarVisibility();
			bool allowVScroll = CalcVScrollBarVisibility();
			if(allowHScroll) {
				this.fContentRect.Height -= OwnerControl.ScrollingController.HScrollHeight;
			}
			if(allowVScroll) {
				this.fContentRect.Width -= OwnerControl.ScrollingController.VScrollWidth;
			}
			this.isHScrollVisible = allowHScroll;
			this.isVScrollVisible = allowVScroll;
			if(AllowDrawRulers) {
				this.fContentRect.X += RulerMetric;
				this.fContentRect.Width -= RulerMetric;
				this.fContentRect.Y += RulerMetric;
				this.fContentRect.Height -= RulerMetric;
			}
			this.fContentRect.Width = Math.Max(this.fContentRect.Width, 0);
			this.fContentRect.Height = Math.Max(this.fContentRect.Height, 0);
		}
		protected virtual bool CalcHScrollBarVisibility() {
			return Extent.Width > ViewPort.Width;
		}
		protected virtual bool CalcVScrollBarVisibility() {
			return Extent.Height > ViewPort.Height;
		}
		protected virtual void CalcPageRects() {
			this.pageContentRect = CalcPageContentRect();
			this.pageContentDisplayRect = CalcPageContentDisplayRect();
			this.gridDrawArea = CalcGridDrawArea();
			this.pageDisplayRect = CalcPageDisplayRect();
			this.gridContentLocation = CalcGridContentLocation();
		}
		public int PageWidth { get { return PageSize.Width; } }
		public int PageHeight { get { return PageSize.Height; } }
		protected virtual Rectangle CalcPageContentRect() {
			return new Rectangle(ContentRect.Location, PageSize);
		}
		protected Size PageSize { get { return OwnerControl.OptionsView.PageSize.ToSize(); } }
		protected virtual Rectangle CalcPageDisplayRect() {
			SkinElement skinElement = PagePainter.GetPageElement(LookAndFeel);
			return skinElement.CoerceBounds(PageContentDisplayRect);
		}
		protected virtual Rectangle CalcGridDrawArea() {
			return new Rectangle(Point.Empty, PageContentDisplayRect.Size);
		}
		protected virtual Point CalcGridContentLocation() {
			Point pageContentLoc = PageContentDisplayRect.Location;
			if(AllowPageCache()) {
				return PointUtils.ApplyOffset(pageContentLoc, -PageDisplayRect.X, -PageDisplayRect.Location.Y);
			}
			return pageContentLoc;
		}
		public Image RotationImage {
			get { return rotationImage ?? (rotationImage = LoadDefaultRotationImage()); }
		}
		protected virtual Image LoadDefaultRotationImage() {
			return DefaultImages.RotateIcon;
		}
		protected internal virtual int RotationGripBoundsOffset { get { return 6; } }
		protected internal virtual Size SelectionSizeGripSize { get { return new Size(6, 6); } }
		protected internal virtual RotationGrip GetRotationGrip() {
			return new RotationGrip(RotationImage.Size, RotationGripBoundsOffset);
		}
		protected internal virtual Size ConnectionPointSize { get { return new Size(4, 4); } }
		protected internal virtual Size HotConnectionPointSize { get { return new Size(10, 10); } }
		protected internal bool AllowPageCache() {
			return PageDisplayRect.Width * PageDisplayRect.Height < MaximumAcceptableCachedPageSize;
		}
		protected virtual int MaximumAcceptableCachedPageSize { get { return 1850 * 1300; } }
		protected internal void CheckPaintCache() {
			if(PaintCache.PageImageCacheSize > MaximumPageCacheSize) PaintCache.ClearPageCache();
		}
		protected virtual int MaximumPageCacheSize { get { return 10; } }
		protected virtual Rectangle CalcPageContentDisplayRect() {
			return LogicalRectToDisplayRect(PageContentRect);
		}
		protected internal DiagramItemViewControllers ItemViewControllers { get { return itemViewControllers; } }
		protected virtual void CalcItems() {
			Items.ClearItems();
			DiagramItemCollection items = OwnerControl.Items;
			for(int i = 0; i < items.Count; i++) {
				DiagramItem item = items[i];
				Items.Add(item, ItemViewControllers.GetController(item).CalcItemInfo(item, PageContentRect, CalcItemAppearance(item)));
			}
		}
		protected DiagramAppearanceObject CalcItemAppearance(DiagramItem item) {
			DiagramAppearanceObject appearance = new DiagramAppearanceObject();
			if(AllowThemes(item)) {
				DiagramAppearanceObject themeAppearance = DiagramThemeUtils.CreateAppearance(OwnerControl.OptionsView.Theme, item);
				appearance.Combine(item.Appearance, GetItemAppearance(item), themeAppearance);
			}
			else {
				appearance.Combine(item.Appearance, GetItemPaintAppearance(item));
			}
			return appearance;
		}
		protected bool AllowThemes(DiagramItem item) {
			return OwnerControl.OptionsView.AllowThemes && item.ThemeStyleId != null;
		}
		protected DiagramAppearanceObject GetItemPaintAppearance(DiagramItem item) {
			if(item.IsRoutable) return ConnectorPaintAppearance;
			return ShapePaintAppearance;
		}
		protected DiagramAppearanceObject GetItemAppearance(DiagramItem item) {
			if(item.IsRoutable) return OwnerControl.Appearance.Connector;
			return OwnerControl.Appearance.Shape;
		}
		protected virtual void CalcAdornments() {
			AdornerController.ForEach(adorner => adorner.Update());
		}
		public IList VisibleItems { get { return Items.VisibleItems; } }
		public Point ClientDisplayPointToPageLogical(Point pt) {
			Point logicalPoint = PointUtils.ApplyOffset(pt, -ContentRect.X, -ContentRect.Y);
			return LayersHostController.TransformToLogicPoint(logicalPoint);
		}
		public Point ClientDisplayPointToLogical(Point pt) {
			Point logicalPoint = ClientDisplayPointToPageLogical(pt);
			return PointUtils.ApplyOffset(logicalPoint, ContentRect.Location);
		}
		public Point ContentDisplayPointToLogical(Point pt) {
			Point point = LayersHostController.TransformToLogicPoint(pt);
			return PointUtils.ApplyOffset(point, ContentRect.Location);
		}
		public Point LogicalPointToDisplayPoint(Point point) {
			Matrix matrix = GetLogicToDisplayTransformMatrix();
			Point[] points = new Point[] { point };
			matrix.TransformPoints(points);
			return points.First();
		}
		public Rectangle LogicalRectToDisplayRect(Rectangle rect) {
			Matrix matrix = GetLogicToDisplayTransformMatrix();
			Point[] points = rect.GetPoints();
			matrix.TransformPoints(points);
			return RectangleUtils.FromPoints(points);
		}
		public virtual DiagramElementBounds PlatformToDiagramRect(Rectangle rect) {
			Rectangle logicalBounds = RectangleUtils.ApplyOffset(rect, ContentRect.Location);
			Rectangle displayBounds = LogicalRectToDisplayRect(logicalBounds);
			return new DiagramElementBounds(logicalBounds, displayBounds);
		}
		public virtual DiagramElementPoint PlatformToDiagramPoint(Point point) {
			Point logicalPoint = PointUtils.ApplyOffset(point, ContentRect.Location);
			return LogicalToDiagramPoint(logicalPoint);
		}
		public virtual DiagramElementPoint LogicalToDiagramPoint(Point point) {
			Point displayPoint = LogicalPointToDisplayPoint(point);
			return new DiagramElementPoint(point, displayPoint);
		}
		public Matrix GetLogicToDisplayTransformMatrix() {
			return LayersHostController.CreateLogicToDisplayTransform();
		}
		public bool IsTextEditMode { get { return AdornerController.IsEditMode; } }
		public DiagramAnimationController AnimationController { get { return OwnerControl.AnimationController; } }
		public virtual DiagramEditInfoArgs CalcDiagramEditInfoArgs(Rectangle adornerBounds, DiagramItem item) {
			return new DiagramEditInfoArgs(adornerBounds, CalcDiagramEditorRects(adornerBounds, item, item.EditValue), GetDiagramEditViewInfo(item), item.EditValue, LookAndFeel, RightToLeft);
		}
		public virtual DiagramEditInfoArgs UpdateDiagramEditInfoArgs(DiagramEditInfoArgs editInfoArgs, DiagramItem item, string editValue) {
			Rectangle adornerBounds = editInfoArgs.AdornerBounds;
			return editInfoArgs.UpdateRects(adornerBounds, CalcDiagramEditorRects(adornerBounds, item, editValue));
		}
		protected virtual DiagramEditorRects CalcDiagramEditorRects(Rectangle adornerBounds, DiagramItem item, string editValue) {
			Rectangle editorBounds = CalcDiagramEditorBounds(adornerBounds);
			Rectangle textRect = CalcDiagramEditorTextRect(editorBounds, item, editValue);
			Rectangle clipRect = CalcDiagramEditorClipRect(editorBounds);
			return new DiagramEditorRects(editorBounds, textRect, clipRect);
		}
		public virtual Rectangle CalcDiagramEditorBounds(Rectangle adornerBounds) {
			if(adornerBounds.IsEmpty) return Rectangle.Empty;
			return Rectangle.Inflate(adornerBounds, -1, -1);
		}
		protected virtual Rectangle CalcDiagramEditorTextRect(Rectangle editorBounds, DiagramItem item, string editValue) {
			if(editorBounds.IsEmpty) return Rectangle.Empty;
			int editBestHeight = DigramTextUtils.CalcTextBoxBestHeight(editValue, GetDiagramEditorFont(item), editorBounds.Size);
			if(editBestHeight > editorBounds.Height) return new Rectangle(Point.Empty, editorBounds.Size);
			int y = editorBounds.Height / 2 - editBestHeight / 2;
			return new Rectangle(0, y, editorBounds.Width, editorBounds.Height);
		}
		public Rectangle CalcDiagramEditorClipRect(Rectangle editorBounds) {
			return RectangleUtils.CalcClipRect(editorBounds, ContentRect);
		}
		protected virtual DiagramEditViewInfo GetDiagramEditViewInfo(DiagramItem item) {
			DiagramAppearanceObject appearance = GetItemInfo(item).PaintAppearance;
			Font font = GetDiagramEditorFont(item);
			Color backColor = DefaultAppearances.GetEditorBackColor();
			Color foreColor = DefaultAppearances.GetEditTextForeColor(item);
			return new DiagramEditViewInfo(font, backColor, foreColor);
		}
		protected virtual Font GetDiagramEditorFont(DiagramItem item) {
			DiagramAppearanceObject appearance = GetItemInfo(item).PaintAppearance;
			return FontUtils.CreateFont(appearance, ZoomFactor);
		}
		public Size CalcDiagramEditorBestSize(DiagramItem item) {
			IXtraPathView pathView = item as IXtraPathView;
			if(pathView == null) {
				throw new ArgumentException("item");
			}
			return CalcDiagramEditorBestSize(item, pathView.Text);
		}
		public Size CalcDiagramEditorBestSize(DiagramItem item, string editValue) {
			Size size = Size.Empty;
			using(Font font = GetDiagramEditorFont(item)) {
				size = DigramTextUtils.CalcTextBoxBestSize(editValue, font).ApplyPadding(TextBoxMargins);
			}
			return size;
		}
		protected virtual Padding TextBoxMargins { get { return new Padding(4); } }
		public DiagramControlHitInfo CalcHitInfo(Point pt) {
			Point logicalPoint = ClientDisplayPointToLogical(pt);
			return CalcHitInfoCore(logicalPoint);
		}
		protected internal virtual DiagramControlHitInfo CalcHitInfoCore(int x, int y) {
			return CalcHitInfoCore(new Point(x, y));
		}
		protected internal virtual DiagramControlHitInfo CalcHitInfoCore(Point pt) {
			DiagramControlHitInfo hitInfo = new DiagramControlHitInfo(pt);
			if(AdornerController.InEditSurface(pt)) {
				hitInfo.SetHitTest(DiagramControlHitTest.EditSurface);
				return hitInfo.SetHitObject(AdornerController.GetEditSurfaceItem(pt));
			}
			if(AdornerController.InConnectorBeginPoint(pt)) {
				hitInfo.SetHitTest(DiagramControlHitTest.ConnectorBeginPoint);
				return hitInfo.SetHitObject(AdornerController.GetConnectorBeginPointItem(pt));
			}
			if(AdornerController.InConnectorEndPoint(pt)) {
				hitInfo.SetHitTest(DiagramControlHitTest.ConnectorEndPoint);
				return hitInfo.SetHitObject(AdornerController.GetConnectorEndPointItem(pt));
			}
			if(AdornerController.InConnectorIntermediatePoint(pt)) {
				hitInfo.SetHitTest(DiagramControlHitTest.ConnectorIntermediatePoint);
				return hitInfo.SetHitObject(AdornerController.GetConnectorIntermediatePointItem(pt));
			}
			if(AdornerController.InShapeParameter(pt)) {
				hitInfo.SetHitTest(DiagramControlHitTest.ShapeParameter);
				return hitInfo.SetHitObject(AdornerController.GetShapeParameterItem(pt));
			}
			if(AdornerController.InRotationGrip(pt)) {
				return hitInfo.SetHitTest(DiagramControlHitTest.RotationGrip);
			}
			if(AdornerController.InSelectionSizeGrip(pt)) {
				hitInfo.SetHitTest(DiagramControlHitTest.ShapeSizeGrip);
				return hitInfo.SetHitObject(AdornerController.GetSizeGripItem(pt));
			}
			IList itemCol = VisibleItems;
			for(int i = itemCol.Count - 1; i >= 0; i--) {
				DiagramItemInfo itemInfo = (DiagramItemInfo)itemCol[i];
				if(itemInfo.HitInItem(pt)) {
					hitInfo.SetHitTest(DiagramControlHitTest.Item);
					return hitInfo.SetHitObject(itemInfo.GetHitItem(pt));
				}
			}
			if(PageContentRect.Contains(pt)) {
				return hitInfo.SetHitTest(DiagramControlHitTest.Page);
			}
			if(ClientRect.Contains(pt)) {
				return hitInfo.SetHitTest(DiagramControlHitTest.Client);
			}
			if(Bounds.Contains(pt)) {
				return hitInfo.SetHitTest(DiagramControlHitTest.Border);
			}
			return hitInfo;
		}
		protected virtual int SmallScrollChange { get { return 5; } }
		public virtual ScrollArgs CalcHScrollArgs() {
			ScrollArgs scrollArgs = new ScrollArgs();
			scrollArgs.Minimum = Location.X;
			scrollArgs.Maximum = Math.Max(Location.X, Extent.Width - 1);
			scrollArgs.SmallChange = SmallScrollChange;
			scrollArgs.LargeChange = HScrollScrollLargeChange;
			scrollArgs.Value = OffsetPoint.X + Location.X;
			return scrollArgs;
		}
		public virtual ScrollArgs CalcVScrollArgs() {
			ScrollArgs scrollArgs = new ScrollArgs();
			scrollArgs.Minimum = Location.Y;
			scrollArgs.Maximum = Math.Max(Location.Y, Extent.Height - 1);
			scrollArgs.SmallChange = SmallScrollChange;
			scrollArgs.LargeChange = VScrollScrollLargeChange;
			scrollArgs.Value = OffsetPoint.Y + Location.Y;
			return scrollArgs;
		}
		protected virtual int HScrollScrollLargeChange { get { return ViewPort.Width; } }
		protected virtual int VScrollScrollLargeChange { get { return ViewPort.Height; } }
		protected internal Size ViewPort {
			get { return LayersHostController.Viewport.ToWinSize(); }
		}
		protected internal Size Extent {
			get { return LayersHostController.Extent.Size.ToWinSize(); }
		}
		public XtraDiagramController DiagramController { get { return OwnerControl.Controller; } }
		protected internal int MaximumVScrollValue {
			get { return Extent.Height + Location.Y - ViewPort.Height; }
		}
		protected internal int MaximumHScrollValue {
			get { return Extent.Width + Location.X - ViewPort.Width; }
		}
		protected internal Point Location {
			get { return LayersHostController.Extent.Location.ToWinPoint(); }
		}
		protected Point OffsetPoint {
			get { return LayersHostController.Offset.ToWinPoint(); }
		}
		public double HRulerOffset {
			get { return PageContentDisplayRect.Left - ContentRect.X; }
		}
		public double VRulerOffset {
			get { return PageContentDisplayRect.Top - ContentRect.Top; }
		}
		protected XtraLayersHostController LayersHostController { get { return OwnerControl.LayersHostController; } }
		public double ZoomFactor { get { return OwnerControl.OptionsView.ZoomFactor; } }
		public MeasureUnit MeasureUnit { get { return OwnerControl.OptionsView.MeasureUnit; } }
		public Point HRulerLabelOffset { get { return new Point(2, 0); } }
		public Point VRulerLabelOffset { get { return new Point(1, -1); } }
		public SizeF? GridSize { get { return OwnerControl.OptionsView.GridSize; } }
		public virtual int GetMouseWheelDistance() {
			return (int)(SystemInformation.MouseWheelScrollLines * 5 * ZoomFactor);
		}
		public IInputElement FindInputElement(Point pt) {
			return FindDiagramInputElement(ContentDisplayPointToLogical(pt));
		}
		protected virtual IInputElement FindDiagramInputElement(Point pt) {
			DiagramControlHitInfo hitInfo = CalcHitInfoCore(pt);
			if(hitInfo.InShapeParameter) {
				return CreateChangeShapeParameterInputElement(hitInfo.ShapeParameterItem);
			}
			if(hitInfo.InRotationGrip) {
				return CreateItemRotateInputElement();
			}
			if(hitInfo.InShapeSizeGrip) {
				return CreateItemResizeInputElement(hitInfo.SizeGripItem);
			}
			if(hitInfo.InConnectorBeginPoint) {
				return CreateMoveBeginPointInputElement(hitInfo.ConnectorPointItem);
			}
			if(hitInfo.InConnectorEndPoint) {
				return CreateMoveEndPointInputElement(hitInfo.ConnectorPointItem);
			}
			if(hitInfo.InConnectorIntermediatePoint) {
				return CreateMoveIntermediatePointInputElement(hitInfo.ConnectorPointItem);
			}
			if(hitInfo.InItem) {
				return hitInfo.ItemInfo.Item.Controller.CreateInputElement();
			}
			return OwnerControl.Page.Controller.CreateInputElement();
		}
		protected virtual IInputElement CreateChangeShapeParameterInputElement(DiagramShapeParameterItem parameterItem) {
			return ChangeParameterInputElement.Create(parameterItem.Shape, parameterItem.Parameter);
		}
		protected virtual IInputElement CreateItemRotateInputElement() {
			return RotateShapeInputElement.Create(AdornerController.GetSelectionAdorner().LayerHandler);
		}
		protected virtual IInputElement CreateItemResizeInputElement(DiagramSizeGripItem sizeGripItem) {
			SizeGripKind gripKind = sizeGripItem.GripKind;
			return ItemResizeInputElement.Create(gripKind.GetResizeMode(), AdornerController.GetSelectionAdorner().LayerHandler);
		}
		protected virtual IInputElement CreateMoveBeginPointInputElement(DiagramConnectorPointItem connectorPointItem) {
			return MoveConnectorPointInputElement.CreateMoveBeginEndPointElement(connectorPointItem.Connector, ConnectorPointType.Begin);
		}
		protected virtual IInputElement CreateMoveEndPointInputElement(DiagramConnectorPointItem connectorPointItem) {
			return MoveConnectorPointInputElement.CreateMoveBeginEndPointElement(connectorPointItem.Connector, ConnectorPointType.End);
		}
		protected virtual IInputElement CreateMoveIntermediatePointInputElement(DiagramConnectorPointItem connectorPointItem) {
			return MoveConnectorPointInputElement.CreateMoveMiddlePointElement(connectorPointItem.Connector, connectorPointItem.PointIndex);
		}
		protected internal Rectangle GetPageDisplayRectRelativeContent() {
			return RectangleUtils.ApplyOffset(PageContentDisplayRect, -ContentRect.X, -ContentRect.Y);
		}
		protected internal Point GetShapeParameterPos(DiagramShape shape) {
			DiagramShapeParametersAdorner adorner = AdornerController.GetShapeParameterAdorner();
			return PointUtils.ApplyOffset(shape.Bounds.Location, PageContentRect.Location, adorner.Parameters.First().Point.ToWinPoint());
		}
		public Pen ConnectorOutlinePen { get { return connectorOutlinePen; } }
		#region Disposing
		public override void Dispose() {
			if(this.items != null) {
				this.items.ClearItems();
				this.items = null;
			}
			this.shapePainter = null;
			this.shapeDrawArgs = null;
			this.dragPreviewShapePainter = null;
			this.dragPreviewConnectorPainter = null;
			this.connectorPointDragPreviewPainter = null;
			this.connectorSelectionPartPainter = null;
			if(this.shapeParser != null) {
				this.shapeParser.Dispose();
				this.shapeParser = null;
			}
			this.containerItemPainter = null;
			this.containerItemDrawArgs = null;
			if(this.connectorParser != null) {
				this.connectorParser.Dispose();
				this.connectorParser = null;
			}
			this.defaultShapeAppearance = null;
			this.defaultConnectorAppearance = null;
			if(this.shapePaintAppearance != null) {
				this.shapePaintAppearance.Dispose();
			}
			this.shapePaintAppearance = null;
			this.connectorPainter = null;
			this.connectorDrawArgs = null;
			if(this.connectorPaintAppearance != null) {
				this.connectorPaintAppearance.Dispose();
			}
			this.connectorPaintAppearance = null;
			if(this.hRulerPaintAppearance != null) {
				this.hRulerPaintAppearance.Dispose();
			}
			this.hRulerPaintAppearance = null;
			this.hRulerPainter = null;
			this.vRulerPainter = null;
			this.hRulerDrawArgs = null;
			this.vRulerDrawArgs = null;
			this.hRulerBackgroundPainter = null;
			this.vRulerBackgroundPainter = null;
			if(this.vRulerPaintAppearance != null) {
				this.vRulerPaintAppearance.Dispose();
			}
			this.vRulerPaintAppearance = null;
			this.gridPainter = null;
			this.gridDrawArgs = null;
			this.itemParseStrategy = null;
			if(this.connectorOutlinePen != null) {
				this.connectorOutlinePen.Dispose();
			}
			this.rotationImage = null;
			this.connectorOutlinePen = null;
			this.backgroundPainter = null;
			this.backgroundDrawArgs = null;
			this.pagePainter = null;
			this.pageDrawArgs = null;
			if(this.paintCache != null) {
				this.paintCache.Dispose();
				this.paintCache = null;
			}
			if(this.defaultAppearances != null) {
				this.defaultAppearances.Dispose();
				this.defaultAppearances = null;
			}
			base.Dispose();
		}
		#endregion
		public new DiagramControl OwnerControl { get { return base.OwnerControl as DiagramControl; } }
	}
}
