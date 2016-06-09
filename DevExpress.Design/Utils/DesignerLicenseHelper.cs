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
using System.ComponentModel.Design;
using System.Reflection;
namespace DevExpress.Utils.Design {
	public static class DesignerLicenseHelper {
		public static bool IsNET4 {
			get {
				object[] attributes = typeof(DesignSurface).Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
				if(attributes == null || attributes.Length == 0) return false;
				AssemblyInformationalVersionAttribute attr = attributes[0] as AssemblyInformationalVersionAttribute;
				if(attr == null) return false;
				string[] versionParts = attr.InformationalVersion.Split('.');
				int major;
				if(int.TryParse(versionParts[0], out major))
					return major >= 4;
				else
					return false;
			}
		}
		static List<DesignSurface> designSurfaces = new List<DesignSurface>();
		public static void HookDesignSurfaceDisposed(ComponentDesigner designer) {
			if(IsNET4) return;
			if(designer == null)
				return;
			IServiceProvider provider = designer.Component.Site;
			if(provider == null)
				return;
			DesignSurface currentDesignSurface = provider.GetService(typeof(DesignSurface)) as DesignSurface;
			lock(designSurfaces) {
				if(currentDesignSurface == null || designSurfaces.Contains(currentDesignSurface))
					return;
				designSurfaces.Add(currentDesignSurface);
			}
			currentDesignSurface.Disposed += new EventHandler(OnDesignSurfaceDisposed);
		}
		static void OnDesignSurfaceDisposed(object sender, EventArgs e) {
			DesignSurface designSurface = sender as DesignSurface;
			lock(designSurfaces) {
				if(designSurface == null || !designSurfaces.Contains(designSurface))
					return;
				designSurfaces.Remove(designSurface);
			}
			designSurface.Disposed -= new EventHandler(OnDesignSurfaceDisposed);
			PropertyInfo pi = typeof(DesignSurface).GetProperty("ServiceContainer", BindingFlags.NonPublic | BindingFlags.Instance);
			if(pi == null)
				return;
			ServiceContainer sc = (ServiceContainer)pi.GetValue(sender, null);
			if(sc == null)
				return;
			FieldInfo fi = typeof(ServiceContainer).GetField("services", BindingFlags.NonPublic | BindingFlags.Instance);
			if(fi == null)
				return;
			object services = fi.GetValue(sc);
			Hashtable hash = services as Hashtable;
			if(hash != null) {
				foreach(DictionaryEntry entry in hash) {
					DisposeLicenseService(entry.Key, entry.Value);
				}
			} else {
				Dictionary<Type, object> dic = services as Dictionary<Type, object>;
				if(dic != null) {
					foreach(KeyValuePair<Type, object> entry in dic) {
						DisposeLicenseService(entry.Key, entry.Value);
					}
				}
			}
		}
		static void DisposeLicenseService(object key, object value) {
			if(key.ToString().EndsWith("LicenseService")) {
				IDisposable srv = value as IDisposable;
				if(srv != null)
					srv.Dispose();
			}
		}
	}
}
