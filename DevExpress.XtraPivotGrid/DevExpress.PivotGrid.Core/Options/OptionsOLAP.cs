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
using DevExpress.WebUtils;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid {
	public enum PivotDefaultMemberFields { NonVisibleFilterFields, AllFilterFields };
#if !SL
	[TypeConverter(typeof(EnumTypeConverter))]
	[ResourceFinder(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum OLAPDataProvider {
		Default,
		Adomd,
		OleDb,
		Xmla
	};
#endif
	public class PivotGridOptionsOLAP : PivotGridOptionsBase {
		bool useAggregateForSingleFilterValue, filterByUniqueName;
		PivotDefaultMemberFields defaultMemberFields;
		bool useDefaultMeasure;
		bool usePrefilter;
		bool allowDuplicatedMeasures;
		bool sortByCustomFieldValueDisplayText;
		public PivotGridOptionsOLAP(EventHandler optionsChanged) : base(optionsChanged) {
			this.useAggregateForSingleFilterValue = false;
			this.filterByUniqueName = false;
			this.defaultMemberFields = PivotDefaultMemberFields.NonVisibleFilterFields;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsOLAPUseAggregateForSingleFilterValue"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
			DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsOLAP.UseAggregateForSingleFilterValue"),
		TypeConverter(typeof(BooleanTypeConverter)),
		DefaultValue(false), XtraSerializableProperty(),
		AutoFormatDisable(), NotifyParentProperty(true)
		]
		public bool UseAggregateForSingleFilterValue {
			get { return useAggregateForSingleFilterValue; }
			set {
				if(useAggregateForSingleFilterValue == value) return;
				useAggregateForSingleFilterValue = value;
				OnOptionsChanged();
			}
		}
		[
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
			DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsOLAP.DefaultMembersBehavior"),
		DefaultValue(PivotDefaultMemberFields.NonVisibleFilterFields), XtraSerializableProperty(),
		AutoFormatDisable(), NotifyParentProperty(true)
		]
		public PivotDefaultMemberFields DefaultMemberFields {
			get { return defaultMemberFields; }
			set {
				if(defaultMemberFields == value) return;
				defaultMemberFields = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsOLAPFilterByUniqueName"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
		DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsOLAP.FilterByUniqueName"),
		DefaultValue(false), XtraSerializableProperty(),
		AutoFormatDisable(), NotifyParentProperty(true)
		]
		public bool FilterByUniqueName {
			get { return filterByUniqueName; }
			set {
				if(filterByUniqueName == value)
					return;
				filterByUniqueName = value;
				OnOptionsChanged();
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UseDefaultMeasure {
			get { return useDefaultMeasure; }
			set {
				if(useDefaultMeasure == value)
					return;
				useDefaultMeasure = value;
				OnOptionsChanged();
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UsePrefilter {
			get { return usePrefilter; }
			set {
				if(usePrefilter == value)
					return;
				usePrefilter = value;
				OnOptionsChanged();
			}
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowDuplicatedMeasures {
			get { return allowDuplicatedMeasures; }
			set {
				if(allowDuplicatedMeasures == value)
					return;
				allowDuplicatedMeasures = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsOLAPSortByCustomFieldValueDisplayText"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
		DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridOptionsOLAP.SortByCustomFieldValueDisplayText"),
		DefaultValue(false), XtraSerializableProperty(),
		AutoFormatDisable(), NotifyParentProperty(true)
		]
		public bool SortByCustomFieldValueDisplayText {
			get { return sortByCustomFieldValueDisplayText; }
			set {
				if(sortByCustomFieldValueDisplayText == value)
					return;
				sortByCustomFieldValueDisplayText = value;
				OnOptionsChanged();
			}
		}
	}
}
