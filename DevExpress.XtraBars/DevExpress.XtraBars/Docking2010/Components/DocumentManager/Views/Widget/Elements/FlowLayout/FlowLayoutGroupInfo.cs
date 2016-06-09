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
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class FlowLayoutGroupInfo : BaseElementInfo {
		FlowLayoutGroup flowLayoutGroupCore;
		public FlowLayoutGroupInfo(WidgetView view, FlowLayoutGroup flowLayoutGroup)
			: base(view) {
			flowLayoutGroupCore = flowLayoutGroup;
		}
		public FlowLayoutGroup Group {
			get { return flowLayoutGroupCore; }
		}
		protected override void CalcCore(Graphics g, Rectangle bounds) {
			base.CalcCore(g, bounds);
			IDocumentInfo[] infos = GetDocumentInfos();
			Point offset = Point.Empty;
			switch(Group.FlowDirection) {
				case FlowDirection.LeftToRight:
					offset = new Point(bounds.X + Group.WidgetView.DocumentSpacing, bounds.Y + Group.WidgetView.DocumentSpacing);
					CalcLeftToRightDirection(offset, infos);
					break;
				case FlowDirection.TopDown:
					offset = new Point(bounds.X + Group.WidgetView.DocumentSpacing, bounds.Y + Group.WidgetView.DocumentSpacing);
					CalcTopDownDirection(offset, infos);
					break;
				case FlowDirection.RightToLeft:
					offset = new Point(bounds.Right - Group.WidgetView.DocumentSpacing, bounds.Y + Group.WidgetView.DocumentSpacing);
					CalcRightToLeftDirection(offset, infos);
					break;
				case FlowDirection.BottomUp:
					offset = new Point(bounds.X + Group.WidgetView.DocumentSpacing, bounds.Bottom - Group.WidgetView.DocumentSpacing);
					CalcBottomUpDirection(offset, infos);
					break;
			}
		}
		public IDocumentInfo[] GetDocumentInfos() {
			List<IDocumentInfo> infos = new List<IDocumentInfo>();
			foreach(Document document in Group.Items) {
				if(document.Info != null && !document.IsDisposing && document.IsVisible)
					infos.Add(document.Info);
			}
			return infos.ToArray();
		}
		protected virtual void CalcBottomUpDirection(Point offset, IDocumentInfo[] infos) {
			int maxRowWidth = 0;
			for(int i = 0; i < infos.Length; i++) {
				IDocumentInfo info = infos[i];
				IDocumentInfo nextInfo = i + 1 < infos.Length ? infos[i + 1] : null;
				info.Bounds = new Rectangle(new Point(offset.X, offset.Y - info.Document.Height), new Size(info.Document.Width, info.Document.Height));
				offset.Y -= Group.WidgetView.DocumentSpacing + info.Document.Height;
				maxRowWidth = info.Document.Width > maxRowWidth ? info.Document.Width : maxRowWidth;
				if((info.Bounds.Top < Bounds.Top || (nextInfo != null && offset.Y - nextInfo.Document.Height - Group.WidgetView.DocumentSpacing < Bounds.Top)) && Group.WrapContent != DefaultBoolean.False) {
					offset.Y = Bounds.Bottom - Group.WidgetView.DocumentSpacing;
					offset.X += Group.WidgetView.DocumentSpacing + maxRowWidth;
					maxRowWidth = 0;
				}
			}
		}
		protected virtual void CalcRightToLeftDirection(Point offset, IDocumentInfo[] infos) {
			int maxRowHeight = 0;
			for(int i = 0; i < infos.Length; i++) {
				IDocumentInfo info = infos[i];
				IDocumentInfo nextInfo = i + 1 < infos.Length ? infos[i + 1] : null;
				info.Bounds = new Rectangle(new Point(offset.X - info.Document.Width, offset.Y), new Size(info.Document.Width, info.Document.Height));
				offset.X -= Group.WidgetView.DocumentSpacing + info.Document.Width;
				maxRowHeight = info.Document.Height > maxRowHeight ? info.Document.Height : maxRowHeight;
				if((info.Bounds.Left < Bounds.Left || (nextInfo != null && offset.X - nextInfo.Document.Width - Group.WidgetView.DocumentSpacing < Bounds.Left)) && Group.WrapContent != DefaultBoolean.False) {
					offset.X = Bounds.Right - Group.WidgetView.DocumentSpacing;
					offset.Y += Group.WidgetView.DocumentSpacing + maxRowHeight;
					maxRowHeight = 0;
				}
			}
		}
		protected virtual void CalcTopDownDirection(Point offset, IDocumentInfo[] infos) {
			int maxRowWidth = 0;
			for(int i = 0; i < infos.Length; i++) {
				IDocumentInfo info = infos[i];
				IDocumentInfo nextInfo = i + 1 < infos.Length ? infos[i + 1] : null;
				info.Bounds = new Rectangle(offset, new Size(info.Document.Width, info.Document.Height));
				offset.Y += Group.WidgetView.DocumentSpacing + info.Document.Height;
				maxRowWidth = info.Bounds.Width > maxRowWidth ? info.Bounds.Width : maxRowWidth;
				if((info.Bounds.Bottom > Bounds.Bottom || (nextInfo != null && nextInfo.Bounds.Height + offset.Y > Bounds.Bottom)) && Group.WrapContent != DefaultBoolean.False) {
					offset.Y = Bounds.Top + Group.WidgetView.DocumentSpacing;
					offset.X += Group.WidgetView.DocumentSpacing + maxRowWidth;
					maxRowWidth = 0;
				}
			}
		}
		protected virtual void CalcLeftToRightDirection(Point offset, IDocumentInfo[] infos) {
			int maxRowHeight = 0;
			for(int i = 0; i < infos.Length; i++) {
				IDocumentInfo info = infos[i];
				IDocumentInfo nextInfo = i + 1 < infos.Length ? infos[i + 1] : null;
				info.Bounds = new Rectangle(offset, new Size(info.Document.Width, info.Document.Height));
				offset.X += Group.WidgetView.DocumentSpacing + info.Document.Width;
				maxRowHeight = info.Document.Height > maxRowHeight ? info.Document.Height : maxRowHeight;
				if((info.Bounds.Right > Bounds.Right || (nextInfo != null && nextInfo.Document.Width + offset.X > Bounds.Right)) && Group.WrapContent != DefaultBoolean.False) {
					offset.X = Bounds.Left + Group.WidgetView.DocumentSpacing;
					offset.Y += Group.WidgetView.DocumentSpacing + maxRowHeight;
					maxRowHeight = 0;
				}
			}
		}
		protected internal virtual void AddAnimation() {
			DevExpress.Utils.Drawing.Animation.ISupportXtraAnimation animationObject = Owner.Manager.GetOwnerControl() as DevExpress.Utils.Drawing.Animation.ISupportXtraAnimation;
			if(animationObject == null) return;
			var existAnimation = DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.Get(animationObject, Group);
			var newAnimation = new WidgetsMovingAnimationInfo(Group, animationObject, Group, (Owner as WidgetView).DocumentAnimationProperties);
			if(existAnimation != null) {
				newAnimation.BeginTick = existAnimation.CurrentTick;
				XtraAnimator.RemoveObject(animationObject, Group);
				existAnimation.Dispose();
			}
			DevExpress.Utils.Drawing.Animation.XtraAnimator.Current.AddAnimation(newAnimation);
		}
		EmptyDocument emptyDocument = new EmptyDocument();
		internal void OnBeginDragging(Document document) {
			emptyDocument.Index = Group.Items.IndexOf(document);
			if(Group.ItemDragMode == ItemDragMode.Swap)
				emptyDocument.SetInfo(new DocumentInfo(Group.WidgetView, emptyDocument));
			else
				Group.Items.Insert(emptyDocument.Index, emptyDocument);
			emptyDocument.SetBoundsCore(document.Info.Bounds);
			if(Group.ItemDragMode == ItemDragMode.Move)
				Group.Items.Remove(document);
		}
		protected internal void OnDragging(Point point, Document dragItem) {
			int newIndex = GetEmptyDocumentIndex(point);
			if(Group.ItemDragMode == ItemDragMode.Swap) {
				emptyDocument.Index = newIndex;
				UpdateEmptyDocumentBounds(dragItem, newIndex);
			}
			else {
				int oldEmptyDocumentIndex = Group.Items.IndexOf(emptyDocument);
				if(newIndex == -1 || newIndex == oldEmptyDocumentIndex) return;
				Group.Items.Remove(emptyDocument);
				Group.Items.Insert(newIndex, emptyDocument);
				emptyDocument.Index = newIndex;
				AddAnimation();
			}
		}
		protected internal void OnEndDragging(Point dropPoint, Document document) {
			int newDocumentIndex = emptyDocument.Index;
			int currentDocumentIndex = Group.Items.IndexOf(document);
			if(Group.ItemDragMode == ItemDragMode.Swap) {
				if(newDocumentIndex == -1) return;
				Document swapDocument = Group.Items[newDocumentIndex];
				if(newDocumentIndex > currentDocumentIndex) newDocumentIndex--;
				Group.Items.Remove(document);
				Group.Items.Remove(swapDocument);
				Group.Items.Insert(newDocumentIndex, document);
				Group.Items.Insert(currentDocumentIndex, swapDocument);
			}
			else {
				Group.Items.Remove(emptyDocument);
				Group.Items.Insert(newDocumentIndex, document);
			}
			emptyDocument.Info.Bounds = Rectangle.Empty;
		}
		void UpdateEmptyDocumentBounds(Document dragItem, int newIndex) {
			if(newIndex == -1) {
				emptyDocument.Info.Bounds = dragItem.Info.Bounds;
			}
			else {
				emptyDocument.Info.Bounds = new Rectangle(Group.Items[newIndex].Info.Bounds.Location, dragItem.Info.Bounds.Size);
			}
		}
		public override System.Type GetUIElementKey() {
			return typeof(FlowLayoutGroupInfo);
		}
		protected int GetEmptyDocumentIndex(Point point) {
			for(int i = 0; i < Group.Items.Count; i++) {
				Rectangle itemRect = Group.Items[i].Info.Bounds;
				if(Group.ItemDragMode == ItemDragMode.Swap)
					itemRect = CalcDocumentHitBounds(itemRect); 
				if(itemRect.Contains(point)) {
					if(Group.Items[i] == emptyDocument || Group.ItemDragMode == ItemDragMode.Swap)
						return i;
					if(CalcPositionConcerningCenter(Group.Items[i].Info, point))
						return i > 0 && Group.Items[i - 1] == emptyDocument ? i : i + 1;
					else
						return i > 0 && Group.Items[i - 1] == emptyDocument ? i - 1 : i;
				}
			}
			return -1;
		}
		Rectangle CalcDocumentHitBounds(Rectangle bounds) {
			bool isHorizontal = Group.FlowDirection == FlowDirection.LeftToRight || Group.FlowDirection == FlowDirection.RightToLeft;
			int halfSpacing = (int)Math.Round(Group.WidgetView.DocumentSpacing / 2.0, MidpointRounding.ToEven);
			if(isHorizontal) {
				bounds.X -= halfSpacing;
				bounds.Width += Group.WidgetView.DocumentSpacing;
				bounds.Y -= halfSpacing;
				bounds.Height += Group.WidgetView.DocumentSpacing;
			}
			else {
				bounds.Y -= halfSpacing;
				bounds.Height += Group.WidgetView.DocumentSpacing;
				bounds.X -= halfSpacing;
				bounds.Width += Group.WidgetView.DocumentSpacing;
			}
			return bounds;
		}
		bool CalcPositionConcerningCenter(IDocumentInfo documentInfo, Point point) {
			if(Group.FlowDirection == FlowDirection.LeftToRight)
				return documentInfo.Bounds.X + documentInfo.Bounds.Width / 2 < point.X;
			if(Group.FlowDirection == FlowDirection.RightToLeft)
				return documentInfo.Bounds.X + documentInfo.Bounds.Width / 2 > point.X;
			if(Group.FlowDirection == FlowDirection.TopDown)
				return documentInfo.Bounds.Y + documentInfo.Bounds.Height / 2 < point.Y;
			if(Group.FlowDirection == FlowDirection.BottomUp)
				return documentInfo.Bounds.Y + documentInfo.Bounds.Height / 2 > point.Y;
			return false;
		}
		internal Rectangle GetEmptyDocumentBounds() {
			if(emptyDocument != null && emptyDocument.Info != null)
				return emptyDocument.Info.Bounds;
			return Rectangle.Empty;
		}
	}
}
