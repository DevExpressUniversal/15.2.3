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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Diagram.Core;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraDiagram.Extensions;
using DevExpress.XtraDiagram.Paint;
using DevExpress.XtraDiagram.ViewInfo;
using DevExpress.XtraToolbox;
namespace DevExpress.XtraDiagram.Toolbox {
	public class ToolboxUtils {
		public static Image CreateToolboxItemGlyph(ShapeDescription desc, Size glyphSize) {
			Image img = null;
			using(DiagramDefaultAppearances appearance = new DiagramDefaultAppearances(UserLookAndFeel.Default)) {
				img = CreateShapeImage(GetToolboxShapePainter(desc), desc, BorderThickness, appearance.CreateToolboxShapeDefaultAppearance());
			}
			return img;
		}
		static readonly int BorderThickness = 3;
		public static ToolboxItem CreateToolboxItem(string caption, ShapeDescription shape, bool beginGroup) {
			ToolboxItem item = new ToolboxItem(shape.Name);
			item.Tag = shape;
			item.BeginGroup = beginGroup;
			item.BeginGroupCaption = beginGroup ? caption : string.Empty;
			return item;
		}
		static ToolboxDiagramShapePainter GetToolboxShapePainter(ShapeDescription desc) {
			Size size = desc.DefaultSize.ToWinSize();
			if(size.Width != 0 && (size.Height / size.Width >= 4)) {
				return new ToolboxHighDiagramShapePainter();
			}
			return new ToolboxDiagramShapePainter();
		}
		static Image CreateShapeImage(DiagramShapePainter shapePainter, ShapeDescription shapeDesc, int borderSize, AppearanceDefault appDefault) {
			Size size = shapeDesc.DefaultSize.ToWinSize();
			Rectangle bounds = new Rectangle(Point.Empty, size);
			Image img = new Bitmap(size.Width + borderSize, size.Height + borderSize);
			img.DrawUsingGraphics(cache => {
				cache.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				DiagramShape shape = new DiagramShape(shapeDesc, bounds);
				DiagramShapeInfo shapeInfo = new DiagramShapeInfo(shape);
				shapeInfo.PaintAppearance = new DiagramAppearanceObject(appDefault);
				shapeInfo.SetBounds(bounds);
				ShapeDraw.DrawShape(cache, shapeInfo, shapePainter, bounds);
			});
			return img;
		}
	}
}
