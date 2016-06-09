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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace DevExpress.XtraReports.Native {
	public static class IApplicationPathServiceExtensions {
		public static string GetFullPath(this IApplicationPathService serv, string fileName) {
			string[] dirs = serv.GetBinDirectories();
			foreach(string dir in dirs) {
				string path = Path.Combine(dir.Trim(), fileName);
				if(File.Exists(path))
					return path;
			}
			return string.Empty;
		}
	}
	public interface IApplicationPathService {
		string[] GetBinDirectories();
	}
	public class DefaultApplicationPathService : IApplicationPathService {
		#region IApplicationPathService Members
		public string[] GetBinDirectories() {
			AppDomainSetup setupInfo = AppDomain.CurrentDomain.SetupInformation;
			List<string> result = new List<string>();
			if(setupInfo.PrivateBinPathProbe == null && !string.IsNullOrEmpty(setupInfo.ApplicationBase))
				result.Add(setupInfo.ApplicationBase);
			if(!string.IsNullOrEmpty(setupInfo.PrivateBinPath)) {
				result.AddRange(setupInfo.PrivateBinPath.Split(';').ToList());
				result.RemoveAll(str => string.IsNullOrEmpty(str));
			}
			return result.ToArray();
		}
		#endregion
	}
}
