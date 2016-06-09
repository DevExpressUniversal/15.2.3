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
using System.Linq;
using System.Text;
using System.ComponentModel;
using DevExpress.Utils.Localization;
using System.Resources;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.Utils.UI.Localization {
	[ToolboxItem(false)]
	public partial class UtilsUILocalizer : XtraLocalizer<UtilsUIStringId> {
		public static new XtraLocalizer<UtilsUIStringId> Active {
			get { return XtraLocalizer<UtilsUIStringId>.Active; }
			set { XtraLocalizer<UtilsUIStringId>.Active = value; }
		}
		internal static XtraLocalizer<UtilsUIStringId> Default {
			get { return GetActiveLocalizerProvider().DefaultLocalizer; }
		}
		static UtilsUILocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<UtilsUIStringId>(CreateDefaultLocalizer()));
		}
		public static XtraLocalizer<UtilsUIStringId> CreateDefaultLocalizer() {
			return new UtilsUIResLocalizer();
		}
		public static string GetString(UtilsUIStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<UtilsUIStringId> CreateResXLocalizer() {
			return new UtilsUIResLocalizer();
		}
		protected override void PopulateStringTable() {
			AddStrings();
		}
	}
	[ToolboxItem(false)]
	public class UtilsUIResLocalizer : XtraResXLocalizer<UtilsUIStringId> {
		public UtilsUIResLocalizer()
			: base(new UtilsUILocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Utils.UI.LocalizationRes", typeof(UtilsUIResLocalizer).Assembly);
		}
	}
}
