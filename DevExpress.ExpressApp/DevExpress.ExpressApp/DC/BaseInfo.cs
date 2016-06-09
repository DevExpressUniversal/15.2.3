#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Linq;
using System.Reflection;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.DC {
	public class BaseInfo : IBaseInfo, IDisposableExt {
		private readonly List<Attribute> allAttributes;
		private readonly List<Attribute> ownAttributes;
		private readonly Dictionary<Type, Object> extenders;
		private TypesInfo store;
		private ITypeInfoSource source;
		private Boolean isAttributeAdding;
		private Boolean isAttributesInitialized;
		private Boolean AreSameObjects(Object obj1, Object obj2) {
			if(obj1 == null) {
				return (obj2 == null);
			}
			return (obj1.Equals(obj2) || AreSameArrays(obj1, obj2));
		}
		private Boolean AreSameArrays(Object obj1, Object obj2) {
			Array array1 = obj1 as Array;
			Array array2 = obj2 as Array;
			if((array1 == null) || (array2 == null) || (array1.Length != array2.Length)) {
				return false;
			}
			for(int i = 0; i < array1.Length; i++) {
				Object item1 = array1.GetValue(i);
				Object item2 = array2.GetValue(i);
				if(item1 == null) {
					if(item2 == null) {
						continue;
					}
					else {
						return false;
					}
				}
				else if(!item1.Equals(item2)) {
					return false;
				}
			}
			return true;
		}
		private Boolean AreSameAttributes(Attribute a, Attribute b) {
			if(a == null) {
				return (b == null);
			}
			else if(b == null) {
				return false;
			}
			Type type1 = a.GetType();
			Type type2 = b.GetType();
			if(type1 != type2) {
				return false;
			}
			if(type1.IsVisible) {
				object obj1, obj2;
				PropertyInfo[] properties = type1.GetProperties(BindingFlags.Public | BindingFlags.Instance);
				for(int i = 0; i < properties.Length; i++) {
					obj1 = properties[i].GetValue(a, null);
					obj2 = properties[i].GetValue(b, null);
					if((obj1 == null) && (obj2 == null)) {
						continue;
					}
					if(!AreSameObjects(obj1, obj2)) {
						return false;
					}
				}
				FieldInfo[] fields = type1.GetFields(BindingFlags.Public | BindingFlags.Instance);
				for(int i = 0; i < fields.Length; i++) {
					obj1 = fields[i].GetValue(a);
					obj2 = fields[i].GetValue(b);
					if((obj1 == null) && (obj2 == null)) {
						continue;
					}
					if(!AreSameObjects(obj1, obj2)) {
						return false;
					}
				}
			}
			return true;
		}
		private void MergeAttributes(List<Attribute> current, Object[] newAttributes) {
			if(newAttributes != null) {
				List<Attribute> toAdd = new List<Attribute>();
				foreach(Object obj in newAttributes) {
					Attribute newAttribute = obj as Attribute;
					if(newAttribute != null && !current.Exists(attribute => AreSameAttributes(attribute, newAttribute))) {
						toAdd.Add(newAttribute);
					}
				}
				current.AddRange(toAdd);
			}
		}
		private void RemoveAttribute(List<Attribute> current, Attribute attribute) {
			for(Int32 i = current.Count - 1; i >= 0; i--) {
				if(AreSameAttributes(current[i], attribute)) {
					current.RemoveAt(i);
				}
			}
		}
		private void RemoveAttributes(List<Attribute> current, Type attributeType) {
			for(Int32 i = current.Count - 1; i >= 0; i--) {
				if(attributeType.IsAssignableFrom(current[i].GetType())) {
					current.RemoveAt(i);
				}
			}
		}
		private void EnsureAttributes() {
			if(!isAttributesInitialized) {
				lock(TypesInfo.lockObject) {
					if(!isAttributesInitialized) {
						EnsureAttributesCore();
						isAttributesInitialized = true;
					}
				}
			}
		}
		protected virtual void EnsureAttributesCore() { }
		public void SetAttributes(Object[] ownAttributes, Object[] allAttributes) {
			MergeAttributes(this.ownAttributes, ownAttributes);
			MergeAttributes(this.allAttributes, allAttributes);
		}
		protected virtual void OnAddAttribute(Attribute attribute) { }
		public BaseInfo(TypesInfo store) {
			this.store = store;
			allAttributes = new List<Attribute>();
			ownAttributes = new List<Attribute>();
			extenders = new Dictionary<Type, Object>();
		}
		public AttributeType FindAttribute<AttributeType>() where AttributeType : Attribute {
			return FindAttribute<AttributeType>(true);
		}
		public virtual IEnumerable<AttributeType> FindAttributes<AttributeType>() where AttributeType : Attribute {
			return FindAttributes<AttributeType>(true);
		}
		public AttributeType FindAttribute<AttributeType>(bool recursive) where AttributeType : Attribute {
			return FindAttributes<AttributeType>(recursive).FirstOrDefault();
		}
		public IEnumerable<AttributeType> FindAttributes<AttributeType>(bool recursive) where AttributeType : Attribute {
			Guard.NotDisposed(this);
			EnsureAttributes();
			List<Attribute> attributeSource;
			if(recursive) {
				attributeSource = allAttributes;
			}
			else {
				attributeSource = ownAttributes;
			}
			foreach(Attribute attribute in attributeSource) {
				AttributeType suitableAttribute = attribute as AttributeType;
				if(suitableAttribute != null) {
					yield return suitableAttribute;
				}
			}
		}
		public Attribute FindAttribute(Type attributeType) {
			EnsureAttributes();
			foreach(Attribute attribute in ownAttributes) {
				if(attributeType == attribute.GetType()) {
					return attribute;
				}
			}
			return null;
		}
		public T GetExtender<T>() {
			object result = null;
			if(extenders.TryGetValue(typeof(T), out result)) {
				return (T)result;
			}
			return default(T);
		}
		public void AddExtender<T>(T extender) {
			extenders[typeof(T)] = extender;
		}
		public void AddAttribute(Attribute attribute) {
			lock(TypesInfo.lockObject) {
				EnsureAttributes();
				if(!isAttributeAdding) {
					isAttributeAdding = true;
					try {
						if(source.AddAttribute(this, attribute)) {
							ownAttributes.Add(attribute);
							allAttributes.Add(attribute);
							OnAddAttribute(attribute);
						}
					}
					finally {
						isAttributeAdding = false;
					}
				}
			}
		}
		public void RemoveAttribute(Attribute attribute) {
			EnsureAttributes();
			if(source.RemoveAttribute(this, attribute.GetType())) {
				RemoveAttribute(ownAttributes, attribute);
				RemoveAttribute(allAttributes, attribute);
			}
		}
		public void RemoveAttributes<AttributeType>() where AttributeType : Attribute {
			EnsureAttributes();
			if(source.RemoveAttribute(this, typeof(AttributeType))) {
				RemoveAttributes(ownAttributes, typeof(AttributeType));
				RemoveAttributes(allAttributes, typeof(AttributeType));
			}
		}
		public ITypeInfoSource Source {
			get { return source; }
			set { source = value; }
		}
		public TypesInfo Store {
			get { return store; }
		}
		public IEnumerable<Attribute> Attributes {
			get {
				EnsureAttributes();
				return allAttributes;
			}
		}
		#region IDisposable Members
		private Boolean isDisposed = false;
		public virtual void Dispose() {
			if(isDisposed) {
				return;
			}
			isDisposed = true;
			store = null;
			source = null;
			allAttributes.Clear();
			ownAttributes.Clear();
			extenders.Clear();
		}
		public bool IsDisposed {
			get { return isDisposed; }
		}
		#endregion
	}
}
