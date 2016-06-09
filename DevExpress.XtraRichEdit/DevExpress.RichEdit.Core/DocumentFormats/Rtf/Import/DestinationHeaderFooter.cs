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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
namespace DevExpress.XtraRichEdit.Import.Rtf {
	#region SectionHeaderFooterDestinationBase (abstract class)
	public abstract class SectionHeaderFooterDestinationBase : DestinationPieceTable {
		readonly Section section;
		protected SectionHeaderFooterDestinationBase(RtfImporter importer, Section section, SectionHeaderFooterBase headerFooter)
			: base(importer, headerFooter.PieceTable) {
			Guard.ArgumentNotNull(section, "section");
			this.section = section;
		}
		protected Section Section { get { return section; } }
		public override void FinalizePieceTableCreation() {
			base.FinalizePieceTableCreation();
			if (PieceTable.IsEmpty)
				RemoveEmptyHeaderFooter();
		}
		protected abstract void RemoveEmptyHeaderFooter();
	}
	#endregion
	#region SectionPageHeaderDestination
	public class SectionPageHeaderDestination : SectionHeaderFooterDestinationBase {
		public SectionPageHeaderDestination(RtfImporter importer, Section section, SectionHeader header)
			: base(importer, section, header) {
		}
		protected override DestinationBase CreateClone() {
			return new SectionPageHeaderDestination(Importer, Section, (SectionHeader)PieceTable.ContentType);
		}
		protected override void RemoveEmptyHeaderFooter() {
			SectionHeader header = (SectionHeader)PieceTable.ContentType;
			Section.Headers.Remove(header.Type);
		}
	}
	#endregion
	#region SectionPageFooterDestination
	public class SectionPageFooterDestination : SectionHeaderFooterDestinationBase {
		public SectionPageFooterDestination(RtfImporter importer, Section section, SectionFooter footer)
			: base(importer, section, footer) {
		}
		protected override DestinationBase CreateClone() {
			return new SectionPageFooterDestination(Importer, Section, (SectionFooter)PieceTable.ContentType);
		}
		protected override void RemoveEmptyHeaderFooter() {
			SectionFooter footer = (SectionFooter)PieceTable.ContentType;
			Section.Footers.Remove(footer.Type);
		}
	}
	#endregion
}
