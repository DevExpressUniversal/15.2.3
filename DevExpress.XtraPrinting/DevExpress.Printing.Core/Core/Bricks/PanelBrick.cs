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

using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting {
#if !SL
	[
	BrickExporter(typeof(PanelBrickExporter))
	]
#endif
	public class PanelBrick : VisualBrick, IPanelBrick {
		#region static
		static RectangleF UnionRects(RectangleF[] rects) {
			RectangleF result = RectangleF.Empty;
			foreach(RectangleF rect in rects)
				result = result == RectangleF.Empty ? rect : RectangleF.Union(result, rect);
			return result;
		}
		#endregion
		BrickCollectionBase bricks;
		IList IPanelBrick.Bricks { get { return bricks; } }
		#region hidden properties
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override object TextValue { get { return base.TextValue; } set { base.TextValue = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string TextValueFormatString { get { return base.TextValueFormatString; } set { base.TextValueFormatString = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override string XlsxFormatString { get { return base.XlsxFormatString; } set { base.XlsxFormatString = value; } }
		#endregion
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("PanelBrickBricks"),
#endif
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached),
		EditorBrowsable(EditorBrowsableState.Always),
		]
		public override BrickCollectionBase Bricks { get { return bricks; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PanelBrickBrickType")]
#endif
		public override string BrickType { get { return BrickTypes.Panel; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("PanelBrickMerged")]
#endif
		public bool Merged {
			get { return flags[bitMerged]; }
			set { flags[bitMerged] = value; }
		}
		public PanelBrick(BrickStyle style)
			: base(style) {
			bricks = new BrickCollectionBase(this);
		}
		public PanelBrick()
			: this(NullBrickOwner.Instance) {
		}
		public PanelBrick(IBrickOwner brickOwner)
			: base(brickOwner) {
			bricks = new BrickCollectionBase(this);
		}
		internal PanelBrick(PanelBrick panelBrick) : base(panelBrick) {
			bricks = new BrickCollectionBase(this);
		}
		#region serialization
		protected override void SetIndexCollectionItemCore(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.Bricks)
				Bricks.Add((Brick)e.Item.Value);
			base.SetIndexCollectionItemCore(propertyName, e);
		}
		protected override object CreateCollectionItemCore(string propertyName, XtraItemEventArgs e) {
			if(propertyName == PrintingSystemSerializationNames.Bricks)
				return BrickFactory.CreateBrick(e);
			return base.CreateCollectionItemCore(propertyName, e);
		}
		#endregion
		protected override void OnSetPrintingSystem(bool cacheStyle) {
			base.OnSetPrintingSystem(cacheStyle);
			foreach(Brick brick in bricks)
				InitializeBrick(brick, cacheStyle);
		}
		public override void Dispose() {
			base.Dispose();
			foreach(Brick brick in bricks)
				brick.Dispose();
			bricks.Clear();
		}
		protected internal override void Scale(double scaleFactor) {
			base.Scale(scaleFactor);
			foreach(Brick brick in bricks)
				brick.Scale(scaleFactor);
		}
		public void CenterChildControls() {
			RemoveSpanPanels();
			PointF diffPoint = GetDiffPoint(Rect);
			foreach(Brick brick in this.bricks) {
				brick.InitialRect = new RectangleF(new PointF(brick.InitialRect.Left + diffPoint.X,
					brick.InitialRect.Top + diffPoint.Y), brick.InitialRect.Size);
			}
			AddAdditionalBricks();
		}
		void RemoveSpanPanels() {
			for(int i = Bricks.Count - 1; i >= 0; i--) {
				if(Bricks[i] is SpanPanelBrick)
				   Bricks.RemoveAt(i);
			}
		}
		void AddAdditionalBricks() {
			if(Bricks.Count == 0) 
				return;
			SpanPanelBrick topPanel = new SpanPanelBrick();
			SpanPanelBrick bottomPanel = new SpanPanelBrick();
			bottomPanel.BackColor = topPanel.BackColor = BackColor;
			bottomPanel.BorderStyle = topPanel.BorderStyle = BorderStyle;
			bottomPanel.BorderWidth = topPanel.BorderWidth = BorderWidth;
			bottomPanel.BorderColor = topPanel.BorderColor = BorderColor;
			RectangleF childBricksRect = GetChildBricksRect();
			topPanel.Rect = new RectangleF(new PointF(0, 0), new SizeF(Rect.Width, childBricksRect.Top));
			bottomPanel.Rect = new RectangleF(0, childBricksRect.Bottom, Rect.Width, Rect.Height - childBricksRect.Bottom);
			topPanel.Sides = BorderSide.Left | BorderSide.Right | BorderSide.Top;
			bottomPanel.Sides = BorderSide.Left | BorderSide.Right | BorderSide.Bottom;
			Bricks.Add(topPanel);
			Bricks.Add(bottomPanel);
		}
		PointF GetDiffPoint(RectangleF clientRect) {
			PointF centerOfBricksRect = GetCenterOfBricksRect();
			PointF centerOfClientRect = new PointF(clientRect.Width / 2, clientRect.Height / 2);
			return new PointF(centerOfClientRect.X - centerOfBricksRect.X, centerOfClientRect.Y - centerOfBricksRect.Y);
		}
		PointF GetCenterOfBricksRect() {
			RectangleF bricksRect = GetChildBricksRect();
			return new PointF(bricksRect.Left + bricksRect.Width / 2, bricksRect.Top + bricksRect.Height / 2);
		}
		RectangleF GetChildBricksRect() {
			RectangleF[] rects = (RectangleF[])Array.CreateInstance(typeof(RectangleF), bricks.Count);
			for(int i = 0; i < bricks.Count; i++ )
				rects[i] = bricks[i].GetViewRectangle();
			return UnionRects(rects);
		}
		internal void InitializeBrick(Brick brick, bool cacheStyle) {
			brick.Modifier = Modifier;
			if(!brick.IsInitialized && PrintingSystem != null)
				PrintingSystem.Graph.InitializeBrick(brick, brick.Rect, cacheStyle);
		}
		protected internal virtual ICollection GetInnerBricks() {
			return Bricks;
		}
		float GetBorderWidth(BorderSide borderSide) {
			return (this.Sides & borderSide) > 0 ? BorderWidth : 0f;
		}
		protected internal override float ValidatePageBottomCore(RectangleF pageBounds, bool enforceSplitNonSeparable, RectangleF rect, IPrintingSystemContext context) {
			return new PageValidator(Bricks).ValidateBottom(pageBounds, enforceSplitNonSeparable, rect, context);
		}
		protected override float ValidatePageRightInternal(float pageRight, RectangleF rect) {
			return new PageValidator(Bricks).ValidateRight(pageRight, rect);
		}
		protected internal override void PerformLayout(IPrintingSystemContext context) {
			PerformInnerBricksLayout(context);
			base.PerformLayout(context);
		}
		protected virtual void PerformInnerBricksLayout(IPrintingSystemContext context) {
			foreach(Brick brick in Bricks) {
				brick.PerformLayout(context);
			}
		}
		protected override bool ShouldSerializeCore(string propertyName) {
			if(propertyName == PrintingSystemSerializationNames.Bricks)
				return Bricks.Count > 0;
			return base.ShouldSerializeCore(propertyName);
		}
		#region ICloneable Members
		public override object Clone() {
			PanelBrick panel = new PanelBrick(this);
			foreach(Brick brick in this.bricks) {
				panel.Bricks.Add((Brick)brick.Clone());
			}
			return panel;
		}
		#endregion
	}
}
namespace DevExpress.XtraPrinting.Native {
	public class PageValidator {
		IList bricks;
		public PageValidator(IList bricks) {
			this.bricks = bricks;
		}
		public float ValidateBottom(RectangleF pageBounds, bool enforceSplitNonSeparable, RectangleF rect, IPrintingSystemContext context) {
			float bricksBottom = float.MinValue;
			foreach(Brick brick in bricks) {
				RectangleF childRect = CreateChildRect(brick, rect);
				float childPageBottom = childRect.Top <= pageBounds.Bottom && pageBounds.Bottom <= childRect.Bottom ?
					brick.ValidatePageBottom(pageBounds, enforceSplitNonSeparable, childRect, context) : float.MinValue;
				if(ComparingUtils.CompareDoubles(childPageBottom, pageBounds.Bottom, 0.001) != 0)
					bricksBottom = Math.Max(bricksBottom, childPageBottom);
			}
			return bricksBottom != float.MinValue ? Math.Min(pageBounds.Bottom, bricksBottom) : pageBounds.Bottom;
		}
		public float ValidateRight(float pageRight, RectangleF rect) {
			if(bricks.Count == 0)
				return rect.Left;
			float initPageRight = pageRight;
			foreach(Brick brick in bricks) {
				RectangleF childRect = CreateChildRect(brick, rect);
				float childPageRight = brick.ValidatePageRight(initPageRight, childRect);
				pageRight = Math.Min(pageRight, childPageRight);
			}
			return pageRight;
		}
		static RectangleF CreateChildRect(Brick childBrick, RectangleF brickRect) {
			RectangleF childRect = childBrick.Rect;
			childRect.X += brickRect.X;
			childRect.Y += brickRect.Y;
			return childRect;
		}
	}
}
#if !SL
namespace DevExpress.XtraPrinting.Native {
	[BrickExporter(typeof(CellWrapperExporter))]
	public abstract class CellWrapper : ITableCell {
		ITableCell innerCell;
		string ITableCell.FormatString { get { return innerCell.FormatString; } }
		string ITableCell.XlsxFormatString { get { return innerCell.XlsxFormatString; } }
		object ITableCell.TextValue { get { return innerCell.TextValue; } }
		DefaultBoolean ITableCell.XlsExportNativeFormat { get { return innerCell.XlsExportNativeFormat; } }
		string ITableCell.Url { get { return innerCell.Url; } }
		BrickModifier ITableCell.Modifier { get { return innerCell.Modifier; } }
		internal ITableCell InnerCell { get { return innerCell; } }
		bool ITableCell.ShouldApplyPadding { get { return innerCell.ShouldApplyPadding; } }
		public CellWrapper(ITableCell innerCell) {
			this.innerCell = innerCell;
		}
	}
	[BrickExporter(typeof(AnchorCellExporter))]
	public class AnchorCell : CellWrapper {
		string anchorName;
		internal string AnchorName { get { return anchorName; } }
		public AnchorCell(ITableCell innerCell, string anchorName) 
			: base(innerCell) {
			this.anchorName = anchorName;
		}
	}
	[BrickExporter(typeof(NavigateUrlCellExporter))]
	public class NavigateUrlCell : CellWrapper {
		VisualBrick brick;
		internal VisualBrick VisualBrick { get { return brick as VisualBrick; } }
		public NavigateUrlCell(ITableCell innerCell, VisualBrick brick)
			: base(innerCell) {
			this.brick = brick;
		}
	}
}
namespace DevExpress.XtraPrinting.BrickExporters {
	public class CellWrapperExporter : BrickExporter {
		protected internal override void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
			GetBrickExporter(exportProvider).FillHtmlTableCell(exportProvider);
		}
		protected internal override void FillRtfTableCellInternal(IRtfExportProvider exportProvider) {
			GetBrickExporter(exportProvider).FillRtfTableCell(exportProvider);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			GetBrickExporter(exportProvider).FillXlsTableCell(exportProvider);
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			GetBrickExporter(exportProvider).FillTextTableCell(exportProvider, shouldSplitText);
		}
		BrickExporter GetBrickExporter(ITableExportProvider provider) {
			return provider.ExportContext.PrintingSystem.ExportersFactory.GetExporter((this.Brick as CellWrapper).InnerCell) as BrickExporter;
		}
	}
	public class AnchorCellExporter : CellWrapperExporter {
		protected internal override void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
			base.FillHtmlTableCellInternal(exportProvider);
			exportProvider.SetAnchor((Brick as AnchorCell).AnchorName);
		}
		protected internal override void FillRtfTableCellInternal(IRtfExportProvider exportProvider) {
			base.FillRtfTableCellInternal(exportProvider);
			exportProvider.SetAnchor((Brick as AnchorCell).AnchorName);
		}
	}
	public class NavigateUrlCellExporter : CellWrapperExporter {
		protected internal override void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
			base.FillHtmlTableCellInternal(exportProvider);
			exportProvider.SetNavigationUrl((Brick as NavigateUrlCell).VisualBrick);
		}
	}
}
#endif
