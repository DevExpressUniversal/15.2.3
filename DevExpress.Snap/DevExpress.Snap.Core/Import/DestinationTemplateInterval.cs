﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.Import {
	#region SnapTemplateIntervalDestination
	public abstract class SnapTemplateIntervalElementDestination : SnapLeafElementDestination {
		protected SnapTemplateIntervalElementDestination(SnapImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string id = Importer.ReadDxStringAttr("id", reader);
			if (String.IsNullOrEmpty(id))
				return;
			SnapImporter importer = Importer;
			ImportSnapTemplateIntervalInfo interval;
			if (!importer.SnapTemplateIntervals.TryGetValue(id, out interval)) {
				interval = new ImportSnapTemplateIntervalInfo();
				importer.SnapTemplateIntervals.Add(id, interval);
			}
			AssignSnapTemplateIntervalPosition(interval);
		}
		protected internal abstract void AssignSnapTemplateIntervalPosition(ImportSnapTemplateIntervalInfo interval);
	}
	#endregion
	#region SnapTemplateIntervalStartElementDestination
	public class SnapTemplateIntervalStartElementDestination : SnapTemplateIntervalElementDestination {
		public SnapTemplateIntervalStartElementDestination(SnapImporter importer)
			: base(importer) {
		}
		protected internal override void AssignSnapTemplateIntervalPosition(ImportSnapTemplateIntervalInfo interval) {
			interval.Start = Importer.Position.LogPosition;
		}
	}
	#endregion
	#region SnapTemplateIntervalEndElementDestination
	public class SnapTemplateIntervalEndElementDestination : SnapTemplateIntervalElementDestination {
		public SnapTemplateIntervalEndElementDestination(SnapImporter importer)
			: base(importer) {
		}
		protected internal override void AssignSnapTemplateIntervalPosition(ImportSnapTemplateIntervalInfo interval) {
			interval.End = Importer.Position.LogPosition;
		}
	}
	#endregion
	#region TemplateIntervalInfoLeafElementDestination (abstract class)
	public abstract class TemplateIntervalInfoLeafElementDestination : SnapLeafElementDestination {
		protected TemplateIntervalInfoLeafElementDestination(SnapImporter importer, ImportSnapTemplateInfo templateIntervalInfo)
			: base(importer) {
			TemplateIntervalInfo = templateIntervalInfo;
		}
		public ImportSnapTemplateInfo TemplateIntervalInfo { get; private set; }
	}
	#endregion
}
