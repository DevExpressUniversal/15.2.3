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
using DevExpress.Utils.Serializing;
namespace DevExpress.Utils.Serializing.Helpers {
	public interface IXtraSupportShouldSerialize {
		bool ShouldSerialize(string propertyName);
	}
	public interface IXtraSupportCreateContentPropertyValue {
		object Create(XtraItemEventArgs e);
	}
	public interface IXtraSupportDeserializeCollectionItem {
		void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e);
		object CreateCollectionItem(string propertyName, XtraItemEventArgs e);
	}
	public interface IXtraPropertyCollection : ICollection {
		XtraPropertyInfo this[string name] { get; }
		XtraPropertyInfo this[int index] { get; }
		bool IsSinglePass { get; }
		void AddRange(ICollection props);
		void Add(XtraPropertyInfo prop);
	}
	public interface IOneTypeObjectConverter {
		Type Type { get; }
		string ToString(object obj);
		object FromString(string str);
	}
	public interface IOneTypeObjectConverter2 : IOneTypeObjectConverter {
		bool CanConvertFromString(string str);
	}
	public interface IXtraSortableProperties {
		bool ShouldSortProperties();
	}
	public interface IXtraRootSerializationObject {
		SerializationInfo GetIndexByObject(string propertyName, object obj);
		object GetObjectByIndex(string propertyName, int index);
		void AfterSerialize();
	}
	[System.Runtime.InteropServices.ComVisible(false)]
	public interface IXtraSerializable2 {
		XtraPropertyInfo[] Serialize();
		void Deserialize(IList props);
	}
	public interface ICustomObjectConverter {
		bool CanConvert(Type type);
		string ToString(Type type, object obj);
		object FromString(Type type, string str);
		Type GetType(string typeName);
	}
	public interface IXtraPartlyDeserializable {
		void Deserialize(object rootObject, IXtraPropertyCollection properties);
	}
	public interface IXtraSupportShouldSerializeCollectionItem {
		bool ShouldSerializeCollectionItem(XtraItemEventArgs e);
	}
	public interface IXtraSupportDeserializeCollection {
		void BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e);
		bool ClearCollection(string propertyName, XtraItemEventArgs e);
		void AfterDeserializeCollection(string propertyName, XtraItemEventArgs e);
	}
	public interface IXtraSupportDeserializeCollectionItemEx : IXtraSupportDeserializeCollectionItem  {
		void RemoveCollectionItem(string propertyName, XtraSetItemIndexEventArgs e);
	}
	public interface IXtraSupportAfterDeserialize {
		void AfterDeserialize(XtraItemEventArgs e);
	}
	public interface IXtraNameSerializable {
		string NameToSerialize { get; }
	}
}
