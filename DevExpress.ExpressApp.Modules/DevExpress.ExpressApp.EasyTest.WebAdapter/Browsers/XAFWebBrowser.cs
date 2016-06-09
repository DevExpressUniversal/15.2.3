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
using DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public class XAFWebBrowser : XAFWebBrowserBase {
		private static Guid clsid_InternerExplorer = new Guid("0002DF01-0000-0000-C000-000000000046");
		private IWebBrowser2 webBrowser = null;
		public XAFWebBrowser(WebBrowserCollection owner) : base(owner) {
			Exception E = null;
			for(int i = 0; i < 10; i++) {
				try {
					Type InternerExplorerType = Type.GetTypeFromCLSID(clsid_InternerExplorer, true);
					webBrowser = (IWebBrowser2)Activator.CreateInstance(InternerExplorerType);
					((IWebBrowserEvents)this).InitWebBrowserEvents(webBrowser);
					break;
				}
				catch(Exception e) {
					E = e;
					EasyTestTracer.Tracer.LogText("Create XAFWebBrowser error: " + Environment.NewLine + E.Message + Environment.NewLine + E.StackTrace);
					if(webBrowser != null) {
						webBrowser.Quit();
					}
				}
			}
			if(webBrowser == null) {
				throw new WarningException(String.Format("It is impossible to create IE object: {0}", E.Message));
			}
		}
		public override IWebBrowser2 Browser {
			get { return webBrowser; }
		}
	}
}
