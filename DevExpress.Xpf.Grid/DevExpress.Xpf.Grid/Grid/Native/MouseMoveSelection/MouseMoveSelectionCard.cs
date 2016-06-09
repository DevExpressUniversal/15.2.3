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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using DevExpress.Utils;
namespace DevExpress.Xpf.Grid.Native {
	public abstract class MouseMoveSelectionCardBase {
		public abstract bool CanScrollHorizontally { get; }
		public abstract bool CanScrollVertically { get; }
		public abstract bool AllowNavigation { get; }
		public abstract void OnMouseDown(DataViewBase cardView, IDataViewHitInfo hitInfo);
		public abstract void UpdateSelection(DataViewBase cardView);
		public abstract void OnMouseUp(DataViewBase cardView);
		public abstract void CaptureMouse(DataViewBase cardView);
		public virtual void ReleaseMouseCapture(DataViewBase cardView) {
			DataViewBase rootView = cardView.RootView;
			if(MouseHelper.Captured == rootView)
				MouseHelper.ReleaseCapture(rootView);
		}
	}
	public class MouseMoveSelectionCardNone : MouseMoveSelectionCardBase {
		public static readonly MouseMoveSelectionCardNone Instance = new MouseMoveSelectionCardNone();
		public override bool CanScrollHorizontally { get { return false; } }
		public override bool CanScrollVertically { get { return false; } }
		public override bool AllowNavigation { get { return true; } }
		public override void OnMouseDown(DataViewBase cardView, IDataViewHitInfo hitInfo) {
		}
		public override void UpdateSelection(DataViewBase cardView) {
		}
		public override void OnMouseUp(DataViewBase cardView) {
		}
		public override void CaptureMouse(DataViewBase cardView) {
		}
	}
	public class MouseMoveSelectionRectangleCard : MouseMoveSelectionCardBase {
		public override bool CanScrollHorizontally { get { return true; } }
		public override bool CanScrollVertically { get { return true; } }
		public override bool AllowNavigation { get { return false; } }
		public static readonly MouseMoveSelectionRectangleCard Instance = new MouseMoveSelectionRectangleCard();
		bool isFirst;
		MouseMoveSelectionRectangleHelper helper = new MouseMoveSelectionRectangleHelper();
		public override void OnMouseDown(DataViewBase cardView, IDataViewHitInfo hitInfo) {
			isFirst = true;
			cardView.ScrollTimer.Start();
			helper.OnMouseDown(cardView);
		}
		public override void UpdateSelection(DataViewBase cardView) {
			int visibleIndex = (int)cardView.GetViewAndVisibleIndex(cardView.ViewBehavior.LastMousePosition.Y).Value;
			if(visibleIndex < 0)
				return;
			CaptureMouse(cardView);
			if(helper.StartPoint == helper.GetTransformPoint(cardView, cardView.ViewBehavior.LastMousePosition))
				return;
			helper.UpdateSelection(cardView, cardView.ViewBehavior);
			CardView card = (CardView)cardView;
			var listRowInfo = card.CardRowInfoCollection;
			Rect rectSelected = new Rect(helper.StartPoint, helper.EndPoint);
			Point min = MouseMoveSelectionRectangleHelper.GetMinTransformPoint(cardView, 0);
			Point max = MouseMoveSelectionRectangleHelper.GetMaxTransformPoint(cardView);
			cardView.BeginSelectionCore();
			foreach(var info in listRowInfo) {
				foreach(var element in info.Elements) {
					if(!element.IsRowVisible)
						continue;
					Rect rectElement = GetRectangleFromElement(element.Element, cardView);
					if(rectElement.X + rectElement.Width <= min.X || rectElement.X > max.X || rectElement.Y + rectElement.Height <= min.Y || rectElement.Y > max.Y)
						continue;
					Rect tempRect = Rect.Intersect(rectSelected, rectElement);
					if(tempRect.Width > 0 || tempRect.Height > 0) {
						if(isFirst) {
							cardView.DataControl.UnselectAll();
							isFirst = false;
						}
						cardView.DataControl.SelectItem(TableView.GetRowHandle(element.Element).Value);
					} else
						cardView.DataControl.UnselectItem(TableView.GetRowHandle(element.Element).Value);					
				}
			}
			cardView.EndSelectionCore();
		}
		public override void OnMouseUp(DataViewBase cardView) {
			cardView.ScrollTimer.Stop();
			helper.OnMouseUp(cardView);		  
		}
		public override void CaptureMouse(DataViewBase cardView) {
			if(MouseHelper.Captured != cardView) {
				if(cardView.IsKeyboardFocusWithin)
					MouseHelper.Capture(cardView);
				else
					cardView.StopSelection();
			}
		}
		Rect GetRectangleFromElement(FrameworkElement element, DataViewBase cardView) {
			Point start = element.TransformToAncestor(cardView.DataControl).Transform(new Point(0, 0));
			return new Rect(start, element.RenderSize);
		}
	}
}
