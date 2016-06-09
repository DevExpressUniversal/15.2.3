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
using System.ComponentModel;
using System.Globalization;
using System.ComponentModel.Design.Serialization;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Design {
	public class GroupFieldConverter : LocalizableObjectConverter {
		#region static
		static string GetFieldName(GroupField field) {
			if(field.Band == null || field.Band.RootReport == null)
				return field.FieldName;
			using(XRDataContextBase dataContext = new XRDataContextBase(null, true)) {
				string dataMember = dataContext.GetDataMemberFromDisplayName(field.Band.Report.GetEffectiveDataSource(), field.Band.Report.DataMember, field.FieldName);
				if(string.IsNullOrEmpty(dataMember))
					return field.FieldName;
				if(dataMember.StartsWith(field.Band.Report.DataMember + "."))
					return dataMember.Substring(field.Band.Report.DataMember.Length + 1);
				return dataMember;
			}
		}
		#endregion
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string)) {
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(InstanceDescriptor)) {
				System.Reflection.ConstructorInfo ci = value.GetType().GetConstructor(new Type[] { typeof(string), typeof(XRColumnSortOrder) });
				GroupField c = value as GroupField;
				return new InstanceDescriptor(ci, new object[] { GetFieldName(c), c.SortOrder });
			}
			else if (destinationType == typeof(string) && context == null) {
				return value.GetType().Name;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
