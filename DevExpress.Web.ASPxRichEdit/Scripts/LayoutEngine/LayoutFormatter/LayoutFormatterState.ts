module __aspxRichEdit {
    export enum LayoutFormatterState {
        DocumentStart = 0,//DocumentStart, no pages in new layout
        PageStart = 1,//Page start. Page can be already in the layout (due to calculation 
        PageAreaStart = 2,//Page Area Start - page already in the layout.
        ColumnStart = 3,//Column start - page & page area already in the layout

        RowFormatting = 4, //Inside the column. Page, page area & column already in the layout

        ColumnEnd = 5,//Column formatting complete. It can be used
        PageAreaEnd = 6,//Page area formatting complete. It can be used
        PageEnd = 7,//Page formatting complete. It can be used. Page can't be used unit PageEnd (even if next page were obtained).
        DocumentEnd = 8
    }
} 