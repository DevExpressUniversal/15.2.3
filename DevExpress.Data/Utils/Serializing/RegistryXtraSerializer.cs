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
using DevExpress.Utils.Serializing.Helpers;
using Microsoft.Win32;
namespace DevExpress.Utils.Serializing {
	public class RegistryXtraSerializer : XtraSerializer {
		static string[] BaseKeyNames =
			new string[] { "HKEY_CLASSES_ROOT", "HKEY_CURRENT_USER", "HKEY_USERS", "HKEY_LOCAL_MACHINE", "HKEY_CURRENT_CONFIG" };
		static RegistryKey[] BaseKeys =
			new RegistryKey[] { Registry.ClassesRoot, Registry.CurrentUser, Registry.Users, Registry.LocalMachine, Registry.CurrentConfig };
		static RegistryKey GetBaseKey(string path) {
			for(int n = 0; n < BaseKeyNames.Length; n++) {
				if(path.StartsWith(BaseKeyNames[n])) {
					return BaseKeys[n];
				}
			}
			return Registry.CurrentUser;
		}
		string GetPath(string path) {
			for(int n = 0; n < BaseKeyNames.Length; n++) {
				if(path.StartsWith(BaseKeyNames[n])) {
					path = path.Remove(0, BaseKeyNames[n].Length);
					break;
				}
			}
			while(path.Length > 0 && path[0] == '\\')
				path = path.Remove(0, 1);
			return path;
		}
		protected override bool Serialize(string path, IXtraPropertyCollection props, string appName) {
			appName += "Data";
			RegistryKey startKey = GetBaseKey(path);
			string relPath = GetPath(path);
			if(relPath.Length == 0)
				return false;
			if(!relPath.EndsWith("\\"))
				relPath += "\\";
			DeleteKey(startKey, relPath, appName, appName);
			RegistryKey key = startKey.CreateSubKey(relPath + appName);
			try {
				SerializeLevel(key, props);
			} finally {
				key.Close();
			}
			return true;
		}
		void SerializeLevel(RegistryKey key, IXtraPropertyCollection props) {
			foreach(XtraPropertyInfo p in props) {
				SerializeProperty(key, p);
			}
		}
		void SerializeProperty(RegistryKey key, XtraPropertyInfo p) {
			RegistryKey newKey = key;
			if(p.IsKey) {
				newKey = key.CreateSubKey(p.Name);
			}
			try {
				object val = p.Value;
				if(val != null) {
					Type type = val.GetType();
					if(!type.IsPrimitive)
						val = ObjectConverter.ObjectToString(val);
				}
				if(!p.IsKey || val != null) {
					newKey.SetValue((p.IsKey ? null : p.Name), val == null ? NullValueString : val);
				}
				if(p.IsKey)
					SerializeLevel(newKey, p.ChildProperties);
			} finally {
				if(p.IsKey)
					newKey.Close();
			}
		}
		protected override IXtraPropertyCollection Deserialize(string path, string appName, IList objects) {
			appName += "Data";
			RegistryKey startKey = GetBaseKey(path);
			string relPath = GetPath(path);
			if(relPath.Length == 0)
				return null;
			if(!relPath.EndsWith("\\"))
				relPath += "\\";
			IXtraPropertyCollection props = null;
			RegistryKey key = startKey.OpenSubKey(relPath + appName);
			if(key == null)
				return null;
			try {
				props = DeserializeLevel(key);
			} finally {
				key.Close();
			}
			return props;
		}
		object CheckNullValue(object val) {
			if(val is string) {
				if(val.ToString() == NullValueString)
					return null;
			}
			return val;
		}
		IXtraPropertyCollection DeserializeLevel(RegistryKey key) {
			IXtraPropertyCollection list = new XtraPropertyCollection();
			string[] valueNames = key.GetValueNames();
			foreach(string valName in valueNames) {
				if(valName.Length == 0)
					continue;
				list.Add(new XtraPropertyInfo(valName, null, CheckNullValue(key.GetValue(valName))));
			}
			string[] subKeyNames = key.GetSubKeyNames();
			foreach(string subKeyName in subKeyNames) {
				RegistryKey newKey = key.OpenSubKey(subKeyName);
				try {
					XtraPropertyInfo pi = new XtraPropertyInfo(subKeyName, null, CheckNullValue(newKey.GetValue(null)), true);
					list.Add(pi);
					pi.ChildProperties.AddRange(DeserializeLevel(newKey));
				} finally {
					newKey.Close();
				}
			}
			return list;
		}
		static void DeleteKey(RegistryKey key, string relPath, string keyName, string appName) {
			if(appName == null || appName.Trim().Length == 0)
				return;
			if(keyName != appName)
				return; 
			RegistryKey newKey = key.OpenSubKey(relPath + keyName);
			if(newKey == null)
				return;
			newKey.Close();
			key.DeleteSubKeyTree(relPath + keyName);
		}
	}
}
