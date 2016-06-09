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
using System.Linq;
using System.Text;
using System.ComponentModel.Design;
using DevExpress.Utils.Design;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.XtraEditors;
using System.Windows.Forms;
using DevExpress.Data;
using System.ComponentModel;
namespace DevExpress.Xpo.Design {
	public class RealtimeSourceDesigner : BaseComponentDesigner {
		public RealTimeSource View { get { return this.Component as RealTimeSource; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if(AllowDesigner)
				list.Add(new RealtimeActionList(this));
			base.RegisterActionLists(list);
		}
		public class RealtimeActionList : DesignerActionList {
			RealtimeSourceDesigner designer;
			public RealtimeActionList(RealtimeSourceDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			public RealTimeSource View { get { return designer.View; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionPropertyItem("DataSource", "DataSource"));
				if(designer.View != null) {
					res.Add(new DesignerActionMethodItem(this, "PopulateDisplayableProperties", "Populate DisplayableProperties"));
				}
				return res;
			}
			public void PopulateDisplayableProperties() {
				if(View != null && DataSource != null) {
					if(XtraMessageBox.Show("The DisplayableProperties collection will be cleared and then populated with properties, based on the specified metadata information.\r\nDo you want to continue?",
						"XPDataView", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == DialogResult.Yes) {
						try {
							IEnumerable<string> properies = RealTimeSource.GetDisplayableProperties(DataSource);
							if(properies != null)
								View.DisplayableProperties = string.Join(";", properies.ToArray());
						} catch(Exception ex) {
							XtraMessageBox.Show(string.Format("Unable to populate DisplayableProperties. Error: {0}", ex.Message), "RealTimeSource", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
						}
						EditorContextHelper.FireChanged(designer, View);
					}
				}
			}
			[AttributeProvider(typeof(IListSource)), DefaultValue(null), Category("Data")]
			public object DataSource {
				get { return View != null ? View.DataSource : null; }
				set {
					if(View == null)
						return;
					if(View.DataSource != value) {
						View.DataSource = value;
					}
				}
			}
		}
	}
}
