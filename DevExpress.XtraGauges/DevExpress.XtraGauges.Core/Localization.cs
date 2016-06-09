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
using DevExpress.Utils;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core;
namespace DevExpress.XtraGauges.Core.Localization {
	#region enum GaugesCoreStringId
	public enum GaugesCoreStringId {
		MsgPathCreationError,
		MsgGaugeRestoreError,
		MsgTextParsingError,
		MsgInvalidClassCreationParameters
	}
	#endregion
	public class GaugesCoreLocalizer : XtraLocalizer<GaugesCoreStringId> {
		#region static
		static GaugesCoreLocalizer() {
			SetActiveLocalizerProvider(
					new DefaultActiveLocalizerProvider<GaugesCoreStringId>(CreateDefaultLocalizer())
				);
		}
		public static XtraLocalizer<GaugesCoreStringId> CreateDefaultLocalizer() {
			return new GaugesCoreResXLocalizer();
		}
		public static new XtraLocalizer<GaugesCoreStringId> Active {
			get { return XtraLocalizer<GaugesCoreStringId>.Active; }
			set { XtraLocalizer<GaugesCoreStringId>.Active = value; }
		}
		public static string GetString(GaugesCoreStringId id) {
			return Active.GetLocalizedString(id);
		}
		#endregion static
		public override XtraLocalizer<GaugesCoreStringId> CreateResXLocalizer() {
			return new GaugesCoreResXLocalizer();
		}
		protected override void PopulateStringTable() {
			#region AddString
			AddString(GaugesCoreStringId.MsgPathCreationError, "Path can't be created.");
			AddString(GaugesCoreStringId.MsgGaugeRestoreError, "The gauge control can't be restored correctly, because the specified layout file contians the following invalid elements: {0}.");
			AddString(GaugesCoreStringId.MsgTextParsingError, "It's impossible to create an instance of a class {0} because specified text is incorrect: ");
			AddString(GaugesCoreStringId.MsgInvalidClassCreationParameters, "It's impossible to create an instance of a class: {0} because specified parameters are incorrect.");
			#endregion AddString
		}
	}
	public class GaugesCoreResXLocalizer : XtraResXLocalizer<GaugesCoreStringId> {
		public GaugesCoreResXLocalizer()
			: base(new GaugesCoreLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraGauges.Core.LocalizationRes", typeof(GaugesCoreResXLocalizer).GetAssembly());
		}
	}
}
