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

using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IContentContainerInfo : IBaseElementInfo, IUIElement {
		IContentContainer Container { get; }
		Rectangle Header { get; }
		Rectangle Content { get; }
		IContentContainerHeaderInfo HeaderInfo { get; }
		Rectangle GetBounds(Document document);
		bool DragMoveHitTest(Point point);
	}
	public interface IDocumentContainerInfo : IContentContainerInfo {
		IDocumentInfo DocumentInfo { get; }
	}
	abstract class BaseContentContainerInfo : BaseElementInfo, IContentContainerInfo {
		IContentContainer containerCore;
		IContentContainerHeaderInfo headerInfoCore;
		ObjectPainter painterCore;
		public BaseContentContainerInfo(WindowsUIView view, IContentContainer container)
			: base(view) {
			containerCore = container;
			headerInfoCore = CreateHeaderInfo();
			LayoutHelper.Register(this, HeaderInfo);
			UpdateStyleCore();
		}
		protected override void OnDispose() {
			ResetStyleCore();
			LayoutHelper.Unregister(this, HeaderInfo);
			Ref.Dispose(ref headerInfoCore);
			LayoutHelper.Unregister(this);
			containerCore = null;
			base.OnDispose();
		}
		protected virtual IContentContainerHeaderInfo CreateHeaderInfo() {
			return new ContentContainerHeaderInfo(Owner, this);
		}
		protected ContentContainerInfoPainter Painter {
			get { return painterCore as ContentContainerInfoPainter; }
		}
		protected override void UpdateStyleCore() {
			painterCore = ((WindowsUIViewPainter)Owner.Painter).GetPainter(this);
		}
		protected override void ResetStyleCore() {
			painterCore = null;
		}
		public new WindowsUIView Owner {
			get { return base.Owner as WindowsUIView; }
		}
		public IContentContainer Container {
			get { return containerCore; }
		}
		public Rectangle Header { get; protected set; }
		public Rectangle Content { get; protected set; }
		public Rectangle GetBounds(Document document) {
			return GetBoundsCore(document);
		}
		public IContentContainerHeaderInfo HeaderInfo {
			get { return headerInfoCore; }
		}
		protected Rectangle CalcContentWithMargins(Rectangle content, out System.Windows.Forms.Padding contentMargins) {
			contentMargins = Container.Properties.HasMargin ?
				Container.Properties.ActualMargin : Painter.GetContentMargins();
			return new Rectangle(
				content.Left + contentMargins.Left,
				content.Top + contentMargins.Top,
				content.Width - contentMargins.Horizontal,
				content.Height - contentMargins.Vertical);
		}
		protected Rectangle CalcContentWithMargins(Rectangle content) {
			System.Windows.Forms.Padding contentMargins;
			return CalcContentWithMargins(content, out contentMargins);
		}
		Rectangle prevContent;
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			Size headerSize = Container.Properties.CanShowCaption ? 
				HeaderInfo.CalcMinSize(g, bounds) : Size.Empty;
			int headerOffset = Container.Properties.CanShowCaption && Container.Properties.HasHeaderOffset ?
				Container.Properties.ActualHeaderOffset : Painter.GetHeaderOffset();
			Header = GetHeaderBounds(g, bounds, headerSize, headerOffset);
			Content = GetContentBounds(g, bounds, headerSize, headerOffset);
			CalcHeader(g, Header);
			CalcContent(g, Content);
			if(prevContent != Content && !Content.IsEmpty) {
				this.prevContent = Content;
				OnContentBoundsChanged();
			}
		}
		protected virtual void OnContentBoundsChanged() {
			if(Owner.Manager != null)
				Owner.Manager.InvokePatchActiveChildren();
		}
		protected virtual Rectangle GetHeaderBounds(Graphics g, Rectangle bounds, Size headerSize, int headerOffset) {
			return new Rectangle(bounds.Left, bounds.Top + headerOffset, headerSize.Width, headerSize.Height);
		}
		protected virtual Rectangle GetContentBounds(Graphics g, Rectangle bounds, Size headerSize, int headerOffset) {
			return new Rectangle(bounds.Left, bounds.Top + headerSize.Height + headerOffset, bounds.Width, bounds.Height - headerSize.Height - headerOffset);
		}
		protected sealed override void DrawCore(GraphicsCache cache) {
			Painter.DrawObject(new ContentContainerInfoArgs(cache, this));
		}
		protected virtual void CalcHeader(Graphics g, Rectangle header) {
			HeaderInfo.Calc(g, header);
		}
		protected abstract void CalcContent(Graphics g, Rectangle content);
		protected virtual Rectangle GetBoundsCore(Document document) {
			return Content;
		}
		public virtual bool DragMoveHitTest(Point point) {
			return new Rectangle(Bounds.Left, Bounds.Top, Bounds.Width, Header.Bottom - Bounds.Top).Contains(point);
		}
		public bool ProcessFlick(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) {
			return ProcessFlickCore(point, args);
		}
		public bool ProcessGesture(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) {
			return ProcessGestureCore(gid, args, parameters);
		}
		protected virtual bool ProcessFlickCore(Point point, DevExpress.Utils.Gesture.FlickGestureArgs args) {
			switch(args.Direction) {
				case DevExpress.Utils.Gesture.FlickDirection.Left:
					Owner.Controller.Back();
					break;
				case DevExpress.Utils.Gesture.FlickDirection.Down:
					Owner.ShowNavigationAdorner();
					break;
			}
			return false;
		}
		const int SwipeThreshold = 12;
		const int SwipeBounds = 80;
		protected virtual bool ProcessGestureCore(GestureID gid, DevExpress.Utils.Gesture.GestureArgs args, object[] parameters) {
			Rectangle top = DevExpress.Utils.PlacementHelper.Arrange(new Size(Bounds.Width, SwipeBounds), Bounds, ContentAlignment.TopCenter);
			Rectangle bottom = DevExpress.Utils.PlacementHelper.Arrange(new Size(Bounds.Width, SwipeBounds), Bounds, ContentAlignment.BottomCenter);
			switch(gid) {
				case GestureID.QueryAllowGesture:
					Point point = (Point)parameters[0];
					if(top.Contains(point) || bottom.Contains(point)) {
						parameters[1] = new DevExpress.Utils.Gesture.GestureAllowArgs[] { DevExpress.Utils.Gesture.GestureAllowArgs.PanVertical };
					}
					else {
						parameters[1] = new DevExpress.Utils.Gesture.GestureAllowArgs[] { DevExpress.Utils.Gesture.GestureAllowArgs.PressAndTap };
					}
					break;
				case GestureID.Pan:
					Point delta = (Point)parameters[0];
					Point overPan = (Point)parameters[1];
					if(System.Math.Abs(delta.Y) > SwipeThreshold) {
						if(delta.Y > 0 && top.Contains(args.Start.Point)) {
							Owner.ShowNavigationAdorner();
							return true;
						}
						if(delta.Y < 0 && bottom.Contains(args.Start.Point)) {
							Owner.ShowNavigationAdorner();
							return true;
						}
					}
					break;
				case GestureID.PressAndTap:
					Owner.ShowNavigationAdorner();
					return true;
			}
			return false;
		}
		#region IUIElement
		IUIElement IUIElement.Scope { get { return Owner; } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children {
			get { return uiChildren; }
		}
		#endregion IUIElement
	}
}
