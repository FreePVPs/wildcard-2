using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hola.Code
{
    class CodeFormatProvider
    {
        public virtual string TextBegin { get; set; } // "
        public virtual string TextEnd { get; set; } // "
        public virtual string IgnoredTextBegin { get; set; } // @"
        public virtual string IgnoredTextEnd { get; set; } // "
        public virtual string Begin { get; set; } // {
        public virtual string End { get; set; } // }
        public virtual string[] CommentBegin { get; set; } // { //, /// }
        public virtual string[] MultilineCommentBegin { get; set; } // { /* }
        public virtual string[] MultilineCommentEnd { get; set; } // { */ }
        public virtual string[] IDELine { get; set; } // #
        public virtual string[] Cycles { get; set; } // { for, while, foreach }
        public virtual string[] IgnoredWords { get; set; } // { try/catch }
        public virtual string[] IgnoredCodeLines { get; set; } // { using, import }
        public virtual bool UseEndOfLine { get; set; } // true
        public virtual string EndOfLine { get; set; } // ;
    }
}
