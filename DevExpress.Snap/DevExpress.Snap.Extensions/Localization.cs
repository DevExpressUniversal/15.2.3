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

using System.Resources;
using System.ComponentModel;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.Snap.Extensions.Localization {
	[DXToolboxItem(false)]
	public partial class SnapExtensionsLocalizer : XtraLocalizer<SnapExtensionsStringId> {
#if !SL
	[DevExpressSnapExtensionsLocalizedDescription("SnapExtensionsLocalizerActive")]
#endif
		public static new XtraLocalizer<SnapExtensionsStringId> Active {
			get { return XtraLocalizer<SnapExtensionsStringId>.Active; }
			set { XtraLocalizer<SnapExtensionsStringId>.Active = value; }
		}
		static SnapExtensionsLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<SnapExtensionsStringId>(CreateDefaultLocalizer()));
		}
		public static XtraLocalizer<SnapExtensionsStringId> CreateDefaultLocalizer() {
			return new SnapExtensionsResLocalizer();
		}
		public static string GetString(SnapExtensionsStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<SnapExtensionsStringId> CreateResXLocalizer() {
			return new SnapExtensionsResLocalizer();
		}
		protected override void PopulateStringTable() {
			AddStrings();
		}
	}
	[DXToolboxItem(false)]
	public class SnapExtensionsResLocalizer : XtraResXLocalizer<SnapExtensionsStringId> {
		public const string DefaultResourceFile = "LocalizationRes";
		public SnapExtensionsResLocalizer()
			: base(new SnapExtensionsLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Snap.Extensions.LocalizationRes", typeof(SnapExtensionsResLocalizer).Assembly);
		}
	}
}
