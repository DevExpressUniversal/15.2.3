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
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Native.CalculatedFields {
	public class CalculatedPropertyDescriptor : CalculatedPropertyDescriptorBase {
		CalculatedField CalculatedField {
			get { return (CalculatedField)calculatedField; }
		}
		public override Type ComponentType {
			get { return typeof(CalculatedFieldCollection); }
		}
		public CalculatedPropertyDescriptor(CalculatedField calculatedField, IParameterSupplierBase parameterSupplier, DataContext dataContext)
			: base(calculatedField, parameterSupplier.GetIParameters(), dataContext) {
		}
		public CalculatedPropertyDescriptor(CalculatedField calculatedField, IParameterSupplierBase parameterSupplier)
			: base(calculatedField, parameterSupplier != null ? parameterSupplier.GetIParameters() : null) {
		}
		public CalculatedPropertyDescriptor(CalculatedField calculatedField)
			: base(calculatedField) {
		}
		protected CalculatedPropertyDescriptor(CalculatedField calculatedField, CalculatedEvaluatorContextDescriptor descriptor)
			: base(calculatedField, descriptor) {
		}
		public override bool Equals(object other) {
			CalculatedPropertyDescriptor otherDescriptor = other as CalculatedPropertyDescriptor;
			if(otherDescriptor != null)
				return CalculatedField.Equals(otherDescriptor.CalculatedField);
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public override object GetValue(object component) {
			using(GetValueEventArgs e = new GetValueEventArgs(CalculatedField.Report, CalculatedField.GetEffectiveDataSource(), CalculatedField.DataMember, component)) {
				CalculatedField.OnGetValue(e);
				return e.IsSet ? e.Value : base.GetValue(component);
			}
		}
	}
}
