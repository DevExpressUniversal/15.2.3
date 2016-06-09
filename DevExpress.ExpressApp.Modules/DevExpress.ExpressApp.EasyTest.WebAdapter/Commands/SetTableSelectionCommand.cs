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
	public class SetTableSelectionCommand : Command {
		public SetTableSelectionCommand() {
			HasMainParameter = true;
		}
		protected override void InternalExecute(ICommandAdapter adapter) {
			ITestControl table = adapter.CreateTestControl(TestControlType.Table, Parameters.MainParameter.Value);
			if(Parameters["Row"] == null) {
				throw new ParserException("Row parameter is necessary.", StartPosition);
			}
			foreach(Parameter parameter in Parameters) {
				if(parameter.Name != "Row") {
					throw new ParserException(string.Format("Cannot recognize the '{0}' parameter. The only supported parameter is 'Row'.", parameter.Name), parameter.PositionInScript);
				}
				bool isSelected;
				if(!bool.TryParse(parameter.Value, out isSelected)) {
					throw new ParserException(string.Format("Only 'True' or 'False' values can be used for 'Row' parameter. {0} is unknown value.", parameter.Value), parameter.PositionInScript);
				}
				if(parameter.Index < 0) {
					throw new ParserException(string.Format("Row index should be greater or equals than '0'.", parameter.Value), parameter.PositionInScript);
				}
				if(isSelected) {
					table.GetInterface<IGridRowsSelection>().SelectRow(parameter.Index);
				} else {
					table.GetInterface<IGridRowsSelection>().UnselectRow(parameter.Index);
				}
			}
		}
	}
}
