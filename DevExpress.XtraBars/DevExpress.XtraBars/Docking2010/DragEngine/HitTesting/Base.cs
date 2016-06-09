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
namespace DevExpress.XtraBars.Docking2010.DragEngine {
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
		[System.Diagnostics.DebuggerStepThrough]
		protected internal bool CheckAndSetHitTest(Rectangle bounds, Point pt, HitType hitTest) {
			bool fContains = !bounds.IsEmpty && BaseLayoutElement.Include(bounds, pt);
			if(fContains) {
				this.hitTestCore = hitTest;
			}
			return fContains;
		}
		[System.Diagnostics.DebuggerStepThrough]
		protected internal bool CheckAndSetHitTest(object expected, object hitResult, HitType hitTest) {
			bool fContains = object.Equals(hitResult, expected);
			if(fContains) {
				this.hitTestCore = hitTest;
				this.hitResultCore = hitResult;
			}
			return fContains;
		}
		public abstract bool IsEmpty { get; }
		public virtual bool IsHot {
			[System.Diagnostics.DebuggerStepThrough]
			get { return Array.IndexOf(GetValidHotTests(), hitTestCore) != -1; }
		}
		public virtual bool IsPressed {
			[System.Diagnostics.DebuggerStepThrough]
			get { return Array.IndexOf(GetValidPressedTests(), hitTestCore) != -1; }
		}
		public bool IsEqual(HitInfo<HitType> hi) {
			return (hi != null) && object.Equals(hi.hitTestCore, hitTestCore);
		}
		protected virtual HitType[] GetValidHotTests() { return EmptyTests; }
		protected virtual HitType[] GetValidPressedTests() { return EmptyTests; }
		protected static HitType[] EmptyTests = new HitType[0];
		protected HitInfo<HitType> Patch(Point point) {
			hitPointCore = point;
			return this;
		}
	}
}
