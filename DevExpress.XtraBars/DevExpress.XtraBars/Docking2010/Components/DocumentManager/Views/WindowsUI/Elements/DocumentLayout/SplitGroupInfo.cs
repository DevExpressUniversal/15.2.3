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
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ISplitGroupInfo :
		IDocumentGroupInfo<SplitGroup> {
		IEnumerable<ISplitterInfo> SplitterInfos { get; }
	}
	class SplitGroupInfo : DocumentGroupInfo<SplitGroup>, ISplitGroupInfo {
		IList<ISplitterInfo> splitterInfos;
		public SplitGroupInfo(WindowsUIView view, SplitGroup group)
			: base(view, group) {
			splitterInfos = new List<ISplitterInfo>();
		}
		public override System.Type GetUIElementKey() {
			return typeof(ISplitGroupInfo);
		}
		protected override void OnBeforeDocumentInfoRegistered(IDocumentInfo info) {
			if(DocumentInfos.Count > 0) {
				ISplitterInfo splitterInfo = CreateSplitterInfo();
				Register(splitterInfo);
			}
		}
		protected override void OnBeforeDocumentInfoUnRegistered(IDocumentInfo info) {
			if(SplitterInfos.Count > 0) {
				ISplitterInfo splitterInfo = SplitterInfos[SplitterInfos.Count - 1];
				Unregister(splitterInfo);
			}
		}
		protected void Register(ISplitterInfo splitterInfo) {
			splitterInfo.SetGroupInfo(this);
			LayoutHelper.Register(this, splitterInfo);
			splitterInfo.SplitIndex = SplitterInfos.Count;
			SplitterInfos.Add(splitterInfo);
		}
		protected void Unregister(ISplitterInfo splitterInfo) {
			if(SplitterInfos.Remove(splitterInfo)) {
				LayoutHelper.Unregister(this, splitterInfo);
				splitterInfo.SetGroupInfo(null);
				Ref.Dispose(ref splitterInfo);
			}
		}
		protected virtual ISplitterInfo CreateSplitterInfo() {
			return new SplitterInfo(Owner);
		}
		protected internal new SplitGroupInfoPainter Painter {
			get { return base.Painter as SplitGroupInfoPainter; }
		}
		protected override void CalcContent(Graphics g, Rectangle content) {
			int[] intervals = new int[SplitterInfos.Count];
			for(int i = 0; i < intervals.Length; i++)
				intervals[i] = SplitterInfos[i].CalcMinSize(g);
			Document[] documents = Group.Items.ToArray();
			int documentCounter = documents.Length;
			int intervalCounter = intervals.Length;
			int[] splitLengths = Group.GetLengths();
			Rectangle splitContent = CalcContentWithMargins(content);
			while(!CalcDocuments(g, splitContent, intervals, documents, splitLengths)) {
				splitLengths[--documentCounter] = -1;
				intervals[--intervalCounter] = 0;
			}
		}
		protected IList<ISplitterInfo> SplitterInfos {
			get { return splitterInfos; }
		}
		IEnumerable<ISplitterInfo> ISplitGroupInfo.SplitterInfos {
			get { return splitterInfos; }
		}
		bool CalcDocuments(Graphics g, Rectangle bounds, int[] intervals, Document[] documents, int[] splitLengths) {
			int visibleDocumentsCount = 0;
			for(int i = 0; i < splitLengths.Length; i++) {
				if(splitLengths[i] != -1) visibleDocumentsCount++;
			}
			bool horz = Group.IsHorizontal;
			Rectangle[] splitters;
			Rectangle[] rects = SplitLayoutHelper.Calc(bounds, splitLengths, intervals, horz, out splitters);
			ISplitterInfo info;
			for(int i = 0; i < splitters.Length; i++) {
				info = SplitterInfos[i];
				info.Calc(g, splitters[i]);
				info.SplitLength1 = horz ? rects[i].Width : rects[i].Height;
				info.SplitLength2 = horz ? rects[i + 1].Width : rects[i + 1].Height;
			}
			bool allVisible = true;
			for(int i = 0; i < documents.Length; i++) {
				if(splitLengths[i] == -1) continue;
				IDocumentInfo documentInfo;
				if(DocumentInfos.TryGetValue(documents[i], out documentInfo)) {
					documentInfo.Calc(g, rects[i]);
					allVisible &= documentInfo.IsVisible;
				}
			}
			return allVisible || visibleDocumentsCount < 2;
		}
	}
	class SplitGroupInfoPainter : ContentContainerInfoPainter {
		protected override void DrawContent(GraphicsCache cache, IContentContainerInfo info) {
			ISplitGroupInfo groupInfo = info as ISplitGroupInfo;
			foreach(ISplitterInfo splitter in groupInfo.SplitterInfos)
				splitter.Draw(cache);
		}
	}
	class SplitGroupInfoSkinPainter : SplitGroupInfoPainter {
		ISkinProvider providerCore;
		public SplitGroupInfoSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		public override Padding GetContentMargins() {
			SkinElement splitGroup = GetSplitGroupElement();
			if(splitGroup != null) {
				var edges = splitGroup.ContentMargins;
				return new Padding(edges.Left, edges.Top, edges.Right, edges.Bottom);
			}
			return base.GetContentMargins();
		}
		protected virtual SkinElement GetSplitGroupElement() {
			return GetSkin()[MetroUISkins.SkinSplitGroup];
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(providerCore);
		}
	}
	class SlideGroupHeaderInfoPainter : ContentContainerHeaderInfoPainter { }
	class SlideGroupHeaderInfoSkinPainter : ContentContainerHeaderInfoSkinPainter {
		public SlideGroupHeaderInfoSkinPainter(ISkinProvider provider)
			: base(provider) {
		}
		protected override SkinElement GetItemHeader() {
			return GetSkin()[MetroUISkins.SkinSlideGroupItemHeader];
		}
	}
}
