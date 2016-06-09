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

using System.Collections.Generic;
using System.IO;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting.Export.Imaging {
	public class ImageSinglePageDocumentBuilder : ImageDocumentBuilderBase, IBrickExportVisitor {
		List<BrickWithOffset> bricks;
		internal override protected ImageGraphicsFactory ImageGraphicsFactory { get { return ImageGraphicsFactory.OnePageImageGraphicsFactory; } }
		protected override float DocumentWidth { get { return DocInfo.RightOfBricks; } }
		protected override float DocumentHeight { get { return DocInfo.BottomOfBricks; } }
		public ImageSinglePageDocumentBuilder(PrintingSystemBase ps, ImageExportOptions options)
			: base(ps, options) {
		}
		public override void CreateDocument(Stream stream) {
			Ps.ProgressReflector.SetProgressRanges(new float[] { 1, 1 });
			bricks = new List<BrickWithOffset>();
			Ps.ProgressReflector.EnsureRangeDecrement(() => Ps.Document.GetContinuousExportInfo().ExecuteExport(this, Ps));
			DocInfo.Update(bricks);
			CreatePicture(stream);
		}
		internal protected override void DrawDocument(IGraphics gr) {
			Ps.ProgressReflector.InitializeRange(bricks.Count);
			foreach(BrickWithOffset brick in bricks) {
				BrickBaseExporter.GetExporter(gr, brick.Brick).Draw(gr, brick.Rect, RectangleF.Empty);
				Ps.ProgressReflector.RangeValue++;
			}
		}
		internal protected override void FlushDocument() {
			Ps.ProgressReflector.MaximizeRange();
		}
		#region IBrickExportVisitor Members
		void IBrickExportVisitor.ExportBrick(double horizontalOffset, double verticalOffset, Brick brick) {
			BrickWithOffset brickWithOffset = new BrickWithOffset(brick, (float)horizontalOffset, (float)verticalOffset);
			bricks.Add(brickWithOffset);
		}
		#endregion
	}
}
