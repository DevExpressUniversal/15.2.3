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

using System.ComponentModel;
using System.Runtime.Serialization;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
namespace DevExpress.ReportServer.ServiceModel.DataContracts {
	[DataContract]
	[TypeConverter(typeof(ReportLayoutRevisionIdentityConverter))]
	public class ReportLayoutRevisionIdentity : InstanceIdentity {
		[DataMember]
		public int Id { get; set; }
		public ReportLayoutRevisionIdentity() {
		}
		public ReportLayoutRevisionIdentity(int id) {
			Id = id;
		}
		public override string ToString() {
			return "ReportLayoutRevisionIdentity_" + Id;
		}
	}
}
namespace DevExpress.ReportServer.ServiceModel.DataContracts {
	using System;
	using System.Globalization;
	using System.Reflection;
#if !SILVERLIGHT
	using System.ComponentModel.Design.Serialization;
#else
	using DevExpress.Xpf.ComponentModel.Design.Serialization;
#endif
	class ReportLayoutRevisionIdentityConverter : TypeConverter {
		static readonly ConstructorInfo reportLayoutRevisionIdentityConstructor = typeof(ReportLayoutRevisionIdentity).GetConstructor(new Type[] { typeof(int) });
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			return destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == null) {
				throw new ArgumentNullException("destinationType");
			}
			var identity = value as ReportLayoutRevisionIdentity;
			if(destinationType == typeof(InstanceDescriptor) && identity != null) {
				return new InstanceDescriptor(reportLayoutRevisionIdentityConstructor, new object[] { identity.Id });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
