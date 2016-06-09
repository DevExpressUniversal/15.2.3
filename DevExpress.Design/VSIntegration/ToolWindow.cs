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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
namespace DevExpress.Design.VSIntegration {
	public interface IVSToolWindow {
		void Show(IServiceProvider provider);
		void ShowNoActivate(IServiceProvider provider);
		void Hide(IServiceProvider provider);
		void CloseFrame(IServiceProvider provider);
		void HostControl(Control control);
		bool IsValid { get; }
	}
	public interface IVSToolWindowService {
		IVSToolWindow Create(IServiceProvider serviceProvider, string caption, Guid guid);
	}
	[
	ProgId("DevExpress.Design.VSIntegration.VSHostControl"),
	ClassInterface(ClassInterfaceType.AutoDispatch),
	Guid("60B917CC-D64B-48D8-AE2E-B334156F5060"),
	ToolboxItem(false)
	]
	public class VSHostControl : UserControl {
		public VSHostControl() {
			this.Disposed += VSHostControl_Disposed;
		}
		void VSHostControl_Disposed(object sender, EventArgs e) {
		}
	}
	public class VSDummyToolWindow : IVSToolWindow {
		public void Show(IServiceProvider provider) { }
		public void ShowNoActivate(IServiceProvider provider) { }
		public void Hide(IServiceProvider provider) { }
		public void CloseFrame(IServiceProvider provider) { }
		public void HostControl(Control control) { }
		public bool IsValid { get { return false; } }
	}
	public abstract class VSToolWindow : IVSToolWindow, IVsWindowFrameNotify3, Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget {
		EnvDTE.Window toolWindow;
		VSHostControl host;
		bool closing;
		bool? persistentVisible;
		IServiceProvider servProvider;
		protected abstract string RegistryKey { get; }
		protected abstract Type ResFinderType { get; }
		string ToolWindowKey {
			get { return toolWindow != null ? toolWindow.ObjectKind : string.Empty; }
		}
		bool PersistentVisible {
			get {
				if(persistentVisible == null)
					persistentVisible = ReadPersistentVisible();
				return (bool)persistentVisible;
			}
			set {
				persistentVisible = value;
			}
		}
		protected VSToolWindow(IServiceProvider servProvider, string caption, Guid toolWindowGuid, string bitmapResourceName) {
			if(servProvider == null)
				throw new ArgumentNullException("servProvider");
			EnvDTE.ProjectItem projectItem = (EnvDTE.ProjectItem)servProvider.GetService(typeof(EnvDTE.ProjectItem));
			EnvDTE80.DTE2 app = (EnvDTE80.DTE2)projectItem.DTE;
			object objTemp = null;
			string guidString = '{' + toolWindowGuid.ToString() + '}';
			toolWindow = CreateToolWindow(app, caption, guidString, ref objTemp);
			if(toolWindow == null) 
				toolWindow = CreateToolWindow(app, caption, guidString, ref objTemp);
			host = (VSHostControl)objTemp;
			AssignBitmap(bitmapResourceName);
			SubscribeWindowHiding(toolWindowGuid, app);
		}
		static EnvDTE.Window CreateToolWindow(EnvDTE80.DTE2 app, string caption, string guidString, ref object objTemp) {
			try {
				return ((EnvDTE80.Windows2)app.Windows).CreateToolWindow2(
					FakeAddIn.Instance,
					typeof(VSHostControl).Assembly.FullName,
					typeof(VSHostControl).FullName,
					caption,
					guidString,
					ref objTemp);
			} catch {
				return null;
			}
		}
		public bool IsValid {
			get { return host != null && !host.IsDisposed; }
		}
		public void Show(IServiceProvider provider) {
			this.servProvider = provider;
			SetToolWindowVisibility(true);
			if(toolWindow != null)
				toolWindow.Activate();
		}
		public void ShowNoActivate(IServiceProvider provider) {
			this.servProvider = provider;
			SetToolWindowVisibility(true);
		}
		public void Hide(IServiceProvider provider) {
			SetToolWindowVisibility(false);
		}
		void SetToolWindowVisibility(bool value) {
			bool visible = PersistentVisible && value;
			if(host != null && !host.IsDisposed)
				host.BeginInvoke(new Action<bool>(SetVisibilityCore), visible);
		}
		void SetVisibilityCore(bool visible) {
			closing = true;
			try {
				if(toolWindow != null && toolWindow.Visible != visible)
					toolWindow.Visible = visible;
			}
			finally {
				closing = false;
			}
		}
		public void CloseFrame(IServiceProvider provider) {
			closing = true;
			try {
				if(toolWindow != null)
					toolWindow.Close(EnvDTE.vsSaveChanges.vsSaveChangesNo);
				if(persistentVisible.HasValue)
					SavePersistentVisible(persistentVisible.Value);
			}
			finally {
				closing = false;
			}
		}
		public void HostControl(Control control) {
			if(host != null && host.Controls.IndexOf(control) < 0) {
				control.Dock = DockStyle.Fill;
				host.Controls.Add(control);
				host.Dock = DockStyle.Fill;
			}
		}
		public void ShowPersistently() {
			PersistentVisible = true;
			Show(null);
		}
		void AssignBitmap(string bitmapResourceName) {
			try {
				if(toolWindow != null) {
					System.Drawing.Bitmap bmp = ResourceImageHelper.CreateBitmapFromResources(bitmapResourceName, ResFinderType);
					if(bmp != null) {
						stdole.IPictureDisp pictDisp = (stdole.IPictureDisp)MenuItemPictureHelper.ConvertImageToPicture(bmp);
						((EnvDTE80.Window2)toolWindow).SetTabPicture(pictDisp);
					}
				}
			}
			catch { }
		}
		void SubscribeWindowHiding(Guid toolWindowGuid, EnvDTE80.DTE2 app) {
			ServiceProvider sp = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)app);
			IVsUIShell uiShell = (IVsUIShell)sp.GetService(typeof(SVsUIShell));
			IVsWindowFrame frame;
			uiShell.FindToolWindow((int)__VSFINDTOOLWIN.FTW_fFrameOnly, ref toolWindowGuid, out frame);
			if(frame != null)
				frame.SetProperty((int)__VSFPROPID.VSFPROPID_ViewHelper, this);
		}
		[CLSCompliant(false)]
		protected virtual bool CanChangePersistentVisibility(__FRAMESHOW fShow) {
			if(closing || fShow != __FRAMESHOW.FRAMESHOW_Hidden || toolWindow == null ||
				toolWindow.Visible || toolWindow.DTE.WindowConfigurations.ActiveConfigurationName != "Design")
				return false;
			return true;
		}
		#region IVsWindowFrameNotify3 Members
		int IVsWindowFrameNotify3.OnShow(int fShow) {
			try {
				if(CanChangePersistentVisibility((__FRAMESHOW)fShow))
					PersistentVisible = false;
			}
			catch {
			}
			return Microsoft.VisualStudio.VSConstants.S_OK;
		}
		int IVsWindowFrameNotify3.OnClose(ref uint pgrfSaveOptions) {
			return Microsoft.VisualStudio.VSConstants.S_OK;
		}
		int IVsWindowFrameNotify3.OnDockableChange(int fDockable, int x, int y, int w, int h) {
			return Microsoft.VisualStudio.VSConstants.S_OK;
		}
		int IVsWindowFrameNotify3.OnMove(int x, int y, int w, int h) {
			return Microsoft.VisualStudio.VSConstants.S_OK;
		}
		int IVsWindowFrameNotify3.OnSize(int x, int y, int w, int h) {
			return Microsoft.VisualStudio.VSConstants.S_OK;
		}
		#endregion
		void SavePersistentVisible(bool visibility) {
			try {
				RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
				if(key == null)
					key = Registry.CurrentUser.CreateSubKey(RegistryKey);
				if(!string.IsNullOrEmpty(ToolWindowKey))
					key.SetValue(ToolWindowKey, visibility ? 1 : 0, RegistryValueKind.DWord);
				key.Close();
			}
			catch {
			}
		}
		bool ReadPersistentVisible() {
			bool visibility = true;
			try {
				RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKey);
				if(key != null && !string.IsNullOrEmpty(ToolWindowKey)) {
					visibility = (int)key.GetValue(ToolWindowKey, 1) != 0;
					key.Close();
				}
			}
			catch {
			}
			return visibility;
		}
		#region IOleCommandTarget Members
		[CLSCompliant(false)]
		public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut) {
			if(servProvider != null) {
				Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget target = servProvider.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget)) as Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
				if(target != null)
					target.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
			}
			return (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDERR_E_NOTSUPPORTED;
		}
		[CLSCompliant(false)]
		public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, Microsoft.VisualStudio.OLE.Interop.OLECMD[] prgCmds, IntPtr pCmdText) {
			if(servProvider != null) {
				Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget target = servProvider.GetService(typeof(Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget)) as Microsoft.VisualStudio.OLE.Interop.IOleCommandTarget;
				if(target != null)
					return target.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
			}
			return (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDERR_E_NOTSUPPORTED;
		}
		#endregion
	}
	[CLSCompliant(false)]
	public class FakeAddIn : AddIn {
		public static readonly AddIn Instance = new FakeAddIn();
		#region AddIn Members
		AddIns AddIn.Collection {
			get { throw new NotImplementedException(); }
		}
		bool AddIn.Connected {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		DTE AddIn.DTE {
			get { throw new NotImplementedException(); }
		}
		string AddIn.Description {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		string AddIn.Guid {
			get { throw new NotImplementedException(); }
		}
		string AddIn.Name {
			get { throw new NotImplementedException(); }
		}
		object AddIn.Object {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		string AddIn.ProgID {
			get { return "FakeProgId"; }
		}
		void AddIn.Remove() {
			throw new NotImplementedException();
		}
		string AddIn.SatelliteDllPath {
			get { throw new NotImplementedException(); }
		}
		#endregion
	}
}
