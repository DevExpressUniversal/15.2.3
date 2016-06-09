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
using System.Linq;
namespace DevExpress.ExpressApp.Design {
	public class XpandModuleLogger {
		private static Dictionary<string, int> xpandModulesNames = new Dictionary<string, int>();
		static XpandModuleLogger() {
			FillModulesNames();
		}
		private static void FillModulesNames() {
			xpandModulesNames.Add("AdditionalViewControlsProviderAspNetModule", 1);
			xpandModulesNames.Add("AdditionalViewControlsProviderWindowsFormsModule", 1);
			xpandModulesNames.Add("XpandSecurityModule", 2);
			xpandModulesNames.Add("XpandChartWinModule", 4);
			xpandModulesNames.Add("DashboardWindowsFormsModule", 8);
			xpandModulesNames.Add("XtraDashboardWebModule", 8);
			xpandModulesNames.Add("FilterDataStoreModule", 0x10);
			xpandModulesNames.Add("XpandHtmlPropertyEditorAspNetModule", 0x20);
			xpandModulesNames.Add("ImportWizardWindowsFormsModule", 0x40);
			xpandModulesNames.Add("IOModule", 0x80);
			xpandModulesNames.Add("IOAspNetModule", 0x80);
			xpandModulesNames.Add("IOWinModule", 0x80);
			xpandModulesNames.Add("JobSchedulerModule", 0x100);
			xpandModulesNames.Add("JobSchedulerJobsModule", 0x100);
			xpandModulesNames.Add("XpandValidationModule", 0x200);
			xpandModulesNames.Add("XpandValidationWebModule", 0x200);
			xpandModulesNames.Add("XpandValidationWinModule", 0x200);
			xpandModulesNames.Add("MasterDetailModule", 0x400);
			xpandModulesNames.Add("MasterDetailWindowsModule", 0x400);
			xpandModulesNames.Add("NCarouselWebModule", 0x800);
			xpandModulesNames.Add("XpandPivotChartModule", 0x1000);
			xpandModulesNames.Add("XpandReportsModule", 0x2000);
			xpandModulesNames.Add("XpandSchedulerWindowsFormsModule", 0x4000);
			xpandModulesNames.Add("XpandSchedulerAspNetModule", 0x4000);
			xpandModulesNames.Add("XpandStateMachineModule", 0x8000);
			xpandModulesNames.Add("ThumbnailWebModule", 0x10000);
			xpandModulesNames.Add("XpandTreeListEditorsModule", 0x20000);
			xpandModulesNames.Add("XpandTreeListEditorsAspNetModule", 0x20000);
			xpandModulesNames.Add("WizardUIWindowsFormsModule", 0x40000);
			xpandModulesNames.Add("XpandWorkFlowModule", 0x80000);
			xpandModulesNames.Add("WorldCreatorModule", 0x100000);
			xpandModulesNames.Add("WorldCreatorDBMapperModule", 0x100000);
			xpandModulesNames.Add("MapViewWebModule", 0x200000);
			xpandModulesNames.Add("EmailModule", 0x400000);
			xpandModulesNames.Add("XpandFileAttachmentsModule", 0x800000);
		}
		public static string CreateXpandLogRecord(XafApplication Application) {
			int modulesCompact = 0;
			int moduleKey;
			string version = null;
			foreach(var module in Application.Modules) {
				if(xpandModulesNames.TryGetValue(module.Name, out moduleKey)) {
					modulesCompact |= moduleKey;
					if(version == null) {
						version = module.Version.ToString();
					}
				}
			}
			if(modulesCompact > 0) {
				string logRecord = string.Format("Opened Xpand v.{0} Solution({1:X}) : 0x{2:X}", version, Application.ApplicationName.GetHashCode(), modulesCompact);
				return logRecord;
			}
			return null;
		}
		public static IEnumerable<string> GetExpandModulesNames(int modulesCompact) {
			List<string> modules = new List<string>();
			foreach(var pair in xpandModulesNames) {
				if((modulesCompact & pair.Value) == pair.Value) {
					modules.Add(pair.Key);
				}
			}
			return modules;
		}
	}
}
