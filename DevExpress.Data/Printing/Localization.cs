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
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPrinting.Localization {
	public static class PreviewStringIdExtentions {
		public static string GetString(this PreviewStringId id) {
			return PreviewLocalizer.GetString(id);
		}
		public static string GetString(this PreviewStringId id, params object[] args) {
			return string.Format(PreviewLocalizer.GetString(id), args);
		}
	}
	internal class PrintBarManagerBarNames {
		public const string
			MainMenu = "Main Menu",
			Toolbar = "Toolbar",
			StatusBar = "Status Bar";
	}
	[ToolboxItem(false)]
	public partial class PreviewLocalizer : XtraLocalizer<PreviewStringId> {
		internal static readonly PreviewLocalizer Default = new PreviewLocalizer(); 
		public static new XtraLocalizer<PreviewStringId> Active {
			get { return XtraLocalizer<PreviewStringId>.Active; } 
			set {XtraLocalizer<PreviewStringId>.Active = value; } 
		}
		static PreviewLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<PreviewStringId>(CreateDefaultLocalizer()));
		}
		public static XtraLocalizer<PreviewStringId> CreateDefaultLocalizer() {
			return new PreviewResLocalizer();
		}
		public static string GetString(PreviewStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<PreviewStringId> CreateResXLocalizer() {
			return new PreviewResLocalizer();
		}
		protected override void PopulateStringTable() {
			AddStrings();
		}
	}
	[ToolboxItem(false)]
	public class PreviewResLocalizer : XtraResXLocalizer<PreviewStringId> {
		public PreviewResLocalizer()
			: base(new PreviewLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Data.Printing.LocalizationRes", typeof(ResFinder).GetAssembly());
		}
	}
}
