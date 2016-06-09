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

using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.LayoutUI;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.Snap.Native.HoverDecorators {
	public enum RenderingUnit { LayoutUnits, Pixels };
	public abstract class SnapSupportsPhysicalBoundsHotZonePainter : SnapHotZonePainter {
		readonly ILayoutToPhysicalBoundsConverter converter;
		protected SnapSupportsPhysicalBoundsHotZonePainter(Painter painter, ILayoutToPhysicalBoundsConverter converter)
			: base(painter) {
			Guard.ArgumentNotNull(converter, "converter");
			this.converter = converter;
		}
		protected ILayoutToPhysicalBoundsConverter Converter { get { return converter; } }
		public abstract RenderingUnit RenderingMode { get; }
	}
	public class DropFieldTableHotZonePainter : SnapSupportsPhysicalBoundsHotZonePainter {
		const int halfLineWidth = 2;
		public DropFieldTableHotZonePainter(Painter painter, ILayoutToPhysicalBoundsConverter converter)
			: base(painter, converter) {
		}
		public override RenderingUnit RenderingMode { get { return RenderingUnit.Pixels; } }
		public override void DrawHotZone(HotZone hotZone) {
			DropFieldTableHotZoneBase tableHotZone = (DropFieldTableHotZoneBase)hotZone;
			Rectangle renderBounds = tableHotZone.Bounds;
			if (RenderingMode == RenderingUnit.Pixels)
				renderBounds = Converter.GetPixelPhysicalBounds(renderBounds);
			if (renderBounds.Width == 0)
				renderBounds.Inflate(halfLineWidth, 0);
			if (renderBounds.Height == 0)
				renderBounds.Inflate(0, halfLineWidth);
			WithHighQualityPixelOffsetMode(Painter, () =>
				Painter.FillRectangle(BackgroundHoverBrush, renderBounds)
			);
		}
	}
}
