module __aspxRichEdit {
    export class ControlOptions {
        copy: DocumentCapability = DocumentCapability.Default;
        createNew: DocumentCapability = DocumentCapability.Default;
        cut: DocumentCapability = DocumentCapability.Default;
        drag: DocumentCapability = DocumentCapability.Default;
        drop: DocumentCapability = DocumentCapability.Default;
        open: DocumentCapability = DocumentCapability.Default;
        paste: DocumentCapability = DocumentCapability.Default;
        printing: DocumentCapability = DocumentCapability.Default;
        save: DocumentCapability = DocumentCapability.Default;
        saveAs: DocumentCapability = DocumentCapability.Default;
        fullScreen: DocumentCapability = DocumentCapability.Default;
        tabMarker: string = Utils.specialCharacters.TabMark;
        pageBreakInsertMode: PageBreakInsertMode = PageBreakInsertMode.NewLine;

        characterFormatting: DocumentCapability = DocumentCapability.Default;
        characterStyle: DocumentCapability = DocumentCapability.Default;
        fields: DocumentCapability = DocumentCapability.Default;
        hyperlinks: DocumentCapability = DocumentCapability.Default;
        inlinePictures: DocumentCapability = DocumentCapability.Default;
        paragraphFormatting: DocumentCapability = DocumentCapability.Default;
        paragraphs: DocumentCapability = DocumentCapability.Default;
        paragraphStyle: DocumentCapability = DocumentCapability.Default;
        paragraphTabs: DocumentCapability = DocumentCapability.Default;
        sections: DocumentCapability = DocumentCapability.Default;
        tabSymbol: DocumentCapability = DocumentCapability.Default;
        undo: DocumentCapability = DocumentCapability.Default;
        bookmarks: DocumentCapability = DocumentCapability.Default;

        numberingBulleted: DocumentCapability = DocumentCapability.Default;
        numberingMultiLevel: DocumentCapability = DocumentCapability.Default;
        numberingSimple: DocumentCapability = DocumentCapability.Default;

        headersFooters: DocumentCapability = DocumentCapability.Default;

        tables: DocumentCapability = DocumentCapability.Default;
        tableStyle: DocumentCapability = DocumentCapability.Default;

        static isEnabled(capability: DocumentCapability): boolean {
            return capability === DocumentCapability.Default || capability === DocumentCapability.Enabled;
        }
    }
    export enum DocumentCapability {
        Default = 0,
        Disabled = 1,
        Enabled = 2,
        Hidden = 3
    }
    export enum PageBreakInsertMode {
        NewLine = 0,
        CurrentLine = 1
    }

    export enum BookmarksVisibility {
        Auto = 0,
        Visible = 1,
        Hidden = 2,
    }

    export class MailMergeOptions {
        isEnabled: boolean;
        viewMergedData: boolean;
        activeRecordIndex: number;
        recordCount: number;

        constructor() {
            this.isEnabled = false;
            this.viewMergedData = false;
            this.activeRecordIndex = 0;
            this.recordCount = 0;
        }
    }
}