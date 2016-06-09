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

using System.Text.RegularExpressions;
using System;
using DevExpress.Utils;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using System.Windows.Forms.Design.Behavior;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Popup;
using DevExpress.Utils.Win.Hook;
using DevExpress.Utils.Win;
using DevExpress.Data.Utils;
using DevExpress.Design.UI;
using System.Windows.Forms.Integration;
using DevExpress.Design.SmartTags;
using DevExpress.Skins;
using DevExpress.XtraEditors.Controls;
using System.Collections.Generic;
namespace DevExpress.Utils.Design {
	public class DesignTimeHelper {
		public static void UpdateDesignTimeLookAndFeel(IComponent component) {
			((DevExpress.LookAndFeel.Design.UserLookAndFeelDefault)DevExpress.LookAndFeel.UserLookAndFeel.Default).UpdateDesignTimeLookAndFeelEx(component);
		}
		public static object CreateComponent(IDesignerHost host, Type type, string name) {
			if(name == null) name = string.Empty;
			object instance = null;
			if(host != null && typeof(IComponent).IsAssignableFrom(type)) {
				if(name == string.Empty)
					instance = host.CreateComponent(type);
				else
					instance = host.CreateComponent(type, name);
				if(host != null) {
					IDesigner designer = host.GetDesigner((IComponent)instance);
					if(designer is ComponentDesigner) {
						((ComponentDesigner)designer).OnSetComponentDefaults();
					}
				}
			}
			else {
				instance = Activator.CreateInstance(type);
			}
			return instance;
		}
		public static PropertyDescriptor GetPropertyDescriptor(object component, string propertyName) {
			PropertyDescriptor result = TypeDescriptor.GetProperties(component)[propertyName];
			if(result == null)
				throw new InvalidOperationException("Can't find required property");
			return result;
		}
		public static void RefreshProperty(Component component, string name) {
			IComponentChangeService svc = (IComponentChangeService)component.Site.GetService(typeof(IComponentChangeService));
			if(svc == null) return;
			PropertyDescriptor pd = DesignTimeHelper.GetPropertyDescriptor(component, name);
			object oldValue = pd.GetValue(component);
			svc.OnComponentChanging(component, pd);
			svc.OnComponentChanged(component, pd, oldValue, oldValue);
		}
		public static IDesigner GetDesignerObject(IComponent component) {
			if(component.Site == null) return null;
			IDesignerHost host = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
			return host.GetDesigner(component);
		}
	}
	public class ControlDesignerEx : BaseControlDesigner { }
	public class DXCollectionEditor : DXCollectionEditorBase {
		public DXCollectionEditor(Type type) : base(type) { }
	}
#if DXWhidbey
	public class LoaderPatcherService {
		IDesignerHost host = null;
		int refCount = 0;
		public LoaderPatcherService(IDesignerHost host) {
			if(host == null) return;
			this.host = host;
			InstallService();
		}
		public static void InstallService(IDesignerHost host) {
			if(host == null) return;
			LoaderPatcherService service = host.GetService(typeof(LoaderPatcherService)) as LoaderPatcherService;
			if(service == null) {
				service = new LoaderPatcherService(host);
				host.AddService(typeof(LoaderPatcherService), service);
			} else {
				service.AddRef();
			}
		}
		public static void UnInstallService(IDesignerHost host) {
			if(host == null) return;
			LoaderPatcherService service = host.GetService(typeof(LoaderPatcherService)) as LoaderPatcherService;
			if(service != null) {
				service.Release();
			}
		}
		protected internal void AddRef() {
			this.refCount++;
		}
		protected internal void Release() {
			this.refCount--;
			if(this.refCount < 1) {
				UnInstallService();
				host.RemoveService(typeof(LoaderPatcherService));
			}
		}
		public IDesignerHost Host { get { return host; } }
		protected void InstallService() {
			this.refCount++;
			try {
				Assembly shell = FindShellAssembly();
				if(shell == null || host == null) return;
				Type handler = shell.GetType("Microsoft.VisualStudio.Shell.Design.AssemblyRefreshedEventHandler");
				Type service = shell.GetType("Microsoft.VisualStudio.Shell.Design.DynamicTypeService");
				if(handler == null || service == null) return;
				this.typeService = host.GetService(service);
				if(typeService == null) return;
				MethodInfo mi = this.GetType().GetMethod("FakeAssemblyRefreshed", BindingFlags.NonPublic | BindingFlags.Instance);
				ConstructorInfo[] ctors = handler.GetConstructors();
				this.handler = ctors[0].Invoke(new object[] { this, mi.MethodHandle.GetFunctionPointer() }) as Delegate;
				this.assemblyRefreshedEvent = service.GetEvent("AssemblyRefreshed");
				this.assemblyRefreshedEvent.AddEventHandler(typeService, this.handler);
			} catch {
			}
		}
		protected void UnInstallService() {
			if(typeService != null && assemblyRefreshedEvent != null) {
				assemblyRefreshedEvent.RemoveEventHandler(typeService, handler);
			}
			this.typeService = null;
			this.assemblyRefreshedEvent = null;
			this.handler = null;
		}
		EventInfo assemblyRefreshedEvent = null;
		object typeService = null;
		Delegate handler = null;
		Assembly FindShellAssembly() {
			foreach(Assembly asm in AppDomain.CurrentDomain.GetAssemblies()) {
				if(asm.GetName().Name == "Microsoft.VisualStudio.Shell.Design") return asm;
			}
			return null;
		}
		void FakeAssemblyRefreshed(object sender, EventArgs e) {
			CheckLoader();
		}
		protected virtual void CheckLoader() {
			try {
				if(this.host == null) return;
				IDesignerLoaderService srv = this.host.GetService(typeof(IDesignerLoaderService)) as IDesignerLoaderService;
				if(!IsRequirePatch(srv)) return;
				MethodInfo mi = srv.GetType().GetMethod("Reload", BindingFlags.NonPublic | BindingFlags.Instance);
				FieldInfo fi = typeof(BasicDesignerLoader).GetField("_state", BindingFlags.NonPublic | BindingFlags.Instance);
				BitVector32 bv = (BitVector32)fi.GetValue(srv);
				bv[0x20] = true; 
				fi.SetValue(srv, bv);
				if(mi == null) return;
				mi.Invoke(srv, new object[] { CreateFlags(srv) });
				bv = (BitVector32)fi.GetValue(srv);
				bv[0x100] = true;
				fi.SetValue(srv, bv); 
			} catch {
			}
		}
		int CreateFlags(IDesignerLoaderService srv) { return 0x100; }
		bool IsRequirePatch(IDesignerLoaderService srv) {
			if(srv == null) return false;
			FieldInfo fi = srv.GetType().GetField("_changedAssemblies", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fi == null) return false;
			object res = fi.GetValue(srv);
			fi.SetValue(srv, null); 
			return res != null;
		}
	}
#else
	public class LoaderPatcherService {
		public static void InstallService(IDesignerHost host) {
		}
		public static void UnInstallService(IDesignerHost host) {
		}
	}
#endif
}
