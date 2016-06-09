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
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.DC {
	public sealed class CustomLogics {
		internal readonly Dictionary<Type, List<Type>> registeredLogics;
		internal readonly Dictionary<Type, List<Type>> unregisteredLogics;
		int hashCode;
		public CustomLogics() {
			registeredLogics = new Dictionary<Type, List<Type>>();
			unregisteredLogics = new Dictionary<Type, List<Type>>();
		}
		internal CustomLogics Clone() {
			CustomLogics clone = new CustomLogics();
			foreach(KeyValuePair<Type, List<Type>> item in registeredLogics) {
				clone.registeredLogics[item.Key] = new List<Type>(item.Value);
			}
			foreach(KeyValuePair<Type, List<Type>> item in unregisteredLogics) {
				clone.unregisteredLogics[item.Key] = new List<Type>(item.Value);
			}
			clone.hashCode = hashCode;
			return clone;
		}
		internal Type GetInterfaceByLogic(Type logicType) {
			Guard.ArgumentNotNull(logicType, "logicType");
			foreach(KeyValuePair<Type, List<Type>> item in registeredLogics) {
				if(item.Value.Contains(logicType)) {
					return item.Key;
				}
			}
			return null;
		}
		public Type[] GetRegisteredLogics(Type forInterface) {
			Guard.ArgumentNotNull(forInterface, "forInterface");
			List<Type> result = new List<Type>();
			foreach(KeyValuePair<Type, List<Type>> item in registeredLogics) {
				if(forInterface.IsAssignableFrom(item.Key) || item.Key.IsAssignableFrom(forInterface)) {
					result.AddRange(item.Value);
				}
			}
			return result.ToArray();
		}
		private void GetLogicsLists(Type forInterface, out List<Type> registeredLogicsList, out List<Type> unregisteredLogicsList) {
			if(!registeredLogics.TryGetValue(forInterface, out registeredLogicsList) || !unregisteredLogics.TryGetValue(forInterface, out unregisteredLogicsList)) {
				registeredLogicsList = new List<Type>();
				unregisteredLogicsList = new List<Type>();
				registeredLogics[forInterface] = registeredLogicsList;
				unregisteredLogics[forInterface] = unregisteredLogicsList;
				hashCode ^= forInterface.GetHashCode();
			}
		}
		public void RegisterLogic(Type forInterface, Type logicType) {
			Guard.ArgumentNotNull(forInterface, "forInterface");
			Guard.ArgumentNotNull(logicType, "logicType");
			List<Type> registeredLogicsList;
			List<Type> unregisteredLogicsList;
			GetLogicsLists(forInterface, out registeredLogicsList, out unregisteredLogicsList);
			int logicTypeHashCode = logicType.GetHashCode();
			if(unregisteredLogicsList.Contains(logicType)) {
				unregisteredLogicsList.Remove(logicType);
				hashCode ^= logicTypeHashCode;
			}
			if(registeredLogicsList.Contains(logicType)) {
				registeredLogicsList.Remove(logicType);
				hashCode ^= logicTypeHashCode;
			}
			registeredLogicsList.Add(logicType);
			hashCode ^= logicTypeHashCode;
		}
		public void UnregisterLogic(Type forInterface, Type logicType) {
			Guard.ArgumentNotNull(forInterface, "forInterface");
			Guard.ArgumentNotNull(logicType, "logicType");
			List<Type> registeredLogicsList;
			List<Type> unregisteredLogicsList;
			GetLogicsLists(forInterface, out registeredLogicsList, out unregisteredLogicsList);
			int logicTypeHashCode = logicType.GetHashCode();
			if(registeredLogicsList.Contains(logicType)) {
				registeredLogicsList.Remove(logicType);
				hashCode ^= logicTypeHashCode;
			}
			if(!unregisteredLogicsList.Contains(logicType)) {
				unregisteredLogicsList.Add(logicType);
				hashCode ^= logicTypeHashCode;
			}
		}
		public bool IsRegisteredLogic(Type forInterface, Type logicType) {
			Guard.ArgumentNotNull(forInterface, "forInterface");
			Guard.ArgumentNotNull(logicType, "logicType");
			List<Type> list;
			return registeredLogics.TryGetValue(forInterface, out list) && list.Contains(logicType);
		}
		public bool IsUnregisteredLogic(Type forInterface, Type logicType) {
			Guard.ArgumentNotNull(forInterface, "forInterface");
			Guard.ArgumentNotNull(logicType, "logicType");
			List<Type> list;
			return unregisteredLogics.TryGetValue(forInterface, out list) && list.Contains(logicType);
		}
		private bool AreLogicsEquals(Dictionary<Type, List<Type>> logics1, Dictionary<Type, List<Type>> logics2) {
			if(logics1.Count != logics2.Count) {
				return false;
			}
			foreach(KeyValuePair<Type, List<Type>> item in logics1) {
				List<Type> list1 = item.Value;
				List<Type> list2;
				if(!logics2.TryGetValue(item.Key, out list2) || list1.Count != list2.Count) {
					return false;
				}
				for(int i = 0; i < list1.Count; ++i) {
					if(list1[i] != list2[i]) {
						return false;
					}
				}
			}
			return true;
		}
		public override bool Equals(object obj) {
			if(this != obj) {
				CustomLogics other = obj as CustomLogics;
				return other != null && AreLogicsEquals(registeredLogics, other.registeredLogics) && AreLogicsEquals(unregisteredLogics, other.unregisteredLogics);
			}
			return true;
		}
		public override int GetHashCode() {
			return hashCode;
		}
	}
}
