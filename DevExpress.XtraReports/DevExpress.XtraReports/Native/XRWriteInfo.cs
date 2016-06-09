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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Export.Web;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Native {
	public class XRWriteInfo {
		DocumentBand docBand;
		Dictionary<object, VisualBrick> bricksHT;
		PrintingSystemBase ps;
		RectangleF bounds;
		PointF offset;
		Dictionary<XRTableCell, object> mergedCells = new Dictionary<XRTableCell,object>();
		public bool IsSecondary { get; set; }
		public BrickGraphics Graphics { get { return ps.Graph; } }
		public PrintingSystemBase PrintingSystem { get { return ps; } }
		public DocumentBand DocBand { get { return docBand; } }
		public RectangleF Bounds { get { return bounds; } }
		public PointF Offset { get { return offset; } }
		public Dictionary<XRTableCell, object> MergedCells { get { return mergedCells; } }
		public XRWriteInfo(PrintingSystemBase ps)
			: this(ps, GraphicsDpi.HundredthsOfAnInch, null, PageBuildInfo.Empty) {
		}
		internal XRWriteInfo(PrintingSystemBase ps, float dpi, DocumentBand docBand, PageBuildInfo pageBuildInfo) {
			if(ps == null)
				throw new ArgumentNullException();
			this.offset = XRConvert.Convert(pageBuildInfo.Offset, GraphicsDpi.Document, dpi);
			this.bounds = XRConvert.Convert(pageBuildInfo.Bounds, GraphicsDpi.Document, dpi);
			this.ps = ps;
			this.docBand = docBand;
		}
		public void InsertPageBreak(float position) {
			docBand.PageBreaks.Add(new PageBreakInfo(position));
		}
		void RegisterSingleBrick(VisualBrick brick) {
			if(brick.BrickOwner != NullBrickOwner.Instance && !bricksHT.ContainsKey(brick.BrickOwner))
				bricksHT.Add(brick.BrickOwner, brick);
			foreach(VisualBrick childBrick in brick.Bricks)
				RegisterSingleBrick(childBrick);
		}
		public void RegisterSecondaryBricks(IEnumerable<Brick> bricks) {
			bricksHT = new Dictionary<object, VisualBrick>();
			foreach(Brick brick in bricks) {
				if(brick is VisualBrick)
					RegisterSingleBrick((VisualBrick)brick);
			}
		}
		public VisualBrick GetSecondaryBrick(XRControl control) {
			if(bricksHT == null) return null;
			VisualBrick brick;
			return bricksHT.TryGetValue(control, out brick) ? brick : null;
		}
	}
}
