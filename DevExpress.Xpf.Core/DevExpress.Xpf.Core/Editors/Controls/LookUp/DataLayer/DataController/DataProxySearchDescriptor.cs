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
using System.ComponentModel;
namespace DevExpress.Xpf.Editors.Helpers {
	public class DataProxySearchDescriptor : PropertyDescriptor {
		readonly string name;
		Func<DataProxy, object> GetFunc { get; set; }
		Action<DataProxy, object> SetFunc { get; set; }
		protected DataProxySearchDescriptor(string name, Attribute[] attrs)
			: base(name, attrs) {
		}
		protected DataProxySearchDescriptor(MemberDescriptor descr)
			: base(descr) {
		}
		protected DataProxySearchDescriptor(MemberDescriptor descr, Attribute[] attrs)
			: base(descr, attrs) {
		}
		public DataProxySearchDescriptor(string name, Func<DataProxy, object> getFunc, Action<DataProxy, object> setFunc)
			: this(string.IsNullOrEmpty(name) ? "Column" : name, null) {
			GetFunc = getFunc;
			SetFunc = setFunc;
			this.name = name;
		}
		public override bool CanResetValue(object component) {
			return false;
		}
		public override object GetValue(object component) {
			return GetFunc((DataProxy)component);
		}
		public override void ResetValue(object component) {
			SetValue(component, null);
		}
		public override void SetValue(object component, object value) {
			if (SetFunc == null)
				return;
			SetFunc((DataProxy)component, value);
		}
		public override bool ShouldSerializeValue(object component) {
			return false;
		}
		public override Type ComponentType {
			get { return typeof(DataProxy); }
		}
		public override bool IsReadOnly {
			get { return false; }
		}
		public override Type PropertyType {
			get { return typeof(object); }
		}
		public override string Name {
			get { return name; }
		}
	}
}
