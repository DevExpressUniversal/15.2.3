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
using System.Text;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Serialization {
	public class ObjectStorageInfo : IObject {
		string type;
		string content;
		string contentRef;
		[
		XtraSerializableProperty(-1),
		]
		public string ObjectType {
			get {
				Type type = GetType();
				return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
			}
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible, 1),]
		public string Ref {
			get { return contentRef; }
			set { contentRef = value; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible, 2),]
		public string Content {
			get { return content; }
			set { content = value; }
		}
		[XtraSerializableProperty(XtraSerializationVisibility.Visible, 3),]
		public string Type {
			get { return type; }
			set { type = value; }
		}
		public ObjectStorageInfo() {
		}
		public ObjectStorageInfo(string type, string content, string contentRef) {
			this.type = type;
			this.content = content;
			this.contentRef = contentRef;
		}
	}
}
