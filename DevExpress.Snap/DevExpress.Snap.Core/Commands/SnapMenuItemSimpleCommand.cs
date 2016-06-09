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
using System.Linq;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.Snap.Localization;
using System.Reflection;
using DevExpress.Utils.Localization;
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
using DevExpress.Utils.Commands;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.Commands {
	public abstract class SnapMenuItemSimpleCommand : RichEditMenuItemSimpleCommand {
		protected SnapMenuItemSimpleCommand(IRichEditControl control)
			: base(control) {
		}
		#region properties
		protected new SnapDocumentModel DocumentModel {
			get { return (SnapDocumentModel)base.DocumentModel; }
		}
		protected IDataSourceDispatcher DataSourceDispatcher { get { return DocumentModel.DataSourceDispatcher; } }
		protected DocumentCapability CharacterFormatting { get { return DocumentModel.DocumentCapabilities.CharacterFormatting; } }
		protected override string ImageResourcePrefix { get { return "DevExpress.Snap.Images"; } }
		protected override Assembly ImageResourceAssembly { get { return Assembly.GetExecutingAssembly(); } }
		protected override sealed XtraLocalizer<XtraRichEditStringId> Localizer {
			get {
				throw new NotImplementedException();
			}
		}
		public override sealed XtraRichEditStringId DescriptionStringId {
			get { throw new NotImplementedException(); }
		}
		public override sealed XtraRichEditStringId MenuCaptionStringId {
			get { throw new NotImplementedException(); }
		}
		public override sealed string MenuCaption {
			get {
				return LocalizationHelper.GetMenuCaption(this);
			}
		}
		public override sealed string Description {
			get {
				return LocalizationHelper.GetDescription(this);
			}
		}
		#endregion
		protected internal override void ApplyCommandsRestriction(ICommandUIState state, DocumentCapability option) {
			ApplyCommandRestrictionOnEditableControl(state, option);
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
	}
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class CommandLocalizationAttribute : Attribute {
		#region properties
		public SnapStringId MenuCaptionId { get; private set; }
		public SnapStringId DescriptionId { get; private set; }
		#endregion
		public CommandLocalizationAttribute(SnapStringId menuCaptionId, SnapStringId descriptionId) {
			this.MenuCaptionId = menuCaptionId;
			this.DescriptionId = descriptionId;
		}
	}
	public static class LocalizationHelper {
		public static string GetMenuCaption(RichEditCommand richEditCommand) {
			CommandLocalizationAttribute snapCommandAttribute = TypeDescriptor.GetAttributes(richEditCommand)[typeof(CommandLocalizationAttribute)] as CommandLocalizationAttribute;
			return snapCommandAttribute != null ? SnapLocalizer.Active.GetLocalizedString(snapCommandAttribute.MenuCaptionId) : string.Empty;
		}
		public static string GetDescription(RichEditCommand richEditCommand) {
			CommandLocalizationAttribute snapCommandAttribute = TypeDescriptor.GetAttributes(richEditCommand)[typeof(CommandLocalizationAttribute)] as CommandLocalizationAttribute;
			return snapCommandAttribute != null ? SnapLocalizer.Active.GetLocalizedString(snapCommandAttribute.DescriptionId) : string.Empty;
		}
	}
}
