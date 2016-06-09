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
using System.Collections;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class JSONInlineObjectModifiersTable : Dictionary<JSONInlineObjectProperty, JSONPieceTableModifier<IntervalCommandState>> { }
	public class ChangeInlineObjectCommand : WebRichEditPropertyStateBasedCommand<IntervalCommandState, JSONInlineObjectProperty> {
		public ChangeInlineObjectCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.ChangeInlineObjectProperties; } }
		protected override bool IsEnabled() { return true; }
		static JSONInlineObjectModifiersTable modifiers = CreateModifiers();
		static JSONInlineObjectModifiersTable CreateModifiers() {
			JSONInlineObjectModifiersTable result = new JSONInlineObjectModifiersTable();
			result.Add(JSONInlineObjectProperty.Scales, pieceTable => new InlineObjectPropertiesScaleRatesModifier(pieceTable));
			result.Add(JSONInlineObjectProperty.LockAspectRatio, pieceTable => new InlineObjectPropertiesLockAspectRatioModifier(pieceTable));
			return result;
		}
		protected override IModelModifier<IntervalCommandState> CreateModifier(JSONInlineObjectProperty property) {
			JSONPieceTableModifier<IntervalCommandState> creator;
			if (!modifiers.TryGetValue(property, out creator))
				throw new ArgumentException();
			return creator(PieceTable);
		}
	}
	public class InlineObjectPropertiesLockAspectRatioModifier : IntervalModelModifier<bool> {
		public InlineObjectPropertiesLockAspectRatioModifier(PieceTable pieceTable)
			: base(pieceTable) { }
		protected override void ModifyCore(DocumentLogPosition position, int length, bool value) {
			if(length != 1)
				Exceptions.ThrowArgumentException("length", length);
			RunIndex runIndex = PieceTable.FindRunInfo(position, length).Start.RunIndex;
			InlinePictureRun run = (InlinePictureRun)PieceTable.Runs[runIndex];
			run.LockAspectRatio = value;
		}
	}
	public class InlineObjectPropertiesScaleRatesModifier : IntervalModelModifier<ArrayList> {
		public InlineObjectPropertiesScaleRatesModifier(PieceTable pieceTable)
			: base(pieceTable) { }
		protected override void ModifyCore(DocumentLogPosition position, int length, ArrayList value) {
			if(length != 1)
				Exceptions.ThrowArgumentException("length", length);
			RunIndex runIndex = PieceTable.FindRunInfo(position, length).Start.RunIndex;
			InlinePictureRun run = (InlinePictureRun)PieceTable.Runs[runIndex];
			run.ScaleX = Convert.ToSingle(value[0]);
			run.ScaleY = Convert.ToSingle(value[1]);
		}
	}
}
