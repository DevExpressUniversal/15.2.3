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
using System.Text;
using DevExpress.XtraRichEdit.Model;
using System.Text.RegularExpressions;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Services.Implementation;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Fields {
	#region TocEntryField
	public partial class TocEntryField : CalculatedFieldBase {
		#region FieldInitialization
		#region static
		public static readonly string FieldType = "TC";
		static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument("f", "l");
		public static CalculatedFieldBase Create() {
			return new TocEntryField();
		}
		#endregion
		protected override Dictionary<string, bool> SwitchesWithArguments { get { return switchesWithArgument; } }
		protected override string FieldTypeName { get { return FieldType; } }
		string text;
		string id;
		int level;
		bool omitPageNumber;
		public string Text { get { return text; } }
		public string Id { get { return id; } }
		public int Level { get { return level; } }
		public bool OmitPageNumber { get { return omitPageNumber; } }
		public override void Initialize(PieceTable pieceTable, InstructionCollection instructions) {
			base.Initialize(pieceTable, instructions);
			this.text = instructions.GetArgumentAsString(0);
			this.id = instructions.GetString("f");
			this.omitPageNumber = instructions.GetBool("n");
			string levelString = instructions.GetString("l");
			if (levelString == null)
				levelString = "1";
			if (!Int32.TryParse(levelString, out this.level))
				this.level = 0;
		}
		#endregion
		protected override FieldMailMergeType MailMergeType() {
			return FieldMailMergeType.NonMailMerge;
		}
		public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField) {
			return new CalculatedFieldValue(String.Empty);
		}
	}
	#endregion
}
