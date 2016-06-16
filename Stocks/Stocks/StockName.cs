using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stocks
{
    enum issue_columns
    {
        code, symbol, name,
    }

    class StockName
    {
        public UInt16 code { get; set; }
        public String symbol { get; set; }
        public String name { get; set; }

        public override string ToString()
        {
            return String.Format("{0},{1},{2}", code, symbol, name);
        }
    }
}
