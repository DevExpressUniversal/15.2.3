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
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils.Animation;
namespace DevExpress.Utils {
	[Designer("DevExpress.Utils.Design.WorkspaceManagerDesigner, " + AssemblyInfo.SRAssemblyDesignFull)]
	[Description("Allows you to implement different workspace in your application.")]
	[ToolboxTabName(AssemblyInfo.DXTabComponents), DXToolboxItem(DXToolboxItemKind.Regular)]
	[System.Drawing.ToolboxBitmap(typeof(DevExpress.Utils.ToolBoxIcons.ToolboxIconsRootNS), "WorkspaceManager")]
	public class WorkspaceManager : Component {
		internal static ArrayList workspaceManagers;
		internal static ArrayList disabledControls;
		TransitionManager transitionManagerCore;
		Control targetControlCore;
		List<IWorkspace> workspacesCore;
		ITransitionAnimator transitionTypeCore;
		public WorkspaceManager() {
			transitionManagerCore = new TransitionManager();
			workspacesCore = new List<IWorkspace>();
			recentWorkspacesCore = new List<IWorkspace>();
			transitionTypeCore = new PushTransition();
			CloseStreamOnWorkspaceSaving = DefaultBoolean.Default;
			CloseStreamOnWorkspaceLoading = DefaultBoolean.Default;
			AllowTransitionAnimation = DefaultBoolean.Default;
			lock(workspaceManagers.SyncRoot) {
				workspaceManagers.Add(this);
			}
		}
		static WorkspaceManager() {
			workspaceManagers = new ArrayList();
			disabledControls = new ArrayList();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				Ref.Dispose(ref transitionManagerCore);
				workspacesCore = null;
				targetControlCore = null;
			}
			disabledControls.Clear();
			lock(workspaceManagers.SyncRoot) {
				workspaceManagers.Remove(this);
			}
		}
		public static void SetSerializationEnabled(Component component, bool enabled) {
			RemoveDeadWeakreference();
			ComponentWeakReference reference = new ComponentWeakReference(component);
			if(enabled && disabledControls.Contains(reference))
				disabledControls.Remove(reference);
			if(!enabled && !disabledControls.Contains(reference))
				disabledControls.Add(reference);
		}
		internal static void RemoveDeadWeakreference() {
			for(int i = WorkspaceManager.disabledControls.Count - 1; i > -1; i--) {
				ComponentWeakReference reference = WorkspaceManager.disabledControls[i] as ComponentWeakReference;
				if(!reference.IsAlive)
					WorkspaceManager.disabledControls.Remove(reference);
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("WorkspaceManagerAllowTransitionAnimation"),
#endif
 Category("Behavior"), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean AllowTransitionAnimation { get; set; }
		[Browsable(false)]
		public List<IWorkspace> Workspaces { get { return workspacesCore; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("WorkspaceManagerTargetControl"),
#endif
 Category("Behavior"), DefaultValue(null)]
		public Control TargetControl {
			get { return targetControlCore; }
			set { targetControlCore = value; }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("WorkspaceManagerTransitionType"),
#endif
 Category("Behavior"), TypeConverter(typeof(TransitionAnimatorTypeConverter))]
		public ITransitionAnimator TransitionType {
			get { return transitionTypeCore; }
			set {
				if(transitionTypeCore == value) return;
				Ref.Dispose(ref transitionTypeCore);
				transitionTypeCore = value;
			}
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("WorkspaceManagerCloseStreamOnWorkspaceSaving"),
#endif
 Category("Behavior"), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean CloseStreamOnWorkspaceSaving { get; set; }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("WorkspaceManagerCloseStreamOnWorkspaceLoading"),
#endif
 Category("Behavior"), DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean CloseStreamOnWorkspaceLoading { get; set; }
		protected void ResetTransitionType() { TransitionType = new PushTransition(); }
		protected internal virtual string SerializationName {
			get { return TargetControl == null ? string.Empty : TargetControl.GetType().Name; }
		}
		protected TransitionManager TransitionManager {
			get { return transitionManagerCore; }
		}
		#region Events
		static readonly object beforeApplyWorkspace = new object();
		static readonly object afterApplyWorkspace = new object();
		static readonly object workspaceSaved = new object();
		static readonly object workspaceAdded = new object();
		static readonly object workspaceRemoved = new object();
		[
#if !SL
	DevExpressUtilsLocalizedDescription("WorkspaceManagerBeforeApplyWorkspace"),
#endif
 Category("Behavior")]
		public event EventHandler BeforeApplyWorkspace {
			add { Events.AddHandler(beforeApplyWorkspace, value); }
			remove { Events.RemoveHandler(beforeApplyWorkspace, value); }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("WorkspaceManagerAfterApplyWorkspace"),
#endif
 Category("Behavior")]
		public event EventHandler AfterApplyWorkspace {
			add { Events.AddHandler(afterApplyWorkspace, value); }
			remove { Events.RemoveHandler(afterApplyWorkspace, value); }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("WorkspaceManagerWorkspaceSaved"),
#endif
 Category("Behavior")]
		public event WorkspaceEventHandler WorkspaceSaved {
			add { Events.AddHandler(workspaceSaved, value); }
			remove { Events.RemoveHandler(workspaceSaved, value); }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("WorkspaceManagerWorkspaceAdded"),
#endif
 Category("Behavior")]
		public event WorkspaceEventHandler WorkspaceAdded {
			add { Events.AddHandler(workspaceAdded, value); }
			remove { Events.RemoveHandler(workspaceAdded, value); }
		}
		[
#if !SL
	DevExpressUtilsLocalizedDescription("WorkspaceManagerWorkspaceRemoved"),
#endif
 Category("Behavior")]
		public event WorkspaceEventHandler WorkspaceRemoved {
			add { Events.AddHandler(workspaceRemoved, value); }
			remove { Events.RemoveHandler(workspaceRemoved, value); }
		}
		protected virtual void RaiseBeforeApplyWorkspace(IWorkspace workspace) {
			EventHandler handler = Events[beforeApplyWorkspace] as EventHandler;
			WorkspaceEventArgs ea = new WorkspaceEventArgs(workspace);
			if(handler != null) handler(this, ea as EventArgs);
		}
		protected virtual void RaiseAfterApplyWorkspace(IWorkspace workspace) {
			EventHandler handler = Events[afterApplyWorkspace] as EventHandler;
			WorkspaceEventArgs ea = new WorkspaceEventArgs(workspace);
			if(handler != null) handler(this, ea as EventArgs);
		}
		protected virtual void RaiseWorkspaceSaved(IWorkspace workspace) {
			WorkspaceEventHandler handler = Events[workspaceSaved] as WorkspaceEventHandler;
			WorkspaceEventArgs ea = new WorkspaceEventArgs(workspace);
			if(handler != null) handler(this, ea);
		}
		protected virtual void RaiseWorkspaceAdded(IWorkspace workspace) {
			WorkspaceEventHandler handler = Events[workspaceAdded] as WorkspaceEventHandler;
			WorkspaceEventArgs ea = new WorkspaceEventArgs(workspace);
			if(handler != null) handler(this, ea);
		}
		protected virtual void RaiseWorkspaceRemoved(IWorkspace workspace) {
			WorkspaceEventHandler handler = Events[workspaceRemoved] as WorkspaceEventHandler;
			WorkspaceEventArgs ea = new WorkspaceEventArgs(workspace);
			if(handler != null) handler(this, ea);
		}
		#endregion
		public void CaptureWorkspace(string name, bool acceptNestedObjects = true) {
			CaptureWorkspaceCore(name, acceptNestedObjects);
		}
		void CaptureWorkspaceCore(string name, bool acceptNestedObjects = true, string path = "") {
			if(TargetControl == null || string.IsNullOrEmpty(name))
				return;
			using(MemoryStream stream = new MemoryStream()) {
				SerializeTargetControl(stream, acceptNestedObjects);
				AddWorkspaceCore(name, stream.ToArray(), path);
			}
		}
		protected void SerializeTargetControl(Stream stream, bool acceptNestedObjects) {
			WorkspaceManagerSerializer.Serialize(TargetControl, stream, SerializationName, acceptNestedObjects);
		}
		public void RemoveWorkspace(string name) {
			IWorkspace workspace = GetWorkspace(name);
			if(workspace != null) {
				Workspaces.Remove(workspace);
				RaiseWorkspaceRemoved(workspace);
			}
			if(RecentWorkspaces.Contains(workspace))
				RecentWorkspaces.Remove(workspace);
		}
		public void RenameWorkspace(string oldName, string newName) {
			IWorkspace workspace = GetWorkspace(oldName);
			if(workspace == null || string.IsNullOrEmpty(newName) || oldName == newName)
				return;
			int index = GetWorkspaceIndex(oldName);
			int recentIndex = GetRecentWorkspaceIndex(oldName);
			Workspaces[index] = new Workspace(newName, workspace.SerializationData);
			if(recentIndex != -1) {
				RecentWorkspaces[recentIndex] = Workspaces[index];
			}
		}
		public void ApplyWorkspace(string name) {
			using(TransitionCreator transition = new TransitionCreator(this)) {
				IWorkspace workspace = GetWorkspace(name);
				if(workspace == null) return;
				RaiseBeforeApplyWorkspace(workspace);
				ApplyWorkspaceCore(workspace);
				RaiseAfterApplyWorkspace(workspace);
				if(!RecentWorkspaces.Contains(workspace))
					RecentWorkspaces.Insert(0, workspace);
				else {
					RecentWorkspaces.Remove(workspace);
					RecentWorkspaces.Insert(0, workspace);
				}
			}
		}
		List<IWorkspace> recentWorkspacesCore;
		[Browsable(false)]
		public List<IWorkspace> RecentWorkspaces {
			get { return recentWorkspacesCore; }
		}
		class TransitionCreator : IDisposable {
			WorkspaceManager workspaceManager;
			public TransitionCreator(WorkspaceManager manager) {
				workspaceManager = manager;
				workspaceManager.TransitionManager.Transitions.Add(new Transition()
				{
					Control = workspaceManager.TargetControl,
					ShowWaitingIndicator = DefaultBoolean.True,
					TransitionType = workspaceManager.TransitionType
				});
				if(workspaceManager.AllowTransitionAnimation != DefaultBoolean.False)
					workspaceManager.TransitionManager.StartTransition(workspaceManager.TargetControl);
			}
			public void Dispose() {
				if(workspaceManager.AllowTransitionAnimation != DefaultBoolean.False)
					workspaceManager.TargetControl.BeginInvoke(new MethodInvoker(() => workspaceManager.TransitionManager.EndTransition()));
				workspaceManager.TransitionManager.Transitions.Clear();
			}
		}
		public bool LoadWorkspace(string name, Stream stream) {
			return LoadWorkspaceCore(name, stream);
		}
		public bool SaveWorkspace(string name, Stream stream, bool createIfNotExisting = false) {
			return SaveWorkspaceCore(name, stream, true, createIfNotExisting);
		}
		public bool LoadWorkspace(string name, object path) {
			bool keepOpened;
			return LoadWorkspaceCore(name, GetLoadStream(path, out keepOpened), keepOpened);
		}
		public bool SaveWorkspace(string name, object path, bool createIfNotExisting = false) {
			bool keepOpened;
			return SaveWorkspaceCore(name, GetSaveStream(path, out keepOpened), keepOpened, createIfNotExisting);
		}
		protected virtual void AddWorkspaceCore(string name, object serializationData) {
			AddWorkspaceCore(name, serializationData, string.Empty);
		}
		protected virtual void AddWorkspaceCore(string name, object serializationData, string path) {
			if(string.IsNullOrEmpty(name) || serializationData == null)
				return;
			IWorkspace workspace = new Workspace(name, serializationData);
			if(path != null && !path.Equals(string.Empty)) {
				workspace.Path = path;
			}
			int index = GetWorkspaceIndex(name);
			if(index != -1)
				Workspaces[index] = workspace;
			else {
				Workspaces.Add(workspace);
				RaiseWorkspaceAdded(workspace);
			}
		}
		protected virtual void ApplyWorkspaceCore(IWorkspace workspace) {
			if(workspace == null)
				return;
			Stream stream = GetSerializationStream(workspace);
			if(stream == null)
				return;
			using(stream)
				DeserializeTargetControl(stream);
		}
		protected void DeserializeTargetControl(Stream stream) {
			WorkspaceManagerSerializer.Deserialize(TargetControl, stream, SerializationName);
		}
		protected virtual bool LoadWorkspaceCore(string name, Stream stream) {
			return LoadWorkspaceCore(name, stream, true);
		}
		protected virtual bool LoadWorkspaceCore(string name, Stream stream, bool keepOpened) {
			if(string.IsNullOrEmpty(name) || stream == null)
				return false;
			try {
				byte[] buffer = new byte[stream.Length];
				stream.Read(buffer, 0, buffer.Length);
				FileStream fileStream = stream as FileStream;
				string path = (fileStream != null && fileStream.Name != null) ? fileStream.Name : string.Empty;
				AddWorkspaceCore(name, buffer, path);
				return true;
			}
			catch { return false; }
			finally {
				if((CloseStreamOnWorkspaceLoading == DefaultBoolean.Default & !keepOpened) || (CloseStreamOnWorkspaceLoading == DefaultBoolean.True))
					stream.Dispose();
			}
		}
		protected virtual bool SaveWorkspaceCore(string name, Stream stream, bool keepOpened, bool createIfNotExisting) {
			if(stream == null)
				return false;
			FileStream fileStream = stream as FileStream;
			string path = (fileStream != null && fileStream.Name != null) ? fileStream.Name : string.Empty;
			IWorkspace workspace = GetWorkspace(name);
			if(!ValidateWorkspace(workspace) && createIfNotExisting) {
				if(!path.Equals(string.Empty))
					CaptureWorkspaceCore(name, true, path);
				else
					CaptureWorkspace(name);
				workspace = GetWorkspace(name);
			}
			if(!ValidateWorkspace(workspace)) return false;
			if(!path.Equals(string.Empty) && workspace.Path != path)
				workspace.Path = path;
			try {
				byte[] buffer = (byte[])workspace.SerializationData;
				stream.Write(buffer, 0, buffer.Length);
				stream.Flush();
				RaiseWorkspaceSaved(workspace);
				return true;
			}
			catch { return false; }
			finally {
				if((CloseStreamOnWorkspaceSaving == DefaultBoolean.Default & !keepOpened) || (CloseStreamOnWorkspaceSaving == DefaultBoolean.True))
					stream.Dispose();
			}
		}
		protected virtual bool WorkspaceNameEquals(IWorkspace workspace, string name) {
			return string.Equals(workspace.Name, name);
		}
		protected internal virtual bool ValidateWorkspace(IWorkspace workspace) {
			return workspace != null && !string.IsNullOrEmpty(workspace.Name) && (workspace.SerializationData as byte[]) != null;
		}
		public virtual IWorkspace GetWorkspace(int index) {
			return (TargetControl == null || index < 0 || index >= Workspaces.Count) ? null : Workspaces[index];
		}
		public virtual IWorkspace GetWorkspace(string name) {
			return (TargetControl == null || string.IsNullOrEmpty(name)) ? null :
				Workspaces.Find(workspace => WorkspaceNameEquals(workspace, name));
		}
		protected internal virtual int GetWorkspaceIndex(string name) {
			for(int i = 0; i < Workspaces.Count; i++) {
				if(WorkspaceNameEquals(Workspaces[i], name))
					return i;
			}
			return -1;
		}
		protected internal virtual int GetRecentWorkspaceIndex(string name) {
			for(int i = 0; i < RecentWorkspaces.Count; i++) {
				if(WorkspaceNameEquals(RecentWorkspaces[i], name))
					return i;
			}
			return -1;
		}
		protected virtual Stream GetSerializationStream(IWorkspace workspace) {
			return ValidateWorkspace(workspace) ? new MemoryStream((byte[])workspace.SerializationData) : null;
		}
		protected virtual Stream GetFileStream(string filePath, bool save) {
			try {
				FileMode mode = save ? FileMode.Create : FileMode.Open;
				return File.Open(filePath, mode);
			}
			catch { return null; }
		}
		protected virtual Stream GetStreamCore(object path, bool save, out bool keepOpened) {
			keepOpened = false;
			if(path is string && !string.IsNullOrEmpty((string)path))
				return GetFileStream((string)path, save);
			if(path is Stream) {
				keepOpened = true;
				return (Stream)path;
			}
			return null;
		}
		protected virtual Stream GetLoadStream(object path) {
			bool keepOpened;
			return GetLoadStream(path, out keepOpened);
		}
		protected virtual Stream GetLoadStream(object path, out bool keepOpened) {
			return GetStreamCore(path, false, out keepOpened);
		}
		protected virtual Stream GetSaveStream(object path) {
			bool keepOpened;
			return GetSaveStream(path, out keepOpened);
		}
		protected virtual Stream GetSaveStream(object path, out bool keepOpened) {
			return GetStreamCore(path, true, out keepOpened);
		}
		public static WorkspaceManager FromControl(Control control) {
			WorkspaceManager manager = null;
			while(control != null) {
				manager = FromControlCore(control);
				if(manager != null) return manager;
				control = control.Parent;
			}
			return null;
		}
		static WorkspaceManager FromControlCore(Control control) {
			lock(workspaceManagers.SyncRoot) {
				foreach(WorkspaceManager manager in workspaceManagers) {
					if(manager.TargetControl == control) return manager;
				}
			}
			return null;
		}
	}
	internal class ComponentWeakReference : WeakReference {
		int componentHashCodeCore;
		public ComponentWeakReference(object component)
			: base(component) {
			componentHashCodeCore = component.GetHashCode();
		}
		public override int GetHashCode() {
			return componentHashCodeCore;
		}
		public override bool Equals(object value) {
			if(value == null) {
				return false;
			}
			if(value.GetHashCode() != componentHashCodeCore) {
				return false;
			}
			return true;
		}
	}
	public interface IWorkspace {
		string Name { get; }
		string Path { get; set; }
		object SerializationData { get; }
	}
	class Workspace : IWorkspace {
		readonly string name;
		readonly object serializationData;
		public Workspace(string name, object serializationData) {
			this.name = name;
			this.serializationData = serializationData;
		}
		public string Name { get { return name; } }
		public string Path { get; set; }
		public object SerializationData { get { return serializationData; } }
	}
	public delegate void WorkspaceEventHandler(object sender, WorkspaceEventArgs args);
	public class WorkspaceEventArgs : EventArgs {
		public WorkspaceEventArgs(IWorkspace workspace) {
			Workspace = workspace;
		}
		public IWorkspace Workspace { get; private set; }
	}
}
