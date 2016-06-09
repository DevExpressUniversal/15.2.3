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
using Microsoft.Win32;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraPrinting.Native {
	public static class RegistryHelper {
		#region inner class
		private class SimpleStorage : IXtraSerializable {
			string data = "";
			[XtraSerializableProperty()]
			public string Data {
				get { return data; }
				set { data = value == null ? "" : value; }
			}
			public string[] GetData() {
				ArrayList items = new ArrayList(System.Text.RegularExpressions.Regex.Split(data, "\r\n"));
				for (int i = items.Count - 1; i >= 0; i--) {
					if (items[i].Equals(""))
						items.RemoveAt(i);
				}
				return items.ToArray(typeof(string)) as string[];
			}
			public void SetData(string[] items) {
				data = String.Join("\r\n", items);
			}
			void IXtraSerializable.OnStartDeserializing(DevExpress.Utils.LayoutAllowEventArgs e) {
			}
			void IXtraSerializable.OnEndDeserializing(string restoredVersion) {
			}
			void IXtraSerializable.OnStartSerializing() {
			}
			void IXtraSerializable.OnEndSerializing() {
			}
		}
		#endregion
		static public string[] LoadRegistryValue(string registryPath, string registryKey) {
			if (registryKey.Length == 0)
				return new string[] { };
			XtraSerializer serializer = new RegistryXtraSerializer();
			SimpleStorage storage = new SimpleStorage();
			serializer.DeserializeObject(storage, registryPath, registryKey);
			return storage.GetData();
		}
		static public void SaveRegistryValue(string[] items, string registryPath, string registryKey) {
			if (registryKey.Length == 0)
				return;
			XtraSerializer serializer = new RegistryXtraSerializer();
			SimpleStorage storage = new SimpleStorage();
			storage.SetData(items);
			serializer.SerializeObject(storage, registryPath, registryKey);
		}
		static public void DeleteRegistryValue(string item, string registryPath, string registryKey) {
			ArrayList items = new ArrayList(LoadRegistryValue(registryPath, registryKey));
			items.Remove(item);
			SaveRegistryValue(items.ToArray(typeof(string)) as string[], registryPath, registryKey);
		}
		static public bool AddRegistryValue(string item, string registryPath, string registryKey) {
			ArrayList items = new ArrayList(LoadRegistryValue(registryPath, registryKey));
			if (!items.Contains(item)) {
				items.Add(item);
				SaveRegistryValue(items.ToArray(typeof(string)) as string[], registryPath, registryKey);
				return true;
			}
			return false;
		}
		public static void SetRegistryValues(string keyPath, object[] values) {
			try {
				RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath);
				for(int i = 0; i < values.Length; i++)
					key.SetValue("item" + i, values[i]);
			} catch { }
		}
		public static object[] GetRegistryValues(string keyPath) {
			try {
				RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath);
				if(key != null) {
					string[] names = key.GetValueNames();
					object[] values = new object[names.Length];
					for(int i = 0; i < names.Length; i++)
						values[i] = key.GetValue(names[i]);
					return values;
				}
			} catch { }
			return new object[0];
		}
		public static bool SetRegistryValue(string keyPath, string name, object val) {
			try {
				RegistryKey key = Registry.CurrentUser.CreateSubKey(keyPath);
				key.SetValue(name, val);
			} catch {
				return false;
			}
			return true;
		}
		public static object GetRegistryValue(string keyPath, string name, object defaultValue) {
			try {
				RegistryKey key = Registry.CurrentUser.OpenSubKey(keyPath);
				if(key == null)
					return defaultValue;
				return key.GetValue(name, defaultValue);
			} catch {
				return defaultValue;
			}
		}
	}
}
