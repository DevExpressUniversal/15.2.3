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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Printing {
	public class CompositeLink : TemplatedLink {
		readonly IEnumerable<TemplatedLink> links;
		readonly PageBreakInfoCollection pageBreaks = new PageBreakInfoCollection();
		public IEnumerable<TemplatedLink> Links { get { return links; } }
		public PageBreakInfoCollection PageBreaks { get { return pageBreaks; } }
		#region ctor
		public CompositeLink(IEnumerable<TemplatedLink> links) : this(links, string.Empty) { }
		public CompositeLink(IEnumerable<TemplatedLink> links, string documentName)
			: base(documentName) {
			this.links = links;
		}
		#endregion
		protected override XtraPrinting.DataNodes.IRootDataNode CreateRootNode() {
			return new CompositeRootNode(this);
		}
	}
	internal class CompositeRootNode : IRootDataNode {
		readonly Dictionary<int, IDataNode> nodesDictionary = new Dictionary<int, IDataNode>();
		readonly int detailCount;
		public CompositeLink Link { get; private set; }
		#region ctor
		public CompositeRootNode(CompositeLink link) {
			Link = link;
			detailCount = link.Links.Count();
		}
		#endregion
		#region IRootDataNode
		public int GetTotalDetailCount() {
			return detailCount;
		}
		#endregion
		#region IDataNode
		public bool CanGetChild(int index) {
			return index >= 0 && index < detailCount;
		}
		public IDataNode GetChild(int index) {
			IDataNode result;
			if(!nodesDictionary.TryGetValue(index, out result)) {
				var link = Link.Links.ElementAt(index);
				link.CreateDocument(false);
				IDataNode node = link.RootNode.GetChild(0);
				PageBreakInfo pageBreaksInfo;
				Link.PageBreaks.TryGetValue(index, out pageBreaksInfo);
				result = new LinkGroupContainer(this, node, index, link.GetReportHeaderRowViewInfo(true), link.GetReportFooterRowViewInfo(true), pageBreaksInfo);
				nodesDictionary[index] = result;
			}
			return result;
		}
		public int Index { get { throw new NotImplementedException(); } }
		public bool IsDetailContainer { get { return false; } }
		public bool PageBreakAfter { get { return false; } }
		public bool PageBreakBefore { get { return false; } }
		public IDataNode Parent { get { return null; } }
		#endregion
	}
	internal class LinkGroupContainer : IVisualGroupNode {
		readonly IDataNode wrapperNode;
		readonly int index;
		readonly RowViewInfo header;
		readonly RowViewInfo footer;
		readonly PageBreakInfo pageBreaksInfo;
		public LinkGroupContainer(CompositeRootNode parent, IDataNode wrapperNode, int index, RowViewInfo headerViewInfo, RowViewInfo footerViewInfo, PageBreakInfo pageBreaksInfo = null) {
			Parent = parent;
			this.wrapperNode = wrapperNode;
			this.index = index;
			this.header = headerViewInfo;
			this.footer = footerViewInfo;
			this.pageBreaksInfo = pageBreaksInfo;
		}
		public RowViewInfo GetFooter(bool allowContentReuse) {
			return footer;
		}
		public RowViewInfo GetHeader(bool allowContentReuse) {
			return header;
		}
		public bool RepeatHeaderEveryPage {
			get { return false; }
		}
		public GroupUnion Union {
			get { return GroupUnion.None; }
		}
		public bool CanGetChild(int index) {
			return index == 0;
		}
		public IDataNode GetChild(int index) {
			return index == 0 ? wrapperNode : null;
		}
		public int Index {
			get { return index; }
		}
		public bool IsDetailContainer {
			get { return wrapperNode is IVisualDetailNode; }
		}
		public bool PageBreakAfter {
			get { return pageBreaksInfo.Return(x=> x.PageBreakAfter, ()=> false); }
		}
		public bool PageBreakBefore {
			get { return pageBreaksInfo.Return(x => x.PageBreakBefore, () => false); }
		}
		public IDataNode Parent {get; private set; }
	}
	public class PageBreakInfo {
		public bool PageBreakAfter { get; set; }
		public bool PageBreakBefore { get; set; }
		public TemplatedLink Link { get; private set; }
		public PageBreakInfo(TemplatedLink link) {
			this.Link = link;
		}
	}
	public class PageBreakInfoCollection : ObservableCollection<PageBreakInfo> {
	}
}
