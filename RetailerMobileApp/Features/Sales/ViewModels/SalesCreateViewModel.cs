using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailerMobileApp.ViewModels;

namespace RetailerMobileApp.Features.Sales.ViewModels;

public record SaleTypeOption(string Value, string Name);

public record SaleCustomerOption(int Id, string Name, decimal Balance);

public class SaleItemEntry : ObservableObject
{
    private readonly Action _onValueChanged;

    public SaleItemEntry(Action onValueChanged)
    {
        _onValueChanged = onValueChanged;
    }

    private int _serialNumber;
    public int SerialNumber
    {
        get => _serialNumber;
        set => SetProperty(ref _serialNumber, value);
    }

    private string _itemName = string.Empty;
    public string ItemName
    {
        get => _itemName;
        set
        {
            if (SetProperty(ref _itemName, value))
            {
                NotifyValueChanged();
            }
        }
    }

    private decimal _rate = 0m;
    public decimal Rate
    {
        get => _rate;
        set
        {
            if (SetProperty(ref _rate, value))
            {
                NotifyValueChanged();
            }
        }
    }

    private decimal _qty = 1m;
    public decimal Qty
    {
        get => _qty;
        set
        {
            if (SetProperty(ref _qty, value))
            {
                NotifyValueChanged();
            }
        }
    }

    private decimal _discountPercent = 0m;
    public decimal DiscountPercent
    {
        get => _discountPercent;
        set
        {
            if (SetProperty(ref _discountPercent, value))
            {
                NotifyValueChanged();
            }
        }
    }

    private decimal _gstPercent = 0m;
    public decimal GstPercent
    {
        get => _gstPercent;
        set
        {
            if (SetProperty(ref _gstPercent, value))
            {
                NotifyValueChanged();
            }
        }
    }

    private decimal _extraTaxPercent = 0m;
    public decimal ExtraTaxPercent
    {
        get => _extraTaxPercent;
        set
        {
            if (SetProperty(ref _extraTaxPercent, value))
            {
                NotifyValueChanged();
            }
        }
    }

    private decimal _furtherTaxPercent = 0m;
    public decimal FurtherTaxPercent
    {
        get => _furtherTaxPercent;
        set
        {
            if (SetProperty(ref _furtherTaxPercent, value))
            {
                NotifyValueChanged();
            }
        }
    }

    public decimal SubTotal => Math.Round(Rate * Qty, 2, MidpointRounding.AwayFromZero);

    public decimal DiscountAmount => Math.Round(SubTotal * (DiscountPercent / 100m), 2, MidpointRounding.AwayFromZero);

    public decimal TaxableAmount => SubTotal - DiscountAmount;

    public decimal GstAmount => Math.Round(TaxableAmount * (GstPercent / 100m), 2, MidpointRounding.AwayFromZero);

    public decimal ExtraTaxAmount => Math.Round(TaxableAmount * (ExtraTaxPercent / 100m), 2, MidpointRounding.AwayFromZero);

    public decimal FurtherTaxAmount => Math.Round(TaxableAmount * (FurtherTaxPercent / 100m), 2, MidpointRounding.AwayFromZero);

    public decimal LineTotal => Math.Round(TaxableAmount + GstAmount + ExtraTaxAmount + FurtherTaxAmount, 2, MidpointRounding.AwayFromZero);

    public void ForceUpdate() => NotifyValueChanged();

    private void NotifyValueChanged()
    {
        OnPropertyChanged(nameof(SubTotal));
        OnPropertyChanged(nameof(DiscountAmount));
        OnPropertyChanged(nameof(TaxableAmount));
        OnPropertyChanged(nameof(GstAmount));
        OnPropertyChanged(nameof(ExtraTaxAmount));
        OnPropertyChanged(nameof(FurtherTaxAmount));
        OnPropertyChanged(nameof(LineTotal));
        _onValueChanged?.Invoke();
    }
}

public partial class SalesCreateViewModel : BaseViewModel
{
    public ObservableCollection<SaleTypeOption> SaleTypes { get; } = new();

    public ObservableCollection<SaleCustomerOption> Customers { get; } = new();

    public ObservableCollection<SaleItemEntry> Items { get; } = new();

    public IRelayCommand AddItemCommand { get; }

    public IRelayCommand<SaleItemEntry?> RemoveItemCommand { get; }

    public IAsyncRelayCommand CancelCommand { get; }

    public IAsyncRelayCommand SaveSaleCommand { get; }

    private DateTime _saleDate = DateTime.Today;
    public DateTime SaleDate
    {
        get => _saleDate;
        set => SetProperty(ref _saleDate, value);
    }

    private SaleTypeOption? _selectedSaleType;
    public SaleTypeOption? SelectedSaleType
    {
        get => _selectedSaleType;
        set => SetProperty(ref _selectedSaleType, value);
    }

