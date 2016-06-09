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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.Web.Internal;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	public static class ShortcutConsts {
		public const string Bold = "Ctrl+B";
		public const string Italic = "Ctrl+I";
		public const string JustifyLeft = "Ctrl+L";
		public const string JustifyCenter = "Ctrl+E";
		public const string JustifyRight = "Ctrl+R";
		public const string JustifyFull = "Ctrl+J";
		public const string Undo = "Ctrl+Z";
		public const string Redo = "Ctrl+Y";
		public const string InsertLinkDialog = "Ctrl+K";
		public const string InsertImageDialog = "Ctrl+G";
		public const string UnLink = "Ctrl+Shift+K";
		public const string Print = "Ctrl+P";
		public const string FullScreen = "F11";
		public const string Paste = "Ctrl+V";
		public const string Copy = "Ctrl+C";
		public const string Cut = "Ctrl+X";
		public const string ShowSearchPanel = "Ctrl+F";
		public const string FindAndReplaceDialog = "Ctrl+H";
		public const string ShowIntelliSense = "Ctrl+Space";
	}
}
namespace DevExpress.Web.ASPxHtmlEditor {
	using DevExpress.Web.ASPxHtmlEditor.Internal;
	using System.Web.UI;
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class HtmlEditorShortcutCollection : Collection<HtmlEditorShortcut> {
		public HtmlEditorShortcutCollection(ASPxHtmlEditor editor)
			: base(editor) { }
		const string customNamePrefix = "customdialog;";
		public void CreateDefaultItems() {
			Add(
				new HtmlEditorShortcut(ShortcutConsts.Bold, "bold"),
				new HtmlEditorShortcut(ShortcutConsts.Italic, "italic"),
				new HtmlEditorShortcut(ShortcutConsts.JustifyLeft, "justifyleft"),
				new HtmlEditorShortcut(ShortcutConsts.JustifyCenter, "justifycenter"),
				new HtmlEditorShortcut(ShortcutConsts.JustifyRight, "justifyright"),
				new HtmlEditorShortcut(ShortcutConsts.JustifyFull, "justifyfull"),
				new HtmlEditorShortcut(ShortcutConsts.Undo, "undo"),
				new HtmlEditorShortcut(ShortcutConsts.Redo, "redo"),
				new HtmlEditorShortcut(ShortcutConsts.InsertLinkDialog, "insertlinkdialog"),
				new HtmlEditorShortcut(ShortcutConsts.InsertImageDialog, "insertimagedialog"),
				new HtmlEditorShortcut(ShortcutConsts.UnLink, "unlink"),
				new HtmlEditorShortcut(ShortcutConsts.Print, "print"),
				new HtmlEditorShortcut(ShortcutConsts.Paste, "paste"),
				new HtmlEditorShortcut(ShortcutConsts.Copy, "copy"),
				new HtmlEditorShortcut(ShortcutConsts.Cut, "cut"),
				new HtmlEditorShortcut(ShortcutConsts.Undo, "undo", HtmlEditorView.Html),
				new HtmlEditorShortcut(ShortcutConsts.Redo, "redo", HtmlEditorView.Html),
				new HtmlEditorShortcut(ShortcutConsts.FullScreen, "fullscreen", HtmlEditorView.Design),
				new HtmlEditorShortcut(ShortcutConsts.FullScreen, "fullscreen", HtmlEditorView.Html),
				new HtmlEditorShortcut(ShortcutConsts.FullScreen, "fullscreen", HtmlEditorView.Preview),
				new HtmlEditorShortcut(ShortcutConsts.ShowSearchPanel, "showsearchpanel"),
				new HtmlEditorShortcut(ShortcutConsts.FindAndReplaceDialog, "findandreplacedialog"),
				new HtmlEditorShortcut(ShortcutConsts.ShowIntelliSense, "showintellisense", HtmlEditorView.Html)
			);
		}
		public HtmlEditorShortcut Add(string shortcut, string actionName) {
			return Add(shortcut, actionName, ActionType.ExecuteCommand);
		}
		public HtmlEditorShortcut Add(string shortcut, string actionName, ActionType actionType) {
			HtmlEditorShortcut sc = new HtmlEditorShortcut(shortcut, actionName, actionType);
			Add(sc);
			return sc;
		}
		internal string FindShortcutByActionName(string actionName) {
			ActionType actionType = actionName.StartsWith(customNamePrefix) ? ActionType.ShowCustomDialog : ActionType.ExecuteCommand;
			if(actionType == ActionType.ShowCustomDialog)
				actionName = actionName.Substring(customNamePrefix.Length, actionName.Length - customNamePrefix.Length);
			HtmlEditorShortcut shortcutObj = Find((item) => String.Equals(item.ActionName, actionName) && item.ActionType == actionType);
			if(shortcutObj != null)
				return shortcutObj.Shortcut;
			return string.Empty;
		}
		internal void CheckDuplicates() {
			foreach(HtmlEditorShortcut sc in this) {
				bool hasDuplicate = this.Count<HtmlEditorShortcut>(t => string.Equals(t.Shortcut, sc.Shortcut)) > 1;
				if(hasDuplicate)
					throw new InvalidDataException(sc.Shortcut + " key combination alredy used");
			}
		}
	}
	public class HtmlEditorShortcut : CollectionItem {
		internal static string[] NonSymbolKeys = new string[] { "Del", "Ins", "Back", "Down", "Left", "Right", "Up", "Space" };
		internal static string[] ModificatorKeys = new string[] { "Ctrl", "Shift", "Alt" };
		public HtmlEditorShortcut() { }
		public HtmlEditorShortcut(string shortcut, string actionName)
			: this(shortcut, actionName, ActionType.ExecuteCommand) { }
		public HtmlEditorShortcut(string shortcut, string actionName, HtmlEditorView actionView)
			: this(shortcut, actionName) {
				ActionView = actionView;
		}
		public HtmlEditorShortcut(string shortcut, string actionName, ActionType actionType) {
			Shortcut = shortcut;
			ActionName = actionName;
			ActionType = actionType;
			ActionView = HtmlEditorView.Design;
		}
		protected IWebControlObject Owner { get { return Collection != null ? Collection.Owner : null; } }
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorShortcutShortcut"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false),
		TypeConverter("DevExpress.Web.ASPxHtmlEditor.Design.ShortcutConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		public string Shortcut {
			get { return GetStringProperty("ShortcutString", string.Empty); }
			set {
				string shortcut = string.IsNullOrEmpty(value) ? string.Empty : GetShortcutString(value);
				if (shortcut == null)
					throw new KeyNotFoundException();
				SetStringProperty("ShortcutString", string.Empty, shortcut);
			}
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorShortcutActionName"),
#endif
		DefaultValue(""), AutoFormatDisable, NotifyParentProperty(true), Localizable(false)]
		public string ActionName {
			get { return GetStringProperty("ActionName", ""); }
			set { SetStringProperty("ActionName", string.Empty, value); }
		}
		[
#if !SL
	DevExpressWebASPxHtmlEditorLocalizedDescription("HtmlEditorShortcutActionType"),
#endif
		DefaultValue(ActionType.ExecuteCommand), AutoFormatDisable, NotifyParentProperty(true), Localizable(false)]
		public ActionType ActionType {
			get { return (ActionType)GetEnumProperty("ActionType", ActionType.ExecuteCommand); }
			set { SetEnumProperty("ActionType", ActionType.ExecuteCommand, value); }
		}
		[AutoFormatDisable, DefaultValue(HtmlEditorView.Design), NotifyParentProperty(true)]
		public HtmlEditorView ActionView {
			get { return (HtmlEditorView)GetEnumProperty("ActionView", HtmlEditorView.Design); }
			set { SetEnumProperty("ActionView", HtmlEditorView.Design, value); }
		}
		internal static string GetShortcutString(string input) {
			List<string> existsKeysExprs = new List<string>(NonSymbolKeys);
			existsKeysExprs.AddRange(new string[] { "F([1-9]|1[0-2])", "[0-9A-Z]" });
			List<string> selectedKeys = new List<string>();
			string mainKey = null;
			string[] pieces = input.Split(new string[] { "+" }, StringSplitOptions.RemoveEmptyEntries);
			foreach(string piece in pieces) {
				string key = FindKey(piece, ModificatorKeys);
				if(!string.IsNullOrEmpty(key) && !selectedKeys.Contains(key)) {
					selectedKeys.Add(key);
					continue;
				}
				key = FindKey(piece, existsKeysExprs.ToArray());
				if(mainKey == null && !string.IsNullOrEmpty(key)) {
					mainKey = key;
					continue;
				}
				return null;
			}
			if(mainKey == null)
				return null;
			selectedKeys.Add(mainKey);
			return string.Join("+", selectedKeys.ToArray());
		}
		static string FindKey(string piece, string[] existsKeysExpr) {
			bool isExists = Array.Exists(existsKeysExpr, k => Regex.IsMatch(piece, string.Format("^{0}$", k), RegexOptions.IgnoreCase));
			if(isExists)
				return piece.Length > 1 ? char.ToUpper(piece[0]) + piece.Substring(1).ToLower() : piece.ToUpper();
			return null;
		}
		protected internal string CommandName {
			get {
				if(ActionType == Web.ASPxHtmlEditor.ActionType.ShowCustomDialog)
					return string.Format("customdialog;{0}", ActionName);
				return ActionName;
			}
		}
		protected internal virtual bool IsShortcutAllowed(HtmlEditorView view) {
			return ActionType == ActionType.ExecuteCommand && view == ActionView || ActionType == ActionType.ShowCustomDialog && ActionView == HtmlEditorView.Design && view == HtmlEditorView.Design;
		}
		public override string ToString() {
			return string.Format("{0} - {1} ({2})", string.IsNullOrEmpty(Shortcut) ? "None" : Shortcut, ActionName, ActionView.ToString());
		}
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			HtmlEditorShortcut src = source as HtmlEditorShortcut;
			if(src != null) {
				Shortcut = src.Shortcut;
				ActionName = src.ActionName;
				ActionType = src.ActionType;
				ActionView = src.ActionView;
			}
		}
	}
	public enum ActionType {
		ExecuteCommand,
		ShowCustomDialog
	}
}
