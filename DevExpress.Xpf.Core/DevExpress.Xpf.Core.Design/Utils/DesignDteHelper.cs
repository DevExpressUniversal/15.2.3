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
using System.Linq;
using System.ServiceModel;
using System.Text;
namespace DevExpress.Xpf.Core.Design.Utils {
	public static class DesignDteHelper {
		static T ProcessInteracton<T>(Func<IDevenvXDesProcInteraction, T> func) {
			T result = default(T);
			try {
				string processId = DevExpress.Xpf.CreateLayoutWizard.RemoteChannelNameHelper.GetParentProcessId().ToString();
				var endPoint = new EndpointAddress(string.Format("net.pipe://localhost/{0}{1}/DevenvXDesProcInteraction", processId, AssemblyInfo.VersionShort));
				using (var factory = new ChannelFactory<IDevenvXDesProcInteraction>(new NetNamedPipeBinding(), endPoint)) {
					IDevenvXDesProcInteraction channel = factory.CreateChannel();
					if (channel != null)
						result = func(channel);
					factory.Close();
				}
			} catch (Exception e) {
#if DEBUG
				System.Diagnostics.Debug.WriteLine(e.Message);
#endif
			}
			return result;
		}
		public static string GetFilePathForActiveProject(string fileName) {
			return ProcessInteracton<string>(delegate(IDevenvXDesProcInteraction channel) {
				return channel.GetFilePathForActiveProject(fileName);
			});
		}
		public static string FormatMarkup(string markup) {
			return ProcessInteracton<string>(delegate(IDevenvXDesProcInteraction channel) {
				return channel.FormatActiveFile(markup);
			});
		}
		public static bool AddAssemblyToReferences(string assemblyName) {
			return ProcessInteracton<bool>(delegate(IDevenvXDesProcInteraction channel) {
				return channel.AddAssemblyToReferences(assemblyName);
			});
		}
		public static bool AddFileToActiveProject(string filePath) {
			return ProcessInteracton<bool>(delegate(IDevenvXDesProcInteraction channel) {
				return channel.AddFileToActiveProject(filePath);
			});
		}
		public static string GetExistingFilePathForActiveProject(string fileName) {
			return ProcessInteracton<string>(delegate(IDevenvXDesProcInteraction channel) {
				return channel.GetExistingFilePathForActiveProject(fileName);
			});
		}
		public static string GetCombinedFilePathForActiveProject(string fileName) {
			return ProcessInteracton<string>(delegate(IDevenvXDesProcInteraction channel) {
				return channel.GetCombinedFilePathForActiveProject(fileName);
			});
		}
		public static bool CheckoutFile(string filePath) {
			return ProcessInteracton<bool>(delegate(IDevenvXDesProcInteraction channel) {
				return channel.CheckoutFile(filePath);
			});
		}
	}
}
