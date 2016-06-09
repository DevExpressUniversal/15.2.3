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

using System.ComponentModel;
using DevExpress.XtraRichEdit;
namespace DevExpress.Snap.Core.Options {
	public enum FieldSelectionOnMouseClickMode {
		Always,
		Never,
		Auto
	}
	public class SnapFieldOptions : FieldOptions {
		const FieldSelectionOnMouseClickMode defaultFieldSelectionMode = FieldSelectionOnMouseClickMode.Auto;
		const bool defaultEnableEmptyFieldDataAlias = true;
		const bool defaultShowChartInfoPanel = true;
		FieldSelectionOnMouseClickMode fieldSelection;
		bool enableEmptyFieldDataAlias = defaultEnableEmptyFieldDataAlias;
		bool showChartInfoPanel = defaultShowChartInfoPanel;
		#region FieldSelection
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("SnapFieldOptionsFieldSelection"),
#endif
 DefaultValue(FieldSelectionOnMouseClickMode.Auto), NotifyParentProperty(true)]
		public FieldSelectionOnMouseClickMode FieldSelection {
			get { return fieldSelection; }
			set {
				if (fieldSelection == value)
					return;
				FieldSelectionOnMouseClickMode oldValue = fieldSelection;
				fieldSelection = value;
				OnChanged("FieldSelection", oldValue, value);
			}
		}
		#endregion
		#region EnableEmptyFieldDataAlias
#if !SL
	[DevExpressSnapCoreLocalizedDescription("SnapFieldOptionsEnableEmptyFieldDataAlias")]
#endif
		public bool EnableEmptyFieldDataAlias {
			get { return enableEmptyFieldDataAlias; }
			set {
				if (enableEmptyFieldDataAlias == value)
					return;
				enableEmptyFieldDataAlias = value;
				OnChanged("EnableEmptyFieldDataAlias", !value, value);
			}
		}
		#endregion
		#region ShowChartInfoPanel
		public bool ShowChartInfoPanel {
			get { return showChartInfoPanel; }
			set {
				if (showChartInfoPanel == value)
					return;
				showChartInfoPanel = value;
				OnChanged("ShowChartInfoPanel", !value, value);
			}
		}
		#endregion
		protected internal override void ResetCore() {
			base.ResetCore();
			FieldSelection = defaultFieldSelectionMode;
			EnableEmptyFieldDataAlias = defaultEnableEmptyFieldDataAlias;
			ShowChartInfoPanel = defaultShowChartInfoPanel;
		}
		protected internal override void CopyFrom(FieldOptions options) {
			base.CopyFrom(options);
			SnapFieldOptions snapOptions = options as SnapFieldOptions;
			if (snapOptions != null) {
				this.FieldSelection = snapOptions.FieldSelection;
				this.EnableEmptyFieldDataAlias = snapOptions.EnableEmptyFieldDataAlias;
				this.ShowChartInfoPanel = snapOptions.ShowChartInfoPanel;
			}
		}
	}
}
