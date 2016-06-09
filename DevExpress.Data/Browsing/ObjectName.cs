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
using DevExpress.Data.Native;
namespace DevExpress.XtraReports.Native 
{
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, DataMember = {dataMember}, Name = {Name}, DisplayName = {DisplayName}}")]
#endif
	public class ObjectName {
		string displayName = string.Empty;
		string name = string.Empty;
		string dataMember = string.Empty;
		public string Name { get { return name; } 
		}
		public string FullName { get { return BindingHelper.JoinStrings(".", dataMember, name); } 
		}
		public string DisplayName { get { return displayName; } 
		}
		public ObjectName(string name, string displayName) : this(name, displayName, string.Empty) {
		}
		public ObjectName(string name, string displayName, string dataMember) {
			this.name = name;
			this.displayName = displayName;
			this.dataMember = dataMember;
		}
		public override bool Equals(object obj) {
			ObjectName objName = obj as ObjectName;
			if(objName != null)
				return name == objName.name && displayName == objName.displayName;
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
	}
	public class ObjectNameCollection : CollectionBase {
		public ObjectName this[int index] { get { return List[index] as ObjectName; } 
		}
		public void CopyFrom(ObjectNameCollection source) {
			Clear();
			AddRange(source);
		}
		public int Add(string name, string displayName) { 
			return Add(new ObjectName(name, displayName));
		}
		public void AddRange(ObjectNameCollection items) { 
			foreach(ObjectName item in items)
				Add(item);
		}
		public int Add(ObjectName item) { 
			if(List.Contains(item)) 
				return List.IndexOf(item);
			return List.Add(item);
		}
		public int IndexOf(string displayName) {
			for(int i=0; i < Count; i++) {
				if (this[i].DisplayName == displayName)
					return i;
			}
			return -1;
		}
		public int IndexOfByName(string name) {
			for (int i = 0; i < Count; i++) {
				if (this[i].Name == name)
					return i;
			}
			return -1;
		}
		public ObjectName GetItemByName(string name) {
			foreach(ObjectName item in this)
				if(item.Name == name)
					return item;
			return null;
		}
	}
	public class ObjectNameCollectionsSet : CollectionBase {
		public ObjectNameCollection this[int index] {
			get { 
				return List[index] as ObjectNameCollection; 
			} 
			set {
				InnerList[index] = value;
			}
		}
		public int Add(ObjectNameCollection collection) { 
			if(List.Contains(collection)) 
				return List.IndexOf(collection);
			return List.Add(collection);
		}
		public void AddRange(ObjectNameCollectionsSet c) { 
			foreach(ObjectNameCollection item in c)
				Add(item);
		}
	}
}
