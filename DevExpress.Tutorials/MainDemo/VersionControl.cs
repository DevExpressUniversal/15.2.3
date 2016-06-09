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
using DevExpress.XtraEditors;
using DevExpress.Utils.Controls;
using System.Drawing;
using DevExpress.Utils.Frames;
using DevExpress.Utils.About;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using System.Windows.Forms;
using System.Reflection;
using DevExpress.Utils;
namespace DevExpress.Tutorials {
	public class VersionControl : PanelControl, IXtraResizableControl {
		LabelInfo labelInfo = new LabelInfo();
		ProductKind product = ProductKind.Default;
		public VersionControl() {
			UserLookAndFeel.Default.StyleChanged += new EventHandler(Default_StyleChanged);
			CreateLabelInfo();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing)
				UserLookAndFeel.Default.StyleChanged -= new EventHandler(Default_StyleChanged);
		}
		void Default_StyleChanged(object sender, EventArgs e) {
			UpdateTextColor();
		}
		void UpdateTextColor() {
			foreach(LabelInfoText liText in labelInfo.Texts)
				liText.Color = liText.Active ? GetLinkColor(LookAndFeel) : ForeColor;   
		}
		string dxSite = Properties.Resources.DXSite;
		string dxMail = AssemblyInfo.DXEmailInfo;
		string dxSupport = AssemblyInfo.DXLinkGetSupport;
		protected bool IsTrial { get { return AboutHelper.GetSerial(ProductInfo) == AboutHelper.TrialVersion; } }
		protected virtual ProductInfo ProductInfo {
			get {
				return new ProductInfo(string.Empty, typeof(VersionControl), product, ProductInfoStage.Registered);
			}
		}
		void CreateLabelInfo() {
			Rectangle r = new Rectangle(0, 0, controlSize.Width, controlSize.Height);
			r.Inflate(-2, -2);
			labelInfo.BackColor = Color.Transparent;
			labelInfo.Bounds = r;
			labelInfo.Parent = this;
			labelInfo.ItemClick += new LabelInfoItemClickEvent(OnLabelClick);
			UpdateLabelText();
		}
		void UpdateLabelText() {
			if(ProductInfo.Kind == ProductKind.Default) return;
			labelInfo.Texts.Clear();
			labelInfo.Texts.Add(string.Format("{0}: ", Properties.Resources.Version));
			labelInfo.Texts.Add(AssemblyInfo.MarketingVersion, Color.Empty, false, true);
			labelInfo.Texts.Add("\r\n\r\n");
			if(!IsTrial) {
				labelInfo.Texts.Add(string.Format("{0}: ", Properties.Resources.SerialNumber));
				labelInfo.Texts.Add(AboutHelper.GetSerial(ProductInfo), !IsTrial);
				labelInfo.Texts.Add("\r\n");
			}
			if(IsTrial) {
				labelInfo.Texts.Add("\r\n");
				labelInfo.Texts.Add(string.Format("{0} ", Properties.Resources.ToPurchase));
				labelInfo.Texts.Add(dxSite, true);
				if(!LocalizationHelper.IsJapanese) {
					labelInfo.Texts.Add(string.Format("\r\n{0} ", Properties.Resources.CallUs));
					labelInfo.Texts.Add("+1 (818) 844-3383", Color.Empty, false, true);
				}
				labelInfo.Texts.Add("\r\n\r\n");
				labelInfo.Texts.Add(string.Format(DPIDisplayFormat, UpdateDPIString(Properties.Resources.ForPrePurchase)));
				labelInfo.Texts.Add(dxMail, true);
				labelInfo.Texts.Add("\r\n");
			}
			labelInfo.Texts.Add("\r\n\r\n");
			if(!LocalizationHelper.IsJapanese) {
				labelInfo.Texts.Add(string.Format(DPIDisplayFormat, UpdateDPIString(Properties.Resources.TechQuestions)));
				labelInfo.Texts.Add(dxSupport, true);
			}
			UpdateTextColor();
		}
		string DPIDisplayFormat {
			get {
				if(ScaleUtils.IsLargeFonts) return "{0}: ";
				return "{0}:\r\n";
			}
		}
		string UpdateDPIString(string s) {
			if(ScaleUtils.IsLargeFonts) return s;
			return s.Replace(",", ",\r\n");
		}
		string GetProcessName(LabelInfoItemClickEventArgs e) {
			if(e.InfoText.Text == dxSite) return AssemblyInfo.DXLinkBuyNow;
			if(e.InfoText.Text == dxMail) return AssemblyInfo.DXLinkEmailInfo;
			if(e.InfoText.Text == dxSupport) return AssemblyInfo.DXLinkGetSupport;
			return null;
		}
		ContextMenu serialMenu;
		protected ContextMenu SerialMenu {
			get {
				if(serialMenu == null) serialMenu = new ContextMenu(new MenuItem[] { new MenuItem("Copy to clipboard", new EventHandler(OnMenuClick)) });
				return serialMenu;
			}
			set {
				if(serialMenu != null) serialMenu.Dispose();
				serialMenu = null;
			}
		}
		void OnMenuClick(object sender, EventArgs e) {
			Clipboard.SetDataObject(AboutHelper.GetSerial(ProductInfo));
		}
		void OnLabelClick(object sender, LabelInfoItemClickEventArgs e) {
			string name = GetProcessName(e);
			if(name == null) {
				SerialMenu.Show(this, PointToClient(Control.MousePosition));
				return;
			}
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = name;
			process.StartInfo.Verb = "Open";
			process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
			process.Start();
		}
		public static Color GetLinkColor(UserLookAndFeel lookAndFeel) {
			Color color = Color.Empty;
			if(lookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin) {
				Skin skin = EditorsSkins.GetSkin(lookAndFeel);
				color = skin.Colors.GetColor(EditorsSkins.SkinHyperlinkTextColor);
			}
			if(color.IsEmpty) color = Color.Blue;
			return color;
		}
		#region IXtraResizableControl Members
		Size controlSize = ScaleUtils.GetScaleSize(Properties.Resources.VCSize);
		public event EventHandler Changed;
		protected virtual void RaiseChanged() {
			if(Changed != null)
				Changed(this, EventArgs.Empty);
		}
		public bool IsCaptionVisible {
			get { return false; }
		}
		public Size MaxSize {
			get { return controlSize; }
		}
		public Size MinSize {
			get { return controlSize; }
		}
		#endregion
		internal void SetProduct(ProductKind ProductKind) {
			product = ProductKind;
			UpdateLabelText();
		}
	}
}
