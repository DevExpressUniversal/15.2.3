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
using System.Collections.Generic;
namespace DevExpress.XtraBars.Docking2010.DragEngine {
	public enum LayoutElementHitTest {
		None, Bounds, Content, Header, Border, ControlBox
	}
	public class LayoutElementHitInfo : HitInfo<LayoutElementHitTest> {
		readonly BaseLayoutElement elementCore;
		public LayoutElementHitInfo(Point point, BaseLayoutElement element)
			: base(point) {
			if(element != null && CheckAndSetHitTest(element.Bounds, point, LayoutElementHitTest.Bounds)) 
				elementCore = element;
		}
		public ILayoutElement Element {
			get { return elementCore; }
		}
		public override bool IsEmpty {
			get { return this is EmptyHitInfo; }
		}
		public bool InBounds {
			get { return (HitTest == LayoutElementHitTest.Bounds) || InHeader || InContent || InBorder || InControlBox; }
		}
		public bool InHeader {
			get { return (HitTest == LayoutElementHitTest.Header); }
		}
		public bool InContent {
			get { return (HitTest == LayoutElementHitTest.Content); }
		}
		public bool InBorder {
			get { return (HitTest == LayoutElementHitTest.Border); }
		}
		public bool InControlBox {
			get { return (HitTest == LayoutElementHitTest.ControlBox); }
		}
		public virtual bool InDragBounds {
			get { return InHeader; }
		}
		public virtual bool InReorderingBounds {
			get { return false; }
		}
		public virtual bool InResizeBounds {
			get { return false; }
		}
		public virtual bool InScrollBounds {
			get { return false; }
		}
		public virtual bool InMenuBounds {
			get { return InHeader; }
		}
		public virtual bool InClickBounds {
			get { return InControlBox || InHeader; }
		}
		public virtual bool InClickPreviewBounds {
			get { return InClickBounds; }
		}
		public virtual bool InDoubleClickBounds {
			get { return InHeader && !InControlBox; }
		}
		public virtual bool InHandlerBounds {
			get { return false; }
		}
		public bool IsDragging {
			get { return (elementCore != null) && elementCore.IsDragging; }
		}
		public static readonly LayoutElementHitInfo Empty = new EmptyHitInfo();
		class EmptyHitInfo : LayoutElementHitInfo {
			public EmptyHitInfo()
				: base(new Point(int.MinValue, int.MinValue), null) {
			}
			public override object HitResult { get { return null; } }
		}
		protected new internal LayoutElementHitInfo Patch(Point point) {
			if(Element != null)
				CheckAndSetHitTest(Element.Bounds, point, LayoutElementHitTest.Bounds);
			return base.Patch(point) as LayoutElementHitInfo;
		}
	}
}
