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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.Utils.Taskbar.Core {
	#region ComObjects
	[ComImport, Guid("56FDF344-FD6D-11d0-958A-006097C9A090"), ClassInterface(ClassInterfaceType.None)]
	internal class CTaskbarList { }
	[ComImport, Guid("00021401-0000-0000-C000-000000000046"), ClassInterface(ClassInterfaceType.None)]
	internal class CShellLink { }
	[ComImport, Guid("77F10CF0-3DB5-4966-B520-B7C54FD35ED6"), ClassInterface(ClassInterfaceType.None)]
	internal class CDestinationList { }
	[ComImport, Guid("2D3468C1-36A7-43B6-AC24-D3F02FD9607A"), ClassInterface(ClassInterfaceType.None)]
	internal class CEnumerableObjectCollection { }
	[ComImport, Guid("6332DEBF-87B5-4670-90C0-5E57B408A49E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ICustomDestinationList {
		void SetAppID([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
		[PreserveSig]
		HResult BeginList(out uint cMaxSlots, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
		[PreserveSig]
		HResult AppendCategory([MarshalAs(UnmanagedType.LPWStr)] string pszCategory, [MarshalAs(UnmanagedType.Interface)] IObjectArray poa);
		void AppendKnownCategory([MarshalAs(UnmanagedType.I4)] JumpListKnownCategoryVisibility category);
		[PreserveSig]
		HResult AddUserTasks([MarshalAs(UnmanagedType.Interface)] IObjectArray poa);
		void CommitList();
		void GetRemovedDestinations(ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
		void DeleteList([MarshalAs(UnmanagedType.LPWStr)] string pszAppID);
		void AbortList();
	}
	[ComImport, Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IObjectArray {
		void GetCount(out uint cObjects);
		void GetAt(uint iIndex, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
	}
	[ComImport, Guid("5632B1A4-E38A-400A-928A-D4CD63230295"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IObjectCollection {
		[PreserveSig]
		void GetCount(out uint cObjects);
		[PreserveSig]
		void GetAt(uint iIndex, ref Guid riid, [Out(), MarshalAs(UnmanagedType.Interface)] out object ppvObject);
		void AddObject([MarshalAs(UnmanagedType.Interface)] object pvObject);
		void AddFromArray([MarshalAs(UnmanagedType.Interface)] IObjectArray poaSource);
		void RemoveObject(uint uiIndex);
		void Clear();
	}
	[ComImport, Guid("000214F9-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IShellLinkW {
		void GetPath([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxPath, IntPtr pfd, uint fFlags);
		void GetIDList(out IntPtr ppidl);
		void SetIDList(IntPtr pidl);
		void GetDescription([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszFile, int cchMaxName);
		void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
		void GetWorkingDirectory([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszDir, int cchMaxPath);
		void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
		void GetArguments([Out(), MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszArgs, int cchMaxPath);
		void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
		void GetHotKey(out short wHotKey);
		void SetHotKey(short wHotKey);
		void GetShowCmd(out uint iShowCmd);
		void SetShowCmd(uint iShowCmd);
		void GetIconLocation([Out(), MarshalAs(UnmanagedType.LPWStr)] out StringBuilder pszIconPath, int cchIconPath, out int iIcon);
		void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
		void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
		void Resolve(IntPtr hwnd, uint fFlags);
		void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
	}
	[ComImport, Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IPropertyStore {
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		HResult GetCount([Out] out uint propertyCount);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		HResult GetAt([In] uint propertyIndex, out PropertyKey key);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		HResult GetValue([In] ref PropertyKey key, [Out] NativeMethods.PropVariant pv);
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
		HResult SetValue([In] ref PropertyKey key, [In] NativeMethods.PropVariant pv);
		[PreserveSig]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		HResult Commit();
	}
	[ComImport, Guid("c43dc798-95d1-4bea-9030-bb99e2983a1a"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface ITaskbarList4 {
		[PreserveSig]
		void HrInit();
		[PreserveSig]
		void AddTab(IntPtr hwnd);
		[PreserveSig]
		void DeleteTab(IntPtr hwnd);
		[PreserveSig]
		void ActivateTab(IntPtr hwnd);
		[PreserveSig]
		void SetActiveAlt(IntPtr hwnd);
		[PreserveSig]
		void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);
		[PreserveSig]
		void SetProgressValue(IntPtr hwnd, UInt64 ullCompleted, UInt64 ullTotal);
		[PreserveSig]
		void SetProgressState(IntPtr hwnd, TaskbarButtonProgressMode tbpFlags);
		[PreserveSig]
		void RegisterTab(IntPtr hwndTab, IntPtr hwndMDI);
		[PreserveSig]
		void UnregisterTab(IntPtr hwndTab);
		[PreserveSig]
		void SetTabOrder(IntPtr hwndTab, IntPtr hwndInsertBefore);
		[PreserveSig]
		void SetTabActive(IntPtr hwndTab, IntPtr hwndMDI, ThumbTabFlags tbatFlags);
		[PreserveSig]
		void ThumbBarAddButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] ThumbnailButtonCore[] pButtons);
		[PreserveSig]
		void ThumbBarUpdateButtons(IntPtr hwnd, uint cButtons, [MarshalAs(UnmanagedType.LPArray)] ThumbnailButtonCore[] pButtons);
		[PreserveSig]
		void ThumbBarSetImageList(IntPtr hwnd, IntPtr himl);
		[PreserveSig]
		void SetOverlayIcon(IntPtr hwnd, IntPtr hIcon, [MarshalAs(UnmanagedType.LPWStr)] string pszDescription);
		[PreserveSig]
		void SetThumbnailTooltip(IntPtr hwnd, [MarshalAs(UnmanagedType.LPWStr)] string pszTip);
		[PreserveSig]
		void SetThumbnailClip(IntPtr hwnd, ref Rectangle prcClip);
		[PreserveSig]
		void SetTabProperties(IntPtr hwndTab, TabPropertiesFlags stpFlags);
	}
	#endregion
	#region Enums
	public enum WindowShowCommand {
		Hide = 0,
		Normal = 1,
		Minimized = 2,
		Maximized = 3,
		ShowNoActivate = 4,
		Show = 5,
		Minimize = 6,
		ShowMinimizedNoActivate = 7,
		ShowNA = 8,
		Restore = 9,
		Default = 10,
		ForceMinimize = 11
	}
	public enum TaskbarButtonProgressMode {
		NoProgress = 0,
		Indeterminate = 0x1,
		Normal = 0x2,
		Error = 0x4,
		Paused = 0x8
	}
	public enum JumpListKnownCategoryVisibility {
		None = 0x0,
		FrequentlyUsedFiles = 0x1,
		RecentFiles = 0x2
	}
	public enum JumpListKnownCategoryPosition {
		Top,
		Bottom
	}
	public enum ThumbTabFlags {
		UseMdiThumbnail = 0x1,
		UseMdiLivePreview = 0x2
	}   
	enum HResult {
		Ok = 0x0000,
		False = 0x0001,
		InvalidArguments = unchecked((int)0x80070057),
		OutOfMemory = unchecked((int)0x8007000E),
		NoInterface = unchecked((int)0x80004002),
		Fail = unchecked((int)0x80004005),
		ElementNotFound = unchecked((int)0x80070490),
		TypeElementNotFound = unchecked((int)0x8002802B),
		NoObject = unchecked((int)0x800401E5),
		Win32ErrorCanceled = 1223,
		Canceled = unchecked((int)0x800704C7),
		ResourceInUse = unchecked((int)0x800700AA),
		AccessDenied = unchecked((int)0x80030005)
	}
	enum ThumbButtonMask {
		Bitmap = 0x1,
		Icon = 0x2,
		Tooltip = 0x4,
		Flags = 0x8
	}
	enum ThumbButtonFlags {
		Enabled = 0,
		Disabled = 0x1,
		DismissOnClick = 0x2,
		NoBackground = 0x4,
		Hidden = 0x8,
		NonInteractive = 0x10
	}
	public enum TabPropertiesFlags {
		None = 0x00000000,
		UseAppThumbnailAlways = 0x00000001,
		UseAppThumbnailWhenActive = 0x00000002,
		UseAppPeekAlways = 0x00000004,
		UseAppPeekWhenActive = 0x00000008
	}
	#endregion
	#region JumpList
	class JumpListInternal {
		public JumpListInternal() {
			list = (ICustomDestinationList)new CDestinationList();
			tasks = new List<JumpListTaskInternal>();
			categories = new List<JumpListCategoryInternal>();
		}
		ICustomDestinationList list;
		List<JumpListTaskInternal> tasks;
		List<JumpListCategoryInternal> categories;
		public void AddTask(JumpListTaskInternal task) {
			tasks.Add(task);
		}
		public void RemoveTask(JumpListTaskInternal task) {
			tasks.Remove(task);
		}
		public void AddCategory(JumpListCategoryInternal category) {
			categories.Add(category);
		}
		public void RemoveCategory(JumpListCategoryInternal category) {
			categories.Remove(category);
		}
		public void Refresh() {
			Refresh(JumpListKnownCategoryPosition.Top);
		}
		public void Refresh(JumpListKnownCategoryPosition position) {
			object removedItems;
			uint maxSlotsInList = 20;
			Guid IObjectArrayGuid = new Guid("92CA9DCD-5622-4BBA-A805-5E9F541BD8C9");
			list.BeginList(out maxSlotsInList, ref IObjectArrayGuid, out removedItems);
			if(position == JumpListKnownCategoryPosition.Top)
				AppendKnownCategory();
			RefreshTasks();
			RefreshCategories();
			if(position == JumpListKnownCategoryPosition.Bottom)
				AppendKnownCategory();
			try {
				list.CommitList();
			}
			catch {
			}
		}
		void AppendKnownCategory() {
			if(KnownCategoryToDisplay == JumpListKnownCategoryVisibility.None) return;
			list.AppendKnownCategory(KnownCategoryToDisplay);
		}
		void RefreshCategories() {
			foreach(JumpListCategoryInternal category in categories) {
				IObjectCollection taskContent = (IObjectCollection)new CEnumerableObjectCollection();
				foreach(IJumpListItemInternal item in category) {
					if(item is JumpListLinkInternal)
						taskContent.AddObject((item as JumpListLinkInternal).NativeShellLink);
				}
				HResult re = list.AppendCategory(category.Caption, (IObjectArray)taskContent);
			}
		}
		void RefreshTasks() {
			IObjectCollection taskContent = (IObjectCollection)new CEnumerableObjectCollection();
			foreach(JumpListTaskInternal task in tasks)
				taskContent.AddObject(task.NativeShellLink);
			HResult re = list.AddUserTasks((IObjectArray)taskContent);
		}
		public JumpListKnownCategoryVisibility KnownCategoryToDisplay { get; set; }
		[SecuritySafeCritical]
		public static void AddToRecent(string path) {
		   NativeMethods.SHAddToRecentDocs(NativeMethods.ShellAddToRecentDocs.PathW, path);
		}
	}
	class JumpListCategoryInternal : Collection<IJumpListItemInternal> {
		public JumpListCategoryInternal(string caption) {
			Caption = caption;
		}
		public string Caption { get; set; }
	}
	class JumpListLinkInternal : JumpListTaskInternal, IJumpListItemInternal, IDisposable {
		internal static PropertyKey PKEY_Title = new PropertyKey(new Guid("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}"), 2);
		public JumpListLinkInternal() : this(string.Empty, string.Empty) { }
		public JumpListLinkInternal(string pathValue, string titleValue) {
			Path = pathValue;
			Title = titleValue;
		}
		public string Title { get; set; }
		public string Path { get; set; }
		public IconReference IconReference { get; set; }
		public string Arguments { get; set; }
		public string WorkingDirectory { get; set; }
		public WindowShowCommand ShowCommand { get; set; }
		IPropertyStore nativePropertyStore;
		IShellLinkW nativeShellLink;
		internal override IShellLinkW NativeShellLink {
			get {
				if(nativeShellLink != null) {
					Marshal.ReleaseComObject(nativeShellLink);
					nativeShellLink = null;
				}
				nativeShellLink = (IShellLinkW)new CShellLink();
				if(nativePropertyStore != null) {
					Marshal.ReleaseComObject(nativePropertyStore);
					nativePropertyStore = null;
				}
				nativePropertyStore = (IPropertyStore)nativeShellLink;
				nativeShellLink.SetPath(Path);
				if(!string.IsNullOrEmpty(IconReference.ModuleName))
					nativeShellLink.SetIconLocation(IconReference.ModuleName, IconReference.ResourceId);
				if(!string.IsNullOrEmpty(Arguments))
					nativeShellLink.SetArguments(Arguments);
				if(!string.IsNullOrEmpty(WorkingDirectory))
					nativeShellLink.SetWorkingDirectory(WorkingDirectory);
				nativeShellLink.SetShowCmd((uint)ShowCommand);
				using(NativeMethods.PropVariant propVariant = new NativeMethods.PropVariant(Title)) {
					nativePropertyStore.SetValue(ref PKEY_Title, propVariant);
					nativePropertyStore.Commit();
				}
				return nativeShellLink;
			}
		}
		protected virtual void Dispose(bool disposing) {
			if(disposing) Title = null;
			if(nativePropertyStore != null) {
				Marshal.ReleaseComObject(nativePropertyStore);
				nativePropertyStore = null;
			}
			if(nativeShellLink != null) {
				Marshal.ReleaseComObject(nativeShellLink);
				nativeShellLink = null;
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
	class JumpListSeparatorInternal : JumpListTaskInternal, IDisposable {
		internal static PropertyKey PKEY_AppUserModel_IsDestListSeparator = IsDestinationListSeparator;
		IPropertyStore nativePropertyStore;
		IShellLinkW nativeShellLink;
		internal override IShellLinkW NativeShellLink {
			get {
				if(nativeShellLink != null) {
					Marshal.ReleaseComObject(nativeShellLink);
					nativeShellLink = null;
				}
				nativeShellLink = (IShellLinkW)new CShellLink();
				if(nativePropertyStore != null) {
					Marshal.ReleaseComObject(nativePropertyStore);
					nativePropertyStore = null;
				}
				nativePropertyStore = (IPropertyStore)nativeShellLink;
				using(NativeMethods.PropVariant propVariant = new NativeMethods.PropVariant(true)) {
					HResult result = nativePropertyStore.SetValue(ref PKEY_AppUserModel_IsDestListSeparator, propVariant);
					nativePropertyStore.Commit();
				}
				return nativeShellLink;
			}
		}
		public static PropertyKey IsDestinationListSeparator {
			get { return new PropertyKey(new Guid("{9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3}"), 6); }
		}
		protected virtual void Dispose(bool disposing) {
			if(nativePropertyStore != null) {
				Marshal.ReleaseComObject(nativePropertyStore);
				nativePropertyStore = null;
			}
			if(nativeShellLink != null) {
				Marshal.ReleaseComObject(nativeShellLink);
				nativeShellLink = null;
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
	struct IconReference {
		public IconReference(string moduleName, int resourceId)
			: this() {
			if(string.IsNullOrEmpty(moduleName)) return;
			ModuleName = moduleName;
			ResourceId = resourceId;
			referencePath = string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0},{1}", moduleName, resourceId);
		}
		public IconReference(string refPath)
			: this() {
			if(string.IsNullOrEmpty(refPath)) return;
			string[] refParams = refPath.Split(commaSeparator);
			if(refParams.Length != 2 || string.IsNullOrEmpty(refParams[0]) || string.IsNullOrEmpty(refParams[1]))
				return;
			ModuleName = refParams[0];
			ResourceId = int.Parse(refParams[1], System.Globalization.CultureInfo.InvariantCulture);
			this.referencePath = refPath;
		}
		static char[] commaSeparator = new char[] { ',' };
		public string ModuleName { get; private set; }
		public int ResourceId { get; private set; }
		string referencePath;
		public string ReferencePath {
			get { return referencePath; }
			set {
				if(string.IsNullOrEmpty(value)) return;
				string[] refParams = value.Split(commaSeparator);
				if(refParams.Length != 2 || string.IsNullOrEmpty(refParams[0]) || string.IsNullOrEmpty(refParams[1]))
					return;
				ModuleName = refParams[0];
				ResourceId = int.Parse(refParams[1], System.Globalization.CultureInfo.InvariantCulture);
				referencePath = value;
			}
		}
	}
	[StructLayout(LayoutKind.Sequential, Pack = 4), EditorBrowsable(EditorBrowsableState.Never)]
	struct PropertyKey : IEquatable<PropertyKey> {
		Guid formatId;
		Int32 propertyId;
		public Guid FormatId { get { return formatId; } }
		public Int32 PropertyId { get { return propertyId; } }
		public PropertyKey(Guid formatId, Int32 propId)
			: this() {
			this.formatId = formatId;
			this.propertyId = propId;
		}
		public PropertyKey(string formatId, Int32 propertyId)
			: this() {
			this.formatId = new Guid(formatId);
			this.propertyId = propertyId;
		}
		public bool Equals(PropertyKey other) {
			return other.Equals((object)this);
		}
		public override int GetHashCode() {
			return formatId.GetHashCode() ^ propertyId;
		}
		public override bool Equals(object obj) {
			if(obj == null) return false;
			if(!(obj is PropertyKey)) return false;
			PropertyKey other = (PropertyKey)obj;
			return other.formatId.Equals(formatId) && (other.propertyId == propertyId);
		}
		public static bool operator ==(PropertyKey propKey1, PropertyKey propKey2) {
			return propKey1.Equals(propKey2);
		}
		public static bool operator !=(PropertyKey propKey1, PropertyKey propKey2) {
			return !propKey1.Equals(propKey2);
		}
	}  
	interface IJumpListItemInternal {
		string Path { get; set; }
	}
	abstract class JumpListTaskInternal {
		internal abstract IShellLinkW NativeShellLink { get; }
	}
	#endregion
	#region Manager
	class TaskbarManager {
		static object syncLock = new object();
		static ITaskbarList4 taskbarList;
		internal static ITaskbarList4 Instance {
			get {
				if(taskbarList == null) {
					lock(syncLock) {
						if(taskbarList == null) {
							taskbarList = (ITaskbarList4)new CTaskbarList();
							taskbarList.HrInit();
						}
					}
				}
				return taskbarList;
			}
		}
	}
	public static class TaskbarAssistantCore {
		internal static readonly int WM_TaskbarButtonCreated = NativeMethods.RegisterWindowMessage("TaskbarButtonCreated");
		internal static readonly int WM_TaskbarRefreshJumpList = NativeMethods.RegisterWindowMessage("TaskbarAssistantJumpListRefresh");
		internal static readonly string AP_Application = "AppTaskbarAssistant";
		internal static readonly string AP_ApplicationSpaceReplacement = "?";
		internal static readonly string AP_JumpListItemEmptyTask = AP_Application + "JumpListEmptyTask";
		internal static readonly string AP_JumpListItemClick = AP_Application + "JumpListItemClick_";
		internal static readonly string AP_JumpListItemClickArgumentSeparator = ":";
		internal static readonly string AP_JumpListItemTask = AP_Application + "JumpListItemTask_";
		internal static readonly string AP_JumpListItemTaskArguments = AP_Application + "JumpListItemTaskArguments_";
		internal static readonly string AP_JumpListItemTaskArgumentsSeparator = "|";
		public static void Initialize() {
			string[] args = Environment.GetCommandLineArgs();
			if(args.All(x => !x.Contains(AP_Application))) return;
			foreach(string arg in args) {
				if(!arg.Contains(AP_Application)) continue;
				if(arg.Contains(AP_JumpListItemEmptyTask)) ProcessEmptyTask();
				if(arg.Contains(AP_JumpListItemTask)) ProcessTask(arg);
				if(arg.Contains(AP_JumpListItemClick)) ProcessClick(arg);
			}
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
		internal static IntPtr ActiveWindowHandle { get; set; }
		internal static IEnumerable<string> GetJumpListItemArguments(JumpListItemTask item) {
			yield return AP_Application;
			if(item.IsEmptyTask) {
				yield return AP_JumpListItemEmptyTask;
			}
			else {
				yield return CalcTaskArgument(item);
				yield return CalcClickArgument(item);
			}
		}
		internal static IntPtr GetHandle(string argument) {
			string[] args = argument.Replace(AP_JumpListItemClick, string.Empty).Split(AP_JumpListItemClickArgumentSeparator.ToCharArray(0, 1));
			int r = 0;
			return int.TryParse(args[0], out r) ? new IntPtr(r) : IntPtr.Zero;
		}
		internal static int GetMessage(string argument) {
			string[] args = argument.Replace(AP_JumpListItemClick, string.Empty).Split(AP_JumpListItemClickArgumentSeparator.ToCharArray(0, 1));
			int r = 0;
			return int.TryParse(args[1], out r) ? r : 0;
		}
		static Dictionary<int, ISupportJumpListItemClick> windowMessages;
		internal static Dictionary<int, ISupportJumpListItemClick> WindowMessages {
			get { return windowMessages ?? (windowMessages = new Dictionary<int, ISupportJumpListItemClick>()); }
		}
		static string CalcTaskArgument(JumpListItemTask item) {
			string arg = string.Empty;
			if(string.IsNullOrEmpty(item.Path)) return arg;
			arg += AP_JumpListItemTask + item.Path.Replace(" ", AP_ApplicationSpaceReplacement);
			if(string.IsNullOrEmpty(item.Arguments)) return arg;
			return arg += AP_JumpListItemTaskArguments + item.Arguments.Replace(" ", AP_JumpListItemTaskArgumentsSeparator);
		}
		static string CalcClickArgument(JumpListItemTask item) {
			if(item.click == null) return string.Empty;
			return AP_JumpListItemClick + ActiveWindowHandle.ToString() + AP_JumpListItemClickArgumentSeparator + item.message;
		}
		static void ProcessEmptyTask() {
			System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
		static void ProcessTask(string arg) {
			arg = arg.Replace(AP_JumpListItemTask, " ");
			arg = arg.Replace(AP_JumpListItemTaskArguments, " ");
			string[] split = { " " };
			string[] values = arg.Split(split, StringSplitOptions.RemoveEmptyEntries);
			if(values.Length == 0) return;
			string taskPath = values[0].Replace(AP_ApplicationSpaceReplacement, " ");
			if(values.Length == 1) {
				System.Diagnostics.Process.Start(taskPath);
				return;
			}
			string taskArguments = values[1].Replace(AP_JumpListItemTaskArgumentsSeparator, " ");
			System.Diagnostics.Process.Start(new ProcessStartInfo(taskPath, taskArguments));
		}
		static void ProcessClick(string arg) {
			int message = TaskbarAssistantCore.GetMessage(arg);
			IntPtr handle = TaskbarAssistantCore.GetHandle(arg);
			NativeMethods.PostMessage(handle, message, IntPtr.Zero, IntPtr.Zero);
		}
	}
	public class TaskbarHelper {
		static TabbedThumbnailCollection tabCollection = new TabbedThumbnailCollection();
		public static void SetOverlayIcon(IntPtr handle, Bitmap bitmap, string accessibilityText) {
			if(!SupportTaskbarFeatures) return;
			IntPtr overlayIconHandle = bitmap != null ? bitmap.GetHicon() : IntPtr.Zero;
			try {
				TaskbarManager.Instance.SetOverlayIcon(handle, overlayIconHandle, accessibilityText);
			}
			finally { NativeMethods.DestroyIcon(overlayIconHandle); }
		}
		public static void SetProgressValue(IntPtr handle, long currentValue, long maximumValue) {
			SetProgressValue(handle, Convert.ToUInt64(currentValue), Convert.ToUInt64(maximumValue));
		}
		[CLSCompliant(false)]
		public static void SetProgressValue(IntPtr handle, ulong currentValue, ulong maximumValue) {
			if(!SupportTaskbarFeatures) return;
			TaskbarManager.Instance.SetProgressValue(handle, currentValue, maximumValue);
		}
		public static void SetProgressState(IntPtr handle, TaskbarButtonProgressMode state) {
			if(!SupportTaskbarFeatures) return;
			TaskbarManager.Instance.SetProgressState(handle, state);
		}
		public static void SetTaskbarButtons(IntPtr handle, Collection<ThumbnailButton> buttons) {
			if(!SupportTaskbarFeatures) return;
			uint count = (uint)(buttons.Count > 7 ? 7 : buttons.Count);
			if(count == 0) return;
			try {
				using(TaskbarButtonsContext context = new TaskbarButtonsContext(buttons))
					TaskbarManager.Instance.ThumbBarAddButtons(handle, count, context.Buttons);
			}
			catch { }
		}
		public static void UpdateTaskbarButtons(IntPtr handle, Collection<ThumbnailButton> buttons) {
			if(!SupportTaskbarFeatures) return;
			uint count = (uint)(buttons.Count > 7 ? 7 : buttons.Count);
			if(count == 0) return;
			try {
				using(TaskbarButtonsContext context = new TaskbarButtonsContext(buttons))
					TaskbarManager.Instance.ThumbBarUpdateButtons(handle, count, context.Buttons);
			}
			catch { }
		}
		public static void SetThumbnailClip(IntPtr handle, ref Rectangle rectangle) {
			if(!SupportTaskbarFeatures) return;
			TaskbarManager.Instance.SetThumbnailClip(handle, ref rectangle);
		}
		public static void AddTab(IntPtr handle) {
			if(!SupportTaskbarFeatures) return;
			TaskbarManager.Instance.AddTab(handle);
		}
		public static void DeleteTab(IntPtr handle) {
			if(!SupportTaskbarFeatures) return;
			TaskbarManager.Instance.DeleteTab(handle);
		}
		public static void SetThumbnailTooltip(IntPtr handle, string caption) {
			if(!SupportTaskbarFeatures) return;
			TaskbarManager.Instance.SetThumbnailTooltip(handle, caption);
		}
		public static void RegisterTab(IntPtr handleTab, IntPtr handleMDI) {
			if(!SupportTaskbarFeatures || handleTab == IntPtr.Zero) return;
			TaskbarManager.Instance.RegisterTab(handleTab, handleMDI);
		}
		public static void UnregisterTab(IntPtr handleTab) {
			if(!SupportTaskbarFeatures || handleTab == IntPtr.Zero) return;
			TaskbarManager.Instance.UnregisterTab(handleTab);
		}
		public static void ActivateTab(IntPtr preview) {
			if(!SupportTaskbarFeatures) return;
			TaskbarManager.Instance.ActivateTab(preview);
		}
		public static void SetTabOrder(IntPtr handleTab, IntPtr handleInsertBefore) {
			if(!SupportTaskbarFeatures) return;
			TaskbarManager.Instance.SetTabOrder(handleTab, handleInsertBefore);
		}
		public static void SetTabActive(IntPtr handleTab, IntPtr handleMDI, ThumbTabFlags tabFlags) {
			if(!SupportTaskbarFeatures) return;
			TaskbarManager.Instance.SetTabActive(handleTab, handleMDI, tabFlags);
		}
		public static void SetTabProperties(IntPtr handleTab, TabPropertiesFlags stpFlags) {
			if(!SupportTaskbarFeatures) return;
			TaskbarManager.Instance.SetTabProperties(handleTab, stpFlags);
		}
		static void AddThumbnailTab(BaseTabbedThumbnail tab) {
			if(!SupportTaskbarFeatures) return;
			Form f = tab.GetRootForm();
			if(f == null || !f.IsHandleCreated) {
				tab.Dispose();
			}
			else {
				tabCollection.Add(tab);
				RegisterTab(tab.Handle, f.Handle);
				SetTabOrder(tab.Handle, IntPtr.Zero);
				ActivateTab(tab.Handle);
			}
		}
		public static void AddThumbnailTab(IntPtr handleTab, IntPtr handleMDI) {
			if(!SupportTaskbarFeatures) return;
			Form form = Control.FromHandle(handleMDI) as Form;
			AddThumbnailTab(Control.FromHandle(handleTab), form);  
		}
		public static void AddThumbnailTab(IThumbnailManagerProvider component, IntPtr handleTab) {
			if(!SupportTaskbarFeatures) return;
			AddThumbnailTab(component, Control.FromHandle(handleTab));			
		}
		public static void AddThumbnailTab(Control ownerControl, Form rootForm) {
			if(!SupportTaskbarFeatures || ownerControl == null || rootForm == null) return;
			AddThumbnailTab(new TabbedThumbnail(rootForm, ownerControl));
		}
		public static void AddThumbnailTab(IThumbnailManagerProvider component, Control ownerControl) {
			if(!SupportTaskbarFeatures || ownerControl == null) return;
			AddThumbnailTab(new TabbedThumbnailComponent(component, ownerControl));
		}
		public static void RemoveThumbnailTab(IntPtr handleTab) {
			if(!SupportTaskbarFeatures) return;
			Control control = Control.FromHandle(handleTab);
			RemoveThumbnailTab(control);
		}
		public static void RemoveThumbnailTab(Control ownerControl) {
			if(!SupportTaskbarFeatures || ownerControl == null) return;			
			BaseTabbedThumbnail tab = tabCollection[ownerControl];
			if(tab == null) return;			
			if(tab.IsHandleCreated)
				UnregisterTab(tab.Handle);
			tabCollection.Remove(tab);
			tab.Dispose();
		}
		static List<ThumbnailButtonCore> ConvertButtons(Collection<ThumbnailButton> collection) {
			List<ThumbnailButtonCore> list = new List<ThumbnailButtonCore>();
			foreach(ThumbnailButton button in collection)
				list.Add(button.ThumbButtonCore);
			return list;
		}
		static bool Contains(IEnumerable<IntPtr> collection, IntPtr target) {
			foreach(IntPtr intPtr in collection)
				if(target == intPtr) return true;
			return false;
		}
		public static void UpdateSettingsTabbedThumbnails() {
			if(tabCollection == null || !SupportTaskbarFeatures) return;
			foreach(BaseTabbedThumbnail tab in tabCollection)
				tab.UpdateTabbedThumbnail();
		}
		internal static bool SupportTaskbarFeatures {
			get { return SupportOSVersion && IsTaskbarVisible; }
		}
		static Version supportOS = new Version(6, 1);
		internal static bool SupportOSVersion { 
			get { return Environment.OSVersion.Version >= supportOS; } 
		}
		internal static bool IsTaskbarVisible {
			get {
				NativeMethods.RECT rect = new NativeMethods.RECT();
				NativeMethods.GetWindowRect(NativeMethods.FindWindow("Shell_TrayWnd", null), ref rect);
				return rect.Left != 0 || rect.Top != 0 || rect.Right != 0 || rect.Bottom != 0;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static void InitDemoJumpList(TaskbarAssistant assistant, Control parent) {
			assistant.Assign(parent);
			assistant.BeginUpdate();
			JumpListCategory categoryUISuperhero = new JumpListCategory("Become a UI Superhero");
			categoryUISuperhero.JumpItems.Add(CreateDemoTask("Explore DevExpress Universal", linkExploreDevExpressUniversal, "Universal.ico", 0));
			categoryUISuperhero.JumpItems.Add(CreateDemoTask("Online Tutorials", linkOnlineTutorial, "Win.ico", 0));
			categoryUISuperhero.JumpItems.Add(CreateDemoTask("Buy Now", linkBuyNow, "DevExpress.ico", 0));
			assistant.JumpListCustomCategories.Add(categoryUISuperhero);
			assistant.EndUpdate();
		}
		static string linkExploreDevExpressUniversal = "https://www.devexpress.com/Subscriptions/Universal.xml";
		static string linkOnlineTutorial = "https://www.devexpress.com/Products/NET/Controls/WinForms/get-started.xml";
		static string linkBuyNow = "https://www.devexpress.com/Subscriptions/";
		static JumpListItemTask CreateDemoTask(string caption, string path, string iconName, int iconIndex) {
			JumpListItemTask task = new JumpListItemTask(caption);
			task.Path = path;
			task.IconPath = GetIconPath(iconName);
			task.IconIndex = iconIndex;
			return task;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetIconPath(string iconName) {
			string directory = Path.GetDirectoryName(Application.ExecutablePath);
			string iconDirectory = Path.Combine(directory, "Icons");
			string iconPath = Path.Combine(iconDirectory, iconName);
			if(File.Exists(iconPath))
				return iconPath;
			return Path.Combine(directory, iconName);
		}
		class TaskbarButtonsContext : IDisposable {
			bool isDisposing;
			List<ThumbnailButtonCore> buttonCollection;
			public TaskbarButtonsContext(Collection<ThumbnailButton> buttons) {
				buttonCollection = ConvertButtons(buttons);
			}
			public ThumbnailButtonCore[] Buttons {
				get {
					if(buttonCollection == null) return null;
					return buttonCollection.ToArray();
				}
			}
			void IDisposable.Dispose() {
				if(!isDisposing) {
					isDisposing = true;
					foreach(ThumbnailButtonCore button in buttonCollection)
						button.Dispose();
					buttonCollection.Clear();
					buttonCollection = null;
				}
			}
		}
	}
	#endregion
	#region ThumbnailButton
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal struct ThumbnailButtonCore : IDisposable {
		internal const int Click = 0x1800;
		[MarshalAs(UnmanagedType.U4)]
		public ThumbButtonMask Mask;
		public int Id;
		public int Bitmap;
		IntPtr icon;
		public IntPtr Icon {
			get { return icon; }
			set {
				if(Icon == value) return;				
				icon = value;
			}
		}
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
		public string Tip;
		[MarshalAs(UnmanagedType.U4)]
		public ThumbButtonFlags Flags;
		#region IDisposable Members
		public void Dispose() {
			if(icon == IntPtr.Zero) return;
			NativeMethods.DestroyIcon(icon);
		}
		#endregion
	}
	#endregion   
	public enum DrawingOptions {
		PRF_CHECKVISIBLE = 0x00000001,
		PRF_NONCLIENT = 0x00000002,
		PRF_CLIENT = 0x00000004,
		PRF_ERASEBKGND = 0x00000008,
		PRF_CHILDREN = 0x00000010,
		PRF_OWNED = 0x00000020
	}   
	public interface IThumbnailManagerProvider {
		void Activate(Control control);
		void Close(Control control);
		Rectangle GetScreenBounds(Control control);
		Form GetRootForm(Control control);
		string GetCaption(Control control);
		Icon GetIcon(Control control);
		void Update(Control control);
	}
	class ActiveXThumbnailCache : IDisposable {
		bool isDisposing;
		Control activeXControl;
		Bitmap screenShotCore;
		public ActiveXThumbnailCache(Control activeXControl) {
			screenShotCore = ScreenCaptureHelper.GetImageFromActiveXControl(activeXControl.Handle, activeXControl.Size);
			this.activeXControl = activeXControl;
		}
		public Bitmap ScreenShot { get { return screenShotCore; } }
		public Point Location { get { return activeXControl.Location; } }
		public Size Size { get { return activeXControl.Size; } }
		public void Dispose() {
			if(isDisposing) return;
			isDisposing = true;
			Ref.Dispose(ref screenShotCore);
			activeXControl = null;
		}
	}
	class TabbedThumbnailContext : IDisposable {
		Control controlCore;
		Rectangle boundsCore;
		Form formCore;
		public TabbedThumbnailContext(Control control, Form form, Rectangle bounds) {
			controlCore = control;
			boundsCore = bounds;
			formCore = form;
		}
		public Control Control { get { return controlCore; } }
		public Form Form { get { return formCore; } }
		public Rectangle Bounds { get { return boundsCore; } }
		void IDisposable.Dispose() {
			controlCore = null;
			boundsCore = Rectangle.Empty;
			formCore = null;
		}
	}
	class TabbedThumbnailCache : IDisposable {
		Bitmap livePreviewCore;
		List<ActiveXThumbnailCache> activeXCacheCollection;
		bool isDisposing;
		Point locationCore;
		bool isReady;
		public TabbedThumbnailCache() {
			activeXCacheCollection = new List<ActiveXThumbnailCache>();
		}
		public void SetDirty() { isReady = false; }
		public void Update(TabbedThumbnailContext context) {
			if(IsDisposing || isReady) return;
			locationCore = CalcLocation(context.Control, context.Form, context.Bounds);
			UpdateActiveXCacheCollection(context.Control);
			Ref.Dispose(ref livePreviewCore);
			livePreviewCore = PrintControl(context.Control);
			isReady = true;
		}
		void UpdateActiveXCacheCollection(Control control) {
			if(!ActiveXControlLocationIsValid(control)) return;
			ClearActiveXCacheCollection();
			GenerateActiveXCacheCollection(control); 
		}
		public bool IsDisposing { get { return isDisposing; } }
		public Point Location { get { return locationCore; } }
		public Bitmap LivePreview { get { return livePreviewCore; } }		 
		bool CheckCustomDrawNonClientArea(Form form) {
			DevExpress.XtraEditors.ICustomDrawNonClientArea nonClientArea = form as DevExpress.XtraEditors.ICustomDrawNonClientArea;
			if(nonClientArea == null) return false;
			return nonClientArea.IsCustomDrawNonClientArea;
		}
		bool CheckGlassForm(Form form) {
			DevExpress.XtraEditors.IGlassForm glassForm = form as DevExpress.XtraEditors.IGlassForm;
			if(glassForm == null) return false;
			return glassForm.IsGlassForm;
		}
		Point CalcLocation(Control control, Form form, Rectangle bounds) {
			Point realLocation = form.PointToClient(bounds.Location);
			if(CheckCustomDrawNonClientArea(form) && !CheckGlassForm(form)) {
				NativeMethods.RECT formBounds = new NativeMethods.RECT();
				NativeMethods.GetWindowRect(form.Handle, ref formBounds);
				NativeMethods.RECT client;
				NativeMethods.GetClientRect(form.Handle, out client);
				Point formLocation = formBounds.ToRectangle().Location;
				Point clientLocation = form.PointToScreen(client.ToRectangle().Location);
				Point offset = new Point(clientLocation.X - formLocation.X, clientLocation.Y - formLocation.Y);
				realLocation.Offset(offset);
			}
			return realLocation;
		}
		void GenerateActiveXCacheCollection(Control control) {
			if(ScreenCaptureHelper.IsActiveXControl(control))
				activeXCacheCollection.Add(new ActiveXThumbnailCache(control));
			else {
				foreach(Control childControl in control.Controls)
					GenerateActiveXCacheCollection(childControl);
			}
		}
		Bitmap PrintControl(Control control) {
			Bitmap bmp = ScreenCaptureHelper.GetRecursiveImageFromControl(control) as Bitmap;
			if(bmp == null) return null;			
			if(activeXCacheCollection == null) return bmp;
			using(Graphics g = Graphics.FromImage(bmp)) {
				foreach(var im in activeXCacheCollection)
					g.DrawImage(im.ScreenShot, new Rectangle(im.Location, im.Size), new Rectangle(new Point(0, 0), im.ScreenShot.Size), GraphicsUnit.Pixel);
			}
			return bmp;
		}
		void ClearActiveXCacheCollection() {
			if(activeXCacheCollection != null) {
				foreach(var activeX in activeXCacheCollection)
					activeX.Dispose();
				activeXCacheCollection.Clear();
			}
		}
		protected virtual bool ActiveXControlLocationIsValid(Control control) {
			if(control == null) return true;
			Rectangle bounds = Screen.FromControl(control).Bounds;
			Rectangle realBounds = ScreenCaptureHelper.CalcBoundsWindow(control.Handle);
			if(Rectangle.Intersect(bounds, realBounds) == Rectangle.Empty)
				return false;
			if(control.Parent != null) {
				Rectangle parentBouds = ScreenCaptureHelper.CalcBoundsWindow(control.Parent.Handle);
				if(Rectangle.Intersect(parentBouds, realBounds) == Rectangle.Empty)
					return false;
			}
			return ActiveXControlLocationIsValid(control.Parent);
		}
		public void Dispose() {
			if(isDisposing) return;
			isDisposing = true;
			Ref.Dispose(ref livePreviewCore);
			ClearActiveXCacheCollection();
			activeXCacheCollection = null;
		}
	}
	class BaseTabbedThumbnail : Form {
		const int DWMWA_FORCE_ICONIC_REPRESENTATION = 7;
		const int DWMWA_HAS_ICONIC_BITMAP = 10;
		const uint WmDwmSendIconicThumbnail = 0x0323;
		const uint WmDwmSendIconicLivePreviewBitmap = 0x0326;
		const int cbAttribute = 4;
		TabbedThumbnailCache tabbedThumbnailCache;
		WeakReference weakControlCore;
		bool isDisposing;
		public BaseTabbedThumbnail(Control ownerControl)
			: base() {
			weakControlCore = new WeakReference(ownerControl);
			tabbedThumbnailCache = new TabbedThumbnailCache();
			InternalEnableCustomWindowPreview(true);
		}
		protected void Initialize() {
			if(!CheckOwnerControl()) return;
			using(Icon iconCore = GetIcon()) {
				if(iconCore != null) {
					Icon = iconCore;
					DevExpress.Utils.Drawing.Helpers.NativeMethods.DestroyIcon(iconCore.Handle);
				}
			}
			Text = GetCaption();
			PatchSystemMenu();
		}
		protected virtual void PatchSystemMenu() {
			if(!IsHandleCreated) return;
			try {
				IntPtr sysMenu = NativeMethods.GetSystemMenu(this.Handle, false);
				if(sysMenu == IntPtr.Zero) return;
				int itemCount = NativeMethods.GetMenuItemCount(sysMenu);
				if(itemCount <= 0) return;
				NativeMethods.MENUITEMINFO itemInfo = new NativeMethods.MENUITEMINFO();
				itemInfo.cbSize = Marshal.SizeOf(typeof(NativeMethods.MENUITEMINFO));
				itemInfo.fMask = NativeMethods.MenuFlags.MIIM_ID;
				for(int i = 0; i < itemCount; i++) {
					NativeMethods.GetMenuItemInfo(sysMenu, i, true, ref itemInfo);
					if(CanRemoveMenuItem(itemInfo.wID))
						NativeMethods.RemoveMenu(sysMenu, itemInfo.wID, NativeMethods.MenuFlags.MF_BYCOMMAND | NativeMethods.MenuFlags.MF_REMOVE);
				}
			}
			finally { }
		}
		protected virtual bool CanRemoveMenuItem(int id) { return false; }
		protected void SetDitryCache() {
			if(tabbedThumbnailCache == null || tabbedThumbnailCache.IsDisposing) return;
			tabbedThumbnailCache.SetDirty();
		}
		protected void UpdateCache() {
			if(tabbedThumbnailCache == null || tabbedThumbnailCache.IsDisposing) return;
			using(TabbedThumbnailContext context = CreateContext(OwnerControl, GetRootForm(), GetOwnerControlBounds())) {
				if(CanUpdateCache(context))
					tabbedThumbnailCache.Update(context);
			}
		}
		[SecuritySafeCritical]
		void InternalEnableCustomWindowPreview(bool enable) {
			IntPtr t = Marshal.AllocHGlobal(cbAttribute);
			Marshal.WriteInt32(t, enable ? 1 : 0);
			try {
				DwmSetWindowAttribute(this.Handle, DWMWA_HAS_ICONIC_BITMAP, t, cbAttribute);
				DwmSetWindowAttribute(this.Handle, DWMWA_FORCE_ICONIC_REPRESENTATION, t, cbAttribute);
			}
			finally {
				Marshal.FreeHGlobal(t);
			}
		}
		[SecuritySafeCritical]
		void DwmSetWindowAttribute(IntPtr hwnd, int dwAttributeToSet, IntPtr pvAttributeValue, int cbAttribute) {
			int rc = NativeMethods.DwmSetWindowAttribute(hwnd, dwAttributeToSet, pvAttributeValue, cbAttribute);
			if(rc != 0) throw Marshal.GetExceptionForHR(rc);
		}
		[SecuritySafeCritical]
		void SetIconicLivePreviewBitmap() {
			UpdateCache();
			if(tabbedThumbnailCache.LivePreview == null) return;
			IntPtr hbitmap = tabbedThumbnailCache.LivePreview.GetHbitmap();
			IntPtr ptr = IntPtr.Zero;
			try {
				int size = Marshal.SizeOf(typeof(Point));
				ptr = Marshal.AllocHGlobal(size);
				Marshal.StructureToPtr(tabbedThumbnailCache.Location, ptr, false);
				NativeMethods.DwmSetIconicLivePreviewBitmap(Handle, hbitmap, ptr, 0x00000000);
			}
			finally {
				Marshal.FreeHGlobal(ptr);
				NativeMethods.DeleteObject(hbitmap);
			}
		}
		void SetIconicThumbnail(int width, int height) {
			UpdateCache();
			if(tabbedThumbnailCache.LivePreview == null) return;
			Bitmap image = ScreenCaptureHelper.ResizeImageWithAspect(tabbedThumbnailCache.LivePreview, width, height, true);
			IntPtr hbitmap = image.GetHbitmap();
			try { NativeMethods.DwmSetIconicThumbnail(Handle, hbitmap, 0x00000001); }
			finally {
				NativeMethods.DeleteObject(hbitmap);
				Ref.Dispose(ref image);
			}
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == 0x031f) Initialize();
			if(m.Msg == WmDwmSendIconicThumbnail) {
				int width = (int)(((long)m.LParam) >> 16);
				int height = (int)(((long)m.LParam) & (0xFFFF));
				SetIconicThumbnail(width, height);
			}
			if(m.Msg == WmDwmSendIconicLivePreviewBitmap)
				SetIconicLivePreviewBitmap();
			if(m.Msg == MSG.WM_ACTIVATE && m.WParam != IntPtr.Zero)
				Activate(m);
			if(m.Msg == 0x10)
				ThumbnailTabClose();
			if(m.Msg == MSG.WM_SYSCOMMAND) {
				int command = FormShadow.Helpers.NativeHelper.LOWORD(m.HWnd);
				m.Result = OnCommand(command & 0xfff0);
			}
			base.WndProc(ref m);
		}
		protected virtual IntPtr OnCommand(int command) { return IntPtr.Zero; }
		protected virtual void ControlRefresh() { OwnerControl.Update(); }
		public void UpdateTabbedThumbnail() {
			if(!CheckOwnerControl()) return;
			ControlRefresh();
			Initialize();
			NativeMethods.DwmInvalidateIconicBitmaps(Handle);
			SetDitryCache();
			UpdateCache();
			SetDitryCache();
		}
		public Form GetRootForm() {
			if(!CheckOwnerControl()) return null;
			return GetRootFormCore();
		}
		void ThumbnailTabClose() {
			if(!CheckOwnerControl()) return;
			this.Close(OwnerControl);
			this.Dispose();
		}
		protected virtual void ActivateOwnerControl(Message m) {
			NativeMethods.SendMessage(OwnerControl.Handle, m.Msg, m.WParam, m.LParam);
		}
		void Activate(Message m) {
			Form form = GetRootForm();
			if(form == null || !form.IsHandleCreated || m.LParam != IntPtr.Zero) return;
			int fMinimize = FormShadow.Helpers.NativeHelper.HIWORD(m.WParam);
			if(fMinimize > 0 || form.WindowState == FormWindowState.Minimized) NativeMethods.ShowWindow(form.Handle, 0x09);
			if(!CheckOwnerControl()) return;
			ActivateOwnerControl(m);
		}
		public Control OwnerControl { get { return weakControlCore.Target as Control; } }
		protected virtual TabbedThumbnailContext CreateContext(Control control, Form form, Rectangle bounds) {
			return new TabbedThumbnailContext(control, form, bounds);
		}
		protected virtual bool CheckOwnerControl() { return OwnerControl != null && OwnerControl.IsHandleCreated; }
		protected virtual bool CanUpdateCache(TabbedThumbnailContext context) {
			if(!CheckOwnerControl()) return false;
			if(context.Form == null || !context.Form.IsHandleCreated) return false;
			Rectangle bounds = ScreenCaptureHelper.CalcBoundsWindow(OwnerControl.Handle);
			return context.Form.WindowState != FormWindowState.Minimized && context.Form.Visible == true && bounds.Width > 0 && bounds.Height > 0;
		}
		protected virtual void Close(Control control) { }
		protected virtual Icon GetIcon() {
			Form form = GetRootForm();
			if(form == null || !form.IsHandleCreated) return null;
			return form.Icon.Clone() as Icon;
		}
		protected virtual Form GetRootFormCore() { return OwnerControl.FindForm(); }
		protected virtual Rectangle GetOwnerControlBounds() { return ScreenCaptureHelper.CalcBoundsWindow(OwnerControl.Handle); }
		protected virtual string GetCaption() { return OwnerControl.Text; }
		public virtual bool CompareTo(Control control) { return OwnerControl == control; }
		protected override void Dispose(bool disposing) {
			if(isDisposing) return;
			isDisposing = disposing;
			if(OwnerControl != null)
				TaskbarHelper.RemoveThumbnailTab(OwnerControl);
			this.weakControlCore = null;
			Ref.Dispose(ref tabbedThumbnailCache);
			base.Dispose(disposing);
		}
	}
	class TabbedThumbnailComponent : BaseTabbedThumbnail {
		IThumbnailManagerProvider provider;
		public TabbedThumbnailComponent(IThumbnailManagerProvider provider, Control ownerControl)
			: base(ownerControl) {
			this.provider = provider;			
		}
		protected override bool CanUpdateCache(TabbedThumbnailContext context) {
			if(provider == null) return false;
			return base.CanUpdateCache(context);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			provider = null;
		}
		protected override bool CanRemoveMenuItem(int id) {
			if(id != NativeMethods.SC.SC_CLOSE) return true;
			return base.CanRemoveMenuItem(id);
		}
		protected override void ActivateOwnerControl(Message m) { provider.Activate(OwnerControl); }
		protected override void Close(Control control) { provider.Close(control); }
		protected override Rectangle GetOwnerControlBounds() { return provider.GetScreenBounds(OwnerControl); }
		protected override string GetCaption() { return provider.GetCaption(OwnerControl); }
		protected override Form GetRootFormCore() { return provider.GetRootForm(OwnerControl); }
		protected override void ControlRefresh() {
			if(provider == null || !CheckOwnerControl()) return;
			provider.Update(OwnerControl);
		}
		protected override Icon GetIcon() {
			Icon icon = provider.GetIcon(OwnerControl);
			if(icon == null) return base.GetIcon();
			return icon;
		}
	}
	class TabbedThumbnail : BaseTabbedThumbnail {
		Form rootFormCore;
		public TabbedThumbnail(Form rootForm, Control ownerControl)
			: base(ownerControl) {
				this.rootFormCore = rootForm;
		}
		protected override void Dispose(bool disposing) {			
			base.Dispose(disposing);
			rootFormCore = null;			
		}
		protected override Form GetRootFormCore() { return rootFormCore; }		
	}
	class TabbedThumbnailCollection : CollectionBase {
		public TabbedThumbnailCollection() { }
		public virtual bool Add(BaseTabbedThumbnail tabbedThumbnail) {
			if(List.Contains(tabbedThumbnail)) return false;
			List.Add(tabbedThumbnail);
			return true;
		}
		public BaseTabbedThumbnail this[Control control] {
			get {
				foreach(BaseTabbedThumbnail tabbedThumbnail in List) {
					if(tabbedThumbnail.CompareTo(control)) return tabbedThumbnail;
				}
				return null;
			}
		}	  
		public virtual void Insert(int index, BaseTabbedThumbnail tabbedThumbnail) {
			if(List.Contains(tabbedThumbnail)) List.Remove(tabbedThumbnail);
			List.Insert(index, tabbedThumbnail);
		}
		public virtual void Remove(BaseTabbedThumbnail tabbedThumbnail) {
			List.Remove(tabbedThumbnail);
		}
		public virtual BaseTabbedThumbnail[] ToArray() {
			BaseTabbedThumbnail[] tabs = new BaseTabbedThumbnail[List.Count];
			List.CopyTo(tabs, 0);
			return tabs;
		}
	}  
	static class IconExtractor {
		static List<Icon> icons;
		[SecuritySafeCritical]
		public static List<Icon> ExtractFromAssembly(string source, IconSize size) {
			icons = new List<Icon>();
			IntPtr[] nullPtr = null;
			int count = NativeMethods.ExtractIconEx(source, -1, nullPtr, nullPtr, 1);
			IntPtr[] largeIcons = size == IconSize.Large ? new IntPtr[count] : null;
			IntPtr[] smallIcons = size == IconSize.Small ? new IntPtr[count] : null;
			NativeMethods.ExtractIconEx(source, 0, largeIcons, smallIcons, count);
			IntPtr[] extractedIcons = largeIcons ?? smallIcons;
			foreach(IntPtr handle in extractedIcons) {
				if(handle != IntPtr.Zero) {
					icons.Add((Icon)Icon.FromHandle(handle).Clone());
					NativeMethods.DestroyIcon(handle);
				}
			}
			return icons;
		}
	}
	enum IconSize {
		Large,
		Small
	}
	class DefaultTaskbarProperties {
		internal const long CurrentProgressValue = 0;
		internal const long MaximumProgressValue = 100;
		internal const Image OverlayIcon = null;
		internal const string IconsAssembly = "";
		internal const TaskbarButtonProgressMode ProgressState = TaskbarButtonProgressMode.NoProgress;
		internal const JumpListKnownCategoryVisibility DefaultJumpListKnownCategoryVisibility = JumpListKnownCategoryVisibility.None;
		internal const JumpListKnownCategoryPosition DefaultJumpListKnownCategoryPosition = JumpListKnownCategoryPosition.Top;
		internal static readonly Rectangle ThumbnailClip = Rectangle.Empty;
	}
	class IconsAssemblyConverter : EnumConverter {
		public IconsAssemblyConverter() : base(typeof(string)) { }
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
			if(sourceType == typeof(string))
				return true;
			return base.CanConvertFrom(context, sourceType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			if(value is string)
				return value;
			return base.ConvertFrom(context, culture, value);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string))
				return (string)value;
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			TaskbarAssistant prop = context.Instance as TaskbarAssistant;
			List<string> list = new List<string>();
			string projectPath = prop.DesignTimeManager.GetProjectPath();
			string[] files = Directory.GetFiles(projectPath, "*.*", SearchOption.AllDirectories);
			foreach(string file in files) {
				string ext = Path.GetExtension(file);
				if(ext == ".dll" || ext == ".exe" || ext == ".ico")
					list.Add(file.Replace(projectPath, ""));
			}
			return new StandardValuesCollection(list);
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
	[ToolboxItem(false)]
	public class FilterWindow : NativeWindow {
		[SecurityCritical]
		public FilterWindow(TaskbarAssistant assistant) {
			Assistant = assistant;
		}
		TaskbarAssistant Assistant { get; set; }
		protected override void WndProc(ref Message m) {
			if(TaskbarAssistantCore.WindowMessages.ContainsKey(m.Msg))
				DispatchJumpListItemMessage(m.Msg);
			if(Assistant.IsParentHandleCreated)
				DispatchThumbnailButtonMessage(ref m, Assistant.ThumbnailButtons, Assistant.ParentHandle);
			if((int)m.Msg == TaskbarAssistantCore.WM_TaskbarButtonCreated)
				Assistant.Refresh();
			if((int)m.Msg == TaskbarAssistantCore.WM_TaskbarRefreshJumpList)
				Assistant.OnJumpListChanged(true);
			base.WndProc(ref m);
		}
		protected virtual void DispatchJumpListItemMessage(int msg) {
			ISupportJumpListItemClick item = TaskbarAssistantCore.WindowMessages[msg];
			if(item != null) item.RaiseClick();
		}
		protected virtual void DispatchThumbnailButtonMessage(ref Message m, ThumbnailButtonCollection collection, IntPtr handle) {
			if(m.Msg == MSG.WM_COMMAND && FormShadow.Helpers.NativeHelper.HIWORD(m.WParam) == ThumbnailButtonCore.Click) {
				int buttonId = FormShadow.Helpers.NativeHelper.LOWORD(m.WParam);
				foreach(ThumbnailButton button in Assistant.ThumbnailButtons.Where(x => x.Id == buttonId))
					button.RaiseThumbButtonClick(handle);
			}
		}
	}
}
