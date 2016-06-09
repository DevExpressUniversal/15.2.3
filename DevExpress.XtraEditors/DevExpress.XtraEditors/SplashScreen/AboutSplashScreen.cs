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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.About;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing.Animation;
using System.ComponentModel;
namespace DevExpress.XtraSplashScreen {
	#region About Splash Screen Manager
	[ToolboxItem(false)]
	public class AboutSplashScreenManager : SplashScreenManager {
		IAboutDialogManager aboutInfo;
		public AboutSplashScreenManager(IAboutDialogManager aboutInfo, Form parentForm, Type splashFormType, bool useFadeIn, bool useFadeOut)
			: base(parentForm, splashFormType, useFadeIn, useFadeOut){
			this.aboutInfo = aboutInfo;
			ShowCore();
		}
		protected override ThreadManagerBase CreateThreadManager() {
			return new AboutSplashScreenThreadManager(AboutInfo, this);
		}
		public static void ShowAbout(IAboutDialogManager aboutInfo, Form parent) {
			if(DesignTimeTools.IsDesignMode)
				return;
			if(Default != null) {
				throw new InvalidOperationException("Splash Form has already been displayed");
			}
			Default = new AboutSplashScreenManager(aboutInfo, parent, null, true, true);
		}
		public static void CloseAbout() {
			if(DesignTimeTools.IsDesignMode)
				return;
			if(Default == null) {
				throw new InvalidOperationException("Splash Form is not displayed");
			}
			Default.CloseCore(0, null);
			Default = null;
		}
		protected internal override bool IsReadyForDisplaying {
			get { return true; }
		}
		protected internal override bool IsNeedAutoDisplaying {
			get { return false; }
		}
		public IAboutDialogManager AboutInfo { get { return aboutInfo; } }
	}
	class AboutSplashScreenThreadManager : SplashScreenThreadManager, ISupportXtraAnimation, AboutSplashFormFadeAnimationInfo.ISupportOpacityChanging {
		IAboutDialogManager aboutInfo;
		public static readonly object AnimationId = new object();
		public AboutSplashScreenThreadManager(IAboutDialogManager aboutInfo, AboutSplashScreenManager manager)
			: base(manager) {
			this.aboutInfo = aboutInfo;
		}
		protected override Form CreateForm() {
			Form form = Activator.CreateInstance(AboutInfo.AboutDialogType, AboutInfo.ProductInfo) as Form;
			AboutInfo.ApplySettings(form);
			if(AboutInfo.ShouldDisplayAnimation) form.Opacity = 0;
			return form;
		}
		#region Animation
		protected virtual void AddFadeAnimation() {
			if(!AboutInfo.ShouldDisplayAnimation) return;
			RemoveFadeAnimation();
			AboutSplashFormFadeAnimationInfo info = new AboutSplashFormFadeAnimationInfo(this, this, 0, 1, FadeAnimationLength);
			XtraAnimator.Current.AddAnimation(info);
		}
		protected virtual void RemoveFadeAnimation() {
			if(!AboutInfo.ShouldDisplayAnimation) return;
			if(XtraAnimator.Current.Get(this, AnimationId) == null)
				return;
			XtraAnimator.Current.Animations.Remove(this, AnimationId);
		}
		protected virtual int FadeAnimationLength {
			get { return 400; }
		}
		bool formActive = false;
		protected override void OnFormDisplaying() {
			base.OnFormDisplaying();
			formActive = true;
			AddFadeAnimation();
		}
		protected override void OnFormClosed() {
			base.OnFormClosed();
			formActive = false;
			RemoveFadeAnimation();
		}
		#endregion
		#region ISupportXtraAnimation Members
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return Form; }
		}
		#endregion
		#region ISupportOpacityChanging Members
		void AboutSplashFormFadeAnimationInfo.ISupportOpacityChanging.SetOpacity(double value) {
			if(Form == null || !formActive)
				return;
			Form.Opacity = value;
		}
		#endregion
		#region Disposing
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RemoveFadeAnimation();
			}
			base.Dispose(disposing);
		}
		#endregion
		public IAboutDialogManager AboutInfo { get { return aboutInfo; } }
	}
	class AboutSplashFormFadeAnimationInfo : BaseAnimationInfo {
		double min, max, current;
		ISupportOpacityChanging opacitySupportObj;
		public AboutSplashFormFadeAnimationInfo(AboutSplashScreenThreadManager manager, ISupportOpacityChanging imp, double min, double max, int length)
			: base(manager, AboutSplashScreenThreadManager.AnimationId, 10, (int)(TimeSpan.TicksPerMillisecond * length / 10)) {
			this.min = min;
			this.max = max;
			this.opacitySupportObj = imp;
		}
		public override void FrameStep() {
			double k = ((double)(CurrentFrame)) / FrameCount;
			this.current = Min + (Max - Min) * k;
			if(IsFinalFrame) {
				this.current = Max;
			}
			OpacitySupportObj.SetOpacity(Current);
		}
		public double Min { get { return this.min; } }
		public double Max { get { return this.max; } }
		public double Current { get { return this.current; } }
		public ISupportOpacityChanging OpacitySupportObj { get { return opacitySupportObj; } }
		#region ISupportOpacityChanging
		public interface ISupportOpacityChanging {
			void SetOpacity(double value);
		}
		#endregion
	}
	#endregion
	public interface IAboutDialogManager {
		Type AboutDialogType { get; }
		ProductInfo ProductInfo { get; }
		void ApplySettings(Form form);
		bool ShouldDisplayAnimation { get; }
	}
	public abstract class AboutDialogManagerBase : IAboutDialogManager {
		ProductInfo productInfo;
		public AboutDialogManagerBase() {
			this.productInfo = CreateProductInfo();
		}
		#region IAboutDialogManager Members
		Type IAboutDialogManager.AboutDialogType {
			get { return CreateAboutDialogType(); }
		}
		ProductInfo IAboutDialogManager.ProductInfo {
			get { return CreateProductInfo(); }
		}
		void IAboutDialogManager.ApplySettings(Form form) {
			ApplyAboutFormSettings(form);
		}
		bool IAboutDialogManager.ShouldDisplayAnimation {
			get { return ShouldDisplayAnimationCore; }
		}
		#endregion
		protected virtual Type CreateAboutDialogType() {
			return typeof(SplashAboutDialog);
		}
		protected virtual void ApplyAboutFormSettings(Form form) {
			form.ShowInTaskbar = false;
		}
		protected virtual bool ShouldDisplayAnimationCore {
			get { return true; }
		}
		public abstract ProductInfo CreateProductInfo();
	}
}
