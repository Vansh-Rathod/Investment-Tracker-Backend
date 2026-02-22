using ClosedXML.Excel;
using Core.ViewModels;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EquityTrackerWebAPI.Services
{
    public class ExportService : IExportService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IMutualFundRepository _mutualFundRepository;
        private readonly ITransactionRepository _transactionRepository;

        public ExportService(
            IStockRepository stockRepository,
            IMutualFundRepository mutualFundRepository,
            ITransactionRepository transactionRepository)
        {
            _stockRepository = stockRepository;
            _mutualFundRepository = mutualFundRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<(byte[] Content, string FileName)> ExportToExcelAsync(int userId, string exportType)
        {
            var type = (exportType ?? "all").ToLowerInvariant();
            using var workbook = new XLWorkbook();

            if (type == "all" || type == "stocks")
            {
                var holdingsResult = await _stockRepository.GetStockHoldings(userId);
                if (holdingsResult.Success && holdingsResult.Data != null && holdingsResult.Data.Any())
                    AddStockHoldingsSheet(workbook, holdingsResult.Data);
                if (type == "stocks")
                {
                    var txResult = await _transactionRepository.GetUserTransactionsAsync(userId, assetTypeId: 1);
                    if (txResult.Success && txResult.Data != null && txResult.Data.Any())
                        AddTransactionsSheet(workbook, txResult.Data, "Stock Transactions");
                }
            }

            if (type == "all" || type == "mutual-funds")
            {
                var mfResult = await _mutualFundRepository.GetMutualFundHoldings(userId);
                if (mfResult.Success && mfResult.Data != null && mfResult.Data.Any())
                    AddMutualFundHoldingsSheet(workbook, mfResult.Data);
                if (type == "mutual-funds")
                {
                    var txResult = await _transactionRepository.GetUserTransactionsAsync(userId, assetTypeId: 2);
                    if (txResult.Success && txResult.Data != null && txResult.Data.Any())
                        AddTransactionsSheet(workbook, txResult.Data, "Mutual Fund Transactions");
                }
            }

            if (type == "all")
            {
                var txResult = await _transactionRepository.GetUserTransactionsAsync(userId);
                if (txResult.Success && txResult.Data != null && txResult.Data.Any())
                    AddTransactionsSheet(workbook, txResult.Data, "All Transactions");
            }

            if (workbook.Worksheets.Count == 0)
            {
                var emptySheet = workbook.Worksheets.Add("Export");
                emptySheet.Cell(1, 1).Value = "No data available for the selected export type.";
            }

            string fileName = $"InvestmentExport_{type}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            using var stream = new System.IO.MemoryStream();
            workbook.SaveAs(stream, false);
            return (stream.ToArray(), fileName);
        }

        private static void AddStockHoldingsSheet(XLWorkbook workbook, List<UserStockHoldingViewModel> data)
        {
            var sheet = workbook.Worksheets.Add("Stock Holdings");
            sheet.Cell(1, 1).Value = "Stock Name";
            sheet.Cell(1, 2).Value = "Symbol";
            sheet.Cell(1, 3).Value = "Exchange";
            sheet.Cell(1, 4).Value = "Quantity";
            sheet.Cell(1, 5).Value = "Average Price";
            sheet.Cell(1, 6).Value = "Current Price";
            sheet.Cell(1, 7).Value = "Invested Amount";
            sheet.Cell(1, 8).Value = "Current Value";
            sheet.Cell(1, 9).Value = "Total Return";
            sheet.Cell(1, 10).Value = "Total Return %";
            sheet.Range(1, 1, 1, 10).Style.Font.Bold = true;

            int row = 2;
            foreach (var h in data)
            {
                sheet.Cell(row, 1).Value = h.StockName;
                sheet.Cell(row, 2).Value = h.Symbol;
                sheet.Cell(row, 3).Value = h.ExchangeName;
                sheet.Cell(row, 4).Value = h.Quantity;
                sheet.Cell(row, 5).Value = h.AveragePrice;
                sheet.Cell(row, 6).Value = h.CurrentPrice;
                sheet.Cell(row, 7).Value = h.InvestedAmount;
                sheet.Cell(row, 8).Value = h.CurrentValue;
                sheet.Cell(row, 9).Value = h.TotalReturn;
                sheet.Cell(row, 10).Value = h.TotalReturnPercentage;
                row++;
            }
            sheet.Columns().AdjustToContents();
        }

        private static void AddMutualFundHoldingsSheet(XLWorkbook workbook, List<MutualFundHoldingViewModel> data)
        {
            var sheet = workbook.Worksheets.Add("Mutual Fund Holdings");
            sheet.Cell(1, 1).Value = "Fund Name";
            sheet.Cell(1, 2).Value = "AMC";
            sheet.Cell(1, 3).Value = "Category";
            sheet.Cell(1, 4).Value = "Units Held";
            sheet.Cell(1, 5).Value = "Average NAV";
            sheet.Cell(1, 6).Value = "Current NAV";
            sheet.Cell(1, 7).Value = "Invested Amount";
            sheet.Cell(1, 8).Value = "Current Value";
            sheet.Cell(1, 9).Value = "Absolute Return";
            sheet.Cell(1, 10).Value = "Absolute Return %";
            sheet.Range(1, 1, 1, 10).Style.Font.Bold = true;

            int row = 2;
            foreach (var h in data)
            {
                sheet.Cell(row, 1).Value = h.FundName;
                sheet.Cell(row, 2).Value = h.AMCCode;
                sheet.Cell(row, 3).Value = h.CategoryName;
                sheet.Cell(row, 4).Value = h.UnitsHeld;
                sheet.Cell(row, 5).Value = h.AverageNAV;
                sheet.Cell(row, 6).Value = h.CurrentNAV;
                sheet.Cell(row, 7).Value = h.InvestedAmount;
                sheet.Cell(row, 8).Value = h.CurrentValue;
                sheet.Cell(row, 9).Value = h.AbsoluteReturn;
                sheet.Cell(row, 10).Value = h.AbsoluteReturnPercentage;
                row++;
            }
            sheet.Columns().AdjustToContents();
        }

        private static void AddTransactionsSheet(XLWorkbook workbook, List<TransactionViewModel> data, string sheetName)
        {
            var sheet = workbook.Worksheets.Add(sheetName);
            sheet.Cell(1, 1).Value = "Date";
            sheet.Cell(1, 2).Value = "Asset Type";
            sheet.Cell(1, 3).Value = "Asset Name";
            sheet.Cell(1, 4).Value = "Type";
            sheet.Cell(1, 5).Value = "Units";
            sheet.Cell(1, 6).Value = "Price";
            sheet.Cell(1, 7).Value = "Amount";
            sheet.Cell(1, 8).Value = "Source";
            sheet.Range(1, 1, 1, 8).Style.Font.Bold = true;

            string TxType(int t) => t switch { 1 => "Buy", 2 => "Sell", 3 => "Dividend", 4 => "Split", 5 => "Bonus", _ => t.ToString() };

            int row = 2;
            foreach (var t in data.OrderByDescending(x => x.TransactionDate))
            {
                sheet.Cell(row, 1).Value = t.TransactionDate;
                sheet.Cell(row, 2).Value = t.AssetTypeName;
                sheet.Cell(row, 3).Value = t.AssetName;
                sheet.Cell(row, 4).Value = TxType(t.TransactionType);
                sheet.Cell(row, 5).Value = t.Units;
                sheet.Cell(row, 6).Value = t.Price;
                sheet.Cell(row, 7).Value = t.Amount;
                sheet.Cell(row, 8).Value = t.SourceType ?? "";
                row++;
            }
            sheet.Columns().AdjustToContents();
        }
    }
}
