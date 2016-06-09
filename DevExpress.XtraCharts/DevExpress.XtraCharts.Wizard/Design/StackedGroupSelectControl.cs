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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Design {
	public partial class SeriesGroupSelectControl : XtraUserControl {
		readonly IWindowsFormsEditorService edSvc;
		object currentGroup;
		bool loading = false;
		bool shouldClose = false;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public object CurrentGroup { get { return currentGroup; } }
		public SeriesGroupSelectControl() { } 
		public SeriesGroupSelectControl(IWindowsFormsEditorService edSvc, SeriesGroupWrappers wrappers, object currentGroup) {
			this.edSvc = edSvc;
			this.currentGroup = currentGroup;
			InitializeComponent();
			loading = true;
			try {
				foreach (SeriesGroupWrapper wrapper in wrappers)
					lbGroups.Items.Add(wrapper);
				lbGroups.SelectedItem = wrappers.GetWrapperByGroup(currentGroup);
			}
			finally {
				loading = false;
			}
		}
		void lbGroups_SelectedIndexChanged(object sender, EventArgs e) {
			if (!loading) {
				currentGroup = ((SeriesGroupWrapper)lbGroups.SelectedValue).Group;
				if (shouldClose)
					edSvc.CloseDropDown();
			}
		}
		void lbGroups_MouseClick(object sender, MouseEventArgs e) {
			if (e.Button == MouseButtons.Left)
				shouldClose = true;
		}
	}
	public class SeriesGroupWrapper {
		string name;
		readonly object group;
		public string Name { get { return name; } set { name = value; } }
		public object Group { get { return group; } }
		public SeriesGroupWrapper(string name, object group) {
			this.name = name;
			this.group = group;
		}
		public override string ToString() {
			return name;
		}
	}
	public class SeriesGroupWrappers : List<SeriesGroupWrapper> {
		static readonly string namePrefix = ChartLocalizer.GetString(ChartStringId.StackedGroupPrefix);
		bool ContainsGroup(object group) {
			return GetWrapperByGroup(group) != null;
		}
		bool ContainsName(string name) {
			return GetWrapperByName(name) != null;
		}
		public SeriesGroupWrapper GetWrapperByGroup(object group) {
			foreach (SeriesGroupWrapper wrapper in this)
				if (object.Equals(wrapper.Group, group))
					return wrapper;
			return null;
		}
		public SeriesGroupWrapper GetWrapperByName(string name) {
			foreach (SeriesGroupWrapper wrapper in this)
				if (wrapper.Name == name)
					return wrapper;
			return null;
		}
		public void AddGroup(object group) {
			if (group != null && !ContainsGroup(group)) {
				string name = group.ToString();
				if (string.IsNullOrEmpty(name))
					name = namePrefix;
				int nameIndex = 1;
				string originalName = name;
				while (ContainsName(name)) {
					name = originalName + " " + nameIndex.ToString();
					nameIndex++;
				}
				Add(new SeriesGroupWrapper(name, group));
			}
		}
	}
}
