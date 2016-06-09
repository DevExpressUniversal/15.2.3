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
using System.Windows.Forms;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native;
using DevExpress.Utils.Design;
namespace DevExpress.Snap.Extensions.Native.ActionLists {
	public class SNCheckBoxActionList : FieldActionList<SNCheckBoxField> {
		public SNCheckBoxActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[ResDisplayName(typeof(ResFinder), SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_Checked")]
		public SnapCheckState Checked {
			get { return ConvertToSnapCheckState(ParsedInfo.CheckState); }
			set {
				if (Checked != value)
					ApplyNewValue((controller, newMode) => controller.SetSwitch(SNCheckBoxField.CheckStateSwitch, SNCheckBoxField.GetCheckStateString(ConvertFromSnapCheckState(newMode))), value);
			}
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "Checked", "Checked");
		}
		CheckState ConvertFromSnapCheckState(SnapCheckState value) {
			switch (value) {
				case SnapCheckState.Unchecked: return CheckState.Unchecked;
				case SnapCheckState.Checked: return CheckState.Checked;
				case SnapCheckState.Indeterminate: return CheckState.Indeterminate;
				default: return CheckState.Indeterminate;
			}
		}
		SnapCheckState ConvertToSnapCheckState(CheckState value) {
			switch (value) {
				case CheckState.Unchecked: return SnapCheckState.Unchecked;
				case CheckState.Checked: return SnapCheckState.Checked;
				case CheckState.Indeterminate: return SnapCheckState.Indeterminate;
				default: return SnapCheckState.Indeterminate;
			}
		}
	}
	[TypeConverter(typeof(EnumTypeConverter)), ResourceFinder(typeof(Data.ResFinder), DXDisplayNameAttribute.DefaultResourceFile)]
	public enum SnapCheckState { Unchecked, Checked, Indeterminate }
}
