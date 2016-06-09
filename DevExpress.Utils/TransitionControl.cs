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
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Drawing.Animation;
using System.ComponentModel;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
namespace DevExpress.Utils.Native {
	[ToolboxItem(false)]
	public class TransitionControl : Control {
		private static readonly object contentDidAppear = new object();
		public TransitionControl() {
			TransitionEffect = TransitionEffect.SwipeLeft;
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
		}
		Control content;
		public Control Content {
			get { return content; }
			set {
				if(Content == value)
					return;
				Control prev = Content;
				content = value;
				OnContentChanged(prev, Content);
			}
		}
		public Control PrevContent { get; set; }
		public TransitionEffect TransitionEffect { get; set; }
		protected virtual void OnContentChanged(Control prev, Control next) {
			PrevContent = prev;
			if(PrevContent == null || next == null) {
				if(next != null) {
					Controls.Add(next);
				}
				RaiseContentDidAppear();
				return;
			}
			Client.PrepareContentChanged(prev);
			Client.Bounds = ClientRectangle;
			Controls.Add(Client);
			Client.BringToFront();
			Client.OnContentChanged(next);
		}
		TransitionClientControl clientControl;
		protected TransitionClientControl Client {
			get {
				if(clientControl == null) {
					clientControl = new TransitionClientControl(this);
				}
				return clientControl;
			}
		}
		void RaiseContentDidAppear() {
			EventHandler handler = Events[contentDidAppear] as EventHandler;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		public event EventHandler ContentDidAppear {
			add { Events.AddHandler(contentDidAppear, value); }
			remove { Events.RemoveHandler(contentDidAppear, value); }
		}
		protected internal virtual void OnContentDidAppear(BaseAnimationInfo info) {
			Controls.Add(Content);
			Controls.Remove(PrevContent);
			Controls.Remove(Client);
			RaiseContentDidAppear();
		}
	}
	public enum TransitionEffect { SwipeLeft, SwipeRight };
	[ToolboxItem(false)]
	public class TransitionClientControl : Control, ISupportXtraAnimationEx {
		protected static int AnimationLength = 600;
		private readonly object animationId = new object();
		public TransitionClientControl(TransitionControl control) {
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			TransitionControl = control;
		}
		protected TransitionControl TransitionControl {
			get; private set;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			DoubleSplineAnimationInfo animInfo = XtraAnimator.Current.Get(this, this.animationId) as DoubleSplineAnimationInfo;
			if(animInfo == null) {
				if(PrevContentImage != null)
					e.Graphics.DrawImage(PrevContentImage, Point.Empty);
				return;
			}
			if(TransitionControl.TransitionEffect == TransitionEffect.SwipeLeft || TransitionControl.TransitionEffect == TransitionEffect.SwipeRight) {
				DrawSwipeTransition(animInfo, e);
				return;
			}
		}
		protected internal virtual void PrepareContentChanged(Control prev) {
			PrevContentImage = CreateImage(PrevContentImage, prev, prev.Bounds);
		}
		protected internal virtual void OnContentChanged(Control next) {
			ContentImage = CreateImage(ContentImage, next, Bounds);
			AddContentChangeAnimation();
		}
		protected Rectangle CalcPrevImageScreenBounds(double value) {
			Rectangle rect = ClientRectangle;
			rect.X = (int)value;
			return rect;
		}
		protected Rectangle CalcImageScreenBounds(double value) {
			Rectangle rect = ClientRectangle;
			if(TransitionControl.TransitionEffect == TransitionEffect.SwipeLeft)
				rect.X = (int)value + rect.Width;
			else
				rect.X = (int)value - rect.Width;
			return rect;
		}
		protected Rectangle CalcImageBounds(Rectangle screenBounds) {
			Rectangle rect = new Rectangle(Point.Empty, screenBounds.Size);
			if(screenBounds.Right < ClientRectangle.X || screenBounds.X > ClientRectangle.Right)
				return Rectangle.Empty;
			else if(screenBounds.X < ClientRectangle.X) {
				rect.Width -= ClientRectangle.X - screenBounds.X;
				rect.X = ClientRectangle.X - screenBounds.X;
			} else if(screenBounds.Right > ClientRectangle.Right) {
				rect.Width -= screenBounds.Right - ClientRectangle.Right;
			}
			if(rect.Width > ClientRectangle.Width)
				rect.Width = ClientRectangle.Width;
			return rect;
		}
		protected virtual void DrawSwipeTransition(DoubleSplineAnimationInfo animInfo, PaintEventArgs e) {
			Rectangle prevScreenBounds = CalcPrevImageScreenBounds(animInfo.Value);
			Rectangle screenBounds = CalcImageScreenBounds(animInfo.Value);
			Rectangle prevImageBounds = CalcImageBounds(prevScreenBounds);
			Rectangle imageBounds = CalcImageBounds(screenBounds);
			prevScreenBounds.Intersect(ClientRectangle);
			screenBounds.Intersect(ClientRectangle);
			if(!prevImageBounds.IsEmpty) {
				e.Graphics.DrawImage(PrevContentImage, prevScreenBounds, prevImageBounds, GraphicsUnit.Pixel);
			}
			if(!imageBounds.IsEmpty) {
				e.Graphics.DrawImage(ContentImage, screenBounds, imageBounds, GraphicsUnit.Pixel);
			}
		}
		protected virtual void AddContentChangeAnimation() {
			double end;
			if(TransitionControl.TransitionEffect == TransitionEffect.SwipeLeft)
				end = -Width;
			else
				end = Width;
			BaseAnimationInfo prev = XtraAnimator.Current.Get(this, this.animationId);
			if(prev != null)
				XtraAnimator.Current.Animations.Remove(prev);
			DoubleSplineAnimationInfo animInfo = new DoubleSplineAnimationInfo(this, this.animationId, 0, end, AnimationLength);
			XtraAnimator.Current.AddAnimation(animInfo);
		}
		protected virtual Image CreateImage(Image image, Control control, Rectangle bounds) {
			if(image == null || image.Size != bounds.Size) {
				if(image != null)
					image.Dispose();
				image = new Bitmap(bounds.Width, bounds.Height);
			}
			control.Size = bounds.Size;
			using(Graphics graph = Graphics.FromImage(image)) {
				int PRF_CLIENT = 0x04, PRF_CHILDREN = 0x10;
				DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(control.Handle, MSG.WM_PRINT, graph.GetHdc(), new IntPtr(PRF_CHILDREN | PRF_CLIENT));
				graph.ReleaseHdc();
			}
			return image;
		}
		Image prevContentImage;
		protected Image PrevContentImage {
			get { return prevContentImage; }
			set {
				if(PrevContentImage == value)
					return;
				Image prev = PrevContentImage;
				prevContentImage = value;
				OnImageChanged(prev, PrevContentImage);
			}
		}
		Image contentImage;
		protected Image ContentImage {
			get { return contentImage; }
			set {
				if(ContentImage == value)
					return;
				Image prev = ContentImage;
				contentImage = value;
				OnImageChanged(prev, ContentImage);
			}
		}
		protected virtual void OnImageChanged(Image prev, Image next) {
			if(prev != null)
				prev.Dispose();
		}
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return this; }
		}
		#endregion
		#region ISupportXtraAnimationEx Members
		void ISupportXtraAnimationEx.OnFrameStep(BaseAnimationInfo info) {
			Invalidate();
		}
		void ISupportXtraAnimationEx.OnEndAnimation(BaseAnimationInfo info) {
			TransitionControl.OnContentDidAppear(info);
		}
		#endregion
	}
}
