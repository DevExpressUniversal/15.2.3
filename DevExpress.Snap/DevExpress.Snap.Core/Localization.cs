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
namespace DevExpress.Snap.Localization {
	[DXToolboxItem(false)]
	public partial class SnapLocalizer : XtraLocalizer<SnapStringId> {
#if !SL
	[DevExpressSnapCoreLocalizedDescription("SnapLocalizerActive")]
#endif
		public static new XtraLocalizer<SnapStringId> Active {
			get { return XtraLocalizer<SnapStringId>.Active; }
			set { XtraLocalizer<SnapStringId>.Active = value; }
		}
		static SnapLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<SnapStringId>(CreateDefaultLocalizer()));
		}
		public static XtraLocalizer<SnapStringId> CreateDefaultLocalizer() {
			return new SnapResLocalizer();
		}
		public static string GetString(SnapStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<SnapStringId> CreateResXLocalizer() {
			return new SnapResLocalizer();
		}
		protected override void PopulateStringTable() {
			AddStrings();
		}
	}
	[DXToolboxItem(false)]
	public class SnapResLocalizer : XtraResXLocalizer<SnapStringId> {
		public const string DefaultResourceFile = "LocalizationRes";
		public SnapResLocalizer()
			: base(new SnapLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Snap.LocalizationRes", typeof(SnapResLocalizer).Assembly);
		}
	}
}
