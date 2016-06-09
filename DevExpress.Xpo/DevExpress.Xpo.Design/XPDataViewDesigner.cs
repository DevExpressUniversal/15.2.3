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
using DevExpress.Utils.Design;
using System.ComponentModel.Design;
using DevExpress.Xpo.Metadata;
using System.ComponentModel;
using DevExpress.Xpo.Metadata.Helpers;
using DevExpress.XtraEditors;
using System.Windows.Forms;
namespace DevExpress.Xpo.Design {
	public class XPDataViewDesigner : BaseComponentDesigner {
		public XPDataView View { get { return this.Component as XPDataView; } }
		protected override void RegisterActionLists(DesignerActionListCollection list) {
			if(AllowDesigner)
				list.Add(new XPDataViewActionList(this));
			base.RegisterActionLists(list);
		}
		public class XPDataViewActionList : DesignerActionList, IXPDictionaryProvider {
			XPDataViewDesigner designer;
			public XPDataViewActionList(XPDataViewDesigner designer)
				: base(designer.Component) {
				this.designer = designer;
			}
			public XPDataView View { get { return designer.View; } }
			public override DesignerActionItemCollection GetSortedActionItems() {
				DesignerActionItemCollection res = new DesignerActionItemCollection();				
				res.Add(new DesignerActionPropertyItem("ObjectClassInfo", "ObjectClassInfo"));
				if(designer.View != null) {
					res.Add(new DesignerActionMethodItem(this, "PopulateProperties", "Populate Properties"));
				}
				return res;
			}
			public void PopulateProperties() {
				if(View != null && ObjectClassInfo != null) {
					if(XtraMessageBox.Show("The Properties collection will be cleared and then populated with persistent properties, based on the specified metadata information.\r\nDo you want to continue?",
						"XPDataView", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question) == DialogResult.Yes) {
						try {
							View.PopulateProperties(ObjectClassInfo);
						} catch(Exception ex) {
							XtraMessageBox.Show(string.Format("Unable to populate properties. Error: {0}", ex.Message), "XPDataView", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
						}
						EditorContextHelper.FireChanged(designer, View);
					}
				}
			}
			XPDictionary designDictionary;
			internal XPDictionary DesignDictionary {
				get {
					if(designDictionary == null) {
						designDictionary = new DesignTimeReflection(View.Site);
					}
					return designDictionary;
				}
			}
			XPClassInfo objectClassInfo = null;
			[TypeConverter(typeof(DevExpress.Xpo.Design.ObjectClassInfoTypeConverter))]
			public XPClassInfo ObjectClassInfo {
				get {
					return objectClassInfo;
				}
				set {
					if(objectClassInfo != value) {
						objectClassInfo = value as XPClassInfo;
					}
				}
			}
			public XPDictionary Dictionary {
				get { return DesignDictionary; }
			}
		}
	}
}
