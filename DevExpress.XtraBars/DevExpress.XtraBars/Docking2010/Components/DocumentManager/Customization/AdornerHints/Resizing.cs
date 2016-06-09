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
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Docking2010.Customization {
	class SplitAdornerPainter : AdornerPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			SplitAdornerInfoArgs ea = e as SplitAdornerInfoArgs;
			ea.Appearance.FillRectangle(e.Cache, e.Bounds);
		}
	}
	class SplitAdornerInfoArgs : AdornerElementInfoArgs {
		AppearanceObject appearanceCore;
		public SplitAdornerInfoArgs() {
			appearanceCore = new FrozenAppearance();
			Appearance.BackColor = Color.FromArgb(0xff, 0x57, 0x79, 0xAD);
		}
		public AppearanceObject Appearance {
			get { return appearanceCore; }
		}
		protected override int CalcCore() { return 0; }
		protected override System.Collections.Generic.IEnumerable<Rectangle> GetRegionsCore(bool opaque) {
			return opaque ? new Rectangle[0] : new Rectangle[] { Bounds };
		}
	}
}
