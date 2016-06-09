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
using System.Collections;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Serializing;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class ObjectCache {
		ArrayList list = new ArrayList();
		Dictionary<object, SerializationInfo> dictionary;
		Dictionary<int, object> indicesDictionary = new Dictionary<int, object>();
		Dictionary<object, object> sharedObjects;
		int collectSharedObjectsLockCount = 0;
		bool CollectSharedObjects { get { return collectSharedObjectsLockCount == 0; } }
		public ICollection SharedObjectsCollection { get { return sharedObjects.Keys; } }
		public ICollection Collection { get { return list; } }
		public ObjectCache()
			: this(InstanceEqualityComparer.Instance) {
		}
		public ObjectCache(IEqualityComparer<object> equalityComparer) {
			dictionary = new Dictionary<object, SerializationInfo>(equalityComparer);
			sharedObjects = new Dictionary<object, object>(equalityComparer);
		}
		public void AddDeserializationObject(object obj, XtraItemEventArgs e) {
			AddDeserializationObject(obj, int.Parse(BrickFactory.GetStringProperty(e, SerializeHelper.IndexPropertyName)));
		}
		public void AddDeserializationObject(object obj, int index) {
			AddObject(obj, index);
		}
		public void AddSerializationObject(object obj) {
			AddObject(obj, SerializeHelper.UndefinedObjectIndex);
		}
		void AddObject(object obj, int index) {
			if(dictionary.ContainsKey(obj)) {
				AddSharedObject(obj);
				return;
			}
			int cacheIndex = list.Add(obj);
			if(index != SerializeHelper.UndefinedObjectIndex)
				cacheIndex = index; 
			dictionary.Add(obj, new SerializationInfo(cacheIndex));
			indicesDictionary.Add(cacheIndex, obj);
		}
		public SerializationInfo GetIndexByObject(object obj) {
			return dictionary[obj];
		}
		public object GetObjectByIndex(int index) {
			return indicesDictionary[index];
		}
		public void StopCollectSharedObjects() {
			collectSharedObjectsLockCount++;
		}
		public void StartCollectSharedObjects() {
			collectSharedObjectsLockCount--;
		}
		void AddSharedObject(object obj) {
			if(CollectSharedObjects && !sharedObjects.ContainsKey(obj))
				sharedObjects.Add(obj, null);
		}
	}
}
