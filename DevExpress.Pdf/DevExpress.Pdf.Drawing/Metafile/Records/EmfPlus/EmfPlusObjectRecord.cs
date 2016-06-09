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

using System.Drawing;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class EmfPlusObjectRecord : EmfPlusObjectRecordBase {
		public static object CreateObject(EmfPlusObjectType type, EmfPlusReader contentStream) {
			switch (type) {
				case EmfPlusObjectType.ObjectTypeBrush:
					return EmfPlusBrush.Create(contentStream);
				case EmfPlusObjectType.ObjectTypePen:
					return new EmfPlusPen(contentStream);
				case EmfPlusObjectType.ObjectTypePath:
					return new EmfPlusPath(contentStream);
				case EmfPlusObjectType.ObjectTypeRegion:
					return new EmfPlusRegion(contentStream);
				case EmfPlusObjectType.ObjectTypeImage:
					return new EmfPlusImage(contentStream);
				case EmfPlusObjectType.ObjectTypeFont:
					return new EmfPlusFont(contentStream);
				case EmfPlusObjectType.ObjectTypeStringFormat:
					return new EmfPlusStringFormat(contentStream);
				default:
					return null;
			}
		}
		readonly object value;
		public object Value { get { return value; } }
		public EmfPlusObjectRecord(short flags, byte[] content)
			: base(flags, content) {
			value = CreateObject(ObjectType, ContentStream);
		}
		public override void Execute(EmfMetafileGraphics context) {
			context.AddEmfPlusObject(Flags & emfPlusObjectIdMask, value);
		}
	}
}
