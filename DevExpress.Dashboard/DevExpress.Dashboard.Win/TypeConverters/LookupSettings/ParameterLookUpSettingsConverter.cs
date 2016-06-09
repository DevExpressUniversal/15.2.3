#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.DashboardWin.Native {
	public class ParameterLookUpSettingsConverter : ExpandableObjectConverter {
		static string GetNoLookupString() {
			return PreviewLocalizer.GetString(PreviewStringId.ParameterLookUpSettingsNoLookUp);
		}
		static string GetStaticListString() {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.PropertyGridParametersLookUpStaticList);
		}
		static string GetDynamicListString() {
			return DashboardWinLocalizer.GetString(DashboardWinStringId.PropertyGridParametersLookUpDynamicList);
		}
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(string)) {
				if(value == null)
					return GetNoLookupString();
				if(value is StaticListLookUpSettings)
					return GetStaticListString();
				if(value is DynamicListLookUpSettings)
					return GetDynamicListString();
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(string))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			List<ListItem> list = new List<ListItem>();
			list.Add(new ListItem(null, GetNoLookupString()));
			list.Add(new ListItem(typeof(StaticListLookUpSettings), GetStaticListString()));
			list.Add(new ListItem(typeof(DynamicListLookUpSettings), GetDynamicListString()));
			return new StandardValuesCollection(list);
		}
	}
}
