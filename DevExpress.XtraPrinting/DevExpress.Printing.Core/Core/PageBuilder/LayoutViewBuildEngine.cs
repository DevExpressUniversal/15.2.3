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
using DevExpress.XtraPrinting.Native;
using System.Drawing;
using DevExpress.XtraPrinting;
using System.Drawing.Printing;
using DevExpress.XtraReports.UI;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Preview;
namespace DevExpress.XtraPrinting.Native.PageBuilder {
	class SinglePageLinkDocument : PSLinkDocument {
		public SinglePageLinkDocument(PrintingSystemBase ps, Action0 afterBuildPages)
			: base(ps, afterBuildPages) {
		}
		protected override PageBuildEngine CreatePageBuildEngine(bool buildPagesInBackground, bool rollPaper) {
			System.Diagnostics.Debug.Assert(!buildPagesInBackground);
			return new SinglePageBuildEngine(this);
		}
	}
	class SinglePageBuildEngine : ContinuousPageBuildEngine {
		class CustomXEngine : XPageContentEngine {
			public override List<PSPage> CreatePages(PSPage source, RectangleF usefulArea, float usefulPageWidth) {
				List<PSPage> pages = new List<PSPage>();
				if(source != null) {
					source.PageData.PageSize = SizeF.Empty;
					Size bricksSize = Size.Round(XRConvert.Convert(source.BricksSize, GraphicsDpi.Document, GraphicsDpi.HundredthsOfAnInch));
					Size pageSize = GetPageSize(source.PageData, bricksSize);
					source.PageData.Landscape = false;
					source.PageData.Size = pageSize;
					source.PageData.PaperKind = PaperKind.Custom;
					source.Rect = new RectangleF(source.Rect.Location, source.BricksSize);
					pages.Add(source);
				}
				return pages;
			}
			Size GetPageSize(ReadonlyPageData pageData, Size bricksSize) {
				const int minHeight = 100;
				int height = Math.Max(minHeight, bricksSize.Height + pageData.Margins.Top + pageData.Margins.Bottom);
				int width = Math.Max(pageData.Landscape ? pageData.Size.Height : pageData.Size.Width, bricksSize.Width + pageData.Margins.Left + pageData.Margins.Right);
				return new Size(width, height);
			}
		}
		public SinglePageBuildEngine(PrintingDocument document)
			: this(document, new CustomXEngine()) {
		}
		protected SinglePageBuildEngine(PrintingDocument document, XPageContentEngine xContentEngine)
			: base(document, xContentEngine, false) {
		}
		protected override void AfterBuildPage(PSPage page, RectangleF usefulPageArea) {
			page.AfterCreate(page.UsefulPageRectF, PrintingSystem);
		}
	}
	class RollPageBuildEngine : SinglePageBuildEngine {
		class CustomXEngine : SimpleXPageContentEngine {
			public override List<PSPage> CreatePages(PSPage source, RectangleF usefulArea, float usefulPageWidth) {
				List<PSPage> pages = base.CreatePages(source, usefulArea, usefulPageWidth);
				if(pages.Count == 0)
					return pages;
				PSPage page = pages[0];
				page.PageData.PageSize = SizeF.Empty;
				Size bricksSize = Size.Round(XRConvert.Convert(source.BricksSize, GraphicsDpi.Document, GraphicsDpi.HundredthsOfAnInch));
				Size pageSize = GetPageSize(page.PageData, bricksSize);
				page.PageData.Landscape = false;
				page.PageData.Size = pageSize;
				page.PageData.PaperKind = PaperKind.Custom;
				page.Rect = new RectangleF(page.Rect.Location, new SizeF(usefulPageWidth, source.BricksSize.Height));
				return new List<PSPage>(new PSPage[] { page });
			}
			Size GetPageSize(ReadonlyPageData pageData, Size bricksSize) {
				const int minHeight = 100;
				int height = Math.Max(minHeight, bricksSize.Height + pageData.Margins.Top + pageData.Margins.Bottom);
				return new Size(pageData.Landscape ? pageData.Size.Height : pageData.Size.Width, height);
			}
		}
		protected override RectangleF ActualUsefulPageRectF {
			get {
				MarginsF margins = PrintingSystem.PageSettings.Data.MarginsF;
				return new RectangleF(margins.Left, margins.Top, PrintingSystem.PageSettings.UsefulPageWidth, float.MaxValue);
			}
		}
		public RollPageBuildEngine(PrintingDocument document)
			: base(document, new CustomXEngine()) {
		}
		protected override void Build() {
			try {
				base.Build();
			} catch(InvalidOperationException) {
			}
		}
		protected override void InitializeContentEngine(YPageContentEngine contentEngine) {
			contentEngine.Stopped.Assign(() => Stopped, null);
			contentEngine.BuildInfoIncreased.Assign(null, _ => {
				IBackgroundService backgroundService = ((IServiceProvider)PrintingSystem).GetService(typeof(IBackgroundService)) as IBackgroundService;
				if(backgroundService != null) backgroundService.PerformAction();
				if(Aborted) throw new InvalidOperationException();
			});
		}
	}
}
