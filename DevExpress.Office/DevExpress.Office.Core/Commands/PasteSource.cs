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
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Office.Utils;
namespace DevExpress.Office.Commands.Internal {
	#region PasteSource (abstract class)
	public abstract class PasteSource {
		public virtual object GetData(string format) {
			return GetData(format, false);
		}
		public virtual bool ContainsData(string format) {
			return ContainsData(format, false);
		}
		public abstract object GetData(string format, bool autoConvert);
		public abstract bool ContainsData(string format, bool autoConvert);
	}
	#endregion
	#region ClipboardPasteSource
	public class ClipboardPasteSource : PasteSource {
		public override object GetData(string format, bool autoConvert) {
#if !SL && !DXPORTABLE
			IDataObject dataObject = OfficeClipboard.GetDataObject();
			if(dataObject == null)
				return null;
			else
				return dataObject.GetData(format, autoConvert);
#else
			return DevExpress.Office.Utils.OfficeClipboard.GetData(format);
#endif
		}
		public override bool ContainsData(string format, bool autoConvert) {
#if !SL && !DXPORTABLE
			IDataObject dataObject = OfficeClipboard.GetDataObject();
			if(dataObject == null)
				return false;
			else
				return dataObject.GetDataPresent(format, autoConvert);
#else
			return DevExpress.Office.Utils.OfficeClipboard.ContainsData(format);
#endif
		}
	}
	#endregion
	#region DataObjectPasteSource
	public class DataObjectPasteSource : PasteSource {
		IDataObject dataObject;
		public DataObjectPasteSource(IDataObject dataObject) {
			this.dataObject = dataObject;
		}
		public IDataObject DataObject { get { return dataObject; } set { dataObject = value; } }
		public override object GetData(string format, bool autoConvert) {
			try {
				if(dataObject != null) {
#if !SL
					return dataObject.GetData(format, autoConvert);
#else
					return dataObject.GetData(format);
#endif
				}
				else
					return null;
			}
			catch(System.Security.SecurityException) {
				return null; 
			}
		}
		public override bool ContainsData(string format, bool autoConvert) {
			try {
				if(dataObject != null)
					return dataObject.GetDataPresent(format, autoConvert);
				else
					return false;
			}
			catch(System.Security.SecurityException) {
				return false; 
			}
		}
	}
	#endregion
	#region EmptyPasteSource
	public class EmptyPasteSource : PasteSource {
		public override object GetData(string format, bool autoConvert) {
			return null;
		}
		public override bool ContainsData(string format, bool autoConvert) {
			return false;
		}
	}
	#endregion
}
