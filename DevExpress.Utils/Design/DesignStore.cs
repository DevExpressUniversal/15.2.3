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
using System.Windows.Forms;
using DevExpress.Utils.Serializing.Helpers;
using System.Drawing;
namespace DevExpress.Utils.Design {
	public class PropertyStore {
		string path;
		Hashtable properties;
		public PropertyStore(PropertyStore baseStore, string path) : this(baseStore.Path + "\\" + path) {
		}
		public PropertyStore(string path) {
			this.path = path;
			this.properties = new Hashtable();
		}
		public bool IsEmpty { get { return Properties.Count == 0; } }
		protected Hashtable Properties { get { return properties; } }
		public string Path { get { return path; } }
		public virtual void AddForm(Form form) {
			PropertyStore formProperties = new PropertyStore(Path + "\\" + GetFormKey(form));
			formProperties.AddProperty("Bounds", form.Bounds);
			formProperties.AddProperty("WindowState", form.WindowState);
			Properties[GetFormKey(form)] = formProperties;
		}
		public virtual void AddProperty(string property, object val) {
			Properties[property] = val is PropertyStore ? val : ObjectConverter.ObjectToString(val);
		}
		public PropertyStore RestoreProperties(string property) {
			return Properties[property] as PropertyStore;
		}
		public object RestoreProperty(string property, object defaultValue) {
			return RestoreProperty(property, null, defaultValue);
		}
		public int RestoreIntProperty(string property, int defaultValue) {
			return (int)RestoreProperty(property, typeof(int), defaultValue);
		}
		public bool RestoreBoolProperty(string property, bool defaultValue) {
			return (bool)RestoreProperty(property, typeof(bool), defaultValue);
		}
		public virtual object RestoreProperty(string property, Type propertyType, object defaultValue) {
			object val = properties[property];
			if(val == null) return defaultValue;
			if(propertyType == null) return val;
			try {
				val = ObjectConverter.StringToObject(val.ToString(), propertyType);
				return Convert.ChangeType(val, propertyType, System.Globalization.CultureInfo.InvariantCulture);
			}
			catch {
			}
			return defaultValue;
		}
		protected string GetFormKey(Form form) {
			return string.Format("PS_Form_{0}_2", form.Name);
		}
		public virtual void RestoreForm(Form form) {
			PropertyStore formProperties = Properties[GetFormKey(form)] as PropertyStore;
			if(formProperties == null) {
				Rectangle sBounds = Screen.GetWorkingArea(form.Location);
				if(form.Height > sBounds.Height) form.Height = sBounds.Height;
				if(form.Width > sBounds.Width) form.Width = sBounds.Width;
				return;
			}
			Rectangle screen = SystemInformation.VirtualScreen;
			Rectangle bounds = (Rectangle)formProperties.RestoreProperty("Bounds", typeof(Rectangle), form.Bounds);
			FormWindowState wState = (FormWindowState)formProperties.RestoreProperty("WindowState", typeof(FormWindowState), form.WindowState);
			if(bounds.Top < screen.Top || bounds.Top > screen.Bottom) bounds.Y = screen.Top;
			if(bounds.Left < screen.Left || bounds.Left > screen.Right) bounds.X = screen.Left;
			if(bounds.Right > screen.Right) bounds.Width = screen.Right - bounds.Left;
			if(bounds.Bottom > screen.Bottom) bounds.Height = screen.Bottom - bounds.Top;
			form.StartPosition = FormStartPosition.Manual;
			form.WindowState = wState;
			form.Bounds = bounds;
		}
		public virtual void Store() {
			RegistryKey key = Registry.CurrentUser.OpenSubKey(Path, true);
			if(key == null) key = Registry.CurrentUser.CreateSubKey(path);
			try {
				foreach(DictionaryEntry entry in Properties) {
					PropertyStore store = entry.Value as PropertyStore;
					if(store != null) {
						store.Store();
						continue;
					}
					key.SetValue(entry.Key.ToString(), entry.Value);
				}
			}
			catch {
			}
			if(key != null) key.Close();
		}
		public virtual void Restore() {
			Properties.Clear();
			RegistryKey key = Registry.CurrentUser.OpenSubKey(Path);
			if(key == null) return;
			string[] subKeys = key.GetSubKeyNames();
			if(subKeys != null && subKeys.Length > 0) {
				foreach(string subKey in subKeys) {
					PropertyStore store = new PropertyStore(Path + "\\" + subKey);
					store.Restore();
					Properties[subKey] = store;
				}
			}
			string[] values = key.GetValueNames();
			if(values != null && values.Length != 0) {
				foreach(string val in values) {
					if(Properties.ContainsKey(val)) continue;
					Properties[val] = key.GetValue(val);
				}
			}
			if(key != null) key.Close();
		}
	}
}
