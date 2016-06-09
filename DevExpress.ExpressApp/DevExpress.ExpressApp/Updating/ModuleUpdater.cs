#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Data;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Localization;
namespace DevExpress.ExpressApp.Updating {
	public class ModuleUpdater {
		public static readonly ModuleUpdater[] EmptyModuleUpdaters = new ModuleUpdater[0];
		private readonly IObjectSpace objectSpace;
		private readonly Version currentDBVersion;
		protected virtual IDbCommand CreateCommand(String commandText) {
			if(objectSpace is BaseObjectSpace) {
				BaseObjectSpace baseObjectSpace = (BaseObjectSpace)objectSpace;
				IDbCommand command = baseObjectSpace.Connection.CreateCommand();
				command.CommandText = commandText;
				return command;
			}
			else {
				return null;
			}
		}
		protected virtual void OnStatusUpdating(String context, String title, String message, params Object[] additionalParams) {
			if(StatusUpdating != null) {
				StatusUpdating(this, new StatusUpdatingEventArgs(context, title, message, additionalParams));
			}
		}
		protected int ExecuteNonQueryCommand(string commandText, bool silent) {
			Tracing.Tracer.LogText("ExecuteNonQueryCommand: '{0}', silent={1}", commandText, silent);
			IDbCommand command = CreateCommand(commandText);
			try {
				return command.ExecuteNonQuery();
			}
			catch(Exception exception) {
				Tracing.Tracer.LogError(exception);
				if(!silent) {
					throw;
				}
				else {
					return -1;
				}
			}
		}
		protected void DeleteObjectType(string typeName) {
			Tracing.Tracer.LogText("DeleteObjectType '{0}'", typeName);
			ExecuteNonQueryCommand("delete from XPObjectType where TypeName = '" + typeName + "' ", true);
		}
		protected void DropTable(string name, bool silent) {
			Tracing.Tracer.LogText("DropTable '{0}'", name);
			if(IsTableExists(name)) {
				ExecuteNonQueryCommand("drop table [" + name + "]", silent);
			}
		}
		protected void DropConstraint(string tableName, string constraintName) {
			Tracing.Tracer.LogText("DropConstraint {0}, {1}", tableName, constraintName);
			ExecuteNonQueryCommand("alter table [" + tableName + "] drop constraint " + constraintName, true);
		}
		protected void RenameTable(string sourceTableName, string destinationTableName) {
			Tracing.Tracer.LogText("RenameTable '{0}' to '{1}'", sourceTableName, destinationTableName);
			ExecuteNonQueryCommand("sp_rename '" + sourceTableName + "', '" + destinationTableName + "', 'OBJECT'", true);
		}
		protected void DropIndex(string tableName, string indexName) {
			ExecuteNonQueryCommand("drop index " + indexName + " on " + tableName, true);
		}
		protected void RenameColumn(string tableName, string currentColumnName, string newColumnName) {
			ExecuteNonQueryCommand("EXECUTE sp_rename N'" + tableName + "." + currentColumnName + "', N'" + newColumnName + "', 'COLUMN'", true);
		}
		protected void DropColumn(string tableName, string columnName) {
			ExecuteNonQueryCommand("ALTER TABLE dbo." + tableName + " DROP COLUMN [" + columnName + "]", true);
		}
		protected IDataReader ExecuteReader(string commandText, bool silent) {
			Tracing.Tracer.LogText("ExecuteReader, command: '{0}', silent='{1}'", commandText, silent);
			IDbCommand command = CreateCommand(commandText);
			try {
				return command.ExecuteReader();
			}
			catch {
				if(!silent) {
					throw;
				}
				else {
					return null;
				}
			}
		}
		protected object ExecuteScalarCommand(string commandText, bool silent) {
			Tracing.Tracer.LogText("ExecuteScalarCommand: '{0}', silent={1}", commandText, silent);
			IDbCommand command = CreateCommand(commandText);
			try {
				return command.ExecuteScalar();
			}
			catch {
				if(!silent) {
					throw;
				}
				else {
					return -1;
				}
			}
		}
		protected bool IsTableExists(string name) {
			Tracing.Tracer.LogText("Check is table '{0}' exists", name);
			try {
				object res = ExecuteScalarCommand("SELECT count(1) FROM sysobjects WHERE name = '" + name + "' AND type = 'U'", true);
				return ((int)res) > 0;
			}
			catch {
				return false;
			}
		}
		protected void UpdateXPObjectType(String oldTypeName, String newTypeName, String assemblyName) {
			Tracing.Tracer.LogText("UpdateXPObjectType '{0}' to '{1}', assembly '{2}'", oldTypeName, newTypeName, assemblyName);
			ExecuteNonQueryCommand(
				"update XPObjectType set TypeName = '" + newTypeName + "', " +
				"AssemblyName = '" + assemblyName + "' where TypeName = '" + oldTypeName + "'", true);
		}
		protected IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
		protected Version CurrentDBVersion {
			get { return currentDBVersion; }
		}
		public ModuleUpdater(IObjectSpace objectSpace, Version currentDBVersion) {
			this.objectSpace = objectSpace;
			this.currentDBVersion = currentDBVersion;
		}
		public virtual void UpdateDatabaseBeforeUpdateSchema() {
		}
		public virtual void UpdateDatabaseAfterUpdateSchema() {
		}
		public void UpdateStatus(String context, String title, String message, params Object[] additionalParams) {
			OnStatusUpdating(context, title, message, additionalParams);
		}
		public event EventHandler<StatusUpdatingEventArgs> StatusUpdating;
	}
}
