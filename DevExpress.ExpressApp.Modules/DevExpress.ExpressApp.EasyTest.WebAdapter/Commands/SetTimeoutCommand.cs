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
using System.Text;
using DevExpress.EasyTest.Framework;
using DevExpress.EasyTest.Framework.Commands;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.Commands {
	public class SetMaxWaitCallbackTimeCommand : Command {
		public const string TimeoutParameterName = "Timeout";
		protected override void InternalExecute(ICommandAdapter adapter) {
			if(Parameters[TimeoutParameterName] == null) {
				throw new ParserException("'" + TimeoutParameterName + "' parameter is necessary.", StartPosition);
			}
			if(Parameters.Count > 1) {
				throw new ParserException("The only supported parameter is '" + TimeoutParameterName + "'.");
			}
			((WebCommandAdapter)adapter).WaitCallbackTime = int.Parse(Parameters[0].Value);
		}
	}
	public class SetMaxWaitTimeoutTimeCommand : Command {
		public const string TimeoutParameterName = "Timeout";
		protected override void InternalExecute(ICommandAdapter adapter) {
			if(Parameters[TimeoutParameterName] == null) {
				throw new ParserException("'" + TimeoutParameterName + "' parameter is necessary.", StartPosition);
			}
			if(Parameters.Count > 1) {
				throw new ParserException("The only supported parameter is '" + TimeoutParameterName + "'.");
			}
			((WebCommandAdapter)adapter).WaitTimeoutTime = int.Parse(Parameters[0].Value);
		}
	}
	public class SetThrowExceptionOnWaitCallbackTimeCommand : Command {
		public const string ParameterName = "CanThrow";
		protected override void InternalExecute(ICommandAdapter adapter) {
			if(Parameters[ParameterName] == null) {
				throw new ParserException("'" + ParameterName + "' parameter is necessary.", StartPosition);
			}
			if(Parameters.Count > 1) {
				throw new ParserException("The only supported parameter is '" + ParameterName + "'.");
			}
			((WebCommandAdapter)adapter).CanThrowExceptionOnWaitCallback = bool.Parse(Parameters[0].Value);
		}
	}
	public class SetThrowExceptionOnWaitTimeoutTimeCommand : Command {
		public const string ParameterName = "CanThrow";
		protected override void InternalExecute(ICommandAdapter adapter) {
			if(Parameters[ParameterName] == null) {
				throw new ParserException("'" + ParameterName + "' parameter is necessary.", StartPosition);
			}
			if(Parameters.Count > 1) {
				throw new ParserException("The only supported parameter is '" + ParameterName + "'.");
			}
			((WebCommandAdapter)adapter).CanThrowExceptionOnWaitTimeout = bool.Parse(Parameters[0].Value);
		}
	}
}
