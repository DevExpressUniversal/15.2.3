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

using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Taskbar.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Windows.Forms;
namespace DevExpress.Utils.Taskbar {
	[
	Designer("DevExpress.Utils.Design.Taskbar.TaskbarAssistantDesigner, " + AssemblyInfo.SRAssemblyDesignFull),
	DesignerCategory("Component"), DXToolboxItem(DXToolboxItemKind.Regular),
	Description("Provides methods to manipulate an application taskbar button, Jump List and thumbnail preview."),
	ToolboxTabName(AssemblyInfo.DXTabComponents),
	ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "TaskbarAssistant")
	]
	public class TaskbarAssistant : Component {
		[SecurityCritical]
		public TaskbarAssistant() {
			TaskbarAssistantCore.Initialize();
			this.lockUpdate = false;
			this.windowInternal = CreateWindowInternal();
			this.progressCurrentValue = DefaultTaskbarProperties.CurrentProgressValue;
			this.progressMaximumValue = DefaultTaskbarProperties.MaximumProgressValue;
			this.progressMode = DefaultTaskbarProperties.ProgressState;
			this.overlayIcon = DefaultTaskbarProperties.OverlayIcon;
			this.iconsAssembly = DefaultTaskbarProperties.IconsAssembly;
			this.thumbnailClipRegion = DefaultTaskbarProperties.ThumbnailClip;
			this.jumpListKnownCategoryVisibility = DefaultTaskbarProperties.DefaultJumpListKnownCategoryVisibility;
			this.jumpListKnownCategoryPosition = DefaultTaskbarProperties.DefaultJumpListKnownCategoryPosition;
			this.DesignTimeManager = CreateDesignTimeManager();
			this.thumbnailButtons = CreateThumbnailButtonCollection();
			this.jumpListTasksCategory = CreateJumpListCategoryItemCollection();
			this.jumpListCustomCategories = CreateJumpListCategoryCollection();
			this.UseSingleJumpListForAllAppInstances = true;
		}
		static TaskbarAssistant defaultTaskbarAssistant;
		public static TaskbarAssistant Default {
			get {
				if(defaultTaskbarAssistant == null)
					defaultTaskbarAssistant = CreateDefaultTaskbarAssistant();
				return defaultTaskbarAssistant;
			}
		}
		[SecuritySafeCritical]
		static TaskbarAssistant CreateDefaultTaskbarAssistant() {
			return new TaskbarAssistant();
		}
		public void Assign(Control parent) {
			this.ParentControl = parent;
		}
		public void Initialize() { }
		protected virtual ThumbnailButtonCollection CreateThumbnailButtonCollection() {
			return new ThumbnailButtonCollection();
		}
		protected virtual JumpListCategoryItemCollection CreateJumpListCategoryItemCollection() {
			return new JumpListCategoryItemCollection();
		}
		protected virtual JumpListCategoryCollection CreateJumpListCategoryCollection() {
			return new JumpListCategoryCollection();
		}
		[SecurityCritical]
		protected internal virtual FilterWindow CreateWindowInternal() {
			return new FilterWindow(this);
		}
		protected virtual TaskbarAssistantDesignTimeManagerBase CreateDesignTimeManager() {
			return new TaskbarAssistantDesignTimeManagerBase(this);
		}
		public static int MaxThumbnailButtonsCount = 7;
		long progressCurrentValue;
		[DefaultValue(DefaultTaskbarProperties.CurrentProgressValue)]
		public long ProgressCurrentValue {
			get { return progressCurrentValue; }
			set {
				if(ProgressCurrentValue == value) return;
				progressCurrentValue = value;
				OnProgressValueChanged(true);
			}
		}
		long progressMaximumValue;
		[DefaultValue(DefaultTaskbarProperties.MaximumProgressValue)]
		public long ProgressMaximumValue {
			get { return progressMaximumValue; }
			set {
				if(ProgressMaximumValue == value) return;
				progressMaximumValue = value;
				OnProgressValueChanged(true);
			}
		}
		TaskbarButtonProgressMode progressMode;
		[DefaultValue(DefaultTaskbarProperties.ProgressState)]
		public TaskbarButtonProgressMode ProgressMode {
			get { return progressMode; }
			set {
				progressMode = value;
				OnProgressModeChanged(true);
			}
		}
		Image overlayIcon;
		[DefaultValue(DefaultTaskbarProperties.OverlayIcon), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public Image OverlayIcon {
			get { return overlayIcon; }
			set {
				if(OverlayIcon == value) return;
				overlayIcon = value;
				OnOverlayIconChanged(true);
			}
		}
		ThumbnailButtonCollection thumbnailButtons;
		[Editor("DevExpress.Utils.Design.Taskbar.ThumbnailButtonCollectionEditor, " + AssemblyInfo.SRAssemblyDesignFull, typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ThumbnailButtonCollection ThumbnailButtons {
			get { return thumbnailButtons; }
			set {
				thumbnailButtons = value;
				OnThumbnailButtonsCollectionChanged(true);
			}
		}
		Rectangle thumbnailClipRegion;
		public Rectangle ThumbnailClipRegion {
			get { return thumbnailClipRegion; }
			set {
				if(ThumbnailClipRegion == value) return;
				thumbnailClipRegion = value;
				OnThumbnailClipRegionChanged(true);
			}
		}
		bool ShouldSerializeThumbnailClipRegion() { return ThumbnailClipRegion != Rectangle.Empty; }
		void ResetThumbnailClipRegion() { ThumbnailClipRegion = Rectangle.Empty; }
		JumpListCategoryItemCollection jumpListTasksCategory;
		[Editor("DevExpress.Utils.Design.Taskbar.JumpListTaskCollectionEditor, " + AssemblyInfo.SRAssemblyDesignFull, typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public JumpListCategoryItemCollection JumpListTasksCategory {
			get { return jumpListTasksCategory; }
			set {
				jumpListTasksCategory = value;
				OnJumpListChanged(true);
			}
		}
		JumpListCategoryCollection jumpListCustomCategories;
		[Editor("DevExpress.Utils.Design.Taskbar.JumpListCategoryCollectionEditor, " + AssemblyInfo.SRAssemblyDesignFull, typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public JumpListCategoryCollection JumpListCustomCategories {
			get { return jumpListCustomCategories; }
			set {
				jumpListCustomCategories = value;
				OnJumpListChanged(true);
			}
		}
		JumpListKnownCategoryVisibility jumpListKnownCategoryVisibility;
		[DefaultValue(DefaultTaskbarProperties.DefaultJumpListKnownCategoryVisibility)]
		public JumpListKnownCategoryVisibility JumpListKnownCategoryVisibility {
			get { return jumpListKnownCategoryVisibility; }
			set {
				if(JumpListKnownCategoryVisibility == value) return;
				jumpListKnownCategoryVisibility = value;
				OnJumpListChanged(true);
			}
		}
		JumpListKnownCategoryPosition jumpListKnownCategoryPosition;
		[DefaultValue(DefaultTaskbarProperties.DefaultJumpListKnownCategoryPosition)]
		public JumpListKnownCategoryPosition JumpListKnownCategoryPosition {
			get { return jumpListKnownCategoryPosition; }
			set {
				if(JumpListKnownCategoryPosition == value) return;
				jumpListKnownCategoryPosition = value;
				OnJumpListChanged(true);
			}
		}
		[DefaultValue(true)]
		public bool UseSingleJumpListForAllAppInstances { get; set; }
		string iconsAssembly;
		[DefaultValue(DefaultTaskbarProperties.IconsAssembly), TypeConverter(typeof(IconsAssemblyConverter))]
		public string IconsAssembly {
			get { return (IsDesignMode || File.Exists(iconsAssembly)) ? iconsAssembly : FullIconsAssembly; }
			set {
				iconsAssembly = value;
				fullIconsAssembly = string.Empty;
				JumpListItemTask.CreateGeneralCollection(File.Exists(iconsAssembly) ? iconsAssembly : FullIconsAssembly);
			}
		}
		string fullIconsAssembly;
		protected internal string FullIconsAssembly {
			get {
				if(string.IsNullOrEmpty(fullIconsAssembly))
					fullIconsAssembly = Path.Combine(DesignTimeManager.GetProjectPath(), iconsAssembly);
				return fullIconsAssembly;
			}
		}
		FilterWindow windowInternal;
		Control parentControl;
		internal bool IsDesignMode { get { return DesignMode; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public TaskbarAssistantDesignTimeManagerBase DesignTimeManager { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Control ParentControl {
			get { return parentControl; }
			set {
				if(ParentControl == value) return;
				parentControl = value;
				if(IsDesignMode || !TaskbarHelper.SupportOSVersion) return;
				SubscribeEvents();
			}
		}
		bool handleAssigned = false;
		void SubscribeEvents() {
			ParentControl.HandleCreated += OnParentControlHandleCreated;
			ParentControl.HandleDestroyed += OnParentControlHandleDestroyed;
			((Form)ParentControl).Activated += OnParentControlActivated;
		}
		void UnsubscribeEvents() {
			ParentControl.HandleCreated -= OnParentControlHandleCreated;
			ParentControl.HandleDestroyed -= OnParentControlHandleDestroyed;
			((Form)ParentControl).Activated -= OnParentControlActivated;
		}
		void OnParentControlHandleCreated(object sender, EventArgs e) {
			AssignFilterHandle();
			this.lockUpdate = false;
			if(!TaskbarHelper.SupportOSVersion) return; 
			NativeMethods.ChangeWindowMessageFilter(TaskbarAssistantCore.WM_TaskbarButtonCreated, NativeMethods.ChangeWindowMessageFilterFlags.Add);
			NativeMethods.ChangeWindowMessageFilter(MSG.WM_COMMAND, NativeMethods.ChangeWindowMessageFilterFlags.Add);
			Refresh();
		}
		void OnParentControlHandleDestroyed(object sender, EventArgs e) {
			this.handleAssigned = false;
			this.lockUpdate = true;
		}
		void OnParentControlActivated(object sender, EventArgs e) {
			TaskbarAssistantCore.ActiveWindowHandle = ParentHandle;
			OnJumpListChanged(true);
		}
		protected virtual void AssignFilterHandle() {
			if(handleAssigned || !IsParentHandleCreated) return;
			this.handleAssigned = true;
			windowInternal.AssignHandle(ParentHandle);
			if(UseSingleJumpListForAllAppInstances) return;
			NativeMethods.SetCurrentProcessExplicitAppUserModelID(Guid.NewGuid().ToString());
		}
		protected internal bool IsParentHandleCreated {
			get { return (ParentControl != null) && ParentControl.IsHandleCreated; }
		}
		protected internal IntPtr ParentHandle {
			get { return IsParentHandleCreated ? ParentControl.Handle : IntPtr.Zero; }
		}
		public void Refresh() {
			if(!ShouldRefresh()) return;
			AssignFilterHandle();
			OnProgressValueChanged(false);
			OnProgressModeChanged(false);
			OnOverlayIconChanged(false);
			OnThumbnailButtonsCollectionChanged(false);
			OnThumbnailClipRegionChanged(false);
			OnJumpListChanged(false);
		}
		protected virtual bool ShouldRefresh() {
			return !IsDesignMode && ParentControl != null && ParentControl.IsHandleCreated && TaskbarHelper.SupportTaskbarFeatures && !lockUpdate;
		}
		protected internal virtual void OnProgressModeChanged(bool forceRefresh) {
			if(!ShouldRefresh()) return;
			if(forceRefresh || ProgressMode != DefaultTaskbarProperties.ProgressState) {
				TaskbarHelper.SetProgressState(ParentHandle, ProgressMode);
				if(ProgressMode == TaskbarButtonProgressMode.Indeterminate) return;
				OnProgressValueChanged(forceRefresh);
			}
		}
		protected internal virtual void OnProgressValueChanged(bool forceRefresh) {
			if(!ShouldRefresh()) return;
			if(ShouldRefreshProgressValue(forceRefresh)) {
				if(ProgressMode == TaskbarButtonProgressMode.NoProgress || ProgressMode == TaskbarButtonProgressMode.Indeterminate) return;
				TaskbarHelper.SetProgressValue(ParentHandle, ProgressCurrentValue, ProgressMaximumValue);
			}
		}
		protected virtual bool ShouldRefreshProgressValue(bool forceRefresh) {
			return forceRefresh || ProgressCurrentValue != DefaultTaskbarProperties.CurrentProgressValue || ProgressMaximumValue != DefaultTaskbarProperties.MaximumProgressValue;
		}
		protected internal virtual void OnOverlayIconChanged(bool forceRefresh) {
			if(!ShouldRefresh()) return;
			if(forceRefresh || OverlayIcon != DefaultTaskbarProperties.OverlayIcon)
				TaskbarHelper.SetOverlayIcon(ParentHandle, (Bitmap)OverlayIcon, string.Empty);
		}
		protected internal virtual void OnThumbnailButtonsCollectionChanged(bool forceRefresh) {
			if(!ShouldRefresh()) return;
			if(forceRefresh || ThumbnailButtons.Count != 0) {
				foreach(ThumbnailButton button in ThumbnailButtons)
					button.WindowHandle = ParentHandle;
				TaskbarHelper.SetTaskbarButtons(ParentHandle, ThumbnailButtons);
				TaskbarHelper.UpdateTaskbarButtons(ParentHandle, ThumbnailButtons);
			}
		}
		protected internal virtual void OnThumbnailClipRegionChanged(bool forceRefresh) {
			if(!ShouldRefresh()) return;
			if(forceRefresh || ThumbnailClipRegion != Rectangle.Empty) {
				Rectangle rect = ThumbnailClipRegion;
				if(rect == Rectangle.Empty)
					rect = new Rectangle(Point.Empty, ParentControl.Bounds.Size);
				TaskbarHelper.SetThumbnailClip(ParentHandle, ref rect);
			}
		}
		protected internal virtual void OnJumpListChanged(bool forceRefresh) {
			if(!ShouldRefresh()) return;
			JumpListInternal list = new JumpListInternal();
			if(forceRefresh)
				list.Refresh();
			CreateTasks(list, JumpListTasksCategory);
			foreach(JumpListCategory category in JumpListCustomCategories)
				CreateCustomGroup(list, category);
			if(JumpListKnownCategoryVisibility != JumpListKnownCategoryVisibility.None)
				list.KnownCategoryToDisplay = JumpListKnownCategoryVisibility;
			list.Refresh(JumpListKnownCategoryPosition);
		}
		void CreateCustomGroup(JumpListInternal list, JumpListCategory category) {
			JumpListCategoryInternal group = new JumpListCategoryInternal(category.Caption);
			foreach (JumpListItemTask item in category.JumpItems.OfType<JumpListItemTask>())
				group.Add(ConvertJumpItem(item));
			list.AddCategory(group);
		}
		void CreateTasks(JumpListInternal list, JumpListCategoryItemCollection category) {
			foreach(IJumpListItem item in category) {
				if(item is JumpListItemSeparator) list.AddTask(new JumpListSeparatorInternal());
				if(item is JumpListItemTask) list.AddTask(ConvertJumpItem(item as JumpListItemTask));
			}
		}
		JumpListLinkInternal ConvertJumpItem(JumpListItemTask item) {
			JumpListLinkInternal link = new JumpListLinkInternal();
			link.Title = item.Caption;
			if(item.CanUseStandardBehavior && !item.IsEmptyTask) {
				link.Path = item.Path;
				link.Arguments = item.Arguments;
			}
			else {
				link.Path = Application.ExecutablePath;
				foreach(string arg in TaskbarAssistantCore.GetJumpListItemArguments(item))
					link.Arguments += arg + " ";
			}
			link.ShowCommand = item.ShowCommand;
			link.WorkingDirectory = item.WorkingDirectory;
			if(!string.IsNullOrEmpty(FullIconsAssembly))
				if(item.IconIndex < 0) link.IconReference = new IconReference(item.Path, 0);
				else link.IconReference = new IconReference(string.IsNullOrEmpty(item.IconPath) ? FullIconsAssembly : item.IconPath, item.IconIndex);
			return link;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(ParentControl != null) 
					UnsubscribeEvents();
				if(JumpListTasksCategory != null) 
					JumpListTasksCategory.Dispose();
				if(JumpListCustomCategories != null)
					JumpListCustomCategories.Dispose();
				if(ThumbnailButtons != null) 
					ThumbnailButtons.Dispose();
			}
			NativeMethods.PostMessage((IntPtr)0xffff, TaskbarAssistantCore.WM_TaskbarRefreshJumpList, IntPtr.Zero, IntPtr.Zero);
			base.Dispose(disposing);
		}
		bool lockUpdate;
		public void BeginUpdate() {
			lockUpdate = true;
		}
		public void EndUpdate() {
			if(!lockUpdate) return;
			lockUpdate = false;
			Refresh();
		}
	}
	#region DesignTime Manager
	public class TaskbarAssistantDesignTimeManagerBase {
		IComponent component;
		public TaskbarAssistantDesignTimeManagerBase(IComponent component) {
			this.component = component;
		}
		TaskbarAssistant TaskbarAssistant { get { return this.component as TaskbarAssistant; } }
		public string GetProjectPath() {
			return TaskbarAssistant.IsDesignMode ? GetProjectPathDesignTime() : GetIconsPathRunTime();
		}
		protected virtual string GetIconsPathRunTime() {
			string location = Assembly.GetEntryAssembly().Location;
			return Path.Combine(Directory.GetParent(location).ToString());
		}
		protected virtual string GetProjectPathDesignTime() {
			return string.Empty;
		}
	}
	#endregion
	#region JumpList
	public class JumpList {
		public JumpList() {
			Categories = new JumpListCategoryCollection();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public JumpListCategoryCollection Categories { get; set; }
		public static void AddRecentFile(string path) {
			JumpListInternal.AddToRecent(path);
		}
	}
	public class JumpListCategoryCollection : Collection<JumpListCategory>, IDisposable {
		public void Dispose() {
			foreach(JumpListCategory category in this)
				category.Dispose();
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class JumpListCategory : Component {
		public JumpListCategory() : this(string.Empty) { }
		public JumpListCategory(string caption) {
			Caption = caption;
			JumpItems = new JumpListCategoryItemCollection();
		}
		public string Caption { get; set; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public JumpListCategoryItemCollection JumpItems { get; set; }
		protected override void Dispose(bool disposing) {
			if(disposing) JumpItems.Dispose();
			base.Dispose(disposing);
		}
	}
	public class JumpListCategoryItemCollection : Collection<IJumpListItem>, IDisposable {
		public void Dispose() {
			foreach(Component item in this)
				item.Dispose();
		}
	}
	[ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("Click")]
	public class JumpListItemTask : Component, IJumpListItem, ISupportJumpListItemClick, ICloneable, DevExpress.Utils.MVVM.ISupportCommandBinding {
		public JumpListItemTask() : this(string.Empty) { }
		public JumpListItemTask(string caption) {
			Path = string.Empty;
			Caption = caption;
			IconIndex = -1;
			Arguments = string.Empty;
			ShowCommand = WindowShowCommand.Normal;
			WorkingDirectory = string.Empty;
		}
		[DefaultValue(""), Editor("DevExpress.Utils.Design.Taskbar.JumpListItemChangeFile, " + AssemblyInfo.SRAssemblyDesignFull, typeof(UITypeEditor))]
		public string Path { get; set; }
		[DefaultValue("")]
		public string Caption { get; set; }
		[DefaultValue(-1), Editor(typeof(DevExpress.Utils.Design.ImageIndexesEditor), typeof(UITypeEditor)), DevExpress.Utils.ImageList("Collection")]
		public int IconIndex { get; set; }
		[DefaultValue("")]
		public string Arguments { get; set; }
		[DefaultValue("")]
		public string WorkingDirectory { get; set; }
		[DefaultValue(typeof(WindowShowCommand), "Normal")]
		public WindowShowCommand ShowCommand { get; set; }
		string iconPath;
		[DefaultValue(""), Editor("DevExpress.Utils.Design.Taskbar.JumpListItemChangeFile, " + AssemblyInfo.SRAssemblyDesignFull, typeof(UITypeEditor))]
		public string IconPath {
			get { return iconPath; }
			set {
				iconPath = value;
				ClearIndividualCollection();
				if(string.IsNullOrEmpty(IconPath)) return;
				CreateIndividualCollection(IconPath);
			}
		}
		public object Clone() {
			JumpListItemTask clone = new JumpListItemTask();
			clone.Arguments = this.Arguments;
			clone.Caption = this.Caption;
			clone.IconIndex = this.IconIndex;
			clone.Path = this.Path;
			clone.ShowCommand = this.ShowCommand;
			clone.WorkingDirectory = this.WorkingDirectory;
			return clone;
		}
		bool IsAssembly(string path) {
			string ext = System.IO.Path.GetExtension(path).ToLower();
			return ext == ".dll" || ext == ".exe";
		}
		void CreateIndividualCollection(string path) {
			individualIconsCollection = new ImageCollection();
			if(!File.Exists(path)) return;
			if(!IsAssembly(path)) individualIconsCollection.AddImage(new Bitmap(Bitmap.FromFile(path), new Size(16, 16)));
			else InitializeIconsCollection(individualIconsCollection, path);
		}
		void ClearIndividualCollection() {
			if(individualIconsCollection == null) return;
			individualIconsCollection.Clear();
			individualIconsCollection = null;
		}
		internal static void CreateGeneralCollection(string path) {
			generalIconsCollection = new ImageCollection();
			if(!File.Exists(path)) return;
			InitializeIconsCollection(generalIconsCollection, path);
		}
		static void InitializeIconsCollection(ImageCollection collection, string assemblyPath) {
			foreach(Icon item in IconExtractor.ExtractFromAssembly(assemblyPath, IconSize.Small))
				collection.AddImage(item.ToBitmap());
		}
		static ImageCollection generalIconsCollection;
		ImageCollection individualIconsCollection;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ImageCollection Collection { get { return individualIconsCollection ?? generalIconsCollection; } }
		string id;
		internal string Id { get { return id ?? (id = Guid.NewGuid().ToString()); } }
		internal int message;
		internal EventHandler click;
		public event EventHandler Click {
			add {
				click += value;
				message = NativeMethods.RegisterWindowMessage(Id);
				TaskbarAssistantCore.WindowMessages[message] = this;
			}
			remove {
				click -= value;
				if(click != null) return;
				TaskbarAssistantCore.WindowMessages.Remove(message);
				message = 0;
			}
		}
		protected virtual internal bool IsEmptyTask { get { return CanUseStandardBehavior && string.IsNullOrEmpty(Path); } }
		protected virtual internal bool CanUseStandardBehavior { get { return click == null; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void RaiseClick() {
			if(click == null) return;
			click(this, EventArgs.Empty);
		}
		#region Commands
		public IDisposable BindCommand(object command, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(task, execute) => task.Click += (s, e) => execute(),
				(task, canExecute) => { },
				command, queryCommandParameter);
		}
		public IDisposable BindCommand(System.Linq.Expressions.Expression<Action> commandSelector, object source, Func<object> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(task, execute) => task.Click += (s, e) => execute(),
				(task, canExecute) => { },
				commandSelector, source, queryCommandParameter);
		}
		public IDisposable BindCommand<T>(System.Linq.Expressions.Expression<Action<T>> commandSelector, object source, Func<T> queryCommandParameter = null) {
			return DevExpress.Utils.MVVM.CommandHelper.Bind(this,
				(task, execute) => task.Click += (s, e) => execute(),
				(task, canExecute) => { },
				commandSelector, source, () => (queryCommandParameter != null) ? queryCommandParameter() : default(T));
		}
		#endregion Commands
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class JumpListItemSeparator : Component, IJumpListItem {
		public override string ToString() {
			return "Separator";
		}
	}
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IJumpListItem { }
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ISupportJumpListItemClick {
		event EventHandler Click;
		void RaiseClick();
	}
	#endregion
	#region ThumbnailButton
	[ToolboxItem(false), DesignTimeVisible(false), DefaultEvent("Click")]
	public class ThumbnailButton : Component {
		public ThumbnailButton() : this(null, string.Empty, IntPtr.Zero) { }
		public ThumbnailButton(Bitmap bitmap, string tooltip, IntPtr parentHandle) {
			isReady = false;
			WindowHandle = parentHandle;
			Id = nextId++;
			Image = bitmap;
			Tooltip = tooltip;
			Visible = true;
			Enabled = true;
			DismissOnClick = false;
			IsInteractive = true;
			IsReady = true;
		}
		static uint nextId = 101;
		internal uint Id { get; set; }
		bool isReady;
		bool IsReady {
			get { return isReady; }
			set {
				if(IsReady == value) return;
				isReady = value;
				if(IsReady) RefreshThumbButton();
			}
		}
		Bitmap image;
		[DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Bitmap Image {
			get { return image; }
			set {
				if(Image == value) return;
				image = value;
				RefreshThumbButton();
			}
		}
		string tooltip;
		[DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public string Tooltip {
			get { return tooltip; }
			set {
				if(Tooltip == value) return;
				tooltip = value;
				RefreshThumbButton();
			}
		}
		[DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool Visible {
			get { return (Flags & ThumbButtonFlags.Hidden) == 0; }
			set {
				if(Visible == value) return;
				if(value) Flags &= ~(ThumbButtonFlags.Hidden);
				else Flags |= ThumbButtonFlags.Hidden;
				RefreshThumbButton();
			}
		}
		[DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool Enabled {
			get { return (Flags & ThumbButtonFlags.Disabled) == 0; }
			set {
				if(Enabled == value) return;
				if(value) Flags &= ~(ThumbButtonFlags.Disabled);
				else Flags |= ThumbButtonFlags.Disabled;
				RefreshThumbButton();
			}
		}
		[DefaultValue(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool DismissOnClick {
			get { return (Flags & ThumbButtonFlags.DismissOnClick) != 0; }
			set {
				if(DismissOnClick == value) return;
				if(value) Flags |= ThumbButtonFlags.DismissOnClick;
				else Flags &= ~(ThumbButtonFlags.DismissOnClick);
				RefreshThumbButton();
			}
		}
		[DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public bool IsInteractive {
			get { return (Flags & ThumbButtonFlags.NonInteractive) == 0; }
			set {
				if(IsInteractive == value) return;
				if(value) Flags &= ~(ThumbButtonFlags.NonInteractive);
				else Flags |= ThumbButtonFlags.NonInteractive;
				RefreshThumbButton();
			}
		}
		internal ThumbButtonFlags Flags { get; set; }
		internal ThumbnailButtonCore ThumbButtonCore {
			get {
				ThumbnailButtonCore thumbButtonCore = new ThumbnailButtonCore();
				thumbButtonCore.Id = (int)Id;
				thumbButtonCore.Tip = Tooltip;
				thumbButtonCore.Icon = Image != null ? Image.GetHicon() : IntPtr.Zero;
				thumbButtonCore.Flags = Flags;
				thumbButtonCore.Mask = ThumbButtonMask.Flags;
				if(Tooltip != null) thumbButtonCore.Mask |= ThumbButtonMask.Tooltip;
				if(Image != null) thumbButtonCore.Mask |= ThumbButtonMask.Icon;
				return thumbButtonCore;
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IntPtr WindowHandle { get; set; }
		internal void RefreshThumbButton() {
			if(!TaskbarHelper.SupportTaskbarFeatures) return;
			if(!IsReady || WindowHandle == IntPtr.Zero) return;
			ThumbnailButtonCore[] nativeButtons = { ThumbButtonCore };
			try {
				TaskbarManager.Instance.ThumbBarUpdateButtons(WindowHandle, 1, nativeButtons);
			}
			finally {
				foreach(ThumbnailButtonCore button in nativeButtons)
					button.Dispose();
			}
		}
		internal void RaiseThumbButtonClick(IntPtr handle) {
			if(Click == null) return;
			Click(this, new ThumbButtonClickEventArgs(handle, this));
		}
		public event EventHandler<ThumbButtonClickEventArgs> Click;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Image != null)
					Image.Dispose();
				Image = null;
			}
			base.Dispose(disposing);
		}
	}
	public class ThumbButtonClickEventArgs : EventArgs {
		public ThumbButtonClickEventArgs(IntPtr handle, ThumbnailButton button) {
			ThumbButton = button;
			Handle = handle;
		}
		public IntPtr Handle { get; private set; }
		public ThumbnailButton ThumbButton { get; private set; }
	}
	public class ThumbnailButtonCollection : Collection<ThumbnailButton>, IDisposable {
		public void Dispose() {
			foreach(ThumbnailButton button in this)
				button.Dispose();
		}
	} 
	#endregion
}
