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

using System.Runtime.Serialization;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.Data.XtraReports.ServiceModel.DataContracts {
	[DataContract]
	public class PropertyDescriptorProxy : IPropertyDescriptor {
		[DataMember]
		public string DisplayName { get; set; }
		[DataMember]
		public bool IsComplex { get; set; }
		[DataMember]
		public bool IsListType { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string ToStringValue { get; set; }
		[DataMember]
		public TypeSpecifics Specifics { get; set; }
		public PropertyDescriptorProxy() {
		}
		public PropertyDescriptorProxy(TypeSpecifics specifics) {
			Specifics = specifics;
		}
		public static PropertyDescriptorProxy CreateFrom(IPropertyDescriptor from) {
			return new PropertyDescriptorProxy() {
				DisplayName = from.DisplayName,
				IsComplex = from.IsComplex,
				IsListType = from.IsComplex,
				Specifics = from.Specifics,
				Name = from.Name,
				ToStringValue = from.ToString()
			};
		}
		public override string ToString() {
			return ToStringValue;
		}
	}
}
