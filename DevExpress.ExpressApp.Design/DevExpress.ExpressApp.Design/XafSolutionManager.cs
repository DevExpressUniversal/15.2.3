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
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using DevExpress.ExpressApp.Design.Commands;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell.Interop;
using DevExpress.ExpressApp.Design.Core;
namespace DevExpress.ExpressApp.Design {
	public class XafSolutionManager {
		private _DTE _dte;
		private IServiceProvider _serviceProvider;
		private BuildEvents _buildEvents = null;
		private bool listening;
		public XafSolutionManager(IServiceProvider serviceProvider) {
			_dte = (_DTE)serviceProvider.GetService(typeof(SDTE));
			_serviceProvider = serviceProvider;
			Initialize();
		}
		protected virtual void Initialize() {
		}
		private void XafSolutionManager_OnBuildDone(vsBuildScope Scope, vsBuildAction Action) {
			XafTypesInfo.Reset();
		}
		public BuildEvents GetBuildEvents() {
			if(_buildEvents == null) {
				EnvDTE.DTE dte = _serviceProvider.GetService(typeof(DTE)) as DTE;
				_buildEvents = dte.Events.BuildEvents;
			}
			return _buildEvents;
		}
		public bool IsListeningToBuildEvents {
			get {
				return listening;
			}
			set {
				if(value != listening) {
					if(value) {
						GetBuildEvents().OnBuildDone += new _dispBuildEvents_OnBuildDoneEventHandler(XafSolutionManager_OnBuildDone);
					}
					else {
						GetBuildEvents().OnBuildDone -= new _dispBuildEvents_OnBuildDoneEventHandler(XafSolutionManager_OnBuildDone);
					}
					listening = value;
				}
			}
		}
	}
}
