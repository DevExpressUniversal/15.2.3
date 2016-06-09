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
using System.Text;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.LayoutAdjustment;
using DevExpress.XtraPrinting;
using System.Drawing;
namespace DevExpress.XtraReports.Native.Printing {
	public class SubreportDocumentBand : DocumentBand, ISubreportDocumentBand {
		RectangleF reportRect = RectangleF.Empty;
		DevExpress.XtraPrinting.Native.MultiColumn multiColumn;
		public override DevExpress.XtraPrinting.Native.MultiColumn MultiColumn {
			get { return multiColumn; }
			set { multiColumn = value; }
		}
		RectangleF ISubreportDocumentBand.ReportRect {
			get { return reportRect; }
			set { reportRect = value; }
		}
		public virtual ILayoutData CreateLayoutData(LayoutDataContext context, RectangleF bounds) {
			return new SubreportDocumentBandLayoutData(this, context, bounds);
		}
		public SubreportDocumentBand(RectangleF rect)
			: base(DocumentBandKind.Storage, -1) {
			reportRect = rect;
		}
		protected SubreportDocumentBand(SubreportDocumentBand source, int rowIndex)
			: base(source, rowIndex) {
			this.reportRect = source.reportRect;
			if(source.multiColumn != null)
				multiColumn = new DevExpress.XtraPrinting.Native.MultiColumn(source.multiColumn);
		}
		public override DocumentBand GetInstance(int rowIndex) {
			return new SubreportDocumentBand(this, rowIndex);
		}
		public override void Scale(double scaleFactor, Predicate<DocumentBand> shouldScale) {
			base.Scale(scaleFactor, shouldScale);
			reportRect = MathMethods.Scale(reportRect, scaleFactor);
			if(multiColumn != null)
				multiColumn.Scale(scaleFactor);
		}
	}
	class TocDocumentBand : SubreportDocumentBand {
		public override bool HasDataSource {
			get { return true; }
		}
		public TocDocumentBand(RectangleF rect)
			: base(rect) {
		}
		protected TocDocumentBand(TocDocumentBand source, int rowIndex)
			: base(source, rowIndex) {
		}
		public override DocumentBand GetInstance(int rowIndex) {
			return new TocDocumentBand(this, rowIndex);
		}
	}
	public class BrickDocumentBand : SubreportDocumentBand {
		#region inner classes
		class BrickDocumentBandLayoutData : SubreportDocumentBandLayoutData {
			VisualBrick brick;
			public BrickDocumentBandLayoutData(BrickDocumentBand docBand, LayoutDataContext context, RectangleF initialRect)
				: base(docBand, context, initialRect) {
				this.brick = docBand.brick;
			}
			public override void UpdateViewBounds() {
				if (brick == null)
					return;
				ILayoutData layoutData = brick.CreateLayoutData(GraphicsDpi.Document);
				if (layoutData != null) {
					layoutData.UpdateViewBounds();
					Bottom = Top + (layoutData.Bottom - layoutData.Top);
				}
			}
			public override void FillPage() {
				context.FillPage(this);
			}
			public override void Anchor(float delta, float dpi) {
				if (brick == null || !VisualBrickHelper.GetCanOverflow(brick))
					return;
				ILayoutData layoutData = brick.CreateLayoutData(dpi);
				if (layoutData == null)
					return;
				layoutData.Anchor(delta, dpi);
			}
		}
		#endregion
		protected VisualBrick brick;
		protected RectangleF brickRect = RectangleF.Empty;
		public BrickDocumentBand(RectangleF rect, VisualBrick brick)
			: base(rect) {
			this.brick = brick;
		}
		protected BrickDocumentBand(BrickDocumentBand source, int rowIndex)
			: base(source, rowIndex) {
			this.brick = source.brick;
			this.brickRect = source.brickRect;
		}
		public override void Reset(bool shouldResetBricksOffset, bool pageBreaksActiveStatus) {
			base.Reset(shouldResetBricksOffset, pageBreaksActiveStatus);
			if (brick != null && brickRect != RectangleF.Empty)
				VisualBrickHelper.SetBrickInitialRect(brick, brickRect);
		}
		public override ILayoutData CreateLayoutData(LayoutDataContext context, RectangleF bounds) {
			if (this.brick != null && brickRect == RectangleF.Empty)
				brickRect = VisualBrickHelper.GetBrickInitialRect(this.brick);
			return CreateLayoutDataCore(context, bounds);
		}
		protected virtual ILayoutData CreateLayoutDataCore(LayoutDataContext context, RectangleF bounds) {
			return new BrickDocumentBandLayoutData(this, context, bounds);
		}
		public override DocumentBand GetInstance(int rowIndex) {
			return new BrickDocumentBand(this, rowIndex);
		}
		public override void Scale(double ratio, Predicate<DocumentBand> shouldScale) {
			base.Scale(ratio, shouldScale);
			brickRect = MathMethods.Scale(brickRect, ratio);
		}
	}
	public class CrossBandBrickDocumentBand : BrickDocumentBand {
		#region inner classes
		class CrossBandBrickDocumentBandLayoutData : SubreportDocumentBandLayoutData {
			VisualBrick brick;
			public CrossBandBrickDocumentBandLayoutData(CrossBandBrickDocumentBand docBand, LayoutDataContext context, RectangleF initialRect)
				: base(docBand, context, initialRect) {
				this.brick = docBand.brick;
			}
			public override void UpdateViewBounds() {
				if (brick == null)
					return;
				if (brick.Height + context.StartNegativeOffsetY > 0)
					return;
				ILayoutData layoutData = brick.CreateLayoutData(GraphicsDpi.Document);
				if (layoutData != null)
					layoutData.Anchor(InitialRect.Height, GraphicsDpi.Document);
			}
			public override void FillPage() {
				context.FillPage(this);
			}
			public override void Anchor(float delta, float dpi) {
				if (brick == null || !VisualBrickHelper.GetCanOverflow(brick))
					return;
				ILayoutData layoutData = brick.CreateLayoutData(dpi);
				if (layoutData == null)
					return;
				layoutData.Anchor(delta, dpi);
			}
		}
		#endregion
		public CrossBandBrickDocumentBand(RectangleF rect, VisualBrick brick)
			: base(rect, brick) {
		}
		protected CrossBandBrickDocumentBand(CrossBandBrickDocumentBand source, int rowIndex)
			: base(source, rowIndex) {
		}
		public override DocumentBand GetInstance(int rowIndex) {
			return new CrossBandBrickDocumentBand(this, rowIndex);
		}
		protected override ILayoutData CreateLayoutDataCore(LayoutDataContext context, RectangleF bounds) {
			return new CrossBandBrickDocumentBandLayoutData(this, context, brickRect);
		}
	}
}
