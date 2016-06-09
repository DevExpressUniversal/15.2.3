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
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.Export;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting.Native {
	public abstract class ExportContext : GraphicsBase {
		public virtual bool RawDataMode {
			get { return false; } 
		}
		public bool CancelPending { get { return PrintingSystem != null && PrintingSystem.CancelPending; } }
		public virtual bool AllowEmptyAreas { get { return false; } }
		protected ExportContext(PrintingSystemBase ps)
			: base(ps) {
		}
		public virtual BrickViewData CreateBrickViewData(BrickStyle style, RectangleF bounds, ITableCell tableCell) {
			if(DrawingPage != null)
				return new PageBrickViewData(style, bounds, tableCell);
			return new BrickViewData(style, bounds, tableCell);
		}
		public BrickViewData[] CreateBrickViewDataArray(BrickStyle style, RectangleF bounds, ITableCell tableCell) {
			return new BrickViewData[] { CreateBrickViewData(style, bounds, tableCell) };
		}
		abstract public BrickViewData[] GetData(Brick brick, RectangleF rect, RectangleF clipRect);
	}
	public class TextExportContext : ExportContext {
		public override bool AllowEmptyAreas { get { return true; } }
		public override bool RawDataMode { get { return true; } }
		public TextExportContext(PrintingSystemBase ps)
			: base(ps) {
		}
		public override BrickViewData CreateBrickViewData(BrickStyle style, RectangleF bounds, ITableCell tableCell) {
			return new TextBrickViewData(style, bounds, tableCell);
		}
		public override BrickViewData[] GetData(Brick brick, RectangleF rect, RectangleF clipRect) {
			BrickExporter exporter = BrickBaseExporter.GetExporter(this, brick) as BrickExporter;
			return exporter.GetTextData(this, rect);
		}
	}
}
