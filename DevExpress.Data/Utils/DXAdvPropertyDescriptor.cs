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
using System.Collections;
namespace DevExpress.Utils.Design {
	public class DXAdvPropertyDescriptor : DXPropertyDescriptor {
		PropertyDescriptor originalDescriptor;
		public DXAdvPropertyDescriptor(PropertyDescriptor sourceDescriptor)
			: base(sourceDescriptor) {
			this.originalDescriptor = null;
			PropertyDescriptorCollection coll = TypeDescriptor.GetProperties(SourceDescriptor.ComponentType);
			if(coll != null) {
				this.originalDescriptor = coll[SourceDescriptor.Name];
			}
		}
		protected virtual PropertyDescriptor OriginalDescriptor { get { return originalDescriptor; } }
		public override bool ShouldSerializeValue(object component) {
			if(OriginalDescriptor != null) return OriginalDescriptor.ShouldSerializeValue(component);
			return base.ShouldSerializeValue(component);
		}
	}
	public class DXPropertyDescriptor : PropertyDescriptor {
		public static void ConvertDescriptors(IDictionary properties, string[] excludeList) {
			object[] keys = new object[properties.Count];
			int n = 0;
			foreach(object key in properties.Keys) {
				keys[n++] = key;
			}
			foreach(object key in keys) {
				if(excludeList != null && Array.IndexOf(excludeList, key) != -1) continue;
				PropertyDescriptor desc = properties[key] as PropertyDescriptor;
				if(desc == null || (desc is DXPropertyDescriptor)) continue;
				if(properties[key].GetType().Name == "ExtendedPropertyDescriptor") continue;
				if(desc.PropertyType.Equals(typeof(Type))) { 
					properties.Remove(key);
					continue;
				}
				if(!IsRequireConvert(desc)) continue;
				properties[key] = new DXPropertyDescriptor(desc as PropertyDescriptor);
			}
		}
		protected static bool IsRequireConvert(PropertyDescriptor desc) {
			if(desc.SerializationVisibility == DesignerSerializationVisibility.Hidden) return true;
			if(desc.IsReadOnly && desc.SerializationVisibility != DesignerSerializationVisibility.Content) return true;
			return false;
		}
		PropertyDescriptor sourceDescriptor;
		public DXPropertyDescriptor(PropertyDescriptor sourceDescriptor)
			: base(sourceDescriptor) {
			this.sourceDescriptor = sourceDescriptor;
		}
		protected PropertyDescriptor SourceDescriptor { get { return sourceDescriptor; } }
		public override bool CanResetValue(object component) { return SourceDescriptor.CanResetValue(component); }
		public override object GetValue(object component) { return SourceDescriptor.GetValue(component); }
		public override void SetValue(object component, object val) { SourceDescriptor.SetValue(component, val); }
		public override bool IsReadOnly { get { return SourceDescriptor.IsReadOnly; } }
		public override string Name { get { return SourceDescriptor.Name; } }
		public override Type ComponentType { get { return SourceDescriptor.ComponentType; } }
		public override Type PropertyType { get { return SourceDescriptor.PropertyType; } }
		public override void ResetValue(object component) {
			SourceDescriptor.ResetValue(component);
		}
		public override bool ShouldSerializeValue(object component) {
			if(SourceDescriptor.IsReadOnly && SerializationVisibility != DesignerSerializationVisibility.Content) return false;
			return SourceDescriptor.ShouldSerializeValue(component);
		}
	}
}
