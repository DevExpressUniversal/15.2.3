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
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using System.Drawing;
using DevExpress.XtraReports.Native.Presenters;
namespace DevExpress.XtraReports.Native.LayoutView {
	class LayoutViewSubreportPresenter : DesignSubreportPresenter {
		XtraReport Report {
			get { return Subreport.ReportSource; }
		}
		public LayoutViewSubreportPresenter(XRSubreport subreport)
			: base(subreport) {
		}
		public override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			if(Report.Pages.Count == 0)
				Report.CreateLayoutViewDocument(BrickPresentation.Runtime);
			if(Report.Pages.Count > 0) {
				PanelBrick panelBrick = new FakedPanelBrick(Subreport);
				FillPanelBrick(panelBrick, ((PSPage)Report.PrintingSystem.Pages[0]).Bricks); 
				return panelBrick;
			}
			return base.CreateBrick(childrenBricks);
		}
		public override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			BrickStyle style = new BrickStyle(brick.Style);
			style.Padding = PaddingInfo.Empty;
			LayoutViewAppearance.ApplyContour(style);
			brick.Style = style;
			brick.SetAttachedValue(BrickAttachedProperties.DetailPath, GetDetailPath());
		}
	}
}
