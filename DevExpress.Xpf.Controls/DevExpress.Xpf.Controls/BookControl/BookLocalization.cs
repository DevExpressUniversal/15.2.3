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
namespace DevExpress.Xpf.Controls.Localization {
	#region BookStringId
	public enum BookStringId {
		MenuCmd_PreviousPage,
		MenuCmd_PreviousPageDescription,
		MenuCmd_NextPage,
		MenuCmd_NextPageDescription
	}
	#endregion
	#region BookLocalizer
	public class BookLocalizer : XtraLocalizer<BookStringId> {
		static BookLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<BookStringId>(CreateDefaultLocalizer()));
		}
		public static new XtraLocalizer<BookStringId> Active {
			get { return XtraLocalizer<BookStringId>.Active; }
			set { XtraLocalizer<BookStringId>.Active = value; }
		}
		#region PopulateStringTable
		protected override void PopulateStringTable() {
			AddString(BookStringId.MenuCmd_PreviousPage, "Previous Page");
			AddString(BookStringId.MenuCmd_PreviousPageDescription, "Go To Previous Page");
			AddString(BookStringId.MenuCmd_NextPage, "Next Page");
			AddString(BookStringId.MenuCmd_NextPageDescription, "Go To Next Page");
		}
		#endregion
		public static XtraLocalizer<BookStringId> CreateDefaultLocalizer() {
			return new BookResLocalizer();
		}
		public static string GetString(BookStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<BookStringId> CreateResXLocalizer() {
			return new BookResLocalizer();
		}
	}
	#endregion
	#region BookResLocalizer
	public class BookResLocalizer : XtraResXLocalizer<BookStringId> {
		public BookResLocalizer()
			: base(new BookLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Controls.LocalizationRes", typeof(BookResLocalizer).Assembly);
		}
	}
	#endregion
}
