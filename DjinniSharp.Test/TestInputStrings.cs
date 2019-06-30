using System;
using System.Collections.Generic;
using System.Text;

namespace DjinniSharp.Test
{
    static class TestInputStrings
    {
        public static readonly string
            EmptyString = "",
            OnlyComment = "# This is a comment",
            CommentOnDirectiveOnly = @"# directive:
@direct ""this is a directive""",
            OnlyType = @"typename = interface +c {
  member(arg: string);
}",
            DirectiveAndType = @"@directive ""yes""
" + OnlyType,
            DirectiveAndTwoTypes = DirectiveAndType + "\r\n" + OnlyType;
    }
}
