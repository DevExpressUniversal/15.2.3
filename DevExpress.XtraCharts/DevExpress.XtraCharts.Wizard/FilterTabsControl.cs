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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTab;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class FilterTabsControl : ValidateControl {
		public virtual XtraTabControl TabControl { get { throw new Exception("The method or operation is not implemented."); } }
		internal object SelectedTabHandle {
			get {
				if (TabControl == null || TabControl.SelectedTabPage == null)
					return null;
				return TabControl.SelectedTabPage.Tag;
			}
			set {
				if(TabControl != null)
					TabControl.SelectedTabPageIndex = GetSelectedTabIndex(value); 
			} 
		}	
		public FilterTabsControl() {
			InitializeComponent();
		}
		protected void InitializeCore(UserLookAndFeel lookAndFeel, CollectionBase filter, object selectedTabHandle) {			
			InitializeTabs();
			InitializeTags();
			HideInvisibleTabs(new ArrayList(filter));
			int selectedIndex = selectedTabHandle != null ? GetSelectedTabIndex(selectedTabHandle) : 0;
			if (TabControl != null && TabControl.TabPages.Count > 0)
				TabControl.SelectedTabPageIndex = selectedIndex;
			Initialize(lookAndFeel);
		}	
		protected virtual void Initialize(UserLookAndFeel lookAndFeel) {
		}
		protected virtual void InitializeTags() {
		}
		protected virtual void InitializeTabs() {
		}
		void HideInvisibleTabs(ArrayList list) {
			if(TabControl != null)
				for(int i = TabControl.TabPages.Count - 1; i >= 0; i--) {
					XtraTabPage page = TabControl.TabPages[i];
					if(page.Tag == null)
						continue;
					if(list.Contains(page.Tag))
						TabControl.TabPages.Remove(page);
				}
		}
		int GetSelectedTabIndex(object handle) {
			if(TabControl != null) {
				int index = 0;
				foreach(XtraTabPage page in TabControl.TabPages) {
					if(page.Tag != null && page.Tag.Equals(handle))
						return index;
					index++;
				}
			}
			return 0;
		}
	}
}
