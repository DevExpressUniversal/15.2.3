﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using DevExpress.Mvvm.Native;
using System.Windows;
using System.ComponentModel;
namespace DevExpress.Mvvm {
	public interface IApplicationJumpItem {
		string CustomCategory { get; set; }
	}
	public interface IApplicationJumpPath : IApplicationJumpItem {
		string Path { get; set; }
	}
	public interface IApplicationJumpTask : IApplicationJumpItem {
		string Title { get; set; }
		ImageSource Icon { get; set; }
		string IconResourcePath { get; set; }
		int IconResourceIndex { get; set; }
		string Description { get; set; }
		string ApplicationPath { get; set; }
		string Arguments { get; set; }
		string WorkingDirectory { get; set; }
		string CommandId { get; set; }
		Action Action { get; set; }
	}
	public abstract class ApplicationJumpItemInfo : ISupportInitialize, ICloneable, IApplicationJumpItemInfoInternal, IApplicationJumpItem {
		string customCategory;
		public ApplicationJumpItemInfo Clone() {
			ApplicationJumpItemInfo item = CreateInstanceCore();
			CloneCore(item);
			return item;
		}
		public string CustomCategory {
			get { return customCategory; }
			set {
				AssertIsNotInitialized();
				customCategory = value;
			}
		}
		protected IApplicationJumpItemInfoSource Source { get; private set; }
		protected bool IsInitialized { get; private set; }
		protected void AssertIsNotInitialized() {
			if(IsInitialized)
				throw new InvalidOperationException();
		}
		protected abstract ApplicationJumpItemInfo CreateInstanceCore();
		protected virtual void CloneCore(ApplicationJumpItemInfo clone) {
			clone.CustomCategory = CustomCategory;
		}
		void ISupportInitialize.BeginInit() { }
		void ISupportInitialize.EndInit() { IsInitialized = true; }
		object ICloneable.Clone() { return Clone(); }
		IApplicationJumpItemInfoSource IApplicationJumpItemInfoInternal.Source {
			get { return Source; }
			set { Source = value; }
		}
	}
	public class ApplicationJumpPathInfo : ApplicationJumpItemInfo, IApplicationJumpPath {
		string path;
		public string Path {
			get { return path; }
			set {
				AssertIsNotInitialized();
				path = value;
			}
		}
		public new ApplicationJumpPathInfo Clone() { return (ApplicationJumpPathInfo)base.Clone(); }
		protected override ApplicationJumpItemInfo CreateInstanceCore() { return new ApplicationJumpPathInfo(); }
		protected override void CloneCore(ApplicationJumpItemInfo clone) {
			base.CloneCore(clone);
			ApplicationJumpPathInfo path = (ApplicationJumpPathInfo)clone;
			path.Path = Path;
		}
	}
	public class ApplicationJumpTaskInfo : ApplicationJumpItemInfo, IApplicationJumpTaskInfoInternal, IApplicationJumpTask {
		const string AutoGeneratedCommandSuffix = "_B132D537-0377-48EB-A905-D03782D8F0B0";
		string commandId;
		string workingDirectory;
		string arguments;
		string applicationPath;
		string description;
		int iconResourceIndex;
		string iconResourcePath;
		ImageSource icon;
		string title;
		public string Title {
			get { return title; }
			set {
				AssertIsNotInitialized();
				title = value;
			}
		}
		public ImageSource Icon {
			get { return icon; }
			set {
				AssertIsNotInitialized();
				icon = value;
			}
		}
		public string IconResourcePath {
			get { return iconResourcePath; }
			set {
				AssertIsNotInitialized();
				iconResourcePath = value;
			}
		}
		public int IconResourceIndex {
			get { return iconResourceIndex; }
			set {
				AssertIsNotInitialized();
				iconResourceIndex = value;
			}
		}
		public string Description {
			get { return description; }
			set {
				AssertIsNotInitialized();
				description = value;
			}
		}
		public string ApplicationPath {
			get { return applicationPath; }
			set {
				AssertIsNotInitialized();
				applicationPath = value;
			}
		}
		public string Arguments {
			get { return arguments; }
			set {
				AssertIsNotInitialized();
				arguments = value;
			}
		}
		public string WorkingDirectory {
			get { return workingDirectory; }
			set {
				AssertIsNotInitialized();
				workingDirectory = value;
			}
		}
		public string CommandId {
			get { return commandId; }
			set {
				AssertIsNotInitialized();
				commandId = value;
			}
		}
		public Action Action { get; set; }
		public new ApplicationJumpTaskInfo Clone() { return (ApplicationJumpTaskInfo)base.Clone(); }
		protected new IApplicationJumpTaskInfoSource Source { get { return (IApplicationJumpTaskInfoSource)base.Source; } }
		protected override ApplicationJumpItemInfo CreateInstanceCore() { return new ApplicationJumpTaskInfo(); }
		protected override void CloneCore(ApplicationJumpItemInfo clone) {
			base.CloneCore(clone);
			ApplicationJumpTaskInfo task = (ApplicationJumpTaskInfo)clone;
			task.Action = Action;
			task.ApplicationPath = ApplicationPath;
			task.Arguments = Arguments;
			task.Description = Description;
			task.Icon = Icon;
			task.IconResourceIndex = IconResourceIndex;
			task.IconResourcePath = IconResourcePath;
			task.Title = Title;
			task.WorkingDirectory = WorkingDirectory;
			if(CommandId != null && !CommandId.EndsWith(AutoGeneratedCommandSuffix, StringComparison.Ordinal))
				task.CommandId = CommandId;
		}
		void IApplicationJumpTaskInfoInternal.Execute() {
			if(Action != null)
				Action();
			if(Source != null && Source.Action != null)
				Source.Action();
		}
		bool IApplicationJumpTaskInfoInternal.IsInitialized { get { return IsInitialized; } }
		void IApplicationJumpTaskInfoInternal.SetAutoGeneratedCommandId(string commandId) {
			CommandId = commandId + AutoGeneratedCommandSuffix;
		}
	}
}
