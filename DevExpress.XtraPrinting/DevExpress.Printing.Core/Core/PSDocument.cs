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
using System.Drawing;
using System.Collections;
using DevExpress.XtraPrinting;
using System.Collections.Generic;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Drawing;
using System.Windows.Media;
using Brush = DevExpress.Xpf.Drawing.Brush;
#else
using System.Windows.Forms;
using DevExpress.XtraPrinting.BrickExporters;
#endif
namespace DevExpress.XtraPrinting.Native {
	static class InformationHelper {
		public static Font Font = new Font("Segoe UI", 8);
		public static Color Color = Color.FromArgb(0xff, 0x5c, 0x5c, 0x5c);
		public static SizeF CalcSize(string infoString, float dpi, Measurer measurer) {
			SizeF infoSize = measurer.MeasureString(infoString, Font, GraphicsUnit.Pixel);
			SizeF rect = GraphicsUnitConverter.Convert(infoSize, GraphicsDpi.Pixel, dpi);
			rect.Width *= 1.2f;
			rect.Height *= 1.5f;
			return rect;
		}
	}
	public class PSDocument : PrintingDocument {
		protected DocumentBand currentDetailContainer;
		Stack<DocumentBand> detailContainers = new Stack<DocumentBand>();
		DocumentBand activeBand;
		public DocumentBand ActiveBand {
			get {
				if(activeBand == null)
					activeBand = CreateDetailBand();
				return this.activeBand;
			}
			protected set {
				if(this.activeBand != null)
					this.activeBand.ApplySpans();
				this.activeBand = value;
			}
		}
		public PSDocument(PrintingSystemBase ps, Action0 invoker)
			: base(ps, null, invoker) {
		}
		protected internal override void Begin() {
			SetRoot(new RootDocumentBand(PrintingSystem));
			ps.Graph.ModifierChanged += new EventHandler(OnModifierChanged);
			base.Begin();
		}
		protected internal override void AfterBuild() {
			base.AfterBuild();
			ps.Graph.ModifierChanged -= new EventHandler(OnModifierChanged);
		}
		protected internal override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (ps.Graph != null)
						ps.Graph.ModifierChanged -= new EventHandler(OnModifierChanged);
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		protected internal override void BeginReport(DocumentBand docBand, PointF offset) {
			this.detailContainers.Push(this.currentDetailContainer);
			if(docBand == null) {
				docBand = new DocumentBandContainer();
				docBand.TopSpan = offset.Y;
				System.Diagnostics.Debug.Assert(this.currentDetailContainer != null);
				this.currentDetailContainer.AddBand(docBand);
			}
			this.currentDetailContainer = docBand;
		}
		protected internal override DocumentBand AddReportContainer() {
			DocumentBand reportContainer = new DocumentBandContainer();
			this.currentDetailContainer.AddBand(reportContainer);
			return reportContainer;
		}
		protected internal override void EndReport() {
			this.currentDetailContainer = detailContainers.Pop();
		}
		protected internal override void AddBrick(Brick brick) {
			if(!IsLocked)
				ActiveBand.Bricks.Add(brick);
		}
		void OnModifierChanged(Object sender, EventArgs e) {
			OnModifierChanged(((BrickGraphics)sender).Modifier);
		}
		protected virtual void OnModifierChanged(BrickModifier modifier) {
			if((modifier & BrickModifier.DetailHeader) > 0) {
				System.Diagnostics.Debug.Assert(this.currentDetailContainer != null);
				DocumentBandContainer detailContainer = new DocumentBandContainer();
				this.currentDetailContainer.AddBand(detailContainer);
				DocumentBand currentHeader = new DocumentBand(DocumentBandKind.Header);
				detailContainer.AddBand(currentHeader);
				currentHeader.RepeatEveryPage = true;
				this.currentDetailContainer = detailContainer;
				ActiveBand = currentHeader;
			} else if((modifier & BrickModifier.DetailFooter) > 0) {
				System.Diagnostics.Debug.Assert(this.currentDetailContainer != null);
				System.Diagnostics.Debug.Assert(this.currentDetailContainer.Parent != null);
				DocumentBand currentFooter = new DocumentBand(DocumentBandKind.Footer);
				this.currentDetailContainer.AddBand(currentFooter);
				currentFooter.RepeatEveryPage = true;
				this.currentDetailContainer = this.currentDetailContainer.Parent;
				ActiveBand = currentFooter;
			} else if((modifier & BrickModifier.Detail) > 0) {
				ActiveBand = CreateDetailBand();
			} else
				ActiveBand = null;
		}
		DocumentBand CreateDetailBand() {
			System.Diagnostics.Debug.Assert(this.currentDetailContainer != null);
			DetailDocumentBand currentDetail = new DetailDocumentBand();
			this.currentDetailContainer.AddBand(currentDetail);
			return currentDetail;
		}
		protected internal override void InsertPageBreak(float pos) {
			System.Diagnostics.Debug.Assert(ActiveBand != null);
			ActiveBand.PageBreaks.Add(new PageBreakInfo(pos));
		}
		protected internal override void InsertPageBreak(float pos, CustomPageData nextPageData) {
			System.Diagnostics.Debug.Assert(ActiveBand != null);
			ActiveBand.PageBreaks.Add(new PageBreakInfo(pos, nextPageData));
		}
		public override void ShowFromNewPage(Brick brick) {
			System.Diagnostics.Debug.Assert(ActiveBand != null);
			ActiveBand.PageBreaks.Add(new PageBreakInfo(brick.Rect.Top));
		}
	}
	public class PSLinkDocument : PSDocument {
		MarginDocumentBand topMargin;
		MarginDocumentBand bottomMargin;
		DocumentBand pageHeader;
		DocumentBand pageFooter;
		DocumentBand reportHeader;
		DocumentBand reportFooter;
		float PageHeaderHeight {
			get {
				return ps.PageSettings.MarginsF.Top - ps.PageSettings.MinMarginsF.Top;
			}
		}
		float PageFooterHeight {
			get {
				return ps.PageSettings.MarginsF.Bottom - ps.PageSettings.MinMarginsF.Bottom;
			}
		}
		public PSLinkDocument(PrintingSystemBase ps, Action0 invoker)
			: base(ps, invoker) {
			VerticalContentSplitting = VerticalContentSplitting.Smart;
		}
		protected internal override void HandleNewPageSettings() {
			System.Diagnostics.Debug.Assert(!IsCreating, "This method can't be called when document is creating");
			if(topMargin != null)
				topMargin.SetHeight(PageHeaderHeight);
			if(bottomMargin != null)
				bottomMargin.SetHeight(PageFooterHeight);
			base.HandleNewPageSettings();
		}
		protected override void OnModifierChanged(BrickModifier modifier) {
			if((modifier & BrickModifier.MarginalHeader) > 0)
				ActiveBand = topMargin;
			else if((modifier & BrickModifier.MarginalFooter) > 0)
				ActiveBand = bottomMargin;
			else if((modifier & BrickModifier.InnerPageHeader) > 0)
				ActiveBand = pageHeader;
			else if((modifier & BrickModifier.InnerPageFooter) > 0)
				ActiveBand = pageFooter;
			else if((modifier & BrickModifier.ReportHeader) > 0) {
				DocumentBand band = GetBand(DocumentBandKind.ReportHeader);
				ActiveBand = band != null ? band : reportHeader;
			} else if((modifier & BrickModifier.ReportFooter) > 0) {
				DocumentBand band = GetBand(DocumentBandKind.ReportFooter);
				ActiveBand = band != null ? band : reportFooter;
			} else
				base.OnModifierChanged(modifier);
		}
		DocumentBand GetBand(DocumentBandKind bandKind) {
			return this.currentDetailContainer != null ? this.currentDetailContainer.Parent.GetBand(bandKind) : null;
		}
		protected internal override void Begin() {
			base.Begin();
			this.currentDetailContainer = new DocumentBandContainer();
			this.Root.AddBand(currentDetailContainer);
			this.topMargin = new MarginDocumentBand(DocumentBandKind.TopMargin, PageHeaderHeight);
			this.bottomMargin = new MarginDocumentBand(DocumentBandKind.BottomMargin, PageFooterHeight);
			this.pageFooter = new DocumentBand(DocumentBandKind.PageFooter);
			this.pageFooter.RepeatEveryPage = true;
			this.pageHeader = new DocumentBand(DocumentBandKind.PageHeader);
			this.pageHeader.RepeatEveryPage = true;		   
			this.reportHeader = new DocumentBand(DocumentBandKind.ReportHeader);
			this.reportFooter = new DocumentBand(DocumentBandKind.ReportFooter);
		}
		protected internal override void End(bool buildPagesInBackground) {
			ActiveBand = null;
			Root.InsertBand(pageHeader, 0);
			Root.InsertBand(reportHeader, 0);
			Root.InsertBand(topMargin, 0);
			Root.AddBand(reportFooter);
			Root.AddBand(pageFooter);
			Root.AddBand(bottomMargin);
			Root.SetParents();
			Root.Completed = true;
			base.End(buildPagesInBackground);
		}
	}
}
