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
using DevExpress.Web.Internal;
namespace DevExpress.Web.Office.Internal {
	public class LocalFSWorkSessionHibernationProvider : WorkSessionHibernationProviderBase {
		public const string GuidFileNameMask = "????????-????-????-????-????????????";
		protected override void PersistHibernationContainer(HibernationContainer hibernationContainer, Guid workSessionId) {
			string filepath = ComposeHibernationContainerFilePath(workSessionId);
			var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			using(Stream stream = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.None)) {
				formatter.Serialize(stream, hibernationContainer.Descriptor);
				formatter.Serialize(stream, hibernationContainer.Chamber);
			}
		}
		protected override HibernationContainer LoadHibernationContainer(Guid workSessionId) {
			return LoadHibernationContainer(workSessionId, false);
		}
		protected HibernationContainer LoadHibernationContainerDescriptor(Guid workSessionId) {
			return LoadHibernationContainer(workSessionId, true);
		}
		protected HibernationContainer LoadHibernationContainer(Guid workSessionId, bool descriptorOnly) {
			string filepath = GetHibernationContainerFilePath(workSessionId);
			if(string.IsNullOrEmpty(filepath)) return null;
			var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
			using(Stream stream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				HibernationChamberDescriptor header = (HibernationChamberDescriptor)formatter.Deserialize(stream);
				HibernationChamber body = descriptorOnly ? null : (HibernationChamber)formatter.Deserialize(stream);
				return new HibernationContainer(header, body);
			}
		}
		protected override void DisposeHibernationContainer(Guid workSessionId) {
			string filepath = GetHibernationContainerFilePath(workSessionId);
			if(!string.IsNullOrEmpty(filepath))
				File.Delete(filepath);
		}
		protected override void DisposeOutdatedHibernationContainers() {
			var files = GetHibernationContainerFiles();
			var now = DateTime.Now;
			foreach(var file in files) {
				var disposeTimeoutExpired = now - File.GetLastWriteTime(file) > WorkSessionProcessing.HibernatedDocumentsDisposeTimeout;
				if(disposeTimeoutExpired)
					File.Delete(file);
			}
		}
		protected override void CreateReadyToWakeUpWorkSessions() {
			var files = GetHibernationContainerFiles();
			foreach(var file in files) {
				Guid workSessionID;
				if(Guid.TryParse(Path.GetFileName(file), out workSessionID)) {
					var hibernationChamberDescriptor = LoadHibernationContainerDescriptor(workSessionID).Descriptor;
					string workSessionTypeName = hibernationChamberDescriptor.WorkSessionTypeName;
					var workSession = WorkSessionFactories.Produce(workSessionTypeName, workSessionID); 
					WorkSessions.AddHibernatedSession(workSession, workSessionID, hibernationChamberDescriptor);
				}
			}
		}
		string ComposeHibernationContainerFilePath(Guid workSessionId) {
			var fileName = workSessionId.ToString();
			return Path.Combine(GetHibernationStorage(), fileName);
		}
		string[] GetHibernationContainerFiles() {
			return Directory.GetFiles(GetHibernationStorage(), GuidFileNameMask, SearchOption.TopDirectoryOnly);
		}
		string GetHibernationContainerFilePath(Guid workSessionId) {
			var fileName = workSessionId.ToString();
			var filePathName = Path.Combine(GetHibernationStorage(), fileName);
			return File.Exists(filePathName) ? filePathName : null;
		}
		protected virtual string GetHibernationStorage() { 
			return WorkSessionProcessing.HibernationStoragePath;
		}
		string ValidatedHibernationDirectory = string.Empty;
		protected override bool IsHibernationStorageValid(string hibernationDirectory){
			try {
				if(ValidatedHibernationDirectory != hibernationDirectory)
					HibernationDirectoryValidationHelper.ValidateHibernationDirectory(hibernationDirectory);
				ValidatedHibernationDirectory = hibernationDirectory;
				return true;
			} catch(Exception exc) {
				CommonUtils.RaiseCallbackErrorInternal(this, exc);
				return false;
			}
		}
	}
	public static class HibernationDirectoryValidationHelper {
		static object syncRoot = new Object();
		public static void ValidateHibernationDirectory(string directoryPath) {
			lock(syncRoot) {
				FileUtils.CheckOrCreateDirectory(directoryPath, FakeWebControlSingleton, "");
			}
		}
		#region FakeControlForErrorMessage
		class WorkSessionProcessingControl : System.Web.UI.WebControls.WebControl {
			const string id = "HibernationDirectory";
			public override string ID { get { return id; } set { } }
		}
		private static volatile WorkSessionProcessingControl fakeControlSingletonInstance;
		static WorkSessionProcessingControl FakeWebControlSingleton {
			get {
				if(fakeControlSingletonInstance == null) {
					lock(syncRoot) {
						if(fakeControlSingletonInstance == null) {
							fakeControlSingletonInstance = new WorkSessionProcessingControl();
						}
					}
				}
				return fakeControlSingletonInstance;
			}
		}
		#endregion
	}
}
