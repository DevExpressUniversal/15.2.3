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

namespace DevExpress.Xpf.Grid.Design {
	public static class SR {
		public const string NoAvailableFieldNamesMessage = "To get available fields list, specify either the {0}.{1} property or assign the {0}.{2} property at design-time.";
		public const string CantShowFieldNameEditorMessage = "A field name editor cannot be shown.";
		public const string CantShowUnboundExpressionEditorMessage = "The Unbound Expression Editor cannot be shown.";
		public const string CantShowFormatConditionExpressionEditorMessage = "The Format Condition Expression Editor cannot be shown.";
		public const string CantShowFormatNameEditorMessage = "The Format Name Editor cannot be shown.";
		public const string CantShowFilterStringEditorMessage = "The Filter Editor cannot be shown.";
		public const string CantPopulateColumnsMessage = "GridControl could not recognize your data object type. Set the {2} property in xaml if it was not set before or specify the data object type explicitly using the {1} property.";
		public const string CantPopulateColumnsCaption = "Columns cannot be populated";
		public const string ShouldClearExistingColumnsMessage = "The existing columns will be removed and then populated with entries for each field in the bound data source. Would you like to continue?";
		public const string ShouldClearExistingColumnsAndAddBandsMessage = "The existing columns will be removed. Would you like to continue?";
		public const string CardViewDoesNotSupportBands = "CardView does not support banded layout.";
		public const string ShouldClearExistingColumnsCaption = "Information";
		public const string ChangeViewDescription = "Change View Type";
		public const string ChangeEditSettingsDescription = "Change EditSettings Type";
		public const string PopulateColumnsDescription = "Populate columns";
		public const string DeleteColumnDescription = "Delete column";
		public const string DeleteColumnsDescription = "Delete columns";
		public const string DeleteBandDescription = "Delete band";
		public const string AddColumnDescription = "Add column";
		public const string AddEmptyColumnDescription = "Add Empty column";
		public const string AddBandDescription = "Add band";
		public const string ReorderColumnDescription = "Reorder grid columns";
		public const string ReorderBandDescription = "Reorder grid bands";
		public const string ChangeGroupingDescription = "Change grouping";
		public const string ChangeSortingDescription = "Change sorting";
		public const string ChangeColumnWidthDescription = "Change column width";
		public const string GenerateColumnsAndExpandProperties = "Generate Columns And Expand Properties";
		public const string GenerateColumns = "Generate Columns";
	}
}
