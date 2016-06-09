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

using DevExpress.Data.Utils;
using DevExpress.Mvvm.Native;
using System;
using System.Collections.Generic;
using System.Windows;
namespace DevExpress.Mvvm.UI.Native.ViewGenerator.Model {
	public interface IModelSubscribedEvent { }
	public interface IModelItem {
		IModelPropertyCollection Properties { get; }
		IModelEditingScope BeginEdit(string description);
		IEditingContext Context { get; }
		IViewItem View { get; }
		Type ItemType { get; }
		IModelItem Root { get; }
		IModelSubscribedEvent SubscribeToPropertyChanged(EventHandler handler);
		void UnsubscribeFromPropertyChanged(IModelSubscribedEvent e);
		object GetCurrentValue();
		IEnumerable<object> GetAttributes(Type attributeType);
		String Name { get; set; }
		IModelItem Parent { get; }
	}
	public interface IModelEditingScope : IDisposable {
		string Description { get; set; }
		void Complete();
		void Update();
	}
	public interface IViewItem {
		object PlatformObject { get; }
	}
	public interface IModelProperty {
		IModelItem Parent { get; }
		string Name { get; }
		bool IsSet { get; }
		bool IsReadOnly { get; }
		void ClearValue();
		IModelItem SetValue(object value);
		object ComputedValue { get; }
		IModelItemCollection Collection { get; }
		IModelItemDictionary Dictionary { get; }
		IModelItem Value { get; }
		Type PropertyType { get; }
	}
	public interface IModelItemCollection : IEnumerable<IModelItem> {
		IModelItem this[int index] { get; set; }
		void Add(IModelItem value);
		IModelItem Add(object value);
		void Clear();
		int IndexOf(IModelItem item);
		void Insert(int index, object value);
		void Insert(int index, IModelItem valueItem);
		bool Remove(IModelItem item);
		bool Remove(object item);
		void RemoveAt(int index);
	}
	public interface IModelItemDictionary {
		IEnumerable<IModelItem> Keys { get; }
		IEnumerable<IModelItem> Values { get; }
		IModelItem this[IModelItem key] { get; set; }
		IModelItem this[object key] { get; set; }
		void Add(object key, object value);
	}
	public interface IModelPropertyCollection : IEnumerable<IModelProperty> {
		IModelProperty this[string propertyName] { get; }
		IModelProperty this[DXPropertyIdentifier propertyName] { get; }
		IModelProperty Find(string propertyName);
		IModelProperty Find(Type propertyType, string propertyName);
	}
	public interface IModelService {
		IModelItem Root { get; }
		IModelSubscribedEvent SubscribeToModelChanged(EventHandler handler);
		void UnsubscribeFromModelChanged(IModelSubscribedEvent e);
		void RaiseModelChanged();
	}
	public interface IEditingContext {
		IModelItem CreateItem(Type type);
		IModelItem CreateItem(Type type, bool useDefaultInitializer);
		IModelItem CreateItem(DXTypeIdentifier typeIdentifier);
		IModelItem CreateItem(DXTypeIdentifier typeIdentifier, bool useDefaultInitializer);
		IModelItem CreateStaticMemberItem(Type type, string memberName);
		IServiceProvider Services { get; }
	}
	public static class ModelExtensions {
		public static void SetValue(this IModelItem item, string propertyName, object value, bool exceptionIfNotExist = true) {
			item.GetProperty(propertyName, exceptionIfNotExist).Do(x => x.SetValue(value));
		}
		public static void SetValue(this IModelItem item, DependencyProperty property, object value, bool exceptionIfNotExist = true) {
			item.GetProperty(property, exceptionIfNotExist).Do(x => x.SetValue(value));
		}
		public static void SetValueIfNotSet(this IModelItem item, string propertyName, object value, bool exceptionIfNotExist = true) {
			item.GetProperty(propertyName, exceptionIfNotExist).Do(x => x.SetValueIfNotSet(value));
		}
		public static void SetValueIfNotSet(this IModelItem item, DependencyProperty property, object value, bool exceptionIfNotExist = true) {
			item.GetProperty(property, exceptionIfNotExist).Do(x => x.SetValueIfNotSet(value));
		}
		public static IModelProperty GetProperty(this IModelItem item, DependencyProperty property, bool exceptionIfNotExist = true) {
			return item.GetProperty(property.Name, exceptionIfNotExist);
		}
		public static IModelProperty GetProperty(this IModelItem item, string propertyName, bool exceptionIfNotExist = true) {
			if(!exceptionIfNotExist) return item.Properties.Find(propertyName);
			return item.Properties[propertyName];
		}
		public static IModelItem GetRoot(this IEditingContext context) {
			return context.Services.GetService<IModelService>().Root;
		}
		public static void SetValueIfNotSet(this IModelProperty property, object value) {
			if(!property.IsSet && !object.Equals(value, property.ComputedValue))
				property.SetValue(value);
		}
	}
	public interface IMarkupAccessService { }
	public struct DXPropertyIdentifier : IEquatable<DXPropertyIdentifier> {
		readonly Type declaringType;
		readonly DXTypeIdentifier declaringTypeId;
		readonly string fullName;
		readonly string name;
		public Type DeclaringType { get { return declaringType; } }
		public DXTypeIdentifier DeclaringTypeIdentifier { get { return declaringTypeId; } }
		public string FullName { get { return fullName; } }
		public string Name { get { return name; } }
		public DXPropertyIdentifier(Type declaringType, string name) {
			if(declaringType == null)
				throw new ArgumentNullException("declaringType");
			if(name == null)
				throw new ArgumentNullException("name");
			this.declaringType = declaringType;
			this.declaringTypeId = new DXTypeIdentifier();
			this.name = name;
			this.fullName = CreateFullName(this.declaringType, this.declaringTypeId, name);
		}
		public DXPropertyIdentifier(DXTypeIdentifier declaringTypeId, string name) {
			if(name == null) {
				throw new ArgumentNullException("name");
			}
			if(declaringTypeId.IsEmpty) {
				throw new ArgumentNullException("declaringTypeId");
			}
			this.declaringType = null;
			this.declaringTypeId = declaringTypeId;
			this.name = name;
			this.fullName = CreateFullName(this.declaringType, this.declaringTypeId, name);
		}
		private static string CreateFullName(Type declaringType, DXTypeIdentifier declaringTypeId, string name) {
			if(!string.IsNullOrEmpty(name)) {
				string str = (declaringType != null) ? declaringType.Name : declaringTypeId.SimpleName;
				return (str + "." + name);
			}
			return string.Empty;
		}
		public override int GetHashCode() {
			int num = (declaringType != null) ? declaringType.GetHashCode() : declaringTypeId.GetHashCode();
			return (num ^ name.GetHashCode());
		}
		public override bool Equals(object obj) {
			return ((obj is DXPropertyIdentifier) && Equals((DXPropertyIdentifier)obj));
		}
		public static bool operator ==(DXPropertyIdentifier first, DXPropertyIdentifier second) {
			return first.Equals(second);
		}
		public static bool operator !=(DXPropertyIdentifier first, DXPropertyIdentifier second) {
			return !first.Equals(second);
		}
		public bool Equals(DXPropertyIdentifier other) {
			if(declaringType != null)
				return ((declaringType == other.declaringType) && (name == other.name));
			return (declaringTypeId.Equals(other.declaringTypeId) && (name == other.name));
		}
		public override string ToString() {
			return FullName;
		}
	}
	public struct DXTypeIdentifier : IEquatable<DXTypeIdentifier> {
		readonly string name;
		readonly int hashCode;
		public DXTypeIdentifier(string fullyQualifiedName) {
			if(fullyQualifiedName == null) {
				throw new ArgumentNullException("fullyQualifiedName");
			}
			this.name = fullyQualifiedName;
			this.hashCode = this.name.GetHashCode();
		}
		public bool IsEmpty { get { return string.IsNullOrEmpty(name); } }
		internal string SimpleName {
			get {
				string result = name;
				int index = result.IndexOf(',');
				if(index >= 0)
					result = result.Substring(0, index);
				index = result.LastIndexOf('.');
				if(index >= 0)
					result = result.Substring(index + 1);
				return result.Trim();
			}
		}
		public string Name { get { return name; } }
		public override int GetHashCode() {
			return hashCode;
		}
		public override bool Equals(object obj) {
			return ((obj is DXTypeIdentifier) && Equals((DXTypeIdentifier)obj));
		}
		public static bool operator ==(DXTypeIdentifier first, DXTypeIdentifier second) {
			return first.Equals(second);
		}
		public static bool operator !=(DXTypeIdentifier first, DXTypeIdentifier second) {
			return !first.Equals(second);
		}
		public bool Equals(DXTypeIdentifier other) {
			return (name == other.name);
		}
		public override string ToString() {
			return name;
		}
	}
}
