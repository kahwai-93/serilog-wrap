using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Common.Utilities.Extensions
{
    public static class TableCellExtension
    {
        private static IContainer HeaderCell(this IContainer container)
        {
            return container
                .MinHeight(25)
                .Border(1)
                .Background(Colors.Grey.Lighten2)
                .AlignCenter()
                .AlignMiddle();
        }

        private static IContainer BodyCell(this IContainer container)
        {
            return container
                 .MinHeight(25)
                 .Border(1)
                 .Background(Colors.White)
                 .AlignMiddle();
        }

        public static void LabelCell(this IContainer container, string text) => container.HeaderCell().Text(text).Medium();

        public static IContainer AlignMiddleCell(this IContainer container) => container.BodyCell().AlignCenter();

        public static IContainer AlignLeftCell(this IContainer container) => container.BodyCell().AlignLeft().PaddingLeft(10);

        public static IContainer AlignRightCell(this IContainer container) => container.BodyCell().AlignRight().PaddingRight(10);

        public static IContainer AlignRightCellWithLeftRightPadding(this IContainer container) => container.BodyCell().AlignRight().PaddingRight(10).PaddingLeft(10);
    }
}
