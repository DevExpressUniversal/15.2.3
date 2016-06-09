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
namespace DevExpress.XtraLayout.HitInfo {
	public enum LayoutItemHitTest {None, Item, TextArea, ControlsArea, HSizing, VSizing} ;
	public class BaseLayoutItemHitInfo {
		BaseLayoutItem item;
		Point hitPoint;
		LayoutItemHitTest hitTestType;
		public BaseLayoutItemHitInfo() {
			hitPoint = Point.Empty;
			hitTestType = LayoutItemHitTest.None;
		}
		public BaseLayoutItemHitInfo(Point point, LayoutItemHitTest hitType, BaseLayoutItem item) {
			SetHitPoint(point);
			SetItem(item);
			SetHitTestType(hitType);
		}
		public BaseLayoutItemHitInfo(BaseLayoutItemHitInfo hitInfo) {
			if(hitInfo != null) {
				SetHitPoint(hitInfo.HitPoint);
				SetItem(hitInfo.Item);
				SetHitTestType(hitInfo.HitType);
			}
		}
		public bool IsSizing { get { return (HitType == LayoutItemHitTest.HSizing || HitType == LayoutItemHitTest.VSizing); } }
		public virtual BaseLayoutItem Item{
			get {return item;}
		}
		public Point HitPoint{
			get { return hitPoint;}
		}
		public LayoutItemHitTest HitType{
			get{ return hitTestType;}
		}
		public virtual bool IsGroup {
			get {
				return false;
			}
		}
		public virtual bool IsTabbedGroup {
			get {
				return false;
			}
		}
		public virtual bool IsExpandButton {
			get {
				return false;
			}
		}
		public virtual int TabPageIndex {
			get {
				return -1;
			}
		}
		public virtual bool IsLastRow { get { return true; } set { } }
		protected internal void SetItem(BaseLayoutItem item) {
			this.item = item;	 
		}
		protected internal void SetHitPoint(Point point) {
			hitPoint = point;
		}
		protected internal void SetHitTestType(LayoutItemHitTest newType) {
			hitTestType = newType;
		}
	}
}
