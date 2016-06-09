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
using System.Collections.Generic;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using DevExpress.Xpo.Metadata;
using DevExpress.Xpo.Metadata.Helpers;
namespace DevExpress.ExpressApp.Xpo.Utils {
	public class XpoCustomizeTableNameEventArgs : CustomizeTableNameEventArgs {
		private XPClassInfo classInfo;
		public XpoCustomizeTableNameEventArgs(XPClassInfo classInfo)
			: base(classInfo.TableName) {
			this.classInfo = classInfo;
		}
		public XPClassInfo ClassInfo {
			get { return classInfo; }
		}
	}
	public class TableNameCustomizer {
		private String prefixes;
		private Dictionary<String, String> tableNamespacePrefixes;
		private String GetTablePrefix(Type classType) {
			String tablePrefix = null;
			int prefixLength = -1;
			foreach(KeyValuePair<String, String> pair in tableNamespacePrefixes) {
				if(classType.FullName.StartsWith(pair.Key) && pair.Key.Length > prefixLength) {
					tablePrefix = pair.Value;
					prefixLength = pair.Key.Length;
				}
			}
			return tablePrefix;
		}
		private void SetTablePrefixesForManyToMany(XpoCustomizeTableNameEventArgs args) {
			IntermediateClassInfo classInfo = args.ClassInfo as IntermediateClassInfo;
			if(classInfo != null) {
				String leftPrefix = null;
				String rightPrefix = null;
				IntermediateObjectFieldInfo[] fields = new IntermediateObjectFieldInfo[2];
				int i = 0;
				foreach(IntermediateObjectFieldInfo obj in classInfo.ObjectProperties) {
					fields[i] = obj;
					i++;
				}
				OrderFields(ref fields, args.TableName);
				leftPrefix = GetTablePrefix(fields[0].ReferenceType.ClassType);
				rightPrefix = GetTablePrefix(fields[1].ReferenceType.ClassType);
				if(!String.IsNullOrEmpty(leftPrefix) && !String.IsNullOrEmpty(rightPrefix)) {
					args.TableName = leftPrefix + fields[0].ReferenceType.ClassType.Name + fields[0].Name +
						"_" + rightPrefix + fields[1].ReferenceType.ClassType.Name + fields[1].Name;
					args.Handled = true;
				}
			}
		}
		private void OrderFields(ref IntermediateObjectFieldInfo[] fields, String tableName) {
			if(tableName != fields[0].ReferenceType.ClassType.Name + fields[0].Name + "_" + fields[1].ReferenceType.ClassType.Name + fields[1].Name) {
				IntermediateObjectFieldInfo temp = fields[0];
				fields[0] = fields[1];
				fields[1] = temp;
			}
		}
		private void SetTablePrefixByClassType(CustomizeTableNameEventArgs args, Type classType) {
			if(!typeof(IModuleInfo).IsAssignableFrom(classType)) {
				String tablePrefix = null;
				tablePrefix = GetTablePrefix(classType);
				if(!String.IsNullOrEmpty(tablePrefix)) {
					args.TableName = tablePrefix + args.TableName;
					args.Handled = true;
				}
			}
			else {
				args.Handled = false;
			}
		}
		private void CollectTablePrefixes() {
			tableNamespacePrefixes = new Dictionary<String, String>();
			if(!String.IsNullOrEmpty(prefixes)) {
				prefixes = prefixes.Trim();
				String[] items = prefixes.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
				foreach(String item in items) {
					String trimmed = item.Trim();
					int p = trimmed.IndexOf('=');
					if(p == -1)
						throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.IsNotValidNamespacePrefixDescription, trimmed));
					tableNamespacePrefixes.Add(trimmed.Substring(0, p) + '.', trimmed.Substring(p + 1));
				}
			}
		}
		protected virtual void OnCustomizeTableName(XpoCustomizeTableNameEventArgs args) {
			if(tableNamespacePrefixes == null) {
				CollectTablePrefixes();
			}
			XPClassInfo classInfo = args.ClassInfo;
			if(classInfo.CanGetByClassType) {
				SetTablePrefixByClassType(args, classInfo.ClassType);
			}
			else {
				SetTablePrefixesForManyToMany(args);
			}
			if(CustomizeTableName != null) {
				CustomizeTableName(this, args);
			}
		}
		public TableNameCustomizer()
			: this(null) {
		}
		public TableNameCustomizer(String prefixes) {
			this.prefixes = prefixes;
		}
		public void Customize(XPDictionary dictionary) {
			foreach(XPClassInfo classInfo in dictionary.Classes) {
				if(classInfo.IsPersistent && (classInfo.TableMapType == MapInheritanceType.OwnTable)) {
					XpoCustomizeTableNameEventArgs args = new XpoCustomizeTableNameEventArgs(classInfo);
					OnCustomizeTableName(args);
					if(args.Handled && (args.TableName != classInfo.TableName) && classInfo.FindAttributeInfo(typeof(PersistentAttribute)) == null) {
						Tracing.Tracer.LogText("the '{0}' class is mapped to the '{1}' table", classInfo.ClassType, args.TableName);
						classInfo.AddAttribute(new PersistentAttribute(args.TableName));
					}
				}
			}
		}
		public event EventHandler<CustomizeTableNameEventArgs> CustomizeTableName;
	}
}
