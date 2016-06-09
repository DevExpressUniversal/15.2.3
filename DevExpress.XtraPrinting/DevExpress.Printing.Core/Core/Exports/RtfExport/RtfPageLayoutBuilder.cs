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
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Export {
	public class RtfPageLayoutBuilder : PageLayoutBuilder {
		public RtfPageLayoutBuilder(XtraPrinting.Page page, RtfExportContext rtfExportContext)
			: base(page, rtfExportContext) {
		}
		internal override RectangleF ValidateLayoutRect(Brick brick, RectangleF rect) {
			if(brick is LineBrick) {
				float minimumLineWidth = GraphicsUnitConverter.Convert(2, GraphicsDpi.Pixel, GraphicsDpi.Document);
				rect.Width = Math.Max(rect.Width, minimumLineWidth);
				rect.Height = Math.Max(rect.Height, minimumLineWidth);
			}
			return rect;
		}
		internal override BrickViewData[] GetData(Brick brick, RectangleF bounds, RectangleF clipRect) {
			BrickExporter exporter = (BrickExporter)BrickBaseExporter.GetExporter(exportContext, brick);
			BrickViewData[] data = exporter.GetRtfData((RtfExportContext)exportContext, bounds, clipRect);
			if(data.Length == 2 && data[0].Bounds == data[1].Bounds)
				return new BrickViewData[] { data[1] };
			return data;
		}
		internal override RectangleF GetCorrectClipRect(RectangleF clipRect) {
			return clipRect;
		}
	}
}
