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
using DevExpress.Data;
using DevExpress.Services.Internal;
using DevExpress.Compatibility.System.ComponentModel;
#if SILVERLIGHT
using DevExpress.Xpf.ComponentModel;
using DevExpress.Data.Browsing;
using TypeConverter = DevExpress.Data.Browsing.TypeConverter;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
#endif
namespace DevExpress.XtraReports.Native.Parameters {
	public class ParameterPropertyDescriptor : PropertyDescriptor, IContainerComponent {
		IParameter parameter;
		object temporaryValue;
		bool changed;
		object IContainerComponent.Component { get { return parameter; } }
		public override Type ComponentType {
			get { return typeof(IEnumerable<IParameter>); }
		}
		public override TypeConverter Converter {
			get { return TypeDescriptor.GetConverter(parameter.Type); }
		}
		public override Type PropertyType {
			get { return parameter.Type; }
		}
		public override bool IsReadOnly {
			get { return false; }
		}
		public override AttributeCollection Attributes {
			get { return AttributeCollection.FromExisting(base.Attributes, new RefreshPropertiesAttribute(RefreshProperties.All)); }
		}
		public ParameterPropertyDescriptor(IParameter parameter)
			: base(parameter.Name, null) {
			this.parameter = parameter;
		}
		public override object GetValue(object component) {
			return changed ? temporaryValue : parameter.Value;
		}
		public override void SetValue(object component, object value) {
			temporaryValue = value;
			changed = true;
		}
		public void Commit() {
			if(changed) {
				parameter.Value = temporaryValue;
				changed = false;
			}
		}
		public void Reset() {
			changed = false;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override void ResetValue(object component) {
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
	}
}
