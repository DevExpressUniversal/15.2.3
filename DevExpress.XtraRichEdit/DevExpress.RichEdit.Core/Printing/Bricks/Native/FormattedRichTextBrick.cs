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

using DevExpress.XtraPrinting.Native;
using System;
using System.Drawing;
using System.IO;
using DevExpress.XtraPrinting.Native.RichText;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.XtraPrinting.Native {
	public class FormattedRichTextBrick : RichTextBrickBase, IXtraSortableProperties {
		SizeF formatSize;
		[XtraSerializableProperty(0)]
		public string RtfText {
			get { return XtraRichTextEditHelper.GetRtfFromDocManager(DocumentModel); }
			set { SetInternalRtf(ValidateRtf(value, DocumentModel)); }
		}
		[XtraSerializableProperty(1)]
		public SizeF FormatSize { 
			get { return formatSize; }
			set {
				formatSize = value;
				Printer.Format(formatSize);
			}
		}
		public override string BrickType { get { return BrickTypes.FormattedRichText; } }
		public FormattedRichTextBrick()
			: base(NullBrickOwner.Instance) {
		}
		public void Init(Stream stream, SizeF formatSize) {
			RecreateDocManagerAndPrinter();
			XtraRichTextEditHelper.ImportRtfTextStreamToDocManager(stream, DocumentModel);
			FormatSize = formatSize;
			Rect = new RectangleF(PointF.Empty, new SizeF(formatSize.Width, EffectiveHeight));
		}
		bool IXtraSortableProperties.ShouldSortProperties() {
			return true;
		}
	}
}
