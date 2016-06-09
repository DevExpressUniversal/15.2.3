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

using DevExpress.DemoData.Model;
using DevExpress.Utils.About;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Threading;
namespace DevExpress.Xpf.DemoBase {
	class DelayedSearchLogger {
		string demo;
		string lastLoggedFilter;
		string currentFilter;
		DispatcherTimer timer;
		public DelayedSearchLogger(string demo, string platform) {
			if(platform == Repository.WinPlatform.Name) {
			} else if(platform == Repository.WpfPlatform.Name) {
			} else {
				Debug.Fail("unknown platform");
				return;
			}
			this.demo = demo;
			this.timer = new DispatcherTimer {
				Interval = TimeSpan.FromSeconds(2)
			};
			timer.Tick += (s, e) => {
				timer.Stop();
				if(!string.IsNullOrWhiteSpace(currentFilter) && lastLoggedFilter != currentFilter) {
					lastLoggedFilter = currentFilter;
					Debug.WriteLine("filter changed: {0}, {1}", demo, currentFilter);
				}
			};
		}
		public void Update(string filter) {
			if(timer == null)
				return;
			currentFilter = filter;
			timer.Stop();
			timer.Start();
		}
	}
}
