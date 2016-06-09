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
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Reflection;
using System.IO;
#if DXWINDOW
namespace DevExpress.Internal.DXWindow {
#else
namespace DevExpress.Xpf.Core {
#endif
	public enum OpenLinkType { Blank, Self, Smart }
#if !DXWINDOW
	[DXToolboxBrowsable(false)]
#endif
	public class DocumentPresenter : Control {
		public static void OpenTabLink(string link, OpenLinkType openLinkType) {
			link = PatchLink(link);
				DocumentPresenter.OpenLink(link);
		}
		public static void OpenLink(string link, string target = "_self") {
			link = PatchLink(link);
			RunProgram(link, string.Empty, false);
		}
		static string PatchLink(string link) {
			link = link.Trim();
			string[] protocols = new string[] { "http://", "https://", "mailto:" };
			bool needAddProtocol = true;
			foreach(string protocol in protocols) {
				if(link.IndexOf(protocol, StringComparison.OrdinalIgnoreCase) == 0) {
					needAddProtocol = false;
					break;
				}
			}
			if(needAddProtocol)
				link = protocols[0] + link;
			return link;
		}
		public static int RunProgram(string program, string args, bool waitOnReturn) {
			try {
				Process process = Process.Start(program, args);
				if(waitOnReturn && process != null) {
					process.WaitForExit();
					return process.ExitCode;
				}
			} catch { }
			return 0;
		}
#if DXWINDOW
		const string LocalNamespaceXaml = " xmlns:documentpresenterns=\"clr-namespace:DevExpress.Internal.DXWindow;assembly=" + AssemblyInfo.SRAssemblyDemoDataCore + "\" ";
#else
		const string LocalNamespaceXaml = " xmlns:documentpresenterns=\"clr-namespace:DevExpress.Xpf.Core;assembly=" + AssemblyInfo.SRAssemblyXpfCore + "\" ";
#endif
		const string DefaultTemplateXaml =
			"<ResourceDictionary" + LocalNamespaceXaml + "\n" +
			"    xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"\n" +
			"    xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n" +
			"    <ControlTemplate x:Key=\"DefaultTemplate\" TargetType=\"documentpresenterns:DocumentPresenter\">\n" +
			"        <ContentPresenter x:Name=\"DocumentPresenter\" MinWidth=\"16\" />\n" +
			"    </ControlTemplate>\n" +
			"</ResourceDictionary>";
		static ControlTemplate defaultTemplate;
		static ControlTemplate DefaultTemplate {
			get {
				if(defaultTemplate == null)
					defaultTemplate = (ControlTemplate)((ResourceDictionary)LoadXaml(DefaultTemplateXaml))["DefaultTemplate"];
				return defaultTemplate;
			}
		}
		static object LoadXaml(string xaml) {
			try {
				return XamlReader.Parse(xaml);
			} catch {
				return null;
			}
		}
		#region Dependency Properties
		public static readonly DependencyProperty DocumentProperty;
		public static readonly DependencyProperty NavigateUriProperty;
		static DocumentPresenter() {
			Type ownerType = typeof(DocumentPresenter);
			DocumentProperty = DependencyProperty.Register("Document", typeof(string), ownerType, new PropertyMetadata(null, RaiseDocumentChanged));
			NavigateUriProperty = DependencyProperty.RegisterAttached("NavigateUri", typeof(string), ownerType, new PropertyMetadata(null, RaiseNavigateUriChanged));
		}
		static void RaiseDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((DocumentPresenter)d).OnDocumentChanged(e);
		}
		#endregion
		ContentPresenter documentPresenter;
		public DocumentPresenter() {
			Template = DefaultTemplate;
		}
		public string Document { get { return (string)GetValue(DocumentProperty); } set { SetValue(DocumentProperty, value); } }
		public static string GetNavigateUri(Hyperlink hyperlink) { return (string)hyperlink.GetValue(NavigateUriProperty); }
		public static void SetNavigateUri(Hyperlink hyperlink, string v) { hyperlink.SetValue(NavigateUriProperty, v); }
		public static event EventHandler<CancelEventArgs> HyperlinkClick;
		static void RaiseNavigateUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Hyperlink hyperlink = (Hyperlink)d;
			hyperlink.Click += OnHyperlinkClick;
		}
		static void OnHyperlinkClick(object sender, RoutedEventArgs e) {
			CancelEventArgs cea = new CancelEventArgs();
			if(HyperlinkClick != null)
				HyperlinkClick(sender, cea);
			if(cea.Cancel) return;
			Hyperlink hyperlink = (Hyperlink)sender;
			OpenLink(GetNavigateUri(hyperlink), string.IsNullOrEmpty(hyperlink.TargetName) ? "_blank" : hyperlink.TargetName);
		}
		protected override Size MeasureOverride(Size constraint) {
			if(double.IsInfinity(constraint.Width))
				constraint = new Size();
			return base.MeasureOverride(constraint);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(this.documentPresenter != null)
				this.documentPresenter.Clip = new RectangleGeometry() { Rect = new Rect(-1.0, 0.0, finalSize.Width + 2, finalSize.Height + 2) };
			return base.ArrangeOverride(finalSize);
		}
		void OnDocumentChanged(DependencyPropertyChangedEventArgs e) {
			UpdateDocument();
		}
		void UpdateDocument() {
			if(documentPresenter == null) return;
			if(Document == null) {
				documentPresenter.Content = null;
				return;
			}
			documentPresenter.Content = LoadXaml(ParsedDocument);
		}
		string ParsedDocument {
			get {
				StringBuilder sb = new StringBuilder(Document);
				sb.Replace(" NavigateUri=", " documentpresenterns:DocumentPresenter.NavigateUri=");
				sb.Replace('[', '<');
				sb.Replace(']', '>');
				sb.Replace("<<", "[");
				sb.Replace(">>", "]");
				StringBuilder namespaces = new StringBuilder();
				while(true) {
					int d = sb.ToString().IndexOf("+xmlns", StringComparison.Ordinal);
					if(d < 0) break;
					int e = sb.ToString().IndexOf('|', d);
					if(e < 0) break;
					namespaces.Append(' ');
					namespaces.Append(sb.ToString().Substring(d + 1, e - (d + 1)));
					sb.Remove(d, e + 1);
				}
				string ForegroundString = GetSolidColorBrush(Foreground as SolidColorBrush);
				ForegroundString = ForegroundString == null ? string.Empty : string.Format("Foreground=\"{0}\"", ForegroundString);
				string BackgroundString = GetSolidColorBrush(Background as SolidColorBrush);
				BackgroundString = BackgroundString == null ? string.Empty : string.Format("Background=\"{0}\"", BackgroundString);
				string margin = "Margin=\"-6,-1,-6,-1\"";
				string parameters = string.Format("Cursor=\"Arrow\" IsReadOnly=\"True\" HorizontalScrollBarVisibility=\"Disabled\" BorderThickness=\"0\" BorderBrush=\"Transparent\" {0} {1} {2} {3}", ForegroundString, BackgroundString, namespaces.ToString(), margin);
				parameters += " IsDocumentEnabled=\"True\"";
				if(!Focusable)
					parameters += " Focusable=\"False\"";
#if !DXWINDOW
				if(!FocusHelper2.GetFocusable(this))
					parameters += " documentpresenterns:FocusHelper2.Focusable=\"False\"";
#endif
				string header = string.Format("<RichTextBox" + LocalNamespaceXaml +"xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" {0}>", parameters);
				sb.Insert(0, "<FlowDocument>");
				sb.Insert(0, header);
				sb.Append("</FlowDocument>");
				sb.Append("</RichTextBox>");
				return sb.ToString();
			}
		}
		protected internal void InsertLineBreaks(StringBuilder sb) {
			if(sb.ToString().Length == 0) return;
			List<int> paragraphEnds = new List<int>();
			string paragraphEnd = "</PARAGRAPH>";
			string lineBreak = "<LineBreak/>";
			string str = sb.ToString().ToUpperInvariant();
			int currentPos = -1;
			while(true) {
				currentPos = str.IndexOf(paragraphEnd, currentPos + 1, StringComparison.Ordinal);
				if(currentPos < 0) break;
				paragraphEnds.Add(currentPos);
			}
			for(int i = paragraphEnds.Count - 1; --i >= 0; ) {
				sb.Insert(paragraphEnds[i], lineBreak);
			}
		}
		string GetSolidColorBrush(SolidColorBrush brush) {
			if(brush == null) return null;
			return brush.Color.ToString();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			documentPresenter = (ContentPresenter)GetTemplateChild("DocumentPresenter");
			UpdateDocument();
		}
	}
}
