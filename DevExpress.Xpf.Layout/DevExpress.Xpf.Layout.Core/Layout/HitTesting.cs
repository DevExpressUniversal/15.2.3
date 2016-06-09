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
using System.Windows;
namespace DevExpress.Xpf.Layout.Core {
	public abstract class HitInfo<HitType> {
		Point hitPointCore;
		HitType hitTestCore;
		object hitResultCore;
		public HitInfo(Point pt) {
			hitPointCore = pt;
			hitTestCore = default(HitType);
		}
		public Point HitPoint {
			get { return hitPointCore; }
		}
		public HitType HitTest {
			get { return hitTestCore; }
		}
		public virtual object HitResult {
			get { return hitResultCore; }
		}
		public bool CheckAndSetHitTest(Rect bounds, Point pt, HitType hitTest) {
			bool fContains = !bounds.IsEmpty && bounds.Contains(pt);
			if(fContains) {
				this.hitTestCore = hitTest;
			}
			return fContains;
		}
		public bool CheckAndSetHitTest(object expected, object hitResult, HitType hitTest) {
			bool fContains = object.Equals(hitResult, expected);
			if(fContains) {
				this.hitTestCore = hitTest;
				this.hitResultCore = hitResult;
			}
			return fContains;
		}
		public abstract bool IsEmpty { get; }
		public virtual bool IsHot {
			get { return Array.IndexOf(GetValidHotTests(), hitTestCore) != -1; }
		}
		public virtual bool IsPressed {
			get { return Array.IndexOf(GetValidPressedTests(), hitTestCore) != -1; }
		}
		public bool IsEqual(HitInfo<HitType> hi) {
			return (hi != null) && object.Equals(hi.hitTestCore, hitTestCore);
		}
		protected virtual HitType[] GetValidHotTests() { return EmptyTests; }
		protected virtual HitType[] GetValidPressedTests() { return EmptyTests; }
		protected static HitType[] EmptyTests = new HitType[0];
	}
	public enum LayoutElementHitTest {
		None, Bounds, Content, Header, Border, ControlBox
	}
	public class LayoutElementHitInfo : HitInfo<LayoutElementHitTest> {
		readonly BaseLayoutElement elementCore;
		public LayoutElementHitInfo(Point point, BaseLayoutElement element)
			: base(point) {
			if(element != null && CheckAndSetHitTest(element.Bounds, point, LayoutElementHitTest.Bounds)) {
				elementCore = element;
			}
		}
		public ILayoutElement Element {
			get { return elementCore; }
		}
		public object Tag { get; set; }
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
			get { return InBorder; }
		}
		public virtual bool InMenuBounds {
			get { return InHeader; }
		}
		public virtual bool InClickBounds {
			get { return InControlBox || InHeader; }
		}
		public virtual bool InClickPreviewBounds {
			get { return InBounds; }
		}
		public virtual bool InDoubleClickBounds {
			get { return InHeader; }
		}
		public bool IsDragging {
			get { return (elementCore != null) && elementCore.IsDragging; }
		}
		protected override LayoutElementHitTest[] GetValidPressedTests() {
			return new LayoutElementHitTest[] { LayoutElementHitTest.ControlBox, LayoutElementHitTest.Header };
		}
		protected override LayoutElementHitTest[] GetValidHotTests() {
			return new LayoutElementHitTest[] { LayoutElementHitTest.ControlBox };
		}
		public static readonly LayoutElementHitInfo Empty = new EmptyHitInfo();
		class EmptyHitInfo : LayoutElementHitInfo {
			public EmptyHitInfo()
				: base(new Point(double.NaN, double.NaN), null) {
			}
			public override object HitResult { get { return null; } }
		}
	}
}
