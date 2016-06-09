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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Native;
using DevExpress.DocumentView;
namespace DevExpress.XtraReports.Native.TOC {
	public interface IContentsReportGenerator {
		XtraReport GenerateContentsReport(ContentsReportGenerationContext generationContext);
	}
	class ContentsReportGenerator : IContentsReportGenerator {
		readonly XRControlFactory xrControlFactory = new XRControlFactory();
		ContentsReportGenerationContext generationContext;
		public ContentsReportGenerator() {
		}
		public ContentsReportGenerator(ContentsReportGenerationContext generationContext) {
			if(generationContext == null)
				throw new ArgumentNullException();
			this.generationContext = generationContext;
		}
		public XtraReport GenerateContentsReport(ContentsReportGenerationContext generationContext) {
			this.generationContext = generationContext;
			return GenerateContentsReport();
		}
		public XtraReport GenerateContentsReport() {
			Guard.ArgumentNotNull(generationContext, "generationContext");
			Guard.ArgumentNotNull(generationContext.Bookmarks, "bookmarks");
			Guard.ArgumentNotNull(generationContext.SourceReport, "sourceReport");
			xrControlFactory.Dpi = generationContext.SourceReport.Dpi;
			var contentsReport = xrControlFactory.CreateXtraReport(generationContext.Bookmarks);
			AddTitleLine(contentsReport);
			if(generationContext.Bookmarks.Count == 0) {
				AddNoBookmarksWereFoundMessageLine(contentsReport);
			} else {
				AddLines(contentsReport);
			}
			return contentsReport;
		}
		void AddTitleLine(XtraReport contentsReport) {
			XRLabel titleLine = xrControlFactory.CreateXRLabel(generationContext.TitleLevel.Text, generationContext.GetLevelStyle(generationContext.TitleLevel), generationContext.TableOfContentsWidth, new PointF(0f, 0f));
			titleLine.HeightF = generationContext.TitleLevel.Height;
			var reportHeader = xrControlFactory.CreateReportHeaderBand(titleLine);
			contentsReport.Bands.Add(reportHeader);
		}
		void AddNoBookmarksWereFoundMessageLine(XtraReport contentsReport) {
			string messageText = ReportLocalizer.GetString(ReportStringId.Msg_NoBookmarksWereFoundInReportForXrToc);
			var parent = contentsReport.Bands[BandKind.ReportHeader];
			float messageLineY = parent.Controls.Count == 0 ? 0f : parent.Controls.Cast<XRControl>().Max(control => control.BottomF);
			XRLabel messageLine = xrControlFactory.CreateXRLabel(messageText, generationContext.DefaultStyle, generationContext.TableOfContentsWidth, new PointF(0f, messageLineY));
			parent.Controls.Add(messageLine);
		}
		void AddLines(XtraReport contentsReport) {
			int actualDepth = 0;
			XRTableOfContentsLevel level = generationContext.GetLevel(actualDepth);
			var actualIndent = generationContext.GetIndent(actualDepth);
			var detailBandWithTocLine = CreateDetailBandWithTocLine(contentsReport.DataSource, string.Empty, level, generationContext.TableOfContentsWidth - actualIndent, actualIndent);
			contentsReport.Bands.Add(detailBandWithTocLine);
			int reachedDepth = 0;
			var parentsByLevel = new Dictionary<int, XtraReportBase>();
			parentsByLevel[actualDepth] = contentsReport;
			foreach(BookmarkNode bookmark in generationContext.Bookmarks)
				VisitChildren(ref reachedDepth, actualDepth, parentsByLevel, bookmark.Nodes, contentsReport.DataSource, "Nodes", generationContext);
		}
		DetailBand CreateDetailBandWithTocLine(object dataSource, string dataMember, XRTableOfContentsLevel level, float width, float x) {
			var style = generationContext.GetLevelStyle(level);
			XRTableOfContentsLine tocLine = xrControlFactory.XRTableOfContentsLine(dataSource, dataMember, width, style, new PointF(x, 0f), level.LeaderSymbol);
			tocLine.HeightF = level.Height;
			return xrControlFactory.CreateDetailBand(tocLine);
		}
		void VisitChildren(ref int reachedDepth, int actualDepth, Dictionary<int, XtraReportBase> parentsByLevel, IList bookmarks, object dataSource, string dataMember, ContentsReportGenerationContext generationContext) {
			actualDepth++;
			foreach(BookmarkNode bookmark in bookmarks) {
				if(actualDepth > reachedDepth && actualDepth < generationContext.MaxNestingLevel) {
					reachedDepth++;
					XRTableOfContentsLevel level = generationContext.GetLevel(actualDepth);
					var actualIndent = generationContext.GetIndent(actualDepth);
					var detailBandWithTocLine = CreateDetailBandWithTocLine(dataSource, dataMember, level, generationContext.TableOfContentsWidth - actualIndent, actualIndent);
					var detailReportBand = xrControlFactory.CreateDetailReportBand(dataSource, dataMember, detailBandWithTocLine);
					var parent = parentsByLevel[actualDepth - 1];
					parent.Bands.Add(detailReportBand);
					parentsByLevel[actualDepth] = detailReportBand;
				}
				VisitChildren(ref reachedDepth, actualDepth, parentsByLevel, bookmark.Nodes, dataSource, dataMember + ".Nodes", generationContext);
			}
		}
	}
	public class ContentsReportGenerationContext {
		Func<XRTableOfContentsLevelBase, XRControlStyle> getLevelStyle;
		XRTableOfContentsLevel defaultLevel;
		XRTableOfContentsTitle titleLevel;
		int maxNestingLevel = 0;
		public XRTableOfContentsLevel DefaultLevel {
			get {
				if(defaultLevel == null)
					defaultLevel = new XRTableOfContentsLevel();
				return defaultLevel;
			}
			set {
				defaultLevel = value;
			}
		}
		public XRTableOfContentsTitle TitleLevel {
			get {
				if(titleLevel == null)
					titleLevel = new XRTableOfContentsTitle();
				return titleLevel;
			}
			set {
				titleLevel = value;
			}
		}
		public IList<XRTableOfContentsLevel> Levels { get; set; }
		public IBookmarkNodeCollection Bookmarks { get; set; }
		public XtraReport SourceReport { get; set; }
		public float DefaultStepIndent { get; set; }
		public XRControlStyle DefaultStyle { get; set; }
		public float TableOfContentsWidth { get; set; }
		public int MaxNestingLevel {
			get { return maxNestingLevel == 0 ? Int32.MaxValue : maxNestingLevel; }
			set { maxNestingLevel = value; }
		}
		public ContentsReportGenerationContext(Func<XRTableOfContentsLevelBase, XRControlStyle> getLevelStyle) {
			this.getLevelStyle = getLevelStyle;
			DefaultStyle = XRControlStyle.Default;
		}
		public ContentsReportGenerationContext()
			: this(null) {
		}
		public XRTableOfContentsLevel GetLevel(int depth) {
			if(Levels != null && depth < Levels.Count)
				return Levels[depth];
			return DefaultLevel;
		}
		public XRControlStyle GetLevelStyle(XRTableOfContentsLevelBase level) {
			return getLevelStyle != null ? getLevelStyle(level) : DefaultStyle;
		}
		public float GetIndent(int depth) {
			XRTableOfContentsLevel level = GetLevel(depth);
			return level.GetIndent(depth);
		}
	}
	class DocumentProxy : IDocumentProxy {
		List<Page> pages = new List<Page>();
		public List<Page> Pages { get { return pages; } }
		public DocumentProxy() {
		}
		void IDocumentProxy.AddPage(PSPage page) {
			pages.Add(page);
		}
		string IDocumentProxy.InfoString {
			get { return NativeSR.InfoString; }
		}
		int IDocumentProxy.PageCount {
			get { return pages.Count; }
		}
		bool IDocumentProxy.SmartXDivision {
			get { return false; }
		}
		bool IDocumentProxy.SmartYDivision {
			get { return false; }
		}
	}
}
