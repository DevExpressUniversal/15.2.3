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
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Reports.UserDesigner.Editors;
using DevExpress.Xpf.Reports.UserDesigner.ReportModel;
using DevExpress.XtraReports.Parameters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
namespace DevExpress.Xpf.Reports.UserDesigner.Native.PropertyConverters {
	public class LookUpSettingsPropertyConverter : TypeConverter, IXRPropertyConverter {
		enum LookUpSettingsType {
			NoLookUp,
			StaticList,
			DynamicList
		}
		class NamedLookUpSettingsType : IDataContainerBase {
			readonly Parameter parameter;
			internal Parameter Parameter { get { return parameter; } }
			public NamedLookUpSettingsType(Parameter parameter) {
				this.parameter = parameter;
				data = GetLookUpDataValue();
			}
			LookUpSettingsType data;
			[NotifyParentProperty(true)]
			[Display(AutoGenerateField = false)]
			[RefreshProperties(RefreshProperties.All)]
			public LookUpSettingsType Data {
				get { return (data = GetLookUpDataValue()); }
				set {
					switch(value) {
						case LookUpSettingsType.StaticList:
							parameter.LookUpSettings = new StaticListLookUpSettings();
							break;
						case LookUpSettingsType.DynamicList:
							parameter.LookUpSettings = new DynamicListLookUpSettings();
							break;
						default:
							parameter.LookUpSettings = null;
							break;
					}
				}
			}
			string IDataContainerBase.DataMember {
				get { return (Parameter.LookUpSettings as DynamicListLookUpSettings).Return(x=> x.DataMember, ()=> null); }
				set { (Parameter.LookUpSettings as DynamicListLookUpSettings).Do(x => x.DataMember = value); }
			}
			object IDataContainerBase.DataSource {
				get { return (Parameter.LookUpSettings as DynamicListLookUpSettings).Return(x => x.DataSource, () => null); }
				set { (Parameter.LookUpSettings as DynamicListLookUpSettings).Do(x => x.DataSource = value); }
			}
			LookUpSettingsType GetLookUpDataValue() {
				if(parameter.LookUpSettings == null)
					return LookUpSettingsType.NoLookUp;
				return parameter.LookUpSettings.GetType() == typeof(DynamicListLookUpSettings) ? LookUpSettingsType.DynamicList : LookUpSettingsType.StaticList;
			}
		}
		static readonly string DataPropertyName = ExpressionHelper.GetPropertyName((NamedLookUpSettingsType x) => x.Data);
		public Type PropertyType { get { return typeof(LookUpSettings); } }
		public Type VirtualPropertyType { get { return typeof(NamedLookUpSettingsType); } }
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			var allProperties = PropertyDescriptorHelper.GetPropertyDescriptors(value, context, null, attributes);
			var lookUpSettingsProperties = new List<PropertyDescriptor>() { allProperties[DataPropertyName] };
			var namedLookUpSettingsType = (NamedLookUpSettingsType)value;
			if(namedLookUpSettingsType.Parameter.LookUpSettings != null)
				lookUpSettingsProperties.AddRange(ProxyPropertyDescriptor.GetProxyDescriptors(namedLookUpSettingsType, x => x.Parameter.LookUpSettings).ToArray());
			return new PropertyDescriptorCollection(lookUpSettingsProperties.Where(x => {
				var browsableAttribute = (BrowsableAttribute)x.Attributes[typeof(BrowsableAttribute)];
				return browsableAttribute == null || browsableAttribute.Browsable;
			}).ToArray());
		}
		public object Convert(object value, object owner) {
			return new NamedLookUpSettingsType((Parameter)XRModelBase.GetXRModel((DiagramItem)owner).XRObject); 
		}
		public object ConvertBack(object value) {
			throw new NotSupportedException();
		}
	}
}
