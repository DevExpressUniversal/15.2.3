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
using DevExpress.XtraPrinting;
using System.Collections;
using System.Drawing;
namespace DevExpress.XtraReports.Native.Presenters {
	public class ControlPresenter {
		protected static void FillPanelBrick(PanelBrick panelBrick, IList bricks) {
			XRControl source = (XRControl)panelBrick.BrickOwner;
			RectangleF bounds = new RectangleF(PointF.Empty, XRConvert.Convert(source.Size, source.Dpi, GraphicsDpi.Document));
			foreach(Brick brick in bricks) {
				if(bounds.IntersectsWith(brick.Rect))
					panelBrick.Bricks.Add(brick);
			}
		}
		protected XRControl control;
		public ControlPresenter(XRControl control) {
			this.control = control;
		}
		public virtual VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			throw new NotImplementedException();
		}
		public virtual void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
		}
		public virtual void BeforeReportPrint() { 
		}
		protected string GetDetailPath() {
			StringBuilder path = new StringBuilder();
			GetPath(control.RealControl.Report, path);
			return path.ToString();
		}
		static void GetPath(XtraReportBase report, StringBuilder path) {
			if(report == null)
				return;
			GetPath((XtraReportBase)report.Parent, path);
			DetailBand band = report.Bands[BandKind.Detail] as DetailBand;
			if(band == null)
				return;
			if(path.Length > 0)
				path.Append("-");
			path.Append(band.PrintedCount);
		}
	}
}
