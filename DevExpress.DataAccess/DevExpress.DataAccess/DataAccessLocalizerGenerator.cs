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
using System;
namespace DevExpress.DataAccess.Localization {
	#region enum DataAccessStringId
	public enum DataAccessStringId {
		SkipWithoutSortingPropertyGridError,
		PositiveIntegerError,
		MessageDuplicateItem,
		MessageInvalidItemName,
		MessageDuplicateItemName,
		MessageNullItem,
		MessageWrongCharacterItemName,
		DefaultNameDataSource,
		ConnectingToDatabaseCaption,
		ConnectingToDatabaseMessage,
		LoadingDataPanelText,
		LoadingDataSourcePanelText,
		LoadingDataSourcePanelCounter,
		EmptyColumnAliasPattern,
		PropertyGridDataCategoryName,
		PropertyGridDesignCategoryName,
		PropertyGridBehaviorCategoryName,
		PropertyGridConnectionCategoryName,
		QueryPropertyGridCommonCategoryName,
		QueryPropertyGridTableSelectionCategoryName,
		QueryPropertyGridStoredProcCategoryName,
		QueryPropertyGridCustomSqlCategoryName,
		RelationEditorRelationTypeInnerJoin,
		RelationEditorRelationTypeLeftOuterJoin,
		RelationEditorRelationTypeMasterDetail,
		QueryDesignerJoinExpressionPattern,
		QueryDesignerJoinExpressionElementSeparator,
		MessageWrongCharacterParameterName,
		ConnectionStringPostfixServerExplorer,
		ConnectionStringPostfixAppConfig,
		RebuildResultSchemaWaitFormText,
		DatabaseConnectionExceptionStringId,
		WizardEmptyConnectionNameMessage,
		WizardDataConnectionNameExistsMessage,
		CustomSqlQueryValidationException,
		TableNotInSchemaValidationException,
		ColumnNullValidationException,
		UnnamedColumnValidationException,
		DuplicatingColumnNamesValidationException,
		ColumnNotInSchemaValidationException,
		RelationColumnNullValidationException,
		RelationColumnNotInSchemaValidationException,
		UnnamedTableValidationException,
		DuplicatingTableNamesValidationException,
		IncompleteRelationValidationException,
		RelationTableNotSelectedValidationException,
		NoTablesValidationException,
		TableNullValidationException,
		NoColumnsValidationException,
		RelationNullValidationException,
		NoRelationColumnsValidationException,
		CircularRelationsValidationException,
		TablesNotRelatedValidationException,
		SortingBySameColumnTwiceValidationException,
		AggregationWithoutAliasValidationException,
		GroupByAggregateColumnValidationException,
		PartialAggregationValidationException,
		SqlStringEmptyValidationException,
		StoredProcNameNullValidationException,
		StoredProcNotInSchemaValidationException,
		StoredProcParamCountValidationException,
		StoredProcParamNullValidationException,
		StoredProcParamNameValidationException,
		StoredProcParamTypeValidationException,
		FilterByColumnOfMissingTableValidationException,
		FilterByMissingInSchemaColumnValidationExpression,
		FilterByAmbiguousColumnValidationException,
		HavingWithoutGroupByValidationException,
		GroupByWithoutAggregateValidationException,
		SkipWithoutSortingValidationException,
		RelationException,
		ProviderNotSupportedException,
		GatheringTypesPanelText,
		ParameterlessConstructor,
		ODSWizardErrorNoDefaultCtor,
		ODSWizardErrorExceptionInCtor,
		ODSWizardErrorExceptionInGetItemProperties,
		ODSWizardErrorStaticValue,
		ODSErrorMissingMember,
		ParameterListNull,
		ParameterListEmpty,
		MessageNonexistentQuery,
		MessageNonexistentColumn,
		QueryBuilderAliasAlreadyExists,
		QueryBuilderCanJoin,
		QueryBuilderJoinedOn,
		QueryBuilderNothingSelected,
		QueryBuilderJoinEditorMissingData,
		QueryBuilderInvalidFilter,
		QueryBuilderNoTablesAndViews,
		WizardNoEFDataContextsMessage,
		WizardCustomConnectionString,
		ExcelEncryptedFileException_PasswordRequired,
		ExcelEncryptedFileException_WrongPassword,
		ExcelEncryptedFileException_EncryptionTypeNotSupported,
		ExcelDataSource_SchemaLoadingText,
		XmlFileStrategy_FileNameFilter,
		FileNameFilter_AllFormats,
		FileNameFilter_Excel,
		FileNameFilter_CSV,
		ResultRelation_ColumnNotFoundError,
		ExcelDataSource_FileNotFoundMessage,
		ConfigureQueryPage_CustomSqlWillBeLost,
	}
	#endregion
	#region DataAccessLocalizer.AddStrings 
	public partial class DataAccessLocalizer {
		void AddStrings() {
			AddString(DataAccessStringId.SkipWithoutSortingPropertyGridError, "The SKIP setting is set while records are not sorted. Please apply sorting to be able to skip the first N records.");
			AddString(DataAccessStringId.PositiveIntegerError, "Only non-negative numeric values are allowed.");
			AddString(DataAccessStringId.MessageDuplicateItem, "The collection already contains the '{0}' item");
			AddString(DataAccessStringId.MessageInvalidItemName, "Item name cannot be null or an empty string");
			AddString(DataAccessStringId.MessageDuplicateItemName, "This collection already contains an item with the '{0}' name");
			AddString(DataAccessStringId.MessageNullItem, "Cannot add a null value to the collection");
			AddString(DataAccessStringId.MessageWrongCharacterItemName, "Item name '{0}' contains wrong characters");
			AddString(DataAccessStringId.DefaultNameDataSource, "Data Source");
			AddString(DataAccessStringId.ConnectingToDatabaseCaption, "Please wait");
			AddString(DataAccessStringId.ConnectingToDatabaseMessage, "Connecting to the database...");
			AddString(DataAccessStringId.LoadingDataPanelText, "Loading Data...");
			AddString(DataAccessStringId.LoadingDataSourcePanelText, "Loading data source");
			AddString(DataAccessStringId.LoadingDataSourcePanelCounter, "{0} of {1}...");
			AddString(DataAccessStringId.EmptyColumnAliasPattern, "Column{0}");
			AddString(DataAccessStringId.PropertyGridDataCategoryName, "Data");
			AddString(DataAccessStringId.PropertyGridDesignCategoryName, "Design");
			AddString(DataAccessStringId.PropertyGridBehaviorCategoryName, "Behavior");
			AddString(DataAccessStringId.PropertyGridConnectionCategoryName, "Connection");
			AddString(DataAccessStringId.QueryPropertyGridCommonCategoryName, "Common");
			AddString(DataAccessStringId.QueryPropertyGridTableSelectionCategoryName, "Table Selection");
			AddString(DataAccessStringId.QueryPropertyGridStoredProcCategoryName, "Stored Procedure");
			AddString(DataAccessStringId.QueryPropertyGridCustomSqlCategoryName, "Custom SQL");
			AddString(DataAccessStringId.RelationEditorRelationTypeInnerJoin, "Inner join");
			AddString(DataAccessStringId.RelationEditorRelationTypeLeftOuterJoin, "Left outer join");
			AddString(DataAccessStringId.RelationEditorRelationTypeMasterDetail, "Master-detail relation");
			AddString(DataAccessStringId.QueryDesignerJoinExpressionPattern, "{0} on {1}");
			AddString(DataAccessStringId.QueryDesignerJoinExpressionElementSeparator, ", ");
			AddString(DataAccessStringId.MessageWrongCharacterParameterName, "Parameter name '{0}' contains wrong characters");
			AddString(DataAccessStringId.ConnectionStringPostfixServerExplorer, " (from the Server Explorer)");
			AddString(DataAccessStringId.ConnectionStringPostfixAppConfig, " (in the config file)");
			AddString(DataAccessStringId.RebuildResultSchemaWaitFormText, "Rebuilding the Result Schema...");
			AddString(DataAccessStringId.DatabaseConnectionExceptionStringId, "Failed to connect to the database. To learn more, see the exception details. \r\n\r\nException details:\r\n {0}");
			AddString(DataAccessStringId.WizardEmptyConnectionNameMessage, "The name cannot be null, empty or contain only whitespaces.");
			AddString(DataAccessStringId.WizardDataConnectionNameExistsMessage, "A data connection with the specified name already exists. Please specify a different connection name.");
			AddString(DataAccessStringId.CustomSqlQueryValidationException, "A custom SQL query should contain only SELECT statements.");
			AddString(DataAccessStringId.TableNotInSchemaValidationException, "The schema does not contain the specified table: \"{0}\".");
			AddString(DataAccessStringId.ColumnNullValidationException, "A column cannot be null.");
			AddString(DataAccessStringId.UnnamedColumnValidationException, "The column name is not specified.");
			AddString(DataAccessStringId.DuplicatingColumnNamesValidationException, "A column with the following name already exists: \"{0}\".\"{1}\".");
			AddString(DataAccessStringId.ColumnNotInSchemaValidationException, "The schema does not contain the following column: \"{0}\".\"{1}\".");
			AddString(DataAccessStringId.RelationColumnNullValidationException, "The relation column cannot be null.");
			AddString(DataAccessStringId.RelationColumnNotInSchemaValidationException, "The schema does not contain the following relation column: \"{0}\".\"{1}\".");
			AddString(DataAccessStringId.UnnamedTableValidationException, "The table name is not specified.");
			AddString(DataAccessStringId.DuplicatingTableNamesValidationException, "A table with the following name already exists: \"{0}\".");
			AddString(DataAccessStringId.IncompleteRelationValidationException, "The specified relation is incomplete: \"{0}\".");
			AddString(DataAccessStringId.RelationTableNotSelectedValidationException, "The reference table has not been selected: \"{0}\".");
			AddString(DataAccessStringId.NoTablesValidationException, "The collection of tables cannot be empty.");
			AddString(DataAccessStringId.TableNullValidationException, "A table cannot be null.");
			AddString(DataAccessStringId.NoColumnsValidationException, "None of the tables contain any columns.");
			AddString(DataAccessStringId.RelationNullValidationException, "A relation cannot be null.");
			AddString(DataAccessStringId.NoRelationColumnsValidationException, "The specified relation does not contain key columns: \"{0}\".");
			AddString(DataAccessStringId.CircularRelationsValidationException, "Circular relations have been detected.");
			AddString(DataAccessStringId.TablesNotRelatedValidationException, "The following tables have no relations: \"{0}\".");
			AddString(DataAccessStringId.SortingBySameColumnTwiceValidationException, "The following column is used as a sorting criterion more than once: \"{0}\".\"{1}\".");
			AddString(DataAccessStringId.AggregationWithoutAliasValidationException, "An aggregate column should have an alias.");
			AddString(DataAccessStringId.GroupByAggregateColumnValidationException, "Cannot group by an aggregate column: \"{0}\".\"{1}\".");
			AddString(DataAccessStringId.PartialAggregationValidationException, "Certain columns on the select list are invalid because they are not contained in either an aggregate function or in the Group By clause. Apply aggregation/grouping either to all columns or to none of them.");
			AddString(DataAccessStringId.SqlStringEmptyValidationException, "The SQL string is null or empty.");
			AddString(DataAccessStringId.StoredProcNameNullValidationException, "The name of a stored procedure cannot be null.");
			AddString(DataAccessStringId.StoredProcNotInSchemaValidationException, "Cannot find the specified stored procedure: \"{0}\".");
			AddString(DataAccessStringId.StoredProcParamCountValidationException, "Parameter count mismatch: <{0}>, <{1}> is expected.");
			AddString(DataAccessStringId.StoredProcParamNullValidationException, "A parameter cannot be null.");
			AddString(DataAccessStringId.StoredProcParamNameValidationException, "Parameter name mismatch: <{0}>, <{1}> is expected.");
			AddString(DataAccessStringId.StoredProcParamTypeValidationException, "Parameter type mismatch: <{0}>, <{1}> is expected.");
			AddString(DataAccessStringId.FilterByColumnOfMissingTableValidationException, "There are no tables containing column [{0}], which is used in the filter string.");
			AddString(DataAccessStringId.FilterByMissingInSchemaColumnValidationExpression, "Column [{0}].[{1}] is used in the filter string, but is missing in DBSchema.");
			AddString(DataAccessStringId.FilterByAmbiguousColumnValidationException, "The column name [{0}], which is used in the filter string, is ambiguous. Columns with this name exist in the following tables: {1}.");
			AddString(DataAccessStringId.HavingWithoutGroupByValidationException, "Group filtering criteria are defined while data is not grouped. Please apply data grouping or remove group filtering criteria on the Group Filter tab of the Filter Editor dialog.");
			AddString(DataAccessStringId.GroupByWithoutAggregateValidationException, "Grouping requires at least one aggregated column.");
			AddString(DataAccessStringId.SkipWithoutSortingValidationException, "The SKIP setting is set while records are not sorted. Please apply sorting to be able to skip the first N records or reset the SKIP setting in the Filter Editor dialog.");
			AddString(DataAccessStringId.RelationException, "Cannot set relation between columns '{0}'.'{1}' of type {2} and '{3}'.'{4}' of type {5}.");
			AddString(DataAccessStringId.ProviderNotSupportedException, "The following database provider is not supported: {0}.");
			AddString(DataAccessStringId.GatheringTypesPanelText, "Gathering types information...");
			AddString(DataAccessStringId.ParameterlessConstructor, "default");
			AddString(DataAccessStringId.ODSWizardErrorNoDefaultCtor, "Cannot get item properties from IListSource without default constructor.");
			AddString(DataAccessStringId.ODSWizardErrorExceptionInCtor, "The default constructor of an ITypedList implementation has thrown an exception.");
			AddString(DataAccessStringId.ODSWizardErrorExceptionInGetItemProperties, "The GetItemProperties method of an ITypedList has thrown an exception.");
			AddString(DataAccessStringId.ODSWizardErrorStaticValue, "The type is static and it does not have any members returning IEnumerable.");
			AddString(DataAccessStringId.ODSErrorMissingMember, "A data member '{0}' does not exist.");
			AddString(DataAccessStringId.ParameterListNull, "null");
			AddString(DataAccessStringId.ParameterListEmpty, "none");
			AddString(DataAccessStringId.MessageNonexistentQuery, "Query '{0}' does not exist.");
			AddString(DataAccessStringId.MessageNonexistentColumn, "Column '{0}'.'{1}' does not exist.");
			AddString(DataAccessStringId.QueryBuilderAliasAlreadyExists, "The specified alias already exists.");
			AddString(DataAccessStringId.QueryBuilderCanJoin, "Can join {0}");
			AddString(DataAccessStringId.QueryBuilderJoinedOn, "Joined on {0}");
			AddString(DataAccessStringId.QueryBuilderNothingSelected, "No column has been selected. Please select at least one column to proceed.");
			AddString(DataAccessStringId.QueryBuilderJoinEditorMissingData, "Not all parts of the expression are specified.");
			AddString(DataAccessStringId.QueryBuilderInvalidFilter, "Please click the 'Filter...' button to correct the filter string.");
			AddString(DataAccessStringId.QueryBuilderNoTablesAndViews, "A database does not contain tables or views and does not support SQL editing. To be able to run the Query Builder, make sure that your database contains at least one table or supports SQL editing.");
			AddString(DataAccessStringId.WizardNoEFDataContextsMessage, "The selected assembly does not contain Entity Framework data contexts.");
			AddString(DataAccessStringId.WizardCustomConnectionString, "Custom connection string");
			AddString(DataAccessStringId.ExcelEncryptedFileException_PasswordRequired, "A password is required to open this workbook.");
			AddString(DataAccessStringId.ExcelEncryptedFileException_WrongPassword, "The password is not correct. Please try again.");
			AddString(DataAccessStringId.ExcelEncryptedFileException_EncryptionTypeNotSupported, "The protected file cannot be opened.");
			AddString(DataAccessStringId.ExcelDataSource_SchemaLoadingText, "Schema loading...");
			AddString(DataAccessStringId.XmlFileStrategy_FileNameFilter, "XML Files");
			AddString(DataAccessStringId.FileNameFilter_AllFormats, "All Supported Formats");
			AddString(DataAccessStringId.FileNameFilter_Excel, "Excel Workbooks");
			AddString(DataAccessStringId.FileNameFilter_CSV, "CSV Files");
			AddString(DataAccessStringId.ResultRelation_ColumnNotFoundError, "Column not found: '{0}'.'{1}'.");
			AddString(DataAccessStringId.ExcelDataSource_FileNotFoundMessage, "{0}\r\nFile not found.\r\nCheck the file name and try again.");
			AddString(DataAccessStringId.ConfigureQueryPage_CustomSqlWillBeLost, "Manual editing of custom SQL is not allowed. The current query is represented by a custom SQL query string, \r\nwhich will be lost if you proceed with editing the query using the Query Builder dialog. \r\n\r\nDo you want to discard the custom SQL and proceed with the Query Builder?");
		}
	}
	 #endregion
}
