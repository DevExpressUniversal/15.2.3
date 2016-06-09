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

using DevExpress.Web.Internal;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DevExpress.Data.IO;
namespace DevExpress.Web.Mvc {
	public class PageControlState {
		public PageControlState() {
			TabPages = new PageControlTabState[0];
		}
		public IEnumerable<PageControlTabState> TabPages { get; private set; }
		public PageControlTabState ActiveTab { get; private set; }
		internal static string SaveTabsInfo(TabPageCollection tabPages) {
			using(var stream = new MemoryStream())
			using(var writer = new TypedBinaryWriter(stream)) {
				writer.WriteObject(tabPages.Count);
				for(int i = 0; i < tabPages.Count; i++) {
					writer.WriteObject(tabPages[i].Index);
					writer.WriteObject(tabPages[i].Name);
					writer.WriteObject(tabPages[i].Text);
				}
				return Convert.ToBase64String(stream.ToArray());
			}
		}
		internal static PageControlState Load(string serializedTabsInfo, int activeTabIndex) {
			if(string.IsNullOrEmpty(serializedTabsInfo))
				return null;
			var state = new PageControlState();
			state.LoadTabsInfo(serializedTabsInfo);
			state.LoadActiveTabIndex(activeTabIndex);
			return state;
		}
		protected void LoadTabsInfo(string serializedGroupsInfo) {
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(serializedGroupsInfo)))
			using(TypedBinaryReader reader = new TypedBinaryReader(stream)) {
				int tabsCount = reader.ReadObject<int>();
				var tabsSate = new PageControlTabState[tabsCount];
				for(int i = 0; i < tabsCount; i++) {
					tabsSate[i] = new PageControlTabState();
					tabsSate[i].Index = reader.ReadObject<int>();
					tabsSate[i].Name = reader.ReadObject<string>();
					tabsSate[i].Text = reader.ReadObject<string>();
				}
				TabPages = tabsSate;
			}
		}
		protected void LoadActiveTabIndex(int activeTabIndex) {
			ActiveTab = TabPages.ElementAt(activeTabIndex);
		}
	}
	public class PageControlTabState {
		public int Index { get; protected internal set; }
		public string Text { get; protected internal set; }
		public string Name { get; protected internal set; }
		public bool ClientEnabled { get; protected internal set; }
		public bool ClientVisible { get; protected internal set; }
	}
}
