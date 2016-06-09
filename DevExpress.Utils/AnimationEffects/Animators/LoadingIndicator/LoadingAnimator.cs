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
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using System.Drawing.Imaging;
namespace DevExpress.Utils.Animation {
	public interface ILoadingAnimator : IAnimator {
		Size GetMinSize();
		void DrawAnimatedItem(GraphicsCache cache, Rectangle bounds, Color color);
	}
	public class LoadingAnimator : BaseLoadingAnimator, ILoadingAnimator {
		public LoadingAnimator(Image image) : base(image) { }
		public LoadingAnimator(Image image, bool disposeImage) : base(image, disposeImage) { }
		protected override void OnStop() {
			base.OnStop();
			RaiseInvalidate();
		}
		protected override void OnImageAnimation(BaseAnimationInfo info) {
			if(!AnimationInProgress) return;
			if(!info.IsFinalFrame) {
				ImageHelper.Image.SelectActiveFrame(FrameDimension.Time, info.CurrentFrame);
				RaiseInvalidate();
			}
			else RestartAnimation();
		}
		public event EventHandler Invalidate;
		protected void RaiseInvalidate() {
			if(Invalidate != null)
				Invalidate(this, EventArgs.Empty);
		}
		#region ILoadingAnimator Members
		void ILoadingAnimator.DrawAnimatedItem(GraphicsCache cache, Rectangle bounds, Color color) {
			DrawAnimatedItem(cache, bounds);
		}
		Size ILoadingAnimator.GetMinSize() { return Image.Size; }
		#endregion
	}	
}
