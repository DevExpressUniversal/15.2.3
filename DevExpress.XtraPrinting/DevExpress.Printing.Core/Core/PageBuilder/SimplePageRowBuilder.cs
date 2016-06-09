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
using System.Text;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.Native {
	public class SimplePageRowBuilder : PageRowBuilderBase {
		Page currentPage;
		protected PrintingSystemBase PrintingSystem { get; private set; }
		public Dictionary<Brick, RectangleF> PageBricks { get; private set; }
		public DocumentBand StartBand { get; private set; }
		public SimplePageRowBuilder(PrintingSystemBase ps, Page page)
			: base() {
			PageBricks = new Dictionary<Brick, RectangleF>();
			PrintingSystem = ps;
			currentPage = page;
		}
		protected override void IncreaseBuildInfo(DocumentBand rootBand, int bi, int value) {
			if(PrintingSystem.PrintingDocument.Root.Completed)
				PrintingSystem.ProgressReflector.IncrementRangeValue(value);
			base.IncreaseBuildInfo(rootBand, bi, value);
		}
		protected override void AfterDocumentBandFill(DocumentBand docBand) {
			if(RenderHistory.IsDocumentBandRendered(docBand)) {
				if(currentPage != null)
					PrintingSystem.PerformIfNotNull<GroupingManager>(groupingManager => groupingManager.AfterBandPrinted(currentPage.ID, docBand));
				if(docBand.Bricks.Count > 0)
					PrintingSystem.OnAfterBandPrint(new PageEventArgs(currentPage, new DocumentBand[] { docBand }));
			}
		}
		protected override bool CanFillPageWithBricks(DocumentBand docBand) {
			return docBand == this.StartBand || base.CanFillPageWithBricks(docBand);
		}
		public void FillPageBricks(DocumentBand docBand, RectangleF bounds) {
			DocumentBand rootBand = new ServiceDocumentBand(DocumentBandKind.Storage);
			rootBand.Bands.Add(docBand);
			StartBand = docBand;
			this.FillPageForBand(rootBand, bounds, bounds);
		}
		protected override PageUpdateData UpdatePageContent(DocumentBand docBand, RectangleF bounds) {
			foreach(Brick brick in docBand.Bricks) {
				brick.PageBuilderOffset = new PointD(Offset.X, bounds.Y + Offset.Y).ToPointF();
				PageBricks.Add(brick, brick.Rect);
			}
			return new PageUpdateData(bounds, true);
		}
		protected internal override PageRowBuilderBase CreateInternalPageRowBuilder() {
			return this;
		}
		public override void CopyFrom(PageRowBuilderBase source) {
			base.CopyFrom(source);
			this.PageBricks = ((SimplePageRowBuilder)source).PageBricks;
		}
	}
}
