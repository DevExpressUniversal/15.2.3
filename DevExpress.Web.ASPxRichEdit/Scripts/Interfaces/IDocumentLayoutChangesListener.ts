module __aspxRichEdit {
    export interface ILayoutChangesListener extends IEventListener {
        //All events are raised after the document layout was changed (i.e. the layout in the new state)

        //NotifyPagesInvalidated is raised when the pages in document layout becomes invalid. The pages are still present in the layout, but they can't be used (only DocumentFormatter can check pages with index >= firstInvalidPageIndex)
        NotifyPagesInvalidated(firstInvalidPageIndex: number);

        //NotifyPageReady is raised, when page is fully ready for display.
        //LayoutPage is the new calculated page, index - the page index in the layout, pageChanges - a collection of changes, which fully describe difference between previous and current version of layoutPage.        
        NotifyPageReady(pageChanges: PageChange);
    }
} 