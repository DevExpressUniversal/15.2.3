#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Document Server                                             }
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
using DevExpress.Utils;
namespace DevExpress.Compression.Internal {
	public static class ZipNameUtils {
		public static string MakeValidName(string name) {
			string actualFilePath = name.Replace("\\", "/");
			return RemoveIligalPathItems(actualFilePath);
		}
		public static string GetZipDirItemName(string filePath, string relativePath) {
			filePath = RelativePathCalculator.GetPathClosedByDirectorySeparatorChar(filePath);
			return GetZipItemName(filePath, relativePath) + "/";
		}
		public static string GetZipItemName(string filePath, string relativePath) {
			Guard.ArgumentNotNull(filePath, "fileName");
			string fileName = Path.GetFileName(filePath);
			string actualFilePath = filePath.Replace("/", "\\");
			if (!String.IsNullOrEmpty(relativePath)) {
				string actualRelativePath = relativePath.Replace("/", "\\");
				actualFilePath = Path.Combine(actualRelativePath, fileName);
			}
			if (Path.IsPathRooted(actualFilePath)) {
				int rootLength = Path.GetPathRoot(actualFilePath).Length;
				actualFilePath = actualFilePath.Remove(0, rootLength);
			}
			return MakeValidName(actualFilePath);
		}
		static string RemoveIligalPathItems(string fullName) {
			String result = fullName.Replace(@"../", "");
			result = result.Replace(@"./", "");
			return result.Trim(new char[] { '/' });
		}
	}
	public class RelativePathCalculator {
		public static string GetPathClosedByDirectorySeparatorChar(string path) {
			if (path.Length <= 0)
				return path;
			if (path[path.Length - 1] == Path.DirectorySeparatorChar)
				return path;
			string resultPath = path + Path.DirectorySeparatorChar;
			return Uri.UnescapeDataString(resultPath);
		}
		Uri baseUri;
		string archivePath;
		string basePath;
		public RelativePathCalculator(string basePath, string archivePath) {
			this.basePath = basePath;
			string closedPath = GetPathClosedByDirectorySeparatorChar(basePath);
			this.baseUri = new Uri(Path.GetFullPath(closedPath));
			this.archivePath = archivePath;
		}
		public string CalculatePath(string fullPath) {
			string relativePath = CalculateRelativePath(fullPath);
			string resultPath = Path.Combine(basePath, relativePath);
			return resultPath;
		}
		public string CalculateArchivePath(string fullPath) {
			if (String.IsNullOrEmpty(archivePath))
				return String.Empty;
			string relativePath = CalculateRelativePath(fullPath);
			string resultPath = (String.IsNullOrEmpty(relativePath)) ? archivePath : Path.Combine(archivePath, Path.GetDirectoryName(relativePath));
			return Uri.UnescapeDataString(resultPath);
		}
		string CalculateRelativePath(string fullPath) {
			Uri itemUri = new Uri(fullPath);
			Uri relativeUri = this.baseUri.MakeRelativeUri(itemUri);
			string resultPath = relativeUri.ToString();
			return Uri.UnescapeDataString(resultPath);
		}
	}
	public static class ByteUtils {
		public static byte[] GetPasswordBytes(string password) {
			return DXEncoding.Default.GetBytes(password);
		}
		public static bool CompareBytes(byte[] buf1, byte[] buf2) {
			int count = buf1.Length;
			if (count != buf2.Length)
				return false;
			for (int i = 0; i < count; i++) {
				if (buf1[i] != buf2[i])
					return false;
			}
			return true;
		}
		public static bool CompareFirstBytes(byte[] buf1, byte[] buf2, int count) {
			if (buf1.Length < count || buf2.Length < count)
				return false;
			for (int i = 0; i < count; i++) {
				if (buf1[i] != buf2[i])
					return false;
			}
			return true;
		}
	}
	public class LongOperationState {
		public bool CanContinue { get; private set; }
		public void Start() {
			CanContinue = true;
		}
		public void End() {
			CanContinue = false;
		}
		public void Cancel() {
			CanContinue = false;
		}
	}
}
