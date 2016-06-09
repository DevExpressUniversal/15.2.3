module __aspxRichEdit {
    export interface IDocumentPageBoundsProvider {
        initialize(sectionProperties: SectionProperties, pageAreaIndex: number, columnIndex: number);
        getNextColumnBounds(): Rectangle;
        getNextPageAreaBounds(): Rectangle;
        getNextPageBounds(headerHeight: number, footerHeight: number): Rectangle;
    }
    export interface IParagraphPropertiesProvider {
        getParagraphProperties(paragraph: Paragraph): ParagraphProperties;
    }
    export interface ILayoutFormatter {
        //---------------------
        //IDocumentFormatter used for calculating document layout.
        //It always calculate layout consequentially, i.e it can't calculate page 10 before pages 0-9.        

        //Calculate layout for page with specified index. If needed, all pages before pageIndex will be calculated. 
        //Method can return null in one of two cases:
        //1) Whole document finished before pageIndex. Document layout calculation completed, but page with pageIndex doesn't present in the layout
        //2) Document loaded partially on the client and formatter reach the end of the loaded part. The request for the next part of document will be placed in the queue before return from this method.
        formatPage(pageIndex: number): LayoutPage;

        //Perform small step in layout calculation.
        //Returns true, if step is OK anf retrurn false in the following cases:
        //1) End of document reached. There are no content for formatting
        //2) Document loaded partially on the client and formatter reach the end of the loaded part. The request for the next part of document will be placed in the queue before return from this method.
        //formatNextRow(): boolean; // available only to the level of formatting the whole page

        //subscribe to formatter events
        addLayoutChangedListener(listener: ILayoutChangesListener): void;

        //unsubscribe from formatter events
        removeLayoutChangedListener(listener: ILayoutChangesListener): void;

        // allow make changes as one transaction
        beginUpdate(): void;
        endUpdate(): void;
    }
}