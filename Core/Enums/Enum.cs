using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public class Enum
    {
        public enum LogLevel
        {
            Trace = 1,
            Debug = 2,
            Info = 3,
            Warning = 4,
            Error = 5,
            Critical = 6,
            Success = 7
        }
        public enum EquityType
        {
            [Description("Stocks")]
            Stocks = 1,

            [Description("Mutual Funds")]
            MutualFunds = 2,

            [Description("Exchange Traded Funds")]
            ETFs = 3
        }

        public enum OperationType
        {
            [Description("Insert")]
            INSERT = 1,

            [Description("Update")]
            UPDATE = 2,

            [Description("Delete")]
            DELETE = 3
        }

        public enum CategoryType
        {
            [Description("Equity")]
            Equity = 1,

            [Description("Debt")]
            Debt = 2,

            [Description("Hybrid")]
            Hybrid = 3,

            [Description("Commodities")]
            Commodities = 4
        }

        public enum Frequency
        {
            [Description("Daily")]
            Daily = 1,

            [Description("Weekly")]
            Weekly = 2,

            [Description("Monthly")]
            Monthly = 3,

            [Description("Yearly")]
            Yearly = 4
        }

        public enum SIPStatus
        {
            [Description("Start")]
            Start = 1,

            [Description("Pause")]
            Pause = 2,

            [Description("Cancel")]
            Cancel = 3,

            [Description("Delete")]
            Delete = 4
        }

        public enum TransactionType
        {
            [Description("Credit")]
            Credit = 1,

            [Description("Debit")]
            Debit = 2
        }

        public enum EquityTransactionType
        {
            [Description("Buy")]
            Buy = 1,

            [Description("Sell")]
            Sell = 2,

            [Description("Dividend")]
            Dividend = 3,

            [Description("Split")]
            Split = 4,

            [Description("Bonus")]
            Bonus = 5
        }

        public enum PortfolioType
        {
            [Description("Stock")]
            Stock = 1,

            [Description("Mutual Fund")]
            MutualFund = 2
        }
    }
}
