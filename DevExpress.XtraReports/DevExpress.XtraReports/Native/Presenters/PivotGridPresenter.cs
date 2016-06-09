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
using System.Linq;
using System.Text;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.Drawing;
namespace DevExpress.XtraReports.Native.Presenters {
	class PivotGridPresenter : ControlPresenter {
		public PivotGridPresenter(XRPivotGrid pivotGrid) : base(pivotGrid) { 
		}
		public override VisualBrick CreateBrick(DevExpress.XtraPrinting.VisualBrick[] childrenBricks) {
			return new SubreportBrick(control);
		}
	}
	class DesignPivotGridPresenter : ControlPresenter {
		protected PrintingSystemBase ps;
		protected LinkBase link;
		public DesignPivotGridPresenter(XRPivotGrid pivotGrid, PrintingSystemBase ps, LinkBase link)
			: base(pivotGrid) {
			this.link = link;
			this.ps = ps;
		}
		public override VisualBrick CreateBrick(DevExpress.XtraPrinting.VisualBrick[] childrenBricks) {
			PanelBrick panelBrick = new FakedPanelBrick(control);
			EnsurePrintingSystemContent();
			if(ps.Pages.Count > 0)
				FillPanelBrick(panelBrick, ((PSPage)ps.Pages[0]).Bricks);
			return panelBrick;
		}
		protected virtual void EnsurePrintingSystemContent() {
			ps.ClearContent();
			link.CreateDocument(ps);
		}
	}
	class FakedPanelBrick : PanelBrick {
		public FakedPanelBrick(IBrickOwner owner)
			: base(owner) {
		}
		public override XtraPrinting.Native.LayoutAdjustment.ILayoutData CreateLayoutData(float dpi) {
			return new DevExpress.XtraPrinting.Native.LayoutAdjustment.BrickLayoutDataBase(this, dpi);
		}
		public override void Dispose() {
			Bricks.Clear();
			base.Dispose();
		}
	}
}
