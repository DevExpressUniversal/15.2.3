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

using DevExpress.Utils.Drawing;
namespace DevExpress.Utils.VisualEffects {
	public abstract class AdornerElementPainter : ObjectPainter {
		AppearanceDefault defaultAppearanceCore;
		public sealed override AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearanceCore == null)
					defaultAppearanceCore = CreateDefaultAppearance();
				return defaultAppearanceCore;
			}
		}
		protected abstract AppearanceDefault CreateDefaultAppearance();
		internal void ResetDefaultAppearance() { defaultAppearanceCore = null; }
	}
	public abstract class AdornerElementPainter<T> : AdornerElementPainter where T : AdornerElementViewInfo {
		public override void DrawObject(ObjectInfoArgs e) {
			T args = e as T;
			if(args == null) return;
			DrawBackground(args);
			DrawContent(args);
		}
		protected virtual void DrawContent(T e) { }
		protected virtual void DrawBackground(T e) {
			e.PaintAppearance.DrawBackground(e.Cache, e.Bounds);
		}
	}
}
