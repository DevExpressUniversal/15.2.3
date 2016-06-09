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
using System.Windows.Forms;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Internal;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class TransitionScreenAdornerInfoArgs : AsyncAdornerElementInfoArgs {
		TransitionScreenAdornerInfoArgs(WindowsUIView owner)
			: base(owner) {
		}
		protected sealed override IntPtr hWndInsertAfter {
			get { return new IntPtr(-2); }
		}
		protected sealed override ObjectPainter GetPainter() {
			return ((WindowsUIViewPainter)Owner.Painter).GetTransitionScreenPainter();
		}
		public Image From { get; set; }
		public Image To { get; set; }
		public AnimationParameters Parameters { get; set; }
		public new WindowsUIView Owner {
			get { return base.Owner as WindowsUIView; }
		}
		DevExpress.Utils.Animation.ITransitionAnimator animatorCore;
		public DevExpress.Utils.Animation.ITransitionAnimator TransitionAnimator {
			get { return animatorCore; }
		}
		IntPtr adornerHandle;
		protected sealed override void BeginAsync(IntPtr adornerHandle) {
			this.adornerHandle = adornerHandle;
			animatorCore = CreareTransitionAnimator();
			TransitionAnimator.Invalidate += TransitionAnimator_Invalidate;
			TransitionAnimator.Complete += TransitionAnimator_Complete;
		}
		protected sealed override void EndAsync() {
			DestroyCore();
		}
		void TransitionAnimator_Complete(object sender, EventArgs e) {
			LayeredWindowMessanger.PostClose(adornerHandle);
		}
		void TransitionAnimator_Invalidate(object sender, EventArgs e) {
			LayeredWindowMessanger.PostInvalidate(adornerHandle);
		}
		protected sealed override void Destroy() {
			LayeredWindowMessanger.PostClose(adornerHandle);
		}
		protected virtual void DestroyCore() {
			if(TransitionAnimator == null) return;
			TransitionAnimator.Complete -= TransitionAnimator_Complete;
			TransitionAnimator.Invalidate -= TransitionAnimator_Invalidate;
			Ref.Dispose(ref animatorCore);
		}
		protected virtual DevExpress.Utils.Animation.ITransitionAnimator CreareTransitionAnimator() {
			switch(Parameters.Type) {
				case TransitionAnimation.HorizontalSlide:
					return new HorizontalSlideTransitionAnimator(From, To, Parameters);
				case TransitionAnimation.VerticalSlide:
					return new VerticalSlideTransitionAnimator(From, To, Parameters);
				case TransitionAnimation.FadeIn:
					return new FadeInTransitionAnimator(From, To, Parameters);
				case TransitionAnimation.SegmentedFade:
					return new SegmentedFadeTransitionAnimator(From, To, Parameters);
				case TransitionAnimation.RandomSegmentedFade:
					return new RandomSegmentedFadeTransitionAnimator(From, To, Parameters);
				default:
					throw new NotSupportedException(Parameters.Type.ToString());
			}
		}
		public static TransitionScreenAdornerInfoArgs EnsureInfoArgs(ref AsyncAdornerElementInfo target, WindowsUIView owner, Document from, Document to) {
			TransitionScreenAdornerInfoArgs args;
			if(target == null) {
				args = new TransitionScreenAdornerInfoArgs(owner);
				target = new AsyncAdornerElementInfo(new AsyncAdornerOpaquePainter(), args);
			}
			else args = target.InfoArgs as TransitionScreenAdornerInfoArgs;
			args.Bounds = owner.GetBounds(from);
			args.From = GetVisibleControlImage(from.Control);
			DevExpress.Utils.Mdi.MdiChildHelper.ResizeControlOutOfTheView(
				to.IsNonMdi ? to.Control.Parent : (to.IsMdiForm ? to.Control : to.Form), args.Bounds);
			args.To = DevExpress.Utils.Mdi.MDIChildPreview.Create(to.Control, Color.Empty);
			args.SetDirty();
			return args;
		}
		static Image GetVisibleControlImage(Control ctrl) {
			using(Graphics graphic = ctrl.CreateGraphics()) {
				Bitmap memImage = new Bitmap(ctrl.Width, ctrl.Height, graphic);
				using(Graphics memGraphic = Graphics.FromImage(memImage)) {
					IntPtr sourceDC = graphic.GetHdc();
					IntPtr targetDC = memGraphic.GetHdc();
					DevExpress.Utils.Drawing.Helpers.NativeMethods.BitBlt(targetDC, 0, 0, ctrl.ClientRectangle.Width,
						ctrl.ClientRectangle.Height, sourceDC, 0, 0, 0xCC0020);
					graphic.ReleaseHdc(sourceDC);
					memGraphic.ReleaseHdc(targetDC);
				}
				return memImage;
			}
		}
	}
	public class TransitionScreenPainter : ObjectPainter {
		public sealed override void DrawObject(ObjectInfoArgs e) {
			TransitionScreenAdornerInfoArgs info = e as TransitionScreenAdornerInfoArgs;
			info.TransitionAnimator.DrawAnimatedItem(e.Cache, e.Bounds);
		}
	}
	public class TransitionScreenSkinPainter : TransitionScreenPainter {
		ISkinProvider provider;
		public TransitionScreenSkinPainter(ISkinProvider provider) {
			this.provider = provider;
		}
	}
}
