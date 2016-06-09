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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.MVVM;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors.ButtonPanel;
using CustomHeaderButtonPreferredConstructor = DevExpress.XtraEditors.Controls.EditorButtonPreferredConstructorAttribute;
namespace DevExpress.XtraBars.Docking {
	public class CustomHeaderButton : BaseButton, IButton, ISupportCommandBinding {
		public CustomHeaderButton() : base() { }
		[CustomHeaderButtonPreferredConstructor]
		public CustomHeaderButton(string caption, ButtonStyle style)
			: this(caption, null, -1, HorizontalImageLocation.Default, style, -1) {
		}
		[CustomHeaderButtonPreferredConstructor]
		public CustomHeaderButton(string caption, Image image, int imageIndex, ButtonStyle style, int groupIndex)
			: this(caption, image, imageIndex, HorizontalImageLocation.Default, style, groupIndex) {
		}
		[CustomHeaderButtonPreferredConstructor]
		public CustomHeaderButton(string caption, string imageUri, ButtonStyle style, int groupIndex)
			: this(caption, imageUri, HorizontalImageLocation.Default, style, groupIndex) {
		}
		[CustomHeaderButtonPreferredConstructor]
		public CustomHeaderButton(string caption, Image image)
			: this(caption, image, -1, HorizontalImageLocation.Default, ButtonStyle.CheckButton, -1) {
		}
		[CustomHeaderButtonPreferredConstructor]
		public CustomHeaderButton(string caption, Image image, string imageUri)
			: this(caption, image, imageUri, -1, HorizontalImageLocation.Default, ButtonStyle.CheckButton, string.Empty, true, -1, true, null, true, false, true, null, null, null, -1) {
		}
		public CustomHeaderButton(string caption, Image image, int imageIndex, HorizontalImageLocation imageLocation, ButtonStyle style, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, string.Empty, true, -1, groupIndex) {
		}
		public CustomHeaderButton(string caption, Image image, int imageIndex, HorizontalImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, true, null, groupIndex) {
		}
		public CustomHeaderButton(string caption, Image image, int imageIndex, HorizontalImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, int groupIndex)
			: this(caption, image, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, true, false, true, null, null, null, groupIndex) {
		}
		public CustomHeaderButton(string caption, string imageUri, HorizontalImageLocation imageLocation, ButtonStyle style, int groupIndex)
			: this(caption, imageUri, imageLocation, style, string.Empty, true, -1, groupIndex) {
		}
		public CustomHeaderButton(string caption, string imageUri, HorizontalImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, int groupIndex)
			: this(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, true, null, groupIndex) {
		}
		public CustomHeaderButton(string caption, string imageUri, HorizontalImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, int groupIndex)
			: this(caption, imageUri, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, true, false, true, null, null, null, groupIndex) {
		}
		[CustomHeaderButtonPreferredConstructor]
		public CustomHeaderButton(string caption, Image image, int imageIndex, HorizontalImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, AppearanceObject appearance, object glyphs, object tag, int groupIndex)
			: this(caption, image, string.Empty, imageIndex, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, appearance, glyphs, tag, groupIndex) {
		}
		[CustomHeaderButtonPreferredConstructor]
		public CustomHeaderButton(string caption, string imageUri, HorizontalImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, AppearanceObject appearance, object glyphs, object tag, int groupIndex)
			: this(caption, null, imageUri, -1, imageLocation, style, toolTip, useCaption, visibleIndex, enabled, superTip, useImage, @checked, visible, appearance, glyphs, tag, groupIndex) {
		}
		[CustomHeaderButtonPreferredConstructor]
		public CustomHeaderButton(string caption, Image image, string imageUri, int imageIndex, HorizontalImageLocation imageLocation, ButtonStyle style, string toolTip, bool useCaption, int visibleIndex, bool enabled, SuperToolTip superTip, bool useImage, bool @checked, bool visible, AppearanceObject appearance, object glyphs, object tag, int groupIndex)
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
			if(Appearance != null)
				Appearance.Assign(appearance);
		}
		protected override void OnDispose() {
			Click = null;
			base.OnDispose();
		}
		protected internal event EventHandler Click;
		protected internal void RaiseClick() {
			EventHandler handler = Click;
			if(handler != null) handler(this, EventArgs.Empty);
		}
		[DefaultValue(HorizontalImageLocation.Default), Category("Appearance")]
		public HorizontalImageLocation ImageLocation {
			get { return ConvertImageLocation(base.ImageLocationCore); }
			set { base.ImageLocationCore = (DevExpress.XtraEditors.ButtonPanel.ImageLocation)((int)value); }
		}
		static HorizontalImageLocation ConvertImageLocation(DevExpress.XtraEditors.ButtonPanel.ImageLocation location) {
			switch(location) {
				case DevExpress.XtraEditors.ButtonPanel.ImageLocation.AfterText:
					return HorizontalImageLocation.AfterText;
				case DevExpress.XtraEditors.ButtonPanel.ImageLocation.BeforeText:
					return HorizontalImageLocation.BeforeText;
				default:
					return HorizontalImageLocation.Default;
			}
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
	public class CloseButton : BaseCloseButton, IButton {
		public override string ToolTip {
			get { return DockManagerLocalizer.GetString(DockManagerStringId.CommandClose); }
		}
		public override int ImageIndex {
			get { return ButtonPainter.CloseIndex; }
		}
	}
	public class PinButton : BasePinButton, IButton {
		public override string ToolTip {
			get { return DockManagerLocalizer.GetString(Checked ? DockManagerStringId.CommandDock : DockManagerStringId.CommandAutoHide); }
		}
		public override int ImageIndex {
			get { return Checked ? ButtonPainter.HideIndex : ButtonPainter.PinDownIndex; }
		}
	}
	public class MaximizeButton : BaseMaximizeButton, IButton {
		public override string ToolTip {
			get { return DockManagerLocalizer.GetString(Checked ? DockManagerStringId.CommandRestore : DockManagerStringId.CommandMaximize); }
		}
		public override int ImageIndex {
			get { return Checked ? ButtonPainter.MinimizeIndex : ButtonPainter.MaximizeIndex; }
		}
	}
}