    private SaleCustomerOption? _selectedCustomer;
    public SaleCustomerOption? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            if (SetProperty(ref _selectedCustomer, value))
            {
                ClosingBalance = value?.Balance ?? 0m;
            }
        }
    }

    private decimal _closingBalance;
    public decimal ClosingBalance
    {
        get => _closingBalance;
        private set => SetProperty(ref _closingBalance, value);
    }

    private decimal _subTotal;
    public decimal SubTotal
    {
        get => _subTotal;
        private set => SetProperty(ref _subTotal, value);
    }

    private decimal _totalDiscount;
    public decimal TotalDiscount
    {
        get => _totalDiscount;
        private set => SetProperty(ref _totalDiscount, value);
    }

    private decimal _totalGst;
    public decimal TotalGst
    {
        get => _totalGst;
        private set => SetProperty(ref _totalGst, value);
    }

    private decimal _totalEdTax;
    public decimal TotalEdTax
    {
        get => _totalEdTax;
        private set => SetProperty(ref _totalEdTax, value);
    }

    private decimal _totalFedTax;
    public decimal TotalFedTax
    {
        get => _totalFedTax;
        private set => SetProperty(ref _totalFedTax, value);
    }

    private decimal _grandTotal;
    public decimal GrandTotal
    {
        get => _grandTotal;
        private set => SetProperty(ref _grandTotal, value);
    }

    private int _totalItems;
    public int TotalItems
    {
        get => _totalItems;
        private set => SetProperty(ref _totalItems, value);
    }

    public SalesCreateViewModel()
    {
        Title = "Create Sale";
        LoadReferenceData();
        Items.CollectionChanged += (_, _) =>
        {
            RefreshSerialNumbers();
            RecalculateTotals();
        };
        AddItem();

        AddItemCommand = new RelayCommand(AddItem);
        RemoveItemCommand = new RelayCommand<SaleItemEntry?>(RemoveItem);
        CancelCommand = new AsyncRelayCommand(CancelAsync);
        SaveSaleCommand = new AsyncRelayCommand(SaveSaleAsync);
    }

    private void AddItem()
    {
        var entry = new SaleItemEntry(RecalculateTotals)
        {
            ItemName = string.Empty,
            Rate = 0m,
            Qty = 1m
        };
        entry.ForceUpdate();
        Items.Add(entry);
    }

    private void RemoveItem(SaleItemEntry? entry)
    {
        if (entry is null)
        {
            return;
        }

        Items.Remove(entry);
    }

    private Task CancelAsync()
    {
        return MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Shell.Current.GoToAsync("..").ConfigureAwait(false);
        });
    }

    private Task SaveSaleAsync()
    {
        return ExecuteBusyActionAsync(async () =>
        {
            await Task.Delay(300).ConfigureAwait(false);

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.DisplayAlert("Sale saved", "This is a placeholder action.", "OK").ConfigureAwait(false);
                await Shell.Current.GoToAsync("..").ConfigureAwait(false);
            }).ConfigureAwait(false);
        });
    }

    private void LoadReferenceData()
    {
        if (SaleTypes.Count == 0)
        {
            SaleTypes.Add(new SaleTypeOption("retail", "Retail"));
            SaleTypes.Add(new SaleTypeOption("wholesale", "Wholesale"));
            SaleTypes.Add(new SaleTypeOption("counter", "Counter Sale"));
        }

        if (Customers.Count == 0)
        {
            Customers.Add(new SaleCustomerOption(1, "Walk-in Customer", 0m));
            Customers.Add(new SaleCustomerOption(2, "Corporate Supplies", 1520.75m));
            Customers.Add(new SaleCustomerOption(3, "Downtown Mart", -320.10m));
        }

        SelectedSaleType = SaleTypes.FirstOrDefault();
        SelectedCustomer = Customers.FirstOrDefault();
    }

    private void RefreshSerialNumbers()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            Items[i].SerialNumber = i + 1;
        }
    }

    private void RecalculateTotals()
    {
        if (Items.Count == 0)
        {
            SubTotal = 0m;
            TotalDiscount = 0m;
            TotalGst = 0m;
            TotalEdTax = 0m;
            TotalFedTax = 0m;
            GrandTotal = 0m;
            TotalItems = 0;
            return;
        }

        SubTotal = Items.Sum(i => i.SubTotal);
        TotalDiscount = Items.Sum(i => i.DiscountAmount);
        TotalGst = Items.Sum(i => i.GstAmount);
        TotalEdTax = Items.Sum(i => i.ExtraTaxAmount);
        TotalFedTax = Items.Sum(i => i.FurtherTaxAmount);
        GrandTotal = Items.Sum(i => i.LineTotal);
        TotalItems = Items.Count;
    }
}
