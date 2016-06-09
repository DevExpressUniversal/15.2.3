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

using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public enum ComboBoxDashboardItemType { Standard, Checked }
	[
	DashboardItemType(DashboardItemType.Combobox)
	]
	public class ComboBoxDashboardItem : FilterElementDashboardItem {
		const string xmlComboBoxType = "ComboBoxType";
		const ComboBoxDashboardItemType DefaultComboBoxType = ComboBoxDashboardItemType.Standard;
		ComboBoxDashboardItemType comboBoxType = DefaultComboBoxType;
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameComboBoxItem); } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ComboBoxDashboardItemComboBoxType"),
#endif
		DefaultValue(DefaultComboBoxType)
		]
		public ComboBoxDashboardItemType ComboBoxType {
			get { return comboBoxType; }
			set {
				if(comboBoxType == value)
					return;
				comboBoxType = value;
				OnElementTypeChanged();
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ComboBoxDashboardItemInteractivityOptions"),
#endif
		Category(CategoryNames.Interactivity),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public FilterableDashboardItemInteractivityOptions InteractivityOptions { get { return InteractivityOptionsBase; } }
		protected internal override bool IsSingleSelection { get { return ComboBoxType == ComboBoxDashboardItemType.Standard; } }
		internal override bool ActualShowAllValue {
			get {
				return comboBoxType == ComboBoxDashboardItemType.Checked || base.ActualShowAllValue;
			}
		}
		protected internal override bool IsFixed { get { return true; } }
		protected internal override DashboardItemViewModel CreateViewModel() {
			return new ComboBoxDashboardItemViewModel(this);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			XmlHelper.Save(element, xmlComboBoxType, comboBoxType, DefaultComboBoxType);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			XmlHelper.LoadEnum<ComboBoxDashboardItemType>(element, xmlComboBoxType, x => comboBoxType = x);
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.FilterElementType = comboBoxType == ComboBoxDashboardItemType.Checked ?
				FilterElementTypeDescription.Checked :
				FilterElementTypeDescription.Radio;
			return description;
		}
		protected override void AssignFilterElementType(bool multiple) {
			comboBoxType = multiple ? ComboBoxDashboardItemType.Checked : ComboBoxDashboardItemType.Standard;
		}
	}
}
