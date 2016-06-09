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
using System.ComponentModel;
using System.IO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using DevExpress.ExpressApp;
namespace DevExpress.Persistent.BaseImpl.EF {
	[DefaultProperty("FileName")]
	public class FileData : IFileData, IEmptyCheckable, IObjectSpaceLink {
		private Byte[] content;
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public Int32 Size { get; set; }
		public String FileName { get; set; }
		public Byte[] Content {
			get { return content; }
			set {
				if(content != value) {
					content = value;
					if(content != null) {
						Size = content.Length;
					}
					else {
						Size = 0;
					}
				}
			}
		}
		[NotMapped, Browsable(false)]
		public Boolean IsEmpty {
			get { return String.IsNullOrEmpty(FileName); }
		}
		public void LoadFromStream(String fileName, Stream stream) {
			FileName = fileName;
			Byte[] bytes = new Byte[stream.Length];
			stream.Read(bytes, 0, bytes.Length);
			Content = bytes;
			ObjectSpace.SetModified(this);
		}
		public void SaveToStream(Stream stream) {
			if(String.IsNullOrEmpty(FileName)) {
				throw new InvalidOperationException();
			}
			stream.Write(Content, 0, Size);
			stream.Flush();
		}
		public void Clear() {
			Content = null;
			FileName = "";
			ObjectSpace.SetModified(this);
		}
		public override String ToString() {
			return FileName;
		}
		[Browsable(false)]
		public IObjectSpace ObjectSpace { get; set; }
	}
}
