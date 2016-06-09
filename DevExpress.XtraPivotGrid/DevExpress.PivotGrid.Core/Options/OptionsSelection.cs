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
namespace DevExpress.XtraPivotGrid {
	public class PivotGridOptionsSelection : PivotGridOptionsBase {
		bool cellSelection;
		bool enableAppearanceFocusedCell;
		int maxWidth, maxHeight;
		bool multiSelect;
		public PivotGridOptionsSelection() {
			this.cellSelection = true;
			this.enableAppearanceFocusedCell = false;
			this.maxWidth = -1;
			this.maxHeight = -1;
			this.multiSelect = true;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsSelectionCellSelection"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool CellSelection { get { return cellSelection; } set { cellSelection = value; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsSelectionEnableAppearanceFocusedCell"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public bool EnableAppearanceFocusedCell { get { return enableAppearanceFocusedCell; } set { enableAppearanceFocusedCell = value; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsSelectionMaxWidth"),
#endif
 DefaultValue(-1), XtraSerializableProperty()]
		public int MaxWidth { get { return maxWidth; } set { maxWidth = value; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsSelectionMaxHeight"),
#endif
 DefaultValue(-1), XtraSerializableProperty()]
		public int MaxHeight { get { return maxHeight; } set { maxHeight = value; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridOptionsSelectionMultiSelect"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public bool MultiSelect { get { return multiSelect; } set { multiSelect = value; } }
	}
}
