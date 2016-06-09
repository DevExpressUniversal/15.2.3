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

using DevExpress.Xpf.Bars.Native;
using System;
using System.Collections.Generic;
namespace DevExpress.Xpf.Bars {
	public class GlobalBarItemScopeKey {
		public string ScopeName { get; protected set; }
		public int ScopeId { get; protected set; }
		public string Name { get; protected set; }
		public GlobalBarItemScopeKey(string name) {
			Name = name;
		}
		public GlobalBarItemScopeKey(string scopeName, string name) {
			ScopeName = scopeName;
			Name = name;
		}
		public GlobalBarItemScopeKey(int scopeId, string name) {
			ScopeId = scopeId;
			Name = name;
		}
		public override bool Equals(object obj) {
			if(obj is string)
				return (string)obj == this.ToString();
			GlobalBarItemScopeKey key = obj as GlobalBarItemScopeKey;
			if(key != null) {
				if(key.Name != Name) return false;
				if(key.ScopeId != ScopeId) return false;
				if(key.ScopeName != ScopeName) return false;
				return true;
			}
			return false;
		}
		public override string ToString() {
			if(ScopeName != null)
				return ScopeName + ":" + Name;
			else
				return ScopeId.ToString() + ":" + Name;
		}
		public override int GetHashCode() {
			return ScopeId ^ (Name == null ? 0 : Name.GetHashCode()) ^ (ScopeName == null ? 0 : ScopeName.GetHashCode());
		}
	}
	public class GlobalBarItemScope {
		private static GlobalBarItemScope instanceCore;
		protected static GlobalBarItemScope Instance {
			get {
				if(instanceCore == null)
					instanceCore = new GlobalBarItemScope();
				return instanceCore;
			}
		}
		protected Dictionary<GlobalBarItemScopeKey, WeakReference> itemScope = new Dictionary<GlobalBarItemScopeKey, WeakReference>();
		protected WeakList<BarItemLinkBase> pendingItemLinks = new WeakList<BarItemLinkBase>();
		public static void AddBarItem(GlobalBarItemScopeKey key, BarItem item) {
			Instance.itemScope.Add(key, new WeakReference(item));
			LinkItem(key, item);
		}
		public static void LinkItem(GlobalBarItemScopeKey key, BarItem item) {
		}
	}
}
