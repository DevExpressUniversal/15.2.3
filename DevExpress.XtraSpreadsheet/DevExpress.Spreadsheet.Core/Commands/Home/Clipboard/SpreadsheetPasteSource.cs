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

#if DXPORTABLE
using PlatformIndependentIDataObject = DevExpress.Compatibility.System.Windows.Forms.IDataObject;
#elif !SL
using PlatformIndependentIDataObject = System.Windows.Forms.IDataObject;
#else
using PlatformIndependentIDataObject = System.Windows.IDataObject;
#endif
using System;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.Office.Commands.Internal;
namespace DevExpress.XtraSpreadsheet {
	public class SpreadsheetPasteSource : ClipboardPasteSource {
		IClipboardProvider clipboardProvider;
		public SpreadsheetPasteSource(IClipboardProvider clipboardProvider) {
			this.clipboardProvider = clipboardProvider;
		}
		public override object GetData(string format, bool autoConvert) {
#if !SL && !DXPORTABLE
			PlatformIndependentIDataObject dataObject = clipboardProvider.GetDataObject();
			if (dataObject == null)
				return null;
			else
				return dataObject.GetData(format, autoConvert);
#else
			return DevExpress.Office.Utils.OfficeClipboard.GetData(format);
#endif
		}
		public override bool ContainsData(string format, bool autoConvert) {
#if !SL && !DXPORTABLE
			PlatformIndependentIDataObject dataObject = clipboardProvider.GetDataObject();
			if (dataObject == null)
				return false;
			else
				return dataObject.GetDataPresent(format, autoConvert);
#else
			return DevExpress.Office.Utils.OfficeClipboard.ContainsData(format);
#endif
		}
	}
}
