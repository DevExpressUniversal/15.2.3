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
using System.Drawing;
using System.IO;
using System.Reflection;
using DevExpress.Services;
using DevExpress.Services.Internal;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.Utils.Localization;
using DevExpress.Compatibility.System;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Utils.Commands {
	#region CommandSourceType
	public enum CommandSourceType {
		Unknown = 0,
		Menu,
		Keyboard,
		Mouse
	}
	#endregion
	#region CommandUIState
	public interface ICommandUIState {
		bool Enabled { get; set; }
		bool Visible { get; set; }
		bool Checked { get; set; }
		object EditValue { get; set; }
	}
	#endregion
	#region IValueBasedCommandUIState
	public interface IValueBasedCommandUIState<T> : ICommandUIState {
		T Value { get; set; }
	}
	#endregion
	#region DefaultCommandUIState (stub class)
	public class DefaultCommandUIState : ICommandUIState {
		bool isEnabled = true;
		bool isChecked = false;
		bool isVisible = true;
		public virtual bool Enabled { get { return isEnabled; } set { isEnabled = value; } }
		public virtual bool Visible { get { return isVisible; } set { isVisible = value; } }
		public virtual bool Checked { get { return isChecked; } set { isChecked = value; } }
		public virtual object EditValue { get { return null; } set { } }
	}
	#endregion
	#region DefaultValueBasedCommandUIState
	public class DefaultValueBasedCommandUIState<T> : DefaultCommandUIState, IValueBasedCommandUIState<T> {
		T editValue;
		public virtual T Value { get { return editValue; } set { editValue = value; } }
		public override object EditValue {
			get { return editValue; }
			set {
				try {
					editValue = (T)value;
				}
				catch {
#if DXPORTABLE
					if (typeof(T).GetTypeInfo().IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
						editValue = (T)Convert.ChangeType(value, typeof(T).GetTypeInfo().GenericTypeArguments[0], System.Globalization.CultureInfo.InvariantCulture);
#else
					if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(Nullable<>))
						editValue = (T)Convert.ChangeType(value, typeof(T).GetGenericArguments()[0], System.Globalization.CultureInfo.InvariantCulture);
#endif
					else
						editValue = (T)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
				}
			}
		}
	}
#endregion
#region Command (abstract class)
	public abstract class Command {
		CommandSourceType commandSourceType;
		bool hideDisabled;
		protected Command() {
		}
#region Properties
		public abstract string MenuCaption { get; }
		public abstract string Description { get; }
		public virtual Image Image { get { return null; } }
		public virtual Image LargeImage { get { return null; } }
		public virtual CommandSourceType CommandSourceType { get { return commandSourceType; } set { commandSourceType = value; } }
		public bool HideDisabled { get { return hideDisabled; } set { hideDisabled = value; } }
		public virtual bool ShowsModalDialog { get { return false; } }
		protected virtual bool ShouldBeExecutedOnKeyUpInSilverlightEnvironment { get { return false; } }
		protected internal bool InnerShouldBeExecutedOnKeyUpInSilverlightEnvironment { get { return ShouldBeExecutedOnKeyUpInSilverlightEnvironment; } }
		protected internal virtual IServiceProvider ServiceProvider { get { return null; } }
#endregion
		public virtual void Execute() {
			ICommandUIState state = CreateDefaultCommandUIState();
			UpdateUIState(state);
			if (state.Visible && state.Enabled)
				ForceExecute(state);
		}
		public virtual bool CanExecute() {
			ICommandUIState state = CreateDefaultCommandUIState();
			UpdateUIState(state);
			return (state.Visible && state.Enabled);
		}
		public virtual ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultCommandUIState();
		}
		public virtual void UpdateUIState(ICommandUIState state) {
			UpdateUIStateCore(state);
			UpdateUIStateViaService(state);
			if (HideDisabled) {
				if (!state.Enabled)
					state.Visible = false;
			}
		}
		protected internal virtual void UpdateUIStateViaService(ICommandUIState state) {
			IServiceProvider serviceProvider = ServiceProvider;
			if (serviceProvider == null)
				return;
			ICommandUIStateManagerService service = (ICommandUIStateManagerService)serviceProvider.GetService(typeof(ICommandUIStateManagerService));
			if (service != null)
				service.UpdateCommandUIState(this, state);
		}
		public abstract void ForceExecute(ICommandUIState state);
		protected abstract void UpdateUIStateCore(ICommandUIState state);
	}
