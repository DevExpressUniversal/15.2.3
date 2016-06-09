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
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridOptionsBehavior : PivotGridOptionsBehaviorBase {
		bool applyBestFitOnFieldDragging;
		PivotGridScrolling horizontalScrolling;
		EditorShowMode editorShowMode;
		bool bestFitConsiderCustomAppearance;
		bool repaintGridOnFocusedCellChanged;
		public PivotGridOptionsBehavior(EventHandler optionsChanged)
			: base(optionsChanged) {
			this.applyBestFitOnFieldDragging = false;
			this.horizontalScrolling = PivotGridScrolling.CellsArea;
			this.editorShowMode = EditorShowMode.Default;
			this.bestFitConsiderCustomAppearance = false;
			this.repaintGridOnFocusedCellChanged = true;
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsBehaviorApplyBestFitOnFieldDragging"),
#endif
		DefaultValue(false), XtraSerializableProperty()
		]
		public bool ApplyBestFitOnFieldDragging {
			get { return applyBestFitOnFieldDragging; }
			set { applyBestFitOnFieldDragging = value; }
		}
		[
		DefaultValue(false), XtraSerializableProperty()
		]
		public bool BestFitConsiderCustomAppearance {
			get { return bestFitConsiderCustomAppearance; }
			set {
				bestFitConsiderCustomAppearance = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsBehaviorHorizontalScrolling"),
#endif
		DefaultValue(PivotGridScrolling.CellsArea), XtraSerializableProperty()
		]
		public PivotGridScrolling HorizontalScrolling {
			get { return horizontalScrolling; }
			set {
				if(HorizontalScrolling == value) return;
				horizontalScrolling = value;
				OnOptionsChanged();
			}
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsBehaviorEditorShowMode"),
#endif
		DefaultValue(EditorShowMode.Default), XtraSerializableProperty(), NotifyParentProperty(true)
		]
		public EditorShowMode EditorShowMode {
			get { return editorShowMode; }
			set {
				if(value == editorShowMode) return;
				editorShowMode = value;
				OnOptionsChanged();
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public EditorShowMode GetEditorShowMode() {
			EditorShowMode res = EditorShowMode;
			if(res == EditorShowMode.Default)
				res = EditorShowMode.MouseDownFocused;
			return res;
		}
		[
#if !SL
	DevExpressXtraPivotGridLocalizedDescription("PivotGridOptionsBehaviorRepaintGridOnFocusedCellChanged"),
#endif
		DefaultValue(true), XtraSerializableProperty()
		]
		public bool RepaintGridOnFocusedCellChanged {
			get { return repaintGridOnFocusedCellChanged; }
			set { repaintGridOnFocusedCellChanged = value; }
		}
	}
}
