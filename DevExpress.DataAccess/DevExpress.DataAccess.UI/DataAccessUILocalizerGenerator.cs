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
namespace DevExpress.DataAccess.UI.Localization {
	#region enum DataAccessUIStringId
	public enum DataAccessUIStringId {
		MessageBoxWarningTitle,
		MessageBoxErrorTitle,
		MessageBoxConfirmationTitle,
		WizardDataSourceNameExistsMessage,
		WizardCannotConnectMessage,
		WizardCannotRetrieveDatabasesMessage,
		WizardDataSourceCreatedMessage,
		WizardFinishPageText,
		WizardCannotCreateDataSourceMessage,
		QueryDesignControlRemoveTables,
		QueryDesignControlExpressionChanged,
		QueryDesignControlNoSelection,
		QueryDesignControlAliasAlreadyExists,
		QueryDesignControlTableNameAlreadyExists,
		QueryDesignControlTableNameEmpty,
		QueryDesignControlDataPreviewCaption,
		QueryDesignControlDataPreviewCaption_All,
		QueryDesignControlJoinCommandPattern,
		QueryBuilder,
		QueryBuilderColumnsOf,
		QueryBuilderTable,
		QueryBuilderColumn,
		QueryBuilder_AllowEdit,
		QueryBuilder_Rename,
		QueryBuilder_Delete,
		QueryBuilderColumns_Alias,
		QueryBuilderColumns_Output,
		QueryBuilderColumns_SortingType,
		QueryBuilderColumns_SortOrder,
		QueryBuilderColumns_GroupBy,
		QueryBuilderColumns_Aggregate,
		QueryBuilderButtons_Filter,
		QueryBuilderButtons_PreviewResults,
		FiltersView_Filter,
		FiltersView_GroupFilter,
		FiltersView,
		FiltersView_CheckText,
		FiltersView_TopAndSkipText,
		JoinInformation,
		JoinEditor,
		JoinEditorEmptyColumnText,
		JoinEditorEmptyTableText,
		JoinEditorEqualOperator,
		JoinEditorNotEqualOperator,
		JoinEditorGreaterOperator,
		JoinEditorGreaterOrEqualOperator,
		JoinEditorLessOperator,
		JoinEditorLessOrEqualOperator,
		JoinEditorFillAllFieldsException,
		ErrorFormDatasourceInitializationText,
		ConnectionErrorFormDetailPattern,
		DefaultNameParameter,
		DataSourceName,
		LoadingDataSqlError,
		LoadingDataCustomError,
		LoadingDataSourceOpeningConnectionError,
		LoadingDatasourceOutOfMemoryError,
		LoadingDataSourceAborted,
		LoadingDataException,
		WizardDataSourceEmptyNameMessage,
		RebuildResultSchemaConfirmationText,
		RebuildResultSchemaCaption,
		RebuildResultSchemaComplete,
		UpdateDBSchemaCaption,
		UpdateDBSchemaComplete,
		ConnectionEditorTitle,
		EFConnectionEditorTitle,
		MessageMissingConnection,
		EFDataMemberEditorTitle,
		MessageLessThanTwoQueries,
		QueryEditorTitle,
		QueryEditorMessageInvalidQuery,
		QueryEditorWaitFormValidating,
		WizardEmptyStoredProceduresListMessage,
		WizardPageChooseDSType,
		WizardPageChooseDataSourceName,
		WizardPageChooseConnection,
		WizardPageChooseConnectionNoChoice,
		WizardPageChooseConnection_SpecifyCustomConnection,
		WizardPageChooseConnection_ChooseExistingConnection,
		WizardPageChooseEFConnectionString,
		WizardPageChooseEFConnectionString_CustomConnection,
		WizardPageChooseEFConnectionString_ChooseConnection,
		WizardPageEFConnectionProperties,
		WizardPageEFConnectionProperties_DefaultConnection,
		WizardPageEFConnectionProperties_CustomConnection,
		WizardPageEFConnectionProperties_SaveToConfig,
		WizardPageChooseEFContext,
		WizardPageChooseEFDataMember,
		WizardPageConfigureQuery,
		WizardPageConfigureQuery_Query,
		WizardPageConfigureQuery_StoredProcedure,
		WizardPageConfigureParameters,
		WizardPageConfigureStoredProcedures,
		WizardConfirmExecutionMessage,
		WizardTitleDatasource,
		WizardPageConnectionProperties,
		WizardPageSaveConnection,
		WizardPageSaveConnection_SaveConnectionString,
		WizardPageSaveConnection_ConfirmSaving,
		WizardPageSaveConnection_SaveCredentialsQuestion,
		WizardPageSaveConnection_SaveCredentials,
		WizardPageSaveConnection_SkipSaveCredentials,
		WizardPageChooseExcelFileDataRange,
		WizardPageConfigureExcelFileColumns,
		WizardPageConfigureExcelFileColumns_ColumnNameEmptyError,
		WizardPageConfigureExcelFileColumns_ColumnExist,
		EFDataSourceEditorTitle,
		EFDataSourceDesignerVerbEdit,
		SqlDataSourceDesignerVerbEditConnection,
		SqlDataSourceDesignerVerbManageQueries,
		SqlDataSourceDesignerVerbManageRelations,
		SqlDataSourceDesignerVerbRebuildSchema,
		SqlDataSourceDesignerVerbRequestDatabaseSchema,
		MasterDetailEditorAddRelationMessage,
		MasterDetailEditorAddConditionMessage,
		MasterDetailEditorRemoveRelationMessage,
		MasterDetailEditorRemoveConditionMessage,
		MasterDetailEditorInvalidQueryNameMessage,
		MasterDetailEditorInvalidColumnNameMessage,
		MasterDetailEditorInvalidColumnQueryNameMessage,
		MasterDetailEditorInvalidRelationNameMessage,
		MasterDetailEditorColumnsHasDifferentTypesConfirmation,
		ODSDataSourceEditorTitle,
		ODSDataMemberEditorTitle,
		ODSParametersEditorTitle,
		ODSConstructorEditorTitle,
		WizardTitleODS,
		ODSEditorsNoDataSetMessage,
		ODSEditorsNoDataMemberMessage,
		ODSEditorsIsPropertyMessage,
		ODSEditorsNoParametersMessage,
		ODSEditorsAbstractTypeMessage,
		ODSEditorsStaticMemberMessage,
		ODSEditorsCannotResolveDataSource,
		ODSEditorsCannotResolveDataMember,
		ODSEditorsNoMembersInType,
		ExcelDataSourceEditorTitle,
		WizardPageChooseObjectAssembly,
		WizardPageChooseObjectAssembly_ShowOnlyHighlighted,
		WizardPageChooseObjectType,
		WizardPageChooseObjectType_ShowOnlyHighlighted,
		WizardPageChooseObjectMember,
		WizardPageChooseObjectMember_BindToObject,
		WizardPageChooseObjectMember_BindToMember,
		WizardPageChooseObjectMember_ShowOnlyHighlighted,
		WizardPageObjectMemberParameters,
		WizardPageChooseObjectBindingMode,
		WizardPageChooseObjectBindingMode_RetrieveSchema,
		WizardPageChooseObjectBindingMode_RetrieveSchemaDescription,
		WizardPageChooseObjectBindingMode_RetrieveData,
		WizardPageChooseObjectBindingMode_RetrieveDataDescription,
		WizardPageChooseObjectConstructor,
		WizardPageChooseObjectConstructor_ShowOnlyHighlighted,
		WizardPageChooseFileOptions,
		WizardPageChooseFileOptions_Options,
		WizardPageChooseFileOptions_FirstRowAsFieldNames,
		WizardPageChooseFileOptions_SkipEmptyRows,
		WizardPageChooseFileOptions_Encoding,
		WizardPageChooseFileOptions_NewLineType,
		WizardPageChooseFileOptions_ValueSeparator,
		WizardPageChooseFileOptions_Culture,
		WizardPageChooseFileOptions_TextQualifier,
		WizardPageChooseFileOptions_TrimBlanks,
		WizardPageChooseFileOptions_SkipHiddenRows,
		WizardPageChooseFileOptions_SkipHiddenColumns,
		WizardPageChooseFileOptions_DetectAutomatically,
		WizardPageChooseFile,
		WizardPageObjectConstructorParameters,
		DSTypeSql,
		DSTypeEF,
		DSTypeObject,
		DSTypeExcel,
		EFEditorsNoStoredProcs,
		EFStoredProcsEditorTitle,
		SortingTypeNone,
		SortingTypeAscending,
		SortingTypeDescending,
		MessageCannotLoadDatabasesList,
		WizardDuplicatingColumnNameMessage,
		PasswordRequest,
		PasswordRequest_FileName,
		PasswordRequest_Password,
		PasswordRequest_SavePassword,
		QueryControl_SqlString,
		StoredProcControl_Caption,
		QueryFilter_AddQueryParameter,
		QueryFilter_BindTo,
		QueryFilter_CreateQueryParameter,
		QueryFilter_BoundTo,
		QueryFilter_SelectParameter,
		ExcelDataSourceWizard_WorksheetItem,
		ExcelDataSourceWizard_DefinedNameItem,
		ExcelDataSourceWizard_TableItem,
		ExcelDataSourceWizard_Title,
		JoinEditor_JoinType,
		ChooseEFStoredProceduresDialog,
		DataConnectionParametersDialog,
		DataConnectionParametersDialog_Header_UnableConnect,
		MasterDetailEditorForm_Title,
		SqlQueryCollectionEditorForm_Title,
		ParametersGridForm_Title,
		DataPreviewForm_Title,
		WaitFormWithCancel_Loading,
		ConnectionProperties_Provider,
		ConnectionProperties_ServerType,
		ConnectionProperties_ServerTypeServer,
		ConnectionProperties_ServerTypeEmbedded,
		ConnectionProperties_Database,
		ConnectionProperties_ServerName,
		ConnectionProperties_Port,
		ConnectionProperties_AuthenticationType,
		ConnectionProperties_AuthenticationType_MSSqlWindows,
		ConnectionProperties_AuthenticationType_MSSqlServer,
		ConnectionProperties_AuthenticationType_BigQueryOAuth,
		ConnectionProperties_AuthenticationType_BigQueryKeyFile,
		ConnectionProperties_UserName,
		ConnectionProperties_Password,
		ConnectionProperties_ProjectID,
		ConnectionProperties_KeyFileName,
		ConnectionProperties_ServiceAccountEmail,
		ConnectionProperties_ClientID,
		ConnectionProperties_ClientSecret,
		ConnectionProperties_RefreshToken,
		ConnectionProperties_DataSetID,
		ConnectionProperties_ConnectionString,
		ConnectionProperties_Hostname,
		ConnectionProperties_AdvantageServerType,
		ConnectionProperties_AdvantageServerTypeLocal,
		ConnectionProperties_AdvantageServerTypeRemote,
		ConnectionProperties_AdvantageServerTypeInternet,
		ParametersColumn_Name,
		ParametersColumn_Type,
		ParametersColumn_Expression,
		ParametersColumn_Value,
		ParametersColumn_Selected,
		ParametersColumn_Version,
		Button_Add,
		Button_Remove,
		Button_OK,
		Button_Cancel,
		Button_Preview,
		Button_Browse,
		Button_QueryBuilder,
		Button_Previous,
		Button_Next,
		Button_Finish,
		Button_Close,
	}
	#endregion
	#region DataAccessUILocalizer.AddStrings 
	public partial class DataAccessUILocalizer {
		void AddStrings() {
			AddString(DataAccessUIStringId.MessageBoxWarningTitle, "Warning");
			AddString(DataAccessUIStringId.MessageBoxErrorTitle, "Error");
			AddString(DataAccessUIStringId.MessageBoxConfirmationTitle, "Confirmation");
			AddString(DataAccessUIStringId.WizardDataSourceNameExistsMessage, "<b>A data source with the specified name already exists</b>");
			AddString(DataAccessUIStringId.WizardCannotConnectMessage, "Cannot connect to the database. See the details below.");
			AddString(DataAccessUIStringId.WizardCannotRetrieveDatabasesMessage, "Cannot retrieve the list of available databases. See the details below.");
			AddString(DataAccessUIStringId.WizardDataSourceCreatedMessage, "The data source has been successfully created");
			AddString(DataAccessUIStringId.WizardFinishPageText, "Select an object to include in your data source");
			AddString(DataAccessUIStringId.WizardCannotCreateDataSourceMessage, "Cannot create an empty data source. Please select an object to include.");
			AddString(DataAccessUIStringId.QueryDesignControlRemoveTables, "The following tables will be removed from the query.\r\n\r\n{0}\r\nDo you want to continue?");
			AddString(DataAccessUIStringId.QueryDesignControlExpressionChanged, "This action will reset the SQL expression, and all your changes will be discarded. Do you want to continue?");
			AddString(DataAccessUIStringId.QueryDesignControlNoSelection, "No column has been selected. Please select at least one column to proceed.");
			AddString(DataAccessUIStringId.QueryDesignControlAliasAlreadyExists, "The specified alias already exists. Do you want to correct the value?");
			AddString(DataAccessUIStringId.QueryDesignControlTableNameAlreadyExists, "A table with the specified name already exists. Please specify a different table name.");
			AddString(DataAccessUIStringId.QueryDesignControlTableNameEmpty, "The table name cannot be empty.");
			AddString(DataAccessUIStringId.QueryDesignControlDataPreviewCaption, "Data Preview (First 1000 Rows Displayed)");
			AddString(DataAccessUIStringId.QueryDesignControlDataPreviewCaption_All, "Data Preview");
			AddString(DataAccessUIStringId.QueryDesignControlJoinCommandPattern, "Join {0}");
			AddString(DataAccessUIStringId.QueryBuilder, "Query Builder");
			AddString(DataAccessUIStringId.QueryBuilderColumnsOf, "Columns of {0}");
			AddString(DataAccessUIStringId.QueryBuilderTable, "Table");
			AddString(DataAccessUIStringId.QueryBuilderColumn, "Column");
			AddString(DataAccessUIStringId.QueryBuilder_AllowEdit, "&Allow Edit SQL");
			AddString(DataAccessUIStringId.QueryBuilder_Rename, "Rename");
			AddString(DataAccessUIStringId.QueryBuilder_Delete, "Delete");
			AddString(DataAccessUIStringId.QueryBuilderColumns_Alias, "Alias");
			AddString(DataAccessUIStringId.QueryBuilderColumns_Output, "Output");
			AddString(DataAccessUIStringId.QueryBuilderColumns_SortingType, "Sorting Type");
			AddString(DataAccessUIStringId.QueryBuilderColumns_SortOrder, "Sort Order");
			AddString(DataAccessUIStringId.QueryBuilderColumns_GroupBy, "Group By");
			AddString(DataAccessUIStringId.QueryBuilderColumns_Aggregate, "Aggregate");
			AddString(DataAccessUIStringId.QueryBuilderButtons_Filter, "&Filter...");
			AddString(DataAccessUIStringId.QueryBuilderButtons_PreviewResults, "&Preview Results...");
			AddString(DataAccessUIStringId.FiltersView_Filter, "&Filter");
			AddString(DataAccessUIStringId.FiltersView_GroupFilter, "&Group Filter");
			AddString(DataAccessUIStringId.FiltersView, "Filter Editor");
			AddString(DataAccessUIStringId.FiltersView_CheckText, "&Select only");
			AddString(DataAccessUIStringId.FiltersView_TopAndSkipText, "records starting with index");
			AddString(DataAccessUIStringId.JoinInformation, "Join Information");
			AddString(DataAccessUIStringId.JoinEditor, "Join Editor");
			AddString(DataAccessUIStringId.JoinEditorEmptyColumnText, "<Select a column>");
			AddString(DataAccessUIStringId.JoinEditorEmptyTableText, "<Select a table>");
			AddString(DataAccessUIStringId.JoinEditorEqualOperator, "Equals to");
			AddString(DataAccessUIStringId.JoinEditorNotEqualOperator, "Does not equal to");
			AddString(DataAccessUIStringId.JoinEditorGreaterOperator, "Is greater than");
			AddString(DataAccessUIStringId.JoinEditorGreaterOrEqualOperator, "Is greater than or equal to");
			AddString(DataAccessUIStringId.JoinEditorLessOperator, "Is less than");
			AddString(DataAccessUIStringId.JoinEditorLessOrEqualOperator, "Is less than or equal to");
			AddString(DataAccessUIStringId.JoinEditorFillAllFieldsException, "Some fields are empty. Please fill all empty fields or remove the corresponding conditions to proceed.");
			AddString(DataAccessUIStringId.ErrorFormDatasourceInitializationText, "Unable to load data into one or several datasources. See information below for details.");
			AddString(DataAccessUIStringId.ConnectionErrorFormDetailPattern, "Connection name: {0}\r\nError message: {1}");
			AddString(DataAccessUIStringId.DefaultNameParameter, "Parameter");
			AddString(DataAccessUIStringId.DataSourceName, "Data source name: {0}");
			AddString(DataAccessUIStringId.LoadingDataSqlError, "SQL execution error:\r\n{0}\r\n");
			AddString(DataAccessUIStringId.LoadingDataCustomError, "Error message:\r\n{0}\r\n");
			AddString(DataAccessUIStringId.LoadingDataSourceOpeningConnectionError, "Connection name: {0}\r\nError message:\r\n{1}\r\n");
			AddString(DataAccessUIStringId.LoadingDatasourceOutOfMemoryError, "Not enough memory to load data");
			AddString(DataAccessUIStringId.LoadingDataSourceAborted, "Data loading has been aborted.");
			AddString(DataAccessUIStringId.LoadingDataException, "Data loading failed because the exception occurred.");
			AddString(DataAccessUIStringId.WizardDataSourceEmptyNameMessage, "<b>The data source name cannot be empty.</b>");
			AddString(DataAccessUIStringId.RebuildResultSchemaConfirmationText, "Do you want to execute the following queries on the server and obtain the resulting schema?");
			AddString(DataAccessUIStringId.RebuildResultSchemaCaption, "Rebuild Result Schema");
			AddString(DataAccessUIStringId.RebuildResultSchemaComplete, "Result schema is rebuilt successfully.");
			AddString(DataAccessUIStringId.UpdateDBSchemaCaption, "Request Database Schema");
			AddString(DataAccessUIStringId.UpdateDBSchemaComplete, "Database schema is updated successfully.");
			AddString(DataAccessUIStringId.ConnectionEditorTitle, "Connection Editor");
			AddString(DataAccessUIStringId.EFConnectionEditorTitle, "Entity Framework Data Connection Editor");
			AddString(DataAccessUIStringId.MessageMissingConnection, "No connection has been specified.");
			AddString(DataAccessUIStringId.EFDataMemberEditorTitle, "Entity Framework Data Member Editor");
			AddString(DataAccessUIStringId.MessageLessThanTwoQueries, "At least two queries are required to create a master-detail relation.");
			AddString(DataAccessUIStringId.QueryEditorTitle, "Query Editor");
			AddString(DataAccessUIStringId.QueryEditorMessageInvalidQuery, "Query validation failed with message:\r\n{0}\r\n\r\nTo discard this query and begin from scratch, click OK.\r\nTo close the visual editor and keep the current query, click Cancel.");
			AddString(DataAccessUIStringId.QueryEditorWaitFormValidating, "Validating query...");
			AddString(DataAccessUIStringId.WizardEmptyStoredProceduresListMessage, "There are no any stored procedures in current database.");
			AddString(DataAccessUIStringId.WizardPageChooseDSType, "Select the data source type.");
			AddString(DataAccessUIStringId.WizardPageChooseDataSourceName, "Enter the data source name");
			AddString(DataAccessUIStringId.WizardPageChooseConnection, "Do you want to use an existing data connection?");
			AddString(DataAccessUIStringId.WizardPageChooseConnectionNoChoice, "Select a data connection.");
			AddString(DataAccessUIStringId.WizardPageChooseConnection_SpecifyCustomConnection, "No, I'd like to specify the connection parameters myself");
			AddString(DataAccessUIStringId.WizardPageChooseConnection_ChooseExistingConnection, "Yes, let me choose an existing connection from the list");
			AddString(DataAccessUIStringId.WizardPageChooseEFConnectionString, "Do you want to select a connection string from the list of available settings?");
			AddString(DataAccessUIStringId.WizardPageChooseEFConnectionString_CustomConnection, "No, specify a custom connection string");
			AddString(DataAccessUIStringId.WizardPageChooseEFConnectionString_ChooseConnection, "Yes, let me choose from list");
			AddString(DataAccessUIStringId.WizardPageEFConnectionProperties, "Specify a connection string.");
			AddString(DataAccessUIStringId.WizardPageEFConnectionProperties_DefaultConnection, "Use default connection string");
			AddString(DataAccessUIStringId.WizardPageEFConnectionProperties_CustomConnection, "Specify a custom connection string");
			AddString(DataAccessUIStringId.WizardPageEFConnectionProperties_SaveToConfig, "Save the connection string to config file as:");
			AddString(DataAccessUIStringId.WizardPageChooseEFContext, "Choose an existing data context or browse for an assembly.");
			AddString(DataAccessUIStringId.WizardPageChooseEFDataMember, "Select a data member");
			AddString(DataAccessUIStringId.WizardPageConfigureQuery, "Create a query or select a stored procedure.");
			AddString(DataAccessUIStringId.WizardPageConfigureQuery_Query, "Query");
			AddString(DataAccessUIStringId.WizardPageConfigureQuery_StoredProcedure, "Stored Procedure");
			AddString(DataAccessUIStringId.WizardPageConfigureParameters, "Configure query parameters and preview the result.");
			AddString(DataAccessUIStringId.WizardPageConfigureStoredProcedures, "Add stored procedures to the data source, configure their parameters and preview the result.");
			AddString(DataAccessUIStringId.WizardConfirmExecutionMessage, "Do you want to execute the query on the server and obtain the resulting query schema?");
			AddString(DataAccessUIStringId.WizardTitleDatasource, "Data Source Wizard");
			AddString(DataAccessUIStringId.WizardPageConnectionProperties, "Select the data provider and specify the connection properties.");
			AddString(DataAccessUIStringId.WizardPageSaveConnection, "Save the connection string.");
			AddString(DataAccessUIStringId.WizardPageSaveConnection_SaveConnectionString, "Do you want to save the connection string to the application's configuration file?");
			AddString(DataAccessUIStringId.WizardPageSaveConnection_ConfirmSaving, "Yes, save the connection as:");
			AddString(DataAccessUIStringId.WizardPageSaveConnection_SaveCredentialsQuestion, "The connection uses server authentication.\r\nDo you want to save the user name and password?");
			AddString(DataAccessUIStringId.WizardPageSaveConnection_SaveCredentials, "Yes, save all required parameters");
			AddString(DataAccessUIStringId.WizardPageSaveConnection_SkipSaveCredentials, "No, skip credentials for security reasons");
			AddString(DataAccessUIStringId.WizardPageChooseExcelFileDataRange, "Select the required worksheet, table or defined name referring to the specified range.");
			AddString(DataAccessUIStringId.WizardPageConfigureExcelFileColumns, "Select required columns and specify their settings.");
			AddString(DataAccessUIStringId.WizardPageConfigureExcelFileColumns_ColumnNameEmptyError, "The column name cannot be empty.");
			AddString(DataAccessUIStringId.WizardPageConfigureExcelFileColumns_ColumnExist, "Column with '{0}' name allready exists.");
			AddString(DataAccessUIStringId.EFDataSourceEditorTitle, "Entity Framework Data Source Editor");
			AddString(DataAccessUIStringId.EFDataSourceDesignerVerbEdit, "Edit...");
			AddString(DataAccessUIStringId.SqlDataSourceDesignerVerbEditConnection, "Configure Connection...");
			AddString(DataAccessUIStringId.SqlDataSourceDesignerVerbManageQueries, "Manage Queries...");
			AddString(DataAccessUIStringId.SqlDataSourceDesignerVerbManageRelations, "Manage Relations...");
			AddString(DataAccessUIStringId.SqlDataSourceDesignerVerbRebuildSchema, "Rebuild Schema");
			AddString(DataAccessUIStringId.SqlDataSourceDesignerVerbRequestDatabaseSchema, "Request Database Schema");
			AddString(DataAccessUIStringId.MasterDetailEditorAddRelationMessage, "Add a relation to the detail query");
			AddString(DataAccessUIStringId.MasterDetailEditorAddConditionMessage, "Create a new condition");
			AddString(DataAccessUIStringId.MasterDetailEditorRemoveRelationMessage, "Remove the relation");
			AddString(DataAccessUIStringId.MasterDetailEditorRemoveConditionMessage, "Remove the condition");
			AddString(DataAccessUIStringId.MasterDetailEditorInvalidQueryNameMessage, "Cannot find the specified query: \"{0}\". Specify an existing query name.");
			AddString(DataAccessUIStringId.MasterDetailEditorInvalidColumnNameMessage, "Cannot find the specified column: \"{0}\". Specify an existing column name.");
			AddString(DataAccessUIStringId.MasterDetailEditorInvalidColumnQueryNameMessage, "Cannot find the specified query: \"{0}\". Specify an existing query name before selecting a column.");
			AddString(DataAccessUIStringId.MasterDetailEditorInvalidRelationNameMessage, "A relation with the specified name already exists.");
			AddString(DataAccessUIStringId.MasterDetailEditorColumnsHasDifferentTypesConfirmation, "The '{0}'.'{1}' and '{3}'.'{4}' columns have different types ({2} and {5}). Do you wish to continue?");
			AddString(DataAccessUIStringId.ODSDataSourceEditorTitle, "Configure Data Source");
			AddString(DataAccessUIStringId.ODSDataMemberEditorTitle, "Configure Data Member");
			AddString(DataAccessUIStringId.ODSParametersEditorTitle, "Configure Parameters");
			AddString(DataAccessUIStringId.ODSConstructorEditorTitle, "Configure Constructor Settings");
			AddString(DataAccessUIStringId.WizardTitleODS, "ObjectDataSource Wizard");
			AddString(DataAccessUIStringId.ODSEditorsNoDataSetMessage, "The current operation cannot be accomplished unless the data source has been specified.");
			AddString(DataAccessUIStringId.ODSEditorsNoDataMemberMessage, "Cannot specify parameters unless the DataMember property has been assigned a value.");
			AddString(DataAccessUIStringId.ODSEditorsIsPropertyMessage, "The {0} data member is a property. To be able to receive parameters, a data member must be a method.");
			AddString(DataAccessUIStringId.ODSEditorsNoParametersMessage, "The {0} data member cannot receive parameters, because this is a method without parameters.");
			AddString(DataAccessUIStringId.ODSEditorsAbstractTypeMessage, "Cannot use a constructor to create an object of the {0} type that is a static or abstract class, or interface.");
			AddString(DataAccessUIStringId.ODSEditorsStaticMemberMessage, "{0} is a static member. Using a constructor to create an instance of the {1} class is not appropriate.");
			AddString(DataAccessUIStringId.ODSEditorsCannotResolveDataSource, "Cannot resolve the specified object type. To specify the object’s assembly, return type, data member and/or constructor parameters, use the Data Source editor.");
			AddString(DataAccessUIStringId.ODSEditorsCannotResolveDataMember, "Cannot resolve the specified data member. To specify the data member and its parameters, use the Data Member editor.");
			AddString(DataAccessUIStringId.ODSEditorsNoMembersInType, "The {0} data source type does not provide any members suitable for binding.");
			AddString(DataAccessUIStringId.ExcelDataSourceEditorTitle, "Excel Data Source Editor");
			AddString(DataAccessUIStringId.WizardPageChooseObjectAssembly, "Select an assembly containing the class type definition of a data source.");
			AddString(DataAccessUIStringId.WizardPageChooseObjectAssembly_ShowOnlyHighlighted, "Show only highlighted assemblies");
			AddString(DataAccessUIStringId.WizardPageChooseObjectType, "Select a data source type.");
			AddString(DataAccessUIStringId.WizardPageChooseObjectType_ShowOnlyHighlighted, "Show only highlighted types");
			AddString(DataAccessUIStringId.WizardPageChooseObjectMember, "Select a data source member (if required).");
			AddString(DataAccessUIStringId.WizardPageChooseObjectMember_BindToObject, "Do not select a member, bind to the entire object.");
			AddString(DataAccessUIStringId.WizardPageChooseObjectMember_BindToMember, "Select a member to bind.");
			AddString(DataAccessUIStringId.WizardPageChooseObjectMember_ShowOnlyHighlighted, "Show only highlighted members");
			AddString(DataAccessUIStringId.WizardPageObjectMemberParameters, "Specify the method parameters.");
			AddString(DataAccessUIStringId.WizardPageChooseObjectBindingMode, "Select the data binding mode.");
			AddString(DataAccessUIStringId.WizardPageChooseObjectBindingMode_RetrieveSchema, "Retrieve the data source schema");
			AddString(DataAccessUIStringId.WizardPageChooseObjectBindingMode_RetrieveSchemaDescription, "Only the data source schema is retrieved from the specified object, without feeding the actual data.\r\n\r\nTo manually retrieve the actual data, create a data source object’s instance in code and assign it to the ObjectDataSource.DataSource property.");
			AddString(DataAccessUIStringId.WizardPageChooseObjectBindingMode_RetrieveData, "Retrieve the actual data");
			AddString(DataAccessUIStringId.WizardPageChooseObjectBindingMode_RetrieveDataDescription, "The object data source automatically creates an instance of the specified type by using one of the available constructors. If only one constructor is available, this constructor will be used.");
			AddString(DataAccessUIStringId.WizardPageChooseObjectConstructor, "Select a data source constructor.");
			AddString(DataAccessUIStringId.WizardPageChooseObjectConstructor_ShowOnlyHighlighted, "Show only highlighted constructors");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions, "Specify import settings.");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_Options, "Options");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_FirstRowAsFieldNames, "Use values of the &first row as field names");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_SkipEmptyRows, "Skip &empty rows");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_Encoding, "&Encoding:");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_NewLineType, "&Newline type:");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_ValueSeparator, "Value &separator:");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_Culture, "&Culture:");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_TextQualifier, "Text &qualifier:");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_TrimBlanks, "Trim blanks");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_SkipHiddenRows, "Skip hidden &rows");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_SkipHiddenColumns, "Skip hidden &columns");
			AddString(DataAccessUIStringId.WizardPageChooseFileOptions_DetectAutomatically, "Detect automatically");
			AddString(DataAccessUIStringId.WizardPageChooseFile, "Select an Excel workbook or CSV file.");
			AddString(DataAccessUIStringId.WizardPageObjectConstructorParameters, "Specify the constructor parameters.");
			AddString(DataAccessUIStringId.DSTypeSql, "Database");
			AddString(DataAccessUIStringId.DSTypeEF, "Entity Framework");
			AddString(DataAccessUIStringId.DSTypeObject, "Object Binding");
			AddString(DataAccessUIStringId.DSTypeExcel, "Excel File");
			AddString(DataAccessUIStringId.EFEditorsNoStoredProcs, "No stored procedures are available in the data context.");
			AddString(DataAccessUIStringId.EFStoredProcsEditorTitle, "Manage Stored Procedures");
			AddString(DataAccessUIStringId.SortingTypeNone, "Unsorted");
			AddString(DataAccessUIStringId.SortingTypeAscending, "Ascending");
			AddString(DataAccessUIStringId.SortingTypeDescending, "Descending");
			AddString(DataAccessUIStringId.MessageCannotLoadDatabasesList, "Error has occurred during loading databases list.");
			AddString(DataAccessUIStringId.WizardDuplicatingColumnNameMessage, "A data schema contains a duplicated column '{0}'. Modify the query so that all columns have unique names.");
			AddString(DataAccessUIStringId.PasswordRequest, "Password");
			AddString(DataAccessUIStringId.PasswordRequest_FileName, "&File name:");
			AddString(DataAccessUIStringId.PasswordRequest_Password, "&Password:");
			AddString(DataAccessUIStringId.PasswordRequest_SavePassword, "&Save password");
			AddString(DataAccessUIStringId.QueryControl_SqlString, "SQL string:");
			AddString(DataAccessUIStringId.StoredProcControl_Caption, "Select a stored procedure:");
			AddString(DataAccessUIStringId.QueryFilter_AddQueryParameter, "Add Query Parameter");
			AddString(DataAccessUIStringId.QueryFilter_BindTo, "Bind To");
			AddString(DataAccessUIStringId.QueryFilter_CreateQueryParameter, "Create Query Parameter");
			AddString(DataAccessUIStringId.QueryFilter_BoundTo, "Bound to {0}");
			AddString(DataAccessUIStringId.QueryFilter_SelectParameter, "<select a parameter>");
			AddString(DataAccessUIStringId.ExcelDataSourceWizard_WorksheetItem, "Worksheet");
			AddString(DataAccessUIStringId.ExcelDataSourceWizard_DefinedNameItem, "Defined Name");
			AddString(DataAccessUIStringId.ExcelDataSourceWizard_TableItem, "Table");
			AddString(DataAccessUIStringId.ExcelDataSourceWizard_Title, "Table");
			AddString(DataAccessUIStringId.JoinEditor_JoinType, "Join type:");
			AddString(DataAccessUIStringId.ChooseEFStoredProceduresDialog, "Select stored procedures to add");
			AddString(DataAccessUIStringId.DataConnectionParametersDialog, "Connection error");
			AddString(DataAccessUIStringId.DataConnectionParametersDialog_Header_UnableConnect, "Unable to connect to the database. See details below.");
			AddString(DataAccessUIStringId.MasterDetailEditorForm_Title, "Master-Detail Relation Editor");
			AddString(DataAccessUIStringId.SqlQueryCollectionEditorForm_Title, "Manage Queries");
			AddString(DataAccessUIStringId.ParametersGridForm_Title, "Query Parameters");
			AddString(DataAccessUIStringId.DataPreviewForm_Title, "Data Preview");
			AddString(DataAccessUIStringId.WaitFormWithCancel_Loading, "Loading Data...");
			AddString(DataAccessUIStringId.ConnectionProperties_Provider, "Provider:");
			AddString(DataAccessUIStringId.ConnectionProperties_ServerType, "Server type:");
			AddString(DataAccessUIStringId.ConnectionProperties_ServerTypeServer, "Server");
			AddString(DataAccessUIStringId.ConnectionProperties_ServerTypeEmbedded, "Embedded");
			AddString(DataAccessUIStringId.ConnectionProperties_Database, "Database:");
			AddString(DataAccessUIStringId.ConnectionProperties_ServerName, "Server name:");
			AddString(DataAccessUIStringId.ConnectionProperties_Port, "Port:");
			AddString(DataAccessUIStringId.ConnectionProperties_AuthenticationType, "Authentication type:");
			AddString(DataAccessUIStringId.ConnectionProperties_AuthenticationType_MSSqlWindows, "Windows authentication");
			AddString(DataAccessUIStringId.ConnectionProperties_AuthenticationType_MSSqlServer, "Server authentication");
			AddString(DataAccessUIStringId.ConnectionProperties_AuthenticationType_BigQueryOAuth, "OAuth");
			AddString(DataAccessUIStringId.ConnectionProperties_AuthenticationType_BigQueryKeyFile, "Key file");
			AddString(DataAccessUIStringId.ConnectionProperties_UserName, "User name:");
			AddString(DataAccessUIStringId.ConnectionProperties_Password, "Password:");
			AddString(DataAccessUIStringId.ConnectionProperties_ProjectID, "Project ID:");
			AddString(DataAccessUIStringId.ConnectionProperties_KeyFileName, "Key file name:");
			AddString(DataAccessUIStringId.ConnectionProperties_ServiceAccountEmail, "Service account email:");
			AddString(DataAccessUIStringId.ConnectionProperties_ClientID, "Client ID:");
			AddString(DataAccessUIStringId.ConnectionProperties_ClientSecret, "Client Secret:");
			AddString(DataAccessUIStringId.ConnectionProperties_RefreshToken, "Refresh Token:");
			AddString(DataAccessUIStringId.ConnectionProperties_DataSetID, "DataSet ID:");
			AddString(DataAccessUIStringId.ConnectionProperties_ConnectionString, "Connection string:");
			AddString(DataAccessUIStringId.ConnectionProperties_Hostname, "Hostname:");
			AddString(DataAccessUIStringId.ConnectionProperties_AdvantageServerType, "Server type:");
			AddString(DataAccessUIStringId.ConnectionProperties_AdvantageServerTypeLocal, "Local");
			AddString(DataAccessUIStringId.ConnectionProperties_AdvantageServerTypeRemote, "Remote");
			AddString(DataAccessUIStringId.ConnectionProperties_AdvantageServerTypeInternet, "Internet");
			AddString(DataAccessUIStringId.ParametersColumn_Name, "Name");
			AddString(DataAccessUIStringId.ParametersColumn_Type, "Type");
			AddString(DataAccessUIStringId.ParametersColumn_Expression, "Expression");
			AddString(DataAccessUIStringId.ParametersColumn_Value, "Value");
			AddString(DataAccessUIStringId.ParametersColumn_Selected, "Selected");
			AddString(DataAccessUIStringId.ParametersColumn_Version, "Version");
			AddString(DataAccessUIStringId.Button_Add, "&Add");
			AddString(DataAccessUIStringId.Button_Remove, "&Remove");
			AddString(DataAccessUIStringId.Button_OK, "&OK");
			AddString(DataAccessUIStringId.Button_Cancel, "&Cancel");
			AddString(DataAccessUIStringId.Button_Preview, "&Preview...");
			AddString(DataAccessUIStringId.Button_Browse, "Browse...");
			AddString(DataAccessUIStringId.Button_QueryBuilder, "&Run Query Builder...");
			AddString(DataAccessUIStringId.Button_Previous, "&Previous");
			AddString(DataAccessUIStringId.Button_Next, "&Next");
			AddString(DataAccessUIStringId.Button_Finish, "&Finish");
			AddString(DataAccessUIStringId.Button_Close, "&Close");
		}
	}
	 #endregion
}
