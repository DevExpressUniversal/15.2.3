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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraSpreadsheet.Commands;
namespace DevExpress.XtraSpreadsheet.UI {
	#region SpreadsheetBarController
	[Designer("DevExpress.XtraSpreadsheet.Design.SpreadsheetBarControllerDesigner," + AssemblyInfo.SRAssemblySpreadsheetDesign), DXToolboxItem(false), ToolboxBitmap(typeof(FakeClassInRootNamespace), DevExpress.Utils.ControlConstants.BitmapPath + "SpreadsheetBarController.bmp")]
	public class SpreadsheetBarController : ControlCommandBarControllerBase<SpreadsheetControl, SpreadsheetCommandId> {
		[DefaultValue(null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public SpreadsheetControl SpreadsheetControl { get { return Control; } set { Control = value; } }
		protected override void SetControlCore(SpreadsheetControl value) {
			if (Control != null)
				Control.BarController = null;
			base.SetControlCore(value);
			if (Control != null)
				Control.BarController = this;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet {
	#region FakeClassInRootNamespace (helper class)
	internal class FakeClassInRootNamespace {
	}
	#endregion
}
