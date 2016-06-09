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

using DevExpress.Data;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Editors.Internal {
	public class SparklineDataClient : IDataControllerData2 {
		const string ArgumentColumn = "ArgumentColumn";
		const string ValueColumn = "ValueColumn";
		IItemsSourceSupport ItemsSourceSettings { get; set; }
		public SparklinePropertyDescriptorBase ArgumentColumnDescriptor { get; private set; }
		public SparklinePropertyDescriptorBase ValueColumnDescriptor { get; private set; }
		public SparklineDataClient(IItemsSourceSupport itemsSourceSettings) {
			ItemsSourceSettings = itemsSourceSettings;
		}
		#region IDataControllerData Members
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			return null;
		}
		object IDataControllerData.GetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
			return null;
		}
		void IDataControllerData.SetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
		}
		#endregion
		#region IDataControllerData2 Members
		bool IDataControllerData2.CanUseFastProperties { get { return false; } }
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter { get { return false; } }
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) {
			return null;
		}
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			var descriptors = new List<PropertyDescriptor>();
			foreach (PropertyDescriptor item in collection)
				descriptors.Add(item);
			ArgumentColumnDescriptor = GetArgumentDescriptor();
			ValueColumnDescriptor = GetValueDescriptor();
			AddDescriptor(descriptors, ArgumentColumnDescriptor);
			AddDescriptor(descriptors, ValueColumnDescriptor);
			return new PropertyDescriptorCollection(descriptors.ToArray());
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			return null;
		}
		#endregion
		void AddDescriptor(List<PropertyDescriptor> descriptors, PropertyDescriptor newDescriptor) {
			int descriptorIndex = descriptors.FindIndex((propertyDescriptor) => propertyDescriptor.Name == newDescriptor.Name);
			if (descriptorIndex > -1)
				descriptors[descriptorIndex] = newDescriptor;
			else
				descriptors.Add(newDescriptor);
		}
		SparklinePropertyDescriptorBase GetArgumentDescriptor() {
			string descriptorName = string.IsNullOrEmpty(ItemsSourceSettings.PointArgumentMember) ? ArgumentColumn : ItemsSourceSettings.PointArgumentMember;
			SparklinePropertyDescriptorBase sparklineDescriptor = ArgumentColumnDescriptor as SparklinePropertyDescriptorBase;
			if (sparklineDescriptor != null && sparklineDescriptor.IsRelevant(descriptorName))
				return sparklineDescriptor;
			return new SparklinePropertyDescriptor(descriptorName, ItemsSourceSettings.PointArgumentMember);
		}
		SparklinePropertyDescriptorBase GetValueDescriptor() {
			string descriptorName = string.IsNullOrEmpty(ItemsSourceSettings.PointValueMember) ? ValueColumn : ItemsSourceSettings.PointValueMember;
			SparklinePropertyDescriptorBase sparklineDescriptor = ValueColumnDescriptor as SparklinePropertyDescriptorBase;
			if (sparklineDescriptor != null && sparklineDescriptor.IsRelevant(descriptorName))
				return sparklineDescriptor;
			return new SparklinePropertyDescriptor(descriptorName, ItemsSourceSettings.PointValueMember);
		}
		public void ResetDescriptors() {
			if (ArgumentColumnDescriptor != null)
				ArgumentColumnDescriptor.Reset();
			if (ValueColumnDescriptor != null)
				ValueColumnDescriptor.Reset();
		}
	}
}
