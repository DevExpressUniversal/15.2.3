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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.API.Internal;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Forms {
	#region TabsFormControllerParameters
	public class TabsFormControllerParameters : FormControllerParameters {
		#region Fields
		readonly TabFormattingInfo tabInfo;
		readonly DocumentModelUnitConverter unitConverter;
		readonly IFormOwner formOwner;
		int defaultTabWidth;
		#endregion
		internal TabsFormControllerParameters(IRichEditControl control, TabFormattingInfo tabInfo, int defaultTabWidth, DocumentModelUnitConverter unitConverter)
			: this(control, tabInfo, defaultTabWidth, unitConverter, null) {
		}
		internal TabsFormControllerParameters(IRichEditControl control, TabFormattingInfo tabInfo, int defaultTabWidth, DocumentModelUnitConverter unitConverter, IFormOwner formOwner)
			: base(control) {
			Guard.ArgumentNotNull(tabInfo, "tabInfo");
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.tabInfo = tabInfo;
			this.defaultTabWidth = defaultTabWidth;
			this.unitConverter = unitConverter;
			this.formOwner = formOwner;
		}
		#region Properties
		internal TabFormattingInfo TabInfo { get { return tabInfo; } }
		internal int DefaultTabWidth { get { return defaultTabWidth; } set { defaultTabWidth = value; } }
		internal DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		internal IFormOwner FormOwner { get { return formOwner; } }
		#endregion
	}
	#endregion
	#region TabsFormController
	public class TabsFormController : FormController {
		#region Fields
		readonly DocumentModelUnitConverter unitConverter;
		readonly TabFormattingInfo sourceTabInfo;
		readonly TabsFormControllerParameters controllerParameters;
		TabFormattingInfo tabInfo;
		int defaultTabWidth;
		#endregion
		public TabsFormController(TabsFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.controllerParameters = controllerParameters;
			this.unitConverter = controllerParameters.UnitConverter;
			this.sourceTabInfo = controllerParameters.TabInfo;
			CreateCopies();
		}
		#region Properties
		public TabFormattingInfo SourceTabInfo { get { return sourceTabInfo; } }
		public int SourceDefaultTabWidth { get { return controllerParameters.DefaultTabWidth; } }
		public TabFormattingInfo TabFormattingInfo { get { return tabInfo; } }
		public int DefaultTabWidth { get { return defaultTabWidth; } set { defaultTabWidth = value; }}
		public DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		#endregion
		void CreateCopies() {
			this.tabInfo = SourceTabInfo.Clone();
			this.defaultTabWidth = SourceDefaultTabWidth;
		}
		public override void ApplyChanges() {
			SourceTabInfo.Clear();
			SourceTabInfo.AddRange(TabFormattingInfo);
			controllerParameters.DefaultTabWidth = DefaultTabWidth;
		}
	}
	#endregion
}
