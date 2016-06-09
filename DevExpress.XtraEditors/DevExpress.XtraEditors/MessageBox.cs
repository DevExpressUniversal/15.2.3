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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Text;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraEditors.Popup;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace DevExpress.XtraEditors {
	static class XtraMessageHelper {
		internal static DialogResult[] MessageBoxButtonsToDialogResults(MessageBoxButtons buttons) {
			if(!Enum.IsDefined(typeof(MessageBoxButtons), buttons)) {
				throw new InvalidEnumArgumentException("buttons", (int)buttons, typeof(DialogResult));
			}
			switch(buttons) {
				case MessageBoxButtons.OK:
					return new DialogResult[] { DialogResult.OK };
				case MessageBoxButtons.OKCancel:
					return new DialogResult[] { DialogResult.OK, DialogResult.Cancel };
				case MessageBoxButtons.AbortRetryIgnore:
					return new DialogResult[] { DialogResult.Abort, DialogResult.Retry, DialogResult.Ignore };
				case MessageBoxButtons.RetryCancel:
					return new DialogResult[] { DialogResult.Retry, DialogResult.Cancel };
				case MessageBoxButtons.YesNo:
					return new DialogResult[] { DialogResult.Yes, DialogResult.No };
				case MessageBoxButtons.YesNoCancel:
					return new DialogResult[] { DialogResult.Yes, DialogResult.No, DialogResult.Cancel };
				default:
					throw new ArgumentException("buttons");
			}
		}
		internal static Icon MessageBoxIconToIcon(MessageBoxIcon mbIcon) {
			if(!Enum.IsDefined(typeof(MessageBoxIcon), mbIcon)) {
				throw new InvalidEnumArgumentException("icon", (int)mbIcon, typeof(DialogResult));
			}
			Icon icon = GetAssociatedIcon(mbIcon);
			return NativeVista.IsWindows8 ? StockIconHelper.GetWindows8AssociatedIcon(icon) : icon;
		}
		internal static Icon GetAssociatedIcon(MessageBoxIcon icon) {
			switch(icon) {
				case MessageBoxIcon.None:
					return null;
				case MessageBoxIcon.Error:
					return SystemIcons.Error;
				case MessageBoxIcon.Exclamation:
					return SystemIcons.Exclamation;
				case MessageBoxIcon.Information:
					return SystemIcons.Information;
				case MessageBoxIcon.Question:
					return SystemIcons.Question;
				default:
					throw new ArgumentException("icon");
			}
		}
		internal static int MessageBoxDefaultButtonToInt(MessageBoxDefaultButton defButton) {
			if(!Enum.IsDefined(typeof(MessageBoxDefaultButton), defButton)) {
				throw new InvalidEnumArgumentException("defaultButton", (int)defButton, typeof(DialogResult));
			}
			switch(defButton) {
				case MessageBoxDefaultButton.Button1:
					return 0;
				case MessageBoxDefaultButton.Button2:
					return 1;
				case MessageBoxDefaultButton.Button3:
					return 2;
				default:
					throw new ArgumentException("defaultButton");
			}
		}
	}
	public static class XtraMessageBox {
		const string DefaultCaption = "";
		const IWin32Window DefaultOwner = null;
		const MessageBoxButtons DefaultButtons = MessageBoxButtons.OK;
		const MessageBoxIcon DefaultIcon = MessageBoxIcon.None;
		const MessageBoxDefaultButton DefaultDefButton = MessageBoxDefaultButton.Button1;
		public static DialogResult Show(string text) { return Show(DefaultOwner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(string text, DefaultBoolean allowHtmlText) { return Show(DefaultOwner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(IWin32Window owner, string text) { return Show(owner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(IWin32Window owner, string text, DefaultBoolean allowHtmlText) { return Show(owner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(string text, string caption) { return Show(DefaultOwner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(string text, string caption, DefaultBoolean allowHtmlText) { return Show(DefaultOwner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(IWin32Window owner, string text, string caption) { return Show(owner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(IWin32Window owner, string text, string caption, DefaultBoolean allowHtmlText) { return Show(owner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons) { return Show(DefaultOwner, text, caption, buttons, DefaultIcon, DefaultDefButton); }
	 	public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, DefaultBoolean allowHtmlText) { return Show(DefaultOwner, text, caption, buttons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) { return Show(owner, text, caption, buttons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, DefaultBoolean allowHtmlText) { return Show(owner, text, caption, buttons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) { return Show(DefaultOwner, text, caption, buttons, icon, DefaultDefButton); }
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, DefaultBoolean allowHtmlText) { return Show(DefaultOwner, text, caption, buttons, icon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) { return Show(owner, text, caption, buttons, icon, DefaultDefButton); }
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, DefaultBoolean allowHtmlText) { return Show(owner, text, caption, buttons, icon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) { return Show(DefaultOwner, text, caption, buttons, icon, defaultButton); }
		public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, DefaultBoolean allowHtmlText) { return Show(DefaultOwner, text, caption, buttons, icon, defaultButton, allowHtmlText); }
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
			return Show(owner, text, caption, XtraMessageHelper.MessageBoxButtonsToDialogResults(buttons), XtraMessageHelper.MessageBoxIconToIcon(icon), XtraMessageHelper.MessageBoxDefaultButtonToInt(defaultButton), icon);
		}
		public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, DefaultBoolean allowHtmlText) {
			return Show(owner, text, caption, XtraMessageHelper.MessageBoxButtonsToDialogResults(buttons), XtraMessageHelper.MessageBoxIconToIcon(icon), XtraMessageHelper.MessageBoxDefaultButtonToInt(defaultButton), icon, allowHtmlText);
		}
		public static DialogResult Show(UserLookAndFeel lookAndFeel, string text) { return Show(lookAndFeel, DefaultOwner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, DefaultOwner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text) { return Show(lookAndFeel, owner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, owner, text, DefaultCaption, DefaultButtons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption) { return Show(lookAndFeel, DefaultOwner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, DefaultOwner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption) { return Show(lookAndFeel, owner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, owner, text, caption, DefaultButtons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption, MessageBoxButtons buttons) { return Show(lookAndFeel, DefaultOwner, text, caption, buttons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption, MessageBoxButtons buttons, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, DefaultOwner, text, caption, buttons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons) { return Show(lookAndFeel, owner, text, caption, buttons, DefaultIcon, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, owner, text, caption, buttons, DefaultIcon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) { return Show(lookAndFeel, DefaultOwner, text, caption, buttons, icon, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, DefaultOwner, text, caption, buttons, icon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) { return Show(lookAndFeel, owner, text, caption, buttons, icon, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, owner, text, caption, buttons, icon, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) { return Show(lookAndFeel, DefaultOwner, text, caption, buttons, icon, defaultButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, DefaultOwner, text, caption, buttons, icon, defaultButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton) {
			return Show(lookAndFeel, owner, text, caption, XtraMessageHelper.MessageBoxButtonsToDialogResults(buttons), XtraMessageHelper.MessageBoxIconToIcon(icon), XtraMessageHelper.MessageBoxDefaultButtonToInt(defaultButton), icon);
		}
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, DefaultBoolean allowHtmlText) {
			return Show(lookAndFeel, owner, text, caption, XtraMessageHelper.MessageBoxButtonsToDialogResults(buttons), XtraMessageHelper.MessageBoxIconToIcon(icon), XtraMessageHelper.MessageBoxDefaultButtonToInt(defaultButton), icon, allowHtmlText);
		}
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButton, MessageBoxIcon messageBeepSound, DefaultBoolean allowHtmlText) {
			EditorsNativeMethods.MessageBeep((uint)(int)messageBeepSound);
			XtraMessageBoxForm form = new XtraMessageBoxForm();
			return form.ShowMessageBoxDialog(new XtraMessageBoxArgs(lookAndFeel, owner, text, caption, buttons, icon, defaultButton, allowHtmlText));
		}
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButton, MessageBoxIcon messageBeepSound) {
			return Show(lookAndFeel, owner, text, caption, buttons, icon, defaultButton, messageBeepSound, DefaultBoolean.Default);
		}
		public static DialogResult Show(IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButton, MessageBoxIcon messageBeepSound) {
			return Show(null, owner, text, caption, buttons, icon, defaultButton, messageBeepSound, DefaultBoolean.Default);
		}
		public static DialogResult Show(IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButton, MessageBoxIcon messageBeepSound, DefaultBoolean allowHtmlText) {
			return Show(null, owner, text, caption, buttons, icon, defaultButton, messageBeepSound, allowHtmlText);
		}
		static bool _AllowCustomLookAndFeel = false;
		public static bool AllowCustomLookAndFeel {
			get { return _AllowCustomLookAndFeel; }
			set { _AllowCustomLookAndFeel = value; }
		}
		static bool allowHtmlText = false;
		public static bool AllowHtmlText {
			get { return allowHtmlText; }
			set { allowHtmlText = value; }
		}
		static bool smartTextWrap = false;
		public static bool SmartTextWrap {
			get { return smartTextWrap; }
			set { smartTextWrap = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public static DialogResult[] MessageBoxButtonsToDialogResults(MessageBoxButtons buttons) {
			return XtraMessageHelper.MessageBoxButtonsToDialogResults(buttons);
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public static Icon MessageBoxIconToIcon(MessageBoxIcon icon) {
			return XtraMessageHelper.MessageBoxIconToIcon(icon);
		}
	}
	public static class XtraDialog {
		const string DefaultCaption = "";
		const IWin32Window DefaultOwner = null;
		const MessageBoxButtons DefaultButtons = MessageBoxButtons.OK;
		const MessageBoxDefaultButton DefaultDefButton = MessageBoxDefaultButton.Button1;
		public static DialogResult Show(Control content) { return Show(DefaultOwner, content, DefaultCaption, DefaultButtons, DefaultDefButton); }
		public static DialogResult Show(Control content, DefaultBoolean allowHtmlText) { return Show(DefaultOwner, content, DefaultCaption, DefaultButtons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(IWin32Window owner, Control content) { return Show(owner, content, DefaultCaption, DefaultButtons, DefaultDefButton); }
		public static DialogResult Show(IWin32Window owner, Control content, DefaultBoolean allowHtmlText) { return Show(owner, content, DefaultCaption, DefaultButtons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(Control content, string caption) { return Show(DefaultOwner, content, caption, DefaultButtons, DefaultDefButton); }
		public static DialogResult Show(Control content, string caption, DefaultBoolean allowHtmlText) { return Show(DefaultOwner, content, caption, DefaultButtons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(IWin32Window owner, Control content, string caption) { return Show(owner, content, caption, DefaultButtons, DefaultDefButton); }
		public static DialogResult Show(IWin32Window owner, Control content, string caption, DefaultBoolean allowHtmlText) { return Show(owner, content, caption, DefaultButtons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(Control content, string caption, MessageBoxButtons buttons) { return Show(DefaultOwner, content, caption, buttons, DefaultDefButton); }
		public static DialogResult Show(Control content, string caption, MessageBoxButtons buttons, DefaultBoolean allowHtmlText) { return Show(DefaultOwner, content, caption, buttons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(IWin32Window owner, Control content, string caption, MessageBoxButtons buttons) { return Show(owner, content, caption, buttons, DefaultDefButton); }
		public static DialogResult Show(IWin32Window owner, Control content, string caption, MessageBoxButtons buttons, DefaultBoolean allowHtmlText) { return Show(owner, content, caption, buttons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(Control content, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) { return Show(DefaultOwner, content, caption, buttons, defaultButton); }
		public static DialogResult Show(Control content, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, DefaultBoolean allowHtmlText) { return Show(DefaultOwner, content, caption, buttons, defaultButton, allowHtmlText); }
		public static DialogResult Show(IWin32Window owner, Control content, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
			return Show(owner, content, caption, XtraMessageHelper.MessageBoxButtonsToDialogResults(buttons), XtraMessageHelper.MessageBoxDefaultButtonToInt(defaultButton));
		}
		public static DialogResult Show(IWin32Window owner, Control content, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, DefaultBoolean allowHtmlText) {
			return Show(owner, content, caption, XtraMessageHelper.MessageBoxButtonsToDialogResults(buttons), XtraMessageHelper.MessageBoxDefaultButtonToInt(defaultButton), allowHtmlText);
		}
		public static DialogResult Show(UserLookAndFeel lookAndFeel, Control content) { return Show(lookAndFeel, DefaultOwner, content, DefaultCaption, DefaultButtons, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, Control content, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, DefaultOwner, content, DefaultCaption, DefaultButtons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content) { return Show(lookAndFeel, owner, content, DefaultCaption, DefaultButtons, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, owner, content, DefaultCaption, DefaultButtons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, Control content, string caption) { return Show(lookAndFeel, DefaultOwner, content, caption, DefaultButtons, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, Control content, string caption, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, DefaultOwner, content, caption, DefaultButtons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, string caption) { return Show(lookAndFeel, owner, content, caption, DefaultButtons, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, string caption, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, owner, content, caption, DefaultButtons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, Control content, string caption, MessageBoxButtons buttons) { return Show(lookAndFeel, DefaultOwner, content, caption, buttons, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, Control content, string caption, MessageBoxButtons buttons, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, DefaultOwner, content, caption, buttons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, string caption, MessageBoxButtons buttons) { return Show(lookAndFeel, owner, content, caption, buttons, DefaultDefButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, string caption, MessageBoxButtons buttons, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, owner, content, caption, buttons, DefaultDefButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, Control content, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) { return Show(lookAndFeel, DefaultOwner, content, caption, buttons, defaultButton); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, Control content, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, DefaultBoolean allowHtmlText) { return Show(lookAndFeel, DefaultOwner, content, caption, buttons, defaultButton, allowHtmlText); }
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton) {
			return Show(lookAndFeel, owner, content, caption, XtraMessageHelper.MessageBoxButtonsToDialogResults(buttons), XtraMessageHelper.MessageBoxDefaultButtonToInt(defaultButton));
		}
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, string caption, MessageBoxButtons buttons, MessageBoxDefaultButton defaultButton, DefaultBoolean allowHtmlText) {
			return Show(lookAndFeel, owner, content, caption, XtraMessageHelper.MessageBoxButtonsToDialogResults(buttons), XtraMessageHelper.MessageBoxDefaultButtonToInt(defaultButton), allowHtmlText);
		}
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, string caption, DialogResult[] buttons, int defaultButton) {
			return Show(lookAndFeel, owner, content, caption, buttons, defaultButton, DefaultBoolean.Default);
		}
		public static DialogResult Show(IWin32Window owner, Control content, string caption, DialogResult[] buttons, int defaultButton) {
			return Show(null, owner, content, caption, buttons, defaultButton, DefaultBoolean.Default);
		}
		public static DialogResult Show(IWin32Window owner, Control content, string caption, DialogResult[] buttons, int defaultButton, DefaultBoolean allowHtmlText) {
			return Show(null, owner, content, caption, buttons, defaultButton, allowHtmlText);
		}
		public static DialogResult Show(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, string caption, DialogResult[] buttons, int defaultButton, DefaultBoolean allowHtmlText) {
			XtraDialogForm form = new XtraDialogForm();
			return form.ShowMessageBoxDialog(new XtraDialogArgs(lookAndFeel, owner, content, caption, buttons, defaultButton, allowHtmlText));
		}
		static bool _AllowCustomLookAndFeel = false;
		public static bool AllowCustomLookAndFeel {
			get { return _AllowCustomLookAndFeel; }
			set { _AllowCustomLookAndFeel = value; }
		}
		static bool allowHtmlText = false;
		public static bool AllowHtmlText {
			get { return allowHtmlText; }
			set { allowHtmlText = value; }
		}
	}
	#region MessageArgs
	public class XtraBaseArgs {
		IWin32Window owner;
		string caption;
		DialogResult[] buttons;
		int defaultButtonIndex;
		UserLookAndFeel lookAndFeel;
		DefaultBoolean allowHtmlText;
		public XtraBaseArgs(UserLookAndFeel lookAndFeel, IWin32Window owner, string caption, DialogResult[] buttons, int defaultButtonIndex, DefaultBoolean allowHtmlText) {
			this.lookAndFeel = lookAndFeel;
			this.owner = owner;
			this.caption = caption;
			this.buttons = buttons;
			this.defaultButtonIndex = defaultButtonIndex;
			this.allowHtmlText = allowHtmlText;
		}
		public DefaultBoolean AllowHtmlText {
			get { return allowHtmlText; }
			set { allowHtmlText = value; }
		}
		public UserLookAndFeel LookAndFeel {
			get { return lookAndFeel; }
			set { lookAndFeel = value; }
		}
		public UserLookAndFeel GetLookAndFeel() {
			if(lookAndFeel != null) return lookAndFeel;
			XtraForm form = Owner as XtraForm;
			if(form != null) return form.LookAndFeel;
			return null;
		}
		public string Caption { get { return caption; } set { caption = value; } }
		public int DefaultButtonIndex { get { return defaultButtonIndex; } set { defaultButtonIndex = value; } }
		public IWin32Window Owner { get { return owner; } set { owner = value; } }
		public DialogResult[] Buttons { get { return buttons; } set { buttons = value; } }
	}
	public class XtraMessageBoxArgs : XtraBaseArgs {
		Icon icon;
		string text;
		public XtraMessageBoxArgs() : 
			this(null, string.Empty, string.Empty, new DialogResult[0], null, 0) { }
		public XtraMessageBoxArgs(IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButtonIndex) :
			this(null, owner, text, caption, buttons, icon, defaultButtonIndex) { }
		public XtraMessageBoxArgs(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButtonIndex) :
			this(lookAndFeel, owner, text, caption, buttons, icon, defaultButtonIndex, DefaultBoolean.Default){ }
		public XtraMessageBoxArgs(UserLookAndFeel lookAndFeel, IWin32Window owner, string text, string caption, DialogResult[] buttons, Icon icon, int defaultButtonIndex, DefaultBoolean allowHtmlText) :
			base(lookAndFeel, owner, caption, buttons, defaultButtonIndex, allowHtmlText){
			this.text = text;
			this.icon = icon;
		}
		public string Text { get { return text; } set { text = value; } }
		public Icon Icon { get { return icon; } set { icon = value; } }
	}
	public class XtraDialogArgs : XtraBaseArgs {
		Control content;
		public XtraDialogArgs() : 
			this(null, null, string.Empty, new DialogResult[0], 0) { }
		public XtraDialogArgs(IWin32Window owner, Control content, string caption, DialogResult[] buttons, int defaultButtonIndex) :
			this(null, owner, content, caption, buttons, defaultButtonIndex) { }
		public XtraDialogArgs(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, string caption, DialogResult[] buttons, int defaultButtonIndex) :
			this(lookAndFeel, owner, content, caption, buttons, defaultButtonIndex, DefaultBoolean.Default) { }
		public XtraDialogArgs(UserLookAndFeel lookAndFeel, IWin32Window owner, Control content, string caption, DialogResult[] buttons, int defaultButtonIndex, DefaultBoolean allowHtmlText) :
			base(lookAndFeel, owner, caption, buttons, defaultButtonIndex, allowHtmlText) {
			this.content = content;
		}
		public Control Content { get { return content; } set { content = value; } }
	}
	#endregion
	#region MessageForms
	public abstract class XtraBaseForm : XtraForm {
		XtraBaseArgs message;
		SimpleButton[] buttons;
		public XtraBaseForm() {
			this.KeyPreview = true;
			this.message = CreateMessage();
		}
		public bool AssignButtonsFont { get; set; }
		protected virtual SimpleButton[] Buttons { get { return buttons; } set { buttons = value; } }
		protected virtual int Spacing { get { return 12; } }
		protected internal abstract XtraBaseArgs CreateMessage();
		protected internal abstract DevExpress.Accessibility.BaseAccessible DXAccessible { get; }
		protected internal virtual XtraBaseArgs Message { get { return message; } set { message = value; } }
		protected internal DevExpress.Accessibility.BaseAccessible DxAccessibleCore;
		protected internal AccessibleObject GetParentAccessible() { return base.CreateAccessibilityInstance().Parent; }
		protected override bool AllowSkinForEmptyText { get { return true; } }
		protected override AccessibleObject CreateAccessibilityInstance() {
			if(DXAccessible == null) return base.CreateAccessibilityInstance();
			return DXAccessible.Accessible;
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
			if(IsCopyShortcut((int)keyData))
				CopyContent();
			return base.ProcessCmdKey(ref msg, keyData);
		}
		protected virtual bool IsCopyShortcut(int value) {
			if(value == (int)Shortcut.CtrlC) return true;
			if(value == (int)Shortcut.CtrlIns) return true;
			return false;
		}
		protected virtual void CopyContent() { }
		protected virtual string GetButtonText(DialogResult target) {
			switch(target) {
				case DialogResult.Abort:
					return Localizer.Active.GetLocalizedString(StringId.XtraMessageBoxAbortButtonText);
				case DialogResult.Cancel:
					return Localizer.Active.GetLocalizedString(StringId.XtraMessageBoxCancelButtonText);
				case DialogResult.Ignore:
					return Localizer.Active.GetLocalizedString(StringId.XtraMessageBoxIgnoreButtonText);
				case DialogResult.No:
					return Localizer.Active.GetLocalizedString(StringId.XtraMessageBoxNoButtonText);
				case DialogResult.OK:
					return Localizer.Active.GetLocalizedString(StringId.XtraMessageBoxOkButtonText);
				case DialogResult.Retry:
					return Localizer.Active.GetLocalizedString(StringId.XtraMessageBoxRetryButtonText);
				case DialogResult.Yes:
					return Localizer.Active.GetLocalizedString(StringId.XtraMessageBoxYesButtonText);
				default:
					return '&' + target.ToString();
			}
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(this.UseWaitCursor)
				this.UseWaitCursor = false;
		}
		protected override void OnClosing(CancelEventArgs e) {
			if(this.CancelButton == null && this.DialogResult == DialogResult.Cancel)
				e.Cancel = true;
			base.OnClosing(e);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(this.Visible && !this.ContainsFocus)
				this.Activate();
		}
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		protected static Form GuessOwner() {
			Form frm = Form.ActiveForm;
			if(frm == null || frm.InvokeRequired)
				return null;
			while(frm != null && frm.Owner != null && !frm.ShowInTaskbar && !frm.TopMost)
				frm = frm.Owner;
			return frm;
		}
		protected virtual Font GetCaptionFont() {
			return IsCustomPainter ? (Font)new AppearanceObject(FormPainter.GetDefaultAppearance()).Font.Clone() : ControlUtils.GetCaptionFont();
		}
		protected virtual int GetCloseButtonWidth() {
			if(!IsCustomPainter) return SystemInformation.CaptionButtonSize.Width;
			return new DevExpress.Skins.XtraForm.FormCaptionButtonSkinPainter(FormPainter).CalcObjectMinBounds(new DevExpress.Skins.XtraForm.FormCaptionButton(DevExpress.Skins.XtraForm.FormCaptionButtonKind.Close)).Width;
		}
		protected Size GetBordersSizes() {
			if(IsCustomPainter) {
				DevExpress.Skins.SkinPaddingEdges margins = FormPainter.Margins;
				return new Size(margins.Left + margins.Right, margins.Top + margins.Bottom);
			}
			return new Size(2 * SystemInformation.FixedFrameBorderSize.Width, 2 * SystemInformation.FixedFrameBorderSize.Height + SystemInformation.CaptionHeight);
		}
		protected virtual void CreateButtons() {
			if(Message.Buttons == null || Message.Buttons.Length <= 0)
				throw new ArgumentException("At least one button must be specified", "buttons");
			buttons = new SimpleButton[Message.Buttons.Length];
			int maxButtonHeight = 0;
			for(int i = 0; i < buttons.Length; ++i) {
				int currentButtonIndex = (Message.DefaultButtonIndex + i) % buttons.Length;
				SimpleButton currentButton = new SimpleButton();
				if(AssignButtonsFont) currentButton.Font = Font;
				currentButton.LookAndFeel.Assign(LookAndFeel);
				buttons[currentButtonIndex] = currentButton;
				currentButton.DialogResult = Message.Buttons[currentButtonIndex];
				if(currentButton.DialogResult == DialogResult.None)
					throw new ArgumentException("The 'DialogResult.None' button cannot be specified", "buttons");
				if(currentButton.DialogResult == DialogResult.Cancel)
					this.CancelButton = currentButton;
				currentButton.Text = GetButtonText(currentButton.DialogResult);
				Size best = currentButton.CalcBestSize();
				if(best.Width > currentButton.Width)
					currentButton.Width = best.Width;
				if(best.Height > maxButtonHeight && best.Height > currentButton.Height)
					maxButtonHeight = best.Height;
				this.Controls.Add(currentButton);
				if(i == 0)
					this.AcceptButton = currentButton;
			}
			if(maxButtonHeight > 0) {
				foreach(SimpleButton currentButton in buttons) {
					currentButton.Height = maxButtonHeight;
				}
			}
			if(buttons.Length == 1)
				this.CancelButton = buttons[0];
			if(this.CancelButton == null)
				this.ControlBox = false;
		}
		protected virtual bool AllowCustomLookAndFeel { get { return false; } }
		protected virtual bool AllowHtmlText { get { return false; } }
		protected static bool IsMessageBoxFormNeedTopMost(IWin32Window owner) {
			if(Form.ActiveForm != null && Form.ActiveForm is ISupportTopMost)
				return ((ISupportTopMost)Form.ActiveForm).IsTopMost;
			Control cnt = owner as Control;
			if(cnt != null) {
				var frm = cnt.FindForm();
				if(frm != null)
					return frm.TopMost;
			}
			Form active = Form.ActiveForm;
			if(active == null || active.InvokeRequired)
				return false;
			return active.TopMost;
		}
		protected virtual DialogResult DoShowDialog(IWin32Window owner) {
			DialogResult result = ShowDialog(owner);
			this.Dispose();
			if(Array.IndexOf(Message.Buttons, result) != -1)
				return result;
			else
				return Message.Buttons[0];
		}
		protected virtual DialogResult ShowMessageBoxDialog() {
			if(Message.GetLookAndFeel() != null)
				LookAndFeel.Assign(Message.GetLookAndFeel());
			if(!AllowCustomLookAndFeel) {
				if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) {
					ActiveLookAndFeelStyle active = UserLookAndFeel.Default.ActiveStyle;
					if(active == ActiveLookAndFeelStyle.Office2003) {
						LookAndFeel.SetStyle(LookAndFeelStyle.Office2003, true, false, "");
					}
					else
						LookAndFeel.SetDefaultStyle();
				}
			}
			if(AllowHtmlText) this.HtmlText = Message.Caption;
			else this.Text = StringPainter.Default.RemoveFormat(Message.Caption, false);
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MinimizeBox = false;
			MaximizeBox = false;
			IWin32Window owner = Message.Owner;
			if(owner == null) {
				owner = GuessOwner();
			}
			if(owner != null) {
				Control ownerControl = owner as Control;
				if(ownerControl != null) {
					if(!ownerControl.Visible) {
						owner = null;
					}
					else {
						Form ownerForm = ownerControl.FindForm();
						if(ownerForm != null) {
							if((!ownerForm.Visible)
								|| ownerForm.WindowState == FormWindowState.Minimized
								) {
								owner = null;
							}
							if(owner != null) {
								this.RightToLeft = ownerForm.RightToLeft;
								this.RightToLeftLayout = ownerForm.RightToLeftLayout;
							}
						}
					}
				}
			}
			if(owner == null) {
				ShowInTaskbar = true;
				StartPosition = FormStartPosition.CenterScreen;
			}
			else {
				ShowInTaskbar = false;
				StartPosition = FormStartPosition.CenterParent;
			}
			CreateButtons();
			CreateContent();
			CalcFinalSizes(); 
			if(IsMessageBoxFormNeedTopMost(owner))
				this.TopMost = true;
			return DoShowDialog(owner);
		}
		protected virtual void CreateContent() { }
		protected virtual void CalcFinalSizes() {
			int buttonsTotalWidth = 0;
			foreach(SimpleButton button in Buttons) {
				if(buttonsTotalWidth != 0)
					buttonsTotalWidth += Spacing;
				buttonsTotalWidth += button.Width;
			}
			int buttonsTop;
			int wantedFormWidth;
			CalcContentSpacing(buttonsTotalWidth, out buttonsTop, out wantedFormWidth);
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			try {
				using(StringFormat fmt = TextOptions.DefaultStringFormat.Clone() as StringFormat) {
					fmt.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
					using(Font captionFont = GetCaptionFont()) {
						int captionTextWidth = (int)Math.Ceiling(ginfo.Cache.CalcTextSize(this.Text, captionFont, fmt, 0).Width);
						int captionTextWidthWithMargins = captionTextWidth + GetBordersSizes().Width;
						int captionTextWidthWithButtonsAndSpacing = captionTextWidthWithMargins;
						if(this.ControlBox)
							captionTextWidthWithButtonsAndSpacing += GetCloseButtonWidth();
						captionTextWidthWithButtonsAndSpacing += 2; 
						int maxCaptionForcedWidth = SystemInformation.WorkingArea.Width / 3 * 2;
						int captionWidth = Math.Min(maxCaptionForcedWidth, captionTextWidthWithButtonsAndSpacing);
						if(wantedFormWidth < captionWidth)
							wantedFormWidth = captionWidth;
					}
				}
			}
			finally {
				ginfo.ReleaseGraphics();
			}
			ClientSize = new Size(wantedFormWidth, buttonsTop + Buttons[0].Height + Spacing);
			CalcButtonsPosition(buttonsTotalWidth, buttonsTop, wantedFormWidth);
			CalcOffset(wantedFormWidth);
			CheckRTL();
		}
		protected virtual bool IsRightToLeft { 
			get {
				if(RightToLeftLayout) return false;
				return WindowsFormsSettings.GetIsRightToLeft(this); 
			} 
		}
		protected virtual void CheckRTL() {
			if(!IsRightToLeft) return;
			CheckButtonsRTL();
			CheckContentRTL();
		}
		protected virtual void CheckButtonsRTL() {
			if(!IsRightToLeft) return;
			for(int i = 0; i < Buttons.Length; i++) {
				Buttons[i].Bounds = ConvertBoundsToRTL(Buttons[i].Bounds, ClientRectangle);
			}
		}
		protected virtual void CheckContentRTL() { }
		protected virtual Rectangle ConvertBoundsToRTL(Rectangle bounds, Rectangle ownerBounds) {
			int x = ownerBounds.Right + ownerBounds.X - bounds.Right;
			return new Rectangle(x, bounds.Y, bounds.Width, bounds.Height);
		}
		protected virtual void CalcButtonsPosition(int buttonsTotalWidth, int buttonsTop, int wantedFormWidth) {
			int nextButtonPos = (wantedFormWidth - buttonsTotalWidth) / 2;
			for(int i = 0; i < Buttons.Length; ++i) {
				Buttons[i].Location = new Point(nextButtonPos, buttonsTop);
				nextButtonPos += Buttons[i].Width + Spacing;
			}
		}
		protected virtual void CalcOffset(int wantedFormWidth) { }
		protected virtual void CalcContentSpacing(int buttonsTotalWidth, out int buttonsTop, out int wantedFormWidth) {
			buttonsTop = 0;
			wantedFormWidth = 0;
		}
	}
	public class XtraMessageBoxForm : XtraBaseForm {
		Rectangle iconRectangle;
		Rectangle messageRectangle;
		public XtraMessageBoxForm() { }
		protected internal override DevExpress.Accessibility.BaseAccessible DXAccessible {
			get {
				if(DxAccessibleCore == null) DxAccessibleCore = new DevExpress.Accessibility.MessageBoxAccessible(this);
				return DxAccessibleCore;
			}
		}
		protected override bool AllowCustomLookAndFeel { 
			get { return XtraMessageBox.AllowCustomLookAndFeel; }
		}
		protected override bool AllowHtmlText {
			get {
				if(Message.AllowHtmlText == DefaultBoolean.Default) return XtraMessageBox.AllowHtmlText;
				return Message.AllowHtmlText == DefaultBoolean.True;
			}
		}
		protected new XtraMessageBoxArgs Message {
			get { return (XtraMessageBoxArgs)base.Message; }
			set { base.Message = value; }
		}
		protected internal override XtraBaseArgs CreateMessage() { return new XtraMessageBoxArgs(); }
		public DialogResult ShowMessageBoxDialog(XtraMessageBoxArgs message) {
			Message = message;
			return ShowMessageBoxDialog();
		}
		void CalcIconBounds() {
			this.iconRectangle = Message.Icon != null ? new Rectangle(Spacing, Spacing, Message.Icon.Width, Message.Icon.Height) : new Rectangle(Spacing, Spacing, 0, 0);
		}
		void CalcMessageBounds() {
			int messageTop, messageLeft, messageWidth, messageHeight;
			messageTop = Spacing;
			messageLeft = (Message.Icon == null) ? Spacing : (this.iconRectangle.Left + this.iconRectangle.Width + Spacing);
			int maxFormWidth = this.MaximumSize.Width;
			if(maxFormWidth <= 0)
				maxFormWidth = SystemInformation.WorkingArea.Width;
			int maxTextWidth = maxFormWidth - GetBordersSizes().Width - Spacing - messageLeft;
			if(maxTextWidth < 10)
				maxTextWidth = 10;
			int maxFormHeight = this.MaximumSize.Height;
			if(maxFormHeight <= 0)
				maxFormHeight = SystemInformation.WorkingArea.Height;
			int maxTextHeight = maxFormHeight - Spacing - Buttons[0].Height - Spacing - messageTop - GetBordersSizes().Height;
			if(maxTextHeight < 10)
				maxTextHeight = 10;
			Size textSize = AllowHtmlText ? CalcHtmlTextSize(maxTextWidth, maxTextHeight) : CalcSimpleTextSize(maxTextWidth, maxTextHeight);
			messageWidth = textSize.Width;
			messageHeight = textSize.Height;
			if(messageHeight > maxTextHeight)
				messageHeight = maxTextHeight;
			this.messageRectangle = new Rectangle(messageLeft, messageTop, messageWidth, messageHeight);
		}
		protected virtual Size CalcSimpleTextSize(int maxTextWidth, int maxTextHeight) {
			return GuessBestTextSize(Message.Text, maxTextWidth, maxTextHeight);
		}
		protected virtual Size CalcHtmlTextSize(int maxTextWidth, int maxTextHeight) {
			return StringPainter.Default.Calculate(CreateGraphics(), GetPaintAppearance(), OptionsHtmlString, Message.Text, new Rectangle(0, 0, maxTextWidth, maxTextHeight)).Bounds.Size;
		}
		[Obsolete("Use XtraMessageBox.SmartTextWrap"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool? ForceShortBox;
		static int maxTextWidth = 400;
		public static int MaxTextWidth {
			get { return maxTextWidth; }
			set { maxTextWidth = value; }
		}
		protected virtual bool ShouldUseNativePainter {
			get { return (Message.Text != null) && (Message.Text.Length > FontCache.MaxMultiLineChars); }
		}
		protected virtual int CalcMaxWidth(int width, float delta) {
			return (int)Math.Ceiling(width * delta);
		}
		protected virtual bool IsShortBox { 
			get {
#pragma warning disable 0612, 0618
				if(ForceShortBox == null) return XtraMessageBox.SmartTextWrap;
				return (bool)ForceShortBox;
#pragma warning restore 0612, 0618
			} 
		}
		protected virtual Size CalcTextSize(Graphics g, string text, int maxTextWidth, int maxTextHeight) {
			if(!ShouldUseNativePainter) 
				return GetPaintAppearance().CalcTextSize(g, text, maxTextWidth).ToSize();
			try {
				return g.MeasureString(text, GetPaintAppearance().Font, maxTextWidth).ToSize();
			}
			catch {
				return Size.Empty;
			}
		}
		protected virtual Size GuessBestTextSize(string text, int maxTextWidth, int maxTextHeight) {
			GraphicsInfo ginfo = new GraphicsInfo();
			ginfo.AddGraphics(null);
			Size size = Size.Empty;
			try {
				size = CalcTextSize(ginfo.Graphics, text, maxTextWidth, maxTextHeight);
				if(!IsShortBox) return size;
				size = CalcTextSize(ginfo.Graphics, text, MaxTextWidth, 0);
				if(size.Height < maxTextHeight) return size;
				size = CalcTextSize(ginfo.Graphics, text, CalcMaxWidth(maxTextWidth, 0.625f), 0);
				if(size.Height < maxTextHeight) return size;
				size = CalcTextSize(ginfo.Graphics, text, CalcMaxWidth(maxTextWidth, 0.75f), 0);
				if(size.Height < maxTextHeight) return size;
				size = CalcTextSize(ginfo.Graphics, text, CalcMaxWidth(maxTextWidth, 0.875f), 0);
				return size;
			}
			finally {
				ginfo.ReleaseGraphics();
			}
		}
		protected override void CalcOffset(int wantedFormWidth) {
			if(Message.Icon == null) {
				this.messageRectangle.Offset((wantedFormWidth - (this.messageRectangle.Right + Spacing)) / 2, 0);
			}
			if(Message.Icon != null && this.messageRectangle.Height < this.iconRectangle.Height) {
				this.messageRectangle.Offset(0, (this.iconRectangle.Height - this.messageRectangle.Height) / 2);
			}
		}
		protected override void CalcContentSpacing(int buttonsTotalWidth, out int buttonsTop, out int wantedFormWidth) {
			buttonsTop = this.messageRectangle.Bottom + Spacing;
			if(Message.Icon != null && (this.iconRectangle.Bottom + Spacing > buttonsTop))
				buttonsTop = this.iconRectangle.Bottom + Spacing;
			wantedFormWidth = this.MinimumSize.Width;
			if(wantedFormWidth == 0)
				wantedFormWidth = SystemInformation.MinimumWindowSize.Width;
			if(wantedFormWidth < this.messageRectangle.Right + Spacing)
				wantedFormWidth = this.messageRectangle.Right + Spacing;
			if(wantedFormWidth < buttonsTotalWidth + Spacing + Spacing)
				wantedFormWidth = buttonsTotalWidth + Spacing + Spacing;
		}
		protected override void CopyContent() {
			Clipboard.SetDataObject(Message.Caption + Environment.NewLine + Environment.NewLine + Message.Text, true);
		}
		public Rectangle MessageRectangle { get { return messageRectangle; } }
		public Rectangle IconRectangle { get { return iconRectangle; } }
		protected override void CreateContent() {
			CalcIconBounds();
			CalcMessageBounds(); 
		}
		AppearanceObject GetPaintAppearance() {
			AppearanceObject paintAppearance = new AppearanceObject(Appearance, DefaultAppearance);
			paintAppearance.TextOptions.WordWrap = WordWrap.Wrap;
			paintAppearance.TextOptions.VAlignment = VertAlignment.Top;
			paintAppearance.TextOptions.Trimming = IsShortBox ? Trimming.Default : Trimming.EllipsisCharacter;
			paintAppearance.TextOptions.RightToLeft = IsRightToLeft;
			return paintAppearance;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			using(GraphicsCache gcache = new GraphicsCache(e)) {
				DrawIcon(gcache);
				DrawText(gcache);
			}
		}
		protected virtual void DrawIcon(GraphicsCache cache) {
			if(Message.Icon == null) return;
			cache.Graphics.DrawIcon(Message.Icon, this.iconRectangle);
		}
		protected virtual void DrawText(GraphicsCache cache) {
			if(AllowHtmlText) DrawHtmlString(cache);
			else DrawSimpleString(cache);
		}
		TextOptions optionsHtmlString;
		protected virtual TextOptions OptionsHtmlString { get { return optionsHtmlString ?? (optionsHtmlString = CreateOptionsHtmlString()); } }
		protected virtual TextOptions CreateOptionsHtmlString() {
			return new TextOptions(HorzAlignment.Default, VertAlignment.Center, WordWrap.Wrap, Trimming.Default);
		}
		protected virtual void DrawHtmlString(GraphicsCache cache) {
			StringPainter.Default.DrawString(cache, GetPaintAppearance(), Message.Text, MessageRectangle, OptionsHtmlString);
		}
		protected virtual void DrawNativeSimpleString(GraphicsCache cache) {
			if(MessageRectangle.Height == 0 || MessageRectangle.Width == 0) return;
			SmoothingMode mode = cache.Graphics.SmoothingMode;
			cache.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			cache.Graphics.DrawString(Message.Text, GetPaintAppearance().Font, GetPaintAppearance().GetForeBrush(cache), MessageRectangle);
			cache.Graphics.SmoothingMode = mode;
		}
		protected virtual void DrawSimpleStringCore(GraphicsCache cache) {
			GetPaintAppearance().DrawString(cache, Message.Text, MessageRectangle);
		}
		protected virtual void DrawSimpleString(GraphicsCache cache) {
			if(!ShouldUseNativePainter) DrawSimpleStringCore(cache);
			else DrawNativeSimpleString(cache);
		}
		protected override void CheckContentRTL() {
			if(!IsRightToLeft) return;
			messageRectangle = ConvertBoundsToRTL(messageRectangle, ClientRectangle);
			iconRectangle = ConvertBoundsToRTL(iconRectangle, ClientRectangle);
		}
	}
	public class XtraDialogForm : XtraBaseForm {
		public XtraDialogForm() : base() { }
		protected internal override XtraBaseArgs CreateMessage() {
			return new XtraDialogArgs();
		}
		protected internal override DevExpress.Accessibility.BaseAccessible DXAccessible {
			get {
				if(DxAccessibleCore == null) DxAccessibleCore = new DevExpress.Accessibility.MessageDialogAccessible(this);
				return DxAccessibleCore;
			}
		}
		protected new XtraDialogArgs Message {
			get { return (XtraDialogArgs)base.Message; }
			set { base.Message = value; }
		}
		public DialogResult ShowMessageBoxDialog(XtraDialogArgs message) {
			Message = message;
			return ShowMessageBoxDialog();
		}
		DefaultSkinProvider defaultSkinProvider = new DefaultSkinProvider();
		public virtual ISkinProvider SkinProvider {
			get {
				if(LookAndFeel.ActiveStyle != ActiveLookAndFeelStyle.Skin) return defaultSkinProvider;
				return LookAndFeel.ActiveLookAndFeel;
			}
		}
		protected SkinElement GetElement(string name) {
			return RibbonSkins.GetSkin(SkinProvider)[name];
		}
		protected virtual SkinElementInfo GetSeparator() {
			Rectangle r = new Rectangle(new Point(ContentRectangle.X, ContentRectangle.Bottom), new Size(ContentRectangle.Width, 1));
			return new SkinElementInfo(CommonSkins.GetSkin(SkinProvider)[CommonSkins.SkinLabelLine], r);
		}
		protected virtual void DrawSeparator(Graphics g) {
			using(GraphicsCache cache = new GraphicsCache(g)) {
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, GetSeparator());
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			DrawSeparator(e.Graphics);
		}
		protected override bool AllowCustomLookAndFeel {
			get { return XtraDialog.AllowCustomLookAndFeel; }
		}
		protected override bool AllowHtmlText {
			get {
				if(Message.AllowHtmlText == DefaultBoolean.Default) return XtraDialog.AllowHtmlText;
				return Message.AllowHtmlText == DefaultBoolean.True;
			}
		}
		protected override void CalcContentSpacing(int buttonsTotalWidth, out int buttonsTop, out int wantedFormWidth) {
			buttonsTop = ContentRectangle.Bottom + Spacing;
			wantedFormWidth = this.MinimumSize.Width;
			if(wantedFormWidth == 0)
				wantedFormWidth = SystemInformation.MinimumWindowSize.Width;
			if(wantedFormWidth < this.ContentRectangle.Right)
				wantedFormWidth = this.ContentRectangle.Right;
			if(wantedFormWidth < buttonsTotalWidth + Spacing + Spacing)
				wantedFormWidth = buttonsTotalWidth + Spacing + Spacing;
		}
		protected override void Dispose(bool disposing) {
			Controls.Clear();
			base.Dispose(disposing);
		}
		protected override void CreateContent() {
			Controls.Add(Message.Content);
		}
		protected override void CalcButtonsPosition(int buttonsTotalWidth, int buttonsTop, int wantedFormWidth) {
			int nextButtonPos = wantedFormWidth - buttonsTotalWidth - Spacing;
			for(int i = 0; i < Buttons.Length; ++i) {
				Buttons[i].Location = new Point(nextButtonPos, buttonsTop);
				nextButtonPos += Buttons[i].Width + Spacing;
			}
		}
		public Rectangle ContentRectangle { get { return Message.Content == null ? Rectangle.Empty : Message.Content.Bounds; } }
	}
	#endregion
}
namespace DevExpress.XtraEditors.MVVM.Services{
	using DevExpress.Utils.MVVM.Services;
	public sealed class XtraDialogFormFactory : IDialogFormFactory {
		IDialogForm IDialogFormFactory.Create() {
			return new DialogForm();
		}
		#region DialogForm
		class DialogForm : XtraEditors.XtraDialogForm, IDialogForm {
			Control IDialogForm.Content { 
				get { return Message.Content; } 
			}
			DialogResult IDialogForm.ShowDialog(IWin32Window owner, Control content, string caption, DialogResult[] buttons) {
				return ShowMessageBoxDialog(new XtraEditors.XtraDialogArgs() { Owner = owner, Content = content, Caption = caption, Buttons = buttons });
			}
		}
		#endregion DialogForm
	}
}
