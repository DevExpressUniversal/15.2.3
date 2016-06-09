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

using System.Drawing;
using DevExpress.Utils;
using DevExpress.XtraVerticalGrid.ViewInfo;
namespace DevExpress.XtraVerticalGrid {
	internal sealed class VGridStore {
		public static readonly Point DefaultCustomizationFormLocation = new Point(int.MinValue, int.MinValue);
		public static readonly Rectangle DefaultCustomizationFormBounds = Rectangle.Empty;
		internal DevExpress.Utils.Win.IPopupServiceControl popupServiceControl = new DevExpress.XtraEditors.Controls.HookPopupController();
		internal readonly object rowChanged = new object();
		internal readonly object rowChanging = new object();
		internal readonly object focusedRowChanged = new object();
		internal readonly object focusedRecordChanged = new object();
		internal readonly object focusedRecordCellChanged = new object();
		internal readonly object customDrawRowHeaderIndent = new object();
		internal readonly object customDrawRowHeaderCell = new object();
		internal readonly object customDrawRowValueCell = new object();
		internal readonly object customDrawSeparator = new object();
		internal readonly object customDrawTreeButton = new object();
		internal readonly object showingEditor = new object();
		internal readonly object shownEditor = new object();
		internal readonly object cellValueChanging = new object();
		internal readonly object cellValueChanged = new object();
		internal readonly object hiddenEditor = new object();
		internal readonly object validatingEditor = new object();
		internal readonly object invalidValueException = new object();
		internal readonly object showCustomizationForm = new object();
		internal readonly object hideCustomizationForm = new object();
		internal readonly object customizationFormCreatingCategory = new object();
		internal readonly object customizationFormDeletingCategory = new object();
		internal readonly object recordCellStyle = new object();
		internal readonly object customRecordCellEdit = new object();
		internal readonly object dataSourceChanged = new object();
		internal readonly object startDragRow = new object();
		internal readonly object processDragRow = new object();
		internal readonly object endDragRow = new object();
		internal readonly object rowHeaderWidthChanged = new object();
		internal readonly object recordWidthChanged = new object();
		internal readonly object gridLayout = new object();
		internal readonly object stateChanged = new object();
		internal readonly object validateRecord = new object();
		internal readonly object invalidRecordException = new object();
		internal readonly object initNewRecord = new object();
		internal readonly object layoutUpgrade = new object();
		internal readonly object beforeLoadLayout = new object();
		internal readonly object customUnboundData = new object();
		internal readonly object showMenu = new object();
		internal readonly object onPopupMenuShowing = new object();
		internal readonly object customPropertyDescriptors = new object();
		internal readonly object customRecordCellEditForEditing = new object();
		internal readonly object leftVisibleRecordChanged = new object();
		internal readonly object topVisibleRowIndexChanged = new object();
		internal readonly object unboundExpressionEditorCreated = new object();
		internal readonly object recordUpdated = new object();
		private VGridStore() {
		}
		static VGridStore instance = new VGridStore();
		internal static VGridStore Instance { get { return instance; } }
	}
}
