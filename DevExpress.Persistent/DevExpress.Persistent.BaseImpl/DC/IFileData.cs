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
using System.ComponentModel;
using System.IO;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
namespace DomainComponents.Common {
	[DomainComponent]
	[XafDefaultProperty("FileName")]
	public interface IPersistentFileData : IFileData, IEmptyCheckable {
		[System.ComponentModel.DefaultValue("")]
		new string FileName { get; set; }
		[System.ComponentModel.DefaultValue(0)]
		new int Size { get; set; }
		[ValueConverter(typeof(CompressionConverter))]
		[MemberDesignTimeVisibility(false)]
		[Delayed]
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		byte[] Content { get; set; }
		#region IEmptyCheckable Members
		[NonPersistentDc, MemberDesignTimeVisibility(false)]
		new bool IsEmpty { get; }
		#endregion
	}
	[DomainLogic(typeof(IPersistentFileData))]
	public class PersistentFileDataLogic {
		public static void LoadFromStream(IPersistentFileData fileData, string fileName, Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			Guard.ArgumentNotNullOrEmpty(fileName, "fileName");
			fileData.FileName = fileName;
			byte[] bytes = new byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			fileData.Content = bytes;
		}
		public static void SaveToStream(IPersistentFileData fileData, Stream stream) {
			if(string.IsNullOrEmpty(fileData.FileName)) {
				throw new InvalidOperationException();
			}
			stream.Write(fileData.Content, 0, fileData.Size);
			stream.Flush();
		}
		public static void Clear(IPersistentFileData fileData) {
			fileData.Content = null;
			fileData.FileName = String.Empty;
		}
		public static void AfterChange_Content(IPersistentFileData fileData) {
			if(fileData != null && fileData.Content != null) {
				fileData.Size = fileData.Content.Length;
			}
			else {
				fileData.Size = 0;
			}
		}
		public static bool Get_IsEmpty(IPersistentFileData fileData) {
			return string.IsNullOrEmpty(fileData.FileName);
		}
	}
}
