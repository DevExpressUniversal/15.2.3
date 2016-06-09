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
using System.Text;
using System.ComponentModel;
using DevExpress.XtraReports.Serialization;
using System.Collections;
using System.ComponentModel.Design;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.UI {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class XRScriptsBase : IXRSerializable, IDisposable {
		#region static
		static readonly object padlock = new object();
		static Dictionary <ReflectionKey, PropertyDescriptor> propertyDict = new Dictionary<ReflectionKey, PropertyDescriptor>();
		static PropertyDescriptor GetPropertyByName(Type type, string propertyName) {
			PropertyDescriptor property = null;
			ReflectionKey key = new ReflectionKey(type, propertyName);
			lock(padlock) {
				if(propertyDict.TryGetValue(key, out property))
					return property;
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
				property = properties[propertyName];
				if(property == null) {
					foreach(PropertyDescriptor dispProperty in properties)
						if(dispProperty.DisplayName == propertyName) {
							property = dispProperty;
							break;
						}
				}
				if(property != null)
					property = XRAccessor.GetFastPropertyDescriptor(property);
				propertyDict.Add(key, property);
			}
			return property;
		}
		static Native.MethodScriptAttribute GetScriptAttribute(PropertyDescriptor property) {
			return (property != null) ? (Native.MethodScriptAttribute)property.Attributes[typeof(Native.MethodScriptAttribute)] :
				null;
		}
		#endregion
		#region Fields & Properties
		protected Component component;
		EventHandlerList events;
		public EventHandlerList m_events;
		internal abstract ScriptLanguage ScriptLanguage { get; }
		internal abstract string ComponentName { get; }
		internal Component Component { get { return component; } }
		internal EventHandlerList Events { get { return events; } }
		#endregion
		protected XRScriptsBase() {
			Initialize();
		}
		protected XRScriptsBase(Component component) : this() {
			this.component = component;
		}
		public void Dispose() {
			component = null;
			if(events != null) {
				events.Dispose();
				events = null;
			}
		}
		internal void Initialize() {
			if(events != null)
				events.Dispose();
			events = new EventHandlerList();
		}
		internal void ConvertScripts(XtraReport report) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
			foreach (PropertyDescriptor property in properties) {
				report.ScriptsSource += CreateConvertedScript(property);
			}
		}
		internal string GetProcName(string eventName) {
			if (string.IsNullOrEmpty(eventName))
				return null;
			PropertyDescriptor property = GetPropertyByName(eventName);
			return property != null ? (string)property.GetValue(this) : null;
		}
		internal bool NeedConvertScript(string scriptSource) {
			if (string.IsNullOrEmpty(scriptSource))
				return false;
			if (ScriptLanguage == ScriptLanguage.CSharp)
				return scriptSource.Contains("}");
			else if (ScriptLanguage == ScriptLanguage.VisualBasic)
				return scriptSource.Contains("End Sub");
			else if (ScriptLanguage == ScriptLanguage.JScript)
				return scriptSource.Contains("}");
			return false;
		}
		string CreateConvertedScript(PropertyDescriptor property) {
			string currentScriptSource = (string)property.GetValue(this);
			Native.MethodScriptAttribute attr = GetScriptAttribute(property);
			if (string.IsNullOrEmpty(currentScriptSource) || attr == null)
				return string.Empty;
			string newScriptName = ComponentName + "_" + attr.Name;
			string scriptName = XRControl.EventNames.HandlerPrefix + attr.Name;
			property.SetValue(this, newScriptName);
			return "\r\n" + currentScriptSource.Replace(scriptName, newScriptName) + "\r\n";
		}
		public string[] GetScriptsNames(Func<PropertyDescriptor, string> getPropertyName) {
			List<string> scriptList = new List<string>();
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
			foreach(PropertyDescriptor property in properties)
				if(((BrowsableAttribute)property.Attributes[typeof(BrowsableAttribute)]).Browsable && !string.IsNullOrEmpty(property.DisplayName))
					scriptList.Add(getPropertyName(property));
			return scriptList.ToArray();
		}
		PropertyDescriptor GetPropertyByName(string propertyName) {
			return GetPropertyByName(GetType(), propertyName);
		}
		#region Serialiation
		void IXRSerializable.SerializeProperties(XRSerializer serializer) {
			SerializeProperties(serializer);
		}
		void IXRSerializable.DeserializeProperties(XRSerializer serializer) {
			DeserializeProperties(serializer);
		}
		IList IXRSerializable.SerializableObjects {
			get { return new object[] { }; }
		}
		protected virtual void SerializeProperties(XRSerializer serializer) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
			foreach (PropertyDescriptor property in properties) {
				Native.MethodScriptAttribute attr = GetScriptAttribute(property);
				if (attr != null)
					serializer.SerializeString(property.Name, (string)property.GetValue(this), String.Empty);
			}
		}
		protected virtual void DeserializeProperties(XRSerializer serializer) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
			foreach (PropertyDescriptor property in properties) {
				Native.MethodScriptAttribute attr = GetScriptAttribute(property);
				if (attr != null)
					property.SetValue(this, serializer.DeserializeString(property.Name, String.Empty));
			}
		}
		#endregion
		public virtual bool IsDefault() {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this);
			foreach (PropertyDescriptor property in properties) {
				Native.MethodScriptAttribute attr = GetScriptAttribute(property);
				if (attr != null && !IsDefault(property))
					return false;
			}
			return true;
		}
		bool IsDefault(PropertyDescriptor property) {
			try {
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)property.Attributes[typeof(DefaultValueAttribute)];
				return defaultValueAttribute != null && Comparer.Equals(defaultValueAttribute.Value, property.GetValue(this));
			} catch {
				return false;
			}
		}
		public string GenerateDefaultEventScript(string propertyName) {
			return GetDefaultEventScript(GetPropertyByName(propertyName), null);
		}
		public string GenerateDefaultEventScript(string propertyName, string propertyValue) {
			return GetDefaultEventScript(GetPropertyByName(propertyName), propertyValue);
		}
		internal void SetPropertyValue(string propertyName, string value) {
			PropertyDescriptor property = GetPropertyByName(propertyName);
			if (property != null && GetScriptAttribute(property) != null) {
				property.SetValue(this, value);
				if (this.component != null && this.component.Site != null) {
					IComponentChangeService changeService = (IComponentChangeService)this.component.Site.GetService(typeof(IComponentChangeService));
					if (changeService != null)
						changeService.OnComponentChanged(this.component, TypeDescriptor.GetProperties(this.component)["Scripts"], null, this);
				}
			}
		}
		internal string GetDefaultPropertyValue(string propertyName) {
			string suffix = GetDefaultSuffix(propertyName);
			if(suffix != null)
				return this.ComponentName + "_" + suffix;
			return string.Empty;
		}
		internal string GetDefaultSuffix(string propertyName) {
			PropertyDescriptor property = GetPropertyByName(propertyName);
			if(property == null)
				return null;
			Native.MethodScriptAttribute attr = GetScriptAttribute(property);
			return attr != null ? attr.Name : null; 
		}
		string GetDefaultEventScript(PropertyDescriptor property, string propertyValue) {
			if (property != null) {
				Native.MethodScriptAttribute attr = GetScriptAttribute(property);
				if (attr != null)
					if (string.IsNullOrEmpty(propertyValue))
						return attr.GetDefaultValue(this.ComponentName + "_", ScriptLanguage);
					else return attr.GetDefaultValueByName(propertyValue, ScriptLanguage);
			}
			return "";
		}
	}
}
