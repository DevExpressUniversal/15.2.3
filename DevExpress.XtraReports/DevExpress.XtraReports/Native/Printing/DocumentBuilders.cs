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
using DevExpress.XtraPrinting.Native;
using System.Drawing;
namespace DevExpress.XtraReports.Native.Printing {
	public class DetailReportDocumentBuilder : DocumentBuilder {
		public DetailReportDocumentBuilder(XtraReportBase report, DocumentBand docBand)
			: base(report, docBand) {
		}
		protected override bool ShouldCreatePageHeader {
			get { return false; }
		}
		protected override bool ShouldCreatePageFooter {
			get { return false; }
		}
	}
	public class RootReportBuilder : DocumentBuilder {
		public RootReportBuilder(XtraReport report, DocumentBand docBand) : base(report, docBand) {
		}
		public override void Build() {
			CreateDocumentBand(BandKind.TopMargin, RowIndex);
			base.Build();
			CreateDocumentBand(BandKind.BottomMargin, RowIndex);
		}
	}
	public class SubreportBuilder : DocumentBuilder {
		bool autoGenerate;
		BuildInfoContainer buildInfoContainer = new BuildInfoContainer();
		public SubreportBuilder(XtraReportBase report, DocumentBand docBand, bool autoGenerate)
			: base(report, docBand) {
			this.autoGenerate = autoGenerate;
		}
		protected override void BuildCore() {
			if(autoGenerate) 
				RenderBand(RootBand);
		}
		void RenderBand(DocumentBand rootBand) {
			int bi = buildInfoContainer.GetBuildInfo(rootBand);
			DocumentBand docBand = rootBand.Bands[bi];
			while(docBand != null && rootBand.BandManager != null) {
				docBand = rootBand.BandManager.GetBand(rootBand, new PageBuildInfo(bi, RectangleF.Empty, PointF.Empty));
				if(docBand == null)
					return;
				if(docBand.HasBands(RectangleF.Empty, PointF.Empty))
					RenderBand(docBand);
				bi++;
				buildInfoContainer.SetBuildInfo(rootBand, bi);
			}
		}
	}
}
