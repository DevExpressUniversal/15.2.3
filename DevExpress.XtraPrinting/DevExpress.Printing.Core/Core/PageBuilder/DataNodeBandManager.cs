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

using System.Collections.Generic;
using DevExpress.XtraPrinting.DataNodes;
namespace DevExpress.XtraPrinting.Native {
	public abstract class DataNodeBandManager : IBandManager {
		#region Fields & Properties
		protected IRootDataNode rootNode;
		protected RootDocumentBand rootBand;
		bool endOfDataWasReached;
		Dictionary<DocumentBand, IDataNode> nodes;
		#endregion // Fields & Properties
		public DataNodeBandManager(IRootDataNode rootNode) {
			this.rootNode = rootNode;
			nodes = new Dictionary<DocumentBand, IDataNode>();
		}
		#region IBandManager Members
		PrintingSystemBase IBandManager.PrintingSystem {
			get { return rootBand.PrintingSystem; }
		}
		bool IBandManager.IsCompleted {
			get { return rootBand.Completed; }
		}
		public void EnsureGroupFooter(DocumentBand documentBand) {
		}
		public void EnsureReportFooterBand(DocumentBand documentBand) {
			if(documentBand == rootBand && documentBand.GetBand(DocumentBandKind.ReportFooter) == null) {
				DocumentBand pageFooterBand = documentBand.GetPageBand(DocumentBandKind.Footer);
				DocumentBand reportFooterBand = new DocumentBand(DocumentBandKind.ReportFooter);
				if(pageFooterBand != null)
					documentBand.Bands.Insert(reportFooterBand, pageFooterBand.Index);
				else
					documentBand.Bands.Add(reportFooterBand);
			}
		}
		public DocumentBand GetBand(DocumentBand parentBand, PageBuildInfo pageBuildInfo) {
			DocumentBand docBand = pageBuildInfo.Index < parentBand.Bands.Count ? parentBand.Bands[pageBuildInfo.Index] : null;
			if(docBand != null && docBand.IsKindOf(DocumentBandKind.Detail, DocumentBandKind.Header, DocumentBandKind.Footer, DocumentBandKind.ReportHeader) && !docBand.IsPageBand(DocumentBandKind.Footer))
				return docBand;
			if(endOfDataWasReached)
				return docBand != null && docBand.IsKindOf(DocumentBandKind.ReportFooter) ? docBand : null;
			if(parentBand is RootDocumentBand || parentBand is ISubreportDocumentBand) {
				DocumentBandContainer rootDetailContainer = new DocumentBandContainer();
				rootDetailContainer.BandManager = this;
				parentBand.Bands.Insert(rootDetailContainer, pageBuildInfo.Index);
				nodes[rootDetailContainer] = rootNode;
				GetRootNodeBand(rootDetailContainer, rootNode, 0);
				return SafeGetBand(parentBand.Bands, pageBuildInfo.Index);
			}
			IDataNode parentNode;
			if(!nodes.TryGetValue(parentBand, out parentNode))
				return null;
			if(parentNode == rootNode) {
				return GetRootNodeBand(parentBand, parentNode, pageBuildInfo.Index);
			}
			return GetNodeBand(parentBand, parentNode, pageBuildInfo.Index);
		}
		DocumentBand GetRootNodeBand(DocumentBand parentBand, IDataNode parentNode, int index) {
			if(!parentNode.IsDetailContainer && parentNode.CanGetChild(index)) {
				InsertDocumentBandContainer(parentBand, parentNode, index, index);
			} else if(parentNode.IsDetailContainer && parentNode.CanGetChild(index)) {
				InsertDetailBand(parentBand, parentNode, index, index);
			} else {
				OnEndOfData();
			}
			return SafeGetBand(parentBand.Bands, index);
		}
		protected virtual void OnEndOfData() {
			endOfDataWasReached = true;
		}
		DocumentBand GetNodeBand(DocumentBand parentBand, IDataNode parentNode, int bandIndex) {
			System.Diagnostics.Debug.Assert(bandIndex != 0);
			int nodeIndex = bandIndex - 1;
			if(!parentNode.IsDetailContainer && parentNode.CanGetChild(nodeIndex)) {
				InsertDocumentBandContainer(parentBand, parentNode, bandIndex, nodeIndex);
			} else if(parentNode.IsDetailContainer && parentNode.CanGetChild(nodeIndex)) {
				InsertDetailBand(parentBand, parentNode, bandIndex, nodeIndex);
			} else if(!parentBand.Bands.GetLast<DocumentBand>().IsKindOf(DocumentBandKind.Footer)) {
				IGroupNode groupNode = (IGroupNode)parentNode;
				DocumentBand groupFooterBand = CreateGroupFooterBand((IGroupNode)parentNode);
				parentBand.Bands.Insert(groupFooterBand, bandIndex);
				if(groupNode.PageBreakAfter)
					groupFooterBand.PageBreaks.Add(new PageBreakInfo(groupFooterBand.SelfHeight));
			}
			return SafeGetBand(parentBand.Bands, bandIndex);
		}
		void InsertDetailBand(DocumentBand parentBand, IDataNode parentNode, int bandIndex, int nodeIndex) {
			IDataNode detailNode = parentNode.GetChild(nodeIndex);
			DocumentBand detailBand = CreateDetailBand(detailNode);
			parentBand.Bands.Insert(detailBand, bandIndex);
			if(detailNode.PageBreakBefore) {
				detailBand.PageBreaks.Add(new PageBreakInfo(0));
			}
			if(detailNode.PageBreakAfter) {
				detailBand.PageBreaks.Add(new PageBreakInfo(detailBand.SelfHeight));
			}
		}
		void InsertDocumentBandContainer(DocumentBand parentBand, IDataNode parentNode, int bandIndex, int nodeIndex) {
			DocumentBandContainer container = new DocumentBandContainer();
			container.BandManager = this;
			parentBand.Bands.Insert(container, bandIndex);
			nodes[container] = parentNode.GetChild(nodeIndex);
			IGroupNode groupNode = (IGroupNode)nodes[container];
			DocumentBand groupHeaderBand = CreateGroupHeaderBand(groupNode);
			container.Bands.Insert(groupHeaderBand, 0);
			groupHeaderBand.RepeatEveryPage = groupNode.RepeatHeaderEveryPage;
			if (groupNode.Union == GroupUnion.WholePage) {
				container.KeepTogether = true;
				container.KeepTogetherOnTheWholePage = true;
			}
			else if (groupNode.Union == GroupUnion.WithFirstDetail)
				groupHeaderBand.FriendLevel = GetDetailLevel(groupNode, 0);
			if(groupNode.PageBreakBefore)
				groupHeaderBand.PageBreaks.Add(new PageBreakInfo(0));
		}
		static int GetDetailLevel(IDataNode dataNode, int level) {
			if(dataNode.IsDetailContainer)
				return level;
			if(dataNode.CanGetChild(0))
				return GetDetailLevel(dataNode.GetChild(0), level + 1);
			return DocumentBand.UndefinedFriendLevel;
		}
		public DocumentBand GetPageFooterBand(DocumentBand band) {
			return band.GetPageBand(DocumentBandKind.Footer);
		}
		#endregion
		public virtual void Initialize(RootDocumentBand rootBand) {
			this.rootBand = rootBand;
			endOfDataWasReached = false;
			nodes.Clear();
		}
		DocumentBand SafeGetBand(IListWrapper<DocumentBand> bands, int index) {
			return index < bands.Count ? bands[index] : null;
		}
		protected abstract DocumentBand CreateDetailBand(IDataNode node);
		protected abstract DocumentBand CreateGroupHeaderBand(IGroupNode node);
		protected abstract DocumentBand CreateGroupFooterBand(IGroupNode node);
	}
}
