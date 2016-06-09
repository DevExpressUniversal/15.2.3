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
using System.Collections.ObjectModel;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.Model {
	public sealed class ModelInterfaceExtenders {
		private static readonly ReadOnlyCollection<Type> EmptyCollection = new ReadOnlyCollection<Type>(Type.EmptyTypes);
		private readonly Dictionary<Type, List<Type>> extenders;
		private int hashCode;
		public ModelInterfaceExtenders() {
			extenders = new Dictionary<Type, List<Type>>();
		}
		public void Add(Type targetInterface, Type extenderInterface) {
			CheckArgumentIsInterface(targetInterface, "targetInterface");
			CheckArgumentIsInterface(extenderInterface, "extenderInterface");
			List<Type> list;
			if(!extenders.TryGetValue(targetInterface, out list)) {
				list = new List<Type>();
				extenders.Add(targetInterface, list);
				hashCode ^= targetInterface.GetHashCode();
			}
			if(!list.Contains(extenderInterface)) {
				list.Add(extenderInterface);
				hashCode ^= extenderInterface.GetHashCode();
			}
		}
		private static void CheckArgumentIsInterface(Type argumentValue, string argumentName) {
			Guard.ArgumentNotNull(argumentValue, argumentName);
			if(!argumentValue.IsInterface) {
				throw new ArgumentException(string.Format("Type '{0}' is not an interface.", argumentValue.FullName), argumentName);
			}
		}
		public void Add<TargetInterface, ExtenderInterface>() {
			Add(typeof(TargetInterface), typeof(ExtenderInterface));
		}
		public ReadOnlyCollection<Type> GetExtendedInterfaces() {
			return new List<Type>(extenders.Keys).AsReadOnly();
		}
		public ReadOnlyCollection<Type> GetInterfaceExtenders(Type targetInterface) {
			Guard.ArgumentNotNull(targetInterface, "targetInterface");
			List<Type> list;
			return extenders.TryGetValue(targetInterface, out list) ? list.AsReadOnly() : EmptyCollection;
		}
		public ModelInterfaceExtenders Clone() {
			ModelInterfaceExtenders clone = new ModelInterfaceExtenders();
			foreach(KeyValuePair<Type, List<Type>> item in extenders) {
				clone.extenders[item.Key] = new List<Type>(item.Value);
			}
			clone.hashCode = hashCode;
			return clone;
		}
		public override int GetHashCode() {
			return hashCode;
		}
		public override bool Equals(object obj) {
			if(this != obj) {
				ModelInterfaceExtenders other = obj as ModelInterfaceExtenders;
				return other != null && AreCollectionsEqual(extenders, other.extenders);
			}
			return true;
		}
		private static bool AreCollectionsEqual(Dictionary<Type, List<Type>> collection1, Dictionary<Type, List<Type>> collection2) {
			if(collection1.Count != collection2.Count) {
				return false;
			}
			foreach(KeyValuePair<Type, List<Type>> item in collection1) {
				List<Type> list1 = item.Value;
				List<Type> list2;
				if(!collection2.TryGetValue(item.Key, out list2) || list1.Count != list2.Count) {
					return false;
				}
				foreach(Type type in list1) {
					if(!list2.Contains(type)) {
						return false;
					}
				}
			}
			return true;
		}
	}
}
