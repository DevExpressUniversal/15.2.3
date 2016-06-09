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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.XtraBars.Docking2010.Customization;
using DevExpress.XtraBars.Docking2010.DragEngine;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public interface IStackGroupInfo : IUIElement, IBaseElementInfo, IDockingAdornerInfo {
		StackGroup Group { get; }
		IEnumerable<IDocumentInfo> Documents { get; }
		void Register(Document document);
		void Unregister(Document document);
		void AddAnimation();
		Rectangle CaptionBounds { get; }
	}
	public class StackGroupInfo : BaseElementInfo, IStackGroupInfo {
		StackGroup groupCore;
		IDictionary<Document, IDocumentInfo> documentInfosCore;
		public StackGroupInfo(WidgetView view, StackGroup group)
			: base(view) {
			groupCore = group;
			documentInfosCore = new Dictionary<Document, IDocumentInfo>();
			PaintAppearance = new FrozenAppearance();
		}
		protected override void OnDispose() {
			LayoutHelper.Unregister(this);
			base.OnDispose();
			groupCore = null;
		}
		public override System.Type GetUIElementKey() {
			return typeof(IStackGroupInfo);
		}
		public IEnumerable<IDocumentInfo> Documents {
			get { return documentInfosCore.Values; }
		}
		protected IDictionary<Document, IDocumentInfo> DocumentInfos {
			get { return documentInfosCore; }
		}
		public StackGroup Group {
			get { return groupCore; }
		}
		public void Register(Document document) {
			IDocumentInfo documentInfo;
			if(!DocumentInfos.TryGetValue(document, out documentInfo)) {
				if(document.Info != null && document.Parent != null && document.Parent.Info != null) {
					document.Parent.Info.Unregister(document);
				}
				if(document is EmptyDocument) {
					documentInfo = CreateEmptyDocumentInfo(document);
				}
				else {
					documentInfo = document.Info ?? CreateDocumentInfo(document);
				}
				document.SetInfo(documentInfo);
				DocumentInfos.Add(document, documentInfo);
			}
		}
		public void Unregister(Document document) {
			IDocumentInfo info;
			if(DocumentInfos.TryGetValue(document, out info)) {
				DocumentInfos.Remove(document);
				if(CanAddAnimation())
					AddAnimation();
				info.Bounds = new Rectangle(-10000, -10000, info.Bounds.Width, info.Bounds.Height);
			}
		}
		bool CanAddAnimation() {
			return Group != null && !Group.IsDisposing && !Group.AnimationUpdateLocked && Owner!= null && !Owner.IsUpdateLocked;
		}
		Rectangle captionBoundsCore = Rectangle.Empty;
		Rectangle textBoundsCore = Rectangle.Empty;
		public Rectangle CaptionBounds { get { return captionBoundsCore; } }
		public Rectangle TextBounds { get { return textBoundsCore; } }
		internal AppearanceObject PaintAppearance { get; set; }
		protected void CalcCaptionBounds(Graphics g, Rectangle bounds) {
			WidgetViewPainter painter = (Owner.Painter as WidgetViewPainter);
			AppearanceHelper.Combine(PaintAppearance, new AppearanceObject[] { Group.AppearanceCaption }, painter.DefaultAppearanceCaption);
			textBoundsCore = painter.GetCaptionClientRect(bounds);
			if(Group.Parent.Orientation == Orientation.Vertical) {
				Size textSize = Size.Round(PaintAppearance.CalcTextSize(g, Group.Caption, TextBounds.Width));
				textBoundsCore.Height = textSize.Height;
			}
			else {
				Size textSize = Size.Round(PaintAppearance.CalcTextSize(g, Group.Caption, TextBounds.Height));
				textBoundsCore.Width = textSize.Height;
			}
			captionBoundsCore = painter.GetCaptionBoundsByClientRect(textBoundsCore);
			AcceptDocumentSpacing();
		}
		void AcceptDocumentSpacing() {
			Size captionSize = captionBoundsCore.Size;
			if(Group.Parent.Orientation == Orientation.Horizontal && Group.Parent.DocumentSpacing > captionSize.Width) {
				int delta = Group.Parent.DocumentSpacing - captionSize.Width;
				captionBoundsCore.Width += delta;
				textBoundsCore.Offset(delta / 2, 0);
			}
			if(Group.Parent.Orientation == Orientation.Vertical && Group.Parent.DocumentSpacing > captionSize.Height) {
				int delta = Group.Parent.DocumentSpacing - captionSize.Height;
				captionBoundsCore.Height += delta;
				textBoundsCore.Offset(0, delta / 2);
			}
		}
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			Point offset = bounds.Location;
			bool useCaption = !string.IsNullOrEmpty(Group.Caption);
			if(useCaption) CalcCaptionBounds(g, bounds);
			if(Group.IsHorizontal)
				offset.X += useCaption ? CaptionBounds.Width : Owner.DocumentSpacing;
			else
				offset.Y += useCaption ? CaptionBounds.Height : Owner.DocumentSpacing;
			for(int i = 0; i < Group.Items.Count; i++) {
				Document document = Group.Items[i];
				if(!document.IsVisible) continue;
				IDocumentInfo documentInfo = document.Info;
				int width = Group.IsHorizontal ? documentInfo.Length : bounds.Width;
				int height = Group.IsHorizontal ? bounds.Height : documentInfo.Length;
				documentInfo.Bounds = new Rectangle(offset, new Size(width, height));
				if(Group.IsHorizontal) offset.X += documentInfo.Bounds.Width + Owner.DocumentSpacing;
				else offset.Y += documentInfo.Bounds.Height + Owner.DocumentSpacing;
			}
		}
		public new WidgetView Owner {
			get { return base.Owner as WidgetView; }
		}
		protected virtual IDocumentInfo CreateDocumentInfo(Document document) {
			return new DocumentInfo(Owner, document);
		}
		protected virtual IDocumentInfo CreateEmptyDocumentInfo(Document document) {
			return new EmptyDocumentInfo(Owner, document);
		}
		#region DocumentDraggin
		EmptyDocument emptyDocument = new EmptyDocument();
		protected internal void OnDragging(Point point, BaseDocument dragItem) {
			if(Group.IsFilledUp && !Group.Items.Contains(emptyDocument)) return;
			int newEmptyDocumentIndex = GetNewEmptyDocumentIndex(point);
			int oldEpmtyDocumentIndex = Group.Items.IndexOf(emptyDocument);
			if(newEmptyDocumentIndex != oldEpmtyDocumentIndex && oldEpmtyDocumentIndex != -1) {
				ResetDragging();
			}
			emptyDocument.AssignProperties((Document)dragItem);
			InsertEmptyDocument(newEmptyDocumentIndex);
		}
		protected internal void EndDragging(BaseDocument document) {
			using(Owner.LockPainting()) {
				int index = Group.Items.IndexOf(emptyDocument);
				Group.Remove(emptyDocument);
				Group.Add(document, index);
				AddAnimation();
			}
		}
		protected internal void ResetDragging() {
			RemoveEmptyDocument();
		}
		protected int GetNewEmptyDocumentIndex(Point point) {
			for(int i = 0; i < Group.Items.Count; i++) {
				if(Group.Items[i].Info.Bounds.Contains(point)) {
					if(Group.Items[i] == emptyDocument)
						return i;
					if(CalcPositionConcerningCenter(Group.Items[i].Info, point))
						return i > 0 && Group.Items[i - 1] == emptyDocument ? i : i + 1;
					else
						return i > 0 && Group.Items[i - 1] == emptyDocument ? i - 1 : i;
				}
			}
			if(Group.Items.Contains(emptyDocument))
				return Group.Items.IndexOf(emptyDocument);
			return Group.Items.Count;
		}
		bool CalcPositionConcerningCenter(IDocumentInfo documentInfo, Point point) {
			if(!Group.IsHorizontal) {
				if(documentInfo.Bounds.Y + documentInfo.Bounds.Height / 2 < point.Y)
					return true;
			}
			else
				if(documentInfo.Bounds.X + documentInfo.Bounds.Width / 2 < point.X)
					return true;
			return false;
		}
		protected internal void InsertEmptyDocument(int index) {
			if(!DocumentInfos.ContainsKey(emptyDocument)) {
				emptyDocument.SetParent(Group);
				Group.Add(emptyDocument, index);
				AddAnimation();
			}
		}
		protected internal void RemoveEmptyDocument() {
			if(DocumentInfos.ContainsKey(emptyDocument)) {
				Group.Remove(emptyDocument);
				emptyDocument.SetParent(null);
			}
			if(Group.Items.Count != 0) {
				AddAnimation();
			}
		}
		protected void AddAnimation() {
			DevExpress.Utils.Drawing.Animation.ISupportXtraAnimation animationObject = Owner.Manager.GetOwnerControl() as DevExpress.Utils.Drawing.Animation.ISupportXtraAnimation;
			if(animationObject == null) return;
			var existAnimation = DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.Get(animationObject, Group);
			var newAnimation = new WidgetsMovingAnimationInfo(Group, animationObject, Group, Group.Parent.DocumentAnimationProperties);
			if(existAnimation != null) {
				newAnimation.BeginTick = existAnimation.CurrentTick;
				XtraAnimator.RemoveObject(animationObject, Group);
				existAnimation.Dispose();
			}
			DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.AddAnimation(newAnimation);
		}
		#endregion
		#region DockPanel Docking
		AdornerElementInfo DockingAdornerInfo;
		public void UpdateDocking(Adorner adorner, Point point, BaseDocument dragItem) {
			DockingAdornerInfoArgs args = DockingAdornerInfoArgs.EnsureInfoArgs(ref DockingAdornerInfo, adorner, Owner, dragItem, Group.Parent.ViewInfo.Bounds);
			args.Adorner = Owner.Manager.GetDockingRect();
			args.Container = Owner.Manager.Bounds;
			args.Bounds = Group.Parent.ViewInfo.Bounds;
			args.MousePosition = point;
			args.DragItem = dragItem;
			if(args.Calc())
				adorner.Invalidate();
		}
		public void ResetDocking(Adorner adorner) {
			adorner.Reset(DockingAdornerInfo);
			DockingAdornerInfo = null;
		}
		public bool CanDock(Point point) {
			DockHint hint = DockHint.None;
			if(DockingAdornerInfo != null) {
				DockingAdornerInfoArgs args = DockingAdornerInfo.InfoArgs as DockingAdornerInfoArgs;
				return args.IsOverDockHint(point, out hint) || DocumentInfos.ContainsKey(emptyDocument);
			}
			return DocumentInfos.ContainsKey(emptyDocument);
		}
		public void Dock(Point point, BaseDocument document) {
			DockHint hint = DockHint.None;
			if(DockingAdornerInfo != null) {
				DockingAdornerInfoArgs args = DockingAdornerInfo.InfoArgs as DockingAdornerInfoArgs;
				if(args.IsOverDockHint(point, out hint)) {
					IWidgetViewController controller = Group.Parent.Controller;
					Docking.FloatForm fForm = document.Form as Docking.FloatForm;
					switch(hint) {
						case DockHint.SideLeft:
						case DockHint.SideTop:
						case DockHint.SideRight:
						case DockHint.SideBottom:
							new DockHelper(Owner).DockSide(document, fForm, hint);
							break;
					}
				}
			}
		}
		#endregion
		#region IUIElement
		IUIElement IUIElement.Scope { get { return Owner; } }
		UIChildren uiChildren = new UIChildren();
		UIChildren IUIElement.Children { get { return uiChildren; } }
		#endregion IUIElement
		#region IStackGroupInfo Members
		void IStackGroupInfo.AddAnimation() {
			AddAnimation();
		}
		#endregion
		internal Rectangle GetEmptyDocumentBounds() {
			if(IsDisposing || !Group.Items.Contains(emptyDocument)) return Rectangle.Empty;
			Rectangle emptyDocumentBounds = emptyDocument.Info.Bounds;
			if(Group.Items.IndexOf(emptyDocument) <= Group.Items.Count - 2) {
				Document nextDocument = Group.Items[Group.Items.IndexOf(emptyDocument) + 1];
				if(nextDocument.Control == null || nextDocument.Control.Parent == null) return emptyDocumentBounds;
				if(Group.IsHorizontal)
					emptyDocumentBounds.Width = nextDocument.Control.Parent.Bounds.X - emptyDocumentBounds.X - Group.Parent.DocumentSpacing;
				else
					emptyDocumentBounds.Height = nextDocument.Control.Parent.Bounds.Y - emptyDocumentBounds.Y - Group.Parent.DocumentSpacing;
			}
			return emptyDocumentBounds;
		}
	}
}
