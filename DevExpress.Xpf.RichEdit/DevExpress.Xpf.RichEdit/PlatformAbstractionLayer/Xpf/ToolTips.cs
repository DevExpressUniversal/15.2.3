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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
namespace DevExpress.XtraRichEdit.Internal {
	public interface IToolTipControlClient {
		bool HasToolTip { get; }
		ToolTipControlInfo GetObjectInfo(Point point);
	}
	public class ToolTipControlInfo {
		string text;
		string footer;
		object obj;
		Point position;
		string header;
		public ToolTipControlInfo() {
		}
		public ToolTipControlInfo(object obj)
			: this(obj, null, null) {
		}
		public ToolTipControlInfo(object obj, string text)
			: this(obj, text, null) {
		}
		public ToolTipControlInfo(object obj, string text, string footer) {
			this.obj = obj;
			this.text = text;
			this.footer = footer;
		}
		public string Text { get { return text; } set { text = value; } }
		public string Footer { get { return footer; } set { footer = value; } }
		public object Object { get { return obj; } set { obj = value; } }
		public Point Position { get { return position; } set { position = value; } }
		public string Header { get { return header; } set { header = value; } }
	}
}
namespace DevExpress.XtraRichEdit.Internal {
	#region ToolTipController
	public class ToolTipController : IDisposable {
		static double DefaultTimerInterval = 10000;
		static int PopupVerticalOffset = -80;
		static int DefaultMaxTextLength = 100;
		object activeObject;
		ToolTip popup;
		DispatcherTimer timer;
		bool isDisposed;
		double popupAutoCloseDelay;
		int maxToolTipLength;
		readonly RichEditControl control;
		public ToolTipController(RichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.popup = CreatePopupControl();
			this.timer = new DispatcherTimer();
			PopupAutoCloseDelay = DefaultTimerInterval;
			MaxToolTipLength = DefaultMaxTextLength;
			SubscribeToEvents();
		}
		public IToolTipControlClient ToolTipClient { get { return (IToolTipControlClient)control; } }
		public object ActiveObject { get { return activeObject; } }
		public bool IsDisposed { get { return isDisposed; } }
		public int MaxToolTipLength {
			get { return maxToolTipLength; }
			set {
				if (value < 1)
					value = 1;
				maxToolTipLength = value;
			}
		}
		public double PopupAutoCloseDelay {
			get { return popupAutoCloseDelay; }
			set {
				if (value < 1)
					value = 1;
				if (value == PopupAutoCloseDelay)
					return;
				popupAutoCloseDelay = value;
				timer.Interval = TimeSpan.FromMilliseconds(PopupAutoCloseDelay);
			}
		}
		protected virtual ToolTip CreatePopupControl() {
			ToolTip result = new ToolTip();
			result.VerticalOffset = PopupVerticalOffset;
			result.PlacementTarget = control;
			result.Placement = PlacementMode.Mouse;
			ToolTipService.SetInitialShowDelay(result, ToolTipService.GetInitialShowDelay(control));
			return result;
		}
		protected virtual void SubscribeToEvents() {
			this.control.MouseMove += OnControlMouseMove;
			this.control.MouseLeftButtonUp += OnMouseClick;
			this.control.MouseRightButtonUp += OnMouseClick;
			this.control.MouseLeave += OnControlMouseLeave;
			this.timer.Tick += timer_Tick;
		}
		protected virtual void UsubscribeToEvents() {
			this.control.MouseMove -= OnControlMouseMove;
			this.control.MouseLeftButtonUp -= OnMouseClick;
			this.control.MouseRightButtonUp -= OnMouseClick;
			this.control.MouseLeave -= OnControlMouseLeave;
			this.timer.Tick -= timer_Tick;
		}
		void timer_Tick(object sender, EventArgs e) {
			this.popup.Dispatcher.BeginInvoke(new Action(ClosePopup));
		}
		void OnControlMouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
			ClosePopup();
		}
		void OnControlMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
			if (!ToolTipClient.HasToolTip)
				return;
			Point point = e.GetPosition(control);
			ToolTipControlInfo toolTipInfo = ToolTipClient.GetObjectInfo(point);
			if (toolTipInfo == null) {
				ClosePopup();
				return;
			}
			object obj = toolTipInfo.Object;
			if (obj == activeObject)
				return;
			else {
				ClosePopup();
				OpenPopup(toolTipInfo);
				activeObject = obj;
			}
		}
		void OnMouseClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			ClosePopup();
		}
		protected internal virtual void OpenPopup(ToolTipControlInfo info) {
			SuperTipControl superTipControl = CreateSuperTipControl(info);
			if (superTipControl == null)
				return;
			popup.Content = superTipControl;
			popup.IsOpen = true;
			timer.Start();
		}
		SuperTipControl CreateSuperTipControl(ToolTipControlInfo info) {
			if ((info.Object as Comment) !=null || (info.Object as CommentViewInfo) != null)
				return CreateSuperTipControlForComment(info);
			if ((info.Object as HyperlinkInfo) != null) 
				return CreateSuperTipControlForHyperlink(info);
			return null;
		}
		SuperTipControl CreateSuperTipControlForComment(ToolTipControlInfo info) {
			SuperTip superTip = new SuperTip();
			SuperTipHeaderItem headerItem = new SuperTipHeaderItem();
			SuperTipItem textItem = new SuperTipItem();
			SuperTipControl superTipControl = new SuperTipControl(superTip);
			superTip.Items.Add(headerItem);
			superTip.Items.Add(textItem);
			headerItem.Content = GetActualText(info.Header);
			textItem.Content = GetActualText(info.Text);
			return superTipControl;
		}
		SuperTipControl CreateSuperTipControlForHyperlink(ToolTipControlInfo info){
			if (!control.Options.Hyperlinks.ShowToolTip)
				return null;
			SuperTip superTip = new SuperTip();
			SuperTipHeaderItem footerItem = new SuperTipHeaderItem();
			SuperTipItem textItem = new SuperTipItem();
			SuperTipControl superTipControl = new SuperTipControl(superTip);
			superTip.Items.Add(textItem);
			superTip.Items.Add(footerItem);
			footerItem.Content = GetActualText(info.Footer);
			textItem.Content = GetActualText(info.Text);
			return superTipControl;
		}
		string GetActualText(string origin) {
			return origin.Length <= MaxToolTipLength ? origin : String.Format("{0}...", origin.Substring(0, MaxToolTipLength));
		}
		protected internal virtual void ClosePopup() {
			if (!popup.IsOpen)
				return;
			popup.IsOpen = false;
			activeObject = null;
			timer.Stop();
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				popup.IsOpen = false;
				UsubscribeToEvents();
			}
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ToolTipController() {
			Dispose(false);
		}
		#endregion
	}
	#endregion
}
