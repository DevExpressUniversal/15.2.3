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
using System.Linq;
using System.Text;
namespace DevExpress.Diagram.Core.Serialization {
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public sealed class CustomSerializableObjectAttribute : Attribute {
		readonly string objectId;
		public CustomSerializableObjectAttribute(string objectId) {
			this.objectId = objectId;
		}
		public string ObjectId { get { return objectId; } }
		public static string GetObjectId(object obj) { return GetObjectId(obj.GetType()); }
		public static string GetObjectId(Type objectType) {
			CustomSerializableObjectAttribute attr = GetCustomAttribute(objectType, typeof(CustomSerializableObjectAttribute)) as CustomSerializableObjectAttribute;
			return attr != null ? attr.ObjectId : string.Empty;
		}
	}
	public interface ICustomSerializableObject {
		Type ObjectType { get; }
		string ObjectId { get; }
		object Create();
	}
	public class CustomSerializableObjectRegistrator {
		readonly Dictionary<string, ICustomSerializableObject> objects;
		internal CustomSerializableObjectRegistrator() {
			this.objects = new Dictionary<string, ICustomSerializableObject>();
		}
		public void Register(ICustomSerializableObject objectInfo) {
			string objectId = CustomSerializableObjectAttribute.GetObjectId(objectInfo.ObjectType);
			this.objects.Add(objectId, objectInfo);
		}
		public void Unregister(Type type) {
			string objectId = CustomSerializableObjectAttribute.GetObjectId(type);
			this.objects.Remove(objectId);
		}
		public bool SupportsId(string objectId) {
			return this.objects.ContainsKey(objectId);
		}
		public bool SupportsObject(Type type) {
			string objectId = CustomSerializableObjectAttribute.GetObjectId(type);
			if(string.IsNullOrEmpty(objectId)) return false;
			return SupportsId(objectId);
		}
		public bool SupportsObject(object obj) {
			return SupportsObject(obj.GetType());
		}
		public ICustomSerializableObject this[object obj] {
			get {
				string objectId = CustomSerializableObjectAttribute.GetObjectId(obj);
				return this[objectId];
			}
		}
		public ICustomSerializableObject this[string objectId] { get { return this.objects[objectId]; } }
		#region Instance
		public static readonly CustomSerializableObjectRegistrator Instance = new CustomSerializableObjectRegistrator(); 
		#endregion
	}
	public class CustomSerializableObject<T> : ICustomSerializableObject where T : class {
		readonly Type objectType;
		readonly string objectId;
		readonly Func<T> factory;
		public CustomSerializableObject(Type objectType, Func<T> factory) {
			this.objectType = objectType;
			this.factory = factory;
			this.objectId = CustomSerializableObjectAttribute.GetObjectId(ObjectType);
			if(string.IsNullOrEmpty(this.objectId)) {
				throw new ArgumentException("objectType");
			}
		}
		public T Create() {
			return factory();
		}
		public Type ObjectType { get { return objectType; } }
		public string ObjectId { get { return objectId; } }
		#region ICustomSerializableObject
		object ICustomSerializableObject.Create() {
			return Create();
		}
		#endregion
	}
}
