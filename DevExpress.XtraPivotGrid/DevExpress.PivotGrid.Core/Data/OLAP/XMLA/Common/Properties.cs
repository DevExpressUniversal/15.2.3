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
namespace DevExpress.PivotGrid.Xmla {
	class XmlaProperty {
		readonly string name;
		readonly string value;
		readonly Type type;
		public XmlaProperty(string name, string value) 
			 : this(name, value, typeof(string)) {
		}
		public XmlaProperty(string name, string value, Type type) {
			this.name = name;
			this.value = value;
			this.type = type;
		}
		public string Name { get { return this.name; } }
		public string Value { get { return this.value; } }
		public Type Type { get { return this.type; } }
		public override string ToString() {
			return string.Format("{0} = {1} ({2})", Name, Value, Type);
		}
	}
	sealed class XmlaProperties : List<XmlaProperty> {
		Dictionary<string, XmlaProperty> dic;
		public XmlaProperties() {
			this.dic = new Dictionary<string, XmlaProperty>();
		}
		public new void Add(XmlaProperty property) {
			this.dic.Add(property.Name, property);
			base.Add(property);
		}
		public new void Clear() {
			base.Clear();
			this.dic.Clear();
		}
		public bool Contains(string name) {
			return this.dic.ContainsKey(name);
		}
		public new void Insert(int index, XmlaProperty property) {
			this.dic.Add(property.Name, property);
			base.Insert(index, property);
		}
		public new bool Remove(XmlaProperty property) {
			bool result = base.Remove(property) && this.dic.Remove(property.Name);
			return result;
		}
		public new void RemoveAt(int index) {
			if(index >= Count || index < 0) return;
			XmlaProperty property = this[index];
			if(this.dic.Remove(property.Name))
				base.RemoveAt(index);
		}
		public bool TryGetValue(string name, out XmlaProperty property) {
			return this.dic.TryGetValue(name, out property);
		}
		public XmlaProperty this[string name] {
			get {
				XmlaProperty value;
				if(!this.dic.TryGetValue(name, out value))
					return null;
				return value;
			}
			set {
				this.dic[name] = value;
			}
		}
		public new XmlaProperty this[int index] {
			get {
				return base[index];
			}
			set {
				XmlaProperty property = base[index];
				this.dic.Remove(property.Name);
				this.dic.Add(value.Name, value);
				base[index] = value;
			}
		}
	}
}
