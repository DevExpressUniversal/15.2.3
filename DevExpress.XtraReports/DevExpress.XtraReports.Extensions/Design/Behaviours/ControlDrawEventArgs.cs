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
using System.Text;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using System.Drawing;
namespace DevExpress.XtraReports.Design.Behaviours {
	public class ControlDrawEventArgs : EventArgs {
		bool shouldDrawReportExplorerImage;
		DrawEventArgs e;
		Graphics graph;
		public RectangleF Bounds {
			get { return e.Bounds; }
		}
		public VisualBrick Brick {
			get { return e.Brick; }
		}
		public bool ShouldDrawReportExplorerImage {
			get { return shouldDrawReportExplorerImage; }
		}
		public Graphics Graph {
			get {
				if(graph == null) {
					GdiGraphics gdiGraph = e.UniGraphics as GdiGraphics;
					if(gdiGraph != null)
						graph = gdiGraph.Graphics;
				}
				return graph;
			}
		}
		public ControlDrawEventArgs(DrawEventArgs e, bool shouldDrawReportExplorerImage) {
			this.e = e;
			this.shouldDrawReportExplorerImage = shouldDrawReportExplorerImage;
		}
	}
}
