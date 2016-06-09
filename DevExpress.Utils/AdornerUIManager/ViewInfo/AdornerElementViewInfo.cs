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
using DevExpress.Utils.Drawing;
using System.Drawing;
namespace DevExpress.Utils.VisualEffects {
	public abstract class AdornerElementViewInfo : ObjectInfoArgs, IAdornerElementViewInfo {
		bool isReady = false, isDisposing = false;
		IAdornerElement elementCore;
		AppearanceObject paintAppearanceCore;
		public AdornerElementViewInfo(IAdornerElement element) { elementCore = element; }
		#region IAdornerElementViewInfo Members
		public IAdornerElement Element { get { return elementCore; } }
		public bool IsReady { get { return isReady; } }
		public void SetDirty() { isReady = false; }
		public Size CalcMinSize(Graphics g) {
			if(Element == null) return Size.Empty;
			return CalcMinSizeCore(g, Element.Painter);
		}
		public AppearanceObject PaintAppearance {
			get {
				if(paintAppearanceCore == null)
					paintAppearanceCore = new FrozenAppearance();
				return paintAppearanceCore;
			}
		}
		IEnumerable<Rectangle> IAdornerElementViewInfo.CalcRegions() { return GetRegions(); }
		void IAdornerElementViewInfo.Calc(Graphics g, Rectangle targetElementBounds) {
			if(IsReady) return;
			Bounds = CalcBounds(g, targetElementBounds);
			CalcContent(g, Bounds);
			isReady = true;
		}
		protected virtual Size CalcMinSizeCore(Graphics g, AdornerElementPainter painter) { return ObjectPainter.CalcObjectMinBounds(g, painter, this).Size; }
		protected virtual IEnumerable<Rectangle> GetRegions() { return new Rectangle[] { Bounds }; }
		protected abstract void CalcContent(Graphics g, Rectangle bounds);
		protected abstract Rectangle CalcBounds(Graphics g, Rectangle controlBounds);
		#endregion
		void IDisposable.Dispose() {
			if(!isDisposing) {
				isDisposing = true;
				OnDispose();
			}
		}
		protected virtual void OnDispose() {
			elementCore = null;
			Ref.Dispose(ref paintAppearanceCore);
		}
	}
}
