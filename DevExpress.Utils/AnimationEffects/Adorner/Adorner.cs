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

using System.Collections.Generic;
using System.Drawing;
using DevExpress.Utils.Drawing;
namespace DevExpress.Utils.Animation {
	public abstract class BaseAdornerElementInfoArgs : ObjectInfoArgs, IAdornerElementInfoArgs {
		int state = -1;
		bool isReadyCore, stateShiftModifier;
		public IEnumerable<Rectangle> CalculateRegions(bool opaque) {
			Calc();
			return GetRegionsCore(opaque);
		}
		protected abstract IEnumerable<Rectangle> GetRegionsCore(bool opaque);
		protected abstract int CalcCore();		
		public bool IsReady { get { return isReadyCore; } }
		public void SetDirty() { isReadyCore = false; }		
		public bool Calc() {
			int lastState = state;
			stateShiftModifier = CanModifierShift();
			if(!isReadyCore || StateShiftModifier) {
				state = CalcCore();
				isReadyCore = true;
			}
			return lastState != state || StateShiftModifier;
		}
		protected virtual bool CanModifierShift() { return false; }
		protected bool StateShiftModifier { get { return stateShiftModifier; } }
		protected static int CalcState(int a, int b, int c) {
			return (a << 0x10) + (b << 0x08) + c;
		}
		#region IAdornerElementInfoArgs Members
		BaseAdornerElementInfoArgs IAdornerElementInfoArgs.InfoArgs { get { return this; } }
		#endregion
	}
	public abstract class AdornerPainter : ObjectPainter {
	}
	public abstract class AdornerOpaquePainter : ObjectPainter {
	}
	public class BaseAdornerElementInfo {
		public BaseAdornerElementInfo(AdornerOpaquePainter opaquePainter, BaseAdornerElementInfoArgs info) {			
			opaquePainterCore = opaquePainter;
			infoArgsCore = info;
		} 
		AdornerOpaquePainter opaquePainterCore;
		public AdornerOpaquePainter OpaquePainter {
			get { return opaquePainterCore; }
		}
		BaseAdornerElementInfoArgs infoArgsCore;
		public BaseAdornerElementInfoArgs InfoArgs {
			get { return infoArgsCore; }
		}
	}
}
