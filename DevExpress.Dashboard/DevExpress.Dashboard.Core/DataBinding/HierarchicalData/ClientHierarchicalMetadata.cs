#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon.Native {
	public class ClientHierarchicalMetadata {
		HierarchicalMetadata metadata;
		public ClientHierarchicalMetadata(HierarchicalMetadata metadata) {
			this.metadata = metadata;
		}
		public Dictionary<string, DimensionDescriptorInternalCollection> DimensionDescriptors { get { return metadata.DimensionDescriptors; } }
		public MeasureDescriptorInternalCollection MeasureDescriptors { get { return metadata.MeasureDescriptors; } }
		public MeasureDescriptorInternalCollection ColorMeasureDescriptors { get { return metadata.ColorMeasureDescriptors; } }
		public MeasureDescriptorInternalCollection FormatConditionMeasureDescriptors { get { return metadata.FormatConditionMeasureDescriptors; } }
		public MeasureDescriptorInternalCollection NormalizedValueMeasureDescriptors { get { return metadata.NormalizedValueMeasureDescriptors; } }
		public MeasureDescriptorInternalCollection ZeroPositionMeasureDescriptors { get { return metadata.ZeroPositionMeasureDescriptors; } }
		public DeltaDescriptorInternalCollection DeltaDescriptors { get { return metadata.DeltaDescriptors; } }
		public string ColumnHierarchy { get { return metadata.ColumnHierarchy; } }
		public string RowHierarchy { get { return metadata.RowHierarchy; } }
		public List<string> DataSourceColumns { get { return metadata.DataSourceColumns; } }
		public DimensionDescriptorInternalCollection GetFields(string hierarchyId) {
			DimensionDescriptorInternalCollection collection;
			return metadata.DimensionDescriptors.TryGetValue(hierarchyId, out collection) ? collection : null;
		}
		public List<string> GetFieldNames(string hierarchyId) {
			List<string> names = new List<string>();
			DimensionDescriptorInternalCollection collection;
			if(metadata.DimensionDescriptors.TryGetValue(hierarchyId, out collection)) {
				names.AddRange(collection.Select<DimensionDescriptorInternal, string>(id => { return id.Name; }));
			}
			return names;
		}
		public string GetDimensionName(string hierarchyId, int level) {
			DimensionDescriptorInternalCollection collection;
			if(!metadata.DimensionDescriptors.TryGetValue(hierarchyId, out collection))
				return string.Empty;
			DimensionDescriptorInternal dimensionDescriptor = collection.Find((id) => { return id.Level == level; });
			return dimensionDescriptor != null ? dimensionDescriptor.Name : string.Empty;
		}
		public string GetDimensionUniqueName(string hierarchyId, string name) {
			DimensionDescriptorInternalCollection collection;
			if(!metadata.DimensionDescriptors.TryGetValue(hierarchyId, out collection))
				return string.Empty;
			DimensionDescriptorInternal dimensionDescriptor = collection.Find((id) => { return id.Name == name; });
			return dimensionDescriptor != null ? dimensionDescriptor.ID : string.Empty;
		}
		public int GetDimensionIndex(string hierarchyId, string name) {
			DimensionDescriptorInternalCollection collection;
			if(!metadata.DimensionDescriptors.TryGetValue(hierarchyId, out collection))
				return -1;
			DimensionDescriptorInternal dimensionDescriptor = collection.Find((id) => { return id.Name == name; });
			return dimensionDescriptor != null ? dimensionDescriptor.Level : -1;
		}
		public int GetDimensionIndex(string dimensionName) {
			foreach(DimensionDescriptorInternalCollection value in metadata.DimensionDescriptors.Values) {
				DimensionDescriptorInternal field = value.Find((id) => { return id.Name == dimensionName; });
				if(field != null)
					return field.Level;
			}
			return -1;
		}
		public string GetHierarchyId(string dimensionName) {
			foreach(KeyValuePair<string, DimensionDescriptorInternalCollection> pair in metadata.DimensionDescriptors) {
				DimensionDescriptorInternal field = pair.Value.Find((id) => { return id.Name == dimensionName; });
				if(field != null)
					return pair.Key;
			}
			return string.Empty;
		}
		public MeasureDescriptorInternalCollection GetMeasures() {
			return metadata.MeasureDescriptors;
		}
		public bool IsInternalInfoMeasure(string name) {
			return FormatConditionMeasureDescriptors.Any(md => md.ID == name)
				|| ColorMeasureDescriptors.Any(md => md.ID == name)
				|| NormalizedValueMeasureDescriptors.Any(md => md.ID == name)
				|| ZeroPositionMeasureDescriptors.Any(md => md.ID == name)
				|| name == DataStorageGenerator.DisplayTextStorageColumnName
				|| name == DataStorageGenerator.ValueStorageColumnName;
		}
		internal List<string> GetHierarchyNames() {
			return DimensionDescriptors.Keys.ToList<string>();
		}
	}
}
