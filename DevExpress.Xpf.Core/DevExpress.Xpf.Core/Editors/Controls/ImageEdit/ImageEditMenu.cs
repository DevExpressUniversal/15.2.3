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
using System.Windows;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
using DevExpress.Mvvm;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Data.Camera;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Editors.Settings.Extension;
using DevExpress.Xpf.Bars;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using Microsoft.Win32;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Windows.Data;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using Separator = System.Windows.Controls.Control;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Xpf.Editors {
	public class ImageEditToolButton : Button {
		public static readonly DependencyProperty ImageSourceProperty;
		static ImageEditToolButton() {
			ImageSourceProperty = DependencyPropertyManager.Register("ImageSource", typeof(ImageSource), typeof(ImageEditToolButton), new FrameworkPropertyMetadata(null));
		}
		public ImageEditToolButton() {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
		internal ImageEditToolButton(string toolTip, string imageId, ICommand command)
			: base() {
			ToolTipService.SetToolTip(this, toolTip);
			ImageSource = ImageHelper.CreateImageFromCoreEmbeddedResource(string.Format("Editors.Images.ImageEdit.{0}.png", imageId));
#if !SL
			Command = command;
#else
			RoutedCommand.SetCommand(this, command);
#endif
		}
public ImageSource ImageSource {
			get { return (ImageSource)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}
		protected virtual IInputElement Owner {
			get { return ImageEdit.GetOwnerEdit(this) as ImageEdit; }
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
#if !SL
			if(Owner != null && CommandTarget == null) {
				CommandTarget = Owner;
				CommandManager.InvalidateRequerySuggested();
			}
#else
			if (Owner != null)
				RoutedCommand.SetCommandTarget(this, Owner);
#endif
		}
	}
	public class PopupImageEditButton : ImageEditToolButton {
		internal PopupImageEditButton(string toolTip, string imageId, ICommand command)
			: base(toolTip, imageId, command) {
		}
		protected override IInputElement Owner {
			get { return PopupBaseEdit.GetPopupOwnerEdit(this) as PopupImageEdit; }
		}
	}
	public class ImageEditToolSeparator : Control {
		public ImageEditToolSeparator() {
			this.SetDefaultStyleKey(typeof(ImageEditToolSeparator));
		}
	}
#if !SL
	public class ImageEditCopyToolButton : ImageEditToolButton {
		public ImageEditCopyToolButton() : base(EditorLocalizer.GetString(EditorStringId.Copy), "copy", ApplicationCommands.Copy) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
	}
	public class ImageEditPasteToolButton : ImageEditToolButton {
		public ImageEditPasteToolButton() : base(EditorLocalizer.GetString(EditorStringId.Paste), "paste", ApplicationCommands.Paste) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
	}
	public class ImageEditCutToolButton : ImageEditToolButton {
		public ImageEditCutToolButton() : base(EditorLocalizer.GetString(EditorStringId.Cut), "cut", ApplicationCommands.Cut) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
	}
	public class ImageEditClearToolButton : ImageEditToolButton {
		public ImageEditClearToolButton() : base(EditorLocalizer.GetString(EditorStringId.Clear), "clear", ApplicationCommands.Delete) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
	}
	public class ImageEditLoadToolButton : ImageEditToolButton {
		public ImageEditLoadToolButton() : base(EditorLocalizer.GetString(EditorStringId.Open), "open", ApplicationCommands.Open) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
	}
	public class ImageEditSaveToolButton : ImageEditToolButton {
		public ImageEditSaveToolButton() : base(EditorLocalizer.GetString(EditorStringId.Save), "save", ApplicationCommands.Save) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
	}
	public class ImageEditTakePictureButton : ImageEditToolButton {
		public ImageEditTakePictureButton() : base(EditorLocalizer.GetString(EditorStringId.CameraTakePictureToolTip), "takepicture", null) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
			Command = GetCommand();
		}
		ICommand GetCommand() {
			return new DelegateCommand(new Action(ShowTakePictureDialog));
		}
		void ShowTakePictureDialog() {
			var size = 350;
			DXWindow dlg = new DXWindow() { Width= size, Height = size, MinHeight= size , MinWidth= size, ShowIcon = false, WindowStartupLocation = WindowStartupLocation.CenterOwner };
			var parentWindow = LayoutHelper.FindLayoutOrVisualParentObject<Window>(Owner as DependencyObject, true);
			if (parentWindow != null)
				dlg.Owner = parentWindow;
			dlg.Title = EditorLocalizer.GetString(EditorStringId.CameraTakePictureCaption);
			var control =  new TakePictureControl();
			var imageEdit = (ImageEdit)Owner;
			control.SetEditor(imageEdit, imageEdit.PopupOwnerEdit as PopupImageEdit);
			dlg.Content = control;
			dlg.ShowDialog();
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			Visibility = DeviceHelper.GetDevices().Count > 0 ? Visibility.Visible : Visibility.Collapsed;
		}
	}
#else
	public class ImageEditClearToolButton : ImageEditToolButton {
		public ImageEditClearToolButton() : base(EditorLocalizer.GetString(EditorStringId.Clear), "clear", ImageEdit.ClearCommand) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
	}
	public class ImageEditLoadToolButton : ImageEditToolButton {
		public ImageEditLoadToolButton() : base(EditorLocalizer.GetString(EditorStringId.Open), "open", ImageEdit.OpenCommand) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
	}
#endif
	public class PopupImageEditOKToolButton : PopupImageEditButton {
		public PopupImageEditOKToolButton()
			: base(EditorLocalizer.GetString(EditorStringId.OK), "ok", PopupImageEdit.OKCommand) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
	}
	public class PopupImageEditCancelToolButton : PopupImageEditButton {
		public PopupImageEditCancelToolButton()
			: base(EditorLocalizer.GetString(EditorStringId.Cancel), "cancel", PopupImageEdit.CancelCommand) {
			this.SetDefaultStyleKey(typeof(ImageEditToolButton));
		}
	}
}
namespace DevExpress.Xpf.Editors.Internal {
	public class MenuContentControl : ContentControl {
#if SL
		public bool IsMouseOver { get; private set; }
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			IsMouseOver = true;
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			IsMouseOver = false;
		}
#endif
	}
}