#endregion
#region CommandCollection
	public class CommandCollection : List<Command> {
	}
#endregion
#region ICommandAwareControl
	public interface ICommandAwareControl<TCommandId> where TCommandId : struct {
		CommandBasedKeyboardHandler<TCommandId> KeyboardHandler { get; }
		Command CreateCommand(TCommandId id);
		bool HandleException(Exception e);
		void Focus();
		void CommitImeContent();
		event EventHandler BeforeDispose;
		event EventHandler UpdateUI;
	}
#endregion
#region ControlCommand<TControl, TCommandId, TLocalizedStringId> (absrtact class)
	public abstract class ControlCommand<TControl, TCommandId, TLocalizedStringId> : Command
		where TControl : class, ICommandAwareControl<TCommandId>, IServiceProvider
		where TCommandId : struct
		where TLocalizedStringId : struct {
		readonly TControl control;
		protected ControlCommand(TControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
		}
#region Properties
		public TControl Control { get { return control; } }
		protected internal override IServiceProvider ServiceProvider { get { return Control; } }
		public abstract TCommandId Id { get; }
		protected abstract XtraLocalizer<TLocalizedStringId> Localizer { get; }
		public abstract TLocalizedStringId MenuCaptionStringId { get; }
		public abstract TLocalizedStringId DescriptionStringId { get; }
		public override string MenuCaption { get { return Localizer.GetLocalizedString(MenuCaptionStringId); } }
		public override string Description { get { return Localizer.GetLocalizedString(DescriptionStringId); } }
		public virtual string ImageName { get { return String.Empty; } }
		protected abstract string ImageResourcePrefix { get; }
#if DXRESTRICTED
		protected virtual Assembly ImageResourceAssembly { get { return GetType().GetTypeInfo().Assembly; } }
#else
		protected virtual Assembly ImageResourceAssembly { get { return GetType().Assembly; } }
#endif
		public override Image Image { get { return !string.IsNullOrEmpty(ImageName) ? LoadImage() : null; } }
		public override Image LargeImage { get { return !string.IsNullOrEmpty(ImageName) ? LoadLargeImage() : null; } }
#endregion
		protected internal virtual Image LoadImage() {
			return CommandResourceImageLoader.LoadSmallImage(ImageResourcePrefix, ImageName, ImageResourceAssembly);
		}
		protected internal virtual Image LoadLargeImage() {
			return CommandResourceImageLoader.LoadLargeImage(ImageResourcePrefix, ImageName, ImageResourceAssembly);
		}
		protected internal virtual ICommandExecutionListenerService GetCommandExecutionListener() {
			return Control.GetService(typeof(ICommandExecutionListenerService)) as ICommandExecutionListenerService;
		}
		protected virtual void NotifyBeginCommandExecution(ICommandUIState state) {
			ICommandExecutionListenerService listener = GetCommandExecutionListener();
			if (listener != null)
				listener.BeginCommandExecution(this, state);
		}
		protected virtual void NotifyEndCommandExecution(ICommandUIState state) {
			ICommandExecutionListenerService listener = GetCommandExecutionListener();
			if (listener != null)
				listener.EndCommandExecution(this, state);
		}
	}
