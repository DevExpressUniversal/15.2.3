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
using System.Data;
using System.Collections;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraVerticalGrid.Localization {
	[ToolboxItem(false)]
	public class VGridLocalizer : XtraLocalizer<VGridStringId> {
		static VGridLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<VGridStringId>(CreateDefaultLocalizer()));
		}
		public new static XtraLocalizer<VGridStringId> Active { 
			get { return XtraLocalizer<VGridStringId>.Active; }
			set { XtraLocalizer<VGridStringId>.Active = value; }
		}
		public override XtraLocalizer<VGridStringId> CreateResXLocalizer() {
			return new VGridResLocalizer();
		}
		public static XtraLocalizer<VGridStringId> CreateDefaultLocalizer() { return new VGridResLocalizer(); }
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(VGridStringId.RowCustomizationText, "Customization");
			AddString(VGridStringId.RowCustomizationNewCategoryFormText, "New Category");
			AddString(VGridStringId.RowCustomizationNewCategoryFormLabelText, "Caption:");
			AddString(VGridStringId.RowCustomizationNewCategoryText, "&New...");
			AddString(VGridStringId.RowCustomizationDeleteCategoryText, "&Delete");
			AddString(VGridStringId.RowCustomizationTabPageCategoriesText, "Categories");
			AddString(VGridStringId.RowCustomizationTabPageRowsText, "Rows");
			AddString(VGridStringId.InvalidRecordExceptionText, " Do you want to correct the value ?");
			AddString(VGridStringId.StyleCreatorName, "customStyleCreator");
			AddString(VGridStringId.MenuReset, "Reset");
			AddString(VGridStringId.MenuRowPropertiesExpressionEditor, "Expression Editor...");
			AddString(VGridStringId.FindControlFindButton, "Find");
			AddString(VGridStringId.FindControlClearButton, "Clear");
			AddString(VGridStringId.FindNullPrompt, "Enter text to search...");
		}
		#endregion
	}
	public class VGridResLocalizer : VGridLocalizer {
		ResourceManager manager = null;
		public VGridResLocalizer() {
			CreateResourceManager();
		}
		protected virtual void CreateResourceManager() {
			if(manager != null) this.manager.ReleaseAllResources();
			this.manager = new ResourceManager("DevExpress.XtraVerticalGrid.Localization.LocalizationRes", typeof(VGridResLocalizer).Assembly);
		}
		protected virtual ResourceManager Manager { get { return manager; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; }}
		public override string GetLocalizedString(VGridStringId id) {
			string resStr = "VGridStringId." + id.ToString();
			string ret = Manager.GetString(resStr);
			if(ret == null) ret = string.Empty;
			return ret;
		}
	}
	#region enum VGridStringId
	public enum VGridStringId {
		RowCustomizationText,
		RowCustomizationNewCategoryFormText,
		RowCustomizationNewCategoryFormLabelText,
		RowCustomizationNewCategoryText,
		RowCustomizationDeleteCategoryText,
		RowCustomizationTabPageCategoriesText,
		RowCustomizationTabPageRowsText,
		InvalidRecordExceptionText,
		StyleCreatorName,
		MenuReset,
		MenuRowPropertiesExpressionEditor,
		FindControlFindButton,
		FindControlClearButton,
		FindNullPrompt
	}
	#endregion
}
