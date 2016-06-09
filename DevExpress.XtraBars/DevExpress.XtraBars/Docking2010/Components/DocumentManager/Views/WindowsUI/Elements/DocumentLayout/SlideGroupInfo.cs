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
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ISlideGroupInfo :
		IDocumentGroupInfo<SlideGroup> {
		ScrollBarInfo ScrollBarInfo { get; }
		Rectangle ClientRect { get; }
		Rectangle ScrollRect { get; }
		int ScrollOffset { get; }
		Rectangle[] Headers { get; }
		void ScrollTo(IDocumentInfo documentInfo);
	}
	class SlideGroupInfo : DocumentGroupInfo<SlideGroup>, ISlideGroupInfo, IScrollBarInfoOwner, IScrollableElementInfo, IInteractiveElementInfo {
		ScrollBarInfo scrollBarInfoCore;
		public SlideGroupInfo(WindowsUIView view, SlideGroup group)
			: base(view, group) {
			this.scrollBarInfoCore = CreateScrollBarInfo();
			ScrollBarInfo.ValueChanged += OnScrollBarInfoValueChanged;
			LayoutHelper.Register(this, (DragEngine.IUIElement)ScrollBarInfo);
		}
		protected override void OnDispose() {
			LayoutHelper.Unregister(this, (DragEngine.IUIElement)ScrollBarInfo);
			ScrollBarInfo.ValueChanged -= OnScrollBarInfoValueChanged;
			Ref.Dispose(ref scrollBarInfoCore);
			base.OnDispose();
		}
		protected virtual ScrollBarInfo CreateScrollBarInfo() {
			return new SlideGroupScrollBarInfo(this);
		}
		int scrollValueChanged = 0;
		void OnScrollBarInfoValueChanged(object sender, System.EventArgs e) {
			if(scrollValueChanged > 0) return;
			scrollValueChanged++;
			using(Owner.LockPainting()) {
				ScrollOffset = -ScrollBarInfo.Value;
				Owner.RequestInvokePatchActiveChild();
			}
			scrollValueChanged--;
		}
		public sealed override Type GetUIElementKey() {
			return typeof(ISlideGroupInfo);
		}
		public ScrollBarInfo ScrollBarInfo {
			get { return scrollBarInfoCore; }
		}
		public ScrollBarVisibility ScrollBarVisibility {
			get { return Group.ScrollBarVisibility; }
		}
		public int ScrollOffset { get; private set; }
		public Rectangle ClientRect { get; private set; }
		public Rectangle ScrollRect { get; private set; }
		public Rectangle[] Headers { get; private set; }
		protected new SlideGroupInfoPainter Painter {
			get { return base.Painter as SlideGroupInfoPainter; }
		}
		public void ScrollTo(IDocumentInfo documentInfo) {
			int value = IsHorizontal ? documentInfo.Bounds.Left : documentInfo.Bounds.Top;
			ScrollBarInfo.Value = Math.Min(ScrollBarInfo.Maximum - ScrollBarInfo.LargeChange,
				Math.Max(0, value - Painter.Interval));
		}
		protected override void CalcContent(Graphics g, Rectangle content) {
			bool horz = Group.IsHorizontal;
			int scrollSize = horz ?
				SystemInformation.HorizontalScrollBarHeight :
				SystemInformation.VerticalScrollBarWidth;
			ScrollRect = Rectangle.Empty;
			ClientRect = content;
			if(ScrollBarInfo.Visible) {
				ClientRect = new Rectangle(content.Left, content.Top,
						horz ? content.Width : content.Width - scrollSize,
						horz ? content.Height - scrollSize : content.Height
					);
				ScrollRect = new Rectangle(
						horz ? content.Left : ClientRect.Right,
						horz ? ClientRect.Bottom : content.Top,
						horz ? content.Width : scrollSize,
						horz ? scrollSize : content.Height
					);
			}
			Document[] documents = Group.Items.ToArray();
			int[] lengths = Group.GetLengths();
			int offset = ScrollOffset;
			Rectangle[] rects = new Rectangle[lengths.Length];
			Headers = new Rectangle[lengths.Length];
			int groupItemLength = Group.Properties.ActualItemLength;
			double itemLengthRatio = Group.Properties.ActualItemLengthRatio;
			if(groupItemLength == 0) {
				if(!Group.Properties.HasItemLengthRatio && Group.Items.Count == 1)
					itemLengthRatio = 1.0;
				groupItemLength = (int)(horz ? ClientRect.Width * itemLengthRatio : ClientRect.Height * itemLengthRatio);
			}
			Rectangle headersRect = ((SlideGroupHeaderInfo)HeaderInfo).HeadersRect;
			Size[] headers = ((SlideGroupHeaderInfo)HeaderInfo).Headers;
			Padding contentMargins;
			Rectangle slideContent = CalcContentWithMargins(ClientRect, out contentMargins);
			int interval = Painter.Interval;
			for(int i = 0; i < documents.Length; i++) {
				if(lengths[i] == -1) continue;
				IDocumentInfo documentInfo;
				if(DocumentInfos.TryGetValue(documents[i], out documentInfo)) {
					int length = lengths[i];
					if(length == 0)
						length = groupItemLength;
					if(i > 0)
						offset += interval;
					rects[i] = new Rectangle(
							horz ? slideContent.Left + offset : slideContent.Left,
							horz ? slideContent.Top + contentMargins.Top : slideContent.Top + offset,
							horz ? length : slideContent.Width,
							horz ? slideContent.Height : length
						);
					if(horz) {
						Headers[i] = new Rectangle(headersRect.Location, headers[i]);
						if(rects[i].Right < headersRect.Right) {
							Headers[i].X = Math.Min(rects[i].Right - headers[i].Width,
								Math.Max(headersRect.Left, rects[i].Left));
						}
						else {
							if(rects[i].Left > headersRect.Left)
								Headers[i].X = rects[i].Left;
						}
					}
					documentInfo.Calc(g, rects[i]);
					offset += length;
				}
			}
			CalcScrollBar(ClientRect, offset + contentMargins.Horizontal - ScrollOffset);
		}
		void CalcScrollBar(Rectangle bounds, int scrollBarMaxValue) {
			ScrollBarInfo.ViewInfo.Reset();
			ScrollBarInfo.Bounds = ScrollRect;
			if(IsHorizontal) {
				ScrollBarInfo.Maximum = scrollBarMaxValue;
				ScrollBarInfo.LargeChange = bounds.Width;
			}
			else {
				ScrollBarInfo.Maximum = scrollBarMaxValue;
				ScrollBarInfo.LargeChange = bounds.Height;
			}
		}
		#region IScrollBarInfoOwner Members
		public Point PointToClient(Point point) {
			return Owner.Manager.ScreenToClient(point);
		}
		public void Invalidate() {
			Owner.Invalidate();
		}
		public bool IsHorizontal {
			get { return Group != null ? Group.IsHorizontal : false; }
		}
		public DevExpress.LookAndFeel.UserLookAndFeel GetLookAndFeel() {
			return Owner.Manager.LookAndFeel;
		}
		#endregion
		protected override IContentContainerHeaderInfo CreateHeaderInfo() {
			return new SlideGroupHeaderInfo(Owner, this);
		}
		class SlideGroupHeaderInfo : ContentContainerHeaderInfo {
			SlideGroupHeadersHandler headersHandlerCore;
			SlideGroupHeadersHandler HeadersHandler {
				get { return headersHandlerCore; }
			}
			public SlideGroupHeaderInfo(WindowsUIView view, ISlideGroupInfo containerInfo)
				: base(view, containerInfo) {
				headersHandlerCore = new SlideGroupHeadersHandler(containerInfo);
			}
			protected override void OnDispose() {
				Ref.Dispose(ref headersHandlerCore);
				base.OnDispose();
			}
			public new ISlideGroupInfo ContainerInfo {
				get { return base.ContainerInfo as ISlideGroupInfo; }
			}
			public Size[] Headers { get; private set; }
			public Rectangle HeadersRect { get; private set; }
			int maxHeaderSize;
			Rectangle headers;
			protected override Size CalcMinContentSize(GraphicsCache cache, Size textSize, Size buttonSize, int interval) {
				Document[] documents = ContainerInfo.Group.Items.ToArray();
				Headers = new Size[documents.Length];
				maxHeaderSize = 0;
				for(int i = 0; i < documents.Length; i++) {
					string header = string.IsNullOrEmpty(documents[i].Header) ? documents[i].Caption : documents[i].Header;
					Headers[i] = CalcTextSize(cache, header, PaintAppearanceItemHeader);
					maxHeaderSize = Math.Max(maxHeaderSize, Headers[i].Height);
				}
				if(maxHeaderSize > 0) {
					headers = Painter.GetItemsBoundsByContentRectangle(new Rectangle(0, 0, 0, maxHeaderSize));
					Size sizeWithButtonPanel = base.CalcMinContentSize(cache, textSize, buttonSize, interval);
					return new Size(-1, Math.Max(textSize.Height + headers.Height, headers.Height + sizeWithButtonPanel.Height));
				}
				return base.CalcMinContentSize(cache, textSize, buttonSize, interval);
			}
			protected override void CalcElements(Point offset, Size textSize, Size buttonSize, Size contentSize, int textOffset) {
				Size oldContentSize = new Size(contentSize.Width, contentSize.Height - headers.Height);
				base.CalcElements(offset, textSize, buttonSize, oldContentSize, textOffset);
				HeadersRect = new Rectangle(offset.X, Bounds.Bottom - headers.Height - headers.Top,
					Bounds.Right - offset.X, maxHeaderSize);
			}
			protected override void OnMouseDown(MouseEventArgs e) {
				if(HeadersHandler != null) HeadersHandler.OnMouseDown(e);
				base.OnMouseDown(e);
			}
			protected override void OnMouseMove(MouseEventArgs e) {
				if(HeadersHandler != null) HeadersHandler.OnMouseMove(e);
				base.OnMouseMove(e);
			}
			protected override void OnMouseUp(MouseEventArgs e) {
				if(HeadersHandler != null) HeadersHandler.OnMouseUp(e);
				base.OnMouseUp(e);
			}
			protected override void OnMouseLeave() {
				if(HeadersHandler != null) HeadersHandler.OnMouseLeave();
				base.OnMouseLeave();
			}
		}
		class SlideGroupHeadersHandler : IDisposable {
			ISlideGroupInfo slideGroupInfo;
			public SlideGroupHeadersHandler(ISlideGroupInfo slideGroupInfo) {
				this.slideGroupInfo = slideGroupInfo;
			}
			public virtual void OnMouseDown(MouseEventArgs e) {
				if(e.Button == MouseButtons.Left)
					PressedInfo = CalcHitInfo(e.Location);
			}
			public virtual void OnMouseUp(MouseEventArgs e) {
				if(e.Button == MouseButtons.Left) {
					IDocumentInfo hitInfo = CalcHitInfo(e.Location);
					if(hitInfo != null && hitInfo.Document == PressedDocument)
						PerformClick(hitInfo.Document);
				}
				PressedInfo = null;
			}
			public virtual void OnMouseMove(MouseEventArgs e) {
				HotInfo = CalcHitInfo(e.Location);
			}
			public virtual void OnMouseLeave() {
				HotInfo = null;
			}
			public void Reset() {
				HotInfo = null;
				PressedInfo = null;
			}
			public Document PressedDocument {
				get { return PressedInfo != null ? PressedInfo.Document : null; }
			}
			public Document HotDocument {
				get { return HotInfo != null ? HotInfo.Document : null; }
			}
			IDocumentInfo pressedInfoCore;
			protected IDocumentInfo PressedInfo {
				get { return pressedInfoCore; }
				private set { SetState(ref pressedInfoCore, value); }
			}
			IDocumentInfo hotInfoCore;
			protected IDocumentInfo HotInfo {
				get { return hotInfoCore; }
				private set { SetState(ref hotInfoCore, value); }
			}
			void SetState(ref IDocumentInfo info, IDocumentInfo value) {
				if(info == value) return;
				if(info != null && value != null && info.Document == value.Document) return;
				if(info != null)
					Invalidate(info.Document);
				info = value;
				if(info != null)
					Invalidate(info.Document);
			}
			protected void Invalidate(Document document) {
				if(slideGroupInfo == null) return;
				Document[] documents = slideGroupInfo.Group.Items.ToArray();
				int index = Array.IndexOf(documents, document);
				slideGroupInfo.Owner.Invalidate(slideGroupInfo.Headers[index]);
			}
			protected IDocumentInfo CalcHitInfo(Point point) {
				IDocumentInfo result = null;
				Document[] documents = slideGroupInfo.Group.Items.ToArray();
				for(int i = 0; i < slideGroupInfo.Headers.Length; i++) {
					if(!slideGroupInfo.Headers[i].Contains(point)) continue;
					if(slideGroupInfo.TryGetValue(documents[i], out result))
						break;
				}
				return result;
			}
			protected void PerformClick(Document document) {
				if(slideGroupInfo.Group.CanHeaderClick(document))
					slideGroupInfo.Owner.Controller.Activate(document);
			}
			#region IDisposable Members
			public void Dispose() {
				slideGroupInfo = null;
			}
			#endregion
		}
		#region IScrollableElementInfo Members
		int? startValue;
		void IScrollableElementInfo.OnStartScroll() {
			startValue = ScrollBarInfo.Value;
		}
		void IScrollableElementInfo.OnScroll(int delta) {
			ScrollBarInfo.SetScrollBarValue(ScrollEventType.First, startValue.Value + delta);
		}
		void IScrollableElementInfo.OnEndScroll() {
			startValue = null;
		}
		#endregion
		#region IInteractiveElementInfo Members
		public void ProcessMouseDown(MouseEventArgs e) { }
		public void ProcessMouseMove(MouseEventArgs e) { }
		public void ProcessMouseUp(MouseEventArgs e) { }
		public void ProcessMouseLeave() { }
		public void ProcessMouseWheel(MouseEventArgs e) {
			if(ScrollBarInfo == null) return;
			if(Control.ModifierKeys == Keys.None)
				ScrollBarInfo.Handler.OnMouseWheel(e);
			if(Control.ModifierKeys == Keys.Control && e.Delta < 0)
				Owner.Controller.Overview(Group);
		}
		#endregion
	}
	class SlideGroupInfoPainter : ContentContainerInfoPainter {
		protected override void DrawContent(GraphicsCache cache, IContentContainerInfo info) {
			ISlideGroupInfo groupInfo = info as ISlideGroupInfo;
			if(groupInfo.ScrollBarInfo.Visible)
				groupInfo.ScrollBarInfo.DoDraw(cache, groupInfo.ScrollRect);
			Document[] documents = groupInfo.Group.Items.ToArray();
			for(int i = 0; i < documents.Length; i++) {
				if(groupInfo.Headers[i].Size.IsEmpty) continue;
				string header = string.IsNullOrEmpty(documents[i].Header) ? documents[i].Caption : documents[i].Header;
				groupInfo.HeaderInfo.PaintAppearanceItemHeader.DrawString(cache, header, groupInfo.Headers[i]);
			}
		}
		public override Padding GetContentMargins() {
			return new Padding(40, 0, 40, 40);
		}
		public virtual int Interval {
			get { return 40; }
		}
	}
	class SlideGroupInfoSkinPainter : SlideGroupInfoPainter {
		ISkinProvider providerCore;
		public SlideGroupInfoSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		public override int Interval {
			get {
				object interval = GetSkin().Properties[MetroUISkins.SlideGroupInterval];
				if(interval != null)
					return (int)interval;
				return base.Interval;
			}
		}
		public override Padding GetContentMargins() {
			SkinElement slideGroup = GetSlideGroupElement();
			if(slideGroup != null) {
				var edges = slideGroup.ContentMargins;
				return new Padding(edges.Left, edges.Top, edges.Right, edges.Bottom);
			}
			return base.GetContentMargins();
		}
		protected virtual SkinElement GetSlideGroupElement() {
			return GetSkin()[MetroUISkins.SkinSlideGroup];
		}
		protected virtual Skin GetSkin() {
			return MetroUISkins.GetSkin(providerCore);
		}
	}
}
