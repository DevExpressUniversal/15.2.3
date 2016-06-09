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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraTreeList.Design;
using DevExpress.Utils.Design;
using DevExpress.Utils;
using DevExpress.XtraScheduler.UI;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal;
namespace DevExpress.XtraScheduler.Design {
	public partial class ResourceTreeDesignerForm : frmEditor {
		public ResourceTreeDesignerForm() {
		}
		protected override void CreateDesigner() {
			ActiveDesigner = new ResourceTreeDesigner1();
		}
		protected override string RegistryStorePath { get { return "Software\\Developer Express\\Designer\\XtraScheduler\\"; } }
		protected override void InitFrame(string caption, Bitmap bitmap) {
			base.InitFrame(caption, bitmap);
		}
		protected override Type ResolveType(string type) {
			Type t = typeof(ResourceTreeDesignerForm).Assembly.GetType(type);
			if (t != null) return t;
			return base.ResolveType(type);
		}
	}
	public class ResourceTreeDesigner1 : TListDesigner {
		protected override void CreateGroups() {
			Groups.Clear();
			DesignerGroup group;
			group = Groups.Add("Main", "Main settings.", null, true);
			group.Add("Columns", 
				"Adjust the Column collection of the current ResourceTree, assign in-place editors to columns and specify total summaries.",
				"DevExpress.XtraScheduler.Design.Frames.ResourceTreeColumnDesigner", 
				GetDefaultLargeImage(0), 
				GetDefaultSmallImage(0));
			group.Add("In-place Editor Repository", "Adjust the editors used for in-place editing.", "DevExpress.XtraTreeList.Frames.PersistentRepositoryTreeListEditor", GetDefaultLargeImage(1), GetDefaultSmallImage(1));
		}
	}
}
namespace DevExpress.XtraScheduler.Design.Frames {
	[DXToolboxItem(false)]
	public class ResourceTreeColumnDesigner : DevExpress.XtraTreeList.Frames.ColumnDesigner {
		public ResourceTreeColumnDesigner() {
		}
		public ResourcesTree ResourcesTree { get { return EditingObject as ResourcesTree; } }
		public override DevExpress.XtraTreeList.TreeList TreeList { get { return ResourcesTree; } }
		protected override Component ColumnsOwner { get { return ResourcesTree; } }
		protected override System.Collections.CollectionBase Columns {
			get {
				return ResourcesTree != null ? ResourcesTree.Columns : null;
			}
		}
		protected override string[] GetDataFieldList() {
			if (ResourcesTree.SchedulerControl == null)
				return new string[0];
			ISchedulerStorageBase storage = ResourcesTree.SchedulerControl.DataStorage;
			if (storage == null)
				return new string[0];
			IResourceStorageBase resources = storage.Resources;
			ResourceMappingInfo mappings = (ResourceMappingInfo)resources.Mappings;
			List<string> fieldNames = new List<string>();
			AddDataFieldFromMappings(fieldNames, mappings.Caption, ResourceSR.Caption);
			AddDataFieldFromMappings(fieldNames, mappings.Color, ResourceSR.Color);
			AddDataFieldFromMappings(fieldNames, mappings.Id, ResourceSR.Id);
			AddDataFieldFromMappings(fieldNames, mappings.Image, ResourceSR.Image);
			AddDataFieldFromMappings(fieldNames, mappings.ParentId, ResourceSR.ParentId);
			ResourceCustomFieldMappingCollection customMappings = (ResourceCustomFieldMappingCollection)resources.CustomFieldMappings;
			for (int i = 0; i < customMappings.Count; i++) {
				string customFieldName = customMappings[i].Name;
				if (!String.IsNullOrEmpty(customFieldName))
					fieldNames.Add(customFieldName);
			}
			return fieldNames.ToArray();
		}
		void AddDataFieldFromMappings(List<string> fieldNames, string mapping, string fieldName) {
			fieldNames.Add( String.IsNullOrEmpty(mapping) ? fieldName : mapping);
		}
	}
}
