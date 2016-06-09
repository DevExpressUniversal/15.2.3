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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.ViewerData {
	public class DimensionDescriptorCollection : ReadOnlyCollection<DimensionDescriptor> {
		internal DimensionDescriptorCollection(IList<DimensionDescriptor> list) : base(list) { }
	}
	public class MeasureDescriptorCollection : ReadOnlyCollection<MeasureDescriptor> {
		internal MeasureDescriptorCollection(IList<MeasureDescriptor> list) : base(list) { }
	}
	public class DeltaDescriptorCollection : ReadOnlyCollection<DeltaDescriptor> {
		internal DeltaDescriptorCollection(IList<DeltaDescriptor> list) : base(list) { }
	}
	public class MeasureDescriptor {
		MeasureDescriptorInternal internalDescriptor;
		FormatterBase formatter;
		internal MeasureDescriptor(MeasureDescriptorInternal internalDescriptor) {
			this.internalDescriptor = internalDescriptor;
			this.formatter = FormatterBase.CreateFormatter(internalDescriptor.Format);
		}
		public string Name { get { return internalDescriptor.Name; } }
		public string ID { get { return internalDescriptor.ID; } }
		public SummaryType SummaryType { get { return internalDescriptor.SummaryType; } }
		public string DataMember { get { return internalDescriptor.DataMember; } }
		public string Format(object value) { return formatter.Format(value); }
		internal MeasureDescriptorInternal InternalDescriptor { get { return internalDescriptor; } }
	}
	public class DimensionDescriptor {
		DimensionDescriptorInternal internalDescriptor;
		FormatterBase formatter;
		internal DimensionDescriptor(DimensionDescriptorInternal internalDescriptor) {
			this.internalDescriptor = internalDescriptor;
			this.formatter = FormatterBase.CreateFormatter(internalDescriptor.Format);
		}
		public string Name { get { return internalDescriptor.Name; } }
		public string ID { get { return internalDescriptor.ID; } }
		public DateTimeGroupInterval DateTimeGroupInterval { get { return internalDescriptor.DateTimeGroupInterval; } }
		public TextGroupInterval TextGroupInterval { get { return internalDescriptor.TextGroupInterval; } }
		public string DataMember { get { return internalDescriptor.DataMember; } }
		public string Format(object value) { return formatter.Format(value); }
		internal DimensionDescriptorInternal InternalDescriptor { get { return internalDescriptor; } }
	}
	public class DeltaDescriptor {
		DeltaDescriptorInternal internalDescriptor;
		internal DeltaDescriptor(DeltaDescriptorInternal internalDescriptor) {
			this.internalDescriptor = internalDescriptor;
		}
		public string Name { get { return internalDescriptor.Name; } }
		public string ID { get { return internalDescriptor.ID; } }
		public string ActualMeasureID { get { return internalDescriptor.ActualMeasureID; } }
		public string TargetMeasureID { get { return internalDescriptor.TargetMeasureID; } }
		internal DeltaDescriptorInternal InternalDescriptor { get { return internalDescriptor; } }
	}
}
