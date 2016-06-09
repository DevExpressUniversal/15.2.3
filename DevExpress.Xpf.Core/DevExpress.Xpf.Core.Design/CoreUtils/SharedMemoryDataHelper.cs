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

#if SILVERLIGHT
extern alias Platform;
using AssemblyInfo = Platform::AssemblyInfo;
#endif
using DevExpress.Xpf.Core.Design;
using System;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Threading;
using DevExpress.Internal;
namespace DevExpress.Xpf.CreateLayoutWizard {
	public static class RemoteChannelNameHelper {
		static string prefix = null;
		public static int GetParentProcessId() {
			Process proc = Process.GetCurrentProcess();
			if(!proc.ProcessName.ToLower().Contains("devenv")) {
				Process parent = ProcessParentHelper.GetParent(proc);
				if(parent != null)
					proc = parent;
			}
			return proc.Id;
		}
		public static string GetActualChannelName(string channelName) {
			if(prefix == null)
				prefix = GetParentProcessId().ToString();
			return channelName + prefix;
		}
	}
	public class InterProcessCriticalSection : IDisposable {
		Mutex mutex;
		bool disposed = false;
		public InterProcessCriticalSection(string sectionName) {
			try {
				mutex = new Mutex(false, sectionName);
				mutex.WaitOne();
			} catch {
			}
		}
		~InterProcessCriticalSection() {
			Dispose(true);
		}
		void IDisposable.Dispose() {
			Dispose(false);
			GC.SuppressFinalize(this);
		}
		public void Dispose(bool finalizing) {
			if (!disposed) {
				if (!finalizing) {
					mutex.ReleaseMutex();
					mutex.Dispose();
				}
				disposed = true;
			}
		}
	}
	public static class SharedMemoryDataHelper {
		const int DataSize = 128;
		static bool isErrorState;
		static string MutexName { get { return RemoteChannelNameHelper.GetActualChannelName("ReadWriteDataMutex" + AssemblyInfo.VersionShort); } }
		static string SharedDataFileName { get { return RemoteChannelNameHelper.GetActualChannelName("SharedData" + AssemblyInfo.VersionShort); } }
		public struct Data {
			public bool IsWizardEnabled;
			public bool IsStandardSmartTagsEnabled;
			public bool WizardFirstRun;
			public bool SmartTagsFirstRun;
		}
		public static Data GetSharedData() {
			Data data = new Data() { IsStandardSmartTagsEnabled = true };
			if(isErrorState) return data;
			try {
				using (new InterProcessCriticalSection(MutexName)) {
					using (MemoryMappedFile file = OpenSharedDataFile()) {
						using (MemoryMappedViewAccessor viewAccessor = file.CreateViewAccessor()) {
							viewAccessor.Read<Data>(0, out data);
						}
					}
				}
			} catch(Exception) {
				isErrorState = true;
			}
			return data;
		}
		public static void UpdateSharedData(Data data) {
			if(isErrorState) return;
			try {
				using (new InterProcessCriticalSection(MutexName)) {
					using (MemoryMappedFile file = OpenSharedDataFile()) {
						using (MemoryMappedViewAccessor viewAccessor = file.CreateViewAccessor()) {
							viewAccessor.Write<Data>(0, ref data);
						}
					}
				}
			} catch(Exception) { }
		}
		public static MemoryMappedFile CreateSharedDataFile() {
			MemoryMappedFile file = null;
			using (new InterProcessCriticalSection(MutexName)) {
				file = MemoryMappedFile.CreateNew(SharedDataFileName, DataSize, MemoryMappedFileAccess.ReadWrite);
			}
			UpdateSharedData(new Data() { IsWizardEnabled = false, IsStandardSmartTagsEnabled = true });			
			return file;
		}
		static MemoryMappedFile OpenSharedDataFile() {
			return MemoryMappedFile.OpenExisting(SharedDataFileName);
		}
	}
}
