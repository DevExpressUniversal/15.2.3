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
using System.Drawing.Design;
using System.Globalization;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.DataAccess.UI.Native.Data;
using DevExpress.Utils.Design;
using DevExpress.Utils.UI;
namespace DevExpress.DataAccess.UI.Native.EntityFramework {
	public class EFStoredProcedureInfoCollectionTypeConverter : CollectionTypeConverter {
		class StoredProcedureInfoPropertyDescriptor : SimplePropertyDescriptor {
			class StoredProcedureInfoExpandableObjectConverter : ExpandableObjectConverter {
				class StoredProcedureInfoNamePropertyDescriptor : SimplePropertyDescriptor {
					public StoredProcedureInfoNamePropertyDescriptor()
						: base(typeof(EFStoredProcedureInfo), "Name", typeof(string), new Attribute[] { new ReadOnlyAttribute(true) }) {
					}
					public override object GetValue(object component) {
						return ((EFStoredProcedureInfo)component).Name;
					}
					public override void SetValue(object component, object value) {
						throw new NotSupportedException();
					}
				}
				class StoredProcedureInfoParametersPropertyDescriptor : SimplePropertyDescriptor {
					class CustomEditor : CollectionEditor {
						public CustomEditor()
							: base(typeof(EFParameter)) {
						}
						public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
							return UITypeEditorEditStyle.None;
						}
					}
					public StoredProcedureInfoParametersPropertyDescriptor()
						: base(typeof(EFStoredProcedureInfo), "Parameters", typeof(EFParameterCollection),
						new Attribute[] {
							new ReadOnlyAttribute(true),
							new TypeConverterAttribute(typeof(EFParameterCollectionTypeConverter)),
							new EditorAttribute(typeof(EFStoredProcedureParametersEditor), typeof(UITypeEditor))
						}
						) {
					}
					public override object GetValue(object component) {
						return ((EFStoredProcedureInfo)component).Parameters;
					}
					public override void SetValue(object component, object value) {
						throw new NotSupportedException();
					}
				}
				public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
					EFStoredProcedureInfo storedProcedure = (EFStoredProcedureInfo)value;
					List<PropertyDescriptor> result = new List<PropertyDescriptor>();
					result.Add(new StoredProcedureInfoNamePropertyDescriptor());
					result.Add(new StoredProcedureInfoParametersPropertyDescriptor());
					return new PropertyDescriptorCollection(result.ToArray());
				}
				public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
					return true;
				}
				public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
					if(destinationType == typeof(string))
						return true;
					return base.CanConvertTo(context, destinationType);
				}
				public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
					if(destinationType == typeof(string)) {
						EFStoredProcedureInfo efStoredProcedureInfo = value as EFStoredProcedureInfo;
						if(efStoredProcedureInfo != null)
							return efStoredProcedureInfo.Name;
					}
					return base.ConvertTo(context, culture, value, destinationType);
				}
			}
			int storedProcedureIndex;
			public StoredProcedureInfoPropertyDescriptor(int index)
				: base(typeof(EFStoredProcedureInfoCollection), string.Format("[{0}]", index), typeof(EFStoredProcedureInfo),
					new Attribute[] { 
						new TypeConverterAttribute(typeof(StoredProcedureInfoExpandableObjectConverter))
					}
				) {
				this.storedProcedureIndex = index;
			}
			public override object GetValue(object component) {
				EFStoredProcedureInfoCollection collection = (EFStoredProcedureInfoCollection)component;
				return collection[this.storedProcedureIndex];
			}
			public override void SetValue(object component, object value) {
				EFStoredProcedureInfoCollection collection = (EFStoredProcedureInfoCollection)component;
				collection[this.storedProcedureIndex] = (EFStoredProcedureInfo)value;
			}
		}
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			EFStoredProcedureInfoCollection collection = (EFStoredProcedureInfoCollection)value;
			PropertyDescriptor[] result = new StoredProcedureInfoPropertyDescriptor[collection.Count];
			for(int i = 0; i < result.Length; i++)
				result[i] = new StoredProcedureInfoPropertyDescriptor(i);
			return new PropertyDescriptorCollection(result);
		}
		public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
