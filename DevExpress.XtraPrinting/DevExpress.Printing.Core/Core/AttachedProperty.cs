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
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Utils.StoredObjects;
namespace DevExpress.XtraPrinting {
	public abstract class AttachedPropertyBase {
		#region // inner classes
		class FromNameKey {
			string name;
			Type ownerType;
			int hashCode;
			public FromNameKey(string name, Type ownerType) {
				this.name = name;
				this.ownerType = ownerType;
				this.hashCode = name.GetHashCode() ^ ownerType.GetHashCode();
			}
			public override int GetHashCode() {
				return this.hashCode;
			}
			public override bool Equals(object o) {
				return o is FromNameKey && EqualsCore((FromNameKey)o);
			}
			bool EqualsCore(FromNameKey key) {
				return name.Equals(key.name) && ownerType == key.ownerType;
			}
		}
		#endregion
		#region // static
		static Int16 globalIndex;
		static object synch = new object();
		static Dictionary<FromNameKey, AttachedPropertyBase> registeredProps = new Dictionary<FromNameKey, AttachedPropertyBase>();
		static List<AttachedPropertyBase> indexedProps = new List<AttachedPropertyBase>();
		public static AttachedPropertyBase GetProperty(int propertyIndex) {
			lock(synch) {
				return indexedProps[propertyIndex];
			}
		}
		public static AttachedProperty<T> Register<T>(string name, Type ownerType) {
			FromNameKey key = new FromNameKey(name, ownerType);
			lock(synch) {
				AttachedProperty<T> prop = new AttachedProperty<T>(name);
				registeredProps.Add(key, prop);
				indexedProps.Add(prop);
				if(indexedProps.Count - 1 != prop.Index)
					throw new InvalidOperationException();
				return prop;
			}
		}
		static Int16 GetGlobalIndex() {
			if(globalIndex < Int16.MaxValue)
				return globalIndex++;
			throw new InvalidOperationException();
		}
		#endregion
		public string Name { get; private set; }
		public Int16 Index { get; private set; }
		public abstract Type PropertyType { get; }
		protected AttachedPropertyBase(string name) {
			Name = name;
			lock(synch)
				Index = GetGlobalIndex();
		}
	}
	public sealed class AttachedProperty<T> : AttachedPropertyBase {
		public override Type PropertyType { get { return typeof(T); } }
		internal AttachedProperty(string name)
			: base(name) {
		}
	}
	struct AttachedPropertyValue {
		object value;
		Int16 propertyIndex;
		public Int16 PropertyIndex { get { return propertyIndex; } private set { propertyIndex = value; } }
		public object Value { get { return value; } set { this.value = value; } }
		public AttachedPropertyValue(Int16 propertyIndex) {
			value = null;
			this.propertyIndex = propertyIndex;
		}
	}
}
