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
using System.Diagnostics;
using System.Runtime.InteropServices;
using DevExpress.EasyTest.Framework;
using System.Windows.Forms;
using System.Threading;
using mshtml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using DevExpress.EasyTest.Framework.Utils;
using System.Drawing.Imaging;
using System.ComponentModel;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.Commands {
	public class ExecuteScriptCommand : Command {
		protected override void InternalExecute(ICommandAdapter adapter) {
			if(adapter is IScriptExecutor) {
				string scriptText = string.Empty;
				foreach(Parameter param in Parameters) {
					if(param.Name.Length > 0) {
						if(string.IsNullOrEmpty(scriptText)) {
							scriptText = param.Value;
						} else {
							scriptText += Environment.NewLine + param.Value;
						}
					} else {
						throw new ParserException("The parameter name of ExecuteScript command is wrong", StartPosition);
					}
				}
				string error = ((IScriptExecutor)adapter).ExecuteScript(scriptText) as string;
				if (error != null) {
					throw new AdapterOperationException(error);
				}
			}
			else {
				throw new ParserException("ExecuteScript command is not supported", StartPosition);
			}
		}
	}
}
