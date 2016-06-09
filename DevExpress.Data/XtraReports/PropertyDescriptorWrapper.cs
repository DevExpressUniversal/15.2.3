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
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraReports.Native {
	public class PropertyDescriptorWrapper : PropertyDescriptor {
		protected PropertyDescriptor oldPropertyDescriptor;
		public PropertyDescriptorWrapper(PropertyDescriptor oldPropertyDescriptor)
			: base(oldPropertyDescriptor) {
			this.oldPropertyDescriptor = oldPropertyDescriptor;
		}
		public PropertyDescriptorWrapper(PropertyDescriptor oldPropertyDescriptor, Attribute[] attrs)
			: base(oldPropertyDescriptor, attrs) {
			this.oldPropertyDescriptor = oldPropertyDescriptor;
		}
		public override bool CanResetValue(object component) {
			return oldPropertyDescriptor.CanResetValue(component);
		}
		public override Type ComponentType {
			get { return oldPropertyDescriptor.ComponentType; }
		}
		public override object GetValue(object component) {
			return oldPropertyDescriptor.GetValue(component);
		}
		public override bool IsReadOnly {
			get { return oldPropertyDescriptor.IsReadOnly; }
		}
		public override Type PropertyType {
			get { return oldPropertyDescriptor.PropertyType; }
		}
		public override void ResetValue(object component) {
			oldPropertyDescriptor.ResetValue(component);
		}
		public override void SetValue(object component, object value) {
			oldPropertyDescriptor.SetValue(component, value);
		}
		public override bool ShouldSerializeValue(object component) {
			return oldPropertyDescriptor.ShouldSerializeValue(component);
		}
	}
}
