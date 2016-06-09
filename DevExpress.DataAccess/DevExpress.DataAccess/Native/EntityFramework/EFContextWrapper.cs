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
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.DataAccess.EntityFramework;
namespace DevExpress.DataAccess.Native.EntityFramework {
	public class EFContextWrapper : IList, ITypedList {
		static IList CreateLocalStorageList(IEnumerable data) {
			IList localStorage = FakedListCreator.CreateGenericList(data);
			try {
				IEnumerator enumerator = data.GetEnumerator();
				try {
					while(enumerator.MoveNext()) {
						localStorage.Add(enumerator.Current);
					}
				} finally {
					IDisposable disposable = enumerator as IDisposable;
					if(disposable != null) {
						disposable.Dispose();
					}
				}
			} catch(EntityException ex) {
				if(ex.InnerException != null) {
					throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException.Message));
				}
				throw;
			}
			return localStorage;
		}
		bool useLocalStorage = true;
		readonly EFDataConnection dataConnection;
		readonly List<EFStoredProcedureInfo> storedProcedures = new List<EFStoredProcedureInfo>();
		readonly Dictionary<string, IList> values = new Dictionary<string, IList>();
		public bool UseLocalStorage { get { return this.useLocalStorage; } set { this.useLocalStorage = value; } }
		public Type Type { get { return this.dataConnection.ConnectionParameters.Source; } }
		public List<EFStoredProcedureInfo> StoredProcedures { get { return this.storedProcedures; } }
		#region IList Members
		int IList.Add(object value) {
			throw new NotSupportedException();
		}
		void IList.Clear() {
			throw new NotSupportedException();
		}
		bool IList.Contains(object value) {
			throw new NotSupportedException();
		}
		int IList.IndexOf(object value) {
			throw new NotSupportedException();
		}
		void IList.Insert(int index, object value) {
			throw new NotSupportedException();
		}
		bool IList.IsFixedSize { get { throw new NotSupportedException(); } }
		bool IList.IsReadOnly { get { throw new NotSupportedException(); } }
		void IList.Remove(object value) {
			throw new NotSupportedException();
		}
		void IList.RemoveAt(int index) {
			throw new NotSupportedException();
		}
		object IList.this[int index] {
			get {
				if(this.dataConnection != null && this.dataConnection.Model != null && this.dataConnection.Model.ContextInstance != null && index == 0)
					return this.dataConnection.Model.ContextInstance;
				throw new ArgumentOutOfRangeException();
			}
			set { throw new NotSupportedException(); }
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
		}
		int ICollection.Count { get { return this.dataConnection != null && this.dataConnection.Model != null && this.dataConnection.Model.ContextInstance != null ? 1 : 0; } }
		bool ICollection.IsSynchronized { get { throw new NotSupportedException(); } }
		object ICollection.SyncRoot { get { throw new NotSupportedException(); } }
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			throw new NotSupportedException();
		}
		#endregion
		#region ITypedList Members
		public string GetListName(PropertyDescriptor[] listAccessors) {
			return String.Empty;
		}
		public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
			if(listAccessors == null || listAccessors.Length == 0) {
				List<PropertyDescriptor> properties = new List<PropertyDescriptor>(GetListDescriptors(listAccessors));
				properties.AddRange(GetMethodDescriptors());
				return new PropertyDescriptorCollection(properties.ToArray());
			}
			return new DevExpress.Data.Browsing.DataBrowserHelper().GetListItemProperties(Type, listAccessors);
		}
		#endregion
		public EFContextWrapper(EFDataConnection dataConnection) {
			this.dataConnection = dataConnection;
		}
		public EFContextWrapper(EFDataConnection dataConnection, IEnumerable<EFStoredProcedureInfo> storedProcedures)
			: this(dataConnection) {
			this.storedProcedures.AddRange(storedProcedures);
		}
		public EFContextWrapper(EFDataConnection dataConnection, string storedProcedureName, IEnumerable<IParameter> parameters)
			: this(dataConnection) {
			this.storedProcedures.Add(new EFStoredProcedureInfo(storedProcedureName, parameters));
		}
		public PropertyDescriptor[] GetMethodDescriptors() {
			List<PropertyDescriptor> items = new List<PropertyDescriptor>();
			foreach(MethodInfo method in GetMethods()) {
				items.Add(new EFMethodDescriptor(method, this));
			}
			return items.ToArray();
		}
		public PropertyDescriptor[] GetListDescriptors(PropertyDescriptor[] listAccessors) {
			PropertyDescriptorCollection itemProperties = new DevExpress.Data.Browsing.DataBrowserHelper().GetListItemProperties(Type, listAccessors);
			string[] ignoreTypes = {
				"System.Data.Entity.Infrastructure.DbChangeTracker",
				"System.Data.Entity.Infrastructure.DbContextConfiguration",
				"System.Data.Entity.Database",
				"Microsoft.Data.Entity.ChangeTracking.ChangeTracker",
				"Microsoft.Data.Entity.Infrastructure.Database",
				"Microsoft.Data.Entity.Infrastructure.DatabaseFacade",
				"Microsoft.Data.Entity.Metadata.IModel",
			};
			List<PropertyDescriptor> items = new List<PropertyDescriptor>();
			foreach(PropertyDescriptor item in itemProperties) {
				if(!ignoreTypes.Contains(item.PropertyType.FullName))
					items.Add(new EFPropertyDescriptor(item, this));
			}
			return items.ToArray();
		}
		public object GetPropertyValue(string name) {
			if(!this.dataConnection.IsConnected)
				return null;
			if(this.values.ContainsKey(name))
				return this.values[name];
			object context = this.dataConnection.Model.ContextInstance;
			PropertyInfo property = context.GetType().GetProperty(name);
			if(property == null)
				return null;
			object data = property.GetValue(context, new object[] { });
			IQueryable queryable = data as IQueryable;
			if(queryable == null || !UseLocalStorage)
				return data;			
			IList localStorage = CreateLocalStorageList(queryable);
			this.values.Add(name, localStorage);
			return localStorage;
		}
		public object GetMethodValue(string name) {
			if(this.storedProcedures.All(p => p.Name != name))
				throw new ArgumentOutOfRangeException(string.Format("Only one of the selected data members ({0}) can be used", string.Join(", ", this.storedProcedures.Select(p => p.Name))));
			return this.values.ContainsKey(name) ? this.values[name] : null;
		}
		internal void FillData() {
			object context = this.dataConnection.Model.ContextInstance;
			if(context == null)
				return;
			try {
				this.values.Clear();
				foreach(EFStoredProcedureInfo storedProcedure in this.storedProcedures) {
					MethodInfo method = context.GetType().GetMethod(storedProcedure.Name);
					if(method != null) {
						IList localStorage;
						object result = method.Invoke(context, storedProcedure.Parameters.Select(p => p.Value).ToArray());
						IEnumerable resultEnumerable = result as IEnumerable;
						if(resultEnumerable != null)
							localStorage = CreateLocalStorageList(resultEnumerable);
						else if(DataContext.IsComplexType(method.ReturnType))
							localStorage = CreateLocalStorageList(new[] {result});
						else {
							Type resultType = typeof(SimpleReturnTypeWrapper<>).MakeGenericType(method.ReturnType);
							localStorage = CreateLocalStorageList(new[] {Activator.CreateInstance(resultType, result)});
						}
						this.values.Add(storedProcedure.Name, localStorage);
					}
				}
			} catch(TargetInvocationException ex) {
				if(ex.InnerException != null) {
					StringBuilder sb = new StringBuilder();
					Exception exception = ex.InnerException;
					while(exception != null) {
						sb.AppendLine(exception.Message);
						exception = exception.InnerException;
					}
					throw new Exception(sb.ToString());
				}
				throw;
			} catch(EntityException ex) {
				if(ex.InnerException != null) {
					throw new Exception(string.Format("{0}\r\n{1}", ex.Message, ex.InnerException.Message));
				}
				throw;
			}
		}
		internal void ClearData() {
			this.values.Clear();
		}
		IEnumerable<MethodInfo> GetMethods() {
			MethodInfo[] methods = Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
			return methods.Where(method => !method.IsSpecialName && this.storedProcedures.Any(p => p.Name == method.Name));
		}
	}
	class EFPropertyDescriptor : DevExpress.XtraReports.Native.PropertyDescriptorWrapper {
		readonly EFContextWrapper contextWrapper;
		public EFPropertyDescriptor(PropertyDescriptor pd, EFContextWrapper contextWrapper)
			: base(pd) {
			this.contextWrapper = contextWrapper;
		}
		public override object GetValue(object component) {
			return this.contextWrapper.GetPropertyValue(Name);
		}
	}
	class EFMethodDescriptor : PropertyDescriptor {
		readonly MethodInfo method;
		readonly EFContextWrapper contextWrapper;
		public override Type ComponentType { get { return this.contextWrapper.Type; } }
		public override bool IsReadOnly { get { return true; } }
		public override Type PropertyType {
			get {
				if(typeof(IEnumerable).IsAssignableFrom(this.method.ReturnType))
					return this.method.ReturnType;
				if(DataContext.IsComplexType(this.method.ReturnType))
					return typeof(IEnumerable<>).MakeGenericType(this.method.ReturnType);
				Type resultType = typeof(SimpleReturnTypeWrapper<>).MakeGenericType(this.method.ReturnType);
				return typeof(IEnumerable<>).MakeGenericType(resultType);
			}
		}
		public EFMethodDescriptor(MethodInfo method, EFContextWrapper contextWrapper)
			: base(method.Name, null) {
			this.method = method;
			this.contextWrapper = contextWrapper;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override object GetValue(object component) {
			return this.contextWrapper.GetMethodValue(this.method.Name);
		}
		public override void ResetValue(object component) {
		}
		public override void SetValue(object component, object value) {
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
	}
	class SimpleReturnTypeWrapper<T> {
		readonly T value;
		public T Value { get { return this.value; } }
		public SimpleReturnTypeWrapper(T value) {
			this.value = value;
		}
	}
}
