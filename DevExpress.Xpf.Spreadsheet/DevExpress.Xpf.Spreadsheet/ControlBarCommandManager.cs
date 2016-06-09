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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Bars.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Ribbon;
using ImageHelper = DevExpress.Xpf.Core.Native.ImageHelper;
using DevExpress.Xpf.Office.UI;
using System.Linq;
using System.Windows.Forms;
namespace DevExpress.Office.Internal {
	#region ControlBarCommandManager (abstract class)
	public abstract class ControlBarCommandManager<TControl, TCommand, TCommandId>
		where TControl : ICommandAwareControl<TCommandId>
		where TCommandId : struct {
		readonly TControl control;
		protected ControlBarCommandManager(TControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
		#region Properties
		protected TControl Control { get { return control; } }
		protected abstract object ControlAccessor { get; }
		protected abstract BarManager BarManager { get; }
		protected abstract RibbonControl Ribbon { get; }
		protected abstract TCommandId EmptyCommandId { get; }
		#endregion
		protected abstract TCommandId GetCommandId(ICommand command);
		protected abstract void SetFocus();
		protected abstract bool IsControlProvider(object value);
		protected abstract BarItemCommandUIState CreateBarItemUIState(BarItem item);
		#region Bars / Ribbon initialization
		protected internal virtual void UpdateBarItemsDefaultValues() {
			if (BarManager == null)
				return;
			foreach (var item in BarNameScope.GetService<IElementRegistratorService>(BarManager).GetElements<IFrameworkInputElement>().OfType<BarItem>()) {
				UpdateBarItemDefaultValues(item);
				UpdateInnerGalleryItemsDefaultValues(item as RibbonGalleryBarItem);
				UpdateDropDownGalleryItemsDefaultValues(item as BarSplitButtonItem);
			}
		}
		protected internal virtual void UpdateBarItemDefaultValues(BarItem item) {
			Command command = CreateCommand(item.Command);
			if (command == null)
				return;
			BarItemDefaultProperties.SetContent(item, ReplaceShortcuts(command.MenuCaption));
			BarItemDefaultProperties.SetDescription(item, command.Description);
			BarItemDefaultProperties.SetHelpText(item, command.Description);
			try {
				SetItemGlyphs(item, command);
			} catch {
			}
			BarItemDefaultProperties.SetHint(item, GenerateHint(command));
			if (item.SuperTip == null) {
				TCommandId commandId = GetCommandId(item.Command);
				try { 
					item.SuperTip = CreateCommandSuperTip(commandId);
				} catch { }
			}
			if (!IsControlProvider(item.CommandParameter))
				item.CommandParameter = this.ControlAccessor;
			UnsubscribeBarItemEvents(item);
			SubscribeBarItemEvents(item);
		}
		protected internal virtual Command CreateCommand(ICommand barItemCommand) {
			TCommandId commandId = GetCommandId(barItemCommand);
			if (Object.Equals(commandId, EmptyCommandId))
				return null;
			return Control.CreateCommand(commandId);
		}
		protected internal virtual void UpdateInnerGalleryItemsDefaultValues(RibbonGalleryBarItem item) {
			if (item == null)
				return;
			UnsubscribeBarItemEvents(item);
			try {
				UpdateGalleryItemsDefaultValues(item.Gallery);
			} finally {
				SubscribeBarItemEvents(item);
			}
		}
		protected virtual void SetItemGlyphs(BarItem item, Command command) {
			if (command.Image != null)
				BarItemDefaultProperties.SetGlyph(item, ImageHelper.CreatePlatformImage(command.Image).Source);
			if (command.LargeImage != null)
				BarItemDefaultProperties.SetLargeGlyph(item, ImageHelper.CreatePlatformImage(command.LargeImage).Source);
		}
		void UpdateDropDownGalleryItemsDefaultValues(BarSplitButtonItem item) {
			if (item == null)
				return;
			UnsubscribeBarItemEvents(item);
			try {
				GalleryDropDownPopupMenu popupMenu = item.PopupControl as GalleryDropDownPopupMenu;
				if (popupMenu == null)
					return;
				UpdateGalleryItemsDefaultValues(popupMenu.Gallery);
			}
			finally {
				SubscribeBarItemEvents(item);
			}
		}
		protected internal virtual void UpdateGalleryItemsDefaultValues(Gallery gallery) {
			if (gallery == null)
				return;
			GalleryItemGroupCollection groups = gallery.Groups;
			int count = groups.Count;
			for (int groupIndex = 0; groupIndex < count; groupIndex++) {
				GalleryItemGroup group = groups[groupIndex];
				int groupItemsCount = group.Items.Count;
				for (int itemIndex = groupItemsCount - 1; itemIndex >= 0; itemIndex--) {
					GalleryItem galleryItem = group.Items[itemIndex];
					UpdateGalleryItemDefaultValue(galleryItem);
				}
			}
		}
		protected internal virtual void UpdateGalleryItemDefaultValue(GalleryItem item) {
			TCommandId commandId = GetCommandId(item.Command);
			if (Object.Equals(commandId, EmptyCommandId))
				return;
			Command command = Control.CreateCommand(commandId);
			if (command == null)
				return;
			if (item.Caption == null)
				item.Caption = ReplaceShortcuts(command.MenuCaption);
			if (item.Description == null)
				item.Description = command.Description;
			if (command.LargeImage != null && item.Glyph == null)
				item.Glyph = ImageHelper.CreatePlatformImage(command.LargeImage).Source;
			item.Hint = GenerateHint(command);
			item.SuperTip = CreateCommandSuperTip(commandId);
			if (!IsControlProvider(item.CommandParameter))
				item.CommandParameter = this.ControlAccessor;
		}
		protected internal virtual void UpdateRibbonItemsDefaultValues() {
			if (Ribbon == null)
				return;
			IList<RibbonPageGroup> groups = ObtainRibbonPageGroups(Ribbon);
			int count = groups.Count;
			for (int i = 0; i < count; i++)
				UpdateRibbonPageGroupDefaultValues(groups[i]);
		}
		protected internal virtual void UpdateRibbonPageGroupDefaultValues(RibbonPageGroup group) {
			TCommandId commandId = GetCommandId(group.CaptionButtonCommand);
			if (Object.Equals(commandId, EmptyCommandId))
				return;
			if (!IsControlProvider(group.CaptionButtonCommandParameter))
				group.CaptionButtonCommandParameter = this.ControlAccessor;
			if (group.SuperTip != null)
				return;
			SuperTip superTip = CreateCommandSuperTip(commandId);
			if (superTip != null)
				group.SuperTip = superTip;
		}
		protected internal virtual SuperTip CreateCommandSuperTip(TCommandId commandId) {
			Command command = Control.CreateCommand(commandId);
			if (command == null)
				return null;
			SuperTipHeaderItem header = new SuperTipHeaderItem();
			header.Content = GetCaptionWithShortcuts(commandId, command.MenuCaption);
			SuperTipItem content = new SuperTipItem();
			content.Content = RemoveShortcuts(command.Description);
			SuperTip superTip = new SuperTip();
			superTip.Items.Add(header);
			superTip.Items.Add(content);
			return superTip;
		}
		public string GetCaptionWithShortcuts(TCommandId commandId) {
			Command command = Control.CreateCommand(commandId);
			if (command == null)
				return String.Empty;
			return GetCaptionWithShortcuts(commandId, command.MenuCaption);
		}
		public string GetCaptionWithShortcuts(TCommandId commandId, string menuCaption) {
			Keys keys = Keys.None;
			if (control.KeyboardHandler != null)
				keys = control.KeyboardHandler.GetKeys(commandId);
			string shortCutText = ConvertKeysToString(keys);
			string totalString = String.IsNullOrEmpty(shortCutText) ? menuCaption : String.Format("{0} ({1})", menuCaption, shortCutText);
			return RemoveShortcuts(totalString);
		}
		public string ConvertKeysToString(Keys keys) {
			return KeysConverter.ToString(keys);
		}
		protected internal virtual IList<RibbonPageGroup> ObtainRibbonPageGroups(RibbonControl ribbon) {
			IList<RibbonPageGroup> result = new List<RibbonPageGroup>();
			IList items = ribbon.Categories;
			if (items == null)
				return result;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				RibbonPageCategoryBase category = items[i] as RibbonPageCategoryBase;
				if (category != null)
					UpdateRibbonCategoryDefaultValues(category, result);
			}
			return result;
		}
		protected internal virtual void UpdateRibbonCategoryDefaultValues(RibbonPageCategoryBase category, IList<RibbonPageGroup> result) {
			RibbonPageCollection pages = category.Pages;
			if (pages == null)
				return;
			int count = pages.Count;
			for (int i = 0; i < count; i++)
				UpdateRibbonPageDefaultValues(pages[i], result);
		}
		protected internal virtual void UpdateRibbonPageDefaultValues(RibbonPage page, IList<RibbonPageGroup> result) {
			RibbonPageGroupCollection groups = page.Groups;
			if (groups == null)
				return;
			int count = groups.Count;
			for (int i = 0; i < count; i++) {
				RibbonPageGroup group = groups[i];
				if (!Object.Equals(EmptyCommandId, GetCommandId(group.CaptionButtonCommand)))
					result.Add(group);
			}
		}
		string GenerateHint(Command command) {
			return RemoveShortcuts(command.Description);
		}
		public string ReplaceShortcuts(string value) {
			return RemoveShortcuts(value);
		}
		string RemoveShortcuts(string value) {
			bool prevWasAmpersand = false;
			StringBuilder sb = new StringBuilder(value.Length);
			int count = value.Length;
			for (int i = 0; i < count; i++) {
				char ch = value[i];
				if (ch == '&') {
					if (prevWasAmpersand)
						sb.Append(ch);
				}
				else
					sb.Append(ch);
				prevWasAmpersand = false;
			}
			return sb.ToString();
		}
		#endregion
		#region Bars / Ribbon update state
		protected internal virtual void UpdateBarItemsState() {
			if (BarManager == null)
				return;
			foreach (var item in BarNameScope.GetService<IElementRegistratorService>(BarManager).GetElements<IFrameworkInputElement>().OfType<BarItem>()) {
				UpdateBarItemState(item);
				UpdateInnerGalleryItemsState(item as RibbonGalleryBarItem);
				UpdateDropDownGalleryItemsState(item as BarSplitButtonItem);
			}
		}
		protected internal virtual void UpdateInnerGalleryItemsState(RibbonGalleryBarItem item) {
			if (item == null)
				return;
			UnsubscribeBarItemEvents(item);
			try {
				UpdateGalleryItemsState(item.Gallery);
			}
			finally {
				SubscribeBarItemEvents(item);
			}
		}
		void UpdateDropDownGalleryItemsState(BarSplitButtonItem item) {
			if (item == null)
				return;
			UnsubscribeBarItemEvents(item);
			try {
				GalleryDropDownPopupMenu popupMenu = item.PopupControl as GalleryDropDownPopupMenu;
				if (popupMenu == null)
					return;
				UpdateGalleryItemsState(popupMenu.Gallery);
			}
			finally {
				SubscribeBarItemEvents(item);
			}
		}
		protected internal virtual void UpdateGalleryItemsState(Gallery gallery) {
			if (gallery == null)
				return;
			GalleryItemGroupCollection groups = gallery.Groups;
			int count = groups.Count;
			for (int groupIndex = 0; groupIndex < count; groupIndex++) {
				GalleryItemGroup group = groups[groupIndex];
				int groupItemsCount = group.Items.Count;
				for (int itemIndex = groupItemsCount - 1; itemIndex >= 0; itemIndex--) {
					GalleryItem galleryItem = group.Items[itemIndex];
					UpdateGalleryItemState(galleryItem);
				}
			}
		}
		protected internal virtual void UpdateBarItemState(BarItem item) {
			TCommandId commandId = GetCommandId(item.Command);
			if (Object.Equals(commandId, EmptyCommandId))
				return;
			UpdateBarItemCommandUIState(item, commandId);
		}
		protected internal virtual void UpdateRibbonItemsState() {
			if (Ribbon == null)
				return;
			UpdateRibbonItemsState(ObtainRibbonPageGroups(Ribbon));
			foreach (RibbonPageCategoryBase category in Ribbon.Categories)
				UpdateRibbonPageCategoryState(category);
		}
		protected internal virtual void UpdateRibbonPageCategoryState(RibbonPageCategoryBase category) {
			ICommand command = AttachedCommand.GetCommand(category);
			if (command == null)
				return;
			TCommandId commandId = GetCommandId(command);
			if (!Object.Equals(commandId, EmptyCommandId))
				UpdateRibbonPageCategoryState(category, commandId);
		}
		protected internal virtual void UpdateRibbonPageCategoryState(RibbonPageCategoryBase category, TCommandId commandId) {
			Command command = Control.CreateCommand(commandId);
			if (command == null)
				return;
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			category.IsVisible = state.Visible && state.Enabled;
		}
		protected internal virtual void UpdateRibbonItemsState(IList<RibbonPageGroup> groups) {
			int count = groups.Count;
			for (int i = count - 1; i >= 0; i--) {
				RibbonPageGroup group = groups[i];
				TCommandId commandId = GetCommandId(group.CaptionButtonCommand);
				if (!Object.Equals(commandId, EmptyCommandId))
					UpdateRibbonPageGroupCommandUIState(group, commandId);
			}
		}
		protected internal virtual void UpdateGalleryItemState(GalleryItem item) {
			TCommandId commandId = GetCommandId(item.Command);
			if (Object.Equals(commandId, EmptyCommandId))
				return;
			UpdateGalleryItemCommandUIState(item, commandId);
		}
		void UpdateGalleryItemCommandUIState(GalleryItem item, TCommandId commandId) {
			Command command = Control.CreateCommand(commandId);
			if (command == null)
				return;
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			item.IsVisible = state.Visible;
			item.IsEnabled = state.Enabled;
			item.IsChecked = state.Checked;
		}
		#endregion
		void SubscribeBarItemEvents(BarItem item) {
			BarEditItem editItem = item as BarEditItem;
			if (editItem != null)
				editItem.EditValueChanged += OnBarItemEditValueChanged;
			IEditValueBarItem editValueBarItem = item as IEditValueBarItem;
			if (editValueBarItem != null)
				editValueBarItem.EditValueChanged += OnIEditValueBarItemEditValueChanged;
		}
		void UnsubscribeBarItemEvents(BarItem item) {
			BarEditItem editItem = item as BarEditItem;
			if (editItem != null)
				editItem.EditValueChanged -= OnBarItemEditValueChanged;
			IEditValueBarItem editValueBarItem = item as IEditValueBarItem;
			if (editValueBarItem != null)
				editValueBarItem.EditValueChanged -= OnIEditValueBarItemEditValueChanged;
		}
		protected internal virtual void UnsubscribeBarItemsEvents(BarManager manager) {
			if (manager == null)
				return;
			int count = manager.Items.Count;
			for (int i = count - 1; i >= 0; i--)
				UnsubscribeBarItemEvents(manager.Items[i]);
		}
		void OnBarItemEditValueChanged(object sender, RoutedEventArgs e) {
			BarEditItem item = sender as BarEditItem;
			if (item == null)
				return;
			Command command = CreateCommand(item.Command);
			if (command == null)
				return;
			ExecuteParametrizedCommand(command, item.EditValue);
		}
		void OnIEditValueBarItemEditValueChanged(object sender, RoutedEventArgs e) {
			IEditValueBarItem item = sender as IEditValueBarItem;
			if (item == null)
				return;
			Command command = CreateCommand(item.Command);
			if (command == null)
				return;
			ExecuteParametrizedCommand(command, item.EditValue);
			PopupMenuManager.CloseAllPopups();
		}
		protected internal virtual void ExecuteParametrizedCommand(TCommandId commandId, object parameter) {
			Command command = Control.CreateCommand(commandId);
			ExecuteParametrizedCommand(command, parameter);
		}
		protected internal virtual void ExecuteParametrizedCommand(ICommand barItemCommand, object parameter) {
			Command command = CreateCommand(barItemCommand);
			ExecuteParametrizedCommand(command, parameter);
		}
		protected internal virtual void ExecuteParametrizedCommand(Command command, object parameter) {
			if (command == null)
				return;
			ICommandUIState state = CreateCommandUIState(command, parameter);
			if (state != null) {
				if (command.CanExecute())
					command.ForceExecute(state);
			}
			if (ShouldSetFocus(command))
				SetFocus();
		}
		protected virtual bool ShouldSetFocus(Command command) {
			return !command.ShowsModalDialog;
		}
		protected internal ICommandUIState CreateCommandUIState(Command command, object parameter) {
			try {
				ICommandUIState state = command.CreateDefaultCommandUIState();
				command.UpdateUIState(state);
				if (parameter != null) {
					try {
						state.EditValue = parameter;
					}
					catch {
#if !SL
						if (parameter != null && parameter.GetType() == typeof(System.Windows.Media.Color) && state.EditValue != null && state.EditValue.GetType() == typeof(System.Drawing.Color)) {
							System.Windows.Media.Color c = (System.Windows.Media.Color)parameter;
							if (c.A == 0 && c.R == 0 && c.G == 0 && c.B == 0)
								state.EditValue = DXColor.Empty;
							else
								state.EditValue = DXColor.FromArgb(c.A, c.R, c.G, c.B);
						}
						else
#endif
							throw;
					}
				}
				return state;
			}
			catch {
				return null;
			}
		}
		public virtual void UpdateBarItemCommandUIState(BarItem item, TCommandId commandId) {
			Command command = Control.CreateCommand(commandId);
			if (command == null) {
				item.IsEnabled = false;
				return;
			}
			UpdateBarItemCommandUIState(item, command);
		}
		public virtual void UpdateBarItemCommandUIState(BarItem item, Command command) {
			System.Diagnostics.Debug.Assert(command != null);
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			UnsubscribeBarItemEvents(item);
			try {
				BarItemCommandUIState barItemUiState = CreateBarItemUIState(item);
				barItemUiState.Visible = state.Visible;
				barItemUiState.Enabled = state.Enabled;
				barItemUiState.Checked = state.Checked;
				barItemUiState.EditValue = state.EditValue;
				if (item is BarStaticItem)
					item.DataContext = state;
			}
			finally {
				SubscribeBarItemEvents(item);
			}
		}
		public virtual void UpdateRibbonPageGroupCommandUIState(RibbonPageGroup group, TCommandId commandId) {
			Command command = Control.CreateCommand(commandId);
			if (command == null) {
				group.ShowCaptionButton = false;
				return;
			}
			ICommandUIState state = command.CreateDefaultCommandUIState();
			command.UpdateUIState(state);
			group.ShowCaptionButton = state.Visible;
			group.IsCaptionButtonEnabled = state.Enabled;
		}
	}
	#endregion
	#region KeysConverter
	public static class KeysConverter {
		#region Fields
		static string ControlText = "Ctrl";
		static string ShiftText = "Shift";
		static string AltText = "Alt";
		static Hashtable keyDisplayText = GetKeyDisplayText();
		static Hashtable GetKeyDisplayText() {
			Hashtable result = keyDisplayText = new Hashtable();
			keyDisplayText[Keys.OemBackslash] = "/";
			keyDisplayText[Keys.OemCloseBrackets] = "]";
			keyDisplayText[Keys.Oemcomma] = ",";
			keyDisplayText[Keys.OemMinus] = "-";
			keyDisplayText[Keys.OemOpenBrackets] = "[";
			keyDisplayText[Keys.OemPeriod] = ".";
			keyDisplayText[Keys.OemPipe] = "\\";
			keyDisplayText[Keys.Oemplus] = "=";
			keyDisplayText[Keys.OemQuestion] = "?";
			keyDisplayText[Keys.OemQuotes] = "'";
			keyDisplayText[Keys.OemSemicolon] = ";";
			keyDisplayText[Keys.Oemtilde] = "`";
			keyDisplayText[Keys.D0] = "0";
			keyDisplayText[Keys.D1] = "1";
			keyDisplayText[Keys.D2] = "2";
			keyDisplayText[Keys.D3] = "3";
			keyDisplayText[Keys.D4] = "4";
			keyDisplayText[Keys.D5] = "5";
			keyDisplayText[Keys.D6] = "6";
			keyDisplayText[Keys.D7] = "7";
			keyDisplayText[Keys.D8] = "8";
			keyDisplayText[Keys.D9] = "9";
			keyDisplayText[Keys.PageDown] = "PageDown";
			keyDisplayText[Keys.PageUp] = "PageUp";
			return result;
		}
		#endregion
		public static string ToString(Keys keys) {
			Keys key = keys & (~Keys.Modifiers);
			if (key == Keys.None)
				return String.Empty;
			String modifierString = String.Empty;
			Keys modifierKeys = keys & Keys.Modifiers;
			if ((modifierKeys & Keys.Control) != Keys.None)
				modifierString = GetModifiedText(modifierString, ControlText);
			if ((modifierKeys & Keys.Alt) != Keys.None)
				modifierString = GetModifiedText(modifierString, AltText);
			if ((modifierKeys & Keys.Shift) != Keys.None)
				modifierString = GetModifiedText(modifierString, ShiftText);
			string keyString = ConvertKeyToString(key);
			if (String.IsNullOrEmpty(modifierString))
				return keyString;
			return modifierString + keyString;
		}
		static string GetModifiedText(string currentText, string text) {
			return currentText + text + "+";
		}
		static string ConvertKeyToString(Keys key) {
			if (keyDisplayText.ContainsKey(key))
				return (string)keyDisplayText[key];
			return key.ToString();
		}
	}
	#endregion
}
