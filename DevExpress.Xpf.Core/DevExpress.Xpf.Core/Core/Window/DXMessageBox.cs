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
using DevExpress.Data;
using System.Threading;
using System.Resources;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Native;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Themes;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
namespace DevExpress.Xpf.Core {
	public static class DXMessageBoxFactory {
		public static void RegisterMessageBoxCreator(DXMessageBoxCreator creator) {
			DXMessageBox.creator = creator;
		}
	}
	public class DXMessageBox : Control {
		internal static DXMessageBoxCreator creator = new DXMessageBoxCreator();
		static DXMessageBox() {
			KeyboardNavigation.DirectionalNavigationProperty.OverrideMetadata(typeof(DXMessageBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			KeyboardNavigation.TabNavigationProperty.OverrideMetadata(typeof(DXMessageBox), new FrameworkPropertyMetadata(KeyboardNavigationMode.Cycle));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DXMessageBox), new FrameworkPropertyMetadata(typeof(DXMessageBox)));
		}
		Size minSize = new Size(350, 100);
		protected MessageBoxResult? messageBoxResult = null;
		protected MessageBoxResult defaultResult;
		protected FloatingContainer fc = null;
		MessageBoxButton _messageBoxButtons = MessageBoxButton.OK;
		protected MessageBoxButton messageBoxButtons {
			get { return _messageBoxButtons; }
			set {
				if(value == _messageBoxButtons)
					return;
				_messageBoxButtons = value;
				UpdateVisualState();
			}
		}
		public string Text {
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(DXMessageBox), new UIPropertyMetadata(String.Empty));
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public static readonly DependencyProperty CaptionProperty =
			DependencyProperty.Register("Caption", typeof(string), typeof(DXMessageBox), new UIPropertyMetadata(String.Empty));
		public ImageSource ImageSource {
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		public static readonly DependencyProperty ImageSourceProperty =
			DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(DXMessageBox), new UIPropertyMetadata(default(ImageSource)));
		private static bool IsValidMessageBoxButton(MessageBoxButton value) {
			if(((value != MessageBoxButton.OK) && (value != MessageBoxButton.OKCancel)) && (value != MessageBoxButton.YesNo)) {
				return (value == MessageBoxButton.YesNoCancel);
			}
			return true;
		}
		private static bool IsValidMessageBoxImage(MessageBoxImage value) {
			if((((value != MessageBoxImage.Asterisk) && (value != MessageBoxImage.Hand)) && ((value != MessageBoxImage.Exclamation) && (value != MessageBoxImage.Hand))) && (((value != MessageBoxImage.Asterisk) && (value != MessageBoxImage.None)) && ((value != MessageBoxImage.Question) && (value != MessageBoxImage.Hand)))) {
				return (value == MessageBoxImage.Exclamation);
			}
			return true;
		}
		private static bool IsValidMessageBoxOptions(MessageBoxOptions value) {
			int num = -3801089;
			return (((int)value & num) == (int)MessageBoxOptions.None);
		}
		private static bool IsValidMessageBoxResult(MessageBoxResult value) {
			if(((value != MessageBoxResult.Cancel) && (value != MessageBoxResult.No)) && ((value != MessageBoxResult.None) && (value != MessageBoxResult.OK))) {
				return (value == MessageBoxResult.Yes);
			}
			return true;
		}
		public static MessageBoxResult Show(string messageBoxText) {
			return Show(null, messageBoxText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public static MessageBoxResult Show(string messageBoxText, string caption) {
			return Show(null, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public static MessageBoxResult Show(FrameworkElement owner, string messageBoxText) {
			return Show(owner, messageBoxText, string.Empty, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button) {
			return Show(null, messageBoxText, caption, button, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption) {
			return Show(owner, messageBoxText, caption, MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon) {
			return Show(null, messageBoxText, caption, button, icon, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button) {
			return Show(owner, messageBoxText, caption, button, MessageBoxImage.None, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult) {
			return Show(null, messageBoxText, caption, button, icon, defaultResult, MessageBoxOptions.None);
		}
		public static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon) {
			return Show(owner, messageBoxText, caption, button, icon, MessageBoxResult.None, MessageBoxOptions.None);
		}
		public static MessageBoxResult Show(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options) {
			return Show(null, messageBoxText, caption, button, icon, defaultResult, options);
		}
		public static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult) {
			return Show(owner, messageBoxText, caption, button, icon, defaultResult, MessageBoxOptions.None);
		}
		public static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options) {
			return Show(owner, messageBoxText, caption, button, icon, defaultResult, options, FloatingMode.Window);
		}
		public static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options, FloatingMode desiredFloatingMode) {
			return Show(owner, messageBoxText, caption, button, icon, defaultResult, options, desiredFloatingMode, false);
		}
		public static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options, FloatingMode desiredFloatingMode, bool allowShowAnimatoin) {
			return ShowCore(owner, messageBoxText, caption, button, icon, defaultResult, options, desiredFloatingMode, allowShowAnimatoin,500);
		}
		public static MessageBoxResult Show(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options, FloatingMode desiredFloatingMode, bool allowShowAnimatoin, double maximumWidth) {
			return ShowCore(owner, messageBoxText, caption, button, icon, defaultResult, options, desiredFloatingMode, allowShowAnimatoin, maximumWidth);
		}
		private static MessageBoxResult ShowCore(FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options, FloatingMode desiredFloatingMode, bool allowShowAnimatoin, double maximumWidth) {
			return ShowCore(creator, owner, messageBoxText, caption, button, icon, defaultResult, options, desiredFloatingMode, allowShowAnimatoin, maximumWidth);
		}
		protected static MessageBoxResult ShowCore(DXMessageBoxCreator creator, FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options, FloatingMode desiredFloatingMode, bool allowShowAnimatoin, double maximumWidth) {
			if(!IsValidMessageBoxButton(button)) {
				throw new InvalidEnumArgumentException("button", (int)button, typeof(MessageBoxButton));
			}
			if(!IsValidMessageBoxImage(icon)) {
				throw new InvalidEnumArgumentException("icon", (int)icon, typeof(MessageBoxImage));
			}
			if(!IsValidMessageBoxResult(defaultResult)) {
				throw new InvalidEnumArgumentException("defaultResult", (int)defaultResult, typeof(MessageBoxResult));
			}
			if(!IsValidMessageBoxOptions(options)) {
				throw new InvalidEnumArgumentException("options", (int)options, typeof(MessageBoxOptions));
			}
			if((options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) != MessageBoxOptions.None) {
				if(owner != null) {
					throw new ArgumentException("CantShowMBServiceWithOwner");
				}
			}
			else if(owner == null) {
				owner = CalcOwner();
			}
			if(VerifyIsAlive()) {
				return MessageBox.Show(messageBoxText, caption, button, icon, defaultResult, options);
			}
			if(owner == null || !owner.IsVisible) {
				Window w = creator.CreateWindow();
				try {
					w.ShowActivated = false;
					w.ShowInTaskbar = true;
					w.WindowState = WindowState.Minimized;
					w.AllowsTransparency = true;
					w.WindowStyle = WindowStyle.None;
					w.Background = Brushes.Transparent;
					if(ThemeManager.ApplicationThemeName != String.Empty) w.SetValue(ThemeManager.ThemeNameProperty, ThemeManager.ApplicationThemeName);
					w.Show();
					return ShowCoreValidOwner(creator, w, messageBoxText, caption, button, icon, defaultResult, options, FloatingMode.Window, allowShowAnimatoin, maximumWidth);
				} finally {
					if(Application.Current == null || Application.Current != null && (!Application.Current.CheckAccess() || Application.Current.Windows.Count <= 1)) {
						w.DelayedExecute(() => {
							w.Close();
						});
					} else w.Close();
				}
			}
			if(owner == null) return MessageBoxResult.None;
			return ShowCoreValidOwner(creator, CorrectOwner(owner), messageBoxText, caption, button, icon, defaultResult, options, desiredFloatingMode, allowShowAnimatoin, maximumWidth);
		}
		protected static FrameworkElement CorrectOwner(FrameworkElement owner) {
			DXWindow dxw = owner as DXWindow;
			if(dxw != null) {
				FrameworkElement newOwner = LayoutHelper.FindElement(dxw, (element) => element is BackgroundPanel);
				if(newOwner != null) return newOwner;
			}
			return owner;
		}
		protected override void OnPreviewKeyUp(KeyEventArgs e) {
			if(e.Key == Key.C && Keyboard.IsKeyDown(Key.LeftCtrl))
				Clipboard.SetText(GetTextForClipBoard());
			base.OnPreviewKeyUp(e);
		}
		private string GetTextForClipBoard() {
			string constString = "---------------------------";
			string newLine = Environment.NewLine;
			string returnText = String.Format("{0}{1}{2}{1}{0}{1}{3}{1}{0}{1}{4}{0}{1}",constString,newLine,Caption,Text,messageBoxButtons.ToString());
			return returnText;
		}
		private static bool VerifyIsAlive() {
			PropertyInfo pi = typeof(Application).GetProperty("IsShuttingDown", BindingFlags.Static | BindingFlags.NonPublic);
			if(pi == null) return false;
			MethodInfo mi = pi.GetGetMethod(true);
			if(mi == null) return false;
			object result = mi.Invoke(null, new object[] { });
			bool isShuttingDown = (bool)result;
			return isShuttingDown;
		}
		[DllImport("user32.dll")]
		[System.Security.SecurityCritical]
		static extern bool ReleaseCapture();
		[System.Security.SecuritySafeCritical]
		private static MessageBoxResult ShowCoreValidOwner(DXMessageBoxCreator creator, FrameworkElement owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options, FloatingMode desiredFloatingMode, bool allowShowAnimation, double maximumWidth) {
			ReleaseCapture();
			FrameworkElement root = LayoutHelper.GetTopLevelVisual(owner);
			DXMessageBox dxmb = creator.Create();
			bool showCloseButtonCloseOnEscape = button != MessageBoxButton.YesNo;
			dxmb.defaultResult = defaultResult;
			dxmb.MaxWidth = maximumWidth;
			dxmb.InitFloatingContainer(owner, messageBoxText, caption, icon, button, desiredFloatingMode, showCloseButtonCloseOnEscape, showCloseButtonCloseOnEscape, allowShowAnimation, options);
			SystemSound sound = null;
			switch(icon) {
				case MessageBoxImage.Asterisk:
					sound = SystemSounds.Asterisk; break;
				case MessageBoxImage.Exclamation:
					sound = SystemSounds.Exclamation; break;
				case MessageBoxImage.Error:
					sound = SystemSounds.Hand; break;
				case MessageBoxImage.Question:
					sound = SystemSounds.Question; break;
			}
			if(sound != null) {
				try {
					sound.Play();
				}
				catch { }
			}
			if(button == MessageBoxButton.YesNo) {
				FloatingWindowContainer fwc = dxmb.fc as FloatingWindowContainer;
				if(fwc != null && fwc.Window != null) { fwc.allowProcessClosing = false; fwc.Window.Closing += (s, e) => { if(dxmb.messageBoxResult == null) { e.Cancel = true; } }; }
			}
			dxmb.DialogWndProc();
			DependencyObject rootElement = LayoutHelper.FindRoot(owner);
			Window rwindow = rootElement as Window;
			if(rwindow != null) rwindow.Activate();
			return dxmb.messageBoxResult != null ? dxmb.messageBoxResult.Value : MessageBoxResult.None;
		}
		static void root_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
			e.Handled = ProcessMouseEvent(e.OriginalSource as DependencyObject);
		}
		static void root_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			e.Handled = ProcessMouseEvent(e.OriginalSource as DependencyObject);
		}
		static void root_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			e.Handled = ProcessMouseEvent(e.OriginalSource as DependencyObject);
		}
		static bool ProcessMouseEvent(DependencyObject originalSource) {
			return true;
		}
		protected object DoIdle(object args) {
			return args;
		}
		protected void DialogWndProc() {
			do {
				DispatcherOperation doo = Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(DoIdle), this);
				DispatcherOperationStatus currentStatus;
				do {
					Thread.Sleep(50);
					currentStatus = doo.Wait(TimeSpan.FromSeconds(1));
					if(fc.IsClosed && messageBoxResult == null) messageBoxResult = MessageBoxResult.None;
					DevExpress.Xpf.Core.FloatingWindowContainer fwc = fc as DevExpress.Xpf.Core.FloatingWindowContainer;
					if(fwc != null && messageBoxResult == null &&(fwc.Window == null || !fwc.Window.IsVisible)) messageBoxResult = MessageBoxResult.None;
					if((fc.Owner == null || !fc.Owner.IsInVisualTree()) && (fc is FloatingAdornerContainer)) messageBoxResult = MessageBoxResult.None;
				} while(currentStatus != DispatcherOperationStatus.Completed && messageBoxResult == null);
			} while(messageBoxResult == null);
			FloatingContainer.SetFloatingContainer(fc.LogicalOwner, storedFC);
		}
		protected static FrameworkElement CalcOwner() {
			FrameworkElement owner = null;
			if(Application.Current != null && Application.Current.Dispatcher.CheckAccess()) {
				foreach(Window w in Application.Current.Windows) {
					if(w.IsActive && w.Background != Brushes.Transparent) {
						owner = w;
						break;
					}
				}
				if(owner == null && Application.Current.Windows.Count > 0) {
					var w = Application.Current.Windows[0];
					if(w.Background != Brushes.Transparent) owner = w;
				}
			}
			return owner;
		}
		protected void DialogClosed(bool? dialogResult) {
			if(messageBoxResult == null) {
				switch(messageBoxButtons) {
					case MessageBoxButton.OK:
						messageBoxResult = MessageBoxResult.OK;
						break;
					case MessageBoxButton.OKCancel:
					case MessageBoxButton.YesNoCancel:
						messageBoxResult = MessageBoxResult.Cancel;
						break;
				}
			}
		}
		protected virtual FloatingContainer CreateFloatingContainer(FloatingMode floatingMode) {
			return FloatingContainerFactory.Create(floatingMode);
		}
		FloatingContainer storedFC = null;
		private void InitFloatingContainer(FrameworkElement owner, string messageBoxText, string caption, MessageBoxImage icon, MessageBoxButton button, FloatingMode desiredFloatingMode, bool showCloseButton, bool closeOnEscape, bool allowShowAnimation, MessageBoxOptions options) {
			InitImageSource(this, icon);
			InitCaption(this, caption);
			InitText(this, messageBoxText);
			fc = CreateFloatingContainer(desiredFloatingMode);
			fc.AllowShowAnimations = allowShowAnimation;
			fc.BeginUpdate();
			storedFC = FloatingContainer.GetFloatingContainer(owner);
			FloatingContainer.SetFloatingContainer(owner, fc);
			fc.Owner = owner;
			fc.AllowSizing = false;
			fc.SizeToContent = SizeToContent.WidthAndHeight;
			this.messageBoxButtons = button;
			if(owner != null) {
				fc.SetValue(ThemeManager.ThemeNameProperty, owner.GetValue(ThemeManager.ThemeNameProperty));
				fc.SetValue(FrameworkElement.FlowDirectionProperty, owner.GetValue(FrameworkElement.FlowDirectionProperty));
				fc.SetValue(FrameworkElement.TagProperty, this);
			}
			if((options & MessageBoxOptions.RtlReading) == MessageBoxOptions.RtlReading) fc.SetValue(FrameworkElement.FlowDirectionProperty, FlowDirection.RightToLeft);
			FloatingContainer.InitDialog(this, owner, DialogClosed, minSize, caption, false, fc, closeOnEscape);
			fc.ShowCloseButton = showCloseButton;
			fc.DeactivateOnClose = false;
			fc.EndUpdate();
			fc.FloatLocation = new Point(Double.NaN, double.NaN);
			InitFloatingContainerCore(owner, messageBoxText, caption, icon, button, desiredFloatingMode, showCloseButton, closeOnEscape, allowShowAnimation);
			fc.IsOpen = true;
		}
		protected virtual void InitFloatingContainerCore(FrameworkElement owner, string messageBoxText, string caption, MessageBoxImage icon, MessageBoxButton button, FloatingMode desiredFloatingMode, bool showCloseButton, bool closeOnEscape, bool allowShowAnimation) { }
		string[] buttonNames = new string[]{
			"PART_NoButton", "PART_CancelButton", "PART_YesButton", "PART_OkButton",
			"PART_NoButton1", "PART_CancelButton1", "PART_YesButton1", "PART_OkButton1"
		};
		protected bool IsButtonVisible(string buttonName) {
			Button button = FindMessageBoxButton(buttonName);
			if(button != null && button.ActualWidth > 0) return true;
			return false;
		}
		protected void ProcessYesNoKey(KeyEventArgs e, string buttonName, Key key, MessageBoxResult mbr) {
			if(e.Key == key && IsButtonVisible(buttonName)) {
				messageBoxResult = mbr;
				ProcessClosing();
				e.Handled = true;
			}
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e) {
			base.OnPreviewKeyDown(e);
			ProcessYesNoKey(e, "PART_YesButton", Key.Y, MessageBoxResult.Yes);
			ProcessYesNoKey(e, "PART_NoButton", Key.N, MessageBoxResult.No);
			if(e.Key == Key.Tab && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control) e.Handled = true;
		}
		protected Button FindMessageBoxButton(string name) {
			Button tempButton = LayoutHelper.FindElementByName(this, name) as Button;
			if(tempButton == null || tempButton.ActualWidth == 0) tempButton = LayoutHelper.FindElementByName(this, name + "1") as Button;
			return tempButton;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateVisualState();
			this.DelayedExecute(() => {
				FindAndFocusDefaultButton();
			});
			if(ImageSource == default(ImageSource)) {
				Image tempImage = LayoutHelper.FindElementByName(this, "PART_MessageBoxImage") as Image;
				if(tempImage != null) tempImage.Visibility = System.Windows.Visibility.Collapsed;
			}
		}
		void UpdateVisualState() {
			VisualStateManager.GoToState(this, messageBoxButtons.ToString(), true);
		}
		private void FindAndFocusDefaultButton() {
			Button defaultButton = null;
			switch(defaultResult) {
				case MessageBoxResult.Cancel:
					defaultButton = FindMessageBoxButton("PART_CancelButton");
					break;
				case MessageBoxResult.No:
					defaultButton = FindMessageBoxButton("PART_NoButton");
					break;
				case MessageBoxResult.OK:
					defaultButton = FindMessageBoxButton("PART_OkButton");
					break;
				case MessageBoxResult.Yes:
					defaultButton = FindMessageBoxButton("PART_YesButton");
					break;
				case MessageBoxResult.None: break;
			}
			if(defaultButton != null) {
				foreach(string name in buttonNames) {
					Button tempButton = LayoutHelper.FindElementByName(this, name) as Button;
					if(tempButton != null) {
						tempButton.Click += new RoutedEventHandler(button_Click);
					}
				}
				FocusButton(defaultButton);
			} else {
				bool focused = false;
				foreach(string name in buttonNames) {
					Button tempButton = LayoutHelper.FindElementByName(this, name) as Button;
					if(tempButton != null) {
						if((messageBoxButtons == MessageBoxButton.OK && tempButton.Name == "PART_OkButton") ||
							(messageBoxButtons == MessageBoxButton.OKCancel && tempButton.Name == "PART_OkButton1") ||
							(messageBoxButtons == MessageBoxButton.YesNo && tempButton.Name == "PART_YesButton") ||
							(messageBoxButtons == MessageBoxButton.YesNoCancel && tempButton.Name == "PART_YesButton1")
							) {
							if(!focused) {
								FocusButton(tempButton);
							}
							focused = true;
						}
						tempButton.Click += new RoutedEventHandler(button_Click);
					}
				}
			}
		}
		void FocusButton(Button tempButton){
			Keyboard.Focus(tempButton);
			tempButton.Focus();
		}
		void button_Click(object sender, RoutedEventArgs e) {
			Button button = e.Source as Button;
			DoButtonClick(button);
		}
		private void DoButtonClick(Button button) {
			switch(button.Name) {
				case "PART_NoButton":
				case "PART_NoButton1":
					messageBoxResult = MessageBoxResult.No;
					break;
				case "PART_YesButton":
				case "PART_YesButton1":
					messageBoxResult = MessageBoxResult.Yes;
					break;
				case "PART_CancelButton":
				case "PART_CancelButton1":
					messageBoxResult = MessageBoxResult.Cancel;
					break;
				case "PART_OkButton":
				case "PART_OkButton1":
					messageBoxResult = MessageBoxResult.OK;
					break;
			}
			ProcessClosing();
		}
		void ProcessClosing() {
			if(fc != null) fc.Close();
		}
		protected static void InitImageSource(DXMessageBox dxmb, MessageBoxImage icon) {
			String uriPrefix = "pack://application:,,,/" + AssemblyInfo.SRAssemblyXpfCore + ";component/Core/Window/Icons/";
			String iconName = String.Empty;
			switch(icon) {
				case MessageBoxImage.Asterisk: iconName = "Information_48x48.png"; break;
				case MessageBoxImage.Error: iconName = "Error_48x48.png"; break;
				case MessageBoxImage.Exclamation: iconName = "Warning_48x48.png"; break;
				case MessageBoxImage.None: return;
				case MessageBoxImage.Question: iconName = "Question_48x48.png"; break;
			}
			String uri = uriPrefix + iconName;
			dxmb.ImageSource = (ImageSource)new ImageSourceConverter().ConvertFromString(uri);
		}
		static void InitText(DXMessageBox dxmb, string messageBoxText) {
			dxmb.Text = messageBoxText;
		}
		static void InitCaption(DXMessageBox dxmb, string caption) {
			dxmb.Caption = caption;
		}
	}
	public class DXMessageBoxCreator {
		public virtual DXMessageBox Create() {
			return new DXMessageBox();
		}
		public virtual Window CreateWindow() {
			return new Window();
		}
	}
	public enum DXMessageBoxStringId { Ok, Cancel, Yes, No }
	public class DXMessageBoxResXLocalizer : DXResXLocalizer<DXMessageBoxStringId> {
		public DXMessageBoxResXLocalizer()
			: base(new DXMessageBoxLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Core.Core.Window.DXMessageBoxRes", typeof(DXMessageBoxResXLocalizer).Assembly);
		}
	}
	public class DXMessageBoxLocalizer : DXLocalizer<DXMessageBoxStringId> {
		static DXMessageBoxLocalizer() {
			XtraLocalizer<DXMessageBoxStringId>.SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<DXMessageBoxStringId>(CreateDefaultLocalizer()));
		}
		public static XtraLocalizer<DXMessageBoxStringId> CreateDefaultLocalizer() {
			return new DXMessageBoxResXLocalizer();
		}
		public override XtraLocalizer<DXMessageBoxStringId> CreateResXLocalizer() {
			return new DXMessageBoxResXLocalizer();
		}
		public static string GetString(DXMessageBoxStringId id) {
			return XtraLocalizer<DXMessageBoxStringId>.Active.GetLocalizedString(id);
		}
		protected override void PopulateStringTable() {
			this.AddString(DXMessageBoxStringId.Cancel, "Cancel");
			this.AddString(DXMessageBoxStringId.Ok, "OK");
			this.AddString(DXMessageBoxStringId.Yes, "Yes");
			this.AddString(DXMessageBoxStringId.No, "No");
		}
	}
	public class DXMessageBoxStringIdExtension : MarkupExtension {
		public DXMessageBoxStringId StringId { get; set; }
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return DXMessageBoxLocalizer.GetString(StringId);
		}
	}
}
