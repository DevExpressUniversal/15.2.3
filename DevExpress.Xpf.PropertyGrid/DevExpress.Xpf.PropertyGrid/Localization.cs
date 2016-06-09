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

using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
namespace DevExpress.Xpf.PropertyGrid {
	public enum PropertyGridControlStringID {
		SearchEditNullText,
		ResetItemContent,
		RefreshItemContent,
		NewItemInitializerButtonContent,
	}
	public class PropertyGridControlLocalizer : DXLocalizer<PropertyGridControlStringID> {
		static PropertyGridControlLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<PropertyGridControlStringID>(CreateDefaultLocalizer()));
		}
		public static XtraLocalizer<PropertyGridControlStringID> CreateDefaultLocalizer() {
			return new PropertyGridControlResXLocalizer();
		}
		public static string GetString(PropertyGridControlStringID id){
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<PropertyGridControlStringID> CreateResXLocalizer() {
			return new PropertyGridControlResXLocalizer();			
		}
		protected override void PopulateStringTable() {
			AddString(PropertyGridControlStringID.SearchEditNullText, "Search Properties");
			AddString(PropertyGridControlStringID.ResetItemContent, "Reset");
			AddString(PropertyGridControlStringID.RefreshItemContent, "Refresh");
			AddString(PropertyGridControlStringID.NewItemInitializerButtonContent, "New");
		}
	}
	public class PropertyGridControlResXLocalizer : DXResXLocalizer<PropertyGridControlStringID> {
		public PropertyGridControlResXLocalizer() : base(new PropertyGridControlLocalizer()) { }
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.PropertyGrid.LocalizationRes", typeof(PropertyGridControlResXLocalizer).Assembly);
		}
	}
	public class PropertyGridControlStringIdConverter : StringIdConverter<PropertyGridControlStringID> {
		protected override DevExpress.Utils.Localization.XtraLocalizer<PropertyGridControlStringID> Localizer { get { return PropertyGridControlLocalizer.Active; } }
	}
	public class PropertyGridControlLocalizedStringExtension : System.Windows.Markup.MarkupExtension {
		PropertyGridControlStringID stringID;
		public PropertyGridControlLocalizedStringExtension(PropertyGridControlStringID stringID) {
			this.stringID = stringID;
		}
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return PropertyGridControlLocalizer.GetString(stringID);
		}
	}	
}