#endregion
}
namespace DevExpress.Services {
#region ICommandExecutionListenerService
	public interface ICommandExecutionListenerService {
		void BeginCommandExecution(Command command, ICommandUIState state);
		void EndCommandExecution(Command command, ICommandUIState state);
	}
#endregion
}
namespace DevExpress.Services.Internal {
	public interface ICommandUIStateManagerService {
		void UpdateCommandUIState(Command command, ICommandUIState state);
	}
}
namespace DevExpress.Utils {
#region CommandResourceImageLoader
	public class CommandResourceImageLoader {
		public static Stream LoadSmallImageStream(string resourcePath, string imageName, Assembly asm) {
			return asm.GetManifestResourceStream(GetSmallImageName(resourcePath, imageName));
		}
		public static Image LoadSmallImage(string resourcePath, string imageName, Assembly asm) {
			return CreateBitmapFromResources(GetSmallImageName(resourcePath, imageName), asm);
		}
		public static Image LoadLargeImage(string resourcePath, string imageName, Assembly asm) {
			return CreateBitmapFromResources(GetLargeImageName(resourcePath, imageName), asm);
		}
#if DXPORTABLE
		public static Bitmap CreateBitmapFromResources(string name, Assembly asm) {
			Stream stream = asm.GetManifestResourceStream(name);
			return (Bitmap)Bitmap.FromStream(stream);
		}
#elif (!SILVERLIGHT)
		public static Bitmap CreateBitmapFromResources(string name, Assembly asm) {
			Stream stream = asm.GetManifestResourceStream(name);
			return (Bitmap)DevExpress.Data.Utils.ImageTool.ImageFromStream(stream);
		}
#else
		public static Image CreateBitmapFromResources(string name, Assembly asm) {
			Stream stream = asm.GetManifestResourceStream(name);
			if(stream == null) return null;
			Image image = new Image();
			BitmapImage b = new BitmapImage();
			b.SetSource(stream);
			image.Source = b;
			return image;
		}
#endif
		internal static string GetSmallImageName(string resourcePath, string imageName) {
			return GetImageName(resourcePath, imageName, "16x16");
		}
		internal static string GetLargeImageName(string resourcePath, string imageName) {
			return GetImageName(resourcePath, imageName, "32x32");
		}
		public static string GetImageName(string resourcePath, string imageName, string size) {
			return String.Format(resourcePath + ".{0}_{1}.png", imageName, size);
		}
	}
#endregion
}
namespace DevExpress.Utils.Menu {
	#region IDXMenuItemBase<T>
	public interface IDXMenuItemBase<T> where T : struct {
		bool BeginGroup { get; set; }
	}
	#endregion
	#region IDXMenuItemCollection<T>
	public class IDXMenuItemCollection<T> : List<IDXMenuItemBase<T>> where T : struct {
	}
	#endregion
	#region IDXMenuItem<T>
	public interface IDXMenuItem<T> : IDXMenuItemBase<T> where T : struct {
	}
	#endregion
	#region IDXMenuCheckItem<T>
	public interface IDXMenuCheckItem<T> : IDXMenuItemBase<T> where T : struct {
	}
	#endregion
	#region IDXPopupMenu<T>
	public interface IDXPopupMenu<T> : IDXMenuItemBase<T> where T : struct {
		int ItemsCount { get; }
		void AddItem(IDXMenuItemBase<T> item);
		T Id { get; set; }
		string Caption { get; set; }
		bool Visible { get; set; }
	}
	#endregion
	#region IDXMenuItemCommandAdapter<T>
	public interface IDXMenuItemCommandAdapter<T> where T : struct {
		IDXMenuItem<T> CreateMenuItem(DXMenuItemPriority priority);
	}
	#endregion
	#region IDXMenuCheckItemCommandAdapter<T>
	public interface IDXMenuCheckItemCommandAdapter<T> where T : struct {
		IDXMenuCheckItem<T> CreateMenuItem(string groupId);
	}
	#endregion
	#region IMenuBuilderUIFactory<TCommand, TMenuId>
	public interface IMenuBuilderUIFactory<TCommand, TMenuId>
		where TCommand : Command
		where TMenuId : struct {
		IDXMenuItemCommandAdapter<TMenuId> CreateMenuItemAdapter(TCommand command);
		IDXMenuCheckItemCommandAdapter<TMenuId> CreateMenuCheckItemAdapter(TCommand command);
		IDXPopupMenu<TMenuId> CreatePopupMenu();
		IDXPopupMenu<TMenuId> CreateSubMenu();
	}
	#endregion
	#region CommandBasedPopupMenuBuilder<TPopupMenuId> (abstract class)
	public abstract class CommandBasedPopupMenuBuilder<TCommand, TMenuId>
		where TCommand : Command
		where TMenuId : struct {
		readonly IMenuBuilderUIFactory<TCommand, TMenuId> uiFactory;
		protected CommandBasedPopupMenuBuilder(IMenuBuilderUIFactory<TCommand, TMenuId> uiFactory) {
			Guard.ArgumentNotNull(uiFactory, "uiFactory");
			this.uiFactory = uiFactory;
		}
		public IMenuBuilderUIFactory<TCommand, TMenuId> UiFactory { get { return uiFactory; } }
		protected internal virtual void AddMenuItemIfCommandVisible(IDXPopupMenu<TMenuId> menu, TCommand command) {
			AddMenuItemIfCommandVisible(menu, command, false);
		}
		protected internal virtual void AddMenuItemIfCommandVisible(IDXPopupMenu<TMenuId> menu, TCommand command, bool beginGroup) {
			DefaultCommandUIState state = new DefaultCommandUIState();
			command.UpdateUIState(state);
			if (state.Visible)
				AddMenuItem(menu, command).BeginGroup = beginGroup;
		}
		protected internal virtual IDXMenuItem<TMenuId> AddMenuItem(IDXPopupMenu<TMenuId> menu, TCommand command) {
			return AddMenuItem(menu, command, DXMenuItemPriority.Normal);
		}
		protected internal virtual IDXMenuItem<TMenuId> AddMenuItem(IDXPopupMenu<TMenuId> menu, TCommand command, DXMenuItemPriority priority) {
			IDXMenuItemCommandAdapter<TMenuId> adapter = uiFactory.CreateMenuItemAdapter(command);
			IDXMenuItem<TMenuId> item = adapter.CreateMenuItem(priority);
			menu.AddItem(item);
			command.CommandSourceType = CommandSourceType.Menu;
			return item;
		}
		protected internal virtual void AddMenuCheckItemIfCommandVisible(IDXPopupMenu<TMenuId> menu, TCommand command, string groupId) {
			DefaultCommandUIState state = new DefaultCommandUIState();
			command.UpdateUIState(state);
			if (state.Visible)
				AddMenuCheckItem(menu, command, groupId);
		}
		protected internal virtual IDXMenuCheckItem<TMenuId> AddMenuCheckItem(IDXPopupMenu<TMenuId> menu, TCommand command) {
			return AddMenuCheckItem(menu, command, String.Empty);
		}
		protected internal virtual IDXMenuCheckItem<TMenuId> AddMenuCheckItem(IDXPopupMenu<TMenuId> menu, TCommand command, string groupId) {
			IDXMenuCheckItemCommandAdapter<TMenuId> adapter = uiFactory.CreateMenuCheckItemAdapter(command);
			IDXMenuCheckItem<TMenuId> item = adapter.CreateMenuItem(groupId);
			menu.AddItem(item);
			command.CommandSourceType = CommandSourceType.Menu;
			return item;
		}
		protected internal virtual void AppendSubmenu(IDXPopupMenu<TMenuId> menu, IDXPopupMenu<TMenuId> subMenu, bool beginGroup) {
			if (subMenu != null && subMenu.ItemsCount > 0) {
				subMenu.BeginGroup = beginGroup;
				menu.AddItem(subMenu);
			}
		}
		public virtual IDXPopupMenu<TMenuId> CreatePopupMenu() {
			IDXPopupMenu<TMenuId> menu = uiFactory.CreatePopupMenu();
			PopulatePopupMenu(menu);
			return menu;
		}
		public virtual IDXPopupMenu<TMenuId> CreateSubMenu() {
			IDXPopupMenu<TMenuId> menu = uiFactory.CreateSubMenu();
			PopulatePopupMenu(menu);
			return menu;
		}
		public abstract void PopulatePopupMenu(IDXPopupMenu<TMenuId> menu);
	}
	#endregion
}
