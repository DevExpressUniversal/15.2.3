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

using System.Windows.Forms;
using System;
using DevExpress.XtraEditors;
using DevExpress.Utils.About;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.Utils.CodedUISupport {
	[ToolboxItem(false), EditorBrowsable(EditorBrowsableState.Never)]
	internal class CUITExtensionAboutForm12 : AboutForm12 {
		enum AnimationTimerTag {
			Showing,
			Closing
		}
		const string CUITExtensionProductName = "CUIT Extension for DevExpress WinForms Controls";
		const int AnimationTimerInterval = 30;
		const double OpacityInterval = 0.05;
		CUITExtensionAboutForm12(bool isTrialExpired)
			: base(new About.ProductInfo(CUITExtensionProductName, typeof(XtraForm), About.ProductKind.DXperienceUni, isTrialExpired ? About.ProductInfoStage.Trial : About.ProductInfoStage.Registered)) {
			this.Shown += new EventHandler(CUITExtensionAboutForm12_Shown);
			this.MouseDown += new MouseEventHandler(CUITExtensionAboutForm12_MouseDown);
			this.Load += new EventHandler(CUITExtensionAboutForm12_Load);
		}
		internal static void ShowInNewThread(object isTrialExpired) {
			Application.Run(new CUITExtensionAboutForm12((bool)isTrialExpired));
		}
		void CUITExtensionAboutForm12_Load(object sender, EventArgs e) {
			this.Left = SystemInformation.PrimaryMonitorSize.Width - this.Width;
			this.Opacity = 0.04;
		}
		void CUITExtensionAboutForm12_Shown(object sender, EventArgs e) {
			StartCloseTimer(7000);
			StartAnimationTimer(AnimationTimerTag.Showing);
		}
		Timer closeTimer;
		void StartCloseTimer(int lifeTime) {
			closeTimer = new Timer();
			closeTimer.Tick += new EventHandler(closeTimer_Tick);
			closeTimer.Interval = lifeTime;
			closeTimer.Start();
		}
		void StartAnimationTimer(AnimationTimerTag tag) {
			Timer animationTimer = new Timer();
			animationTimer.Tick += new EventHandler(animationTimer_Tick);
			animationTimer.Interval = AnimationTimerInterval;
			animationTimer.Tag = tag;
			animationTimer.Start();
		}
		void animationTimer_Tick(object sender, EventArgs e) {
			if((AnimationTimerTag)(sender as Timer).Tag == AnimationTimerTag.Showing) {
				if(this.Opacity < 0.95)
					this.Opacity += OpacityInterval;
				else (sender as Timer).Dispose();
			}
			else {
				if(this.Opacity > 0)
					this.Opacity -= OpacityInterval;
				else this.Dispose();
			}
		}
		void CUITExtensionAboutForm12_MouseDown(object sender, MouseEventArgs e) {
			DisposeCloseTimer();
		}
		void closeTimer_Tick(object sender, EventArgs e) {
			DisposeCloseTimer();
			StartAnimationTimer(AnimationTimerTag.Closing);
		}
		void DisposeCloseTimer() {
			if(closeTimer != null)
				closeTimer.Dispose();
		}
		protected internal override About.ProductStringInfo GetProductInfo(ProductInfo info) {
			string dx = "DevExpress";
			return new ProductStringInfo(dx, CUITExtensionProductName);
		}
		protected internal override string GetInfoLabelText() {
			if(this.info.Stage == ProductInfoStage.Trial)
				return base.GetInfoLabelText();
			else return string.Format("{0} requires DevExpress Universal Subscription", CUITExtensionProductName);
		}
		protected internal override Image LabelImage {
			get {
				if(this.info.Stage == ProductInfoStage.Trial) return Properties.Resources.Expired;
				else return Properties.Resources.UpgradeRequired;
			}
		}
	}
}
