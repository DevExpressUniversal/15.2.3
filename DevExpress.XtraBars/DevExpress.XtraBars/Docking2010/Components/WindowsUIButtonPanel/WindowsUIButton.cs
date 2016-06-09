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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraEditors.ButtonPanel;
using BaseHeaderButtonPreferredConstructor = DevExpress.XtraEditors.Controls.EditorButtonPreferredConstructorAttribute;
namespace DevExpress.XtraBars.Docking2010 {
	public class WindowsUISeparator : BaseSeparator, IButton, IButtonProperties {
		public WindowsUISeparator() { }
		public WindowsUISeparator(Image image) : this(image, true, -1) { }
		[BaseHeaderButtonPreferredConstructor]
		public WindowsUISeparator(Image image, bool visible, int visibleIndex)
			: this() {
			Image = image;
			Visible = visible;
			VisibleIndex = visibleIndex;
		}
		[DefaultValue(false), Category(DockConsts.AppearanceCategory)]
		public new bool IsLeft {
			get { return GetIsLeft(); }
			set { SetIsLeft(value); }
		}
		protected internal IButtonsPanel Owner {
			get { return GetOwner(); }
		}
		protected internal IButtonsPanel NativeOwner {
			get { return GetNativeOwner(); }
		}
	}
	public class WindowsUIButton : BaseButton, IButton, IButtonProperties, Docking2010.Views.WindowsUI.IHeaderButton, DevExpress.Utils.MVVM.ISupportCommandBinding {
		ImageLocation imageLocationCore;
		bool enableImageTransparencyCore;
		public WindowsUIButton()
			: base() {
			enableImageTransparencyCore = false;
		}
		[BaseHeaderButtonPreferredConstructor]
		public WindowsUIButton(string caption, ButtonStyle style)
			: this(caption, null, -1, ImageLocation.Default, style, -1) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public WindowsUIButton(string caption, Image image, int imageIndex, ButtonStyle style, int groupIndex)
			: this(caption, image, imageIndex, ImageLocation.Default, style, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public WindowsUIButton(string caption, string imageUri, ButtonStyle style, int groupIndex)
			: this(caption, imageUri, ImageLocation.Default, style, groupIndex) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public WindowsUIButton(string caption, Image image)
			: this(caption, image, -1, ImageLocation.Default, ButtonStyle.PushButton, -1) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public WindowsUIButton(string caption, Image image, string imageUri)
			: this(caption, image, imageUri, -1, ImageLocation.Default, ButtonStyle.PushButton, String.Empty, true, -1, true, null, true, false, true, null, null, -1, false, false) {
		}
		public WindowsUIButton(string caption, Image image, int imageIndex, ImageLocation imageLocation, ButtonStyle style, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, string.Empty, true, -1, groupIndex) {
		}
		public WindowsUIButton(string caption, string imageUri, ImageLocation imageLocation, ButtonStyle style, int groupIndex)
			: this(caption, imageUri, imageLocation, style, string.Empty, true, -1, groupIndex) {
		}
		public WindowsUIButton(string caption, Image image, int imageIndex, ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, true, null, groupIndex) {
		}
		public WindowsUIButton(string caption, string imageUri, ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, int groupIndex)
			: this(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, true, null, groupIndex) {
		}
		public WindowsUIButton(string caption, Image image, int imageIndex, ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, true, false, true, null, null, groupIndex, false, false) {
		}
		public WindowsUIButton(string caption, string imageUri, ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, int groupIndex)
			: this(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, true, false, true, null, null, groupIndex, false, false) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public WindowsUIButton(string caption, Image image, int imageIndex, ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex, bool isLeft, bool enableImageTransparency)
			: this(caption, image, string.Empty, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, glyphs, tag, groupIndex, isLeft, enableImageTransparency) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public WindowsUIButton(string caption, string imageUri, ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex, bool isLeft, bool enableImageTransparency)
			: this(caption, null, imageUri, -1, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, glyphs, tag, groupIndex, isLeft, enableImageTransparency) {
		}
		[BaseHeaderButtonPreferredConstructor]
		public WindowsUIButton(string caption, Image image, string imageUri, int imageIndex, ImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, object glyphs, object tag, int groupIndex, bool isLeft, bool enableImageTransparency)
			: this() {
			Image = image;
			ImageUri = imageUri;
			Caption = caption;
			ImageIndex = imageIndex;
			ImageLocation = imageLocation;
			Visible = visible;
			UseCaption = useCaption;
			Enabled = enabled;
			UseImage = useImage;
			VisibleIndex = visibleIndex;
			Checked = @checked;
			Style = style;
			ToolTip = toolTip;
			SuperTip = superTip;
			Glyphs = glyphs;
			Tag = tag;
			GroupIndex = groupIndex;
			IsLeft = isLeft;
			EnableImageTransparency = enableImageTransparency;
		}
		[DefaultValue(ImageLocation.Default), Category(DockConsts.AppearanceCategory)]
		public ImageLocation ImageLocation {
			get { return imageLocationCore; }
			set { SetValue(ref imageLocationCore, value, "ImageLocation"); }
		}
		DevExpress.XtraEditors.ButtonPanel.ImageLocation IButtonProperties.ImageLocation {
			get { return (DevExpress.XtraEditors.ButtonPanel.ImageLocation)(int)(ImageLocation); }
			set { ImageLocation = (ImageLocation)((int)value); }
		}
		[DefaultValue(false), Category(DockConsts.AppearanceCategory)]
		public new bool IsLeft {
			get { return GetIsLeft(); }
			set { SetIsLeft(value); }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("WindowsUIButtonEnableImageTransparency"),
#endif
		DefaultValue(false), Category(DockConsts.AppearanceCategory)]
		public bool EnableImageTransparency {
			get { return enableImageTransparencyCore; }
			set { SetValue(ref enableImageTransparencyCore, value, "EnableImageTransparency"); }
		}
		AppearanceObject appearanceCore = new SerializableAppearanceObject();
		[Browsable(false)]
		public new AppearanceObject Appearance { get { return base.Appearance; } }
		protected internal IButtonsPanel Owner {
			get { return GetOwner(); }
		}
		protected internal IButtonsPanel NativeOwner {
			get { return GetNativeOwner(); }
		}
		IButtonsPanelOwner DevExpress.XtraBars.Docking2010.Views.WindowsUI.IHeaderButton.Owner {
			get { return Owner != null ? Owner.Owner : null; }
		}
		public event EventHandler Click;
		protected internal void RaiseClick() {
			EventHandler handler = Click;
			if(handler != null)
				handler(this, EventArgs.Empty);
		}
		#region Commands
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				(button, canExecute) => button.Enabled = canExecute(),
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				(button, canExecute) => button.Enabled = canExecute(),
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(button, execute) => button.Click += (s, e) => execute(),
				(button, canExecute) => button.Enabled = canExecute(),
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
	}
}
