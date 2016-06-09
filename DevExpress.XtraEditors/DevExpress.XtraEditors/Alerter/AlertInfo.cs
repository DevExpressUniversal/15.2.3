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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using System.ComponentModel.Design;
using System.Drawing.Design;
namespace DevExpress.XtraBars.Alerter {
	public interface IAlertControl {
		AppearanceCaptionObject AppearanceCaption { get; }
		AppearanceObject AppearanceText { get; }
		AppearanceObject AppearanceHotTrackedText { get; }
		object Images { get; }
		void RaiseFormClosing(AlertFormClosingEventArgs args);
		UserLookAndFeel LookAndFeel { get; }
		AlertButtonCollection Buttons { get; }
		bool AutoHeight { get; }
		bool ShowToolTips { get; }
		bool ShowCloseButton { get; }
		bool ShowPinButton { get; }
		AlertFormControlBoxPosition ControlBoxPosition { get; }
		AlertFormLocation FormLocation { get; }
		AlertFormDisplaySpeed FormDisplaySpeed { get; }
		AlertFormShowingEffect FormShowingEffect { get; }
		int AutoFormDelay { get; }
		bool AllowHtmlText { get; }
		bool AllowHotTrack { get; }
		int RaiseGetDesiredAlertFormWidth(AlertFormWidthEventArgs args);
		bool RaiseAlertClick(AlertInfo info, AlertFormCore form);
		void RaiseFormLoad(AlertFormCore form);
		bool RaiseMouseFromEnter(AlertEventArgs args);
		bool RaiseMouseFromLeave(AlertEventArgs args);
		void RaiseButtonClick(AlertButton bi, AlertInfo info, AlertFormCore form);
		void RaiseButtonDownChanged(AlertButton bi, AlertInfo info, AlertFormCore form);
	}
	public class AlertControlHelper {
		[ThreadStatic]
		static ImageCollection windowImages;
		public static ImageCollection WindowImages {
			get {
				if(windowImages == null)
					windowImages = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources("DevExpress.XtraEditors.Alerter.docking-icons.png", typeof(AlertControlHelper).Assembly, new Size(9, 9), Color.Transparent);
				return windowImages;
			}
		}
		public static void ShowAlertMessage(Form owner, Point bottomLocation, string caption, string text, Image image) {
			AlertFormMessage frm = new AlertFormMessage(bottomLocation, null, new AlertInfo(caption, text, image));
			frm.ShowForm(owner, bottomLocation);
		}
	}
	public class AlertInfo {
		internal event EventHandler InfoChanged;
		string caption, text, hotTrackedText;
		object tag;
		Image image;
		public AlertInfo(string caption, string text) : this(caption, text, null, null, null) { }
		public AlertInfo(string caption, string text, string hotTrackedText) : this(caption, text, hotTrackedText, null, null) { }
		public AlertInfo(string caption, string text, Image image) : this(caption, text, null, image, null) { }
		public AlertInfo(string caption, string text, string hotTrackedText, Image image) : this(caption, text, hotTrackedText, image, null) { }
		public AlertInfo(string caption, string text, string hotTrackedText, Image image, object tag) {
			this.text = text;
			this.hotTrackedText = hotTrackedText;
			this.caption = caption;
			this.image = image;
			this.tag = tag;
		}
		internal string GetCaption() {
			if(Caption == null) return string.Empty;
			return Caption;
		}
		internal string GetText() {
			if(Text == null) return string.Empty;
			return Text;
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("AlertInfoCaption")]
#endif
		public string Caption {
			get { return caption; }
			set {
				if(value == caption) return;
				caption = value;
				OnInfoChanged();
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("AlertInfoText")]
#endif
		public string Text {
			get { return text; }
			set {
				if(value == text) return;
				text = value;
				OnInfoChanged();
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("AlertInfoHotTrackedText")]
#endif
		public string HotTrackedText {
			get { return hotTrackedText; }
			set {
				if(value == hotTrackedText) return;
				hotTrackedText = value;
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("AlertInfoTag")]
#endif
		public object Tag {
			get { return tag; }
			set {
				if(value == tag) return;
				tag = value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AlertInfoImage"),
#endif
 Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image Image {
			get { return image; }
			set {
				if(value == image) return;
				image = value;
				OnInfoChanged();
			}
			}
		protected virtual void OnInfoChanged() {
			if(InfoChanged != null)
				InfoChanged(this, EventArgs.Empty);
		}
	}
	public class AlertEventArgs : CancelEventArgs {
		AlertFormCore form;
		public AlertEventArgs(AlertFormCore form) {
			this.form = form;
		}
		public AlertFormCore AlertForm { get { return form; } }
	}
	public class AlertFormClosingEventArgs : AlertEventArgs {
		AlertFormCloseReason closeReason = AlertFormCloseReason.None;
		public AlertFormClosingEventArgs(AlertFormCore form, AlertFormCloseReason reason)
			: base(form) {
			closeReason = reason;
		}
		public AlertFormCloseReason CloseReason { get { return closeReason; } }
	}
	public class AlertFormWidthEventArgs : EventArgs {
		int width = -1;
		public int Width {
			get { return width; }
			set { if(value > 0) width = value; }
		}
	}
	public class AlertFormEventArgs : AlertEventArgs {
		Point location = Point.Empty;
		public AlertFormEventArgs(AlertFormCore form)
			: base(form) {
		}
		public Point Location {
			get { return location; }
			set { location = value; }
		}
	}
	public class AppearanceCaptionObject : AppearanceObject {
		static Font defaultFont;
		protected override Font InnerDefaultFont {
			get {
				if(defaultFont == null) defaultFont = CreateDefaultCaptionFont();
				return defaultFont;
			}
		}
		static Font CreateDefaultCaptionFont() {
			try {
				return new Font(AppearanceObject.DefaultFont, FontStyle.Bold);
			}
			catch { }
			return AppearanceObject.DefaultFont;
		}
	}
	public class AppearanceSelectedObject : AppearanceObject {
		static Font defaultFont;
		protected override Font InnerDefaultFont {
			get {
				if(defaultFont == null)  defaultFont = CreateDefaultSelectedFont();
				return defaultFont;
			}
		}
		static Font CreateDefaultSelectedFont() {
			try {
				return new Font(AppearanceObject.DefaultFont, FontStyle.Underline);
			}
			catch { }
			return AppearanceObject.DefaultFont;
		}
	}
}
