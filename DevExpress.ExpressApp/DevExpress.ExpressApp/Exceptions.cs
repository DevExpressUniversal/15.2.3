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
using System.Runtime.Serialization;
using DevExpress.ExpressApp.Localization;
namespace DevExpress.ExpressApp {
	public interface IUserFriendlyException {
	}
	[Serializable]
	public class UserFriendlyException : Exception, IUserFriendlyException {
		protected UserFriendlyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public UserFriendlyException(String message) : base(message) { }
		public UserFriendlyException(UserVisibleExceptionId exceptionId)
			: base(UserVisibleExceptionLocalizer.GetExceptionMessage(exceptionId)) { }
		public UserFriendlyException(Exception innerException) : base(innerException.Message, innerException) { }
		public UserFriendlyException(UserVisibleExceptionId exceptionId, Exception innerException)
			: base(UserVisibleExceptionLocalizer.GetExceptionMessage(exceptionId), innerException) { }
	}
	[Serializable]
	public class CreateListViewException : Exception {
		protected CreateListViewException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public CreateListViewException(string message, Exception innerException) : base(message, innerException) { }
	}
	[Serializable]
	public class CannotRemoveModule : InvalidOperationException {
		private IList<ModuleBase> dependentModules;
		private ModuleBase removingModule;
		private static string FormatMessage(ModuleBase removingModule, IList<ModuleBase> dependentModules) {
			string dependentModuleNames = "";
			foreach(ModuleBase module in dependentModules) {
				dependentModuleNames += "  " + module.Name + "\n";
			}
			return string.Format(
					"Cannot remove the \"{0}\" module, there are dependent modules: {1}",
					removingModule.Name, dependentModuleNames);
		}
		private static string FormatMessage(ModuleBase removingModule, string message) {
			return string.Format(
					"Cannot remove the \"{0}\" module: {1}",
					removingModule.Name, message);
		}
		protected CannotRemoveModule(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public CannotRemoveModule(ModuleBase removingModule, IList<ModuleBase> dependentModules)
			: base(FormatMessage(removingModule, dependentModules)) {
			this.dependentModules = dependentModules;
			this.removingModule = removingModule;
		}
		public CannotRemoveModule(ModuleBase removingModule, string message)
			: base(FormatMessage(removingModule, message)) {
			this.dependentModules = new List<ModuleBase>();
			this.removingModule = removingModule;
		}
		public IList<ModuleBase> DependentModules {
			get { return dependentModules; }
		}
		public ModuleBase RemovingModule {
			get { return removingModule; }
		}
	}
}
