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
using System.Windows.Shell;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
namespace DevExpress.Mvvm.UI.Native {
	public static class ApplicationJumpItemWrapper {
		public static JumpItem Wrap(ApplicationJumpItemInfo applicationJumpItem) {
			ApplicationJumpTaskInfo applicationJumpTask = applicationJumpItem as ApplicationJumpTaskInfo;
			if(applicationJumpTask != null) return new ApplicationJumpTaskWrap(applicationJumpTask);
			ApplicationJumpPathInfo applicationJumpPath = applicationJumpItem as ApplicationJumpPathInfo;
			if(applicationJumpPath != null) return new ApplicationJumpPathWrap(applicationJumpPath);
			throw new ArgumentException(string.Empty, "applicationJumpItem");
		}
		public static ApplicationJumpItemInfo Unwrap(JumpItem jumpItem) {
			ApplicationJumpTaskWrap applicationJumpTaskWrap = jumpItem as ApplicationJumpTaskWrap;
			if(applicationJumpTaskWrap != null) return applicationJumpTaskWrap.ApplicationJumpTask;
			ApplicationJumpPathWrap applicationJumpPathWrap = jumpItem as ApplicationJumpPathWrap;
			if(applicationJumpPathWrap != null) return applicationJumpPathWrap.ApplicationJumpPath;
			throw new ArgumentException(string.Empty, "jumpItem");
		}
		public static void FillWrapProperties(JumpItem jumpItem) {
			ApplicationJumpTaskWrap applicationJumpTaskWrap = jumpItem as ApplicationJumpTaskWrap;
			if(applicationJumpTaskWrap != null) {
				applicationJumpTaskWrap.CustomCategory = applicationJumpTaskWrap.ApplicationJumpTask.CustomCategory;
				applicationJumpTaskWrap.Title = applicationJumpTaskWrap.ApplicationJumpTask.Title;
				applicationJumpTaskWrap.Description = applicationJumpTaskWrap.ApplicationJumpTask.Description;
				return;
			}
			ApplicationJumpPathWrap applicationJumpPathWrap = jumpItem as ApplicationJumpPathWrap;
			if(applicationJumpPathWrap != null) {
				applicationJumpPathWrap.CustomCategory = applicationJumpPathWrap.ApplicationJumpPath.CustomCategory;
				applicationJumpPathWrap.Path = applicationJumpPathWrap.ApplicationJumpPath.Path;
				return;
			}
			throw new ArgumentException(string.Empty, "jumpItem");
		}
		public static string GetJumpItemCommandId(JumpItem jumpItem) {
			ApplicationJumpTaskWrap jumpTask = jumpItem as ApplicationJumpTaskWrap;
			return jumpTask == null ? null : jumpTask.ApplicationJumpTask.CommandId;
		}
	}
	public class ApplicationJumpPathWrap : JumpPath {
		public ApplicationJumpPathWrap(ApplicationJumpPathInfo applicationJumpPath) {
			GuardHelper.ArgumentNotNull(applicationJumpPath, "applicationJumpPath");
			ApplicationJumpPath = applicationJumpPath;
		}
		public ApplicationJumpPathInfo ApplicationJumpPath { get; private set; }
		#region Equality
		public static bool operator ==(ApplicationJumpPathWrap a, ApplicationJumpPathWrap b) {
			bool aIsNull = (object)a == null;
			bool bIsNull = (object)b == null;
			if(aIsNull && bIsNull) return true;
			if(aIsNull || bIsNull) return false;
			return a.ApplicationJumpPath == b.ApplicationJumpPath;
		}
		public override int GetHashCode() {
			return ApplicationJumpPath.GetHashCode();
		}
		public static bool operator !=(ApplicationJumpPathWrap a, ApplicationJumpPathWrap b) {
			return !(a == b);
		}
		public override bool Equals(object obj) {
			return this == obj as ApplicationJumpPathWrap;
		}
		#endregion
	}
	public class ApplicationJumpTaskWrap : JumpTask, IJumpAction {
		public ApplicationJumpTaskWrap(ApplicationJumpTaskInfo applicationJumpTask) {
			GuardHelper.ArgumentNotNull(applicationJumpTask, "applicationJumpTask");
			ApplicationJumpTask = applicationJumpTask;
		}
		public ApplicationJumpTaskInfo ApplicationJumpTask { get; private set; }
		#region Equality
		public static bool operator ==(ApplicationJumpTaskWrap a, ApplicationJumpTaskWrap b) {
			bool aIsNull = (object)a == null;
			bool bIsNull = (object)b == null;
			if(aIsNull && bIsNull) return true;
			if(aIsNull || bIsNull) return false;
			return a.ApplicationJumpTask == b.ApplicationJumpTask;
		}
		public override int GetHashCode() {
			return ApplicationJumpTask.GetHashCode();
		}
		public static bool operator !=(ApplicationJumpTaskWrap a, ApplicationJumpTaskWrap b) {
			return !(a == b);
		}
		public override bool Equals(object obj) {
			return this == obj as ApplicationJumpTaskWrap;
		}
		#endregion
		#region IJumpAction
		string IJumpAction.CommandId { get { return ApplicationJumpTask.CommandId; } }
		string IJumpAction.ApplicationPath { get { return ApplicationJumpTask.ApplicationPath; } }
		string IJumpAction.Arguments { get { return ApplicationJumpTask.Arguments; } }
		string IJumpAction.WorkingDirectory { get { return ApplicationJumpTask.WorkingDirectory; } }
		void IJumpAction.SetStartInfo(string applicationPath, string arguments) {
			ApplicationPath = applicationPath;
			Arguments = arguments;
		}
		void IJumpAction.Execute() {
			IApplicationJumpTaskInfoInternal applicationJumpTask = ApplicationJumpTask;
			applicationJumpTask.Execute();
		}
		#endregion
	}
}
