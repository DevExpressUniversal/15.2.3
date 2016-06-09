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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DevExpress.Xpo.DB.Exceptions;
namespace DevExpress.ExpressApp.Updating {
	[Serializable]
	public class UpdatingException : Exception {
		protected UpdatingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public UpdatingException() : base() { }
		public UpdatingException(string message) : base(message) { }
	}
	public interface IModuleInfo {
		String Name { get; set; }
		String AssemblyFileName { get; set; }
		String Version { get; set; }
		Boolean IsMain { get; set; }
	}
	public abstract class Updater {
		private IObjectSpace objectSpace;
		private IList<ModuleBase> modules;
		private String applicationName;
		private Type moduleInfoType;
		private IList versionInfoList;
		protected String ApplicationName {
			get { return applicationName; }
		}
		protected IList<ModuleBase> Modules {
			get { return modules; }
		}
		protected IModuleInfo GetModuleInfoFromDB(String name) {
			IModuleInfo result = null;
			try {
				foreach(IModuleInfo moduleInfo in versionInfoList) {
					if(moduleInfo.Name == name) {
						result = moduleInfo;
						break;
					}
				}
			}
			catch(UnableToOpenDatabaseException) {
			}
			catch(SchemaCorrectionNeededException) {
			}
			return result;
		}
		protected String GetApplicationNameFromDB() {
			String result = "";
			try {
				foreach(IModuleInfo moduleInfo in versionInfoList) {
					if(moduleInfo.IsMain == true) {
						result = moduleInfo.Name;
						break;
					}
				}
			}
			catch(UnableToOpenDatabaseException) {
			}
			catch(SchemaCorrectionNeededException) {
			}
			return result;
		}
		protected Boolean IsDatabaseExist() {
			try {
				versionInfoList = ObjectSpace.GetObjects(moduleInfoType);
			}
			catch(UnableToOpenDatabaseException) {
				return false;
			}
			catch(SchemaCorrectionNeededException) {
				return false;
			}
			return true;
		}
		protected void ReloadVersionInfoList() {
			versionInfoList = ObjectSpace.GetObjects(moduleInfoType);
		}
		protected Updater(IObjectSpace objectSpace, IList<ModuleBase> modules, String applicationName, Type moduleInfoType) {
			this.objectSpace = objectSpace;
			this.modules = modules;
			this.applicationName = applicationName;
			this.moduleInfoType = moduleInfoType;
		}
		public IObjectSpace ObjectSpace {
			get { return objectSpace; }
		}
	}
}
