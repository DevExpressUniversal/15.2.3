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
#if SL
using DevExpress.Xpf.ComponentModel;
#endif
namespace DevExpress.XtraPivotGrid {
	public class PivotGridOptionsLayout : 
#if !SL
		OptionsLayoutGrid 
#else
		OptionsLayoutBase
#endif
		, IXtraSerializableLayoutEx 
		{
		bool addNewGroups;
		bool storeLayoutOptions;
		PivotGridResetOptions resetOptions;
		public PivotGridOptionsLayout() {
			resetOptions = PivotGridResetOptions.OptionsPrint | PivotGridResetOptions.OptionsDataField;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new bool ShouldSerialize() { return base.ShouldSerialize(); }
		[
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.OptionsLayoutPivotGrid.AddNewGroups"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty, 
		DefaultValue(false),
		AutoFormatDisable
		]
		public bool AddNewGroups { get { return addNewGroups; } set { addNewGroups = value; } }
		[
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.OptionsLayoutPivotGrid.StoreLayoutOptions"),
		TypeConverter(typeof(BooleanTypeConverter)),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		DefaultValue(false),
		AutoFormatDisable
		]
		public virtual bool StoreLayoutOptions { get { return storeLayoutOptions; } set { storeLayoutOptions = value; } }
		[
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.OptionsLayoutPivotGrid.ResetOptions"),
		NotifyParentProperty(true),
		XtraSerializableProperty,
#if !SL && !DXPORTABLE
		Editor("DevExpress.Utils.Editors.AttributesEditor, " + AssemblyInfo.SRAssemblyUtils, typeof(System.Drawing.Design.UITypeEditor)),
#endif
		DefaultValue(PivotGridResetOptions.OptionsPrint | PivotGridResetOptions.OptionsDataField)
		]
		public virtual PivotGridResetOptions ResetOptions { get { return resetOptions; } set { resetOptions = value; } }
		public override void Assign(DevExpress.Utils.Controls.BaseOptions source) {
			base.Assign(source);
			PivotGridOptionsLayout options = source as PivotGridOptionsLayout;
			if (options != null) {
				ResetOptions = options.ResetOptions;
				AddNewGroups = options.AddNewGroups;
				StoreLayoutOptions = options.StoreLayoutOptions;
			}
		}
		#region IXtraSerializableLayoutEx Members
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return StoreLayoutOptions;
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			if((ResetOptions & PivotGridResetOptions.OptionsLayout) > 0 && StoreLayoutOptions)
				Reset();
		}
		#endregion
	}
	[Flags]
	public enum PivotGridResetOptions {
		None = 0,
		OptionsBehavior = 1,
		OptionsChartDataSource = 2,
		OptionsCustomization = 4,
		OptionsData = 8,
		OptionsDataField = 16,
		OptionsFilterPopup = 32,
		OptionsHint = 64,
		OptionsMenu = 128,
		OptionsOLAP = 256,
		OptionsPrint = 512,
		OptionsSelection = 1024,
		OptionsLoadingPanel = 2048,
		OptionsLayout = 8192,
		All = OptionsBehavior | OptionsChartDataSource | OptionsCustomization |
				OptionsData | OptionsDataField | OptionsFilterPopup |
				OptionsHint | OptionsMenu | OptionsOLAP | OptionsPrint |
				OptionsSelection | OptionsLoadingPanel
	}
}
