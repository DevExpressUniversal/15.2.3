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
using DevExpress.Utils.Animation;
namespace DevExpress.Utils.Animation {
	class WaitingTransitionPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			TransitionInfoArgs args = e as TransitionInfoArgs;
			args.Cache.Paint.DrawImage(args.Cache.Graphics, args.ImageStart, args.Bounds.Location);
			if(args.WaitingInfo == null) return;
			args.WaitingInfo.Painter.DrawObject(args);
		}
	}
	class TransitionPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			TransitionInfoArgs args = e as TransitionInfoArgs;
			if(args.TransitionAnimator == null) return;
			if(args.SkinInfo != null)
				args.TransitionAnimator.SetSkinProvider(args.SkinInfo.LookAndFeel);
			args.TransitionAnimator.DrawAnimatedItem(args.Cache, args.Bounds);
		}
	}
	class TransitionOpaquePainter : AdornerOpaquePainter {
		public override void DrawObject(ObjectInfoArgs e) {
			IAsyncAdornerElementInfoArgs ea = e as IAsyncAdornerElementInfoArgs;
			if(ea == null) return;
			ObjectPainter.DrawObject(e.Cache, ea.GetPainter(), ea.InfoArgs);
		}
	}	
}
