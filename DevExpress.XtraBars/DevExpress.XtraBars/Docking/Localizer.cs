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
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.XtraBars.Docking {
	#region enum DocumentManagerLocalizerId
	public enum DockManagerStringId {
		CommandActivate,
		CommandClose,
		CommandFloat,
		CommandDock,
		CommandDockAsTabbedDocument,
		CommandAutoHide,
		CommandMaximize,
		CommandRestore,
		MessageFormPropertyChangedCaption,
		MessageFormPropertyChangedText
	}
	#endregion
	class DockManagerLocalizer : XtraLocalizer<DockManagerStringId> {
		#region static
		static DockManagerLocalizer() {
			SetActiveLocalizerProvider(
					new DefaultActiveLocalizerProvider<DockManagerStringId>(CreateDefaultLocalizer())
				);
		}
		public new static XtraLocalizer<DockManagerStringId> Active {
			get { return XtraLocalizer<DockManagerStringId>.Active; }
			set { XtraLocalizer<DockManagerStringId>.Active = value; }
		}
		public static XtraLocalizer<DockManagerStringId> CreateDefaultLocalizer() {
			return new DockManagerResXLocalizer();
		}
		public static string GetString(DockManagerStringId id) {
			return Active.GetLocalizedString(id);
		}
		#endregion static
		public override XtraLocalizer<DockManagerStringId> CreateResXLocalizer() {
			return new DockManagerResXLocalizer();
		}
		protected override void PopulateStringTable() {
			#region AddString
			AddString(DockManagerStringId.CommandActivate, "Activate");
			AddString(DockManagerStringId.CommandClose, "Close");
			AddString(DockManagerStringId.CommandFloat, "Float");
			AddString(DockManagerStringId.CommandDock, "Dock");
			AddString(DockManagerStringId.CommandDockAsTabbedDocument, "Dock as Tabbed Document");
			AddString(DockManagerStringId.CommandAutoHide, "Auto Hide");
			AddString(DockManagerStringId.CommandMaximize, "Maximize");
			AddString(DockManagerStringId.CommandRestore, "Restore");
			AddString(DockManagerStringId.MessageFormPropertyChangedCaption, "Warning");
			AddString(DockManagerStringId.MessageFormPropertyChangedText, "If you change the 'Form' property all layout changes will be lost. Do you want to continue?");
			#endregion AddString
		}
	}
	public class DockManagerResXLocalizer : XtraResXLocalizer<DockManagerStringId> {
		public DockManagerResXLocalizer()
			: base(new DockManagerLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.XtraBars.Docking.LocalizationRes", typeof(DockManagerResXLocalizer).Assembly);
		}
	}
}
