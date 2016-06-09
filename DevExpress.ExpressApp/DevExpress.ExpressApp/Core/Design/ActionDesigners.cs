#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Globalization;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Core.Design {
	public class ActionCategoryConverter : ReferenceConverter {
		private static readonly List<string> list;
		static ActionCategoryConverter() {
			list = new List<string>(Enum.GetNames(typeof(DevExpress.Persistent.Base.PredefinedCategory)));
			list.Sort();
		}
		private void RegisterValue(string str) {
			lock(list) {
				if(!list.Contains(str)) {
					list.Add(str);
					list.Sort();
				}
			}
		}
		public ActionCategoryConverter() : base(typeof(string)) { }
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object val) {
			string str = val as string;
			if(str != null) {
				RegisterValue(str);
				return str;
			}
			return base.ConvertFrom(context, culture, val);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object val, Type destType) {
			if(val == null) {
				return PredefinedCategory.Unspecified.ToString();
			}
			string result = val.ToString();
			RegisterValue(result);
			return result;
		}
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			lock(list) {
				return new StandardValuesCollection(list);
			}
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
	}
	public class ContainerIdConverter : ActionCategoryConverter {
		public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			StandardValuesCollection baseValues = base.GetStandardValues(context);
			List<string> result = new List<string>(baseValues.Count-1);
			foreach(string item in baseValues) {
				if(item != PredefinedCategory.Unspecified.ToString()) {
					result.Add(item);
				}
			}
			return new StandardValuesCollection(result);
		}
	}
}
