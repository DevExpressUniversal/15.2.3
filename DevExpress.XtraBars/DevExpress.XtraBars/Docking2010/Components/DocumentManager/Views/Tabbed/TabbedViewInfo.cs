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
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Docking2010.Views.Tabbed {
	public class TabbedViewInfo : BaseViewInfo {
		IDictionary<Document, IDocumentInfo> documentInfosCore;
		IDictionary<DocumentGroup, IDocumentGroupInfo> documentGroupInfosCore;
		IList<ISplitterInfo> splitterInfos;
#if DEBUGTEST
		IList<IResizeAssistentInfo> resizeAssistentInfos;
#endif
		public TabbedViewInfo(TabbedView view)
			: base(view) {
			splitterInfos = new List<ISplitterInfo>();
#if DEBUGTEST
			resizeAssistentInfos = new List<IResizeAssistentInfo>();
#endif
			documentInfosCore = new Dictionary<Document, IDocumentInfo>();
			documentGroupInfosCore = new Dictionary<DocumentGroup, IDocumentGroupInfo>();
		}
		protected TabbedView TabbedView {
			get { return View as TabbedView; }
		}
		protected IList<ISplitterInfo> SplitterInfos {
			get { return splitterInfos; }
		}
#if DEBUGTEST
		protected internal IList<IResizeAssistentInfo> ResizeAssistentInfos {
			get { return resizeAssistentInfos; }
		}
#endif
		protected IDictionary<Document, IDocumentInfo> DocumentInfos {
			get { return documentInfosCore; }
		}
		protected IDictionary<DocumentGroup, IDocumentGroupInfo> DocumentGroupInfos {
			get { return documentGroupInfosCore; }
		}
		public IEnumerable<IDocumentGroupInfo> GetGroupInfos() {
			return DocumentGroupInfos.Values;
		}
		public IEnumerable<ISplitterInfo> GetSplitterInfos() {
			return splitterInfos;
		}
		protected override IDockingAdornerInfo CreateEmptyViewAdornerInfo() {
			return new EmptyViewDockingAdornerInfo(TabbedView);
		}
#if DEBUGTEST
		public IResizeAssistentInfo RegisterResizeAssistent() {
			IResizeAssistentInfo resizeAssistent = CreateResizeAssistentInfo();
			RegisterInfo(resizeAssistent);
			return resizeAssistent;
		}
		public void UnregisterResizeAssistent(IResizeAssistentInfo resizeAssistentInfo) {
			UnregisterInfo(resizeAssistentInfo);
		}
		protected virtual IResizeAssistentInfo CreateResizeAssistentInfo(){
			return new ResizeAssistentInfo(View as TabbedView);
		}
#endif
		public ISplitterInfo RegisterSplitter() {
			ISplitterInfo splitter = CreateSplitterInfo();
			RegisterInfo(splitter);
			return splitter;
		}
		public void UnregisterSplitter(ISplitterInfo splitter) {
			ResetSplitterState(splitter);
			UnregisterInfo(splitter);
		}
		public void RegisterInfo(DocumentGroup group) {
			IDocumentGroupInfo info = CreateDocumentGroupInfo(group);
			group.SetInfo(info);
			DocumentGroupInfos.Add(group, info);
			LayoutHelper.Register(View, info);
		}
		public void UnregisterInfo(DocumentGroup group) {
			IDocumentGroupInfo info;
			if(DocumentGroupInfos.TryGetValue(group, out info)) {
				DocumentGroupInfos.Remove(group);
				LayoutHelper.Unregister(View, info);
				Ref.Dispose(ref info);
			}
			group.SetInfo(null);
		}
		protected virtual void ResetSplitterState(ISplitterInfo splitter) {
			if((splitter.State & DevExpress.Utils.Drawing.ObjectState.Pressed) != 0) {
				View.Manager.CancelDragOperation();
			}
			splitter.State = DevExpress.Utils.Drawing.ObjectState.Normal;
		}
		public void RegisterInfo(Document document) {
			IDocumentInfo info = CreateDocumentInfo(document);
			document.SetInfo(info);
			DocumentInfos.Add(document, info);
		}
		public void RegisterInfo(Document document, DocumentGroup group) {
			IDocumentGroupInfo groupInfo;
			if(DocumentGroupInfos.TryGetValue(group, out groupInfo)) {
				IDocumentInfo info;
				if(DocumentInfos.TryGetValue(document, out info)) {
					info.SetGroupInfo(groupInfo);
					LayoutHelper.Register(groupInfo, info);
				}
			}
		}
		public void UnregisterInfo(Document document, DocumentGroup group) {
			IDocumentGroupInfo groupInfo;
			if(group != null && DocumentGroupInfos.TryGetValue(group, out groupInfo)) {
				IDocumentInfo info;
				if(DocumentInfos.TryGetValue(document, out info))
					LayoutHelper.Unregister(groupInfo, info);
				info.SetGroupInfo(null);
			}
		}
		public void UnregisterInfo(Document document) {
			IDocumentInfo info;
			if(DocumentInfos.TryGetValue(document, out info)) {
				IDocumentGroupInfo groupInfo;
				if(document.Parent != null && DocumentGroupInfos.TryGetValue(document.Parent, out groupInfo))
					LayoutHelper.Unregister(groupInfo, info);
				info.SetGroupInfo(null);
				DocumentInfos.Remove(document);
				Ref.Dispose(ref info);
			}
			document.SetInfo(null);
		}
		protected void RegisterInfo(ISplitterInfo splitter) {
			LayoutHelper.Register(View, splitter);
			splitter.SplitIndex = SplitterInfos.Count;
			SplitterInfos.Add(splitter);
		}
#if DEBUGTEST
		protected void RegisterInfo(IResizeAssistentInfo resizeAssistent) {
			LayoutHelper.Register(View, resizeAssistent);
			ResizeAssistentInfos.Add(resizeAssistent);
		}
		protected void UnregisterInfo(IResizeAssistentInfo resizeAssistent) {
			if(ResizeAssistentInfos.Remove(resizeAssistent)) {
				LayoutHelper.Unregister(View, resizeAssistent);
				Ref.Dispose(ref resizeAssistent);
			}
		}
#endif
		protected void UnregisterInfo(ISplitterInfo splitter) {
			if(SplitterInfos.Remove(splitter)) {
				LayoutHelper.Unregister(View, splitter);
				Ref.Dispose(ref splitter);
			}
		}
		protected virtual ISplitterInfo CreateSplitterInfo() {
#if DEBUGTEST
			return new StickySplitterInfo(TabbedView);
#else
			return new SplitterInfo(TabbedView);
#endif
		}
		protected virtual IDocumentInfo CreateDocumentInfo(Document document) {
			return new DocumentInfo(TabbedView, document);
		}
		protected virtual IDocumentGroupInfo CreateDocumentGroupInfo(DocumentGroup group) {
			return new DocumentGroupInfo(TabbedView, group);
		}
		protected override Rectangle[] CalculateCore(Graphics g, Rectangle bounds) {
			if(bounds.Width < 0 || bounds.Height < 0) return new Rectangle[TabbedView.DocumentGroups.Count];
			DocumentGroup[] groups = TabbedView.DocumentGroups.ToArray();
			if(TabbedView.DocumentGroups.Count == 0) return new Rectangle[TabbedView.DocumentGroups.Count];
			tabMargin = null;
			CalcContainers(bounds, g);
#if DEBUGTEST
			RepopulateResizeAssistents();
#endif
			Rectangle[] result = new Rectangle[TabbedView.DocumentGroups.Count];
			int i = 0;
			foreach(var item in TabbedView.DocumentGroups) {
				if(item.Info != null) {
					result[i] = item.Info.Client;
					if(!tabMargin.HasValue) {
						Rectangle r1 = item.Info.Bounds;
						Rectangle r2 = item.Info.Client;
						tabMargin = new Padding(r2.Left - r1.Left, r2.Top - r1.Top, r1.Right - r2.Right, r1.Bottom - r2.Bottom);
					}
				}
				i++;
			}
			return result;
		}
		protected internal void CalcContainers(Rectangle bounds = default(Rectangle), Graphics g = null, DockingContainer node = null) {
			if(bounds == default(Rectangle))
				bounds = Bounds;
			if(g == null)
				g = TabbedView.BeginMeasure().Graphics;
			if(node == null) {
				node = TabbedView.RootContainer;
				node.LayoutRect = Bounds;
			}
			CalcContainerNodes(node, g, bounds, node.Orientation == Orientation.Horizontal);
			foreach(var item in node.Nodes)
				CalcContainers(item.LayoutRect, g, item);
		}
		protected virtual void CalcContainerNodes(DockingContainer node, Graphics g, Rectangle bounds, bool isHorizontal) {
			int length = isHorizontal ? bounds.Width : bounds.Height;
			int splitLength = node.Splitters.Count > 0 ? node.Splitters[0].CalcMinSize(g) : 0;
			length -= splitLength * node.Splitters.Count;
			Size splitSize = isHorizontal ? new Size(splitLength, bounds.Height) : new Size(bounds.Width, splitLength);
			LayoutGroupLengthHelper.CalcActualGroupLength(length, node.Nodes, false);
			Point offset = bounds.Location;
			int i = 0;
			foreach(var item in node.Nodes) {
				Size layoutSize = isHorizontal ? new Size(item.ActualLength, bounds.Height) : new Size(bounds.Width, item.ActualLength);
				item.LayoutRect = new Rectangle(offset, layoutSize);
				item.CaclLayout(g, item.LayoutRect);
				if(isHorizontal)
					offset.X += item.ActualLength;
				else
					offset.Y += item.ActualLength;
				if(node.Splitters.Count > 0 && i < node.Nodes.Count - 1) {
					var splitterInfo = node.Splitters[i];
					splitterInfo.Calc(g, new Rectangle(offset, splitSize));
					i++;
					if(isHorizontal)
						offset.X += splitLength;
					else
						offset.Y += splitLength;
				}
			}
		}
#if DEBUGTEST
		void RepopulateResizeAssistents() {
			ClearResizeAssistentInfosCollection();
			IList<FourAdjacentContainersInfo> fourAdjacentContainersInfos = FourAdjacentContainersInfo.FindFourAdjacentContainers(TabbedView.RootContainer);
			foreach(var fourAdjacentContainersInfo in fourAdjacentContainersInfos) {
				foreach(var splitter in SplitterInfos) {
					Rectangle crossRect = fourAdjacentContainersInfo.CrossRect;
					crossRect.Inflate(-2, -2);
					if(splitter.Bounds.IntersectsWith(crossRect)) {
						SplitterInfo splitterInfo = splitter as SplitterInfo;
						var resizeAssistentInfo = RegisterResizeAssistent() as ResizeAssistentInfo;
						resizeAssistentInfo.Orientation = splitterInfo.IsHorizontal ? Orientation.Horizontal : Orientation.Vertical;
						resizeAssistentInfo.SplittableContainers = new[] { splitterInfo.Node1, splitterInfo.Node2 };
						resizeAssistentInfo.FourAdjacentContainersInfo = fourAdjacentContainersInfo;
						break;
					}
				}
			}
		}
		void ClearResizeAssistentInfosCollection() {
			while(ResizeAssistentInfos.Count != 0) {
				UnregisterResizeAssistent(ResizeAssistentInfos[0] as ResizeAssistentInfo);
			}
		}
#endif
		protected internal override Rectangle GetBounds(BaseDocument document) {
			IDocumentInfo info;
			if(DocumentInfos.TryGetValue((Document)document, out info)) {
				IDocumentGroupInfo groupInfo = info.GroupInfo;
				if(groupInfo == null)
					groupInfo = System.Linq.Enumerable.FirstOrDefault(DocumentGroupInfos.Values);
				if(groupInfo != null) {
					var tab = groupInfo.Tab;
					if((tab != null) && (tab.ViewInfo != null) && !tab.ViewInfo.PageClientBounds.IsEmpty)
						return tab.ViewInfo.PageClientBounds;
				}
			}
			Padding m = tabMargin.HasValue ? tabMargin.Value : new Padding(0);
			return new Rectangle(Bounds.Left + m.Left, Bounds.Top + m.Top, Bounds.Width - m.Horizontal, Bounds.Height - m.Vertical);
		}
		protected internal override Rectangle GetFloatingBounds(Rectangle bounds) {
			Padding m = tabMargin.HasValue ? tabMargin.Value : new Padding(0);
			return new Rectangle(bounds.Left - m.Left, bounds.Top - m.Top, m.Horizontal + bounds.Width, m.Vertical + bounds.Height);
		}
		protected internal override Point GetFloatLocation(BaseDocument document) {
			IDocumentInfo info;
			if(DocumentInfos.TryGetValue((Document)document, out info)) {
				DevExpress.XtraTab.IXtraTab tab = info.GroupInfo.Tab;
				return tab.ViewInfo.PageClientBounds.Location;
			}
			return Point.Empty;
		}
		Padding? tabMargin;
	}
}
