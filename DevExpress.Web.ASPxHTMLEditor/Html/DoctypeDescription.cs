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
using System.Collections.ObjectModel;
using System.Linq;
namespace DevExpress.Web.ASPxHtmlEditor.Internal {
	[Flags]
	public enum DTD { 
		XHTML = 1, HTML5 = 2,
		All = 0xFF
	};
	public static class DtdXhtml10Trans {
		private static readonly DtdElementDeclaration[] elementDecls;
		private static readonly Dictionary<string, int> charEntityNameCodePairs;
		public const string AposEntityName = "apos";
		public const int AposEntityCode = 39;
		static DtdXhtml10Trans() {
			elementDecls = new DtdElementDeclaration[] {
				new DtdElementDeclaration(E.Document, DTD.XHTML | DTD.HTML5, Group.Any),
				DtdHtmlElementDeclaration.Instance,
				#region Forbidden elements
				new DtdElementDeclaration(E.NoFrames, DTD.XHTML, ValidationBehavior.ForbiddenTag, Group.Flow),
				new DtdElementDeclaration(E.BaseFont, DTD.XHTML, ValidationBehavior.ForbiddenTag, Group.Empty),
				new DtdElementDeclaration(E.Applet, DTD.XHTML, ValidationBehavior.ForbiddenTagAndItsContent, Group.PCData, E.Param, Group.Block, E.Form, Group.Inline, Group.Misc),
				new DtdElementDeclaration(E.Menu, DTD.XHTML | DTD.HTML5, ValidationBehavior.ForbiddenTagAndItsContent, E.LI),
				new DtdElementDeclaration(E.Dir, DTD.XHTML, ValidationBehavior.ForbiddenTagAndItsContent, E.LI),
				new DtdElementDeclaration(E.IsIndex, DTD.XHTML, ValidationBehavior.ForbiddenTag, Group.Empty),
				new DtdElementDeclaration(E.Video, DTD.HTML5, 
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.MediaEvents,
						DtdAttributeDeclRepository.GetDecl(A.AutoPlay, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Controls, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Loop, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.PreLoad, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Muted, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Src, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Height, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Width, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Poster, DTD.HTML5)
				), E.Source, E.Track),
				new DtdElementDeclaration(E.Audio, DTD.HTML5, 
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.MediaEvents,
						DtdAttributeDeclRepository.GetDecl(A.AutoPlay, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Controls, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Loop, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.PreLoad, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Src, DTD.HTML5)
				), E.Source, E.Track),
				new DtdElementDeclaration(E.Source, DTD.HTML5, new AttList(
					DtdAttributeDeclRepository.Attrs,
					DtdAttributeDeclRepository.GetDecl(A.Media, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Src, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Type, DTD.HTML5)
				)),
				new DtdElementDeclaration(E.Track, DTD.HTML5, new AttList(
					DtdAttributeDeclRepository.Attrs,
					DtdAttributeDeclRepository.GetDecl(A.Default, DTD.HTML5, AttrValueType.Flag),
					DtdAttributeDeclRepository.GetDecl(A.Kind, DTD.HTML5, AttrValueType.Enum, false, string.Empty, new string[] {"captions", "chapters", "descriptions", "metadata", "subtitles"}),
					DtdAttributeDeclRepository.GetDecl(A.Label, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Src, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.SrcLang, DTD.HTML5)
				)),
				#endregion
				#region Optionally Forbidden Elements
				new DtdElementDeclaration(E.Head, DTD.XHTML | DTD.HTML5,  new AttList(
				   DtdAttributeDeclRepository.GlobalAttributes
				   ), ValidationBehavior.HeadValidationBehavior, E.Title, E.BaseElement, Group.HeadMisc, E.NoScript),
				new DtdElementDeclaration(E.Title, DTD.XHTML | DTD.HTML5,  new AttList(
				   DtdAttributeDeclRepository.GlobalAttributes
				   ), ValidationBehavior.TitleValidationBehavior, Group.PCData),
				new DtdElementDeclaration(E.BaseElement, DTD.XHTML | DTD.HTML5,
					 new AttList(
						 DtdAttributeDeclRepository.GlobalAttributes,
						 DtdAttributeDeclRepository.GetDecl(A.Href, DTD.XHTML | DTD.HTML5, AttrValueType.Text),
						 DtdAttributeDeclRepository.GetDecl(A.Target, DTD.XHTML | DTD.HTML5, AttrValueType.Text) 
						 ), ValidationBehavior.BaseTagValidationBehavior, Group.Empty),
				new DtdElementDeclaration(E.Meta, DTD.XHTML | DTD.HTML5,
					 new AttList(
						 DtdAttributeDeclRepository.GlobalAttributes,
						 DtdAttributeDeclRepository.GetDecl(A.Name, DTD.HTML5, AttrValueType.Text),
						 DtdAttributeDeclRepository.GetDecl(A.HttpEquiv, DTD.HTML5, AttrValueType.Text),
						 DtdAttributeDeclRepository.GetDecl(A.Charset, DTD.HTML5, AttrValueType.Text),
						 DtdAttributeDeclRepository.GetDecl(A.Content, DTD.HTML5, AttrValueType.Text) 
						 ), ValidationBehavior.MetaTagValidationBehavior, Group.Empty),
				new DtdElementDeclaration(E.Style, DTD.XHTML | DTD.HTML5,  new AttList(
						DtdAttributeDeclRepository.GlobalAttributes,
						DtdAttributeDeclRepository.GetDecl(A.Media, DTD.HTML5, AttrValueType.Text),
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.HTML5, AttrValueType.Text),
						DtdAttributeDeclRepository.GetDecl(A.Title, DTD.XHTML | DTD.HTML5, AttrValueType.Text)
					),
					ValidationBehavior.OptionallyForbiddenTagAndItsContent, Group.PCData),
				new DtdElementDeclaration(E.Link, DTD.XHTML | DTD.HTML5, new AttList(
						DtdAttributeDeclRepository.GlobalAttributes,
						DtdAttributeDeclRepository.GetDecl(A.Href, DTD.HTML5, AttrValueType.Text),
						DtdAttributeDeclRepository.GetDecl(A.CrossOrigin, DTD.XHTML | DTD.HTML5, AttrValueType.Text),
						DtdAttributeDeclRepository.GetDecl(A.Rel, DTD.XHTML | DTD.HTML5, AttrValueType.Text, true, string.Empty),
						DtdAttributeDeclRepository.GetDecl(A.Media, DTD.XHTML | DTD.HTML5, AttrValueType.Text),
						DtdAttributeDeclRepository.GetDecl(A.HrefLang, DTD.XHTML | DTD.HTML5, AttrValueType.Text),
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.XHTML | DTD.HTML5, AttrValueType.Text),
						DtdAttributeDeclRepository.GetDecl(A.Sizes, DTD.XHTML | DTD.HTML5, AttrValueType.Text),
						DtdAttributeDeclRepository.GetDecl(A.Title, DTD.XHTML | DTD.HTML5, AttrValueType.Text)
					), ValidationBehavior.OptionallyForbiddenTag, Group.Empty),
				new DtdElementDeclaration(E.Body, DTD.XHTML | DTD.HTML5, new AttList(
						DtdAttributeDeclRepository.GlobalAttributes
					), ValidationBehavior.OptionallyForbiddenTag, Group.Flow),
				new DtdElementDeclaration(E.Script, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.GetDecl(A.ID, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.XHTML | DTD.HTML5, AttrValueType.Text, true, V.ScriptTypeDefault),
						DtdAttributeDeclRepository.GetDecl(A.Charset, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Language, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Src, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Defer, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Async, DTD.HTML5, AttrValueType.Flag)
					),
					ValidationBehavior.OptionallyForbiddenTagAndItsContent, Group.PCData),
				new DtdElementDeclaration(E.IFrame, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.CoreAttrs,
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.LongDesc, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Src, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.FrameBorder, DTD.XHTML, AttrValueType.Enum, false, V.FrameBorderDefault, V.FrameBorder),
						DtdAttributeDeclRepository.GetDecl(A.MarginWidth, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.MarginHeight, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Scrolling, DTD.XHTML, AttrValueType.Enum, false, V.ScrollingDefault, V.Scrolling),
						DtdAttributeDeclRepository.GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.ImageAlign),
						DtdAttributeDeclRepository.GetDecl(A.Width, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Height, DTD.XHTML | DTD.HTML5)
					),
					ValidationBehavior.OptionallyForbiddenTag, Group.Flow),
				new DtdElementDeclaration(E.Form, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.FormEvents,
						DtdAttributeDeclRepository.GetDecl(A.Action, DTD.XHTML | DTD.HTML5, AttrValueType.Text, true, string.Empty),
						DtdAttributeDeclRepository.GetDecl(A.Method, DTD.XHTML | DTD.HTML5, AttrValueType.Enum, false, V.MethodDefault, V.Method),
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.EncType, DTD.XHTML | DTD.HTML5, AttrValueType.Text, false, V.EncTypeDefault),
						DtdAttributeDeclRepository.GetDecl(A.Accept, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.AcceptCharset, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Target, DTD.XHTML | DTD.HTML5)
					),
					ValidationBehavior.OptionallyForbiddenTag, Group.FormContent),
				new DtdElementDeclaration(E.Label, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.Focus,
						DtdAttributeDeclRepository.GetDecl(A.For, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Form, DTD.HTML5)
					),
					ValidationBehavior.OptionallyForbiddenTag, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Input, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.FormEvents,
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.XHTML | DTD.HTML5, AttrValueType.Enum, false, V.InputTypeDefault, V.InputType),
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Value, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Checked, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Disabled, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.ReadOnly, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Size, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.MaxLength, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Src, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Alt, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.UseMap, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Accept, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.ImageAlign),
						DtdAttributeDeclRepository.GetDecl(A.AutoComplete, DTD.HTML5, AttrValueType.Enum, false, string.Empty, new string[] { "on", "off" }),
						DtdAttributeDeclRepository.GetDecl(A.AutoFocus, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Form, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.FormAction, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.FormEncType, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.FormMethod, DTD.HTML5, AttrValueType.Enum, false, string.Empty, new string[] { "get", "post" }),
						DtdAttributeDeclRepository.GetDecl(A.FormNoValidate, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.FormTarget, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Height, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.List, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Max, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Min, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Multiple, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Pattern, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Placeholder, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Required, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Step, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Width, DTD.HTML5, AttrValueType.Flag)
					),
					ValidationBehavior.OptionallyForbiddenTagAndItsContent, Group.Empty),
				new DtdElementDeclaration(E.Select, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.FormEvents,
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Size, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Multiple, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Disabled, DTD.XHTML | DTD.HTML5, AttrValueType.Flag)
					),
					ValidationBehavior.OptionallyForbiddenTagAndItsContent, E.OptGroup, E.Option),
				new DtdElementDeclaration(E.Option, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.FormEvents,
						DtdAttributeDeclRepository.GetDecl(A.Selected, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Disabled, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Label, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Value, DTD.XHTML | DTD.HTML5)
					),
					ValidationBehavior.OptionallyForbiddenTagAndItsContent, Group.PCData),
				new DtdElementDeclaration(E.OptGroup, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Disabled, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Label, DTD.XHTML | DTD.HTML5, AttrValueType.Text, true, string.Empty)
					),
					ValidationBehavior.OptionallyForbiddenTagAndItsContent),
				new DtdElementDeclaration(E.TextArea, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.FormEvents,
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Rows, DTD.XHTML | DTD.HTML5, AttrValueType.Text, true, string.Empty),
						DtdAttributeDeclRepository.GetDecl(A.Cols, DTD.XHTML | DTD.HTML5, AttrValueType.Text, true, string.Empty),
						DtdAttributeDeclRepository.GetDecl(A.Disabled, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.ReadOnly, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.AutoFocus, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Form, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.MaxLength, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Placeholder, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Required, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Wrap, DTD.HTML5, AttrValueType.Enum, false, string.Empty, new string[] { "hard", "soft" })
					),
					ValidationBehavior.OptionallyForbiddenTagAndItsContent, Group.PCData),
				new DtdElementDeclaration(E.Datalist, DTD.HTML5, 
					new AttList(
						DtdAttributeDeclRepository.Attrs
					), 
					ValidationBehavior.OptionallyForbiddenTagAndItsContent, E.Option),
				new DtdElementDeclaration(E.Button, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.FormEvents,
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Value, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.XHTML | DTD.HTML5, AttrValueType.Enum, false, null, V.ButtonType),
						DtdAttributeDeclRepository.GetDecl(A.Disabled, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.AutoFocus, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Form, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.FormAction, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.FormEncType, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.FormMethod, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.FormNoValidate, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.FormTarget, DTD.HTML5)
					),
					ValidationBehavior.OptionallyForbiddenTagAndItsContent, Group.ButtonContent),
					new DtdElementDeclaration(E.Keygen, DTD.HTML5, new AttList(
					DtdAttributeDeclRepository.Attrs,
					DtdAttributeDeclRepository.FormEvents,
					DtdAttributeDeclRepository.GetDecl(A.AutoFocus, DTD.HTML5, AttrValueType.Flag),
					DtdAttributeDeclRepository.GetDecl(A.Challenge, DTD.HTML5, AttrValueType.Flag),
					DtdAttributeDeclRepository.GetDecl(A.Disabled, DTD.HTML5, AttrValueType.Flag),
					DtdAttributeDeclRepository.GetDecl(A.Form, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Keytype, DTD.HTML5, AttrValueType.Enum, false, string.Empty, new string[] {"rsa","dsa","ec"}),
					DtdAttributeDeclRepository.GetDecl(A.Name, DTD.HTML5)
				), ValidationBehavior.OptionallyForbiddenTagAndItsContent, Group.Empty),
				new DtdElementDeclaration(E.Output, DTD.HTML5, new AttList(
					DtdAttributeDeclRepository.Attrs,
					DtdAttributeDeclRepository.FormEvents,
					DtdAttributeDeclRepository.GetDecl(A.For, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Form, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Name, DTD.HTML5)
				), ValidationBehavior.OptionallyForbiddenTagAndItsContent, Group.Flow),
				new DtdElementDeclaration(E.Progress, DTD.HTML5, new AttList(
					DtdAttributeDeclRepository.Attrs,
					DtdAttributeDeclRepository.GetDecl(A.Max, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Value, DTD.HTML5)
				), ValidationBehavior.OptionallyForbiddenTagAndItsContent, Group.Flow),
				#endregion
				#region Deprecated Elements & Elements with Deprecated Attributes
				new DtdElementDeclaration(E.Table, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Summary, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Width, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Height, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Border, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.BorderColor, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Frame, DTD.XHTML, AttrValueType.Enum, false, V.TableFrameDefault, V.TableFrame),
						DtdAttributeDeclRepository.GetDecl(A.Rules, DTD.XHTML, AttrValueType.Enum, false, V.TableRulesDefault, V.TableRules),
						DtdAttributeDeclRepository.GetDecl(A.CellPadding, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.CellSpacing, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.TableAlign),
						DtdAttributeDeclRepository.GetDecl(A.BgColor, DTD.XHTML)
					),
					E.Caption, E.THead, E.TFoot, E.TBody, E.Col, E.ColGroup, E.TR),
				new DtdElementDeclaration(E.U, DTD.XHTML, AttList.Attrs, ValidationBehavior.UElementValidationBehavior, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.S, DTD.XHTML | DTD.HTML5, AttList.Attrs, ValidationBehavior.SAndStrikeElementsValidationBehavior, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Strike, DTD.XHTML, AttList.Attrs, ValidationBehavior.SAndStrikeElementsValidationBehavior, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Font, DTD.XHTML,
					new AttList(
						DtdAttributeDeclRepository.CoreAttrs,
						DtdAttributeDeclRepository.I18n,
						DtdAttributeDeclRepository.GetDecl(A.Size, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Color, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Face, DTD.XHTML)
					),
					ValidationBehavior.FontElementValidationBehavior, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Center, DTD.XHTML, AttList.Attrs, ValidationBehavior.CenterElementValidationBehavior, Group.Flow),
				#endregion
				#region Replaceable Elements
				new DtdElementDeclaration(E.I, DTD.XHTML | DTD.HTML5, AttList.Attrs, ValidationBehavior.IElementValidationBehavior, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.B, DTD.XHTML | DTD.HTML5, AttList.Attrs, ValidationBehavior.BElementValidationBehavior, Group.InlineAndTextLevel),
				#endregion
				#region Valid Elements
				new DtdElementDeclaration(E.Meter, DTD.HTML5, new AttList(
					DtdAttributeDeclRepository.Attrs,
					DtdAttributeDeclRepository.GetDecl(A.Form, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.High, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Low, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Max, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Min, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Optimum, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.Value, DTD.HTML5)
				), Group.Flow),
				new DtdElementDeclaration(E.NoScript, DTD.XHTML | DTD.HTML5, AttList.Attrs, ValidationBehavior.NoScriptValidationBehavior, Group.Flow),
				new DtdElementDeclaration(E.Div, DTD.XHTML | DTD.HTML5, AttList.AttrsAndTextAlign, Group.Flow),
				new DtdElementDeclaration(E.P, DTD.XHTML | DTD.HTML5, AttList.AttrsAndTextAlign, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.FieldSet, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.PCData, E.Legend, Group.Block, E.Form, Group.Inline, Group.Misc),
				new DtdElementDeclaration(E.ObjectElement, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Declare, DTD.XHTML, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.ClassID, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.CodeBase, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Data, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.CodeType, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Archive, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.StandBy, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Height, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Width, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.UseMap, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.ImageAlign),
						DtdAttributeDeclRepository.GetDecl(A.Border, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.HSpace, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.VSpace, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Form, DTD.HTML5)
					),
					Group.PCData, E.Param, Group.Block, E.Form, Group.Inline, Group.Misc, E.Embed),
				new DtdElementDeclaration(E.Param, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML | DTD.HTML5, AttrValueType.Text, true, string.Empty),
						DtdAttributeDeclRepository.GetDecl(A.Value, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.ValueType, DTD.XHTML, AttrValueType.Enum, false, V.ValueTypeDefault, V.ValueType),
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.XHTML)
					),
					Group.Empty),
				new DtdElementDeclaration(E.Legend, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.LegendAlign)
					),
					Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Span, DTD.XHTML | DTD.HTML5,
					new AttList(DtdAttributeDeclRepository.Attrs),
					Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.A, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.Focus,		
						DtdAttributeDeclRepository.GetDecl(A.Charset, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Href, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.HrefLang, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Rel, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Rev, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Shape, DTD.XHTML, AttrValueType.Enum, false, string.Empty, V.Shape),
						DtdAttributeDeclRepository.GetDecl(A.Coords, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Target, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Media, DTD.HTML5)
					),
					Group.AContent),
				new DtdElementDeclaration(E.Img, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Src, DTD.XHTML | DTD.HTML5, AttrValueType.Text, true, string.Empty),
						DtdAttributeDeclRepository.GetDecl(A.Alt, DTD.XHTML | DTD.HTML5, AttrValueType.Text, true, string.Empty),
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.LongDesc, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Height, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Width, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.UseMap, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.IsMap, DTD.XHTML | DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.ImageAlign),
						DtdAttributeDeclRepository.GetDecl(A.Border, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.HSpace, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.VSpace, DTD.XHTML)
					),
					Group.Empty),
				new DtdElementDeclaration(E.Map, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.ID, DTD.XHTML, AttrValueType.Text, true, string.Empty),
						DtdAttributeDeclRepository.GetDecl(A.Name, DTD.XHTML | DTD.HTML5)
					),
					Group.Block, E.Form, Group.Misc, E.Area),
				new DtdElementDeclaration(E.Area, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.Focus,
						DtdAttributeDeclRepository.GetDecl(A.Shape, DTD.XHTML | DTD.HTML5, AttrValueType.Enum, false, string.Empty, V.Shape),
						DtdAttributeDeclRepository.GetDecl(A.Coords, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Href, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.NoHref, DTD.XHTML, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Alt, DTD.XHTML | DTD.HTML5, AttrValueType.Text, true, string.Empty),
						DtdAttributeDeclRepository.GetDecl(A.Target, DTD.XHTML | DTD.HTML5)
					),
					Group.Empty),
				new DtdElementDeclaration(E.Bdo, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.CoreAttrs,
						DtdAttributeDeclRepository.Events,
						DtdAttributeDeclRepository.GetDecl(A.Lang, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.XmlLang, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Dir, DTD.XHTML | DTD.HTML5, AttrValueType.Enum, true, V.BdoDirectionDefault, V.Direction)
					),
					Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Hr, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.Align),
						DtdAttributeDeclRepository.GetDecl(A.NoShade, DTD.XHTML, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Size, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Width, DTD.XHTML)
					),
					Group.Empty),
				new DtdElementDeclaration(E.Br, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.CoreAttrs,
						DtdAttributeDeclRepository.GetDecl(A.Clear, DTD.XHTML, AttrValueType.Enum, false, V.ClearDefault, V.Clear)
					),
					Group.Empty),
				new DtdElementDeclaration(E.H1, DTD.XHTML | DTD.HTML5, AttList.AttrsAndTextAlign, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.H2, DTD.XHTML | DTD.HTML5, AttList.AttrsAndTextAlign, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.H3, DTD.XHTML | DTD.HTML5, AttList.AttrsAndTextAlign, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.H4, DTD.XHTML | DTD.HTML5, AttList.AttrsAndTextAlign, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.H5, DTD.XHTML | DTD.HTML5, AttList.AttrsAndTextAlign, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.H6, DTD.XHTML | DTD.HTML5, AttList.AttrsAndTextAlign, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.UL, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.XHTML, AttrValueType.Enum, false, null, V.ULStyle),
						DtdAttributeDeclRepository.GetDecl(A.Compact, DTD.XHTML, AttrValueType.Flag)
					),
					E.LI),
				new DtdElementDeclaration(E.OL, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Compact, DTD.XHTML, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Start, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Reversed, DTD.HTML5)
					),
					E.LI),
				new DtdElementDeclaration(E.LI, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.XHTML),
						DtdAttributeDeclRepository.GetDecl(A.Value, DTD.XHTML | DTD.HTML5)
					),
					Group.Flow),
				new DtdElementDeclaration(E.DL, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Compact, DTD.XHTML, AttrValueType.Flag)
					),
					E.DT, E.DD),
				new DtdElementDeclaration(E.DT, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.DD, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.Flow),
				new DtdElementDeclaration(E.Pre, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Width, DTD.XHTML)
					),
					Group.PreContent),
				new DtdElementDeclaration(E.Address, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.PCData, Group.Inline, Group.MiscInline, E.P),
				new DtdElementDeclaration(E.BlockQuote, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Cite, DTD.XHTML | DTD.HTML5)
					),
					Group.Flow),
				new DtdElementDeclaration(E.Q, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Cite, DTD.XHTML | DTD.HTML5)
					),
					Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Ins, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Cite, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.DateTime, DTD.XHTML | DTD.HTML5)
					),
					Group.Flow),
				new DtdElementDeclaration(E.Del, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Cite, DTD.XHTML | DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.DateTime, DTD.XHTML | DTD.HTML5)
					),
					Group.Flow),
				new DtdElementDeclaration(E.Em, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Strong, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Dfn, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Code, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Samp, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Kbd, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Var, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Cite, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Abbr, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Acronym, DTD.XHTML, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Sub, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Sup, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.TT, DTD.XHTML, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Big, DTD.XHTML, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Small, DTD.XHTML | DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Caption, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Align, DTD.XHTML, AttrValueType.Enum, false, null, V.CaptionAlign)
					),
					Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.THead, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.HeaderCellHAlign,
						DtdAttributeDeclRepository.GetDecl(A.VerticalAlign, DTD.XHTML, AttrValueType.Enum, false, null, V.CellVAlign)
					),
					E.TR),
				new DtdElementDeclaration(E.TFoot, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.HeaderCellHAlign,
						DtdAttributeDeclRepository.GetDecl(A.VerticalAlign, DTD.XHTML, AttrValueType.Enum, false, null, V.CellVAlign)
					),
					E.TR),
				new DtdElementDeclaration(E.TBody, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.DataCellHAlign,
						DtdAttributeDeclRepository.GetDecl(A.VerticalAlign, DTD.XHTML, AttrValueType.Enum, false, null, V.CellVAlign)
					),
					E.TR),
				new DtdElementDeclaration(E.TR, DTD.XHTML | DTD.HTML5,
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.DataCellHAlign,
						DtdAttributeDeclRepository.GetDecl(A.VerticalAlign, DTD.XHTML, AttrValueType.Enum, false, null, V.CellVAlign),
						DtdAttributeDeclRepository.GetDecl(A.BgColor, DTD.XHTML)
					),
					E.TD, E.TH),
				new DtdElementDeclaration(E.TH, DTD.XHTML | DTD.HTML5, AttList.Cell, Group.Flow),
				new DtdElementDeclaration(E.TD, DTD.XHTML | DTD.HTML5, AttList.Cell, Group.Flow),
				new DtdElementDeclaration(E.ColGroup, DTD.XHTML | DTD.HTML5, AttList.Col, E.Col),
				new DtdElementDeclaration(E.Col, DTD.XHTML | DTD.HTML5, AttList.Col, Group.Empty),
				new DtdElementDeclaration(E.Article, DTD.HTML5, AttList.Attrs, Group.Flow),
				new DtdElementDeclaration(E.Aside, DTD.HTML5, AttList.Attrs, Group.Flow),
				new DtdElementDeclaration(E.Canvas, DTD.HTML5, 
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Height, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Width, DTD.HTML5)
					), 
					Group.Flow),
				new DtdElementDeclaration(E.Command, DTD.HTML5, 
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Checked, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Disabled, DTD.HTML5, AttrValueType.Flag),
						DtdAttributeDeclRepository.GetDecl(A.Icon, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Label, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.RadioGroup, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.HTML5, AttrValueType.Enum, false, string.Empty, new string[] { "checkbox", "command", "radio" } )
					), 
					Group.Empty),
				new DtdElementDeclaration(E.Details, DTD.HTML5, 
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Open, DTD.HTML5, AttrValueType.Flag)
					), 
					Group.Flow, E.Summary),
				new DtdElementDeclaration(E.Embed, DTD.HTML5, 
					new AttList(
						DtdAttributeDeclRepository.Attrs,
						DtdAttributeDeclRepository.GetDecl(A.Height, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Width, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Type, DTD.HTML5),
						DtdAttributeDeclRepository.GetDecl(A.Src, DTD.HTML5)
					), 
					Group.Empty),
				new DtdElementDeclaration(E.FigCaption, DTD.HTML5, 
					new AttList(
						DtdAttributeDeclRepository.Attrs
					), 
					Group.Flow),
				new DtdElementDeclaration(E.Figure, DTD.HTML5, 
					new AttList(
						DtdAttributeDeclRepository.Attrs
					), 
					Group.Flow, E.FigCaption),
				new DtdElementDeclaration(E.Footer, DTD.HTML5, AttList.Attrs, Group.Flow),
				new DtdElementDeclaration(E.Header, DTD.HTML5, AttList.Attrs, Group.Flow),
				new DtdElementDeclaration(E.Hgroup, DTD.HTML5, AttList.Attrs, E.H1, E.H2, E.H3, E.H4, E.H5, E.H6),
				new DtdElementDeclaration(E.Mark, DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Nav, DTD.HTML5, AttList.Attrs, Group.Flow),
				new DtdElementDeclaration(E.Rp, DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Rt, DTD.HTML5, AttList.Attrs, Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Ruby, DTD.HTML5, AttList.Attrs, Group.Flow),
				new DtdElementDeclaration(E.Section, DTD.HTML5, AttList.Attrs, Group.Flow),
				new DtdElementDeclaration(E.Summary, DTD.HTML5, AttList.Attrs, Group.Flow),
				new DtdElementDeclaration(E.Time, DTD.HTML5, new AttList(
					DtdAttributeDeclRepository.Attrs,
					DtdAttributeDeclRepository.GetDecl(A.DateTime, DTD.HTML5),
					DtdAttributeDeclRepository.GetDecl(A.PubDate, DTD.HTML5)
				), Group.InlineAndTextLevel),
				new DtdElementDeclaration(E.Wbr, DTD.HTML5, AttList.Attrs, Group.Empty)
				#endregion
			};
			charEntityNameCodePairs = new Dictionary<string, int>();
			#region Character entities: Latin 1
			charEntityNameCodePairs.Add("nbsp", 160);
			charEntityNameCodePairs.Add("iexcl", 161);
			charEntityNameCodePairs.Add("cent", 162);
			charEntityNameCodePairs.Add("pound", 163);
			charEntityNameCodePairs.Add("curren", 164);
			charEntityNameCodePairs.Add("yen", 165);
			charEntityNameCodePairs.Add("brvbar", 166);
			charEntityNameCodePairs.Add("sect", 167);
			charEntityNameCodePairs.Add("uml", 168);
			charEntityNameCodePairs.Add("copy", 169);
			charEntityNameCodePairs.Add("ordf", 170);
			charEntityNameCodePairs.Add("laquo", 171);
			charEntityNameCodePairs.Add("not", 172);
			charEntityNameCodePairs.Add("shy", 173);
			charEntityNameCodePairs.Add("reg", 174);
			charEntityNameCodePairs.Add("macr", 175);
			charEntityNameCodePairs.Add("deg", 176);
			charEntityNameCodePairs.Add("plusmn", 177);
			charEntityNameCodePairs.Add("sup2", 178);
			charEntityNameCodePairs.Add("sup3", 179);
			charEntityNameCodePairs.Add("acute", 180);
			charEntityNameCodePairs.Add("micro", 181);
			charEntityNameCodePairs.Add("para", 182);
			charEntityNameCodePairs.Add("middot", 183);
			charEntityNameCodePairs.Add("cedil", 184);
			charEntityNameCodePairs.Add("sup1", 185);
			charEntityNameCodePairs.Add("ordm", 186);
			charEntityNameCodePairs.Add("raquo", 187);
			charEntityNameCodePairs.Add("frac14", 188);
			charEntityNameCodePairs.Add("frac12", 189);
			charEntityNameCodePairs.Add("frac34", 190);
			charEntityNameCodePairs.Add("iquest", 191);
			charEntityNameCodePairs.Add("Agrave", 192);
			charEntityNameCodePairs.Add("Aacute", 193);
			charEntityNameCodePairs.Add("Acirc", 194);
			charEntityNameCodePairs.Add("Atilde", 195);
			charEntityNameCodePairs.Add("Auml", 196);
			charEntityNameCodePairs.Add("Aring", 197);
			charEntityNameCodePairs.Add("AElig", 198);
			charEntityNameCodePairs.Add("Ccedil", 199);
			charEntityNameCodePairs.Add("Egrave", 200);
			charEntityNameCodePairs.Add("Eacute", 201);
			charEntityNameCodePairs.Add("Ecirc", 202);
			charEntityNameCodePairs.Add("Euml", 203);
			charEntityNameCodePairs.Add("Igrave", 204);
			charEntityNameCodePairs.Add("Iacute", 205);
			charEntityNameCodePairs.Add("Icirc", 206);
			charEntityNameCodePairs.Add("Iuml", 207);
			charEntityNameCodePairs.Add("ETH", 208);
			charEntityNameCodePairs.Add("Ntilde", 209);
			charEntityNameCodePairs.Add("Ograve", 210);
			charEntityNameCodePairs.Add("Oacute", 211);
			charEntityNameCodePairs.Add("Ocirc", 212);
			charEntityNameCodePairs.Add("Otilde", 213);
			charEntityNameCodePairs.Add("Ouml", 214);
			charEntityNameCodePairs.Add("times", 215);
			charEntityNameCodePairs.Add("Oslash", 216);
			charEntityNameCodePairs.Add("Ugrave", 217);
			charEntityNameCodePairs.Add("Uacute", 218);
			charEntityNameCodePairs.Add("Ucirc", 219);
			charEntityNameCodePairs.Add("Uuml", 220);
			charEntityNameCodePairs.Add("Yacute", 221);
			charEntityNameCodePairs.Add("THORN", 222);
			charEntityNameCodePairs.Add("szlig", 223);
			charEntityNameCodePairs.Add("agrave", 224);
			charEntityNameCodePairs.Add("aacute", 225);
			charEntityNameCodePairs.Add("acirc", 226);
			charEntityNameCodePairs.Add("atilde", 227);
			charEntityNameCodePairs.Add("auml", 228);
			charEntityNameCodePairs.Add("aring", 229);
			charEntityNameCodePairs.Add("aelig", 230);
			charEntityNameCodePairs.Add("ccedil", 231);
			charEntityNameCodePairs.Add("egrave", 232);
			charEntityNameCodePairs.Add("eacute", 233);
			charEntityNameCodePairs.Add("ecirc", 234);
			charEntityNameCodePairs.Add("euml", 235);
			charEntityNameCodePairs.Add("igrave", 236);
			charEntityNameCodePairs.Add("iacute", 237);
			charEntityNameCodePairs.Add("icirc", 238);
			charEntityNameCodePairs.Add("iuml", 239);
			charEntityNameCodePairs.Add("eth", 240);
			charEntityNameCodePairs.Add("ntilde", 241);
			charEntityNameCodePairs.Add("ograve", 242);
			charEntityNameCodePairs.Add("oacute", 243);
			charEntityNameCodePairs.Add("ocirc", 244);
			charEntityNameCodePairs.Add("otilde", 245);
			charEntityNameCodePairs.Add("ouml", 246);
			charEntityNameCodePairs.Add("divide", 247);
			charEntityNameCodePairs.Add("oslash", 248);
			charEntityNameCodePairs.Add("ugrave", 249);
			charEntityNameCodePairs.Add("uacute", 250);
			charEntityNameCodePairs.Add("ucirc", 251);
			charEntityNameCodePairs.Add("uuml", 252);
			charEntityNameCodePairs.Add("yacute", 253);
			charEntityNameCodePairs.Add("thorn", 254);
			charEntityNameCodePairs.Add("yuml", 255);
			#endregion
			#region Character entities: Special
			charEntityNameCodePairs.Add("quot", 34);
			charEntityNameCodePairs.Add("amp", 38);
			charEntityNameCodePairs.Add("lt", 38);
			charEntityNameCodePairs.Add("gt", 62);
			charEntityNameCodePairs.Add("apos", 39);
			charEntityNameCodePairs.Add("OElig", 338);
			charEntityNameCodePairs.Add("oelig", 339);
			charEntityNameCodePairs.Add("Scaron", 352);
			charEntityNameCodePairs.Add("scaron", 353);
			charEntityNameCodePairs.Add("Yuml", 376);
			charEntityNameCodePairs.Add("circ", 710);
			charEntityNameCodePairs.Add("tilde", 732);
			charEntityNameCodePairs.Add("ensp", 8194);
			charEntityNameCodePairs.Add("emsp", 8195);
			charEntityNameCodePairs.Add("thinsp", 8201);
			charEntityNameCodePairs.Add("zwnj", 8204);
			charEntityNameCodePairs.Add("zwj", 8205);
			charEntityNameCodePairs.Add("lrm", 8206);
			charEntityNameCodePairs.Add("rlm", 8207);
			charEntityNameCodePairs.Add("ndash", 8211);
			charEntityNameCodePairs.Add("mdash", 8212);
			charEntityNameCodePairs.Add("lsquo", 8216);
			charEntityNameCodePairs.Add("rsquo", 8217);
			charEntityNameCodePairs.Add("sbquo", 8218);
			charEntityNameCodePairs.Add("ldquo", 8220);
			charEntityNameCodePairs.Add("rdquo", 8221);
			charEntityNameCodePairs.Add("bdquo", 8222);
			charEntityNameCodePairs.Add("dagger", 8224);
			charEntityNameCodePairs.Add("Dagger", 8225);
			charEntityNameCodePairs.Add("permil", 8240);
			charEntityNameCodePairs.Add("lsaquo", 8249);
			charEntityNameCodePairs.Add("rsaquo", 8250);
			charEntityNameCodePairs.Add("euro", 8364);
			#endregion
			#region Character entities: Symbol
			charEntityNameCodePairs.Add("fnof", 402);
			charEntityNameCodePairs.Add("Alpha", 913);
			charEntityNameCodePairs.Add("Beta", 914);
			charEntityNameCodePairs.Add("Gamma", 915);
			charEntityNameCodePairs.Add("Delta", 916);
			charEntityNameCodePairs.Add("Epsilon", 917);
			charEntityNameCodePairs.Add("Zeta", 918);
			charEntityNameCodePairs.Add("Eta", 919);
			charEntityNameCodePairs.Add("Theta", 920);
			charEntityNameCodePairs.Add("Iota", 921);
			charEntityNameCodePairs.Add("Kappa", 922);
			charEntityNameCodePairs.Add("Lambda", 923);
			charEntityNameCodePairs.Add("Mu", 924);
			charEntityNameCodePairs.Add("Nu", 925);
			charEntityNameCodePairs.Add("Xi", 926);
			charEntityNameCodePairs.Add("Omicron", 927);
			charEntityNameCodePairs.Add("Pi", 928);
			charEntityNameCodePairs.Add("Rho", 929);
			charEntityNameCodePairs.Add("Sigma", 931);
			charEntityNameCodePairs.Add("Tau", 932);
			charEntityNameCodePairs.Add("Upsilon", 933);
			charEntityNameCodePairs.Add("Phi", 934);
			charEntityNameCodePairs.Add("Chi", 935);
			charEntityNameCodePairs.Add("Psi", 936);
			charEntityNameCodePairs.Add("Omega", 937);
			charEntityNameCodePairs.Add("alpha", 945);
			charEntityNameCodePairs.Add("beta", 946);
			charEntityNameCodePairs.Add("gamma", 947);
			charEntityNameCodePairs.Add("delta", 948);
			charEntityNameCodePairs.Add("epsilon", 949);
			charEntityNameCodePairs.Add("zeta", 950);
			charEntityNameCodePairs.Add("eta", 951);
			charEntityNameCodePairs.Add("theta", 952);
			charEntityNameCodePairs.Add("iota", 953);
			charEntityNameCodePairs.Add("kappa", 954);
			charEntityNameCodePairs.Add("lambda", 955);
			charEntityNameCodePairs.Add("mu", 956);
			charEntityNameCodePairs.Add("nu", 957);
			charEntityNameCodePairs.Add("xi", 958);
			charEntityNameCodePairs.Add("omicron", 959);
			charEntityNameCodePairs.Add("pi", 960);
			charEntityNameCodePairs.Add("rho", 961);
			charEntityNameCodePairs.Add("sigmaf", 962);
			charEntityNameCodePairs.Add("sigma", 963);
			charEntityNameCodePairs.Add("tau", 964);
			charEntityNameCodePairs.Add("upsilon", 965);
			charEntityNameCodePairs.Add("phi", 966);
			charEntityNameCodePairs.Add("chi", 967);
			charEntityNameCodePairs.Add("psi", 968);
			charEntityNameCodePairs.Add("omega", 969);
			charEntityNameCodePairs.Add("thetasym", 977);
			charEntityNameCodePairs.Add("upsih", 978);
			charEntityNameCodePairs.Add("piv", 982);
			charEntityNameCodePairs.Add("bull", 8226);
			charEntityNameCodePairs.Add("hellip", 8230);
			charEntityNameCodePairs.Add("prime", 8242);
			charEntityNameCodePairs.Add("Prime", 8243);
			charEntityNameCodePairs.Add("oline", 8254);
			charEntityNameCodePairs.Add("frasl", 8260);
			charEntityNameCodePairs.Add("weierp", 8472);
			charEntityNameCodePairs.Add("image", 8465);
			charEntityNameCodePairs.Add("real", 8476);
			charEntityNameCodePairs.Add("trade", 8482);
			charEntityNameCodePairs.Add("alefsym", 8501);
			charEntityNameCodePairs.Add("larr", 8592);
			charEntityNameCodePairs.Add("uarr", 8593);
			charEntityNameCodePairs.Add("rarr", 8594);
			charEntityNameCodePairs.Add("darr", 8595);
			charEntityNameCodePairs.Add("harr", 8596);
			charEntityNameCodePairs.Add("crarr", 8629);
			charEntityNameCodePairs.Add("lArr", 8656);
			charEntityNameCodePairs.Add("uArr", 8657);
			charEntityNameCodePairs.Add("rArr", 8658);
			charEntityNameCodePairs.Add("dArr", 8659);
			charEntityNameCodePairs.Add("hArr", 8660);
			charEntityNameCodePairs.Add("forall", 8704);
			charEntityNameCodePairs.Add("part", 8706);
			charEntityNameCodePairs.Add("exist", 8707);
			charEntityNameCodePairs.Add("empty", 8709);
			charEntityNameCodePairs.Add("nabla", 8711);
			charEntityNameCodePairs.Add("isin", 8712);
			charEntityNameCodePairs.Add("notin", 8713);
			charEntityNameCodePairs.Add("ni", 8715);
			charEntityNameCodePairs.Add("prod", 8719);
			charEntityNameCodePairs.Add("sum", 8721);
			charEntityNameCodePairs.Add("minus", 8722);
			charEntityNameCodePairs.Add("lowast", 8727);
			charEntityNameCodePairs.Add("radic", 8730);
			charEntityNameCodePairs.Add("prop", 8733);
			charEntityNameCodePairs.Add("infin", 8734);
			charEntityNameCodePairs.Add("ang", 8736);
			charEntityNameCodePairs.Add("and", 8743);
			charEntityNameCodePairs.Add("or", 8744);
			charEntityNameCodePairs.Add("cap", 8745);
			charEntityNameCodePairs.Add("cup", 8746);
			charEntityNameCodePairs.Add("int", 8747);
			charEntityNameCodePairs.Add("there4", 8756);
			charEntityNameCodePairs.Add("sim", 8764);
			charEntityNameCodePairs.Add("cong", 8773);
			charEntityNameCodePairs.Add("asymp", 8776);
			charEntityNameCodePairs.Add("ne", 8800);
			charEntityNameCodePairs.Add("equiv", 8801);
			charEntityNameCodePairs.Add("le", 8804);
			charEntityNameCodePairs.Add("ge", 8805);
			charEntityNameCodePairs.Add("sub", 8834);
			charEntityNameCodePairs.Add("sup", 8835);
			charEntityNameCodePairs.Add("nsub", 8836);
			charEntityNameCodePairs.Add("sube", 8838);
			charEntityNameCodePairs.Add("supe", 8839);
			charEntityNameCodePairs.Add("oplus", 8853);
			charEntityNameCodePairs.Add("otimes", 8855);
			charEntityNameCodePairs.Add("perp", 8869);
			charEntityNameCodePairs.Add("sdot", 8901);
			charEntityNameCodePairs.Add("lceil", 8968);
			charEntityNameCodePairs.Add("rceil", 8969);
			charEntityNameCodePairs.Add("lfloor", 8970);
			charEntityNameCodePairs.Add("rfloor", 8971);
			charEntityNameCodePairs.Add("lang", 9001);
			charEntityNameCodePairs.Add("rang", 9002);
			charEntityNameCodePairs.Add("loz", 9674);
			charEntityNameCodePairs.Add("spades", 9824);
			charEntityNameCodePairs.Add("clubs", 9827);
			charEntityNameCodePairs.Add("hearts", 9829);
			charEntityNameCodePairs.Add("diams", 9830);
			#endregion
		}
		public static IList<DtdElementDeclaration> ElementDeclarations {
			get { return new ReadOnlyCollection<DtdElementDeclaration>(elementDecls); }
		}
		public static bool IsChildValid(Node parent, Node child, AllowedDocumentType documentType, bool allowHTML5MediaElements, bool allowObjectAndEmbedElements, HtmlEditorContentElementFiltering contentFiltering, bool allowEditFullDocument) {
			DtdElementDeclaration decl = FindElementDecl(parent.Name, documentType, allowHTML5MediaElements, allowObjectAndEmbedElements, contentFiltering);
			if(allowEditFullDocument && string.Equals(decl.Name, E.Html)) {
				return string.Equals(child.Name, E.Head) || string.Equals(child.Name, E.Body);
			}
			return decl != null ? decl.CanContain(child) : false;
		}
		public static bool IsElementEmpty(DtdElementDeclaration decl) {
			return decl != null && decl.IsEmpty;
		}
		public static int GetEntityCodeByName(string name) {
			int code;
			return charEntityNameCodePairs.TryGetValue(name, out code) ? code : -1;
		}
		public static string GetEntityNameByCode(int code) {
			foreach(KeyValuePair<string, int> pair in charEntityNameCodePairs) {
				if(pair.Value == code)
					return pair.Key;
			}
			return null;
		}
		public static DtdElementDeclaration FindElementDecl(string elementName, AllowedDocumentType documentType) {
			return FindElementDecl(elementName, documentType, false, false, null);
		}
		public static DtdElementDeclaration FindElementDecl(string elementName, AllowedDocumentType documentType, HtmlEditorContentElementFiltering contentFiltering) {
			return FindElementDecl(elementName, documentType, false, false, contentFiltering);
		}
		public static DtdElementDeclaration FindElementDecl(string elementName, AllowedDocumentType documentType, bool allowHTML5MediaElements, bool allowObjectAndEmbedElements, HtmlEditorContentElementFiltering contentFiltering) {
			return elementDecls.FirstOrDefault(ed => {
				if(!ed.Name.Equals(elementName, StringComparison.InvariantCultureIgnoreCase))
					return false;
				if(contentFiltering != null && contentFiltering.Tags.Length > 0 && !ed.Name.Equals("html", StringComparison.InvariantCultureIgnoreCase)) {
					List<string> tags = contentFiltering.Tags.ToList<string>();
					foreach(string tagName in tags) {
						if(ed.Name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase))
							return contentFiltering.TagFilterMode == HtmlEditorFilterMode.WhiteList; 
					}
					if(contentFiltering.TagFilterMode == HtmlEditorFilterMode.WhiteList)
						return false;
				}
				if(documentType == AllowedDocumentType.Both)
					return true;
				if(ed.IsEmbedElement || ed.IsObjectElement)
					return allowObjectAndEmbedElements;
				if(ed.IsHTML5MediaElement)
					return allowHTML5MediaElements;
				DTD dtd = documentType == AllowedDocumentType.HTML5 ? DTD.HTML5 : DTD.XHTML;
				return (ed.DTDs & dtd) == dtd;
			});
		}
	}
}
