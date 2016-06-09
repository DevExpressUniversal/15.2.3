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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Export.Rtf;
using System.Drawing;
using DevExpress.XtraPrinting.Export;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraPrinting.BrickExporters;
namespace DevExpress.XtraPrinting.Native {
	public class RtfExportContext : ExportContext {
		RtfExportHelper rtfExportHelper;
		public virtual bool IsPageByPage { get { return false; } }
		public RtfExportHelper RtfExportHelper { get { return rtfExportHelper; } }
		public RtfExportContext(PrintingSystemBase ps, RtfExportHelper rtfExportHelper)
			: base(ps) {
			if(rtfExportHelper == null)
				throw new ArgumentNullException("rtfExportHelper");
			this.rtfExportHelper = rtfExportHelper;
		}
		public override BrickViewData[] GetData(Brick brick, RectangleF rect, RectangleF clipRect) {
			BrickExporter exporter = BrickBaseExporter.GetExporter(this, brick) as BrickExporter;
			return exporter.GetRtfData(this, rect, clipRect);
		}
	}
	public class RtfPageExportContext : RtfExportContext {
		public override bool IsPageByPage { get { return true; } }
		public RtfPageExportContext(PrintingSystemBase ps, RtfExportHelper rtfExportHelper)
			: base(ps, rtfExportHelper) {
		}
	}
}
