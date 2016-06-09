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
using DevExpress.XtraSpreadsheet.Utils;
#if !DXPORTABLE
using PlatformIndependentDataObject = System.Windows.Forms.DataObject;
using System.Windows.Forms;
#else
using PlatformIndependentDataObject = DevExpress.Compatibility.System.Windows.Forms.DataObject;
using DevExpress.Compatibility.System.Windows.Forms;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	public interface IClipboard {
		event EventHandler ChangedOutside;
		void SetData(ICopiedRangeDataForClipboard provider, CopiedRangeProvider copiedRange);
		void Clear();
		void LeaveCopiedDataInClipboard();
	}
	public class SpreadsheetClipboard : IClipboard {
		IServiceProvider serviceProvider;
		public event EventHandler ChangedOutside { add { }  remove { }  }
		public SpreadsheetClipboard(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		public bool IsDataAvailable(string format) {
			IClipboardProvider innerClipboard = serviceProvider.GetService(typeof(IClipboardProvider)) as IClipboardProvider;
			return innerClipboard.ContainsData(format);
		}
		public void SetData(ICopiedRangeDataForClipboard dataProvider, CopiedRangeProvider copiedRangeProvider) {
			IDataObject dataObject = new PlatformIndependentDataObject();
			foreach (string dataFormat in dataProvider.DataFormatsForCopy) {
				object data = dataProvider.GetData(dataFormat, copiedRangeProvider);
				if (data != null) {
					dataObject.SetData(dataFormat, data);
				}
			}
			IClipboardProvider clipboardProvider = serviceProvider.GetService(typeof(IClipboardProvider)) as IClipboardProvider;
			clipboardProvider.Clear();
			clipboardProvider.SetDataObject(dataObject, true);
		}
		public void Clear() {
			IClipboardProvider innerClipboard = serviceProvider.GetService(typeof(IClipboardProvider)) as IClipboardProvider;
			innerClipboard.Clear();
		}
		public void LeaveCopiedDataInClipboard() {
		}
	}
}
