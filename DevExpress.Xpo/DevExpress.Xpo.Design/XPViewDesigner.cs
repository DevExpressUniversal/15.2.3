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
using System.Text;
using System.ComponentModel.Design;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.Xpo.Metadata;
using DevExpress.XtraEditors;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Design;
using System.ComponentModel;
using System.Windows.Forms;
namespace DevExpress.Xpo.Design {
	public class XPViewDesigner: SessionOwnerDesigner {
		public XPView View { get { return this.Component as XPView; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if(AllowDesigner)
				list.Add(new XPViewActionList(this));
			base.RegisterActionLists(list);
		}
		public class XPViewActionList : DesignerActionList, IXPDictionaryProvider {
			XPViewDesigner designer;
			public XPViewActionList(XPViewDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			public XPView View { get { return designer.View; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();
				res.Add(new DesignerActionPropertyItem("ObjectClassInfo", "ObjectClassInfo"));
				if(designer.View != null) {
					res.Add(new DesignerActionMethodItem(this, "PopulateProperties", "Populate Properties"));
				}
				return res;
			}
			public void PopulateProperties() {
				if(View != null && View.ObjectClassInfo != null) {
					if(XtraMessageBox.Show("The Properties collection will be cleared and then populated with persistent properties, based on the specified metadata information.\r\nDo you want to continue?",
								"XPView", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == DialogResult.Yes) {
						try {
							View.Properties.Clear();
							foreach(XPMemberInfo mi in View.ObjectClassInfo.PersistentProperties) {
								DisplayNameAttribute dnAttribute = mi.FindAttributeInfo(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
								string name = dnAttribute == null ? mi.Name : dnAttribute.DisplayName;
								View.AddProperty(name, new OperandProperty(mi.Name));
							}
						} catch(Exception ex) {
							XtraMessageBox.Show(string.Format("Unable to populate properties. Error: {0}", ex.Message), "XPView", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
						}
						EditorContextHelper.FireChanged(designer, View);
					}
				}
			}
			[TypeConverter(typeof(DevExpress.Xpo.Design.ObjectClassInfoTypeConverter))]
			public XPClassInfo ObjectClassInfo {
				get { return View.ObjectClassInfo; }
				set { EditorContextHelper.SetPropertyValue(designer, View, "ObjectClassInfo", value); }
			}
			public XPDictionary Dictionary {
				get { return ((IXPDictionaryProvider)View).Dictionary; }
			}
		}
	}
}
