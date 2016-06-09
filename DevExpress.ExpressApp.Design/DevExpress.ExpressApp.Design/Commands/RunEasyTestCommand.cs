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
using System.ComponentModel.Design;
using System.Threading;
using EnvDTE;
using DevExpress.ExpressApp.Design.Core;
using DevExpress.EasyTest.Framework;
namespace DevExpress.ExpressApp.Design.Commands {
	public class EasyTestCommandBase : VSCommand {
		private static EasyTestManager easyTestManager;
		private static bool isTryToInitialize = false;
		protected EasyTestManager EasyTestManager {
			get {
				if(easyTestManager == null || !isTryToInitialize) {
					isTryToInitialize = true;
					easyTestManager = new EasyTestManager(dte, serviceProvider);
				}
				return easyTestManager;
			}
		}
		protected override vsCommandStatus InternalQueryStatus(vsCommandStatus status) {
			return vsCommandStatus.vsCommandStatusUnsupported | vsCommandStatus.vsCommandStatusInvisible;
		}
		public EasyTestCommandBase(IServiceProvider serviceProvider)
			: base(serviceProvider) {
			skipXafVersionChecking = true;
		}
		public override void InternalExec() {
		}
		public override String CommandName {
			get { return ""; }
		}
		public override String CommandToolName {
			get { return ""; }
		}
		public override CommandID CommandID {
			get { return null; }
		}
	}
	public class EasyTestRunCommand : EasyTestCommandBase {
		protected override vsCommandStatus InternalQueryStatus(vsCommandStatus status) {
			EasyTestManager.TraceMethodEnter(this, "InternalQueryStatus");
			vsCommandStatus result = vsCommandStatus.vsCommandStatusUnsupported | vsCommandStatus.vsCommandStatusInvisible;
			if(EasyTestManager.IsRunAvailable) {
				result = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
			}
			EasyTestManager.TraceValue("result", result);
			EasyTestManager.TraceMethodExit(this, "InternalQueryStatus");
			return result;
		}
		public EasyTestRunCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }
		public override void InternalExec() {
			EasyTestManager.Run();
		}
		public override String CommandName {
			get { return "RunEasyTest"; }
		}
		public override String CommandToolName {
			get { return "Run EasyTest"; }
		}
		public override CommandID CommandID {
			get { return new CommandID(ConstantList.guidMenuAndCommandsCmdSet, CommandIds.EasyTestRunCommandId); }
		}
	}
	public class EasyTestRunNextStepCommand : EasyTestCommandBase {
		protected override vsCommandStatus InternalQueryStatus(vsCommandStatus status) {
			vsCommandStatus result = vsCommandStatus.vsCommandStatusUnsupported | vsCommandStatus.vsCommandStatusInvisible;
			if(EasyTestManager.IsRunNextStepAvailable) {
				result = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
			}
			return result;
		}
		public EasyTestRunNextStepCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }
		public override void InternalExec() {
			EasyTestManager.RunNextStep();
		}
		public override String CommandName {
			get { return "EasyTestRunNextStep"; }
		}
		public override String CommandToolName {
			get { return "Run Next Step"; }
		}
		public override CommandID CommandID {
			get { return new CommandID(ConstantList.guidMenuAndCommandsCmdSet, CommandIds.EasyTestRunNextStepCommandId); }
		}
	}
	public class EasyTestRunToCursorCommand : EasyTestCommandBase {
		protected override vsCommandStatus InternalQueryStatus(vsCommandStatus status) {
			vsCommandStatus result = vsCommandStatus.vsCommandStatusUnsupported | vsCommandStatus.vsCommandStatusInvisible;
			if(EasyTestManager.IsRunToCursorAvailable) {
				result = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
			}
			return result;
		}
		public EasyTestRunToCursorCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }
		public override void InternalExec() {
			EasyTestManager.RunToCursor();
		}
		public override String CommandName {
			get { return "EasyTestRunToCursor"; }
		}
		public override String CommandToolName {
			get { return "Run to Cursor"; }
		}
		public override CommandID CommandID {
			get { return new CommandID(ConstantList.guidMenuAndCommandsCmdSet, CommandIds.EasyTestRunToCursorCommandId); }
		}
	}
	public class EasyTestStopRunningCommand : EasyTestCommandBase {
		protected override vsCommandStatus InternalQueryStatus(vsCommandStatus status) {
			vsCommandStatus result = vsCommandStatus.vsCommandStatusUnsupported | vsCommandStatus.vsCommandStatusInvisible;
			if(EasyTestManager.IsStopRunningAvailable) {
				result = vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
			}
			return result;
		}
		public EasyTestStopRunningCommand(IServiceProvider serviceProvider) : base(serviceProvider) { }
		public override void InternalExec() {
			EasyTestManager.StopExecuting();
		}
		public override String CommandName {
			get { return "EasyTestStopRunning"; }
		}
		public override String CommandToolName {
			get { return "Stop Running"; }
		}
		public override CommandID CommandID {
			get { return new CommandID(ConstantList.guidMenuAndCommandsCmdSet, CommandIds.EasyTestStopRunningCommandId); }
		}
	}
}
