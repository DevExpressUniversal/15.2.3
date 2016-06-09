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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.DXperience.Demos;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Frames;
using DevExpress.Utils.About;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.Tutorials.Controls;
namespace DevExpress.Tutorials {
	public partial class ucOverviewPage : TutorialControlBase {
		bool firstShow = true;
		#if !DEBUG
		#endif
		public ucOverviewPage() {
			InitializeComponent();
			peAward.Image = Awards;
			peAward.Properties.NullText = " ";
			UpdateAwardsSize();
			InitData();
			UpdateImages();
			UserLookAndFeel.Default.StyleChanged += new EventHandler(Default_StyleChanged);
			this.SizeChanged += (s, e) => { UpdateAwardsSize(); };
		}
		void UpdateAwardsSize() {
			int awardsWidth = Awards != null ? Math.Max(this.Width / 2, Awards.Width + 20) : 10;
			int awardHeight = Awards != null ? Awards.Height + 50 : Properties.Resources.Awards_main.Height;
			layoutControlItem4.MaxSize = layoutControlItem4.MinSize = new Size(awardsWidth, awardHeight);
		}
		protected virtual Image Awards { get { return Properties.Resources.Awards_main; } }
		protected virtual string Line1Text { get { return string.Empty; } }
		protected virtual string Line2Text { get { return string.Empty; } }
		protected virtual string Line3Text { get { return string.Empty; } }
		protected virtual string Line4Text { get { return string.Empty; } }
		protected virtual ProductKind ProductKind { get { return ProductKind.DXperienceWin; } }
		void Default_StyleChanged(object sender, EventArgs e) {
			UpdateImages();
		}
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			if(disposing)
				UserLookAndFeel.Default.StyleChanged -= new EventHandler(Default_StyleChanged);
			base.Dispose(disposing);
		}
		void UpdateImages() {
			peLogo.Image = DevExpress.Utils.ResourceImageHelper.CreateImageFromResources(ucAboutPage.GetLogoImageName(), typeof(ApplicationCaption).Assembly);
			lcLine2.ForeColor = lcLine4.ForeColor = CommonSkins.GetSkin(DevExpress.LookAndFeel.UserLookAndFeel.Default).Colors.GetColor("DisabledText");
			if(UserLookAndFeel.Default.ActiveSkinName == "Office 2013")
				panelControl1.BackColor = panelControl2.BackColor = panelControl3.BackColor = peAward.BackColor = lcLine3.BackColor = lcLine4.BackColor = Color.FromArgb(242, 242, 242);
			else panelControl1.BackColor = panelControl2.BackColor = panelControl3.BackColor = peAward.BackColor = lcLine3.BackColor = lcLine4.BackColor = Color.Transparent;
		}
		protected virtual ProductInfo ProductInfo {
			get {
				return new ProductInfo(string.Empty, typeof(ucOverviewPage), ProductKind, ProductInfoStage.Registered);
			}
		}
		protected bool IsTrial { get { return AboutHelper.GetSerial(ProductInfo) == AboutHelper.TrialVersion; } }
		string GetVSMText(string s) {
			string vsmString = "of Visual Studio Magazine.";
			string vsmStringCorrect = "of\r\nVisual Studio Magazine.";
			if(s.IndexOf(vsmString) > 0) s = s.Replace(vsmString, vsmStringCorrect);
			return s;
		}
		void InitData() {
			try {
				lcLine1.Font = new Font("Segoe UI Light", 36, FontStyle.Regular);
				lcLine2.Font = new Font("Segoe UI Light", 24, FontStyle.Regular);
				lcLine3.Font = new Font("Segoe UI Light", 24, FontStyle.Regular);
				lcLine4.Font = new Font("Segoe UI Light", 18, FontStyle.Regular);
			} catch { }
			lcLine1.Text = Line1Text;
			lcLine2.Text = Line2Text;
			lcLine3.Text = Line3Text;
			lcLine4.Text = GetVSMText(Line4Text);
			lcCopyright.Text = AboutHelper.CopyRightOverview;
			lcVersion.Text = string.Format("{0} {1}", Properties.Resources.Version, AssemblyInfo.Version);
			if(!IsTrial)
				lcVersion.Text += string.Format("\r\n{0}: {1}", Properties.Resources.SerialNumber, AboutHelper.GetSerial(ProductInfo));
			new OverviewButton(pcRunButton,
				DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Tutorials.Images.RunButtonNormal.png", typeof(ucAboutPage).Assembly),
				DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Tutorials.Images.RunButtonHover.png", typeof(ucAboutPage).Assembly),
				DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Tutorials.Images.RunButtonPressed.png", typeof(ucAboutPage).Assembly),
				string.Empty).ButtonClick += new EventHandler(runButton_ButtonClick);
			new OverviewButton(pcGettingStarted,
				DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Tutorials.Images.Overview-Getting-Started-Normal.png", typeof(ucAboutPage).Assembly),
				null, null, RibbonMainForm.GetStartedLink);
			new OverviewButton(pcBuyNow,
			   DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Tutorials.Images.Overview-Buy-Normal.png", typeof(ucAboutPage).Assembly),
			   null, null, AssemblyInfo.DXLinkBuyNow);
			new OverviewButton(pcFreeSupport,
			   DevExpress.Utils.ResourceImageHelper.CreateImageFromResources("DevExpress.Tutorials.Images.Overview-Support-Normal.png", typeof(ucAboutPage).Assembly),
			   null, null, AssemblyInfo.DXLinkGetSupport);
		}
		protected override void DoShow() {
			base.DoShow();
			ShowOverview(false);
		}
		string IniFileName { get { return string.Format("{0}.ini", Application.ExecutablePath); } }
		void SaveInit() {
			#if !DEBUG
			#endif
		}
		void ResetShow() {
			firstShow = false;
			SaveInit();
		}
		void CheckOpened() { 
			#if !DEBUG
			#else
			ResetShow();
			#endif
		}
		protected override void DoHide() {
			base.DoHide();
			BeginInvoke(new MethodInvoker(delegate { ShowOverview(true); }));
		}
		void ShowOverview(bool visible) {
			if(!visible && firstShow) CheckOpened();
			if(!firstShow) return;
			if(ParentFormMain != null) {
				ParentFormMain.SuspendLayout();
				if(ParentFormMain.MainPage != null)
					ParentFormMain.MainPage.Visible = visible;
				if(visible)
					ParentFormMain.ShowServiceElements();
				else
					ParentFormMain.HideServiceElements();
				ParentFormMain.ResumeLayout();
			}
			if(visible) ResetShow();
		}
		void runButton_ButtonClick(object sender, EventArgs e) {
			StartDemo();
		}
		void StartDemo() {
			RibbonMainForm form = this.FindForm() as RibbonMainForm;
			if(form != null)
				form.StartDemo();
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if(keyData == Keys.Enter)
				StartDemo();
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
