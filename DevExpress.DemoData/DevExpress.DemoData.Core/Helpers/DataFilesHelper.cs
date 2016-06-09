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
using System.Reflection;
using System.Threading;
using DevExpress.Internal;
#if !REALTORWORLDDEMO
using System.Windows;
using System.Deployment.Application;
using DevExpress.Internal.DXWindow;
using System.Security;
#endif
#if REALTORWORLDDEMO
namespace DevExpress.RealtorWorld.Xpf.Helpers {
#else
namespace DevExpress.DemoData.Helpers {
#endif
	public sealed class ReusableStream : IDisposable {
		Stream data;
		AutoResetEvent mutex;
		public ReusableStream() {
			this.mutex = new AutoResetEvent(true);
		}
		public Stream Data {
			get { return data; }
			set {
				data = value;
				if(data != null && !data.CanSeek)
					data = StreamHelper.CopyToMemoryStream(data);
			}
		}
		public void Reset() {
			this.mutex.WaitOne();
			if(this.data != null)
				this.data.Seek(0, SeekOrigin.Begin);
		}
		public void Dispose() {
			this.mutex.Set();
		}
	}
	public static class DataFilesHelper {
		static object dataFilesLock = new object();
		static Dictionary<string, ReusableStream> dataFiles = new Dictionary<string, ReusableStream>();
		public static string DataDirectory { get { return DataDirectoryHelper.GetDataDirectory(); } }
		public static string DataPath = DataDirectoryHelper.DataFolderName;
		public static string FindFile(string fileName, string directoryName) {
			return DataDirectoryHelper.GetFile(fileName, directoryName);
		}
		public static string FindDirectory(string directoryName) {
			return DataDirectoryHelper.GetDirectory(directoryName);
		}
		public static ReusableStream GetDataFile(string name) {
			ReusableStream stream;
			bool loadData = false;
			lock(dataFilesLock) {
				if(!dataFiles.TryGetValue(name, out stream)) {
					stream = new ReusableStream();
					loadData = true;
					dataFiles.Add(name, stream);
				}
			}
			stream.Reset();
			if(loadData) {
				stream.Data = GetDataFileCore(name);
			}
			return stream;
		}
#if !REALTORWORLDDEMO
		static IEnumerable<Tuple<byte[], string>> ReadZipFile(string archivePath) {
			if(!File.Exists(archivePath))
				throw new Exception(string.Format("Archive {0} doesn't exist", archivePath));
			var files = new List<Tuple<byte[], string>>();
			using(FileStream archiveStream = new FileStream(archivePath, FileMode.Open, FileAccess.Read)) {
				InternalZipFileCollection archiveFiles = InternalZipArchive.Open(archiveStream);
				foreach(InternalZipFile archiveFile in archiveFiles) {
					byte[] data = new byte[archiveFile.UncompressedSize];
					archiveFile.FileDataStream.Read(data, 0, data.Length);
					files.Add(Tuple.Create(data, archiveFile.FileName));
				}
			}
			return files;
		}
		[SecurityCritical]
		public static void UnpackData(params string[] archiveNames) {
			if(!EnvironmentHelper.IsClickOnce) return;
			string archivesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
			string dataDirectory = Path.Combine(ApplicationDeployment.CurrentDeployment.DataDirectory, "Data");
			Mutex mutex = new Mutex(false, "DevExpress.Demo.ZipUnpacker" + AssemblyInfo.VersionShort);
			if(!mutex.WaitOne(30000))
				throw new Exception("Can't acquire the mutex.");
			try {
				foreach(string archiveName in archiveNames) {
					string archivePath = Path.Combine(archivesDirectory, archiveName);
					foreach(var zipFile in ReadZipFile(archivePath)) {
						string dataFilePath = Path.Combine(dataDirectory, zipFile.Item2);
						string dataFileDirectory = Path.GetDirectoryName(dataFilePath);
						Directory.CreateDirectory(dataFileDirectory);
						var mdfLogs = Directory.GetFiles(dataFileDirectory, "*.ldf");
						foreach(string log in mdfLogs) {
							try { File.Delete(log); } catch { }
						}
						try { File.WriteAllBytes(dataFilePath, zipFile.Item1); } catch { }
					}
				}
			} finally {
				mutex.ReleaseMutex();
			}
		}
#endif
		static Stream GetDataFileCore(string name) {
			string filePath = FindFile(name, DataPath);
			FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			return fileStream;
		}
	}
}
