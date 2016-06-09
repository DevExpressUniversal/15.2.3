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

using DevExpress.Utils.Serializing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraScheduler.Printing {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ResourceOptions : ICloneable {
		#region fields
		int resourcesPerPage = 1;
		bool printAllResourcesOnOnePage = true;
		ResourcesKind resourcesKind = ResourcesKind.All;
		ResourceBaseCollection customResourcesCollection = new ResourceBaseCollection();
		bool printCustomCollection = false;
		SchedulerGroupType groupType = SchedulerGroupType.None;
		bool useActiveViewGroupType = true;
		#endregion
		#region ResourcesPerPage
		[DefaultValue(1),
		XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int ResourcesPerPage {
			get { return resourcesPerPage; }
			set { resourcesPerPage = Math.Max(1, value); }
		}
		#endregion
		#region ResourcesKind
		[XtraSerializableProperty(),
		DefaultValue(ResourcesKind.All)]
		public ResourcesKind ResourcesKind { get { return resourcesKind; } set { resourcesKind = value; } }
		#endregion
		#region GroupType
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue),
		DefaultValue(SchedulerGroupType.None)]
		public SchedulerGroupType GroupType { get { return groupType; } set { groupType = value; } }
		#endregion
		#region UseActiveViewGroupType
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue),
		DefaultValue(true)]
		public bool UseActiveViewGroupType { get { return useActiveViewGroupType; } set { useActiveViewGroupType = value; } }
		#endregion
		#region PrintAllResourcesOnOnePage
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue),
		DefaultValue(true)]
		public bool PrintAllResourcesOnOnePage { get { return printAllResourcesOnOnePage; } set { printAllResourcesOnOnePage = value; } }
		#endregion
		#region PrintCustomCollection
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)
		]
		public bool PrintCustomCollection { get { return printCustomCollection; } set { printCustomCollection = value; } }
		#endregion
		#region CustomResourcesCollection
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)]
		public ResourceBaseCollection CustomResourcesCollection {
			get { return customResourcesCollection; }
		}
		#endregion
		public object Clone() {
			ResourceOptions result = new ResourceOptions();
			result.ResourcesPerPage = ResourcesPerPage;
			result.ResourcesKind = ResourcesKind;
			result.UseActiveViewGroupType = UseActiveViewGroupType;
			result.PrintAllResourcesOnOnePage = PrintAllResourcesOnOnePage;
			result.PrintCustomCollection = PrintCustomCollection;
			result.GroupType = GroupType;
			result.CustomResourcesCollection.AddRange(CustomResourcesCollection);
			return result;
		}
		public bool IsEqual(ResourceOptions resourceOptions) {
			if (resourceOptions.GroupType != GroupType ||
				resourceOptions.PrintAllResourcesOnOnePage != PrintAllResourcesOnOnePage ||
				resourceOptions.PrintCustomCollection != PrintCustomCollection ||
				resourceOptions.ResourcesKind != ResourcesKind ||
				resourceOptions.ResourcesPerPage != ResourcesPerPage ||
				resourceOptions.UseActiveViewGroupType != UseActiveViewGroupType)
				return false;
			return AreResourceCollectionEquals(resourceOptions.CustomResourcesCollection, customResourcesCollection);
		}
		public override bool Equals(object obj) {
			ResourceOptions resourceOptions = obj as ResourceOptions;
			if (resourceOptions == null)
				return false;
			return IsEqual(resourceOptions);
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		static bool AreResourceCollectionEquals(ResourceBaseCollection collection1, ResourceBaseCollection collection2) {
			if (collection1.Count != collection2.Count)
				return false;
			int count = collection1.Count;
			for (int i = 0; i < count; i++)
				if (!collection1.Contains(collection2[i]))
					return false;
			return true;
		}
	}
}
