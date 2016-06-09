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
using System.ComponentModel;
using DevExpress.WebUtils;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Utils.Controls {
	public class OptionsHelper {
		public static string GetObjectText(object obj) {
			return GetObjectText(obj, false);
		}
		public static void SetOptionValue(object obj, string name, object value) {
			PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(obj);
			PropertyDescriptor property = coll[name];
			if(property != null) property.SetValue(obj, value);
		}
		public static object GetOptionValue(object obj, string name) {
			PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(obj);
			PropertyDescriptor property = coll[name];
			if(property != null) return property.GetValue(obj);
			return null;
		}
		public static T GetOptionValue<T>(object obj, string name) {
			PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(obj);
			PropertyDescriptor property = coll[name];
			if(property != null) return (T)property.GetValue(obj);
			return default(T);
		}
		public static string GetObjectText(object obj, bool includeSubObjects) {
			string res = string.Empty;
			try {
				PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(obj);
				foreach(PropertyDescriptor pd in coll) {
					if(!pd.IsBrowsable || pd.SerializationVisibility == DesignerSerializationVisibility.Hidden)
						continue;
					if(pd.SerializationVisibility == DesignerSerializationVisibility.Content) {
						if(includeSubObjects) {
							object val = pd.GetValue(obj);
							string s = (val != null) ? val.ToString() : string.Empty;
							if(!string.IsNullOrEmpty(s)) {
								if(res.Length > 0)
									res += ", ";
								res += string.Format("{0} = {{ {1} }}", pd.Name, s);
							}
						}
					}
					else if(!pd.IsReadOnly && pd.ShouldSerializeValue(obj)) {
						if(res.Length > 0)
							res += ", ";
						res += pd.Name;
						object val = pd.GetValue(obj);
						if(pd.PropertyType.Equals(typeof(string)))
							res += string.Format(" = '{0}'", pd.Converter.ConvertToString(val));
						else
							res += string.Format(" = {0}", val);
					}
				}
			}
			catch {
			}
			return res;
		}
	}
	#region BaseOptionChangedEventArgs
	public class BaseOptionChangedEventArgs : EventArgs {
		#region Fields
		string name;
		object oldValue, newValue;
		#endregion
		public BaseOptionChangedEventArgs() : this("", null, null) { }
		public BaseOptionChangedEventArgs(string name, object oldValue, object newValue) {
			this.name = name;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		#region Properties
		public string Name { get { return name; } }
		public object OldValue { get { return oldValue; } }
		public object NewValue {
			get { return newValue; }
			set { newValue = value; }
		}
		#endregion
	}
	#endregion
	#region BaseOptionChangedEventHandler
	public delegate void BaseOptionChangedEventHandler(object sender, BaseOptionChangedEventArgs e);
	#endregion
	#region BaseOptions
#if !DXPORTABLE
	[TypeConverter(typeof(ExpandableObjectConverter))]
#endif
	public class BaseOptions : ViewStatePersisterCore, INotifyPropertyChanged {
		public BaseOptions(IViewBagOwner viewBagOwner, string objectPath) : base(viewBagOwner, objectPath) { }
		public BaseOptions() : this(null, string.Empty) { }
		int lockUpdate = 0;
		protected internal BaseOptionChangedEventHandler ChangedCore;
		public virtual void Assign(BaseOptions options) {
		}
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			if (--lockUpdate == 0) {
				OnChanged(new BaseOptionChangedEventArgs());
			}
		}
		public virtual void CancelUpdate() { lockUpdate--; }
		protected virtual bool IsLockUpdate { get { return lockUpdate != 0; } }
		static object boolFalse = false;
		static object boolTrue = true;
		#region INotifyPropertyChanged Members
		PropertyChangedEventHandler onPropertyChanged;
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected internal virtual void RaisePropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		protected void OnChanged(string option, bool oldValue, bool newValue) {
			OnChanged(option, oldValue ? boolTrue : boolFalse, newValue ? boolTrue : boolFalse);
		}
		protected void OnChanged(string option, object oldValue, object newValue) {
			RaisePropertyChanged(option);
			if (IsLockUpdate)
				return;
			RaiseOnChanged(new BaseOptionChangedEventArgs(option, oldValue, newValue));
		}
#if DEBUGTEST
		internal void CallOnChanged(BaseOptionChangedEventArgs e) {
			OnChanged(e);
		}
#endif
		protected virtual void OnChanged(BaseOptionChangedEventArgs e) {
			RaisePropertyChanged(e.Name);
			if (IsLockUpdate)
				return;
			RaiseOnChanged(e);
		}
		protected virtual void RaiseOnChanged(BaseOptionChangedEventArgs e) {
			if (ChangedCore != null) ChangedCore(this, e);
		}
		protected internal bool ShouldSerialize() { return ShouldSerialize(null); }
#if !DXPORTABLE
		public override string ToString() {
			return OptionsHelper.GetObjectText(this);
		}
		protected internal virtual bool ShouldSerialize(IComponent owner) {
			return UniversalTypeConverter.ShouldSerializeObject(this, owner);
		}
#else
		protected internal virtual bool ShouldSerialize(IComponent owner) {
			return true;
		}
#endif
		public virtual void Reset() {
			PropertyDescriptorCollection pdColl = TypeDescriptor.GetProperties(this);
			BeginUpdate();
			try {
				foreach (PropertyDescriptor pd in pdColl) {
					pd.ResetValue(this);
				}
			}
			finally {
				EndUpdate();
			}
		}
	}
	#endregion
}
