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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.Data;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Localization;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
using System;
namespace DevExpress.DashboardCommon {
	public class StaticListLookUpSettings : ParameterLookUpSettings {
		internal const string xmlName = "StaticListLookUpSettings";
		const string xmlValues = "Values", xmlValue = "Value";
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("StaticListLookUpSettingsValues"),
#endif
		LocalizableCategory(DataAccessStringId.PropertyGridDataCategoryName),
#if !DXPORTABLE
		TypeConverter(typeof(DevExpress.Utils.Design.CollectionTypeConverter)),
#endif
		Editor("DevExpress.DashboardWin.Native.ParameterStringArrayEditor," + AssemblyInfo.SRAssemblyDashboardWin, typeof(UITypeEditor)),
		DXDisplayName(typeof(DevExpress.DashboardCommon.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.DashboardCommon.StaticListLookUpSettings.Values")
		]
		public string[] Values { get; set; }
		public StaticListLookUpSettings() {
			Values = new string[] { };
		}
		protected internal override bool SettingsEquals(object settings) {
			var lookupsettings = settings as StaticListLookUpSettings;
			if(lookupsettings == null)
				return false;
			if(Values.Length != lookupsettings.Values.Length)
				return false;
			for(int i = 0; i < Values.Length; i++)
				if(Values[i] != lookupsettings.Values[i])
					return false;
			return true;
		}
		protected override void Assign(ParameterLookUpSettings lookupsettings) {
			base.Assign(lookupsettings);
			StaticListLookUpSettings source = lookupsettings as StaticListLookUpSettings;
			if(source == null)
				return;
			Values = new string[source.Values.Length];
			for(int i = 0; i < source.Values.Length; i++)
				Values[i] = source.Values[i];
		}
		protected override ParameterLookUpSettings CreateInstance() {
			return new StaticListLookUpSettings();
		}
		protected internal override XElement SaveToXml() {
			XElement element = new XElement(xmlName);
			XElement valuesElement = new XElement(xmlValues);
			element.Add(valuesElement);
			foreach(string value in Values)
				valuesElement.Add(new XElement(xmlValue, value));
			return element;
		}
		protected internal override void LoadFromXml(XElement element) {
			XElement valuesElement = element.Element(xmlValues);
			if(valuesElement == null)
				return;
			List<string> valuesList = new List<string>();
			foreach(XElement valueElement in valuesElement.Elements(xmlValue))
				valuesList.Add(valueElement.Value);
			if(valuesList.Count > 0)
				Values = valuesList.ToArray();
		}
	}
}
