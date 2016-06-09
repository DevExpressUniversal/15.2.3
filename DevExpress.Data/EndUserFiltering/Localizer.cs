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

namespace DevExpress.Utils.Filtering.Internal {
	using DevExpress.Utils.Localization;
	using DevExpress.Utils.Localization.Internal;
	#region FilteringLocalizerStringId
	public enum FilteringLocalizerStringId {
		FromName,
		ToName,
		TrueName,
		FalseName,
		DefaultName,
		SelectAllName,
		NullName,
	}
	#endregion FilteringLocalizerStringId
	public class FilteringLocalizer : XtraLocalizer<FilteringLocalizerStringId> {
		#region static
		static FilteringLocalizer() {
			SetActiveLocalizerProvider(
					new DefaultActiveLocalizerProvider<FilteringLocalizerStringId>(CreateDefaultLocalizer())
				);
		}
		public static XtraLocalizer<FilteringLocalizerStringId> CreateDefaultLocalizer() {
			return new FilteringResXLocalizer();
		}
		public new static XtraLocalizer<FilteringLocalizerStringId> Active {
			get { return XtraLocalizer<FilteringLocalizerStringId>.Active; }
			set { XtraLocalizer<FilteringLocalizerStringId>.Active = value; }
		}
		public static string GetString(FilteringLocalizerStringId id) {
			return Active.GetLocalizedString(id);
		}
		#endregion static
		public override XtraLocalizer<FilteringLocalizerStringId> CreateResXLocalizer() {
			return new FilteringResXLocalizer();
		}
		protected override void PopulateStringTable() {
			#region AddString
			AddString(FilteringLocalizerStringId.FromName, "From");
			AddString(FilteringLocalizerStringId.ToName, "To");
			AddString(FilteringLocalizerStringId.TrueName, "True");
			AddString(FilteringLocalizerStringId.FalseName, "False");
			AddString(FilteringLocalizerStringId.DefaultName, "Default");
			AddString(FilteringLocalizerStringId.SelectAllName, "All");
			AddString(FilteringLocalizerStringId.NullName, "Null");
			#endregion AddString
		}
	}
	public class FilteringResXLocalizer : XtraResXLocalizer<FilteringLocalizerStringId> {
		const string baseName = "DevExpress.Data.EndUserFiltering.LocalizationRes";
		public FilteringResXLocalizer()
			: base(new FilteringLocalizer()) {
		}
		protected override System.Resources.ResourceManager CreateResourceManagerCore() {
			return new System.Resources.ResourceManager(baseName, typeof(FilteringResXLocalizer).Assembly);
		}
	}
}
#if DEBUGTEST
namespace DevExpress.Utils.Filtering.Tests {
	using DevExpress.Utils.Localization.Tests;
	using Internal;
	using NUnit.Framework;
	[TestFixture]
	public class LocalizerTest {
		[Test]
		public void CompareLocalizers() {
			LocalizerTester.CompareLocalizers<FilteringLocalizerStringId>(new FilteringLocalizer(), new FilteringResXLocalizer());
		}
	}
}
#endif
