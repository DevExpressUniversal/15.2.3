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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
#if !SILVERLIGHT && !WINRT
using System.ComponentModel.Design.Serialization;
#endif
#if WINRT
using Browsable = DevExpress.Mvvm.Native.BrowsableAttribute;
#endif
namespace DevExpress.XtraReports.Parameters {
	[DataContract]
	public class LookUpValue {
		public const string ValuePropertyName = "Value",
							DescriptionPropertyName = "RealDescription";
		[
		XtraSerializableProperty(XtraSerializationVisibility.Reference),
		DataMember
		]
		public object Value { get; set; }
		[
		XtraSerializableProperty,
		DataMember
		]
		public string Description { get; set; }
		[
		Browsable(false),
		]
		public string RealDescription {
			get {
				return !string.IsNullOrEmpty(Description) ? Description :
					Value != null ? Value.ToString() :
					string.Empty;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public LookUpValue Clone() { 
			return new LookUpValue(Value, Description);
		}
		public LookUpValue() { }
		public LookUpValue(object value, string description) {
			this.Value = value;
			this.Description = description;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static IEqualityComparer<LookUpValue> CreateComparer() {
			return new LookUpValueValueComparer();
		}
	}
#if !SILVERLIGHT && !WINRT
	[DesignerSerializer("DevExpress.XtraReports.Design.LookUpValueCollectionSerializer," + AssemblyInfo.SRAssemblyReportsExtensions, AttributeConstants.CodeDomSerializer)]
#endif
	public class LookUpValueCollection : Collection<LookUpValue> {
		public void AddRange(IEnumerable<LookUpValue> values) {
			foreach(var value in values) {
				Add(value);
			}
		}
	}
	class LookUpValueValueComparer : IEqualityComparer<LookUpValue> {
		public int GetHashCode(LookUpValue obj) {
			return HashCodeHelper.CalcHashCode(obj.Value, obj.Description);
		}
		public bool Equals(LookUpValue x, LookUpValue y) {
			if(x != null && y != null)
				return x == y || x.Description == y.Description && object.Equals(x.Value, y.Value);
			return x == null && y == null;
		}
	}
}
