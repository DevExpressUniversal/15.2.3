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
using System.Collections;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native.Data;
using System.Drawing.Printing;
using DevExpress.XtraPrinting;
using System.Drawing;
namespace DevExpress.XtraReports.Native.Printing {
	public abstract class DetailWriterBase {
		#region inner classes
		class GroupBandInfo {
			public XRGroup Group { get; private set; }
			public DocumentBand HeaderBand { get; private set; }
			public GroupBandInfo(XRGroup group, DocumentBand headerBand) {
				HeaderBand = headerBand;
				Group = group;
			}
		}
		#endregion
		#region static
		public static DetailWriterBase CreateInstance(XtraReportBase report, DocumentBand docBand) {
			DetailBand band = report.Bands[BandKind.Detail] as DetailBand;
			if(band != null && report.DataSource != null) {
				if(report.Groups.Count > 0)
					return new DetailWriterWithGroups(report, docBand);
				return new DetailWriterWithDS(report, docBand);
			}
			return new DetailWriter(report, docBand);
		}
		static Band GetBand(DocumentBand docBand) {
			return docBand is SelfGeneratedDocumentBand ?
				((SelfGeneratedDocumentBand)docBand).Band : null;
		}
		static bool KeepTogetherWithDetailReports(SelfGeneratedDocumentBand docBand) {
			return docBand != null && docBand.Band is DetailBand && ((DetailBand)docBand.Band).KeepTogetherWithDetailReports;
		}
		static void DecomposeDocumentBand(DocumentBand rootBand, DocumentBand detailDocumentBand) {
			DocumentBand docBand = new DetailDocumentBand(detailDocumentBand.RowIndex);
			foreach(Brick item in detailDocumentBand.Bricks)
				docBand.Bricks.Add(item);
			detailDocumentBand.Bricks.Clear();
			foreach(PageBreakInfo pageBreak in detailDocumentBand.PageBreaks)
				docBand.PageBreaks.Add(pageBreak);
			detailDocumentBand.PageBreaks.Clear();
			detailDocumentBand.Bands.Add(docBand);
			docBand.BottomSpan = detailDocumentBand.BottomSpan;
			docBand.TopSpan = detailDocumentBand.TopSpan;
			docBand.KeepTogether = detailDocumentBand.KeepTogether;
			detailDocumentBand.BottomSpan = 0;
			detailDocumentBand.TopSpan = 0;
			detailDocumentBand.KeepTogether = false;
		}
		#endregion
		List<GroupBandInfo> groupInfos = new List<GroupBandInfo>();
		protected DocumentBand root;
		bool detailWasWritten;
		bool shouldWriteGroupHeaders = true;
		int groupIndex;
		protected XtraReportBase fReport;
		protected DetailBand detailBand;
		HashSet<DocumentBand> processedGroups = new HashSet<DocumentBand>();
		bool shouldWriteGroupFooters = true;
		bool emptyDetailsWereWritten;
		PrintingSystemBase printingSystem;
		public virtual int RowIndex {
			get { return fReport.DataBrowser.Position; }
		}
		ProgressReflector ProgressReflector {
			get {
				return printingSystem.ProgressReflector;
			}
		}
		protected XRGroupCollection Groups { get { return fReport.Groups; } }
		bool CanWrite {
			get { return detailBand != null; }
		}
		protected DetailWriterBase(XtraReportBase report, DocumentBand root) {
			if(report == null)
				throw new ArgumentException();
			this.fReport = report;
			this.root = root;
			detailBand = fReport.Bands[BandKind.Detail] as DetailBand;
			printingSystem = report.RootReport.GetCreatingPrintingSystem();
		}
		void SetHeaderFriend(GroupBandInfo info) {
			DocumentBand header = info.HeaderBand;
			XRGroup group = info.Group;
			if(header.Parent == null || group.Header == null)
				return;
			int index = groupInfos.IndexOf(info);
			if(index < 0) return;
			if(group.Header.GroupUnion == GroupUnion.WithFirstDetail) {
				XRGroup innerGroup = index - 1 >= 0 ? groupInfos[index - 1].Group : null;
				if(innerGroup != null && innerGroup.Header != null && innerGroup.Header.GroupUnion == GroupUnion.WholePage) {
					header.FriendLevel = 0;
				} else {
					header.FriendLevel = group.Header.Level;
				}
			} else if(group.Header.GroupUnion == GroupUnion.WholePage) {
				header.Parent.KeepTogetherOnTheWholePage = CanKeepTogetherOnTheWholePage(header);
				header.FriendLevel = group.Header.Level;
			}
		}
		void SetFooterFriend(GroupBandInfo groupBandInfo) {
			GroupFooterUnion groupFooterUnion = groupBandInfo.Group.Footer != null ? groupBandInfo.Group.Footer.GroupUnion : GroupFooterUnion.None;
			if(groupBandInfo.HeaderBand == null || groupBandInfo.HeaderBand.Parent == null) 
				return;
			if(groupFooterUnion == GroupFooterUnion.WithLastDetail) {
				DocumentBand docLastDetailBand = GetLastDetail(groupBandInfo.HeaderBand.Parent);
				if(docLastDetailBand == null)
					return;
				if(groupBandInfo.Group.Footer != null && groupBandInfo.Group.Footer.RepeatEveryPage)
					docLastDetailBand.KeepTogether = true;
				else if(groupBandInfo.Group.Header != null)
					docLastDetailBand.FriendLevel = groupBandInfo.Group.Header.Level;
				else if(groupBandInfo.Group.Header == null)
					docLastDetailBand.FriendLevel = groupBandInfo.Group.Footer.Level;
			}
		}
		DocumentBand GetLastDetail(DocumentBand docBand) {
			if(GetBand(docBand) is DetailBand)
				return docBand;
			for(int i = docBand.Bands.Count - 1; i >= 0; i--) {
				if(GetBand(docBand.Bands[i]) is GroupFooterBand)
					continue;
				DocumentBand result = GetLastDetail(docBand.Bands[i]);
				if(result != null)
					return result;
			}
			return null;
		}
		DocumentBand GetParentBand(GroupBandInfo groupInfo) {
			return groupInfo.HeaderBand.Parent ?? this.root;
		}
		DocumentBand CreateGroupFooterBand(GroupBandInfo groupInfo) {
			DocumentBand docFooterBand = GetParentBand(groupInfo).GetBand(DocumentBandKind.Footer);
			if(docFooterBand == null) {
				Band item = groupInfo.Group.Footer;
				docFooterBand = item != null ? item.CreateDocumentBand(printingSystem, fReport.DataBrowser.Position, PageBuildInfo.Empty) :
					BandKind.GroupFooter.ToEmptyDocumentBand();
				DocumentBand parentBand = GetParentBand(groupInfo);
				parentBand.Bands.Add(docFooterBand);
				processedGroups.Add(parentBand);
			}
			SetFooterFriend(groupInfo);
			fReport.OnGroupFinished(groupInfo.Group);
			return docFooterBand;
		}
		public bool ShouldWriteGroupFooters(DocumentBand rootBand) {
			return processedGroups.Contains(rootBand) && shouldWriteGroupFooters;
		}
		public void Write(DocumentBand rootBand, PageBuildInfo pageBuildInfo) {
			if(processedGroups.Contains(rootBand)) {
				if(shouldWriteGroupFooters) {
					WriteGroupFooters(groupIndex);
					shouldWriteGroupFooters = false;
				}
				return;
			}
			if(shouldWriteGroupHeaders && detailWasWritten) {
				MoveNextRow();
				if(groupIndex >= 0) {
					WriteGroupHeaders(rootBand, pageBuildInfo.Index, groupIndex);
					shouldWriteGroupHeaders = false;
					return;
				}
			} else if(shouldWriteGroupHeaders && Groups.Count - 1 >= 0) {
				WriteGroupHeaders(rootBand, pageBuildInfo.Index, Groups.Count - 1);
				shouldWriteGroupHeaders = false;
				return;
			}
			WriteDetailCore(pageBuildInfo);
			groupIndex = FindNextGroupIndex();
			if(groupIndex >= 0) {
				for(int i = 0; i <= groupIndex; i++)
					SetFooterFriend(groupInfos[i]);
				shouldWriteGroupFooters = true;
				processedGroups.Add(GetRootBand(pageBuildInfo.Index));
			}
		}
		void WriteGroupFooters(int finishIndex) {
			for(int i = 0; i <= finishIndex; i++) {
				CreateGroupFooterBand(groupInfos[0]);
				groupInfos.RemoveAt(0);
			}
		}
		void WriteGroupHeaders(DocumentBand rootBand, int index, int groupStartIndex) {
			List<GroupBandInfo> infos = new List<GroupBandInfo>();
			for(int i = groupStartIndex; i >= 0; i--) {
				XRGroup xrGroup = Groups[i];
				fReport.OnGroupBegin(xrGroup);
				DocumentBandContainer groupParent = new DocumentBandContainer(i == 0) { GroupKey = xrGroup.Header };
				DocumentBand rootBand2 = GetRootBand(index);
				if(rootBand != rootBand2) {
					rootBand2.Bands.Insert(groupParent, GetDetailIndex(rootBand2));
				} else {
					rootBand.Bands.Insert(groupParent, index);
				}
				groupParent.BandManager = groupParent.Parent.BandManager;
				DocumentBand headerBand = xrGroup.Header != null ? xrGroup.Header.CreateDocumentBand(printingSystem, fReport.DataBrowser.Position, PageBuildInfo.Empty) :
					BandKind.GroupHeader.ToEmptyDocumentBand();
				groupParent.Bands.Add(headerBand);
				GroupBandInfo groupBandInfo = new GroupBandInfo(xrGroup, headerBand);
				groupInfos.Insert(0, groupBandInfo);
				infos.Add(groupBandInfo);
			}
			foreach(GroupBandInfo info in infos)
				SetHeaderFriend(info);
		}
		bool CanKeepTogetherOnTheWholePage(DocumentBand headerBand) {
			for(int i = groupInfos.Count - 1; i >= 0; i--) {
				GroupBandInfo item = groupInfos[i];
				if(ReferenceEquals(item.HeaderBand, headerBand)) break;
				if(item.HeaderBand == null) continue;
				if(item.HeaderBand.RowIndex != headerBand.RowIndex)
					return true;
				else if(item.HeaderBand.IsFriendLevelSet)
					return false;
			}
			return true;
		}
		public DocumentBand GetPageFooterBand(DocumentBand band) {
			if(groupInfos.Count == 0)
				return null;
			GroupFooterBand footer = GetFooterReportBand(band);
			DocumentBand footerDocBand = footer != null && footer.RepeatEveryPage ?
				footer.CreateDocumentBand(printingSystem, fReport.DataBrowser.Position, PageBuildInfo.Empty) :
				null;
			if(footerDocBand != null)
				band.Bands.Add(footerDocBand);
			return footerDocBand;
		}
		GroupFooterBand GetFooterReportBand(DocumentBand docBand) {
			foreach(GroupBandInfo groupInfo in groupInfos) {
				if(groupInfo.HeaderBand.Parent == docBand)
					return groupInfo.Group.Footer;
			}
			return null;
		}
		public DocumentBand GetFooterBand(DocumentBand band) {
			if(groupInfos.Count == 0)
				return null;
			GroupFooterBand footer = GetFooterReportBand(band);
			DocumentBand footerDocBand = footer != null ? footer.CreateDocumentBand(printingSystem, fReport.DataBrowser.Position, PageBuildInfo.Empty) :
				null;
			if(footerDocBand != null)
				band.Bands.Add(footerDocBand);
			return footerDocBand;
		}
		void WriteDetailCore(PageBuildInfo pageBuildInfo) {
			DocumentBand rootBand = GetRootBand(pageBuildInfo.Index);
			if(!emptyDetailsWereWritten && fReport.ReportPrintOptions.BlankDetailCount > 0) {
				int detailIndex = GetDetailIndex(rootBand);
				for(int i = 0; i < fReport.ReportPrintOptions.BlankDetailCount; i++) {
					DocumentBand blankBand = CreateBlankDocumentBand();
					rootBand.Bands.Insert(blankBand, detailIndex + i);
				}
				emptyDetailsWereWritten = true;
			}
			detailWasWritten = true;
			shouldWriteGroupHeaders = true;
			DocumentBand detailDocumentBand = detailBand.CreateDocumentBand(printingSystem, RowIndex, fReport.DataBrowser.Count, pageBuildInfo);
			rootBand.Bands.Insert(detailDocumentBand, GetDetailIndex(rootBand));
			if(fReport.Bands[BandKind.DetailReport] != null) {
				if(detailDocumentBand.Bands.Count == 0)
					DecomposeDocumentBand(rootBand, detailDocumentBand);
				detailDocumentBand.BandManager = new DetailReportBandManager(fReport);
			}
			if(KeepTogetherWithDetailReports(detailDocumentBand as SelfGeneratedDocumentBand))
				detailDocumentBand.KeepTogether = true;
			XRProgressReflectorLogic logic;
			object accessor;
			if(TryGetLogicAndAccessor(out logic, out accessor))
				logic.SetRangeValue(accessor, logic.RangeValue + 1);
		}
		DocumentBand CreateBlankDocumentBand() {
			DocumentBand blankBand = new EmptyDetailDocumentBand(BandKind.Detail.ToDocumentBandKind());
			EmptyBrick emptyBrick = new EmptyBrick();
			RectangleF rect = new RectangleF(PointF.Empty, detailBand.ClientRectangleF.Size);
			if(detailBand.MultiColumn.Mode != MultiColumnMode.None)
				rect.Width = detailBand.MultiColumn.GetUsefulColumnWidth(detailBand.ClientRectangleF.Width, detailBand.Dpi);
			emptyBrick.SetBounds(rect, detailBand.Dpi);
			VisualBrickHelper.InitializeBrick(emptyBrick, printingSystem, emptyBrick.Rect);
			blankBand.Bricks.Add(emptyBrick);
			return blankBand;
		}
		protected virtual int FindNextGroupIndex() {
			return EndOfData() ? Groups.Count - 1 : -1;
		}
		public static int GetDetailIndex(DocumentBand rootBand) {
			for(int i = rootBand.Bands.Count - 1; i >= 0; i--) {
				if(rootBand.Bands[i].IsKindOf(DocumentBandKind.Detail, DocumentBandKind.Header, DocumentBandKind.ReportHeader))
					return i + 1;
			}
			return 0;
		}
		DocumentBand GetRootBand(int index) {
			if(groupInfos != null && groupInfos.Count > 0) {
				GroupBandInfo groupBandInfo = groupInfos[0];
				return groupBandInfo.HeaderBand.Parent ?? GetContainer(index);
			}
			return GetContainer(index);
		}
		protected virtual DocumentBand GetContainer(int index) {
			return this.root;
		}
		protected abstract void MoveNextRow();
		protected abstract bool EndOfDataCore();
		protected abstract int GetRowCount();
		public bool EndOfData() {
			return !CanWrite || (EndOfDataCore() && detailWasWritten && shouldWriteGroupHeaders);
		}
		public void BeforeWrite() {
			fReport.DataBrowser.Position = 0;
			XRProgressReflectorLogic logic;
			object accessor;
			if(CanWrite && TryGetLogicAndAccessor(out logic, out accessor))
				logic.InitializeRange(accessor, GetRowCount());
		}
		bool TryGetLogicAndAccessor(out XRProgressReflectorLogic logic, out object accessor) {
			accessor = null;
			logic = this.ProgressReflector.Logic as XRProgressReflectorLogic;
			if(logic == null) return false;
			ISupportProgress progressSupporter = root as ISupportProgress;
			if(progressSupporter != null)
				accessor = progressSupporter.ProgressAccessor;
			else if(root.IsRoot)
				accessor = fReport;
			return true;
		}
		public void AfterWrite() {
			XRProgressReflectorLogic logic;
			object accessor;
			if(CanWrite && TryGetLogicAndAccessor(out logic, out accessor))
				logic.MaximizeRange(accessor);
			if(root.IsRoot) {
				while(ProgressReflector.RangeCount > 1) {
					ProgressReflector.InitializeRange(1);
					ProgressReflector.MaximizeRange();
				}
			}
		}
	}
}
