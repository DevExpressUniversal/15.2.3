#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.Export {
	public class DashboardExportReport : XtraReport {
		class DashboardExportTopMarginBand : TopMarginBand {
			public XRWriteInfo WriteInfo { get { return writeInfo; } }
		}
		class DashboardExportBottomMarginBand : BottomMarginBand {
			public XRWriteInfo WriteInfo { get { return writeInfo; } }
		}
		readonly DashboardExportTopMarginBand topMarginBand = new DashboardExportTopMarginBand();
		readonly DashboardExportBottomMarginBand bottomMarginBand = new DashboardExportBottomMarginBand();
		readonly PageHeaderFooter pageHF = new PageHeaderFooter();
		readonly List<Image> images = new List<Image>();
		public PageHeaderFooter PageHeaderFooter { get { return pageHF; } }
		public IList Images { get { return images; } }
		public DashboardExportReport() {
			topMarginBand.AfterPrint += (s, e) => {
				AddMarginBricks(topMarginBand.WriteInfo, pageHF.Header);
			};
			bottomMarginBand.AfterPrint += (s, e) => {
				AddMarginBricks(bottomMarginBand.WriteInfo, pageHF.Footer);
			};
			this.Bands.Add(topMarginBand);
			this.Bands.Add(bottomMarginBand);
		}
		void AddMarginBricks(XRWriteInfo writeInfo, PageArea area) {
			AddBricks(writeInfo, area.GetBricks(PrintingSystem.Graph, images.ToArray()), BrickModifier.MarginalHeader);
		}
		void AddBricks(XRWriteInfo writeInfo, IList<Brick> bricks, BrickModifier modifier) {
			foreach(Brick brick in bricks) {
				brick.Initialize(PrintingSystem, brick.Rect);
				brick.Modifier = modifier;
				writeInfo.DocBand.Bricks.Add(brick);
			}
		}
	}
}
