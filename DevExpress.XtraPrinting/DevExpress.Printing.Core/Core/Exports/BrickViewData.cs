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
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Export {
	[BrickExporter(typeof(NullTableCellExporter))]
	public sealed class NullTableCell : ITableCell {
		public static readonly ITableCell Instance = new NullTableCell();
		bool ITableCell.ShouldApplyPadding { get { return false; } }
		string ITableCell.FormatString { get { return null; } }
		string ITableCell.XlsxFormatString { get { return null; } }
		object ITableCell.TextValue { get { return null; } }
		BrickModifier ITableCell.Modifier { get { return BrickModifier.None; } }
		DefaultBoolean ITableCell.XlsExportNativeFormat { get { return DefaultBoolean.Default; } }
		string ITableCell.Url { get { return null; } }
		NullTableCell() {
		}
	}
	public class NullTableCellExporter : BrickExporter {
		protected internal override void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
		}
		protected internal override void FillRtfTableCellInternal(IRtfExportProvider exportProvider) {
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
		}
	}
	public class BrickViewData : ILayoutControl {
		ITableCell tableCell;
		RectangleF bounds = RectangleF.Empty;
		BrickStyle style = null;
		int offsetYMult;
		int OffsetY { get { return offsetYMult << 13; }  }
		public int Left { get { return System.Convert.ToInt32(bounds.Left); } }
		public int Top { get { return System.Convert.ToInt32(bounds.Top) + OffsetY; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickViewDataBounds")]
#endif
		public Rectangle Bounds { 
			get {
				Rectangle result = GraphicsUnitConverter.Round(bounds);
				result.Offset(0, OffsetY);
				return result;
			} 
			set { 
				bounds = value;
				bounds.Offset(0, -(OffsetY));
			} 
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickViewDataBoundsF")]
#endif
		public RectangleF BoundsF {
			get {
				RectangleF result = bounds;
				result.Offset(0, OffsetY);
				return result;
			}
			protected set {
				bounds = value;
				bounds.Offset(0, -(OffsetY));
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickViewDataWidth")]
#endif
		public int Width { get { return Bounds.Width; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickViewDataHeight")]
#endif
		public int Height { get { return Bounds.Height; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickViewDataStyle")]
#endif
		public BrickStyle Style { get { return style; } set { style = value; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickViewDataTableCell")]
#endif
		public ITableCell TableCell {
			get { return tableCell != null ? tableCell : NullTableCell.Instance; }
			set { tableCell = value; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickViewDataOriginalBounds")]
#endif
		public virtual Rectangle OriginalBounds { get { return GraphicsUnitConverter.Round(this.OriginalBoundsF); } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("BrickViewDataOriginalBoundsF")]
#endif
		public virtual RectangleF OriginalBoundsF { get { return BoundsF; } }
		public BrickViewData(BrickStyle style, RectangleF bounds, ITableCell tableCell) {
			this.tableCell = tableCell;
			this.bounds = bounds;
			this.style = style;
		}
		public virtual void ApplyClipping(Rectangle clipBounds, bool round) {
		}
		internal void SetOffesetY(int offsetY) {
			this.offsetYMult = offsetY;
		}
	}
	public class PageBrickViewData : BrickViewData {
		RectangleF originalBounds = RectangleF.Empty;
		public override RectangleF OriginalBoundsF {
			get { return originalBounds == RectangleF.Empty ? BoundsF : originalBounds; }
		}
		public PageBrickViewData(BrickStyle style, RectangleF bounds, ITableCell tableCell)
			: base(style, bounds, tableCell) {
		}
		public override void ApplyClipping(Rectangle clipBounds, bool round) {
			originalBounds = BoundsF;
			if(round)
				Bounds = Rectangle.Intersect(Bounds, clipBounds);
			else 
				BoundsF = RectangleF.Intersect(BoundsF, clipBounds);
		}
	}
}
