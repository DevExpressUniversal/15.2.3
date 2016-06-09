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
using System.IO;
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusContinuedObject {
		byte[] content;
		readonly int totalSize;
		readonly EmfPlusObjectType objectType;
		int currentIndex;
		public EmfPlusContinuedObject(EmfPlusObjectType objectType, int totalSize, byte[] content) {
			this.content = content;
			this.totalSize = totalSize;
			this.objectType = objectType;
			currentIndex = content.Length;
			content.CopyTo(this.content, 0);
		}
		public object Append(EmfPlusContinuedObject record) {
			if (content.Length < totalSize) {
				byte[] data = new byte[totalSize];
				content.CopyTo(data, 0);
				content = data;
			}
			Array.Copy(record.content, 0, content, currentIndex, record.currentIndex);
			currentIndex += record.currentIndex;
			if (currentIndex > (totalSize - 1))
				return CreateObject();
			else
				return this;
		}
		object CreateObject() {
			using (EmfPlusReader contentStream = new EmfPlusReader(new MemoryStream(content)))
				return EmfPlusObjectRecord.CreateObject(objectType, contentStream);
		}
	}
}
