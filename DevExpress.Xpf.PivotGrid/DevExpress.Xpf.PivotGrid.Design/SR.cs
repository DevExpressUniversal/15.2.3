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

namespace DevExpress.Xpf.PivotGrid.Design {
	public static class SR {
		public const string CantPopulateFieldsMessage = "Please bind the PivotGridControl to an Olap data source using its OlapConnectionString property or specify a data source using the DataSource property.";
		public const string CantPopulateFieldsCaption = "Fields cannot be populated";
		public const string ShouldClearExistingFieldsMessage = "The existing fields will be removed and then populated with entries for each field in the bound data source. Would you like to continue?";
		public const string ShouldClearExistingFieldsCaption = "Information";
		public const string PopulateFieldsDescription = "Populate Fields";
		public const string AddFieldDescription = "Add Field";
		public const string GenerateFieldsCommandText = "Retrieve Fields";
		public const string AddFieldCommandText = "Add {0} Field";
		public const string ChangeFieldProperty = "Change Field {0}";
		public const string OrderField = "Order Fields";
		public const string CantShowFormatConditionExpressionEditorMessage = "The Format Condition Expression Editor cannot be shown.";
		public const string EditFormatConditionsCommandText = "Manage Conditional Formatting Rules";
		public const string EditFormatConditionsDescription = "Edit Conditional Formatting Rules";
		public static string CantShowFormatNameEditorMessage = "The Format Name Editor cannot be shown.";
	}
}
