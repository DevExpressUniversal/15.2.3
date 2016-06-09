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
using DevExpress.Utils.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPivotGrid.Customization;
namespace DevExpress.XtraPivotGrid {
	public enum PivotGridScrolling { CellsArea, Control };
	public class PivotGridOptionsCustomizationEx : PivotGridOptionsCustomization {
		bool allowEdit;
		bool allowResizing;
		public PivotGridOptionsCustomizationEx(EventHandler optionsChanged)
			: this(optionsChanged, null, string.Empty) {
		}
		public PivotGridOptionsCustomizationEx(EventHandler optionsChanged, DevExpress.WebUtils.IViewBagOwner viewBagOwner, string objectPath)
			: base(optionsChanged, viewBagOwner, objectPath) {
			this.allowEdit = true;
			this.allowResizing = true;
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsCustomizationExAllowEdit"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowEdit {
			get { return allowEdit; }
			set {
				if(value == allowEdit) return;
				allowEdit = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsCustomizationExAllowResizing"),
#endif
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public bool AllowResizing {
			get { return allowResizing; }
			set {
				if(value == allowResizing) return;
				allowResizing = value;
				OnOptionsChanged();
			}
		}
	}
}
