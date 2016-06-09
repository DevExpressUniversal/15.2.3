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
using System.Linq;
using System.Text;
using DevExpress.Utils;
using DevExpress.Utils.About;
using System.Drawing;
using DevExpress.XtraEditors;
namespace DevExpress.XtraSplashScreen {
	public class SplashAboutDialog : AboutForm12 {
		const int labelIndent = 21;
		const int progressBarIndent = 85;
		const int progressBarHeight = 18;
		const int progressBarHorzOffset = 30;
		public SplashAboutDialog(ProductInfo productInfo)
			: base(productInfo) {
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			CreateProgressBar();
		}
		protected virtual void CreateProgressBar() {
			LabelHtmlText info = new LabelHtmlText();
			info.Text = GetInfoLabelText();
			info.Bounds = new Rectangle(ProductInfoRect.X, ProductInfoRect.Bottom + labelIndent, ProductInfoRect.Width, info.AppearanceFontHeight);
			info.Parent = this;
			info.Calc();
			MarqueeProgressBarControl progressBar = new MarqueeProgressBarControl();
			progressBar.LookAndFeel.UseDefaultLookAndFeel = false;
			progressBar.LookAndFeel.SetSkinStyle("Metropolis");
			progressBar.Bounds = new Rectangle(ProductInfoRect.X + progressBarHorzOffset, ProductInfoRect.Bottom + progressBarIndent, ProductInfoRect.Width - 2 * progressBarHorzOffset, progressBarHeight);
			progressBar.Parent = this;
		}
		protected override void CreateLabels() {
		}
		protected override void CreateLinks() {
		}
		static Color logoBackColor1 = Color.FromArgb(0x3C, 0x3C, 0x3C);
		protected override Color LogoBackColor1 {
			get { return logoBackColor1; }
		}
		protected override bool AllowMouseEvents { get { return false; } }
	}
}
